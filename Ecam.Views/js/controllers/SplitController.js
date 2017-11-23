"use strict";
define("SplitController", ["knockout", "komapping", "helper", "service"], function (ko, komapping, helper, service) {
    return function () {
        var self = this;
        this.template = "/Home/Split";

        this.rows = ko.observableArray([]);

        this.refresh = function () {
            self.loadGrid();
        }

        this.profit = ko.observable();
        this.profit_percentage = ko.observable();
        this.total_investment = ko.observable();
        this.total_market_value = ko.observable();
        this.final_total = ko.observable();

        this.loadGrid = function (callback) {
            handleBlockUI();
            var $Split = $("#Split");
            var arr = $("#frmSplitSearch").serializeArray();
            arr.push({ "name": "PageSize", "value": $(":input[name='rows']", $Split).val() });
            arr.push({ "name": "PageIndex", "value": $(":input[name='page_index']", $Split).val() });
            arr.push({ "name": "SortName", "value": $(":input[name='sort_name']", $Split).val() });
            arr.push({ "name": "SortOrder", "value": $(":input[name='sort_order']", $Split).val() });
            var url = apiUrl("/Split/List");
            $.ajax({
                "url": url,
                "cache": false,
                "type": "GET",
                "data": arr
            }).done(function (json) {
                self.rows.removeAll();
                self.rows(json.rows);
                if (callback)
                    callback();
            }).always(function () {
                unblockUI();
            });
        }

        this.applyPlugins = function () {
        }

        this.openItem = function (row) {
            $('#temp-modal-container').remove();
            var $cnt = $("<div id='temp-modal-container'></div>");
            $('body').append($cnt);
            var data = {
                "name": "Edit"
                , "title": (row == null ? "Add" : "Edit")
                , "is_modal_full": false
                , "position": "top"
            };
            $("#modal-template").tmpl(data).appendTo($cnt);
            var $modal = $("#modal-" + data.name, $cnt);
            $modal.modal('show');
            if (row == null) {
                row = {
                    'symbol': ''
                    , 'id': 0
                    , 'split_factor': 0
                    , 'split_date': ''
                };
            }
            ko.applyBindings(row, $modal[0]);
            var $frm = $("form", $modal);
            var $btn = $("#save", $frm);
            $btn.click(function () {
                if ($frm.valid()) {
                    $btn.button('loading');
                    var data = $frm.serializeArray();
                    var url = apiUrl("/Split/Create");
                    var type = "POST";
                    handleBlockUI({ "target": $("body"), "message": "Save..." });
                    $.ajax({
                        "url": url,
                        "cache": false,
                        "type": type,
                        "data": data
                    }).done(function (json) {
                        //jAlert("Saved");
                        $modal.modal('hide');
                        self.loadGrid();
                    }).fail(function (jqxhr) {
                        alertErrorMessage(jqxhr.responseJSON);
                    }).always(function (jqxhr) {
                        $btn.button('reset');
                        unblockUI();
                    });
                }
            });
        }

        this.onElements = function () {
            self.offElements();
            $("body").on("click", "#Split .btn-add", function (event) {
                self.openItem(null)
            });
            $("body").on("click", "#Split .btn-edit", function (event) {
                self.openItem(ko.dataFor(this));
            });
            $("body").on("click", "#Split .btn-delete", function (event) {
                var dataFor = ko.dataFor(this);
                jConfirm({
                    "message": "Are you sure?", "ok": function () {
                        var url = apiUrl("/Split/Delete/" + dataFor.id);
                        $.ajax({
                            "url": url,
                            "cache": false,
                            "type": "DELETE"
                        }).done(function () {
                            //jAlert("Deleted");
                            self.loadGrid();
                        }).fail(function (response) {
                            //self.errors(generateErrors(response.responseJSON));
                        }).always(function (jqxhr) {
                        });
                    }
                });
            });
            $("body").on("change", "#Split #rows", function (event) {
                self.loadGrid();
            });
        }

        this.offElements = function () {
            $("body").off("click", "#Split .btn-add");
            $("body").off("click", "#Split .btn-edit");
            $("body").off("click", "#Split .btn-delete");
            $("body").off("change", "#Split #rows");
        }

        this.unInit = function () {
            self.offElements();
        }

        this.init = function (callback) {
            unblockUI();
            if (callback)
                callback();

            self.applyPlugins();
            pushGCEvent(function (e, ui) {
                self.loadGrid();
            });
            self.loadGrid(function () {
                self.onElements();

                var $Split = $("#Split");
                var $tbl = $("#SplitTable", $Split);
                var $sort_name = $(":input[name='sort_name']", $Split);
                var $sort_order = $(":input[name='sort_order']", $Split);
                $tbl.attr("sortname", $sort_name.val());
                $tbl.attr("sortorder", $sort_order.val());
                helper.sortingTable($tbl[0], {
                    "onSorting": function (sortname, sortorder) {
                        $(":input[name='sort_name']", $Split).val(sortname);
                        $(":input[name='sort_order']", $Split).val(sortorder);
                        self.loadGrid();
                    }
                });
                $tbl.floatThead({
                    scrollContainer: function ($t) {
                        return $t.closest('.summary-table-cnt');
                    }
                });
            });
        }
    }
}
);
