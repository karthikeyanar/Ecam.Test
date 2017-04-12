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
$.widget("custom.ajaxcombobox",{

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
					.addClass("input-group ui-ajax-combo")
					.insertAfter(this.element);
        var input=this.element;
        if($(this.element).hasClass("ui-readonly")) {
            this.wrapper.addClass("ui-readonly");
            var $uiReadonlyLabel=$("<div class='ui-readonly-label'></div>");
            this.wrapper.append($uiReadonlyLabel);
            var wasOpen=false;
            $uiReadonlyLabel
            .mousedown(function() {
                wasOpen=input.autocomplete("widget").is(":visible");
            })
		    .click(function() {
		        input.focus();
		        // Close if already visible
		        if(wasOpen) {
		            return;
		        }
		        input.autocomplete("option",{ minLength: 0 });
		        input.autocomplete("search","");
		    });
        }

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

        var wrapper=this.wrapper;
        var isNoClearBtn=$(input).attr("is_no_clear_btn");
        if(isNoClearBtn!="true") {
            var $spanUIACBtn=$("<div class='ui-ac-clear-btn' title='Clear'><span class='img-trigger'></span></div>");
            $spanUIACBtn.appendTo(this.wrapper);
            $spanUIACBtn.tooltip({
                'container': 'body','delay': {
                    "show": 0,"hide": 0
                }
            }).click(function(event) {
                var $input=$(input);
                $input.val("");
                var autocomplete=$input.data("ui-autocomplete");
                autocomplete._trigger("select",event,{ item: { "id": "","label": "","text": "","value": "","other": "","other2": "","id2": "" } });
            });
        }

        var isNoShowAllBtn=$(input).attr("is_no_show_all_btn");

        if(isNoShowAllBtn!="true") {
            var $span=$("<span class='input-group-btn'></span>");
            $span.appendTo(this.wrapper);

            var $btn=$("<a class='btn btn-default input-sm'></a>");

            $btn
            .html("<span class='caret'></span>")
            .attr("tabIndex",-1)
            .attr("title","Show All Items")
            .tooltip({
                'container': 'body','delay': { "show": 500,"hide": 0 }
            })
            .appendTo($span)
            .button({
                icons: {
                    primary: "ui-icon-triangle-1-s"
                },
                text: false
            })
            .removeClass("ui-corner-all")
            .addClass("custom-combobox-toggle ui-corner-right")
            .mousedown(function() {
                wasOpen=input.autocomplete("widget").is(":visible");
            })
            .click(function() {
                input.focus();
                // Close if already visible
                if(wasOpen) {
                    return;
                }
                /*
                if($.trim(input.val())!="") {
                    var autocomplete=$(input).data("ui-autocomplete");
                    autocomplete._trigger('select','autocompleteselect',{ item: { id: "",value: "",label: "" } });
                    $(input).val("");
                    $(wrapper).removeClass("ui-autocomplete-selected");
                }
                // Pass empty string as value to search for, displaying all results
                */
                input.autocomplete("option",{ minLength: 0 });
                input.autocomplete("search","");
            });
        } else {
            $(this.wrapper).addClass("is_no_show_all_btn");
        }
    }
});