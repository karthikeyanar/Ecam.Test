var $gc_ext=$('#gc_ext');
var $gc_ext_btn_container=$('#gc_ext_btn_container');
var $gc_ext_btn_name=$('#gc_ext_btn_name');
var $gc_ext_btn_css=$('#gc_ext_btn_css');
var $gcb=$('#gcb');
if(!$gcb[0]&&$gc_ext.val()=='true'&&$gc_ext_btn_container.val()!='') {
	var selector=$gc_ext_btn_container.val();
	var $cnt=$(selector);
	$cnt.removeClass('hide');
	var $btn=$("<button id='gcb' class='"+$gc_ext_btn_css.val()+"'>"+$gc_ext_btn_name.val()+"</button>");
	$cnt.append($btn);
	$btn.click(function() {
		console.log($('#gc_btn_callback')[0]);
		$('#gc_btn_callback').click();
		//var json=$('#gc_ext_callback_json').val();
		//chrome.runtime.sendMessage({ cmd: 'values',data: json });
	});
}
function completedAWBNO(companyId,awbNo,awbIndex,reservationId) {
	//alert('completedAWBNO companyId='+companyId+',awbNo='+awbNo+',awbIndex='+awbIndex+',reservationId='+reservationId);
	$('#gc_btn_save_awb_submit').attr('company_id',companyId).attr('awb_no',awbNo).attr('awb_index',awbIndex).attr('reservation_id',reservationId);
	$('#gc_btn_save_awb_submit').click();
} 