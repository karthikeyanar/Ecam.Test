"use strict";
define("IndicatorController", ["knockout", "komapping", "helper", "service"], function (ko, komapping, helper, service) {
    return function () {
        var self = this;
        this.template = "/Home/Indicator";

        this.index = -1;
        this.company_json = null;
        this.rows = ko.observableArray([]);
        this.start_date = ko.observable("");
        this.end_date = ko.observable("");

        this.refresh = function () {
            self.loadGrid();
        }

        this.last_grid_data = null;

        this.loadGrid = function (callback) {
            handleBlockUI();
            var $Company = $("#Company");
            var arr = $("#frmCompanySearch").serializeArray();
            var sortName = $(":input[name='sort_name']", $Company).val();
            var sortOrder = $(":input[name='sort_order']", $Company).val();
            arr.push({ "name": "PageSize", "value": $(":input[name='rows']", $Company).val() });
            arr.push({ "name": "PageIndex", "value": $(":input[name='page_index']", $Company).val() });
            arr.push({ "name": "SortName", "value": sortName });
            arr.push({ "name": "SortOrder", "value": sortOrder });
            var isarchive = $("#frmCompanySearch #is_archive")[0].checked;
            if (isarchive == true) {
                arr[arr.length] = { "name": "is_archive", "value": isarchive };
            }
            var is_book_mark = $("#frmCompanySearch #is_book_mark")[0].checked;
            if (is_book_mark == true) {
                arr[arr.length] = { "name": "is_book_mark", "value": is_book_mark };
            }
            var is_book_mark_category = $("#frmCompanySearch #is_book_mark_category")[0].checked;
            if (is_book_mark_category == true) {
                arr[arr.length] = { "name": "is_book_mark_category", "value": is_book_mark_category };
            }
            var is_macd_check = $("#frmCompanySearch #is_macd_check")[0].checked;
            if (is_macd_check == true) {
                arr[arr.length] = { "name": "is_macd_check", "value": is_macd_check };
            }
            var url = apiUrl("/Market/List");
            $.ajax({
                "url": url,
                "cache": false,
                "type": "GET",
                "data": arr
            }).done(function (json) {
                self.rows.removeAll();
                self.rows(json.rows);
                self.last_grid_data = json.rows;
                $(".manual-pagination", $Company).each(function () {
                    var element = this;
                    $(element).twbsPagination({
                        total: cInt(json.total),
                        rowsPerPage: cInt($(":input[name='rows']", $Company).val()),
                        startPage: cInt($(":input[name='page_index']", $Company).val()),
                        currentPage: cInt($(":input[name='page_index']", $Company).val()),
                        onRender: function (status) {
                            $("#paging_status", $Company).html(status);
                        },
                        onPageClick: function (page) {
                            $(":input[name='page_index']", $Company).val(page);
                            self.loadGrid();
                        }
                    });
                });
                if (callback)
                    callback();
            }).always(function () {
                unblockUI();
            });
        }

        this.applyPlugins = function () {
            var $frmCompanySearch = $("#frmCompanySearch");
            var $symbols = $(":input[name='symbols']", $frmCompanySearch);
            var $categories = $(":input[name='categories']", $frmCompanySearch);
            select2Setup($symbols[0], {
                multiple: true
                , width: 300
               , url: apiUrl("/Company/Select")
               , placeholder: "Select Company"
               , resultsCallBack: function (data, page) {
                   var s2data = [];
                   $.each(data, function (i, d) {
                       s2data.push({ "id": d.id, "text": d.label, "source": d });
                   });
                   return { results: s2data };
               }
               , onParam: function (term, page) {
                   return {
                       term: term,
                       categories: $categories.val()
                   };
               }
               , onChange: function (e, ui) {
                   var $target = $(".page-content");
                   self.loadGrid();
               }
            });

            select2Setup($categories[0], {
                multiple: true
               , width: 300
               , url: apiUrl("/Company/SelectCategories")
               , placeholder: "Select Categories"
               , resultsCallBack: function (data, page) {
                   var s2data = [];
                   $.each(data, function (i, d) {
                       s2data.push({ "id": d.id, "text": d.label, "source": d });
                   });
                   return { results: s2data };
               }
               , onParam: function (term, page) {
                   return {
                       term: term
                   };
               }
               , onChange: function (e, ui) {
                   var $target = $(".page-content");
                   self.loadGrid();
               }
            });

            var $pageContent = $(".page-content");
            var start = moment(_TODAYDATE).subtract('days', 30);// moment(arrlastsixmonths[0]);
            var end = moment(_TODAYDATE); //moment(_TODAYDATE);//moment(arrlastsixmonths[1]);
            self.start_date(start.format('MM/DD/YYYY'));
            self.end_date(end.format('MM/DD/YYYY'));
            ////////console.log('$pageContent=',$pageContent[0]);
            var $reportRange = $('#reportrange', $pageContent);
            ////////console.log('reportrange=',$reportRange[0]);
            helper.handleDateRangePicker($reportRange, {
                'opens': 'left',
                'start': start,
                'end': end,
                'changeDate': function (start, end) {
                    var daysDiff = helper.getTimeDiff(start.format('MM/DD/YYYY'), end.format('MM/DD/YYYY')).days;
                    self.start_date(start.format('MM/DD/YYYY'));
                    self.end_date(end.format('MM/DD/YYYY'));
                    helper.changeDateRangeLabel($('span', $reportRange), start, end, self.start_date(), self.end_date());
                    self.loadGrid();
                }
            });
            helper.changeDateRangeLabel($('span', $reportRange), start, end, self.start_date(), self.end_date());
        }

        this.nseDownload = function () {
            if (self.company_json == null) {
                handleBlockUI();
                var dt = '27-Jul-2018';
                var url = apiUrl("/Company/GetNSEUpdate?last_trade_date=" + dt + "&is_book_mark_category=" + $("#is_book_mark_category")[0].checked);
                var arr = [];
                $.ajax({
                    "url": url,
                    "cache": false,
                    "type": "GET",
                    "data": arr
                }).done(function (json) {
                    self.index = -1;
                    self.company_json = json;
                    self.startNSE();
                }).always(function () {
                    unblockUI();
                });
            } else {
                console.log(2);
                self.startNSE();
            }
        }

        this.startNSE = function () {
            var symbols = '';
            for (var i = 0; i < self.company_json.length; i++) {
                symbols += self.company_json[i].symbol + '|' + self.company_json[i].start_date + '|' + self.company_json[i].end_date + ',';
            }
            if (symbols != '') {
                symbols = symbols.substring(0, symbols.length - 1);
            }
            $('#gcb_cmd').attr('cmd', 'open_nse');
            $('#gcb_cmd').attr('symbol', symbols);
            $('#gcb_cmd').click();
        }

        this.onElements = function () {
            self.offElements();
            $("body").on("click", "#frmCompanySearch #is_archive", function (event) {
                self.loadGrid();
            });
            $("body").on("click", "#frmCompanySearch #is_book_mark", function (event) {
                self.loadGrid();
            });
            $("body").on("click", "#frmCompanySearch #is_book_mark_category", function (event) {
                self.loadGrid();
            });
            $("body").on("click", "#frmCompanySearch #is_macd_check", function (event) {
                self.loadGrid();
            });
            $("body").on("change", "#Company #rows", function (event) {
                self.loadGrid();
            });
            $("body").on("click", ".is-book-mark", function (event) {
                var $this = $(this);
                var $i = $("i", $this);
                var dataFor = ko.dataFor(this);
                var url = apiUrl('/Company/UpdateArchive');
                var arr = [];
                var isArchive = $i.hasClass('fa-bookmark');
                arr.push({ "name": "symbol", "value": dataFor.symbol });
                arr.push({ "name": "is_archive", "value": !isArchive });
                $i.removeClass('fa-bookmark').removeClass('fa-bookmark-o').removeClass('fg-primary');
                if (isArchive == true) {
                    $i.addClass('fa-bookmark-o');
                } else {
                    $i.addClass('fa-bookmark fg-primary');
                }
                $.ajax({
                    "url": url,
                    "cache": false,
                    "type": "POST",
                    "data": arr
                }).done(function (json) {
                });
            });
            $("body").on("click", ".is-current-stock", function (event) {
                var $this = $(this);
                var $i = $("i", $this);
                var dataFor = ko.dataFor(this);
                var url = apiUrl('/Company/UpdateBookMark');
                var arr = [];
                var is_book_mark = $i.hasClass('fa-user');
                arr.push({ "name": "symbol", "value": dataFor.symbol });
                arr.push({ "name": "is_book_mark", "value": !is_book_mark });
                $i.removeClass('fa-user').removeClass('fa-user-o').removeClass('fg-primary');
                if (is_book_mark == true) {
                    $i.addClass('fa-user-o');
                } else {
                    $i.addClass('fa-user fg-primary');
                }
                $.ajax({
                    "url": url,
                    "cache": false,
                    "type": "POST",
                    "data": arr
                }).done(function (json) {
                });
            });
            $("body").on("click", "#chkSelectAll", function (event) {
                var that = this;
                $(".chk-symbol").each(function () {
                    this.checked = that.checked;
                });
            });
            $("body").on("change", "#super_trend_signal", function (event) {
                self.loadGrid();
            });
            $("body").on("click", "#btnSTUpdate", function (event) {
                handleBlockUI({ "target": $("body"), "message": "Update..." });
                var that = this;
                var dataFor = ko.dataFor(that);
                var arr = [];
                arr.push({ "name": "symbol", "value": dataFor.symbol });
                var url = apiUrl("/Market/UpdateSuperTrend");
                $.ajax({
                    "url": url,
                    "cache": false,
                    "type": "POST",
                    "data": arr
                }).done(function (json) {
                    unblockUI();
                    self.loadGrid();
                });
            });
            $("body").on("click", "#btnNSEDownload", function (event) {
                self.nseDownload();
            });
            $("body").on("click", "#btnNSEUpdateCSV", function (event) {
                var $nse_csv = $("#nse_csv");
                //console.log('nse_csv=', $nse_csv.val());
                var url = apiUrl("/Market/UpdateCSV");
                var arr = [];
                arr.push({ 'name': 'csv', 'value': $nse_csv.val() });
                handleBlockUI({ "target": $("body"), "message": "Update..." });
                $.ajax({
                    "url": url,
                    "cache": false,
                    "type": "POST",
                    "data": arr
                }).done(function (json) {
                    var html = $("#nse_index").val() + ' of ' + $("#nse_total").val() + " - " + json.symbol;
                    $("#nse_csv_log").html(html);
                    unblockUI();
                }).always(function () {
                    unblockUI();
                });
            });
        }

        this.offElements = function () {
            $("body").off("click", "#frmCompanySearch #is_archive");
            $("body").off("click", "#frmCompanySearch #is_book_mark");
            $("body").off("click", "#frmCompanySearch #is_book_mark_category");
            $("body").off("click", "#frmCompanySearch #is_macd_check");
            $("body").off("change", "#Company #rows");
            $("body").off("click", ".is-book-mark");
            $("body").off("click", ".is-current-stock");
            $("body").off("change", "#super_trend_signal");
            $("body").off("click", "#btnSTUpdate");
            $("body").off("click", "#btnNSEDownload");
            $("body").off("click", "#btnNSEUpdateCSV");
        }

        this.unInit = function () {
            self.offElements();
        }

        this.init = function (callback) {
            unblockUI();
            if (callback)
                callback();

            self.applyPlugins();
            pushGCEvent(function (e, ui) {
                self.loadGrid();
            });
            self.loadGrid(function () {
                self.onElements();

                var $Company = $("#Company");
                var $tbl = $("#CompanyTable", $Company);
                var $sort_name = $(":input[name='sort_name']", $Company);
                var $sort_order = $(":input[name='sort_order']", $Company);
                $tbl.attr("sortname", $sort_name.val());
                $tbl.attr("sortorder", $sort_order.val());
                helper.sortingTable($tbl[0], {
                    "onSorting": function (sortname, sortorder) {
                        $(":input[name='sort_name']", $Company).val(sortname);
                        $(":input[name='sort_order']", $Company).val(sortorder);
                        self.loadGrid();
                    }
                });
                $tbl.floatThead({
                    scrollContainer: function ($t) {
                        return $t.closest('.summary-table-cnt');
                    }
                });
            });
        }
    }
}
);
