<%@ Page Language="C#" Title="Filter" AutoEventWireup="true" CodeFile="AssistantStandardReportRename.aspx.cs"
    Inherits="Assistant_AssistantStandardReportRename" MasterPageFile="~/Assistant/AssistantMasterPage.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <link href="../../default/main.css" type="text/css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <fieldset class="fie">
        <%--by yyan 20110530 item w17 del start--%>
        <%--<legend><strong>Filter</strong></legend>--%>
        <%--by yyan 20110530 item w17 del end--%>
        <%--by yyan 20110530 item w17 add start--%>
        <legend><strong>Rename</strong></legend>
        <%--by yyan 20110530 item w17 add end--%>
        <div id="divError" runat="server" />
        <asp:GridView ID="GridView1" runat="server" CssClass="c_tab" Width="100%" CellSpacing="0"
            CellPadding="4" AutoGenerateColumns="false" >
            <Columns>
                <asp:BoundField HeaderText="Name" DataField="FieldName" SortExpression="FieldName">
                    <HeaderStyle Width="15%" />
                </asp:BoundField>
              
                <asp:TemplateField HeaderText="NEW NAME">
                    <ItemTemplate>
                        <asp:TextBox ID="txtName" runat="server" Width="310" MaxLength="100" />
                        <asp:HiddenField ID="hidFieldID" runat="server" Value='<%# Eval("FieldID") %>' />
                       <asp:HiddenField ID="hidReportID" runat="server" Value='<%# Eval("ID") %>' />
                    </ItemTemplate>
                    <HeaderStyle Width="40%" />
                </asp:TemplateField>
              
                       
                    
            </Columns>
        </asp:GridView>
        <p>
            <asp:Button ID="btnConfrim" runat="server" Text="OK" OnClick="btnConfrim_Click" />
            <asp:Button ID="btnCancel" runat="server" Text="Cancel" OnClick="btnCancel_Click" />
        </p>
    </fieldset>
    
    <input type="hidden" id="clicktextid" runat="server" value=""/>
    <input type="hidden" id="clickvalueid"  runat="server" value=""/>
    <div id="cer">
    <script type="text/javascript" src="../js/report.js"></script>    
   
    </div>
</asp:Content>
