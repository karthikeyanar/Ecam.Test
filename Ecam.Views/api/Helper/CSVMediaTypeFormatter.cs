using Ecam.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;

namespace Ecam.Views {



    //public class CSVMediaTypeFormatter:MediaTypeFormatter {
    //    public CSVMediaTypeFormatter() {
    //        SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/csv"));
    //    }

    //    public CSVMediaTypeFormatter(MediaTypeMapping mediaTypeMapping)
    //        : this() {
    //        MediaTypeMappings.Add(mediaTypeMapping);
    //    }

    //    public CSVMediaTypeFormatter(IEnumerable<MediaTypeMapping> mediaTypeMappings)
    //        : this() {
    //        foreach(var mediaTypeMapping in mediaTypeMappings)
    //            MediaTypeMappings.Add(mediaTypeMapping);
    //    }

    //    private string _FileName { get; set; }

    //    public override bool CanReadType(Type type) {
    //        return false;
    //    }

    //    public override bool CanWriteType(Type type) {
    //        if(type == null)
    //            throw new ArgumentNullException("type");

    //        return IsTypeOfCSVData(type);
    //    }

    //    public override Task WriteToStreamAsync(Type type,object value,Stream stream,HttpContent content,TransportContext transportContext) {
    //        return Task.Factory.StartNew(() => WriteStream(type,value,stream,content));
    //    }

    //    //private utils

    //    private void WriteStream(Type type,object value,Stream stream,HttpContent content) {
    //        //NOTE: We have check the type inside CanWriteType method
    //        //If request comes this far, the type is IEnumerable. We are safe.


    //        Ecam.Framework.CSVData csvData = (Ecam.Framework.CSVData)value;
    //        string csvStr = Ecam.Framework.CSVHelper.CreateCSVFromGenericList(csvData.rows,csvData.column_formats);

    //        using(var streamWriter = new StreamWriter(stream))
    //            streamWriter.Write(csvStr);
    //        /*
    //        Type itemType = type.GetGenericArguments()[0];

    //        using(StringWriter stringWriter = new StringWriter()) {
    //            stringWriter.WriteLine(
    //                string.Join<string>(
    //                    ",",itemType.GetProperties().Select(x => x.Name)
    //                )
    //            );

    //            foreach(var obj in (IEnumerable<object>)value) {
    //                var vals = obj.GetType().GetProperties().Select(
    //                    pi => new {
    //                        Value = pi.GetValue(obj,null)
    //                    }
    //                );

    //                string valueLine = string.Empty;

    //                foreach(var val in vals) {
    //                    string _val = val.Value.ToString();
    //                    if(val.Value != null) {
    //                        //Check if the value contans a comma and place it in quotes if so
    //                        if(_val.Contains(","))
    //                            _val = string.Concat("\"",_val,"\"");

    //                        //Replace any \r or \n special characters from a new line with a space
    //                        if(_val.Contains("\r"))
    //                            _val = _val.Replace("\r"," ");
    //                        if(_val.Contains("\n"))
    //                            _val = _val.Replace("\n"," ");

    //                        valueLine = string.Concat(valueLine,_val,",");

    //                    } else {
    //                        valueLine = string.Concat(valueLine,",");
    //                    }
    //                }

    //                stringWriter.WriteLine(valueLine.TrimEnd(','));
    //            }

    //            using(var streamWriter = new StreamWriter(stream))
    //                streamWriter.Write(stringWriter.ToString());
    //        }
    //        */

    //    }

    //    private bool IsTypeOfCSVData(Type type) {
    //        return (type == typeof(CSVData));
    //    }

    //    public override MediaTypeFormatter GetPerRequestFormatterInstance(Type type,HttpRequestMessage request,MediaTypeHeaderValue mediaType) {
    //        if(request.Properties.ContainsKey("filename"))
    //            _FileName = request.Properties["filename"].ToString();

    //        return base.GetPerRequestFormatterInstance(type,request,mediaType);
    //    }

    //    public override void SetDefaultContentHeaders(Type type,HttpContentHeaders headers,MediaTypeHeaderValue mediaType) {
    //        base.SetDefaultContentHeaders(type,headers,mediaType);
    //        headers.Add("Content-Disposition",string.Format("attachment; filename={0}",(string.IsNullOrEmpty(_FileName) == false ? _FileName : "ExportAWBNoCSV.csv")));
    //    }
    //}
}