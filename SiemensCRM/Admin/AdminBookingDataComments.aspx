<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AdminBookingDataComments.aspx.cs"
    Inherits="Admin_AdminBookingDataComments" EnableSessionState="ReadOnly" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Comments</title>
</head>
<body>
    <form id="form1" runat="server" target="_self">
    <br />
    <br />
    <div>
        <asp:Label ID="label_header" runat="server" Text="Booking Data and Comments" Font-Bold="True"
            Font-Size="Medium"></asp:Label>
    </div>
    <table>
        <tr>
            <td align="left" valign="top">
                <asp:GridView ID="gv_data" runat="server" CellPadding="4" ForeColor="#333333" GridLines="None">
                    <RowStyle BackColor="#EFF3FB" />
                    <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                    <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                    <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                    <EditRowStyle BackColor="#33CCCC" />
                    <AlternatingRowStyle BackColor="White" />
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td align="left" valign="top">                                     
                <asp:GridView ID="gv_comments" runat="server" CellPadding="4" ForeColor="#333333"
                    GridLines="None" OnRowCancelingEdit="gv_comments_RowCancelingEdit" OnRowEditing="gv_comments_RowEditing"
                    OnRowUpdating="gv_comments_RowUpdating">
                    <RowStyle BackColor="#EFF3FB" />
                    <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                    <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                    <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                    <EditRowStyle BackColor="#33CCCC" />
                    <AlternatingRowStyle BackColor="White" />
                </asp:GridView>
            </td>
        </tr>      
        <tr>
            <td>
                <asp:Label ID="label_note" runat="server" Text="" ForeColor="Red"></asp:Label>
            </td>
        </tr>
        <tr>
           
                <asp:Panel ID="panel_comments" runat="server">
                   <td>
                                <asp:Label ID="label_project" runat="server" Text="Product" Width="200px"></asp:Label>
                                <asp:DropDownList ID="ddlist_product" runat="server" Width="200px" OnSelectedIndexChanged="ddl_SelectedProductChanged" AutoPostBack="True"  >
                                </asp:DropDownList>
                                <br />
                                <asp:Label ID="label_input" runat="server" Text="Input Information" Width="200px"></asp:Label><br />
                   
                                  <textarea id="content" name="TEXTAREA1" rows="4" cols="65" runat="server" style="WIDTH: 100%"></textarea>

                                <br />
                               <%-- <asp:Label ID="label_ok" runat="server" Text="If you want to input following information, Click "></asp:Label>
                                <asp:Image ID="Image1" runat="server" ImageUrl="~/images/ok.jpg" />
                                <br />--%>
                                <asp:Label ID="label_comments" runat="server" Text=""></asp:Label>
                            </td>   
                  </asp:Panel>
     </tr>
        <tr>
             <td>
                  <asp:Button ID="btn_ok" runat="server" Text="Save comments" OnClick="btn_ok_Click"
                                    Width="130px" /> 
                <asp:Button ID="btn_close" runat="server" Text="Close" Width="60px" OnClick="btn_close_Click" />
            </td>
        </tr>
    </table>
    </form>
</body>
</html>
