/*
 * File Name    ; AdminSetMeetingDate.aspx.cs
 * 
 * Description  : Used for setting the meeting date, Staff only input data in accordance with the date
 * 
 * Author       : Wangjun
 * 
 * Modify Date  : 2010-12-13
 * 
 * Problem      : 
 * 
 * Version      : Release (2.0)
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using Resources;
using System.Text;

public partial class Admin_AdminTimeTable : System.Web.UI.Page
{
    SQLHelper helper = new SQLHelper();
    LogUtility log = new LogUtility();
    SQLStatement sql = new SQLStatement();

    GetMeetingDate date = new GetMeetingDate();
    CommonFunction cf = new CommonFunction();
    DisplayInfo info = new DisplayInfo();

    string strDates = "";
    protected void Page_Load(object sender, EventArgs e)
    {
        date.setDate();
        if (getRoleID(getRole()) != "0" && getRoleID(getRole()) != "5")
            Response.Redirect("~/AccessDenied.aspx");

        if (!IsPostBack)
        {
            btn_del.Visible = true;
            div_dis.Visible = true;
            if (getRoleID(getRole()) == "5")
            {
                btn_del.Visible = false;
                div_dis.Visible = false;
            }
            bindDropDownList();
            refreshSession();
            ddlist_meetingdate.Text = strDates;

        }
    }

    /* Get user'role */
    private string getRole()
    {
        return Session["Role"].ToString().Trim();
    }

    private void refreshSession()
    {
        Session["MeetingDate"] = date.setDisplayDate();
    }

    private string getRoleID(string str_name)
    {
        DataSet ds_role = sql.getRole();

        for (int i = 0; i < ds_role.Tables[0].Rows.Count; i++)
        {
            if (ds_role.Tables[0].Rows[i][0].ToString().Trim() == str_name)
            {
                return ds_role.Tables[0].Rows[i][1].ToString().Trim();
            }
        }
        return "";
    }

    public void bindDropDownList(DropDownList ddl, DataSet ds)
    {
        if (ds.Tables[0].Rows.Count > 0)
        {
            DataTable dt = ds.Tables[0];
            int count = dt.Rows.Count;
            int index = 0;
            while (index < count)
            {
                string str_displayYear = dt.Rows[index][2].ToString().Trim();
                string str_displayMonth = dt.Rows[index][1].ToString().Trim();
                if (str_displayMonth.Equals("10"))
                    str_displayYear = (int.Parse(str_displayYear) + 1).ToString().Trim();
                string str_display = date.getMeetingName(int.Parse(str_displayMonth)) + " " + str_displayYear;
                ddl.Items.Add(new ListItem(str_display, dt.Rows[index][0].ToString().Trim()));
                index++;
            }
            ddl.Enabled = true;
        }
        else
        {
            ddl.Items.Add(new ListItem("Not Exist", "-1"));
            ddl.Enabled = false;
        }

    }

    private void bindDropDownList()
    {
        date.setDate();

        DataSet ds = date.getMeetingDate();
        ddlist_meetingdate.Items.Clear();
        bindDropDownList(ddlist_meetingdate, ds);

        label_year.Text = DateTime.Now.Year.ToString().Trim();
        ddlist_month.Items.Clear();
        ddlist_month.Items.Add(new ListItem("3+9 Meeting", "01"));
        ddlist_month.Items.Add(new ListItem("5+7 Meeting", "03"));
        ddlist_month.Items.Add(new ListItem("9+3 Meeting", "07"));
        ddlist_month.Items.Add(new ListItem("0+12 Meeting", "10"));

        label_currentmeetingdate.Text = "";
        label_currentmeetingdate.ForeColor = System.Drawing.Color.Green;
        label_currentmeetingdate.Text = "The current meeting date is " + date.getyear() + "-" + date.getmonth() + "-" + date.getDay();

        string month = date.getmonth();
        string day = date.getDay();
        if (month.Length == 1)
        {
            month = "0" + month;
        }
        if (day.Length == 1)
        {
            day = "0" + day;
        }
        strDates = date.getyear() + "-" + month + "-" + day;

    }

    protected void btn_ok_Click(object sender, EventArgs e)
    {
        DataSet ds_date = date.getMeetingDate();
        int count = -1;
        string str_newdate = label_year.Text.Trim() + "-" + ddlist_month.SelectedItem.Value.Trim() + "-01";
        if (date.existMeetingDate(str_newdate))
        {
            label_info.ForeColor = System.Drawing.Color.Red;
            label_info.Text = info.addExist(str_newdate);
            return;
        }
        string insert_date = "INSERT INTO [MeetingDate](MeetingDate) VALUES('" + str_newdate + "')";
        count = helper.ExecuteNonQuery(CommandType.Text, insert_date, null);
        if (count > 0)
        {
            label_info.ForeColor = System.Drawing.Color.Green;
            label_info.Text = "Next meeting date has been add in meeting date list successfully, please set it in time.";
        }
        else
        {
            label_info.ForeColor = System.Drawing.Color.Red;
            label_info.Text = "Failed to add date";
        }
        bindDropDownList();
        refreshSession();
    }

    protected void btn_set_Click(object sender, EventArgs e)
    {
        string query_date = "SELECT MeetingDate FROM [SetMeetingDate]";
        DataSet ds_date = helper.GetDataSet(query_date);
        string year = date.getyear();
        string month = date.getmonth();
        if (Convert.ToInt32(month) < 10)
        {
            month = "0" + month;
        }
        int count = -1;
        string str_date = ddlist_meetingdate.Text.Trim();
        if (ds_date.Tables[0].Rows.Count > 0)
        {
            string update_date = "UPDATE [SetMeetingDate] SET MeetingDate = '" + str_date + "'";
            count = helper.ExecuteNonQuery(CommandType.Text, update_date, null);
        }
        else
        {
            string insert_date = "INSERT INTO [SetMeetingDate](MeetingDate) VALUES('" + str_date + "')";
            count = helper.ExecuteNonQuery(CommandType.Text, insert_date, null);
        }

        if (count > 0)
        {
            label_info.ForeColor = System.Drawing.Color.Green;
            label_info.Text = "Meeting date has been set to " + str_date + " successfully";
            transBookingSalesData();
            //transBackLog(year, month);
            transBackLog();   //Edit by Sino Bug8

            string sql = "delete from [ActualSalesandBL_Status];delete from [User_Status];";
            helper.ExecuteNonQuery(CommandType.Text, sql);
        }
        else
        {
            label_info.ForeColor = System.Drawing.Color.Red;
            label_info.Text = "Failed to set date";
        }
        bindDropDownList();
        ddlist_meetingdate.Text = str_date;
        refreshSession();
        Response.Redirect("~/Admin/AdminSetMeetingDate.aspx");
    }

    protected void btn_del_Click(object sender, EventArgs e)
    {
        string str_date = ddlist_meetingdate.Text.Trim();
        string del_date = "DELETE FROM [MeetingDate] WHERE MeetingDate = '" + str_date + "'";
        string date1 = ddlist_meetingdate.SelectedValue;
        string year = date1.Substring(0, 4);
        string month = date1.Substring(5, 2);

        if (date.existSetMeetingDate(str_date))
        {
            label_info.ForeColor = System.Drawing.Color.Red;
            label_info.Text = "The date has been set to current meeting date, <br />if you want to delete the date, please set to other date.";
            return;
        }

        int count = helper.ExecuteNonQuery(CommandType.Text, del_date, null);
        if (count > 0)
        {
            label_info.ForeColor = System.Drawing.Color.Green;
            label_info.Text = "Meeting date has been deleted successfully";
            DeleteBookingSalesData(year, month);
            DeleteBacklogData(year, month);
        }
        else
        {
            label_info.ForeColor = System.Drawing.Color.Red;
            label_info.Text = "Failed to delete date";
        }
        bindDropDownList();
    }

    /// <summary>
    /// Transfer Booking Sales Data
    /// </summary>
    private void transBookingSalesData()
    {
        string date = ddlist_meetingdate.SelectedValue;
        string year = date.Substring(0, 4);
        string month = date.Substring(5, 2);
        //DeleteBookingSalesData(year, month);
        if (isNoBookingSalesData(year, month))
        {
            string preYear = Convert.ToString(int.Parse(year) - 1);
            string afterYear = Convert.ToString(int.Parse(year) + 1);
            StringBuilder strSQL = new StringBuilder();
            strSQL.AppendLine(" INSERT INTO ");
            strSQL.AppendLine("   Bookings ");
            strSQL.AppendLine(" SELECT ");
            strSQL.AppendLine("   RSMID, ");
            strSQL.AppendLine("   SalesOrgID, ");
            strSQL.AppendLine("   CountryID, ");
            strSQL.AppendLine("   CustomerID, ");
            strSQL.AppendLine("   BookingY, ");
            strSQL.AppendLine("   DeliverY, ");
            strSQL.AppendLine("   SegmentID, ");
            strSQL.AppendLine("   ProductID, ");
            strSQL.AppendLine("   OperationID, ");
            strSQL.AppendLine("   ProjectID, ");
            strSQL.AppendLine("   Amount, ");
            strSQL.AppendLine("   Comments, ");
            strSQL.AppendLine("   '" + date + "', ");
            strSQL.AppendLine("   SalesChannelID, ");
            strSQL.AppendLine("   [Delivery in FY], ");
            strSQL.AppendLine("   [NO in FY], ");
            strSQL.AppendLine("   Value, ");
            strSQL.AppendLine("   Percentage, ");
            strSQL.AppendLine(" RecordID ");
            strSQL.AppendLine(" FROM ");
            strSQL.AppendLine("   Bookings ");
            strSQL.AppendLine(" WHERE ");
            if (string.Equals(month, Resource.MEETING_MONTH_FIRST))
            {
                strSQL.AppendLine("   YEAR(TimeFlag)=" + year);
                strSQL.AppendLine("   AND MONTH(TimeFlag)=" + Resource.MEETING_MONTH_FOURTH);
                strSQL.AppendLine("   AND BookingY='" + year.Substring(2) + "' ");
                strSQL.AppendLine("   AND DeliverY='YTD'");
            }
            else if (string.Equals(month, Resource.MEETING_MONTH_SECOND))
            {
                strSQL.AppendLine("   YEAR(TimeFlag)=" + preYear);
                strSQL.AppendLine("   AND MONTH(TimeFlag)=" + Resource.MEETING_MONTH_FIRST);
                strSQL.AppendLine("   AND BookingY='" + year.Substring(2) + "' ");
            }
            else if (string.Equals(month, Resource.MEETING_MONTH_THIRD))
            {
                strSQL.AppendLine("   YEAR(TimeFlag)=" + year);
                strSQL.AppendLine("   AND MONTH(TimeFlag)=" + Resource.MEETING_MONTH_SECOND);
                strSQL.AppendLine("   AND BookingY='" + year.Substring(2) + "' ");
            }
            else if (string.Equals(month, Resource.MEETING_MONTH_FOURTH))
            {
                strSQL.AppendLine("   YEAR(TimeFlag)=" + year);
                strSQL.AppendLine("   AND MONTH(TimeFlag)=" + Resource.MEETING_MONTH_THIRD);
            }
            helper.ExecuteNonQuery(CommandType.Text, strSQL.ToString(), null);
        }
    }

    private void transBackLog()
    {

        //Edit by Sino bug8 begin  
        string date = ddlist_meetingdate.SelectedValue;
        string year = date.Substring(0, 4);
        string month = date.Substring(5, 2);

        string preYear = Convert.ToString(int.Parse(year) - 1);
        //string afterYear = Convert.ToString(int.Parse(year) + 1);
        string backlogY = year.Substring(2);

        //DeleteBacklogData(year, month);
        StringBuilder strSQL = new StringBuilder();
        strSQL.AppendLine(" INSERT INTO ");
        strSQL.AppendLine("   ActualSalesandBL ");
        strSQL.AppendLine(" SELECT ");
        strSQL.AppendLine("   MarketingMgrID, ");
        strSQL.AppendLine("   OperationID, ");
        strSQL.AppendLine("   SegmentID, ");
        strSQL.AppendLine("   SalesOrgID, ");
        strSQL.AppendLine("   ProductID, ");
        strSQL.AppendLine("   Backlog, ");
        strSQL.AppendLine("   BacklogY, ");
        strSQL.AppendLine("   @TimeFlag, ");
        strSQL.AppendLine("   Comments ");
        strSQL.AppendLine(" FROM ");
        strSQL.AppendLine("   ActualSalesandBL ");
        strSQL.AppendLine(" WHERE ");
        strSQL.AppendLine("   TimeFlag=@CreateDate");
        //strSQL.AppendLine("   AND BacklogY=@BacklogY");

        List<SqlParameter> parameters = new List<SqlParameter>();

        if (string.Equals(month, Resource.MEETING_MONTH_FIRST))
        {
            if (isNoBacklogData(year, month))
            {
                parameters.Add(new SqlParameter("@TimeFlag", date));
                parameters.Add(new SqlParameter("@CreateDate", year + "-" + Resource.MEETING_MONTH_FOURTH + "-01"));
                //parameters[2] = new SqlParameter("@BacklogY", backlogY);
                helper.ExecuteNonQuery(CommandType.Text, strSQL.ToString(), parameters.ToArray());
            }
            //if (isNoBacklogData(year, month))
            //{
            //    parameters[0] = new SqlParameter("@TimeFlag", date);
            //    parameters[1] = new SqlParameter("@CreateDate", year + "-" + Resource.MEETING_MONTH_FOURTH + "-01");
            //    parameters[2] = new SqlParameter("@BacklogY", afterYear.Substring(2));
            //    helper.ExecuteNonQuery(CommandType.Text, strSQL.ToString(), parameters);
            //}

        }
        else if (string.Equals(month, Resource.MEETING_MONTH_SECOND))
        {
            if (isNoBacklogData(year, month))
            {
                strSQL.AppendLine("   AND BacklogY=@BacklogY");
                parameters.Add(new SqlParameter("@TimeFlag", date));
                parameters.Add(new SqlParameter("@CreateDate", preYear + "-" + Resource.MEETING_MONTH_FIRST + "-01"));
                parameters.Add(new SqlParameter("@BacklogY", backlogY));
                helper.ExecuteNonQuery(CommandType.Text, strSQL.ToString(), parameters.ToArray());
            }
        }
        else if (string.Equals(month, Resource.MEETING_MONTH_THIRD))
        {
            if (isNoBacklogData(year, month))
            {
                parameters.Add(new SqlParameter("@TimeFlag", date));
                parameters.Add(new SqlParameter("@CreateDate", year + "-" + Resource.MEETING_MONTH_SECOND + "-01"));
                //parameters[2] = new SqlParameter("@BacklogY", backlogY);
                helper.ExecuteNonQuery(CommandType.Text, strSQL.ToString(), parameters.ToArray());
            }

        }
        else if (string.Equals(month, Resource.MEETING_MONTH_FOURTH))
        {
            if (isNoBacklogData(year, month))
            {
                parameters.Add(new SqlParameter("@TimeFlag", date));
                parameters.Add(new SqlParameter("@CreateDate", year + "-" + Resource.MEETING_MONTH_THIRD + "-01"));
                //parameters[2] = new SqlParameter("@BacklogY", backlogY);
                helper.ExecuteNonQuery(CommandType.Text, strSQL.ToString(), parameters.ToArray());

            }
            //if (isNoBacklogData(year, month))
            //{
            //    parameters[0] = new SqlParameter("@TimeFlag", date);
            //    parameters[1] = new SqlParameter("@CreateDate", year + "-" + Resource.MEETING_MONTH_THIRD + "-01");
            //    parameters[2] = new SqlParameter("@BacklogY", afterYear.Substring(2));
            //    helper.ExecuteNonQuery(CommandType.Text, strSQL.ToString(), parameters);
            //}

        }


        //Edit by Sino bug8 End
    }

    /// <summary>
    /// Check is has date or not by date
    /// </summary>
    /// <param name="year">Year</param>
    /// <param name="month">Month</param>
    /// <returns>No date return true;else return false.</returns>
    private bool isNoBookingSalesData(string year, string month)
    {
        string strSQL = "SELECT COUNT(*) COUNT FROM Bookings WHERE YEAR(TimeFlag)=" + year + " AND MONTH(TimeFlag)=" + month;
        object count = helper.ExecuteScalar(CommandType.Text, strSQL, null);
        return Convert.ToInt32(count) == 0 ? true : false;
    }

    private void DeleteBookingSalesData(string year, string month)
    {
        string strSQL = "delete FROM Bookings WHERE YEAR(TimeFlag)=" + year + " AND MONTH(TimeFlag)=" + month;
        helper.ExecuteScalar(CommandType.Text, strSQL, null);
    }

    private void DeleteBacklogData(string year, string month)
    {
        string strSQL = "delete FROM ActualSalesandBL  WHERE YEAR(TimeFlag)=" + year + " AND MONTH(TimeFlag)=" + month;
        helper.ExecuteScalar(CommandType.Text, strSQL, null);
    }

    //Edit by Sino Bug8 begin

    private bool isNoBacklogData(string year, string month, string backlogY)
    {

        string strSQL = "SELECT COUNT(*) COUNT FROM ActualSalesandBL  WHERE YEAR(TimeFlag)=" + year + " AND MONTH(TimeFlag)=" + month + " AND BacklogY=" + backlogY;
        object count = helper.ExecuteScalar(CommandType.Text, strSQL, null);
        return Convert.ToInt32(count) == 0 ? true : false;

    }

    private bool isNoBacklogData(string year, string month)
    {

        string strSQL = "SELECT COUNT(*) COUNT FROM ActualSalesandBL  WHERE YEAR(TimeFlag)=" + year + " AND MONTH(TimeFlag)=" + month;
        object count = helper.ExecuteScalar(CommandType.Text, strSQL, null);
        return Convert.ToInt32(count) == 0 ? true : false;

    }

    
}
