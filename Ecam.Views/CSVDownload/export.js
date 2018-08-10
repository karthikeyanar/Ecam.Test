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

function exportTable(tabid,openerId) {
    window.alert = function() {};
    var $tbl = $("table:eq(3)");
    var symbol = '';
    try {
        var $title = $("title");
        var html = $title.html();
        var myRegexp = /NSE:\s*(?<symbol>.*)/g;
        var match = myRegexp.exec(html);
        symbol = match[1];
    } catch (e) {
        ////console.log(e);
    }
    //////console.log($tbl[0]);
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
    //////console.log('colRowIndex=', colRowIndex);
    $("tbody > tr", $tbl).each(function (j) {
        //////console.log('j=', j);
        if (colRowIndex < j) {
            var $tr = $(this);
            var row = [];
            $("td.det.brdL.brdR", $tr).each(function (i) {
                var $td = $(this);
                var value = removeHTML($td.html());
                value = value.replace('--', '').replace('&amp;', '&');
                ////////console.log('i=', i, 'value=', value);
                if (i < struct.cols.length) {
                    row.push(value);
                }
            });
            if (row.length > 0) {
                struct.rows.push(row);
            }
        }
    });
    //////console.log(struct);
    var csvContent = convertCSVContent(struct);
    var fileName = '';
    if (struct.cols.length > 0) {
        fileName += struct.cols[0] + '-' + struct.cols[1] + '-' + struct.cols[struct.cols.length - 1] + '.csv';
    }
    ////console.log('fileName=', fileName);
    download(csvContent, fileName, 'text/csv;encoding:utf-8');
    setTimeout(function(){
        //chrome.runtime.sendMessage({ cmd: 'mc-quaterly-downloaded','tabid':openerId  });
        chrome.runtime.sendMessage({ cmd: 'close_tab','tabid':tabid,'openerid':openerId  });
    },1000);
}

function pageLoad(pageUrl,tabId,openerId,symbol) {
    ////console.log('pageLoad pageUrl=',pageUrl,'tabId=',pageUrl,'openerId=',openerId);
    nseStart(pageUrl,tabId,openerId,symbol);
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
    //////console.log(content);
    return content;
}

var _NSE = null;
/* */
function nseStart(pageUrl,tabId,openerId,symbol){
    ////console.log('nseStart tabId=',tabId,'openerId=',openerId,'symbol=',symbol);
    _NSE = new NSE();
    _NSE.init(pageUrl,tabId,openerId,symbol);
}
/* */

function NSE(){

    var self = this;
    this.tabId = 0;
    this.openerId = 0;
    this.pageUrl = '';
    this.symbol = '';
    this.symbolsArray = [];
    this.index = -1;
    
    this.init = function(pageUrl,tabId,openerId,symbol){
        ////console.log('NSE init tabId=',tabId,'openerId=',openerId,'symbol=',symbol);
        self.tabId=parseInt(tabId);
        self.openerId=parseInt(openerId);
        self.pageUrl=pageUrl;
        self.symbol=symbol;
        self.symbolsArray = self.symbol.split(',');
        self.index=-1;
        self.downloadData();
        //var code = "$('#btnNSEDownloadBackground').attr('tabid',"+self.tabId+");$('#btnNSEDownloadBackground').click();";
        //chrome.runtime.sendMessage({ cmd: 'execute_code','tabid':openerId,'code':code  });
    }

    this.downloadData = function() {
        var symbol = '';var startDate = '';var endDate = '';var nseType = '';
        self.index+=1; 
        console.log('self.index=',self.index,',total=',self.symbolsArray.length);
        if(self.index<=self.symbolsArray.length){
            var arr = self.symbolsArray[self.index].split('|');
            symbol = arr[0];
            startDate = arr[1];
            endDate = arr[2];
            nseType = arr[3];
            if(nseType==''){
                nseType='EQ';
            }
            console.log('download symbol=',symbol,'startDate=',startDate,'endDate=',endDate,'nseType=',nseType);
            var $frm = $("#histForm");
            var $dataType = $("#dataType",$frm);
            var $symbol = $("#symbol",$frm);
            var $series = $("#series",$frm);
            var $rdDateToDate = $("#rdDateToDate",$frm);
            var $csvFileName=$('#csvFileName');
            var $csvContentDiv=$('#csvContentDiv');

            $csvFileName.val('');
            $csvContentDiv.val('');

            $dataType.val('priceVolume');
            $symbol.val(symbol);
            $series.val(nseType);
            $rdDateToDate[0].checked=true;

            var $fromDate = $("#fromDate",$frm);
            $fromDate.val(startDate);
            var $toDate = $("#toDate",$frm);
            $toDate.val(endDate);

            var $btn = $(".getdata-button",$frm);
            $btn.click();
            
            setTimeout(function(){
                $csvFileName=$('#csvFileName');
                $csvContentDiv=$('#csvContentDiv');
                var csv='';
                var fileName = '';
                ////console.log('$csvFileName=',$csvFileName[0]);
                ////console.log('$csvContentDiv=',$csvContentDiv[0]);
                if($csvFileName[0]) {
                    if($csvFileName.val()!='') {
                        fileName=$csvFileName.val();
                        csv=$csvContentDiv.html();
                    }
                } 
                //console.log('fileName=',fileName);
                //console.log('csv=',csv);
                if(csv!='') {
                    //csv=csv.replace(/:/g,"\n");
                    var code = "$('#nse_index').val('"+(self.index+1)+"');$('#nse_total').val('"+self.symbolsArray.length+"');$('#nse_csv').val('"+csv+"');$('#btnNSEUpdateCSV').click();";
                    chrome.runtime.sendMessage({ cmd: 'execute_code','tabid':self.openerId,'code':code  });
                    setTimeout(function(){
                        csv=csv.replace(/:/g,"\n");
                        var csvFile;
                        var downloadLink;
                        csvFile=new Blob([csv],{
                            type: "text/csv"
                        });
                        downloadLink=document.createElement("a");
                        downloadLink.download=fileName;
                        downloadLink.href=window.URL.createObjectURL(csvFile);
                        downloadLink.style.display="none";
                        document.body.appendChild(downloadLink);
                        downloadLink.click();
                    },500);
                    setTimeout(function(){
                        self.downloadData();
                    },1000);
                } else {
                    var $missingSymbols = $("#missing_symbols");
                    if(!$missingSymbols[0]){
                        $("body").append("<div id='missing_symbols' style='position:absolute;left:100px;top:100px;font-size:18px;'></div>");
                    }
                    $missingSymbols.html($missingSymbols.html() + symbol + "</br>");
                    self.downloadData();
                }
            },10000);
            //}
        }
    }
     
}