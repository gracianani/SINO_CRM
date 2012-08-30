<%@ Page Title="User Account Management" Language="C#" MasterPageFile="~/Admin/AdminMasterPage.master"
    AutoEventWireup="true" CodeFile="AdminAccountProfile.aspx.cs" Inherits="AdminAccount"
    EnableSessionState="ReadOnly" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">

    <script type="text/javascript">
function listchange(obj, id) {
    document.getElementById(id).value = obj.value;
}
    </script>

    <div>
        <%-- by FXW 20110509 item add start--%>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <%-- by FXW 20110509 item add end--%>
                <asp:Label ID="label_find" runat="server" Text="Search:"></asp:Label>
                <asp:DropDownList ID="ddlist_find" runat="server">
                </asp:DropDownList>
                <asp:Label ID="label_in" runat="server" Text="IN"></asp:Label>
                <asp:DropDownList ID="ddlist_in" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlist_in_SelectedIndexChanged">
                </asp:DropDownList>
                <asp:Button ID="btn_find" runat="server" Text="Search" Width="60px" OnClick="btn_find_Click" />
                <%-- by FXW 20110509 item del start
                <asp:LinkButton ID="lbtn_findhelp" runat="server" OnClick="lbtn_findhelp_Click">Search help</asp:LinkButton>
                by FXW 20110509 item del end--%>
                <%-- by FXW 20110509 item add start--%>
            </ContentTemplate>
            <Triggers>
                <asp:PostBackTrigger ControlID="btn_find" />
            </Triggers>
        </asp:UpdatePanel>
        <%-- by FXW 20110509 item add end--%>
    </div>
    <br />
    <%-- by FXW 20110509 item del start
    <div style="width: 810px; overflow: scroll; height: 600px;">
    by FXW 20110509 item del end--%>
    <table>
        <tr>
            <td align="center">
                <asp:Label ID="label_head" runat="server" Text="User Information" Font-Bold="True"
                    Font-Size="Medium"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                <%-- by FXW 20110509 item add start--%>
                <%-- by FXW 20110509 item del start
                <div runat="server" id="divflow">
                by FXW 20110509 item del end--%>
                <div style="overflow: auto; height: 450px; width: 750px; position: relative; padding-top: 22px;">
                <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                    
                        <%-- by FXW 20110509 item add end--%>
                        <asp:GridView ID="gv_administrator" runat="server" CellPadding="4" ForeColor="#333333"
                            GridLines="None" OnPageIndexChanging="gv_administrator_PageIndexChanging" OnRowCancelingEdit="gv_administrator_RowCancelingEdit"
                            OnRowDataBound="gv_administrator_RowDataBound" OnRowDeleting="gv_administrator_RowDeleting"
                            OnRowEditing="gv_administrator_RowEditing" OnRowUpdating="gv_administrator_RowUpdating" Style="font-size: 12px;">
                            <RowStyle BackColor="#EFF3FB" />
                            <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                            <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                            <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" CssClass="Freezing" />
                            <EditRowStyle BackColor="#33CCCC" />
                            <AlternatingRowStyle BackColor="White" />
                        </asp:GridView>
                        <%-- by FXW 20110509 item add start--%>
                    </ContentTemplate>
                </asp:UpdatePanel>
                </div>
                <%--</div>--%>
                <%-- by FXW 20110509 item add end--%>
            </td>
        </tr>
        <tr>
            <td>
            <%-- by FXW 20110516 item18 add start--%>
            <asp:UpdatePanel ID="UpdatePanel5" runat="server">
                    <ContentTemplate>
                    <%-- by FXW 20110516 item18 add end--%>
                <asp:Label ID="label_edt_del" runat="server" Text=""></asp:Label>
                <%-- by FXW 20110516 item18 add start--%>
                </ContentTemplate>
                </asp:UpdatePanel>
                <%-- by FXW 20110516 item18 add end--%>
            </td>
        </tr>
        <tr>
            <td>
                <%-- by FXW 20110509 item add start--%>
                <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                    <ContentTemplate>
                        <%-- by FXW 20110509 item add end--%>
                        <asp:Panel ID="panel_readonly" runat="server">
                            <asp:LinkButton ID="lbtn_addUser" runat="server" OnClick="lbtn_addUser_click" Font-Bold="True">Add new user</asp:LinkButton>
                            <br />
                            <asp:Panel ID="panel_add" runat="server">
                                <table>
                                    <tr>
                                        <td>
                                            <asp:Label ID="label_firstName" runat="server" Text="FirstName:" Width="100px"></asp:Label>&nbsp;
                                            <asp:TextBox ID="tbox_firstName" runat="server" Width="140px"></asp:TextBox>&nbsp;&nbsp;
                                        </td>
                                        <td>
                                            <asp:Label ID="label_lastName" runat="server" Text="LastName:" Width="100px"></asp:Label>&nbsp;
                                            <asp:TextBox ID="tbox_lastName" runat="server" Width="140px"></asp:TextBox>&nbsp;&nbsp;
                                        </td>
                                        <td>
                                            <asp:Label ID="label_Alias" runat="server" Text="Login:" Width="100px"></asp:Label>&nbsp;
                                            <asp:TextBox ID="tbox_Alias" runat="server" Width="140px"></asp:TextBox>&nbsp;&nbsp;
                                        </td>
                                        <td>
                                            <asp:Label ID="label_abbr" runat="server" Text="Abbr:" Width="80px"></asp:Label>&nbsp;
                                            <asp:TextBox ID="tbox_abbr" runat="server" Width="70px"></asp:TextBox>&nbsp;&nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="label_Role" runat="server" Text="Role:" Width="100px"></asp:Label>&nbsp;
                                            <asp:DropDownList ID="ddlist_Role" runat="server" Width="140px">
                                            </asp:DropDownList>
                                        </td>
                                        <td>
                                            <asp:Label ID="label_startdate" runat="server" Text="StartDate:" Width="100px"></asp:Label>&nbsp;
                                            <asp:TextBox ID="tbox_startdate" runat="server" Width="140px"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:Label ID="label_enddate" runat="server" Text="EndDate:" Width="100px"></asp:Label>&nbsp;
                                            <asp:TextBox ID="tbox_enddate" runat="server" Width="140px"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:Label ID="label_gendar" runat="server" Text="Gender:" Width="80px"></asp:Label>&nbsp;
                                            <asp:DropDownList ID="ddlist_gendar" runat="server" Width="70px">
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="center" colspan="4">
                                            <asp:Button ID="btn_AddUser" runat="server" Text="Add" OnClick="btn_AddUser_Click"
                                                Width="60px" />
                                            &nbsp;
                                            <asp:Button ID="btn_CancelUser" runat="server" Text="Cancel" OnClick="btn_CancelUser_Click"
                                                Width="60px" />
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                            <asp:Label ID="label_add" runat="server" Text=""></asp:Label>
                            <asp:HiddenField ID="hidrole" runat="server" />
                        </asp:Panel>
                        <%-- by FXW 20110509 item add start--%>
                    </ContentTemplate>
                    <Triggers>
                        <asp:PostBackTrigger ControlID="btn_AddUser" />
                    </Triggers>
                </asp:UpdatePanel>
                <%-- by FXW 20110509 item add end--%>
            </td>
        </tr>
    </table>
    <%-- by FXW 20110509 item del start
    </div>
    <by FXW 20110509 item del end--%>
</asp:Content>
