<%@ Page Title="Meeting Date" Language="C#" MasterPageFile="~/Admin/AdminMasterPage.master"
    AutoEventWireup="true" CodeFile="AdminSetMeetingDate.aspx.cs" Inherits="Admin_AdminTimeTable"
    EnableSessionState="ReadOnly" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
<script type="text/javascript">
    function confirmDel() {
        if (confirm("Are you sure to delete meeting date?")) {
            return true;
        }
        return false;
    }
</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div>
        <br />
        <div>
            <asp:Label ID="label_currentmeetingdate" runat="server" Text=""></asp:Label>
        </div>
        <div>
            <table>
                <tr>
                    <td colspan="3" align="left">
                        <asp:Label ID="label1" runat="server" Text="Set Meeting Date" Font-Bold="true" Font-Size="Medium"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td colspan="3" align="left">
                        <asp:Label ID="label_meetingdate" runat="server" Text="If you want to search or delete data in one date, <br />Please Set Meeting Date."></asp:Label>
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
                        <asp:Button ID="btn_set" runat="server" Text="Set" Width="60px" OnClick="btn_set_Click" />
                        <asp:Button ID="btn_del" runat="server" Text="Delete" Width="60px" OnClick="btn_del_Click" OnClientClick="return confirmDel();" />
                    </td>
                </tr>
                <tr>
                    <td colspan="3">
                        <br />
                    </td>
                </tr>
                <asp:Panel runat="server" ID="div_dis">
                    <tr>
                        <td colspan="3" align="left">
                            <asp:Label ID="label2" runat="server" Text="Add New Meeting Date" Font-Bold="true"
                                Font-Size="Medium"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="3">
                            <asp:Label ID="label_setnewdate" runat="server" Text="If you want to Add <strong>Next Meeting Date</strong>, <br />Please Select Month."></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            This Year is <asp:Label ID="label_year" runat="server" Text="" ForeColor="Green"></asp:Label>
                        </td>
                        <td>
                            Month:<asp:DropDownList ID="ddlist_month" runat="server" Width="120px">
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:Button ID="btn_ok" runat="server" Text="OK" Width="60px" OnClick="btn_ok_Click" />
                        </td>
                    </tr>
                </asp:Panel>
                <tr>
                    <td colspan="3">
                        <asp:Label ID="label_info" runat="server" Text=""></asp:Label>
                    </td>
                </tr>
                <tr>
            </table>
        </div>
    </div>
</asp:Content>
