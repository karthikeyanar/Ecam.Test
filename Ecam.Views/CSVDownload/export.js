function tableStructure() {
    this.cols = [];
    this.rows = [];
}

function removeHTML(html) {
    var regex = /(<([^>]+)>)/ig,
        body = html,
        result = body.replace(regex, "");

    return $.trim(result); //.replace(/\s+$/g, "").replace(/(\r\n|\n|\r)/gm, "").replace(/\s+/g, "");
}

// The download function takes a CSV string, the filename and mimeType as parameters
// Scroll/look down at the bottom of this snippet to see how download is called
var download = function (content, fileName, mimeType) {
    var a = document.createElement('a');
    mimeType = mimeType || 'application/octet-stream';

    if (navigator.msSaveBlob) { // IE10
        navigator.msSaveBlob(new Blob([content], {
            type: mimeType
        }), fileName);
    } else if (URL && 'download' in a) { //html5 A[download]
        a.href = URL.createObjectURL(new Blob([content], {
            type: mimeType
        }));
        a.setAttribute('download', fileName);
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
    } else {
        location.href = 'data:application/octet-stream,' + encodeURIComponent(content); // only this mime type is supported
    }
}

function exportTable(tabid) {
    var $tbl = $("table:eq(3)");
    var symbol = '';
    try {
        var $title = $("title");
        var html = $title.html();
        var myRegexp = /NSE:\s*(?<symbol>.*)/g;
        var match = myRegexp.exec(html);
        symbol = match[1];
    } catch (e) {
        console.log(e);
    }
    //console.log($tbl[0]);
    var colRowIndex = 0;
    var cols = '';
    var struct = new tableStructure();
    var isStop = false;
    $("tbody > tr", $tbl).each(function (i) {
        if (isStop == false) {
            colRowIndex = i;
            var $tr = $(this);
            $("td.detb.brdL.brdR", $tr).each(function (z) {
                isStop = true;
                var $td = $(this);
                var value = removeHTML($td.html());
                if (z == 0) {
                    value = symbol;
                }
                if (value.indexOf('Jun') >= 0) {
                    value = value.replace('Jun \'', '06/01/');
                } else if (value.indexOf('Mar') >= 0) {
                    value = value.replace('Mar \'', '03/01/');
                } else if (value.indexOf('Dec') >= 0) {
                    value = value.replace('Dec \'', '12/01/');
                } else if (value.indexOf('Sep') >= 0) {
                    value = value.replace('Sep \'', '09/01/');
                }
                struct.cols.push(value);
            });
        } else {
            return false;
        }
    });
    //console.log('colRowIndex=', colRowIndex);
    $("tbody > tr", $tbl).each(function (j) {
        //console.log('j=', j);
        if (colRowIndex < j) {
            var $tr = $(this);
            var row = [];
            $("td.det.brdL.brdR", $tr).each(function (i) {
                var $td = $(this);
                var value = removeHTML($td.html());
                value = value.replace('--', '').replace('&amp;', '&');
                ////console.log('i=', i, 'value=', value);
                if (i < struct.cols.length) {
                    row.push(value);
                }
            });
            if (row.length > 0) {
                struct.rows.push(row);
            }
        }
    });
    //console.log(struct);
    var csvContent = convertCSVContent(struct);
    var fileName = '';
    if (struct.cols.length > 0) {
        fileName += struct.cols[0] + '-' + struct.cols[2] + '-' + struct.cols[struct.cols.length - 1] + '.csv';
    }
    console.log('fileName=', fileName);
    download(csvContent, fileName, 'text/csv;encoding:utf-8');
    setTimeout(function(){
        chrome.runtime.sendMessage({ cmd: 'close_tab','tabid':tabid  });
    },500);
}

function convertCSVContent(struct) {
    var content = '';
    var cols = '';
    var rows = '';
    for (var i = 0; i < struct.cols.length; i++) {
        cols += '"' + struct.cols[i] + '",';
    }
    if (cols != '') {
        cols = cols.substring(0, cols.length - 1);
    }
    cols = cols + '\n';

    for (var i = 0; i < struct.rows.length; i++) {
        var row = '';
        for (var j = 0; j < struct.rows[i].length; j++) {
            row += '"' + struct.rows[i][j] + '",';
        }
        if (row != '') {
            row = row.substring(0, row.length - 1);
        }
        rows += row + '\n';
    }

    content = cols + rows;
    //console.log(content);
    return content;
}