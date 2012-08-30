<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SiemensCRMEnter.aspx.cs"
    Inherits="SiemensCRMEnter" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>SiemensCRM Tools Login</title>
</head>
<body>
    <asp:Panel ID="Panel3" runat="server" Width="100%" BackImageUrl="~/images/bg.jpg"
        Style="background-repeat: repeat-x; background-attachment: scroll; background-position: top;
        margin: auto;" >
        <table width="100%" style="text-align: center;" border="1">
            <tr valign="top">
                <td>
                </td>
                <td style="width: 1024px" align="left">
                    <form id="form1" runat="server" target="_self">
                    <div align="center">
                        <asp:Panel ID="Panel1" runat="server" BackImageUrl="~/images/logo.JPG" Height="108px">
                            <br />
                            <br />
                        </asp:Panel>
                        <hr />
                        <br />
                        <br />
                        <br />
                        <asp:Panel ID="Panel4" runat="server" BackColor="#CCCCCC" BorderColor="White" Height="230px"
                            Width="400px">
                            <br />
                            <div>
                                <strong>
                                    <asp:Label ID="lbl_welcom" runat="server" Text="Welcome to Siemens HP CRM Tools."></asp:Label>
                                </strong>
                            </div>
                            <br />
                            <br />
                            <br />
                            <div>
                                <asp:Label ID="lbl_user" runat="server" Text="User Account" Width="120px"></asp:Label>
                                <asp:TextBox ID="tbox_user" runat="server" Width="120px"></asp:TextBox>
                                <br />
                                <br />
                                <asp:Label ID="lbl_password" runat="server" Text="User Password" Width="120px"></asp:Label>
                                <asp:TextBox ID="tbox_password" runat="server" Width="120px" MaxLength="12" TextMode="Password"></asp:TextBox>
                                <br />
                                <br />
                                <asp:CheckBox ID="ck_remember" runat="server" Text="Remember account and password" Visible="false" />
                            </div>
                            <br />
                            <div>
                                <asp:Button ID="btn_login" runat="server" Text="Login" Width="60px" OnClick="btn_login_Click" />
                                <br />
                                <asp:Label ID="lbl_error" runat="server" ForeColor="Red" Text=""></asp:Label>
                            </div>
                        </asp:Panel>
                        <br /><br /><br /><br />
                    </div>
                    </form>
                </td>
                <td>
                </td>
            </tr>
        </table>
    </asp:Panel>
</body>
</html>
