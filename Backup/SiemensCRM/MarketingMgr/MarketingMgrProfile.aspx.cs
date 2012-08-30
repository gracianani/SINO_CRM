/*++

Module Name:

    MarketingMgrProfile.aspx.cs

Abstract:
    
    Look at the information by one's own and modify it

Author:

    Wang Jun 2010-05-28


Revision : Release (1.0)

--*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;

// By daixuesong 20110517 Item ? Add Start 
using System.Text;
// By daixuesong 20110517 Item ? Add End 

public partial class MarketingMgr_MarketMgrProfile : System.Web.UI.Page
{
    SQLHelper helper = new SQLHelper();
    LogUtility log = new LogUtility();
    WebUtility webU = new WebUtility();
    DisplayInfo info = new DisplayInfo();
    SQLStatement sql = new SQLStatement();

    protected void Page_Load(object sender, EventArgs e)
    {
        Page.MaintainScrollPositionOnPostBack = true;
        string roleid = getRoleID(getRole());
        if (roleid != "2")
        {
            log.WriteLog(LogUtility.LogErrorLevel.LOG_CRITICAL, "The web page is not visible to the role whose identity is '" + roleid + "';");
            Response.Redirect("~/AccessDenied.aspx");
        }

        if (!IsPostBack)
        {
            log.WriteLog(LogUtility.LogErrorLevel.LOG_ACCESS, "Enter general marketing mgr profile.");
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

        if (ds_role != null)
        {
            for (int i = 0; i < ds_role.Tables[0].Rows.Count; i++)
            {
                if (ds_role.Tables[0].Rows[i][0].ToString().Trim() == str_name)
                {
                    log.WriteLog(LogUtility.LogErrorLevel.LOG_NOISE, "'" + str_name + "' is defined in the table name 'Role', it's role identity is '" + ds_role.Tables[0].Rows[i][1].ToString().Trim() + "';");
                    return ds_role.Tables[0].Rows[i][1].ToString().Trim();
                }
            }
        }
        log.WriteLog(LogUtility.LogErrorLevel.LOG_CRITICAL, "'" + str_name + "' is not defined in the table name 'Role';");
        return "";
    }

    private string getMarketingMgrID()
    {
        return Session["GeneralMarketingMgrID"].ToString().Trim();
    }

    protected DataSet getInfo()
    {
        // by daixuesong 20110517 del start
        //string queryProfile = "SELECT [User].UserID, [User].FirstName,[User].LastName, [User].Alias AS Login,[User].Abbr,"
        //        + " (case when [User].Gender = 1 Then 'Male' else 'Female' End) AS Gender, "
        //        + " CONVERT(varchar(15),[User].StartDate,23) AS StartDate,"
        //        + " CONVERT(varchar(15),[User].EndDate,23) AS EndDate,"
        //        + " [User].Email "
        //        + " FROM [User]"
        //        + " WHERE [User].Deleted = 0 AND [User].UserID = '" + getMarketingMgrID().Trim() + "'";
        // by daixuesong 20110517 del end
        // by daixuesong 20110517 add start
        // By DingJunjie 20110523 Item W8 Delete Start
        //        string queryProfile = @"SELECT [User].UserID, [User].FirstName,[User].LastName, 
        //                                [User].Alias AS Login,[User].Abbr, [Role].Name  
        //                                AS Role,(case when [User].Gender = 1 Then 'Male' else 'Female' End) AS Gender,  
        //                                CONVERT(varchar(15),[User].StartDate,23) AS StartDate, 
        //                                CONVERT(varchar(15),[User].EndDate,23) AS EndDate, 
        //                                [User].Email  
        //                                FROM [User],[Role] WHERE [User].Deleted = 0 AND [User].UserID = '180'
        //                                 and [User].RoleID = [Role].ID ";
        // By DingJunjie 20110523 Item W8 Delete End
        // By DingJunjie 20110523 Item W8 Add Start
        string queryProfile = @"SELECT [User].UserID, [User].FirstName,[User].LastName, 
                                [User].Alias AS Login,[User].Abbr, [Role].Name  
                                AS Role,(case when [User].Gender = 1 Then 'Male' else 'Female' End) AS Gender,  
                                CONVERT(varchar(15),[User].StartDate,23) AS StartDate, 
                                CONVERT(varchar(15),[User].EndDate,23) AS EndDate, 
                                [User].Email  
                                FROM [User],[Role] WHERE [User].Deleted = 0 AND [User].UserID = '" + getMarketingMgrID().Trim() + @"'
                                 and [User].RoleID = [Role].ID ";
        // By DingJunjie 20110523 Item W8 Add End
        // by daixuesong 20110517 add end
        
        DataSet ds_query = helper.GetDataSet(queryProfile);

        if (ds_query.Tables.Count > 0)
            return ds_query;
        else
            return null;
    }

    //by yyan item w35 20110620 del start 
    //protected void bindDataSource()
    //by yyan item w35 20110620 del end 
    //by yyan item w35 20110620 add start 
    protected void bindDataSource(DataSet ds)
    //by yyan item w35 20110620 add end 
    {
        
        //by yyan item 35 20110620 del start 
        //DataSet ds = getInfo();
        //by yyan item 35 20110620 del end
        if (ds != null)
        {
            // by daixuesong 20110517 del start
            //gv_profile.Width = Unit.Pixel(800);
            // by daixuesong 20110517 del end
            // by daixuesong 20110517 add start
            gv_profile.Width = Unit.Pixel(1100);
            // by daixuesong 20110517 add end
            gv_profile.AutoGenerateColumns = false;
            gv_profile.Visible = true;
            // by daixuesong 20110517 add start
            //add columns
            addOperationCol(ds);
            addCountryCol(ds);
            addSegmentCol(ds);
            // by daixuesong 20110517 add end

            for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
            {
                BoundField bf = new BoundField();

                bf.DataField = ds.Tables[0].Columns[i].ColumnName.ToString();
                bf.HeaderText = ds.Tables[0].Columns[i].Caption.ToString();

                bf.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
                bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;

                bf.ReadOnly = false;
                if (i == 0 || i == 3 || i == 4 || i == 5 || i == 6 || i == 7)
                    bf.ReadOnly = true;
                else if (i == 1 || i == 2)
                {
                    bf.ItemStyle.Width = 80;
                    bf.ControlStyle.Width = bf.ItemStyle.Width;
                }
                else
                {
                    bf.ItemStyle.Width = 180;
                    bf.ControlStyle.Width = bf.ItemStyle.Width;
                }

                gv_profile.Columns.Add(bf);
            }
           //By Wsy 20110505 ITEM 30 DEL Start

           /* CommandField cf_Update = new CommandField();
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
            gv_profile.Columns.Add(cf_Update);*/

           //By Wsy 20110505 ITEM 30 DEL End
            gv_profile.AllowSorting = true;
            gv_profile.DataSource = ds.Tables[0];
            gv_profile.DataBind();
            gv_profile.Columns[0].Visible = false;
            // by daixuesong 20110517 add start
            gv_profile.Columns[6].Visible = false;
            gv_profile.Columns[7].Visible = false;
            gv_profile.Columns[8].Visible = false;
            gv_profile.Columns[9].Visible = false;
            // by daixuesong 20110517 add end
        }
        else
            Response.Redirect("~/AccessDenied.aspx");
    }

    protected void gv_profile_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        label_edt_del.Visible = true;
        label_edt_del.ForeColor = System.Drawing.Color.Red;

        string str_UserID = gv_profile.Rows[e.RowIndex].Cells[0].Text;
        string str_Alias = gv_profile.Rows[e.RowIndex].Cells[3].Text;
        string str_firstname = ((TextBox)(gv_profile.Rows[e.RowIndex].Cells[1].Controls[0])).Text.Trim();
        string str_lastname = ((TextBox)(gv_profile.Rows[e.RowIndex].Cells[2].Controls[0])).Text.Trim();
        string str_email = ((TextBox)(gv_profile.Rows[e.RowIndex].Cells[8].Controls[0])).Text.Trim();

        if (webU.checkEmail(str_email) && str_firstname.Length != 0 && str_lastname.Length != 0)
        {
            string update_user = "UPDATE [User] SET FirstName = '" + str_firstname + "',"
                             + " LastName = '" + str_lastname + "',"
                             + " Email = '" + str_email + "'"
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
            label_edt_del.Text = info.addillegal("Email:" + str_email + " or your first name and last name is null.");

        gv_profile.Columns.Clear();
        gv_profile.EditIndex = -1;
        bindDataSource();
    }

    protected void gv_profile_RowEditing(object sender, GridViewEditEventArgs e)
    {
        gv_profile.Columns.Clear();
        label_edt_del.Text = "";
        gv_profile.EditIndex = e.NewEditIndex;
        bindDataSource();
    }

    protected void gv_profile_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        gv_profile.Columns.Clear();
        gv_profile.EditIndex = -1;
        bindDataSource();
    }

    // by daixuesong 20110517 add start
    private void addOperationCol(DataSet ds)
    {
        ds.Tables[0].Columns.Add("Operation");
        if (ds.Tables[0].Rows[0][0] != DBNull.Value)
        {
            // By DingJunjie 20110516 Item 2 Delete Start
            //foreach (DataRow dr in ds.Tables[0].Rows)
            //{
            //    string userid = dr[0].ToString().Trim();
            //    DataSet ds_operation = sql.getOperationByUser(userid);
            //    if (ds_operation == null && ds_operation.Tables[0].Rows.Count == 0)
            //    {
            //        dr["Operation"] = "";
            //    }
            //    else
            //    {
            //        int index = 0;
            //        string temp = "";
            //        for (; index < ds_operation.Tables[0].Rows.Count; index++)
            //        {
            //            temp += ds_operation.Tables[0].Rows[index][0].ToString().Trim() + ",";
            //            if (index % 4 == 0 && index > 0)
            //                temp += " ";
            //        }
            //        dr["Operation"] = temp;
            //    }
            //}
            // By DingJunjie 20110516 Item 2 Delete End
            // By DingJunjie 20110516 Item 2 Add Start
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
            // By DingJunjie 20110516 Item 2 Add End
        }
    }

    private void addSegmentCol(DataSet ds)
    {
        ds.Tables[0].Columns.Add("Segment");
        if (ds.Tables[0].Rows[0][0] != DBNull.Value)
        {
            // By DingJunjie 20110516 Item 2 Delete Start 
            //foreach (DataRow dr in ds.Tables[0].Rows)
            //{
            //    string userid = dr[0].ToString().Trim();
            //    DataSet ds_segment = sql.getSegmentByUser(userid);
            //    if (ds_segment == null && ds_segment.Tables[0].Rows.Count == 0)
            //    {
            //        dr["Segment"] = "";
            //    }
            //    else
            //    {
            //        int index = 0;
            //        string temp = "";
            //        for (; index < ds_segment.Tables[0].Rows.Count; index++)
            //        {
            //            temp += ds_segment.Tables[0].Rows[index][0].ToString().Trim() + ",";
            //            if (index % 4 == 0 && index > 0)
            //                temp += " ";
            //        }
            //        dr["Segment"] = temp;
            //    }
            //}
            // By DingJunjie 20110516 Item 2 Delete End
            // By DingJunjie 20110516 Item 2 Add Start 
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
            // By DingJunjie 20110516 Item 2 Add End
        }
    }

    private void addCountryCol(DataSet ds)
    {
        ds.Tables[0].Columns.Add("SubRegion");
        if (ds.Tables[0].Rows[0][0] != DBNull.Value)
        {
            // By DingJunjie 20110516 Item 2 Delete Start 
            //foreach (DataRow dr in ds.Tables[0].Rows)
            //{
            //    string userid = dr[0].ToString().Trim();
            //    DataSet ds_country = sql.getCountryByUser(userid);
            //    if (ds_country == null && ds_country.Tables[0].Rows.Count == 0)
            //    {
            //        dr["SubRegion"] = "";
            //    }
            //    else
            //    {
            //        int index = 0;
            //        string temp = "";
            //        for (; index < ds_country.Tables[0].Rows.Count; index++)
            //        {
            //            temp += ds_country.Tables[0].Rows[index][0].ToString().Trim() + ",";
            //            if (index % 4 == 0 && index > 0)
            //                temp += " ";
            //        }
            //        dr["SubRegion"] = temp;
            //    }
            //}
            // By DingJunjie 20110516 Item 2 Delete End
            // By DingJunjie 20110516 Item 2 Add Start 
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
            // By DingJunjie 20110516 Item 2 Add End
        }
    }

    #region DingJunjie Add
    // By DingJunjie 20110516 Item 2 Add Start 
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
    // By DingJunjie 20110516 Item 2 Add End 
    #endregion
    // by daixuesong 20110517 add end

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
        gv_profile.EditIndex = -1;
        gv_profile.Columns.Clear();
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
