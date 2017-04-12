using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Winnovative.WnvHtmlConvert;

namespace Ecam.Framework {

    public class ExportPdfFile
    {

        public string FileName;
        public string Html;

        public string PdfFullName { get; set; }

        public string PdfName { get; set; }

        public string SourceFileName { get; set; }

        public string DestinationFileName { get; set; }

        public int PageWidth { get; set; }

        public int PageHeight { get; set; }

        public bool IsLandscape { get; set; }

        public ExportPdfFile(string fileName, string html, int pageWidth = 0, int pageHeight = 0, bool isLandscape = false)
        {
            this.FileName = fileName;
            this.Html = html;
            this.PdfFullName = string.Empty;
            this.PdfName = string.Empty;
            this.SourceFileName = string.Empty;
            this.DestinationFileName = string.Empty;
            this.PageWidth = pageWidth;
            this.PageHeight = pageHeight;
            this.IsLandscape = isLandscape;
        }

        public void ExportPdf()
        {

            PdfConverter pdfConverterObj = new PdfConverter();

            Guid guid = System.Guid.NewGuid();

            this.FileName = FileName.ToString();

            this.FileName = FileHelper.GetValidFileName(this.FileName) + ".pdf";

            this.SourceFileName = this.FileName.Replace(".pdf", ".htm");

            //this.SourceFileName = UploadFileHelper.GetFullFileName(Helper.TempPathSettingName, this.SourceFileName);

            this.DestinationFileName = this.FileName;

            //this.DestinationFileName = UploadFileHelper.GetFullFileName(Helper.TempPathSettingName, this.DestinationFileName);

            if (UploadFileHelper.FileExist(Helper.TempPathSettingName, this.SourceFileName))
            {
                UploadFileHelper.DeleteFile(Helper.TempPathSettingName, this.SourceFileName);
            }

            if (UploadFileHelper.FileExist(Helper.TempPathSettingName, this.DestinationFileName))
            {
                UploadFileHelper.DeleteFile(Helper.TempPathSettingName, this.DestinationFileName);
            }


            byte[] pdfBytes;
            pdfConverterObj.LicenseKey = "saFXUumYqQkFyGSJMqYqJTEaCLsFY+YJvYZ1w/jOpg/SALDg50HlcMtPcu1Xbinw";
            pdfConverterObj.PdfDocumentOptions.PdfPageSize = PdfPageSize.A4;

            if (this.PageWidth > 0)
            {
                pdfConverterObj.PageWidth = this.PageWidth;
            }
            if (this.PageHeight > 0)
            {
                pdfConverterObj.PageHeight = this.PageHeight;
            }

            pdfConverterObj.PdfDocumentOptions.PdfCompressionLevel = PdfCompressionLevel.Normal;
            if (this.IsLandscape == true)
            {
                pdfConverterObj.PdfDocumentOptions.PdfPageOrientation = PDFPageOrientation.Landscape;
            }
            pdfConverterObj.PdfDocumentOptions.ShowHeader = false;
            pdfConverterObj.PdfDocumentOptions.ShowFooter = false;
            pdfConverterObj.PdfDocumentOptions.LeftMargin = 10;
            pdfConverterObj.PdfDocumentOptions.RightMargin = 10;
            pdfConverterObj.PdfDocumentOptions.TopMargin = 10;
            pdfConverterObj.PdfDocumentOptions.BottomMargin = 10;
            pdfConverterObj.PdfDocumentOptions.GenerateSelectablePdf = true;
            pdfConverterObj.PdfDocumentOptions.LiveUrlsEnabled = true;
            pdfConverterObj.PdfBookmarkOptions.TagNames = null;

            FileInfo htmlFileInfo = UploadFileHelper.WriteFileText(Helper.TempPathSettingName, this.SourceFileName, this.Html, false);
            if (UploadFileHelper.FileExist(Helper.TempPathSettingName, this.SourceFileName))
            {
                pdfBytes = pdfConverterObj.GetPdfBytesFromHtmlFile(htmlFileInfo.FullName);
                FileInfo destFileInfo = UploadFileHelper.WriteFileAllBytes(Helper.TempPathSettingName, this.DestinationFileName, pdfBytes);
                UploadFileHelper.DeleteFile(Helper.TempPathSettingName, this.SourceFileName);
                if (UploadFileHelper.FileExist(Helper.TempPathSettingName, this.DestinationFileName))
                {
                    this.PdfFullName = destFileInfo.FullName;
                    this.PdfName = destFileInfo.Name;
                }
            }
        }
         
    }

    public class ExportToPdf:ActionResult {

        public ExportPdfFile ExportPdfFile { get; set; }

        public ExportToPdf(string fileName,string html,int pageWidth = 0,int pageHeight = 0,bool isLandscape = false) {
            this.ExportPdfFile = new ExportPdfFile(fileName, html, pageWidth, pageHeight, isLandscape);
        }

        public void ExportPdf() {
            this.ExportPdfFile.ExportPdf();
        }

        public string PdfName { get; set; }
        public string PdfFullName { get; set; }

        public override void ExecuteResult(ControllerContext context) {

            this.ExportPdf();
            this.PdfName = this.ExportPdfFile.PdfName;
            this.PdfFullName = this.ExportPdfFile.PdfFullName;
            

            if(UploadFileHelper.FileExist(Helper.TempPathSettingName,this.ExportPdfFile.DestinationFileName)) {

                byte[] bytes = System.IO.File.ReadAllBytes(this.ExportPdfFile.PdfFullName);
                UploadFileHelper.DeleteFile(Helper.TempPathSettingName, this.ExportPdfFile.DestinationFileName);
                context.HttpContext.Response.Buffer = true;
                context.HttpContext.Response.Clear();
                context.HttpContext.Response.ClearHeaders();
                context.HttpContext.Response.AddHeader("content-disposition","attachment; filename=" + this.ExportPdfFile.FileName + ";" +
                "size=" + bytes.Length.ToString() + "; " +
                "creation-date=" + DateTime.Now.ToString("R").Replace(",","") + "; " +
                "modification-date=" + DateTime.Now.ToString("R").Replace(",","") + "; " +
                "read-date=" + DateTime.Now.ToString("R").Replace(",",""));
                context.HttpContext.Response.ContentType = "application/pdf";
                context.HttpContext.Response.BinaryWrite(bytes);

            }
        }
    }
}
