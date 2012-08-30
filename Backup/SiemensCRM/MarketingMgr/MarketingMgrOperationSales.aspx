<%@ Page Title="OP-Sales(Operational Sales)" Language="C#" MasterPageFile="~/MarketingMgr/MarketingMgrMasterPage.master"
    AutoEventWireup="true" CodeFile="MarketingMgrOperationSales.aspx.cs" Inherits="MarketingMgr_Default" EnableEventValidation="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
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
                    <asp:LinkButton ID="lbtn_opSales" runat="server" Font-Underline="False" OnClick="lbtn_opSales_Click"
                        Font-Bold="True">> OP - Sales</asp:LinkButton>
                </td>
                <td width="100" align="left">
                    <asp:LinkButton ID="lbtn_opBKG" runat="server" Font-Underline="False" OnClick="lbtn_opBKG_Click">> OP - BKG</asp:LinkButton>
                </td>
                <td width="100" align="left">
                    <asp:LinkButton ID="lbtn_SO" runat="server" Font-Underline="False" OnClick="lbtn_SO_Click">> SO</asp:LinkButton>
                </td>
            </tr>
        </table>
    </div>
    <div style="width: 830px; overflow: scroll; height: 440px;">
        <table>
            <tr>
                <td>
                    <asp:Label ID="label_operation" runat="server" Text="Operation"></asp:Label>
                    &nbsp;
                    <asp:DropDownList ID="ddlist_operation" runat="server">
                    </asp:DropDownList>
                    &nbsp;&nbsp;
                    <asp:Button ID="btn_search" runat="server" Text="Search" OnClick="btn_search_Click" />
                    <%--By Fxw 20110517 ITEM25 ADD Start--%>
<asp:Label ID="label_show" runat="server" Text=""></asp:Label>
<%--By Fxw 20110517 ITEM25 ADD End--%>
                </td>
            </tr>
            <tr>
                <td>
                    *****************************************************************************************
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="label_description" runat="server" Text="" Font-Bold="True" Font-Size="Medium"
                        Visible="false"></asp:Label>
                    <br />
                    <asp:Label ID="label_currency" runat="server" Text=""></asp:Label>
                    <br />
                    <asp:Button ID="btn_export" runat="server" Text="Export To Excel" OnClick="btn_export_Click"
                Width="230px" BackColor="#66CCFF" Font-Bold="True" />
                     <%--by yyan item8 20110614 add start --%>
                     <asp:Button ID="btnLocal" runat="server" Text="" Width="60px" OnClick="btnLocal_Click" />
                    <asp:Button ID="btnEUR" runat="server" Text="€" Width="60px" OnClick="btnEUR_Click" />
                    <asp:Label ID="lblEUR" runat="server" Text=""></asp:Label>
                    <%--by yyan item8 20110614 add end --%>
                    <br />
                </td>
            </tr>
            <tr>
                <td>
                    *****************************************************************************************
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Table ID="table_opSales" runat="server" Visible="false">
                    </asp:Table>
                </td>
            </tr>
        </table>
    </div>
</asp:Content>
