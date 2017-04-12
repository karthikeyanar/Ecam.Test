"use strict";
define("MarketController", ["knockout", "komapping", "../models/GridModel", "../models/MarketModel"
    , "service", "helper", "tokenfield"]
    , function (ko, komapping, GridModel, MarketModel, service, helper, tokenfield) {
        return function () {
            var self = this;
            this.template = "/Home/Market";

            this.ls_key = "market";
            this.ls_JSON = { "start_date": "", "end_date": "" };
            this.start_date = ko.observable();
            this.end_date = ko.observable();

            /* modal setup */
            this.templates = [];
            this.getTemplate = function (name) {
                var template = "";
                $.each(self.templates, function (i, temp) {
                    if (temp.name == name) {
                        template = temp.template;
                    }
                });
                return template;
            }
            this.loadTemplate = function ($modal, data, callback) {
                var template = self.getTemplate(data.name);
                if (template == "") {
                    $.get(data.url, function (html) {
                        template = html;
                        self.templates.push({ "name": data.name, "template": template });
                        $(".modal-body", $modal).html(template);
                        if (callback) { callback(); }
                    });
                } else {
                    $(".modal-body", $modal).html(template);
                    if (callback) { callback(); }
                }
            }
            this.openModal = function (data, callback) {
                var $cnt = $(".modal-container");
                var modalID = "modal-" + data.name;
                $("#" + modalID, $cnt).remove();
                $("#modal-template").tmpl(data).appendTo($cnt);
                var $modal = $("#" + modalID, $cnt);
                var $body = $("body");
                handleBlockUI({
                    "target": $body
                });
                self.loadTemplate($modal, data, function () {
                    unblockUI($body);
                    if (callback) { callback($modal); }
                });
            }
            /* end modal setup */

            /* group */
            this.grid = ko.observable(null);
            this.addMarket = function (id, onSave) {
                self.openModal({
                    "name": "EditMarket"
                    , "url": "/Home/AddMarket"
                    , "title": (id == 0 ? "Add Market" : "Edit Market")
                    , "is_modal_full": false
                    , "position": "top"
                }
                , function ($modal) {
                    var $target = $(".modal-body", $modal);
                    $target.addClass('no-padding');
                    var $editCnt = $(".edit-cnt", $modal);
                    var m = new MarketModel();
                    m.is_edit_mode(true);
                    if (id == 0) {
                        m.is_edit_mode(false);
                    }
                    m.onBeforeSave = function (formElement) {
                        $("#save", formElement).button('loading');
                    }
                    m.onAfterSave = function (formElement) {
                        $("#save", formElement).button('reset');
                        if (m.errors() == null) {
                            $modal.modal('hide');
                        }
                    }
                    m.onBeforeSelectMarket = function () {
                        handleBlockUI({
                            "target": $editCnt, verticalTop: true
                        });
                    }
                    m.onAfterSelectMarket = function () {
                        unblockUI($editCnt);
                        self.applyModalPlugins($modal);
                        if (cString(m.symbol()) != '') {
                            $(":input[name='symbol']", $modal).select2Refresh("data", [{ id: m.symbol(), text: m.company_name() }]);
                        }
                    }
                    m.onRefreshGrid = function () {
                        self.loadMarketGrid(self.grid(), $("#marketTable"));
                    }
                    m.onSave = function (json) {
                        if (onSave) {
                            onSave(m, json);
                        }
                    }
                    m.onPasswordChange = function () {
                    }
                    ko.applyBindings(m, $target[0]);
                    $modal.modal('show');
                    if (id > 0) {
                        m.id(id);
                        m.load();
                        m.is_focus(true);
                    } else {
                        self.applyModalPlugins($modal);
                    }
                });
            }
            this.assignEditEvents = function (model, gridModel) {
                if (model.onEdit == null) {
                    model.onEdit = function () {
                        self.addMarket(model.id(), function (em, json) {
                            var findRow = gridModel.findMarket(json.id);
                            if (findRow != null) {
                                komapping.fromJS(json, {}, findRow);
                            }
                        });
                    }
                }
                if (model.onDelete == null) {
                    model.onDelete = function () {
                        gridModel.rows.remove(this);
                        gridModel.total_rows(cInt(gridModel.total_rows()) - 1);
                    }
                }
            }


            this.applyModalPlugins = function ($modal) {
                var $symbol = $(":input[name='symbol']", $modal);
                select2Setup($symbol[0], {
                    multiple: true
                   , maximumSelectionSize: 1
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
                           term: term
                       };
                   }
                   , onChange: function (e, ui) {
                   }
                });
            }

            this.loadMarketGrid = function (m, $target) {
                var $table = $("#marketTable");
                if ($table[0]) { $target = $table; }
                handleBlockUI({ "target": $target });
                var arr = $("#frmMarketSearch").serializeArray();
                arr[arr.length] = { "name": "pageIndex", "value": m.page_index() };
                arr[arr.length] = { "name": "pageSize", "value": m.rows_per_page() };
                arr[arr.length] = { "name": "sortName", "value": m.sort_name() };
                arr[arr.length] = { "name": "sortOrder", "value": m.sort_order() };
                var url = apiUrl("/Market/List");
                $.ajax({
                    "url": url,
                    "cache": false,
                    "type": "GET",
                    "data": arr
                }).done(function (json) {
                    m.total_rows(json.total);
                    m.rows(json.rows); //.removeAll();
                    //$.each(json.rows, function (i, row) {
                    //    var rm = new MarketModel();
                    //    self.assignEditEvents(rm, m)
                    //    komapping.fromJS(row, {}, rm);
                    //    m.rows.push(rm);
                    //});
                    if (m.refreshCallBack == null) {
                        m.refreshCallBack = function (m) {
                            self.loadMarketGrid(m, $target);
                        }
                    }
                }).always(function () {
                    unblockUI($target);
                });
            }


            this.applyPlugins = function () {
                var $frmCompanySearch = $("#frmMarketSearch");
                var $symbols = $(":input[name='symbols']", $frmCompanySearch);
                var $categories = $(":input[name='categories']", $frmCompanySearch);
                select2Setup($symbols[0], {
                    multiple: true
                    , width: 400
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
                       self.loadMarketGrid(self.grid(), $target);
                   }
                });

                select2Setup($categories[0], {
                    multiple: true
                   , width: 400
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
                       self.loadMarketGrid(self.grid(), $target);
                   }
                });

                var lsJSON = getLS(self.ls_key);
                var startDate = '';
                var endDate = '';
                if (lsJSON != null) {
                    startDate = cDateString(lsJSON.start_date);
                    endDate = cDateString(lsJSON.end_date);
                }

                var $pageContent = $(".page-content");
                var $reportRange = $('#reportrange', $pageContent);
                //var arrlastsixmonths = helper.getLastSixMonths();
                if (startDate == "") {
                    startDate = moment(_TODAYDATE).startOf('week').format("YYYY-MM-DD");
                }
                if (endDate == "") {
                    endDate = moment(_TODAYDATE).endOf('week').format("YYYY-MM-DD");
                }
                //if(startDate=="") {
                //    startDate=arrlastsixmonths[0];
                //}
                //if(endDate=="") {
                //    endDate=arrlastsixmonths[1];
                //}
                var start = moment(startDate);
                var end = moment(endDate);

                console.log('startDate=', startDate, 'endDate=', endDate);

                helper.handleDateRangePicker($reportRange, {
                    'opens': 'right',
                    'start': start,
                    'end': end,
                    'changeDate': function (start, end) {
                        var daysDiff = helper.getTimeDiff(start.format('MM/DD/YYYY'), end.format('MM/DD/YYYY')).days;
                        //if(daysDiff>365) {
                        //    jAlert("Please select <= 365 days.");
                        //} else {
                        self.start_date(start.format('MM/DD/YYYY'));
                        self.end_date(end.format('MM/DD/YYYY'));
                        self.ls_JSON.start_date = self.start_date();
                        self.ls_JSON.end_date = self.end_date();
                        setLS(self.ls_key, self.ls_JSON);
                        helper.changeDateRangeLabel($('span', $reportRange), start, end, self.start_date(), self.end_date());
                        var $target = $(".page-content");
                        self.loadMarketGrid(self.grid(), $target);
                        //}
                    }
                });

                self.start_date(start.format('MM/DD/YYYY'));
                self.end_date(end.format('MM/DD/YYYY'));

                helper.changeDateRangeLabel($('span', $reportRange), start, end, self.start_date(), self.end_date());
            }

            this.loadMarket = function (callback) {
                GridModel.prototype.findMarket = function (id) {
                    var findRow = null;
                    $.each(this.rows(), function (i, row) {
                        if (row.id() == id) {
                            findRow = row;
                        }
                    });
                    return findRow;
                }
                GridModel.prototype.add = function () {
                    var that = self.grid();
                    self.addMarket(0, function (group, json) {
                        komapping.fromJS(json, {}, group);
                        self.assignEditEvents(group, that);
                        that.rows.splice(0, 0, group);
                        that.total_rows(cInt(that.total_rows()) + 1);
                    })
                }
                self.grid(new GridModel())
                self.grid().sort_name("trade_date");
                self.grid().sort_order("desc");
                var $target = $(".page-content");
                if (callback)
                    callback();
            }

            /* end group */

            this.init = function () {
                self.loadMarket(function () {
                    self.applyPlugins();
                    var $target = $(".page-content");
                    self.loadMarketGrid(self.grid(), $target);
                });
                unblockUI();
            }
        }
    }
);
