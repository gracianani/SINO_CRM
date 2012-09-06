using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text;
using Microsoft.Office.Interop.Excel;
using System.Drawing;
using System.IO;

public partial class BookingBySalesOrgByUser : System.Web.UI.Page
{
    #region Constant
    private const string fiscalStart = "Oct.1";
    private const string fiscalEnd = "Sept.30";
    #endregion

    #region Global Variable
    private static SQLHelper helper = new SQLHelper();
    private static LogUtility log = new LogUtility();
    private static SQLStatement sql = new SQLStatement();
    private static WebUtility web = new WebUtility();
    private static GetMeetingDate date = new GetMeetingDate();
    SQLBookingInterface sqlBooking = new SQLBookingInterface();
    CommonFunction cf = new CommonFunction();

    private static string timeFlag;
    private static string preyear;
    private static string year;
    private static string nextyear;
    private static string yearAfterNext;
    private static string month;
    private static string premonth;
    private static string userAbbr;

    private static bool NullData;
    #endregion

    #region Event
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            initParams();
            initLable();
            bindDataSource();
        }
    }
    #endregion

    #region Method
    #region Helper Methon
    /// <summary>
    /// Init Set Meeging Date
    /// </summary>
    private void initParams()
    {
        timeFlag = getTimeFlag();
        date.setDate();
        preyear = date.getpreyear();
        year = date.getyear();
        nextyear = date.getnextyear();
        yearAfterNext = date.getyearAfterNext();
        month = date.getmonth();
        premonth = date.getPreMonth(month);
        userAbbr = getAbbrByUserID().Trim();
    }

    /// <summary>
    /// Init Lable Info
    /// </summary>
    private void initLable()
    {
        this.label_salesorgAbbr.Text = getAbbrBySalesOrg();
        this.label_headdescription.Text = getAbbrSegment() + " -BOOKINGS " + year;
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
    }

    /// <summary>
    /// Get TimeFlag
    /// </summary>
    /// <returns>TimeFlag</returns>
    private string getTimeFlag()
    {
        DataSet ds = date.getSetMeetingDate();
        return ds.Tables[0].Rows[0][0].ToString();
    }

    /// <summary>
    /// Get Abbr By User
    /// </summary>
    /// <returns>Abbr</returns>
    private string getAbbrByUserID()
    {
        string selSql = " SELECT Abbr FROM [User] WHERE UserID=" + Request.QueryString["RSMID"];
        object abbr = helper.ExecuteScalar(CommandType.Text, selSql, null);
        return Convert.ToString(abbr);
    }


    /// <summary>
    /// Get Alias By User
    /// </summary>
    /// <returns>Abbr</returns>
    private string getAliasByUserID()
    {
        string selSql = " SELECT Alias FROM [User] WHERE UserID=" + Request.QueryString["RSMID"];
        object abbr = helper.ExecuteScalar(CommandType.Text, selSql, null);
        return Convert.ToString(abbr);
    }

    /// <summary>
    /// Get Abbr By SalesOrg
    /// </summary>
    /// <returns>Abbr</returns>
    private string getAbbrBySalesOrg()
    {
        string selSql = "SELECT Abbr FROM SalesOrg WHERE ID=" + Request.QueryString["SalesOrgID"];
        object abbr = helper.ExecuteScalar(CommandType.Text, selSql, null);
        return Convert.ToString(abbr);
    }

    /// <summary>
    /// Get Abbr By Segment
    /// </summary>
    /// <returns>Abbr</returns>
    protected string getAbbrSegment()
    {
        string selSql = "SELECT Abbr FROM Segment WHERE ID=" + Request.QueryString["SegmentID"];
        object abbr = helper.ExecuteScalar(CommandType.Text, selSql, null);
        return Convert.ToString(abbr);
    }

    /// <summary>
    /// Get Product By Segment
    /// </summary>
    /// <returns>Product</returns>
    private DataSet getProductBySegment()
    {
        if (string.IsNullOrEmpty(Request.QueryString["SegmentID"]))
        {
            return null;
        }
        else
        {
            StringBuilder selSql = new StringBuilder();
            selSql.AppendLine(" SELECT ");
            selSql.AppendLine("   Product.ID, ");
            selSql.AppendLine("   Product.Abbr ");
            selSql.AppendLine(" FROM ");
            selSql.AppendLine("   Product ");
            selSql.AppendLine("   INNER JOIN Segment_Product ON Segment_Product.ProductID=Product.ID ");
            selSql.AppendLine(" WHERE ");
            selSql.AppendLine("   Segment_Product.SegmentID=" + Request.QueryString["SegmentID"]);
            selSql.AppendLine("   AND Product.Deleted=0 ");
            selSql.AppendLine("   AND Segment_Product.Deleted=0 ");
            selSql.AppendLine(" ORDER BY ");
            selSql.AppendLine("   Product.Abbr ");
            return helper.GetDataSet(selSql.ToString());
        }
    }

    /// <summary>
    /// Get Country SQL
    /// </summary>
    /// <returns>SQL</returns>
    private string getCountrySQL()
    {
        StringBuilder selSql = new StringBuilder();
        if (!string.Equals(Session["Role"].ToString(), "Administrator"))
        {
            selSql.AppendLine(" SELECT ");
            selSql.AppendLine("   SubRegion.ID ");
            selSql.AppendLine(" FROM ");
            selSql.AppendLine("   User_Country ");
            selSql.AppendLine("   INNER JOIN SubRegion ON User_Country.CountryID=SubRegion.ID");
            selSql.AppendLine(" WHERE ");
            selSql.AppendLine("   User_Country.UserID=" + Request.QueryString["UserID"]);
            selSql.AppendLine("   AND SubRegion.Deleted=0 ");
            selSql.AppendLine("   AND User_Country.Deleted=0 ");
        }
        else
        {
            selSql.AppendLine(" SELECT ");
            selSql.AppendLine("   SubRegion.ID ");
            selSql.AppendLine(" FROM ");
            selSql.AppendLine("   SubRegion");
            selSql.AppendLine(" WHERE ");
            selSql.AppendLine("   SubRegion.Deleted=0 ");
        }
        return selSql.ToString();
    }

    /// <summary>
    /// Set Word Break
    /// </summary>
    /// <param name="gv"></param>
    private void setBreakStyele(GridView gv)
    {

        for (int i = 0; i < gv.Rows.Count; i++)
        {
            for (int j = 0; j < gv.Columns.Count; j++)
            {
                gv.Rows[i].Cells[j].Attributes.Add("style", "width:" + gv.Rows[i].Cells[j].Width + "px;word-break:break-all;");
            }
        }
    }
    #endregion

    /// <summary>
    /// Bind DataSource
    /// </summary>
    private void bindDataSource()
    {
        DataSet dsProduct = getProductBySegment();
        // Year YTD
        gv_bookingbydatebyproduct.Columns.Clear();
        gv_bookingTotalbydatebyproduct.Columns.Clear();
        this.GridView1.Columns.Clear();
        bindDataByDateByProduct(gv_bookingbydatebyproduct, dsProduct, year.Substring(2, 2), "YTD");
        bindDataTotalByOperation(gv_bookingTotalbydatebyproduct, dsProduct, year.Substring(2, 2), "YTD");
        bindDataTotalByCountry(this.GridView1, dsProduct, year.Substring(2, 2), "YTD");
        // Year For Year
        gv_booking1bydatebyproduct.Columns.Clear();
        gv_booking1Totalbydatebyproduct.Columns.Clear();
        this.GridView2.Columns.Clear();
        bindDataByDateByProduct(gv_booking1bydatebyproduct, dsProduct, year.Substring(2, 2), year.Substring(2, 2));
        bindDataTotalByOperation(gv_booking1Totalbydatebyproduct, dsProduct, year.Substring(2, 2), year.Substring(2, 2));
        bindDataTotalByCountry(this.GridView2, dsProduct, year.Substring(2, 2), year.Substring(2, 2));
        // Year For NextYear
        gv_booking2bydatebyproduct.Columns.Clear();
        gv_booking2Totalbydatebyproduct.Columns.Clear();
        this.GridView3.Columns.Clear();
        bindDataByDateByProduct(gv_booking2bydatebyproduct, dsProduct, year.Substring(2, 2), nextyear.Substring(2, 2));
        bindDataTotalByOperation(gv_booking2Totalbydatebyproduct, dsProduct, year.Substring(2, 2), nextyear.Substring(2, 2));
        bindDataTotalByCountry(this.GridView3, dsProduct, year.Substring(2, 2), nextyear.Substring(2, 2));
        // NextYear For NextYear
        gv_booking3bydatebyproduct.Columns.Clear();
        gv_booking3Totalbydatebyproduct.Columns.Clear();
        this.GridView4.Columns.Clear();
        bindDataByDateByProduct(gv_booking3bydatebyproduct, dsProduct, nextyear.Substring(2, 2), nextyear.Substring(2, 2));
        bindDataTotalByOperation(gv_booking3Totalbydatebyproduct, dsProduct, nextyear.Substring(2, 2), nextyear.Substring(2, 2));
        bindDataTotalByCountry(this.GridView4, dsProduct, nextyear.Substring(2, 2), nextyear.Substring(2, 2));
        // NextYear For YearAfterNext
        gv_booking4bydatebyproduct.Columns.Clear();
        gv_booking4Totalbydatebyproduct.Columns.Clear();
        this.GridView5.Columns.Clear();
        bindDataByDateByProduct(gv_booking4bydatebyproduct, dsProduct, nextyear.Substring(2, 2), yearAfterNext.Substring(2, 2));
        bindDataTotalByOperation(gv_booking4Totalbydatebyproduct, dsProduct, nextyear.Substring(2, 2), yearAfterNext.Substring(2, 2));
        bindDataTotalByCountry(this.GridView5, dsProduct, nextyear.Substring(2, 2), yearAfterNext.Substring(2, 2));
        // If Has Data
        if (NullData)
        {
            // Product
            bindDataByProduct(dsProduct);
            // Total Year
            gv_bookingtbydate.Columns.Clear();
            gv_bookingtTotalbydate.Columns.Clear();
            this.GridView6.Columns.Clear();
            bindDataByDate(gv_bookingtbydate, dsProduct);
            bindDataTotalByDate(gv_bookingtTotalbydate, dsProduct);
            bindDataTotalGBCountry(this.GridView6, dsProduct);
            // Total NextYear
            gv_bookingnextbydate.Columns.Clear();
            gv_bookingnextTotalbydate.Columns.Clear();
            this.GridView7.Columns.Clear();
            bindDataNextByDate(gv_bookingnextbydate, dsProduct);
            bindDataNextTotalByDate(gv_bookingnextTotalbydate, dsProduct);
            bindDataNextTotalGBCountry(this.GridView7, dsProduct);
            // Total Year(BYear)
            gv_VS.Columns.Clear();
            gv_VSTotal.Columns.Clear();
            this.GridView8.Columns.Clear();
            bindDataVSPre(gv_VS, dsProduct);
            bindDataTotalVSPre(gv_VSTotal, dsProduct);
            bindDataTotalVSPreGBCountry(this.GridView8, dsProduct);
        }
    }

    #region Bookings Sales Data
    /// <summary>
    /// Get Booking Data
    /// </summary>
    /// <param name="dsPro">Product</param>
    /// <param name="bookingY">Booking Year</param>
    /// <param name="deliverY">Deliver Year</param>
    /// <returns>Booking Data</returns>
    private DataSet getBookingDataByDateByProduct(DataSet dsPro, string bookingY, string deliverY)
    {
        string strSQL = sqlBooking.getBookingSalesDataByBD(Request.QueryString["RSMID"], Request.QueryString["SalesOrgID"],
            Request.QueryString["SegmentID"], Request.QueryString["CountryID"], year, month, getCountrySQL());
        return sql.getBookingSalesDataByBD(strSQL, dsPro, Request.QueryString["SalesOrgID"], bookingY, deliverY, year, month,
            Convert.ToBoolean(Request.QueryString["ConvertFlag"]));
    }

    /// <summary>
    /// Bind Product Data By Booking Year And Delivery Year
    /// </summary>
    /// <param name="gv">GridView</param>
    /// <param name="dsProduct">Product Data Set</param>
    /// <param name="bookingY">Booking Year</param>
    /// <param name="deliverY">Delivery Year</param>
    private void bindDataByDateByProduct(GridView gv, DataSet dsProduct, string bookingY, string deliverY)
    {
        if (dsProduct == null || dsProduct.Tables.Count == 0 || dsProduct.Tables[0].Rows.Count == 0
            || string.IsNullOrEmpty(Request.QueryString["RSMID"]))
        {
            NullData = false;
            gv.Visible = false;
        }
        else
        {
            NullData = true;
            DataSet dsBookingInfo = getBookingDataByDateByProduct(dsProduct, bookingY, deliverY);
            if (dsBookingInfo == null)
            {
                gv.Visible = false;
            }
            else
            {
                dsBookingInfo.Tables[0].Columns.RemoveAt(4);
                if (dsBookingInfo.Tables[0].Rows.Count == 0)
                {
                    sql.getNullDataSet(dsBookingInfo);
                }
                string caption = null;
                if (string.Equals(deliverY, "YTD"))
                {
                    caption = bookingY + deliverY + "  " + fiscalStart + "," + preyear + " to " + date.getMonth(month) + date.getDay() + "," + year;
                }
                else if (string.Equals(bookingY, year.Substring(2, 2)))
                {
                    caption = bookingY + " for " + deliverY + "  " + date.getMonth(month) + date.getDay() + "," + year + " to " + fiscalEnd + "," + bookingY + " for " + deliverY + " delivery";
                }
                else
                {
                    caption = bookingY + " for " + deliverY + "  " + fiscalStart + "," + year + " to " + fiscalEnd + "," + bookingY + " for " + deliverY + " delivery";
                }
                gv.Visible = true;
                gv.AutoGenerateColumns = false;
                gv.Caption = caption;
                gv.CaptionAlign = TableCaptionAlign.Top;
                BoundField bf = null;
                int tableWidth = 0;
                for (int i = 0; i < dsBookingInfo.Tables[0].Columns.Count; i++)
                {
                    bf = new BoundField();
                    bf.DataField = dsBookingInfo.Tables[0].Columns[i].ColumnName;
                    bf.HeaderText = dsBookingInfo.Tables[0].Columns[i].Caption;
                    if (i <= 9)
                    {
                        if (i == 4 || i == 5 || i == 6 || i == 7 || i == 8)
                        {
                            bf.HeaderStyle.Width = Unit.Pixel(100);
                            bf.ItemStyle.Width = Unit.Pixel(100);
                            tableWidth += 100;
                        }
                        else if (i == 9)
                        {
                            bf.HeaderStyle.Width = Unit.Pixel(75);
                            bf.ItemStyle.Width = Unit.Pixel(75);
                            tableWidth += 75;
                        }
                    }
                    else
                    {
                        if (i % 3 == 2)
                        {
                            bf.HeaderText = null;
                            bf.HeaderStyle.Width = Unit.Pixel(25);
                            bf.ItemStyle.Width = Unit.Pixel(25);
                            tableWidth += 25;
                        }
                        if (i % 3 == 1)
                        {
                            bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Right;
                            bf.ItemStyle.HorizontalAlign = HorizontalAlign.Right;
                            bf.HeaderStyle.Width = Unit.Pixel(75);
                            bf.ItemStyle.Width = Unit.Pixel(75);
                            tableWidth += 75;
                        }
                    }
                    gv.Columns.Add(bf);
                }
                gv.DataSource = dsBookingInfo.Tables[0];
                gv.DataBind();
                gv.Columns[0].Visible = false;
                gv.Columns[1].Visible = false;
                gv.Columns[2].Visible = false;
                gv.Columns[3].Visible = false;
                int visableNum = 4;
                for (int i = 10; i < gv.Columns.Count; i += 3)
                {
                    gv.Columns[i + 2].Visible = false;
                    visableNum++;
                }
                int pandding = (dsBookingInfo.Tables[0].Columns.Count - visableNum) * 2 * gv.CellPadding;
                gv.Width = Unit.Pixel(tableWidth + pandding);
                setBreakStyele(gv);
            }
        }
    }

    /// <summary>
    /// Get Total Booking Data
    /// </summary>
    /// <param name="dsPro">Product</param>
    /// <param name="bookingY">Booking Year</param>
    /// <param name="deliverY">Deliver Year</param>
    /// <returns>Total Booking Data</returns>
    private DataSet getBookingDataTotalByDateByProduct(DataSet dsPro, string bookingY, string deliverY)
    {
        string sqlStr = sqlBooking.getBookingDataTotal(Request.QueryString["SalesOrgID"], dsPro,
            Request.QueryString["SegmentID"], userAbbr, Request.QueryString["RSMID"], bookingY, deliverY,
            year, month, Request.QueryString["CountryID"], Convert.ToBoolean(Request.QueryString["ConvertFlag"]),
            getCountrySQL());
        if (!string.IsNullOrEmpty(sqlStr))
        {
            DataSet ds = helper.GetDataSet(sqlStr);
            if (ds == null || ds.Tables.Count == 0)
            {
                return null;
            }
            else
            {
                DataRow sumRow = ds.Tables[0].NewRow();
                int sum = 0;
                sumRow[0] = "TTL/" + userAbbr;
                for (int i = 1; i < ds.Tables[0].Columns.Count; i++)
                {
                    sum = 0;
                    for (int j = 0; j < ds.Tables[0].Rows.Count; j++)
                    {
                        sum += Convert.ToInt32(ds.Tables[0].Rows[j][i]);
                    }
                    sumRow[i] = sum;
                }
                ds.Tables[0].Rows.InsertAt(sumRow, 0);
                return ds;
            }
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Bind Product Data Total By Booking Year And Delivery Year
    /// </summary>
    /// <param name="gv">GridView</param>
    /// <param name="dsProduct">Product Data Set</param>
    /// <param name="bookingY">Booking Year</param>
    /// <param name="deliverY">Delivery Year</param>
    protected void bindDataTotalByOperation(GridView gv, DataSet dsProduct, string bookingY, string deliverY)
    {
        if (dsProduct == null || dsProduct.Tables.Count == 0 || dsProduct.Tables[0].Rows.Count == 0
            || string.IsNullOrEmpty(Request.QueryString["RSMID"]))
        {
            gv.Visible = false;
        }
        else
        {
            DataSet dsBookingInfo = getBookingDataTotalByDateByProduct(dsProduct, bookingY, deliverY);
            if (dsBookingInfo == null)
            {
                gv.Visible = false;
            }
            else
            {
                if (dsBookingInfo.Tables[0].Rows.Count == 0)
                {
                    sql.getNullDataSet(dsBookingInfo);
                }
                gv.Visible = true;
                gv.AutoGenerateColumns = false;
                BoundField bf = null;
                BoundField nullBf = null;
                int tableWidth = 0;
                for (int i = 0; i < dsBookingInfo.Tables[0].Columns.Count; i++)
                {
                    bf = new BoundField();
                    bf.DataField = dsBookingInfo.Tables[0].Columns[i].ColumnName;
                    bf.HeaderText = dsBookingInfo.Tables[0].Columns[i].Caption;
                    bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Right;
                    bf.ItemStyle.HorizontalAlign = HorizontalAlign.Right;
                    if (i == 0)
                    {
                        bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
                        bf.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
                        bf.HeaderStyle.Width = Unit.Pixel(575 + 6 * 2 * gv.CellPadding);
                        bf.ItemStyle.Width = Unit.Pixel(575 + 6 * 2 * gv.CellPadding);
                        tableWidth += 575 + 6 * 2 * gv.CellPadding;
                    }
                    else
                    {
                        bf.HeaderStyle.Width = Unit.Pixel(75);
                        bf.ItemStyle.Width = Unit.Pixel(75);
                        tableWidth += 75;

                        nullBf = new BoundField();
                        nullBf.HeaderStyle.Width = Unit.Pixel(25);
                        nullBf.ItemStyle.Width = Unit.Pixel(25);
                        tableWidth += 25;
                    }
                    gv.Columns.Add(bf);
                    if (nullBf != null)
                    {
                        gv.Columns.Add(nullBf);
                    }
                }
                int pandding = (dsBookingInfo.Tables[0].Columns.Count - 1) * 2 * 2 * gv.CellPadding;
                gv.Width = Unit.Pixel(tableWidth + pandding);
                gv.DataSource = dsBookingInfo.Tables[0];
                gv.DataBind();
                setBreakStyele(gv);
            }
        }
    }

    /// <summary>
    /// Get Total Booking Data
    /// </summary>
    /// <param name="dsPro">Product</param>
    /// <param name="bookingY">Booking Year</param>
    /// <param name="deliverY">Deliver Year</param>
    /// <returns>Total Booking Data</returns>
    private DataSet getBookingDataTotalByCountry(DataSet dsPro, string bookingY, string deliverY)
    {
        string sqlStr = sqlBooking.getBookingDataTotalByCountry(Request.QueryString["SalesOrgID"], dsPro,
            Request.QueryString["SegmentID"], userAbbr, Request.QueryString["RSMID"], bookingY, deliverY,
            year, month, Request.QueryString["CountryID"], Convert.ToBoolean(Request.QueryString["ConvertFlag"]),
            getCountrySQL());
        if (!string.IsNullOrEmpty(sqlStr))
        {
            DataSet ds = helper.GetDataSet(sqlStr);
            if (ds == null || ds.Tables.Count == 0)
            {
                return null;
            }
            else
            {
                DataRow sumRow = ds.Tables[0].NewRow();
                int sum = 0;
                sumRow[0] = "TTL/" + userAbbr;
                for (int i = 1; i < ds.Tables[0].Columns.Count; i++)
                {
                    sum = 0;
                    for (int j = 0; j < ds.Tables[0].Rows.Count; j++)
                    {
                        sum += Convert.ToInt32(ds.Tables[0].Rows[j][i]);
                    }
                    sumRow[i] = sum;
                }
                ds.Tables[0].Rows.InsertAt(sumRow, 0);
                return ds;
            }
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Bind Product Data Total By Booking Year And Delivery Year
    /// </summary>
    /// <param name="gv">GridView</param>
    /// <param name="dsProduct">Product Data Set</param>
    /// <param name="bookingY">Booking Year</param>
    /// <param name="deliverY">Delivery Year</param>
    protected void bindDataTotalByCountry(GridView gv, DataSet dsProduct, string bookingY, string deliverY)
    {
        if (dsProduct == null || dsProduct.Tables.Count == 0 || dsProduct.Tables[0].Rows.Count == 0
            || string.IsNullOrEmpty(Request.QueryString["RSMID"]))
        {
            gv.Visible = false;
        }
        else
        {
            DataSet dsBookingInfo = getBookingDataTotalByCountry(dsProduct, bookingY, deliverY);
            if (dsBookingInfo == null)
            {
                gv.Visible = false;
            }
            else
            {
                if (dsBookingInfo.Tables[0].Rows.Count == 0)
                {
                    sql.getNullDataSet(dsBookingInfo);
                }
                gv.Visible = true;
                gv.AutoGenerateColumns = false;
                BoundField bf = null;
                BoundField nullBf = null;
                int tableWidth = 0;
                for (int i = 0; i < dsBookingInfo.Tables[0].Columns.Count; i++)
                {
                    bf = new BoundField();
                    bf.DataField = dsBookingInfo.Tables[0].Columns[i].ColumnName;
                    bf.HeaderText = dsBookingInfo.Tables[0].Columns[i].Caption;
                    bf.ItemStyle.HorizontalAlign = HorizontalAlign.Right;
                    bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Right;
                    if (i == 0)
                    {
                        bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
                        bf.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
                        bf.HeaderStyle.Width = Unit.Pixel(575 + 6 * 2 * gv.CellPadding);
                        bf.ItemStyle.Width = Unit.Pixel(575 + 6 * 2 * gv.CellPadding);
                        tableWidth += 575 + 6 * 2 * gv.CellPadding;
                    }
                    else
                    {
                        bf.HeaderStyle.Width = Unit.Pixel(75);
                        bf.ItemStyle.Width = Unit.Pixel(75);
                        tableWidth += 75;

                        nullBf = new BoundField();
                        nullBf.HeaderStyle.Width = Unit.Pixel(25);
                        nullBf.ItemStyle.Width = Unit.Pixel(25);
                        tableWidth += 25;
                    }
                    gv.Columns.Add(bf);
                    if (nullBf != null)
                    {
                        gv.Columns.Add(nullBf);
                    }
                }
                int pandding = (dsBookingInfo.Tables[0].Columns.Count - 1) * 2 * 2 * gv.CellPadding;
                gv.Width = Unit.Pixel(tableWidth + pandding);
                gv.DataSource = dsBookingInfo.Tables[0];
                gv.DataBind();
            }
        }
    }
    #endregion

    #region Product
    /// <summary>
    /// Bind Product Bookings Info
    /// </summary>
    /// <param name="dsProduct">Product Data Set</param>
    private void bindDataByProduct(DataSet dsProduct)
    {
        if (dsProduct != null && dsProduct.Tables.Count > 0)
        {
            DataSet dsBookings = null;
            DataSet dsTotalByOperation = null;
            DataSet dsTotalByCountry = null;
            GridView gvBookings = null;
            GridView gvTotalByOperation = null;
            GridView gvTotalByCountry = null;
            TableRow tr = new TableRow();
            TableCell tc = null;
            HtmlGenericControl div = null;
            int index = 0;
            this.table_bookingsByProduct.Visible = true;
            this.table_bookingsByProduct.Rows.Add(tr);
            for (int i = 0; i < dsProduct.Tables[0].Rows.Count; i++)
            {
                tc = new TableCell();
                tc.HorizontalAlign = HorizontalAlign.Left;
                tc.VerticalAlign = VerticalAlign.Top;
                #region Detail
                gvBookings = new GridView();
                web.setProperties(gvBookings);
                dsBookings = getBookingsDataByProduct(dsProduct.Tables[0].Rows[i][0].ToString());
                if (dsBookings != null && dsBookings.Tables.Count != 0)
                {
                    if (dsBookings.Tables[0].Rows.Count == 0)
                    {
                        sql.getNullDataSet(dsBookings);
                    }
                    gvBookings = showBookingsByProduct(dsBookings, gvBookings, dsProduct.Tables[0].Rows[i][1].ToString());
                    div = new HtmlGenericControl();
                    div.ID = "" + index++;
                    div.Attributes.Add("style", "display:block");
                    div.Controls.Add(gvBookings);
                    tc.Controls.Add(div);
                }
                #endregion

                #region Total By Operation
                gvTotalByOperation = new GridView();
                web.setProperties(gvTotalByOperation);
                dsTotalByOperation = getBookingDataTotalByProduct(dsProduct.Tables[0].Rows[i][0].ToString());
                if (dsTotalByOperation != null)
                {
                    gvTotalByOperation = showTotalByProduct(dsTotalByOperation, gvTotalByOperation);
                    div = new HtmlGenericControl();
                    div.ID = "" + index++;
                    div.Attributes.Add("style", "display:block");
                    div.Controls.Add(gvTotalByOperation);
                    tc.Controls.Add(div);
                }
                #endregion

                #region Total By Country
                gvTotalByCountry = new GridView();
                web.setProperties(gvTotalByCountry);
                dsTotalByCountry = getBDTotalByProductGBCountry(dsProduct.Tables[0].Rows[i][0].ToString());
                if (dsTotalByCountry != null && dsTotalByCountry.Tables.Count > 0)
                {
                    gvTotalByCountry = showTotalByProduct(dsTotalByCountry, gvTotalByCountry);
                    div = new HtmlGenericControl();
                    div.ID = "" + index++;
                    div.Attributes.Add("style", "display:block");
                    div.Controls.Add(gvTotalByCountry);
                    tc.Controls.Add(div);
                }
                #endregion
                tr.Controls.Add(tc);
            }
        }
    }

    /// <summary>
    /// Get Bookings Data By Product
    /// </summary>
    /// <param name="productID">Product ID</param>
    /// <returns>Bookings Date</returns>
    private DataSet getBookingsDataByProduct(string productID)
    {
        string sqlstr = sqlBooking.getBookingSalesDataByProduct(Request.QueryString["RSMID"], Request.QueryString["SalesOrgID"],
            Request.QueryString["SegmentID"], Request.QueryString["CountryID"], productID, year, month, preyear,
            Convert.ToBoolean(Request.QueryString["ConvertFlag"]), getCountrySQL());
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

    /// <summary>
    /// Bind Bookings Data By Product GirdView
    /// </summary>
    /// <param name="ds">DataSet</param>
    /// <param name="gv">GridView</param>
    /// <param name="header">Header</param>
    /// <returns>GridView</returns>
    private GridView showBookingsByProduct(DataSet ds, GridView gv, string header)
    {
        if (ds == null)
        {
            gv.Visible = false;
        }
        else
        {
            gv.Visible = true;
            gv.AutoGenerateColumns = false;
            gv.CellPadding = 4;
            gv.Columns.Clear();
            try
            {
                ds.Tables[0].Columns.Add("VAR");
                if (ds.Tables[0].Rows[0][0] != DBNull.Value)
                {
                    float tmp = 0;
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
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
                Response.Redirect("Error.aspx");
            }
            BoundField bf = null;
            int tableWidth = 0;
            for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
            {
                bf = new BoundField();
                bf.DataField = ds.Tables[0].Columns[i].ColumnName.ToString();
                bf.HeaderText = ds.Tables[0].Columns[i].Caption.ToString();
                if (i >= 4)
                {
                    if (i <= 7)
                    {
                        bf.HeaderStyle.Width = Unit.Pixel(100);
                        bf.ItemStyle.Width = Unit.Pixel(100);
                        tableWidth += 100;
                    }
                    else
                    {
                        bf.ItemStyle.HorizontalAlign = HorizontalAlign.Right;
                        bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Right;
                        bf.HeaderStyle.Width = Unit.Pixel(75);
                        bf.ItemStyle.Width = Unit.Pixel(75);
                        tableWidth += 75;
                    }
                }
                gv.Columns.Add(bf);
            }
            int padding = (ds.Tables[0].Columns.Count - 4) * 2 * gv.CellPadding;
            gv.Width = Unit.Pixel(tableWidth + padding);
            gv.Caption = header;
            gv.CaptionAlign = TableCaptionAlign.Top;
            gv.DataSource = ds;
            gv.DataBind();
            gv.Columns[0].Visible = false;
            gv.Columns[1].Visible = false;
            gv.Columns[2].Visible = false;
            gv.Columns[3].Visible = false;
            setBreakStyele(gv);
        }
        return gv;
    }

    /// <summary>
    /// Get Total Bookings Data By Product
    /// </summary>
    /// <param name="productID">Product ID</param>
    /// <returns>GirdView</returns>
    private DataSet getBookingDataTotalByProduct(string productID)
    {
        string strSQL = sqlBooking.getBookingSalesDataTotleByProduct(Request.QueryString["RSMID"], userAbbr,
            Request.QueryString["SalesOrgID"], Request.QueryString["SegmentID"], Request.QueryString["CountryID"],
            productID, year, month, preyear, Convert.ToBoolean(Request.QueryString["ConvertFlag"]), getCountrySQL());
        DataSet ds = helper.GetDataSet(strSQL);
        if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
        {
            return null;
        }
        else
        {
            DataRow sumRow = ds.Tables[0].NewRow();
            int sum = 0;
            sumRow[0] = "TTL/" + userAbbr;
            for (int i = 1; i < ds.Tables[0].Columns.Count; i++)
            {
                sum = 0;
                for (int j = 0; j < ds.Tables[0].Rows.Count; j++)
                {
                    sum += Convert.ToInt32(ds.Tables[0].Rows[j][i]);
                }
                sumRow[i] = sum;
            }
            ds.Tables[0].Rows.InsertAt(sumRow, 0);
            return ds;
        }
    }

    /// <summary>
    /// Get Total Bookings Data By Product
    /// </summary>
    /// <param name="productID">Product ID</param>
    /// <returns>GirdView</returns>
    private DataSet getBDTotalByProductGBCountry(string productID)
    {
        string sqlStr = sqlBooking.getBDTotalByProductGBCountry(Request.QueryString["SegmentID"], productID,
            userAbbr, Request.QueryString["RSMID"], Request.QueryString["SalesOrgID"], year, month, preyear,
            Request.QueryString["CountryID"], Convert.ToBoolean(Request.QueryString["ConvertFlag"]), getCountrySQL());
        if (!string.IsNullOrEmpty(sqlStr))
        {
            DataSet ds = helper.GetDataSet(sqlStr);
            if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
            {
                return null;
            }
            else
            {
                DataRow sumRow = ds.Tables[0].NewRow();
                int sum = 0;
                sumRow[0] = "TTL/" + userAbbr;
                for (int i = 1; i < ds.Tables[0].Columns.Count; i++)
                {
                    sum = 0;
                    for (int j = 0; j < ds.Tables[0].Rows.Count; j++)
                    {
                        sum += Convert.ToInt32(ds.Tables[0].Rows[j][i]);
                    }
                    sumRow[i] = sum;
                }
                ds.Tables[0].Rows.InsertAt(sumRow, 0);
                return ds;
            }
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    ///Bind Total Bookings Data By Product GridView
    /// </summary>
    /// <param name="ds"></param>
    /// <param name="gv"></param>
    /// <returns></returns>
    private GridView showTotalByProduct(DataSet ds, GridView gv)
    {
        if (ds == null)
        {
            gv.Visible = false;
        }
        else
        {
            gv.Visible = true;
            gv.AutoGenerateColumns = false;
            gv.CellPadding = 4;
            gv.Columns.Clear();
            try
            {
                ds.Tables[0].Columns.Add("VAR");
                if (ds.Tables[0].Rows[0][0] != DBNull.Value)
                {
                    float tmp = 0;
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
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
                Response.Redirect("Error.aspx");
            }
            BoundField bf = null;
            int tableWidth = 0;
            for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
            {
                bf = new BoundField();
                bf.DataField = ds.Tables[0].Columns[i].ColumnName.ToString();
                bf.HeaderText = ds.Tables[0].Columns[i].Caption.ToString();
                bf.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
                bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
                if (i == 0)
                {
                    bf.HeaderStyle.Width = Unit.Pixel(400 + 4 * 2 * gv.CellPadding);
                    bf.ItemStyle.Width = Unit.Pixel(400 + 4 * 2 * gv.CellPadding);
                    tableWidth += 400 + 4 * 2 * gv.CellPadding;
                }
                else
                {
                    bf.ItemStyle.HorizontalAlign = HorizontalAlign.Right;
                    bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Right;
                    bf.HeaderStyle.Width = Unit.Pixel(75);
                    bf.ItemStyle.Width = Unit.Pixel(75);
                    tableWidth += 75;
                }
                gv.Columns.Add(bf);
            }
            int padding = (ds.Tables[0].Columns.Count - 1) * 2 * gv.CellPadding;
            gv.Width = Unit.Pixel(tableWidth + padding);
            gv.DataSource = ds;
            gv.DataBind();
            setBreakStyele(gv);
        }
        return gv;
    }
    #endregion

    #region Total Year
    /// <summary>
    /// Get Bookings By Date
    /// </summary>
    /// <returns>Bookings</returns>
    private DataSet getBookingsDataThisyear(DataSet dsPro)
    {
        string strSQL = sqlBooking.getBookingSalesDataByBD(Request.QueryString["RSMID"], Request.QueryString["SalesOrgID"],
            Request.QueryString["SegmentID"], Request.QueryString["CountryID"], year, month, getCountrySQL());
        DataSet ds = sql.getBookingSalesDataThisYear(strSQL, dsPro, Request.QueryString["SalesOrgID"], year, month,
            Convert.ToBoolean(Request.QueryString["ConvertFlag"]));
        if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
        {
            return null;
        }
        else
        {
            return ds;
        }
    }

    /// <summary>
    /// Bind Data By Date
    /// </summary>
    /// <param name="gv">GridView</param>
    protected void bindDataByDate(GridView gv, DataSet dsProduct)
    {
        DataSet ds = getBookingsDataThisyear(dsProduct);
        if (ds == null)
        {
            gv.Visible = false;
        }
        else
        {
            gv.Visible = true;
            gv.AutoGenerateColumns = false;
            gv.Caption = "Total(" + year.Substring(2, 2) + ")";
            gv.CaptionAlign = TableCaptionAlign.Top;
            ds.Tables[0].Columns.Add("Total");
            try
            {
                if (ds.Tables[0].Rows[0][0] != DBNull.Value)
                {
                    float tmp = 0;
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        tmp = 0;
                        for (int i = 9; i < ds.Tables[0].Columns.Count - 2; i += 2)
                        {
                            tmp += float.Parse(dr[i].ToString());
                        }
                        dr["Total"] = tmp.ToString();
                    }
                }
            }
            catch
            {
                log.WriteLog(LogUtility.LogErrorLevel.LOG_ERROR, "Format string to float Error.");
                Response.Redirect("Error.aspx");
            }
            BoundField bf = null;
            int tableWidth = 0;
            for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
            {
                bf = new BoundField();
                bf.DataField = ds.Tables[0].Columns[i].ColumnName.ToString();
                bf.HeaderText = ds.Tables[0].Columns[i].Caption.ToString();
                if (i <= 8)
                {
                    if (i == 4 || i == 5 || i == 6 || i == 7)
                    {
                        bf.HeaderStyle.Width = Unit.Pixel(100);
                        bf.ItemStyle.Width = Unit.Pixel(100);
                        tableWidth += 100;
                    }
                    else if (i == 8)
                    {
                        bf.HeaderStyle.Width = Unit.Pixel(80);
                        bf.ItemStyle.Width = Unit.Pixel(80);
                        tableWidth += 80;
                    }
                }
                else if (i < ds.Tables[0].Columns.Count - 1)
                {
                    if (i % 2 == 0)
                    {
                        bf.HeaderText = null;
                        bf.HeaderStyle.Width = Unit.Pixel(25);
                        bf.ItemStyle.Width = Unit.Pixel(25);
                        tableWidth += 25;
                    }
                    if (i % 2 == 1)
                    {
                        bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Right;
                        bf.ItemStyle.HorizontalAlign = HorizontalAlign.Right;
                        bf.HeaderStyle.Width = Unit.Pixel(75);
                        bf.ItemStyle.Width = Unit.Pixel(75);
                        tableWidth += 75;
                    }
                }
                else
                {
                    bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Right;
                    bf.ItemStyle.HorizontalAlign = HorizontalAlign.Right;
                    bf.HeaderStyle.Width = Unit.Pixel(75);
                    bf.ItemStyle.Width = Unit.Pixel(75);
                    tableWidth += 75;
                }
                gv.Columns.Add(bf);
            }
            int pandding = (ds.Tables[0].Columns.Count - 4) * 2 * gv.CellPadding;
            gv.Width = Unit.Pixel(tableWidth + pandding);
            gv.DataSource = ds.Tables[0];
            gv.DataBind();
            gv.Columns[0].Visible = false;
            gv.Columns[1].Visible = false;
            gv.Columns[2].Visible = false;
            gv.Columns[3].Visible = false;
            setBreakStyele(gv);
        }
    }

    /// <summary>
    /// Get Total Bookings By Date
    /// </summary>
    /// <returns>Total Bookings</returns>
    private DataSet getBookingDataTotalThisYear(DataSet dsPro)
    {
        string strSQL = sqlBooking.getBookingDataTotalThisYear(dsPro, Request.QueryString["SalesOrgID"],
            Request.QueryString["SegmentID"], userAbbr, Request.QueryString["RSMID"], year, month,
            Request.QueryString["CountryID"], Convert.ToBoolean(Request.QueryString["ConvertFlag"]), getCountrySQL());
        if (string.IsNullOrEmpty(strSQL))
        {
            return null;
        }
        else
        {
            return helper.GetDataSet(strSQL);
        }
    }

    /// <summary>
    /// Bind Total Data By Date
    /// </summary>
    /// <param name="gv">GridView</param>
    private void bindDataTotalByDate(GridView gv, DataSet dsProduct)
    {
        DataSet ds = getBookingDataTotalThisYear(dsProduct);
        if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
        {
            gv.Visible = false;
        }
        else
        {
            gv.Visible = true;
            gv.AutoGenerateColumns = false;
            DataRow sumRow = ds.Tables[0].NewRow();
            int sum = 0;
            sumRow[0] = "TTL/" + userAbbr;
            for (int i = 1; i < ds.Tables[0].Columns.Count; i++)
            {
                sum = 0;
                for (int j = 0; j < ds.Tables[0].Rows.Count; j++)
                {
                    sum += Convert.ToInt32(ds.Tables[0].Rows[j][i]);
                }
                sumRow[i] = sum;
            }
            ds.Tables[0].Rows.InsertAt(sumRow, 0);
            ds.Tables[0].Columns.Add("Total");
            try
            {
                if (ds.Tables[0].Rows[0][0] != DBNull.Value)
                {
                    float tmp = 0;
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        tmp = 0;
                        for (int i = 1; i < ds.Tables[0].Columns.Count - 3; i++)
                        {
                            tmp += float.Parse(dr[i].ToString());
                        }
                        dr["Total"] = tmp.ToString();
                    }
                }
            }
            catch
            {
                log.WriteLog(LogUtility.LogErrorLevel.LOG_ERROR, "Format string to float Error.");
                Response.Redirect("Error.aspx");
            }
            BoundField bf = null;
            BoundField nullBf = null;
            int tableWidth = 0;
            for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
            {
                bf = new BoundField();
                bf.DataField = ds.Tables[0].Columns[i].ColumnName.ToString();
                bf.HeaderText = ds.Tables[0].Columns[i].Caption.ToString();
                bf.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
                bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
                if (i == 0)
                {
                    bf.HeaderStyle.Width = Unit.Pixel(480 + 5 * 2 * gv.CellPadding);
                    bf.ItemStyle.Width = Unit.Pixel(480 + 5 * 2 * gv.CellPadding);
                    tableWidth += 480 + 5 * 2 * gv.CellPadding;
                }
                else
                {
                    bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Right;
                    bf.ItemStyle.HorizontalAlign = HorizontalAlign.Right;
                    bf.HeaderStyle.Width = Unit.Pixel(75);
                    bf.ItemStyle.Width = Unit.Pixel(75);
                    tableWidth += 75;
                    if (i != ds.Tables[0].Columns.Count - 1)
                    {
                        nullBf = new BoundField();
                        nullBf.HeaderStyle.Width = Unit.Pixel(25);
                        nullBf.ItemStyle.Width = Unit.Pixel(25);
                        tableWidth += 25;
                    }
                    else
                    {
                        nullBf = null;
                    }
                }
                gv.Columns.Add(bf);
                if (nullBf != null)
                {
                    gv.Columns.Add(nullBf);
                }
            }
            int pandding = (ds.Tables[0].Columns.Count - 1) * 2 * 2 * gv.CellPadding - 8;
            gv.Width = Unit.Pixel(tableWidth + pandding);
            gv.DataSource = ds.Tables[0];
            gv.DataBind();
            setBreakStyele(gv);
        }
    }

    /// <summary>
    /// Get Total Bookings By Date
    /// </summary>
    /// <returns>Total Bookings</returns>
    private DataSet getBDTotalThisYearGBCountry(DataSet dsPro)
    {
        string sqlStr = sqlBooking.getBDTotalThisYearGBCountry(dsPro, Request.QueryString["SalesOrgID"],
            Request.QueryString["SegmentID"], userAbbr, Request.QueryString["RSMID"], year, month,
            Request.QueryString["CountryID"], Convert.ToBoolean(Request.QueryString["ConvertFlag"]), getCountrySQL());
        if (string.IsNullOrEmpty(sqlStr))
        {
            return null;
        }
        else
        {
            return helper.GetDataSet(sqlStr);
        }
    }

    /// <summary>
    /// Bind Total Data By Date
    /// </summary>
    /// <param name="gv">GridView</param>
    private void bindDataTotalGBCountry(GridView gv, DataSet dsProduct)
    {
        DataSet ds = getBDTotalThisYearGBCountry(dsProduct);
        if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
        {
            gv.Visible = false;
        }
        else
        {
            gv.Visible = true;
            gv.AutoGenerateColumns = false;
            DataRow sumRow = ds.Tables[0].NewRow();
            int sum = 0;
            sumRow[0] = "TTL/" + userAbbr;
            for (int i = 1; i < ds.Tables[0].Columns.Count; i++)
            {
                sum = 0;
                for (int j = 0; j < ds.Tables[0].Rows.Count; j++)
                {
                    sum += Convert.ToInt32(ds.Tables[0].Rows[j][i]);
                }
                sumRow[i] = sum;
            }
            ds.Tables[0].Rows.InsertAt(sumRow, 0);
            ds.Tables[0].Columns.Add("Total");
            try
            {
                if (ds.Tables[0].Rows[0][0] != DBNull.Value)
                {
                    float tmp = 0;
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        for (int i = 1; i < ds.Tables[0].Columns.Count - 3; i++)
                        {
                            tmp += float.Parse(dr[i].ToString());
                        }
                        dr["Total"] = tmp.ToString();
                    }
                }
            }
            catch
            {
                log.WriteLog(LogUtility.LogErrorLevel.LOG_ERROR, "Format string to float Error.");
                Response.Redirect("Error.aspx");
            }
            BoundField bf = null;
            BoundField nullBf = null;
            int tableWidth = 0;
            for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
            {
                bf = new BoundField();
                bf.DataField = ds.Tables[0].Columns[i].ColumnName.ToString();
                bf.HeaderText = ds.Tables[0].Columns[i].Caption.ToString();
                bf.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
                bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
                if (i == 0)
                {
                    bf.HeaderStyle.Width = Unit.Pixel(480 + 5 * 2 * gv.CellPadding);
                    bf.ItemStyle.Width = Unit.Pixel(480 + 5 * 2 * gv.CellPadding);
                    tableWidth += 480 + 5 * 2 * gv.CellPadding;
                }
                else
                {
                    bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Right;
                    bf.ItemStyle.HorizontalAlign = HorizontalAlign.Right;
                    bf.HeaderStyle.Width = Unit.Pixel(75);
                    bf.ItemStyle.Width = Unit.Pixel(75);
                    tableWidth += 75;
                    if (i != ds.Tables[0].Columns.Count - 1)
                    {
                        nullBf = new BoundField();
                        nullBf.HeaderStyle.Width = Unit.Pixel(25);
                        nullBf.ItemStyle.Width = Unit.Pixel(25);
                        tableWidth += 25;
                    }
                    else
                    {
                        nullBf = null;
                    }
                }
                gv.Columns.Add(bf);
                if (nullBf != null)
                {
                    gv.Columns.Add(nullBf);
                }
            }
            int pandding = (ds.Tables[0].Columns.Count - 1) * 2 * 2 * gv.CellPadding - 8;
            gv.Width = Unit.Pixel(tableWidth + pandding);
            gv.DataSource = ds.Tables[0];
            gv.DataBind();
            setBreakStyele(gv);
        }
    }
    #endregion

    #region Total NextYear
    /// <summary>
    /// Get Bookings By Next Date
    /// </summary>
    /// <param name="dsPro">Product</param>
    /// <returns>Bookings</returns>
    private DataSet getBookingsDataNextYear(DataSet dsPro)
    {
        string strSQL = sqlBooking.getBookingSalesDataByBD(Request.QueryString["RSMID"], Request.QueryString["SalesOrgID"],
            Request.QueryString["SegmentID"], Request.QueryString["CountryID"], year, month, getCountrySQL());
        DataSet ds = sql.getBookingSalesDataNextYear(strSQL, dsPro, Request.QueryString["SalesOrgID"], year, month, nextyear,
            Convert.ToBoolean(Request.QueryString["ConvertFlag"]));
        if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
        {
            return null;
        }
        else
        {
            return ds;
        }
    }

    /// <summary>
    /// Bind Date By Next Date
    /// </summary>
    /// <param name="gv">GridView</param>
    /// <param name="dsProduct">Product</param>
    private void bindDataNextByDate(GridView gv, DataSet dsProduct)
    {
        DataSet ds = getBookingsDataNextYear(dsProduct);
        if (ds == null)
        {
            gv.Visible = false;
        }
        else
        {
            gv.Visible = true;
            gv.AutoGenerateColumns = false;
            gv.Caption = "Total(" + nextyear.Substring(2, 2) + ")";
            gv.CaptionAlign = TableCaptionAlign.Top;
            ds.Tables[0].Columns.Add("Total");
            try
            {
                if (ds.Tables[0].Rows[0][0] != DBNull.Value)
                {
                    float tmp = 0;
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        tmp = 0;
                        for (int i = 9; i < ds.Tables[0].Columns.Count - 2; i += 2)
                        {
                            tmp += float.Parse(dr[i].ToString());
                        }
                        dr["Total"] = tmp.ToString();
                    }
                }
            }
            catch
            {
                log.WriteLog(LogUtility.LogErrorLevel.LOG_ERROR, "Format string to float Error.");
                Response.Redirect("Error.aspx");
            }
            BoundField bf = null;
            int tableWidth = 0;
            for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
            {
                bf = new BoundField();
                bf.DataField = ds.Tables[0].Columns[i].ColumnName.ToString();
                bf.HeaderText = ds.Tables[0].Columns[i].Caption.ToString();
                if (i <= 8)
                {
                    if (i == 4 || i == 5 || i == 6 || i == 7)
                    {
                        bf.HeaderStyle.Width = Unit.Pixel(100);
                        bf.ItemStyle.Width = Unit.Pixel(100);
                        tableWidth += 100;
                    }
                    else if (i == 8)
                    {
                        bf.HeaderStyle.Width = Unit.Pixel(80);
                        bf.ItemStyle.Width = Unit.Pixel(80);
                        tableWidth += 80;
                    }
                }
                else if (i < ds.Tables[0].Columns.Count - 1)
                {
                    if (i % 2 == 0)
                    {
                        bf.HeaderText = null;
                        bf.HeaderStyle.Width = Unit.Pixel(25);
                        bf.ItemStyle.Width = Unit.Pixel(25);
                        tableWidth += 25;
                    }
                    if (i % 2 == 1)
                    {
                        bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Right;
                        bf.ItemStyle.HorizontalAlign = HorizontalAlign.Right;
                        bf.HeaderStyle.Width = Unit.Pixel(75);
                        bf.ItemStyle.Width = Unit.Pixel(75);
                        tableWidth += 75;
                    }
                }
                else
                {
                    bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Right;
                    bf.ItemStyle.HorizontalAlign = HorizontalAlign.Right;
                    bf.HeaderStyle.Width = Unit.Pixel(75);
                    bf.ItemStyle.Width = Unit.Pixel(75);
                    tableWidth += 75;
                }
                gv.Columns.Add(bf);
            }
            int pandding = (ds.Tables[0].Columns.Count - 4) * 2 * gv.CellPadding;
            gv.Width = Unit.Pixel(tableWidth + pandding);
            gv.DataSource = ds.Tables[0];
            gv.DataBind();
            gv.Columns[0].Visible = false;
            gv.Columns[1].Visible = false;
            gv.Columns[2].Visible = false;
            gv.Columns[3].Visible = false;
            setBreakStyele(gv);
        }
    }

    /// <summary>
    /// Get Total Bookings By Next Date
    /// </summary>
    /// <param name="dsPro">Product</param>
    /// <returns>Total Bookings</returns>
    private DataSet getBookingDataTotalNextYear(DataSet dsPro)
    {
        string sqlStr = sqlBooking.getBookingDataTotalByNextYear(Request.QueryString["SalesOrgID"], dsPro,
            Request.QueryString["SegmentID"], userAbbr, Request.QueryString["RSMID"], year, month, nextyear,
            Request.QueryString["CountryID"], Convert.ToBoolean(Request.QueryString["ConvertFlag"]), getCountrySQL());
        if (string.IsNullOrEmpty(sqlStr))
        {
            return null;
        }
        else
        {
            return helper.GetDataSet(sqlStr);
        }
    }

    /// <summary>
    /// Bind Total Date By Next Date
    /// </summary>
    /// <param name="gv">GridView</param>
    /// <param name="dsProduct">Product</param>
    private void bindDataNextTotalByDate(GridView gv, DataSet dsProduct)
    {
        DataSet ds = getBookingDataTotalNextYear(dsProduct);
        if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
        {
            gv.Visible = false;
        }
        else
        {
            gv.Visible = true;
            gv.AutoGenerateColumns = false;
            DataRow sumRow = ds.Tables[0].NewRow();
            int sum = 0;
            sumRow[0] = "TTL/" + userAbbr;
            for (int i = 1; i < ds.Tables[0].Columns.Count; i++)
            {
                sum = 0;
                for (int j = 0; j < ds.Tables[0].Rows.Count; j++)
                {
                    sum += Convert.ToInt32(ds.Tables[0].Rows[j][i]);
                }
                sumRow[i] = sum;
            }
            ds.Tables[0].Rows.InsertAt(sumRow, 0);
            ds.Tables[0].Columns.Add("Total");
            try
            {
                if (ds.Tables[0].Rows[0][0] != DBNull.Value)
                {
                    float tmp = 0;
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        tmp = 0;
                        for (int i = 1; i < ds.Tables[0].Columns.Count - 3; i++)
                        {
                            tmp += float.Parse(dr[i].ToString());
                        }
                        dr["Total"] = tmp.ToString();
                    }
                }
            }
            catch
            {
                log.WriteLog(LogUtility.LogErrorLevel.LOG_ERROR, "Format string to float Error.");
                Response.Redirect("Error.aspx");
            }
            BoundField bf = null;
            BoundField nullBf = null;
            int tableWidth = 0;
            for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
            {
                bf = new BoundField();
                bf.DataField = ds.Tables[0].Columns[i].ColumnName.ToString();
                bf.HeaderText = ds.Tables[0].Columns[i].Caption.ToString();
                bf.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
                bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
                if (i == 0)
                {
                    bf.HeaderStyle.Width = Unit.Pixel(480 + 5 * 2 * gv.CellPadding);
                    bf.ItemStyle.Width = Unit.Pixel(480 + 5 * 2 * gv.CellPadding);
                    tableWidth += 480 + 5 * 2 * gv.CellPadding;
                }
                else
                {
                    bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Right;
                    bf.ItemStyle.HorizontalAlign = HorizontalAlign.Right;
                    bf.HeaderStyle.Width = Unit.Pixel(75);
                    bf.ItemStyle.Width = Unit.Pixel(75);
                    tableWidth += 75;
                    if (i != ds.Tables[0].Columns.Count - 1)
                    {
                        nullBf = new BoundField();
                        nullBf.HeaderStyle.Width = Unit.Pixel(25);
                        nullBf.ItemStyle.Width = Unit.Pixel(25);
                        tableWidth += 25;
                    }
                    else
                    {
                        nullBf = null;
                    }
                }
                gv.Columns.Add(bf);
                if (nullBf != null)
                {
                    gv.Columns.Add(nullBf);
                }
            }
            int pandding = (ds.Tables[0].Columns.Count - 1) * 2 * 2 * gv.CellPadding - 8;
            gv.Width = Unit.Pixel(tableWidth + pandding);
            gv.DataSource = ds.Tables[0];
            gv.DataBind();
            setBreakStyele(gv);
        }
    }

    /// <summary>
    /// Get Total Bookings By Next Date
    /// </summary>
    /// <param name="dsPro">Product</param>
    /// <returns>Total Bookings</returns>
    private DataSet getBDTotalNextYearGBCountry(DataSet dsPro)
    {
        string sqlStr = sqlBooking.getBDTotalNextYearGBCountry(Request.QueryString["SalesOrgID"], dsPro,
            Request.QueryString["SegmentID"], userAbbr, Request.QueryString["RSMID"], year, month, nextyear,
            Request.QueryString["CountryID"], Convert.ToBoolean(Request.QueryString["ConvertFlag"]), getCountrySQL());
        if (string.IsNullOrEmpty(sqlStr))
        {
            return null;
        }
        else
        {
            return helper.GetDataSet(sqlStr);
        }
    }

    /// <summary>
    /// Bind Total Date By Next Date
    /// </summary>
    /// <param name="gv">GridView</param>
    /// <param name="dsProduct">Product</param>
    private void bindDataNextTotalGBCountry(GridView gv, DataSet dsProduct)
    {
        DataSet ds = getBDTotalNextYearGBCountry(dsProduct);
        if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
        {
            gv.Visible = false;
        }
        else
        {
            gv.Visible = true;
            gv.AutoGenerateColumns = false;
            DataRow sumRow = ds.Tables[0].NewRow();
            int sum = 0;
            sumRow[0] = "TTL/" + userAbbr;
            for (int i = 1; i < ds.Tables[0].Columns.Count; i++)
            {
                sum = 0;
                for (int j = 0; j < ds.Tables[0].Rows.Count; j++)
                {
                    sum += Convert.ToInt32(ds.Tables[0].Rows[j][i]);
                }
                sumRow[i] = sum;
            }
            ds.Tables[0].Rows.InsertAt(sumRow, 0);
            ds.Tables[0].Columns.Add("Total");
            try
            {
                if (ds.Tables[0].Rows[0][0] != DBNull.Value)
                {
                    float tmp = 0;
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        for (int i = 1; i < ds.Tables[0].Columns.Count - 3; i++)
                        {
                            tmp += float.Parse(dr[i].ToString());
                        }
                        dr["Total"] = tmp.ToString();
                    }
                }
            }
            catch
            {
                log.WriteLog(LogUtility.LogErrorLevel.LOG_ERROR, "Format string to float Error.");
                Response.Redirect("Error.aspx");
            }
            BoundField bf = null;
            BoundField nullBf = null;
            int tableWidth = 0;
            for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
            {
                bf = new BoundField();
                bf.DataField = ds.Tables[0].Columns[i].ColumnName.ToString();
                bf.HeaderText = ds.Tables[0].Columns[i].Caption.ToString();
                bf.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
                bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
                if (i == 0)
                {
                    bf.HeaderStyle.Width = Unit.Pixel(480 + 5 * 2 * gv.CellPadding);
                    bf.ItemStyle.Width = Unit.Pixel(480 + 5 * 2 * gv.CellPadding);
                    tableWidth += 480 + 5 * 2 * gv.CellPadding;
                }
                else
                {
                    bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Right;
                    bf.ItemStyle.HorizontalAlign = HorizontalAlign.Right;
                    bf.HeaderStyle.Width = Unit.Pixel(75);
                    bf.ItemStyle.Width = Unit.Pixel(75);
                    tableWidth += 75;
                    if (i != ds.Tables[0].Columns.Count - 1)
                    {
                        nullBf = new BoundField();
                        nullBf.HeaderStyle.Width = Unit.Pixel(25);
                        nullBf.ItemStyle.Width = Unit.Pixel(25);
                        tableWidth += 25;
                    }
                    else
                    {
                        nullBf = null;
                    }
                }
                gv.Columns.Add(bf);
                if (nullBf != null)
                {
                    gv.Columns.Add(nullBf);
                }
            }
            int pandding = (ds.Tables[0].Columns.Count - 1) * 2 * 2 * gv.CellPadding - 8;
            gv.Width = Unit.Pixel(tableWidth + pandding);
            gv.DataSource = ds.Tables[0];
            gv.DataBind();
            setBreakStyele(gv);
        }
    }
    #endregion

    #region Total Year PerYear VS
    /// <summary>
    /// Get Bookings By DateVSPer
    /// </summary>
    /// <param name="dsPro">Product</param>
    /// <returns>Bookings</returns>
    private DataSet getBookingsDataVSPre(DataSet dsPro)
    {
        string thisYearSQL = sqlBooking.getBookingSalesDataByBD(Request.QueryString["RSMID"], Request.QueryString["SalesOrgID"],
            Request.QueryString["SegmentID"], Request.QueryString["CountryID"], year, month, getCountrySQL());
        string thisYearVSSQL = sqlBooking.getBookingSalesDataByBD(Request.QueryString["RSMID"], Request.QueryString["SalesOrgID"],
            Request.QueryString["SegmentID"], Request.QueryString["CountryID"], year, premonth, getCountrySQL());
        string preYearVSSQL = sqlBooking.getBookingSalesDataByBD(Request.QueryString["RSMID"], Request.QueryString["SalesOrgID"],
            Request.QueryString["SegmentID"], Request.QueryString["CountryID"], preyear, premonth, getCountrySQL());
        DataSet ds = sql.getBookingSalesDataThisYearVSPreYear(thisYearSQL, thisYearVSSQL, preYearVSSQL,
            dsPro, Request.QueryString["SalesOrgID"], year, month, preyear, premonth, Convert.ToBoolean(Request.QueryString["ConvertFlag"]));
        if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
        {
            return null;
        }
        else
        {
            return ds;
        }
    }

    /// <summary>
    /// Bind Data VS PerDate
    /// </summary>
    /// <param name="gv">GridView</param>
    /// <param name="dsProduct">Product</param>
    protected void bindDataVSPre(GridView gv, DataSet dsProduct)
    {
        DataSet ds = getBookingsDataVSPre(dsProduct);
        if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
        {
            gv.Visible = false;
        }
        else
        {
            gv.Visible = true;
            gv.Width = Unit.Pixel(700);
            gv.AutoGenerateColumns = false;
            gv.Caption = "Total" + year.Substring(2, 2) + "(" + preyear.Substring(2, 2) + ")";
            gv.CaptionAlign = TableCaptionAlign.Top;
            ds.Tables[0].Columns.Add("Total");
            try
            {
                if (ds.Tables[0].Rows[0][0] != DBNull.Value)
                {
                    float tmp = 0;
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        tmp = 0;
                        for (int i = 9; i < ds.Tables[0].Columns.Count - 1; i++)
                        {
                            tmp += float.Parse(dr[i].ToString());
                        }
                        dr["Total"] = tmp.ToString();
                    }
                }
            }
            catch
            {
                log.WriteLog(LogUtility.LogErrorLevel.LOG_ERROR, "Format string to float Error.");
                Response.Redirect("Error.aspx");
            }
            BoundField bf = null;
            int tableWidth = 0;
            for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
            {
                bf = new BoundField();
                bf.DataField = ds.Tables[0].Columns[i].ColumnName.ToString();
                bf.HeaderText = ds.Tables[0].Columns[i].Caption.ToString();
                if (i <= 8)
                {
                    if (i == 4 || i == 5 || i == 6 || i == 7)
                    {
                        bf.HeaderStyle.Width = Unit.Pixel(100);
                        bf.ItemStyle.Width = Unit.Pixel(100);
                        tableWidth += 100;
                    }
                    else if (i == 8)
                    {
                        bf.HeaderStyle.Width = Unit.Pixel(80);
                        bf.ItemStyle.Width = Unit.Pixel(80);
                        tableWidth += 80;
                    }
                }
                else
                {
                    bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Right;
                    bf.ItemStyle.HorizontalAlign = HorizontalAlign.Right;
                    bf.HeaderStyle.Width = Unit.Pixel(75);
                    bf.ItemStyle.Width = Unit.Pixel(75);
                    tableWidth += 75;
                }
                gv.Columns.Add(bf);
            }
            int pandding = (ds.Tables[0].Columns.Count - 4) * 2 * gv.CellPadding;
            gv.Width = Unit.Pixel(tableWidth + pandding);
            gv.DataSource = ds.Tables[0];
            gv.DataBind();
            gv.Columns[0].Visible = false;
            gv.Columns[1].Visible = false;
            gv.Columns[2].Visible = false;
            gv.Columns[3].Visible = false;
            setBreakStyele(gv);
        }
    }

    /// <summary>
    /// Get Total Bookings By DateVSPer
    /// </summary>
    /// <param name="dsPro">Product</param>
    /// <returns>Total Bookings</returns>
    private DataSet getBookingDataTotalVSPre(DataSet dsPro)
    {
        string sqlStr = sqlBooking.getBookingDataTotaByThislVSPre(Request.QueryString["SalesOrgID"], dsPro,
            Request.QueryString["SegmentID"], userAbbr, Request.QueryString["RSMID"], year, month, preyear, premonth,
            Request.QueryString["CountryID"], Convert.ToBoolean(Request.QueryString["ConvertFlag"]), getCountrySQL());
        if (string.IsNullOrEmpty(sqlStr))
        {
            return null;
        }
        else
        {
            return helper.GetDataSet(sqlStr);
        }
    }

    /// <summary>
    /// Bind Total Data VS PerDate
    /// </summary>
    /// <param name="gv"></param>
    /// <param name="dsProduct"></param>
    protected void bindDataTotalVSPre(GridView gv, DataSet dsProduct)
    {
        DataSet ds = getBookingDataTotalVSPre(dsProduct);
        if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
        {
            gv.Visible = false;
        }
        else
        {
            gv.Visible = true;
            gv.AutoGenerateColumns = false;
            DataRow sumRow = ds.Tables[0].NewRow();
            int sum = 0;
            sumRow[0] = "TTL/" + userAbbr;
            for (int i = 1; i < ds.Tables[0].Columns.Count; i++)
            {
                sum = 0;
                for (int j = 0; j < ds.Tables[0].Rows.Count; j++)
                {
                    sum += Convert.ToInt32(ds.Tables[0].Rows[j][i]);
                }
                sumRow[i] = sum;
            }
            ds.Tables[0].Rows.InsertAt(sumRow, 0);
            ds.Tables[0].Columns.Add("Total");
            try
            {
                if (ds.Tables[0].Rows[0][0] != DBNull.Value)
                {
                    float tmp = 0;
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        for (int i = 1; i < ds.Tables[0].Columns.Count - 1; i++)
                        {
                            tmp += float.Parse(dr[i].ToString());
                        }
                        dr["Total"] = tmp.ToString();
                    }
                }
            }
            catch
            {
                log.WriteLog(LogUtility.LogErrorLevel.LOG_ERROR, "Format string to float Error.");
                Response.Redirect("Error.aspx");
            }
            BoundField bf = null;
            int tableWidth = 0;
            for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
            {
                bf = new BoundField();
                bf.DataField = ds.Tables[0].Columns[i].ColumnName.ToString();
                bf.HeaderText = ds.Tables[0].Columns[i].Caption.ToString();
                bf.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
                bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
                if (i == 0)
                {
                    bf.HeaderStyle.Width = Unit.Pixel(480 + 5 * 2 * gv.CellPadding);
                    bf.ItemStyle.Width = Unit.Pixel(480 + 5 * 2 * gv.CellPadding);
                    tableWidth += 480 + 5 * 2 * gv.CellPadding;
                }
                else
                {
                    bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Right;
                    bf.ItemStyle.HorizontalAlign = HorizontalAlign.Right;
                    bf.HeaderStyle.Width = Unit.Pixel(75);
                    bf.ItemStyle.Width = Unit.Pixel(75);
                    tableWidth += 75;
                }
                gv.Columns.Add(bf);
            }
            int pandding = (ds.Tables[0].Columns.Count - 1) * 2 * gv.CellPadding;
            gv.Width = Unit.Pixel(tableWidth + pandding);
            gv.DataSource = ds.Tables[0];
            gv.DataBind();
            setBreakStyele(gv);
        }
    }

    /// <summary>
    /// Get Total Bookings By DateVSPer
    /// </summary>
    /// <param name="dsPro">Product</param>
    /// <returns>Total Bookings</returns>
    private DataSet getBDTotaByThislVSPreGBCountry(DataSet dsPro)
    {
        string sqlStr = sqlBooking.getBDTotaByThislVSPreGBCountry(Request.QueryString["SalesOrgID"], dsPro,
            Request.QueryString["SegmentID"], userAbbr, Request.QueryString["RSMID"], year, month, preyear, premonth,
            Request.QueryString["CountryID"], Convert.ToBoolean(Request.QueryString["ConvertFlag"]), getCountrySQL());
        if (string.IsNullOrEmpty(sqlStr))
        {
            return null;
        }
        else
        {
            return helper.GetDataSet(sqlStr);
        }
    }

    /// <summary>
    /// Bind Total Data VS PerDate
    /// </summary>
    /// <param name="gv"></param>
    /// <param name="dsProduct"></param>
    protected void bindDataTotalVSPreGBCountry(GridView gv, DataSet dsProduct)
    {
        DataSet ds = getBDTotaByThislVSPreGBCountry(dsProduct);
        if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
        {
            gv.Visible = false;
        }
        else
        {
            gv.Visible = true;
            gv.AutoGenerateColumns = false;
            DataRow sumRow = ds.Tables[0].NewRow();
            int sum = 0;
            sumRow[0] = "TTL/" + userAbbr;
            for (int i = 1; i < ds.Tables[0].Columns.Count; i++)
            {
                sum = 0;
                for (int j = 0; j < ds.Tables[0].Rows.Count; j++)
                {
                    sum += Convert.ToInt32(ds.Tables[0].Rows[j][i]);
                }
                sumRow[i] = sum;
            }
            ds.Tables[0].Rows.InsertAt(sumRow, 0);
            ds.Tables[0].Columns.Add("Total");
            try
            {
                if (ds.Tables[0].Rows[0][0] != DBNull.Value)
                {
                    float tmp = 0;
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        for (int i = 1; i < ds.Tables[0].Columns.Count - 1; i++)
                        {
                            tmp += float.Parse(dr[i].ToString());
                        }
                        dr["Total"] = tmp.ToString();
                    }
                }
            }
            catch
            {
                log.WriteLog(LogUtility.LogErrorLevel.LOG_ERROR, "Format string to float Error.");
                Response.Redirect("Error.aspx");
            }
            BoundField bf = null;
            int tableWidth = 0;
            for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
            {
                bf = new BoundField();
                bf.DataField = ds.Tables[0].Columns[i].ColumnName.ToString();
                bf.HeaderText = ds.Tables[0].Columns[i].Caption.ToString();
                bf.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
                bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
                if (i == 0)
                {
                    bf.HeaderStyle.Width = Unit.Pixel(480 + 5 * 2 * gv.CellPadding);
                    bf.ItemStyle.Width = Unit.Pixel(480 + 5 * 2 * gv.CellPadding);
                    tableWidth += 480 + 5 * 2 * gv.CellPadding;
                }
                else
                {
                    bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Right;
                    bf.ItemStyle.HorizontalAlign = HorizontalAlign.Right;
                    bf.HeaderStyle.Width = Unit.Pixel(75);
                    bf.ItemStyle.Width = Unit.Pixel(75);
                    tableWidth += 75;
                }
                gv.Columns.Add(bf);
            }
            int pandding = (ds.Tables[0].Columns.Count - 1) * 2 * gv.CellPadding;
            gv.Width = Unit.Pixel(tableWidth + pandding);
            gv.DataSource = ds.Tables[0];
            gv.DataBind();
            setBreakStyele(gv);
        }
    }
    #endregion

    #region Export
    /// <summary>
    /// Export
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
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
        setExpData(sheet, this.gv_bookingbydatebyproduct, ref rowIndex, ref colIndex, 0);
        colIndex = startCIndex;
        setExpData(sheet, this.gv_bookingTotalbydatebyproduct, ref rowIndex, ref colIndex, 5);
        colIndex = startCIndex;
        setExpData(sheet, this.GridView1, ref rowIndex, ref colIndex, 5);
        // Year-Year
        rowIndex = startRIndex;
        colIndex++;
        startCIndex = colIndex;
        setExpData(sheet, this.gv_booking1bydatebyproduct, ref rowIndex, ref colIndex, 0);
        colIndex = startCIndex;
        setExpData(sheet, this.gv_booking1Totalbydatebyproduct, ref rowIndex, ref colIndex, 5);
        colIndex = startCIndex;
        setExpData(sheet, this.GridView2, ref rowIndex, ref colIndex, 5);
        // Year-NextYear
        rowIndex = startRIndex;
        colIndex++;
        startCIndex = colIndex;
        setExpData(sheet, this.gv_booking2bydatebyproduct, ref rowIndex, ref colIndex, 0);
        colIndex = startCIndex;
        setExpData(sheet, this.gv_booking2Totalbydatebyproduct, ref rowIndex, ref colIndex, 5);
        colIndex = startCIndex;
        setExpData(sheet, this.GridView3, ref rowIndex, ref colIndex, 5);
        // NextYear-NextYear
        rowIndex = startRIndex;
        colIndex++;
        startCIndex = colIndex;
        setExpData(sheet, this.gv_booking3bydatebyproduct, ref rowIndex, ref colIndex, 0);
        colIndex = startCIndex;
        setExpData(sheet, this.gv_booking3Totalbydatebyproduct, ref rowIndex, ref colIndex, 5);
        colIndex = startCIndex;
        setExpData(sheet, this.GridView4, ref rowIndex, ref colIndex, 5);
        // NextYear-AfterNextYear
        rowIndex = startRIndex;
        colIndex++;
        startCIndex = colIndex;
        setExpData(sheet, this.gv_booking4bydatebyproduct, ref rowIndex, ref colIndex, 0);
        colIndex = startCIndex;
        setExpData(sheet, this.gv_booking4Totalbydatebyproduct, ref rowIndex, ref colIndex, 5);
        colIndex = startCIndex;
        setExpData(sheet, this.GridView5, ref rowIndex, ref colIndex, 5);
        // PerPorduct
        if (NullData)
        {
            ArrayList gvList = new ArrayList();
            ComputeSubControls(this.table_bookingsByProduct, typeof(GridView), gvList);
            for (int i = 0; i < gvList.Count; i++)
            {
                if ((i + 1) % 3 == 1)
                {
                    rowIndex = startRIndex;
                    colIndex++;
                    startCIndex = colIndex;
                    setExpData(sheet, (GridView)gvList[i], ref rowIndex, ref colIndex, 0);
                }
                else
                {
                    colIndex = startCIndex;
                    setExpData(sheet, (GridView)gvList[i], ref rowIndex, ref colIndex, 3);
                }
            }
            // Total Year
            rowIndex = startRIndex;
            colIndex++;
            startCIndex = colIndex;
            setExpData(sheet, this.gv_bookingtbydate, ref rowIndex, ref colIndex, 0);
            colIndex = startCIndex;
            setExpData(sheet, this.gv_bookingtTotalbydate, ref rowIndex, ref colIndex, 4);
            colIndex = startCIndex;
            setExpData(sheet, this.GridView6, ref rowIndex, ref colIndex, 4);
            // Total NextYear
            rowIndex = startRIndex;
            colIndex++;
            startCIndex = colIndex;
            setExpData(sheet, this.gv_bookingnextbydate, ref rowIndex, ref colIndex, 0);
            colIndex = startCIndex;
            setExpData(sheet, this.gv_bookingnextTotalbydate, ref rowIndex, ref colIndex, 4);
            colIndex = startCIndex;
            setExpData(sheet, this.GridView7, ref rowIndex, ref colIndex, 4);
            // Total Year VS NextYear
            rowIndex = startRIndex;
            colIndex++;
            startCIndex = colIndex;
            setExpData(sheet, this.gv_VS, ref rowIndex, ref colIndex, 0);
            colIndex = startCIndex;
            setExpData(sheet, this.gv_VSTotal, ref rowIndex, ref colIndex, 4);
            colIndex = startCIndex;
            setExpData(sheet, this.GridView8, ref rowIndex, ref colIndex, 4);
        }
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
        Response.AddHeader("Content-Disposition", "attachment; filename=" + Server.UrlEncode("RSM(" + getAbbrSegment().Trim() + "-" + getAliasByUserID().Trim() + ").xlsx"));
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
    private void setExpData(Worksheet sheet, GridView gv, ref int rowIndex, ref int colIndex, int mergeNum)
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
    #endregion
    #endregion
}
