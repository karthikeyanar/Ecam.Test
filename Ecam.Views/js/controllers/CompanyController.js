"use strict";
define("CompanyController", ["knockout", "komapping", "helper", "service"], function (ko, komapping, helper, service) {
    return function () {
        var self = this;
        this.template = "/Home/Company";

        this.rows = ko.observableArray([]);

        this.refresh = function () {
            self.loadGrid();
        }

        this.last_grid_data = null;
        this.loadGrid = function (callback) {
            handleBlockUI();
            var $Company = $("#Company");
            var arr = $("#frmCompanySearch").serializeArray();
            arr.push({ "name": "PageSize", "value": $(":input[name='rows']", $Company).val() });
            arr.push({ "name": "PageIndex", "value": $(":input[name='page_index']", $Company).val() });
            arr.push({ "name": "SortName", "value": $(":input[name='sort_name']", $Company).val() });
            arr.push({ "name": "SortOrder", "value": $(":input[name='sort_order']", $Company).val() });
            var isarchive = $("#frmCompanySearch #is_archive")[0].checked;
            if (isarchive == true) {
                arr[arr.length] = { "name": "is_archive", "value": isarchive };
            }
            var is_book_mark = $("#frmCompanySearch #is_book_mark")[0].checked;
            if (is_book_mark == true) {
                arr[arr.length] = { "name": "is_book_mark", "value": is_book_mark };
            }
            var isNifty50 = $("#frmCompanySearch #is_nifty_50")[0].checked;
            if (isNifty50 == true) {
                arr[arr.length] = { "name": "is_nifty_50", "value": isNifty50 };
            }
            var isNifty100 = $("#frmCompanySearch #is_nifty_100")[0].checked;
            if (isNifty100 == true) {
                arr[arr.length] = { "name": "is_nifty_100", "value": isNifty100 };
            }
            var isNifty200 = $("#frmCompanySearch #is_nifty_200")[0].checked;
            if (isNifty200 == true) {
                arr[arr.length] = { "name": "is_nifty_200", "value": isNifty200 };
            }
            var isOld = $("#frmCompanySearch #is_old")[0].checked;
            if (isOld == true) {
                arr[arr.length] = { "name": "is_old", "value": isOld };
            }
            var is_all_time_low = $("#frmCompanySearch #is_all_time_low")[0].checked;
            if (is_all_time_low == true) {
                arr[arr.length] = { "name": "is_all_time_low", "value": is_all_time_low };
            }
            var is_all_time_high = $("#frmCompanySearch #is_all_time_high")[0].checked;
            if (is_all_time_high == true) {
                arr[arr.length] = { "name": "is_all_time_high", "value": is_all_time_high };
            }
            var is_all_time_low_15_days = $("#frmCompanySearch #is_all_time_low_15_days")[0].checked;
            if (is_all_time_low_15_days == true) {
                arr[arr.length] = { "name": "is_all_time_low_15_days", "value": is_all_time_low_15_days };
            }
            var is_all_time_high_15_days = $("#frmCompanySearch #is_all_time_high_15_days")[0].checked;
            if (is_all_time_high_15_days == true) {
                arr[arr.length] = { "name": "is_all_time_high_15_days", "value": is_all_time_high_15_days };
            }

            var is_all_time_low_5_days = $("#frmCompanySearch #is_all_time_low_5_days")[0].checked;
            if (is_all_time_low_5_days == true) {
                arr[arr.length] = { "name": "is_all_time_low_5_days", "value": is_all_time_low_5_days };
            }
            var is_all_time_high_5_days = $("#frmCompanySearch #is_all_time_high_5_days")[0].checked;
            if (is_all_time_high_5_days == true) {
                arr[arr.length] = { "name": "is_all_time_high_5_days", "value": is_all_time_high_5_days };
            }

            var is_all_time_low_2_days = $("#frmCompanySearch #is_all_time_low_2_days")[0].checked;
            if (is_all_time_low_2_days == true) {
                arr[arr.length] = { "name": "is_all_time_low_2_days", "value": is_all_time_low_2_days };
            }
            var is_all_time_high_2_days = $("#frmCompanySearch #is_all_time_high_2_days")[0].checked;
            if (is_all_time_high_2_days == true) {
                arr[arr.length] = { "name": "is_all_time_high_2_days", "value": is_all_time_high_2_days };
            }
            var is_sell_to_buy = $("#frmCompanySearch #is_sell_to_buy")[0].checked;
            if (is_sell_to_buy == true) {
                arr[arr.length] = { "name": "is_sell_to_buy", "value": is_sell_to_buy };
            }
            var is_buy_to_sell = $("#frmCompanySearch #is_buy_to_sell")[0].checked;
            if (is_buy_to_sell == true) {
                arr[arr.length] = { "name": "is_buy_to_sell", "value": is_buy_to_sell };
            }

            var is_mf = $("#frmCompanySearch #is_mf")[0].checked;
            if (is_mf == true) {
                arr[arr.length] = { "name": "is_mf", "value": is_mf };
            }
            var url = apiUrl("/Company/Companies");
            $.ajax({
                "url": url,
                "cache": false,
                "type": "GET",
                "data": arr
            }).done(function (json) {
                var arr = [];
                var cnt = 0;
                var positiveCnt = 0;
                var totalPercentage = 0;
                var i, j;
                for (j = 0; j < json.rows.length; j++) {
                    cnt = 0;
                    totalPercentage = 0;
                    positiveCnt = 0;
                    var year = 2018;
                    for (i = 1; i <= 12; i++) {
                        //console.log('percentage_' + (year - i) + '=', cFloat(json.rows[j]['percentage_' + (year - i)]))
                        var p = cFloat(json.rows[j]['percentage_' + (year - i)]);
                        if (p != 0) {
                            cnt += 1;
                            if (p > 0) {
                                positiveCnt += 1;
                            }
                            totalPercentage = cFloat(totalPercentage) + p;
                            //console.log('i=', i, 'totalPercentage=', totalPercentage)
                        }
                    }
                    var avg = cFloat((totalPercentage / cnt));
                    json.rows[j].avg = avg;
                    json.rows[j].positive_cnt = (positiveCnt / cnt) * 100;
                    var positive_percentage = cFloat($(":input[name='positive_percentage']", "#frmCompanySearch").val());
                    var avg = cFloat($(":input[name='avg']", "#frmCompanySearch").val());
                    console.log('positive_percentage=', positive_percentage, 'avg=', avg);
                    if (positive_percentage > 0 && avg > 0) {
                        if (json.rows[j].positive_cnt >= positive_percentage && json.rows[j].avg > avg) {
                            arr.push(json.rows[j]);
                        }
                    } else if (positive_percentage > 0) {
                        if (json.rows[j].positive_cnt >= positive_percentage) {
                            arr.push(json.rows[j]);
                        }
                    } else if (avg > 0) {
                        if (json.rows[j].avg >= avg) {
                            arr.push(json.rows[j]);
                        }
                    } else {
                        arr.push(json.rows[j]);
                    }
                }
                arr = arr.sort(getDescOrder('avg'));
                self.rows.removeAll();
                self.rows(arr);
                self.last_grid_data = arr;
                $(".manual-pagination", $Company).each(function () {
                    var element = this;
                    $(element).twbsPagination({
                        total: cInt(json.total),
                        rowsPerPage: cInt($(":input[name='rows']", $Company).val()),
                        startPage: cInt($(":input[name='page_index']", $Company).val()),
                        currentPage: cInt($(":input[name='page_index']", $Company).val()),
                        onRender: function (status) {
                            $("#paging_status", $Company).html(status);
                        },
                        onPageClick: function (page) {
                            $(":input[name='page_index']", $Company).val(page);
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

        this.exportGrid = function () {
            var $Company = $("#Company");
            var arr = $("#frmCompanySearch").serializeArray();
            arr.push({ "name": "PageSize", "value": 0 });
            arr.push({ "name": "PageIndex", "value": 0 });
            arr.push({ "name": "SortName", "value": $(":input[name='sort_name']", $Company).val() });
            arr.push({ "name": "SortOrder", "value": $(":input[name='sort_order']", $Company).val() });
            var isarchive = $("#frmCompanySearch #is_archive")[0].checked;
            if (isarchive == true) {
                arr[arr.length] = { "name": "is_archive", "value": isarchive };
            }
            var is_book_mark = $("#frmCompanySearch #is_book_mark")[0].checked;
            if (is_book_mark == true) {
                arr[arr.length] = { "name": "is_book_mark", "value": is_book_mark };
            }
            arr[arr.length] = { "name": "is_export_excel", "value": true };
            var url = apiUrl("/Company/Export?t=1");
            $.each(arr, function (i, p) {
                url += "&" + p.name + "=" + p.value;
            });
            var width = 300; var height = 200; var left = (screen.availWidth / 2) - (width / 2); var top = (screen.availHeight / 2) - (height / 2); var features = "width=" + width + ",height=" + height + ",left=" + left + ",top=" + top + ",location=no,menubar=no,toobar=no,scrollbars=yes,resizable=yes,status=yes";
            window.open(url, "downloadexcelfile", features);
        }

        this.applyPlugins = function () {
            var $frmCompanySearch = $("#frmCompanySearch");
            var $symbols = $(":input[name='symbols']", $frmCompanySearch);
            var $categories = $(":input[name='categories']", $frmCompanySearch);
            var $mf_ids = $(":input[name='mf_ids']", $frmCompanySearch);
            select2Setup($symbols[0], {
                multiple: true
                , width: 300
               , url: apiUrl("/Company/Select")
               , placeholder: "Select Company"
               , resultsCallBack: function (data, page) {
                   var s2data = [];
                   $.each(data, function (i, d) {
                       s2data.push({ "id": d.id, "text": d.label, "source": d });
                   });
                   return { results: s2data };
               }
               , onParam: function (term, page) {
                   return {
                       term: term,
                       categories: $categories.val()
                   };
               }
               , onChange: function (e, ui) {
                   var $target = $(".page-content");
                   self.loadGrid();
               }
            });

            select2Setup($categories[0], {
                multiple: true
               , width: 300
               , url: apiUrl("/Company/SelectCategories")
               , placeholder: "Select Categories"
               , resultsCallBack: function (data, page) {
                   var s2data = [];
                   $.each(data, function (i, d) {
                       s2data.push({ "id": d.id, "text": d.label, "source": d });
                   });
                   return { results: s2data };
               }
               , onParam: function (term, page) {
                   return {
                       term: term
                   };
               }
               , onChange: function (e, ui) {
                   var $target = $(".page-content");
                   self.loadGrid();
               }
            });

            select2Setup($mf_ids[0], {
                multiple: true
               , width: 300
               , url: apiUrl("/Company/SelectMFS")
               , placeholder: "Select MFS"
               , resultsCallBack: function (data, page) {
                   var s2data = [];
                   $.each(data, function (i, d) {
                       s2data.push({ "id": d.id, "text": d.label, "source": d });
                   });
                   return { results: s2data };
               }
               , onParam: function (term, page) {
                   return {
                       term: term
                   };
               }
               , onChange: function (e, ui) {
                   var $target = $(".page-content");
                   self.loadGrid();
               }
            });

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
                    'company_name': ''
                    , 'symbol': ''
                    , 'category_name': ''
                    , 'id': 0
                    , 'category_list': []
                };
            }
            ko.applyBindings(row, $modal[0]);

            var $frm = $("form", $modal);
            var $categories = $(":input[name='category_name']", $frm);
            select2Setup($categories[0], {
                multiple: true
              , width: 300
              , url: apiUrl("/Company/SelectCategories")
              , placeholder: "Select Categories"
              , resultsCallBack: function (data, page) {
                  var s2data = [];
                  $.each(data, function (i, d) {
                      s2data.push({ "id": d.id, "text": d.label, "source": d });
                  });
                  return { results: s2data };
              }
              , onParam: function (term, page) {
                  return {
                      term: term
                  };
              }
              , onChange: function (e, ui) {
              }
            });
            var arr = [];
            $.each(row.category_list, function (i, cat) {
                arr.push({ "id": cat, "text": cat });
            });
            $categories.select2Refresh("data", arr);
            var $btn = $("#save", $frm);
            $btn.click(function () {
                if ($frm.valid()) {
                    $btn.button('loading');
                    var data = $frm.serializeArray();
                    data.push({ "name": "company_id", "value": getGlobalCompanyID() });
                    var url = apiUrl("/Company/Create");
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
                        //self.loadGrid();
                    }).fail(function (jqxhr) {
                        alertErrorMessage(jqxhr.responseJSON);
                    }).always(function (jqxhr) {
                        $btn.button('reset');
                        unblockUI();
                    });
                }
            });

        }

        this.openLog = function () {
            $('#temp-investment-modal-container').remove();
            var $cnt = $("<div id='temp-investment-modal-container'></div>");
            $('body').append($cnt);
            var data = {
                "name": "Investment"
                , "title": "Investment"
                , "is_modal_full": false
                , "position": "top"
                , "width": $(window).width() - 100
            };
            $("#modal-investment-template").tmpl(data).appendTo($cnt);
            var $modal = $("#modal-investment-" + data.name, $cnt);
            $modal.modal('show');
            var $btn = $("#btn", $modal);
            $btn.unbind('click').click(function () {
                var investment = cFloat($(":input[name='investment']", $modal).val());
                var totalInv = cFloat($(":input[name='investment']", $modal).val());
                var data = [];
                var rows = self.last_grid_data;
                var z;
                var startYear = 2007;
                for (z = 0; z < 12; z++) {
                    var year = startYear + z;
                    var totalEquity = 0;
                    var i;
                    var $CompanyTable = $("#CompanyTable");
                    $("tbody > tr", $CompanyTable).each(function () {
                        var symbol = $("#symbol", this).val();
                        var $tr = $(this);
                        var $chk = $("#chk", $tr);
                        if ($chk[0]) {
                            if ($chk[0].checked) {
                                for (i = 0; i < rows.length; i++) {
                                    console.log('symbol=', symbol);
                                    if (rows[i].symbol == symbol) {
                                        var p = cFloat(rows[i]['percentage_' + year]);
                                        console.log('symbol=', rows[i].symbol, 'p=', p);
                                        if (p != 0) {
                                            totalEquity += 1;
                                        }
                                    }
                                }
                            }
                        }
                    });

                    console.log('totalEquity=', totalEquity);
                    if (totalEquity > 0) {
                        //if (z > 0) {
                        //    investment = cFloat(investment) + 240000;
                        //}
                        var investmentPerEquity = cFloat(investment) / cFloat(totalEquity);
                        if (investmentPerEquity > 0) {
                            var total = 0;

                            $("tbody > tr", $CompanyTable).each(function () {
                                var symbol = $("#symbol", this).val();
                                var $tr = $(this);
                                var $chk = $("#chk", $tr);
                                if ($chk[0]) {
                                    if ($chk[0].checked) {
                                        for (i = 0; i < rows.length; i++) {
                                            if (rows[i].symbol == symbol) {
                                                var p = cFloat(rows[i]['percentage_' + year]);
                                                if (p != 0) {
                                                    var eqp = cFloat(investmentPerEquity) + ((cFloat(investmentPerEquity) * p) / 100);
                                                    console.log('symbol=', rows[i].symbol, 'eqp=', eqp);
                                                    total = total + eqp;
                                                }
                                            }
                                        }
                                    }
                                }
                            });
                            var totalPercentage = ((cFloat(total) - cFloat(investment)) / cFloat(investment)) * 100;
                            console.log('totalEquity=', totalEquity, 'investmentPerEquity=', investmentPerEquity, 'investment=', investment, 'total=', total, 'totalPercentage=', totalPercentage);
                            data.push({
                                'year': year, 'total_equity': totalEquity, 'investment': investment, 'cmv': total, 'percentage': totalPercentage
                            });
                            investment = cFloat(total);
                        } else {
                            totalEquity = totalEquity - 1;
                        }
                    }
                }
                var $detailTable = $("#detailTable", $modal);
                var d = { 'rows': data };
                var finalAmount = 0;
                if (data.length > 0) {
                    finalAmount = data[data.length - 1].cmv;
                }
                if (finalAmount > 0) {
                    d.investment = totalInv; // cFloat($(":input[name='investment']", $modal).val());
                    d.final_amount = finalAmount;
                    var totalFinalPercentage = ((cFloat(d.final_amount) - cFloat(d.investment)) / cFloat(d.investment)) * 100;
                    d.total_final_percentage = totalFinalPercentage;
                }
                console.log('d=', d);
                $("#detail-template").tmpl(d).appendTo($detailTable);
            });
        }

        this.onElements = function () {
            self.offElements();
            $("body").on("click", "#frmCompanySearch #is_archive", function (event) {
                self.loadGrid();
            });
            $("body").on("click", "#frmCompanySearch #is_book_mark", function (event) {
                self.loadGrid();
            });
            $("body").on("click", "#frmCompanySearch #is_nifty_50", function (event) {
                self.loadGrid();
            });
            $("body").on("click", "#frmCompanySearch #is_nifty_100", function (event) {
                self.loadGrid();
            });
            $("body").on("click", "#frmCompanySearch #is_nifty_200", function (event) {
                self.loadGrid();
            });
            $("body").on("click", "#frmCompanySearch #is_old", function (event) {
                self.loadGrid();
            });
            $("body").on("click", "#frmCompanySearch #is_all_time_low", function (event) {
                self.loadGrid();
            });
            $("body").on("click", "#frmCompanySearch #is_all_time_high", function (event) {
                self.loadGrid();
            });
            $("body").on("click", "#frmCompanySearch #is_all_time_low_15_days", function (event) {
                self.loadGrid();
            });
            $("body").on("click", "#frmCompanySearch #is_all_time_high_15_days", function (event) {
                self.loadGrid();
            });
            $("body").on("click", "#frmCompanySearch #is_all_time_low_5_days", function (event) {
                self.loadGrid();
            });
            $("body").on("click", "#frmCompanySearch #is_all_time_high_5_days", function (event) {
                self.loadGrid();
            });
            $("body").on("click", "#frmCompanySearch #is_all_time_low_2_days", function (event) {
                self.loadGrid();
            });
            $("body").on("click", "#frmCompanySearch #is_all_time_high_2_days", function (event) {
                self.loadGrid();
            });
            $("body").on("click", "#frmCompanySearch #is_sell_to_buy", function (event) {
                self.loadGrid();
            });
            $("body").on("click", "#frmCompanySearch #is_buy_to_sell", function (event) {
                self.loadGrid();
            });
            $("body").on("click", "#frmCompanySearch #is_mf", function (event) {
                self.loadGrid();
            });
            $("body").on("click", "#Company .btn-add", function (event) {
                self.openItem(null)
            });
            $("body").on("click", "#Company .btn-export", function (event) {
                self.exportGrid()
            });
            $("body").on("click", "#Company .btn-edit", function (event) {
                self.openItem(ko.dataFor(this));
            });
            $("body").on("click", "#Company .btn-delete", function (event) {
                var dataFor = ko.dataFor(this);
                jConfirm({
                    "message": "Are you sure?", "ok": function () {
                        var url = apiUrl("/Company/Delete/" + dataFor.id);
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
            $("body").on("change", "#Company #rows", function (event) {
                self.loadGrid();
            });
            $("body").on("click", ".is-book-mark", function (event) {
                var $this = $(this);
                var $i = $("i", $this);
                var dataFor = ko.dataFor(this);
                var url = apiUrl('/Company/UpdateArchive');
                var arr = [];
                var isArchive = $i.hasClass('fa-bookmark');
                arr.push({ "name": "symbol", "value": dataFor.symbol });
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
            $("body").on("click", ".is-current-stock", function (event) {
                var $this = $(this);
                var $i = $("i", $this);
                var dataFor = ko.dataFor(this);
                var url = apiUrl('/Company/UpdateBookMark');
                var arr = [];
                var is_book_mark = $i.hasClass('fa-user');
                arr.push({ "name": "symbol", "value": dataFor.symbol });
                arr.push({ "name": "is_book_mark", "value": !is_book_mark });
                $i.removeClass('fa-user').removeClass('fa-user-o').removeClass('fg-primary');
                if (is_book_mark == true) {
                    $i.addClass('fa-user-o');
                } else {
                    $i.addClass('fa-user fg-primary');
                }
                $.ajax({
                    "url": url,
                    "cache": false,
                    "type": "POST",
                    "data": arr
                }).done(function (json) {
                });
            });
            $("body").on("click", "#chkSelectAll", function (event) {
                var that = this;
                $(".chk-symbol").each(function () {
                    this.checked = that.checked;
                });
            });
        }

        this.offElements = function () {
            $("body").off("click", "#frmCompanySearch #is_archive");
            $("body").off("click", "#frmCompanySearch #is_book_mark");
            $("body").off("click", "#frmCompanySearch #is_nifty_50");
            $("body").off("click", "#frmCompanySearch #is_nifty_100");
            $("body").off("click", "#frmCompanySearch #is_nifty_200");
            $("body").off("click", "#frmCompanySearch #is_old");
            $("body").off("click", "#frmCompanySearch #is_all_time_low");
            $("body").off("click", "#frmCompanySearch #is_all_time_high");
            $("body").off("click", "#frmCompanySearch #is_all_time_low_15_days");
            $("body").off("click", "#frmCompanySearch #is_all_time_high_15_days");
            $("body").off("click", "#frmCompanySearch #is_all_time_low_5_days");
            $("body").off("click", "#frmCompanySearch #is_all_time_high_5_days");
            $("body").off("click", "#frmCompanySearch #is_all_time_low_2_days");
            $("body").off("click", "#frmCompanySearch #is_all_time_high_2_days");
            $("body").off("click", "#frmCompanySearch #is_sell_to_buy");
            $("body").off("click", "#frmCompanySearch #is_buy_to_sell");
            $("body").off("click", "#frmCompanySearch #is_mf");
            $("body").off("click", "#Company .btn-add");
            $("body").off("click", "#Company .btn-edit");
            $("body").off("click", "#Company .btn-delete");
            $("body").off("change", "#Company #rows");
            $("body").off("click", ".is-book-mark");
            $("body").off("click", ".is-current-stock");
            $("body").off("click", "#Company .btn-export");
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

                var $Company = $("#Company");
                var $tbl = $("#CompanyTable", $Company);
                var $sort_name = $(":input[name='sort_name']", $Company);
                var $sort_order = $(":input[name='sort_order']", $Company);
                $tbl.attr("sortname", $sort_name.val());
                $tbl.attr("sortorder", $sort_order.val());
                helper.sortingTable($tbl[0], {
                    "onSorting": function (sortname, sortorder) {
                        $(":input[name='sort_name']", $Company).val(sortname);
                        $(":input[name='sort_order']", $Company).val(sortorder);
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
