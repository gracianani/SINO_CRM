<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AccessDenied.aspx.cs" Inherits="AccessDenied" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Access Denied</title>
</head>
<body>
    <form id="form1" runat="server">
    <div  style="font-size: medium; padding-left:12px">
        <br /><br />
        <strong>Access Denied</strong> 
    </div>
    <div style="padding-left:12px">
        <p>
        You do not have permission to access this site.  
        <br />
        If you think that you have received this message in error please send email to 
            <asp:Label ID="label_display" runat="server" Text=""></asp:Label>
        </p>
    </div>
    </form>
</body>
</html>
