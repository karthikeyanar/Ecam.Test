/*
* jQuery UI Autocomplete Auto Select Extension
*
* Copyright 2010, Scott González (http://scottgonzalez.com)
* Dual licensed under the MIT or GPL Version 2 licenses.
*
* http://github.com/scottgonzalez/jquery-ui-extensions
*/
(function($) {
    $.ui.autocomplete.prototype.options.autoSelect=true;
    $(document).on("blur.ui-autocomplete-input",".ui-autocomplete-input",function(event) {
        var $input=$(this);
        //window.console.log('autocomplete autoselect is_changed=',$input.attr("is_changed"));
        if($input.attr("is_auto_select")=="false"||$input.attr("is_changed")!="true")
            return false;

        var inputValue=$input.val();
        var url=$input.data("url");
        var autocomplete=$input.data("ui-autocomplete");
        if(!autocomplete.options.autoSelect||autocomplete.selectedItem) { return; }

        var matcher=new RegExp("^"+$.ui.autocomplete.escapeRegex(inputValue)+"$","i");
        autocomplete.widget().children(".ui-menu-item").each(function() {
            var item=$(this).data("item.autocomplete");
            if(matcher.test(item.label||item.value||item)) {
                autocomplete.selectedItem=item;
                return false;
            }
            if(url.indexOf('/Airline/Select')>-1||url.indexOf('/Company/Select')>-1) {
                if($.trim(inputValue)!="") {
                    var checkValue=inputValue.toLowerCase()+' - ';
                    if(item.label.toLowerCase().startsWith(checkValue)==true) {
                        autocomplete.selectedItem=item;
                        return false;
                    }
                }
            }
            if(url.indexOf('/Airport/Select')>-1) {
                if($.trim(inputValue)!="") {
                    var checkValue=inputValue.toLowerCase()+',';
                    if(item.label.toLowerCase().startsWith(checkValue)==true) {
                        autocomplete.selectedItem=item;
                        return false;
                    }
                }
            }
        });
        //window.console.log('autocomplete autoselect selectedItem=',autocomplete.selectedItem,'is_auto_select=',$input.attr("is_auto_select"));
        if(autocomplete.selectedItem) {
            $input.val(autocomplete.selectedItem.label);
            $input.attr("is_changed","false");
            autocomplete._trigger("select",event,{ item: autocomplete.selectedItem });
        } else {
            $input.attr("is_changed","false");
            autocomplete._trigger("select",event,{ item: { "id": "","label": "","text": "","value": "","other": "","other2": "","id2": "" } });
        }
    });

}(jQuery));

// Highlight the characters
$.extend($.ui.autocomplete.prototype,{
    _renderItem: function(ul,item) {
        var term=this.element.val(),
            html=item.label.replace(term,"<b>$&</b>");
        return $("<li></li>")
            .data("item.autocomplete",item)
            .append($("<a></a>").html(html))
            .appendTo(ul);
    }
});
$.widget("custom.autoCompleteEx",{

    options: {
        appendTo: "body",
        autoFocus: null,
        delay: 300,
        minLength: 0,
        position: {
            my: "left top",
            at: "left bottom",
            collision: "none"
        },
        source: null,
        // callbacks
        change: null,
        close: null,
        focus: null,
        open: null,
        response: null,
        search: null,
        select: null
    },
    _create: function() {
        this.wrapper=$("<div>")
					.addClass("input-icon ui-autocomplete-input-box")
					.insertAfter(this.element);

        this._createAutocomplete();
        this._createShowAllButton();
    },

    _createAutocomplete: function() {

        this.input=$(this.element);

        this.input
		.appendTo(this.wrapper)
		.autocomplete(this.options);

    },

    _createShowAllButton: function() {
        var input=this.input,
					wasOpen=false;

        var $i=$("<i class='fa fa-search' style='z-index:3'></i>");
        $i.insertBefore(this.element)
    }
});