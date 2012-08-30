/*
 * File Name     : AssistantRegion.aspx.cs
 * 
 * Description   : add and delete relation about country
 * 
 * Author        : Wangjun
 * 
 * Modify Date   : 2010.12.14
 * 
 * Problem       : 
 * 
 * Version       : Release (2.0)
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

public partial class Assistant_AssistantRegion : System.Web.UI.Page
{
    SQLHelper helper = new SQLHelper();
    LogUtility log = new LogUtility();
    SQLStatement sql = new SQLStatement();
    DisplayInfo info = new DisplayInfo();

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
            pnl_addcluster.Visible = false;
            pnl_addcountry.Visible = false;
            pnl_addsubregion.Visible = false;
            getsearchIN();
            list.bindFind(list.getRegion(), ddlist_find);
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
    //By Lhy 20110509 ITEM 6 DEL Start
    /* protected void getsearchIN()
     {
         ddlist_in.Width = 100;
         ddlist_in.Items.Add(new ListItem("Country", "0"));
         ddlist_in.Items.Add(new ListItem("ISO Code", "1"));
         ddlist_in.Items.Add(new ListItem("Cluster", "2"));
         ddlist_in.Items.Add(new ListItem("Region", "3"));
         ddlist_in.Items.Add(new ListItem("SubRegion", "4"));
     }
     */
    //By Lhy 20110509 ITEM 6 DEL End

    //By Lhy 20110509 ITEM 6 ADD Start
    protected void getsearchIN()
    {
        ddlist_in.Width = 100;
        ddlist_in.Items.Add(new ListItem("Region", "0"));
        ddlist_in.Items.Add(new ListItem("Cluster", "1"));
        ddlist_in.Items.Add(new ListItem("Country", "2"));
        ddlist_in.Items.Add(new ListItem("SubRegion", "3"));
        ddlist_in.Items.Add(new ListItem("ISO Code", "4"));
    }
    //By Lhy 20110509 ITEM 6 ADD End
    private void clearText()
    {
        label_add.Text = "";
        label_del.Text = "";

        ddlist_region.Items.Clear();
        ddlist_region_cluster.Items.Clear();
        ddlist_cluster.Items.Clear();
        ddlist_cluster_country.Items.Clear();
        ddlist_country.Items.Clear();
        ddlist_country_subregion.Items.Clear();
    }

    protected void bindDataSource(DataSet ds_region)
    {
        bool notNullFlag = true;
        if (ds_region.Tables[0].Rows.Count == 0)
        {
            notNullFlag = false;
            sql.getNullDataSet(ds_region);
        }

        gv_Region.Width = Unit.Pixel(800);
        gv_Region.AutoGenerateColumns = false;
        gv_Region.AllowPaging = false;
        gv_Region.Visible = true;

        for (int i = 0; i < ds_region.Tables[0].Columns.Count; i++)
        {
            BoundField bf = new BoundField();

            bf.DataField = ds_region.Tables[0].Columns[i].ColumnName.ToString();
            bf.HeaderText = ds_region.Tables[0].Columns[i].Caption.ToString();
            bf.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
            bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
            bf.HeaderStyle.Wrap = false;
            gv_Region.Columns.Add(bf);
        }

        //CommandField cf_Delete = new CommandField();
        //cf_Delete.ButtonType = ButtonType.Image;
        //cf_Delete.ShowDeleteButton = true;
        //cf_Delete.ShowCancelButton = true;
        //cf_Delete.CausesValidation = false;
        //cf_Delete.DeleteImageUrl = "~/images/del.jpg";
        //cf_Delete.DeleteText = "Delete";
        //gv_Region.Columns.Add(cf_Delete);

        gv_Region.AllowSorting = true;
        gv_Region.DataSource = ds_region.Tables[0];
        gv_Region.DataBind();

        gv_Region.Columns[0].Visible = false;
        gv_Region.Columns[1].Visible = false;
        gv_Region.Columns[2].Visible = false;
        gv_Region.Columns[3].Visible = false;
    }

    protected void bindDataSource()
    {
        gv_Region.Columns.Clear();
        string str = ddlist_find.Text.Trim();
        int sel = int.Parse(ddlist_in.SelectedItem.Value.Trim());

        DataSet ds;
        //By Lhy 20110509 ITEM 6 DEL Start
        //ds = sql.getRegionInfo(str, sel, hidOrderBy.Value);
        //By Lhy 20110509 ITEM 6 DEL End

        //By Lhy 20110509 ITEM 6 ADD Start

        ds = sql.getRegionInfo(str, sel);
        //By Lhy 20110509 ITEM 6 ADD End
        bindDataSource(ds);
    }

    protected void gv_Region_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        gv_Region.PageIndex = e.NewPageIndex;
        gv_Region.Columns.Clear();
        bindDataSource();
    }

    protected void gv_Region_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        label_del.ForeColor = System.Drawing.Color.Red;
        clearText();
        string del_countryID = gv_Region.Rows[e.RowIndex].Cells[2].Text.Trim();
        string del_subregionID = gv_Region.Rows[e.RowIndex].Cells[3].Text.Trim();
        string regionName = gv_Region.Rows[e.RowIndex].Cells[7].Text.ToString().Trim();
        string subregionName = gv_Region.Rows[e.RowIndex].Cells[8].Text.ToString().Trim();

        string del_country_subregion = "UPDATE [Country_SubRegion] SET Deleted = 1"
                            + " WHERE SubRegionID = " + del_subregionID
                            + " AND CountryID = " + del_countryID;
        int count_subregion = helper.ExecuteNonQuery(CommandType.Text, del_country_subregion, null);

        if (count_subregion == 1)
        {
            label_del.ForeColor = System.Drawing.Color.Green;
            label_del.Text = info.delLabelInfo(regionName, subregionName, true);
        }
        else
            label_del.Text = info.delLabelInfo(regionName, subregionName, true);
        bindDataSource();
    }

    protected void gv_Region_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        //if (e.Row.RowType == DataControlRowType.DataRow)
        //{
        //    if (e.Row.RowState == DataControlRowState.Normal || e.Row.RowState == DataControlRowState.Alternate)
        //    {
        //        ((ImageButton)e.Row.Cells[gv_Region.Columns.Count - 1].Controls[0]).Attributes.Add("onclick", info.setRowDataBound(e.Row.Cells[8].Text, e.Row.Cells[7].Text));
        //    }
        //}
    }

    /// <summary>
    /// get country name
    /// </summary>
    /// <returns>return dataset</returns>
    private DataSet getNOTCluster(string regionID)
    {
        //By Lhy 20110504 ITEM 6 DEL Start

        /*string sql_cluster = " SELECT [Cluster].ID, [Cluster].Name FROM [Cluster] WHERE [Cluster].ID NOT IN ("
                            + " SELECT [Cluster].ID  FROM [Cluster] INNER JOIN [Region_Cluster]"
                            + " ON [Cluster].ID = [Region_Cluster].ClusterID"
                            + " WHERE [Region_Cluster].RegionID = " + regionID + " AND [Region_Cluster].Deleted = 0 AND [Cluster].Deleted = 0)"
                            + " GROUP BY [Cluster].ID, [Cluster].Name ORDER BY [Cluster].Name ASC";
         */
        //By Lhy 20110504 ITEM 6 DEL End

        //By Lhy 20110504 ITEM 6 ADD Start
        string sql_cluster = @"SELECT distinct c.ID, c.Name" +
                                 " FROM Region r INNER JOIN Region_Cluster rc ON r.ID=rc.RegionID" +
                                 " INNER JOIN Cluster c ON c.ID = rc.ClusterID" +
                                 " WHERE  rc.RegionID =" + regionID +
                                  "AND rc.Deleted=0 " +
                                 " AND c.Deleted=0";
        //By Lhy 20110504 ITEM 6 ADD End
        try
        {
            DataSet ds_cluster = helper.GetDataSet(sql_cluster);
            log.WriteLog(LogUtility.LogErrorLevel.LOG_NOISE, "SQLStatment.cs:(" + sql_cluster + "), execute successfully.");
            return ds_cluster;
        }
        catch (Exception ex)
        {
            log.WriteLog(LogUtility.LogErrorLevel.LOG_CRITICAL, "SQLStatment.cs:(" + sql_cluster + "), Exception:" + ex.Message);
            return null;
        }
    }

    /// <summary>
    /// get country name
    /// </summary>
    /// <returns>return dataset</returns>
    private DataSet getNOTCountry(string clusterID)
    {
        //By Lhy 20110505 ITEM 6 DEL End

        /*
          string sql_country = " SELECT [Country].ID, [Country].Name FROM [Country] WHERE [Country].ID NOT IN ("
                            + " SELECT [Country].ID  FROM [Country] INNER JOIN [Cluster_Country]"
                            + " ON [Country].ID = [Cluster_Country].CountryID"
                            + " WHERE [Cluster_Country].ClusterID = " + clusterID + " AND [Cluster_Country].Deleted = 0 AND [Country].Deleted = 0)"
                            + " GROUP BY [Country].ID, [Country].Name ORDER BY [Country].Name ASC";
          
        */
        //By Lhy 20110505 ITEM 6 DEL End

        //By Lhy 20110505 ITEM 6 ADD Start
        string sql_country = @"SELECT distinct c.ID, c.Name" +
                                  " FROM Cluster cl INNER JOIN Cluster_Country cc ON cl.ID=cc.ClusterID" +
                                  " INNER JOIN Country c ON c.ID = cc.CountryID" +
                                  " WHERE cc.ClusterID = " + clusterID +
                                  " AND cc.Deleted=0 " +
                                  " AND c.Deleted=0";
        //By Lhy 20110505 ITEM 6 ADD End
        try
        {
            DataSet ds_country = helper.GetDataSet(sql_country);
            log.WriteLog(LogUtility.LogErrorLevel.LOG_NOISE, "SQLStatment.cs:(" + sql_country + "), execute successfully.");
            return ds_country;
        }
        catch (Exception ex)
        {
            log.WriteLog(LogUtility.LogErrorLevel.LOG_CRITICAL, "SQLStatment.cs:(" + sql_country + "), Exception:" + ex.Message);
            return null;
        }
    }

    /// <summary>
    /// get subregion name
    /// </summary>
    /// <returns>return dataset</returns>
    private DataSet getNOTSubRegion(string countryID)
    {
        //By Lhy 20110505 ITEM 6 DEL Start

        /*
          string sql_subregion = " SELECT [SubRegion].ID, [SubRegion].Name FROM [SubRegion] WHERE [SubRegion].ID NOT IN ("
                            + " SELECT [SubRegion].ID FROM [SubRegion] INNER JOIN [Country_SubRegion]"
                            + " ON [SubRegion].ID = [Country_SubRegion].CountryID"
                            + " WHERE [Country_SubRegion].CountryID = " + countryID + " AND [Country_SubRegion].Deleted = 0 AND [SubRegion].Deleted = 0)"
                            + " GROUP BY [SubRegion].ID, [SubRegion].Name ORDER BY [SubRegion].Name ASC";
        */
        //By Lhy 20110505 ITEM 6 DEL End

        //By Lhy 20110505 ITEM 6 ADD Start
        string sql_subregion = @"SELECT distinct s.ID, s.Name" +
                                  " FROM Country c INNER JOIN Country_SubRegion sc ON c.ID=sc.CountryID" +
                                  " INNER JOIN SubRegion s ON s.ID =sc.SubRegionID" +
                                  " WHERE sc.CountryID = " + countryID +
                                  " AND sc.Deleted=0 " +
                                  " AND s.Deleted=0";
        //By Lhy 20110505 ITEM 6 ADD End
        try
        {
            DataSet ds_subregion = helper.GetDataSet(sql_subregion);
            log.WriteLog(LogUtility.LogErrorLevel.LOG_NOISE, "SQLStatment.cs:(" + sql_subregion + "), execute successfully.");
            return ds_subregion;
        }
        catch (Exception ex)
        {
            log.WriteLog(LogUtility.LogErrorLevel.LOG_CRITICAL, "SQLStatment.cs:(" + sql_subregion + "), Exception:" + ex.Message);
            return null;
        }
    }

    private void bindDropDownList(DataSet ds, DropDownList ddlist)
    {
        if (ds.Tables[0].Rows.Count > 0)
        {
            DataTable dt = ds.Tables[0];
            int count = dt.Rows.Count;
            int index = 0;
            while (index < count)
            {
                ListItem ll = new ListItem(dt.Rows[index][1].ToString().Trim(), dt.Rows[index][0].ToString().Trim());
                ddlist.Items.Add(ll);
                index++;
            }
            ddlist.Enabled = true;
        }
        else
            ddlist.Enabled = false;
    }

    protected void btn_find_Click(object sender, EventArgs e)
    {
        gv_Region.Columns.Clear();
        string str = ddlist_find.Text.Trim();
        int sel = int.Parse(ddlist_in.SelectedItem.Value.Trim());
        DataSet ds;

        ds = sql.getRegionInfo(str, sel);
        bindDataSource(ds);
    }

    protected void lbtn_findhelp_Click(object sender, EventArgs e)
    {
        string str_args = "'AssistantHelp.aspx'" + ",'Help', 'height=500,width=800,top=100,left=200,toolbar=no,status=no, menubar=no,location=no,resizable=no,scrollbars=yes'";
        Response.Write("<script   language='javascript'>window.open(" + str_args + ");</script>");
    }

    private DataSet getRegion()
    {
        string sql_region = "SELECT ID, Name FROM [Region] WHERE Deleted = 0 GROUP BY Name,ID ORDER BY Name,ID ASC";
        DataSet ds_region = helper.GetDataSet(sql_region);
        return ds_region;
    }

    protected void lbtn_addcluster_Click(object sender, EventArgs e)
    {
        lbtn_addcluster.Text = "Select region and cluster";
        lbtn_addcluster.Enabled = false;
        clearText();
        pnl_addcluster.Visible = true;

        bindDropDownList(getRegion(), ddlist_region);
        bindDropDownList(getNOTCluster(ddlist_region.SelectedItem.Value.Trim()), ddlist_region_cluster);

        lbtn_addcountry.Enabled = false;
        lbtn_addsubregion.Enabled = false;
    }

    private void addCluster(string regionID, string clusterID, string cluster)
    {
        string sql = " INSERT INTO [Region_Cluster](RegionID, ClusterID, Deleted)"
                   + " VALUES(" + regionID + "," + clusterID + ",0)";
        int count = helper.ExecuteNonQuery(CommandType.Text, sql, null);
        if (count == 1)
        {
            label_add.ForeColor = System.Drawing.Color.Green;
            label_add.Text = info.addLabelInfo(cluster, true);
        }
        else
        {
            label_add.ForeColor = System.Drawing.Color.Red;
            label_add.Text = info.addLabelInfo(cluster, false);
        }
    }

    /// <summary>
    /// Check the Region-Cluster relation is existed or not.
    /// </summary>
    /// <param name="regionID">Region ID</param>
    /// <param name="clusterID">Cluster ID</param>
    /// <returns>Result</returns>
    private bool isRCRelationExist(string regionID, string clusterID)
    {
        StringBuilder sql = new StringBuilder();
        sql.AppendLine(" SELECT ");
        sql.AppendLine("   COUNT(*) COUNT ");
        sql.AppendLine(" FROM ");
        sql.AppendLine("   Region_Cluster ");
        sql.AppendLine(" WHERE ");
        sql.AppendLine("   RegionID=" + regionID);
        sql.AppendLine("   AND ClusterID=" + clusterID);
        sql.AppendLine("   AND Deleted=0");
        object count = helper.ExecuteScalar(CommandType.Text, sql.ToString(), null);
        if (Convert.ToInt32(count) == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    protected void btn_addcluster_Click(object sender, EventArgs e)
    {
        string regionID = ddlist_region.SelectedItem.Value.Trim();
        string clusterID = ddlist_region_cluster.SelectedItem.Value.Trim();
        string cluster = ddlist_region_cluster.SelectedItem.Text.Trim();

        if (isRCRelationExist(regionID, clusterID))
        {
            addCluster(regionID, clusterID, cluster);
        }
        else
        {
            label_add.ForeColor = System.Drawing.Color.Red;
            label_add.Text = info.addExist(cluster);
        }

        //By Lhy 20110513 ITEM 6 DEL Start
        //lbtn_addcluster.Text = "Add Cluster";
        //By Lhy 20110513 ITEM 6 DEL End

        //By Lhy 20110513 ITEM 6 ADD Start
        lbtn_addcluster.Text = "Region/Cluster";
        //By Lhy 20110513 ITEM 6 ADD End
        pnl_addcluster.Visible = false;
        lbtn_addcluster.Enabled = true;
        lbtn_addcountry.Enabled = true;
        lbtn_addsubregion.Enabled = true;
        bindDataSource();
    }

    protected void btn_cancelcluster_Click(object sender, EventArgs e)
    {
        //By Lhy 20110513 ITEM 6 DEL Start
        //lbtn_addcluster.Text = "Add Cluster";
        //By Lhy 20110513 ITEM 6 DEL End

        //By Lhy 20110513 ITEM 6 ADD Start
        lbtn_addcluster.Text = "Region/Cluster";
        //By Lhy 20110513 ITEM 6 ADD End
        pnl_addcluster.Visible = false;
        lbtn_addcluster.Enabled = true;
        lbtn_addcountry.Enabled = true;
        lbtn_addsubregion.Enabled = true;
    }

    private DataSet getCluster()
    {
        string sql = " SELECT ID, Name FROM [Cluster] WHERE Deleted = 0 GROUP BY Name, ID ORDER BY Name, ID ASC";
        DataSet ds = helper.GetDataSet(sql);
        return ds;
    }


    protected void lbtn_addcountry_Click(object sender, EventArgs e)
    {
        lbtn_addcountry.Text = "Select cluster and country";
        lbtn_addcountry.Enabled = false;
        clearText();
        pnl_addcountry.Visible = true;

        bindDropDownList(getCluster(), ddlist_cluster);
        bindDropDownList(getNOTCountry(ddlist_cluster.SelectedItem.Value.Trim()), ddlist_cluster_country);

        lbtn_addcluster.Enabled = false;
        lbtn_addsubregion.Enabled = false;
    }

    private void addCountry(string clusterID, string countryID, string country)
    {
        string sql = " INSERT INTO [Cluster_Country](ClusterID,CountryID, Deleted)"
                   + " VALUES(" + clusterID + "," + countryID + ",0)";
        int count = helper.ExecuteNonQuery(CommandType.Text, sql, null);
        if (count == 1)
        {
            label_add.ForeColor = System.Drawing.Color.Green;
            label_add.Text = info.addLabelInfo(country, true);
        }
        else
        {
            label_add.ForeColor = System.Drawing.Color.Red;
            label_add.Text = info.addLabelInfo(country, false);
        }
    }

    /// <summary>
    /// Check the Cluster-Country relation is existed or not.
    /// </summary>
    /// <param name="clusterID">Cluster ID</param>
    /// <param name="countryID">Country ID</param>
    /// <returns>Result</returns>
    private bool isCCRelationExist(string clusterID, string countryID)
    {
        StringBuilder sql = new StringBuilder();
        sql.AppendLine(" SELECT ");
        sql.AppendLine("   COUNT(*) COUNT ");
        sql.AppendLine(" FROM ");
        sql.AppendLine("   Cluster_Country ");
        sql.AppendLine(" WHERE ");
        sql.AppendLine("   ClusterID=" + clusterID);
        sql.AppendLine("   AND CountryID=" + countryID);
        sql.AppendLine("   AND Deleted=0");
        object count = helper.ExecuteScalar(CommandType.Text, sql.ToString(), null);
        if (Convert.ToInt32(count) == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    protected void btn_addcountry_Click(object sender, EventArgs e)
    {
        string clusterID = ddlist_cluster.SelectedItem.Value.Trim();
        string countryID = ddlist_cluster_country.SelectedItem.Value.Trim();
        string country = ddlist_cluster_country.SelectedItem.Text.Trim();

        if (isCCRelationExist(clusterID, countryID))
        {
            addCountry(clusterID, countryID, country);
        }
        else
        {
            label_add.ForeColor = System.Drawing.Color.Red;
            label_add.Text = info.addExist(country);
        }

        //By Lhy 20110513 ITEM 6 DEL Start
        //  lbtn_addcountry.Text = "Add Country";
        //By Lhy 20110513 ITEM 6 DEL End

        //By Lhy 20110513 ITEM 6 ADD Start
        lbtn_addcountry.Text = "Cluster/Country";
        //By Lhy 20110513 ITEM 6 ADD End
        pnl_addcountry.Visible = false;
        lbtn_addcluster.Enabled = true;
        lbtn_addcountry.Enabled = true;
        lbtn_addsubregion.Enabled = true;
        bindDataSource();
    }

    protected void btn_cancelcountry_Click(object sender, EventArgs e)
    {
        //By Lhy 20110513 ITEM 6 DEL Start
        // lbtn_addcountry.Text = "Add Country";
        //By Lhy 20110513 ITEM 6 DEL End

        //By Lhy 20110513 ITEM 6 ADD Start
        lbtn_addcountry.Text = "Cluster/Country";
        //By Lhy 20110513 ITEM 6 ADD End
        pnl_addcountry.Visible = false;
        lbtn_addcluster.Enabled = true;
        lbtn_addcountry.Enabled = true;
        lbtn_addsubregion.Enabled = true;
    }

    private DataSet getCountry()
    {
        string sql = " SELECT ID, Name FROM [Country] WHERE Deleted = 0 GROUP BY Name, ID ORDER BY Name, ID ASC";
        DataSet ds = helper.GetDataSet(sql);
        return ds;
    }

    protected void lbtn_addsubregion_Click(object sender, EventArgs e)
    {
        lbtn_addsubregion.Text = "Select country and subregion";
        lbtn_addsubregion.Enabled = false;
        clearText();
        pnl_addsubregion.Visible = true;

        bindDropDownList(getCountry(), ddlist_country);
        bindDropDownList(getNOTSubRegion(ddlist_country.SelectedItem.Value.Trim()), ddlist_country_subregion);

        lbtn_addcountry.Enabled = false;
        lbtn_addcluster.Enabled = false;
    }

    private void addSubregion(string countryID, string subregionID, string subregion)
    {
        string sql = " INSERT INTO [Country_SubRegion](CountryID,SubRegionID, Deleted)"
                   + " VALUES(" + countryID + "," + subregionID + ",0)";
        int count = helper.ExecuteNonQuery(CommandType.Text, sql, null);
        if (count == 1)
        {
            label_add.ForeColor = System.Drawing.Color.Green;
            label_add.Text = info.addLabelInfo(subregion, true);
        }
        else
        {
            label_add.ForeColor = System.Drawing.Color.Red;
            label_add.Text = info.addLabelInfo(subregion, false);
        }
    }

    /// <summary>
    /// Check the Country-Subregion relation is existed or not.
    /// </summary>
    /// <param name="countryID">Country ID</param>
    /// <param name="subregionID">Subregion ID</param>
    /// <returns>Result</returns>
    private bool isCSRelationExist(string countryID, string subregionID)
    {
        StringBuilder sql = new StringBuilder();
        sql.AppendLine(" SELECT ");
        sql.AppendLine("   COUNT(*) COUNT ");
        sql.AppendLine(" FROM ");
        sql.AppendLine("   Country_SubRegion ");
        sql.AppendLine(" WHERE ");
        sql.AppendLine("   CountryID=" + countryID);
        sql.AppendLine("   AND SubRegionID=" + subregionID);
        sql.AppendLine("   AND Deleted=0");
        object count = helper.ExecuteScalar(CommandType.Text, sql.ToString(), null);
        if (Convert.ToInt32(count) == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    protected void btn_addsubregion_Click(object sender, EventArgs e)
    {
        string countryID = ddlist_country.SelectedItem.Value.Trim();
        string subregionID = ddlist_country_subregion.SelectedItem.Value.Trim();
        string subregion = ddlist_country_subregion.SelectedItem.Text.Trim();

        if (isCSRelationExist(countryID, subregionID))
        {
            addSubregion(countryID, subregionID, subregion);
        }
        else
        {
            label_add.ForeColor = System.Drawing.Color.Red;
            label_add.Text = info.addExist(subregion);
        }

        //By Lhy 20110513 ITEM 6 DEL Start
        // lbtn_addsubregion.Text = "Add SubRegion";
        //By Lhy 20110513 ITEM 6 DEL End

        //By Lhy 20110513 ITEM 6 ADD Start
        lbtn_addsubregion.Text = "Country/Subregion";
        //By Lhy 20110513 ITEM 6 ADD End
        pnl_addsubregion.Visible = false;
        lbtn_addcluster.Enabled = true;
        lbtn_addcountry.Enabled = true;
        lbtn_addsubregion.Enabled = true;
        bindDataSource();
    }

    protected void btn_cancelsubregion_Click(object sender, EventArgs e)
    {
        //By Lhy 20110513 ITEM 6 DEL Start
        // lbtn_addsubregion.Text = "Add SubRegion";
        //By Lhy 20110513 ITEM 6 DEL End

        //By Lhy 20110513 ITEM 6 ADD Start
        lbtn_addsubregion.Text = "Country/Subregion";
        //By Lhy 20110513 ITEM 6 ADD End

        pnl_addsubregion.Visible = false;
        lbtn_addcluster.Enabled = true;
        lbtn_addcountry.Enabled = true;
        lbtn_addsubregion.Enabled = true;
    }

    protected void ddlist_region_SelectedIndexChanged(object sender, EventArgs e)
    {
        ddlist_region_cluster.Items.Clear();
        bindDropDownList(getNOTCluster(ddlist_region.SelectedItem.Value.Trim()), ddlist_region_cluster);
    }

    protected void ddlist_country_SelectedIndexChanged(object sender, EventArgs e)
    {
        ddlist_country_subregion.Items.Clear();
        bindDropDownList(getNOTSubRegion(ddlist_country.SelectedItem.Value.Trim()), ddlist_country_subregion);
    }

    protected void ddlist_cluster_SelectedIndexChanged(object sender, EventArgs e)
    {
        ddlist_cluster_country.Items.Clear();
        bindDropDownList(getNOTCountry(ddlist_cluster.SelectedItem.Value.Trim()), ddlist_cluster_country);
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
                    list.bindFind(list.getRegion(), ddlist_find);
                    break;

                }
            case 1:
                {
                    list.bindFind(list.getCluster(), ddlist_find);
                    break;

                }
            case 2:
                {
                    list.bindFind(list.getCountryName(), ddlist_find);
                    break;

                }
            case 3:
                {
                    list.bindFind(list.getSubRegion(), ddlist_find);
                    break;
                }
            case 4:
                {
                    list.bindFind(list.getCountryISO_Code(), ddlist_find);
                    break;

                }
        }
    }
}
