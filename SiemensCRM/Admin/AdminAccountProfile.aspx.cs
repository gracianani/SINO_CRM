/*
* File Name   : AdminAcountProfile.aspx.cs
* 
* Description : Add,modify and delete users' Information
*               If modify alias, only delete the user ,and then add him
* 
* Author      : Wang Jun
* 
* Modified Date : 2010-12-09
* 
* Problem     : none
* 
* Version     : Release (2.0)
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Web.Security;
// By DingJunjie 20110516 Item 2 Add Start 
using System.Text;
// By DingJunjie 20110516 Item 2 Add End 

public partial class AdminAccount : System.Web.UI.Page
{
    SQLHelper helper = new SQLHelper();
    LogUtility log = new LogUtility();
    SQLStatement sql = new SQLStatement();
    WebUtility webU = new WebUtility();
    DisplayInfo info = new DisplayInfo();

    protected void Page_Load(object sender, EventArgs e)
    {
        Page.MaintainScrollPositionOnPostBack = true;
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
            log.WriteLog(LogUtility.LogErrorLevel.LOG_ACCESS, "AdminAccountProfile Access.");
            panel_add.Visible = false;
            label_add.Text = "";
            label_edt_del.Text = "";
            getsearchIN();
            list.bindFind(list.getUserFirstName(), ddlist_find);
            tbox_startdate.Text = System.DateTime.Now.Year.ToString() + "-" + System.DateTime.Now.Month.ToString() + "-" + System.DateTime.Now.Day.ToString();
            tbox_enddate.Text = System.DateTime.Now.Year.ToString() + "-" + System.DateTime.Now.Month.ToString() + "-" + System.DateTime.Now.Day.ToString();
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

    protected void lbtn_findhelp_Click(object sender, EventArgs e)
    {
        string str_args = "'AdminHelp.aspx'" + ",'Help', 'height=500,width=800,top=100,left=200,toolbar=no,status=no, menubar=no,location=no,resizable=no,scrollbars=yes'";
        Response.Write("<script   language='javascript'>window.open(" + str_args + ");</script>");
    }

    /* Bind dropdownlist of role */
    protected void bindRole()
    {
        DataSet ds_role = sql.getRole();

        if (ds_role.Tables[0].Rows.Count > 0)
        {
            DataTable dt_role = ds_role.Tables[0];
            int countrole = dt_role.Rows.Count;
            int indexrole = 0;
            while (indexrole < countrole)
            {
                ListItem li = new ListItem(dt_role.Rows[indexrole][0].ToString().Trim(), dt_role.Rows[indexrole][1].ToString().Trim());
                ddlist_Role.Items.Add(li);
                indexrole++;
            }
            ddlist_Role.Enabled = true;
            btn_AddUser.Enabled = true;
        }
        else
        {
            ddlist_Role.Enabled = false;
            btn_AddUser.Enabled = false;
        }
    }

    /* Bind dropdownlist of Gender */
    protected void getGendar()
    {
        ddlist_gendar.Items.Add(new ListItem("Male", "1"));
        ddlist_gendar.Items.Add(new ListItem("Female", "0"));
    }

    /* Clear input information of some control */
    protected void null_input()
    {
        tbox_Alias.Text = "";
        ddlist_Role.Items.Clear();
        ddlist_gendar.Items.Clear();
    }

    /// <summary>
    ///  Judge whether or not the user has existed
    /// </summary>
    /// <param name="user">User alias</param>
    /// <param name="roleID">User roleID</param>
    /// <param name="gender">User gender</param>
    /// <returns>Having existed,return false; if not ,return true</returns>
    protected bool Exist_User(string str_user, string str_roleID, string str_gender)
    {
        string query_user = "SELECT UserID FROM [User] WHERE Alias = '" + str_user + "' AND RoleID = " + str_roleID + " AND Gender = " + str_gender + " AND Deleted = 0";
        DataSet ds_user = helper.GetDataSet(query_user);

        if (ds_user.Tables[0].Rows.Count > 0)
            return false;
        else
            return true;
    }

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
            // By DingJunjie 20110516 Item 2 Add End
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
            // By DingJunjie 20110516 Item 2 Add End
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
            // By DingJunjie 20110516 Item 2 Add End
        }
    }

    /// <summary>
    ///  Bind user information
    /// </summary>
    /// <param name="ds">dataset</param>
    protected void bindDataSource(DataSet ds)
    {
        if (ds.Tables[0].Rows.Count == 0)
        {
            sql.getNullDataSet(ds);
        }
        //gv_administrator.Width = Unit.Pixel(1000);
        gv_administrator.AutoGenerateColumns = false;
        //By FXW 20110509 ITEM  ADD Start
        gv_administrator.AllowPaging = false;
        //By FXW 20110509 ITEM  ADD End
        //By FXW 20110509 ITEM  DEL Start
        //gv_administrator.AllowPaging = true;
        //By FXW 20110509 ITEM  DEL End
        gv_administrator.Visible = true;

        //add columns
        addOperationCol(ds);
        addCountryCol(ds);
        addSegmentCol(ds);

        for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
        {
            BoundField bf = new BoundField();

            bf.DataField = ds.Tables[0].Columns[i].ColumnName.ToString();
            bf.HeaderText = ds.Tables[0].Columns[i].Caption.ToString();
            bf.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
            bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;

            bf.ReadOnly = false;
            if (i <= 6 && i != 1 && i != 2 && i != 3 && i != 4 || (i == 10 || i == 12 || i == 11))
            {
                bf.ReadOnly = true;
            }
            else if (i == 4)
            {
                //By FXW 20110511 ITEM18  DEL Start
                //bf.ItemStyle.Width = 80;
                //bf.ItemStyle.Width = 30;
                //bf.ControlStyle.Width = bf.ItemStyle.Width;
                //By FXW 20110511 ITEM18  DEL End

            }
            else if (i == 7 || i == 8)
            {
                //By FXW 20110511 ITEM18  DEL Start
                //bf.ItemStyle.Width = 100;
                //bf.ControlStyle.Width = bf.ItemStyle.Width;
                //By FXW 20110511 ITEM18  DEL End
            }
            else
            {
                //By FXW 20110511 ITEM18  DEL Start
                //bf.ItemStyle.Width = 200;
                //bf.ControlStyle.Width = bf.ItemStyle.Width;
                //By FXW 20110511 ITEM18  DEL End
            }
            gv_administrator.Columns.Add(bf);
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
        gv_administrator.Columns.Add(cf_Update);

        CommandField cf_Delete = new CommandField();
        cf_Delete.ButtonType = ButtonType.Image;
        cf_Delete.ShowDeleteButton = true;
        cf_Delete.ShowCancelButton = true;
        cf_Delete.CausesValidation = false;
        cf_Delete.DeleteImageUrl = "~/images/del.jpg";
        cf_Delete.DeleteText = "Delete";
        gv_administrator.Columns.Add(cf_Delete);

        gv_administrator.AllowSorting = true;
        gv_administrator.DataSource = ds.Tables[0];
        gv_administrator.DataBind();
        gv_administrator.Columns[0].Visible = false;
        gv_administrator.Columns[6].Visible = false;
        gv_administrator.Columns[7].Visible = false;
        gv_administrator.Columns[8].Visible = false;
        gv_administrator.Columns[9].Visible = false;
        if (getRoleID(getRole()) != "0")
        {
            gv_administrator.Columns[gv_administrator.Columns.Count - 1].Visible = false;
            gv_administrator.Columns[gv_administrator.Columns.Count - 2].Visible = false;
        }
    }

    protected void bindDataSource()
    {
        gv_administrator.Columns.Clear();
        string str = ddlist_find.Text.Trim();
        int sel = int.Parse(ddlist_in.SelectedItem.Value.Trim());
        DataSet ds;
        ds = sql.getAdministratorInfo(str, sel);
        //By FXW 20110509 ITEM  ADD Start
        //By FXW 20110511 ITEM18  DEL Start
        //if (ds.Tables[0].Rows.Count < 15)
        //{
        //    this.divflow.Attributes.Add("Style", " overflow:scroll; width: 810px;  ");
        //}
        //else
        //{
        //    this.divflow.Attributes.Add("Style", " overflow:scroll; width: 810px;  height:500px ");
        //}
        //By FXW 20110511 ITEM18  DEL END
        //By FXW 20110509 ITEM  ADD End
        bindDataSource(ds);
    }

    //the following is some functions about add
    protected void lbtn_addUser_click(object sender, EventArgs e)
    {
        lbtn_addUser.Text = "Input user loginName and select roleName and Gendar :";
        lbtn_addUser.Enabled = false;
        ddlist_gendar.Items.Clear();
        ddlist_Role.Items.Clear();
        getGendar();
        bindRole();

        panel_add.Visible = true;

        label_edt_del.Text = "";
        label_add.Text = "";
        //by yyan item W32 20110609 del start 
        //gv_administrator.Columns.Clear();
        //by yyan item W32 20110609 del end
    }

    protected void btn_AddUser_Click(object sender, EventArgs e)
    {
        //By FXW 20110516 ITEM18  ADD Start
        label_edt_del.Text = "";
        label_add.Text = "";
        //By FXW 20110516 ITEM18  ADD End
        lbtn_addUser.Text = "Add new user";
        lbtn_addUser.Enabled = true;
        panel_add.Visible = false;

        string str_firstname = tbox_firstName.Text.Trim();
        string str_lastname = tbox_lastName.Text.Trim();
        string str_user = tbox_Alias.Text.Trim();
        string str_abbr = tbox_abbr.Text.Trim();
        string str_role = ddlist_Role.SelectedItem.Value.Trim();
        string str_startdate = tbox_startdate.Text.Trim();
        string str_enddate = tbox_enddate.Text.Trim();
        string str_gender = ddlist_gendar.Text.Trim();
        //By FXW 20110516 ITEM18 ADD Start
        null_input();
        //By FXW 20110516 ITEM18 ADD End
        addUser(str_firstname, str_lastname, str_user, str_abbr, str_role, str_startdate, str_enddate, str_gender);
        //By FXW 20110516 ITEM18 DEL Start
        //null_input();
        //By FXW 20110516 ITEM18 DEL End
        gv_administrator.Columns.Clear();
        bindDataSource();
    }

    /// <summary>
    ///  the function provide a interface of adding user
    /// </summary>
    /// <param name="str_alias"> User Alias</param>
    /// <param name="str_abbr">User Abbr</param>
    /// <param name="str_role">User roleID</param>
    /// <param name="str_startDate">User date of starting work</param>
    /// <param name="str_gender">User gender</param>
    private void addUser(string firstname, string lastname, string str_alias, string str_abbr, string str_role, string str_startDate, string endDate, string str_gender)
    {
        label_add.ForeColor = System.Drawing.Color.Red;
        if (str_alias != "" && webU.CheckDate(str_startDate) && str_abbr != "")
        {
            int gender = (str_gender == "Female" ? 1 : 0);
            if (Exist_User(str_alias, str_role, gender.ToString()))
            {
                if (existAbbr(str_abbr, null, str_role))
                {
                    string str_passwordmd5 = FormsAuthentication.HashPasswordForStoringInConfigFile("123456", "MD5");
                    string insert_user = "INSERT INTO [User](FirstName, LastName, Alias,Abbr,Password,RoleID,StartDate,EndDate, Gender,Deleted) VALUES('" + firstname + "','" + lastname + "','" + str_alias + "','" + str_abbr + "','" + str_passwordmd5 + "'," + str_role + ",'" + str_startDate + "','" + endDate + "'," + gender + ",0)";
                    int count = helper.ExecuteNonQuery(CommandType.Text, insert_user, null);

                    if (count == 1)
                    {
                        //By FXW 20110516 ITEM 18 ADD Start
                        this.tbox_firstName.Text = "";
                        this.tbox_lastName.Text = "";
                        this.tbox_Alias.Text = "";
                        this.tbox_abbr.Text = "";
                        this.tbox_startdate.Text = "";
                        this.tbox_enddate.Text = "";
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
                        //By FXW 20110516 ITEM 18 ADD End
                        label_add.ForeColor = System.Drawing.Color.Green;
                        label_add.Text = info.addLabelInfo(str_alias, true);
                    }
                    else
                        label_add.Text = info.addLabelInfo(str_alias, false);
                }
                else
                {
                    label_add.Text = info.addExist("Abbr(" + str_abbr + ")");
                }
            }
            else
            {
                label_add.Text = info.addExist("In the role," + str_alias);
            }
        }
        else
        {
            label_add.Text = info.addillegal("Login:" + str_alias + ", Abbr:" + str_abbr + ", StartDate:" + str_startDate);
        }
    }

    //cancel adding
    protected void btn_CancelUser_Click(object sender, EventArgs e)
    {
        //By FXW 20110516 ITEM18  ADD Start
        label_edt_del.Text = "";
        label_add.Text = "";
        //By FXW 20110516 ITEM18  ADD End
        lbtn_addUser.Text = "Add new user";
        lbtn_addUser.Enabled = true;
        panel_add.Visible = false;

        gv_administrator.Columns.Clear();
        bindDataSource();
    }

    protected void gv_administrator_RowEditing(object sender, GridViewEditEventArgs e)
    {
        //by yyan item W32 20110609 add start 
        panel_readonly.Visible = true;
        panel_add.Visible = false;
        lbtn_addUser.Enabled = false;
        lbtn_addUser.Text = "Add new user";
        //by yyan item W32 20110609 add end
        gv_administrator.Columns.Clear();
        gv_administrator.EditIndex = e.NewEditIndex;
        label_edt_del.Text = "";
        label_add.Text = "";

        bindDataSource();
    }

    protected void gv_administrator_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        //By FXW 20110516 ITEM18  ADD Start
        label_edt_del.Text = "";
        label_add.Text = "";
        //By FXW 20110516 ITEM18  ADD End
        string str_UserID = gv_administrator.Rows[e.RowIndex].Cells[0].Text.Trim();
        string str_Alias = gv_administrator.Rows[e.RowIndex].Cells[3].Text.Trim();
        string str_Role = gv_administrator.Rows[e.RowIndex].Cells[5].Text.Trim();
        string str_RoleID = getRoleID(getRoleName(str_Role));

        delUser(str_UserID, str_RoleID, str_Alias, str_Role);

        gv_administrator.Columns.Clear();
        bindDataSource();
    }

    /// <summary>
    ///  the function provide a interface of deleting user, after deleting user,the user is no use, but exist
    /// </summary>
    /// <param name="str_UserID">User UserID</param>
    /// <param name="str_RoleID">User RoleID</param>
    /// <param name="str_Alias">User Alias</param>
    /// <param name="str_Role">User Role</param>
    private void delUser(string str_UserID, string str_RoleID, string str_Alias, string str_Role)
    {
        label_edt_del.ForeColor = System.Drawing.Color.Red;
        string del_user_role = "UPDATE [User] SET Deleted = 1 WHERE UserID = " + str_UserID + " AND RoleID = " + str_RoleID;
        int count = helper.ExecuteNonQuery(CommandType.Text, del_user_role, null);

        if (count > 0)
        {
            //By FXW 20110516 ITEM18 ADD Start
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
            //By FXW 20110516 ITEM18 ADD End
            label_edt_del.ForeColor = System.Drawing.Color.Green;
            label_edt_del.Text = info.delLabelInfo(str_Role, str_Alias, true);
        }
        else
            label_edt_del.Text = info.delLabelInfo(str_Role, str_Alias, false);
    }

    protected void gv_administrator_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        label_edt_del.ForeColor = System.Drawing.Color.Red;
        string str_UserID = gv_administrator.Rows[e.RowIndex].Cells[0].Text;
        //By FXW 20110516 ITEM18 UPDATE Start
        //string str_Alias = gv_administrator.Rows[e.RowIndex].Cells[3].Controls[0].Text;
        string str_Alias = ((TextBox)(gv_administrator.Rows[e.RowIndex].Cells[3].Controls[0])).Text;
        //By FXW 20110516 ITEM18 UPDATE End
        //By Mbq 20110504 ITEM 1 DEL Start 
        //string str_role = ((TextBox)(gv_administrator.Rows[e.RowIndex].Cells[3].Controls[0])).Text.Trim();
        //By Mbq 20110504 ITEM 1 DEL End

        //By Mbq 20110504 ITEM 1 ADD Start 
        string str_RoleID = hidrole.Value;
        //By Mbq 20110504 ITEM 1 ADD End

        string str_abbr = ((TextBox)(gv_administrator.Rows[e.RowIndex].Cells[4].Controls[0])).Text.Trim();
        string str_startDate = ((TextBox)(gv_administrator.Rows[e.RowIndex].Cells[7].Controls[0])).Text.Trim();
        string str_endDate = ((TextBox)(gv_administrator.Rows[e.RowIndex].Cells[8].Controls[0])).Text.Trim();
        string str_email = ((TextBox)(gv_administrator.Rows[e.RowIndex].Cells[9].Controls[0])).Text.Trim();

        //By Mbq 20110504 ITEM 1 ADD Start
        string str_FirstName = ((TextBox)(gv_administrator.Rows[e.RowIndex].Cells[1].Controls[0])).Text.Trim();
        string str_LastName = ((TextBox)(gv_administrator.Rows[e.RowIndex].Cells[2].Controls[0])).Text.Trim();
        string str_Login = ((TextBox)(gv_administrator.Rows[e.RowIndex].Cells[3].Controls[0])).Text.Trim();
        //By Mbq 20110504 ITEM 1 ADD End

        if (str_endDate == "" || str_endDate == null)
        {
            str_endDate = "2099-12-31";
        }

        if (str_startDate == "" || str_startDate == null)
        {
            str_startDate = tbox_startdate.Text.Trim();
        }

        updateUser(str_UserID, str_RoleID, str_Alias, str_abbr, str_startDate, str_endDate, str_email, str_FirstName, str_LastName, str_Login);

        gv_administrator.Columns.Clear();
        gv_administrator.EditIndex = -1;
        bindDataSource();
        //by yyan item W32 20110609 add start 
        lbtn_addUser.Enabled = true;
        panel_readonly.Enabled = true;
        //by yyan item W32 20110609 add end
    }

    private string getRoleName(string str_name)
    {
        if (str_name == "GSM")
            str_name = "General Sales Manager";
        if (str_name == "GMM")
            str_name = "General Maketing Manager";
        string sql_searchRole = "SELECT Name FROM [Role] WHERE Name like '%" + str_name + "%'";
        DataSet ds_role = helper.GetDataSet(sql_searchRole);

        if (ds_role.Tables[0].Rows.Count == 1)
            return ds_role.Tables[0].Rows[0][0].ToString().Trim();
        else
            return "";
    }

    private void updateUser(string str_UserID, string str_RoleID, string str_Alias, string str_abbr, string str_startDate, string str_endDate, string str_email, string str_FirstName, string str_LastName, string str_Login )
    {
        label_edt_del.ForeColor = System.Drawing.Color.Red;
        if (webU.checkEmail(str_email) && webU.CheckDate(str_endDate) && webU.CheckDate(str_startDate))
        {
            
            if (existAbbr(str_abbr, str_UserID, str_RoleID))
            {
                //By Mbq 20110504 ITEM 1 ADD Start 
                string update_user = "UPDATE [User] SET Abbr = '" + str_abbr + "',"
                                 + "  RoleID = '" + str_RoleID + "'"
                                 + ", FirstName = '" + str_FirstName + "'"
                                 + ", LastName = '" + str_LastName + "'"
                                 + ", Alias = '" + str_Login + "'"
                                 + ", StartDate = '" + str_startDate + "'"
                                 + ", EndDate = '" + str_endDate + "'"
                                 + " ,Email = '" + str_email + "'"
                                 + "  WHERE Deleted = 0 AND UserID = " + str_UserID;
                //By Mbq 20110504 ITEM 1 ADD End 

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
            {
                label_edt_del.Text = info.addExist("Abbr(" + str_abbr + ")");
            }
        }
        else
            label_edt_del.Text = info.addillegal("StartDate: " + str_startDate
                                + ",EndDate:" + str_endDate + ",Email:" + str_email + "),");
    }

    protected void gv_administrator_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        gv_administrator.Columns.Clear();
        gv_administrator.EditIndex = -1;
        bindDataSource();
        //by yyan item W32 20110609 add start 
        panel_readonly.Visible = true;
        lbtn_addUser.Enabled = true;
        //by yyan item W32 20110609 add end
    }

    protected void gv_administrator_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        gv_administrator.Columns.Clear();
        gv_administrator.PageIndex = e.NewPageIndex;
        bindDataSource();
    }

    protected void gv_administrator_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (e.Row.RowState == DataControlRowState.Normal || e.Row.RowState == DataControlRowState.Alternate)
            {
                ((ImageButton)e.Row.Cells[gv_administrator.Columns.Count - 1].Controls[0]).Attributes.Add("onclick", info.setRowDataBound(e.Row.Cells[5].Text, e.Row.Cells[3].Text));
            }
            if (e.Row.RowIndex == gv_administrator.EditIndex)
            {
                string text = e.Row.Cells[5].Text;
                DropDownList ddList = new DropDownList();

                    /* Bind dropdownlist of role */
                    ddList.ID = "ddl123";
                    ddList.Attributes.Add("onchange", "listchange(this,'" + hidrole.ClientID + "')");
                    // ddList.AutoPostBack = true;
                    bind(getAllRole(), ddList);
                    // DropDownList默认值
                    for (int i = 0; i < ddList.Items.Count; i++)
                    {
                        if (ddList.Items[i].Text == text.Trim())
                        {
                            ddList.SelectedIndex = i;
                            hidrole.Value = ddList.SelectedValue;
                            break;
                        }
 
                   }
                e.Row.Cells[5].Controls.Add(ddList);
            }
        }
    }

    protected void btn_find_Click(object sender, EventArgs e)
    {
        //by yyan item W32 20110609 add start 
        gv_administrator.EditIndex = -1;
        panel_readonly.Visible = true;
        panel_add.Visible = false;
        lbtn_addUser.Enabled = true;
        lbtn_addUser.Text = "Add new user";
        //by yyan item W32 20110609 add end 
        //By FXW 20110516 ITEM18  ADD Start
        label_edt_del.Text = "";
        label_add.Text = "";
        //By FXW 20110516 ITEM18  ADD End
        gv_administrator.Columns.Clear();
        string str_content = ddlist_find.Text.Trim();
        int sel = int.Parse(ddlist_in.SelectedItem.Value.Trim());
        DataSet ds;
        ds = sql.getAdministratorInfo(str_content, sel);
               

        bindDataSource(ds);
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

    //By Mbq 20110504 ITEM 1 ADD Start 
    protected DataSet getAllRole()
    {
        DataSet ds_role = sql.getRole();

        if (ds_role.Tables[0].Rows.Count > 0)
        {
            return ds_role;
        }
        else
        {
            return null;
        }
    }
    //By Mbq 20110504 ITEM 1 ADD End

    //By Mbq 20110504 ITEM 1 ADD Start 
    protected void bind(DataSet ds, DropDownList ddlist)
    {
        if (ds != null)
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
            ddlist.SelectedIndex = 0;
            ddlist.Enabled = true;
        }
        else
        {
            ddlist.Enabled = false;
        }
    }
    //By Mbq 20110504 ITEM 1 ADD End

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

    /// <summary>
    ///  Judge whether or not the user abbreviation has existed
    /// </summary>
    /// <param name="abbr">User abbreviation</param>
    /// <param name="userID">User ID</param>
    /// <returns>Having existed,return false;if not,return true.</returns>
    protected bool existAbbr(string abbr, string userID, string str_role)
    {
        //yyan itemw125 20110829 del start
        //string query_user = "SELECT UserID FROM [User] WHERE Abbr='" + abbr + "'" + (string.IsNullOrEmpty(userID) ? string.Empty : " AND UserID<>" + userID) + " AND Deleted=0";
        //yyan itemw125 20110829 del end
        //yyan itemw125 20110829 add start
        string query_user = "SELECT UserID FROM [User] WHERE roleid ='" + str_role + "'and Abbr='" + abbr + "'" + (string.IsNullOrEmpty(userID) ? string.Empty : " AND UserID<>" + userID) + " AND Deleted=0";
        //yyan itemw125 20110829 add end

        DataSet ds_user = helper.GetDataSet(query_user);
        if (ds_user.Tables[0].Rows.Count>0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    #endregion
}
