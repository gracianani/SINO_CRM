<%@ Page Title="Customer Relation" Language="C#" MasterPageFile="~/Admin/AdminMasterPage.master"
    AutoEventWireup="true" CodeFile="AdminCustomerRelation.aspx.cs" Inherits="Admin_AdminCustomerRelation" %>

<%--<html xmlns="http://www.w3.org/1999/xhtml">--%>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <table>
        <tr>
            <td>
                &nbsp;&nbsp;&nbsp;<asp:Label ID="lbl_type" runat="server" Text="Customer Type" Font-Bold="true"></asp:Label>
            </td>
            <td>
                &nbsp;&nbsp;&nbsp;<asp:Label ID="lbl_name" runat="server" Text="Customer Name" Font-Bold="true"></asp:Label>
            </td>
        </tr>
        <tr>
            <td valign="top">
                <%--by dxs 20110511 add start--%>
                <asp:UpdatePanel ID="UpdatePanel33" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <%--by dxs 20110511 add end--%>
                        <%-- by dxs 20110511 add start--%>
                        <div style="overflow: auto; height: 510px; width: 370px; position: relative; padding-top: 22px;">
                            <%-- by dxs 20110511 add end--%>
                            <asp:GridView ID="gv_type" runat="server" CellPadding="4" ForeColor="#333333" GridLines="None"
                                Style="font-size: 12px;" OnPageIndexChanging="gv_type_PageIndexChanging" OnRowDataBound="gv_type_RowDataBound"
                                OnRowDeleting="gv_type_RowDeleting">
                                <RowStyle BackColor="#EFF3FB" />
                                <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                                <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" CssClass="Freezing" />
                                <AlternatingRowStyle BackColor="White" />
                            </asp:GridView>
                            <%-- by dxs 20110511 add start--%>
                        </div>
                        <%-- by dxs 20110511 add end--%>
                        <%--by dxs 20110511 add start--%>
                    </ContentTemplate>
                </asp:UpdatePanel>
                <%--by dxs 20110511 add end--%>
            </td>
            <td valign="top">
               <%-- by dxs 20110511 add start--%>
                        <div style="overflow: auto; height: 510px; width: 370px; position: relative; padding-top: 22px;">
                            <%-- by dxs 20110511 add end--%>
                             <%--by dxs 20110511 add start--%>
                <asp:UpdatePanel ID="UpdatePanel22" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <%-- by dxs 20110511 add end--%>
                            <asp:GridView ID="gv_name" runat="server" CellPadding="4" ForeColor="#333333" GridLines="None"
                                Style="font-size: 12px;" OnPageIndexChanging="gv_name_PageIndexChanging" OnRowDataBound="gv_name_RowDataBound"
                                OnRowDeleting="gv_name_RowDeleting">
                                <RowStyle BackColor="#EFF3FB" />
                                <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                                <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" CssClass="Freezing" />
                                <AlternatingRowStyle BackColor="White" />
                            </asp:GridView>
                    <%-- by dxs 20110511 add start--%>
                    </ContentTemplate>
                </asp:UpdatePanel>
                <%-- by dxs 20110511 add end--%>
                            <%-- by dxs 20110511 add start--%>
                        </div>
                        <%-- by dxs 20110511 add end--%>

            </td>
        </tr>
        <asp:Panel ID="pnl_readonly" runat="server">
            <tr>
                <td>
                    <%--by dxs 20110511 add start--%>
                    <asp:UpdatePanel ID="UpdatePanel888" runat="server">
                        <ContentTemplate>
                            <%--by dxs 20110511 add end--%>
                            <asp:Label ID="lbl_deltype" runat="server" Text=""></asp:Label>
                            <%--by dxs 20110511 add start--%>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    <%--by dxs 20110511 add end--%>
                </td>
                <td>
                    <%--by dxs 20110511 add start--%>
                    <asp:UpdatePanel ID="UpdatePanel889" runat="server">
                        <ContentTemplate>
                            <%--by dxs 20110511 add end--%>
                            <asp:Label ID="lbl_delname" runat="server" Text=""></asp:Label>
                            <%--by dxs 20110511 add start--%>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    <%--by dxs 20110511 add end--%>
                </td>
            </tr>
            <tr>
                <td>
                    <%-- by dxs 20110511 add start--%>
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                        <ContentTemplate>
                            <%--by dxs 20110511 add end--%>
                            <asp:LinkButton ID="lbtn_addtype" runat="server" Font-Underline="false" OnClick="lbtn_addtype_Click">Add Customer Type</asp:LinkButton>
                            <asp:Panel ID="pnl_addtype" runat="server">
                                <asp:Label ID="lbl_typeadd" runat="server" Text="Type"></asp:Label>
                                <asp:TextBox ID="tbox_type" runat="server"></asp:TextBox>
                                <asp:Button ID="btn_addtype" runat="server" Text="Add" Width="60px" OnClick="btn_addtype_Click" />
                                <asp:Button ID="btn_canceltype" runat="server" Text="Cancel" Width="60px" OnClick="btn_canceltype_Click" />
                            </asp:Panel>
                            <%--by dxs 20110511 add start--%>
                        </ContentTemplate>
                        <Triggers>
                            <asp:PostBackTrigger ControlID="btn_addtype" />
                        </Triggers>
                    </asp:UpdatePanel>
                    <%--by dxs 20110511 add end--%>
                </td>
                <td>
                    <%--by dxs 20110511 add start--%>
                    <asp:UpdatePanel ID="UpdatePanel2" runat="server" Visible="false">
                        <ContentTemplate>
                            <%--by dxs 20110511 add end--%>
                            <asp:LinkButton ID="lbtn_addname" runat="server" Font-Underline="false" OnClick="lbtn_addname_Click">Add Customer Name</asp:LinkButton>
                            <asp:Panel ID="pnl_addname" runat="server">
                                <asp:Label ID="lbl_nameadd" runat="server" Text="Name"></asp:Label>
                                <asp:TextBox ID="tbox_name" runat="server"></asp:TextBox>
                                <asp:Button ID="btn_addname" runat="server" Text="Add" Width="60px" OnClick="btn_addname_Click" />
                                <asp:Button ID="btn_cancelname" runat="server" Text="Cancel" Width="60px" OnClick="btn_cancelname_Click" />
                            </asp:Panel>
                            <%--by dxs 20110511 add start--%>
                        </ContentTemplate>
                        <Triggers>
                            <asp:PostBackTrigger ControlID="btn_addname" />
                        </Triggers>
                    </asp:UpdatePanel>
                    <%--by dxs 20110511 add end--%>
                </td>
            </tr>
            <tr>
                <td>
                    <%--by dxs 20110511 add start--%>
                    <asp:UpdatePanel ID="UpdatePanel88" runat="server">
                        <ContentTemplate>
                            <%--by dxs 20110511 add end--%>
                            <asp:Label ID="lbl_addtype" runat="server" Text=""></asp:Label>
                            <%--by dxs 20110511 add start--%>
                        </ContentTemplate>
                        <%--by dxs 20110511 add end--%>
                    </asp:UpdatePanel>
                </td>
                <td>
                    <%--by dxs 20110511 add start--%>
                    <asp:UpdatePanel ID="UpdatePanel89" runat="server">
                        <ContentTemplate>
                            <%--by dxs 20110511 add end--%>
                            <asp:Label ID="lbl_addname" runat="server" Text=""></asp:Label>
                            <%--by dxs 20110511 add start--%>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    <%--by dxs 20110511 add end--%>
                </td>
            </tr>
        </asp:Panel>
    </table>
</asp:Content>
