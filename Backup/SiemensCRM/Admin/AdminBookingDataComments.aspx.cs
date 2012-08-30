/*
 *   File Name   :AdminBooingDataComments.aspx.cs
 *   
 *   Description :
 *   
 *   Author      : WangJun
 * 
 *   Modified    :2010-3-31
 * 
 *   Problem     : 
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

public partial class Admin_AdminBookingDataComments : System.Web.UI.Page
{
    SQLHelper helper = new SQLHelper();
    LogUtility log = new LogUtility();
    GetMeetingDate meeting = new GetMeetingDate();
    SQLStatement sql = new SQLStatement();

    private static string str_salesOrgID;
    private static string str_countryID;
    private static string str_customerID;
    private static string str_segmentID;
    private static string str_projectID;
    private static string str_rsmID;
    private static string str_bookingY;
    private static string str_deliverY;
    //By Sj 20110530 ITEM 20 ADD Start
    private static string str_salesChannelID;
    //By Sj 20110530 ITEM 20 ADD End

    /* Set Date */
    protected static string preyear;
    protected static string year;
    protected static string nextyear;
    protected static string yearAfterNext;
    protected static string month;
    protected const string fiscalStart = "Oct.1";
    protected const string fiscalEnd = "Sept.30";

    protected void Page_Load(object sender, EventArgs e)
    {
        

        if (!IsPostBack)
        {
            log.WriteLog(LogUtility.LogErrorLevel.LOG_ACCESS, "AdminBookingDataComments Access.");

            panel_comments.Visible = true;

            meeting.setDate();
            preyear = meeting.getpreyear();
            year = meeting.getyear();
            nextyear = meeting.getnextyear();
            yearAfterNext = meeting.getyearAfterNext();
            month = meeting.getmonth();

            str_salesOrgID = Request.QueryString["SalesOrgID"].ToString().Trim();
            str_countryID = Request.QueryString["CountryID"].ToString().Trim();
            str_customerID = Request.QueryString["CustomerID"].ToString().Trim();
            str_segmentID = Request.QueryString["SegmentID"].ToString().Trim();
            str_projectID = Request.QueryString["ProjectID"].ToString().Trim();
            str_rsmID = Request.QueryString["RSMID"].ToString().Trim();
            str_bookingY = Request.QueryString["BookingY"].ToString().Trim();
            str_deliverY = Request.QueryString["DeliverY"].ToString().Trim();
            //By Sj 20110530 ITEM 20 ADD Start
            str_salesChannelID = Request.QueryString["SalesChannelID"].ToString().Trim();
            //By Sj 20110530 ITEM 20 ADD End
            gv_data.Columns.Clear();
            gv_comments.Columns.Clear();

            getProductInfo();
            //By SJ 20110524 Item 20 Del Start
            //bindDataSource();
            //By SJ 20110524 Item 20 Del End

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

    protected DataSet getBookingComments(string countryID, string segmentID, string RSMID, string bookingY, string deliverY)
    {
        string sqlstr = "SELECT";
        string temp = "";
        temp += " [Product].Abbr AS Product, (CASE WHEN datalength([Bookings].Comments) = NULL THEN ' ' ELSE [Bookings].Comments END ) AS 'Comments'";
        temp += " FROM [Bookings] INNER JOIN [Product] ON [Bookings].ProductID = [Product].ID"
        + " WHERE RSMID = " + RSMID + " AND SegmentID = " + segmentID
        + " AND BookingY = '" + bookingY + "' AND DeliverY='" + deliverY + "' AND YEAR(TimeFlag) = '" + year + "'"
        + " AND MONTH(TimeFlag) = '" + month + "' AND CountryID = " + countryID
        + " AND CustomerID = " + str_customerID + " AND ProjectID = " + str_projectID + " AND SalesOrgID = " + str_salesOrgID
        + " AND Product.Deleted=0 "
        + " ORDER BY [Bookings].ProductID";

        sqlstr += temp;
        DataSet ds = helper.GetDataSet(sqlstr);

        if (ds.Tables.Count > 0)
            return ds;
        else
            return null;
    }

    protected DataSet getBookingData(DataSet dsPro, string countryID, string segmentID, string RSMID, string bookingY, string deliverY)
    {
        if (dsPro != null)
        {
            string sqlstr = "SELECT [Country].ID AS CoID,[Operation].ID AS OpID";
            string temp = "";
            sqlstr += ", [Country].Name AS Country,[Country].ISO_Code AS Code";
            for (int count = 0; count < dsPro.Tables[0].Rows.Count; count++)
            {
                temp += " ,SUM(CASE WHEN ProductID = " + dsPro.Tables[0].Rows[count][0].ToString()
                     + " THEN Amount ELSE 0 END ) AS '"
                     + dsPro.Tables[0].Rows[count][1].ToString() + "'" + " ,[Operation].Abbr AS Abbr";
            }
            temp += " FROM [Bookings] INNER JOIN [Country] ON [Bookings].CountryID = [Country].ID"
                  + " INNER JOIN [Operation] ON [Bookings].OperationID = [Operation].ID"
                  + " WHERE RSMID = " + RSMID + " AND SegmentID = " + segmentID
                  + " AND BookingY = '" + bookingY + "' AND DeliverY='" + deliverY + "' AND YEAR(TimeFlag) = '" + year + "'"
                  + " AND MONTH(TimeFlag) = '" + month + "' AND CountryID = " + countryID
                  + " AND CustomerID = " + str_customerID + " AND ProjectID = " + str_projectID + " AND SalesOrgID = " + str_salesOrgID
                  + " AND Country.Deleted=0 "
                  + " AND Operation.Deleted=0 "
                  + " GROUP BY Country.Name,Country.ISO_Code,Operation.Abbr,[Country].ID,[Operation].ID"
                  + " ORDER BY Country.Name,[Operation].ID ASC";

            sqlstr += temp;
            DataSet ds = helper.GetDataSet(sqlstr); ;

            if (ds.Tables.Count > 0)
                return ds;
            else
                return null;
        }
        else
            return null;

    }

    protected DataSet getProductBySegment(string segmentID)
    {
        string query_product = "SELECT ID,Abbr FROM [Product] INNER JOIN [Segment_Product] ON [Segment_Product].ProductID = [Product].ID "
                       + " WHERE SegmentID = " + segmentID + " AND [Product].Deleted = 0 AND [Segment_Product].Deleted = 0";
        DataSet ds_product = helper.GetDataSet(query_product);

        if (ds_product.Tables[0].Rows.Count > 0)
            return ds_product;
        else
            return null;
    }

    protected void bind(GridView gv, string bookingY, string deliverY, DataSet ds, bool flag)
    {
        if (ds != null)
        {
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

                if (i % 2 == 1 && i > 4)
                {
                    bf.HeaderText = null;
                    bf.ReadOnly = true;
                }

                if (i % 2 == 0 && i > 3)
                {
                    bf.ItemStyle.HorizontalAlign = HorizontalAlign.Right;
                    bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Right;
                    bf.ReadOnly = false;
                    bf.ItemStyle.Width = 100;
                    bf.ControlStyle.Width = 100;
                }

                gv.Columns.Add(bf);
            }

            if (flag)
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
            }

            if (deliverY == "YTD")
                gv.Caption = bookingY + deliverY + "  " + fiscalStart + "," + preyear + " to " + meeting.getMonth(month) + meeting.getDay() + "," + year;
            else
                gv.Caption = bookingY + " for " + deliverY + "  " + meeting.getMonth(month) + meeting.getDay() + "," + year + " to " + fiscalEnd + "," + bookingY + " for " + deliverY + " delivery";
            gv.CaptionAlign = TableCaptionAlign.Top;
            gv.AllowSorting = true;
            gv.DataSource = ds.Tables[0];

            gv.DataBind();
            gv.Columns[0].Visible = false;
            gv.Columns[1].Visible = false;
        }
        else
        {
            gv.Visible = false;
        }
    }

    protected void bindComments(GridView gv, string bookingY, string deliverY, DataSet ds, bool flag)
    {
        if (ds != null)
        {
            gv.Visible = true;
            gv.Width = Unit.Pixel(800);
            gv.AutoGenerateColumns = false;
            gv.AllowPaging = false;

            for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
            {
                BoundField bf = new BoundField();

                bf.DataField = ds.Tables[0].Columns[i].ColumnName.ToString();
                bf.HeaderText = ds.Tables[0].Columns[i].Caption.ToString();
                bf.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
                bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;

                if (i == 0)
                {
                    bf.ReadOnly = true;
                    bf.ItemStyle.Width = 100;
                    bf.ControlStyle.Width = 100;
                }
                if (i == 1)
                {
                    bf.ItemStyle.Width = 600;
                    bf.ControlStyle.Width = 600;
                }

                gv.Columns.Add(bf);
            }

            if (flag)
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
            }
            gv.AllowSorting = true;
            gv.DataSource = ds.Tables[0];

            gv.DataBind();
        }
        else
        {
            gv.Visible = false;
        }
    }

    protected void bindDataSource()
    {
        DataSet ds_product = getProductBySegment(str_segmentID);
        if (ds_product == null)
        {
            return;
        }
        DataSet dsData = getBookingData(ds_product, str_countryID, str_segmentID, str_rsmID, str_bookingY, str_deliverY);
        DataSet dsComments = getBookingComments(str_countryID, str_segmentID, str_rsmID, str_bookingY, str_deliverY);

        //By SJ 20110523 Item20  Del Start
        //bind(gv_data, str_bookingY, str_deliverY, dsData, false);
        //bindComments(gv_comments, str_bookingY, str_deliverY, dsComments, true);
        //By SJ 20110523 Item20  Del End
    }

    protected void gv_comments_RowEditing(object sender, GridViewEditEventArgs e)
    {
        gv_data.Columns.Clear();
        gv_comments.Columns.Clear();
        gv_comments.EditIndex = e.NewEditIndex;

        //panel_comments.Visible = true;
        //bindDropDownList();

        bindDataSource();
    }

    protected string getProductIDByAbbr(string abbr, string segmentID)////ProductID By SegmentID and ProductAbbr
    {
        string query_abbr = "SELECT [Product].ID FROM [Segment_Product] "
                        + " INNER JOIN [Segment] ON [Segment].ID = [Segment_Product].SegmentID "
                        + " INNER JOIN [Product] ON [Product].ID = [Segment_Product].ProductID "
                        + " WHERE SegmentID = " + segmentID
                        + " AND [Product].Abbr = '" + abbr + "'";
        DataSet ds_abbr = helper.GetDataSet(query_abbr);

        if (ds_abbr.Tables[0].Rows.Count == 1)
            return ds_abbr.Tables[0].Rows[0][0].ToString();
        else
            return null;
    }

    protected void gv_comments_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        gv_data.Columns.Clear();
        string str_proID = getProductIDByAbbr(gv_comments.Rows[e.RowIndex].Cells[0].Text.Trim(), str_segmentID);
        //string str_comments = label_comments.Text.Trim();
        string str_comments = ((TextBox)(gv_comments.Rows[e.RowIndex].Cells[1].Controls[0])).Text.Trim();

        string update_comments = "UPDATE [Bookings] SET Comments = '" + str_comments + "'"
                                + " WHERE RSMID = " + str_rsmID
                                + " AND SegmentID = " + str_segmentID
                                + " AND CustomerID= " + str_customerID
                                + " AND ProjectID = " + str_projectID
                                + " AND SalesOrgID = " + str_salesOrgID
                                + " AND ProductID = " + str_proID
                                + " AND CountryID = " + str_countryID
                                + " AND BookingY = '" + str_bookingY + "'"
                                + " AND DeliverY = '" + str_deliverY + "'"
                                + " AND YEAR(TimeFlag) = '" + year + "'"
                                + " AND MONTH(TimeFlag) = '" + month + "'";

        int count = helper.ExecuteNonQuery(CommandType.Text, update_comments, null);
        if (count == 1)
        {
            label_note.ForeColor = System.Drawing.Color.Green;
            label_note.Text = "Modified successfully.";
        }
        else
        {
            label_note.ForeColor = System.Drawing.Color.Red;
            label_note.Text = "Please input again.";
        }

        gv_comments.Columns.Clear();
        gv_comments.EditIndex = -1;
        label_comments.Text = "";
        //panel_comments.Visible = false;
        bindDataSource();
    }

    protected void gv_comments_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        gv_data.Columns.Clear();
        gv_comments.Columns.Clear();
        gv_comments.EditIndex = -1;
        panel_comments.Visible = false;
        bindDataSource();
    }

    protected void btn_close_Click(object sender, EventArgs e)
    {   //By Sj 20110509 ITEM 20 DEL Start 
        //  Response.Write("<script language=javascript> window.opener.window.document.forms(0).submit();window.close();</script>");
        //By Sj 20110509 ITEM 20 DEL End
        //By Sj 20110509 ITEM 20 ADD Start 
        Response.Write("<script language=javascript> window.close();</script>");
        //By Sj 20110509 ITEM 20 ADD End
    }

    protected void btn_ok_Click(object sender, EventArgs e)
    {
        

        //By Sj 20110506 ITEM 20 ADD Start
        string str_ProductID = ddlist_product.Text.Trim();
        string str_input = content.Value.ToString().Trim();
        string update_comments = "UPDATE [Bookings] SET Comments = '" + str_input + "'"
                               + " WHERE RSMID = " + str_rsmID
                               + " AND SegmentID = " + str_segmentID
                               + " AND CustomerID= " + str_customerID
                               + " AND ProductID = " + str_ProductID
                               + " AND ProjectID = " + str_projectID
                               + " AND SalesOrgID = " + str_salesOrgID
                               + " AND CountryID = " + str_countryID
                               + " AND BookingY = '" + str_bookingY + "'"
                               + " AND DeliverY = '" + str_deliverY + "'"
                               + " AND YEAR(TimeFlag) = '" + year + "'"
                               + " AND MONTH(TimeFlag) = '" + month + "'"
            //By Sj 20110530 ITEM 20 ADD Start
                               + " AND SalesChannelID = '" + str_salesChannelID + "'";
            //By Sj 20110530 ITEM 20 ADD End 

        int count = helper.ExecuteNonQuery(CommandType.Text, update_comments, null);
        if (count == 1)
        {
            label_note.ForeColor = System.Drawing.Color.Green;
            label_note.Text = "Modified successfully.";
        }
        else
        {
            label_note.ForeColor = System.Drawing.Color.Red;
            label_note.Text = "Please input again.";
        }
        gv_comments.EditIndex = -1;
        label_comments.Text = "";
        //By Sj 20110506 ITEM 20 ADD End
        gv_data.Columns.Clear();
        gv_comments.Columns.Clear();
        bindDataSource();
    }


    private void bindDropDownList()
    {
        ddlist_product.Items.Clear();
        DataSet ds_project = sql.getProjectInfo();
        bindDropDownList(ddlist_product, ds_project, true);
    }

    private void bindDropDownList(DropDownList ddl, DataSet ds, bool flag)
    {
        if (ds.Tables[0].Rows.Count > 0)
        {
            DataTable dt = ds.Tables[0];
            int count = dt.Rows.Count;
            int index = 0;
            while (index < count)
            {
                if (flag)
                {
                    ListItem li = new ListItem(dt.Rows[index][1].ToString().Trim(), dt.Rows[index][0].ToString().Trim());
                    ddl.Items.Add(li);
                }
                else
                    ddl.Items.Add(dt.Rows[index][0].ToString().Trim());
                index++;
            }
            ddl.Enabled = true;
        }
        else
        {
            if (flag)
            {
                ListItem li = new ListItem("Not Exist", "-1");
                ddl.Items.Add(li);
            }
            else
                ddl.Items.Add("Not Exist");
            ddl.Enabled = false;
        }
    }

    //By Sj 20110506 ITEM 20 ADD Start 
    public void getProductInfo()
    {
        StringBuilder strSQL = new StringBuilder();
        strSQL.AppendLine(" SELECT ");
        strSQL.AppendLine("   [Bookings].ProductID, ");
        strSQL.AppendLine("   [Product].Abbr ");
        strSQL.AppendLine(" FROM ");
        strSQL.AppendLine("   [Bookings] INNER JOIN [Product] ON [Bookings].ProductID=[Product].ID ");
        strSQL.AppendLine(" WHERE ");
        strSQL.AppendLine("   RSMID=" + str_rsmID);
        strSQL.AppendLine("   AND [Bookings].SalesOrgID=" + str_salesOrgID);
        strSQL.AppendLine("   AND [Bookings].CountryID=" + str_countryID);
        strSQL.AppendLine("   AND [Bookings].CustomerID=" + str_customerID);
        strSQL.AppendLine("   AND [Bookings].SegmentID=" + str_segmentID);
        strSQL.AppendLine("   AND [Bookings].ProjectID=" + str_projectID);
        strSQL.AppendLine("   AND [Bookings].SalesChannelID=" + str_salesChannelID);
        strSQL.AppendLine("   AND [Bookings].BookingY='" + str_bookingY + "' ");
        strSQL.AppendLine("   AND [Bookings].DeliverY='" + str_deliverY + "' ");
        strSQL.AppendLine("   AND YEAR([Bookings].TimeFlag)=" + year);
        strSQL.AppendLine("   AND MONTH([Bookings].TimeFlag)=" + month);
        strSQL.AppendLine("   AND [Product].Deleted=0 ");
        strSQL.AppendLine(" ORDER BY ");
        strSQL.AppendLine("   [Product].Abbr ");
        DataSet ds = helper.GetDataSet(strSQL.ToString());
        if (ds.Tables[0].Rows.Count > 0)
        {
            this.content.Value = getComments(ds.Tables[0].Rows[0][0].ToString());
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                this.ddlist_product.Items.Add(new ListItem(ds.Tables[0].Rows[i][1].ToString(), ds.Tables[0].Rows[i][0].ToString()));
            }
            this.ddlist_product.Enabled = true;
        }
        else
        {
            label_note.ForeColor = System.Drawing.Color.Red;
            label_note.Text = "Please input amount first!";
            ddlist_product.Items.Add(new ListItem("Not Exist", "-1"));
            ddlist_product.Enabled = false;
            label_input.Visible = false;
            content.Visible = false;
            btn_ok.Visible = false;
        }
    }

    public void ddl_SelectedProductChanged(object sender, EventArgs e)
    {
        string str_ProductID = ddlist_product.Text.Trim();
        label_note.Text = "";
        string select_comments = " SELECT Comments FROM [Bookings] "
                                 + " WHERE RSMID = " + str_rsmID
                                 + " AND SegmentID = " + str_segmentID
                                 + " AND CustomerID= " + str_customerID
                                 + " AND SalesOrgID = " + str_salesOrgID
                                 + " AND ProductID = " + str_ProductID
                                 + " AND CountryID = " + str_countryID
                                 + " AND ProjectID=" + str_projectID
                                 + " AND SalesChannelID=" + str_salesChannelID
                                 + " AND BookingY = '" + str_bookingY + "'"
                                 + " AND DeliverY = '" + str_deliverY + "'"
                                 + " AND YEAR(TimeFlag) = '" + year + "'"
                                 + " AND MONTH(TimeFlag) = '" + month + "'";
        DataSet ds = helper.GetDataSet(select_comments);
        if (ds.Tables.Count > 0)
        {
            content.Value = ds.Tables[0].Rows[0][0].ToString();
        }
        else
        {
            content.Value = "";
        }
    }

    public string getComments(string str_ProductID)
    {
        string select_comments = " SELECT Comments FROM [Bookings] "
                                 + " WHERE RSMID = " + str_rsmID
                                 + " AND SegmentID = " + str_segmentID
                                 + " AND CustomerID= " + str_customerID
                                 + " AND SalesOrgID = " + str_salesOrgID
                                 + " AND ProductID = " + str_ProductID
                                 + " AND CountryID = " + str_countryID
                                 + " AND ProjectID=" + str_projectID
                                 + " AND SalesChannelID=" + str_salesChannelID
                                 + " AND BookingY = '" + str_bookingY + "'"
                                 + " AND DeliverY = '" + str_deliverY + "'"
                                 + " AND YEAR(TimeFlag) = '" + year + "'"
                                 + " AND MONTH(TimeFlag) = '" + month + "'";
        DataSet ds = helper.GetDataSet(select_comments);
        if (ds.Tables.Count > 0)
        {
            return ds.Tables[0].Rows[0][0].ToString();
        }
        else
        {
            return "";
        }
    }
    //By Sj 20110506 ITEM 20 ADD Start End
}
