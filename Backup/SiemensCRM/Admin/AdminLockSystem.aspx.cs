/*
 *  File Name   : AdminLockSystem.aspx.cs
 * 
 *  Description : Lock or Unlock some RSMs or some sales Org
 * 
 *  Author      : Wang Jun
 * 
 *  Modify      : 2010-12-29
 * 
 *  Problem     : 
 *  
 *  Version     : Release (2.0)
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

public partial class Admin_AdminLockSystem : System.Web.UI.Page
{
    SQLHelper helper = new SQLHelper();
    LogUtility log = new LogUtility();
    // by mbq 20110509 item13 add start   
    SQLStatement sql = new SQLStatement();
    DisplayInfo info = new DisplayInfo();
    LockInterface LockInterface = new LockInterface();
    //TrafficLightRule trafficLightRule = new TrafficLightRule();
    // by mbq 20110509 item13 add end   

    protected void Page_Load(object sender, EventArgs e)
    {
        Page.MaintainScrollPositionOnPostBack = true;
        /*by mbq 20110509 item13 add start*/
        this.lbtn_segment.Attributes.Add("onclick", "javascript:showDialog('AdminLockClickSelect.aspx?select=segment','segment');");
        this.lbtn_unsegment.Attributes.Add("onclick", "javascript:showDialog('AdminLockClickSelect.aspx?select=unsegment','unsegment');");
        /*by mbq 20110509 item13 add end*/
        if (getRoleID(getRole()) == "0")
        {
            panel_visible.Visible = true;
            label_visible.Text = "";
        }
        else if (getRoleID(getRole()) == "5")
        {
            panel_visible.Visible = false;
            label_visible.Text = "Sorry,the function can only be operated by administrator.";
        }
        else
            Response.Redirect("~/AccessDenied.aspx");

        if (!IsPostBack)
        {
            //autoUnlock();
            bindDataSource();
            label_lock.Text = "If you don't specify an unlock date,it will be locked indefinitely until you explicitly unlock it.";
        }

        /*by mbq 20110509 item13  add start*/
        string date = Request.Form["back_date"];
        String type = Request.Form["back_type"];
        String keys = Request.Form["back_key"];
        String[] a_keys = null;
        string insert_locksegment = null;

        if (type != null && type != "")
        {
            if (keys != null && !"".Equals(keys) && !"undefined".Equals(keys))
            {
                a_keys = keys.Split(',');
                if (type == "segment")
                {
                    //string Delete_locksegment = "DELETE FROM [LockSegment]";
                    //helper.ExecuteNonQuery(CommandType.Text, Delete_locksegment, null);
                    if (string.IsNullOrEmpty(date))
                        date = (new DateTime(2099, 1, 1)).ToString();

                    string delSql = string.Empty;
                    foreach (String var in a_keys)
                    {
                        delSql = "DELETE FROM [LockSegment] where SegmentID=" + var.Trim();
                        helper.ExecuteNonQuery(CommandType.Text, delSql, null);
                        if (string.IsNullOrEmpty(date) || string.Equals(date, "undefined"))
                        {
                            insert_locksegment = "INSERT INTO [LockSegment] VALUES('" + var.Trim() + "', NULL)";
                        }
                        else
                        {
                            insert_locksegment = "INSERT INTO [LockSegment] VALUES('" + var.Trim() + "','" + date + "')";
                        }
                        helper.ExecuteNonQuery(CommandType.Text, insert_locksegment, null);
                    }
                    label_lockSegmentNote.Text = "";
                    label_lockSegmentNote.ForeColor = System.Drawing.Color.Green;
                    label_lockSegmentNote.Text = "Locked successfully.";
                    label_unLockSegmentNote.Text = "";
                }
                else if (type == "unsegment")
                {
                    string upSql = string.Empty;
                    string upSql1 = string.Empty;
                    //if (a_keys != null && a_keys.Count() > 0)
                    //{
                    //    upSql = "delete from User_Status where SegmentID in (" + a_keys. + ") and UserID not in (select UserID from Lock where UnlockTime>=GETDATE() )";
                    //    helper.ExecuteNonQuery(CommandType.Text, upSql, null);
                    //}
                    foreach (String var in a_keys)
                    {
                        //upSql = "UPDATE [LockSegment] set UnlockTime='" + DateTime.Now.Date.AddDays(-10).ToString() + "' where SegmentID=" + var.Trim();
                        //helper.ExecuteNonQuery(CommandType.Text, upSql, null);
                        //upSql = "delete from User_Status where SegmentID =" + var.Trim() + " and UserID not in (select UserID from Lock where UnlockTime>=GETDATE() )";
                        //helper.ExecuteNonQuery(CommandType.Text, upSql, null);

                        //upSql1 = "delete from ActualSalesandBL_Status where SegmentID = "+var.Trim()
                        //    + " and "
                        //    + " MarketingMgrID not in (select UserID from Lock where UnlockTime>=GETDATE() )";
                        //helper.ExecuteNonQuery(CommandType.Text, upSql1, null);

                        insert_locksegment = "DELETE FROM  [LockSegment]  WHERE [LockSegment].SegmentID = " + var.Trim() + "";
                        helper.ExecuteNonQuery(CommandType.Text, insert_locksegment, null);

                        //string strSql = "DELETE FROM [User_Status] where SegmentID=" + var.Trim();
                        //helper.ExecuteNonQuery(CommandType.Text, strSql, null);
                    }
                    label_unLockSegmentNote.Text = "";
                    label_unLockSegmentNote.ForeColor = System.Drawing.Color.Red;
                    label_unLockSegmentNote.Text = "Unlocked successfully.";
                    label_lockSegmentNote.Text = "";
                }
            }
        }
        gv_segment.Columns.Clear();
        bindDataSource(gv_segment, "Segment", getsegmentInfo());
        //lock
        //bool lockflag = LockInterface.getLockUserData(Session["AdministratorID"].ToString().Trim());
        bool lockflag = TrafficLightRule.IsLockAll();
        if (lockflag == true)
        {
            lbtn_alluser.Enabled = false;
            //lbtn_unalluser.Enabled = false;
            lbtn_unalluser.Enabled = true;
            lbtn_segment.Enabled = false;
            lbtn_unsegment.Enabled = false;
            btn_lock.Enabled = false;
            btn_unlock.Enabled = false;
        }
        else
        {
            lbtn_alluser.Enabled = true;
            lbtn_unalluser.Enabled = true;
            lbtn_segment.Enabled = true;
            lbtn_unsegment.Enabled = true;
            btn_lock.Enabled = true;
            btn_unlock.Enabled = true;

            if (ddlist_unlocksalesorg.SelectedItem.Value == "-1")
                btn_unlock.Enabled = false;
            else
            {
                btn_unlock.Enabled = true;
            }
            LockAllUser();
        }
        //by mbq 20110509 item13 add end
    }

    protected string getRole()
    {
        return Session["Role"].ToString().Trim();
    }

    protected string getRoleID(string str_name)
    {
        string query_role = "SELECT ID FROM [Role] WHERE Name = '" + str_name + "'";
        DataSet ds_role = helper.GetDataSet(query_role);
        return ds_role.Tables[0].Rows[0][0].ToString().Trim();
    }

    protected void autoUnlock()
    {
        string year = System.DateTime.Now.Year.ToString().Trim();
        string month = System.DateTime.Now.Month.ToString().Trim(); 
        string day = System.DateTime.Now.Day.ToString().Trim();
        string del1 = "DELETE FROM [Lock] WHERE YEAR(UnlockTime) < '" + year + "'"
                    + " OR (YEAR(UnlockTime) = '" + year + "' AND MONTH(UnlockTime) < '" + month + "')"
                    + " OR (YEAR(UnlockTime) = '" + year + "' AND MONTH(UnlockTime) = '" + month + "' AND DAY(UnlockTime) <= '" + day + "')";
        
        int delcount1 = helper.ExecuteNonQuery(CommandType.Text, del1, null);
        return;

    }

    protected DataSet getSalesOrgInfo()
    {
        StringBuilder sql = new StringBuilder();
        sql.AppendLine(" SELECT ");
        sql.AppendLine("   ID, ");
        sql.AppendLine("   Abbr ");
        sql.AppendLine(" FROM ");
        sql.AppendLine("   [SalesOrg] ");
        sql.AppendLine(" WHERE ");
        sql.AppendLine("   Deleted=0 ");
        sql.AppendLine("   AND ID IN (SELECT ");
        sql.AppendLine("                [SalesOrg_User].SalesOrgID ");
        sql.AppendLine(" 			 FROM ");
        sql.AppendLine(" 			   [SalesOrg_User] ");
        sql.AppendLine(" 			 WHERE ");
        sql.AppendLine(" 			   [SalesOrg_User].UserID NOT IN (SELECT UserID FROM [Lock] where UnlockTime>=GETDATE()) ");
        sql.AppendLine(" 			   AND [SalesOrg_User].SalesOrgID=[SalesOrg].ID ");
        sql.AppendLine(" 			   AND [SalesOrg_User].Deleted=0) ");
        sql.AppendLine(" ORDER BY ");
        sql.AppendLine("   Abbr ");
        DataSet ds = helper.GetDataSet(sql.ToString());

        if (ds.Tables[0].Rows.Count > 0)
            return ds;
        else
            return null;
    }

    protected DataSet getLockSalesOrgInfo()
    {
        string sql = "SELECT [SalesOrg].ID,[SalesOrg].Abbr FROM [SalesOrg_User] "
                     + " INNER JOIN [SalesOrg] ON [SalesOrg].ID = [SalesOrg_User].SalesOrgID"
                     + " INNER JOIN [Lock] ON [Lock].UserID = [SalesOrg_User].UserID"
                     + " WHERE [SalesOrg].Deleted = 0 AND [SalesOrg_User].Deleted = 0 and [Lock].UnlockTime>=GETDATE()"
                     + " GROUP BY [SalesOrg].ID,[SalesOrg].Abbr"
                     + " ORDER BY [SalesOrg].Abbr ASC";
        DataSet ds = helper.GetDataSet(sql);

        if (ds.Tables[0].Rows.Count > 0)
            return ds;
        else
            return null;
    }

    protected DataSet getRSMBySalesOrgID(string str_salesorgID, bool lockflag)
    {
        if (lockflag)
        {
            //by mbq 20110517 item13 del start   
            /**
            string sql = "SELECT [User].UserID,[User].Alias "
                        + " FROM [User] INNER JOIN [SalesOrg_User]"
                        + " ON [User].UserID = [SalesOrg_User].UserID"
                        + " WHERE [SalesOrg_User].Deleted = 0 AND [User].Deleted = 0"
                        + " AND [SalesOrg_User].SalesOrgID = " + str_salesorgID
                        + " AND [User].UserID NOT IN (SELECT UserID FROM [Lock])"
                        + " ORDER BY [User].Alias ASC";
            **/
            //by mbq 20110517 item13 del start   

            //by mbq 20110517 item13 add start   
            string sql = "SELECT [User].UserID,[User].Alias+'('+ case when [User].RoleID = 1 then 'Executive' when [User].RoleID = 2 then 'GMM' when [User].RoleID = 3 then 'GSM' when [User].RoleID = 4 then 'RSM' when [User].RoleID = 5 then 'Assistant'end +')'  "
            + " FROM [User] INNER JOIN [SalesOrg_User]"
            + " ON [User].UserID = [SalesOrg_User].UserID"
            + " WHERE [SalesOrg_User].Deleted = 0 AND [User].Deleted = 0"
            + " AND [SalesOrg_User].SalesOrgID = " + str_salesorgID
            + " AND [User].UserID NOT IN (SELECT UserID FROM [Lock] where [Lock].UnlockTime>=GETDATE())"
            + " ORDER BY [User].Alias ASC";
            //by mbq 20110517 item13 add end
   
            DataSet ds = helper.GetDataSet(sql);

            if (ds.Tables[0].Rows.Count > 0)
                return ds;
            else
                return null;
        }
        else
        {
            //by mbq 20110517 item13 del start   
            /** 
            string sql = "SELECT [User].UserID,[User].Alias "
                        + " FROM [User] INNER JOIN [SalesOrg_User]"
                        + " ON [User].UserID = [SalesOrg_User].UserID"
                        + " WHERE [SalesOrg_User].Deleted = 0 AND [User].Deleted = 0"
                        + " AND [SalesOrg_User].SalesOrgID = " + str_salesorgID
                        + " AND [User].UserID IN (SELECT UserID FROM [Lock])"
                        + " ORDER BY [User].Alias ASC";
            **/
            //by mbq 20110517 item13 del start   

            //by mbq 20110517 item13 add start   
            string sql = "SELECT [User].UserID,[User].Alias+'('+ case when [User].RoleID = 1 then 'Executive' when [User].RoleID = 2 then 'GMM' when [User].RoleID = 3 then 'GSM' when [User].RoleID = 4 then 'RSM' when [User].RoleID = 5 then 'Assistant' end +')'  "
                        + " FROM [User] INNER JOIN [SalesOrg_User]"
                        + " ON [User].UserID = [SalesOrg_User].UserID"
                        + " WHERE [SalesOrg_User].Deleted = 0 AND [User].Deleted = 0"
                        + " AND [SalesOrg_User].SalesOrgID = " + str_salesorgID
                        + " AND [User].UserID IN (SELECT UserID FROM [Lock] where [Lock].UnlockTime>=GETDATE())"
                        + " ORDER BY [User].Alias ASC";
            //by mbq 20110517 item13 add end

            DataSet ds = helper.GetDataSet(sql);

            if (ds.Tables[0].Rows.Count > 0)
                return ds;
            else
                return null;
        }
    }

    protected DataSet getRSMAll()
    {
        string sql = "SELECT [User].UserID,[User].Alias "
                    + " FROM [User] INNER JOIN [SalesOrg_User]"
                    + " ON [User].UserID = [SalesOrg_User].UserID"
                    + " WHERE [SalesOrg_User].Deleted = 0 AND [User].Deleted = 0"
                    + " ORDER BY [User].Alias ASC";
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

    protected void bindDataSource()
    {
        string lockSalesOrg = this.ddlist_locksalesorg.SelectedValue;
        string unlockSalesOrg = this.ddlist_unlocksalesorg.SelectedValue == "-1" ? "" : this.ddlist_unlocksalesorg.SelectedValue;

        ddlist_locksalesorg.Items.Clear();
        ddlist_unlocksalesorg.Items.Clear();
        ddlist_lockrsm.Items.Clear();
        ddlist_unlockrsm.Items.Clear();

        bind(getSalesOrgInfo(), ddlist_locksalesorg);
        bind(getLockSalesOrgInfo(), ddlist_unlocksalesorg);

        this.ddlist_locksalesorg.SelectedIndex = this.ddlist_locksalesorg.Items.IndexOf(this.ddlist_locksalesorg.Items.FindByValue(lockSalesOrg));
        this.ddlist_unlocksalesorg.SelectedIndex = this.ddlist_unlocksalesorg.Items.IndexOf(this.ddlist_unlocksalesorg.Items.FindByValue(unlockSalesOrg));

        string str_locksalesID;
        string str_unlocksalesID;
        if (ddlist_locksalesorg.SelectedItem.Value == "-1")
            btn_lock.Enabled = false;
        else
        {
            str_locksalesID = ddlist_locksalesorg.SelectedItem.Value.Trim();
            bind(getRSMBySalesOrgID(str_locksalesID, true), ddlist_lockrsm);
            btn_lock.Enabled = true;
        }
        if (ddlist_unlocksalesorg.SelectedItem.Value == "-1")
            btn_unlock.Enabled = false;
        else
        {
            str_unlocksalesID = ddlist_unlocksalesorg.SelectedItem.Value.Trim();
            bind(getRSMBySalesOrgID(str_unlocksalesID, false), ddlist_unlockrsm);
            btn_unlock.Enabled = true;
            unlocknote();
        }


    }
    // by mbq 20110509 item13 add start   
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
            gridview.Width = Unit.Pixel(260);
            gridview.AutoGenerateColumns = false;
            gridview.AllowPaging = true;
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
            cf_Delete.ShowDeleteButton = false;
            cf_Delete.ShowCancelButton = true;
            cf_Delete.CausesValidation = false;
            cf_Delete.DeleteImageUrl = "~/images/del.jpg";
            cf_Delete.DeleteText = "Delete";
            gridview.Columns.Add(cf_Delete);

            gridview.AllowSorting = false;
            // by mbq 20110509 item13 del start   
            //gridview.Caption = str_caption;
            //gridview.CaptionAlign = TableCaptionAlign.Top;
            // by mbq 20110509 item13 del end  
            gridview.DataSource = ds.Tables[0];
            gridview.DataBind();
            gridview.Columns[0].Visible = false;
            gridview.Columns[gridview.Columns.Count - 1].Visible = flag;
            if (getRoleID(getRole()) != "0")
                gridview.Columns[gridview.Columns.Count - 1].Visible = false;
        }
    }
    /*by mbq 20110509 item13 add end*/
    /*by mbq 20110509 item13 add start*/
    protected DataSet getsegmentInfo()
    {
        /**
       string query_segment = " SELECT [Segment].ID,[Segment].Abbr AS 'Segment'"
                            + " FROM [User_Segment] INNER JOIN [Segment] "
                            + " ON [User_Segment].SegmentID = [Segment].ID"
                            + " WHERE [User_Segment].UserID  = '" + getUserID() + "'"
                            + " AND [Segment].Deleted = 0 AND [User_Segment].Deleted = 0 "
                            + " ORDER BY [Segment].Abbr ASC";

        
        String type = Request.Form["back_type"];
        String keys = Request.Form["back_key"];
        String[] a_keys = null;
       
        
        string strSql = "";
        if (type != null)
        {
            if (!"".Equals(type) && !"undefined".Equals(type)){
                if (keys != null && !"undefined".Equals(keys))
                { 
                    hidback_key.Value = keys;
                }
           }
                if ("".Equals(hidback_key.Value) && hidback_key.Value.IndexOf(",") == -1)
                {
                    strSql = "''";
                }
                else {
                    a_keys = hidback_key.Value.Split(',');
                    foreach (String var in a_keys)
                    {
                        strSql += "'" + var.Trim() + "',";
                    }
                    strSql = strSql.Substring(0, strSql.Length - 1);
                }
               
        }
        else {
            strSql = "''";
        }
        string query_segment = " SELECT [Segment].ID,[Segment].Abbr AS 'Segment'"
                            + " FROM [Segment] , [LockSegment]"
                            + " WHERE [Segment].Deleted = 0 AND [Segment].ID = (" + strSql + ")"
                            + " ORDER BY [Segment].Abbr ASC";
         **/
        string query_segment = " SELECT [Segment].ID,[Segment].Abbr AS 'Segment'"
                           + " FROM [Segment] , [LockSegment]"
                           + " WHERE [Segment].Deleted = 0 AND [Segment].ID = LockSegment.SegmentID and [LockSegment].UnlockTime>=GETDATE()"
                           + " ORDER BY [Segment].Abbr ASC";
        DataSet ds_segment = helper.GetDataSet(query_segment);
        return ds_segment;
    }
    /*by mbq 20110509 item13 add end*/

    /*by mbq 20110509 item13 add start*/
    protected void gv_segment_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        gv_segment.PageIndex = e.NewPageIndex;
        bindDataSource();
    }
    /*by mbq 20110509 item13 add end*/

    /*by mbq 20110509 item13 add start*/
    protected void gv_segment_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (e.Row.RowState == DataControlRowState.Normal || e.Row.RowState == DataControlRowState.Alternate)
            {
                //string a = e.Row.Cells[0].Text;
                //if ("1".Equals(a))
                //{
                //((ImageButton)e.Row.Cells[gv_segment.Columns.Count - 1].Controls[0]).Visible = false;
                    //((ImageButton)e.Row.Cells[gv_segment.Columns.Count - 1].Controls[0]).Attributes.Add("onclick", info.setRowDataBound(e.Row.Cells[1].Text.Trim()));
                //}
                //((ImageButton)e.Row.Cells[gv_segment.Columns.Count - 1].Controls[0]).Attributes.Add("onclick", info.setRowDataBound(e.Row.Cells[1].Text.Trim()));
            }
        }
    }
    /*by mbq 20110509 item13 add end*/

    /*Following is RSM_Segment Module */
    protected void gv_segment_RowDeleting(object sender, GridViewDeleteEventArgs e)///modify
    {
        /**
        label_delsegment.Visible = true;
        label_delsegment.Text = "";
        label_delsegment.ForeColor = System.Drawing.Color.Red;
        
        string segmentID = gv_segment.Rows[e.RowIndex].Cells[0].Text.Trim();
        string segment = gv_segment.Rows[e.RowIndex].Cells[1].Text.Trim();

        string delete_segment = "UPDATE [User_Segment] SET Deleted = 1 WHERE UserID = " + getUserID() + " AND SegmentID = " + segmentID;
        string delete_segment = "UPDATE [User_Segment] SET Deleted = 1";
        int count = helper.ExecuteNonQuery(CommandType.Text, delete_segment, null);

        if (count > 0)
        {
            label_delsegment.ForeColor = System.Drawing.Color.Green;
            label_delsegment.Text = info.delLabelInfo(segment, true);
        }
        else
            label_delsegment.Text = info.delLabelInfo(segment, false);
        **/
        bindDataSource();
    }
    //by mbq 20110509 item13 add end

    protected void ddlist_locksalesorg_SelectedIndexChanged(object sender, EventArgs e)
    {
        string str_locksalesID = ddlist_locksalesorg.SelectedItem.Value.Trim();
        ddlist_lockrsm.Items.Clear();
        if (ddlist_locksalesorg.SelectedItem.Value.Trim() == "-1")
        {
            ddlist_lockrsm.Enabled = false;
            btn_lock.Enabled = false;
        }
        else
        {
            ddlist_lockrsm.Enabled = true;
            btn_lock.Enabled = true;
        }
        bind(getRSMBySalesOrgID(str_locksalesID, true), ddlist_lockrsm);
    }

    protected void ddlist_unlocksalesorg_SelectedIndexChanged(object sender, EventArgs e)
    {
        string str_unlocksalesID = ddlist_unlocksalesorg.SelectedItem.Value.Trim();
        ddlist_unlockrsm.Items.Clear();
        if (ddlist_unlocksalesorg.SelectedItem.Value.Trim() == "-1")
        {
            ddlist_unlockrsm.Enabled = false;
            btn_unlock.Enabled = false;
        }
        else
        {
            ddlist_unlockrsm.Enabled = true;
            btn_unlock.Enabled = true;
        }
        bind(getRSMBySalesOrgID(str_unlocksalesID, false), ddlist_unlockrsm);
    }

    protected void cal_date_SelectionChanged(object sender, EventArgs e)
    {
        tbox_date.Text = cal_date.SelectedDate.Year.ToString() + "-" + cal_date.SelectedDate.Month.ToString() + "-" + cal_date.SelectedDate.Day.ToString();
    }
    // by mbq 20110509 item13 add start   
    protected void cal_date_LockAll_SelectionChanged(object sender, EventArgs e)
    {
        tbox_date_LockAll.Text = cal_date_LockAll.SelectedDate.Year.ToString() + "-" + cal_date_LockAll.SelectedDate.Month.ToString() + "-" + cal_date_LockAll.SelectedDate.Day.ToString();
    }
    // by mbq 20110509 item13 add end   
    protected void btn_lock_Click(object sender, EventArgs e)
    {
        label_locknote.Text = "";
        label_unlockdate.Text = "";
        lockdata();
    }

    protected void btn_unlock_Click(object sender, EventArgs e)
    {
        label_locknote.Text = "";
        label_unlockdate.Text = "";
        unlockdata();
    }

    protected void lockdata()
    {
        label_locknote.ForeColor = System.Drawing.Color.Red;
        label_locknote.Text = "";
        string str_salesID = ddlist_locksalesorg.SelectedItem.Value.Trim();
        string str_UserID = ddlist_lockrsm.SelectedItem.Value.Trim();
        string str_User = ddlist_lockrsm.SelectedItem.Text.Trim();
        string str_date = tbox_date.Text.Trim();
        if (tbox_date.Text.Trim() == "")
            str_date = "2099-12-31";
        if (str_salesID != "-1")
        {
            if (str_UserID != "-1")
            {
                string del = "DELETE FROM [Lock] WHERE UserID = " + str_UserID;
                int delcount = helper.ExecuteNonQuery(CommandType.Text, del, null);
                if (delcount >= 0)
                {
                    string ins = "INSERT INTO [Lock]  VALUES(" + str_UserID + ",'" + str_date + "')";
                    int inscount = helper.ExecuteNonQuery(CommandType.Text, ins, null);
                    if (inscount == 1)
                    {
                        label_locknote.Text += "";
                    }
                    else
                    {
                        label_locknote.Text += str_User + " was locked failly. ";
                        label_unlocknote.Text = "";
                    }
                }
                else
                {
                    label_locknote.Text += "Exception occur,please look over log file.";
                    label_unlocknote.Text = "";
                }
            }
        }

        if (label_locknote.Text == "")
        {
            label_locknote.ForeColor = System.Drawing.Color.Green;
            label_locknote.Text = "Locked successfully.";
            label_unlocknote.Text = "";
        }
        bindDataSource();
    }

    protected void unlockdata()
    {
        label_unlocknote.ForeColor = System.Drawing.Color.Red;
        string str_salesID = ddlist_unlocksalesorg.SelectedItem.Value.Trim();
        string str_UserID = ddlist_unlockrsm.SelectedItem.Value.Trim();
        string str_User = ddlist_unlockrsm.SelectedItem.Text.Trim();
        if (str_salesID != "-1")
        {
            if (str_UserID != "-1")
            {
                //string sql = "delete from User_Status where UserID=" + str_UserID + " and SegmentID not in (select SegmentID from LockSegment where UnlockTime>=GETDATE() )";
                //helper.ExecuteNonQuery(CommandType.Text, sql, null);

                //string sql1 = "delete from ActualSalesandBL_Status where MarketingMgrID=" + str_UserID + "  and "
                //    + " SegmentID not in (select SegmentID from LockSegment where UnlockTime>=GETDATE() )";
                //helper.ExecuteNonQuery(CommandType.Text, sql1, null);
                
                string del = "DELETE FROM [Lock] WHERE UserID = " + str_UserID;
                //string del = "UPDATE [Lock] set UnlockTime='"+DateTime.Now.Date.AddDays(-10).ToString()+"' WHERE UserID = " + str_UserID;
                int delcount = helper.ExecuteNonQuery(CommandType.Text, del, null);
                if (delcount == 1)
                {
                    label_unlocknote.Text += "";
                }
                else
                {
                    label_unlocknote.Text += str_User + " unlocked failly.";
                    label_locknote.Text = "";
                }

                //string strSql = "DELETE FROM [User_Status] where UserID=" + str_UserID;
                //helper.ExecuteNonQuery(CommandType.Text, strSql, null);

            }
        }

        if (label_unlocknote.Text == "")
        {
            label_unlocknote.ForeColor = System.Drawing.Color.Red;
            label_unlocknote.Text = "Unlocked successfully.";
        }
        bindDataSource();
    }

    protected DataSet getDate(string str_rsmID)
    {
        string sql = "SELECT YEAR(UnlockTime),MONTH(UnlockTime),DAY(UnlockTime) FROM [Lock] WHERE UserID = '" + str_rsmID + "'";
        DataSet ds = helper.GetDataSet(sql);

        if (ds.Tables[0].Rows.Count == 1)
            return ds;
        else
            return null;

    }

    protected void unlocknote()
    {
        label_date.Text = "";
        DataSet ds = getDate(ddlist_unlockrsm.SelectedItem.Value.Trim());
        if (ds != null)
        {
            string str_year = ds.Tables[0].Rows[0][0].ToString().Trim();
            string str_month = ds.Tables[0].Rows[0][1].ToString().Trim();
            string str_day = ds.Tables[0].Rows[0][2].ToString().Trim();

            string str_date = str_year + "-" + str_month + "-" + str_day;
            if (str_date != "2099-12-31")
            {
                label_date.Text = "This lock will expire on " + str_date + ".";
            }
            else
            {
                label_date.Text = "This lock is indefinite.";
            }
        }
    }

    protected void ddlist_unlockrsm_SelectedIndexChanged(object sender, EventArgs e)
    {
        unlocknote();
    }

    // by mbq 20110509 item13 add start   
    protected void lbtn_alluser_Click(object sender, EventArgs e)
    {
        string Delete_lockAllUser = "DELETE FROM [LockAllUser]";
        helper.ExecuteNonQuery(CommandType.Text, Delete_lockAllUser, null);
        
        DateTime dt;
        if (string.IsNullOrEmpty(tbox_date_LockAll.Text.Trim()))
            dt = new DateTime(2099, 1, 1);
        else if (!DateTime.TryParse(tbox_date_LockAll.Text.Trim(), out dt))
            return;
        string str_date = dt.ToString();
        
        //string str_date = tbox_date_LockAll.Text.Trim();

        string AdministratorID = Session["AdministratorID"].ToString().Trim();
        //string insert_lockAllUser = " INSERT INTO [LockAllUser](UserID,UnlockTime) select UserID,'" + str_date + "' AS UnlockTime from [USER] where [USER].Deleted = 0 AND [USER].UserId <> " + AdministratorID + " "
        //                          + " AND [USER].UserId NOT IN (SELECT UserId From [USER] WHERE [USER].RoleID = 1)";
        //helper.ExecuteNonQuery(CommandType.Text, insert_lockAllUser, null);


        string insert_lockAllUser = " INSERT INTO [LockAllUser](UserID,UnlockTime) select top 1 UserID,'" + str_date + "' AS UnlockTime from [USER] where [USER].Deleted = 0 AND [USER].UserId <> " + AdministratorID + " "
                                  + " AND [USER].UserId NOT IN (SELECT UserId From [USER] WHERE [USER].RoleID = 1)";
        helper.ExecuteNonQuery(CommandType.Text, insert_lockAllUser, null);

        label_lockAllUserNote.Text = "";
        label_lockAllUserNote.ForeColor = System.Drawing.Color.Green;
        label_lockAllUserNote.Text = "Locked successfully.";
        label_unlockAllUserNote.Text = "";
        LockAllUser();


        //string strSql = "DELETE FROM [User_Status] ";
        //helper.ExecuteNonQuery(CommandType.Text, strSql, null);
    }
    // by mbq 20110509 item13 add end   
    // by mbq 20110509 item13 add start   
    protected void lbtn_unalluser_Click(object sender, EventArgs e)
    {
//        string sql = "delete from User_Status where UserID not in ";
//        sql += " (select UserID from Lock where UnlockTime>=GETDATE()) ";
//        sql += " and SegmentID not in ";
//        sql += " (select SegmentID from LockSegment where UnlockTime>=GETDATE()) ";
//        helper.ExecuteNonQuery(CommandType.Text, sql, null);

//        string sql1 = "delete from ActualSalesandBL_Status where MarketingMgrID not in "
//+ "(select UserID from Lock where UnlockTime>=GETDATE()) and "
//+ " SegmentID not in "
// + " (select SegmentID from LockSegment where UnlockTime>=GETDATE())";
//        helper.ExecuteNonQuery(CommandType.Text, sql1, null);

        
        string Delete_lockAllUser = "DELETE FROM [LockAllUser]";
        helper.ExecuteNonQuery(CommandType.Text, Delete_lockAllUser, null);

        //string Delete_lockAllUser = "UPDATE [LockAllUser] set UnlockTime='"+DateTime.Now.Date.AddDays(-10).ToString()+"'";
        //helper.ExecuteNonQuery(CommandType.Text, Delete_lockAllUser, null);


        label_unlockAllUserNote.Text = "";
        label_unlockAllUserNote.ForeColor = System.Drawing.Color.Red;
        label_unlockAllUserNote.Text = "Unlocked successfully.";
        label_lockAllUserNote.Text = "";
        LockAllUser();
    }
    // by mbq 20110509 item13 add end   
    // by mbq 20110509 item13 add start   
    protected void LockAllUser()
    {
        //string sql = "SELECT * FROM [LockAllUser]";
        string sql = "SELECT * FROM [LockAllUser] where UnlockTime>=GETDATE()";
        DataSet ds = helper.GetDataSet(sql);

        if (ds.Tables[0].Rows.Count > 0)
        {
            lbtn_alluser.Enabled = false;
        }
        else
        {
            lbtn_alluser.Enabled = true;
        }
    }
    // by mbq 20110509 item13 add end   
}
