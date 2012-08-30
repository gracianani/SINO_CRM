<%@ Page Title="Actual Sales Data" Language="C#" MasterPageFile="~/Admin/AdminMasterPage.master" AutoEventWireup="true"
    CodeFile="AdminSalesData.aspx.cs" Inherits="Admin_AdminSalesData" EnableEventValidation="false"%>
    <%@ MasterType VirtualPath="~/Admin/AdminMasterPage.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
 <script language="jscript" type ="text/javascript"> 
     document.onkeydown=function enterToTab() 
{ 
    if(event.keyCode == 13)
    { 
        if (event.srcElement.type == "text")
        {
            return false;
        }
    } 
} 
 </script> 
 
    <table>
        <tr>
            <td align="center">
                <asp:Label ID="label_header" runat="server" Text="Sales Data And Backlog" Font-Size="Medium"
                    Font-Bold="True"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="label_marketingmgr" runat="server" Text="MarketingMgr"></asp:Label>&nbsp;&nbsp;
                <asp:DropDownList ID="ddlist_marketingmgr" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlist_marketingmgr_SelectedIndexChanged">
                </asp:DropDownList>
                &nbsp;&nbsp;
                <%--ryzhang 20110512 item37 add start--%>
                 <asp:Label ID="label_operation" runat="server" Text="Operation"></asp:Label>&nbsp;&nbsp;
                <asp:DropDownList ID="ddlist_operation" runat="server" >
                </asp:DropDownList>
                &nbsp;&nbsp;
               <%-- ryzhang 20110512 item37 add end--%>
                <asp:Label ID="label_segment" runat="server" Text="Segment"></asp:Label>&nbsp;&nbsp;
                <asp:DropDownList ID="ddlist_segment" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlist_segment_SelectedIndexChanged">
                </asp:DropDownList>
                &nbsp;&nbsp;
                <asp:Label ID="label_saleorg" runat="server" Text="SalesOrg"></asp:Label>&nbsp;&nbsp;
                <asp:DropDownList ID="ddlist_saleorg" runat="server">
                </asp:DropDownList>
                &nbsp;&nbsp;
                <asp:Button ID="btn_search" runat="server" Text="Search" Width="60px" OnClick="btn_search_Click" />
            </td>
        </tr>
       <%-- by ryzhang 20110509 item37 add start--%>
       <tr>
            <td>*****************************************************************************************</td>
       </tr>
            <tr>
                <td>
                    <asp:Panel ID="panel_search" runat="server">
                        <table>
                            <tr>
                                <td>
                                    Traffic Light:<asp:ImageButton ID="ibtn_red" runat="server" ImageUrl="~/images/red.png" OnClick="ibtn_red_Click" onclientclick="return confirmS('red')"/>
                                    <asp:ImageButton ID="ibtn_green" runat="server" ImageUrl="~/images/green.png" OnClick="ibtn_green_Click" onclientclick="return confirmS('green')" />
                                    |&nbsp;Current Status:<asp:Image ID="img_status" runat="server" />
                                    <%--by yyan itemw82 20110721 add start--%> 
                                    &nbsp;&nbsp;Currency: <asp:Label ID="lblCurrency" runat="server" ></asp:Label>
                                    <%--by yyan itemw82 20110721 add end--%> 
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                </td>
            </tr>
       <tr>
            <td>*****************************************************************************************</td>
       </tr>
       <%-- by ryzhang 20110509 item37 add end--%>
        <tr>
            <td align="left" valign="top">
            <div runat="server" id="div_export">
                <asp:GridView ID="gv_actualBaclog" runat="server" CellPadding="4" ForeColor="#333333"
                    GridLines="None" OnRowCancelingEdit="gv_actualBaclog_RowCancelingEdit" OnRowEditing="gv_actualBaclog_RowEditing"
                    OnRowUpdating="gv_actualBaclog_RowUpdating" OnRowDataBound="gv_actualBaclog_RowDataBound" >
                    <RowStyle BackColor="#EFF3FB" />
                    <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                    <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                    <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                    <EditRowStyle BackColor="#33CCCC"  CssClass="edit" />
                    <AlternatingRowStyle BackColor="White" />
                </asp:GridView>
            </div>
                <br />
                <asp:Label ID="label_note" runat="server" Text="" ForeColor="Red"></asp:Label>
            </td>
        </tr>
        <asp:Panel ID="panel_readonly" runat="server">
        <tr>
            <td>
                <asp:LinkButton ID="lbtn_add" runat="server" OnClick="lbtn_add_Click">Add Backlog</asp:LinkButton>
                <br />
                <asp:Panel ID="panel_add" runat="server" Visible="false">
                    <asp:Label ID="label_backlogY" runat="server" Text="Date:"></asp:Label>&nbsp;&nbsp;
                    <asp:DropDownList ID="ddlist_backlogY" runat="server" Width="60px">
                    </asp:DropDownList>
                    &nbsp;&nbsp;&nbsp;&nbsp;
                    <asp:Button ID="btn_add" runat="server" Text="Add" Width="60px" OnClick="btn_add_Click" />
                    &nbsp;&nbsp;
                    <asp:Button ID="btn_cancel" runat="server" Text="Cancel" Width="60px" OnClick="btn_cancel_Click" />
                </asp:Panel>
            </td>
        </tr>
        </asp:Panel>
    </table>
</asp:Content>
