﻿"use strict";
define("CategoryController", ["knockout", "komapping", "helper", "service"], function (ko, komapping, helper, service) {
    return function () {
        var self = this;
        this.template = "/Home/Category";

        this.rows = ko.observableArray([]);

        this.refresh = function () {
            self.loadGrid();
        }

        this.loadGrid = function (callback) {
            handleBlockUI();
            var $Category = $("#Category");
            var arr = $("#frmCategorySearch").serializeArray();
            arr.push({ "name": "PageSize", "value": $(":input[name='rows']", $Category).val() });
            arr.push({ "name": "PageIndex", "value": $(":input[name='page_index']", $Category).val() });
            arr.push({ "name": "SortName", "value": $(":input[name='sort_name']", $Category).val() });
            arr.push({ "name": "SortOrder", "value": $(":input[name='sort_order']", $Category).val() });
            var url = apiUrl("/Category/Search");
            $.ajax({
                "url": url,
                "cache": false,
                "type": "GET",
                "data": arr
            }).done(function (json) {
                self.rows.removeAll();
                self.rows(json.rows);
                $(".manual-pagination", $Category).each(function () {
                    var element = this;
                    $(element).twbsPagination({
                        total: cInt(json.total),
                        rowsPerPage: cInt($(":input[name='rows']", $Category).val()),
                        startPage: cInt($(":input[name='page_index']", $Category).val()),
                        currentPage: cInt($(":input[name='page_index']", $Category).val()),
                        onRender: function (status) {
                            $("#paging_status", $Category).html(status);
                        },
                        onPageClick: function (page) {
                            $(":input[name='page_index']", $Category).val(page);
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
                    'category_name': ''
                    , 'id': 0
                };
            }
            ko.applyBindings(row, $modal[0]);
            var $frm = $("form", $modal);
            var $btn = $("#save", $frm);
            $btn.click(function () {
                if ($frm.valid()) {
                    $btn.button('loading');
                    var data = $frm.serializeArray();
                    var url = apiUrl("/Category/Create");
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
            $("body").on("click", "#Category .btn-add", function (event) {
                self.openItem(null)
            });
            $("body").on("click", "#Category .btn-edit", function (event) {
                self.openItem(ko.dataFor(this));
            });
            $("body").on("click", "#Category .btn-delete", function (event) {
                var dataFor = ko.dataFor(this);
                jConfirm({
                    "message": "Are you sure?", "ok": function () {
                        var url = apiUrl("/Category/Delete/" + dataFor.id);
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
            $("body").on("change", "#Category #rows", function (event) {
                self.loadGrid();
            });
        }

        this.offElements = function () {
            $("body").off("click", "#Category .btn-add");
            $("body").off("click", "#Category .btn-edit");
            $("body").off("click", "#Category .btn-delete");
            $("body").off("change", "#Category #rows");
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

                var $Category = $("#Category");
                var $tbl = $("#CategoryTable", $Category);
                var $sort_name = $(":input[name='sort_name']", $Category);
                var $sort_order = $(":input[name='sort_order']", $Category);
                $tbl.attr("sortname", $sort_name.val());
                $tbl.attr("sortorder", $sort_order.val());
                helper.sortingTable($tbl[0], {
                    "onSorting": function (sortname, sortorder) {
                        $(":input[name='sort_name']", $Category).val(sortname);
                        $(":input[name='sort_order']", $Category).val(sortorder);
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
