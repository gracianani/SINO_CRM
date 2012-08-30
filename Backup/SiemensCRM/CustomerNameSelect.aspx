<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CustomerNameSelect.aspx.cs"
    Inherits="CustomerNameSelect" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <base target="_self" />
    <title>Please Select Customer</title>

    <script type="text/javascript" language="javascript">
    function setDefault() {
        var argu = dialogArguments;
        if (argu != "") {
            var objs = document.getElementsByName("rdoCusName");
            for (var i = 0; i < objs.length; i++) {
                if (objs[i].value == argu) {
                    objs[i].checked = true;
                }
            }
        }
    }
    
    function sendValue(obj) {
        var id = obj.value;
        var name = document.getElementById("hidCusName" + id).value;
        var array = new Array();
        array.push(id);
        array.push(name);
        window.returnValue = array;
    }
    </script>

</head>
<body style="text-align: center; margin: 0px;" onload="setDefault();">
    <form id="form1" runat="server">
        <table width="100%" border="0" cellpadding="0" cellspacing="5">
            <tr>
                <td>
                    <strong>
                        <asp:Label ID="lbl_selectInfo" runat="server" Text="Please Select Customer" Style="font-size: medium;" /></strong>
                </td>
            </tr>
            <tr>
                <td align="left">
                    Customer Name:
                    <asp:TextBox ID="txtCusNameQuery" runat="server" />
                    <asp:HiddenField ID="hidCusNameQuery" runat="server" />
                    <asp:Button ID="btnQuery" runat="server" Text="Query" OnClick="btnQuery_Click" />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:GridView ID="gvCusName" runat="server" AutoGenerateColumns="false" GridLines="None"
                        EmptyDataText="There is no related customer found in the system!" ForeColor="#333333"
                        Width="100%" AllowPaging="true" PageSize="20" OnPageIndexChanging="gvCusName_PageIndexChanging">
                        <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                        <RowStyle BackColor="#EFF3FB" />
                        <AlternatingRowStyle BackColor="White" />
                        <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                        <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                        <Columns>
                            <asp:TemplateField HeaderStyle-Width="10%" ItemStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <input type="radio" name="rdoCusName" value='<%# Eval("ID") %>' onclick='sendValue(this)' />
                                    <input type="hidden" name="hidCusName" id='hidCusName<%# Eval("ID") %>' value='<%# Eval("Customer Name") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="Customer Name" HeaderText="Customer Name" HeaderStyle-Width="30%" />
                            <asp:BoundField DataField="Customer Type" HeaderText="Customer Type" HeaderStyle-Width="30%" />
                            <asp:BoundField DataField="SubRegion Name" HeaderText="SubRegion Name" HeaderStyle-Width="30%" />
                        </Columns>
                    </asp:GridView>
                </td>
            </tr>
            <tr>
                <td align="center">
                    <asp:Button ID="btn_submit" runat="server" Text="OK" Width="80px" OnClientClick="window.close();" />
                    <asp:Button ID="btn_cancel" runat="server" Text="Cancel" Width="80px" OnClientClick="window.returnValue=null;window.close();" />
                </td>
            </tr>
        </table>
    </form>
</body>
</html>
