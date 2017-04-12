using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace Ecam.Framework {
	public class SLExcelReader {
		private string GetColumnName(string cellReference) {
			var regex = new Regex("[A-Za-z]+");
			var match = regex.Match(cellReference);

			return match.Value;
		}

		private int ConvertColumnNameToNumber(string columnName) {
			var alpha = new Regex("^[A-Z]+$");
			if(!alpha.IsMatch(columnName)) throw new ArgumentException();

			char[] colLetters = columnName.ToCharArray();
			Array.Reverse(colLetters);

			var convertedValue = 0;
			for(int i = 0;i < colLetters.Length;i++) {
				char letter = colLetters[i];
				int current = i == 0 ? letter - 65 : letter - 64; // ASCII 'A' = 65
				convertedValue += current * (int)Math.Pow(26,i);
			}

			return convertedValue;
		}

		private IEnumerator<Cell> GetExcelCellEnumerator(Row row) {
			int currentCount = 0;
			foreach(Cell cell in row.Descendants<Cell>()) {
				string columnName = GetColumnName(cell.CellReference);

				int currentColumnIndex = ConvertColumnNameToNumber(columnName);

				for(;currentCount < currentColumnIndex;currentCount++) {
					var emptycell = new Cell() { DataType = null,CellValue = new CellValue(string.Empty) };
					yield return emptycell;
				}

				yield return cell;
				currentCount++;
			}
		}

		private string ReadExcelCell(Cell cell,WorkbookPart workbookPart) {
			var cellValue = cell.CellValue;
			var text = (cellValue == null) ? cell.InnerText : cellValue.Text;
			if((cell.DataType != null) && (cell.DataType == CellValues.SharedString)) {
				text = workbookPart.SharedStringTablePart.SharedStringTable.Elements<SharedStringItem>().ElementAt(
						Convert.ToInt32(cell.CellValue.Text)).InnerText;
			}
			if(string.IsNullOrEmpty(text) == false) {
				if(text.EndsWith(".0") == true) {
					text = text.Replace(".0","");
				}
			}
			return (text ?? string.Empty).Trim();
		}

		public SLExcelData ReadExcel(string filePath) {
			var data = new SLExcelData();
			bool isNotEmptyRow = false;
			string temp = string.Empty;
			// Check if the file is excel
			//if (file.ContentLength <= 0)
			//{
			//	data.Status.Message = "You uploaded an empty file";
			//	return data;
			//}

			//if (file.ContentType != "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
			//{
			//    data.Status.Message = "Please upload a valid excel file of version 2007 and above";
			//    return data;
			//}

			// Open the excel document
			WorkbookPart workbookPart; List<Row> rows;
			try {
				using(var document = SpreadsheetDocument.Open(filePath,false)) {
					workbookPart = document.WorkbookPart;

					var sheets = workbookPart.Workbook.Descendants<Sheet>();
					var sheet = sheets.First();
					data.SheetName = sheet.Name;

					var workSheet = ((WorksheetPart)workbookPart.GetPartById(sheet.Id)).Worksheet;
					var columns = workSheet.Descendants<Columns>().FirstOrDefault();
					data.ColumnConfigurations = columns;

					var sheetData = workSheet.Elements<SheetData>().First();
					rows = sheetData.Elements<Row>().ToList();

					int columnCount = 0;
					// Read the header
					if(rows.Count > 0) {
						var row = rows[0];
						var cellEnumerator = GetExcelCellEnumerator(row);
						while(cellEnumerator.MoveNext()) {
							var cell = cellEnumerator.Current;
							var text = ReadExcelCell(cell,workbookPart).Trim();
							if(text.ToLower() != "error"
								&& text.ToLower() != "issuccessrow"
								&& text.ToLower() != "warning"
								&& string.IsNullOrEmpty(text) == false) {
								data.Headers.Add(text);
								columnCount += 1;
							}
						}
					}

					if(data.Headers.Count() > 0) {
						data.Headers.Add("Error");
						data.Headers.Add("IsSuccessRow");
						data.Headers.Add("Warning");
					}

					// Read the sheet data
					if(rows.Count > 1) {
						for(var i = 1;i < rows.Count;i++) {

							var dataRow = new List<string>();
							var row = rows[i];
							var cellEnumerator = GetExcelCellEnumerator(row);
							int cellCount = 0;
							while(cellEnumerator.MoveNext()) {
								cellCount += 1;
								var cell = cellEnumerator.Current;
								var text = ReadExcelCell(cell,workbookPart).Trim();
								if(cellCount <= columnCount) {
									dataRow.Add(text);
								} else {
									break;
								}
							}
							if(cellCount < columnCount) {
								int k;
								int cecount = cellCount;
								for(k = cecount;k < columnCount;k++) {
									dataRow.Add("");
									cellCount += 1;
								}
							}
							int additionalColCount = 3;
							int z;
							for(z = 0;z < additionalColCount;z++) {
								dataRow.Add("");
							}
							if(cellCount >= columnCount) {
								isNotEmptyRow = false;
								temp = string.Empty;
								foreach(string dr in dataRow) {
									temp = dr;
									if(string.IsNullOrEmpty(temp) == false) {
										temp = temp.Trim();
									}
									if(string.IsNullOrEmpty(temp) == false
										&& isNotEmptyRow == false) {
										isNotEmptyRow = true;
									}
								}
								if(isNotEmptyRow == true) {
									data.DataRows.Add(dataRow);
								}
							}
						}
					}
				}
			} catch(Exception e) {
				data.Status.Message = "Unable to open the file = " + e.Message;
				return data;
			}
			return data;
		}
	}
}