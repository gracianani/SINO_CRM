<%@ Page Title="Market Data Upload" Language="C#" MasterPageFile="~/Admin/AdminMasterPage.master"
    AutoEventWireup="true" CodeFile="AdminUploadMarketData.aspx.cs" Inherits="Admin_AdminUploadMarketData"
    EnableSessionState="ReadOnly" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">

    <script type="text/javascript" src="../js/LoadingBox.js"></script>

    <style type="text/css">
    .lightbox{width:300px;background:#FFFFFF;border:1px solid #ccc;line-height:25px; top:20%; left:20%;}
    .lightbox dt{background:#f4f4f4; padding:5px;}
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <dl id="idBox" class="lightbox">
        <dt id="idBoxHead" style="text-align: center;"><b>Uploading... ... </b></dt>
        <dd>
            <embed type="application/x-shockwave-flash" src="../images/loading.swf" width="206"
                height="20" />
        </dd>
    </dl>
    <asp:Panel ID="panel_visible" runat="server">
        <table width="100%" cellpadding="4">
            <tr>
                <td align="center" valign="middle" colspan="3">
                    <asp:Label ID="label_header" runat="server" Text="Market Data Upload" Font-Bold="True"
                        Font-Size="Medium"></asp:Label>
                </td>
            </tr>
            <tr>
                <td align="right" style="width: 100px;">
                 <%--by yyan 20110726 itemW85 edit start--%>
                    Market Data:</td>
                 <%--by yyan 20110726 itemW85 edit end--%>
                <td align="left" style="width: 220px;">
                    <asp:FileUpload ID="fleMarketData" runat="server" />
                </td>
                <td align="left">
                    <asp:Button ID="btn_upload" runat="server" OnClick="btn_upload_Click" Text="Upload"
                        OnClientClick="box.Show();" />
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:Label ID="label_visible" runat="server" Text="" ForeColor="Red"></asp:Label>

    <script type="text/javascript">
    InitBox();
    </script>

</asp:Content>
