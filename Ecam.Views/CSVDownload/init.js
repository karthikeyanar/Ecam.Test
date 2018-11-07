var _CMD = '';
var _SYMBOL = '';
var _COMPANY_ID = '';
var _URL = '';
var _START_DATE = '';
var _END_DATE = '';
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
    //console.log('cmd=', cmd, 'symbol=', symbol);
    _SYMBOL = symbol;
    _SYMBOLS = symbol.split(',');
    _CMD = cmd;
    _COMPANY_ID = $(this).attr('company_id');
    _URL = $(this).attr('url');
    _START_DATE = $(this).attr('start_date');
    _END_DATE = $(this).attr('end_date');
    startMC();
});

function startMC() {
    if (_CMD == 'open-nse') {
        self.index += 1;
        console.log('_SYMBOLS.length=', _SYMBOLS.length, 'self.index=', self.index);
        if (_SYMBOLS.length > self.index) {
            setTimeout(function () {
                chrome.runtime.sendMessage({ cmd: _CMD, symbol: _SYMBOLS[self.index] });
            }, 500);
        }
    } else if (_CMD == 'investing-history') {
        chrome.runtime.sendMessage({ cmd: _CMD, symbol: '', company_id: _COMPANY_ID, url: _URL, start_date: _START_DATE, end_date: _END_DATE });
    } else {
        chrome.runtime.sendMessage({ cmd: _CMD, symbol: _SYMBOL });
    }
}
