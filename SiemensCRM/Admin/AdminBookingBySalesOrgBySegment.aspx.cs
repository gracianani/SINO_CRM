/*
 *  File Name       : AdminBookingBySalesOrgBySegment.aspx.cs
 *  
 *  Description     : 
 * 
 *  Author          : Wang Jun
 *  
 *  Modified Date   : 2010-12-21
 * 
 *  Problem         : none
 * 
 *  Version         : Release 2.0
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Office.Interop.Excel;
using System.Collections;
using System.IO;
using System.Text;

public partial class Admin_AdminBookingBySalesOrgBySegment : System.Web.UI.Page
{
    SQLHelper helper = new SQLHelper();
    LogUtility log = new LogUtility();
    SQLStatement sql = new SQLStatement();
    DisplayInfo info = new DisplayInfo();
    WebUtility web = new WebUtility();
    GetMeetingDate date = new GetMeetingDate();
    SQLBookingInterface bookingSql = new SQLBookingInterface();
    CommonFunction cf = new CommonFunction();

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
            log.WriteLog(LogUtility.LogErrorLevel.LOG_ACCESS, "AdminBookingBySalesOrg Access.");
            setDate();
            label_salesorgAbbr.Text = getAbbrBySalesOrg(getSalesOrgID());
            label_headdescription.Text = getSegmentDec(getSegmentID()) + " -New Orders in " + getMeetingDateYear() + " BY SALES ORGANIZATION";
            getCurrencyBySalesOrg(getSalesOrgID());
            bindDataSource();
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

    /* DropdownList */
    protected string getSegmentID()
    {
        return Request.QueryString["SegmentID"].ToString().Trim();
    }

    protected string getSalesOrgID()
    {
        return Request.QueryString["SalesOrgID"].ToString().Trim();
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

    protected string getAbbrBySalesOrg(string str_salesorgID)
    {
        string query_currency = "SELECT Abbr FROM [SalesOrg] WHERE ID = '" + str_salesorgID + "'";
        DataSet ds_currency = helper.GetDataSet(query_currency);

        if (ds_currency.Tables[0].Rows.Count > 0)
            return ds_currency.Tables[0].Rows[0][0].ToString().Trim();
        else
            return "";
    }

    protected void getCurrencyBySalesOrg(string str_salesorgID)
    {
        // By DingJunjie 20110706 ItemW60 Add Start
        if (string.Equals(Request.QueryString["ConvertFlag"], "true"))
        {
            this.label_currency.Text = "kEUR";
        }
        else
        {
            string str_currency = sql.getSalesOrgCurrency(Request.QueryString["SalesOrgID"]);
            if (!string.IsNullOrEmpty(str_currency.Trim()))
            {
                this.label_currency.Text = "K" + str_currency;
            }
            else
            {
                this.label_currency.Text = "Error";
            }
        }
        // By DingJunjie 20110706 ItemW60 Add End
    }

    protected void bindDataSource()
    {
        /* Data by date by product  start*/

        gv_bookingbydatebyproduct.Columns.Clear();
        gv_bookingTotalbydatebyproduct.Columns.Clear();

        bindDataByDateByProduct(gv_bookingbydatebyproduct, year.Substring(2, 2), "YTD");
        bindDataTotalByDateByProduct(gv_bookingTotalbydatebyproduct, year.Substring(2, 2), "YTD");

        gv_booking1bydatebyproduct.Columns.Clear();
        gv_booking1Totalbydatebyproduct.Columns.Clear();

        bindDataByDateByProduct(gv_booking1bydatebyproduct, year.Substring(2, 2), year.Substring(2, 2));
        bindDataTotalByDateByProduct(gv_booking1Totalbydatebyproduct, year.Substring(2, 2), year.Substring(2, 2));

        gv_booking2bydatebyproduct.Columns.Clear();
        gv_booking2Totalbydatebyproduct.Columns.Clear();

        bindDataByDateByProduct(gv_booking2bydatebyproduct, year.Substring(2, 2), nextyear.Substring(2, 2));
        bindDataTotalByDateByProduct(gv_booking2Totalbydatebyproduct, year.Substring(2, 2), nextyear.Substring(2, 2));

        gv_booking3bydatebyproduct.Columns.Clear();
        gv_booking3Totalbydatebyproduct.Columns.Clear();

        bindDataByDateByProduct(gv_booking3bydatebyproduct, nextyear.Substring(2, 2), nextyear.Substring(2, 2));
        bindDataTotalByDateByProduct(gv_booking3Totalbydatebyproduct, nextyear.Substring(2, 2), nextyear.Substring(2, 2));

        gv_booking4bydatebyproduct.Columns.Clear();
        gv_booking4Totalbydatebyproduct.Columns.Clear();

        bindDataByDateByProduct(gv_booking4bydatebyproduct, nextyear.Substring(2, 2), yearAfterNext.Substring(2, 2));
        bindDataTotalByDateByProduct(gv_booking4Totalbydatebyproduct, nextyear.Substring(2, 2), yearAfterNext.Substring(2, 2));

        /* Data by date by product  end */

        if (flag)
        {
            /* Data by product  start*/

            bindDataByProduct();

            /* Data by product  end */

            /* Data by date  start */

            gv_bookingtbydate.Columns.Clear();
            gv_bookingtTotalbydate.Columns.Clear();

            bindDataByDate(gv_bookingtbydate);
            bindDataTotalByDate(gv_bookingtTotalbydate);

            gv_bookingnextbydate.Columns.Clear();
            gv_bookingnextTotalbydate.Columns.Clear();

            bindDataNextByDate(gv_bookingnextbydate);
            bindDataNextTotalByDate(gv_bookingnextTotalbydate);

            /* Data by date  end */

            /* Comparsion */
            gv_VS.Columns.Clear();
            gv_VSTotal.Columns.Clear();

            bindDataNextVSThisByDate(gv_VS);
            bindDataTotalNextVSThisByDate(gv_VSTotal);
        }
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
        year = getMeetingDateYear();
        month = getMeetingDateMonth();
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

    protected DataSet getBookingDataByDateByProduct(DataSet dsPro, string segID, string bookingY, string deliverY, string salesOrgID)
    {
        if (dsPro != null)
        {
            string sqlstr = bookingSql.getBDDataByYearSOSql(dsPro, segID, bookingY, deliverY, salesOrgID, year, month, Request.QueryString["ConvertFlag"]);
            DataSet ds = helper.GetDataSet(sqlstr);
            if (ds.Tables[0].Rows.Count > 0)
                return ds;
        }
        return null;
    }

    protected DataSet getProductBySegment(string segmentID)
    {
        return sql.getProductInfoBySegmentID(segmentID);
    }

    protected void bindDataByDateByProduct(GridView gv, string bookingY, string deliverY)
    {
        DataSet ds_product = getProductBySegment(getSegmentID());
        DataSet ds = getBookingDataByDateByProduct(ds_product, getSegmentID(), bookingY, deliverY, getSalesOrgID());

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

    protected DataSet getBookingDataTotalByDateByProduct(DataSet dsPro, string segID, string salesOrgID, string bookingY, string deliverY)
    {
        string str_salesOrgabbr = getAbbrBySalesOrg(getSalesOrgID());
        if (dsPro != null)
        {
            string sqlstr = bookingSql.getBDDataTotleByYearSOSql(str_salesOrgabbr, dsPro, segID, bookingY, deliverY, salesOrgID, year, month, Request.QueryString["ConvertFlag"]);
            DataSet ds = helper.GetDataSet(sqlstr);

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                return ds;
        }
        return null;
    }

    protected void bindDataTotalByDateByProduct(GridView gv, string bookingY, string deliverY)
    {
        DataSet ds_product = getProductBySegment(getSegmentID());
        DataSet ds = getBookingDataTotalByDateByProduct(ds_product, getSegmentID(), getSalesOrgID(), bookingY, deliverY);

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
                    drSum[0] = "TTL/" + getAbbrBySalesOrg(getSalesOrgID());
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

    protected string getProductAbbr(string productID)
    {
        string query_productID = "SELECT Abbr FROM [Product] WHERE ID = " + productID;
        DataSet ds_productID = helper.GetDataSet(query_productID);

        if (ds_productID.Tables[0].Rows.Count > 0)
            return ds_productID.Tables[0].Rows[0][0].ToString();
        else
            return null;
    }

    protected DataSet getBookingsDataByProduct(string productID, string salesOrgID)
    {
        string sqlstr = bookingSql.getBDProdDataSOSql(salesOrgID, year, month, productID, preyear, getSegmentID(), Request.QueryString["ConvertFlag"]);
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

    protected void bindDataByProduct()
    {
        DataSet dsPro = getProductBySegment(getSegmentID());

        DataSet[] dsBookingsByProduct = new DataSet[dsPro.Tables[0].Rows.Count];
        DataSet[] dsTotalByProduct = new DataSet[dsPro.Tables[0].Rows.Count];
        GridView[] gvBookingsByProduct = new GridView[dsPro.Tables[0].Rows.Count];
        GridView[] gvTotalByProduct = new GridView[dsPro.Tables[0].Rows.Count];

        TableRow tr = new TableRow();
        table_bookingsByProduct.Rows.Add(tr);
        table_bookingsByProduct.Visible = true;

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

            dsBookingsByProduct[count] = getBookingsDataByProduct(productID, getSalesOrgID());
            if (dsBookingsByProduct[count] != null)
            {
                if (dsBookingsByProduct[count].Tables.Count > 0)
                {
                    gvBookingsByProduct[count] = showBookingsByProduct(dsBookingsByProduct[count], gvBookingsByProduct[count], productName);
                    tc.Controls.Add(gvBookingsByProduct[count]);
                }
                else
                {
                    log.WriteLog(LogUtility.LogErrorLevel.LOG_CRITICAL, string.Format("Error:ds.Tables.Count is {0}", dsPro.Tables.Count));
                    Response.Redirect("~/Admin/AdminError.aspx");
                }
            }

            dsTotalByProduct[count] = getBookingDataTotalByProduct(productID, getSalesOrgID());
            if (dsTotalByProduct[count] != null)
            {
                if (dsTotalByProduct[count].Tables.Count > 0)
                {
                    gvTotalByProduct[count] = showTotalByProduct(dsTotalByProduct[count], gvTotalByProduct[count], productName);
                    tc.Controls.Add(gvTotalByProduct[count]);
                }
                else
                {
                    log.WriteLog(LogUtility.LogErrorLevel.LOG_CRITICAL, string.Format("Error:ds.Tables.Count is {0}", dsPro.Tables.Count));
                    Response.Redirect("~/Admin/AdminError.aspx");
                }
            }

            tr.Controls.Add(tc);
        }
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
        string str_salesOrgabbr = getAbbrBySalesOrg(getSalesOrgID());
        string sqlstr = bookingSql.getBDProdDataTotleSOSql(str_salesOrgabbr, salesOrgID, year, month, productID, preyear, getSegmentID(), Request.QueryString["ConvertFlag"]);
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
                    drSum[0] = "TTL/" + getAbbrBySalesOrg(getSalesOrgID());
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
        string sqlstr = bookingSql.getBDDataByYearSOSql(salesOrgID, year, month, preyear, segmentID, Request.QueryString["ConvertFlag"]);

        DataSet ds = helper.GetDataSet(sqlstr);

        if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            return ds;
        else
            return null;
    }

    protected void bindDataByDate(GridView gv)
    {
        DataSet ds = getBookingsDataThisyear(getSegmentID(), getSalesOrgID());
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
        string str_salesOrgabbr = getAbbrBySalesOrg(getSalesOrgID());
        string sqlstr = bookingSql.getBDDataTotleByYearSOSql(str_salesOrgabbr, salesOrgID, year, month, preyear, segmentID, Request.QueryString["ConvertFlag"]);

        DataSet ds = helper.GetDataSet(sqlstr);

        if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            return ds;
        else
            return null;
    }

    protected void bindDataTotalByDate(GridView gv)
    {
        DataSet ds = getBookingDataTotalThisYear(getSegmentID(), getSalesOrgID());
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
                    drSum[0] = "TTL/" + getAbbrBySalesOrg(getSalesOrgID());
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

        string sqlstr = bookingSql.getBDDataByNextYearSOSql(dsPro, segmentID, salesOrgID, year, month, nextyear, Request.QueryString["ConvertFlag"]);
        DataSet ds = helper.GetDataSet(sqlstr);

        if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            return ds;
        else
            return null;
    }

    protected void bindDataNextByDate(GridView gv)
    {
        DataSet ds = getBookingsDataNextYear(getProductBySegment(getSegmentID()), getSegmentID(), getSalesOrgID());
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
        string str_salesOrgabbr = getAbbrBySalesOrg(getSalesOrgID());

        string sqlstr = bookingSql.getBDDataTotleByNextYearSOSql(str_salesOrgabbr, dsPro, segmentID, salesOrgID, year, month, nextyear, Request.QueryString["ConvertFlag"]);

        DataSet ds = helper.GetDataSet(sqlstr);

        if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            return ds;
        else
            return null;
    }

    protected void bindDataNextTotalByDate(GridView gv)
    {
        DataSet ds = getBookingDataTotalNextYear(getProductBySegment(getSegmentID()), getSegmentID(), getSalesOrgID());
        if (ds.Tables[0].Rows.Count > 0)
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
                    drSum[0] = "TTL/" + getAbbrBySalesOrg(getSalesOrgID());
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
            string sqlstr = bookingSql.getBDDataByYearVSNextYearSOSql(dsPro, segmentID, str_salesOrgID, year, month, preyear, premonth, Request.QueryString["ConvertFlag"]);
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
        DataSet ds = getBookingsDataNextYearVSThis(getProductBySegment(getSegmentID()), getSegmentID(), getSalesOrgID());
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
        string str_salesOrgabbr = getAbbrBySalesOrg(getSalesOrgID());
        if (dsPro != null)
        {
            string sqlstr = bookingSql.getBDDataTotleByYearVSNextYearSOSql(str_salesOrgabbr, dsPro, segmentID, str_salesOrgID, year, month, preyear, premonth, Request.QueryString["ConvertFlag"]);

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
        DataSet ds = getBookingDataTotalNextYearVSThis(getProductBySegment(getSegmentID()), getSegmentID(), getSalesOrgID());
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
                    drSum[0] = "TTL/" + getAbbrBySalesOrg(getSalesOrgID());
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

    /// <summary>
    /// Get Abbr By Segment
    /// </summary>
    /// <returns>Abbr</returns>
    protected string getAbbrSegment()
    {
        string selSql = "SELECT Abbr FROM Segment WHERE ID=" + getSegmentID();
        object abbr = helper.ExecuteScalar(CommandType.Text, selSql, null);
        return Convert.ToString(abbr);
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

        int rowIndex = 1;
        int colIndex = 1;
        int startRIndex = 1;
        int startCIndex = 1;
        // Year-YTD
        setExpData(sheet, this.gv_bookingbydatebyproduct, ref rowIndex, ref colIndex, 0, true);
        colIndex = startCIndex;
        setExpData(sheet, this.gv_bookingTotalbydatebyproduct, ref rowIndex, ref colIndex, 1, false);
        // Year-Year
        rowIndex = startRIndex;
        colIndex++;
        startCIndex = colIndex;
        setExpData(sheet, this.gv_booking1bydatebyproduct, ref rowIndex, ref colIndex, 0, true);
        colIndex = startCIndex;
        setExpData(sheet, this.gv_booking1Totalbydatebyproduct, ref rowIndex, ref colIndex, 1, false);
        // Year-NextYear
        rowIndex = startRIndex;
        colIndex++;
        startCIndex = colIndex;
        setExpData(sheet, this.gv_booking2bydatebyproduct, ref rowIndex, ref colIndex, 0, true);
        colIndex = startCIndex;
        setExpData(sheet, this.gv_booking2Totalbydatebyproduct, ref rowIndex, ref colIndex, 1, false);
        // NextYear-NextYear
        rowIndex = startRIndex;
        colIndex++;
        startCIndex = colIndex;
        setExpData(sheet, this.gv_booking3bydatebyproduct, ref rowIndex, ref colIndex, 0, true);
        colIndex = startCIndex;
        setExpData(sheet, this.gv_booking3Totalbydatebyproduct, ref rowIndex, ref colIndex, 1, false);
        // NextYear-AfterNextYear
        rowIndex = startRIndex;
        colIndex++;
        startCIndex = colIndex;
        setExpData(sheet, this.gv_booking4bydatebyproduct, ref rowIndex, ref colIndex, 0, true);
        colIndex = startCIndex;
        setExpData(sheet, this.gv_booking4Totalbydatebyproduct, ref rowIndex, ref colIndex, 1, false);
        // PerPorduct
        ArrayList gvList = new ArrayList();
        ComputeSubControls(this.table_bookingsByProduct, typeof(GridView), gvList);
        for (int i = 0; i < gvList.Count; i++)
        {
            if ((i + 1) % 2 == 1)
            {
                rowIndex = startRIndex;
                colIndex++;
                startCIndex = colIndex;
                setExpData(sheet, (GridView)gvList[i], ref rowIndex, ref colIndex, 0, true);
            }
            else
            {
                colIndex = startCIndex;
                setExpData(sheet, (GridView)gvList[i], ref rowIndex, ref colIndex, 0, false);
            }
        }
        // Totle Year
        rowIndex = startRIndex;
        colIndex++;
        startCIndex = colIndex;
        setExpData(sheet, this.gv_bookingtbydate, ref rowIndex, ref colIndex, 0, true);
        colIndex = startCIndex;
        setExpData(sheet, this.gv_bookingtTotalbydate, ref rowIndex, ref colIndex, 0, false);
        // Totle NextYear
        rowIndex = startRIndex;
        colIndex++;
        startCIndex = colIndex;
        setExpData(sheet, this.gv_bookingnextbydate, ref rowIndex, ref colIndex, 0, true);
        colIndex = startCIndex;
        setExpData(sheet, this.gv_bookingnextTotalbydate, ref rowIndex, ref colIndex, 0, false);
        // Totle Year VS NextYear
        rowIndex = startRIndex;
        colIndex++;
        startCIndex = colIndex;
        setExpData(sheet, this.gv_VS, ref rowIndex, ref colIndex, 0, true);
        colIndex = startCIndex;
        setExpData(sheet, this.gv_VSTotal, ref rowIndex, ref colIndex, 1, false);
        // Set All Text Warp False And Auto Fit
        sheet.Cells.WrapText = false;
        sheet.Cells.EntireColumn.AutoFit();
        // Export
        string path = Server.MapPath("~") + @"ExcelReport\" + DateTime.Now.ToString("yyyyMMdd") + @"\";
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
        Response.AddHeader("Content-Disposition", "attachment; filename=" + Server.UrlEncode("SO(" + getAbbrBySalesOrg(getSalesOrgID()) + "-" + getAbbrSegment().Trim() + ").xlsx"));
        Response.AddHeader("Content-Length", file.Length.ToString());
        Response.ContentType = "application/ms-excel";
        Response.WriteFile(filePath);
        Response.End();
    }

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
    /// <param name="colIndex">Column Index</param>
    /// <param name="mergeNum">Merge Number</param>
    /// <param name="hasTitleFlag">Has Title Or Not</param>
    private void setExpData(Worksheet sheet, GridView gv, ref int rowIndex, ref int colIndex, int mergeNum, bool hasTitleFlag)
    {
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
        for (int i = 0; i < gv.Columns.Count; i++)
        {
            if (gv.Columns[i].Visible)
            {
                sheet.Cells[rowIndex, colIndex] = gv.Columns[i].HeaderText;
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
        }
        rowIndex++;

        if (gv.Columns.Count != 0)
        {
            if (hasTitleFlag)
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
        for (int i = 0; i < gv.Rows.Count; i++)
        {
            colIndex = indexCStart;
            for (int j = 0; j < gv.Columns.Count; j++)
            {
                if (gv.Columns[j].Visible)
                {
                    text = HttpUtility.HtmlDecode(gv.Rows[i].Cells[j].Text);
                    if (ExcelHandler.IsFloat(text))
                    {
                        sheet.Cells[rowIndex, colIndex] = Convert.ToDecimal(text);
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
            }
            rowIndex++;
        }
        #endregion
    }
}
