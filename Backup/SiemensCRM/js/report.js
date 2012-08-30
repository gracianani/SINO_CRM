	
// add by zy 20110107	
var init;
function drawmask(){
init = 1;
var str="<div id=\"maskdiv\" class=\"hidden\"><iframe id=\"maskbk\" frameborder=\"0\"></iframe></div>";
str+="<div id=\"maskpopout\" class=\"hidden\"><div id=\"masktitle\" onclick=\"hidemask();\"></div><div id=\"maskcont\"></div></div>";
document.write(str);
}
var pop_w=600;
var pop_top=3;
function topopmask(url,w,h,title,textid,fieldName)
{
   var ctextid= document.getElementById("ctl00_ContentPlaceHolder1_clicktextid");
   if(ctextid !=null )
   {
     ctextid.value=textid;
   }
   
   var valueName= document.getElementById(textid);
   if(valueName !=null )
   {
     url+="?valueName=" + valueName.value +"&fieldName=" + fieldName;
   }
   
  
   popmask(url,w,h,title,fieldName);
}

function popmask(url,w,h,title,fieldName){	
	pop_w=w
	setmaskpos(w,h);
	showmask();
	setpopcont(url,w,h)
	var out=document.getElementById("maskpopout");
	var t=document.getElementById("masktitle");
	t.innerHTML=title;
	out.style.width=w+"px";
	
}
function setpopcont(url,w,h){
	var cont="<iframe frameborder=\"0\" width=\""+w+"\"  height=\""+h+"\" id=\"popwindow\" name=\"popwindow\" scrolling=\"auto\" ></iframe>";
	var outdiv=document.getElementById("maskcont");
	outdiv.innerHTML=cont;
	var cont=document.getElementById("popwindow");
	cont.src=url;
	//var text = document.getElementById("sendto");
   // var text = parent.parent.document.getElementById("sendto");
}
function setmaskpos(){
	var win_w=document.body.clientWidth;
	var win_h=document.body.clientHeight;
	var outdiv=document.getElementById("maskdiv");
	var frm=document.getElementById("maskbk");
	outdiv.style.height=(win_h)+"px";
	frm.style.height=(win_h)+"px";
	outdiv.style.width=(win_w)+"px";
	frm.style.width=(win_w)+"px";
	maskgetpos("maskpopout");	
}
//window.onscroll= function(){
//    var win_w=document.body.clientWidth;
//	var win_h=document.body.clientHeight+document.documentElement.scrollTop;
//	var outdiv=document.getElementById("maskdiv");
//	var frm=document.getElementById("maskbk");
//	outdiv.style.height=(win_h)+"px";
//	frm.style.height=(win_h)+"px";
//	outdiv.style.width=(win_w)+"px";
//	frm.style.width=(win_w)+"px";
//	maskgetpos("maskpopout");	
//}

//div position
NS6 = (document.getElementById&&!document.all)
IE = (document.all)
NS = (navigator.appName=="Netscape" && navigator.appVersion.charAt(0)=="4")
function maskgetpos(divid){ 
	if (NS||NS6){
		winY = window.pageYOffset+pop_top;
	}
	if (IE){
		winY = truebody().scrollTop+pop_top;
	}
	var obj=document.getElementById(divid);
	obj.style.top=winY+"px"	
	var win_w=document.body.clientWidth;
	obj.style.left=Math.floor((win_w-pop_w)/2)+"px";
}
function truebody(){
	return (document.compatMode!="BackCompat")? document.documentElement : document.body
}
window.onresize = function(){
	setTimeout(setmaskpos,50);
}
function tohidemask(texts)
{
//alert("tohidemask texts =" + texts);
  var cid= document.getElementById("ctl00_ContentPlaceHolder1_clicktextid").value;
//alert("tohidemask cid =" + cid);
  var ctext = document.getElementById(cid);
  if(ctext!=null)
  {
     ctext.value=texts;
     if(ctext.onchange)
     {
       ctext.onchange();
     }
  }
  hidemask();
}

function hidemask(){
	var outdiv=document.getElementById("maskdiv");
	outdiv.style.display="none";
	var popdiv=document.getElementById("maskpopout");
	popdiv.style.display="none";
	var outdiv=document.getElementById("maskcont");
	outdiv.innerHTML="";
	
}
function showmask(){
	var outdiv=document.getElementById("maskdiv");
	outdiv.style.display="block";
	var popdiv=document.getElementById("maskpopout");
	popdiv.style.display="block";
}