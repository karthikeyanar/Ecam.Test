"use strict";
define("FinancialCategoryController", ["knockout", "komapping", "helper", "service"], function (ko, komapping, helper, service) {
    return function () {
        var self = this;
        this.template = "/Home/FinancialCategory";

        this.rows = ko.observableArray([]);

        this.refresh = function () {
            self.loadGrid();
        }

        this.loadGrid = function (callback) {
            handleBlockUI();
            var $FinancialCategory = $("#FinancialCategory");
            var arr = $("#frmFinancialCategorySearch").serializeArray();
            arr.push({ "name": "PageSize", "value": $(":input[name='rows']", $FinancialCategory).val() });
            arr.push({ "name": "PageIndex", "value": $(":input[name='page_index']", $FinancialCategory).val() });
            arr.push({ "name": "SortName", "value": $(":input[name='sort_name']", $FinancialCategory).val() });
            arr.push({ "name": "SortOrder", "value": $(":input[name='sort_order']", $FinancialCategory).val() });
            var isarchive = $("#frmFinancialCategorySearch #is_archive")[0].checked;
            if (isarchive == true) {
                arr[arr.length] = { "name": "is_archive", "value": isarchive };
            }
            var url = apiUrl("/FinancialCategory/List");
            $.ajax({
                "url": url,
                "cache": false,
                "type": "GET",
                "data": arr
            }).done(function (json) {
                self.rows.removeAll();
                $.each(json.rows, function (i, row) {
                    self.rows.push(komapping.fromJS(row));
                })
                $(".manual-pagination", $FinancialCategory).each(function () {
                    var element = this;
                    $(element).twbsPagination({
                        total: cInt(json.total),
                        rowsPerPage: cInt($(":input[name='rows']", $FinancialCategory).val()),
                        startPage: cInt($(":input[name='page_index']", $FinancialCategory).val()),
                        currentPage: cInt($(":input[name='page_index']", $FinancialCategory).val()),
                        onRender: function (status) {
                            $("#paging_status", $FinancialCategory).html(status);
                        },
                        onPageClick: function (page) {
                            $(":input[name='page_index']", $FinancialCategory).val(page);
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
                    var url = apiUrl("/FinancialCategory/Create");
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
            $("body").on("click", "#frmFinancialCategorySearch #is_archive", function (event) {
                self.loadGrid();
            });
            $("body").on("click", "#frmFinancialCategorySearch #is_book_mark", function (event) {
                self.loadGrid();
            });
            $("body").on("click", ".is-archive", function (event) {
                var $this = $(this);
                var $i = $("i", $this);
                var dataFor = ko.dataFor(this);
                var url = apiUrl('/FinancialCategory/UpdateArchive');
                var arr = [];
                var isArchive = $i.hasClass('fa-bookmark');
                arr.push({ "name": "category_name", "value": dataFor.category_name() });
                arr.push({ "name": "is_archive", "value": !isArchive });
                $i.removeClass('fa-bookmark').removeClass('fa-bookmark-o').removeClass('fg-primary');
                if (isArchive == true) {
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
            $("body").on("click", "#FinancialCategory .btn-add", function (event) {
                self.openItem(null)
            });
            $("body").on("click", "#FinancialCategory .btn-edit", function (event) {
                self.openItem(ko.dataFor(this));
            });
            $("body").on("click", "#FinancialCategory .btn-delete", function (event) {
                var dataFor = ko.dataFor(this);
                jConfirm({
                    "message": "Are you sure?", "ok": function () {
                        var url = apiUrl("/FinancialCategory/Delete/" + dataFor.id);
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
            $("body").on("change", "#FinancialCategory #rows", function (event) {
                self.loadGrid();
            });
        }

        this.offElements = function () {
            $("body").off("click", "#frmFinancialCategorySearch #is_archive");
            $("body").off("click", "#FinancialCategory .btn-add");
            $("body").off("click", "#FinancialCategory .btn-edit");
            $("body").off("click", "#FinancialCategory .btn-delete");
            $("body").off("change", "#FinancialCategory #rows");
            $("body").off("click", ".is-archive");
            $("body").off("click", ".is-book-mark");
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

                var $FinancialCategory = $("#FinancialCategory");
                var $tbl = $("#FinancialCategoryTable", $FinancialCategory);
                var $sort_name = $(":input[name='sort_name']", $FinancialCategory);
                var $sort_order = $(":input[name='sort_order']", $FinancialCategory);
                $tbl.attr("sortname", $sort_name.val());
                $tbl.attr("sortorder", $sort_order.val());
                helper.sortingTable($tbl[0], {
                    "onSorting": function (sortname, sortorder) {
                        $(":input[name='sort_name']", $FinancialCategory).val(sortname);
                        $(":input[name='sort_order']", $FinancialCategory).val(sortorder);
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
