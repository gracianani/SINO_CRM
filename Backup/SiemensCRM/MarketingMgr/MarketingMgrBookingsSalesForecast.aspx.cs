/*
 *  FileName      :  MarketingMgrForecast.aspx.cs
 * 
 *  Description   :  Query 'OP-BKG','OP-Sales','GR-BKG','GR-Sales' and 'Booking Data By SalesOrg'
 *                   by one's segment
 *  Author        :  Wang Jun
 * 
 *  Modified Date :  2010-04-02
 * 
 *  Problem       :  none
 * 
 *  Version       : Release (1.0)
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;

public partial class MarketingMgr_fault : System.Web.UI.Page
{
    SQLHelper helper = new SQLHelper();
    WebUtility web = new WebUtility();
    LogUtility log = new LogUtility();
    SQLStatement sql = new SQLStatement();
    GetMeetingDate meeting = new GetMeetingDate();

    /* Set Date */
    protected static string yearBeforePre;
    protected static string preyear;
    protected static string year;
    protected static string nextyear;
    protected static string yearAfterNext;
    protected static string month;
    protected const string fiscalStart = "Oct.1";
    protected const string fiscalEnd = "Sept.30";

    protected void Page_Load(object sender, EventArgs e)
    {
        if (getRoleID(getRole()) != "2")
            Response.Redirect("~/AccessDenied.aspx");

        if (!IsPostBack)
        {
            log.WriteLog(LogUtility.LogErrorLevel.LOG_ACCESS, "MarketingMgrBookingSalesForecast Access.");
            meeting.setDate();
            yearBeforePre = meeting.getyearBeforePre();
            preyear = meeting.getpreyear();
            year = meeting.getyear();
            nextyear = meeting.getnextyear();
            yearAfterNext = meeting.getyearAfterNext();
            month = meeting.getmonth();
            bindDdlist(getSegmentInfo());
        }
    }

    /* Get user'role */
    private string getRole()
    {
        return Session["Role"].ToString().Trim();
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

    protected string getMarketingMgrID()
    {
        return Session["GeneralMarketingMgrID"].ToString().Trim();
    }

    protected DataSet getSegmentInfo()
    {
        string query_segment = "SELECT Abbr,ID FROM [Segment] INNER JOIN [User_Segment] ON [User_Segment].SegmentID=[Segment].ID"
                        + " WHERE UserID = '" + getMarketingMgrID() + "' AND [Segment].Deleted='0' AND [User_Segment].Deleted = '0'"
                        + " GROUP BY Abbr,ID"
                        + " ORDER BY Abbr";
        DataSet ds = helper.GetDataSet(query_segment);

        if (ds.Tables[0].Rows.Count > 0)
            return ds;
        else
            return null;
    }

    protected void bindDdlist(DataSet ds)
    {
        if (ds.Tables[0].Rows.Count > 0)
        {
            DataTable dt = ds.Tables[0];
            int count = dt.Rows.Count;
            int index = 0;
            while (index < count)
            {
                ListItem li = new ListItem(dt.Rows[index][0].ToString().Trim(), dt.Rows[index][1].ToString().Trim());
                ddlist_segment.Items.Add(li);
                index++;
            }
            ddlist_segment.SelectedIndex = 0;
            ddlist_segment.Enabled = true;
            btn_search.Enabled = true;
        }
        else
        {

            ddlist_segment.Enabled = false;
            ddlist_segment.Items.Add("");
            btn_search.Enabled = false;
        }
    }

    protected void bindDataSource()
    {
        DataSet ds_Country = sql.getOperationBySegment(ddlist_segment.SelectedItem.Value);

        if (ds_Country.Tables[0].Rows.Count > 0)
        {
            gv_opAbbr.Width = Unit.Pixel(200);
            gv_opAbbr.AutoGenerateColumns = false;
            gv_opAbbr.AllowPaging = true;
            gv_opAbbr.Visible = true;

            for (int i = 2; i < ds_Country.Tables[0].Columns.Count; i++)
            {
                BoundField bf = new BoundField();

                bf.DataField = ds_Country.Tables[0].Columns[i].ColumnName.ToString();
                bf.HeaderText = ds_Country.Tables[0].Columns[i].Caption.ToString();
                bf.ItemStyle.Width = 100;
                bf.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
                bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
                bf.ReadOnly = true;

                gv_opAbbr.Columns.Add(bf);
            }

            gv_opAbbr.AllowSorting = true;
            gv_opAbbr.DataSource = ds_Country.Tables[0];
            gv_opAbbr.DataBind();
            panel_enter.Visible = true;
            panel_enter.Enabled = true;
        }
        else
        {
            panel_enter.Visible = true;
            gv_opAbbr.Visible = false;
            panel_enter.Enabled = false;
        }
    }

    protected void btn_search_Click(object sender, EventArgs e)
    {
        label_segDecription.Text = ddlist_segment.SelectedItem.Text;
        label_segDecription.Visible = true;
        //By Fxw 20110506 ITEM25 ADD Start
        string query_date = "SELECT CONVERT(varchar(15),SelectMeetingDate,23) FROM [SetSelectMeetingDate] where userid=" + Session["GeneralMarketingMgrID"].ToString();
        DataSet ds_date = helper.GetDataSet(query_date);
        if (ds_date.Tables[0].Rows.Count > 0 && !ds_date.Tables[0].Rows[0][0].ToString().Equals("") && ds_date.Tables[0].Rows[0][0].ToString() != null)
        {
            //by ryzhang 20110523 item49 del start
            //label_show.Text = "This report is related to the meeting date " + ds_date.Tables[0].Rows[0][0].ToString();
            //by ryzhang 20110523 item49 del end
            //by ryzhang 20110523 item49 add start
            label_show.Text = "This report is related to the meeting date (select) " + ds_date.Tables[0].Rows[0][0].ToString();
            this.lbtn_op_sales.Enabled = true;
            this.lbtn_op_bkg.Enabled = true;
            this.lbtn_gr_sales.Enabled = true;
            this.lbtn_gr_bkg.Enabled = true;
            this.lbtn_salesorg.Enabled = true;
            //by ryzhang 20110523 item49 add end
            string query_year = "SELECT YEAR(SelectMeetingDate) FROM [SetSelectMeetingDate] where userid=" + Session["GeneralMarketingMgrID"].ToString();
            DataSet ds_year = helper.GetDataSet(query_year);
            string query_month = "SELECT MONTH(SelectMeetingDate) FROM [SetSelectMeetingDate] where userid=" + Session["GeneralMarketingMgrID"].ToString();
            DataSet ds_month = helper.GetDataSet(query_month);
            string query_day = "SELECT Day(SelectMeetingDate) FROM [SetSelectMeetingDate] where userid=" + Session["GeneralMarketingMgrID"].ToString();
            DataSet ds_day = helper.GetDataSet(query_day);
            if (ds_date.Tables[0].Rows.Count > 0 && ds_date.Tables[0].Rows.Count > 0 && ds_date.Tables[0].Rows.Count > 0)
            {
                label_noteDate.Text = "Date of B/L : <br />" + meeting.getMonth(ds_month.Tables[0].Rows[0][0].ToString()) + " " + ds_day.Tables[0].Rows[0][0].ToString() + "," + ds_year.Tables[0].Rows[0][0].ToString();
                label_noteDate.Text += " <br /><br />bookings forecast from : <br />" + meeting.getMonth(ds_month.Tables[0].Rows[0][0].ToString()) + " " + ds_day.Tables[0].Rows[0][0].ToString() + "," + ds_year.Tables[0].Rows[0][0].ToString();
                label_bookingsDecription.Text = " BOOKINGS FORECAST BY SALES ORGANIZATION FOR " + (Convert.ToInt32(ds_year.Tables[0].Rows[0][0].ToString()) + 1) + "&" + (Convert.ToInt32(ds_year.Tables[0].Rows[0][0].ToString()) + 2);
                label_salesDecription.Text = " SALES FORECAST BY OPERATION FOR " + (Convert.ToInt32(ds_year.Tables[0].Rows[0][0].ToString()) + 1);
            }
        }
        else
        {
            //by ryzhang 20110523 item49 del start
            //label_show.Text = "There is no meeting date selected!";
            //by ryzhang 20110523 item49 del end
            //by ryzhang 20110523 item49 add start
            label_show.Text = "There is no meeting date selected! Please select one meeting date first!";
            label_show.Style.Add("color", "red");
            this.lbtn_op_sales.Enabled = false;
            this.lbtn_op_bkg.Enabled = false;
            this.lbtn_gr_sales.Enabled = false;
            this.lbtn_gr_bkg.Enabled = false;
            this.lbtn_salesorg.Enabled = false;
            //by ryzhang 20110523 item49 add end
            label_noteDate.Text = "Date of B/L : <br />";
            label_noteDate.Text += " <br /><br />bookings forecast from : <br />";
            label_bookingsDecription.Text = " BOOKINGS FORECAST BY SALES ORGANIZATION";
            label_salesDecription.Text = " SALES FORECAST BY OPERATION";
        }
        //By Fxw 20110506 ITEM25 ADD End
        //By Fxw 20110506 ITEM25 DEL Start
        //label_bookingsDecription.Text = " BOOKINGS FORECAST BY SALN FOR " + nextyear;
        //label_noteDate.Text = "Date of B/L : <br />" + meeting.getMonth(month) + " " + meeting.getDay() + "," + year;
        //label_noteDate.Text += " <br /><br />bookings forecast from : <br />" + meeting.getMonth(month) + " " + meeting.getDay() + "," + year;
        //By Fxw 20110506 ITEM25 DEL End
        panel_dec.Visible = true;
        gv_opAbbr.Columns.Clear();
        bindDataSource();
    }

    protected void lbtn_op_sales_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/MarketingMgr/MarketingMgrOperationSales.aspx?SegmentID=" + ddlist_segment.SelectedItem.Value);
    }
    protected void lbtn_op_bkg_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/MarketingMgr/MarketingMgrOperationalBookings.aspx?SegmentID=" + ddlist_segment.SelectedItem.Value);
    }
    protected void lbtn_gr_sales_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/MarketingMgr/MarketingMgrGrossSales.aspx?SegmentID=" + ddlist_segment.SelectedItem.Value);
    }
    protected void lbtn_gr_bkg_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/MarketingMgr/MarketingMgrGrossBookings.aspx?SegmentID=" + ddlist_segment.SelectedItem.Value);
    }
    protected void lbtn_salesorg_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/MarketingMgr/MarketingMgrBookingBySalesOrg.aspx?SegmentID=" + ddlist_segment.SelectedItem.Value);
    }
    protected void lbtn_region_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/MarketingMgr/MarketingMgrBookingByCountryByRegion.aspx?SegmentID=" + ddlist_segment.SelectedItem.Value);
    }
}