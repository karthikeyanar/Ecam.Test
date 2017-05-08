using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Collections;
using System.Reflection;
using System.IO;
using System.Data;
using System.Web.Http;
using System.Web.UI;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Http.Headers;

namespace Ecam.Framework {

    public class ExportToCSV<T>:IHttpActionResult {

        //private readonly ApiController _controller;

        private string _FileName;
        private T _Model;

        //private System.IO.StringWriter swr;
        //private HtmlTextWriter tw;
        private System.Text.StringBuilder sb = new StringBuilder();
        private List<Ecam.Framework.CSVColumn> _ColumnFormats;

        public ExportToCSV(string fileName,List<Ecam.Framework.CSVColumn> columnFormats,T model) {
            _ColumnFormats = columnFormats;
            _FileName = fileName;
            _Model = model;
        }

        //public string LocalPath { get; private set; }

        //public string ContentType { get; private set; }

        //public string DownloadFileName { get; private set; }

        //public HttpRequestMessage Request {
        //    get { return _controller.Request; }
        //}

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken) {
            return Task.FromResult(Execute());
        }

        private HttpResponseMessage Execute() {
            this._FileName = _FileName.ToString();
            this._FileName = FileHelper.GetValidFileName(this._FileName) + ".csv";
            IList list;
            list = (IList)_Model;
            if(list.Count > 0) {
                sb.Append(Ecam.Framework.CSVHelper.CreateCSVFromGenericList(list,_ColumnFormats));
            }

            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(sb.ToString());
            writer.Flush();
            stream.Position = 0;

            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            result.Content = new StreamContent(stream);
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("text/csv");
            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = this._FileName };
            return result;
        }

        private static string MapPath(string path) {
            // The following code is for demonstration purposes only and is not fully robust for production usage.
            // HttpContext.Current is not always available after asynchronous calls complete.
            // Also, this call is host-specific and will need to be modified for other hosts such as OWIN.
            return HttpContext.Current.Server.MapPath(path);
        }
          
    }

    public static class CSVHelper {

        private const string CSVDelimeter = ",";
        private const string CSVQualifiers = "\"\"";

        public static string ConvertCSV(Type t) {
            StringBuilder sb = new StringBuilder();
            StringBuilder sbcolumns = new StringBuilder();
            StringBuilder sbdata = new StringBuilder();
            PropertyInfo[] properties = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach(var p in properties) {
                if(p.CanRead) {
                    sbcolumns.AppendFormat("{0}{1}",ReplaceCSVFormat(p.Name),CSVDelimeter);
                }
            }
            sb.Append(sbcolumns.ToString()).AppendLine().Append(sbdata.ToString());
            return sb.ToString();
        }

        public static string ConvertCSV<T>(T objectTo,string title) {
            StringBuilder sb = new StringBuilder();
            StringBuilder sbcolumns = new StringBuilder();
            StringBuilder sbdata = new StringBuilder();
            if(String.IsNullOrEmpty(title) == false) {
                sb.Append(title).AppendLine();
            }
            IList source = (IList)objectTo;
            if(source != null) {
                var s = source.GetEnumerator();
                Type t = typeof(T);
                PropertyInfo[] properties = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach(var p in properties) {
                    if(p.CanRead) {
                        sbcolumns.AppendFormat("{0}{1}",ReplaceCSVFormat(p.Name),CSVDelimeter);
                    }
                }
                foreach(var r in source) {
                    foreach(var p in properties) {
                        if(p.CanRead) {
                            var proValue = p.GetValue(r,null);
                            string value = string.Empty;
                            if(proValue != null)
                                value = proValue.ToString();
                            sbdata.AppendFormat("{0}{1}",ReplaceCSVFormat(value),CSVDelimeter);
                        }
                    }
                }
            }
            sb.Append(sbcolumns.ToString()).AppendLine().Append(sbdata.ToString());
            return sb.ToString();
        }

        private static string ConvertCSV(IList source) {
            StringBuilder sb = new StringBuilder();
            StringBuilder sbcolumns = new StringBuilder();
            StringBuilder sbdata = new StringBuilder();
            if(source != null) {
                var properties = source.GetType().GetProperties();
                foreach(var p in properties) {
                    if(p.CanRead) {
                        sbcolumns.AppendFormat("{0}{1}",ReplaceCSVFormat(p.Name),CSVDelimeter);
                    }
                }
                foreach(var r in source) {
                    foreach(var p in properties) {
                        if(p.CanRead) {
                            var proValue = p.GetValue(r,null);
                            string value = string.Empty;
                            if(proValue != null)
                                value = proValue.ToString();
                            sbdata.AppendFormat("{0}{1}",ReplaceCSVFormat(value),CSVDelimeter);
                        }
                    }
                }
            }
            sb.Append(sbcolumns.ToString()).AppendLine().Append(sbdata.ToString());
            return sb.ToString();
        }

        private static string ReplaceCSVFormat(string value) {
            if(string.IsNullOrEmpty(value) == false) {
                value = value.Replace(CSVQualifiers,CSVQualifiers + CSVQualifiers);
                //value = value.Replace(CSVDelimeter, " ");
                value = value.Replace(Environment.NewLine," ");
            }
            return value;
        }

        //private static void ReadProperties(IList list) {
        //    //this acts as datarow
        //    foreach(var item in list) {
        //        var properties = item.GetType().GetProperties();
        //        foreach(var p in properties) {
        //            if(p.CanRead) {
        //                string s = p.Name + ",";
        //            }
        //        }
        //    }
        //}

        private static void AddColumnFormats(ref List<CSVColumn> columnFormats) {
            if(columnFormats == null) {
                columnFormats = new List<CSVColumn>();
            }
            columnFormats.Add(new Ecam.Framework.CSVColumn { PropertyName = "airline_name", DisplayName = "Airline" });
            columnFormats.Add(new Ecam.Framework.CSVColumn { PropertyName = "Errors",IsIgNore = true });
            columnFormats.Add(new Ecam.Framework.CSVColumn { PropertyName = "id",IsIgNore = true });
        }

        /// <summary>
        /// Creates the CSV from a generic list.
        /// </summary>;
        /// <typeparam name="T"></typeparam>;
        /// <param name="list">The list.</param>;
        /// <param name="csvNameWithExt">Name of CSV (w/ path) w/ file ext.</param>;
        public static DataTable CreateDataTableFromGenericList(IList list,List<CSVColumn> columnFormats = null,string tableName = "tbl") {
            DataTable dt = new DataTable();
            dt.TableName = tableName;
            //StringBuilder sb = new StringBuilder();
            AddColumnFormats(ref columnFormats);
            if(list.Count > 0) {

                string newLine = Environment.NewLine;

                //gets all properties
                PropertyInfo[] props = list[0].GetType().GetProperties();

                //foreach of the properties in class above, write out properties
                //this is the header row
                string displayName = string.Empty;
                foreach(PropertyInfo pi in props) {
                    if(pi.CanRead && pi.PropertyType.Name != "List`1"
                        && pi.PropertyType.FullName.Contains("Pepper.") == false) {
                        if(CSVHelper.IsIgNoreColumn(pi.Name,columnFormats) == false) {
                            displayName = CSVHelper.GetDisplayName(pi.Name,columnFormats);
                            dt.Columns.Add(displayName);
                            //sb.Append("\"").Append(displayName).Append("\"").Append(",");
                        }
                    }
                }
                //sb.Append(newLine);

                //this acts as datarow
                foreach(var item in list) {

                    DataRow drow = dt.NewRow();

                    //this acts as datacolumn
                    foreach(PropertyInfo pi in props) {
                        if(pi.CanRead && pi.PropertyType.Name != "List`1"
                        && pi.PropertyType.FullName.Contains("Pepper.") == false) {
                            if(CSVHelper.IsIgNoreColumn(pi.Name,columnFormats) == false) {
                                //this is the row+col intersection (the value)
                                var value = item.GetType()
                                            .GetProperty(pi.Name)
                                            .GetValue(item,null);
                                string whatToWrite = Convert.ToString(value);
                                if(CSVHelper.IsNoFormatColumn(pi.Name,columnFormats) == false) {
                                    string propertyType = item.GetType().GetProperty(pi.Name).PropertyType.FullName;
                                    if(propertyType.Contains("System.Boolean")) {
                                        bool b = false;
                                        bool.TryParse(whatToWrite,out b);
                                        whatToWrite = (b == true ? "Yes" : "No");
                                    }
                                    if(propertyType.Contains("System.DateTime")) {
                                        DateTime d;
                                        DateTime.TryParse(whatToWrite,out d);
                                        if(d.Year > 1900)
                                            whatToWrite = d.ToString("MM/dd/yyyy");
                                        else
                                            whatToWrite = string.Empty;
                                    }
                                    if(propertyType.Contains("System.Decimal")) {
                                        decimal v;
                                        decimal.TryParse(whatToWrite,out v);
                                        if(v == 0) {
                                            whatToWrite = string.Empty;
                                        } else {
                                            int precision = CSVHelper.GetPrecision(pi.Name,columnFormats);
                                            if(CSVHelper.IsNumberColumn(pi.Name,columnFormats)) {
                                                whatToWrite = FormatHelper.NumberFormat(v,precision);
                                            } else if(CSVHelper.IsPercentageColumn(pi.Name,columnFormats)) {
                                                whatToWrite = FormatHelper.PercentageFormat(v);
                                            } else {
                                                whatToWrite = FormatHelper.CurrencyFormat(v,precision);
                                            }
                                        }
                                    }
                                }
                                //sb.Append("\"").Append(ReplaceCSVFormat(whatToWrite)).Append("\"").Append(CSVDelimeter);
                                displayName = CSVHelper.GetDisplayName(pi.Name,columnFormats);
                                drow[displayName] = whatToWrite;
                            }
                        }
                    }
                    dt.Rows.Add(drow);
                    //sb.Append(newLine);
                }
            }
            //return sb.ToString();
            return dt;
        }

        /// <summary>
        /// Creates the CSV from a generic list.
        /// </summary>;
        /// <typeparam name="T"></typeparam>;
        /// <param name="list">The list.</param>;
        /// <param name="csvNameWithExt">Name of CSV (w/ path) w/ file ext.</param>;
        public static string CreateCSVFromGenericList(IList list,List<CSVColumn> columnFormats = null) {
            StringBuilder sb = new StringBuilder();
            AddColumnFormats(ref columnFormats);
            if(list.Count > 0) {

                string newLine = Environment.NewLine;

                //gets all properties
                PropertyInfo[] props = list[0].GetType().GetProperties();

                //foreach of the properties in class above, write out properties
                //this is the header row
                string displayName = string.Empty;
                foreach(PropertyInfo pi in props) {
                    if(pi.CanRead && pi.PropertyType.Name != "List`1"
                         && pi.PropertyType.FullName.Contains("Pepper.") == false) {
                        int cnt = (from q in columnFormats where q.PropertyName == pi.Name select q).Count();
                        if (cnt > 0)
                        {
                            if (CSVHelper.IsIgNoreColumn(pi.Name, columnFormats) == false)
                            {
                                displayName = CSVHelper.GetDisplayName(pi.Name, columnFormats);
                                sb.Append("\"").Append(displayName).Append("\"").Append(",");
                            }
                        }
                    }
                }
                sb.Append(newLine);

                //this acts as datarow
                foreach(var item in list) {
                    //this acts as datacolumn
                    foreach(PropertyInfo pi in props) {
                        if (pi.CanRead && pi.PropertyType.Name != "List`1"
                        && pi.PropertyType.FullName.Contains("Pepper.") == false)
                        {
                            int cnt = (from q in columnFormats where q.PropertyName == pi.Name select q).Count();
                            if (cnt > 0)
                            {
                                if (CSVHelper.IsIgNoreColumn(pi.Name, columnFormats) == false)
                                {
                                    //this is the row+col intersection (the value)
                                    var value = item.GetType()
                                                .GetProperty(pi.Name)
                                                .GetValue(item, null);
                                    string whatToWrite = Convert.ToString(value);
                                    if (CSVHelper.IsNoFormatColumn(pi.Name, columnFormats) == false)
                                    {
                                        string propertyType = item.GetType().GetProperty(pi.Name).PropertyType.FullName;
                                        if (propertyType.Contains("System.Boolean"))
                                        {
                                            bool b = false;
                                            bool.TryParse(whatToWrite, out b);
                                            whatToWrite = (b == true ? "Yes" : "No");
                                        }
                                        if (propertyType.Contains("System.DateTime"))
                                        {
                                            DateTime d;
                                            DateTime.TryParse(whatToWrite, out d);
                                            if (d.Year > 1900)
                                                whatToWrite = d.ToString("MM/dd/yyyy");
                                            else
                                                whatToWrite = string.Empty;
                                        }
                                        if (propertyType.Contains("System.Decimal"))
                                        {
                                            decimal v;
                                            decimal.TryParse(whatToWrite, out v);
                                            if (v == 0)
                                            {
                                                whatToWrite = string.Empty;
                                            }
                                            else
                                            {
                                                int precision = CSVHelper.GetPrecision(pi.Name, columnFormats);
                                                if (CSVHelper.IsNumberColumn(pi.Name, columnFormats))
                                                {
                                                    whatToWrite = FormatHelper.NumberFormat(v, precision);
                                                }
                                                else if (CSVHelper.IsPercentageColumn(pi.Name, columnFormats))
                                                {
                                                    whatToWrite = FormatHelper.PercentageFormat(v);
                                                }
                                                else
                                                {
                                                    whatToWrite = FormatHelper.CurrencyFormat(v, precision);
                                                }
                                            }
                                        }
                                    }
                                    sb.Append("\"").Append(ReplaceCSVFormat(whatToWrite)).Append("\"").Append(CSVDelimeter);
                                }
                            }
                        }
                    }
                    sb.Append(newLine);
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Creates the CSV from a generic list.
        /// </summary>;
        /// <typeparam name="T"></typeparam>;
        /// <param name="list">The list.</param>;
        /// <param name="csvNameWithExt">Name of CSV (w/ path) w/ file ext.</param>;
        public static string CreateTableFromGenericList(IList list,List<CSVColumn> columnFormats = null) {
            StringBuilder sb = new StringBuilder();
            AddColumnFormats(ref columnFormats);
            if(list.Count > 0) {

                string newLine = Environment.NewLine;

                sb.Append("<table cellpadding=0 cellspacing=0 border=1>");
                sb.Append("<thead>");
                sb.Append("<tr>");

                //gets all properties
                PropertyInfo[] props = list[0].GetType().GetProperties();

                //foreach of the properties in class above, write out properties
                //this is the header row
                string displayName = string.Empty;
                string align = string.Empty;
                foreach(PropertyInfo pi in props) {
                    if(pi.CanRead && pi.PropertyType.Name != "List`1"
                        && pi.PropertyType.FullName.Contains("Pepper.") == false) {
                        if(CSVHelper.IsIgNoreColumn(pi.Name,columnFormats) == false) {
                            displayName = CSVHelper.GetDisplayName(pi.Name,columnFormats);
                            align = "style=\"text-align:left\"";
                            string propertyType = pi.PropertyType.FullName.ToString();
                            if(propertyType.Contains("System.Decimal")) {
                                align = "style=\"text-align:right\"";
                            }
                            sb.Append("<th " + align + ">").Append(displayName).Append("</th>");
                        }
                    }
                }

                sb.Append("</tr>");
                sb.Append("</thead>");

                sb.Append("<tbody>");

                //this acts as datarow
                foreach(var item in list) {
                    sb.Append("<tr>");
                    //this acts as datacolumn
                    foreach(PropertyInfo pi in props) {
                        if(pi.CanRead && pi.PropertyType.Name != "List`1"
                        && pi.PropertyType.FullName.Contains("Pepper.") == false) {
                            if(CSVHelper.IsIgNoreColumn(pi.Name,columnFormats) == false) {
                                //this is the row+col intersection (the value)
                                var value = item.GetType()
                                            .GetProperty(pi.Name)
                                            .GetValue(item,null);
                                align = "style=\"text-align:left\"";
                                string whatToWrite = Convert.ToString(value);
                                if(CSVHelper.IsNoFormatColumn(pi.Name,columnFormats) == false) {
                                    string propertyType = item.GetType().GetProperty(pi.Name).PropertyType.FullName;
                                    if(propertyType.Contains("System.Boolean")) {
                                        bool b = false;
                                        bool.TryParse(whatToWrite,out b);
                                        whatToWrite = (b == true ? "Yes" : "No");
                                    }
                                    if(propertyType.Contains("System.DateTime")) {
                                        DateTime d;
                                        DateTime.TryParse(whatToWrite,out d);
                                        if(d.Year > 1900)
                                            whatToWrite = d.ToString("MM/dd/yyyy");
                                        else
                                            whatToWrite = string.Empty;
                                    }
                                    if(propertyType.Contains("System.Decimal")) {
                                        decimal v;
                                        decimal.TryParse(whatToWrite,out v);
                                        if(v == 0) {
                                            whatToWrite = string.Empty;
                                        } else {
                                            if(CSVHelper.IsNumberColumn(pi.Name,columnFormats)) {
                                                whatToWrite = FormatHelper.NumberFormat(v);
                                            } else if(CSVHelper.IsPercentageColumn(pi.Name,columnFormats)) {
                                                whatToWrite = FormatHelper.PercentageFormat(v);
                                            } else {
                                                whatToWrite = FormatHelper.CurrencyFormat(v);
                                            }
                                        }
                                        align = "style=\"text-align:right\"";
                                    }
                                }
                                sb.Append("<td " + align + ">").Append(whatToWrite).Append("</td>");
                            }
                        }
                    }
                    sb.Append("</tr>");
                }
                sb.Append("</tbody>");
                sb.Append("</table>");
            }
            return sb.ToString();
        }

        public static string GetDisplayName(string propertyName,List<CSVColumn> columnFormats) {
            string displayName = string.Empty;
            if(columnFormats == null)
                displayName = string.Empty;
            else
                displayName = (from c in columnFormats
                               where c.PropertyName == propertyName
                               select c.DisplayName).FirstOrDefault();
            if(string.IsNullOrEmpty(displayName))
                return propertyName;
            else
                return displayName;
        }

        public static int GetPrecision(string propertyName,List<CSVColumn> columnFormats) {
            int precision = (from c in columnFormats
                             where c.PropertyName.ToLower() == propertyName.ToLower()
                             select c.Precision).FirstOrDefault();
            if(precision <= 0) {
                precision = 2;
            }
            if(propertyName.ToLower().Contains("number")
                   || propertyName.ToLower().Contains("quantity")
                   || propertyName.ToLower().Contains("shares")) {
                precision = 4;
            }
            return precision;
        }

        public static bool IsIgNoreColumn(string propertyName,List<CSVColumn> columnFormats) {
            if(columnFormats == null)
                return false;
            else
                return (from c in columnFormats
                        where c.IsIgNore == true && c.PropertyName.ToLower() == propertyName.ToLower()
                        select c).Count() > 0;
        }

        //private static bool IsCurrencyColumn(string propertyName, List<CSVColumn> columnFormats) {
        //    if (columnFormats == null)
        //        return false;
        //    else
        //        return (from c in columnFormats
        //                where c.PropertyName == propertyName
        //                select c.IsCurrency).FirstOrDefault();
        //}

        public static bool IsNoFormatColumn(string propertyName,List<CSVColumn> columnFormats) {
            return (from c in columnFormats
                    where c.PropertyName == propertyName
                    select c.IsNoFormat).FirstOrDefault();
        }

        public static bool IsNumberColumn(string propertyName,List<CSVColumn> columnFormats) {
            if(string.IsNullOrEmpty(propertyName) == false) {
                if(propertyName.ToLower().Contains("share"))
                    return true;
                if(propertyName.ToLower().Contains("multiple"))
                    return true;
                if(propertyName.ToLower().Contains("number"))
                    return true;
                if(propertyName.ToLower().Contains("split"))
                    return true;
                if(propertyName.ToLower().Contains("quantity"))
                    return true;
            }
            if(columnFormats == null)
                return false;
            else
                return (from c in columnFormats
                        where c.PropertyName == propertyName
                        select c.IsNumber).FirstOrDefault();
        }

        public static bool IsPercentageColumn(string propertyName,List<CSVColumn> columnFormats) {
            if(string.IsNullOrEmpty(propertyName) == false) {
                if(propertyName.ToLower().Contains("percentage"))
                    return true;
                if(propertyName.ToLower().Contains("%"))
                    return true;
            }
            if(columnFormats == null)
                return false;
            else
                return (from c in columnFormats
                        where c.PropertyName == propertyName
                        select c.IsPercentage).FirstOrDefault();
        }
    }

    public class CSVColumn {

        public CSVColumn() {
            this.Precision = 2;
        }

        public string PropertyName { get; set; }

        public string DisplayName { get; set; }

        public bool IsNumber { get; set; }

        public bool IsPercentage { get; set; }

        public bool IsIgNore { get; set; }

        public bool IsNoFormat { get; set; }

        public int Precision { get; set; }
    }

}
