"use strict";
define(["knockout", "komapping", "helper"], function (ko, komapping, helper) {
    return function CompanyListModel() {
        var self = this;

        this.id = ko.observable(0);
        this.company_name = ko.observable("");
        this.symbol = ko.observable("");

        this.open_price = ko.observable();
        this.high_price = ko.observable();
        this.low_price = ko.observable();
        this.close_price = ko.observable();
        this.prev_price = ko.observable();
        this.week_52_high = ko.observable();
        this.months_3_high = ko.observable();
        this.months_1_high = ko.observable();
        this.day_5_high = ko.observable();

        this.category_name = ko.observable();

        this.is_edit_mode = ko.observable(false);
        this.is_focus = ko.observable(false);
        this.success_result = ko.observable("");
        this.is_save_and_exit = ko.observable(true);

        this.onEdit = null;
        this.edit = function () {
            if (self.onEdit)
                self.onEdit();
        }
        this.onAdd = null;
        this.add = function () {
            if (self.onAdd)
                self.onAdd();
        }
        this.onDelete = null;
        this.deleteCompany = function () {
            var url = apiUrl("/Company/delete/" + self.id());
            $.ajax({
                "url": url,
                "cache": false,
                "type": "DELETE"
            }).done(function (json) {
                if (self.onDelete)
                    self.onDelete();
            }).fail(function (jqxhr) {
                jAlert(helper.deleteErrorMessage);
            });
        }

        this.errors = ko.observable(null);
        this.searchCompany = function (request, response) {
            service.searchCompany(request, response);
        }
        this.onBeforeSelectCompany = null;
        this.onAfterSelectCompany = null;
        this.selectCompany = function (event, ui) {
            self.id(ui.item.id);
            self.load();
        }
        this.load = function () {
            if (self.onBeforeSelectCompany) {
                self.onBeforeSelectCompany();
            }
            var url = apiUrl("/Company/find/" + self.id());
            handleBlockUI();
            $.ajax({
                "url": url,
                "cache": false,
                "type": "GET"
            }).done(function (json) {
                unblockUI();
                komapping.fromJS(json, {}, self);
                if (self.onAfterSelectCompany) {
                    self.onAfterSelectCompany();
                }
            });
        }
        this.onSave = null;
        this.onBeforeSave = null;
        this.onAfterSave = null;
        this.save = function (formElement) {
            self.success_result("");
            var $frm = $(formElement);
            if ($frm.valid()) {
                var data = $frm.serializeArray();
                var url = apiUrl("/Company/create");
                var type = "POST";
                if (self.id() > 0) { type = "PUT"; url = apiUrl("/Company/update/") + self.id(); }
                if (self.onBeforeSave) {
                    self.onBeforeSave(formElement);
                }
                $.ajax({
                    "url": url,
                    "cache": false,
                    "type": type,
                    "data": data
                }).done(function (json) {
                    self.errors(null);
                    if (json.Errors == null) {
                        komapping.fromJS(json, {}, self);
                        if (self.onSave) {
                            self.onSave(json);
                        }
                    }
                }).fail(function (response) {
                    self.errors(generateErrors(response.responseJSON));
                }).always(function (jqxhr) {
                    if (self.onAfterSave)
                        self.onAfterSave(formElement);
                });
            }
        }
    }
});