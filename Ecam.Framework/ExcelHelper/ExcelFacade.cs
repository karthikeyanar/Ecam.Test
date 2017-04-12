using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ecam.Framework.ExcelHelper {

    public class ExcelSheet {
        public ExcelSheet() {
            this.SheetName = string.Empty;
            this.Columns = new List<ExcelGridHeaderColumn>();
            this.Headings = new List<ExcelHeadingColumn>();
        }
        public string SheetName { get; set; }
        public IList Rows { get; set; }
        public List<ExcelGridHeaderColumn> Columns { get; set; }
        public List<ExcelHeadingColumn> Headings { get; set; }
    }

    public class ExcelHeadingColumn {
        public ExcelHeadingColumn() {
            this.BackGroundColor = System.Drawing.Color.White; //Yellow
            this.ForeGroundColor = System.Drawing.Color.Black;
            this.FontSize = 15;
            this.IsBold = true;
        }
        public string Text { get; set; }
        public int FontSize { get; set; }
        public bool IsBold { get; set; }

        public System.Drawing.Color BackGroundColor { get; set; }
        public System.Drawing.Color ForeGroundColor { get; set; }
    }

    public class ExcelGridHeaderColumn {
        public ExcelGridHeaderColumn() {
            this.HeaderBackGroundColor = System.Drawing.Color.FromArgb(201,218,248); //Blue
            this.HeaderForeGroundColor = System.Drawing.Color.Black;
            this.PropertyName = string.Empty;
            this.DisplayName = string.Empty;
            this.IsIgNore = false;
            this.IsSerialNumberColumn = false;
            this.Formula = string.Empty;
            this.ExcelColumnName = string.Empty;
            this.IsCustomFormat = false;
            this.IsFooterSum = false;
            this.FooterText = string.Empty;
            this.DataType = ExcelDataType.String;
            this.IsExcelDataRowColumn = false;
            this.Width = 0;
        }
        public string PropertyName { get; set; }
        public string DisplayName { get; set; }
        public bool IsIgNore { get; set; }
        public bool IsSerialNumberColumn { get; set; }
        public bool IsExcelDataRowColumn { get; set; }
        public string Formula { get; set; }
        public string ExcelColumnName { get; set; }
        public bool IsCustomFormat { get; set; }
        public bool IsFooterSum { get; set; }
        public string FooterText { get; set; }
        public int Width { get; set; }
        public bool IsCheckZero { get; set; }

        public ExcelDataType DataType { get; set; }

        public bool IsDynamicFormula { get; set; }

        public Func<string,string> CustomFormat { get; set; }

        public System.Drawing.Color HeaderBackGroundColor { get; set; }
        public System.Drawing.Color HeaderForeGroundColor { get; set; }
    }

    public enum ExcelDataType {
        Int16 = 1,
        Int32 = 2,
        Int64 = 3,
        Decimal = 4,
        DateTime = 5,
        String = 6,
        Boolean = 7
    }

    public class ExcelDataRow {
        public ExcelDataRow() {
            this.Cells = new List<string>();
        }
        public List<string> Cells { get; set; }
        public object OriginalRow { get; set; }
    }

    public class ExcelGuiDetail {
        public ExcelGuiDetail() {
            this.BackGroundColor = System.Drawing.Color.White; //Yellow
            this.ForeGroundColor = System.Drawing.Color.Black;
            this.FontSize = 11;
            this.IsBold = false;
        }
        public int FontSize { get; set; }
        public bool IsBold { get; set; }
        public System.Drawing.Color BackGroundColor { get; set; }
        public System.Drawing.Color ForeGroundColor { get; set; }
    }

    public class ExcelFacade {

        public Func<object,ExcelGuiDetail> OnBeforeRowRender { get; set; }

        public void Create(
           string fileName,
           List<ExcelSheet> excelSheets) {
            //Open the copied template workbook. 
            using(SpreadsheetDocument myWorkbook = SpreadsheetDocument.Create(fileName,SpreadsheetDocumentType.Workbook)) {
                WorkbookPart workbookPart = myWorkbook.AddWorkbookPart();

                // Create Styles and Insert into Workbook
                WorkbookStylesPart stylesPart = myWorkbook.WorkbookPart.AddNewPart<WorkbookStylesPart>();
                Stylesheet styles = new CustomStylesheet();
                styles.Save(stylesPart);

                Workbook workbook = new Workbook();
                FileVersion fileVersion = new FileVersion { ApplicationName = "Microsoft Office Excel" };

                UInt32 sheetId = 0;
                Sheets sheets = new Sheets();
                foreach(var excelSheet in excelSheets) {
                    sheetId += 1;

                    WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                    string relId = workbookPart.GetIdOfPart(worksheetPart);

                    SheetData sheetData = CreateSheetData(excelSheet,stylesPart);
                    Worksheet worksheet = new Worksheet();

                    int numCols = excelSheet.Columns.Where(q => q.IsIgNore == false).Count();

                    Columns columns = new Columns();
                    int colIndex = -1;
                    int width = 0;
                    foreach(var colm in excelSheet.Columns) {
                        if(colm.IsIgNore == true) continue;
                        colIndex += 1;
                        if(colm.Width > 0) {
                            width = colm.Width;
                        } else {
                            width = colm.DisplayName.Length + 5;
                        }
                        Column c = CreateColumnData((UInt32)colIndex + 1,(UInt32)numCols + 1,width);
                        columns.Append(c);
                    }
                    worksheet.Append(columns);

                    worksheet.Append(sheetData);
                    worksheetPart.Worksheet = worksheet;
                    worksheetPart.Worksheet.Save();

                    Sheet sheet = new Sheet { Name = excelSheet.SheetName,SheetId = sheetId,Id = relId };
                    sheets.Append(sheet);
                }
                workbook.Append(fileVersion);
                workbook.Append(sheets);
                myWorkbook.WorkbookPart.Workbook = workbook;
                myWorkbook.WorkbookPart.Workbook.Save();
                myWorkbook.Close();
            }
        }

        public static List<string> GetHeaderLetters(int max) {
            var result = new List<string>();
            int i = 0;
            var columnPrefix = new Queue<string>();
            string prefix = null;
            int prevRoundNo = 0;
            int maxPrefix = max / 26;

            while(i < max) {
                int roundNo = i / 26;
                if(prevRoundNo < roundNo) {
                    prefix = columnPrefix.Dequeue();
                    prevRoundNo = roundNo;
                }
                string item = prefix + ((char)(65 + (i % 26))).ToString(CultureInfo.InvariantCulture);
                if(i <= maxPrefix) {
                    columnPrefix.Enqueue(item);
                }
                result.Add(item);
                i++;
            }
            return result;
        }

        private SheetData CreateSheetData(ExcelSheet excelSheet,WorkbookStylesPart stylesPart) {
            IList objects = excelSheet.Rows;
            List<ExcelHeadingColumn> headingColumns = excelSheet.Headings;
            List<ExcelGridHeaderColumn> headerColumns = excelSheet.Columns;
            SheetData sheetData = new SheetData();
            bool isExcelDataRows = false;
            string value = string.Empty;

            TextCell textCell = null;
            FomulaCell formulaCell = null;
            Row headingRow = null;
            if(objects != null) {

                if(objects.GetType() == (new List<ExcelDataRow>()).GetType()) {
                    isExcelDataRows = true;
                }

                //Fields names of object
                //List<string> fields = GetPropertyInfo<T>();

                //var az = new List<Char>(Enumerable.Range('A','Z' - 'A' + 1).Select(i => (Char)i).ToArray());
                List<string> az = GetHeaderLetters(headerColumns.Count + 1);

                List<string> headers = az.GetRange(0,headerColumns.Count);

                int index = 0;
                foreach(var col in headingColumns) {
                    headingRow = new Row();
                    index += 1;
                    textCell = new TextCell(headers[0]
                       ,col.Text
                       ,index
                       ,stylesPart.Stylesheet
                       ,col.BackGroundColor
                       ,col.ForeGroundColor
                       ,col.FontSize
                       ,col.IsBold);
                    headingRow.Append(textCell);
                    sheetData.Append(headingRow);
                }

                int numRows = objects.Count;
                int numCols = headerColumns.Where(q => q.IsIgNore == false).Count();
                Row header = new Row();
                int gridRowsStartIndex = 0;
                index += 1;
                header.RowIndex = (uint)index;

                int colIndex = -1;
                foreach(var col in headerColumns) {
                    if(col.IsIgNore == true) continue;
                    colIndex += 1;
                    col.ExcelColumnName = headers[colIndex].ToString();
                    textCell = new TextCell(col.ExcelColumnName
                       ,col.DisplayName
                       ,index
                       ,stylesPart.Stylesheet
                       ,col.HeaderBackGroundColor
                       ,col.HeaderForeGroundColor
                       ,12
                       ,true);
                    header.Append(textCell);
                }
                sheetData.Append(header);

                ExcelGuiDetail rowGui = null;
                for(int i = 0;i < numRows;i++) {
                    index++;
                    if(gridRowsStartIndex <= 0) {
                        gridRowsStartIndex = index;
                    }
                    var row = objects[i];
                    rowGui = null;
                    if(this.OnBeforeRowRender != null) {
                        rowGui = this.OnBeforeRowRender(row);
                    } else {
                        rowGui = new ExcelGuiDetail();
                    }
                    var r = new Row { RowIndex = (uint)index };

                    colIndex = -1;
                    foreach(var colm in headerColumns) {
                        value = string.Empty;
                        if(colm.IsIgNore == true) continue;
                        colIndex += 1;

                        if(string.IsNullOrEmpty(colm.Formula) == true
                            && string.IsNullOrEmpty(colm.PropertyName) == true
                            && colm.IsSerialNumberColumn == false
                            && colm.IsExcelDataRowColumn == false) {
                            r.Append(new TextCell(headers[colIndex].ToString(),string.Empty,index,stylesPart.Stylesheet,rowGui.BackGroundColor,rowGui.ForeGroundColor,rowGui.FontSize,rowGui.IsBold));
                            continue;
                        }

                        if(colm.IsSerialNumberColumn == true) {
                            r.Append(new NumberCell(headers[colIndex].ToString(),(i + 1).ToString(),index,stylesPart.Stylesheet,rowGui.BackGroundColor,rowGui.ForeGroundColor,rowGui.FontSize,rowGui.IsBold));
                            continue;
                        } else if(string.IsNullOrEmpty(colm.Formula) == false) {
                            string formula = colm.Formula;
                            foreach(var tempC in headerColumns) {
                                if(string.IsNullOrEmpty(tempC.PropertyName) == false
                                    && string.IsNullOrEmpty(tempC.ExcelColumnName) == false) {
                                    formula = formula.Replace(tempC.PropertyName,tempC.ExcelColumnName);
                                }
                            }
                            formula = formula.Replace("{RI}",r.RowIndex);
                            r.Append(new FomulaCell(headers[colIndex].ToString(),formula,index,stylesPart.Stylesheet,rowGui.BackGroundColor,rowGui.ForeGroundColor,rowGui.FontSize,rowGui.IsBold));
                            continue;
                        }
                        bool isAppend = false;
                        if(isExcelDataRows == true) {
                            ExcelDataRow excelDataRow = (ExcelDataRow)row;
                            value = excelDataRow.Cells[colIndex];
                            isAppend = false; 
                            if(colm.IsDynamicFormula == false) {
                                switch(colm.DataType) {
                                    case ExcelDataType.Decimal:
                                        if(colm.IsCheckZero == true) {
                                            decimal v = DataTypeHelper.ToDecimal(value);
                                            if(v == 0) {
                                                r.Append(new TextCell(headers[colIndex].ToString(),string.Empty,index,stylesPart.Stylesheet,rowGui.BackGroundColor,rowGui.ForeGroundColor,rowGui.FontSize,rowGui.IsBold));
                                                isAppend = true;
                                            }
                                        }
                                        if(isAppend == false)
                                            r.Append(new NumberCell(headers[colIndex].ToString(),DataTypeHelper.ToDecimal(value).ToString(),index,stylesPart.Stylesheet,rowGui.BackGroundColor,rowGui.ForeGroundColor,rowGui.FontSize,rowGui.IsBold));
                                        break;
                                    case ExcelDataType.Int16:
                                    case ExcelDataType.Int32:
                                    case ExcelDataType.Int64:
                                        if(colm.IsCheckZero == true) {
                                            Int64 v = DataTypeHelper.ToInt64(value);
                                            if(v == 0) {
                                                r.Append(new TextCell(headers[colIndex].ToString(),string.Empty,index,stylesPart.Stylesheet,rowGui.BackGroundColor,rowGui.ForeGroundColor,rowGui.FontSize,rowGui.IsBold));
                                                isAppend = true;
                                            }
                                        }
                                        r.Append(new NumberCell(headers[colIndex].ToString(),DataTypeHelper.ToInt64(value).ToString(),index,stylesPart.Stylesheet,rowGui.BackGroundColor,rowGui.ForeGroundColor,rowGui.FontSize,rowGui.IsBold));
                                        break;
                                    case ExcelDataType.DateTime:
                                        r.Append(new DateCell(headers[colIndex].ToString(),DataTypeHelper.ToDateTime(value),index,stylesPart.Stylesheet,rowGui.BackGroundColor,rowGui.ForeGroundColor,rowGui.FontSize,rowGui.IsBold));
                                        break;
                                    case ExcelDataType.Boolean:
                                        value = (DataTypeHelper.ToBoolean(value) == true ? "Yes" : "No");
                                        r.Append(new TextCell(headers[colIndex].ToString(),value,index,stylesPart.Stylesheet,rowGui.BackGroundColor,rowGui.ForeGroundColor,rowGui.FontSize,rowGui.IsBold));
                                        break;
                                    case ExcelDataType.String:
                                        r.Append(new TextCell(headers[colIndex].ToString(),value,index,stylesPart.Stylesheet,rowGui.BackGroundColor,rowGui.ForeGroundColor,rowGui.FontSize,rowGui.IsBold));
                                        break;
                                }
                            } else {
                                r.Append(new FomulaCell(headers[colIndex].ToString(),value,index,stylesPart.Stylesheet,rowGui.BackGroundColor,rowGui.ForeGroundColor,rowGui.FontSize,rowGui.IsBold));
                            }
                        } else {

                            // List Of Class Objects Only
                            if(string.IsNullOrEmpty(colm.PropertyName) == false) {
                                string fieldName = colm.PropertyName;
                                PropertyInfo myf = row.GetType().GetProperty(fieldName);
                                if(myf != null) {
                                    object obj = myf.GetValue(row,null);
                                    if(obj != null) {
                                        if(colm.CustomFormat != null) {
                                            r.Append(new TextCell(headers[colIndex].ToString(),colm.CustomFormat(obj.ToString()),index,stylesPart.Stylesheet,rowGui.BackGroundColor,rowGui.ForeGroundColor,rowGui.FontSize,rowGui.IsBold));
                                        } else {
                                            if(obj.GetType() == typeof(string)) {
                                                r.Append(new TextCell(headers[colIndex].ToString(),obj.ToString(),index,stylesPart.Stylesheet,rowGui.BackGroundColor,rowGui.ForeGroundColor,rowGui.FontSize,rowGui.IsBold));
                                            } else if(obj.GetType() == typeof(bool)) {
                                                value = (bool)obj ? "Yes" : "No";
                                                r.Append(new TextCell(headers[colIndex].ToString(),value,index,stylesPart.Stylesheet,rowGui.BackGroundColor,rowGui.ForeGroundColor,rowGui.FontSize,rowGui.IsBold));
                                            } else if(obj.GetType() == typeof(DateTime)) {
                                                r.Append(new DateCell(headers[colIndex].ToString(),(DateTime)obj,index,stylesPart.Stylesheet,rowGui.BackGroundColor,rowGui.ForeGroundColor,rowGui.FontSize,rowGui.IsBold));
                                            } else if(obj.GetType() == typeof(decimal)
                                                || obj.GetType() == typeof(double)
                                                || obj.GetType() == typeof(Single)
                                                || obj.GetType() == typeof(float)
                                                ) {
                                                r.Append(new NumberCell(headers[colIndex].ToString(),obj.ToString(),index,stylesPart.Stylesheet,rowGui.BackGroundColor,rowGui.ForeGroundColor,rowGui.FontSize,rowGui.IsBold));
                                            } else if(obj.GetType() == typeof(Int16)
                                                || obj.GetType() == typeof(Int32)
                                                || obj.GetType() == typeof(Int64)
                                                || obj.GetType() == typeof(UInt16)
                                                || obj.GetType() == typeof(UInt32)
                                                || obj.GetType() == typeof(UInt64)
                                                ) {
                                                r.Append(new NumberCell(headers[colIndex].ToString(),obj.ToString(),index,stylesPart.Stylesheet,rowGui.BackGroundColor,rowGui.ForeGroundColor,rowGui.FontSize,rowGui.IsBold));
                                            } else {
                                                r.Append(new TextCell(headers[colIndex].ToString(),obj.ToString(),index,stylesPart.Stylesheet,rowGui.BackGroundColor,rowGui.ForeGroundColor,rowGui.FontSize,rowGui.IsBold));
                                            }
                                        }
                                    }
                                }
                            }
                            // End List Of Class Objects Only

                        }

                    }
                    sheetData.Append(r);
                }

                index++;
                int totalColumnCount = headerColumns.Where(q => q.IsFooterSum == true).Count();
                if(totalColumnCount > 0 && excelSheet.Rows.Count > 0) {
                    Row total = new Row();
                    total.RowIndex = (uint)index;
                    colIndex = -1;
                    foreach(var colm in headerColumns) {
                        if(colm.IsIgNore == true) continue;
                        colIndex += 1;
                        if(colm.IsFooterSum == false) {
                            textCell = new TextCell(headers[colIndex].ToString(),colm.FooterText,index,stylesPart.Stylesheet,rowGui.BackGroundColor,rowGui.ForeGroundColor,rowGui.FontSize,rowGui.IsBold);
                            textCell.StyleIndex = 12;
                            total.Append(textCell);
                        } else {
                            string headerCol = headers[colIndex].ToString();
                            string firstRow = headerCol + gridRowsStartIndex.ToString();
                            string lastRow = headerCol + ((numRows + 1) + excelSheet.Headings.Count());
                            string formula = "=SUM(" + firstRow + " : " + lastRow + ")";
                            //c = CreateFomulaCell(headers[col].ToString(), formula, index, stylesPart.Stylesheet);
                            formulaCell = new FomulaCell(headers[colIndex].ToString(),formula,index,stylesPart.Stylesheet,rowGui.BackGroundColor,rowGui.ForeGroundColor,rowGui.FontSize,rowGui.IsBold);
                            formulaCell.StyleIndex = 11;
                            total.Append(formulaCell);
                        }
                    }
                    sheetData.Append(total);
                }
            }
            return sheetData;
        }

        /*
        private Cell CreateIntegerCell(string header,string text,int index) {
            Cell c = new Cell();
            c.DataType = CellValues.Number;
            c.CellReference = header + index;

            CellValue v = new CellValue();
            v.Text = text;
            c.AppendChild(v);
            return c;
        }

        private Cell CreateDecimalCell(string header,string text,int index,Stylesheet styles) {
            Cell c = new Cell();
            c.DataType = CellValues.Number;
            c.CellReference = header + index;
            UInt32Value fontId = CreateFont(styles,"Calibri",11,false,System.Drawing.Color.Black);
            UInt32Value fillId = CreateFill(styles,System.Drawing.Color.White);
            UInt32Value formatId = CreateCellFormat(styles,fontId,fillId,171);
            c.StyleIndex = formatId;

            CellValue v = new CellValue();
            v.Text = text;
            c.AppendChild(v);
            return c;
        }

        private Cell CreateFomulaCell(string header,string formula,int index,Stylesheet styles) {
            Cell c = new Cell();
            c.DataType = CellValues.Number;
            c.CellReference = header + index;
            UInt32Value fontId = CreateFont(styles,"Calibri",11,false,System.Drawing.Color.Black);
            UInt32Value fillId = CreateFill(styles,System.Drawing.Color.White);
            UInt32Value formatId = CreateCellFormat(styles,fontId,fillId,171);
            c.StyleIndex = formatId;

            CellFormula f = new CellFormula();
            f.CalculateCell = true;
            f.Text = formula;
            c.Append(f);

            CellValue v = new CellValue();
            c.AppendChild(v);
            return c;
        }

        private Cell CreateDateCell(string header,string text,int index,Stylesheet styles) {
            Cell c = new Cell();
            c.DataType = CellValues.Date;
            c.CellReference = header + index;

            UInt32Value fontId = CreateFont(styles,"Calibri",11,false,System.Drawing.Color.Black);
            UInt32Value fillId = CreateFill(styles,System.Drawing.Color.White);
            UInt32Value formatId = CreateCellFormat(styles,fontId,fillId,14);
            c.StyleIndex = formatId;

            CellValue v = new CellValue();
            v.Text = text;
            c.CellValue = v;


            return c;
        }

        private Cell CreateTextCell(string header,string text,int index) {

            //Create a new inline string cell.
            Cell c = new Cell();
            c.DataType = CellValues.InlineString;
            c.CellReference = header + index;

            //Add text to the text cell.
            InlineString inlineString = new InlineString();
            Text t = new Text();
            t.Text = text;
            inlineString.AppendChild(t);
            c.AppendChild(inlineString);
            return c;
        }

        private Cell CreateHeaderCell(string header,string text,int index,Stylesheet styles) {
            //Create a new inline string cell.
            Cell c = new Cell();
            c.DataType = CellValues.InlineString;
            c.CellReference = header + index;
            Console.WriteLine(header + index);

            UInt32Value fontId = CreateFont(styles,"Calibri",12,true,System.Drawing.Color.Black);
            UInt32Value fillId = CreateFill(styles,System.Drawing.Color.ForestGreen);
            UInt32Value formatId = CreateCellFormat(styles,fontId,fillId,0);
            c.StyleIndex = formatId;

            //Add text to the text cell.
            InlineString inlineString = new InlineString();
            Text t = new Text();
            t.Text = text;
            inlineString.AppendChild(t);
            c.AppendChild(inlineString);
            return c;
        }
        */

        private List<string> GetPropertyInfo<T>() {

            PropertyInfo[] propertyInfos = typeof(T).GetProperties();
            // write property names
            return propertyInfos.Select(propertyInfo => propertyInfo.Name).ToList();
        }

        private static UInt32Value CreateCellFormat(
            Stylesheet styleSheet,
            UInt32Value fontIndex,
            UInt32Value fillIndex,
            UInt32Value numberFormatId) {
            CellFormat cellFormat = new CellFormat();

            if(fontIndex != null)
                cellFormat.FontId = fontIndex;

            if(fillIndex != null)
                cellFormat.FillId = fillIndex;

            if(numberFormatId != null) {
                cellFormat.NumberFormatId = numberFormatId;
                cellFormat.ApplyNumberFormat = BooleanValue.FromBoolean(true);
            }

            styleSheet.CellFormats.Append(cellFormat);

            UInt32Value result = styleSheet.CellFormats.Count;
            styleSheet.CellFormats.Count++;
            return result;
        }

        private UInt32Value CreateFill(
            Stylesheet styleSheet,
            System.Drawing.Color fillColor) {


            PatternFill patternFill =
                new PatternFill(
                    new ForegroundColor() {
                        Rgb = new HexBinaryValue() {
                            Value =
                            System.Drawing.ColorTranslator.ToHtml(
                                System.Drawing.Color.FromArgb(
                                    fillColor.A,
                                    fillColor.R,
                                    fillColor.G,
                                    fillColor.B)).Replace("#","")
                        }
                    });

            patternFill.PatternType = fillColor ==
                        System.Drawing.Color.White ? PatternValues.None : PatternValues.LightDown;

            Fill fill = new Fill(patternFill);

            styleSheet.Fills.Append(fill);

            UInt32Value result = styleSheet.Fills.Count;
            styleSheet.Fills.Count++;
            return result;
        }

        private UInt32Value CreateFont(
            Stylesheet styleSheet,
            string fontName,
            double? fontSize,
            bool isBold,
            System.Drawing.Color foreColor) {

            Font font = new Font();

            if(!string.IsNullOrEmpty(fontName)) {
                FontName name = new FontName() {
                    Val = fontName
                };
                font.Append(name);
            }

            if(fontSize.HasValue) {
                FontSize size = new FontSize() {
                    Val = fontSize.Value
                };
                font.Append(size);
            }

            if(isBold == true) {
                Bold bold = new Bold();
                font.Append(bold);
            }


            Color color = new Color() {
                Rgb = new HexBinaryValue() {
                    Value =
                        System.Drawing.ColorTranslator.ToHtml(
                            System.Drawing.Color.FromArgb(
                                foreColor.A,
                                foreColor.R,
                                foreColor.G,
                                foreColor.B)).Replace("#","")
                }
            };
            font.Append(color);

            styleSheet.Fonts.Append(font);
            UInt32Value result = styleSheet.Fonts.Count;
            styleSheet.Fonts.Count++;
            return result;
        }

        private Column CreateColumnData(UInt32 startColumnIndex,UInt32 endColumnIndex,double columnWidth) {
            Column column;
            column = new Column();
            column.Min = startColumnIndex;
            column.Max = endColumnIndex;
            column.Width = columnWidth;
            column.CustomWidth = true;
            return column;
        }

        private int GetExcelSerialDate(DateTime input) {
            int nDay = input.Day;
            int nMonth = input.Month;
            int nYear = input.Year;
            // Excel/Lotus 123 have a bug with 29-02-1900. 1900 is not a
            // leap year, but Excel/Lotus 123 think it is...
            if(nDay == 29 && nMonth == 02 && nYear == 1900)
                return 60;

            // DMY to Modified Julian calculatie with an extra substraction of 2415019.
            long nSerialDate =
                    (int)((1461 * (nYear + 4800 + (int)((nMonth - 14) / 12))) / 4) +
                    (int)((367 * (nMonth - 2 - 12 * ((nMonth - 14) / 12))) / 12) -
                    (int)((3 * ((int)((nYear + 4900 + (int)((nMonth - 14) / 12)) / 100))) / 4) +
                    nDay - 2415019 - 32075;

            if(nSerialDate < 60) {
                // Because of the 29-02-1900 bug, any serial date 
                // under 60 is one off... Compensate.
                nSerialDate--;
            }

            return (int)nSerialDate;

        }
    }

    public class CustomStylesheet:Stylesheet {
        public CustomStylesheet() {
            Fonts fts = new Fonts();
            DocumentFormat.OpenXml.Spreadsheet.Font ft = new DocumentFormat.OpenXml.Spreadsheet.Font();
            FontName ftn = new FontName();
            ftn.Val = StringValue.FromString("Calibri");
            FontSize ftsz = new FontSize();
            ftsz.Val = DoubleValue.FromDouble(11);
            ft.FontName = ftn;
            ft.FontSize = ftsz;
            fts.Append(ft);

            ft = new DocumentFormat.OpenXml.Spreadsheet.Font();
            ftn = new FontName();
            ftn.Val = StringValue.FromString("Palatino Linotype");
            ftsz = new FontSize();
            ftsz.Val = DoubleValue.FromDouble(18);
            ft.FontName = ftn;
            ft.FontSize = ftsz;
            fts.Append(ft);

            ft = new DocumentFormat.OpenXml.Spreadsheet.Font();
            ftn = new FontName();
            ftn.Val = StringValue.FromString("Calibri");
            ftsz = new FontSize();
            ftsz.Val = DoubleValue.FromDouble(11);
            ft.FontName = ftn;
            ft.FontSize = ftsz;
            ft.Bold = new Bold();
            fts.Append(ft);

            fts.Count = UInt32Value.FromUInt32((uint)fts.ChildElements.Count);

            Fills fills = new Fills();
            Fill fill;
            PatternFill patternFill;
            fill = new Fill();
            patternFill = new PatternFill();
            patternFill.PatternType = PatternValues.None;
            fill.PatternFill = patternFill;
            fills.Append(fill);

            fill = new Fill();
            patternFill = new PatternFill();
            patternFill.PatternType = PatternValues.Gray125;
            fill.PatternFill = patternFill;
            fills.Append(fill);

            fill = new Fill();
            patternFill = new PatternFill();
            patternFill.PatternType = PatternValues.Solid;
            patternFill.ForegroundColor = new ForegroundColor();
            patternFill.ForegroundColor.Rgb = HexBinaryValue.FromString("00ff9728");
            patternFill.BackgroundColor = new BackgroundColor();
            patternFill.BackgroundColor.Rgb = patternFill.ForegroundColor.Rgb;
            fill.PatternFill = patternFill;
            fills.Append(fill);

            fills.Count = UInt32Value.FromUInt32((uint)fills.ChildElements.Count);

            Borders borders = new Borders();
            Border border = new Border();
            border.LeftBorder = new LeftBorder();
            border.RightBorder = new RightBorder();
            border.TopBorder = new TopBorder();
            border.BottomBorder = new BottomBorder();
            border.DiagonalBorder = new DiagonalBorder();
            borders.Append(border);

            //Boarder Index 1
            border = new Border();
            border.LeftBorder = new LeftBorder();
            border.LeftBorder.Style = BorderStyleValues.Thin;
            border.RightBorder = new RightBorder();
            border.RightBorder.Style = BorderStyleValues.Thin;
            border.TopBorder = new TopBorder();
            border.TopBorder.Style = BorderStyleValues.Thin;
            border.BottomBorder = new BottomBorder();
            border.BottomBorder.Style = BorderStyleValues.Thin;
            border.DiagonalBorder = new DiagonalBorder();
            borders.Append(border);


            //Boarder Index 2
            border = new Border();
            border.LeftBorder = new LeftBorder();
            border.RightBorder = new RightBorder();
            border.TopBorder = new TopBorder();
            border.TopBorder.Style = BorderStyleValues.Thin;
            border.BottomBorder = new BottomBorder();
            border.BottomBorder.Style = BorderStyleValues.Thin;
            border.DiagonalBorder = new DiagonalBorder();
            borders.Append(border);


            borders.Count = UInt32Value.FromUInt32((uint)borders.ChildElements.Count);

            CellStyleFormats csfs = new CellStyleFormats();
            CellFormat cf = new CellFormat();
            cf.NumberFormatId = 0;
            cf.FontId = 0;
            cf.FillId = 0;
            cf.BorderId = 0;
            csfs.Append(cf);
            csfs.Count = UInt32Value.FromUInt32((uint)csfs.ChildElements.Count);

            uint iExcelIndex = 164;
            NumberingFormats nfs = new NumberingFormats();
            CellFormats cfs = new CellFormats();

            cf = new CellFormat();
            cf.NumberFormatId = 0;
            cf.FontId = 0;
            cf.FillId = 0;
            cf.BorderId = 0;
            cf.FormatId = 0;
            cfs.Append(cf);

            NumberingFormat nfDateTime = new NumberingFormat();
            nfDateTime.NumberFormatId = UInt32Value.FromUInt32(iExcelIndex++);
            nfDateTime.FormatCode = StringValue.FromString("dd/mm/yyyy hh:mm:ss");
            nfs.Append(nfDateTime);

            NumberingFormat nf4decimal = new NumberingFormat();
            nf4decimal.NumberFormatId = UInt32Value.FromUInt32(iExcelIndex++);
            nf4decimal.FormatCode = StringValue.FromString("#,##0.0000");
            nfs.Append(nf4decimal);

            // #,##0.00 is also Excel style index 4
            NumberingFormat nf2decimal = new NumberingFormat();
            nf2decimal.NumberFormatId = UInt32Value.FromUInt32(iExcelIndex++);
            nf2decimal.FormatCode = StringValue.FromString("#,##0.00");
            nfs.Append(nf2decimal);

            // @ is also Excel style index 49
            NumberingFormat nfForcedText = new NumberingFormat();
            nfForcedText.NumberFormatId = UInt32Value.FromUInt32(iExcelIndex++);
            nfForcedText.FormatCode = StringValue.FromString("@");
            nfs.Append(nfForcedText);

            // index 1
            // Format dd/mm/yyyy
            cf = new CellFormat();
            cf.NumberFormatId = 14;
            cf.FontId = 0;
            cf.FillId = 0;
            cf.BorderId = 0;
            cf.FormatId = 0;
            cf.ApplyNumberFormat = BooleanValue.FromBoolean(true);
            cfs.Append(cf);

            // index 2
            // Format #,##0.00
            cf = new CellFormat();
            cf.NumberFormatId = 4;
            cf.FontId = 0;
            cf.FillId = 0;
            cf.BorderId = 0;
            cf.FormatId = 0;
            cf.ApplyNumberFormat = BooleanValue.FromBoolean(true);
            cfs.Append(cf);

            // index 3
            cf = new CellFormat();
            cf.NumberFormatId = nfDateTime.NumberFormatId;
            cf.FontId = 0;
            cf.FillId = 0;
            cf.BorderId = 0;
            cf.FormatId = 0;
            cf.ApplyNumberFormat = BooleanValue.FromBoolean(true);
            cfs.Append(cf);

            // index 4
            cf = new CellFormat();
            cf.NumberFormatId = nf4decimal.NumberFormatId;
            cf.FontId = 0;
            cf.FillId = 0;
            cf.BorderId = 0;
            cf.FormatId = 0;
            cf.ApplyNumberFormat = BooleanValue.FromBoolean(true);
            cfs.Append(cf);

            // index 5
            cf = new CellFormat();
            cf.NumberFormatId = nf2decimal.NumberFormatId;
            cf.FontId = 0;
            cf.FillId = 0;
            cf.BorderId = 0;
            cf.FormatId = 0;
            cf.ApplyNumberFormat = BooleanValue.FromBoolean(true);
            cfs.Append(cf);

            // index 6
            cf = new CellFormat();
            cf.NumberFormatId = nfForcedText.NumberFormatId;
            cf.FontId = 0;
            cf.FillId = 0;
            cf.BorderId = 0;
            cf.FormatId = 0;
            cf.ApplyNumberFormat = BooleanValue.FromBoolean(true);
            cfs.Append(cf);

            // index 7
            // Header text
            cf = new CellFormat();
            cf.NumberFormatId = nfForcedText.NumberFormatId;
            cf.FontId = 1;
            cf.FillId = 0;
            cf.BorderId = 0;
            cf.FormatId = 0;
            cf.ApplyNumberFormat = BooleanValue.FromBoolean(true);
            cfs.Append(cf);

            // index 8
            // column text
            cf = new CellFormat();
            cf.NumberFormatId = nfForcedText.NumberFormatId;
            cf.FontId = 0;
            cf.FillId = 0;
            cf.BorderId = 1;
            cf.FormatId = 0;
            cf.ApplyNumberFormat = BooleanValue.FromBoolean(true);
            cfs.Append(cf);

            // index 9
            // coloured 2 decimal text
            cf = new CellFormat();
            cf.NumberFormatId = nf2decimal.NumberFormatId;
            cf.FontId = 0;
            cf.FillId = 2;
            cf.BorderId = 2;
            cf.FormatId = 0;
            cf.ApplyNumberFormat = BooleanValue.FromBoolean(true);
            cfs.Append(cf);

            // index 10
            // coloured column text
            cf = new CellFormat();
            cf.NumberFormatId = nfForcedText.NumberFormatId;
            cf.FontId = 0;
            cf.FillId = 2;
            cf.BorderId = 2;
            cf.FormatId = 0;
            cf.ApplyNumberFormat = BooleanValue.FromBoolean(true);
            cfs.Append(cf);

            // index 11
            // bold text
            cf = new CellFormat();
            cf.NumberFormatId = nfForcedText.NumberFormatId;
            cf.FontId = 2;
            cf.FillId = 0;
            cf.BorderId = 0;
            cf.FormatId = 0;
            cf.Alignment = new Alignment {
                Horizontal = HorizontalAlignmentValues.Right
            };
            cf.ApplyNumberFormat = BooleanValue.FromBoolean(true);
            cfs.Append(cf);

            // index 12
            // bold text
            cf = new CellFormat();
            cf.FontId = 2;
            cf.FillId = 0;
            cf.BorderId = 0;
            cf.FormatId = 0;
            cfs.Append(cf);

            nfs.Count = UInt32Value.FromUInt32((uint)nfs.ChildElements.Count);
            cfs.Count = UInt32Value.FromUInt32((uint)cfs.ChildElements.Count);

            this.Append(nfs);
            this.Append(fts);
            this.Append(fills);
            this.Append(borders);
            this.Append(csfs);
            this.Append(cfs);

            CellStyles css = new CellStyles();
            CellStyle cs = new CellStyle();
            cs.Name = StringValue.FromString("Normal");
            cs.FormatId = 0;
            cs.BuiltinId = 0;
            css.Append(cs);
            css.Count = UInt32Value.FromUInt32((uint)css.ChildElements.Count);
            this.Append(css);

            DifferentialFormats dfs = new DifferentialFormats();
            dfs.Count = 0;
            this.Append(dfs);

            TableStyles tss = new TableStyles();
            tss.Count = 0;
            tss.DefaultTableStyle = StringValue.FromString("TableStyleMedium9");
            tss.DefaultPivotStyle = StringValue.FromString("PivotStyleLight16");
            this.Append(tss);
        }
    }

    public class NumberCell:GuiCell {
        public NumberCell(string header,string text,int index,Stylesheet styles,
            System.Drawing.Color backGroundColour,
            System.Drawing.Color foreGroundColor,
            double? fontSize = 11,
            bool isBold = false)
            : base(header,text,index,styles,backGroundColour,foreGroundColor,fontSize,isBold) {
            this.DataType = CellValues.Number;
            this.CellReference = header + index;
            this.CellValue = new CellValue(text);
        }
    }

    public class TextCell:GuiCell {
        public TextCell(string header,string text,int index,Stylesheet styles,
            System.Drawing.Color backGroundColour,
            System.Drawing.Color foreGroundColor,
            double? fontSize = 11,
            bool isBold = false)
            : base(header,text,index,styles,backGroundColour,foreGroundColor,fontSize,isBold) {
            this.DataType = CellValues.InlineString;
            this.CellReference = header + index;
            //Add text to the text cell.
            this.InlineString = new InlineString { Text = new Text { Text = text } };
        }
    }

    public class FormatedNumberCell:NumberCell {
        public FormatedNumberCell(string header,string text,int index,Stylesheet styles,
            System.Drawing.Color backGroundColour,
            System.Drawing.Color foreGroundColor,
            double? fontSize = 11,
            bool isBold = false)
            : base(header,text,index,styles,backGroundColour,foreGroundColor,fontSize,isBold) {
            this.StyleIndex = 2;
        }
    }

    public class DateCell:GuiCell {
        public DateCell(string header,DateTime dateTime,int index,Stylesheet styles,
            System.Drawing.Color backGroundColour,
            System.Drawing.Color foreGroundColor,
            double? fontSize = 11,
            bool isBold = false)
            : base(header,"",index,styles,backGroundColour,foreGroundColor,fontSize,isBold) {
            this.DataType = CellValues.Date;
            this.CellReference = header + index;
            this.StyleIndex = 1;
            //DateTime dateTime = DataTypeHelper.ToDateTime(text);
            this.CellValue = new CellValue { Text = dateTime.ToOADate().ToString() }; ;
        }
    }

    public class FomulaCell:GuiCell {
        public FomulaCell(string header,string text,int index,Stylesheet styles,
            System.Drawing.Color backGroundColour,
            System.Drawing.Color foreGroundColor,
            double? fontSize = 11,
            bool isBold = false)
            : base(header,text,index,styles,backGroundColour,foreGroundColor,fontSize,isBold) {
            this.CellFormula = new CellFormula { CalculateCell = true,Text = text };
            this.DataType = CellValues.Number;
            this.CellReference = header + index;
            this.StyleIndex = 2;
        }
    }


    public class GuiCell:Cell {
        public GuiCell(string header,string text,int index,Stylesheet styles,
            System.Drawing.Color backGroundColour,
            System.Drawing.Color foreGroundColor,
            double? fontSize = 11,
            bool isBold = false) {
            UInt32Value fontId = CreateFont(styles,"",fontSize,isBold,foreGroundColor);
            UInt32Value fillId = CreateFill(styles,backGroundColour);
            UInt32Value formatId = CreateCellFormat(styles,fontId,fillId,0);
            this.StyleIndex = formatId;
        }

        private static UInt32Value CreateCellFormat(
            Stylesheet styleSheet,
            UInt32Value fontIndex,
            UInt32Value fillIndex,
            UInt32Value numberFormatId) {
            CellFormat cellFormat = new CellFormat();

            if(fontIndex != null)
                cellFormat.FontId = fontIndex;

            if(fillIndex != null)
                cellFormat.FillId = fillIndex;

            if(numberFormatId != null) {
                cellFormat.NumberFormatId = numberFormatId;
                cellFormat.ApplyNumberFormat = BooleanValue.FromBoolean(true);
            }

            styleSheet.CellFormats.Append(cellFormat);

            UInt32Value result = styleSheet.CellFormats.Count;
            styleSheet.CellFormats.Count++;
            return result;
        }

        private static UInt32Value CreateFill(
            Stylesheet styleSheet,
            System.Drawing.Color fillColor) {


            PatternFill patternFill =
                new PatternFill(
                    new ForegroundColor() {
                        Rgb = new HexBinaryValue() {
                            Value =
                            System.Drawing.ColorTranslator.ToHtml(
                                System.Drawing.Color.FromArgb(
                                    fillColor.A,
                                    fillColor.R,
                                    fillColor.G,
                                    fillColor.B)).Replace("#","")
                        }
                    });

            patternFill.PatternType = fillColor ==
                        System.Drawing.Color.White ? PatternValues.None : PatternValues.Solid;

            Fill fill = new Fill(patternFill);

            styleSheet.Fills.Append(fill);

            UInt32Value result = styleSheet.Fills.Count;
            styleSheet.Fills.Count++;
            return result;
        }

        private static UInt32Value CreateFont(
            Stylesheet styleSheet,
            string fontName,
            double? fontSize,
            bool isBold,
            System.Drawing.Color foreColor) {

            Font font = new Font();

            if(!string.IsNullOrEmpty(fontName)) {
                FontName name = new FontName() {
                    Val = fontName
                };
                font.Append(name);
            }

            if(fontSize.HasValue) {
                FontSize size = new FontSize() {
                    Val = fontSize.Value
                };
                font.Append(size);
            }

            if(isBold == true) {
                Bold bold = new Bold();
                font.Append(bold);
            }


            Color color = new Color() {
                Rgb = new HexBinaryValue() {
                    Value =
                        System.Drawing.ColorTranslator.ToHtml(
                            System.Drawing.Color.FromArgb(
                                foreColor.A,
                                foreColor.R,
                                foreColor.G,
                                foreColor.B)).Replace("#","")
                }
            };
            font.Append(color);

            styleSheet.Fonts.Append(font);
            UInt32Value result = styleSheet.Fonts.Count;
            styleSheet.Fonts.Count++;
            return result;
        }
    }
}
