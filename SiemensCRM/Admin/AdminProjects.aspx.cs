/*
 * File Name   : AdminProjects.aspx.cs
 * 
 * Description : add a project, edit a project and delete a project
 * 
 * Author      : Wang Jun
 * 
 * Modify Date : 2011-01-13
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

public partial class Admin_AdminProjects : System.Web.UI.Page
{
    SQLHelper helper = new SQLHelper();
    LogUtility log = new LogUtility();
    DisplayInfo info = new DisplayInfo();
    WebUtility web = new WebUtility();
    SQLStatement sql = new SQLStatement();

    protected void Page_Load(object sender, EventArgs e)
    {
        Page.MaintainScrollPositionOnPostBack = true;
        if (getRoleID(getRole()) == "0")
        {
            panel_readonly.Visible = true;
        }
        else if (getRoleID(getRole()) == "5" )
        {
            panel_readonly.Visible = false;
        }
        else
        {
            Response.Redirect("~/AccessDenied.aspx");
        }
        if (!IsPostBack)
        {
            log.WriteLog(LogUtility.LogErrorLevel.LOG_ACCESS, "AdminCountry Access.");
            panel_addproject.Visible = false;
            label_del.Visible = false;
            label_add.Visible = false;
            //By xsdai 2011-5-4 Item11  
            //getsearchIN();
            //list.bindFind(list.getProject(), ddlist_find);
            bindDataSource();
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

    protected void getsearchIN()
    {
        ddlist_in.Items.Add(new ListItem("Project Name", "0"));
        ddlist_in.Items.Add(new ListItem("Customer Name", "1"));
        ddlist_in.Items.Add(new ListItem("Project Country(POS)", "2"));
        ddlist_in.Items.Add(new ListItem("Currency", "3"));
        ddlist_in.Items.Add(new ListItem("Project Value", "4"));
    }

    private void nullInput()
    {
        tbox_name.Text = "";
        tbox_probability.Text = "";
        tbox_value.Text = "";
    }
     //By xsdai 2011-5-4 Item11
     // protected DataSet getProjectInfo(string str, int sel) 
     protected DataSet getProjectInfo()
     {//Segment 'AS Segment',
         string sql_project = "SELECT  [Project].ID, [Project].PODID, [Project].Name AS 'Project Name', 'Segment' AS 'Segment',[CustomerName].Name AS 'Customer Name',"
                                
                                + " [SubRegion].Name AS 'Project Country(POS)',"
                                + " [Project].Value AS 'Project Value', [Project].Probability AS '% in budget',[Currency].Name AS 'Currency', [Project].Comments"
                                + " FROM [Project], [SubRegion], [Customer], [CustomerName], [Currency]"
                                + " WHERE [SubRegion].ID = [Project].POSID"
                                + " AND [Customer].ID = [Project].CustomerNameID"
                                + " AND [Customer].NameID = [CustomerName].ID"
                                + " AND [Project].CurrencyID = [Currency].ID"
                                + " AND SubRegion.Deleted=0 "
                                + " AND Customer.Deleted=0 "
                                + " AND CustomerName.Deleted=0 "
                                + " AND Currency.Deleted=0 "
                                + " AND [Project].Name = null" // add daixeusong 2011-5-4
                                + " AND [Project].Deleted = 0 AND [SubRegion].Deleted = 0 AND [CustomerName].Deleted = 0 AND [Customer].Deleted = 0";
        //By xsdai 2011-5-4 Item11 
        //if (sel == 0)
        //{
        //    sql_project += " AND [Project].Name like '%" + str + "%'"
        //                + " ORDER BY [Project].Name, [SubRegion].Name ASC";
        //}
        //else if (sel == 1)
        //{
        //    sql_project += " AND [CustomerName].Name like '%" + str + "%'"
        //                + " ORDER BY [CustomerName].Name, [Project].Name ASC";
        //}
        //else if (sel == 2)
        //{
        //    sql_project += " AND [SubRegion].Name like '%" + str + "%'"
        //                + " ORDER BY [SubRegion].Name,[Project].Name ASC";
        //}
        //else if (sel == 3)
        //{
        //    sql_project += " AND [Currency].Name like '%" + str + "%'"
        //                + " ORDER BY [Currency].Name,[Project].Name ASC";
        //}
        //else if (sel == 4)
        //{
        //    sql_project += " AND [Project].Value = '" + str + "'"
        //                + " ORDER BY [Project].Value,[Project].Name ASC";
        //}
        //else
        //{
            sql_project += " ORDER BY [Project].Name, [SubRegion].Name,[CustomerName].Name ASC";
        //}
        
        DataSet ds_project = helper.GetDataSet(sql_project);
        return ds_project;
    }

    private void addCalculateCol(DataSet ds)
    {
        ds.Tables[0].Columns.Add("absolute in budget");
        if (ds.Tables[0].Rows[0][0] != DBNull.Value)
        {
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                string str_value = dr[5].ToString().Trim();
                string str_probility = dr[6].ToString().Trim();
                if (str_value == "")
                    str_value = "0";
                if (str_probility == "")
                    str_probility = "";
                float value = float.Parse(dr[5].ToString().Trim());
                float probility = float.Parse(dr[6].ToString().Trim());
                dr["absolute in budget"] = value  * probility / 100;
            }
        }
    }

    private void addPODCol(DataSet ds)
    {
        ds.Tables[0].Columns.Add("Project Country(POD)");
        if (ds.Tables[0].Rows[0][0] != DBNull.Value)
        {
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                string str_countryID = dr[1].ToString().Trim();
                string str_countryISO = sql.getSubRegionByID(str_countryID);
                dr["Project Country(POD)"] = str_countryISO;
            }
        }
    }

    protected void bindDataSource(DataSet ds_project)
    {
        bool notNullFlag = true;
        if (ds_project.Tables[0].Rows.Count == 0)
        {
            notNullFlag = false;
            sql.getNullDataSet(ds_project);
        }
        gv_Project.Width = Unit.Pixel(800);
        gv_Project.AutoGenerateColumns = false;
        gv_Project.AllowPaging = true;
        gv_Project.Visible = true;

        addCalculateCol(ds_project);
        addPODCol(ds_project);

        for (int i = 0; i < ds_project.Tables[0].Columns.Count; i++)
        {
            BoundField bf = new BoundField();

            bf.DataField = ds_project.Tables[0].Columns[i].ColumnName.ToString();
            bf.HeaderText = ds_project.Tables[0].Columns[i].Caption.ToString();
            bf.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
            bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
            bf.ReadOnly = true;

            if (i == 5 || i == 6 || i == 8)
                bf.ReadOnly = false;

            gv_Project.Columns.Add(bf);
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
        gv_Project.Columns.Add(cf_Update);

        CommandField cf_Delete = new CommandField();
        cf_Delete.ButtonType = ButtonType.Image;
        cf_Delete.ShowDeleteButton = true;
        cf_Delete.ShowCancelButton = true;
        cf_Delete.CausesValidation = false;
        cf_Delete.DeleteImageUrl = "~/images/del.jpg";
        cf_Delete.DeleteText = "Delete";
        gv_Project.Columns.Add(cf_Delete);

        gv_Project.AllowSorting = true;
        gv_Project.DataSource = ds_project.Tables[0];
        gv_Project.DataBind();

        gv_Project.Columns[gv_Project.Columns.Count - 1].Visible = notNullFlag;
        gv_Project.Columns[gv_Project.Columns.Count - 2].Visible = notNullFlag;
        gv_Project.Columns[0].Visible = false;
        gv_Project.Columns[1].Visible = false;
        if (getRoleID(getRole()) != "0")
        {
            gv_Project.Columns[gv_Project.Columns.Count - 2].Visible = false;
            gv_Project.Columns[gv_Project.Columns.Count - 1].Visible = false;
        }
        gv_Project.Visible = true;
    }

    protected void bindDataSource()
    {
        gv_Project.Columns.Clear();
        //By xsdai 2011-5-4 Item11 
        //string str = ddlist_find.Text.Trim();
        //int sel = int.Parse(ddlist_in.SelectedItem.Value.Trim());
        DataSet ds;
        //By xsdai 2011-5-4 Item11 
        //ds = getProjectInfo(str, sel);
        ds = getProjectInfo();
        bindDataSource(ds);
    }
    //By xsdai 2011-5-4 Item11  
    //protected void btn_find_Click(object sender, EventArgs e)
    //{
    //    gv_Project.Columns.Clear();
    //    string str = ddlist_find.Text.Trim();
    //    int sel = int.Parse(ddlist_in.SelectedItem.Value.Trim());
    //    DataSet ds;

    //     ds = getProjectInfo(str, sel);
    //    bindDataSource(ds);
    //}

    protected void lbtn_findhelp_Click(object sender, EventArgs e)
    {
        string str_args = "'AdminHelp.aspx'" + ",'Help', 'height=500,width=800,top=100,left=200,toolbar=no,status=no, menubar=no,location=no,resizable=no,scrollbars=yes'";
        Response.Write("<script   language='javascript'>window.open(" + str_args + ");</script>");
    }

    protected void gv_Project_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        gv_Project.Columns.Clear();
        gv_Project.PageIndex = e.NewPageIndex;
        bindDataSource();
    }

    protected void gv_Project_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        label_del.Visible = true;
        
        string str_ID = gv_Project.Rows[e.RowIndex].Cells[0].Text.Trim();
        string str_name = gv_Project.Rows[e.RowIndex].Cells[2].Text.Trim();

        string del_project = "UPDATE [Project] SET Deleted = 1 "
                               + " WHERE ID =" + str_ID ;
        int delcount = helper.ExecuteNonQuery(CommandType.Text, del_project, null);

        if (delcount > 0)
        {
            label_del.ForeColor = System.Drawing.Color.Green;
            label_del.Text = info.delLabelInfo(str_name, true);
        }
        else
        {
            label_del.ForeColor = System.Drawing.Color.Red;
            label_del.Text = info.delLabelInfo(str_name, false);
        }

        gv_Project.Columns.Clear();
        bindDataSource();
    }

    protected void gv_Project_RowEditing(object sender, GridViewEditEventArgs e)
    {
        label_del.Visible = false;
        label_add.Visible = false;

        gv_Project.Columns.Clear();
        gv_Project.EditIndex = e.NewEditIndex;
        bindDataSource();
    }
    //By xsdai 2011-5-4 Item11 
    //protected void gv_Project_RowUpdating(object sender, GridViewUpdateEventArgs e)
    //{
    //    label_del.Visible = true;

    //    string str = ddlist_find.Text.Trim();
    //    int sel = int.Parse(ddlist_in.SelectedItem.Value.Trim());
    //    DataSet ds = getProjectInfo(str, sel);

    //    string str_id = gv_Project.Rows[e.RowIndex].Cells[0].Text.Trim();
    //    string str_name = gv_Project.Rows[e.RowIndex].Cells[2].Text.Trim();
    //    string str_value = ((TextBox)(gv_Project.Rows[e.RowIndex].Cells[5].Controls[0])).Text.Trim().Replace(',','.');
    //    string str_probability = ((TextBox)(gv_Project.Rows[e.RowIndex].Cells[6].Controls[0])).Text.Trim().Replace(',', '.');
    //    string str_comments = ((TextBox)(gv_Project.Rows[e.RowIndex].Cells[8].Controls[0])).Text.Trim();
    //    updproject(str_id, str_name, str_value, str_probability, str_comments);

    //    gv_Project.Columns.Clear();
    //    gv_Project.EditIndex = -1;
    //    bindDataSource();
    //}

    /// <summary>
    /// update a project
    /// </summary>
    /// <param name="str_ID"></param>
    /// <param name="str_name"></param>
    /// <param name="str_from"></param>
    /// <param name="str_value"></param>
    /// <param name="str_probability"></param>
    /// <param name="str_toID"></param>
    private void updproject(string str_ID, string str_name, string str_value, string str_probability, string str_comments)
    {
        label_del.ForeColor = System.Drawing.Color.Red;

        if (str_name.Trim().Length == 0)
        {
            label_del.Text = info.addillegal("Project Name is null.");
            return;
        }

        if (str_name.Trim().Length > 100)
        {
            label_del.Text = info.addillegal("The length of Project Name > 100.");
            return;
        }

        if (str_value.Trim().Length == 0)
        {
            label_del.Text = info.addillegal("Project Value is null.");
            return;
        }

        if (str_probability.Trim().Length == 0)
        {
            label_del.Text = info.addillegal("Probability is null.");
            return;
        }

        string sql = "UPDATE [Project] SET"
                   + " Value = '" + str_value + "',"
                   + " Probability = '" + str_probability + "',"
                   + " Comments = '" + str_comments + "'"
                   + " WHERE ID =" + str_ID;
        int count = helper.ExecuteNonQuery(CommandType.Text, sql, null);
        if (count == 1)
        {
            label_del.ForeColor = System.Drawing.Color.Green;
            label_del.Text = info.edtLabelInfo(str_name, true);
        }
        else
        {
            label_del.Text = info.edtLabelInfo(str_name, false);
        }
    }

    protected void gv_Project_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        gv_Project.Columns.Clear();
        gv_Project.EditIndex = -1;
        bindDataSource();
    }

    protected void gv_Project_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (e.Row.RowState == DataControlRowState.Normal || e.Row.RowState == DataControlRowState.Alternate)
            {
                ((ImageButton)e.Row.Cells[gv_Project.Columns.Count - 1].Controls[0]).Attributes.Add("onclick", info.setRowDataBound(e.Row.Cells[2].Text));
            }
        }
    }

    /*add a project*/
    private void bindCurrency()
    {
        DataSet ds_currency = sql.getCurrency();
        if (ds_currency.Tables[0].Rows.Count > 0)
        {
            DataTable dt_currency = ds_currency.Tables[0];
            int count = dt_currency.Rows.Count;
            int index = 0;
            while (index < count)
            {
                ListItem ll = new ListItem(dt_currency.Rows[index][0].ToString().Trim(), dt_currency.Rows[index][1].ToString().Trim());
                ddlist_currency.Items.Add(ll);
                index++;
            }
            ddlist_currency.Enabled = true;
        }
    }

    protected void bindToCountry()
    {
        DataSet ds_country = sql.getSubRegionInfo();
        if (ds_country.Tables[0].Rows.Count > 0)
        {
            DataTable dt_country = ds_country.Tables[0];
            int countcountry = dt_country.Rows.Count;
            int indexcountry = 0;
            while (indexcountry < countcountry)
            {
                ListItem ll = new ListItem(dt_country.Rows[indexcountry][1].ToString().Trim(), dt_country.Rows[indexcountry][0].ToString().Trim());
                ddlist_tocountry.Items.Add(ll);
                indexcountry++;
            }
            ddlist_tocountry.Enabled = true;
        }
    }

    protected void bindFromCountry()
    {
        DataSet ds_country = sql.getSubRegionInfo();
        if (ds_country.Tables[0].Rows.Count > 0)
        {
            DataTable dt_country = ds_country.Tables[0];
            int countcountry = dt_country.Rows.Count;
            int indexcountry = 0;
            while (indexcountry < countcountry)
            {
                ListItem ll = new ListItem(dt_country.Rows[indexcountry][1].ToString().Trim(), dt_country.Rows[indexcountry][0].ToString().Trim());
                ddlist_fromcountry.Items.Add(ll);
                indexcountry++;
            }
            ddlist_fromcountry.Enabled = true;
        }
    }

    protected void bindcustomer()
    {
        DataSet ds_customer = sql.getCustomerInfo("",10);
        if (ds_customer.Tables[0].Rows.Count > 0)
        {
            DataTable dt_customer = ds_customer.Tables[0];
            int count = dt_customer.Rows.Count;
            int index = 0;
            while (index < count)
            {
                string str_display = dt_customer.Rows[index][1].ToString().Trim() + "-" + dt_customer.Rows[index][2].ToString().Trim() + "-" + dt_customer.Rows[index][3].ToString().Trim();
                ListItem ll = new ListItem(str_display, dt_customer.Rows[index][0].ToString().Trim());
                ddlist_customer.Items.Add(ll);
                index++;
            }
        }
    }

    protected void lbtn_addproject_Click(object sender, EventArgs e)
    {
        lbtn_addproject.Text = "Input project name, value, propettitors and select project country, point of destination";
        lbtn_addproject.Enabled = false;
        panel_addproject.Visible = true;

        ddlist_fromcountry.Items.Clear();
        ddlist_tocountry.Items.Clear();
        ddlist_customer.Items.Clear();
        ddlist_currency.Items.Clear();
        bindcustomer();
        bindToCountry();
        bindFromCountry();
        bindCurrency();
        nullInput();

        label_add.Visible = false;
        label_del.Visible = false;
    }

    protected bool existproject(string str_name, string str_customer)
    {
        string sql_project = "SELECT [Project].ID"
                           + " FROM [Project] INNER JOIN [Customer]"
                           + " ON [Customer].ID = [Project].CustomerNameID"
                           + " WHERE [Project].Deleted = 0 AND [Project].Name = '" + str_name + "'"
                           + " AND [Customer].Deleted = 0 AND [Customer].ID = " + str_customer;
        DataSet ds_project = helper.GetDataSet(sql_project);

        if (ds_project.Tables[0].Rows.Count > 0)
            return false;
        else
            return true;
    }

    /// <summary>
    /// add a project
    /// </summary>
    /// <param name="str_name">Project Name</param>
    /// <param name="str_fromID">Project Country</param>
    /// <param name="str_value">Project Value</param>
    /// <param name="str_probability">Probability</param>
    /// <param name="str_toID">Point of Destination</param>
    private void addproject(string str_name, string str_customerID, string str_from, string str_value, string str_probability, string str_toID, string str_currencyID)
    {
        label_add.ForeColor = System.Drawing.Color.Red;

        if (str_name.Trim().Length == 0)
        {
            label_add.Text = info.addillegal("Project Name is null.");
            return;
        }

        if (str_name.Trim().Length > 100)
        {
            label_add.Text = info.addillegal("The length of Project Name > 100.");
            return;
        }

        if (str_value.Trim().Length == 0)
        {
            label_add.Text = info.addillegal("Project Value is null.");
            return;
        }

        if (str_probability.Trim().Length == 0)
        {
            label_add.Text = info.addillegal("Probability is null.");
            return;
        }

        if (!existproject(str_name, str_customerID))
        {
            label_add.Text = info.addExist(str_name);
            return;
        }

        string sql = "INSERT INTO [Project]([Name], CustomerNameID, POSID, [Value], Probability, PODID, CurrencyID,  Deleted)"
                   + " VALUES ('" + str_name + "','" + str_customerID + "','" + str_from + "','" + str_value + "','" + str_probability + "','" + str_toID + "','" + str_currencyID + "','0')";
        int count = helper.ExecuteNonQuery(CommandType.Text, sql, null);
        if (count == 1)
        {
            label_add.ForeColor = System.Drawing.Color.Green;
            label_add.Text = info.addLabelInfo(str_name, true);
        }
        else
        {
            label_add.Text = info.addLabelInfo(str_name, false);
        }
    }

    protected void btn_addproject_Click(object sender, EventArgs e)
    {
        lbtn_addproject.Text = "Add project";
        lbtn_addproject.Enabled = true;
        panel_addproject.Visible = false;

        label_add.Visible = true;

        string str_name = tbox_name.Text.Trim();
        string str_from = ddlist_fromcountry.SelectedItem.Value.Trim();
        string str_value = tbox_value.Text.Trim().Replace(',','.');
        string str_probability = tbox_probability.Text.Trim().Replace(',', '.');
        string str_toID = ddlist_tocountry.SelectedItem.Value.Trim();
        string str_customerID = ddlist_customer.SelectedItem.Value.Trim();
        string str_currencyID = ddlist_currency.SelectedItem.Value.Trim();
        addproject(str_name,str_customerID, str_from, str_value, str_probability, str_toID, str_currencyID);

        gv_Project.Columns.Clear();
        bindDataSource();
    }

    protected void btn_Cancelproject_Click(object sender, EventArgs e)
    {
        lbtn_addproject.Text = "Add project";
        lbtn_addproject.Enabled = true;
        panel_addproject.Visible = false;
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
                    list.bindFind(list.getProject(), ddlist_find);
                    break;
                }
            case 1:
                {
                    list.bindFind(list.getCustomerName(), ddlist_find);
                    break;
                }
            case 2:
                {
                    list.bindFind(list.getCountryISO_Code(), ddlist_find);
                    break;
                }
            case 3:
                {
                    list.bindFind(list.getCurrencyName(), ddlist_find);
                    break;
                }
            case 4:
                {
                    list.bindFind(list.getProjectValue(), ddlist_find);
                    break;
                }
        }
    }
}
