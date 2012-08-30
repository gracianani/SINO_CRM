<%@ Page Title="My Account" Language="C#" MasterPageFile="~/RSM/RSMMasterPage.master"
    AutoEventWireup="true" CodeFile="RsmProfile.aspx.cs" Inherits="RSM_RsmProfile" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
     <%--By yyan 20110620 ItemW35 add Start--%>
         <asp:UpdatePanel ID="UpdatePanel11" runat="server">
            <ContentTemplate>
                <asp:Label ID="label_find" runat="server" Text="Search:"></asp:Label>
                <asp:DropDownList ID="ddlist_find" runat="server">
                </asp:DropDownList>
                <asp:Label ID="label_in" runat="server" Text="IN"></asp:Label>
                <asp:DropDownList ID="ddlist_in" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlist_in_SelectedIndexChanged">
                </asp:DropDownList>
                <asp:Button ID="btn_find" runat="server" Text="Search" Width="60px" OnClick="btn_find_Click" />
            </ContentTemplate>
            <Triggers>
                <asp:PostBackTrigger ControlID="btn_find" />
            </Triggers>
        </asp:UpdatePanel>
        <%--By yyan 20110620 ItemW35 add end--%>
         <br />
    <div align="center" style="font-size: large">
        <strong>User Information</strong>
    </div>
    <div>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <div style="overflow: auto; height: 450px; width: 750px; position: relative; padding-top: 22px;">
                    <asp:GridView ID="gv_rsmprofile" runat="server" CellPadding="4" ForeColor="#333333"
                        GridLines="None" OnRowCancelingEdit="gv_rsmprofile_RowCancelingEdit" OnRowEditing="gv_rsmprofile_RowEditing"
                        OnRowUpdating="gv_rsmprofile_RowUpdating" Style="font-size: 12px;">
                        <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                        <RowStyle BackColor="#EFF3FB" />
                        <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                        <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" CssClass="Freezing" />
                        <EditRowStyle BackColor="#33CCCC" />
                        <AlternatingRowStyle BackColor="White" />
                    </asp:GridView>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    <asp:Label ID="label_editnote" runat="server"></asp:Label>
</asp:Content>
