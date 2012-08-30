<%@ Page Title="User Relation Management" Language="C#" MasterPageFile="~/RSM/RSMMasterPage.master"
    AutoEventWireup="true" CodeFile="RSMInfo.aspx.cs" Inherits="RSM_RSMInfo"
    EnableSessionState="ReadOnly" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div align="center">
        <asp:Label ID="label_head" runat="server" Text="User Relation Management" Font-Bold="True"
            Font-Size="Medium"></asp:Label>
    </div>
    <br />
    <div>
        <asp:UpdatePanel ID="UpdatePanel4" runat="server">
            <ContentTemplate>
                <asp:Label ID="label_role" runat="server" Text="Role"></asp:Label>
                <asp:DropDownList ID="ddlist_role" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlist_role_SelectedIndexChanged">
                </asp:DropDownList>
                <asp:Label ID="label_user" runat="server" Text="User"></asp:Label>
                <asp:DropDownList ID="ddlist_user" runat="server" AutoPostBack="True">
                </asp:DropDownList>
                <asp:Button ID="btn_search" runat="server" Text="Search" Width="60px" OnClick="btn_search_Click" />
            </ContentTemplate>
            <Triggers>
                <asp:PostBackTrigger ControlID="btn_search" />
            </Triggers>
        </asp:UpdatePanel>
    </div>
    <div>
        <table>
            <tr>
                <td align="center">
                    <asp:Label ID="lblSegment" runat="server" Style="text-align: center;" Visible="false"
                        Text="Segment" />
                </td>
                <td align="center">
                    <asp:Label ID="lblCustomer" runat="server" Style="text-align: center;" Visible="false"
                        Text="Customer" />
                </td>
                <td align="center">
                    <asp:Label ID="lblCountry" runat="server" Style="text-align: center;" Visible="false"
                        Text="Country" />
                </td>
            </tr>
            <tr>
                <td valign="top">
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div style="overflow: auto; height: 480px; width: 220px; position: relative; padding-top: 22px;">
                                <asp:GridView ID="gv_segment" runat="server" CellPadding="4" ForeColor="#333333"
                                    GridLines="None" OnPageIndexChanging="gv_segment_PageIndexChanging" OnRowDeleting="gv_segment_RowDeleting"
                                    PageSize="10" OnRowDataBound="gv_segment_RowDataBound" Style="font-size: 12px;">
                                    <RowStyle BackColor="#EFF3FB" />
                                    <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                    <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                                    <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" CssClass="Freezing" />
                                    <AlternatingRowStyle BackColor="White" />
                                </asp:GridView>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </td>
                <td valign="top">
                    <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div style="overflow: auto; height: 480px; width: 220px; position: relative; padding-top: 22px;">
                                <asp:GridView ID="gv_customer" runat="server" CellPadding="4" ForeColor="#333333"
                                    GridLines="None" PageSize="10" OnPageIndexChanging="gv_customer_PageIndexChanging"
                                    OnRowDeleting="gv_customer_RowDeleting" OnRowDataBound="gv_customer_RowDataBound"
                                    Style="font-size: 12px;">
                                    <RowStyle BackColor="#EFF3FB" />
                                    <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                    <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                                    <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" CssClass="Freezing" />
                                    <AlternatingRowStyle BackColor="White" />
                                </asp:GridView>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </td>
                <td valign="top">
                    <asp:UpdatePanel ID="UpdatePanel3" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div style="overflow: auto; height: 480px; width: 220px; position: relative; padding-top: 22px;">
                                <asp:GridView ID="gv_country" runat="server" CellPadding="4" ForeColor="#333333"
                                    GridLines="None" PageSize="10" OnPageIndexChanging="gv_country_PageIndexChanging"
                                    OnRowDeleting="gv_country_RowDeleting" OnRowDataBound="gv_country_RowDataBound"
                                    Style="font-size: 12px;">
                                    <RowStyle BackColor="#EFF3FB" />
                                    <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                    <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                                    <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" CssClass="Freezing" />
                                    <AlternatingRowStyle BackColor="White" />
                                </asp:GridView>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </td>
            </tr>
            <asp:Panel ID="panel_readonly" runat="server">
                <tr>
                    <td>
                        <asp:Label ID="label_delsegment" runat="server" Text=""></asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="label_delcustomer" runat="server" Text=""> </asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="label_delcountry" runat="server" Text=""></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td align="left" valign="top">
                        <asp:LinkButton ID="lbtn_segment" runat="server" Font-Bold="True" OnClick="lbtn_segment_Click">Add Segment</asp:LinkButton>
                        <br />
                        <asp:Panel ID="panel_segment" runat="server">
                            <asp:Label ID="label_segment" runat="server" Text="Segment:"></asp:Label>&nbsp;&nbsp;
                            <asp:DropDownList ID="ddlist_segment" runat="server" Width="100px">
                            </asp:DropDownList>
                            <div align="center">
                                <asp:Button ID="btn_segment" runat="server" Text="Add" Width="60px" OnClick="btn_segment_Click" />
                                <asp:Button ID="btn_canSegment" runat="server" Text="Cancel" Width="60px" OnClick="btn_canSegment_Click" />
                            </div>
                        </asp:Panel>
                        <asp:Label ID="label_addsegment" runat="server" Text=""></asp:Label>
                    </td>
                    <td align="left" valign="top">
                        <asp:LinkButton ID="lbtn_customer" runat="server" Font-Bold="True" OnClick="lbtn_customer_Click">Add Operation</asp:LinkButton>
                        <br />
                        <asp:Panel ID="panel_customer" runat="server">
                            <asp:Label ID="label_customer" runat="server" Text="Operation:"></asp:Label>&nbsp;&nbsp;
                            <asp:DropDownList ID="ddlist_customer" runat="server" Width="100px">
                            </asp:DropDownList>
                            <div align="center">
                                <asp:Button ID="btn_customer" runat="server" Text="Add" Width="60px" OnClick="btn_customer_Click" />
                                <asp:Button ID="btn_cancustomer" runat="server" Text="Cancel" Width="60px" OnClick="btn_cancustomer_Click" />
                            </div>
                        </asp:Panel>
                        <asp:Label ID="label_addcustomer" runat="server" Text=""></asp:Label>
                    </td>
                    <td align="left" valign="top">
                        <asp:LinkButton ID="lbtn_country" runat="server" Font-Bold="True" OnClick="lbtn_country_Click">Add SubRegion</asp:LinkButton>
                        <br />
                        <asp:Panel ID="panel_country" runat="server">
                            <asp:Label ID="label_country" runat="server" Text="SubRegion:"></asp:Label>&nbsp;&nbsp;
                            <asp:DropDownList ID="ddlist_country" runat="server" Width="140px">
                            </asp:DropDownList>
                            <div align="center">
                                <asp:Button ID="btn_country" runat="server" Text="Add" Width="60px" OnClick="btn_country_Click" />
                                <asp:Button ID="btn_canCountry" runat="server" Text="Cancel" Width="60px" OnClick="btn_canCountry_Click" />
                            </div>
                        </asp:Panel>
                        <asp:Label ID="label_addcountry" runat="server" Text=""></asp:Label>
                    </td>
                </tr>
            </asp:Panel>
        </table>
    </div>
</asp:Content>
