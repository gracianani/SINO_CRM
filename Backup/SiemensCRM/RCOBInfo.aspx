<%@ Page Language="C#" AutoEventWireup="true" CodeFile="RCOBInfo.aspx.cs" Inherits="RCOBInfo" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Please Input Data</title>

    <script type="text/javascript" language="javascript">
    function sendInitPage() {
        var paramArr = window.dialogArguments;
        var array = new Array();
        array.push(paramArr[0]);
        array.push(paramArr[1]);
        array.push(paramArr[2]);
        array.push(paramArr[3]);
        array.push(paramArr[4]);
        array.push(paramArr[5]);
        array.push(paramArr[6]);
        array.push(paramArr[7]);
        array.push(paramArr[8]);
        array.push(paramArr[9]);
        array.push(paramArr[10]);
        array.push(paramArr[11]);
        array.push(paramArr[12]);
        RCOBInfo.bindDataSource(array, receiveInitPage);
    }
    
    function receiveInitPage(result) {
        var value = result.value;
        document.getElementById("lblAmount").innerHTML = value[2];
        if (value[1] != "")
        {
            document.getElementById("lblPercentage").innerHTML = value[1] + "%";
        }
        document.getElementById("lblFamount").innerHTML = value[0];
    }
    </script>

</head>
<body style="text-align: center; margin: 0px;" onload="sendInitPage();">
    <form id="form1" runat="server">
        <div style="padding-top:15px;">
            <table width="100%" border="1" cellpadding="2" cellspacing="0">
                <tr>
                    <th align="right" style="width:120px">
                        GA-Price:
                    </th>
                    <td align="left">
                        <asp:Label ID="lblAmount" runat="server" />
                    </td>
                </tr>
                <tr>
                    <th align="right">
                        Mark up (%):
                    </th>
                    <td align="left">
                        <asp:Label ID="lblPercentage" runat="server" />
                    </td>
                </tr>
                <tr>
                    <th align="right">
                        K-Price:
                    </th>
                    <td align="left">
                        <asp:Label ID="lblFamount" runat="server" />
                    </td>
                </tr>
            </table>
            <div style="text-align: center; padding-top:20px; margin-top: 2px; height: 50px">
                <input type="button" id="btnCancel" name="btnConfirm" value="Cancel" onclick="window.close();" />
            </div>
        </div>
    </form>
</body>
</html>
