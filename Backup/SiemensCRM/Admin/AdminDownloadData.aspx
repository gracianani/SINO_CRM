<%@ Page Title="Download Data" Language="C#" MasterPageFile="~/Admin/AdminMasterPage.master"
    AutoEventWireup="true" CodeFile="AdminDownloadData.aspx.cs" Inherits="Admin_AdminDownloadData"
    EnableSessionState="ReadOnly" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:Panel ID="panel_visible" runat="server">
        <table width="100%" cellpadding="4">
            <tr>
                <td align="center" valign="middle">
                    <asp:Label ID="label_header" runat="server" Text="Download Data" Font-Bold="True"
                        Font-Size="Medium"></asp:Label>
                </td>
            </tr>
            <tr>
                <td align="center" valign="middle">
                </br>
                </br>
                    <asp:Label ID="label1" runat="server" Text="Click following button to download all data into Excel files."></asp:Label>
                </td>
            </tr>
            <tr>
                <td align="center" valign="middle">
                </br>
                    <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="Download Data"  Font-Bold="True" />
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:Label ID="label_visible" runat="server" Text="" ForeColor="Red"></asp:Label>
</asp:Content>
