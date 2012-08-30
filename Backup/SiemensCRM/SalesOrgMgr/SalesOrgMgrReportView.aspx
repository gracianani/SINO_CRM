<%@ Page Language="C#" Title="Report View List" MasterPageFile="~/SalesOrgMgr/SalesOrgMgrMasterPage.master"
    AutoEventWireup="true" CodeFile="SalesOrgMgrReportView.aspx.cs" Inherits="SalesOrgMgr_SalesOrgMgrReportView" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
<%--BY yyan 20110511 item 57 add start  --%>
<script type="text/javascript">
function share()
{
    var flagShare=window.confirm("Are you sure to Share?");
    if (flagShare)
    {
         return true;
    }
    else
    {
         return false;
    }
}
</script>
<%--BY yyan 20110511 item 57 add end --%>
        <div>
            <asp:Label ID="label_currentmeetingdate" runat="server" Text=""></asp:Label>
        </div>
    <fieldset class="fie">
        <legend><strong>Report View List</strong></legend>
        <table border="0" cellpadding="0" cellspacing="0" width="100%">
            <tr>
                <td align="right">
                    <asp:Button ID="lbtnAdd" Text="New Add" runat="server" OnClick="lbtnAdd_Click"></asp:Button>
                </td>
            </tr>
        </table>
        <asp:GridView Width="100%" ID="gvList" CssClass="c_tab" AllowPaging="true" PageSize="10"
            runat="server" GridLines="None" AutoGenerateColumns="False" OnRowCommand="gvList_RowCommand"
            OnRowDataBound="gvList_RowDataBound" OnPageIndexChanging="gvList_PageIndexChanging">
            <Columns>
                <asp:BoundField HeaderText="Name" DataField="Name">
                    <ItemStyle Width="120" />
                </asp:BoundField>
                <asp:BoundField HeaderText="Description" DataField="Depiction"></asp:BoundField>
                <%--  <asp:BoundField HeaderText="ViewName" DataField="ViewName" >
                     <ItemStyle  Width="120" />
                   </asp:BoundField>--%>
                <asp:BoundField HeaderText="CreateDate" DataField="CreateDate">
                    <ItemStyle Width="100" />
                </asp:BoundField>
                <asp:TemplateField HeaderText="Action" HeaderStyle-HorizontalAlign="center">
                    <ItemTemplate>
                        <asp:ImageButton ID="mbtnEidt" CommandName="EidtReport" AlternateText="Edit" CommandArgument='<%# DataBinder.Eval(Container.DataItem,"ID")%>'
                            ImageUrl="~/images/tablist2_04.png" runat="server" />
                        <asp:ImageButton ID="mbtnDel" CommandName="DelReport" AlternateText="Delete" CommandArgument='<%# DataBinder.Eval(Container.DataItem,"ID")%>'
                            ImageUrl="~/images/tablist2_03.png" runat="server" />
                        <asp:ImageButton ID="mbtnFactor" CommandName="Factor" AlternateText="Select" CommandArgument='<%# DataBinder.Eval(Container.DataItem,"ID")%>'
                            ImageUrl="~/images/tablist2_05.png" runat="server" />
                        <asp:ImageButton ID="mbtnRun" CommandName="Run" AlternateText="Run" CommandArgument='<%# DataBinder.Eval(Container.DataItem,"ID")%>'
                            ImageUrl="~/images/tablist2_06.png" runat="server" />
                        <%--BY yyan item 53 20110513 add start--%>
                        <asp:ImageButton ID="mbtnRename" CommandName="Rename" AlternateText="Rename" CommandArgument='<%# DataBinder.Eval(Container.DataItem,"ID")%>'
                            ImageUrl="~/images/cmm1.png" runat="server" />
                        <%--BY yyan item 53 20110513 add end --%>
                    </ItemTemplate>
                    <%--BY yyan item 53 20110513 del start--%>
                    <%-- <ItemStyle HorizontalAlign="center" Width="70" /> --%>
                    <%--BY yyan item 53 20110513 del end --%>
                    <%--BY yyan item 53 20110513 add start--%>
                    <ItemStyle HorizontalAlign="center" Width="100" />
                    <%--BY yyan item 53 20110513 add end --%>
                </asp:TemplateField>
            </Columns>
            <PagerStyle HorizontalAlign="center" Height="2" Font-Size="X-Small" />
             <PagerTemplate>
             <asp:DropDownList ID="ddlpage" AutoPostBack="True"  runat ="server" OnSelectedIndexChanged="SelectedIndexChanged"></asp:DropDownList>
             </PagerTemplate>
            <EmptyDataRowStyle Font-Overline="false" BorderStyle="None" />
            <EmptyDataTemplate>
                <table width="100%" cellpadding="0" cellspacing="0" border="0">
                    <th width="120">
                        Name</th>
                    <th>
                        Description</th>
                    <th width="160">
                        CreateDate</th>
                    <th width="140">
                        Action</th>
                </table>
            </EmptyDataTemplate>
        </asp:GridView>
    </fieldset>
</asp:Content>
