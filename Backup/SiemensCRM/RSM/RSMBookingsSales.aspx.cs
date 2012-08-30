/*
 *   FileName       :  RSMBookingSales.aspx.cs
 * 
 *   Description    :  Query booking data by rsm
 * 
 *   Author         :  Wang Jun
 * 
 *   Modified date  :  2010-10-25
 * 
 *   problem        : 
 * 
 *   Version        : Release (2.0)
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;

public partial class RSMBookingsSalesReadOnly : System.Web.UI.Page
{
    SQLHelper helper = new SQLHelper();
    LogUtility log = new LogUtility();
    DisplayInfo info = new DisplayInfo();
    WebUtility web = new WebUtility();
    SQLStatement sql = new SQLStatement();
    GetMeetingDate meeting = new GetMeetingDate();
    CommonFunction cf = new CommonFunction();
    SQLBookingInterface sqlBooking = new SQLBookingInterface();

    private static bool NullData;
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
        if (getRoleID(getRole()) != "4")
            Response.Redirect("!/AccessDenied.aspx");

        if (!IsPostBack)
        {
            panel_head.Visible = false;
            meeting.setDate();
            preyear = meeting.getpreyear();
            year = meeting.getyear();
            nextyear = meeting.getnextyear();
            yearAfterNext = meeting.getyearAfterNext();
            month = meeting.getmonth();
            premonth = meeting.getPreMonth(month);

            ddlist_salesOrg.Items.Clear();
            bindSalesOrgInfo(getRSMID());

            if (ddlist_salesOrg.SelectedItem.Value.Trim() != "-1")
            {
                ddlist_segment.Items.Clear();
                // update by zy 20110126 start
                //bind(getSegmentInfo(ddlist_salesOrg.SelectedItem.Value.Trim()));
                bind(getSegmentInfo(ddlist_salesOrg.SelectedItem.Value.Trim()), 1);
                bind(getCountryInfo(), 2);
                // update by zy 20110126 end
            }
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

    protected DataSet getRSMInfo(string str_rsmID)
    {
        string sql = "SELECT Alias, Abbr  FROM [User] WHERE UserID = " + str_rsmID + " AND Deleted = 0";
        DataSet ds = helper.GetDataSet(sql);
        if (ds.Tables[0].Rows.Count == 1)
            return ds;
        else
        {
            return null;
        }
    }

    private void bindSalesOrgInfo(string str_RSMID)
    {
        string query_salesOrg = "SELECT [SalesOrg].Name, [SalesOrg].Abbr, [SalesOrg].ID FROM [SalesOrg] INNER JOIN [SalesOrg_User]"
                              + " ON [SalesOrg].ID = [SalesOrg_User].SalesOrgID"
                              + " WHERE [SalesOrg].Deleted = 0 AND [SalesOrg_User].Deleted = 0 AND [SalesOrg_User].UserID = " + str_RSMID
                              + " ORDER BY [SalesOrg].Name ASC";
        DataSet ds_salesOrg = helper.GetDataSet(query_salesOrg);

        if (ds_salesOrg.Tables[0].Rows.Count > 0)
        {
            DataTable dt = ds_salesOrg.Tables[0];
            int count = dt.Rows.Count;
            int index = 0;
            while (index < count)
            {
                ListItem li = new ListItem((dt.Rows[index][0].ToString() + "(" + dt.Rows[index][1].ToString().Trim() + ")"), dt.Rows[index][2].ToString());
                ddlist_salesOrg.Items.Add(li);
                index++;
            }
            ddlist_salesOrg.SelectedIndex = 0;
            ddlist_salesOrg.Enabled = true;
            btn_search.Enabled = true;
        }
        else
        {
            ListItem li = new ListItem("", "-1");
            ddlist_salesOrg.Items.Add(li);
            ddlist_salesOrg.Enabled = false;
            btn_search.Enabled = false;
        }
    }

    protected void ddlist_salesOrg_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddlist_salesOrg.SelectedItem.Value.Trim() != "-1")
        {
            ddlist_segment.Items.Clear();
            // update by zy 20110126 start
            //bind(getSegmentInfo(ddlist_salesOrg.SelectedItem.Value.Trim()));
            bind(getSegmentInfo(ddlist_salesOrg.SelectedItem.Value.Trim()), 2);
            bind(getCountryInfo(), 2);
            // update by zy 20110126 end
        }
    }
    
    protected DataSet getSegmentInfo(string str_salesOrgID)
    {
        string query_segment = "SELECT DISTINCT [Segment].ID,[Segment].Abbr"
                            + " FROM [SalesOrg_Segment] INNER JOIN [Segment] "
                            + " ON [SalesOrg_Segment].SegmentID  = [Segment].ID "
                            + " INNER JOIN [SalesOrg] "
                            + " ON [SalesOrg].ID = [SalesOrg_Segment].SalesOrgID "
                            + " WHERE [SalesOrg].Deleted = 0 AND [Segment].Deleted = 0 AND [SalesOrg].ID = '"
                            + str_salesOrgID + "'"
                            + " ORDER BY [Segment].Abbr ASC";
        DataSet ds_segment = helper.GetDataSet(query_segment);

        if ((ds_segment.Tables.Count > 0) && (ds_segment.Tables[0].Rows.Count > 0))
        {
            return ds_segment;
        }
        else
        {
            return null;
        }
    }
    // update by zy 20110126 start
    //protected void bind(DataSet ds)
    protected void bind(DataSet ds, int sel) // 1:segment 2:country
    // update by zy 20110126 end
    {
        if (ds != null)
        {
            DataTable dt = ds.Tables[0];
            int count = dt.Rows.Count;
            int index = 0;
            // add by zy 20110126 start
            if (sel == 2)
            {
                ListItem item = new ListItem("", "");
                ddlist_country.Items.Insert(0, item);
            }
            // add by zy 20110126 end

            while (index < count)
            {
                // update by zy 20110126 start
                //ListItem li = new ListItem(dt.Rows[index][1].ToString().Trim(), dt.Rows[index][0].ToString().Trim());
                //ddlist_segment.Items.Add(li);
                if (sel == 1)
                {
                    ListItem li = new ListItem(dt.Rows[index][1].ToString().Trim(), dt.Rows[index][0].ToString().Trim());
                    ddlist_segment.Items.Add(li);
                }
                else if (sel == 2)
                {
                    ListItem li = new ListItem(dt.Rows[index][1].ToString().Trim(), dt.Rows[index][0].ToString().Trim());
                    ddlist_country.Items.Add(li);
                }
                // update by zy 20110126 end
                index++;
            }
            // update by zy 20110126 start
            //ddlist_segment.SelectedIndex = 0;
            //ddlist_segment.Enabled = true;
            //btn_search.Enabled = true;
            if (sel == 1)
            {
                ddlist_segment.SelectedIndex = 0;
                ddlist_segment.Enabled = true;
                btn_search.Enabled = true;
            }
            else if (sel == 2)
            {
                ddlist_country.SelectedIndex = 0;
                ddlist_country.Enabled = true;                
            }
            // update by zy 20110126 end
        }
        else
        {
            // update by zy 20110126 start
            //ListItem li = new ListItem("", "-1");
            //ddlist_segment.Items.Add(li);
            //ddlist_segment.Enabled = false;
            //btn_search.Enabled = false;
            if (sel == 1)
            {
                ListItem li = new ListItem("", "-1");
                ddlist_segment.Items.Add(li);
                ddlist_segment.Enabled = false;
                btn_search.Enabled = false;
            }
            else if (sel == 2)
            {
                ddlist_country.Enabled = false;
                ddlist_country.Items.Add(new ListItem("", "-1"));
            }
            // update by zy 20110126 end
        }
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

    /*protected string getCurrencyBySalesOrg(string salesOrgID)//?
    {
        string cur = "Error";
        string query_currency = " SELECT [Currency].Name FROM [SalesOrg] INNER JOIN [Currency]"
                              + " ON [SalesOrg].CurrencyID = [Currency].ID"
                              + " WHERE [SalesOrg].ID = '" + salesOrgID + "' AND [SalesOrg].Deleted = 0"
                              + " AND [Currency].Deleted = 0";
        DataSet ds_currency = helper.GetDataSet(query_currency);
        if (ds_currency.Tables.Count > 0 && ds_currency.Tables[0].Rows.Count > 0)
        {
            cur = ds_currency.Tables[0].Rows[0][0].ToString();
            lbl_currency.Text = "K" + ds_currency.Tables[0].Rows[0][0].ToString();
        }
        else
            lbl_currency.Text = "Error";
        return cur;
    }*/

    protected void btn_search_Click(object sender, EventArgs e)
    {
        panel_head.Visible = true;
        string str_segment = ddlist_segment.SelectedItem.Text.Trim();
        string str_salesOrgID = ddlist_salesOrg.SelectedItem.Value.Trim();
        label_RSMAbbr.Text = getRSMInfo(getRSMID().Trim()).Tables[0].Rows[0][0].ToString().Trim();

        if (str_segment != null)
        {
            label_headdescription.Text = str_segment + "-New Orders For " + year + " And " + preyear;
        }
        else
            label_headdescription.Text = "Please add segments to" + label_RSMAbbr.Text + ".";

        bindStatus(getRSMID(), ddlist_segment.SelectedItem.Value.Trim());

        helper.ExecuteNonQuery(CommandType.Text, "DELETE FROM [Bookings] WHERE Amount = 0", null);
        bindDataSource();

        if (cf.GetLockUser(getRSMID()))
            lbtn_editRSM.Enabled = false;
        else
            lbtn_editRSM.Enabled = true;

        //wj 20110120
        DataSet ds_currency = getCurrencyBySalesOrgID(str_salesOrgID);
        if (ds_currency != null)
            lbl_currency.Text = "K" + ds_currency.Tables[0].Rows[0][0].ToString().Trim();
        else
            lbl_currency.Text = "No Currency";
        btn_local.Text = lbl_currency.Text;
        //end
        convert_flag = false;
        //getCurrencyBySalesOrg(getRSMID());
    }

    protected void btn_local_Click(object sender, EventArgs e)
    {
        convert_flag = false;

        bindStatus(getRSMID(), ddlist_segment.SelectedItem.Value.Trim());
        helper.ExecuteNonQuery(CommandType.Text, "DELETE FROM [Bookings] WHERE Amount = 0", null);
        bindDataSource();

        btn_local.Enabled = false;
        btn_EUR.Enabled = true;
    }
    protected void btn_EUR_Click(object sender, EventArgs e)
    {
        convert_flag = true;

        bindStatus(getRSMID(), ddlist_segment.SelectedItem.Value.Trim());
        helper.ExecuteNonQuery(CommandType.Text, "DELETE FROM [Bookings] WHERE Amount = 0", null);
        bindDataSource();

        btn_local.Enabled = true;
        btn_EUR.Enabled = false;
    }

    protected void bindDataSource()
    {
        /* Data by date by product  start*/

        gv_bookingbydatebyproduct.Columns.Clear();
        gv_bookingTotalbydatebyproduct.Columns.Clear();

        bindDataByDateByProduct(gv_bookingbydatebyproduct, year.Substring(2, 2), "YTD", 0);
        bindDataTotalByDateByProduct(gv_bookingTotalbydatebyproduct, year.Substring(2, 2), "YTD");
        gv_bookingbydatebyproduct.Visible = true;
        gv_bookingTotalbydatebyproduct.Visible = true;

        gv_booking1bydatebyproduct.Columns.Clear();
        gv_booking1Totalbydatebyproduct.Columns.Clear();

        bindDataByDateByProduct(gv_booking1bydatebyproduct, year.Substring(2, 2), year.Substring(2, 2), 1);
        bindDataTotalByDateByProduct(gv_booking1Totalbydatebyproduct, year.Substring(2, 2), year.Substring(2, 2));

        gv_booking2bydatebyproduct.Columns.Clear();
        gv_booking2Totalbydatebyproduct.Columns.Clear();

        bindDataByDateByProduct(gv_booking2bydatebyproduct, year.Substring(2, 2), nextyear.Substring(2, 2), 2);
        bindDataTotalByDateByProduct(gv_booking2Totalbydatebyproduct, year.Substring(2, 2), nextyear.Substring(2, 2));
        gv_booking1bydatebyproduct.Visible = true;
        gv_booking1Totalbydatebyproduct.Visible = true;
        gv_booking2bydatebyproduct.Visible = true;
        gv_booking2Totalbydatebyproduct.Visible = true;

        gv_booking3bydatebyproduct.Columns.Clear();
        gv_booking3Totalbydatebyproduct.Columns.Clear();

        bindDataByDateByProduct(gv_booking3bydatebyproduct, nextyear.Substring(2, 2), nextyear.Substring(2, 2), 3);
        bindDataTotalByDateByProduct(gv_booking3Totalbydatebyproduct, nextyear.Substring(2, 2), nextyear.Substring(2, 2));

        gv_booking4bydatebyproduct.Columns.Clear();
        gv_booking4Totalbydatebyproduct.Columns.Clear();

        bindDataByDateByProduct(gv_booking4bydatebyproduct, nextyear.Substring(2, 2), yearAfterNext.Substring(2, 2), 4);
        bindDataTotalByDateByProduct(gv_booking4Totalbydatebyproduct, nextyear.Substring(2, 2), yearAfterNext.Substring(2, 2));
        gv_booking3bydatebyproduct.Visible = true;
        gv_booking3Totalbydatebyproduct.Visible = true;
        gv_booking4bydatebyproduct.Visible = true;
        gv_booking4Totalbydatebyproduct.Visible = true;

        if (!NullData)
        {
            /* Data by date by product  end */

            /* Data by product  start*/

            bindDataByProduct();

            /* Data by product  end */

            /* Data by date  start */

            gv_bookingtbydate.Columns.Clear();
            gv_bookingtTotalbydate.Columns.Clear();

            bindDataByDate(gv_bookingtbydate);
            bindDataTotalByDate(gv_bookingtTotalbydate);
            gv_bookingtbydate.Visible = true;
            gv_bookingtTotalbydate.Visible = true;
            gv_bookingnextbydate.Columns.Clear();
            gv_bookingnextTotalbydate.Columns.Clear();

            bindDataNextByDate(gv_bookingnextbydate);
            bindDataNextTotalByDate(gv_bookingnextTotalbydate);
            gv_bookingnextbydate.Visible = true;
            gv_bookingnextTotalbydate.Visible = true;
            /* Data by date  end */

            /* Comparsion */
            gv_VS.Columns.Clear();
            gv_VSTotal.Columns.Clear();

            bindDataVSPre(gv_VS);
            bindDataTotalVSPre(gv_VSTotal);
            gv_VS.Visible = true;
            gv_VSTotal.Visible = true;
        }
        /* Open and Close */
    }

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
        // update by zy 20110126 start
        //string countryID = "-1";
        string countryID = ddlist_country.SelectedItem.Value.Trim();
        // update by zy 20110126 end

        string sqlstr = sqlBooking.getBookingData(salesorgID, segmentID, RSMID, bookingY, deliverY, dsPro, year, month, countryID, convert_flag, getCountrySQL());
        if (sqlstr != "")
        {
            DataSet ds = helper.GetDataSet(sqlstr);

            if (ds.Tables.Count > 0)
                return ds;
            else
                return null;
        }
        else
            return null;
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
        NullData = false;
        DataSet ds = getBookingDataByDateByProduct(ds_product, ddlist_segment.SelectedItem.Value, getRSMID(), bookingY, deliverY);

        if (ds != null)
        {
            if (ds.Tables[0].Rows.Count == 0)
            {
                sql.getNullDataSet(ds);
            }
            gv.Visible = true;
            gv.Width = Unit.Pixel(800);
            gv.AutoGenerateColumns = false;
            gv.AllowPaging = false;

            for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
            {
                BoundField bf = new BoundField();

                bf.DataField = ds.Tables[0].Columns[i].ColumnName.ToString();
                bf.HeaderText = ds.Tables[0].Columns[i].Caption.ToString();
                bf.ReadOnly = true;
                bf.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
                bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;

                // update by zy 20110128 start
                //if (i % 2 == 1 && i != 7 && i != 5)
                if (i % 2 == 1 && i != 7 && i != 9 && i != 5)
                // update by zy 20110128 end
                {
                    bf.HeaderText = null;
                    bf.ControlStyle.Width = 15;
                    bf.ItemStyle.Width = 15;
                }
                // update by zy 20110128 start
                //if (i % 2 == 0 && i != 6 && i != 4)
                if (i % 2 == 0 && i != 6 && i != 8 && i != 4)
                // update by zy 20110128 end
                {
                    bf.ItemStyle.HorizontalAlign = HorizontalAlign.Right;
                    bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Right;
                    bf.ControlStyle.Width = 50;
                    bf.ItemStyle.Width = 50;
                }

                gv.Columns.Add(bf);
            }

            if (deliverY == "YTD")
                gv.Caption = bookingY + deliverY + "  " + fiscalStart + "," + preyear + " to " + meeting.getMonth(month) + meeting.getDay() + "," + year;
            else
                gv.Caption = bookingY + " for " + deliverY + "  " + fiscalStart + "," + year + " to " + fiscalEnd + "," + bookingY + " for " + deliverY + " delivery";
            gv.CaptionAlign = TableCaptionAlign.Top;
            gv.AllowSorting = true;
            gv.DataSource = ds.Tables[0];

            gv.DataBind();
            gv.Columns[0].Visible = false;
            gv.Columns[1].Visible = false;
            gv.Columns[2].Visible = false;
            gv.Columns[3].Visible = false;
            // add by zy 20110128 start
            gv.Columns[4].Visible = false;
            // add by zy 20110128 end
            // add by zy 20110126 start
            for (int i = 0; i < gv.Rows.Count; i++)
            {
                addCellsAttributes(gv.Rows[i], 6, 2);
            }
            // add by zy 20110126 end
        }
        else
        {
            gv.Visible = false;
        }
    }

    public string SubStr(string sString, int nLeng)
    {
        if (sString.Length <= nLeng)
        {
            return sString;
        }
        string sNewStr = sString.Substring(0, nLeng);
        sNewStr = sNewStr + "...";
        return sNewStr;
    }

    /* GridView Booking Total Data */

    protected string getAbbrByUserID(string str_userID)
    {
        string query_RSM = "SELECT Abbr "
                            + " FROM [User]"
                            + " WHERE UserID = " + str_userID;
        DataSet ds_RSM = helper.GetDataSet(query_RSM);

        if (ds_RSM.Tables[0].Rows.Count > 0)
        {
            return ds_RSM.Tables[0].Rows[0][0].ToString().Trim();
        }
        else
        {
            return "";
        }
    }

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
        string countryID = "-1";
        string rsmAbbr = getAbbrByUserID(RSMID);

        string sqlstr = sqlBooking.getBookingDataTotal(salesorgID, dsPro, segID, rsmAbbr, RSMID, bookingY, deliverY, year, month, countryID, convert_flag);
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
        DataSet ds = getBookingDataTotalByDateByProduct(ds_product, ddlist_segment.SelectedItem.Value, getRSMID(), bookingY, deliverY);

        if (ds != null && !NullData)
        {
            gv.Visible = true;
            // update by zy 20110128 start
            //gv.Width = Unit.Pixel(800);
            gv.Width = Unit.Pixel(1016);
            // update by zy 20110128 end
            gv.AutoGenerateColumns = false;
            gv.AllowPaging = false;

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
                    drSum[0] = "TTL/" + getAbbrByUserID(getRSMID());
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
                           + " WHERE SegmentID = " + segmentID + " AND [Product].Deleted = 0 AND [Segment_Product].Deleted = 0";
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
        string countryID = "-1";
        string segmentID = ddlist_segment.SelectedItem.Value.Trim();

        string sqlstr = sqlBooking.getBookingsDataByProduct(segmentID, productID, RSMID, salesorgID, year, month, preyear, countryID, convert_flag);
        DataSet ds = helper.GetDataSet(sqlstr);

        if (ds.Tables[0].Rows.Count > 0)
            return ds;
        else
            return null;
    }

    protected GridView showBookingsByProduct(DataSet ds, GridView gv, string header)
    {
        if (ds != null && !NullData)
        {
            gv.Visible = true;
            gv.AutoGenerateColumns = false;
            gv.Width = Unit.Pixel(400);
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
                        // update by zy 20110126 start
                        //if (float.Parse(dr[3].ToString()) != 0)
                        if (float.Parse(dr[11].ToString()) != 0)
                        // update by zy 20110126 end
                        {
                            // update by zy 20110126 start
                            //tmp = (float.Parse(dr[2].ToString()) - float.Parse(dr[3].ToString())) * 100 / float.Parse(dr[3].ToString());
                            tmp = (float.Parse(dr[10].ToString()) - float.Parse(dr[11].ToString())) * 100 / float.Parse(dr[11].ToString());
                            // update by zy 20110126 end
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

                // update by zy 20110128 start
                //if (i < 8)
                if (i < 10)
                // update by zy 20110128 end
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
            // add by zy 20110128 start
            gv.Columns[4].Visible = false;
            // add by zy 20110128 end

            // add by zy 20110126 start
            for (int i = 0; i < gv.Rows.Count; i++)
            {
                addCellsAttributes(gv.Rows[i], 6, 2);
            }
            // add by zy 20110126 end
        }
        else
            gv.Visible = false;
        return gv;
    }

    protected void bindDataByProduct()
    {
        DataSet dsPro = getProductBySegment(ddlist_segment.SelectedItem.Value.ToString().Trim());

        if (dsPro != null)
        {
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

                dsBookingsByProduct[count] = getBookingsDataByProduct(productID, getRSMID());
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

                dsTotalByProduct[count] = getBookingDataTotalByProduct(productID, getRSMID());
                if (dsTotalByProduct[count] != null)
                {
                    if (dsTotalByProduct[count].Tables.Count > 0)
                    {
                        gvTotalByProduct[count] = showTotalByProduct(dsTotalByProduct[count], gvTotalByProduct[count]);
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
        string countryID = "-1";
        string rsmAbbr = getAbbrByUserID(RSMID);
        string segmentID = ddlist_segment.SelectedItem.Value.Trim();

        string sqlstr = sqlBooking.getBookingsDataTotalByProduct(segmentID, productID, rsmAbbr, RSMID, salesorgID, year, month, preyear, countryID, convert_flag);
        DataSet ds = helper.GetDataSet(sqlstr);

        if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            return ds;
        else
            return null;
    }

    protected GridView showTotalByProduct(DataSet ds, GridView gv)
    {
        if (ds != null && !NullData)
        {
            gv.Visible = true;
            gv.AutoGenerateColumns = false;
            // update by zy 20110128 start
            gv.Width = Unit.Pixel(400);
            gv.Width = Unit.Pixel(492);
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
                    drSum[0] = "TTL/" + getAbbrByUserID(getRSMID());
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

    protected DataSet getBookingsDataThisyear(string segmentID, string RSMID)
    {
        string salesorgID = ddlist_salesOrg.SelectedItem.Value.Trim();
        string countryID = "-1";

        string sqlstr = sqlBooking.getBookingsDataByThisyear(salesorgID, segmentID, RSMID, year, month, preyear, countryID, convert_flag);
        DataSet ds = helper.GetDataSet(sqlstr);

        if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            return ds;
        else
            return null;
    }

    protected void bindDataByDate(GridView gv)
    {
        DataSet ds = getBookingsDataThisyear(ddlist_segment.SelectedItem.Value.ToString().Trim(), getRSMID().ToString().Trim());
        if (ds != null && !NullData)
        {
            gv.Visible = true;
            gv.Width = Unit.Pixel(400);
            gv.AutoGenerateColumns = false;
            gv.AllowPaging = false;

            //Calculate the VAR column and Total row of next year.
            try
            {
                ds.Tables[0].Columns.Add("VAR");
                if (ds.Tables[0].Rows[0][0] != DBNull.Value)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        float tmp = 0;
                        // update by zy 20110126 start
                        //if (float.Parse(dr[3].ToString()) != 0)
                        if (float.Parse(dr[11].ToString()) != 0)
                        // update by zy 20110126 end
                        {
                            // update by zy 20110126 start
                            //tmp = (float.Parse(dr[2].ToString()) - float.Parse(dr[3].ToString())) * 100 / float.Parse(dr[3].ToString());
                            tmp = (float.Parse(dr[10].ToString()) - float.Parse(dr[11].ToString())) * 100 / float.Parse(dr[11].ToString());
                            // update by zy 20110126 start
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
                bf.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
                bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
                bf.ReadOnly = true;

                // update by zy 20110128 start
                //if (i != 4 && i != 5 && i != 6 && i != 7)
                if (i != 4 && i != 5 && i != 6 && i != 7 && i != 8)
                // update by zy 20110128 end
                {
                    bf.ItemStyle.HorizontalAlign = HorizontalAlign.Right;
                    bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Right;
                }

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
            // add by zy 20110128 start
            gv.Columns[4].Visible = false;
            // add by zy 20110128 end

            // add by zy 20110126 start
            for (int i = 0; i < gv.Rows.Count; i++)
            {
                addCellsAttributes(gv.Rows[i], 6, 2);
            }
            // add by zy 20110126 end
        }
        else
            gv.Visible = false;
    }

    /* GridView Booking Total Data By Operation This Year*/

    /// <summary>
    /// Get booking total amount this year
    /// </summary>
    /// <param name="segmentID"></param>
    /// <param name="RSMID"></param>
    /// <returns></returns>

    protected DataSet getBookingDataTotalThisYear(string segmentID, string RSMID)
    {
        string salesorgID = ddlist_salesOrg.SelectedItem.Value.Trim();
        string countryID = "-1";
        string rsmAbbr = getAbbrByUserID(RSMID);

        string sqlstr = sqlBooking.getBookingDataTotalThisYear(salesorgID, segmentID, rsmAbbr, RSMID, year, month, preyear, countryID, convert_flag);
        DataSet ds = helper.GetDataSet(sqlstr);

        if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            return ds;
        else
            return null;
    }

    protected void bindDataTotalByDate(GridView gv)
    {
        DataSet ds = getBookingDataTotalThisYear(ddlist_segment.SelectedItem.Value.ToString().Trim(), getRSMID().ToString().Trim());
        if (ds != null && !NullData)
        {
            gv.Visible = true;
            // update by zy 20110128 start
            //gv.Width = Unit.Pixel(400);
            gv.Width = Unit.Pixel(528);
            // update by zy 20110128 end
            gv.AutoGenerateColumns = false;
            gv.AllowPaging = false;

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
                bf.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
                bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
                bf.ReadOnly = true;

                if (i != 0)
                {
                    bf.ItemStyle.HorizontalAlign = HorizontalAlign.Right;
                    bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Right;
                }
                else
                {
                    bf.ItemStyle.Width = 280;
                }

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
                    drSum[0] = "TTL/" + getAbbrByUserID(getRSMID());
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
        string countryID = "-1";

        string sqlstr = sqlBooking.getBookingsDataNextYear(salesorgID, dsPro, segmentID, RSMID, year, month, nextyear, countryID, convert_flag);
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

    protected void bindDataNextByDate(GridView gv)
    {
        DataSet ds = getBookingsDataNextYear(getProductBySegment(ddlist_segment.SelectedItem.Value.ToString().Trim()), ddlist_segment.SelectedItem.Value.ToString().Trim(), getRSMID().ToString().Trim());
        if (ds != null && !NullData)
        {
            gv.Visible = true;
            gv.Width = Unit.Pixel(800);
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
                        // update by zy 20110126 start
                        //for (int count2 = 2; count2 < ds.Tables[0].Columns.Count - 1; count2 += 2)
                        for (int count2 = 10; count2 < ds.Tables[0].Columns.Count - 1; count2 += 2)
                        // update by zy 20110126 end
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

                bf.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
                bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Right;
                // update by zy 20110128 start
                //if (i == 4 || i == 5 || i == 6)
                if (i == 5 || i == 6 || i == 7 || i == 8) //SubRegion,Customer,Project,SalesChannel
                // update by zy 20110128 end
                {
                    bf.ItemStyle.Width = 150;
                    bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
                }

                // update by zy 20110128 start
                //if (i == 7)
                if (i == 9) // Country
                // update by zy 20110128 end
                {
                    bf.ItemStyle.Width = 50;
                    bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
                }

                // update by zy 20110128 start
                //if (i % 2 == 1 && i != 5 && i != 7)
                if (i % 2 == 1 && i != 5 && i != 7 && i != 9)
                // update by zy 20110128 end
                {
                    bf.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
                    bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
                    bf.HeaderText = null;
                }

                // update by zy 20110128 start
                //if (i % 2 == 0 && i != 4 && i != 6)
                if (i % 2 == 0 && i != 4 && i != 6 && i != 8)
                // update by zy 20110128 end
                {
                    bf.ItemStyle.HorizontalAlign = HorizontalAlign.Right;
                    bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Right;
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
            // add by zy 20110126 start
            for (int i = 0; i < gv.Rows.Count; i++)
            {
                addCellsAttributes(gv.Rows[i], 6, 2);
            }
            // add by zy 20110126 end
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

    protected DataSet getBookingDataTotalNextYear(DataSet dsPro, string segmentID, string RSMID)
    {
        string salesorgID = ddlist_salesOrg.SelectedItem.Value.Trim();
        string countryID = "-1";
        string rsmAbbr = getAbbrByUserID(RSMID);

        string sqlstr = sqlBooking.getBookingDataTotalByNextYear(salesorgID, dsPro, segmentID, rsmAbbr, RSMID, year, month, nextyear, countryID, convert_flag);
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
        DataSet ds = getBookingDataTotalNextYear(getProductBySegment(ddlist_segment.SelectedItem.Value.ToString().Trim()), ddlist_segment.SelectedItem.Value.ToString().Trim(), getRSMID().ToString().Trim());
        if (ds != null && !NullData)
        {
            gv.Visible = true;
            // update by zy 20110128 start
            //gv.Width = Unit.Pixel(800);
            gv.Width = Unit.Pixel(1105);
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
                    drSum[0] = "TTL/" + getAbbrByUserID(getRSMID());
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
        string countryID = "-1";

        string sqlstr = sqlBooking.getBookingsDataByThisVSPre(salesorgID, dsPro, segmentID, RSMID, year, month, preyear, premonth, countryID, convert_flag);
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

    protected void bindDataVSPre(GridView gv)
    {
        DataSet ds = getBookingsDataVSPre(getProductBySegment(ddlist_segment.SelectedItem.Value), ddlist_segment.SelectedItem.Value, getRSMID());
        if (ds != null && !NullData)
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
                        // update by zy 20110126 start
                        //for (int count2 = 2; count2 < ds.Tables[0].Columns.Count - 1; count2++)
                        for (int count2 = 10; count2 < ds.Tables[0].Columns.Count - 1; count2++)
                        // update by zy 20110126 end
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

                bf.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
                bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Right;

                bf.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
                bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Right;

                // update by zy 20110128 start
                //if (i < 8)
                if (i < 10)
                // update by zy 20110128 end
                {
                    bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
                }

                if (i > 6)
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
            // add by zy 20110128 start
            gv.Columns[4].Visible = false;
            // add by zy 20110128 end

            // add by zy 20110126 start
            for (int i = 0; i < gv.Rows.Count; i++)
            {
                addCellsAttributes(gv.Rows[i], 6, 2);
            }
            // add by zy 20110126 end
        }
        else
            gv.Visible = false;
    }

    protected DataSet getBookingDataTotalVSPre(DataSet dsPro, string segmentID, string RSMID)
    {
        string salesorgID = ddlist_salesOrg.SelectedItem.Value.Trim();
        string countryID = "-1";
        string rsmAbbr = getAbbrByUserID(RSMID);

        string sqlstr = sqlBooking.getBookingDataTotaByThislVSPre(salesorgID, dsPro, segmentID, rsmAbbr, RSMID, year, month, preyear, premonth, countryID, convert_flag);
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
        DataSet ds = getBookingDataTotalVSPre(getProductBySegment(ddlist_segment.SelectedItem.Value), ddlist_segment.SelectedItem.Value, getRSMID());
        if (ds != null && !NullData)
        {
            // update by zy 20110128 start
            //gv.Width = Unit.Pixel(700);
            gv.Width = Unit.Pixel(872);
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
                    drSum[0] = "TTL/" + getAbbrByUserID(getRSMID());
                }
                else
                {
                    drSum[i] = Sum[i].ToString();
                }
            }
            ds.Tables[0].Rows.InsertAt(drSum, 0);
            gv.DataBind();
        }
    }

    protected void lbtn_editRSM_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/BookingSalesData.aspx?SalesOrgID=" + ddlist_salesOrg.SelectedItem.Value.Trim() + "&SegmentID=" + ddlist_segment.SelectedItem.Value.Trim() + "&RSMID=" + getRSMID() + "&CountryID=-1");
    }

    #region
    public override void VerifyRenderingInServerForm(Control control)
    {
        // Confirms that an HtmlForm control is rendered for
    }

    protected void btn_export_Click(object sender, EventArgs e)
    {
        bindDataSource();
        cf.ToExcel(div_export, "RSM(" + ddlist_segment.SelectedItem.Text + "-" + getRSMInfo(getRSMID()).Tables[0].Rows[0][0].ToString() + ").xls");
    }
    #endregion

    private bool setUserStatus(string str_userID, string str_segmentID, string str_status, bool flag)
    {
        string sql;
        if (flag)
            sql = "INSERT INTO [User_Status](UserID, SegmentID, Status) VALUES('" + str_userID + "','" + str_segmentID + "','" + str_status + "')";
        else
            sql = "UPDATE [User_Status] SET Status = '" + str_status + "' WHERE UserID = '" + str_userID + "' AND SegmentID = '" + str_segmentID + "'";
        int count = helper.ExecuteNonQuery(CommandType.Text, sql.Trim(), null);
        if (count == 1)
            return true;
        else
            return false;
    }

    private string getUserStatus(string str_userID, string str_segmentID)
    {
        string sql = "SELECT status FROM [User_Status] WHERE UserID = '" + str_userID + "' AND SegmentID = '" + str_segmentID + "'";
        DataSet ds = helper.GetDataSet(sql);

        if (ds.Tables[0].Rows.Count == 0)
            return "";
        else
            return ds.Tables[0].Rows[0][0].ToString().Trim();
    }

    private void bindStatus(string str_userID, string str_segmentID)
    {
        string str_status = getUserStatus(str_userID, str_segmentID).Trim();
        bool success;

        ibtn_green.Enabled = false;
        ibtn_red.Enabled = false;
        if (str_status == "")
        {
            ibtn_orange.Enabled = true;
            success = setUserStatus(str_userID, str_segmentID, "R", true);
            img_status.ImageUrl = "~/images/red.png";
            if (success)
            { }
            else
            { }
        }
        else
        {
            if (str_status == "Y")
            {
                img_status.ImageUrl = "~/images/orange.png";
                ibtn_orange.Enabled = false;
                lbtn_editRSM.Visible = false;
            }
            else if (str_status == "G")
            {
                img_status.ImageUrl = "~/images/green.png";
                ibtn_orange.Enabled = false;
                lbtn_editRSM.Visible = false;
            }
            else
            {
                img_status.ImageUrl = "~/images/red.png";
                ibtn_orange.Enabled = true;
                lbtn_editRSM.Visible = true;
            }
        }
    }

    protected void ibtn_red_Click(object sender, ImageClickEventArgs e)
    {

    }

    protected void ibtn_orange_Click(object sender, ImageClickEventArgs e)
    {
        bool success;
        success = setUserStatus(getRSMID(), ddlist_segment.SelectedItem.Value.Trim(), "Y", false);
        if (success)
        { }
        else
        { }

        img_status.ImageUrl = "~/images/orange.png";
        lbtn_editRSM.Visible = false;
        ibtn_orange.Enabled = false;
    }

    protected void ibtn_green_Click(object sender, ImageClickEventArgs e)
    {

    }

    // 20110216 wy add start
    private string getCountrySQL()
    {
        string query_country = "SELECT [SubRegion].ID FROM [Country_SubRegion] "
                            + " INNER JOIN [SubRegion] ON [SubRegion].ID = [Country_SubRegion].SubregionID "
                            + " INNER JOIN [Country] ON [Country].ID = [Country_SubRegion].CountryID "
                            + " INNER JOIN [User_Country] ON [User_Country].CountryID = [Country].ID"
                            + " WHERE UserID  = " + getRSMID()
                            + " AND [User_Country].Deleted = 0 AND [SubRegion].Deleted = 0"
                            + " GROUP BY [SubRegion].ID, [SubRegion].Name";
        return query_country;
    }
    // 20110216 wy add end

    // add by zy 20110126 start
    // get counrty bu rsm
    protected DataSet getCountryInfo()
    {
        string query_country = "SELECT [SubRegion].ID, [SubRegion].Name FROM [Country_SubRegion] "
                            + " INNER JOIN [SubRegion] ON [SubRegion].ID = [Country_SubRegion].SubregionID "
                            + " INNER JOIN [Country] ON [Country].ID = [Country_SubRegion].CountryID "
                            + " INNER JOIN [User_Country] ON [User_Country].CountryID = [Country].ID"
                            + " WHERE UserID  = " + getRSMID()
                            + " AND [User_Country].Deleted = 0 AND [SubRegion].Deleted = 0"
                            + " GROUP BY [SubRegion].ID, [SubRegion].Name";
        DataSet ds_country = helper.GetDataSet(query_country);

        if ((ds_country.Tables.Count > 0) && (ds_country.Tables[0].Rows.Count > 0))
            return ds_country;
        else
            return null;
    }

    // add cell onclick attributes
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
    // add by zy 20110126 end
}
