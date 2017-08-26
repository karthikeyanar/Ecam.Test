"use strict";
define("IntradayController", ["knockout", "komapping", "helper", "service"], function (ko, komapping, helper, service) {
    return function () {
        var self = this;
        this.template = "/Home/Intraday";

        this.rows = ko.observableArray([]);

        this.start_date = ko.observable("");
        this.end_date = ko.observable("");

        this.refresh = function () {
            self.loadGrid();
        }

        this.loadGrid = function (callback) {
            handleBlockUI();
            var $Company = $("#Company");
            var arr = $("#frmCompanySearch").serializeArray();
            arr.push({ "name": "PageSize", "value": $(":input[name='rows']", $Company).val() });
            arr.push({ "name": "PageIndex", "value": $(":input[name='page_index']", $Company).val() });
            arr.push({ "name": "SortName", "value": $(":input[name='sort_name']", $Company).val() });
            arr.push({ "name": "SortOrder", "value": $(":input[name='sort_order']", $Company).val() });
            var isbookmark = $("#frmCompanySearch #is_book_mark")[0].checked;
            if (isbookmark == true) {
                arr[arr.length] = { "name": "is_book_mark", "value": isbookmark };
            }
            var is_current_stock = $("#frmCompanySearch #is_current_stock")[0].checked;
            if (is_current_stock == true) {
                arr[arr.length] = { "name": "is_current_stock", "value": is_current_stock };
            }
            var isNifty50 = $("#frmCompanySearch #is_nifty_50")[0].checked;
            if (isNifty50 == true) {
                arr[arr.length] = { "name": "is_nifty_50", "value": isNifty50 };
            }
            var isNifty100 = $("#frmCompanySearch #is_nifty_100")[0].checked;
            if (isNifty100 == true) {
                arr[arr.length] = { "name": "is_nifty_100", "value": isNifty100 };
            }
            var isNifty200 = $("#frmCompanySearch #is_nifty_200")[0].checked;
            if (isNifty200 == true) {
                arr[arr.length] = { "name": "is_nifty_200", "value": isNifty200 };
            }

            var is_sell_to_buy = $("#frmCompanySearch #is_sell_to_buy")[0].checked;
            if (is_sell_to_buy == true) {
                arr[arr.length] = { "name": "is_sell_to_buy", "value": is_sell_to_buy };
            }

            var is_buy_to_sell = $("#frmCompanySearch #is_buy_to_sell")[0].checked;
            if (is_buy_to_sell == true) {
                arr[arr.length] = { "name": "is_buy_to_sell", "value": is_buy_to_sell };
            }

            var is_all_category = $("#frmCompanySearch #is_all_category")[0].checked;
            if (is_all_category == true) {
                arr[arr.length] = { "name": "is_all_category", "value": is_all_category };
            }

            var is_mf = $("#frmCompanySearch #is_mf")[0].checked;
            if (is_mf == true) {
                arr[arr.length] = { "name": "is_mf", "value": is_mf };
            }
            var url = apiUrl("/Company/List");
            $.ajax({
                "url": url,
                "cache": false,
                "type": "GET",
                "data": arr
            }).done(function (json) {
                self.rows.removeAll();
                if (json.rows != null) {
                    $.each(json.rows, function (i, row) {
                        row.quantity = 0;
                        row.investment_amount = 0;
                        row.target_price = 0;
                        row.stop_loss_price = 0;
                        row.bts_sell_target = 0;
                        row.bts_stop_loss_target = 0;
                        row.bts_target_profit = 0;
                        row.bts_stop_loss_profit = 0;
                        row.stb_sell_target = 0;
                        row.stb_stop_loss_target = 0;
                        row.stb_target_profit = 0;
                        row.stb_stop_loss_profit = 0;
                        var m = komapping.fromJS(row);
                        self.rows.push(m);
                    });
                }
                self.calculateJSON();
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


        this.calculateJSON = function () {
            var $frmSearch = $("#frmCompanySearch");
            var availableAmount = cFloat($("#available_amount", $frmSearch).val());
            var profitPercentage = cFloat($("#profit_percentage", $frmSearch).val());
            var stoplossPercentage = cFloat($("#stoploss_percentage", $frmSearch).val());
            if (self.rows() != null) {
                $.each(self.rows(), function (i, row) {
                    var ltpPrice = cFloat(row.ltp_price());

                    row.quantity(cInt(availableAmount / ltpPrice));
                    row.investment_amount(cFloat(row.quantity()) * ltpPrice);

                    row.target_price(cFloat((ltpPrice * profitPercentage) / 100));
                    row.stop_loss_price(cFloat((ltpPrice * stoplossPercentage) / 100));

                    row.bts_sell_target(ltpPrice + row.target_price());
                    row.bts_stop_loss_target(ltpPrice - row.stop_loss_price());

                    row.bts_target_profit((row.bts_sell_target() * row.quantity()) - row.investment_amount());
                    row.bts_stop_loss_profit((row.bts_stop_loss_target() * row.quantity()) - row.investment_amount());

                    row.stb_sell_target(ltpPrice - row.target_price());
                    row.stb_stop_loss_target(ltpPrice + row.stop_loss_price());

                    row.stb_target_profit(((row.stb_sell_target() * row.quantity()) - row.investment_amount()) * -1);
                    row.stb_stop_loss_profit(((row.stb_stop_loss_target() * row.quantity()) - row.investment_amount()) * -1);
                });
            }
        }

        this.exportGrid = function () {
            var $Company = $("#Company");
            var arr = $("#frmCompanySearch").serializeArray();
            arr.push({ "name": "PageSize", "value": 0 });
            arr.push({ "name": "PageIndex", "value": 0 });
            arr.push({ "name": "SortName", "value": $(":input[name='sort_name']", $Company).val() });
            arr.push({ "name": "SortOrder", "value": $(":input[name='sort_order']", $Company).val() });
            var isbookmark = $("#frmCompanySearch #is_book_mark")[0].checked;
            if (isbookmark == true) {
                arr[arr.length] = { "name": "is_book_mark", "value": isbookmark };
            }
            var is_current_stock = $("#frmCompanySearch #is_current_stock")[0].checked;
            if (is_current_stock == true) {
                arr[arr.length] = { "name": "is_current_stock", "value": is_current_stock };
            }
            arr[arr.length] = { "name": "is_export_excel", "value": true };
            var url = apiUrl("/Company/Export?t=1");
            $.each(arr, function (i, p) {
                url += "&" + p.name + "=" + p.value;
            });
            var width = 300; var height = 200; var left = (screen.availWidth / 2) - (width / 2); var top = (screen.availHeight / 2) - (height / 2); var features = "width=" + width + ",height=" + height + ",left=" + left + ",top=" + top + ",location=no,menubar=no,toobar=no,scrollbars=yes,resizable=yes,status=yes";
            window.open(url, "downloadexcelfile", features);
        }

        this.applyPlugins = function () {
            var $frmCompanySearch = $("#frmCompanySearch");
            var $symbols = $(":input[name='symbols']", $frmCompanySearch);
            var $categories = $(":input[name='categories']", $frmCompanySearch);
            var $mf_ids = $(":input[name='mf_ids']", $frmCompanySearch);
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

            select2Setup($mf_ids[0], {
                multiple: true
               , width: 300
               , url: apiUrl("/Company/SelectMFS")
               , placeholder: "Select MFS"
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


            var arrlastsixmonths = helper.getLastSixMonths();
            var start = moment(arrlastsixmonths[0]);
            var end = moment(arrlastsixmonths[1]);
            self.start_date(start.format('MM/DD/YYYY'));
            self.end_date(end.format('MM/DD/YYYY'));

            var $pageContent = $(".page-content");
            //console.log('$pageContent=',$pageContent[0]);
            var $reportRange = $('#reportrange', $pageContent);
            //console.log('reportrange=',$reportRange[0]);
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

        this.openItem = function (row) {
            $('#temp-modal-container').remove();
            var $cnt = $("<div id='temp-modal-container'></div>");
            $('body').append($cnt);
            var data = {
                "name": "Edit"
                , "title": (row == null ? "Add" : "Edit")
                , "is_modal_full": false
                , "position": "top"
            };
            $("#modal-template").tmpl(data).appendTo($cnt);
            var $modal = $("#modal-" + data.name, $cnt);
            $modal.modal('show');
            var isNew = false;
            if (row == null) {
                row = {
                    'company_name': ''
                    , 'symbol': ''
                    , 'category_name': ''
                    , 'id': 0
                    , 'category_list': []
                };
                isNew = true;
            }
            ko.applyBindings(row, $modal[0]);

            var $frm = $("form", $modal);
            var $categories = $(":input[name='category_name']", $frm);
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
              }
            });
            if (isNew == false) {
                var arr = [];
                console.log(row.category_list);
                $.each(row.category_list(), function (i, cat) {
                    arr.push({ "id": cat, "text": cat });
                });
                $categories.select2Refresh("data", arr);
            }
            var $btn = $("#save", $frm);
            $btn.click(function () {
                if ($frm.valid()) {
                    $btn.button('loading');
                    var data = $frm.serializeArray();
                    data.push({ "name": "company_id", "value": getGlobalCompanyID() });
                    var url = apiUrl("/Company/Create");
                    var type = "POST";
                    handleBlockUI({ "target": $("body"), "message": "Save..." });
                    $.ajax({
                        "url": url,
                        "cache": false,
                        "type": type,
                        "data": data
                    }).done(function (json) {
                        //jAlert("Saved");
                        $modal.modal('hide');
                        self.loadGrid();
                    }).fail(function (jqxhr) {
                        alertErrorMessage(jqxhr.responseJSON);
                    }).always(function (jqxhr) {
                        $btn.button('reset');
                        unblockUI();
                    });
                }
            });
        }

        this.loadTradeDetailChart = function ($childTD) {
            $childTD.addClass("loading");
            $childTD.empty();
            $("#detail-template").tmpl({}).appendTo($childTD);
            $childTD.css("padding-left", "25px").css("background-color", "#F2F2F2");
            $childTD.removeClass("loading");

            var symbol = $childTD.attr("symbol");
            var $rsiBox = $(".box-cnt", $childTD);
            self.loadRSIChart(symbol, $rsiBox, function (json) {
                google.charts.load('current', { 'packages': ['corechart'] });
                google.charts.setOnLoadCallback(function () {
                    var arr = [['Date', 'Trades']];
                    $.each(json.rows, function (i, row) {
                        arr.push([row.trade_date, row.close_price]);
                    });
                    var data = google.visualization.arrayToDataTable(arr);

                    var options = {
                        title: 'Company Performance',
                        curveType: 'function',
                        legend: { position: 'bottom' }
                    };
                    //console.log($('#curve_chart', $rsiBox)[0]);
                    var chart = new google.visualization.LineChart($('#curve_chart', $rsiBox)[0]);
                    chart.draw(data, options);
                });
            });
        }

        this.loadTradeDetail = function ($childTD) {
            $childTD.addClass("loading");
            $childTD.empty();
            $("#detail-template").tmpl({}).appendTo($childTD);
            $childTD.css("padding-left", "25px").css("background-color", "#F2F2F2");
            $childTD.removeClass("loading");

            var symbol = $childTD.attr("symbol");
            var $rsiBox = $(".rsi-box", $childTD);
            var $avgMonthBox = $(".avg-month-box", $childTD);
            self.loadRSI(symbol, $rsiBox, function () {
                self.loadAVG(symbol, $avgMonthBox, function () {
                });
            });
        }

        this.loadRSIChart = function (symbol, $box, callback) {
            $box.html('loading...');
            handleBlockUI();
            var url = apiUrl("/Company/RSIList");
            var arr = [];
            arr[arr.length] = { "name": "symbols", "value": symbol };
            arr[arr.length] = { "name": "SortName", "value": "m.trade_date" };
            arr[arr.length] = { "name": "SortOrder", "value": "asc" };
            arr[arr.length] = { "name": "PageSize", "value": "0" };
            $.ajax({
                "url": url,
                "cache": false,
                "type": "GET",
                "data": arr
            }).done(function (json) {
                $box.empty();
                $("#detail-rsi-chart-template").tmpl(json).appendTo($box);
                $box.removeClass("loading");
                if (callback)
                    callback(json);
            })
            .always(function () {
                unblockUI();
            });
        }

        this.loadRSI = function (symbol, $box, callback) {
            $box.html('loading...');
            handleBlockUI();
            var url = apiUrl("/Company/RSIList");
            var arr = [];
            arr[arr.length] = { "name": "symbols", "value": symbol };
            arr[arr.length] = { "name": "SortName", "value": "m.trade_date" };
            arr[arr.length] = { "name": "SortOrder", "value": "desc" };
            arr[arr.length] = { "name": "PageSize", "value": "0" };
            $.ajax({
                "url": url,
                "cache": false,
                "type": "GET",
                "data": arr
            }).done(function (json) {
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

        this.loadAVG = function (symbol, $box, callback) {
            $box.html('loading...');
            handleBlockUI();
            var url = apiUrl("/Company/AvgList");
            var arr = [];
            arr[arr.length] = { "name": "symbols", "value": symbol };
            arr[arr.length] = { "name": "SortName", "value": "intra.avg_date" };
            arr[arr.length] = { "name": "SortOrder", "value": "desc" };
            arr[arr.length] = { "name": "PageSize", "value": "0" };
            $.ajax({
                "url": url,
                "cache": false,
                "type": "GET",
                "data": arr
            }).done(function (json) {
                $box.empty();
                var months = [];
                var weeks = [];
                $.each(json.rows, function (i, row) {
                    if (row.avg_type == "M") {
                        months.push(row);
                    } else {
                        weeks.push(row);
                    }
                });
                var $divMonth = $("<div class='pull-left m-r-15'></div>");
                $box.append($divMonth);
                $("#detail-avg-month-template").tmpl({ "rows": months }).appendTo($divMonth);
                var $divWeek = $("<div class='pull-left m-l-15'></div>");
                $box.append($divWeek);
                $("#detail-avg-month-template").tmpl({ "rows": weeks }).appendTo($divWeek);
                $box.removeClass("loading");
                if (callback)
                    callback();
            })
            .always(function () {
                unblockUI();
            });
        }

        this.onElements = function () {
            self.offElements();
            $("body").on("click", "#frmCompanySearch #is_book_mark", function (event) {
                self.loadGrid();
            });
            $("body").on("click", "#frmCompanySearch #is_current_stock", function (event) {
                self.loadGrid();
            });
            $("body").on("click", "#frmCompanySearch #is_nifty_50", function (event) {
                self.loadGrid();
            });
            $("body").on("click", "#frmCompanySearch #is_nifty_100", function (event) {
                self.loadGrid();
            });
            $("body").on("click", "#frmCompanySearch #is_nifty_200", function (event) {
                self.loadGrid();
            });
            $("body").on("click", "#frmCompanySearch #is_all_time_low", function (event) {
                self.loadGrid();
            });
            $("body").on("click", "#frmCompanySearch #is_all_time_high", function (event) {
                self.loadGrid();
            });
            $("body").on("click", "#frmCompanySearch #is_all_time_low_15_days", function (event) {
                self.loadGrid();
            });
            $("body").on("click", "#frmCompanySearch #is_all_time_high_15_days", function (event) {
                self.loadGrid();
            });
            $("body").on("click", "#frmCompanySearch #is_all_time_low_5_days", function (event) {
                self.loadGrid();
            });
            $("body").on("click", "#frmCompanySearch #is_all_time_high_5_days", function (event) {
                self.loadGrid();
            });
            $("body").on("click", "#frmCompanySearch #is_all_time_low_2_days", function (event) {
                self.loadGrid();
            });
            $("body").on("click", "#frmCompanySearch #is_all_time_high_2_days", function (event) {
                self.loadGrid();
            });
            $("body").on("click", "#frmCompanySearch #is_low_yesterday", function (event) {
                self.loadGrid();
            });
            $("body").on("click", "#frmCompanySearch #is_high_yesterday", function (event) {
                self.loadGrid();
            });
            $("body").on("click", "#frmCompanySearch #is_sell_to_buy", function (event) {
                self.loadGrid();
            });
            $("body").on("click", "#frmCompanySearch #is_buy_to_sell", function (event) {
                self.loadGrid();
            });
            $("body").on("click", "#frmCompanySearch #is_all_category", function (event) {
                self.loadGrid();
            });
            $("body").on("click", "#frmCompanySearch #is_mf", function (event) {
                self.loadGrid();
            });
            $("body").on("click", "#Company .btn-add", function (event) {
                self.openItem(null)
            });
            $("body").on("click", "#Company .btn-export", function (event) {
                self.exportGrid()
            });
            $("body").on("click", "#Company .btn-edit", function (event) {
                self.openItem(ko.dataFor(this));
            });
            $("body").on("click", "#Company .btn-delete", function (event) {
                var dataFor = ko.dataFor(this);
                jConfirm({
                    "message": "Are you sure " + dataFor.company_name() + " : " + dataFor.symbol() + "?", "ok": function () {
                        var url = apiUrl("/Company/Delete/" + dataFor.id());
                        $.ajax({
                            "url": url,
                            "cache": false,
                            "type": "DELETE"
                        }).done(function () {
                            //jAlert("Deleted");
                            self.loadGrid();
                        }).fail(function (response) {
                            //self.errors(generateErrors(response.responseJSON));
                        }).always(function (jqxhr) {
                        });
                    }
                });
            });
            $("body").on("change", "#Company #rows", function (event) {
                self.loadGrid();
            });
            $("body").on("click", ".is-book-mark", function (event) {
                var $this = $(this);
                var $i = $("i", $this);
                var dataFor = ko.dataFor(this);
                var url = apiUrl('/Company/UpdateBookMark');
                var arr = [];
                var isBookMark = $i.hasClass('fa-bookmark');
                arr.push({ "name": "symbol", "value": dataFor.symbol() });
                arr.push({ "name": "is_book_mark", "value": !isBookMark });
                $i.removeClass('fa-bookmark').removeClass('fa-bookmark-o').removeClass('fg-primary');
                if (isBookMark == true) {
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
                var url = apiUrl('/Company/UpdateCurrentStock');
                var arr = [];
                var is_current_stock = $i.hasClass('fa-user');
                arr.push({ "name": "symbol", "value": dataFor.symbol() });
                arr.push({ "name": "is_current_stock", "value": !is_current_stock });
                $i.removeClass('fa-user').removeClass('fa-user-o').removeClass('fg-primary');
                if (is_current_stock == true) {
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
            $("body").on("keyup", "#available_amount", function (event) {
                self.calculateJSON();
            });
            $("body").on("keyup", "#profit_percentage", function (event) {
                self.calculateJSON();
            });
            $("body").on("keyup", "#stoploss_percentage", function (event) {
                self.calculateJSON();
            });
            $("body").on("click", "#CompanyTable > tbody > tr > td.cls-symbol", function (event) {
                var $this = $(this);
                var dataFor = ko.dataFor($this[0]);
                var fbJson = JSON.parse(ko.toJSON(dataFor));
                var $tr = $this.parents("tr:first");
                var $tbl = $this.parents("table:first");
                var $tbody = $("tbody", $tbl);
                var companyId = cInt(dataFor.id());
                var childTRId = "child_" + companyId;
                var $childTR = $("#" + childTRId, $tbody);
                var $treeExpand = $(".tree-expand", this);
                $("#cargoTable .child-rows[cid!='" + companyId + "']").addClass("hide");
                $("#cargoTable .tree-expand[cid!='" + companyId + "']").removeClass("ex-minus").addClass("ex-plus");
                if ($treeExpand.hasClass("ex-plus")) {
                    $treeExpand.removeClass("ex-minus").removeClass("ex-plus");
                    if (!$childTR[0]) {
                        $treeExpand.addClass("ex-minus");
                        $childTR = $("<tr class='child-rows' cid='" + companyId + "'><td symbol='" + dataFor.symbol() + "' company_id='" + companyId + "' colspan='" + $("thead > tr > th", $tbl).length + "' class='child-row-cnt'></td></tr>");
                        $childTR.attr("id", childTRId).attr("row-key", companyId);
                        var $childTD = $("td", $childTR);
                        $tr.after($childTR);
                        $childTD.data("fbjson", fbJson);
                        self.loadTradeDetail($childTD);
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
            $("body").on("click", ".refresh-symbol", function (event) {
                var $this = $(this);
                var dataFor = ko.dataFor(this);
                var url = apiUrl("/Company/RefreshSymbol");
                var data = [];
                data.push({ "name": "symbol", "value": dataFor.symbol() });
                var type = "GET";
                handleBlockUI({ "target": $("body"), "message": "Refresh " + dataFor.symbol() + " ..." });
                $.ajax({
                    "url": url,
                    "cache": false,
                    "type": type,
                    "data": data
                }).done(function (json) {
                    self.refresh();
                    /*
                    if (json != null) {
                        dataFor.ltp_price(json);
                        //dataFor.open_price(json.open_price);
                        //dataFor.high_price(json.high_price);
                        //dataFor.low_price(json.low_price);
                        //dataFor.ltp_price(json.ltp_price);
                        //dataFor.close_price(json.close_price);
                        //dataFor.prev_price(json.prev_price);
                    }
                    */
                    //jAlert("Saved");
                }).fail(function (jqxhr) {
                }).always(function (jqxhr) {
                    unblockUI();
                });
            });
            $("body").on("click", ".open-chart", function (event) {
                var $this = $(this);
                var dataFor = ko.dataFor($this[0]);
                var fbJson = JSON.parse(ko.toJSON(dataFor));
                var $tr = $this.parents("tr:first");
                var $tbl = $this.parents("table:first");
                var $tbody = $("tbody", $tbl);
                var companyId = cInt(dataFor.id());
                var childTRId = "child_chart_" + companyId;
                var $childTR = $("#" + childTRId, $tbody);
                $("#cargoTable .child-rows[cid!='" + companyId + "']").addClass("hide");
                $("#cargoTable .tree-expand[cid!='" + companyId + "']").removeClass("ex-minus").addClass("ex-plus");
                if ($this.hasClass("ex-plus") == false && $this.hasClass("ex-minus") == false) {
                    $this.addClass("ex-plus");
                }
                if ($this.hasClass("ex-plus")) {
                    $this.removeClass("ex-minus").removeClass("ex-plus");
                    if (!$childTR[0]) {
                        $this.addClass("ex-minus");
                        $childTR = $("<tr class='child-rows' cid='" + companyId + "'><td symbol='" + dataFor.symbol() + "' company_id='" + companyId + "' colspan='" + $("thead > tr > th", $tbl).length + "' class='child-row-cnt'></td></tr>");
                        $childTR.attr("id", childTRId).attr("row-key", companyId);
                        var $childTD = $("td", $childTR);
                        $tr.after($childTR);
                        $childTD.data("fbjson", fbJson);
                        self.loadTradeDetailChart($childTD);
                    } else {
                        if ($childTR.hasClass('hide')) {
                            $childTR.removeClass("hide");
                            $this.addClass("ex-minus");
                        } else {
                            $childTR.addClass("hide");
                            $this.addClass("ex-plus");
                        }
                    }
                } else {
                    $childTR.addClass("hide");
                    $this.removeClass("ex-minus").addClass("ex-plus");
                }
            });
        }

        this.offElements = function () {
            $("body").off("click", "#frmCompanySearch #is_book_mark");
            $("body").off("click", "#frmCompanySearch #is_current_stock");
            $("body").off("click", "#frmCompanySearch #is_nifty_50");
            $("body").off("click", "#frmCompanySearch #is_nifty_100");
            $("body").off("click", "#frmCompanySearch #is_nifty_200");
            $("body").off("click", "#frmCompanySearch #is_all_time_low");
            $("body").off("click", "#frmCompanySearch #is_all_time_high");
            $("body").off("click", "#frmCompanySearch #is_all_time_low_15_days");
            $("body").off("click", "#frmCompanySearch #is_all_time_high_15_days");
            $("body").off("click", "#frmCompanySearch #is_all_time_low_5_days");
            $("body").off("click", "#frmCompanySearch #is_all_time_high_5_days");
            $("body").off("click", "#frmCompanySearch #is_all_time_low_2_days");
            $("body").off("click", "#frmCompanySearch #is_all_time_high_2_days");
            $("body").off("click", "#frmCompanySearch #is_low_yesterday");
            $("body").off("click", "#frmCompanySearch #is_high_yesterday");
            $("body").off("click", "#frmCompanySearch #is_sell_to_buy");
            $("body").off("click", "#frmCompanySearch #is_buy_to_sell");
            $("body").off("click", "#frmCompanySearch #is_all_category");
            $("body").off("click", "#frmCompanySearch #is_mf");
            $("body").off("click", "#Company .btn-add");
            $("body").off("click", "#Company .btn-edit");
            $("body").off("click", "#Company .btn-delete");
            $("body").off("change", "#Company #rows");
            $("body").off("click", ".is-book-mark");
            $("body").off("click", ".is-current-stock");
            $("body").off("click", "#Company .btn-export");
            $("body").off("keyup", "#available_amount");
            $("body").off("keyup", "#profit_percentage");
            $("body").off("keyup", "#stoploss_percentage");
            $("body").off("click", ".open-chart");
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
