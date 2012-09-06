/*
 * File Name    : BookingSalesData.aspx.cs
 * 
 * Description  : According to roles, users may edit and add booking data. and administrator can edit and add comments.
 * 
 * Author       : Wangjun
 * 
 * Modify Date : 2010-01-05
 * 
 * Problem     : 
 * 
 * Version     : Release(2.0)
 */



using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using Resources;
using AjaxPro;
using System.Globalization;

public partial class BookingSalesData : System.Web.UI.Page
{
    SQLHelper helper = new SQLHelper();
    LogUtility log = new LogUtility();
    DisplayInfo info = new DisplayInfo();
    WebUtility web = new WebUtility();
    SQLStatement sql = new SQLStatement();
    GetMeetingDate meeting = new GetMeetingDate();
    string preId = "Hid";

    private static bool NullData;
    protected static string preyear;
    protected static string year;
    protected static string nextyear;
    protected static string yearAfterNext;
    protected static string month;
    protected const string fiscalStart = "Oct.1";
    protected const string fiscalEnd = "Sept.30";
    private string countryID = "";
    DataSet productDs;
    private static bool butFlag = true;

    protected void Page_Load(object sender, EventArgs e)
    {
        Page.MaintainScrollPositionOnPostBack = true;
        Utility.RegisterTypeForAjax(typeof(BookingSalesData), this);
        if (getRoleID(getRole()) != "4" && getRoleID(getRole()) != "0" && getRoleID(getRole()) != "3")
        {
            log.WriteLog(LogUtility.LogErrorLevel.LOG_CRITICAL, "Only Administrator, General Sales Manager, RSM can access the page, BookingSalesData.aspx.");
            Response.Redirect("!/AccessDenied.aspx");
        }
        meeting.setDate();
        preyear = meeting.getpreyear();
        year = meeting.getyear();
        nextyear = meeting.getnextyear();
        yearAfterNext = meeting.getyearAfterNext();
        month = meeting.getmonth();
        productDs = getProductBySegment(getSegmentID());
        if (!IsPostBack)
        {
            this.txtOldCusName.Attributes.Add("readonly", "readonly");
            this.ddlist_projectYTD.Attributes.Add("readonly", "readonly");
            label_note.Text = "";
            this.lblError.Text = "";
            string str_segment = getSegmentInfo(getSegmentID()).Tables[0].Rows[0][0].ToString().Trim();
            label_RSMAbbr.Text = getRSMInfo(getRSMID()).Tables[0].Rows[0][0].ToString().Trim();
            label_headdescription.Text = str_segment + "-BOOKINGS for " + preyear + " and " + year;
            getCurrencyBySalesOrg(getSalesOrgID());
            pnl_edit.Visible = false;
            initDataTypeList();
            this.ddlDataType.SelectedValue = Request.QueryString["DataType"];
            ddlDataType_SelectedIndexChanged(sender, e);
            setEditCustomer();
        }
    }

    private string getRSMID()
    {
        return Request.QueryString["RSMID"].ToString().Trim();
    }

    private string getSegmentID()
    {
        return Request.QueryString["SegmentID"].ToString().Trim();
    }

    private string getSalesOrgID()
    {
        return Request.QueryString["SalesOrgID"].ToString().Trim();
    }
    private string getCountryID()
    {
        return Request.QueryString["CountryID"].ToString().Trim();
    }
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

    private DataSet getRSMInfo(string str_rsmID)
    {
        string sql = "SELECT Alias, Abbr  FROM [User] WHERE UserID = " + str_rsmID + " AND Deleted = 0";
        DataSet ds = helper.GetDataSet(sql);
        if (ds.Tables[0].Rows.Count == 1)
            return ds;
        else
        {
            log.WriteLog(LogUtility.LogErrorLevel.LOG_CRITICAL, "'" + sql + "' ,Querying result : " + ds.Tables[0].Rows.Count);
            Response.Redirect("~/AccessDenied.aspx");
            return null;
        }
    }

    protected DataSet getSegmentInfo(string str_segmentID)
    {
        string query_segment = "SELECT Description, Abbr FROM [Segment]"
                               + " WHERE ID=" + str_segmentID;
        DataSet ds_segment = helper.GetDataSet(query_segment);
        return ds_segment;
    }


    protected void getCurrencyBySalesOrg(string str_salesorgID)
    {
        string query_currency = "SELECT [Currency].Name FROM [SalesOrg] "
                            + " INNER JOIN [Currency] ON [Currency].ID = [SalesOrg].CurrencyID"
                            + " WHERE [SalesOrg].ID = '" + str_salesorgID + "'"
                            + " AND [SalesOrg].Deleted = 0";
        DataSet ds_currency = helper.GetDataSet(query_currency);

        if (ds_currency.Tables[0].Rows.Count > 0)
            label_currency.Text = "K" + ds_currency.Tables[0].Rows[0][0].ToString();
        else
            label_currency.Text = "Error";
    }

    protected DataSet getProductBySegment(string segmentID)
    {
        string query_product = "SELECT ID,Abbr FROM [Product] INNER JOIN [Segment_Product] ON [Segment_Product].ProductID = [Product].ID "
                       + " WHERE SegmentID = " + segmentID + " AND [Product].Deleted = 0 AND [Segment_Product].Deleted = 0 ORDER BY Abbr";
        DataSet ds_product = helper.GetDataSet(query_product);
        return ds_product;
    }

    // add by zy 20110130 start
    protected void bindEditGV(GridView gv, int col)
    {
        if (gv.EditIndex != -1)
        {
            DropDownList ddList = new DropDownList();
            if (col == 4)
            {
                ddList.Attributes.Add("onchange", "listchange(this,'" + this.hidSubRegionID.ClientID + "');getCustomerListInfo(this);");
                getCountryByRSM(getRSMID().Trim(), ddList);
                for (int i = 0; i < ddList.Items.Count; i++)
                {
                    if (string.Equals(ddList.Items[i].Value, gv.Rows[gv.EditIndex].Cells[0].Text))
                    {
                        ddList.SelectedIndex = i;
                        this.hidSubRegionID.Value = ddList.SelectedValue;
                        break;
                    }
                }
            }
            // by daixuesong 20110526 item43 add start
            else if (col == 5) // Customer
            {
                ddList.AutoPostBack = false;
                ddList.ID = "ddlRowCustomerID";
                if (!countryID.Equals(""))
                {
                    ddList.Attributes.Add("onchange", "listchange(this,'" + this.HiddenCustomer.ClientID + "');setRCOBState(this);");
                    bind(getCustomerByCountryID1(countryID), ddList);
                    // DropDownList默认值


                    for (int i = 0; i < ddList.Items.Count; i++)
                    {
                        if (string.Equals(ddList.Items[i].Value, gv.Rows[gv.EditIndex].Cells[1].Text))
                        {
                            ddList.SelectedIndex = i;
                            this.HiddenCustomer.Value = ddList.SelectedValue;
                            break;
                        }
                    }
                }
            }
            // by daixuesong 20110526 item43 add end
            else if (col == 6) // Project
            {
                ddList.ID = "ddl123";
                ddList.Attributes.Add("onchange", "listchange(this,'" + this.hidproject.ClientID + "')");
                bind(getAllProject(), ddList);
                // DropDownList default value


                for (int i = 0; i < ddList.Items.Count; i++)
                {
                    if (string.Equals(ddList.Items[i].Value, gv.Rows[gv.EditIndex].Cells[2].Text))
                    {
                        ddList.SelectedIndex = i;
                        this.hidproject.Value = ddList.SelectedValue;
                        break;
                    }
                }
            }
            else if (col == 7) // SalesChannel
            {
                ddList.Attributes.Add("onchange", "listchange(this,'" + this.hidsale.ClientID + "');setRCOBState(this);");
                bind(sql.getSalesChannelInfo(), ddList);
                // DropDownList default value


                for (int i = 0; i < ddList.Items.Count; i++)
                {
                    if (string.Equals(ddList.Items[i].Value, gv.Rows[gv.EditIndex].Cells[3].Text))
                    {
                        ddList.SelectedIndex = i;
                        this.hidsale.Value = ddList.SelectedValue;
                        break;
                    }
                }
            }
            ddList.Width = 180;
            // By DingJunjie 20110523 Item 7 Add Start
            gv.Rows[gv.EditIndex].Cells[col].Text = "";
            // By DingJunjie 20110523 Item 7 Add End
            gv.Rows[gv.EditIndex].Cells[col].Controls.Clear();
            gv.Rows[gv.EditIndex].Cells[col].Controls.Add(ddList);
        }
    }

    protected void BindOperationIds()
    {
        if (gv_bookingbydatebyproduct.EditIndex != -1)
        {
            int rIndex = gv_bookingbydatebyproduct.EditIndex;
            Dictionary<string, string> hidValues = new Dictionary<string, string>();
            
            for (int j = 11; j < gv_bookingbydatebyproduct.Columns.Count - 3; j += 7)
            {
                
                string productId = gv_bookingbydatebyproduct.Rows[rIndex].Cells[j].Text;
                string operationId = gv_bookingbydatebyproduct.Rows[rIndex].Cells[j+2].Text;
                string id = string.Format("{0}{1}", preId, productId);
                if (!hidValues.ContainsKey(id))
                {
                    hidValues.Add(id, operationId);
                }
                else
                {
                    hidValues[id] = operationId;
                }
                
              // hidValues.Add(id, operationId);

                DropDownList dp = new DropDownList();
                dp.DataSource = getOperationBySegment(getSegmentID());
                dp.DataTextField = "opname";
                dp.DataValueField = "opid";
                dp.DataBind();
                dp.SelectedValue = operationId;
                dp.AutoPostBack = false;
                dp.Attributes.Add("onchange", "setOperation('" + HDOperationIds.ClientID + "','" + preId + productId + "',this);");


                gv_bookingbydatebyproduct.Rows[gv_bookingbydatebyproduct.EditIndex].Cells[j+3].Text = "";
                gv_bookingbydatebyproduct.Rows[gv_bookingbydatebyproduct.EditIndex].Cells[j+3].Controls.Clear();
                gv_bookingbydatebyproduct.Rows[gv_bookingbydatebyproduct.EditIndex].Cells[j+3].Controls.Add(dp);


            }
            SetHidValue(hidValues);
            
        }
    }
    // add by zy 20110130 end

    protected void SetHidValue(Dictionary<string, string> values)
    {
        if (values == null || values.Count < 1)
        {
            return;
        }
        string result = string.Empty;
        foreach (var item in values)
        {
            if (string.IsNullOrEmpty(result))
            {
                result = string.Format("{1}:{2}", preId, item.Key, item.Value);
            }
            else
                result += string.Format(";{1}:{2}", preId, item.Key, item.Value);
        }
        HDOperationIds.Value = result;
    }

    protected Dictionary<string, string> GetHidValue()
    {
        Dictionary<string, string> result = new Dictionary<string, string>();
        if (string.IsNullOrEmpty(HDOperationIds.Value))
            return result;
        else
        {
            string[] ids = HDOperationIds.Value.Trim().Split(';');
            foreach (var item in ids)
            {
                result.Add(item.Substring(3, item.LastIndexOf(':')-3), item.Substring(item.LastIndexOf(':')+1));
            }
            return result;
        }
    }

    /// <summary>
    /// Get booking sales data of every product
    /// </summary>
    /// <param name="dsPro">Products</param>
    /// <param name="bookingY">BookingY</param>
    /// <param name="deliverY">DeliverY</param>
    /// <returns>booking sales data of every product</returns>
    protected DataSet getBookingDataByDateByProduct(DataSet dsPro, string bookingY, string deliverY)
    {
        string strRSMID = getRSMID().Trim();
        string strSalesOrgID = getSalesOrgID().Trim();
        string strSegmentID = getSegmentID().Trim();
        string strSubregionID = getCountryID().Trim();
        string strRoleName = getRole();
        DataSet ds = null;
        if (dsPro != null && !string.IsNullOrEmpty(strRSMID) && !string.IsNullOrEmpty(strSalesOrgID) && !string.IsNullOrEmpty(strSegmentID))
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine(" SELECT ");
            sql.AppendLine("   [Bookings].CountryID AS SubRegionID, ");
            sql.AppendLine("   [Bookings].CustomerID, ");
            sql.AppendLine("   [Bookings].ProjectID, ");
            sql.AppendLine("   [Bookings].SalesChannelID, ");
            sql.AppendLine("   [Bookings].ProductID, ");
            sql.AppendLine("   [Bookings].OperationID, ");
            sql.AppendLine("   [Bookings].BookingY, ");
            sql.AppendLine("   [Bookings].DeliverY, ");
            sql.AppendLine("   [SubRegion].Name AS SubRegion, ");
            sql.AppendLine("   [CustomerName].Name AS Customer, ");
            sql.AppendLine("   [Project].Name AS Project, ");
            sql.AppendLine("   [SalesChannel].Name AS SalesChannel, ");
            sql.AppendLine("   [Country].ISO_Code AS Country, ");
            sql.AppendLine("   [Product].Abbr AS ProductAbbr, ");
            sql.AppendLine("   [Operation].Abbr AS Operation, ");
            sql.AppendLine("   [Bookings].Amount, ");
            sql.AppendLine("   [Bookings].Value, ");
            sql.AppendLine("   [Bookings].Percentage, ");
            sql.AppendLine("   [Bookings].Comments, ");
            sql.AppendLine("   [Bookings].RecordID");
            sql.AppendLine(" FROM ");
            sql.AppendLine("   [Bookings] ");
            sql.AppendLine("   INNER JOIN [SubRegion] ON [Bookings].CountryID=[SubRegion].ID ");
            sql.AppendLine("   INNER JOIN [Customer] ON [Customer].ID=[Bookings].CustomerID ");
            sql.AppendLine("   INNER JOIN [CustomerName] ON [Customer].NameID=[CustomerName].ID ");
            sql.AppendLine("   INNER JOIN [Operation] ON [Bookings].OperationID=[Operation].ID ");
            sql.AppendLine("   INNER JOIN [Product] ON [Bookings].ProductID=[Product].ID ");
            sql.AppendLine("   LEFT JOIN [Country_SubRegion] ON [Bookings].CountryID=[Country_SubRegion].SubRegionID AND [Country_SubRegion].Deleted=0 ");
            sql.AppendLine("   LEFT JOIN [Country] ON [Country].ID=[Country_SubRegion].CountryID ");
            sql.AppendLine("   LEFT JOIN [Project] ON [Project].ID=[Bookings].ProjectID and [Project].Deleted=0 ");
            sql.AppendLine("   LEFT JOIN [SalesChannel] ON [SalesChannel].ID=[Bookings].SalesChannelID ");
            sql.AppendLine(" WHERE ");
            sql.AppendLine("   RSMID=" + strRSMID);
            sql.AppendLine("   AND [Bookings].SalesOrgID=" + strSalesOrgID);
            sql.AppendLine("   AND [Bookings].SegmentID=" + strSegmentID);
            if (!string.IsNullOrEmpty(strSubregionID) && !string.Equals(strSubregionID, "-1"))
            {
                sql.AppendLine("   AND [Bookings].CountryID=" + strSubregionID);
            }
            else if (!string.Equals(strRoleName, "Administrator"))
            {
                sql.AppendLine("   AND [Bookings].CountryID IN (" + getCountrySQL() + ")");
            }
            sql.AppendLine("   AND YEAR([Bookings].TimeFlag)=" + year);
            sql.AppendLine("   AND MONTH([Bookings].TimeFlag)=" + month);
            sql.AppendLine(" ORDER BY ");
            sql.AppendLine("    [SubRegion].Name, ");
            sql.AppendLine("    [CustomerName].Name, ");
            sql.AppendLine("    [Project].Name, ");
            sql.AppendLine("    [SalesChannel].Name, ");
            sql.AppendLine("    [Bookings].bookingY, ");
            sql.AppendLine("    [Bookings].DeliverY, ");
            sql.AppendLine("    [Product].Abbr ");
            ds = helper.GetDataSet(sql.ToString());
            if (ds != null && ds.Tables.Count > 0)
            {
                DataTable dt = ds.Tables[0].Clone();
                dt.Columns.RemoveAt(4);
                dt.Columns.RemoveAt(4);
                dt.Columns.RemoveAt(4);
                dt.Columns.RemoveAt(4);
                dt.Columns.RemoveAt(dt.Columns.Count - 1-1);
                dt.Columns.RemoveAt(dt.Columns.Count - 1-1);
                dt.Columns.RemoveAt(dt.Columns.Count - 1-1);
                dt.Columns.RemoveAt(dt.Columns.Count - 1-1);
                dt.Columns.RemoveAt(dt.Columns.Count - 1-1);
                dt.Columns.RemoveAt(dt.Columns.Count - 1-1);
                dt.Columns.Add("Total");
                for (int i = 0; i < dsPro.Tables[0].Rows.Count; i++)
                {
                    dt.Columns.Add("ProductID" + i);
                    dt.Columns.Add(dsPro.Tables[0].Rows[i][1].ToString());
                    dt.Columns.Add("OperationID" + i);
                    dt.Columns.Add("OperationAbbr" + i);
                    dt.Columns.Add("Value" + i);
                    dt.Columns.Add("Percentage" + i);
                    dt.Columns.Add("Comments" + i);
                }
                DataRow[] rows = null;
                DataRow row = null;
                string where = null;
                int index = 0;
                double sum = 0.0;
                List<object> productIDList = new List<object>();
                List<object> operationIDList = new List<object>();
                List<object> operationNameList = new List<object>();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    where = "SubRegionID=" + ds.Tables[0].Rows[i][0] + " AND CustomerID=" + ds.Tables[0].Rows[i][1]
                        + " AND ProjectID=" + ds.Tables[0].Rows[i][2] + " AND SalesChannelID=" + ds.Tables[0].Rows[i][3] + " and RecordID="+ds.Tables[0].Rows[i][19];
                    rows = dt.Select(where);
                    if (rows.Length == 0)
                    {
                        row = dt.NewRow();
                        row[0] = ds.Tables[0].Rows[i][0];
                        row[1] = ds.Tables[0].Rows[i][1];
                        row[2] = ds.Tables[0].Rows[i][2];
                        row[3] = ds.Tables[0].Rows[i][3];
                        row[4] = ds.Tables[0].Rows[i][8];
                        row[5] = ds.Tables[0].Rows[i][9];
                        row[6] = ds.Tables[0].Rows[i][10];
                        row[7] = ds.Tables[0].Rows[i][11];
                        row[8] = ds.Tables[0].Rows[i][12];
                        row[9] = ds.Tables[0].Rows[i][19];
                        productIDList.Clear();
                        operationIDList.Clear();
                        operationNameList.Clear();
                        rows = ds.Tables[0].Select(where);
                        for (int j = 0; j < rows.Length; j++)
                        {
                            if (!productIDList.Contains(rows[j][4]))
                            {
                                productIDList.Add(rows[j][4]);
                                operationIDList.Add(rows[j][5].ToString());
                                operationNameList.Add(rows[j][14].ToString().Trim());
                            }
                        }
                        rows = ds.Tables[0].Select(where + " AND bookingY='" + bookingY + "' AND DeliverY='" + deliverY + "'");
                        if (rows.Length == 0)
                        {
                            index = 0;
                            for (int j = 0; j < dsPro.Tables[0].Rows.Count; j++)
                            {
                                if (productIDList.Contains(dsPro.Tables[0].Rows[j][0]))
                                {
                                    row[10 + j * 7 + 1] = productIDList[index];
                                    row[10 + j * 7 + 2] = "0";
                                    row[10 + j * 7 + 3] = operationIDList[index];
                                    row[10 + j * 7 + 4] = operationNameList[index];
                                    row[10 + j * 7 + 5] = string.Empty;
                                    row[10 + j * 7 + 6] = string.Empty;
                                    row[10 + j * 7 + 7] = string.Empty;
                                    index++;
                                }
                                else
                                {
                                    row[10 + j * 7 + 1] = dsPro.Tables[0].Rows[j][0];
                                    row[10 + j * 7 + 2] = "0";
                                    row[10 + j * 7 + 3] = string.Empty;
                                    row[10 + j * 7 + 4] = string.Empty;
                                    row[10 + j * 7 + 5] = string.Empty;
                                    row[10 + j * 7 + 6] = string.Empty;
                                    row[10 + j * 7 + 7] = string.Empty;
                                }
                            }
                        }
                        else
                        {
                            index = 0;
                            for (int j = 0; j < dsPro.Tables[0].Rows.Count; j++)
                            {

                                //Edit by sino bug35、36、38 begin

                                if (index >= rows.Length)
                                {
                                    break;
                                }
                                //Edit by sino bug35、36、38 end
                                if (productIDList.Contains(dsPro.Tables[0].Rows[j][0]))
                                {
                                    row[10 + j * 7 + 1] = rows[index][4];
                                    row[10 + j * 7 + 2] = rows[index][15];
                                    row[10 + j * 7 + 3] = rows[index][5];
                                    row[10 + j * 7 + 4] = rows[index][14];
                                    row[10 + j * 7 + 5] = rows[index][16];
                                    row[10 + j * 7 + 6] = rows[index][17];
                                    row[10 + j * 7 + 7] = rows[index][18];
                                    index++;
                                }
                                else
                                {
                                    row[10 + j * 7 + 1] = dsPro.Tables[0].Rows[j][0];
                                    row[10 + j * 7 + 2] = "0";
                                    row[10 + j * 7 + 3] = string.Empty;
                                    row[10 + j * 7 + 4] = string.Empty;
                                    row[10 + j * 7 + 5] = string.Empty;
                                    row[10 + j * 7 + 6] = string.Empty;
                                    row[10 + j * 7 + 7] = string.Empty;
                                }
                            }
                        }
                        dt.Rows.Add(row);
                        sum = 0.0;
                        for (int j = 12; j < dt.Columns.Count; j += 7)
                        {
                            //Edit by sino bug35、36、38 begin
                            if (row[j].ToString() != null && row[j].ToString() != "")
                            {
                                sum += Convert.ToDouble(row[j]);
                            }
                            //Edit by sino bug35、36、38 end

                            
                        }
                        row[10] = sum;
                    }
                }
                ds.Tables.Clear();
                ds.Tables.Add(dt);
            }
            else
            {
                return null;
            }
        }
        return ds;
    }

    protected void bindDataByDateByProduct(GridView gv, string bookingY, string deliverY)
    {
        bool flag = true;
        DataSet ds_product = getProductBySegment(getSegmentID());
        if (ds_product == null)
        {
            gv.Visible = false;
            return;
        }
        
        DataSet ds = getBookingDataByDateByProduct(ds_product, bookingY, deliverY);
        if (ds == null)
        {
            return;
        }
        if (ds.Tables[0].Rows.Count == 0)
        {
            flag = false;
            butFlag = false;
            sql.getNullDataSet(ds);
        }
        else
        {
            butFlag = true;
        }
        gv.Width = Unit.Pixel(1000);
        gv.AutoGenerateColumns = false;
        gv.AllowPaging = false;
        for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
        {
            BoundField bf = new BoundField();
            bf.DataField = ds.Tables[0].Columns[i].ColumnName.ToString();
            bf.HeaderText = ds.Tables[0].Columns[i].Caption.ToString();
            bf.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
            bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
            bf.HeaderStyle.Wrap = false;
            bf.ReadOnly = true;
            if (i > 9)
            {
                if (i % 7 == 0)
                {
                    bf.HeaderText = null;
                    bf.ReadOnly = true;
                    //bf.ControlStyle.Width = 15;
                    //bf.ItemStyle.Width = 15;
                }
                else if (i % 7 == 5)
                {
                    bf.ItemStyle.HorizontalAlign = HorizontalAlign.Right;
                    bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Right;
                    bf.ControlStyle.Width = 50;
                    bf.ItemStyle.Width = 50;
                    bf.ReadOnly = false;
                }
                else
                {
                    bf.ReadOnly = true;
                }
            }
            else
            {
                bf.ReadOnly = true;
            }
            gv.Columns.Add(bf);
        }
        if (butFlag)
        {
            CommandField cf_Update = new CommandField();
            cf_Update.ButtonType = ButtonType.Image;
            cf_Update.ShowEditButton = true;
            cf_Update.ShowCancelButton = true;
            cf_Update.EditImageUrl = "~/images/edit.jpg";
            cf_Update.EditText = "Edit";
            cf_Update.CausesValidation = false;
            cf_Update.CancelImageUrl = "~/images/cancel.jpg";
            cf_Update.CancelText = "Cancel";
            cf_Update.UpdateImageUrl = "~/images/ok.jpg";
            cf_Update.UpdateText = "OK";
            gv.Columns.Add(cf_Update);
            CommandField cf_Delete = new CommandField();
            cf_Delete.ButtonType = ButtonType.Image;
            cf_Delete.ShowDeleteButton = true;
            cf_Delete.DeleteImageUrl = "~/images/del.jpg";
            cf_Delete.DeleteText = "Delete";
            cf_Delete.CausesValidation = false;
            gv.Columns.Add(cf_Delete);
            CommandField cf_Comments = new CommandField();
            cf_Comments.ButtonType = ButtonType.Image;
            cf_Comments.ShowSelectButton = true;
            cf_Comments.ShowCancelButton = true;
            cf_Comments.SelectImageUrl = "~/images/comments.jpg";
            cf_Comments.SelectText = "Comments";
            cf_Comments.CausesValidation = false;
            gv.Columns.Add(cf_Comments);
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
        gv.Columns[9].Visible = false;
        for (int i = 11; i < gv.Columns.Count - 3; i += 7)
        {
            gv.Columns[i].Visible = false;
            gv.Columns[i + 2].Visible = false;
            gv.Columns[i + 4].Visible = false;
            gv.Columns[i + 5].Visible = false;
            gv.Columns[i + 6].Visible = false;
        }
        if (flag)
        {
            if (string.Equals(getRoleID(getRole()), "0")
                || string.Equals(getRoleID(getRole()), "3")
                || string.Equals(getRoleID(getRole()), "4"))
            {
                gv.Columns[gv.Columns.Count - 1].Visible = true;
                gv.Columns[gv.Columns.Count - 2].Visible = true;
                gv.Columns[gv.Columns.Count - 3].Visible = true;
                string str_data = null;
                string str_comments = null;
                if (gv.Rows.Count > 0)
                {
                    for (int i = 0; i < gv.Rows.Count; i++)
                    {
                        for (int j = 11; j < gv.Columns.Count - 3; j += 7)
                        {
                            str_data = gv.Rows[i].Cells[j + 1].Text.Replace("&nbsp;", string.Empty);
                            str_comments = gv.Rows[i].Cells[j + 6].Text.Replace("&nbsp;", string.Empty);
                            if (!string.Equals(str_data, "0") && str_comments.Length != 0)
                            {
                                gv.Rows[i].Cells[j + 1].ForeColor = Color.Red;
                                gv.Rows[i].Cells[j + 1].ToolTip = str_comments;
                            }
                        }
                    }
                }
            }
            else
            {
                gv.Columns[gv.Columns.Count - 1].Visible = false;
                gv.Columns[gv.Columns.Count - 2].Visible = false;
                gv.Columns[gv.Columns.Count - 3].Visible = false;
            }
        }
    }

    protected void getCountryByRSM(string RSMID, DropDownList ddlist)////ddlist_country
    {
        // update by zy 20110128 start
        //string query_country = "SELECT [Country].Name,[Country].ID FROM [User_Country] "
        //                    + " INNER JOIN [Country] ON [Country].ID = [User_Country].CountryID"
        //                    + " WHERE UserID = " + RSMID
        //                    + " AND [User_Country].Deleted = 0 AND [Country].Deleted = 0"
        //                    + " GROUP BY [Country].Name,[Country].ID ";

        //By Mbq 20110505 ITEM 13 Del Start
        /** string query_country = "SELECT [SubRegion].ID, [SubRegion].Name FROM [Country_SubRegion] "
                            + " INNER JOIN [SubRegion] ON [SubRegion].ID = [Country_SubRegion].SubregionID "
                            + " INNER JOIN [Country] ON [Country].ID = [Country_SubRegion].CountryID"
                            + " INNER JOIN [User_Country] ON [Country].ID = [User_Country].CountryID"
                            + " WHERE  [SubRegion].Deleted = 0 AND [Country_SubRegion].Deleted=0"
                            + " AND [Country].Deleted = 0 AND [User_Country].Deleted = 0 AND UserID = " + RSMID
                            + " GROUP BY [SubRegion].ID, [SubRegion].Name";
         **/
        //By Mbq 20110505 ITEM 13 Del Start


        //By Mbq 20110505 ITEM 13 ADD Start
        string query_country = "";

        query_country = " SELECT [SubRegion].ID, [SubRegion].Name AS SubRegion FROM [User_Country] ";
        query_country = query_country + " INNER JOIN [SubRegion] ";
        query_country = query_country + " ON [User_Country].CountryID = [SubRegion].ID";
        query_country = query_country + " WHERE [SubRegion].Deleted = 0 AND [User_Country].UserID  = '" + getRSMID() + "'";
        if (!getCountryID().Trim().Equals("-1") && !getCountryID().Trim().Equals(""))
        {
            query_country = query_country + " AND [SubRegion].ID = " + getCountryID().Trim();
        }
        else
        {
            query_country = query_country + " AND [SubRegion].ID IN (" + getCountryGVConditionSQL() + ")";
        }
        query_country = query_country + " AND [User_Country].Deleted = 0";
        query_country = query_country + " ORDER BY Name ASC";
        DataSet ds_country = helper.GetDataSet(query_country);
        if (ds_country.Tables[0].Rows.Count > 0)
        {
            DataTable dt = ds_country.Tables[0];
            int count = dt.Rows.Count;
            int index = 0;
            while (index < count)
            {
                ListItem li = new ListItem(dt.Rows[index][1].ToString().Trim(), dt.Rows[index][0].ToString().Trim());
                ddlist.Items.Add(li);
                index++;
            }
            ddlist.SelectedIndex = 0;
            ddlist.Enabled = true;
        }
    }

    // add by zy 20101224 start
    protected DataSet getCustomerByCountryID(string countryID)
    {
        // update by zy 20110128 start
        //string query_customer = "SELECT [CustomerName].ID, [CustomerName].Name"
        //                        + " FROM [Customer] INNER JOIN [CustomerName] ON [Customer].NameID = [CustomerName].ID "
        //                        + " WHERE [Customer].Deleted = 0 AND [CustomerName].Deleted = 0"
        //                        + " AND [Customer].CountryID = " + countryID
        //                        + " ORDER BY [CustomerName].Name";
        // By DingJunjie 20110519 Item 7 Delete Start
        //string query_customer = "SELECT [CustomerName].ID, [CustomerName].Name"
        //                        + " FROM [Customer] INNER JOIN [CustomerName] ON [Customer].NameID = [CustomerName].ID "
        //                        + " INNER JOIN [Country_SubRegion] ON [Country_SubRegion].CountryID = [Customer].CountryID"
        //                        + " INNER JOIN [SubRegion] ON [SubRegion].ID = [Country_SubRegion].SubRegionID"
        //                        + " INNER JOIN [Country] ON [Country].ID = [Country_SubRegion].CountryID"
        //                        + " WHERE [Customer].Deleted = 0 AND [CustomerName].Deleted = 0"
        //                        + " AND [Country].Deleted = 0 AND [Country_SubRegion].Deleted = 0"
        //                        + " AND [SubRegion].Deleted = 0"
        //                        + " AND [Customer].CountryID = " + countryID
        //                        + " ORDER BY [CustomerName].Name";
        // By DingJunjie 20110519 Item 7 Delete End
        // By DingJunjie 20110519 Item 7 Add Start
        StringBuilder sql = new StringBuilder();
        sql.AppendLine(" SELECT ");
        sql.AppendLine("   CustomerName.ID, ");
        sql.AppendLine("   CustomerName.Name+'('+CustomerType.Name+')' AS NAME ");
        sql.AppendLine(" FROM ");
        sql.AppendLine("   Customer ");
        sql.AppendLine("   INNER JOIN CustomerName ON Customer.NameID=CustomerName.ID ");
        sql.AppendLine("   INNER JOIN SubRegion ON SubRegion.ID=Customer.CountryID ");
        sql.AppendLine("   INNER JOIN CustomerType ON Customer.TypeID=CustomerType.ID ");
        sql.AppendLine(" WHERE ");
        sql.AppendLine("   Customer.Deleted=0 ");
        sql.AppendLine("   AND CustomerName.Deleted=0 ");
        sql.AppendLine("   AND SubRegion.Deleted=0 ");
        sql.AppendLine("   AND CustomerType.Deleted=0 ");
        sql.AppendLine("   AND Customer.CountryID=" + countryID);
        sql.AppendLine(" ORDER BY ");
        sql.AppendLine("   CustomerName.Name ");
        string query_customer = sql.ToString();
        // By DingJunjie 20110519 Item 7 Add End
        // update by zy 20110128 end
        DataSet ds_customer = helper.GetDataSet(query_customer);

        if (ds_customer != null && ds_customer.Tables.Count > 0 && ds_customer.Tables[0].Rows.Count > 0)
        {
            return ds_customer;
        }
        else
        {
            return null;
        }
    }

    protected DataSet getCustomerByCountryID1(string countryID)
    {
        
        StringBuilder sql = new StringBuilder();
        sql.AppendLine(" SELECT ");
        sql.AppendLine("   Customer.ID, ");
        sql.AppendLine("   CustomerName.Name+'('+CustomerType.Name+')' AS NAME ");
        sql.AppendLine(" FROM ");
        sql.AppendLine("   Customer ");
        sql.AppendLine("   INNER JOIN CustomerName ON Customer.NameID=CustomerName.ID ");
        sql.AppendLine("   INNER JOIN SubRegion ON SubRegion.ID=Customer.CountryID ");
        sql.AppendLine("   INNER JOIN CustomerType ON Customer.TypeID=CustomerType.ID ");
        sql.AppendLine(" WHERE ");
        sql.AppendLine("   Customer.Deleted=0 ");
        sql.AppendLine("   AND CustomerName.Deleted=0 ");
        sql.AppendLine("   AND SubRegion.Deleted=0 ");
        sql.AppendLine("   AND CustomerType.Deleted=0 ");
        sql.AppendLine("   AND Customer.CountryID=" + countryID);
        sql.AppendLine(" ORDER BY ");
        sql.AppendLine("   CustomerName.Name ");
        string query_customer = sql.ToString();
        
        DataSet ds_customer = helper.GetDataSet(query_customer);

        if (ds_customer != null && ds_customer.Tables.Count > 0 && ds_customer.Tables[0].Rows.Count > 0)
        {
            return ds_customer;
        }
        else
        {
            return null;
        }
    }

    protected DataSet getProjectByCustomer(string customerID)
    {
        string query_project = "SELECT [Project].ID, [Project].Name"
                                + " FROM [Project] "
                                + " WHERE [Project].CustomerNameID = " + customerID
                                + " AND [Project].Deleted = 0"
                                + " ORDER BY [Project].Name";
        DataSet ds_project = helper.GetDataSet(query_project);

        if (ds_project.Tables[0].Rows.Count > 0)
        {
            return ds_project;
        }
        else
        {
            return null;
        }
    }

    // add by zy 20110130 start
    protected DataSet getAllProject()
    {
        string query_project = "SELECT [Project].ID, [Project].Name"
                                + " FROM [Project] "
                                + " WHERE [Project].Deleted = 0"
                                + " GROUP BY [Project].ID,[Project].Name"
                                + " ORDER BY [Project].Name";
        DataSet ds_project = helper.GetDataSet(query_project);

        if (ds_project.Tables[0].Rows.Count > 0)
        {
            return ds_project;
        }
        else
        {
            return null;
        }
    }
    // add by zy 20110130 end

    protected void bind(DataSet ds, DropDownList ddlist)
    {
        

        ListItem lib = new ListItem("", "-1");
        ddlist.Items.Add(lib);

        if (ds != null)
        {
            DataTable dt = ds.Tables[0];
            int count = dt.Rows.Count;
            int index = 0;
            while (index < count)
            {
                ListItem li = new ListItem(dt.Rows[index][1].ToString().Trim(), dt.Rows[index][0].ToString().Trim());
                ddlist.Items.Add(li);
                index++;
            }
            ddlist.SelectedIndex = 0;
            ddlist.Enabled = true;
        }
        else
        {
            ddlist.Enabled = false;
        }
    }

    protected void ddlist_countryYTD_SelectedIndexChanged(object sender, EventArgs e)
    {
        ddlist_customerYTD.Items.Clear();
        // By DingJunjie 20110512 Item 7 Delete Start
        //ddlist_projectYTD.Items.Clear();
        // By DingJunjie 20110512 Item 7 Delete End
        bind(getCustomerByCountryID1(ddlist_countryYTD.SelectedValue.ToString().Trim()), ddlist_customerYTD);
        // By DingJunjie 20110512 Item 7 Delete Start
        //bind(getProjectByCustomer(ddlist_customerYTD.SelectedValue.ToString().Trim()), ddlist_projectYTD);
        // By DingJunjie 20110512 Item 7 Delete End
    }

    //protected void ddlist_customerYTD_SelectedIndexChanged(object sender, EventArgs e)
    //{
    //    // By DingJunjie 20110512 Item 7 Delete Start
    //    //ddlist_projectYTD.Items.Clear();
    //    //bind(getProjectByCustomer(ddlist_customerYTD.SelectedValue.ToString().Trim()), ddlist_projectYTD);
    //    // By DingJunjie 20110512 Item 7 Delete End
    //}

    // add customerYTD
    protected void lbtn_addcustomerYTD_Click(object sender, EventArgs e)
    {
        // By DingJunjie 20110517 Item 7 Add Start
        this.hidButtomFlag.Value = "true";
        // By DingJunjie 20110517 Item 7 Add End
        lbtn_addcustomerYTD.Text = "Select customer name, type and country";
        lbtn_addcustomerYTD.Enabled = false;
        panel_addcustomerYTD.Visible = true;
        clearCustomerText(tbox_customeraddressYTD, tbox_customercityYTD, tbox_customerdepartmentYTD);

        ddlist_customertypeYTD.Items.Clear();
        // add by zy 20110121 start
        ddlist_saleschannelYTD.Items.Clear();
        // add by zy 20110121 end
        ddlist_customercountryYTD.Items.Clear();
        // add by zy 20110119 start
        //add_ddlist_empty(ddlist_customertypeYTD);
        txt_customernameYTD.Text = "";
        label_addYTD.Text = "";
        // add by zy 20110119 end

        bindDrop(sql.getCustomerType(), ddlist_customertypeYTD);
        // add by zy 20110121 start
        //bindDrop(sql.getSalesChannelInfo(), ddlist_saleschannelYTD);
        // add by zy 20110121 end

        //bindDrop(sql.getCountryInfo(), ddlist_customercountryYTD);
        getCountryByRSM(getRSMID().ToString().Trim(), ddlist_customercountryYTD);
        ddlist_customercountryYTD.SelectedIndex = ddlist_countryYTD.SelectedIndex;

        lbtn_addprojectYTD.Enabled = false;
        // By DingJunjie 20110517 Item 7 Add Start
        this.lbtn_modifyprojectYTD.Enabled = false;
        // By DingJunjie 20110517 Item 7 Add End
        this.lnkButtonModifyCustmer.Enabled = false;
    }
    // add by zy 20110119 start
    protected void add_ddlist_empty(DropDownList ddList)
    {
        ListItem item = new ListItem("", "");
        ddList.Items.Insert(0, item);
    }
    // add by zy 20110119 end

    protected void btn_customerYTD_add_Click(object sender, EventArgs e)
    {
        lbtn_addcustomerYTD.Text = "Add Customer";
        lbtn_addcustomerYTD.Enabled = true;
        panel_addcustomerYTD.Visible = false;
        lbtn_addprojectYTD.Enabled = true;
        // By DingJunjie 20110517 Item 7 Add Start
        lbtn_modifyprojectYTD.Enabled = true;
        // By DingJunjie 20110517 Item 7 Add End

        // By DingJunjie 20110608 ItemW18 Add Start
        label_addYTD.Visible = true;
        // By DingJunjie 20110608 ItemW18 Add End
        lnkButtonModifyCustmer.Enabled = true;

        // add by zy 20110119 start
        if (!check_customer(this.hidOldCusName, txt_customernameYTD, label_addYTD, "Customer Name"))
        {
            return;
        }
        if (!check_customer_double_input(this.hidOldCusName, txt_customernameYTD, label_addYTD, "Customer Name"))
        {
            return;
        }
        // add by zy 20110119 end

        label_addYTD.ForeColor = System.Drawing.Color.Red;
        // update by zy 20110119 start
        //string str_nameID = ddlist_customernameYTD.SelectedItem.Value.Trim();
        string str_name = string.IsNullOrEmpty(this.txt_customernameYTD.Text.Trim()) ?
            this.txtOldCusName.Text.Trim() : this.txt_customernameYTD.Text.Trim();
        // update by zy 20110119 end
        string str_typeID = ddlist_customertypeYTD.SelectedItem.Value.Trim();
        // add by zy 20110121 start
        //string str_salesChannelID = ddlist_saleschannelYTD.SelectedItem.Value.Trim();
        string str_salesChannelID = "0";
        // add by zy 20110121 end
        string str_countryID = ddlist_customercountryYTD.SelectedItem.Value.Trim();
        string str_address = tbox_customeraddressYTD.Text.Trim();
        string str_city = tbox_customercityYTD.Text.Trim();
        string str_department = tbox_customerdepartmentYTD.Text.Trim();
        // By DingJunjie 20110519 Item 7 Delete Start
        //add_customer(str_name, str_typeID, str_salesChannelID, str_countryID, str_address, str_city, str_department, label_addYTD, ddlist_customernameYTD);
        // By DingJunjie 20110519 Item 7 Delete End
        // By DingJunjie 20110519 Item 7 Add Start
        int flag = add_customer1(str_name, str_typeID, str_salesChannelID, str_countryID, str_address, str_city, str_department, label_addYTD, txtOldCusName);
        this.ddlist_countryYTD.SelectedValue = this.ddlist_customercountryYTD.SelectedValue;
        // By DingJunjie 20110519 Item 7 Add End
        // ddlist bindDataSource
        ddlist_customerYTD.Items.Clear();
        // By DingJunjie 20110512 Item 7 Delete Start
        //ddlist_projectYTD.Items.Clear();
        // By DingJunjie 20110512 Item 7 Delete End
        bind(getCustomerByCountryID1(ddlist_countryYTD.SelectedValue.ToString().Trim()), ddlist_customerYTD);
        // By DingJunjie 20110512 Item 7 Delete Start
        //bind(getProjectByCustomer(ddlist_customerYTD.SelectedValue.ToString().Trim()), ddlist_projectYTD);
        // By DingJunjie 20110512 Item 7 Delete End
        // By DingJunjie 20110519 Item 7 Add Start


        //if (flag)
        //{
        //    // By DingJunjie 20110520 Item 7 Add Start
        //    if (string.IsNullOrEmpty(this.hidOldCusName.Value))
        //    {
        //        // By DingJunjie 20110520 Item 7 Add End
        //        object id = helper.ExecuteScalar(CommandType.Text, "SELECT MAX(ID) FROM CustomerName", null);
        //        this.ddlist_customerYTD.SelectedValue = Convert.ToString(id);
        //        // By DingJunjie 20110520 Item 7 Add Start
        //    }
        //    else
        //    {
        //        this.ddlist_customerYTD.SelectedValue = this.hidOldCusName.Value;
        //    }
        //    // By DingJunjie 20110520 Item 7 Add End
        //}

        this.ddlist_customerYTD.SelectedValue = flag.ToString(CultureInfo.InvariantCulture);
        this.hidButtomFlag.Value = "true";
        // By DingJunjie 20110519 Item 7 Add End
    }

    //protected int GetCustomerId(str_name, str_typeID, str_salesChannelID, str_countryID, str_address, str_city, str_department, label_addYTD, txtOldCusName)
    //{ 
        
    //}

     protected int add_customer1(string str_name, string str_typeID, string str_salesChannelID, string str_countryID,
        string str_address, string str_city, string str_department, Label label_add, TextBox txtOldCusName)
     {
        
        string customerID = GetCustomernameID(str_name);
        if (customerID == "-1")
        {
            bool bSuccess = sql.insertCustomerName(str_name);
            if (!bSuccess)
            {
                label_add.Text = info.addExist(str_name);
                // By DingJunjie 20110519 Item 7 Delete Start
                //return;
                // By DingJunjie 20110519 Item 7 Delete End
                // By DingJunjie 20110519 Item 7 Add Start
                //return false;
                // By DingJunjie 20110519 Item 7 Add End

                return -1;
            }
            customerID = GetCustomernameID(str_name);
        }
        int resultID=-1;
        if (!Exist_customer(customerID, str_typeID, str_countryID,out resultID))
        {
            string add_country = "INSERT INTO [Customer](NameID, TypeID, SalesChannelID, CountryID, City, Address, Department, Deleted)"
                // update by zy 20110119 start
                //+ " VALUES('" + str_nameID + "','" + str_typeID + "','" + str_countryID + "','" + str_city + "','" + str_address + "','" + str_department + "','0')";
                        + " VALUES('" + customerID + "','" + str_typeID + "','" + str_salesChannelID + "','" + str_countryID + "','" + str_city + "','" + str_address + "','" + str_department + "','0');select @@IDENTITY; ";
            // update by zy 20110119 start
            //int add_count = helper.ExecuteNonQuery(CommandType.Text, add_country, null);
            int add_count=-1;
            DataSet ds= helper.GetDataSet(add_country);
            if(ds!=null&&ds.Tables[0].Rows.Count>0)
                add_count= Convert.ToInt32(ds.Tables[0].Rows[0][0]);


            if (add_count >0)
            {
                label_add.ForeColor = System.Drawing.Color.Green;
                label_add.Text = info.addLabelInfo(str_name, true);
                // By DingJunjie 20110519 Item 7 Add Start
                return add_count;
                // By DingJunjie 20110519 Item 7 Add End
            }
            else
            // By DingJunjie 20110519 Item 7 Add Start
            {
                // By DingJunjie 20110519 Item 7 Add End
                label_add.Text = info.addLabelInfo(str_name, false);
                // By DingJunjie 20110519 Item 7 Add Start
                return -1;
            }
            // By DingJunjie 20110519 Item 7 Add End
        }
        else
        // By DingJunjie 20110519 Item 7 Add Start
        {
            // By DingJunjie 20110519 Item 7 Add End
            label_add.Text = info.addExist(str_name);
            // By DingJunjie 20110519 Item 7 Add Start
            return resultID;
        }
        // By DingJunjie 20110519 Item 7 Add End
    }

    // By DingJunjie 20110519 Item 7 Delete Start
    //protected void add_customer(string str_name, string str_typeID, string str_salesChannelID, string str_countryID,
    // By DingJunjie 20110519 Item 7 Delete End
    // By DingJunjie 20110519 Item 7 Add Start
    //protected bool add_customer(string str_name, string str_typeID, string str_salesChannelID, string str_countryID,
    //    // By DingJunjie 20110519 Item 7 Add End
    //    string str_address, string str_city, string str_department, Label label_add, TextBox txtOldCusName)
    //{
    //    // add by zy 20110119 start
    //    // add customer name
    //    string customerID = GetCustomernameID(str_name);
    //    if (customerID == "-1")
    //    {
    //        bool bSuccess = sql.insertCustomerName(str_name);
    //        if (!bSuccess)
    //        {
    //            label_add.Text = info.addExist(str_name);
    //            // By DingJunjie 20110519 Item 7 Delete Start
    //            //return;
    //            // By DingJunjie 20110519 Item 7 Delete End
    //            // By DingJunjie 20110519 Item 7 Add Start
    //            return false;
    //            // By DingJunjie 20110519 Item 7 Add End
    //        }
    //        customerID = GetCustomernameID(str_name);
    //    }
    //    // add by zy 20110119 end

    //    // update by zy 20110119 start
    //    //if (!Exist_customer(str_nameID, str_typeID, str_countryID))
    //    if (!Exist_customer(customerID, str_typeID, str_countryID))

    //    // update by zy 20110119 end
    //    {
    //        string add_country = "INSERT INTO [Customer](NameID, TypeID, SalesChannelID, CountryID, City, Address, Department, Deleted)"
    //            // update by zy 20110119 start
    //            //+ " VALUES('" + str_nameID + "','" + str_typeID + "','" + str_countryID + "','" + str_city + "','" + str_address + "','" + str_department + "','0')";
    //                    + " VALUES('" + customerID + "','" + str_typeID + "','" + str_salesChannelID + "','" + str_countryID + "','" + str_city + "','" + str_address + "','" + str_department + "','0')";
    //        // update by zy 20110119 start
    //        int add_count = helper.ExecuteNonQuery(CommandType.Text, add_country, null);

    //        if (add_count == 1)
    //        {
    //            label_add.ForeColor = System.Drawing.Color.Green;
    //            label_add.Text = info.addLabelInfo(str_name, true);
    //            // By DingJunjie 20110519 Item 7 Add Start
    //            return true;
    //            // By DingJunjie 20110519 Item 7 Add End
    //        }
    //        else
    //        // By DingJunjie 20110519 Item 7 Add Start
    //        {
    //            // By DingJunjie 20110519 Item 7 Add End
    //            label_add.Text = info.addLabelInfo(str_name, false);
    //            // By DingJunjie 20110519 Item 7 Add Start
    //            return true;
    //        }
    //        // By DingJunjie 20110519 Item 7 Add End
    //    }
    //    else
    //    // By DingJunjie 20110519 Item 7 Add Start
    //    {
    //        // By DingJunjie 20110519 Item 7 Add End
    //        label_add.Text = info.addExist(str_name);
    //        // By DingJunjie 20110519 Item 7 Add Start
    //        return false;
    //    }
    //    // By DingJunjie 20110519 Item 7 Add End
    //}

    protected void btn_customerYTD_cancel_Click(object sender, EventArgs e)
    {
        // By DingJunjie 20110517 Item 7 Add Start
        this.hidButtomFlag.Value = "true";
        // By DingJunjie 20110517 Item 7 Add End
        lbtn_addcustomerYTD.Text = "Add Customer";
        lbtn_addcustomerYTD.Enabled = true;
        panel_addcustomerYTD.Visible = false;
        lbtn_addprojectYTD.Enabled = true;
        // By DingJunjie 20110517 Item 7 Add Start
        lbtn_modifyprojectYTD.Enabled = true;
        // By DingJunjie 20110517 Item 7 Add End
        lnkButtonModifyCustmer.Enabled = true;
    }

    // add by zy 20110119 start
    protected string GetCustomernameID(string str_name)
    {
        string query_customer = "SELECT ID FROM [CustomerName] WHERE  Name = '" + str_name
                            + "' AND Deleted = 0 ";
        DataSet ds_customer = helper.GetDataSet(query_customer);

        if (ds_customer.Tables[0].Rows.Count > 0)
            return ds_customer.Tables[0].Rows[0].ItemArray[0].ToString();
        else
            return "-1";
    }

    // add by zy 20110119 end
    protected bool Exist_customer(string str_nameID, string str_typeID, string str_countryID,out int customerId)
    {
        customerId = -1;
        string query_customer = "SELECT ID FROM [Customer] WHERE  NameID = '" + str_nameID
                            + "' AND TypeID = '" + str_typeID + "' AND CountryID = '" + str_countryID + "' AND Deleted = 0 ";
        DataSet ds_customer = helper.GetDataSet(query_customer);

        if (ds_customer.Tables[0].Rows.Count > 0)
        {
            customerId= Convert.ToInt32(ds_customer.Tables[0].Rows[0][0]);
            return true;
        }
        else
            return false;
    }

    private void clearCustomerText(TextBox tbox_address, TextBox tbox_city, TextBox tbox_department)
    {
        //label_add.Text = "";
        //label_del.Text = "";

        // By DingJunjie 20110608 ItemW18 Add Start
        this.txtOldCusName.Text = "";
        this.hidOldCusName.Value = "";
        // By DingJunjie 20110608 ItemW18 Add End

        tbox_address.Text = "";
        tbox_city.Text = "";
        tbox_department.Text = "";
    }

    /*add a projectYTD*/
    protected void lbtn_addprojectYTD_Click(object sender, EventArgs e)
    {
        // By DingJunjie 20110517 Item 7 Add Start
        this.hidButtomFlag.Value = "true";
        // By DingJunjie 20110517 Item 7 Add End
        lbtn_addprojectYTD.Text = "Input project name, value, propettitors and select project country, point of destination";
        lbtn_addprojectYTD.Enabled = false;
        panel_addprojectYTD.Visible = true;
        ddlist_project_fromcountryYTD.Items.Clear();
        ddlist_project_tocountryYTD.Items.Clear();
        ddlist_currencyYTD.Items.Clear();
        bindToCountry(ddlist_project_tocountryYTD);
        bindFromCountry(ddlist_project_fromcountryYTD);
        bindCurrency(ddlist_currencyYTD);
        //by yyan itemw73 20110714 edit start
        nullInput(tbox_project_nameYTD, tbox_project_probabilityYTD, tbox_project_valueYTD, txtComments);
        //by yyan itemw73 20110714 edit end
        //BY yyan 20110505 Item 11 add start
        ddlist_segmentYTD.Items.Clear();
        bindSegment(ddlist_segmentYTD);
        //BY yyan 20110505 Item 11 add end

        // By DingJunjie 20110512 Item 7 Add Start
        // Set [Project Name] and [Project Country(POD)]
        if (this.chkYTDIsCopy.Checked &&
            //by yyan itemw57 20110630 del start 
            //!string.IsNullOrEmpty(this.ddlist_projectYTD.SelectedValue)
            //&& !string.Equals(this.ddlist_projectYTD.SelectedValue, "-1"))
            //by yyan itemw57 20110630 del end 
            //by yyan itemw57 20110630 add start 
             !string.Equals(this.hidOldProName.Value, "-1"))
        //by yyan itemw57 20110630 add end 
        {
            //by yyan itemw57 20110630 del start 
            //DataSet ds = getProjectByID(this.ddlist_projectYTD.SelectedValue);
            //by yyan itemw57 20110630 del end 
            //by yyan itemw57 20110630 add start 
            DataSet ds = getProjectByID(this.hidOldProName.Value);
            //by yyan itemw57 20110630 add end 
            if (ds != null)
            {
                this.tbox_project_nameYTD.Text = ds.Tables[0].Rows[0][0].ToString();
                this.ddlist_project_tocountryYTD.SelectedValue = ds.Tables[0].Rows[0][1].ToString();
            }
        }
        // Set [Customer Name]
        if (!string.IsNullOrEmpty(this.ddlist_customerYTD.SelectedValue) && !string.Equals(this.ddlist_customerYTD.SelectedValue, "-1"))
        {
            this.txtProCusName.Text = this.ddlist_customerYTD.SelectedItem.Text;
            this.hidProCusName.Value = this.ddlist_customerYTD.SelectedValue;
        }
        // Set [Project Country(POS)] 
        if (!string.IsNullOrEmpty(this.ddlist_countryYTD.SelectedValue) && !string.Equals(this.ddlist_countryYTD.SelectedValue, "-1"))
        {
            this.ddlist_project_fromcountryYTD.SelectedValue = getCountryIDBySubRegion(this.ddlist_countryYTD.SelectedValue);
        }
        // By DingJunjie 20110512 Item 7 Add End

        label_addYTD.Visible = false;
        lbtn_addcustomerYTD.Enabled = false;
        // By DingJunjie 20110517 Item 7 Add Start
        this.lbtn_modifyprojectYTD.Enabled = false;
        // By DingJunjie 20110517 Item 7 Add End
        this.lnkButtonModifyCustmer.Enabled = false;

    }
    private void nullInput(TextBox tbox_name, TextBox tbox_probability, TextBox tbox_value, TextBox txtComments)
    {
        tbox_name.Text = "";
        tbox_probability.Text = "";
        tbox_value.Text = "";
        // By DingJunjie 20110608 ItemW18 Add Start
        this.txtProCusName.Text = "";
        this.hidProCusName.Value = "";
        // By DingJunjie 20110608 ItemW18 Add End
        //by yyan itemw73 20110714 add start
        txtComments.Text = "";
        //by yyan itemw73 20110714 add end
    }

    protected void bindcustomer(DropDownList ddlist_customer)
    {
        DataSet ds_customer = sql.getCustomerName();
        if (ds_customer.Tables[0].Rows.Count > 0)
        {
            DataTable dt_customer = ds_customer.Tables[0];
            int count = dt_customer.Rows.Count;
            int index = 0;
            while (index < count)
            {
                ListItem ll = new ListItem(dt_customer.Rows[index][1].ToString().Trim(), dt_customer.Rows[index][0].ToString().Trim());
                ddlist_customer.Items.Add(ll);
                index++;
            }
        }
    }

    protected void bindFromCountry(DropDownList ddlist_fromcountry)
    {
        DataSet ds_country = sql.getCountryInfo();
        if (ds_country.Tables[0].Rows.Count > 0)
        {
            DataTable dt_country = ds_country.Tables[0];
            int countcountry = dt_country.Rows.Count;
            int indexcountry = 0;
            while (indexcountry < countcountry)
            {
                ListItem ll = new ListItem(dt_country.Rows[indexcountry][1].ToString().Trim(), dt_country.Rows[indexcountry][0].ToString().Trim());
                ddlist_fromcountry.Items.Add(ll);
                indexcountry++;
            }
            ddlist_fromcountry.Enabled = true;
        }
    }

    private void bindCurrency(DropDownList ddlist_currency)
    {
        DataSet ds_currency = sql.getCurrency();
        if (ds_currency.Tables[0].Rows.Count > 0)
        {
            DataTable dt_currency = ds_currency.Tables[0];
            int count = dt_currency.Rows.Count;
            int index = 0;
            while (index < count)
            {
                ListItem ll = new ListItem(dt_currency.Rows[index][0].ToString().Trim(), dt_currency.Rows[index][1].ToString().Trim());
                ddlist_currency.Items.Add(ll);
                index++;
            }
            ddlist_currency.Enabled = true;
        }
    }

    protected void bindToCountry(DropDownList ddlist_tocountry)
    {
        DataSet ds_country = sql.getCountryInfo();
        if (ds_country.Tables[0].Rows.Count > 0)
        {
            DataTable dt_country = ds_country.Tables[0];
            int countcountry = dt_country.Rows.Count;
            int indexcountry = 0;
            while (indexcountry < countcountry)
            {
                ListItem ll = new ListItem(dt_country.Rows[indexcountry][1].ToString().Trim(), dt_country.Rows[indexcountry][0].ToString().Trim());
                ddlist_tocountry.Items.Add(ll);
                indexcountry++;
            }
            ddlist_tocountry.Enabled = true;
        }
    }

    protected void btn_addprojectYTD_Click(object sender, EventArgs e)
    {
        lbtn_addprojectYTD.Text = "Add Project";
        lbtn_addprojectYTD.Enabled = true;
        panel_addprojectYTD.Visible = false;
        lbtn_addcustomerYTD.Enabled = true;
        // By DingJunjie 20110518 Item 7 Add Start
        this.lbtn_modifyprojectYTD.Enabled = true;
        // By DingJunjie 20110518 Item 7 Add End

        label_addYTD.Visible = true;

        string str_name = tbox_project_nameYTD.Text.Trim();
        string str_from = ddlist_project_fromcountryYTD.SelectedItem.Value.Trim();
        string str_value = tbox_project_valueYTD.Text.Trim();
        string str_probability = tbox_project_probabilityYTD.Text.Trim();
        string str_toID = ddlist_project_tocountryYTD.SelectedItem.Value.Trim();
        string str_customerID = this.hidProCusName.Value;
        string str_currencyID = ddlist_currencyYTD.SelectedItem.Value.Trim();
        //by yyam itemw73 20110714 add start
        string str_Comments = txtComments.Text;
        //by yyam itemw73 20110714 add end
        //BY yyan 20110505 Item 11 add start
        string str_segment = ddlist_segmentYTD.SelectedItem.Value.Trim();
        //addproject(str_name, str_customerID, str_from, str_value, str_probability, str_toID, label_addYTD, str_currencyID);
        // By DingJunjie 20110513 Item 7 Delete Start
        //addproject(str_name, str_customerID, str_from, str_value, str_probability, str_toID, label_addYTD, str_currencyID, str_segment);
        // By DingJunjie 20110513 Item 7 Delete End
        //BY yyan 20110505 Item 11 add end
        // bind project datasource
        // By DingJunjie 20110512 Item 7 Delete Start
        //ddlist_projectYTD.Items.Clear();
        //bind(getProjectByCustomer(ddlist_customerYTD.SelectedValue.ToString().Trim()), ddlist_projectYTD);
        // By DingJunjie 20110512 Item 7 Delete End

        // By DingJunjie 20110512 Item 7 Add Start
        this.chkYTDIsCopy.Checked = false;
        //by yyan itemw73 20110714 edit start
        bool flag = addproject(str_name, str_customerID, str_from, str_value, str_probability, str_toID, label_addYTD, str_currencyID, str_segment, str_Comments);
        //by yyan itemw73 20110714 edit end
        //by yyan itemw57 20110630 del start 
        if (flag)
        {
            DataSet dsProject = helper.GetDataSet("SELECT ID,NAME FROM Project WHERE ID=(SELECT MAX(ID) FROM Project)");
            if (dsProject != null && dsProject.Tables.Count != 0 && dsProject.Tables[0].Rows.Count > 0)
            {
                this.hidOldProName.Value = dsProject.Tables[0].Rows[0][0].ToString();
                this.ddlist_projectYTD.Text = dsProject.Tables[0].Rows[0][1].ToString();
            }
        }
        //by yyan itemw57 20110630 del end 
        // By DingJunjie 20110512 Item 7 Add End
        // By DingJunjie 20110519 Item 7 Add Start
        this.hidButtomFlag.Value = "true";
        // By DingJunjie 20110519 Item 7 Add End
    }

    protected void btn_CancelprojectYTD_Click(object sender, EventArgs e)
    {
        // By DingJunjie 20110517 Item 7 Add Start
        this.hidButtomFlag.Value = "true";
        // By DingJunjie 20110517 Item 7 Add End
        lbtn_addprojectYTD.Text = "Add Project";
        lbtn_addprojectYTD.Enabled = true;
        panel_addprojectYTD.Visible = false;
        lbtn_addcustomerYTD.Enabled = true;
        // By DingJunjie 20110512 Item 7 Add Start
        this.chkYTDIsCopy.Checked = false;
        // By DingJunjie 20110512 Item 7 Add End
        // By DingJunjie 20110517 Item 7 Add Start
        this.lbtn_modifyprojectYTD.Enabled = true;
        // By DingJunjie 20110517 Item 7 Add End
        this.lnkButtonModifyCustmer.Enabled = true;
    }

    /// <summary>
    /// add a project
    /// </summary>
    /// <param name="str_name">Project Name</param>
    /// <param name="str_fromID">Project Country</param>
    /// <param name="str_value">Project Value</param>
    /// <param name="str_probability">Probability</param>
    /// <param name="str_toID">Point of Destination</param>
    //BY yyan 20110505 Item 11 del start
    //private void addproject(string str_name, string str_customerID, string str_from, string str_value, string str_probability, string str_toID, Label label_add, string str_currencyID)
    //BY yyan 20110505 Item 11 del end
    //BY yyan 20110505 Item 11 add start
    // By DingJunjie 20110513 Item 7 Delete Start
    //private void addproject(string str_name, string str_customerID, string str_from, string str_value, string str_probability, string str_toID, Label label_add, string str_currencyID, string str_segmentID)
    // By DingJunjie 20110513 Item 7 Delete End
    //BY yyan 20110505 Item 11 add end
    // By DingJunjie 20110513 Item 7 Add Start
    private bool addproject(string str_name, string str_customerID, string str_from, string str_value, string str_probability, string str_toID, Label label_add, string str_currencyID, string str_segmentID, string str_Comments)
    // By DingJunjie 20110513 Item 7 Add End
    {
        label_add.ForeColor = System.Drawing.Color.Red;

        if (str_name.Trim().Length == 0)
        {
            label_add.Text = info.addillegal("Project Name is null.");
            // By DingJunjie 20110512 Item 7 Delete Start
            //return;
            // By DingJunjie 20110512 Item 7 Delete End
            // By DingJunjie 20110512 Item 7 Add Start
            return false;
            // By DingJunjie 20110512 Item 7 Add End
        }

        if (str_name.Trim().Length > 100)
        {
            label_add.Text = info.addillegal("The length of Project Name > 100.");
            // By DingJunjie 20110512 Item 7 Delete Start
            //return;
            // By DingJunjie 20110512 Item 7 Delete End
            // By DingJunjie 20110512 Item 7 Add Start
            return false;
            // By DingJunjie 20110512 Item 7 Add End
        }
        //by yyan itemw73 20110714 add start
        if (str_Comments.Trim().Length > 4000)
        {
            label_add.Text = info.addillegal("The length of Comments > 4000.");
            return false;
        }
        //by yyan itemw73 20110714 add end
        if (str_value.Trim().Length == 0)
        {
            label_add.Text = info.addillegal("Project Value is null.");
            // By DingJunjie 20110512 Item 7 Delete Start
            //return;
            // By DingJunjie 20110512 Item 7 Delete End
            // By DingJunjie 20110512 Item 7 Add Start
            return false;
            // By DingJunjie 20110512 Item 7 Add End
        }

        if (str_probability.Trim().Length == 0)
        {
            label_add.Text = info.addillegal("Probability is null.");
            // By DingJunjie 20110512 Item 7 Delete Start
            //return;
            // By DingJunjie 20110512 Item 7 Delete End
            // By DingJunjie 20110512 Item 7 Add Start
            return false;
            // By DingJunjie 20110512 Item 7 Add End
        }
        //BY yyan 20110505 Item 11 add start
        if (str_segmentID.Trim().Length == 0)
        {
            label_add.Text = info.addillegal("Segment is null.");
            // By DingJunjie 20110512 Item 7 Delete Start
            //return;
            // By DingJunjie 20110512 Item 7 Delete End
            // By DingJunjie 20110512 Item 7 Add Start
            return false;
            // By DingJunjie 20110512 Item 7 Add End
        }
        //BY yyan 20110505 Item 11 add end
        // By DingJunjie 20110512 Item 7 Delete Start
        //if (!existproject(str_name))
        //{
        //    label_add.Text = info.addExist(str_name);
        //    return;
        //}
        // By DingJunjie 20110512 Item 7 Delete End
        // By DingJunjie 20110512 Item 7 Add Start
        if (!existproject(str_name, str_customerID, str_from, str_segmentID))
        {
            label_add.Text = info.addExist(str_name);
            return false;
        }
        // By DingJunjie 20110512 Item 7 Add End
        //BY yyan 20110505 Item 11 del start
        //string sql = "INSERT INTO [Project]([Name], CustomerNameID, POSID, [Value], Probability, PODID, CurrencyID,  Deleted,)"
        //           + " VALUES ('" + str_name + "','" + str_customerID + "','" + str_from + "','" + str_value + "','" + str_probability + "','" + str_toID + "','" + str_currencyID + "','0')";
        //BY yyan 20110505 Item 11 del end
        //BY yyan 20110505 Item 11 add start
        string sql = "INSERT INTO [Project]([Name], CustomerNameID, POSID, [Value], Probability, PODID, CurrencyID,  Deleted,[ProSegmentID],CreateUser,Comments)"
                   + " VALUES ('" + str_name + "','" + str_customerID + "','" + str_from + "','" + str_value + "','" + str_probability + "','" + str_toID + "','" + str_currencyID + "','0','" + str_segmentID + "'," + getRSMID() + ",'" + str_Comments + "')";
        //BY yyan 20110505 Item 11 add end        
        int count = helper.ExecuteNonQuery(CommandType.Text, sql, null);
        if (count == 1)
        {
            label_add.ForeColor = System.Drawing.Color.Green;
            label_add.Text = info.addLabelInfo(str_name, true);
            // By DingJunjie 20110513 Item 7 Add Start
            return true;
            // By DingJunjie 20110513 Item 7 Add End
        }
        else
        {
            label_add.Text = info.addLabelInfo(str_name, false);
            // By DingJunjie 20110513 Item 7 Add Start
            return false;
            // By DingJunjie 20110513 Item 7 Add End
        }
    }

    // By DingJunjie 20110512 Item 7 Delete Start
    //protected bool existproject(string str_name)
    //{
    //    string sql_project = "SELECT ID"
    //                       + " FROM [Project]"
    //                       + " WHERE Deleted = 0 AND Name = '" + str_name + "'";
    //    DataSet ds_project = helper.GetDataSet(sql_project);

    //    if (ds_project.Tables[0].Rows.Count > 0)
    //        return false;
    //    else
    //        return true;
    //}
    // By DingJunjie 20110512 Item 7 Delete End

    private void bindDrop(DataSet ds, DropDownList ddlist)
    {
        if (ds.Tables[0].Rows.Count > 0)
        {
            DataTable dt = ds.Tables[0];
            int count = dt.Rows.Count;
            int index = 0;
            while (index < count)
            {
                ListItem ll = new ListItem(dt.Rows[index][1].ToString().Trim(), dt.Rows[index][0].ToString().Trim());
                ddlist.Items.Add(ll);
                index++;
            }
            ddlist.Enabled = true;
        }
    }

    // add by zy 20101224 end

    protected string getCountryAbbr(string countryID)
    {
        string query_country = "SELECT ISO_Code FROM [Country] "
                            + " WHERE ID = " + countryID
                            + " GROUP BY [Country].ISO_Code ";
        DataSet ds_country = helper.GetDataSet(query_country);

        if (ds_country.Tables[0].Rows.Count > 0)
        {
            return ds_country.Tables[0].Rows[0][0].ToString();
        }
        else
            return null;
    }

    protected void getOperationBySegment(string segmentID, DropDownList ddlist)////ddlist_operation
    {
        string query_operation = "SELECT [Operation].Abbr+' - '+[Operation].Name,[Operation].ID FROM [Operation_Segment] "
                            + " INNER JOIN [Operation] ON [Operation].ID = [Operation_Segment].OperationID"
                            + " WHERE SegmentID = " + segmentID
                            + " AND [Operation].Deleted = 0 AND [Operation_Segment].Deleted = 0"
                            + " GROUP BY [Operation].Abbr+' - '+[Operation].Name,[Operation].ID";
        DataSet ds_operation = helper.GetDataSet(query_operation);

        if (ds_operation.Tables[0].Rows.Count > 0)
        {
            DataTable dt = ds_operation.Tables[0];
            int count = dt.Rows.Count;
            int index = 0;
            while (index < count)
            {
                ListItem li = new ListItem(dt.Rows[index][0].ToString().Trim(), dt.Rows[index][1].ToString().Trim());
                ddlist.Items.Add(li);
                index++;
            }
            ddlist.SelectedIndex = 0;
            ddlist.Enabled = true;
        }
        else
        {
            ListItem li = new ListItem("", "-1");
            ddlist.Items.Add(li);
            ddlist.Enabled = false;
        }
    }

    protected DataSet getOperationBySegment(string segmentId)
    {
        string query_operation = "SELECT [Operation].Abbr as opname,[Operation].ID as opid FROM [Operation_Segment] "
                            + " INNER JOIN [Operation] ON [Operation].ID = [Operation_Segment].OperationID"
                            + " WHERE SegmentID = " + segmentId
                            + " AND [Operation].Deleted = 0 AND [Operation_Segment].Deleted = 0"
                            + " GROUP BY [Operation].Abbr,[Operation].ID";
        return helper.GetDataSet(query_operation);
    }

    protected string getOperationIDByAbbr(string Abbr)
    {
        string query_operation = "SELECT [Operation].ID FROM [Operation] "
                            + " INNER JOIN [Operation_Segment] ON [Operation].ID = [Operation_Segment].OperationID"
                            + " WHERE [Operation].Abbr = '" + Abbr + "' AND [Operation_Segment].SegmentID = " + getSegmentID();
        DataSet ds_operation = helper.GetDataSet(query_operation);

        if (ds_operation.Tables[0].Rows.Count == 1)
            return ds_operation.Tables[0].Rows[0][0].ToString().Trim();
        else
            return "";
    }

    // add by zy 20101227 start
    protected void addCellsAttributes(GridViewRow row)
    {
        if (row.Cells[0].Text != "&nbsp;")
        {
            if (row.Cells[1].Text != "&nbsp;")
            {
                // by daixuesong 20110526 item43 update start
                LinkButton link = new LinkButton();
                link.ID = "lnkCustomer";
                link.Text = row.Cells[5].Text;
                link.ToolTip = "Customer Detail";
                link.OnClientClick = "window.open('CustomerDetail.aspx?customerID=" + row.Cells[1].Text + "&salesChannelID=" + row.Cells[3].Text + "','Customer', 'height=150,width=810,top=100,left=200,toolbar=no,status=no, menubar=no,location=no,resizable=yes,scrollbars=no');return false";
                row.Cells[5].Controls.Add(link);
                //row.Cells[6].Attributes["onclick"] = "window.open('CustomerDetail.aspx?customerID=" + row.Cells[2].Text + "&salesChannelID=" + row.Cells[4].Text + "','Customer', 'height=150,width=810,top=100,left=200,toolbar=no,status=no, menubar=no,location=no,resizable=yes,scrollbars=no');";
                //row.Cells[6].Attributes["title"] = "Customer Detail";
                //row.Cells[6].Text = "<a href='#'>" + row.Cells[6].Text + "</a>";
                // by daixuesong 20110526 item43 update start
            }
            if (row.Cells[2].Text != "&nbsp;")
            {
                // By DingJunjie 20110512 Item 7 Delete Start
                //if (row.RowState == DataControlRowState.Alternate)
                //{
                // By DingJunjie 20110512 Item 7 Delete End
                // By DingJunjie 20110523 Item 7 Delete Start
                //row.Cells[7].Text = "<a href='#'>" + row.Cells[7].Text + "</a>";
                //row.Cells[7].Attributes["onclick"] = "window.open('ProjectDetail.aspx?projectID=" + row.Cells[3].Text + "&customerID=" + row.Cells[2].Text + "','Project', 'height=150,width=810,top=100,left=200,toolbar=no,status=no, menubar=no,location=no,resizable=yes,scrollbars=no');";
                //row.Cells[7].Attributes["title"] = "Project Detail";
                // By DingJunjie 20110523 Item 7 Delete End
                // By DingJunjie 20110523 Item 7 Add Start
                LinkButton link = new LinkButton();
                link.ID = "lnkProject";
                link.Text = row.Cells[6].Text;
                link.ToolTip = "Project Detail";
                //by yyan itemw72 20110713 del start
                //link.OnClientClick = "window.open('ProjectDetail.aspx?projectID=" + row.Cells[3].Text + "&customerID=" + row.Cells[2].Text + "','Project', 'height=150,width=810,top=100,left=200,toolbar=no,status=no, menubar=no,location=no,resizable=yes,scrollbars=no');";
                //by yyan itemw72 20110713 del end
                //by yyan itemw72 20110713 add start
                link.OnClientClick = "window.open('ProjectDetail.aspx?projectID=" + row.Cells[2].Text + "&customerID=" + row.Cells[1].Text + "','Project', 'height=150,width=810,top=100,left=100,toolbar=no,status=no, menubar=no,location=no,resizable=yes,scrollbars=no');return false";
                //by yyan itemw72 20110713 add end
                row.Cells[6].Controls.Add(link);
                // By DingJunjie 20110523 Item 7 Add End
                // By DingJunjie 20110512 Item 7 Delete Start
                //}
                // By DingJunjie 20110512 Item 7 Delete End
            }
            if (string.Equals(row.Cells[7].Text.ToLower(), "rc own business"))
            {
                LinkButton link = null;
                string id = row.Parent.ID;
                for (int i = 11; i < row.Cells.Count - 3; i += 7)
                {
                    StringBuilder param = new StringBuilder();
                    param.Append("'").Append(getRSMID()).Append("',");
                    param.Append("'").Append(getSalesOrgID()).Append("',");
                    param.Append("'").Append(row.Cells[0].Text.Replace("&nbsp;", "")).Append("',");
                    param.Append("'").Append(row.Cells[1].Text.Replace("&nbsp;", "-1")).Append("',");
                    param.Append("'").Append(this.hidBookingY.Value).Append("',");
                    param.Append("'").Append(this.hidDeliverY.Value).Append("',");
                    param.Append("'").Append(getSegmentID()).Append("',");
                    param.Append("'").Append(row.Cells[i + 2].Text.Replace("&nbsp;", "")).Append("',");
                    param.Append("'").Append(row.Cells[2].Text.Replace("&nbsp;", "-1")).Append("',");
                    param.Append("'").Append(row.Cells[3].Text.Replace("&nbsp;", "-1")).Append("',");
                    param.Append("'").Append(year).Append("',");
                    param.Append("'").Append(month).Append("',");
                    param.Append("'").Append(row.Cells[i].Text).Append("'");
                    link = new LinkButton();
                    link.Text = row.Cells[i + 1].Text;
                    link.ForeColor = row.Cells[i + 1].ForeColor;
                    if (string.IsNullOrEmpty(row.Cells[i + 1].ToolTip))
                    {
                        link.ToolTip = link.Text;
                    }
                    link.OnClientClick = "showRCOBViewPage(" + param.ToString() + ");return false;";
                    row.Cells[i + 1].Controls.Add(link);
                }
            }
        }
    }

    // add by zy 20101227 end
    protected override void Render(HtmlTextWriter writer)
    {
        foreach (GridViewRow row in gv_bookingbydatebyproduct.Rows)
        {
            if (row.RowState == DataControlRowState.Edit)
            {

                row.Attributes.Remove("ondblclick");
                row.Attributes.Remove("style");
                row.Attributes["title"] = "Edit Row";
                continue;
            }
            if (row.RowType == DataControlRowType.DataRow)
            {
                if (butFlag)
                {
                    row.Attributes["ondblclick"] = ClientScript.GetPostBackEventReference(gv_bookingbydatebyproduct, "Edit$" + row.RowIndex.ToString(), true);
                    row.Attributes["style"] = "cursor:pointer";
                    row.Attributes["title"] = "Double-click to edit";
                    // add by zy 20101227 start
                    addCellsAttributes(row);
                    // add by zy 20101227 end
                }
            }
        }

        base.Render(writer);
    }
    // update by zy 20101228 start
    //protected bool existRow(string str_bookingY, string str_deliverY, string str_countryID, string str_operationID)
    // By DingJunjie 20110601 ItemW18 Delete Start
    //protected bool existRow(string str_bookingY, string str_deliverY, string str_countryID, string str_operationID, string str_customerID, string str_projectID, string SalesChannelID)
    // By DingJunjie 20110601 ItemW18 Delete End
    // By DingJunjie 20110601 ItemW18 Add Start
    protected bool existRow(string str_bookingY, string str_deliverY, string str_countryID, string str_customerID, string str_projectID, string SalesChannelID, string str_salesorgID)
    // By DingJunjie 20110601 ItemW18 Add End
    // update by zy 20101228 end
    {
        string sql = "SELECT Amount FROM [Bookings]"
                    + " WHERE RSMID = " + getRSMID()
                    + " AND CountryID = " + str_countryID
                    + " AND BookingY = '" + str_bookingY + "'"
                    + " AND DeliverY = '" + str_deliverY + "'"
                    + " AND SegmentID = " + getSegmentID()
                    + " AND YEAR(TimeFlag) = '" + year + "'"
                    + " AND MONTH(TimeFlag) = '" + month + "'";
        // add by zy 20101228 start
        if (!str_customerID.Equals(""))
        {
            sql += "AND CustomerID = " + str_customerID;
        }
        else
        {
            sql += "AND CustomerID = -1";
        }

        if (!str_projectID.Equals(""))
        {
            sql += "AND ProjectID = " + str_projectID;
        }
        else
        {
            sql += "AND ProjectID = -1";
        }

        // add by zy 20101228 end
        // add by zy 20110121 start
        sql += " AND SalesChannelID = " + SalesChannelID;
        // add by zy 20110121 end
        // By DingJunjie 20110601 ItemW18 Add Start
        sql += "  AND SalesOrgID=" + str_salesorgID;
        // By DingJunjie 20110601 ItemW18 Add End

        DataSet ds = helper.GetDataSet(sql);
        if (ds.Tables[0].Rows.Count > 0)
            return false;
        else
            return true;
    }

    protected int getRowId(string str_bookingY, string str_deliverY, string str_countryID, string str_customerID, string str_projectID, string SalesChannelID, string str_salesorgID)
    {
        string sql = "SELECT  case when MAX(RecordID) is null then 0 else MAX(RecordID) end FROM [Bookings]"
                    + " WHERE RSMID = " + getRSMID()
                    + " AND CountryID = " + str_countryID
                    + " AND BookingY = '" + str_bookingY + "'"
                    + " AND DeliverY = '" + str_deliverY + "'"
                    + " AND SegmentID = " + getSegmentID()
                    + " AND YEAR(TimeFlag) = '" + year + "'"
                    + " AND MONTH(TimeFlag) = '" + month + "'";
       
        /* TODO : 
         * // add by zy 20101228 start
        if (!str_customerID.Equals(""))
        {
            sql += "AND CustomerID = " + str_customerID;
        }
        else
        {
            sql += "AND CustomerID = -1";
        }

        if (!str_projectID.Equals(""))
        {
            sql += "AND ProjectID = " + str_projectID;
        }
        else
        {
            sql += "AND ProjectID = -1";
        }

        // add by zy 20101228 end
        // add by zy 20110121 start
        sql += " AND SalesChannelID = " + SalesChannelID;
        // add by zy 20110121 end
        // By DingJunjie 20110601 ItemW18 Add Start
        sql += "  AND SalesOrgID=" + str_salesorgID;
        // By DingJunjie 20110601 ItemW18 Add End
        */
        DataSet ds = helper.GetDataSet(sql);
        return Convert.ToInt32(ds.Tables[0].Rows[0][0]);
    }

    protected void add_booking(GridView gv, string bookingY, string deliverY, Label label_note,
        string CouID, string OpID, string CustomerID, string ProjectID, string SalesChannelID)
    {
        this.lblError.Visible = true;
        this.lblError.Text = "";
        DataSet ds_product = getProductBySegment(getSegmentID());
        string str_deliveryInFy = getDeliveryInFy(bookingY, deliverY);
        string str_NoInFy = getNoInFy(bookingY, deliverY);
        if (inputCheck(CouID, CustomerID, SalesChannelID, this.lblError))
        {
            if (isCountryCanUsed(CouID))
            {
                if (isCSRelationExist(CouID))
                {
                    var dict = new Dictionary<int, string[]>();

                    for (int key = 0; key < ds_product.Tables[0].Rows.Count; key++)
                    {
                        var product = ds_product.Tables[0].Rows[key];
                        dict.Add(key, new String[] { product.ItemArray[0].ToString(), OpID });
                    }
                    if(!validateOperation(dict, getRSMID().Trim(), getSalesOrgID().Trim(), getSegmentID(), CouID, CustomerID, ProjectID, SalesChannelID, ""))
                    {
                        gv.EditIndex = -1;
                        ScriptManager.RegisterStartupScript(this.upYTD1, this.upYTD1.GetType(), "UpdateExistError", "alert('The record had been existed!');", true);
                        return;
                    }
                    int recordId = getRowId(bookingY, deliverY, CouID, CustomerID, ProjectID, SalesChannelID, getSalesOrgID().Trim())+1;
                    for (int i = 0; i < ds_product.Tables[0].Rows.Count; i++)
                        {
                            string insert_booking = "INSERT INTO [Bookings](RecordID,RSMID, SalesOrgID, CountryID, CustomerID, BookingY, DeliverY, SegmentID, ProductID, OperationID, ProjectID, SalesChannelID, Amount, Comments,[Delivery in FY], [NO in FY],  TimeFlag)"
                                        + " VALUES("
                                        + recordId.ToString(CultureInfo.InvariantCulture) + ","
                                        + getRSMID().Trim() + ","
                                        + getSalesOrgID().Trim() + ","
                                        + CouID + ","
                                        + CustomerID + ","
                                        + "'" + bookingY + "'" + ","
                                        + "'" + deliverY + "'" + ","
                                        + getSegmentID() + ","
                                        + ds_product.Tables[0].Rows[i][0].ToString() + ","
                                        + OpID + ","
                                        + ProjectID + ","
                                        + SalesChannelID + ","
                                          + "0,NULL,'"
                                        + str_deliveryInFy + "','"
                                        + str_NoInFy + "','"
                                        + year + "-" + month + "-01" + "')";
                            int count = helper.ExecuteNonQuery(CommandType.Text, insert_booking, null);

                            if (count == 1)
                            {
                                this.lblError.Text += "";
                            }
                            else
                            {
                                this.lblError.Text += "Added error.." + count.ToString();
                                // By DingJunjie 20110524 Item 7 Delete Start
                                //helper.ExecuteNonQuery(CommandType.Text, "DELETE FROM [Bookings] WHERE Amount = 0", null);
                                // By DingJunjie 20110524 Item 7 Delete End
                                break;
                            }
                        }

                        if (this.lblError.Text == "")
                        {
                            this.lblError.Text = "";
                            this.panel_addYTD.Visible = false;
                            // By DingJunjie 20110601 ItemW18 Delete Start
                            //Page.ClientScript.RegisterStartupScript(Page.GetType(), "message", "<script language='javascript' defer>alert('Added successfully!');</script>");
                            // By DingJunjie 20110601 ItemW18 Delete End
                            // By DingJunjie 20110601 ItemW18 Add Start
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "AddSuccess", "alert('Added successfully!');", true);
                            // By DingJunjie 20110601 ItemW18 Add End
                        }
                        else
                        {
                            this.lblError.Text += "";
                            // By DingJunjie 20110601 ItemW18 Delete Start
                            //Page.ClientScript.RegisterStartupScript(Page.GetType(), "message", "<script language='javascript' defer>alert('Added unsuccessfully!');</script>");
                            // By DingJunjie 20110601 ItemW18 Delete End
                            // By DingJunjie 20110601 ItemW18 Add Start
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "AddUnsuccess", "alert('Added unsuccessfully!');", true);
                            // By DingJunjie 20110601 ItemW18 Add End
                        }
                    //}
                    //else
                    //{
                    //    // By DingJunjie 20110601 ItemW18 Delete Start
                    //    //Page.ClientScript.RegisterStartupScript(Page.GetType(), "message", "<script language='javascript' defer>alert('The record had been existed!');</script>");
                    //    // By DingJunjie 20110601 ItemW18 Delete End
                    //    // By DingJunjie 20110601 ItemW18 Add Start
                    //    ScriptManager.RegisterStartupScript(this, this.GetType(), "AddExistError", "alert('The record had been existed!');", true);
                    //    // By DingJunjie 20110601 ItemW18 Add End
                    //}
                    // By DingJunjie 20110601 ItemW25 Add Start
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "RelationError", "alert('There is no related country to this subregion(" + ddlist_countryYTD.Items.FindByValue(CouID).Text + ")!');", true);
                }
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "CountryUnusedError", "alert('Subregion(" + ddlist_countryYTD.Items.FindByValue(CouID).Text + ") is not assigned to this user!');", true);
            }
        }
        // By DingJunjie 20110601 ItemW25 Add End
    }

    //wj 20110107 start
    protected void ddlist_customer_edit_SelectedIndexChanged(object sender, EventArgs e)
    {
        ddlist_project_edit.Items.Clear();
        bind(getProjectByCustomer(ddlist_customer_edit.SelectedItem.Value.Trim()), ddlist_project_edit);
        bindDataSource();
    }
    //end

    /* gv_bookingbydatebyproduct */
    protected void gv_bookingbydatebyproduct_RowEditing(object sender, GridViewEditEventArgs e)
    {
        imgbtn_addYTD.Visible = false;
        imgbtn_addYTD.Enabled = false;
        panel_addYTD.Visible = false;

        gv_bookingbydatebyproduct.EditIndex = e.NewEditIndex;
        //wj 20110107 start
        //------------------------------------------------
        pnl_edit.Visible = false;
        string str_countryID = gv_bookingbydatebyproduct.Rows[e.NewEditIndex].Cells[0].Text.Trim();
        countryID = str_countryID;
        ddlist_customer_edit.Items.Clear();
        bind(getCustomerByCountryID1(str_countryID), ddlist_customer_edit);
        ddlist_project_edit.Items.Clear();
        bind(getProjectByCustomer(ddlist_customer_edit.SelectedItem.Value.Trim()), ddlist_project_edit);
        ckbox_ok.Checked = false;
        //end
        bindDataSource();
        
        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ShowOptionTip", "showOptionTip();", true);

        //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "", "GetResultFromServer();", true);



        ScriptManager.RegisterStartupScript(this, this.GetType(), "", "GetResultFromServer();", true);

        //ScriptManager.RegisterStartupScript(this, this.GetType(), "", "<script>window.onload=function(){GetResultFromServer();};</script>", true);

    //    Page.ClientScript.RegisterStartupScript(this.GetType(), "",
    //"window.onload = function() {GetResultFromServer();};", true);      
    }

    protected void gv_bookingbydatebyproduct_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        imgbtn_addYTD.Visible = true;
        imgbtn_addYTD.Enabled = true;
        panel_addYTD.Visible = false;
        pnl_edit.Visible = false;//wj

        gv_bookingbydatebyproduct.EditIndex = -1;
        // By DingJunjie 20110524 Item 7 Delete Start
        //helper.ExecuteNonQuery(CommandType.Text, "DELETE FROM [Bookings] WHERE Amount = 0", null);
        // By DingJunjie 20110524 Item 7 Delete End
        bindDataSource();
    }

    protected void gv_bookingbydatebyproduct_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        imgbtn_addYTD.Visible = true;
        imgbtn_addYTD.Enabled = true;
        panel_addYTD.Visible = false;
        pnl_edit.Visible = false;//wj

        
        update_booking(gv_bookingbydatebyproduct, e.RowIndex, this.hidBookingY.Value, this.hidDeliverY.Value, label_note);
        bindDataSource();

        ScriptManager.RegisterStartupScript(this, this.GetType(), "", "GetResultFromServer();", true);
    }

    protected void gv_bookingbydatebyproduct_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        // By DingJunjie 20110608 ItemW18 Add Start
        if (e.Row.RowType == DataControlRowType.Header)
        {
            e.Row.Cells[gv_bookingbydatebyproduct.Columns.Count - 1].Controls.Clear();
            e.Row.Cells[gv_bookingbydatebyproduct.Columns.Count - 2].Controls.Clear();
            e.Row.Cells[gv_bookingbydatebyproduct.Columns.Count - 3].Controls.Clear();
        }
        // By DingJunjie 20110608 ItemW18 Add End
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Attributes.Add("onmouseover", "e=this.style.backgroundColor; this.style.backgroundColor='#33CCCC'");
            e.Row.Attributes.Add("onmouseout", "this.style.backgroundColor=e");
        }
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (e.Row.RowState == DataControlRowState.Normal || e.Row.RowState == DataControlRowState.Alternate)
            {
                if (butFlag)
                {
                    string str_countryID = e.Row.Cells[0].Text.Replace("&nbsp;", "");
                    string str_customerID = e.Row.Cells[1].Text.Replace("&nbsp;", "");
                    string str_projectID = e.Row.Cells[2].Text.Replace("&nbsp;", "");
                    string str_salesChannelID = e.Row.Cells[3].Text.Replace("&nbsp;", "");
                    if (str_customerID.Equals("-1") || str_customerID.Equals(""))
                    {
                        str_customerID = "-1";
                    }
                    if (str_projectID.Equals("-1") || str_projectID.Equals(""))
                    {
                        str_projectID = "-1";
                    }
                    if (str_salesChannelID.Equals("-1") || str_salesChannelID.Equals(""))
                    {
                        str_salesChannelID = "-1";
                    }
                    string args = "SalesOrgID=" + getSalesOrgID() + "&CountryID=" + str_countryID  + "&SegmentID=" + getSegmentID()
                        + "&RSMID=" + getRSMID() + "&BookingY=" + this.hidBookingY.Value  + "&DeliverY=" + this.hidDeliverY.Value
                        + "&ProjectID=" + str_projectID + "&CustomerID=" + str_customerID + "&SalesChannelID=" + str_salesChannelID;
                    string str_args = "'Admin/AdminBookingDataComments.aspx?" + args + "'" + ",'Comments', 'height=350,width=400,top=100,left=200,toolbar=no,status=no, menubar=no,location=no,resizable=yes,scrollbars=yes'";
                    ((ImageButton)e.Row.Cells[gv_bookingbydatebyproduct.Columns.Count - 1].Controls[0]).Attributes.Add("onclick", "window.open(" + str_args + ");return false;");
                    ((ImageButton)e.Row.Cells[gv_bookingbydatebyproduct.Columns.Count - 2].Controls[0]).Attributes.Add("onclick", info.setRowDataBound("this info"));
                }

                
            }

            if ((e.Row.RowState & DataControlRowState.Edit)!=0)
            {

                for (int i = 0; i < e.Row.Cells.Count; i++)
                {
                    if (e.Row.Cells[i].Controls != null && e.Row.Cells[i].Controls.Count > 0)
                    {
                        TextBox txtEdit = e.Row.Cells[i].Controls[0] as TextBox;
                        //txtEdit.Width = System.Web.UI.WebControls.Unit.Pixel(100);
                        if (txtEdit != null)
                        {
                            txtEdit.Attributes.Add("style", "text-align:right;");
                        }
                    }
                }

            }
        }
        
    }

    protected void imgbtn_addYTD_Click(object sender, ImageClickEventArgs e)
    {
        // By DingJunjie 20110517 Item 7 Add Start
        this.hidButtomFlag.Value = "true";
        // By DingJunjie 20110517 Item 7 Add End
        imgbtn_addYTD.Visible = false;
        imgbtn_addYTD.Enabled = false;
        panel_addYTD.Visible = true;
        // add by zy 20101229 start
        label_addYTD.Text = "";
        this.lblError.Text = "";

        // init 
        lbtn_addcustomerYTD.Text = "Add Customer";
        lbtn_addcustomerYTD.Enabled = true;
        panel_addcustomerYTD.Visible = false;

        lbtn_addprojectYTD.Text = "Add Project";
        lbtn_addprojectYTD.Enabled = true;
        panel_addprojectYTD.Visible = false;
        // add by zy 20101229 end

        // By DingJunjie 20110518 Item 7 Add Start
        lbtn_modifyprojectYTD.Enabled = true;
        p_ProjectYTD.Visible = false;
        this.gv_ProjectYTD.Columns.Clear();
        // By DingJunjie 20110518 Item 7 Add End

        ddlist_countryYTD.Items.Clear();
        getCountryByRSM(getRSMID().ToString().Trim(), ddlist_countryYTD);
        // add by zy 20101224 start
        ddlist_customerYTD.Items.Clear();
        bind(getCustomerByCountryID1(ddlist_countryYTD.SelectedValue.ToString().Trim()), ddlist_customerYTD);
        //by yyan itemw57 20110630 del start 
        //ddlist_projectYTD.Items.Clear();
        //by yyan itemw57 20110630 del end 
        // By DingJunjie 20110512 Item 7 Delete Start
        //bind(getProjectByCustomer(ddlist_customerYTD.SelectedValue.ToString().Trim()), ddlist_projectYTD);
        // By DingJunjie 20110512 Item 7 Delete End
        // By DingJunjie 20110512 Item 7 Add Start
        //by yyan itemw57 20110630 del start 
        //bind(getProject(), ddlist_projectYTD);
        //by yyan itemw57 20110630 del end 
        //by yyan itemw57 20110630 add start 
        ddlist_projectYTD.Text = "Click to select.";
        hidOldProName.Value = "-1";
        //by yyan itemw57 20110630 add end 
        // By DingJunjie 20110512 Item 7 Add End
        // add by zy 20101224 end
        // add by zy 20110121 start
        dropdownlist_saleschannelYTD.Items.Clear();
        bind(sql.getSalesChannelInfo(), dropdownlist_saleschannelYTD);
        // add by zy 20110121 end
        ddlist_operationYTD.Items.Clear();
        getOperationBySegment(getSegmentID(), ddlist_operationYTD);

        if (ddlist_countryYTD.SelectedItem == null || ddlist_countryYTD.SelectedItem.Text == "" || ddlist_operationYTD.SelectedItem.Text == "")
            btn_addYTD.Enabled = false;
        else
            btn_addYTD.Enabled = true;
    }

    protected void btn_addYTD_Click(object sender, EventArgs e)
    {
        imgbtn_addYTD.Visible = true;
        imgbtn_addYTD.Enabled = true;

        string CouID = ddlist_countryYTD.SelectedItem.Value;
        string OpID = ddlist_operationYTD.SelectedItem.Value;
        // add by zy 20101228 start
        string CustomerID = this.ddlist_customerYTD.SelectedValue;
        //by yyan itemw57 20110630 del start 
        //string ProjectID = ddlist_projectYTD.SelectedItem.Value;
        //by yyan itemw57 20110630 del end 
        //by yyan itemw57 20110630 add start 
        string ProjectID = this.hidOldProName.Value;
        //by yyan itemw57 20110630 add end 
        // add by zy 20101228 end
        // add by zy 20110121 start
        string SalesChannelID = dropdownlist_saleschannelYTD.SelectedItem.Value;
        // add by zy 20110121 end
        // update by zy 20101228 start
        //add_booking(gv_bookingbydatebyproduct, year.Substring(2, 2), "YTD", label_note, CouID, OpID);
        add_booking(gv_bookingbydatebyproduct, this.hidBookingY.Value, this.hidDeliverY.Value, label_note, CouID, OpID, CustomerID, ProjectID, SalesChannelID);
        // update by zy 20101228 end
        // By DingJunjie 20110524 Item 7 Add Start
        // By DingJunjie 20110527 Item 7 Delete Start
        //add_booking(gv_bookingbydatebyproduct, year.Substring(2, 2), year.Substring(2, 2), label_note, CouID, OpID, CustomerID, ProjectID, SalesChannelID);
        //add_booking(gv_bookingbydatebyproduct, year.Substring(2, 2), nextyear.Substring(2, 2), label_note, CouID, OpID, CustomerID, ProjectID, SalesChannelID);
        //add_booking(gv_bookingbydatebyproduct, nextyear.Substring(2, 2), nextyear.Substring(2, 2), label_note, CouID, OpID, CustomerID, ProjectID, SalesChannelID);
        //add_booking(gv_bookingbydatebyproduct, nextyear.Substring(2, 2), yearAfterNext.Substring(2, 2), label_note, CouID, OpID, CustomerID, ProjectID, SalesChannelID);
        // By DingJunjie 20110527 Item 7 Delete End
        // By DingJunjie 20110524 Item 7 Add End
        bindDataSource();
    }

    protected void btn_cancelYTD_Click(object sender, EventArgs e)
    {
        imgbtn_addYTD.Visible = true;
        imgbtn_addYTD.Enabled = true;
        panel_addYTD.Visible = false;
        // By DingJunjie 20110513 Item 7 Add Start
        p_ProjectYTD.Visible = false;
        lbtn_modifyprojectYTD.Enabled = true;
        this.gv_ProjectYTD.Columns.Clear();
        // By DingJunjie 20110513 Item 7 Add End

        bindDataSource();
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

    private void setEditCustomer()
    {
        if (getRoleID(getRole()) != "0")
            lnkButtonModifyCustmer.Visible = false;
    }

    protected void lbtn_return_Click(object sender, EventArgs e)
    {
        // By DingJunjie 20110706 ItemW60 Add Start
        string param = "?SalesOrgID=" + Request.QueryString["SalesOrgID"]
            + "&SegmentID=" + Request.QueryString["SegmentID"]
            + "&RSMID=" + Request.QueryString["RSMID"]
            + "&CountryID=" + Request.QueryString["CountryID"]
            + "&DataType=" + Request.QueryString["DataType"];
        // By DingJunjie 20110706 ItemW60 Add End
        if (getRoleID(getRole()) == "0")
        {
            Response.Redirect("~/Admin/AdminBookingSalesData.aspx" + param);
        }
        if (getRoleID(getRole()) == "3")
        {
            Response.Redirect("~/SalesOrgMgr/SalesOrgMgrBookingSalesData.aspx" + param);
        }
        if (getRoleID(getRole()) == "4")
        {
            // mbq del start
            //Response.Redirect("~/RSM/RSMBookingsSales.aspx");
            // mbq del end
            // mbq add start
            Response.Redirect("~/RSM/RSMBookingSalesData.aspx" + param);
            // mbq add end
        }
    }

    protected void gv_bookingbydatebyproduct_SelectedIndexChanging(object sender, GridViewSelectEventArgs e)
    {
        gv_bookingbydatebyproduct.SelectedIndex = -1;
        bindDataSource();
    }

    // add by zy 20110130 start
    // get Delivery In FY
    protected string getDeliveryInFy(string bookingY, string deliverY)
    {
        string str_deliveryInFy;
        
        if (deliverY == "YTD")
        {
            str_deliveryInFy = bookingY + " for " + bookingY;
        }
        else
        {
            str_deliveryInFy = bookingY + " for " + deliverY;
        }
        return str_deliveryInFy;
    }

    // get NO IN FY
    protected string getNoInFy(string bookingY, string deliverY)
    {
        string str_NoInFy;

       
        if (deliverY == "YTD")
        {
            str_NoInFy = bookingY + " " + deliverY;
        }
        else if (month == "03")
        {
            str_NoInFy = bookingY + " Bdg";
        }
        else
        {
            str_NoInFy = bookingY + " FC";
        }

        return str_NoInFy;
    }
    // add by zy 20110130 end
    //BY yyan 20110505 Item 11 add start
    private void bindSegment(DropDownList ddlist)
    {
        // By DingJunjie 20110525 ItemW11 Delete Start
        //DataSet ds = sql.getSegmentInfo();
        // By DingJunjie 20110525 ItemW11 Delete End
        // By DingJunjie 20110525 ItemW11 Add Start
        DataSet ds = sql.getSegmentByUser(getUserID());
        // By DingJunjie 20110526 ItemW11 Add Start
        bool allFlag = false;
        if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
        {
            ds = sql.getSegmentInfo();
            allFlag = true;
        }
        // By DingJunjie 20110526 ItemW11 Add End
        // By DingJunjie 20110525 ItemW11 Add End
        if (ds.Tables[0].Rows.Count > 0)
        {
            DataTable dt = ds.Tables[0];
            int count = dt.Rows.Count;
            int index = 0;
            while (index < count)
            {
                // By DingJunjie 20110525 ItemW11 Delete Start
                //ListItem ll = new ListItem(dt.Rows[index][1].ToString().Trim(), dt.Rows[index][0].ToString().Trim());
                // By DingJunjie 20110525 ItemW11 Delete End
                // By DingJunjie 20110525 ItemW11 Add Start
                // By DingJunjie 20110526 ItemW11 Delete Start
                //ListItem ll = new ListItem(dt.Rows[index][0].ToString().Trim(), dt.Rows[index][1].ToString().Trim());
                // By DingJunjie 20110526 ItemW11 Delete End
                // By DingJunjie 20110526 ItemW11 Add Start
                ListItem ll = null;
                if (allFlag)
                {
                    ll = new ListItem(dt.Rows[index][1].ToString().Trim(), dt.Rows[index][0].ToString().Trim());
                }
                else
                {
                    ll = new ListItem(dt.Rows[index][0].ToString().Trim(), dt.Rows[index][1].ToString().Trim());
                }
                // By DingJunjie 20110526 ItemW11 Add End
                // By DingJunjie 20110525 ItemW11 Add End
                ddlist.Items.Add(ll);
                index++;
            }
            ddlist.Enabled = true;
        }
    }
    //BY yyan 20110505 Item 11 add start

    //By Mbq 20110505 ITEM 1 ADD Start
    private string getCountrySQL()
    {
        string query_country = " SELECT [SubRegion].ID AS SubRegion"
                    + " FROM [User_Country] "
                    + " INNER JOIN [SubRegion] "
                    + " ON [User_Country].CountryID = [SubRegion].ID"
                    + " WHERE [User_Country].UserID  = '" + getUserID() + "'"
                    + " AND [SubRegion].Deleted = 0 AND [User_Country].Deleted = 0";
        return query_country;
    }
    //By Mbq 20110505 ITEM 1 ADD End

    //By Mbq 20110511 ITEM 1 Add Start
    private string getUserID()
    {
        return Request.QueryString["UserID"].ToString().Trim();
    }
    //By Mbq 20110511 ITEM 1 Add End

    #region DingJunjie Add
    // By DingJunjie 20110512 Item 7 Add Start
    /// <summary>
    /// YTD Project Cpoy
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void chkYTDIsCopy_CheckedChanged(object sender, EventArgs e)
    {
        if (this.chkYTDIsCopy.Checked)
        {
            lbtn_addprojectYTD_Click(sender, e);
        }
        else
        {
            btn_CancelprojectYTD_Click(sender, e);
        }
    }

    /// <summary>
    /// [lbtn_modifyprojectYTD] Button OnClick Method
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void lbtn_modifyprojectYTD_Click(object sender, EventArgs e)
    {
        // By DingJunjie 20110517 Item 7 Add Start
        this.hidButtomFlag.Value = "true";
        // By DingJunjie 20110517 Item 7 Add End
        bindDataSource(this.gv_ProjectYTD, getEditProject());
        this.panel_addcustomerYTD.Visible = false;
        this.lbtn_addprojectYTD.Enabled = false;
        this.panel_addprojectYTD.Visible = false;
        this.lbtn_addcustomerYTD.Enabled = false;
        this.p_ProjectYTD.Visible = true;
        this.lbtn_modifyprojectYTD.Enabled = false;
        this.lnkButtonModifyCustmer.Enabled = false;
    }

    protected void lnkButtonModifyCustmer_Click(object sender, EventArgs e)
    {
        // By DingJunjie 20110517 Item 7 Add Start
        this.hidButtomFlag.Value = "true";
        // By DingJunjie 20110517 Item 7 Add End
        //bindDataSource(this.gv_Customer, getEditProject());
        bindGVCustomer();
        this.panel_addcustomerYTD.Visible = false;
        this.lbtn_addprojectYTD.Enabled = false;
        this.panel_addprojectYTD.Visible = false;
        this.lbtn_addcustomerYTD.Enabled = false;
        this.p_ProjectYTD.Visible = false;
        this.lbtn_modifyprojectYTD.Enabled = false;

        this.lbtn_modifyprojectYTD.Enabled = false;
        this.panelCustomer.Visible = true;
        this.lnkButtonModifyCustmer.Enabled = false;
    }

    /// <summary>
    /// Cancel Project List
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnYTDProCancel_Click(object sender, EventArgs e)
    {
        // By DingJunjie 20110517 Item 7 Add Start
        this.hidButtomFlag.Value = "true";
        // By DingJunjie 20110517 Item 7 Add End
        this.lbtn_addprojectYTD.Text = "Add Project";
        this.p_ProjectYTD.Visible = false;
        this.lbtn_addprojectYTD.Enabled = true;
        this.lbtn_addcustomerYTD.Enabled = true;
        this.lbtn_modifyprojectYTD.Enabled = true;
        this.label_addYTD.Text = "";
        this.lblError.Text = "";
        this.label_addYTD.Visible = false;
        this.lblError.Visible = false;
        this.lnkButtonModifyCustmer.Enabled = true;
        this.gv_ProjectYTD.Columns.Clear();

    }

    protected void btnCusCancel_Click(object sender, EventArgs e)
    {
        //this.hidButtomFlag.Value = "true";
        //this.lbtn_addprojectYTD.Text = "Add Project";
        //this.p_ProjectYTD.Visible = false;
        this.panelCustomer.Visible = false;
        this.lbtn_addprojectYTD.Enabled = true;
        this.lbtn_addcustomerYTD.Enabled = true;
        this.lbtn_modifyprojectYTD.Enabled = true;
        this.lnkButtonModifyCustmer.Enabled = true;
        this.label_addYTD.Text = "";
        this.lblError.Text = "";
        this.label_addYTD.Visible = false;
        this.lblError.Visible = false;
        this.gv_Customer.Columns.Clear();
    }

    /// <summary>
    /// Edit Project Info
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void gv_Project_RowEditing(object sender, GridViewEditEventArgs e)
    {
        GridView gv_Project = sender as GridView;
        gv_Project.EditIndex = e.NewEditIndex;
        gv_Project.Columns.Clear();
        bindDataSource(gv_Project, getEditProject());
    }

    protected void gv_Customer_RowEditing(object sender, GridViewEditEventArgs e)
    {
        GridView gv_Customer = sender as GridView;
        gv_Customer.EditIndex = e.NewEditIndex;
        gv_Customer.Columns.Clear();
        //bindDataSource(gv_Customer, getEditProject());
        bindGVCustomer();
    }

    /// <summary>
    /// Update Project Info
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void gv_Project_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        GridView gv_Project = sender as GridView;
        string str_id = gv_Project.Rows[e.RowIndex].Cells[0].Text.Trim();
        string str_name = gv_Project.Rows[e.RowIndex].Cells[2].Text.Trim();
        string str_value = ((TextBox)(gv_Project.Rows[e.RowIndex].Cells[6].Controls[0])).Text.Trim().Replace(',', '.');
        string str_probability = ((TextBox)(gv_Project.Rows[e.RowIndex].Cells[7].Controls[0])).Text.Trim().Replace(',', '.');
        string str_comments = ((TextBox)(gv_Project.Rows[e.RowIndex].Cells[9].Controls[0])).Text.Trim();
        GridView vg_project = (GridView)sender;
        updproject(gv_Project.ID, str_id, str_name, str_value, str_probability, str_comments);
        gv_Project.Columns.Clear();
        gv_Project.EditIndex = -1;
        bindDataSource(gv_Project, getEditProject());
    }

    protected void gv_Customer_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        GridView gv_Customer = sender as GridView;
        string recordId = gv_Customer.Rows[e.RowIndex].Cells[0].Text.Trim();
        string nameId = gv_Customer.Rows[e.RowIndex].Cells[1].Text.Trim();

        //object obj = hdCustType.Value + hdCustRegion.Value;

        updateCustomerName(nameId, ((TextBox)(gv_Customer.Rows[e.RowIndex].Cells[4].Controls[0])).Text.Trim());



        string typeId = hdCustType.Value;
        string regionId = hdCustRegion.Value;
        string city=((TextBox)(gv_Customer.Rows[e.RowIndex].Cells[7].Controls[0])).Text.Trim();
        string address = ((TextBox)(gv_Customer.Rows[e.RowIndex].Cells[8].Controls[0])).Text.Trim();
        string dept=((TextBox)(gv_Customer.Rows[e.RowIndex].Cells[9].Controls[0])).Text.Trim();


        updateCustomer(recordId
            , typeId
            , regionId
        , city
            , address
                , dept);

        gv_Customer.EditIndex = -1;
        bindGVCustomer();

        //string str_name = gv_Customer.Rows[e.RowIndex].Cells[2].Text.Trim();
        //string str_value = ((TextBox)(gv_Customer.Rows[e.RowIndex].Cells[6].Controls[0])).Text.Trim().Replace(',', '.');
        //string str_probability = ((TextBox)(gv_Customer.Rows[e.RowIndex].Cells[7].Controls[0])).Text.Trim().Replace(',', '.');
        //string str_comments = ((TextBox)(gv_Customer.Rows[e.RowIndex].Cells[9].Controls[0])).Text.Trim();
        //GridView vg_project = (GridView)sender;
        //updproject(gv_Customer.ID, str_id, str_name, str_value, str_probability, str_comments);
        //gv_Customer.Columns.Clear();
        //gv_Customer.EditIndex = -1;
        //bindDataSource(gv_Customer, getEditProject());
    }

    private void updateCustomerName(string nameId, string name)
    {
        string sqlStr = "update CustomerName set Name='" + name + "' where id=" + nameId;
        helper.ExecuteNonQuery(CommandType.Text, sqlStr);
    }

    private void updateCustomer(string id, string typeId, string regionId, string city, string address, string dept)
    {
        SqlParameter[] parms = new SqlParameter[6];
        parms[0] = new SqlParameter("@TypeID", Convert.ToInt32(typeId));
        parms[1] = new SqlParameter("@CountryID", Convert.ToInt32(regionId));

        parms[2] = new SqlParameter("@City", city);
        parms[3] = new SqlParameter("@Address", address);
        parms[4] = new SqlParameter("@Department", dept);
        parms[5] = new SqlParameter("@ID", Convert.ToInt32(id));

        string strSql = "UPDATE Customer set TypeID=@TypeID,CountryID=@CountryID,City=@City,Address=@Address,Department=@Department where ID=@ID;";
        helper.ExecuteNonQuery(CommandType.Text, strSql, parms);

    }

    /// <summary>
    /// Cancel Edit Projct Info
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void gv_Project_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        GridView gv_Project = sender as GridView;
        gv_Project.Columns.Clear();
        gv_Project.EditIndex = -1;
        bindDataSource(gv_Project, getEditProject());
    }

    protected void gv_Customer_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        GridView gv_Customer = sender as GridView;
        gv_Customer.Columns.Clear();
        gv_Customer.EditIndex = -1;
        //bindDataSource(gv_Project, getEditProject());
        bindGVCustomer();
    }

    /// <summary>
    /// Delete Project Info
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void gv_Project_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        GridView gv_Project = sender as GridView;
        Label label_del = new Label();
        if (string.Equals(gv_Project.ID, "gv_ProjectYTD"))
        {
            label_del = this.label_addYTD;
        }
        label_del.Visible = true;
        string str_ID = gv_Project.Rows[e.RowIndex].Cells[0].Text.Trim();
        string str_name = gv_Project.Rows[e.RowIndex].Cells[2].Text.Trim();
        string del_project = "UPDATE [Project] SET Deleted=1 WHERE ID=" + str_ID;
        int delcount = helper.ExecuteNonQuery(CommandType.Text, del_project, null);
        if (delcount > 0)
        {
            label_del.ForeColor = System.Drawing.Color.Green;
            label_del.Text = info.delLabelInfo(str_name, true);
        }
        else
        {
            label_del.ForeColor = System.Drawing.Color.Red;
            label_del.Text = info.delLabelInfo(str_name, false);
        }
        gv_Project.Columns.Clear();
        bindDataSource(gv_Project, getEditProject());
    }

    protected void gv_Customer_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        GridView gv_Customer = sender as GridView;
        Label label_del = this.label_addYTD;
        string str_name = gv_Customer.Rows[e.RowIndex].Cells[4].Text.Trim();
        
        string str_ID = gv_Customer.Rows[e.RowIndex].Cells[0].Text.Trim();

        string del_project = "UPDATE [Customer] SET Deleted=1 WHERE ID=" + str_ID;
        int delcount = helper.ExecuteNonQuery(CommandType.Text, del_project, null);
        if (delcount > 0)
        {
            label_del.ForeColor = System.Drawing.Color.Green;
            label_del.Text = info.delLabelInfo(str_name, true);
        }
        else
        {
            label_del.ForeColor = System.Drawing.Color.Red;
            label_del.Text = info.delLabelInfo(str_name, false);
        }
        gv_Customer.Columns.Clear();
        bindGVCustomer();
    }

    protected void gv_Customer_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        this.gv_Customer.PageIndex = e.NewPageIndex;

        bindGVCustomer();
    } 


    /// <summary>
    /// Update Project
    /// </summary>
    /// <param name="gv_project_ID">GridView ID</param>
    /// <param name="str_ID">Project ID</param>
    /// <param name="str_name">Project Name</param>
    /// <param name="str_value">Value</param>
    /// <param name="str_probability">Probability</param>
    /// <param name="str_comments">Comments</param>
    private void updproject(string gv_project_ID, string str_ID, string str_name,
        string str_value, string str_probability, string str_comments)
    {
        Label label_del = new Label();
        if (string.Equals(gv_project_ID, "gv_ProjectYTD"))
        {
            label_del = this.label_addYTD;
        }
        label_del.Visible = true;
        label_del.ForeColor = System.Drawing.Color.Red;
        if (str_value.Trim().Length == 0)
        {
            label_del.Text = info.addillegal("Project Value is null.");
            return;
        }

        if (str_probability.Trim().Length == 0)
        {
            label_del.Text = info.addillegal("Probability is null.");
            return;
        }

        //Edit by Forquan at 2011-12-29 begin


        string projectCurrency = hidProject_Currency.Value; 


        string sql = "UPDATE [Project] SET"
                   + " Value = '" + str_value + "',"
                   + " Probability = '" + str_probability + "',"
                   + " Comments = '" + str_comments + "',"
                   +" CurrencyID='"+ projectCurrency +"'"
                   + " WHERE ID =" + str_ID;


        //Edit by Forquan at 2011-12-29 end

        int count = helper.ExecuteNonQuery(CommandType.Text, sql, null);
        if (count == 1)
        {
            label_del.ForeColor = System.Drawing.Color.Green;
            label_del.Text = info.edtLabelInfo(str_name, true);
        }
        else
        {
            label_del.Text = info.edtLabelInfo(str_name, false);
        }
    }
    // By DingJunjie 20110602 ItemW28 Add Start
    int i = 0;
    // By DingJunjie 20110602 ItemW28 Add End
    // By mbq 20110602 ItemW25 Add Start
    protected void gv_modifyprojectYTD_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        // By DingJunjie 20110602 ItemW28 Add Start
        i++;
        // By DingJunjie 20110602 ItemW28 Add End
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (e.Row.RowState == DataControlRowState.Normal || e.Row.RowState == DataControlRowState.Alternate)
            {
                // By DingJunjie 20110602 ItemW28 Add Start
                ((ImageButton)e.Row.Cells[gv_ProjectYTD.Columns.Count - 2].Controls[0]).Attributes.Add("onclick", "setFocusInfo('gv_ProjectYTD','" + (i - 1) + "');");
                ((ImageButton)e.Row.Cells[gv_ProjectYTD.Columns.Count - 1].Controls[0]).Attributes.Add("onclick", info.setRowDataBound(e.Row.Cells[2].Text) + "setFocusInfo('gv_ProjectYTD','" + (i - 1) + "');");
                // By DingJunjie 20110602 ItemW28 Add End
                // By DingJunjie 20110602 ItemW28 Delete Start
                //((ImageButton)e.Row.Cells[gv_ProjectYTD.Columns.Count - 1].Controls[0]).Attributes.Add("onclick", info.setRowDataBound(e.Row.Cells[2].Text)+"");
                // By DingJunjie 20110602 ItemW28 Delete End
            }

            //Edit by Forquan at 2011-12-29 begin

            if (e.Row.RowIndex == gv_ProjectYTD.EditIndex)
            {
                string text = e.Row.Cells[8].Text;
                DropDownList ddList = new DropDownList();

                ddList.ID = "ddlProject_Currency";
                ddList.Attributes.Add("onchange", "listchange(this,'" + hidProject_Currency.ClientID + "')");
                bindDrop(GetCurrencyList(), ddList);
                // DropDownList default value
                for (int K = 0; K < ddList.Items.Count; K++)
                {
                    if (ddList.Items[K].Text == text.Trim())
                    {
                        ddList.SelectedIndex = i;
                        hidProject_Currency.Value = ddList.SelectedValue;
                        break;
                    }
                }
                e.Row.Cells[8].Controls.Add(ddList);
            }
        }
    }

    protected void gv_Customer_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {

            if (e.Row.RowIndex == gv_Customer.EditIndex)
            {
                //string text = e.Row.Cells[8].Text;
                //DropDownList ddList = new DropDownList();

                //ddList.ID = "ddlProject_Currency";
                //ddList.Attributes.Add("onchange", "listchange(this,'" + hidProject_Currency.ClientID + "')");
                //bindDrop(GetCurrencyList(), ddList);
                
                //for (int K = 0; K < ddList.Items.Count; K++)
                //{
                //    if (ddList.Items[K].Text == text.Trim())
                //    {
                //        ddList.SelectedIndex = i;
                //        hidProject_Currency.Value = ddList.SelectedValue;
                //        break;
                //    }
                //}
                //e.Row.Cells[8].Controls.Add(ddList);

                string customerType = e.Row.Cells[5].Text.Trim();
                DropDownList dpType = new DropDownList();
                dpType.ID = "dp11";
                dpType.Attributes.Add("onchange", "listchange(this,'" + hdCustType.ClientID + "')");
                dpType.DataSource = getCustomerType();
                dpType.DataTextField = "Name";
                dpType.DataValueField = "ID";
                dpType.DataBind();
                dpType.SelectedValue = e.Row.Cells[2].Text;
                hdCustType.Value = e.Row.Cells[2].Text;
                //for (int K = 0; K < dpType.Items.Count; K++)
                //{
                //    if (dpType.Items[K].Text == customerType)
                //    {
                //        dpType.Items[K].Selected = true;
                //        break;
                //    }
                //}
                e.Row.Cells[5].Controls.Add(dpType);

                string subRegionName = e.Row.Cells[6].Text.Trim();
                DropDownList dpSubRegion = new DropDownList();
                dpSubRegion.ID = "dp22";
                dpSubRegion.Attributes.Add("onchange", "listchange(this,'" + hdCustRegion.ClientID + "')");
                dpSubRegion.DataSource = getSubRegion();
                dpSubRegion.DataTextField = "Name";
                dpSubRegion.DataValueField = "ID";
                dpSubRegion.DataBind();
                dpSubRegion.SelectedValue = e.Row.Cells[3].Text;
                hdCustRegion.Value = e.Row.Cells[3].Text;
                //for (int K = 0; K < dpSubRegion.Items.Count; K++)
                //{
                //    if (dpSubRegion.Items[K].Text == subRegionName)
                //    {
                //        dpSubRegion.Items[K].Selected = true;
                //        break;
                //    }
                //}
                e.Row.Cells[6].Controls.Add(dpSubRegion);

            }
        }
    }

    private DataSet getSubRegion()
    {
        string strSql = "SELECT [ID],[Name] from [SubRegion]  where Deleted=0  order by Name";
        return helper.GetDataSet(strSql);
    }

    public DataSet getCustomerType()
    {
        string sqlStr = "SELECT [ID],[Name] from [CustomerType] where Deleted=0";
        return helper.GetDataSet(sqlStr);
    }

    
    private DataSet GetCurrencyList()
    {
        DataSet DSCurrency = sql.getCurrencyName();
        return DSCurrency;
    }


    //Edit by Forquan at 2011-12-29 End

    // By mbq 20110602 ItemW25 Add End

    /// <summary>
    /// Get Project Info
    /// </summary>
    /// <returns>Project Info</returns>
    protected DataSet getProjectInfo()
    {
        StringBuilder sql = new StringBuilder();
        sql.AppendLine(" SELECT ");
        sql.AppendLine("   Project.ID, ");
        sql.AppendLine("   Project.PODID, ");
        sql.AppendLine("   Project.Name AS 'Project Name', ");
        sql.AppendLine("   Segment.Abbr AS 'Segment', ");
        sql.AppendLine("   CustomerName.Name AS 'Customer Name', ");
        sql.AppendLine("   Country.ISO_Code AS 'Project Country(POS)', ");
        sql.AppendLine("   Project.Value AS 'Project Value', ");
        sql.AppendLine("   Project.Probability AS '% in budget', ");
        sql.AppendLine("   Currency.Name AS 'Currency', ");
        sql.AppendLine("   Project.Comments, ");
        sql.AppendLine("   ISNULL(Project.Value*Project.Probability/100, 0) AS 'absolute in budget', ");
        sql.AppendLine("   CountryPOD.ISO_Code AS 'Project Country(POD)' ");
        sql.AppendLine(" FROM ");
        sql.AppendLine("   Project ");
        sql.AppendLine("   INNER JOIN Country ON Project.POSID=Country.ID ");
        sql.AppendLine("   INNER JOIN Country AS CountryPOD ON Project.PoDID=CountryPOD.ID ");
        //by yyan itemw78 20110720 del start
        sql.AppendLine("   INNER JOIN Customer ON Project.CustomerNameID=Customer.ID ");
        sql.AppendLine("   INNER JOIN CustomerName ON Customer.NameID=CustomerName.ID ");
        //by yyan itemw78 20110720 del end
        //by yyan itemw78 20110720 add start
        //sql.AppendLine("   INNER JOIN CustomerName ON Project.CustomerNameID=CustomerName.ID ");
        //by yyan itemw78 20110720 add end
        sql.AppendLine("   INNER JOIN Currency ON Project.CurrencyID=Currency.ID ");
        sql.AppendLine("   INNER JOIN Segment ON Project.ProSegmentID=Segment.ID ");
        sql.AppendLine(" WHERE ");
        sql.AppendLine("   Project.Deleted=0 ");
        sql.AppendLine("   AND Country.Deleted=0 ");
        sql.AppendLine("   AND CustomerName.Deleted=0 ");
        //by yyan itemw78 20110720 del start
        //sql.AppendLine("   AND Customer.Deleted=0 ");
        //by yyan itemw78 20110720 del end
        sql.AppendLine("   AND Segment.Deleted=0 ");
        sql.AppendLine(" ORDER BY ");
        sql.AppendLine("   Project.Name, ");
        sql.AppendLine("   Country.ISO_Code, ");
        sql.AppendLine("   CustomerName.Name ASC ");
        DataSet ds_project = helper.GetDataSet(sql.ToString());
        return ds_project;
    }

    /// <summary>
    /// Init Project Info
    /// </summary>
    /// <param name="gv_Project">GridView Object</param>
    /// <param name="ds_project">Project DataSet</param>
    protected void bindDataSource(GridView gv_Project, DataSet ds_project)
    {
        gv_Project.Columns.Clear();
        bool notNullFlag = true;
        if (ds_project.Tables[0].Rows.Count == 0)
        {
            notNullFlag = false;
            sql.getNullDataSet(ds_project);
        }
        gv_Project.Width = Unit.Pixel(1000);
        gv_Project.AutoGenerateColumns = false;
        gv_Project.Visible = true;

        //addCalculateCol(ds_project);
        //addPODCol(ds_project);

        for (int i = 0; i < ds_project.Tables[0].Columns.Count; i++)
        {
            BoundField bf = new BoundField();

            bf.DataField = ds_project.Tables[0].Columns[i].ColumnName.ToString();
            bf.HeaderText = ds_project.Tables[0].Columns[i].Caption.ToString();
            bf.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
            bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
            bf.ReadOnly = true;

            if (i == 6 || i == 7 || i == 9)
                bf.ReadOnly = false;

            gv_Project.Columns.Add(bf);
        }

        CommandField cf_Update = new CommandField();
        cf_Update.ButtonType = ButtonType.Image;
        cf_Update.ShowEditButton = true;
        cf_Update.ShowCancelButton = true;
        cf_Update.EditImageUrl = "~/images/edit.jpg";
        cf_Update.EditText = "Edit";
        cf_Update.CausesValidation = false;
        cf_Update.CancelImageUrl = "~/images/cancel.jpg";
        cf_Update.CancelText = "Cancel";
        cf_Update.UpdateImageUrl = "~/images/ok.jpg";
        cf_Update.UpdateText = "Update";
        gv_Project.Columns.Add(cf_Update);

        CommandField cf_Delete = new CommandField();
        cf_Delete.ButtonType = ButtonType.Image;
        cf_Delete.ShowDeleteButton = true;
        cf_Delete.ShowCancelButton = true;
        cf_Delete.CausesValidation = false;
        cf_Delete.DeleteImageUrl = "~/images/del.jpg";
        cf_Delete.DeleteText = "Delete";
        gv_Project.Columns.Add(cf_Delete);

        gv_Project.AllowSorting = true;
        gv_Project.DataSource = ds_project.Tables[0];
        gv_Project.DataBind();

        gv_Project.Columns[gv_Project.Columns.Count - 1].Visible = notNullFlag;
        gv_Project.Columns[gv_Project.Columns.Count - 2].Visible = notNullFlag;
        gv_Project.Columns[0].Visible = false;
        gv_Project.Columns[1].Visible = false;
        // By DingJunjie 20110516 Item 7 Delete Start
        //if (getRoleID(getRole()) != "0")
        // By DingJunjie 20110516 Item 7 Delete End
        // By DingJunjie 20110516 Item 7 Add Start
        if (!string.Equals(getRoleID(getRole()), "0")
            && !string.Equals(getRoleID(getRole()), "3")
            && !string.Equals(getRoleID(getRole()), "4"))
        // By DingJunjie 20110516 Item 7 Add Start
        {
            gv_Project.Columns[gv_Project.Columns.Count - 2].Visible = false;
            gv_Project.Columns[gv_Project.Columns.Count - 1].Visible = false;
        }
        gv_Project.Visible = true;
    }


    protected void bindGVCustomer()
    {
        DataSet ds_customer = getEditCustomer();
        if (ds_customer == null)
            return;
        gv_Customer.Columns.Clear();
        bool notNullFlag = true;
        if (ds_customer.Tables[0].Rows.Count == 0)
        {
            notNullFlag = false;
            sql.getNullDataSet(ds_customer);
        }
        gv_Customer.Width = Unit.Pixel(1000);
        gv_Customer.AutoGenerateColumns = false;
        gv_Customer.Visible = true;

        //addCalculateCol(ds_project);
        //addPODCol(ds_project);

        for (int i = 0; i < ds_customer.Tables[0].Columns.Count; i++)
        {
            BoundField bf = new BoundField();

            bf.DataField = ds_customer.Tables[0].Columns[i].ColumnName.ToString();
            bf.HeaderText = ds_customer.Tables[0].Columns[i].Caption.ToString();
            bf.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
            bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
            bf.ReadOnly = true;

            //if (i == 6 || i == 7 || i == 9)
            //    bf.ReadOnly = false;

            if (i < 4)
                bf.ReadOnly = true;
            else if (i == 5 || i == 6)
                bf.ReadOnly = true;
            else
                bf.ReadOnly = false;

            gv_Customer.Columns.Add(bf);
        }

        CommandField cf_Update = new CommandField();
        cf_Update.ButtonType = ButtonType.Image;
        cf_Update.ShowEditButton = true;
        cf_Update.ShowCancelButton = true;
        cf_Update.EditImageUrl = "~/images/edit.jpg";
        cf_Update.EditText = "Edit";
        cf_Update.CausesValidation = false;
        cf_Update.CancelImageUrl = "~/images/cancel.jpg";
        cf_Update.CancelText = "Cancel";
        cf_Update.UpdateImageUrl = "~/images/ok.jpg";
        cf_Update.UpdateText = "Update";
        gv_Customer.Columns.Add(cf_Update);

        CommandField cf_Delete = new CommandField();
        cf_Delete.ButtonType = ButtonType.Image;
        cf_Delete.ShowDeleteButton = true;
        cf_Delete.ShowCancelButton = true;
        cf_Delete.CausesValidation = false;
        cf_Delete.DeleteImageUrl = "~/images/del.jpg";
        cf_Delete.DeleteText = "Delete";
        gv_Customer.Columns.Add(cf_Delete);

        gv_Customer.AllowSorting = true;
        gv_Customer.DataSource = ds_customer.Tables[0];
        gv_Customer.DataBind();

        //gv_Customer.Columns[gv_Customer.Columns.Count - 1].Visible = notNullFlag;
        //gv_Customer.Columns[gv_Customer.Columns.Count - 2].Visible = notNullFlag;
        //gv_Customer.Columns[0].Visible = false;
        //gv_Customer.Columns[1].Visible = false;
       
        //if (!string.Equals(getRoleID(getRole()), "0")
        //    && !string.Equals(getRoleID(getRole()), "3")
        //    && !string.Equals(getRoleID(getRole()), "4"))
        //// By DingJunjie 20110516 Item 7 Add Start
        //{
        //    gv_Customer.Columns[gv_Customer.Columns.Count - 2].Visible = false;
        //    gv_Customer.Columns[gv_Customer.Columns.Count - 1].Visible = false;
        //}

        gv_Customer.Columns[0].Visible = false;
        gv_Customer.Columns[1].Visible = false;
        gv_Customer.Columns[2].Visible = false;
        gv_Customer.Columns[3].Visible = false;
        gv_Customer.Visible = true;
    }

    /// <summary>
    /// Add [absolute in budget] Info
    /// </summary>
    /// <param name="ds"></param>
    private void addCalculateCol(DataSet ds)
    {
        ds.Tables[0].Columns.Add("absolute in budget");
        if (ds.Tables[0].Rows[0][0] != DBNull.Value)
        {
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                string str_value = dr[6].ToString().Trim();
                string str_probility = dr[7].ToString().Trim();
                if (str_value == "")
                    str_value = "0";
                if (str_probility == "")
                    str_probility = "";
                float value = float.Parse(dr[6].ToString().Trim());
                float probility = float.Parse(dr[7].ToString().Trim());
                dr["absolute in budget"] = value * probility / 100;
            }
        }
    }

    /// <summary>
    /// Add [Project Country(POD)] Info
    /// </summary>
    /// <param name="ds"></param>
    private void addPODCol(DataSet ds)
    {
        ds.Tables[0].Columns.Add("Project Country(POD)");
        if (ds.Tables[0].Rows[0][0] != DBNull.Value)
        {
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                string str_countryID = dr[1].ToString().Trim();
                string str_countryISO = sql.getSubRegionByID(str_countryID);
                dr["Project Country(POD)"] = str_countryISO;
            }
        }
    }

    /// <summary>
    /// Get All Project List
    /// </summary>
    /// <returns>All Project List</returns>
    private DataSet getProject()
    {
        StringBuilder sql = new StringBuilder();
        sql.AppendLine(" SELECT ");
        sql.AppendLine("   P.ID, ");
        sql.AppendLine("   P.Name+'('+C.ISO_Code+')' Name ");
        sql.AppendLine(" FROM ");
        sql.AppendLine("   Project P LEFT JOIN Country C ON P.PoDID=C.ID ");
        sql.AppendLine(" WHERE ");
        sql.AppendLine("   P.Deleted=0 ");
        sql.AppendLine(" ORDER BY ");
        sql.AppendLine("   P.Name ");
        DataSet ds_project = helper.GetDataSet(sql.ToString());
        if (ds_project.Tables.Count > 0)
        {
            return ds_project;
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Get Project Info By ProjectID
    /// </summary>
    /// <param name="projectID">ProjectID</param>
    /// <returns>Project Info</returns>
    private DataSet getProjectByID(string projectID)
    {
        StringBuilder sql = new StringBuilder();
        sql.AppendLine(" SELECT ");
        sql.AppendLine("   P.Name,");
        sql.AppendLine("   P.PoDID");
        sql.AppendLine(" FROM ");
        sql.AppendLine("   Project P");
        sql.AppendLine(" WHERE ");
        sql.AppendLine("   P.ID=" + projectID);
        sql.AppendLine(" ORDER BY ");
        sql.AppendLine("   P.Name ");
        DataSet ds_project = helper.GetDataSet(sql.ToString());
        if (ds_project.Tables.Count > 0)
        {
            return ds_project;
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Get CountryID By SubRegionID
    /// </summary>
    /// <param name="projectID">SubRegionID</param>
    /// <returns>CountryID</returns>
    private string getCountryIDBySubRegion(string subRegionID)
    {
        StringBuilder sql = new StringBuilder();
        sql.AppendLine(" SELECT ");
        sql.AppendLine("   CS.CountryID ");
        sql.AppendLine(" FROM ");
        sql.AppendLine("   SubRegion SR INNER JOIN Country_SubRegion CS ON SR.ID=CS.SubRegionID ");
        sql.AppendLine(" WHERE ");
        sql.AppendLine("   SR.ID=" + subRegionID);
        DataSet ds_project = helper.GetDataSet(sql.ToString());
        if (ds_project.Tables.Count > 0)
        {
            return ds_project.Tables[0].Rows[0][0].ToString();
        }
        else
        {
            return "-1";
        }
    }

    /// <summary>
    /// Check The Project Is Exist Or Not
    /// </summary>
    /// <param name="str_name">Project Name</param>
    /// <param name="str_customerID">Customer ID</param>
    /// <param name="str_toID">POD ID</param>
    /// <param name="str_segmentID">Segment ID</param>
    /// <returns>Result</returns>
    private bool existproject(string str_name, string str_customerID, string str_from, string str_segmentID)
    {
        StringBuilder sql = new StringBuilder();
        sql.AppendLine(" SELECT ");
        sql.AppendLine("   ID ");
        sql.AppendLine(" FROM ");
        sql.AppendLine("   Project ");
        sql.AppendLine(" WHERE ");
        sql.AppendLine("   Deleted=0 ");
        sql.AppendLine("   AND Name='" + str_name.Replace("'", "''") + "'");
        if (!string.IsNullOrEmpty(str_customerID))
        {
            sql.AppendLine("   AND CustomerNameID=" + str_customerID);
        }
        sql.AppendLine("   AND PoSID=" + str_from);
        sql.AppendLine("   AND ProSegmentID=" + str_segmentID);
        DataSet ds_project = helper.GetDataSet(sql.ToString());
        if (ds_project.Tables.Count > 0 && ds_project.Tables[0].Rows.Count > 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    // By DingJunjie 20110512 Item 7 Add End

    // By DingJunjie 20110601 ItemW18 Add Start
    /// <summary>
    /// Update Bookings Data
    /// </summary>
    /// <param name="gv">GridView</param>
    /// <param name="rIndex">Row ID</param>
    /// <param name="bookingY">bookingY</param>
    /// <param name="deliverY">deliverY</param>
    /// <param name="label_note">Message Lable</param>
    private void update_booking(GridView gv, int rIndex, string bookingY, string deliverY, Label label_note)
    {
        label_note.Visible = true;
        label_note.Text = "";
        label_note.ForeColor = Color.Green;
        string str_RSMID = null;
        string str_segmentID = null;
        string str_salesorgID = null;
        string str_deliveryInFy = null;
        string str_NoInFy = null;
        string str_countryID = null;
        string str_customerID = null;
        string str_projectID = null;
        string str_salesChannelID = null;
        string str_preData = null;
        string pro = null;
        string proID = null;
        string strCurSubRegionID = null;
        string str_curcustomerID = null;
        string str_curProjectID = null;
        string str_curSalesChannelID = null;
        string str_nexOp = null;
        string str_nexOpID = null;
        string str_nexData = null;
        string str_subValue = null;
        string str_percentage = null;
        int count = 0;

        string recordId = gv.Rows[rIndex].Cells[9].Text.Trim();
        DataSet ds_product = getProductBySegment(getSegmentID());
        if (ds_product != null && ds_product.Tables.Count > 0 && ds_product.Tables[0].Rows.Count > 0)
        {
            DataSet ds = getBookingDataByDateByProduct(ds_product, bookingY, deliverY);
            if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
            {
                gv.EditIndex = -1;
                ScriptManager.RegisterStartupScript(this.upYTD1, this.upYTD1.GetType(), "NoDataError", "alert('There are any data in the row!');", true);
                return;
            }
            else
            {
                try
                {
                    str_RSMID = getRSMID().Trim();
                    str_segmentID = getSegmentID();
                    str_salesorgID = getSalesOrgID();
                    str_deliveryInFy = getDeliveryInFy(bookingY, deliverY);
                    str_NoInFy = getNoInFy(bookingY, deliverY);
                    str_countryID = ds.Tables[0].Rows[rIndex][0].ToString().Trim();
                    str_customerID = ds.Tables[0].Rows[rIndex][1].ToString().Trim();
                    str_customerID = (str_customerID == "") ? "-1" : str_customerID;
                    str_projectID = ds.Tables[0].Rows[rIndex][2].ToString().Trim();
                    str_projectID = (str_projectID == "") ? "-1" : str_projectID;
                    str_salesChannelID = ds.Tables[0].Rows[rIndex][3].ToString().Trim();
                    str_salesChannelID = (str_salesChannelID == "") ? "-1" : str_salesChannelID;
                    strCurSubRegionID = this.hidSubRegionID.Value;
                    strCurSubRegionID = string.IsNullOrEmpty(strCurSubRegionID) ? "-1" : strCurSubRegionID;
                    str_curcustomerID = this.HiddenCustomer.Value;
                    str_curcustomerID = (str_curcustomerID == "") ? "-1" : str_curcustomerID;
                    str_curProjectID = hidproject.Value;
                    str_curProjectID = (str_curProjectID == "") ? "-1" : str_curProjectID;
                    str_curSalesChannelID = this.hidsale.Value;
                    str_curSalesChannelID = (str_curSalesChannelID == "") ? "-1" : str_curSalesChannelID;
                }
                catch (Exception ex)
                {
                    gv.EditIndex = -1;
                    log.WriteLog(LogUtility.LogErrorLevel.LOG_SUSPICIOUS, ex.Message + ";e.RowIndex = " + rIndex + ".");
                    return;
                }
            }
            string[] hiddenValue = this.hidHiddenValue.Value.Trim('^').Split('^');
            string[] values = null;
            int valueIndex = 0;

            Dictionary<string, string> operationIds = GetHidValue();
            string opIds = string.Empty;
            Dictionary<int, string[]> opDic = new Dictionary<int, string[]>();
            for (int j = 11; j < gv.Columns.Count - 3; j += 7)
            {
                //str_nexOp = ((TextBox)(gv.Rows[rIndex].Cells[j + 3].Controls[0])).Text.ToString().Trim();
                //str_nexData = ((TextBox)(gv.Rows[rIndex].Cells[j + 1].Controls[0])).Text.ToString().Trim().Replace(',', '.');
                //if (Regex.IsMatch(str_nexData, "^\\-?(\\d*|0)(\\.\\d*)?$") && Regex.IsMatch(str_nexOp, "^([A-Z]{1,4})$"))
                //{
                    
                    //string id = getOperationIDByAbbr(str_nexOp).Trim();
                    //if (string.IsNullOrEmpty(id))
                    //{
                    //    label_note.ForeColor = Color.Red;
                    //    label_note.Text += "Operation whose abbr is '" + str_nexOp + "' isn't exist or unique.";
                    //    gv.EditIndex = -1;
                    //    return;
                    //}
                    //opDic.Add(j, id);
                string[] args = new string[] { gv.Rows[rIndex].Cells[j].Text, operationIds[gv.Rows[rIndex].Cells[j].Text], ((TextBox)(gv.Rows[rIndex].Cells[j + 1].Controls[0])).Text.ToString().Trim().Replace(',', '.') }; // productd,operationid,amount.
                    opDic.Add(j, args);
                //}
                //else
                //{
                //    label_note.ForeColor = Color.Red;
                //    label_note.Text += pro + "'value was illegal.";
                //    gv.EditIndex = -1;
                //    return;
                //}
                
            }
            if (opDic.Count < 1)
                return;
            if (!validateOperation(opDic, str_RSMID, str_salesorgID, str_segmentID, strCurSubRegionID, str_curcustomerID, str_curProjectID, str_curSalesChannelID, recordId))
            {
                gv.EditIndex = -1;
                ScriptManager.RegisterStartupScript(this.upYTD1, this.upYTD1.GetType(), "UpdateExistError", "alert('The record had been existed!');", true);
                return;
            }

            for (int j = 11; j < gv.Columns.Count - 3; j += 7)
            {
                pro = gv.HeaderRow.Cells[j + 1].Text;
                proID = gv.Rows[rIndex].Cells[j].Text;
                //str_nexOp = ((TextBox)(gv.Rows[rIndex].Cells[j + 3].Controls[0])).Text.ToString().Trim();
                str_preData = ds.Tables[0].Rows[rIndex][j + 1].ToString().Trim().Replace(',', '.');
                str_nexData = ((TextBox)(gv.Rows[rIndex].Cells[j + 1].Controls[0])).Text.ToString().Trim().Replace(',', '.');
                str_preData = string.IsNullOrEmpty(str_preData) ? "0" : str_preData;
                str_nexData = string.IsNullOrEmpty(str_nexData) ? "0" : str_nexData;
                 values = hiddenValue[valueIndex].Split(',');
                str_subValue = values[0].Trim();
                str_percentage = values[1].Trim();
                valueIndex++;
                //if (Regex.IsMatch(str_nexData, "^\\-?(\\d*|0)(\\.\\d*)?$") && Regex.IsMatch(str_nexOp, "^([A-Z]{1,4})$"))
                //{
                //    str_nexOpID = getOperationIDByAbbr(str_nexOp).Trim();
                //    if (string.IsNullOrEmpty(str_nexOpID))
                //    {
                //        label_note.ForeColor = Color.Red;
                //        label_note.Text += "Operation whose abbr is '" + str_nexOp + "' isn't exist or unique.";
                //        gv.EditIndex = -1;
                //        return;
                //    }
                //}
                //else
                //{
                //    label_note.ForeColor = Color.Red;
                //    label_note.Text += pro + "'value was illegal.";
                //    gv.EditIndex = -1;
                //    return;
                //}
                str_nexOpID = opDic[j][1];
                if (j == 11)
                {
                    if (inputCheck(strCurSubRegionID, str_curcustomerID, str_curSalesChannelID, label_note))
                    {
                        if (!string.Equals(str_countryID, strCurSubRegionID)
                            || !string.Equals(str_customerID, str_curcustomerID)
                            || !string.Equals(str_projectID, str_curProjectID)
                            || !string.Equals(str_salesChannelID, str_curSalesChannelID))
                        {
                            //if (existBookingsBaseData(str_RSMID, str_salesorgID, str_segmentID, strCurSubRegionID,
                            //    str_curcustomerID, str_curProjectID, str_curSalesChannelID))
                            //{
                                updBookingsBaseInfo(strCurSubRegionID, str_curcustomerID, str_curProjectID,
                                    str_curSalesChannelID, str_nexOpID, str_RSMID, str_salesorgID, str_segmentID,
                                    str_countryID, str_customerID, str_projectID, str_salesChannelID,recordId);
                                str_countryID = strCurSubRegionID;
                                str_customerID = str_curcustomerID;
                                str_projectID = str_curProjectID;
                                str_salesChannelID = str_curSalesChannelID;
                            //}
                            //else
                            //{
                            //    gv.EditIndex = -1;
                            //    ScriptManager.RegisterStartupScript(this.upYTD1, this.upYTD1.GetType(), "UpdateExistError", "alert('The record had been existed!');", true);
                            //    return;
                            //}
                        }
                    }
                    else
                    {
                        gv.EditIndex = -1;
                        return;
                    }
                }
                //yyan 20110907 itemw135 edit start
                if (str_salesChannelID != "1")
                {
                    str_subValue = str_nexData;
                    str_percentage = "";
                }
                else {
                    if (str_percentage=="")
                    {
                        str_subValue = str_nexData;
                    }
                }
                //yyan 20110907 itemw135 edit end
                count = updAmount(str_nexData, str_subValue, str_percentage, str_RSMID, str_salesorgID, str_segmentID, str_countryID,
                    str_customerID, str_projectID, str_salesChannelID, proID, bookingY, deliverY,recordId);
                if (count == 0)
                {
                    count = insAmount(str_RSMID, str_salesorgID, str_countryID, str_curcustomerID, bookingY, deliverY,
                        str_segmentID, proID, str_nexOpID, str_curProjectID, str_curSalesChannelID, str_nexData, str_subValue,
                        str_percentage, str_deliveryInFy, str_NoInFy,recordId);
                    if (count == 0)
                    {
                        label_note.ForeColor = Color.Red;
                        label_note.Text += pro + ",";
                    }
                }
                else if (count > 1)
                {
                    //duplicate. ...  not sure what cause the problem

                    delAmount(str_RSMID, str_salesorgID, str_segmentID, str_countryID,
                        str_customerID, str_projectID, str_salesChannelID, proID, bookingY, deliverY, recordId);
                    count = insAmount(str_RSMID, str_salesorgID, str_countryID, str_curcustomerID, bookingY, deliverY,
                        str_segmentID, proID, str_nexOpID, str_curProjectID, str_curSalesChannelID, str_nexData, str_subValue,
                        str_percentage, str_deliveryInFy, str_NoInFy, recordId);
                    if (count == 0)
                    {
                        label_note.ForeColor = Color.Red;
                        label_note.Text += pro + ",";
                    }
                }
                updOperation(str_nexOpID, str_RSMID, str_salesorgID, str_segmentID, str_countryID, str_customerID, str_projectID, str_salesChannelID, proID,recordId);
            }
        }
        if (string.IsNullOrEmpty(label_note.Text.Trim()))
        {
            ScriptManager.RegisterStartupScript(this.upYTD1, this.upYTD1.GetType(), "Success", "alert('Modified successfully!');", true);
        }
        else
        {
            label_note.ForeColor = Color.Red;
            label_note.Text += "please re-enter..";
            ScriptManager.RegisterStartupScript(this.upYTD1, this.upYTD1.GetType(), "Error", "alert('" + label_note.Text + "!');", true);
        }
        gv.EditIndex = -1;
    }

    /// <summary>
    /// Check Is Bookings Data Exist Or Not
    /// </summary>
    /// <param name="str_RSMID">UserID</param>
    /// <param name="str_salesorgID">Sales Organization ID</param>
    /// <param name="str_segmentID">Segment ID</param>
    /// <param name="str_countryID">Country ID</param>
    /// <param name="str_curcustomerID">Customer ID</param>
    /// <param name="str_curProjectID">Project ID</param>
    /// <param name="str_curSalesChannelID">Sales Channel ID</param>
    /// <returns>Result</returns>
    private bool existBookingsBaseData(string str_RSMID, string str_salesorgID, string str_segmentID,
        string str_countryID, string str_curcustomerID, string str_curProjectID, string str_curSalesChannelID)
    {
        StringBuilder sql = new StringBuilder();
        sql.AppendLine(" SELECT ");
        sql.AppendLine("   COUNT(*) COUNT ");
        sql.AppendLine(" FROM ");
        sql.AppendLine("   Bookings ");
        sql.AppendLine(" WHERE ");
        sql.AppendLine("   RSMID=" + str_RSMID);
        sql.AppendLine("   AND SalesOrgID=" + str_salesorgID);
        sql.AppendLine("   AND SegmentID=" + str_segmentID);
        sql.AppendLine("   AND CountryID=" + str_countryID);
        sql.AppendLine("   AND CustomerID=" + str_curcustomerID);
        sql.AppendLine("   AND ProjectID=" + str_curProjectID);
        sql.AppendLine("   AND SalesChannelID=" + str_curSalesChannelID);
        sql.AppendLine("   AND YEAR(TimeFlag)=" + year);
        sql.AppendLine("   AND MONTH(TimeFlag)=" + month);
        DataSet ds = helper.GetDataSet(sql.ToString());
        if (Convert.ToInt32(ds.Tables[0].Rows[0][0]) == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Update Bookings Base Data
    /// </summary>
    /// <param name="strCurSubRegionID">New SubRegion ID</param>
    /// <param name="str_curcustomerID">New Customer ID</param>
    /// <param name="str_curProjectID">New Project ID</param>
    /// <param name="str_curSalesChannelID">New Sales Channel ID</param>
    /// <param name="str_nexOpID">New Operation ID</param>
    /// <param name="str_RSMID">UserID</param>
    /// <param name="str_salesorgID">Sales Organization ID</param>
    /// <param name="str_segmentID">Segment ID</param>
    /// <param name="str_countryID">Country ID</param>
    /// <param name="str_customerID">Customer ID</param>
    /// <param name="str_projectID">Project ID</param>
    /// <param name="str_salesChannelID">Sales Channel ID</param>
    private void updBookingsBaseInfo(string strCurSubRegionID, string str_curcustomerID, string str_curProjectID,
        string str_curSalesChannelID, string str_nexOpID, string str_RSMID, string str_salesorgID, string str_segmentID,
        string str_countryID, string str_customerID, string str_projectID, string str_salesChannelID,string recordId)
    {
        StringBuilder sql = new StringBuilder();
        sql.AppendLine(" UPDATE ");
        sql.AppendLine("   Bookings ");
        sql.AppendLine(" SET ");
        sql.AppendLine("   CountryID=" + strCurSubRegionID + ",");
        sql.AppendLine("   CustomerID=" + str_curcustomerID + ",");
        sql.AppendLine("   ProjectID=" + str_curProjectID + ",");
        sql.AppendLine("   SalesChannelID=" + str_curSalesChannelID);
        sql.AppendLine(" WHERE ");
        sql.AppendLine("  RSMID=" + str_RSMID);
        sql.AppendLine("  AND SalesOrgID=" + str_salesorgID);
        sql.AppendLine("  AND SegmentID=" + str_segmentID);
        sql.AppendLine("  AND CountryID=" + str_countryID);
        sql.AppendLine("  AND CustomerID=" + str_customerID);
        sql.AppendLine("  AND ProjectID = " + str_projectID);
        sql.AppendLine("  AND SalesChannelID = " + str_salesChannelID);
        sql.AppendLine("  AND Year(TimeFlag)='" + year + "'");
        sql.AppendLine("  AND Month(TimeFlag)='" + month + "'");
        sql.AppendLine(" and RecordID=" + recordId);
        helper.ExecuteNonQuery(CommandType.Text, sql.ToString(), null);
    }

    /// <summary>
    /// delete dup Bookings 
    /// </summary>
    private int delAmount(string str_RSMID,
        string str_salesorgID, string str_segmentID, string str_countryID, string str_customerID,
        string str_projectID, string str_salesChannelID, string proID, string bookingY, string deliverY, string recordId)
    {
        StringBuilder sql = new StringBuilder();
        // should add if not exist
        sql.AppendLine(" delete ");
        sql.AppendLine("   Bookings ");
        sql.AppendLine(" WHERE ");
        sql.AppendLine("  RSMID=" + str_RSMID);
        sql.AppendLine("  AND SalesOrgID=" + str_salesorgID);
        sql.AppendLine("  AND SegmentID=" + str_segmentID);
        sql.AppendLine("  AND CountryID=" + str_countryID);
        sql.AppendLine("  AND CustomerID=" + str_customerID);
        sql.AppendLine("  AND ProjectID = " + str_projectID);
        sql.AppendLine("  AND SalesChannelID = " + str_salesChannelID);
        sql.AppendLine("  AND ProductID = " + proID);
        sql.AppendLine("  AND BookingY='" + bookingY + "'");
        sql.AppendLine("  AND DeliverY='" + deliverY + "'");
        sql.AppendLine("  AND Year(TimeFlag)=" + year);
        sql.AppendLine("  AND Month(TimeFlag)=" + month);
        sql.AppendLine(" and RecordID=" + recordId);
        return helper.ExecuteNonQuery(CommandType.Text, sql.ToString(), null);
    }

    /// <summary>
    /// Update Bookings Amount
    /// </summary>
    /// <param name="str_nexData">New Amount</param>
    /// <param name="str_subValue">Value</param>
    /// <param name="str_percentage">Percentage</param>
    /// <param name="str_RSMID">UserID</param>
    /// <param name="str_salesorgID">Sales Organization ID</param>
    /// <param name="str_segmentID">Segment ID</param>
    /// <param name="str_countryID">Country ID</param>
    /// <param name="str_customerID">Customer ID</param>
    /// <param name="str_projectID">Project ID</param>
    /// <param name="str_salesChannelID">Sales Channel ID</param>
    /// <param name="str_preOpID">Operation ID</param>
    /// <param name="proID">Product ID</param>
    /// <param name="bookingY">bookingY</param>
    /// <param name="deliverY">deliverY</param>
    /// <returns>Execute Count</returns>
    private int updAmount(string str_nexData, string str_subValue, string str_percentage, string str_RSMID,
        string str_salesorgID, string str_segmentID, string str_countryID, string str_customerID,
        string str_projectID, string str_salesChannelID, string proID, string bookingY, string deliverY,string recordId)
    {
        StringBuilder sql = new StringBuilder();
        // should add if not exist
        sql.AppendLine(" UPDATE ");
        sql.AppendLine("   Bookings ");
        sql.AppendLine(" SET ");
        sql.AppendLine("   Amount=" + str_nexData);
        if (string.IsNullOrEmpty(str_subValue))
        {
            sql.AppendLine("   ,Value=NULL ");
        }
        else
        {
            sql.AppendLine("   ,Value=" + str_subValue);
        }
        if (string.IsNullOrEmpty(str_percentage))
        {
            sql.AppendLine("   ,Percentage=NULL ");
        }
        else
        {
            sql.AppendLine("   ,Percentage=" + str_percentage);
        }
        sql.AppendLine(" WHERE ");
        sql.AppendLine("  RSMID=" + str_RSMID);
        sql.AppendLine("  AND SalesOrgID=" + str_salesorgID);
        sql.AppendLine("  AND SegmentID=" + str_segmentID);
        sql.AppendLine("  AND CountryID=" + str_countryID);
        sql.AppendLine("  AND CustomerID=" + str_customerID);
        sql.AppendLine("  AND ProjectID = " + str_projectID);
        sql.AppendLine("  AND SalesChannelID = " + str_salesChannelID);
        sql.AppendLine("  AND ProductID = " + proID);
        sql.AppendLine("  AND BookingY='" + bookingY + "'");
        sql.AppendLine("  AND DeliverY='" + deliverY + "'");
        sql.AppendLine("  AND Year(TimeFlag)=" + year);
        sql.AppendLine("  AND Month(TimeFlag)=" + month);
        sql.AppendLine(" and RecordID=" + recordId);
        return helper.ExecuteNonQuery(CommandType.Text, sql.ToString(), null);
    }

    /// <summary>
    /// Insert Bookings Amount
    /// </summary>
    /// <param name="str_RSMID">UserID</param>
    /// <param name="str_salesorgID">Sales Organization ID</param>
    /// <param name="str_countryID">Country ID</param>
    /// <param name="str_curcustomerID">Customer ID</param>
    /// <param name="bookingY">bookingY</param>
    /// <param name="deliverY">deliverY</param>
    /// <param name="str_segmentID">Segment ID</param>
    /// <param name="proID">Product ID</param>
    /// <param name="operationID">Operation ID</param>
    /// <param name="str_curProjectID">Project ID</param>
    /// <param name="str_curSalesChannelID">Sales Channel ID</param>
    /// <param name="str_nexData">Amount</param>
    /// <param name="str_subValue">Value</param>
    /// <param name="str_percentage">Percentage</param>
    /// <param name="str_deliveryInFy">Delivery In Fy</param>
    /// <param name="str_NoInFy">No In Fy</param>
    /// <returns>Execute Count</returns>
    private int insAmount(string str_RSMID, string str_salesorgID, string str_countryID,
        string str_curcustomerID, string bookingY, string deliverY, string str_segmentID,
        string proID, string operationID, string str_curProjectID, string str_curSalesChannelID,
        string str_nexData, string str_subValue, string str_percentage, string str_deliveryInFy,
        string str_NoInFy, string recordID)
    {
        StringBuilder sql = new StringBuilder();
        sql.AppendLine(" INSERT INTO ");
        sql.AppendLine("   Bookings(RecordID, ");
        sql.AppendLine("   RSMID, ");
        sql.AppendLine("   SalesOrgID, ");
        sql.AppendLine("   CountryID, ");
        sql.AppendLine("   CustomerID, ");
        sql.AppendLine("   BookingY, ");
        sql.AppendLine("   DeliverY, ");
        sql.AppendLine("   SegmentID, ");
        sql.AppendLine("   ProductID, ");
        sql.AppendLine("   OperationID, ");
        sql.AppendLine("   ProjectID, ");
        sql.AppendLine("   SalesChannelID, ");
        sql.AppendLine("   Amount, ");
        sql.AppendLine("   Value, ");
        sql.AppendLine("   Percentage, ");
        sql.AppendLine("   Comments, ");
        sql.AppendLine("   [Delivery in FY], ");
        sql.AppendLine("   [NO in FY], ");
        sql.AppendLine("   TimeFlag) ");
        sql.AppendLine(" VALUES( "+recordID+", ");
        sql.AppendLine("   " + str_RSMID + ",");
        sql.AppendLine("   " + str_salesorgID + ",");
        sql.AppendLine("   " + str_countryID + ",");
        sql.AppendLine("   " + str_curcustomerID + ",");
        sql.AppendLine("   '" + bookingY + "',");
        sql.AppendLine("   '" + deliverY + "',");
        sql.AppendLine("   " + str_segmentID + ",");
        sql.AppendLine("   " + proID + ",");
        sql.AppendLine("   " + operationID + ",");
        sql.AppendLine("   " + str_curProjectID + ",");
        sql.AppendLine("   " + str_curSalesChannelID + ",");
        sql.AppendLine("   " + str_nexData + ",");
        if (string.IsNullOrEmpty(str_subValue))
        {
            sql.AppendLine("   NULL,");
        }
        else
        {
            sql.AppendLine("   " + str_subValue + ",");
        }
        if (string.IsNullOrEmpty(str_percentage))
        {
            sql.AppendLine("   NULL,");
        }
        else
        {
            sql.AppendLine("   " + str_percentage + ",");
        }
        sql.AppendLine("   NULL,");
        sql.AppendLine("   '" + str_deliveryInFy + "',");
        sql.AppendLine("   '" + str_NoInFy + "',");
        sql.AppendLine("   '" + year + "-" + month + "-01" + "')");
        return helper.ExecuteNonQuery(CommandType.Text, sql.ToString(), null);
    }
    // By DingJunjie 20110601 ItemW18 Add End

    // By DingJunjie 20110601 ItemW25 Add Start
    /// <summary>
    /// Is Country Can Used
    /// </summary>
    /// <param name="countryID">Country ID</param>
    /// <returns>Result</returns>
    private bool isCountryCanUsed(string countryID)
    {
        StringBuilder sql = new StringBuilder();
        if (!string.Equals(getRoleID(getRole()), "0"))
        {
            sql.AppendLine(" SELECT ");
            sql.AppendLine("   CountryID ");
            sql.AppendLine(" FROM ");
            sql.AppendLine("   User_Country ");
            sql.AppendLine(" WHERE ");
            sql.AppendLine("   CountryID IN (SELECT CountryID FROM User_Country WHERE UserID=" + getUserID() + " AND Deleted=0) ");
            sql.AppendLine("   AND UserID=" + getRSMID());
            sql.AppendLine("   AND Deleted=0 ");
            DataSet ds = helper.GetDataSet(sql.ToString());
            if (ds == null || ds.Tables.Count == 0)
            {
                return false;
            }
            else
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    if (string.Equals(ds.Tables[0].Rows[i]["CountryID"].ToString(), countryID))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        else
        {
            return true;
        }
    }
    // By DingJunjie 20110601 ItemW25 Add End

    // By DingJunjie 20110530 ItemW18 Add Start
    /// <summary>
    /// Data Type DropDownList SelectedIndexChanged Method
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void ddlDataType_SelectedIndexChanged(object sender, EventArgs e)
    {
        this.imgbtn_addYTD.Visible = true;
        this.imgbtn_addYTD.Enabled = true;
        this.panel_addYTD.Visible = false;
        if (string.Equals(this.ddlDataType.SelectedValue, "P1"))
        {
            this.hidBookingY.Value = year.Substring(2, 2);
            this.hidDeliverY.Value = "YTD";
        }
        else if (string.Equals(this.ddlDataType.SelectedValue, "P2"))
        {
            this.hidBookingY.Value = year.Substring(2, 2);
            this.hidDeliverY.Value = year.Substring(2, 2);
        }
        else if (string.Equals(this.ddlDataType.SelectedValue, "P3"))
        {
            this.hidBookingY.Value = year.Substring(2, 2);
            this.hidDeliverY.Value = nextyear.Substring(2, 2);
        }
        else if (string.Equals(this.ddlDataType.SelectedValue, "P4"))
        {
            this.hidBookingY.Value = nextyear.Substring(2, 2);
            this.hidDeliverY.Value = nextyear.Substring(2, 2);
            if (int.Parse(month) == int.Parse(Resource.MEETING_MONTH_SECOND))
            {
                this.imgbtn_addYTD.Visible = false;
                this.panel_addYTD.Visible = false;
            }
            else
            {
                this.imgbtn_addYTD.Visible = true;
            }
        }
        else if (string.Equals(this.ddlDataType.SelectedValue, "P5"))
        {
            this.hidBookingY.Value = nextyear.Substring(2, 2);
            this.hidDeliverY.Value = yearAfterNext.Substring(2, 2);
            if (int.Parse(month) == int.Parse(Resource.MEETING_MONTH_SECOND))
            {
                this.imgbtn_addYTD.Visible = false;
                this.panel_addYTD.Visible = false;
            }
            else
            {
                this.imgbtn_addYTD.Visible = true;
            }
        }
        this.gv_bookingbydatebyproduct.EditIndex = -1;
        bindDataSource();
    }

    /// <summary>
    /// Init Data Type DropDownList
    /// </summary>
    private void initDataTypeList()
    {
        this.ddlDataType.Items.Add(new ListItem(year.Substring(2, 2) + "YTD " + fiscalStart + "," + preyear + " to " + meeting.getMonth(month) + meeting.getDay() + "," + year, "P1"));
        this.ddlDataType.Items.Add(new ListItem(year.Substring(2, 2) + " for " + year.Substring(2, 2) + "  " + meeting.getMonth(month) + meeting.getDay() + "," + year + " to " + fiscalEnd + "," + year.Substring(2, 2) + " for " + year.Substring(2, 2) + " delivery", "P2"));
        this.ddlDataType.Items.Add(new ListItem(year.Substring(2, 2) + " for " + nextyear.Substring(2, 2) + "  " + meeting.getMonth(month) + meeting.getDay() + "," + year + " to " + fiscalEnd + "," + year.Substring(2, 2) + " for " + nextyear.Substring(2, 2) + " delivery", "P3"));
        this.ddlDataType.Items.Add(new ListItem(nextyear.Substring(2, 2) + " for " + nextyear.Substring(2, 2) + "  " + fiscalStart + "," + year + " to " + fiscalEnd + "," + nextyear.Substring(2, 2) + " for " + nextyear.Substring(2, 2) + " delivery", "P4"));
        this.ddlDataType.Items.Add(new ListItem(nextyear.Substring(2, 2) + " for " + yearAfterNext.Substring(2, 2) + "  " + fiscalStart + "," + year + " to " + fiscalEnd + "," + nextyear.Substring(2, 2) + " for " + yearAfterNext.Substring(2, 2) + " delivery", "P5"));
    }

    /// <summary>
    /// Data Bind
    /// </summary>
    /// <param name="bookingY"></param>
    /// <param name="deliverY"></param>
    private void bindDataSource()
    {
        gv_bookingbydatebyproduct.Columns.Clear();
        bindDataByDateByProduct(this.gv_bookingbydatebyproduct, this.hidBookingY.Value, this.hidDeliverY.Value);
        bindEditGV(gv_bookingbydatebyproduct, 4);
        bindEditGV(gv_bookingbydatebyproduct, 5);
        bindEditGV(gv_bookingbydatebyproduct, 6);
        bindEditGV(gv_bookingbydatebyproduct, 7);
        bindEditGVLink(gv_bookingbydatebyproduct, this.hidBookingY.Value, this.hidDeliverY.Value);

        BindOperationIds();
    }
    // By DingJunjie 20110530 ItemW18 Add End

    // By DingJunjie 20110607 ItemW18 Add Start
    private bool check_customer(HiddenField hidOldCusName, TextBox txtNewCusName, Label lbl, string message)
    {
        if (string.IsNullOrEmpty(hidOldCusName.Value) && string.IsNullOrEmpty(txtNewCusName.Text.Trim()))
        {
            lbl.Text = info.errorNullInfo(message);
            lbl.ForeColor = Color.Red;
            return false;
        }
        return true;
    }

    private bool check_customer_double_input(HiddenField hidOldCusName, TextBox txtNewCusName, Label lbl, string message)
    {
        if (!string.IsNullOrEmpty(hidOldCusName.Value) && !string.IsNullOrEmpty(txtNewCusName.Text.Trim()))
        {
            lbl.Text = "Please select [Customer Name] only or input [Customer Name] only.";
            lbl.ForeColor = Color.Red;
            return false;
        }
        return true;
    }
    // By DingJunjie 20110607 ItemW18 Add End

    /// <summary>
    /// Is Country-Subregion Relation Exist Or Not
    /// </summary>
    /// <param name="sID">Country ID</param>
    private bool isCSRelationExist(string sID)
    {
        StringBuilder sql = new StringBuilder();
        sql.AppendLine(" SELECT ");
        sql.AppendLine("   COUNT(*) COUNT ");
        sql.AppendLine(" FROM ");
        sql.AppendLine("   SubRegion ");
        sql.AppendLine("   INNER JOIN Country_SubRegion ON SubRegion.ID=Country_SubRegion.SubRegionID ");
        sql.AppendLine("   INNER JOIN Country ON Country_SubRegion.CountryID=Country.ID ");
        sql.AppendLine(" WHERE ");
        sql.AppendLine("   SubRegion.ID=" + sID);
        sql.AppendLine("   AND Country_SubRegion.Deleted=0 ");
        sql.AppendLine("   AND Country.Deleted=0 ");
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

    /// <summary>
    /// Get Subregion
    /// </summary>
    /// <returns></returns>
    private string getCountryGVConditionSQL()
    {
        string query_country = " SELECT [SubRegion].ID AS SubRegion"
                    + " FROM [User_Country] "
                    + " INNER JOIN [SubRegion] "
                    + " ON [User_Country].CountryID = [SubRegion].ID"
                    + " WHERE [User_Country].UserID  = '" + getRSMID() + "'"
                    + " AND [SubRegion].Deleted = 0 AND [User_Country].Deleted = 0";
        return query_country;
    }

    /// <summary>
    /// bind Amount Link
    /// </summary>
    /// <param name="gv">GridView</param>
    public void bindEditGVLink(GridView gv, string bookingY, string deliverY)
    {
        if (gv.EditIndex != -1)
        {
            string subRegionID = gv.Rows[gv.EditIndex].Cells[0].Text.Replace("&nbsp;", "");
            string customerID = gv.Rows[gv.EditIndex].Cells[1].Text.Replace("&nbsp;", "-1");
            string projectID = gv.Rows[gv.EditIndex].Cells[2].Text.Replace("&nbsp;", "-1");
            string salesChannelID = gv.Rows[gv.EditIndex].Cells[3].Text.Replace("&nbsp;", "-1");
            DropDownList ddlSalesChannel = (DropDownList)(gv.Rows[gv.EditIndex].Cells[7].Controls[0]);
            string salesChannelName = ddlSalesChannel.SelectedItem == null ? string.Empty : ddlSalesChannel.SelectedItem.Text.ToLower();
            StringBuilder param = null;
            StringBuilder backupParam = new StringBuilder();
            StringBuilder hiddenID = new StringBuilder();
            TextBox text = null;
            HiddenField subAmountHidden = null;
            HiddenField percentageHidden = null;
            for (int i = 11; i < gv.Columns.Count - 3; i += 7)
            {
                text = (TextBox)gv.Rows[gv.EditIndex].Cells[i + 1].Controls[0];
                subAmountHidden = new HiddenField();
                subAmountHidden.ID = "hidSubAmount_" + gv.EditIndex + "_" + (i + 1);
                subAmountHidden.Value = gv.Rows[gv.EditIndex].Cells[i + 4].Text.Replace("&nbsp;", string.Empty);
                percentageHidden = new HiddenField();
                percentageHidden.ID = "hidPercentage_" + gv.EditIndex + "_" + (i + 1);
                percentageHidden.Value = gv.Rows[gv.EditIndex].Cells[i + 5].Text.Replace("&nbsp;", string.Empty);
                gv.Rows[gv.EditIndex].Cells[i + 1].Controls.Add(subAmountHidden);
                gv.Rows[gv.EditIndex].Cells[i + 1].Controls.Add(percentageHidden);
                param = new StringBuilder();
                param.Append("'" + text.ClientID + "',");
                param.Append("'" + subAmountHidden.ClientID + "',");
                param.Append("'" + percentageHidden.ClientID + "',");
                param.Append("'").Append(getRSMID()).Append("',");
                param.Append("'").Append(getSalesOrgID()).Append("',");
                param.Append("'").Append(subRegionID).Append("',");
                param.Append("'").Append(customerID).Append("',");
                param.Append("'").Append(bookingY).Append("',");
                param.Append("'").Append(deliverY).Append("',");
                param.Append("'").Append(getSegmentID()).Append("',");
                param.Append("'").Append(gv.Rows[gv.EditIndex].Cells[i + 2].Text.Replace("&nbsp;", "")).Append("',");
                param.Append("'").Append(projectID).Append("',");
                param.Append("'").Append(salesChannelID).Append("',");
                param.Append("'").Append(year).Append("',");
                param.Append("'").Append(month).Append("',");
                param.Append("'").Append(gv.Rows[gv.EditIndex].Cells[i].Text).Append("'");
                if (string.Equals(salesChannelName, "rc own business"))
                {
                    text.Attributes.Add("readonly", "readonly");
                    text.Attributes.Add("onclick", "showRCOBPage(" + param.ToString() + ");");
                }
                backupParam.Append(text.ClientID).Append("@").Append(param.ToString().Replace("'", "")).Append("^");
                hiddenID.Append(subAmountHidden.ClientID).Append(",").Append(percentageHidden.ClientID).Append("^");
            }
            this.hidBackupParam.Value = backupParam.ToString().Trim('^');
            this.hidHiddenID.Value = hiddenID.ToString().Trim('^');
            ImageButton iBut = (ImageButton)gv.Rows[gv.EditIndex].Cells[gv.Columns.Count - 3].Controls[0];
            iBut.Attributes.Add("onclick", "setRCOBValue();");
        }
    }


    /// <summary>
    /// Get Project Info
    /// </summary>
    /// <returns>Project Info</returns>
    private DataSet getEditProject()
    {
        string roleID = getRoleID(getRole());
        if (string.Equals(roleID, "0"))
        {
            return getProjectInfo();
        }
        else if (string.Equals(roleID, "3"))
        {
            return getGSMProjectInfo();
        }
        else if (string.Equals(roleID, "4"))
        {
            return getRSMProjectInfo();
        }
        else
        {
            return null;
        }
    }

    private DataSet getEditCustomer()
    {
        string roleID = getRoleID(getRole());
        if (string.Equals(roleID, "0"))
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT Customer.[ID]");
            sb.Append(" ,[NameID] ");
            sb.Append(" ,[TypeID] ")
                //.Append(" ,[SalesChannelID] ")
                .Append(" ,[CountryID] ")
                .Append(" ,CustomerName.Name as 'Customer Name',CustomerType.Name as 'Customer Type',SubRegion.Name as 'SubRegion'  ")
                .Append(" ,[City] ")
                .Append(" ,[Address] ")
                .Append(" ,[Department] ")
                .Append(" FROM [Customer] inner join [CustomerName] on [Customer].NameID=[CustomerName].ID ")
                .Append(" inner join [SubRegion] on SubRegion.ID=Customer.CountryID ")
                .Append(" inner join CustomerType on CustomerType.ID=Customer.TypeID ")
                .Append(" where Customer.Deleted=0 and CustomerName.Deleted=0 and SubRegion.Deleted=0  and CustomerType.Deleted=0 ")
                .Append(" order by CustomerName.Name ");
            return helper.GetDataSet(sb.ToString());
        }
        else
            return null;
    }

    /// <summary>
    /// Get RSM Project Info
    /// </summary>
    /// <returns>RSM Project Info</returns>
    private DataSet getRSMProjectInfo()
    {
        StringBuilder sql = new StringBuilder();
        sql.AppendLine(" SELECT ");
        sql.AppendLine("   Project.ID, ");
        sql.AppendLine("   Project.PODID, ");
        sql.AppendLine("   Project.Name AS 'Project Name', ");
        sql.AppendLine("   Segment.Abbr AS 'Segment', ");
        sql.AppendLine("   CustomerName.Name AS 'Customer Name', ");
        sql.AppendLine("   Country.ISO_Code AS 'Project Country(POS)', ");
        sql.AppendLine("   Project.Value AS 'Project Value', ");
        sql.AppendLine("   Project.Probability AS '% in budget', ");
        sql.AppendLine("   Currency.Name AS 'Currency', ");
        sql.AppendLine("   Project.Comments, ");
        sql.AppendLine("   ISNULL(Project.Value*Project.Probability/100, 0) AS 'absolute in budget', ");
        sql.AppendLine("   CountryPOD.ISO_Code AS 'Project Country(POD)' ");
        sql.AppendLine(" FROM ");
        sql.AppendLine("   Project ");
        sql.AppendLine("   INNER JOIN Country ON Project.POSID=Country.ID ");
        sql.AppendLine("   INNER JOIN Country AS CountryPOD ON Project.PoDID=CountryPOD.ID ");
        //sql.AppendLine("   INNER JOIN CustomerName ON Project.CustomerNameID=CustomerName.ID ");
        sql.AppendLine("   INNER JOIN Customer ON Project.CustomerNameID=Customer.ID ");
        sql.AppendLine("   INNER JOIN CustomerName ON Customer.NameID=CustomerName.ID ");
        sql.AppendLine("   INNER JOIN Currency ON Project.CurrencyID=Currency.ID ");
        sql.AppendLine("   INNER JOIN Segment ON Project.ProSegmentID=Segment.ID ");
        sql.AppendLine(" WHERE ");
        sql.AppendLine("   Project.Deleted=0 ");
        sql.AppendLine("   AND Country.Deleted=0 ");
        sql.AppendLine("   AND CustomerName.Deleted=0 ");
        sql.AppendLine("   AND Segment.Deleted=0 ");
        sql.AppendLine("   AND CreateUser=" + getUserID());
        sql.AppendLine(" ORDER BY ");
        sql.AppendLine("   Project.Name, ");
        sql.AppendLine("   Country.ISO_Code, ");
        sql.AppendLine("   CustomerName.Name ASC ");
        DataSet ds_project = helper.GetDataSet(sql.ToString());
        return ds_project;
    }

    /// <summary>
    /// Get GSM Project Info
    /// </summary>
    /// <returns>GSM Project Info</returns>
    private DataSet getGSMProjectInfo()
    {
        StringBuilder sql = new StringBuilder();
        sql.AppendLine(" SELECT ");
        sql.AppendLine("   Project.ID, ");
        sql.AppendLine("   Project.PODID, ");
        sql.AppendLine("   Project.Name AS 'Project Name', ");
        sql.AppendLine("   Segment.Abbr AS 'Segment', ");
        sql.AppendLine("   CustomerName.Name AS 'Customer Name', ");
        sql.AppendLine("   Country.ISO_Code AS 'Project Country(POS)', ");
        sql.AppendLine("   Project.Value AS 'Project Value', ");
        sql.AppendLine("   Project.Probability AS '% in budget', ");
        sql.AppendLine("   Currency.Name AS 'Currency', ");
        sql.AppendLine("   Project.Comments, ");
        sql.AppendLine("   ISNULL(Project.Value*Project.Probability/100, 0) AS 'absolute in budget', ");
        sql.AppendLine("   CountryPOD.ISO_Code AS 'Project Country(POD)' ");
        sql.AppendLine(" FROM ");
        sql.AppendLine("   Project ");
        sql.AppendLine("   INNER JOIN Country ON Project.POSID=Country.ID ");
        sql.AppendLine("   INNER JOIN Country AS CountryPOD ON Project.PoDID=CountryPOD.ID ");
        //sql.AppendLine("   INNER JOIN CustomerName ON Project.CustomerNameID=CustomerName.ID ");
        sql.AppendLine("   INNER JOIN Customer ON Project.CustomerNameID=Customer.ID ");
        sql.AppendLine("   INNER JOIN CustomerName ON Customer.NameID=CustomerName.ID ");
        sql.AppendLine("   INNER JOIN Currency ON Project.CurrencyID=Currency.ID ");
        sql.AppendLine("   INNER JOIN Segment ON Project.ProSegmentID=Segment.ID ");
        sql.AppendLine("   INNER JOIN [User] ON Project.CreateUser=[User].UserID ");
        sql.AppendLine(" WHERE ");
        sql.AppendLine("   Project.Deleted=0 ");
        sql.AppendLine("   AND Country.Deleted=0 ");
        sql.AppendLine("   AND CustomerName.Deleted=0 ");
        sql.AppendLine("   AND Segment.Deleted=0 ");
        sql.AppendLine("   AND [User].Deleted=0 ");
        sql.AppendLine("   AND (Project.CreateUser=" + getUserID() + " OR ([User].RoleID=4 AND Project.CreateUser=" + getRSMID());
        sql.AppendLine("        AND Project.CreateUser IN (SELECT UserID FROM User_Country WHERE CountryID IN (SELECT CountryID FROM User_Country WHERE UserID=" + getUserID() + " AND Deleted=0) AND Deleted=0) ");
        sql.AppendLine("        AND Project.CreateUser IN (SELECT UserID FROM User_Segment WHERE SegmentID IN (SELECT SegmentID FROM User_Segment WHERE UserID=" + getUserID() + " AND Deleted=0) AND Deleted=0))) ");
        sql.AppendLine(" ORDER BY ");
        sql.AppendLine("   Project.Name, ");
        sql.AppendLine("   Country.ISO_Code, ");
        sql.AppendLine("   CustomerName.Name ");
        DataSet ds_project = helper.GetDataSet(sql.ToString());
        return ds_project;
    }

    /// <summary>
    /// Input check
    /// </summary>
    /// <param name="subRegion">SubRegion</param>
    /// <param name="customer">Customer</param>
    /// <param name="salesChannel">Sales Channel</param>
    /// <param name="lable">Error Lable</param>
    /// <returns>Result</returns>
    private bool inputCheck(string subRegion, string customer, string salesChannel, Label lable)
    {
        lable.ForeColor = Color.Red;
        if (string.IsNullOrEmpty(subRegion))
        {
            lable.Text = info.errorNullInfo("SubRegion");
            return false;
        }
        else if (string.IsNullOrEmpty(customer.Replace("-1", "")))
        {
            lable.Text = info.errorNullInfo("Customer");
            return false;
        }
        else if (string.IsNullOrEmpty(salesChannel.Replace("-1", "")))
        {
            lable.Text = info.errorNullInfo("Sales Channel");
            return false;
        }
        return true;
    }

    /// <summary>
    /// Delete Booking Sales Data
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">GridView Delete Event Arguments</param>
    protected void gv_bookingbydatebyproduct_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        imgbtn_addYTD.Visible = true;
        imgbtn_addYTD.Enabled = true;
        panel_addYTD.Visible = false;
        pnl_edit.Visible = false;
        int i = Convert.ToInt32(gv_bookingbydatebyproduct.Rows[e.RowIndex].Cells[9].Text);
        delete_booking(gv_bookingbydatebyproduct, e.RowIndex, this.hidBookingY.Value, this.hidDeliverY.Value, i);
        bindDataSource();
    }

    /// <summary>
    /// Delete Booking Sales Data
    /// </summary>
    /// <param name="gv">GridView</param>
    /// <param name="rowIndex">Row Index</param>
    /// <param name="bookingY">bookingY</param>
    /// <param name="deliverY">deliverY</param>
    private void delete_booking(GridView gv, int rIndex, string bookingY, string deliverY, int recordId)
    {
        string strRSMID = getRSMID().Trim();
        string strSalesOrgID = getSalesOrgID().Trim();
        string strSubregionID = gv.Rows[rIndex].Cells[0].Text.Replace("&nbsp;", string.Empty);
        string strCustomerID = gv.Rows[rIndex].Cells[1].Text.Replace("&nbsp;", string.Empty);
        string strSegmentID = getSegmentID().Trim();
        string strProjectID = string.Equals(gv.Rows[rIndex].Cells[2].Text, "&nbsp;") ? "-1" : gv.Rows[rIndex].Cells[2].Text;
        string strSalesChannelID = string.Equals(gv.Rows[rIndex].Cells[3].Text, "&nbsp;") ? "-1" : gv.Rows[rIndex].Cells[3].Text;
        StringBuilder sql = new StringBuilder();
        sql.AppendLine(" DELETE FROM ");
        sql.AppendLine("   Bookings ");
        sql.AppendLine(" WHERE ");
        sql.AppendLine("   RSMID=" + strRSMID);
        sql.AppendLine("   AND SalesOrgID=" + strSalesOrgID);
        sql.AppendLine("   AND CountryID=" + strSubregionID);
        sql.AppendLine("   AND CustomerID=" + strCustomerID);
        sql.AppendLine("   AND BookingY='" + bookingY + "'");
        sql.AppendLine("   AND DeliverY='" + deliverY + "'");
        sql.AppendLine("   AND SegmentID=" + strSegmentID);
        sql.AppendLine("   AND ProjectID=" + strProjectID);
        sql.AppendLine("   AND SalesChannelID=" + strSalesChannelID);
        sql.AppendLine("   AND YEAR(TimeFlag)=" + year);
        sql.AppendLine("   AND MONTH(TimeFlag)=" + month);
        sql.AppendLine(" and RecordID=" + recordId.ToString(CultureInfo.InvariantCulture));
        helper.ExecuteNonQuery(CommandType.Text, sql.ToString(), null);
        ScriptManager.RegisterStartupScript(this.upYTD1, this.upYTD1.GetType(), "Success", "alert('Deleted successfully!');", true);
    }

    /// <summary>
    /// Update Operation
    /// </summary>
    /// <param name="operationID">operationID</param>
    /// <param name="RSMID">RSM ID</param>
    /// <param name="salesOrgID">Sales Organization ID</param>
    /// <param name="segmentID">Segment ID</param>
    /// <param name="subRegionID">Subregion ID</param>
    /// <param name="customerID">Customer ID</param>
    /// <param name="projectID">Project ID</param>
    /// <param name="salesChannelID">Sales Channel ID</param>
    /// <param name="productID">Product ID</param>
    private void updOperation(string operationID, string RSMID, string salesOrgID, string segmentID,
        string subregionID, string customerID, string projectID, string salesChannelID, string productID, string recordID)
    {
        StringBuilder sql = new StringBuilder();
        sql.AppendLine(" UPDATE ");
        sql.AppendLine("   Bookings ");
        sql.AppendLine(" SET ");
        sql.AppendLine("   OperationID=" + operationID);
        sql.AppendLine(" WHERE ");
        sql.AppendLine("  RSMID=" + RSMID);
        sql.AppendLine("  AND SalesOrgID=" + salesOrgID);
        sql.AppendLine("  AND SegmentID=" + segmentID);
        sql.AppendLine("  AND CountryID=" + subregionID);
        sql.AppendLine("  AND CustomerID=" + customerID);
        sql.AppendLine("  AND ProjectID=" + projectID);
        sql.AppendLine("  AND SalesChannelID=" + salesChannelID);
        sql.AppendLine("  AND ProductID=" + productID);
        sql.AppendLine("  AND Year(TimeFlag)=" + year);
        sql.AppendLine("  AND Month(TimeFlag)=" + month);
        sql.AppendLine(" and RecordID=" + recordID);
        helper.ExecuteNonQuery(CommandType.Text, sql.ToString(), null);
    }

    private bool validateOperation(Dictionary<int, string[]> opDic, string RSMID, string salesOrgID, string segmentID,
        string subregionID, string customerID, string projectID, string salesChannelID, string recordID)
    {
        StringBuilder sb = new StringBuilder();

        // productID and operationId defines here.
        
        foreach (var item in opDic)
        { 
            if (sb.Length < 1)
            {
                sb.Append(string.Format("([ProductID]={0} and [OperationID]={1} )", item.Value[0], item.Value[1]));
            }
            else
            {
                sb.Append(string.Format(" or ([ProductID]={0} and [OperationID]={1} )", item.Value[0], item.Value[1]));
            }
        }
       
        StringBuilder sql = new StringBuilder();
        sql.AppendLine(" select count(*) from ");
        sql.AppendLine("   Bookings ");
        sql.AppendLine(" WHERE ");
        sql.AppendLine("  RSMID=" + RSMID);
        sql.AppendLine("  AND SalesOrgID=" + salesOrgID);
        sql.AppendLine("  AND SegmentID=" + segmentID);
        sql.AppendLine("  AND CountryID=" + subregionID);
        sql.AppendLine("  AND CustomerID=" + customerID);
        sql.AppendLine("  AND ProjectID=" + projectID);
        sql.AppendLine("  AND SalesChannelID=" + salesChannelID);
        sql.AppendLine("  AND Year(TimeFlag)=" + year);
        sql.AppendLine("  AND Month(TimeFlag)=" + month);

        // append product ID and operation Id constrains.
       if (sb.Length > 0)
            sql.AppendLine(" and (" + sb.ToString() + ")");
  
        if (!string.IsNullOrEmpty(recordID))
        {
            sql.AppendLine(" and RecordID <>" + recordID);
        }
        DataSet ds = helper.GetDataSet(sql.ToString());
        if (ds != null && (int)ds.Tables[0].Rows[0].ItemArray[0] ==7)
            return false;
        else
            return true;
    }

    [AjaxMethod]
    public DataTable bindCustomerBySubRegionIDAjax(string subRegionID)
    {
        DataSet ds = getCustomerByCountryID(subRegionID);
        return ds == null ? null : ds.Tables[0];
    }
    #endregion
}
