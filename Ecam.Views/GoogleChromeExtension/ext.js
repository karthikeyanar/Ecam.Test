var $gc_ext=$('#gc_ext');
var $gc_ext_btn_container=$('#gc_ext_btn_container');
var $gcb=$('#gcb');
if(!$gcb[0]&&$gc_ext.val()=='true'&&$gc_ext_btn_container.val()!='') {
	var selector=$gc_ext_btn_container.val();
	var $cnt=$(selector);
	$cnt.removeClass('hide');
	var $btn=$("<button id='gcb' class='btn btn-success btn-sm'>Send To Ecams</button>");
	$cnt.append($btn);
	$btn.click(function() {
		console.log($('#gc_btn_callback')[0]);
		$('#gc_btn_callback').click();
		var json=$('#gc_ext_callback_json').val();
		chrome.runtime.sendMessage({ cmd: 'values',data: json });
	});
}