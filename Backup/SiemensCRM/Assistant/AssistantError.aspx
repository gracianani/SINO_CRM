<%@ Page Title="Error" Language="C#" MasterPageFile="~/Assistant/AssistantMasterPage.master" AutoEventWireup="true" CodeFile="AssistantError.aspx.cs" Inherits="Assistant_AssistantError" EnableSessionState="ReadOnly" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

<div>
    <asp:Label ID="label_head" runat="server" Text="ERROR" Font-Bold="True" 
        Font-Size="X-Large" ForeColor="Red"></asp:Label>
</div>

<br />
<br />
    <asp:Label ID="label_error" runat="server" Text="" ></asp:Label>

</asp:Content>

