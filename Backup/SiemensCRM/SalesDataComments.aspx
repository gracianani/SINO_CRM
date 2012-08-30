<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SalesDataComments.aspx.cs"
    Inherits="SalesDataComments" EnableSessionState="ReadOnly" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Comments</title>
</head>
<body>
    <form id="form1" runat="server" target="_self">
        <br />
        <br />
        <div>
            <asp:Label ID="label_header" runat="server" Text="Sales Data/Backlog and Comments" Font-Bold="True"
                Font-Size="Medium"></asp:Label>
        </div>
        <table>
            <tr>
                <td>
                    <asp:Label ID="label_note" runat="server" Text="" ForeColor="Red"></asp:Label>
                </td>
            </tr>
            <tr>
                <asp:Panel ID="panel_comments" runat="server">
                    <td>
                        <asp:Label ID="label_project" runat="server" Text="Product" Width="200px"></asp:Label>
                        <asp:DropDownList ID="ddlist_product" runat="server" Width="200px" OnSelectedIndexChanged="ddl_SelectedProductChanged"
                            AutoPostBack="True">
                        </asp:DropDownList>
                        <br />
                        <asp:Label ID="label_input" runat="server" Text="Input Information" Width="200px"></asp:Label><br />
                        <textarea id="content" name="TEXTAREA1" rows="4" cols="65" runat="server" style="width: 100%"></textarea>
                    </td>
                </asp:Panel>
            </tr>
            <tr>
                <td>
                    <asp:Button ID="btn_ok" runat="server" Text="Save comments" OnClick="btn_ok_Click"
                        Width="130px" />
                    <asp:Button ID="btn_close" runat="server" Text="Close" Width="60px" OnClientClick="window.close();return false;"/>
                </td>
            </tr>
        </table>
    </form>
</body>
</html>
