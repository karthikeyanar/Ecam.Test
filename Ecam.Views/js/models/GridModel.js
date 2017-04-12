"use strict";
define(["knockout", "komapping", "helper"], function (ko, komapping, helper) {
    return function GridModel() {
        var self = this;
        this.rows = ko.observableArray([]);
        this.total_rows = ko.observable(0);
        this.page_index = ko.observable(1);
        this.rows_per_page = ko.observable(20);
        this.sort_name = ko.observable("");
        this.sort_order = ko.observable("");
        this.paging_status = ko.observable("");

        this.row_sizes = ko.observableArray([20, 50, 100, 150, 200]);

        this.is_manual_refresh = false;

        this.total_pages = ko.computed(function () {
            return Math.ceil(cInt(self.total_rows()) / cInt(self.rows_per_page()))
        });

        this.refreshCallBack = null;
        this.onRenderPagination = function (status) {
            self.paging_status(status);
        }
        this.onPageClick = function (pageIndex) {
            self.page_index(pageIndex);
        }
        this.page_index.subscribe(function (newValue) {
            if (self.is_manual_refresh == true) return;
            if (self.page_index() > self.total_pages()) {
                return;
            }
            if (self.refreshCallBack)
                self.refreshCallBack(self);
        });
        this.rows_per_page.subscribe(function (newValue) {
            if (self.is_manual_refresh == true) return;
            if (self.page_index() > self.total_pages()) {
                return;
            }
            if (self.page_index() != 1) {
                self.page_index(1);
            } else {
                if (self.refreshCallBack)
                    self.refreshCallBack(self);
            }
        });
        this.changeSortOrder = function (sortname, sortorder) {
            if (self.is_manual_refresh == true) return;
            self.sort_order(sortorder);
            self.sort_name(sortname);
            if (self.page_index() > self.total_pages()) {
                return;
            }
            if (self.refreshCallBack)
                self.refreshCallBack(self);
        };
        this.refresh = function () {
            self.is_manual_refresh = true;
            self.page_index(1);
            self.is_manual_refresh = false;
            if (self.refreshCallBack)
                self.refreshCallBack(self);
        }
        this.table_id = ko.observable("");
    }
});