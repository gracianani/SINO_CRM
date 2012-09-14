<%@ Page Language="C#" AutoEventWireup="true" CodeFile="RCOBEdit.aspx.cs" Inherits="RCOBEdit" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Please Input Data</title>

    <script type="text/javascript" language="javascript">
    function getFinalAmount() {
        var amount = document.getElementById("txtAmount").value;
        var percentage = document.getElementById("txtPercentage").value;
        var pattern = /^\d*[.]{0,1}\d+$/;
        //yyan 20110907 itemw135 edit start
        if (pattern.exec(amount) == null ) {
            document.getElementById("txtFAmount").value = "0";
        }else if ( pattern.exec(percentage) == null) {
           document.getElementById("txtFAmount").value =amount
        //yyan 20110907 itemw135 edit end
        } else {
            amount = parseFloat(amount);
            percentage = parseFloat(percentage);
            document.getElementById("txtFAmount").value = amount + (amount * percentage / 100);
        }
    }
    
    function sendValue() {
        var amount = document.getElementById("txtAmount").value;
        var percentage = document.getElementById("txtPercentage").value;
        var finalAmount = document.getElementById("txtFAmount").value;
        var pattern = /^\d*[.]{0,1}\d+$/;
   //     if (pattern.exec(amount) == null) {
   //         alert("Please input correct amount!");
   //     } else if (pattern.exec(percentage) == null) {

   
        //Edit by Sino Bug28 
        //debugger;
        if(percentage==null || percentage=='')
        {
            alert("Mark up value is not allow empty!");
            return;
        }
        
//        if(percentage==0)
//        {
//            alert("Mark up value is not allow zero!");
//            return;
//        }
        if(pattern.exec(percentage)==null)
        {
            alert("Please input correct Mark up value!");
            return;
        }
        
        //Edit by Sino Bug28
        if (pattern.exec(amount) == null) 
        {
            alert("Please input correct amount!");
        } 
        else if (pattern.exec(percentage) == null) {
            //yyan 20110907 itemw135 edit start
            //alert("Please input correct percentage!");
            var array = new Array();
            array.push(amount);
            array.push(percentage);
            array.push(amount);
            window.returnValue = array;
            window.close();
            //yyan 20110907 itemw135 edit end
        } else {
            var array = new Array();
            array.push(amount);
            array.push(percentage);
            array.push(finalAmount);
            window.returnValue = array;
            window.close();
        }
    }
    
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
        //RCOBEdit.bindDataSource(array, receiveInitPage);
        document.getElementById("txtAmount").value = paramArr[13];
        if (paramArr[14] == "") {
            paramArr[14] = "0";
        }
        document.getElementById("txtPercentage").value = paramArr[14];
        document.getElementById("txtFAmount").value = paramArr[15];
    }
    
    function receiveInitPage(result) {
        var value = result.value;
        var paramArr = window.dialogArguments;
        console.log(paramArr);


    }
    </script>

</head>
<body style="text-align: center; margin: 0px;" onload="sendInitPage();">
    <form id="form1" runat="server">
        <div style="padding-top:15px;">
            <table width="100%" border="1" cellpadding="2" cellspacing="0">
                <tr>
                    <th align="right">
                        GA-Price:
                    </th>
                    <td align="left">
                        <asp:TextBox ID="txtAmount" runat="server" />
                    </td>
                </tr>
                <tr>
                    <th align="right">
                        Mark up (%):
                    </th>
                    <td align="left">
                        <asp:TextBox ID="txtPercentage" runat="server" Width="70" />
                    </td>
                </tr>
                <tr>
                    <th align="right">
                         K-Price:
                    </th>
                    <td align="left">
                        <asp:TextBox ID="txtFAmount" runat="server" />
                    </td>
                </tr>
            </table>
            <div style="text-align: center; padding-top:20px; margin-top: 2px; height: 50px">
                <input type="button" id="btnConfirm" name="btnConfirm" value="OK" onclick="sendValue();" />
                <input type="button" id="btnCancel" name="btnConfirm" value="Cancel" onclick="window.close();" />
            </div>
        </div>
    </form>
</body>
</html>
