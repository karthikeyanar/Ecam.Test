"use strict";
define("helper", function () {


    var helper = new function () {

        var self = this;

        this.themesPath = "/css/themes/";
        this.autoCompletePageSize = 50;
        this.deleteErrorMessage = "Cann't Delete! Child records found!";
        // IE mode
        this._isIE = false;
        this._IsIE8 = false;
        this._IsIE9 = false;
        this._IsIE10 = false;
        this._IsIE11 = false;

        this.isIE = function () {
            if (navigator.userAgent.match(/Trident\/7\./)) {
                return true;
            } else {
                if (navigator.userAgent.match(/MSIE/)) {
                    return true;
                } else {
                    return false;
                }
            }
        }

        this.isIE11 = function () {
            return self._IsIE11;
        }

        this.isIE8 = function () {
            return self._IsIE8;
        }

        this.isIE9 = function () {
            return self._IsIE9;
        }

        this.isIE10 = function () {
            return self._IsIE10;
        }

        this.isTouchDevice = function () {
            return ('ontouchstart' in document.documentElement);
        }

        this.scrollTo = function (el, offeset) {
            var pos = el ? el.offset().top : 0;
            $('html,body').animate({
                scrollTop: pos + (offeset ? offeset : 0)
            }, 0);
        }

        this.scrollTop = function () {
            self.scrollTo();
        }

        this.scrollBottom = function () {
            var WH = $(window).height();
            var SH = $('body')[0].scrollHeight;
            $('html, body').stop().animate({ scrollTop: SH - WH });
        }

        this.Left = function (str, n) {
            if (n <= 0)
                return "";
            else if (n > String(str).length)
                return str;
            else
                return String(str).substring(0, n);
        }

        this.Right = function (str, n) {
            if (n <= 0)
                return "";
            else if (n > String(str).length)
                return str;
            else {
                var iLen = String(str).length;
                return String(str).substring(iLen, iLen - n);
            }
        }

        this.destroyFixedHeader = function ($t) {
            try {
                $t.floatThead('destroy');
            } catch (e) { }
        }

        this.fixedHeader = function ($t, opts) {
            try {
                $t.floatThead('destroy');
            } catch (e) {
            }
            try {
                $t.floatThead(opts);
            } catch (e) {
            }
            return;
        }

        this.getScrollTop = function () {
            return 80;
        }


        this.applySorting = function (element, options) {
            var $element = $(element);
            var $thead = $("thead", element);
            $thead.find("th")
			.removeClass("sort").removeClass("asc").removeClass("desc")
			.each(function () {
			    var $this = $(this);
			    var displayName = $this.attr("displayname");
			    if (displayName == undefined) {
			        displayName = $this.html();
			    }
			    $this.empty();
			    var $div = $("<div></div>").html(displayName);
			    //console.log(11);
			    if ($this.hasClass('text-right')) {
			        $div.addClass('text-right').addClass('pull-right');
			    } else if ($this.hasClass('text-center')) {
			        $div.addClass('text-center');
			    }
			    var width = $this.attr("display-width");
			    if (width != undefined) {
			        $div.width(width);
			    }
			    $this.empty().append($div);
			    if ($element.attr("sortname") == $this.attr("sortname")) {
			        $this.addClass("sort").addClass($element.attr("sortorder"));
			    }
			    $this.unbind("click").click(function () {
			        var sortName = cString($this.attr("sortname"));
			        if (sortName == "") return;
			        var sortorder = cString($element.attr("sortorder"));
			        if (sortorder == "asc" || sortorder == "") { sortorder = "desc"; } else { sortorder = "asc"; }
			        $element.attr("sortorder", sortorder);
			        $this.removeClass("sort").removeClass("asc").removeClass("desc").siblings().removeClass("sort").removeClass("asc").removeClass("desc");
			        $this.addClass("sort").addClass(sortorder);
			        if (options.onSorting) {
			            options.onSorting(sortName, $element.attr("sortorder"));
			        }
			    });
			});
        }

        this.sortingTable = function (element, options) {
            var $element = $(element);
            var $thead = $("thead", element);
            self.applySorting($element, options);

            var $floatTheadWrapper = $element.parents(".floatThead-wrapper:first");
            if ($floatTheadWrapper[0]) {
                var $floatTheadContainer = $(".floatThead-container", $floatTheadWrapper);
                var $tbl = $("table", $floatTheadContainer);
                self.applySorting($tbl[0], options);
            }
        }

        this.handleMetroCheck = function (target) {
            if (!target) target = $("body");

            $('.metro-checkbox,.metro-radio', $(target)).find(":input").each(function () {
                var $this = $(this);
                if ($this.closest("label").size() == 1) {
                    if ($this.closest("label").find(".check").size() == 0)
                        $this.after("<span class='check'></span>");
                }
            });
        }

        this.handleFormValidation = function () {
            if ($.fn.validate) {
                $(".form-validate").validate({
                    ignore: "input[type='text']:hidden"
                });
            }
        }

        this.hideAllToolTips = function () {
            $('[data-toggle="tooltip"]').tooltip('hide');
            $('[data-original-title]').tooltip('hide');
            $('[rel="tooltip"]').tooltip('hide');
        }

        this.handleToolTip = function (target) {
            if (!target) target = $("body");

            $("[data-toggle='tooltip']", target).tooltip({
                'container': 'body', 'delay': {
                    "show": 300, "hide": 0
                }
            });
            $("[rel='tooltip']", target).tooltip({
                'container': 'body', 'delay': {
                    "show": 300, "hide": 0
                }
            });
        }

        this.handleConfirmPopover = function (target) {
            if (!target) target = $("body");
            $("[data-toggle='confirmation']", target).confirmation();
        }

        this.handleDropdownHoldClick = function () {
            $('body').on('click', '.dropdown-menu.hold-on-click', function (e) {
                e.stopPropagation();
            })
        }

        this.alert = function (msg, callback) {
            msg = msg.replace(/\n/g, '<br />');
            bootbox.jAlert({
                "message": msg,
                "animate": false,
                "callback": function () {
                    if (callback)
                        callback();
                }
            });
        }

        this.selectMenu = function (menuName) {
            var url = SH(window.location.href.toString()).replaceAll(window.location.origin.toString(), '').replaceAll('/#', '#').s;
            var $menuBar = $(".sidebar-wrapper");
            var $menuList = (localStorage.getItem('dock_mobile') == "true" || $(window).width() < 780) ? $(".sidebar-menu", $menuBar) : $(".sf-menu", $menuBar);
            $("li.active", $menuList).each(function () {
                $("ul > li", this).removeClass("open");
                $(this).removeClass("active");
            });
            $("a[href='" + url + "']", $menuList).each(function () {
                var $this = $(this);
                var $dm = $this.closest('li');
                $this.closest('ul').closest('li').addClass("active open");
                if ($dm[0]) {
                    $dm.addClass("active open").siblings().removeClass("active open");
                } else {
                    $this.parent().addClass("active open").siblings().removeClass("active open");
                }
            });
        }

        this.handleBackToTop = function () {
            var offset = 220;
            var duration = 500;
            var $button = $('<a href="javascript:;" class="back-to-top"><i class="fa fa-angle-up"></i></a>');
            $(".back-to-top").remove();
            $("body").append($button);

            $(window).scroll(function () {
                if ($(this).scrollTop() > offset) {
                    $('.back-to-top').fadeIn(duration);
                } else {
                    $('.back-to-top').fadeOut(duration);
                }
            });

            $('.back-to-top').click(function (event) {
                event.preventDefault();
                self.scrollTop();
                return false;
            });

        }

        this.getThisMonth = function () {
            var fromDate = new Date();
            var getDt = fromDate.getDate();
            var startDate = moment(_TODAYDATE).startOf('month').format("YYYY-MM-DD");
            var endDate = moment(_TODAYDATE).endOf('month').format("YYYY-MM-DD");
            var arr = [];
            arr.push(startDate);
            arr.push(endDate);
            return arr;
        }

        this.getLastSixMonths = function () {
            var fromDate = new Date();
            var getDt = fromDate.getDate();
            var startDate = moment(fromDate).subtract('month', 5).startOf('month').format("YYYY-MM-DD");
            var endDate = moment(fromDate).endOf('month').format("YYYY-MM-DD");
            var arr = [];
            arr.push(startDate);
            arr.push(endDate);
            return arr;
        }

        this.calcFortNights = function (fromDate) {
            var getDt = fromDate.getDate();
            var thisMonthStartDate = moment(fromDate).startOf('month').format("YYYY-MM-DD");
            var thisMonthEndDate = moment(fromDate).endOf('month').format("YYYY-MM-DD");
            var thisMonth15 = moment(fromDate).startOf('month').format("YYYY-MM") + "-15";
            var thisMonth16 = moment(fromDate).startOf('month').format("YYYY-MM") + "-16";
            var fnStartDate = "";
            var fnEndDate = "";
            if (getDt < 16) {
                fnStartDate = thisMonthStartDate;
                fnEndDate = thisMonth15;
            } else {
                fnStartDate = thisMonth16;
                fnEndDate = thisMonthEndDate;
            }
            var arr = [];
            arr.push(fnStartDate);
            arr.push(fnEndDate);
            return arr;
        }

        this.getFortNightDates = function () {
            var todayDate = new Date();
            var arr = self.calcFortNights(todayDate);
            var getDt = todayDate.getDate();
            var fromDate = "";
            if (getDt < 16) {
                fromDate = moment(_TODAYDATE).subtract('month', 1).endOf('month').format("MM/DD/YYYY");
            } else {
                fromDate = moment(_TODAYDATE).startOf('month').format("MM/DD/YYYY");
            }
            var lastArr = self.calcFortNights(new Date(fromDate));
            $.each(lastArr, function (i, d) {
                arr.push(d);
            });
            return arr;
        }

        this.changeDateRangeLabel = function ($span, start, end, currentStartDate, currentEndDate) {
            var dthtml = start.format('MMM D, YYYY') + ' - ' + end.format('MMM D, YYYY');
            var checkStart = moment(_TODAYDATE).startOf('week').format("MM/DD/YYYY");
            var checkEnd = moment(_TODAYDATE).endOf('week').format("MM/DD/YYYY");
            if (checkStart == currentStartDate && checkEnd == currentEndDate) {
                dthtml = "This Week";
            }
            checkStart = moment(_TODAYDATE).startOf('month').format("MM/DD/YYYY");
            checkEnd = moment(_TODAYDATE).endOf('month').format("MM/DD/YYYY");
            if (checkStart == currentStartDate && checkEnd == currentEndDate) {
                dthtml = "This Month";
            }
            checkStart = moment(_TODAYDATE).subtract('year', 1).startOf('year').format("MM/DD/YYYY");
            checkEnd = moment(_TODAYDATE).subtract('year', 1).endOf('year').format("MM/DD/YYYY");
            if (checkStart == currentStartDate && checkEnd == currentEndDate) {
                dthtml = "Last Year";
            }
            checkStart = moment(_TODAYDATE).startOf('year').format("MM/DD/YYYY");
            checkEnd = moment(_TODAYDATE).endOf('year').format("MM/DD/YYYY");
            if (checkStart == currentStartDate && checkEnd == currentEndDate) {
                dthtml = "This Year";
            }
            checkStart = moment(_TODAYDATE).format("MM/DD/YYYY");
            checkEnd = moment(_TODAYDATE).format("MM/DD/YYYY");
            //console.log('checkStart=',checkStart,'currentStartDate=',currentStartDate);
            if (checkStart == currentStartDate && checkEnd == currentEndDate) {
                dthtml = "Today";
            }
            var arrfndates = helper.getFortNightDates();
            var fnStartDate = arrfndates[0];
            var fnEndDate = arrfndates[1];
            var lastFnStartDate = arrfndates[2];
            var lastFnEndDate = arrfndates[3];
            if (fnStartDate == start.format('YYYY-MM-DD') && fnEndDate == end.format('YYYY-MM-DD')) {
                dthtml = "Current Two Weeks";
                //dthtml="This Fort Night";
            }
            if (lastFnStartDate == start.format('YYYY-MM-DD') && lastFnEndDate == end.format('YYYY-MM-DD')) {
                dthtml = "Last Two Weeks";
                //dthtml = "Last Fort Night";
            }
            var arr = self.getLastSixMonths();
            var startDate = arr[0];
            var endDate = arr[1];
            if (startDate == start.format('YYYY-MM-DD') && endDate == end.format('YYYY-MM-DD')) {
                dthtml = "Last Six Months";
            }
            $span.html(dthtml);
        }

        this.handleDateRangePicker = function ($ctl, options) {
            var arrfndates = helper.getFortNightDates();
            var fnStartDate = arrfndates[0];
            var fnEndDate = arrfndates[1];
            var lastFnStartDate = arrfndates[2];
            var lastFnEndDate = arrfndates[3];
            var arrlastsixmonths = helper.getLastSixMonths();
            //var daysLimit = 365;
            //if (cInt(options.daysLimit) > 0) {
            //    daysLimit = cInt(options.daysLimit);
            //}
            var openMode = "";
            if (options.opens == undefined) {
                openMode = 'left';
            } else {
                openMode = options.opens;
            }
            var ranges = options.ranges;
             
            if (!ranges) {
                ranges = {
                    'Today': [moment(_TODAYDATE), moment(_TODAYDATE)],
                    'Last Week': [moment(_TODAYDATE).startOf('week').subtract('day', 7), moment(_TODAYDATE).startOf('week').subtract('day', 7).endOf('week'), ['left']],
                    'Last Two Weeks': [moment(_TODAYDATE).startOf('week').subtract('day', 14), moment(_TODAYDATE).startOf('week').subtract('day', 7).endOf('week'), ['left']],
                    'Last Month': [moment(_TODAYDATE).subtract('month', 1).startOf('month'), moment(_TODAYDATE).subtract('month', 1).endOf('month'), ['left']],
                    'Last 2 Month': [moment(_TODAYDATE).subtract('month', 2).startOf('month'), moment(_TODAYDATE).subtract('month', 1).endOf('month'), ['left']],
                    'Last 3 Month': [moment(_TODAYDATE).subtract('month', 3).startOf('month'), moment(_TODAYDATE).subtract('month', 1).endOf('month'), ['left']],
                    'Last 4 Month': [moment(_TODAYDATE).subtract('month', 4).startOf('month'), moment(_TODAYDATE).subtract('month', 1).endOf('month'), ['left']],
                    'Last 6 Months': [moment(arrlastsixmonths[0]), moment(arrlastsixmonths[1]), ['left']],
                    'Last Year': [moment(_TODAYDATE).subtract('year', 1).startOf('year'), moment(_TODAYDATE).subtract('year', 1).endOf('year'), ['left']],
                    'This Month': [moment(_TODAYDATE).startOf('month'), moment(_TODAYDATE).endOf('month'), ['left']],

                    'Yesterday': [moment(_TODAYDATE).subtract('days', 1), moment(_TODAYDATE).subtract('days', 1), ['right']],
                    'This Week': [moment(_TODAYDATE).startOf('week'), moment(_TODAYDATE).endOf('week'), ['right']],
                    'Last 19 Days': [moment(_TODAYDATE).subtract('days', 19), moment(_TODAYDATE), ['right']],
                    'Last 30 Days': [moment(_TODAYDATE).subtract('days', 30), moment(_TODAYDATE), ['right']],
                    'Last 60 Days': [moment(_TODAYDATE).subtract('days', 60), moment(_TODAYDATE), ['right']],
                    'Last 90 Days': [moment(_TODAYDATE).subtract('days', 90), moment(_TODAYDATE), ['right']],
                    'Last 182 Days': [moment(_TODAYDATE).subtract('days', 182), moment(_TODAYDATE), ['right']],
                    'Last 365 Days': [moment(_TODAYDATE).subtract('days', 365), moment(_TODAYDATE), ['right']],
                    'This Year': [moment(_TODAYDATE).startOf('year'), moment(_TODAYDATE).endOf('year'), ['right']]
                };
            }

            $ctl.daterangepicker({
                startDate: options.start,
                endDate: options.end,
                //minDate: '01/01/2000',
                //maxDate: '12/31/2014',
                //dateLimit: { days: daysLimit },
                showDropdowns: true,
                showWeekNumbers: true,
                timePicker: false,
                timePickerIncrement: 1,
                timePicker12Hour: true,
                ranges: ranges,
                opens: openMode,
                buttonClasses: ['btn btn-default btn-sm'],
                applyClass: 'btn-primary btn-sm',
                cancelClass: 'btn btn-sm',
                format: 'MM/DD/YYYY',
                separator: ' to ',
                locale: {
                    applyLabel: 'Submit',
                    fromLabel: 'From',
                    toLabel: 'To',
                    customRangeLabel: 'Custom Range',
                    daysOfWeek: ['Su', 'Mo', 'Tu', 'We', 'Th', 'Fr', 'Sa'],
                    monthNames: ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'],
                    firstDay: 1
                }
            }, function (start, end) {
                if (options.changeDate)
                    options.changeDate(start, end);
            });

        }

        this.getTimeDiff = function (earlierDate, laterDate) {
            //console.log('getTimeDiff earlierDate=',earlierDate);
            //console.log('getTimeDiff laterDate=',laterDate);
            return getTimeDifference(earlierDate, laterDate);
        }


        this.jqValidationSetDefaults = function () {
            if ($.validator) {
                $.validator.setDefaults({
                    highlight: function (element) {
                        //console.log('element=',$(element)[0]);
                        //console.log('parent=',$(element).parent()[0]);
                        if ($(element).closest('.form-valid-cnt').size() === 1)
                            $(element).closest('.form-valid-cnt').addClass('has-error');
                        else
                            $(element).closest('.form-group').addClass('has-error');
                    },
                    unhighlight: function (element) {
                        if ($(element).closest('.form-valid-cnt').size() === 1)
                            $(element).closest('.form-valid-cnt').removeClass('has-error');
                        else
                            $(element).closest('.form-group').removeClass('has-error');
                    },
                    errorElement: 'span',
                    errorClass: 'help-block',
                    errorPlacement: function (error, element) {
                        if (element.closest('.input-group').size() === 1) {
                            error.insertAfter(element.closest('.input-group'));
                        } else if (element.closest('.input-icon').size() === 1) {
                            error.insertAfter(element.closest('.input-icon'));
                        } else if (element.closest('.select-group').size() === 1) {
                            error.insertAfter(element.closest('.select-group'));
                        } else {
                            error.insertAfter(element);
                        }
                    }
                });
            }
        }

        this.loadScript = function (files, callback) {
            $.each(files, function (i, fileName) {
                try {
                    self.removeScript(fileName);
                } catch (e) { }
                var fileRef = document.createElement("script");
                fileRef.setAttribute("type", "text/javascript");
                fileRef.setAttribute("src", fileName + "?v=" + _Version);
                fileRef.setAttribute("id", self.getScriptFileId(fileName));
                document.getElementsByTagName("head")[0].appendChild(fileRef);
            });
            if (callback) {
                callback();
            }
        }
        this.getScriptFileId = function (fileName) {
            fileName = fileName.replaceAll("/", "_").replaceAll(".js", "").replaceAll(".", "").replaceAll("-", "_") + "_js_file";
            return fileName;
        }
        this.removeScript = function (files, callback) {
            $.each(files, function (i, fileName) {
                $("#" + self.getScriptFileId(fileName)).remove();
            });
            if (callback) {
                callback();
            }
        }

        this.loadCSS = function (files, callback) {
            $.each(files, function (i, fileName) {
                try {
                    self.removeCSS(fileName, true);
                } catch (e) { }
                var fileRef = document.createElement("link");
                fileRef.setAttribute("rel", "stylesheet");
                fileRef.setAttribute("type", "text/css");
                fileRef.setAttribute("href", fileName + "?v=" + _Version);
                fileRef.setAttribute("id", self.getCSSFileId(fileName));
                document.getElementsByTagName("head")[0].appendChild(fileRef);
            });
            if (callback) {
                callback();
            }
        }
        this.getCSSFileId = function (fileName) {
            return fileName.replaceAll("/", "_").replaceAll(".css", "").replaceAll(".", "").replaceAll("-", "_") + "_css_file";
        }
        this.removeCSS = function (files, isNotInterval) {
            if (isNotInterval == true) {
                $.each(files, function (i, fileName) {
                    //console.log('removeCSS fileName=', fileName);
                    $("#" + self.getCSSFileId(fileName)).remove();
                });
            } else {
                setTimeout(function () {
                    $.each(files, function (i, fileName) {
                        //console.log('removeCSS fileName1=', fileName);
                        $("#" + self.getCSSFileId(fileName)).remove();
                    });
                }, 2000);
            }
        }
        this.appModel = null;
        this.addAuthHeader = function (xhr) {
            //var authToken = getAuth();
            //if (authToken != null) {
            //    xhr.setRequestHeader("Authorization", "Bearer " + authToken.access_token);
            //}
        }
        this.onAjaxError = null;
        this.ajaxSetup = function () {
            $.ajaxSetup({
                beforeSend: function (xhr) {
                    helper.addAuthHeader(xhr);
                },
                error: function (jqXHR, exception) {
                    if (helper.onAjaxError) {
                        helper.onAjaxError(jqXHR, exception);
                    }
                    /*
                    if(jqXHR.status===0) {
                    jAlert('Not connect. Verify Network.');
                    } else if(jqXHR.status==404) {
                    jAlert('Requested page not found');
                    } else if(jqXHR.status==500) {
                    jAlert('Internal Server Error');
                    } else if(jqXHR.status==401) {
                    jAlert('Unauthorized');
                    } else if(exception==='parsererror') {
                    jAlert('Requested JSON parse failed.');
                    } else if(exception==='timeout') {
                    jAlert('Time out error.');
                    } else if(exception==='abort') {
                    jAlert('Ajax request aborted.');
                    } else {
                    jAlert('Uncaught Error.\n'+jqXHR.responseText);
                    }
                    */
                },
                success: function (event, xhr, settings) {
                    //try {
                    //    if (event.Message == 'Authorization has been denied for this request.') {
                    //        goToLogin();
                    //    }
                    //} catch (e) {
                    //}
                }
            });
        }

        this.tempData = [];
        this.clearTempData = function () {
            self.tempData = [];
        }
        this.getTempData = function (key) {
            var data = null;
            $.each(self.tempData, function (i, d) {
                if (d.key == key) {
                    data = d.data;
                }
            })
            return data;
        }
        this.setTempData = function (key, data) {
            var existData = self.getTempData(key);
            if (existData == null)
                self.tempData.push({ "key": key, "data": data });
            else
                existData.data = data;
        }
        this.onInit = null;
        this.init = function () {
            self._IsIE8 = !!navigator.userAgent.match(/MSIE 8.0/);
            self._IsIE9 = !!navigator.userAgent.match(/MSIE 9.0/);
            self._IsIE10 = !!navigator.userAgent.match(/MSIE 10.0/);
            // Plugins
            self.jqValidationSetDefaults();
            self.handleFormValidation();
            self.handleToolTip();
            self.handleConfirmPopover();
            self.handleDropdownHoldClick();
            self.handleMetroCheck();
            self.handleBackToTop();
            self.ajaxSetup();
        }
    }
    helper.init();
    return helper;
});

