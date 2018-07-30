"use strict";
define("CM_CompanyUpdateController", ["knockout", "komapping", "helper", "service", "tokenfield"], function (ko, komapping, helper, service, tokenfield) {
    return function () {
        var self = this;
        this.template = "/CM/CompanyUpdate";
        this.index = -1;
        this.company_json = null;
        this.rows = ko.observableArray([]);
        this.start_date = ko.observable("");
        this.end_date = ko.observable("");

        this.nseDownload = function () {
            if (self.company_json == null) {
                handleBlockUI();
                var dt = '27-Jul-2018';
                var url = apiUrl("/Company/GetNSEUpdate?last_trade_date=" + dt + "&is_book_mark_category=" + $("#is_book_mark_category")[0].checked);
                var arr = [];
                $.ajax({
                    "url": url,
                    "cache": false,
                    "type": "GET",
                    "data": arr
                }).done(function (json) {
                    self.index = -1;
                    self.company_json = json;
                    self.startNSE();
                }).always(function () {
                    unblockUI();
                });
            } else {
                console.log(2);
                self.startNSE();
            }
        }

        this.startNSE = function () {
            $('#gcb_nse_cmd').click();
        }

        this.startNSEDownload = function () {
            self.index += 1;
            console.log('startNSEDownload self.index=', self.index);
            if (self.index <= self.company_json.length) {
                console.log(self.company_json[self.index]);
                //setTimeout(function () {
                //    self.startNSE();
                //}, 2000);
            }
        }


        this.onElements = function () {
            self.offElements();
            $("body").on("click", "#btnNSEDownload", function (event) {
                self.nseDownload();
            });
        }

        this.offElements = function () {
            $("body").off("click", "#btnNSEDownload");
        }

        this.init = function (callback) {
            self.company_json = [];
            self.company_json.push({ 'symbol': 'SBIN', 'start_date': '01/01/2018', 'end_date': '30/07/2018' });
            self.company_json.push({ 'symbol': 'HDFC', 'start_date': '01/01/2018', 'end_date': '30/07/2018' });
            self.company_json.push({ 'symbol': 'INFY', 'start_date': '01/01/2018', 'end_date': '30/07/2018' });
            self.company_json.push({ 'symbol': 'TCS', 'start_date': '01/01/2018', 'end_date': '30/07/2018' });
            self.onElements();
            if (callback)
                callback();
        }

        this.unInit = function () {
            self.offElements();
        }
    }
}
);