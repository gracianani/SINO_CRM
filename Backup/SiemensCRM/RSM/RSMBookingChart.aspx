<%@ Page Title="Charts" Language="C#" MasterPageFile="~/RSM/RSMMasterPage.master" AutoEventWireup="true"
    CodeFile="RSMBookingChart.aspx.cs" Inherits="RSM_BookingChart" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div align="center">
        <asp:Label ID="label_header" runat="server" Text="Bookings By key Countries" Font-Size="Medium"
            Font-Bold="True"></asp:Label>
    </div>
    <div style="width: 830px; overflow: scroll; height: 590px;">
        <table>
            <tr>
                <td colspan="3">
                    <asp:Label ID="label_segment" runat="server" Text="Segment"></asp:Label>&nbsp;&nbsp;
                    <asp:DropDownList ID="ddlist_segment" runat="server" Width="60px">
                    </asp:DropDownList>
                    &nbsp;&nbsp;&nbsp;&nbsp;
                    <asp:Button ID="btn_show" runat="server" Text="Show" Width="60px" OnClick="btn_show_Click" />
                </td>
            </tr>
            <tr>
                <td colspan="3">
                    *****************************************************************************************
                </td>
            </tr>
            <tr>
                <td colspan="3">
             *****************************************************************************************
                </td>
            </tr>
            <asp:Panel ID="panel_visible" runat="server" Visible="false">
            <tr>
                <td>
                    <asp:Image ID="Image1" runat="server" />
                </td>
                <td>
                    <asp:Image ID="Image2" runat="server" />
                </td>
                <td>
                    <asp:Image ID="Image3" runat="server" />
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <asp:GridView ID="gv_bookingSegment" runat="server" CellPadding="4" 
                        ForeColor="#333333" BorderColor="Black" BorderWidth="1px">
                        <RowStyle BackColor="#EFF3FB" />
                        <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                        <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                        <HeaderStyle BackColor="#FFF68F" Font-Bold="True" ForeColor="Black" />
                        <AlternatingRowStyle BackColor="White" />
                    </asp:GridView>
                </td>
                <td>
                    <asp:GridView ID="gv_totalBookingSegment" runat="server" CellPadding="4" 
                        ForeColor="#333333">
                        <RowStyle BackColor="#EFF3FB" />
                        <FooterStyle BackColor="#EFF3FB" Font-Bold="True" ForeColor="White" />
                        <HeaderStyle BackColor="#FFF68F" Font-Bold="True" ForeColor="Black" />
                        <AlternatingRowStyle BackColor="White" />
                    </asp:GridView>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Image ID="Image6" runat="server" />
                </td>
                <td>
                    <asp:Image ID="Image7" runat="server" />
                </td>
                <td>
                    <asp:Image ID="Image8" runat="server" />
                </td>
            </tr>
            </asp:Panel>
        </table>
    </div>
</asp:Content>
