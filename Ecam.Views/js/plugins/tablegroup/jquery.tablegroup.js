(function($) {
    $.addGroup=function(t,p) {
        if(t.grid) { return false; }
        p=$.extend({
            onComplete: null
        },p);
        var g={
            gDiv: null,
            groToggler: null,
            init: function() {
                g.gDiv=$("<div class='gro-gdiv'></div>");
                g.groToggler=$("<div class='gro-toggler'><a href='javascript:;' data-toggle='dropdown' role='button'><i class='fa fa-caret-down'></i></a></div>");
                var $ul=$("<ul class='dropdown-menu dropdown-menu-right' role='menu' aria-labelledby='dLabel'></ul>");
                g.groToggler.append($ul);
                $ul.append("<li role='presentation' class='dropdown-header'>Group By</li>");

                var $liclear=$("<li class='gro-clear' role='presentation'></li>");
                var $aclear=$("<a href='javascript:;'>None</a>");

                $liclear.append($aclear);
                $ul.append($liclear);




                $(g.gDiv).append(g.groToggler);
                $(t).before(g.gDiv);
                g.gDiv.append(t);
                var $thead=$("thead",t);
                var $tbody=$("tbody",t);
                $tbody.hover(function() {
                    g.groToggler.removeClass("open").hide();
                });
                g.gDiv.hover(function() { },function() {
                    g.groToggler.removeClass("open").hide();
                });

                $aclear.addClass("select");

                $aclear.unbind("click").click(function() {
                    $("tr > th",$thead).show();
                    $(".grow",$tbody).remove();
                    $("li > a",$ul).removeClass("select");
                    $aclear.addClass("select");
                    $("tr",$tbody).show();
                });
                $("tr > th",t).each(function(i) {
                    var $th=$(this);
                    $th.attr("ci",i);
                    if($th.attr("sum")!="true") {
                        var $li=$("<li role='presentation'></li>");
                        var $a=$("<a href='javascript:;'><i class='fa fa-check'></i>"+$th.attr("displayname")+"</a>");
                        $li.append($a)
                        $ul.append($li);
                        $a.unbind("click").click(function() {

                            g.gDiv.addClass("gro-applied");

                            $("li > a",$ul).removeClass("select");
                            $a.addClass("select");

                            $(t).addClass("apply-group");

                            $(".grow",$tbody).remove();
                            $("tr",$tbody).hide();
                            $("tr > th",t).each(function() {
                                var $checkth=$(this);
                                var checkdname=$checkth.attr("displayname");
                                if(checkdname!=$th.attr("displayname")&&$checkth.attr("sum")!="true") {
                                    $checkth.hide();
                                } else {
                                    $checkth.show();
                                }
                            });
                            var temparr=[];
                            $("tr > th:not([sum='true']):visible",t).each(function() {
                                var $th=$(this);
                                var ci=$th.attr("ci");
                                $("tr:not(.grow)",$tbody).each(function() {
                                    var row={ "key": "","index": ci,sums: [] };
                                    var rowkey=$("td:eq("+ci+")",this).html();
                                    row.key=rowkey;
                                    var isExist=false;
                                    $(temparr).each(function(i,exist) {
                                        if(row.key==exist.key) {
                                            isExist=true;
                                        }
                                    });
                                    if(isExist==false) {
                                        temparr.push(row);
                                        $("tr:not(.grow)",$tbody).each(function() {
                                            var $sumrow=$(this);
                                            if($("td:eq("+ci+")",this).html()==row.key) {
                                                $("tr > th[sum='true']:visible",t).each(function() {
                                                    var sumcolindex=$(this).attr("ci");
                                                    var value=cFloat($("td:eq("+sumcolindex+")",$sumrow).html());
                                                    var sumrow=null;
                                                    $.each(row.sums,function(i,su) {
                                                        if(su.index==sumcolindex) {
                                                            sumrow=su;
                                                        }
                                                    });
                                                    if(sumrow==null) {
                                                        sumrow={ "index": sumcolindex,"value": value };
                                                        row.sums.push(sumrow);
                                                    } else {
                                                        sumrow.value+=cFloat(value);
                                                    }
                                                });
                                            }
                                        });
                                    }
                                });
                                var grows=[];
                                $.each(temparr,function(i,arr) {
                                    var indexarr=[];
                                    var rowarr=[];
                                    indexarr.push(cInt(arr.index));
                                    $.each(arr.sums,function(i,su) {
                                        indexarr.push(cInt(su.index));
                                    });
                                    var sortarr=indexarr.sort(function(a,b) {
                                        return (a-b);
                                    });
                                    $.each(sortarr,function(i,ind) {
                                        if(cInt(arr.index)==ind) {
                                            rowarr.push({ "value": arr.key,"iskeycol": "true" });
                                        } else {
                                            $.each(arr.sums,function(i,su) {
                                                if(cInt(su.index)==ind) {
                                                    rowarr.push({ "value": su.value,"iskeycol": "false" });
                                                }
                                            });
                                        }
                                    });
                                    grows.push(rowarr);
                                });
                                $.each(grows,function(i,rows) {
                                    var $tr=$("<tr></tr>");
                                    $.each(rows,function(i,r) {
                                        $tr.append("<td class='"+(r.iskeycol=="true"?"gro-key-col":"gro-sum-col")+"'><div>"+r.value+"</div></td>");
                                    });
                                    $tr.addClass("grow");
                                    $tbody.append($tr);
                                });
                            });
                            g.groToggler.removeClass("open").hide();
                            if(p.onComplete)
                                p.onComplete();
                        });
                    }
                    $(this).hover(function() {
                        //g.groToggler.removeClass("open").hide();
                        var offset=$(this).position();
                        var left=offset.left+$(this).outerWidth(true)-$(g.groToggler).outerWidth(true);
                        $ul.removeClass("dropdown-menu-right").removeClass("dropdown-menu-left");
                        if(g.gDiv.offset().left<left) {
                            $ul.addClass("dropdown-menu-right");
                        } else {
                            $ul.addClass("dropdown-menu-left");
                        }
                        g.groToggler.css({
                            "top": 0, //offset.top-1,
                            "left": left,
                            "display": "block"
                        });
                    },function() {
                        // g.groToggler.hide();
                    });
                });
            }
        }
        g.init();
        t.grid=g;
        return t;
    }

    var docloaded=false;
    $(document).ready(function() { docloaded=true });
    $.fn.tableGroup=function(p) {
        return this.each(function() {
            var t=this;
            $.addGroup(t,p);
        });
    };
})(jQuery);