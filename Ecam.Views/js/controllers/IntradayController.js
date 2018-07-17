"use strict";
define("IntradayController", ["knockout", "komapping", "helper", "service"], function (ko, komapping, helper, service) {
    return function () {
        var self = this;
        this.template = "/Home/Intraday";

        this.rows = ko.observableArray([]);
        this.categories = ko.observableArray([]);

        this.start_date = ko.observable("");
        this.end_date = ko.observable("");
        this.total_start_date = ko.observable("");
        this.total_end_date = ko.observable("");

        //this.trigger_start_date = ko.observable("");
        //this.trigger_end_date = ko.observable("");

        this.avg_profit = ko.observable();
        //this.high_avg_profit = ko.observable();
        //this.low_avg_profit = ko.observable();
        this.positive_count = ko.observable();
        this.negative_count = ko.observable();
        this.positive_percentage = ko.observable();
        this.negative_percentage = ko.observable();

        this._investments = null;

        this.refresh = function () {
            self.loadGrid();
        }

        this.getArr = function () {
            var $Company = $("#Company");
            var arr = $("#frmCompanySearch").serializeArray();
            arr.push({ "name": "PageSize", "value": $(":input[name='rows']", $Company).val() });
            arr.push({ "name": "PageIndex", "value": $(":input[name='page_index']", $Company).val() });
            arr.push({ "name": "SortName", "value": $(":input[name='sort_name']", $Company).val() });
            arr.push({ "name": "SortOrder", "value": $(":input[name='sort_order']", $Company).val() });
            var isarchive = $("#frmCompanySearch #is_archive")[0].checked;
            if (isarchive == true) {
                arr[arr.length] = { "name": "is_archive", "value": isarchive };
            }
            var is_book_mark = $("#frmCompanySearch #is_book_mark")[0].checked;
            if (is_book_mark == true) {
                arr[arr.length] = { "name": "is_book_mark", "value": is_book_mark };
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
            var isOld = $("#frmCompanySearch #is_old")[0].checked;
            if (isOld == true) {
                arr[arr.length] = { "name": "is_old", "value": isOld };
            }
            var is_book_mark_category = $("#frmCompanySearch #is_book_mark_category")[0].checked;
            if (is_book_mark_category == true) {
                arr[arr.length] = { "name": "is_book_mark_category", "value": is_book_mark_category };
            }
            //var is_sell_to_buy = $("#frmCompanySearch #is_sell_to_buy")[0].checked;
            //if (is_sell_to_buy == true) {
            //    arr[arr.length] = { "name": "is_sell_to_buy", "value": is_sell_to_buy };
            //}

            //var is_buy_to_sell = $("#frmCompanySearch #is_buy_to_sell")[0].checked;
            //if (is_buy_to_sell == true) {
            //    arr[arr.length] = { "name": "is_buy_to_sell", "value": is_buy_to_sell };
            //}

            var is_all_category = $("#frmCompanySearch #is_all_category")[0].checked;
            if (is_all_category == true) {
                arr[arr.length] = { "name": "is_all_category", "value": is_all_category };
            }

            //var is_mf = $("#frmCompanySearch #is_mf")[0].checked;
            //if (is_mf == true) {
            //    arr[arr.length] = { "name": "is_mf", "value": is_mf };
            //}
            return arr;
        }

        this.last_params = null;
        this.loadCompanies = function (callback) {
            handleBlockUI();
            var $Company = $("#Company");
            var arr = self.getArr();
            var url = apiUrl("/Company/List");
            self.last_params = arr;
            $.ajax({
                "url": url,
                "cache": false,
                "type": "GET",
                "data": arr
            }).done(function (json) {
                self.rows.removeAll();

                self.avg_profit(0);
                //self.high_avg_profit(0);
                //self.low_avg_profit(0);
                self.positive_count(0);
                self.negative_count(0);
                self.positive_percentage(0);
                self.negative_percentage(0);

                var totalInvestment = 0;
                var totalCurrentValue = 0;
                var positiveCount = 0;
                var negativeCount = 0;

                var highCurrentValue = 0;
                var lowCurrentValue = 0;

                var totalAmount = cFloat($(":input[name='total_amount']").val());
                var totalEquity = json.rows.length;
                var totalInvestmentPerEquity = cFloat(totalAmount / totalEquity);

                var investments = [];
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

                        var quantity = cInt(totalInvestmentPerEquity / cFloat(row.first_price));
                        var investment = cFloat(quantity * cFloat(row.first_price));
                        var cmv = cFloat(quantity * cFloat(row.last_price));
                        //var high_cmv = cFloat(quantity * cFloat(row.profit_high_price));
                        //var low_cmv = cFloat(quantity * cFloat(row.profit_low_price));
                        var profit = cFloat(((cmv - investment) / investment) * 100);
                        //var highProfit = cFloat(((high_cmv - investment) / investment) * 100);
                        //var lowProfit = cFloat(((low_cmv - investment) / investment) * 100);
                        investments.push({
                            "symbol": row.symbol,
                            "quantity": quantity,
                            "investment": investment,
                            "cmv": cmv,
                            //"high_cmv": high_cmv,
                            //"low_cmv": low_cmv,
                            "profit": profit,
                            //"high_profit": highProfit,
                            //"low_profit": lowProfit,
                            "first_price": row.first_price,
                            "last_price": row.last_price
                            //"profit_high_price": row.profit_high_price,
                            //"profit_low_price": row.profit_low_price
                        });

                        var m = komapping.fromJS(row);
                        self.rows.push(m);
                    });
                }

                $.each(investments, function (i, row) {
                    totalInvestment += cFloat(row.investment);
                    totalCurrentValue += cFloat(row.cmv);

                    highCurrentValue += cFloat(row.high_cmv);
                    lowCurrentValue += cFloat(row.low_cmv);

                    var p = ((cFloat(row.last_price) - cFloat(row.first_price)) / cFloat(row.last_price)) * 100;

                    if (cFloat(p) > 0) {
                        positiveCount += 1;
                    } else {
                        negativeCount += 1;
                    }
                });

                self._investments = investments;

                ////////console.log('investments=', investments);
                ////////console.log('totalAmount=', totalAmount, 'totalInvestment=', totalInvestment, 'totalCurrentValue=', totalCurrentValue, 'balance=', (totalAmount - totalInvestment));
                var totalProfitAVG = 0;

                totalProfitAVG = cFloat(cFloat(totalCurrentValue - totalInvestment) / totalInvestment) * 100;
                self.avg_profit(totalProfitAVG);

                totalProfitAVG = cFloat(cFloat(highCurrentValue - totalInvestment) / totalInvestment) * 100;
                //self.high_avg_profit(totalProfitAVG);

                totalProfitAVG = cFloat(cFloat(lowCurrentValue - totalInvestment) / totalInvestment) * 100;
                //self.low_avg_profit(totalProfitAVG);

                self.positive_count(positiveCount);
                self.negative_count(negativeCount);

                var t = positiveCount + negativeCount;
                var p = 0;
                p = (positiveCount / t) * 100;
                self.positive_percentage(p);
                p = (negativeCount / t) * 100;
                self.negative_percentage(p);

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

        this.loadGrid = function (callback) {
            var $active = $(".nav-tabs > li.active > a");
            if ($active.html() == 'Categories') {
                self.loadCategories();
            } else {
                self.loadCompanies(callback);
            }
        }

        this.loadCategories = function () {
            var arr = self.getArr();
            handleBlockUI({ "message": "Loading categories..." });
            var url = apiUrl("/Company/CategoryGroups");
            $.ajax({
                "url": url,
                "cache": false,
                "type": "GET",
                "data": arr
            }).done(function (json) {
                self.categories.removeAll();
                if (json != null) {
                    $.each(json, function (i, row) {
                        var m = komapping.fromJS(row);
                        self.categories.push(m);
                    });
                }
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
            var isarchive = $("#frmCompanySearch #is_archive")[0].checked;
            if (isarchive == true) {
                arr[arr.length] = { "name": "is_archive", "value": isarchive };
            }
            var is_book_mark = $("#frmCompanySearch #is_book_mark")[0].checked;
            if (is_book_mark == true) {
                arr[arr.length] = { "name": "is_book_mark", "value": is_book_mark };
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

            var $pageContent = $(".page-content");
            //var arr = [];
            //var categories = 'CEMENT & CEMENT PRODUCTS,ENERGY,CHEMICALS,CONSTRUCTION,CONSUMER GOODS,METALS,NIFTY FMGC,RETAIL,TEXTILES,FERTILISERS & PESTICIDES,MEDIA & ENTERTAINMENT,NIFTY INFRA,PAPER';
            //var caregoryList = categories.split(',');
            //$.each(caregoryList, function (i, cat) {
            //    arr.push({ "id": cat, "text": cat });
            //});
            //$categories.select2Refresh("data", arr);

            //select2Setup($mf_ids[0], {
            //    multiple: true
            //   , width: 300
            //   , url: apiUrl("/Company/SelectMFS")
            //   , placeholder: "Select MFS"
            //   , resultsCallBack: function (data, page) {
            //       var s2data = [];
            //       $.each(data, function (i, d) {
            //           s2data.push({ "id": d.id, "text": d.label, "source": d });
            //       });
            //       return { results: s2data };
            //   }
            //   , onParam: function (term, page) {
            //       return {
            //           term: term
            //       };
            //   }
            //   , onChange: function (e, ui) {
            //       var $target = $(".page-content");
            //       self.loadGrid();
            //   }
            //});

            // Current month
            var arrlastsixmonths = helper.getLastSixMonths();
            var start = moment(_TODAYDATE).startOf('month');//moment(_TODAYDATE).subtract('days', 30);// moment(arrlastsixmonths[0]);
            var end = moment(_TODAYDATE).endOf('month'); //moment(_TODAYDATE);//moment(arrlastsixmonths[1]);
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


            // Total start,end date
            var start = moment(_TODAYDATE).subtract('month', 1).endOf('month').subtract('month', 6).add('days', 7).startOf('month');
            var end = moment(_TODAYDATE).subtract('month', 1).endOf('month');
            self.total_start_date(start.format('MM/DD/YYYY'));
            self.total_end_date(end.format('MM/DD/YYYY'));
            ////////console.log('$pageContent=',$pageContent[0]);
            var $totalReportRange = $('#total_reportrange', $pageContent);
            ////////console.log('reportrange=',$totalReportRange[0]);
            helper.handleDateRangePicker($totalReportRange, {
                'opens': 'left',
                'start': start,
                'end': end,
                'changeDate': function (start, end) {
                    var daysDiff = helper.getTimeDiff(start.format('MM/DD/YYYY'), end.format('MM/DD/YYYY')).days;
                    self.total_start_date(start.format('MM/DD/YYYY'));
                    self.total_end_date(end.format('MM/DD/YYYY'));
                    helper.changeDateRangeLabel($('span', $totalReportRange), start, end, self.total_start_date(), self.total_end_date());
                    self.loadGrid();
                }
            });
            helper.changeDateRangeLabel($('span', $totalReportRange), start, end, self.total_start_date(), self.total_end_date());


            //// Trigger start,end date
            //var start = moment(_TODAYDATE).subtract('month', 1).endOf('month').subtract('month', 1).add('days', 7).startOf('month');
            //var end = moment(_TODAYDATE).subtract('month', 1).endOf('month');
            //self.trigger_start_date(start.format('MM/DD/YYYY'));
            //self.trigger_end_date(end.format('MM/DD/YYYY'));
            //////////console.log('$pageContent=',$pageContent[0]);
            //var $triggerReportRange = $('#trigger_reportrange', $pageContent);
            //////////console.log('reportrange=',$triggerReportRange[0]);
            //helper.handleDateRangePicker($triggerReportRange, {
            //    'opens': 'left',
            //    'start': start,
            //    'end': end,
            //    'changeDate': function (start, end) {
            //        var daysDiff = helper.getTimeDiff(start.format('MM/DD/YYYY'), end.format('MM/DD/YYYY')).days;
            //        self.trigger_start_date(start.format('MM/DD/YYYY'));
            //        self.trigger_end_date(end.format('MM/DD/YYYY'));
            //        helper.changeDateRangeLabel($('span', $triggerReportRange), start, end, self.trigger_start_date(), self.trigger_end_date());
            //        self.loadGrid();
            //    }
            //});
            //helper.changeDateRangeLabel($('span', $triggerReportRange), start, end, self.trigger_start_date(), self.trigger_end_date());
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
                //////console.log(row.category_list);
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

        this.openBatchLog = function () {
            $('#temp-batch-modal-container').remove();
            var $cnt = $("<div id='temp-batch-modal-container'></div>");
            $('body').append($cnt);
            handleBlockUI();
            var arr = self.last_params;
            var url = apiUrl("/Company/BatchList");
            $.ajax({
                "url": url,
                "cache": false,
                "type": "GET",
                "data": arr
            }).done(function (json) {
                var data = {
                    "name": "Investment"
                   , "title": "Investment"
                   , "is_modal_full": false
                   , "position": "top"
                   , "width": $(window).width() - 100
                    , "rows": json
                };
                $("#modal-batch-template").tmpl(data).appendTo($cnt);
                var $modal = $("#modal-batch-" + data.name, $cnt);
                $modal.modal('show');
            }).always(function () {
                unblockUI();
            });
        }

        this.openDailyLog = function () {
            self.openDailyLogModal(self.last_params);
        }

        this.openDailyLogModal = function (arr, callback, isNotOpen) {
            $('#temp-daily-modal-container').remove();
            var $cnt = $("<div id='temp-daily-modal-container'></div>");
            if (isNotOpen != true) {
                $('body').append($cnt);
            }
            handleBlockUI();
            var url = apiUrl("/Company/DailyList");
            $.ajax({
                "url": url,
                "cache": false,
                "type": "GET",
                "data": arr
            }).done(function (json) {
                var data = {
                    "name": "Investment"
                   , "title": "Investment"
                   , "is_modal_full": false
                   , "position": "top"
                   , "width": $(window).width() - 100
                    , "rows": json
                };
                if (callback) {
                    callback(json);
                }
                if (isNotOpen != true) {
                    //console.log(data);
                    $("#modal-daily-template").tmpl(data).appendTo($cnt);
                    var $modal = $("#modal-daily-" + data.name, $cnt);
                    $modal.modal('show');
                }
            }).always(function () {
                unblockUI();
            });
        }

        this.openInvestments = function () {
            self.openInvestmentsModal(self._investments);
        }

        this.openInvestmentsModal = function (investments) {
            investments = investments.sort(getDescOrder("profit"));
            $('#temp-investment-modal-container').remove();
            var $cnt = $("<div id='temp-investment-modal-container'></div>");
            $('body').append($cnt);
            var totalAmount = cFloat($(":input[name='total_amount']").val());
            var totalInvestment = 0, totalCMV = 0;
            //var totalHighCMV = 0, totalLowCMV = 0;
            var balance = 0, profit = 0;
            //var highProfit = 0, lowProfit = 0;
            var positiveCount = 0;
            var negativeCount = 0;
            $.each(investments, function (i, row) {
                totalInvestment += cFloat(row.investment);
                totalCMV += cFloat(row.cmv);
                //totalHighCMV += cFloat(row.high_cmv);
                //totalLowCMV += cFloat(row.low_cmv);
                if (cFloat(row.profit) > 0) {
                    positiveCount += 1;
                } else {
                    negativeCount += 1;
                }
            });
            balance = cFloat(totalAmount - totalInvestment);
            profit = cFloat(cFloat(totalCMV - totalInvestment) / totalInvestment) * 100;
            //highProfit = cFloat(cFloat(totalHighCMV - totalInvestment) / totalInvestment) * 100;
            //lowProfit = cFloat(cFloat(totalLowCMV - totalInvestment) / totalInvestment) * 100;
            var data = {
                "name": "Investment"
                , "title": "Investment"
                , "is_modal_full": false
                , "position": "top"
                , "width": $(window).width() - 100
                , "rows": investments
                , "TotalAmount": totalAmount
                , "TotalInvestment": totalInvestment
                , "TotalCMV": totalCMV
                //, "TotalHighCMV": totalHighCMV
                //, "TotalLowCMV": totalLowCMV
                , "Balance": balance
                , "Profit": profit
                //, "HighProfit": highProfit
                //, "LowProfit": lowProfit
                , "PositiveCount": positiveCount
                , "NegativeCount": negativeCount
            };
            //////console.log(data);
            $("#modal-investment-template").tmpl(data).appendTo($cnt);
            var $modal = $("#modal-investment-" + data.name, $cnt);
            $modal.modal('show');
        }

        this.start_year_wise = false;
        this.year_count = 0;
        this.start_index = -1;
        this.temp_investments = [];
        this.temp_total_amount = 0;
        this.openLog = function () {
            var $modal = $(".modal");
            $modal.modal('hide');
            $('#temp-modal-container').remove();
            var $cnt = $("<div id='temp-modal-container'></div>");
            $('body').append($cnt);
            var data = {
                "name": "Log"
                , "title": "Log"
                , "is_modal_full": false
                , "position": "top"
                , "width": $(window).width() - 100
            };
            $("#modal-log-template").tmpl(data).appendTo($cnt);
            $modal = $("#modal-log-" + data.name, $cnt);
            $modal.modal('show');
            var $selYear = $("#selYear", $modal);
            //$selYear.val(moment(_TODAYDATE).format('YYYY'));
            //self.temp_investments = [];
            //self.start_index = -1;
            //self.year_count = 0;
            //self.createLogs($modal, cFloat($(":input[name='total_amount']").val()));
            $selYear.change(function () {
                if (this.value != '') {
                    self.temp_investments = [];
                    self.start_index = -1;
                    self.year_count = 0;
                    self.createLogs($modal, cFloat($(":input[name='total_amount']").val()), '');
                }
            });
        }


        this.createLogs = function ($modal, totalAmount, ignoreSymbols) {
            ////console.log('ignoreSymbols7=', ignoreSymbols);
            totalAmount = cFloat(totalAmount);
            var monthCount = cInt($("#months", $modal).val());
            var totalCount = 11;
            self.start_index = cInt(self.start_index) + 1;
            var $selYear = $("#selYear", $modal);
            var index = self.start_index; // totalCount - self.start_index;
            console.log('index=', index, 'monthCount=', monthCount);
            var finYearStart = moment('04/01/' + $selYear.val()).format('MM/DD/YYYY');
            console.log('finYearStart=', finYearStart);
            var startDate = moment(finYearStart).add('month', index).startOf('month').format('MM/DD/YYYY');
            var endDate = moment(finYearStart).add('month', index).endOf('month').format('MM/DD/YYYY');
            var totalStartDate = moment(startDate).subtract('month', monthCount).add('days', 7).startOf('month').format('MM/DD/YYYY');
            var totalEndDate = moment(totalStartDate).add('month', monthCount).add('days', -7).endOf('month').format('MM/DD/YYYY');

            console.log('startDate=', formatDate(startDate, 'DD/MMM/YYYY'), 'endDate=', formatDate(endDate, 'DD/MMM/YYYY'), 'totalStartDate=', formatDate(totalStartDate, 'DD/MMM/YYYY'), 'totalEndDate=', formatDate(totalEndDate, 'DD/MMM/YYYY'));
            //var startDate = moment(_TODAYDATE).subtract('days', (index * 14)).format('MM/DD/YYYY');
            //var endDate = moment(startDate).add('days', 13).format('MM/DD/YYYY');
            //var totalStartDate = moment(startDate).subtract('days', 175).format('MM/DD/YYYY');
            //var totalEndDate = moment(startDate).subtract('days', 1).format('MM/DD/YYYY');

            //var triggerStartDate = moment(_TODAYDATE).subtract('month', index + 1).endOf('month').subtract('month', 1).add('days', 7).startOf('month').format('MM/DD/YYYY');
            //var triggerEndDate = moment(_TODAYDATE).subtract('month', index + 1).endOf('month').format('MM/DD/YYYY');

            //var $chkCalcSameYear = $("#chkCalcSameYear", $modal);
            //if ($chkCalcSameYear[0]) {
            //    if ($chkCalcSameYear[0].checked == true) {
            //        var strTotalStartDate = formatDate(startDate, 'MMM');
            //        if (strTotalStartDate != 'Jan') {
            //            totalStartDate = "01/01/" + $selYear.val();
            //        } else {
            //            totalStartDate = "01/01/" + (cInt($selYear.val()) - 1);
            //            totalEndDate = "12/31/" + (cInt($selYear.val()) - 1);
            //        }
            //    }
            //}

            if (self.start_index <= totalCount) {

                var arr = [];
                if (self.last_params != null) {
                    $.each(self.last_params, function (i, row) {
                        if (row.name == "PageIndex") {
                            row.value = 1;
                        } else if (row.name == "PageSize") {
                            //row.value = 10;
                        } else if (row.name == "SortName") {
                            row.value = "total_profit";
                        } else if (row.name == "SortOrder") {
                            row.value = "desc";
                        } else if (row.name == "start_date") {
                            row.value = startDate
                        } else if (row.name == "end_date") {
                            row.value = endDate;
                        } else if (row.name == "total_start_date") {
                            row.value = totalStartDate;
                        } else if (row.name == "total_end_date") {
                            row.value = totalEndDate;
                        }
                        //else if (row.name == "trigger_start_date") {
                        //    row.value = triggerStartDate;
                        //} else if (row.name == "trigger_end_date") {
                        //    row.value = triggerEndDate;
                        //}
                        arr.push(row);
                    });
                }
                //console.log('ignoreSymbols=', ignoreSymbols);
                //arr.push({ 'name': 'ignore_symbols', 'value': ignoreSymbols });
                //arr.push({ 'name': 'PageIndex', 'value': 1 });
                //arr.push({ 'name': 'PageSize', 'value': 10 });
                //arr.push({ 'name': 'SortName', 'value': 'total_profit' });
                //arr.push({ 'name': 'SortOrder', 'value': 'desc' });
                //arr.push({ 'name': 'start_date', 'value': startDate });
                //arr.push({ 'name': 'end_date', 'value': endDate });
                //arr.push({ 'name': 'total_start_date', 'value': totalStartDate });
                //arr.push({ 'name': 'total_end_date', 'value': totalEndDate });
                //arr.push({ 'name': 'ignore_symbols', 'value': $(":input[name='ignore_symbols']").val() });
                ////arr.push({ 'name': 'total_from_profit', 'value': 1 });
                ////arr.push({ 'name': 'is_nifty_200', 'value': true });
                ////arr.push({ 'name': 'max_negative_count', 'value': 1 });
                //$.each(arr, function (i, row) {
                //    if (row.name == "categories") {
                //        row.value = categories;
                //    }
                //});

                handleBlockUI({ "message": 'Loading... - ' + formatDate(startDate) }); // + 'startDate='+  formatDate(startDate) +  'endDate=', formatDate(endDate)+  'totalStartDate='+  formatDate(totalStartDate)+  'totalEndDate='+  formatDate(totalEndDate) + ' ...' });
                ////////console.log('index=', index, 'startDate=', formatDate(startDate), 'endDate=', formatDate(endDate), 'totalStartDate=', formatDate(totalStartDate), 'totalEndDate=', formatDate(totalEndDate));

                var url = apiUrl("/Company/List");
                $.ajax({
                    "url": url,
                    "cache": false,
                    "type": "GET",
                    "data": arr
                }).done(function (json) {
                    self.openDailyLogModal(self.last_params, function (dailyJSON) {

                        var dailyHigh = 0;
                        var dailyLow = 0;
                        var dailyArr = [];
                        var negativeArr = [];
                        var positiveArr = [];
                        $.each(dailyJSON, function (i, daily) {
                            if (daily.percentage != -100 && daily.percentage != 100) {
                                dailyArr.push(daily.percentage);
                                if (daily.percentage > 0) {
                                    positiveArr.push(daily.percentage);
                                } else {
                                    negativeArr.push(daily.percentage);
                                }
                            }
                        });

                        if (positiveArr.length > 0) {
                            dailyHigh = findMaxNo(positiveArr);
                        } else {
                            dailyHigh = findMaxNo(negativeArr);
                        }
                        if (negativeArr.length > 0) {
                            dailyLow = findMinNo(negativeArr);
                        } else {
                            dailyLow = findMinNo(positiveArr);
                        }
                        //console.log('positiveArr=', positiveArr);
                        //console.log('negativeArr=', negativeArr);
                        //console.log('dailyHigh=', dailyHigh, 'dailyLow=', dailyLow);

                        var totalEquity = json.rows.length;
                        var totalInvestmentPerEquity = cFloat(totalAmount / totalEquity);

                        //////console.log('totalAmount=', totalAmount, 'totalEquity=', totalEquity, 'totalInvestmentPerEquity=', totalInvestmentPerEquity);

                        var totalInvestment = 0;
                        var totalCurrentValue = 0;
                        var positiveCount = 0;
                        var negativeCount = 0;

                        var highCurrentValue = 0;
                        var lowCurrentValue = 0;

                        var investments = [];
                        var symbols = '';
                        if (json.rows != null) {
                            $.each(json.rows, function (i, row) {
                                symbols += row.symbol + ',';

                                if (cFloat(row.first_price) <= 0) { row.first_price = 0; }
                                if (cFloat(row.last_price) <= 0) { row.last_price = 0; }
                                //if (cFloat(row.profit_high_price) <= 0) { row.profit_high_price = 1; }
                                //if (cFloat(row.profit_low_price) <= 0) { row.profit_low_price = 1; }

                                var quantity = cInt(totalInvestmentPerEquity / cFloat(row.first_price));
                                var investment = cFloat(quantity * cFloat(row.first_price));
                                var cmv = cFloat(quantity * cFloat(row.last_price));
                                //var high_cmv = cFloat(quantity * cFloat(row.profit_high_price));
                                //var low_cmv = cFloat(quantity * cFloat(row.profit_low_price));
                                var profit = cFloat(((cmv - investment) / investment) * 100);
                                //var highProfit = cFloat(((high_cmv - investment) / investment) * 100);
                                //var lowProfit = cFloat(((low_cmv - investment) / investment) * 100);

                                investments.push({
                                    "symbol": row.symbol,
                                    "quantity": quantity,
                                    "investment": investment,
                                    "cmv": cmv,
                                    //"high_cmv": high_cmv,
                                    //"low_cmv": low_cmv,
                                    "profit": profit,
                                    //"high_profit": highProfit,
                                    //"low_profit": lowProfit,
                                    "first_price": row.first_price,
                                    "last_price": row.last_price
                                    //"profit_high_price": row.profit_high_price,
                                    //"profit_low_price": row.profit_low_price
                                });
                            });
                        }


                        ignoreSymbols = '';
                        $.each(investments, function (i, row) {
                            totalInvestment += cFloat(row.investment);
                            totalCurrentValue += cFloat(row.cmv);

                            highCurrentValue += cFloat(row.high_cmv);
                            lowCurrentValue += cFloat(row.low_cmv);

                            if (cFloat(row.profit) > 0) {
                                positiveCount += 1;
                            } else {
                                negativeCount += 1;
                                ignoreSymbols += row.symbol + ",";
                                ////console.log('ignoreSymbols3=', ignoreSymbols);
                            }
                        });
                        ////console.log('ignoreSymbols8=', ignoreSymbols);
                        if (ignoreSymbols != '') {
                            ////console.log('ignoreSymbols9 substring=', ignoreSymbols.substring(0, ignoreSymbols - 1));
                            ignoreSymbols = ignoreSymbols.substring(0, ignoreSymbols.length - 1);
                        }
                        ////console.log('ignoreSymbols2=', ignoreSymbols);

                        if (investments.length <= 0) {
                            totalInvestment = totalAmount;
                            totalCurrentValue = totalAmount;
                            highCurrentValue = totalAmount;
                            lowCurrentValue = totalAmount;
                        }

                        var maximumLoss = cFloat($("#maximum_loss", $modal).val());
                        if (maximumLoss > 0) {
                            maximumLoss = maximumLoss * -1;
                            var pr = cFloat(cFloat(totalCurrentValue - totalInvestment) / totalInvestment) * 100;
                            ////console.log('pr=', pr);
                            if (dailyLow <= maximumLoss) {
                                pr = maximumLoss * -1;
                                ////console.log('pr fixed=' + maximumLoss);
                                totalCurrentValue = (totalInvestment - (cFloat(totalInvestment) * cFloat(pr) / 100));
                            }
                        }

                        //////console.log('before totalAmount=', totalAmount, 'totalInvestment=', totalInvestment, 'totalCurrentValue=', totalCurrentValue, 'highCurrentValue=', highCurrentValue, 'lowCurrentValue=', lowCurrentValue);

                        totalCurrentValue = self.calcFinalMarketValue(totalCurrentValue, investments.length, false);
                        highCurrentValue = self.calcFinalMarketValue(highCurrentValue, investments.length, false);
                        lowCurrentValue = self.calcFinalMarketValue(lowCurrentValue, investments.length, false);

                        //////console.log('after totalAmount=', totalAmount, 'totalInvestment=', totalInvestment, 'totalCurrentValue=', totalCurrentValue, 'highCurrentValue=', highCurrentValue, 'lowCurrentValue=', lowCurrentValue);

                        var totalProfitAVG = 0;

                        var data = {
                            'total_from_date': formatDate(totalStartDate),
                            'total_to_date': formatDate(totalEndDate),
                            'from_date': formatDate(startDate),
                            'to_date': formatDate(endDate),
                            'low_avg_profit': 0,
                            'high_avg_profit': 0,
                            'avg_profit': 0,
                            'daily_high': dailyHigh,
                            'daily_low': dailyLow,
                            'total_equity': 0,
                            'symbols': symbols,
                            'total_amount': totalAmount,
                            'investment': totalInvestment,
                            'cmv': totalCurrentValue,
                            'investments': investments,
                            'positive_count': positiveCount,
                            'negative_count': negativeCount,
                            'positive_percentage': (positiveCount / (positiveCount + negativeCount)) * 100,
                            'negative_percentage': (negativeCount / (positiveCount + negativeCount)) * 100
                        };

                        data.total_equity = json.rows.length;

                        totalProfitAVG = cFloat(cFloat(totalCurrentValue - totalInvestment) / totalInvestment) * 100;
                        //self.avg_profit(totalProfitAVG);
                        if (data.total_equity > 0) {
                            data.avg_profit = totalProfitAVG;
                        } else {
                            data.avg_profit = 0;
                        }

                        totalProfitAVG = cFloat(cFloat(highCurrentValue - totalInvestment) / totalInvestment) * 100;
                        //self.high_avg_profit(totalProfitAVG);
                        if (data.total_equity > 0) {
                            data.high_avg_profit = totalProfitAVG;
                        } else {
                            data.high_avg_profit = 0;
                        }

                        totalProfitAVG = cFloat(cFloat(lowCurrentValue - totalInvestment) / totalInvestment) * 100;
                        //self.low_avg_profit(totalProfitAVG);
                        if (data.total_equity > 0) {
                            data.low_avg_profit = totalProfitAVG;
                        } else {
                            data.low_avg_profit = 0;
                        }

                        var $tbl = $("#tblLog", $modal);
                        var $tbody = $("tbody", $tbl);
                        unblockUI();
                        //if (self.start_index <= 50
                        //&& formatDate(startDate).indexOf('2016') < 0
                        //   ) {

                        //console.log('data=', data);
                        $("#detail-log-template").tmpl(data).appendTo($tbody);

                        self.temp_investments.push(investments);

                        var balance = 0; var profit = 0; var highProfit = 0; var lowProfit = 0;

                        balance = cFloat(totalAmount - totalInvestment);

                        //////console.log('before balance=', balance);
                        balance = cFloat(balance) - cFloat(self.getCharges(totalInvestment, investments.length, true));
                        //////console.log('after balance=', balance);

                        profit = cFloat(cFloat(totalCurrentValue - totalInvestment) / totalInvestment) * 100;
                        highProfit = cFloat(cFloat(highCurrentValue - totalInvestment) / totalInvestment) * 100;
                        lowProfit = cFloat(cFloat(lowCurrentValue - totalInvestment) / totalInvestment) * 100;

                        var monthlyInvestment = cFloat($(":input[name='monthly_investment']").val());
                        totalAmount = cFloat(totalCurrentValue) + cFloat(balance) + monthlyInvestment;
                        //////console.log('follow next year totalCurrentValue=', totalCurrentValue, 'balance=', balance, 'monthlyInvestment=', monthlyInvestment);
                        $(":input[name='total_amount']").val(totalAmount);

                        ////console.log('ignoreSymbols4=', ignoreSymbols);
                        self.createLogs($modal, totalAmount, ignoreSymbols);

                        //cFloat($(":input[name='total_amount']").val())
                        //////console.log('cell=', $("tr:eq(0) > td:eq(0)", $tbody)[0]);
                        //////console.log('amount=', cFloat($("tr:eq(0) > td:eq(0)", $tbody).html()))
                        var totalFinalAmount = cFloat($("tr:eq(0) > td:eq(0)", $tbody).html()) + (monthlyInvestment * cInt($("tr", $tbody).length));
                        $("#spnFinalTotalAmount", $modal).html(formatNumber(totalFinalAmount));
                        $("#spnFinalTotalCMV", $modal).html(formatNumber(totalAmount));
                        var p = ((cFloat(totalAmount) - cFloat(totalFinalAmount)) / cFloat(totalFinalAmount)) * 100;
                        $("#spnFinalTotalProfit", $modal).html(formatPercentage(p));

                        // } else {

                        // }

                        var $tblLogCount = $("#tblLogCount", $modal);
                        $("tbody", $tblLogCount).empty();
                        var data = [];
                        $("tr", $tbody).each(function () {
                            var $tr = $(this);
                            var symbols = cString($(":input[name='symbols']", $tr).val());
                            if (symbols != '') {
                                var arr = symbols.split(',');
                                $.each(arr, function (i, sym) {
                                    if (cString(sym) != '') {
                                        var temp = null;
                                        $.each(data, function (j, d) {
                                            if (d.symbol == sym) {
                                                temp = d;
                                            }
                                        });
                                        if (temp == null) {
                                            temp = { "symbol": sym, "count": 0 };
                                            data.push(temp);
                                        }
                                        temp.count += 1;
                                    }
                                });
                            }
                        });
                        data = data.sort(getDescOrder("count"));
                        ////////console.log('data=', data);
                        $.each(data, function (i, row) {
                            $("tbody", $tblLogCount).append("<tr><td>" + row.symbol + "</td><td class='text-right'>" + row.count + "</td></tr>");
                        });

                        unblockUI();



                    }, true);

                }).always(function () {
                });
            } else if (self.start_index <= totalCount) {
                $(":input[name='total_amount']").val(totalAmount);
                ////console.log('ignoreSymbols5=', ignoreSymbols);
                self.createLogs($modal, totalAmount, ignoreSymbols);
            }
            if (self.start_index > totalCount) {
                var $tbl = $("#tblLog", $modal);
                var $tbody = $("tbody", $tbl);
                $("tr", $tbody).each(function () {
                    var $tr = $(this);
                    var $search = $(".fa-search", $tr);
                    $search.unbind('click').click(function () {
                        var index = $("tr", $tbody).index($tr);
                        var arr = self.temp_investments[index];
                        self.openInvestmentsModal(arr);
                    });
                    var $lnkDaily = $("#lnkDaily", $tr);
                    $lnkDaily.unbind('click').click(function () {
                        var arr = self.last_params;
                        $.each(arr, function (i, p) {
                            switch (p.name) {
                                case "start_date": p.value = $("#FromDate", $tr).val(); break;
                                case "end_date": p.value = $("#ToDate", $tr).val(); break;
                                case "total_start_date": p.value = $("#TotalFromDate", $tr).val(); break;
                                case "total_end_date": p.value = $("#TotalToDate", $tr).val(); break;
                            }
                        });
                        self.openDailyLogModal(arr);
                    });
                });
                //////console.log('self.year_count=', self.year_count);
                self.year_count += 1;
                var finalEndDate = moment(_TODAYDATE).add('year', self.year_count).format('DD/MMM/YYYY');
                $("#spnFinalEndDate", $modal).html(finalEndDate);
                var $tblYearCount = $("#tblYearCount", $modal);
                if (self.start_year_wise == true) {
                    $("tbody", $tblYearCount).append("<tr><td>" + finalEndDate + "</td><td class='text-right'>" + $("#spnFinalTotalCMV", $modal).html() + "</td></tr>");
                }
                if (self.year_count <= 10 && self.start_year_wise == true) {
                    var $tbl = $("#tblLog", $modal);
                    var $tbody = $("tbody", $tbl);
                    $tbody.empty();
                    var $tblLogCount = $("#tblLogCount", $modal);
                    $("tbody", $tblLogCount).empty();
                    self.temp_investments = [];
                    self.start_index = -1;
                    //////console.log('start totalAmount=', totalAmount);
                    $(":input[name='total_amount']").val(totalAmount);
                    ////console.log('ignoreSymbols6=', ignoreSymbols);
                    self.createLogs($modal, totalAmount, ignoreSymbols);
                }
            }

        }

        this.calcFinalMarketValue = function (totalMarketValue, totalEquity, isInvestment) {
            return (cFloat(totalMarketValue) - self.getCharges(totalMarketValue, totalEquity, isInvestment));
        }

        this.getCharges = function (totalMarketValue, totalEquity, isInvestment) {
            totalMarketValue = cFloat(totalMarketValue);
            totalEquity = cInt(totalEquity);
            var stt = (totalMarketValue * 0.1) / 100;
            var txn = (totalMarketValue * 0.00325) / 100;
            var gst = (txn * 18) / 100;
            var stamb = (totalMarketValue * 0.006) / 100;
            var sebi = ((15 * totalMarketValue) / 10000000);
            var dpcharges = (totalEquity * 15.93);
            if (isInvestment == true) {
                dpcharges = 0;
            }
            return (stt + txn + gst + stamb + sebi + dpcharges);
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
                    ////////console.log($('#curve_chart', $rsiBox)[0]);
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
                //self.loadAVG(symbol, $avgMonthBox, function () {
                //});
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

        this.loadDefaultCategories = function () {
            var $frmCompanySearch = $("#frmCompanySearch");
            var $symbols = $(":input[name='symbols']", $frmCompanySearch);
            var $categories = $(":input[name='categories']", $frmCompanySearch);
            var $pageContent = $(".page-content");
            var startDate = formatDate(self.total_start_date());
            var endDate = formatDate(self.total_end_date());
            var categories = 'CEMENT & CEMENT PRODUCTS,CHEMICALS,CONSTRUCTION,CONSUMER GOODS,FOOD PROCESSING,RETAIL,TEXTILES,AUTOMOBILE,FINANCIAL SERVICES';
            //self.getTop10Categories(startDate, endDate, function (categories) {
            var caregoryList = categories.split(',');
            //////console.log('caregoryList=', caregoryList);
            var carr = [];
            $.each(caregoryList, function (i, cat) {
                if (cString(cat) != '') {
                    carr.push({ "id": cat, "text": cat });
                }
            });
            //////console.log('carr=', carr);
            $categories.select2Refresh("data", carr);
            //});
        }

        this.getTop10Categories = function (startDate, endDate, callback) {
            var $frmCompanySearch = $("#frmCompanySearch");
            var $symbols = $(":input[name='symbols']", $frmCompanySearch);
            var $categories = $(":input[name='categories']", $frmCompanySearch);
            var $pageContent = $(".page-content");
            handleBlockUI();
            var url = apiUrl("/Category/GetMonthlyProfitCategories");
            var arr = [];
            arr[arr.length] = { "name": "startDate", "value": startDate };
            arr[arr.length] = { "name": "endDate", "value": endDate };
            $.ajax({
                "url": url,
                "cache": false,
                "type": "GET",
                "data": arr
            }).done(function (json) {
                var categories = '';
                $.each(json, function (i, name) {
                    categories += name + ',';
                });
                if (categories != '') {
                    categories = categories.substring(0, categories.length - 1);
                }
                //////console.log('categories=', categories);
                if (callback)
                    callback(categories);
            })
            .always(function () {
                unblockUI();
            });
        }

        this.goToNextSlap = function (index) {
            var $pageContent = $(".page-content");
            var $frmCompanySearch = $("#frmCompanySearch");
            var $btnNextSlap = $("#btnNextSlap", $frmCompanySearch);
            var $btnPrevSlap = $("#btnPrevSlap", $frmCompanySearch);
            $btnNextSlap.attr("index", index);
            $btnPrevSlap.attr("index", index);

            var start = moment(_TODAYDATE).subtract('month', index).startOf('month');
            var end = moment(_TODAYDATE).subtract('month', index).endOf('month');
            self.start_date(start.format('MM/DD/YYYY'));
            self.end_date(end.format('MM/DD/YYYY'));
            var $reportRange = $('#reportrange', $pageContent);
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

            start = moment(_TODAYDATE).subtract('month', index + 1).endOf('month').subtract('month', 6).add('days', 7).startOf('month');
            end = moment(_TODAYDATE).subtract('month', index + 1).endOf('month');
            self.total_start_date(start.format('MM/DD/YYYY'));
            self.total_end_date(end.format('MM/DD/YYYY'));
            var $totalReportRange = $('#total_reportrange', $pageContent);
            helper.handleDateRangePicker($totalReportRange, {
                'opens': 'left',
                'start': start,
                'end': end,
                'changeDate': function (start, end) {
                    var daysDiff = helper.getTimeDiff(start.format('MM/DD/YYYY'), end.format('MM/DD/YYYY')).days;
                    self.total_start_date(start.format('MM/DD/YYYY'));
                    self.total_end_date(end.format('MM/DD/YYYY'));
                    helper.changeDateRangeLabel($('span', $totalReportRange), start, end, self.total_start_date(), self.total_end_date());
                    self.loadGrid();
                }
            });
            helper.changeDateRangeLabel($('span', $totalReportRange), start, end, self.total_start_date(), self.total_end_date());

            //self.loadGrid();
        }

        this.onElements = function () {
            self.offElements();
            $("body").on("click", "#frmCompanySearch #btnNextSlap", function (event) {
                var index = cInt($(this).attr('index'));
                index -= 1;
                self.goToNextSlap(index);
            });
            $("body").on("click", "#frmCompanySearch #btnPrevSlap", function (event) {
                var index = cInt($(this).attr('index'));
                index += 1;
                self.goToNextSlap(index);
            });
            $("body").on("click", "#frmCompanySearch #is_archive", function (event) {
                self.loadGrid();
            });
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
            $("body").on("click", "#frmCompanySearch #is_old", function (event) {
                self.loadGrid();
            });
            $("body").on("click", "#frmCompanySearch #is_book_mark_category", function (event) {
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
            $("body").on("click", "#btnYear", function (event) {
                self.start_year_wise = true;
                self.openLog();
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
            $("body").on("click", ".is-archive", function (event) {
                var $this = $(this);
                var $i = $("i", $this);
                var dataFor = ko.dataFor(this);
                var url = apiUrl('/Company/UpdateArchive');
                var arr = [];
                var isArchive = $i.hasClass('fa-bookmark');
                arr.push({ "name": "symbol", "value": dataFor.symbol() });
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
            $("body").on("click", ".is-book-mark", function (event) {
                var $this = $(this);
                var $i = $("i", $this);
                var dataFor = ko.dataFor(this);
                var url = apiUrl('/Company/UpdateBookMark');
                var arr = [];
                var is_book_mark = $i.hasClass('fa-user');
                arr.push({ "name": "symbol", "value": dataFor.symbol() });
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
            $("body").on("keyup", "#available_amount", function (event) {
                self.calculateJSON();
            });
            $("body").on("keyup", "#profit_percentage", function (event) {
                self.calculateJSON();
            });
            $("body").on("keyup", "#stoploss_percentage", function (event) {
                self.calculateJSON();
            });
            $("body").on("click", "#CategoryTable > tbody > tr > td.cls-symbol", function (event) {
                var $this = $(this);
                var dataFor = ko.dataFor($this[0]);
                var $tr = $this.parents("tr:first");
                var $tbl = $this.parents("table:first");
                var $tbody = $("tbody", $tbl);
                var childTRId = "tr_child_" + $tr.attr('index');
                var $childTR = $("#" + childTRId, $tbody);
                ////////console.log('childTRId=', childTRId, '$childTR=', $childTR[0]);
                var $treeExpand = $(".tree-expand", this);
                if ($treeExpand.hasClass("ex-plus")) {
                    $treeExpand.removeClass("ex-minus").removeClass("ex-plus");
                    if (!$childTR[0]) {
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
            $("body").off("click", "#frmCompanySearch #btnNextSlap");
            $("body").off("click", "#frmCompanySearch #btnPrevSlap");
            $("body").off("click", "#frmCompanySearch #is_archive");
            $("body").off("click", "#frmCompanySearch #is_book_mark");
            $("body").off("click", "#frmCompanySearch #is_current_stock");
            $("body").off("click", "#frmCompanySearch #is_nifty_50");
            $("body").off("click", "#frmCompanySearch #is_nifty_100");
            $("body").off("click", "#frmCompanySearch #is_nifty_200");
            $("body").off("click", "#frmCompanySearch #is_old");
            $("body").off("click", "#frmCompanySearch #is_book_mark_category");
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
            $("body").off("click", "#btnYear");
            $("body").off("click", "#Company .btn-add");
            $("body").off("click", "#Company .btn-edit");
            $("body").off("click", "#Company .btn-delete");
            $("body").off("change", "#Company #rows");
            $("body").off("click", ".is-archive");
            $("body").off("click", ".is-book-mark");
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
            $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
                ////////console.log('e.target=', e.target);
                self.loadGrid();
                //e.target // newly activated tab
                //e.relatedTarget // previous active tab
            });
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
