/*
 *  FileName      :  RSMForecast.aspx.cs
 * 
 *  Description   :  Query 'OP-BKG','OP-Sales','GR-BKG','GR-Sales' and 'Booking Data By SalesOrg'
 *                   by one's segment
 *  Author        :  Wang Jun
 * 
 *  Modified Date :  2010-10-28
 * 
 *  Problem       :  none
 * 
 * Version        :  Release(1.0)
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;

public partial class RSM_Default : System.Web.UI.Page
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
        if (getRoleID(getRole()) != "4")
            Response.Redirect("!/AccessDenied.aspx");

        if (!IsPostBack)
        {
            log.WriteLog(LogUtility.LogErrorLevel.LOG_ACCESS, "RSMBookingSalesForecast Access.");

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

    protected string getRSMID()
    {
        return Session["RSMID"].ToString().Trim();
    }

    //Get segment's information by rsmID
    protected DataSet getSegmentInfo()
    {
        string query_segment = "SELECT Abbr,ID FROM [Segment] INNER JOIN [User_Segment] ON [User_Segment].SegmentID=[Segment].ID"
                        + " WHERE UserID='" + getRSMID() + "' AND [Segment].Deleted='0' AND [User_Segment].Deleted = '0' "
                        + " ORDER BY ID ASC";
        DataSet ds = helper.GetDataSet(query_segment);

        if (ds.Tables[0].Rows.Count > 0)
            return ds;
        else
            return null;
    }

    protected void bindDdlist(DataSet ds)
    {
        if (ds != null)
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
        label_bookingsDecription.Text = " BOOKINGS FORECAST BY SALES ORGANIZATION FOR " + nextyear + "&" + yearAfterNext;
        label_salesDecription.Text = " SALES FORECAST BY OPERATION FOR " + nextyear;
        label_noteDate.Text = "Date of B/L : <br />" + meeting.getMonth(month) + " " + meeting.getDay() + "," + year;
        label_noteDate.Text += " <br /><br />bookings forecast from : <br />" + meeting.getMonth(month) + " " + meeting.getDay() + "," + year;

        panel_dec.Visible = true;
        gv_opAbbr.Columns.Clear();
        bindDataSource();
    }

    protected void lbtn_op_sales_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/RSM/RSMOperationSales.aspx?SegmentID=" + ddlist_segment.SelectedItem.Value);
    }
    protected void lbtn_op_bkg_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/RSM/RSMOperationalBookings.aspx?SegmentID=" + ddlist_segment.SelectedItem.Value);
    }
    protected void lbtn_gr_sales_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/RSM/RSMGrossSales.aspx?SegmentID=" + ddlist_segment.SelectedItem.Value);
    }
    protected void lbtn_gr_bkg_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/RSM/RSMGrossBookings.aspx?SegmentID=" + ddlist_segment.SelectedItem.Value);
    }
    protected void lbtn_salesorg_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/RSM/RSMBookingBySalesOrg.aspx?SegmentID=" + ddlist_segment.SelectedItem.Value);
    }
}