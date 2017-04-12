(function($) {
    // Shortuct functions
    jAlert=function(options) {
        if($.type(options)=="string") {
            options={ "message": options };
        }
        if(options==null) {
            return;
        }
        var isNotify=false;
        if(options.isNotify) {
            isNotify=options.isNotify;
        }
        var modalid="jAlert-modal";
        $("#"+modalid).modal('hide').remove();
        var width=cInt(options.width);
        if(width<=0) width=800;
        var template="<div id='"+modalid+"' class='modal modal-top"+(isNotify==true?" is-notify-modal":"")+"' data-width='"+width+"' style='display: none;'>";
        template+="<div class='modal-body'>";
        template+="<p><h4>"+options.message+"</h4></p>";
        template+="</div>";
        if(isNotify==false) {
            template+="<div class='modal-footer'>";
            template+="<div class='pull-right'>";
            template+="<button type='button' class='btn btn-sm btn-primary' data-dismiss='modal'>OK</button>";
            template+="</div><div class='clearfix'></div>";
        }
        template+="</div>";
        template+="</div>";
        var $modal=$(template);
        $(".btn",$modal).on('click',function() {
            if(options.ok)
                options.ok();
        });
        $modal.on('shown.bs.modal',function(e) {
            $(".btn",$modal).focus();
            if(options.show)
                options.show();
        });
        $modal.on('hide.bs.modal',function(e) {
            if(options.hide)
                options.hide();
        });
        $modal.on('hidden.bs.modal',function(e) {
            $("#"+modalid).remove();
        });
        $('body').append($modal);
        $modal.modal({
            keyboard: false
        });
        $modal.modal('show');
    }
    jCloseAlert=function() {
        $('#jAlert-modal').modal('hide').remove();
    }
    jConfirm=function(options) {
        var modalid="jConfirm-modal";
        $("#"+modalid).remove();
        if(cString(options.message)=="") {
            options.message="Are you sure?";
        }
        var width=cInt(options.width);
        if(width<=0) width=600;
        var template="<div id='"+modalid+"' class='modal modal-top' data-width='"+width+"' style='display: none;'>";
        template+="<div class='modal-body'>";
        template+="<p><h4>"+options.message+"</h4></p>";
        template+="</div>";
        template+="<div class='modal-footer'>";
        template+="<div class='pull-right'>";
        template+="<button type='button' id='ok' class='btn btn-sm btn-primary'>OK</button>";
        template+="<button type='button' id='cancel' class='btn btn-sm btn-danger m-l-15' data-dismiss='modal'>Cancel</button>";
        template+="</div><div class='clearfix'></div>";
        template+="</div>";
        template+="</div>";
        var $modal=$(template);
        $("#ok",$modal).on('click',function() {
            if(options.ok)
                options.ok();
            $modal.modal('hide');
        });
        $("#cancel",$modal).on('click',function() {
            if(options.cancel)
                options.cancel();
        });
        $modal.on('shown.bs.modal',function(e) {
            $("#ok",$modal).focus();
            if(options.show)
                options.show();
        });
        $modal.on('hide.bs.modal',function(e) {
            if(options.hide)
                options.hide();
        });
        $modal.on('hidden.bs.modal',function(e) {
            $("#"+modalid).remove();
        });
        $('body').append($modal);
        $modal.modal({
            keyboard: false
        });
        $modal.modal('show');
    };
})(jQuery);