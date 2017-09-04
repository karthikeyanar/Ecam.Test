// Settings object that controls default parameters for library methods:
accounting.settings = {
    currency: {
        symbol: "$", // default currency symbol is '$'
        format: "%s%v", // controls output: %s = symbol, %v = value/number (can be object: see below)
        decimal: ".", // decimal point separator
        thousand: ",", // thousands separator
        precision: 2 // decimal places
    },
    number: {
        precision: 2, // default precision on numbers is 0
        thousand: ",",
        decimal: "."
    }
}
// Format can be an object, with `pos`, `neg` and `zero`:
accounting.settings.currency.format = {
    pos: "%s%v", // for positive values, eg. "$1.00" (required)
    neg: "(%s%v)", // for negative values, eg. "$(1.00)" [optional]
    zero: "%s0" // for zero values, eg. "$  --" [optional]
};
// Format can be an object, with `pos`, `neg` and `zero`:
accounting.settings.number.format = {
    pos: "%v",
    neg: "(%v)",
    zero: "0"
};
var _baseJsUrl = '/js/controllers';
var _helperUrl = '/js/helper/helper';
if (_IsUseMinifier == "true") {
    _baseJsUrl = 'js/min/controllers'
    _helperUrl = '/js/min/helper/helper.min';
}
$(function () {

    // might as well use requirejs instead of continuing to put script tags everywherer
    require.config({
        baseUrl: _baseJsUrl,
        paths: {
            'app': '/js/app/app.js?v=' + _Version,
            'knockout': '/js/knockout/knockout-3.4.0.js?v=1',
            'komapping': '/js/knockout/knockout.mapping-latest.js?v=1',
            'ko-binding': '/js/knockout/ko-binding.js?v=1.0.5',
            'finch': '/js/finch/finch-0.5.13.min.js?v=1.0.1',
            'infuser': '/js/finch/infuser-amd.js?v=1.0.5',
            'trafficCop': '/js/finch/TrafficCop.min.js?v=1',
            'scroll-page': '/js/plugins/scroll-page/jquery-paged-scroll.min.js?v=1',
            'koext': '/js/finch/koExternalTemplateEngine-amd.js?v=1',
            'service': '/js/services/service.js?v=1.0.4',
            'ckeditor': '/js/plugins/ckeditor/ckeditor.js?v=1',
            'ckeditorjquery': '/js/plugins/ckeditor/adapters/jquery.js?v=1',
            'tokenfield': '/js/plugins/bootstrap-tokenfield/bootstrap-tokenfield.js?v=1.0.1',
            'bsclockpicker': '/js/plugins/bs-clockpicker/bootstrap-clockpicker.js?v=1',
            'multiselect2side': '/js/plugins/multiselect2side/jquery.multiselect2side.js?v=1',
            'fileinput': '/js/plugins/fileinput/fileinput.js?v=1',
            'jquery_ui_sortable': '/js/plugins/jqueryui/jquery.ui.sortable.js?v=1',
            'jquery_ui_widget': '/js/plugins/jqueryui/jquery.ui.widget.js?v=1',
            'jquery_ui_core': '/js/plugins/jqueryui/jquery.ui.core.js?v=1',
            'jquery_ui_custom': '/js/plugins/jqueryui/jquery.ui.custom.js?v=1',
            'jquery_ui_mouse': '/js/plugins/jqueryui/jquery.ui.mouse.js?v=1',
            'draggabilly': '/js/plugins/draggabilly/draggabilly.pkgd.min.js?v=1',
            'b3_editable': '/js/plugins/bootstrap3-editable/bootstrap-editable.min.js?v=1',
            'datatable.net': '/js/plugins/datatable/DataTables-1.10.12/js/jquery.dataTables.min.js?v=1',
            'datatable_fc': '/js/plugins/datatable/FixedColumns-3.2.2/js/dataTables.fixedColumns.min.js?v=1',
            'datatable_fh': '/js/plugins/datatable/FixedHeader-3.1.2/js/dataTables.fixedHeader.min.js?v=1',
            'bootstrap-tour': '/js/plugins/bootstrap-tour/js/bootstrap-tour.js?v=' + _Version,
            'koEditables': '/js/knockout/ko.editables.js?v=1.0.1',
            'sticky': '/js/plugins/sticky/jquery.sticky.js?v=1.0.1',
            'helper': '/js/helper/helper.js?v=' + _Version
        },
        shim: {
            'helper': {
                deps: []
            },
            'jquery_ui_sortable': {
                deps: ['helper']
            },
            'jquery_ui_widget': {
                deps: ['jquery_ui_sortable']
            },
            'jquery_ui_core': {
                deps: ['jquery_ui_widget']
            },
            'jquery_ui_custom': {
                deps: ['jquery_ui_core']
            },
            'jquery_ui_mouse': {
                deps: ['jquery_ui_custom']
            },
            'sticky': {
                deps: ['helper']
            },
            'tokenfield': {
                deps: ['helper']
            },
            'bsclockpicker': {
                deps: ['helper']
            },
            'multiselect2side': {
                deps: ['helper']
            },
            'ckeditor': {
                deps: ['helper']
            },
            'ckeditorjquery': {
                deps: ['ckeditor']
            },
            'service': {
                deps: ['helper']
            },
            'fileinput': {
                deps: ['helper']
            },
            'knockout': {
                deps: ['helper']
            },
            'komapping': {
                deps: ['knockout']
            },
            'ko-binding': {
                deps: ['knockout']
            },
            'ko-editables': {
                deps: ['knockout']
            },
            'koext': {
                deps: ['knockout']
            },
            'datatable_fc': {
                deps: ['datatable.net']
            },
            'datatable_fh': {
                deps: ['datatable.net']
            },
            'app': {
                deps: ['knockout', 'ko-binding']
            },
            'finch': {
                exports: 'Finch',
                deps: ['knockout']
            },
            'infuser': {
                deps: ['finch']
            }
        },
        urlArgs: "v=" + _Version
    });
    define('jquery', [], function () { return jQuery; });
    // hack in a 'ko' library since that's what ko-ext expects
    define('ko', ['knockout'], function (ko, komapping) { return ko; });


    // define route and outer ko viewModel
    require(['knockout', 'app', 'finch', 'infuser', 'koext', 'helper'], function (ko, AppViewModel, finch, infuser, koext, helper) {

        ko.options.deferUpdates = false;

        var app = new AppViewModel();
        // templates
        infuser.defaults.useLoadingTemplate = true;
        infuser.defaults.templateUrl = "";
        infuser.defaults.templateSuffix = "";
        infuser.defaults.loadingTemplate.content = "<div class='blockUI blockMsg blockPage'><div class='blockUI-message bg-primary'><span><i class='fa fa-circle-o-notch fa-spin'></i>&nbsp;&nbsp;Loading...</span></div></div>";

        // routes
        finch.route("/", function () {
            app.viewModel(null);
            finch.navigate("/blank");
        });

        var createFinchRoute = function (url, options) {
            finch.route(url, {
                setup: function () {
                    options.setup(url);
                },
                load: function (bindings) {
                    options.load(bindings);
                },
                unload: function () {
                    options.unload();
                }
            });
        }

        createFinchRoute("/blank/:rnd", {
            setup: function (url) {
                app.checkRole([], url);
                handleBlockUI();
            },
            load: function (bindings) {
                require(['BlankController'], function (ViewModel) {
                    document.title = "Home Page - Ecams";
                    var m = new ViewModel("/Home/Blank", bindings.rnd);
                    app.viewModel(m);
                    unblockUI();
                });
            },
            unload: function () { }
        });

        createFinchRoute("/company", {
            setup: function (url) {
                handleBlockUI();
            },
            load: function () {
                require(['CompanyController'], function (ViewModel) {
                    document.title = "Company - Ecams";
                    var m = new ViewModel();
                    app.viewModel(m);
                });
            },
            unload: function () { }
        });


        createFinchRoute("/intraday", {
            setup: function (url) {
                handleBlockUI();
            },
            load: function () {
                require(['IntradayController'], function (ViewModel) {
                    document.title = "Intraday - Ecams";
                    var m = new ViewModel();
                    app.viewModel(m);
                });
            },
            unload: function () { }
        });

        createFinchRoute("/monthlyavg", {
            setup: function (url) {
                handleBlockUI();
            },
            load: function () {
                require(['MonthlyAVGController'], function (ViewModel) {
                    document.title = "Monthly AVG - Ecams";
                    var m = new ViewModel();
                    app.viewModel(m);
                });
            },
            unload: function () { }
        });

        createFinchRoute("/order", {
            setup: function (url) {
                handleBlockUI();
            },
            load: function () {
                require(['OrderController'], function (ViewModel) {
                    document.title = "Order - Ecams";
                    var m = new ViewModel();
                    app.viewModel(m);
                });
            },
            unload: function () { }
        });

        createFinchRoute("/category", {
            setup: function (url) {
                handleBlockUI();
            },
            load: function () {
                require(['CategoryController'], function (ViewModel) {
                    document.title = "Category - Ecams";
                    var m = new ViewModel();
                    app.viewModel(m);
                });
            },
            unload: function () { }
        });

        createFinchRoute("/market", {
            setup: function (url) {
                handleBlockUI();
            },
            load: function () {
                require(['MarketController'], function (ViewModel) {
                    document.title = "Market - Ecams";
                    var m = new ViewModel();
                    app.viewModel(m);
                });
            },
            unload: function () { }
        });
         
        // start running
        ko.applyBindings(app);

        finch.listen();


        helper.appModel = app;
        helper.onAjaxError = function (jqXHR, exception) {
            if (jqXHR.status == 401) {
                app.logOut();
            }
        }

    });

});