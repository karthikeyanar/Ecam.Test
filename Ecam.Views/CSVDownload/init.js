var _CMD = '';
var _SYMBOLS = [];
var index = -1;

var $gcb_cmd = $('#gcb_cmd');
if (!$gcb_cmd[0]) {
    $gcb_cmd = $("<button id='gcb_cmd' name='gcb_cmd' class='hide'>Google Crome Ext Init</button>");
    $("body").append($gcb_cmd);
}
$gcb_cmd.unbind('click').click(function () {
    var cmd = $(this).attr('cmd');
    var symbol = $(this).attr('symbol');
    console.log('cmd=', cmd, 'symbol=', symbol);
    _SYMBOLS = symbol.split(',');
    _CMD = cmd;
    startMC();
});


var $gcb_nse_cmd = $('#gcb_nse_cmd');
if (!$gcb_nse_cmd[0]) {
    $gcb_nse_cmd = $("<button id='gcb_nse_cmd' name='gcb_nse_cmd' class='hide1'>NSE Init</button>");
    $("body").append($gcb_nse_cmd);
}
$gcb_nse_cmd.unbind('click').click(function () {
    var cmd = $(this).attr('cmd');
    var symbol = $(this).attr('symbol');
    var start_date = $(this).attr('start_date');
    var end_date = $(this).attr('end_date');
    var tabid = $(this).attr('tabid');

    console.log('cmd=', cmd, 'tabid=', tabid);
    chrome.runtime.sendMessage({ cmd: cmd, symbol: symbol, start_date: start_date, end_date: end_date, tabid: tabid });
});

function startMC() {
    self.index += 1;
    console.log('_SYMBOLS.length=', _SYMBOLS.length, 'self.index=', self.index);
    if (_SYMBOLS.length > self.index) {
        setTimeout(function () {
            chrome.runtime.sendMessage({ cmd: _CMD, symbol: _SYMBOLS[self.index] });
        }, 500);
    }
}
