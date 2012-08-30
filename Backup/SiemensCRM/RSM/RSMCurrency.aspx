<%@ Page Title="Currency" Language="C#" MasterPageFile="~/RSM/RSMMasterPage.master"
    AutoEventWireup="true" CodeFile="RSMCurrency.aspx.cs" Inherits="RSM_RSMCurrency"
    EnableSessionState="ReadOnly" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div align="center">
        <asp:Label ID="label_head" runat="server" Text="Currency & Exchange" Font-Bold="True"
            Font-Size="Medium"></asp:Label>
    </div>
    <br />
    <div>
        <asp:Label ID="label_meetingdate" runat="server" Text="Meeting Date:"></asp:Label>&nbsp;&nbsp;
        <asp:DropDownList ID="ddlist_meetingdate" runat="server">
        </asp:DropDownList>
        &nbsp;&nbsp;&nbsp;&nbsp;
        <asp:Button ID="btn_search" runat="server" Text="Search" OnClick="btn_search_Click" />
    </div>
    <br />
   
 <div style="overflow: auto; height: 200px; width: 620px; position: relative; padding-top: 22px;">
        <asp:GridView ID="gv_currency" runat="server" CellPadding="4" ForeColor="#333333" Style="font-size: 12px;"
            GridLines="None" OnPageIndexChanging="gv_currency_PageIndexChanging" OnRowDeleting="gv_currency_RowDeleting"
            OnRowCancelingEdit="gv_currency_RowCancelingEdit" OnRowEditing="gv_currency_RowEditing"
            OnRowUpdating="gv_currency_RowUpdating" OnRowDataBound="gv_currency_RowDataBound">
            <RowStyle BackColor="#EFF3FB" />
            <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
            <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
            <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" CssClass="Freezing"/>
            <EditRowStyle BackColor="#33CCCC" />
            <AlternatingRowStyle BackColor="White" />
        </asp:GridView>
    </div>
    <div>
        <asp:Label ID="label_note" runat="server" Text="Rate*: Against EURO" Font-Bold="True"></asp:Label>
        <br />
        <asp:Label ID="label_date" runat="server" Text="Date*: Meeting Date" Font-Bold="True"></asp:Label>
        <br />
        <asp:Label ID="label_edt_del" runat="server" Text=""></asp:Label>
    </div>
    <asp:Panel ID="panel_readonly" runat="server">
        <br />
        <div>
            <asp:LinkButton ID="lbtn_AddCurrency" runat="server" OnClick="lbtn_AddCurrency_Click"
                Font-Bold="True">Add Currency</asp:LinkButton>
            <br />
            <asp:Panel ID="panel_addcurrency" runat="server">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="label_currency" runat="server" Text="Currency:"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlist_currency" runat="server" Width="100px">
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:Label ID="label_meetdate" runat="server" Text="Meeting Date:"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="tbox_date" runat="server" Enabled="false" Width="100px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="label_rate" runat="server" Text="Current Rate:"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="tbox_rate" runat="server" Width="100px" MaxLength="8"></asp:TextBox>
                        </td>
                        <td>
                            <asp:Label ID="label_rate2" runat="server" Text="Next Rate:"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="tbox_rate2" runat="server" Width="100px" MaxLength="8"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                        </td>
                        <td colspan="2">
                            <asp:Button ID="btn_Addcurrency" runat="server" Text="Add" OnClick="btn_Addcurrency_Click"
                                Width="60px" />
                            <asp:Button ID="btn_Cancel" runat="server" Text="Cancel" OnClick="btn_CancelCcurrency_Click"
                                Width="60px" />
                        </td>
                        <td>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </div>
        <asp:Label ID="label_add" runat="server"></asp:Label>
    </asp:Panel>
</asp:Content>
