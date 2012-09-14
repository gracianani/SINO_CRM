<%@ Page Language="C#" AutoEventWireup="true" CodeFile="BookingSalesData.aspx.cs" 
    Inherits="BookingSalesData" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Booking Sales Data</title>
    <%-- By DingJunjie 20110516 Item 4 Add Start --%>
    <link type="text/css" rel="stylesheet" href="js/tableheadstyle.css" />

    <script type="text/javascript" language="javascript" src="js/jquery-1.4.2.min.js"></script>

    <%--<script type="text/javascript" language="javascript" src="js/fixtablehead.js"></script>--%>

    <%-- By DingJunjie 20110516 Item 4 Add End --%>
</head>

<script type="text/javascript" language="javascript">
    function linkUpdate() {
        var link = document.location.href;
        document.location = link.substring(0, link.lastIndexOf('#')) + "#down";
    }
    function RecordPostion(obj) {
        var div1 = obj;
        var sx = document.getElementById('<%=dvscrollX.ClientID %>');
        var sy = document.getElementById('<%=dvscrollY.ClientID %>');

        sy.value = div1.scrollTop;
        sx.value = div1.scrollLeft;
//        alert(sx.value);
//        alert(sy.value);
    }
    function GetResultFromServer() {
        try {
            var sx = document.getElementById('<%=dvscrollX.ClientID %>');
            var sy = document.getElementById('<%=dvscrollY.ClientID %>');

//            alert(sy.value);
//            alert(document.getElementById('outbox1').scrollTop);
            document.getElementById('outbox1').scrollTop = sy.value;
//            alert(document.getElementById('outbox1').scrollTop);
            document.getElementById('outbox1').scrollLeft = sx.value;
//            alert(document.getElementById('outbox1').scrollHeight);
//            alert(sx.value);
            
        } catch (e) {
        }
    }
    function listchange(obj, id) {
        document.getElementById(id).value = obj.value;
    }

    function setOperation(hid, keyId, obj) {
    
        var hidObj = document.getElementById(hid);
        var val = hidObj.value;
        var strs = new Array(); 
        if (val != null) {
            strs = val.split(";");
            for (i = 0; i < strs.length; i++) {
                if (strs[i].substring(0, strs[i].indexOf(":")) == keyId)
                    strs[i] = keyId + ":" + obj.value;
            }
            var newval = "";
            for (m = 0; m < strs.length; m++) {
                if (m == 0)
                    newval = strs[m];
                else {
                    newval += ";" + strs[m];
                }
            }
            hidObj.value = newval;
        }
    }

    // By DingJunjie 20110602 ItemW28 Add Start
    function setFocusInfo(tableName, rowIndex) {
        document.getElementById("hidTableName").value = tableName;
        document.getElementById("hidRowIndex").value = rowIndex;
    }
    // By DingJunjie 20110602 ItemW28 Add End
    //by yyan 20110630 itemw57 add start
    function showProNamePage(txtID, hidID) {
        var txtObj = document.getElementById(txtID);
        var hidObj = document.getElementById(hidID);
        var date = new Date();
        var url = "ProjectSelect.aspx?TimeFlag=" + date.getFullYear() + date.getMonth() + date.getDate() + date.getHours() + date.getMinutes + date.getSeconds() + date.getMilliseconds();
        var argu = hidObj.value;
        var returnValue = window.showModalDialog(url, argu, 'dialogWidth:550px;dialogHeight:550px;resizable:yes;status:no;');
        if (returnValue != null) {
            txtObj.value = returnValue[1];
            hidObj.value = returnValue[0];
        }
    }
    //by yyan 20110630 itemw57 add end
    // By DingJunjie 20110607 ItemW18 Add Start
    function showCusNamePage(txtID, hidID) {
        var txtObj = document.getElementById(txtID);
        var hidObj = document.getElementById(hidID);
        var date = new Date();
        var url = "CustomerNameSelect.aspx?TimeFlag=" + date.getFullYear() + date.getMonth() + date.getDate() + date.getHours() + date.getMinutes + date.getSeconds() + date.getMilliseconds();
        var argu = hidObj.value;
        var returnValue = window.showModalDialog(url, argu, 'dialogWidth:550px;dialogHeight:600px;resizable:yes;status:no;');
        if (returnValue != null) {
            txtObj.value = returnValue[1];
            hidObj.value = returnValue[0];
        }
    }

    function clearOldCusName() {
        document.getElementById("txtOldCusName").value = "";
        document.getElementById("hidOldCusName").value = "";
    }

    function clearNewCusName() {
        document.getElementById("txt_customernameYTD").value = "";
    }
    // By DingJunjie 20110602 ItemW18 Add End

    function showRCOBViewPage(RSMID, salesOrgID, subRgionID, customerID,
    bookingY, deliverY, segmentID, operationID, projectID, salesChannelID, year, month, productID) {
        var url = "RCOBInfo.aspx?timestame=" + Date.parse(new Date());
        var argu = new Array();
        argu.push(RSMID);
        argu.push(salesOrgID);
        argu.push(subRgionID);
        argu.push(customerID);
        argu.push(bookingY);
        argu.push(deliverY);
        argu.push(segmentID);
        argu.push(operationID);
        argu.push(projectID);
        argu.push(salesChannelID);
        argu.push(year);
        argu.push(month);
        argu.push(productID);
        window.showModalDialog(url, argu, 'dialogWidth:320px;dialogHeight:180px;resizable:yes;status:no;');
    }

    function showRCOBPage(textCID, hidSubAmountCID, hidPercentageCID, RSMID, salesOrgID, subRgionID, customerID,
    bookingY, deliverY, segmentID, operationID, projectID, salesChannelID, year, month, productID) {
        var url = "RCOBEdit.aspx?timestame=" + Date.parse(new Date());
        var value = document.getElementById(hidSubAmountCID).value;
        var percentage = document.getElementById(hidPercentageCID).value;
        var text = document.getElementById(textCID).value;

        var argu = new Array();
        argu.push(RSMID);
        argu.push(salesOrgID);
        argu.push(subRgionID);
        argu.push(customerID);
        argu.push(bookingY);
        argu.push(deliverY);
        argu.push(segmentID);
        argu.push(operationID);
        argu.push(projectID);
        argu.push(salesChannelID);
        argu.push(year);
        argu.push(month);
        argu.push(productID);
        argu.push(text);
        argu.push(percentage);
        argu.push(value);
        var returnValue = window.showModalDialog(url, argu, 'dialogWidth:320px;dialogHeight:180px;resizable:yes;status:no;');
        if (returnValue != null) {
            document.getElementById(hidSubAmountCID).value = returnValue[2];
            document.getElementById(hidPercentageCID).value = returnValue[1];
            document.getElementById(textCID).value = returnValue[0];
        }
    }

    function setRCOBState(obj) {
        var backupParam = document.getElementById("hidBackupParam").value;
        if (backupParam != null) {
            var backupParamArr = backupParam.split("^");
            var eventArr = null;
            var paramArr = null;
            var textObj = null;
            var nowID = null;
            for (var i = 0; i < backupParamArr.length; i++) {
                eventArr = backupParamArr[i].split("@");
                textObj = document.getElementById(eventArr[0]);
                if (obj.options[obj.selectedIndex].text.toLocaleLowerCase().indexOf("rc own business") != -1) {
                    textObj.onclick = function() {
                        nowid = 0;
                        for (var j = 0; j < backupParamArr.length; j++) {
                            if (backupParamArr[j].indexOf(this.id) > -1) {
                                nowid = j;
                                break;
                            }
                        }
                        eventArr = backupParamArr[nowid].split("@");
                        paramArr = eventArr[1].split(",");

                        showRCOBPage(paramArr[0], paramArr[1], paramArr[2], paramArr[3], paramArr[4], paramArr[5],
                        paramArr[6], paramArr[7], paramArr[8], paramArr[9], paramArr[10], paramArr[11], paramArr[12],
                        paramArr[13], paramArr[14], paramArr[15]);

                    };
                    textObj.readOnly = true;
                }
                else {
                    textObj.onclick = null;
                    textObj.readOnly = false;
                }
            }
        }
    }

    function setRCOBValue() {
        var hiddenID = document.getElementById("hidHiddenID").value;
        var hiddenIDArr = hiddenID.split("^");
        var hiddenValue = "";
        var IDArr = null;
        if (hiddenIDArr != "") {
            for (var i = 0; i < hiddenIDArr.length; i++) {
                IDArr = hiddenIDArr[i].split(",");
                hiddenValue += document.getElementById(IDArr[0]).value + "," + document.getElementById(IDArr[1]).value + "^";
            }
            document.getElementById("hidHiddenValue").value = hiddenValue;
        }
    }

    function showOptionTip() {
        var selectArray = document.getElementsByTagName("select");
        for (var i = 0; i < selectArray.length; i++) {
            if (selectArray[i].id != "ddlDataType") {
                for (var j = 0; j < selectArray[i].options.length; j++) {
                    selectArray[i].options[j].title = selectArray[i].options[j].text;
                }
            }
        }
    }

    function getCustomerListInfo(obj) {
        BookingSalesData.bindCustomerBySubRegionIDAjax(obj.value, receiveCustomerListInfo);
    }

    function receiveCustomerListInfo(result) {
        var selectObj = null;
        var selectArray = document.getElementsByTagName("select");
        for (var i = 0; i < selectArray.length; i++) {
            if (selectArray[i].id.indexOf("ddlRowCustomerID") != -1) {
                selectObj = selectArray[i];
                break;
            }
        }
        if (selectObj != null) {
            selectObj.options.length = 0;
            selectObj.options.add(new Option("", ""));
            var table = result.value;
            if (table != null) {
                for (var i = 0; i < table.Rows.length; i++) {
                    selectObj.options.add(new Option(table.Rows[i]["NAME"], table.Rows[i]["ID"]));
                }
                setRCOBState(selectObj);
            }
            document.getElementById("HiddenCustomer").value = "";
            showOptionTip();
        }
    }
</script>

<body>
    <asp:Panel ID="Panel3" runat="server" Width="100%" BackImageUrl="~/images/bg.jpg"
        Style="background-repeat: repeat-x; background-attachment: scroll; background-position: top;
        margin: auto;" Height="645px">
        <input type="hidden" id="dvscrollX" runat="server" /><input type="hidden" id="dvscrollY" runat="server" />
        <table style="text-align: center" width="100%">
            <tbody>
                <tr valign="top">
                    <td>
                    </td>
                    <td style="width: 1024px" align="left">
                        <form id="form1" runat="server">
                        <%-- By DingJunjie 20110512 Item 7 Add Start --%><asp:ScriptManager ID="ScriptManager1"
                            runat="server">
                        </asp:ScriptManager>
                        <%-- By DingJunjie 20110523 Item 7 Delete Start --%><%--<asp:HiddenField ID="hidproject" runat="server" />
                        <asp:HiddenField ID="hidsale" runat="server" />--%><%-- By DingJunjie 20110523 Item 7 Delete End --%><%-- By DingJunjie 20110517 Item 7 Add Start --%><asp:UpdatePanel
                            ID="upHidden" runat="server">
                            <ContentTemplate>
                                <asp:HiddenField ID="hidSubRegionID" runat="server" />
                                <%-- by daixuesong 20110526 Item add start--%>
                                <asp:HiddenField ID="HiddenCustomer" runat="server" />
                                <%-- by daixuesong 20110526 Item add end--%>
                                <%-- By DingJunjie 20110523 Item 7 Add Start --%>
                                <asp:HiddenField ID="hidproject" runat="server" />
                                <asp:HiddenField ID="hidsale" runat="server" />
                                <%-- By DingJunjie 20110523 Item 7 Add End --%>
                                <asp:HiddenField ID="hidButtomFlag" runat="server" />
                                <%-- By DingJunjie 20110602 ItemW28 Add Start--%>
                                <asp:HiddenField ID="hidTableName" runat="server" />
                                <asp:HiddenField ID="hidRowIndex" runat="server" />
                                <%-- By DingJunjie 20110602 ItemW28 Add End--%>
                                <%-- By DingJunjie 20110601 ItemW18 Add Start--%>
                                <asp:HiddenField ID="hidBookingY" runat="server" />
                                <asp:HiddenField ID="hidDeliverY" runat="server" />
                                <%-- By DingJunjie 20110601 ItemW18 Add End--%>
                                <asp:HiddenField ID="hidBackupParam" runat="server" />
                                <asp:HiddenField ID="hidHiddenID" runat="server" />
                                <asp:HiddenField ID="hidHiddenValue" runat="server" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <%-- By DingJunjie 20110517 Item 7 Add Start --%><%-- By DingJunjie 20110512 Item 7 Add Start --%><div>
                            <asp:Panel ID="Panel1" runat="server" Height="108px" BackImageUrl="~/images/logo.JPG">
                                <br />
                                <br />
                            </asp:Panel>
                            <hr />
                            <asp:Panel ID="Panel4" runat="server">
                                <div style="float: left; padding-right: 5px;">
                                    <strong>
                                        <asp:Label ID="lbl_welcom" runat="server" Text=""></asp:Label>
                                    </strong>
                                </div>
                            </asp:Panel>
                            <div align="center">
                                <table style="height: 497px; text-align: left" width="100%">
                                    <tbody>
                                        <tr>
                                            <td>
                                                <asp:Label ID="label_RSMAbbr" runat="server" Font-Size="Large" Font-Bold="True" Text=""></asp:Label>&nbsp;&nbsp;&nbsp;&nbsp;
                                                <asp:Label ID="label_headdescription" runat="server" Font-Size="Medium" Font-Bold="True"
                                                    Text=""></asp:Label>
                                                <br />
                                                <asp:Label ID="label_currency" runat="server" Text=""></asp:Label>&nbsp;&nbsp;&nbsp;&nbsp;
                                                <asp:LinkButton ID="lbtn_return" OnClick="lbtn_return_Click" runat="server" Width="100px"
                                                    Font-Bold="true">Return</asp:LinkButton>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                *****************************************************************************************
                                            </td>
                                        </tr>
                                        <tr>
                                            <td valign="top">
                                                <%-- By DingJunjie 20110517 Item 4 Delete Start --%><%--<div style="overflow: scroll; height: 350px; width: 1010px; " id="dvBody">--%><%-- By DingJunjie 20110517 Item 4 Delete End --%><%-- By DingJunjie 20110530 ItemW18 Add Start --%><asp:DropDownList
                                                    ID="ddlDataType" runat="server" OnSelectedIndexChanged="ddlDataType_SelectedIndexChanged"
                                                    AutoPostBack="true">
                                                </asp:DropDownList>
                                                <%-- By DingJunjie 20110530 ItemW18 Add End --%><table>
                                                    <asp:Panel ID="pnl_edit" runat="server">
                                                        <tr />
                                                        <td align="left" colspan="5" />
                                                        Customer
                                                        <asp:DropDownList ID="ddlist_customer_edit" runat="server" OnSelectedIndexChanged="ddlist_customer_edit_SelectedIndexChanged"
                                                            AutoPostBack="True">
                                                        </asp:DropDownList>
                                                        &nbsp; Project
                                                        <asp:DropDownList ID="ddlist_project_edit" runat="server" AutoPostBack="True">
                                                        </asp:DropDownList>
                                                        &nbsp;
                                                        <asp:Label ID="Label1" runat="server" Text=" If you have confimed customer and project, please fill the box."
                                                            ForeColor="Green"></asp:Label>
                                                        <asp:CheckBox ID="ckbox_ok" runat="server" Checked="false"></asp:CheckBox>
                                                        <%-- By DingJunjie 20110512 Item 7 Delete Start --%><%--
                                                                <input type="hidden" id="hidproject" runat="server" />
                                                                <input type="hidden" id="hidsale" runat="server" />
                                                                --%><%-- By DingJunjie 20110512 Item 7 Delete End --%></asp:Panel>
                                                    <tbody>
                                                        <tr>
                                                            <td valign="top" align="left">
                                                                <%-- By DingJunjie 20110512 Item 7 Add Start --%><asp:UpdatePanel ID="upYTD1" runat="server">
                                                                    <ContentTemplate>
                                                                        <%-- By DingJunjie 20110512 Item 7 Add End --%>
                                                                        <%-- <div> --%>
                                                                        <%-- By DingJunjie 20110516 Item 4 Delete End --%>
                                                                        <%-- By DingJunjie 20110516 Item 4 Add Start --%>
                                                                        <div id="outbox1" style="overflow:auto; height:300px;"  onscroll="javascript:RecordPostion(this);" runat="server">
                                                                            <%-- By DingJunjie 20110516 Item 4 Add End --%>
                                                                            <asp:GridView ID="gv_bookingbydatebyproduct" runat="server" CellPadding="4" ForeColor="#333333"
                                                                                GridLines="None" OnRowEditing="gv_bookingbydatebyproduct_RowEditing" OnRowCancelingEdit="gv_bookingbydatebyproduct_RowCancelingEdit"
                                                                                OnRowDataBound="gv_bookingbydatebyproduct_RowDataBound" OnRowUpdating="gv_bookingbydatebyproduct_RowUpdating"
                                                                                OnRowDeleting="gv_bookingbydatebyproduct_RowDeleting" OnSelectedIndexChanging="gv_bookingbydatebyproduct_SelectedIndexChanging">
                                                                                <RowStyle BackColor="#EFF3FB" />
                                                                                <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                                                                <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                                                                                <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                                                                                <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                                                                <EditRowStyle BackColor="#33CCCC" />
                                                                                <AlternatingRowStyle BackColor="White" />
                                                                            </asp:GridView>
                                                                            <asp:HiddenField ID="HDOperationIds" runat="server"></asp:HiddenField>
                                                                        </div>
                                                                        <%-- By DingJunjie 20110512 Item 7 Add Start --%>
                                                                    </ContentTemplate>
                                                                </asp:UpdatePanel>
                                                                <asp:UpdatePanel ID="upYTD2" runat="server">
                                                                    <ContentTemplate>
                                                                        <%-- By DingJunjie 20110512 Item 7 Add End --%><asp:ImageButton ID="imgbtn_addYTD"
                                                                            OnClick="imgbtn_addYTD_Click" runat="server" ImageAlign="Bottom" ImageUrl="~/images/add.jpg"
                                                                            AlternateText="Add a row of new data"></asp:ImageButton>
                                                                        <asp:Panel ID="panel_addYTD" runat="server" Visible="false">
                                                                            <br />
                                                                            <asp:Label ID="label_countryYTD" runat="server" Width="100px" Text="SubRegion:"></asp:Label>
                                                                            <asp:DropDownList ID="ddlist_countryYTD" runat="server" Width="280px" OnSelectedIndexChanged="ddlist_countryYTD_SelectedIndexChanged"
                                                                                AutoPostBack="true">
                                                                            </asp:DropDownList>
                                                                            &nbsp;<br />
                                                                            <br />
                                                                            <%-- Edit By Forquan  2011-12-27  Bug Item 22 Begin--%><asp:Label ID="label_customerYTD"
                                                                                runat="server" Width="100px" Text="Customer:"></asp:Label>
                                                                            <%-- By DingJunjie 20110513 Item 7 Delete Start --%><%--
                                                                        <asp:DropDownList ID="ddlist_customerYTD" runat="server" Width="220px" AutoPostBack="true"
                                                                            OnSelectedIndexChanged="ddlist_customerYTD_SelectedIndexChanged">
                                                                        </asp:DropDownList>
                                                                        --%><%-- By DingJunjie 20110513 Item 7 Delete End --%><%-- By DingJunjie 20110513 Item 7 Add Start --%><asp:DropDownList
                                                                            ID="ddlist_customerYTD" runat="server" Width="800px">
                                                                        </asp:DropDownList>
                                                                            &nbsp;<%-- By DingJunjie 20110513 Item 7 Add End --%>&nbsp;<br />
                                                                            <br />
                                                                            <asp:Label ID="label_ProjectYTD" runat="server" Width="100px" Text="Project:"></asp:Label>
                                                                            <%--by yyan 20110630 itemw57 del start--%><%-- <asp:DropDownList ID="ddlist_projectYTD" runat="server" Width="220px" >
                                                                    </asp:DropDownList>--%><%--by yyan 20110630 itemw57 del end--%><%--by yyan 20110630 itemw57 add start--%><asp:TextBox
                                                                        ID="ddlist_projectYTD" onclick="showProNamePage('ddlist_projectYTD', 'hidOldProName');"
                                                                        runat="server" Width="280px"></asp:TextBox>
                                                                            <%-- Edit By Forquan  2011-12-27  Bug Item 22 End--%><asp:HiddenField ID="hidOldProName"
                                                                                runat="server"></asp:HiddenField>
                                                                            <%--by yyan 20110630 itemw57 add end--%><%-- By DingJunjie 20110512 Item 7 Add Start --%><asp:CheckBox
                                                                                ID="chkYTDIsCopy" runat="server" AutoPostBack="true" OnCheckedChanged="chkYTDIsCopy_CheckedChanged">
                                                                            </asp:CheckBox>
                                                                            <%-- By DingJunjie 20110512 Item 7 Add Start --%>&nbsp;&nbsp;
                                                                            <br />
                                                                            <br />
                                                                            <asp:Label ID="label_saleschannelYTD" runat="server" Text="Sales Channel:"></asp:Label>
                                                                            <asp:DropDownList ID="dropdownlist_saleschannelYTD" runat="server" Width="240px">
                                                                            </asp:DropDownList>
                                                                            &nbsp;&nbsp;
                                                                            <asp:Label ID="label_operationYTD" runat="server" Text="Operation:"></asp:Label>
                                                                            <asp:DropDownList ID="ddlist_operationYTD" runat="server" >
                                                                            </asp:DropDownList>
                                                                            &nbsp;&nbsp;
                                                                            <asp:Button ID="btn_addYTD" OnClick="btn_addYTD_Click" runat="server" Width="60px"
                                                                                Text="Add"></asp:Button>&nbsp;
                                                                            <asp:Button ID="btn_cancelYTD" OnClick="btn_cancelYTD_Click" runat="server" Width="60px"
                                                                                Text="Cancel"></asp:Button>
                                                                            <br />
                                                                            <asp:Label ID="lblError" runat="server" Visible="false"></asp:Label>
                                                                            <br />
                                                                            <asp:LinkButton ID="lbtn_addcustomerYTD" OnClick="lbtn_addcustomerYTD_Click" runat="server"
                                                                                Font-Bold="True">Add Customer</asp:LinkButton>
                                                                            <asp:Panel ID="panel_addcustomerYTD" runat="server">
                                                                                <table>
                                                                                    <tbody>
                                                                                        <tr>
                                                                                            <td>
                                                                                                <asp:Label ID="lbl_customernameYTD" runat="server" Text="Customer Name"></asp:Label>
                                                                                            </td>
                                                                                            <td>
                                                                                                <asp:TextBox ID="txtOldCusName" onclick="clearNewCusName();showCusNamePage('txtOldCusName', 'hidOldCusName');"
                                                                                                    runat="server" Width="300px"></asp:TextBox>
                                                                                                <asp:HiddenField ID="hidOldCusName" runat="server"></asp:HiddenField>
                                                                                            </td>
                                                                                            <td colspan="2">
                                                                                                <asp:TextBox ID="txt_customernameYTD" onclick="clearOldCusName();" runat="server"></asp:TextBox>
                                                                                            </td>
                                                                                            <td>
                                                                                                <asp:Label ID="Label2" runat="server" Text="If you want to create a new customer, please input customer name here."></asp:Label>
                                                                                            </td>
                                                                                        </tr>
                                                                                        <tr>
                                                                                            <td>
                                                                                                <asp:Label ID="label_customertypeYTD" runat="server" Text="Customer Type"></asp:Label>
                                                                                            </td>
                                                                                            <td>
                                                                                                <asp:DropDownList ID="ddlist_customertypeYTD" runat="server" Width="200px">
                                                                                                </asp:DropDownList>
                                                                                            </td>
                                                                                            <td align="right">
                                                                                                <asp:Label ID="lbl_saleschannelYTD" runat="server" Text="Sales Channel" Visible="false"></asp:Label>
                                                                                            </td>
                                                                                            <td colspan="3">
                                                                                                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:DropDownList
                                                                                                    ID="ddlist_saleschannelYTD" runat="server" Visible="false">
                                                                                                </asp:DropDownList>
                                                                                            </td>
                                                                                        </tr>
                                                                                        <tr>
                                                                                            <td>
                                                                                                <%-- By DingJunjie 20110516 Item 7 Delete Start --%><%-- <asp:Label ID="lbl_customercountryYTD" runat="server" Text="Country"></asp:Label> --%><%-- By DingJunjie 20110516 Item 7 Delete End --%><%-- By DingJunjie 20110516 Item 7 Add Start --%><asp:Label
                                                                                                    ID="lbl_customercountryYTD" runat="server" Text="SubRegion"></asp:Label>
                                                                                                <%-- By DingJunjie 20110516 Item 7 Add End --%>
                                                                                            </td>
                                                                                            <td>
                                                                                                <asp:DropDownList ID="ddlist_customercountryYTD" runat="server" Width="300px">
                                                                                                </asp:DropDownList>
                                                                                            </td>
                                                                                            <td colspan="2">
                                                                                                <asp:Label ID="lbl_customercityYTD" runat="server" Text="City"></asp:Label>&nbsp;
                                                                                                <asp:TextBox ID="tbox_customercityYTD" runat="server" Width="110px"></asp:TextBox>
                                                                                            </td>
                                                                                            <td>
                                                                                                <asp:Label ID="lbl_customeraddressYTD" runat="server" Text="Address"></asp:Label>&nbsp;&nbsp;<asp:TextBox
                                                                                                    ID="tbox_customeraddressYTD" runat="server" Width="200px"></asp:TextBox>
                                                                                            </td>
                                                                                        </tr>
                                                                                        <tr>
                                                                                            <td>
                                                                                                <asp:Label ID="lbl_customerdepartmentYTD" runat="server" Text="Department"></asp:Label>
                                                                                            </td>
                                                                                            <td>
                                                                                                <asp:TextBox ID="tbox_customerdepartmentYTD" runat="server" Width="200px"></asp:TextBox>
                                                                                            </td>
                                                                                            <td colspan="3">
                                                                                                <asp:Button ID="btn_customerAddYTD" OnClick="btn_customerYTD_add_Click" runat="server"
                                                                                                    Width="60px" Text="Add"></asp:Button>&nbsp;&nbsp;
                                                                                                <asp:Button ID="btn_customerCancelYTD" OnClick="btn_customerYTD_cancel_Click" runat="server"
                                                                                                    Width="60px" Text="Cancel"></asp:Button>
                                                                                            </td>
                                                                                        </tr>
                                                                                    </tbody>
                                                                                </table>
                                                                            </asp:Panel>
                                                                            <br />
                                                                            <asp:LinkButton ID="lbtn_addprojectYTD" OnClick="lbtn_addprojectYTD_Click" runat="server"
                                                                                Font-Bold="True">Add Project</asp:LinkButton>
                                                                            <asp:Panel ID="panel_addprojectYTD" runat="server">
                                                                                <div>
                                                                                    <asp:Label ID="label_project_nameYTD" runat="server" Text="Project Name"></asp:Label>
                                                                                    <asp:TextBox ID="tbox_project_nameYTD" runat="server" Width="200px"></asp:TextBox>
                                                                                    <%-- Edit By Forquan  2011-12-27  Bug Item 17 Begin--%><asp:Label ID="lbl_project_customerYTD"
                                                                                        runat="server" Text="Customer Name(POD)"></asp:Label>
                                                                                    <%-- Edit By Forquan  2011-12-27  Bug Item 17 End--%><asp:TextBox ID="txtProCusName"
                                                                                        onclick="showCusNamePage('txtProCusName', 'hidProCusName');" runat="server" Width="200px"></asp:TextBox>
                                                                                        <%--<asp:DropDownList
                                                                            ID="dplProjectCustomer" runat="server">
                                                                        </asp:DropDownList>--%>
                                                                                    <asp:HiddenField ID="hidProCusName" runat="server"></asp:HiddenField>
                                                                                    <asp:Label ID="lbl_currencyYTD" runat="server" Text="Currency"></asp:Label>
                                                                                    <asp:DropDownList ID="ddlist_currencyYTD" runat="server">
                                                                                    </asp:DropDownList>
                                                                                </div>
                                                                                <div>
                                                                                    <table>
                                                                                        <tbody>
                                                                                            <tr>
                                                                                                <td>
                                                                                                    <asp:Label ID="label_project_fromcountryYTD" runat="server" Text="Project Country(POS)"></asp:Label>
                                                                                                </td>
                                                                                                <td>
                                                                                                    <asp:Label ID="label_project_valueYTD" runat="server" Width="100px" Text="Project Value"></asp:Label>
                                                                                                </td>
                                                                                                <td>
                                                                                                    <asp:Label ID="label_project_probabilityYTD" runat="server" Width="100px" Text="% in budget"></asp:Label>
                                                                                                </td>
                                                                                                <td>
                                                                                                    <asp:Label ID="label_project_tocountryYTD" runat="server" Text="Project Country(POD)"></asp:Label>
                                                                                                </td>
                                                                                                <%--BY yyan 20110505 item 11 add start --%><td>
                                                                                                    <asp:Label ID="label3" runat="server" Text="Segment"></asp:Label>
                                                                                                </td>
                                                                                                <%--BY yyan 20110505 item 11 add end --%></tr>
                                                                                            <tr>
                                                                                                <td>
                                                                                                    <asp:DropDownList ID="ddlist_project_fromcountryYTD" runat="server">
                                                                                                    </asp:DropDownList>
                                                                                                </td>
                                                                                                <td>
                                                                                                    <asp:TextBox ID="tbox_project_valueYTD" runat="server"></asp:TextBox>
                                                                                                </td>
                                                                                                <td>
                                                                                                    <asp:TextBox ID="tbox_project_probabilityYTD" runat="server"></asp:TextBox>
                                                                                                </td>
                                                                                                <td>
                                                                                                    <asp:DropDownList ID="ddlist_project_tocountryYTD" runat="server">
                                                                                                    </asp:DropDownList>
                                                                                                </td>
                                                                                                <%--BY yyan 20110505 item 11 add start --%><td>
                                                                                                    <asp:DropDownList ID="ddlist_segmentYTD" runat="server">
                                                                                                    </asp:DropDownList>
                                                                                                </td>
                                                                                                <%--BY yyan 20110505 item 11 add end --%><td>
                                                                                                    <asp:Label ID="lblComments" runat="server" Text="Comments"></asp:Label>
                                                                                                    <asp:TextBox ID="txtComments" runat="server"></asp:TextBox>
                                                                                                </td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                                <td align="center" colspan="4">
                                                                                                    <asp:Button ID="btn_addprojectYTD" OnClick="btn_addprojectYTD_Click" runat="server"
                                                                                                        Width="60px" Text="Add"></asp:Button>&nbsp;
                                                                                                    <asp:Button ID="btn_CancelprojectYTD" OnClick="btn_CancelprojectYTD_Click" runat="server"
                                                                                                        Width="60px" Text="Cancel"></asp:Button>
                                                                                                </td>
                                                                                            </tr>
                                                                                        </tbody>
                                                                                    </table>
                                                                                </div>
                                                                            </asp:Panel>
                                                                            <br />
                                                                            <%-- By DingJunjie 20110513 Item 7 Add Start --%><asp:LinkButton ID="lbtn_modifyprojectYTD"
                                                                                OnClick="lbtn_modifyprojectYTD_Click" runat="server" Font-Bold="True">Modify Project</asp:LinkButton>
                                                                            <asp:Panel ID="p_ProjectYTD" runat="server" Visible="false">
                                                                                <asp:GridView ID="gv_ProjectYTD" runat="server" CellPadding="4" ForeColor="#333333"
                                                                                    GridLines="None" OnRowEditing="gv_Project_RowEditing" OnRowUpdating="gv_Project_RowUpdating"
                                                                                    OnRowCancelingEdit="gv_Project_RowCancelingEdit" OnRowDeleting="gv_Project_RowDeleting"
                                                                                    OnRowDataBound="gv_modifyprojectYTD_RowDataBound">
                                                                                    <RowStyle BackColor="#EFF3FB" />
                                                                                    <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                                                                    <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                                                                    <AlternatingRowStyle BackColor="White" />
                                                                                </asp:GridView>
                                                                                <br />
                                                                                <asp:Button ID="btnYTDProCancel" runat="server" Text="Cancel" Width="60px" OnClick="btnYTDProCancel_Click" />
                                                                            </asp:Panel>
                                                                            <asp:HiddenField ID="hidProject_Currency" runat="server" __designer:wfdid="w1"></asp:HiddenField>
                                                                            <br />
                                                                            <%-- By DingJunjie 20110513 Item 7 Add Start --%>
                                                                            
                                                                            <asp:LinkButton ID="lnkButtonModifyCustmer"
                                                                                 runat="server" Font-Bold="True" OnClick="lnkButtonModifyCustmer_Click" >Modify Customer</asp:LinkButton> 
                                                                            <asp:Panel ID="panelCustomer" runat="server" Visible="false">
                                                                            <asp:GridView ID="gv_Customer" runat="server" CellPadding="4" ForeColor="#333333" GridLines="None" OnRowEditing="gv_Customer_RowEditing"
                                                                             OnRowCancelingEdit="gv_Customer_RowCancelingEdit" OnRowDataBound="gv_Customer_RowDataBound" OnRowUpdating="gv_Customer_RowUpdating" OnRowDeleting="gv_Customer_RowDeleting" AllowPaging="true" OnPageIndexChanging="gv_Customer_PageIndexChanging" PageSize="20">
                                                                                    <RowStyle BackColor="#EFF3FB" />
                                                                                    <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                                                                    <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                                                                    <AlternatingRowStyle BackColor="White" />
                                                                            </asp:GridView>
                                                                            <br />
                                                                            <asp:Button ID="btnCusCancel" runat="server" Text="Cancel" Width="60px" OnClick="btnCusCancel_Click"  />
                                                                            <asp:HiddenField ID="hdCustType" runat="server" ></asp:HiddenField>
                                                                            <asp:HiddenField ID="hdCustRegion" runat="server" ></asp:HiddenField>
                                                                            </asp:Panel>  
                                                                            <br />
                                                                            
                                                                            
                                                                            <asp:Label ID="label_addYTD" runat="server"
                                                                                Text="">
                                                                               </asp:Label>
                                                                               
                                                                               
                                                                                
                                                                                
                                                                                
                                                                               
                                                                               
                                                                        </asp:Panel>
                                                                        <%-- By DingJunjie 20110512 Item 7 Add Start --%>
                                                                    </ContentTemplate>
                                                                </asp:UpdatePanel>
                                                                <%-- By DingJunjie 20110512 Item 7 Add End --%>
                                                            </td>
                                                        </tr>
                                                    </tbody>
                                                </table>
                                                <asp:GridView ID="gv_header" runat="server" GridLines="None" ForeColor="#333333"
                                                    CellPadding="4">
                                                    <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                                </asp:GridView>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <%-- By DingJunjie 20110524 Item 7 Add Start --%><asp:UpdatePanel ID="upNote" runat="server">
                                                    <ContentTemplate>
                                                        <%-- By DingJunjie 20110524 Item 7 Add End --%>
                                                        <asp:Label ID="label_note" runat="server" Text=""></asp:Label>
                                                        <%-- By DingJunjie 20110524 Item 7 Add Start --%>
                                                    </ContentTemplate>
                                                </asp:UpdatePanel>
                                                <%-- By DingJunjie 20110524 Item 7 Add End --%>
                                            </td>
                                        </tr>
                                    </tbody>
                                </table>
                            </div>
                        </div>
                        <hr />
                        <div style="width: 100%; text-align: center">
                            Siemens HP CRM Tool
                        </div>
                        </form>
                    </td>
                    <td style="width: 49px">
                    </td>
                </tr>
            </tbody>
        </table>
    </asp:Panel>
    <%-- By DingJunjie 20110516 Item 4 Add Start --%>

    <script type="text/javascript" language="javascript">
//        var flowtab1;
//        $(document).ready(function() {
//            flowtab1 = new flowThTable("outbox1", 1);
//            flowtab1.creatTable(300);
//        });
//        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function() {
//            flowtab1.creatTable(300);
//            // By DingJunjie 20110517 Item 7 Add Start
//            setButtom();
//            // By DingJunjie 20110517 Item 7 Add End

//            // By DingJunjie 20110602 ItemW28 Add Start
//            var tableName = document.getElementById("hidTableName").value;
//            var rowIndex = document.getElementById("hidRowIndex").value
//            if (tableName != '') {
//                var table = document.getElementById(tableName);
//                if (table != null) {
//                    if (table.rows[rowIndex] == null) {
//                        table.rows[rowIndex - 1].cells[0].focus();
//                    }
//                    else {
//                        table.rows[rowIndex].cells[0].focus();
//                    }
//                }
//            }
//            // By DingJunjie 20110602 ItemW28 Add End
//        });
//        // By DingJunjie 20110517 Item 7 Add Start
//        function setButtom() {
//            if (document.getElementById("hidButtomFlag").value == "true") {
//                document.documentElement.scrollTop = document.documentElement.scrollHeight;
//                document.getElementById("hidButtomFlag").value = "false";
//            }
//        }
//        // By DingJunjie 20110517 Item 7 Add End
    </script>

    <%-- By DingJunjie 20110516 Item 4 Add End --%>
</body>
</html>
