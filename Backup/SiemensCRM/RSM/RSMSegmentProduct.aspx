<%@ Page Title="Segment Product" Language="C#" MasterPageFile="~/RSM/RSMMasterPage.master"
    AutoEventWireup="true" CodeFile="RSMSegmentProduct.aspx.cs" Inherits="RSMSegmentProduct" EnableSessionState="ReadOnly" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div >
        <table>
            <tr>
                <td align="center">
                    <asp:Label ID="label_head" runat="server" Text="Segment and Product Management" Font-Bold="True"
                        Font-Size="Medium"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                <div style="overflow: auto; height: 200px; width: 767px; position: relative; padding-top: 22px;">
                  <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                        <ContentTemplate>
                    <asp:GridView ID="gv_segment" runat="server" CellPadding="4" ForeColor="#333333"
                        GridLines="None" OnRowCancelingEdit="gv_segment_RowCancelingEdit"
                        OnRowDataBound="gv_segment_RowDataBound" OnRowDeleting="gv_segment_RowDeleting"
                        OnRowEditing="gv_segment_RowEditing" OnRowUpdating="gv_segment_RowUpdating" OnSelectedIndexChanging="gv_segment_SelectedIndexChanging" Style="font-size: 12px;">
                        <RowStyle BackColor="#EFF3FB" />
                        <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                        <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                        <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                        <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" CssClass="Freezing"/>
                        <EditRowStyle BackColor="#33CCCC" />
                        <AlternatingRowStyle BackColor="White" />
                    </asp:GridView>
                     </ContentTemplate>
                 </asp:UpdatePanel>
                    </div>
                </td>
            </tr>
            <asp:Panel ID="panel_readonly1" runat="server">
                <tr>
                    <td>
                        <asp:Label ID="labelseg_edit_del" runat="server" Text=""></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:LinkButton ID="lbtn_AddSegment" runat="server" Font-Bold="True" OnClick="lbtn_AddSegment_Click">Add Segment</asp:LinkButton>
                        <br />
                        <asp:Panel ID="panel_addsegment" runat="server">
                            <asp:Label ID="label_Segment" runat="server" Text="Segment:" Width="90px"></asp:Label>&nbsp;&nbsp;
                            <asp:TextBox ID="tbox_SegmentDes" runat="server" Width="120px"></asp:TextBox>
                            &nbsp;&nbsp;&nbsp;&nbsp;
                            <asp:Label ID="label_segmentAbbr" runat="server" Text="Abbr:" Width="40px"></asp:Label>&nbsp;&nbsp;
                            <asp:TextBox ID="tbox_segmentAbbr" runat="server" Width="100px" MaxLength="10"></asp:TextBox>
                            &nbsp;&nbsp;&nbsp;&nbsp;
                            <asp:Button ID="btn_AddSegment" runat="server" Text="Add" Width="60px" OnClick="btn_AddSegment_Click" />
                            &nbsp;&nbsp;
                            <asp:Button ID="btn_CancelSegment" runat="server" Text="Cancel" Width="60px" OnClick="btn_CancelSegment_Click" />
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="label_addsegment" runat="server"></asp:Label>
                    </td>
                </tr>
            </asp:Panel>
            <tr>
                <td>
                 <div style="overflow: auto; height: 200px; width: 767px; position: relative; padding-top: 22px;">
                  <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                        <ContentTemplate>
                    <asp:GridView ID="gv_product" runat="server" CellPadding="4" ForeColor="#333333"
                        GridLines="None" OnRowCancelingEdit="gv_product_RowCancelingEdit"
                        OnRowDataBound="gv_product_RowDataBound" OnRowDeleting="gv_product_RowDeleting"
                        OnRowEditing="gv_product_RowEditing" OnRowUpdating="gv_product_RowUpdating" Style="font-size: 12px;">
                        <RowStyle BackColor="#EFF3FB" />
                        <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                        <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                        <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" CssClass="Freezing"/>
                        <EditRowStyle BackColor="#33CCCC" />
                        <AlternatingRowStyle BackColor="White" />
                    </asp:GridView>
                     </ContentTemplate>
                 </asp:UpdatePanel>
                    </div>
                </td>
            </tr>
            <asp:Panel ID="panel_readonly2" runat="server">
                <tr>
                    <td>
                        <asp:Label ID="labelpro_edit_del" runat="server" Text=""></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:LinkButton ID="lbtn_AddProduct" runat="server" Font-Bold="True" OnClick="lbtn_AddProduct_Click">Add Product</asp:LinkButton>
                        <br />
                        <asp:Panel ID="panel_addproduct" runat="server">
                            <asp:Label ID="label_product" runat="server" Text="Product:" Width="90px"></asp:Label>&nbsp;&nbsp;
                            <asp:TextBox ID="tbox_productDes" runat="server" Width="200px" MaxLength="20"></asp:TextBox>
                            &nbsp;&nbsp;&nbsp;&nbsp;
                            <asp:Button ID="btn_addproduct" runat="server" Text="Add" Width="60px" OnClick="btn_addproduct_Click" />
                            &nbsp;&nbsp;
                            <asp:Button ID="btn_cancelproduct" runat="server" Text="Cancel" Width="60px" OnClick="btn_cancelproduct_Click" />
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="label_addproduct" runat="server"></asp:Label>
                    </td>
                </tr>
            </asp:Panel>
        </table>
    </div>
</asp:Content>
