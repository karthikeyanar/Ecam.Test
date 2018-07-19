var $gcb_cmd = $('#gcb_cmd');
if(!$gcb_cmd[0]) {
	$gcb_cmd=$("<button id='gcb_cmd' class='hide'>Google Crome Ext Init</button>");
	$("body").append($gcb_cmd);
}
$gcb_cmd.unbind('click').click(function() {
	var cmd=$(this).attr('cmd');
	chrome.runtime.sendMessage({ cmd: cmd,data: null });
});