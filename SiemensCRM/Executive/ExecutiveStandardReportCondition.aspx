<%@ Page Language="C#" Title="Filter" AutoEventWireup="true" CodeFile="ExecutiveStandardReportCondition.aspx.cs"
    Inherits="Executive_ExecutiveStandardReportCondition" MasterPageFile="~/Executive/ExecutiveMasterPage.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <link href="../../default/main.css" type="text/css" rel="stylesheet" />
    <script type="text/javascript" language="javascript">
        function initCondition()
        {
            var selectArray = document.getElementsByTagName("select");
            var inputArray = document.getElementsByTagName("input");
            
            var idTemp = null;
            for(var i = 0; i < selectArray.length; i++)
            {
                idTemp = selectArray[i].id
                if (idTemp.indexOf("selOperator") != -1)
                {
                    idTemp = idTemp.substring(0, idTemp.indexOf("selOperator"));
                    if (document.getElementById(idTemp + "hidFieldType").value == "DateTime"
                        && selectArray[i].value == "8")
                    {
                        document.getElementById(idTemp + "txtCondition1").style.width = "150";
                        document.getElementById(idTemp + "txtCondition2").style.display = "";
                    }
                    else
                    {
                        document.getElementById(idTemp + "txtCondition1").style.width = "295";
                        document.getElementById(idTemp + "txtCondition2").style.display = "none";
                        document.getElementById(idTemp + "txtCondition2").value = "";
                    }
                    if(document.getElementById(idTemp + "selOperator").value == 9)
                    {
                        document.getElementById(idTemp + "imgSelect").style.display = ""; 
                        document.getElementById(idTemp + "txtCondition1").readOnly = true;
                    }
                    else
                    {
                        document.getElementById(idTemp + "imgSelect").style.display = "none";
                        document.getElementById(idTemp + "txtCondition1").readOnly = false;
                    }
                }
            }
        }
        
        function OperatorChange(operatorObj)
        {
            idTemp = operatorObj.id.substring(0, operatorObj.id.indexOf("selOperator"));
            if (document.getElementById(idTemp + "hidFieldType").value == "DateTime"
                && operatorObj.value == "8")
            {
                document.getElementById(idTemp + "txtCondition1").style.width = "150";
                document.getElementById(idTemp + "txtCondition2").style.display = "";
                document.getElementById(idTemp + "imgSelect").style.display = "none";
            }
            else
            {
                document.getElementById(idTemp + "txtCondition1").style.width = "295";
                document.getElementById(idTemp + "txtCondition2").style.display = "none";
                document.getElementById(idTemp + "txtCondition2").value = "";
            }
            if (operatorObj.value == 9)
            {
               document.getElementById(idTemp + "imgSelect").style.display = ""; 
               document.getElementById(idTemp + "txtCondition1").readOnly = true;
               var Condition1V = document.getElementById(idTemp + "txtCondition1").value;
               if (Condition1V.length>0)
               {
               
                 if (((Condition1V.substr(0,1) != "'") || 
                      (Condition1V.substr(Condition1V.length-1,1) != "'")) || 
                    (Condition1V.length<2))
                 {
                    document.getElementById(idTemp + "txtCondition1").value = "";
                 }
                 // 字符'是否成对出现
                 if ((Condition1V.split("'").length-1)%2)
                 {
                    document.getElementById(idTemp + "txtCondition1").value = "";
                 } 
                 // 字符,是否连续出现两个以上
                 if (Condition1V.indexOf(",,") > 0)                
                 {
                    document.getElementById(idTemp + "txtCondition1").value = "";
                 }
               }               
            } 
            else
            {
                document.getElementById(idTemp + "imgSelect").style.display = "none";
                document.getElementById(idTemp + "txtCondition1").readOnly = false;
            }
        }
        
        function CleanRowDate(id)
        {
            var idTemp = id.substring(0, id.indexOf("imgClean"));
            document.getElementById(idTemp + "selOperator").value = "";
            document.getElementById(idTemp + "txtCondition1").value = "";
            document.getElementById(idTemp + "txtCondition2").value = "";
            document.getElementById(idTemp + "selSortierung").value = "0";
            document.getElementById(idTemp + "txtCondition1").style.width = "295";
            document.getElementById(idTemp + "txtCondition2").style.display = "none";
        }
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <fieldset class="fie">
        <legend><strong>Filter</strong></legend>
        <div id="divError" runat="server" />
        <asp:GridView ID="GridView1" runat="server" CssClass="c_tab" Width="100%" CellSpacing="0"
            CellPadding="4" AutoGenerateColumns="false" OnRowDataBound="GridView1_RowDataBound">
            <Columns>
                <asp:BoundField HeaderText="Name" DataField="FieldName" SortExpression="FieldName">
                    <HeaderStyle Width="15%" />
                </asp:BoundField>
                <asp:TemplateField HeaderText="Operator">
                    <ItemTemplate>
                        <asp:DropDownList ID="selOperator" runat="server" Width="130" />
                    </ItemTemplate>
                    <HeaderStyle Width="20%" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Condition">
                    <ItemTemplate>
                        <asp:TextBox ID="txtCondition1" runat="server" Width="310" MaxLength="100" />
                        <asp:TextBox ID="txtCondition2" runat="server" Width="150" MaxLength="100" />
                        <img id="imgSelect" class="hand" src ="../images/nkk_03.png"  runat="server" alt="Multi Select"/>
                    </ItemTemplate>
                    <HeaderStyle Width="40%" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Sort">
                    <ItemTemplate>
                        <asp:DropDownList ID="selSortierung" runat="server" Width="130">
                            <asp:ListItem Value="0" Text="Please select" />
                            <asp:ListItem Value="1" Text="Asc" />
                            <asp:ListItem Value="2" Text="Desc" />
                        </asp:DropDownList>
                        <img id="imgClean" runat="server" alt="Clean" class="hand" src="../images/tablist_11.png"
                            width="9" height="11" onclick="CleanRowDate(this.id);" />
                        <asp:HiddenField ID="hidFieldID" runat="server" Value='<%# Eval("FieldID") %>' />
                        <asp:HiddenField ID="hidFieldType" runat="server" Value='<%# Eval("FieldType") %>' />
                        <asp:HiddenField ID="hidFieldLength" runat="server" Value='<%# Eval("FieldLength") %>' />
                        <asp:HiddenField ID="hidReportID" runat="server" Value='<%# Eval("ID") %>' />
                        <asp:HiddenField ID="HidNewFieldName" runat="server" Value='<%# Eval("NewFieldName") %>' />
                        <%--by yyan 20110518 item 53 add start --%>
                        <asp:HiddenField ID="hidCondition1" runat="server" Value='<%# Eval("Condition1") %>' />
                        <asp:HiddenField ID="hidCondition2" runat="server" Value='<%# Eval("Condition2") %>' />
                        <%--by yyan 20110518 item 53 add end --%>
                    </ItemTemplate>
                    <HeaderStyle Width="20%" />
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
        <p>
            <%--by yyan 20110518 item 53 del start --%>
            <%--<asp:Button ID="btnConfrim" runat="server" Text="OK" OnClick="btnConfrim_Click" />--%>
            <%--by yyan 20110518 item 53 del end --%>
            <%--by yyan 20110518 item 53 add start --%>
            <asp:Button ID="btnConfrim" runat="server" Text="Save As" OnClick="btnConfrim_Click" />
            <%--by yyan 20110518 item 53 add end --%>
            <asp:Button ID="btnCancel" runat="server" Text="Cancel" OnClick="btnCancel_Click" />
        </p>
        <%--by yyan 20110518 item 53 add start --%>
        <asp:Panel ID="panel_add" runat="server">
            <table>
                <tr>
                    <td>
                        <asp:Label ID="label_Name" runat="server" Text="ReportName:" Width="100px"></asp:Label>
                        <asp:TextBox ID="tbox_Name" runat="server" Width="140px"></asp:TextBox>
                        <asp:Button ID="btn_AddName" runat="server" Text="Add"  OnClick="btnConfrim1_Click" Width="60px" />
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <%--by yyan 20110518 item 53 add end --%>
    </fieldset>
    <%--by yyan 20110518 item 53 add start --%>
    <asp:HiddenField ID="hidViewName" runat="server" />
    <%--by yyan 20110518 item 53 add end --%>
    <input type="hidden" id="clicktextid" runat="server" value=""/>
    <input type="hidden" id="clickvalueid"  runat="server" value=""/>
    <div id="cer">
    <script type="text/javascript" src="../js/report.js"></script>    
    <script type="text/javascript" language="javascript">
        initCondition(); 
        drawmask(); 
    </script>
    </div>
</asp:Content>
