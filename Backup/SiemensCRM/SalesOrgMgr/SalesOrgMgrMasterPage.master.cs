/*++

Module Name:

    SalesOrgMgrMasterPage.master.cs

Abstract:
    
    This is the master page of SalesOrgMgr. It defines the common layout and navigation, as well as the common default content,
            for all of the content pages that are attached to it. 

Author:

    Wang Jun January-20-2010


Revision History:

--*/

using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;
using System.Collections.Generic;

public partial class SalesOrgMgr_SalesOrgMgrMasterPage : System.Web.UI.MasterPage
{
    GetMeetingDate date = new GetMeetingDate();
    LogUtility log = new LogUtility();
    CommonFunction cf = new CommonFunction();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            log.WriteLog(LogUtility.LogErrorLevel.LOG_ACCESS, "General Sales Organization Manager Access.");
            lbl_welcom.Text = Session["WelcomStr"].ToString();
            cf.unlockdataSales(Session["GeneralSalesOrgMgrID"].ToString().Trim());

            date.setDate();
            string str_displayYear = date.getyear();
            string str_displayMonth = date.getmonth();
            if (str_displayMonth.Equals("10"))
                str_displayYear = (int.Parse(str_displayYear) + 1).ToString().Trim();
            Session["MeetingDate"] = date.getMeetingName(int.Parse(str_displayMonth)) + " " + str_displayYear;

            lbl_currentmeetingdate.Text = Session["MeetingDate"].ToString().Trim();

            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["ServerName"]))
            {
                Label1.Visible = true;
                Label1.Text = ConfigurationManager.AppSettings["ServerName"];
            }
            else
                Label1.Visible = false;
        }
    }

    protected void lbtn_account_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/SalesOrgMgr/SalesOrgMgrAccountProfile.aspx");
    }
    protected void lbtn_bkgsales_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/SalesOrgMgr/SalesOrgMgrBookingSalesData.aspx");
    }
    protected void lbtn_forecast_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/SalesOrgMgr/SalesOrgMgrForecast.aspx");
    }
    protected void lbtn_offline_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/SalesOrgMgr/SalesOrgMgrWorkOffline.aspx");
    }
    protected void lbtn_info_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/SalesOrgMgr/SalesOrgMgrSalesOrgInfo.aspx");
    }
    protected void lbtn_reportview_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/SalesOrgMgr/SalesOrgMgrReportView.aspx");
    }
    // add by zy 20110121 start
    protected void lbtn_select_meeting_date_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/SalesOrgMgr/SalesOrgMgrSelectMeetingDate.aspx");
    }
    // add by zy 20110121 end
    protected void lbtn_password_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/SalesOrgMgr/SalesOrgMgrModifyPassword.aspx");
    }
    //By Mbq 20110504 ITEM 1 DEL Start 
    protected void btn_Logoff_Click(object sender, EventArgs e)
    {
        Session.Clear();
        Response.Redirect("~/SiemensCRMEnter.aspx");
    }
    //By Mbq 20110504 ITEM 1 DEL End 

    public void LockPage()
    {
        lblLock.Visible = true;
        Page.RegisterClientScriptBlock("", "<script>changebg();</script>");
    }

    public void UnLockPage()
    {
        lblLock.Visible = false;
    }
}
