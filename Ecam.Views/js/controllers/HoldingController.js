"use strict";
define("HoldingController", ["knockout", "komapping", "helper", "service"], function (ko, komapping, helper, service) {
    return function () {
        var self = this;
        this.template = "/Home/Holding";

        this.rows = ko.observableArray([]);

        this.refresh = function () {
            self.loadGrid();
        }

        this.profit = ko.observable();
        this.profit_percentage = ko.observable();
        this.total_investment = ko.observable();
        this.total_market_value = ko.observable();

        this.loadGrid = function (callback) {
            handleBlockUI();
            var $Holding = $("#Holding");
            var arr = $("#frmHoldingSearch").serializeArray();
            arr.push({ "name": "PageSize", "value": $(":input[name='rows']", $Holding).val() });
            arr.push({ "name": "PageIndex", "value": $(":input[name='page_index']", $Holding).val() });
            arr.push({ "name": "SortName", "value": $(":input[name='sort_name']", $Holding).val() });
            arr.push({ "name": "SortOrder", "value": $(":input[name='sort_order']", $Holding).val() });
            var url = apiUrl("/Holding/List");
            $.ajax({
                "url": url,
                "cache": false,
                "type": "GET",
                "data": arr
            }).done(function (json) {
                self.rows.removeAll();
                self.rows(json.rows);
                var totalInvestment = 0;
                var totalMarketValue = 0;
                var profitPercentage = 0;
                $.each(json.rows, function (i, row) {
                    totalInvestment += cFloat(row.investment);
                    totalMarketValue += cFloat(row.final_total);
                });
                profitPercentage = ((cFloat(totalMarketValue) - cFloat(totalInvestment)) / cFloat(totalInvestment)) * 100;
                self.total_investment(totalInvestment);
                self.total_market_value(totalMarketValue);
                self.profit(cFloat(totalMarketValue) - cFloat(totalInvestment));
                self.profit_percentage(profitPercentage);
                $(".manual-pagination", $Holding).each(function () {
                    var element = this;
                    $(element).twbsPagination({
                        total: cInt(json.total),
                        rowsPerPage: cInt($(":input[name='rows']", $Holding).val()),
                        startPage: cInt($(":input[name='page_index']", $Holding).val()),
                        currentPage: cInt($(":input[name='page_index']", $Holding).val()),
                        onRender: function (status) {
                            $("#paging_status", $Holding).html(status);
                        },
                        onPageClick: function (page) {
                            $(":input[name='page_index']", $Holding).val(page);
                            self.loadGrid();
                        }
                    });
                });
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
                    , 'quantity': 0
                    , 'trade_date': formatDate(new Date())
                    , 'avg_price': 0
                    , 'ltp_price': 0
                };
            }
            ko.applyBindings(row, $modal[0]);
            var $frm = $("form", $modal);
            var $btn = $("#save", $frm);
            $btn.click(function () {
                if ($frm.valid()) {
                    $btn.button('loading');
                    var data = $frm.serializeArray();
                    var url = apiUrl("/Holding/Create");
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
            $("body").on("click", "#Holding .btn-add", function (event) {
                self.openItem(null)
            });
            $("body").on("click", "#Holding .btn-edit", function (event) {
                self.openItem(ko.dataFor(this));
            });
            $("body").on("click", "#Holding .btn-delete", function (event) {
                var dataFor = ko.dataFor(this);
                jConfirm({
                    "message": "Are you sure?", "ok": function () {
                        var url = apiUrl("/Holding/Delete/" + dataFor.id);
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
            $("body").on("change", "#Holding #rows", function (event) {
                self.loadGrid();
            });
        }

        this.offElements = function () {
            $("body").off("click", "#Holding .btn-add");
            $("body").off("click", "#Holding .btn-edit");
            $("body").off("click", "#Holding .btn-delete");
            $("body").off("change", "#Holding #rows");
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

                var $Holding = $("#Holding");
                var $tbl = $("#HoldingTable", $Holding);
                var $sort_name = $(":input[name='sort_name']", $Holding);
                var $sort_order = $(":input[name='sort_order']", $Holding);
                $tbl.attr("sortname", $sort_name.val());
                $tbl.attr("sortorder", $sort_order.val());
                helper.sortingTable($tbl[0], {
                    "onSorting": function (sortname, sortorder) {
                        $(":input[name='sort_name']", $Holding).val(sortname);
                        $(":input[name='sort_order']", $Holding).val(sortorder);
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
