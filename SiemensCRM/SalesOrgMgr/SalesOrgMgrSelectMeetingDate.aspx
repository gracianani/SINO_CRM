<%@ Page Language="C#" MasterPageFile="~/SalesOrgMgr/SalesOrgMgrMasterPage.master" AutoEventWireup="true" CodeFile="SalesOrgMgrSelectMeetingDate.aspx.cs" Inherits="SalesOrgMgr_SalesOrgMgrSelectMeetingDate" Title="Untitled Page" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
 <div>
        <br />
        <div>
            <asp:Label ID="label_currentmeetingdate" runat="server" Text=""></asp:Label>
        </div>
        <div>
            <table>
                <tr>
                    <td colspan="3" align="left">
                        <asp:Label ID="label1" runat="server" Text="Select Meeting Date" Font-Bold="true" Font-Size="Medium"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td colspan="3" align="left">
                        <asp:Label ID="label_meetingdate" runat="server" Text="If you want to search data in one date, <br />Please Select Meeting Date."></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                        Meeting Date
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlist_meetingdate" runat="server" Width="120px">
                        </asp:DropDownList>
                    </td>
                    <td>
                        <asp:Button ID="btn_set" runat="server" Text="Select" Width="60px" OnClick="btn_set_Click" />
                        <asp:Button ID="btn_unset" runat="server" Text="Select None" Width="90px" OnClick="btn_unset_Click" />
                    </td>
                </tr>
                <tr>
                    <td colspan="3">
                        <br />
                    </td>
                </tr>               
                <tr>
                    <td colspan="3">
                        <asp:Label ID="label_info" runat="server" Text=""></asp:Label>
                    </td>
                </tr>
               
            </table>
        </div>
    </div>
</asp:Content>

