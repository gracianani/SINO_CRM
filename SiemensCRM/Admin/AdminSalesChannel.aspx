<%@ Page Title="Sales Channel" Language="C#" MasterPageFile="~/Admin/AdminMasterPage.master"
    AutoEventWireup="true" CodeFile="AdminSalesChannel.aspx.cs" Inherits="Admin_AdminSalesChannel" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <asp:Label ID="label_find" runat="server" Text="Search:"></asp:Label>
                <asp:DropDownList ID="ddlist_find" runat="server">
                </asp:DropDownList>
                <asp:Label ID="label_in" runat="server" Text="IN"></asp:Label>
                <asp:DropDownList ID="ddlist_in" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlist_in_SelectedIndexChanged">
                </asp:DropDownList>
                <asp:Button ID="btn_find" runat="server" Text="Search" Width="60px" OnClick="btn_find_Click" />
            </ContentTemplate>
            <Triggers>
                <asp:PostBackTrigger ControlID="btn_find" />
            </Triggers>
        </asp:UpdatePanel>
        <%--<asp:LinkButton ID="lbtn_findhelp" runat="server" OnClick="lbtn_findhelp_Click">Search help</asp:LinkButton>--%>
    </div>
    <br />
    <div align="center">
        <asp:Label ID="Label_head" runat="server" Text="Sales Channel" Font-Bold="True" Font-Size="Medium"></asp:Label>
    </div>
    <div>
        <table>
            <tr>
                <td valign="top" align="left">
                <div style="overflow: auto; height: 180px; width: 617px; position: relative; padding-top: 22px;">
                   <asp:UpdatePanel ID="UpdatePanel2" runat="server">
            <ContentTemplate>
                    <asp:GridView ID="gv_channel" runat="server" CellPadding="4" ForeColor="#333333"
                        GridLines="None" OnPageIndexChanging="gv_channel_PageIndexChanging" OnRowDataBound="gv_channel_RowDataBound"
                        OnRowDeleting="gv_channel_RowDeleting" Style="font-size: 12px;">
                        <RowStyle BackColor="#EFF3FB" />
                        <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                        <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                        <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" CssClass="Freezing"/>
                        <AlternatingRowStyle BackColor="White" />
                    </asp:GridView>
                        </ContentTemplate>
        </asp:UpdatePanel>
                    </div>
                    <br />
                     <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                        <ContentTemplate>
                    <asp:Label ID="label_del" runat="server"></asp:Label>
                    </ContentTemplate>
                    </asp:UpdatePanel>
                </td>
            </tr>
        </table>
                    <asp:UpdatePanel ID="UpdatePanel4" runat="server">
                        <ContentTemplate>
                            <asp:Panel ID="panel_readonly" runat="server">
                                <asp:LinkButton ID="lbtn_channel" runat="server" Font-Bold="True" OnClick="lbtn_channel_Click">Add sales channel</asp:LinkButton>
                                <asp:Panel ID="panel_addchannel" runat="server">
                                    <table>
                                        <tr>
                                            <td>
                                                <asp:Label ID="lbl_name" runat="server" Text="Sales Channel"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="tbox_name" runat="server" Width="200px" Text=""></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                            </td>
                                            <td>
                                                <asp:Button ID="btn_addchannel" runat="server" Text="Add" Width="60px" OnClick="btn_addchannel_Click" />
                                                <asp:Button ID="btn_cancelchannel" runat="server" Text="Cancel" Width="60px" OnClick="btn_cancelchannel_Click" />
                                            </td>
                                        </tr>
                                    </table>
                                </asp:Panel>
                                <br />
                                <asp:Label ID="label_add" runat="server" Text=""></asp:Label>
                            </asp:Panel>
                        </ContentTemplate>
                        <Triggers>
                            <asp:PostBackTrigger ControlID="btn_addchannel" />
                        </Triggers>
                    </asp:UpdatePanel>
    </div>
</asp:Content>
