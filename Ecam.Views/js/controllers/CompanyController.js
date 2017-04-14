"use strict";
define("CompanyController", ["knockout", "komapping", "../models/GridModel", "../models/CompanyModel"
    , "service", "helper", "tokenfield"]
    , function (ko, komapping, GridModel, CompanyModel, service, helper, tokenfield) {
        return function () {
            var self = this;
            this.template = "/Home/Company";

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
            this.addCompany = function (id, onSave) {
                self.openModal({
                    "name": "EditCompany"
                    , "url": "/Home/AddCompany"
                    , "title": (id == 0 ? "Add Company" : "Edit Company")
                    , "is_modal_full": false
                    , "position": "top"
                }
                , function ($modal) {
                    var $target = $(".modal-body", $modal);
                    $target.addClass('no-padding');
                    var $editCnt = $(".edit-cnt", $modal);
                    var m = new CompanyModel();
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
                    m.onBeforeSelectCompany = function () {
                        handleBlockUI({
                            "target": $editCnt, verticalTop: true
                        });
                    }
                    m.onAfterSelectCompany = function () {
                        unblockUI($editCnt);
                        self.applyModalPlugins($modal);
                    }
                    m.onRefreshGrid = function () {
                        self.loadCompanyGrid(self.grid(), $("#companyTable"));
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
                        self.addCompany(model.id(), function (em, json) {
                            var findRow = gridModel.findCompany(json.id);
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

            this.loadCompanyGrid = function (m, $target) {
                var $table = $("#companyTable");
                if ($table[0]) { $target = $table; }
                handleBlockUI({ "target": $target });
                var arr = $("#frmCompanySearch").serializeArray();
                arr[arr.length] = { "name": "pageIndex", "value": m.page_index() };
                arr[arr.length] = { "name": "pageSize", "value": m.rows_per_page() };
                arr[arr.length] = { "name": "sortName", "value": m.sort_name() };
                arr[arr.length] = { "name": "sortOrder", "value": m.sort_order() };
                var isbookmark = $("#frmCompanySearch #is_book_mark")[0].checked;
                if (isbookmark == true) {
                    arr[arr.length] = { "name": "is_book_mark", "value": isbookmark };
                }
                var url = apiUrl("/Company/List");
                $.ajax({
                    "url": url,
                    "cache": false,
                    "type": "GET",
                    "data": arr
                }).done(function (json) {
                    m.total_rows(json.total);
                    m.rows(json.rows);
                    //$.each(json.rows, function (i, row) {
                    //    var rm = new CompanyModel();
                    //    self.assignEditEvents(rm, m)
                    //    komapping.fromJS(row, {}, rm);
                    //    m.rows.push(rm);
                    //});
                    if (m.refreshCallBack == null) {
                        m.refreshCallBack = function (m) {
                            self.loadCompanyGrid(m, $target);
                        }
                    }
                }).always(function () {
                    unblockUI($target);
                });
            }


            this.refresh = function () {
                var $target = $(".page-content");
                self.loadCompanyGrid(self.grid(), $target);
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
                       self.loadCompanyGrid(self.grid(), $target);
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
                       self.loadCompanyGrid(self.grid(), $target);
                   }
                });
            }

            this.loadCompany = function (callback) {
                GridModel.prototype.findCompany = function (id) {
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
                    self.addCompany(0, function (group, json) {
                        komapping.fromJS(json, {}, group);
                        self.assignEditEvents(group, that);
                        that.rows.splice(0, 0, group);
                        that.total_rows(cInt(that.total_rows()) + 1);
                    })
                }
                self.grid(new GridModel())
                self.grid().sort_name("week_52_percentage");
                self.grid().sort_order("desc");
                self.grid().rows_per_page(200);
                var $target = $(".page-content");
                if (callback)
                    callback();
            }

            /* end group */

            this.init = function () {
                self.loadCompany(function () {
                    self.applyPlugins();
                    var $target = $(".page-content");
                    self.loadCompanyGrid(self.grid(), $target);

                    $("body").on("click", "#frmCompanySearch #is_book_mark", function (event) {
                        self.loadCompanyGrid(self.grid(), $target);
                    });

                    $("body").on("click", ".is-book-mark", function (event) {
                        var $this = $(this);
                        var $i = $("i", $this);
                        var dataFor = ko.dataFor(this);
                        var url = apiUrl('/Company/UpdateBookMark');
                        var arr = [];
                        var isBookMark = $i.hasClass('fa-bookmark');
                        arr.push({ "name": "symbol", "value": dataFor.symbol });
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
                });
                unblockUI();
            }
        }
    }
);
