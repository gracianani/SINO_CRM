<%@ Page Title="Sales Organization Information" Language="C#" MasterPageFile="~/SalesOrgMgr/SalesOrgMgrMasterPage.master"
    AutoEventWireup="true" CodeFile="SalesOrgMgrSalesOrgInfo.aspx.cs" Inherits="SalesOrgMgr_SalesOrgMgrSalesOrgInfo" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div style="width: 830px; overflow: scroll; height: 480px">
        <table>
            <tr>
                <td align="center">
                    <asp:Label ID="label_head" runat="server" Text="Sales Organization" Font-Bold="True"
                        Font-Size="Medium"></asp:Label>
                </td>
            </tr>
            <tr>
                <td align="left">
                    <asp:GridView ID="gv_salsOrg" runat="server" CellPadding="4" ForeColor="#333333"
                        GridLines="None">
                        <RowStyle BackColor="#EFF3FB" />
                        <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                        <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                        <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                        <AlternatingRowStyle BackColor="White" />
                    </asp:GridView>
                    <br />
                    <asp:Label ID="label_salesOeg_edit_del" runat="server" Text=""></asp:Label>
                </td>
            </tr>
            <tr>
                <td align="left">
                    <table>
                        <tr>
                            <td align="left" valign="top">
                                <asp:GridView ID="gv_RSM" runat="server" CellPadding="4" ForeColor="#333333" GridLines="None"
                                    OnRowDataBound="gv_RSM_RowDataBound" OnRowDeleting="gv_RSM_RowDeleting">
                                    <RowStyle BackColor="#EFF3FB" />
                                    <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                    <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                                    <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                    <AlternatingRowStyle BackColor="White" />
                                </asp:GridView>
                                <br />
                                <asp:Label ID="label_RSM_del" runat="server" Text=""></asp:Label>
                            </td>
                            <td align="left" valign="top">
                                <asp:GridView ID="gv_Segment" runat="server" CellPadding="4" ForeColor="#333333"
                                    GridLines="None" OnRowDataBound="gv_Segment_RowDataBound" OnRowDeleting="gv_Segment_RowDeleting">
                                    <RowStyle BackColor="#EFF3FB" />
                                    <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                    <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                                    <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                    <AlternatingRowStyle BackColor="White" />
                                </asp:GridView>
                                <br />
                                <asp:Label ID="label_Segment_del" runat="server" Text=""></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td align="left" valign="top">
                            </td>
                            <td align="left" valign="top">
                                <asp:LinkButton ID="lbtn_segment" runat="server" Font-Bold="True" OnClick="lbtn_segment_Click">Add Segment</asp:LinkButton>
                                <br />
                                <asp:Panel runat="server" ID="panel_addSegment">
                                    <asp:Label ID="label_Segment" runat="server" Text="Segment:"></asp:Label>
                                    <asp:DropDownList ID="ddlist_segment" runat="server" Width="80px">
                                    </asp:DropDownList>
                                    <asp:Button ID="btn_addSegment" runat="server" Text="Add" OnClick="btn_addSegment_Click"
                                        Width="60px" />
                                    &nbsp;
                                    <asp:Button ID="btn_CancelSegment" runat="server" Text="Cancel" OnClick="btn_CancelSegment_Click"
                                        Width="60px" />
                                </asp:Panel>
                                <asp:Label ID="label_Segment_add" runat="server" Text=""></asp:Label>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </div>
</asp:Content>
