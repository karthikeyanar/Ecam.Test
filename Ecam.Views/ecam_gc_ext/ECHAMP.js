function ECHAMP() {
	var self=this;
	this.TABID=null;
	this.OPENERID=null;
	this.RESERVATIONID=null;
	this.JSON=null;
	this.JSONTXT=null;
	this.INDEX=-1;
	this.init=function(tabid,openerid,reservationid,json,jsonTXT) {
		self.TABID=tabid;
		self.OPENERID=openerid;
		self.RESERVATIONID=reservationid;
		self.JSON=json;
		self.JSONTXT=jsonTXT;
		//console.log('tabid='+self.TABID+',openerid='+self.OPENERID);
		//console.log('self.JSON.page_url='+self.JSON.page_url);
		var i;
		for(i=0;i<self.JSON.awbs.length;i++) {
			self.JSON.awbs[i].chrome_status='P';
			self.JSON.awbs[i].chrome_error='';
		}
		switch(self.JSON.page_url) {
			case 'https://webportal.champ.aero/wcs/':
			case 'https://webportal.champ.aero/wcs':
			case 'https://webportal.champ.aero/wcs/default.asp?':
			case 'https://webportal.champ.aero/wcs/default.asp':
				self.login();
				break;
			case 'https://webportal.champ.aero/wcs/default.asp?Category=ChampMenu':
			case 'https://webportal.champ.aero/wcs/default.asp?Category=ChampMenu&Service=Main':
				if(self.checkSessionTimeout()==false) {
					self.INDEX=-1;
					self.formFill();
					var isNotOpenBookingMenu=false;
					var $cnt=$('#main').contents();
					var $frm=$cnt.find("form[name='MSB']");
					var $btnViewResOnly=$cnt.find(":input[value='View Res Only']");
					try {
						if($btnViewResOnly[0]||$frm.attr('id')=='f1') {
							isNotOpenBookingMenu=true;
						}
					} catch(e) {
						console.log('init e=',e);
					}
					console.log('isNotOpenBookingMenu=',isNotOpenBookingMenu);
					if(isNotOpenBookingMenu==false) {
						self.openBookingMenu();
						self.redirectBookingScreen();
					} else {
						self.findAWBNoAndLoad();
					}
				}
				break;
		}
	}
	this.openBookingMenu=function() {
		var code="document.getElementById('itemTextLink21').onclick()";
		var html='<button type="button" id="gc_btn_routing_change" onclick="'+code+'" style="position:absolute;top:10px;">Routing Change</button>';
		$('#navleft').contents().find('body').append(html);
		$('#navleft').contents().find('#gc_btn_routing_change').click();
		$('#navleft').contents().find('#gc_btn_routing_change').remove();
	}
	this.redirectBookingScreen=function() {
		var code="document.getElementById('itemTextLink22').onclick()";
		var html='<button type="button" id="gc_btn_routing_change" onclick="'+code+'" style="position:absolute;top:10px;">Routing Change</button>';
		$('#navleft').contents().find('body').append(html);
		$('#navleft').contents().find('#gc_btn_routing_change').click();
		$('#navleft').contents().find('#gc_btn_routing_change').remove();
	}
	this.findAWBNoAndLoad=function() {
		$('#main').attr('width','70%');
		var $cnt=$('#main').contents();
		var $frm=$cnt.find("form[name='MSB']");
		var $btnViewResOnly=$cnt.find(":input[value='View Res Only']");
		if(!$btnViewResOnly[0]) {
			var pfx=$frm.find(":input[name='pfx']").val();
			var awb=$frm.find(":input[name='awb']").val();
			var awbno=pfx+awb;
			var data=self.getAWB(awbno);
			if(data!=null) {
				self.LoopIndex=0;
				self.loadValues(data);
			}
		}
	}
	this.login=function() {
		if(self.JSON.reservation) {
			var $frm=$("form[name='signonform']");
			$(":input[name='UserId']",$frm).val(self.JSON.reservation.user_id);
			$(":input[name='Dept']",$frm).val(self.JSON.reservation.department);
			$(":input[name='PassWd']",$frm).val(self.JSON.reservation.password);
			$(":input[name='Signon']",$frm).click();
		}
	}
	this.checkSessionTimeout=function() {
		var isSessionTimeOut=false;
		$('font').each(function() {
			var html=$(this).html();
			////console.log('this2=',this);
			////console.log('html2=',html);
			if(html.indexOf('Your session has timed out')>-1) {
				////console.log(self);
				//console.log('session time out');
				isSessionTimeOut=true;
			}
		});
		if(isSessionTimeOut==true) {
			location.href='https://webportal.champ.aero/wcs/default.asp?';
		}
		return isSessionTimeOut;
	}
	this.formFill=function() {
		createAWBBOX(self);
	}
	this.LoopIndex=0;
	this.loadValues=function(data) {
		var $cnt=$('#main').contents();
		var $frm=$cnt.find("form[name='MSB']");
		var $btnViewResOnly=$cnt.find(":input[value='View Res Only']");
		var awbno=data.awb_no;
		var $gc_awb_box=$("#gc_awb_box");
		var $tbl=$("#gc_awb_list",$gc_awb_box);
		var $tdStatus=$("tr[awb_no='"+awbno+"'] > #tdStatus",$tbl);
		var $btnLoad=$("tr[awb_no='"+awbno+"'] #btnLoad",$tbl);
		$tdStatus.html('Submit...');
		//console.log('loadValues $btnViewResOnly=',$btnViewResOnly[0]);
		var pfx=data.awb_no.substring(0,3);
		var awb=data.awb_no.substring(3,data.awb_no);
		//console.log('pfx=',pfx,'awb=',awb);
		if($btnViewResOnly[0]) {
			//self.openBookingMenu();
			//self.redirectBookingScreen();
			$cnt.find(":input[name='pfx']").val(pfx);
			$cnt.find(":input[name='awb']").val(awb);
			var code="var elements = document.getElementsByName('MSB')[0].elements;";
			code+="var x = elements.length;"; // //console.log('elements=',elements,',x=',x);";
			code+="var i;var ele = null;";
			code+="for(i=0;i<x;i++){"
			code+=" if(elements[i].type=='submit' && elements[i].value=='Submit' && (elements[i].name=='' || elements[i].name==undefined)){";
			code+=" ele=elements[i];"
			code+=" }";
			code+=" if(ele!=null){ ele.click(); }"
			code+="}"
			var html='<button type="button" id="gc_btn_routing_change" onclick="'+code+'" style="position:absolute;top:10px;">Routing Change</button>';
			$cnt.find('body').append(html);
			$cnt.find('#gc_btn_routing_change').click();
			$cnt.find('#gc_btn_routing_change').remove();
		} else {
			if($frm.find(":input[name='pfx']").val()==pfx&&$frm.find(":input[name='awb']").val()==awb) {
				$frm.find(":input[name='tpcs']").val(data.total_pieces);
				$frm.find(":input[name='twgt']").val(data.grwt);
				$frm.find(":input[name='tchgwgt']").val(data.chwt);
				$frm.find(":input[name='desc']").val(data.commodity);

				if(data.shipment_type=='PP') {
					$frm.find(":input[name='chgcd']").value='PX    ';
				}
				if(data.shipment_type=='CC') {
					$frm.find(":input[name='chgcd']")[0].value='CX    ';
				}
				$frm.find(":input[name='ratecl']")[0].value='G  ';

				if(data.agent_code)
					$frm.find(":input[name='agtaccnbr']").val(data.agent_code);
				else
					$frm.find(":input[name='agtaccnbr']").val(data.agent_iata_code);

				if(data.setting_agent_name)
					$frm.find(":input[name='agtname']").val(data.setting_agent_name);
				else
					$frm.find(":input[name='agtname']").val(data.agent_name);

				$frm.find(":input[name='agtcity']").val(data.agent_airport_city);
				//console.log('data.flight_details=',data.flight_details);
				if(data.flight_details) {
					var j=0;
					var originName=''
					var destName='';
					var airlineName='';
					var $txt=null;
					for(j=0;j<data.flight_details.length;j++) {
						originName='';
						destName='';
						switch(j) {
							case 0:
								originName='org';
								destName='to1';
								airlineName='by1';
								break;
							case 1:
								destName='to2';
								airlineName='by2';
								break;
							case 2:
								destName='to3';
								airlineName='by3';
								break;
							case 3:
								destName='to4';
								airlineName='by4';
								break;
							case 4:
								destName='to5';
								airlineName='by5';
								break;
						}
						if(originName!='') {
							$txt=$frm.find(":input[name='"+originName+"']");
							$txt.val(data.flight_details[j].origin_code);
						}
						if(destName!='') {
							$txt=$frm.find(":input[name='"+destName+"']");
							$txt.val(data.flight_details[j].dest_code);
						}
						if(airlineName!='') {
							$txt=$frm.find(":input[name='"+airlineName+"']");
							$txt.val(data.flight_details[j].airline_code);
						}
					}
				}
				//console.log('data.participant_details=',data.participant_details);
				if(data.participant_details) {
					var j;
					for(j=0;j<data.participant_details.length;j++) {
						var row=data.participant_details[j];
						if(row.participants=='SH') {
							$frm.find(":input[name='shpname']").val(row.name);
							$frm.find(":input[name='shpadr']").val(cString(row.address1)+(cString(row.address2)!=''?','+cString(row.address2):''));
							$frm.find(":input[name='shpcity']").val(row.city);
							$frm.find(":input[name='shpcntry']").val(row.country);
						} else if(row.participants=='CO') {
							$frm.find(":input[name='conname']").val(row.name);
							$frm.find(":input[name='conadr']").val(cString(row.address1)+(cString(row.address2)!=''?','+cString(row.address2):''));
							$frm.find(":input[name='concity']").val(row.city);
							$frm.find(":input[name='concntry']").val(row.country);
						}
					}
				}
				if(data.flight_details) {
					var j=0;
					for(j=0;j<data.flight_details.length;j++) {
						var originName='';
						var destName='';
						var flightNo='';
						var pieces='';
						var grwt='';
						var chwt='';
						var $txt;
						originName='fonl'+(j+1);
						destName='fofl'+(j+1);
						flightNo='flight'+(j+1);
						pieces='fpcs'+(j+1);
						grwt='fwgt'+(j+1);
						chwt='fcwgt'+(j+1);
						if(originName!='') {
							$txt=$frm.find(":input[name='"+originName+"']");
							$txt.val(data.flight_details[j].origin_code);
						}
						if(destName!='') {
							$txt=$frm.find(":input[name='"+destName+"']");
							$txt.val(data.flight_details[j].dest_code);
						}
						if(flightNo!='') {
							$txt=$frm.find(":input[name='"+flightNo+"']");
							$txt.val(data.flight_details[j].flight_no+'/'+(new Date(data.flight_details[j].flight_date)).formatDate('ddMMM').toUpperCase());
						}
						if(pieces!='') {
							$txt=$frm.find(":input[name='"+pieces+"']");
							$txt.val(data.flight_details[j].pieces);
						}
						if(grwt!='') {
							$txt=$frm.find(":input[name='"+grwt+"']");
							$txt.val(data.flight_details[j].grwt);
						}
						if(chwt!='') {
							$txt=$frm.find(":input[name='"+chwt+"']");
							$txt.val(data.flight_details[j].chwt);
						}
					}
				}
				$tdStatus.html('Submitted');
				$btnLoad.css({
					'background-color': 'red','color': 'white'
				}).html('Resubmit');
				self.passOpenerTab(data);
			} else {
				self.LoopIndex=self.LoopIndex+1;
				if(self.LoopIndex<=3) {
					self.openBookingMenu();
					self.redirectBookingScreen();
					setTimeout(function() {
						self.loadValues(data);
					},2000);
				}
			}
		}
	}
	this.selectValueDDL=function(ddl,value) {
		var findOptionValue='';
		for(var i=0,len=ddl.options.length;i<len;i++) {
			opt=ddl.options[i];
			console.log('opt=',opt);
			console.log('opt text=',opt.text);
			console.log('opt value=',opt.value);
			var optValue=opt.value.trim();
			if(optValue==value) {
				findOptionValue=optValue;
			}
		}
		console.log('findOptionValue=',findOptionValue);
		if(findOptionValue!='') {
			ddl.value=findOptionValue;
		}
	}
	this.passOpenerTab=function(awbdata) {
		var code="completedAWBNO('"+awbdata.company_id+"','"+awbdata.awb_no+"','"+awbdata.awb_index+"','"+self.RESERVATIONID+"');";
		chrome.runtime.sendMessage({ cmd: 'execute_code','code': code,'tabid': self.OPENERID });
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
}
