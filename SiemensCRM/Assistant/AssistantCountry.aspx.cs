/*
 * File Name   : AssistantCountry.aspx.cs
 * 
 * Description : add and delete region, subregion, cluster and country 
 * 
 * Author      : Wang Jun
 * 
 * Modify Date : 2010.12.14
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

public partial class Assistant_AssistantCountryCustomer : System.Web.UI.Page
{
    SQLHelper helper = new SQLHelper();
    LogUtility log = new LogUtility();
    DisplayInfo info = new DisplayInfo();
    WebUtility web = new WebUtility();
    SQLStatement sql = new SQLStatement();

    protected void Page_Load(object sender, EventArgs e)
    {
        Page.MaintainScrollPositionOnPostBack = true;
        if (getRoleID(getRole()) == "5")
        {
            panel_readonly.Visible = false;
        }
        else
        {
            Response.Redirect("~/AccessDenied.aspx");
        }
        if (!IsPostBack)
        {
            log.WriteLog(LogUtility.LogErrorLevel.LOG_ACCESS, "AssistantCountry Access.");
            setSearchOption();
            lbtn_add.Visible = false;
            pnl_add.Visible = false;
            null_input();
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

    protected void null_input()
    {
        lbl_add.Text = "";
        tbox_addname.Text = "";
        tbox_countryISO.Text = "";

        lbl_del.Text = "";
    }

    protected void setSearchOption()
    {
        ddlist_description.Width = 100;
        ddlist_description.Items.Add(new ListItem("Region", "0"));
        ddlist_description.Items.Add(new ListItem("Cluster", "1"));
        ddlist_description.Items.Add(new ListItem("Country", "2"));
        ddlist_description.Items.Add(new ListItem("SubRegion", "3"));

    }


    protected bool Exist_country(string str_country, string coutryISOCode)
    {
        string query_country = "SELECT ID FROM [Country] WHERE (ISO_Code = '" + coutryISOCode + "' OR Name = '" + str_country + "')"
                            + "  AND Deleted = 0 ";
        DataSet ds_country = helper.GetDataSet(query_country);

        if (ds_country.Tables[0].Rows.Count > 0)
            return false;
        else
            return true;
    }

    //By Lhy 20110505 ITEM 6 DEL Start
    /*protected DataSet getOptionInfo(int sel)
    {
        string sql = "";
        DataSet ds = null;

        if (sel == 0)
        {
            sql = "SELECT ID, Name FROM [Region] WHERE Deleted = 0 GROUP BY Name, ID ORDER BY Name, ID ASC";
            ds = helper.GetDataSet(sql);
        }
        else if (sel == 1)
        {
            sql = " SELECT ID, Name FROM [Cluster] WHERE Deleted = 0 GROUP BY Name, ID ORDER BY Name, ID ASC";
            ds = helper.GetDataSet(sql);
        }
        else if (sel == 2)
        {
            sql = " SELECT ID, Name, ISO_Code FROM [Country] WHERE Deleted = 0 GROUP BY Name, ISO_Code, ID ORDER BY Name, ISO_Code, ID ASC";
            ds = helper.GetDataSet(sql);
        }
        else if (sel == 3)
        {
            sql = " SELECT ID, Name FROM [SubRegion] WHERE Deleted = 0 GROUP BY Name, ID ORDER BY Name, ID ASC";
            ds = helper.GetDataSet(sql);
        }

        return ds;
    }
      */
    //By Lhy 20110505 ITEM 6 DEL Start

    //By Lhy 20110505 ITEM 6 ADD Start

    protected DataSet getOptionInfo(int sel, string text)
    {
        string sql = "";
        DataSet ds = null;

        if (sel == 0)
        {
            sql = "SELECT ID, Name FROM [Region] WHERE Deleted = 0 AND Name like '%" + text + "%' GROUP BY Name, ID ORDER BY Name, ID ASC ";
            ds = helper.GetDataSet(sql);
        }
        else if (sel == 1)
        {
            sql = " SELECT ID, Name FROM [Cluster] WHERE Deleted = 0 AND Name like '%" + text + "%'GROUP BY Name, ID ORDER BY Name, ID ASC";
            ds = helper.GetDataSet(sql);
        }
        else if (sel == 2)
        {
            sql = " SELECT ID, Name, ISO_Code FROM [Country] WHERE Deleted = 0 AND Name like '%" + text + "%'GROUP BY Name, ISO_Code, ID ORDER BY Name, ISO_Code, ID ASC";
            ds = helper.GetDataSet(sql);
        }
        else if (sel == 3)
        {
            sql = " SELECT ID, Name FROM [SubRegion] WHERE Deleted = 0 AND Name like '%" + text + "%'GROUP BY Name, ID ORDER BY Name, ID ASC";
            ds = helper.GetDataSet(sql);
        }
        return ds;
    }
    //By Lhy 20110505 ITEM 6 ADD End
   
    protected void bindDataSource(DataSet ds)
    {
        bool notNullFlag = true;
        if (ds.Tables[0].Rows.Count == 0)
        {
            notNullFlag = false;
            sql.getNullDataSet(ds);
        }
        //By Lhy 20110504 ITEM 6 DEL start
             //gv_option.Width = Unit.Pixel(450);
        //By Lhy 20110504 ITEM 6 DEL End

        gv_option.AutoGenerateColumns = false;
        gv_option.AllowPaging = false;
        gv_option.Visible = true;

        for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
        {
            BoundField bf = new BoundField();

            bf.DataField = ds.Tables[0].Columns[i].ColumnName.ToString();
            //By Lhy 20110504 ITEM 6 DEL start
                //ds.Tables[0].Columns[i].Caption.ToString();
            //By Lhy 20110504 ITEM 6 DEL End

            //By Lhy 20110504 ITEM 6 ADD start
            bf.HeaderText = ddlist_description.SelectedItem.Text + " Name";
           //By Lhy 20110504 ITEM 6 ADD End
            bf.ItemStyle.Width = 350;
            //By Lhy 20110510 ITEM 6 ADD start
            if (string.Equals(ddlist_description.SelectedValue, "0"))
            {

                this.divflow.Attributes.Add("Style", "overflow: auto; height: 480px; width: 221px; position: relative; padding-top: 22px;");
               
            }

            else if (string.Equals(ddlist_description.SelectedValue, "1"))
            {

                this.divflow.Attributes.Add("Style", "overflow: auto; height: 480px; width: 221px; position: relative; padding-top: 22px;");

            }

            else if (string.Equals(ddlist_description.SelectedValue, "2"))
            {

                this.divflow.Attributes.Add("Style", "overflow: auto; height: 480px; width: 440px; position: relative; padding-top: 22px;");

            }

            else if (string.Equals(ddlist_description.SelectedValue, "3"))
            {

                this.divflow.Attributes.Add("Style", "overflow: auto; height: 480px; width: 320px; position: relative; padding-top: 22px;");

            }
            //By Lhy 20110504 ITEM 10 ADD End
            bf.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
            bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
            bf.HeaderStyle.Wrap = false;
            gv_option.Columns.Add(bf);
        }

        gv_option.AllowSorting = true;
        gv_option.DataSource = ds.Tables[0];
        gv_option.DataBind();
        gv_option.Columns[0].Visible = false;
    }

    //By Lhy 20110504 ITEM 6 DEL Start
    /* protected void bindDataSource()
     {
         gv_option.Columns.Clear();
         int sel = int.Parse(ddlist_description.SelectedItem.Value.Trim());
         DataSet ds;
         ds = getOptionInfo(sel);
         bindDataSource(ds);
     }
    */
    //By Lhy 20110504 ITEM 6 DEL End

    //By Lhy 20110504 ITEM 6 ADD Start
    protected void bindDataSource()
    {
        gv_option.Columns.Clear();
        int sel = int.Parse(ddlist_description.SelectedItem.Value.Trim());
        string text = ddlist_search2_text.Text.Trim();
        DataSet ds;
        ds = getOptionInfo(sel, text);
        bindDataSource(ds);
    }
    //By Lhy 20110504 ITEM 6 ADD Start

    protected void gv_option_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        gv_option.PageIndex = e.NewPageIndex;
        bindDataSource();
    }

    private void delRegion(string id, string name)
    {
        null_input();
        string sql = "UPDATE [Region] SET Deleted = 1 WHERE ID = " + id;
        int count = helper.ExecuteNonQuery(CommandType.Text, sql, null);
        if (count == 1)
        {
            lbl_del.ForeColor = System.Drawing.Color.Green;
            lbl_del.Text = info.delLabelInfo(name, true);
        }
        else
        {
            lbl_del.ForeColor = System.Drawing.Color.Red;
            lbl_del.Text = info.delLabelInfo(name, false);
        }
    }

    private void delCluster(string id, string name)
    {   
        //By Lhy 20110513 ITEM 6 ADD Start
        null_input();
        //By Lhy 20110513 ITEM 6 ADD Start
        string sql = "UPDATE [Cluster] SET Deleted = 1 WHERE ID = " + id;
        int count = helper.ExecuteNonQuery(CommandType.Text, sql, null);
        if (count == 1)
        {
            //By ryzhang 20110516 ITEM 6 ADD Start
            sql = "UPDATE [Region_Cluster] SET Deleted = 1 WHERE ClusterID = "+id;
            helper.ExecuteNonQuery(CommandType.Text, sql, null);
            //By ryzhang 2011051 ITEM 6 ADD End
            lbl_del.ForeColor = System.Drawing.Color.Green;
            lbl_del.Text = info.delLabelInfo(name, true);
        }
        else
        {
            lbl_del.ForeColor = System.Drawing.Color.Red;
            lbl_del.Text = info.delLabelInfo(name, false);
        }
    }

    private void delCountry(string id, string name)
    {
        //By Lhy 20110513 ITEM 6 ADD Start
        null_input();
        //By Lhy 20110513 ITEM 6 ADD Start
        string sql = "UPDATE [Country] SET Deleted = 1 WHERE ID = " + id;
        int count = helper.ExecuteNonQuery(CommandType.Text, sql, null);
        if (count == 1)
        {
            //By ryzhang 20110516 ITEM 6 ADD Start
            sql = "UPDATE [Cluster_Country] SET Deleted = 1 WHERE CountryID = " + id;
            helper.ExecuteNonQuery(CommandType.Text, sql, null);
            //By ryzhang 2011051 ITEM 6 ADD End
            lbl_del.ForeColor = System.Drawing.Color.Green;
            lbl_del.Text = info.delLabelInfo(name, true);
        }
        else
        {
            lbl_del.ForeColor = System.Drawing.Color.Red;
            lbl_del.Text = info.delLabelInfo(name, false);
        }
    }

    private void delSubRegion(string id, string name)
    {
        //By Lhy 20110513 ITEM 6 ADD Start
        null_input();
        //By Lhy 20110513 ITEM 6 ADD Start
        string sql = "UPDATE [SubRegion] SET Deleted = 1 WHERE ID = " + id;
        int count = helper.ExecuteNonQuery(CommandType.Text, sql, null);
        if (count == 1)
        {
            //By ryzhang 20110516 ITEM 6 ADD Start
            sql = "UPDATE [Country_SubRegion] SET Deleted = 1 WHERE SubRegionID = " + id;
            helper.ExecuteNonQuery(CommandType.Text, sql, null);
            //By ryzhang 20110516 ITEM 6 ADD End
            lbl_del.ForeColor = System.Drawing.Color.Green;
            lbl_del.Text = info.delLabelInfo(name, true);
        }
        else
        {
            lbl_del.ForeColor = System.Drawing.Color.Red;
            lbl_del.Text = info.delLabelInfo(name, false);
        }
    }

    protected void gv_option_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        //By ryzhang 20110517 ITEM 6 ADD Start
        this.btn_cancel_parent_Click(null,null);
        //By ryzhang 20110517 ITEM 6 ADD End
        null_input();
        string id = gv_option.Rows[e.RowIndex].Cells[0].Text.Trim();
        string name = gv_option.Rows[e.RowIndex].Cells[1].Text.Trim();

        if (ddlist_description.SelectedItem.Value.Trim() == "0")
            delRegion(id, name);
        else if (ddlist_description.SelectedItem.Value.Trim() == "1")
            delCluster(id, name);
        else if (ddlist_description.SelectedItem.Value.Trim() == "2")
            delCountry(id, name);
        else if (ddlist_description.SelectedItem.Value.Trim() == "3")
            delSubRegion(id, name);

        bindDataSource();
    }

    private bool existRegion(string name)
    {
        string query_region = "SELECT ID FROM [Region] WHERE Deleted = 0 AND Name = '" + name + "'";
        DataSet ds_region = helper.GetDataSet(query_region);
        if (ds_region.Tables[0].Rows.Count > 0)
            return true;
        else
            return false;
    }

    private void addRegion(string name)
    {
        //By Lhy 20110513 ITEM 6 ADD Start
        null_input();
        //By Lhy 20110513 ITEM 6 ADD Start
        lbl_add.ForeColor = System.Drawing.Color.Red;
        if (name != "")
        {
            if (!existRegion(name))
            {
                string insert_region = "INSERT INTO [Region](Name,Deleted) VALUES('" + name + "',0)";
                int count = helper.ExecuteNonQuery(CommandType.Text, insert_region, null);
                

                if (count == 1)
                {
           
                    lbl_add.ForeColor = System.Drawing.Color.Green;
                    lbl_add.Text = info.addLabelInfo(name, true);
                }
                else
                    lbl_add.Text = info.addLabelInfo(name, false);
            }
            else
                lbl_add.Text = info.addExist(name);
        }
        else
            lbl_add.Text = info.addillegal();
    }

    private bool existCluster(string name)
    {
        string query_cluster = "SELECT ID FROM [Cluster] WHERE Deleted = 0 AND Name = '" + name + "'";
        DataSet ds_cluster = helper.GetDataSet(query_cluster);
        if (ds_cluster.Tables[0].Rows.Count > 0)
            return true;
        else
            return false;
    }

    private void addCluster(string name)
    {
        //By Lhy 20110513 ITEM 6 ADD Start
        null_input();
        //By Lhy 20110513 ITEM 6 ADD Start
        lbl_add.ForeColor = System.Drawing.Color.Red;
        if (name != "")
        {
            if (!existCluster(name))
            {
                string insert_cluster = "INSERT INTO [Cluster](Name,Deleted) VALUES('" + name + "',0)";
                int count = helper.ExecuteNonQuery(CommandType.Text, insert_cluster, null);

                if (count == 1)
                {
                    //By ryzhang 20110516 ITEM 6 ADD Start
                    string sql = String.Format(@"SELECT ID FROM [Cluster] WHERE Name = '{0}' AND Deleted = 0",name);
                    DataSet ds = helper.GetDataSet(sql);
                    if (ds.Tables.Count==0 || ds.Tables[0].Rows.Count == 0)
                    {
                        return;
                    }
                    sql = String.Format(@"INSERT INTO [Region_Cluster] (RegionID,ClusterID,Deleted) VALUES ({0},{1},0)",ddlist_parent.SelectedItem.Value,ds.Tables[0].Rows[0][0].ToString());
                    helper.ExecuteNonQuery(CommandType.Text, sql, null);
                    //By ryzhang 20110516 ITEM 6 ADD End
                    lbl_add.ForeColor = System.Drawing.Color.Green;
                    lbl_add.Text = info.addLabelInfo(name, true);
                }
                else
                    lbl_add.Text = info.addLabelInfo(name, false);
            }
            else
                lbl_add.Text = info.addExist(name);
        }
        else
            lbl_add.Text = info.addillegal();
    }

    private bool existCountry(string name, string ISO)
    {
        string query_country = "SELECT ID FROM [Country] WHERE Deleted = 0 AND Name = '" + name + "' AND ISO_Code = '" + ISO + "'";
        DataSet ds_country = helper.GetDataSet(query_country);
        if (ds_country.Tables[0].Rows.Count > 0)
            return true;
        else
            return false;
    }

    private void addCountry(string name, string ISO)
    {
        //By Lhy 20110513 ITEM 6 ADD Start
        null_input();
        //By Lhy 20110513 ITEM 6 ADD Start
        lbl_add.ForeColor = System.Drawing.Color.Red;
        if (name != "" && ISO != "")
        {
            if (!existCountry(name, ISO))
            {
                string insert_country = "INSERT INTO [Country](Name, ISO_Code, Deleted) VALUES('" + name + "','" + ISO + "',0)";
                int count = helper.ExecuteNonQuery(CommandType.Text, insert_country, null);

                if (count == 1)
                {
                    //By ryzhang 20110516 ITEM 6 ADD Start
                    string sql = String.Format(@"SELECT ID FROM [Country] WHERE Name = '{0}' AND Deleted = 0", name);
                    DataSet ds = helper.GetDataSet(sql);
                    if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                    {
                        return;
                    }
                    sql = String.Format(@"INSERT INTO [Cluster_Country] (ClusterID,CountryID,Deleted) VALUES ({0},{1},0)", ddlist_parent.SelectedItem.Value, ds.Tables[0].Rows[0][0].ToString());
                    helper.ExecuteNonQuery(CommandType.Text, sql, null);
                    //By ryzhang 20110516 ITEM 6 ADD End
                    lbl_add.ForeColor = System.Drawing.Color.Green;
                    lbl_add.Text = info.addLabelInfo(name, true);
                }
                else
                    lbl_add.Text = info.addLabelInfo(name, false);
            }
            else
                lbl_add.Text = info.addExist(name);
        }
        else
            lbl_add.Text = info.addillegal();
    }

    private bool existSubRegion(string name)
    {
        string query_subregion = "SELECT ID FROM [SubRegion] WHERE Deleted = 0 AND Name = '" + name + "'";
        DataSet ds_subregion = helper.GetDataSet(query_subregion);
        if (ds_subregion.Tables[0].Rows.Count > 0)
            return true;
        else
            return false;
    }

    private void addSubRegion(string name)
    {
        //By Lhy 20110513 ITEM 6 ADD Start
        null_input();
        //By Lhy 20110513 ITEM 6 ADD Start
        lbl_add.ForeColor = System.Drawing.Color.Red;
        if (name != "")
        {
            if (!existSubRegion(name))
            {
                string insert_subregion = "INSERT INTO [SubRegion](Name,Deleted) VALUES('" + name + "',0)";
                int count = helper.ExecuteNonQuery(CommandType.Text, insert_subregion, null);

                if (count == 1)
                {
                    //By ryzhang 20110516 ITEM 6 ADD Start
                    string sql = String.Format(@"SELECT ID FROM [SubRegion] WHERE Name = '{0}' AND Deleted = 0", name);
                    DataSet ds = helper.GetDataSet(sql);
                    if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                    {
                        return;
                    }
                    sql = String.Format(@"INSERT INTO [Country_SubRegion] (CountryID,SubRegionID,Deleted) VALUES ({0},{1},0)", ddlist_parent.SelectedItem.Value, ds.Tables[0].Rows[0][0].ToString());
                    helper.ExecuteNonQuery(CommandType.Text, sql, null);
                    //By ryzhang 20110516 ITEM 6 ADD End
                    lbl_add.ForeColor = System.Drawing.Color.Green;
                    lbl_add.Text = info.addLabelInfo(name, true);
                }
                else
                    lbl_add.Text = info.addLabelInfo(name, false);
            }
            else
                lbl_add.Text = info.addExist(name);
        }
        else
            lbl_add.Text = info.addillegal();
    }

    protected void btn_add_Click(object sender, EventArgs e)
    {
        lbtn_add.Text = "Add " + ddlist_description.SelectedItem.Text;
        lbtn_add.Enabled = true;
        pnl_add.Visible = false;

        string name = tbox_addname.Text.Trim();
        string iso = tbox_countryISO.Text.Trim();

        if (ddlist_description.SelectedItem.Value.Trim() == "0")
            addRegion(name);
        else if (ddlist_description.SelectedItem.Value.Trim() == "1")
            addCluster(name);
        else if (ddlist_description.SelectedItem.Value.Trim() == "2")
            addCountry(name, iso);
        else if (ddlist_description.SelectedItem.Value.Trim() == "3")
            addSubRegion(name);

        bindDataSource();
    }

    protected void btn_cancel_Click(object sender, EventArgs e)
    {
        lbtn_add.Text = "Add " + ddlist_description.SelectedItem.Text;
        lbtn_add.Enabled = true;
        pnl_add.Visible = false;
    }

    protected void lbtn_add_Click(object sender, EventArgs e)
    {
        lbtn_add.Text = "Input " + ddlist_description.SelectedItem.Text;
        lbtn_add.Enabled = false;
        pnl_add.Visible = true;
        null_input();
        lbl_addname.Text = ddlist_description.SelectedItem.Text;

        if (ddlist_description.SelectedItem.Value.Trim() == "2")
        {
            lbl_countryISO.Visible = true;
            tbox_countryISO.Visible = true;
        }
        else
        {
            lbl_countryISO.Visible = false;
            tbox_countryISO.Visible = false;
        }
    }


    //By Lhy 20110505 ITEM 6 DEL Start
      /*  protected void btn_search_Click(object sender, EventArgs e)
        {
            lbtn_add.Text = "Add " + ddlist_description.SelectedItem.Text;
            lbtn_add.Visible = true;
            null_input();
            bindDataSource();
        }
       */
    //By Lhy 20110505 ITEM 6 DEL Start

    //By Lhy 20110505 ITEM 6 ADD Start
    protected void btn_search_Click(object sender, EventArgs e)
    {
        lbtn_add.Text = "Add " + ddlist_description.SelectedItem.Text;
        //By ryzhang 20110516 ITEM 6 ADD Start
        this.panel_modify_parent.Visible = false;
        this.optionId.Value = ddlist_description.SelectedItem.Value;
        this.optionName.Value = ddlist_description.SelectedItem.Text;
        if (ddlist_description.SelectedItem.Text == "Region")
        {
            lbl_parent.Text = "";
            lbl_parent.Visible = false;
            ddlist_parent.Visible = false;
        }
        else
        {
            if (pnl_add.Visible)
            {
                this.btn_cancel_Click(null,null);
            }
            lbl_parent.Visible = true;
            ddlist_parent.Visible = true;

            if (ddlist_description.SelectedIndex>=1)
            {
                lbl_parent.Text = ddlist_description.Items[ddlist_description.SelectedIndex-1].Text;
                SetParent(ddlist_description.SelectedIndex);
            }
            else
            {
                lbl_parent.Visible = true;
                ddlist_parent.Visible = true;
            }
        }

        //By ryzhang 20110516 ITEM 6 ADD end
        lbtn_add.Visible = true;
        null_input();
        bindDataSource();
        this.ddlist_search2_text.Enabled = true;
    }
    //By Lhy 20110505 ITEM 6 ADD END
    //By ryzhang 20110516 ITEM 6 ADD Start
    /// <summary>
    /// set value in dropdownlist
    /// </summary>
    /// <param name="falg">1、region,2、cluster，3、Country</param>
    private void SetParent(int falg)
    {
        String sql = null;
        //BY RYZHANG ITEM6 20110525 MODIFY START
        switch (falg)
        {
            //case 1:
            //    sql = "SELECT distinct ID, Name FROM [Region]  WHERE Deleted = 0";
            //    break;
            //case 2:
            //    sql = "SELECT distinct ID, Name FROM [Cluster] WHERE Deleted = 0";
            //    break;
            //case 3:
            //    sql = "SELECT distinct ID, Name FROM [Country] WHERE Deleted = 0";
            //    break;
            //default:
            //    return;
            case 1:
                sql = "SELECT distinct ID, Name FROM [Region]  WHERE Deleted = 0 ORDER BY NAME";
                break;
            case 2:
                sql = "SELECT distinct ID, Name FROM [Cluster] WHERE Deleted = 0 ORDER BY NAME";
                break;
            case 3:
                sql = "SELECT distinct ID, Name FROM [Country] WHERE Deleted = 0 ORDER BY NAME";
                break;
            default:
                return;
        }
        //BY RYZHANG ITEM6 20110525 MODIFY END
        DataSet ds = helper.GetDataSet(sql);
        ddlist_parent.Items.Clear();
        if (ds.Tables.Count!=0)
        {
            foreach (DataRow row in ds.Tables[0].Rows)
            {
               ddlist_parent.Items.Add(new ListItem(row[1].ToString(), row[0].ToString()));
            }
        }
    }

    /// <summary>
    /// edit parent item
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void gv_option_SelectedIndexChanging(object sender, GridViewSelectEventArgs e)
    {
        this.lbl_modify_parent_error.Text = "";
        String sqlAll = null;
        String sqlSelect = null;
        ddlist_modify_parent.Items.Clear();
        //BY RYZHANG ITEM6 20110525 MODIFY START
        switch (this.optionName.Value)
        {
            //case "Cluster":
            //    lbl_modify_parent.Text = "Region";
            //    sqlAll = "SELECT ID,Name FROM [Region] WHERE Deleted = 0";
            //    sqlSelect = "SELECT RegionID FROM [Region_Cluster] WHERE Deleted = 0 AND ClusterID = " + gv_option.Rows[e.NewSelectedIndex].Cells[0].Text;
            //    break;
            //case "Country":
            //    lbl_modify_parent.Text = "Cluster";
            //    sqlAll = "SELECT ID,Name FROM [Cluster] WHERE Deleted = 0";
            //    sqlSelect = "SELECT ClusterID FROM [Cluster_Country] WHERE Deleted = 0 AND CountryID = " + gv_option.Rows[e.NewSelectedIndex].Cells[0].Text;
            //    break;
            //case "SubRegion":
            //    lbl_modify_parent.Text = "Country";
            //    sqlAll = "SELECT ID,Name FROM [Country] WHERE Deleted = 0";
            //    sqlSelect = "SELECT CountryID FROM [Country_SubRegion] WHERE Deleted = 0 AND SubRegionID = " + gv_option.Rows[e.NewSelectedIndex].Cells[0].Text;
            //    break;
            //default:
            //    break;
            case "Cluster":
                lbl_modify_parent.Text = "Region";
                sqlAll = "SELECT ID,Name FROM [Region] WHERE Deleted = 0 ORDER BY NAME";
                sqlSelect = "SELECT RegionID FROM [Region_Cluster] WHERE Deleted = 0 AND ClusterID = " + gv_option.Rows[e.NewSelectedIndex].Cells[0].Text;
                break;
            case "Country":
                lbl_modify_parent.Text = "Cluster";
                sqlAll = "SELECT ID,Name FROM [Cluster] WHERE Deleted = 0 ORDER BY NAME";
                sqlSelect = "SELECT ClusterID FROM [Cluster_Country] WHERE Deleted = 0 AND CountryID = " + gv_option.Rows[e.NewSelectedIndex].Cells[0].Text;
                break;
            case "SubRegion":
                lbl_modify_parent.Text = "Country";
                sqlAll = "SELECT ID,Name FROM [Country] WHERE Deleted = 0 ORDER BY NAME";
                sqlSelect = "SELECT CountryID FROM [Country_SubRegion] WHERE Deleted = 0 AND SubRegionID = " + gv_option.Rows[e.NewSelectedIndex].Cells[0].Text;
                break;
            default:
                break;
        }
        //BY RYZHANG ITEM6 20110525 MODIFY START
        ddlist_modify_parent.Items.Add(new ListItem("",""));
        String selectId = "";
        DataSet dsAll = helper.GetDataSet(sqlAll);
        DataSet dsSelect = helper.GetDataSet(sqlSelect);
        if (dsSelect.Tables.Count!=0 && dsSelect.Tables[0].Rows.Count!=0)
        {
            selectId = dsSelect.Tables[0].Rows[0][0].ToString();
        }
        foreach (DataRow row in dsAll.Tables[0].Rows)
        {
            ListItem li = new ListItem(row[1].ToString(),row[0].ToString());
            if (row[0].ToString() == selectId)
            {
                li.Selected = true;
            }
            ddlist_modify_parent.Items.Add(li);
        }
        this.modify_option_id.Value = gv_option.Rows[e.NewSelectedIndex].Cells[0].Text;
        this.panel_modify_parent.Visible = true;
    }
    protected void btn_modify_parent_Click(object sender, EventArgs e)
    {
        this.lbl_del.Text = "";
        lbl_modify_parent_error.Text = "";
        if (ddlist_modify_parent.SelectedItem.Value=="")
        {
            lbl_modify_parent_error.ForeColor = System.Drawing.Color.Red;
            switch (this.optionName.Value)
            {
                case "Cluster":
                    lbl_modify_parent_error.Text = "Please select one Region!";
                    break;
                case "Country":
                    lbl_modify_parent_error.Text = "Please select one Cluster!";
                    break;
                case "SubRegion":
                    lbl_modify_parent_error.Text = "Please select one Country!";
                    break;
                default:
                    break;
            }
        }
        else
        {
            String sql = null;
            switch (this.optionName.Value)
            {
                case "Cluster":
                    sql = String.Format(@"UPDATE [Region_Cluster] SET RegionID = {0},Deleted=0 WHERE ClusterID={1}", ddlist_modify_parent.SelectedItem.Value, this.modify_option_id.Value);
                    break;
                case "Country":
                    sql = String.Format(@"UPDATE [Cluster_Country] SET ClusterID = {0},Deleted=0 WHERE CountryID={1}", ddlist_modify_parent.SelectedItem.Value, this.modify_option_id.Value);
                    break;
                case "SubRegion":
                    sql = String.Format(@"UPDATE [Country_SubRegion] SET CountryID = {0},Deleted=0 WHERE SubRegionID={1}", ddlist_modify_parent.SelectedItem.Value, this.modify_option_id.Value);
                    break;
                default:
                    break;
            }
            helper.ExecuteNonQuery(CommandType.Text, sql, null);
            lbl_modify_parent_error.ForeColor = System.Drawing.Color.Green;
            lbl_modify_parent_error.Text = @"Successfully modified!";
        }
    }
    protected void btn_cancel_parent_Click(object sender, EventArgs e)
    {
        this.panel_modify_parent.Visible = false;
    }
    //By ryzhang 20110516 ITEM 6 ADD End
}


