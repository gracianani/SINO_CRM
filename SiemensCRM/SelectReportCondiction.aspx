<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SelectReportCondiction.aspx.cs" Inherits="SelectReportCondiction" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Select Report Filter Condition</title>
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
    <script type="text/javascript" src="./js/report.js"></script>
</head>
<body>
    <form id="form1" runat="server">
<br />
    <div style="overflow: scroll;">
        <table width="100%" border="0" cellpadding="0" cellspacing="0" visible="true">
          <tr >            
            <td style="height:30px">
            <textarea name="sendto" class="selectbox" rows="5"  cols="50" id="sendto" runat="server" style="width:100%;height:100%" readonly></textarea>              
            </td>           
          </tr>
        </table>
       
        <asp:GridView ID="gv_ReportCondiction" runat="server" CellPadding="4" ForeColor="#333333"
            GridLines="None" OnPageIndexChanging="gv_ReportCondiction_PageIndexChanging"  OnRowDataBound="gv_ReportCondiction_RowDataBound"
             AllowPaging="false" >
            <RowStyle BackColor="#EFF3FB" />
            <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
            <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
            <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
            <EditRowStyle BackColor="#33CCCC" />
            <AlternatingRowStyle BackColor="White" />
            <Columns>
                 <asp:TemplateField> 
                <ItemTemplate>
                    <asp:CheckBox runat="server" ID="chkCheck" OnCheckedChanged="chkCheck_CheckedChanged"  AutoPostBack="true"/> 
                    <asp:HiddenField ID="hdShowValue" runat="server" Value='<%#DataBinder.Eval(Container.DataItem,"ShowValue")%>' />
                </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
        <div  >
            <input type="button"  name="Btn_OK" id="Btn_OK" value="OK" onclick="divclose()" style="border:1px solid #ccc; background:transparent;" />
            <input type="button" name="Btn_Cancel" id="Btn_Cancel"  value="Cancel" onclick="parent.parent.hidemask()" style="border:1px solid #ccc; background:transparent;"/>
        </div>
    </div>
    <input type="hidden" id="hidFieldName" runat="server" value=""/>
     <script type="text/jscript" language="javascript" >
     function divclose()
     {
        var texts  = document.getElementById("sendto").value;        
        parent.parent.tohidemask(texts);
     }
    </script>

    </form>
</body>
</html>
