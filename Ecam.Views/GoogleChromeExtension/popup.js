'use strict';
function doInCurrentTab(callback) {
	chrome.tabs.query({ currentWindow: true,active: true },
        function(tabArray) {
        	console.log('tabArray[0]=',tabArray[0]);
        	callback(tabArray[0]);
        }
    );
}
function setupSymbolList() {
    var symbols = $('#symbols').val();
    var rows = symbols.split('|');
    var $tbl = $("#tblSymbolList");
    var $tbody = $("tbody", $tbl);
    $tbody.empty();
    var totalAmount = parseFloat($("#total_amount").val());
    var stopLossPercentage = parseFloat($("#stop_loss_percentage").val());
    var targetPercentage = parseFloat($("#target_percentage").val());
    var trailingStopLossPercentage = parseFloat($("#trailing_stop_loss_percentage").val());
    var totalEquities = parseInt(rows.length);
    var investmentPerQty = parseInt(totalAmount / totalEquities);
    var i;
    for (i = 0; i < rows.length; i++) {
        var arr = rows[i].split(':');
        var symbol = arr[0];
        var action = arr[1];
        var $tr = $("<tr>");
        $tr.append("<td>" + symbol + "</td>");
        $tr.append("<td>" + (action == 'B' ? 'Buy' : 'Sell') + "</td>");
        $tr.append("<td>" + qty + "</td>");
    }
}
doInCurrentTab(function (tab) {
    chrome.tabs.executeScript(tab.id, { file: "jquery-3.3.1.min.js" });
    chrome.tabs.executeScript(tab.id, { file: "form-fill.js" });
});
var $btnSubmit = $('#btnSubmit');
$btnSubmit.unbind('click').click(function () {
    //setupSymbolList();
    var symbols = $('#symbols').val();
    var totalAmount = parseFloat($("#total_amount").val());
    var stopLossPercentage = parseFloat($("#stop_loss_percentage").val());
    var targetPercentage = parseFloat($("#target_percentage").val());
    var trailingStopLossPercentage = parseFloat($("#trailing_stop_loss_percentage").val());
    var code = "start('" + symbols + "'," + totalAmount + "," + stopLossPercentage + "," + targetPercentage + "," + trailingStopLossPercentage + ");";
    doInCurrentTab(function (tab) {
        chrome.tabs.executeScript(tab.id, { code: code });
    });
});
var port = chrome.extension.connect({
    name: "Sample Communication"
});
port.postMessage("Hi BackGround");
port.onMessage.addListener(function (msg) {
    console.log("message recieved 1=" + msg);
});
