/*
 *  File Name     : AdminSalesData.aspx.cs
 * 
 *  Description   : Get and set sale data
 * 
 *  Author        : Wang Jun
 * 
 *  Modified Date : 2010-12-21
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
using System.Text;
using System.Drawing;

public partial class Admin_AdminSalesData : System.Web.UI.Page
{
    SQLHelper helper = new SQLHelper();
    SQLStatement sql = new SQLStatement();
    WebUtility web = new WebUtility();
    LogUtility log = new LogUtility();
    GetMeetingDate meeting = new GetMeetingDate();
    CommonFunction cf = new CommonFunction();
    DataSet dataDetail = null;
    bool dataFlag = true;

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
        else if (getRoleID(getRole()) == "5")
        {
            panel_readonly.Visible = false;
        }
        else
        {
           Response.Redirect("~/AccessDenied.aspx");
        }
        if (!IsPostBack)
        {
            ddlist_segment.Items.Clear();
            ddlist_saleorg.Items.Clear();
            /*by ryzhang 20110510 item37 add start*/
            this.panel_search.Visible = false;
            /*by ryzhang 20110510 item37 add end*/
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
            getOperationInfoByGeneralMarketingID(getGeneralMarketingID());
            getSegmentByOperation(this.ddlist_operation.SelectedItem.Value.Trim());
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

    protected void getOperationInfoByGeneralMarketingID(string str_GeneralMarketingID)
    {
        string sql = "SELECT [Operation].AbbrL,[Operation].ID FROM [User_Operation] "
                            + " INNER JOIN [Operation] ON [Operation].ID = [User_Operation].OperationID"
                            + " WHERE UserID = '" + str_GeneralMarketingID + "'"
            // By daixuesong 20110516 Item35 ADD Start
                            + " AND [Operation].Deleted = 0"
                            + " AND [User_Operation].Deleted = 0"
            // By daixuesong 20110516 Item35 ADD END
                            + " GROUP BY [Operation].ID,[Operation].AbbrL";

        DataSet ds = helper.GetDataSet(sql);

        if (bindDropDownList(ds, ddlist_operation))
            btn_search.Enabled = true;
        else
            btn_search.Enabled = false;
        //ryzhang 20110512 item37 del start
        //if (ds.Tables[0].Rows.Count == 1)
        //    return ds.Tables[0].Rows[0][0].ToString().Trim();
        //else
        //    return "";
        //ryzhang 20110512 item37 del end
    }
    //ryzhang 20110512 item37 add start
    protected void getSegmentByOperation(string operationId)
    {
        //string query_string = "SELECT distinct S.Abbr,S.ID "
        //        + "FROM Segment s INNER JOIN Operation_Segment os ON s.ID = os.SegmentID AND s.Deleted!=1 AND os.Deleted != 1"
        //        + "WHERE os.OperationID = " + operationId;

        string query_string = "SELECT distinct S.Abbr,S.ID "
                + "FROM Segment s INNER JOIN [User_Segment] os ON s.ID = os.SegmentID AND s.Deleted!=1 AND os.Deleted != 1"
                + "WHERE os.UserID = " + getGeneralMarketingID();

        DataSet ds = helper.GetDataSet(query_string);

        if (bindDropDownList(ds, this.ddlist_segment))
            btn_search.Enabled = true;
        else
            btn_search.Enabled = false;
    }
    //ryzhang 20110512 item37 del start
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
                             + " ORDER BY [Product].Abbr";
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
            string str_operationID = this.ddlist_operation.SelectedItem.Value.Trim();
            if (str_operationID != "")
            {
                DataSet ds = getBacklog(dsPro, str_operationID, ddlist_saleorg.SelectedItem.Value.Trim(), ddlist_segment.SelectedItem.Value.Trim());
                dataDetail = getBackLogDetail();
                if (ds.Tables[0].Rows.Count == 0)
                {
                    sql.getNullDataSet(ds);
                    flag = false;
                    dataFlag = false;
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

                    if (i == 0 || i == 1)
                    {
                        bf.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
                        bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
                        bf.ReadOnly = true;
                    }
                    gv_actualBaclog.Columns.Add(bf);
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
                    cf_Update.UpdateText = "Update";
                    gv_actualBaclog.Columns.Add(cf_Update);
                    CommandField cf_Comments = new CommandField();
                    cf_Comments.ButtonType = ButtonType.Image;
                    cf_Comments.ShowSelectButton = true;
                    cf_Comments.SelectImageUrl = "~/images/comments.jpg";
                    cf_Comments.SelectText = "Comments";
                    cf_Comments.CausesValidation = false;
                    gv_actualBaclog.Columns.Add(cf_Comments);
                }
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
        /*by ryzhang 20110510 item37 add start*/
        this.panel_search.Visible = true;
        string str_operationID = this.ddlist_operation.SelectedItem.Value.Trim();
        //by yyan itemw82 20110721 add start 
        string sql_currency = "select Name from Currency where ID = (select CurrencyID from Operation where ID=" + str_operationID + " )";
        DataSet dsCurrency = helper.GetDataSet(sql_currency); ;
        if (dsCurrency.Tables[0].Rows.Count > 0)
        {
            lblCurrency.Text = dsCurrency.Tables[0].Rows[0][0].ToString();
        }
        else
        {
            lblCurrency.Text = "";
        }
        //TrafficLightRule.SetBLGreen(ddlist_marketingmgr.SelectedItem.Value, ddlist_segment.SelectedItem.Value,str_operationID, ddlist_saleorg.SelectedItem.Value);
        //by yyan itemw82 20110721 add end 
        //DataSet ds = this.GetActualSalesandBLStatus(ddlist_marketingmgr.SelectedItem.Value, ddlist_segment.SelectedItem.Value, ddlist_saleorg.SelectedItem.Value, this.ddlist_operation.SelectedItem.Value.Trim());
        //if (ds.Tables[0].Rows.Count == 0)
        //    TrafficLightRule.SetDefaultBLStatus(ddlist_marketingmgr.SelectedItem.Value, ddlist_segment.SelectedItem.Value, str_operationID, ddlist_saleorg.SelectedItem.Value);
        GetUserStatus();
        CheckLock(ddlist_marketingmgr.SelectedItem.Value, ddlist_segment.SelectedItem.Value);
        DataSet ds = this.GetActualSalesandBLStatus(ddlist_marketingmgr.SelectedItem.Value, ddlist_segment.SelectedItem.Value, this.ddlist_operation.SelectedItem.Value.Trim(), ddlist_saleorg.SelectedItem.Value);
        //don't find record
        this.ibtn_green.Enabled = true;
        this.ibtn_red.Enabled = true;
        if (ds.Tables[0].Rows.Count == 0)
        {
            //this.img_status.ImageUrl = "~/images/green.png";
            this.img_status.Visible = false;
            //this.ibtn_green.Enabled = false;
            //this.ibtn_red.Enabled = false;
            //this.gv_actualBaclog.Columns[this.gv_actualBaclog.Columns.Count - 1].Visible = false;
            //this.gv_actualBaclog.Columns[this.gv_actualBaclog.Columns.Count - 2].Visible = false;
            this.lbtn_add.Visible = false;
        }
        else
        {
            this.img_status.Visible = true;
            String status = ds.Tables[0].Rows[0][0].ToString().Trim();
            if (status=="G")
            {
                this.img_status.ImageUrl = "~/images/green.png";
                //this.ibtn_green.Enabled = false;
                //this.ibtn_red.Enabled = true;
                this.gv_actualBaclog.Columns[this.gv_actualBaclog.Columns.Count - 1].Visible = true;
                this.gv_actualBaclog.Columns[this.gv_actualBaclog.Columns.Count - 2].Visible = true;
                this.lbtn_add.Visible = true;
            }
            else
            {
                this.img_status.ImageUrl = "~/images/red.png";
                //this.ibtn_green.Enabled = true;
                this.gv_actualBaclog.Columns[this.gv_actualBaclog.Columns.Count - 1].Visible = false;
                this.gv_actualBaclog.Columns[this.gv_actualBaclog.Columns.Count - 2].Visible = false;
                //this.ibtn_red.Enabled = false;
                this.lbtn_add.Visible = false;
            }
        }
        if (lbtn_add.Text != "Add Backlog")
        {
            this.btn_cancel_Click(null,null);   
        }
        /*by ryzhang 20110510 item37 add end*/
    }

    protected void gv_actualBaclog_RowEditing(object sender, GridViewEditEventArgs e)
    {
        gv_actualBaclog.Columns.Clear();
        label_note.Text = "";
        gv_actualBaclog.EditIndex = e.NewEditIndex;
        bindDataSource();
        
        //gv_actualBaclog.ed

        //ryzhang 20110512 item37 add start
        if (lbtn_add.Text != "Add Backlog")
        {
            this.btn_cancel_Click(null, null);
        }
        lbtn_add.Visible = false;
        //ryzhang 20110512 item37 add end
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
        string str_operationID = this.ddlist_operation.SelectedItem.Value.Trim();
        DataSet ds_product = getProductInfoBySegmentID(str_segmentID);
        DataSet ds = getBacklog(ds_product, str_operationID, str_salesorgID, str_segmentID);
        string str_backlogY = ""; ;

        //by ryzhang 20110511 item? del start
        //if (ds.Tables[0].Rows.Count > 0)
        //by ryzhang 20110511 item? del end
        //by ryzhang 20110511 item? add start
        if (ds.Tables[0].Rows.Count > 0 && ds.Tables[0].Rows.Count > rIndex)
        //by ryzhang 20110511 item? add end
            str_backlogY = ds.Tables[0].Rows[rIndex][0].ToString().Substring(0, 2).Trim();
        if (str_backlogY != "")
        {
            for (int j = 2; j < gv.Columns.Count - 2; j++)
            {
                string pro = gv.HeaderRow.Cells[j].Text.ToString().Trim();
                string str_preData = ds.Tables[0].Rows[rIndex][j].ToString().Trim();
                string str_nexData = ((TextBox)(gv.Rows[rIndex].Cells[j].Controls[0])).Text.ToString().Trim();
                if (str_nexData == "")
                    str_nexData = "0";

                float f_preData;
                float f_nexData;

                if (System.Text.RegularExpressions.Regex.IsMatch(str_nexData, "^(0|[0-9]*)$"))
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
                                        + ",NULL)";
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
        string str_operationID = this.ddlist_operation.SelectedItem.Value.Trim();
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
        string str_operationID = this.ddlist_operation.SelectedItem.Value.Trim();
        string str_backlogY = ddlist_backlogY.Text.Trim();

        for (int j = 2; j < gv.Columns.Count - 1; j++)
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
                                + ",NULL)";
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
        ddlist_operation.Items.Clear();
        ddlist_saleorg.Items.Clear();
        ddlist_segment.Items.Clear();
        getOperationInfoByGeneralMarketingID(getGeneralMarketingID());
        getSegmentByOperation(ddlist_operation.SelectedItem.Value.Trim());
        getSalesOrgInfoBySegmentID(ddlist_segment.SelectedItem.Value.Trim());
    }

    /*by ryzhang 20110510 item37 add start*/
    private void SetStatus(String marketingMgrID, String segmentID, String operationID, String salesOrgID,String status)
    {
        string sql = null;
        DataSet ds = this.GetActualSalesandBLStatus(marketingMgrID, segmentID, operationID, salesOrgID);
        if (ds.Tables[0].Rows.Count == 0)
            sql = String.Format(@"INSERT INTO [ActualSalesandBL_Status] (MarketingMgrID,SegmentID,OperationID,SalesOrgID,[Status]) VALUES ({0},{1},{2},{3},'{4}')"
                , marketingMgrID, segmentID, operationID, salesOrgID, status);
        else
            sql = String.Format(@"UPDATE [ActualSalesandBL_Status] SET [Status] = '{0}' WHERE MarketingMgrID = {1} AND SegmentID = {2} AND OperationID = {3} AND  SalesOrgID = {4}"
                , status, marketingMgrID, segmentID, operationID, salesOrgID);
        int count = helper.ExecuteNonQuery(CommandType.Text, sql.Trim(), null);
    }

    private DataSet GetActualSalesandBLStatus(String marketingMgrID, String segmentID, String operationID, String salesOrgID)
    {
        string sql = String.Format(@"SELECT [Status] FROM [ActualSalesandBL_Status] WHERE MarketingMgrID = {0} AND SegmentID = {1} AND OperationID = {2} AND  SalesOrgID = {3}"
            , marketingMgrID, segmentID, operationID, salesOrgID);
        DataSet ds = helper.GetDataSet(sql);
        if (ds.Tables.Count==0)
        {
            ds.Tables.Add(new DataTable());
        }
        return ds;
    }
    
    protected void ibtn_red_Click(object sender, ImageClickEventArgs e)
    {
        string str_operationID = this.ddlist_operation.SelectedItem.Value.Trim();
        this.SetStatus(ddlist_marketingmgr.SelectedItem.Value, ddlist_segment.SelectedItem.Value, str_operationID, ddlist_saleorg.SelectedItem.Value, "R");
        this.btn_search_Click(null, null);
    }

    protected void ibtn_green_Click(object sender, ImageClickEventArgs e)
    {
        string str_operationID = this.ddlist_operation.SelectedItem.Value.Trim();
        this.SetStatus(ddlist_marketingmgr.SelectedItem.Value, ddlist_segment.SelectedItem.Value, str_operationID, ddlist_saleorg.SelectedItem.Value, "G");
        this.btn_search_Click(null, null);
    }


    protected void ddlist_operation_SelectedIndexChanged(object sender, EventArgs e)
    {
        ddlist_segment.Items.Clear();
        ddlist_saleorg.Items.Clear();
        if (ddlist_operation.SelectedItem.Value != "-1")
        {
            this.getSegmentByOperation(ddlist_operation.SelectedItem.Value.Trim());
            getSalesOrgInfoBySegmentID(ddlist_segment.SelectedItem.Value.Trim());
        }
    }
    /*by ryzhang 20110510 item37 add end*/

    protected override void Render(HtmlTextWriter writer)
    {
        foreach (GridViewRow row in gv_actualBaclog.Rows)
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
                if (dataFlag)
                {
                    row.Attributes["ondblclick"] = ClientScript.GetPostBackEventReference(gv_actualBaclog, "Edit$" + row.RowIndex, true);
                    row.Attributes["style"] = "cursor:pointer";
                    row.Attributes["title"] = "Double-click to edit";
                }
            }
        }
        base.Render(writer);
    }

    protected void gv_actualBaclog_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (dataFlag && this.gv_actualBaclog.EditIndex == -1)
            {
                string marketingMgrID = getGeneralMarketingID();
                string salesOrgID = this.ddlist_saleorg.SelectedValue;
                string segmentID = this.ddlist_segment.SelectedValue;
                string operationID = this.ddlist_operation.SelectedValue;
                string backLogY = e.Row.Cells[0].Text.Substring(0, 2);
                if (dataDetail != null)
                {
                    DataRow[] rows = null;
                    string comments = null;
                    for (int i = 2; i < this.gv_actualBaclog.Columns.Count; i++)
                    {
                        rows = dataDetail.Tables[0].Select("Abbr='" + this.gv_actualBaclog.Columns[i].HeaderText + "' AND BacklogY='" + backLogY + "'");
                        if (rows.Length != 0)
                        {
                            comments = rows[0][1].ToString();
                            if (!string.IsNullOrEmpty(comments))
                            {
                                e.Row.Cells[i].ForeColor = Color.Red;
                                e.Row.Cells[i].ToolTip = comments;
                            }
                        }
                    }
                }
                StringBuilder args = new StringBuilder();
                args.Append("?marketingMgrID=" + marketingMgrID);
                args.Append("&salesOrgID=" + salesOrgID);
                args.Append("&segmentID=" + segmentID);
                args.Append("&operationID=" + operationID);
                args.Append("&backLogY=" + backLogY);
                string modle = "height=350,width=400,top=100,left=200,toolbar=no,status=no, menubar=no,location=no,resizable=yes,scrollbars=yes";
                string windowArgs = "'../SalesDataComments.aspx" + args.ToString() + "','Comments','" + modle + "'";
                ((ImageButton)e.Row.Cells[this.gv_actualBaclog.Columns.Count - 1].Controls[0]).Attributes.Add("onclick", "window.open(" + windowArgs + ");return false;");
            }
        }
        if ((e.Row.RowState & DataControlRowState.Edit) != 0)
        {
           
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                if (e.Row.Cells[i].Controls != null && e.Row.Cells[i].Controls.Count>0)
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

    /// <summary>
    /// Get backlog detail
    /// </summary>
    private DataSet getBackLogDetail()
    {
        string marketingMgrID = getGeneralMarketingID();
        string salesOrgID = this.ddlist_saleorg.SelectedValue;
        string segmentID = this.ddlist_segment.SelectedValue;
        string operationID = this.ddlist_operation.SelectedValue;
        StringBuilder strSQL = new StringBuilder();
        strSQL.AppendLine(" SELECT ");
        strSQL.AppendLine("   Product.Abbr, ");
        strSQL.AppendLine("   ActualSalesandBL.Comments, ");
        strSQL.AppendLine("   ActualSalesandBL.BacklogY ");
        strSQL.AppendLine(" FROM ");
        strSQL.AppendLine("   ActualSalesandBL INNER JOIN Product ON ActualSalesandBL.ProductID=Product.ID ");
        strSQL.AppendLine(" WHERE ");
        strSQL.AppendLine("   ActualSalesandBL.MarketingMgrID=" + marketingMgrID);
        strSQL.AppendLine("   AND ActualSalesandBL.SalesOrgID=" + salesOrgID);
        strSQL.AppendLine("   AND ActualSalesandBL.SegmentID=" + segmentID);
        strSQL.AppendLine("   AND ActualSalesandBL.OperationID=" + operationID);
        strSQL.AppendLine("   AND YEAR(ActualSalesandBL.TimeFlag)=" + year);
        strSQL.AppendLine("   AND MONTH(ActualSalesandBL.TimeFlag)=" + month);
        strSQL.AppendLine("   AND Product.Deleted=0 ");
        strSQL.AppendLine(" ORDER BY ");
        strSQL.AppendLine("   ActualSalesandBL.BacklogY, ");
        strSQL.AppendLine("   ActualSalesandBL.ProductID ");
        DataSet ds = helper.GetDataSet(strSQL.ToString());
        if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
        {
            return null;
        }
        else
        {
            return ds;
        }
    }

    private bool CheckLock(string rsm, string segmentId)
    {
        if (TrafficLightRule.IsLock(Convert.ToInt32(rsm), Convert.ToInt32(segmentId)))
        {
            Master.LockPage();
            return true;
        }
        else
        {
            Master.UnLockPage();
            return false;
        }

    }

    private void GetUserStatus()
    {
        //string status = TrafficLightRule.GetUserStatus(str_userID, str_segmentID);
        //if (string.IsNullOrEmpty(status))
        //{

        //    return TrafficLightRule.InsertDefaultUserStatus(str_userID, str_segmentID);

        //}
        //else
        //    return status;

        DataSet ds = this.GetActualSalesandBLStatus(ddlist_marketingmgr.SelectedItem.Value, ddlist_segment.SelectedItem.Value, this.ddlist_operation.SelectedItem.Value.Trim(),ddlist_saleorg.SelectedItem.Value);
        if (ds.Tables[0].Rows.Count == 0)
            TrafficLightRule.SetDefaultBLStatus(ddlist_marketingmgr.SelectedItem.Value, ddlist_segment.SelectedItem.Value, this.ddlist_operation.SelectedItem.Value.Trim(), ddlist_saleorg.SelectedItem.Value);
    }
}
