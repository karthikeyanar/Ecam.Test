chrome.runtime.onInstalled.addListener(function() {
	chrome.storage.sync.set({ color: '#3aa757' },function() {
		console.log('The color is green.');
	});
	chrome.declarativeContent.onPageChanged.removeRules(undefined,function() {
		chrome.declarativeContent.onPageChanged.addRules([{
			conditions: [new chrome.declarativeContent.PageStateMatcher({
				pageUrl: {
					hostPrefix: '',
					hostSuffix: 'kite.zerodha.com',
					schemes: ['https']
				}
			})],
			actions: [new chrome.declarativeContent.ShowPageAction()]
		}]);
	});
});
chrome.tabs.onUpdated.addListener(function(tabId,changeInfo,tab) {
	if(changeInfo.status==="complete") {
		chrome.tabs.executeScript(tabId,{ file: "jquery-3.3.1.min.js" });
		if(tab.url.indexOf('/aa-flight-book')>0) {
			//alert('flight_book');
			chrome.tabs.executeScript(tabId,{ file: "init.js" });
		}
		else if(tab.url.indexOf('/ChromeExtensionLogin')>0) {
			//alert('login');
			chrome.tabs.executeScript(tabId,{ file: "form-fill.js" });
			setTimeout(function() {
				var code="pageLoad('login',"+tab.id+");";
				//alert('1_tabid='+tabId);
				chrome.tabs.executeScript(tabId,{ code: code });
			},1000);
		}
		else if(tab.url.indexOf('/ChromeExtension')>0) {
			//alert('login');
			chrome.tabs.executeScript(tabId,{ file: "form-fill.js" });
			setTimeout(function() {
				var code="pageLoad('form',"+tab.id+");";
				//alert('1_tabid='+tabId);
				chrome.tabs.executeScript(tabId,{ code: code });
			},1000);
		}
		chrome.tabs.executeScript({ code: "console.log('id=','"+tab.id+"','url=','"+tab.url+"','windowId=','"+tab.windowId+"','openerTabId=','"+tab.openerTabId+"');" });
	}
});
var TAB_DATA=[];
function doInCurrentTab(tabCallback) {
	chrome.tabs.query({ currentWindow: true,active: true },
        function(tabArray) {
        	//alert('tabArray[0]='+tabArray[0]);
        	tabCallback(tabArray[0]);
        }
    );
}
function getTabData(tabid) {
	var data=null;
	var i;
	for(i=0;i<TAB_DATA.length;i++) {
		if(TAB_DATA[i].tabid==tabid) {
			data=TAB_DATA[i].data;
		}
	}
	return data;
}
chrome.runtime.onMessage.addListener(function(msg) {
	//alert(msg.cmd);
	if(msg.cmd!==undefined) {
		switch(msg.cmd) {
		    case 'price':

				break;
		}
	}
});
chrome.extension.onConnect.addListener(function (port) {
    console.log("Connected .....");
    port.onMessage.addListener(function (msg) {
        //alert("message recieved " + msg);
        port.postMessage("Hi Popup.js");
    });
});