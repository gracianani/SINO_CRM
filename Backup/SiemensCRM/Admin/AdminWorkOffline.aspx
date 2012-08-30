<%@ Page Title="Work Offline" Language="C#" MasterPageFile="~/Admin/AdminMasterPage.master"
    AutoEventWireup="true" CodeFile="AdminWorkOffline.aspx.cs" Inherits="Admin_AdminWorkOffline"
    EnableSessionState="ReadOnly" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div>
        <table>
            <tr>
                <td colspan="2" align="center" valign="middle">
                    <asp:Label ID="lbl_headertitle" runat="server" Text="Work Offline" Font-Bold="true"
                        Font-Size="Medium"></asp:Label>
                </td>
            </tr>
            <tr>
                <td align="left">
                    <asp:Label ID="lbl_downloadheader" runat="server" Text="DownLoad" Font-Bold="true"></asp:Label>
                </td>
                <td align="left">
                    <asp:Label ID="lbl_downloadnote" runat="server" Text=""></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                </td>
                <td align="left" valign="middle">
                    <table border="1">
                        <tr>
                            <td>
                                <asp:Label ID="lbl_virtualmachinenote" runat="server" Text="The virtual machine: "></asp:Label>
                            </td>
                            <td>
                                <asp:ImageButton ID="ibtn_virtualmachine" runat="server" ImageUrl="~/images/rar.png"
                                    OnClick="ibtn_virtualmachine_Click" ToolTip="Zip the virtual machine" />
                            </td>
                            <td>
                                <asp:ImageButton ID="ibtn_downloadmachine" runat="server" ImageUrl="~/images/download.png"
                                    OnClick="ibtn_downloadmachine_Click" ToolTip="Download the virtual machine" />
                            </td>
                        </tr>
                        <tr>
                            <td colspan="3">
                                <asp:Label ID="lbl_virtualmachine" runat="server" Text=""></asp:Label>
                                <br />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="lbl_sourcecodenote" runat="server" Text="The installation file: "></asp:Label>
                            </td>
                            <td>
                                <asp:ImageButton ID="ibtn_sourcecode" runat="server" ImageUrl="~/images/rar.png"
                                    OnClick="ibtn_sourcecode_Click" ToolTip="Zip the installation file" />
                            </td>
                            <td>
                                <asp:ImageButton ID="ibtn_downloadcode" runat="server" ImageUrl="~/images/download.png"
                                    OnClick="ibtn_downloadcode_Click" ToolTip="Download the installation file" />
                            </td>
                        </tr>
                        <tr>
                            <td colspan="3">
                                <asp:Label ID="lbl_sourcecode" runat="server" Text=""></asp:Label>
                                <br />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="lbl_databasenote" runat="server" Text="The database file: "></asp:Label>
                            </td>
                            <td>
                                <asp:ImageButton ID="ibtn_database" runat="server" ImageUrl="~/images/rar.png" OnClick="ibtn_database_Click"
                                    ToolTip="Zip the database file" />
                            </td>
                            <td>
                                <asp:ImageButton ID="ibtn_downloaddatabase" runat="server" ImageUrl="~/images/download.png"
                                    OnClick="ibtn_downloaddatabase_Click" ToolTip="Download the database file" />
                            </td>
                        </tr>
                        <tr>
                            <td colspan="3">
                                <asp:Label ID="lbl_database" runat="server" Text=""></asp:Label>
                                <br />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <asp:Panel ID="panel_readonly" runat="server">
                <tr>
                    <td align="left">
                        <asp:Label ID="lbl_uploadheader" runat="server" Text="UpLoad" Font-Bold="true"></asp:Label>
                    </td>
                    <td align="left">
                        <asp:Label ID="lbl_uploadnote" runat="server" Text=""></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                    </td>
                    <td align="left" valign="middle">
                        <asp:Label ID="label_serverIP" runat="server" Text="Server IP:"></asp:Label>&nbsp;
                        <asp:TextBox ID="tbox_serverIP" runat="server" ToolTip="Please input IP address of server"></asp:TextBox>&nbsp;
                        <asp:Button ID="btn_ok" runat="server" Text="Check IP" Width="80px" OnClick="btn_ok_Click" />
                        <asp:Label ID="label_check" runat="server" Text="" ForeColor="Red"></asp:Label>
                        <br />
                        <br />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:Button ID="btn_upload" runat="server" Text="Upload Database" Width="150px" OnClick="btn_upload_Click"
                            Enabled="false" />
                        <asp:Label ID="label_upnote" runat="server" Text="" ForeColor="Red"></asp:Label>
                    </td>
                </tr>
            </asp:Panel>
        </table>
    </div>
</asp:Content>
