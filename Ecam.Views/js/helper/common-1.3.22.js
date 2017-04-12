"use strict";
// trimming with array ops
String.prototype.trim = function () { return this.split(/\s/).join(' '); }

// string replicator 
String.prototype.times = function (n) { var s = ''; var i; for (i = 0; i < n; i++) s += this; return s; }

// zero padding and trailing
String.prototype.zp = function (n) { return '0'.times(n - this.length) + this; }
String.prototype.zt = function (n) { return this + '0'.times(n - this.length); }

// string reverse
String.prototype.reverse = function () { return this.split('').reverse().join(''); }

// clear format from a string representation of a number
String.prototype.clean = function () { return parseFloat(this.replace(/[^0-9|.|-]/g, '')); }

String.prototype.replaceAll = function (str1, str2, ignore) {
    return this.replace(new RegExp(str1.replace(/([\/\,\!\\\^\$\{\}\[\]\(\)\.\*\+\?\|\<\>\-\&])/g, "\\$&"), (ignore ? "gi" : "g")), (typeof (str2) == "string") ? str2.replace(/\$/g, "$$$$") : str2);
};

if (!String.prototype.startsWith) {
    String.prototype.startsWith = function (searchString, position) {
        position = position || 0;
        return this.lastIndexOf(searchString, position) === position;
    };
}
String.prototype.isExist = function (str, ignoreCase) {
    return (ignoreCase ? this.toUpperCase() : this).indexOf(ignoreCase ? str.toUpperCase() : str) >= 0;
};

//Array.prototype.getArrayUnique=function() {
//    var u={},a=[];
//    for(var i=0,l=this.length;i<l;++i) {
//        if(u.hasOwnProperty(this[i])) {
//            continue;
//        }
//        a.push(this[i]);
//        u[this[i]]=1;
//    }
//    return a;
//}



function AjaxDownloadTempFile(url, fileName, contentType) {
    var $iframe,
        iframe_doc,
        iframe_html;

    //if(($iframe=$('#download_iframe')).length===0) {
    var rnd = Math.floor((Math.random() * 1000) + 1);
    $iframe = $("<iframe id='download_iframe_" + rnd + "'" +
                " style='display: none' src='about:blank'></iframe>"
               ).appendTo("body");
    //}

    iframe_doc = $iframe[0].contentWindow || $iframe[0].contentDocument;
    if (iframe_doc.document) {
        iframe_doc = iframe_doc.document;
    }

    iframe_html = "<html><head></head><body><form method='POST' action='" +
                  url + "'>" +
                  "<input type=hidden name='file_name' value='" + fileName + "'/><input type=hidden name='content_type' value='" + contentType + "'/><button type='submit'>Submit</button></form>" +
                  "</body></html>";

    iframe_doc.open();
    iframe_doc.write(iframe_html);
    $(iframe_doc).find('form').submit();
    setTimeout(function () {
        $iframe.remove();
    }, 10000);
}

function getArrayUnique(arr) {
    var u = {}, a = [];
    for (var i = 0, l = arr.length; i < l; ++i) {
        if (u.hasOwnProperty(arr[i])) {
            continue;
        }
        a.push(arr[i]);
        u[arr[i]] = 1;
    }
    return a;
}

function findMaxNo(array) {
    var max = 0;
    var a = array.length;
    for (counter = 0; counter < a; counter++) {
        if (array[counter] > max) {
            max = array[counter];
        }
    }
    return max;
}

function findMinNo(array) {
    var min = array[0];
    var a = array.length;
    for (counter = 0; counter < a; counter++) {
        if (array[counter] < min) {
            min = array[counter];
        }
    }
    return min;
}

function randomNumber(minimum, maximum) {
    return Math.round(Math.random() * (maximum - minimum) + minimum);
}



function getSortOrder(prop) {
    return function (a, b) {
        //console.log('prop=', prop, 'a[prop]=', a[prop], 'b[prop]=', b[prop]);
        if (a[prop] > b[prop]) {
            return 1;
        } else if (a[prop] < b[prop]) {
            return -1;
        }
        return 0;
    }
}

function getDescOrder(prop) {
    return function (a, b) {
        //console.log('prop=', prop, 'a[prop]=', a[prop], 'b[prop]=', b[prop]);
        if (a[prop] > b[prop]) {
            return -1;
        } else if (a[prop] < b[prop]) {
            return 1;
        }
        return 0;
    }
}


function PropertyModel(_dname, _pname) {
    this.display_name = _dname;
    this.property_name = _pname;
}
function ForexDataGroup() {
    var self = this;
    this.values = [];
    this.id = 0;
    this.from_date = "";
}

Number.prototype.zp = function (n) { return this.toString().zp(n); }

var _COMPANY_CACHE = null;
var _COUNTRY_CACHE = null;
var _ZONE_CACHE = null;
var _FLIGHT_TYPE_CACHE = null;
var _COMMODITY_TYPE_CACHE = null;
var _USER_AUTHORIZE_CACHE = null;
var _USER_GROUP_IDS = null;
var _GLOBAL_COMPANY_EVENTS = [];
var _SET_MY_EVENTS = [];
var _AGENT_OUR_AIRLINE_CODES = [];
var _ECAMAPP = null;
$.extend(window, {
    cFloat: function (value) {
        //return SH(v).toFloat();
        if (typeof value === "number") return value;
        var decimal = ".";
        var regex = new RegExp("[^0-9-" + decimal + "]", ["g"]),
			unformatted = parseFloat(
				("" + value)
				.replace(/\((.*)\)/, "-$1")
				.replace(regex, '')
				.replace(decimal, '.')
			);
        return !isNaN(unformatted) ? unformatted : 0;
    }
	, cInt: function (value) {
	    //return SH(v).toInt();
	    if (typeof value === "number") return parseInt(value);
	    var decimal = ".";
	    var regex = new RegExp("[^0-9-" + decimal + "]", ["g"]),
			unformatted = parseInt(
				("" + value)
				.replace(/\((.*)\)/, "-$1")
				.replace(regex, '')
				.replace(decimal, '.')
			);
	    return !isNaN(unformatted) ? unformatted : 0;
	}
    , cString: function (v) {
        var result = SH(v).s;
        return (result == "null" ? "" : result);
    }
    , cDateParse: function (v) {
        var oval = v;
        var isMonthExist = false;
        v = SH(v).replaceAll("-", "/").s;
        var spaceArr = v.split(" ");
        var splitValue = $.trim(spaceArr[0]);
        var timeValue = "";
        if (spaceArr.length > 1) {
            timeValue = $.trim(spaceArr[1]);
        }
        var arr = splitValue.toString().split("/");
        //console.log('cDateParse arr=',arr);
        if (arr.length >= 3) {
            var months = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
            if (SH(arr[1]).isNumeric() == false) {
                var i;
                var d = arr[0];
                var m = '';
                var y = arr[2];
                for (i = 1; i <= months.length; i++) {
                    if (months[i - 1].toLowerCase() == arr[1].toLowerCase()) {
                        isMonthExist = true;
                        m = ((i < 10) ? "0" + i : i);
                    }
                }
                //console.log('d=',d,'m=',m,'y=',y);
                v = y + "/" + m + "/" + d;
                if (timeValue != '') {
                    v += " " + timeValue;
                }
            }
        }
        if (isMonthExist == false)
            v = oval;
        return v;
    }
    , cDateObj: function (v) {
        //console.log('cDateObj v=',v);
        var result = new Date(cDateParse(v));
        //console.log('cDateObj result=',result);
        return result;
    }
    , cDateString: function (v) {
        if (v.toString().indexOf('GMT+') > 0) {
            return v;
        } else {
            //console.log('cDateString Start v=',v);
            v = cDateParse(v);
            v = cString(moment(v).format("YYYY-MM-DD"));
            if ($.trim(v).toString().toLowerCase() == 'invalid date') { v = ''; }
            //console.log('cDateString v=',v);
            return v;
        }
    }
    , cBool: function (v) {
        return SH(v).toBoolean();
    }
    , getScaleAndPrecision: function (x) {
        x = parseFloat(x) + "";
        var arr = x.split(".");
        var d = {
            scale: cFloat(arr[0]),
            precision: cFloat(arr[1])
        };
        return d;
    }
    , formatCurrency: function (d, decimalPlace) {        
        var precision = cFloat(decimalPlace);
        if (precision <= 0) {
            precision = 2;
        }
        d = cFloat(d);
        if (d == 0) {
            return "";
        } else {
            return accounting.formatMoney(d, {
                precision: precision
            });
        }
    }
	, formatPercentage: function (d) {
	    d = cFloat(d);
	    if (d == 0) {
	        return "";
	    } else {
	        var precision = 2;
	        var pre = getScaleAndPrecision(d);
	        if (pre.precision == 0)
	            precision = 0;

	        return accounting.formatNumber(d, {
	            precision: precision
	        }) + "%";
	    }
	}
    , getCurrentURL: function () {
        return window.location.href;
    }
    , formatCurrentURL: function () {
        var url = '';
        var currentUrl = window.location.href;
        //url=SH(url).replaceAll(SITE_URL,'').replaceAll('http','').replaceAll('www').replaceAll('/','').replaceAll('.','').s;
        var arr = currentUrl.split("/#/");
        if (arr.length > 1) {
            url = '/' + arr[1];
        }
        return url;
    }
    , getTimeDifference: function (earlierDate, laterDate) {
        var one_day = 1000 * 60 * 60 * 24;

        //console.log('getTimeDifference earlierDate=',earlierDate);
        //console.log('getTimeDifference laterDate=',laterDate);

        earlierDate = earlierDate ? new Date(earlierDate).getTime() : new Date().getTime();
        laterDate = laterDate ? new Date(laterDate).getTime() : new Date().getTime();

        //console.log('getTimeDifference1 earlierDate=',earlierDate);
        //console.log('getTimeDifference1 laterDate=',laterDate);

        var oDiff = {};
        var diff = laterDate - earlierDate;
        oDiff.totalDiff = diff;

        diff = cFloat(diff / 1000);
        oDiff.seconds = Math.floor(diff % 60);
        if (oDiff.seconds > 0)
            oDiff.seconds = oDiff.seconds > 10 ? oDiff.seconds : "0" + oDiff.seconds;

        diff = cFloat(diff / 60);
        oDiff.minutes = Math.floor(diff % 60);
        if (oDiff.minutes > 0)
            oDiff.minutes = oDiff.minutes > 10 ? oDiff.minutes : "0" + oDiff.minutes;

        diff = cFloat(diff / 60);
        oDiff.hours = Math.floor(diff % 24);
        if (oDiff.hours > 0)
            oDiff.hours = oDiff.hours > 10 ? oDiff.hours : "0" + oDiff.hours;

        diff = cFloat(diff / 24);
        oDiff.days = diff;

        //oDiff.days=oDiff.days>10?oDiff.days:cInt(oDiff.days);

        var millisec = cFloat(oDiff.totalDiff);

        var seconds = (millisec / 1000);

        var minutes = (millisec / (1000 * 60));

        var hours = (millisec / (1000 * 60 * 60));

        var days = (millisec / (1000 * 60 * 60 * 24));

        //console.log('days=', days);
        //console.log('hours=', hours);
        //console.log('minutes=', minutes);
        //console.log('seconds=', seconds);

        oDiff.totalMinutes = minutes; //(cInt(oDiff.hours) * 60) + cInt(oDiff.minutes);
        oDiff.totalSeconds = seconds; // oDiff.totalMinutes * 60 + cInt(oDiff.seconds);

        return oDiff;
    }
    , getLocalGMTOffset: function () {
        var s = 0;
        var timeDiff = getTimeDifference(moment.utc().format('YYYY/MM/DD HH:mm:ss'), moment().format('YYYY/MM/DD HH:mm:ss'));
        return timeDiff.totalSeconds;
    }
    , toLocalTime: function (d) {
        try {
            return moment.utc(d).toDate();
        } catch (e) {
            return '';
        }
    }
    , formatDateTime: function (d, f) {
        var m = moment(d);
        return m.format(f);
    }
    , formatDate: function (d, f) {
        if (cString(d) == "") {
            return "";
        }
        if (cString(f) == "") {
            f = "DD/MMM/YYYY";
        }
        var ty = $.type(d);
        if (ty == "object")
            return "";

        d = cDateString(d.toString());
        var m = moment(d);
        if (m.get('year') <= 1901)
            return "";
        else
            return m.format(f);
    }
    , formatInteger: function (d) {
        d = cFloat(d);
        if (d == 0) {
            return "";
        } else {
            return accounting.formatNumber(d, { precision: 0 });
        }
    }
    , formatAWBNo: function (v) {
        v = v.toString();
        if (v.length >= 11)
            return v.substring(0, 3) + " " + v.substring(3, 7) + " " + v.substring(7, v.length);
        else
            return "";
    }
    , formatAWBNo2: function (v) {
        v = v.toString();
        if (v.length >= 11)
            return v.substring(0, 3) + " " + v.substring(3, 7) + v.substring(7, v.length);
        else
            return "";
    }
    , formatAWBNo3: function (v) {
        v = v.toString();
        if (v.length >= 11)
            return v.substring(0, 3) + "-" + v.substring(3, 7) + v.substring(7, v.length);
        else
            return "";
    }
    , format24Hrs: function (v) {
        if (v.indexOf(':') > 0) {
            return v;
        }
        var time = "";
        if ($.type(parseInt(v)) == "number" && parseInt(v) > 0) {
            if (v.length > 4) {
                return "";
            }
            if (v.length == 3) {
                var temp = v.charAt(0) + v.charAt(1);
                if (cInt(temp) < 10) {
                    v = "0" + v;
                } else {
                    v = temp + "0" + v.charAt(2);
                }
            }
            if (v.length == 2) {
                if (cInt(v) > 24) {
                    return "";
                }
                v = v + "00";
            }
            if (v.length == 1) {
                v = "0" + v + "00";
            }
            if (v.length >= 3) {
                for (var i = 0; i < v.length; i++) {
                    if (i == 2) {
                        time += ":";
                    }
                    time += v.charAt(i);
                }
                if (time.length >= 4) {
                    var arr = time.split(":");
                    if (cInt(arr[0]) > 24) {
                        return "";
                    }
                }
            }
        }
        return time;
    }
    , GUIId: function () {
        var S4 = function () {
            return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1);
        }
        // then to call it, plus stitch in '4' in the third group
        return (S4() + S4() + "-" + S4() + "-4" + S4().substr(0, 3) + "-" + S4() + "-" + S4() + S4() + S4()).toLowerCase();
    }
	, formatNumber: function (d, decimalPlace) {
	    var precision = cFloat(decimalPlace);
	    if (precision <= 0) {
	        precision = 2;
	    }
	    d = cFloat(d);
	    if (d == 0) {
	        return "";
	    } else {
	        return accounting.formatNumber(d, { precision: precision, checkNegative: false });
	    }
	}
    , appendNRF: function (t) {
        var $thead = $("thead", t);
        var $tbody = $("tbody", t);
        $("tr.row-nrf", $tbody).remove();
        var rowLength = $("tr", $tbody).length;
        var columnLength = $("tr > th", $thead).length;
        if (rowLength <= 0) {
            $tbody.append($("<tr class='row-nrf'><td class='text-center' colspan='" + columnLength + "'>No records found</td></tr>"));
        }
    }
    , getCookie: function (key) {
        try {
            return $.cookieStorage.get(key);
        } catch (e) {
            //console.log('getCookie e=', e);
            return null;
        }
    }
    , setCookie: function (key, value) {
        return $.cookieStorage.set(key, value);
    }
    , getLS: function (key) {
        try {
            return $.localStorage.get(key);
        } catch (e) {
            //console.log('getLS e=', e);
            return null;
        }
    }
    , setLS: function (key, value) {
        $.localStorage.set(key, value);
    }
    , removeLS: function (key) {
        try { $.localStorage.remove(key); } catch (e) { }
    }
    , getSS: function (key) {
        try {
            return $.sessionStorage.get(key);
        } catch (e) {
            //console.log('getSS e=', e);
            return null;
        }
    }
    , setSS: function (key, value) {
        $.sessionStorage.set(key, value);
    }
    , setAuth: function (json) {
        var key = "auth";
        setLS(key, null);
        setSS(key, null);
        if (json != null) {
            setLS(key, json);
        }
    }
    , getAuth: function () {
        var key = "auth";
        var auth = getLS(key);
        return auth;
    }
    , getCurrentUserID: function () {
        var userId = 0;
        var auth = getAuth();
        if (auth != null) {
            userId = auth.id;
        }
        return userId;
    }
    , getIsCompanyActAgentRole: function () {
        var isResult = false;
        var auth = getAuth();
        var currentRole = auth.role;
        var arr = auth.user_roles.split(",");
        if (arr.length >= 2) {
            $.each(arr, function (i, ro) {
                if (ro != currentRole) {
                    if ((ro == 'CA' || ro == 'CM') && currentRole == 'AA') {
                        isResult = true; // Company Agent = true; (CA,AA) or (CM,AA);
                    }
                }
            });
        }
        return isResult;
    }
    , getViewPort: function () {
        var e = window, a = 'inner';
        if (!('innerWidth' in window)) {
            a = 'client';
            e = document.documentElement || document.body;
        }
        return {
            width: e[a + 'Width'],
            height: e[a + 'Height']
        }
    }
    , handleBlockUI: function (options) {
        var options = $.extend(true, {}, options);
        var message = (options.message ? options.message : "Loading...");
        message = "<i class='fa fa-circle-o-notch fa-spin'></i>&nbsp;&nbsp;" + message;
        var color = (options.bgColor ? options.bgColor : "bg-primary");
        var centery = (options.verticalTop == true ? false : true);
        var basez = (options.zindex ? options.zindex : "999999");
        var overlayOpacity = (options.overlayOpacity ? options.overlayOpacity : "0.2");
        message = '<div class="blockUI-message ' + color + '"><span>' + message + '</span></div>';
        if (options.target) {
            $(options.target).block({
                message: message,
                baseZ: basez,
                centerY: centery,
                fadeIn: 0,
                fadeOut: 0,
                css: {
                    top: '10%',
                    border: '0',
                    padding: '0',
                    backgroundColor: 'none'
                },
                overlayCSS: {
                    backgroundColor: 'transparent',
                    opacity: overlayOpacity,
                    cursor: 'wait'
                }
            });
        } else {
            $.blockUI({
                message: message,
                baseZ: basez,
                centerY: centery,
                fadeIn: 0,
                fadeOut: 0,
                css: {
                    border: '0',
                    padding: '0',
                    backgroundColor: 'none'
                },
                overlayCSS: {
                    backgroundColor: 'transparent',
                    opacity: overlayOpacity,
                    cursor: 'wait'
                }
            });
        }
    }
    , unblockUI: function (target) {
        if (target) {
            $(target).unblock({
                onUnblock: function () {
                    $(target).css('position', '').css('zoom', '');
                }
            });
        } else {
            $.unblockUI();
        }
    }
    , apiUrl: function (url) {
        return "/api" + url;
    }
    , siteUrl: function (url) {
        return url;
    }
    , goToLogin: function () {
        //$(location).attr("href", "/Account/Login");
    }
	, getCompanyAddress: function () {
	    var id = parseInt(getGlobalCompanyID());
	    var url = apiUrl("/Company/GetCompanyAddress");
	    $.ajax({
	        "url": url,
	        "cache": false,
	        "type": "GET",
	        "data": { "companyId": id }
	    }).done(function (json) {
	        $(".sidebar-content div.address").html("");
	        if (json && json.address)
	            $(".sidebar-content div.address").html(json.address.replace(/\n/g, "<br>"));
	    });
	}
    /*,getGroupName: function(gid){		
		var url=apiUrl("/Company/GetGroupName");
    	$.ajax({
    		"url": url,
    		"cache": false,
    		"type": "GET",
			"data" : {"groupId":gid}
    	}).done(function(json) {
			//var json = {group_id:1,group_name:"ABDA Group Test",is_archieve:1,created_by:27};
			$(".navbar-header span.name,.navbar-header img").html("");
			if(json && json.group_name)
				$(".navbar-header span.name").html(json.group_name);
    	});
	}*/
    , checkIdentity: function (callback) {
    }
    , removeUserAuthorizeCache: function () {
        _USER_AUTHORIZE_CACHE = null;
    }
    , getUserAuthorizeJSON: function (callback) {
        if (_USER_AUTHORIZE_CACHE == null) {
            checkIdentity(function () {
                handleBlockUI();
                var id = getCurrentUserID();
                var url = "/Account/AuthorizeList";
                $.ajax({
                    "url": url,
                    "cache": false,
                    "type": "POST"
                }).done(function (json) {
                    _USER_AUTHORIZE_CACHE = {};
                    _USER_AUTHORIZE_CACHE.CompanyAirlines = json.CompanyAirlines;
                    _COMPANY_CACHE = json.Companies;
                    setAuth(json.Cliams);
                    getBlockedAirlines(function () {
                        if (_ECAMAPP != null) {
                            _ECAMAPP.setMy(getAuth());
                        }
                        setUpGlobalCompany();
                        if (json.Cliams.role == "CA" || json.Cliams.role == "CM")
                            getCompanyAddress();
                        /*if(json.Cliams.role=="AA" || json.Cliams.role=="AM")
                            $(".navbar-header span.name").html(json.Cliams.agent_name);
                        else if(json.Cliams.role=="EA" || json.Cliams.role=="EM")
                            $(".navbar-header span.name").html($(".navbar-header span.EA_name").html());
                        else 
                            if(json.Cliams.role=="CA" || json.Cliams.role=="CM")
                                getCompanyAddress();*/
                        $(".dropdown-menu.companyList > li").show();
                        $(".dropdown-menu.companyList > li > a[rel='" + getGlobalCompanyID() + "']").closest('li').hide();

                        checkAgentOURPage();
                        if (callback)
                            callback();
                    });
                }).always(function () {
                    unblockUI();
                });
            });
        } else {
            if (callback)
                callback();
        }
    }
    , checkAgentOURPage: function () {
        _AGENT_OUR_AIRLINE_CODES = [];
        var role = getAuth().role;
        if (role == 'AA' || role == 'AM') {
            var url = apiUrl('/CompanyAirlineSetting/Search');
            var arr = [];
            arr.push({ "name": "PageSize", "value": 0 });
            $.ajax({
                "url": url,
                "cache": false,
                "type": "GET",
                "data": arr
            }).done(function (json) {
                $.each(json.rows, function (i, row) {
                    if (cBool(row.is_show_origin_uplift_report) == true) {
                        _AGENT_OUR_AIRLINE_CODES.push(row.airline_code);
                    }
                });
                //console.log('checkAgentOURPage _AGENT_OUR_AIRLINE_CODES=', _AGENT_OUR_AIRLINE_CODES);
                if (_AGENT_OUR_AIRLINE_CODES.length <= 0) {
                    $("[menu-name='Origin Uplift Report']").remove();
                }
            });
        }
    }
    , getCompanyCurrencyCode: function (cid) {
        var code = '';
        if (_COMPANY_CACHE != null) {
            $.each(_COMPANY_CACHE, function (i, row) {
                if (cInt(row.id) == cInt(cid)) {
                    code = cString(row.other2);
                }
            });
        }
        return code;
    }
    , getCompanyForexConversionType: function (cid) {
        var code = '';
        if (_COMPANY_CACHE != null) {
            $.each(_COMPANY_CACHE, function (i, row) {
                if (cInt(row.id) == cInt(cid)) {
                    code = cString(row.other3);
                }
            });
        }
        return code;
    }
    , getGlobalCompanyID: function () {
        return cInt($(":input[name='global_company_id']").val());
    }
    , getAgentCompanyIDs: function (airlineCodes) {
        var role = getAuth().role;
        //if CA,CM role get global company id;
        if (role == "CA" || role == "CM") {
            return getGlobalCompanyID();
        } else {
            var companyIds = "";
            var companyArr = [];
            airlineCodes = $.trim(cString(airlineCodes));
            if (airlineCodes != '') {
                var arr = [];
                arr = airlineCodes.split(",");
                if (_USER_AUTHORIZE_CACHE != null) {
                    if (_USER_AUTHORIZE_CACHE.CompanyAirlines != null) {
                        $.each(arr, function (i, aid) {
                            $.each(_USER_AUTHORIZE_CACHE.CompanyAirlines, function (i, q) {
                                if (cString(q.airline_code) == cString(aid)) {
                                    companyArr.push(q.company_id);
                                }
                            });
                        });
                    }
                }
            } else {
                if (_USER_AUTHORIZE_CACHE != null) {
                    if (_USER_AUTHORIZE_CACHE.CompanyAirlines != null) {
                        $.each(_USER_AUTHORIZE_CACHE.CompanyAirlines, function (i, q) {
                            companyArr.push(q.company_id);
                        });
                    }
                }
            }
            companyArr = getArrayUnique(companyArr);
            $.each(companyArr, function (i, id) {
                companyIds += id + ",";
            });
            if (companyIds != '') {
                companyIds = companyIds.substring(0, companyIds.length - 1);
            }
            return companyIds;
        }
    }
    , pushGCEvent: function (callback) {
        _GLOBAL_COMPANY_EVENTS.push(callback);
    }
    , pushSetMyEvent: function (callback) {
        _SET_MY_EVENTS.push(callback);
    }
    , setUpGlobalCompany: function (callback) {
        var gc_key = "global_company_id_" + getCurrentUserID();
        var role = getAuth().role;
        var $globalCompany = $(":input[name='global_company_id']");
        if ($globalCompany[0]) {
            //select2Destroy($globalCompany);
            //$globalCompany.val(0);
            if (role == "EA" || role == "EM" || role == "AA" || role == "AM") {
                $(":input[name='global_company_id']").val("");
                return;
            }
            var companies = "";

            $(".dropdown-menu.companyList").empty();
            $.each(_COMPANY_CACHE, function (i, company) {
                if (company.id != getGlobalCompanyID())
                    companies += "<li><a href='javascript:;' rel='" + company.id + "'>" + company.label + "</a></li>";

            });
            if (_COMPANY_CACHE.length > 1)
                $(".dropdown-menu.companyList").html(companies);
            else
                $(".addressDiv .dropdown-submenu").hide();

            $(".dropdown-menu.companyList > li").on('click', function (e) {
                var gcid = 0;
                if ($(e.target)) {
                    gcid = cInt($(e.target).attr('rel'));
                }
                setLS(gc_key, gcid);
                $globalCompany.val(gcid);
                $(".dropdown-menu.companyList > li").show();
                $(this, ".dropdown-menu.companyList > li").hide();
                $(".navbar-header span.name").html(getLogo(cInt(gcid), false, 'icon'));
                //$(".navbar-header img").html(getLogo(cInt(gcid),true,'icon'));
                $(".sidebar-content div.name").html(getLogo(cInt(gcid), false, 'medium'));
                $(".sidebar-content .img").html(getLogo(cInt(gcid), true, 'medium'));
                getCompanyAddress();
                var ui = {};
                $.each(_GLOBAL_COMPANY_EVENTS, function (i, cb) {
                    ui['item'] = {};
                    ui['item']['id'] = gcid;
                    if (cb) {
                        cb(e, ui);
                    }
                });
                // air-sel-cnt
                $("[is_img_airline='true']").each(function () {
                    var hdn = this;
                    if (hdn) {
                        if (hdn.g)
                            hdn.g.init();
                    }
                });
                if (APP) APP.resizeContentHeight();
                //console.log(APP);
            })
            /*select2Setup($globalCompany[0],{
                multiple: true
               ,isReadOnly: 'true'
               ,width: '50px'
               ,maximumSelectionSize: 1
               ,url: apiUrl("/Company/Select")
               ,placeholder: "Select Company"
               ,resultsCallBack: function(data,page) {
                   var s2data=[];
                   $.each(data,function(i,d) {
                       s2data.push({ "id": d.id,"text": d.label,"logo": d.other,"source": d });
                   });
                   return { results: s2data };
               }
                 ,onParam: function(term,page) {
                     return {
                         term: term,
                         is_check_cache: "true"
                     };
                 }
               ,formatResult: showLogo
               ,formatSelection: showLogo
               ,onChange: function(e,ui) {
                   var gcid=0;
                   if(ui) {
                       gcid=cInt(ui.item.id);
                   }
                   setLS(gc_key,gcid);
                   $(".navbar-header span.name").html(getLogo(cInt(gcid),false,'icon'));
                   $(".navbar-header img").html(getLogo(cInt(gcid),true,'icon'));
                   $(".sidebar-content div.name").html(getLogo(cInt(gcid),false,'medium'));
                   $(".sidebar-content .img").html(getLogo(cInt(gcid),true,'medium'));
                   getCompanyAddress();
                   $.each(_GLOBAL_COMPANY_EVENTS,function(i,cb) {
                       if(cb) {
                           cb(e,ui);
                       }
                   });
                   // air-sel-cnt
                   $("[is_img_airline='true']").each(function() {
                       var hdn=this;
                       if(hdn) {
                           if(hdn.g)
                               hdn.g.init();
                       }
                   });
               }
            });*/
            var lsGCId = cInt(getLS(gc_key));
            if (lsGCId <= 0) {
                if (role == "GA" || role == "GM") {
                    //lsGCId = cInt(_COMPANY_CACHE[0].id);                 
                    if (typeof _COMPANY_CACHE[0] == "undefined") {
                    } else {
                        lsGCId = cInt(_COMPANY_CACHE[0].id);
                    }
                } else {
                    var userCompanyIds = getUserCompanyIds();
                    var temparr = userCompanyIds.split(",");
                    if (temparr.length > 0) {
                        lsGCId = cInt(temparr[0]);
                    }
                }
            }
            if (lsGCId > 0) {
                $globalCompany.val(lsGCId);
                if (role != "EA" && role != "EM" && role != "GA" && role != "GM") {
                    $(".navbar-header span.name").html(getLogo(cInt(lsGCId), false, 'icon'));
                    //$(".navbar-header img").html(getLogo(cInt(lsGCId),true,'icon'));
                    $(".sidebar-content div.name").html(getLogo(cInt(lsGCId), false, 'medium'));
                    $(".sidebar-content .img").html(getLogo(cInt(lsGCId), true, 'medium'));
                }
                getCompanyLogo({
                    "company_id": lsGCId,
                    "onComplete": function (logo) {
                        //$globalCompany.select2Refresh("data",[{ id: lsGCId,text: logo }]);
                        if (callback)
                            callback();
                    }
                });
            }
        } else {
            if (callback)
                callback();
        }
    }
    , getAirlineNameOnly: function (id, iataCode, name) {
        iataCode = cString(iataCode);
        name = cString(name);
        var html = "";               
        var imgSrc = "";
        html += '<span class="logo_icon_spn" style="vertical-align: bottom;">';
        html += imgSrc + (iataCode != "" ? iataCode + (name != "" ? " - " : "") : "") + name + '</span>';
        return html;
    }
    , getAirlineLogo: function (id, iataCode, name) {
        iataCode = cString(iataCode);
        name = cString(name);
        var html = "";
        //var imgUrl = siteUrl("/files/airline_logo/" + iataCode + "_icon.png");
        var imgUrl = siteUrl("/files/airline_logo/" + iataCode + "_small.png");                
        var imgSrc = '<img class="logo_icon" src="' + imgUrl + '" onerror="this.style.display=\'none\'" />';
        html += '<span class="logo_icon_spn" style="vertical-align: bottom;">';
        html += imgSrc + (iataCode != "" ? iataCode + (name != "" ? " - " : "") : "") + name + '</span>';
        return html;
    }
    , getMediumAirlineLogo: function (id, iataCode, name) {
        iataCode = cString(iataCode);
        name = cString(name);
        var html = "";
        var imgUrl = siteUrl("/files/airline_logo/" + iataCode + "_medium.png");
        var imgSrc = '<img class="logo_m_icon" src="' + imgUrl + '" onerror="this.style.display=\'none\'" />';
        html += '<span class="logo_m_icon_spn">';
        html += imgSrc;
        //html+=(iataCode!=""?iataCode+(name!=""?" - ":""):"")+name;
        html += '</span>';
        return html;
    }
    , getCompanyCodeLogo: function (options) {
        if (options == undefined) return "";
        var optType = $.type(options);
        if (optType == "number")
            companyId = cInt(options);
        else
            companyId = cInt(options.company_id);
        if (optType == "number") {
            var logo = "";
            $.each(_COMPANY_CACHE, function (i, row) {
                if (cInt(row.id) == cInt(companyId)) {
                    var res = row.label.split("-");
                    logo = showLogo({
                        "id": row.id, "text": res[0], "logo": row.other
                    });
                }
            });
            if (optType == "number") {
                return logo;
            } else {
                if (options.onComplete) {
                    unblockUI();
                    options.onComplete(logo);
                }
            }
        }
    }
    , getCompanyLogo: function (options, flag) {
        if (options == undefined) return "";
        var optType = $.type(options);
        var companyId = 0;
        if (optType == "number")
            companyId = cInt(options);
        else
            companyId = cInt(options.company_id);

        var searchLogo = function () {
            var logo = "";
            $.each(_COMPANY_CACHE, function (i, row) {
                if (cInt(row.id) == cInt(companyId)) {
                    logo = showLogo({
                        "id": row.id, "text": row.label, "logo": row.other, "flag": flag
                    });
                }
            });
            if (optType == "number") {
                return logo;
            } else {
                if (options.onComplete) {
                    unblockUI();
                    options.onComplete(logo);
                }
            }
        }
        if (optType == "number") {
            return searchLogo();
        } else {
            if (_COMPANY_CACHE == null) {
                var arr = [];
                arr[arr.length] = { "name": "term", "value": "" };
                arr[arr.length] = {
                    "name": "isGetAllRecords", "value": true
                };
                var url = apiUrl("/Company/Select");
                //handleBlockUI();
                $.ajax({
                    "url": url,
                    "cache": false,
                    "type": "GET",
                    "data": arr
                }).done(function (json) {
                    _COMPANY_CACHE = json;
                    searchLogo();
                }).always(function () {
                    //unblockUI();
                });
            } else {
                searchLogo();
            }
        }
    }
    , getLogo: function (options, flag, type) {
        if (options == undefined) return "";
        var optType = $.type(options);
        var companyId = 0;
        if (optType == "number")
            companyId = cInt(options);
        else
            companyId = cInt(options.company_id);

        var searchLogo = function () {
            var logo = "";
            $.each(_COMPANY_CACHE, function (i, row) {
                if (cInt(row.id) == cInt(companyId)) {
                    logo = showLogoImage({
                        "id": row.id, "text": row.label, "short_name": row.value, "logo": row.other, "flag": flag, "type": type
                    });
                }
            });
            if (optType == "number") {
                return logo;
            } else {
                if (options.onComplete) {
                    unblockUI();
                    options.onComplete(logo);
                }
            }
        }
        if (optType == "number") {
            return searchLogo();
        } else {
            if (_COMPANY_CACHE == null) {
                var arr = [];
                arr[arr.length] = { "name": "term", "value": "" };
                arr[arr.length] = {
                    "name": "isGetAllRecords", "value": true
                };
                var url = apiUrl("/Company/Select");
                //handleBlockUI();
                $.ajax({
                    "url": url,
                    "cache": false,
                    "type": "GET",
                    "data": arr
                }).done(function (json) {
                    _COMPANY_CACHE = json;
                    searchLogo();
                }).always(function () {
                    //unblockUI();
                });
            } else {
                searchLogo();
            }
        }
    }
    , showLogo: function (result) {
        var returnResult = "";
        if ($.trim(result.logo) == "") {
            returnResult = result.text;
        }
        else {
            var imgUrl = '';
            var markup = '';
            result.logo = result.logo.replace(new RegExp(/\\/g), '/');
            var ext = result.logo.split('.').pop();
            var fileName = result.logo.split(".").slice(0, -1);
            //var src = fileName + '_icon' + '.' + ext;
            var src = fileName + '_medium' + '.' + ext;
            imgUrl = siteUrl("/" + src);
            markup = '<span class="logo_icon_spn"><img class="logo_icon" src="' + imgUrl + '" />' + result.text + '</span>';
            returnResult = markup;
        }
        return returnResult;
    }
    , showLogoImage: function (result) {
        var returnResult = "";
        if ($.trim(result.logo) == "" || !result.flag) {
            returnResult = (!result.flag) ? '<span class="name">' + (result.type == "icon" ? result.short_name : result.text) + '</span>' : '';
        }
        else {
            var imgUrl = '';
            var markup = '';
            result.logo = result.logo.replace(new RegExp(/\\/g), '/');
            var ext = result.logo.split('.').pop();
            var fileName = result.logo.split(".").slice(0, -1);
            var src = fileName + '_' + result.type + '.' + ext;
            imgUrl = siteUrl("/" + src);
            var clss = result.type == "icon" ? '' : 'p-t-15 p-b-15';
            markup = '<img class="logo_icon ' + clss + '" src="' + imgUrl + '" />';
            returnResult = markup;
        }
        return returnResult;
    }
    , getCompanies: function (options) {
        if (options == undefined) return [];
        var searchCompany = function () {
            var arr = [];
            $.each(_COMPANY_CACHE, function (i, row) {
                $.each(options.company_ids, function (z, fid) {
                    if (cInt(row.id) == cInt(fid)) {
                        getCompanyLogo({
                            "company_id": fid,
                            "onComplete": function (logo) {
                                arr.push({
                                    "id": fid, "text": logo
                                });
                            }
                        });
                    }
                });
            });
            if (options.onComplete) {
                unblockUI();
                options.onComplete(arr);
            }
        }
        if (_COMPANY_CACHE == null) {
            var arr = [];
            arr[arr.length] = { "name": "term", "value": "" };
            arr[arr.length] = {
                "name": "isGetAllRecords", "value": true
            };
            var url = apiUrl("/Company/Select");
            handleBlockUI();
            $.ajax({
                "url": url,
                "cache": false,
                "type": "GET",
                "data": arr
            }).done(function (json) {
                _COMPANY_CACHE = json;
                searchCompany();
            }).always(function () {
                //unblockUI();
            });
        } else {
            searchCompany();
        }
    }
    , getFlights: function (options) {
        if (options == undefined) return [];
        var arr = [];
        arr[arr.length] = { "name": "term", "value": "" };
        var inputIds = "";
        $.each(options.flight_nos, function (z, fid) {
            inputIds += fid + ",";
        });
        if (inputIds != "") {
            inputIds = inputIds.substring(0, inputIds.length - 1);
        } else {
            inputIds = "-1";
        }
        arr[arr.length] = { "name": "flightNos", "value": inputIds };
        if (options.companyIds != "") {
            arr[arr.length] = { "name": "companyIds", "value": options.companyIds };
        }
        arr[arr.length] = {
            "name": "isGetAllRecords", "value": true
        };
        var url = apiUrl("/Flight/Select");
        handleBlockUI();
        $.ajax({
            "url": url,
            "cache": false,
            "type": "GET",
            "data": arr
        }).done(function (json) {
            var arr = [];
            $.each(json, function (i, row) {
                arr.push({
                    "id": row.id, "text": row.label
                });
            });
            unblockUI();
            if (options.onComplete) {
                options.onComplete(arr);
            }
        }).always(function () {
            //unblockUI();
        });
    }
    , getAirlines: function (airlineCodes, callback) {
        var url = apiUrl("/Airline/Select");
        var arr = [];
        arr.push({ "name": "term", "value": "" });
        arr.push({ "name": "airlineCodes", "value": airlineCodes });
        handleBlockUI();
        $.ajax({
            "url": url,
            "cache": false,
            "type": "GET",
            "data": arr
        }).done(function (json) {
            var arr = [];
            $.each(json, function (i, row) {
                arr.push({
                    "id": row.id, "text": row.label
                });
            });
            if (callback) {
                callback(arr);
            }
        }).always(function () {
            //unblockUI();
        });
    }
    , getAirports: function (options) {
        if (options == undefined) return [];
        var arr = [];
        arr[arr.length] = { "name": "term", "value": "" };
        var iatacode = "";
        if (options.iata_code) {
            iatacode = $.trim(cString(options.iata_code));
        }
        if (iatacode != "") {
            arr[arr.length] = { "name": "iataCode", "value": iatacode };
        }
        var companyIds = "";
        if (options.company_ids) {
            companyIds = $.trim(cString(options.company_ids));
        }
        if (companyIds != "") {
            arr[arr.length] = { "name": "company_ids", "value": companyIds };
        }
        var inputIds = "";
        if (options.airport_codes) {
            $.each(options.airport_codes, function (z, fid) {
                inputIds += fid + ",";
            });
        }
        if (inputIds != "") {
            inputIds = inputIds.substring(0, inputIds.length - 1);
        } else if (iatacode == "") {
            inputIds = "-1";
        }
        arr[arr.length] = { "name": "airportCodes", "value": inputIds };
        arr[arr.length] = {
            "name": "isGetAllRecords", "value": true
        };
        var url = apiUrl("/Airport/Select");
        handleBlockUI();
        $.ajax({
            "url": url,
            "cache": false,
            "type": "GET",
            "data": arr
        }).done(function (json) {
            var arr = [];
            $.each(json, function (i, row) {
                arr.push({
                    "id": row.id, "text": row.label, "source": row
                });
            });
            unblockUI();
            if (options.onComplete) {
                options.onComplete(arr);
            }
        }).always(function () {
            //unblockUI();
        });
        //} else {
        //   searchAirport();
        //}
    }
    , getAgents: function (options) {
        if (options == undefined) return [];
        var arr = [];
        arr[arr.length] = { "name": "term", "value": "" };
        var inputIds = "";
        $.each(options.agent_ids, function (z, fid) {
            inputIds += fid + ",";
        });
        if (inputIds != "") {
            inputIds = inputIds.substring(0, inputIds.length - 1);
        } else {
            inputIds = "-1";
        }
        arr[arr.length] = { "name": "agentIds", "value": inputIds };
        arr[arr.length] = {
            "name": "isGetAllRecords", "value": true
        };
        var url = apiUrl("/Agent/Select");
        handleBlockUI();
        $.ajax({
            "url": url,
            "cache": false,
            "type": "GET",
            "data": arr
        }).done(function (json) {
            var arr = [];
            $.each(json, function (i, row) {
                arr.push({
                    "id": row.id, "text": row.label
                });
            });
            unblockUI();
            if (options.onComplete) {
                options.onComplete(arr);
            }
        }).always(function () {
            //unblockUI();
        });
    }
    , getFlightTypes: function (options) {
        if (options == undefined) return [];
        var searchFlightType = function () {
            var arr = [];
            if (options.flight_type_ids.length > 0) {
                $.each(_FLIGHT_TYPE_CACHE, function (i, row) {
                    $.each(options.flight_type_ids, function (z, fid) {
                        if (cInt(row.id) == cInt(fid)) {
                            arr.push({
                                "id": row.id, "text": row.label
                            });
                        }
                    });
                });
            } else {
                arr = _FLIGHT_TYPE_CACHE;
            }
            if (options.onComplete) {
                unblockUI();
                options.onComplete(arr);
            }
        }
        if (_FLIGHT_TYPE_CACHE == null) {
            var arr = [];
            arr[arr.length] = { "name": "term", "value": "" };
            arr[arr.length] = { "name": "pageSize", "value": 500 };
            arr[arr.length] = {
                "name": "isGetAllRecords", "value": true
            };
            var url = apiUrl("/FlightType/Select");
            handleBlockUI();
            $.ajax({
                "url": url,
                "cache": false,
                "type": "GET",
                "data": arr
            }).done(function (json) {
                _FLIGHT_TYPE_CACHE = json;
                searchFlightType();
            }).always(function () {
                //unblockUI();
            });
        } else {
            searchFlightType();
        }
    }
    , getCountrys: function (options) {
        if (options == undefined) return [];
        var searchCountry = function () {
            var arr = [];
            $.each(_COUNTRY_CACHE, function (i, row) {
                if (options.country_codes) {
                    $.each(options.country_codes, function (z, fid) {
                        if (cString(row.id) == cString(fid)) {
                            arr.push({
                                "id": row.id, "text": row.label
                            });
                        }
                    });
                }
                if (options.country_codes) {
                    $.each(options.country_codes, function (z, co) {
                        if (row.other == co) {
                            arr.push({
                                "id": row.id, "text": row.label
                            });
                        }
                    });
                }
            });
            if (options.onComplete) {
                unblockUI();
                options.onComplete(arr);
            }
        }
        if (_COUNTRY_CACHE == null) {
            var arr = [];
            arr[arr.length] = { "name": "term", "value": "" };
            arr[arr.length] = { "name": "pageSize", "value": 500 };
            arr[arr.length] = {
                "name": "isGetAllRecords", "value": true
            };
            var url = apiUrl("/Country/Select");
            handleBlockUI();
            $.ajax({
                "url": url,
                "cache": false,
                "type": "GET",
                "data": arr
            }).done(function (json) {
                _COUNTRY_CACHE = json;
                searchCountry();
            }).always(function () {
                //unblockUI();
            });
        } else {
            searchCountry();
        }
    }
    , getZones: function (options) {
        if (options == undefined) return [];
        var searchZone = function () {
            var arr = [];
            $.each(_ZONE_CACHE, function (i, row) {
                $.each(options.zone_ids, function (z, fid) {
                    if (cInt(row.id) == cInt(fid)) {
                        arr.push({
                            "id": row.id, "text": row.label
                        });
                    }
                });
            });
            if (options.onComplete) {
                unblockUI();
                options.onComplete(arr);
            }
        }
        if (_ZONE_CACHE == null) {
            var arr = [];
            arr[arr.length] = { "name": "term", "value": "" };
            arr[arr.length] = { "name": "pageSize", "value": 500 };
            arr[arr.length] = {
                "name": "isGetAllRecords", "value": true
            };
            var url = apiUrl("/Zone/Select");
            handleBlockUI();
            $.ajax({
                "url": url,
                "cache": false,
                "type": "GET",
                "data": arr
            }).done(function (json) {
                _ZONE_CACHE = json;
                searchZone();
            }).always(function () {
                //unblockUI();
            });
        } else {
            searchZone();
        }
    }
    , getInvoiceFields: function (options) {
        if (options == undefined) return [];
        var arr = [];
        arr[arr.length] = { "name": "term", "value": "" };
        var inputIds = "";
        $.each(options.field_codes, function (z, fid) {
            inputIds += fid + ",";
        });
        if (inputIds != "") {
            inputIds = inputIds.substring(0, inputIds.length - 1);
        } else {
            inputIds = "-1";
        }
        arr[arr.length] = { "name": "fieldCodes", "value": inputIds };
        var url = apiUrl("/InvoiceField/Select");
        handleBlockUI();
        $.ajax({
            "url": url,
            "cache": false,
            "type": "GET",
            "data": arr
        }).done(function (json) {
            var arr = [];
            $.each(json, function (i, row) {
                arr.push({
                    "id": row.other, "text": row.label
                });
            });
            unblockUI();
            if (options.onComplete) {
                options.onComplete(arr);
            }
        }).always(function () {
            //unblockUI();
        });
    }
    , getCommodityTypes: function (options) {
        if (options == undefined) return [];
        var searchCommodityType = function () {
            var arr = [];
            $.each(_COMMODITY_TYPE_CACHE, function (i, row) {
                $.each(options.commodity_type_ids, function (z, fid) {
                    if (cInt(row.id) == cInt(fid)) {
                        arr.push({
                            "id": row.id, "text": row.label
                        });
                    }
                });
            });
            if (options.onComplete) {
                unblockUI();
                options.onComplete(arr);
            }
        }
        if (_COMMODITY_TYPE_CACHE == null) {
            var arr = [];
            arr[arr.length] = { "name": "term", "value": "" };
            arr[arr.length] = { "name": "pageSize", "value": 500 };
            arr[arr.length] = { "name": "companyIds", "value": getGlobalCompanyID().toString() };
            arr[arr.length] = {
                "name": "isGetAllRecords", "value": true
            };
            var url = apiUrl("/CommodityType/Select");
            handleBlockUI();
            $.ajax({
                "url": url,
                "cache": false,
                "type": "GET",
                "data": arr
            }).done(function (json) {
                _COMMODITY_TYPE_CACHE = json;
                searchCommodityType();
            }).always(function () {
                //unblockUI();
            });
        } else {
            searchCommodityType();
        }
    }
    , getUserCompanyIds: function () {
        var ids = "";
        var auth = getAuth();
        if (auth != null) {
            ids = cString(auth[auth.role + "_company_ids"]);
        }
        return ids;
    }
    , getGroupIds: function () {
        var auth = getAuth();
        var ids = '';
        if (auth != null) {
            return cString(auth.groupids);
        }
        if (ids == '')
            ids = '-1';
        return ids;
    }
    , alertErrorMessage: function (json) {
        var errors = generateErrors(json);
        var msg = "";
        $.each(errors, function (i, err) {
            msg += err.ErrorMessage + "<br/>";
        });
        if (msg != "") {
            jAlert(msg);
        }
    }
    , replaceHTML: function (value) {
        var rex = /(<([^>]+)>)/ig;
        return $.trim(value.replace(rex, ""));
    }
    , getErrorMessage: function (json) {
        var errors = generateErrors(json);
        var msg = '';
        if (errors != '') {
            var i;
            for (i = 0; i < errors.length; i++) {
                msg += errors[i].ErrorMessage + ',';
            }
        }
        if (msg != '')
            msg = msg.substring(0, msg.length - 1);
        return msg;
    }
    , cloneObj: function (obj) {
        return JSON.parse(JSON.stringify(obj));
    }
    , generateErrors: function (json) {
        var errros = [];
        if (json.ModelState) {
            var jsonData = json.ModelState;
            for (var obj in jsonData) {
                if (jsonData.hasOwnProperty(obj)) {
                    for (var prop in jsonData[obj]) {
                        if (jsonData[obj].hasOwnProperty(prop)) {
                            errros.push({
                                "PropertyName": obj, "ErrorMessage": jsonData[obj][prop]
                            });
                        }
                    }
                }
            }
        }
        var getErrorCount = function (propertyName) {
            var cnt = 0;
            $.each(errros, function (i, err) {
                if (err.PropertyName == propertyName
                    && err.ErrorMessage != "A value is required.") {
                    cnt += 1;
                }
            });
            return cnt;
        }
        var removeDuplicate = function () {
            $.each(errros, function (i, err) {
                if (err.ErrorMessage == "A value is required.") {
                    if (getErrorCount(err.PropertyName) > 0) {
                        errros.splice(i, 1);
                    }
                }
            });
        }
        try {
            removeDuplicate();
        } catch (ex) {
            removeDuplicate();
        }
        return errros;
    }
    , checkCompanyAuth: function (companyId) {
        var isResult = false;
        var role = getAuth().role;
        if (role == "GM" || role == "CA" || role == "CM" || role == "AA" || role == "AM") {
            var ids = getUserCompanyIds();
            var arr = ids.split(",");
            $.each(arr, function (i, strid) {
                if (cInt(strid) == companyId) {
                    isResult = true;
                }
            });
        } else {
            isResult = true;
        }
        return isResult;
    }
    , getCacheAirlineWithPrefix: function (prefix) {
        var airlines = [];
        var companyAirlines = [];
        var arr = [];
        if (_USER_AUTHORIZE_CACHE != null) {
            if (_USER_AUTHORIZE_CACHE.CompanyAirlines != null) {
                $.each(_USER_AUTHORIZE_CACHE.CompanyAirlines, function (i, airline) {
                    if (airline.awb_prefix == prefix) {
                        if (checkCompanyAirlines(companyAirlines, airline) == false) {
                            companyAirlines.push(airline);
                        }
                    }
                });
            }
        }
        $.each(companyAirlines, function (i, ca) {
            var lbl = cString(ca.airline_code) + " - " + cString(ca.airline_name);
            airlines.push({
                "id": ca.airline_code,
                "label": lbl,
                "value": lbl,
                "other": cString(ca.airline_logo),
                "other2": cString(ca.awb_prefix)
            });
        });
        return airlines;
    }
    , getCacheCompanyAirlines: function (companyId) {
        var arr = [];
        if (_USER_AUTHORIZE_CACHE != null) {
            if (_USER_AUTHORIZE_CACHE.CompanyAirlines != null) {
                $.each(_USER_AUTHORIZE_CACHE.CompanyAirlines, function (i, q) {
                    if (q.company_id == cInt(companyId)) {
                        arr.push(q);
                    }
                });
            }
        }
        return arr;
    }
    , getBlockedAirlines: function (callback) {
        var isLoadAirlines = false;
        if (_USER_AUTHORIZE_CACHE != null) {
            if (_USER_AUTHORIZE_CACHE.BlockedAirlines == null) {
                isLoadAirlines = true;
            }
        }
        if (isLoadAirlines == true) {
            var url = apiUrl("/Airline/GetBlockedAirlines");
            $.ajax({
                "url": url,
                "cache": false,
                "type": "GET"
            }).done(function (json) {
                //console.log('getBlockedAirlines _USER_AUTHORIZE_CACHE=', _USER_AUTHORIZE_CACHE);
                if (_USER_AUTHORIZE_CACHE != null) {
                    _USER_AUTHORIZE_CACHE.BlockedAirlines = json;
                }
                if (callback)
                    callback();
            });
        } else {
            if (callback)
                callback();
        }
    }
    , getUserAgentIds: function () {
        var auth = getAuth();
        var ids = '';
        if (auth != null) {
            var role = auth.role;
            if (role == "AA" || role == "AM") {
                var key = role + "_" + "agent_ids";
                if (auth[key]) {
                    ids = auth[key];
                }
            }
        }
        return ids;
    }
    , getUserAgents: function () {
        return getUserAgentIds().split(",");
    }
    , checkCompanyAirlines: function (companyAirlines, airline) {
        var isExist = false;
        $.each(companyAirlines, function (i, ca) {
            if (cString(ca.airline_code) == cString(airline.airline_code)) {
                isExist = true;
            }
        });
        return isExist;
    }
    , searchCompanyAirlineCache: function (params) {
        var airlines = [];
        var term = getParamValue("term", params);
        var companyIds = getParamValue("companyids", params);
        if (companyIds == '' || companyIds == "0") {
            companyIds = getUserCompanyIds();
        }
        var arr = companyIds.split(",");
        var companyAirlines = [];
        $.each(arr, function (i, cid) {
            var companyId = cInt(cid);
            if (companyId > 0) {
                var temparr = getCacheCompanyAirlines(companyId);
                $.each(temparr, function (j, airline) {
                    if (term != '') {
                        if (cString(airline.airline_code).toString() == term) {
                            if (checkCompanyAirlines(companyAirlines, airline) == false) {
                                companyAirlines.push(airline);
                            }
                        }
                    } else {
                        if (checkCompanyAirlines(companyAirlines, airline) == false) {
                            companyAirlines.push(airline);
                        }
                    }
                });
            }
        });
        $.each(companyAirlines, function (i, ca) {
            var lbl = cString(ca.airline_code) + " - " + cString(ca.airline_name);
            airlines.push({
                "id": ca.airline_code,
                "label": lbl,
                "value": lbl,
                "other": cString(ca.airline_logo),
                "other2": cString(ca.awb_prefix),
                "other3": cString(ca.airline_code)
            });
        });
        return airlines;
    }
    , searchCompanyCache: function (params) {
        var arr = [];
        var term = getParamValue("term", params);
        var airlineCodes = getParamValue("airlineids", params);
        if (_COMPANY_CACHE != null) {
            $.each(_COMPANY_CACHE, function (i, item) {
                if ($.trim(term) != "") {
                    if (item.label.toString().isExist(term.toLowerCase(), true)) {
                        if (checkCompanyAuth(item.id) == true) {
                            arr.push(item);
                        }
                    }
                } else if (checkCompanyAuth(item.id) == true) {
                    arr.push(item);
                }
            });
        }
        return arr;
    }
    , select2Ajax: function (url, params, callback, isNotGlobalCompanyFilter) {
        var role = '';
        var auth = getAuth();
        if (auth != null) {
            role = auth.role;
        }
        if ((url.indexOf('/Airport/Select') > -1
            || url.indexOf('/Airline/Select') > -1
            || url.indexOf('/Agent/Select') > -1)
            && isNotGlobalCompanyFilter == false) {
            var isCompanyIdExist = false;
            var term = getParamValue("term", params);
            var companyIds = getParamValue("companyids", params);
            if (companyIds != "") {
                isCompanyIdExist = true;
            }
            if (isCompanyIdExist == false && getGlobalCompanyID() > 0) {
                var isNameParams = false;
                $.each(params, function (i, p) {
                    if (p) {
                        if (p.name) {
                            isNameParams = true;
                        }
                    }
                });
                try {
                    if (isNameParams == true) {
                        params.push({ "name": "companyIds", "value": getGlobalCompanyID().toString() });
                    } else {
                        params.companyIds = getGlobalCompanyID().toString();
                    }
                } catch (ex) { }
            }
        }
        var isNotExist = false;
        if (getParamValue("is_check_cache", params) == "true" && (role != 'EA' && role != 'EM')) {
            var arr = [];
            if (url.indexOf('/Company/Select') > -1) {
                arr = searchCompanyCache(params);
            } else if (url.indexOf('/Airline/Select') > -1 && (role != 'GA')) {
                arr = searchCompanyAirlineCache(params);
            } else {
                isNotExist = true;
            }
        } else {
            isNotExist = true;
        }
        if (isNotExist == false) {
            if (callback)
                callback(arr);
        } else {
            $.ajax({
                "url": url,
                "cache": false,
                "type": "GET",
                "data": params
            }).done(function (json) {
                if (callback)
                    callback(json);
            }).always(function () {
            });
        }
    }
    , getParamValue: function (name, params) {
        var value = "";
        for (var obj in params) {
            if (params.hasOwnProperty(obj)) {
                for (var prop in params[obj]) {
                    if (obj.toString().toLowerCase() == name.toString().toLowerCase()) {
                        value = params[obj];
                    }
                }
            }
        }
        if (value == "") {
            $.each(params, function (i, p) {
                if (p) {
                    if (p.name) {
                        if (p.name.toString().toLowerCase() == name.toString().toLowerCase())
                            value = p.value;
                    }
                }
            });
        }
        return cString(value);
    }
    , removeHTML: function (text) {
        var rex = /(<([^>]+)>)/ig;
        text = cString(text).replace(rex, "");
        return $.trim(text);
    }
    , serializeArray: function (target) {
        var params = [];
        $(":input", target).each(function () {
            var $this = $(this);
            if (cString($this.attr("name")) != "")
                params.push({
                    "name": $this.attr("name"), "value": cString($this.val())
                })
        });
        return params;
    }
    , imageAirlineSelect: function (element, options) {
        $(element).attr("is_img_airline", "true").airlineSelect({
            "multiple": (options.multiple ? options.multiple : false)
            , "logoLimit": (cInt(options.logoLimit) > 0 ? cInt(options.logoLimit) : 3)
            , "width": (cInt(options.width) > 0 ? cInt(options.width) : 370)
            , "is_iatacode": (cString(options.is_iatacode) == "true" ? "true" : "false")
			, "is_fullcontainer": (cString(options.is_fullcontainer) == "true" ? "true" : "false")
            , "is_check_our": (cString(options.is_check_our) == "true" ? true : false)
            , "is_show_blocked_airline": (cString(options.is_show_blocked_airline) == "true" ? "true" : "")
            , "onChange": function (e) {
                if (options.onChange)
                    options.onChange(e, null);
            }
            , "onParams": function () {
                var term = '';
                var companyIds = '';
                if (options.onParam) {
                    var params = options.onParam("", 1);
                    term = getParamValue("term", params);
                    companyIds = getParamValue("companyids", params);
                }
                if (cString(companyIds) == "") {
                    var cid = cInt(getGlobalCompanyID());
                    if (cid > 0)
                        companyIds = cid;
                }
                var arr = [];
                arr.push({ "name": "companyIds", "value": companyIds });
                arr.push({ "name": "term", "value": term });
                return arr;
            }
        });
    }
    , hideAllToolTips: function () {
        $('[data-toggle="tooltip"]:not(.popover)').tooltip('hide');
        $('[data-original-title]:not(.popover)').tooltip('hide');
        $('[rel="tooltip"]:not(.popover)').tooltip('hide');
        $('[role="tooltip"]:not(.popover)').remove();
    }
    , formatAirportDisplay: function (source) {
        var r = "";
        if (source.other3) {
            var arr = source.other3.split("|");
            if ($.trim(arr[0]) != '')
                r += arr[0];
            if ($.trim(arr[2]) != '')
                r += ", " + arr[2];
            if ($.trim(arr[4]) != '')
                r += ", " + arr[4];
        }
        return r;
    }
    , select2Destroy: function (element) {
        if ($(element).data("select2")) {
            $(element).select2("destroy")
                .off('change')
                .attr("is_apply_select2_setup", "false")
                .removeAttr("style");
        } else {
            try {
                var txtid = cString($(element).data("txtid"));
                var $txt = $("#" + txtid);
                $txt.autocomplete("destroy");
                var $box = $txt.parents(".ui-ajax-combo:first");
                $box.remove();
            } catch (e) {
                //console.log('select2Destroy e1=', e);
            }
        }
    }
    , select2Setup: function (element, options) {
        $(element).attr("is_apply_select2_setup", "true");
        var url = options.url;
        var ajaxUrl = "";
        var isDontClearValue = false;
        var isAutoSelect = true;
        var isReadOnly = false;
        var isNotGlobalCompanyFilter = false;
        var delay = 300;
        var isImgAirline = false;
        var searchMinLength = 1;

        if (options.is_img_airline) {
            if (options.is_img_airline.toString() == "true") {
                imageAirlineSelect(element, options);
                return;
            }
        }
        if (options.delay) {
            delay = cInt(options.delay);
        }
        if (delay <= 0) {
            delay = 500;
        }
        if (options.isAutoSelect == "false") {
            isAutoSelect = false;
        }
        if (options.isDontClearValue == "true") {
            isDontClearValue = true;
        }
        if (options.isReadOnly == "true") {
            isReadOnly = true;
        }
        if (options.isNotGlobalCompanyFilter == "true") {
            isNotGlobalCompanyFilter = true;
        }
        var width = cString(options.width);
        if (width == "")
            width = '100%';
        var isMultiple = options.multiple;
        var maximumSelectionSize = cInt(options.maximumSelectionSize);
        if (maximumSelectionSize <= 0) {
            maximumSelectionSize = 1000;
        }
        var allowClear = (options.allowClear ? options.allowClear : false);
        var isNoShowAllBtn = "false";
        if (options.is_no_show_all_btn == "true") {
            isNoShowAllBtn = "true";
        }
        var isNoClearBtn = "false";
        if (options.is_no_clear_btn == "true") {
            isNoClearBtn = "true";
        }
        var isDontChangeTextBoxValue = "false";
        if (options.is_dont_change_txt_value == "true") {
            isDontChangeTextBoxValue = "true";
        }
        if (maximumSelectionSize <= 1 && options.isFixedData != "true") {
            var txtid = $(element).attr("name") + "_" + Math.floor((Math.random() * 100000) + 1);
            var $txtElement = null;

            if ($(element).attr("type") != "text") {
                $txtElement = $("<input type='text' id='" + txtid + "' name='" + txtid + "' class='form-control input-sm' placeholder='" + options.placeholder + "' />");
                $(element).after($txtElement);
            } else {
                $txtElement = $(element);
                var isshowallbtn = cString($txtElement.attr("is_no_show_all_btn"));
                if (isshowallbtn == "true")
                    isNoShowAllBtn = "true";
                var isnoclearbtn = cString($txtElement.attr("is_no_clear_btn"));
                if (isnoclearbtn == "true")
                    isNoClearBtn = "true";
            }
            if (isNoShowAllBtn == "true")
                $txtElement.attr("is_no_show_all_btn", "true");
            if (isNoClearBtn == "true")
                $txtElement.attr("is_no_clear_btn", "true");

            if (width != '100%')
                $txtElement.width(width);

            $(element).data("txtid", txtid);
            $txtElement.attr("is_auto_select", isAutoSelect);

            ajaxUrl = "";
            if ($.type(options.url) == "function") {
                ajaxUrl = options.url();
            } else {
                ajaxUrl = options.url;
            }
            //console.log('ajaxUrl=',ajaxUrl,'index=',ajaxUrl.indexOf('/Airport/Select'));
            if (ajaxUrl.indexOf('/Airport/Select') > -1) {
                searchMinLength = 3;
            } else if (ajaxUrl.indexOf('/Airline/Select') > -1) {
                searchMinLength = 2;
            }
            //console.log('searchMinLength=',searchMinLength);

            $txtElement.attr("search_min_length", searchMinLength);

            if ($txtElement.attr("is_hide_drop_down") == "true") {
                $txtElement.addClass("is-hide-drop-down");
            }

            if (isReadOnly == true) {
                $txtElement.attr("readonly", "readonly").addClass("ui-readonly");
            }
            $txtElement.data("url", ajaxUrl);
            $txtElement
            .keypress(function (e) {
                $txtElement.attr("is_key_press", "true");
                if (e.keyCode == 13
                    || e.keyCode == 9
                    || e.keyCode == 46) {
                    if ($.trim($txtElement.val()) == ''
                        && $txtElement.attr("is_no_show_all_btn") == "true") {
                        var autocomplete = $txtElement.data("ui-autocomplete");
                        autocomplete._trigger("select", e, { item: { "id": "", "label": "", "text": "", "value": "", "other": "", "other2": "", "id2": "" } });
                        $txtElement.attr("is_changed", "false");
                        $txtElement.attr("is_key_press", "false");
                    }
                }
            })
            .change(function (e) {
                $txtElement.attr("is_changed", "true");
            })
            .ajaxcombobox({
                position: {
                    my: "left top",
                    at: "left bottom",
                    collision: "flip"
                }
                , minLength: searchMinLength
                , delay: delay
                , source: function (request, response) {
                    $txtElement.autocomplete("option", { minLength: cInt($txtElement.attr("search_min_length")) });
                    var params = [{
                        "name": "term", "value": request.term
                    }];
                    if (options.onParam) {
                        params = options.onParam(request.term, 1);
                    }
                    ajaxUrl = "";
                    if ($.type(options.url) == "function") {
                        ajaxUrl = options.url();
                    } else {
                        ajaxUrl = options.url;
                    }
                    select2Ajax(ajaxUrl, params, function (arr) {
                        if (options.uiACCallBack) {
                            arr = options.uiACCallBack(arr);
                        } else {
                            if (ajaxUrl.indexOf('/Airport/Select') > -1) {
                                $.each(arr, function (i, ar) {
                                    var r = "";
                                    if (ar.other3) {
                                        var arr = ar.other3.split("|");
                                        if ($.trim(arr[0]) != '')
                                            r += arr[0];
                                        if ($.trim(arr[2]) != '')
                                            r += ", " + arr[2];
                                        if ($.trim(arr[4]) != '')
                                            r += ", " + arr[4];
                                    }
                                    ar.value = r;
                                    ar.label = r;
                                });
                            }
                        }
                        response(arr);
                    }, isNotGlobalCompanyFilter);
                }
                , change: function (event, ui) {
                    $txtElement.autocomplete("option", {
                        minLength: 1
                    });
                    var $uiReadonly = $txtElement.parents(".ui-readonly:first");
                    if ($uiReadonly[0] && ui) {
                        $(".ui-readonly-label", $uiReadonly).html(ui.item.label);
                    }
                    $txtElement.tooltip('destroy');
                    $txtElement.attr("title", "").attr("is_key_press", "false");
                    $txtElement.parent().removeClass("is_select");
                    if ($.trim(cString($txtElement.val())) != "") {
                        $txtElement.parent().addClass("is_select");
                    }
                    if (isDontClearValue == false) {
                        $(element).val("");
                        if (ui.item) {
                            if (isDontChangeTextBoxValue != 'true')
                                $(element).val(ui.item.id);

                            $txtElement.attr("title", ui.item.label);
                        }
                        if ($txtElement.attr("title").length > 15) {
                            $txtElement.tooltip({
                                'container': 'body', 'delay': {
                                    "show": 500, "hide": 0
                                }
                            });
                        }
                    }
                    if ($txtElement.attr("is_select_ui") == "true") {
                        $txtElement.attr("is_select_ui", "");
                    } else {
                        $txtElement.attr("is_select_ui", "");
                        $(element).change().attr("is_changed", "false");
                        if (options.onChange)
                            options.onChange(event, ui);
                    }
                }
                , select: function (event, ui) {
                    $txtElement.autocomplete("option", {
                        minLength: 1
                    });
                    var $uiReadonly = $txtElement.parents(".ui-readonly:first");
                    if ($uiReadonly[0] && ui) {
                        $(".ui-readonly-label", $uiReadonly).html(ui.item.label);
                    }

                    var prevID = "";
                    if ($txtElement.attr("is_key_press") != "true") {
                        prevID = $(element).val();
                    }
                    $txtElement.attr("is_key_press", "false");
                    $txtElement.tooltip('destroy');
                    $txtElement.attr("title", "").attr("is_select_ui", "true");
                    $txtElement.parent().removeClass("is_select");
                    if ($.trim(cString($txtElement.val())) != "") {
                        $txtElement.parent().addClass("is_select");
                    }
                    if (ui.item) {
                        if (ui.item.id != prevID
                            || $.trim($txtElement.val()) == "") {
                            if (isDontChangeTextBoxValue != 'true')
                                $(element).val(ui.item.id);

                            $txtElement.attr("title", ui.item.label);
                            $(element).change().attr("is_changed", "false");
                            if (options.onChange)
                                options.onChange(event, ui);
                        }
                    }
                    if ($txtElement.attr("title").length > 15) {
                        $txtElement.tooltip({
                            'container': 'body', 'delay': {
                                "show": 500, "hide": 0
                            }
                        });
                    }
                }
            }).autocomplete("instance")._renderItem = function (ul, item) {
                $txtElement.autocomplete("option", { minLength: cInt($txtElement.attr("search_min_length")) });
                var term = $txtElement.val(), html = item.label.replace(term, "<b>$&</b>");
                ajaxUrl = "";
                if ($.type(options.url) == "function") {
                    ajaxUrl = options.url();
                } else {
                    ajaxUrl = options.url;
                }
                if (ajaxUrl.indexOf('/Airline/Select') > -1 || ajaxUrl.indexOf('/Company/Select') > -1) {
                    html = showLogo({
                        "id": item.id, "text": html, "logo": ""
                    });
                    return $("<li></li>").data("item.autocomplete", item).append($("<a></a>").html(html)).appendTo(ul);
                } else if (ajaxUrl.indexOf('/Airport/Select') > -1) {
                    var r = "";
                    if (item.other3) {
                        var arr = item.other3.split("|");
                        if ($.trim(arr[0]) != '')
                            r += arr[0];
                        if ($.trim(arr[2]) != '')
                            r += ", " + arr[2];
                        if ($.trim(arr[3]) != '')
                            r += ", " + arr[3];
                        if ($.trim(arr[1]) != '')
                            r += ", " + arr[1];
                    }
                    html = r;
                    return $("<li></li>").data("item.autocomplete", item).append($("<a></a>").html(html)).appendTo(ul);
                } else {
                    return $("<li></li>").data("item.autocomplete", item).append($("<a></a>").html(html)).appendTo(ul);
                }
            };
            $txtElement.autocomplete("instance")._renderMenu = function (ul, items) {
                //console.log('_renderMenu ul=',ul);
                var that = this;
                $.each(items, function (index, item) {
                    that._renderItemData(ul, item);
                });
                if ($txtElement.attr("is_hide_drop_down") == "true") {
                    $(ul).addClass("is-hide-drop-down");
                }
            }
        } else {
            var minimumResultsForSearch = (cInt(options.minimumResultsForSearch) < 0 ? cInt(options.minimumResultsForSearch) : 0);
            var opt;
            if (options.isFixedData == "true") {
                opt = {
                    placeholder: options.placeholder,
                    minimumInputLength: 0,
                    maximumSelectionSize: maximumSelectionSize,
                    minimumResultsForSearch: minimumResultsForSearch,
                    multiple: isMultiple,
                    allowClear: allowClear,
                    width: width,
                    query: function (query) {
                        query.callback(options.fixedData);
                    }
                };
            } else {
                opt = {
                    placeholder: options.placeholder,
                    minimumInputLength: 0,
                    maximumSelectionSize: maximumSelectionSize,
                    minimumResultsForSearch: minimumResultsForSearch,
                    multiple: isMultiple,
                    allowClear: allowClear,
                    delay: delay,
                    width: width,
                    query: function (query) {
                        var params = [{
                            "name": "term", "value": query.term
                        }];
                        if (options.onParam) {
                            params = options.onParam(query.term, query.page);
                        }
                        ajaxUrl = "";
                        if ($.type(options.url) == "function") {
                            ajaxUrl = cString(options.url());
                        } else {
                            ajaxUrl = cString(options.url);
                        }
                        select2Ajax(ajaxUrl, params, function (arr) {
                            if (options.resultsCallBack)
                                query.callback(options.resultsCallBack(arr, query.page));
                        }, isNotGlobalCompanyFilter);
                    }
                };
            }
            if (options.formatResult) {
                opt.formatResult = options.formatResult;
            }
            if (options.formatSelection) {
                opt.formatSelection = options.formatSelection;
            }
            var isIgnoreAirportFormat = false;
            if (options.isIgnoreAirportFormat) {
                isIgnoreAirportFormat = options.isIgnoreAirportFormat;
            }
            ajaxUrl = "";
            if ($.type(options.url) == "function") {
                ajaxUrl = cString(options.url());
            } else {
                ajaxUrl = cString(options.url);
            }
            if (isIgnoreAirportFormat == false) {
                if (ajaxUrl.indexOf('/Airport/Select') > -1) {
                    opt.formatResult = function (result) {
                        var r = "";
                        if (result.source) {
                            var arr = result.source.other3.split("|");
                            if ($.trim(arr[0]) != '')
                                r += arr[0];
                            if ($.trim(arr[2]) != '')
                                r += ", " + arr[2];
                            if ($.trim(arr[3]) != '')
                                r += ", " + arr[3];
                            if ($.trim(arr[1]) != '')
                                r += ", " + arr[1];
                        }
                        return r;
                    }
                    opt.formatSelection = function (result) {
                        var r = "";
                        if (result.source) {
                            var arr = result.source.other3.split("|");

                            if ($.trim(arr[0]) != '')
                                r += arr[0];
                            if ($.trim(arr[2]) != '')
                                r += ", " + arr[2];
                            if ($.trim(arr[4]) != '')
                                r += ", " + arr[4];
                        }
                        return r;
                    }
                }
            }
            $(element).select2(opt).on("change", function (e) {
                if (options.onChange)
                    options.onChange(e);
            });
        }
    }
    , getAirlineDayIndex: function (dt) {
        var dayIndex = 0;
        var dayFormat = moment(dt).format("MM/DD/YYYY")
        var dayName = moment(dt).format("dddd").toString().toUpperCase();
        //console.log('getAirlineDayIndex dayName=',dayName,'dayFormat=',dayFormat);
        switch (dayName) {
            case "MONDAY": dayIndex = 0; break;
            case "TUESDAY": dayIndex = 1; break;
            case "WEDNESDAY": dayIndex = 2; break;
            case "THURSDAY": dayIndex = 3; break;
            case "FRIDAY": dayIndex = 4; break;
            case "SATURDAY": dayIndex = 5; break;
            case "SUNDAY": dayIndex = 6; break;
        }
        return dayIndex;
    }
    , checkFormData: function (params, arr) {
        $.each(arr, function (j, row) {
            $.each(params, function (i, p) {
                if (p.name == row.name) {
                    if (row.type == "int") {
                        p.value = cInt(p.value);
                    }
                    if (row.type == "decimal") {
                        p.value = cFloat(p.value);
                    }
                }
            });
        });
        return params;
    }
    , setTokenToViewsApp: function (token, callback) {
        // Save token to ecam.views
        var p = [];
        p.push({ "name": "access_token", "value": token });
        $.ajax({
            "url": "/Account/SetToken",
            "cache": false,
            "type": "POST",
            "data": p
        }).done(function (stjson) {
            if (callback)
                callback();
        });
    }
    , refreshToken: function () {
        var auth = getAuth();
        if (auth != null) {
            var url = apiUrl("/Token");
            var data = [];
            data.push({ "name": "grant_type", "value": "refresh_token" });
            data.push({ "name": "client_id", "value": "desktop_website" });
            data.push({ "name": "refresh_token", "value": auth.refresh_token });
            $.ajax({
                "url": url,
                "cache": false,
                "type": "POST",
                "dataType": "JSON",
                "data": data
            }).done(function (json) {
                auth[".expires"] = json[".expires"];
                auth[".issued"] = json[".issued"];
                auth["expires_in"] = json["expires_in"];
                auth["access_token"] = json["access_token"];
                auth["refresh_token"] = json["refresh_token"];
                setAuth(auth);
            }).fail(function (jqxhr) {
            }).always(function (jqxhr) {
            });
        }
    }
    , checkTokenRefresh: function () {
        try {
            var auth = getAuth();
            if (auth != null) {
                var gmdLocalOffSet = getLocalGMTOffset();
                var d = new Date(auth[".expires"]);
                var expireDate = d.addSeconds(-gmdLocalOffSet);
                var nowDate = new Date(moment.utc().format('YYYY/MM/DD HH:mm:ss'))
                var timeDiff = getTimeDifference(moment.utc().format('YYYY/MM/DD HH:mm:ss'), moment(expireDate).format('YYYY/MM/DD HH:mm:ss'));
                if (timeDiff.totalMinutes <= 5) {
                    refreshToken();
                }
            }
        } catch (e) { }
    }
    , startCheckTokenRefresh: function () {
        setInterval(function () {
            checkTokenRefresh();
        }, (60 * 1000));
        checkTokenRefresh();
    }
    , removeJSONProperties: function (json, prop) {
        $.each(prop, function (i, p) {
            delete json[p];
        });
        return json;
    }
    , changeLayoutLeftTop: function () {
        //console.log('changeLayoutLeftTop');
        $(".is-img-airline-hdn").each(function () {
            var hdn = this;
            if (hdn.g) {
                hdn.g.redraw();
                //console.log('values=', hdn.value, 'p=', hdn.p);
                //console.log('g.json=', hdn.g.json);
            }
        });
    }
});
(function ($) {
    setInterval(function () { hideAllToolTips(); }, 5000);
    $.fn.select2Refresh = function (mode, options) {
        var element = this;
        if ($(element).attr("is_img_airline") == "true") {
            try {
                var g = $(element)[0].g;
                if (g) {
                    var ids = "";
                    $.each(options, function (i, row) {
                        ids += row.id + ",";
                    });
                    if (ids != '') {
                        ids = ids.substring(0, ids.length - 1);
                    }
                    g.applyValues(ids);
                }
            } catch (e) {
                //console.log('e=', e);
            }
            return;
        }
        if ($(element).data("select2")) {
            if (mode == "data") {
                $(element).select2("data", options);
            } else if (mode == "val") {
                $(element).select2("val", options);
            }
        } else {
            var $txtElement = $("#" + $(element).data("txtid"));
            $txtElement.parent().removeClass("is_select");
            var lbl = "";
            if (mode == "data") {
                $.each(options, function (i, opt) {
                    if (opt.text)
                        lbl = opt.text;
                    else
                        lbl = opt.label;
                });
            } else if (mode == "val") {
                lbl = options;
            }
            var rex = /(<([^>]+)>)/ig;
            lbl = cString(lbl).replace(rex, "");
            $txtElement.val(lbl);
            $txtElement.parent().removeClass("is_select");
            if ($.trim(cString($txtElement.val())) != "") {
                $txtElement.parent().addClass("is_select");
            }
            var $uiReadonly = $txtElement.parents(".ui-readonly:first");
            if ($uiReadonly[0]) {
                $(".ui-readonly-label", $uiReadonly).html(lbl);
            }
            if (lbl.length > 15) {
                $txtElement.attr("title", lbl).tooltip({
                    'container': 'body', 'delay': {
                        "show": 500, "hide": 0
                    }
                });
            }
        }
    };
})(jQuery);
