<%@ Page Title="Currency" Language="C#" MasterPageFile="~/Admin/AdminMasterPage.master"
    AutoEventWireup="true" CodeFile="AdminCurrencyDic.aspx.cs" Inherits="Admin_AdminCurrencyDic" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <table>
        <tr>
            <td colspan="2" align="center" valign="middle">
                <asp:Label ID="lbl_header" runat="server" Text="Currency Management" Font-Bold="true"
                    Font-Size="Medium"></asp:Label>
            </td>
        </tr>
        <tr>
            <td valign="top">
                <div style="overflow: auto; height: 200px; width: 420px; position: relative; padding-top: 22px;">
                    <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
                        <contenttemplate>
                            <asp:GridView ID="gv_currency" runat="server" CellPadding="4" ForeColor="#333333"
                                Style="font-size: 12px;" GridLines="None" OnPageIndexChanging="gv_currency_PageIndexChanging"
                                OnRowDataBound="gv_currency_RowDataBound" OnRowDeleting="gv_currency_RowDeleting"
                                OnRowCancelingEdit="gv_currency_RowCancelingEdit" OnRowEditing="gv_currency_RowEditing"
                                OnRowUpdating="gv_currency_RowUpdating">
                                <RowStyle BackColor="#EFF3FB" />
                                <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                                <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" CssClass="Freezing" />
                                <EditRowStyle BackColor="#33CCCC" />
                                <AlternatingRowStyle BackColor="White" />
                            </asp:GridView>     
                        <br />
                    </contenttemplate>
                    </asp:UpdatePanel>
                </div>
            </td>
        </tr>
            <tr>
                <td>
                    <asp:UpdatePanel ID="UpdatePanel3" runat="server" >
                        <contenttemplate>
                  <asp:Label ID="lbl_del" runat="server" Text=""></asp:Label>
              </contenttemplate>
                    </asp:UpdatePanel>
                </td>
            </tr>
        <tr>
            <asp:Panel ID="panel_readonly" runat="server">
                <td valign="top">
                </td>
            </asp:Panel>
        </tr>
    </table>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <contenttemplate>
            <asp:LinkButton ID="lbtn_addcurrency" runat="server" OnClick="lbtn_addcurrency_Click"
                Font-Bold="true">Add Currency</asp:LinkButton>
            <asp:Panel ID="pnl_addcurrency" runat="server">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="lbl_currency" runat="server" Text="Currency"></asp:Label>
                            <asp:TextBox ID="tbox_currency" runat="server" Width="60px"></asp:TextBox>
                        </td>
                        <td>
                            <asp:Label ID="lbl_currencydes" runat="server" Text="Description"></asp:Label>
                            <asp:TextBox ID="tbox_currencydes" runat="server" Width="100px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                        </td>
                        <td>
                            <asp:Button ID="btn_add" runat="server" Text="Add" Width="60px" OnClick="btn_add_Click" />
                            <asp:Button ID="btn_cancel" runat="server" Text="Cancel" Width="60px" OnClick="btn_cancel_Click" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <br />
            <asp:Label ID="lbl_add" runat="server" Text=""></asp:Label>
        </contenttemplate>
        <triggers>
            <asp:PostBackTrigger ControlID="btn_add" />
        </triggers>
    </asp:UpdatePanel>
</asp:Content>
