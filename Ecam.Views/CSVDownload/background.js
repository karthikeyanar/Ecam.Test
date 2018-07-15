// Copyright (c) 2012 The Chromium Authors. All rights reserved.
// Use of this source code is governed by a BSD-style license that can be
// found in the LICENSE file.

//chrome.tabs.onUpdated.addListener(function(tabId, changeInfo, tab) {
//  chrome.tabs.executeScript({ code: "console.log('onUpdated.addListener changeInfo.status=',\"" + changeInfo.status + "\");" });
//if (changeInfo.status === "complete") {
//alert(tabId);

//}
//});

// Called when the user clicks on the browser action.
chrome.browserAction.onClicked.addListener(function(tab) {
    chrome.tabs.executeScript(tab.id, { file: "jquery-3.3.1.min.js" }, function(result) {
        chrome.tabs.executeScript(tab.id, { file: "export.js" }, function(result) {
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