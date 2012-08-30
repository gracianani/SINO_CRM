/*++

Module Name:

    ExecutiveMasterPage.master.cs

Abstract:
    
    This is the master page of Executive. It defines the common layout and navigation, as well as the common default content,
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

public partial class Assistant_AssistantMasterPage : System.Web.UI.MasterPage
{
    LogUtility log = new LogUtility();
    GetMeetingDate date = new GetMeetingDate();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            log.WriteLog(LogUtility.LogErrorLevel.LOG_ACCESS, "Assistant Access.");
            lbl_welcom.Text = Session["WelcomStr"].ToString();

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