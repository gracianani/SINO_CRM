<%@ Page Title="" Language="C#" MasterPageFile="~/MarketingMgr/MarketingMgrMasterPage.master"
    AutoEventWireup="true" CodeFile="MarketingMgrModifyPassword.aspx.cs" Inherits="MarketingMgr_MarketingMgrModifyPassword" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div align="center">
        <asp:Panel ID="Panel4" runat="server" BackColor="#CCCCCC" BorderColor="White" Height="170px"
            Width="400px">
            <div>
                <strong>
                    <asp:Label ID="lbl_welcom" runat="server" Text="Modify Own Password"></asp:Label>
                </strong>
            </div>
            <br />
            <br />
            <div>
                <asp:Label ID="lbl_newpassword" runat="server" Text="New Password" Width="120px"></asp:Label>
                <asp:TextBox ID="tbox_password" runat="server" Width="185px" MaxLength="12" TextMode="Password"></asp:TextBox>
                <br />
                <asp:Label ID="lbl_refirmpassword" runat="server" Text="Enter it again" Width="120px"></asp:Label>
                <asp:TextBox ID="tbox_refirmpassword" runat="server" Width="185px" MaxLength="12"
                    TextMode="Password"></asp:TextBox>
            </div>
            <br />
            <div>
                <asp:Button ID="btn_ok" runat="server" Text="OK" Width="60px" OnClick="btn_ok_Click" />
                <br />
                <asp:Label ID="lbl_error" runat="server" ForeColor="Red" Text=""></asp:Label>
            </div>
        </asp:Panel>
    </div>
</asp:Content>
