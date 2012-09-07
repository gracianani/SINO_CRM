/*
 * File Name   : RSMBookingSalesData.aspx.cs
 * 
 * Description : search booking data
 * 
 * Author      : ry zhang
 * 
 * Modify Date : 2011-5-5
 * 
 * Problem     : 
 * 
 * Version     : Release (2.0)
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.HtmlControls;
using System.Text;
using System.Collections;
using Microsoft.Office.Interop.Excel;
using System.IO;
using Resources;
using System.Drawing;


public partial class RSM_RSMBookSalesData : System.Web.UI.Page
{
    SQLHelper helper = new SQLHelper();
    LogUtility log = new LogUtility();
    DisplayInfo info = new DisplayInfo();
    WebUtility web = new WebUtility();
    GetMeetingDate meeting = new GetMeetingDate();
    SQLStatement sql = new SQLStatement();
    CommonFunction cf = new CommonFunction();
    SQLBookingInterface sqlBooking = new SQLBookingInterface();

    // by mbq 20110509 item13 add start   
    LockInterface LockInterface = new LockInterface();
    // by mbq 20110509 item13 add end   

    private static bool NullData;//没有数据
    private static bool convert_flag;
    /* Set Date */
    protected static string preyear;
    protected static string year;
    protected static string nextyear;
    protected static string yearAfterNext;
    protected static string month;
    protected static string premonth;
    protected const string fiscalStart = "Oct.1";
    protected const string fiscalEnd = "Sept.30";

    protected void Page_Load(object sender, EventArgs e)
    {
        Page.MaintainScrollPositionOnPostBack = true;
        if (getRoleID(getRole()) == "0" || getRoleID(getRole()) == "4")
        {
        }
        else
            Response.Redirect("~/AccessDenied.aspx");
        meeting.setDate();
        preyear = meeting.getpreyear();
        year = meeting.getyear();
        nextyear = meeting.getnextyear();
        yearAfterNext = meeting.getyearAfterNext();
        month = meeting.getmonth();
        premonth = meeting.getPreMonth(month);
        if (!IsPostBack)
        {
            log.WriteLog(LogUtility.LogErrorLevel.LOG_ACCESS, "RSMBookingSalesData Access.");
            panel_search.Visible = false;
            panel_head.Visible = false;
            bind(getSalesOrgInfo(), 1);
            // By DingJunjie 20110706 ItemW60 Add Start
            if (Request.QueryString.Count != 0)
            {
                this.ddlist_salesOrg.SelectedValue = Request.QueryString["SalesOrgID"];
            }
            // By DingJunjie 20110706 ItemW60 Add End

            //By Mbq 20110505 ITEM 1 ADD Start
            if (ddlist_salesOrg.SelectedItem != null)
            {
                bind(getSegmentInfo(ddlist_salesOrg.SelectedItem.Value.Trim()), 2);
            }
            else
            {
                bind(getSegmentInfo("".Trim()), 2);
            }
            //By Mbq 20110505 ITEM 1 ADD END
            bind(getCountryInfo(), 4);

            if (getRoleID(getRole()) == "5")
                lbtn_editRSM.Visible = false;
            else
                lbtn_editRSM.Visible = true;
            // By DingJunjie 20110706 ItemW60 Add Start
            if (Request.QueryString.Count != 0)
            {
                backInitPage(sender, e);
            }
            // By DingJunjie 20110706 ItemW60 Add End
        }

        //by mbq 20110511 item13 add start   
        //lockUser();
        //LockSegment();
        //by mbq 20110511 item13 add end  
    }

    protected string getRole()
    {
        return Session["Role"].ToString().Trim();
    }

    protected string getRoleID(string str_name)
    {
        string query_role = "SELECT ID FROM [Role] WHERE Name = '" + str_name + "'";
        DataSet ds_role = helper.GetDataSet(query_role);
        return ds_role.Tables[0].Rows[0][0].ToString().Trim();
    }

    protected DataSet getSalesOrgInfo()
    {
        //By Mbq 20110505 ITEM 1 DEL Start
        // string query_salesOrg = "SELECT Name,Abbr,ID FROM [SalesOrg] WHERE Deleted = 0 ORDER BY Abbr ASC";
        //By Mbq 20110505 ITEM 1 DEL End

        //By Mbq 20110505 ITEM 1 ADD Start
        string query_salesOrg = "SELECT [SalesOrg].Name, [SalesOrg].Abbr, [SalesOrg].ID FROM [SalesOrg] INNER JOIN [SalesOrg_User]"
                              + " ON [SalesOrg].ID = [SalesOrg_User].SalesOrgID"
                              + " WHERE [SalesOrg].Deleted = '0' AND [SalesOrg_User].Deleted = '0' AND [SalesOrg_User].UserID = " + getRSMID()
                              + " ORDER BY [SalesOrg].Name ASC";
        //By Mbq 20110505 ITEM 1 ADD End

        DataSet ds_salesOrg = helper.GetDataSet(query_salesOrg);

        if (ds_salesOrg.Tables[0].Rows.Count > 0)
            return ds_salesOrg;
        else
            return null;
    }

    /* * wj 20110120 start * *
    protected string getSalesOrgID(string Abbr)
    {
        string query_salesOrgID = "SELECT ID FROM [SalesOrg] WHERE Abbr = '" + Abbr + "'ORDER BY Name ASC";
        DataSet ds_salesOrgID = helper.GetDataSet(query_salesOrgID);

        if (ds_salesOrgID.Tables[0].Rows.Count > 0)
            return ds_salesOrgID.Tables[0].Rows[0][0].ToString().Trim();
        else
            return null;
    }
    */

    protected DataSet getSegmentInfo(string str_salesOrgID)
    {
        //By Mbq 20110505 ITEM 1 DLL Start
        /** string query_segment = "SELECT DISTINCT [Segment].ID,[Segment].Abbr"
                            + " FROM [SalesOrg_Segment] INNER JOIN [Segment] "
                            + " ON [SalesOrg_Segment].SegmentID  = [Segment].ID "
                            + " INNER JOIN [SalesOrg] "
                            + " ON [SalesOrg].ID = [SalesOrg_Segment].SalesOrgID AND [SalesOrg_Segment].Deleted ='0'"
                            + " WHERE [SalesOrg].Deleted = '0'  AND [SalesOrg].ID = '"
                            + str_salesOrgID + "'"
                            + " ORDER BY [Segment].Abbr ASC";
         **/
        //By Mbq 20110505 ITEM 1 DLL End

        //By Mbq 20110505 ITEM 1 ADD Start
        //string query_segment = "SELECT DISTINCT [Segment].ID,[Segment].Abbr"
        //                    + " FROM [SalesOrg_Segment] INNER JOIN [Segment] "
        //                    + " ON [SalesOrg_Segment].SegmentID  = [Segment].ID "
        //                    + " INNER JOIN [SalesOrg] "
        //                    + " ON [SalesOrg].ID = [SalesOrg_Segment].SalesOrgID AND [SalesOrg_Segment].Deleted ='0'"
        //                    + " WHERE [SalesOrg].Deleted = '0' AND [SalesOrg_Segment].Deleted ='0' AND [Segment].Deleted = '0' AND [SalesOrg].ID = '"
        //                    + str_salesOrgID + "'"
        //                    + " ORDER BY [Segment].Abbr ASC";
        ////By Mbq 20110505 ITEM 1 ADD End

        string query_segment = "SELECT DISTINCT [Segment].ID,[Segment].Abbr"
                          + " FROM [User_Segment] INNER JOIN [Segment] "
                          + " ON [User_Segment].SegmentID  = [Segment].ID "
                          + " WHERE [Segment].Deleted = '0' AND [User_Segment].Deleted ='0' AND [User_Segment].[UserID] = '"
                          + getRSMID() + "'"
                          + " ORDER BY [Segment].Abbr ASC";
        DataSet ds_segment = helper.GetDataSet(query_segment);

        if ((ds_segment.Tables.Count > 0) && (ds_segment.Tables[0].Rows.Count > 0))
            return ds_segment;
        else
            return null;
    }
    //By Mbq 20110505 ITEM 1 ADD Start
    protected string getRSMID()
    {
        return Session["RSMID"].ToString().Trim();
    }
    // 20110216 wy add start
    private string getCountrySQL()
    {
        //By Mbq 20110505 ITEM 1 DEL Start
        /**
        string query_country = "SELECT [SubRegion].ID AS SubRegion FROM [Country_SubRegion]  "
                            + " INNER JOIN [SubRegion] ON [SubRegion].ID = [Country_SubRegion].SubregionID  "
                            + " INNER JOIN [Country] ON [Country].ID = [Country_SubRegion].CountryID  "
                            + " INNER JOIN [User_Country] ON [User_Country].CountryID = [Country].ID "
                            + " WHERE UserID  IN (  "
                            + " SELECT [User].UserID FROM [User] "
                            + " INNER JOIN [User_Segment] ON [User_Segment].UserID = [User].UserID "
                            + " INNER JOIN [SalesOrg_User] ON [SalesOrg_User].UserID = [User].UserID "
                            + " WHERE [SalesOrg_User].SalesOrgID=" + ddlist_salesOrg.SelectedItem.Value.Trim()
                            + " AND [User_Segment].SegmentID=" + ddlist_segment.SelectedItem.Value.Trim()
                            + " GROUP BY  [User].UserID ) "
                            + " AND [User_Country].Deleted = 0 AND [SubRegion].Deleted = 0 AND UserID  = '" + getRSMID() + "'"
                            + " GROUP BY [SubRegion].ID, [SubRegion].Name";
        **/
        //By Mbq 20110505 ITEM 1 DEL End

        //By Mbq 20110505 ITEM 1 ADD Start
        string query_country = " SELECT [SubRegion].ID AS SubRegion"
                            + " FROM [User_Country] "
                            + " INNER JOIN [SubRegion] "
                            + " ON [User_Country].CountryID = [SubRegion].ID"
                            + " WHERE [User_Country].UserID  = '" + getRSMID() + "'"
                            + " AND [SubRegion].Deleted = 0 AND [User_Country].Deleted = 0";
        return query_country;
        //By Mbq 20110505 ITEM 1 ADD End
    }
    // 20110216 wy add end
    // get Country by segmentId
    protected DataSet getCountryInfo()
    {
        //By Mbq 20110505 ITEM 1 DEL Start
        /**
        string query_country = "SELECT [SubRegion].ID, [SubRegion].Name FROM [Country_SubRegion] "
                            + " INNER JOIN [SubRegion] ON [SubRegion].ID = [Country_SubRegion].SubregionID "
                            + " INNER JOIN [Country] ON [Country].ID = [Country_SubRegion].CountryID "
                            + " INNER JOIN [User_Country] ON [User_Country].CountryID = [Country].ID"
                            + " WHERE UserID  = " + getRSMID()
                            + " AND [User_Country].Deleted = 0 AND [SubRegion].Deleted = 0"
                            + " GROUP BY [SubRegion].ID, [SubRegion].Name";
        **/
        //By Mbq 20110505 ITEM 1 DEL End 
        //By Mbq 20110505 ITEM 1 ADD Start
        string query_country = " SELECT [SubRegion].ID, [SubRegion].Name AS SubRegion"
                            + " FROM [User_Country] "
                            + " INNER JOIN [SubRegion] "
                            + " ON [User_Country].CountryID = [SubRegion].ID"
                            + " WHERE [User_Country].UserID  = '" + getRSMID() + "'"
                            + " AND [SubRegion].Deleted = 0 AND [User_Country].Deleted = 0"
                            + " ORDER BY Name ASC";
        //By Mbq 20110505 ITEM 1 ADD End
        DataSet ds_country = helper.GetDataSet(query_country);

        if ((ds_country.Tables.Count > 0) && (ds_country.Tables[0].Rows.Count > 0))
            return ds_country;
        else
            return null;
    }

    protected DataSet getRSmInfo(string str_salesOrgID, string str_segmentID)
    {
        //string query_RSM = "SELECT [User].UserID, [User].Alias "
        //                    + " FROM [SalesOrg_Segment],[Segment],[SalesOrg],[User],[User_Segment],[SalesOrg_User] "
        //                    + " WHERE [SalesOrg_Segment].SegmentID  = [Segment].ID "
        //                    + " AND [SalesOrg].ID = [SalesOrg_Segment].SalesOrgID "
        //                    + " AND [User].UserID = [User_Segment].UserID "
        //                    + " AND [User_Segment].SegmentID = [Segment].ID "
        //                    + " AND [User].UserID = [SalesOrg_User].UserID "
        //                    + " AND [SalesOrg_User].SalesOrgID = [SalesOrg].ID "
        //                    + " AND [SalesOrg].Deleted = 0 "
        //                    + " AND [Segment].Deleted = 0 "
        //                    + " AND [SalesOrg_Segment].Deleted = 0 "
        //                    + " AND [User].Deleted = 0 "
        //                    + " AND [SalesOrg_User].Deleted = 0 "
        //                    + " AND [SalesOrg].ID = '" + str_salesOrgID + "'"
        //                    + " AND [Segment].ID = '" + str_segmentID + "'"
        //                    + " ORDER BY [User].Alias ASC";
        StringBuilder sql = new StringBuilder();
        sql.AppendLine(" SELECT ");
        sql.AppendLine("   [User].UserID, ");
        sql.AppendLine("   [User].Alias ");
        sql.AppendLine(" FROM ");
        sql.AppendLine("   [SalesOrg_Segment] ");
        sql.AppendLine("   INNER JOIN [SalesOrg] ON [SalesOrg_Segment].SalesOrgID=[SalesOrg].ID ");
        sql.AppendLine("   INNER JOIN [Segment] ON [SalesOrg_Segment].SegmentID=[Segment].ID ");
        sql.AppendLine("   INNER JOIN [SalesOrg_User] ON [SalesOrg].ID=[SalesOrg_User].SalesOrgID ");
        sql.AppendLine("   INNER JOIN [User_Segment] ON [Segment].ID=[User_Segment].SegmentID ");
        sql.AppendLine("   INNER JOIN [User] ON [User_Segment].UserID=[User].UserID AND [SalesOrg_User].UserID=[User].UserID ");
        sql.AppendLine(" WHERE ");
        sql.AppendLine("   [SalesOrg_Segment].Deleted=0 ");
        sql.AppendLine("   AND [SalesOrg].Deleted=0 ");
        sql.AppendLine("   AND [Segment].Deleted=0 ");
        sql.AppendLine("   AND [SalesOrg_User].Deleted=0 ");
        sql.AppendLine("   AND [User_Segment].Deleted=0 ");
        sql.AppendLine("   AND [User].Deleted=0 ");
        sql.AppendLine("   AND [SalesOrg].ID=" + str_salesOrgID);
        sql.AppendLine("   AND [Segment].ID=" + str_segmentID);
        sql.AppendLine("   AND [User].UserID=" + getRSMID());
        sql.AppendLine(" ORDER BY ");
        sql.AppendLine("   [User].Alias ASC ");
        DataSet ds_RSM = helper.GetDataSet(sql.ToString());

        if (ds_RSM.Tables[0].Rows.Count > 0)
            return ds_RSM;
        else
            return null;
    }

    protected string getAbbrByUserID(string str_userID)
    {
        string query_RSM = " SELECT Abbr FROM [User] WHERE [User].Deleted = 0 AND UserID = '" + str_userID + "'";
        DataSet ds_RSM = helper.GetDataSet(query_RSM);

        if (ds_RSM.Tables[0].Rows.Count > 0)
            return ds_RSM.Tables[0].Rows[0][0].ToString().Trim();
        else
            return "";
    }

    protected void btn_local_Click(object sender, EventArgs e)
    {
        convert_flag = false;
        this.hidConvertFlag.Value = "false";
        //bindStatus(ddlist_RSM.SelectedItem.Value.Trim(), ddlist_segment.SelectedItem.Value.Trim());
        //setImageEnabled(ddlist_RSM.SelectedItem.Value.Trim(), ddlist_segment.SelectedItem.Value.Trim());
        // By DingJunjie 20110525 Delete Start
        //helper.ExecuteNonQuery(CommandType.Text, "DELETE FROM [Bookings] WHERE Amount = 0", null);
        // By DingJunjie 20110525 Delete Start
        bindDataSource();

        btn_local.Enabled = false;
        btn_EUR.Enabled = true;
        // By DingJunjie 20110706 ItemW60 Add Start
        this.label_currency.Text = this.btn_local.Text;
        // By DingJunjie 20110706 ItemW60 Add End

        //by mbq 20110511 item13 add start   
        lockUser();
        LockSegment();
        //by mbq 20110511 item13 add end  
    }

    protected void btn_EUR_Click(object sender, EventArgs e)
    {
        convert_flag = true;
        this.hidConvertFlag.Value = "true";
        //bindStatus(ddlist_RSM.SelectedItem.Value.Trim(), ddlist_segment.SelectedItem.Value.Trim());
        //setImageEnabled(ddlist_RSM.SelectedItem.Value.Trim(), ddlist_segment.SelectedItem.Value.Trim());
        // By DingJunjie 20110525 Delete Start
        //helper.ExecuteNonQuery(CommandType.Text, "DELETE FROM [Bookings] WHERE Amount = 0", null);
        // By DingJunjie 20110525 Delete Start
        bindDataSource();

        btn_local.Enabled = true;
        btn_EUR.Enabled = false;
        // By DingJunjie 20110706 ItemW60 Add Start
        this.label_currency.Text = this.btn_EUR.Text;
        // By DingJunjie 20110706 ItemW60 Add End

        //by mbq 20110511 item13 add start   
        lockUser();
        LockSegment();
        //by mbq 20110511 item13 add end  
    }

    protected DataSet getCurrencyBySalesOrgID(string str_salesorgID)
    {
        string query_currency = " SELECT [Currency].Name, [Currency].ID FROM [SalesOrg] INNER JOIN [Currency]"
                              + " ON [SalesOrg].CurrencyID = [Currency].ID"
                              + " WHERE [SalesOrg].ID = '" + str_salesorgID + "' AND [SalesOrg].Deleted = 0"
                              + " AND [Currency].Deleted = 0";
        DataSet ds_currency = helper.GetDataSet(query_currency);

        if (ds_currency.Tables.Count > 0 && ds_currency.Tables[0].Rows.Count > 0)
            return ds_currency;
        else
            return null;
    }

    /* * wj 20110120 * *
    protected string getCurrencyBySalesOrg(string str_salesorgID)
    {
        string cur = "Error";

        string query_currency = " SELECT [Currency].Name, [Currency].ID FROM [SalesOrg] INNER JOIN [Currency]"
                              + " ON [SalesOrg].CurrencyID = [Currency].ID"
                              + " WHERE [SalesOrg].ID = '" + str_salesorgID + "' AND [SalesOrg].Deleted = 0"
                              + " AND [Currency].Deleted = 0";
        DataSet ds_currency = helper.GetDataSet(query_currency);

        if (ds_currency.Tables.Count > 0 && ds_currency.Tables[0].Rows.Count > 0)
        {
            cur = ds_currency.Tables[0].Rows[0][0].ToString();
            label_currency.Text = "K" + ds_currency.Tables[0].Rows[0][0].ToString();
        }
        else
        {
            cur = "";
            label_currency.Text = "Error";
        }
        return cur;
    }

    protected string getAbbrBySalesOrg()
    {
        int start = ddlist_salesOrg.SelectedItem.Text.Trim().IndexOf('(');
        return ddlist_salesOrg.SelectedItem.Text.Trim().Substring(start + 1, ddlist_salesOrg.SelectedItem.Text.Trim().Length - 2 - start);
    }
     * * */

    private static int countRSM = 0;
    private static int indexRSM = 0;

    protected void bind(DataSet ds, int sel)//1:SalesOrg  2:Segment  3:RSM  4:Country
    {
        if (ds != null)
        {
            System.Data.DataTable dt = ds.Tables[0];
            int count = dt.Rows.Count;
            int index = 0;

            if (sel == 4)
            {
                ListItem item = new ListItem("", "");
                ddlist_country.Items.Insert(0, item);
            }

            while (index < count)
            {
                if (sel == 1)
                {
                    ListItem li = new ListItem(dt.Rows[index][1].ToString().Trim(), dt.Rows[index][2].ToString());
                    ddlist_salesOrg.Items.Add(li);
                }
                if (sel == 2)
                {
                    ListItem li = new ListItem(dt.Rows[index][1].ToString().Trim(), dt.Rows[index][0].ToString().Trim());
                    ddlist_segment.Items.Add(li);
                }
                if (sel == 3)
                {
                    ListItem li = new ListItem(dt.Rows[index][1].ToString().Trim(), dt.Rows[index][0].ToString().Trim());
                    ddlist_RSM.Items.Add(li);
                }

                if (sel == 4)
                {
                    ListItem li = new ListItem(dt.Rows[index][1].ToString().Trim(), dt.Rows[index][0].ToString().Trim());
                    ddlist_country.Items.Add(li);
                }

                index++;
            }

            if (sel == 1)
            {
                ddlist_salesOrg.SelectedIndex = 0;
                ddlist_salesOrg.Enabled = true;
            }
            if (sel == 2)
            {
                ddlist_segment.SelectedIndex = 0;
                ddlist_segment.Enabled = true;
                btn_search.Enabled = true;
            }
            if (sel == 3)
            {
                countRSM = count;
                indexRSM = ddlist_RSM.Items.IndexOf(ddlist_RSM.Items.FindByValue(ddlist_RSM.SelectedItem.Value.ToString().Trim()));
                ddlist_RSM.SelectedIndex = indexRSM;
                ddlist_RSM.Enabled = true;
            }

            if (sel == 4)
            {
                ddlist_country.SelectedIndex = 0;
                ddlist_country.Enabled = true;
            }
        }
        else
        {
            if (sel == 1)
                ddlist_salesOrg.Enabled = false;
            if (sel == 2)
            {
                ddlist_segment.Enabled = false;
                ddlist_segment.Items.Add(new ListItem("", "-1"));
                btn_search.Enabled = false;
            }
            if (sel == 3)
            {
                ddlist_RSM.Enabled = false;
                ddlist_RSM.Items.Add(new ListItem("", "-1"));
            }

            if (sel == 4)
            {
                ddlist_country.Enabled = false;
                ddlist_country.Items.Add(new ListItem("", "-1"));
            }
        }
    }

    protected void ddlist_salesOrg_SelectedIndexChanged(object sender, EventArgs e)
    {
        ddlist_segment.Items.Clear();
        // add by zy 20110117 start
        ddlist_country.Items.Clear();
        // add by zy 20110117 end
        bind(getSegmentInfo(ddlist_salesOrg.SelectedItem.Value.Trim()), 2);
        //By Mbq 20110505 ITEM 1 DEL Start
        //By yyan 20110622 ITEM w48 add Start
        bind(getCountryInfo(), 4);
        //By yyan 20110622 ITEM w48 add end
        //By Mbq 20110505 ITEM 1 DEL End
        //wj 20110120
        //ddlist_salesOrg.ToolTip = ddlist_salesOrg.Text;

        //by mbq 20110511 item13 add start   
        lockUser();
        LockSegment();
        //by mbq 20110511 item13 add end  
    }

    //wj 20110120
    //private static bool searchClick = false;
    protected void btn_search_Click(object sender, EventArgs e)
    {
        //wj 20110120
        //searchClick = true;
        ddlist_RSM.Items.Clear();
        panel_search.Visible = true;
        panel_head.Visible = true;

        string str_segment = ddlist_segment.SelectedItem.Text.ToString().Trim();
        string str_salesorgID = ddlist_salesOrg.SelectedItem.Value.Trim();
        //wj 20110120 end
        label_salesorgAbbr.Text = ddlist_salesOrg.SelectedItem.Text.Trim();

        if (str_segment != "")
            label_headdescription.Text = str_segment + " -BOOKINGS " + meeting.getyear();
        else
            label_headdescription.Text = "Please add segments to " + ddlist_salesOrg.SelectedItem.Text.Trim() + ".";

        bind(getRSmInfo(str_salesorgID, ddlist_segment.SelectedItem.Value.Trim()), 3);
        if (this.ddlist_RSM.Items.FindByValue(Session["RSMID"].ToString()) != null)
        {
            this.ddlist_RSM.SelectedValue = Session["RSMID"].ToString();
        }

        //wj 20110120
        DataSet ds_currency = getCurrencyBySalesOrgID(str_salesorgID);
        if (ds_currency != null)
            label_currency.Text = "K" + ds_currency.Tables[0].Rows[0][0].ToString().Trim();
        else
            label_currency.Text = "No Currency";
        btn_local.Text = label_currency.Text;
        //getCurrencyBySalesOrg(str_salesorgID);
        //end

        //bindStatus(ddlist_RSM.SelectedItem.Value.Trim(), ddlist_segment.SelectedItem.Value.Trim());
        GetUserStatus(ddlist_RSM.SelectedItem.Value.Trim(), ddlist_salesOrg.SelectedItem.Value.Trim());
        setImageEnabled(ddlist_RSM.SelectedItem.Value.Trim(), ddlist_segment.SelectedItem.Value.Trim(), ddlist_salesOrg.SelectedValue.Trim());

        this.hidBookingY.Value = year.Substring(2, 2);
        this.hidDeliverY.Value = "YTD";
        this.hidGVType.Value = "1";

        // By DingJunjie 20110525 Delete Start
        //helper.ExecuteNonQuery(CommandType.Text, "DELETE FROM [Bookings] WHERE Amount = 0", null);
        // By DingJunjie 20110525 Delete Start
        // By DingJunjie 20110706 ItemW60 Add Start
        initDataTypeList();
        convert_flag = false;
        this.btn_local.Enabled = false;
        this.btn_EUR.Enabled = true;
        this.label_currency.Text = this.btn_local.Text;
        // By DingJunjie 20110706 ItemW60 Add End
        bindDataSource();

        //by mbq 20110511 item13 add start   
        lockUser();
        LockSegment();
        //by mbq 20110511 item13 add end  
    }

    protected void ddlist_RSM_SelectedIndexChanged(object sender, EventArgs e)
    {
        //bindStatus(ddlist_RSM.SelectedItem.Value.Trim(), ddlist_segment.SelectedItem.Value.Trim());
        GetUserStatus(ddlist_RSM.SelectedItem.Value.Trim(), ddlist_salesOrg.SelectedItem.Value.Trim());
        setImageEnabled(ddlist_RSM.SelectedItem.Value.Trim(), ddlist_segment.SelectedItem.Value.Trim(), ddlist_salesOrg.SelectedValue.Trim());

        // By DingJunjie 20110525 Delete Start
        //helper.ExecuteNonQuery(CommandType.Text, "DELETE FROM [Bookings] WHERE Amount = 0", null);
        // By DingJunjie 20110525 Delete Start
        initDataTypeList();
        bindDataSource();

        //by mbq 20110511 item13 add start   
        lockUser();
        LockSegment();
        //by mbq 20110511 item13 add end  
    }

    /* GridView Booking Data */

    /// <summary>
    /// Get booking data of every product
    /// </summary>
    /// <param name="dsPro"></param>
    /// <param name="segmentID"></param>
    /// <param name="RSMID"></param>
    /// <param name="bookingY"></param>
    /// <param name="deliverY"></param>
    /// <returns></returns>
    protected DataSet getBookingDataByDateByProduct(DataSet dsPro, string segmentID, string RSMID, string bookingY, string deliverY)
    {
        string salesorgID = ddlist_salesOrg.SelectedItem.Value.Trim();
        string countryID = ddlist_country.SelectedItem.Value.Trim();
        string strSQL = sqlBooking.getBookingSalesDataByBD(RSMID, salesorgID, segmentID, countryID, year, month, getCountrySQL());
        return sql.getBookingSalesDataByBD(strSQL, dsPro, salesorgID, bookingY, deliverY, year, month, convert_flag);
    }


    protected void bindDataByDateByProduct(GridView gv, string bookingY, string deliverY, int number)
    {
        DataSet ds_product = getProductBySegment(ddlist_segment.SelectedItem.Value.Trim());
        if (ds_product == null)
        {
            NullData = true;
            gv.Visible = false;
            return;
        }
        if (ddlist_RSM.SelectedItem.Text == "")
        {
            gv.Visible = false;
            NullData = true;
            return;
        }
        NullData = false;
        DataSet ds = getBookingDataByDateByProduct(ds_product, ddlist_segment.SelectedItem.Value, ddlist_RSM.SelectedItem.Value, bookingY, deliverY);
        if (ds != null)
        {
            if (ds.Tables[0].Rows.Count == 0)
            {
                sql.getNullDataSet(ds);
            }
            gv.Width = Unit.Pixel(800);
            gv.AutoGenerateColumns = false;
            gv.AllowPaging = false;
            gv.Visible = true;
            for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
            {
                BoundField bf = new BoundField();
                bf.DataField = ds.Tables[0].Columns[i].ColumnName.ToString();
                bf.HeaderText = ds.Tables[0].Columns[i].Caption.ToString();
                bf.ReadOnly = true;
                bf.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
                bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
                if (i >= 10)
                {
                    if (i % 3 == 0)
                    {
                        bf.HeaderText = null;
                        bf.ControlStyle.Width = Unit.Pixel(15);
                        bf.ItemStyle.Width = Unit.Pixel(15);
                    }
                    else if (i % 3 == 2)
                    {
                        bf.ItemStyle.HorizontalAlign = HorizontalAlign.Right;
                        bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Right;
                        bf.ControlStyle.Width = Unit.Pixel(50);
                        bf.ItemStyle.Width = Unit.Pixel(50);
                    }
                }
                gv.Columns.Add(bf);
            }
            if (deliverY == "YTD")
            {
                gv.Caption = bookingY + deliverY + "  " + fiscalStart + "," + preyear + " to " + meeting.getMonth(month) + meeting.getDay() + "," + year;
            }
            else if (bookingY == year.Substring(2, 2).Trim())
            {
                gv.Caption = bookingY + " for " + deliverY + "  " + meeting.getMonth(month) + meeting.getDay() + "," + year + " to " + fiscalEnd + "," + bookingY + " for " + deliverY + " delivery";
            }
            else
            {
                gv.Caption = bookingY + " for " + deliverY + "  " + fiscalStart + "," + year + " to " + fiscalEnd + "," + bookingY + " for " + deliverY + " delivery";
            }
            gv.CaptionAlign = TableCaptionAlign.Top;
            gv.AllowSorting = true;
            gv.DataSource = ds.Tables[0];
            gv.DataBind();
            gv.Columns[0].Visible = false;
            gv.Columns[1].Visible = false;
            gv.Columns[2].Visible = false;
            gv.Columns[3].Visible = false;
            gv.Columns[4].Visible = false;
            for (int i = 11; i < gv.Columns.Count; i += 3)
            {
                gv.Columns[i + 2].Visible = false;
            }
            string str_data = null;
            string str_comments = null;
            if (gv.Rows.Count > 0)
            {
                for (int i = 0; i < gv.Rows.Count; i++)
                {
                    for (int j = 11; j < gv.Columns.Count; j += 3)
                    {
                        str_data = gv.Rows[i].Cells[j].Text.Replace("&nbsp;", string.Empty);
                        str_comments = gv.Rows[i].Cells[j + 2].Text.Replace("&nbsp;", string.Empty);
                        if (!string.Equals(str_data, "0") && str_comments.Length != 0)
                        {
                            gv.Rows[i].Cells[j].ForeColor = Color.Red;
                            gv.Rows[i].Cells[j].ToolTip = str_comments;
                        }
                    }
                }
            }
            for (int i = 0; i < gv.Rows.Count; i++)
            {
                addCellsAttributes(gv.Rows[i], 6, 1);
            }
        }
        else
        {
            gv.Visible = false;
        }
    }

    // add by zy 20101228 start
    protected void addCellsAttributes(GridViewRow row, int colShowIndex, int colIndex)
    {
        if (row.Cells[0].Text != "&nbsp;")
        {
            if (row.Cells[colIndex].Text != "&nbsp;")
            {
                row.Cells[colShowIndex].Attributes["onclick"] = "window.open('../CustomerDetail.aspx?customerID=" + row.Cells[colIndex].Text + "&salesChannelID=" + row.Cells[colIndex + 2].Text + "','Customer', 'height=200,width=810,top=100,left=200,toolbar=no,status=no, menubar=no,location=no,resizable=yes,scrollbars=no');";
                row.Cells[colShowIndex].Attributes["title"] = "Customer Detail";
                row.Cells[colShowIndex].Text = "<a href='#'>" + row.Cells[colShowIndex].Text + "</a>";
            }
            if (row.Cells[colIndex + 1].Text != "&nbsp;")
            {
                row.Cells[colShowIndex + 1].Text = "<a href='#'>" + row.Cells[colShowIndex + 1].Text + "</a>";
                row.Cells[colShowIndex + 1].Attributes["onclick"] = "window.open('../ProjectDetail.aspx?projectID=" + row.Cells[colIndex + 1].Text + "&customerID=" + row.Cells[colIndex].Text + "','Project', 'height=150,width=810,top=100,left=200,toolbar=no,status=no, menubar=no,location=no,resizable=yes,scrollbars=no');";
                row.Cells[colShowIndex + 1].Attributes["title"] = "Project Detail";
            }
        }
    }
    // add by zy 20101228 end

    /// <summary>
    /// Get booking total data of every product by operation
    /// </summary>
    /// <param name="dsPro"></param>
    /// <param name="segmentID"></param>
    /// <param name="RSMID"></param>
    /// <param name="bookingY"></param>
    /// <param name="deliverY"></param>
    /// <returns></returns>

    protected DataSet getBookingDataTotalByDateByProduct(DataSet dsPro, string segID, string RSMID, string bookingY, string deliverY)
    {
        string salesorgID = ddlist_salesOrg.SelectedItem.Value.Trim();
        //By Wsy 20110526 ITEM 43 DEL Start 
        //string countryID = ddlist_country.SelectedItem.Value.Trim();
        //By Wsy 20110526 ITEM 43 DEL End 

        //By Wsy 20110526 ITEM 43 ADD Start 
        string countryID = ddlist_country.SelectedValue.Trim();
        //By Wsy 20110526 ITEM 43 ADD End 
        string rsmAbbr = getAbbrByUserID(ddlist_RSM.SelectedItem.Value);

        //20110120 wj
        string sqlstr = sqlBooking.getBookingDataTotal(salesorgID, dsPro, segID, rsmAbbr, RSMID, bookingY, deliverY, year, month, countryID, convert_flag, getCountrySQL());
        if (sqlstr != "")
        {
            DataSet ds = helper.GetDataSet(sqlstr);

            if (ds.Tables[0].Rows.Count > 0)
                return ds;
            else
                return null;
        }
        else
            return null;
    }

    protected void bindDataTotalByDateByProduct(GridView gv, string bookingY, string deliverY)
    {
        DataSet ds_product = getProductBySegment(ddlist_segment.SelectedItem.Value);
        if (ds_product == null)
        {
            gv.Visible = false;
            return;
        }
        if (ddlist_RSM.SelectedItem.Text == "")
        {
            gv.Visible = false;
            return;
        }
        DataSet ds = getBookingDataTotalByDateByProduct(ds_product, ddlist_segment.SelectedItem.Value, ddlist_RSM.SelectedItem.Value, bookingY, deliverY);

        if (ds != null)
        {
            // update by zy 20110128 start
            //gv.Width = Unit.Pixel(800);
            gv.Width = Unit.Pixel(842);
            // update by zy 20110128 end
            gv.AutoGenerateColumns = false;
            gv.AllowPaging = false;
            gv.Visible = true;

            for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
            {
                BoundField bf = new BoundField();

                bf.DataField = ds.Tables[0].Columns[i].ColumnName.ToString();
                bf.HeaderText = ds.Tables[0].Columns[i].Caption.ToString();
                bf.ItemStyle.HorizontalAlign = HorizontalAlign.Right;
                bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Right;

                if (i == 0)
                {
                    bf.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
                    bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;

                    bf.ItemStyle.Width = 280;
                }
                bf.ReadOnly = true;

                gv.Columns.Add(bf);
            }

            gv.AllowSorting = true;
            gv.DataSource = ds.Tables[0];

            DataRow drSum = ds.Tables[0].NewRow();
            DataRow[] rows = ds.Tables[0].Select();

            float[] Sum = new float[20];
            for (int j = 1; j < ds.Tables[0].Columns.Count; j++)
            {
                Sum[j] = 0;
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    Sum[j] += float.Parse(ds.Tables[0].Rows[i][j].ToString());
                }
            }

            for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
            {
                if (i == 0)
                {
                    drSum[0] = "TTL/" + getAbbrByUserID(ddlist_RSM.SelectedItem.Value);
                }
                else
                {
                    drSum[i] = Sum[i].ToString();
                }
            }
            ds.Tables[0].Rows.InsertAt(drSum, 0);

            gv.DataBind();

        }
        else
            gv.Visible = false;
    }

    /* Gridview Booking Data By Product */

    /// <summary>
    /// Get booking total dat of product by every country
    /// </summary>
    /// <param name="productID"></param>
    /// <param name="RSMID"></param>
    /// <returns></returns>

    protected DataSet getProductBySegment(string segmentID)
    {
        if (ddlist_segment.SelectedItem.Text != "")
        {
            string query_product = "SELECT ID,Abbr FROM [Product] INNER JOIN [Segment_Product] ON [Segment_Product].ProductID = [Product].ID "
                           + " WHERE SegmentID = " + segmentID + " AND [Product].Deleted = 0 AND [Segment_Product].Deleted = 0 ORDER BY Abbr";
            DataSet ds_product = helper.GetDataSet(query_product);

            if (ds_product.Tables[0].Rows.Count > 0)
                return ds_product;
            else
                return null;
        }
        else
            return null;
    }

    protected DataSet getBookingsDataByProduct(string productID, string RSMID)
    {
        string salesorgID = ddlist_salesOrg.SelectedItem.Value.Trim();
        string countryID = ddlist_country.SelectedItem.Value.Trim();
        string segmentID = ddlist_segment.SelectedItem.Value.Trim();
        string sqlstr = sqlBooking.getBookingSalesDataByProduct(RSMID, salesorgID, segmentID,
            countryID, productID, year, month, preyear, convert_flag, getCountrySQL());
        DataSet ds = helper.GetDataSet(sqlstr);
        if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
        {
            return null;
        }
        else
        {
            return ds;
        }
    }

    protected GridView showBookingsByProduct(DataSet ds, GridView gv, string header)
    {
        if (ds != null && !NullData)
        {
            gv.Visible = true;
            gv.AutoGenerateColumns = false;
            gv.Width = Unit.Pixel(400);
            gv.Columns.Clear();
            try
            {
                ds.Tables[0].Columns.Add("VAR");
                if (ds.Tables[0].Rows[0][0] != DBNull.Value)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        float tmp = 0;
                        if (float.Parse(dr[9].ToString()) != 0)
                        {
                            tmp = (float.Parse(dr[8].ToString()) - float.Parse(dr[9].ToString())) * 100 / float.Parse(dr[9].ToString());
                            dr["VAR"] = Convert.ToInt32(tmp).ToString() + "%";
                        }
                    }
                }
            }
            catch
            {
                log.WriteLog(LogUtility.LogErrorLevel.LOG_ERROR, "Format string to float Error.");
                Response.Redirect("~/Admin/AdminError.aspx");
            }
            for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
            {
                BoundField bf = new BoundField();

                bf.DataField = ds.Tables[0].Columns[i].ColumnName.ToString();
                bf.HeaderText = ds.Tables[0].Columns[i].Caption.ToString();
                bf.ItemStyle.HorizontalAlign = HorizontalAlign.Right;
                bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Right;
                if (i < 8)
                {
                    bf.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
                    bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
                }

                gv.Columns.Add(bf);
            }
            gv.Caption = header;
            gv.CaptionAlign = TableCaptionAlign.Top;
            gv.DataSource = ds;
            gv.DataBind();
            gv.Columns[0].Visible = false;
            gv.Columns[1].Visible = false;
            gv.Columns[2].Visible = false;
            gv.Columns[3].Visible = false;
            for (int i = 0; i < gv.Rows.Count; i++)
            {
                addCellsAttributes(gv.Rows[i], 6, 1);
            }
        }
        else
        {
            gv.Visible = false;
        }
        return gv;
    }

    /* GridView Booking Total Data By Product */

    /// <summary>
    /// Get booking total data of a product by operation
    /// </summary>
    /// <param name="productID"></param>
    /// <param name="RSMID"></param>
    /// <returns></returns>
    protected DataSet getBookingDataTotalByProduct(string productID, string RSMID)
    {
        string salesorgID = ddlist_salesOrg.SelectedItem.Value.Trim();
        string countryID = ddlist_country.SelectedItem.Value.Trim();
        string rsmAbbr = getAbbrByUserID(RSMID);
        string segmentID = ddlist_segment.SelectedItem.Value.Trim();
        string sqlstr = sqlBooking.getBookingSalesDataTotleByProduct(RSMID, rsmAbbr, salesorgID,
            segmentID, countryID, productID, year, month, preyear, convert_flag, getCountrySQL());
        DataSet ds = helper.GetDataSet(sqlstr);
        if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
        {
            return null;
        }
        else
        {
            return ds;
        }
    }

    protected GridView showTotalByProduct(DataSet ds, GridView gv)
    {
        if (ds != null && !NullData)
        {
            gv.Visible = true;
            gv.AutoGenerateColumns = false;
            // update by zy 20110128 start
            //gv.Width = Unit.Pixel(400);
            gv.Width = Unit.Pixel(449);
            // update by zy 20110128 end
            gv.Columns.Clear();

            //Calculate the VAR column and Total row of next year.
            try
            {
                ds.Tables[0].Columns.Add("VAR");
                if (ds.Tables[0].Rows[0][0] != DBNull.Value)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        float tmp = 0;
                        if (float.Parse(dr[2].ToString()) != 0)
                        {
                            tmp = (float.Parse(dr[1].ToString()) - float.Parse(dr[2].ToString())) * 100 / float.Parse(dr[2].ToString());
                            dr["VAR"] = Convert.ToInt32(tmp).ToString() + "%";
                        }
                    }
                }
            }
            catch
            {
                log.WriteLog(LogUtility.LogErrorLevel.LOG_ERROR, "Format string to float Error.");
                Response.Redirect("~/Admin/AdminError.aspx");
            }
            for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
            {
                BoundField bf = new BoundField();

                bf.DataField = ds.Tables[0].Columns[i].ColumnName.ToString();
                bf.HeaderText = ds.Tables[0].Columns[i].Caption.ToString();
                bf.ItemStyle.HorizontalAlign = HorizontalAlign.Right;
                bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Right;

                if (i == 0)
                {
                    bf.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
                    bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
                    bf.ItemStyle.Width = 220;
                }

                bf.ReadOnly = true;
                gv.Columns.Add(bf);
            }
            gv.DataSource = ds;

            DataRow drSum = ds.Tables[0].NewRow();
            DataRow[] rows = ds.Tables[0].Select();

            float[] Sum = new float[20];
            for (int j = 1; j < ds.Tables[0].Columns.Count - 1; j++)
            {
                Sum[j] = 0;
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    Sum[j] += float.Parse(ds.Tables[0].Rows[i][j].ToString());
                }
            }

            for (int i = 0; i < ds.Tables[0].Columns.Count - 1; i++)
            {
                if (i == 0)
                {
                    drSum[0] = "TTL/" + getAbbrByUserID(ddlist_RSM.SelectedItem.Value);
                }
                else
                {
                    drSum[i] = Sum[i].ToString();
                }
            }

            ds.Tables[0].Rows.InsertAt(drSum, 0);
            if (float.Parse(ds.Tables[0].Rows[0][2].ToString()) != 0)
                ds.Tables[0].Rows[0][3] = (Convert.ToInt32((float.Parse(ds.Tables[0].Rows[0][1].ToString()) - float.Parse(ds.Tables[0].Rows[0][2].ToString())) * 100 / float.Parse(ds.Tables[0].Rows[0][2].ToString()))).ToString() + "%";

            gv.DataBind();
        }
        else
            gv.Visible = false;
        return gv;
    }

    /* GridView Booking Total Data AND Forecast Data This Fiscal Year By Country */

    /// <summary>
    /// Get booking real amount this year and booking estimating amount last year
    /// </summary>
    /// <param name="segmentID"></param>
    /// <param name="RSMID"></param>
    /// <returns></returns>
    protected DataSet getBookingsDataThisyear(DataSet dsPro, string segmentID, string RSMID)
    {
        string salesorgID = ddlist_salesOrg.SelectedItem.Value.Trim();
        string countryID = ddlist_country.SelectedItem.Value.Trim();
        string strSQL = sqlBooking.getBookingSalesDataByBD(RSMID, salesorgID, segmentID, countryID, year, month, getCountrySQL());
        DataSet ds = sql.getBookingSalesDataThisYear(strSQL, dsPro, salesorgID, year, month, convert_flag);
        if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
        {
            return null;
        }
        else
        {
            return ds;
        }
    }

    protected void bindDataByDate(GridView gv)
    {
        DataSet ds = getBookingsDataThisyear(getProductBySegment(ddlist_segment.SelectedItem.Value.ToString().Trim()),
            ddlist_segment.SelectedItem.Value.ToString().Trim(), ddlist_RSM.SelectedItem.Value.ToString().Trim());
        if (ds != null && !NullData)
        {
            gv.Visible = true;
            gv.Width = Unit.Pixel(800);
            gv.AutoGenerateColumns = false;
            gv.AllowPaging = false;
            ds.Tables[0].Columns.Add("Total");
            try
            {
                if (ds.Tables[0].Rows[0][0] != DBNull.Value)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        float tmp = 0;
                        for (int count2 = 10; count2 < ds.Tables[0].Columns.Count - 2; count2 += 2)
                        {
                            tmp += float.Parse(dr[count2].ToString());
                        }
                        dr["Total"] = tmp.ToString();
                    }
                }
            }
            catch
            {
                log.WriteLog(LogUtility.LogErrorLevel.LOG_ERROR, "Format string to float Error.");
                Response.Redirect("~/Admin/AdminError.aspx");
            }
            BoundField bf = null;
            for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
            {
                bf = new BoundField();
                bf.DataField = ds.Tables[0].Columns[i].ColumnName.ToString();
                bf.HeaderText = ds.Tables[0].Columns[i].Caption.ToString();

                bf.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
                bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Right;
                if (i == 5 || i == 6 || i == 7 || i == 8)
                {
                    bf.ItemStyle.Width = 150;
                    bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
                }
                if (i == 9)
                {
                    bf.ItemStyle.Width = 50;
                    bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
                }
                if (i > 9)
                {
                    if (i % 2 != 0)
                    {
                        bf.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
                        bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
                        bf.HeaderText = null;
                    }
                    else
                    {
                        bf.ItemStyle.HorizontalAlign = HorizontalAlign.Right;
                        bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Right;
                    }
                }
                bf.ReadOnly = true;
                gv.Columns.Add(bf);
            }
            gv.Caption = "Total(" + year.Substring(2, 2) + ")";
            gv.CaptionAlign = TableCaptionAlign.Top;
            gv.AllowSorting = true;
            gv.DataSource = ds.Tables[0];
            gv.DataBind();
            gv.Columns[0].Visible = false;
            gv.Columns[1].Visible = false;
            gv.Columns[2].Visible = false;
            gv.Columns[3].Visible = false;
            gv.Columns[4].Visible = false;
            for (int i = 0; i < gv.Rows.Count; i++)
            {
                addCellsAttributes(gv.Rows[i], 6, 1);
            }
        }
        else
        {
            gv.Visible = false;
        }
    }

    /* GridView Booking Total Data By Operation This Year*/

    /// <summary>
    /// Get booking total amount this year
    /// </summary>
    /// <param name="segmentID"></param>
    /// <param name="RSMID"></param>
    /// <returns></returns>
    protected DataSet getBookingDataTotalThisYear(DataSet dsPro, string segmentID, string RSMID)
    {
        string salesorgID = ddlist_salesOrg.SelectedItem.Value.Trim();
        string countryID = ddlist_country.SelectedItem.Value.Trim();
        string rsmAbbr = getAbbrByUserID(RSMID);
        string sqlstr = sqlBooking.getBookingDataTotalThisYear(dsPro, salesorgID, segmentID,
            rsmAbbr, RSMID, year, month, countryID, convert_flag, getCountrySQL());
        if (sqlstr != "")
        {
            DataSet ds = helper.GetDataSet(sqlstr);
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                return ds;
            }
            else
            {
                return null;
            }
        }
        else
        {
            return null;
        }
    }

    protected void bindDataTotalByDate(GridView gv)
    {
        DataSet ds = getBookingDataTotalThisYear(getProductBySegment(ddlist_segment.SelectedItem.Value.ToString().Trim()),
            ddlist_segment.SelectedItem.Value.ToString().Trim(), ddlist_RSM.SelectedItem.Value.ToString().Trim());
        if (ds != null && !NullData)
        {
            gv.Visible = true;
            gv.Width = Unit.Pixel(886);
            gv.AutoGenerateColumns = false;
            gv.AllowPaging = false;
            ds.Tables[0].Columns.Add("Total");
            try
            {
                if (ds.Tables[0].Rows[0][0] != DBNull.Value)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        float tmp = 0;
                        for (int count2 = 1; count2 < ds.Tables[0].Columns.Count - 1; count2++)
                        {
                            tmp += float.Parse(dr[count2].ToString());
                        }
                        dr["Total"] = tmp.ToString();
                    }
                }
            }
            catch
            {
                log.WriteLog(LogUtility.LogErrorLevel.LOG_ERROR, "Format string to float Error.");
                Response.Redirect("~/Admin/AdminError.aspx");
            }
            BoundField bf = null;
            for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
            {
                bf = new BoundField();
                bf.DataField = ds.Tables[0].Columns[i].ColumnName.ToString();
                bf.HeaderText = ds.Tables[0].Columns[i].Caption.ToString();
                bf.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
                bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
                if (i == 0)
                {
                    bf.ItemStyle.Width = 200;
                }
                if (i != 0)
                {
                    bf.ItemStyle.HorizontalAlign = HorizontalAlign.Right;
                    bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Right;
                }
                bf.ReadOnly = true;
                gv.Columns.Add(bf);
            }
            gv.AllowSorting = true;
            gv.DataSource = ds.Tables[0];
            DataRow drSum = ds.Tables[0].NewRow();
            DataRow[] rows = ds.Tables[0].Select();
            float[] Sum = new float[20];
            for (int j = 1; j < ds.Tables[0].Columns.Count; j++)
            {
                Sum[j] = 0;
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    Sum[j] += float.Parse(ds.Tables[0].Rows[i][j].ToString());
                }
            }
            for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
            {
                if (i == 0)
                {
                    drSum[0] = "TTL/" + getAbbrByUserID(ddlist_RSM.SelectedItem.Value);
                }
                else
                {
                    drSum[i] = Sum[i].ToString();
                }
            }
            ds.Tables[0].Rows.InsertAt(drSum, 0);
            gv.DataBind();
        }
        else
        {
            gv.Visible = false;
        }
    }

    /* GridView Booking Total Data By Country Next Year */

    /// <summary>
    /// Get
    /// </summary>
    /// <param name="dsPro"></param>
    /// <param name="segmentID"></param>
    /// <param name="RSMID"></param>
    /// <returns></returns>
    protected DataSet getBookingsDataNextYear(DataSet dsPro, string segmentID, string RSMID)
    {
        string salesorgID = ddlist_salesOrg.SelectedItem.Value.Trim();
        string countryID = ddlist_country.SelectedItem.Value.Trim();
        string strSQL = sqlBooking.getBookingSalesDataByBD(RSMID, salesorgID, segmentID, countryID, year, month, getCountrySQL());
        DataSet ds = sql.getBookingSalesDataNextYear(strSQL, dsPro, salesorgID, year, month, nextyear, convert_flag);
        if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
        {
            return null;
        }
        else
        {
            return ds;
        }
    }

    protected void bindDataNextByDate(GridView gv)
    {
        DataSet ds = getBookingsDataNextYear(getProductBySegment(ddlist_segment.SelectedItem.Value.ToString().Trim()), ddlist_segment.SelectedItem.Value.ToString().Trim(), ddlist_RSM.SelectedItem.Value.ToString().Trim());
        if (ds != null && !NullData)
        {
            gv.Visible = true;
            gv.Width = Unit.Pixel(800);
            gv.AutoGenerateColumns = false;
            gv.AllowPaging = false;
            ds.Tables[0].Columns.Add("Total");
            try
            {
                if (ds.Tables[0].Rows[0][0] != DBNull.Value)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        float tmp = 0;
                        for (int count2 = 9; count2 < ds.Tables[0].Columns.Count - 2; count2 += 2)
                        {
                            tmp += float.Parse(dr[count2].ToString());
                        }
                        dr["Total"] = tmp.ToString();
                    }
                }
            }
            catch
            {
                log.WriteLog(LogUtility.LogErrorLevel.LOG_ERROR, "Format string to float Error.");
                Response.Redirect("~/Admin/AdminError.aspx");
            }
            BoundField bf = null;
            for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
            {
                bf = new BoundField();
                bf.DataField = ds.Tables[0].Columns[i].ColumnName.ToString();
                bf.HeaderText = ds.Tables[0].Columns[i].Caption.ToString();

                bf.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
                bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Right;
                if (i == 4 || i == 5 || i == 6 || i == 7)
                {
                    bf.ItemStyle.Width = 150;
                    bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
                }
                if (i == 8)
                {
                    bf.ItemStyle.Width = 50;
                    bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
                }
                if (i > 8)
                {
                    if (i % 2 == 0)
                    {
                        bf.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
                        bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
                        bf.HeaderText = null;
                    }
                    else
                    {
                        bf.ItemStyle.HorizontalAlign = HorizontalAlign.Right;
                        bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Right;
                    }
                }
                bf.ReadOnly = true;
                gv.Columns.Add(bf);
            }
            gv.Caption = "Total(" + nextyear.Substring(2, 2) + ")";
            gv.CaptionAlign = TableCaptionAlign.Top;
            gv.AllowSorting = true;
            gv.DataSource = ds.Tables[0];
            gv.DataBind();
            gv.Columns[0].Visible = false;
            gv.Columns[1].Visible = false;
            gv.Columns[2].Visible = false;
            gv.Columns[3].Visible = false;
            for (int i = 0; i < gv.Rows.Count; i++)
            {
                addCellsAttributes(gv.Rows[i], 6, 1);
            }
        }
        else
        {
            gv.Visible = false;
        }
    }

    /* GridView Booking Total Data By Operation Next Year*/

    /// <summary>
    /// Get
    /// </summary>
    /// <param name="dsPro"></param>
    /// <param name="segmentID"></param>
    /// <param name="RSMID"></param>
    /// <returns></returns>

    protected DataSet getBookingDataTotalNextYear(DataSet dsPro, string segmentID, string RSMID)
    {
        string salesorgID = ddlist_salesOrg.SelectedItem.Value.Trim();
        //By Wsy 20110526 ITEM 43 DEL Start 
        //string countryID = ddlist_country.SelectedItem.Value.Trim();
        //By Wsy 20110526 ITEM 43 DEL End 

        //By Wsy 20110526 ITEM 43 ADD Start 
        string countryID = ddlist_country.SelectedValue.Trim();
        //By Wsy 20110526 ITEM 43 ADD End 
        string rsmAbbr = getAbbrByUserID(RSMID);

        string sqlstr = sqlBooking.getBookingDataTotalByNextYear(salesorgID, dsPro, segmentID, rsmAbbr, RSMID, year, month, nextyear, countryID, convert_flag, getCountrySQL());
        if (sqlstr != "")
        {
            DataSet ds = helper.GetDataSet(sqlstr);

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                return ds;
            else
                return null;
        }
        else
            return null;
    }

    protected void bindDataNextTotalByDate(GridView gv)
    {
        DataSet ds = getBookingDataTotalNextYear(getProductBySegment(ddlist_segment.SelectedItem.Value.ToString().Trim()), ddlist_segment.SelectedItem.Value.ToString().Trim(), ddlist_RSM.SelectedItem.Value.ToString().Trim());
        if (ds != null && !NullData)
        {
            gv.Visible = true;
            // update by zy 20110128 start
            //gv.Width = Unit.Pixel(800);
            gv.Width = Unit.Pixel(886);
            // update by zy 20110128 end
            gv.AutoGenerateColumns = false;
            gv.AllowPaging = false;

            //Calculate the total column of next year.
            ds.Tables[0].Columns.Add("Total");
            try
            {
                if (ds.Tables[0].Rows[0][0] != DBNull.Value)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        float tmp = 0;
                        for (int count2 = 1; count2 < ds.Tables[0].Columns.Count - 1; count2++) //Edit by  Sino Bug27
                        {
                            tmp += float.Parse(dr[count2].ToString());
                        }
                        dr["Total"] = tmp.ToString();
                    }
                }
            }
            catch
            {
                log.WriteLog(LogUtility.LogErrorLevel.LOG_ERROR, "Format string to float Error.");
                Response.Redirect("~/RSM/RSMError.aspx");
            }

            for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
            {
                BoundField bf = new BoundField();

                bf.DataField = ds.Tables[0].Columns[i].ColumnName.ToString();
                bf.HeaderText = ds.Tables[0].Columns[i].Caption.ToString();
                bf.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
                bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;

                if (i == 0)
                    bf.ItemStyle.Width = 200;

                if (i != 0)
                {
                    bf.ItemStyle.HorizontalAlign = HorizontalAlign.Right;
                    bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Right;
                }
                bf.ReadOnly = true;
                gv.Columns.Add(bf);
            }

            gv.AllowSorting = true;
            gv.DataSource = ds.Tables[0];

            DataRow drSum = ds.Tables[0].NewRow();
            DataRow[] rows = ds.Tables[0].Select();

            float[] Sum = new float[20];
            for (int j = 1; j < ds.Tables[0].Columns.Count; j++)
            {
                Sum[j] = 0;
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    Sum[j] += float.Parse(ds.Tables[0].Rows[i][j].ToString());
                }
            }

            for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
            {
                if (i == 0)
                {
                    drSum[0] = "TTL/" + getAbbrByUserID(ddlist_RSM.SelectedItem.Value);
                }
                else
                {
                    drSum[i] = Sum[i].ToString();
                }
            }
            ds.Tables[0].Rows.InsertAt(drSum, 0);
            gv.DataBind();
        }
        else
            gv.Visible = false;
    }

    /* Comparsion */
    protected DataSet getBookingsDataVSPre(DataSet dsPro, string segmentID, string RSMID)
    {
        string salesorgID = ddlist_salesOrg.SelectedItem.Value.Trim();
        string countryID = ddlist_country.SelectedItem.Value.Trim();
        string thisYearSQL = sqlBooking.getBookingSalesDataByBD(RSMID, salesorgID, segmentID, countryID, year, month, getCountrySQL());
        string thisYearVSSQL = sqlBooking.getBookingSalesDataByBD(RSMID, salesorgID, segmentID, countryID, year, premonth, getCountrySQL());
        string preYearVSSQL = sqlBooking.getBookingSalesDataByBD(RSMID, salesorgID, segmentID, countryID, preyear, premonth, getCountrySQL());
        DataSet ds = sql.getBookingSalesDataThisYearVSPreYear(thisYearSQL, thisYearVSSQL, preYearVSSQL, dsPro, salesorgID, year, month, preyear, premonth, convert_flag);
        if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
        {
            return null;
        }
        else
        {
            return ds;
        }
    }

    protected void bindDataVSPre(GridView gv)
    {
        if (ddlist_RSM.SelectedItem.Text == "")
        {
            gv.Visible = false;
            return;
        }
        DataSet ds = getBookingsDataVSPre(getProductBySegment(ddlist_segment.SelectedItem.Value), ddlist_segment.SelectedItem.Value, ddlist_RSM.SelectedItem.Value);
        if (ds != null && !NullData)
        {
            gv.Width = Unit.Pixel(700);
            gv.AutoGenerateColumns = false;
            gv.AllowPaging = false;
            gv.Visible = true;
            ds.Tables[0].Columns.Add("Total");
            try
            {
                if (ds.Tables[0].Rows[0][0] != DBNull.Value)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        float tmp = 0;
                        for (int count2 = 9; count2 < ds.Tables[0].Columns.Count - 3; count2++)
                        {
                            tmp += float.Parse(dr[count2].ToString());
                        }
                        dr["Total"] = tmp.ToString();
                    }
                }
            }
            catch
            {
                log.WriteLog(LogUtility.LogErrorLevel.LOG_ERROR, "Format string to float Error.");
                Response.Redirect("~/Admin/AdminError.aspx");
            }
            BoundField bf = null;
            for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
            {
                bf = new BoundField();
                bf.DataField = ds.Tables[0].Columns[i].ColumnName;
                bf.HeaderText = ds.Tables[0].Columns[i].Caption;
                bf.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
                bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Right;
                if (i < 9)
                {
                    bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
                }
                if (i > 8)
                {
                    bf.ItemStyle.HorizontalAlign = HorizontalAlign.Right;
                    bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Right;
                }
                bf.ReadOnly = true;
                gv.Columns.Add(bf);
            }
            gv.Caption = "Total" + year.Substring(2, 2) + "(" + preyear.Substring(2, 2) + ")";
            gv.CaptionAlign = TableCaptionAlign.Top;
            gv.AllowSorting = true;
            gv.DataSource = ds.Tables[0];
            gv.DataBind();
            gv.Columns[0].Visible = false;
            gv.Columns[1].Visible = false;
            gv.Columns[2].Visible = false;
            gv.Columns[3].Visible = false;
            for (int i = 0; i < gv.Rows.Count; i++)
            {
                addCellsAttributes(gv.Rows[i], 6, 1);
            }
        }
        else
        {
            gv.Visible = false;
        }
    }

    protected DataSet getBookingDataTotalVSPre(DataSet dsPro, string segmentID, string RSMID)
    {
        string salesorgID = ddlist_salesOrg.SelectedItem.Value.Trim();
        //By Wsy 20110526 ITEM 43 DEL Start 
        //string countryID = ddlist_country.SelectedItem.Value.Trim();
        //By Wsy 20110526 ITEM 43 DEL End 

        //By Wsy 20110526 ITEM 43 ADD Start 
        string countryID = ddlist_country.SelectedValue.Trim();
        //By Wsy 20110526 ITEM 43 ADD End 
        string rsmAbbr = getAbbrByUserID(RSMID);

        string sqlstr = sqlBooking.getBookingDataTotaByThislVSPre(salesorgID, dsPro, segmentID, rsmAbbr, RSMID, year, month, preyear, premonth, countryID, convert_flag, getCountrySQL());
        if (sqlstr != "")
        {
            DataSet ds = helper.GetDataSet(sqlstr);

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                return ds;
            else
                return null;
        }
        else
            return null;
    }

    protected void bindDataTotalVSPre(GridView gv)
    {
        if (ddlist_RSM.SelectedItem.Text == "")
        {
            gv.Visible = false;
            return;
        }
        DataSet ds = getBookingDataTotalVSPre(getProductBySegment(ddlist_segment.SelectedItem.Value), ddlist_segment.SelectedItem.Value, ddlist_RSM.SelectedItem.Value);
        if (ds != null && !NullData)
        {
            // update by zy 20110128 start
            //gv.Width = Unit.Pixel(700);
            gv.Width = Unit.Pixel(752);
            // update by zy 20110128 end
            gv.AutoGenerateColumns = false;
            gv.AllowPaging = false;
            // by daixuesong 20110527 item W15 add start 
            gv.Visible = true;
            // by daixuesong 20110527 item W15 add end

            //Calculate the total column of next year.
            ds.Tables[0].Columns.Add("Total");
            try
            {
                if (ds.Tables[0].Rows[0][0] != DBNull.Value)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        float tmp = 0;
                        for (int count2 = 1; count2 < ds.Tables[0].Columns.Count - 1; count2++)
                        {
                            tmp += float.Parse(dr[count2].ToString());
                        }
                        dr["Total"] = tmp.ToString();
                    }
                }
            }
            catch
            {
                log.WriteLog(LogUtility.LogErrorLevel.LOG_ERROR, "Format string to float Error.");
                Response.Redirect("~/RSM/RSMError.aspx");
            }

            for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
            {
                BoundField bf = new BoundField();

                bf.DataField = ds.Tables[0].Columns[i].ColumnName.ToString();
                bf.HeaderText = ds.Tables[0].Columns[i].Caption.ToString();
                bf.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
                bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;

                if (i == 0)
                    bf.ItemStyle.Width = 200;

                if (i != 0)
                {
                    bf.ItemStyle.HorizontalAlign = HorizontalAlign.Right;
                    bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Right;
                }
                bf.ReadOnly = true;
                gv.Columns.Add(bf);
            }

            gv.AllowSorting = true;
            gv.DataSource = ds.Tables[0];

            DataRow drSum = ds.Tables[0].NewRow();
            DataRow[] rows = ds.Tables[0].Select();

            float[] Sum = new float[20];
            for (int j = 1; j < ds.Tables[0].Columns.Count; j++)
            {
                Sum[j] = 0;
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    Sum[j] += float.Parse(ds.Tables[0].Rows[i][j].ToString());
                }
            }

            for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
            {
                if (i == 0)
                {
                    drSum[0] = "TTL/" + getAbbrByUserID(ddlist_RSM.SelectedItem.Value);
                }
                else
                {
                    drSum[i] = Sum[i].ToString();
                }
            }
            ds.Tables[0].Rows.InsertAt(drSum, 0);
            gv.DataBind();
        }
        // by daixuesong 20110527 item W15 add start
        else
        {
            gv.Visible = false;
        }
        // by daixuesong 20110527 item W15 add end
    }

    protected void lbtn_editRSM_Click(object sender, EventArgs e)
    {
        if (ddlist_country.SelectedItem == null)
        {
            //By Wsy 20110527 ITEM W16 DEL Start
            //Response.Redirect("~/BookingSalesData.aspx?SalesOrgID=" + ddlist_salesOrg.SelectedItem.Value.Trim() + "&SegmentID=" + ddlist_segment.SelectedItem.Value.Trim() + "&RSMID=" + ddlist_RSM.SelectedItem.Value.Trim() + "&CountryID=-1");
            //By Wsy 20110527 ITEM W16 DEL End

            //By Wsy 20110527 ITEM W16 ADD Start
            Response.Redirect("~/BookingSalesData.aspx?SalesOrgID=" + ddlist_salesOrg.SelectedItem.Value.Trim() + "&SegmentID=" + ddlist_segment.SelectedItem.Value.Trim() + "&RSMID=" + ddlist_RSM.SelectedItem.Value.Trim() + "&CountryID=-1&UserID=" + getRSMID() + "&DataType=" + this.ddlDataType.SelectedValue);
            //By Wsy 20110527 ITEM W16 ADD End
        }
        else
        {
            //By Mbq 20110511 ITEM 1 del Start
            //Response.Redirect("~/BookingSalesData.aspx?SalesOrgID=" + ddlist_salesOrg.SelectedItem.Value.Trim() + "&SegmentID=" + ddlist_segment.SelectedItem.Value.Trim() + "&RSMID=" + ddlist_RSM.SelectedItem.Value.Trim() + "&CountryID=" + ddlist_country.SelectedItem.Value.Trim());
            //By Mbq 20110511 ITEM 1 del Start
            //By Mbq 20110511 ITEM 1 Add Start
            Response.Redirect("~/BookingSalesData.aspx?SalesOrgID=" + ddlist_salesOrg.SelectedItem.Value.Trim() + "&SegmentID=" + ddlist_segment.SelectedItem.Value.Trim() + "&RSMID=" + ddlist_RSM.SelectedItem.Value.Trim() + "&CountryID=" + ddlist_country.SelectedItem.Value.Trim() + "&UserID=" + getRSMID() + "&DataType=" + this.ddlDataType.SelectedValue);
            //By Mbq 20110511 ITEM 1 Add End
        }
    }

    #region
    public override void VerifyRenderingInServerForm(Control control)
    {
        // Confirms that an HtmlForm control is rendered for
    }

    protected void btn_export_Click(object sender, EventArgs e)
    {
        System.Globalization.CultureInfo CurrentCI = System.Threading.Thread.CurrentThread.CurrentCulture;
        System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
        bindDataSource();

        Application excel = new Application();
        Workbook book = excel.Workbooks.Add(XlWBATemplate.xlWBATWorksheet);
        Worksheet sheet = (Worksheet)book.Sheets[1];
        sheet.Name = "Sheet1";
        ArrayList gvList = new ArrayList();
        ComputeSubControls(this.div_export, typeof(GridView), gvList);
        int rowIndex = 1;
        int nullCellFlag = 0;
        int totleMagerNum = 3;
        if (string.Equals(this.ddlDataType.SelectedValue, "P1")
            || string.Equals(this.ddlDataType.SelectedValue, "P2")
            || string.Equals(this.ddlDataType.SelectedValue, "P3")
            || string.Equals(this.ddlDataType.SelectedValue, "P4")
            || string.Equals(this.ddlDataType.SelectedValue, "P5"))
        {
            nullCellFlag = 1;
            totleMagerNum = 5;
        }
        else if (string.Equals(this.ddlDataType.SelectedValue, "T1")
            || string.Equals(this.ddlDataType.SelectedValue, "T2"))
        {
            nullCellFlag = 2;
            totleMagerNum = 4;
        }
        else if (string.Equals(this.ddlDataType.SelectedValue, "T3"))
        {
            totleMagerNum = 4;
        }
        setExpData(sheet, this.gv_bookingbydatebyproduct, ref rowIndex, 0, nullCellFlag);
        setExpData(sheet, this.gv_bookingTotalbydatebyproduct, ref rowIndex, totleMagerNum, nullCellFlag);
        // Set All Text Warp False And Auto Fit
        sheet.Cells.WrapText = false;
        sheet.Cells.EntireColumn.AutoFit();
        // Export
        string path = Server.MapPath("~") + @"\ExcelReport\" + DateTime.Now.ToString("yyyyMMdd") + @"\";
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        string filePath = path + DateTime.Now.Ticks + ".xlsx";
        book.Saved = true;
        book.SaveCopyAs(filePath);
        excel.Workbooks.Close();
        excel.Quit();
        // DownLoad
        FileInfo file = new FileInfo(filePath);
        Response.Charset = "UTF-8";
        Response.ContentEncoding = Encoding.UTF8;
        Response.AddHeader("Content-Disposition", "attachment; filename=" + Server.UrlEncode("RSM(" + ddlist_segment.SelectedItem.Text + "-" + ddlist_RSM.SelectedItem.Text + ").xlsx"));
        Response.AddHeader("Content-Length", file.Length.ToString());
        Response.ContentType = "application/ms-excel";
        Response.WriteFile(filePath);
        Response.End();

        //by mbq 20110511 item13 add start   
        lockUser();
        LockSegment();
        //by mbq 20110511 item13 add end  
    }
    #endregion

    //private bool setUserStatus(string str_userID, string str_segmentID, string str_status, bool flag)
    //{
    //    string sql;
    //    if (flag)
    //        sql = "INSERT INTO [User_Status](UserID, SegmentID, Status) VALUES('" + str_userID + "','" + str_segmentID + "','" + str_status + "')";
    //    else
    //        sql = "UPDATE [User_Status] SET Status = '" + str_status + "' WHERE UserID = '" + str_userID + "' AND SegmentID = '" + str_segmentID + "'";
    //    int count = helper.ExecuteNonQuery(CommandType.Text, sql.Trim(), null);
    //    if (count == 1)
    //        return true;
    //    else
    //        return false;
    //}

    //private string getUserStatus(string str_userID, string str_segmentID)
    //{
    //    string sql = "SELECT status FROM [User_Status] WHERE UserID = '" + str_userID + "' AND SegmentID = '" + str_segmentID + "'";
    //    DataSet ds = helper.GetDataSet(sql);

    //    if (ds.Tables[0].Rows.Count == 0)
    //        return "";
    //    else
    //        return ds.Tables[0].Rows[0][0].ToString().Trim();
    //}

    private void setImageEnabled(string str_userID, string str_segmentID,string salesOrgId)
    {
        //string str_status = getUserStatus(str_userID, str_segmentID).Trim();
        string str_status = TrafficLightRule.GetUserStatus(str_userID, salesOrgId).Trim();
        if (string.IsNullOrEmpty(str_status))
            img_status.Visible = false;
        else
        {
            img_status.Visible = true;
            if (str_status == "R")
                img_status.ImageUrl = "~/images/red.png";
            else if (str_status == "Y")
                img_status.ImageUrl = "~/images/orange.png";
            else if (str_status == "G")
                img_status.ImageUrl = "~/images/green.png";
        }
        if (CheckLock(str_userID, str_segmentID))
        {
            ibtn_green.Enabled = false;
            ibtn_red.Enabled = false;
            ibtn_orange.Enabled = false;
            lbtn_editRSM.Visible = false;
        }
        else
        {
            if (str_status == "R")
            {
                ibtn_orange.Enabled = true;
                lbtn_editRSM.Visible = true;
                ibtn_green.Enabled = false;
                ibtn_red.Enabled = false;
            }
            else
            {
                ibtn_green.Enabled = false;
                ibtn_red.Enabled = false;
                ibtn_orange.Enabled = false;
                lbtn_editRSM.Visible = false;
            }
        }

        //if (str_status == "G")
        //{
        //    ibtn_green.Enabled = false;
        //    ibtn_red.Enabled = false;
        //    ibtn_orange.Enabled = false;
        //    img_status.ImageUrl = "~/images/green.png";
        //    lbtn_editRSM.Visible = false;
        //}
        //else if (str_status == "Y")
        //{
        //    ibtn_green.Enabled = false;
        //    ibtn_red.Enabled = false;
        //    ibtn_orange.Enabled = false;
        //    img_status.ImageUrl = "~/images/orange.png";
        //    lbtn_editRSM.Visible = false;
        //}
        //else if (str_status == "R")
        //{
        //    ibtn_green.Enabled = false;
        //    ibtn_red.Enabled = false;
        //    ibtn_orange.Enabled = true;
        //    img_status.ImageUrl = "~/images/red.png";
        //    lbtn_editRSM.Visible = true;
        //}
    }

    //private void bindStatus(string str_userID, string str_segmentID)
    //{
    //    if (!TrafficLightRule.SetGreen(Convert.ToInt32(str_userID), Convert.ToInt32(str_segmentID)))
    //    {
    //        string str_status = getUserStatus(str_userID, str_segmentID).Trim();
    //        bool success;

    //        if (str_status == "")
    //        {
    //            success = setUserStatus(str_userID, str_segmentID, "R", true);//yy
    //            if (success)
    //            { }
    //            else
    //            { }
    //        }
    //    }
    //}

    protected void ibtn_red_Click(object sender, ImageClickEventArgs e)
    {
        /*by ryzhang 20110511 item20 del start*/
        //bool success;
        //success = TrafficLightRule.UpdateUserStatus(ddlist_RSM.SelectedItem.Value.Trim(), ddlist_segment.SelectedItem.Value.Trim(), "R");
        ////success = setUserStatus(ddlist_RSM.SelectedItem.Value.Trim(), ddlist_segment.SelectedItem.Value.Trim(), "R", false);
        ////if (success)
        ////{ }
        ////else
        ////{ }
        //img_status.ImageUrl = "~/images/red.png";
        //setImageEnabled(ddlist_RSM.SelectedItem.Value.Trim(), ddlist_segment.SelectedItem.Value.Trim());
        //ibtn_green.Enabled = false;
        //ibtn_red.Enabled = false;
        //lbtn_editRSM.Visible = true;
        //ibtn_orange.Enabled = true;
        /*by ryzhang 20110511 item20 del end*/
        /*by ryzhang 20110511 item20 add start*/
        //ibtn_green.Enabled = false;
        //ibtn_red.Enabled = false;
        ///*by ryzhang 20110511 item20 add end*/

        ////by mbq 20110511 item13 add start   
        //lockUser();
        //LockSegment();
        ////by mbq 20110511 item13 add end  
    }

    protected void ibtn_orange_Click(object sender, ImageClickEventArgs e)
    {
        //bool success;
        //success = setUserStatus(ddlist_RSM.SelectedItem.Value.Trim(), ddlist_segment.SelectedItem.Value.Trim(), "Y", false);
        //if (success)
        //{ }
        //else
        //{ }
        ///*by ryzhang 20110511 item20 del start*/
        ////img_status.ImageUrl = "~/images/orange.png";
        ////ibtn_green.Enabled = true;
        ////ibtn_red.Enabled = true;
        ////lbtn_editRSM.Visible = false;
        ////ibtn_orange.Enabled = false;
        ///*by ryzhang 20110511 item20 del end*/
        ///*by ryzhang 20110511 item20 add start*/
        //img_status.ImageUrl = "~/images/orange.png";
        //ibtn_green.Enabled = false;
        //ibtn_red.Enabled = false;
        //lbtn_editRSM.Visible = false;
        //ibtn_orange.Enabled = false;
        ///*by ryzhang 20110511 item20 add end*/

        ////by mbq 20110511 item13 add start   
        //lockUser();
        //LockSegment();
        ////by mbq 20110511 item13 add end  

        bool success;
        success = TrafficLightRule.UpdateUserStatus(ddlist_RSM.SelectedItem.Value.Trim(), ddlist_salesOrg.SelectedItem.Value.Trim(), "Y");
        img_status.ImageUrl = "~/images/orange.png";
        setImageEnabled(ddlist_RSM.SelectedItem.Value.Trim(), ddlist_segment.SelectedItem.Value.Trim(), ddlist_salesOrg.SelectedValue.Trim());
    }

    protected void ibtn_green_Click(object sender, ImageClickEventArgs e)
    {
        /*by ryzhang 20110511 item20 del start*/
        //bool success;
        //success = setUserStatus(ddlist_RSM.SelectedItem.Value.Trim(), ddlist_segment.SelectedItem.Value.Trim(), "G", false);
        //if (success)
        //{ }
        //else
        //{ }

        //img_status.ImageUrl = "~/images/green.png";
        //ibtn_green.Enabled = false;
        //ibtn_red.Enabled = false;
        //lbtn_editRSM.Visible = false;
        //ibtn_orange.Enabled = true;
        /*by ryzhang 20110511 item20 del end*/
        //ibtn_green.Enabled = false;
        ///*by ryzhang 20110511 item20 add end*/

        ////by mbq 20110511 item13 add start   
        //lockUser();
        //LockSegment();
        //by mbq 20110511 item13 add end  
    }

    //by mbq 20110511 item13 add start   
    protected void lockUser()
    {
        //if (LockInterface.getLockUserData(getRSMID()))
        //{
        //    lbtn_editRSM.Visible = false;
        //    ibtn_green.Enabled = false;
        //    ibtn_red.Enabled = false;
        //    ibtn_orange.Enabled = false;
        //    //by ryzhang item13 20110519 add start
        //    img_status.ImageUrl = "~/images/red.png";
        //    //by ryzhang item13 20110519 add end
        //}
    }
    //by mbq 20110511 item13 add end   
    //by mbq 20110511 item13 add start   
    protected void LockSegment()
    {
        //if (LockInterface.getLockSegmentData(ddlist_segment.SelectedItem.Value.Trim()))
        //{
        //    lbtn_editRSM.Visible = false;
        //    ibtn_green.Enabled = false;
        //    ibtn_red.Enabled = false;
        //    ibtn_orange.Enabled = false;
        //    //by ryzhang item13 20110519 add start
        //    img_status.ImageUrl = "~/images/red.png";
        //    //by ryzhang item13 20110519 add end
        //}
    }
    //by mbq 20110511 item13 add end   

    #region DingJunji Add
    // By DingJunjie 20110531 ItemW18 Add Start
    /// <summary>
    /// Data Type DropDownList SelectedIndexChanged Method
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void ddlDataType_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (string.Equals(this.ddlDataType.SelectedValue, "P1"))
        {
            this.hidBookingY.Value = year.Substring(2, 2);
            this.hidDeliverY.Value = "YTD";
            this.hidGVType.Value = "1";
        }
        else if (string.Equals(this.ddlDataType.SelectedValue, "P2"))
        {
            this.hidBookingY.Value = year.Substring(2, 2);
            this.hidDeliverY.Value = year.Substring(2, 2);
            this.hidGVType.Value = "1";
        }
        else if (string.Equals(this.ddlDataType.SelectedValue, "P3"))
        {
            this.hidBookingY.Value = year.Substring(2, 2);
            this.hidDeliverY.Value = nextyear.Substring(2, 2);
            this.hidGVType.Value = "1";
        }
        else if (string.Equals(this.ddlDataType.SelectedValue, "P4"))
        {
            this.hidBookingY.Value = nextyear.Substring(2, 2);
            this.hidDeliverY.Value = nextyear.Substring(2, 2);
            this.hidGVType.Value = "1";
        }
        else if (string.Equals(this.ddlDataType.SelectedValue, "P5"))
        {
            this.hidBookingY.Value = nextyear.Substring(2, 2);
            this.hidDeliverY.Value = yearAfterNext.Substring(2, 2);
            this.hidGVType.Value = "1";
        }
        else if (string.Equals(this.ddlDataType.SelectedValue, "T1"))
        {
            this.hidGVType.Value = "2";
        }
        else if (string.Equals(this.ddlDataType.SelectedValue, "T2"))
        {
            this.hidGVType.Value = "2";
        }
        else if (string.Equals(this.ddlDataType.SelectedValue, "T3"))
        {
            this.hidGVType.Value = "2";
        }
        else
        {
            this.hidGVType.Value = "3";
        }
        bindDataSource();
    }

    /// <summary>
    /// Init Data Type DropDownList
    /// </summary>
    private void initDataTypeList()
    {
        this.ddlDataType.Items.Clear();
        this.ddlDataType.Items.Add(new ListItem(year.Substring(2, 2) + "YTD " + fiscalStart + "," + preyear + " to " + meeting.getMonth(month) + meeting.getDay() + "," + year, "P1"));
        this.ddlDataType.Items.Add(new ListItem(year.Substring(2, 2) + " for " + year.Substring(2, 2) + "  " + meeting.getMonth(month) + meeting.getDay() + "," + year + " to " + fiscalEnd + "," + year.Substring(2, 2) + " for " + year.Substring(2, 2) + " delivery", "P2"));
        this.ddlDataType.Items.Add(new ListItem(year.Substring(2, 2) + " for " + nextyear.Substring(2, 2) + "  " + meeting.getMonth(month) + meeting.getDay() + "," + year + " to " + fiscalEnd + "," + year.Substring(2, 2) + " for " + nextyear.Substring(2, 2) + " delivery", "P3"));
        this.ddlDataType.Items.Add(new ListItem(nextyear.Substring(2, 2) + " for " + nextyear.Substring(2, 2) + "  " + fiscalStart + "," + year + " to " + fiscalEnd + "," + nextyear.Substring(2, 2) + " for " + nextyear.Substring(2, 2) + " delivery", "P4"));
        this.ddlDataType.Items.Add(new ListItem(nextyear.Substring(2, 2) + " for " + yearAfterNext.Substring(2, 2) + "  " + fiscalStart + "," + year + " to " + fiscalEnd + "," + nextyear.Substring(2, 2) + " for " + yearAfterNext.Substring(2, 2) + " delivery", "P5"));
        if (isDateExist(true))
        {
            DataSet dsPro = getProductBySegment(ddlist_segment.SelectedItem.Value.ToString().Trim());
            if (dsPro != null && dsPro.Tables.Count > 0)
            {
                for (int i = 0; i < dsPro.Tables[0].Rows.Count; i++)
                {
                    this.ddlDataType.Items.Add(new ListItem(dsPro.Tables[0].Rows[i]["Abbr"].ToString(), dsPro.Tables[0].Rows[i]["ID"].ToString()));
                }
            }
        }
        if (isDateExist(false))
        {
            this.ddlDataType.Items.Add(new ListItem("Total(" + year.Substring(2, 2) + ")", "T1"));
            this.ddlDataType.Items.Add(new ListItem("Total(" + nextyear.Substring(2, 2) + ")", "T2"));
            this.ddlDataType.Items.Add(new ListItem("Total" + year.Substring(2, 2) + "(" + preyear.Substring(2, 2) + ")", "T3"));
        }
        this.ddlDataType.SelectedIndex = 0;
        this.hidGVType.Value = "1";
    }

    /// <summary>
    /// Bind Data
    /// </summary>
    private void bindDataSource()
    {
        this.gv_bookingbydatebyproduct.Columns.Clear();
        this.gv_bookingTotalbydatebyproduct.Columns.Clear();
        if (string.Equals(this.hidGVType.Value, "1"))
        {
            bindDataByDateByProduct(this.gv_bookingbydatebyproduct, this.hidBookingY.Value, this.hidDeliverY.Value, 0);
            bindDataTotalByDateByProduct(this.gv_bookingTotalbydatebyproduct, this.hidBookingY.Value, this.hidDeliverY.Value);
            if (int.Parse(month) == int.Parse(Resource.MEETING_MONTH_SECOND) && string.Equals(this.hidBookingY.Value, nextyear.Substring(2)))
            {
                this.lbtn_editRSM.Enabled = false;
            }
            else
            {
                this.lbtn_editRSM.Enabled = true;
            }
        }
        else if (string.Equals(this.hidGVType.Value, "2"))
        {
            if (string.Equals(this.ddlDataType.SelectedValue, "T1"))
            {
                bindDataByDate(this.gv_bookingbydatebyproduct);
                bindDataTotalByDate(this.gv_bookingTotalbydatebyproduct);
            }
            else if (string.Equals(this.ddlDataType.SelectedValue, "T2"))
            {
                bindDataNextByDate(this.gv_bookingbydatebyproduct);
                bindDataNextTotalByDate(this.gv_bookingTotalbydatebyproduct);
            }
            else if (string.Equals(this.ddlDataType.SelectedValue, "T3"))
            {
                bindDataVSPre(this.gv_bookingbydatebyproduct);
                bindDataTotalVSPre(this.gv_bookingTotalbydatebyproduct);
            }
            this.lbtn_editRSM.Enabled = false;
        }
        else if (string.Equals(this.hidGVType.Value, "3"))
        {
            bindDataByProduct();
            this.lbtn_editRSM.Enabled = false;
        }
    }

    /// <summary>
    /// Bind Product
    /// </summary>
    private void bindDataByProduct()
    {
        string productID = "";
        string productName = "";
        if (this.ddlDataType.SelectedItem != null)
        {
            productID = this.ddlDataType.SelectedValue;
            productName = this.ddlDataType.SelectedItem.Text;
        }

        DataSet ds = getBookingsDataByProduct(productID, ddlist_RSM.SelectedItem.Value);
        if (ds != null && ds.Tables.Count > 0)
        {
            this.gv_bookingbydatebyproduct = showBookingsByProduct(ds, this.gv_bookingbydatebyproduct, productName);
        }
        else
        {
            log.WriteLog(LogUtility.LogErrorLevel.LOG_CRITICAL, string.Format("Error:ds.Tables.Count is {0}", 0));
            Response.Redirect("~/Admin/AdminError.aspx");
        }

        ds = getBookingDataTotalByProduct(productID, ddlist_RSM.SelectedItem.Value);
        if (ds != null && ds.Tables.Count > 0)
        {
            this.gv_bookingTotalbydatebyproduct = showTotalByProduct(ds, this.gv_bookingTotalbydatebyproduct);
        }
        else
        {
            log.WriteLog(LogUtility.LogErrorLevel.LOG_CRITICAL, string.Format("Error:ds.Tables.Count is {0}", 0));
            Response.Redirect("~/Admin/AdminError.aspx");
        }
    }
    // By DingJunjie 20110531 ItemW18 Add End

    // By DingJunjie 20110601 ItemW18 Add Start
    /// <summary>
    /// Is Data Exist
    /// </summary>
    /// <param name="bookingYFlag">BookingY Flag</param>
    /// <returns>Result</returns>
    private bool isDateExist(bool bookingYFlag)
    {
        StringBuilder sql = new StringBuilder();
        sql.AppendLine(" SELECT ");
        sql.AppendLine(" 	COUNT(*) Count ");
        sql.AppendLine(" FROM ");
        sql.AppendLine("   Bookings ");
        sql.AppendLine("   INNER JOIN SubRegion ON Bookings.CountryID=SubRegion.ID ");
        sql.AppendLine("   INNER JOIN Country_SubRegion ON Bookings.CountryID=Country_SubRegion.SubRegionID ");
        sql.AppendLine("   INNER JOIN Country ON Country.ID=Country_SubRegion.CountryID ");
        sql.AppendLine("   INNER JOIN Operation ON Bookings.OperationID=Operation.ID ");
        sql.AppendLine("   LEFT JOIN Customer ON Customer.ID=Bookings.CustomerID ");
        sql.AppendLine("   LEFT JOIN CustomerName ON Customer.NameID=CustomerName.ID ");
        sql.AppendLine("   LEFT JOIN Project ON Project.ID=Bookings.ProjectID ");
        sql.AppendLine("   LEFT JOIN SalesChannel ON SalesChannel.ID=Bookings.SalesChannelID ");
        sql.AppendLine(" WHERE ");
        sql.AppendLine("   Bookings.RSMID=" + this.ddlist_RSM.SelectedValue);
        sql.AppendLine("   AND Bookings.SegmentID=" + this.ddlist_segment.SelectedValue);
        sql.AppendLine("   AND Bookings.SalesOrgID=" + this.ddlist_salesOrg.SelectedValue);
        if (!string.IsNullOrEmpty(this.ddlist_country.SelectedValue) && !string.Equals(this.ddlist_country.SelectedValue, "-1"))
        {
            sql.AppendLine("   AND Bookings.CountryID=" + this.ddlist_country.SelectedValue);
        }
        else
        {
            sql.AppendLine("   AND Bookings.CountryID IN (" + getCountrySQL() + ")");
        }
        sql.AppendLine("   AND SubRegion.Deleted=0 ");
        sql.AppendLine("   AND Country_SubRegion.Deleted=0 ");
        sql.AppendLine("   AND Operation.Deleted=0 ");
        sql.AppendLine("   AND Operation.Deleted=0 ");
        if (bookingYFlag)
        {
            sql.AppendLine("   AND Bookings.BookingY=" + year.Substring(2));
        }
        sql.AppendLine("   AND YEAR(Bookings.TimeFlag)=" + year);
        sql.AppendLine("   AND MONTH(Bookings.TimeFlag)=" + month);
        object count = helper.ExecuteScalar(CommandType.Text, sql.ToString(), null);
        if (Convert.ToInt32(count) == 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    // By DingJunjie 20110601 ItemW18 Add End

    // By DingJunjie 20110706 ItemW60 Add Start
    private void backInitPage(object sender, EventArgs e)
    {
        this.panel_search.Visible = true;
        this.panel_head.Visible = true;

        string str_segment = ddlist_segment.SelectedItem.Text;
        string str_salesorgID = ddlist_salesOrg.SelectedItem.Value;
        this.label_salesorgAbbr.Text = ddlist_salesOrg.SelectedItem.Text;
        this.label_headdescription.Text = str_segment + " -BOOKINGS " + meeting.getyear();

        this.ddlist_segment.SelectedValue = Request.QueryString["SegmentID"];
        ddlist_RSM.Items.Clear();
        bind(getRSmInfo(str_salesorgID, ddlist_segment.SelectedItem.Value.Trim()), 3);

        this.ddlist_country.SelectedValue = Request.QueryString["CountryID"];
        this.ddlist_RSM.SelectedValue = Request.QueryString["RSMID"];

        initDataTypeList();

        this.ddlDataType.SelectedValue = Request.QueryString["DataType"];

        string currency = null;
        DataSet ds_currency = getCurrencyBySalesOrgID(str_salesorgID);
        if (ds_currency != null)
        {
            currency = "K" + ds_currency.Tables[0].Rows[0][0].ToString().Trim();
        }
        else
        {
            currency = "No Currency";
        }
        this.btn_local.Text = currency;

        if (convert_flag)
        {
            this.btn_local.Enabled = true;
            this.btn_EUR.Enabled = false;
            this.label_currency.Text = this.btn_EUR.Text;
        }
        else
        {
            this.btn_local.Enabled = false;
            this.btn_EUR.Enabled = true;
            this.label_currency.Text = this.btn_local.Text;
        }

        //bindStatus(this.ddlist_RSM.SelectedItem.Value.Trim(), this.ddlist_segment.SelectedItem.Value.Trim());
        GetUserStatus(this.ddlist_RSM.SelectedItem.Value.Trim(), this.ddlist_salesOrg.SelectedItem.Value.Trim());
        setImageEnabled(this.ddlist_RSM.SelectedItem.Value.Trim(), this.ddlist_segment.SelectedItem.Value.Trim(), ddlist_salesOrg.SelectedValue.Trim());
        this.hidConvertFlag.Value = Convert.ToString(convert_flag).ToLower();

        ddlDataType_SelectedIndexChanged(sender, e);
        bindDataSource();
        lockUser();
        LockSegment();
    }
    // By DingJunjie 20110706 ItemW60 Add End

    /// <summary>
    /// Get Controls From One Control
    /// </summary>
    /// <param name="control">Control</param>
    /// <param name="type">Type</param>
    /// <param name="list">GridView List</param>
    private void ComputeSubControls(Control control, Type type, ArrayList list)
    {
        for (int i = 0; i < control.Controls.Count; i++)
        {
            if (control.Controls[i].GetType() == type)
            {
                list.Add(control.Controls[i]);
            }
            else
            {
                ComputeSubControls(control.Controls[i], type, list);
            }
        }
    }

    /// <summary>
    /// Set Export Data
    /// </summary>
    /// <param name="cells">Cells</param>
    /// <param name="gv">GridView</param>
    /// <param name="xfTitle">Title Style</param>
    /// <param name="xfHeader">Header Style</param>
    /// <param name="xfCell">Cell Style</param>
    /// <param name="rowIndex">Row Index</param>
    /// <param name="mergeNum">Merge Number</param>
    /// <param name="nullCellFlag">Empty Cell Flag</param>
    private void setExpData(Worksheet sheet, GridView gv, ref int rowIndex, int mergeNum, int nullCellFlag)
    {
        int colIndex = 1;
        int indexCStart = colIndex;
        int indexRStart = rowIndex;
        Range range = null;

        #region Set Title
        if (!string.IsNullOrEmpty(gv.Caption))
        {
            sheet.Cells[rowIndex, colIndex] = gv.Caption;
            range = sheet.get_Range(sheet.Cells[rowIndex, colIndex], sheet.Cells[rowIndex, colIndex]);
            range.HorizontalAlignment = XlHAlign.xlHAlignCenter;
            range.VerticalAlignment = XlVAlign.xlVAlignCenter;
            range.Font.Bold = true;
            rowIndex++;
        }
        #endregion

        #region Set Header
        int allColIndex = 0;
        for (int i = 0; i < gv.Columns.Count; i++)
        {
            if (gv.Columns[i].Visible)
            {
                sheet.Cells[rowIndex, colIndex] = gv.Columns[i].HeaderText;
                if (mergeNum != 0 && colIndex > 5)
                {
                    if (nullCellFlag == 1)
                    {
                        colIndex++;
                        sheet.Cells[rowIndex, colIndex] = null;
                    }
                    else if (nullCellFlag == 2 && allColIndex < gv.Columns.Count - 1)
                    {
                        colIndex++;
                        sheet.Cells[rowIndex, colIndex] = null;
                    }
                }
                if (mergeNum == 0)
                {
                    colIndex++;
                }
                else
                {
                    if (colIndex == indexCStart)
                    {
                        range = sheet.get_Range(sheet.Cells[rowIndex, indexCStart], sheet.Cells[rowIndex, indexCStart + mergeNum]);
                        range.Merge(0);
                        colIndex += mergeNum + 1;
                    }
                    else
                    {
                        colIndex++;
                    }
                }
            }
            allColIndex++;
        }
        rowIndex++;

        if (gv.Columns.Count != 0)
        {
            if (mergeNum == 0)
            {
                range = sheet.get_Range(sheet.Cells[indexRStart + 1, indexCStart], sheet.Cells[indexRStart + 1, colIndex - 1]);
            }
            else
            {
                range = sheet.get_Range(sheet.Cells[indexRStart, indexCStart], sheet.Cells[indexRStart, colIndex - 1]);
            }
            range.Interior.ColorIndex = 41;
            range.Font.Bold = true;
            range.Font.ColorIndex = 2;
        }
        #endregion

        #region Merge Title
        if (!string.IsNullOrEmpty(gv.Caption))
        {
            range = sheet.get_Range(sheet.Cells[indexRStart, indexCStart], sheet.Cells[indexRStart, colIndex - 1]);
            range.Merge(0);
        }
        #endregion

        #region Set Data
        string text = null;
        string aStart = "<a href='#'>";
        string aEnd = "</a>";
        string space = "&nbsp;";
        for (int i = 0; i < gv.Rows.Count; i++)
        {
            colIndex = indexCStart;
            allColIndex = 0;
            for (int j = 0; j < gv.Columns.Count; j++)
            {
                if (gv.Columns[j].Visible)
                {
                    text = gv.Rows[i].Cells[j].Text;
                    if (text.IndexOf(aStart) == 0)
                    {
                        text = text.Substring(aStart.Length);
                    }
                    if (text.LastIndexOf(aEnd) != -1 && text.LastIndexOf(aEnd) + aEnd.Length == text.Length)
                    {
                        text = text.Substring(0, text.LastIndexOf(aEnd));
                    }
                    if (text.IndexOf(space) == 0)
                    {
                        text = string.Empty;
                    }
                    if (ExcelHandler.IsFloat(text))
                    {
                        sheet.Cells[rowIndex, colIndex] = Convert.ToDecimal(text);
                        if (mergeNum != 0 && colIndex > 5)
                        {
                            if (nullCellFlag == 1)
                            {
                                colIndex++;
                                sheet.Cells[rowIndex, colIndex] = null;
                            }
                            else if (nullCellFlag == 2 && allColIndex < gv.Columns.Count - 1)
                            {
                                colIndex++;
                                sheet.Cells[rowIndex, colIndex] = null;
                            }
                        }
                    }
                    else
                    {
                        sheet.Cells[rowIndex, colIndex] = text;
                    }

                    if (mergeNum == 0)
                    {
                        colIndex++;
                    }
                    else
                    {
                        if (colIndex == indexCStart)
                        {
                            range = sheet.get_Range(sheet.Cells[rowIndex, indexCStart], sheet.Cells[rowIndex, indexCStart + mergeNum]);
                            range.Merge(0);
                            colIndex += mergeNum + 1;
                        }
                        else
                        {
                            colIndex++;
                        }
                    }
                }
                allColIndex++;
            }
            rowIndex++;
        }
        #endregion
    }
    #endregion

    private bool CheckLock(string rsm, string segmentId)
    {
        if (TrafficLightRule.IsLock(Convert.ToInt32(rsm), Convert.ToInt32(segmentId)))
        {
            Master.LockPage();
            return true;
        }
        else
        {
            Master.UnLockPage();
            return false;
        }

    }

    private string GetUserStatus(string str_userID, string str_segmentID)
    {
        string status = TrafficLightRule.GetUserStatus(str_userID, str_segmentID);
        if (string.IsNullOrEmpty(status))
        {

            return TrafficLightRule.InsertDefaultUserStatus(str_userID, str_segmentID);

        }
        else
            return status;
    }
}