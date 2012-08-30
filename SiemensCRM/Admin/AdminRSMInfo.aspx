<%@ Page Title="User Relation Management" Language="C#" MasterPageFile="~/Admin/AdminMasterPage.master"
    AutoEventWireup="true" CodeFile="AdminRSMInfo.aspx.cs" Inherits="Admin_AdminRSMInfo"
    EnableSessionState="ReadOnly" %>

<%--<html xmlns="http://www.w3.org/1999/xhtml">--%>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">

    <script language="javascript" type="text/javascript">
    function showDialog(url,type)
    {
        var date = new Date();
        var returnVal = window.showModalDialog(url+'&d='+date.getFullYear()+date.getMonth()+date.getDate()+date.getHours()+date.getMinutes+date.getSeconds()+date.getMilliseconds(),null,'dialogWidth:850px;dialogHeight:500px;help:no;unadorned:no;resizable:yes;status:no;');
        if(returnVal!="null")
        {
            document.getElementById('back_key').value = returnVal;
            document.getElementById('back_type').value = type;
        }
        return false;
    }
    window.onload = function()
    {
        document.getElementById('back_key').value = "";
        document.getElementById('back_type').value = "";
    }
    </script>

    <div align="center">
        <%-- By DingJunjie 20110504 Item 45 Delete Start --%>
        <%-- <asp:Label ID="label_head" runat="server" Text="User Relation Management" Font-Bold="True" Font-Size="Medium"></asp:Label> --%>
        <%-- By DingJunjie 20110504 Item 45 Delete End --%>
        <%-- By DingJunjie 20110504 Item 45 ADD Start --%>
        <asp:Label ID="label_head" runat="server" Text="User Relation Management" Font-Bold="True"
            Font-Size="Medium"></asp:Label>
        <%-- By DingJunjie 20110504 Item 45 Add End --%>
    </div>
    <br />
    <%-- by FXW 20110510 item add start--%>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <%-- by FXW 20110510 item add end--%>
            <asp:Label ID="label_role" runat="server" Text="Role"></asp:Label>
            <asp:DropDownList ID="ddlist_role" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlist_role_SelectedIndexChanged">
            </asp:DropDownList>
            <asp:Label ID="label_user" runat="server" Text="User"></asp:Label>
            <asp:DropDownList ID="ddlist_user" runat="server" AutoPostBack="True">
            </asp:DropDownList>
            <asp:Button ID="btn_search" runat="server" Text="Search" Width="60px" OnClick="btn_search_Click" />
            <%-- by FXW 20110510 item add start--%>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btn_search" />
        </Triggers>
    </asp:UpdatePanel>
    <%-- by FXW 20110510 item add end--%>
    <input id="back_type" name="back_type" type="hidden" />
    <input id="back_key" name="back_key" type="hidden" />
    <div>
        <table>
            <%-- by FXW 20110511 item18 Del start--%>
            <tr>
                <td align="center">
                    <asp:Label ID="lblSegment" runat="server" Visible="false" Text="Segment" />
                </td>
                <td align="center">
                    <asp:Label ID="lblOperation" runat="server" Visible="false" Text="Operation" />
                </td>
                <td align="center">
                    <asp:Label ID="lblSubRegion" runat="server" Visible="false" Text="SubRegion" />
                </td>
            </tr>
            <%-- by FXW 20110511 item18 Del end--%>
            <tr>
                <td valign="top">
                    <%-- by FXW 20110510 item add start--%>
                    <%-- by FXW 20110511 item18 Del start--%>
                    <%--<div runat="server" id="div1">--%>
                    <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div style="overflow: auto; height: 480px; width: 240px; position: relative; padding-top: 22px;">
                                <%-- by FXW 20110511 item18 Del end--%>
                                <%-- by FXW 20110510 item add end--%>
                                <asp:GridView ID="gv_segment" runat="server" CellPadding="4" ForeColor="#333333"
                                    GridLines="None" OnPageIndexChanging="gv_segment_PageIndexChanging" OnRowDeleting="gv_segment_RowDeleting"
                                    PageSize="10" OnRowDataBound="gv_segment_RowDataBound" Style="font-size: 12px;">
                                    <RowStyle BackColor="#EFF3FB" />
                                    <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                    <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                                    <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" CssClass="Freezing" />
                                    <AlternatingRowStyle BackColor="White" />
                                </asp:GridView>
                                <%-- by FXW 20110510 item add start--%>
                                <%-- by FXW 20110511 item18 Del start--%>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    <%--</div>--%>
                    <%-- by FXW 20110511 item18 Del end--%>
                    <%-- by FXW 20110510 item add end--%>
                </td>
                <td valign="top">
                    <%-- by FXW 20110510 item add start--%>
                    <%-- by FXW 20110511 item18 Del start--%>
                    <%--<div runat="server" id="div2">--%>
                    <asp:UpdatePanel ID="UpdatePanel3" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div style="overflow: auto; height: 480px; width: 240px; position: relative; padding-top: 22px;">
                                <%-- by FXW 20110511 item18 Del end--%>
                                <%-- by FXW 20110510 item add end--%>
                                <asp:GridView ID="gv_customer" runat="server" CellPadding="4" ForeColor="#333333"
                                    GridLines="None" PageSize="10" OnPageIndexChanging="gv_customer_PageIndexChanging"
                                    OnRowDeleting="gv_customer_RowDeleting" OnRowDataBound="gv_customer_RowDataBound">
                                    <RowStyle BackColor="#EFF3FB" />
                                    <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                    <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                                    <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" CssClass="Freezing" />
                                    <AlternatingRowStyle BackColor="White" />
                                </asp:GridView>
                                <%-- by FXW 20110510 item add start--%>
                                <%-- by FXW 20110511 item18 Del start--%>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    <%--</div>--%>
                    <%-- by FXW 20110511 item18 Del end--%>
                    <%-- by FXW 20110510 item add end--%>
                </td>
                <td valign="top">
                    <%-- by FXW 20110510 item add start--%>
                    <%--<div runat="server" id="div3">--%>
                    <asp:UpdatePanel ID="UpdatePanel4" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div style="overflow: auto; height: 480px; width: 240px; position: relative; padding-top: 22px;">
                                <%-- by FXW 20110510 item add end--%>
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
                            <%-- by FXW 20110510 item add start--%>
                            <%-- by FXW 20110511 item18 Del start--%>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    <%--</div>--%>
                    <%-- by FXW 20110511 item18 Del end--%>
                    <%-- by FXW 20110510 item add end--%>
                </td>
            </tr>
            <asp:Panel ID="panel_readonly" runat="server">
                <tr>
                    <td>
                        <%-- by FXW 20110511 item18 add start--%>
                        <asp:UpdatePanel ID="UpdatePanel6" runat="server">
                            <ContentTemplate>
                                <%-- by FXW 20110511 item18 add end--%>
                                <asp:Label ID="label_delsegment" runat="server" Text=""></asp:Label>
                                <%-- by FXW 20110511 item18 Del start--%>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <%-- by FXW 20110511 item18 Del end--%>
                    </td>
                    <td>
                        <%-- by FXW 20110511 item18 add start--%>
                        <asp:UpdatePanel ID="UpdatePanel7" runat="server">
                            <ContentTemplate>
                                <%-- by FXW 20110511 item18 add end--%>
                                <asp:Label ID="label_delcustomer" runat="server" Text=""> </asp:Label>
                                <%-- by FXW 20110511 item18 Del start--%>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <%-- by FXW 20110511 item18 Del end--%>
                    </td>
                    <td>
                        <%-- by FXW 20110511 item18 add start--%>
                        <asp:UpdatePanel ID="UpdatePanel8" runat="server">
                            <ContentTemplate>
                                <%-- by FXW 20110511 item18 add end--%>
                                <asp:Label ID="label_delcountry" runat="server" Text=""></asp:Label>
                                <%-- by FXW 20110511 item18 Del start--%>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <%-- by FXW 20110511 item18 Del end--%>
                    </td>
                </tr>
                <tr>
                    <td align="left" valign="top">
                        <%-- by FXW 20110510 item add start--%>
                        <asp:UpdatePanel ID="UpdatePanel5" runat="server">
                            <ContentTemplate>
                                <%-- by FXW 20110510 item add end--%>
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
                                <%-- by FXW 20110510 item add start--%>
                            </ContentTemplate>
                            <Triggers>
                                <asp:PostBackTrigger ControlID="btn_segment" />
                            </Triggers>
                        </asp:UpdatePanel>
                        <%-- by FXW 20110510 item add end--%>
                    </td>
                    <td align="left" valign="top">
                        <%-- by ryzhang 20110506 item47 del start
                        <asp:LinkButton ID="lbtn_customer" Font-Bold="True" runat="server" OnClick="lbtn_customer_Click">>Add Operation</asp:LinkButton>
                        by ryzhang 20110506 item47 del end--%>
                        <%-- by ryzhang 20110506 item47 add start--%>
                        <asp:LinkButton ID="lbtn_customer" Font-Bold="True" runat="server" OnClick="btn_search_Click"> Add Operation</asp:LinkButton>
                        <%--by ryzhang 20110506 item47 add end--%>
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
                        <%-- by ryzhang 20110506 item47 del start
                        <asp:LinkButton ID="lbtn_country" runat="server" Font-Bold="True" OnClick="lbtn_country_Click">Add SubRegion</asp:LinkButton>
                     by ryzhang 20110506 item47 del end--%>
                        <%-- by ryzhang 20110506 item47 add start--%>
                        <asp:LinkButton ID="lbtn_country" runat="server" Font-Bold="True" OnClick="btn_search_Click">Add SubRegion</asp:LinkButton>
                        <%--by ryzhang 20110506 item47 add end--%>
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
