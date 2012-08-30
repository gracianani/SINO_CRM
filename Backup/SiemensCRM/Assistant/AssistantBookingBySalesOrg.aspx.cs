using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web.UI.HtmlControls;

public partial class Assistant_AssistantBooking : System.Web.UI.Page
{
    SQLHelper helper = new SQLHelper();
    LogUtility log = new LogUtility();
    SQLStatement sql = new SQLStatement();
    DisplayInfo info = new DisplayInfo();
    WebUtility web = new WebUtility();
    CommonFunction cf = new CommonFunction();
    //by ryzhang item49 20110519 del start 
    //GetMeetingDate meeting = new GetMeetingDate();
    //by ryzhang item49 20110519 del end 
    //by ryzhang item49 20110519 add start 
    GetSelectMeetingDate date = new GetSelectMeetingDate();
    //by ryzhang item49 20110519 add end 

    private static bool flag;

    protected void Page_Load(object sender, EventArgs e)
    {
        Page.MaintainScrollPositionOnPostBack = true;
        if (getRoleID(getRole()) == "0" || getRoleID(getRole()) == "5")
        {

        }
        else
            Response.Redirect("~/AccessDenied.aspx");

        if (!IsPostBack)
        {
            log.WriteLog(LogUtility.LogErrorLevel.LOG_ACCESS, "AssistantBookingBySalesOrg Access.");
            panel_search.Visible = false;
            panel_head.Visible = false;
            //by ryzhang item49 20110519 del start 
            //setDate();
            //by ryzhang item49 20110519 del end 
            bind(getSalesOrgInfo(getSegmentID()));
        }
        //by ryzhang item49 20110519 add start 
        setDate();
        //by ryzhang item49 20110519 add end 
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

    /* DropdownList */
    protected string getSegmentID()
    {
        return Request.QueryString["SegmentID"].ToString().Trim();
    }

    //Get segment description by segmentID
    protected string getSegmentDec(string segmentID)
    {
        DataSet ds_segment = sql.getSegmentInfo();
        string str_segmentDec = "";
        for (int i = 0; i < ds_segment.Tables[0].Rows.Count; i++)
        {
            string str_segmentID = ds_segment.Tables[0].Rows[i][0].ToString().Trim();
            if (str_segmentID == segmentID)
            {
                str_segmentDec = ds_segment.Tables[0].Rows[i][2].ToString().Trim();
                break;
            }
        }
        return str_segmentDec;
    }

    protected DataSet getSalesOrgInfo(string str_segmentID)
    {
        return sql.getSalesOrgInfoBySegment(str_segmentID);
    }

    protected string getAbbrBySalesOrg(string str_salesorgID)
    {
        string query_currency = "SELECT Abbr FROM [SalesOrg] WHERE Deleted = 0 AND ID = '" + str_salesorgID + "'";
        DataSet ds_currency = helper.GetDataSet(query_currency);

        if (ds_currency.Tables[0].Rows.Count > 0)
            return ds_currency.Tables[0].Rows[0][0].ToString().Trim();
        else
            return "";
    }

    protected void getCurrencyBySalesOrg(string str_salesorgID)
    {
        string str_currency = sql.getSalesOrgCurrency(str_salesorgID);

        if (str_currency.Trim() != "")
            label_currency.Text = "K" + str_currency;
        else
            label_currency.Text = "Error";
    }

    protected void bind(DataSet ds)
    {
        if (ds != null)
        {
            DataTable dt = ds.Tables[0];
            int count = dt.Rows.Count;
            int index = 0;
            while (index < count)
            {
                ListItem li = new ListItem(dt.Rows[index][2].ToString().Trim(), dt.Rows[index][1].ToString().Trim());
                ddlist_salesOrg.Items.Add(li);
                index++;
            }
            ddlist_salesOrg.SelectedIndex = 0;
            ddlist_salesOrg.Enabled = true;
        }
        else
        {
            ddlist_salesOrg.Enabled = false;
            ddlist_salesOrg.Items.Add("");
            btn_search.Enabled = false;
        }
    }

    protected void bindDataSource()
    {
        /* Data by date by product  start*/

        gv_bookingbydatebyproduct.Columns.Clear();
        gv_bookingTotalbydatebyproduct.Columns.Clear();

        bindDataByDateByProduct(gv_bookingbydatebyproduct, year.Substring(2, 2), "YTD",false);
        //By Wsy 20110517 ITEM 22 ADD Start 
        gv_bookingbydatebyproduct.Style.Clear();
        gv_bookingbydatebyproduct.Style.Add("border", "#000 solid 1px");
        gv_bookingbydatebyproduct.Style.Add("border-collapse", "collapse");
        gv_bookingbydatebyproduct.Style.Add("font-size", "12px");
        if (gv_bookingbydatebyproduct.HeaderRow != null)
        {
            gv_bookingbydatebyproduct.HeaderRow.Style.Add("background", "#000");
        }
        for (int i = 0; i < gv_bookingbydatebyproduct.Rows.Count; i++)
        {
            foreach (TableCell cell in gv_bookingbydatebyproduct.Rows[i].Cells)
            {
                cell.Style.Clear();
                cell.Style.Add("background", "#FF9");
            }
        }
        //By Wsy 20110517 ITEM 22 ADD End 
        bindDataTotalByDateByProduct(gv_bookingTotalbydatebyproduct, year.Substring(2, 2), "YTD",false);
        //By Wsy 20110517 ITEM 22 ADD Start 
        gv_bookingTotalbydatebyproduct.Style.Clear();
        gv_bookingTotalbydatebyproduct.Style.Add("border", "#000 solid 1px");
        gv_bookingTotalbydatebyproduct.Style.Add("border-collapse", "collapse");
        gv_bookingTotalbydatebyproduct.Style.Add("font-size", "12px");
        if (gv_bookingTotalbydatebyproduct.HeaderRow != null)
        {
            gv_bookingTotalbydatebyproduct.HeaderRow.Style.Add("background", "#000");
        }
        for (int i = 0; i < gv_bookingTotalbydatebyproduct.Rows.Count; i++)
        {
            foreach (TableCell cell in gv_bookingTotalbydatebyproduct.Rows[i].Cells)
            {
                cell.Style.Clear();
                cell.Style.Add("background", "#FF9");
            }
        }
        //By Wsy 20110517 ITEM 22 ADD End 
        gv_booking1bydatebyproduct.Columns.Clear();
        gv_booking1Totalbydatebyproduct.Columns.Clear();

        bindDataByDateByProduct(gv_booking1bydatebyproduct, year.Substring(2, 2), year.Substring(2, 2),true);
        //By Wsy 20110517 ITEM 22 ADD Start 
        gv_booking1bydatebyproduct.Style.Clear();
        gv_booking1bydatebyproduct.Style.Add("border", "#000 solid 1px");
        gv_booking1bydatebyproduct.Style.Add("border-collapse", "collapse");
        gv_booking1bydatebyproduct.Style.Add("font-size", "12px");
        if (gv_booking1bydatebyproduct.HeaderRow != null)
        {
            gv_booking1bydatebyproduct.HeaderRow.Style.Add("background", "#000");
        }
        for (int i = 0; i < gv_booking1bydatebyproduct.Rows.Count; i++)
        {
            foreach (TableCell cell in gv_booking1bydatebyproduct.Rows[i].Cells)
            {
                cell.Style.Clear();
                cell.Style.Add("background", "#ccffff");
            }
        }
        //By Wsy 20110517 ITEM 22 ADD End 
        bindDataTotalByDateByProduct(gv_booking1Totalbydatebyproduct, year.Substring(2, 2), year.Substring(2, 2),true);
        //By Wsy 20110517 ITEM 22 ADD Start 
        gv_booking1Totalbydatebyproduct.Style.Clear();
        gv_booking1Totalbydatebyproduct.Style.Add("border", "#000 solid 1px");
        gv_booking1Totalbydatebyproduct.Style.Add("border-collapse", "collapse");
        gv_booking1Totalbydatebyproduct.Style.Add("font-size", "12px");
        if (gv_booking1Totalbydatebyproduct.HeaderRow != null)
        {
            gv_booking1Totalbydatebyproduct.HeaderRow.Style.Add("background", "#000");
        }
        for (int i = 0; i < gv_booking1Totalbydatebyproduct.Rows.Count; i++)
        {
            foreach (TableCell cell in gv_booking1Totalbydatebyproduct.Rows[i].Cells)
            {
                cell.Style.Clear();
                cell.Style.Add("background", "#ccffff");
            }
        }
        //By Wsy 20110517 ITEM 22 ADD End 
        gv_booking2bydatebyproduct.Columns.Clear();
        gv_booking2Totalbydatebyproduct.Columns.Clear();

        bindDataByDateByProduct(gv_booking2bydatebyproduct, year.Substring(2, 2), nextyear.Substring(2, 2),true);
        //By Wsy 20110517 ITEM 22 ADD Start 
        gv_booking2bydatebyproduct.Style.Clear();
        gv_booking2bydatebyproduct.Style.Add("border", "#000 solid 1px");
        gv_booking2bydatebyproduct.Style.Add("border-collapse", "collapse");
        gv_booking2bydatebyproduct.Style.Add("font-size", "12px");
        if (gv_booking2bydatebyproduct.HeaderRow != null)
        {
            gv_booking2bydatebyproduct.HeaderRow.Style.Add("background", "#000");
        }
        for (int i = 0; i < gv_booking2bydatebyproduct.Rows.Count; i++)
        {
            foreach (TableCell cell in gv_booking2bydatebyproduct.Rows[i].Cells)
            {
                cell.Style.Clear();
                cell.Style.Add("background", "#FF9");
            }
        }
        //By Wsy 20110517 ITEM 22 ADD End 
        bindDataTotalByDateByProduct(gv_booking2Totalbydatebyproduct, year.Substring(2, 2), nextyear.Substring(2, 2),true);
        //By Wsy 20110517 ITEM 22 ADD Start 
        gv_booking2Totalbydatebyproduct.Style.Clear();
        gv_booking2Totalbydatebyproduct.Style.Add("border", "#000 solid 1px");
        gv_booking2Totalbydatebyproduct.Style.Add("border-collapse", "collapse");
        gv_booking2Totalbydatebyproduct.Style.Add("font-size", "12px");
        if (gv_booking2Totalbydatebyproduct.HeaderRow != null)
        {
            gv_booking2Totalbydatebyproduct.HeaderRow.Style.Add("background", "#000");
        }
        for (int i = 0; i < gv_booking2Totalbydatebyproduct.Rows.Count; i++)
        {
            foreach (TableCell cell in gv_booking2Totalbydatebyproduct.Rows[i].Cells)
            {
                cell.Style.Clear();
                cell.Style.Add("background", "#FF9");
            }
        }
        //By Wsy 20110517 ITEM 22 ADD End 
        gv_booking3bydatebyproduct.Columns.Clear();
        gv_booking3Totalbydatebyproduct.Columns.Clear();

        bindDataByDateByProduct(gv_booking3bydatebyproduct, nextyear.Substring(2, 2), nextyear.Substring(2, 2),false);
        //By Wsy 20110517 ITEM 22 ADD Start 
        gv_booking3bydatebyproduct.Style.Clear();
        gv_booking3bydatebyproduct.Style.Add("border", "#000 solid 1px");
        gv_booking3bydatebyproduct.Style.Add("border-collapse", "collapse");
        gv_booking3bydatebyproduct.Style.Add("font-size", "12px");
        if (gv_booking3bydatebyproduct.HeaderRow != null)
        {
            gv_booking3bydatebyproduct.HeaderRow.Style.Add("background", "#000");
        }
        for (int i = 0; i < gv_booking3bydatebyproduct.Rows.Count; i++)
        {
            foreach (TableCell cell in gv_booking3bydatebyproduct.Rows[i].Cells)
            {
                cell.Style.Clear();
                cell.Style.Add("background", "#ccffff");
            }
        }
        //By Wsy 20110517 ITEM 22 ADD End 
        bindDataTotalByDateByProduct(gv_booking3Totalbydatebyproduct, nextyear.Substring(2, 2), nextyear.Substring(2, 2),false);
        //By Wsy 20110517 ITEM 22 ADD Start 
        gv_booking3Totalbydatebyproduct.Style.Clear();
        gv_booking3Totalbydatebyproduct.Style.Add("border", "#000 solid 1px");
        gv_booking3Totalbydatebyproduct.Style.Add("border-collapse", "collapse");
        gv_booking3Totalbydatebyproduct.Style.Add("font-size", "12px");
        if (gv_booking3Totalbydatebyproduct.HeaderRow != null)
        {
            gv_booking3Totalbydatebyproduct.HeaderRow.Style.Add("background", "#000");
        }
        for (int i = 0; i < gv_booking3Totalbydatebyproduct.Rows.Count; i++)
        {
            foreach (TableCell cell in gv_booking3Totalbydatebyproduct.Rows[i].Cells)
            {
                cell.Style.Clear();
                cell.Style.Add("background", "#ccffff");
            }
        }
        //By Wsy 20110517 ITEM 22 ADD End 
        gv_booking4bydatebyproduct.Columns.Clear();
        gv_booking4Totalbydatebyproduct.Columns.Clear();

        bindDataByDateByProduct(gv_booking4bydatebyproduct, nextyear.Substring(2, 2), yearAfterNext.Substring(2, 2),false);
        //By Wsy 20110517 ITEM 22 ADD Start 
        gv_booking4bydatebyproduct.Style.Clear();
        gv_booking4bydatebyproduct.Style.Add("border", "#000 solid 1px");
        gv_booking4bydatebyproduct.Style.Add("border-collapse", "collapse");
        gv_booking4bydatebyproduct.Style.Add("font-size", "12px");
        if (gv_booking4bydatebyproduct.HeaderRow != null)
        {
            gv_booking4bydatebyproduct.HeaderRow.Style.Add("background", "#000");
        }
        for (int i = 0; i < gv_booking4bydatebyproduct.Rows.Count; i++)
        {
            foreach (TableCell cell in gv_booking4bydatebyproduct.Rows[i].Cells)
            {
                cell.Style.Clear();
                cell.Style.Add("background", "#FF9");
            }
        }
        //By Wsy 20110517 ITEM 22 ADD End 
        bindDataTotalByDateByProduct(gv_booking4Totalbydatebyproduct, nextyear.Substring(2, 2), yearAfterNext.Substring(2, 2),false);
        //By Wsy 20110517 ITEM 22 ADD Start 
        gv_booking4Totalbydatebyproduct.Style.Clear();
        gv_booking4Totalbydatebyproduct.Style.Add("border", "#000 solid 1px");
        gv_booking4Totalbydatebyproduct.Style.Add("border-collapse", "collapse");
        gv_booking4Totalbydatebyproduct.Style.Add("font-size", "12px");
        if (gv_booking4Totalbydatebyproduct.HeaderRow != null)
        {
            gv_booking4Totalbydatebyproduct.HeaderRow.Style.Add("background", "#000");
        }
        for (int i = 0; i < gv_booking4Totalbydatebyproduct.Rows.Count; i++)
        {
            foreach (TableCell cell in gv_booking4Totalbydatebyproduct.Rows[i].Cells)
            {
                cell.Style.Clear();
                cell.Style.Add("background", "#FF9");
            }
        }
        //By Wsy 20110517 ITEM 22 ADD End 
        /* Data by date by product  end */

        if (flag)
        {
            /* Data by product  start*/
            //By Wsy 20110517 ITEM 22 DEL Start 
            // bindDataByProduct();
            //By Wsy 20110517 ITEM 22 DEL End 

            //By Wsy 20110517 ITEM 22 ADD Start 
            int gvCount = bindDataByProduct();
            //By Wsy 20110517 ITEM 22 ADD End 

            //By Wsy 20110517 ITEM 22 ADD Start 
            string gvColor1 = "";
            string gvColor2 = "";
            if (gvCount % 2 == 0)
            {
                 gvColor1 = "#ccffff";
                 gvColor2 = "#FF9";
            }
            else {
                 gvColor1 = "#FF9";
                 gvColor2 = "#ccffff";
            }
            //By Wsy 20110517 ITEM 22 ADD End 
            /* Data by product  end */

            /* Data by date  start */

            gv_bookingtbydate.Columns.Clear();
            gv_bookingtTotalbydate.Columns.Clear();

            bindDataByDate(gv_bookingtbydate);
            //By Wsy 20110517 ITEM 22 ADD Start 
            gv_bookingtbydate.Style.Clear();
            gv_bookingtbydate.Style.Add("border", "#000 solid 1px");
            gv_bookingtbydate.Style.Add("border-collapse", "collapse");
            gv_bookingtbydate.Style.Add("font-size", "12px");
            if (gv_bookingtbydate.HeaderRow != null)
            {
                gv_bookingtbydate.HeaderRow.Style.Add("background", "#000");
            }
            for (int i = 0; i < gv_bookingtbydate.Rows.Count; i++)
            {
                foreach (TableCell cell in gv_bookingtbydate.Rows[i].Cells)
                {
                    cell.Style.Clear();
                    cell.Style.Add("background", gvColor1);
                }
            }
            //By Wsy 20110517 ITEM 22 ADD End 
            bindDataTotalByDate(gv_bookingtTotalbydate);
            //By Wsy 20110517 ITEM 22 ADD Start 
            gv_bookingtTotalbydate.Style.Clear();
            gv_bookingtTotalbydate.Style.Add("border", "#000 solid 1px");
            gv_bookingtTotalbydate.Style.Add("border-collapse", "collapse");
            gv_bookingtTotalbydate.Style.Add("font-size", "12px");
            if (gv_bookingtTotalbydate.HeaderRow != null)
            {
                gv_bookingtTotalbydate.HeaderRow.Style.Add("background", "#000");
            }
            for (int i = 0; i < gv_bookingtTotalbydate.Rows.Count; i++)
            {
                foreach (TableCell cell in gv_bookingtTotalbydate.Rows[i].Cells)
                {
                    cell.Style.Clear();
                    cell.Style.Add("background", gvColor1);
                }
            }
            //By Wsy 20110517 ITEM 22 ADD End 
            gv_bookingnextbydate.Columns.Clear();
            gv_bookingnextTotalbydate.Columns.Clear();

            bindDataNextByDate(gv_bookingnextbydate);
            //By Wsy 20110517 ITEM 22 ADD Start 
            gv_bookingnextbydate.Style.Clear();
            gv_bookingnextbydate.Style.Add("border", "#000 solid 1px");
            gv_bookingnextbydate.Style.Add("border-collapse", "collapse");
            gv_bookingnextbydate.Style.Add("font-size", "12px");
            if (gv_bookingnextbydate.HeaderRow != null)
            {
                gv_bookingnextbydate.HeaderRow.Style.Add("background", "#000");
            }
            for (int i = 0; i < gv_bookingnextbydate.Rows.Count; i++)
            {
                foreach (TableCell cell in gv_bookingnextbydate.Rows[i].Cells)
                {
                    cell.Style.Clear();
                    cell.Style.Add("background", gvColor2);
                }
            }
            //By Wsy 20110517 ITEM 22 ADD End 
            bindDataNextTotalByDate(gv_bookingnextTotalbydate);
            //By Wsy 20110517 ITEM 22 ADD Start 
            gv_bookingnextTotalbydate.Style.Clear();
            gv_bookingnextTotalbydate.Style.Add("border", "#000 solid 1px");
            gv_bookingnextTotalbydate.Style.Add("border-collapse", "collapse");
            gv_bookingnextTotalbydate.Style.Add("font-size", "12px");
            if (gv_bookingnextTotalbydate.HeaderRow != null)
            {
                gv_bookingnextTotalbydate.HeaderRow.Style.Add("background", "#000");
            }
            for (int i = 0; i < gv_bookingnextTotalbydate.Rows.Count; i++)
            {
                foreach (TableCell cell in gv_bookingnextTotalbydate.Rows[i].Cells)
                {
                    cell.Style.Clear();
                    cell.Style.Add("background", gvColor2);
                }
            }
            //By Wsy 20110517 ITEM 22 ADD End 
            /* Data by date  end */

            /* Comparsion */
            gv_VS.Columns.Clear();
            gv_VSTotal.Columns.Clear();

            bindDataNextVSThisByDate(gv_VS);
            //By Wsy 20110517 ITEM 22 ADD Start 
            gv_VS.Style.Clear();
            gv_VS.Style.Add("border", "#000 solid 1px");
            gv_VS.Style.Add("border-collapse", "collapse");
            gv_VS.Style.Add("font-size", "12px");
            if (gv_VS.HeaderRow != null)
            {
                gv_VS.HeaderRow.Style.Add("background", "#000");
            }
            for (int i = 0; i < gv_VS.Rows.Count; i++)
            {
                foreach (TableCell cell in gv_VS.Rows[i].Cells)
                {
                    cell.Style.Clear();
                    cell.Style.Add("background", gvColor1);
                }
            }
            //By Wsy 20110517 ITEM 22 ADD End 
            bindDataTotalNextVSThisByDate(gv_VSTotal);
            //By Wsy 20110517 ITEM 22 ADD Start 
            gv_VSTotal.Style.Clear();
            gv_VSTotal.Style.Add("border", "#000 solid 1px");
            gv_VSTotal.Style.Add("border-collapse", "collapse");
            gv_VSTotal.Style.Add("font-size", "12px");
            if (gv_VSTotal.HeaderRow != null)
            {
                gv_VSTotal.HeaderRow.Style.Add("background", "#000");
            }
            for (int i = 0; i < gv_VSTotal.Rows.Count; i++)
            {
                foreach (TableCell cell in gv_VSTotal.Rows[i].Cells)
                {
                    cell.Style.Clear();
                    cell.Style.Add("background", gvColor1);
                }
            }
            //By Wsy 20110517 ITEM 22 ADD End 
        }
    }

    protected void btn_search_Click(object sender, EventArgs e)
    {
        //By Fxw 20110517 ITEM25 ADD Start
        string query_date = "SELECT CONVERT(varchar(15),SelectMeetingDate,23) FROM [SetSelectMeetingDate] where userid=" + Session["AssistantID"].ToString();
        DataSet ds_date = helper.GetDataSet(query_date);
        if (ds_date.Tables[0].Rows.Count > 0 && !ds_date.Tables[0].Rows[0][0].ToString().Equals("") && ds_date.Tables[0].Rows[0][0].ToString() != null)
        {
            label_show.Text = "This report is related to the meeting date " + ds_date.Tables[0].Rows[0][0].ToString();
        }
        else
        {
            label_show.Text = "There is no meeting date selected!";
        }
        //By Fxw 20110517 ITEM25 ADD End
        panel_search.Visible = true;
        panel_head.Visible = true;

        label_headdescription.Text = getSegmentDec(getSegmentID()) + " -New Orders in " + getMeetingDateYear() + " BY SALES ORGANIZATION";

        getCurrencyBySalesOrg(ddlist_salesOrg.SelectedItem.Value);
        label_salesorgAbbr.Text = getAbbrBySalesOrg(ddlist_salesOrg.SelectedItem.Value);

        this.btn_EUR.Enabled = true;
        this.hidCurrencyFlag.Value = "0";

        bindDataSource();
    }

    /* Set Date */

    protected static string preyear;
    protected static string year;
    protected static string nextyear;
    protected static string yearAfterNext;
    protected static string month;
    protected static string premonth;

    protected void setDate()
    {
        //ryzhang item49 20110519 del start
        //year = getMeetingDateYear();
        //month = getMeetingDateMonth();
        //ryzhang item49 20110519 del end
        //ryzhang item49 20110519 add start
        date.setSelectDate(Session["AssistantID"].ToString());
        year = date.getyear();
        month = date.getmonth();
        //ryzhang item49 20110519 add end
        premonth = date.getPreMonth(month);

        preyear = (int.Parse(year) - 1).ToString();
        nextyear = (int.Parse(year) + 1).ToString();
        yearAfterNext = (int.Parse(nextyear) + 1).ToString();
    }

    protected string getMeetingDateYear()
    {
        string query_meetingyear = "SELECT YEAR(MeetingDate) FROM [SetMeetingDate]";
        DataSet ds_year = helper.GetDataSet(query_meetingyear);

        if (ds_year.Tables[0].Rows.Count > 0)
        {
            string str_year = ds_year.Tables[0].Rows[0][0].ToString();
            return str_year;
        }
        return null;
    }

    protected string getMeetingDateMonth()
    {
        string query_meetingmonth = "SELECT MONTH(MeetingDate) FROM [SetMeetingDate]";
        DataSet ds_month = helper.GetDataSet(query_meetingmonth);

        if (ds_month.Tables[0].Rows.Count > 0)
        {
            string str_month = ds_month.Tables[0].Rows[0][0].ToString();
            return str_month;
        }
        return null;
    }

    protected string getMeetingDateDay()
    {
        string query_meetingday = "SELECT Day(MeetingDate) FROM [SetMeetingDate]";
        DataSet ds_day = helper.GetDataSet(query_meetingday);

        if (ds_day.Tables[0].Rows.Count > 0)
        {
            string str_day = ds_day.Tables[0].Rows[0][0].ToString();
            return str_day;
        }
        return null;
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

    protected DataSet getBookingDataByDateByProduct(DataSet dsPro, string segID, string bookingY, string deliverY, string salesOrgID,bool bz)
    {
        if (dsPro != null)
        {
            string sqlstr = null;
            if (string.Equals(this.hidCurrencyFlag.Value, "0"))
            {
                string temp = "";
                sqlstr = "SELECT [SubRegion].Name AS SubRegion,[Country].ISO_Code AS Country";
                for (int count = 0; count < dsPro.Tables[0].Rows.Count; count++)
                {
                    temp += " ,SUM(CASE WHEN ProductID = " + dsPro.Tables[0].Rows[count][0].ToString() + " AND YEAR(TimeFlag) = '" + year + "' AND MONTH(TimeFlag) = '" + month + "' THEN (CASE WHEN [Bookings].BookingY='" + bookingY + "' AND DeliverY='" + deliverY + "' THEN ROUND(Amount,0) ELSE 0 END ) ELSE 0 END) AS '"
                          + dsPro.Tables[0].Rows[count][1].ToString() + "'";
                }
                temp += " FROM [Bookings]"
                      + " INNER JOIN [SubRegion] ON [Bookings].CountryID = [SubRegion].ID"
                      + " INNER JOIN [Country_SubRegion] ON [Bookings].CountryID = [Country_SubRegion].SubRegionID"
                      + " INNER JOIN [Country] ON [Country].ID = [Country_SubRegion].CountryID"
                      + " WHERE SegmentID = " + segID
                      + " AND SalesOrgID = " + salesOrgID
                      + " AND [SubRegion].Deleted=0 "
                      + " AND [Country_SubRegion].Deleted=0 "
                      + " AND [Country].Deleted=0 "
                      + " GROUP BY SubRegion.Name,Country.ISO_Code"
                      + " ORDER BY SubRegion.Name ASC";

                sqlstr += temp;
            }
            else if (string.Equals(this.hidCurrencyFlag.Value, "1"))
            {
                StringBuilder selSql = new StringBuilder();
                selSql.AppendLine(" SELECT ");
                selSql.AppendLine("   [SubRegion].Name AS SubRegion, ");
                selSql.AppendLine("   [Country].ISO_Code AS Country ");
                for (int count = 0; count < dsPro.Tables[0].Rows.Count; count++)
                {
                    selSql.AppendLine("   ,SUM(CASE WHEN ProductID=" + dsPro.Tables[0].Rows[count][0].ToString());
                    selSql.AppendLine("                  AND YEAR([Bookings].TimeFlag)=" + year);
                    selSql.AppendLine("                  AND MONTH([Bookings].TimeFlag)=" + month);
                    selSql.AppendLine("                  AND YEAR([Currency_Exchange].TimeFlag)=" + year);
                    selSql.AppendLine("                  AND MONTH([Currency_Exchange].TimeFlag)=" + month);
                    selSql.AppendLine("             THEN (CASE WHEN [Bookings].BookingY='" + bookingY + "' AND DeliverY='" + deliverY + "' ");
                    selSql.AppendLine("                        THEN (CASE WHEN DeliverY='YTD' ");
                    selSql.AppendLine("                                   THEN ROUND([Bookings].Amount*[Currency_Exchange].Rate1,0) ");
                    //yyan 20110831 itemw126 edit start
                    if (bz && (month == "3" || month == "03"))
                    {
                        selSql.AppendLine("                                   ELSE ROUND([Bookings].Amount*[Currency_Exchange].Rate1,0) ");
                    }
                    else
                    {
                        selSql.AppendLine("                                   ELSE ROUND([Bookings].Amount*[Currency_Exchange].Rate2,0) ");
                    }
                    //yyan 20110831 itemw126 edit end
                    selSql.AppendLine("                              END) ");
                    selSql.AppendLine("                        ELSE 0 ");
                    selSql.AppendLine("                   END) ");
                    selSql.AppendLine("             ELSE 0 ");
                    selSql.AppendLine("        END) AS '" + dsPro.Tables[0].Rows[count][1].ToString() + "' ");
                }
                selSql.AppendLine(" FROM ");
                selSql.AppendLine("   [Bookings] ");
                selSql.AppendLine("   INNER JOIN [SubRegion] ON [Bookings].CountryID=[SubRegion].ID ");
                selSql.AppendLine("   INNER JOIN [Country_SubRegion] ON [Bookings].CountryID=[Country_SubRegion].SubRegionID ");
                selSql.AppendLine("   INNER JOIN [Country] ON [Country].ID=[Country_SubRegion].CountryID ");
                selSql.AppendLine("   INNER JOIN [SalesOrg] ON [Bookings].SalesOrgID=[SalesOrg].ID ");
                selSql.AppendLine("   INNER JOIN [Currency_Exchange] ON [Currency_Exchange].CurrencyID=[SalesOrg].CurrencyID ");
                selSql.AppendLine(" WHERE ");
                selSql.AppendLine("   [SubRegion].Deleted=0 ");
                selSql.AppendLine("   AND [Country_SubRegion].Deleted=0 ");
                selSql.AppendLine("   AND [Country].Deleted=0 ");
                selSql.AppendLine("   AND [SalesOrg].Deleted=0 ");
                selSql.AppendLine("   AND [Currency_Exchange].Deleted=0 ");
                selSql.AppendLine("   AND SegmentID=" + segID);
                selSql.AppendLine("   AND SalesOrgID=" + salesOrgID);
                selSql.AppendLine(" GROUP BY ");
                selSql.AppendLine("   SubRegion.Name, ");
                selSql.AppendLine("   Country.ISO_Code ");
                selSql.AppendLine(" ORDER BY ");
                selSql.AppendLine("   SubRegion.Name ASC ");
                sqlstr = selSql.ToString();
            }
            DataSet ds = helper.GetDataSet(sqlstr);

            if (ds.Tables[0].Rows.Count > 0)
            {
                return ds;
            }
        }
        return null;
    }

    protected DataSet getProductBySegment(string segmentID)
    {
        return sql.getProductInfoBySegmentID(segmentID);
    }

    protected void bindDataByDateByProduct(GridView gv, string bookingY, string deliverY,bool bz)
    {
        DataSet ds_product = getProductBySegment(getSegmentID());
        DataSet ds = getBookingDataByDateByProduct(ds_product, getSegmentID(), bookingY, deliverY, ddlist_salesOrg.SelectedItem.Value,bz);

        if (ds != null)
        {
            gv.Width = Unit.Pixel(650);
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

                if (i == 0 || i == 1)
                {
                    bf.ItemStyle.Width = 100;
                    bf.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
                    bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
                }

                gv.Columns.Add(bf);
            }

            if (deliverY == "YTD")
                gv.Caption = bookingY + deliverY;
            else
                gv.Caption = bookingY + " for " + deliverY;
            gv.CaptionAlign = TableCaptionAlign.Top;
            gv.AllowSorting = true;
            gv.DataSource = ds.Tables[0];
            gv.DataBind();
            flag = true;
        }
        else
        {
            flag = false;
            gv.Visible = false;
        }
    }

    /* GridView Booking Total Data */

    /// <summary>
    /// Get booking total data of every product by operation
    /// </summary>
    /// <param name="dsPro"></param>
    /// <param name="segmentID"></param>
    /// <param name="RSMID"></param>
    /// <param name="bookingY"></param>
    /// <param name="deliverY"></param>
    /// <returns></returns>

    protected DataSet getBookingDataTotalByDateByProduct(DataSet dsPro, string segID, string salesOrgID, string bookingY, string deliverY,bool bz)
    {
        string str_salesOrgabbr = getAbbrBySalesOrg(ddlist_salesOrg.SelectedItem.Value);
        if (dsPro != null)
        {
            string sqlstr = null;
            if (string.Equals(this.hidCurrencyFlag.Value, "0"))
            {

                sqlstr = "SELECT ('" + str_salesOrgabbr + "/'+[Operation].AbbrL) AS Operation, [Operation].Abbr";
                string temp = "";
                for (int count = 0; count < dsPro.Tables[0].Rows.Count; count++)
                {
                    temp += " ,SUM(CASE WHEN ProductID = " + dsPro.Tables[0].Rows[count][0].ToString() + " AND YEAR(TimeFlag) = '" + year + "' AND MONTH(TimeFlag) = '" + month + "'  AND BookingY='" + bookingY + "' AND DeliverY='" + deliverY + "' THEN ROUND(Amount,0) ELSE 0 END) AS '"
                          + dsPro.Tables[0].Rows[count][1].ToString() + "'";
                }
                temp += " FROM [Bookings] "
                     + " INNER JOIN [Operation] ON [Bookings].OperationID = [Operation].ID"
                     + " WHERE SegmentID = " + segID
                     + " AND SalesOrgID = " + salesOrgID
                     + " AND Operation.Deleted=0"
                     + " GROUP BY Operation.AbbrL, [Operation].Abbr"
                     + " ORDER BY [Operation].AbbrL, [Operation].Abbr ASC";

                sqlstr += temp;
            }
            else if (string.Equals(this.hidCurrencyFlag.Value, "1"))
            {
                StringBuilder selSql = new StringBuilder();
                selSql.AppendLine(" SELECT ");
                selSql.AppendLine("   ('" + str_salesOrgabbr + "/'+[Operation].AbbrL) AS Operation, ");
                selSql.AppendLine("   [Operation].Abbr ");
                for (int count = 0; count < dsPro.Tables[0].Rows.Count; count++)
                {
                    selSql.AppendLine("   ,SUM(CASE WHEN ProductID=" + dsPro.Tables[0].Rows[count][0].ToString());
                    selSql.AppendLine("                  AND YEAR([Bookings].TimeFlag)='" + year + "' ");
                    selSql.AppendLine("                  AND MONTH([Bookings].TimeFlag)='" + month + "' ");
                    selSql.AppendLine("                  AND YEAR([Currency_Exchange].TimeFlag)='" + year + "' ");
                    selSql.AppendLine("                  AND MONTH([Currency_Exchange].TimeFlag)='" + month + "' ");
                    selSql.AppendLine("                  AND BookingY='" + bookingY + "' ");
                    selSql.AppendLine("                  AND DeliverY='" + deliverY + "' ");
                    selSql.AppendLine("             THEN (CASE WHEN DeliverY='YTD' ");
                    selSql.AppendLine("                        THEN ROUND([Bookings].Amount*[Currency_Exchange].Rate1,0) ");
                    //yyan 20110831 itemw126 edit start
                    if (bz && (month=="3" || month=="03"))
                    {selSql.AppendLine("                        ELSE ROUND([Bookings].Amount*[Currency_Exchange].Rate1,0) ");
                    } else
                    {selSql.AppendLine("                        ELSE ROUND([Bookings].Amount*[Currency_Exchange].Rate2,0) ");
                    }
                    //yyan 20110831 itemw126 edit end
                    selSql.AppendLine("                   END) ");
                    selSql.AppendLine("             ELSE 0 ");
                    selSql.AppendLine("     END) AS '" + dsPro.Tables[0].Rows[count][1].ToString() + "' ");
                }
                selSql.AppendLine(" FROM ");
                selSql.AppendLine("   [Bookings] ");
                selSql.AppendLine("   INNER JOIN [Operation] ON [Bookings].OperationID=[Operation].ID ");
                selSql.AppendLine("   INNER JOIN [SalesOrg] ON [Bookings].SalesOrgID=[SalesOrg].ID ");
                selSql.AppendLine("   INNER JOIN [Currency_Exchange] ON [Currency_Exchange].CurrencyID=[SalesOrg].CurrencyID ");
                selSql.AppendLine(" WHERE ");
                selSql.AppendLine("   SegmentID=" + segID);
                selSql.AppendLine("   AND SalesOrgID=" + salesOrgID);
                selSql.AppendLine("   AND Operation.Deleted=0 ");
                selSql.AppendLine("   AND SalesOrg.Deleted=0 ");
                selSql.AppendLine("   AND Currency_Exchange.Deleted=0 ");
                selSql.AppendLine(" GROUP BY ");
                selSql.AppendLine("   Operation.AbbrL, ");
                selSql.AppendLine("   Operation.Abbr ");
                selSql.AppendLine(" ORDER BY ");
                selSql.AppendLine("   Operation.AbbrL, ");
                selSql.AppendLine("   Operation.Abbr ");
                sqlstr = selSql.ToString();
            }
            DataSet ds = helper.GetDataSet(sqlstr);

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                return ds;
            }
        }
        return null;
    }

    protected void bindDataTotalByDateByProduct(GridView gv, string bookingY, string deliverY,bool bz)
    {
        DataSet ds_product = getProductBySegment(getSegmentID());
        DataSet ds = getBookingDataTotalByDateByProduct(ds_product, getSegmentID(), ddlist_salesOrg.SelectedItem.Value, bookingY, deliverY,bz);

        if (ds != null)
        {
            gv.Width = Unit.Pixel(650);
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
                    bf.ItemStyle.Width = 200;
                }
                if (i == 1)
                {
                    bf.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
                    bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
                }
                bf.ReadOnly = true;

                gv.Columns.Add(bf);
            }

            gv.AllowSorting = true;
            gv.DataSource = ds.Tables[0];

            DataRow drSum = ds.Tables[0].NewRow();
            DataRow[] rows = ds.Tables[0].Select();

            float[] Sum = new float[20];
            for (int j = 2; j < ds.Tables[0].Columns.Count; j++)
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
                    drSum[0] = "TTL/" + getAbbrBySalesOrg(ddlist_salesOrg.SelectedItem.Value);
                    drSum[1] = "Abbr";
                }
                else
                {
                    drSum[i] = Sum[i].ToString();
                }
            }
            ds.Tables[0].Rows.InsertAt(drSum, 0);
            gv.DataBind();
            gv.Columns[1].Visible = false;
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

    protected string getProductAbbr(string productID)
    {
        string query_productID = "SELECT Abbr FROM [Product] WHERE Deleted = 0 AND ID = " + productID;
        DataSet ds_productID = helper.GetDataSet(query_productID);

        if (ds_productID.Tables[0].Rows.Count > 0)
            return ds_productID.Tables[0].Rows[0][0].ToString();
        else
            return null;
    }

    protected DataSet getBookingsDataByProduct(string productID, string salesOrgID)
    {
        string sqlstr = null;
        if (string.Equals(this.hidCurrencyFlag.Value, "0"))
        {
            sqlstr = "SELECT [SubRegion].Name AS SubRegion"
                                + ",SUM(CASE WHEN ProductID = " + productID + " AND [BookingY]='" + year.Substring(2, 2) + "' AND YEAR(TimeFlag)=" + year + " AND MONTH(TimeFlag)=" + month + " THEN ROUND(Amount,0) ELSE 0 END) AS '" + year.Substring(2, 2) + "'"
                                + ",SUM(CASE WHEN ProductID = " + productID + " AND [BookingY]='" + year.Substring(2, 2) + "' AND YEAR(TimeFlag)=" + preyear + " AND MONTH(TimeFlag)=03 THEN ROUND(Amount,0) ELSE 0 END) AS '" + year.Substring(2, 2) + "F/C" + year.Substring(2, 2) + "' FROM [Bookings] "
                                + " INNER JOIN [SubRegion] ON [Bookings].CountryID = [SubRegion].ID"
                                + " INNER JOIN [Country_SubRegion] ON [Bookings].CountryID = [Country_SubRegion].SubRegionID"
                                + " INNER JOIN [Country] ON [Country].ID = [Country_SubRegion].CountryID"
                                + " WHERE SegmentID = " + getSegmentID()
                                + " AND SalesOrgID = " + salesOrgID
                                + " AND [SubRegion].Deleted=0 "
                                + " AND [Country_SubRegion].Deleted=0 "
                                + " AND [Country].Deleted=0 "
                                + " GROUP BY [SubRegion].Name"
                                + " ORDER BY [SubRegion].Name ASC";
        }
        else if (string.Equals(this.hidCurrencyFlag.Value, "1"))
        {
            StringBuilder selSql = new StringBuilder();
            selSql.AppendLine(" SELECT ");
            selSql.AppendLine("   [SubRegion].Name AS SubRegion, ");
            selSql.AppendLine("   SUM(CASE WHEN ProductID=" + productID);
            selSql.AppendLine("                 AND [BookingY]='" + year.Substring(2, 2) + "' ");
            selSql.AppendLine("                 AND YEAR([Bookings].TimeFlag)=" + year);
            selSql.AppendLine("                 AND MONTH([Bookings].TimeFlag)=" + month);
            selSql.AppendLine("                 AND YEAR([Currency_Exchange].TimeFlag)=" + year);
            selSql.AppendLine("                 AND MONTH([Currency_Exchange].TimeFlag)=" + month);
            selSql.AppendLine("            THEN (CASE WHEN [Bookings].BookingY='" + year.Substring(2, 2) + "' AND [Bookings].DeliverY='YTD' ");
            selSql.AppendLine("                       THEN ROUND([Bookings].Amount*[Currency_Exchange].Rate1,0) ");
            selSql.AppendLine("                       ELSE ROUND([Bookings].Amount*[Currency_Exchange].Rate2,0) ");
            selSql.AppendLine("                  END) ");
            selSql.AppendLine("            ELSE 0 ");
            selSql.AppendLine("       END) AS '" + year.Substring(2, 2) + "', ");
            selSql.AppendLine("   SUM(CASE WHEN ProductID=" + productID);
            selSql.AppendLine("                 AND [BookingY]='" + year.Substring(2, 2) + "' ");
            selSql.AppendLine("                 AND YEAR([Bookings].TimeFlag)=" + preyear);
            selSql.AppendLine("                 AND MONTH([Bookings].TimeFlag)=03 ");
            selSql.AppendLine("                 AND YEAR([Currency_Exchange].TimeFlag)=" + preyear);
            selSql.AppendLine("                 AND MONTH([Currency_Exchange].TimeFlag)=03 ");
            selSql.AppendLine("            THEN (CASE WHEN [Bookings].BookingY='" + year.Substring(2, 2) + "' AND [Bookings].DeliverY='YTD' ");
            selSql.AppendLine("                       THEN ROUND([Bookings].Amount*[Currency_Exchange].Rate1,0) ");
            selSql.AppendLine("                       ELSE ROUND([Bookings].Amount*[Currency_Exchange].Rate2,0) ");
            selSql.AppendLine("                  END) ");
            selSql.AppendLine("            ELSE 0 ");
            selSql.AppendLine("       END) AS '" + year.Substring(2, 2) + "F/C" + year.Substring(2, 2) + "' ");
            selSql.AppendLine(" FROM ");
            selSql.AppendLine("   [Bookings] ");
            selSql.AppendLine("   INNER JOIN [SubRegion] ON [Bookings].CountryID=[SubRegion].ID ");
            selSql.AppendLine("   INNER JOIN [Country_SubRegion] ON [Bookings].CountryID=[Country_SubRegion].SubRegionID ");
            selSql.AppendLine("   INNER JOIN [Country] ON [Country].ID=[Country_SubRegion].CountryID ");
            selSql.AppendLine("   INNER JOIN [SalesOrg] ON [Bookings].SalesOrgID = [SalesOrg].ID ");
            selSql.AppendLine("   INNER JOIN [Currency_Exchange] ON [Currency_Exchange].CurrencyID = [SalesOrg].CurrencyID ");
            selSql.AppendLine(" WHERE ");
            selSql.AppendLine("   [SubRegion].Deleted=0 ");
            selSql.AppendLine("   AND [Country_SubRegion].Deleted=0 ");
            selSql.AppendLine("   AND [Country].Deleted=0 ");
            selSql.AppendLine("   AND [SalesOrg].Deleted=0 ");
            selSql.AppendLine("   AND [Currency_Exchange].Deleted=0 ");
            selSql.AppendLine("   AND SegmentID=" + getSegmentID());
            selSql.AppendLine("   AND SalesOrgID=" + salesOrgID);
            selSql.AppendLine(" GROUP BY ");
            selSql.AppendLine("   [SubRegion].Name ");
            selSql.AppendLine(" ORDER BY ");
            selSql.AppendLine("   [SubRegion].Name ASC ");
            sqlstr = selSql.ToString();
        }
        DataSet ds = helper.GetDataSet(sqlstr);

        if (ds.Tables[0].Rows.Count > 0)
            return ds;
        else
            return null;
    }

    protected GridView showBookingsByProduct(DataSet ds, GridView gv, string header)
    {
        if (ds != null)
        {
            gv.AutoGenerateColumns = false;
            gv.Width = Unit.Pixel(300);
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
                Response.Redirect("~/Assistant/AssistantError.aspx");
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
                }
                bf.ReadOnly = true;

                gv.Columns.Add(bf);
            }
            gv.Caption = header;
            gv.CaptionAlign = TableCaptionAlign.Top;
            gv.DataSource = ds;
            gv.DataBind();
        }
        return gv;
    }
    //By Wsy 20110517 ITEM 22 Del Start 
    //protected void bindDataByProduct()
    //By Wsy 20110517 ITEM 22 Del End 

    //By Wsy 20110517 ITEM 22 ADD Start 
    protected int bindDataByProduct()
    //By Wsy 20110517 ITEM 22 ADD End 
    {
        DataSet dsPro = getProductBySegment(getSegmentID());

        DataSet[] dsBookingsByProduct = new DataSet[dsPro.Tables[0].Rows.Count];
        DataSet[] dsTotalByProduct = new DataSet[dsPro.Tables[0].Rows.Count];
        GridView[] gvBookingsByProduct = new GridView[dsPro.Tables[0].Rows.Count];
        GridView[] gvTotalByProduct = new GridView[dsPro.Tables[0].Rows.Count];

        TableRow tr = new TableRow();
        table_bookingsByProduct.Rows.Add(tr);
        table_bookingsByProduct.Visible = true;
        //By Wsy 20110517 ITEM 22 ADD Start 
        int gvCount = 0;
        //By Wsy 20110517 ITEM 22 ADD End 
        for (int count = 0; count < dsPro.Tables[0].Rows.Count; count++)
        {
            TableCell tc = new TableCell();
            tc.HorizontalAlign = HorizontalAlign.Left;
            tc.VerticalAlign = VerticalAlign.Top;

            string productID = dsPro.Tables[0].Rows[count][0].ToString();
            string productName = dsPro.Tables[0].Rows[count][1].ToString();

            //New the instance to this controls
            gvBookingsByProduct[count] = new GridView();
            web.setProperties(gvBookingsByProduct[count]);
            gvTotalByProduct[count] = new GridView();
            web.setProperties(gvTotalByProduct[count]);

            dsBookingsByProduct[count] = getBookingsDataByProduct(productID, ddlist_salesOrg.SelectedItem.Value);
            if (dsBookingsByProduct[count] != null)
            {
                if (dsBookingsByProduct[count].Tables.Count > 0)
                {
                    gvBookingsByProduct[count] = showBookingsByProduct(dsBookingsByProduct[count], gvBookingsByProduct[count], productName);
                    //By Wsy 20110517 ITEM 22 ADD Start 
                    if (count % 2 == 0)
                    {
                        gvBookingsByProduct[count].Style.Clear();
                        gvBookingsByProduct[count].Style.Add("border", "#000 solid 1px");
                        gvBookingsByProduct[count].Style.Add("border-collapse", "collapse");
                        gvBookingsByProduct[count].Style.Add("font-size", "12px");
                        if (gvBookingsByProduct[count].HeaderRow != null)
                        {
                            gvBookingsByProduct[count].HeaderRow.Style.Add("background", "#000");
                        }
                        for (int i = 0; i < gvBookingsByProduct[count].Rows.Count; i++)
                        {
                            foreach (TableCell cell in gvBookingsByProduct[count].Rows[i].Cells)
                            {
                                cell.Style.Clear();
                                cell.Style.Add("background", "#ccffff");
                            }
                        }
                    }
                    else
                    {
                        gvBookingsByProduct[count].Style.Clear();
                        gvBookingsByProduct[count].Style.Add("border", "#000 solid 1px");
                        gvBookingsByProduct[count].Style.Add("border-collapse", "collapse");
                        gvBookingsByProduct[count].Style.Add("font-size", "12px");
                        if (gvBookingsByProduct[count].HeaderRow != null)
                        {
                            gvBookingsByProduct[count].HeaderRow.Style.Add("background", "#000");
                        }
                        for (int i = 0; i < gvBookingsByProduct[count].Rows.Count; i++)
                        {
                            foreach (TableCell cell in gvBookingsByProduct[count].Rows[i].Cells)
                            {
                                cell.Style.Clear();
                                cell.Style.Add("background", "#FF9");
                            }
                        }
                    }
                    //By Wsy 20110517 ITEM 22 ADD End 
                    tc.Controls.Add(gvBookingsByProduct[count]);
                }
                else
                {
                    log.WriteLog(LogUtility.LogErrorLevel.LOG_CRITICAL, string.Format("Error:ds.Tables.Count is {0}", dsPro.Tables.Count));
                    Response.Redirect("~/Assistant/AssistantError.aspx");
                }
            }

            dsTotalByProduct[count] = getBookingDataTotalByProduct(productID, ddlist_salesOrg.SelectedItem.Value);
            if (dsTotalByProduct[count] != null)
            {
                if (dsTotalByProduct[count].Tables.Count > 0)
                {
                    gvTotalByProduct[count] = showTotalByProduct(dsTotalByProduct[count], gvTotalByProduct[count], productName);
                    //By Wsy 20110517 ITEM 22 ADD Start 
                    if (count % 2 == 0)
                    {
                        gvTotalByProduct[count].Style.Clear();
                        gvTotalByProduct[count].Style.Add("border", "#000 solid 1px");
                        gvTotalByProduct[count].Style.Add("border-collapse", "collapse");
                        gvTotalByProduct[count].Style.Add("font-size", "12px");
                        if (gvTotalByProduct[count].HeaderRow != null)
                        {
                            gvTotalByProduct[count].HeaderRow.Style.Add("background", "#000");
                        }
                        for (int i = 0; i < gvTotalByProduct[count].Rows.Count; i++)
                        {
                            foreach (TableCell cell in gvTotalByProduct[count].Rows[i].Cells)
                            {
                                cell.Style.Clear();
                                cell.Style.Add("background", "#ccffff");
                            }
                        }
                    }
                    else
                    {
                        gvTotalByProduct[count].Style.Clear();
                        gvTotalByProduct[count].Style.Add("border", "#000 solid 1px");
                        gvTotalByProduct[count].Style.Add("border-collapse", "collapse");
                        gvTotalByProduct[count].Style.Add("font-size", "12px");
                        if (gvTotalByProduct[count].HeaderRow != null)
                        {
                            gvTotalByProduct[count].HeaderRow.Style.Add("background", "#000");
                        }
                        for (int i = 0; i < gvTotalByProduct[count].Rows.Count; i++)
                        {
                            foreach (TableCell cell in gvTotalByProduct[count].Rows[i].Cells)
                            {
                                cell.Style.Clear();
                                cell.Style.Add("background", "#FF9");
                            }
                        }
                    }
                    //By Wsy 20110517 ITEM 22 ADD End 
                    tc.Controls.Add(gvTotalByProduct[count]);
                }
                else
                {
                    log.WriteLog(LogUtility.LogErrorLevel.LOG_CRITICAL, string.Format("Error:ds.Tables.Count is {0}", dsPro.Tables.Count));
                    Response.Redirect("~/Assistant/AssistantError.aspx");
                }
            }

            tr.Controls.Add(tc);
            //By Wsy 20110517 ITEM 22 ADD Start 
            gvCount++;
            //By Wsy 20110517 ITEM 22 ADD End 
        }
        //By Wsy 20110517 ITEM 22 ADD Start 
        return gvCount;
        //By Wsy 20110517 ITEM 22 ADD End 
    }

    /* GridView Booking Total Data By Product */

    /// <summary>
    /// Get booking total data of a product by operation
    /// </summary>
    /// <param name="productID"></param>
    /// <param name="RSMID"></param>
    /// <returns></returns>

    protected DataSet getBookingDataTotalByProduct(string productID, string salesOrgID)
    {
        string sqlstr = null;
        if (string.Equals(this.hidCurrencyFlag.Value, "0"))
        {
            sqlstr = "SELECT ('" + getAbbrBySalesOrg(ddlist_salesOrg.SelectedItem.Value) + "/'+[Operation].AbbrL) AS Operation"
                                + ",SUM(CASE WHEN ProductID = " + productID + " AND BookingY='" + year.Substring(2, 2) + "' AND YEAR(TimeFlag)=" + year + " AND MONTH(TimeFlag)=" + month + " THEN ROUND(Amount,0) ELSE 0 END) AS '" + year.Substring(2, 2) + "'"
                                + ",SUM(CASE WHEN ProductID = " + productID + " AND BookingY='" + year.Substring(2, 2) + "' AND YEAR(TimeFlag)=" + preyear + " AND MONTH(TimeFlag)=03 THEN ROUND(Amount,0) ELSE 0 END) AS '" + year.Substring(2, 2) + "F/C" + year.Substring(2, 2) + "' FROM Bookings "
                                + " INNER JOIN Operation ON Bookings.OperationID=Operation.ID"
                                + " WHERE SegmentID = " + getSegmentID()
                                + " AND SalesOrgID = " + salesOrgID
                                + " AND Operation.Deleted=0"
                                + " GROUP BY Operation.AbbrL"
                                + " ORDER BY Operation.AbbrL ASC";
        }
        else if (string.Equals(this.hidCurrencyFlag.Value, "1"))
        {
            StringBuilder selSql = new StringBuilder();
            selSql.AppendLine(" SELECT ");
            selSql.AppendLine("   ('" + getAbbrBySalesOrg(ddlist_salesOrg.SelectedItem.Value) + "/'+Operation.AbbrL) AS Operation, ");
            selSql.AppendLine("   SUM(CASE WHEN ProductID=" + productID);
            selSql.AppendLine("                 AND BookingY='" + year.Substring(2, 2) + "' ");
            selSql.AppendLine("                 AND YEAR(Bookings.TimeFlag)=" + year);
            selSql.AppendLine("                 AND MONTH(Bookings.TimeFlag)=" + month);
            selSql.AppendLine("                 AND YEAR(Currency_Exchange.TimeFlag)=" + year);
            selSql.AppendLine("                 AND MONTH(Currency_Exchange.TimeFlag)=" + month);
            selSql.AppendLine("            THEN (CASE WHEN DeliverY='YTD' ");
            selSql.AppendLine("                       THEN ROUND([Bookings].Amount*[Currency_Exchange].Rate1,0) ");
            selSql.AppendLine("                       ELSE ROUND([Bookings].Amount*[Currency_Exchange].Rate2,0) ");
            selSql.AppendLine("                  END) ");
            selSql.AppendLine("            ELSE 0 ");
            selSql.AppendLine("       END) AS '" + year.Substring(2, 2) + "', ");
            selSql.AppendLine("   SUM(CASE WHEN ProductID=" + productID);
            selSql.AppendLine("                 AND BookingY='" + year.Substring(2, 2) + "' ");
            selSql.AppendLine("                 AND YEAR(Bookings.TimeFlag)=" + preyear);
            selSql.AppendLine("                 AND MONTH(Bookings.TimeFlag)=03 ");
            selSql.AppendLine("                 AND YEAR(Currency_Exchange.TimeFlag)=" + preyear);
            selSql.AppendLine("                 AND MONTH(Currency_Exchange.TimeFlag)=03 ");
            selSql.AppendLine("            THEN (CASE WHEN DeliverY='YTD' ");
            selSql.AppendLine("                       THEN ROUND([Bookings].Amount*[Currency_Exchange].Rate1,0) ");
            selSql.AppendLine("                       ELSE ROUND([Bookings].Amount*[Currency_Exchange].Rate2,0) ");
            selSql.AppendLine("                  END) ");
            selSql.AppendLine("            ELSE 0 ");
            selSql.AppendLine("       END) AS '" + year.Substring(2, 2) + "F/C" + year.Substring(2, 2) + "' ");
            selSql.AppendLine(" FROM ");
            selSql.AppendLine("   Bookings ");
            selSql.AppendLine("   INNER JOIN Operation ON Bookings.OperationID=Operation.ID ");
            selSql.AppendLine("   INNER JOIN SalesOrg ON Bookings.SalesOrgID=SalesOrg.ID ");
            selSql.AppendLine("   INNER JOIN Currency_Exchange ON Currency_Exchange.CurrencyID=SalesOrg.CurrencyID ");
            selSql.AppendLine(" WHERE ");
            selSql.AppendLine("   SegmentID=" + getSegmentID());
            selSql.AppendLine("   AND SalesOrgID=" + salesOrgID);
            selSql.AppendLine("   AND Operation.Deleted=0 ");
            selSql.AppendLine("   AND SalesOrg.Deleted=0 ");
            selSql.AppendLine("   AND Currency_Exchange.Deleted=0 ");
            selSql.AppendLine(" GROUP BY ");
            selSql.AppendLine("   Operation.AbbrL ");
            selSql.AppendLine(" ORDER BY ");
            selSql.AppendLine("   Operation.AbbrL ");
            sqlstr = selSql.ToString();
        }
        DataSet ds = helper.GetDataSet(sqlstr);

        if (ds.Tables[0].Rows.Count > 0)
            return ds;
        else
            return null;
    }

    protected GridView showTotalByProduct(DataSet ds, GridView gv, string productName)
    {
        if (ds != null)
        {
            gv.AutoGenerateColumns = false;
            gv.Width = Unit.Pixel(300);
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
                Response.Redirect("~/Assistant/AssistantError.aspx");
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
                }
                bf.ReadOnly = true;

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
                    drSum[0] = "TTL/" + getAbbrBySalesOrg(ddlist_salesOrg.SelectedItem.Value);
                }
                else
                {
                    drSum[i] = Sum[i].ToString();
                }
            }

            ds.Tables[0].Rows.InsertAt(drSum, 0);
            if (float.Parse(ds.Tables[0].Rows[0][2].ToString()) != 0)
            {
                ds.Tables[0].Rows[0][3] = (Convert.ToInt32((float.Parse(ds.Tables[0].Rows[0][1].ToString()) - float.Parse(ds.Tables[0].Rows[0][2].ToString())) * 100 / float.Parse(ds.Tables[0].Rows[0][2].ToString()))).ToString() + "%";
            }

            gv.DataBind();
        }
        return gv;
    }

    /* GridView Booking Total Data AND Forecast Data This Fiscal Year By Country*/

    /// <summary>
    /// Get booking real ROUND(Amount,0) this year and booking estimating ROUND(Amount,0) last year
    /// </summary>
    /// <param name="segmentID"></param>
    /// <param name="RSMID"></param>
    /// <returns></returns>

    protected DataSet getBookingsDataThisyear(string segmentID, string salesOrgID)
    {
        string sqlstr = null;
        if (string.Equals(this.hidCurrencyFlag.Value, "0"))
        {
            sqlstr = "SELECT SubRegion.Name AS SubRegion"
                            + " ,SUM(CASE WHEN YEAR(TimeFlag)=" + year + " AND BookingY='" + year.Substring(2, 2) + "' AND MONTH(TimeFlag)=" + month + " THEN ROUND(Amount,0) ELSE 0 END) AS '" + year.Substring(2, 2) + "'"
                            + " ,SUM(CASE WHEN YEAR(TimeFlag)=" + preyear + " AND BookingY='" + year.Substring(2, 2) + "' AND MONTH(TimeFlag) = '03' THEN ROUND(Amount,0) ELSE 0 END) AS '" + year.Substring(2, 2) + "F/C" + year.Substring(2, 2) + "'"
                            + " FROM [Bookings] "
                            + " INNER JOIN [SubRegion] ON [Bookings].CountryID = [SubRegion].ID"
                            + " INNER JOIN [Country_SubRegion] ON [Bookings].CountryID = [Country_SubRegion].SubRegionID"
                            + " INNER JOIN [Country] ON [Country].ID = [Country_SubRegion].CountryID"
                            + " WHERE SegmentID='" + segmentID + "'"
                            + " AND SalesOrgID = " + salesOrgID
                            + " AND [SubRegion].Deleted=0 "
                            + " AND [Country_SubRegion].Deleted=0 "
                            + " AND [Country].Deleted=0 "
                            + " GROUP BY SubRegion.Name"
                            + " ORDER BY SubRegion.Name ASC";
        }
        else if (string.Equals(this.hidCurrencyFlag.Value, "1"))
        {
            StringBuilder selSql = new StringBuilder();
            selSql.AppendLine(" SELECT ");
            selSql.AppendLine("   SubRegion.Name AS SubRegion, ");
            selSql.AppendLine("   SUM(CASE WHEN BookingY='" + year.Substring(2, 2) + "' ");
            selSql.AppendLine(" 				AND YEAR([Bookings].TimeFlag)=" + year);
            selSql.AppendLine("                 AND MONTH([Bookings].TimeFlag)=" + month);
            selSql.AppendLine("                 AND YEAR([Currency_Exchange].TimeFlag)=" + year);
            selSql.AppendLine("                 AND MONTH([Currency_Exchange].TimeFlag)=" + month);
            selSql.AppendLine("            THEN (CASE WHEN DeliverY='YTD' ");
            selSql.AppendLine("                       THEN ROUND([Bookings].Amount*[Currency_Exchange].Rate1,0) ");
            selSql.AppendLine("                       ELSE ROUND([Bookings].Amount*[Currency_Exchange].Rate2,0) ");
            selSql.AppendLine("                  END) ");
            selSql.AppendLine("            ELSE 0 ");
            selSql.AppendLine("       END) AS '" + year.Substring(2, 2) + "', ");
            selSql.AppendLine("   SUM(CASE WHEN BookingY='" + year.Substring(2, 2) + "' ");
            selSql.AppendLine(" 				AND YEAR([Bookings].TimeFlag)=" + preyear);
            selSql.AppendLine("                 AND MONTH([Bookings].TimeFlag)='03' ");
            selSql.AppendLine("                 AND YEAR([Currency_Exchange].TimeFlag)=" + preyear);
            selSql.AppendLine("                 AND MONTH([Currency_Exchange].TimeFlag)='03' ");
            selSql.AppendLine("            THEN (CASE WHEN DeliverY='YTD' ");
            selSql.AppendLine("                       THEN ROUND([Bookings].Amount*[Currency_Exchange].Rate1,0) ");
            selSql.AppendLine("                       ELSE ROUND([Bookings].Amount*[Currency_Exchange].Rate2,0) ");
            selSql.AppendLine("                  END) ");
            selSql.AppendLine("            ELSE 0 ");
            selSql.AppendLine("       END) AS '" + year.Substring(2, 2) + "F/C" + year.Substring(2, 2) + "' ");
            selSql.AppendLine(" FROM ");
            selSql.AppendLine("   [Bookings] ");
            selSql.AppendLine("   INNER JOIN [SubRegion] ON [Bookings].CountryID=[SubRegion].ID ");
            selSql.AppendLine("   INNER JOIN [Country_SubRegion] ON [Bookings].CountryID=[Country_SubRegion].SubRegionID ");
            selSql.AppendLine("   INNER JOIN [Country] ON [Country].ID=[Country_SubRegion].CountryID ");
            selSql.AppendLine("   INNER JOIN [SalesOrg] ON [Bookings].SalesOrgID=[SalesOrg].ID ");
            selSql.AppendLine("   INNER JOIN [Currency_Exchange] ON [Currency_Exchange].CurrencyID=[SalesOrg].CurrencyID ");
            selSql.AppendLine(" WHERE ");
            selSql.AppendLine("   [SubRegion].Deleted=0 ");
            selSql.AppendLine("   AND [Country_SubRegion].Deleted=0 ");
            selSql.AppendLine("   AND [Country].Deleted=0 ");
            selSql.AppendLine("   AND [SalesOrg].Deleted=0 ");
            selSql.AppendLine("   AND [Currency_Exchange].Deleted=0 ");
            selSql.AppendLine("   AND SegmentID=" + segmentID);
            selSql.AppendLine("   AND SalesOrgID=" + salesOrgID);
            selSql.AppendLine(" GROUP BY ");
            selSql.AppendLine("   SubRegion.Name ");
            selSql.AppendLine(" ORDER BY ");
            selSql.AppendLine("   SubRegion.Name ASC ");
            sqlstr = selSql.ToString();
        }
        DataSet ds = helper.GetDataSet(sqlstr);

        if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            return ds;
        else
            return null;
    }

    protected void bindDataByDate(GridView gv)
    {
        DataSet ds = getBookingsDataThisyear(getSegmentID(), ddlist_salesOrg.SelectedItem.Value);
        if (ds != null)
        {
            gv.Width = Unit.Pixel(300);
            gv.AutoGenerateColumns = false;
            gv.AllowPaging = false;
            gv.Visible = true;

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
                Response.Redirect("~/Assistant/AssistantError.aspx");
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
                }
                bf.ReadOnly = true;
                gv.Columns.Add(bf);
            }

            gv.Caption = "Total(" + year.Substring(2, 2) + ")";
            gv.CaptionAlign = TableCaptionAlign.Top;
            gv.AllowSorting = true;
            gv.DataSource = ds.Tables[0];
            gv.DataBind();
        }
        else
            gv.Visible = false;
    }

    /* GridView Booking Total Data By Operation This Year*/

    /// <summary>
    /// Get booking total ROUND(Amount,0) this year
    /// </summary>
    /// <param name="segmentID"></param>
    /// <param name="RSMID"></param>
    /// <returns></returns>

    protected DataSet getBookingDataTotalThisYear(string segmentID, string salesOrgID)
    {
        string sqlstr = null;
        if (string.Equals(this.hidCurrencyFlag.Value, "0"))
        {
            sqlstr = "SELECT ('" + getAbbrBySalesOrg(ddlist_salesOrg.SelectedItem.Value) + "/'+[Operation].AbbrL) AS Operation"
                                + " ,SUM(CASE WHEN YEAR(TimeFlag)=" + year + " AND BookingY='" + year.Substring(2, 2) + "' AND MONTH(TimeFlag)=" + month + " THEN ROUND(Amount,0) ELSE 0 END) AS '" + year.Substring(2, 2) + "'"
                                + " ,SUM(CASE WHEN YEAR(TimeFlag)=" + preyear + " AND BookingY='" + year.Substring(2, 2) + "' AND MONTH(TimeFlag)=03 THEN ROUND(Amount,0) ELSE 0 END) AS '" + year.Substring(2, 2) + "F/C" + year.Substring(2, 2) + "'"
                                + " FROM [Bookings] "
                                + " INNER JOIN [Operation] ON [Bookings].OperationID=[Operation].ID"
                                + " WHERE SegmentID='" + segmentID + "'"
                                + " AND SalesOrgID = " + salesOrgID
                                + " AND Operation.Deleted=0 "
                                + " GROUP BY [Operation].AbbrL"
                                + " ORDER BY [Operation].AbbrL ASC";
        }
        else if (string.Equals(this.hidCurrencyFlag.Value, "1"))
        {
            StringBuilder selSql = new StringBuilder();
            selSql.AppendLine(" SELECT ");
            selSql.AppendLine("   ('" + getAbbrBySalesOrg(ddlist_salesOrg.SelectedItem.Value) + "/'+[Operation].AbbrL) AS Operation, ");
            selSql.AppendLine("   SUM(CASE WHEN BookingY='" + year.Substring(2, 2) + "' ");
            selSql.AppendLine("                 AND YEAR([Bookings].TimeFlag)=" + year);
            selSql.AppendLine("                 AND MONTH([Bookings].TimeFlag)=" + month);
            selSql.AppendLine("                 AND YEAR([Currency_Exchange].TimeFlag)=" + year);
            selSql.AppendLine("                 AND MONTH([Currency_Exchange].TimeFlag)=" + month);
            selSql.AppendLine("            THEN (CASE WHEN DeliverY='YTD' ");
            selSql.AppendLine("                       THEN ROUND([Bookings].Amount*[Currency_Exchange].Rate1,0) ");
            selSql.AppendLine("                       ELSE ROUND([Bookings].Amount*[Currency_Exchange].Rate2,0) ");
            selSql.AppendLine("                  END) ");
            selSql.AppendLine("            ELSE 0 ");
            selSql.AppendLine("       END) AS '" + year.Substring(2, 2) + "', ");
            selSql.AppendLine("   SUM(CASE WHEN BookingY='" + year.Substring(2, 2) + "' ");
            selSql.AppendLine("                 AND YEAR([Bookings].TimeFlag)=" + preyear);
            selSql.AppendLine("                 AND MONTH([Bookings].TimeFlag)=03 ");
            selSql.AppendLine("                 AND YEAR([Currency_Exchange].TimeFlag)=" + preyear);
            selSql.AppendLine("                 AND MONTH([Currency_Exchange].TimeFlag)=03 ");
            selSql.AppendLine("            THEN (CASE WHEN DeliverY='YTD' ");
            selSql.AppendLine("                       THEN ROUND([Bookings].Amount*[Currency_Exchange].Rate1,0) ");
            selSql.AppendLine("                       ELSE ROUND([Bookings].Amount*[Currency_Exchange].Rate2,0) ");
            selSql.AppendLine("                  END) ");
            selSql.AppendLine("            ELSE 0 ");
            selSql.AppendLine("       END) AS '" + year.Substring(2, 2) + "F/C" + year.Substring(2, 2) + "' ");
            selSql.AppendLine(" FROM ");
            selSql.AppendLine("   [Bookings] ");
            selSql.AppendLine("   INNER JOIN [Operation] ON [Bookings].OperationID=[Operation].ID ");
            selSql.AppendLine("   INNER JOIN [SalesOrg] ON [Bookings].SalesOrgID=[SalesOrg].ID ");
            selSql.AppendLine("   INNER JOIN [Currency_Exchange] ON [Currency_Exchange].CurrencyID=[SalesOrg].CurrencyID ");
            selSql.AppendLine(" WHERE ");
            selSql.AppendLine("   SegmentID=" + segmentID);
            selSql.AppendLine("   AND SalesOrgID=" + salesOrgID);
            selSql.AppendLine("   AND Operation.Deleted=0 ");
            selSql.AppendLine("   AND SalesOrg.Deleted=0 ");
            selSql.AppendLine("   AND Currency_Exchange.Deleted=0 ");
            selSql.AppendLine(" GROUP BY ");
            selSql.AppendLine("   [Operation].AbbrL ");
            selSql.AppendLine(" ORDER BY ");
            selSql.AppendLine("   [Operation].AbbrL ");
            sqlstr = selSql.ToString();
        }
        DataSet ds = helper.GetDataSet(sqlstr);

        if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            return ds;
        else
            return null;
    }

    protected void bindDataTotalByDate(GridView gv)
    {
        DataSet ds = getBookingDataTotalThisYear(getSegmentID(), ddlist_salesOrg.SelectedItem.Value);
        if (ds != null)
        {
            gv.Width = Unit.Pixel(300);
            gv.AutoGenerateColumns = false;
            gv.AllowPaging = false;
            gv.Visible = true;

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
                Response.Redirect("~/Assistant/AssistantError.aspx");
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
                }
                bf.ReadOnly = true;

                gv.Columns.Add(bf);
            }

            gv.AllowSorting = true;
            gv.DataSource = ds.Tables[0];

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
                    drSum[0] = "TTL/" + getAbbrBySalesOrg(ddlist_salesOrg.SelectedItem.Value);
                }
                else
                {
                    drSum[i] = Sum[i].ToString();
                }
            }

            ds.Tables[0].Rows.InsertAt(drSum, 0);
            if (float.Parse(ds.Tables[0].Rows[0][2].ToString()) != 0)
            {
                ds.Tables[0].Rows[0][3] = (Convert.ToInt32((float.Parse(ds.Tables[0].Rows[0][1].ToString()) - float.Parse(ds.Tables[0].Rows[0][2].ToString())) * 100 / float.Parse(ds.Tables[0].Rows[0][2].ToString()))).ToString() + "%";
            }
            gv.DataBind();
        }
        else
            gv.Visible = false;
    }

    /* GridView Booking Total Data By Country Next Year */

    /// <summary>
    /// Get
    /// </summary>
    /// <param name="dsPro"></param>
    /// <param name="segmentID"></param>
    /// <param name="RSMID"></param>
    /// <returns></returns>

    protected DataSet getBookingsDataNextYear(DataSet dsPro, string segmentID, string salesOrgID)
    {
        DataTable dt = dsPro.Tables[0];
        string sqlstr = null;
        if (string.Equals(this.hidCurrencyFlag.Value, "0"))
        {
            string temp = "";
            sqlstr = "SELECT SubRegion.Name AS SubRegion";
            for (int count = 0; count < dt.Rows.Count; count++)
            {
                temp += " ,SUM(CASE WHEN ProductID = " + dt.Rows[count][0].ToString() + " AND BookingY='" + nextyear.Substring(2, 2) + "' AND YEAR(TimeFlag)=" + year + " AND MONTH(TimeFlag)=" + month + " THEN ROUND(Amount,0) ELSE 0 END) AS '"
                      + dt.Rows[count][1].ToString() + "'";
            }
            temp += " FROM Bookings INNER JOIN SubRegion ON Bookings.CountryID=SubRegion.ID"
                  + " INNER JOIN [Country_SubRegion] ON [Bookings].CountryID = [Country_SubRegion].SubRegionID"
                  + " INNER JOIN [Country] ON [Country].ID = [Country_SubRegion].CountryID"
                  + " WHERE SegmentID='" + segmentID + "'"
                  + " AND SalesOrgID = " + salesOrgID
                  + " AND [SubRegion].Deleted=0 "
                  + " AND [Country_SubRegion].Deleted=0 "
                  + " AND [Country].Deleted=0 "
                  + " GROUP BY SubRegion.Name"
                  + " ORDER BY SubRegion.Name ASC";

            sqlstr += temp;
        }
        else if (string.Equals(this.hidCurrencyFlag.Value, "1"))
        {
            StringBuilder selSql = new StringBuilder();
            selSql.AppendLine(" SELECT ");
            selSql.AppendLine("   SubRegion.Name AS SubRegion ");
            for (int count = 0; count < dt.Rows.Count; count++)
            {
                selSql.AppendLine("   ,SUM(CASE WHEN ProductID=" + dt.Rows[count][0].ToString());
                selSql.AppendLine("                  AND BookingY='" + nextyear.Substring(2, 2) + "' ");
                selSql.AppendLine("                  AND YEAR([Bookings].TimeFlag)=" + year);
                selSql.AppendLine("                  AND MONTH([Bookings].TimeFlag)=" + month);
                selSql.AppendLine("                  AND YEAR([Currency_Exchange].TimeFlag)=" + year);
                selSql.AppendLine("                  AND MONTH([Currency_Exchange].TimeFlag)=" + month);
                selSql.AppendLine("             THEN (CASE WHEN DeliverY='YTD' ");
                selSql.AppendLine("                        THEN ROUND([Bookings].Amount*[Currency_Exchange].Rate1,0) ");
                selSql.AppendLine("                        ELSE ROUND([Bookings].Amount*[Currency_Exchange].Rate2,0) ");
                selSql.AppendLine("                   END) ");
                selSql.AppendLine("             ELSE 0 ");
                selSql.AppendLine("        END) AS '" + dt.Rows[count][1].ToString() + "' ");
            }
            selSql.AppendLine(" FROM ");
            selSql.AppendLine("   Bookings ");
            selSql.AppendLine("   INNER JOIN SubRegion ON Bookings.CountryID=SubRegion.ID ");
            selSql.AppendLine("   INNER JOIN [Country_SubRegion] ON [Bookings].CountryID=[Country_SubRegion].SubRegionID ");
            selSql.AppendLine("   INNER JOIN [Country] ON [Country].ID=[Country_SubRegion].CountryID ");
            selSql.AppendLine("   INNER JOIN [SalesOrg] ON [Bookings].SalesOrgID=[SalesOrg].ID ");
            selSql.AppendLine("   INNER JOIN [Currency_Exchange] ON [Currency_Exchange].CurrencyID=[SalesOrg].CurrencyID ");
            selSql.AppendLine(" WHERE ");
            selSql.AppendLine("   [SubRegion].Deleted=0 ");
            selSql.AppendLine("   AND [Country_SubRegion].Deleted=0 ");
            selSql.AppendLine("   AND [Country].Deleted=0 ");
            selSql.AppendLine("   AND [SalesOrg].Deleted=0 ");
            selSql.AppendLine("   AND [Currency_Exchange].Deleted=0 ");
            selSql.AppendLine("   AND SegmentID=" + segmentID);
            selSql.AppendLine("   AND SalesOrgID=" + salesOrgID);
            selSql.AppendLine(" GROUP BY ");
            selSql.AppendLine("   SubRegion.Name ");
            selSql.AppendLine(" ORDER BY ");
            selSql.AppendLine("   SubRegion.Name ");
            sqlstr = selSql.ToString();
        }
        DataSet ds = helper.GetDataSet(sqlstr);

        if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            return ds;
        else
            return null;
    }

    protected void bindDataNextByDate(GridView gv)
    {
        DataSet ds = getBookingsDataNextYear(getProductBySegment(getSegmentID()), getSegmentID(), ddlist_salesOrg.SelectedItem.Value);
        if (ds != null)
        {
            gv.Width = Unit.Pixel(700);
            gv.AutoGenerateColumns = false;
            gv.AllowPaging = false;
            gv.Visible = true;

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
                Response.Redirect("~/Assistant/AssistantError.aspx");
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
                }
                bf.ReadOnly = true;
                gv.Columns.Add(bf);
            }

            gv.Caption = "Total(" + nextyear.Substring(2, 2) + ")";
            gv.CaptionAlign = TableCaptionAlign.Top;
            gv.AllowSorting = true;
            gv.DataSource = ds.Tables[0];
            gv.DataBind();
        }
        else
            gv.Visible = false;
    }

    /* GridView Booking Total Data By Operation Next Year*/

    /// <summary>
    /// Get
    /// </summary>
    /// <param name="dsPro"></param>
    /// <param name="segmentID"></param>
    /// <param name="RSMID"></param>
    /// <returns></returns>

    protected DataSet getBookingDataTotalNextYear(DataSet dsPro, string segmentID, string salesOrgID)
    {
        DataTable dt = dsPro.Tables[0];
        string sqlstr = null;
        if (string.Equals(this.hidCurrencyFlag.Value, "0"))
        {
            sqlstr = "SELECT ('" + getAbbrBySalesOrg(ddlist_salesOrg.SelectedItem.Value) + "/'+[Operation].AbbrL) AS Operation";
            string temp = "";
            for (int count = 0; count < dt.Rows.Count; count++)
            {
                temp += " ,SUM(CASE WHEN ProductID = " + dt.Rows[count][0].ToString() + " AND BookingY='" + nextyear.Substring(2, 2) + "' AND YEAR(TimeFlag)=" + year + " AND MONTH(TimeFlag)=" + month + " THEN ROUND(Amount,0) ELSE 0 END) AS '"
                      + dt.Rows[count][1].ToString() + "'";
            }
            temp += " FROM Bookings INNER JOIN Operation ON Bookings.OperationID = Operation.ID"
                  + " WHERE SegmentID='" + segmentID + "'"
                  + " AND SalesOrgID = " + salesOrgID
                  + " AND Operation.Deleted=0 "
                  + " GROUP BY Operation.AbbrL"
                  + " ORDER BY Operation.AbbrL ASC";

            sqlstr += temp;
        }
        else if (string.Equals(this.hidCurrencyFlag.Value, "1"))
        {
            StringBuilder selSql = new StringBuilder();
            selSql.AppendLine(" SELECT ");
            selSql.AppendLine("   ('" + getAbbrBySalesOrg(ddlist_salesOrg.SelectedItem.Value) + "/'+[Operation].AbbrL) AS Operation ");
            for (int count = 0; count < dt.Rows.Count; count++)
            {
                selSql.AppendLine("   ,SUM(CASE WHEN ProductID=" + dt.Rows[count][0].ToString());
                selSql.AppendLine("                  AND BookingY='" + nextyear.Substring(2, 2) + "' ");
                selSql.AppendLine("                  AND YEAR(Bookings.TimeFlag)=" + year);
                selSql.AppendLine("                  AND MONTH(Bookings.TimeFlag)=" + month);
                selSql.AppendLine("                  AND YEAR([Currency_Exchange].TimeFlag)=" + year);
                selSql.AppendLine("                  AND MONTH([Currency_Exchange].TimeFlag)=" + month);
                selSql.AppendLine("             THEN (CASE WHEN DeliverY='YTD' ");
                selSql.AppendLine("                        THEN ROUND([Bookings].Amount*[Currency_Exchange].Rate1,0) ");
                selSql.AppendLine("                        ELSE ROUND([Bookings].Amount*[Currency_Exchange].Rate2,0) ");
                selSql.AppendLine("                   END) ");
                selSql.AppendLine("             ELSE 0 ");
                selSql.AppendLine("        END) AS '" + dt.Rows[count][1].ToString() + "' ");
            }
            selSql.AppendLine(" FROM ");
            selSql.AppendLine("   Bookings ");
            selSql.AppendLine("   INNER JOIN Operation ON Bookings.OperationID=Operation.ID ");
            selSql.AppendLine("   INNER JOIN [SalesOrg] ON [Bookings].SalesOrgID=[SalesOrg].ID ");
            selSql.AppendLine("   INNER JOIN [Currency_Exchange] ON [Currency_Exchange].CurrencyID=[SalesOrg].CurrencyID ");
            selSql.AppendLine(" WHERE ");
            selSql.AppendLine("   SegmentID=" + segmentID);
            selSql.AppendLine("   AND SalesOrgID=" + salesOrgID);
            selSql.AppendLine("   AND Operation.Deleted=0 ");
            selSql.AppendLine("   AND SalesOrg.Deleted=0 ");
            selSql.AppendLine("   AND Currency_Exchange.Deleted=0 ");
            selSql.AppendLine(" GROUP BY ");
            selSql.AppendLine("   Operation.AbbrL ");
            selSql.AppendLine(" ORDER BY ");
            selSql.AppendLine("   Operation.AbbrL ");
            sqlstr = selSql.ToString();
        }
        DataSet ds = helper.GetDataSet(sqlstr);

        if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            return ds;
        else
            return null;
    }

    protected void bindDataNextTotalByDate(GridView gv)
    {
        DataSet ds = getBookingDataTotalNextYear(getProductBySegment(getSegmentID()), getSegmentID(), ddlist_salesOrg.SelectedItem.Value);
        if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        {
            gv.Width = Unit.Pixel(700);
            gv.AutoGenerateColumns = false;
            gv.AllowPaging = false;
            gv.Visible = true;

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
                Response.Redirect("~/Assistant/AssistantError.aspx");
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
                    drSum[0] = "TTL/" + getAbbrBySalesOrg(ddlist_salesOrg.SelectedItem.Value);
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

    protected DataSet getBookingsDataNextYearVSThis(DataSet dsPro, string segmentID, string str_salesOrgID)
    {
        if (dsPro != null)
        {
            DataTable dt = dsPro.Tables[0];
            string sqlstr = null;
            if (string.Equals(this.hidCurrencyFlag.Value, "0"))
            {
                sqlstr = "SELECT SubRegion.Name AS SubRegion,Country.ISO_Code AS Country";
                string temp = "";
                for (int count = 0; count < dt.Rows.Count; count++)
                {
                    temp += " ,SUM(CASE WHEN ProductID = " + dt.Rows[count][0].ToString();

                    if (date.JudgeFirstMonth(month))
                        temp += " AND YEAR(TimeFlag)='" + preyear + "' AND MONTH(TimeFlag)='" + premonth + "'"
                              + " AND BookingY = '" + year.Substring(2, 2)
                              + "' THEN ROUND(Amount,0) ELSE 0 END) AS '"
                              + dt.Rows[count][1].ToString() + "'";
                    else
                        temp += " AND YEAR(TimeFlag)='" + year + "' AND MONTH(TimeFlag)='" + premonth + "'"
                              + " AND BookingY = '" + year.Substring(2, 2)
                              + "' THEN ROUND(Amount,0) ELSE 0 END) AS '"
                              + dt.Rows[count][1].ToString() + "'";
                }

                temp += " FROM Bookings INNER JOIN SubRegion ON Bookings.CountryID=SubRegion.ID"
                  + " INNER JOIN [Country_SubRegion] ON [Bookings].CountryID = [Country_SubRegion].SubRegionID"
                  + " INNER JOIN [Country] ON [Country].ID = [Country_SubRegion].CountryID"
                  + " WHERE SegmentID='" + segmentID + "'"
                  + " AND SalesOrgID = " + str_salesOrgID
                  + " AND [SubRegion].Deleted=0 "
                  + " AND [Country_SubRegion].Deleted=0 "
                  + " AND [Country].Deleted=0 "
                  + " GROUP BY SubRegion.Name,Country.ISO_Code"
                  + " ORDER BY SubRegion.Name,Country.ISO_Code ASC";

                sqlstr += temp;
            }
            else if (string.Equals(this.hidCurrencyFlag.Value, "1"))
            {
                StringBuilder selSql = new StringBuilder();
                selSql.AppendLine(" SELECT ");
                selSql.AppendLine("   SubRegion.Name AS SubRegion, ");
                selSql.AppendLine("   Country.ISO_Code AS Country ");
                for (int count = 0; count < dt.Rows.Count; count++)
                {
                    selSql.AppendLine("   ,SUM(CASE WHEN ProductID=" + dt.Rows[count][0].ToString());
                    if (date.JudgeFirstMonth(month))
                    {
                        selSql.AppendLine("                  AND YEAR([Bookings].TimeFlag)=" + preyear);
                        selSql.AppendLine("                  AND MONTH([Bookings].TimeFlag)=" + premonth);
                        selSql.AppendLine("                  AND YEAR([Currency_Exchange].TimeFlag)=" + preyear);
                        selSql.AppendLine("                  AND MONTH([Currency_Exchange].TimeFlag)=" + premonth);
                        selSql.AppendLine("                  AND BookingY='" + year.Substring(2, 2) + "' ");
                        selSql.AppendLine("             THEN (CASE WHEN DeliverY='YTD' ");
                        selSql.AppendLine("                        THEN ROUND([Bookings].Amount*[Currency_Exchange].Rate1,0) ");
                        selSql.AppendLine("                        ELSE ROUND([Bookings].Amount*[Currency_Exchange].Rate2,0) ");
                        selSql.AppendLine("                   END) ");
                        selSql.AppendLine("             ELSE 0 ");
                        selSql.AppendLine("        END) AS '" + dt.Rows[count][1].ToString() + "' ");
                    }
                    else
                    {
                        selSql.AppendLine("                  AND YEAR([Bookings].TimeFlag)=" + year);
                        selSql.AppendLine("                  AND MONTH([Bookings].TimeFlag)=" + premonth);
                        selSql.AppendLine("                  AND YEAR([Currency_Exchange].TimeFlag)=" + year);
                        selSql.AppendLine("                  AND MONTH([Currency_Exchange].TimeFlag)=" + premonth);
                        selSql.AppendLine("                  AND BookingY='" + year.Substring(2, 2) + "' ");
                        selSql.AppendLine("             THEN (CASE WHEN DeliverY='YTD' ");
                        selSql.AppendLine("                       THEN ROUND([Bookings].Amount*[Currency_Exchange].Rate1,0) ");
                        selSql.AppendLine("                       ELSE ROUND([Bookings].Amount*[Currency_Exchange].Rate2,0) ");
                        selSql.AppendLine("                  END) ");
                        selSql.AppendLine("             ELSE 0 ");
                        selSql.AppendLine("        END) AS '" + dt.Rows[count][1].ToString() + "' ");
                    }
                }
                selSql.AppendLine(" FROM ");
                selSql.AppendLine("   Bookings ");
                selSql.AppendLine("   INNER JOIN SubRegion ON Bookings.CountryID=SubRegion.ID ");
                selSql.AppendLine("   INNER JOIN [Country_SubRegion] ON [Bookings].CountryID=[Country_SubRegion].SubRegionID ");
                selSql.AppendLine("   INNER JOIN [Country] ON [Country].ID = [Country_SubRegion].CountryID ");
                selSql.AppendLine("   INNER JOIN [SalesOrg] ON [Bookings].SalesOrgID=[SalesOrg].ID ");
                selSql.AppendLine("   INNER JOIN [Currency_Exchange] ON [Currency_Exchange].CurrencyID=[SalesOrg].CurrencyID ");
                selSql.AppendLine(" WHERE ");
                selSql.AppendLine("   [SubRegion].Deleted=0 ");
                selSql.AppendLine("   AND [Country_SubRegion].Deleted=0 ");
                selSql.AppendLine("   AND [Country].Deleted=0 ");
                selSql.AppendLine("   AND [SalesOrg].Deleted=0 ");
                selSql.AppendLine("   AND [Currency_Exchange].Deleted=0 ");
                selSql.AppendLine("   AND SegmentID=" + segmentID);
                selSql.AppendLine("   AND SalesOrgID=" + str_salesOrgID);
                selSql.AppendLine(" GROUP BY ");
                selSql.AppendLine("   SubRegion.Name, ");
                selSql.AppendLine("   Country.ISO_Code ");
                selSql.AppendLine(" ORDER BY ");
                selSql.AppendLine("   SubRegion.Name, ");
                selSql.AppendLine("   Country.ISO_Code ");
                sqlstr = selSql.ToString();
            }
            DataSet ds = helper.GetDataSet(sqlstr);

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                return ds;
            else
                return null;
        }
        else
            return null;
    }

    protected void bindDataNextVSThisByDate(GridView gv)
    {
        DataSet ds = getBookingsDataNextYearVSThis(getProductBySegment(getSegmentID()), getSegmentID(), ddlist_salesOrg.SelectedItem.Value);
        if (ds != null)
        {
            gv.Visible = true;
            gv.Width = Unit.Pixel(700);
            gv.AutoGenerateColumns = false;
            gv.AllowPaging = false; ;

            //Calculate the total column of next year.
            ds.Tables[0].Columns.Add("Total");
            try
            {
                if (ds.Tables[0].Rows[0][0] != DBNull.Value)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        float tmp = 0;
                        for (int count2 = 3; count2 < ds.Tables[0].Columns.Count - 1; count2 += 2)
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
                Response.Redirect("~/Assistant/AssistantError.aspx");
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
                }
                bf.ReadOnly = true;
                gv.Columns.Add(bf);
            }

            gv.Caption = "Total" + nextyear.Substring(2, 2) + "(" + year.Substring(2, 2) + ")";
            gv.CaptionAlign = TableCaptionAlign.Top;
            gv.AllowSorting = true;
            gv.DataSource = ds.Tables[0];
            gv.DataBind();
        }
        else
            gv.Visible = false;
    }

    protected DataSet getBookingDataTotalNextYearVSThis(DataSet dsPro, string segmentID, string str_salesOrgID)
    {
        string str_salesOrgabbr = getAbbrBySalesOrg(ddlist_salesOrg.SelectedItem.Value);


        if (dsPro != null)
        {
            DataTable dt = dsPro.Tables[0];
            string sqlstr = null;
            if (string.Equals(this.hidCurrencyFlag.Value, "0"))
            {
                sqlstr = "SELECT ('" + str_salesOrgabbr + "/'+[Operation].AbbrL) AS Operation";
                string temp = "";
                for (int count = 0; count < dt.Rows.Count; count++)
                {
                    temp += " ,SUM(CASE WHEN ProductID = " + dt.Rows[count][0].ToString();
                    if (date.JudgeFirstMonth(month))
                        temp += " AND YEAR(TimeFlag)='" + preyear + "' AND MONTH(TimeFlag)='" + premonth + "'"
                              + " AND BookingY = '" + year.Substring(2, 2)
                              + "' THEN ROUND(Amount,0) ELSE 0 END) AS '"
                              + dt.Rows[count][1].ToString() + "'";
                    else
                        temp += " AND YEAR(TimeFlag)='" + year + "' AND MONTH(TimeFlag)='" + premonth + "'"
                              + " AND BookingY = '" + year.Substring(2, 2)
                              + "' THEN ROUND(Amount,0) ELSE 0 END) AS '"
                              + dt.Rows[count][1].ToString() + "'";

                }
                temp += " FROM Bookings INNER JOIN Operation ON Bookings.OperationID = Operation.ID"
                      + " WHERE SegmentID='" + segmentID + "'"
                      + " AND SalesOrgID = " + str_salesOrgID
                      + " AND Operation.Deleted=0 "
                      + " GROUP BY Operation.AbbrL";

                sqlstr += temp;
            }
            else if (string.Equals(this.hidCurrencyFlag.Value, "1"))
            {
                StringBuilder selSql = new StringBuilder();
                selSql.AppendLine(" SELECT ");
                selSql.AppendLine("   ('" + str_salesOrgabbr + "/'+[Operation].AbbrL) AS Operation ");
                for (int count = 0; count < dt.Rows.Count; count++)
                {
                    selSql.AppendLine("   ,SUM(CASE WHEN ProductID=" + dt.Rows[count][0].ToString());
                    if (date.JudgeFirstMonth(month))
                    {
                        selSql.AppendLine("                  AND YEAR([Bookings].TimeFlag)=" + preyear);
                        selSql.AppendLine("                  AND MONTH([Bookings].TimeFlag)=" + premonth);
                        selSql.AppendLine("                  AND YEAR([Currency_Exchange].TimeFlag)=" + preyear);
                        selSql.AppendLine("                  AND MONTH([Currency_Exchange].TimeFlag)=" + premonth);
                        selSql.AppendLine("                  AND BookingY='" + year.Substring(2, 2) + "' ");
                        selSql.AppendLine("             THEN (CASE WHEN DeliverY='YTD' ");
                        selSql.AppendLine("                        THEN ROUND([Bookings].Amount*[Currency_Exchange].Rate1,0) ");
                        selSql.AppendLine("                        ELSE ROUND([Bookings].Amount*[Currency_Exchange].Rate2,0) ");
                        selSql.AppendLine("                   END) ");
                        selSql.AppendLine("             ELSE 0 ");
                        selSql.AppendLine("        END) AS '" + dt.Rows[count][1].ToString() + "' ");
                    }
                    else
                    {
                        selSql.AppendLine("                  AND YEAR([Bookings].TimeFlag)=" + year);
                        selSql.AppendLine("                  AND MONTH([Bookings].TimeFlag)=" + premonth);
                        selSql.AppendLine("                  AND YEAR([Currency_Exchange].TimeFlag)=" + year);
                        selSql.AppendLine("                  AND MONTH([Currency_Exchange].TimeFlag)=" + premonth);
                        selSql.AppendLine("                  AND BookingY='" + year.Substring(2, 2) + "' ");
                        selSql.AppendLine("             THEN (CASE WHEN DeliverY='YTD' ");
                        selSql.AppendLine("                        THEN ROUND([Bookings].Amount*[Currency_Exchange].Rate1,0) ");
                        selSql.AppendLine("                        ELSE ROUND([Bookings].Amount*[Currency_Exchange].Rate2,0) ");
                        selSql.AppendLine("                   END) ");
                        selSql.AppendLine("             ELSE 0 ");
                        selSql.AppendLine("        END) AS '" + dt.Rows[count][1].ToString() + "' ");
                    }
                }
                selSql.AppendLine(" FROM ");
                selSql.AppendLine("   Bookings ");
                selSql.AppendLine("   INNER JOIN Operation ON Bookings.OperationID=Operation.ID ");
                selSql.AppendLine("   INNER JOIN [SalesOrg] ON [Bookings].SalesOrgID=[SalesOrg].ID ");
                selSql.AppendLine("   INNER JOIN [Currency_Exchange] ON [Currency_Exchange].CurrencyID=[SalesOrg].CurrencyID ");
                selSql.AppendLine(" WHERE ");
                selSql.AppendLine("   SegmentID=" + segmentID);
                selSql.AppendLine("   AND SalesOrgID=" + str_salesOrgID);
                selSql.AppendLine("   AND Operation.Deleted=0 ");
                selSql.AppendLine("   AND SalesOrg.Deleted=0 ");
                selSql.AppendLine("   AND Currency_Exchange.Deleted=0 ");
                selSql.AppendLine(" GROUP BY ");
                selSql.AppendLine("   Operation.AbbrL ");
                sqlstr = selSql.ToString();
            }
            DataSet ds = helper.GetDataSet(sqlstr);

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                return ds;
            else
                return null;
        }
        else
            return null;
    }

    protected void bindDataTotalNextVSThisByDate(GridView gv)
    {
        DataSet ds = getBookingDataTotalNextYearVSThis(getProductBySegment(getSegmentID()), getSegmentID(), ddlist_salesOrg.SelectedItem.Value);
        if (ds != null)
        {
            gv.Visible = true;
            gv.Width = Unit.Pixel(700);
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
                Response.Redirect("~/Assistant/AssistantError.aspx");
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
                    drSum[0] = "TTL/" + getAbbrBySalesOrg(ddlist_salesOrg.SelectedItem.Value);
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

    /* One page to another page */

    protected void lbtn_grSales_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/Assistant/AssistantGrossSales.aspx?SegmentID=" + getSegmentID());
    }

    protected void lbtn_grBKG_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/Assistant/AssistantGrossBookings.aspx?SegmentID=" + getSegmentID());
    }

    protected void lbtn_opSales_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/Assistant/AssistantOperationSales.aspx?SegmentID=" + getSegmentID());
    }

    protected void lbtn_opBKG_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/Assistant/AssistantOperationalBookings.aspx?SegmentID=" + getSegmentID());
    }

    protected void lbtn_SO_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/Assistant/AssistantBookingBySalesOrg.aspx?SegmentID=" + getSegmentID());
    }

    #region
    public override void VerifyRenderingInServerForm(Control control)
    {
        // Confirms that an HtmlForm control is rendered for
    }

    protected void btn_export_Click(object sender, EventArgs e)
    {
        HtmlTableRow tr5 = new HtmlTableRow();
        div_export.Rows.Insert(0,tr5);
        HtmlTableCell tc5 = new HtmlTableCell();
        tc5.InnerText = label_show.Text;
        tr5.Controls.Add(tc5);

        labelLocal.Style.Add(HtmlTextWriterStyle.BackgroundColor, "LawnGreen");
        labelLocal.Style.Add(HtmlTextWriterStyle.BorderStyle, "solid");
        labelLocal.Style.Add(HtmlTextWriterStyle.BorderWidth, "1");
        labelLocal.Style.Add(HtmlTextWriterStyle.FontWeight, "bold");
        if (this.hidCurrencyFlag.Value == "0")
        {
            labelLocal.InnerText = "BookingsBySalesOrganization(" + getSegmentDec(getSegmentID()) + " - " + ddlist_salesOrg.SelectedItem.Text + ")" + " Values in " + label_currency.Text.Substring(1);
        }
        else
        {
            labelLocal.InnerText = "BookingsBySalesOrganization(" + getSegmentDec(getSegmentID()) + " - " + ddlist_salesOrg.SelectedItem.Text + ")" + " Values in EURO";
        }
        bindDataSource();
        cf.ToExcel(div_export, "BookingsBySalesOrganization(" + getSegmentDec(getSegmentID()) + " - " + ddlist_salesOrg.SelectedItem.Text + ").xls");
    }
    #endregion

    protected void btn_EUR_Click(object sender, EventArgs e)
    {
        //this.btn_EUR.Enabled = false;
        this.label_currency.Text = "KEUR";
        this.hidCurrencyFlag.Value = "1";
        bindDataSource();
    }

    protected void btnLocal_Click(object sender, EventArgs e)
    {
        this.hidCurrencyFlag.Value = "0";
        bindDataSource();
    }
}
