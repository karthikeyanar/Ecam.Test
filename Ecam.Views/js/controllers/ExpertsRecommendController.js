"use strict";
define("ExpertsRecommendController", ["knockout", "komapping", "../models/GridModel", "../models/ExpertsRecommendModel"
    , "service", "helper", "tokenfield"]
    , function (ko, komapping, GridModel, ExpertsRecommendModel, service, helper, tokenfield) {
        return function () {
            var self = this;
            this.template = "/Home/ExpertsRecommend";

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
            this.addExpertsRecommend = function (id, onSave) {
                self.openModal({
                    "name": "EditExpertsRecommend"
                    , "url": "/Home/AddExpertsRecommend"
                    , "title": (id == 0 ? "Add Experts Recommend" : "Edit Experts Recommend")
                    , "is_modal_full": false
                    , "position": "top"
                }
                , function ($modal) {
                    var $target = $(".modal-body", $modal);
                    $target.addClass('no-padding');
                    var $editCnt = $(".edit-cnt", $modal);
                    var m = new ExpertsRecommendModel();
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
                    m.onBeforeSelectExpertsRecommend = function () {
                        handleBlockUI({
                            "target": $editCnt, verticalTop: true
                        });
                    }
                    m.onAfterSelectExpertsRecommend = function () {
                        unblockUI($editCnt);
                        self.applyModalPlugins($modal);
                        if (cInt(m.company_id()) > 0) {
                            $(":input[name='company_id']", $modal).select2Refresh("data", [{ id: m.company_id(), text: m.company_name() }]);
                        }
                        if (cInt(m.experts_id()) > 0) {
                            $(":input[name='experts_id']", $modal).select2Refresh("data", [{ id: m.experts_id(), text: m.experts_name() }]);
                        }
                    }
                    m.onRefreshGrid = function () {
                        self.loadExpertsRecommendGrid(self.grid(), $("#expertsExpertsRecommendTable"));
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
                        self.addExpertsRecommend(model.id(), function (em, json) {
                            var findRow = gridModel.findExpertsRecommend(json.id);
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
                var $experts_id = $(":input[name='experts_id']", $modal);
                var $company_id = $(":input[name='company_id']", $modal);
                select2Setup($experts_id[0], {
                    multiple: true
                   , maximumSelectionSize: 1
                   , url: apiUrl("/Experts/Select")
                   , placeholder: "Select Experts"
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
                select2Setup($company_id[0], {
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

            this.loadExpertsRecommendGrid = function (m, $target) {
                var $table = $("#expertsExpertsRecommendTable");
                if ($table[0]) { $target = $table; }
                handleBlockUI({ "target": $target });
                var arr = $("#frmExpertsRecommendSearch").serializeArray();
                arr[arr.length] = { "name": "pageIndex", "value": m.page_index() };
                arr[arr.length] = { "name": "pageSize", "value": m.rows_per_page() };
                arr[arr.length] = { "name": "sortName", "value": m.sort_name() };
                arr[arr.length] = { "name": "sortOrder", "value": m.sort_order() };
                var url = apiUrl("/ExpertsRecommend/List");
                $.ajax({
                    "url": url,
                    "cache": false,
                    "type": "GET",
                    "data": arr
                }).done(function (json) {
                    m.total_rows(json.total);
                    m.rows.removeAll();
                    $.each(json.rows, function (i, row) {
                        var rm = new ExpertsRecommendModel();
                        self.assignEditEvents(rm, m)
                        komapping.fromJS(row, {}, rm);
                        m.rows.push(rm);
                    });
                    if (m.refreshCallBack == null) {
                        m.refreshCallBack = function (m) {
                            self.loadExpertsRecommendGrid(m, $target);
                        }
                    }
                }).always(function () {
                    unblockUI($target);
                });
            }


            this.applyPlugins = function () {
            }

            this.loadExpertsRecommend = function (callback) {
                GridModel.prototype.findExpertsRecommend = function (id) {
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
                    self.addExpertsRecommend(0, function (group, json) {
                        komapping.fromJS(json, {}, group);
                        self.assignEditEvents(group, that);
                        that.rows.splice(0, 0, group);
                        that.total_rows(cInt(that.total_rows()) + 1);
                    })
                }
                self.grid(new GridModel())
                self.grid().sort_name("experts_name");
                var $target = $(".page-content");
                if (callback)
                    callback();
            }

            /* end group */

            this.init = function () {
                self.applyPlugins();
                self.loadExpertsRecommend(function () {
                    var $target = $(".page-content");
                    self.loadExpertsRecommendGrid(self.grid(), $target);
                });
                unblockUI();
            }
        }
    }
);
