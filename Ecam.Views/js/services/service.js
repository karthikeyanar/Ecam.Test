"use strict";
define("service",['helper'],function(helper) {
    function Service() {

        var self=this;
      
        this.selectCompanies=function(param,callback) {
            var url=apiUrl("/Company/Select");
            $.ajax({
                "url": url,
                "cache": false,
                "type": "GET",
                "data": param
            }).done(function(json) {
                if(callback)
                    callback(json);
            });
        }
        this.searchCompanies=function(request,response,onBefore,onSuccess) {
            var param=new Array();
            param[param.length]={ "name": "pagesize","value": helper.autoCompletePageSize }
            param[param.length]={ "name": "term","value": request.term }
            if(onBefore) {
                onBefore(param);
            }
            self.selectCompanies(param,function(json) {
                if(onSuccess) {
                    onSuccess(json)
                }
                response(json);
            });
        }
        this.selectShipmentType=function(param,callback) {
            var url=apiUrl("/ShipmentType/Select");
            $.ajax({
                "url": url,
                "cache": false,
                "type": "GET",
                "data": param
            }).done(function(json) {
                if(callback)
                    callback(json);
            });
        }
        this.searchShipmentType=function(request,response,onBefore,onSuccess) {
            var param=new Array();
            param[param.length]={ "name": "pagesize","value": helper.autoCompletePageSize }
            param[param.length]={ "name": "term","value": request.term }
            if(onBefore) {
                onBefore(param);
            }
            var url=apiUrl("/ShipmentType/Select");
            $.ajax({
                "url": url,
                "cache": false,
                "type": "GET",
                "data": param
            }).done(function(json) {
                if(onSuccess) {
                    onSuccess(json)
                }
                response(json);
            });
        }
        this.searchCountry=function(request,response,onBefore,onSuccess) {
            var param=new Array();
            param[param.length]={ "name": "pagesize","value": helper.autoCompletePageSize }
            param[param.length]={ "name": "term","value": request.term }
            if(onBefore) {
                onBefore(param);
            }
            var url=apiUrl("/Country/Select");
            $.ajax({
                "url": url,
                "cache": false,
                "type": "GET",
                "data": param
            }).done(function(json) {
                if(onSuccess) {
                    onSuccess(json)
                }
                response(json);
            });
        }
        this.selectCommodityType=function(param,callback) {
            var url=apiUrl("/CommodityType/Select");
            $.ajax({
                "url": url,
                "cache": false,
                "type": "GET",
                "data": param
            }).done(function(json) {
                if(callback)
                    callback(json);
            });
        }
        this.searchCommodityType=function(request,response,onBefore,onSuccess) {
            var param=new Array();
            param[param.length]={ "name": "pagesize","value": helper.autoCompletePageSize }
            param[param.length]={ "name": "term","value": request.term }
            if(onBefore) {
                onBefore(param);
            }
            var url=apiUrl("/CommodityType/Select");
            $.ajax({
                "url": url,
                "cache": false,
                "type": "GET",
                "data": param
            }).done(function(json) {
                if(onSuccess) {
                    onSuccess(json)
                }
                response(json);
            });
        }

        this.selectCurrency=function(param,callback) {
            var url=apiUrl("/Currency/Select");
            $.ajax({
                "url": url,
                "cache": false,
                "type": "GET",
                "data": param
            }).done(function(json) {
                if(callback)
                    callback(json);
            });
        }
        this.searchCurrency=function(request,response,onBefore,onSuccess) {
            var param=new Array();
            param[param.length]={ "name": "pagesize","value": helper.autoCompletePageSize }
            param[param.length]={ "name": "term","value": request.term }
            if(onBefore) {
                onBefore(param);
            }
            self.selectCurrency(param,function(json) {
                if(onSuccess) {
                    onSuccess(json)
                }
                response(json);
            });
        }

        this.selectSMTPSetting=function(param,callback) {
            var url=apiUrl("/SMTPSetting/Select");
            $.ajax({
                "url": url,
                "cache": false,
                "type": "GET",
                "data": param
            }).done(function(json) {
                if(callback)
                    callback(json);
            });
        }
        this.searchSMTPSetting=function(request,response,onBefore,onSuccess) {
            var param=new Array();
            param[param.length]={ "name": "pagesize","value": helper.autoCompletePageSize }
            param[param.length]={ "name": "term","value": request.term }
            if(onBefore) {
                onBefore(param);
            }
            self.selectSMTPSetting(param,function(json) {
                if(onSuccess) {
                    onSuccess(json)
                }
                response(json);
            });
        }

        this.selectAgent=function(param,callback) {
            var url=apiUrl("/Agent/Select");
            $.ajax({
                "url": url,
                "cache": false,
                "type": "GET",
                "data": param
            }).done(function(json) {
                if(callback)
                    callback(json);
            });
        }
        this.searchAgent=function(request,response,onBefore,onSuccess) {
            var param=new Array();
            param[param.length]={ "name": "pagesize","value": helper.autoCompletePageSize }
            param[param.length]={ "name": "term","value": request.term }
            if(onBefore) {
                onBefore(param);
            }
            self.selectAgent(param,function(json) {
                if(onSuccess) {
                    onSuccess(json)
                }
                response(json);
            });
        }

        this.selectTimeZone=function(param,callback) {
            var url=apiUrl("/TimeZone/Select");
            $.ajax({
                "url": url,
                "cache": false,
                "type": "GET",
                "data": param
            }).done(function(json) {
                if(callback)
                    callback(json);
            });
        }
        this.searchTimeZone=function(request,response,onBefore,onSuccess) {
            var param=new Array();
            param[param.length]={ "name": "pagesize","value": helper.autoCompletePageSize }
            param[param.length]={ "name": "term","value": request.term }
            if(onBefore) {
                onBefore(param);
            }
            self.selectTimeZone(param,function(json) {
                if(onSuccess) {
                    onSuccess(json)
                }
                response(json);
            });
        }
        this.selectAirport=function(param,callback) {
            var url=apiUrl("/Airport/Select?isShowAllAirport=true");
            $.ajax({
                "url": url,
                "cache": false,
                "type": "GET",
                "data": param
            }).done(function(json) {
                if(callback)
                    callback(json);
            });
        }
        this.searchAirport=function(request,response,onBefore,onSuccess) {
            var param=new Array();
            param[param.length]={ "name": "pagesize","value": helper.autoCompletePageSize }
            param[param.length]={ "name": "term","value": request.term }
            if(onBefore) {
                onBefore(param);
            }
            self.selectAirport(param,function(json) {
                if(onSuccess) {
                    onSuccess(json)
                }
                response(json);
            });
        }
        this.selectAirline=function(param,callback) {
            var url=apiUrl("/Airline/Select");
            $.ajax({
                "url": url,
                "cache": false,
                "type": "GET",
                "data": param
            }).done(function(json) {
                if(callback)
                    callback(json);
            });
        }
        this.searchAirline=function(request,response,onBefore,onSuccess) {
            var param=new Array();
            param[param.length]={ "name": "pagesize","value": helper.autoCompletePageSize }
            param[param.length]={ "name": "term","value": request.term }
            if(onBefore) {
                onBefore(param);
            }
            self.selectAirline(param,function(json) {
                if(onSuccess) {
                    onSuccess(json)
                }
                response(json);
            });
        }
        this.searchPLLedger=function(request,response,onBefore,onSuccess) {
            var param=new Array();
            param[param.length]={ "name": "pagesize","value": helper.autoCompletePageSize }
            param[param.length]={ "name": "term","value": request.term }
            if(onBefore) {
                onBefore(param);
            }
            var url=apiUrl("/PLLedger/Select");             
            $.ajax({
                "url": url,
                "cache": false,
                "type": "GET",
                "data": param
            }).done(function(json) {
                if(onSuccess) {
                    onSuccess(json)
                }
                response(json);
            });
        }
        this.searchFlight=function(request,response,onBefore,onSuccess) {
            var param=new Array();
            param[param.length]={ "name": "pagesize","value": helper.autoCompletePageSize }
            param[param.length]={ "name": "term","value": request.term }
            if(onBefore) {
                onBefore(param);
            }
            var url=apiUrl("/Flight/Select");
            $.ajax({
                "url": url,
                "cache": false,
                "type": "GET",
                "data": param
            }).done(function(json) {
                if(onSuccess) {
                    onSuccess(json)
                }
                response(json);
            });
        }
        this.searchFlightType=function(request,response,onBefore,onSuccess) {
            var param=new Array();
            param[param.length]={ "name": "pagesize","value": helper.autoCompletePageSize }
            param[param.length]={ "name": "term","value": request.term }
            if(onBefore) {
                onBefore(param);
            }
            var url=apiUrl("/FlightType/Select");
            $.ajax({
                "url": url,
                "cache": false,
                "type": "GET",
                "data": param
            }).done(function(json) {
                if(onSuccess) {
                    onSuccess(json)
                }
                response(json);
            });
        }
        this.searchGroup=function(request,response,onBefore,onSuccess) {
            var param=new Array();
            param[param.length]={ "name": "pagesize","value": helper.autoCompletePageSize }
            param[param.length]={ "name": "term","value": request.term }
            if(onBefore) {
                onBefore(param);
            }
            var url=apiUrl("/Group/Select");
            $.ajax({
                "url": url,
                "cache": false,
                "type": "GET",
                "data": param
            }).done(function(json) {
                if(onSuccess) {
                    onSuccess(json)
                }
                response(json);
            });
        }

        this.selectGroupMember=function(param,callback) {
            var url=apiUrl("/GroupMember/Select");
            $.ajax({
                "url": url,
                "cache": false,
                "type": "GET",
                "data": param
            }).done(function(json) {
                if(callback)
                    callback(json);
            });
        }
        this.searchGroupMember=function(request,response,onBefore,onSuccess) {
            var param=new Array();
            param[param.length]={ "name": "pagesize","value": helper.autoCompletePageSize }
            param[param.length]={ "name": "term","value": request.term }
            if(onBefore) {
                onBefore(param);
            }
            self.selectGroupMember(param,function(json) {
                if(onSuccess) {
                    onSuccess(json)
                }
                response(json);
            });
        }
    }
    var s=new Service();
    return s;
});

