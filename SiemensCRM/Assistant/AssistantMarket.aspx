<%@ Page Title="Market" Language="C#" MasterPageFile="~/Assistant/AssistantMasterPage.master"
    AutoEventWireup="true" CodeFile="AssistantMarket.aspx.cs" Inherits="Assistant_AssistantMarket" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div>
        <%-- by dxs 20110510 add start--%>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <%-- by dxs 20110510 add end--%>
                <asp:Label ID="label_find" runat="server" Text="Search:"></asp:Label>
                <%-- By DingJunjie 20110520 Item 12 Add Start --%>
                <asp:DropDownList ID="ddlSegment" runat="server" />
                IN Segment,
                <%-- By DingJunjie 20110520 Item 12 Add End --%>
                <asp:DropDownList ID="ddlist_find" runat="server">
                </asp:DropDownList>
                <asp:Label ID="label_in" runat="server" Text="IN"></asp:Label>
                <asp:DropDownList ID="ddlist_in" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlist_in_SelectedIndexChanged">
                </asp:DropDownList>
                <asp:Button ID="btn_find" runat="server" Text="Search" Width="60px" OnClick="btn_find_Click" />
                <%-- by dxs 20110510 add start--%>
            </ContentTemplate>
            <Triggers>
                <asp:PostBackTrigger ControlID="btn_find" />
            </Triggers>
        </asp:UpdatePanel>
        <%-- by dxs 20110510 add end--%>
        <%--<asp:LinkButton ID="lbtn_findhelp" runat="server" OnClick="lbtn_findhelp_Click">Search help</asp:LinkButton>--%>
    </div>
    <br />
    <div align="center">
        <asp:Label ID="Label_head" runat="server" Text="Market Management" Font-Bold="True"
            Style="font-size: 12px;" Font-Size="Medium"></asp:Label>
    </div>
    <div>
        <asp:Label ID="Label1" runat="server" Text="Currency: €"></asp:Label>
    </div>
    <div>
        <asp:Label ID="Label2" runat="server" Text="Unit: 1000"></asp:Label>
    </div>
    <div>
        <%--by dxs 20110511 add start--%>
        <%--by dxs 20110511 add end--%>
        <%-- by dxs 20110511 add start--%>
        <div style="overflow: auto; height: 400px; width: 820px; position: relative; padding-top: 22px;">
            <asp:UpdatePanel ID="UpdatePanel33" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <%-- by dxs 20110511 add end--%>
                    <asp:GridView ID="gv_market" runat="server" CellPadding="4" ForeColor="#333333" GridLines="None"
                        OnPageIndexChanging="gv_market_PageIndexChanging" OnRowCancelingEdit="gv_market_RowCancelingEdit"
                        OnRowDeleting="gv_market_RowDeleting" OnRowEditing="gv_market_RowEditing" OnRowUpdating="gv_market_RowUpdating">
                        <RowStyle BackColor="#EFF3FB" />
                        <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                        <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                        <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" CssClass="Freezing" />
                        <EditRowStyle BackColor="#33CCCC" />
                        <AlternatingRowStyle BackColor="White" />
                    </asp:GridView>
                    <%-- by dxs 20110511 add start--%>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
        <%-- by dxs 20110511 add end--%>
        <%--by dxs 20110511 add start--%>
        <!-- By DingJunjie 20110520 Item 12 Delete Start -->
        <%--</ContentTemplate> </asp:UpdatePanel>--%>
        <!-- By DingJunjie 20110520 Item 12 Delete End -->
        <%--by dxs 20110511 add end--%>
    </div>
    <asp:Label ID="label_del" runat="server"></asp:Label>
    <br />
    <br />
    <asp:Panel ID="panel_readonly" runat="server">
        <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <asp:LinkButton ID="lbtn_market" runat="server" Font-Bold="True" OnClick="lbtn_market_Click">Add market</asp:LinkButton>
                <br />
                <asp:Panel ID="panel_addmarket" runat="server">
                    <table>
                        <!-- By DingJunjie 20110520 Item 12 Add Start -->
                        <tr>
                            <td>
                                Segment</td>
                            <td>
                                Region</td>
                            <td>
                                Cluster</td>
                            <td>
                                Country</td>
                        </tr>
                        <tr>
                            <td>
                                <asp:DropDownList ID="ddlist_segment" runat="server" /></td>
                            <td>
                                <asp:DropDownList ID="ddlRegion" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlRegion_SelectedIndexChanged" /></td>
                            <td>
                                <asp:DropDownList ID="ddlCluster" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlCluster_SelectedIndexChanged" /></td>
                            <td>
                                <asp:DropDownList ID="ddlist_country" runat="server" /></td>
                        </tr>
                        <!-- By DingJunjie 20110520 Item 12 Add End -->
                        <tr>
                            <!-- By DingJunjie 20110520 Item 12 Delete Start -->
                            <%--<td>
                                <asp:Label ID="label_country" runat="server" Text="Country"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="label_Segment" runat="server" Text="Segment"></asp:Label>
                            </td>--%>
                            <!-- By DingJunjie 20110520 Item 12 Delete End -->
                            <td>
                                <asp:Label ID="label_thisyear" runat="server" Text="This Year"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="label_nextyear" runat="server" Text="Next Year"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="lbl_afteryear" runat="server" Text="After Year"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <!-- By DingJunjie 20110520 Item 12 Delete Start -->
                            <%--<td>
                                <asp:DropDownList ID="ddlist_country" runat="server">
                                </asp:DropDownList>
                            </td>
                            <td>
                                <asp:DropDownList ID="ddlist_segment" runat="server">
                                </asp:DropDownList>
                            </td>--%>
                            <!-- By DingJunjie 20110520 Item 12 Delete End -->
                            <td>
                                <asp:TextBox ID="tbox_thisyear" runat="server" Width="100px" Text=""></asp:TextBox>
                            </td>
                            <td>
                                <asp:TextBox ID="tbox_nextyear" runat="server" Width="100px" Text=""></asp:TextBox>
                            </td>
                            <td>
                                <asp:TextBox ID="tbox_afteryear" runat="server" Width="100px" Text=""></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td align="center" colspan="4">
                                <asp:Button ID="btn_addmarket" runat="server" Text="Add" Width="60px" OnClick="btn_addmarket_Click" />
                                <asp:Button ID="btn_cancelmarket" runat="server" Text="Cancel" Width="60px" OnClick="btn_cancelmarket_Click" />
                                &nbsp;&nbsp;
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
                <br />
                <asp:Label ID="label_add" runat="server" Text=""></asp:Label>
            </ContentTemplate>
            <Triggers>
                <asp:PostBackTrigger ControlID="btn_addmarket" />
            </Triggers>
        </asp:UpdatePanel>
    </asp:Panel>
</asp:Content>
