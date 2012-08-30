<%@ Page Title="Forecast" Language="C#" MasterPageFile="~/Executive/ExecutiveMasterPage.master" AutoEventWireup="true"
    CodeFile="ExecutiveBookingsSalesForecast.aspx.cs" Inherits="Executive_ExecutiveBookingsSalesForecast" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <table>
        <tr>
            <td colspan="2">
                <asp:Label ID="label_segment" runat="server" Text="Segment"></asp:Label>
                &nbsp;
                <asp:DropDownList ID="ddlist_segment" runat="server">
                </asp:DropDownList>
                &nbsp;&nbsp;
                <asp:Button ID="btn_search" runat="server" Text="Search" OnClick="btn_search_Click" />
                <asp:Label ID="label_show" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                *****************************************************************************************
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:Panel ID="panel_dec" runat="server" Visible="false">
                    <div align="center">
                        <asp:Label ID="label_segDecription" runat="server" Text="" Font-Bold="True" Font-Size="X-Large"></asp:Label>
                        <br />
                        <asp:Label ID="label_bookingsDecription" runat="server" Text="" Font-Bold="True"
                            Font-Size="Medium"></asp:Label>
                        <br />
                        <asp:Label ID="label_salesDecription" runat="server" Text="" Font-Bold="True" Font-Size="Medium"></asp:Label>
                    </div>
                </asp:Panel>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                *****************************************************************************************
            </td>
        </tr>
        <tr>
            <td align="left" valign="top">
                <asp:GridView ID="gv_opAbbr" runat="server" CellPadding="4" ForeColor="#333333" GridLines="None"
                    ShowHeader="False" PageSize="20">
                    <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
                    <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                    <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                </asp:GridView>
            </td>
            <td align="center" valign="top">
                <asp:Label ID="label_noteDate" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:Panel ID="panel_enter" runat="server" Visible="false">
                    <asp:LinkButton ID="lbtn_op_sales" runat="server" OnClick="lbtn_op_sales_Click">Operational Sales (Sales by Operation by Sales Organization)</asp:LinkButton>
                    <br />
                    <br />
                    <asp:LinkButton ID="lbtn_op_bkg" runat="server" OnClick="lbtn_op_bkg_Click">Operational New Orders (New Orders by Operation by Sales Organization)</asp:LinkButton>
                    <br />
                    <br />
                    <asp:LinkButton ID="lbtn_gr_sales" runat="server" OnClick="lbtn_gr_sales_Click">Group Sales (Sales by Operation)</asp:LinkButton>
                    <br />
                    <br />
                    <asp:LinkButton ID="lbtn_gr_bkg" runat="server" OnClick="lbtn_gr_bkg_Click">Group New Orders (New Orders by Operation)</asp:LinkButton>
                    <br />
                    <br />
                    <asp:LinkButton ID="lbtn_salesorg" runat="server" OnClick="lbtn_salesorg_Click">New Orders By Sales Organizations</asp:LinkButton>
                    <br />
                    <br />
                    <asp:LinkButton ID="lbtn_region" runat="server" OnClick="lbtn_region_Click">New Orders by Key Countries</asp:LinkButton>
                    &nbsp;&nbsp;
                    <asp:LinkButton ID="lbtn_chart" runat="server" OnClick="lbtn_chart_Click">Regional Charts By Segment</asp:LinkButton>
                </asp:Panel>
            </td>
        </tr>
    </table>
</asp:Content>
