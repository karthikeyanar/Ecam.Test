using System;
using System.IO.Compression;
using System.Net;
using System.Web;
using System.Web.Optimization;

namespace Ecam.Views {

    public class GZipBundle:Bundle {
        public GZipBundle(string virtualPath,params IBundleTransform[] transforms)
            : base(virtualPath,null,transforms) { }

        public override BundleResponse CacheLookup(BundleContext context) {
            if(null != context) GZipEncodePage(context.HttpContext);
            return base.CacheLookup(context);
        }

        // Sets up the current page or handler to use GZip through a Response.Filter.
        public static void GZipEncodePage(HttpContextBase httpContext) {
            if(null != httpContext && null != httpContext.Request && null != httpContext.Response
                && (null == httpContext.Response.Filter
                || !(httpContext.Response.Filter is GZipStream || httpContext.Response.Filter is DeflateStream))) {
                // Is GZip supported?
                string acceptEncoding = httpContext.Request.Headers["Accept-Encoding"];
                if(null != acceptEncoding
                    && acceptEncoding.IndexOf(DecompressionMethods.GZip.ToString(),StringComparison.OrdinalIgnoreCase) >= 0) {
                    httpContext.Response.Filter = new GZipStream(httpContext.Response.Filter,CompressionMode.Compress);
                    httpContext.Response.AddHeader("Content-Encoding",DecompressionMethods.GZip.ToString().ToLowerInvariant());
                } else if(null != acceptEncoding
                      && acceptEncoding.IndexOf(DecompressionMethods.Deflate.ToString(),StringComparison.OrdinalIgnoreCase) >= 0) {
                    httpContext.Response.Filter = new DeflateStream(httpContext.Response.Filter,CompressionMode.Compress);
                    httpContext.Response.AddHeader("Content-Encoding",DecompressionMethods.Deflate.ToString().ToLowerInvariant());
                }

                // Allow proxy servers to cache encoded and unencoded versions separately
                httpContext.Response.AppendHeader("Vary","Content-Encoding");
            }
        }
    }

    // Represents a bundle that does CSS minification and GZip compression.
    public sealed class GZipStyleBundle:GZipBundle {
        public GZipStyleBundle(string virtualPath,params IBundleTransform[] transforms) : base(virtualPath,transforms) { }
    }

    // Represents a bundle that does JS minification and GZip compression.
    public sealed class GZipScriptBundle:GZipBundle {
        public GZipScriptBundle(string virtualPath,params IBundleTransform[] transforms)
            : base(virtualPath,transforms) {
            base.ConcatenationToken = ";" + Environment.NewLine;
        }
    }

    public class BundleConfig {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles) {

            bundles.UseCdn = true;   //enable CDN support

            bundles.Add(new GZipBundle("~/bundles/css")
                .Include(
            "~/css/plugins/bootstrap-modal/bootstrap-modal-min.css",
			"~/css/plugins/bootstrap-modal/bootstrap-modal-bs3patch.min.css",
			"~/css/plugins/bootstrap-modal/bootstrap-modal-custom-{version}.css",
			"~/css/plugins/jqueryui/jquery-ui-1.10.3.autocomplete-{version}.css",
            "~/css/plugins/select2/select2-{version}.css",
            "~/css/plugins/select2/select2-bootstrap-{version}.css",
            "~/css/plugins/bootstrap-datepicker/bootstrap-datepicker-{version}.min.css",
            "~/css/plugins/bs-daterangepicker/daterangepicker-bs3-{version}.css",
            "~/css/plugins/bootstrap-timepicker/bootstrap-timepicker-{version}.min.css",
            "~/css/plugins/dropzone/dropzone-{version}.min.css",
            "~/css/plugins/datatable/datatable-bs3-{version}.min.css",
            "~/css/plugins/bs-multiselect/bootstrap-multiselect-{version}.css",
            "~/css/plugins/airline-select/airline-select-{version}.css",
            "~/css/components-{version}.css",
            "~/css/site.min-{version}.css",
            "~/css/themes/sky-blue.min.css",
            "~/css/custom-{version}.css"
          ));

            bundles.Add(new GZipBundle("~/bundles/flot")
           .Include(
            "~/js/plugins/flot/jquery.flot.min.js",
              "~/js/plugins/flot/jquery.flot.pie.min.js",
              "~/js/plugins/flot/jquery.flot.navigate.min.js",
              "~/js/plugins/flot/jquery.flot.categories.min.js",
              "~/js/plugins/flot/jquery.flot.resize.min.js",
              "~/js/plugins/flot/excanvas.min.js"
          ));

            bundles.Add(new GZipBundle("~/bundles/plugins")
         .Include(
          "~/js/plugins/bs-notification/bs-notification-{version}.js",
          "~/js/plugins/jqueryui/jquery-ui-1.11.4.autocomplete.min.js",
          "~/js/plugins/jqueryui/jquery-ui-1.10.3.ajaxcombobox-{version}.js",
          "~/js/plugins/jqueryui/jquery-ui-1.10.3.autocomplete-ex-{version}.js",
          "~/js/plugins/draggable/jquery.tiny-draggable-{version}.js",
          "~/js/plugins/bootstrap-modal/bootstrap-modal.min.js",
          "~/js/plugins/bootstrap-modal/bootstrap-modalmanager.min.js",
          "~/js/plugins/bs-multiselect/bootstrap-multiselect.min-{version}.js",
          "~/js/plugins/bootbox/bootbox.min.js",
          "~/js/plugins/select2/select2.min-{version}.js",
          "~/js/plugins/blockUI/jquery.blockUI.min.js",
          "~/js/plugins/dropzone/dropzone.js",
          "~/js/plugins/bs-daterangepicker/moment.min-{version}.js",
          "~/js/plugins/bs-daterangepicker/daterangepicker-{version}.js",
          "~/js/plugins/bs-contextmenu/contextmenu-{version}.js",
          "~/js/plugins/slimscroll/jquery.slimscroll.min.js",
          "~/js/plugins/autofix/jquery.autofix_anything.min.js",
          "~/js/plugins/storage/jquery.cookie-{version}.js",
          "~/js/plugins/storage/jquery.storageapi-{version}.js",
          "~/js/plugins/jquery-floatthead/jquery.floatThead.min-{version}.js",
          "~/js/plugins/jquery-bstable/jquery.bs.table.min-{version}.js",
          "~/js/plugins/pagination/jquery.twbsPagination.min.js",
          "~/js/plugins/bootstrap-datepicker/bootstrap-datepicker-{version}.js",
          "~/js/plugins/bootstrap-timepicker/bootstrap-timepicker.min.js",
          "~/js/plugins/bootstrap-confirmation/bootstrap-confirmation.min.js",
          "~/js/plugins/accounting/accounting-{version}.js",
          "~/js/plugins/airlineselect/airline-select-{version}.js",
          "~/js/plugins/string-helper/string-helper-{version}.js",
          "~/js/helper/common-{version}.js"
       ));
            BundleTable.EnableOptimizations = true;
        }
    }
}
