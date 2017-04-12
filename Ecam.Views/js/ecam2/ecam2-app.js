var APP = new function () {

    var self = this;

    this.isHeaderFixed = false;
    this.isSidebarFixed = true;
    this.isFixedLayout = false;
    this.sidebarType = "";
    this.sidebarPosition = "";
    this.responsiveHandlers = [];
    this.addResponsiveHandler = function (func) {
        this.responsiveHandlers.push(func);
    }
    this.getViewPort = function () {
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

    this.isTouchDevice = function () {
        return ('ontouchstart' in document.documentElement);
    }

    this.scrollTo = function (el, offeset) {
        pos = el ? el.offset().top : 0;
        $('html,body').animate({
            scrollTop: pos + (offeset ? offeset : 0)
        }, 'slow');
    }

    this.scrollTop = function () {
        self.scrollTo();
    }

    this.unblockUI = function (target) {
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

    this.handleBlockUI = function (options) {
        var options = $.extend(true, {}, options);
        var message = (options.message ? options.message : "<i class='fa fa-circle-o-notch fa-spin'></i>&nbsp;&nbsp;Loading...");
        var color = (options.bgColor ? options.bgColor : "bg-primary");
        var centery = (options.verticalTop == true ? false : true);
        var basez = (options.zindex ? options.zindex : "1000");
        var overlayOpacity = (options.overlayOpacity ? options.overlayOpacity : "0.2");
        message = '<div class="blockUI-message ' + color + '"><span>' + message + '</span></div>';
        if (options.target) {
            $(options.target).block({
                message: message,
                baseZ: basez,
                centerY: centery,
                css: {
                    top: '10%',
                    border: '0',
                    padding: '0',
                    backgroundColor: 'none'
                },
                overlayCSS: {
                    backgroundColor: '#000',
                    opacity: overlayOpacity,
                    cursor: 'wait'
                }
            });
        } else {
            $.blockUI({
                message: message,
                baseZ: basez,
                centerY: centery,
                css: {
                    border: '0',
                    padding: '0',
                    backgroundColor: 'none'
                },
                overlayCSS: {
                    backgroundColor: '#000',
                    opacity: overlayOpacity,
                    cursor: 'wait'
                }
            });
        }
    }

    this.handleSlimScroll = function () {
        if ($.fn.slimScroll) {
            $('.slimscroll').each(function () {
                var $this = $(this);
                if ($this.attr("data-initialized") == "1") return;
                $this.slimScroll({
                    width: ($this.attr("data-width") ? $this.attr("data-width") : 'auto'),
                    height: ($this.attr("data-height") ? $this.attr("data-height") : '250px'),
                    size: ($this.attr("data-size") ? $this.attr("data-size") : '5px'),
                    color: ($this.attr("data-color") ? $this.attr("data-color") : '#000'),
                    position: ($this.attr("data-position") ? $this.attr("data-position") : 'right'),
                    distance: ($this.attr("data-distance") ? $this.attr("data-distance") : '1px'),
                    start: ($this.attr("data-start") ? $this.attr("data-start") : 'top'),
                    opacity: ($this.attr("data-opacity") ? $this.attr("data-opacity") : '0.4'),
                    alwaysVisible: ($this.attr("data-always-visible") == "true" ? true : false),
                    disableFadeOut: ($this.attr("data-disable-fade-out") == "true" ? true : false),
                    railVisible: ($this.attr("data-rail-visible") == "true" ? true : false),
                    railColor: ($this.attr("data-rail-color") ? $this.attr("data-rail-color") : '#fff'),
                    railOpacity: ($this.attr("data-rail-opacity") ? $this.attr("data-rail-opacity") : '.2'),
                    railDraggable: ($this.attr("data-rail-draggable") == "false" ? false : true),
                    railClass: ($this.attr("data-rail-class") ? $this.attr("data-rail-class") : 'slimScrollRail'),
                    barClass: ($this.attr("data-bar-class") ? $this.attr("data-bar-class") : 'slimScrollBar'),
                    wrapperClass: ($this.attr("data-wrapper-class") ? $this.attr("data-wrapper-class") : 'slimScrollDiv'),
                    allowPageScroll: ($this.attr("data-allow-page-scroll") == "true" ? true : false),
                    wheelStep: ($this.attr("data-wheel-step") ? $this.attr("data-wheel-step") : '20'),
                    touchScrollStep: ($this.attr("data-touch-scroll-step") ? $this.attr("data-touch-scroll-step") : '200'),
                    borderRadius: ($this.attr("data-border-radius") ? $this.attr("data-border-radius") : '0'),
                    railBorderRadius: ($this.attr("data-rail-border-radius") ? $this.attr("data-rail-border-radius") : '0'),
                    animate: ($this.attr("data-animate") == "false" ? false : true)
                });
                $this.attr("data-initialized", "1");
            });
        }
    }

    this.resizeContentHeight = function () {
        var $body = $('body');
        var $header = $('.header');
        var $footer = $('.footer');
        var $pageContainer = $('.page-container');
        var $pageContent = $('.page-content', $pageContainer);
        var $sidebar = $('.sidebar', $pageContainer);
        var $addressDiv = $('.addressDiv', $sidebar);

        var windowHeight = this.getViewPort().height;
        var headerHeight = $header.outerHeight(true);
        var footerHeight = $footer.outerHeight(true);
        var sidebarHeight = $sidebar.outerHeight(true);
        var addressDivHeight = $addressDiv.outerHeight(true);
        var height = 0;

        if ($body.hasClass("sidebar-fixed")) {
            height = windowHeight - headerHeight - footerHeight - 5;
        } else {
            if (this.getViewPort().width < 992) {
                height = windowHeight - headerHeight - footerHeight;
            } else {
                height = sidebarHeight + 20;
            }
            if (this.getViewPort().width > 1024 && (height + headerHeight + footerHeight) < windowHeight) {
                height = windowHeight - headerHeight - footerHeight - 50;
            }
        }
        $pageContent.css({ 'min-height': height + 'px' });
    }

    this.responsive_start = false;
    this.responsive = function () {
        //console.log('APP responsive');
        // reinitialize other subscribed elements
        //console.log('self.responsiveHandlers length=', self.responsiveHandlers.length);
        $("body").addClass("no-x-scroll");
        for (var i in self.responsiveHandlers) {
            var each = self.responsiveHandlers[i];
            each.call();
        }
        setTimeout(function () {
            if (self.responsive_start == false)
                $("body").removeClass("no-x-scroll");

            self.responsive_start = false;
        }, 2000);
    }

    this.handleSidebar = function () {
        var $body = $("body");
        var $header = $(".header");
        var $sidebar = $(".sidebar");
        var $sidebarContent = $(".sidebar-content", $sidebar);
        var $sidebarMenu = $(".sidebar-menu", $sidebar);
        var $pageContent = $(".page-content");

        var toggle = true;
        var slideSpeed = 200;
        var slideOffeset = -200;


        // Sidebar User  
        $(".sidebar-user", $sidebar).off('click').off('mouseenter').off('mouseleave');
        if ($body.hasClass("sidebar-fixed") == false) {
            $(".sidebar-user", $sidebar)
			.on('click', function () {
			    $(".info", this).toggleClass("show");
			}).on('mouseenter', function () {
			    $(".info", this).addClass("show");
			}).on('mouseleave', function () {
			    $(".info", this).removeClass("show");
			});
        }

        // Sidebar Menu Accordion
        $sidebarMenu.find('li.open').has('ul').children('ul').addClass('collapse in');
        $sidebarMenu.find('li').not('.open').has('ul').children('ul').addClass('collapse');

        $sidebarMenu.children('li').off('mouseenter').off('mouseleave');
        $sidebarMenu.find('li').has('ul').children('a').off('click').on('click', function (e) {
            e.preventDefault();
            var $this = $(this);
            var $currentUL = $(this).parent('li').children('ul');
            //var $currentUL=$(this).parent('li').toggleClass('open').children('ul');
            var $body = $("body");

            var dynamicScrollFunc = function () {
                if ($body.hasClass("sidebar-sm") == false && $body.hasClass("sidebar-md") == false) {
                    if ($body.hasClass("sidebar-fixed") == false) {
                        self.scrollTo($this, slideOffeset);
                    } else {
                        var offsetTop = $this.offset().top - $(window).scrollTop();
                        $sidebarContent.slimScroll({ 'scrollTo': offsetTop });
                    }
                }
                self.resizeContentHeight();
            }

            if ($currentUL.hasClass("in") == false) {
                $currentUL.slideDown(slideSpeed, function () {
                    $(this).addClass("in").css({ "display": "" });
                    dynamicScrollFunc();
                });
            } else {
                $currentUL.slideUp(slideSpeed, function () {
                    $(this).removeClass("in").css({ "display": "" });
                    dynamicScrollFunc();
                });
            }
            if (toggle) {
                $(this).parent('li').siblings().removeClass('open').children('ul.in').slideUp(slideSpeed, function () {
                    $(this).removeClass("in").css({ "display": "" });
                });
            }
        });

        $sidebar.off("mouseenter").off("mouseleave");

        // Sidebar Fixed And Sidebar Medium Fixed
        if ($body.hasClass("sidebar-fixed") && $body.hasClass("sidebar-md-fixed")) {
            $sidebar.on('mouseenter', function () {
                if ($body.hasClass("sidebar-fixed")) {
                    $body.removeClass("sidebar-md");
                }
            }).on('mouseleave', function () {
                if ($body.hasClass("sidebar-fixed")) {
                    $body.addClass("sidebar-md");
                }
            });
        }

        // Sidebar Fixed And Sidebar Small Fixed 
        if ($body.hasClass("sidebar-fixed") && $body.hasClass("sidebar-sm-fixed")) {
            $sidebar.on('mouseenter', function () {
                if ($body.hasClass("sidebar-fixed")) {
                    $body.removeClass("sidebar-sm");
                }
            }).on('mouseleave', function () {
                if ($body.hasClass("sidebar-fixed")) {
                    //console.log(1);
                    $body.addClass("sidebar-sm");
                }
            });
        }

        // Sidebar Medium,Small And Sidebar Fixed = False
        $sidebarMenu.children('li').off('mouseenter').off('mouseleave');
        if ($body.hasClass("sidebar-fixed") == false && ($body.hasClass("sidebar-sm") || $body.hasClass("sidebar-md")) && self.getViewPort().width >= 992) {
            $sidebarMenu.children('li').on('mouseleave', function (e) {
                e.preventDefault();
                $(this).removeClass('open').removeClass('hover').children('ul.in').removeClass("in").css({
                    "display": ""
                });
            })
			.on('mouseenter', function (e) {
			    var $this = $(this).removeClass("pull-up");
			    var $currentUL = $this.addClass('open hover').children('ul').addClass("in").css({ "display": "" }).css("top", "");
			    if (!$currentUL.get(0)) {
			        $this.addClass("no-submenu")
			    } else {
			        $this.removeClass("no-submenu")
			    }
			    var $menuText = $(".menu-text", $currentUL.parent()).css("top", "");

			    var offset = $this.offset();
			    var windowHeight = document.documentElement.clientHeight;
			    var scrollY = document.documentElement.scrollTop || document.body.scrollTop;
			    windowHeight += scrollY;
			    var menuTogglePositon = offset.top;
			    var menuToggleHeight = $this.innerHeight();
			    var menuHeight = $currentUL.innerHeight();
			    var headerHeight = $(".header").innerHeight();
			    var footerHeight = $(".footer").innerHeight();
			    var availbleHeight = windowHeight - headerHeight - menuTogglePositon;
			    var menuTop = 0;
			    var menuTextTop = 0;
			    if (availbleHeight < (menuHeight)) {
			        menuTextTop = (menuHeight) - availbleHeight;
			        if (menuTextTop > menuTogglePositon) {
			            menuTextTop = (menuTogglePositon - headerHeight - 5);
			        }
			        menuTop = (menuTextTop - menuToggleHeight) * -1;
			        if (menuTextTop > menuToggleHeight) {
			            $this.addClass("pull-up");
			        }
			        menuTextTop = menuTextTop * -1;
			    } else {
			        menuTextTop = 0;
			        menuTop = menuToggleHeight;
			    }
			    if ($body.hasClass("sidebar-md")) {
			        menuTop -= menuToggleHeight;
			    }
			    $currentUL.css("top", menuTop);
			    $menuText.css("top", menuTextTop);
			    $(this).siblings().removeClass('open').removeClass('hover').children('ul.in').removeClass("in").css({
			        "display": ""
			    });
			});
        }

        if ($body.hasClass("sidebar-fixed") && $body.hasClass("header-fixed") == false) {
            $sidebar
            .on("affixed.bs.affix", function () {
                self.handleSidebarSlimScroll();
            })
            .on("affixed-top.bs.affix", function () {
                self.handleSidebarSlimScroll();
            })
            .affix({
                offset: {
                    top: function () {
                        //console.log('h=', $('.header').outerHeight(true));
                        return $('.header').outerHeight(true); //(this.bottom = $('.header').outerHeight(true))
                    }
                    , bottom: 0
                }
            })
        } else {
            $(window).off('.affix')
            $sidebar.removeData('bs.affix').removeClass('affix affix-top affix-bottom')
        }

        self.handleSidebarSlimScroll();
        self.resizeContentHeight();
    }

    this.handleSidebarSlimScroll = function () {
        var $body = $("body");
        var $header = $(".header");
        var $sidebar = $(".sidebar");
        var $sidebarContent = $(".sidebar-content", $sidebar);
        // Sidebar Fixed Custom Scroll
        if ($.fn.slimScroll) {
            var windowHeight = $(window).height();
            if ($sidebarContent.parent('.slimScrollDiv').size() === 1) { // destroy existing instance  
                $sidebarContent.slimScroll({ destroy: true });
                $sidebarContent.css({ 'height': '', 'overflow': '', 'width': '' });
            }
            if ($body.hasClass("sidebar-fixed")) {
                var height = windowHeight;
                if ($body.hasClass("header-fixed") || $sidebar.hasClass("affix-top"))
                    height = height - $header.outerHeight(true);

                $sidebarContent.slimScroll({
                    allowPageScroll: false,
                    size: '5px',
                    color: '#000',
                    opacity: 1,
                    borderRadius: '0px',
                    railBorderRadius: '0px',
                    wrapperClass: 'slimScrollDiv',
                    position: 'right',
                    height: height,
                    disableFadeOut: true
                });
            }
        }
    }

    this.handleHorizontalMenu = function () {
        var $header = $(".header");
        var $horMenu = $(".hor-menu", $header);
        if (this.isTouchDevice() == true) {
            $horMenu.find('.dropdown-menu').click(function (e) {
                e.stopPropagation();
            });
            // handle submenus
            $horMenu.find('.dropdown-submenu').each(function () {
                var $this = $(this);
                var subTimeout;
                $this.click(function () {
                    $this.toggleClass("open");
                });
            });
        }
    }

    this.changeLayout = function () {
        self.applyLayoutClasses();
        self.handleSidebar();
        //console.log('APP ChangeLayout');
        self.responsive_start = true;
        setTimeout(function () {
            self.responsive();
        }, 500);
    }

    this.applyLayoutClasses = function () {
        var $body = $("body");
        var $header = $(".header");
        var $headerContent = $(".header-content", $header);
        var $pageContainer = $(".page-container");

        if (this.sidebarPosition == "right")
            $body.addClass("sidebar-right");
        else
            $body.removeClass("sidebar-right");

        $body
		.removeClass("sidebar-sm")
		.removeClass("sidebar-sm-fixed")
		.removeClass("sidebar-md")
		.removeClass("sidebar-md-fixed")
        .addClass("show-left-menu")
        ;

        if (self.sidebarType == "sm") {
            $body.addClass("sidebar-sm").removeClass("show-left-menu");
        } else if (self.sidebarType == "md") {
            $body.addClass("sidebar-md").removeClass("show-left-menu");
        }

        if (this.isSidebarFixed) {
            $body.addClass("sidebar-fixed");
            if ($body.hasClass("sidebar-sm"))
                $body.addClass("sidebar-sm-fixed")
            else if ($body.hasClass("sidebar-md"))
                $body.addClass("sidebar-md-fixed")

            if (self.getViewPort().width < 992 && $body.hasClass("sidebar-open")) {
                $body.addClass("page-container-no-scroll");
            }
        } else {
            $body.removeClass("sidebar-fixed")
			.removeClass("sidebar-sm-fixed")
			.removeClass("sidebar-md-fixed")
			.removeClass("page-container-no-scroll")
            ;
        }

        if (this.isHeaderFixed) {
            $body.addClass("header-fixed");
            $header.addClass("navbar-fixed-top");
        } else {
            $body.removeClass("header-fixed");
            $header.removeClass("navbar-fixed-top");
        }

        if (this.isFixedLayout && this.getViewPort().width >= 1024) {
            $body.addClass("page-boxed");
            $headerContent.addClass("container").removeClass("container-fluid");
            $pageContainer.addClass("container").removeClass("container-fluid");
        } else {
            $headerContent.removeClass("container").addClass("container-fluid");
            $pageContainer.removeClass("container").addClass("container-fluid");
        }
    }

    this.init = function () {
        self._IsIE8 = !!navigator.userAgent.match(/MSIE 8.0/);
        self._IsIE9 = !!navigator.userAgent.match(/MSIE 9.0/);
        self._IsIE10 = !!navigator.userAgent.match(/MSIE 10.0/);

        var $body = $("body");
        this.isSidebarFixed = $body.hasClass("sidebar-fixed");
        this.isFixedLayout = $body.hasClass("page-boxed");

        if ($body.hasClass("sidebar-md")) {
            this.sidebarType = "md";
        } else if ($body.hasClass("sidebar-sm")) {
            this.sidebarType = "sm";
        }
        // Layout,Sidebar
        self.applyLayoutClasses();
        self.handleSidebar();
        self.handleHorizontalMenu();

        // Apply sidebar toggle event
        $("body").addClass("sidebar-animation");
        //console.log('APP INIT=1');
        $("[data-sidebar-action='sidebar-toggle']").off('click').on("click", function () {
            var type = $(this).attr("data-sidebar-type");
            if (type == "") type = "sm";
            if (self.sidebarType == type) {
                self.sidebarType = "";
            } else {
                self.sidebarType = type;
            }
            //console.log('Call SideBar Click=', 1);
            self.changeLayout();
        });
        // Small Devices Sidebar Toggle
        $("[data-sidebar-action='sidebar-sm-toggle']").off('click').on('click', function (e) {
            e.preventDefault();
            e.stopPropagation();
            var $body = $("body");
            $body.toggleClass("sidebar-open");
            if ($body.hasClass("sidebar-fixed") && $body.hasClass("sidebar-open")) {
                $body.addClass("page-container-no-scroll");
            } else {
                $body.removeClass("page-container-no-scroll");
            }
        });

        $(".search-form-toggle").off('click').on('click', function (e) {
            $(".search-form", ".header").toggleClass("show");
        });

        // Plugins
        self.handleSlimScroll();
    }

    this.changeMenuView = function (isDockMobile) {
        var $body = $("body");
        $body.removeClass("left-menu-view").removeClass("top-menu-view").removeClass("show-left-menu");
        var $this = $(this);
        if (isDockMobile == '') { isDockMobile = 'false'; }
        //if($(window).width()<780) {
        //    isDockMobile='true';
        //}
        //console.log('isDockMobile=',isDockMobile);
        if (isDockMobile == 'true') {
            $body.addClass("left-menu-view").addClass("show-left-menu");
        } else {
            $body.addClass("top-menu-view").removeClass("sidebar-sm");
        }        
        setLS("is_dock_mobile", isDockMobile);
        self.resizeContentHeight();
        self.responsive_start = true;
        setTimeout(function () {
            self.responsive();
        }, 500);
        if ((isDockMobile==true || isDockMobile=="true") && $body.hasClass("sidebar-fixed") && $body.hasClass("sidebar-sm-fixed")) {
            if ($body.hasClass("sidebar-fixed")) {
                $body.addClass("sidebar-sm");
            }
        }
    }
}
$(window).resize(function () {
    APP.handleSidebar();
    //APP.responsive();
});
pushSetMyEvent(function () {
    var isDockMobile = cString(getLS('is_dock_mobile'));
    APP.changeMenuView(isDockMobile);
    APP.init();
    $("body").off("click", ".menu-nav-lnk");
    $("body").on("click", ".menu-nav-lnk", function (event) {
        $("#ui-id-1").hide();
        var $this = $(this);
        APP.changeMenuView(cString($this.hasClass('left')));
    });
    $("body").off("click", ".page-content,.menu-bar");
    $("body").on("click", ".page-content,.menu-bar", function (event) {
        $("#ui-id-1").hide();
    });
    // var prevLeft = 0;
    // $(document).scroll(function (evt) {
    //     var currentLeft = $(this).scrollLeft();
    //     //console.log('currentLeft',currentLeft);
    //     if (prevLeft === currentLeft) {
    //         //console.log("I scrolled vertically.");
    //     }
    //     else {
    //         prevLeft = currentLeft;
    //         //var px = parseInt($(".page-container .sidebar").css('left').replace('px',''))+currentLeft; //console.log(px);
    //         $(".page-container .sidebar").css('left', currentLeft + 'px');
    //     }
    // });
    $('.dropdown-menu [data-toggle=dropdown]').unbind().click(function (event) {
        event.preventDefault();
        event.stopPropagation();
        $(this).parent().siblings().removeClass('open');
        $(this).parent().toggleClass('open');
    });
});
