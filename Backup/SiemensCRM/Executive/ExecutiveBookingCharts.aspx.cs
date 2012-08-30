/*
* File Name   : ExecutiveBookingChart.aspx.cs
* 
* Description : get booking charts
* 
* Author      : Wang Jun
* 
* Modify Date : 2010-04-01
* 
* Problem     : none
* 
 * Version    : Release (1.0)
*/

using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using Microsoft.Office.Interop.Owc11;
using System.IO;

public partial class Executive_ExecutiveBookingCharts : System.Web.UI.Page
{
    SQLHelper helper = new SQLHelper();
    LogUtility log = new LogUtility();
    DisplayInfo info = new DisplayInfo();
    WebUtility web = new WebUtility();
    SQLStatement sql = new SQLStatement();

    string dateI;
    int region_number = 0;
    bool flag;
    ArrayList al = new ArrayList();

    string[] periodName = new string[4];
    bool isNullOrNot;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (getRoleID(getRole()) != "1")
            Response.Redirect("~/AccessDenied.aspx");

        if (!IsPostBack)
        {
            isNullOrNot = true;
            setDate();
            Image1.Visible = false;
            Image2.Visible = false;
            Image3.Visible = false;
            Image6.Visible = false;
            Image7.Visible = false;
            Image8.Visible = false;
            btn_show.Visible = true;
            bind(getSegmentName());
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

    /*set period suffixes*/
    protected void setPeriodArray()
    {
        periodName[0] = year.Substring(2, 2) + "  A";
        periodName[1] = year.Substring(2, 2) + "  B";
        periodName[2] = nextyear.Substring(2, 2) + "  B";
        periodName[3] = nextyear.Substring(2, 2) + "  F";
    }

    /*set date*/
    protected static string yearBeforePre;
    protected static string preyear;
    protected static string year;
    protected static string nextyear;
    protected static string yearAfterNext;
    protected static string month;
    protected const string fiscalStart = "Oct.1";
    protected const string fiscalEnd = "Sept.30";



    protected void setDate()
    {
        year = getMeetingDateYear();
        month = getMeetingDateMonth();

        preyear = (int.Parse(year) - 1).ToString();
        yearBeforePre = (int.Parse(preyear) - 1).ToString();
        nextyear = (int.Parse(year) + 1).ToString();
        yearAfterNext = (int.Parse(nextyear) + 1).ToString();
    }
    protected string getMeetingDateYear()
    {
        //by ryzhang item49 20110519 del start 
        //string query_meetingyear = "SELECT YEAR(MeetingDate) FROM [SetMeetingDate]";
        //by ryzhang item49 20110519 del end 
        //by ryzhang item49 20110519 add start 
        string query_meetingyear = "SELECT YEAR(SelectMeetingDate) FROM [SetSelectMeetingDate] where userid=" + Session["ExecutiveID"].ToString();
        //by ryzhang item49 20110519 add end 
        DataSet ds_year = helper.GetDataSet(query_meetingyear);

        if (ds_year.Tables.Count > 0)
        {
            if (ds_year.Tables[0].Rows.Count > 0)
            {
                string str_year = ds_year.Tables[0].Rows[0][0].ToString();
                return str_year;
            }
        }
        return null;
    }
    protected string getMeetingDateMonth()
    {
        //by ryzhang item49 20110519 del start 
        //string query_meetingmonth = "SELECT MONTH(MeetingDate) FROM [SetMeetingDate]";
        //by ryzhang item49 20110519 del end 
        //by ryzhang item49 20110519 add start 
        string query_meetingmonth = "SELECT MONTH(SelectMeetingDate) FROM [SetSelectMeetingDate] where userid=" + Session["ExecutiveID"].ToString();
        //by ryzhang item49 20110519 add end 
        DataSet ds_month = helper.GetDataSet(query_meetingmonth);

        if (ds_month.Tables.Count > 0)
        {
            if (ds_month.Tables[0].Rows.Count > 0)
            {
                string str_month = ds_month.Tables[0].Rows[0][0].ToString();
                return str_month;
            }
        }
        return null;
    }
    protected string getMeetingDateDay()
    {
        //by ryzhang item49 20110519 del start 
        //string query_meetingday = "SELECT Day(MeetingDate) FROM [SetMeetingDate]";
        //by ryzhang item49 20110519 del end 
        //by ryzhang item49 20110519 add start 
        string query_meetingday = "SELECT Day(SelectMeetingDate) FROM [SetSelectMeetingDate] where userid=" + Session["ExecutiveID"].ToString();
        //by ryzhang item49 20110519 add end 
        DataSet ds_day = helper.GetDataSet(query_meetingday);

        if (ds_day.Tables.Count > 0)
        {
            if (ds_day.Tables[0].Rows.Count > 0)
            {
                string str_day = ds_day.Tables[0].Rows[0][0].ToString();
                return str_day;
            }
        }
        return null;
    }
    protected DataSet getBookingDataByCountry(string str_ClusterID, string str_segmentID)
    {/////////////MONTH  10月份吗
        string sql = "";
        if (str_segmentID != "-1")
        {
            sql = "SELECT [Country].Name, "
                        + " SUM(CASE WHEN BookingY = '" + yearBeforePre.Substring(2, 2) + "' and YEAR(TimeFlag) = '" + yearBeforePre + "' and MONTH(TimeFlag) = '10' THEN Amount ELSE 0 END) AS 'FY " + yearBeforePre + "',"
                        + " SUM(CASE WHEN BookingY = '" + preyear.Substring(2, 2) + "' and YEAR(TimeFlag) = '" + preyear + "' and MONTH(TimeFlag) = '10' THEN Amount ELSE 0 END) AS 'FY " + preyear + "',"
                        + " SUM(CASE WHEN BookingY = '" + year.Substring(2, 2) + "' and YEAR(TimeFlag) = '" + year + "' and MONTH(TimeFlag) = '10' THEN Amount ELSE 0 END) AS 'FY " + year + "',"
                        + " SUM(CASE WHEN BookingY = '" + nextyear.Substring(2, 2) + "' and YEAR(TimeFlag) = '" + year + "' and MONTH(TimeFlag) = '10' THEN Amount ELSE 0 END) AS 'FY " + nextyear + "' "
                        + " FROM [Bookings]"
                        + " INNER JOIN [Country] ON [Country].ID = [Bookings].CountryID"
                        + " WHERE CountryID IN (SELECT CountryID FROM [Cluster_Country] WHERE ClusterID = " + str_ClusterID + " AND Deleted=0)"
                        + " AND SegmentID = " + str_segmentID
                        + " AND [Country].Deleted=0 "
                        + " GROUP BY [Country].Name"
                        + " ORDER BY [Country].Name";
        }
        else
        {
            sql = "SELECT [Country].Name, "
                        + " SUM(CASE WHEN BookingY = '" + yearBeforePre.Substring(2, 2) + "' and YEAR(TimeFlag) = '" + yearBeforePre + "' and MONTH(TimeFlag) = '10' THEN Amount ELSE 0 END) AS 'FY " + yearBeforePre + "',"
                        + " SUM(CASE WHEN BookingY = '" + preyear.Substring(2, 2) + "' and YEAR(TimeFlag) = '" + preyear + "' and MONTH(TimeFlag) = '10' THEN Amount ELSE 0 END) AS 'FY " + preyear + "',"
                        + " SUM(CASE WHEN BookingY = '" + year.Substring(2, 2) + "' and YEAR(TimeFlag) = '" + year + "' and MONTH(TimeFlag) = '10' THEN Amount ELSE 0 END) AS 'FY " + year + "',"
                        + " SUM(CASE WHEN BookingY = '" + nextyear.Substring(2, 2) + "' and YEAR(TimeFlag) = '" + year + "' and MONTH(TimeFlag) = '10' THEN Amount ELSE 0 END) AS 'FY " + nextyear + "' "
                        + " FROM [Bookings]"
                        + " INNER JOIN [Country] ON [Country].ID = [Bookings].CountryID"
                        + " WHERE CountryID IN (SELECT CountryID FROM [Cluster_Country] WHERE ClusterID = " + str_ClusterID + " AND Deleted=0)"
                        + " AND [Country].Deleted=0 "
                        + " GROUP BY [Country].Name"
                        + " ORDER BY [Country].Name";
        }
        DataSet ds = helper.GetDataSet(sql);

        if (ds.Tables[0].Rows.Count > 0)
            return ds;
        else
            return null;
    }

    protected void deleteFile()
    {
        string dir = Server.MapPath("~/ChartImages");
        foreach (string d in Directory.GetFileSystemEntries(dir))
        {
            if (File.Exists(d))
            {
                FileInfo fi = new FileInfo(d);
                if (fi.Attributes.ToString().IndexOf("ReadOnly") != -1)
                    fi.Attributes = FileAttributes.Normal;
                File.Delete(d);
            }

        }
    }
    protected void btn_show_Click(object sender, EventArgs e)
    {


        gv_bookingSegment.Columns.Clear();
        gv_totalBookingSegment.Columns.Clear();
        setPeriodArray();
        CountRegionNumber();
        Drawtable();
        // FileInfo f1=new FileInfo(Server.MapPath("~/ChartImages/")) 
        deleteFile();
        if (SegmentExistOrNot() == true || ddlist_segment.SelectedItem.Text.ToString() == "Total")
        {
            DrawPieChart();
            DrawHistoGram();
            panel_visible.Visible = true;
        }
        else
            panel_visible.Visible = false;

    }
    /*Draw HistoGram */
    protected void DrawHistoGram()
    {
        ChartSpace objSpace = new ChartSpace();
        ChChart objChart = objSpace.Charts.Add(0);
        objChart.Type = ChartChartTypeEnum.chChartTypeColumnClustered;

        objChart.HasTitle = true;
        objChart.Title.Font.Size = 12;
        objChart.Title.Caption = GetTitle(5);
        objChart.Title.Font.Bold = true;
        objChart.SeriesCollection.Add(0);
        objChart.SeriesCollection.Add(1);
        objChart.HasLegend = true;

        objChart.SeriesCollection[0].Caption = year.Substring(2, 2) + " A";

        objChart.SeriesCollection[0].SetData(ChartDimensionsEnum.chDimSeriesNames,
                +Convert.ToInt32(ChartSpecialDataSourcesEnum.chDataLiteral), year.Substring(2, 2) + " A");
        objChart.SeriesCollection[0].SetData(ChartDimensionsEnum.chDimCategories,
               +(int)ChartSpecialDataSourcesEnum.chDataLiteral, GetDimCategories(3));
        objChart.SeriesCollection[0].SetData(ChartDimensionsEnum.chDimValues,
                +(int)ChartSpecialDataSourcesEnum.chDataLiteral, GetDimValues(5));

        objChart.SeriesCollection[1].Caption = nextyear.Substring(2, 2) + " F";

        objChart.SeriesCollection[1].SetData(ChartDimensionsEnum.chDimSeriesNames,
                +Convert.ToInt32(ChartSpecialDataSourcesEnum.chDataLiteral), nextyear.Substring(2, 2) + " F");
        objChart.SeriesCollection[1].SetData(ChartDimensionsEnum.chDimCategories,
               +(int)ChartSpecialDataSourcesEnum.chDataLiteral, GetDimCategories(3));
        objChart.SeriesCollection[1].SetData(ChartDimensionsEnum.chDimValues,
                +(int)ChartSpecialDataSourcesEnum.chDataLiteral, GetDimValues(6));

        Random ro = new Random();
        int a = ro.Next();
        dateI = a.ToString();


        string fileName = "pchart8" + dateI + ".gif";
        string filePath = Server.MapPath("~");
        filePath += "\\ChartImages\\" + fileName;
        objSpace.ExportPicture(filePath, "GIF", 250, 200);
        Image8.ImageUrl = filePath.ToString();
        Image8.ImageUrl = "~/ChartImages/" + fileName;
        Image8.Visible = true;

    }
    /*Draw PieChart */
    protected void DrawPieChart()
    {
        Image[] thisimage = { Image1, Image2, Image7, Image6, Image3 };
        string[] fileName = new string[5];
        for (int i = 0; i < 5; i++)
        {
            ChartSpace objSpace = new ChartSpace();
            ChChart objChart = objSpace.Charts.Add(0);
            objChart.Type = ChartChartTypeEnum.chChartTypePie3D;//chChartTypeColumnClustered
            objChart.HasTitle = true;
            objChart.Title.Font.Size = 12;
            objChart.Title.Caption = GetTitle(i);
            objChart.Title.Font.Bold = true;
            objChart.SeriesCollection.Add(0);
            objChart.HasLegend = true;

            objChart.SeriesCollection[0].DataLabelsCollection.Add();
            objChart.SeriesCollection[0].DataLabelsCollection[0].HasValue = false;
            objChart.SeriesCollection[0].DataLabelsCollection[0].HasPercentage = true;
            objChart.Legend.Position = ChartLegendPositionEnum.chLegendPositionAutomatic;

            string dimCatgories = GetDimCategories(i);
            string dimVales = GetDimValues(i);

            objChart.SeriesCollection[0].SetData(ChartDimensionsEnum.chDimCategories,
                   +(int)ChartSpecialDataSourcesEnum.chDataLiteral, dimCatgories);


            objChart.SeriesCollection[0].SetData(ChartDimensionsEnum.chDimValues,
                    +(int)ChartSpecialDataSourcesEnum.chDataLiteral, dimVales);

            Random ro = new Random();
            int a = ro.Next();
            dateI = a.ToString();


            fileName[i] = "pchart" + i.ToString() + dateI + ".gif";
            string filePath = Server.MapPath("~");
            filePath = filePath + @"\ChartImages\" + fileName[i];
            objSpace.ExportPicture(filePath, "gif", 250, 200);
            thisimage[i].ImageUrl = "~/ChartImages/" + fileName[i];
            if (dimVales != null)
            {
                thisimage[i].Visible = true;
            }
            thisimage[i].Dispose();
        }

    }
    protected bool SegmentExistOrNot()
    {
        string query = " SELECT Count(Amount) FROM [Bookings]"
                     + " WHERE [Bookings].SegmentID='" + ddlist_segment.SelectedItem.Value.ToString() + "'";
        DataSet ds = helper.GetDataSet(query);
        if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0 && (int)ds.Tables[0].Rows[0][0] != 0)
        {
            return true;
        }
        else
            return false;
    }
    protected string GetTitle(int number)
    {

        string str_segment = ddlist_segment.SelectedItem.Text.Trim();
        if (str_segment != "Total")
        {
            switch (number)
            {
                case 0:
                    return str_segment + " " + year + " A";
                case 1:
                    return str_segment + " " + year + " B";
                case 2:
                    return str_segment + " " + nextyear + " B";
                case 3:
                    return str_segment + " " + nextyear + " F";
                case 4:
                    return str_segment + " " + " Total";
                case 5:
                    return str_segment + " " + " Region";
                default:
                    return null;
            }
        }
        else
        {
            switch (number)
            {
                case 0:
                    return "All Products" + " " + year + " A";
                case 1:
                    return "All Products" + " " + year + " B";
                case 2:
                    return "All Products" + " " + nextyear + " B";
                case 3:
                    return "All Products" + " " + nextyear + " F";
                case 4:
                    return "All Products" + " " + " Total";
                case 5:
                    return "All Products" + " " + " Region";
                default:
                    return null;
            }
        }
    }
    protected string GetDimCategories(int num)
    {
        DataSet ds = new DataSet();
        string str_categories = "";
        if (num < 4)
        {
            ds = getRegionName();
            DataTable dt = ds.Tables[0];
            if (ds != null)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string section = dt.Rows[i][0].ToString().Trim();
                    if (section.Contains(","))
                    {
                        section = section.Replace(",", " ");
                    }
                    if (i + 1 == dt.Rows.Count)
                    {
                        str_categories += section;
                    }
                    else
                        str_categories += section + '\t';
                }
                return str_categories;
            }
            else
                return null;
        }
        else
            if (num == 4)
            {
                ds = getTotalBookingByPeriodByRegionBySegment();
                if (ds != null)
                {
                    DataTable dt = ds.Tables[0];
                    for (int j = 0; j < ds.Tables[0].Rows.Count; j++)
                    {
                        string section = dt.Rows[j][0].ToString().Trim();
                        if (section.Contains(","))
                        {
                            section = section.Replace(",", " ");
                        }
                        if (j + 1 == dt.Rows.Count)
                        {
                            str_categories += section;
                        }
                        else
                            str_categories += section + '\t';
                    }
                    return str_categories;
                }
                else
                    return null;
            }
            else
                if (num == 5)
                {
                    return year.Substring(2, 2) + " A" + '\t' + nextyear.Substring(2, 2) + " F";
                }
                else
                    return null;
    }

    protected int tag = 0;
    protected string GetDimValues(int number)
    {
        DataSet ds_Booking = getBookingByPeriodByRegionBySegment();
        DataTable dt = ds_Booking.Tables[0];
        string strDimValues = "";
        if (number < 4)
        {
            strDimValues = "";
            for (int i = 1; i < dt.Columns.Count; i++)
            {
                var num = dt.Rows[number][i];
                if (num is DBNull)
                {
                    return null;
                }
                strDimValues += num.ToString() + "\t";
            }
            strDimValues.Substring(0, strDimValues.Length - 1);
        }
        if (number == 4)
        {
            strDimValues = "";
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                var num = dt.Rows[i][dt.Columns.Count - 1];
                strDimValues += (num is DBNull ? 0 : num).ToString() + "\t";
            }
            strDimValues.Substring(0, strDimValues.Length - 1);
        }
        if (number == 5)
        {
            strDimValues = "";
            for (int i = 1; i < dt.Columns.Count; i++)
            {
                var num = dt.Rows[0][i];
                strDimValues += (num is DBNull ? 0 : num).ToString() + "\t";
            }
            strDimValues.Substring(0, strDimValues.Length - 1);
        }
        if (number == 6)
        {
            strDimValues = "";
            for (int i = 1; i < dt.Columns.Count; i++)
            {
                var num = dt.Rows[3][i];
                strDimValues += (num is DBNull ? 0 : num).ToString() + "\t";
            }
            strDimValues.Substring(0, strDimValues.Length - 1);
        }
        return strDimValues;
    }
    /*Draw table*/
    protected void Drawtable()
    {
        DataSet ds1 = getBookingByPeriodByRegionBySegment();
        DataSet ds2 = getTotalBookingByPeriodByRegionBySegment();
        if (ds1 != null)
        {
            isNullOrNot = true;
            bindDatasource(ds1, ds2);
        }
        else
        {
            isNullOrNot = false;
        }
    }


    protected DataSet getRegionName()
    {
        string query_RegionName = "SELECT [Region].Name FROM [Region] WHERE Deleted='0' ORDER BY [Region].Name ";
        DataSet ds_regionName = helper.GetDataSet(query_RegionName);
        if ((ds_regionName.Tables.Count > 0) && (ds_regionName.Tables[0].Rows.Count > 0))
        {
            return ds_regionName;
        }
        else
        {
            return null;
        }
    }
    protected void CountRegionNumber()
    {
        DataSet ds = getRegionName();
        region_number = ds.Tables[0].Rows.Count;
        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
        {
            al.Add(ds.Tables[0].Rows[i][0]);
        }

    }

    protected DataSet getSegmentName()
    {
        string query_segmentName = "SELECT [Segment].Abbr,[Segment].ID FROM [Segment] WHERE Deleted=0 ORDER BY [Segment].Description";
        DataSet ds_segmentName = helper.GetDataSet(query_segmentName);
        if ((ds_segmentName.Tables.Count > 0) && (ds_segmentName.Tables[0].Rows.Count > 0))
        {
            return ds_segmentName;
        }
        else
        {
            return null;
        }
    }

    protected void bind(DataSet ds)
    {
        if (ds != null)
        {
            DataTable dt = ds.Tables[0];
            int count = dt.Rows.Count;
            int index = 0;
            while (index < count)
            {
                ListItem li = new ListItem(dt.Rows[index][0].ToString().Trim(), dt.Rows[index][1].ToString().Trim());
                ddlist_segment.Items.Add(li);
                index++;
            }
            ddlist_segment.SelectedValue = Request.QueryString["SegmentID"];
            ListItem li1 = new ListItem("Total", "-1");
            ddlist_segment.Items.Add(li1);
            ddlist_segment.Enabled = true;
        }
        else
        {
            ddlist_segment.Enabled = false;
            btn_show.Enabled = false;
        }
    }


    protected DataSet getBookingByPeriodByRegionBySegment()
    {
        string query_yearA = "SELECT '" + year.Substring(2, 2).Trim() + "  A' AS ' '";
        string query_yearB = "SELECT '" + year.Substring(2, 2).Trim() + "  B' AS ' '";
        string query_nextYearB = "SELECT '" + nextyear.Substring(2, 2).Trim() + "  B' AS ' '";
        string query_nextYearF = "SELECT '" + nextyear.Substring(2, 2).Trim() + "  F' AS ' '";
        string str = "";
        for (int i = 0; i < region_number; i++)
        {
            str += ",SUM(CASE WHEN [Region].Name='" + al[i].ToString().Trim()
                + "' THEN ROUND((Amount*(CASE WHEN [Bookings].BookingY='" + year.Substring(2, 2).Trim()
                + "' AND ([Bookings].DeliverY='YTD' OR [Bookings].DeliverY='" + year.Substring(2, 2).Trim()
                + "') THEN [Currency_Exchange].Rate1 ELSE [Currency_Exchange].Rate2 END))/1000,0) ELSE 0 END) AS '" + al[i].ToString().Trim() + "'";
        }
        if (ddlist_segment.SelectedItem.Text.ToString() != "Total")
        {
            str += ",SUM(ROUND((Amount*(CASE WHEN [Bookings].BookingY='" + year.Substring(2, 2).Trim()
                + "' AND ([Bookings].DeliverY='YTD' OR [Bookings].DeliverY='" + year.Substring(2, 2).Trim()
                + "') THEN [Currency_Exchange].Rate1 ELSE [Currency_Exchange].Rate2 END))/1000,0)) AS  'ToTal'"
                + " FROM [Bookings],[Cluster_Country],[Region_Cluster],[Region],[SalesOrg],[SalesOrg_User],[Currency_Exchange]"
                + " WHERE [Bookings].SegmentID='" + ddlist_segment.SelectedItem.Value + "'"
                + " AND [Cluster_Country].CountryID = [Bookings].CountryID"
                + " AND [Bookings].RSMID=[SalesOrg_User].UserID"
                + " AND [SalesOrg_User].SalesOrgID=[SalesOrg].ID"
                + " AND [SalesOrg].CurrencyID = [Currency_Exchange].CurrencyID"
                + " AND [Currency_Exchange].TimeFlag=[Bookings].TimeFlag"
                + " AND [Region_Cluster].ClusterID = [Cluster_Country].ClusterID"
                + " AND [Region].ID = [Region_Cluster].RegionID"
                + " AND [Cluster_Country].Deleted=0 "
                + " AND [Region_Cluster].Deleted=0 "
                + " AND [Region].Deleted=0 "
                + " AND [SalesOrg].Deleted=0 "
                + " AND [SalesOrg_User].Deleted=0 "
                + " AND [Currency_Exchange].Deleted=0 ";
        }
        else
        {
            str += ",SUM(ROUND(Amount/1000,0)) AS  'ToTal'"
                + " FROM [Bookings],[Cluster_Country],[Region_Cluster],[Region],[SalesOrg],[SalesOrg_User],[Currency_Exchange]"
                + " WHERE [Cluster_Country].CountryID = [Bookings].CountryID"
                + " AND [Bookings].RSMID = [SalesOrg_User].UserID"
                + " AND [SalesOrg_User].SalesOrgID=[SalesOrg].ID"
                + " AND [SalesOrg].CurrencyID = [Currency_Exchange].CurrencyID"
                + " AND [Currency_Exchange].TimeFlag = [Bookings].TimeFlag"
                + " AND [Region_Cluster].ClusterID = [Cluster_Country].ClusterID"
                + " AND [Region].ID = [Region_Cluster].RegionID"
                + " AND [Cluster_Country].Deleted=0 "
                + " AND [Region_Cluster].Deleted=0 "
                + " AND [Region].Deleted=0 "
                + " AND [SalesOrg].Deleted=0 "
                + " AND [SalesOrg_User].Deleted=0 "
                + " AND [Currency_Exchange].Deleted=0 ";
        }
        query_yearA += str;
        query_yearB += str;
        query_nextYearB += str;
        query_nextYearF += str;

        query_yearA += " AND [Bookings].BookingY='" + year.Substring(2, 2).Trim() + "'"
                    + " AND Year([Bookings].TimeFlag)='" + year.Trim() + "'"
                    + " AND Month([Bookings].TimeFlag)='" + month.Trim() + "'";
        query_yearB += " AND [Bookings].BookingY='" + year.Substring(2, 2).Trim() + "'"
                    + " AND Year([Bookings].TimeFlag)='" + preyear.Trim() + "'"
                    + " AND Month([Bookings].TimeFlag)='03'";
        query_nextYearB += " AND [Bookings].BookingY='" + nextyear.Substring(2, 2).Trim() + "'"
                        + " AND Year([Bookings].TimeFlag)='" + year.Trim() + "'"
                        + " AND Month([Bookings].TimeFlag)='03'";
        query_nextYearF += " AND [Bookings].BookingY='" + nextyear.Substring(2, 2).Trim() + "'"
                        + " AND Year([Bookings].TimeFlag)='" + year.Trim() + "'"
                        + " AND Month([Bookings].TimeFlag)='" + month.Trim() + "'";
        string query = query_yearA + " UNION " + query_yearB + " UNION " + query_nextYearB + " UNION " + query_nextYearF;
        DataSet ds = helper.GetDataSet(query);

        if (ds.Tables[0].Rows.Count > 0)
            return ds;
        else
            return null;

    }

    protected DataSet getTotalBookingByPeriodByRegionBySegment()
    {
        string query_yearA = "SELECT '" + year.Substring(2, 2).Trim() + "  A' AS ' '";
        string query_yearB = "SELECT '" + year.Substring(2, 2).Trim() + "  B' AS ' '";
        string query_nextYearB = "SELECT '" + nextyear.Substring(2, 2).Trim() + "  B' AS ' '";
        string query_nextYearF = "SELECT '" + nextyear.Substring(2, 2).Trim() + "  F' AS ' '";
        string str = "";
        if (ddlist_segment.SelectedItem.Text.ToString() != "Total")
        {
            str += ",SUM(ROUND((Amount*(CASE WHEN [Bookings].BookingY='" + year.Substring(2, 2).Trim()
                 + "' AND ([Bookings].DeliverY='YTD' OR [Bookings].DeliverY='" + year.Substring(2, 2).Trim()
                 + "') THEN [Currency_Exchange].Rate1 ELSE [Currency_Exchange].Rate2 END))/1000,0)) AS  'ToTal'"
                 + " FROM [Bookings],[Cluster_Country],[Region_Cluster],[Region],[SalesOrg],[SalesOrg_User],[Currency_Exchange]"
                 + " WHERE [Bookings].SegmentID='" + ddlist_segment.SelectedItem.Value + "'"
                 + " AND [Cluster_Country].CountryID = [Bookings].CountryID"
                 + " AND [Bookings].RSMID=[SalesOrg_User].UserID"
                 + " AND [SalesOrg_User].SalesOrgID=[SalesOrg].ID"
                 + " AND [SalesOrg].CurrencyID=[Currency_Exchange].CurrencyID"
                 + " AND [Currency_Exchange].TimeFlag=[Bookings].TimeFlag"
                 + " AND [Region_Cluster].ClusterID = [Cluster_Country].ClusterID"
                 + " AND [Region].ID = [Region_Cluster].RegionID"
                 + " AND [Cluster_Country].Deleted=0 "
                 + " AND [Region_Cluster].Deleted=0 "
                 + " AND [Region].Deleted=0 "
                 + " AND [SalesOrg].Deleted=0 "
                 + " AND [SalesOrg_User].Deleted=0 "
                 + " AND [Currency_Exchange].Deleted=0 ";
        }
        else
        {
            str += ",SUM(ROUND(Amount/1000,0)) AS  'ToTal'"
                + " FROM [Bookings],[Cluster_Country],[Region_Cluster],[Region],[SalesOrg],[SalesOrg_User],[Currency_Exchange]"
                + " WHERE [Cluster_Country].CountryID = [Bookings].CountryID"
                + " AND [Bookings].RSMID=[SalesOrg_User].UserID"
                + " AND [SalesOrg_User].SalesOrgID=[SalesOrg].ID"
                + " AND [SalesOrg].CurrencyID=[Currency_Exchange].CurrencyID"
                + " AND [Currency_Exchange].TimeFlag=[Bookings].TimeFlag"
                + " AND [Region_Cluster].ClusterID = [Cluster_Country].ClusterID"
                + " AND [Region].ID = [Region_Cluster].RegionID"
                + " AND [Cluster_Country].Deleted=0 "
                + " AND [Region_Cluster].Deleted=0 "
                + " AND [Region].Deleted=0 "
                + " AND [SalesOrg].Deleted=0 "
                + " AND [SalesOrg_User].Deleted=0 "
                + " AND [Currency_Exchange].Deleted=0 ";
        }
        query_yearA += str;
        query_yearB += str;
        query_nextYearB += str;
        query_nextYearF += str;

        query_yearA += " AND [Bookings].BookingY='" + year.Substring(2, 2).Trim() + "'"
                    + " AND Year([Bookings].TimeFlag)='" + year.Trim() + "'"
                    + " AND Month([Bookings].TimeFlag)='" + month.Trim() + "'";
        query_yearB += " AND [Bookings].BookingY='" + year.Substring(2, 2).Trim() + "'"
                    + " AND Year([Bookings].TimeFlag)='" + preyear.Trim() + "'"
                    + " AND Month([Bookings].TimeFlag)='03'";
        query_nextYearB += " AND [Bookings].BookingY='" + nextyear.Substring(2, 2).Trim() + "'"
                        + " AND Year([Bookings].TimeFlag)='" + year.Trim() + "'"
                        + " AND Month([Bookings].TimeFlag)='03'";
        query_nextYearF += " AND [Bookings].BookingY='" + nextyear.Substring(2, 2).Trim() + "'"
                        + " AND Year([Bookings].TimeFlag)='" + year.Trim() + "'"
                        + " AND Month([Bookings].TimeFlag)='" + month.Trim() + "'";
        string query = query_yearA + " UNION " + query_yearB + " UNION " + query_nextYearB + " UNION " + query_nextYearF;
        DataSet ds = helper.GetDataSet(query);

        if (ds.Tables[0].Rows.Count > 0)
            return ds;
        else
            return null;
    }
    protected void bindDatasource(DataSet ds_Booking, DataSet ds_totalBooking)
    {
        if (ds_Booking != null && ds_totalBooking != null)
        {
            flag = true;
            gv_bookingSegment.Width = Unit.Pixel(200);
            gv_bookingSegment.AutoGenerateColumns = false;
            gv_bookingSegment.AllowPaging = false;
            gv_bookingSegment.Visible = true;

            gv_totalBookingSegment.Width = Unit.Pixel(200);
            gv_totalBookingSegment.AutoGenerateColumns = false;
            gv_totalBookingSegment.AllowPaging = false;
            gv_totalBookingSegment.Visible = true;

            for (int i = 0; i < ds_Booking.Tables[0].Columns.Count; i++)
            {
                BoundField bf = new BoundField();
                bf.DataField = ds_Booking.Tables[0].Columns[i].ColumnName.ToString();
                bf.HeaderText = ds_Booking.Tables[0].Columns[i].Caption.ToString();
                bf.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
                bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
                gv_bookingSegment.Columns.Add(bf);
            }
            for (int j = 0; j < ds_totalBooking.Tables[0].Columns.Count; j++)
            {
                BoundField bf = new BoundField();
                bf.DataField = ds_totalBooking.Tables[0].Columns[j].ColumnName.ToString();
                bf.HeaderText = ds_totalBooking.Tables[0].Columns[j].Caption.ToString();
                bf.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
                bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
                gv_totalBookingSegment.Columns.Add(bf);
            }


            gv_bookingSegment.DataSource = ds_Booking.Tables[0];
            gv_bookingSegment.DataBind();

            gv_totalBookingSegment.DataSource = ds_totalBooking.Tables[0];
            gv_totalBookingSegment.DataBind();
        }
        else
            flag = false;
    }
}
