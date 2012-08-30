<%@ Page Title="Country" Language="C#" MasterPageFile="~/Admin/AdminMasterPage.master"
    AutoEventWireup="true" CodeFile="AdminRegion.aspx.cs" Inherits="Admin_AdminRegion"
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
            </ContentTemplate>
            <Triggers>
                <asp:PostBackTrigger ControlID="btn_find" />
            </Triggers>
        </asp:UpdatePanel>
        <%--<asp:LinkButton ID="lbtn_findhelp" runat="server" OnClick="lbtn_findhelp_Click">Search help</asp:LinkButton>--%>
    </div>
    <br />
    <div align="center">
        <asp:Label ID="label_head" runat="server" Text="Country  Relation" Font-Bold="True"
            Font-Size="Medium"></asp:Label>
    </div>
    <div style="overflow: auto; height: 480px; width: 780px; position: relative; padding-top: 22px;">
        <asp:UpdatePanel ID="UpdatePanel3" runat="server">
            <ContentTemplate>
                <asp:GridView ID="gv_Region" runat="server" CellPadding="4" ForeColor="#333333" GridLines="None"
                    OnPageIndexChanging="gv_Region_PageIndexChanging" OnRowDeleting="gv_Region_RowDeleting"
                    OnRowDataBound="gv_Region_RowDataBound" Style="font-size: 12px;">
                    <RowStyle BackColor="#EFF3FB" />
                    <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                    <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                    <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" CssClass="Freezing" />
                    <AlternatingRowStyle BackColor="White" />
                </asp:GridView>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    <asp:UpdatePanel ID="UpdatePanel4" runat="server">
        <ContentTemplate>
            <asp:SqlDataSource ID="SqlDataSource1" runat="server"></asp:SqlDataSource>
            <asp:Label ID="label_del" runat="server" Text=""></asp:Label>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:Panel ID="panel_readonly" runat="server">
        <br />
        <div>
            <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                <ContentTemplate>
                    <asp:LinkButton ID="lbtn_addcluster" runat="server" Font-Bold="True" OnClick="lbtn_addcluster_Click">Region/Cluster</asp:LinkButton>
                    <asp:Panel ID="pnl_addcluster" runat="server">
                        <table>
                            <tr>
                                <td>
                                    <asp:Label ID="lbl_region" runat="server" Text="Region"></asp:Label>
                                    <asp:DropDownList AutoPostBack="true" ID="ddlist_region" runat="server" OnSelectedIndexChanged="ddlist_region_SelectedIndexChanged">
                                    </asp:DropDownList>
                                </td>
                                <td>
                                    <asp:Label ID="lbl_region_cluster" runat="server" Text="Cluster"></asp:Label>
                                    <asp:DropDownList ID="ddlist_region_cluster" runat="server" AutoPostBack="True">
                                    </asp:DropDownList>
                                </td>
                                <td>
                                    <%--<asp:Button ID="btn_addcluster" runat="server" Text="Add" Width="60px" OnClick="btn_addcluster_Click" />--%>
                                    <asp:Button ID="btn_cancelcluster" runat="server" Text="Cancel" Width="60px" OnClick="btn_cancelcluster_Click" />
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                    <br />
                    <asp:LinkButton ID="lbtn_addcountry" runat="server" Font-Bold="True" OnClick="lbtn_addcountry_Click">Cluster/Country</asp:LinkButton>
                    <asp:Panel ID="pnl_addcountry" runat="server">
                        <table>
                            <tr>
                                <td>
                                    <asp:Label ID="lbl_cluster" runat="server" Text="Cluster"></asp:Label>
                                    <asp:DropDownList ID="ddlist_cluster" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlist_cluster_SelectedIndexChanged">
                                    </asp:DropDownList>
                                </td>
                                <td>
                                    <asp:Label ID="lbl_cluster_country" runat="server" Text="Country"></asp:Label>
                                    <asp:DropDownList ID="ddlist_cluster_country" runat="server" AutoPostBack="True">
                                    </asp:DropDownList>
                                </td>
                                <td>
                                    <%--<asp:Button ID="btn_addcountry" runat="server" Text="Add" Width="60px" OnClick="btn_addcountry_Click" />--%>
                                    <asp:Button ID="btn_cancelcountry" runat="server" Text="Cancel" Width="60px" OnClick="btn_cancelcountry_Click" />
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                    <br />
                    <asp:LinkButton ID="lbtn_addsubregion" runat="server" Font-Bold="True" OnClick="lbtn_addsubregion_Click">Country/Subregion</asp:LinkButton>
                    <asp:Panel ID="pnl_addsubregion" runat="server">
                        <table>
                            <tr>
                                <td>
                                    <asp:Label ID="lbl_country" runat="server" Text="Country"></asp:Label>
                                    <asp:DropDownList ID="ddlist_country" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlist_country_SelectedIndexChanged">
                                    </asp:DropDownList>
                                </td>
                                <td>
                                    <asp:Label ID="lbl_country_subregion" runat="server" Text="SubRegion"></asp:Label>
                                    <asp:DropDownList ID="ddlist_country_subregion" runat="server" AutoPostBack="True">
                                    </asp:DropDownList>
                                </td>
                                <td>
                                    <%--<asp:Button ID="btn_addsubregion" runat="server" Text="Add" Width="60px" OnClick="btn_addsubregion_Click" />--%>
                                    <asp:Button ID="btn_cancelsubregion" runat="server" Text="Cancel" Width="60px" OnClick="btn_cancelsubregion_Click" />
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                    <br />
                    <asp:Label ID="label_add" runat="server"></asp:Label>
                </ContentTemplate>
                <%--<Triggers>
                    <asp:PostBackTrigger ControlID="btn_addcluster" />
                </Triggers>
                <Triggers>
                    <asp:PostBackTrigger ControlID="btn_addcountry" />
                </Triggers>
                <Triggers>
                    <asp:PostBackTrigger ControlID="btn_addsubregion" />
                </Triggers>--%>
            </asp:UpdatePanel>
        </div>
    </asp:Panel>
</asp:Content>
