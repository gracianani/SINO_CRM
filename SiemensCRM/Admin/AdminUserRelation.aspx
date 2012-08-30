<%@ Page Title="User Relation Management" Language="C#" MasterPageFile="~/Admin/AdminMasterPage.master"
    AutoEventWireup="true" CodeFile="AdminUserRelation.aspx.cs" Inherits="AdminRelation"
    EnableSessionState="ReadOnly" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div>
        <asp:Label ID="label_find" runat="server" Text="Search:"></asp:Label>
        <asp:TextBox ID="tbox_find" runat="server"></asp:TextBox>
        <asp:Label ID="label_in" runat="server" Text="IN"></asp:Label>
        <asp:DropDownList ID="ddlist_in" runat="server">
        </asp:DropDownList>
        <asp:Button ID="btn_find" runat="server" Text="Search" Width="60px" OnClick="btn_find_Click" />
        <asp:LinkButton ID="lbtn_findhelp" runat="server" OnClick="lbtn_findhelp_Click">Search help</asp:LinkButton>
    </div>
    <div style="width: 830px; overflow: scroll; height: 600px;">
        <table>
            <tr>
                <td align="center">
                    <asp:Label ID="label_head" runat="server" Text="User Relation Management" Font-Bold="True"
                        Font-Size="Medium"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:GridView ID="gv_administrator" runat="server" CellPadding="4" ForeColor="#333333"
                        GridLines="None" OnPageIndexChanging="gv_administrator_PageIndexChanging">
                        <RowStyle BackColor="#EFF3FB" />
                        <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                        <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                        <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                        <AlternatingRowStyle BackColor="White" />
                    </asp:GridView>
                </td>
            </tr>
        </table>
    </div>
</asp:Content>
