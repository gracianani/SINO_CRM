<%@ Page Title="User Account" Language="C#" MasterPageFile="~/Assistant/AssistantMasterPage.master"
    AutoEventWireup="true" CodeFile="AssistantProfile.aspx.cs" Inherits="Assistant_AssistantProfile" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div>
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
        <table>
            <tr>
                <td align="center">
                    <asp:Label ID="label_head" runat="server" Text="User Information" Font-Bold="True" Font-Size="Medium"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                        <%--By FXW 20110518 ItemW2 UPDATE Start--%>
                            <%--<div style="overflow: auto; height: 480px; width: 780px; position: relative; padding-top: 22px;">--%>
                            <div style="overflow: auto; height: 480px; width: 750px; position: relative; padding-top: 22px;">
                            <%--By FXW 20110518 ItemW2 UPDATE End--%>
                                <asp:GridView ID="gv_executive" runat="server" CellPadding="4" ForeColor="#333333"
                                    GridLines="None" OnPageIndexChanging="gv_executive_PageIndexChanging" Style="font-size: 12px;">
                                    <RowStyle BackColor="#EFF3FB" />
                                    <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                    <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                                    <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White"  CssClass="Freezing"/>
                                    <AlternatingRowStyle BackColor="White" />
                                </asp:GridView>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </td>
            </tr>
        </table>
    </div>
</asp:Content>
