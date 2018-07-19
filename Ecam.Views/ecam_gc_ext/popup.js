'use strict';
function doInCurrentTab(callback) {
	chrome.tabs.query({ currentWindow: true,active: true },
        function(tabArray) {
        	console.log('tabArray[0]=',tabArray[0]);
        	callback(tabArray[0]);
        }
    );
}
var $btnSubmit=$('#btnSubmit');
$btnSubmit.unbind('click').click(function() {
});
var $btnGetSymbols=$('#btnGetSymbols');
$btnGetSymbols.unbind('click').click(function() {
	chrome.windows.create({ 'url': 'http://dvl.v-gsa.com/Home/ChromeExtension','type': 'popup','width': 1200,'height': 700 },function(newWindow) {
		alert(newWindow.tabs[0].id);
		setTimeout(function() {
			chrome.tabs.remove(newWindow.tabs[0].id,function() { });
		},10000);
	});
});
//chrome.storage.sync.get('color',function(data) {
//	changeColor.style.backgroundColor=data.color;
//	changeColor.setAttribute('value',data.color);
//});