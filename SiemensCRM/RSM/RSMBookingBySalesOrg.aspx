<%@ Page Title="SO(Sales Organization)" Language="C#" MasterPageFile="~/RSM/RSMMasterPage.master"
    AutoEventWireup="true" CodeFile="RSMBookingBySalesOrg.aspx.cs" Inherits="RSM_Default"
    EnableEventValidation="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:HiddenField ID="hidCurrencyFlag" runat="server" />
    <div>
        <table>
            <tr>
                <td width="100" align="left">
                    <asp:LinkButton ID="lbtn_grSales" runat="server" Font-Underline="False" OnClick="lbtn_grSales_Click">> GR - Sales</asp:LinkButton>
                </td>
                <td width="100" align="left">
                    <asp:LinkButton ID="lbtn_grBKG" runat="server" Font-Underline="False" OnClick="lbtn_grBKG_Click">> GR - BKG</asp:LinkButton>
                </td>
                <td width="100" align="left">
                    <asp:LinkButton ID="lbtn_opSales" runat="server" Font-Underline="False" OnClick="lbtn_opSales_Click">> OP - Sales</asp:LinkButton>
                </td>
                <td width="100" align="left">
                    <asp:LinkButton ID="lbtn_opBKG" runat="server" Font-Underline="False" OnClick="lbtn_opBKG_Click">> OP - BKG</asp:LinkButton>
                </td>
                <td width="100" align="left">
                    <asp:LinkButton ID="lbtn_SO" runat="server" Font-Underline="False" OnClick="lbtn_SO_Click"
                        Font-Bold="True">> SO</asp:LinkButton>
                </td>
            </tr>
        </table>
    </div>
    <div style="width: 830px; overflow: scroll; height: 580px">
        <table>
            <tr>
                <td colspan="20">
                    <asp:Label ID="label_salesOrg" runat="server" Text="Sales Organization:"></asp:Label>
                    &nbsp;
                    <asp:DropDownList ID="ddlist_salesOrg" runat="server" Width="300px">
                    </asp:DropDownList>
                    &nbsp;&nbsp;
                    <asp:Button ID="btn_search" runat="server" Text="Search" Width="60px" OnClick="btn_search_Click" />
                    <%--By Fxw 20110517 ITEM25 ADD Start--%>
                    <asp:Label ID="label_show" runat="server" Text=""></asp:Label>
                    <%--By Fxw 20110517 ITEM25 ADD End--%>
                </td>
            </tr>
            <tr>
                <td colspan="20">
                    *****************************************************************************************
                </td>
            </tr>
            <tr>
                <td colspan="20">
                    <asp:Panel ID="panel_head" runat="server">
                        <asp:Label ID="label_salesorgAbbr" runat="server" Text="" Font-Bold="True" Font-Size="Large"></asp:Label>
                        &nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:Label ID="label_headdescription" runat="server" Text="" Font-Bold="True" Font-Size="Medium"></asp:Label>
                    </asp:Panel>
                </td>
            </tr>
            <tr>
                <td colspan="20">
                    <asp:Panel ID="panel_search" runat="server">
                        <asp:Label ID="label_currency" runat="server" Text="" Font-Bold="True"></asp:Label>
                        <br />
                        <asp:Button ID="btn_export" runat="server" Text="Export To Excel" OnClick="btn_export_Click"
                            Width="230px" BackColor="#66CCFF" Font-Bold="True" />
                        <asp:Button ID="btnLocal" runat="server" Text="kLocal" Width="60px" OnClick="btnLocal_Click" />
                        <asp:Button ID="btn_EUR" runat="server" Text="€" Width="60px" OnClick="btn_EUR_Click" />
                    </asp:Panel>
                </td>
            </tr>
            <tr>
                <td colspan="20">
                    *****************************************************************************************
                </td>
            </tr>
            <tr>
                <td>
                    <table runat="server" id="div_export">
                      <tr>
                        </tr>
                        <tr>
                            <td id="labelLocal" runat="server" align="left" >
                            </td>
                        </tr>
                         <tr>
                        </tr>
                        <tr>
                            <td align="left" valign="top">
                                <div>
                                    <asp:GridView ID="gv_bookingbydatebyproduct" runat="server" CellPadding="4" ForeColor="#333333"
                                        GridLines="None">
                                        <RowStyle BackColor="#EFF3FB" />
                                        <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                        <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                                        <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                        <AlternatingRowStyle BackColor="White" />
                                    </asp:GridView>
                                </div>
                                <div>
                                    <asp:GridView ID="gv_bookingTotalbydatebyproduct" runat="server" CellPadding="4"
                                        ForeColor="#333333" GridLines="None">
                                        <RowStyle BackColor="#EFF3FB" />
                                        <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                        <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                                        <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                        <AlternatingRowStyle BackColor="White" />
                                    </asp:GridView>
                                </div>
                            </td>
                            <td align="left" valign="top">
                                <div>
                                    <asp:GridView ID="gv_booking1bydatebyproduct" runat="server" CellPadding="4" ForeColor="#333333"
                                        GridLines="None">
                                        <RowStyle BackColor="#EFF3FB" />
                                        <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                        <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                                        <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                        <AlternatingRowStyle BackColor="White" />
                                    </asp:GridView>
                                </div>
                                <div>
                                    <asp:GridView ID="gv_booking1Totalbydatebyproduct" runat="server" CellPadding="4"
                                        ForeColor="#333333" GridLines="None">
                                        <RowStyle BackColor="#EFF3FB" />
                                        <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                        <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                                        <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                        <AlternatingRowStyle BackColor="White" />
                                    </asp:GridView>
                                </div>
                            </td>
                            <td align="left" valign="top">
                                <div>
                                    <asp:GridView ID="gv_booking2bydatebyproduct" runat="server" CellPadding="4" ForeColor="#333333"
                                        GridLines="None">
                                        <RowStyle BackColor="#EFF3FB" />
                                        <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                        <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                                        <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                        <AlternatingRowStyle BackColor="White" />
                                    </asp:GridView>
                                </div>
                                <div>
                                    <asp:GridView ID="gv_booking2Totalbydatebyproduct" runat="server" CellPadding="4"
                                        ForeColor="#333333" GridLines="None">
                                        <RowStyle BackColor="#EFF3FB" />
                                        <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                        <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                                        <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                        <AlternatingRowStyle BackColor="White" />
                                    </asp:GridView>
                                </div>
                            </td>
                            <td align="left" valign="top">
                                <div>
                                    <asp:GridView ID="gv_booking3bydatebyproduct" runat="server" CellPadding="4" ForeColor="#333333"
                                        GridLines="None">
                                        <RowStyle BackColor="#EFF3FB" />
                                        <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                        <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                                        <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                        <AlternatingRowStyle BackColor="White" />
                                    </asp:GridView>
                                </div>
                                <div>
                                    <asp:GridView ID="gv_booking3Totalbydatebyproduct" runat="server" CellPadding="4"
                                        ForeColor="#333333" GridLines="None">
                                        <RowStyle BackColor="#EFF3FB" />
                                        <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                        <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                                        <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                        <AlternatingRowStyle BackColor="White" />
                                    </asp:GridView>
                                </div>
                            </td>
                            <td align="left" valign="top">
                                <div>
                                    <asp:GridView ID="gv_booking4bydatebyproduct" runat="server" CellPadding="4" ForeColor="#333333"
                                        GridLines="None">
                                        <RowStyle BackColor="#EFF3FB" />
                                        <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                        <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                                        <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                        <AlternatingRowStyle BackColor="White" />
                                    </asp:GridView>
                                </div>
                                <div>
                                    <asp:GridView ID="gv_booking4Totalbydatebyproduct" runat="server" CellPadding="4"
                                        ForeColor="#333333" GridLines="None">
                                        <RowStyle BackColor="#EFF3FB" />
                                        <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                        <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                                        <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                        <AlternatingRowStyle BackColor="White" />
                                    </asp:GridView>
                                </div>
                            </td>
                            <td align="left" valign="top">
                                <asp:Table ID="table_bookingsByProduct" runat="server" Visible="false">
                                </asp:Table>
                            </td>
                            <td align="left" valign="top">
                                <div>
                                    <asp:GridView ID="gv_bookingtbydate" runat="server" CellPadding="4" ForeColor="#333333"
                                        GridLines="None">
                                        <RowStyle BackColor="#EFF3FB" />
                                        <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                        <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                                        <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                        <AlternatingRowStyle BackColor="White" />
                                    </asp:GridView>
                                </div>
                                <div>
                                    <asp:GridView ID="gv_bookingtTotalbydate" runat="server" CellPadding="4" ForeColor="#333333"
                                        GridLines="None">
                                        <RowStyle BackColor="#EFF3FB" />
                                        <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                        <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                                        <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                        <AlternatingRowStyle BackColor="White" />
                                    </asp:GridView>
                                </div>
                            </td>
                            <td align="left" valign="top">
                                <div>
                                    <asp:GridView ID="gv_bookingnextbydate" runat="server" CellPadding="4" ForeColor="#333333"
                                        GridLines="None">
                                        <RowStyle BackColor="#EFF3FB" />
                                        <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                        <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                                        <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                        <AlternatingRowStyle BackColor="White" />
                                    </asp:GridView>
                                </div>
                                <div>
                                    <asp:GridView ID="gv_bookingnextTotalbydate" runat="server" CellPadding="4" ForeColor="#333333"
                                        GridLines="None">
                                        <RowStyle BackColor="#EFF3FB" />
                                        <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                        <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                                        <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                        <AlternatingRowStyle BackColor="White" />
                                    </asp:GridView>
                                </div>
                            </td>
                            <td align="left" valign="top">
                                <div>
                                    <asp:GridView ID="gv_VS" runat="server" CellPadding="4" ForeColor="#333333" GridLines="None">
                                        <RowStyle BackColor="#EFF3FB" />
                                        <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                        <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                                        <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                        <AlternatingRowStyle BackColor="White" />
                                    </asp:GridView>
                                </div>
                                <div>
                                    <asp:GridView ID="gv_VSTotal" runat="server" CellPadding="4" ForeColor="#333333"
                                        GridLines="None">
                                        <RowStyle BackColor="#EFF3FB" />
                                        <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                        <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                                        <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                        <AlternatingRowStyle BackColor="White" />
                                    </asp:GridView>
                                </div>
                            </td>
                            <td align="left" valign="top">
                                <div>
                                    <asp:GridView ID="gv_VS03" runat="server" CellPadding="4" ForeColor="#333333" GridLines="None">
                                        <RowStyle BackColor="#EFF3FB" />
                                        <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                        <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                                        <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                        <AlternatingRowStyle BackColor="White" />
                                    </asp:GridView>
                                </div>
                                <div>
                                    <asp:GridView ID="gv_VSTotal03" runat="server" CellPadding="4" ForeColor="#333333"
                                        GridLines="None">
                                        <RowStyle BackColor="#EFF3FB" />
                                        <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                        <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                                        <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                        <AlternatingRowStyle BackColor="White" />
                                    </asp:GridView>
                                </div>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </div>
</asp:Content>
