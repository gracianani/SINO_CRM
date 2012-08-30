<%@ Page Title="Country" Language="C#" MasterPageFile="~/Assistant/AssistantMasterPage.master"
    AutoEventWireup="true" CodeFile="AssistantCountry.aspx.cs" Inherits="Assistant_AssistantCountryCustomer"
    EnableSessionState="ReadOnly" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div>
        <%--by ryzhang 20110517 item6 start--%>
        <asp:HiddenField ID="optionId" runat="server" />
        <asp:HiddenField ID="optionName" runat="server" />
        <%--by ryzhang 20110517 item6 end--%>
        <table>
            <tr>
                <td align="center">
                    <asp:Label ID="lbl_header" runat="server" Text="Country Management" Font-Bold="true"
                        Font-Size="Medium"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="lbl_description" runat="server" Text="Please select adding options"></asp:Label>
                    <asp:DropDownList ID="ddlist_description" runat="server">
                    </asp:DropDownList>
                    <asp:TextBox ID="ddlist_search2_text" runat="server" Enabled="false" />
                    <asp:Button ID="btn_search" runat="server" Text="Search" OnClick="btn_search_Click"
                        Width="60px" />
                </td>
            </tr>
            <tr>
                <td align="right">
                    <asp:Label ID="lbl2_description" runat="server" Text="(Input keywords for search)"></asp:Label>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                </td>
            </tr>
            <tr>
                <td>
                    <div id="divflow" runat="server">
                        <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:GridView ID="gv_option" runat="server" CellPadding="4" ForeColor="#333333" GridLines="None"
                                    OnPageIndexChanging="gv_option_PageIndexChanging" OnRowDeleting="gv_option_RowDeleting"
                                    Style="font-size: 12px;" OnSelectedIndexChanging="gv_option_SelectedIndexChanging">
                                    <RowStyle BackColor="#EFF3FB" />
                                    <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                    <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                                    <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" CssClass="Freezing" />
                                    <AlternatingRowStyle BackColor="White" />
                                </asp:GridView>
                                <asp:HiddenField ID="hidWidth" runat="server" Value="100px" />
                                <br />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    <asp:UpdatePanel ID="UpdatePanel6" runat="server">
                        <ContentTemplate>
                            <asp:Label ID="lbl_del" runat="server" Text=""></asp:Label>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </td>
            </tr>
            <asp:Panel ID="panel_readonly" runat="server">
                <tr>
                    <td>
                        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                            <ContentTemplate>
                                <asp:LinkButton ID="lbtn_add" runat="server" Text="" OnClick="lbtn_add_Click"></asp:LinkButton>
                                <asp:Panel ID="pnl_add" runat="server">
                                    <table>
                                        <tr>
                                            <td colspan="2">
                                                <asp:Label ID="lbl_parent" runat="server"></asp:Label>
                                                <asp:DropDownList ID="ddlist_parent" runat="server">
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Label ID="lbl_addname" runat="server" Text=""></asp:Label>
                                                <asp:TextBox ID="tbox_addname" runat="server" Width="250px"></asp:TextBox>
                                            </td>
                                            <td>
                                                <asp:Label ID="lbl_countryISO" runat="server" Text="Country ISO"></asp:Label>
                                                <asp:TextBox ID="tbox_countryISO" runat="server" Width="80px"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="center" colspan="2">
                                                <asp:Button ID="btn_add" runat="server" Text="Add" OnClick="btn_add_Click" Width="60px" />
                                                <asp:Button ID="btn_cancel" runat="server" Text="Cancel" OnClick="btn_cancel_Click"
                                                    Width="60px" />
                                            </td>
                                        </tr>
                                    </table>
                                </asp:Panel>
                                <br />
                                <asp:Label ID="lbl_add" runat="server" Text=""></asp:Label>
                            </ContentTemplate>
                            <Triggers>
                                <asp:PostBackTrigger ControlID="btn_add" />
                            </Triggers>
                        </asp:UpdatePanel>
                    </td>
                </tr>
            </asp:Panel>
            <%--by ryzhang 20110516 item6 start--%>
            <tr>
                <td>
                    <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                        <ContentTemplate>
                            <asp:Panel ID="panel_modify_parent" runat="server" Visible="false">
                                <table>
                                    <tbody>
                                        <tr>
                                            <td>
                                                <asp:Label ID="lbl_modify_parent" runat="server"></asp:Label>
                                                <asp:DropDownList ID="ddlist_modify_parent" runat="server">
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:HiddenField ID="modify_option_id" runat="server" />
                                                <asp:Label ID="lbl_modify_parent_error" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="center">
                                                <asp:Button ID="btn_modify_parent" OnClick="btn_modify_parent_Click" runat="server"
                                                    Width="60px" Text="Modify"></asp:Button>
                                                <asp:Button ID="btn_cancel_parent" runat="server" Width="60px" Text="Cancel" OnClick="btn_cancel_parent_Click">
                                                </asp:Button>
                                            </td>
                                        </tr>
                                    </tbody>
                                </table>
                            </asp:Panel>
                        </ContentTemplate>
                        <Triggers>
                            <asp:PostBackTrigger ControlID="btn_modify_parent"></asp:PostBackTrigger>
                        </Triggers>
                    </asp:UpdatePanel>
                    <br />
                </td>
            </tr>
            <%--by ryzhang 20110516 item6 end--%>
        </table>
    </div>
</asp:Content>
