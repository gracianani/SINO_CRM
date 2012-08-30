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

public partial class RSM_RSMSalesOrg : System.Web.UI.Page
{
    SQLHelper helper = new SQLHelper();
    LogUtility log = new LogUtility();
    DisplayInfo info = new DisplayInfo();
    SQLStatement sql = new SQLStatement();

    private static int str_salesOrgID;

    protected void Page_Load(object sender, EventArgs e)
    {
        Page.MaintainScrollPositionOnPostBack = true;
        if (getRoleID(getRole()) == "4")
        {
            panel_readonly1.Visible = false;
            panel_readonly2.Visible = false;
        }
        else
        {
            Response.Redirect("~/AccessDenied.aspx");
        }
        if (!IsPostBack)
        {
            log.WriteLog(LogUtility.LogErrorLevel.LOG_ACCESS, "RSMSalesOrg Access.");
            str_salesOrgID = 0;
            initVisible();

            getsearchIN();
            list.bindFind(list.getSalesOrgName(), ddlist_find);

            bindDataSourceSaleOrg();
        }
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

    /* Get  search options */
    private void getsearchIN()
    {
        ddlist_in.Width = 100;
        ddlist_in.Items.Add(new ListItem("Sales Org Name", "0"));
        ddlist_in.Items.Add(new ListItem("Sales Org Abbr", "1"));
        ddlist_in.Items.Add(new ListItem("Currency", "2"));
    }

    protected void lbtn_findhelp_Click(object sender, EventArgs e)
    {
        string str_args = "'RSMHelp.aspx'" + ",'Help', 'height=500,width=800,top=100,left=200,toolbar=no,status=no, menubar=no,location=no,resizable=no,scrollbars=yes'";
        Response.Write("<script   language='javascript'>window.open(" + str_args + ");</script>");
    }

    protected void initVisible()
    {
        label_salesOeg_edit_del.Visible = false;
        label_salesOrg_add.Visible = false;
        panel_add.Visible = false;
        pnl_updatecurrency.Visible = false;

        label_RSM_del.Visible = false;
        label_RSM_add.Visible = false;
        panel_addRSM.Visible = false;

        label_Segment_del.Visible = false;
        label_Segment_add.Visible = false;
        panel_addSegment.Visible = false;

        panel_select.Visible = false;
    }

    protected DataSet getSalesOrgInfo(string str_Name, int sel)
    {
        string query_SalesOrg = "SELECT [SalesOrg].ID,[SalesOrg].Name AS 'Sales Organization',[SalesOrg].Abbr, [Currency].Name AS 'Currency'"
                            + " FROM [SalesOrg] INNER JOIN [Currency]"
                            + " ON [SalesOrg].CurrencyID = [Currency].ID"
                            + " WHERE [SalesOrg].Deleted = 0 and [Currency].Deleted = 0";
        if (sel == 0)
        {
            query_SalesOrg += " AND [SalesOrg].Name like '%" + str_Name + "%'"
                           + " ORDER BY [SalesOrg].Name ASC";
        }
        else if (sel == 1)
        {
            query_SalesOrg += " AND[SalesOrg].Abbr like '%" + str_Name + "%'"
                           + " ORDER BY [SalesOrg].Name ASC";
        }
        else if (sel == 2)
        {
            query_SalesOrg += " AND [Currency].Name like '%" + str_Name + "%'"
                           + " ORDER BY [SalesOrg].Name ASC";
        }
        else
            query_SalesOrg += " ORDER BY [SalesOrg].Name ASC";

        DataSet ds_SalesOrg = helper.GetDataSet(query_SalesOrg);
        return ds_SalesOrg;
    }

    protected DataSet getRSMInfo()
    {
        //string query_RSM = "SELECT [User].UserID,[User].Abbr AS 'User Abbr' "
        //                + " FROM [SalesOrg_User] INNER JOIN [SalesOrg]"
        //                + " ON [SalesOrg_User].SalesOrgID = [SalesOrg].ID"
        //                + " INNER JOIN [User] "
        //                + " ON [SalesOrg_User].UserID = [User].UserID"
        //                + " WHERE [SalesOrg].ID = " + str_salesOrgID + " AND [SalesOrg].Deleted = 0 AND [User].Deleted = 0 AND [SalesOrg_User].Deleted = 0"
        //                + " ORDER BY [User].Alias ASC";
        //DataSet ds_RSM = helper.GetDataSet(query_RSM);
        //return ds_RSM;

        string query_RSM = "SELECT [User].UserID,[User].Abbr+'('+[Role].Name+')' AS 'User Abbr' "
                        + " FROM [SalesOrg_User] INNER JOIN [SalesOrg]"
                        + " ON [SalesOrg_User].SalesOrgID = [SalesOrg].ID"
                        + " INNER JOIN [User] "
                        + " ON [SalesOrg_User].UserID = [User].UserID "
                        + " inner join [Role] on [Role].ID = [User].RoleID"
                        + " WHERE [SalesOrg].ID = " + str_salesOrgID + " AND [SalesOrg].Deleted = 0 AND [User].Deleted = 0 AND [SalesOrg_User].Deleted = 0"
                        + " ORDER BY [User].Alias ASC";
        DataSet ds_RSM = helper.GetDataSet(query_RSM);
        return ds_RSM;
    }

    protected DataSet getSegmentInfo()
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

    protected void getCurrencyInfo()
    {
        string query_currency = "SELECT ID, Name FROM [Currency] WHERE Deleted = 0 GROUP BY Name,ID ORDER BY Name,ID ASC";
        DataSet ds_currency = helper.GetDataSet(query_currency);

        if (ds_currency.Tables[0].Rows.Count > 0)
        {
            DataTable dt_currency = ds_currency.Tables[0];
            int countcurrency = dt_currency.Rows.Count;
            int indexcourrency = 0;
            while (indexcourrency < countcurrency)
            {
                this.ddlist_currency.Items.Add(new ListItem(dt_currency.Rows[indexcourrency][1].ToString(), dt_currency.Rows[indexcourrency][0].ToString()));
                this.ddlist_currency2.Items.Add(new ListItem(dt_currency.Rows[indexcourrency][1].ToString(), dt_currency.Rows[indexcourrency][0].ToString()));
                indexcourrency++;
            }
            ddlist_currency.Enabled = true;
            ddlist_currency2.Enabled = true;
        }
        else
        {
            log.WriteLog(LogUtility.LogErrorLevel.LOG_ERROR, info.errorInfo("Table Currency_Exchange"));
            ddlist_currency.Enabled = false;
            ddlist_currency2.Enabled = false;
        }
    }

    protected void bindDataSourceSaleOrg()
    {
        bool flag = true;

        string str_content = ddlist_find.Text.Trim();
        int sel = int.Parse(ddlist_in.SelectedItem.Value.Trim());

        DataSet ds_SalesOrg = getSalesOrgInfo(str_content, sel);

        if (ds_SalesOrg.Tables[0].Rows.Count == 0)
        {
            flag = false;
            sql.getNullDataSet(ds_SalesOrg);
        }
        gv_salsOrg.Width = Unit.Pixel(800);
        gv_salsOrg.AutoGenerateColumns = false;
        //By SJ 20110511 ITEM  Update Start
        //gv_salsOrg.AllowPaging = true;
        gv_salsOrg.AllowPaging = false;
        //By SJ 20110511 ITEM  Update End
        gv_salsOrg.Visible = true;

        for (int i = 0; i < ds_SalesOrg.Tables[0].Columns.Count; i++)
        {
            BoundField bf = new BoundField();

            bf.DataField = ds_SalesOrg.Tables[0].Columns[i].ColumnName.ToString();
            bf.HeaderText = ds_SalesOrg.Tables[0].Columns[i].Caption.ToString();
            bf.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
            bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;

            bf.ReadOnly = false;
            if (i == 0 || i == 3)
                bf.ReadOnly = true;

            if (i == 1)
            {
                bf.ItemStyle.Width = 400;
                bf.ControlStyle.Width = bf.ItemStyle.Width;
            }
            if (i == 2)
            {
                bf.ItemStyle.Width = 100;
                bf.ControlStyle.Width = bf.ItemStyle.Width;
            }

            gv_salsOrg.Columns.Add(bf);
        }

        CommandField cf_Select = new CommandField();
        cf_Select.ButtonType = ButtonType.Image;
        cf_Select.ShowSelectButton = true;
        cf_Select.ShowCancelButton = true;
        cf_Select.SelectImageUrl = "~/images/search.jpg";
        cf_Select.SelectText = "Select";
        cf_Select.CausesValidation = false;
        gv_salsOrg.Columns.Add(cf_Select);

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
        gv_salsOrg.Columns.Add(cf_Update);

        CommandField cf_Delete = new CommandField();
        cf_Delete.ButtonType = ButtonType.Image;
        cf_Delete.ShowDeleteButton = true;
        cf_Delete.ShowCancelButton = true;
        cf_Delete.CausesValidation = false;
        cf_Delete.DeleteImageUrl = "~/images/del.jpg";
        cf_Delete.DeleteText = "Delete";
        gv_salsOrg.Columns.Add(cf_Delete);

        gv_salsOrg.AllowSorting = true;
        gv_salsOrg.DataSource = ds_SalesOrg.Tables[0];
        gv_salsOrg.DataBind();
        gv_salsOrg.Columns[0].Visible = false;
        gv_salsOrg.Columns[gv_salsOrg.Columns.Count - 1].Visible = flag;
        gv_salsOrg.Columns[gv_salsOrg.Columns.Count - 2].Visible = flag;
        if (getRoleID(getRole()) != "0")
        {
            gv_salsOrg.Columns[gv_salsOrg.Columns.Count - 1].Visible = false;
            gv_salsOrg.Columns[gv_salsOrg.Columns.Count - 2].Visible = false;
        }
        label_RSM_del.Text = "";
    }

    /* SalesOrganization */

    protected void gv_salsOrg_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        panel_select.Visible = false;
        gv_salsOrg.SelectedIndex = -1;
        gv_salsOrg.PageIndex = e.NewPageIndex;
        bindDataSource();
    }

    protected void gv_salsOrg_RowEditing(object sender, GridViewEditEventArgs e)
    {
        gv_salsOrg.SelectedIndex = -1;
        gv_salsOrg.EditIndex = e.NewEditIndex;
        panel_select.Visible = false;
        label_salesOeg_edit_del.Text = "";
        bindDataSource();
    }

    protected void gv_salsOrg_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        gv_salsOrg.SelectedIndex = -1;
        label_salesOeg_edit_del.Visible = true;
        label_salesOeg_edit_del.ForeColor = System.Drawing.Color.Red;

        string salesOrgID = gv_salsOrg.Rows[e.RowIndex].Cells[0].Text.Trim();
        string str_name = ((TextBox)(gv_salsOrg.Rows[e.RowIndex].Cells[1].Controls[0])).Text.Trim();
        string str_abbr = ((TextBox)(gv_salsOrg.Rows[e.RowIndex].Cells[2].Controls[0])).Text.Trim();

        string update_salesOrg = "UPDATE [SalesOrg] SET "
                                + " Name = '" + str_name + "' ,"
                                + " Abbr = '" + str_abbr + "' WHERE ID = " + salesOrgID + " AND Deleted = 0";
        int count = helper.ExecuteNonQuery(CommandType.Text, update_salesOrg, null);

        if (count > 0)
        {
            label_salesOeg_edit_del.ForeColor = System.Drawing.Color.Green;
            label_salesOeg_edit_del.Text = info.edtLabelInfo(str_name, true);
        }
        else
            label_salesOeg_edit_del.Text = info.edtLabelInfo(str_name, false);

        gv_salsOrg.EditIndex = -1;
        panel_select.Visible = false;
        bindDataSource();
    }

    protected void gv_salsOrg_RowCancelingEdit1(object sender, GridViewCancelEditEventArgs e)
    {
        gv_salsOrg.EditIndex = -1;
        panel_select.Visible = false;
        bindDataSource();
    }

    protected void gv_salsOrg_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        gv_salsOrg.SelectedIndex = -1;
        label_salesOeg_edit_del.Visible = true;
        label_salesOeg_edit_del.Text = "";
        label_salesOeg_edit_del.ForeColor = System.Drawing.Color.Red;

        DataSet ds_RSM = getRSMInfo();
        DataSet ds_Segment = getSegmentInfo();

        string str_name = gv_salsOrg.Rows[e.RowIndex].Cells[1].Text.Trim();
        string str_ID = gv_salsOrg.Rows[e.RowIndex].Cells[0].Text.Trim();

        if (ds_RSM == null && ds_Segment == null)
        {
            string del_salesOrg = "UPDATE [SalesOrg] SET Deleted = 1 WHERE ID = " + str_ID;
            int count = helper.ExecuteNonQuery(CommandType.Text, del_salesOrg, null);

            if (count > 0)
            {
                label_salesOeg_edit_del.ForeColor = System.Drawing.Color.Green;
                label_salesOeg_edit_del.Text = info.delLabelInfo(str_name, true);
            }
            else
                label_salesOeg_edit_del.Text = info.delLabelInfo(str_name, false);
        }
        else
            label_salesOeg_edit_del.Text = "Because of being not null," + str_name + " cannot be deleted.";

        panel_select.Visible = false;
        bindDataSource();
    }

    protected void gv_salsOrg_SelectedIndexChanging(object sender, GridViewSelectEventArgs e)
    {
        lbtn_AddSalesOrg.Enabled = true;
        lbtn_AddSalesOrg.Text = "Add SalesOrganization";
        panel_select.Visible = true;
        panel_add.Visible = false;
        //By SJ 20110511 ITEM  ADD Start
        label_RSM.Visible = true;
        label_Segments.Visible = true;
        //By SJ 20110511 ITEM  ADD End

        gv_salsOrg.SelectedIndex = e.NewSelectedIndex;
        int index = 0;
        index = gv_salsOrg.SelectedIndex;
        if (index < 0 || gv_salsOrg.Rows.Count < 1)
            Response.Redirect("~/RSM/RSMError.aspx");
        str_salesOrgID = Convert.ToInt16(gv_salsOrg.Rows[index].Cells[0].Text);

        string str_salesOrgName;
        str_salesOrgName = gv_salsOrg.Rows[index].Cells[1].Text.ToString().Trim();
        label_caption.Text = str_salesOrgName.ToString().Trim();

        gv_salsOrg.SelectedIndex = -1;
        bindDataSource();
    }

    protected void gv_salsOrg_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (e.Row.RowState == DataControlRowState.Normal || e.Row.RowState == DataControlRowState.Alternate)
            {
                ((ImageButton)e.Row.Cells[gv_salsOrg.Columns.Count - 1].Controls[0]).Attributes.Add("onclick", info.setRowDataBound(e.Row.Cells[1].Text));
            }
        }
    }

    protected bool existSalesOrg(string strname, string strabbr)
    {
        string query_sales = "SELECT ID FROM [SalesOrg] WHERE Name = '" + strname + "' AND Abbr = '" + strabbr + "' AND Deleted = 0";
        DataSet ds_sales = helper.GetDataSet(query_sales);

        if (ds_sales.Tables[0].Rows.Count > 0)
            return false;
        else
            return true;
    }

    protected void lbtn_AddSalesOrg_Click(object sender, EventArgs e)
    {
        panel_add.Visible = true;
        panel_select.Visible = false;
        lbtn_AddSalesOrg.Enabled = false;

        lbtn_AddSalesOrg.Text = "Input Name, Abbr And Master of SalesOrg";
        label_salesOrg_add.Text = "";
        tbox_name.Text = "";
        tbox_abbr.Text = "";

        ddlist_currency.Items.Clear();
        getCurrencyInfo();
        gv_salsOrg.Columns.Clear();
        bindDataSourceSaleOrg();
    }

    protected void btn_AddSalesOrg_Click(object sender, EventArgs e)
    {
        panel_add.Visible = false;
        lbtn_AddSalesOrg.Enabled = true;
        lbtn_AddSalesOrg.Text = "Add SalesOrganization";
        label_salesOrg_add.Visible = true;
        label_salesOrg_add.ForeColor = System.Drawing.Color.Red;

        string str_name = tbox_name.Text.Trim();
        string str_abbr = tbox_abbr.Text.Trim();
        string str_currency = ddlist_currency.SelectedItem.Value.Trim();

        if (str_name != "" && str_abbr != "")
        {
            if (existSalesOrg(str_name, str_abbr))
            {
                string insert_operation = "INSERT INTO [SalesOrg](Name,Abbr,CurrencyID,Deleted) "
                                        + " VALUES('" + str_name + "','" + str_abbr + "','" + str_currency + "',0)";
                int count = helper.ExecuteNonQuery(CommandType.Text, insert_operation, null);

                if (count > 0)
                {
                    label_salesOrg_add.ForeColor = System.Drawing.Color.Green;
                    label_salesOrg_add.Text = info.addLabelInfo(str_name, true);
                }
                else
                    label_salesOrg_add.Text = info.addLabelInfo(str_name, false);
            }
            else
            {
                label_salesOrg_add.Text = info.addExist(str_name);
            }
        }
        else
        {
            label_salesOrg_add.Text = info.addillegal();
        }

        ddlist_currency.Items.Clear();
        gv_salsOrg.Columns.Clear();
        bindDataSourceSaleOrg();
    }

    /// <summary>
    /// bindDataSource
    /// </summary>
    /// <param name="gridview"></param>
    /// <param name="str_caption"></param>
    /// <param name="ds"></param>
    /// <param name="sel"></param>
    /// 
    protected void bindDataSource(GridView gridview, string str_caption, DataSet ds)
    {
        bool flag = true;

        if (ds.Tables[0].Rows.Count == 0)
        {
            flag = false;
            sql.getNullDataSet(ds);
        }
        gridview.Width = Unit.Pixel(300);
        gridview.AutoGenerateColumns = false;
        gridview.AllowPaging = true;
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

        //By SJ 20110511 ITEM  DEL Start
        //gridview.Caption = str_caption;
        //gridview.CaptionAlign = TableCaptionAlign.Top;
        //By SJ 20110511 ITEM  DEL End
        gridview.AllowSorting = true;
        gridview.DataSource = ds.Tables[0];
        gridview.DataBind();
        gridview.Columns[0].Visible = false;
        gridview.Columns[gridview.Columns.Count - 1].Visible = flag;
        if (getRoleID(getRole()) != "0")
            gridview.Columns[gridview.Columns.Count - 1].Visible = false;
    }

    protected void bindDataSource()
    {
        gv_RSM.Columns.Clear();
        gv_Segment.Columns.Clear();
        gv_salsOrg.Columns.Clear();

        bindDataSource(gv_RSM, "Staff", getRSMInfo());
        bindDataSource(gv_Segment, "Segment", getSegmentInfo());
        bindDataSourceSaleOrg();
    }

    /* RSM */
    protected void gv_RSM_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        gv_RSM.SelectedIndex = -1;
        label_RSM_del.Visible = true;
        label_RSM_del.Text = "";
        label_RSM_del.ForeColor = System.Drawing.Color.Red;

        string str_ID = gv_RSM.Rows[e.RowIndex].Cells[0].Text.ToString().Trim();
        string str_Alias = gv_RSM.Rows[e.RowIndex].Cells[1].Text.ToString().Trim();

        string del_RSM = "UPDATE [SalesOrg_User] SET Deleted = 1 WHERE RSMID = " + str_ID + " AND SalesOrgID = " + str_salesOrgID;
        int count = helper.ExecuteNonQuery(CommandType.Text, del_RSM, null);

        if (count > 0)
        {
            label_RSM_del.ForeColor = System.Drawing.Color.Green;
            label_RSM_del.Text = info.delLabelInfo(str_Alias, true);
        }
        else
            label_RSM_del.Text = info.delLabelInfo(str_Alias, false);

        ddlist_RSM.Items.Clear();
        getRSMInfo(str_salesOrgID.ToString().Trim());
        bindDataSource();
    }

    protected void gv_RSM_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (e.Row.RowState == DataControlRowState.Normal || e.Row.RowState == DataControlRowState.Alternate)
            {
                ((ImageButton)e.Row.Cells[gv_RSM.Columns.Count - 1].Controls[0]).Attributes.Add("onclick", info.setRowDataBound(e.Row.Cells[1].Text));
            }
        }
    }

    protected void getRSMInfo(string str_ID)
    {
        string query_RSM = "SELECT UserID, Abbr FROM [User] WHERE Deleted = 0 AND (RoleID = 4 OR RoleID = 3 OR RoleID = 2)"
                          + " AND UserID NOT IN"
                          + " (SELECT [User].UserID "
                          + " FROM [SalesOrg_User] INNER JOIN [User] "
                          + " ON [SalesOrg_User].UserID = [User].UserID "
                          + " INNER JOIN [SalesOrg] "
                          + " ON [SalesOrg].ID = [SalesOrg_User].SalesOrgID "
                          + " WHERE [SalesOrg].Deleted = 0 AND [User].Deleted = 0"
                          + " AND [SalesOrg].ID = " + str_ID
                          + " AND [SalesOrg_User].Deleted = 0 AND (RoleID = 4 OR RoleID = 3 OR RoleID = 2))"
                          + " ORDER BY Abbr, UserID ASC";
        DataSet ds_RSM = helper.GetDataSet(query_RSM);

        if (ds_RSM.Tables.Count > 0)
        {
            if (ds_RSM.Tables[0].Rows.Count > 0)
            {
                DataTable dt_RSM = ds_RSM.Tables[0];
                int countRSM = dt_RSM.Rows.Count;
                int indexRSM = 0;
                while (indexRSM < countRSM)
                {
                    this.ddlist_RSM.Items.Add(new ListItem(dt_RSM.Rows[indexRSM][1].ToString(), dt_RSM.Rows[indexRSM][0].ToString()));
                    indexRSM++;
                }
                ddlist_RSM.Enabled = true;
                btn_addRSM.Enabled = true;
            }
            else
            {
                ddlist_RSM.Enabled = false;
                btn_addRSM.Enabled = false;
            }
        }
    }

    protected void lbtn_RSM_Click(object sender, EventArgs e)
    {
        lbtn_RSM.Text = "Select login:";
        lbtn_RSM.Enabled = false;
        panel_addRSM.Visible = true;
        label_RSM_add.Text = "";
        label_RSM_del.Text = "";

        getRSMInfo(str_salesOrgID.ToString());
        bindDataSource();
    }

    protected void btn_addRSM_Click(object sender, EventArgs e)
    {
        lbtn_RSM.Text = "Add User";
        lbtn_RSM.Enabled = true;
        panel_addRSM.Visible = false;

        label_RSM_add.Visible = true;
        label_RSM_add.ForeColor = System.Drawing.Color.Red;

        string str_name = ddlist_RSM.SelectedItem.Text.Trim();
        string str_ID = ddlist_RSM.SelectedItem.Value.Trim(); ;

        if (str_ID != null)
        {
            string insert_salesOrg_RSM = "INSERT INTO [SalesOrg_User] VALUES(" + str_salesOrgID + "," + str_ID + " ,0)";
            int count = helper.ExecuteNonQuery(CommandType.Text, insert_salesOrg_RSM, null);

            if (count > 0)
            {
                label_RSM_add.ForeColor = System.Drawing.Color.Green;
                label_RSM_add.Text = info.addLabelInfo(str_name, true);
            }
            else
                label_RSM_add.Text = info.addLabelInfo(str_name, false);
        }

        ddlist_RSM.Items.Clear();
        bindDataSource();
    }

    /* Segment */

    protected void gv_Segment_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        label_Segment_del.Visible = true;
        label_Segment_del.Text = "";

        string str_segmentID = gv_Segment.Rows[e.RowIndex].Cells[0].Text.Trim();
        string str_segmentName = gv_Segment.Rows[e.RowIndex].Cells[1].Text.Trim();
        string del_segment = "UPDATE [SalesOrg_Segment] SET Deleted = 1 "
                            + " WHERE SegmentID = " + str_segmentID + " AND SalesOrgID = " + str_salesOrgID;
        int count = helper.ExecuteNonQuery(CommandType.Text, del_segment, null);

        if (count > 0)
        {
            label_Segment_del.ForeColor = System.Drawing.Color.Green;
            label_Segment_del.Text = info.delLabelInfo(str_segmentName, true);
        }
        else
        {
            label_Segment_del.ForeColor = System.Drawing.Color.Red;
            label_Segment_del.Text = info.delLabelInfo(str_segmentName, false);
        }

        ddlist_segment.Items.Clear();
        getSegmentInfo(str_salesOrgID.ToString().Trim());
        bindDataSource();
    }

    protected void gv_Segment_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (e.Row.RowState == DataControlRowState.Normal || e.Row.RowState == DataControlRowState.Alternate)
            {
                ((ImageButton)e.Row.Cells[gv_Segment.Columns.Count - 1].Controls[0]).Attributes.Add("onclick", info.setRowDataBound(e.Row.Cells[1].Text));
            }
        }
    }

    protected void getSegmentInfo(string str_ID)
    {
        string query_Segment = "SELECT ID, Abbr FROM [Segment] WHERE Deleted = 0 "
                           + " AND ID NOT IN"
                           + " (SELECT [Segment].ID "
                           + " FROM [SalesOrg_Segment] INNER JOIN [Segment] "
                           + " ON [SalesOrg_Segment].SegmentID  = [Segment].ID "
                           + " INNER JOIN [SalesOrg] "
                           + " ON [SalesOrg].ID = [SalesOrg_Segment].SalesOrgID "
                           + " WHERE [SalesOrg].Deleted = 0 AND [Segment].Deleted = 0 AND [SalesOrg].ID = "
                           + str_ID + " AND [SalesOrg_Segment].Deleted = 0)";
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
                    this.ddlist_segment.Items.Add(new ListItem(dt_segment.Rows[indexsegment][1].ToString(), dt_segment.Rows[indexsegment][0].ToString()));
                    indexsegment++;
                }
                ddlist_segment.Enabled = true;
                btn_addSegment.Enabled = true;
            }
            else
            {
                btn_addSegment.Enabled = false;
                ddlist_segment.Enabled = false;
            }
        }
    }

    protected void lbtn_segment_Click(object sender, EventArgs e)
    {
        lbtn_segment.Text = "Select segment:";
        lbtn_segment.Enabled = false;
        panel_addSegment.Visible = true;
        label_Segment_add.Text = "";
        label_Segment_del.Text = "";

        getSegmentInfo(str_salesOrgID.ToString());
        bindDataSource();
    }

    protected void btn_addSegment_Click(object sender, EventArgs e)
    {
        lbtn_segment.Text = "Add Segment";
        lbtn_segment.Enabled = true;
        panel_addSegment.Visible = false;

        label_Segment_add.Visible = true;

        string str_name = ddlist_segment.SelectedItem.Text.Trim();
        string str_ID = ddlist_segment.SelectedItem.Value.Trim();

        if (str_ID != null)
        {
            string insert_salesOrg_segment = "INSERT INTO [SalesOrg_Segment] VALUES(" + str_salesOrgID + "," + str_ID + " ,0)";
            int count = helper.ExecuteNonQuery(CommandType.Text, insert_salesOrg_segment, null);

            if (count > 0)
            {
                label_Segment_add.ForeColor = System.Drawing.Color.Green;
                label_Segment_add.Text = info.addLabelInfo(str_name, true);
            }
            else
            {
                label_Segment_add.ForeColor = System.Drawing.Color.Red;
                label_Segment_add.Text = info.addLabelInfo(str_name, false);
            }
        }

        ddlist_segment.Items.Clear();
        bindDataSource();
    }

    protected void btn_CancelSalesOrg_Click(object sender, EventArgs e)
    {
        panel_add.Visible = false;
        lbtn_AddSalesOrg.Enabled = true;
        lbtn_AddSalesOrg.Text = "Add SalesOrganization";
        ddlist_currency.Items.Clear();
    }

    protected void btn_CancelRSM_Click(object sender, EventArgs e)
    {
        lbtn_RSM.Text = "Add User";
        lbtn_RSM.Enabled = true;
        panel_addRSM.Visible = false;

        ddlist_RSM.Items.Clear();
    }

    protected void btn_CancelSegment_Click(object sender, EventArgs e)
    {
        lbtn_segment.Text = "Add Segment";
        lbtn_segment.Enabled = true;
        panel_addSegment.Visible = false;

        ddlist_segment.Items.Clear();
    }

    protected void gv_RSM_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        gv_RSM.PageIndex = e.NewPageIndex;
        bindDataSource();
    }

    protected void gv_Segment_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        gv_Segment.PageIndex = e.NewPageIndex;
        bindDataSource();
    }

    protected DataSet getSalesOrg()
    {
        string sql = "SELECT ID,Abbr FROM [SalesOrg] WHERE Deleted = 0 ORDER BY Abbr ASC";
        DataSet ds = helper.GetDataSet(sql);

        if (ds.Tables[0].Rows.Count > 0)
            return ds;
        else
            return null;
    }

    protected void bind(DataSet ds, DropDownList ddlist)
    {
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
            ListItem linull = new ListItem("", "-1");
            ddlist.Items.Add(linull);
            ddlist.Enabled = false;
        }
    }

    protected void lbtn_updatecurrency_Click(object sender, EventArgs e)
    {
        pnl_updatecurrency.Visible = true;
        lbtn_updatecurrency.Enabled = false;
        lbtn_updatecurrency.Text = "Modify currency";
        label_salesOrg_add.Text = "";

        ddlist_salesorg.Items.Clear();
        bind(getSalesOrg(), ddlist_salesorg);
        ddlist_currency2.Items.Clear();
        getCurrencyInfo();
        gv_salsOrg.Columns.Clear();
        bindDataSourceSaleOrg();
    }

    protected void btn_addcurrency_Click(object sender, EventArgs e)
    {
        lbtn_updatecurrency.Text = "Modify currency";
        lbtn_updatecurrency.Enabled = true;
        pnl_updatecurrency.Visible = false;
        label_salesOrg_add.ForeColor = System.Drawing.Color.Red;

        string str_salesorgID = ddlist_salesorg.SelectedItem.Value.Trim();
        string str_currency = ddlist_currency2.SelectedItem.Text.Trim();
        string str_currencyID = ddlist_currency2.SelectedItem.Value.Trim();

        string update_currency = " UPDATE [SalesOrg] SET CurrencyID = '" + str_currencyID + "'"
                              + " WHERE Deleted = 0 AND ID = '" + str_salesorgID + "'";
        int count = helper.ExecuteNonQuery(CommandType.Text, update_currency, null);
        if (count == 1)
        {
            label_salesOrg_add.ForeColor = System.Drawing.Color.Green;
            label_salesOrg_add.Text = info.addLabelInfo(str_currency, true);
        }
        else
            label_salesOrg_add.Text = info.addLabelInfo(str_currency, false);

        gv_salsOrg.Columns.Clear();
        bindDataSourceSaleOrg();
    }

    protected void btn_cancelcurrency_Click(object sender, EventArgs e)
    {
        lbtn_updatecurrency.Text = "Modify currency";
        lbtn_updatecurrency.Enabled = true;
        pnl_updatecurrency.Visible = false;
    }

    protected void btn_find_Click(object sender, EventArgs e)
    {
        panel_select.Visible = false;
        gv_salsOrg.Columns.Clear();
        bindDataSourceSaleOrg();
    }

    //Find
    FindList list = new FindList();

    protected void ddlist_in_SelectedIndexChanged(object sender, EventArgs e)
    {
        int sel = int.Parse(ddlist_in.SelectedItem.Value.Trim());
        ddlist_find.Items.Clear();
        switch (sel)
        {
            case 0:
                {
                    list.bindFind(list.getSalesOrgName(), ddlist_find);
                    break;
                }
            case 1:
                {
                    list.bindFind(list.getSalesOrgAbbr(), ddlist_find);
                    break;
                }
            case 2:
                {
                    list.bindFind(list.getCurrencyName(), ddlist_find);
                    break;
                }
        }
    }
}
