// Copyright (c) 2012 The Chromium Authors. All rights reserved.
// Use of this source code is governed by a BSD-style license that can be
// found in the LICENSE file.

//chrome.tabs.onUpdated.addListener(function(tabId, changeInfo, tab) {
//  chrome.tabs.executeScript({ code: "console.log('onUpdated.addListener changeInfo.status=',\"" + changeInfo.status + "\");" });
//if (changeInfo.status === "complete") {
//alert(tabId);

//}
//});
function doInCurrentTab(tabCallback) {
    chrome.tabs.query({ currentWindow: true, active: true },
        function (tabArray) {
            //////////alert('tabArray[0]='+tabArray[0]);
            tabCallback(tabArray[0]);
        }
    );
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
    if (changeInfo.status === "complete") {
        chrome.tabs.executeScript(tabId, { file: "jquery-3.3.1.min.js" });
        if (tab.url.indexOf('#/company') > 0 || tab.url.indexOf('#/quater') > 0) {
            chrome.tabs.executeScript(tabId, { file: "init.js" });
        }
        chrome.tabs.executeScript({ code: "console.log('id=','" + tab.id + "','url=','" + tab.url + "','windowId=','" + tab.windowId + "','openerTabId=','" + tab.openerTabId + "');" });
    }
});
chrome.runtime.onMessage.addListener(function (msg) {
    //alert(msg.cmd);
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
        }
    }
});