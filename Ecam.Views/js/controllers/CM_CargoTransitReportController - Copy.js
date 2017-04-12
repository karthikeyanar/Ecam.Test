"use strict";
define("CM_CargoTransitReportController", ["knockout", "komapping", "../models/GridModel", "helper", "service"], function (ko, komapping, GridModel, helper, service) {
    return function () {
        var self = this;
        this.template = "/CM/CargoTransitReports";

        this.summary_grid = ko.observable(null);
        this.detail_grid = ko.observable(null);
        this.is_show_detail = ko.observable(false);
        this.group_key_name = ko.observable("AWBNO");
        this.report_mode = ko.observable("S");
        this.child_row_index = ko.observable();
        this.current_row_key = ko.observable();
        this.current_row_value = ko.observable();
        this.ls_key = "cm-cargo-transit-report-" + getCurrentUserID();
        this.ls_JSON = { "awb_no": "", "start_date": "", "end_date": "", "flight_nos": "", "airline_codes": "" };

        this.start_date = ko.observable("");
        this.end_date = ko.observable("");
        this.is_append_summary = false;

        this.is_append_detail = false;

        this.changeReportMode = function (mode) {
            self.report_mode(mode);
        }


        this.filterRefreshSummary = function () {
            var $summaryTableCnt = $(".summary-table-cnt");
            self.hideDetail();
            $summaryTableCnt.off("scroll").unbind('scroll');
            $summaryTableCnt.animate({ scrollTop: 0 }, 0);
            self.refreshSummaryGrid(function () {
                $summaryTableCnt.off("scroll").unbind('scroll');
                $summaryTableCnt.animate({ scrollTop: 0 }, 0);
                self.applyInfinityScroll();
            });
        }


        this.applyPlugins = function (callback) {
            self.summary_grid(new GridModel())
            self.summary_grid().sort_name("key_value");
            self.summary_grid().sort_order("asc");
            self.summary_grid().rows_per_page(self.getSummaryPageSize());

            var $body = $("body");
            var $header = $(".header");
            var $footer = $(".footer");
            var $menuBar = $(".menu-bar > .navbar");
            var $pageContent = $(".page-content");
            var $pageHeader = $(".page-header");
            var $summaryTableCnt = $(".summary-table-cnt");
            var $summaryTable = $("#transitSummaryTable");
            var $detailGridBox = $(".detail-grid-box");
            var $detailTableCnt = $(".detail-table-cnt");
            var $detailTable = $("#detailTable", $detailTableCnt);
            var $groupKeyList = $("#groupkeylist");

            $("a", $groupKeyList).click(function () {
                self.hideDetail();
                $summaryTableCnt.off("scroll").unbind('scroll');
                $summaryTableCnt.animate({ scrollTop: 0 }, 0);

                self.is_append_summary = false;
                self.summary_grid().is_manual_refresh = true;
                var $this = $(this);
                $this.siblings().removeClass("active");
                $this.addClass("active");
                var keyname = $this.attr("group_key");
                if (keyname == "Date" && self.summary_grid().sort_name() != "flight_date") {
                    self.summary_grid().sort_name("flight_date");
                    self.summary_grid().sort_order("desc");
                } else if (self.summary_grid().sort_name() == "flight_date") {
                    self.summary_grid().sort_name("key_value");
                    self.summary_grid().sort_order("asc");
                }
                self.group_key_name(keyname);
                $(".g-key-col-heading").attr("displayname", keyname);
                $("div", ".g-key-col-heading").html(keyname);
                self.refreshSummaryGrid(function () {
                    $summaryTableCnt.off("scroll").unbind('scroll');
                    $summaryTableCnt.animate({ scrollTop: 0 }, 0);
                    self.applyInfinityScroll();
                });
            });

            //Get data from local storage
            var lsJSON = getLS(self.ls_key);
            var awbNo = "";
            var startDate = "";
            var endDate = "";
            var airlineCodes = "";
            var flightNos = "";
            if (lsJSON != null) {
                awbNo = cString(lsJSON.awb_no);
                startDate = cDateString(lsJSON.start_date);
                endDate = cDateString(lsJSON.end_date);
                airlineCodes = cString(lsJSON.airline_codes);
                flightNos = cString(lsJSON.flight_nos);
            }

            var $frm = $("#frmTransitReportSearch");
            $(":input[name='awb_no']", $frm).val(awbNo);
            var $pageContent = $(".page-content");
            var $input = $(":input[name='airline_codes']", $pageContent);
            var $frm = $("#frmTransitReportSearch");
            var $airlineCodes = $(":input[name='airline_codes']", $frm);
            var $flightNos = $(":input[name='flight_nos']", $frm);
            var $agentIds = $(":input[name='agent_ids']", $frm);
            var $originAirportCodes = $(":input[name='origin_codes']", $frm);
            var $destAirportCodes = $(":input[name='dest_codes']", $frm);

            $(":input[name='cargo_status']", $frm).unbind('change').change(function () {
                self.filterRefreshSummary();
            });

            select2Setup($originAirportCodes[0], {
                multiple: true
                , maximumSelectionSize: 1
                , url: apiUrl("/Airport/Select")
                , placeholder: "Select Origin"
                , resultsCallBack: function (data, page) {
                    var s2data = [];
                    $.each(data, function (i, d) {
                        s2data.push({ "id": d.id, "text": d.label, "source": d });
                    });
                    return { results: s2data };
                }
                , onChange: function (e) {
                    //self.loadFlightSearch();                    
                }
            });

            select2Setup($destAirportCodes[0], {
                multiple: true
               , maximumSelectionSize: 1
               , url: apiUrl("/Airport/Select")
               , placeholder: "Select Destination"
               , resultsCallBack: function (data, page) {
                   var s2data = [];
                   $.each(data, function (i, d) {
                       s2data.push({ "id": d.id, "text": d.label, "source": d });
                   });
                   return { results: s2data };
               }
               , onChange: function (e) {
                   //self.loadFlightSearch();
               }
            });

            select2Setup($agentIds[0], {
                multiple: true
               , width: '250px'
               , url: apiUrl("/Agent/Select")
               , placeholder: "Select Agent"
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
                       companyIds: getGlobalCompanyID().toString()
                   };
               }
               , onChange: function (e, ui) {
                   self.filterRefreshSummary();
               }
            });

            select2Setup($flightNos[0], {
                multiple: true
                 , width: '150px'
                 , url: apiUrl("/Flight/Select")
                 , placeholder: "Select Flight"
                 , onParam: function (term, page) {
                     return {
                         term: term,
                         airlineCodes: $(":input[name='airline_codes']").val(),
                         companyIds: getGlobalCompanyID().toString()
                     };
                 }
                 , resultsCallBack: function (data, page) {
                     var s2data = [];
                     $.each(data, function (i, d) {
                         s2data.push({ "id": d.id, "text": d.label, "source": d });
                     });
                     return { results: s2data };
                 }
                 , onChange: function (e) {
                     //set left Agent option panel active                     
                     //$(".list-group-item").each(function(index,element) {
                     //    if(element.getAttribute("group_key")=="Flight") {
                     //        $(element).addClass("list-group-item active");
                     //    } else {
                     //        $(element).removeClass("active");
                     //    }
                     //});
                     self.filterRefreshSummary();
                 }
            });

            select2Setup($airlineCodes[0], {
                multiple: true
                , is_img_airline: "true"
                , width: '1024px'
				, logoLimit: 9
                , url: apiUrl("/Airline/Select")
                , placeholder: "Select Airline"
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
                        companyIds: self.getSelectCompanyIds()
                    };
                }
                , formatResult: showLogo
                , formatSelection: showLogo
                , onChange: function (e, ui) {
                    self.filterRefreshSummary();
                }
            });

            self.loadLSAirlines($airlineCodes, airlineCodes, function () {
                self.loadLSFlights($flightNos, flightNos, function () {
                    // Below code calls again the summaryGrid()
                    //if(callback)
                    //callback();
                });
            });

            var arrfndates = helper.getFortNightDates();
            var fnStartDate = arrfndates[0];
            var fnEndDate = arrfndates[1];
            var lastFnStartDate = arrfndates[2];
            var lastFnEndDate = arrfndates[3];

            var start = moment(fnStartDate);
            var end = moment(fnEndDate);
            if (startDate != "") {
                start = moment(startDate);
            }
            if (endDate != "") {
                end = moment(endDate);
            }

            var $reportRange = $('#reportrange', $pageContent);

            helper.handleDateRangePicker($reportRange, {
                'opens': 'right',
                'start': start,
                'end': end,
                'changeDate': function (start, end) {
                    var daysDiff = helper.getTimeDiff(start.format('MM/DD/YYYY'), end.format('MM/DD/YYYY')).days;
                    if (daysDiff > 365) {
                        jAlert("Please select <= 365 days.");
                    } else {
                        self.start_date(start.format('MM/DD/YYYY'));
                        self.end_date(end.format('MM/DD/YYYY'));
                        helper.changeDateRangeLabel($('span', $reportRange), start, end, self.start_date(), self.end_date());
                        self.refreshSummaryGrid(function () {
                        });
                    }
                }
            });

            var start = moment(fnStartDate);//.startOf('week');
            var end = moment(fnEndDate);//.endOf('week');

            if (startDate != "") {
                self.start_date(startDate);
                start = moment(startDate);
            }
            if (endDate != "") {
                self.end_date(endDate);
                end = moment(endDate);
            }

            $reportRange.data('daterangepicker').setStartDate(start.format('MM/DD/YYYY'));
            $reportRange.data('daterangepicker').setEndDate(end.format('MM/DD/YYYY'));

            self.start_date(start.format('MM/DD/YYYY'));
            self.end_date(end.format('MM/DD/YYYY'));

            helper.changeDateRangeLabel($('span', $reportRange), start, end, self.start_date(), self.end_date());

            self.detail_grid(new GridModel())
            self.detail_grid().page_index(1);
            self.detail_grid().rows_per_page(10);
            self.detail_grid().sort_name("agent_name");
            self.detail_grid().sort_order("asc");

            var $detailGridCnt = $("<div class='additional-cnt'></div>");

            //$("#detailGridViewTemplate").tmpl({}).appendTo($detailGridCnt);


            $(".page-container").after($detailGridCnt);

            ko.applyBindings(self, $detailGridCnt[0]);

            helper.sortingTable($summaryTable[0], {
                'onSorting': function (sortname, sortorder) {
                    self.is_append_summary = false;
                    self.summary_grid().is_manual_refresh = true;
                    self.summary_grid().sort_name(sortname);
                    self.summary_grid().sort_order(sortorder);
                    self.summary_grid().is_manual_refresh = false;
                    self.refreshSummaryGrid(function () {
                    });
                }
            });
            self.is_show_detail(false);
            self.changeLayout();
            if (helper.isIE() == false) {
                $summaryTable.floatThead({
                    scrollContainer: function ($t) {
                        return $t.closest('.summary-table-cnt');
                    }
                });
            }
            if (callback)
                callback();
        }

        this.getSummaryScrollPageSize = function () {
            return 10;
        }

        this.getSummaryPageSize = function () {
            return 10;
        }

        this.refreshSummaryGrid = function (callback) {
            self.is_append_summary = false;
            self.summary_grid().is_manual_refresh = true;
            self.summary_grid().page_index(1);
            self.summary_grid().rows_per_page(self.getSummaryPageSize());
            self.loadSummaryGrid(function () {
                if (callback)
                    callback();
            });
        }

        this.showMoreSummary = function () {
            self.summary_grid().is_manual_refresh = true;
            self.is_append_summary = true;
            self.summary_grid().page_index(self.summary_grid().page_index() + 1);
            if (self.summary_grid().page_index() <= self.summary_grid().total_pages()) {
                self.loadSummaryGrid();
            }
        }

        this.applyInfinityScroll = function () {
            var $summaryTableCnt = $(".summary-table-cnt");
            self.summary_grid().is_manual_refresh = true;
            var calcPageIndex = 0;
            var newPageSize = self.getSummaryScrollPageSize();
            if (self.summary_grid().rows_per_page() > newPageSize) {
                calcPageIndex = (self.summary_grid().rows_per_page() / newPageSize);
                self.summary_grid().page_index(calcPageIndex);
                self.summary_grid().rows_per_page(newPageSize);
            }
            self.summary_grid().is_manual_refresh = false;
        }

        this.getSelectCompanyIds = function () {
            return cInt(getGlobalCompanyID()).toString();
        }

        this.loadLSAirlines = function ($airlineCodes, ids, callback) {
            //if(ids!="") {
            //    $airlineCodes.val(ids);
            //    getAirlines(ids,function(json) {
            //        $airlineCodes.select2Refresh("data",json);
            //        if(callback)
            //            callback();
            //    });
            //} else {
            //    if(callback)
            //        callback();
            //}
        }

        this.loadLSFlights = function ($flightNos, ids, callback) {
            if (ids != "") {
                $flightNos.val(ids);
                getFlights({
                    "flight_nos": ids.split(','),
                    "onComplete": function (flights) {
                        $flightNos.select2Refresh("data", flights);
                        if (callback)
                            callback();
                    }
                });
            } else {
                if (callback)
                    callback();
            }
        }

        this.getSummaryData = function (callback, isCheckCompare) {
            var $pageContent = $(".page-content");
            var $target = $(".summary-table-cnt");
            var $table = $("#transitSummaryTable");
            var $frm = $("#frmTransitReportSearch", $pageContent);
            var arr = $frm.serializeArray();

            arr[arr.length] = { "name": "pageIndex", "value": self.summary_grid().page_index() };
            arr[arr.length] = { "name": "pageSize", "value": self.summary_grid().rows_per_page() };
            arr[arr.length] = { "name": "sortName", "value": "awb_no" };
            arr[arr.length] = { "name": "sortOrder", "value": self.summary_grid().sort_order() };
            arr[arr.length] = { "name": "group_key", "value": self.group_key_name().toString().toLowerCase() };
            arr[arr.length] = { "name": "company_ids", "value": self.getSelectCompanyIds() };
            //arr[arr.length]={ "name": "airline_codes","value": $(":input[name='airline_codes']",$pageContent).val() };
            arr[arr.length] = { "name": "currency_codes", "value": $(":input[name='currency_codes']", $pageContent).val() };
            var startDate = self.start_date();
            var endDate = self.end_date();
            var diff = helper.getTimeDiff(new Date(startDate), new Date(endDate));
         
            //arr[arr.length]={ "name": "start_date","value": startDate };
            //arr[arr.length]={ "name": "end_date","value": endDate };
            var url = apiUrl("/FlightBook/TransitSummary");
            $.ajax({
                "url": url,
                "cache": false,
                "type": "GET",
                "data": arr
            }).done(function (json) {
                $.each(arr, function (i, p) {
                    switch (p.name) {
                        case "awb_no":
                            self.ls_JSON.awb_no = p.value;
                            break;
                        case "airline_codes":
                            self.ls_JSON.airline_codes = p.value
                            break;
                        case "flight_nos":
                            self.ls_JSON.flight_nos = p.value;
                            break;
                        case "start_date":
                            self.ls_JSON.start_date = p.value;
                            break;
                        case "end_date":
                            self.ls_JSON.end_date = p.value;
                            break;
                    }
                });

                setLS(self.ls_key, self.ls_JSON);

                if (callback)
                    callback(json);
            }).always(function () {
                //unblockUI($target);
            });
        }

        this.applySummaryGridSetup = function (json) {
            var $pageContent = $(".page-content");
            var $target = $(".summary-table-cnt");
            var $table = $("#transitSummaryTable");
            var $tbody = $("#tbody_transitReportTable");
            if (self.is_append_summary == false)
                $tbody.empty();

            $("#summarytemplate").tmpl(json).appendTo($tbody);
            self.summary_grid().total_rows(json.total);

            //if (json.rows.length == 1 && self.group_key_name() == "AWBNO") {
            //    var $td = $("#transitSummaryTable > tbody > tr > td.cls-awn-no:first");
            //    $("#transitSummaryTable > tbody > tr > td.cls-awn-no:first").click(function () {
            //        var $tr = $("#transitSummaryTable > tbody > tr");
            //        var rowIndex = $tr.attr("row-index");
            //        var rowKey = $tr.attr("row-key");
            //        var rowValue = $tr.attr("row-value");
            //        var childTRId = "child_" + rowIndex;
            //        var $treeExpand = $(".tree-expand", $tr);
            //        var $childTR = $("#" + childTRId, $tbody);
            //        if (!$childTR[0]) {
            //            $treeExpand.addClass("ex-minus");
            //            $childTR = $("<tr><td colspan='4' class='child-row-cnt'></td></tr>");
            //            $childTR.attr("id", childTRId).attr("row-key", rowKey);
            //            $childTR.attr("id", childTRId).attr("row-value", rowValue);
            //            var $childTD = $("td", $childTR);
            //            $("#groupChildTabletemplate").tmpl(null).appendTo($childTD);
            //            $tr.after($childTR);
            //            self.loadChildRows($childTR);
            //        }
            //    });

            //    $(".cls-awn-no").click();
            //}

            if (self.summary_grid().refreshCallBack == null) {
                self.summary_grid().refreshCallBack = function (m) {
                    self.loadSummaryGrid();
                }
            }

            $("tr", $tbody).each(function () {
                var $tr = $(this);
                var rowIndex = $tr.attr("row-index");
                var rowKey = $tr.attr("row-key");
                var rowValue = $tr.attr("row-value");
                if ($tr.attr("is-apply") != "true") {
                    $tr.attr("is-apply", "true").click(function () {
                        self.detail_grid().rows_per_page(10);
                        var childTRId = "child_" + rowIndex;
                        self.child_row_index(rowIndex);
                        var $childTR = $("#" + childTRId, $tbody);
                        var $treeExpand = $(".tree-expand", this);
                        if ($treeExpand.hasClass("ex-plus")) {
                            $treeExpand.removeClass("ex-plus");
                            if (!$childTR[0]) {
                                $treeExpand.addClass("ex-minus");
                                ///$childTR = $("<tr><td colspan='4' class='child-row-cnt'></td></tr>");
                                $childTR = $("<tr><td colspan='7' class='child-row-cnt'></td></tr>");
                                $childTR.attr("id", childTRId).attr("row-key", rowKey);
                                $childTR.attr("id", childTRId).attr("row-value", rowValue);
                                var $childTD = $("td", $childTR);
                                $("#groupChildTabletemplate").tmpl(null).appendTo($childTD);
                                $tr.after($childTR);
                                self.loadChildRows($childTR);
                            } else {
                                $childTR.removeClass("hide");
                                $treeExpand.addClass("ex-minus");
                            }
                        } else {
                            $childTR.addClass("hide");
                            $treeExpand.removeClass("ex-minus").addClass("ex-plus");
                        }
                    });
                }
            });
            unblockUI($target);
            self.is_append_summary = false;
            self.summary_grid().is_manual_refresh = false;
        }


        this.loadSummaryGrid = function (callback) {
            var $pageContent = $(".page-content");
            var $target = $(".summary-table-cnt");
            var $table = $("#transitSummaryTable");
            handleBlockUI({ "target": $target });
            self.getSummaryData(function (json) {
                self.applySummaryGridSetup(json);
            }, false);
        }

        this.changeLayout = function () {
            var $body = $("body");
            var $header = $(".header");
            var $footer = $(".footer");
            var $menuBar = $(".menu-bar > .navbar");
            var $pageContent = $(".page-content");
            var $pageHeader = $(".page-header");
            var $summaryTableCnt = $(".summary-table-cnt");
            var $summaryTable = $("#transitSummaryTable");
            var $detailGridBox = $(".detail-grid-box");
            var $detailTableCnt = $(".detail-table-cnt");
            var $detailTable = $("#detailTable", $detailTableCnt);
            var $filterBox = $(".filter-box");
            //$filterBox.slimScroll({ destroy: true });
            //$filterBox.css({ 'height': '','overflow': '','width': '' });
            if (self.is_show_detail() == true) {
                if ($body.hasClass("detail-mode") == false) {
                    $summaryTableCnt.animate({ scrollTop: 0 }, 0);
                    var $selectRow = $(".select-row");
                    $summaryTableCnt.animate({ scrollTop: $selectRow.position().top - 50 }, 0);
                }
                $body.addClass("detail-mode");
                var windowHeight = $(window).height() - $header.outerHeight(true) - $menuBar.outerHeight(true) - $pageHeader.outerHeight(true);
                var detailHeight = windowHeight / 2;
                $detailTableCnt.height(detailHeight);
                $(".filter-box").css("bottom", detailHeight + 50 + "px");
                var summaryHeight = windowHeight - detailHeight - 70;
                $summaryTableCnt.height(summaryHeight);
                //$filterBox.height(summaryHeight);

                //$filterBox.slimScroll({
                // width: 'auto',
                //  height: summaryHeight+'px',
                //  size: '5px',
                // alwaysVisible: true
                //});

                $detailTableCnt.animate({ scrollTop: 0 }, 0);
                helper.destroyFixedHeader($detailTable);
                helper.sortingTable($detailTable[0], {
                    'onSorting': function (sortname, sortorder) {
                        self.detail_grid().is_manual_refresh = true;
                        self.detail_grid().page_index(1);
                        self.detail_grid().sort_name(sortname);
                        self.detail_grid().sort_order(sortorder);
                        self.detail_grid().is_manual_refresh = false;
                        self.is_append_detail = false;
                        self.loadDetail();
                    }
                });
                if (helper.isIE() == false) {
                    $detailTable.floatThead({
                        scrollContainer: function ($t) {
                            return $t.closest('.detail-table-cnt');
                        }
                    });
                }
            } else {
                $body.removeClass("detail-mode");
                $(".filter-box").css("bottom", "");
                var windowHeight = $(window).height() - $header.outerHeight(true) - $menuBar.outerHeight(true) - $pageHeader.outerHeight(true) - $footer.outerHeight(true) - 60;
                $summaryTableCnt.height(windowHeight);
                //$filterBox.height(windowHeight);
                //$filterBox.slimScroll({
                //    width: 'auto',
                //    height: summaryHeight+'px',
                //    size: '5px',
                //    alwaysVisible: true
                //});
            }
        }

        this.applyInfinityDetailScroll = function () {
            var $detailTableCnt = $(".detail-table-cnt");
            $detailTableCnt.off("scroll").unbind("scroll");
            $detailTableCnt.animate({ scrollTop: 0 }, 0);
            $detailTableCnt.paged_scroll({
                handleScroll: function (page, container, doneCallback) {
                    setTimeout(function () {
                        self.detail_grid().is_manual_refresh = true;
                        self.is_append_detail = true;
                        self.detail_grid().page_index(self.detail_grid().page_index() + 1);
                        if (self.detail_grid().page_index() <= self.detail_grid().total_pages()) {
                            self.loadDetail(function () {
                                doneCallback();
                            });
                        } else {
                            doneCallback();
                        }
                    }, 2000);
                },
                pagesToScroll: self.detail_grid().total_pages() - 1,
                triggerFromBottom: '50',
                loader: '<div class="loader"></div>',
                //debug: true,
                targetElement: $detailTableCnt,
                monitorTargetChange: false
            });
        }

        this.current_json = ko.observable();
        this.loadChildRows = function ($tr, arr) {
            var $pageContent = $(".page-content");
            var $summaryTableCnt = $(".summary-table-cnt");
            var groupKey = self.group_key_name().toString().toLowerCase();

            if (typeof $tr == "undefined") {
                var rowkey = self.current_row_key();
                var rowValue = self.current_row_value();
                $tr = $("#child_" + self.child_row_index(), "#tbody_transitReportTable");
            } else {
                var rowKey = $tr.attr("row-key");
                var rowValue = $tr.attr("row-value");
                self.current_row_key(rowKey);
                self.current_row_value(rowValue);
            }

            var arr = new Array();
            switch (groupKey) {
                case "awbno":
                    arr[arr.length] = { "name": "awb_no", "value": rowKey };
                    break;
                case "flight":
                    arr[arr.length] = { "name": "flight_no", "value": rowValue };
                    break;
                case "airline":
                    arr[arr.length] = { "name": "airline_name", "value": rowKey };
                    arr[arr.length] = { "name": "airline_code", "value": rowValue };
                    break;
                case "agent":
                    arr[arr.length] = { "name": "agent_name", "value": rowKey };
                    break;
                case "origin":
                    arr[arr.length] = { "name": "origin", "value": rowKey };
                    arr[arr.length] = { "name": "origin_code", "value": rowValue };
                    break;
                case "destination":
                    arr[arr.length] = { "name": "dest_code", "value": rowValue };
                    break;
            }


            //arr[arr.length] = { "name": "pageIndex", "value": 1 };
            //arr[arr.length] = { "name": "pageSize", "value": 10 };
            arr[arr.length] = { "name": "pageIndex", "value": self.detail_grid().page_index() };
            arr[arr.length] = { "name": "pageSize", "value": self.detail_grid().rows_per_page() };
            arr[arr.length] = { "name": "sortName", "value": "fb.flight_date" };
            arr[arr.length] = { "name": "sortOrder", "value": "desc" };
            arr[arr.length] = { "name": "group_key", "value": groupKey };
            arr[arr.length] = { "name": "company_ids", "value": self.getSelectCompanyIds() };
            arr[arr.length] = { "name": "airline_codes", "value": $(":input[name='airline_codes']", $pageContent).val() };
            arr[arr.length] = { "name": "start_date", "value": self.start_date() };
            arr[arr.length] = { "name": "end_date", "value": self.end_date() };
            arr[arr.length] = { "name": "cargo_status", "value": $("#cargo_status", "#frmTransitReportSearch").val() };

            var url = apiUrl("/FlightBook/TransitGroup");
            $.ajax({
                "url": url,
                "cache": false,
                "type": "GET",
                "data": arr
            }).done(function (json) {
                var $tbody = $("tbody", $tr);
                $tbody.empty();
                self.current_json(json);

                if (json != null) {
                    var arr = [];
                    var tempJson = []; //collect unique json data
                    $.each(json.rows, function (index, value) {
                        if ($.inArray(value.awb_no, arr) == -1) {
                            arr.push(value.awb_no);
                            tempJson.push(value);
                        }
                    });

                    var tempArr = new Array();
                    $.each(tempJson, function (i, res) {
                        $.map(json.rows, function (obj) {
                            if (obj.awb_no == res.awb_no) {
                                tempArr.push(obj);
                            }
                        });
                    });

                    if (groupKey == 'flight') {
                        var newTempArr = new Array();
                        $.each(tempArr, function (index, value) {
                            newTempArr.push(value);
                        })

                        var flightMultiObj = {
                            "groups": newTempArr
                        };

                        $("#flightRowtemplate").tmpl(flightMultiObj).appendTo($tbody);
                    }
                    if (groupKey == 'airline') {
                        self.loadFlightGroup(json, $tr);
                    }

                    if (groupKey == 'agent' || groupKey == "origin" || groupKey == "destination" || groupKey == "awbno") {
                        self.loadAirlineGroup($tr);
                    }

                    var current_limit = self.detail_grid().page_index() * self.detail_grid().rows_per_page();
                    if (json.total > current_limit) {
                        $("#loadmoreRowtemplate").tmpl().appendTo($tbody);
                    }
                }

            }).always(function () {
            });
        }

        this.loadAirlineGroup = function ($tr) {
            var $tbody = $("tbody", $tr);
            $tbody.empty();
            var json = self.current_json();

            var newArr = new Array();
            $.each(json.rows, function (index, value) {
                if ($.inArray(value.airline_code, newArr) == -1) {
                    newArr.push(value.airline_code);
                }
            })

            var newSummaryObj = new Array();

            $.each(newArr, function (index, value) {
                var sumgrwt = 0;
                var sumchwt = 0;
                var summc = 0;
                var shipments = 0;
                var origin_code, dest_code, airline_code = '';

                $.each(json.rows, function (j, obj) {
                    if (obj.airline_code == value) {
                        //console.log(obj);
                        shipments++;
                        sumgrwt = parseInt(sumgrwt) + parseInt(obj.grwt);
                        sumchwt = parseInt(sumchwt) + parseInt(obj.chwt);
                        summc = parseInt(summc) + parseInt(obj.mc);
                        origin_code = obj.origin_code;
                        dest_code = obj.dest_code;
                        airline_code = obj.airline_code;
                    }
                })
                var newObj = {
                    "i": index,
                    "key_value": airline_code,
                    "airline_code": airline_code,
                    "origin_code": origin_code,
                    "dest_code": dest_code,
                    "shipments": shipments,
                    "sum_grwt": sumgrwt,
                    "sum_chwt": sumchwt,
                    "sum_mc": summc
                }

                //console.log(newObj);
                newSummaryObj.push(newObj);
            });
            //console.log("newSummaryObj :",newSummaryObj);
            //this.applynewSummaryGrid(newSummaryObj, json, $tbody, 'airline');            
            this.applynewAirlineSummaryGrid(newSummaryObj, json, $tbody, 'airline');
        }

        this.loadFlightGroup = function (json, $tr) {
            var $tbody = $("tbody", $tr);
            $tbody.empty();

            var newArr = new Array();
            $.each(json.rows, function (index, value) {
                if ($.inArray(value.flight_no, newArr) == -1) {
                    newArr.push(value.flight_no);
                }
            })

            var newSummaryObj = new Array();

            $.each(newArr, function (index, value) {
                var sumgrwt = 0;
                var sumchwt = 0;
                var summc = 0;
                var shipments = 0;
                var origin_code, dest_code, flight_no = '';

                $.each(json.rows, function (j, obj) {
                    if (obj.flight_no == value) {
                        //console.log(obj);
                        shipments++;
                        sumgrwt = parseInt(sumgrwt) + parseInt(obj.grwt);
                        sumchwt = parseInt(sumchwt) + parseInt(obj.chwt);
                        summc = parseInt(summc) + parseInt(obj.mc);
                        origin_code = obj.origin_code;
                        dest_code = obj.dest_code;
                        flight_no = obj.flight_no;
                    }
                })
                var newObj = {
                    "i": index,
                    "key_value": flight_no,
                    "flight_no": flight_no,
                    "origin_code": origin_code,
                    "dest_code": dest_code,
                    "shipments": shipments,
                    "sum_grwt": sumgrwt,
                    "sum_chwt": sumchwt,
                    "sum_mc": summc
                }

                //console.log(newObj);
                newSummaryObj.push(newObj);
            });

            if ($tbody.length > 0) {
                this.applynewSummaryGrid(newSummaryObj, json, $tbody, 'flight');
            } else {
                this.applynewSummaryGrid(newSummaryObj, json, $tr, 'flight');
            }

        }

        //this.getFlightRows = function (json, airlineCode, agentId, originCode, destCode) {
        //}

        this.applynewSummaryGrid = function (newSummaryObj, json, $tbody, mode) {
            $("#newSummarytemplate").tmpl(newSummaryObj).appendTo($tbody);

            $("tr", $tbody).each(function () {
                var $tr = $(this);
                var rowIndex = $tr.attr("row-index");
                var rowKey = $tr.attr("row-key");
                var rowValue = $tr.attr("row-value");
                if ($tr.attr("is-apply") != "true") {
                    $tr.attr("is-apply", "true").click(function () {
                        self.detail_grid().rows_per_page(10);
                        var childTRId = "child_fli_" + rowIndex;
                        self.child_row_index(rowIndex);
                        var $childTR = $("#" + childTRId, $tbody);
                        var $treeExpand = $(".tree-expand", this);
                        if ($treeExpand.hasClass("ex-plus")) {
                            $treeExpand.removeClass("ex-plus");
                            if (!$childTR[0]) {
                                $treeExpand.addClass("ex-minus");
                                $childTR = $("<tr><td colspan='6' class='child-row-cnt'></td></tr>");
                                $childTR.attr("id", childTRId).attr("row-key", rowKey);
                                $childTR.attr("id", childTRId).attr("row-value", rowValue);
                                var $childTD = $("td", $childTR);
                                $("#groupChildTabletemplate").tmpl(null).appendTo($childTD);
                                $tr.after($childTR);
                                //$childTR.html("");                                
                                if (mode == 'airline') {
                                    self.loadFlightGroup(self.current_json(), $childTR);
                                } else {
                                    self.loadFlightRows($childTR, json, 'flight');
                                }

                                //self.loadChildRows($childTR);
                            } else {
                                $childTR.removeClass("hide");
                                $treeExpand.addClass("ex-minus");
                            }
                        } else {
                            $childTR.addClass("hide");
                            $treeExpand.removeClass("ex-minus").addClass("ex-plus");
                        }
                    });
                }
            });

        }

        this.applynewAirlineSummaryGrid = function (newSummaryObj, json, $tbody, mode) {

            $("#newAirlineSummarytemplate").tmpl(newSummaryObj).appendTo($tbody);

            $("tr", $tbody).each(function () {
                var $tr = $(this);
                var rowIndex = $tr.attr("row-index");
                var rowKey = $tr.attr("row-key");
                var rowValue = $tr.attr("row-value");
                if ($tr.attr("is-apply") != "true") {
                    $tr.attr("is-apply", "true").click(function () {
                        self.detail_grid().rows_per_page(10);
                        var childTRId = "child_air_" + rowIndex;
                        self.child_row_index(rowIndex);
                        var $childTR = $("#" + childTRId, $tbody);
                        var $treeExpand = $(".tree-expand", this);
                        if ($treeExpand.hasClass("ex-plus")) {
                            $treeExpand.removeClass("ex-plus");
                            if (!$childTR[0]) {
                                $treeExpand.addClass("ex-minus");
                                $childTR = $("<tr><td colspan='7' class='child-row-cnt'></td></tr>");
                                $childTR.attr("id", childTRId).attr("row-key", rowKey);
                                $childTR.attr("id", childTRId).attr("row-value", rowValue);
                                var $childTD = $("td", $childTR);
                                $("#groupChildTabletemplate").tmpl(null).appendTo($childTD);
                                $tr.after($childTR);
                                //$childTR.html("");								
                                var groupKey = self.group_key_name().toString().toLowerCase();
                                //if(mode=='airline' && (groupKey=="destination") && self.detail_grid().rows_per_page()<11) {                                     
                                if (mode == 'airline') {
                                    self.loadFlightGroup(self.current_json(), $childTR);
                                } else {
                                    self.loadFlightRows($childTR, json);
                                }

                                //self.loadChildRows($childTR);
                            } else {
                                $childTR.removeClass("hide");
                                $treeExpand.addClass("ex-minus");
                            }
                        } else {
                            $childTR.addClass("hide");
                            $treeExpand.removeClass("ex-minus").addClass("ex-plus");
                        }
                    });
                }
            });

        }

        this.loadFlightRows = function ($tr, json, mode) {
            var rowKey = $tr.attr("row-key");
            var singleFlightObj = new Array();
            var flight_no = rowKey;

            if (flight_no != "") {
                $.each(json.rows, function (j, obj) {
                    if (obj.flight_no == flight_no) {
                        singleFlightObj.push(obj);
                    }
                });
            }
            console.log("mode :" + mode);

            var flightMultiObj = {
                "mode": mode,
                "groups": singleFlightObj
            };
            var $childTD = $("td", $tr);
            $childTD.html("");

            //$("#newFlightRowtemplate").tmpl(flightMultiObj).appendTo($childTD);
            $("#flightRowtemplate").tmpl(flightMultiObj).appendTo($childTD);
        }
         
        this.hideDetail = function () {
            var $body = $("body");
            $body.removeClass("detail-mode");
            $(".filter-box").css("bottom", "");
            self.is_show_detail(false);
            self.changeLayout();
        }

        this.offElements = function () {
            $("body").off("click", ".cls-load-more");
            $("body").off("keypress", "#frmTransitReportSearch :input[name='awb_no']");
            $(window).unbind("resize");
        }

        this.onElements = function () {
            self.offElements();

            $("body").on("click", ".cls-load-more", function (event) {
                var current_page_index = self.detail_grid().rows_per_page();
                current_page_index += 10;
                //self.detail_grid().page_index(current_page_index);
                self.detail_grid().rows_per_page(current_page_index);
                self.loadChildRows();
            });
            $("body").on("keypress", "#frmTransitReportSearch :input[name='awb_no']", function (event) {
                if (event.keyCode == 13) {
                    self.filterRefreshSummary();
                }
            });
        }

        this.isInit = false;
        this.init = function (callback) {
            if (self.isInit == true) return;
            self.isInit = true;
            self.applyPlugins(function () {
                self.loadSummaryGrid();
                self.onElements();
                pushGCEvent(function (e, ui) {
                    self.filterRefreshSummary();
                });
                $(window).resize(function () {
                    self.changeLayout();
                });
                unblockUI();
                if (callback)
                    callback();
            });
        }
        this.unInit = function () {
            self.offElements();
        }
    }
}
);