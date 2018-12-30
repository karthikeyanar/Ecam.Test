var _CMD = '';
var _SYMBOL = '';
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
        self.index += 1;
        console.log('_SYMBOLS.length=', _SYMBOLS.length, 'self.index=', self.index);
        if (_SYMBOLS.length > self.index) {
            setTimeout(function () {
                var arr = _SYMBOLS[self.index].split('|');
                var companyId = arr[0];
                var investingUrl = arr[1];
                var startDate = arr[2];
                var endDate = arr[3];
                chrome.runtime.sendMessage({ cmd: _CMD, symbol: '', company_id: companyId, url: investingUrl, start_date: startDate, end_date: endDate, index: self.index, total: _SYMBOLS.length });
            }, 500);
        }
    } else if (_CMD == 'screener-history') {
        self.index += 1;
        console.log('_SYMBOLS.length=', _SYMBOLS.length, 'self.index=', self.index);
        if (_SYMBOLS.length > self.index) {
            setTimeout(function () {
                var arr = _SYMBOLS[self.index].split('|');
                var companyId = arr[0];
                var screenerUrl = arr[1];
                chrome.runtime.sendMessage({ cmd: _CMD, symbol: '', company_id: companyId, url: screenerUrl, index: self.index, total: _SYMBOLS.length });
            }, 500);
        }
    } else if (_CMD == 'moneycontrol-history') {
        self.index += 1;
        console.log('_SYMBOLS.length=', _SYMBOLS.length, 'self.index=', self.index);
        if (_SYMBOLS.length > self.index) {
            setTimeout(function () {
                var arr = _SYMBOLS[self.index].split('|');
                var companyId = arr[0];
                var moneycontrolUrl = arr[1];
                chrome.runtime.sendMessage({ cmd: _CMD, symbol: '', company_id: companyId, url: moneycontrolUrl, index: self.index, total: _SYMBOLS.length });
            }, 500);
        }
    } else {
        chrome.runtime.sendMessage({ cmd: _CMD, symbol: _SYMBOL });
    }
}
