/*
* File Name   : ExecutiveProfile.aspx.cs
* 
* Description : Users' Information and roles
* 
* Author      : Wang Jun
* 
* Modify Date : 2010-05-28
* 
* Problem     : none
* 
* Version     : Release (1.0)
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
// By lhy 20110517 Item W2 Add Start 
using System.Web.Security;
using System.Text;
// By DingJunjie 20110517 W2 lhy01 Add End 

public partial class Executive_ExecutiveProfile : System.Web.UI.Page
{
    SQLHelper helper = new SQLHelper();
    LogUtility log = new LogUtility();
    SQLStatement sql = new SQLStatement();
    // By lhy 20110517 Item W2 Add Start 
    WebUtility webU = new WebUtility();
    DisplayInfo info = new DisplayInfo();
    // By DingJunjie 20110517 W2 lhy01 Add End 

    protected void Page_Load(object sender, EventArgs e)
    {
        Page.MaintainScrollPositionOnPostBack = true;
        if (getRoleID(getRole()) != "1")
            Response.Redirect("~/AccessDenied.aspx");

        if (!IsPostBack)
        {
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

    protected DataSet getExecutiveInfo()
    {
        string query_Exe = "SELECT [User].UserID, [User].FirstName,[User].LastName, [User].Alias AS Login,[User].Abbr,"
                    + " [Role].Name AS Role,(case when [User].Gender = 1 Then 'Male' else 'Female' End) AS Gender, "
                    + " CONVERT(varchar(15),[User].StartDate,23) AS StartDate,"
                    + " CONVERT(varchar(15),[User].EndDate,23) AS EndDate,"
                    + " [User].Email "
                    + " FROM [User] INNER JOIN [Role]"
                    + " ON [User].RoleID = [Role].ID"
                    + " WHERE [User].Deleted = 0 AND [User].RoleID > 0"
                    + " ORDER By [Role].Name, [User].Alias ASC ";
        DataSet ds_query = helper.GetDataSet(query_Exe);

        return ds_query;
    }

    //by yyan item w35 20110620 del start 
    //protected void bindDataSource()
    //by yyan item w35 20110620 del end 
    //by yyan item w35 20110620 add start 
    protected void bindDataSource(DataSet ds_Executive)
    //by yyan item w35 20110620 add end 
    {
        //by yyan item 35 20110620 del start 
        //DataSet ds_Executive = getExecutiveInfo();
        //by yyan item 35 20110620 del end
        if (ds_Executive.Tables[0].Rows.Count == 0)
        {
            sql.getNullDataSet(ds_Executive);
        }
        gv_executive.Width = Unit.Pixel(1000);
        gv_executive.AutoGenerateColumns = false;
        gv_executive.AllowPaging = false;
        gv_executive.Visible = true;
        //By lhy 20110517 Item W2 Add Start 
        addOperationCol(ds_Executive);
        addCountryCol(ds_Executive);
        addSegmentCol(ds_Executive);
        //By lhy 20110517 Item W2 Add End
        for (int i = 0; i < ds_Executive.Tables[0].Columns.Count; i++)
        {
            BoundField bf = new BoundField();

            bf.DataField = ds_Executive.Tables[0].Columns[i].ColumnName.ToString();
            bf.HeaderText = ds_Executive.Tables[0].Columns[i].Caption.ToString();
            bf.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
            bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;

            bf.ReadOnly = false;
            if (i <= 6 && i != 4)
            {
                bf.ReadOnly = true;
            }
            else if (i == 4)
            {
                bf.ItemStyle.Width = 80;
                bf.ControlStyle.Width = bf.ItemStyle.Width;
            }
            else if (i == 7 || i == 8)
            {
                bf.ItemStyle.Width = 100;
                bf.ControlStyle.Width = bf.ItemStyle.Width;
            }
            else
            {
                bf.ItemStyle.Width = 200;
                bf.ControlStyle.Width = bf.ItemStyle.Width;
            }
            gv_executive.Columns.Add(bf);
        }

        gv_executive.AllowSorting = true;
        gv_executive.DataSource = ds_Executive.Tables[0];
        gv_executive.DataBind();
        gv_executive.Columns[0].Visible = false;
        //By lhy 20110517 ITEM W2 Add Start
        gv_executive.Columns[6].Visible = false;
        gv_executive.Columns[7].Visible = false;
        gv_executive.Columns[8].Visible = false;
        gv_executive.Columns[9].Visible = false;
        //By lhy 20110517 ITEM W2 Add End
    }

    protected void gv_executive_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        gv_executive.Columns.Clear();
        gv_executive.PageIndex = e.NewPageIndex;
        bindDataSource();
    }

    //By lhy 20110517 ITEM W2 Add Start

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

    ////By lhy 20110517 ITEM W2 Add Start

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
        gv_executive.EditIndex = -1;
        gv_executive.Columns.Clear();
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
