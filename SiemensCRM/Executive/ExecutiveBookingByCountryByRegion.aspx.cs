/*
 * File Name   : AdminBookingByCountryByRegion.aspx.cs
 * 
 * Description : search booking data by key country by region
 * 
 * Author      : Wang Jun
 * 
 * Modify Date : 2010-03-08
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

public partial class Executive_ExecutiveBookingByCountryByRegion: System.Web.UI.Page
{
    SQLHelper helper = new SQLHelper();
    LogUtility log = new LogUtility();
    SQLStatement sql = new SQLStatement();
    //by ryzhang item49 20110519 del start 
    //GetMeetingDate meeting = new GetMeetingDate();
    //by ryzhang item49 20110519 del end 
    //by ryzhang item49 20110519 add start 
    GetSelectMeetingDate meeting = new GetSelectMeetingDate();
    //by ryzhang item49 20110519 add end 
    DisplayInfo info = new DisplayInfo();
    WebUtility web = new WebUtility();
    CommonFunction cf = new CommonFunction();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (getRoleID(getRole()) != "1")
            Response.Redirect("~/AccessDenied.aspx");

        if (!IsPostBack)
        {
            log.WriteLog(LogUtility.LogErrorLevel.LOG_ACCESS, "ExecutiveBookingByCountryByRegion Access.");
            //by ryzhang item49 20110519 del start 
            //meeting.setDate();
            //by ryzhang item49 20110519 del end 
            //by ryzhang item49 20110519 add start 
            meeting.setSelectDate(Session["ExecutiveID"].ToString());
            //by ryzhang item49 20110519 add end 
            bind(getRegionInfo(), 1);
            bind(getSegmentInfo(), 2);
            btn_export.Visible = false;
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

    protected DataSet getSegmentInfo()
    {
        string query_segment = "SELECT ID,Description"
                            + " FROM [Segment] "
                            + " WHERE Deleted = 0"
                            + " ORDER BY Description ASC";
        DataSet ds_segment = helper.GetDataSet(query_segment);

        if ((ds_segment.Tables.Count > 0) && (ds_segment.Tables[0].Rows.Count > 0))
        {
            return ds_segment;
        }
        else
        {
            return null;
        }

    }

    protected DataSet getRegionInfo()
    {
        string query_region = "SELECT ID,Name"
                            + " FROM [Region]"
                            + " WHERE Deleted = 0"
                            + " ORDER BY Name ASC";
        DataSet ds_region = helper.GetDataSet(query_region);

        if ((ds_region.Tables.Count > 0) && (ds_region.Tables[0].Rows.Count > 0))
        {
            return ds_region;
        }
        else
        {
            return null;
        }
    }

    protected void bind(DataSet ds, int sel)//1:region  2:Segment
    {
        if (ds != null)
        {
            DataTable dt = ds.Tables[0];
            int count = dt.Rows.Count;
            int index = 0;
            while (index < count)
            {
                if (sel == 2)
                {
                    ListItem li = new ListItem(dt.Rows[index][1].ToString().Trim(), dt.Rows[index][0].ToString().Trim());
                    ddlist_segment.Items.Add(li);
                }
                if (sel == 1)
                {
                    ListItem li = new ListItem(dt.Rows[index][1].ToString().Trim(), dt.Rows[index][0].ToString().Trim());
                    ddlist_region.Items.Add(li);
                }
                index++;
            }
            if (sel == 2)
            {
                ddlist_segment.SelectedValue = Request.QueryString["SegmentID"]; ;
                ListItem li = new ListItem("Group Total", "-1");
                ddlist_segment.Items.Add(li);
                ddlist_segment.Enabled = true;
            }
            if (sel == 1)
            {
                ddlist_region.SelectedIndex = 0;
                ListItem li = new ListItem("Group Region Total", "-1");
                ddlist_region.Items.Add(li);
                ddlist_region.Enabled = true;
            }
        }
        else
        {
            if (sel == 2)
            {
                ddlist_segment.Enabled = false;
                btn_search.Enabled = false;
            }
            if (sel == 1)
            {
                ddlist_region.Enabled = false;
                ddlist_region.Items.Add("");
                btn_search.Enabled = false;
            }
        }
    }

    protected void btn_search_Click(object sender, EventArgs e)
    {
        label_segmentDec.Text = ddlist_segment.SelectedItem.Text.Trim();
        label_description.Text = "Sum of New Orders(EUR) On " + meeting.getyear() + "-" + meeting.getmonth();

        bindDataSource();

        btn_export.Visible = true;
    }

    protected DataSet getBookingDataByCountry(string str_subregionID, string str_segmentID)
    {
        return sql.getBookingDataByCountry(str_subregionID, str_segmentID);
    }

    protected DataSet getClusterIDIDByRegionID(string str_regionID)
    {
        string sql = "SELECT [Cluster].ID,[Cluster].Name FROM [Region_Cluster] "
                    + " INNER JOIN [Cluster] ON [Cluster].ID = [Region_Cluster].ClusterID"
                    + " WHERE RegionID = " + str_regionID
                    + " AND [Region_Cluster].Deleted = 0 AND [Cluster].Deleted = 0"
                    + " GROUP BY [Cluster].ID,[Cluster].Name"
                    + " ORDER BY [Cluster].Name ASC";
        DataSet ds = helper.GetDataSet(sql);
        if (ds.Tables[0].Rows.Count > 0)
            return ds;
        else
            return null;
    }

    protected GridView bindDataByCountry(GridView gv, DataSet ds, string header)
    {
        if (ds != null)
        {
            gv.Visible = true;
            gv.Width = Unit.Pixel(650);
            gv.AutoGenerateColumns = false;
            gv.AllowPaging = false;

            for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
            {
                BoundField bf = new BoundField();

                bf.DataField = ds.Tables[0].Columns[i].ColumnName.ToString();
                bf.HeaderText = ds.Tables[0].Columns[i].Caption.ToString();
                bf.ItemStyle.HorizontalAlign = HorizontalAlign.Right;
                bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Right;

                if (i == 0)
                {
                    bf.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
                    bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
                    if (header != "")
                        bf.ItemStyle.Width = 250;
                }

                bf.ReadOnly = true;
                gv.Columns.Add(bf);
            }

            if (header != "")
            {
                gv.Caption = "<br />" + header;
                gv.CaptionAlign = TableCaptionAlign.Left;
                gv.AllowSorting = true;
                gv.DataSource = ds.Tables[0];

                DataRow drSum = ds.Tables[0].NewRow();
                float[] Sum = new float[50];
                for (int j = 1; j < ds.Tables[0].Columns.Count; j++)
                {
                    Sum[j] = 0;
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        Sum[j] += float.Parse(ds.Tables[0].Rows[i][j].ToString());
                    }
                }

                for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
                {
                    if (i == 0)
                    {
                        if (header.Substring(0, 3) != "Tot")
                            drSum[0] = "Total(" + header + ")";
                        else
                            drSum[0] = header;
                    }
                    else
                    {
                        drSum[i] = Sum[i].ToString();
                    }
                }
                ds.Tables[0].Rows.InsertAt(drSum, ds.Tables[0].Rows.Count);

                gv.DataBind();
                return gv;
            }
            else
            {
                gv.Caption = "<br />" + "GROUP REGION";
                gv.CaptionAlign = TableCaptionAlign.Left;
                gv.DataSource = ds.Tables[0];
                gv.DataBind();
                return gv;
            }
        }
        else
        {
            gv.Visible = false;
            return gv;
        }
    }

    protected void bindDataSource()
    {
        // by daixuesong  20110530  Item22 add start
        int index = 0;
        // by daixuesong  20110530  Item22 add end
        if (ddlist_region.SelectedItem.Value != "-1" && ddlist_segment.SelectedItem.Value != "-1")
        {
            DataSet dsSubID = getClusterIDIDByRegionID(ddlist_region.SelectedItem.Value);

            if (dsSubID != null)
            {
                DataSet[] ds = new DataSet[dsSubID.Tables[0].Rows.Count];
                GridView[] gv = new GridView[dsSubID.Tables[0].Rows.Count];
                DataSet dsTotal = new DataSet();
                GridView gvTotal = new GridView();

                TableRow tr = new TableRow();
                table_bookingbycountry.Rows.Add(tr);
                table_bookingbycountry.Visible = true;

                TableCell tc = new TableCell();
                tc.HorizontalAlign = HorizontalAlign.Left;
                tc.VerticalAlign = VerticalAlign.Top;

                for (int count = 0; count < dsSubID.Tables[0].Rows.Count; count++)
                {
                    string str_SubID = dsSubID.Tables[0].Rows[count][0].ToString().Trim();
                    string str_SubName = dsSubID.Tables[0].Rows[count][1].ToString().Trim();

                    //New the instance to this controls
                    gv[count] = new GridView();
                    web.setProperties(gv[count]); ;

                    ds[count] = getBookingDataByCountry(str_SubID, ddlist_segment.SelectedItem.Value);
                    if (ds[count] != null)
                    {
                        if (ds[count].Tables.Count > 0)
                        {
                            gv[count] = bindDataByCountry(gv[count], ds[count], str_SubName);
                            // by daixuesong  20110530  Item22 add start
                            gv[count].Style.Clear();
                            gv[count].Style.Add("border", "#000 solid 1px");
                            gv[count].Style.Add("border-collapse", "collapse");
                            gv[count].Style.Add("font-size", "12px");
                            gv[count].HeaderRow.Style.Add("background", "#000");
                            for (int i = 0; i < gv[count].Rows.Count; i++)
                            {
                                foreach (TableCell cell in gv[count].Rows[i].Cells)
                                {
                                    cell.Style.Clear();
                                    if (index % 2 == 0)
                                    {
                                        cell.Style.Add("background", "#FF9");
                                    }
                                    else
                                    {

                                        cell.Style.Add("background", "#ccffff");

                                    }

                                }
                            }

                            index++;
                            // by daixuesong  20110530  Item22 add end
                            tc.Controls.Add(gv[count]);
                        }
                        else
                        {
                            log.WriteLog(LogUtility.LogErrorLevel.LOG_CRITICAL, string.Format("Error:ds.Tables.Count is {0}", dsSubID.Tables.Count));
                            Response.Redirect("~/Admin/AdminError.aspx");
                        }
                    }
                }
                gvTotal = new GridView();
                web.setProperties(gvTotal); ;

                dsTotal = getBookingDataTotalByCountry(ddlist_region.SelectedItem.Value, ddlist_segment.SelectedItem.Value);
                if (dsTotal != null)
                {
                    if (dsTotal.Tables.Count > 0)
                    {
                        gvTotal = bindDataByCountry(gvTotal, dsTotal, "Total(" + ddlist_region.SelectedItem.Text + ")");
                        // by daixuesong  20110530  Item22 add start

                        gvTotal.Style.Clear();
                        gvTotal.Style.Add("border", "#000 solid 1px");
                        gvTotal.Style.Add("border-collapse", "collapse");
                        gvTotal.Style.Add("font-size", "12px");

                        gvTotal.HeaderRow.Style.Add("background", "#000");
                        for (int i = 0; i < gvTotal.Rows.Count; i++)
                        {
                            foreach (TableCell cell in gvTotal.Rows[i].Cells)
                            {
                                cell.Style.Clear();
                                if (index % 2 == 0)
                                {
                                    cell.Style.Add("background", "#FF9");
                                }
                                else
                                {
                                    cell.Style.Add("background", "#ccffff");
                                }
                            }
                        }

                        // by daixuesong  20110530  Item22 add end
                        tc.Controls.Add(gvTotal);
                    }
                    else
                    {
                        log.WriteLog(LogUtility.LogErrorLevel.LOG_CRITICAL, string.Format("Error:ds.Tables.Count is {0}", dsTotal.Tables.Count));
                        Response.Redirect("~/Admin/AdminError.aspx");
                    }
                }
                tr.Controls.Add(tc);
            }
        }
        else if (ddlist_region.SelectedItem.Value == "-1" && ddlist_segment.SelectedItem.Value != "-1")
        {
            DataSet ds_region = getRegionInfo();
            DataSet[] dsTotal = new DataSet[ds_region.Tables[0].Rows.Count];
            GridView[] gvTotal = new GridView[ds_region.Tables[0].Rows.Count];
            GridView gv = new GridView();
            DataSet ds = new DataSet();

            TableRow tr = new TableRow();
            table_bookingbycountry.Rows.Add(tr);
            table_bookingbycountry.Visible = true;

            TableCell tc = new TableCell();
            tc.HorizontalAlign = HorizontalAlign.Left;
            tc.VerticalAlign = VerticalAlign.Top;
            for (int i = 0; i < ds_region.Tables[0].Rows.Count; i++)
            {
                string regionID = ds_region.Tables[0].Rows[i][0].ToString().Trim();
                string regionName = ds_region.Tables[0].Rows[i][1].ToString().Trim();

                gvTotal[i] = new GridView();
                web.setProperties(gvTotal[i]);
                dsTotal[i] = getBookingDataTotalByCountry(regionID, ddlist_segment.SelectedItem.Value);
                if (dsTotal[i] != null)
                {
                    if (dsTotal[i].Tables.Count > 0)
                    {
                        gvTotal[i] = bindDataByCountry(gvTotal[i], dsTotal[i], "Total(" + regionName + ")");
                        // by daixuesong  20110530  Item22 add start

                        gvTotal[i].Style.Clear();
                        gvTotal[i].Style.Add("border", "#000 solid 1px");
                        gvTotal[i].Style.Add("border-collapse", "collapse");
                        gvTotal[i].Style.Add("font-size", "12px");

                        gvTotal[i].HeaderRow.Style.Add("background", "#000");
                        for (int j = 0; j < gvTotal[i].Rows.Count; j++)
                        {
                            foreach (TableCell cell in gvTotal[i].Rows[j].Cells)
                            {
                                cell.Style.Clear();
                                if (index % 2 == 0)
                                {
                                    cell.Style.Add("background", "#FF9");
                                }
                                else
                                {
                                    cell.Style.Add("background", "#ccffff");
                                }
                            }
                        }

                        // by daixuesong  20110530  Item22 add end
                        index++;
                        tc.Controls.Add(gvTotal[i]);
                    }
                    else
                    {
                        log.WriteLog(LogUtility.LogErrorLevel.LOG_CRITICAL, string.Format("Error:ds.Tables.Count is {0}", dsTotal[i].Tables.Count));
                        Response.Redirect("~/Admin/AdminError.aspx");
                    }
                }
            }
            gv = new GridView();
            web.setProperties(gv);
            ds = getTotalRegion(ddlist_segment.SelectedItem.Value);
            if (ds != null)
            {
                if (ds.Tables.Count > 0)
                {
                    gv = bindDataByCountry(gv, ds, "");
                    // by daixuesong  20110530  Item22 add start

                    gv.Style.Clear();
                    gv.Style.Add("border", "#000 solid 1px");
                    gv.Style.Add("border-collapse", "collapse");
                    gv.Style.Add("font-size", "12px");

                    gv.HeaderRow.Style.Add("background", "#000");
                    for (int i = 0; i < gv.Rows.Count; i++)
                    {
                        foreach (TableCell cell in gv.Rows[i].Cells)
                        {
                            cell.Style.Clear();
                            if (index % 2 == 0)
                            {
                                cell.Style.Add("background", "#FF9");
                            }
                            else
                            {
                                cell.Style.Add("background", "#ccffff");
                            }
                        }
                    }

                    // by daixuesong  20110530  Item22 add end

                    tc.Controls.Add(gv);
                }
                else
                {
                    log.WriteLog(LogUtility.LogErrorLevel.LOG_CRITICAL, string.Format("Error:ds.Tables.Count is {0}", ds.Tables.Count));
                    Response.Redirect("~/Admin/AdminError.aspx");
                }
            }

            tr.Controls.Add(tc);
        }
        else if (ddlist_region.SelectedItem.Value != "-1" && ddlist_segment.SelectedItem.Value == "-1")
        {
            DataSet dsSubID = getClusterIDIDByRegionID(ddlist_region.SelectedItem.Value);

            if (dsSubID != null)
            {
                DataSet[] ds = new DataSet[dsSubID.Tables[0].Rows.Count];
                GridView[] gv = new GridView[dsSubID.Tables[0].Rows.Count];
                DataSet dsTotal = new DataSet();
                GridView gvTotal = new GridView();

                TableRow tr = new TableRow();
                table_bookingbycountry.Rows.Add(tr);
                table_bookingbycountry.Visible = true;

                TableCell tc = new TableCell();
                tc.HorizontalAlign = HorizontalAlign.Left;
                tc.VerticalAlign = VerticalAlign.Top;

                for (int count = 0; count < dsSubID.Tables[0].Rows.Count; count++)
                {
                    string str_SubID = dsSubID.Tables[0].Rows[count][0].ToString().Trim();
                    string str_SubName = dsSubID.Tables[0].Rows[count][1].ToString().Trim();

                    //New the instance to this controls
                    gv[count] = new GridView();
                    web.setProperties(gv[count]); ;

                    ds[count] = getBookingDataByCountry(str_SubID, "-1");
                    if (ds[count] != null)
                    {
                        if (ds[count].Tables.Count > 0)
                        {
                            gv[count] = bindDataByCountry(gv[count], ds[count], str_SubName);
                            // by daixuesong  20110530  Item22 add start
                            gv[count].Style.Clear();
                            gv[count].Style.Add("border", "#000 solid 1px");
                            gv[count].Style.Add("border-collapse", "collapse");
                            gv[count].Style.Add("font-size", "12px");
                            gv[count].HeaderRow.Style.Add("background", "#000");
                            for (int i = 0; i < gv[count].Rows.Count; i++)
                            {
                                foreach (TableCell cell in gv[count].Rows[i].Cells)
                                {
                                    cell.Style.Clear();
                                    if (index % 2 == 0)
                                    {
                                        cell.Style.Add("background", "#FF9");
                                    }
                                    else
                                    {

                                        cell.Style.Add("background", "#ccffff");

                                    }

                                }
                            }

                            index++;
                            // by daixuesong  20110530  Item22 add end
                            tc.Controls.Add(gv[count]);
                        }
                        else
                        {
                            log.WriteLog(LogUtility.LogErrorLevel.LOG_CRITICAL, string.Format("Error:ds.Tables.Count is {0}", dsSubID.Tables.Count));
                            Response.Redirect("~/Admin/AdminError.aspx");
                        }
                    }
                }
                gvTotal = new GridView();
                web.setProperties(gvTotal); ;

                dsTotal = getBookingDataTotalByCountry(ddlist_region.SelectedItem.Value, ddlist_segment.SelectedItem.Value);
                if (dsTotal != null)
                {
                    if (dsTotal.Tables.Count > 0)
                    {
                        gvTotal = bindDataByCountry(gvTotal, dsTotal, "Total(" + ddlist_region.SelectedItem.Text + ")");
                        // by daixuesong  20110530  Item22 add start

                        gvTotal.Style.Clear();
                        gvTotal.Style.Add("border", "#000 solid 1px");
                        gvTotal.Style.Add("border-collapse", "collapse");
                        gvTotal.Style.Add("font-size", "12px");

                        gvTotal.HeaderRow.Style.Add("background", "#000");
                        for (int i = 0; i < gvTotal.Rows.Count; i++)
                        {
                            foreach (TableCell cell in gvTotal.Rows[i].Cells)
                            {
                                cell.Style.Clear();
                                if (index % 2 == 0)
                                {
                                    cell.Style.Add("background", "#FF9");
                                }
                                else
                                {
                                    cell.Style.Add("background", "#ccffff");
                                }
                            }
                        }
                        // by daixuesong  20110530  Item22 add end
                        tc.Controls.Add(gvTotal);
                    }
                    else
                    {
                        log.WriteLog(LogUtility.LogErrorLevel.LOG_CRITICAL, string.Format("Error:ds.Tables.Count is {0}", dsTotal.Tables.Count));
                        Response.Redirect("~/Admin/AdminError.aspx");
                    }
                }
                tr.Controls.Add(tc);
            }
        }
        else
        {
            GridView gv = new GridView();
            DataSet dsTotal = new DataSet();

            TableRow tr = new TableRow();
            table_bookingbycountry.Rows.Add(tr);
            table_bookingbycountry.Visible = true;

            TableCell tc = new TableCell();
            tc.HorizontalAlign = HorizontalAlign.Left;
            tc.VerticalAlign = VerticalAlign.Top;

            //New the instance to this controls
            gv = new GridView();
            web.setProperties(gv);

            dsTotal = getTotalRegion(ddlist_segment.SelectedItem.Value);
            if (dsTotal != null)
            {
                if (dsTotal.Tables.Count > 0)
                {
                    gv = bindDataByCountry(gv, dsTotal, "");
                    // by daixuesong  20110530  Item22 add start

                    gv.Style.Clear();
                    gv.Style.Add("border", "#000 solid 1px");
                    gv.Style.Add("border-collapse", "collapse");
                    gv.Style.Add("font-size", "12px");

                    gv.HeaderRow.Style.Add("background", "#000");
                    for (int i = 0; i < gv.Rows.Count; i++)
                    {
                        foreach (TableCell cell in gv.Rows[i].Cells)
                        {
                            cell.Style.Clear();
                            if (index % 2 == 0)
                            {
                                cell.Style.Add("background", "#FF9");
                            }
                            else
                            {
                                cell.Style.Add("background", "#ccffff");
                            }
                        }
                    }

                    // by daixuesong  20110530  Item22 add end
                    tc.Controls.Add(gv);
                }
                else
                {
                    log.WriteLog(LogUtility.LogErrorLevel.LOG_CRITICAL, string.Format("Error:ds.Tables.Count is {0}", dsTotal.Tables.Count));
                    Response.Redirect("~/Admin/AdminError.aspx");
                }
            }
            tr.Controls.Add(tc);
        }
    }

    protected DataSet getBookingDataTotalByCountry(string str_regionID, string str_segmentID)
    {
        return sql.getBookingDataTotalByCountry(str_regionID, str_segmentID);
    }

    protected DataSet getTotalRegion(string str_segmentID)
    {
        return sql.getTotalRegion(str_segmentID);
    }

    #region
    public override void VerifyRenderingInServerForm(Control control)
    {
        // Confirms that an HtmlForm control is rendered for
    }

    protected void btn_export_Click(object sender, EventArgs e)
    {
        string meetingd = string.Empty;
        string query_date = "SELECT CONVERT(varchar(15),SelectMeetingDate,23) FROM [SetSelectMeetingDate] where userid=" + Session["ExecutiveID"].ToString();
        DataSet ds_date = helper.GetDataSet(query_date);
        if (ds_date.Tables[0].Rows.Count > 0 && !ds_date.Tables[0].Rows[0][0].ToString().Equals("") && ds_date.Tables[0].Rows[0][0].ToString() != null)
        {
            meetingd = "This report is related to the meeting date " + ds_date.Tables[0].Rows[0][0].ToString();
        }
        else
        {
            meetingd = "There is no meeting date selected!";
        }
        TableRow tr5 = new TableRow();
        table_bookingbycountry.Rows.Add(tr5);
        TableCell tc5 = new TableCell();
        tc5.Text = meetingd;
        tr5.Controls.Add(tc5);
        bindDataSource();
        cf.ToExcel(table_bookingbycountry, "BookingsByKeyCountries(" + ddlist_segment.SelectedItem.Text + " IN " + ddlist_region.SelectedItem.Text + ").xls");
    }
    #endregion
}
