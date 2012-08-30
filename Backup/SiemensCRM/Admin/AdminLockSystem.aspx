<%@ Page Title="Lock System" Language="C#" MasterPageFile="~/Admin/AdminMasterPage.master" AutoEventWireup="true"
    CodeFile="AdminLockSystem.aspx.cs" Inherits="Admin_AdminLockSystem" EnableSessionState="ReadOnly" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <script language="javascript" type="text/javascript">
    function showDialog(url,type)
    {
        var date = new Date();
        var returnVal = window.showModalDialog(url + '&d ='+date.getFullYear()+date.getMonth()+date.getDate()+date.getHours()+date.getMinutes()+date.getSeconds()+date.getMilliseconds(),null,'dialogWidth:600px;dialogHeight:400px;help:no;unadorned:no;resizable:yes;status:no;');
        {
            document.getElementById('back_date').value = returnVal[0];
            document.getElementById('back_key').value = returnVal[1];
            document.getElementById('back_type').value = type;
        }
        return false;
    }
    window.onload = function()
    {
        document.getElementById('back_date').value = "";
        document.getElementById('back_key').value = "";
        document.getElementById('back_type').value = "";
    }
    </script>
    <asp:Panel ID="panel_visible" runat="server">
    <input id="back_date" name="back_date" type="hidden" />
    <input id="back_key" name="back_key" type="hidden" />
    <input id="back_type"name="back_type"  type="hidden" />
    <asp:HiddenField ID="hidback_key" runat="server" />
    <table>
        <tr>
            <td align="center" valign="middle" colspan="4">
                <asp:Label ID="label_header" runat="server" Text="Lock/Unlock Management" Font-Bold="True"
                    Font-Size="Medium"></asp:Label>
            </td>
        </tr>
        <tr>
            <td valign="top" align="left">
                <asp:Label ID="label_subheaderlock" runat="server" Text="Lock:" Font-Bold="True"></asp:Label>
            </td>
            <td valign="top" align="left" style="width: 169px">
            </td>
             <td valign="top" align="left">
                <asp:Label ID="label_subheaderunlock" runat="server" Text="Unlock:" Font-Bold="True"></asp:Label>
            </td>
            <td valign="top" align="left" style="width: 169px">
            </td>
        </tr>
        <tr>
            <td valign="top" align="left">
                <asp:Label ID="label_locksalesOrg" runat="server" Text="SalesOrg:"></asp:Label>
            </td>
            <td valign="top" align="left" style="width: 169px">
                <asp:DropDownList ID="ddlist_locksalesorg" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlist_locksalesorg_SelectedIndexChanged"
                    Width="160px">
                </asp:DropDownList>
            </td>
            
              <td valign="top" align="left">
                <asp:Label ID="label_unlocksalesOrg" runat="server" Text="SalesOrg:"></asp:Label>
            </td>
            <td valign="top" align="left" style="width: 169px">
                <asp:DropDownList ID="ddlist_unlocksalesorg" runat="server" AutoPostBack="True" Width="160px"
                    OnSelectedIndexChanged="ddlist_unlocksalesorg_SelectedIndexChanged">
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td valign="top" align="left">
                <asp:Label ID="label_lockrsm" runat="server" Text="User:"></asp:Label>
            </td>
            <td valign="top" align="left" style="width: 220px">
                <asp:DropDownList ID="ddlist_lockrsm" runat="server" Width="220px">
                </asp:DropDownList>
            </td>
            
            <td valign="top" align="left">
                <asp:Label ID="label_unlockrsm" runat="server" Text="User:"></asp:Label>
            </td>
            <td style="width: 220px">
                <asp:DropDownList ID="ddlist_unlockrsm" runat="server" Width="220px" 
                    onselectedindexchanged="ddlist_unlockrsm_SelectedIndexChanged" 
                    AutoPostBack="True">
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td valign="top" align="left">
                <asp:Label ID="label_unlockdate" runat="server" Text="Unlock Date:"></asp:Label>
            </td>
            <td valign="top" align="left" style="width: 169px">
                <asp:TextBox ID="tbox_date" runat="server" Font-Bold="True" Width="160px" ReadOnly="True"></asp:TextBox>
                <br />
                <asp:Label ID="label_lock" runat="server" Text="aaa"></asp:Label>
                <br />
                <asp:Calendar ID="cal_date" runat="server" Width="160px" FirstDayOfWeek="Sunday"
                    OnSelectionChanged="cal_date_SelectionChanged">
                    <SelectedDayStyle BackColor="#0099FF" />
                    <WeekendDayStyle ForeColor="Red" />
                    <TodayDayStyle BackColor="#66FFFF" />
                    <OtherMonthDayStyle BackColor="White" ForeColor="#CCCCCC" />
                    <DayStyle BackColor="White" />
                    <DayHeaderStyle BackColor="#CCFFFF" />
                    <TitleStyle BackColor="#0099FF" />
                </asp:Calendar>
            </td>
            
            <td valign="top" align="left" style="width: 169px">
            <asp:Label ID="label_date" runat="server" Text=""></asp:Label>
            </td>
            <td align="left" valign="top">
                <asp:Button ID="btn_unlock" runat="server" Text="Unlock" Width="80px" OnClick="btn_unlock_Click" />
                <br />
                <asp:Label ID="label_unlocknote" runat="server" Text="" ForeColor="Red"></asp:Label>
            </td>
        </tr>
        <tr>
            <td colspan="2" align="left" valign="middle">
                <asp:Button ID="btn_lock" runat="server" Text="Lock" Width="80px" OnClick="btn_lock_Click" />
                <br />
                <asp:Label ID="label_locknote" runat="server" Text="" ForeColor="Red"></asp:Label>
            </td>
        </tr>
        <tr>
            <td valign="top" align="left">
                <asp:Label ID="label2" runat="server" Text="Lock Segment:" Font-Bold="True"></asp:Label>
            </td>
            <td valign="top" align="left" style="width: 169px">
            </td>
            <td valign="top" align="left">
                <asp:Label ID="label1" runat="server" Text="Lock All Users:" Font-Bold="True"></asp:Label>
            </td>
            <td valign="top" align="left" style="width: 169px">
            </td>
        </tr>
        <tr>
        <td  colspan="2" align="left" valign="top">
            <asp:GridView ID="gv_segment" runat="server" CellPadding="4" ForeColor="#333333"
                GridLines="None" OnPageIndexChanging="gv_segment_PageIndexChanging"  OnRowDataBound="gv_segment_RowDataBound">
                <RowStyle BackColor="#EFF3FB" />
                <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                <AlternatingRowStyle BackColor="White" />
            </asp:GridView>
        </td>
        <td valign="top" align="left">
                <asp:Label ID="label3" runat="server" Text="Unlock Date:"></asp:Label>
            </td>
            <td valign="top" align="left" style="width: 169px">
                <asp:TextBox ID="tbox_date_LockAll" runat="server" Font-Bold="True" Width="160px" ReadOnly="True"></asp:TextBox>
                <br />
                <asp:Label ID="label4" runat="server" Text=""></asp:Label>
                <br />
                <asp:Calendar ID="cal_date_LockAll" runat="server" Width="160px" FirstDayOfWeek="Sunday"
                    OnSelectionChanged="cal_date_LockAll_SelectionChanged">
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
        <tr>
            <td colspan="2"  align="left" valign="top">
                <%-- by mbq 20110509 item13 add start  --%>
                <asp:Button ID="lbtn_segment" runat="server" Text="Lock Segment" Width="100px" />
                <%-- by mbq 20110509 item13 add end--%>
                 <%-- by mbq 20110509 item13 add start  --%>
                <asp:Button ID="lbtn_unsegment" runat="server" Text="Unlock Segment" Width="110px" />
                <%-- by mbq 20110509 item13 add end--%>
            </td>
              <td colspan="2"  align="left" valign="top">
                <%-- by mbq 20110509 item13 add start  --%>
                <asp:Button ID="lbtn_alluser" runat="server" Text="Lock All Users" Width="110px" OnClick="lbtn_alluser_Click" />
                <%-- by mbq 20110509 item13 add end--%>
                 <%-- by mbq 20110509 item13 add start  --%>
                <asp:Button ID="lbtn_unalluser" runat="server" Text="Unlock All Users" Width="120px" OnClick="lbtn_unalluser_Click" />
                <%-- by mbq 20110509 item13 add end--%>
            </td>
        </tr>
        <tr>
            <td colspan="2"  align="left" valign="top">
                <%-- by mbq 20110509 item13 add start  --%>
                <asp:Label ID="label_lockSegmentNote" runat="server" Text="" ForeColor="Red"></asp:Label>
                <%-- by mbq 20110509 item13 add end--%>
                 <%-- by mbq 20110509 item13 add start  --%>
                <asp:Label ID="label_unLockSegmentNote" runat="server" Text="" ForeColor="Red"></asp:Label>
                <%-- by mbq 20110509 item13 add end--%>
            </td>
              <td colspan="2"  align="left" valign="top">
                <%-- by mbq 20110509 item13 add start  --%>
                <asp:Label ID="label_lockAllUserNote" runat="server" Text="" ForeColor="Red"></asp:Label>
                <%-- by mbq 20110509 item13 add end--%>
                 <%-- by mbq 20110509 item13 add start  --%>
                <asp:Label ID="label_unlockAllUserNote" runat="server" Text="" ForeColor="Red"></asp:Label>
                <%-- by mbq 20110509 item13 add end--%>
            </td>
        </tr>
        <tr>

        </tr>
        <tr>

        </tr>
        <tr>
          
        </tr>
        <tr>
          
        </tr>
    </table>
    </asp:Panel>
    <br />
    <br />
    <asp:Label ID="label_visible" runat="server" Text="" ForeColor="Red"></asp:Label>
</asp:Content>


