using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.Text;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;
using System.Configuration;
using MySql.Data.MySqlClient;
using System.Text.RegularExpressions;
using System.Data;
using System.Collections;
using DocumentFormat.OpenXml.Drawing;
using System.Data.Common;
using Newtonsoft.Json;
using System.Security.Cryptography;

namespace Ecam.Framework
{
    public static class HtmlControls
    {

        #region Url
        public static string Url(this HtmlHelper helper)
        {
            return new UrlHelper(helper.ViewContext.RequestContext).Content("~/");
        }
        public static string Url(this HtmlHelper helper, string content)
        {
            if (content != null)
            {
                if (content.StartsWith("~/") == false)
                {
                    content = "~" + (content.StartsWith("/") == false ? "/" + content : content);
                }
            }
            return new UrlHelper(helper.ViewContext.RequestContext).Content(content);
        }
        public static string Url(this HtmlHelper helper, string actionName, string controllerName)
        {
            return new UrlHelper(helper.ViewContext.RequestContext).Action(actionName, controllerName);
        }
        #endregion

        private static string GetVersion(string fileName)
        {
            try
            {
                string isLocal = System.Configuration.ConfigurationManager.AppSettings["is_local"];
                if (isLocal == "true")
                {
                    string fullFileName = HttpContext.Current.Server.MapPath(fileName);
                    System.IO.FileInfo fileInfo = new System.IO.FileInfo(fullFileName);
                    fileName = string.Format("{0}?v={1}", fileName, fileInfo.LastWriteTime.ToString("yyyyMMddhhmmss"));
                }
                else
                {
                    fileName = string.Format("{0}?v={1}", fileName, System.Configuration.ConfigurationManager.AppSettings["product_version"]);
                }
            }
            catch
            {
            }
            return fileName;
        }

        #region javascript
        public static MvcHtmlString JavascriptInclueTag(this HtmlHelper helper, string scriptFileName)
        {
            return MvcHtmlString.Create(string.Format("<script src=\"{0}\" type=\"text/javascript\"></script>", Url(helper, GetVersion(string.Format("{0}", scriptFileName)))));
        }
        #endregion

        #region stylesheet
        public static MvcHtmlString StylesheetLinkTag(this HtmlHelper helper, string cssname)
        {
            return MvcHtmlString.Create(string.Format("<link href=\"{0}\" rel=\"stylesheet\" type=\"text/css\" />",
                Url(helper, GetVersion(string.Format("{0}", cssname)))));
        }
        #endregion

        //public static string GetTemplateHtml<T>(string templatePath, T viewObject)
        //{
        //    string html = Westwind.Web.Mvc.ViewRenderer.RenderView(templatePath, viewObject);
        //    return html;
        //}

        public static Dictionary<string, string> GetRequestCollections(HttpRequest req)
        {
            Dictionary<string, string> t = new Dictionary<string, string>();
            if (req != null)
            {
                if (req.QueryString != null)
                {
                    if (req.QueryString.Count > 0)
                    {
                        foreach (string key in req.QueryString.AllKeys)
                        {
                            if (t.ContainsKey(key) == false)
                            {
                                t.Add(key, req.QueryString[key]);
                            }
                        }
                    }
                }
                if (req.Params != null)
                {
                    if (req.Params.Count > 0)
                    {
                        foreach (string key in req.Params.AllKeys)
                        {
                            if (t.ContainsKey(key) == false)
                            {
                                t.Add(key, req.Params[key]);
                            }
                        }
                    }
                }
            }
            return t;
        }

        public static string GetCSSContent(string fileName)
        {
            string content = string.Empty;
            try
            {
                string fullFileName = HttpContext.Current.Server.MapPath(fileName);
                System.IO.FileInfo fileInfo = new System.IO.FileInfo(fullFileName);
                if (fileInfo.Exists)
                {
                    content = System.IO.File.ReadAllText(fileInfo.FullName);
                }
            }
            catch
            {
            }
            return content;
        }
    }

}

