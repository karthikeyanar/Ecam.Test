"use strict";
define("BlankController",["knockout","komapping","helper"],function(ko,komapping,helper) {
	return function(url,rnd) {
		var self=this;
        this.template = url;
		//this.template="/Home/Blank";
	}
}
);