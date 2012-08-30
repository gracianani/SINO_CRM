/*
 *  File Name     : RSMSalesData.aspx.cs
 * 
 *  Description   : Get and set sale data
 * 
 * Author         : ryzhang
 * 
 * Modify Date    : 2011-5-5
 * 
 *  Problem       : none
 * 
 *  Version       : Release (2.0)
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;

public partial class RSM_RSMSalesData : System.Web.UI.Page
{
    SQLHelper helper = new SQLHelper();
    SQLStatement sql = new SQLStatement();
    WebUtility web = new WebUtility();
    LogUtility log = new LogUtility();
    GetMeetingDate meeting = new GetMeetingDate();

    /* Set Date */
    protected static string yearBeforePre;
    protected static string preyear;
    protected static string year;
    protected static string nextyear;
    protected static string yearAfterNext;
    protected static string month;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (getRoleID(getRole()) == "0")
        {
            panel_readonly.Visible = true;
        }
        else if (getRoleID(getRole()) == "4")
        {
            panel_readonly.Visible = false;
        }
        else
            Response.Redirect("~/AccessDenied.aspx");

        if (!IsPostBack)
        {
            ddlist_segment.Items.Clear();
            ddlist_saleorg.Items.Clear();

            meeting.setDate();
            yearBeforePre = meeting.getyearBeforePre();
            preyear = meeting.getpreyear();
            year = meeting.getyear();
            nextyear = meeting.getnextyear();
            yearAfterNext = meeting.getyearAfterNext();
            month = meeting.getmonth();

            label_note.Text = "";
            ddlist_marketingmgr.Items.Clear();
            getMarketingMgrByUser();
            getGeneralMarketingID();
            ddlist_saleorg.Items.Clear();
            ddlist_segment.Items.Clear();
            getSegmentInfoByGeneralMarketingID(getGeneralMarketingID());
            getSalesOrgInfoBySegmentID(ddlist_segment.SelectedItem.Value.Trim());
            lbtn_add.Visible = false;
        }
    }

    protected string getRole()
    {
        return Session["Role"].ToString().Trim();
    }

    protected string getGeneralMarketingID()
    {
        return ddlist_marketingmgr.SelectedItem.Value.Trim();
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

    protected bool bindDropDownList(DataSet ds, DropDownList ddlist)
    {
        if (ds.Tables[0].Rows.Count > 0)
        {
            DataTable dt = ds.Tables[0];
            int count = dt.Rows.Count;
            int index = 0;
            while (index < count)
            {
                ListItem li = new ListItem(dt.Rows[index][0].ToString().Trim(), dt.Rows[index][1].ToString().Trim());
                ddlist.Items.Add(li);
                index++;
            }
            ddlist.Enabled = true;
            return true;
        }
        else
        {
            ListItem li = new ListItem("", "-1");
            ddlist.Items.Add(li);
            ddlist.Enabled = false;
            return false;
        }
    }

    protected void getMarketingMgrByUser()
    {
        string query_SalesOrg = "SELECT Alias,UserID FROM [User] "
                                + " WHERE RoleID = 2"
                                + " AND [User].Deleted = '0' "
                                + " GROUP BY Alias,UserID"
                                + " ORDER BY Alias ASC";

        DataSet ds = helper.GetDataSet(query_SalesOrg);

        if (bindDropDownList(ds, ddlist_marketingmgr))
            btn_search.Enabled = true;
        else
            btn_search.Enabled = false;
    }

    protected string getOperationInfoByGeneralMarketingID(string str_GeneralMarketingID)
    {
        string sql = "SELECT [Operation].ID,[Operation].AbbrL FROM [User_Operation] "
                            + " INNER JOIN [Operation] ON [Operation].ID = [User_Operation].OperationID"
                            + " WHERE UserID = '" + str_GeneralMarketingID + "'"
                            + " AND [Operation].Deleted = 0"
                            + " AND [User_Operation].Deleted = 0"
                            + " GROUP BY [Operation].ID,[Operation].AbbrL";

        DataSet ds = helper.GetDataSet(sql);

        if (ds.Tables[0].Rows.Count == 1)
            return ds.Tables[0].Rows[0][0].ToString().Trim();
        else
            return "";
    }

    protected void getSalesOrgInfoBySegmentID(string str_segmentID)
    {
        string query_SalesOrg = "SELECT [SalesOrg].Abbr,[SalesOrg].ID FROM [SalesOrg_Segment] "
                                + " INNER JOIN [Segment] ON [Segment].ID = [SalesOrg_Segment].SegmentID "
                                + " INNER JOIN [SalesOrg] ON [SalesOrg].ID = [SalesOrg_Segment].SalesOrgID"
                                + " WHERE [SalesOrg_Segment].SegmentID = " + str_segmentID
                                //by yyan 20110531 item w22 add start
                                + " AND [SalesOrg_Segment].Deleted=0 AND [SalesOrg].Deleted=0 AND [Segment].Deleted=0 "
                                //by yyan 20110531 item w22 add end
                                + " GROUP BY [SalesOrg].Abbr,[SalesOrg].ID"
                                + " ORDER BY [SalesOrg].Abbr ASC";

        DataSet ds = helper.GetDataSet(query_SalesOrg);

        if (bindDropDownList(ds, ddlist_saleorg))
            btn_search.Enabled = true;
        else
            btn_search.Enabled = false;
    }

    protected void getSegmentInfoByGeneralMarketingID(string str_GeneralMarketingID)
    {
        string query_segment = "SELECT [Segment].Abbr,[Segment].ID FROM [User_Segment] "
                            + " INNER JOIN [Segment] ON [Segment].ID = [User_Segment].SegmentID"
                            + " WHERE UserID = " + str_GeneralMarketingID
                            + " AND [Segment].deleted=0 AND [User_Segment].deleted=0 "
                            + " GROUP BY [Segment].ID,[Segment].Abbr"
                            + " ORDER BY [Segment].Abbr ASC";

        DataSet ds = helper.GetDataSet(query_segment);

        if (bindDropDownList(ds, ddlist_segment))
            btn_search.Enabled = true;
        else
            btn_search.Enabled = false;
    }

    protected void ddlist_segment_SelectedIndexChanged(object sender, EventArgs e)
    {
        ddlist_saleorg.Items.Clear();
        if (ddlist_segment.SelectedItem.Value != "-1")
            getSalesOrgInfoBySegmentID(ddlist_segment.SelectedItem.Value);
    }

    protected DataSet getProductInfoBySegmentID(string str_segmentID)
    {
        string query_product = "SELECT [Product].Abbr,[Product].ID FROM [Segment_Product] "
                             + " INNER JOIN [Product] ON [Product].ID = [Segment_Product].ProductID "
                             + " WHERE SegmentID = " + str_segmentID
                             //by yyan 20110531 item w22 add start
                             + " AND [Segment_Product].deleted=0 AND [Product].deleted=0 "
                             //by yyan 20110531 item w22 add end
                             + " GROUP BY [Product].ID,[Product].Abbr "
                             + " ORDER BY [Product].ID ASC";
        DataSet ds = helper.GetDataSet(query_product);
        return ds;
    }

    protected DataSet getBacklog(DataSet dsPro, string str_operationID, string str_salesorgID, string str_segmentID)
    {
        string sqlstr = "SELECT (BacklogY + 'BL') AS 'Backlog Year '";
        string temp = "";
        for (int count = 0; count < dsPro.Tables[0].Rows.Count; count++)
        {
            temp += " ,SUM(CASE WHEN ProductID = " + dsPro.Tables[0].Rows[count][1].ToString()
                 + " THEN Backlog ELSE 0 END) AS '" + dsPro.Tables[0].Rows[count][0].ToString() + "'";
        }
        temp += " FROM [ActualSalesandBL]"
              + " WHERE MarketingMgrID = " + getGeneralMarketingID() + " AND SegmentID = " + str_segmentID
              + " AND OperationID = " + str_operationID + " AND SalesOrgID = " + str_salesorgID
              + " AND YEAR(TimeFlag) = '" + year + "'" + " AND MONTH(TimeFlag) = '" + month + "'"
              + " GROUP BY BacklogY"
              + " ORDER BY BacklogY ASC";

        sqlstr += temp;
        DataSet ds = helper.GetDataSet(sqlstr);
        DataTable dt = new DataTable();
        if (ds != null && ds.Tables.Count != 0)
        {
            for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
            {
                dt.Columns.Add(ds.Tables[0].Columns[i].ColumnName);
                if (i == 0)
                {
                    dt.Columns.Add("Total");
                }
            }
            DataRow row = null;
            int sum = 0;
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                row = dt.NewRow();
                sum = 0;
                for (int j = 0; j < ds.Tables[0].Columns.Count; j++)
                {
                    if (j == 0)
                    {
                        row[j] = ds.Tables[0].Rows[i][j].ToString();
                        for (int k = 1; k < ds.Tables[0].Columns.Count; k++)
                        {
                            sum += Convert.ToInt32(ds.Tables[0].Rows[i][k]);
                        }
                        row[j + 1] = sum;
                    }
                    else
                    {
                        row[j + 1] = ds.Tables[0].Rows[i][j].ToString();
                    }
                }
                dt.Rows.Add(row);
            }
            ds.Tables.Clear();
            ds.Tables.Add(dt);
        }
        return ds;
    }

    protected void bindDataSource()
    {
        bool flag = true;
        lbtn_add.Visible = false;
        DataSet dsPro = getProductInfoBySegmentID(ddlist_segment.SelectedItem.Value);
        if (dsPro.Tables[0].Rows.Count > 0)
        {
            string str_operationID = getOperationInfoByGeneralMarketingID(getGeneralMarketingID());
            if (str_operationID != "")
            {
                DataSet ds = getBacklog(dsPro, str_operationID, ddlist_saleorg.SelectedItem.Value.Trim(), ddlist_segment.SelectedItem.Value.Trim());
                if (ds.Tables[0].Rows.Count == 0)
                {
                    sql.getNullDataSet(ds);
                    flag = false;
                }

                if (ds.Tables[0].Rows.Count < 2)
                {
                    lbtn_add.Visible = true;
                }
                gv_actualBaclog.Width = Unit.Pixel(800);
                gv_actualBaclog.AutoGenerateColumns = false;
                gv_actualBaclog.AllowPaging = true;
                gv_actualBaclog.Visible = true;

                for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
                {
                    BoundField bf = new BoundField();

                    bf.DataField = ds.Tables[0].Columns[i].ColumnName.ToString();
                    bf.HeaderText = ds.Tables[0].Columns[i].Caption.ToString();
                    bf.ItemStyle.HorizontalAlign = HorizontalAlign.Right;
                    bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Right;
                    bf.ReadOnly = false;

                    if (i == 0)
                    {
                        bf.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
                        bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
                        bf.ReadOnly = true;
                    }

                    gv_actualBaclog.Columns.Add(bf);
                }

                //by yyan itemW42 20110621 del start
                //if (flag)
                //{
                //    CommandField cf_Update = new CommandField();
                //    cf_Update.ButtonType = ButtonType.Image;
                //    cf_Update.ShowEditButton = true;
                //    cf_Update.ShowCancelButton = true;
                //    cf_Update.EditImageUrl = "~/images/edit.jpg";
                //    cf_Update.EditText = "Edit";
                //    cf_Update.CausesValidation = false;
                //    cf_Update.CancelImageUrl = "~/images/cancel.jpg";
                //    cf_Update.CancelText = "Cancel";
                //    cf_Update.UpdateImageUrl = "~/images/ok.jpg";
                //    cf_Update.UpdateText = "Update";
                //    gv_actualBaclog.Columns.Add(cf_Update);
                //}
                //by yyan itemW42 20110621 del end
                gv_actualBaclog.AllowSorting = true;
                gv_actualBaclog.DataSource = ds.Tables[0];
                gv_actualBaclog.DataBind();
            }
            else
            {
                label_note.Visible = true;
                label_note.ForeColor = System.Drawing.Color.Red;
                label_note.Text = "The general marketing manager has not managed any operations.";
            }
        }
    }

    protected void btn_search_Click(object sender, EventArgs e)
    {
        lbtn_add.Visible = true;
        label_note.Text = "";
        gv_actualBaclog.Columns.Clear();
        bindDataSource();
    }

    protected void gv_actualBaclog_RowEditing(object sender, GridViewEditEventArgs e)
    {
        gv_actualBaclog.Columns.Clear();
        label_note.Text = "";
        gv_actualBaclog.EditIndex = e.NewEditIndex;
        bindDataSource();
    }

    protected string getProductIDByAbbr(string abbr, string segmentID)////ProductID By SegmentID and ProductAbbr
    {
        string query_abbr = "SELECT [Product].ID FROM [Segment_Product] "
                        + " INNER JOIN [Segment] ON [Segment].ID = [Segment_Product].SegmentID "
                        + " INNER JOIN [Product] ON [Product].ID = [Segment_Product].ProductID "
                        + " WHERE SegmentID = " + segmentID
                        + " AND Segment_Product.Deleted=0 "
                        + " AND Segment.Deleted=0 "
                        + " AND Product.Deleted=0 "
                        + " AND [Product].Abbr = '" + abbr + "'";
        DataSet ds_abbr = helper.GetDataSet(query_abbr);

        if (ds_abbr.Tables[0].Rows.Count == 1)
            return ds_abbr.Tables[0].Rows[0][0].ToString();
        else
            return null;
    }

    protected void update_BL(GridView gv, int rIndex, Label label_note)
    {
        label_note.Visible = true;
        label_note.ForeColor = System.Drawing.Color.Red;
        
        string str_segmentID = ddlist_segment.SelectedItem.Value.Trim();
        string str_salesorgID = ddlist_saleorg.SelectedItem.Value.Trim();
        string str_marketingmgrID = getGeneralMarketingID();
        string str_operationID = getOperationInfoByGeneralMarketingID(getGeneralMarketingID());
        DataSet ds_product = getProductInfoBySegmentID(str_segmentID);
        DataSet ds = getBacklog(ds_product, str_operationID, str_salesorgID, str_segmentID);
        string str_backlogY = ""; ;

        if (ds.Tables[0].Rows.Count > 0)
            str_backlogY = ds.Tables[0].Rows[rIndex][0].ToString().Substring(0, 2).Trim();
        if (str_backlogY != "")
        {
            for (int j = 1; j < gv.Columns.Count - 1; j++)
            {
                string pro = gv.HeaderRow.Cells[j].Text.ToString().Trim();
                string str_preData = ds.Tables[0].Rows[rIndex][j].ToString().Trim();
                string str_nexData = ((TextBox)(gv.Rows[rIndex].Cells[j].Controls[0])).Text.ToString().Trim();
                if (str_nexData == "")
                    str_nexData = "0";

                float f_preData;
                float f_nexData;

                if (System.Text.RegularExpressions.Regex.IsMatch(str_nexData, "^(0|[1-9][0-9]*)$"))
                {
                    if (str_nexData.Length > 8)
                        f_nexData = float.Parse(str_nexData.Substring(0, 7));
                    f_nexData = float.Parse(str_nexData);
                }
                else
                {
                    label_note.Text += pro + "'value was illegal.";
                    continue;
                }
                f_preData = float.Parse(str_preData);

                string proID = getProductIDByAbbr(pro, ddlist_segment.SelectedItem.Value);
                if (f_preData != f_nexData && f_preData > 0 && f_nexData > 0)
                {
                    //Update
                    string update_booking = "UPDATE [ActualSalesandBL] SET"
                                            + " Backlog = '" + str_nexData + "'"
                                            + " WHERE MarketingMgrID = '" + str_marketingmgrID + "'"
                                            + " AND OperationID = '" + str_operationID + "'"
                                            + " AND SalesOrgID = '" + str_salesorgID + "'"
                                            + " AND SegmentID = '" + str_segmentID + "'"
                                            + " AND ProductID = '" + proID + "'"
                                            + " AND BacklogY = '" + str_backlogY + "'"
                                            + " AND Year(TimeFlag) = '" + year + "'"
                                            + " AND Month(TimeFlag) = '" + month + "'";
                    int count = helper.ExecuteNonQuery(CommandType.Text, update_booking, null);

                    if (count != 1)
                    {
                        label_note.Text += "Updated error.." + count.ToString();
                    }
                }
                else if (f_preData != f_nexData && f_preData == 0 && f_nexData > 0)
                {
                    //Insert
                    string insert_booking = "INSERT INTO [ActualSalesandBL]"
                                        + " VALUES('"
                                        + str_marketingmgrID + "','"
                                        + str_operationID + "','"
                                        + str_segmentID + "','"
                                        + str_salesorgID + "','"
                                        + proID + "','"
                                        + str_nexData + "','"
                                        + str_backlogY + "','"
                                        + year + "-" + month + "-01" + "'"
                                        + ")";
                    int count = helper.ExecuteNonQuery(CommandType.Text, insert_booking, null);

                    if (count != 1)
                    {
                        label_note.Text += "Inserted error.." + count.ToString();
                    }
                }
                else if (f_preData != f_nexData && f_preData > 0 && f_nexData == 0)
                {
                    //Delete
                    string delete_booking = "DELETE FROM [ActualSalesandBL]"
                                            + " WHERE MarketingMgrID = '" + str_marketingmgrID + "'"
                                            + " AND OperationID = '" + str_operationID + "'"
                                            + " AND SalesOrgID = '" + str_salesorgID + "'"
                                            + " AND SegmentID = '" + str_segmentID + "'"
                                            + " AND ProductID = '" + proID + "'"
                                            + " AND BacklogY = '" + str_backlogY + "'"
                                            + " AND Year(TimeFlag) = '" + year + "'"
                                            + " AND Month(TimeFlag) = '" + month + "'";
                    int count = helper.ExecuteNonQuery(CommandType.Text, delete_booking, null);

                    if (count != 1)
                    {
                        label_note.Text += "Deleted error.." + count.ToString();
                    }
                }
                else
                { }
            }

            if (label_note.Text == "")
            {
                label_note.ForeColor = System.Drawing.Color.Green;
                label_note.Text = "Modified successfully..";
            }
            else
            {
                label_note.Text += "please re-enter..";
            }
        }
    }

    protected void gv_actualBaclog_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        label_note.Text = "";
        update_BL(gv_actualBaclog, e.RowIndex, label_note);
        
        helper.ExecuteNonQuery(CommandType.Text, "DELETE FROM [ActualSalesandBL] WHERE Backlog = 0", null);
        gv_actualBaclog.Columns.Clear();
        gv_actualBaclog.EditIndex = -1;
        bindDataSource();
    }

    protected void gv_actualBaclog_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        gv_actualBaclog.Columns.Clear();
        gv_actualBaclog.EditIndex = -1;
        bindDataSource();
    }

    protected void getBacklogY()
    {
        string str_segmentID = ddlist_segment.SelectedItem.Value.Trim();
        string str_salesorgID = ddlist_saleorg.SelectedItem.Value.Trim();
        string str_marketingmgrID = getGeneralMarketingID();
        string str_operationID = getOperationInfoByGeneralMarketingID(getGeneralMarketingID());
        DataSet ds_product = getProductInfoBySegmentID(str_segmentID);
        DataSet ds = getBacklog(ds_product, str_operationID, str_salesorgID, str_segmentID);
        if (ds.Tables[0].Rows.Count == 0)
        {
            ddlist_backlogY.Items.Add(year.Substring(2, 2));
            ddlist_backlogY.Items.Add(nextyear.Substring(2, 2));
            btn_add.Enabled = true;
        }
        else if (ds.Tables[0].Rows.Count == 1 && ds.Tables[0].Rows[0][0].ToString().Substring(0, 2) == year.Substring(2, 2))
        {
            ddlist_backlogY.Items.Add(nextyear.Substring(2, 2));
            btn_add.Enabled = true;
        }
        else if (ds.Tables[0].Rows.Count == 1 && ds.Tables[0].Rows[0][0].ToString().Substring(0, 2) == nextyear.Substring(2, 2))
        {
            ddlist_backlogY.Items.Add(year.Substring(2, 2));
            btn_add.Enabled = true;
        }
        else
        {
            ddlist_backlogY.Items.Clear();
            btn_add.Enabled = false;
        }
    }

    protected void addBacklog(GridView gv, Label label_note)
    {
        helper.ExecuteNonQuery(CommandType.Text, "DELETE FROM [ActualSalesandBL] WHERE Backlog = 0", null);
        label_note.Visible = true;
        
        label_note.ForeColor = System.Drawing.Color.Red;
        string str_segmentID = ddlist_segment.SelectedItem.Value.Trim();
        string str_salesorgID = ddlist_saleorg.SelectedItem.Value.Trim();
        string str_marketingmgrID = getGeneralMarketingID();
        string str_operationID = getOperationInfoByGeneralMarketingID(getGeneralMarketingID());
        string str_backlogY = ddlist_backlogY.Text.Trim();

        for (int j = 1; j < gv.Columns.Count - 1; j++)
        {
            string pro = gv.HeaderRow.Cells[j].Text.ToString().Trim();
            string proID = getProductIDByAbbr(pro, ddlist_segment.SelectedItem.Value);
            //Insert
            string insert_booking = "INSERT INTO [ActualSalesandBL]"
                                + " VALUES('"
                                + str_marketingmgrID + "','"
                                + str_operationID + "','"
                                + str_segmentID + "','"
                                + str_salesorgID + "','"
                                + proID + "','"
                                + "0" + "','"
                                + str_backlogY + "','"
                                + year + "-" + month + "-01" + "'"
                                + ")";
            int count = helper.ExecuteNonQuery(CommandType.Text, insert_booking, null);

            if (count != 1)
            {
                label_note.Text += "Inserted error.." + count.ToString();
            }
        }

        if (label_note.Text == "")
        {
            label_note.ForeColor = System.Drawing.Color.Green;
            label_note.Text = "Added successfully.";
        }
        else
        {
            label_note.Text += "please re-enter..";
            helper.ExecuteNonQuery(CommandType.Text, "DELETE FROM [ActualSalesandBL] WHERE Backlog = 0", null);
        }
    }

    protected void btn_add_Click(object sender, EventArgs e)
    {
        lbtn_add.Enabled = true;
        lbtn_add.Text = "Add Backlog";
        panel_add.Visible = false;
        label_note.Text = "";
        addBacklog(gv_actualBaclog, label_note);

        gv_actualBaclog.Columns.Clear();
        bindDataSource();
    }

    protected void lbtn_add_Click(object sender, EventArgs e)
    {
        lbtn_add.Enabled = false;
        lbtn_add.Text = "Select date";
        panel_add.Visible = true;
        ddlist_backlogY.Items.Clear();
        getBacklogY();
    }

    protected void btn_cancel_Click(object sender, EventArgs e)
    {
        lbtn_add.Enabled = true;
        lbtn_add.Text = "Add Backlog";
        panel_add.Visible = false;

        gv_actualBaclog.Columns.Clear();
        bindDataSource();
    }

    protected void ddlist_marketingmgr_SelectedIndexChanged(object sender, EventArgs e)
    {
        getGeneralMarketingID();
        ddlist_saleorg.Items.Clear();
        ddlist_segment.Items.Clear();
        getSegmentInfoByGeneralMarketingID(getGeneralMarketingID());
        getSalesOrgInfoBySegmentID(ddlist_segment.SelectedItem.Value.Trim());
    }
}
