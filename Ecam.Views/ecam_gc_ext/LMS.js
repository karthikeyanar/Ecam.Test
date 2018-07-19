//////alert('pageLoadScript run');
function LMS() {
	var self=this;
	this.TABID=null;
	this.OPENERID=null;
	this.RESERVATIONID=null;
	this.JSON=null;
	this.JSONTXT=null;
	this.INDEX=-1;
	this.DOMAIN='';
	this.init=function(tabid,openerid,reservationid,json,jsonTXT) {
		self.TABID=tabid;
		self.OPENERID=openerid;
		self.RESERVATIONID=reservationid;
		self.JSON=json;
		self.JSONTXT=jsonTXT;
		console.log('tabid=',self.TABID,'openerid=',self.OPENERID);
		console.log('self.JSON.page_url=',self.JSON.page_url);
		var i;
		for(i=0;i<self.JSON.awbs.length;i++) {
			if(!self.JSON.awbs[i].chrome_status) {
				self.JSON.awbs[i].chrome_status='P';
			}
			if(!self.JSON.awbs[i].chrome_error) {
				self.JSON.awbs[i].chrome_error='';
			}
			if(!self.JSON.awbs[i].chrome_retrieve) {
				self.JSON.awbs[i].chrome_retrieve='';
			}
		}
		self.JSON.page_url=self.JSON.page_url.toLowerCase();
		self.DOMAIN=getHostName(self.JSON.page_url);
		console.log('self.DOMAIN=',self.DOMAIN);
		switch(self.JSON.page_url) {
			case 'https://aicargo.airindia.in/':
			case 'https://airasiacargo.airasia.com/':
				self.login();
				break;
			case 'https://aicargo.airindia.in/web_si-so.asp':
			case 'https://airasiacargo.airasia.com/web_si-so.asp':
				window.location.href='/BOOKING_INFORMATION.ASP';
				break;
			case 'https://aicargo.airindia.in/booking_information.asp':
			case 'https://airasiacargo.airasia.com/booking_information.asp':
				self.INDEX=-1;
				self.checkErrorOnAWBNO();
				self.formFill();
				break;
		}
	}
	this.login=function() {
		if(self.JSON.reservation) {
			$(":input[name='txtUserId']").val(self.JSON.reservation.user_id);
			$(":input[name='txtPassword']").val(self.JSON.reservation.password);
			$(":input[name='btnsubmit']").click();
		}
	}

	this.formFill=function() {
		createAWBBOX(self);
	}

	this.checkErrorOnAWBNO=function() {
		var $txtACN=$(":input[name='txtACN']");
		var $txtSRef=$(":input[name='txtSRef']");
		var awbno=$txtACN.val()+$txtSRef.val();
		var $messageSection=$('#idMessageSection.inpanewarning');
		var $h3error=$(".h3error",$messageSection);
		console.log('$messageSection=',$messageSection[0]);
		console.log('$h3error=',$h3error[0]);
		var data=self.getAWB(awbno);
		if(data!=null) {
			if($h3error[0]) {
				data.chrome_error=$h3error.html();
				self.updateJSON();
			}
			self.loadValues(data);
		}
		//console.log('awbno=',awbno,'awbPrefix=',awbPrefix,'awbSuffix=',awbSuffix,'txtACN=',txtACN);
	}

	this.updateJSON=function() {
		var currentHostName=getHostName(self.JSON.page_url);
		chrome.runtime.sendMessage({ cmd: 'update_json','data': JSON.stringify(self.JSON),'host_name': currentHostName });
	}

	this.resetOtherAWBNORetrive=function(awbno) {
		var i;
		for(i=0;i<self.JSON.awbs.length;i++) {
			if(self.JSON.awbs[i].awb_no!=awbno) {
				self.JSON.awbs[i].chrome_retrieve='';
			}
		}
	}

	this.loadValues=function(data) {
		var $txtACN=$(":input[name='txtACN']");
		var txtACN=$txtACN.val();
		var $gc_awb_box=$("#gc_awb_box");
		var $tbl=$("#gc_awb_list",$gc_awb_box);
		var awbno=data.awb_no;
		var $tdStatus=$("tr[awb_no='"+awbno+"'] > #tdStatus",$tbl);
		var $btnLoad=$("tr[awb_no='"+awbno+"'] #btnLoad",$tbl);
		$tdStatus.html('Submit...');
		var awbPrefix='';
		var awbSuffix='';
		if(awbno.length>=11) {
			awbPrefix=awbno.substring(0,3);
			awbSuffix=awbno.substring(3,awbno.length);
		}
		//console.log('awbno=',awbno,'awbPrefix=',awbPrefix,'awbSuffix=',awbSuffix,'txtACN=',txtACN);
		if(awbPrefix!=txtACN) {
			$tdStatus.html('AWB Prefix is wrong.please check');
		} else {
			$(":input[name='txtProduct']").val('NOR');
			if(data.cargo_type=='S') {
				$(":input[name='txtProduct']").val('FOC');
			}
			$(":input[name='txtSRef']").val(awbSuffix);
			if(cString(data.chrome_retrieve)=='') {
				data.chrome_retrieve='true';
				self.resetOtherAWBNORetrive(data.awb_no);
				self.updateJSON();
				self.onEventFire('btnRetrieve','click');
			}
			if(cString(data.chrome_retrieve)=='true') {
				$(":input[name='txtERPayCode']").val('PX');
				$(":input[name='txtIRPayCode']").val('CX');
				$(":input[name='txtDescription']").val(data.commodity+' - '+data.agent_name);

				var $tbody=$("#participant2");
				if(data.agent_code)
					$(":input[name='txtAccount']",$tbody).val(data.agent_code);
				else
					$(":input[name='txtAccount']",$tbody).val(data.agent_iata_code);

				if(data.setting_agent_name)
					$(":input[name='txtPartName']",$tbody).val(data.setting_agent_name);
				else
					$(":input[name='txtPartName']",$tbody).val(data.agent_name);

				$(":input[name='txtStation']",$tbody).val(data.agent_airport_code);

				//console.log('$tbody=',$tbody[0]);
				//console.log('txtAccount=',$(":input[name='txtAccount']",$tbody)[0]);
				//console.log('txtPartName=',$(":input[name='txtPartName']",$tbody)[0]);
				//console.log('txtStation=',$(":input[name='txtStation']",$tbody)[0]);

				//self.onEventFire('txtAccount','onchange');
				//self.onEventFire('txtPartName','onchange');
				//self.onEventFire('txtStation','onchange');

				var j;
				// Reset all routes
				var originName='';
				var destName='';
				for(j=0;j<6;j++) {
					originName='txtRoute'+(j+1);
					var $txt=$(":input[name='"+originName+"']");
					$txt.val('');
				}
				self.onEventFire('txtRoute2','onchange');
				var $tbodySegmentDetails=$("#segmentDetails");
				$("tr",$tbodySegmentDetails).each(function() {
					var $tr=$(this);
					$(":input[name='txtPieces']",$tr).val('');
					$(":input[name='txtWeight']",$tr).val('');
					$(":input[name='txtVolume']",$tr).val('');
					$(":input[name='txtFlight']",$tr).val('');
					$(":input[name='selFlightDate']",$tr).val('');
				});
				self.onEventFire('txtPieces','onchange');
				self.onEventFire('txtWeight','onchange');
				self.onEventFire('txtVolume','onchange');
				self.onEventFire('txtFlight','onchange');
				self.onEventFire('selFlightDate','onchange');

				setTimeout(function() {
					if(data.flight_details) {
						j=0;
						for(j=0;j<data.flight_details.length;j++) {
							originName='';
							destName='';
							switch(j) {
								case 0:
									originName='txtRoute1';
									destName='txtRoute2';
									break;
								case 1:
									destName='txtRoute3';
									break;
								case 2:
									destName='txtRoute4';
									break;
								case 3:
									destName='txtRoute5';
									break;
								case 4:
									destName='txtRoute6';
									break;
							}
							if(originName!='') {
								var $txt=$(":input[name='"+originName+"']");
								$txt.val(data.flight_details[j].origin_code);
							}
							if(destName!='') {
								var $txt=$(":input[name='"+destName+"']");
								//console.log('$txt=',$txt[0]);
								//console.log('self.JSON.reservation.airline_code=',self.JSON.reservation.airline_code);
								//console.log('data.flight_details[j].dest_code+self.JSON.reservation.airline_code=',data.flight_details[j].dest_code+self.JSON.reservation.airline_code);
								$txt.val(data.flight_details[j].dest_code+self.JSON.reservation.airline_code);
							}
						}
					}
					self.onEventFire('txtRoute2','onchange');
					var $tbodySegmentDetails=$("#segmentDetails");
					if(data.flight_details) {
						var j;
						for(j=0;j<data.flight_details.length;j++) {
							var $tr=$("tr:eq("+j+")",$tbodySegmentDetails);
							if(j<=0) {
								$(":input[name='txtPieces']",$tr).val(data.flight_details[j].pieces);
								$(":input[name='txtWeight']",$tr).val(data.flight_details[j].grwt);
							}
							//$(":input[name='txtVolume']",$tr).val(data.flight_details[j].chwt);
							$(":input[name='txtFlight']",$tr).val(data.flight_details[j].flight_no);
							$(":input[name='selFlightDate']",$tr).val((new Date(data.flight_details[j].flight_date)).formatDate('ddMMMyy').toUpperCase());
							$(":input[name='txtFlightStatus']",$tr).val('KK');
						}
						self.onEventFire('txtPieces','onchange');
						self.onEventFire('txtWeight','onchange');
						self.onEventFire('txtVolume','onchange');
						self.onEventFire('txtFlight','onchange');
						self.onEventFire('selFlightDate','onchange');
						$tdStatus.html('Submitted');
						$btnLoad.css({
							'background-color': 'red','color': 'white'
						}).html('Resubmit');
						self.passOpenerTab(data);
					}
				},500);
			}
		}
	}

	this.passOpenerTab=function(awbdata) {
		var data=self.getAWB(awbdata.awb_no);
		if(data!=null) {
			data.chrome_status='S';
		}
		var code="completedAWBNO('"+awbdata.company_id+"','"+awbdata.awb_no+"','"+awbdata.awb_index+"','"+self.RESERVATIONID+"');";
		chrome.runtime.sendMessage({ cmd: 'execute_code','code': code,'tabid': self.OPENERID });
		self.updateJSON();
	}

	this.getAWB=function(awbno) {
		var data=null;
		var i;
		for(i=0;i<self.JSON.awbs.length;i++) {
			if(self.JSON.awbs[i].awb_no==awbno) {
				data=self.JSON.awbs[i];
			}
		}
		return data;
	}

	this.onEventFire=function(name,event) {
		var html="<button type='button' id='gc_btn_routing_change' onclick='var collections=document.getElementsByName(\""+name+"\");var i;for(i=0;i<collections.length;i++){collections[i]."+event+"();}' style='position:absolute;top:10px;display:none;'>Routing Change</button>";
		$('body').append(html);
		$('#gc_btn_routing_change').click();
		$('#gc_btn_routing_change').remove();
	}

	this.sendKeys=function($txt,value) {
		//console.log('$txt=',$txt[0],'value=',value);
		$txt.sendkeys(value);
	}
}
