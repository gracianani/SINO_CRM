<%@ Page Language="C#" AutoEventWireup="true" CodeFile="RSMHelp.aspx.cs" Inherits="RSM_RSMHelp" EnableSessionState="ReadOnly" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Help</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <br />
        <br />
        <table>
            <tr>
                <td align="left" valign="top">
                    Find Help (Account as an example)
                    <br />
                </td>
            </tr>
            <tr>
                <td align="left" valign="top">
                    Find will help to filter through the listing of documents to help you find the information
                    <br />
                    you are looking for quickly and easily. Use these instructions to help you maximize
                    the
                    <br />
                    effectiveness of this feature.
                </td>
            </tr>
            <tr>
                <td align="left" valign="top">
                    <asp:Image ID="img_find" runat="server" ImageUrl="~/images/find.JPG" />
                </td>
            </tr>
            <tr>
                <td align="left" valign="top">
                    <asp:Image ID="img_find1" runat="server" ImageUrl="~/images/find1.JPG" />
                </td>
            </tr>
            <tr>
                <td align="left" valign="top">
                    Find Field: The Find field is located on the left side of the list below the instructions.
                    <br />
                    Type one or more terms in the Find box, and select the correct drop down for the
                    “in”
                    <br />
                    field.
                </td>
            </tr>
            <tr>
                <td align="left" valign="top">
                    <asp:Image ID="img_in" runat="server" ImageUrl="~/images/in.JPG" />
                </td>
            </tr>
            <tr>
                <td align="left" valign="top">
                    In Field: This field is a listing of the column headings in the list. After entering
                    a term(s)
                    <br />
                    into the “Find” field you then need to select the column heading you want to look
                    for the
                    <br />
                    information in. After selecting the appropriate column heading within the drop down
                    click the
                    <br />
                    Find button. You will then get a listing of documents related to your search.
                </td>
            </tr>
            <tr>
                <td align="left" valign="top" bgcolor="#99FF99">
                    Notice: If you select "Gender" ,you only input "Male" or "Female" in Find Box.
                </td>
            </tr>
            <tr>
                <td align="center" valign="middle">
                    <br />
                    <asp:Button ID="btn_close" runat="server" Text="Close" Width="60px" BackColor="#66CCFF"
                        OnClick="btn_close_Click" />
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
