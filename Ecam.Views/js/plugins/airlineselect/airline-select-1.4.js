(function ($) {
    $.addAirlineSelect = function (hdn, p) {
        if (hdn.g) { return false; }
        p = $.extend({
            onChange: null,
            multiple: true,
            is_iatacode: "false",
            logoLimit: 3,
            width: 370,
            is_fullcontainer: "false",
            is_check_our: false,
            is_show_blocked_airline: "",
            onParams: []
        }, p);
        var g = {
            cnt: null
            , childcnt: null
            , json: null
            , setLogoLimit: function () {
                var $hdn = $(hdn);
                if (p.is_fullcontainer == "true") {
                    var parentWidth = $hdn.parent().width();
                    p.logoLimit = Math.ceil(parentWidth / 115) - 1;
                    //console.log('parentWidth=', parentWidth, 'p.logoLimit=', p.logoLimit);
                    p.width = parentWidth;
                }
            }
            , init: function () {
                var $hdn = $(hdn);
                if (g.cnt) {
                    g.cnt.remove();
                }
                g.setLogoLimit();
                if (p.width < 370)
                    p.width = 370;

                //console.log('width=', p.width);
                var html = "<div class='air-sel-cnt' " + (cInt(p.width) > 0 ? "style='width:" + p.width + "px'" : "") + ">";
                html += "<div class='no-airline'></div></div>";
                g.cnt = $(html);
                g.childcnt = $("<div class='child-air-sel-cnt close-box'></div>");
                $hdn.before(g.cnt);
                $(".no-airline", g.cnt).css({ "font-size": "13px" }).html("Loading...");

                g.getAirlines(function (data) {
                    g.json = null;
                    var role = getAuth().role;
                    //console.log('is_check_our=', p.is_check_our);
                    if (p.is_check_our == true && (role == 'AA' || role == 'AM')) {
                        g.json = [];
                        //console.log('Airline Select _AGENT_OUR_AIRLINE_CODES=', _AGENT_OUR_AIRLINE_CODES);
                        if (_AGENT_OUR_AIRLINE_CODES.length > 0) {
                            var i = 0;
                            for (i = 0; i < _AGENT_OUR_AIRLINE_CODES.length; i++) {
                                $.each(data, function (j, row) {
                                    if (row.id == _AGENT_OUR_AIRLINE_CODES[i]) {
                                        g.json.push(row);
                                    }
                                });
                            }
                        }
                    } else {
                        g.json = data;
                    }
                    var divLimit = p.logoLimit;
                    var isDisPlayDownArrow = false;
                    if (g.json.length > p.logoLimit) {
                        isDisPlayDownArrow = true;
                    }
                    if (g.json.length <= 0) {
                        $(".no-airline", g.cnt).html("No Airlines");
                    } else {
                        $(".no-airline", g.cnt).remove();
                    }
                    //console.log('g.json=', g.json);
                    $.each(g.json, function (i, row) {
                        var $div = $("<div class='air-row'></div>");
                        //loading img
                        $div.attr("airline_code", row.id);
                        $div.attr("airline_code", row.other3);
                        var result = "";
                        if ($.trim(row.other) == "") {
                            result = "<span class='spn-iata-code'>" + row.label.split("-")[0] + "</span>";
                        } else {
                            var imgUrl = '';
                            row.other = row.other.replace(new RegExp(/\\/g), '/');
                            var ext = row.other.split('.').pop();
                            var fileName = row.other.split(".").slice(0, -1);
                            var src = fileName + '_medium' + '.' + ext;
                            imgUrl = siteUrl("/" + src);
                            var arr = row.label.split(" - ");
                            if (arr.length > 0)
                                result = '<img class="airline-select-logo" src="' + imgUrl + '" alt="' + arr[0] + '" />';
                        }
                        $div.html(result);
                        if (i < divLimit) {
                            $div.addClass("is-parent");
                            g.cnt.append($div);
                        } else {
                            $div.addClass("is-child");
                            g.childcnt.append($div);
                        }
                        if (cInt(i) == (divLimit - 1) && isDisPlayDownArrow) {
                            //click downarrow
                            var $div = $("<div class='air-arrow'></div>");
                            var row = ({ "id": "0" });
                            $div.data("row", row);
                            var result = "<img class='air-arrow-image' src='../../img/down.png' />";
                            $div.html(result);
                            g.cnt.append($div);
                        }
                    });

                    g.cnt.append(g.childcnt);
                    $(".air-arrow", g.cnt).unbind('click').click(function () {
                        g.childcnt.toggleClass("close-box");
                    });
                    g.bindEvents();
                });

                g.cnt.append("<div class='clearfix'></div>");
                g.childcnt.append("<div class='clearfix'></div>");
                g.cnt.data("hdn", hdn);
            }
            , redraw: function () {
                //console.log('redraw 1');
                g.setLogoLimit();
                var totalAirlines = $(".air-row.is-parent", g.cnt).length;
                //console.log('totalAirlines=', totalAirlines);
                var diff = totalAirlines - p.logoLimit;
                //console.log('diff=', diff);
                //console.log('p.width=', p.width, 'logoLimit=', p.logoLimit);
                g.cnt.width(p.width);
                //console.log('redraw 2');
                var isAdd = false;
                if (diff <= 0) {
                    diff = diff * -1;
                    isAdd = true;
                }
                var $childBox = $(".child-air-sel-cnt");
                var i;
                var $row;
                var $lastRow;
                //console.log('diff=', diff);
                for (i = 0; i < diff; i++) {
                    $row = null;
                    $lastRow = null;
                    if (isAdd == true) {
                        $row = $(".air-row.is-child:first", $childBox);
                        $lastRow = $(".air-row.is-parent:last", g.cnt);
                        g.addClassParent($row);
                    } else {
                        $row = $(".air-row.is-parent:last", g.cnt);
                        $lastRow = $(".air-row.is-child:last", $childBox);
                        g.addClassChild($row);
                    }
                    if ($row[0]) {
                        if ($lastRow[0]) {
                            $lastRow.after($row);
                        } else {
                            if (isAdd == true)
                                g.cnt.append($row);
                            else
                                $childBox.append($row);
                        }
                    }
                }
                g.bindEvents();
            }
            , checkAirlineBlockWithCompany: function (companyId, airlineCode) {
                var isresult = false;
                var cnt = 0;
                var companyAirlines = null;
                if (_USER_AUTHORIZE_CACHE != null) {
                    if (_USER_AUTHORIZE_CACHE.CompanyAirlines != null) {
                        companyAirlines = _USER_AUTHORIZE_CACHE.CompanyAirlines;
                    }
                }
                //console.log('companyAirlines=', companyAirlines)
                if (companyAirlines != null) {
                    $.each(companyAirlines, function (i, ca) {
                        if (cInt(ca.company_id) == companyId && ca.airline_code == airlineCode) {
                            var isblock = false;
                            if (_USER_AUTHORIZE_CACHE != null) {
                                if (_USER_AUTHORIZE_CACHE.BlockedAirlines != null) {
                                    var blockedAirlines = _USER_AUTHORIZE_CACHE.BlockedAirlines;
                                    $.each(blockedAirlines, function (i, row) {
                                        if (cInt(row.company_id) == cInt(ca.company_id)) {
                                            isblock = true;
                                        }
                                    });
                                }
                            }
                            //console.log('isblock=', isblock, 'airlinecode=', airlineCode);
                            if (isblock == false) {
                                cnt += 1;
                            }
                        }
                    });
                }
                //console.log('airlineCode=', airlineCode, 'cnt=', cnt);
                if (cnt > 0) {
                    isresult = true;
                }
                return isresult;
            }
            , getAirlines: function (callback) {
                var arr = [];
                if (p.onParams) {
                    arr = p.onParams();
                }
                arr.push({ "name": "is_check_cache", "value": "true" });
                var role = '';
                var auth = getAuth();
                if (auth != null) {
                    role = auth.role;
                }
                //console.log(1);
                if (role != 'EA' && role != 'EM' && role != 'GA') {
                    //console.log(2);
                    var airlines = searchCompanyAirlineCache(arr);
                    //console.log("airlines :", airlines);
                    //console.log("p.is_show_blocked_airline:", p.is_show_blocked_airline);
                    //console.log("BlockedAirlines:", _USER_AUTHORIZE_CACHE.BlockedAirlines);
                    if (p.is_show_blocked_airline == "") {
                        var json = [];
                        if (_USER_AUTHORIZE_CACHE != null) {
                            if (_USER_AUTHORIZE_CACHE.BlockedAirlines != null) {
                                var blockedAirlines = _USER_AUTHORIZE_CACHE.BlockedAirlines;
                                var companyId = getGlobalCompanyID();
                                $.each(blockedAirlines, function (i, row) {
                                    if (role == 'AA' || role == 'AM') {
                                        var checkAirline = g.checkAirlineBlockWithCompany(row.company_id, row.airline_code);
                                        if (checkAirline == true) {
                                            json.push({ "airline_code": row.airline_code });
                                        }
                                    } else {
                                        if (companyId == row.company_id) {
                                            json.push({ "airline_code": row.airline_code });
                                        }
                                    }
                                });
                            }
                        }
                        //console.log('json=', json);
                        if (json) {
                            $.each(json, function (i, row) {
                                //console.log('row airline code=', row.airline_code);
                                airlines = airlines.filter(function (e) {
                                    return e.id != row.airline_code;
                                })
                            });
                        }
                    }
                    if (callback)
                        callback(airlines);
                } else {
                    //console.log(3);
                    var url = apiUrl("/Airline/Select");
                    $.ajax({
                        "url": url,
                        "cache": false,
                        "data": arr,
                        "type": "GET"
                    }).done(function (json) {
                        //console.log(4);
                        if (callback)
                            callback(json);
                    });
                }
            }
            , reArrange: function ($this) {
                var $childBox = $(".child-air-sel-cnt", g.cnt);
                //$(".child-air-sel-cnt > .air-row.select",g.cnt).each(function() {
                //var $this=$(this);
                var $firstParent = $(".air-row.is-parent:first", g.cnt);
                var $lastParent = $(".air-row.is-parent:last", g.cnt);
                if ($lastParent[0]) {
                    $firstParent.before($this);
                    g.addClassParent($this);
                    $childBox.append($lastParent);
                    g.addClassChild($lastParent);
                }
                //});
                g.bindEvents();
            }
            , removeCSS: function ($this) {
                $this.removeClass("is-parent").removeClass("is-child");
            }
            , addClassParent: function ($this) {
                g.removeCSS($this);
                $this.addClass("is-parent");
            }
            , addClassChild: function ($this) {
                g.removeCSS($this);
                $this.addClass("is-child");
            }
            , bindEvents: function () {
                $(".air-row", g.cnt).unbind('click').click(function (e) {
                    var $this = $(this);
                    var $child = $this.parents(".child-air-sel-cnt:first");
                    if (p.multiple == false) {
                        if ($child[0]) {
                            $(".air-row.is-parent", g.cnt).removeClass("select");
                        } else {
                            $(".air-row.is-child", g.cnt).removeClass("select");
                        }
                        $this.siblings().removeClass("select");
                    }
                    $this.toggleClass("select");
                    if ($child[0]) {
                        g.reArrange($this);
                    }
                    g.childcnt.addClass("close-box");
                    g.addValues();
                    if (p.onChange)
                        p.onChange(e);
                });
            }
            , addValues: function () {
                var ids = "";
                $(".air-row", g.cnt).each(function () {
                    var $this = $(this);
                    var row = $this.data("row");
                    if ($this.hasClass("select")) {
                        if (p.is_iatacode == "true") {
                            ids += cString($this.attr("airline_code")) + ",";
                        } else {
                            ids += cString($this.attr("airline_code")) + ",";
                        }
                    }
                });
                if (ids != "") {
                    ids = ids.substring(0, ids.length - 1);
                }
                $(hdn).val(ids);
            }
            , applyValues: function (ids) {
                $(".air-row", g.cnt).removeClass("select");
                var arr = ids.split(",");
                $.each(arr, function (i, strid) {
                    $(".air-row[airline_code='" + strid + "']", g.cnt).addClass("select");
                });
                $(".child-air-sel-cnt .air-row.select").each(function () {
                    g.reArrange($(this));
                });
                g.addValues();
            }
        };

        $(hdn).addClass('is-img-airline-hdn');
        hdn.g = g;
        hdn.p = p;
        g.init();
        return hdn;
    };
    var docloaded = false;
    $(document).ready(function () { docloaded = true });
    $.fn.airlineSelect = function (p) {
        return this.each(function () {
            if (!docloaded) {
                var hdn = this;
                $(document).ready
					(
						function () {
						    $.addAirlineSelect(hdn, p);
						}
					);
            } else {
                $.addAirlineSelect(this, p);
            }
        });
    };
    $(document).click(function (e) {
        var $parent = $(e.target).parents(".air-sel-cnt:first");
        if (!$parent[0]) {
            $(".air-sel-cnt > .child-air-sel-cnt").addClass("close-box");
        }
    });
})(jQuery);
