define("ko-binding",['knockout','helper'],function(ko,helper) {
    ko.bindingHandlers.formValidate={
        init: function(element,valueAccessor,allBindings,viewModel,bindingContext) {
            //var dataFor=ko.contextFor(element);
            //window.log("formValidate=",element);
            $(element).validate({
                ignore: "input[type='text']:hidden"
            });
            //dataFor.$root.sortOfflineUsers();
        },
        update: function(element,valueAccessor,allBindings,viewModel,bindingContext) {
        }
    }
    ko.bindingHandlers.initMenuBar={
        init: function(element,valueAccessor,allBindings,viewModel,bindingContext) {
            //setTimeout(function() {
            //    resizeContentHeight();
            //},500);
        },
        update: function(element,valueAccessor,allBindings,viewModel,bindingContext) {
        }
    }
    ko.bindingHandlers.ajaxComboBox={
        init: function(element,valueAccessor,allBindingsAccessor,viewModel) {
            var obj=valueAccessor(),allBindings=allBindingsAccessor();
            obj.position={
                my: "left top",
                at: "left bottom",
                collision: "flip"
            }
            $(element).ajaxcombobox(obj);
            var checkSelect=function(element) {
                var $uiAjaxCombo=$(element).parents(".ui-ajax-combo:first");
                if($.trim(element.value)=="")
                    $uiAjaxCombo.removeClass("ui-autocomplete-selected")
                else
                    $uiAjaxCombo.addClass("ui-autocomplete-selected")
            }
            $(element).on("autocompletechange",function(event,ui) {
                checkSelect(element);
            });
            $(element).on("autocompleteselect",function(event,ui) {
                checkSelect(element);
            });
        },
        update: function(element,valueAccessor,allBindingsAccessor,viewModel) {
            var obj=valueAccessor(),allBindings=allBindingsAccessor();
            var $uiAjaxCombo=$(element).parents(".ui-ajax-combo:first");
            obj.inputValue=cString(obj.inputValue);
            if($.trim(obj.inputValue)=="")
                $uiAjaxCombo.removeClass("ui-autocomplete-selected")
            else
                $uiAjaxCombo.addClass("ui-autocomplete-selected")
        }
    }
    ko.bindingHandlers.autoComplete={
        init: function(element,valueAccessor,allBindingsAccessor,viewModel) {
            var obj=valueAccessor(),allBindings=allBindingsAccessor();
            obj.position={
                my: "left top",
                at: "left bottom",
                collision: "flip"
            }
            var $txtElement=$(element);
            $txtElement.attr("url",cString(obj.url));
            $txtElement.keypress(function(e) {
                $txtElement.attr("is_key_press","true");
            })
            .change(function(e) {
                $txtElement.attr("is_changed","true");
            }).autoCompleteEx(obj);
        },
        update: function(element,valueAccessor,allBindingsAccessor,viewModel) {
        }
    }

    ko.bindingHandlers.timePicker={
        init: function(element,valueAccessor,allBindingsAccessor,viewModel) {
            var obj=valueAccessor(),allBindings=allBindingsAccessor();
            $(element).timepicker(obj);
            $(element).timepicker().on('changeTime.timepicker',function(e) {
                if(obj.onChange)
                    obj.onChange(e);
            });
        },
        update: function(element,valueAccessor,allBindingsAccessor,viewModel) {
        }
    }
    ko.bindingHandlers.bsPagination={
        init: function(element,valueAccessor,allBindingsAccessor,viewModel,bindingContext) {
            var obj=valueAccessor(),allBindings=allBindingsAccessor();
        },
        update: function(element,valueAccessor,allBindingsAccessor,viewModel,bindingContext) {
            var obj=valueAccessor(),allBindings=allBindingsAccessor();
            $(element).twbsPagination({
                total: obj.total_rows,
                rowsPerPage: obj.rows_per_page,
                startPage: obj.page_index,
                onRender: function(status) {
                    if(obj.onRender)
                        obj.onRender(status);
                },
                onPageClick: function(page) {
                    if(obj.onPageClick)
                        obj.onPageClick(page);
                }
            });
        }
    }

    ko.bindingHandlers.sortingTable={
        init: function(element,valueAccessor,allBindings,viewModel,bindingContext) {
            var options=ko.unwrap(valueAccessor());
            helper.sortingTable(element,options);
        },
        update: function(element,valueAccessor,allBindings,viewModel,bindingContext) {
        }
    }
    ko.bindingHandlers.jConfirm={
        init: function(element,valueAccessor,allBindings,viewModel,bindingContext) {
            var options=ko.unwrap(valueAccessor());
            $(element).unbind('click').click(function() {
                jConfirm(options);
            });
        },
        update: function(element,valueAccessor,allBindings,viewModel,bindingContext) {
        }
    }
    ko.bindingHandlers.popoverConfirm={
        init: function(element,valueAccessor,allBindings,viewModel,bindingContext) {
            var options=ko.unwrap(valueAccessor());
            $(element).confirmation(options).tooltip({
                'container': 'body','delay': {
                    "show": 500,"hide": 0
                }
            });
        },
        update: function(element,valueAccessor,allBindings,viewModel,bindingContext) {
        }
    }
    ko.bindingHandlers.dropZone={
        init: function(element,valueAccessor,allBindings,viewModel,bindingContext) {
            var options=ko.unwrap(valueAccessor());
            $(element).dropzone(options);
        },
        update: function(element,valueAccessor,allBindings,viewModel,bindingContext) {
        }
    }
    ko.bindingHandlers.datePicker={
        init: function(element,valueAccessor,allBindings,viewModel,bindingContext) {
            var options=ko.unwrap(valueAccessor());
            var $element=$(element);
            if(options) {
                if(options.changeDate) {
                    $element.on('keyup', function (e) {
                        if($element.val()=="")
                            options.changeDate(null);
                    }).on('changeDate', function (e) {
                        options.changeDate(e);
                    });
                }
                $element.datepicker(options);
            } else {
                $element.datepicker();
            }
        },
        update: function(element,valueAccessor,allBindings,viewModel,bindingContext) {
        }
    }
    ko.bindingHandlers.multiSelect={
        init: function(element,valueAccessor,allBindingsAccessor,viewModel,bindingContext) {
            var obj=valueAccessor(),allBindings=allBindingsAccessor();
            var arguements_list=ko.utils.unwrapObservable(valueAccessor());
            $(element)
            .multiselect(obj)
            .change(function() {
                if(obj.change)
                    obj.change();
            });
        },
        update: function(element,valueAccessor,allBindingsAccessor,viewModel,bindingContext) {
        }
    }
    ko.bindingHandlers.bsTableEmptyCheck={
        init: function(element,valueAccessor,allBindingsAccessor,viewModel,bindingContext) {
            var obj=valueAccessor(),allBindings=allBindingsAccessor();
            checkTBodyNRF(element);
        },
        update: function(element,valueAccessor,allBindingsAccessor,viewModel,bindingContext) {
            checkTBodyNRF(element);
        }
    }
    ko.bindingHandlers.select2={
        init: function(element,valueAccessor,allBindingsAccessor,viewModel,bindingContext) {
            var obj=valueAccessor(),allBindings=allBindingsAccessor();
            $(element).select2(obj);
        },
        update: function(element,valueAccessor,allBindingsAccessor,viewModel,bindingContext) {
        }
    }
    ko.bindingHandlers.companySelect2={
        init: function(element,valueAccessor,allBindings,viewModel,bindingContext) {
            var options=ko.unwrap(valueAccessor());
            options.url=apiUrl("/Company/Select");
            options.placeholder="Select Companies";
            options.resultsCallBack=function(data,page) {
                var s2data=[];
                $.each(data,function(i,d) {
                    s2data.push({ "id": d.id,"text": d.label,"logo": d.other,"source": d });
                });
                return { results: s2data };
            };
            options.formatResult=showLogo;
            options.formatSelection=showLogo;
            select2Setup(element,options);
        },
        update: function(element,valueAccessor,allBindings,viewModel,bindingContext) {
        }
    }


    ko.bindingHandlers.airlineSelect2={
        init: function(element,valueAccessor,allBindings,viewModel,bindingContext) {
            var options=ko.unwrap(valueAccessor());
            var isShowAllAirline="false";
            try {
                isShowAllAirline=options.isShowAllAirline;
            } catch(e) { }
            options.url=apiUrl("/Airline/Select");
            if(cString(isShowAllAirline)=="true") {
                options.url+="?isShowAllAirline=true";
            }
            options.placeholder="Select Airlines";
            options.resultsCallBack=function(data,page) {
                var s2data=[];
                $.each(data,function(i,d) {
                    s2data.push({ "id": d.id,"text": d.label,"logo": d.other,"source": d });
                });
                return { results: s2data };
            };
            options.formatResult=showLogo;
            options.formatSelection=showLogo;
            select2Setup(element,options);
        },
        update: function(element,valueAccessor,allBindings,viewModel,bindingContext) {
        }
    }

    ko.bindingHandlers.airportSelect2={
        init: function(element,valueAccessor,allBindings,viewModel,bindingContext) {
            var options=ko.unwrap(valueAccessor());
            options.url=apiUrl("/Airport/Select");
            options.placeholder="Select Airports";
            options.resultsCallBack=function(data,page) {
                var s2data=[];
                $.each(data,function(i,d) {
                    s2data.push({ "id": d.id,"text": d.label,"source": d });
                });
                return { results: s2data };
            };
            select2Setup(element,options);
        },
        update: function(element,valueAccessor,allBindings,viewModel,bindingContext) {
        }
    }

    ko.bindingHandlers.agentSelect2={
        init: function(element,valueAccessor,allBindings,viewModel,bindingContext) {
            var options=ko.unwrap(valueAccessor());
            options.url=apiUrl("/Agent/Select");
            options.placeholder="Select Agent";
            options.resultsCallBack=function(data,page) {
                var s2data=[];
                $.each(data,function(i,d) {
                    s2data.push({ "id": d.id,"text": d.label,"source": d });
                });
                return { results: s2data };
            };
            select2Setup(element,options);
        },
        update: function(element,valueAccessor,allBindings,viewModel,bindingContext) {
        }
    }

    ko.bindingHandlers.flightSelect2={
        init: function(element,valueAccessor,allBindings,viewModel,bindingContext) {
            var options=ko.unwrap(valueAccessor());
            options.url=apiUrl("/Flight/Select");
            options.placeholder="Select Flights";
            options.resultsCallBack=function(data,page) {
                var s2data=[];
                $.each(data,function(i,d) {
                    s2data.push({ "id": d.id,"text": d.label,"source": d });
                });
                return { results: s2data };
            };
            select2Setup(element,options);
        },
        update: function(element,valueAccessor,allBindings,viewModel,bindingContext) {
        }
    }

    ko.bindingHandlers.countrySelect2={
        init: function(element,valueAccessor,allBindings,viewModel,bindingContext) {
            var options=ko.unwrap(valueAccessor());
            options.url=apiUrl("/Country/Select");
            options.placeholder="Select Countries";
            options.resultsCallBack=function(data,page) {
                var s2data=[];
                $.each(data,function(i,d) {
                    s2data.push({ "id": d.id,"text": d.label,"source": d });
                });
                return { results: s2data };
            };
            select2Setup(element,options);
        },
        update: function(element,valueAccessor,allBindings,viewModel,bindingContext) {
        }
    }

    ko.bindingHandlers.currencySelect2 = {
        init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
            var options = ko.unwrap(valueAccessor());
            options.url = apiUrl("/Currency/Select");
            options.placeholder = "Select Currencies";
            options.resultsCallBack = function (data, page) {
                var s2data = [];
                $.each(data, function (i, d) {
                    s2data.push({ "id": d.id, "text": d.label, "source": d });
                });
                return { results: s2data };
            };
            select2Setup(element, options);
        },
        update: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
        }
    }

    ko.bindingHandlers.flightTypeSelect2 = {
        init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
            var options = ko.unwrap(valueAccessor());
            options.url = apiUrl("/FlightType/Select");
            options.placeholder = "Select FlightType";
            options.resultsCallBack = function (data, page) {
                var s2data = [];
                $.each(data, function (i, d) {
                    s2data.push({ "id": d.id, "text": d.label, "source": d });
                });
                return { results: s2data };
            };
            select2Setup(element, options);
        },
        update: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
        }
    }

    ko.bindingHandlers.tokenField={
        init: function(element,valueAccessor,allBindings,viewModel,bindingContext) {
            var options=ko.unwrap(valueAccessor());
            var minWidth=cString(options.minWidth);
            if(minWidth=="")
                minWidth='180';
            $(element)
            .on('tokenfield:createtoken',function(e) {
            })
            .on('tokenfield:createdtoken',function(e) {
            })
            .on('tokenfield:edittoken',function(e) {
            })
            .on('tokenfield:removedtoken',function(e) {
            })
            .tokenfield({ "minWidth": 180 });
        },
        update: function(element,valueAccessor,allBindings,viewModel,bindingContext) {
        }
    }

    ko.observableArray.fn.prepend=function(valuesToPush) {
        var underlyingArray=this();
        this.valueWillMutate();

        for(var i=0,j=valuesToPush.length;i<j;i++) {
            underlyingArray.splice(i,0,valuesToPush[i]);
        }

        this.valueHasMutated();

        return this;
    };
    ko.observableArray.fn.prependSingle=function(valueToPush) {
        var underlyingArray=this();
        this.valueWillMutate();
        underlyingArray.splice(0,0,valueToPush);
        this.valueHasMutated();
        return this;
    };

});