<%@ Page Title="Project" Language="C#" MasterPageFile="~/Admin/AdminMasterPage.master"
    AutoEventWireup="true" CodeFile="AdminProjects.aspx.cs" Inherits="Admin_AdminProjects" %>
 
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div>
        <asp:Label ID="label_find" runat="server" Text="Search:" Visible="False"></asp:Label>
        <asp:DropDownList ID="ddlist_find" runat="server" Visible="False">
        </asp:DropDownList>
        <asp:Label ID="label_in" runat="server" Text="IN" Visible="False"></asp:Label>
        <asp:DropDownList ID="ddlist_in" runat="server" AutoPostBack="True" 
            onselectedindexchanged="ddlist_in_SelectedIndexChanged" Visible="False">
        </asp:DropDownList>
        <asp:Button ID="btn_find" runat="server" Text="Search" Width="60px" Visible="False"  />
        <asp:LinkButton ID="lbtn_findhelp" runat="server" OnClick="lbtn_findhelp_Click" Visible="False">Search help</asp:LinkButton>
    </div>
    <br />
    <div align="center">
        <asp:Label ID="Label_head" runat="server" Text="Project Management" Font-Bold="True" Font-Size="Medium"></asp:Label>
    </div>
    
    <div>
        <asp:GridView ID="gv_Project" runat="server" CellPadding="4" ForeColor="#333333"
            GridLines="None" >
            <RowStyle BackColor="#EFF3FB" />
            <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
            <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
            <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
            <AlternatingRowStyle BackColor="White" />
        </asp:GridView>
    </div>
    <asp:Label ID="label_del" runat="server" Visible="False"></asp:Label>
    <br />
    <br />
    <asp:Panel ID="panel_readonly" runat="server" Visible="False">
        <asp:LinkButton ID="lbtn_addproject" runat="server" Font-Bold="True" OnClick="lbtn_addproject_Click" Visible="False">Add project</asp:LinkButton>
        <asp:Panel ID="panel_addproject" runat="server">
            <div>
                <asp:Label ID="label_name" runat="server" Text="Project Name" Visible="False"></asp:Label>
                <asp:TextBox ID="tbox_name" runat="server" Width="200px" Visible="False"></asp:TextBox>
                <asp:Label ID="lbl_customer" runat="server" Text="Customer" Visible="False"></asp:Label>
                <asp:DropDownList ID="ddlist_customer" runat="server"  Width="300px" Visible="False">
                </asp:DropDownList>
                <asp:Label ID="lbl_currency" runat="server" Text="Currency" Visible="False"></asp:Label>
                <asp:DropDownList ID="ddlist_currency" runat="server" Visible="False">
                </asp:DropDownList>
            </div>
            <div>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="label_fromcountry" runat="server" Text="Project Country(POS)" Visible="False" ></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="label_value" runat="server" Text="Project Value" Width="100px" Visible="False"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="label_probability" runat="server" Text="% in budget" Width="100px" Visible="False"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="label_tocountry" runat="server" Text="Project Country(POD)" Visible="False"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:DropDownList ID="ddlist_fromcountry" runat="server" Width="250px" Visible="False">
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:TextBox ID="tbox_value" runat="server" Width="80px" Visible="False"></asp:TextBox>
                        </td>
                        <td>
                            <asp:TextBox ID="tbox_probability" runat="server" Width="80px" Visible="False"></asp:TextBox>
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlist_tocountry" runat="server" Width="250px" Visible="False">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="4" align="center">
                            <asp:Button ID="btn_addproject" runat="server" Text="Add" Width="60px" OnClick="btn_addproject_Click" Visible="False" />&nbsp;
                            <asp:Button ID="btn_Cancelproject" runat="server" Text="Cancel" Width="60px" OnClick="btn_Cancelproject_Click" Visible="False" />
                        </td>
                    </tr>
                </table>
            </div>
        </asp:Panel>
        <br />
        <asp:Label ID="label_add" runat="server"></asp:Label>
    </asp:Panel>
</asp:Content>
