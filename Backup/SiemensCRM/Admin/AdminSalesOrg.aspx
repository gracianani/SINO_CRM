<%@ Page Title="Sales Organization" Language="C#" MasterPageFile="~/Admin/AdminMasterPage.master"
    AutoEventWireup="true" CodeFile="AdminSalesOrg.aspx.cs" Inherits="Admin_AdminSalesOrg"
    EnableSessionState="ReadOnly" %>

<%--<html xmlns="http://www.w3.org/1999/xhtml">--%>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <asp:HiddenField runat="server" ID="hidSalesOrgID" Value="0" />
                <asp:Label ID="label_find" runat="server" Text="Search:"></asp:Label>
                <asp:DropDownList ID="ddlist_find" runat="server">
                </asp:DropDownList>
                <asp:Label ID="label_in" runat="server" Text="IN"></asp:Label>
                <asp:DropDownList ID="ddlist_in" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlist_in_SelectedIndexChanged">
                </asp:DropDownList>
                <asp:Button ID="btn_find" runat="server" Text="Search" Width="60px" OnClick="btn_find_Click" />
                <%-- <asp:LinkButton ID="lbtn_findhelp" runat="server" OnClick="lbtn_findhelp_Click">Search help</asp:LinkButton>--%>
            </ContentTemplate>
            <Triggers>
                <asp:PostBackTrigger ControlID="btn_find" />
            </Triggers>
        </asp:UpdatePanel>
    </div>
    <br />
    <table>
        <tr>
            <td align="center">
                <asp:Label ID="label_head" runat="server" Text="Sales Organization" Font-Bold="True"
                    Font-Size="Medium"></asp:Label>
            </td>
        </tr>
        <tr>
            <td align="left">
                <div style="overflow: auto; height: 400px; width: 820px; position: relative; padding-top: 22px;">
                    <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:GridView ID="gv_salsOrg" runat="server" CellPadding="4" ForeColor="#333333"
                                Style="font-size: 12px;" GridLines="None" OnPageIndexChanging="gv_salsOrg_PageIndexChanging"
                                OnRowDeleting="gv_salsOrg_RowDeleting" OnRowDataBound="gv_salsOrg_RowDataBound"
                                OnSelectedIndexChanging="gv_salsOrg_SelectedIndexChanging" OnRowCancelingEdit="gv_salsOrg_RowCancelingEdit1"
                                OnRowEditing="gv_salsOrg_RowEditing" OnRowUpdating="gv_salsOrg_RowUpdating">
                                <RowStyle BackColor="#EFF3FB" />
                                <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                                <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                                <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" CssClass="Freezing" />
                                <EditRowStyle BackColor="#33CCCC" />
                                <AlternatingRowStyle BackColor="White" />
                            </asp:GridView>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
                <br />
                <asp:UpdatePanel ID="UpdatePanel10" runat="server" >
                        <ContentTemplate>
                <asp:Label ID="label_salesOeg_edit_del" runat="server" Text=""></asp:Label>
                  </ContentTemplate>
                    </asp:UpdatePanel>
            </td>
        </tr>
        <asp:Panel ID="panel_readonly1" runat="server">
            <tr>
                <%--<td align="left">
                        <asp:LinkButton ID="lbtn_AddSalesOrg" runat="server" OnClick="lbtn_AddSalesOrg_Click"
                            Font-Bold="True">Add SalesOrganization</asp:LinkButton>
                    </td>
                </tr>
                <tr>
                    <td align="left">
                        <asp:Panel runat="server" ID="panel_add">
                            <br />
                            <asp:Label ID="label_Name" runat="server" Text="SalesOrg:"></asp:Label>&nbsp;
                            <asp:TextBox ID="tbox_name" runat="server" Width="200px"></asp:TextBox>&nbsp;&nbsp;
                            <asp:Label ID="label_abbr" runat="server" Text="Abbr:"></asp:Label>&nbsp;
                            <asp:TextBox ID="tbox_abbr" runat="server" Width="50px"></asp:TextBox>&nbsp;&nbsp;
                            <asp:Label ID="label_currency" runat="server" Text="Currency:"></asp:Label>&nbsp;
                            <asp:DropDownList ID="ddlist_currency" runat="server" Width="50px">
                            </asp:DropDownList>
                            &nbsp;&nbsp;
                            <asp:Button ID="btn_AddSalesOrg" runat="server" Text="Add" OnClick="btn_AddSalesOrg_Click"
                                Width="60px" />&nbsp;
                            <asp:Button ID="btn_CancelSalesOrg" runat="server" Text="Cancel" Width="60px" OnClick="btn_CancelSalesOrg_Click" />
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td align="left">
                        <asp:LinkButton ID="lbtn_updatecurrency" runat="server" Font-Bold="true" OnClick="lbtn_updatecurrency_Click">Modify currency</asp:LinkButton>
                    </td>
                </tr>
                <tr>
                    <td align="left">
                        <asp:Panel ID="pnl_updatecurrency" runat="server">
                            <asp:Label ID="lbl_salesorg2" runat="server" Text="SalesOrg"></asp:Label>&nbsp;
                            <asp:DropDownList ID="ddlist_salesorg" runat="server">
                            </asp:DropDownList>
                            &nbsp;&nbsp;
                            <asp:Label ID="lbl_currency" runat="server" Text="Currency"></asp:Label>&nbsp;
                            <asp:DropDownList ID="ddlist_currency2" runat="server">
                            </asp:DropDownList>
                            &nbsp;&nbsp;
                            <asp:Button ID="btn_addcurrency" runat="server" Text="Add" Width="60px" OnClick="btn_addcurrency_Click" />
                            &nbsp;
                            <asp:Button ID="btn_cancelcurrency" runat="server" Text="Cancel" Width="60px" OnClick="btn_cancelcurrency_Click" />
                        </asp:Panel>
                        <asp:Label ID="label_add" runat="server" Text=""></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td align="left">
                        <asp:Label ID="label_salesOrg_add" runat="server" Text=""></asp:Label>
                    </td>
                </tr>--%>
                <td align="left">
                    <asp:UpdatePanel ID="UpdatePanel7" runat="server">
                        <ContentTemplate>
                            <asp:LinkButton ID="lbtn_AddSalesOrg" runat="server" OnClick="lbtn_AddSalesOrg_Click"
                                Font-Bold="True">Add SalesOrganization</asp:LinkButton>
                            <br />
                            <asp:Panel runat="server" ID="panel_add">
                                <br />
                                <asp:Label ID="label_Name" runat="server" Text="SalesOrg:"></asp:Label>&nbsp;
                                <asp:TextBox ID="tbox_name" runat="server" Width="200px"></asp:TextBox>&nbsp;&nbsp;
                                <asp:Label ID="label_abbr" runat="server" Text="Abbr:"></asp:Label>&nbsp;
                                <asp:TextBox ID="tbox_abbr" runat="server" Width="50px"></asp:TextBox>&nbsp;&nbsp;
                                <asp:Label ID="label_currency" runat="server" Text="Currency:"></asp:Label>&nbsp;
                                <asp:DropDownList ID="ddlist_currency" runat="server" Width="50px">
                                </asp:DropDownList>
                                <br />
                                &nbsp;&nbsp;
                                <asp:Button ID="btn_AddSalesOrg" runat="server" Text="Add" OnClick="btn_AddSalesOrg_Click"
                                    Width="60px" />&nbsp;
                                <asp:Button ID="btn_CancelSalesOrg" runat="server" Text="Cancel" Width="60px" OnClick="btn_CancelSalesOrg_Click" />
                            </asp:Panel>
                        </ContentTemplate>
                        <Triggers>
                            <asp:PostBackTrigger ControlID="btn_AddSalesOrg" />
                        </Triggers>
                    </asp:UpdatePanel>
                </td>
            </tr>
            <tr>
                <td align="left">
                    <asp:UpdatePanel ID="UpdatePanel6" runat="server">
                        <ContentTemplate>
                            <asp:LinkButton ID="lbtn_updatecurrency" runat="server" Font-Bold="true" OnClick="lbtn_updatecurrency_Click">Modify currency</asp:LinkButton>
                            <br />
                            <asp:Panel ID="pnl_updatecurrency" runat="server">
                                <asp:Label ID="lbl_salesorg2" runat="server" Text="SalesOrg"></asp:Label>&nbsp;
                                <asp:DropDownList ID="ddlist_salesorg" runat="server">
                                </asp:DropDownList>
                                &nbsp;&nbsp;
                                <asp:Label ID="lbl_currency" runat="server" Text="Currency"></asp:Label>&nbsp;
                                <asp:DropDownList ID="ddlist_currency2" runat="server">
                                </asp:DropDownList>
                                <br />
                                &nbsp;&nbsp;
                                <asp:Button ID="btn_addcurrency" runat="server" Text="Add" Width="60px" OnClick="btn_addcurrency_Click" />
                                &nbsp;
                                <asp:Button ID="btn_cancelcurrency" runat="server" Text="Cancel" Width="60px" OnClick="btn_cancelcurrency_Click" />
                            </asp:Panel>
                            <asp:Label ID="label_add" runat="server" Text=""></asp:Label>
                            <asp:Label ID="label_salesOrg_add" runat="server" Text=""></asp:Label>
                        </ContentTemplate>
                        <Triggers>
                            <asp:PostBackTrigger ControlID="btn_addcurrency" />
                        </Triggers>
                    </asp:UpdatePanel>
                </td>
            </tr>
        </asp:Panel>
        <div>
            <tr>
                <td align="left">
                    <asp:UpdatePanel ID="UpdatePanel100" runat="server">
                        <ContentTemplate>
                            <asp:Panel runat="server" ID="panel_select">
                                <table>
                                    <tr>
                                        <td colspan="2" align="center">
                                            <asp:Label ID="label_caption" runat="server" Text="" Font-Bold="True"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="center">
                                            <asp:Label ID="label_RSM" runat="server" Visible="false" Text="Staff" />
                                        </td>
                                        <td align="center">
                                            <asp:Label ID="label_Segments" runat="server" Visible="false" Text="Segment" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" valign="top">
                                            <div style="overflow: auto; height: 200px; width: 310px; position: relative; padding-top: 22px;">
                                                <asp:UpdatePanel ID="UpdatePanel5" runat="server">
                                                    <ContentTemplate>
                                                        <asp:GridView ID="gv_RSM" runat="server" CellPadding="4" ForeColor="#333333" GridLines="None"
                                                            PageSize="10" OnRowDataBound="gv_RSM_RowDataBound" OnRowDeleting="gv_RSM_RowDeleting"
                                                            OnPageIndexChanging="gv_RSM_PageIndexChanging" Style="font-size: 12px;">
                                                            <RowStyle BackColor="#EFF3FB" />
                                                            <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                                            <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                                                            <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" CssClass="Freezing" />
                                                            <AlternatingRowStyle BackColor="White" />
                                                        </asp:GridView>
                                                    </ContentTemplate>
                                                </asp:UpdatePanel>
                                            </div>
                                            <br />
                                            <asp:Label ID="label_RSM_del" runat="server" Text=""></asp:Label>
                                        </td>
                                        <td align="left" valign="top">
                                            <div style="overflow: auto; height: 200px; width: 310px; position: relative; padding-top: 22px;">
                                                <asp:UpdatePanel ID="UpdatePanel4" runat="server">
                                                    <ContentTemplate>
                                                        <asp:GridView ID="gv_Segment" runat="server" CellPadding="4" ForeColor="#333333"
                                                            Style="font-size: 12px;" GridLines="None" PageSize="10" OnRowDataBound="gv_Segment_RowDataBound"
                                                            OnRowDeleting="gv_Segment_RowDeleting" OnPageIndexChanging="gv_Segment_PageIndexChanging">
                                                            <RowStyle BackColor="#EFF3FB" />
                                                            <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                                            <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                                                            <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" CssClass="Freezing" />
                                                            <AlternatingRowStyle BackColor="White" />
                                                        </asp:GridView>
                                                    </ContentTemplate>
                                                </asp:UpdatePanel>
                                            </div>
                                            <br />
                                            <asp:Label ID="label_Segment_del" runat="server" Text=""></asp:Label>
                                        </td>
                                    </tr>
                                    <asp:Panel ID="panel_readonly2" runat="server">
                                        <tr>
                                            <td align="left" valign="top">
                                                <asp:UpdatePanel ID="UpdatePanel8" runat="server">
                                                    <ContentTemplate>
                                                        <asp:LinkButton ID="lbtn_RSM" runat="server" Font-Bold="True" OnClick="lbtn_RSM_Click">Add User</asp:LinkButton>
                                                        <br />
                                                        <asp:Panel runat="server" ID="panel_addRSM">
                                                            <asp:Label ID="label_RSMName" runat="server" Text="Login:"></asp:Label>
                                                            <asp:DropDownList ID="ddlist_RSM" runat="server" Width="185px">
                                                            </asp:DropDownList>
                                                            <br />
                                                            <div align="right">
                                                                <asp:Button ID="btn_addRSM" runat="server" Text="Add" OnClick="btn_addRSM_Click"
                                                                    Width="60px" />&nbsp;
                                                                <asp:Button ID="btn_CancelRSM" runat="server" Text="Cancel" OnClick="btn_CancelRSM_Click"
                                                                    Width="60px" />
                                                            </div>
                                                        </asp:Panel>
                                                        <asp:Label ID="label_RSM_add" runat="server" Text=""></asp:Label>
                                                    </ContentTemplate>
                                                    <Triggers>
                                                        <asp:PostBackTrigger ControlID="btn_addRSM" />
                                                    </Triggers>
                                                </asp:UpdatePanel>
                                            </td>
                                            <td align="left" valign="top">
                                                <asp:UpdatePanel ID="UpdatePanel9" runat="server">
                                                    <ContentTemplate>
                                                        <asp:LinkButton ID="lbtn_segment" runat="server" Font-Bold="True" OnClick="lbtn_segment_Click">Add Segment</asp:LinkButton>
                                                        <br />
                                                        <asp:Panel runat="server" ID="panel_addSegment">
                                                            <asp:Label ID="label_Segment" runat="server" Text="Segment:"></asp:Label>
                                                            <asp:DropDownList ID="ddlist_segment" runat="server" Width="185px">
                                                            </asp:DropDownList>
                                                            <br />
                                                            <div align="right">
                                                                <asp:Button ID="btn_addSegment" runat="server" Text="Add" OnClick="btn_addSegment_Click"
                                                                    Width="60px" />
                                                                &nbsp;
                                                                <asp:Button ID="btn_CancelSegment" runat="server" Text="Cancel" OnClick="btn_CancelSegment_Click"
                                                                    Width="60px" />
                                                            </div>
                                                        </asp:Panel>
                                                        <asp:Label ID="label_Segment_add" runat="server" Text=""></asp:Label>
                                                    </ContentTemplate>
                                                    <Triggers>
                                                        <asp:PostBackTrigger ControlID="btn_addSegment" />
                                                    </Triggers>
                                                </asp:UpdatePanel>
                                            </td>
                                        </tr>
                                    </asp:Panel>
                                </table>
                            </asp:Panel>
                        </ContentTemplate>
                        
                    </asp:UpdatePanel>
                </td>
            </tr>
    </table>
</asp:Content>
