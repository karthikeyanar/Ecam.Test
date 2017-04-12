(function($) {
    'use strict';
    var BSTable=function(ele,options) {
        this.element=ele;
        this.$element=$(ele);
        this.options=$.extend({},BSTable.DEFAULTS,options);
        this.applySorting();
        if(this.options.autoload==true)
            this.refresh();
    }

    BSTable.prototype.applySorting=function() {
        var self=this;
        var options=self.options;
        var element=self.element;
        var $element=$(element);
        var $thead=$("thead",element);
        $thead.find("th")
        .removeClass("sort").removeClass("asc").removeClass("desc")
        .each(function() {
            var $this=$(this);
            var displayName=$this.attr("displayname");
            if(displayName==undefined) {
                displayName=$this.html();
            }
            $this.empty();
            var $div=$("<div></div>").html(displayName);
            var width=$this.attr("display-width");
            if(width!=undefined) {
                $div.width(width);
            }
            $this.empty().append($div);
            if($element.attr("sortname")==$this.attr("sortname")) {
                $this.addClass("sort").addClass($element.attr("sortorder"));
            }
            if($this.attr("sortname")==""||$this.attr("sortname")==undefined) return;
            $this.unbind("click").click(function() {
                var sortorder=$element.attr("sortorder");
                if(sortorder=="asc"||sortorder=="") { sortorder="desc"; } else { sortorder="asc"; }
                $element.attr("sortorder",sortorder);
                $this.removeClass("sort").removeClass("asc").removeClass("desc").siblings().removeClass("sort").removeClass("asc").removeClass("desc");
                $this.addClass("sort").addClass(sortorder);
                if(options.onSorting) {
                    options.onSorting($this.attr("sortname"),$element.attr("sortorder"));
                }
            });
        });
    }


    BSTable.DEFAULTS={
        onSort: null,
        onBefore: null,
        onDone: null,
        onFail: null,
        onAlways: null,
        sortName: '',
        sortOrder: '',
        pageIndex: 0,
        pageSize: 0,
        params: [],
        url: '',
        type: 'GET',
        dataType: 'JSON',
        cache: false,
        autoload: true
    }

    BSTable.prototype.sort=function(sortname,sortorder) {
        this.options.sortName=sortname;
        this.options.sortOrder=sortorder;
        this.options.pageIndex=1;
        this.refresh();
    }

    BSTable.prototype.goTo=function(pageIndex) {
        this.options.pageIndex=pageIndex;
        this.refresh();
    }

    BSTable.prototype.resetPageIndex=function() {
        this.options.pageIndex=1;
    }

    BSTable.prototype.getOptions=function() {
        return this.options;
    }

    BSTable.prototype.refresh=function() {
        var self=this;
        self.options.params=[];
        if(self.options.onBefore)
            self.options.params=self.options.onBefore();
        var url="";
        var pageIndex=0;
        var pageSize=0;
        var sortName="";
        var sortOrder="";
        if($.type(self.options.url)=="function")
            url=self.options.url();
        else
            url=self.options.url;


        if($.type(self.options.pageIndex)=="function")
            pageIndex=self.options.pageIndex();
        else
            pageIndex=self.options.pageIndex;

        if($.type(self.options.pageSize)=="function")
            pageSize=self.options.pageSize();
        else
            pageSize=self.options.pageSize;

        if($.type(self.options.sortName)=="sortName")
            sortName=self.options.sortName();
        else
            sortName=self.options.sortName;

        if($.type(self.options.sortOrder)=="sortOrder")
            sortOrder=self.options.sortOrder();
        else
            sortOrder=self.options.sortOrder;

        var params=self.options.params;
        params[params.length]={ "name": "pageindex","value": pageIndex }
        params[params.length]={ "name": "pagesize","value": pageSize }
        params[params.length]={ "name": "sortname","value": sortName }
        params[params.length]={ "name": "sortorder","value": sortOrder }

        self.options.params=params;
        $.ajax({
            "url": url,
            "cache": self.options.cache,
            "type": self.options.type,
            "dataType": self.options.dataType,
            "data": params
        }).done(function(data) {
            if(self.options.onDone)
                self.options.onDone(data);
        }).fail(function() {
            if(self.options.onFail)
                self.options.onFail(data);
        }).always(function(data) {
            if(self.options.onAlways)
                self.options.onAlways(data);
        });
    }


    $.fn.bsTable=function(options,values) {
        return this.each(function() {
            var $this=$(this)
            var data=$this.data('bsTable')
            if(!data) $this.data('bsTable',(data=new BSTable(this,options)))

            if(!data) return;
            if(options=="sort")
                data.sort(values.sortName,values.sortOrder);

            if(options=="goto")
                data.goTo(values.pageIndex);

            if(options=="refresh")
                data.refresh();

            if(options=="resetPageIndex")
                data.resetPageIndex();

        });
    };
})(jQuery);