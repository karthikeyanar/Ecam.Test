//var TAB_DATA=[];
function doInCurrentTab(tabCallback) {
	chrome.tabs.query({ currentWindow: true,active: true },
        function(tabArray) {
        	//////////alert('tabArray[0]='+tabArray[0]);
        	tabCallback(tabArray[0]);
        }
    );
}
//function getTabData(tabid) {
//	var data=null;
//	var i;
//	for(i=0;i<TAB_DATA.length;i++) {
//		if(TAB_DATA[i].tabid==tabid) {
//			data=TAB_DATA[i];
//		}
//	}
//	return data;
//}
function setLocalStorage(key,value) {
	localStorage[key]=value;
}
function getLocalStorage(key,callback) {
	var retValue='';
	if(typeof (localStorage[key])==='string') {
		retValue=localStorage[key];
	}
	if(callback)
		callback(retValue);
}
function cString(v) {
	if(v==undefined)
		return '';
	if(v==null)
		return '';
	return v;
}
function getHostName(url) {
	var match=url.match(/:\/\/(www[0-9]?\.)?(.[^/:]+)/i);
	if(match!=null&&match.length>2&&typeof match[2]==='string'&&match[2].length>0) {
		return match[2];
	}
	else {
		return null;
	}
}
function getDomain(url) {
	var hostName=getHostName(url);
	var domain=hostName;

	if(hostName!=null) {
		var parts=hostName.split('.').reverse();

		if(parts!=null&&parts.length>1) {
			domain=parts[1]+'.'+parts[0];

			if(hostName.toLowerCase().indexOf('.co.uk')!=-1&&parts.length>2) {
				domain=parts[2]+'.'+domain;
			}
		}
	}

	return domain;
}
chrome.runtime.onInstalled.addListener(function() {
	chrome.storage.sync.set({ color: '#3aa757' },function() {
		console.log('The color is green.');
	});
	chrome.declarativeContent.onPageChanged.removeRules(undefined,function() {
		chrome.declarativeContent.onPageChanged.addRules([{
			conditions: [new chrome.declarativeContent.PageStateMatcher({
				pageUrl: {
					hostPrefix: '',
					hostSuffix: 'v-gsa.com',
					schemes: ['http']
				}
			})],
			actions: [new chrome.declarativeContent.ShowPageAction()]
		}]);
	});
});
chrome.tabs.onUpdated.addListener(function(tabId,changeInfo,tab) {
	//chrome.tabs.executeScript({ code: "console.log('onUpdated.addListener changeInfo.status=',\""+changeInfo.status+"\");" });
	if(changeInfo.status==="complete") {
		var code="try { getLibrary(); } catch(e) { } ";
		chrome.tabs.executeScript(tabId,{ code: code },function(result) {
			//alert('result2='+result);
			//chrome.tabs.executeScript({ code: "console.log('onUpdated.addListener result type=','"+(typeof result)+"');" });
			//chrome.tabs.executeScript({ code: "console.log('onUpdated.addListener result=','"+(result)+"');" });
			var isAlreadyExist=false;
			try {
				if(result.toString()=='[object Object]') {
					isAlreadyExist=true;
				}
			} catch(e) { }
			//chrome.tabs.executeScript({ code: "console.log('onUpdated.addListener isAlreadyExist=','"+isAlreadyExist+"');" });
			if(isAlreadyExist==true) {
				code="pageLoad('"+tab.url+"',"+tab.id+",'"+tab.openerTabId+"');";
				chrome.tabs.executeScript(tabId,{ code: code });
			} else {
				chrome.tabs.executeScript(tabId,{ file: "jquery-3.3.1.min.js" });
				chrome.tabs.executeScript(tabId,{ file: "dateFormat.js" });
				if(tab.url.indexOf('/aa-auto-submit-list')>0) {
					chrome.tabs.executeScript(tabId,{ file: "init.js" });
				}
				chrome.tabs.executeScript(tabId,{ file: "exec.js" },function(result) {
					chrome.tabs.executeScript(tabId,{ file: "LMS.js" },function(result) {
						chrome.tabs.executeScript(tabId,{ file: "ECHAMP.js" },function(result) {
							code="pageLoad('"+tab.url+"',"+tab.id+",'"+tab.openerTabId+"');";
							chrome.tabs.executeScript(tabId,{ code: code });
							//chrome.tabs.executeScript({ code: "console.log('onUpdated.addListener id=','"+tab.id+"','url=','"+tab.url+"','windowId=','"+tab.windowId+"','openerTabId=','"+tab.openerTabId+"');" });
						});
					});
				});
			}
		});
	}
});

chrome.runtime.onMessage.addListener(function(msg) {
	try {
		chrome.tabs.executeScript({ code: "console.log('onMessage.addListener cmd=','"+msg.cmd+"');" });
		if(msg.cmd!==undefined) {
			switch(msg.cmd) {
				case 'create':
					chrome.tabs.executeScript(null,{ file: "ext.js" });
					break;
				case 'execute_code':
					//chrome.tabs.executeScript({ code: "console.log('onMessage.addListener tabid=','"+parseInt(msg.tabid)+"');" });
					if(parseInt(msg.tabid)>0) {
						chrome.tabs.executeScript(parseInt(msg.tabid),{ code: msg.code });
					}
					break;
				case 'update_json':
					setLocalStorage(msg.host_name.toString(),msg.data);
					break;
				case 'open':
					//alert('TAB_DATA is reset');

					doInCurrentTab(function(currentTab) {
						var json=JSON.parse(msg.data);
						chrome.windows.create({ 'url': json.url,'type': json.type,'width': json.width,'height': json.height },function(newWindow) {
							var key=newWindow.tabs[0].id.toString();
							var hostName=getHostName(json.url);
							var domain=getDomain(json.url);
							json.host_name=hostName;
							json.domain=domain;
							json.openerid=currentTab.id;
							msg.data=JSON.stringify(json);
							//chrome.tabs.executeScript({ code: "console.log('open cmd key=','"+key+"');" });
							//chrome.tabs.executeScript({ code: "console.log('open cmd msg.data=','"+msg.data+"');" });
							//TAB_DATA.push({ 'tabid': key,'data': msg.data,'openerid': currentTab.id,'url': json.url });
							var currentHostName=getHostName(json.url);
							setLocalStorage(currentHostName,msg.data);
						});
					});

					break;
				case 'page_load':
					//chrome.tabs.executeScript({ code: "console.log('page_load cmd to page_url='+'"+msg.page_url+"');" });
					//chrome.tabs.executeScript({ code: "console.log('tabid='+'"+msg.tabid+"','openerid='+'"+msg.openerid+"','cmd=','"+msg.cmd+"');" });
					var currentHostName=getHostName(msg.page_url);
					getLocalStorage(currentHostName,function(tabData) {
						//chrome.tabs.executeScript({ code: "console.log('tabData 1=','"+tabData+"');" });
						//if(tabData=='') {
						//	var i;
						//	for(i=0;i<localStorage.length;i++) {
						//		chrome.tabs.executeScript({ code: "console.log('data is null=','"+(localStorage[i]=='')+"');" });
						//		if(cString(localStorage[i])!='') {
						//			var tempdata=JSON.parse(localStorage[i]);
						//			var currentHostName=getHostName(msg.page_url);
						//			chrome.tabs.executeScript({ code: "console.log('currentHostName=','"+currentHostName+"');" });
						//			chrome.tabs.executeScript({ code: "console.log('tempdata.host_name=','"+tempdata.host_name+"');" });
						//			if(currentHostName!=tempdata.host_name) {
						//				tabData='';
						//			}
						//		}
						//	}
						//}
						//chrome.tabs.executeScript({ code: "console.log('tabData 2=','"+tabData+"');" });
						var storage='';
						if(tabData!='') {
							storage=tabData;
						}
						//chrome.tabs.executeScript({ code: "console.log('storage is null=','"+(storage=='')+"');" });
						var data=null;
						try {
							data=JSON.parse(storage);
						} catch(e) {
							chrome.tabs.executeScript({ code: "console.log('page_load convert json ex=','"+e+"');" });
						}
						if(data!=null) {
							var openerId=data.openerid;
							if(data.reservation.reservation_name) {
								//chrome.tabs.executeScript({ code: "console.log('data.reservation.reservation_name=','"+data.reservation.reservation_name+"');" });
								if(data.reservation.reservation_name!=undefined) {
									data.page_url=msg.page_url;
									storage=JSON.stringify(data);
									var code="init('"+msg.tabid+"','"+openerId+"','"+storage+"');";
									chrome.tabs.executeScript(parseInt(msg.tabid),{ code: code });
								}
							}
						}
					});
					break;
			}
		}
	} catch(e) {
		chrome.tabs.executeScript({ code: "console.log('onMessage.addListener ex=','"+e+"');" });
	}
});