<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CustomerDetail.aspx.cs" Inherits="CustomerDetail" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Customer Detail</title>
<style>
body{TEXT-ALIGN: center;}
#center{ MARGIN-RIGHT: auto;
MARGIN-LEFT: auto; 
height:200px;
width:400px;
vertical-align:middle;
line-height:200px;
}
</style>

</head>
<body>
    <form id="form1" runat="server">
    <br />
    <div style="overflow: scroll;">
        <asp:GridView ID="gv_Customer" runat="server" CellPadding="4" ForeColor="#333333"
            GridLines="None" 
            PageSize="10" OnRowDataBound="gv_Customer_RowDataBound" 
            >
            <RowStyle BackColor="#EFF3FB" />
            <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
            <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
            <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
            <EditRowStyle BackColor="#33CCCC" />
            <AlternatingRowStyle BackColor="White" />
        </asp:GridView>
        <br />
        <div id="center" >
            <input type="button" id="Close"  value="Close" onclick="window.close()" style="border:1px solid #ccc; background:transparent;"/>
        </div>
    </div>
    </form>
</body>
</html>
