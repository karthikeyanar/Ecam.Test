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
            var is_ema_check = $("#frmCompanySearch #is_ema_check")[0].checked;
            if (is_ema_check == true) {
                arr[arr.length] = { "name": "is_ema_check", "value": is_ema_check };
            }
            var is_ema_positive_check = $("#frmCompanySearch #is_ema_positive_check")[0].checked;
            if (is_ema_positive_check == true) {
                arr[arr.length] = { "name": "is_ema_positive_check", "value": is_ema_positive_check };
            }
            var url = apiUrl("/Market/List");
            $.ajax({
                "url": url,
                "cache": false,
                "type": "GET",
                "data": arr
            }).done(function (json) {
                for (var i = 0; i < json.rows.length; i++) {
                    json.rows[i].id = i;
                }
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

        this.loadTradeDetail = function ($childTD, dataFor) {
            $childTD.addClass("loading");
            $childTD.empty();
            $("#detail-template").tmpl({}).appendTo($childTD);
            $childTD.css("padding-left", "25px").css("background-color", "#F2F2F2");
            $childTD.removeClass("loading");

            var symbol = dataFor.symbol;// $childTD.attr("symbol");
            var startDate = moment(cDateObj(dataFor.trade_date)).subtract('month', 12).format('MM/DD/YYYY');
            var endDate = moment(cDateObj(dataFor.trade_date)).add('month', 1).format('MM/DD/YYYY');
            var $rsiBox = $(".rsi-box", $childTD);
            var $avgMonthBox = $(".avg-month-box", $childTD);
            self.loadRSI(symbol, startDate, endDate, moment(cDateObj(dataFor.trade_date)).format('MM/DD/YYYY'), $rsiBox, function () {
                //self.loadAVG(symbol, $avgMonthBox, function () {
                //});
            });
        }

        this.loadRSI = function (symbol, startDate, endDate, tradeDate, $box, callback) {
            $box.html('loading...');
            handleBlockUI();
            var url = apiUrl("/Market/List");
            var arr = [];
            arr[arr.length] = { "name": "symbols", "value": symbol };
            arr[arr.length] = { "name": "start_date", "value": startDate };
            arr[arr.length] = { "name": "end_date", "value": endDate };
            arr[arr.length] = { "name": "SortName", "value": "ct.trade_date" };
            arr[arr.length] = { "name": "SortOrder", "value": "desc" };
            arr[arr.length] = { "name": "PageSize", "value": "0" };
            $.ajax({
                "url": url,
                "cache": false,
                "type": "GET",
                "data": arr
            }).done(function (json) {
                for (var i = 0; i < json.rows.length; i++) {
                    json.rows[i].is_trade_date = 'false';
                    if (formatDate(json.rows[i].trade_date, 'MM/DD/YYYY') == tradeDate) {
                        json.rows[i].is_trade_date = 'true';
                    }
                }
                $box.empty();
                $("#detail-rsi-template").tmpl(json).appendTo($box);
                $box.removeClass("loading");
                if (callback)
                    callback();
            })
            .always(function () {
                unblockUI();
            });
        }

        this.loadChart = function (dataFor, callback) {
            var symbol = dataFor.symbol;// $childTD.attr("symbol");
            var startDate = moment(cDateObj(dataFor.trade_date)).subtract('month', 6).format('MM/DD/YYYY');
            var endDate = moment(cDateObj(dataFor.trade_date)).add('month', 6).format('MM/DD/YYYY');
            var tradeDate = moment(cDateObj(dataFor.trade_date)).format('MM/DD/YYYY');
            $('#temp-chart-modal-container').remove();
            var $cnt = $("<div id='temp-chart-modal-container'></div>");
            $('body').append($cnt);

            handleBlockUI();
            var url = apiUrl("/Market/List");
            var arr = [];
            arr[arr.length] = { "name": "symbols", "value": symbol };
            arr[arr.length] = { "name": "start_date", "value": startDate };
            arr[arr.length] = { "name": "end_date", "value": endDate };
            arr[arr.length] = { "name": "SortName", "value": "ct.trade_date" };
            arr[arr.length] = { "name": "SortOrder", "value": "asc" };
            arr[arr.length] = { "name": "PageSize", "value": "0" };
            $.ajax({
                "url": url,
                "cache": false,
                "type": "GET",
                "data": arr
            }).done(function (json) {
                var arrDates = [];
                var arrClosePrice = [];
                var arrEMA20 = [];
                var arrEMA5 = [];
                var arrEMACross = [];
                var arrEMAIncrease = [];
                for (var i = 0; i < json.rows.length; i++) {
                    json.rows[i].is_trade_date = 'false';
                    if (formatDate(json.rows[i].trade_date, 'MM/DD/YYYY') == tradeDate) {
                        json.rows[i].is_trade_date = 'true';
                    }
                    arrDates.push(formatDate(json.rows[i].trade_date, 'MM/DD/YYYY'));
                    arrClosePrice.push(cFloat(json.rows[i].close_price));
                    arrEMA20.push(cFloat(json.rows[i].ema_200));
                    arrEMA5.push(cFloat(json.rows[i].ema_50));
                    arrEMACross.push(cFloat(json.rows[i].ema_cross));
                    arrEMAIncrease.push(cFloat(json.rows[i].ema_increase_profit));
                }

                var data = {
                    "name": "chart"
                   , "title": dataFor.company_name
                   , "is_modal_full": false
                   , "position": "top"
                   , "width": $(window).width() - 50
                };
                //////console.log(data);
                $("#modal-chart-template").tmpl(data).appendTo($cnt);
                var $modal = $("#modal-investment-" + data.name, $cnt);
                $modal.modal('show');

                Highcharts.chart('trade_chart', {
                    title: {
                        text: dataFor.company_name
                    },
                    xAxis: {
                        categories: arrDates
                    },
                    labels: {
                        items: [{
                            html: symbol,
                            style: {
                                left: '50px',
                                top: '18px',
                                color: (Highcharts.theme && Highcharts.theme.textColor) || 'black'
                            }
                        }]
                    },
                    series: [{
                        type: 'spline',
                        name: 'Close Price',
                        data: arrClosePrice,
                        color: 'blue',
                        marker: {
                            enabled: false
                        }
                    }, {
                        type: 'spline',
                        name: 'EMA 20',
                        data: arrEMA20,
                        color: 'red',
                        marker: {
                            enabled: false
                        }
                    }, {
                        type: 'spline',
                        name: 'EMA 5',
                        data: arrEMA5,
                        color: 'green',
                        marker: {
                            enabled: false
                        }
                    }]
                });

                Highcharts.chart('trade_column_chart', {
                    title: {
                        text: dataFor.company_name
                    },
                    xAxis: {
                        categories: arrDates
                    },
                    labels: {
                        items: [{
                            html: symbol,
                            style: {
                                left: '50px',
                                top: '18px',
                                color: (Highcharts.theme && Highcharts.theme.textColor) || 'black'
                            }
                        }]
                    },
                    series: [{
                        type: 'column',
                        name: 'EMA Cross',
                        data: arrEMACross,
                        color: 'lightgray'
                    }]
                });

                if (callback)
                    callback();
            })
            .always(function () {
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
                var dt = $(":input[name='last_trade_date']").val();
                if (cString(dt) != '') {
                    handleBlockUI();
                    var categories = $(":input[name='categories']").val();
                    var url = apiUrl("/Company/GetNSEUpdate?categories=" + categories + "&last_trade_date=" + dt
                        + "&is_book_mark=" + $("#is_book_mark")[0].checked
                        + "&is_book_mark_category=" + $("#is_book_mark_category")[0].checked);
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
                }
            } else {
                console.log(2);
                self.startNSE();
            }
        }

        this.startNSE = function () {
            var symbols = '';
            for (var i = 0; i < self.company_json.length; i++) {
                symbols += self.company_json[i].symbol + '|' + self.company_json[i].start_date + '|' + self.company_json[i].end_date + '|' + cString(self.company_json[i].nse_type) + ',';
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
            $("body").on("click", "#frmCompanySearch #is_ema_check", function (event) {
                self.loadGrid();
            });
            $("body").on("click", "#frmCompanySearch #is_ema_positive_check", function (event) {
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
            $("body").on("change", "#ema_signal", function (event) {
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
            $("body").on("click", "#CompanyTable > tbody > tr > td.btn-open-chart", function (event) {
                var $this = $(this);
                var dataFor = ko.dataFor($this[0]);
                self.loadChart(dataFor, function () {
                });
            });
            $("body").on("click", "#CompanyTable > tbody > tr > td.cls-symbol", function (event) {
                var $this = $(this);
                var dataFor = ko.dataFor($this[0]);
                var fbJson = JSON.parse(ko.toJSON(dataFor));
                var $tr = $this.parents("tr:first");
                var $tbl = $this.parents("table:first");
                var $tbody = $("tbody", $tbl);
                var companyId = cInt(dataFor.id);
                var childTRId = "child_" + companyId;
                var $childTR = $("#" + childTRId, $tbody);
                var $treeExpand = $(".tree-expand", this);
                $("#cargoTable .child-rows[cid!='" + companyId + "']").addClass("hide");
                $("#cargoTable .tree-expand[cid!='" + companyId + "']").removeClass("ex-minus").addClass("ex-plus");
                if ($treeExpand.hasClass("ex-plus")) {
                    $treeExpand.removeClass("ex-minus").removeClass("ex-plus");
                    if (!$childTR[0]) {
                        $treeExpand.addClass("ex-minus");
                        $childTR = $("<tr class='child-rows' cid='" + companyId + "'><td symbol='" + dataFor.symbol + "' company_id='" + companyId + "' colspan='" + $("thead > tr > th", $tbl).length + "' class='child-row-cnt'></td></tr>");
                        $childTR.attr("id", childTRId).attr("row-key", companyId);
                        var $childTD = $("td", $childTR);
                        $tr.after($childTR);
                        $childTD.data("fbjson", fbJson);
                        self.loadTradeDetail($childTD, dataFor);
                    } else {
                        if ($childTR.hasClass('hide')) {
                            $childTR.removeClass("hide");
                            $treeExpand.addClass("ex-minus");
                        } else {
                            $childTR.addClass("hide");
                            $treeExpand.addClass("ex-plus");
                        }
                    }
                } else {
                    $childTR.addClass("hide");
                    $treeExpand.removeClass("ex-minus").addClass("ex-plus");
                }
            });
        }

        this.offElements = function () {
            $("body").off("click", "#frmCompanySearch #is_archive");
            $("body").off("click", "#frmCompanySearch #is_book_mark");
            $("body").off("click", "#frmCompanySearch #is_book_mark_category");
            $("body").off("click", "#frmCompanySearch #is_ema_check");
            $("body").off("click", "#frmCompanySearch #is_ema_positive_check");
            $("body").off("change", "#Company #rows");
            $("body").off("click", ".is-book-mark");
            $("body").off("click", ".is-current-stock");
            $("body").off("change", "#super_trend_signal");
            $("body").off("change", "#ema_signal");
            $("body").off("click", "#btnSTUpdate");
            $("body").off("click", "#btnNSEDownload");
            $("body").off("click", "#btnNSEUpdateCSV");
            $("body").off("click", "#CompanyTable > tbody > tr > td.btn-open-chart");
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

            var $symbols = $(":input[name='symbols']");
            $symbols.select2Refresh("data", [{ id: 'NIFTY500', text: 'NIFTY500' }]);

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
