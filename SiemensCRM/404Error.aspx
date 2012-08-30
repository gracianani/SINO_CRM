<%@ Page Language="C#" AutoEventWireup="true" CodeFile="404Error.aspx.cs" Inherits="_404Error" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <br />
    <h1>
        <strong>File Not Found </strong>
    </h1>
    <br />
    <br />
    The resource cannot be found.
    <br />
    Description: HTTP 404.
    <br />
    The resource you are looking for (or one of its dependencies) could have been removed,
    renamed, or is temporarily unavailable.
    <br />
    <br />
    Please report this error to 
        <asp:Label ID="label_email" runat="server" Text=""></asp:Label>
    </div>
    </form>
</body>
</html>
