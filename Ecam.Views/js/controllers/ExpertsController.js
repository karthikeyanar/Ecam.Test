"use strict";
define("ExpertsController", ["knockout", "komapping", "../models/GridModel", "../models/ExpertsModel"
    , "service", "helper", "tokenfield"]
    , function (ko, komapping, GridModel, ExpertsModel, service, helper, tokenfield) {
        return function () {
            var self = this;
            this.template = "/Home/Experts";

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
            this.addExperts = function (id, onSave) {
                self.openModal({
                    "name": "EditExperts"
                    , "url": "/Home/AddExperts"
                    , "title": (id == 0 ? "Add Experts" : "Edit Experts")
                    , "is_modal_full": false
                    , "position": "top"
                }
                , function ($modal) {
                    var $target = $(".modal-body", $modal);
                    $target.addClass('no-padding');
                    var $editCnt = $(".edit-cnt", $modal);
                    var m = new ExpertsModel();
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
                    m.onBeforeSelectExperts = function () {
                        handleBlockUI({
                            "target": $editCnt, verticalTop: true
                        });
                    }
                    m.onAfterSelectExperts = function () {
                        unblockUI($editCnt);
                        self.applyModalPlugins($modal);
                    }
                    m.onRefreshGrid = function () {
                        self.loadExpertsGrid(self.grid(), $("#expertsExpertsTable"));
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
                        self.addExperts(model.id(), function (em, json) {
                            var findRow = gridModel.findExperts(json.id);
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
            }

            this.loadExpertsGrid = function (m, $target) {
                var $table = $("#expertsExpertsTable");
                if ($table[0]) { $target = $table; }
                handleBlockUI({ "target": $target });
                var arr = $("#frmExpertsSearch").serializeArray();
                arr[arr.length] = { "name": "pageIndex", "value": m.page_index() };
                arr[arr.length] = { "name": "pageSize", "value": m.rows_per_page() };
                arr[arr.length] = { "name": "sortName", "value": m.sort_name() };
                arr[arr.length] = { "name": "sortOrder", "value": m.sort_order() };
                var url = apiUrl("/Experts/List");
                $.ajax({
                    "url": url,
                    "cache": false,
                    "type": "GET",
                    "data": arr
                }).done(function (json) {
                    m.total_rows(json.total);
                    m.rows.removeAll();
                    $.each(json.rows, function (i, row) {
                        var rm = new ExpertsModel();
                        self.assignEditEvents(rm, m)
                        komapping.fromJS(row, {}, rm);
                        m.rows.push(rm);
                    });
                    if (m.refreshCallBack == null) {
                        m.refreshCallBack = function (m) {
                            self.loadExpertsGrid(m, $target);
                        }
                    }
                }).always(function () {
                    unblockUI($target);
                });
            }


            this.applyPlugins = function () {
            }

            this.loadExperts = function (callback) {
                GridModel.prototype.findExperts = function (id) {
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
                    self.addExperts(0, function (group, json) {
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
                self.loadExperts(function () {
                    var $target = $(".page-content");
                    self.loadExpertsGrid(self.grid(), $target);
                });
                unblockUI();
            }
        }
    }
);
