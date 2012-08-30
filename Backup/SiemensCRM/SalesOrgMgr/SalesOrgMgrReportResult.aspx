<%@ Page Title="Run" Language="C#" 
    AutoEventWireup="true" CodeFile="SalesOrgMgrReportResult.aspx.cs" Inherits="SalesOrgMgrReportResult"
    EnableSessionState="ReadOnly" EnableEventValidation="false" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <style type="text/css">
        .r
        {
            position: relative;
        }
    </style>

    <script type="text/javascript" src="../js/action.js"></script>

</head>
<body>

    <asp:Panel ID="Panel3" runat="server" Width="100%" BackImageUrl="~/images/bg.jpg"
        Style="background-repeat: repeat-x; background-attachment: scroll; background-position: top;
        margin: auto;" Height="700px">
               <table width="100%" style="text-align: center;">
            <tr valign="top">
                <td>
                </td>
                <td style="width: 1024px" align="left">
                    <form id="form1" runat="server" target="_self">
                    <div>
                        <asp:Panel ID="Panel1" runat="server" BackImageUrl="~/images/logo.JPG" Height="108px">
                            <br />
                            <br />
                        </asp:Panel>
                        <hr />
                        <asp:Panel ID="Panel4" runat="server">
                            <div style="float: left; padding-right: 5px;">
                                <strong>
                                    <asp:Label ID="lbl_welcom" runat="server" Text="Welcome Alias"></asp:Label>
                                    <asp:Label ID="lbl_currentmeetingdate" runat="server" Text=""></asp:Label>
                                </strong>
                            </div>
                        </asp:Panel>
                        <table style="text-align: center; height: 550px; width: 100%;">
                            <tr>
                                <td id="menu_o" align="left" valign="top" style="width: 12px; display: none;">
                                    <img style="cursor: pointer" src="../images/cl_op_03.gif" alt="open the menu" width="12"
                                        height="53" onclick="show('menu_c');hide('menu_o')" />
                                </td>
                                <td style="width: 1024px" align="left" valign="top">
                                    <div style="width: 100%">
    <fieldset class="fie">
        <legend><strong>Run</strong></legend>
       <%-- <div style="width: 980px; overflow: scroll; height: 632px;">--%>
            <table>
              
                <tr>
                    <td>
                    <div style="overflow: auto; height: 400px; width: 950px; position: relative; padding-top: 22px;">
                        <asp:GridView ID="gv_administrator" runat="server" AllowSorting="True" CellPadding="4" 
                            GridLines="None" OnSorting="gv_administrator_Sorting" OnPageIndexChanging="gv_administrator_PageIndexChanging"
                            OnRowDataBound="gv_administrator_RowDataBound" Style="font-size: 12px;" >
                            <RowStyle BackColor="#EFF3FB" />
                            <PagerStyle HorizontalAlign="center" Height="2" Font-Size="X-Small" />
                            <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" CssClass="Freezing" />
                            <AlternatingRowStyle BackColor="White" />
                        </asp:GridView>
                         </div>
                    </td>
                </tr>
                
                <div runat="server" id="div_export" visible="false">
                    <tr>
                    <asp:GridView ID="gv_administrator_hid" runat="server" CellPadding="4" 
                            OnRowDataBound="gv_administrator_hid_RowDataBound"
                            CssClass="c_tab">
                            <HeaderStyle  HorizontalAlign="center" VerticalAlign="Middle" />
                            <PagerStyle HorizontalAlign="center" Height="2" Font-Size="X-Small" />
                            <AlternatingRowStyle BackColor="White" />
                        </asp:GridView>
                    </tr>
                </div>
            </table>
            <asp:HiddenField ID="HidOrderBy" runat="server" />
            <asp:HiddenField ID="HidReportId" runat="server" />
     <%--   </div>--%>
        <div>
            <p>
            <br />
                <asp:Button ID="btn_output" runat="server" Text="Export" Width="60px" OnClick="btn_output_Click" />
                <input type="button" id="Close"  value="Close" onclick="window.close()"  style="border:1px solid #aaa; background:transparent; width:60px;"/>
            </p>
        </div>
    </fieldset>
                                    </div>
                                </td>
                            </tr>
                        </table>
                        <hr />
                        <div style="text-align: center; width: 100%;">
                            Siemens HP Forecast & Planning Tool
                        </div>
                    </div>
                    </form>
                </td>
                <td>
                </td>
            </tr>
        </table>
        
  

    </asp:Panel>
</body>
</html>
