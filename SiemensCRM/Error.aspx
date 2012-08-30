<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Error.aspx.cs" Inherits="Error" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Siemens HP CRM Tool - Server busy</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <br /><br />
        <asp:Label ID="label_error" runat="server" Text="ERROR" ForeColor="Red" Font-Size="Medium" Font-Bold="True"></asp:Label>
        <br />
        <br />
        <asp:Label ID="label_note" runat="server" Text=""></asp:Label>
    </div>
    </form>
</body>
</html>
