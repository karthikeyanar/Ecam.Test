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
    } else {
        chrome.runtime.sendMessage({ cmd: _CMD, symbol: _SYMBOL });
    }
}
