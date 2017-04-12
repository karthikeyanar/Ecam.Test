"use strict";
define(["knockout", "komapping", "helper"], function (ko, komapping, helper) {
    return function MarketListModel() {
        var self = this;

        this.id = ko.observable(0);
        this.symbol = ko.observable();
        this.trade_date = ko.observable();
        this.trade_type = ko.observable();
        this.open_price = ko.observable();
        this.high_price = ko.observable();
        this.low_price = ko.observable();
        this.close_price = ko.observable();
        this.prev_price = ko.observable();
        this.week_52_high = ko.observable();
        this.months_3_high = ko.observable();
        this.months_1_high = ko.observable();
        this.day_5_high = ko.observable();

        this.company_name = ko.observable();
        this.prev_percentage = ko.observable();
        this.week_52_percentage = ko.observable();
        this.months_3_percentage = ko.observable();
        this.months_1_percentage = ko.observable();
        this.day_5_percentage = ko.observable();

        this.is_edit_mode = ko.observable(false);
        this.is_focus = ko.observable(false);
        this.success_result = ko.observable("");
        this.is_save_and_exit = ko.observable(true);

        this.onEdit = null;
        this.edit = function () {
            return;
            if (self.onEdit)
                self.onEdit();
        }
        this.onAdd = null;
        this.add = function () {
            return;
            if (self.onAdd)
                self.onAdd();
        }
        this.onDelete = null;
        this.deleteMarket = function () {
            var url = apiUrl("/Market/delete/" + self.id());
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
        this.searchMarket = function (request, response) {
            service.searchMarket(request, response);
        }
        this.onBeforeSelectMarket = null;
        this.onAfterSelectMarket = null;
        this.selectMarket = function (event, ui) {
            self.id(ui.item.id);
            self.load();
        }
        this.load = function () {
            if (self.onBeforeSelectMarket) {
                self.onBeforeSelectMarket();
            }
            var url = apiUrl("/Market/find/" + self.id());
            handleBlockUI();
            $.ajax({
                "url": url,
                "cache": false,
                "type": "GET"
            }).done(function (json) {
                unblockUI();
                komapping.fromJS(json, {}, self);
                if (self.onAfterSelectMarket) {
                    self.onAfterSelectMarket();
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
                var url = apiUrl("/Market/create");
                var type = "POST";
                if (self.id() > 0) { type = "PUT"; url = apiUrl("/Market/update/") + self.id(); }
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