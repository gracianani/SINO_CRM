<%@ Page Language="C#" Title="Report View List" MasterPageFile="~/RSM/RSMMasterPage.master"
    AutoEventWireup="true" CodeFile="RSMStandardReportView.aspx.cs" Inherits="RSMStandardReportView" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
        <div>
            <asp:Label ID="label_currentmeetingdate" runat="server" Text=""></asp:Label>
        </div>
    <fieldset class="fie">
        <legend><strong>Report View List</strong></legend>
        <table border="0" cellpadding="0" cellspacing="0" width="100%">

        </table>
        <div id="divError" runat="server" />
        <asp:GridView Width="100%" ID="gvList" CssClass="c_tab" AllowPaging="true" PageSize="10"
            runat="server" GridLines="None" AutoGenerateColumns="False" OnRowCommand="gvList_RowCommand"
            OnRowDataBound="gvList_RowDataBound" OnPageIndexChanging="gvList_PageIndexChanging">
            <Columns>
                <asp:BoundField HeaderText="Name" DataField="Name">
                    <ItemStyle Width="120" />
                </asp:BoundField>
                <asp:BoundField HeaderText="Description" DataField="Depiction"></asp:BoundField>
                <asp:BoundField HeaderText="CreateDate" DataField="CreateDate">
                    <ItemStyle Width="100" />
                </asp:BoundField>
                <asp:TemplateField HeaderText="Action" HeaderStyle-HorizontalAlign="center">
                    <ItemTemplate>
                    
                        <asp:ImageButton ID="mbtnFactor" CommandName="Factor" AlternateText="Select" CommandArgument='<%# DataBinder.Eval(Container.DataItem,"ID")%>'
                            ImageUrl="~/images/tablist2_05.png" runat="server" />
                        <asp:ImageButton ID="mbtnRun" CommandName="Run" AlternateText="Run" CommandArgument='<%# DataBinder.Eval(Container.DataItem,"ID")%>'
                            ImageUrl="~/images/tablist2_06.png" runat="server" />
                     </ItemTemplate>
                    <ItemStyle HorizontalAlign="center" Width="60" />
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
