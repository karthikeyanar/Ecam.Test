var $gcb_cmd=$('#gcb_cmd');
if(!$gcb_cmd[0]) {
	$gcb_cmd=$("<button id='gcb_cmd' class='hide'>Google Chrome Ext Command</button>");
	$("body").append($gcb_cmd);
}
$gcb_cmd.unbind('click').click(function() {
	var cmd=$(this).attr('cmd');
	var $gc_ext_callback_json=$('#gc_ext_callback_json');
	var data=$gc_ext_callback_json.val();
	chrome.runtime.sendMessage({ cmd: cmd,data: data });
});