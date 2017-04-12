"use strict";
define(["knockout", "komapping", "helper"], function (ko, komapping, helper) {
    return function ExpertsRecommendListModel() {
        var self = this;

        this.id = ko.observable();
        this.experts_id = ko.observable();
        this.record_date = ko.observable();
        this.company_id = ko.observable();
        this.recommend_type = ko.observable();
        this.stop_loss = ko.observable();
        this.target = ko.observable();
        this.url = ko.observable();

        this.company_name = ko.observable("");
        this.experts_name = ko.observable("");

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
        this.deleteExpertsRecommend = function () {
            var url = apiUrl("/ExpertsRecommend/delete/" + self.id());
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
        this.searchExpertsRecommend = function (request, response) {
            service.searchExpertsRecommend(request, response);
        }
        this.onBeforeSelectExpertsRecommend = null;
        this.onAfterSelectExpertsRecommend = null;
        this.selectExpertsRecommend = function (event, ui) {
            self.id(ui.item.id);
            self.load();
        }
        this.load = function () {
            if (self.onBeforeSelectExpertsRecommend) {
                self.onBeforeSelectExpertsRecommend();
            }
            var url = apiUrl("/ExpertsRecommend/find/" + self.id());
            handleBlockUI();
            $.ajax({
                "url": url,
                "cache": false,
                "type": "GET"
            }).done(function (json) {
                unblockUI();
                komapping.fromJS(json, {}, self);
                if (self.onAfterSelectExpertsRecommend) {
                    self.onAfterSelectExpertsRecommend();
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
                var url = apiUrl("/ExpertsRecommend/create");
                var type = "POST";
                if (self.id() > 0) { type = "PUT"; url = apiUrl("/ExpertsRecommend/update/") + self.id(); }
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