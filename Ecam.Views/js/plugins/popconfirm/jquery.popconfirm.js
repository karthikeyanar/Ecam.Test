+function($) {
	'use strict';

	// DROPDOWN CLASS DEFINITION
	// =========================

	var PopoverConfirm=function(element,options) {
		this.init(element,options);
	}

	PopoverConfirm.prototype.init=function(element,options) {
		var self=this;
		this.options=$.extend({},PopoverConfirm.DEFAULTS,options)

		this.$element=$(element);
		this.$element.addClass("popover-confirm");
		var template="<div> \
				<p class='bs-popover-msg'>"+self.options.message+"</p> \
				<div> \
					<button type='button' id='ok' class='"+self.options.okBtnClass+"' aria-hidden='true'>"+self.options.okBtnText+"</button> \
					<button type='button' id='cancel' class='"+self.options.cancelBtnClass+"' aria-hidden='true'>"+self.options.cancelBtnText+"</button> \
				</div> \
			</div>";

		this.$element.on('click.bs.popoverconfirm',function() {
			$('.popover-confirm').not(this).popover('hide'); //all but this
		})

		this.popover=this.$element.popover({
			"animation": self.options.animation,
			"html": self.options.html,
			"placement": self.options.placement,
			"trigger": self.options.trigger,
			"title": self.options.title,
			"content": template,
			"delay": self.options.delay,
			"container": self.options.container
		})
		.on('shown.bs.popover',function() {
			var $tip=$(self.$element.data('bs.popover').tip()[0]);
			$('#ok',$tip).unbind("click").click(function() {
				self.$element.popover('hide');
				if(self.options.onConfirm)
					self.options.onConfirm();
			});
			$('#cancel',$tip).unbind("click").click(function() {
				self.$element.popover('hide');
				if(self.options.onCancel)
					self.options.onCancel();
			});
		});
	}

	PopoverConfirm.DEFAULTS={
		message: 'Are you sure?',
		okBtnClass: 'btn btn-primary input-60',
		okBtnText: 'OK',
		cancelBtnClass: 'btn btn-sm btn-danger input-60 m-l-10',
		cancelBtnText: 'Cancel',
		modalClass: '',
		placement: 'bottom',
		trigger: 'click',
		animation: true,
		html: true,
		title: '',
		delay: 0,
		container: 'body',
		onConfirm: null,
		onCancel: null
	}


	var old=$.fn.popoverconfirm

	$.fn.popoverconfirm=function(option) {
		return this.each(function() {
			var $this=$(this)
			var data=$this.data('bs.popoverconfirm')
			var options=typeof option=='object'&&option

			if(!data) $this.data('bs.popoverconfirm',(data=new PopoverConfirm(this,options)))
			if(typeof option=='string') data[option].call($this)
		})
	}

	$.fn.popoverconfirm.Constructor=PopoverConfirm


	$.fn.popoverconfirm.noConflict=function() {
		$.fn.popoverconfirm=old
		return this
	}


} (jQuery);
