// Copyright (c) 2012 The Chromium Authors. All rights reserved.
// Use of this source code is governed by a BSD-style license that can be
// found in the LICENSE file.

//chrome.tabs.onUpdated.addListener(function(tabId, changeInfo, tab) {
//  chrome.tabs.executeScript({ code: "console.log('onUpdated.addListener changeInfo.status=',\"" + changeInfo.status + "\");" });
//if (changeInfo.status === "complete") {
//alert(tabId);

//}
//});

function getHostName(url) {
    var match = url.match(/:\/\/(www[0-9]?\.)?(.[^/:]+)/i);
    if (match != null && match.length > 2 && typeof match[2] === 'string' && match[2].length > 0) {
        return match[2];
    }
    else {
        return null;
    }
}
function getDomain(url) {
    var hostName = getHostName(url);
    var domain = hostName;

    if (hostName != null) {
        var parts = hostName.split('.').reverse();

        if (parts != null && parts.length > 1) {
            domain = parts[1] + '.' + parts[0];

            if (hostName.toLowerCase().indexOf('.co.uk') != -1 && parts.length > 2) {
                domain = parts[2] + '.' + domain;
            }
        }
    }

    return domain;
}
function doInCurrentTab(tabCallback) {
    chrome.tabs.query({ currentWindow: true, active: true },
        function (tabArray) {
            //////////alert('tabArray[0]='+tabArray[0]);
            tabCallback(tabArray[0]);
        }
    );
}
function setLocalStorage(key, value) {
    localStorage[key] = value;
}
function getLocalStorage(key, callback) {
    var retValue = '';
    if (typeof (localStorage[key]) === 'string') {
        retValue = localStorage[key];
    }
    if (callback)
        callback(retValue);
}
// Called when the user clicks on the browser action.
chrome.browserAction.onClicked.addListener(function (tab) {
    chrome.tabs.executeScript(tab.id, { file: "jquery-3.3.1.min.js" }, function (result) {
        chrome.tabs.executeScript(tab.id, { file: "export.js" }, function (result) {
            var code = "exportTable();"
            chrome.tabs.executeScript(tab.id, { code: code });
        });
    });
    /*
    var xhr = new XMLHttpRequest;
    xhr.open("GET", chrome.runtime.getURL("myfile.json"));
    xhr.onreadystatechange = function() {
        if (this.readyState == 4) {
            alert("request finished, now parsing");
            window.json_text = xhr.responseText;
            window.parsed_json = JSON.parse(xhr.responseText);
            alert("xhr.responseText=" + xhr.responseText);
            alert(window.parsed_json.total);
        }
    };
    xhr.send();
	*/
    var action_url = "javascript:window.print();";
    //chrome.tabs.update(tab.id, { url: action_url });
});
chrome.tabs.onUpdated.addListener(function (tabId, changeInfo, tab) {
    chrome.tabs.executeScript({ code: "console.log('changeInfo.status=','" + changeInfo.status + "');" });
    if (changeInfo.status === "complete") {
        chrome.tabs.executeScript(tabId, { file: "jquery-3.3.1.min.js" }, function (result1) {
            //alert('result1='+result1);
            //chrome.tabs.executeScript(tabId, { file: "bililiteRange.js" }, function (result2) {
            //alert('result2='+result2);
            //chrome.tabs.executeScript(tabId, { file: "jquery.sendkeys.js" }, function (result3) {
            //alert('result3='+result3);

            if (tab.url.indexOf('#/company') > 0
                || tab.url.indexOf('#/quater') > 0
                || tab.url.indexOf('#/indicator') > 0
                || tab.url.indexOf('#/cm-company-update') > 0
                || tab.url.indexOf('/Company') > 0) {
                chrome.tabs.executeScript(tabId, { file: "init.js" });
            } else {
                if (tab.url.indexOf('nseindia.com') > 0) {
                    chrome.tabs.executeScript(tabId, { file: "export.js" }, function (result) {
                        var arr = tab.url.split('?');
                        var openerId = arr[1].split('=')[1].toString();
                        var hostname = getHostName(tab.url);
                        var symbol = '';
                        getLocalStorage(hostname, function (data) {
                            symbol = data;
                        });
                        code = "pageLoad('" + tab.url + "','" + tabId + "','" + openerId + "','" + symbol + "');";
                        //alert(code);
                        chrome.tabs.executeScript(tabId, { code: code });
                        //chrome.tabs.executeScript({ code: "console.log('onUpdated.addListener id=','"+tab.id+"','url=','"+tab.url+"','windowId=','"+tab.windowId+"','openerTabId=','"+tab.openerTabId+"');" });
                    });
                }
            }
            chrome.tabs.executeScript({ code: "console.log('id=','" + tab.id + "','url=','" + tab.url + "','windowId=','" + tab.windowId + "','openerTabId=','" + tab.openerTabId + "');" });

            //   });
            // });
        });
    }
});
chrome.runtime.onMessage.addListener(function (msg) {
    try {
        chrome.tabs.executeScript({ code: "console.log('msg.cmd=','" + msg.cmd + "','msg.tabid=','" + msg.tabid + "');" });
        if (msg.cmd !== undefined) {
            switch (msg.cmd) {
                case 'mc-quaterly':
                    var symbol = msg.symbol;
                    var url = 'https://www.moneycontrol.com/stocks/company_info/print_financials.php?sc_did=' + symbol + '&type=quarterly&t=' + (new Date()).getTime();
                    var type = 'popup';
                    var width = 1200;
                    var height = 800;
                    doInCurrentTab(function (currentTab) {
                        chrome.windows.create({ 'url': url, 'type': type, 'width': width, 'height': height }, function (newWindow) {
                            var tab = newWindow.tabs[0];
                            chrome.tabs.executeScript(tab.id, { file: "jquery-3.3.1.min.js" }, function (result) {
                                chrome.tabs.executeScript(tab.id, { file: "export.js" }, function (result) {
                                    var code = "exportTable(" + tab.id + "," + currentTab.id + ");"
                                    chrome.tabs.executeScript(tab.id, { code: code });
                                });
                            });
                        });
                    });
                    break;
                case 'investing-company':
                    var symbol = msg.symbol;
                    var url = 'https://in.investing.com/indices/s-p-cnx-500-components';
                    var type = 'popup';
                    var width = 1200;
                    var height = 800;
                    doInCurrentTab(function (currentTab) {
                        chrome.windows.create({ 'url': url, 'type': type, 'width': width, 'height': height }, function (newWindow) {
                            var tab = newWindow.tabs[0];
                            chrome.tabs.executeScript(tab.id, { file: "jquery-3.3.1.min.js" }, function (result) {
                                chrome.tabs.executeScript(tab.id, { file: "export.js" }, function (result) {
                                    var code = "investingNifty500(" + tab.id + "," + currentTab.id + ");"
                                    chrome.tabs.executeScript(tab.id, { code: code });
                                });
                            });
                        });
                    });
                    break;
                case 'investing-history':
                    var symbol = msg.symbol;
                    var url = 'https://in.investing.com' + msg.url + '-historical-data';
                    var type = '';
                    var width = 20000;
                    var height = 1000;
                    doInCurrentTab(function (currentTab) {
                        chrome.windows.create({ 'url': url }, function (newWindow) {
                            var tab = newWindow.tabs[0];
                            var tabId = tab.id;
                            chrome.tabs.executeScript(tabId, { file: "jquery-3.3.1.min.js" }, function (result1) {
                                //alert('result1='+result1);
                                chrome.tabs.executeScript(tabId, { file: "bililiteRange.js" }, function (result2) {
                                    //alert('result2='+result2);
                                    chrome.tabs.executeScript(tabId, { file: "jquery.sendkeys.js" }, function (result3) {
                                        //alert('result3='+result3);
                                        chrome.tabs.executeScript(tab.id, { file: "export.js" }, function (result) {
                                            var code = "investingHistory(" + tab.id + "," + currentTab.id + "," + msg.company_id + ",'" + msg.start_date + "','" + msg.end_date + "'," + msg.index + "," + msg.total + ");"
                                            chrome.tabs.executeScript(tab.id, { code: code });
                                        });
                                    });
                                });
                            });
                        });
                    });
                    break;
                case 'investing_close_tab':
                    //alert(msg.tabid);
                    var code = "startMC();"
                    chrome.tabs.executeScript(parseInt(msg.openerid), { code: code }, function () {
                        chrome.tabs.remove(parseInt(msg.tabid), function () { });
                    });
                    break;
                case 'screener-history':
                    var symbol = msg.symbol;
                    var url = 'https://www.screener.in/' + msg.url;
                    var type = '';
                    var width = 20000;
                    var height = 1000;
                    doInCurrentTab(function (currentTab) {
                        chrome.windows.create({ 'url': url }, function (newWindow) {
                            var tab = newWindow.tabs[0];
                            var tabId = tab.id;
                            chrome.tabs.executeScript(tabId, { file: "jquery-3.3.1.min.js" }, function (result1) {
                                //alert('result1='+result1);
                                chrome.tabs.executeScript(tabId, { file: "bililiteRange.js" }, function (result2) {
                                    //alert('result2='+result2);
                                    chrome.tabs.executeScript(tabId, { file: "jquery.sendkeys.js" }, function (result3) {
                                        //alert('result3='+result3);
                                        chrome.tabs.executeScript(tab.id, { file: "export.js" }, function (result) {
                                            var code = "screenerHistory(" + tab.id + "," + currentTab.id + "," + msg.company_id + "," + msg.index + "," + msg.total + ");"
                                            chrome.tabs.executeScript(tab.id, { code: code });
                                        });
                                    });
                                });
                            });
                        });
                    });
                    break;
                case 'screener_close_tab':
                    //alert(msg.tabid);
                    var code = "startMC();"
                    chrome.tabs.executeScript(parseInt(msg.openerid), { code: code }, function () {
                        chrome.tabs.remove(parseInt(msg.tabid), function () { });
                    });
                    break;
                case 'close_tab':
                    //alert(msg.tabid);
                    var code = "startMC();"
                    chrome.tabs.executeScript(parseInt(msg.openerid), { code: code }, function () {
                        chrome.tabs.remove(parseInt(msg.tabid), function () { });
                    });
                    break;
                case 'mc-quaterly-downloaded':
                    //alert(msg.tabid);
                    var code = "startMC();"
                    chrome.tabs.executeScript(parseInt(msg.tabid), { code: code });
                    break;
                case 'execute_code':
                    //chrome.tabs.executeScript({ code: "console.log('onMessage.addListener tabid=','"+parseInt(msg.tabid)+"');" });
                    if (parseInt(msg.tabid) > 0) {
                        chrome.tabs.executeScript(parseInt(msg.tabid), { code: msg.code });
                    }
                    break;
                case 'open_nse':
                    var type = 'normal';
                    var width = 1200;
                    var height = 800;
                    doInCurrentTab(function (currentTab) {
                        var url = 'https://www.nseindia.com/products/content/equities/equities/eq_security.htm?openerTabId=' + currentTab.id;
                        chrome.windows.create({ 'url': url, 'type': type, 'width': width, 'height': height }, function (newWindow) {
                            var hostName = getHostName(url);
                            setLocalStorage(hostName, msg.symbol);
                            chrome.tabs.executeScript({ code: "console.log('setSymbol=','" + msg.symbol + "');" });
                            //var tab = newWindow.tabs[0];
                            ////alert(tab.id);
                            //        var code = "nseStart(" + tab.id + "," + currentTab.id + ");"
                            //        //alert(code);
                            //        chrome.tabs.executeScript(tab.id, { code: code });
                        });
                    });
                    break;
                case 'page_load':
                    break;
            }
        }
    } catch (e) {
        chrome.tabs.executeScript({ code: "console.log('onMessage.addListener ex=','" + e + "');" });
    }
});