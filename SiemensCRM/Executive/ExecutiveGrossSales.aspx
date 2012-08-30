<%@ Page Title="GR-Sales(Gross Sales)" Language="C#" MasterPageFile="~/Executive/ExecutiveMasterPage.master" AutoEventWireup="true" CodeFile="ExecutiveGrossSales.aspx.cs" EnableEventValidation="false" Inherits="Executive_ExecutiveGrossSales" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
   <div>
        <table>
            <tr>
                <td width="100" align="left">
                    <asp:LinkButton ID="lbtn_grSales" runat="server" Font-Underline="False" OnClick="lbtn_grSales_Click"
                        Font-Bold="True">> GR - Sales</asp:LinkButton>
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
                    <asp:LinkButton ID="lbtn_SO" runat="server" Font-Underline="False" OnClick="lbtn_SO_Click">> SO</asp:LinkButton>
                </td>
            </tr>
        </table>
    </div>
 <div style="width: 800px; overflow: scroll; height: 440px;">
        <table>
            <tr>
                <td>
                    *****************************************************************************************
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="label_description" runat="server" Text="" Font-Bold="True" Font-Size="Medium"></asp:Label>
                    <br />
                    <asp:Button ID="btn_export" runat="server" Text="Export To Excel" OnClick="btn_export_Click"
                Width="230px" BackColor="#66CCFF" Font-Bold="True" />
                <%--By Fxw 20110517 ITEM25 ADD Start--%>
<asp:Label ID="label_show" runat="server" Text=""></asp:Label>
<%--By Fxw 20110517 ITEM25 ADD End--%>
<%--by yyan item8 20110617 add start --%>
                    <br/>
                    <asp:Button ID="btnLocal" runat="server" Text="kLocal" Width="60px" OnClick="btnLocal_Click" />
                    <asp:Button ID="btnEUR" runat="server" Text="€" Width="60px" OnClick="btnEUR_Click" />
                    <asp:Label ID="lblEUR" runat="server" Text=""></asp:Label>
                    <%--by yyan item8 20110617 add end --%>
                </td>
            </tr>
            <tr>
                <td>
                    *****************************************************************************************
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Table ID="table_GrSales" runat="server" Visible="false">
                    </asp:Table>
                </td>
            </tr>
        </table>
    </div>
</asp:Content>

