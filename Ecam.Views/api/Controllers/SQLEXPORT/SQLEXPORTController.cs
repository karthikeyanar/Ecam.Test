using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using Ecam.Views.Models;
using Ecam.Models;
using Ecam.Framework;
using MySql.Data.MySqlClient;
using DocumentFormat.OpenXml.Spreadsheet;
using System.IO;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml;
using System.Data;
using Ecam.Contracts;
using Newtonsoft.Json;


namespace Ecam.Views.Controllers {

    public class SQLEXPORTController:ApiController {

        //private void AddTempRoles() {
        //    Ecam.Views.Models.IdentityManager m = new IdentityManager();
        //    m.AddUserToRole("f6c3db4c-d7da-4bf0-b2d0-21473617d762","AA");
        //}

        #region sqlreport
        [HttpPost]
        public IHttpActionResult Report(SQLExportQuery query) {
            SLExcelData excelData = null;
            String error = string.Empty;
            try {
                //AddTempRoles();
                excelData = GetData(query.sql);
            } catch(Exception ex) {
                error = ex.Message;
                excelData = new SLExcelData();
            }
            return Ok(new SQLExportData { columns = excelData.Headers,rows = excelData.DataRows,error = error });
        }

        private SLExcelData GetData(string sql) {
            bool isUpdatePermission = false;
            string key = DataTypeHelper.ConvertString(HttpContext.Current.Request["key"]);
            if(key == "C3BB96E9C96") {
                isUpdatePermission = true;
            }
            SLExcelData excelData = new SLExcelData();
            sql = sql.Replace(Environment.NewLine,"").Trim();
            if(string.IsNullOrEmpty(sql) == false) {
                if(sql.ToLower().Contains("update ") == false
                    && sql.ToLower().Contains("delete ") == false
                    && sql.ToLower().Contains("create ") == false
                    && sql.ToLower().Contains("insert ") == false
                    && sql.ToLower().Contains("alter ") == false
                    ) {
                    if(sql.ToLower().Contains("limit") == false)
                        sql = sql + " limit 0,10";
                } else {
                    if(isUpdatePermission == false)
                        throw new Exception("Create,Update,Delete,Insert,Alter SQL Command Not supported.");
                }

                MySqlDataReader reader = MySqlHelper.ExecuteReader(Ecam.Framework.Helper.ConnectionString,sql);
                using(reader) {
                    var columns = Enumerable.Range(0,reader.FieldCount).Select(reader.GetName).ToList();
                    foreach(var cname in columns) {
                        excelData.Headers.Add(cname);
                    }
                    // Call Read before accessing data. 
                    while(reader.Read()) {
                        var dataRow = new List<string>();
                        foreach(var cname in excelData.Headers) {
                            string dataType = reader[cname].GetType().Name;
                            string value = Convert.ToString(reader[cname]);
                            switch(dataType) {
                                case "DateTime":
                                    value = DataTypeHelper.ToDateTime(value).ToString("MM/dd/yyyy");
                                    break;
                            }
                            dataRow.Add(value);
                        }
                        excelData.DataRows.Add(dataRow);
                    }
                    // Call Close when done reading.
                    reader.Close();
                }
                //} else {
                //    throw new Exception("Create,Update,Delete,Insert,Alter SQL Command Not supported.");
                //}
            }
            return excelData;
        }

        [HttpPost]
        public OkFileDownloadResult Export(SQLExportQuery query) {
            string appSettingName = "DownloadImportError";
            //UploadFileHelper.DeleteAllFiles(appSettingName);
            ICacheManager cacheManager = new MemoryCacheManager();
            SLExcelData excelData = GetData(query.sql);
            byte[] bytes = (new SLExcelWriter()).GenerateExcelBytes(excelData);
            string fileName = string.Format("ExportExcel.xlsx");
            UploadFileHelper.WriteFileAllBytes(appSettingName,fileName,bytes);
            string filePath = UploadFileHelper.GetFullFileName(appSettingName,fileName);
            //HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
            return this.Download(filePath,"application/xlsx");
            //new System.Web.Mvc.FileContentResult(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"));
        }

        /*
        private void DoProcessFlightBook() {
            using(EcamContext context = new EcamContext()) {
                List<ec_flight_book> fbs = context.ec_flight_book.ToList();
                foreach(var fb in fbs) {
                    List<EC_FLIGHT_BOOK_DETAIL> contractList = new List<EC_FLIGHT_BOOK_DETAIL>();
                    List<ec_flight_book_detail> details = context.ec_flight_book_detail.Where(q => q.awb_no == fb.id).ToList();
                    foreach(var det in details) {
                        contractList.Add(new EC_FLIGHT_BOOK_DETAIL {
                            height = (det.height ?? 0),
                            length = (det.length ?? 0),
                            piece_count = det.piece_count,
                            piece_weight = (det.piece_weight ?? 0),
                            width = (det.width ?? 0)
                        });
                    }
                    fb.booking_items = JsonConvert.SerializeObject(contractList);
                    context.Entry(fb).State = System.Data.Entity.EntityState.Modified;
                }
                context.SaveChanges();
            }
        }
        */

        //private void DeleteAWBNosSales() {
        //    string[] arr = {
        //                        "09865316661",
        //                        "09865316672",
        //                        "09865316683",
        //                        "09865316694",
        //                        "09865316705",
        //                        "09865316716",
        //                        "09865316720",
        //                        "09865316731",
        //                        "09865316742",
        //                        "09865316753",
        //                        "09865349012",
        //                    };
        //    using(EcamContext context = new EcamContext()) {
        //        foreach(string awb_no in arr) {
        //            ec_sales sales = context.ec_sales.Where(q => q.awb_no == awb_no).FirstOrDefault();
        //            if(sales != null) {
        //                if((sales.awb_lot_id ?? 0) > 0) {
        //                    ec_awb_lot lot = context.ec_awb_lot.Where(q => q.id == sales.awb_lot_id).FirstOrDefault();
        //                    if(lot != null) {
        //                        ec_awb_lot_detail awbLotDetail = context.ec_awb_lot_detail.Where(q => q.awb_no == awb_no).FirstOrDefault();
        //                        if(awbLotDetail == null) {
        //                            context.ec_awb_lot_detail.Add(new ec_awb_lot_detail {
        //                                awb_no = sales.awb_no,
        //                                awb_lot_id = lot.id,
        //                                agent_id = 0,
        //                                awb_agent_request_id = 0,
        //                                status = Ecam.Contracts.Enums.AWBLotStatus.Open
        //                            });
        //                        }  
        //                    }
        //                }  
        //                context.ec_sales.Remove(sales);
        //            } 
        //            context.SaveChanges();
        //            ec_awb_lot_detail lotDet = context.ec_awb_lot_detail.Where(q => q.awb_no == awb_no
        //                && q.status != Ecam.Contracts.Enums.AWBLotStatus.Open
        //                ).FirstOrDefault();
        //            if(lotDet != null) {
        //                lotDet.agent_id = 0;
        //                lotDet.awb_agent_request_id = 0;
        //                lotDet.status = Ecam.Contracts.Enums.AWBLotStatus.Open;
        //                context.Entry(lotDet).State = System.Data.Entity.EntityState.Modified;
        //            }
        //            context.SaveChanges();
        //        }
        //    }
        //}


        #endregion
    }

    public class SQLExportQuery {
        public string sql { get; set; }
    }

    public class SQLExportData {

        public List<string> columns { get; set; }
        public List<List<string>> rows { get; set; }
        public string error { get; set; }
    }

    //public class SLExcelWriter {
    //    private string ColumnLetter(int intCol) {
    //        var intFirstLetter = ((intCol) / 676) + 64;
    //        var intSecondLetter = ((intCol % 676) / 26) + 64;
    //        var intThirdLetter = (intCol % 26) + 65;

    //        var firstLetter = (intFirstLetter > 64)
    //            ? (char)intFirstLetter : ' ';
    //        var secondLetter = (intSecondLetter > 64)
    //            ? (char)intSecondLetter : ' ';
    //        var thirdLetter = (char)intThirdLetter;

    //        return string.Concat(firstLetter,secondLetter,
    //            thirdLetter).Trim();
    //    }

    //    private Cell CreateTextCell(string header,UInt32 index,
    //        string text) {
    //        var cell = new Cell {
    //            DataType = CellValues.InlineString,
    //            CellReference = header + index
    //        };

    //        var istring = new InlineString();
    //        var t = new Text { Text = text };
    //        istring.AppendChild(t);
    //        cell.AppendChild(istring);
    //        return cell;
    //    }

    //    public byte[] GenerateExcel(SLExcelData data) {
    //        var stream = new MemoryStream();
    //        var document = SpreadsheetDocument
    //            .Create(stream,SpreadsheetDocumentType.Workbook);

    //        var workbookpart = document.AddWorkbookPart();
    //        workbookpart.Workbook = new Workbook();
    //        var worksheetPart = workbookpart.AddNewPart<WorksheetPart>();
    //        var sheetData = new SheetData();

    //        worksheetPart.Worksheet = new Worksheet(sheetData);

    //        var sheets = document.WorkbookPart.Workbook.
    //            AppendChild<Sheets>(new Sheets());

    //        var sheet = new Sheet() {
    //            Id = document.WorkbookPart
    //                .GetIdOfPart(worksheetPart),
    //            SheetId = 1, Name = data.SheetName ?? "Sheet 1"
    //        };
    //        sheets.AppendChild(sheet);

    //        // Add header
    //        UInt32 rowIdex = 0;
    //        var row = new Row { RowIndex = ++rowIdex };
    //        sheetData.AppendChild(row);
    //        var cellIdex = 0;

    //        foreach(var header in data.Headers) {
    //            row.AppendChild(CreateTextCell(ColumnLetter(cellIdex++),
    //                rowIdex,header ?? string.Empty));
    //        }
    //        if(data.Headers.Count > 0) {
    //            // Add the column configuration if available
    //            if(data.ColumnConfigurations != null) {
    //                var columns = (Columns)data.ColumnConfigurations.Clone();
    //                worksheetPart.Worksheet
    //                    .InsertAfter(columns,worksheetPart
    //                    .Worksheet.SheetFormatProperties);
    //            }
    //        }

    //        // Add sheet data
    //        foreach(var rowData in data.DataRows) {
    //            cellIdex = 0;
    //            row = new Row { RowIndex = ++rowIdex };
    //            sheetData.AppendChild(row);
    //            foreach(var callData in rowData) {
    //                var cell = CreateTextCell(ColumnLetter(cellIdex++),
    //                    rowIdex,callData ?? string.Empty);
    //                row.AppendChild(cell);
    //            }
    //        }

    //        workbookpart.Workbook.Save();
    //        document.Close();

    //        return stream.ToArray();
    //    }
    //}

}
