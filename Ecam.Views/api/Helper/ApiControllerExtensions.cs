using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;


namespace Ecam.Views {

    //static class ApiControllerExtensions {
    //    public static OkTextPlainResult Text(this ApiController controller, string content) {
    //        return Text(controller, content, Encoding.UTF8);
    //    }

    //    public static OkTextPlainResult Text(this ApiController controller, string content, Encoding encoding) {
    //        if (controller == null) {
    //            throw new ArgumentNullException("controller");
    //        }

    //        return new OkTextPlainResult(content, encoding, controller);
    //    }

    //    public static OkFileDownloadResult Download(this ApiController controller,string path,string contentType) {
    //        string downloadFileName = Path.GetFileName(path);
    //        return Download(controller,path,contentType,downloadFileName);
    //    }

    //    public static OkFileDownloadResult Download(this ApiController controller, string path, string contentType, bool isDeleteFile, string downloadFileName) {
    //        if (controller == null) {
    //            throw new ArgumentNullException("controller");
    //        }

    //        return new OkFileDownloadResult(path, contentType, downloadFileName, isDeleteFile, controller);
    //    }
    //}

    static class ApiControllerExtensions {
        public static OkTextPlainResult Text(this ApiController controller,string content) {
            return Text(controller,content,Encoding.UTF8);
        }

        public static OkTextPlainResult Text(this ApiController controller,string content,Encoding encoding) {
            if(controller == null) {
                throw new ArgumentNullException("controller");
            }

            return new OkTextPlainResult(content,encoding,controller);
        }

        public static OkFileDownloadResult Download(this ApiController controller,string path,string contentType) {
            string downloadFileName = Path.GetFileName(path);
            return Download(controller,path,contentType,downloadFileName);
        }

        public static OkFileDownloadResult Download(this ApiController controller,string path,string contentType,string downloadFileName) {
            if(controller == null) {
                throw new ArgumentNullException("controller");
            }

            return new OkFileDownloadResult(path,contentType,downloadFileName,controller);
        }
    }


    public class HtmlActionResult:IHttpActionResult {
        //private const string ViewDirectory = @"E:devConsoleApplication8ConsoleApplication8";
        private readonly string _view;
        private readonly dynamic _model;

        public HtmlActionResult(string viewName,dynamic model) {
            //_view = LoadView(viewName);
            _view = viewName;
            _model = model;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken) {
            string html = Westwind.Web.Mvc.ViewRenderer.RenderView(_view,_model);
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent(html);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return Task.FromResult(response);
            /*
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            var parsedView =  RazorEngine.Razor.Parse(_view,_model);
            response.Content = new StringContent(parsedView);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return Task.FromResult(response);
            */
        }

        //private static string LoadView(string name) {
        //    var view = File.ReadAllText(Path.Combine(ViewDirectory,name + ".cshtml"));
          //  return view;
        //}
        
    }
}