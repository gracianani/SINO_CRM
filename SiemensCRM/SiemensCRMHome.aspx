<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SiemensCRMHome.aspx.cs" Inherits="SiemensCRMHome" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>SiemensCRM Tools Login Page</title>
</head>
<body>
    <asp:Panel ID="Panel3" runat="server" Width="100%" BackImageUrl="~/images/bg.jpg"
        Style="background-repeat: repeat-x; background-attachment: scroll; background-position: top;
        margin: auto;" Height="600px">
        <table width="100%" style="text-align: center;">
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
                        <asp:Panel ID="Panel4" runat="server" BackColor="#CCCCCC" BorderColor="White" 
                            Height="140px" Width="400px">
                            <br />
                            <div>
                                <strong>
                                    <asp:Label ID="lbl_welcom" runat="server" Text="Welcome Alias"></asp:Label>
                                </strong>
                            </div>
                            <br />
                            <div>
                                <asp:Label ID="lbl_role" runat="server" Text="Select one role, please."></asp:Label>
                                <br />
                                <asp:DropDownList ID="ddlist_role" runat="server" Width="160px">
                                </asp:DropDownList>
                            </div>
                            <br />
                            <div>
                                <asp:Button ID="btn_login" runat="server" Text="Login" Width="60px" 
                                    onclick="btn_login_Click" />
                                <asp:Button ID="btn_off" runat="server" Text="Logoff" Width="60px" 
                                    onclick="btn_off_Click" />
                                <br />
                                <asp:Label ID="lbl_error" runat="server" ForeColor="Red" Text=""></asp:Label>
                            </div>
                        </asp:Panel>
                        <br />
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
