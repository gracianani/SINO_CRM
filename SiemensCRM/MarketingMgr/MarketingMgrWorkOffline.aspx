<%@ Page Title="Work Offline" Language="C#" MasterPageFile="~/MarketingMgr/MarketingMgrMasterPage.master"
    AutoEventWireup="true" CodeFile="MarketingMgrWorkOffline.aspx.cs" Inherits="MarketingMgr_MarketingWorkOffline" %>

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
        </table>
    </div>
</asp:Content>
