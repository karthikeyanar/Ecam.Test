(function(a){function q(a,k,y){if(8==g){var d=e.width();a=b.debounce(function(){var a=e.width();d!=a&&(d=a,y())},a);e.on(k,a)}else e.on(k,b.debounce(y,a))}function V(){var h=a('<div style="width:50px;height:50px;overflow-y:scroll;position:absolute;top:-200px;left:-200px;"><div style="height:100px;width:100%"></div>');a("body").append(h);var b=h.innerWidth(),e=a("div",h).innerWidth();h.remove();return b-e}function y(a){if(a.dataTableSettings)for(var b=0;b<a.dataTableSettings.length;b++)if(a[0]==a.dataTableSettings[b].nTable)return!0;
return!1}a.floatThead=a.floatThead||{};a.floatThead.defaults={cellTag:null,headerCellSelector:"tr:first>th:visible",zIndex:1001,debounceResizeMs:10,useAbsolutePositioning:!0,scrollingTop:0,scrollingBottom:0,scrollContainer:function(b){return a([])},getSizingRow:function(a,b,e){return a.find("tbody tr:visible:first>*")},floatTableClass:"floatThead-table",floatWrapperClass:"floatThead-wrapper",floatContainerClass:"floatThead-container",copyTableClass:!0,debug:!1};var b=window._,g=function(){for(var a=
3,b=document.createElement("b"),e=b.all||[];a=1+a,b.innerHTML="\x3c!--[if gt IE "+a+"]><i><![endif]--\x3e",e[0];);return 4<a?a:document.documentMode}(),u=null,r=function(){if(g)return!1;var b=a("<table><colgroup><col></colgroup><tbody><tr><td style='width:10px'></td></tbody></table>");a("body").append(b);var e=b.find("col").width();b.remove();return 0==e},e=a(window),ea=0;a.fn.floatThead=function(h){h=h||{};b||(b=window._||a.floatThead._)||window.console.log("jquery.floatThead-slim.js requires underscore. You should use the non-lite version since you do not have underscore.");
if(8>g)return this;null==u&&(u=r())&&(document.createElement("fthtr"),document.createElement("fthtd"),document.createElement("fthfoot"));if(b.isString(h)){var k=h,D=this;this.filter("table").each(function(){var d=a(this).data("floatThead-attached");d&&b.isFunction(d[k])&&(d=d[k](),"undefined"!==typeof d&&(D=d))});return D}var d=a.extend({},a.floatThead.defaults||{},h);a.each(h,function(e,y){if(!(e in a.floatThead.defaults)&&d.debug){var h="jQuery.floatThead: used ["+e+"] key to init plugin, but that param is not an option for the plugin. Valid options are: "+
b.keys(a.floatThead.defaults).join(", ");window.console&&window.console&&window.console.log&&window.console.log(h)}});this.filter(":not(."+d.floatTableClass+")").each(function(){function h(f){return f+".fth-"+fa+".floatTHead"}function k(){var f=0;z.find("tr:visible").each(function(){f+=a(this).outerHeight(!0)});L.outerHeight(f);ga.outerHeight(f)}function r(){A=(b.isFunction(d.scrollingTop)?d.scrollingTop(c):d.scrollingTop)||0;ha=(b.isFunction(d.scrollingBottom)?d.scrollingBottom(c):d.scrollingBottom)||
0}function D(){var f,c;M?f=B.find("col").length:(c=z.find(null==d.cellTag&&d.headerCellSelector?d.headerCellSelector:"tr:first>"+d.cellTag),f=0,c.each(function(){f+=parseInt(a(this).attr("colspan")||1,10)}));if(f!=ia){ia=f;c=[];for(var b=[],w=[],e=0;e<f;e++)c.push('<th class="floatThead-col"/>'),b.push("<col/>"),w.push("<fthtd style='display:table-cell;height:0;width:auto;'/>");b=b.join("");c=c.join("");u&&(w=w.join(""),W.html(w),X=W.find("fthtd"));L.html(c);ga=L.find("th");M||B.html(b);N=B.find("col");
Y.html(b);ja=Y.find("col")}return f}function O(){if(!G){G=!0;if(n){var f=c.width(),a=P.width();f>a&&c.css("minWidth",f)}c.css(ka);p.css(ka);p.append(z);pa.before(H);k()}}function Q(){G&&(G=!1,n&&c.width(qa),H.detach(),c.prepend(z),c.css(I),p.css(I))}function la(f){n!=f&&(n=f,l.css({position:n?"absolute":"fixed"}))}function Z(){var f,a=D();return function(){var b;b=N;b=u?X:g?d.getSizingRow(c,b,X):b;if(b.length==a&&0<a){if(!M)for(f=0;f<a;f++)N.eq(f).css("width","");Q();var e=[];for(f=0;f<a;f++)e[f]=
b.get(f).offsetWidth;for(f=0;f<a;f++)ja.eq(f).width(e[f]),N.eq(f).width(e[f]);O()}else p.append(z),c.css(I),p.css(I),k()}}function ma(f){f=m.css("border-"+f+"-width");var a=0;f&&~f.indexOf("px")&&(a=parseInt(f,10));return a}function aa(){var a=m.scrollTop(),b,d=0,w=R?S.outerHeight(!0):0,h=T?w:-w,y=l.height(),k=c.offset(),p=0;if(x){var g=m.offset(),d=k.top-g.top+a;R&&T&&(d+=w);d-=ma("top");p=ma("left")}else b=k.top-A-y+ha+J.horizontal;var v=e.scrollTop(),r=e.scrollLeft(),q=m.scrollLeft(),a=m.scrollTop();
return function(l){"windowScroll"==l?(v=e.scrollTop(),r=e.scrollLeft()):"containerScroll"==l?(a=m.scrollTop(),q=m.scrollLeft()):"init"!=l&&(v=e.scrollTop(),r=e.scrollLeft(),a=m.scrollTop(),q=m.scrollLeft());if(!u||!(0>v||0>r)){if(ra)"windowScrollDone"==l?la(!0):la(!1);else if("windowScrollDone"==l)return null;k=c.offset();R&&T&&(k.top+=w);var g,t;l=c.outerHeight();x&&n?(d>=a?(g=d-a,g=0<g?g:0):g=ba?0:a,t=p):!x&&n?(v>b+l+h?g=l-y+h:k.top>v+A?(g=0,Q()):(g=A+v-k.top+d+(T?w:0),O()),t=0):x&&!n?(d>a||a-d>
l?(g=k.top-v,Q()):(g=k.top+a-v-d,O()),t=k.left+q-r):x||n||(v>b+l+h?g=l+A-v+b+h:k.top>v+A?(g=k.top-v,O()):g=A,t=k.left-r);return{top:g,left:t}}}}function na(){var a=null,b=null,d=null;return function(e,h,g){null==e||a==e.top&&b==e.left||(l.css({top:e.top,left:e.left}),a=e.top,b=e.left);h&&(e=c.outerWidth(),h=m.width()||e,l.width(h-J.vertical),x?p.css("width",100*e/(h-J.vertical)+"%"):p.outerWidth(e));g&&k();g=m.scrollLeft();n&&d==g||(l.scrollLeft(g),d=g)}}function ca(){if(m.length){var a=m.width(),
b=m.height(),d=c.height(),e=c.width(),g=a<e?U:0;J.horizontal=a-(b<d?U:0)<e?U:0;J.vertical=b-g<d?U:0}}var fa=ea,c=a(this);if(c.data("floatThead-attached"))return!0;if(!c.is("table"))return window.console.log('jQuery.floatThead must be run on a table element. ex: $("table").floatThead();'),!0;var z=c.find("thead:first"),pa=c.find("tbody:first");if(0==z.length)return window.console.log("jQuery.floatThead must be run on a table that contains a <thead> element"),!0;var G=!1,A,ha,J={vertical:0,horizontal:0},
U=V(),ia=0,m=d.scrollContainer(c)||a([]),n=d.useAbsolutePositioning;null==n&&(n=d.scrollContainer(c).length);var S=c.find("caption"),R=1==S.length;if(R)var T="top"===(S.css("caption-side")||S.attr("align")||"top");var da=a('<fthfoot style="display:table-footer-group;"/>'),x=0<m.length,ba=!1,P=a([]),ra=9>=g&&!x&&n,p=a("<table/>"),Y=a("<colgroup/>"),B=c.find("colgroup:first"),M=!0;0==B.length&&(B=a("<colgroup/>"),M=!1);var W=a('<fthrow style="display:table-row;height:0;"/>'),l=a('<div style="overflow: hidden;"></div>'),
H=a("<thead/>"),L=a('<tr class="size-row"/>'),ga=a([]),N=a([]),ja=a([]),X=a([]);H.append(L);c.prepend(B);u&&(da.append(W),c.append(da));p.append(Y);l.append(p);d.copyTableClass&&p.attr("class",c.attr("class"));p.attr({cellpadding:c.attr("cellpadding"),cellspacing:c.attr("cellspacing"),border:c.attr("border")});p.css({borderCollapse:c.css("borderCollapse"),border:c.css("border")});p.addClass(d.floatTableClass).css("margin",0);if(n){var K=function(a,b){var c=a.css("position");if("relative"!=c&&"absolute"!=
c||b)c={paddingLeft:a.css("paddingLeft"),paddingRight:a.css("paddingRight")},l.css(c),a=a.wrap("<div class='"+d.floatWrapperClass+"' style='position: relative; clear:both;'></div>").parent(),ba=!0;return a};x?(P=K(m,!0),P.append(l)):(P=K(c),c.after(l))}else c.after(l);l.css({position:n?"absolute":"fixed",marginTop:0,top:n?0:"auto",zIndex:d.zIndex});l.addClass(d.floatContainerClass);r();var ka={"table-layout":"fixed"},I={"table-layout":c.css("tableLayout")||"auto"},qa=c[0].style.width||"";ca();var E;
E=Z();E();var t=aa(),C=na();C(t("init"),!0);var sa=b.debounce(function(){C(t("windowScrollDone"),!1)},300),K=function(){C(t("windowScroll"),!1);sa()},oa=function(){C(t("containerScroll"),!1)},F=b.debounce(function(){ca();r();E=Z();E();t=aa();C(t("reflow"),!0)},1);if(x)if(n)m.on(h("scroll"),oa);else m.on(h("scroll"),oa),e.on(h("scroll"),K);else e.on(h("scroll"),K);e.on(h("load"),F);q(d.debounceResizeMs,h("resize"),function(){r();ca();E=Z();E();t=aa();C=na();C(t("resize"),!0,!0)});c.on("reflow",F);
if(y(c))c.on("filter",F).on("sort",F).on("page",F);c.data("floatThead-attached",{destroy:function(){var a=".fth-"+fa;Q();c.css(I);B.remove();u&&da.remove();H.parent().length&&H.replaceWith(z);c.off("reflow");m.off(a);ba&&(m.length?m.unwrap():c.unwrap());n&&c.css("minWidth","");l.remove();c.data("floatThead-attached",!1);e.off(a)},reflow:function(){F()},setHeaderHeight:function(){k()},getFloatContainer:function(){return l},getRowGroups:function(){return G?l.find("thead").add(c.find("tbody,tfoot")):
c.find("thead,tbody,tfoot")}});ea++});return this}})(jQuery);
(function(a){a.floatThead=a.floatThead||{};a.floatThead._=window._||function(){var q={},V=Object.prototype.hasOwnProperty;q.has=function(a,b){return V.call(a,b)};q.keys=function(a){if(a!==Object(a))return window.console.log("Invalid object"),[];var b=[],g;for(g in a)q.has(a,g)&&b.push(g);return b};a.each("Arguments Function String Number Date RegExp".split(" "),function(){var a=this;q["is"+a]=function(b){return Object.prototype.toString.call(b)=="[object "+a+"]"}});q.debounce=function(a,b,g){var u,
r,e,q,h;return function(){e=this;r=arguments;q=new Date;var k=function(){var d=new Date-q;d<b?u=setTimeout(k,b-d):(u=null,g||(h=a.apply(e,r)))},D=g&&!u;u||(u=setTimeout(k,b));D&&(h=a.apply(e,r));return h}};return q}()})(jQuery);
