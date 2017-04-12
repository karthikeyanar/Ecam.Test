/* ===========================================================
* jquery.autofix_anything.js v1
* ===========================================================
* Copyright 2013 Pete Rojwongsuriya.
* http://www.thepetedesign.com
*
* Fix position of anything on your website automatically
* with one js call
*
* https://github.com/peachananr/autofix_anything
*
* ========================================================== */

!function($) {

    var defaults={
        customOffset: false,
        manual: false,
        onlyInContainer: true,
        manualCSS: false
    };

    $.fn.autofix_anything=function(options) {
        var settings=$.extend({},defaults,options),
        el=$(this),
        curpos=el.position(),
        offset=settings.customOffset,
        pos=el.offset();

        el.addClass("autofix_sb")

        $.fn.manualfix=function() {
            var el=$(this),
            pos=el.offset();
            if(el.hasClass("fixed")) {
                el.removeClass("fixed")
            } else {
                el.addClass("fixed");
                if(settings.manualCSS==false) {
                    el.css({
                        top: 0,
                        left: pos.left,
                        right: "auto",
                        bottom: "auto"
                    });
                }
            }
        }

        fixAll=function(el,settings,curpos,pos) {
            if(el.hasClass("autofix_sb")) {
                if(settings.customOffset==false) offset=el.parent().offset().top
                if($(document).scrollTop()>offset&&$(document).scrollTop()<=(el.parent().height()+(offset-$(window).height()))) {
                    el.removeClass("bottom").addClass("fixed");
                    if(settings.manualCSS==false) {
                        el
                        .css({
                            "top": 0,
                            "left": pos.left,
                            "right": "auto",
                            "bottom": "auto",
                            "position": "fixed",
                            "height": "100%",
                            "overflow": "auto"
                        });
                    }
                } else {
                    if($(document).scrollTop()>offset) {
                        if(settings.onlyInContainer==true) {
                            if($(document).scrollTop()>(el.parent().height()-$(window).height())) {
                                el.addClass("bottom fixed").removeAttr('style');
                                if(settings.manualCSS==false) {
                                    el.css({
                                        "top": 0,
                                        "left": curpos.left,
                                        "right": "auto",
                                        "bottom": "0",
                                        "position": "absolute",
                                        "height": "100%",
                                        "overflow": "auto"
                                    });
                                }
                            } else {
                                el.removeClass("bottom fixed").removeAttr('style');
                            }
                        }
                    } else {
                        el.removeClass("bottom fixed").removeAttr('style');
                    }
                }
            }
        }
        if(settings.manual==false) {
            $(window).bind("scroll.autofix_anything",function() {
                fixAll(el,settings,curpos,pos);
            });
            fixAll(el,settings,curpos,pos);
        }
    }
} (window.jQuery);

