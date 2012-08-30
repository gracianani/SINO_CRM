<%@ Page Title="Operation" Language="C#" MasterPageFile="~/Assistant/AssistantMasterPage.master"
    AutoEventWireup="true" CodeFile="AssistantOperation.aspx.cs" Inherits="AssistantOperation"
    EnableSessionState="ReadOnly" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
        <asp:Label ID="label_find" runat="server" Text="Search:"></asp:Label>
        <asp:DropDownList ID="ddlist_find" runat="server">
        </asp:DropDownList>
        <asp:Label ID="label_in" runat="server" Text="IN"></asp:Label>
        <asp:DropDownList ID="ddlist_in" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlist_in_SelectedIndexChanged">
        </asp:DropDownList>
        <asp:Button ID="btn_find" runat="server" Text="Search" Width="60px" OnClick="btn_find_Click" />
       <%-- <asp:LinkButton ID="lbtn_findhelp" runat="server" OnClick="lbtn_findhelp_Click">Search help</asp:LinkButton>--%>
    </ContentTemplate>
            <Triggers>
                <asp:PostBackTrigger ControlID="btn_find" />
            </Triggers>
        </asp:UpdatePanel>
    </div>
    <br />
    <div align="center">
        <asp:Label ID="label_head" runat="server" Text="Manufacturing Operation" Font-Bold="True"
            Font-Size="Medium"></asp:Label>
    </div>
    <div style="overflow: auto; height: 240px; width: 767px; position: relative; padding-top: 22px;">
        <asp:GridView ID="gv_Operation" runat="server" CellPadding="4" ForeColor="#333333"
            GridLines="None" OnPageIndexChanging="gv_Operation_PageIndexChanging" OnRowCancelingEdit="gv_Operation_RowCancelingEdit"
            OnRowEditing="gv_Operation_RowEditing" OnRowUpdating="gv_Operation_RowUpdating"
            OnRowDeleting="gv_Operation_RowDeleting" OnRowDataBound="gv_Operation_RowDataBound" Style="font-size: 12px;">
            <RowStyle BackColor="#EFF3FB" />
            <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
            <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
            <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" CssClass="Freezing"/>
            <EditRowStyle BackColor="#33CCCC" />
            <AlternatingRowStyle BackColor="White" />
        </asp:GridView>
    </div>
    <asp:Panel ID="panel_readonly" runat="server">
        <asp:Label ID="label_edt_del" runat="server" Text=""></asp:Label>
        <br />
        <br />
        <div>
            <asp:LinkButton ID="lbtn_AddOperation" runat="server" OnClick="lbtn_AddOperation_Click"
                Font-Bold="True">Add operation</asp:LinkButton>
            <br />
            <asp:Panel ID="panel_add" runat="server">
                <asp:Label ID="label_OperationName" runat="server" Text="Operation:"></asp:Label>&nbsp;
                <asp:TextBox ID="tbox_OperationName" runat="server" Width="200px"></asp:TextBox>&nbsp;&nbsp;
                <asp:Label ID="lbl_operationAbbrL" runat="server" Text="Abbr"></asp:Label>&nbsp;
                <asp:TextBox ID="tbox_operationAbbrL" runat="server" Width="50px"></asp:TextBox>&nbsp;&nbsp;
                <asp:Label ID="lbl_operationAbbr" runat="server" Text="Allocation"></asp:Label>&nbsp;
                <asp:TextBox ID="tbox_operationAbbr" runat="server" Width="35px"></asp:TextBox>&nbsp;&nbsp;
                <asp:Label ID="lbl_currency" runat="server" Text="Currency"></asp:Label>&nbsp;
                <asp:DropDownList ID="ddlist_currency" runat="server" Width="50px">
                </asp:DropDownList>
                &nbsp;&nbsp;
                <asp:Button ID="btn_AddOperation" runat="server" Text="Add" OnClick="btn_AddOperation_click"
                    Width="60px" />
                &nbsp;
                <asp:Button ID="btn_CancelOperation" runat="server" Text="Cancel" OnClick="btn_CancelOperation_click"
                    Width="60px" />
            </asp:Panel>
            <br />
            <asp:LinkButton ID="lbtn_addsegment" runat="server" Font-Bold="True" OnClick="lbtn_addsegment_Click">Add segment to operation</asp:LinkButton>
            <br />
            <asp:Panel ID="pnl_addsegment" runat="server">
                <asp:Label ID="lbl_operation" runat="server" Text="Operation"></asp:Label>&nbsp;
                <asp:DropDownList ID="ddlist_operation" runat="server">
                </asp:DropDownList>
                &nbsp;&nbsp;
                <asp:Label ID="label_Segment" runat="server" Text="Segment:"></asp:Label>&nbsp;
                <asp:DropDownList ID="ddlist_Segment" runat="server" Width="50px">
                </asp:DropDownList>
                &nbsp;&nbsp;
                <asp:Button ID="btn_addsegment" runat="server" Text="Add" Width="60px" OnClick="btn_addsegment_Click" />
                &nbsp;
                <asp:Button ID="btn_cancelsegment" runat="server" Text="Cancel" Width="60px" OnClick="btn_cancelsegment_Click" />
            </asp:Panel>
            <br />
            <asp:LinkButton ID="lbtn_updatecurrency" runat="server" Font-Bold="true" OnClick="lbtn_updatecurrency_Click">Modify currency</asp:LinkButton>
            <br />
            <asp:Panel ID="pnl_updatecurrency" runat="server">
                <asp:Label ID="lbl_operation2" runat="server" Text="Operation"></asp:Label>&nbsp;
                <asp:DropDownList ID="ddlist_operation2" runat="server">
                </asp:DropDownList>
                &nbsp;&nbsp;
                <asp:Label ID="lbl_currency2" runat="server" Text="Currency"></asp:Label>&nbsp;
                <asp:DropDownList ID="ddlist_currency2" runat="server">
                </asp:DropDownList>
                &nbsp;&nbsp;
                <asp:Button ID="btn_addcurrency" runat="server" Text="Add" Width="60px" OnClick="btn_addcurrency_Click" />
                &nbsp;
                <asp:Button ID="btn_cancelcurrency" runat="server" Text="Cancel" Width="60px" OnClick="btn_cancelcurrency_Click" />
            </asp:Panel>
            <asp:Label ID="label_add" runat="server" Text=""></asp:Label>
        </div>
    </asp:Panel>
</asp:Content>
