//OBJECT
function dragitem(name,px,py,va,cont,qune){
	this.name=name;
	this.px=px;
	this.py=py;
	this.va=va;
	this.cont=cont;
	this.qune=qune;
}

function setitems(c_array,s_array){
	var rarrar=new Array();
	var id=0;
	for(var i=0;i<c_array.length;i++){
		name="dragitem_"+id;
		rarrar[id]=new dragitem(name,0,0,"forSelsect",c_array[i],id);
		id++;
	}
	for(var j=0;j<s_array.length;j++){
		name="dragitem_"+id;
		rarrar[id]=new dragitem(name,0,0,"selected",s_array[j],id);
		id++;
	}
	return rarrar;
}

function dragSelectObject(objname,outid,outboxW,pos,itemH,cArray,sArray){
	this.objname=objname;
	var out=document.getElementById(outid);
	this.outbox_w=out.clientWidth;	
	if(this.outbox_w==0){this.outbox_w=outboxW;}	
	this.outbox_h=0;
	this.outboxID=outid;
	this.s_w=Math.floor((this.outbox_w-pos)/2);//item width
	this.s_h=itemH;//item height
	this.s_h_w=pos;//space	
	this.displace=10;//close size	
	this.pageArray=setitems(cArray,sArray);	

	this.drawUI=function (){
		var obj=this;	
		bigArray=obj.pageArray;		
		var str="";
		var l_top=0;
		var r_top=0;
		for(var i=0;i<bigArray.length;i++){
			if(bigArray[i].va=="forSelsect"){
				bigArray[i].px=0;
				bigArray[i].py=l_top;
				l_top+=obj.s_h;		
			}else{
				bigArray[i].px=obj.s_w+obj.s_h_w;	
				bigArray[i].py=r_top;
				r_top+=obj.s_h;	
			}		
			str+="<div class=\"lis\" id=\""+bigArray[i].name+"\" onMouseUP=\""+obj.objname+".drop(this.id);\" onMouseDown=\""+obj.objname+".setindex(this.id);\" style=\"width:"+obj.s_w+"px;top:"+bigArray[i].py+"px;left:"+bigArray[i].px+"px;\" >"+bigArray[i].cont+"</div>";
			
		}
		
		if(l_top>r_top){
			obj.outbox_h=l_top;			
		}else{
			obj.outbox_h=r_top;
		}
		var box=document.getElementById(obj.outboxID);
		box.style.height=obj.outbox_h+"px";
		box.style.width=obj.outbox_w+"px";
		box.innerHTML=str;		
		obj.installdrag(bigArray);//set drag	
		obj.setWidth();	
	}
	
	this.setWidth=function(_w){

		var obj=this;
		var out=document.getElementById(obj.outboxID);
		if(!_w){
			obj.outbox_w=out.clientWidth;
		}else{
			obj.outbox_w=_w;	
		}
		if(obj.outbox_w==0){obj.outbox_w=780;}
		out.style.width=obj.outbox_w+"px";
		obj.s_w=Math.floor((obj.outbox_w-obj.s_h_w)/2);//item width
		var bigArray=obj.pageArray;
		for(var i=0;i<bigArray.length;i++){
			var li=document.getElementById(bigArray[i].name);			
			if(li){	
				if(bigArray[i].va=="forSelsect"){
					bigArray[i].px=0;
				}else{
					bigArray[i].px=obj.s_w+obj.s_h_w;	
				}

				li.style.width=obj.s_w+"px";					
				li.style.left=bigArray[i].px+"px";
			}
		}	
	}
	
	this.installdrag=function (){
		var obj=this;	
		var bigArray=obj.pageArray;
		for(var i=0;i<bigArray.length;i++){
			id=bigArray[i].name;
			//============================
			$( "#"+id ).draggable();			
			//============================
		}	
	}
	
	this.setindex=function(id){
		var obj=this;	
		var bigArray=obj.pageArray;
		var maxindex=0;
		// add by zy 20101230 start
		var sort = 1;
		// add by zy 20101230 end
		for(var j=0;j<bigArray.length;j++){
			if(bigArray[j].name!=id){			
				var li=document.getElementById(bigArray[j].name);
				if(li){	
				    // update by zy 20101230 start 	
					//li.style.zIndex=0;
					li.style.zIndex=sort;
					sort++;
					// update by zy 20101230 end
				}
			}
		}
		var toobj=document.getElementById(id);
		toobj.style.zIndex=(sort+1);
		//alert(toobj.style.zIndex);
	}
	
	this.reloadUI=function (){
		var obj=this;	
		var bigArray=obj.pageArray;
		var l_top=0;
		var r_top=0;
		for(var i=0;i<bigArray.length;i++){
			var qid=0;
			for(var j=0;j<bigArray.length;j++){
				//sort----
				if(bigArray[j].qune==i){
					qid=j;
					break;
				}
			}
			//pos
			if(bigArray[qid].va=="forSelsect"){
				bigArray[qid].px=0;
				bigArray[qid].py=l_top;
				l_top+=obj.s_h;		
			}else{
				bigArray[qid].px=obj.s_w+obj.s_h_w;	
				bigArray[qid].py=r_top;
				r_top+=obj.s_h;	
			}			
			
			if(l_top>r_top){
				obj.outbox_h=l_top;			
			}else{
				obj.outbox_h=r_top;
			}
			var box=document.getElementById(obj.outboxID);
			box.style.height=obj.outbox_h+"px";
			
			var itemobj=document.getElementById(bigArray[qid].name);
			if(itemobj){
				itemobj.style.top=bigArray[qid].py+"px";
				itemobj.style.left=bigArray[qid].px+"px";
			}
		}
	}
	//down
	this.drop=function(id){
	
		var obj=this;
		var bigArray=obj.pageArray;
		var dragitem=document.getElementById(id);
		var sta="";
		var i_left=dragitem.offsetLeft;
		var i_top=dragitem.offsetTop;
	
		if(i_top<-obj.s_h-20 || i_top>obj.outbox_h+20){//out of range
			obj.reloadUI();
			return;	
		}
		var iname="";
		for(var i=0;i<bigArray.length;i++){
			if(bigArray[i].name==id){
				sta=bigArray[i].va;								
			}
		}					
		//get value	
		if(sta=="forSelsect" && (i_left>(obj.s_h_w+obj.displace) && i_left<(obj.s_w+obj.s_h_w+obj.s_w))){
			sta="selected";								
		}else if(sta=="selected" && (i_left>-obj.s_w && i_left<(obj.s_w-obj.displace))){
			sta="forSelsect";
		}
		//change Array
		var nowID=null;
		for(var i=0;i<bigArray.length;i++){
			if(bigArray[i].name==id){
				bigArray[i].va=sta;
				nowID=i;
				break;
			}
		}
		var toID=null;
		for(var i=0;i<bigArray.length;i++){	
			if(bigArray[i].va==sta && i!=nowID){
				if(bigArray[i].py<i_top && (bigArray[i].py+obj.s_h)>=i_top){
					toID=i;
					break;
				}			
			}			
		}
		var nowqune=bigArray[nowID].qune;		
		
		if(toID){	
			var toqun=bigArray[toID].qune;
			var up=false;
			if(nowqune>toqun){
				up=true;
			}	
			var toup=false;	
			for(var i=0;i<bigArray.length;i++){
				if(i!=nowID){					
					if(bigArray[i].qune>nowqune){					
						bigArray[i].qune--;	
					}											
					if(bigArray[i].qune>toqun){
						bigArray[i].qune++;		
					}else if((bigArray[i].qune==toqun)){
						if(!up){
							bigArray[i].qune++;	
						}
							
					}	
				}						
			}			
			bigArray[nowID].qune=toqun;	
			if(up){
				bigArray[nowID].qune++;
			}					
		}else{
			if(i_top>=(obj.outbox_h)/2){
				for(var i=0;i<bigArray.length;i++){					
					if(bigArray[i].qune>nowqune){					
						bigArray[i].qune--;	
					}			
				}
				bigArray[nowID].qune=bigArray.length-1;//to last
			}else{
				for(var i=0;i<bigArray.length;i++){					
					if(bigArray[i].qune<nowqune){					
						bigArray[i].qune++;	
					}			
				}
				bigArray[nowID].qune=0;//to first
			}
		}
		obj.reloadUI();
	}

	//return values
	this.getSelected=function(){
		var obj=this;
		var bigArray=obj.pageArray;
		var str="";
		// add by zy 20101230 start
		var preSortArray = new Array();
		var iIndex = 0;
		// add by zy 20101230 end
		for(var i=0;i<bigArray.length;i++){
			if(bigArray[i].va=="selected"){
			    // update by zy 20101230 start
				//str+=(bigArray[i].cont+"|");
				preSortArray[iIndex] = bigArray[i];
				iIndex++;
				// update by zy 20101230 end					
			}
		}
		
		// add by zy 20101230 start		
        preSortArray.sort(sortFun);
        for(var i=0;i<preSortArray.length;i++){
            str += (preSortArray[i]["cont"]+"|");
        }
        // add by zy 20101230 end
		if(str!==""){
			str=str.substring(0,str.length-1)
		}
		return(str);
	}
	
	//return values
	this.getnoSelected=function(){
		var obj=this;
		var bigArray=obj.pageArray;
		var str="";
		for(var i=0;i<bigArray.length;i++){
			if(bigArray[i].va=="forSelsect"){
				str+=(bigArray[i].cont+"|");					
			}
		}
		if(str!==""){
			str=str.substring(0,str.length-1)
		}
		return(str);
	}
}
// add by zy 20101230 start	
function sortFun(x,y)
{
    return x.qune - y.qune;
}
// add by zy 20101230 end