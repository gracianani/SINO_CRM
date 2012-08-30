/*
 *  FileName       :  SalesOrgMgrAccountProfile.master.cs
 * 
 *  Description    :  Sales Org Manager can manage the profile of his staff.
 * 
 *  Author         :  Wang Jun
 * 
 *  Modified date  :  2010-11-02
 * 
 *  Problem        :  none
 * 
 *  Version        : Release (1.0)
 */

using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;
using System.Collections.Generic;
// By SJ 20110517 Item w2 Add Start 
using System.Text;
// By SJ 20110517 Item w2 Add End 

public partial class SalesOrgMgr_SalesOrgMgrAccount : System.Web.UI.Page
{
    SQLHelper helper = new SQLHelper();
    LogUtility log = new LogUtility();
    WebUtility webU = new WebUtility();
    DisplayInfo info = new DisplayInfo();
    SQLStatement sql = new SQLStatement();

    protected void Page_Load(object sender, EventArgs e)
    {
        Page.MaintainScrollPositionOnPostBack = true;
        //By Wsy 20110505 ITEM 30 ADD Start 
        if (getRoleID(getRole()) == "3")
        {
            lbtn_addUser.Visible = false;
        }
        //By Wsy 20110505 ITEM 30 ADD End 
        if (getRoleID(getRole()) != "3")
            Response.Redirect("~/AccessDenied.aspx");

        if (!IsPostBack)
        {
            panel_add.Visible = false;
            label_add.Visible = false;
            label_edt_del.Visible = false;
            tbox_startdate.Text = System.DateTime.Now.Year.ToString() + "-" + System.DateTime.Now.Month.ToString() + "-" + System.DateTime.Now.Day.ToString();
            //by yyan item w35 20110620 add start 
            getsearchIN();
            list.bindFind(list.getUserFirstName(), ddlist_find);
            //by yyan item w35 20110620 add end 
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
        //By Wsy 20110510 ITEM 30 DEL Start 
        //if (ds.Tables[0].Rows.Count == 1)
        //By Wsy 20110510 ITEM 30 DEL End 

        //By Wsy 20110510 ITEM 30 ADD Start 
          if (ds.Tables[0].Rows.Count != 0)
        //By Wsy 20110510 ITEM 30 ADD End 
            return ds.Tables[0].Rows[0][0].ToString().Trim();
        else
        {
            Response.Redirect("~/AccessDenied.aspx");
            return null;
        }
    }

    protected bool Exist_SalesUser(string userID, string salesorgID)
    {
        string query_user = "SELECT UserID FROM [SalesOrg_User]"
                        + " WHERE [SalesOrg_User].UserID = '" + userID + "'"
                        + " AND [SalesOrg_User].SalesOrgID = " + salesorgID
                        + " AND [SalesOrg_User].Deleted = 0";
        DataSet ds_user = helper.GetDataSet(query_user);

        if (ds_user.Tables[0].Rows.Count > 0)
            return true;
        else
            return false;
    }

    protected bool Exist_User(string user, string roleID, string gender)
    {
        string query_user = "SELECT [User].UserID FROM [User]"
                        + " WHERE [User].Alias = '" + user + "' AND [User].RoleID = " + roleID
                        + " AND [User].Gender = " + gender + " AND [User].Deleted = 0";
        DataSet ds_user = helper.GetDataSet(query_user);

        if (ds_user.Tables[0].Rows.Count > 0)
            return true;
        else
            return false;
    }

    protected void getGendar()
    {
        ddlist_gendar.Items.Add("Male");
        ddlist_gendar.Items.Add("Female");
    }
    //By Wsy 20110510 ITEM 30 DEL Start 
    /*protected DataSet getSalesOrgMgrInfo(string str_salesorgID)
    {
        string query_salesOrgMgr;
        query_salesOrgMgr = "SELECT [User].UserID, [User].FirstName,[User].LastName, [User].Alias AS Login,[User].Abbr,"
                + " [Role].Name AS Role,(case when [User].Gender = 1 Then 'Male' else 'Female' End) AS Gender, "
                + " CONVERT(varchar(15),[User].StartDate,23) AS StartDate,"
                + " CONVERT(varchar(15),[User].EndDate,23) AS EndDate,"
                + " [User].Email "
                + " FROM [User] INNER JOIN [Role]"
                + " ON [User].RoleID = [Role].ID"
                + " INNER JOIN [SalesOrg_User]"
                + " ON [SalesOrg_User].UserID = [User].UserID"
                + " WHERE [User].Deleted = 0 AND [SalesOrg_User].Deleted = 0"
                + " AND [SalesOrg_User].SalesOrgID = " + str_salesorgID
                + " ORDER By [User].RoleID,[User].Alias ASC ";
        DataSet ds_query_salesOrgMgr = helper.GetDataSet(query_salesOrgMgr);
        return ds_query_salesOrgMgr;
    }*/
    //By Wsy 20110510 ITEM 30 DEL End 

     //By Wsy 20110510 ITEM 30 ADD Start 
      protected DataSet getSalesOrgMgrInfo(string str_salesorgID, string str_managerID)
    {
        string query_salesOrgMgr;
        query_salesOrgMgr = " SELECT [User].UserID, [User].FirstName,[User].LastName," 
                            +" [User].Alias AS Login,[User].Abbr, [Role].Name "
                            +" AS Role,(case when [User].Gender = 1"
                            +" Then 'Male' else 'Female' End) AS Gender,"  
                            +" CONVERT(varchar(15),[User].StartDate,23) AS StartDate," 
                            +" CONVERT(varchar(15),[User].EndDate,23) AS EndDate, [User].Email" 
                            +" FROM [User] INNER JOIN [Role] ON [User].RoleID = [Role].ID" 
                            +" INNER JOIN [SalesOrg_User] ON [SalesOrg_User].UserID = [User].UserID" 
                            +" WHERE [User].Deleted = 0 AND [SalesOrg_User].Deleted = 0"
                            + "AND [SalesOrg_User].SalesOrgID = " + str_salesorgID + " AND [User].UserID = " + str_managerID + ""
                            +" UNION SELECT [User].UserID, [User].FirstName,[User].LastName," 
                            +" [User].Alias AS Login,[User].Abbr, [Role].Name" 
                            +" AS Role,(case when [User].Gender = 1 Then 'Male' else 'Female' End) AS Gender,"  
                            +" CONVERT(varchar(15),[User].StartDate,23) AS StartDate," 
                            +" CONVERT(varchar(15),[User].EndDate,23) AS EndDate, [User].Email" 
                            +" FROM [User], [Role], [SalesOrg_User],[User_Country],[Country]"
                            +" WHERE [User].RoleID = [Role].ID  AND [SalesOrg_User].UserID = [User].UserID" 
                            +" AND [User].Deleted = 0 AND [SalesOrg_User].Deleted = 0" 
                            +" AND [Country].ID = [User_Country].CountryID AND [User_Country].UserID = [User].UserID" 
                            +" AND [Country].Name IN(SELECT [Country].Name FROM [Country] WHERE" 
                            +" Deleted=0 AND [Country].ID IN(SELECT DISTINCT [User_Country].CountryID FROM [User_Country] WHERE"
                            +" Deleted=0 AND [User_Country].UserID = " + str_managerID + ")) AND [User].RoleID = 4";
        DataSet ds_query_salesOrgMgr = helper.GetDataSet(query_salesOrgMgr);
        return ds_query_salesOrgMgr;
    }
    //By Wsy 20110510 ITEM 30 ADD End 

    protected void null_input()
    {
        tbox_Alias.Text = "";
        ddlist_gendar.Items.Clear();
    }

    //by yyan item w35 20110620 del start 
    //protected void bindDataSource()
    //by yyan item w35 20110620 del end 
    //by yyan item w35 20110620 add start 
    protected void bindDataSource(DataSet ds_salesOrgMgr)
    //by yyan item w35 20110620 add end 
    {
        //By Wsy 20110505 ITEM 30 DEL Start 
        //bool bflag = true;
        //By Wsy 20110505 ITEM 30 DEL End 

        //By Wsy 20110505 ITEM 30 ADD Start 
        bool bflag = false;
        //By Wsy 20110505 ITEM 30 ADD Start 
        string str_managerID = getManagerID();
        string str_salesorgID = getSalesOrgID(str_managerID);
        //By Wsy 20110510 ITEM 30 DEL Start 
        //DataSet ds_salesOrgMgr = getSalesOrgMgrInfo(str_salesorgID);
        //By Wsy 20110510 ITEM 30 DEL Start 

        //By Wsy 20110510 ITEM 30 ADD Start 
        //by yyan item 35 20110620 del start 
        //DataSet ds_salesOrgMgr = getSalesOrgMgrInfo(str_salesorgID, str_managerID);
        //by yyan item 35 20110620 del end
        //By Wsy 20110510 ITEM 30 ADD End 
        if (ds_salesOrgMgr.Tables[0].Rows.Count == 0)
        {
            bflag = false;
            sql.getNullDataSet(ds_salesOrgMgr);
        }
        //By SJ 20110517 ITEM w2 Update Start
        //gv_salesOrgMgr.Width = Unit.Pixel(800);
        gv_salesOrgMgr.Width = Unit.Pixel(1000);
        //By SJ 20110517 ITEM w2 Update End
        gv_salesOrgMgr.AutoGenerateColumns = false;
        //By Lhy 20110512 ITEM 18 Update Start
        //gv_salesOrgMgr.AllowPaging = true;
        gv_salesOrgMgr.AllowPaging = false;
        //By Lhy 20110512 ITEM 18 Update End
        gv_salesOrgMgr.Visible = true;
        //By SJ 20110517 ITEM w2 ADD Start
        addOperationCol(ds_salesOrgMgr);
        addCountryCol(ds_salesOrgMgr);
        addSegmentCol(ds_salesOrgMgr);
        //By SJ 20110517 ITEM w2 ADD End

        for (int i = 0; i < ds_salesOrgMgr.Tables[0].Columns.Count; i++)
        {
            BoundField bf = new BoundField();
            bf.DataField = ds_salesOrgMgr.Tables[0].Columns[i].ColumnName.ToString();
            bf.HeaderText = ds_salesOrgMgr.Tables[0].Columns[i].Caption.ToString();
            bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
            bf.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
            bf.ReadOnly = false;
            if (i == 0 || i == 3 || i == 4 || i == 5 || i == 6 )
                bf.ReadOnly = true;
            else if (i == 1 || i == 2 || i == 7 || i == 8)
            {
                bf.ItemStyle.Width = 80;
                bf.ControlStyle.Width = bf.ItemStyle.Width;
            }
            else
            {
                bf.ItemStyle.Width = 180;
                bf.ControlStyle.Width = bf.ItemStyle.Width;
            }
            gv_salesOrgMgr.Columns.Add(bf);
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
        gv_salesOrgMgr.Columns.Add(cf_Update);

        CommandField cf_Delete = new CommandField();
        cf_Delete.ButtonType = ButtonType.Image;
        cf_Delete.ShowDeleteButton = true;
        cf_Delete.ShowCancelButton = true;
        cf_Delete.CausesValidation = false;
        cf_Delete.DeleteImageUrl = "~/images/del.jpg";
        cf_Delete.DeleteText = "Delete";
        gv_salesOrgMgr.Columns.Add(cf_Delete);

        gv_salesOrgMgr.AllowSorting = true;
        gv_salesOrgMgr.DataSource = ds_salesOrgMgr.Tables[0];
        gv_salesOrgMgr.DataBind();
        gv_salesOrgMgr.Columns[0].Visible = false;
        //By SJ 20110517 ITEM w2 ADD Start
        gv_salesOrgMgr.Columns[6].Visible = false;
        gv_salesOrgMgr.Columns[7].Visible = false;
        gv_salesOrgMgr.Columns[8].Visible = false;
        gv_salesOrgMgr.Columns[9].Visible = false;
        //By SJ 20110517 ITEM w2 ADD End
        gv_salesOrgMgr.Columns[gv_salesOrgMgr.Columns.Count - 1].Visible = bflag;
        gv_salesOrgMgr.Columns[gv_salesOrgMgr.Columns.Count - 2].Visible = bflag;
    }


    protected void lbtn_addUser_click(object sender, EventArgs e)
    {
        lbtn_addUser.Text = "Input loginName and select Gendar :";
        lbtn_addUser.Enabled = false;
        getGendar();
        panel_add.Visible = true;

        label_edt_del.Visible = false;
        label_add.Visible = false;
        label_edt_del.Text = "";
        label_add.Text = "";

        gv_salesOrgMgr.Columns.Clear();
        bindDataSource();
    }

    protected void btn_AddUser_Click(object sender, EventArgs e)
    {
        lbtn_addUser.Text = "Add new user";
        lbtn_addUser.Enabled = true;
        panel_add.Visible = false;
        label_add.Visible = true;
        label_add.ForeColor = System.Drawing.Color.Red;

        string str_salesorgId = getSalesOrgID(getManagerID());
        string str_user = tbox_Alias.Text.Trim();
        string str_abbr = tbox_Abbr.Text.Trim();
        string str_startdate = tbox_startdate.Text.Trim();
        string str_gender = ddlist_gendar.Text.Trim();

        if (str_user != "" && webU.CheckDate(str_startdate))
        {
            int gender = (str_gender == "Female" ? 0 : 1);
            if (!Exist_User(str_user, "4", gender.ToString()))
            {
                string str_passwordmd5 = FormsAuthentication.HashPasswordForStoringInConfigFile("123456", "MD5");
                string insert_user = "INSERT INTO [User](Alias,Abbr,Password,RoleID,StartDate,Gender,Deleted) VALUES('" + str_user + "','" + str_abbr + "','" + str_passwordmd5 + "'," + "4" + ",'" + str_startdate + "'," + gender + ",0)";
                int count = helper.ExecuteNonQuery(CommandType.Text, insert_user, null);

                if (count == 1)
                {
                    string sql = "SELECT UserID FROM [User] WHERE Alias = '" + str_user + "' AND RoleID = 4 AND Deleted = 0 AND Gender = " + gender.ToString();
                    DataSet ds = helper.GetDataSet(sql);
                    string str_rsmID = ds.Tables[0].Rows[0][0].ToString().Trim();
                    if (!Exist_SalesUser(str_rsmID, str_salesorgId))
                    {
                        string insert_related = "INSERT INTO [SalesOrg_User]  VALUES('" + str_salesorgId + "'," + str_rsmID + ",0)";
                        int count1 = helper.ExecuteNonQuery(CommandType.Text, insert_related, null);
                        if (count1 == 1)
                        {
                            label_add.ForeColor = System.Drawing.Color.Green;
                            label_add.Text = info.addLabelInfo(str_user, true);
                        }
                        else
                        {
                            label_add.Text = info.addLabelInfo(str_user, false);
                            helper.ExecuteNonQuery(CommandType.Text, "DELETE FROM [User] WHERE [UserID] = " + str_rsmID, null);
                        }
                    }
                    else
                        label_add.Text = info.addExist(str_user);
                }
                else
                    label_add.Text = info.addLabelInfo(str_user, false);
            }
            else
            {
                string sql = "SELECT UserID FROM [User] WHERE Alias = '" + str_user + "'";
                DataSet ds = helper.GetDataSet(sql);
                string str_rsmID = ds.Tables[0].Rows[0][0].ToString().Trim();
                if (!Exist_SalesUser(str_rsmID, str_salesorgId))
                {
                    string insert_related = "INSERT INTO [SalesOrg_User]  VALUES('" + str_salesorgId + "'," + str_rsmID + ",0)";
                    int count1 = helper.ExecuteNonQuery(CommandType.Text, insert_related, null);
                    if (count1 == 1)
                    {
                        label_add.ForeColor = System.Drawing.Color.Green;
                        label_add.Text = info.addLabelInfo(str_user, true);
                    }
                    else
                    {
                        label_add.Text = info.addLabelInfo(str_user, false);
                        helper.ExecuteNonQuery(CommandType.Text, "DELETE FROM [User] WHERE [UserID] = " + str_rsmID, null);
                    }
                }
                else
                    label_add.Text = info.addExist(str_user);
            }
        }
        else
        {
            label_add.Text = info.addillegal();
        }
        null_input();
        gv_salesOrgMgr.Columns.Clear();
        bindDataSource();


    }
    protected void gv_salesOrgMgr_RowEditing(object sender, GridViewEditEventArgs e)
    {
        label_edt_del.Visible = false;
        label_add.Visible = false;
        label_edt_del.Text = "";
        label_add.Text = "";
        gv_salesOrgMgr.Columns.Clear();
        gv_salesOrgMgr.EditIndex = e.NewEditIndex;
        bindDataSource();
    }

    protected void gv_salesOrgMgr_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        label_edt_del.Visible = true;
        label_add.Visible = false;

        string str_ID = gv_salesOrgMgr.Rows[e.RowIndex].Cells[0].Text.ToString().Trim();
        string str_Alias = gv_salesOrgMgr.Rows[e.RowIndex].Cells[3].Text.ToString().Trim();
        string str_salesorgID = getSalesOrgID(getManagerID());

        string del_RSM = "UPDATE [SalesOrg_User] SET Deleted = 1 WHERE UserID = " + str_ID + " AND SalesOrgID = " + str_salesorgID;
        int count = helper.ExecuteNonQuery(CommandType.Text, del_RSM, null);

        if (count > 0)
        {
            label_edt_del.ForeColor = System.Drawing.Color.Green;
            label_edt_del.Text = info.delLabelInfo(str_Alias, true);
        }
        else
        {
            label_edt_del.ForeColor = System.Drawing.Color.Red;
            label_edt_del.Text = info.delLabelInfo(str_Alias, false);
        }

        gv_salesOrgMgr.Columns.Clear();
        bindDataSource();
    }

    protected void gv_salesOrgMgr_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        label_edt_del.Visible = true;
        label_add.Visible = false;
        label_edt_del.ForeColor = System.Drawing.Color.Red;

        string str_UserID = gv_salesOrgMgr.Rows[e.RowIndex].Cells[0].Text;
        string str_Alias = gv_salesOrgMgr.Rows[e.RowIndex].Cells[3].Text;
        string str_firstname = ((TextBox)(gv_salesOrgMgr.Rows[e.RowIndex].Cells[1].Controls[0])).Text.Trim();
        string str_lastname = ((TextBox)(gv_salesOrgMgr.Rows[e.RowIndex].Cells[2].Controls[0])).Text.Trim();
        string str_startDate = ((TextBox)(gv_salesOrgMgr.Rows[e.RowIndex].Cells[7].Controls[0])).Text.Trim();
        string str_endDate = ((TextBox)(gv_salesOrgMgr.Rows[e.RowIndex].Cells[8].Controls[0])).Text.Trim();
        string str_email = ((TextBox)(gv_salesOrgMgr.Rows[e.RowIndex].Cells[9].Controls[0])).Text.Trim();

        if (str_endDate.Trim() == "")
            str_endDate = "2099-12-31";

        if (webU.checkEmail(str_email) && webU.CheckDate(str_endDate))
        {
            string update_user = "UPDATE [User] SET StartDate = '" + str_startDate + "'"
                             + " ,EndDate = '" + str_endDate + "'"
                             + " ,Email = '" + str_email + "'"
                             + " ,FirstName = '" + str_firstname + "'"
                             + " ,LastName = '" + str_lastname + "'"
                             + "  WHERE Deleted = 0 AND UserID = " + str_UserID;
            int count = helper.ExecuteNonQuery(CommandType.Text, update_user, null);

            if (count > 0)
            {
                label_edt_del.ForeColor = System.Drawing.Color.Green;
                label_edt_del.Text = info.edtLabelInfo(str_Alias, true);
            }
            else
                label_edt_del.Text = info.edtLabelInfo(str_Alias, false);
        }
        else
            label_edt_del.Text = "(EndDate:" + str_endDate + ",Email:" + str_email + ")," + info.addillegal();

        gv_salesOrgMgr.Columns.Clear();
        gv_salesOrgMgr.EditIndex = -1;

        bindDataSource();
    }

    protected void gv_salesOrgMgr_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        gv_salesOrgMgr.Columns.Clear();
        gv_salesOrgMgr.EditIndex = -1;
        bindDataSource();
    }

    protected void gv_salesOrgMgr_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        gv_salesOrgMgr.Columns.Clear();
        gv_salesOrgMgr.PageIndex = e.NewPageIndex;
        bindDataSource();
    }

    protected void gv_salesOrgMgr_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (e.Row.RowState == DataControlRowState.Normal || e.Row.RowState == DataControlRowState.Alternate)
            {
                ((ImageButton)e.Row.Cells[gv_salesOrgMgr.Columns.Count - 1].Controls[0]).Attributes.Add("onclick", info.setRowDataBound(e.Row.Cells[5].Text, e.Row.Cells[3].Text));
            }
        }
    }

    protected void btn_CanUser_Click(object sender, EventArgs e)
    {
        lbtn_addUser.Text = "Add new user";
        lbtn_addUser.Enabled = true;
        panel_add.Visible = false;

        null_input();
        gv_salesOrgMgr.Columns.Clear();
        bindDataSource();
    }

    //By SJ 20110517 ITEM w2 ADD Start 
    private void addOperationCol(DataSet ds)
    {
        ds.Tables[0].Columns.Add("Operation");
        if (ds.Tables[0].Rows[0][0] != DBNull.Value)
        {
            Dictionary<string, string> map = GetUserOperations();
            if (ds.Tables[0].Rows[0][0] != DBNull.Value)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    if (map.ContainsKey(dr[0].ToString().Trim()))
                    {
                        dr["Operation"] = map[dr[0].ToString().Trim()];
                    }
                }
            }
        }
    }

    private void addSegmentCol(DataSet ds)
    {
        ds.Tables[0].Columns.Add("Segment");
        if (ds.Tables[0].Rows[0][0] != DBNull.Value)
        {
            Dictionary<string, string> map = GetUserSegments();
            if (ds.Tables[0].Rows[0][0] != DBNull.Value)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    if (map.ContainsKey(dr[0].ToString().Trim()))
                    {
                        dr["Segment"] = map[dr[0].ToString().Trim()];
                    }
                }
            }
        }
    }

    private void addCountryCol(DataSet ds)
    {
        ds.Tables[0].Columns.Add("SubRegion");
        if (ds.Tables[0].Rows[0][0] != DBNull.Value)
        {
            Dictionary<string, string> map = GetUserSubRegion();
            if (ds.Tables[0].Rows[0][0] != DBNull.Value)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    if (map.ContainsKey(dr[0].ToString().Trim()))
                    {
                        dr["SubRegion"] = map[dr[0].ToString().Trim()];
                    }
                }
            }
        }
    }

    /// <summary>
    /// Get User Operaton
    /// </summary>
    /// <returns>User Operaton</returns>
    private Dictionary<string, string> GetUserOperations()
    {
        Dictionary<string, string> map = new Dictionary<string, string>();
        DataSet ds = sql.getAllUserOperation();
        string userID = "";
        StringBuilder operations = null;
        DataRow[] rows = null;
        if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        {
            string count = sql.getAllOperationCount();
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                if (!string.Equals(userID, ds.Tables[0].Rows[i]["UserID"].ToString()))
                {
                    userID = ds.Tables[0].Rows[i]["UserID"].ToString();
                    rows = ds.Tables[0].Select("UserID=" + userID);
                    operations = new StringBuilder();
                    if (string.Equals(rows.Length.ToString(), count))
                    {
                        map.Add(userID, "All");
                    }
                    else
                    {
                        for (int j = 0; j < rows.Length; j++)
                        {
                            operations.Append(rows[j]["AbbrL"].ToString().Trim()).Append(",");
                            if (j % 4 == 0 && j > 0)
                            {
                                operations.Append(" ");
                            }
                        }
                        map.Add(userID, operations.ToString().Trim(','));
                    }
                }
            }
        }
        return map;
    }

    /// <summary>
    /// Get User Segment
    /// </summary>
    /// <returns>User Segment</returns>
    private Dictionary<string, string> GetUserSegments()
    {
        Dictionary<string, string> map = new Dictionary<string, string>();
        DataSet ds = sql.getAllUserSegment();
        string userID = "";
        StringBuilder segments = null;
        DataRow[] rows = null;
        if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        {
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                if (!string.Equals(userID, ds.Tables[0].Rows[i]["UserID"].ToString()))
                {
                    userID = ds.Tables[0].Rows[i]["UserID"].ToString();
                    rows = ds.Tables[0].Select("UserID=" + userID);
                    segments = new StringBuilder();
                    for (int j = 0; j < rows.Length; j++)
                    {
                        segments.Append(rows[j]["Abbr"].ToString().Trim()).Append(",");
                        if (j % 4 == 0 && j > 0)
                        {
                            segments.Append(" ");
                        }
                    }
                    map.Add(userID, segments.ToString().Trim(','));
                }
            }
        }
        return map;
    }

    /// <summary>
    /// Get User Country
    /// </summary>
    /// <returns>User Country</returns>
    private Dictionary<string, string> GetUserSubRegion()
    {
        Dictionary<string, string> map = new Dictionary<string, string>();
        DataSet ds = sql.getAllUserSubRegion();
        string userID = "";
        StringBuilder subRegions = null;
        DataRow[] rows = null;
        if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        {
            string count = sql.getAllSubRegionCount();
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                if (!string.Equals(userID, ds.Tables[0].Rows[i]["UserID"].ToString()))
                {
                    userID = ds.Tables[0].Rows[i]["UserID"].ToString();
                    rows = ds.Tables[0].Select("UserID=" + userID);
                    subRegions = new StringBuilder();
                    if (string.Equals(rows.Length.ToString(), count))
                    {
                        map.Add(userID, "All");
                    }
                    else
                    {
                        for (int j = 0; j < rows.Length; j++)
                        {
                            subRegions.Append(rows[j]["Name"].ToString().Trim()).Append(",");
                            if (j % 4 == 0 && j > 0)
                            {
                                subRegions.Append(" ");
                            }
                        }
                        map.Add(userID, subRegions.ToString().Trim(','));
                    }
                }
            }
        }
        return map;
    }
    //By SJ 20110517 ITEM w2 ADD End

    //by yyan item w35 20110620 add start 
    FindList list = new FindList();

    protected void ddlist_in_SelectedIndexChanged(object sender, EventArgs e)
    {
        int sel = int.Parse(ddlist_in.SelectedItem.Value.Trim());
        ddlist_find.Items.Clear();
        switch (sel)
        {
            case 0:
                {
                    list.bindFind(list.getUserFirstName(), ddlist_find);
                    break;
                }
            case 1:
                {
                    list.bindFind(list.getUserLastName(), ddlist_find);
                    break;
                }
            case 2:
                {
                    list.bindFind(list.getUserAlias(), ddlist_find);
                    break;
                }
            case 3:
                {
                    list.bindFind(list.getUserAbbr(), ddlist_find);
                    break;
                }
            case 4:
                {
                    list.bindFind(list.getRole(), ddlist_find);
                    break;
                }
        }
    }
    protected void btn_find_Click(object sender, EventArgs e)
    {
        gv_salesOrgMgr.EditIndex = -1;
        gv_salesOrgMgr.Columns.Clear();
        string str_content = ddlist_find.Text.Trim();
        int sel = int.Parse(ddlist_in.SelectedItem.Value.Trim());
        DataSet ds;
        ds = sql.getAdministratorInfo(str_content, sel);
        bindDataSource(ds);
    }

    protected void bindDataSource()
    {
        DataSet ds;
        ds = sql.getAdministratorInfo("", 0);
        bindDataSource(ds);
    }

    /* Get  search options */
    private void getsearchIN()
    {
        ddlist_in.Width = 100;
        ddlist_in.Items.Add(new ListItem("First Name", "0"));
        ddlist_in.Items.Add(new ListItem("Last Name", "1"));
        ddlist_in.Items.Add(new ListItem("Login", "2"));
        ddlist_in.Items.Add(new ListItem("Abbr", "3"));
        ddlist_in.Items.Add(new ListItem("Role", "4"));
    }
    //by yyan item w35 20110620 add end 
}
