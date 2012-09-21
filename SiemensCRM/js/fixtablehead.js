// JavaScript Document
function flowThTable(divid,tborder){
	this.na=divid;
	this.tdArray=new Array();
	this.head_h;
	this.headis="th";	
	this.tabborder=tborder;
	this.sctop=0;
	this.tabheight=100;
	this.timout=null;
	this.padd=$("#"+this.na+" table").attr("cellpadding");
	this.spac=$("#"+this.na+" table").attr("cellspacing");	
	var pobj=this;		
	this.getSize=function(){
		var tabhead=$("#"+pobj.na+" tr:eq(0)");
		pobj.head_h=tabhead.height();
		if(tabhead.find("th").length>0){
			tabhead.find("th").each(pobj.getTd);
			pobj.headis="th";
		}else if(tabhead.find("td").length>0){
			tabhead.find("td").each(pobj.getTd);
			pobj.headis="td";
		}
	}	
	this.getTd=function(){		
		var w=$(this).width();			
		if($.browser.mozilla){
			w-=2;
		}else if($.browser.msie){
			if($.browser.version == "6.0" || $.browser.version == "7.0"){
				if(pobj.tabborder>1){
					w+=pobj.tabborder-1;;
				}
				if(pobj.tabborder<2){
					if($(this).index()>=1){					
						w--;
					}
				}else{
					if($(this).index()==1){					
						w--;
					}
				}
			}else{
				w+=pobj.tabborder;
			}			
		}else{
			w-=2-pobj.tabborder;
		}	
		if($.browser.msie && ($.browser.version == "8.0")){
			w-=parseInt(pobj.padd)+1;
		}else{
			w+=Math.ceil(parseInt(pobj.padd)/2);
		}		
		var td=new tddata(w,$(this).html())
		pobj.tdArray.push(td);
	}
	this.creatTable = function (tabh) {
	    pobj.tabheight = tabh;
	    $("#" + pobj.na).css("overflow", "auto").css("height", pobj.tabheight + "px").css("position", "relative");
	    $("#" + pobj.na + " .flowtabhead").remove();
	    pobj.tdArray.splice(0, pobj.tdArray.length);
	    pobj.getSize();
	    var str = "<div class=\"flowtabhead\"";
	    if ($.browser.msie && ($.browser.version == "6.0" || $.browser.version == "7.0")) { //
	        //			str+=" style=\"top:expression(document.getElementById('"+pobj.na+"').scrollTop)\"";
	    }
	    str += " ></div>";

	    var hbox = $(str);

	    var tabstr = $("#" + pobj.na + " table:eq(0)").clone();
	    tabstr.find("tr:gt(0)").remove();

	    var len = pobj.tdArray.length;

	    for (var i = 0; i < len; i++) {
	        var tth = tabstr.find(pobj.headis + ":eq(" + i + ")");

	        //tth.style = "width:" + (pobj.tdArray[i].w) + "px;height:" + pobj.head_h + "px;padding:4px";
	        var tdstr = "<div style=\"width:" + (pobj.tdArray[i].w - 2) + "px;height:" + pobj.head_h + "px\">" + tth.html() + "</div>";
	
	       tth.html(tdstr);
	    }
	    hbox.append(tabstr);
	    $("#" + pobj.na).append(hbox);
	    // By DingJunjie 20110516 Item 4 Delete Start	
	    //$("#"+pobj.na).scrollTop=pobj.sctop;	
	    // By DingJunjie 20110516 Item 4 Delete End
	    // By DingJunjie 20110516 Item 4 Add Start	
	    $("#" + pobj.na)[0].scrollTop = pobj.sctop;
	    // By DingJunjie 20110516 Item 4 Add End	
	    var head = $("#" + pobj.na + " .flowtabhead");
	    if (head.length > 0) {
	        head.get(0).style.top = pobj.sctop + "px";
	    }
	    $("#" + pobj.na).scroll(pobj.scrollfollow);
	}	
	this.ieresize=function(){
		if(pobj.timout){
			clearTimeout(pobj.timout);
		}
		pobj.timout=setTimeout(function(){pobj.creatTable(pobj.tabheight)},100);		
	}
	this.scrollfollow=function(){
        nScrollTop = $(this)[0].scrollTop;
		pobj.sctop=nScrollTop;
//		if($.browser.msie && ($.browser.version == "6.0" || $.browser.version == "7.0")){
//			//null
//		}else{			
			var head=$("#"+pobj.na+" .flowtabhead");
			head.get(0).style.top=nScrollTop+"px";
//		}
	}
	$(window).bind("resize",this.ieresize);
}
function tddata(_w,_txt){
	this.w=_w;
	this.txt=_txt;
}