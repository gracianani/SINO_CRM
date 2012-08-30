
<%@ Page Title="ReportValue" Language="C#" MasterPageFile="~/MarketingMgr/MarketingMgrMasterPage.master"
    AutoEventWireup="true" CodeFile="MarketingMgrReportValue.aspx.cs" Inherits="MarketingMgr_ReportValue" EnableSessionState="ReadOnly" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
<link href="../../default/forms.css" type="text/css" rel="stylesheet" />
<link href="../../default/styles.css" type="text/css" rel="stylesheet" />
<link href="../../default/newadd.css" type="text/css" rel="stylesheet" />
<script type="text/javascript" src="../js/action.js"></script>
<script type="text/javascript" src="../js/drag2select.js"></script>
<script src="../js/jquery-1.4.2.js"></script>
<script src="../js/jquery.ui.core.js"></script>
<script src="../js/jquery.ui.widget.js"></script>
<script src="../js/jquery.ui.mouse.js"></script>
<script src="../js/jquery.ui.draggable.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
<table>


    <tr>
    <td style="width: 10px" align="left" valign="top"></td>
    <td align="left" valign="top" colspan="4">
        <fieldset class="fie">
            <legend><strong>Report Overview</strong></legend>
            <table>
            <tr>
                <td style="width: 10px" align="left" valign="top"></td>
                <td align="right"><asp:Label ID="lblName" runat="server" Text="Name:"></asp:Label></td>
                <td><asp:TextBox ID="txtName" runat="server"></asp:TextBox></td>
                <td align="right"><asp:Label ID="lblViewname" runat="server" Text="ViewName:"></asp:Label></td>
                <td><asp:DropDownList ID="ddlViewName" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlViewName_SelectedIndexChanged">
                </asp:DropDownList>
                </td>
              </tr>
               <tr>
                <td style="width: 10px" align="left" valign="top"></td>
                <td align="right"><asp:Label ID="lblDepiction" runat="server" Text="Description:"></asp:Label></td>
                <td colspan="3"><asp:TextBox ID="txtDepiction" runat="server" Width="438px"></asp:TextBox></td>
                </tr>
            </table>
                <table width="100%" border="0" cellspacing="0" cellpadding="4" class="c_tab">
                <tr>
                    <th width="350" align="center">Fields</th>
                    <th style="width:90px" align="center">&nbsp;</th>
                    <th width="350" align="center">Fields Selected</th>
                </tr>
                <tr>
                    <td colspan="3" align="right"><asp:ImageButton ID="ImageButton1" runat="server" width="12" height="10" ImageUrl="../images/nkk_03.png" OnClientClick="getselectedString();" OnClick="btnSubmit_Click"/> <asp:ImageButton ID="ImageButton4" runat="server" width="10" height="9" ImageUrl="../images/tablist2_03.png"  OnClick="btnCancel_Click"/></td>
                </tr>
                <tr>
                    <td colspan="3">
                    <div id="outbox" class="bgg" style="position: relative;width:100%;"></div> 
                    </td>
                </tr>
                <tr>
                    <td colspan="3" align="right"><asp:ImageButton ID="ImageButton2" runat="server" width="12" height="10" ImageUrl="../images/nkk_03.png" OnClientClick="getselectedString();" OnClick="btnSubmit_Click"/> <asp:ImageButton ID="ImageButton3" runat="server" width="10" height="9" ImageUrl="../images/tablist2_03.png"  OnClick="btnCancel_Click"/></td>  
                </tr>
                </table>
          </fieldset>
    </td>
    </tr>
</table>
    <asp:HiddenField ID="hidRoleId" runat="server" />
    <asp:HiddenField ID="hidReportId" runat="server" />
    <asp:HiddenField ID="hidUserId" runat="server" />
    <asp:HiddenField ID="hidLeft" runat="server" />
    <asp:HiddenField ID="hidRight" runat="server" />
    <asp:HiddenField ID="hidValue" runat="server" />
    <asp:HiddenField ID="hidViewName" runat="server" />
    <asp:Label ID="label_add" runat="server" ForeColor="Red" Text=""></asp:Label>
<script type="text/javascript">
//读取数据	---------------
if (document.getElementById("ctl00_ContentPlaceHolder1_hidLeft").value.length>0)
{
var clumArray=document.getElementById("ctl00_ContentPlaceHolder1_hidLeft").value.split('|');//左侧数组
}
else
{var clumArray=new Array();}

if  (document.getElementById("ctl00_ContentPlaceHolder1_hidRight").value.length>0)
{var selectArray=document.getElementById("ctl00_ContentPlaceHolder1_hidRight").value.split('|');//右侧数组
}
else
{var selectArray=new Array();}
//读取数据end	---------------
var dobj=new dragSelectObject("dobj","outbox",780,100,24,clumArray,selectArray);
dobj.drawUI();//设置位置


//获取值方法
function getselectedString(){
	var values=dobj.getSelected();
	document.getElementById("ctl00_ContentPlaceHolder1_hidRight").value=values;
	var valuesLeft=dobj.getnoSelected();
	document.getElementById("ctl00_ContentPlaceHolder1_hidLeft").value=valuesLeft;
}
function setsel(type){
	if(type){
		dobj.setWidth(960);
	}else{
		dobj.setWidth(780);
	}

}
</script> 
</asp:Content>
