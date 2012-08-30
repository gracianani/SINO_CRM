<%@ Page Title="User Account" Language="C#" MasterPageFile="~/MarketingMgr/MarketingMgrMasterPage.master"
    AutoEventWireup="true" CodeFile="MarketingMgrProfile.aspx.cs" Inherits="MarketingMgr_MarketMgrProfile" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
     <%--By yyan 20110620 ItemW35 add Start--%>
         <asp:UpdatePanel ID="UpdatePanel11" runat="server">
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
        <%--By yyan 20110620 ItemW35 add end--%>
        <br />
    <div align="center" style="font-size: large">
        <strong>User Information</strong>
    </div>
    <table>
        <tr>
            <td>
                <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div style="overflow: auto; height:450px; width: 750px; position: relative; padding-top: 22px;">
                            <asp:GridView ID="gv_profile" runat="server" CellPadding="4" ForeColor="#333333"
                                GridLines="None" OnRowCancelingEdit="gv_profile_RowCancelingEdit" OnRowEditing="gv_profile_RowEditing"
                                OnRowUpdating="gv_profile_RowUpdating" Style="font-size: 12px;">
                                <RowStyle BackColor="#EFF3FB" />
                                <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                                <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White"  CssClass="Freezing" />
                                <AlternatingRowStyle BackColor="White" />
                            </asp:GridView>
                        </div>
                        <br />
                        <asp:Label ID="label_edt_del" runat="server" Text="" Visible="false"></asp:Label>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
    </table>
</asp:Content>
