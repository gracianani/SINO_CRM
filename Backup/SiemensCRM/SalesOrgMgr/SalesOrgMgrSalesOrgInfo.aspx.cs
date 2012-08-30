/*
 * File Name     : SalesOrgMgrInfo.aspx.cs
 * 
 * Description   : Use for operating table SalesOrg and his RSM ,Customer and Country
 * 
 * Author        : Wangjun
 * 
 * Modify Date   : 2010-11-02
 * 
 * Problem       : none
 * 
 * Version       : Release (1.0)
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;

public partial class SalesOrgMgr_SalesOrgMgrSalesOrgInfo : System.Web.UI.Page
{
    SQLHelper helper = new SQLHelper();
    LogUtility log = new LogUtility();
    DisplayInfo info = new DisplayInfo();
    WebUtility webU = new WebUtility();
    SQLStatement sql = new SQLStatement();

    protected void Page_Load(object sender, EventArgs e)
    {
        Page.MaintainScrollPositionOnPostBack = true;
        if (getRoleID(getRole()) != "3")
            Response.Redirect("~/AccessDenied.aspx");

        if (!IsPostBack)
        {
            getManagerID();
            initVisible();
            bindDataSourceSaleOrg();
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

    protected string getManagerID()
    {
        return Session["GeneralSalesOrgMgrID"].ToString().Trim();
    }

    protected string getSalesOrgID(string str_managerID)
    {
        string sql = "SELECT [SalesOrg].ID FROM [SalesOrg] INNER JOIN [SalesOrg_User] "
                   + " ON [SalesOrg].ID = [SalesOrg_User].SalesOrgID"
                   + " WHERE [SalesOrg_User].UserID = " + str_managerID + " AND [SalesOrg].Deleted = 0 AND [SalesOrg_User].Deleted = 0";
        DataSet ds = helper.GetDataSet(sql);
        if (ds.Tables[0].Rows.Count == 1)
            return ds.Tables[0].Rows[0][0].ToString().Trim();
        else
        {
            Response.Redirect("~/AccessDenied.aspx");
            return null;
        }
    }

    protected void initVisible()
    {
        label_salesOeg_edit_del.Visible = false;

        label_RSM_del.Visible = false;

        label_Segment_del.Visible = false;
        label_Segment_add.Visible = false;
        panel_addSegment.Visible = false;
    }

    protected DataSet getSalesOrgInfo(string str_managerID)
    {
        string query_SalesOrg = "SELECT [SalesOrg].ID,[SalesOrg].Name AS 'Sales Organization Name',[SalesOrg].Abbr, [Currency].Name AS 'Currency'"
                            + " FROM [SalesOrg] INNER JOIN [Currency]"
                            + " ON [SalesOrg].CurrencyID = [Currency].ID"
                            + " INNER JOIN [SalesOrg_User]"
                            + " ON [SalesOrg_User].SalesOrgID = [SalesOrg].ID"
                            + " WHERE [SalesOrg].Deleted = 0 AND [SalesOrg_User].UserID=" + str_managerID
                            + " AND [SalesOrg_User].Deleted = 0 AND [Currency].Deleted = 0"
                            + " ORDER BY [SalesOrg].Abbr ASC";

        DataSet ds_SalesOrg = helper.GetDataSet(query_SalesOrg);

        return ds_SalesOrg;
    }

    protected DataSet getRSMInfogv(string str_salesOrgID)
    {
        string query_RSM = "SELECT [User].UserID,[User].Abbr AS Login "
                        + " FROM [SalesOrg_User] INNER JOIN [SalesOrg]"
                        + " ON [SalesOrg_User].SalesOrgID = [SalesOrg].ID"
                        + " INNER JOIN [User] "
                        + " ON [SalesOrg_User].UserID = [User].UserID"
                        + " WHERE [SalesOrg].ID = " + str_salesOrgID + " AND [SalesOrg].Deleted = 0 AND [User].Deleted = 0 AND [SalesOrg_User].Deleted = 0"
                        + " ORDER BY [User].Alias ASC";
        DataSet ds_RSM = helper.GetDataSet(query_RSM);

        return ds_RSM;
    }

    protected DataSet getSegmentInfogv(string str_salesOrgID)
    {
        string query_segment = "SELECT [Segment].ID,[Segment].Abbr AS Segment "
                            + " FROM [SalesOrg_Segment] INNER JOIN [Segment] "
                            + " ON [SalesOrg_Segment].SegmentID  = [Segment].ID "
                            + " INNER JOIN [SalesOrg] "
                            + " ON [SalesOrg].ID = [SalesOrg_Segment].SalesOrgID "
                            + " WHERE [SalesOrg].Deleted = 0 AND [Segment].Deleted = 0 AND [SalesOrg].ID = " + str_salesOrgID + " AND [SalesOrg_Segment].Deleted = 0"
                            + " ORDER BY [Segment].Abbr ASC";
        DataSet ds_segment = helper.GetDataSet(query_segment);

        return ds_segment;
    }

    protected void bindDataSourceSaleOrg()
    {
        string str_managerID = getManagerID();
        DataSet ds_SalesOrg = getSalesOrgInfo(str_managerID);

        gv_salsOrg.Width = Unit.Pixel(750);
        gv_salsOrg.AutoGenerateColumns = false;
        gv_salsOrg.AllowPaging = false;
        gv_salsOrg.Visible = true;

        for (int i = 0; i < ds_SalesOrg.Tables[0].Columns.Count; i++)
        {
            BoundField bf = new BoundField();

            bf.DataField = ds_SalesOrg.Tables[0].Columns[i].ColumnName.ToString();
            bf.HeaderText = ds_SalesOrg.Tables[0].Columns[i].Caption.ToString();
            bf.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
            bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
            if (i <= 3)
                bf.ReadOnly = true;

            gv_salsOrg.Columns.Add(bf);
        }

        gv_salsOrg.AllowSorting = true;
        gv_salsOrg.DataSource = ds_SalesOrg.Tables[0];
        gv_salsOrg.DataBind();
        gv_salsOrg.Columns[0].Visible = false;
    }

    /// <summary>
    /// bindDataSource
    /// </summary>
    /// <param name="gridview"></param>
    /// <param name="str_caption"></param>
    /// <param name="ds"></param>
    /// <param name="sel"></param>
    /// 

    protected void bindDataSource(GridView gridview, string str_caption, DataSet ds, bool sel)
    {
        bool bflag = true;
        if (ds.Tables[0].Rows.Count == 0)
        {
            bflag = false;
            sql.getNullDataSet(ds);
        }
        gridview.Width = Unit.Pixel(370);
        gridview.AutoGenerateColumns = false;
        gridview.AllowPaging = false;
        gridview.Visible = true;

        for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
        {
            BoundField bf = new BoundField();

            bf.DataField = ds.Tables[0].Columns[i].ColumnName.ToString();
            bf.ItemStyle.Width = 200;
            bf.HeaderText = ds.Tables[0].Columns[i].Caption.ToString();
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

        gridview.Caption = str_caption;
        gridview.CaptionAlign = TableCaptionAlign.Top;
        gridview.AllowSorting = true;
        gridview.DataSource = ds.Tables[0];
        gridview.DataBind();
        gridview.Columns[0].Visible = false;
        gridview.Columns[gridview.Columns.Count - 1].Visible = bflag;
    }

    protected void bindDataSource()
    {
        string str_SalesOrgID = getSalesOrgID(getManagerID());
        gv_RSM.Columns.Clear();
        gv_Segment.Columns.Clear();
        gv_salsOrg.Columns.Clear();

        bindDataSource(gv_RSM, "RSM", getRSMInfogv(str_SalesOrgID), true);
        bindDataSource(gv_Segment, "Segment", getSegmentInfogv(str_SalesOrgID), false);
        bindDataSourceSaleOrg();
    }

    /* RSM */

    protected void gv_RSM_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        label_RSM_del.Visible = true;
        label_RSM_del.Text = "";
        label_RSM_del.ForeColor = System.Drawing.Color.Red;

        string str_ID = gv_RSM.Rows[e.RowIndex].Cells[0].Text.ToString().Trim();
        string str_Alias = gv_RSM.Rows[e.RowIndex].Cells[1].Text.ToString().Trim();
        string str_salesorgID = getSalesOrgID(getManagerID());

        string del_RSM = "UPDATE [SalesOrg_User] SET Deleted = 1 WHERE UserID = " + str_ID + " AND SalesOrgID = " + str_salesorgID;
        int count = helper.ExecuteNonQuery(CommandType.Text, del_RSM, null);

        if (count > 0)
        {
            label_RSM_del.ForeColor = System.Drawing.Color.Green;
            label_RSM_del.Text = info.delLabelInfo(str_Alias, true);
        }
        else
            label_RSM_del.Text = info.delLabelInfo(str_Alias, false);

        bindDataSource();
    }

    protected void gv_RSM_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (e.Row.RowState == DataControlRowState.Normal || e.Row.RowState == DataControlRowState.Alternate)
            {
                ((ImageButton)e.Row.Cells[gv_RSM.Columns.Count - 1].Controls[0]).Attributes.Add("onclick", info.setRowDataBound(e.Row.Cells[1].Text.Trim()));
            }
        }
    }

    /* Segment */

    protected void gv_Segment_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        label_Segment_del.Visible = true;
        label_Segment_del.Text = "";
        label_Segment_del.ForeColor = System.Drawing.Color.Red;

        string str_segmentID = gv_Segment.Rows[e.RowIndex].Cells[0].Text.Trim();
        string str_segmentName = gv_Segment.Rows[e.RowIndex].Cells[1].Text.Trim();
        string str_salesorgID = getSalesOrgID(getManagerID());
        string del_segment = "UPDATE [SalesOrg_Segment] SET Deleted = 1 "
                            + " WHERE SegmentID = " + str_segmentID + " AND SalesOrgID = " + str_salesorgID;
        int count = helper.ExecuteNonQuery(CommandType.Text, del_segment, null);

        if (count > 0)
        {
            label_Segment_del.ForeColor = System.Drawing.Color.Green;
            label_Segment_del.Text = info.delLabelInfo(str_segmentName, true);
        }
        else
            label_Segment_del.Text = info.delLabelInfo(str_segmentName, false);

        ddlist_segment.Items.Clear();
        getSegmentInfo(str_salesorgID);
        bindDataSource();
    }

    protected void gv_Segment_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (e.Row.RowState == DataControlRowState.Normal || e.Row.RowState == DataControlRowState.Alternate)
            {
                ((ImageButton)e.Row.Cells[gv_Segment.Columns.Count - 1].Controls[0]).Attributes.Add("onclick", info.setRowDataBound(e.Row.Cells[1].Text.Trim()));
            }
        }
    }

    protected void getSegmentInfo(string str_ID)
    {
        ddlist_segment.Items.Clear();
        string query_Segment = "SELECT ID,Abbr FROM [Segment] WHERE Deleted = 0 "
                           + " AND ID NOT IN"
                           + " (SELECT SegmentID "
                           + " FROM [SalesOrg_Segment] INNER JOIN [SalesOrg] "
                           + " ON [SalesOrg].ID = [SalesOrg_Segment].SalesOrgID "
                           + " WHERE [SalesOrg].Deleted = 0 AND [SalesOrg].ID = "
                           + str_ID + " AND [SalesOrg_Segment].Deleted = 0)"
                           + " GROUP BY ID,Abbr"
                           + " ORDER BY Abbr ASC";
        DataSet ds_segment = helper.GetDataSet(query_Segment);

        if (ds_segment.Tables.Count > 0)
        {
            if (ds_segment.Tables[0].Rows.Count > 0)
            {
                DataTable dt_segment = ds_segment.Tables[0];
                int countsegment = dt_segment.Rows.Count;
                int indexsegment = 0;
                while (indexsegment < countsegment)
                {
                    ListItem li = new ListItem(dt_segment.Rows[indexsegment][1].ToString().Trim(), dt_segment.Rows[indexsegment][0].ToString().Trim());
                    ddlist_segment.Items.Add(li);
                    indexsegment++;
                }
                ddlist_segment.Enabled = true;
                btn_addSegment.Enabled = true;
            }
            else
            {
                ListItem li = new ListItem("", "-1");
                ddlist_segment.Items.Add(li);
                ddlist_segment.Enabled = false;
                btn_addSegment.Enabled = false;
            }
        }
    }

    protected void lbtn_segment_Click(object sender, EventArgs e)
    {
        string str_salesOrgID = getSalesOrgID(getManagerID());
        lbtn_segment.Text = "Select segment:";
        lbtn_segment.Enabled = false;
        label_Segment_del.Text = "";
        label_Segment_add.Text = "";
        panel_addSegment.Visible = true;

        getSegmentInfo(str_salesOrgID);
        bindDataSource();
        if (ddlist_segment.SelectedItem.Value == "")
            btn_addSegment.Enabled = false;
    }

    protected void btn_addSegment_Click(object sender, EventArgs e)
    {
        lbtn_segment.Text = "Add Segment";
        lbtn_segment.Enabled = true;
        panel_addSegment.Visible = false;
        label_Segment_add.Visible = true;
        label_Segment_add.ForeColor = System.Drawing.Color.Red;

        string str_name = ddlist_segment.SelectedItem.Text.Trim();
        string str_ID = ddlist_segment.SelectedItem.Value.Trim();
        string str_salesorgID = getSalesOrgID(getManagerID());

        string insert_salesOrg_segment = "INSERT INTO [SalesOrg_Segment] VALUES(" + str_salesorgID + "," + str_ID + " ,0)";
        int count = helper.ExecuteNonQuery(CommandType.Text, insert_salesOrg_segment, null);

        if (count > 0)
        {
            label_Segment_add.ForeColor = System.Drawing.Color.Green;
            label_Segment_add.Text = info.addLabelInfo(str_name, true);
        }
        else
            label_Segment_add.Text = info.addLabelInfo(str_name, false);

        ddlist_segment.Items.Clear();
        bindDataSource();
    }

    protected void btn_CancelSegment_Click(object sender, EventArgs e)
    {
        lbtn_segment.Text = "Add Segment";
        lbtn_segment.Enabled = true;
        panel_addSegment.Visible = false;

        ddlist_segment.Items.Clear();
    }
}
