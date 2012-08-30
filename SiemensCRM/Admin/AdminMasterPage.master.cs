/*++

Module Name:

    AdminMasterPage.master.cs

Abstract:
    
    This is the master page of Administrator. It defines the common layout and navigation, as well as the common default content,
            for all of the content pages that are attached to it. 

Author:

    Longran Wei January-20-2010


Revision History:

--*/

using System;
using System.Collections;
using System.Configuration; 
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;
using System.Collections.Generic;

public partial class Trench_MasterPage : System.Web.UI.MasterPage
{
    LogUtility log = new LogUtility();
    GetMeetingDate date = new GetMeetingDate();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            log.WriteLog(LogUtility.LogErrorLevel.LOG_ACCESS, "Administrator OR Sales OR Marketing Access.");

            lbl_welcom.Text = Session["WelcomStr"].ToString();
            date.setDate();
            string str_displayYear = date.getyear();
            string str_displayMonth = date.getmonth();
            if (str_displayMonth.Equals("10"))
                str_displayYear = (int.Parse(str_displayYear) + 1).ToString().Trim();
            Session["MeetingDate"] = date.getMeetingName(int.Parse(str_displayMonth)) + " " + str_displayYear;

            lbl_currentmeetingdate.Text = Session["MeetingDate"].ToString().Trim();

            this.form1.Target = "_self";

            //by yyan item59 20110705 add start
            if (Session["MenuValue"] == "")
            {
                for (int i = 0; i < navi_tree.Nodes.Count; i++)
                {
                    navi_tree.Nodes[i].Expand();
                    for (int j = 0; j < navi_tree.Nodes[i].ChildNodes.Count; j++)
                    {
                        navi_tree.Nodes[i].ChildNodes[j].Expanded = false;
                    }
     
                }
            } else {
                for (int i = 0; i < navi_tree.Nodes.Count; i++)
                {
                    navi_tree.Nodes[i].CollapseAll();
                }
                for (int i = 0; i < navi_tree.Nodes.Count; i++)
                {
                    if (Session["MenuValue"].ToString().Contains(navi_tree.Nodes[i].Text))
                    {
                        navi_tree.Nodes[i].Expanded = true;
                    }
                    for (int j = 0; j < navi_tree.Nodes[i].ChildNodes.Count; j++)
                    {
                        if (Session["MenuValue"].ToString().Contains(navi_tree.Nodes[i].ChildNodes[j].Text))
                        {
                            navi_tree.Nodes[i].ChildNodes[j].Expanded = true;
                        }
                    }
                }
            }
            //by yyan item59 20110705 add end

            if(!string.IsNullOrEmpty(ConfigurationManager.AppSettings["ServerName"]))
            {
                Label1.Visible=true;
                Label1.Text=ConfigurationManager.AppSettings["ServerName"];
            }
            else
                Label1.Visible=false;

        }
    }

    public Label LockLable
    {
        get { return this.lblLock; }
        set { this.lblLock = value; }
    }

    public Panel LockPanel
    {
        get { return this.Panel3; }
        set { this.Panel3 = value; }
    }

    public void LockPage()
    {
        lblLock.Visible = true;
        Page.RegisterClientScriptBlock("", "<script>changebg();</script>");
    }

    public void UnLockPage()
    {
        lblLock.Visible = false;
    }
    //By Mbq 20110504 ITEM 1 DEL Start 
    protected void btn_Logoff_Click(object sender, EventArgs e)
    {
        Session.Clear();
        Response.Redirect("~/SiemensCRMEnter.aspx");
    }
    //By Mbq 20110504 ITEM 1 DEL End 

    //by yyan item59 20110705 add start
    protected void navi_tree_SelectedNodeChanged(object sender, EventArgs e)
    {
        string menuName = "";
        for (int i = 0; i < navi_tree.Nodes.Count; i++)
        {
            if (navi_tree.Nodes[i].Expanded.Value)
            {
                menuName = menuName + navi_tree.Nodes[i].Text + ",";
            }
            for (int j = 0; j < navi_tree.Nodes[i].ChildNodes.Count; j++)
            {
                if (navi_tree.Nodes[i].ChildNodes[j].Expanded.Value)
                {
                    menuName = menuName + navi_tree.Nodes[i].ChildNodes[j].Text + ",";
                }
            }
        }
        Session["MenuValue"] = menuName;
        //by yyan itemw68 20110712 del start
        //if ("User Account".Equals(navi_tree.SelectedValue))
        //by yyan itemw68 20110712 del end
        //by yyan itemw68 20110712 add start
        if ("User Account".Equals(navi_tree.SelectedValue) || "User".Equals(navi_tree.SelectedValue))
        //by yyan itemw68 20110712 add end
        {
            Response.Redirect("~/Admin/AdminAccountProfile.aspx");
        }
        else if ("User Relation".Equals(navi_tree.SelectedValue))
        {
            Response.Redirect("~/Admin/AdminRSMInfo.aspx");
        }
        else if ("Modify Password".Equals(navi_tree.SelectedValue))
        {
            Response.Redirect("~/Admin/AdminModifyPassword.aspx");
        }
        //by yyan itemw68 20110712 del start
        //else if ("Segment/Product".Equals(navi_tree.SelectedValue))
        //by yyan itemw68 20110712 del end
        //by yyan itemw68 20110712 add start
        else if ("Segment/Product".Equals(navi_tree.SelectedValue) || "Master Data".Equals(navi_tree.SelectedValue))
        //by yyan itemw68 20110712 add end
        {
            Response.Redirect("~/Admin/AdminSegmentProduct.aspx");
        }
        else if ("Sales Channel".Equals(navi_tree.SelectedValue))
        {
            Response.Redirect("~/Admin/AdminSalesChannel.aspx");
        }
        else if ("Operation".Equals(navi_tree.SelectedValue))
        {
            Response.Redirect("~/Admin/AdminOperation.aspx");
        }
        else if ("Country Management".Equals(navi_tree.SelectedValue))
        {
            Response.Redirect("~/Admin/AdminCountry.aspx");
        }
        else if ("Country Relation".Equals(navi_tree.SelectedValue))
        {
            Response.Redirect("~/Admin/AdminRegion.aspx");
        }
        else if ("Sales Org".Equals(navi_tree.SelectedValue))
        {
            Response.Redirect("~/Admin/AdminSalesOrg.aspx");
        }
        else if ("Currency".Equals(navi_tree.SelectedValue))
        {
            Response.Redirect("~/Admin/AdminCurrencyDic.aspx");
        }
        else if ("Currency/Exchange".Equals(navi_tree.SelectedValue))
        {
            Response.Redirect("~/Admin/AdminCurrency.aspx");
        }
        else if ("Market".Equals(navi_tree.SelectedValue))
        {
            Response.Redirect("~/Admin/AdminMarket.aspx");
        }
        else if ("Project".Equals(navi_tree.SelectedValue))
        {
            Response.Redirect("~/Admin/AdminProjects.aspx");
        }
        else if ("Customer Name/Type".Equals(navi_tree.SelectedValue))
        {
            Response.Redirect("~/Admin/AdminCustomerRelation.aspx");
        }
        else if ("Customer Details".Equals(navi_tree.SelectedValue))
        {
            Response.Redirect("~/Admin/AdminCustomer.aspx");
        }
        else if ("Set Meeting Date".Equals(navi_tree.SelectedValue))
        {
            Response.Redirect("~/Admin/AdminSetMeetingDate.aspx");
        }
        else if ("Lock/Unlock System".Equals(navi_tree.SelectedValue))
        {
            Response.Redirect("~/Admin/AdminLockSystem.aspx");
        }
        else if ("DownLoad Data".Equals(navi_tree.SelectedValue))
        {
            Response.Redirect("~/Admin/AdminDownloadData.aspx");
        }
        //by yyan itemw68 20110712 del start
        //else if ("Booking/Sales Data".Equals(navi_tree.SelectedValue))
        //by yyan itemw68 20110712 del end
        //by yyan itemw68 20110712 add start
        else if ("Booking/Sales Data".Equals(navi_tree.SelectedValue) || "Upload Data".Equals(navi_tree.SelectedValue))
        //by yyan itemw68 20110712 add end
        {
            Response.Redirect("~/Admin/AdminUploadBookingSalesData.aspx");
        }
        else if ("Market Data".Equals(navi_tree.SelectedValue))
        {
            Response.Redirect("~/Admin/AdminUploadMarketData.aspx");
        }
        else if ("Project Data".Equals(navi_tree.SelectedValue))
        {
            Response.Redirect("~/Admin/AdminUploadProjectData.aspx");
        }
        //by yyan itemw68 20110712 del start
        //else if ("Booking Sales Data".Equals(navi_tree.SelectedValue))
        //by yyan itemw68 20110712 del end
        //by yyan itemw68 20110712 add start
        else if ("Booking Sales Data".Equals(navi_tree.SelectedValue) || "Input".Equals(navi_tree.SelectedValue))
        //by yyan itemw68 20110712 add end
        {
            Response.Redirect("~/Admin/AdminBookingSalesData.aspx");
        }
        else if ("Sales Data & Backlog".Equals(navi_tree.SelectedValue))
        {
            Response.Redirect("~/Admin/AdminSalesData.aspx");
        }
        //by yyan itemw68 20110712 del start
        //else if ("Mina Reports".Equals(navi_tree.SelectedValue))
        //by yyan itemw68 20110712 del end
        //by yyan itemw68 20110712 add start
        else if ("Mina Reports".Equals(navi_tree.SelectedValue) || "Report".Equals(navi_tree.SelectedValue))
        //by yyan itemw68 20110712 add end
        {
            Response.Redirect("~/Admin/AdminBookSalesForecast.aspx");
        }
        else if ("Standard Reports".Equals(navi_tree.SelectedValue))
        {
            Response.Redirect("~/Admin/AdminStandardReportView.aspx");
        }
        else if ("Individual Reports".Equals(navi_tree.SelectedValue))
        {
            Response.Redirect("~/Admin/AdminReportView.aspx");
        }
        else if ("Set Select Date".Equals(navi_tree.SelectedValue))
        {
            Response.Redirect("~/Admin/AdminSelectMeetingDate.aspx");
        }
    }
    //by yyan item59 20110705 add end

    
}