/*
* File Name   : AdminUserRelation.aspx.cs
* 
* Description : display user relation
* 
* Author      : Wang Jun
* 
* Modified Date : 2010-12-09
* 
* Problem     : 
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

public partial class AdminRelation : System.Web.UI.Page
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
        }
        else if (getRoleID(getRole()) == "5")
        {
        }
        else
        {
            Response.Redirect("~/AccessDenied.aspx");
        }

        if (!IsPostBack)
        {
            log.WriteLog(LogUtility.LogErrorLevel.LOG_ACCESS, "AdminAccountProfile Access.");
            getsearchIN();
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
        ddlist_in.Items.Add(new ListItem("Login", "0"));
        ddlist_in.Items.Add(new ListItem("Abbr", "1"));
        ddlist_in.Items.Add(new ListItem("Role", "2"));
        ddlist_in.Items.Add(new ListItem("All", "-1"));
    }

    protected void lbtn_findhelp_Click(object sender, EventArgs e)
    {
        string str_args = "'AdminHelp.aspx'" + ",'Help', 'height=500,width=800,top=100,left=200,toolbar=no,status=no, menubar=no,location=no,resizable=no,scrollbars=yes'";
        Response.Write("<script   language='javascript'>window.open(" + str_args + ");</script>");
    }

    private void addOperationCol(DataSet ds)
    {
        ds.Tables[0].Columns.Add("Operation");
        if (ds.Tables[0].Rows[0][0] != DBNull.Value)
        {
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                string userid = dr[0].ToString().Trim();
                DataSet ds_operation = sql.getOperationByUser(userid);
                if (ds_operation == null && ds_operation.Tables[0].Rows.Count == 0)
                {
                    dr["Operation"] = "";
                }
                else
                {
                    int index = 0;
                    string temp = "";
                    for (; index < ds_operation.Tables[0].Rows.Count; index++)
                    {
                        temp += ds_operation.Tables[0].Rows[index][0].ToString().Trim() + ",";
                        if (index % 4 == 0 && index > 0)
                            temp += " ";
                    }
                    dr["Operation"] = temp;
                }
            }
        }
    }

    private void addSegmentCol(DataSet ds)
    {
        ds.Tables[0].Columns.Add("Segment");
        if (ds.Tables[0].Rows[0][0] != DBNull.Value)
        {
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                string userid = dr[0].ToString().Trim();
                DataSet ds_segment = sql.getSegmentByUser(userid);
                if (ds_segment == null && ds_segment.Tables[0].Rows.Count == 0)
                {
                    dr["Segment"] = "";
                }
                else
                {
                    int index = 0;
                    string temp = "";
                    for (; index < ds_segment.Tables[0].Rows.Count; index++)
                    {
                        temp += ds_segment.Tables[0].Rows[index][0].ToString().Trim() + ",";
                        if (index % 4 == 0 && index > 0)
                            temp += " ";
                    }
                    dr["Segment"] = temp;
                }
            }
        }
    }

    private void addCountryCol(DataSet ds)
    {
        ds.Tables[0].Columns.Add("Country");
        if (ds.Tables[0].Rows[0][0] != DBNull.Value)
        {
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                string userid = dr[0].ToString().Trim();
                DataSet ds_country = sql.getCountryByUser(userid);
                if (ds_country == null && ds_country.Tables[0].Rows.Count == 0)
                {
                    dr["Country"] = "";
                }
                else
                {
                    int index = 0;
                    string temp = "";
                    for (; index < ds_country.Tables[0].Rows.Count; index++)
                    {
                        temp += ds_country.Tables[0].Rows[index][0].ToString().Trim() + ",";
                        if (index % 4 == 0 && index > 0)
                            temp += " ";
                    }
                    dr["Country"] = temp;
                }
            }
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
        gv_administrator.Width = Unit.Pixel(800);
        gv_administrator.AutoGenerateColumns = false;
        gv_administrator.AllowPaging = true;
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

            gv_administrator.Columns.Add(bf);
        }

        gv_administrator.AllowSorting = true;
        gv_administrator.DataSource = ds.Tables[0];
        gv_administrator.DataBind();
        gv_administrator.Columns[0].Visible = false;
        gv_administrator.Columns[6].Visible = false;
        gv_administrator.Columns[7].Visible = false;
        gv_administrator.Columns[8].Visible = false;
        gv_administrator.Columns[9].Visible = false;
    }

    protected void bindDataSource()
    {
        gv_administrator.Columns.Clear();
        string str = tbox_find.Text.Trim();
        int sel = int.Parse(ddlist_in.SelectedItem.Value.Trim());
        DataSet ds;
        ds = sql.getAdministratorInfo(str, sel);
        bindDataSource(ds);
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

    protected void gv_administrator_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        gv_administrator.Columns.Clear();
        gv_administrator.PageIndex = e.NewPageIndex;
        bindDataSource();
    }

    protected void btn_find_Click(object sender, EventArgs e)
    {
        gv_administrator.Columns.Clear();
        string str_content = tbox_find.Text.Trim();
        int sel = int.Parse(ddlist_in.SelectedItem.Value.Trim());
        DataSet ds;
        ds = sql.getAdministratorInfo(str_content, sel);
        bindDataSource(ds);
    }
}
