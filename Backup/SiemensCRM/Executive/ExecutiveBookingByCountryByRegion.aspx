<%@ Page Title="Bookings by key countries" Language="C#" MasterPageFile="~/Executive/ExecutiveMasterPage.master" AutoEventWireup="true"
    CodeFile="ExecutiveBookingByCountryByRegion.aspx.cs" Inherits="Executive_ExecutiveBookingByCountryByRegion" EnableEventValidation="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div align="center">
        <asp:Label ID="label_header" runat="server" Text="New Orders by Key Countries" Font-Size="Medium"
            Font-Bold="True"></asp:Label>
    </div>
    <div style="width: 830px; overflow: scroll; height: 480px;">
        <table>
            <tr>
                <td>
                    <asp:Label ID="label_region" runat="server" Text="Region"></asp:Label>&nbsp;&nbsp;
                    <asp:DropDownList ID="ddlist_region" runat="server">
                    </asp:DropDownList>
                    &nbsp;&nbsp;
                    <asp:Label ID="label_segment" runat="server" Text="Segment"></asp:Label>&nbsp;&nbsp;
                    <asp:DropDownList ID="ddlist_segment" runat="server">
                    </asp:DropDownList>
                    &nbsp;&nbsp;&nbsp;&nbsp;
                    <asp:Button ID="btn_search" runat="server" Text="Search" 
                        onclick="btn_search_Click" />
                </td>
            </tr>
            <tr>
                <td>
                    *****************************************************************************************
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="label_segmentDec" runat="server" Text="" Font-Bold="True"></asp:Label>
                    <br />
                    <asp:Label ID="label_description" runat="server" Text="" Font-Bold="True"></asp:Label>
                    <br />
                    <asp:Button ID="btn_export" runat="server" Text="Export To Excel" OnClick="btn_export_Click"
                Width="230px" BackColor="#66CCFF" Font-Bold="True" />
                </td>
            </tr>
            <tr>
                <td>
                    *****************************************************************************************
                </td>
            </tr>
            <tr>
                <td align="left" valign="top">
                    <asp:Table ID="table_bookingbycountry" runat="server" Visible="false">
                    </asp:Table>
                </td>
            </tr>
        </table>
    </div>
</asp:Content>
