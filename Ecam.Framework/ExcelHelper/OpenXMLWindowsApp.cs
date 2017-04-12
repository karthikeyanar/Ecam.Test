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
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ecam.Framework.ExcelHelper
{

    public class OpenXMLCellValue
    {
        public uint RowIndex { get; set; }
        public string ColumnName { get; set; }
        public DocumentFormat.OpenXml.Spreadsheet.CellValues CellDataType { get; set; }
        public string Value { get; set; }
        public bool IsIgnore { get; set; }
    }

    public class OpenXMLWindowsApp
    {

        public static void Update(string fileName, string sheetName,
            uint startRowIndex, List<OpenXMLCellValue> cellValues, uint totalRowIndex)
        {
            // Open the document for editing.
            using (SpreadsheetDocument spreadSheet = SpreadsheetDocument.Open(fileName, true))
            {
                WorksheetPart worksheetPart = GetWorksheetPartByName(spreadSheet, sheetName);
                if (worksheetPart != null)
                {
                    Row startRow = GetRow(worksheetPart.Worksheet, startRowIndex);
                    Row row = null;
                    if (worksheetPart != null && startRow != null)
                    {
                        List<Cell> startRowCells = (from q in startRow.Elements<Cell>()
                                                    select q).ToList();

                        uint maxRowIndex = 0;
                        if (cellValues.Count > 0)
                        {
                            maxRowIndex = (from q in cellValues
                                           where q.IsIgnore == false
                                           select q.RowIndex).Max();
                        }
                        uint diffRows = (totalRowIndex - startRowIndex);
                        maxRowIndex = maxRowIndex + diffRows;
                        uint z;
                        int j;
                        for (z = (startRowIndex + 1); z < maxRowIndex; z++)
                        {
                            // OpenXMLCellValue cellValue = cellValues[i];
                            row = GetRow(worksheetPart.Worksheet, z);
                            if (totalRowIndex > 0 && z == totalRowIndex)
                            {
                                row = InsertRow(z, worksheetPart, new Row { RowIndex = z }, false);
                                totalRowIndex = z + 1;
                            }
                            if (row != null)
                            {
                                for (j = 0; j < startRowCells.Count(); j++)
                                {
                                    Cell startRowcell = startRowCells[j];
                                    if (startRowcell.CellReference != null)
                                    {
                                        string columnName = GetColumnName(startRowcell.CellReference.Value);
                                        Cell cell = GetCell(worksheetPart.Worksheet, columnName, row);
                                        if (cell == null)
                                        {
                                            cell = new Cell();
                                            cell.CellReference = new StringValue { Value = columnName + z };

                                            if (row != null)
                                            {
                                                row.AppendChild(cell);
                                            }
                                        }
                                        if (cell != null)
                                        {
                                            string cellValue = string.Empty;
                                            //if (startRowcell.CellValue != null)
                                            //{
                                            //    cellValue = startRowcell.CellValue.Text;
                                            //}
                                            cell.CellValue = new CellValue(cellValue);
                                            CellValues cellDataType = CellValues.String;
                                            //if (startRowcell.DataType != null)
                                            //{
                                            //    cellDataType = startRowcell.DataType;
                                            //}
                                            cell.DataType = new EnumValue<CellValues>(cellDataType);
                                        }
                                    }
                                }
                            }
                        }

                        int i = 0;
                        for (i = 0; i < cellValues.Count; i++)
                        {
                            OpenXMLCellValue cellValue = cellValues[i];
                            row = GetRow(worksheetPart.Worksheet, cellValue.RowIndex);
                            //if (totalRowIndex > 0 && cellValue.RowIndex == totalRowIndex)
                            //{
                            //    row = InsertRow(cellValue.RowIndex, worksheetPart, new Row { RowIndex = cellValue.RowIndex }, false);
                            //    totalRowIndex = cellValue.RowIndex + 1;
                            //}
                            if (row != null)
                            {
                                Cell cell = GetCell(worksheetPart.Worksheet, cellValue.ColumnName, row);
                                //if (cell == null)
                                //{
                                //    cell = new Cell();
                                //    cell.CellReference = new StringValue { Value = cellValue.ColumnName + cellValue.RowIndex };

                                //    if (row != null)
                                //    {
                                //        row.AppendChild(cell);
                                //    }
                                //}
                                if (cell != null)
                                {
                                    cell.CellValue = new CellValue(cellValue.Value);
                                    cell.DataType = new EnumValue<CellValues>(cellValue.CellDataType);
                                }
                            }
                        }

                        List<uint> rowIndexList = (from q in cellValues
                                                   where q.IsIgnore == false select q.RowIndex).Distinct().ToList();
                        rowIndexList = (from q in rowIndexList
                                        orderby q
                                        select q).ToList();
                        foreach (uint rowIndex in rowIndexList)
                        {
                            row = GetRow(worksheetPart.Worksheet, rowIndex);
                            if (row != null)
                            {
                                i = 0;
                                for (i = 0; i < startRowCells.Count(); i++)
                                {
                                    Cell cell = startRowCells[i];
                                    if (cell.CellReference != null)
                                    {
                                        if (cell.CellFormula != null)
                                        {
                                            string originalFormula = cell.CellFormula.Text;
                                            string replaceFormula = GetUpdatedFormulaToNewRow(originalFormula, rowIndex);
                                            string formulaColumn = GetColumnName(cell.CellReference.Value);
                                            Cell formulaCell = GetCell(worksheetPart.Worksheet, formulaColumn, row);
                                            if (formulaCell == null)
                                            {
                                                formulaCell = new Cell();
                                                formulaCell.CellReference = new StringValue { Value = formulaColumn + rowIndex };
                                                row.AppendChild(formulaCell);
                                            }
                                            if (formulaCell.CellFormula == null)
                                            {
                                                formulaCell.CellFormula = new CellFormula { CalculateCell = true, Text = "" };
                                            }
                                            formulaCell.CellFormula.Text = replaceFormula;
                                            formulaCell.DataType = new EnumValue<CellValues>(CellValues.Number);
                                        }
                                    }
                                }
                            }
                        }
                        row = GetRow(worksheetPart.Worksheet, totalRowIndex);
                        if (row != null)
                        {
                            List<Cell> cells = (from q in row.Elements<Cell>()
                                                select q).ToList();
                            i = 0;
                            for (i = 0; i < cells.Count(); i++)
                            {
                                Cell cell = cells[i];
                                if (cell.CellReference != null)
                                {
                                    string formulaColumn = GetColumnName(cell.CellReference.Value);
                                    if (cell.CellFormula != null)
                                    {
                                        if (rowIndexList.Count > 0)
                                        {
                                            cell.CellFormula.Text = string.Format("SUM({0}{1}:{0}{2})", formulaColumn, startRowIndex, rowIndexList.Last());
                                        }
                                    }
                                }
                            }
                        }
                        // Save the worksheet.
                        worksheetPart.Worksheet.Save();
                    }
                }
            }
        }

        public static string GetUpdatedFormulaToNewRow(string formula, uint newRow)
        {
            return Regex.Replace(formula, @"[A-Za-z]+\d+", delegate (Match match)
            {
                //Calculate the new row for this cell in the formula by the given offset
                uint oldRow = GetRowIndex(match.Value);
                string col = GetColumnName(match.Value);
                //uint newRow = oldRow + offset;

                //Create the new reference for this cell
                string newRef = col + newRow;
                return newRef;
            });
        }

        public static void UpdateCell(string fileName, string sheetName, uint rowIndex, string columnName, DocumentFormat.OpenXml.Spreadsheet.CellValues cellDataType, string value)
        {
            // Open the document for editing.
            using (SpreadsheetDocument spreadSheet = SpreadsheetDocument.Open(fileName, true))
            {
                WorksheetPart worksheetPart = GetWorksheetPartByName(spreadSheet, sheetName);

                if (worksheetPart != null)
                {
                    Cell cell = GetCell(worksheetPart.Worksheet, columnName, rowIndex);
                    if (cell == null)
                    {
                        cell = new Cell();
                        cell.CellReference = new StringValue { Value = columnName + rowIndex };
                        Row row = GetRow(worksheetPart.Worksheet, rowIndex);
                        if (row != null)
                        {
                            row.AppendChild(cell);
                        }
                    }
                    cell.CellValue = new CellValue(value);
                    cell.DataType = new EnumValue<CellValues>(cellDataType);
                    // Save the worksheet.
                    worksheetPart.Worksheet.Save();
                }
            }
        }

        private static WorksheetPart GetWorksheetPartByName(SpreadsheetDocument document, string sheetName)
        {
            IEnumerable<Sheet> sheets = document.WorkbookPart.Workbook.GetFirstChild<Sheets>().Elements<Sheet>().Where(s => s.Name == sheetName);

            if (sheets.Count() == 0)
            {
                // The specified worksheet does not exist.
                return null;
            }

            string relationshipId = sheets.First().Id.Value;
            WorksheetPart worksheetPart = (WorksheetPart)document.WorkbookPart.GetPartById(relationshipId);
            return worksheetPart;

        }

        // Given a worksheet, a column name, and a row index, 
        // gets the cell at the specified column and 
        private static Cell GetCell(Worksheet worksheet, string columnName, Row row)
        {
            if (row == null)
                return null;

            var cells = (from q in row.Elements<Cell>()
                         where q.CellReference != null
                         select q).ToList();

            Cell cell = null;
            int i;
            for (i = 0; i < cells.Count(); i++)
            {
                if (cells[i].CellReference != null)
                {
                    if (string.Compare(cells[i].CellReference.Value, columnName + row.RowIndex, true) == 0)
                    {
                        cell = cells[i];
                    }
                }
            }
            return cell;
            //return row.Elements<Cell>().Where(c =>  string.Compare(c.CellReference.Value, columnName + rowIndex, true) == 0).FirstOrDefault();
        }

        // Given a worksheet, a column name, and a row index, 
        // gets the cell at the specified column and 
        private static Cell GetCell(Worksheet worksheet, string columnName, uint rowIndex)
        {
            Row row = GetRow(worksheet, rowIndex);

            if (row == null)
                return null;

            return GetCell(worksheet, columnName, row.RowIndex);
        }

        // Given a worksheet and a row index, return the row.
        private static Row GetRow(Worksheet worksheet, uint rowIndex)
        {
            Row row = worksheet.GetFirstChild<SheetData>().Elements<Row>().Where(r => r.RowIndex == rowIndex).FirstOrDefault();
            if (row == null)
            {
                SheetData sheetData = worksheet.GetFirstChild<SheetData>();
                Row lastRow = sheetData.Elements<Row>().LastOrDefault();
                if (lastRow != null)
                {
                    //sheetData.InsertAfter(new Row() { RowIndex = (lastRow.RowIndex + 1) }, lastRow);
                    sheetData.InsertAfter(new Row() { RowIndex = rowIndex }, lastRow);
                }
                else
                {
                    sheetData.Append(new Row() { RowIndex = 0 });
                }
                row = worksheet.GetFirstChild<SheetData>().Elements<Row>().Where(r => r.RowIndex == rowIndex).FirstOrDefault();
            }
            return row;
        }

        /// <summary>
        /// Inserts a new row at the desired index. If one already exists, then it is
        /// returned. If an insertRow is provided, then it is inserted into the desired
        /// rowIndex
        /// </summary>
        /// <param name="rowIndex">Row Index</param>
        /// <param name="worksheetPart">Worksheet Part</param>
        /// <param name="insertRow">Row to insert</param>
        /// <param name="isLastRow">Optional parameter - True, you can guarantee that this row is the last row (not replacing an existing last row) in the sheet to insert; false it is not</param>
        /// <returns>Inserted Row</returns>
        public static Row InsertRow(uint rowIndex, WorksheetPart worksheetPart, Row insertRow, bool isNewLastRow = false)
        {
            Worksheet worksheet = worksheetPart.Worksheet;
            SheetData sheetData = worksheet.GetFirstChild<SheetData>();

            Row retRow = !isNewLastRow ? sheetData.Elements<Row>().FirstOrDefault(r => r.RowIndex == rowIndex) : null;

            // If the worksheet does not contain a row with the specified row index, insert one.
            if (retRow != null)
            {
                // if retRow is not null and we are inserting a new row, then move all existing rows down.
                if (insertRow != null)
                {
                    UpdateRowIndexes(worksheetPart, rowIndex, false);
                    UpdateMergedCellReferences(worksheetPart, rowIndex, false);
                    UpdateHyperlinkReferences(worksheetPart, rowIndex, false);

                    // actually insert the new row into the sheet
                    retRow = sheetData.InsertBefore(insertRow, retRow);  // at this point, retRow still points to the row that had the insert rowIndex

                    string curIndex = retRow.RowIndex.ToString();
                    string newIndex = rowIndex.ToString();

                    foreach (Cell cell in retRow.Elements<Cell>())
                    {
                        // Update the references for the rows cells.
                        cell.CellReference = new StringValue(cell.CellReference.Value.Replace(curIndex, newIndex));
                    }

                    // Update the row index.
                    retRow.RowIndex = rowIndex;
                }
            }
            else
            {
                // Row doesn't exist yet, shifting not needed.
                // Rows must be in sequential order according to RowIndex. Determine where to insert the new row.
                Row refRow = !isNewLastRow ? sheetData.Elements<Row>().FirstOrDefault(row => row.RowIndex > rowIndex) : null;

                // use the insert row if it exists
                retRow = insertRow ?? new Row() { RowIndex = rowIndex };

                IEnumerable<Cell> cellsInRow = retRow.Elements<Cell>();

                if (cellsInRow.Any())
                {
                    string curIndex = retRow.RowIndex.ToString();
                    string newIndex = rowIndex.ToString();

                    foreach (Cell cell in cellsInRow)
                    {
                        // Update the references for the rows cells.
                        cell.CellReference = new StringValue(cell.CellReference.Value.Replace(curIndex, newIndex));
                    }

                    // Update the row index.
                    retRow.RowIndex = rowIndex;
                }

                sheetData.InsertBefore(retRow, refRow);
            }

            return retRow;
        }

        /// <summary>
        /// Updates all of the Row indexes and the child Cells' CellReferences whenever
        /// a row is inserted or deleted.
        /// </summary>
        /// <param name="worksheetPart">Worksheet Part</param>
        /// <param name="rowIndex">Row Index being inserted or deleted</param>
        /// <param name="isDeletedRow">True if row was deleted, otherwise false</param>
        private static void UpdateRowIndexes(WorksheetPart worksheetPart, uint rowIndex, bool isDeletedRow)
        {
            // Get all the rows in the worksheet with equal or higher row index values than the one being inserted/deleted for reindexing.
            IEnumerable<Row> rows = worksheetPart.Worksheet.Descendants<Row>().Where(r => r.RowIndex.Value >= rowIndex);

            foreach (Row row in rows)
            {
                uint newIndex = (isDeletedRow ? row.RowIndex - 1 : row.RowIndex + 1);
                string curRowIndex = row.RowIndex.ToString();
                string newRowIndex = newIndex.ToString();

                foreach (Cell cell in row.Elements<Cell>())
                {
                    // Update the references for the rows cells.
                    cell.CellReference = new StringValue(cell.CellReference.Value.Replace(curRowIndex, newRowIndex));
                }

                // Update the row index.
                row.RowIndex = newIndex;
            }
        }

        /// <summary>
        /// Updates the MergedCelss reference whenever a new row is inserted or deleted. It will simply take the
        /// row index and either increment or decrement the cell row index in the merged cell reference based on
        /// if the row was inserted or deleted.
        /// </summary>
        /// <param name="worksheetPart">Worksheet Part</param>
        /// <param name="rowIndex">Row Index being inserted or deleted</param>
        /// <param name="isDeletedRow">True if row was deleted, otherwise false</param>
        private static void UpdateMergedCellReferences(WorksheetPart worksheetPart, uint rowIndex, bool isDeletedRow)
        {
            if (worksheetPart.Worksheet.Elements<MergeCells>().Count() > 0)
            {
                MergeCells mergeCells = worksheetPart.Worksheet.Elements<MergeCells>().FirstOrDefault();

                if (mergeCells != null)
                {
                    // Grab all the merged cells that have a merge cell row index reference equal to or greater than the row index passed in
                    List<MergeCell> mergeCellsList = mergeCells.Elements<MergeCell>().Where(r => r.Reference.HasValue)
                                                                                     .Where(r => GetRowIndex(r.Reference.Value.Split(':').ElementAt(0)) >= rowIndex ||
                                                                                                 GetRowIndex(r.Reference.Value.Split(':').ElementAt(1)) >= rowIndex).ToList();

                    // Need to remove all merged cells that have a matching rowIndex when the row is deleted
                    if (isDeletedRow)
                    {
                        List<MergeCell> mergeCellsToDelete = mergeCellsList.Where(r => GetRowIndex(r.Reference.Value.Split(':').ElementAt(0)) == rowIndex ||
                                                                                       GetRowIndex(r.Reference.Value.Split(':').ElementAt(1)) == rowIndex).ToList();

                        // Delete all the matching merged cells
                        foreach (MergeCell cellToDelete in mergeCellsToDelete)
                        {
                            cellToDelete.Remove();
                        }

                        // Update the list to contain all merged cells greater than the deleted row index
                        mergeCellsList = mergeCells.Elements<MergeCell>().Where(r => r.Reference.HasValue)
                                                                         .Where(r => GetRowIndex(r.Reference.Value.Split(':').ElementAt(0)) > rowIndex ||
                                                                                     GetRowIndex(r.Reference.Value.Split(':').ElementAt(1)) > rowIndex).ToList();
                    }

                    // Either increment or decrement the row index on the merged cell reference
                    foreach (MergeCell mergeCell in mergeCellsList)
                    {
                        string[] cellReference = mergeCell.Reference.Value.Split(':');

                        if (GetRowIndex(cellReference.ElementAt(0)) >= rowIndex)
                        {
                            string columnName = GetColumnName(cellReference.ElementAt(0));
                            cellReference[0] = isDeletedRow ? columnName + (GetRowIndex(cellReference.ElementAt(0)) - 1).ToString() : IncrementCellReference(cellReference.ElementAt(0), CellReferencePartEnum.Row);
                        }

                        if (GetRowIndex(cellReference.ElementAt(1)) >= rowIndex)
                        {
                            string columnName = GetColumnName(cellReference.ElementAt(1));
                            cellReference[1] = isDeletedRow ? columnName + (GetRowIndex(cellReference.ElementAt(1)) - 1).ToString() : IncrementCellReference(cellReference.ElementAt(1), CellReferencePartEnum.Row);
                        }

                        mergeCell.Reference = new StringValue(cellReference[0] + ":" + cellReference[1]);
                    }
                }
            }
        }

        /// <summary>
        /// Updates all hyperlinks in the worksheet when a row is inserted or deleted.
        /// </summary>
        /// <param name="worksheetPart">Worksheet Part</param>
        /// <param name="rowIndex">Row Index being inserted or deleted</param>
        /// <param name="isDeletedRow">True if row was deleted, otherwise false</param>
        private static void UpdateHyperlinkReferences(WorksheetPart worksheetPart, uint rowIndex, bool isDeletedRow)
        {
            Hyperlinks hyperlinks = worksheetPart.Worksheet.Elements<Hyperlinks>().FirstOrDefault();

            if (hyperlinks != null)
            {
                Match hyperlinkRowIndexMatch;
                uint hyperlinkRowIndex;

                foreach (Hyperlink hyperlink in hyperlinks.Elements<Hyperlink>())
                {
                    hyperlinkRowIndexMatch = Regex.Match(hyperlink.Reference.Value, "[0-9]+");
                    if (hyperlinkRowIndexMatch.Success && uint.TryParse(hyperlinkRowIndexMatch.Value, out hyperlinkRowIndex) && hyperlinkRowIndex >= rowIndex)
                    {
                        // if being deleted, hyperlink needs to be removed or moved up
                        if (isDeletedRow)
                        {
                            // if hyperlink is on the row being removed, remove it
                            if (hyperlinkRowIndex == rowIndex)
                            {
                                hyperlink.Remove();
                            }
                            // else hyperlink needs to be moved up a row
                            else
                            {
                                hyperlink.Reference.Value = hyperlink.Reference.Value.Replace(hyperlinkRowIndexMatch.Value, (hyperlinkRowIndex - 1).ToString());

                            }
                        }
                        // else row is being inserted, move hyperlink down
                        else
                        {
                            hyperlink.Reference.Value = hyperlink.Reference.Value.Replace(hyperlinkRowIndexMatch.Value, (hyperlinkRowIndex + 1).ToString());
                        }
                    }
                }

                // Remove the hyperlinks collection if none remain
                if (hyperlinks.Elements<Hyperlink>().Count() == 0)
                {
                    hyperlinks.Remove();
                }
            }
        }

        /// <summary>
        /// Given a cell name, parses the specified cell to get the row index.
        /// </summary>
        /// <param name="cellReference">Address of the cell (ie. B2)</param>
        /// <returns>Row Index (ie. 2)</returns>
        public static uint GetRowIndex(string cellReference)
        {
            // Create a regular expression to match the row index portion the cell name.
            Regex regex = new Regex(@"\d+");
            Match match = regex.Match(cellReference);

            return uint.Parse(match.Value);
        }

        /// <summary>
        /// Given a cell name, parses the specified cell to get the column name.
        /// </summary>
        /// <param name="cellReference">Address of the cell (ie. B2)</param>
        /// <returns>Column name (ie. A2)</returns>
        private static string GetColumnName(string cellName)
        {
            // Create a regular expression to match the column name portion of the cell name.
            Regex regex = new Regex("[A-Za-z]+");
            Match match = regex.Match(cellName);

            return match.Value;
        }
        public enum CellReferencePartEnum
        {
            None,
            Column,
            Row,
            Both
        }
        private static List<char> Letters = new List<char>() { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', ' ' };

        /// <summary>
        /// Increments the reference of a given cell.  This reference comes from the CellReference property
        /// on a Cell.
        /// </summary>
        /// <param name="reference">reference string</param>
        /// <param name="cellRefPart">indicates what is to be incremented</param>
        /// <returns></returns>
        public static string IncrementCellReference(string reference, CellReferencePartEnum cellRefPart)
        {
            string newReference = reference;

            if (cellRefPart != CellReferencePartEnum.None && !String.IsNullOrEmpty(reference))
            {
                string[] parts = Regex.Split(reference, "([A-Z]+)");

                if (cellRefPart == CellReferencePartEnum.Column || cellRefPart == CellReferencePartEnum.Both)
                {
                    List<char> col = parts[1].ToCharArray().ToList();
                    bool needsIncrement = true;
                    int index = col.Count - 1;

                    do
                    {
                        // increment the last letter
                        col[index] = Letters[Letters.IndexOf(col[index]) + 1];

                        // if it is the last letter, then we need to roll it over to 'A'
                        if (col[index] == Letters[Letters.Count - 1])
                        {
                            col[index] = Letters[0];
                        }
                        else
                        {
                            needsIncrement = false;
                        }

                    } while (needsIncrement && --index >= 0);

                    // If true, then we need to add another letter to the mix. Initial value was something like "ZZ"
                    if (needsIncrement)
                    {
                        col.Add(Letters[0]);
                    }

                    parts[1] = new String(col.ToArray());
                }

                if (cellRefPart == CellReferencePartEnum.Row || cellRefPart == CellReferencePartEnum.Both)
                {
                    // Increment the row number. A reference is invalid without this componenet, so we assume it will always be present.
                    parts[2] = (int.Parse(parts[2]) + 1).ToString();
                }

                newReference = parts[1] + parts[2];
            }

            return newReference;
        }

        private static Row InsertBefore(Worksheet worksheet, uint rowIndex)
        {
            Row newRow = null;
            Row row = worksheet.GetFirstChild<SheetData>().Elements<Row>().Where(r => r.RowIndex == rowIndex).FirstOrDefault();
            if (row != null)
            {
                SheetData sheetData = worksheet.GetFirstChild<SheetData>();
                newRow = new Row() { RowIndex = rowIndex };
                worksheet.InsertBefore(newRow, row);
            }
            return newRow;
        }
    }
}
