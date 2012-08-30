<%@ Page Title="User Account" Language="C#" MasterPageFile="~/SalesOrgMgr/SalesOrgMgrMasterPage.master"
    AutoEventWireup="true" CodeFile="SalesOrgMgrAccountProfile.aspx.cs" Inherits="SalesOrgMgr_SalesOrgMgrAccount" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div>
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
          <br />
        <%--By yyan 20110620 ItemW35 add end--%>
        <table>
            <tr>
                <td align="center">
                    <asp:Label ID="label_head" runat="server" Text="User Information" Font-Bold="True" Font-Size="Medium"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:UpdatePanel ID="UpdatePanel3" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div style="overflow: auto; height: 450px; width: 750px; position: relative; padding-top: 22px;">
                                <asp:GridView ID="gv_salesOrgMgr" runat="server" CellPadding="4" ForeColor="#333333"
                                    GridLines="None" OnPageIndexChanging="gv_salesOrgMgr_PageIndexChanging" OnRowCancelingEdit="gv_salesOrgMgr_RowCancelingEdit"
                                    OnRowDataBound="gv_salesOrgMgr_RowDataBound" OnRowDeleting="gv_salesOrgMgr_RowDeleting" Style="font-size: 12px;"
                                    OnRowEditing="gv_salesOrgMgr_RowEditing" OnRowUpdating="gv_salesOrgMgr_RowUpdating">
                                    <RowStyle BackColor="#EFF3FB" />
                                    <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                    <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                                    <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" CssClass="Freezing" />
                                    <EditRowStyle BackColor="#33CCCC" />
                                    <AlternatingRowStyle BackColor="White" />
                                </asp:GridView>
                            </div>
                            <asp:Label ID="label_edt_del" runat="server" Text=""></asp:Label>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </td>
            </tr>
            <tr>
                <td>
                    <div>
                        <asp:LinkButton ID="lbtn_addUser" runat="server" OnClick="lbtn_addUser_click" Font-Bold="True">Add new user</asp:LinkButton>
                        <br />
                        <asp:Panel ID="panel_add" runat="server">
                            <asp:Label ID="label_Alias" runat="server" Text="Login:"></asp:Label>&nbsp;
                            <asp:TextBox ID="tbox_Alias" runat="server" Width="100px"></asp:TextBox>&nbsp;&nbsp;
                            <asp:Label ID="label_Abbr" runat="server" Text="Abbr:"></asp:Label>&nbsp;
                            <asp:TextBox ID="tbox_Abbr" runat="server" Width="40px"></asp:TextBox>&nbsp;&nbsp;
                            <asp:Label ID="label_gendar" runat="server" Text="Gender:"></asp:Label>&nbsp;
                            <asp:DropDownList ID="ddlist_gendar" runat="server">
                            </asp:DropDownList>
                            &nbsp;&nbsp;
                            <asp:Label ID="label_startdate" runat="server" Text="StartDate:"></asp:Label>&nbsp;
                            <asp:TextBox ID="tbox_startdate" runat="server" Width="130px"></asp:TextBox>
                            <asp:Button ID="btn_AddUser" runat="server" Text="Add" OnClick="btn_AddUser_Click"
                                Width="60px" />&nbsp;
                            <asp:Button ID="btn_CanUser" runat="server" Text="Cancel" OnClick="btn_CanUser_Click"
                                Width="60px" />
                        </asp:Panel>
                        <asp:Label ID="label_add" runat="server" Text=""></asp:Label>
                    </div>
                </td>
            </tr>
        </table>
    </div>
</asp:Content>
