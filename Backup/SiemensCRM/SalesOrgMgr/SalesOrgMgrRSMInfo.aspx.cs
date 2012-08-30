/*
 * File Name      : SalesOrgMgrRSMInfo.aspx.cs
 * 
 * Description    : Use for deleting some segments which RSM sold,some customers and countries which RSM sold to;and add them
 * 
 * Author         : Wangjun
 * 
 * Modify Date    : 2010-12-09
 * 
 * Problem        : problem
 * 
 * Version        : Release (2.0)
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;

public partial class SalesOrgMgr_SalesOrgMgrRSMInfo : System.Web.UI.Page
{
    SQLHelper helper = new SQLHelper();
    LogUtility log = new LogUtility();
    DisplayInfo info = new DisplayInfo();
    SQLStatement sql = new SQLStatement();

    protected void Page_Load(object sender, EventArgs e)
    {
        Page.MaintainScrollPositionOnPostBack = true;
        if (getRoleID(getRole()) == "0")
            panel_readonly.Visible = true;
        else if (getRoleID(getRole()) == "3")
            panel_readonly.Visible = false;
        else
            Response.Redirect("~/AccessDenied.aspx");
        if (!IsPostBack)
        {
            init();
            clearText();
            ddlist_role.Items.Clear();
            bindDDL(sql.getRole(), ddlist_role);
            if (ddlist_role.Items.Count > 0)
            {
                ddlist_user.Items.Clear();
                bindDDL(sql.getUser(ddlist_role.SelectedItem.Value.Trim()), ddlist_user);
                if (ddlist_user.Items.Count <= 0)
                    btn_search.Enabled = false;
                else
                    btn_search.Enabled = true;
            }
            else
                btn_search.Enabled = false;
        }
    }

    private string getUserID()
    {
        return ddlist_user.SelectedItem.Value.Trim();
    }

    protected string getRole()
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

    protected void init()
    {
        panel_country.Visible = false;
        panel_customer.Visible = false;
        panel_segment.Visible = false;

        lbtn_country.Visible = false;
        lbtn_customer.Visible = false;
        lbtn_segment.Visible = false;
    }

    private void clearText()
    {
        label_addcountry.Text = "";
        label_addcustomer.Text = "";
        label_addsegment.Text = "";

        label_delcountry.Text = "";
        label_delcustomer.Text = "";
        label_delsegment.Text = "";
    }

    protected void nullddlist()
    {
        ddlist_segment.Items.Clear();
        ddlist_customer.Items.Clear();
        ddlist_country.Items.Clear();
    }

    private void bindDDL(DataSet ds, DropDownList ddlist)
    {
        if (ds.Tables[0].Rows.Count > 0)
        {
            DataTable dt = ds.Tables[0];
            int countsegment = dt.Rows.Count;
            int indexsegment = 0;
            while (indexsegment < countsegment)
            {
                ddlist.Items.Add(new ListItem(dt.Rows[indexsegment][0].ToString(), dt.Rows[indexsegment][1].ToString()));
                indexsegment++;
            }
            ddlist.Enabled = true;
        }
        else
        {
            ddlist.Enabled = false;
        }
    }

    private void bindDropDownList(DataSet ds, DropDownList ddlist)
    {
        if (ds.Tables[0].Rows.Count > 0)
        {
            DataTable dt = ds.Tables[0];
            int count = dt.Rows.Count;
            int index = 0;
            while (index < count)
            {
                ddlist.Items.Add(new ListItem(dt.Rows[index][1].ToString(), dt.Rows[index][0].ToString()));
                index++;
            }
            ddlist.Enabled = true;
        }
        else
        {
            ddlist.Enabled = false;
        }
    }

    private DataSet getNOTSegment()
    {
        string query_segment = "SELECT ID, Abbr FROM [Segment] WHERE Deleted = 0 "
                               + " AND Abbr not in ("
                               + " SELECT [Segment].Abbr "
                               + " FROM [User_Segment] INNER JOIN [Segment] "
                               + " ON [User_Segment].SegmentID = [Segment].ID"
                               + " WHERE [User_Segment].UserID  = '" + getUserID() + "'"
                               + " AND [Segment].Deleted = 0 AND [User_Segment].Deleted = 0)"
                               + " GROUP BY Abbr,ID"
                               + " ORDER BY Abbr ASC";
        DataSet ds_segment = helper.GetDataSet(query_segment);
        return ds_segment;
    }

    private DataSet getNOTOperation()
    {
        string query_operation = "SELECT ID, AbbrL FROM [Operation] WHERE Deleted = 0"
                            + " AND AbbrL not in ("
                            + " SELECT [Operation].AbbrL "
                            + " FROM [User_Operation] "
                            + " INNER JOIN [Operation] "
                            + " ON [User_Operation].OperationID = [Operation].ID"
                            + " WHERE [User_Operation].UserID  = '" + getUserID() + "'"
                            + " AND [Operation].Deleted = 0 AND [User_Operation].Deleted = 0)"
                            + " GROUP BY AbbrL,ID"
                            + " ORDER BY AbbrL ASC";
        DataSet ds_operation = helper.GetDataSet(query_operation);
        return ds_operation;
    }

    private DataSet getNOTCountry()
    {
        string query_country = "SELECT [SubRegion].ID, [Country].ISO_Code + '(' + [SubRegion].Name + ')' FROM [Country_SubRegion] "
                            + " INNER JOIN [Country] ON [Country].ID = [Country_SubRegion].CountryID"
                            + " INNER JOIN [SubRegion] ON [SubRegion].ID = [Country_SubRegion].SubRegionID"
                            + " WHERE [SubRegion].Deleted = 0"
                            + " AND [SubRegion].Name not in ("
                            + " SELECT [SubRegion].Name "
                            + " FROM [User_Country] "
                            + " INNER JOIN [SubRegion] "
                            + " ON [User_Country].CountryID = [SubRegion].ID"
                            + " WHERE [User_Country].UserID  = '" + getUserID() + "'"
                            + " AND [SubRegion].Deleted = 0 AND [User_Country].Deleted = 0)"
                            + " GROUP BY [SubRegion].Name,[SubRegion].ID,[Country].ISO_Code"
                            + " ORDER BY [Country].ISO_Code ASC";
        DataSet ds_country = helper.GetDataSet(query_country);
        return ds_country;
    }

    protected DataSet getsegmentInfo()
    {
        string query_segment = " SELECT [Segment].ID,[Segment].Abbr AS 'Segment'"
                            + " FROM [User_Segment] INNER JOIN [Segment] "
                            + " ON [User_Segment].SegmentID = [Segment].ID"
                            + " WHERE [User_Segment].UserID  = '" + getUserID() + "'"
                            + " AND [Segment].Deleted = 0 AND [User_Segment].Deleted = 0 "
                            + " ORDER BY [Segment].Abbr ASC";
        DataSet ds_segment = helper.GetDataSet(query_segment);
        return ds_segment;
    }

    protected DataSet getcustomerInfo()
    {
        string query_customer = "SELECT [Operation].ID, [Operation].AbbrL AS Operation"
                            + " FROM [User_Operation] "
                            + " INNER JOIN [Operation] "
                            + " ON [User_Operation].OperationID = [Operation].ID"
                            + " WHERE [User_Operation].UserID  = '" + getUserID() + "'"
                            + " AND [Operation].Deleted = 0 AND [User_Operation].Deleted = 0"
                            + " ORDER BY AbbrL ASC";
        DataSet ds_customer = helper.GetDataSet(query_customer);
        return ds_customer;
    }

    protected DataSet getcountryInfo()
    {
        string query_country = " SELECT [SubRegion].ID, [SubRegion].Name AS SubRegion"
                            + " FROM [User_Country] "
                            + " INNER JOIN [SubRegion] "
                            + " ON [User_Country].CountryID = [SubRegion].ID"
                            + " WHERE [User_Country].UserID  = '" + getUserID() + "'"
                            + " AND [SubRegion].Deleted = 0 AND [User_Country].Deleted = 0"
                            + " ORDER BY Name ASC";
        DataSet ds_country = helper.GetDataSet(query_country);
        return ds_country;
    }

    protected void bindDataSource()
    {
        gv_segment.Columns.Clear();
        gv_customer.Columns.Clear();
        gv_country.Columns.Clear();
        bindDataSource(gv_segment, "Segment", getsegmentInfo());
        bindDataSource(gv_customer, "Operation", getcustomerInfo());
        bindDataSource(gv_country, "SubRegion", getcountryInfo());
    }

    /// <summary>
    /// Use for displaying out a data table by gridview 
    /// </summary>
    /// <param name="gridview"></param>
    /// <param name="str_caption">caption of gridview</param>
    /// <param name="ds">DataSet</param>

    protected void bindDataSource(GridView gridview, string str_caption, DataSet ds)
    {
        bool flag = true;
        if (ds != null)
        {
            if (ds.Tables[0].Rows.Count == 0)
            {
                flag = false;
                sql.getNullDataSet(ds);
            }
            //By Lhy 20110512 ITEM18  DEL Start
            //gridview.Width = Unit.Pixel(260);
            //By Lhy 20110512 ITEM18  DEL End
            gridview.AutoGenerateColumns = false;
            gridview.AllowPaging = false;
            gridview.Visible = true;

            for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
            {
                BoundField bf = new BoundField();

                bf.DataField = ds.Tables[0].Columns[i].ColumnName.ToString();
                bf.HeaderText = ds.Tables[0].Columns[i].Caption.ToString();
                bf.ItemStyle.Width = 200;
                bf.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
                bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;

                gridview.Columns.Add(bf);
            }

            CommandField cf_Delete = new CommandField();
            cf_Delete.ButtonType = ButtonType.Image;
            cf_Delete.ShowDeleteButton = true;
            cf_Delete.ShowCancelButton = true;
            cf_Delete.CausesValidation = false;
            cf_Delete.DeleteImageUrl = "~/images/del.jpg";
            cf_Delete.DeleteText = "Delete";
            gridview.Columns.Add(cf_Delete);

            gridview.AllowSorting = true;
            //By Lhy 20110512 ITEM18  DEL End
            //gridview.Caption = str_caption;
            //gridview.CaptionAlign = TableCaptionAlign.Top;
            //By Lhy 20110512 ITEM18  DEL End
            gridview.DataSource = ds.Tables[0];
            gridview.DataBind();
            gridview.Columns[0].Visible = false;
            gridview.Columns[gridview.Columns.Count - 1].Visible = flag;
            if (getRoleID(getRole()) != "0")
                gridview.Columns[gridview.Columns.Count - 1].Visible = false;
        }
    }

    /*Following is RSM_Segment Module */
    protected void gv_segment_RowDeleting(object sender, GridViewDeleteEventArgs e)///modify
    {
        label_delsegment.Visible = true;
        label_delsegment.Text = "";
        label_delsegment.ForeColor = System.Drawing.Color.Red;

        string segmentID = gv_segment.Rows[e.RowIndex].Cells[0].Text.Trim();
        string segment = gv_segment.Rows[e.RowIndex].Cells[1].Text.Trim();

        string delete_segment = "UPDATE [User_Segment] SET Deleted = 1 WHERE UserID = " + getUserID() + " AND SegmentID = " + segmentID;
        int count = helper.ExecuteNonQuery(CommandType.Text, delete_segment, null);

        if (count > 0)
        {
            label_delsegment.ForeColor = System.Drawing.Color.Green;
            label_delsegment.Text = info.delLabelInfo(segment, true);
        }
        else
            label_delsegment.Text = info.delLabelInfo(segment, false);

        bindDataSource();
    }

    protected void gv_segment_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        gv_segment.PageIndex = e.NewPageIndex;
        bindDataSource();
    }

    protected void lbtn_segment_Click(object sender, EventArgs e)
    {
        lbtn_segment.Text = "Select Segment:";
        lbtn_segment.Enabled = false;
        panel_segment.Visible = true;

        nullddlist();
        clearText();
        bindDropDownList(getNOTSegment(), ddlist_segment);
    }

    protected void btn_segment_Click(object sender, EventArgs e)
    {
        lbtn_segment.Text = "Add Segment";
        lbtn_segment.Enabled = true;
        panel_segment.Visible = false;
        label_addsegment.ForeColor = System.Drawing.Color.Red;

        string segmentID = ddlist_segment.SelectedItem.Value.Trim();
        string segment = ddlist_segment.SelectedItem.Text.Trim();
        string insert_segment = "INSERT INTO [User_Segment] VALUES(" + getUserID() + "," + segmentID + ",0)";
        int count = helper.ExecuteNonQuery(CommandType.Text, insert_segment, null);

        if (count > 0)
        {
            label_addsegment.ForeColor = System.Drawing.Color.Green;
            label_addsegment.Text = info.addLabelInfo(ddlist_segment.SelectedItem.Text, true);
        }
        else
            label_addsegment.Text = info.addLabelInfo(ddlist_segment.SelectedItem.Text, false);
        bindDataSource();
    }

    protected void gv_segment_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (e.Row.RowState == DataControlRowState.Normal || e.Row.RowState == DataControlRowState.Alternate)
            {
                ((ImageButton)e.Row.Cells[gv_segment.Columns.Count - 1].Controls[0]).Attributes.Add("onclick", info.setRowDataBound(e.Row.Cells[1].Text.Trim()));
            }
        }
    }

    protected void btn_canSegment_Click(object sender, EventArgs e)
    {
        lbtn_segment.Text = "Add Segment";
        lbtn_segment.Enabled = true;
        panel_segment.Visible = false;
    }

    /* Following is RSM_Customer Module */

    protected void gv_customer_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        clearText();
        string operationID = gv_customer.Rows[e.RowIndex].Cells[0].Text.Trim();
        string operation = gv_customer.Rows[e.RowIndex].Cells[1].Text.Trim();
        string delete_customer = "UPDATE [User_Operation] SET Deleted = 1 WHERE UserID = " + getUserID() + " AND OperationID = " + operationID;
        int count = helper.ExecuteNonQuery(CommandType.Text, delete_customer, null);

        if (count > 0)
        {
            label_delcustomer.ForeColor = System.Drawing.Color.Green;
            label_delcustomer.Text = info.delLabelInfo(operation, true);
        }
        else
        {
            label_delcustomer.ForeColor = System.Drawing.Color.Red;
            label_delcustomer.Text = info.delLabelInfo(operation, false);
        }

        bindDataSource();
    }

    protected void gv_customer_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        gv_customer.PageIndex = e.NewPageIndex;
        bindDataSource();
    }

    protected void lbtn_customer_Click(object sender, EventArgs e)
    {
        lbtn_customer.Text = "Select Operation:";
        lbtn_customer.Enabled = false;
        panel_customer.Visible = true;

        nullddlist();
        clearText();
        bindDropDownList(getNOTOperation(), ddlist_customer);
    }

    protected void btn_customer_Click(object sender, EventArgs e)
    {
        lbtn_customer.Text = "Add Operation";
        lbtn_customer.Enabled = true;
        panel_customer.Visible = false;
        label_addcustomer.ForeColor = System.Drawing.Color.Red;


        string insert_customer = "INSERT INTO [User_Operation] VALUES(" + getUserID() + "," + ddlist_customer.SelectedItem.Value.Trim() + ",0)";
        int count = helper.ExecuteNonQuery(CommandType.Text, insert_customer, null);

        if (count > 0)
        {
            label_addcustomer.ForeColor = System.Drawing.Color.Green;
            label_addcustomer.Text = info.addLabelInfo(ddlist_customer.SelectedItem.Text, true);
        }
        else
            label_addcustomer.Text = info.addLabelInfo(ddlist_customer.SelectedItem.Text, false);
        bindDataSource();
    }

    protected void gv_customer_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (e.Row.RowState == DataControlRowState.Normal || e.Row.RowState == DataControlRowState.Alternate)
            {
                ((ImageButton)e.Row.Cells[gv_customer.Columns.Count - 1].Controls[0]).Attributes.Add("onclick", info.setRowDataBound(e.Row.Cells[1].Text.Trim()));
            }
        }
    }

    protected void btn_cancustomer_Click(object sender, EventArgs e)
    {
        lbtn_customer.Text = "Add Operation";
        lbtn_customer.Enabled = true;
        panel_customer.Visible = false;
    }

    /* Following is RSM_Country Module */

    protected void gv_country_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        clearText();

        string countryID = gv_country.Rows[e.RowIndex].Cells[0].Text.Trim();
        string country = gv_country.Rows[e.RowIndex].Cells[1].Text.Trim();
        string delete_country = "UPDATE [User_Country] SET Deleted = 1 WHERE UserID = " + getUserID() + " AND CountryID = '" + countryID + "'";
        int count = helper.ExecuteNonQuery(CommandType.Text, delete_country, null);

        if (count > 0)
        {
            label_delcountry.ForeColor = System.Drawing.Color.Green;
            label_delcountry.Text = info.delLabelInfo(country, true);
        }
        else
        {
            label_delcountry.ForeColor = System.Drawing.Color.Red;
            label_delcountry.Text = info.delLabelInfo(country, false);
        }

        bindDataSource();
    }

    protected void gv_country_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        gv_country.PageIndex = e.NewPageIndex;
        bindDataSource();
    }

    protected void lbtn_country_Click(object sender, EventArgs e)
    {
        lbtn_country.Text = "Select SubRegion";
        lbtn_country.Enabled = false;
        panel_country.Visible = true;

        nullddlist();
        clearText();
        if (gv_country.Rows[0].Cells[1].Text.ToString().Trim() == "All")
        {
            ddlist_country.Enabled = false;
            btn_country.Enabled = false;
        }
        else
        {
            ddlist_country.Enabled = false;
            btn_country.Enabled = true;
            bindDropDownList(getNOTCountry(), ddlist_country);
        }
    }

    protected void btn_country_Click(object sender, EventArgs e)
    {
        lbtn_country.Text = "Add SubRegion";
        lbtn_country.Enabled = true;
        panel_country.Visible = false;

        label_addcountry.ForeColor = System.Drawing.Color.Red;
        if (getcountryInfo().Tables[0].Rows.Count > 0)
        {
            if (ddlist_country.SelectedItem.Text.Trim() == "All")
            {
                label_addcountry.Text = "If you want to be responsible for all countries, Please delete all countries and add country 'All'";
                return;
            }
        }
        string insert_country = "INSERT INTO [User_Country] VALUES(" + getUserID() + ",'" + ddlist_country.SelectedItem.Value.Trim() + "',0)";
        int count = helper.ExecuteNonQuery(CommandType.Text, insert_country, null);

        if (count > 0)
        {
            label_addcountry.ForeColor = System.Drawing.Color.Green;
            label_addcountry.Text = info.addLabelInfo(ddlist_country.SelectedItem.Text.Trim(), true);
        }
        else
            label_addcountry.Text = info.addLabelInfo(ddlist_country.SelectedItem.Text.Trim(), false);

        bindDataSource();
    }

    protected void gv_country_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (e.Row.RowState == DataControlRowState.Normal || e.Row.RowState == DataControlRowState.Alternate)
            {
                ((ImageButton)e.Row.Cells[gv_country.Columns.Count - 1].Controls[0]).Attributes.Add("onclick", info.setRowDataBound(e.Row.Cells[1].Text.Trim()));
            }
        }
    }

    protected void btn_canCountry_Click(object sender, EventArgs e)
    {
        lbtn_country.Text = "Add SubRegion";
        lbtn_country.Enabled = true;
        panel_country.Visible = false;
    }

    protected void ddlist_role_SelectedIndexChanged(object sender, EventArgs e)
    {
        ddlist_user.Items.Clear();
        bindDDL(sql.getUser(ddlist_role.SelectedItem.Value.Trim()), ddlist_user);
        if (ddlist_user.Items.Count <= 0)
            btn_search.Enabled = false;
        else
            btn_search.Enabled = true;
    }

    protected void btn_search_Click(object sender, EventArgs e)
    {
        lbtn_country.Visible = true;
        lbtn_customer.Visible = true;
        lbtn_segment.Visible = true;
        //By Lhy 20110512 ITEM18  ADD Start   
        this.lblSubRegion.Visible = true;
        this.lblSegment.Visible = true;
        this.lblOperation.Visible = true;
        //By Lhy 20110512 ITEM18  ADD End

        bindDataSource();
    }
}
