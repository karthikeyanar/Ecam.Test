"use strict";
function strHelper(s) {
    this.orig=s;
    if(s!==null&&s!==undefined) {
        if(typeof s==='string')
            this.s=s;
        else
            this.s=s.toString();
    } else {
        this.s=''; //null or undefined
    }
}
var __nsp=String.prototype;
var _sp=strHelper.prototype={
    replaceAll: function(ss,r) {
        var s=this.s.split(ss).join(r)
        return new this.constructor(s);
    },
    getNumbers: function() {
        var rx=/[+-]?((\.\d+)|(\d+(\.\d+)?)([eE][+-]?\d+)?)/g,
        mapN=this.s.match(rx)||[];
        return mapN.map(Number);
    },
    isAlpha: function() {
        return !/[^a-z\xDF-\xFF]|^$/.test(this.s.toLowerCase());
    },
    isAlphaNumeric: function() {
        return !/[^0-9a-z\xDF-\xFF]/.test(this.s.toLowerCase());
    },
    isNumeric: function() {
        return !/[^0-9]/.test(this.s);
    },
    isInt: function() {
        return /^-?\d+$/.test(this.s);
    },
    isFloat: function() {
        return /^-?\d+(?:[.,]\d*?)?$/.test(this.s);
    },
    removeNonNumeric: function() {
        return this.s.replace(/[^0-9-.]/g,'');
    },
    left: function(N) {
        if(N>=0) {
            var s=this.s.substr(0,N);
            return new this.constructor(s);
        } else {
            return this.right(-N);
        }
    },
    right: function(N) {
        if(N>=0) {
            var s=this.s.substr(this.s.length-N,N);
            return new this.constructor(s);
        } else {
            return this.left(-N);
        }
    },
    toInt: function() {
        var result=parseInt(this.s);
        if(/^\s*-?0x/i.test(this.s))
            result=parseInt(this.s,16);
        if(isNaN(result))
            this.s=this.replaceAll('(','-').replaceAll(')','').s;
        if(isNaN(result))
            result=parseInt(this.removeNonNumeric());
        if(isNaN(result)) {
            var arr=this.getNumbers();
            if(arr.length>0)
                result=parseInt(arr[0]);
        }
        return (isNaN(result)?0:result);
    },
    toFloat: function(precision) {
        var result=parseFloat(this.s);
        if(isNaN(result))
            this.s=this.replaceAll('(','-').replaceAll(')','').s;
        if(isNaN(result))
            result=parseFloat(this.removeNonNumeric());
        if(isNaN(result)) {
            var arr=this.getNumbers();
            if(arr.length>0)
                result=parseFloat(arr[0]);
        }
        return (isNaN(result)?0:((precision)?parseFloat(result.toFixed(precision)):result));
    },
    toBoolean: function() {
        if(typeof this.orig==='string') {
            var s=this.s.toLowerCase();
            return s==='true'||s==='yes'||s==='on'||s==='1';
        } else
            return this.orig===true||this.orig===1;
    },
    trim: function() {
        var s;
        if(typeof __nsp.trim==='undefined')
            s=this.s.replace(/(^\s*|\s*$)/g,'')
        else
            s=this.s.trim()
        return new this.constructor(s);
    },

    trimLeft: function() {
        var s;
        if(__nsp.trimLeft)
            s=this.s.trimLeft();
        else
            s=this.s.replace(/(^\s*)/g,'');
        return new this.constructor(s);
    },
    trimRight: function() {
        var s;
        if(__nsp.trimRight)
            s=this.s.trimRight();
        else
            s=this.s.replace(/\s+$/,'');
        return new this.constructor(s);
    },
    between: function(left,right) {
        var s=this.s;
        var startPos=s.indexOf(left);
        var endPos=s.indexOf(right,startPos+left.length);
        if(endPos==-1&&right!=null)
            return new this.constructor('')
        else if(endPos==-1&&right==null)
            return new this.constructor(s.substring(startPos+left.length))
        else
            return new this.constructor(s.slice(startPos+left.length,endPos));
    },
    endsWith: function() {
        var suffixes=Array.prototype.slice.call(arguments,0);
        for(var i=0;i<suffixes.length;++i) {
            var l=this.s.length-suffixes[i].length;
            if(l>=0&&this.s.indexOf(suffixes[i],l)===l) return true;
        }
        return false;
    },
    startsWith: function() {
        var prefixes=Array.prototype.slice.call(arguments,0);
        for(var i=0;i<prefixes.length;++i) {
            if(this.s.lastIndexOf(prefixes[i],0)===0) return true;
        }
        return false;
    },
    removeHTMLTag: function() {
        var s=this.s.replace(/(<([^>]+)>)/ig,"")
        return new this.constructor(s);
    }
}
_sp.constructor=strHelper;
function Export(str) {
    return new strHelper(str);
};
if(typeof module!=='undefined'&&typeof module.exports!=='undefined') {
    module.exports=Export;
} else {
    if(typeof define==="function"&&define.amd) {
        define([],function() {
            return Export;
        });
    } else {
        window.SH=Export;
    }
}