﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Data;
using DocumentFormat.OpenXml.Packaging;
using System.Reflection;
using System.Diagnostics;
using DocumentFormat.OpenXml;


namespace Ecam.Framework {

    public class CreateExcelFile {
        public static bool CreateExcelDocument<T>(List<T> list,string xlsxFilePath) {
            DataSet ds = new DataSet();
            ds.Tables.Add(ListToDataTable(list));

            return CreateExcelDocument(ds,xlsxFilePath);
        }
        #region HELPER_FUNCTIONS
        //  My thanks to Carl Quirion, for making it "nullable-friendly".
        public static DataTable ListToDataTable<T>(List<T> list) {
            DataTable dt = new DataTable();

            foreach(PropertyInfo info in typeof(T).GetProperties()) {
                dt.Columns.Add(new DataColumn(info.Name,GetNullableType(info.PropertyType)));
            }
            foreach(T t in list) {
                DataRow row = dt.NewRow();
                foreach(PropertyInfo info in typeof(T).GetProperties()) {
                    if(!IsNullableType(info.PropertyType))
                        row[info.Name] = info.GetValue(t,null);
                    else
                        row[info.Name] = (info.GetValue(t,null) ?? DBNull.Value);
                }
                dt.Rows.Add(row);
            }
            return dt;
        }
        private static Type GetNullableType(Type t) {
            Type returnType = t;
            if(t.IsGenericType && t.GetGenericTypeDefinition().Equals(typeof(Nullable<>))) {
                returnType = Nullable.GetUnderlyingType(t);
            }
            return returnType;
        }
        private static bool IsNullableType(Type type) {
            return (type == typeof(string) ||
                    type.IsArray ||
                    (type.IsGenericType &&
                     type.GetGenericTypeDefinition().Equals(typeof(Nullable<>))));
        }

        public static bool CreateExcelDocument(DataTable dt,string xlsxFilePath) {
            DataSet ds = new DataSet();
            ds.Tables.Add(dt);
            bool result = CreateExcelDocument(ds,xlsxFilePath);
            ds.Tables.Remove(dt);
            return result;
        }
        #endregion

#if INCLUDE_WEB_FUNCTIONS
        /// <summary>
        /// Create an Excel file, and write it out to a MemoryStream (rather than directly to a file)
        /// </summary>
        /// <param name="dt">DataTable containing the data to be written to the Excel.</param>
        /// <param name="filename">The filename (without a path) to call the new Excel file.</param>
        /// <param name="Response">HttpResponse of the current page.</param>
        /// <returns>True if it was created succesfully, otherwise false.</returns>
        public static bool CreateExcelDocument(DataTable dt, string filename, System.Web.HttpResponse Response)
        {
            try
            {
                DataSet ds = new DataSet();
                ds.Tables.Add(dt);
                CreateExcelDocumentAsStream(ds, filename, Response);
                ds.Tables.Remove(dt);
                return true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Failed, exception thrown: " + ex.Message);
                return false;
            }
        }

        public static bool CreateExcelDocument<T>(List<T> list, string filename, System.Web.HttpResponse Response)
        {
            try
            {
                DataSet ds = new DataSet();
                ds.Tables.Add(ListToDataTable(list));
                CreateExcelDocumentAsStream(ds, filename, Response);
                return true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Failed, exception thrown: " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Create an Excel file, and write it out to a MemoryStream (rather than directly to a file)
        /// </summary>
        /// <param name="ds">DataSet containing the data to be written to the Excel.</param>
        /// <param name="filename">The filename (without a path) to call the new Excel file.</param>
        /// <param name="Response">HttpResponse of the current page.</param>
        /// <returns>Either a MemoryStream, or NULL if something goes wrong.</returns>
        public static bool CreateExcelDocumentAsStream(DataSet ds, string filename, System.Web.HttpResponse Response)
        {
            try
            {
                System.IO.MemoryStream stream = new System.IO.MemoryStream();
                using (SpreadsheetDocument document = SpreadsheetDocument.Create(stream, SpreadsheetDocumentType.Workbook, true))
                {
                    WriteExcelFile(ds, document);
                }
                stream.Flush();
                stream.Position = 0;

                Response.ClearContent();
                Response.Clear();
                Response.Buffer = true;
                Response.Charset = "";

                //  NOTE: If you get an "HttpCacheability does not exist" error on the following line, make sure you have
                //  manually added System.Web to this project's References.

                Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);
                Response.AddHeader("content-disposition", "attachment; filename=" + filename);
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                byte[] data1 = new byte[stream.Length];
                stream.Read(data1, 0, data1.Length);
                stream.Close();
                Response.BinaryWrite(data1);
                Response.Flush();
                Response.End();

                return true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Failed, exception thrown: " + ex.Message);
                return false;
            }
        }
#endif      //  End of "INCLUDE_WEB_FUNCTIONS" section

        /// <summary>
        /// Create an Excel file, and write it to a file.
        /// </summary>
        /// <param name="ds">DataSet containing the data to be written to the Excel.</param>
        /// <param name="excelFilename">Name of file to be written.</param>
        /// <returns>True if successful, false if something went wrong.</returns>
        public static bool CreateExcelDocument(DataSet ds,string excelFilename,int tabIndex = -1,string redColorColumnName = "") {
            try {
                using(SpreadsheetDocument document = SpreadsheetDocument.Create(excelFilename,SpreadsheetDocumentType.Workbook)) {
                    WriteExcelFile(ds,document,tabIndex,redColorColumnName);
                }
                //Trace.WriteLine("Successfully created: " + excelFilename);
                return true;
            } catch(Exception ex) {
                //Trace.WriteLine("Failed, exception thrown: " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Create an Excel file, and write it to a file.
        /// </summary>
        /// <param name="ds">DataSet containing the data to be written to the Excel.</param>
        /// <param name="excelFilename">Name of file to be written.</param>
        /// <returns>True if successful, false if something went wrong.</returns>
        public static byte[] CreateExcelDocumentBytes(DataSet ds,int tabIndex = -1,string redColorColumnName = "") {
            try {
                byte[] bytes = null;
                string excelFilename = UploadFileHelper.GetTempFolderPath() + "\\" + (new Guid()).ToString() + ".xlsx";
                using(SpreadsheetDocument document = SpreadsheetDocument.Create(excelFilename,SpreadsheetDocumentType.Workbook)) {
                    WriteExcelFile(ds,document,tabIndex,redColorColumnName);
                }
                if(System.IO.File.Exists(excelFilename) == true) {
                    bytes = System.IO.File.ReadAllBytes(excelFilename);
                    System.IO.File.Delete(excelFilename);
                }
                //Trace.WriteLine("Successfully created: " + excelFilename);
                return bytes;
            } catch(Exception ex) {
                //Trace.WriteLine("Failed, exception thrown: " + ex.Message);
                return null;
            }
        }

        private static void WriteExcelFile(DataSet ds,SpreadsheetDocument spreadsheet,int tabIndex = -1,string redColorColumnName = "") {
            //  Create the Excel file contents.  This function is used when creating an Excel file either writing 
            //  to a file, or writing to a MemoryStream.
            spreadsheet.AddWorkbookPart();
            spreadsheet.WorkbookPart.Workbook = new DocumentFormat.OpenXml.Spreadsheet.Workbook();

            //  My thanks to James Miera for the following line of code (which prevents crashes in Excel 2010)
            spreadsheet.WorkbookPart.Workbook.Append(new BookViews(new WorkbookView()));

            //  If we don't add a "WorkbookStylesPart", OLEDB will refuse to connect to this .xlsx file !
            WorkbookStylesPart workbookStylesPart = spreadsheet.WorkbookPart.AddNewPart<WorkbookStylesPart>("rIdStyles");
            Stylesheet stylesheet = new Stylesheet();
            workbookStylesPart.Stylesheet = stylesheet;

            //  Loop through each of the DataTables in our DataSet, and create a new Excel Worksheet for each.
            uint worksheetNumber = 1;
            int tableIndex = -1;
            foreach(DataTable dt in ds.Tables) {
                tableIndex += 1;
                //  For each worksheet you want to create
                string workSheetID = "rId" + worksheetNumber.ToString();
                string worksheetName = dt.TableName;

                WorksheetPart newWorksheetPart = spreadsheet.WorkbookPart.AddNewPart<WorksheetPart>();
                newWorksheetPart.Worksheet = new DocumentFormat.OpenXml.Spreadsheet.Worksheet();

                // create sheet data
                newWorksheetPart.Worksheet.AppendChild(new DocumentFormat.OpenXml.Spreadsheet.SheetData());

                if(tableIndex != tabIndex) {
                    redColorColumnName = string.Empty;
                }

                // save worksheet
                WriteDataTableToExcelWorksheet(dt,newWorksheetPart,redColorColumnName);
                newWorksheetPart.Worksheet.Save();

                // create the worksheet to workbook relation
                if(worksheetNumber == 1)
                    spreadsheet.WorkbookPart.Workbook.AppendChild(new DocumentFormat.OpenXml.Spreadsheet.Sheets());

                spreadsheet.WorkbookPart.Workbook.GetFirstChild<DocumentFormat.OpenXml.Spreadsheet.Sheets>().AppendChild(new DocumentFormat.OpenXml.Spreadsheet.Sheet() {
                    Id = spreadsheet.WorkbookPart.GetIdOfPart(newWorksheetPart),
                    SheetId = (uint)worksheetNumber,
                    Name = dt.TableName
                });

                worksheetNumber++;
            }

            spreadsheet.WorkbookPart.Workbook.Save();
        }

        private static void WriteDataTableToExcelWorksheet(DataTable dt,WorksheetPart worksheetPart,string redColorColumnName = "") {
            var worksheet = worksheetPart.Worksheet;
            var sheetData = worksheet.GetFirstChild<SheetData>();

            string cellValue = "";

            //  Create a Header Row in our Excel file, containing one header for each Column of data in our DataTable.
            //
            //  We'll also create an array, showing which type each column of data is (Text or Numeric), so when we come to write the actual
            //  cells of data, we'll know if to write Text values or Numeric cell values.
            int numberOfColumns = dt.Columns.Count;
            bool[] IsNumericColumn = new bool[numberOfColumns];

            string[] excelColumnNames = new string[numberOfColumns];
            for(int n = 0;n < numberOfColumns;n++)
                excelColumnNames[n] = GetExcelColumnName(n);

            //
            //  Create the Header row in our Excel Worksheet
            //
            uint rowIndex = 1;

            var headerRow = new Row { RowIndex = rowIndex };  // add a row at the top of spreadsheet
            sheetData.Append(headerRow);

            for(int colInx = 0;colInx < numberOfColumns;colInx++) {
                DataColumn col = dt.Columns[colInx];
                AppendTextCell(excelColumnNames[colInx] + "1",col.ColumnName,headerRow,false);
                IsNumericColumn[colInx] = (col.DataType.FullName == "System.Decimal") || (col.DataType.FullName == "System.Int32");
            }

            //
            //  Now, step through each row of data in our DataTable...
            //
            double cellNumericValue = 0;
            foreach(DataRow dr in dt.Rows) {
                // ...create a new row, and append a set of this row's data to it.
                ++rowIndex;
                var newExcelRow = new Row { RowIndex = rowIndex };  // add a row at the top of spreadsheet
                sheetData.Append(newExcelRow);

                for(int colInx = 0;colInx < numberOfColumns;colInx++) {

                    bool checkRedColorColumn = false;
                    if(dt.Columns[colInx].ColumnName == redColorColumnName) {
                        checkRedColorColumn = true;
                    }

                    cellValue = dr.ItemArray[colInx].ToString();

                    // Create cell with data
                    if(IsNumericColumn[colInx]) {
                        //  For numeric cells, make sure our input data IS a number, then write it out to the Excel file.
                        //  If this numeric value is NULL, then don't write anything to the Excel file.
                        cellNumericValue = 0;
                        if(double.TryParse(cellValue,out cellNumericValue)) {
                            cellValue = cellNumericValue.ToString();
                            AppendNumericCell(excelColumnNames[colInx] + rowIndex.ToString(),cellValue,newExcelRow);
                        }
                    } else {
                        //  For text cells, just write the input data straight out to the Excel file.
                        AppendTextCell(excelColumnNames[colInx] + rowIndex.ToString(),cellValue,newExcelRow,checkRedColorColumn);
                    }
                }
            }
        }

        private static void AppendTextCell(string cellReference,string cellStringValue,Row excelRow,bool checkRedColorColumn) {
            //  Add a new Excel Cell to our Row 
            Cell cell = new Cell() { CellReference = cellReference,DataType = CellValues.String };
            CellValue cellValue = new CellValue();
            cellValue.Text = cellStringValue;
            cell.Append(cellValue);
            excelRow.Append(cell);
        }

        private static void AppendNumericCell(string cellReference,string cellStringValue,Row excelRow) {
            //  Add a new Excel Cell to our Row 
            Cell cell = new Cell() { CellReference = cellReference };
            CellValue cellValue = new CellValue();
            cellValue.Text = cellStringValue;
            cell.Append(cellValue);
            excelRow.Append(cell);
        }

        private static string GetExcelColumnName(int columnIndex) {
            //  Convert a zero-based column index into an Excel column reference  (A, B, C.. Y, Y, AA, AB, AC... AY, AZ, B1, B2..)
            //
            //  eg  GetExcelColumnName(0) should return "A"
            //      GetExcelColumnName(1) should return "B"
            //      GetExcelColumnName(25) should return "Z"
            //      GetExcelColumnName(26) should return "AA"
            //      GetExcelColumnName(27) should return "AB"
            //      ..etc..
            //
            if(columnIndex < 26)
                return ((char)('A' + columnIndex)).ToString();

            char firstChar = (char)('A' + (columnIndex / 26) - 1);
            char secondChar = (char)('A' + (columnIndex % 26));

            return string.Format("{0}{1}",firstChar,secondChar);
        }

        private static Stylesheet GenerateStyleSheet() {
            return new Stylesheet(
                 new Fonts(
                     new Font(                                                               // Index 0 - The default font.
                         new FontSize() { Val = 11 },
                         new Color() { Rgb = new HexBinaryValue() { Value = "000000" } },
                         new FontName() { Val = "Calibri" }),
                     new Font(                                                               // Index 1 - The bold font.
                         new Bold(),
                         new FontSize() { Val = 11 },
                         new Color() { Rgb = new HexBinaryValue() { Value = "000000" } },
                         new FontName() { Val = "Calibri" }),
                     new Font(                                                               // Index 2 - The Italic font.
                // new Italic(),
                         new FontSize() { Val = 11 },
                         new Color() { Rgb = new HexBinaryValue() { Value = "FFFFFF" } },
                         new FontName() { Val = "Calibri" }),
                     new Font(                                                               // Index 3 - The Times Roman font. with 16 size
                         new FontSize() { Val = 16 },
                         new Color() { Rgb = new HexBinaryValue() { Value = "000000" } },
                         new FontName() { Val = "Times New Roman" })
                 ),
                 new Fills(
                     new Fill(                                                           // Index 0 - The default fill.
                         new PatternFill() { PatternType = PatternValues.None }),
                     new Fill(                                                           // Index 1 - The default fill of gray 125 (required)
                         new PatternFill() { PatternType = PatternValues.Gray125 }),
                     new Fill(                                                           // Index 2 - The red fill.
                         new PatternFill(
                             new ForegroundColor() { Rgb = new HexBinaryValue() { Value = "FF0000" } }
                         ) { PatternType = PatternValues.Solid })
                 ),
                 new Borders(
                     new Border(                                                         // Index 0 - The default border.
                         new LeftBorder(),
                         new RightBorder(),
                         new TopBorder(),
                         new BottomBorder(),
                         new DiagonalBorder()),
                     new Border(                                                         // Index 1 - Applies a Left, Right, Top, Bottom border to a cell
                         new LeftBorder(
                             new Color() { Auto = true }
                         ) { Style = BorderStyleValues.Thin },
                         new RightBorder(
                             new Color() { Auto = true }
                         ) { Style = BorderStyleValues.Thin },
                         new TopBorder(
                             new Color() { Auto = true }
                         ) { Style = BorderStyleValues.Thin },
                         new BottomBorder(
                             new Color() { Auto = true }
                         ) { Style = BorderStyleValues.Thin },
                         new DiagonalBorder())
                 ),
                 new CellFormats(
                     new CellFormat() { FontId = 0,FillId = 0,BorderId = 0 },                          // Index 0 - The default cell style.  If a cell does not have a style index applied it will use this style combination instead
                     new CellFormat() { FontId = 1,FillId = 0,BorderId = 0,ApplyFont = true },       // Index 1 - Bold 
                     new CellFormat() { FontId = 2,FillId = 0,BorderId = 0,ApplyFont = true },       // Index 2 - Italic
                     new CellFormat() { FontId = 3,FillId = 0,BorderId = 0,ApplyFont = true },       // Index 3 - Times Roman
                     new CellFormat() { FontId = 2,FillId = 2,BorderId = 0,ApplyFill = true },       // Index 4 - Red Fill
                     new CellFormat(                                                                 // Index 5 - Alignment
                         new Alignment() { Horizontal = HorizontalAlignmentValues.Center,Vertical = VerticalAlignmentValues.Center }
                     ) { FontId = 0,FillId = 0,BorderId = 0,ApplyAlignment = true },
                     new CellFormat() { FontId = 0,FillId = 0,BorderId = 1,ApplyBorder = true }      // Index 6 - Border
                 )
             ); // return
        }
    }

}
