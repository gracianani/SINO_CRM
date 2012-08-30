/*
 * File Name   : SalesOrgMgrMarket.aspx.cs
 * 
 * Description : Add, edit and delete a market.
 * 
 * Author      : Wang Jun
 * 
 * Modify Date : 2010-10-22
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
// By DingJunjie 20110520 Item 17 Add Start
using System.Text;
// By DingJunjie 20110520 Item 17 Add End

public partial class SalesOrgMgr_SalesOrgMgrMarket : System.Web.UI.Page
{
    SQLHelper helper = new SQLHelper();
    LogUtility log = new LogUtility();
    DisplayInfo info = new DisplayInfo();
    WebUtility web = new WebUtility();
    SQLStatement sql = new SQLStatement();
    GetMeetingDate date = new GetMeetingDate();

    protected void Page_Load(object sender, EventArgs e)
    {
        Page.MaintainScrollPositionOnPostBack = true;
        if (getRoleID(getRole()) == "3")
        {
            panel_readonly.Visible = false;
        }
        else
        {
            Response.Redirect("~/AccessDenied.aspx");
        }
        if (!IsPostBack)
        {
            log.WriteLog(LogUtility.LogErrorLevel.LOG_ACCESS, "SalesOrgMgrMarket Access.");
            panel_addmarket.Visible = false;
            date.setDate();
            label_thisyear.Text = "Total Market " + date.getyear();
            label_nextyear.Text = "Total Market " + date.getnextyear();
            lbl_afteryear.Text = "Total Market " + date.getyearAfterNext();
            label_del.Visible = false;
            label_add.Visible = false;
            getsearchIN();
            // By DingJunjie 20110520 Item 12 Add Start
            bindDropDownList(sql.getSegmentInfo(), this.ddlSegment, "Segment", "ID", true);
            bindDropDownList(sql.getRegionInfo(), this.ddlist_find, "Name", "ID", true);
            // By DingJunjie 20110520 Item 12 Add End
            // By DingJunjie 20110520 Item 12 Delete Start
            //list.bindFind(list.getCountryName(), ddlist_find);
            // By DingJunjie 20110520 Item 12 Delete End
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

    protected void null_input()
    {
        tbox_afteryear.Text = "";
        tbox_nextyear.Text = "";
        tbox_thisyear.Text = "";
    }

    protected void getsearchIN()
    {
        // By DingJunjie 20110520 Item 12 Delete Start
        //ddlist_in.Items.Add(new ListItem("Country", "0"));
        //ddlist_in.Items.Add(new ListItem("Segment", "1"));
        // By DingJunjie 20110520 Item 12 Delete End
        // By DingJunjie 20110520 Item 12 Add Start
        ddlist_in.Items.Add(new ListItem("Region", "0"));
        ddlist_in.Items.Add(new ListItem("Cluster", "1"));
        ddlist_in.Items.Add(new ListItem("Country", "2"));
        // By DingJunjie 20110520 Item 12 Add End
    }

    protected void btn_find_Click(object sender, EventArgs e)
    {
        gv_market.Columns.Clear();
        string str = ddlist_find.Text.Trim();
        int sel = int.Parse(ddlist_in.SelectedItem.Value.Trim());
        DataSet ds;

        this.gv_market.EditIndex = -1;
        ds = getmarketInfo(str, sel);
        bindDataSource(ds);
        this.panel_addmarket.Visible = false;
        this.lbtn_market.Enabled = true;
        this.lbtn_market.Text = "Add market";
    }

    protected DataSet getmarketInfo(string str, int sel)
    {
        // By DingJunjie 20110520 Item 12 Delete Start
        //string sql_market = "SELECT [Segment].ID, [Country].ID, [Segment].Abbr AS 'Segment', [Country].Name AS 'Country', ROUND(ThisYear,0) AS '" + "Total Market " + date.getyear() + "', ROUND(NextYear,0) AS '" + "Total Market " + date.getnextyear() + "', ROUND(AfterYear,0) AS '" + "Total Market " + date.getyearAfterNext() + "'"
        //                   + " FROM [Market] INNER JOIN [Country] ON [Market].CountryID = [Country].ID"
        //                   + " INNER JOIN [Segment] ON [Market].SegmentID = [Segment].ID"
        //                   + " WHERE [Country].Deleted = 0 AND [Segment].Deleted = 0";
        //if (sel == 0)
        //    sql_market += " AND [Country].Name like '%" + str + "%'"
        //               + " ORDER BY [Country].Name, [Segment].Abbr ASC";
        //else if (sel == 1)
        //    sql_market += " AND [Segment].Abbr like '%" + str + "%'"
        //               + " ORDER BY [Segment].Abbr, [Country].Name ASC";
        //else
        //    sql_market += " ORDER BY [Segment].Abbr, [Country].Name ASC";
        // By DingJunjie 20110520 Item 12 Delete End

        // By DingJunjie 20110520 Item 12 Add Start
        StringBuilder sql = new StringBuilder();
        sql.AppendLine(" SELECT DISTINCT");
        sql.AppendLine("   Segment.ID AS SegmentID, ");
        sql.AppendLine("   Region.ID AS RegionID, ");
        sql.AppendLine("   Cluster.ID AS ClusterID, ");
        sql.AppendLine("   Country.ID AS CountryID, ");
        sql.AppendLine("   Segment.Abbr AS Segment, ");
        sql.AppendLine("   Region.Name AS Region, ");
        sql.AppendLine("   Cluster.Name AS Cluster, ");
        sql.AppendLine("   Country.Name AS Country, ");
        sql.AppendLine("   ROUND(ThisYear,0) AS 'Total Market " + date.getyear() + "', ");
        sql.AppendLine("   ROUND(NextYear,0) AS 'Total Market " + date.getnextyear() + "', ");
        sql.AppendLine("   ROUND(AfterYear,0) AS 'Total Market " + date.getyearAfterNext() + "' ");
        sql.AppendLine(" FROM ");
        sql.AppendLine("   Market ");
        sql.AppendLine("   INNER JOIN Segment ON Market.SegmentID=Segment.ID ");
        sql.AppendLine("   INNER JOIN Country ON Market.CountryID=Country.ID ");
        sql.AppendLine("   INNER JOIN Cluster_Country ON Country.ID=Cluster_Country.CountryID ");
        sql.AppendLine("   INNER JOIN Cluster ON Cluster_Country.ClusterID=Cluster.ID ");
        sql.AppendLine("   INNER JOIN Region_Cluster ON Cluster.ID=Region_Cluster.ClusterID ");
        sql.AppendLine("   INNER JOIN Region ON Region_Cluster.RegionID=Region.ID ");
        sql.AppendLine(" WHERE ");
        sql.AppendLine("   Segment.Deleted=0 ");
        sql.AppendLine("   AND Country.Deleted=0 ");
        sql.AppendLine("   AND Cluster_Country.Deleted=0 ");
        sql.AppendLine("   AND Cluster.Deleted=0 ");
        sql.AppendLine("   AND Region_Cluster.Deleted=0 ");
        sql.AppendLine("   AND Region.Deleted=0 ");
        sql.AppendLine("   AND TimeFlag='" + date.getSetMeetingDate().Tables[0].Rows[0][0].ToString().Trim() + "'");
        if (!string.IsNullOrEmpty(this.ddlSegment.SelectedValue))
        {
            sql.AppendLine("   AND Market.SegmentID=" + this.ddlSegment.SelectedValue);
        }
        if (!string.IsNullOrEmpty(this.ddlist_find.SelectedValue))
        {
            if (string.Equals(this.ddlist_in.SelectedValue, "0"))
            {
                sql.AppendLine("   AND Region.ID=" + this.ddlist_find.SelectedValue);
            }
            else if (string.Equals(this.ddlist_in.SelectedValue, "1"))
            {
                sql.AppendLine("   AND Cluster.ID=" + this.ddlist_find.SelectedValue);
            }
            else if (string.Equals(this.ddlist_in.SelectedValue, "2"))
            {
                sql.AppendLine("   AND Market.CountryID=" + this.ddlist_find.SelectedValue);
            }
        }
        sql.AppendLine(" ORDER BY ");
        sql.AppendLine("   Segment.Abbr, ");
        sql.AppendLine("   Region.Name, ");
        sql.AppendLine("   Cluster.Name, ");
        sql.AppendLine("   Country.Name ");
        string sql_market = sql.ToString();
        // By DingJunjie 20110520 Item 12 Add End
        DataSet ds_market = helper.GetDataSet(sql_market);
        return ds_market;
    }

    protected void bindDataSource(DataSet ds_market)
    {
        bool notNullFlag = true;
        if (ds_market.Tables[0].Rows.Count == 0)
        {
            notNullFlag = false;
            sql.getNullDataSet(ds_market);
        }
        gv_market.Width = Unit.Pixel(800);
        gv_market.AutoGenerateColumns = false;
        // by dxs 20110511 del start
        // gv_market.AllowPaging = true;
        // by dxs 20110511 del end 
        // by dxs 20110513 add start
        gv_market.AllowPaging = false;
        // by dxs 20110513 add end
        gv_market.Visible = true;

        // By DingJunjie 20110520 Item 12 Add Start
        int sumPer = 0;
        int sumNow = 0;
        int sumNex = 0;
        List<string> existList = new List<string>();
        if (ds_market != null && ds_market.Tables.Count > 0 && ds_market.Tables[0].Rows.Count > 0)
        {
            for (int i = 0; i < ds_market.Tables[0].Rows.Count; i++)
            {
                if (!existList.Contains(ds_market.Tables[0].Rows[i][0] + "|" + ds_market.Tables[0].Rows[i][3]))
                {
                    if (!string.IsNullOrEmpty(ds_market.Tables[0].Rows[i][8].ToString()))
                    {
                        sumPer += int.Parse(ds_market.Tables[0].Rows[i][8].ToString());
                    }
                    if (!string.IsNullOrEmpty(ds_market.Tables[0].Rows[i][9].ToString()))
                    {
                        sumNow += int.Parse(ds_market.Tables[0].Rows[i][9].ToString());
                    }
                    if (!string.IsNullOrEmpty(ds_market.Tables[0].Rows[i][10].ToString()))
                    {
                        sumNex += int.Parse(ds_market.Tables[0].Rows[i][10].ToString());
                    }
                    existList.Add(ds_market.Tables[0].Rows[i][0] + "|" + ds_market.Tables[0].Rows[i][3]);
                }
            }
        }
        DataRow row = ds_market.Tables[0].NewRow();
        row[7] = "Total";
        row[8] = sumPer.ToString();
        row[9] = sumNow.ToString();
        row[10] = sumNex.ToString();
        ds_market.Tables[0].Rows.Add(row);
        // By DingJunjie 20110520 Item 12 Add End

        for (int i = 0; i < ds_market.Tables[0].Columns.Count; i++)
        {
            BoundField bf = new BoundField();

            bf.DataField = ds_market.Tables[0].Columns[i].ColumnName.ToString();
            bf.HeaderText = ds_market.Tables[0].Columns[i].Caption.ToString();
            bf.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
            bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
            bf.ReadOnly = false;

            // By DingJunjie 20110520 Item 12 Delete Start
            //if (i == 0 || i == 1 || i == 2 || i == 3)
            // By DingJunjie 20110520 Item 12 Delete End
            // By DingJunjie 20110520 Item 12 Add Start
            if (i == 0 || i == 1 || i == 2 || i == 3
                || i == 4 || i == 5 || i == 6 || i == 7)
            // By DingJunjie 20110520 Item 12 Add End
                bf.ReadOnly = true;

            gv_market.Columns.Add(bf);
        }

        gv_market.AllowSorting = true;
        gv_market.DataSource = ds_market.Tables[0];
        gv_market.DataBind();

        gv_market.Columns[gv_market.Columns.Count - 1].Visible = notNullFlag;
        gv_market.Columns[gv_market.Columns.Count - 2].Visible = notNullFlag;
        gv_market.Columns[0].Visible = false;
        gv_market.Columns[1].Visible = false;
        // By DingJunjie 20110520 Item 12 Add Start
        gv_market.Columns[2].Visible = false;
        gv_market.Columns[3].Visible = false;
        this.gv_market.Rows[this.gv_market.Rows.Count - 1].Style.Add(HtmlTextWriterStyle.FontWeight, "900");
        this.gv_market.Rows[this.gv_market.Rows.Count - 1].Style.Add(HtmlTextWriterStyle.Color, "Red");
        this.gv_market.Rows[this.gv_market.Rows.Count - 1].Cells[this.gv_market.Columns.Count - 1].Controls.Clear();
        this.gv_market.Rows[this.gv_market.Rows.Count - 1].Cells[this.gv_market.Columns.Count - 2].Controls.Clear();
        // By DingJunjie 20110520 Item 12 Add End
    }

    protected void bindDataSource()
    {
        gv_market.Columns.Clear();
        string str = ddlist_find.Text.Trim();
        int sel = int.Parse(ddlist_in.SelectedItem.Value.Trim());
        DataSet ds;

        ds = getmarketInfo(str, sel);
        bindDataSource(ds);
    }

    protected void lbtn_findhelp_Click(object sender, EventArgs e)
    {
        string str_args = "'SalesOrgMgrHelp.aspx'" + ",'Help', 'height=500,width=800,top=100,left=200,toolbar=no,status=no, menubar=no,location=no,resizable=no,scrollbars=yes'";
        Response.Write("<script   language='javascript'>window.open(" + str_args + ");</script>");
    }

    protected bool existmarket(string str_countryid, string str_segmentid)
    {
        string sql_market = "SELECT *"
                           + " FROM [Market]"
                           + " WHERE CountryID = '" + str_countryid + "'"
                           + " AND SegmentID = '" + str_segmentid + "'";
        DataSet ds_market = helper.GetDataSet(sql_market);

        if (ds_market.Tables[0].Rows.Count > 0)
            return false;
        else
            return true;
    }


    /// <summary>
    /// add one market to the system if the segment and the country of the market did not repeat.
    /// </summary>
    /// <param name="str_countryid">Country ID</param>
    /// <param name="str_totalmarket">Total Market</param>
    /// <param name="str_compettitors">Compettitors</param>
    /// <param name="str_segmentid">Segment ID</param>
    /// <param name="str_country">Country Name</param>
    /// <param name="str_segment">Segment Description</param>
    private void addmarket(string str_countryid, string str_segmentid, string str_thisyear, string str_nextyear, string str_afteryear)
    {
        label_add.ForeColor = System.Drawing.Color.Red;

        if (str_thisyear.Trim().Length == 0)
        {
            str_thisyear = "0";
        }

        if (str_nextyear.Trim().Length == 0)
        {
            str_nextyear = "0";
        }

        if (str_afteryear.Trim().Length == 0)
        {
            str_afteryear = "0";
        }

        if (!existmarket(str_countryid, str_segmentid))
        {
            label_add.Text = info.addExist("Market");
            return;
        }

        string sql = "INSERT INTO [Market](CountryID,SegmentID,ThisYear,NextYear, AfterYear, TimeFlag)"
                   + " VALUES ('" + str_countryid + "','" + str_segmentid + "','" + str_thisyear + "','" + str_nextyear + "','" + str_afteryear + "','" + date.getSetMeetingDate().Tables[0].Rows[0][0].ToString().Trim() + "')";
        int count = helper.ExecuteNonQuery(CommandType.Text, sql, null);
        if (count == 1)
        {
            label_add.ForeColor = System.Drawing.Color.Green;
            label_add.Text = info.addLabelInfo("The market data", true);
        }
        else
        {
            label_add.Text = info.addLabelInfo("The market data", false);
        }
    }

    protected void btn_addmarket_Click(object sender, EventArgs e)
    {
        lbtn_market.Text = "Add market";
        lbtn_market.Enabled = true;
        panel_addmarket.Visible = false;
        label_add.Visible = true;

        string str_countryid = ddlist_country.SelectedItem.Value.Trim();
        string str_thisyear = tbox_thisyear.Text.Trim().Replace(',', '.');
        string str_nextyear = tbox_nextyear.Text.Trim().Replace(',', '.');
        string str_segmentid = ddlist_segment.SelectedItem.Value.Trim();
        string str_afteryear = tbox_afteryear.Text.Trim().Replace(',', '.');
        addmarket(str_countryid, str_segmentid, str_thisyear, str_nextyear, str_afteryear);

        gv_market.Columns.Clear();
        bindDataSource();
    }

    protected void btn_cancelmarket_Click(object sender, EventArgs e)
    {
        lbtn_market.Text = "Add market";
        lbtn_market.Enabled = true;
        panel_addmarket.Visible = false;
    }

    /// <summary>
    /// bind segment to dropdownlist
    /// </summary>
    protected void bindSegment()
    {
        DataSet ds_segment = sql.getSegmentInfo();
        if (ds_segment.Tables[0].Rows.Count > 0)
        {
            DataTable dt_segment = ds_segment.Tables[0];
            int countsegment = dt_segment.Rows.Count;
            int indexsegment = 0;
            while (indexsegment < countsegment)
            {
                ListItem ll = new ListItem(dt_segment.Rows[indexsegment][1].ToString().Trim(), dt_segment.Rows[indexsegment][0].ToString().Trim());
                ddlist_segment.Items.Add(ll);
                indexsegment++;
            }
            ddlist_segment.Enabled = true;
        }
        else
        {
            ddlist_segment.Items.Add(new ListItem("", "-1"));
            ddlist_segment.Enabled = false;
        }
    }

    /// <summary>
    /// bind country to dropdownlist
    /// </summary>
    protected void bindCountry()
    {
        DataSet ds_country = sql.getCountryInfo();
        if (ds_country.Tables[0].Rows.Count > 0)
        {
            DataTable dt_country = ds_country.Tables[0];
            int countcountry = dt_country.Rows.Count;
            int indexcountry = 0;
            while (indexcountry < countcountry)
            {
                ListItem ll = new ListItem(dt_country.Rows[indexcountry][2].ToString().Trim(), dt_country.Rows[indexcountry][0].ToString().Trim());
                ddlist_country.Items.Add(ll);
                indexcountry++;
            }
            ddlist_country.Enabled = true;
        }
        else
        {
            ddlist_country.Items.Add(new ListItem("", "-1"));
            ddlist_country.Enabled = false;
        }
    }

    protected void lbtn_market_Click(object sender, EventArgs e)
    {
        lbtn_market.Text = "Select country and segment, then input total market";
        lbtn_market.Enabled = false;
        panel_addmarket.Visible = true;
        null_input();

        // By DingJunjie 20110520 Item12 Delete Start
        //ddlist_country.Items.Clear();
        //bindCountry();
        // By DingJunjie 20110520 Item12 Delete End
        // By DingJunjie 20110520 Item12 Add Start
        bindDropDownList(sql.getRegionInfo(), this.ddlRegion, "Name", "ID", false);
        bindDropDownList(sql.getCluster(this.ddlRegion.SelectedValue), this.ddlCluster, "Name", "ID", false);
        bindDropDownList(sql.getCountry(this.ddlCluster.SelectedValue), this.ddlist_country, "Name", "ID", false);
        // By DingJunjie 20110520 Item12 Add End
        ddlist_segment.Items.Clear();
        bindSegment();

        label_del.Visible = false;
        label_add.Visible = false;
    }
    protected void gv_market_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        gv_market.Columns.Clear();
        gv_market.PageIndex = e.NewPageIndex;
        bindDataSource();
    }

    protected void gv_market_RowEditing(object sender, GridViewEditEventArgs e)
    {
        gv_market.Columns.Clear();
        gv_market.EditIndex = e.NewEditIndex;
        bindDataSource();

        this.panel_addmarket.Visible = false;
        this.lbtn_market.Enabled = false;
        this.lbtn_market.Text = "Add market";
        this.UpdatePanel2.Update();
    }

    /// <summary>
    /// edit one market
    /// </summary>
    /// <param name="str_id">Market ID</param>
    /// <param name="str_country">Country Name</param>
    /// <param name="str_segment">Segment Description</param>
    /// <param name="str_totalmarket">Total Market</param>
    /// <param name="str_compettitors">Compettitors</param>
    private void updatemarket(string str_countryid, string str_segmentid, string str_thisyear, string str_nextyear, string str_afteryear)
    {
        label_del.ForeColor = System.Drawing.Color.Red;
        if (str_thisyear.Trim().Length == 0)
        {
            str_thisyear = "0";
        }

        if (str_nextyear.Trim().Length == 0)
        {
            str_nextyear = "0";
        }

        if (str_afteryear.Trim().Length == 0)
        {
            str_afteryear = "0";
        }

        string sql = "UPDATE [Market] SET ThisYear = '" + str_thisyear + "'"
        // By DingJunjie 20110517 Bug Delete Start
        //           + ", NextYear = '" + str_thisyear + "', AfterYear = '" + str_afteryear + "'"
        // By DingJunjie 20110517 Bug Delete End
        // By DingJunjie 20110517 Bug Add Start
                   + ", NextYear = '" + str_nextyear + "', AfterYear = '" + str_afteryear + "'"
        // By DingJunjie 20110517 Bug Add End
                   + " WHERE SegmentID = " + str_segmentid + " AND CountryID = " + str_countryid + " AND TimeFlag = '" + date.getSetMeetingDate().Tables[0].Rows[0][0].ToString().Trim() + "'";
        int count = helper.ExecuteNonQuery(CommandType.Text, sql, null);
        if (count == 1)
        {
            label_del.ForeColor = System.Drawing.Color.Green;
            label_del.Text = info.edtLabelInfo("The market data ", true);
        }
        else
        {
            label_del.Text = info.edtLabelInfo("The market data ", false);
        }
    }

    protected void gv_market_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        label_del.Visible = true;
        label_add.Visible = false;

        string str = ddlist_find.Text.Trim();
        int sel = int.Parse(ddlist_in.SelectedItem.Value.Trim());
        DataSet ds = getmarketInfo(str, sel);

        // By DingJunjie 20110517 Bug Delete Start
        //string str_country = gv_market.Rows[e.RowIndex].Cells[0].Text.Trim();
        //string str_segment = gv_market.Rows[e.RowIndex].Cells[1].Text.Trim();
        // By DingJunjie 20110517 Bug Delete End
        // By DingJunjie 20110517 Bug Add Start
        // By DingJunjie 20110520 Item 12 Delete Start
        //string str_country = gv_market.Rows[e.RowIndex].Cells[1].Text.Trim();
        //string str_segment = gv_market.Rows[e.RowIndex].Cells[0].Text.Trim();
        // By DingJunjie 20110520 Item 12 Delete End
        // By DingJunjie 20110520 Item 12 Add Start
        string str_segment = gv_market.Rows[e.RowIndex].Cells[0].Text.Trim();
        string str_country = gv_market.Rows[e.RowIndex].Cells[3].Text.Trim();
        // By DingJunjie 20110520 Item 12 Add End
        // By DingJunjie 20110517 Bug Add End
        // By DingJunjie 20110520 Item 12 Delete Start
        //string str_thisyear = ((TextBox)(gv_market.Rows[e.RowIndex].Cells[4].Controls[0])).Text.Trim().Replace(',', '.');
        //string str_nextyear = ((TextBox)(gv_market.Rows[e.RowIndex].Cells[5].Controls[0])).Text.Trim().Replace(',', '.');
        //string str_afteryear = ((TextBox)(gv_market.Rows[e.RowIndex].Cells[6].Controls[0])).Text.Trim().Replace(',', '.');
        // By DingJunjie 20110520 Item 12 Delete End
        // By DingJunjie 20110520 Item 12 Add Start
        string str_thisyear = ((TextBox)(gv_market.Rows[e.RowIndex].Cells[8].Controls[0])).Text.Trim().Replace(',', '.');
        string str_nextyear = ((TextBox)(gv_market.Rows[e.RowIndex].Cells[9].Controls[0])).Text.Trim().Replace(',', '.');
        string str_afteryear = ((TextBox)(gv_market.Rows[e.RowIndex].Cells[10].Controls[0])).Text.Trim().Replace(',', '.');
        // By DingJunjie 20110520 Item 12 Add End
        updatemarket(str_country, str_segment, str_thisyear, str_nextyear, str_afteryear);

        gv_market.Columns.Clear();
        gv_market.EditIndex = -1;
        bindDataSource();

        this.lbtn_market.Enabled = true;
        this.UpdatePanel2.Update();
    }

    protected void gv_market_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        label_del.Visible = false;
        label_add.Visible = false;

        gv_market.Columns.Clear();
        gv_market.EditIndex = -1;
        bindDataSource();

        this.lbtn_market.Enabled = true;
        this.UpdatePanel2.Update();
    }

    /// <summary>
    /// delete one market
    /// </summary>
    /// <param name="str_id">Market ID</param>
    /// <param name="str_country">Country Name</param>
    /// <param name="str_segment">Segment Name</param>
    private void delmarket(string str_country, string str_segment)
    {
        string sql = "DELETE FROM [Market] WHERE CountryID = " + str_country + " AND SegmentID = " + str_segment;
        int count = helper.ExecuteNonQuery(CommandType.Text, sql, null);
        if (count == 1)
        {
            label_del.ForeColor = System.Drawing.Color.Green;
            label_del.Text = info.delLabelInfo("The market data ", true);
        }
        else
        {
            label_del.ForeColor = System.Drawing.Color.Red;
            label_del.Text = info.delLabelInfo("The market data ", false);
        }
    }

    protected void gv_market_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        label_del.Visible = true;
        label_add.Visible = false;

        // By DingJunjie 20110520 Item 12 Delete Start
        //string str_country = gv_market.Rows[e.RowIndex].Cells[1].Text.Trim();
        // By DingJunjie 20110517 Bug Delete Start
        //string str_segment = gv_market.Rows[e.RowIndex].Cells[2].Text.Trim();
        // By DingJunjie 20110517 Bug Delete End
        // By DingJunjie 20110517 Bug Add Start
        //string str_segment = gv_market.Rows[e.RowIndex].Cells[0].Text.Trim();
        // By DingJunjie 20110517 Bug Add End
        // By DingJunjie 20110520 Item 12 Delete End
        // By DingJunjie 20110520 Item 12 Add Start
        string str_segment = gv_market.Rows[e.RowIndex].Cells[0].Text.Trim();
        string str_country = gv_market.Rows[e.RowIndex].Cells[3].Text.Trim();
        // By DingJunjie 20110520 Item 12 Add End
        delmarket(str_country, str_segment);

        gv_market.Columns.Clear();
        bindDataSource();
    }

    protected void gv_market_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (e.Row.RowState == DataControlRowState.Normal || e.Row.RowState == DataControlRowState.Alternate)
            {
                ((ImageButton)e.Row.Cells[gv_market.Columns.Count - 1].Controls[0]).Attributes.Add("onclick", info.setRowDataBound("the market data"));
            }
        }
    }

    //Find
    FindList list = new FindList();

    protected void ddlist_in_SelectedIndexChanged(object sender, EventArgs e)
    {
        int sel = int.Parse(ddlist_in.SelectedItem.Value.Trim());
        ddlist_find.Items.Clear();
        switch (sel)
        {
            // By DingJunjie 20110520 Item 12 Delete Start
            //case 0:
            //    {
            //        list.bindFind(list.getCountryName(), ddlist_find);
            //        break;
            //    }
            //case 1:
            //    {
            //        list.bindFind(list.getSegmentAbbr(), ddlist_find);
            //        break;
            //    }
            // By DingJunjie 20110520 Item 12 Delete End
            // By DingJunjie 20110520 Item 12 Add Start
            case 0:
                {
                    bindDropDownList(sql.getRegionInfo(), this.ddlist_find, "Name", "ID", true);
                    break;
                }
            case 1:
                {
                    bindDropDownList(sql.getClusterInfo(), this.ddlist_find, "Name", "ID", true);
                    break;
                }
            case 2:
                {
                    bindDropDownList(sql.getCountryInfo(), this.ddlist_find, "Name", "ID", true);
                    break;
                }
            // By DingJunjie 20110520 Item 12 Add End
        }
    }

    #region DingJunjie Add
    // By DingJunjie 20110520 Item 12 Add Start
    /// <summary>
    /// Select Cluster By Region
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void ddlRegion_SelectedIndexChanged(object sender, EventArgs e)
    {
        bindDropDownList(sql.getCluster(this.ddlRegion.SelectedValue), this.ddlCluster, "Name", "ID", false);
    }

    /// <summary>
    /// Select Country By Cluster
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void ddlCluster_SelectedIndexChanged(object sender, EventArgs e)
    {
        bindDropDownList(sql.getCountry(this.ddlCluster.SelectedValue), this.ddlist_country, "Name", "ID", false);
    }

    /// <summary>
    /// Bind DropDownList
    /// </summary>
    /// <param name="ds">DataSet</param>
    /// <param name="ddl">DropDownList</param>
    /// <param name="label">Label Field</param>
    /// <param name="value">Value Field</param>
    /// <param name="value">Empty Item Flag</param>
    private void bindDropDownList(DataSet ds, DropDownList ddl, string label, string value, bool emptyFlag)
    {
        if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        {
            ddl.Items.Clear();
            ddl.DataSource = ds.Tables[0];
            ddl.DataTextField = label;
            ddl.DataValueField = value;
            ddl.DataBind();
        }
        if (emptyFlag)
        {
            ddl.Items.Insert(0, new ListItem());
        }
    }
    // By DingJunjie 20110520 Item 17 Add End
    #endregion
}
