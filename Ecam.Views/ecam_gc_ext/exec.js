String.prototype.padL=function(width,pad) {
	if(!width||width<1)
		return this;

	if(!pad) pad=" ";
	var length=width-this.length;
	if(length<1) return this.substr(0,width);

	return (this.repeat(pad,length)+this).substr(0,width);
};
var _monthNames=['January','February','March','April','May','June','July','August','September','October','November','December'];
Date.prototype.formatDate=function(format) {
	var date=this;
	if(!format)
		format="MM/dd/yyyy";

	var month=date.getMonth();
	var year=date.getFullYear();

	if(format.indexOf("yyyy")>-1)
		format=format.replace("yyyy",year.toString());
	else if(format.indexOf("yy")>-1)
		format=format.replace("yy",year.toString().substr(2,2));

	format=format.replace("dd",date.getDate().toString().padL(2,"0"));

	var hours=date.getHours();
	if(format.indexOf("t")>-1) {
		if(hours>11)
			format=format.replace("t","pm");
		else
			format=format.replace("t","am");
	}
	if(format.indexOf("HH")>-1)
		format=format.replace("HH",hours.toString().padL(2,"0"));
	if(format.indexOf("hh")>-1) {
		if(hours>12) hours-=12;
		if(hours==0) hours=12;
		format=format.replace("hh",hours.toString().padL(2,"0"));
	}
	if(format.indexOf("mm")>-1)
		format=format.replace("mm",date.getMinutes().toString().padL(2,"0"));
	if(format.indexOf("ss")>-1)
		format=format.replace("ss",date.getSeconds().toString().padL(2,"0"));

	if(format.indexOf("MMMM")>-1)
		format=format.replace("MMMM",_monthNames[month]);
	else if(format.indexOf("MMM")>-1)
		format=format.replace("MMM",_monthNames[month].substr(0,3));
	else
		format=format.replace("MM",(month+1).toString().padL(2,"0"));

	return format;
};


var LIBRARY=[];

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
function pageLoad(pageUrl,tabId,openerId) {
	console.log('pageLoad pageUrl=',pageUrl,'tabId=',pageUrl,'openerId=',openerId);
	var $btn=$('#btn_gc_init');
	if(!$btn[0]) {
		$btn=$("<button type='button' id='btn_gc_init' style='display:none;position:absolute;top:100px;z-index:99999;'>Google Page Load Trigger</button>");
		$btn.unbind('click').click(function() {
			chrome.runtime.sendMessage({ cmd: 'page_load','page_url': pageUrl,'tabid': tabId,'openerid': openerId });
		});
		$('body').append($btn);
		setTimeout(function() {
			$btn.click();
		},2000);
	} else {
		var hostName=getHostName(pageUrl);
		console.log('pageLoad hostName=',hostName);
		console.log('pageLoad LIBRARY.length=',LIBRARY.length);
		var i; var library=null;
		for(i=0;i<LIBRARY.length;i++) {
			if(LIBRARY[i].host_name==hostName) {
				library=LIBRARY[i];
			}
		}
		console.log('pageLoad library=',library);
		if(library!=null) {
			if(library.reservation_name=='ECHAMP') {
				library.obj.findAWBNoAndLoad();
			}
		}
	}
}

function getLibrary() {
	return LIBRARY;
}

function helpLog(tabid) {
	//console.log(window);
}

function init(tabid,openerid,storage) {
	console.log('init tabid=',tabid,'openerid=',openerid);
	var json=JSON.parse(storage);
	var obj=null;
	console.log('json.reservation.reservation_name='+json.reservation.reservation_name);
	switch(json.reservation.reservation_name) {
		case 'LMS':
			obj=new LMS();
			break;
		case 'ECHAMP':
			obj=new ECHAMP();
			break;
	}
	if(obj) {
		LIBRARY.push({ 'obj': obj,'host_name': json.host_name,'reservation_name': json.reservation.reservation_name });
		obj.init(tabid,openerid,json.reservation.reservation_id,json,storage);
	}
}



function createAWBBOX(self) {
	var awbs=self.JSON.awbs;
	var $gc_awb_box=$("#gc_awb_box");
	if($gc_awb_box[0]) {
		$gc_awb_box.remove();
	}
	$("body").append("<div id='gc_awb_box'><table id='gc_awb_list' style='border:solid 1px #333;border-collapse: collapse !important;'><thead></thead><tbody></tbody></table></div>");
	$gc_awb_box=$("#gc_awb_box");
	$gc_awb_box.css({
		'position': 'absolute',
		'top': '10px',
		'right': '10px',
		'width': 'auto',
		'background': '#fff',
		'color': '#333',
		'font-family': 'Arial',
		'font-size': '14px',
		'padding': '10px'
	});
	var $tbl=$("#gc_awb_list",$gc_awb_box);
	var $thead=$("thead",$tbl);
	var $tbody=$("tbody",$tbl);
	var $tr=null;
	$tr=$("<tr><th>#</th><th style='font-size:16px;font-weight:normal;padding:5px;text-align:left;'>AWB NO</th><th style='font-size:20px;font-weight:normal;padding:5px;text-align:left;'>Status</th><th></th></tr>");
	$thead.append($tr);
	var i;
	for(i=0;i<awbs.length;i++) {
		var tdStyle="font-size:20px;font-weight:normal;padding:5px;white-space:nowrap;";
		var html="<tr awb_no='"+awbs[i].awb_no+"'>";
		html+="<td style='"+tdStyle+"'>"+(i+1)+"</td>";
		html+="<td style='"+tdStyle+"'>"+awbs[i].awb_no+"</td>";
		html+="<td style='"+tdStyle+"' id='tdStatus'>";
		if(awbs[i].chrome_error!='') {
			html+=awbs[i].chrome_error;
		} else {
			if(awbs[i].chrome_status=='S') {
				html+="Submitted";
			} else {
				html+="Pending";
			}
		}
		html+="</td>";
		html+="<td style='"+tdStyle+"'><input type='hidden' id='gc_hdn_awb_no' value='"+awbs[i].awb_no+"' /><button id='btnLoad' style='background-image:none;padding: 6px 12px;font-size: 16px;text-align: center;white-space: nowrap;vertical-align: middle;color: #fff;background-color: #337ab7;border-color: #2e6da4;border: 1px solid transparent;border-radius: 4px;'>Submit</td>";
		html+="</tr>";
		$tr=$(html);
		$tbody.append($tr);
		$("#btnLoad",$tr).click(function() {
			var $btn=$(this);
			var $parentTR=$btn.parents('tr:first');
			var awbno=$("#gc_hdn_awb_no",$parentTR).val();
			console.log('createAWBBOX self.JSON=',self.JSON);
			var data=self.getAWB(awbno);
			if(data!=null) {
				self.LoopIndex=0;
				self.loadValues(data);
			}
		});
	}
}


