<%@ Page Title="Customer" Language="C#" MasterPageFile="~/Admin/AdminMasterPage.master"
    AutoEventWireup="true" CodeFile="AdminCustomer.aspx.cs" Inherits="Admin_AdminCustomer"
    EnableSessionState="ReadOnly" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
<%--    <div>
        <asp:Label ID="label_find" runat="server" Text="Search:"></asp:Label>
        <asp:DropDownList ID="ddlist_find" runat="server">
        </asp:DropDownList>
        <asp:Label ID="label_in" runat="server" Text="IN"></asp:Label>
        <asp:DropDownList ID="ddlist_in" runat="server" AutoPostBack="True" 
            onselectedindexchanged="ddlist_in_SelectedIndexChanged">
        </asp:DropDownList>
        <asp:Button ID="btn_find" runat="server" Text="Search" Width="60px" OnClick="btn_find_Click" />
        <asp:LinkButton ID="lbtn_findhelp" runat="server" OnClick="lbtn_findhelp_Click">Search help</asp:LinkButton>
    </div>--%>
    <br />
    <div align="center">
        <asp:Label ID="label_head" runat="server" Text="Customer Management" Font-Bold="True" Font-Size="Medium"></asp:Label>
    </div>
    <div>
        <asp:GridView ID="gv_Customer" runat="server" CellPadding="4" ForeColor="#333333"
            GridLines="None" >
            <RowStyle BackColor="#EFF3FB" />
            <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
            <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
            <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
            <EditRowStyle BackColor="#33CCCC" />
            <AlternatingRowStyle BackColor="White" />
        </asp:GridView>
    </div>
    <asp:Label ID="label_del" runat="server"></asp:Label>
    <br />
    <br />
    <asp:Panel ID="panel_readonly" runat="server">
     <%--   <asp:LinkButton ID="lbtn_addcustomer" runat="server" Font-Underline="false" OnClick="lbtn_customer_Click">Add Customer Details</asp:LinkButton>--%>
        <asp:Panel ID="panel_addcustomer" runat="server">
          <%--   <table>
                <tr>
                    <td>
                        <asp:Label ID="lbl_name" runat="server" Text="Customer Name"></asp:Label>
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlist_name" runat="server" Width="300px">
                        </asp:DropDownList>
                    </td>
                    <td>
                        <asp:Label ID="label_type" runat="server" Text="Customer Type"></asp:Label>
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlist_type" runat="server" Width="200px">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                <td>
                    <asp:Label ID="lbl_saleschannel" runat="server" Text="Sales Channel"></asp:Label>
                </td>
                <td>
                    <asp:DropDownList ID="ddlist_saleschannel" runat="server">
                    </asp:DropDownList>
                </td>
                    <td>
                        <asp:Label ID="lbl_country" runat="server" Text="SubRegion"></asp:Label>
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlist_country" runat="server" Width="100px">
                        </asp:DropDownList>
                        <asp:Label ID="lbl_city" runat="server" Text="City"></asp:Label>
                        <asp:TextBox ID="tbox_city" runat="server" Width="100px"></asp:TextBox>
                    </td>
                    <tr>
                    <td>
                        <asp:Label ID="lbl_address" runat="server" Text="Address"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="tbox_address" runat="server" Width="200px"></asp:TextBox>
                    </td>
                    <td>
                        <asp:Label ID="lbl_department" runat="server" Text="Department"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="tbox_department" runat="server" Width="200px"></asp:TextBox>
                    </td>
                    </tr>
                    <tr>
                    <td colspan="4" align="center">
                        <asp:Button ID="btn_add" runat="server" Text="Add" Width="60px" OnClick="btn_add_Click" />&nbsp;&nbsp;
                        <asp:Button ID="btn_cancel" runat="server" Text="Cancel" Width="60px" OnClick="btn_cancel_Click" />
                    </td>
                    </tr>
                </tr>
            </table>--%>
        </asp:Panel>
        <br />
       <%-- <asp:Label ID="label_add" runat="server" Text=""></asp:Label>--%>
    </asp:Panel>
</asp:Content>
