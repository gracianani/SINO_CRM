<%@ Page Title="Actual Sales Data" Language="C#" MasterPageFile="~/Executive/ExecutiveMasterPage.master" AutoEventWireup="true"
    CodeFile="ExecutiveSalesData.aspx.cs" Inherits="Executive_ExecutiveSalesData" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
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
                &nbsp;&nbsp;&nbsp;&nbsp;
                <asp:Label ID="label_segment" runat="server" Text="Segment"></asp:Label>&nbsp;&nbsp;
                <asp:DropDownList ID="ddlist_segment" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlist_segment_SelectedIndexChanged">
                </asp:DropDownList>
                &nbsp;&nbsp;&nbsp;&nbsp;
                <asp:Label ID="label_saleorg" runat="server" Text="SalesOrg"></asp:Label>&nbsp;&nbsp;
                <asp:DropDownList ID="ddlist_saleorg" runat="server">
                </asp:DropDownList>
                &nbsp;&nbsp;&nbsp;&nbsp;
                <asp:Button ID="btn_search" runat="server" Text="Search" Width="60px" OnClick="btn_search_Click" />
            </td>
        </tr>
        <tr>
            <td align="left" valign="top">
                <asp:GridView ID="gv_actualBaclog" runat="server" CellPadding="4" ForeColor="#333333"
                    GridLines="None" OnRowCancelingEdit="gv_actualBaclog_RowCancelingEdit" OnRowEditing="gv_actualBaclog_RowEditing"
                    OnRowUpdating="gv_actualBaclog_RowUpdating">
                    <RowStyle BackColor="#EFF3FB" />
                    <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                    <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                    <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                    <EditRowStyle BackColor="#33CCCC" />
                    <AlternatingRowStyle BackColor="White" />
                </asp:GridView>
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
