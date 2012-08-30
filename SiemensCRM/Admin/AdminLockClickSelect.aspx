<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AdminLockClickSelect.aspx.cs"
    Inherits="Admin_AdminLockClickSelect" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <base target="_self" />
    <title></title>

    <script language="javascript" type="text/javascript">
    function modifySegment()
    {
        var hidden = document.getElementById('txt_keyAndValue');
        if(hidden == null )
        {
          return "";
        }
        var txtObjs = eval('('+hidden.value+')');
        
        var returnVal = "";
        
        var objs = document.getElementsByTagName('input');
        for(i=0;i<objs.length;i++)
        {
            if(objs[i].type=="checkbox" && objs[i].checked==true)
            {
                var idStringArray = objs[i].name.split("$");
                var id =idStringArray[idStringArray.length-1];
                returnVal = returnVal + (txtObjs[parseInt(id)].id+",");
            }
        }
        var paramArray = new Array();
        var lockDate = document.getElementById("tbox_date_LockSegment").value;
        returnVal = returnVal.substr(0,returnVal.length-1);
        paramArray.push(lockDate);
        paramArray.push(returnVal);
        window.returnValue = paramArray;
        window.close();
        return false;
    }
    
    
    function closeWindow()
    {
        window.close();
        return false;
    }
    </script>

    <style type="text/css">
.table_con{
border:solid 1px #ccc;
}
.table_con td{
padding:3px;
width:300px;
border:1px solid #efefef;
background:#f4f4f4;
text-align:left;
}
.table_con td input{
margin-right:3px;}
</style>
</head>
<body style="text-align: center; margin: 0px;">
    <form id="form1" runat="server">
        <table width="600px">
            <tr>
                <td>
                    <strong>
                        <asp:Label ID="lbl_selectInfo" runat="server" Text="Label" Style="font-size: medium;"></asp:Label></strong>
                </td>
            </tr>
            <tr>
                <td>
                    <strong>
                        <asp:Label ID="lbl_nothing" runat="server" Text="Label" Style="font-size: medium;"
                            ForeColor="red"></asp:Label></strong>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:CheckBoxList ID="chk_list" runat="server" RepeatColumns="3" RepeatDirection="Horizontal"
                        CellPadding="5" CellSpacing="5" CssClass="table_con">
                    </asp:CheckBoxList>
                </td>
            </tr>
            <tr runat="server" id="trDate">
                <td>
                    <table width="100%">
                        <tr>
                            <td valign="top" align="left" style="width:30%;">
                                <asp:Label ID="label1" runat="server" Text="Unlock Date:"></asp:Label></td>
                            <td valign="top" align="left" style="width:70%;">
                                <asp:TextBox ID="tbox_date_LockSegment" runat="server" Font-Bold="True" Width="160px"
                                    ReadOnly="True"></asp:TextBox>
                                <br />
                                <asp:Label ID="label2" runat="server" Text=""></asp:Label>
                                <br />
                                <asp:Calendar ID="cal_date_LockSegment" runat="server" Width="160px" FirstDayOfWeek="Sunday"
                                    OnSelectionChanged="cal_date_LockSegment_SelectionChanged">
                                    <SelectedDayStyle BackColor="#0099FF" />
                                    <WeekendDayStyle ForeColor="Red" />
                                    <TodayDayStyle BackColor="#66FFFF" />
                                    <OtherMonthDayStyle BackColor="White" ForeColor="#CCCCCC" />
                                    <DayStyle BackColor="White" />
                                    <DayHeaderStyle BackColor="#CCFFFF" />
                                    <TitleStyle BackColor="#0099FF" />
                                </asp:Calendar>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Button ID="btn_selectAll" runat="server" Text="All" Width="80px" />
                    &nbsp;&nbsp;
                    <asp:Button ID="btn_selectNone" runat="server" Text="None" Width="80px" />
                    &nbsp;&nbsp;
                    <asp:Button ID="btn_submit" runat="server" Text="Submit" Width="80px" />
                    &nbsp;&nbsp;
                    <asp:Button ID="btn_cancel" runat="server" Text="Cancel" Width="80px" />
                </td>
            </tr>
        </table>
        <input id="txt_keyAndValue" runat="server" type="hidden" />
    </form>
</body>
</html>
