using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.OleDb;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Text.RegularExpressions;

namespace Ecam.Framework {
	public class ExcelConnection {

		#region Constants
		private const string EXCELDATABASE_BY_KEY = "ExcelDatabase-{0}";
		#endregion

		public static DataSet GetDataSet(string path, string fileName, ref string errorMessage, ref string sessionKey) {
			ICacheManager cacheManager = new MemoryCacheManager();
			DataSet ds = new DataSet();
			PagingDataTable dt = null;
			try {
				string inputFileName = System.IO.Path.Combine(path, fileName);
				using (SpreadsheetDocument myWorkbook = SpreadsheetDocument.Open(inputFileName, false)) {
					//Access the main Workbook part, which contains data
					WorkbookPart workbookPart = myWorkbook.WorkbookPart;
					WorksheetPart worksheetPart = null;
					List<Sheet> sheets = workbookPart.Workbook.Descendants<Sheet>().ToList();
					foreach (var ss in sheets) {
						dt = new PagingDataTable();
						dt.TableName = ss.Name;
						worksheetPart = (WorksheetPart)workbookPart.GetPartById(ss.Id);
						SharedStringTablePart stringTablePart = workbookPart.SharedStringTablePart;
						if (worksheetPart != null) {
							string relationshipId = sheets.First().Id.Value;
							Worksheet workSheet = worksheetPart.Worksheet;
							SheetData sheetData = workSheet.GetFirstChild<SheetData>();
							IEnumerable<Row> rows = sheetData.Descendants<Row>();
							if (rows.ToArray().Count() > 0) {
								foreach (Cell cell in rows.ElementAt(0)) {
									dt.Columns.Add(GetCellValue(myWorkbook, cell));
								}

								int rowIndex = 0;
								foreach (Row row in rows) //this will also include your header row...
							{
									if (rowIndex > 0) {
										DataRow tempRow = dt.NewRow();
										int columnIndex = 0;
										foreach (Cell cell in row.Descendants<Cell>()) {
											// Gets the column index of the cell with data
											int cellColumnIndex = (int)GetColumnIndexFromName(GetColumnName(cell.CellReference));
											cellColumnIndex--; //zero based index
											if (columnIndex < cellColumnIndex) {
												do {
													try {
														tempRow[columnIndex] = ""; //Insert blank data here;
													} catch { }
													columnIndex++;
												}
												while (columnIndex < cellColumnIndex);
											}
											try {
												tempRow[columnIndex] = GetCellValue(myWorkbook, cell);
											} catch { }
											columnIndex++;
										}
										bool isAllColumnBlank = true;
										foreach (DataColumn col in dt.Columns) {
											if (string.IsNullOrEmpty(Convert.ToString(tempRow[col.ColumnName])) == false) {
												isAllColumnBlank = false;
												break;
											}
										}
										if (isAllColumnBlank == false) {
											dt.Rows.Add(tempRow);
										}
									}
									rowIndex++;
								}
								dt.Columns.Add(new DataColumn {
									DataType = typeof(int),
									//AutoIncrement = true,
									//AutoIncrementSeed = 1,
									//AutoIncrementStep = 1,
									ColumnName = "RowNumber",
									//AllowDBNull = false,
								});
								dt.Columns.Add(new DataColumn {
									ColumnName = "ImportError",
								});
								rowIndex = 1;
								foreach (DataRow row in dt.Rows) {
									row["RowNumber"] = rowIndex;
									rowIndex++;
								}
								ds.Tables.Add(dt);
							}
						}
					}
				}
				Guid guid = System.Guid.NewGuid();
				sessionKey = string.Format(EXCELDATABASE_BY_KEY, guid);
				cacheManager.Set(sessionKey, ds, 120);
			} catch (Exception ex) {
				errorMessage = ex.Message.ToString();
			} finally {
                UploadFileHelper.DeleteFile(Helper.TempPathSettingName,fileName);
			}
			return ds;
		}

		/// <summary>
		/// Given a cell name, parses the specified cell to get the column name.
		/// </summary>
		/// <param name="cellReference">Address of the cell (ie. B2)</param>
		/// <returns>Column Name (ie. B)</returns>
		public static string GetColumnName(string cellReference) {
			// Create a regular expression to match the column name portion of the cell name.
			Regex regex = new Regex("[A-Za-z]+");
			Match match = regex.Match(cellReference);
			return match.Value;
		}

		/// <summary>
		/// Given just the column name (no row index), it will return the zero based column index.
		/// Note: This method will only handle columns with a length of up to two (ie. A to Z and AA to ZZ). 
		/// A length of three can be implemented when needed.
		/// </summary>
		/// <param name="columnName">Column Name (ie. A or AB)</param>
		/// <returns>Zero based index if the conversion was successful; otherwise null</returns>
		public static int? GetColumnIndexFromName(string columnName) {

			//return columnIndex;
			string name = columnName;
			int number = 0;
			int pow = 1;
			for (int i = name.Length - 1; i >= 0; i--) {
				number += (name[i] - 'A' + 1) * pow;
				pow *= 26;
			}
			return number;
		}


		public static string GetCellValue(SpreadsheetDocument document, Cell cell) {
			SharedStringTablePart stringTablePart = document.WorkbookPart.SharedStringTablePart;
			if (cell.CellValue == null) {
				return "";
			}
			string value = cell.CellValue.InnerXml;
			if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString) {
				return stringTablePart.SharedStringTable.ChildElements[Int32.Parse(value)].InnerText;
			} else {
				return value;
			}
		}

		private static string GetValue(Cell cell, SharedStringTablePart stringTablePart) {
			if (cell.ChildElements.Count == 0) return null;
			//get cell value
			string c = cell.CellValue.ToString();
			string value = cell.ElementAt(0).InnerText;//CellValue.InnerText;
			//Look up real value from shared string table
			if ((cell.DataType != null) && (cell.DataType == CellValues.SharedString))
				value = stringTablePart.SharedStringTable.ChildElements[Int32.Parse(value)].InnerText;

			return value;
		}

		public static DataSet ImportExcelDataset(string key) {
			ICacheManager cacheManager = new MemoryCacheManager();
			return cacheManager.Get<DataSet>(key);
		}

		/*
		public static DataSet GetDataSet_(string path,string fileName,ref string errorMessage,ref string sessionKey) {
			ICacheManager cacheManager=new MemoryCacheManager();
			DataSet ds=new DataSet();
			PagingDataTable table=null;
			try {
				//string connectionString = "provider=Microsoft.ACE.OLEDB.12.0;data source='" + fileName + "';Extended Properties=Excel 12.0;";

				string connectionString=string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0 Xml;HDR=YES\";",fileName);
				using(OleDbConnection connection=new OleDbConnection(connectionString)) {
					connection.Open();
					DataTable schema=connection.GetSchema("Tables");
					foreach(DataRow row in schema.Rows) {
						string tableName=row["TABLE_NAME"].ToString();
						string tableDisplayName=tableName.Replace("$","").Replace("'","");
						using(OleDbCommand command=new OleDbCommand(string.Format("select * from [{0}]",tableName),connection)) {
							table=new PagingDataTable();
							table.TableName=tableDisplayName;
							table.Load(command.ExecuteReader());
							ds.Tables.Add(table);
						}
					}
					Guid guid=System.Guid.NewGuid();
					sessionKey=string.Format(EXCELDATABASE_BY_KEY,guid);
					cacheManager.Set(sessionKey,ds,15);
				}
			} catch(Exception ex) {
				errorMessage=ex.Message.ToString();
			}
			return ds;
		}
		*/
	}
}
