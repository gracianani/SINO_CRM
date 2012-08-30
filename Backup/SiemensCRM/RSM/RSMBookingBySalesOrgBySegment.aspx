<%@ Page Language="C#" AutoEventWireup="true" CodeFile="RSMBookingBySalesOrgBySegment.aspx.cs"
    Inherits="RSM_RSMBookingBySalesOrgBySegment" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>SO</title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <table>
                <tr>
                    <td>
                        *****************************************************************************************
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="label_salesorgAbbr" runat="server" Text="" Font-Bold="True" Font-Size="Large"></asp:Label>&nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:Label ID="label_headdescription" runat="server" Text="" Font-Bold="True" Font-Size="Medium"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="label_currency" runat="server" Text="" Font-Bold="True"></asp:Label>
                        <asp:Button ID="btn_export" runat="server" Text="Export To Excel" OnClick="btn_export_Click"
                            Width="230px" BackColor="#66CCFF" Font-Bold="True" />
                        <input id="btnClose" type="button" value="Close" onclick="window.close();" style="background-color: #66CCFF;
                            font-weight: bold; width: 100px;" />
                        <br />
                    </td>
                </tr>
                <tr>
                    <td>
                        *****************************************************************************************
                    </td>
                </tr>
                <tr>
                    <td align="left" valign="top">
                        <div runat="server" id="div_export">
                            <table>
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
                        </div>
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
