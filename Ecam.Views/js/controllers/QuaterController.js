"use strict";
define("QuaterController", ["knockout", "komapping", "helper", "service"], function (ko, komapping, helper, service) {
    return function () {
        var self = this;
        this.template = "/Home/Quater";

        this.columns = ko.observableArray([]);
        this.rows = ko.observableArray();

        this.loadGrid = function () {
            var $companyQuaterTable = $("#companyQuaterTable");
            var url = apiUrl("/Quater/List");
            var arr = [];
            arr.push({ 'name': 'start_date', 'value': '01/01/2001' });
            arr.push({ 'name': 'end_date', 'value': '12/31/2028' });
            arr.push({ 'name': 'pageindex', 'value': '1' });
            arr.push({ 'name': 'pagesize', 'value': '1500000' });
            arr.push({ 'name': 'sortname', 'value': $companyQuaterTable.attr('sortname') });
            arr.push({ 'name': 'sortorder', 'value': $companyQuaterTable.attr('sortorder') });
            arr.push({ 'name': 'financial_category_id', 'value': 27 });
            console.log(arr);
            $.ajax({
                "url": url,
                "cache": false,
                "type": "GET",
                "data": arr
            }).done(function (json) {
                self.columns.removeAll();
                self.rows.removeAll();
                self.columns(json.columns);
                for (var i = 0; i < json.rows.length; i++) {
                    self.rows.push(komapping.fromJS(json.rows[i]));
                }
                helper.sortingTable($companyQuaterTable[0], {
                    'onSorting': function (sortname, sortorder) {
                        console.log('sortname=', sortname, 'sortorder=', sortorder);
                        $companyQuaterTable.attr('sortname', sortname);
                        $companyQuaterTable.attr('sortorder', sortorder);
                        self.loadGrid();
                    }
                });
                //console.log(self.rows());
            }).always(function () {
                unblockUI();
            });
        }

        this.onElements = function () {
            self.offElements();
        }

        this.offElements = function () {
        }

        this.init = function () {
            var $companyQuaterTable = $("#companyQuaterTable");
            var dt = moment(_TODAYDATE).subtract('month', 4).toDate();
            var year = dt.getFullYear();
            var month = dt.getMonth() + 1;
            var quaterName = '';
            if (month >= 4 && month <= 6) {
                quaterName = 'Q1';
            } else if (month >= 7 && month <= 9) {
                quaterName = 'Q2';
            } else if (month >= 10 && month <= 12) {
                quaterName = 'Q3';
            } else if (month >= 1 && month <= 3) {
                quaterName = 'Q4';
                year = year - 1;
            }
            var sortname = year + ' ' + quaterName;
            console.log('year=', year, 'month=', month, 'sortname=', sortname);
            $companyQuaterTable.attr('sortname', sortname);
            $companyQuaterTable.attr('sortorder', 'desc');
            self.loadGrid();
            unblockUI();
        }

        this.unInit = function () {
            self.offElements();
        }
    }
}
);
