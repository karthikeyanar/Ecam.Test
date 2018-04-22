var $container = null;
var $right_container = null;
function start(symbols, totalAmount, stopLoss, target, trailingStopLoss) {
    var id = 'gc_setup_symbol_cnt';
    $("#" + id).remove();
    $container = $("<div id='" + id + "' style='position:absolute;left:0;top:0;height:500px;width:235px;background:#fff;color:#333;z-index:99999;padding:10px;'></div>");
    var html = '';
    html += "<table id='tblSymbolList' cellpadding='0' cellspacing='0' border='1'>";
    html += "<thead>";
    html += "<tr>";
    html += "<th style='padding:5px;'>Symbol</th>";
    html += "<th style='padding:5px;'>Action</th>";
    html += "</tr>";
    html += "</thead>";
    html += "<tbody>";
    html += "</tbody>";
    html += "</table>";
    html += "<br/>";
    html += "<div style='margin:10px 0px;'>";
    html += "<button type='button' id='btnCheckSymbol'>Check Symbol</button>";
    html += "</div>"
    $container.append(html);
    $('body').append($container);
    id = 'gc_right_setup_symbol_cnt';
    $("#" + id).remove();
    $right_container = $("<div id='" + id + "' style='position:absolute;bottom:0;right:0;height:auto;width:235px;background:#fff;color:#333;z-index:99999;padding:10px;'></div>");
    var html = '';
    html += "<div style='margin:10px 0px;'>";
    html += "<button type='button' id='btnFill'>Fill</button>";
    html += "<input type='hidden' id='gc_total_amount' value='" + totalAmount + "'>";
    html += "<input type='hidden' id='gc_stop_loss_percentage' value='" + stopLoss + "'>";
    html += "<input type='hidden' id='gc_target_parcentage' value='" + target + "'>";
    html += "<input type='hidden' id='gc_trailing_stop_loss_percentage' value='" + trailingStopLoss + "'>";
    html += "</div>"
    $right_container.append(html);
    $('body').append($right_container);
    $('body').append($right_container);
    //console.log('symbols=', symbols);
    //console.log(1);
    SYMBOL_INDEX = -1;
    var $tbl = $("#tblSymbolList", $container);
    var $tbody = $("tbody", $tbl);
    var i;
    var rows = symbols.split('|');
    for (i = 0; i < rows.length; i++) {
        var arr = rows[i].split(':');
        var symbol = arr[0];
        var action = arr[1];
        var $tr = $("<tr></tr>");
        $tr.append("<td style='padding:5px;'>" + symbol + "</td>");
        var html = "";
        html += "<td style='padding:5px;'>" + (action == 'B' ? 'Buy' : 'Sell');
        html += "<input type='hidden' id='hdnsymbol' value='" + symbol + "'>";
        html += "<input type='hidden' id='hdnaction' value='" + action + "'>";
        html += "</td>";
        $tr.append(html);
        $tbody.append($tr);
    }
    var $btnCheckSymbol = $("#btnCheckSymbol", $container);
    $btnCheckSymbol.click(function () {
        $("tr", $tbody).each(function () {
            var $tr = $(this);
            //console.log('$tr', $tr[0]);
            var symbol = $("#hdnsymbol", $tr).val();
            var action = $("#hdnaction", $tr).val();
            var $symbolrow = null;
            var $list = $(".vddl-list.list-flat");
            //console.log('$list', $list[0]);
            $('.vddl-draggable.instrument', $list).each(function () {
                var $row = $(this);
                //console.log('$row', $row[0]);
                var $name = $('.info > .symbol > .nice-name', $row);
                var name = $.trim($name.html());
                name = name.replace('&amp;', '&').replace('&amp', '&');
                if (symbol == 'M&M') {
                    console.log('name=', name, 'symbol=', symbol, 'action=', action);
                }
                if (name == symbol) {
                    $name.html(symbol + ' - ' + (action == 'B' ? 'BUY' : 'SELL'));
                    //$name.css({
                    //    'color': '#fff'
                    //});
                    $row.css({
                        'background': (action == 'B' ? '#99EF9D' : '#FFDDDD')
                    });
                }
            });
        });
    });
    var $btnFill = $("#btnFill", $right_container);
    $btnFill.click(function () {
        var $orderwindow = $('.vdr.order-window-draggable');
        var tradeSymbol = $.trim($('.tradingsymbol', $orderwindow).html());
        tradeSymbol = tradeSymbol.replace('&amp;', '&').replace('&amp', '&');
        $("tr", $tbody).each(function () {
            var $tr = $(this);
            //console.log('$tr', $tr[0]);
            //var symbol = $("#hdnsymbol", $tr).val();
            //var action = $("#hdnaction", $tr).val();
            //if (tradeSymbol == symbol) {
             //   console.log('tradeSymbol=', tradeSymbol, 'symbol=', symbol, 'action=', action);
            //}
            var price = parseFloat($(":input[type='number'][label='Price']").val());
            var totalInvestment = parseFloat($(":input[id='gc_total_amount']").val());
            var stopLossPercentage = parseFloat($(":input[id='gc_stop_loss_percentage']").val());
            var targetPercentage = parseFloat($(":input[id='gc_target_parcentage']").val());
            var trailingStoplossPercentage = parseFloat($(":input[id='gc_trailing_stop_loss_percentage']").val());
            var totalEquities = parseInt($("tbody > tr", $tbl).length);
            if (totalEquities <= 0) {
                totalEquities = 1;
            }
            var investmentPerEquity = parseInt(totalInvestment / totalEquities);
            var qty = parseInt(investmentPerEquity / price);
            var stoploss = parseFloat((price * stopLossPercentage) / 100);
            var target = parseFloat((price * targetPercentage) / 100);
            var trailing = parseFloat((price * trailingStoplossPercentage) / 100);
            console.log('price=', price, 'totalInvestment=', totalInvestment, 'stopLossPercentage=', stopLossPercentage, 'targetPercentage=', targetPercentage, 'trailingStoplossPercentage=', trailingStoplossPercentage);
            console.log('totalEquities=', totalEquities, 'investmentPerEquity=', investmentPerEquity);
            console.log('qty=', qty, 'stoploss=', stoploss, 'target=', target, 'trailing=', trailing);
            stoploss = convertDecimalPart(stoploss);
            target = convertDecimalPart(target);
            trailing = convertDecimalPart(trailing);
            console.log('stoploss=', stoploss, 'target=', target, 'trailing=', trailing);
            var $content = $('.content', $orderwindow);
            $('#gc_helper_notes', $orderwindow).remove();
            var $helperNotes = $("<div id='gc_helper_notes' style='position:absolute;right:0;top:-150px;left:0;background:#fff;font-weight:bold;border:solid 1px #333;padding:5px;width:800px;'></div>");
            var html = '';
            html += '<table>';
            html += '<tbody>';
            html += '<tr>';
            html += '<td style="padding:5px;white-space:nowrap;">Total Investment: ' + totalInvestment + '</td>';
            html += '<td style="padding:5px;white-space:nowrap;">Investment Per Equity: ' + investmentPerEquity + '</td>';
            html += '<td style="padding:5px;white-space:nowrap;">SL: ' + stopLossPercentage + '%' + '</td>';
            html += '<td style="padding:5px;white-space:nowrap;">TARGET: ' + targetPercentage + '%' + '</td>';
            html += '<td style="padding:5px;white-space:nowrap;">TRAIL-SL: ' + trailingStoplossPercentage + '%' + '</td>';
            html += '</tr>';
            html += '<tr>';
            html += '<td style="padding:5px;white-space:nowrap;">Symbol: ' + tradeSymbol + '</td>';
            html += '<td style="padding:5px;white-space:nowrap;">Price: ' + price + '</td>';
            //html += '<td style="padding:5px;white-space:nowrap;">Action: ' + (action == 'B' ? 'Buy' : 'Sell') + '</td>';
            html += '</tr>';
            html += '<tr>';
            html += '<td>Qty: ' + qty + '</td>';
            html += '<td style="padding:5px;white-space:nowrap;">StopLoss: ' + stoploss + '</td>';
            html += '<td style="padding:5px;white-space:nowrap;">Target: ' + target + '</td>';
            html += '<td style="padding:5px;white-space:nowrap;">Trailing: ' + trailing + '</td>';
            html += '</tr>';
            html += '</tbody>';
            html += '</table>';
            $helperNotes.append(html);
            $orderwindow.append($helperNotes);
            //$(":input[type='number'][label='Stoploss']").val(stoploss).keypress();
            //$(":input[type='number'][label='Target']").val(target).keypress();
            //$(":input[type='number'][label='Trailing stoploss']").val(trailing).keypress();
            //var qty = 
        });
    });
}

function convertDecimalPart(num) {
    var org = num;
    var decimalPart = num % 1;
    if (decimalPart <= 0.5 && decimalPart > 0.1) {
        num = num + (0.5 - decimalPart);
    } else if (decimalPart > 0.5) {
        num = num - (decimalPart - 0.5);
    } else {
        num = parseInt(num);
    }
    console.log('org=', org, 'num=', num, 'decimalPart=', decimalPart, '(decimalPart - 0.5)=', (decimalPart - 0.5));
    return num;
}