/*
 *  FileName       :  RSMProfile.aspx.cs
 * 
 *  Description    :  Scan RSM information and modify his email and telephone.
 * 
 *  Author         :  Wang Jun
 * 
 *  Modified date  :  2010.12.17
 * 
 *  Problem        : 
 * 
 *  Version        :  Release(2.0)
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
//By Wsy 20110517 Item W2 Add Start 
using System.Text;
//By Wsy 20110517 Item W2 Add End 
public partial class RSM_RsmProfile : System.Web.UI.Page
{
    SQLHelper helper = new SQLHelper();
    LogUtility log = new LogUtility();
    WebUtility webU = new WebUtility();
    DisplayInfo info = new DisplayInfo();
    SQLStatement sql = new SQLStatement();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (getRoleID(getRole()) != "4")
            Response.Redirect("!/AccessDenied.aspx");

        if (!IsPostBack)
        {
            getRSMID();
            //by yyan item w35 20110620 add start 
            getsearchIN();
            list.bindFind(list.getUserFirstName(), ddlist_find);
            //by yyan item w35 20110620 add end 
            bindDataSource();
        }
    }

    //Role-access
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

    protected string getRSMID()
    {
        return Session["RSMID"].ToString().Trim();
    }

    protected DataSet getInfo(string str_rsmID)
    {
        string queryProfile;
        //By Wsy 20110517 Item W2 DEL Start 
        /* queryProfile = "SELECT [User].UserID, [User].FirstName,[User].LastName, [User].Alias AS Login,[User].Abbr,"
                + " (case when [User].Gender = 1 Then 'Male' else 'Female' End) AS Gender, "
                + " CONVERT(varchar(15),[User].StartDate,23) AS StartDate,"
                + " CONVERT(varchar(15),[User].EndDate,23) AS EndDate,"
                + " [User].Email "
                + " FROM [User]"
                + " WHERE [User].Deleted = 0 AND [User].UserID = '" + str_rsmID + "'";*/
        //By Wsy 20110517 Item W2 DEL End 

        //By Wsy 20110517 Item W2 Add Start 
        queryProfile = "SELECT [User].UserID, [User].FirstName,[User].LastName, [User].Alias AS Login,[User].Abbr,"
                + " [Role].Name AS Role,(case when [User].Gender = 1 Then 'Female' else 'Male' End) AS Gender, "
                + " CONVERT(varchar(15),[User].StartDate,23) AS StartDate,"
                + " CONVERT(varchar(15),[User].EndDate,23) AS EndDate,"
                + " [User].Email "
                + " FROM [User] INNER JOIN [Role]"
                + " ON [User].RoleID = [Role].ID"
                + " WHERE [User].Deleted = 0 AND [User].UserID = '" + str_rsmID + "'";
        //By Wsy 20110517 Item W2 Add End 
        DataSet ds_query = helper.GetDataSet(queryProfile);
        return ds_query;
    }

    //by yyan item w35 20110620 del start 
    //protected void bindDataSource()
    //by yyan item w35 20110620 del end 
    //by yyan item w35 20110620 add start 
    protected void bindDataSource(DataSet ds)
    //by yyan item w35 20110620 add end 
    {
       
        //by yyan item 35 20110620 del start 
        // DataSet ds = getInfo(getRSMID());
        //by yyan item 35 20110620 del end
        if (ds.Tables[0].Rows.Count == 0)
        {
            log.WriteLog(LogUtility.LogErrorLevel.LOG_CRITICAL, "No User.");
            Response.Redirect("~/AccessDenied.aspx");
        }
        //By Wsy 20110518 Item W2 DEL Start 
        //gv_rsmprofile.Width = Unit.Pixel(800);
        //By Wsy 20110518 Item W2 DEL End 

        //By Wsy 20110518 Item W2 Add Start 
        gv_rsmprofile.Width = Unit.Pixel(1000);
        //By Wsy 20110518 Item W2 Add End 
        gv_rsmprofile.AutoGenerateColumns = false;
        //By lhy 20110512 Item 18 DEL Start 
        // gv_rsmprofile.AllowPaging = true;
        //By lhy 20110512 Item 18 DEL End 

        //By lhy 20110512 Item 18 Add Start 
        gv_rsmprofile.AllowPaging = false;
        //By lhy 20110512 Item 18 Add End 
        gv_rsmprofile.Visible = true;
        //By Wsy 20110517 Item W2 ADD Start 
        addOperationCol(ds);
        addCountryCol(ds);
        addSegmentCol(ds);
        //By Wsy 20110517 Item W2 ADD End 
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
            else if (i == 1 || i == 2 )
            {
                bf.ItemStyle.Width = 80;
                bf.ControlStyle.Width = bf.ItemStyle.Width;
            }
            //By Wsy 20110518 Item W2 ADD Start 
            else if (i == 9 || i == 10)
            {
                bf.ItemStyle.Width = 120;
                bf.ControlStyle.Width = bf.ItemStyle.Width;
            }
            //By Wsy 20110518 Item W2 ADD End 
            else
            {
                bf.ItemStyle.Width = 180;
                bf.ControlStyle.Width = bf.ItemStyle.Width;
            }

            gv_rsmprofile.Columns.Add(bf);
        }
        //By Wsy 20110505 ITEM 30 DEL Start

        /*CommandField cf_Update = new CommandField();
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
        gv_rsmprofile.Columns.Add(cf_Update);*/

        //By Wsy 20110505 ITEM 30 DEL End
        gv_rsmprofile.AllowSorting = true;
        gv_rsmprofile.DataSource = ds.Tables[0];
        gv_rsmprofile.DataBind();
        gv_rsmprofile.Columns[0].Visible = false;
        //By Wsy 20110517 Item W2 ADD Start 
        gv_rsmprofile.Columns[6].Visible = false;
        gv_rsmprofile.Columns[7].Visible = false;
        gv_rsmprofile.Columns[8].Visible = false;
        gv_rsmprofile.Columns[9].Visible = false;
        //By Wsy 20110517 Item W2 ADD End 
    }

    protected void gv_rsmprofile_RowEditing(object sender, GridViewEditEventArgs e)
    {
        label_editnote.Text = "";
        gv_rsmprofile.Columns.Clear();
        gv_rsmprofile.EditIndex = e.NewEditIndex;
        bindDataSource();
    }

    protected void gv_rsmprofile_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        gv_rsmprofile.Columns.Clear();
        gv_rsmprofile.EditIndex = -1;
        bindDataSource();
    }

    //Modify
    protected void gv_rsmprofile_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        string str_UserID = gv_rsmprofile.Rows[e.RowIndex].Cells[0].Text.Trim();
        string str_firstname = ((TextBox)(gv_rsmprofile.Rows[e.RowIndex].Cells[1].Controls[0])).Text.Trim();
        string str_lastname = ((TextBox)(gv_rsmprofile.Rows[e.RowIndex].Cells[2].Controls[0])).Text.Trim();
        string str_email = ((TextBox)(gv_rsmprofile.Rows[e.RowIndex].Cells[8].Controls[0])).Text.Trim();

        if (webU.checkEmail(str_email) && str_firstname.Length != 0 && str_lastname.Length != 0)
        {
            string update_user = "UPDATE [User] SET FirstName = '" + str_firstname + "',"
                             + " LastName = '" + str_lastname + "',"
                             + " Email = '" + str_email + "'"
                             + "  WHERE Deleted = 0 AND UserID = " + str_UserID;
            int count = helper.ExecuteNonQuery(CommandType.Text, update_user, null);

            if (count > 0)
            {
                label_editnote.ForeColor = System.Drawing.Color.Green;
                label_editnote.Text = info.edtLabelInfo("your info..", true);
            }
            else
            {
                label_editnote.ForeColor = System.Drawing.Color.Red;
                label_editnote.Text = info.edtLabelInfo("your info..", false);
            }
        }
        else
        {
            label_editnote.ForeColor = System.Drawing.Color.Red;
            label_editnote.Text = info.addillegal("Email:" + str_email + " or your first name and last name is null.");
        }

        gv_rsmprofile.Columns.Clear();
        gv_rsmprofile.EditIndex = -1;
        bindDataSource();
    }

    //By Wsy 20110517 Item W2 ADD Start 
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
    //By Wsy 20110517 Item W2 ADD End 

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
        gv_rsmprofile.EditIndex = -1;
        gv_rsmprofile.Columns.Clear();
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
