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
using System.Xml;
using Microsoft.Office.Interop.Owc11;
using Microsoft.Office.Core;
using PowerPoint = Microsoft.Office.Interop.PowerPoint;
using System.Reflection;
using System.Drawing;

public partial class Assistant_AssistantBookSalesForecast : System.Web.UI.Page
{
    SQLHelper helper = new SQLHelper();
    LogUtility log = new LogUtility();
    DisplayInfo info = new DisplayInfo();
    WebUtility web = new WebUtility();
    SQLStatement sql = new SQLStatement();
    //by ryzhang item49 20110519 del start 
    //GetMeetingDate date = new GetMeetingDate();
    //by ryzhang item49 20110519 del end 
    //by ryzhang item49 20110519 add start 
    GetSelectMeetingDate meeting = new GetSelectMeetingDate();
    //by ryzhang item49 20110519 add end 

    /* Set Date */
    protected static string yearBeforePre;
    protected static string preyear;
    protected static string year;
    protected static string nextyear;
    protected static string yearAfterNext;
    protected static string month;
    protected const string fiscalStart = "Oct.1";
    protected const string fiscalEnd = "Sept.30";

    protected void Page_Load(object sender, EventArgs e)
    {
        if (getRoleID(getRole()) == "0" || getRoleID(getRole()) == "5")
        {

        }
        else
            Response.Redirect("~/AccessDenied.aspx");

        if (!IsPostBack)
        {
            log.WriteLog(LogUtility.LogErrorLevel.LOG_ACCESS, "AssistantBookingSalesForecast Access.");
            //by ryzhang item49 20110519 del start 
            //meeting.setDate();
            //by ryzhang item49 20110519 del end 
            //by ryzhang item49 20110519 add start 
            meeting.setSelectDate(Session["AssistantID"].ToString());
            //by ryzhang item49 20110519 add end 
            yearBeforePre = meeting.getyearBeforePre();
            preyear = meeting.getpreyear();
            year = meeting.getyear();
            nextyear = meeting.getnextyear();
            yearAfterNext = meeting.getyearAfterNext();
            month = meeting.getmonth();
            Response.Clear();
            bindDdlist(getSegmentInfo());
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
        //by yyan 20110915 itemw148 add start
        string sql = "  SELECT Segment.id ID,Segment.Abbr AS 'Segment',Segment.Description Description from Segment,[User],User_Segment where " +
                   " Segment.Deleted=0 and User_Segment.SegmentID =Segment.ID and " +
                   " User_Segment.Deleted=0 and User_Segment.UserID= [User].UserID and [User].Deleted =0 and" +
                   " [User].UserID=" + Session["AssistantID"].ToString()  ;
        DataSet ds_segment = helper.GetDataSet(sql);
        return ds_segment;
        //by yyan 20110915 itemw148 add end
        //by yyan 20110915 itemw148 del start
        //return sql.getSegmentInfo();
        //by yyan 20110915 itemw148 del end
    }

    protected void bindDdlist(DataSet ds)
    {
        if (ds.Tables[0].Rows.Count > 0)
        {
            DataTable dt = ds.Tables[0];
            int count = dt.Rows.Count;
            int index = 0;
            while (index < count)
            {
                ListItem li = new ListItem(dt.Rows[index][1].ToString().Trim(), dt.Rows[index][0].ToString().Trim());
                ddlist_segment.Items.Add(li);
                index++;
            }
            ddlist_segment.SelectedIndex = 0;
            ddlist_segment.Enabled = true;
            btn_search.Enabled = true;
        }
        else
        {

            ddlist_segment.Enabled = false;
            ddlist_segment.Items.Add("");
            btn_search.Enabled = false;
        }
    }

    protected void bindDataSource()
    {
        DataSet ds_Country = sql.getOperationBySegment(ddlist_segment.SelectedItem.Value);

        if (ds_Country.Tables[0].Rows.Count > 0)
        {
            gv_opAbbr.Width = Unit.Pixel(200);
            gv_opAbbr.AutoGenerateColumns = false;
            gv_opAbbr.AllowPaging = true;
            gv_opAbbr.Visible = true;

            for (int i = 2; i < ds_Country.Tables[0].Columns.Count; i++)
            {
                BoundField bf = new BoundField();

                bf.DataField = ds_Country.Tables[0].Columns[i].ColumnName.ToString();
                bf.HeaderText = ds_Country.Tables[0].Columns[i].Caption.ToString();
                bf.ItemStyle.Width = 100;
                bf.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
                bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
                bf.ReadOnly = true;

                gv_opAbbr.Columns.Add(bf);
            }

            gv_opAbbr.AllowSorting = true;
            gv_opAbbr.DataSource = ds_Country.Tables[0];
            gv_opAbbr.DataBind();
            panel_enter.Visible = true;
            panel_enter.Enabled = true;
        }
        else
        {
            panel_enter.Visible = true;
            gv_opAbbr.Visible = false;
            panel_enter.Enabled = false;
        }
    }

    protected void btn_search_Click(object sender, EventArgs e)
    {
        label_segDecription.Text = ddlist_segment.SelectedItem.Text;
        label_segDecription.Visible = true;
        //By Fxw 20110506 ITEM25 ADD Start
        string query_date = "SELECT CONVERT(varchar(15),SelectMeetingDate,23) FROM [SetSelectMeetingDate] where userid=" + Session["AssistantID"].ToString();
        DataSet ds_date = helper.GetDataSet(query_date);
        if (ds_date.Tables[0].Rows.Count > 0 && !ds_date.Tables[0].Rows[0][0].ToString().Equals("") && ds_date.Tables[0].Rows[0][0].ToString() != null)
        {
            //by ryzhang 20110523 item49 del start
            //label_show.Text = "This report is related to the meeting date " + ds_date.Tables[0].Rows[0][0].ToString();
            //by ryzhang 20110523 item49 del end
            //by ryzhang 20110523 item49 add start
            label_show.Text = "This report is related to the meeting date (select) " + ds_date.Tables[0].Rows[0][0].ToString();
            this.lbtn_op_sales.Enabled = true;
            this.lbtn_op_bkg.Enabled = true;
            this.lbtn_gr_sales.Enabled = true;
            this.lbtn_gr_bkg.Enabled = true;
            this.lbtn_salesorg.Enabled = true;
            this.lbtn_region.Enabled = true;
            //by yyan itemW95 20110812 del start
            //this.lbtn_ppt.Enabled = true;
            //by yyan itemW95 20110812 del end
            this.lbtn_regionchart.Enabled = true;
            //by ryzhang 20110523 item49 add end
            string query_year = "SELECT YEAR(SelectMeetingDate) FROM [SetSelectMeetingDate] where userid=" + Session["AssistantID"].ToString();
            DataSet ds_year = helper.GetDataSet(query_year);
            string query_month = "SELECT MONTH(SelectMeetingDate) FROM [SetSelectMeetingDate] where userid=" + Session["AssistantID"].ToString();
            DataSet ds_month = helper.GetDataSet(query_month);
            string query_day = "SELECT Day(SelectMeetingDate) FROM [SetSelectMeetingDate] where userid=" + Session["AssistantID"].ToString();
            DataSet ds_day = helper.GetDataSet(query_day);
            if (ds_date.Tables[0].Rows.Count > 0 && ds_date.Tables[0].Rows.Count > 0 && ds_date.Tables[0].Rows.Count > 0)
            {
                label_noteDate.Text = "Date of B/L : <br />" + meeting.getMonth(ds_month.Tables[0].Rows[0][0].ToString()) + " " + ds_day.Tables[0].Rows[0][0].ToString() + "," + ds_year.Tables[0].Rows[0][0].ToString();
                label_noteDate.Text += " <br /><br />bookings forecast from : <br />" + meeting.getMonth(ds_month.Tables[0].Rows[0][0].ToString()) + " " + ds_day.Tables[0].Rows[0][0].ToString() + "," + ds_year.Tables[0].Rows[0][0].ToString();
                label_bookingsDecription.Text = " BOOKINGS FORECAST BY SALES ORGANIZATION FOR " + (Convert.ToInt32(ds_year.Tables[0].Rows[0][0].ToString()) + 1) + "&" + (Convert.ToInt32(ds_year.Tables[0].Rows[0][0].ToString()) + 2);
                label_salesDecription.Text = " SALES FORECAST BY OPERATION FOR " + (Convert.ToInt32(ds_year.Tables[0].Rows[0][0].ToString()) + 1);
            }
        }
        else
        {
            //by ryzhang 20110523 item49 del start
            //label_show.Text = "There is no meeting date selected!";
            //by ryzhang 20110523 item49 del end
            //by ryzhang 20110523 item49 add start
            label_show.Text = "There is no meeting date selected! Please select one meeting date first!";
            label_show.Style.Add("color", "red");
            this.lbtn_op_sales.Enabled = false;
            this.lbtn_op_bkg.Enabled = false;
            this.lbtn_gr_sales.Enabled = false;
            this.lbtn_gr_bkg.Enabled = false;
            this.lbtn_salesorg.Enabled = false;
            this.lbtn_region.Enabled = false;
            //by yyan itemW95 20110812 del start
            //this.lbtn_ppt.Enabled = false;
            //by yyan itemW95 20110812 del end
            this.lbtn_regionchart.Enabled = false;
            //by ryzhang 20110523 item49 add end
            label_noteDate.Text = "Date of B/L : <br />";
            label_noteDate.Text += " <br /><br />bookings forecast from : <br />";
            label_bookingsDecription.Text = " BOOKINGS FORECAST BY SALES ORGANIZATION";
            label_salesDecription.Text = " SALES FORECAST BY OPERATION";
        }
        //By Fxw 20110506 ITEM25 ADD End
        //By Fxw 20110506 ITEM25 DEL Start
        //label_bookingsDecription.Text = " BOOKINGS FORECAST BY SALN FOR " + nextyear;
        //label_noteDate.Text = "Date of B/L : <br />" + meeting.getMonth(month) + " " + meeting.getDay() + "," + year;
        //label_noteDate.Text += " <br /><br />bookings forecast from : <br />" + meeting.getMonth(month) + " " + meeting.getDay() + "," + year;
        //By Fxw 20110506 ITEM25 DEL End
        panel_dec.Visible = true;
        gv_opAbbr.Columns.Clear();
        bindDataSource();
    }

    protected void lbtn_salesorg_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/Assistant/AssistantBookingBySalesOrg.aspx?SegmentID="+ ddlist_segment.SelectedItem.Value);
    }

    protected void lbtn_op_bkg_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/Assistant/AssistantOperationalBookings.aspx?SegmentID=" + ddlist_segment.SelectedItem.Value);
    }

    protected void lbtn_gr_bkg_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/Assistant/AssistantGrossBookings.aspx?SegmentID=" + ddlist_segment.SelectedItem.Value);
    }

    protected void lbtn_op_sales_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/Assistant/AssistantOperationSales.aspx?SegmentID=" + ddlist_segment.SelectedItem.Value);
    }

    protected void lbtn_gr_sales_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/Assistant/AssistantGrossSales.aspx?SegmentID=" + ddlist_segment.SelectedItem.Value);
    }

     protected void lbtn_region_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/Assistant/AssistantBookingByCountryByRegion.aspx?SegmentID=" + ddlist_segment.SelectedItem.Value);
    }

     protected void lbtn_regionchart_Click(object sender, EventArgs e)
     {
         Response.Redirect("~/Assistant/AssistantBookingChart.aspx?SegmentID=" + ddlist_segment.SelectedItem.Value);
     }

    /*****************************************************/
     private string pictureName = "PPT.gif";
     private string pictureFolderPath;
     private string picturePath;

     private int segmentNumber;
     private int regionNumber;
     private string dimSegmentCategories;
     private string dimRegionCategories;
     private string salesGap;
     private float[] x_four = { 60, 60, 370, 370 };
     private float[] y_four = { 90, 300, 90, 300 };

     private ArrayList alSegment = new ArrayList();
     private ArrayList alRegion = new ArrayList();
     private ArrayList alRegionID = new ArrayList();
     private ArrayList fiveTopCountries = new ArrayList();
     private ArrayList alOperation = new ArrayList();
     private ArrayList alProduct = new ArrayList();
     private ArrayList alSalesOrg = new ArrayList();
     private ArrayList alYears = new ArrayList();

     private int slideIndex = 0;

     protected DataSet getBookingDataByCountry(string str_subregionID, string str_segmentID)
     {
         string sql = "";
         if (str_segmentID != "-1")
         {
             sql = "SELECT [Country].Name, "
                         + " SUM(CASE WHEN BookingY = '" + meeting.getyearBeforePre().Substring(2, 2) + "' and YEAR(TimeFlag) = '" + meeting.getyearBeforePre() + "' and MONTH(TimeFlag) = '10' THEN Amount ELSE 0 END) AS 'FY " + meeting.getyearBeforePre() + "',"
                         + " SUM(CASE WHEN BookingY = '" + meeting.getpreyear().Substring(2, 2) + "' and YEAR(TimeFlag) = '" + meeting.getpreyear() + "' and MONTH(TimeFlag) = '10' THEN Amount ELSE 0 END) AS 'FY " + meeting.getpreyear() + "',"
                         + " SUM(CASE WHEN BookingY = '" + meeting.getyear().Substring(2, 2) + "' and YEAR(TimeFlag) = '" + meeting.getyear() + "' and MONTH(TimeFlag) = '10' THEN Amount ELSE 0 END) AS 'FY " + meeting.getyear() + "',"
                         + " SUM(CASE WHEN BookingY = '" + meeting.getnextyear().Substring(2, 2) + "' and YEAR(TimeFlag) = '" + meeting.getyear() + "' and MONTH(TimeFlag) = '10' THEN Amount ELSE 0 END) AS 'FY " + meeting.getnextyear() + "' "
                         + " FROM [Bookings]"
                         + " INNER JOIN [Country] ON [Country].ID = [Bookings].CountryID"
                         + " WHERE CountryID IN (SELECT CountryID FROM [Cluster_Country] WHERE ClusterID = " + str_subregionID + " AND Deleted=0)"
                         + " AND Country.Deleted=0 "
                         + " AND SegmentID = " + str_segmentID
                         + " GROUP BY [Country].Name"
                         + " ORDER BY [Country].Name";
         }
         else
         {
             sql = "SELECT [Country].Name, "
                         + " SUM(CASE WHEN BookingY = '" + meeting.getyearBeforePre().Substring(2, 2) + "' and YEAR(TimeFlag) = '" + meeting.getyearBeforePre() + "' and MONTH(TimeFlag) = '10' THEN Amount ELSE 0 END) AS 'FY " + meeting.getyearBeforePre() + "',"
                         + " SUM(CASE WHEN BookingY = '" + meeting.getpreyear().Substring(2, 2) + "' and YEAR(TimeFlag) = '" + meeting.getpreyear() + "' and MONTH(TimeFlag) = '10' THEN Amount ELSE 0 END) AS 'FY " + meeting.getpreyear() + "',"
                         + " SUM(CASE WHEN BookingY = '" + meeting.getyear().Substring(2, 2) + "' and YEAR(TimeFlag) = '" + meeting.getyear() + "' and MONTH(TimeFlag) = '10' THEN Amount ELSE 0 END) AS 'FY " + meeting.getyear() + "',"
                         + " SUM(CASE WHEN BookingY = '" + meeting.getnextyear().Substring(2, 2) + "' and YEAR(TimeFlag) = '" + meeting.getyear() + "' and MONTH(TimeFlag) = '10' THEN Amount ELSE 0 END) AS 'FY " + meeting.getnextyear() + "' "
                         + " FROM [Bookings]"
                         + " INNER JOIN [Country] ON [Country].ID = [Bookings].CountryID"
                         + " WHERE CountryID IN (SELECT CountryID FROM [Cluster_Country] WHERE ClusterID = " + str_subregionID + " AND Deleted=0)"
                         + " AND Country.Deleted=0 "
                         + " GROUP BY [Country].Name"
                         + " ORDER BY [Country].Name";
         }
         DataSet ds = helper.GetDataSet(sql);

         if (ds.Tables[0].Rows.Count > 0)
             return ds;
         else
             return null;
     }

     PowerPoint.Application application;
     PowerPoint.Presentations presentations;
     PowerPoint._Presentation presentation;
     PowerPoint.Slides slides;

     protected void lbtn_ppt_Click(object sender, EventArgs e)
     {
         GC.Collect();
         Response.Clear();
         ShareInformation();
         OperatePPT();
         System.IO.FileInfo file = new System.IO.FileInfo(pictureFolderPath + "TEST.ppt");
         Response.Charset = "GB2312";
         Response.ContentEncoding = System.Text.Encoding.UTF8;
         Response.AddHeader("Content-Disposition", "attachment; filename=" + Server.UrlEncode(file.FullName));
         Response.AddHeader("Content-Length", file.Length.ToString());
         Response.ContentType = "application/ms-excel";
         Response.WriteFile(file.FullName);
         Response.End();
     }
     //---------------------------------------------------------share Infomation
     protected void ShareInformation()
     {
         pictureFolderPath = Server.MapPath("~") + @"\PPTImages\";
         getRegionInfo();
         getSegmentInfoPPT();
         GetYears();
     }
     protected void GetYears()
     {
         string query = "SELECT [Bookings].BookingY FROM [Bookings]"
                      + " WHERE ([Bookings].BookingY='" + meeting.getyearBeforePre().Substring(2, 2) + "'"
                      + " OR [Bookings].BookingY='" + meeting.getpreyear().Substring(2, 2) + "'"
                      + " OR [Bookings].BookingY='" + meeting.getyear().Substring(2, 2) + "'"
                      + " OR [Bookings].BookingY='" + meeting.getnextyear().Substring(2, 2) + "')"
                      + " GROUP BY [Bookings].BookingY"
                      + " ORDER BY [Bookings].BookingY";
         DataSet ds = helper.GetDataSet(query);
         for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
         {
             alYears.Add(ds.Tables[0].Rows[i][0]);
         }
     }
     protected void getRegionInfo()
     {
         string query_regions = "SELECT [Region].Name,[Region].ID FROM [Region] WHERE [Region].Deleted='0'";
         DataSet ds_regions = helper.GetDataSet(query_regions);
         regionNumber = ds_regions.Tables[0].Rows.Count;
         if (regionNumber > 0)
         {
             for (int i = 0; i < regionNumber; i++)
             {
                 alRegion.Add(ds_regions.Tables[0].Rows[i][0]);
                 alRegionID.Add(ds_regions.Tables[0].Rows[i][1]);
                 string section = ds_regions.Tables[0].Rows[i][0].ToString();
                 if (section.Contains(","))
                     section = section.Replace(",", " ");
                 dimRegionCategories += section + "\t";
             }
             dimRegionCategories = dimRegionCategories.Substring(0, dimRegionCategories.Length - 1);
         }
     }
     protected void getSegmentInfoPPT()
     {
         string query_segment = "SELECT [Segment].Abbr,[Segment].ID FROM [Segment] WHERE [Segment].Deleted='0' ORDER BY [Segment].Abbr";
         DataSet ds_segment = helper.GetDataSet(query_segment);
         segmentNumber = ds_segment.Tables[0].Rows.Count;
         if (segmentNumber > 0)
         {
             for (int i = 0; i < segmentNumber; i++)
             {
                 alSegment.Add(ds_segment.Tables[0].Rows[i][0]);
                 string section = ds_segment.Tables[0].Rows[i][0].ToString();
                 if (section.Contains(","))
                     section = section.Replace(",", " ");
                 dimSegmentCategories += section + "\t";
             }
             dimSegmentCategories = dimSegmentCategories.Substring(0, dimSegmentCategories.Length - 1);
         }
     }
     //-------------------------------------------------------------------------------
     protected void InitializePPT()
     {
         application = (PowerPoint.ApplicationClass)Server.CreateObject("PowerPoint.Application");
         presentations = application.Presentations;
         presentation = presentations.Add(Microsoft.Office.Core.MsoTriState.msoTrue);
         slides = presentation.Slides;
     }
     protected void OperatePPT()
     {
         InitializePPT();
         DrawSlide_One();
         DrawSlide_Two();
         DrawSlide_Three();
         DrawSlide_Four();
         DrawSlide_Seven();
         DrawSlide_Nine();
         DrawSlide_Ten();
         DrawSlide_eleven();
         DrawSlide_Telwe();
         DrawSlide_Thirteen();
         DrawSlide_Fourteen();
         DrawSlide_Fifteen();
         DrawSlide_Sixteen();
         DrawSlide_Nineteen();
         DrawSlide_TwentySeven();
         DrawSlide_TwentyNine();
         presentation.SaveAs(pictureFolderPath + "TEST.ppt", PowerPoint.PpSaveAsFileType.ppSaveAsDefault, Microsoft.Office.Core.MsoTriState.msoTrue);
         presentation.Close();
         application.Quit();
     }
     protected void UniformLayout(PowerPoint._Slide slide, string title)
     {
         slide.Master.Background.Fill.UserPicture(pictureFolderPath + "background.jpg");
         slide.Shapes.Title.Left = 175;
         slide.Shapes.Title.Top = 10;
         slide.Shapes.Title.Width = 350;
         slide.Shapes.Title.Height = 35;
         slide.Shapes.Title.TextFrame.WordWrap = Microsoft.Office.Core.MsoTriState.msoTrue;
         PowerPoint.TextRange textR = slide.Shapes.Title.TextFrame.TextRange;
         textR.Text = title;
         textR.Font.Size = 24;
         textR.Font.Bold = Microsoft.Office.Core.MsoTriState.msoTrue;
     }
     public static void InsertText(PowerPoint._Slide slide, string text, float left, float top, float width, float height, int fontSize)
     {
         PowerPoint.Shape shape = slide.Shapes.AddTextbox(Microsoft.Office.Core.
             MsoTextOrientation.msoTextOrientationHorizontal, left, top, width, height);
         PowerPoint.TextRange textRng = shape.TextFrame.TextRange;
         shape.TextFrame.WordWrap = MsoTriState.msoTrue;
         shape.TextFrame.HorizontalAnchor = Microsoft.Office.Core.MsoHorizontalAnchor.msoAnchorCenter;
         textRng.Text = text;
         textRng.Font.Size = fontSize;
         textRng.Font.Color.RGB = Color.Black.ToArgb();
         shape.Line.Visible = MsoTriState.msoTrue;
     }
     public static void InsertText(PowerPoint._Slide slide, string text, float left, float top, float width, float height)
     {
         PowerPoint.Shape shape = slide.Shapes.AddTextbox(Microsoft.Office.Core.
             MsoTextOrientation.msoTextOrientationHorizontal, left, top, width, height);
         PowerPoint.TextRange textRng = shape.TextFrame.TextRange;
         shape.TextFrame.WordWrap = MsoTriState.msoTrue;
         shape.TextFrame.HorizontalAnchor = Microsoft.Office.Core.MsoHorizontalAnchor.msoAnchorCenter;
         textRng.Text = text;
         textRng.Font.Size = 14;
         textRng.Font.Color.RGB = Color.Black.ToArgb();
         shape.Line.Visible = MsoTriState.msoTrue;
     }
     public static void InsertPicture(PowerPoint._Slide slide, string fileName, float left, float top, float width, float height)
     {
         PowerPoint.Shape shape = slide.Shapes.AddPicture(fileName, Microsoft.Office.Core.MsoTriState.msoFalse, Microsoft.Office.Core.MsoTriState.msoTrue, left, top, width, height);
     }
     protected void StandardTitle(PowerPoint.Slide slide, string titleText)
     {
         slide.Shapes.Title.Left = 175;
         slide.Shapes.Title.Top = 20;
         slide.Shapes.Title.Width = 350;
         slide.Shapes.Title.Height = 50;
         slide.Shapes.Title.TextFrame.WordWrap = Microsoft.Office.Core.MsoTriState.msoTrue;
         PowerPoint.TextRange textR = slide.Shapes.Title.TextFrame.TextRange;
         textR.Text = titleText;
         textR.Font.Size = 24;
         textR.Font.Bold = Microsoft.Office.Core.MsoTriState.msoTrue;
     }
     protected void DrawSlide_One()
     {
         slides.Add(++slideIndex, Microsoft.Office.Interop.PowerPoint.PpSlideLayout.ppLayoutTitleOnly);
         PowerPoint.Slide slide = slides[slideIndex];
         slide.Master.Background.Fill.UserPicture(pictureFolderPath + "background.jpg");
         string titleText = " 0+12 " + meeting.getnextyear().Trim()
                    + " Sales & Marketing Meeting "
                    + " SIEMENS E T HP";
         StandardTitle(slide, titleText);
         InsertPicture(slide, pictureFolderPath + @"worldMap.png", 20, 100, 700, 400);
         InsertPicture(slide, pictureFolderPath + @"upperRight_HSP.png", 600, 5, 50, 50);
         float startPosition = 50;
         for (int i = 0; i < segmentNumber; i++)
         {
             startPosition += 85;
             PowerPoint.Shape shape = slide.Shapes.AddTextbox(Microsoft.Office.Core.
             MsoTextOrientation.msoTextOrientationHorizontal, startPosition, 130, 76, 37);
             PowerPoint.TextRange textRng = shape.TextFrame.TextRange;
             textRng.Text = alSegment[i].ToString();
             textRng.Font.Color.RGB = Color.Red.ToArgb();
             textRng.Font.Bold = MsoTriState.msoTrue;
             textRng.Font.Size = 18;
             textRng.ParagraphFormat.Alignment = Microsoft.Office.Interop.PowerPoint.PpParagraphAlignment.ppAlignCenter;
             shape.Line.Visible = MsoTriState.msoTrue;
         }
     }
     //---------------------------------------------------------------------------------
     protected void DrawSlide_Two()
    {
        slides.Add(++slideIndex, Microsoft.Office.Interop.PowerPoint.PpSlideLayout.ppLayoutTitleOnly);
        PowerPoint.Slide slide = slides[slideIndex];
        UniformLayout(slide, "Sales Forcast");

        ChartSpace objSpace = new ChartSpace();
        ChChart objChart = objSpace.Charts.Add(0);
        objChart.Type = ChartChartTypeEnum.chChartTypeColumnStacked;
        objChart.HasLegend = true;

        string dimCategories = "";
        DataSet dsAdd = getSlide_TwoAdd();
        DataSet ds = getSlide_Two();
        if (ds != null)
        {
            for (int k = 0; k < ds.Tables[0].Rows.Count; k++)
            {
                string str = ds.Tables[0].Rows[k][0].ToString();
                if (k != ds.Tables[0].Rows.Count - 1)
                    dimCategories += str + "\t";
                else
                    dimCategories += meeting.getnextyear().Trim() + " E/B\t";
            }
            dimCategories = dimCategories.Substring(0, dimCategories.Length - 1);
            for (int i = 0; i < segmentNumber; i++)
            {
                string dimValues = "";
                for (int g = 0; g < ds.Tables[0].Rows.Count; g++)
                {
                    var num1 = ds.Tables[0].Rows[g][i + 1];
                    if (dsAdd != null)
                    {
                        var num2 = dsAdd.Tables[0].Rows[g][i + 1];
                        dimValues += (Convert.ToDouble(num1 is DBNull ? 0 : num1) + Convert.ToDouble(num2 is DBNull ? 0 : num2)).ToString() + "\t";
                    }
                    else
                        dimValues += Convert.ToDouble(num1 is DBNull ? 0 : num1).ToString() + "\t";
                }
                dimValues = dimValues.Substring(0, dimValues.Length - 1);
                objChart.SeriesCollection.Add(i);
                objChart.SeriesCollection[i].Caption = alSegment[i].ToString();

                objChart.SeriesCollection[i].SetData(ChartDimensionsEnum.chDimCategories,
                       +(int)ChartSpecialDataSourcesEnum.chDataLiteral, dimCategories);
                objChart.SeriesCollection[i].SetData(ChartDimensionsEnum.chDimValues,
                        +(int)ChartSpecialDataSourcesEnum.chDataLiteral, dimValues);
            }
            picturePath = pictureFolderPath + pictureName;
            objSpace.ExportPicture(picturePath, "GIF", 500, 350);
            InsertPicture(slide, picturePath, 60, 160, 600, 350);
            float startPosition = 37;
            double preyearTotal = 0;
            double yearTotal = 0;
            double nextyearTotal = 0;
            for (int m = 0; m < ds.Tables[0].Rows.Count; m++)
            {
                double total = 0;
                for (int n = 1; n < ds.Tables[0].Columns.Count; n++)
                {
                    var num1 = ds.Tables[0].Rows[m][n];
                    total = Convert.ToDouble(num1 is DBNull ? 0 : num1);
                    if (dsAdd != null)
                    {
                        var num2 = dsAdd.Tables[0].Rows[m][n];
                        total += Convert.ToDouble(num2 is DBNull ? 0 : num2);
                    }
                }
                switch (m)
                {
                    case 0:
                        preyearTotal = total;
                        break;
                    case 1:
                        yearTotal = total;
                        break;
                    case 4:
                        nextyearTotal = total;
                        break;
                }
                startPosition += 80;
                InsertText(slide, total.ToString(), startPosition, 80, 50, 20, 10);
            }
            InsertText(slide, "Eur 0’", 600, 20, 50, 30, 14);
            slide.Shapes.AddShape(MsoAutoShapeType.msoShapeCurvedDownArrow, 130, 105, 100, 50);
            slide.Shapes.AddShape(MsoAutoShapeType.msoShapeCurvedDownArrow, 220, 105, 265, 50);

            string recent = String.Format("{0:N1}", 100 * (nextyearTotal - yearTotal) / yearTotal);
            InsertText(slide, recent, 310, 130, 50, 30, 14);
            InsertText(slide, meeting.getyear() + " Standard Exchange", 550, 515, 150, 20, 10);
        }
    }
     protected DataSet getSlide_TwoAdd()
    {
        string query_preyear = "SELECT '" + meeting.getpreyear().Trim() + "' AS ' '";
        string query_yearA = "SELECT '" + meeting.getyear().Trim() + "A' AS ' '";
        string query_yearB = "SELECT '" + meeting.getyear().Trim() + "B' AS ' '";
        string query_nextyearB = "SELECT '" + meeting.getnextyear().Trim() + "B' AS ' '";
        string query_nextyearF = "SELECT '" + meeting.getnextyear().Trim() + "F' AS ' '";
        string query_nextyearEB = "SELECT '" + meeting.getnextyear().Trim() + "W' AS ' '";
        for (int i = 0; i < segmentNumber; i++)
        {
            query_preyear += ",ROUND(SUM(CASE WHEN [Segment].Abbr='" + alSegment[i].ToString().Trim()
                          + "' THEN [ActualSalesandBL].Backlog*[Currency_Exchange].Rate2 ELSE 0 END)/1000,0)AS '"
                          + alSegment[i].ToString().Trim() + "'";
            query_yearA += ",ROUND(SUM(CASE WHEN [Segment].Abbr='" + alSegment[i].ToString().Trim()
                          + "' THEN [ActualSalesandBL].Backlog*[Currency_Exchange].Rate1 ELSE 0 END)/1000,0)AS '"
                          + alSegment[i].ToString().Trim() + "'";
            query_yearB += ",ROUND(SUM(CASE WHEN [Segment].Abbr='" + alSegment[i].ToString().Trim()
                          + "' THEN [ActualSalesandBL].Backlog*[Currency_Exchange].Rate2 ELSE 0 END)/1000,0)AS '"
                          + alSegment[i].ToString().Trim() + "'";
            query_nextyearB += ",ROUND(SUM(CASE WHEN [Segment].Abbr='" + alSegment[i].ToString().Trim()
                          + "' THEN [ActualSalesandBL].Backlog*[Currency_Exchange].Rate2 ELSE 0 END)/1000,0)AS '"
                          + alSegment[i].ToString().Trim() + "'";
            query_nextyearF += ",ROUND(SUM(CASE WHEN [Segment].Abbr='" + alSegment[i].ToString().Trim()
                          + "' THEN [ActualSalesandBL].Backlog*[Currency_Exchange].Rate2 ELSE 0 END)/1000,0)AS '"
                          + alSegment[i].ToString().Trim() + "'";
            query_nextyearEB += ",ROUND(SUM(CASE WHEN [Segment].Abbr='" + alSegment[i].ToString().Trim()
                          + "' THEN [ActualSalesandBL].Backlog*[Currency_Exchange].Rate2 ELSE 0 END)/1000,0)AS '"
                          + alSegment[i].ToString().Trim() + "'";
        }
        query_preyear += " FROM [ActualSalesandBL],[Segment],[SalesOrg],[Currency_Exchange]"
                      + " WHERE [ActualSalesandBL].BacklogY='" + meeting.getpreyear().Substring(2, 2).Trim() + "'"
                      + " AND YEAR([ActualSalesandBL].TimeFlag)='" + meeting.getpreyear().Trim() + "'"
                      + " AND MONTH([ActualSalesandBL].TimeFlag)='10'"
                      + " AND [Segment].Deleted='0'"
                      + " AND [ActualSalesandBL].SegmentID = [Segment].ID"
                      + " AND [ActualSalesandBL].SalesOrgID=[SalesOrg].ID"
                      + " AND [SalesOrg].CurrencyID=[Currency_Exchange].CurrencyID"
                      + " AND [Segment].Deleted='0'"
                      + " AND [SalesOrg].Deleted='0'"
                      + " AND [Currency_Exchange].Deleted='0'";
        query_yearA += " FROM [ActualSalesandBL],[Segment],[SalesOrg],[Currency_Exchange]"
                      + " WHERE [ActualSalesandBL].BacklogY='" + meeting.getyear().Substring(2, 2).Trim() + "'"
                      + " AND YEAR([ActualSalesandBL].TimeFlag)='" + meeting.getyear().Trim() + "'"
                      + " AND MONTH([ActualSalesandBL].TimeFlag)='" + month.Trim() + "'"
                      + " AND [Segment].Deleted='0'"
                      + " AND [ActualSalesandBL].SegmentID = [Segment].ID"
                      + " AND [ActualSalesandBL].SalesOrgID=[SalesOrg].ID"
                      + " AND [SalesOrg].CurrencyID=[Currency_Exchange].CurrencyID"
                      + " AND [Segment].Deleted='0'"
                      + " AND [SalesOrg].Deleted='0'"
                      + " AND [Currency_Exchange].Deleted='0'";
        query_yearB += " FROM [ActualSalesandBL],[Segment],[SalesOrg],[Currency_Exchange]"
                      + " WHERE [ActualSalesandBL].BacklogY='" + meeting.getyear().Substring(2, 2).Trim() + "'"
                      + " AND YEAR([ActualSalesandBL].TimeFlag)='" + meeting.getpreyear().Trim() + "'"
                      + " AND MONTH([ActualSalesandBL].TimeFlag)='03'"
                      + " AND [Segment].Deleted='0'"
                      + " AND [ActualSalesandBL].SegmentID = [Segment].ID"
                      + " AND [ActualSalesandBL].SalesOrgID=[SalesOrg].ID"
                      + " AND [SalesOrg].CurrencyID=[Currency_Exchange].CurrencyID"
                      + " AND [Segment].Deleted='0'"
                      + " AND [SalesOrg].Deleted='0'"
                      + " AND [Currency_Exchange].Deleted='0'";

        query_nextyearB += " FROM [ActualSalesandBL],[Segment],[SalesOrg],[Currency_Exchange]"
                      + " WHERE [ActualSalesandBL].BacklogY='" + meeting.getnextyear().Substring(2, 2).Trim() + "'"
                      + " AND YEAR([ActualSalesandBL].TimeFlag)='" + meeting.getyear().Trim() + "'"
                      + " AND MONTH([ActualSalesandBL].TimeFlag)='03'"
                      + " AND [Segment].Deleted='0'"
                      + " AND [ActualSalesandBL].SegmentID = [Segment].ID"
                      + " AND [ActualSalesandBL].SalesOrgID=[SalesOrg].ID"
                      + " AND [SalesOrg].CurrencyID=[Currency_Exchange].CurrencyID"
                      + " AND [Segment].Deleted='0'"
                      + " AND [SalesOrg].Deleted='0'"
                      + " AND [Currency_Exchange].Deleted='0'";

        query_nextyearF += " FROM [ActualSalesandBL],[Segment],[SalesOrg],[Currency_Exchange]"
                      + " WHERE [ActualSalesandBL].BacklogY='" + meeting.getnextyear().Substring(2, 2).Trim() + "'"
                      + " AND YEAR([ActualSalesandBL].TimeFlag)='" + meeting.getyear().Trim() + "'"
                      + " AND MONTH([ActualSalesandBL].TimeFlag)='" + month.Trim() + "'"
                      + " AND [Segment].Deleted='0'"
                      + " AND [ActualSalesandBL].SegmentID = [Segment].ID"
                      + " AND [ActualSalesandBL].SalesOrgID=[SalesOrg].ID"
                      + " AND [SalesOrg].CurrencyID=[Currency_Exchange].CurrencyID"
                      + " AND [Segment].Deleted='0'"
                      + " AND [SalesOrg].Deleted='0'"
                      + " AND [Currency_Exchange].Deleted='0'";
        query_nextyearEB += " FROM [ActualSalesandBL],[Segment],[SalesOrg],[Currency_Exchange]"
                      + " WHERE [ActualSalesandBL].BacklogY='" + meeting.getnextyear().Substring(2, 2).Trim() + "'"
                      + " AND YEAR([ActualSalesandBL].TimeFlag)='" + meeting.getyear().Trim() + "'"
                      + " AND MONTH([ActualSalesandBL].TimeFlag)='" + month.Trim() + "'"
                      + " AND [Segment].Deleted='0'"
                      + " AND [ActualSalesandBL].SegmentID = [Segment].ID"
                      + " AND [ActualSalesandBL].SalesOrgID=[SalesOrg].ID"
                      + " AND [SalesOrg].CurrencyID=[Currency_Exchange].CurrencyID"
                      + " AND [Segment].Deleted='0'"
                      + " AND [SalesOrg].Deleted='0'"
                      + " AND [Currency_Exchange].Deleted='0'";
        query_preyear += " UNION " + query_yearA + " UNION " + query_yearB
                      + " UNION " + query_nextyearB + " UNION "
                      + query_nextyearF + " UNION " + query_nextyearEB;
        DataSet ds = helper.GetDataSet(query_preyear);
        if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            return ds;
        else
            return null;
    }
     protected DataSet getSlide_Two()
    {
        string query_preyear = "SELECT '" + meeting.getpreyear().Trim() + "' AS ' '";
        string query_yearA = "SELECT '" + meeting.getyear().Trim() + "A' AS ' '";
        string query_yearB = "SELECT '" + meeting.getyear().Trim() + "B' AS ' '";
        string query_nextyearB = "SELECT '" + meeting.getnextyear().Trim() + "B' AS ' '";
        string query_nextyearF = "SELECT '" + meeting.getnextyear().Trim() + "F' AS ' '";
        string query_nextyearEB = "SELECT '" + meeting.getnextyear().Trim() + "W' AS ' '";
        for (int i = 0; i < segmentNumber; i++)
        {
            query_preyear += ",ROUND(0/1000,0) AS '" + alSegment[i].ToString().Trim() + "'";
            query_yearA += ",ROUND(0/1000,0) AS '" + alSegment[i].ToString().Trim() + "'";
            query_yearB += ",ROUND(SUM(CASE WHEN [Segment].Abbr='" + alSegment[i].ToString().Trim()
                          + "' THEN [Bookings].Amount*[Currency_Exchange].Rate2 ELSE 0 END)/1000,0)AS '"
                          + alSegment[i].ToString().Trim() + "'";
            query_nextyearB += ",ROUND(SUM(CASE WHEN [Segment].Abbr='" + alSegment[i].ToString().Trim()
                          + "' THEN [Bookings].Amount*[Currency_Exchange].Rate2 ELSE 0 END)/1000,0)AS '"
                          + alSegment[i].ToString().Trim() + "'";
            query_nextyearF += ",ROUND(SUM(CASE WHEN [Segment].Abbr='" + alSegment[i].ToString().Trim()
                          + "' THEN [Bookings].Amount*[Currency_Exchange].Rate2 ELSE 0 END)/1000,0)AS '"
                          + alSegment[i].ToString().Trim() + "'";
            query_nextyearEB += ",ROUND(SUM(CASE WHEN [Segment].Abbr='" + alSegment[i].ToString().Trim()
                          + "' THEN [ActualSalesandBL].Backlog*[Currency_Exchange].Rate2 ELSE 0 END)/1000,0) AS '"
                          + alSegment[i].ToString().Trim() + "'";
        }
        query_preyear += " FROM [Bookings]";
        query_yearA += " FROM [Bookings]";
        query_yearB += " FROM [Bookings],[SalesOrg],[SalesOrg_User],[Segment],[Currency_Exchange]"
                      + " WHERE [Bookings].DeliverY='" + meeting.getyear().Substring(2, 2).Trim() + "'"
                      + " AND ([Bookings].BookingY='" + meeting.getyear().Substring(2, 2).Trim()
                      + "' OR [Bookings].BookingY='" + meeting.getpreyear().Substring(2, 2).Trim() + "')"
                      + " AND YEAR([Bookings].TimeFlag)='" + meeting.getpreyear().Trim() + "'"
                      + " AND MONTH([Bookings].TimeFlag)='03'"
                      + " AND [Bookings].SegmentID=[Segment].ID"
                      + " AND [Bookings].RSMID=[SalesOrg_User].UserID"
                      + " AND [SalesOrg_User].SalesOrgID=[SalesOrg].ID"
                      + " AND [SalesOrg].CurrencyID=[Currency_Exchange].CurrencyID"
                      + " AND [Currency_Exchange].TimeFlag=[Bookings].TimeFlag"
                      + " AND [Segment].ID=[Bookings].SegmentID"
                      + " AND [Segment].Deleted='0'"
                      + " AND [SalesOrg].Deleted='0'"
                      + " AND [Currency_Exchange].Deleted='0'"
                      + " AND [SalesOrg_User].Deleted='0'";

        query_nextyearB += "FROM [Bookings],[SalesOrg],[SalesOrg_User],[Segment],[Currency_Exchange]"
                      + " WHERE [Bookings].DeliverY='" + meeting.getnextyear().Substring(2, 2).Trim() + "'"
                      + " AND ([Bookings].BookingY='" + meeting.getyear().Substring(2, 2).Trim()
                      + "' OR [Bookings].BookingY='" + meeting.getnextyear().Substring(2, 2).Trim() + "')"
                      + " AND YEAR([Bookings].TimeFlag)='" + meeting.getyear().Trim() + "'"
                      + " AND MONTH([Bookings].TimeFlag)='03'"
                      + " AND [Bookings].SegmentID=[Segment].ID"
                      + " AND [Bookings].RSMID=[SalesOrg_User].UserID"
                      + " AND [SalesOrg_User].SalesOrgID=[SalesOrg].ID"
                      + " AND [SalesOrg].CurrencyID=[Currency_Exchange].CurrencyID"
                      + " AND [Currency_Exchange].TimeFlag=[Bookings].TimeFlag"
                      + " AND [Segment].ID=[Bookings].SegmentID"
                      + " AND [Segment].Deleted='0'"
                      + " AND [SalesOrg].Deleted='0'"
                      + " AND [Currency_Exchange].Deleted='0'"
                      + " AND [SalesOrg_User].Deleted='0'";

        query_nextyearF += "FROM [Bookings],[SalesOrg],[SalesOrg_User],[Segment],[Currency_Exchange]"
                      + " WHERE [Bookings].DeliverY='" + meeting.getnextyear().Substring(2, 2).Trim() + "'"
                      + " AND ([Bookings].BookingY='" + meeting.getyear().Substring(2, 2).Trim()
                      + "' OR [Bookings].BookingY='" + meeting.getnextyear().Substring(2, 2).Trim() + "')"
                      + " AND YEAR([Bookings].TimeFlag)='" + meeting.getyear().Trim() + "'"
                      + " AND MONTH([Bookings].TimeFlag)='" + month.Trim() + "'"
                      + " AND [Bookings].SegmentID=[Segment].ID"
                      + " AND [Bookings].RSMID=[SalesOrg_User].UserID"
                      + " AND [SalesOrg_User].SalesOrgID=[SalesOrg].ID"
                      + " AND [SalesOrg].CurrencyID=[Currency_Exchange].CurrencyID"
                      + " AND [Currency_Exchange].TimeFlag=[Bookings].TimeFlag"
                      + " AND [Segment].ID=[Bookings].SegmentID"
                      + " AND [Segment].Deleted='0'"
                      + " AND [SalesOrg].Deleted='0'"
                      + " AND [Currency_Exchange].Deleted='0'"
                      + " AND [SalesOrg_User].Deleted='0'";

        query_nextyearEB += " FROM [ActualSalesandBL],[Segment],[SalesOrg],[Currency_Exchange]"
                      + " WHERE [ActualSalesandBL].BacklogY='" + meeting.getnextyear().Substring(2, 2).Trim() + "'"
                      + " AND YEAR([ActualSalesandBL].TimeFlag)='" + meeting.getpreyear().Trim() + "'"
                      + " AND MONTH([ActualSalesandBL].TimeFlag)='" + month.Trim() + "'"
                      + " AND [Segment].Deleted='0'"
                      + " AND [ActualSalesandBL].SegmentID = [Segment].ID"
                      + " AND [SalesOrg].CurrencyID=[Currency_Exchange].CurrencyID"
                      + " AND [SalesOrg].ID=[ActualSalesandBL].SegmentID"
                      + " AND [Segment].Deleted='0'"
                      + " AND [SalesOrg].Deleted='0'"
                      + " AND [Currency_Exchange].Deleted='0'";
        query_preyear += " UNION " + query_yearA + " UNION " + query_yearB
                     + " UNION " + query_nextyearB + " UNION " + query_nextyearF
                     + " UNION " + query_nextyearEB;
        DataSet ds = helper.GetDataSet(query_preyear);
        if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            return ds;
        else
            return null;
    }
     //------------------------------------------------------------------
     protected void DrawSlide_Three()
    {
        slides.Add(++slideIndex, Microsoft.Office.Interop.PowerPoint.PpSlideLayout.ppLayoutTitleOnly);
        PowerPoint.Slide slide = slides[slideIndex];
        UniformLayout(slide, "Bookings");
        ChartSpace objSpace = new ChartSpace();
        ChChart objChart = objSpace.Charts.Add(0);
        objChart.Type = ChartChartTypeEnum.chChartTypeColumnStacked;
        objChart.HasLegend = true;
        string dimCategories = "";

        DataSet ds = getSlide_Three();
        if (ds != null)
        {
            for (int k = 0; k < ds.Tables[0].Rows.Count; k++)
            {
                dimCategories += ds.Tables[0].Rows[k][0].ToString() + "\t";
            }
            dimCategories = dimCategories.Substring(0, dimCategories.Length - 1);
            for (int i = 0; i < segmentNumber; i++)
            {
                string dimValues = "";
                for (int g = 0; g < ds.Tables[0].Rows.Count; g++)
                {
                    var num1 = ds.Tables[0].Rows[g][i + 1];
                    dimValues += (num1 is DBNull ? "0\t" : num1.ToString() + "\t");
                }

                dimValues = dimValues.Substring(0, dimValues.Length - 1);
                objChart.SeriesCollection.Add(i);
                objChart.SeriesCollection[i].Caption = alSegment[i].ToString();

                objChart.SeriesCollection[i].SetData(ChartDimensionsEnum.chDimCategories,
                       +(int)ChartSpecialDataSourcesEnum.chDataLiteral, dimCategories);
                objChart.SeriesCollection[i].SetData(ChartDimensionsEnum.chDimValues,
                        +(int)ChartSpecialDataSourcesEnum.chDataLiteral, dimValues);
            }
            picturePath = pictureFolderPath + pictureName;
            objSpace.ExportPicture(picturePath, "GIF", 500, 350);
            InsertPicture(slide, picturePath, 60, 160, 600, 350);
            float startPosition = 27;
            double preyearTotal = 0;
            double yearTotal = 0;
            double nextyearTotal = 0;
            for (int m = 0; m < ds.Tables[0].Rows.Count; m++)
            {
                double total = 0;
                for (int n = 1; n < ds.Tables[0].Columns.Count; n++)
                {
                    var num1 = ds.Tables[0].Rows[m][n];
                    total += (num1 is System.DBNull ? 0 : (double)num1);
                }
                startPosition += 95;
                InsertText(slide, total.ToString(), startPosition, 80, 50, 20, 10);
                switch (m)
                {
                    case 0:
                        preyearTotal = total;
                        break;
                    case 1:
                        yearTotal = total;
                        break;
                    case 4:
                        nextyearTotal = total;
                        break;
                }
            }
            InsertText(slide, "Eur 0’", 600, 20, 50, 30, 14);
            slide.Shapes.AddShape(MsoAutoShapeType.msoShapeCurvedDownArrow, 130, 105, 100, 50);
            slide.Shapes.AddShape(MsoAutoShapeType.msoShapeCurvedDownArrow, 220, 105, 255, 50);

            string recent = String.Format("{0:N1}", 100 * (nextyearTotal - yearTotal) / yearTotal);
            InsertText(slide, recent, 310, 130, 50, 30, 14);
            InsertText(slide, meeting.getyear() + " Standard Exchange", 550, 515, 150, 20, 10);
        }
    }

     protected DataSet getSlide_Three()
    {
        string query_preyear = "SELECT '" + meeting.getpreyear().Trim() + "' AS ' '";
        string query_yearA = "SELECT '" + meeting.getyear().Trim() + "A' AS ' '";
        string query_yearB = "SELECT '" + meeting.getyear().Trim() + "B' AS ' '";
        string query_nextyearB = "SELECT '" + meeting.getnextyear().Trim() + "B' AS ' '";
        string query_nextyearF = "SELECT '" + meeting.getnextyear().Trim() + "F'AS ' '";
        string str = "";
        for (int i = 0; i < segmentNumber; i++)
        {
            query_preyear += ",ROUND(SUM(CASE WHEN [Segment].Abbr='" + alSegment[i].ToString().Trim()
                          + "' THEN [ActualSalesandBL].Backlog*[Currency_Exchange].Rate2 ELSE 0 END)/1000,0)AS '"
                          + alSegment[i].ToString().Trim() + "'";
            str += ",ROUND(SUM(CASE WHEN [Segment].Abbr='" + alSegment[i].ToString() + "'"
                          + " THEN ([Bookings].Amount*(CASE WHEN ([Bookings].DeliverY='YTD'"
                          + " OR [Bookings].BookingY='" + meeting.getyear().Substring(2, 2).Trim() + "') "
                          + "THEN [Currency_Exchange].Rate1 ELSE [Currency_Exchange].Rate2 END))ELSE 0 END)/1000,0)";
        }
        query_preyear += " FROM [ActualSalesandBL],[SalesOrg],[Currency_Exchange],[Segment]"
                      + " WHERE [ActualSalesandBL].BacklogY='" + meeting.getpreyear().Substring(2, 2).Trim() + "'"
                      + " AND YEAR([ActualSalesandBL].TimeFlag)='" + meeting.getpreyear().Trim() + "'"
                      + " AND MONTH([ActualSalesandBL].TimeFlag)='" + month.Trim() + "'"
                      + " AND [Segment].Deleted='0'"
                      + " AND [ActualSalesandBL].SegmentID = [Segment].ID"
                      + " AND [SalesOrg].CurrencyID=[Currency_Exchange].CurrencyID"
                      + " AND [SalesOrg].ID=[ActualSalesandBL].SegmentID"
                      + " AND [SalesOrg].Deleted='0'"
                      + " AND [Segment].Deleted='0'"
                      + " AND [Currency_Exchange].Deleted='0'";

        query_yearA += str + " FROM [Bookings],[SalesOrg],[SalesOrg_User],[Currency_Exchange],[Segment]"
                      + " WHERE [Bookings].BookingY='" + meeting.getyear().Substring(2, 2).Trim() + "'"
                      + " AND YEAR([Bookings].TimeFlag)='" + meeting.getyear().Trim() + "'"
                      + " AND MONTH([Bookings].TimeFlag)='" + month.Trim() + "'"
                      + " AND [Bookings].SegmentID=[Segment].ID"
                      + " AND [Bookings].RSMID=[SalesOrg_User].UserID"
                      + " AND [SalesOrg_User].SalesOrgID=[SalesOrg].ID"
                      + " AND [SalesOrg].CurrencyID=[Currency_Exchange].CurrencyID"
                      + " AND [Currency_Exchange].TimeFlag=[Bookings].TimeFlag"
                      + " AND [Segment].ID=[Bookings].SegmentID"
                      + " AND [SalesOrg].Deleted='0'"
                      + " AND [Segment].Deleted='0'"
                      + " AND [Currency_Exchange].Deleted='0'"
                      + " AND [SalesOrg_User].Deleted='0'";
        query_yearB += str + " FROM [Bookings],[SalesOrg],[SalesOrg_User],[Currency_Exchange],[Segment]"
                      + " WHERE [Bookings].BookingY='" + meeting.getyear().Substring(2, 2).Trim() + "'"
                      + " AND ([Bookings].DeliverY='" + meeting.getyear().Substring(2, 2).Trim() + "'"
                      + " OR [Bookings].DeliverY='" + meeting.getnextyear().Substring(2, 2).Trim() + "')"
                      + " AND YEAR([Bookings].TimeFlag)='" + meeting.getpreyear().Trim() + "'"
                      + " AND MONTH([Bookings].TimeFlag)='03'"
                      + " AND [Bookings].SegmentID=[Segment].ID"
                      + " AND [Bookings].RSMID=[SalesOrg_User].UserID"
                      + " AND [SalesOrg_User].SalesOrgID=[SalesOrg].ID"
                      + " AND [SalesOrg].CurrencyID=[Currency_Exchange].CurrencyID"
                      + " AND [Currency_Exchange].TimeFlag=[Bookings].TimeFlag"
                      + " AND [Segment].ID=[Bookings].SegmentID"
                      + " AND [SalesOrg].Deleted='0'"
                      + " AND [Segment].Deleted='0'"
                      + " AND [Currency_Exchange].Deleted='0'"
                      + " AND [SalesOrg_User].Deleted='0'";

        query_nextyearB += str + " FROM [Bookings],[SalesOrg],[SalesOrg_User],[Currency_Exchange],[Segment]"
                      + " WHERE [Bookings].BookingY='" + meeting.getnextyear().Substring(2, 2).Trim() + "'"
                      + " AND ([Bookings].DeliverY='" + meeting.getnextyear().Substring(2, 2).Trim() + "'"
                      + " OR [Bookings].DeliverY='" + yearAfterNext.Substring(2, 2).Trim() + "')"
                      + " AND YEAR([Bookings].TimeFlag)='" + meeting.getyear().Trim() + "'"
                      + " AND MONTH([Bookings].TimeFlag)='03'"
                      + " AND [Bookings].SegmentID=[Segment].ID"
                      + " AND [Bookings].RSMID=[SalesOrg_User].UserID"
                      + " AND [SalesOrg_User].SalesOrgID=[SalesOrg].ID"
                      + " AND [SalesOrg].CurrencyID=[Currency_Exchange].CurrencyID"
                      + " AND [Currency_Exchange].TimeFlag=[Bookings].TimeFlag"
                      + " AND [Segment].ID=[Bookings].SegmentID"
                      + " AND [SalesOrg].Deleted='0'"
                      + " AND [Segment].Deleted='0'"
                      + " AND [Currency_Exchange].Deleted='0'"
                      + " AND [SalesOrg_User].Deleted='0'";

        query_nextyearF += str + " FROM [Bookings],[SalesOrg],[SalesOrg_User],[Currency_Exchange],[Segment]"
                      + " WHERE [Bookings].BookingY='" + meeting.getnextyear().Substring(2, 2).Trim() + "'"
                      + " AND ([Bookings].DeliverY='" + meeting.getnextyear().Substring(2, 2).Trim() + "'"
                      + " OR [Bookings].DeliverY='" + yearAfterNext.Substring(2, 2).Trim() + "')"
                      + " AND YEAR([Bookings].TimeFlag)='" + meeting.getyear().Trim() + "'"
                      + " AND MONTH([Bookings].TimeFlag)='" + month.Trim() + "'"
                      + " AND [Bookings].SegmentID=[Segment].ID"
                      + " AND [Bookings].RSMID=[SalesOrg_User].UserID"
                      + " AND [SalesOrg_User].SalesOrgID=[SalesOrg].ID"
                      + " AND [SalesOrg].CurrencyID=[Currency_Exchange].CurrencyID"
                      + " AND [Currency_Exchange].TimeFlag=[Bookings].TimeFlag"
                      + " AND [Segment].ID=[Bookings].SegmentID"
                      + " AND [SalesOrg].Deleted='0'"
                      + " AND [Segment].Deleted='0'"
                      + " AND [Currency_Exchange].Deleted='0'"
                      + " AND [SalesOrg_User].Deleted='0'";
        query_preyear += " UNION " + query_yearA + " UNION " + query_yearB
                     + " UNION " + query_nextyearB + " UNION " + query_nextyearF;
        DataSet ds = helper.GetDataSet(query_preyear);

        if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            return ds;
        else
            return null;
    }
     //--------------------------------------------------------
     protected void DrawSlide_Four()
     {
         int slideNum = 0;
         int chartsNum = 0;
         slideNum = (segmentNumber / 2) + (segmentNumber % 2 == 0 ? 0 : 1);
         for (int j = 0; j < slideNum; j++)
         {
             bool isNull = false;
             slides.Add(++slideIndex, Microsoft.Office.Interop.PowerPoint.PpSlideLayout.ppLayoutTitleOnly);
             PowerPoint.Slide slide = slides[slideIndex];
             UniformLayout(slide, "HP " + meeting.getyear().Trim() + " - " + meeting.getnextyear().Trim() + " Bookings");
             InsertText(slide, "5 Top Countries", 215, 50, 260, 35, 24);
             DataSet[] ds = new DataSet[4];
             DataSet[] ds_FourTotal = new DataSet[4];
             ds[0] = getSlide_Four(meeting.getyear(), alSegment[2 * j].ToString());
             ds_FourTotal[0] = getSlide_FourTotal(meeting.getyear(), alSegment[2 * j].ToString());
             ds[1] = getSlide_Four(meeting.getnextyear(), alSegment[2 * j].ToString());
             ds_FourTotal[1] = getSlide_FourTotal(meeting.getnextyear(), alSegment[2 * j].ToString());
             if ((2 * j + 1) < segmentNumber)
             {
                 ds[2] = getSlide_Four(meeting.getyear(), alSegment[2 * j + 1].ToString());
                 ds_FourTotal[2] = getSlide_FourTotal(meeting.getyear(), alSegment[2 * j + 1].ToString());
                 ds[3] = getSlide_Four(meeting.getnextyear(), alSegment[2 * j + 1].ToString());
                 ds_FourTotal[3] = getSlide_FourTotal(meeting.getnextyear(), alSegment[2 * j + 1].ToString());
             }
             chartsNum = (segmentNumber % 2 == 1 && j == (slideNum - 1) ? 2 : 4);
             for (int k = 0; k < chartsNum; k++)
             {
                 if (ds[k].Tables.Count > 0 && ds[k].Tables[0].Rows.Count > 0)
                 {
                     isNull = true;
                     ChartSpace objSpace = new ChartSpace();
                     ChChart objChart = objSpace.Charts.Add(0);
                     objChart.Type = ChartChartTypeEnum.chChartTypePie3D;
                     objChart.HasLegend = true;
                     objChart.Legend.Position = ChartLegendPositionEnum.chLegendPositionLeft;
                     if (k == 0)
                     {
                         objChart.HasTitle = true;
                         objChart.Title.Caption = alSegment[2 * j].ToString().Trim();
                     }
                     if (k == 2)
                     {
                         objChart.HasTitle = true;
                         objChart.Title.Caption = alSegment[2 * j + 1].ToString().Trim();
                     }
                     objChart.SeriesCollection.Add(0);
                     //objChart.SeriesCollection[0].Caption = titile.ToString();
                     objChart.SeriesCollection[0].DataLabelsCollection.Add();

                     string dimCategories = "";
                     string dimValues = "";
                     double total = (double)ds_FourTotal[k].Tables[0].Rows[0][0];
                     double total1 = 0;
                     for (int m = 0; m < ds[k].Tables[0].Rows.Count; m++)
                     {
                         total1 += (double)ds[k].Tables[0].Rows[m][1];
                         dimCategories += ds[k].Tables[0].Rows[m][0].ToString() + "\t";
                         dimValues += String.Format("{0:F}", 100 * (double)ds[k].Tables[0].Rows[m][1] / total) + "\t";
                     }
                     dimCategories += "Others";
                     dimValues += String.Format("{0:F}", 100 * (total - total1) / total);


                     objChart.SeriesCollection[0].SetData(ChartDimensionsEnum.chDimCategories,
                            +(int)ChartSpecialDataSourcesEnum.chDataLiteral, dimCategories);
                     objChart.SeriesCollection[0].SetData(ChartDimensionsEnum.chDimValues,
                         +(int)ChartSpecialDataSourcesEnum.chDataLiteral, dimValues);
                     objChart.SeriesCollection[0].DataLabelsCollection[0].HasPercentage = true;
                     objChart.SeriesCollection[0].DataLabelsCollection[0].HasValue = false;
                     objChart.SeriesCollection[0].DataLabelsCollection[0].Position = Microsoft.Office.Interop.Owc11.ChartDataLabelPositionEnum.chLabelPositionCenter;
                     objSpace.ExportPicture(picturePath, "GIF", 260, 200);
                     picturePath = pictureFolderPath + pictureName;
                     if (ds[0].Tables[0].Rows.Count == 0 && ds[1].Tables[0].Rows.Count == 0)
                     {
                         x_four[2] = 215;
                         x_four[3] = 215;
                     }
                     InsertPicture(slide, picturePath, x_four[k], y_four[k], 260, 180);
                 }
             }
             slide.Shapes.AddLine(0, 290, 720, 290);
             InsertText(slide, meeting.getyear().Trim(), 95, 100, 60, 25, 14);
             InsertText(slide, meeting.getnextyear().Trim(), 95, 320, 60, 25, 14);
             if (isNull == false)
             {
                 slideIndex--;
             }
         }
     }

     protected string getCountryName(string id)
     {
         string query_countryName = "SELECT [Country].Name FROM [Country] WHERE [Country].ID='"
                                  + id + "' AND [Country].Deleted='0'";
         DataSet ds_countryName = helper.GetDataSet(query_countryName);
         if (ds_countryName.Tables[0].Rows.Count > 0)
         {
             return ds_countryName.Tables[0].Rows[0][0].ToString();
         }
         else
             return null;
     }
     protected DataSet getSlide_Four(string period, string abbr)
     {
         string query_SlideFour = "";
         if (period == meeting.getyear())
         {
             query_SlideFour = " SELECT TOP 5 [Country].Name AS 'Name'"
                                + " ,ROUND(SUM(Amount*(CASE WHEN [Bookings].TimeFlag=[Currency_Exchange].TimeFlag"
                                + " AND ([Bookings].DeliverY='YTD' OR [Bookings].DeliverY='"
                                + meeting.getyear().Substring(2, 2).Trim() + "') THEN [Currency_Exchange].Rate1"
                                + " ELSE [Currency_Exchange].Rate2 END))/1000,0) AS '" + abbr + "'"
                                + " FROM [Bookings],[Segment],[Country],[SalesOrg],[SalesOrg_User],[Currency_Exchange]"
                                + " WHERE [Bookings].BookingY='" + meeting.getyear().Substring(2, 2).Trim() + "'"
                                + " AND [Bookings].SegmentID=[Segment].ID"
                                + " AND [Bookings].CountryID=[Country].ID"
                                + " AND [SalesOrg_User].UserID=[Bookings].RSMID"
                                + " AND [SalesOrg_User].SalesOrgID=[SalesOrg].ID"
                                + " AND [SalesOrg].CurrencyID=[Currency_Exchange].CurrencyID"
                                + " AND [Segment].Abbr='" + abbr.ToString().Trim() + "'"
                                + " AND [Segment].Deleted='0'"
                                + " AND [Country].Deleted='0'"
                                + " AND [SalesOrg].Deleted='0'"
                                + " AND [SalesOrg_User].Deleted='0'"
                                + " AND [Currency_Exchange].Deleted='0'"
                                + " GROUP BY [Country].Name"
                                + " ORDER BY SUM(Amount*(CASE WHEN [Bookings].TimeFlag=[Currency_Exchange].TimeFlag"
                                + " AND ([Bookings].DeliverY='YTD' OR [Bookings].DeliverY='"
                                + meeting.getyear().Substring(2, 2).Trim() + "') THEN [Currency_Exchange].Rate1"
                                + " ELSE [Currency_Exchange].Rate2 END)) DESC";
         }
         else
         {
             query_SlideFour = " SELECT TOP 5 [Country].Name AS 'Name'"
                                + " ,ROUND(SUM(Amount*[Currency_Exchange].Rate2)/1000,0) AS '" + abbr + "'"
                                + " FROM [Bookings],[Segment],[Country],[SalesOrg],[SalesOrg_User],[Currency_Exchange]"
                                + " WHERE [Bookings].BookingY='" + meeting.getnextyear().Substring(2, 2).Trim() + "'"
                                + " AND ([Bookings].DeliverY='" + meeting.getnextyear().Substring(2, 2).Trim() + "'"
                                + " OR [Bookings].DeliverY='" + yearAfterNext.Substring(2, 2).Trim() + "')"
                                + " AND YEAR([Bookings].TimeFlag)='" + meeting.getyear().Trim() + "'"
                                + " AND MONTH([Bookings].TimeFlag)='" + month.Trim() + "'"
                                + " AND [Bookings].SegmentID=[Segment].ID"
                                + " AND [Bookings].CountryID=[Country].ID"
                                + " AND [SalesOrg_User].UserID=[Bookings].RSMID"
                                + " AND [SalesOrg_User].SalesOrgID=[SalesOrg].ID"
                                + " AND [SalesOrg].CurrencyID=[Currency_Exchange].CurrencyID"
                                + " AND [Segment].Abbr='" + abbr.ToString().Trim() + "'"
                                + " AND [Segment].Deleted='0'"
                                + " AND [Country].Deleted='0'"
                                + " AND [SalesOrg].Deleted='0'"
                                + " AND [SalesOrg_User].Deleted='0'"
                                + " AND [Currency_Exchange].Deleted='0'"
                                + " GROUP BY [Country].Name"
                                + " ORDER BY SUM(Amount*(CASE WHEN [Bookings].TimeFlag=[Currency_Exchange].TimeFlag"
                                + " AND ([Bookings].DeliverY='YTD' OR [Bookings].DeliverY='"
                                + meeting.getyear().Substring(2, 2).Trim() + "') THEN [Currency_Exchange].Rate1"
                                + " ELSE [Currency_Exchange].Rate2 END)) DESC";
         }
         DataSet ds = helper.GetDataSet(query_SlideFour);
         return ds;
     }
     protected DataSet getSlide_FourTotal(string period, string abbr)
     {
         string query_SlideFour = "";
         if (period == meeting.getyear())
         {
             query_SlideFour = " SELECT ROUND(SUM(Amount*(CASE WHEN [Bookings].TimeFlag=[Currency_Exchange].TimeFlag"
                                    + " AND ([Bookings].DeliverY='YTD' OR [Bookings].DeliverY='"
                                    + meeting.getyear().Substring(2, 2).Trim() + "') THEN [Currency_Exchange].Rate1"
                                    + " ELSE [Currency_Exchange].Rate2 END))/1000,0) AS '" + abbr.ToString().Trim() + "'"
                                    + " FROM [Bookings],[Segment],[SalesOrg],[SalesOrg_User],[Currency_Exchange]"
                                    + " WHERE [Bookings].BookingY='" + meeting.getyear().Substring(2, 2).Trim() + "'"
                                    + " AND[Bookings].SegmentID=[Segment].ID"
                                    + " AND [Segment].Abbr='" + abbr.ToString().Trim() + "'"
                                    + " AND [SalesOrg_User].UserID=[Bookings].RSMID"
                                    + " AND [SalesOrg_User].SalesOrgID=[SalesOrg].ID"
                                    + " AND [SalesOrg].CurrencyID=[Currency_Exchange].CurrencyID"
                                    + " AND [Segment].Deleted='0'"
                                    + " AND [SalesOrg].Deleted='0'"
                                    + " AND [SalesOrg_User].Deleted='0'"
                                    + " AND [Currency_Exchange].Deleted='0'";
         }
         else
         {
             query_SlideFour = " SELECT ROUND(SUM(Amount*[Currency_Exchange].Rate2)/1000,0) AS '" + abbr.ToString().Trim() + "'"
                                    + " FROM [Bookings],[Segment],[SalesOrg],[SalesOrg_User],[Currency_Exchange]"
                                    + " WHERE [Bookings].BookingY='" + meeting.getnextyear().Substring(2, 2).Trim() + "'"
                                    + " AND ([Bookings].DeliverY='" + meeting.getnextyear().Substring(2, 2).Trim() + "'"
                                    + " OR [Bookings].DeliverY='" + yearAfterNext.Substring(2, 2).Trim() + "')"
                                    + " AND YEAR([Bookings].TimeFlag)='" + meeting.getyear().Trim() + "'"
                                    + " AND MONTH([Bookings].TimeFlag)='" + month.Trim() + "'"
                                    + " AND[Bookings].SegmentID=[Segment].ID"
                                    + " AND [SalesOrg_User].UserID=[Bookings].RSMID"
                                    + " AND [SalesOrg_User].SalesOrgID=[SalesOrg].ID"
                                    + " AND [SalesOrg].CurrencyID=[Currency_Exchange].CurrencyID"
                                    + " AND [Segment].Abbr='" + abbr.ToString().Trim() + "'"
                                    + " AND [Segment].Deleted='0'"
                                    + " AND [SalesOrg].Deleted='0'"
                                    + " AND [SalesOrg_User].Deleted='0'"
                                    + " AND [Currency_Exchange].Deleted='0'";
         }
         DataSet ds = helper.GetDataSet(query_SlideFour);
         return ds;
     }
     // -------------------------------------------------------------
     protected void DrawSlide_Seven()
     {
         int slideNum = segmentNumber / 3 + (segmentNumber % 3 == 0 ? 0 : 1);
         for (int i = 0; i < slideNum; i++)
         {
             bool isNull = false;
             slides.Add(++slideIndex, Microsoft.Office.Interop.PowerPoint.PpSlideLayout.ppLayoutTitleOnly);
             PowerPoint.Slide slide = slides[slideIndex];
             UniformLayout(slide, "BOOKING BY REGIONS");
             int chartNum = 3;
             if (i == (slideNum - 1))
             {
                 switch (segmentNumber % 3)
                 {
                     case 1:
                         chartNum = 1;
                         break;
                     case 2:
                         chartNum = 2;
                         break;
                 }
             }

             DataSet[] ds = new DataSet[chartNum];
             for (int j = 0; j < chartNum; j++)
             {
                 float[] x = { 60, 370, 215 };
                 float[] y = { 80, 80, 380 };
                 ds[j] = getSlide_Seven(alSegment[3 * i + j].ToString());
                 if (ds[j].Tables.Count > 0 && ds[j].Tables[0].Rows.Count > 0)
                 {
                     isNull = true;
                     ChartSpace objSpace = new ChartSpace();
                     ChChart objChart = objSpace.Charts.Add(0);
                     objChart.Type = ChartChartTypeEnum.chChartTypeColumnClustered;
                     string dimValuesYear = "";
                     string dimValuesNextYear = "";
                     for (int k = 0; k < regionNumber; k++)
                     {
                         dimValuesYear += ds[j].Tables[0].Rows[0][k + 1].ToString().Trim() + "\t";
                         if (ds[j].Tables[0].Rows.Count > 1)
                             dimValuesNextYear += ds[j].Tables[0].Rows[1][k + 1].ToString().Trim() + "\t";
                         else
                             dimValuesNextYear += " \t";
                     }
                     for (int l = 0; l < 2; l++)
                     {
                         objChart.SeriesCollection.Add(l);
                         objChart.HasTitle = true;
                         objChart.HasLegend = true;
                         objChart.Title.Font.Size = 12;
                         objChart.Title.Font.Bold = true;

                         objChart.Title.Caption = alSegment[3 * i + j].ToString().Trim() + " Regions";
                         if (ds[j].Tables[0].Rows.Count > 1)
                         {
                             objChart.SeriesCollection[l].Caption = ds[j].Tables[0].Rows[l][0].ToString();
                             objChart.SeriesCollection[l].SetData(ChartDimensionsEnum.chDimCategories,
                                        +(int)ChartSpecialDataSourcesEnum.chDataLiteral, dimRegionCategories);
                         }
                     }
                     objChart.SeriesCollection[0].SetData(ChartDimensionsEnum.chDimValues,
                                 +(int)ChartSpecialDataSourcesEnum.chDataLiteral, dimValuesYear);
                     objChart.SeriesCollection[1].SetData(ChartDimensionsEnum.chDimValues,
                                 +(int)ChartSpecialDataSourcesEnum.chDataLiteral, dimValuesNextYear);

                     objSpace.ExportPicture(picturePath, "GIF", 500, 400);
                     picturePath = pictureFolderPath + pictureName;
                     if (chartNum == 3)
                     {
                         if (ds[0].Tables.Count == 0 || ds[0].Tables[0].Rows.Count == 0)
                         {
                             x[1] = x[0];
                             y[1] = y[0];
                         }
                         if (ds[0].Tables.Count == 0 || ds[0].Tables[0].Rows.Count == 0)
                         {
                             x[2] = x[1];
                             y[2] = y[1];
                         }
                     }
                     InsertPicture(slide, picturePath, x[j], y[j], 500, 400);
                 }

             }
             if (isNull == false)
             {
                 slideIndex--;
             }
         }
     }
     protected DataSet getSlide_Seven(string str_segment)
     {
         string query_SlideSevenYear = "SELECT '" + meeting.getyear().Substring(2, 2).Trim() + " A' AS ' '";
         string query_SlideSevenNextYear = "SELECT '" + meeting.getnextyear().Substring(2, 2).Trim() + " F' AS ' '";
         string str = "";
         for (int i = 0; i < regionNumber; i++)
         {
             str += ",ROUND(SUM(CASE WHEN [Region_Cluster].RegionID='" + alRegionID[i].ToString().Trim() + "'"
                                   + " THEN ([Bookings].Amount*(CASE WHEN ([Bookings].DeliverY='YTD'"
                                   + " OR [Bookings].DeliverY='" + meeting.getyear().Substring(2, 2).Trim() + "')"
                                   + " THEN [Currency_Exchange].Rate1 ELSE [Currency_Exchange].Rate2 END))"
                                   + " ELSE 0 END)/1000,0) AS '" + alRegion[i].ToString().Trim() + "'";
         }
         query_SlideSevenYear += str + " FROM [Bookings],[Region_Cluster],[Cluster_Country],[Segment],[SalesOrg_User],[SalesOrg],[Currency_Exchange]"
                               + " WHERE [Bookings].BookingY='" + meeting.getyear().Substring(2, 2).Trim() + "'"
                               + " AND YEAR([Bookings].TimeFlag)='" + meeting.getyear().Trim() + "'"
                               + " AND MONTH([Bookings].TimeFlag)='" + month.Trim() + "'"
                               + " AND [Region_Cluster].ClusterID=[Cluster_Country].ClusterID "
                               + " AND [Cluster_Country].CountryID=[Bookings].CountryID"
                               + " AND [Bookings].SegmentID=[Segment].ID"
                               + " AND [SalesOrg_User].UserID=[Bookings].RSMID"
                               + " AND [SalesOrg_User].SalesOrgID=[SalesOrg].ID"
                               + " AND [SalesOrg].CurrencyID=[Currency_Exchange].CurrencyID"
                               + " AND Region_Cluster.Deleted=0 "
                               + " AND Cluster_Country.Deleted=0 "
                               + " AND Segment.Deleted=0 "
                               + " AND SalesOrg_User.Deleted=0 "
                               + " AND SalesOrg.Deleted=0 "
                               + " AND Currency_Exchange.Deleted=0 "
                               + " AND [Segment].Abbr='" + str_segment + "'"
                               + " GROUP BY [Bookings].BookingY";
         query_SlideSevenNextYear += str + " FROM [Bookings],[Region_Cluster],[Cluster_Country],[Segment],[SalesOrg],[SalesOrg_User],[Currency_Exchange]"
                                   + " WHERE [Bookings].BookingY='" + meeting.getyear().Substring(2, 2).Trim() + "'"
                                   + " AND ([Bookings].DeliverY='" + meeting.getnextyear().Substring(2, 2).Trim() + "'"
                                   + " OR [Bookings].DeliverY='" + yearAfterNext.Substring(2, 2).Trim() + "')"
                                   + " AND YEAR([Bookings].TimeFlag)='" + meeting.getyear().Trim() + "'"
                                   + " AND MONTH([Bookings].TimeFlag)='" + month.Trim() + "'"
                                   + " AND [Region_Cluster].ClusterID=[Cluster_Country].ClusterID "
                                   + " AND [Cluster_Country].CountryID=[Bookings].CountryID"
                                   + " AND [Bookings].SegmentID=[Segment].ID"
                                   + " AND [SalesOrg_User].UserID=[Bookings].RSMID"
                                   + " AND [SalesOrg_User].SalesOrgID=[SalesOrg].ID"
                                   + " AND [SalesOrg].CurrencyID=[Currency_Exchange].CurrencyID"
                                   + " AND Region_Cluster.Deleted=0 "
                                   + " AND Cluster_Country.Deleted=0 "
                                   + " AND Segment.Deleted=0 "
                                   + " AND SalesOrg_User.Deleted=0 "
                                   + " AND SalesOrg.Deleted=0 "
                                   + " AND Currency_Exchange.Deleted=0 "
                                   + " AND [Segment].Abbr='" + str_segment + "'"
                                   + " GROUP BY [Bookings].BookingY";
         query_SlideSevenYear += " UNION " + query_SlideSevenNextYear;

         DataSet ds = helper.GetDataSet(query_SlideSevenYear);
         return ds;
     }
     //--------------------------------------------------------------------------
     protected void DrawSlide_Nine()
     {
         slides.Add(++slideIndex, Microsoft.Office.Interop.PowerPoint.PpSlideLayout.ppLayoutTitleOnly);
         PowerPoint.Slide slide = slides[slideIndex];
         UniformLayout(slide, "BOOKING BY REGIONS");
         ChartSpace objSpace = new ChartSpace();
         ChChart objChart = objSpace.Charts.Add(0);
         DataSet ds_dimValues = getSlide_Nine();
         if (ds_dimValues.Tables[0].Rows.Count > 0)
         {

             objChart.Type = ChartChartTypeEnum.chChartTypeColumnClustered;
             //string dimCategories = "";
             string dimValuesYear = "";
             string dimValuesNextYear = "";

             for (int k = 1; k < ds_dimValues.Tables[0].Columns.Count; k++)
             {
                 var num1 = ds_dimValues.Tables[0].Rows[0][k];
                 var num2 = ds_dimValues.Tables[0].Rows[1][k];
                 if (num1 is DBNull)
                     dimValuesYear += "0\t";
                 else
                     dimValuesYear += num1.ToString() + "\t";
                 if (num2 is DBNull)
                     dimValuesNextYear += "0\t";
                 else
                     dimValuesNextYear += num2.ToString() + "\t";
             }
             dimValuesYear = dimValuesYear.Substring(0, dimValuesYear.Length - 1);
             dimValuesNextYear = dimValuesNextYear.Substring(0, dimValuesNextYear.Length - 1);

             objChart.SeriesCollection.Add(0);
             objChart.SeriesCollection.Add(1);
             objChart.HasLegend = true;
             objChart.HasTitle = true;
             objChart.Title.Font.Size = 12;
             objChart.Title.Font.Bold = true;
             objChart.Title.Caption = "All Region";
             objChart.SeriesCollection[0].Caption = meeting.getyear().Substring(2, 2).Trim() + " A";
             objChart.SeriesCollection[1].Caption = meeting.getnextyear().Substring(2, 2).Trim() + " F";

             objChart.SeriesCollection[0].SetData(ChartDimensionsEnum.chDimCategories,
                    +(int)ChartSpecialDataSourcesEnum.chDataLiteral, dimRegionCategories);
             objChart.SeriesCollection[0].SetData(ChartDimensionsEnum.chDimValues,
                     +(int)ChartSpecialDataSourcesEnum.chDataLiteral, dimValuesYear);

             objChart.SeriesCollection[1].SetData(ChartDimensionsEnum.chDimCategories,
                    +(int)ChartSpecialDataSourcesEnum.chDataLiteral, dimRegionCategories);
             objChart.SeriesCollection[1].SetData(ChartDimensionsEnum.chDimValues,
                     +(int)ChartSpecialDataSourcesEnum.chDataLiteral, dimValuesNextYear);
         }
         objSpace.ExportPicture(picturePath, "GIF", 500, 400);
         picturePath = pictureFolderPath + pictureName;
         InsertPicture(slide, picturePath, 80, 120, 500, 400);


     }
     protected DataSet getSlide_Nine()
     {
         string query_SlideNineYear = "SELECT '" + meeting.getyear().Substring(2, 2).Trim() + " A' AS ' '";
         string query_SlideNineNextYear = "SELECT '" + meeting.getnextyear().Substring(2, 2).Trim() + "' AS ' '";
         string str = "";
         for (int i = 0; i < regionNumber; i++)
         {
             str += ",ROUND(SUM(CASE WHEN [Region_Cluster].RegionID='" + alRegionID[i].ToString().Trim() + "'"
                                   + " THEN ([Bookings].Amount*(CASE WHEN ([Bookings].DeliverY='YTD'"
                                   + " OR [Bookings].DeliverY='" + meeting.getyear().Substring(2, 2).Trim() + "')"
                                   + " THEN [Currency_Exchange].Rate1 ELSE [Currency_Exchange].Rate2 END))"
                                   + " ELSE 0 END)/1000,0) AS '" + alRegion[i].ToString().Trim() + "'";
         }
         query_SlideNineYear += str + " FROM [Bookings],[Region_Cluster],[Cluster_Country],[SalesOrg],[SalesOrg_User],[Currency_Exchange]"
                                   + " WHERE [Bookings].BookingY='" + meeting.getyear().Substring(2, 2).Trim() + "'"
                                   + " AND YEAR([Bookings].TimeFlag)='" + meeting.getyear().Trim() + "'"
                                   + " AND MONTH([Bookings].TimeFlag)='" + month.Trim() + "'"
                                   + " AND [Region_Cluster].ClusterID=[Cluster_Country].ClusterID "
                                   + " AND [Cluster_Country].CountryID=[Bookings].CountryID"
                                   + " AND [SalesOrg_User].UserID=[Bookings].RSMID"
                                   + " AND [SalesOrg_User].SalesOrgID=[SalesOrg].ID"
                                   + " AND [SalesOrg].CurrencyID=[Currency_Exchange].CurrencyID"
                                   + " AND Region_Cluster.Deleted=0 "
                                   + " AND Cluster_Country.Deleted=0 "
                                   + " AND SalesOrg.Deleted=0 "
                                   + " AND SalesOrg_User.Deleted=0 "
                                   + " AND Currency_Exchange.Deleted=0 "
                                   + " GROUP BY [Bookings].BookingY";
         query_SlideNineNextYear += str + " FROM [Bookings],[Region_Cluster],[Cluster_Country],[SalesOrg],[SalesOrg_User],[Currency_Exchange]"
                                   + " WHERE [Bookings].BookingY='" + meeting.getyear().Substring(2, 2).Trim() + "'"
                                   + " AND ([Bookings].DeliverY='" + meeting.getnextyear().Substring(2, 2).Trim() + "'"
                                   + " OR [Bookings].DeliverY='" + yearAfterNext.Substring(2, 2).Trim() + "')"
                                   + " AND YEAR([Bookings].TimeFlag)='" + meeting.getyear().Trim() + "'"
                                   + " AND MONTH([Bookings].TimeFlag)='" + month.Trim() + "'"
                                   + " AND [Region_Cluster].ClusterID=[Cluster_Country].ClusterID "
                                   + " AND [Cluster_Country].CountryID=[Bookings].CountryID"
                                   + " AND [SalesOrg_User].UserID=[Bookings].RSMID"
                                   + " AND [SalesOrg_User].SalesOrgID=[SalesOrg].ID"
                                   + " AND [SalesOrg].CurrencyID=[Currency_Exchange].CurrencyID"
                                   + " AND Region_Cluster.Deleted=0 "
                                   + " AND Cluster_Country.Deleted=0 "
                                   + " AND SalesOrg.Deleted=0 "
                                   + " AND SalesOrg_User.Deleted=0 "
                                   + " AND Currency_Exchange.Deleted=0 "
                                   + " GROUP BY [Bookings].BookingY";
         query_SlideNineYear += " UNION " + query_SlideNineNextYear;

         DataSet ds = helper.GetDataSet(query_SlideNineYear);
         return ds;
     }
     //----------------------------------------------------------------------------
     protected void DrawSlide_Ten()
     {
         slides.Add(++slideIndex, Microsoft.Office.Interop.PowerPoint.PpSlideLayout.ppLayoutTitleOnly);
         PowerPoint.Slide slide = slides[slideIndex];
         UniformLayout(slide, "BOOKINGS BYREGIONS");
         DataSet ds = getSlide_Nine();
         if (ds.Tables[0].Rows.Count == 0)
             return;
         string[] titile = { meeting.getyear().Substring(2, 2) + " A", meeting.getnextyear().Substring(2, 2) + " F" };
         float[] x = { 60, 380 };
         for (int i = 0; i < 2; i++)
         {
             ChartSpace objSpace = new ChartSpace();
             ChChart objChart = objSpace.Charts.Add(0);
             objChart.Type = ChartChartTypeEnum.chChartTypePie3D;
             objChart.HasTitle = true;
             objChart.Title.Font.Size = 12;
             objChart.Title.Font.Bold = true;
             objChart.Title.Caption = "All Products " + titile[i];
             objChart.HasLegend = true;
             string dimValues = "";
             double total = 0;
             for (int j = 0; j < regionNumber; j++)
             {
                 var num = ds.Tables[0].Rows[0][j + 1];
                 if (num is DBNull)
                     total += 0;
                 else
                     total += (double)num;
             }
             for (int j = 0; j < regionNumber; j++)
             {
                 var num = ds.Tables[0].Rows[0][j + 1];
                 if (num is DBNull)
                     dimValues += "0\t";
                 else
                     dimValues += string.Format("{0:F}", 100 * (double)num / total) + "\t";
             }
             dimValues = dimValues.Substring(0, dimValues.Length - 1);
             objChart.SeriesCollection.Add(0);
             objChart.Legend.Position = ChartLegendPositionEnum.chLegendPositionLeft;
             objChart.SeriesCollection[0].DataLabelsCollection.Add();
             objChart.SeriesCollection[0].DataLabelsCollection[0].HasValue = false;
             objChart.SeriesCollection[0].DataLabelsCollection[0].HasPercentage = true;
             objSpace.Charts[0].SeriesCollection[0].SetData(ChartDimensionsEnum.chDimCategories,
                    +(int)ChartSpecialDataSourcesEnum.chDataLiteral, dimRegionCategories);
             objSpace.Charts[0].SeriesCollection[0].SetData(ChartDimensionsEnum.chDimValues,
                     +(int)ChartSpecialDataSourcesEnum.chDataLiteral, dimValues);
             objSpace.ExportPicture(picturePath, "GIF", 300, 250);
             picturePath = pictureFolderPath + pictureName;
             InsertPicture(slide, picturePath, x[i], 120, 300, 250);
         }
     }

     //--------------------------------------------------------------------------
     protected void DrawSlide_eleven()
     {
         PowerPoint.Slide slide = slides[++slideIndex];
         UniformLayout(slide, "ET HP " + meeting.getyear() + " Actual," + meeting.getnextyear() + "Forecast");
         DataSet ds = getSlide_eleven();
         if (ds.Tables[0].Rows.Count == 0)
             return;
         float x = 0;
         float y = 0;
         string dimValues = "";
         for (int i = 0; i < 3; i++)
         {
             switch (i)
             {
                 case 0:
                     x = 55;
                     y = 80;
                     break;
                 case 1:
                     x = 485;
                     y = 80;
                     break;
                 case 2:
                     x = 55;
                     y = 300;
                     break;
             }
             ChartSpace objSpace = new ChartSpace();
             ChChart objChart = objSpace.Charts.Add(0);
             objChart.Type = ChartChartTypeEnum.chChartTypePie3D;
             objChart.HasTitle = true;
             objChart.Title.Font.Size = 12;
             objChart.Title.Font.Bold = true;
             objChart.Title.Caption = "Booking " + ds.Tables[0].Columns[i + 1].ColumnName;
             objChart.HasLegend = true;
             objChart.SeriesCollection.Add(0);
             objChart.Legend.Position = ChartLegendPositionEnum.chLegendPositionLeft;

             for (int k = 0; k < ds.Tables[0].Rows.Count; k++)
             {
                 var num = ds.Tables[0].Rows[k][i + 1];
                 double num1 = (num is DBNull ? 0 : (double)num);
                 if (alSegment.Count < ds.Tables[0].Rows.Count - 1)
                     dimValues += String.Format("{0:F}", (100 * num1 / (double)ds.Tables[0].Rows[alSegment.Count][i + 1])) + "\t";
                 else
                     return;
             }
             dimValues = dimValues.Substring(0, dimValues.Length - 1);
             salesGap = dimValues;
             objChart.SeriesCollection[0].DataLabelsCollection.Add();
             objChart.SeriesCollection[0].DataLabelsCollection[0].HasValue = false;
             objChart.SeriesCollection[0].DataLabelsCollection[0].HasPercentage = true;
             objSpace.Charts[0].SeriesCollection[0].SetData(ChartDimensionsEnum.chDimCategories,
                +(int)ChartSpecialDataSourcesEnum.chDataLiteral, dimSegmentCategories);
             objSpace.Charts[0].SeriesCollection[0].SetData(ChartDimensionsEnum.chDimValues,
                    +(int)ChartSpecialDataSourcesEnum.chDataLiteral, dimValues);
             dimValues = "";
             objSpace.ExportPicture(picturePath, "GIF", 180, 180);
             picturePath = pictureFolderPath + pictureName;
             InsertPicture(slide, picturePath, x, y, 180, 180);
         }

         ChartSpace objSpace1 = new ChartSpace();
         ChChart objChart1 = objSpace1.Charts.Add(0);
         objChart1.Type = ChartChartTypeEnum.chChartTypeColumnStacked;
         objChart1.HasTitle = true;
         objChart1.HasLegend = true;
         objChart1.Title.Caption = "Bookings " + meeting.getnextyear().Substring(2, 2).Trim() + " Gap";
         for (int k = 0; k < segmentNumber; k++)
         {
             string dimValues1 = "";
             for (int l = 0; l < segmentNumber; l++)
             {
                 if (l == k)
                 {
                     var num = ds.Tables[0].Rows[k][4];
                     dimValues1 += (num is DBNull ? 0 : (double)num).ToString() + "\t";
                 }
                 else
                     dimValues1 += "0\t";
             }
             dimValues1 = dimValues1.Substring(0, dimValues1.Length - 1);
             objChart1.SeriesCollection.Add(k);
             objChart1.SeriesCollection[k].Caption = alSegment[k].ToString();
             objSpace1.Charts[0].SeriesCollection[k].SetData(ChartDimensionsEnum.chDimCategories,
                    +(int)ChartSpecialDataSourcesEnum.chDataLiteral, dimSegmentCategories);
             objSpace1.Charts[0].SeriesCollection[k].SetData(ChartDimensionsEnum.chDimValues,
                    +(int)ChartSpecialDataSourcesEnum.chDataLiteral, dimValues1);
         }
         objSpace1.ExportPicture(picturePath, "GIF", 180, 180);
         picturePath = pictureFolderPath + pictureName;
         InsertPicture(slide, picturePath, 485, 300, 180, 180);
         int rowCount = ds.Tables[0].Rows.Count;
         int columnCount = ds.Tables[0].Columns.Count;

         PowerPoint.Shape shape = slide.Shapes.AddTable(rowCount + 1, columnCount, 385, 300, 700, 200);

         shape.Fill.BackColor.SchemeColor = PowerPoint.PpColorSchemeIndex.ppBackground;
         for (int s = 0; s < rowCount + 1; s++)
         {
             for (int t = 0; t < columnCount; t++)
             {
                 PowerPoint.TextFrame textF = shape.Table.Cell(s + 1, t + 1).Shape.TextFrame;
                 PowerPoint.TextRange textR = textF.TextRange;
                 textR.Font.Bold = MsoTriState.msoFalse;
                 textR.Font.Size = 4;

                 if (s == 0)
                 {
                     textR.Text = ds.Tables[0].Columns[t].ColumnName;
                 }
                 else
                 {
                     var num = ds.Tables[0].Rows[s - 1][t];

                     textR.Text = (num is DBNull ? 0 : num).ToString();
                 }
             }
         }
         shape.Export(picturePath, Microsoft.Office.Interop.PowerPoint.PpShapeFormat.ppShapeFormatGIF, 400, 200, Microsoft.Office.Interop.PowerPoint.PpExportMode.ppScaleXY);
         shape.Delete();
         InsertPicture(slide, picturePath, 240, 230, 240, 120);
     }

     protected DataSet getSlide_eleven()
     {
         string query = "";
         for (int i = 0; i < alSegment.Count; i++)
         {
             string str = " SELECT '" + alSegment[i].ToString() + "' AS ' '"
                         + " ,ROUND(SUM(CASE WHEN [Bookings].BookingY='" + meeting.getyear().Substring(2, 2).Trim() + "'"
                         + " AND YEAR([Bookings].TimeFlag)='" + meeting.getyear().Trim() + "'"
                         + " AND MONTH([Bookings].TimeFlag)='" + month.Trim() + "'"
                         + " THEN Amount*(CASE WHEN [Bookings].DeliverY='YTD'"
                         + " OR [Bookings].DeliverY='" + meeting.getyear().Substring(2, 2).Trim() + "'"
                         + " THEN [Currency_Exchange].Rate1 ELSE [Currency_Exchange].Rate2 END)"
                         + " ELSE 0 END)/1000,0) AS '" + meeting.getyear().Trim() + " A'"
                         + " ,ROUND(SUM(CASE WHEN [Bookings].BookingY='" + meeting.getnextyear().Substring(2, 2).Trim() + "'"
                         + " AND ([Bookings].DeliverY='" + meeting.getnextyear().Substring(2, 2).Trim() + "'"
                         + " OR [Bookings].DeliverY='" + yearAfterNext.Substring(2, 2).Trim() + "')"
                         + " AND YEAR([Bookings].TimeFlag)='" + meeting.getyear().Trim() + "'"
                         + " AND MONTH([Bookings].TimeFlag)='03'"
                         + " THEN Amount*([Currency_Exchange].Rate2) ELSE 0 END)/1000,0) AS '" + meeting.getnextyear().Trim() + " B'"
                         + " ,ROUND(SUM(CASE WHEN [Bookings].BookingY='" + meeting.getnextyear().Substring(2, 2).Trim() + "'"
                         + " AND ([Bookings].DeliverY='" + meeting.getnextyear().Substring(2, 2).Trim() + "'"
                         + " OR [Bookings].DeliverY='" + yearAfterNext.Substring(2, 2).Trim() + "')"
                         + " AND YEAR([Bookings].TimeFlag)='" + meeting.getyear().Trim() + "'"
                         + " AND MONTH([Bookings].TimeFlag)='" + month.Trim() + "'"
                         + " THEN Amount*([Currency_Exchange].Rate2) ELSE 0 END)/1000,0) AS '" + meeting.getnextyear().Trim() + " F'"
                         + " ,ROUND((SUM(CASE WHEN [Bookings].BookingY='" + meeting.getnextyear().Substring(2, 2).Trim() + "'"
                         + " AND ([Bookings].DeliverY='" + meeting.getnextyear().Substring(2, 2).Trim() + "'"
                         + " OR [Bookings].DeliverY='" + yearAfterNext.Substring(2, 2).Trim() + "')"
                         + " AND YEAR([Bookings].TimeFlag)='" + meeting.getyear().Trim() + "'"
                         + " AND MONTH([Bookings].TimeFlag)='03'"
                         + " THEN Amount*([Currency_Exchange].Rate2) ELSE 0 END)-"
                         + " SUM(CASE WHEN [Bookings].BookingY='" + meeting.getnextyear().Substring(2, 2).Trim() + "'"
                         + " AND ([Bookings].DeliverY='" + meeting.getnextyear().Substring(2, 2).Trim() + "'"
                         + " OR [Bookings].DeliverY='" + yearAfterNext.Substring(2, 2).Trim() + "')"
                         + " AND YEAR([Bookings].TimeFlag)='" + meeting.getyear().Trim() + "'"
                         + " AND MONTH([Bookings].TimeFlag)='" + month.Trim() + "'"
                         + " THEN Amount*([Currency_Exchange].Rate2) ELSE 0 END))/1000,0) AS '" + meeting.getnextyear().Trim() + " GAP'"
                         + " FROM [Bookings],[Segment],[SalesOrg],[SalesOrg_User],[Currency_Exchange]"
                         + " WHERE [Bookings].SegmentID=[Segment].ID"
                         + " AND Segment.Deleted=0 "
                         + " AND SalesOrg.Deleted=0 "
                         + " AND SalesOrg_User.Deleted=0 "
                         + " AND Currency_Exchange.Deleted=0 "
                         + " AND [Segment].Abbr='" + alSegment[i].ToString() + "'"
                         + " AND [Bookings].RSMID=[SalesOrg_User].UserID"
                         + " AND [SalesOrg_User].SalesOrgID=[SalesOrg].ID"
                         + " AND [SalesOrg].CurrencyID=[Currency_Exchange].CurrencyID"
                         + " AND [Currency_Exchange].TimeFlag=[Bookings].TimeFlag UNION ";
             query += str;
         }
         query += " SELECT 'TOTAL ET HP' AS ' '"
                         + " ,ROUND(SUM(CASE WHEN [Bookings].BookingY='" + meeting.getyear().Substring(2, 2).Trim() + "'"
                         + " AND YEAR([Bookings].TimeFlag)='" + meeting.getyear().Trim() + "'"
                         + " AND MONTH([Bookings].TimeFlag)='" + month.Trim() + "'"
                         + " THEN Amount*(CASE WHEN [Bookings].DeliverY='YTD'"
                         + " OR [Bookings].DeliverY='" + meeting.getyear().Substring(2, 2).Trim() + "'"
                         + " THEN [Currency_Exchange].Rate1 ELSE [Currency_Exchange].Rate2 END)"
                         + " ELSE 0 END)/1000,0) AS '" + meeting.getyear().Trim() + " A'"
                         + " ,ROUND(SUM(CASE WHEN [Bookings].BookingY='" + meeting.getnextyear().Substring(2, 2).Trim() + "'"
                         + " AND ([Bookings].DeliverY='" + meeting.getnextyear().Substring(2, 2).Trim() + "'"
                         + " OR [Bookings].DeliverY='" + yearAfterNext.Substring(2, 2).Trim() + "')"
                         + " AND YEAR([Bookings].TimeFlag)='" + meeting.getyear().Trim() + "'"
                         + " AND MONTH([Bookings].TimeFlag)='03'"
                         + " THEN Amount*([Currency_Exchange].Rate2) ELSE 0 END)/1000,0) AS '" + meeting.getnextyear().Trim() + " B'"
                         + " ,ROUND(SUM(CASE WHEN [Bookings].BookingY='" + meeting.getnextyear().Substring(2, 2).Trim() + "'"
                         + " AND ([Bookings].DeliverY='" + meeting.getnextyear().Substring(2, 2).Trim() + "'"
                         + " OR [Bookings].DeliverY='" + yearAfterNext.Substring(2, 2).Trim() + "')"
                         + " AND YEAR([Bookings].TimeFlag)='" + meeting.getyear().Trim() + "'"
                         + " AND MONTH([Bookings].TimeFlag)='" + month.Trim() + "'"
                         + " THEN Amount*([Currency_Exchange].Rate2) ELSE 0 END)/1000,0) AS '" + meeting.getnextyear().Trim() + " F'"
                         + " ,ROUND((SUM(CASE WHEN [Bookings].BookingY='" + meeting.getnextyear().Substring(2, 2).Trim() + "'"
                         + " AND ([Bookings].DeliverY='" + meeting.getnextyear().Substring(2, 2).Trim() + "'"
                         + " OR [Bookings].DeliverY='" + yearAfterNext.Substring(2, 2).Trim() + "')"
                         + " AND YEAR([Bookings].TimeFlag)='" + meeting.getyear().Trim() + "'"
                         + " AND MONTH([Bookings].TimeFlag)='03'"
                         + " THEN Amount*([Currency_Exchange].Rate2) ELSE 0 END)-"
                         + " SUM(CASE WHEN [Bookings].BookingY='" + meeting.getnextyear().Substring(2, 2).Trim() + "'"
                         + " AND ([Bookings].DeliverY='" + meeting.getnextyear().Substring(2, 2).Trim() + "'"
                         + " OR [Bookings].DeliverY='" + yearAfterNext.Substring(2, 2).Trim() + "')"
                         + " AND YEAR([Bookings].TimeFlag)='" + meeting.getyear().Trim() + "'"
                         + " AND MONTH([Bookings].TimeFlag)='" + month.Trim() + "'"
                         + " THEN Amount*([Currency_Exchange].Rate2) ELSE 0 END))/1000,0) AS '" + meeting.getnextyear().Trim() + " GAP'"
                         + " FROM [Bookings],[Segment],[SalesOrg],[SalesOrg_User],[Currency_Exchange]"
                         + " WHERE [Bookings].SegmentID=[Segment].ID"
                         + " AND Segment.Deleted=0 "
                         + " AND SalesOrg.Deleted=0 "
                         + " AND SalesOrg_User.Deleted=0 "
                         + " AND Currency_Exchange.Deleted=0 "
                         + " AND [Bookings].RSMID=[SalesOrg_User].UserID"
                         + " AND [SalesOrg_User].SalesOrgID=[SalesOrg].ID"
                         + " AND [SalesOrg].CurrencyID=[Currency_Exchange].CurrencyID"
                         + " AND [Currency_Exchange].TimeFlag=[Bookings].TimeFlag";
         DataSet ds = helper.GetDataSet(query);
         return ds;
     }
     //--------------------------------------------------------------------------
     protected void DrawSlide_Telwe()
     {
         slides.Add(++slideIndex, Microsoft.Office.Interop.PowerPoint.PpSlideLayout.ppLayoutTitleOnly);
         PowerPoint.Slide slide = slides[slideIndex];
         UniformLayout(slide, "ET HP " + meeting.getyear() + " Actual," + meeting.getnextyear() + "Forecast");
         DataSet ds = getSlide_Telwe();
         if (ds.Tables[0].Rows.Count == 0)
             return;
         DataSet dsAdd = getSlide_TelweAdd();
         if (ds.Tables[0].Rows.Count == 0)
             return;
         float x = 0;
         float y = 0;

         for (int i = 0; i < 3; i++)
         {
             string dimValues = "";
             switch (i)
             {
                 case 0:
                     x = 55;
                     y = 80;
                     break;
                 case 1:
                     x = 485;
                     y = 80;
                     break;
                 case 2:
                     x = 55;
                     y = 300;
                     break;
             }
             ChartSpace objSpace = new ChartSpace();
             ChChart objChart = objSpace.Charts.Add(0);
             objChart.Type = ChartChartTypeEnum.chChartTypePie3D;
             objChart.HasTitle = true;
             objChart.Title.Font.Size = 12;
             objChart.Title.Font.Bold = true;
             objChart.Title.Caption = "Sales " + ds.Tables[0].Columns[i + 1].ColumnName;
             objChart.HasLegend = true;
             objChart.SeriesCollection.Add(0);
             objChart.Legend.Position = ChartLegendPositionEnum.chLegendPositionLeft;
             objChart.SeriesCollection[0].DataLabelsCollection.Add();

             for (int k = 0; k < segmentNumber; k++)
             {
                 var num1 = ds.Tables[0].Rows[k][i + 1];
                 if (dsAdd != null)
                 {
                     var num2 = dsAdd.Tables[0].Rows[k][i + 1];
                     double total1 = Convert.ToDouble(num1 is DBNull ? 0 : num1) + Convert.ToDouble(num2 is DBNull ? 0 : num2);
                     double total2;
                     if (ds.Tables[0].Rows[segmentNumber][i + 1].ToString() == null || ds.Tables[0].Rows[segmentNumber][i + 1].ToString() == "" || dsAdd.Tables[0].Rows[segmentNumber][i + 1].ToString() == "" || dsAdd.Tables[0].Rows[segmentNumber][i + 1].ToString() == null)
                        total2 = 1.0;
                     else
                         total2 = Convert.ToDouble(ds.Tables[0].Rows[segmentNumber][i + 1].ToString()) + Convert.ToDouble(dsAdd.Tables[0].Rows[segmentNumber][i + 1].ToString());
                     dimValues += String.Format("{0:F}", (100 * total1 / total2)) + "\t";
                 }
                 else
                 {
                     double total1 = Convert.ToDouble(num1 is DBNull ? 0 : num1);
                     double total2 = Convert.ToDouble(ds.Tables[0].Rows[segmentNumber][i + 1]);
                     dimValues += String.Format("{0:F}", (100 * total1 / total2)) + "\t";
                 }
             }
             dimValues = dimValues.Substring(0, dimValues.Length - 1);
             objChart.SeriesCollection[0].DataLabelsCollection[0].HasValue = false;
             objChart.SeriesCollection[0].DataLabelsCollection[0].HasPercentage = true;
             objSpace.Charts[0].SeriesCollection[0].SetData(ChartDimensionsEnum.chDimCategories,
                +(int)ChartSpecialDataSourcesEnum.chDataLiteral, dimSegmentCategories);
             objSpace.Charts[0].SeriesCollection[0].SetData(ChartDimensionsEnum.chDimValues,
                    +(int)ChartSpecialDataSourcesEnum.chDataLiteral, dimValues);


             objSpace.ExportPicture(picturePath, "GIF", 180, 180);
             picturePath = pictureFolderPath + pictureName;
             InsertPicture(slide, picturePath, x, y, 180, 180);
         }

         ChartSpace objSpace1 = new ChartSpace();
         ChChart objChart1 = objSpace1.Charts.Add(0);
         objChart1.Type = ChartChartTypeEnum.chChartTypeColumnStacked;
         objChart1.HasTitle = true;
         objChart1.HasLegend = true;
         objChart1.Title.Caption = "Sales " + meeting.getnextyear().Substring(2, 2).Trim() + " Gap";
         for (int k = 0; k < segmentNumber; k++)
         {
             string dimValues1 = "";
             for (int l = 0; l < segmentNumber; l++)
             {
                 if (l == k)
                 {
                     var num1 = ds.Tables[0].Rows[k][4];
                     var num2 = dsAdd.Tables[0].Rows[k][4];
                     dimValues1 += ((num1 is DBNull ? 0 : (double)num1) + (num2 is DBNull ? 0 : (double)num2)).ToString() + "\t";
                 }
                 else
                     dimValues1 += "0\t";
             }
             dimValues1 = dimValues1.Substring(0, dimValues1.Length - 1);
             objChart1.SeriesCollection.Add(k);
             objChart1.SeriesCollection[k].Caption = alSegment[k].ToString();
             objSpace1.Charts[0].SeriesCollection[k].SetData(ChartDimensionsEnum.chDimCategories,
                    +(int)ChartSpecialDataSourcesEnum.chDataLiteral, dimSegmentCategories);
             objSpace1.Charts[0].SeriesCollection[k].SetData(ChartDimensionsEnum.chDimValues,
                    +(int)ChartSpecialDataSourcesEnum.chDataLiteral, dimValues1);
         }
         objSpace1.ExportPicture(picturePath, "GIF", 180, 180);
         picturePath = pictureFolderPath + pictureName;
         InsertPicture(slide, picturePath, 485, 300, 180, 180);
         int rowCount = ds.Tables[0].Rows.Count;
         int columnCount = ds.Tables[0].Columns.Count;
         PowerPoint.Shape shape = slide.Shapes.AddTable(rowCount + 1, columnCount, 385, 300, 700, 200);

         for (int s = 0; s < rowCount + 1; s++)
         {
             for (int t = 0; t < columnCount; t++)
             {
                 PowerPoint.TextFrame textF = shape.Table.Cell(s + 1, t + 1).Shape.TextFrame;
                 PowerPoint.TextRange textR = textF.TextRange;
                 textR.Font.Bold = MsoTriState.msoFalse;
                 textR.Font.Size = 4;

                 if (s == 0)
                 {
                     textR.Text = ds.Tables[0].Columns[t].ColumnName;
                 }
                 else
                 {
                     if (t == 0)
                     {
                         var num1 = ds.Tables[0].Rows[s - 1][t];
                         textR.Text = num1.ToString();
                     }
                     else
                     {
                         var num1 = ds.Tables[0].Rows[s - 1][t];
                         var num2 = dsAdd.Tables[0].Rows[s - 1][t];
                         textR.Text = (Convert.ToDouble(num1 is DBNull ? 0 : num1) + Convert.ToDouble(num2 is DBNull ? 0 : num2)).ToString();
                     }
                 }
             }
         }

         shape.Export(picturePath, Microsoft.Office.Interop.PowerPoint.PpShapeFormat.ppShapeFormatGIF, 400, 200, Microsoft.Office.Interop.PowerPoint.PpExportMode.ppScaleXY);
         shape.Delete();
         InsertPicture(slide, picturePath, 240, 230, 240, 120);
     }
     protected DataSet getSlide_Telwe()
     {
         string query = "";
         for (int i = 0; i < alSegment.Count; i++)
         {
             string str = " SELECT '" + alSegment[i].ToString() + "' AS ' '"
                         + " ,ROUND(0/1000,0) AS '" + meeting.getyear().Trim() + " A'"
                         + " ,ROUND(SUM(CASE WHEN [Bookings].BookingY='" + meeting.getnextyear().Substring(2, 2).Trim() + "'"
                         + " AND ([Bookings].DeliverY='" + meeting.getnextyear().Substring(2, 2).Trim() + "'"
                         + " OR [Bookings].DeliverY='" + yearAfterNext.Substring(2, 2).Trim() + "')"
                         + " AND YEAR([Bookings].TimeFlag)='" + meeting.getyear().Trim() + "'"
                         + " AND MONTH([Bookings].TimeFlag)='03'"
                         + " THEN Amount*([Currency_Exchange].Rate2) ELSE 0 END)/1000,0) AS '" + meeting.getnextyear().Trim() + " B'"
                         + " ,ROUND(SUM(CASE WHEN [Bookings].BookingY='" + meeting.getnextyear().Substring(2, 2).Trim() + "'"
                         + " AND ([Bookings].DeliverY='" + meeting.getnextyear().Substring(2, 2).Trim() + "'"
                         + " OR [Bookings].DeliverY='" + yearAfterNext.Substring(2, 2).Trim() + "')"
                         + " AND YEAR([Bookings].TimeFlag)='" + meeting.getyear().Trim() + "'"
                         + " AND MONTH([Bookings].TimeFlag)='" + month.Trim() + "'"
                         + " THEN Amount*([Currency_Exchange].Rate2) ELSE 0 END)/1000,0) AS '" + meeting.getnextyear().Trim() + " F'"
                         + " ,ROUND((SUM(CASE WHEN [Bookings].BookingY='" + meeting.getnextyear().Substring(2, 2).Trim() + "'"
                         + " AND ([Bookings].DeliverY='" + meeting.getnextyear().Substring(2, 2).Trim() + "'"
                         + " OR [Bookings].DeliverY='" + yearAfterNext.Substring(2, 2).Trim() + "')"
                         + " AND YEAR([Bookings].TimeFlag)='" + meeting.getyear().Trim() + "'"
                         + " AND MONTH([Bookings].TimeFlag)='03'"
                         + " THEN Amount*([Currency_Exchange].Rate2) ELSE 0 END)-"
                         + " SUM(CASE WHEN [Bookings].BookingY='" + meeting.getnextyear().Substring(2, 2).Trim() + "'"
                         + " AND ([Bookings].DeliverY='" + meeting.getnextyear().Substring(2, 2).Trim() + "'"
                         + " OR [Bookings].DeliverY='" + yearAfterNext.Substring(2, 2).Trim() + "')"
                         + " AND YEAR([Bookings].TimeFlag)='" + meeting.getyear().Trim() + "'"
                         + " AND MONTH([Bookings].TimeFlag)='" + month.Trim() + "'"
                         + " THEN Amount*([Currency_Exchange].Rate2) ELSE 0 END))/1000,0) AS '" + meeting.getnextyear().Trim() + " GAP'"
                         + " FROM [Bookings],[Segment],[SalesOrg],[SalesOrg_User],[Currency_Exchange]"
                         + " WHERE [Bookings].SegmentID=[Segment].ID"
                         + " AND Segment.Deleted=0 "
                         + " AND SalesOrg.Deleted=0 "
                         + " AND SalesOrg_User.Deleted=0 "
                         + " AND Currency_Exchange.Deleted=0 "
                         + " AND [Segment].Abbr='" + alSegment[i].ToString() + "'"
                         + " AND [Bookings].RSMID=[SalesOrg_User].UserID"
                         + " AND [SalesOrg_User].SalesOrgID=[SalesOrg].ID"
                         + " AND [SalesOrg].CurrencyID=[Currency_Exchange].CurrencyID"
                         + " AND [Currency_Exchange].TimeFlag=[Bookings].TimeFlag UNION ";
             query += str;
         }
         query += " SELECT 'TOTAL ET HP' AS ' '"
                         + " ,ROUND(0/1000,0) AS '" + meeting.getyear().Trim() + " A'"
                         + " ,ROUND(SUM(CASE WHEN [Bookings].BookingY='" + meeting.getnextyear().Substring(2, 2).Trim() + "'"
                         + " AND ([Bookings].DeliverY='" + meeting.getnextyear().Substring(2, 2).Trim() + "'"
                         + " OR [Bookings].DeliverY='" + yearAfterNext.Substring(2, 2).Trim() + "')"
                         + " AND YEAR([Bookings].TimeFlag)='" + meeting.getyear().Trim() + "'"
                         + " AND MONTH([Bookings].TimeFlag)='03'"
                         + " THEN Amount*([Currency_Exchange].Rate2) ELSE 0 END)/1000,0) AS '" + meeting.getnextyear().Trim() + " B'"
                         + " ,ROUND(SUM(CASE WHEN [Bookings].BookingY='" + meeting.getnextyear().Substring(2, 2).Trim() + "'"
                         + " AND ([Bookings].DeliverY='" + meeting.getnextyear().Substring(2, 2).Trim() + "'"
                         + " OR [Bookings].DeliverY='" + yearAfterNext.Substring(2, 2).Trim() + "')"
                         + " AND YEAR([Bookings].TimeFlag)='" + meeting.getyear().Trim() + "'"
                         + " AND MONTH([Bookings].TimeFlag)='" + month.Trim() + "'"
                         + " THEN Amount*([Currency_Exchange].Rate2) ELSE 0 END)/1000,0) AS '" + meeting.getnextyear().Trim() + " F'"
                         + " ,ROUND((SUM(CASE WHEN [Bookings].BookingY='" + meeting.getnextyear().Substring(2, 2).Trim() + "'"
                         + " AND ([Bookings].DeliverY='" + meeting.getnextyear().Substring(2, 2).Trim() + "'"
                         + " OR [Bookings].DeliverY='" + yearAfterNext.Substring(2, 2).Trim() + "')"
                         + " AND YEAR([Bookings].TimeFlag)='" + meeting.getyear().Trim() + "'"
                         + " AND MONTH([Bookings].TimeFlag)='03'"
                         + " THEN Amount*([Currency_Exchange].Rate2) ELSE 0 END)-"
                         + " SUM(CASE WHEN [Bookings].BookingY='" + meeting.getnextyear().Substring(2, 2).Trim() + "'"
                         + " AND ([Bookings].DeliverY='" + meeting.getnextyear().Substring(2, 2).Trim() + "'"
                         + " OR [Bookings].DeliverY='" + yearAfterNext.Substring(2, 2).Trim() + "')"
                         + " AND YEAR([Bookings].TimeFlag)='" + meeting.getyear().Trim() + "'"
                         + " AND MONTH([Bookings].TimeFlag)='" + month.Trim() + "'"
                         + " THEN Amount*([Currency_Exchange].Rate2) ELSE 0 END))/1000,0) AS '" + meeting.getnextyear().Trim() + " GAP'"
                         + " FROM [Bookings],[Segment],[SalesOrg],[SalesOrg_User],[Currency_Exchange]"
                         + " WHERE [Bookings].SegmentID=[Segment].ID"
                         + " AND Segment.Deleted=0 "
                         + " AND SalesOrg.Deleted=0 "
                         + " AND SalesOrg_User.Deleted=0 "
                         + " AND Currency_Exchange.Deleted=0 "
                         + " AND [Bookings].RSMID=[SalesOrg_User].UserID"
                         + " AND [SalesOrg_User].SalesOrgID=[SalesOrg].ID"
                         + " AND [SalesOrg].CurrencyID=[Currency_Exchange].CurrencyID"
                         + " AND [Currency_Exchange].TimeFlag=[Bookings].TimeFlag";
         DataSet ds = helper.GetDataSet(query);
         return ds;
     }
     protected DataSet getSlide_TelweAdd()
     {
         string query = "";
         for (int i = 0; i < alSegment.Count; i++)
         {
             string str = " SELECT '" + alSegment[i].ToString() + "' AS ' '"
                         + " ,ROUND(SUM(CASE WHEN [ActualSalesandBL].BacklogY='" + meeting.getyear().Substring(2, 2).Trim() + "'"
                         + " AND YEAR([ActualSalesandBL].TimeFlag)='" + meeting.getyear().Trim() + "'"
                         + " AND MONTH([ActualSalesandBL].TimeFlag)='" + month.Trim() + "'"
                         + " THEN Backlog*[Currency_Exchange].Rate1 ELSE 0 END)/1000,0) AS '" + meeting.getyear().Trim() + " A'"
                         + " ,ROUND(SUM(CASE WHEN [ActualSalesandBL].BacklogY='" + meeting.getnextyear().Substring(2, 2).Trim() + "'"
                         + " AND YEAR([ActualSalesandBL].TimeFlag)='" + meeting.getyear().Trim() + "'"
                         + " AND MONTH([ActualSalesandBL].TimeFlag)='03'"
                         + " THEN Backlog*([Currency_Exchange].Rate2) ELSE 0 END)/1000,0) AS '" + meeting.getnextyear().Trim() + " B'"
                         + " ,ROUND(SUM(CASE WHEN [ActualSalesandBL].BacklogY='" + meeting.getnextyear().Substring(2, 2).Trim() + "'"
                         + " AND YEAR([ActualSalesandBL].TimeFlag)='" + meeting.getyear().Trim() + "'"
                         + " AND MONTH([ActualSalesandBL].TimeFlag)='" + month.Trim() + "'"
                         + " THEN Backlog*([Currency_Exchange].Rate2) ELSE 0 END)/1000,0) AS '" + meeting.getnextyear().Trim() + " F'"
                         + " ,ROUND((SUM(CASE WHEN [ActualSalesandBL].BacklogY='" + meeting.getnextyear().Substring(2, 2).Trim() + "'"
                         + " AND YEAR([ActualSalesandBL].TimeFlag)='" + meeting.getyear().Trim() + "'"
                         + " AND MONTH([ActualSalesandBL].TimeFlag)='03'"
                         + " THEN Backlog*([Currency_Exchange].Rate2) ELSE 0 END)-"
                         + " SUM(CASE WHEN [ActualSalesandBL].BacklogY='" + meeting.getnextyear().Substring(2, 2).Trim() + "'"
                         + " AND YEAR([ActualSalesandBL].TimeFlag)='" + meeting.getyear().Trim() + "'"
                         + " AND MONTH([ActualSalesandBL].TimeFlag)='" + month.Trim() + "'"
                         + " THEN Backlog*([Currency_Exchange].Rate2) ELSE 0 END))/1000,0) AS '" + meeting.getnextyear().Trim() + " GAP'"
                         + " FROM [ActualSalesandBL],[Segment],[SalesOrg],[Currency_Exchange]"
                         + " WHERE [ActualSalesandBL].SegmentID=[Segment].ID"
                         + " AND Segment.Deleted=0 "
                         + " AND SalesOrg.Deleted=0 "
                         + " AND Currency_Exchange.Deleted=0 "
                         + " AND [Segment].Abbr='" + alSegment[i].ToString() + "'"
                         + " AND [ActualSalesandBL].SalesOrgID=[SalesOrg].ID"
                         + " AND [SalesOrg].CurrencyID=[Currency_Exchange].CurrencyID"
                         + " AND [Currency_Exchange].TimeFlag=[ActualSalesandBL].TimeFlag UNION ";
             query += str;
         }
         query += " SELECT 'TOTAL ET HP' AS ' '"
                         + " ,ROUND(SUM(CASE WHEN [ActualSalesandBL].BacklogY='" + meeting.getyear().Substring(2, 2).Trim() + "'"
                         + " AND YEAR([ActualSalesandBL].TimeFlag)='" + meeting.getyear().Trim() + "'"
                         + " AND MONTH([ActualSalesandBL].TimeFlag)='" + month.Trim() + "'"
                         + " THEN Backlog*[Currency_Exchange].Rate1 ELSE 0 END)/1000,0) AS '" + meeting.getyear().Trim() + " A'"
                         + " ,ROUND(SUM(CASE WHEN [ActualSalesandBL].BacklogY='" + meeting.getnextyear().Substring(2, 2).Trim() + "'"
                         + " AND YEAR([ActualSalesandBL].TimeFlag)='" + meeting.getyear().Trim() + "'"
                         + " AND MONTH([ActualSalesandBL].TimeFlag)='03'"
                         + " THEN Backlog*([Currency_Exchange].Rate2) ELSE 0 END)/1000,0) AS '" + meeting.getnextyear().Trim() + " B'"
                         + " ,ROUND(SUM(CASE WHEN [ActualSalesandBL].BacklogY='" + meeting.getnextyear().Substring(2, 2).Trim() + "'"
                         + " AND YEAR([ActualSalesandBL].TimeFlag)='" + meeting.getyear().Trim() + "'"
                         + " AND MONTH([ActualSalesandBL].TimeFlag)='" + month.Trim() + "'"
                         + " THEN Backlog*([Currency_Exchange].Rate2) ELSE 0 END)/1000,0) AS '" + meeting.getnextyear().Trim() + " F'"
                         + " ,ROUND((SUM(CASE WHEN [ActualSalesandBL].BacklogY='" + meeting.getnextyear().Substring(2, 2).Trim() + "'"
                         + " AND YEAR([ActualSalesandBL].TimeFlag)='" + meeting.getyear().Trim() + "'"
                         + " AND MONTH([ActualSalesandBL].TimeFlag)='03'"
                         + " THEN Backlog*([Currency_Exchange].Rate2) ELSE 0 END)-"
                         + " SUM(CASE WHEN [ActualSalesandBL].BacklogY='" + meeting.getnextyear().Substring(2, 2).Trim() + "'"
                         + " AND YEAR([ActualSalesandBL].TimeFlag)='" + meeting.getyear().Trim() + "'"
                         + " AND MONTH([ActualSalesandBL].TimeFlag)='" + month.Trim() + "'"
                         + " THEN Backlog*([Currency_Exchange].Rate2) ELSE 0 END))/1000,0) AS '" + meeting.getnextyear().Trim() + " GAP'"
                         + " FROM [ActualSalesandBL],[Segment],[SalesOrg],[Currency_Exchange]"
                         + " WHERE [ActualSalesandBL].SegmentID=[Segment].ID"
                         + " AND Segment.Deleted=0 "
                         + " AND SalesOrg.Deleted=0 "
                         + " AND Currency_Exchange.Deleted=0 "
                         + " AND [ActualSalesandBL].SalesOrgID=[SalesOrg].ID"
                         + " AND [SalesOrg].CurrencyID=[Currency_Exchange].CurrencyID"
                         + " AND [Currency_Exchange].TimeFlag=[ActualSalesandBL].TimeFlag";
         DataSet ds = helper.GetDataSet(query);
         return ds;
     }
     //----------------------------------------------------------
     protected void DrawSlide_Thirteen()
     {
         slides.Add(++slideIndex, Microsoft.Office.Interop.PowerPoint.PpSlideLayout.ppLayoutTitleOnly);
         PowerPoint.Slide slide = slides[slideIndex];
         UniformLayout(slide, "ET HP " + meeting.getyear() + " Actual, " + meeting.getnextyear() + " Forecast");
         DataSet ds = getSlide_eleven();
         if (ds.Tables[0].Rows.Count == 0)
             return;
         DataSet ds1 = getSlide_Telwe();
         if (ds.Tables[0].Rows.Count == 0)
             return;
         DataSet ds2 = getSlide_TelweAdd();
         if (ds.Tables[0].Rows.Count == 0)
             return;
         //int x = 0;
         int y = 0;

         for (int i = 0; i < 2; i++)
         {
             ChartSpace objSpace = new ChartSpace();
             ChChart objChart = objSpace.Charts.Add(0);
             objChart.Type = ChartChartTypeEnum.chChartTypeColumnClustered;
             objChart.HasTitle = true;
             objChart.Title.Font.Size = 12;
             objChart.HasLegend = true;
             objChart.Title.Font.Bold = true;
             string dimValues = "";
             switch (i)
             {
                 case 0:
                     y = 80;
                     objChart.Title.Caption = "Bookings";
                     for (int j = 4; j > 0; j--)
                     {
                         dimValues = "";
                         for (int k = 0; k < ds.Tables[0].Rows.Count - 1; k++)
                         {
                             var num = ds.Tables[0].Rows[k][j];
                             dimValues += String.Format("{0:F}", (num is DBNull ? 0 : (double)num)) + "\t";
                         }
                         dimValues.Substring(0, dimValues.Length - 1);
                         objChart.SeriesCollection.Add(0);
                         objChart.SeriesCollection[0].Caption = ds.Tables[0].Columns[j].ColumnName;

                         objSpace.Charts[0].SeriesCollection[0].SetData(ChartDimensionsEnum.chDimCategories,
                                +(int)ChartSpecialDataSourcesEnum.chDataLiteral, dimSegmentCategories);
                         objSpace.Charts[0].SeriesCollection[0].SetData(ChartDimensionsEnum.chDimValues,
                                 +(int)ChartSpecialDataSourcesEnum.chDataLiteral, dimValues);
                     }
                     int rowCount = ds.Tables[0].Rows.Count;
                     int columnCount = ds.Tables[0].Columns.Count;
                     PowerPoint.Shape shape = slide.Shapes.AddTable(rowCount + 1, columnCount, 385, 300, 700, 200);

                     for (int s = 0; s < rowCount + 1; s++)
                     {
                         for (int t = 0; t < columnCount; t++)
                         {
                             PowerPoint.TextFrame textF = shape.Table.Cell(s + 1, t + 1).Shape.TextFrame;
                             PowerPoint.TextRange textR = textF.TextRange;
                             textR.Font.Bold = MsoTriState.msoFalse;
                             textR.Font.Size = 4;

                             if (s == 0)
                             {
                                 textR.Text = ds.Tables[0].Columns[t].ColumnName;
                             }
                             else
                             {
                                 if (t == 0)
                                 {
                                     var num1 = ds.Tables[0].Rows[s - 1][t];
                                     textR.Text = num1.ToString();
                                 }
                                 else
                                 {
                                     var num = ds.Tables[0].Rows[s - 1][t];
                                     textR.Text = Convert.ToDouble(num is DBNull ? 0 : num).ToString();
                                 }
                             }
                         }
                     }
                     shape.Export(picturePath, Microsoft.Office.Interop.PowerPoint.PpShapeFormat.ppShapeFormatGIF, 400, 200, Microsoft.Office.Interop.PowerPoint.PpExportMode.ppScaleXY);
                     shape.Delete();
                     InsertPicture(slide, picturePath, 400, 120, 240, 120);
                     InsertText(slide, "Bookings", 490, 80, 60, 35, 12);
                     break;
                 case 1:
                     objChart.Title.Caption = "Sales";
                     y = 300;
                     for (int j = 4; j > 0; j--)
                     {
                         dimValues = "";
                         for (int k = 0; k < ds1.Tables[0].Rows.Count - 1; k++)
                         {
                             var num1 = ds1.Tables[0].Rows[k][j];
                             var num2 = ds2.Tables[0].Rows[k][j];
                             dimValues += (Convert.ToDouble(num1 is DBNull ? 0 : num1) + Convert.ToDouble(num2 is DBNull ? 0 : num2)).ToString() + "\t";
                         }
                         dimValues.Substring(0, dimValues.Length - 1);
                         objChart.SeriesCollection.Add(0);
                         objChart.SeriesCollection[0].Caption = ds1.Tables[0].Columns[j].ColumnName;

                         objSpace.Charts[0].SeriesCollection[0].SetData(ChartDimensionsEnum.chDimCategories,
                                +(int)ChartSpecialDataSourcesEnum.chDataLiteral, dimSegmentCategories);
                         objSpace.Charts[0].SeriesCollection[0].SetData(ChartDimensionsEnum.chDimValues,
                                 +(int)ChartSpecialDataSourcesEnum.chDataLiteral, dimValues);
                     }
                     int rowCount1 = ds1.Tables[0].Rows.Count;
                     int columnCount1 = ds1.Tables[0].Columns.Count;
                     PowerPoint.Shape shape1 = slide.Shapes.AddTable(rowCount1 + 1, columnCount1, 385, 300, 700, 200);

                     for (int s = 0; s < rowCount1 + 1; s++)
                     {
                         for (int t = 0; t < columnCount1; t++)
                         {
                             PowerPoint.TextFrame textF = shape1.Table.Cell(s + 1, t + 1).Shape.TextFrame;
                             PowerPoint.TextRange textR = textF.TextRange;
                             textR.Font.Bold = MsoTriState.msoFalse;
                             textR.Font.Size = 4;

                             if (s == 0)
                             {
                                 textR.Text = ds1.Tables[0].Columns[t].ColumnName;
                             }
                             else
                             {
                                 if (t == 0)
                                 {
                                     var num1 = ds1.Tables[0].Rows[s - 1][t];
                                     textR.Text = num1.ToString();
                                 }
                                 else
                                 {
                                     var num1 = ds1.Tables[0].Rows[s - 1][t];
                                     var num2 = ds2.Tables[0].Rows[s - 1][t];
                                     textR.Text = (Convert.ToDouble(num1 is DBNull ? 0 : num1) + Convert.ToDouble(num2 is DBNull ? 0 : num2)).ToString();
                                 }
                             }
                         }
                     }
                     shape1.Export(picturePath, Microsoft.Office.Interop.PowerPoint.PpShapeFormat.ppShapeFormatGIF, 400, 200, Microsoft.Office.Interop.PowerPoint.PpExportMode.ppScaleXY);
                     shape1.Delete();
                     InsertPicture(slide, picturePath, 400, 335, 240, 120);
                     InsertText(slide, "Sales", 490, 295, 60, 35, 12);
                     break;
             }
             objSpace.ExportPicture(picturePath, "GIF", 260, 200);
             picturePath = pictureFolderPath + pictureName;
             InsertPicture(slide, picturePath, 60, y, 260, 200);
         }
     }
     //--------------------------------------------------------------------------
     protected void DrawSlide_Fourteen()
     {
         slides.Add(++slideIndex, Microsoft.Office.Interop.PowerPoint.PpSlideLayout.ppLayoutTitleOnly);
         PowerPoint.Slide slide = slides[slideIndex];
         UniformLayout(slide, "HP " + meeting.getpreyear().Trim() + "-" + meeting.getnextyear().Substring(2, 2) + " Bookings");
         InsertText(slide, "5 Top Countries CL & IN ", 205, 50, 280, 35, 24);
         string dimCategories = meeting.getpreyear().Substring(2, 2) + "\t" + meeting.getyear().Substring(2, 2) + "\t" + meeting.getnextyear().Substring(2, 2);

         DataSet ds = getSlide_Fourteen();
         if (ds == null)
             return;
         if (ds.Tables[0].Rows.Count == 0)
             return;
         int x = 22;
         string[] color = { "goldenrod", "deepskyblue", "forestgreen", "maroon", "yellow", "blue" };
         double[] total = { 0, 0, 0 };
         for (int i = 0; i < 6; i++)
         {
             string dimValues = "";
             ChartSpace objSpace = new ChartSpace();
             ChChart objChart = objSpace.Charts.Add(0);
             objChart.Type = ChartChartTypeEnum.chChartTypeAreaStacked;
             objChart.HasTitle = true;
             objChart.Title.Caption = ds.Tables[0].Columns[i + 1].ColumnName;
             objChart.Title.Font.Size = 12;
             objChart.Title.Font.Bold = true;
             if (i != 5)
             {
                 for (int j = 0; j < 3; j++)
                 {
                     var num = ds.Tables[0].Rows[j][i + 1];
                     dimValues += (num is DBNull ? 0 : num).ToString() + "\t";
                     total[j] += (num is DBNull ? 0 : (double)num);
                 }
             }
             else
             {
                 for (int j = 0; j < 3; j++)
                 {
                     var num = ds.Tables[0].Rows[j][i + 1];
                     dimValues += ((num is DBNull ? 0 : (double)num) - total[j]).ToString() + "\t";
                 }
             }
             dimValues = dimValues.Substring(0, dimValues.Length - 1);
             objChart.Border.Color = "black";
             objChart.Interior.Color = "mediumturquoise";
             objChart.PlotArea.Interior.Color = "white";
             objChart.SeriesCollection.Add(0);
             objChart.SeriesCollection[0].Interior.Color = color[i];
             objSpace.Charts[0].SeriesCollection[0].SetData(ChartDimensionsEnum.chDimCategories,
                        +(int)ChartSpecialDataSourcesEnum.chDataLiteral, dimCategories);
             objSpace.Charts[0].SeriesCollection[0].SetData(ChartDimensionsEnum.chDimValues,
                     +(int)ChartSpecialDataSourcesEnum.chDataLiteral, dimValues);
             objSpace.ExportPicture(picturePath, "GIF", 115, 170);
             picturePath = pictureFolderPath + pictureName;
             InsertPicture(slide, picturePath, x, 160, 115, 170);
             x += 113;
         }
     }
     protected DataSet getSlide_Fourteen()
    {
        string query_5TopCountry = "SELECT Top 5 [Country].Name AS 'countryID'"
                                   + " FROM [Bookings],[Segment],[Country],[SalesOrg],[SalesOrg_User],[Currency_Exchange]"
                                   + " WHERE [Bookings].SegmentID=[Segment].ID"
                                   + " AND Segment.Deleted=0 "
                                   + " AND Country.Deleted=0 "
                                   + " AND SalesOrg.Deleted=0 "
                                   + " AND SalesOrg_User.Deleted=0 "
                                   + " AND Currency_Exchange.Deleted=0 "
                                   + " AND [Country].ID=[Bookings].CountryID"
                                   + " AND [SalesOrg_User].UserID=[Bookings].RSMID"
                                   + " AND [SalesOrg_User].SalesOrgID=[SalesOrg].ID"
                                   + " AND [SalesOrg].CurrencyID=[Currency_Exchange].CurrencyID"
                                   + " AND ([Segment].Abbr='CL' OR [Segment].Abbr='IN')"
                                   + " GROUP BY [Country].Name"
                                   + " ORDER BY SUM(Amount*(CASE WHEN [Bookings].TimeFlag=[Currency_Exchange].TimeFlag"
                                   + " AND ([Bookings].DeliverY='YTD' OR [Bookings].DeliverY='10') THEN [Currency_Exchange].Rate1"
                                   + " ELSE [Currency_Exchange].Rate2 END)) DESC";
        DataSet ds_5TopCountry = helper.GetDataSet(query_5TopCountry);
        if (ds_5TopCountry.Tables.Count > 0 && ds_5TopCountry.Tables[0].Rows.Count > 0)
        {
            string query_preyear = "SELECT '" + meeting.getpreyear().Substring(2, 2).Trim() + "'AS ' ' ";
            string query_year = "SELECT '" + meeting.getyear().Substring(2, 2).Trim() + "'AS ' ' ";
            string query_nextyear = "SELECT '" + meeting.getnextyear().Substring(2, 2).Trim() + "'AS ' ' ";
            for (int i = 0; i < ds_5TopCountry.Tables[0].Rows.Count; i++)
            {
                string abbr = ds_5TopCountry.Tables[0].Rows[i][0].ToString();
                query_preyear += ",ROUND(SUM(CASE WHEN [Country].Name='" + abbr + "' THEN(Amount*(CASE WHEN ([Bookings].DeliverY='YTD'"
                              + " OR [Bookings].DeliverY='" + meeting.getpreyear().Substring(2, 2) + "') THEN [Currency_Exchange].Rate1"
                              + " ELSE [Currency_Exchange].Rate2 END))ELSE 0 END)/1000,0) AS '" + abbr + "' ";
                query_year += ",ROUND(SUM(CASE WHEN [Country].Name='" + abbr + "' THEN(Amount*(CASE WHEN ([Bookings].DeliverY='YTD'"
                              + " OR [Bookings].DeliverY='" + meeting.getyear().Substring(2, 2) + "') THEN [Currency_Exchange].Rate1"
                              + " ELSE [Currency_Exchange].Rate2 END))ELSE 0 END)/1000,0) AS '" + abbr + "' ";
                query_nextyear += ",ROUND(SUM(CASE WHEN [Country].Name='" + abbr + "' THEN(Amount*(CASE WHEN ([Bookings].DeliverY='YTD'"
                              + " OR [Bookings].DeliverY='" + meeting.getnextyear().Substring(2, 2) + "') THEN [Currency_Exchange].Rate1"
                              + " ELSE [Currency_Exchange].Rate2 END))ELSE 0 END)/1000,0)AS '" + abbr + "' ";
            }
            string str = ",ROUND(SUM(Amount*(CASE WHEN  ([Bookings].DeliverY='YTD'"
                       + " OR [Bookings].DeliverY='" + meeting.getnextyear().Substring(2, 2) + "') THEN [Currency_Exchange].Rate1"
                       + " ELSE [Currency_Exchange].Rate2 END))/1000,0)AS 'Others' ";
            query_preyear += str + " FROM [Bookings],[Segment],[Country],[SalesOrg],[SalesOrg_User],[Currency_Exchange]"
                           + " WHERE [Bookings].BookingY='" + meeting.getpreyear().Substring(2, 2).Trim() + "'"
                           + " AND YEAR([Bookings].TimeFlag)='" + meeting.getpreyear().Trim() + "'"
                           + " AND MONTH([Bookings].TimeFlag)='10'"
                           + " AND Segment.Deleted=0 "
                           + " AND Country.Deleted=0 "
                           + " AND SalesOrg.Deleted=0 "
                           + " AND SalesOrg_User.Deleted=0 "
                           + " AND Currency_Exchange.Deleted=0 "
                           + " AND [Bookings].SegmentID=[Segment].ID"
                           + " AND ([Segment].Abbr='IN' OR [Segment].Abbr='CL')"
                           + " AND [Country].ID=[Bookings].CountryID"
                           + " AND [SalesOrg].ID=[SalesOrg_User].SalesOrgID"
                           + " AND [SalesOrg_User].UserID=[Bookings].RSMID"
                           + " AND [SalesOrg].CurrencyID=[Currency_Exchange].CurrencyID";
            query_year += str + " FROM [Bookings],[Segment],[Country],[SalesOrg],[SalesOrg_User],[Currency_Exchange]"
                           + " WHERE [Bookings].BookingY='" + meeting.getyear().Substring(2, 2).Trim() + "'"
                           + " AND YEAR([Bookings].TimeFlag)='" + meeting.getyear().Trim() + "'"
                           + " AND MONTH([Bookings].TimeFlag)='" + month.Trim() + "'"
                           + " AND Segment.Deleted=0 "
                           + " AND Country.Deleted=0 "
                           + " AND SalesOrg.Deleted=0 "
                           + " AND SalesOrg_User.Deleted=0 "
                           + " AND Currency_Exchange.Deleted=0 "
                           + " AND [Bookings].SegmentID=[Segment].ID"
                           + " AND ([Segment].Abbr='IN' OR [Segment].Abbr='CL')"
                           + " AND [Country].ID=[Bookings].CountryID"
                           + " AND [SalesOrg].ID=[SalesOrg_User].SalesOrgID"
                           + " AND [SalesOrg_User].UserID=[Bookings].RSMID"
                           + " AND [SalesOrg].CurrencyID=[Currency_Exchange].CurrencyID";
            query_nextyear += str + " FROM [Bookings],[Segment],[Country],[SalesOrg],[SalesOrg_User],[Currency_Exchange]"
                           + " WHERE [Bookings].BookingY='" + meeting.getnextyear().Substring(2, 2).Trim() + "'"
                           + " AND YEAR([Bookings].TimeFlag)='" + meeting.getyear().Trim() + "'"
                           + " AND MONTH([Bookings].TimeFlag)='" + month.Trim() + "'"
                           + " AND Segment.Deleted=0 "
                           + " AND Country.Deleted=0 "
                           + " AND SalesOrg.Deleted=0 "
                           + " AND SalesOrg_User.Deleted=0 "
                           + " AND Currency_Exchange.Deleted=0 "
                           + " AND [Bookings].SegmentID=[Segment].ID"
                           + " AND ([Segment].Abbr='IN' OR [Segment].Abbr='CL')"
                           + " AND [Country].ID=[Bookings].CountryID"
                           + " AND [SalesOrg].ID=[SalesOrg_User].SalesOrgID"
                           + " AND [SalesOrg_User].UserID=[Bookings].RSMID"
                           + " AND [SalesOrg].CurrencyID=[Currency_Exchange].CurrencyID";
            query_preyear += " UNION " + query_year + " UNION " + query_nextyear;
            DataSet ds = helper.GetDataSet(query_preyear);
            return ds;
        }
        else
            return null;
    }
     //--------------------------------------------------------------------------
     protected void DrawSlide_Fifteen()
     {
         slides.Add(++slideIndex, Microsoft.Office.Interop.PowerPoint.PpSlideLayout.ppLayoutTitleOnly);
         PowerPoint.Slide slide = slides[slideIndex];
         UniformLayout(slide, "HP " + meeting.getpreyear().Trim() + "-" + meeting.getnextyear().Substring(2, 2) + " Bookings");
         InsertText(slide, "5 Top Countries Group", 205, 50, 280, 35, 24);
         string dimCategories = meeting.getpreyear().Substring(2, 2) + "\t" + meeting.getyear().Substring(2, 2) + "\t" + meeting.getnextyear().Substring(2, 2);

         DataSet ds = getSlide_Fifteen();
         if (ds == null || ds.Tables[0].Rows.Count == 0)
             return;
         int x = 22;
         string[] color = { "goldenrod", "deepskyblue", "forestgreen", "maroon", "yellow", "blue" };
         double[] total = { 0, 0, 0 };
         for (int i = 0; i < 6; i++)
         {
             string dimValues = "";
             ChartSpace objSpace = new ChartSpace();
             ChChart objChart = objSpace.Charts.Add(0);
             objChart.Type = ChartChartTypeEnum.chChartTypeAreaStacked;
             objChart.HasTitle = true;
             objChart.Title.Caption = ds.Tables[0].Columns[i + 1].ColumnName;
             objChart.Title.Font.Size = 12;
             objChart.Title.Font.Bold = true;
             if (i != 5)
             {
                 for (int j = 0; j < 3; j++)
                 {
                     var num = ds.Tables[0].Rows[j][i + 1];
                     dimValues += (num is DBNull ? 0 : num).ToString() + "\t";
                     total[j] += (num is DBNull ? 0 : (double)num);
                 }
             }
             else
             {
                 for (int j = 0; j < 3; j++)
                 {
                     var num = ds.Tables[0].Rows[j][i + 1];
                     dimValues += ((num is DBNull ? 0 : (double)num) - total[j]).ToString() + "\t";
                 }
             }
             dimValues = dimValues.Substring(0, dimValues.Length - 1);
             objChart.SeriesCollection.Add(0);
             objChart.Border.Color = "black";
             objChart.Interior.Color = "mediumturquoise";
             objChart.PlotArea.Interior.Color = "white";
             objChart.SeriesCollection[0].Interior.Color = color[i];
             objSpace.Charts[0].SeriesCollection[0].SetData(ChartDimensionsEnum.chDimCategories,
                        +(int)ChartSpecialDataSourcesEnum.chDataLiteral, dimCategories);
             objSpace.Charts[0].SeriesCollection[0].SetData(ChartDimensionsEnum.chDimValues,
                     +(int)ChartSpecialDataSourcesEnum.chDataLiteral, dimValues);
             objSpace.ExportPicture(picturePath, "GIF", 115, 170);
             picturePath = pictureFolderPath + pictureName;
             InsertPicture(slide, picturePath, x, 160, 115, 170);
             x += 113;
         }
     }
     protected DataSet getSlide_Fifteen()
    {
        string query_5TopCountry = "SELECT Top 5 [Country].Name AS 'countryID'"
                                   + " FROM [Bookings],[Country],[SalesOrg],[SalesOrg_User],[Currency_Exchange]"
                                   + " WHERE [Country].ID=[Bookings].CountryID"
                                   + " AND Country.Deleted=0 "
                                   + " AND SalesOrg.Deleted=0 "
                                   + " AND SalesOrg_User.Deleted=0 "
                                   + " AND Currency_Exchange.Deleted=0 "
                                   + " AND [SalesOrg_User].UserID=[Bookings].RSMID"
                                   + " AND [SalesOrg_User].SalesOrgID=[SalesOrg].ID"
                                   + " AND [SalesOrg].CurrencyID=[Currency_Exchange].CurrencyID"
                                   + " GROUP BY [Country].Name"
                                   + " ORDER BY SUM(Amount*(CASE WHEN [Bookings].TimeFlag=[Currency_Exchange].TimeFlag"
                                   + " AND ([Bookings].DeliverY='YTD' OR [Bookings].DeliverY='10') THEN [Currency_Exchange].Rate1"
                                   + " ELSE [Currency_Exchange].Rate2 END)) DESC";
        DataSet ds_5TopCountry = helper.GetDataSet(query_5TopCountry);
        if (ds_5TopCountry.Tables.Count > 0 && ds_5TopCountry.Tables[0].Rows.Count > 0)
        {
            string query_preyear = "SELECT '" + meeting.getpreyear().Substring(2, 2).Trim() + "'AS ' ' ";
            string query_year = "SELECT '" + meeting.getyear().Substring(2, 2).Trim() + "'AS ' ' ";
            string query_nextyear = "SELECT '" + meeting.getnextyear().Substring(2, 2).Trim() + "'AS ' ' ";
            for (int i = 0; i < ds_5TopCountry.Tables[0].Rows.Count; i++)
            {
                string abbr = ds_5TopCountry.Tables[0].Rows[i][0].ToString();
                query_preyear += ",ROUND(SUM(CASE WHEN [Country].Name='" + abbr + "' THEN(Amount*(CASE WHEN ([Bookings].DeliverY='YTD'"
                              + " OR [Bookings].DeliverY='" + meeting.getpreyear().Substring(2, 2) + "') THEN [Currency_Exchange].Rate1"
                              + " ELSE [Currency_Exchange].Rate2 END))ELSE 0 END)/1000,0) AS '" + abbr + "' ";
                query_year += ",ROUND(SUM(CASE WHEN [Country].Name='" + abbr + "' THEN(Amount*(CASE WHEN ([Bookings].DeliverY='YTD'"
                              + " OR [Bookings].DeliverY='" + meeting.getyear().Substring(2, 2) + "') THEN [Currency_Exchange].Rate1"
                              + " ELSE [Currency_Exchange].Rate2 END))ELSE 0 END)/1000,0) AS '" + abbr + "' ";
                query_nextyear += ",ROUND(SUM(CASE WHEN [Country].Name='" + abbr + "' THEN(Amount*(CASE WHEN ([Bookings].DeliverY='YTD'"
                              + " OR [Bookings].DeliverY='" + meeting.getnextyear().Substring(2, 2) + "') THEN [Currency_Exchange].Rate1"
                              + " ELSE [Currency_Exchange].Rate2 END))ELSE 0 END)/1000,0)AS '" + abbr + "' ";
            }
            string str = ",ROUND(SUM(Amount*(CASE WHEN  ([Bookings].DeliverY='YTD'"
                       + " OR [Bookings].DeliverY='" + meeting.getnextyear().Substring(2, 2) + "') THEN [Currency_Exchange].Rate1"
                       + " ELSE [Currency_Exchange].Rate2 END))/1000,0)AS 'Others' ";
            query_preyear += str + " FROM [Bookings],[Country],[SalesOrg],[SalesOrg_User],[Currency_Exchange]"
                           + " WHERE [Bookings].BookingY='" + meeting.getpreyear().Substring(2, 2).Trim() + "'"
                           + " AND YEAR([Bookings].TimeFlag)='" + meeting.getpreyear().Trim() + "'"
                           + " AND MONTH([Bookings].TimeFlag)='10'"
                           + " AND Country.Deleted=0 "
                           + " AND SalesOrg.Deleted=0 "
                           + " AND SalesOrg_User.Deleted=0 "
                           + " AND Currency_Exchange.Deleted=0 "
                           + " AND [Country].ID=[Bookings].CountryID"
                           + " AND [SalesOrg].ID=[SalesOrg_User].SalesOrgID"
                           + " AND [SalesOrg_User].UserID=[Bookings].RSMID"
                           + " AND [SalesOrg].CurrencyID=[Currency_Exchange].CurrencyID";
            query_year += str + " FROM [Bookings],[Country],[SalesOrg],[SalesOrg_User],[Currency_Exchange]"
                           + " WHERE [Bookings].BookingY='" + meeting.getyear().Substring(2, 2).Trim() + "'"
                           + " AND YEAR([Bookings].TimeFlag)='" + meeting.getyear().Trim() + "'"
                           + " AND MONTH([Bookings].TimeFlag)='" + month.Trim() + "'"
                           + " AND Country.Deleted=0 "
                           + " AND SalesOrg.Deleted=0 "
                           + " AND SalesOrg_User.Deleted=0 "
                           + " AND Currency_Exchange.Deleted=0 "
                           + " AND [Country].ID=[Bookings].CountryID"
                           + " AND [SalesOrg].ID=[SalesOrg_User].SalesOrgID"
                           + " AND [SalesOrg_User].UserID=[Bookings].RSMID"
                           + " AND [SalesOrg].CurrencyID=[Currency_Exchange].CurrencyID";
            query_nextyear += str + " FROM [Bookings],[Country],[SalesOrg],[SalesOrg_User],[Currency_Exchange]"
                           + " WHERE [Bookings].BookingY='" + meeting.getnextyear().Substring(2, 2).Trim() + "'"
                           + " AND YEAR([Bookings].TimeFlag)='" + meeting.getyear().Trim() + "'"
                           + " AND MONTH([Bookings].TimeFlag)='" + month.Trim() + "'"
                           + " AND Country.Deleted=0 "
                           + " AND SalesOrg.Deleted=0 "
                           + " AND SalesOrg_User.Deleted=0 "
                           + " AND Currency_Exchange.Deleted=0 "
                           + " AND [Country].ID=[Bookings].CountryID"
                           + " AND [SalesOrg].ID=[SalesOrg_User].SalesOrgID"
                           + " AND [SalesOrg_User].UserID=[Bookings].RSMID"
                           + " AND [SalesOrg].CurrencyID=[Currency_Exchange].CurrencyID";
            query_preyear += " UNION " + query_year + " UNION " + query_nextyear;
            DataSet ds = helper.GetDataSet(query_preyear);
            return ds;
        }
        else
            return null;
    }
     protected DataSet getSlide_FifteenTotalCountry()
     {
         string query_getBookingTotalByperiod = "SELECT SUM(Amount) AS 'Total'"
                                               + ",SUM((CASE WHEN [Bookings].BookingY='" + meeting.getpreyear().Substring(2, 2) + "' THEN Amount ELSE 0 END))AS '09'"
                                               + ",SUM((CASE WHEN [Bookings].BookingY='" + meeting.getyear().Substring(2, 2) + "' THEN Amount ELSE 0 END))AS '10'"
                                               + ",SUM((CASE WHEN [Bookings].BookingY='" + meeting.getnextyear().Substring(2, 2) + "' THEn Amount ELSE 0 END))AS '11'"
                                               + " FROM [Bookings],[Segment],[Country]"
                                               + " WHERE [Bookings].SegmentID=[Segment].ID"
                                               + " AND Segment.Deleted=0 "
                                               + " AND Country.Deleted=0 "
                                               + " AND [Country].ID=[Bookings].CountryID";
         DataSet ds = helper.GetDataSet(query_getBookingTotalByperiod);
         return ds;
     }
     //---------------------------------------------------
     protected void DrawSlide_Sixteen()
     {
         int slideNum = segmentNumber / 2 + (segmentNumber % 2 == 0 ? 0 : 1);
         for (int i = 0; i < slideNum; i++)
         {
             slides.Add(++slideIndex, Microsoft.Office.Interop.PowerPoint.PpSlideLayout.ppLayoutTitleOnly);
             PowerPoint.Slide slide = slides[slideIndex];
             UniformLayout(slide, meeting.getyear().Trim() + " & " + meeting.getnextyear().Trim() + "  Sales Forecast   by Products");
             int chartNum = 2;
             float x = 40;
             float x1 = 70;
             if ((i == slideNum - 1) && (segmentNumber % 2 == 1))
             {
                 chartNum = 1;
             }
             DataSet[] ds = new DataSet[chartNum];
             DataSet[] dsAdd = new DataSet[chartNum];
             bool slideIsNull = true;
             for (int j = 0; j < chartNum; j++)
             {
                 ds[j] = getSlide_Sixteen(alSegment[2 * i + j].ToString());
                 dsAdd[j] = getSlide_SixteenAdd(alSegment[2 * i + j].ToString());
                 if (ds[j] != null && ds[j].Tables.Count > 0 && ds[j].Tables[0].Rows.Count > 0)
                 {
                     slideIsNull = false;
                     string dimCategories = "";
                     string dimValues = "";
                     ChartSpace objSpace = new ChartSpace();
                     ChChart objChart = objSpace.Charts.Add(0);
                     objChart.Type = ChartChartTypeEnum.chChartTypeColumnClustered;
                     objChart.HasTitle = true;
                     objChart.HasLegend = true;
                     objChart.Title.Font.Size = 12;
                     objChart.Title.Font.Bold = true;
                     for (int m = 1; m < ds[j].Tables[0].Columns.Count; m++)
                     {
                         dimCategories += ds[j].Tables[0].Columns[m].ColumnName + "\t";
                     }
                     dimCategories = dimCategories.Substring(0, dimCategories.Length - 1);
                     for (int k = ds[j].Tables[0].Rows.Count - 1; k >= 0; k--)
                     {
                         dimValues = "";
                         for (int l = 1; l < ds[j].Tables[0].Columns.Count; l++)
                         {
                             var num1 = ds[j].Tables[0].Rows[k][l];
                             if (dsAdd[j] != null && dsAdd[j].Tables.Count > 0 && dsAdd[j].Tables[0].Rows.Count > 0)
                             {
                                 var num2 = dsAdd[j].Tables[0].Rows[k][l];
                                 dimValues += Convert.ToDouble(num1 is DBNull ? 0 : num1) + Convert.ToDouble(num2 is DBNull ? 0 : num2) + "\t";
                             }
                             else
                                 dimValues += Convert.ToDouble(num1 is DBNull ? 0 : num1) + "\t";
                         }
                         dimValues = dimValues.Substring(0, dimValues.Length - 1);
                         objChart.SeriesCollection.Add(0);
                         objChart.SeriesCollection[0].Caption = ds[j].Tables[0].Rows[k][0].ToString();
                         objChart.Title.Caption = alSegment[2 * i + j].ToString() + " Products";
                         objSpace.Charts[0].SeriesCollection[0].SetData(ChartDimensionsEnum.chDimCategories,
                                    +(int)ChartSpecialDataSourcesEnum.chDataLiteral, dimCategories);
                         objSpace.Charts[0].SeriesCollection[0].SetData(ChartDimensionsEnum.chDimValues,
                                 +(int)ChartSpecialDataSourcesEnum.chDataLiteral, dimValues);
                     }

                     objSpace.ExportPicture(picturePath, "GIF", 300, 200);
                     picturePath = pictureFolderPath + pictureName;
                     InsertPicture(slide, picturePath, x, 80, 300, 200);

                     int rowCount = ds[j].Tables[0].Rows.Count;
                     int columnCount = ds[j].Tables[0].Columns.Count;
                     PowerPoint.Shape shape = slide.Shapes.AddTable(rowCount + 1, columnCount, 385, 300, 700, 200);

                     for (int s = 0; s < rowCount + 1; s++)
                     {
                         for (int t = 0; t < columnCount; t++)
                         {
                             PowerPoint.TextFrame textF = shape.Table.Cell(s + 1, t + 1).Shape.TextFrame;
                             textF.HorizontalAnchor = Microsoft.Office.Core.MsoHorizontalAnchor.msoAnchorCenter;
                             PowerPoint.TextRange textR = textF.TextRange;
                             textR.Font.Bold = MsoTriState.msoTrue;
                             textR.Font.Size = 4;
                             if (s == 0)
                             {
                                 textR.Text = ds[j].Tables[0].Columns[t].ColumnName;
                             }
                             else
                             {
                                 if (t == 0)
                                 {
                                     var num1 = ds[j].Tables[0].Rows[s - 1][t];
                                     textR.Text = num1.ToString();
                                 }
                                 else
                                 {
                                     var num1 = ds[j].Tables[0].Rows[s - 1][t];
                                     if (dsAdd[j] != null && dsAdd[j].Tables.Count > 0 && dsAdd[j].Tables[0].Rows.Count > 0)
                                     {
                                         var num2 = dsAdd[j].Tables[0].Rows[s - 1][t];
                                         textR.Text = (Convert.ToDouble(num1 is DBNull ? 0 : num1) + Convert.ToDouble(num2 is DBNull ? 0 : num2)).ToString();
                                     }
                                     else
                                         textR.Text = (Convert.ToDouble(num1 is DBNull ? 0 : num1)).ToString();
                                 }
                             }
                         }
                     }
                     shape.Export(picturePath, Microsoft.Office.Interop.PowerPoint.PpShapeFormat.ppShapeFormatGIF, 480, 240, Microsoft.Office.Interop.PowerPoint.PpExportMode.ppScaleXY);
                     shape.Delete();
                     InsertPicture(slide, picturePath, x1, 340, 240, 120);
                     x1 = 410;
                     x = 380;
                 }
             }
             if (slideIsNull == true)
                 --slideIndex;
         }
     }
     protected DataSet getSlide_Sixteen(string abbr)
     {
         string query_ExistOrNot = " SELECT Count(RSMID) FROM [Bookings],[Segment]"
                                 + " WHERE [Bookings].SegmentID=[Segment].ID"
                                 + " AND Segment.Deleted=0 "
                                 + " AND [Segment].Abbr='" + abbr + "'";
         DataSet ds_ExistOrNot = helper.GetDataSet(query_ExistOrNot);
         if (ds_ExistOrNot.Tables[0].Rows.Count == 0)
             return null;
         if (ds_ExistOrNot.Tables.Count > 0 && ds_ExistOrNot.Tables[0].Rows.Count > 0 && (int)ds_ExistOrNot.Tables[0].Rows[0][0] != 0)
         {
             string query_product = " SELECT [Product].ID,[Product].Abbr FROM [Segment],[Product],[Segment_Product]"
                             + " WHERE [Segment].Abbr='" + abbr + "'"
                             + " AND Segment.Deleted=0 "
                             + " AND Product.Deleted=0 "
                             + " AND Segment_Product.Deleted=0 "
                             + " AND [Segment].ID=[Segment_Product].SegmentID"
                             + " AND [Segment_Product].ProductID=[Product].ID";
             DataSet ds_product = helper.GetDataSet(query_product);
             if (ds_product.Tables[0].Rows.Count == 0)
                 return null;
             if (ds_product.Tables.Count > 0 && ds_product.Tables[0].Rows.Count > 0)
             {
                 string query_yearA = " SELECT '" + meeting.getyear().Trim() + " A' AS ' '";
                 string query_yearB = "SELECT '" + meeting.getyear() + " B' AS ' '";
                 string query_nextyearB = "SELECT '" + meeting.getnextyear().Trim() + " B' AS ' '";
                 string query_nextyearF = "SELECT '" + meeting.getnextyear().Trim() + " F' AS ' '";
                 for (int i = 0; i < ds_product.Tables[0].Rows.Count; i++)
                 {
                     string str1 = ds_product.Tables[0].Rows[i][0].ToString();
                     string str2 = ds_product.Tables[0].Rows[i][1].ToString();
                     query_yearA += " ,ROUND(0/1000,0) AS '" + str2 + "'";
                     query_yearB += " ,ROUND(SUM(CASE WHEN [Bookings].ProductID='" + str1 + "'"
                                  + " THEN Amount*[Currency_Exchange].Rate2 ELSE 0 END)/1000,0) AS '" + str2 + "'";
                     query_nextyearB += " ,ROUND(SUM(CASE WHEN [Bookings].ProductID='" + str1 + "'"
                                  + " THEN Amount*[Currency_Exchange].Rate2 ELSE 0 END)/1000,0)AS '" + str2 + "'";
                     query_nextyearF += " ,ROUND(SUM(CASE WHEN [Bookings].ProductID='" + str1 + "'"
                                  + " THEN Amount*[Currency_Exchange].Rate2 ELSE 0 END)/1000,0)AS '" + str2 + "'";
                 }
                 query_yearA += " FROM [Bookings]";
                 query_yearB += " FROM [Bookings],[Segment],[SalesOrg],[SalesOrg_User],[Currency_Exchange]"
                              + " WHERE [Bookings].SegmentID=[Segment].ID"
                              + " AND [Segment].Abbr='" + abbr + "'"
                              + " AND Segment.Deleted=0 "
                              + " AND SalesOrg.Deleted=0 "
                              + " AND SalesOrg_User.Deleted=0 "
                              + " AND Currency_Exchange.Deleted=0 "
                              + " AND [Bookings].RSMID=[SalesOrg_User].UserID"
                              + " AND [SalesOrg_User].SalesOrgID=[SalesOrg].ID"
                              + " AND [SalesOrg].CurrencyID=[Currency_Exchange].CurrencyID"
                              + " AND [Currency_Exchange].TimeFlag=[Bookings].TimeFlag"
                              + " AND [Bookings].DeliverY='" + meeting.getyear().Substring(2, 2).Trim() + "'"
                              + " AND ([Bookings].BookingY='" + meeting.getpreyear().Substring(2, 2).Trim() + "'"
                              + " OR [Bookings].BookingY='" + meeting.getyear().Substring(2, 2).Trim() + "'"
                              + " AND YEAR([Bookings].TimeFlag)='" + meeting.getpreyear().Trim() + "')"
                              + " AND MONTH([Bookings].TimeFlag)='03'";
                 query_nextyearB += " FROM [Bookings],[Segment],[SalesOrg],[SalesOrg_User],[Currency_Exchange]"
                              + " WHERE [Bookings].SegmentID=[Segment].ID"
                              + " AND [Segment].Abbr='" + abbr + "'"
                              + " AND Segment.Deleted=0 "
                              + " AND SalesOrg.Deleted=0 "
                              + " AND SalesOrg_User.Deleted=0 "
                              + " AND Currency_Exchange.Deleted=0 "
                              + " AND [Bookings].RSMID=[SalesOrg_User].UserID"
                              + " AND [SalesOrg_User].SalesOrgID=[SalesOrg].ID"
                              + " AND [SalesOrg].CurrencyID=[Currency_Exchange].CurrencyID"
                              + " AND [Currency_Exchange].TimeFlag=[Bookings].TimeFlag"
                              + " AND [Bookings].DeliverY='" + meeting.getnextyear().Substring(2, 2).Trim() + "'"
                              + " AND ([Bookings].BookingY='" + meeting.getnextyear().Substring(2, 2).Trim() + "'"
                              + " OR [Bookings].BookingY='" + meeting.getyear().Substring(2, 2).Trim() + "')"
                              + " AND YEAR([Bookings].TimeFlag)='" + meeting.getyear().Trim() + "'"
                              + " AND MONTH([Bookings].TimeFlag)='03'";
                 query_nextyearF += " FROM [Bookings],[Segment],[SalesOrg],[SalesOrg_User],[Currency_Exchange]"
                              + " WHERE [Bookings].SegmentID=[Segment].ID"
                              + " AND [Segment].Abbr='" + abbr + "'"
                              + " AND Segment.Deleted=0 "
                              + " AND SalesOrg.Deleted=0 "
                              + " AND SalesOrg_User.Deleted=0 "
                              + " AND Currency_Exchange.Deleted=0 "
                              + " AND [Bookings].RSMID=[SalesOrg_User].UserID"
                              + " AND [SalesOrg_User].SalesOrgID=[SalesOrg].ID"
                              + " AND [SalesOrg].CurrencyID=[Currency_Exchange].CurrencyID"
                              + " AND [Currency_Exchange].TimeFlag=[Bookings].TimeFlag"
                              + " AND [Bookings].DeliverY='" + meeting.getnextyear().Substring(2, 2).Trim() + "'"
                              + " AND ([Bookings].BookingY='" + meeting.getyear().Substring(2, 2).Trim() + "'"
                              + " OR [Bookings].BookingY='" + meeting.getnextyear().Substring(2, 2).Trim() + "')"
                              + " AND YEAR([Bookings].TimeFlag)='" + meeting.getyear().Trim() + "'"
                              + " AND MONTH([Bookings].TimeFlag)='" + month.Trim() + "'";
                 query_yearA += " UNION " + query_yearB + " UNION " + query_nextyearB + " UNION " + query_nextyearF;
                 DataSet ds = helper.GetDataSet(query_yearA);
                 return ds;
             }
             else
                 return null;
         }
         else
             return null;
     }
     protected DataSet getSlide_SixteenAdd(string abbr)
     {
         string query_ExistOrNot = " SELECT Count(SegmentID) FROM [ActualSalesandBL],[Segment]"
                                 + " WHERE [ActualSalesandBL].SegmentID=[Segment].ID"
                                 + " AND [Segment].Abbr='" + abbr + "'";
         DataSet ds_ExistOrNot = helper.GetDataSet(query_ExistOrNot);
         if (ds_ExistOrNot.Tables.Count > 0 && ds_ExistOrNot.Tables[0].Rows.Count > 0 && (int)ds_ExistOrNot.Tables[0].Rows[0][0] != 0)
         {
             string query_product = " SELECT [Product].ID,[Product].Abbr FROM [Segment],[Product],[Segment_Product]"
                                 + " WHERE [Segment].Abbr='" + abbr + "'"
                                 + " AND Segment.Deleted=0 "
                                 + " AND Product.Deleted=0 "
                                 + " AND Segment_Product.Deleted=0 "
                                 + " AND [Segment].ID=[Segment_Product].SegmentID"
                                 + " AND [Segment_Product].ProductID=[Product].ID";
             DataSet ds_product = helper.GetDataSet(query_product);
             if (ds_product.Tables.Count > 0 && ds_product.Tables[0].Rows.Count > 0)
             {
                 string query_yearA = " SELECT '" + meeting.getyear().Trim() + " A' AS ' '";
                 string query_yearB = "SELECT '" + meeting.getyear() + " B' AS ' '";
                 string query_nextyearB = "SELECT '" + meeting.getnextyear().Trim() + " B' AS ' '";
                 string query_nextyearF = "SELECT '" + meeting.getnextyear().Trim() + " F' AS ' '";
                 for (int i = 0; i < ds_product.Tables[0].Rows.Count; i++)
                 {
                     string str1 = ds_product.Tables[0].Rows[i][0].ToString();
                     string str2 = ds_product.Tables[0].Rows[i][1].ToString();
                     query_yearA += " ,ROUND(SUM(CASE WHEN [ActualSalesandBL].BacklogY='" + meeting.getyear().Substring(2, 2).Trim() + "'"
                                  + " AND YEAR([ActualSalesandBL].TimeFlag)='" + meeting.getyear().Trim() + "'"
                                  + " AND MONTH([ActualSalesandBL].TimeFlag)='" + month.Trim() + "'"
                                  + " AND [ActualSalesandBL].ProductID='" + str1 + "'"
                                  + " THEN Backlog*([Currency_Exchange].Rate1) ELSE 0 END)/1000,0) AS '" + str2 + "'";
                     query_yearB += " ,ROUND(SUM(CASE WHEN [ActualSalesandBL].BacklogY='" + meeting.getyear().Substring(2, 2).Trim() + "'"
                                  + " AND YEAR([ActualSalesandBL].TimeFlag)='" + meeting.getpreyear().Trim() + "'"
                                  + " AND MONTH([ActualSalesandBL].TimeFlag)='03'"
                                  + " AND [ActualSalesandBL].ProductID='" + str1 + "'"
                                  + " THEN Backlog*[Currency_Exchange].Rate2 ELSE 0 END)/1000,0)AS '" + str2 + "'";
                     query_nextyearB += " ,ROUND(SUM(CASE WHEN [ActualSalesandBL].BacklogY='" + meeting.getnextyear().Substring(2, 2).Trim() + "'"
                                  + " AND YEAR([ActualSalesandBL].TimeFlag)='" + meeting.getyear().Trim() + "'"
                                  + " AND MONTH([ActualSalesandBL].TimeFlag)='03'"
                                  + " AND [ActualSalesandBL].ProductID='" + str1 + "'"
                                  + " THEN Backlog*[Currency_Exchange].Rate2 ELSE 0 END)/1000,0)AS '" + str2 + "'";
                     query_nextyearF += " ,ROUND(SUM(CASE WHEN [ActualSalesandBL].BacklogY='" + meeting.getnextyear().Substring(2, 2).Trim() + "'"
                                  + " AND YEAR([ActualSalesandBL].TimeFlag)='" + meeting.getyear().Trim() + "'"
                                  + " AND MONTH([ActualSalesandBL].TimeFlag)='" + month.Trim() + "'"
                                  + " AND [ActualSalesandBL].ProductID='" + str1 + "'"
                                  + " THEN Backlog*[Currency_Exchange].Rate2 ELSE 0 END)/1000,0)AS '" + str2 + "'";
                 }
                 query_yearA += " FROM [ActualSalesandBL],[Segment],[SalesOrg],[Currency_Exchange]"
                              + " WHERE [ActualSalesandBL].SegmentID=[Segment].ID"
                              + " AND [Segment].Abbr='" + abbr + "'"
                              + " AND Segment.Deleted=0 "
                              + " AND SalesOrg.Deleted=0 "
                              + " AND Currency_Exchange.Deleted=0 "
                              + " AND [ActualSalesandBL].SalesOrgID=[SalesOrg].ID"
                              + " AND [SalesOrg].CurrencyID=[Currency_Exchange].CurrencyID"
                              + " AND [ActualSalesandBL].TimeFlag=[Currency_Exchange].TimeFlag";
                 query_yearB += " FROM [ActualSalesandBL],[Segment],[SalesOrg],[Currency_Exchange]"
                              + " WHERE [ActualSalesandBL].SegmentID=[Segment].ID"
                              + " AND [Segment].Abbr='" + abbr + "'"
                              + " AND Segment.Deleted=0 "
                              + " AND SalesOrg.Deleted=0 "
                              + " AND Currency_Exchange.Deleted=0 "
                              + " AND [ActualSalesandBL].SalesOrgID=[SalesOrg].ID"
                              + " AND [SalesOrg].CurrencyID=[Currency_Exchange].CurrencyID"
                              + " AND [ActualSalesandBL].TimeFlag=[Currency_Exchange].TimeFlag";
                 query_nextyearB += " FROM [ActualSalesandBL],[Segment],[SalesOrg],[Currency_Exchange]"
                              + " WHERE [ActualSalesandBL].SegmentID=[Segment].ID"
                              + " AND [Segment].Abbr='" + abbr + "'"
                              + " AND Segment.Deleted=0 "
                              + " AND SalesOrg.Deleted=0 "
                              + " AND Currency_Exchange.Deleted=0 "
                              + " AND [ActualSalesandBL].SalesOrgID=[SalesOrg].ID"
                              + " AND [SalesOrg].CurrencyID=[Currency_Exchange].CurrencyID"
                              + " AND [ActualSalesandBL].TimeFlag=[Currency_Exchange].TimeFlag";
                 query_nextyearF += " FROM [ActualSalesandBL],[Segment],[SalesOrg],[Currency_Exchange]"
                              + " WHERE [ActualSalesandBL].SegmentID=[Segment].ID"
                              + " AND [Segment].Abbr='" + abbr + "'"
                              + " AND Segment.Deleted=0 "
                              + " AND SalesOrg.Deleted=0 "
                              + " AND Currency_Exchange.Deleted=0 "
                              + " AND [ActualSalesandBL].SalesOrgID=[SalesOrg].ID"
                              + " AND [SalesOrg].CurrencyID=[Currency_Exchange].CurrencyID"
                              + " AND [ActualSalesandBL].TimeFlag=[Currency_Exchange].TimeFlag";
                 query_yearA += " UNION " + query_yearB + " UNION " + query_nextyearB + " UNION " + query_nextyearF;
                 DataSet ds = helper.GetDataSet(query_yearA);
                 return ds;
             }
             else
                 return null;
         }
         else
             return null;
     }
     //---------------------------------------------------------------------------
     protected void DrawSlide_Nineteen()
     {
         PowerPoint.Slide slide;
         for (int i = 0; i < segmentNumber; i++)
         {
             DataSet ds_operation = getOperation(alSegment[i].ToString());
             if (ds_operation.Tables.Count > 0 && ds_operation.Tables[0].Rows.Count > 0)
             {
                 int numOfSegment = ds_operation.Tables[0].Rows.Count / 2;
                 int count = 0;
                 for (int j = 0; j < numOfSegment + (ds_operation.Tables[0].Rows.Count % 2 == 0 ? 0 : 1); j++)
                 {
                     float x = 60;
                     float x1 = 70;
                     slides.Add(++slideIndex, Microsoft.Office.Interop.PowerPoint.PpSlideLayout.ppLayoutTitleOnly);
                     slide = slides[slideIndex];
                     UniformLayout(slide, alSegment[i].ToString().Trim() + " Sales By Operation by Product 2011 vs 2010");
                     int chartNum = 2;
                     if (j == numOfSegment)
                         chartNum = ds_operation.Tables[0].Rows.Count % 2;
                     bool slideIsNull = true;
                     for (int k = 0; k < chartNum; k++)
                     {
                         ChartSpace objSpace = new ChartSpace();
                         ChChart objChart = objSpace.Charts.Add(0);
                         objChart.Type = ChartChartTypeEnum.chChartTypeColumnClustered;
                         DataSet ds = getSlide_Nineteen(alSegment[i].ToString(), ds_operation.Tables[0].Rows[count][0].ToString());
                         DataSet dsAdd = getSlide_NineteenAdd(alSegment[i].ToString(), ds_operation.Tables[0].Rows[count][0].ToString());
                         objChart.HasTitle = true;
                         objChart.HasLegend = true;
                         objChart.Title.Font.Size = 12;
                         objChart.Title.Font.Bold = true;
                         objChart.Title.Caption = ds_operation.Tables[0].Rows[count][1].ToString();
                         count++;
                         if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                         {
                             slideIsNull = false;
                             string dimCategories = "";
                             for (int l = 1; l < ds.Tables[0].Columns.Count; l++)
                             {
                                 dimCategories += ds.Tables[0].Columns[l].ColumnName + "\t";
                             }
                             dimCategories = dimCategories.Substring(0, dimCategories.Length - 1);
                             for (int r = 0; r < ds.Tables[0].Rows.Count; r++)
                             {
                                 string dimValues = "";
                                 for (int n = 1; n < ds.Tables[0].Columns.Count; n++)
                                 {
                                     var num1 = ds.Tables[0].Rows[r][n];
                                     if (dsAdd != null && dsAdd.Tables.Count > 0 && dsAdd.Tables[0].Rows.Count > 0)
                                     {
                                         var num2 = dsAdd.Tables[0].Rows[r][n];
                                         dimValues += Convert.ToDouble(num1 is DBNull ? 0 : num1) + Convert.ToDouble(num2 is DBNull ? 0 : num2) + "\t";
                                     }
                                     else
                                         dimValues += Convert.ToDouble(num1 is DBNull ? 0 : num1) + "\t";
                                 }
                                 dimValues = dimValues.Substring(0, dimValues.Length - 1);
                                 objChart.SeriesCollection.Add(r);
                                 objChart.SeriesCollection[r].Caption = ds.Tables[0].Rows[r][0].ToString();
                                 objSpace.Charts[0].SeriesCollection[r].SetData(ChartDimensionsEnum.chDimCategories,
                                        +(int)ChartSpecialDataSourcesEnum.chDataLiteral, dimCategories);
                                 objSpace.Charts[0].SeriesCollection[r].SetData(ChartDimensionsEnum.chDimValues,
                                         +(int)ChartSpecialDataSourcesEnum.chDataLiteral, dimValues);
                             }
                         }
                         objSpace.ExportPicture(picturePath, "GIF", 320, 160);
                         picturePath = pictureFolderPath + pictureName;
                         InsertPicture(slide, picturePath, x, 80, 300, 180);

                         int rowCount = ds.Tables[0].Rows.Count;
                         int columnCount = ds.Tables[0].Columns.Count;
                         PowerPoint.Shape shape = slide.Shapes.AddTable(rowCount + 1, columnCount, 385, 300, 700, 200);

                         for (int s = 0; s < rowCount + 1; s++)
                         {
                             for (int t = 0; t < columnCount; t++)
                             {
                                 PowerPoint.TextFrame textF = shape.Table.Cell(s + 1, t + 1).Shape.TextFrame;
                                 textF.HorizontalAnchor = Microsoft.Office.Core.MsoHorizontalAnchor.msoAnchorCenter;
                                 PowerPoint.TextRange textR = textF.TextRange;
                                 textR.Font.Bold = MsoTriState.msoTrue;
                                 textR.Font.Size = 4;
                                 if (s == 0)
                                 {
                                     textR.Text = ds.Tables[0].Columns[t].ColumnName;
                                 }
                                 else
                                 {
                                     if (t == 0)
                                     {
                                         var num1 = ds.Tables[0].Rows[s - 1][t];
                                         textR.Text = num1.ToString();
                                     }
                                     else
                                     {
                                         var num1 = ds.Tables[0].Rows[s - 1][t];
                                         if (dsAdd != null && dsAdd.Tables.Count > 0 && dsAdd.Tables[0].Rows.Count > 0)
                                         {
                                             var num2 = dsAdd.Tables[0].Rows[s - 1][t];
                                             textR.Text = (Convert.ToDouble(num1 is DBNull ? 0 : num1) + Convert.ToDouble(num2 is DBNull ? 0 : num2)).ToString();
                                         }
                                         else
                                             textR.Text = (Convert.ToDouble(num1 is DBNull ? 0 : num1)).ToString();
                                     }
                                 }
                             }
                         }
                         shape.Export(picturePath, Microsoft.Office.Interop.PowerPoint.PpShapeFormat.ppShapeFormatGIF, 480, 240, Microsoft.Office.Interop.PowerPoint.PpExportMode.ppScaleXY);
                         shape.Delete();
                         InsertPicture(slide, picturePath, x1, 340, 240, 120);
                         x = 380;
                         x1 = 410;
                     }
                     if (slideIsNull == true)
                         --slideIndex;
                 }
             }
         }
     }
     protected DataSet getSlide_Nineteen(string abbr, string operationID)
     {
         string query_ExistOrNot = " SELECT Count(SegmentID) FROM [Bookings],[Segment]"
                                 + " WHERE [Bookings].SegmentID=[Segment].ID"
                                 + " AND Segment.Deleted=0 "
                                 + " AND [Segment].Abbr='" + abbr + "'";
         DataSet ds_ExistOrNot = helper.GetDataSet(query_ExistOrNot);
         if (ds_ExistOrNot.Tables.Count > 0 && ds_ExistOrNot.Tables[0].Rows.Count > 0 && (int)ds_ExistOrNot.Tables[0].Rows[0][0] != 0)
         {
             string query_product = " SELECT [Product].ID,[Product].Abbr FROM [Segment],[Product],[Segment_Product]"
                                 + " WHERE [Segment].Abbr='" + abbr + "'"
                                 + " AND Segment.Deleted=0 "
                                 + " AND Product.Deleted=0 "
                                 + " AND Segment_Product.Deleted=0 "
                                 + " AND [Segment].ID=[Segment_Product].SegmentID"
                                 + " AND [Segment_Product].ProductID=[Product].ID";
             DataSet ds_product = helper.GetDataSet(query_product);
             string query_yearA = "SELECT '" + meeting.getyear().Trim() + " A' AS ' '";
             string query_yearB = "SELECT '" + meeting.getyear() + " B' AS ' '";
             string query_nextyearB = "SELECT '" + meeting.getnextyear().Trim() + " B' AS ' '";
             string query_nextyearF = "SELECT '" + meeting.getnextyear().Trim() + " F' AS ' '";
             if (ds_product.Tables.Count > 0 && ds_product.Tables[0].Rows.Count > 0)
             {
                 string st = "";
                 for (int i = 0; i < ds_product.Tables[0].Rows.Count; i++)
                 {
                     string str1 = ds_product.Tables[0].Rows[i][0].ToString();
                     string str2 = ds_product.Tables[0].Rows[i][1].ToString();
                     query_yearA += ",ROUND(0/1000,0) AS '" + str2 + "'";
                     st += ",ROUND(SUM(CASE WHEN [Bookings].ProductID='" + str1
                                + "' THEN [Bookings].Amount*[Currency_Exchange].Rate2"
                                + " ELSE 0 END)/1000,0) AS '" + str2 + "'";
                 }
                 query_yearA += " FROM [Bookings]";
                 query_yearB += st + " FROM [Bookings],[Segment],[SalesOrg],[SalesOrg_User],[Currency_Exchange]"
                                 + " WHERE [Bookings].DeliverY='" + meeting.getyear().Substring(2, 2).Trim() + "'"
                                 + " AND ([Bookings].BookingY='" + meeting.getyear().Substring(2, 2).Trim() + "'"
                                 + " OR [Bookings].BookingY='" + meeting.getnextyear().Substring(2, 2).Trim() + "')"
                                 + " AND YEAR([Bookings].TimeFlag)='" + meeting.getpreyear().Trim() + "'"
                                 + " AND MONTH([Bookings].TimeFlag)='03'"
                                 + " AND Segment.Deleted=0 "
                                 + " AND SalesOrg.Deleted=0 "
                                 + " AND SalesOrg_User.Deleted=0 "
                                 + " AND Currency_Exchange.Deleted=0 "
                                 + " AND [Bookings].SegmentID=[Segment].ID"
                                 + " AND [Segment].Abbr='" + abbr + "'"
                                 + " AND [Bookings].OperationID='" + operationID + "'"
                                 + " AND [Bookings].RSMID=[SalesOrg_User].UserID"
                                 + " AND [SalesOrg_User].SalesOrgID=[SalesOrg].ID"
                                 + " AND [SalesOrg].CurrencyID=[Currency_Exchange].CurrencyID"
                                 + " AND [Currency_Exchange].TimeFlag=[Bookings].TimeFlag";
                 query_nextyearB += st + " FROM [Bookings],[Segment],[SalesOrg],[SalesOrg_User],[Currency_Exchange]"
                                 + " WHERE [Bookings].DeliverY='" + meeting.getnextyear().Substring(2, 2).Trim() + "'"
                                 + " AND ([Bookings].BookingY='" + meeting.getnextyear().Substring(2, 2).Trim() + "'"
                                 + " OR [Bookings].BookingY='" + yearAfterNext.Substring(2, 2).Trim() + "')"
                                 + " AND YEAR([Bookings].TimeFlag)='" + meeting.getyear().Trim() + "'"
                                 + " AND MONTH([Bookings].TimeFlag)='03'"
                                 + " AND Segment.Deleted=0 "
                                 + " AND SalesOrg.Deleted=0 "
                                 + " AND SalesOrg_User.Deleted=0 "
                                 + " AND Currency_Exchange.Deleted=0 "
                                 + " AND [Bookings].SegmentID=[Segment].ID"
                                 + " AND [Segment].Abbr='" + abbr + "'"
                                 + " AND [Bookings].OperationID='" + operationID + "'"
                                 + " AND [Bookings].RSMID=[SalesOrg_User].UserID"
                                 + " AND [SalesOrg_User].SalesOrgID=[SalesOrg].ID"
                                 + " AND [SalesOrg].CurrencyID=[Currency_Exchange].CurrencyID"
                                 + " AND [Currency_Exchange].TimeFlag=[Bookings].TimeFlag";
                 query_nextyearF += st + " FROM [Bookings],[Segment],[SalesOrg],[SalesOrg_User],[Currency_Exchange]"
                                 + " WHERE [Bookings].DeliverY='" + meeting.getnextyear().Substring(2, 2).Trim() + "'"
                                 + " AND ([Bookings].BookingY='" + meeting.getnextyear().Substring(2, 2).Trim() + "'"
                                 + " OR [Bookings].BookingY='" + yearAfterNext.Substring(2, 2).Trim() + "')"
                                 + " AND YEAR([Bookings].TimeFlag)='" + meeting.getyear().Trim() + "'"
                                 + " AND MONTH([Bookings].TimeFlag)='" + month.Trim() + "'"
                                 + " AND [Bookings].SegmentID=[Segment].ID"
                                 + " AND [Segment].Abbr='" + abbr + "'"
                                 + " AND Segment.Deleted=0 "
                                 + " AND SalesOrg.Deleted=0 "
                                 + " AND SalesOrg_User.Deleted=0 "
                                 + " AND Currency_Exchange.Deleted=0 "
                                 + " AND [Bookings].OperationID='" + operationID + "'"
                                 + " AND [Bookings].RSMID=[SalesOrg_User].UserID"
                                 + " AND [SalesOrg_User].SalesOrgID=[SalesOrg].ID"
                                 + " AND [SalesOrg].CurrencyID=[Currency_Exchange].CurrencyID"
                                 + " AND [Currency_Exchange].TimeFlag=[Bookings].TimeFlag";
                 query_yearA += " UNION " + query_yearB + " UNION " + query_nextyearB + " UNION " + query_nextyearF;
                 DataSet ds = helper.GetDataSet(query_yearA);
                 return ds;
             }
             else
                 return null;
         }
         else
             return null;
     }
     protected DataSet getSlide_NineteenAdd(string abbr, string operationID)
     {
         string query_ExistOrNot = " SELECT Count(SegmentID) FROM [ActualSalesandBL],[Segment]"
                                 + " WHERE [ActualSalesandBL].SegmentID=[Segment].ID"
                                 + " AND Segment.Deleted=0 "
                                 + " AND [Segment].Abbr='" + abbr + "'";
         DataSet ds_ExistOrNot = helper.GetDataSet(query_ExistOrNot);
         if (ds_ExistOrNot.Tables.Count > 0 && ds_ExistOrNot.Tables[0].Rows.Count > 0 && (int)ds_ExistOrNot.Tables[0].Rows[0][0] != 0)
         {
             string query_product = " SELECT [Product].ID,[Product].Abbr FROM [Segment],[Product],[Segment_Product]"
                                  + " WHERE [Segment].Abbr='" + abbr + "'"
                                  + " AND Segment.Deleted=0 "
                                  + " AND Product.Deleted=0 "
                                  + " AND Segment_Product.Deleted=0 "
                                  + " AND [Segment].ID=[Segment_Product].SegmentID"
                                  + " AND [Segment_Product].ProductID=[Product].ID";
             DataSet ds_product = helper.GetDataSet(query_product);
             string query_yearA = "SELECT '" + meeting.getyear().Trim() + " A' AS ' '";
             string query_yearB = "SELECT '" + meeting.getyear() + " B' AS ' '";
             string query_nextyearB = "SELECT '" + meeting.getnextyear().Trim() + " B' AS ' '";
             string query_nextyearF = "SELECT '" + meeting.getnextyear().Trim() + " F' AS ' '";
             if (ds_product.Tables.Count > 0 && ds_product.Tables[0].Rows.Count > 0)
             {
                 string str = "";
                 for (int i = 0; i < ds_product.Tables[0].Rows.Count; i++)
                 {
                     string str1 = ds_product.Tables[0].Rows[i][0].ToString();
                     string str2 = ds_product.Tables[0].Rows[i][1].ToString();
                     query_yearA += ",ROUND(SUM(CASE WHEN [ActualSalesandBL].ProductID='" + str1 + "'"
                                 + " THEN [ActualSalesandBL].Backlog*[Currency_Exchange].Rate1"
                                 + " ELSE 0 END)/1000,0) AS '" + str2 + "'";
                     str += ",ROUND(SUM(CASE WHEN [ActualSalesandBL].ProductID='" + str1 + "'"
                                 + " THEN [ActualSalesandBL].Backlog*[Currency_Exchange].Rate2"
                                 + " ELSE 0 END)/1000,0) AS '" + str2 + "'";
                 }
                 query_yearA += " FROM [ActualSalesandBL],[Segment],[SalesOrg],[Currency_Exchange]"
                                 + " WHERE [ActualSalesandBL].BacklogY='" + meeting.getyear().Substring(2, 2).Trim() + "'"
                                 + " AND YEAR([ActualSalesandBL].TimeFlag)='" + meeting.getyear().Trim() + "'"
                                 + " AND MONTH([ActualSalesandBL].TimeFlag)='" + month.Trim() + "'"
                                 + " AND Segment.Deleted=0 "
                                 + " AND SalesOrg.Deleted=0 "
                                 + " AND Currency_Exchange.Deleted=0 "
                                 + " AND [ActualSalesandBL].SegmentID=[Segment].ID"
                                 + " AND [Segment].Abbr='" + abbr + "'"
                                 + " AND [ActualSalesandBL].OperationID='" + operationID + "'"
                                 + " AND [ActualSalesandBL].SalesOrgID=[SalesOrg].ID"
                                 + " AND [SalesOrg].CurrencyID=[Currency_Exchange].CurrencyID"
                                 + " AND [Currency_Exchange].TimeFlag=[ActualSalesandBL].TimeFlag";
                 query_yearB += str + " FROM [ActualSalesandBL],[Segment],[SalesOrg],[Currency_Exchange]"
                                 + " WHERE [ActualSalesandBL].BacklogY='" + meeting.getyear().Substring(2, 2).Trim() + "'"
                                 + " AND YEAR([ActualSalesandBL].TimeFlag)='" + meeting.getpreyear().Trim() + "'"
                                 + " AND MONTH([ActualSalesandBL].TimeFlag)='03'"
                                 + " AND Segment.Deleted=0 "
                                 + " AND SalesOrg.Deleted=0 "
                                 + " AND Currency_Exchange.Deleted=0 "
                                 + " AND [ActualSalesandBL].SegmentID=[Segment].ID"
                                 + " AND [Segment].Abbr='" + abbr + "'"
                                 + " AND [ActualSalesandBL].OperationID='" + operationID + "'"
                                 + " AND [ActualSalesandBL].SalesOrgID=[SalesOrg].ID"
                                 + " AND [SalesOrg].CurrencyID=[Currency_Exchange].CurrencyID"
                                 + " AND [Currency_Exchange].TimeFlag=[ActualSalesandBL].TimeFlag";
                 query_nextyearB += str + " FROM [ActualSalesandBL],[Segment],[SalesOrg],[Currency_Exchange]"
                                 + " WHERE [ActualSalesandBL].BacklogY='" + meeting.getnextyear().Substring(2, 2).Trim() + "'"
                                 + " AND YEAR([ActualSalesandBL].TimeFlag)='" + meeting.getyear().Trim() + "'"
                                 + " AND MONTH([ActualSalesandBL].TimeFlag)='03'"
                                 + " AND Segment.Deleted=0 "
                                 + " AND SalesOrg.Deleted=0 "
                                 + " AND Currency_Exchange.Deleted=0 "
                                 + " AND [ActualSalesandBL].SegmentID=[Segment].ID"
                                 + " AND [Segment].Abbr='" + abbr + "'"
                                 + " AND [ActualSalesandBL].OperationID='" + operationID + "'"
                                 + " AND [ActualSalesandBL].SalesOrgID=[SalesOrg].ID"
                                 + " AND [SalesOrg].CurrencyID=[Currency_Exchange].CurrencyID"
                                 + " AND [Currency_Exchange].TimeFlag=[ActualSalesandBL].TimeFlag";
                 query_nextyearF += str + " FROM [ActualSalesandBL],[Segment],[SalesOrg],[Currency_Exchange]"
                                 + " WHERE [ActualSalesandBL].BacklogY='" + meeting.getnextyear().Substring(2, 2).Trim() + "'"
                                 + " AND YEAR([ActualSalesandBL].TimeFlag)='" + meeting.getyear().Trim() + "'"
                                 + " AND MONTH([ActualSalesandBL].TimeFlag)='" + month.Trim() + "'"
                                 + " AND Segment.Deleted=0 "
                                 + " AND SalesOrg.Deleted=0 "
                                 + " AND Currency_Exchange.Deleted=0 "
                                 + " AND [ActualSalesandBL].SegmentID=[Segment].ID"
                                 + " AND [Segment].Abbr='" + abbr + "'"
                                 + " AND [ActualSalesandBL].OperationID='" + operationID + "'"
                                 + " AND [ActualSalesandBL].SalesOrgID=[SalesOrg].ID"
                                 + " AND [SalesOrg].CurrencyID=[Currency_Exchange].CurrencyID"
                                 + " AND [Currency_Exchange].TimeFlag=[ActualSalesandBL].TimeFlag";
                 query_yearA += " UNION " + query_yearB + " UNION " + query_nextyearB + " UNION " + query_nextyearF;
                 DataSet ds = helper.GetDataSet(query_yearA);
                 return ds;
             }
             else
                 return null;
         }
         else
             return null;
     }
     protected DataSet getOperation(string abbr)
     {
         string query = "SELECT [Operation].ID,[Operation].AbbrL"
                      + " FROM [Bookings],[Operation],[Segment]"
                      + " WHERE  [Operation].ID=[Bookings].OperationID"
                      + " AND [Bookings].SegmentID=[Segment].ID"
                      + " AND [Segment].Abbr='" + abbr + "'"
                      + " AND Operation.Deleted=0 "
                      + " AND Segment.Deleted=0 "
                      + " GROUP BY [Operation].AbbrL,[Operation].ID"
                      + " ORDER BY [Operation].ID";
         DataSet ds = helper.GetDataSet(query);
         return ds;
     }
     //------------------------------------------------------------
     //protected void DrawSlide_TwentySeven(PowerPoint.Slide slide)
     //{
     //    UniformLayout(slide, "2009  & 2010 FCST Booking by Sales Organization");
     //}
     //------------------------------------------------------------
     protected void DrawSlide_TwentySeven()
     {
         PowerPoint.Slide slide;
         int num = segmentNumber / 2;
         int count = 0;

         for (int i = 0; i < segmentNumber / 2 + (segmentNumber % 2 == 0 ? 0 : 1); i++)
         {
             bool slideIsNull = true;
             int chartNum = 2;
             if (i == num)
                 chartNum = segmentNumber / 2;
             slides.Add(++slideIndex, Microsoft.Office.Interop.PowerPoint.PpSlideLayout.ppLayoutTitleOnly);
             slide = slides[slideIndex];
             UniformLayout(slide, "2010 Bookings by Sales Organization 2010 vs 2011");

             for (int j = 0; j < chartNum; j++)
             {

                 DataSet ds = getSlide_TwentySeven(alSegment[count].ToString());
                 DataSet dsAdd = getSlide_TwentySevenAdd(alSegment[count].ToString());
                 count++;
                 float x = 80;
                 //float x1 = 100;
                 if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                 {
                     slideIsNull = false;
                     ChartSpace objSpace = new ChartSpace();
                     ChChart objChart = objSpace.Charts.Add(0);
                     objChart.HasTitle = true;
                     objChart.HasLegend = true;
                     objChart.Title.Font.Size = 12;
                     objChart.Title.Font.Bold = true;
                     objChart.Title.Caption = alSegment[count - 1].ToString();

                     objChart.Type = ChartChartTypeEnum.chChartTypeColumnClustered;
                     for (int k = 0; k < ds.Tables[0].Rows.Count; k++)
                     {
                         string dimCategories = "";
                         string dimValues = "";
                         for (int m = 0; m < alSalesOrg.Count; m++)
                         {
                             dimCategories += alSalesOrg[m].ToString() + "\t";
                         }
                         for (int s = 1; s < ds.Tables[0].Columns.Count; s++)
                         {
                             var num1 = ds.Tables[0].Rows[k][s];
                             var num2 = dsAdd.Tables[0].Rows[k][s];
                             dimValues += Convert.ToDouble(num1 is DBNull ? 0 : num1) + Convert.ToDouble(num2 is DBNull ? 0 : num2) + "\t";
                         }
                         dimCategories = dimCategories.Substring(0, dimCategories.Length - 1);
                         dimValues = dimValues.Substring(0, dimValues.Length - 1);
                         objChart.SeriesCollection.Add(k);
                         objChart.SeriesCollection[k].Caption = ds.Tables[0].Rows[k][0].ToString();
                         objSpace.Charts[0].SeriesCollection[k].SetData(ChartDimensionsEnum.chDimCategories,
                                +(int)ChartSpecialDataSourcesEnum.chDataLiteral, dimCategories);
                         objSpace.Charts[0].SeriesCollection[k].SetData(ChartDimensionsEnum.chDimValues,
                                 +(int)ChartSpecialDataSourcesEnum.chDataLiteral, dimValues);
                     }
                     int w = ds.Tables[0].Columns.Count * 40;
                     int h = ds.Tables[0].Columns.Count * 18;
                     objSpace.ExportPicture(picturePath, "GIF", w, 160);
                     picturePath = pictureFolderPath + pictureName;
                     InsertPicture(slide, picturePath, x, 80, w, 180);

                     int rowCount = ds.Tables[0].Rows.Count;
                     int columnCount = ds.Tables[0].Columns.Count;
                     PowerPoint.Shape shape = slide.Shapes.AddTable(rowCount + 1, columnCount, 385, 300, 700, 200);

                     for (int s = 0; s < rowCount + 1; s++)
                     {
                         for (int t = 0; t < columnCount; t++)
                         {
                             PowerPoint.TextFrame textF = shape.Table.Cell(s + 1, t + 1).Shape.TextFrame;
                             textF.HorizontalAnchor = Microsoft.Office.Core.MsoHorizontalAnchor.msoAnchorCenter;
                             PowerPoint.TextRange textR = textF.TextRange;
                             textR.Font.Bold = MsoTriState.msoTrue;
                             textR.Font.Size = 4;
                             if (s == 0)
                             {
                                 textR.Text = ds.Tables[0].Columns[t].ColumnName;
                             }
                             else
                             {
                                 if (t == 0)
                                 {
                                     var num1 = ds.Tables[0].Rows[s - 1][t];
                                     textR.Text = num1.ToString();
                                 }
                                 else
                                 {
                                     var num1 = ds.Tables[0].Rows[s - 1][t];
                                     var num2 = dsAdd.Tables[0].Rows[s - 1][t];
                                     textR.Text = (Convert.ToDouble(num1 is DBNull ? 0 : num1) + Convert.ToDouble(num2 is DBNull ? 0 : num2)).ToString();
                                 }
                             }
                         }
                     }
                     shape.Export(picturePath, Microsoft.Office.Interop.PowerPoint.PpShapeFormat.ppShapeFormatGIF, w, h, Microsoft.Office.Interop.PowerPoint.PpExportMode.ppScaleXY);
                     shape.Delete();
                     InsertPicture(slide, picturePath, x + 20, 340, w - 40, 120);
                     x = 380;
                 }


             }
             if (slideIsNull == true)
                 --slideIndex;
         }
     }
     protected DataSet getSlide_TwentySeven(string abbr)
     {
         string query_ExistOrNot = " SELECT Count(SegmentID) FROM [ActualSalesandBL],[Segment]"
                                 + " WHERE [ActualSalesandBL].SegmentID=[Segment].ID"
                                 + " AND Segment.Deleted=0 "
                                 + " AND [Segment].Abbr='" + abbr + "'";
         DataSet ds_ExistOrNot = helper.GetDataSet(query_ExistOrNot);
         if (ds_ExistOrNot.Tables.Count > 0 && ds_ExistOrNot.Tables[0].Rows.Count > 0 && (int)ds_ExistOrNot.Tables[0].Rows[0][0] != 0)
         {
             string query_getSaleOrg = "SELECT [SalesOrg].Abbr FROM [SalesOrg],[SalesOrg_Segment],[Segment] "
                                     + "WHERE [SalesOrg].ID=[SalesOrg_Segment].SalesOrgID "
                                     + " AND SalesOrg.Deleted=0 "
                                     + " AND SalesOrg_Segment.Deleted=0 "
                                     + " AND Segment.Deleted=0 "
                                     + "AND [SalesOrg_Segment].SegmentID=[Segment].ID "
                                     + "AND [Segment].Abbr='" + abbr.Trim() + "'";
             DataSet ds_SalesOrg = helper.GetDataSet(query_getSaleOrg);
             string query_yearA = "SELECT '" + meeting.getyear().Trim() + " A' AS ' '";
             string query_yearB = "SELECT '" + meeting.getyear() + " B' AS ' '";
             string query_nextyearB = "SELECT '" + meeting.getnextyear().Trim() + " B' AS ' '";
             string query_nextyearF = "SELECT '" + meeting.getnextyear().Trim() + " F' AS ' '";
             if (ds_SalesOrg.Tables.Count > 0 && ds_SalesOrg.Tables[0].Rows.Count > 0)
             {
                 alSalesOrg.Clear();
                 for (int i = 0; i < ds_SalesOrg.Tables[0].Rows.Count; i++)
                 {
                     string str = ds_SalesOrg.Tables[0].Rows[i][0].ToString().Trim();
                     alSalesOrg.Add(str);
                     query_yearA += ",ROUND(0/1000,0) AS '" + str + "'";
                     query_yearB += ",ROUND(SUM(CASE WHEN [SalesOrg].Abbr='" + str
                                    + "' THEN [Bookings].Amount*[Currency_Exchange].Rate2"
                                    + " ELSE 0 END)/1000,0) AS '" + str + "'";
                     query_nextyearB += ",ROUND(SUM(CASE WHEN [SalesOrg].Abbr='" + str
                                    + "' THEN [Bookings].Amount*[Currency_Exchange].Rate2"
                                    + " ELSE 0 END)/1000,0) AS '" + str + "'";
                     query_nextyearF += ",ROUND(SUM(CASE WHEN [SalesOrg].Abbr='" + str
                                    + "' THEN [Bookings].Amount*[Currency_Exchange].Rate2"
                                    + " ELSE 0 END)/1000,0) AS '" + str + "'";
                 }
                 query_yearA += " FROM [Bookings]";
                 query_yearB += " FROM [Bookings],[Segment],[SalesOrg],[SalesOrg_User],[Currency_Exchange]"
                                 + " WHERE [Bookings].DeliverY='" + meeting.getyear().Substring(2, 2).Trim() + "'"
                                 + " AND ([Bookings].BookingY='" + meeting.getyear().Substring(2, 2).Trim() + "'"
                                 + " OR [Bookings].BookingY='" + meeting.getnextyear().Substring(2, 2).Trim() + "')"
                                 + " AND YEAR([Bookings].TimeFlag)='" + meeting.getpreyear().Trim() + "'"
                                 + " AND MONTH([Bookings].TimeFlag)='03'"
                                 + " AND Segment.Deleted=0 "
                                 + " AND SalesOrg.Deleted=0 "
                                 + " AND SalesOrg_Segment.Deleted=0 "
                                 + " AND Currency_Exchange.Deleted=0 "
                                 + " AND [Bookings].SegmentID=[Segment].ID"
                                 + " AND [Segment].Abbr='" + abbr + "'"
                                 + " AND [Bookings].RSMID=[SalesOrg_User].UserID"
                                 + " AND [SalesOrg_User].SalesOrgID=[SalesOrg].ID"
                                 + " AND [SalesOrg].CurrencyID=[Currency_Exchange].CurrencyID"
                                 + " AND [Currency_Exchange].TimeFlag=[Bookings].TimeFlag";
                 query_nextyearB += " FROM [Bookings],[Segment],[SalesOrg],[SalesOrg_User],[Currency_Exchange]"
                                 + " WHERE [Bookings].DeliverY='" + meeting.getnextyear().Substring(2, 2).Trim() + "'"
                                 + " AND ([Bookings].BookingY='" + meeting.getnextyear().Substring(2, 2).Trim() + "'"
                                 + " OR [Bookings].BookingY='" + yearAfterNext.Substring(2, 2).Trim() + "')"
                                 + " AND YEAR([Bookings].TimeFlag)='" + meeting.getyear().Trim() + "'"
                                 + " AND MONTH([Bookings].TimeFlag)='03'"
                                 + " AND Segment.Deleted=0 "
                                 + " AND SalesOrg.Deleted=0 "
                                 + " AND SalesOrg_Segment.Deleted=0 "
                                 + " AND Currency_Exchange.Deleted=0 "
                                 + " AND [Bookings].SegmentID=[Segment].ID"
                                 + " AND [Segment].Abbr='" + abbr + "'"
                                 + " AND [Bookings].RSMID=[SalesOrg_User].UserID"
                                 + " AND [SalesOrg_User].SalesOrgID=[SalesOrg].ID"
                                 + " AND [SalesOrg].CurrencyID=[Currency_Exchange].CurrencyID"
                                 + " AND [Currency_Exchange].TimeFlag=[Bookings].TimeFlag";
                 query_nextyearF += " FROM [Bookings],[Segment],[SalesOrg],[SalesOrg_User],[Currency_Exchange]"
                                 + " WHERE [Bookings].DeliverY='" + meeting.getnextyear().Substring(2, 2).Trim() + "'"
                                 + " AND ([Bookings].BookingY='" + meeting.getnextyear().Substring(2, 2).Trim() + "'"
                                 + " OR [Bookings].BookingY='" + yearAfterNext.Substring(2, 2).Trim() + "')"
                                 + " AND YEAR([Bookings].TimeFlag)='" + meeting.getyear().Trim() + "'"
                                 + " AND MONTH([Bookings].TimeFlag)='" + month.Trim() + "'"
                                 + " AND Segment.Deleted=0 "
                                 + " AND SalesOrg.Deleted=0 "
                                 + " AND SalesOrg_Segment.Deleted=0 "
                                 + " AND Currency_Exchange.Deleted=0 "
                                 + " AND [Bookings].SegmentID=[Segment].ID"
                                 + " AND [Segment].Abbr='" + abbr + "'"
                                 + " AND [Bookings].RSMID=[SalesOrg_User].UserID"
                                 + " AND [SalesOrg_User].SalesOrgID=[SalesOrg].ID"
                                 + " AND [SalesOrg].CurrencyID=[Currency_Exchange].CurrencyID"
                                 + " AND [Currency_Exchange].TimeFlag=[Bookings].TimeFlag";
                 query_yearA += " UNION " + query_yearB + " UNION " + query_nextyearB + " UNION " + query_nextyearF;
                 DataSet ds = helper.GetDataSet(query_yearA);
                 return ds;
             }
             else
                 return null;
         }
         else
             return null;
     }
     protected DataSet getSlide_TwentySevenAdd(string abbr)
     {
         string query_ExistOrNot = " SELECT Count(SegmentID) FROM [ActualSalesandBL],[Segment]"
                                 + " WHERE [ActualSalesandBL].SegmentID=[Segment].ID"
                                 + " AND Segment.Deleted=0 "
                                 + " AND [Segment].Abbr='" + abbr + "'";
         DataSet ds_ExistOrNot = helper.GetDataSet(query_ExistOrNot);
         if (ds_ExistOrNot.Tables.Count > 0 && ds_ExistOrNot.Tables[0].Rows.Count > 0 && (int)ds_ExistOrNot.Tables[0].Rows[0][0] != 0)
         {
             string query_getSaleOrg = "SELECT [SalesOrg].Abbr FROM [SalesOrg],[SalesOrg_Segment],[Segment] "
                                     + "WHERE [SalesOrg].ID=[SalesOrg_Segment].SalesOrgID "
                                     + " AND SalesOrg.Deleted=0 "
                                     + " AND SalesOrg_Segment.Deleted=0 "
                                     + " AND Segment.Deleted=0 "
                                     + "AND [SalesOrg_Segment].SegmentID=[Segment].ID "
                                     + "AND [Segment].Abbr='" + abbr.Trim() + "'";
             DataSet ds_SalesOrg = helper.GetDataSet(query_getSaleOrg);
             string query_yearA = "SELECT '" + meeting.getyear().Trim() + " A' AS ' '";
             string query_yearB = "SELECT '" + meeting.getyear() + " B' AS ' '";
             string query_nextyearB = "SELECT '" + meeting.getnextyear().Trim() + " B' AS ' '";
             string query_nextyearF = "SELECT '" + meeting.getnextyear().Trim() + " F' AS ' '";
             if (ds_SalesOrg.Tables.Count > 0 && ds_SalesOrg.Tables[0].Rows.Count > 0)
             {
                 alSalesOrg.Clear();
                 for (int i = 0; i < ds_SalesOrg.Tables[0].Rows.Count; i++)
                 {
                     string str = ds_SalesOrg.Tables[0].Rows[i][0].ToString().Trim();
                     alSalesOrg.Add(str);
                     query_yearA += ",ROUND(SUM(CASE WHEN [SalesOrg].Abbr='" + str + "'"
                                     + " THEN [ActualSalesandBL].Backlog*[Currency_Exchange].Rate1"
                                     + " ELSE 0 END)/1000,0) AS '" + str + "'";
                     query_yearB += ",ROUND(SUM(CASE WHEN [SalesOrg].Abbr='" + str + "'"
                                     + " THEN [ActualSalesandBL].Backlog*[Currency_Exchange].Rate1"
                                     + " ELSE 0 END)/1000,0) AS '" + str + "'";
                     query_nextyearB += ",ROUND(SUM(CASE WHEN [SalesOrg].Abbr='" + str + "'"
                                     + " THEN [ActualSalesandBL].Backlog*[Currency_Exchange].Rate1"
                                     + " ELSE 0 END)/1000,0) AS '" + str + "'";
                     query_nextyearF += ",ROUND(SUM(CASE WHEN [SalesOrg].Abbr='" + str + "'"
                                     + " THEN [ActualSalesandBL].Backlog*[Currency_Exchange].Rate1"
                                     + " ELSE 0 END)/1000,0) AS '" + str + "'";
                 }
                 query_yearA += " FROM [ActualSalesandBL],[Segment],[SalesOrg],[Currency_Exchange]"
                                     + " WHERE [ActualSalesandBL].BacklogY='" + meeting.getyear().Substring(2, 2).Trim() + "'"
                                     + " AND YEAR([ActualSalesandBL].TimeFlag)='" + meeting.getyear().Trim() + "'"
                                     + " AND MONTH([ActualSalesandBL].TimeFlag)='" + month.Trim() + "'"
                                     + " AND Segment.Deleted=0 "
                                     + " AND SalesOrg.Deleted=0 "
                                     + " AND Currency_Exchange.Deleted=0 "
                                     + " AND [ActualSalesandBL].SegmentID=[Segment].ID"
                                     + " AND [Segment].Abbr='" + abbr + "'"
                                     + " AND [ActualSalesandBL].SalesOrgID=[SalesOrg].ID"
                                     + " AND [SalesOrg].CurrencyID=[Currency_Exchange].CurrencyID"
                                     + " AND [Currency_Exchange].TimeFlag=[ActualSalesandBL].TimeFlag";
                 query_yearB += " FROM [ActualSalesandBL],[Segment],[SalesOrg],[Currency_Exchange]"
                                 + " WHERE [ActualSalesandBL].BacklogY='" + meeting.getyear().Substring(2, 2).Trim() + "'"
                                 + " AND YEAR([ActualSalesandBL].TimeFlag)='" + meeting.getpreyear().Trim() + "'"
                                 + " AND MONTH([ActualSalesandBL].TimeFlag)='03'"
                                 + " AND Segment.Deleted=0 "
                                 + " AND SalesOrg.Deleted=0 "
                                 + " AND Currency_Exchange.Deleted=0 "
                                 + " AND [ActualSalesandBL].SegmentID=[Segment].ID"
                                 + " AND [Segment].Abbr='" + abbr + "'"
                                 + " AND [ActualSalesandBL].SalesOrgID=[SalesOrg].ID"
                                 + " AND [SalesOrg].CurrencyID=[Currency_Exchange].CurrencyID"
                                 + " AND [Currency_Exchange].TimeFlag=[ActualSalesandBL].TimeFlag";
                 query_nextyearB += " FROM [ActualSalesandBL],[Segment],[SalesOrg],[Currency_Exchange]"
                                 + " WHERE [ActualSalesandBL].BacklogY='" + meeting.getnextyear().Substring(2, 2).Trim() + "'"
                                 + " AND YEAR([ActualSalesandBL].TimeFlag)='" + meeting.getyear().Trim() + "'"
                                 + " AND MONTH([ActualSalesandBL].TimeFlag)='03'"
                                 + " AND Segment.Deleted=0 "
                                 + " AND SalesOrg.Deleted=0 "
                                 + " AND Currency_Exchange.Deleted=0 "
                                 + " AND [ActualSalesandBL].SegmentID=[Segment].ID"
                                 + " AND [Segment].Abbr='" + abbr + "'"
                                 + " AND [ActualSalesandBL].SalesOrgID=[SalesOrg].ID"
                                 + " AND [SalesOrg].CurrencyID=[Currency_Exchange].CurrencyID"
                                 + " AND [Currency_Exchange].TimeFlag=[ActualSalesandBL].TimeFlag";
                 query_nextyearF += " FROM [ActualSalesandBL],[Segment],[SalesOrg],[Currency_Exchange]"
                                 + " WHERE [ActualSalesandBL].BacklogY='" + meeting.getnextyear().Substring(2, 2).Trim() + "'"
                                 + " AND YEAR([ActualSalesandBL].TimeFlag)='" + meeting.getyear().Trim() + "'"
                                 + " AND MONTH([ActualSalesandBL].TimeFlag)='" + month.Trim() + "'"
                                 + " AND Segment.Deleted=0 "
                                 + " AND SalesOrg.Deleted=0 "
                                 + " AND Currency_Exchange.Deleted=0 "
                                 + " AND [ActualSalesandBL].SegmentID=[Segment].ID"
                                 + " AND [Segment].Abbr='" + abbr + "'"
                                 + " AND [ActualSalesandBL].SalesOrgID=[SalesOrg].ID"
                                 + " AND [SalesOrg].CurrencyID=[Currency_Exchange].CurrencyID"
                                 + " AND [Currency_Exchange].TimeFlag=[ActualSalesandBL].TimeFlag";
                 query_yearA += " UNION " + query_yearB + " UNION " + query_nextyearB + " UNION " + query_nextyearF;
                 DataSet ds = helper.GetDataSet(query_yearA);
                 return ds;
             }
             else
                 return null;
         }
         return null;
     }
     //-----------------------------------------------------------------------
     protected void DrawSlide_TwentyNine()
     {
         for (int i = 0; i < segmentNumber; i++)
         {
             DataSet ds_operation = getOperation(alSegment[i].ToString());
             if (ds_operation.Tables.Count > 0 && ds_operation.Tables[0].Rows.Count > 0)
             {
                 int operationIndex = 0;
                 int slideNum = ds_operation.Tables[0].Rows.Count / 2 + (ds_operation.Tables[0].Rows.Count % 2 == 0 ? 0 : 1);

                 for (int j = 0; j < slideNum; j++)
                 {
                     float x = 60;
                     slides.Add(++slideIndex, Microsoft.Office.Interop.PowerPoint.PpSlideLayout.ppLayoutTitleOnly);
                     PowerPoint.Slide slide = slides[slideIndex];
                     UniformLayout(slide, meeting.getyear().Trim() + " & " + meeting.getnextyear().Trim() + " " + alSegment[i].ToString().Trim() + " PRODUCTS SALES");
                     int chartNum = 2;
                     if ((j == slideNum - 1) && (ds_operation.Tables[0].Rows.Count % 2 != 0))
                         chartNum = 1;
                     for (int k = 0; k < chartNum; k++)
                     {
                         DataSet ds = getSlide_TwentyNine(alSegment[i].ToString(), ds_operation.Tables[0].Rows[operationIndex][0].ToString());
                         DataSet dsAdd = getSlide_TwentyNineAdd(alSegment[i].ToString(), ds_operation.Tables[0].Rows[operationIndex][0].ToString());
                         if (ds != null)
                         {
                             ChartSpace objSpace = new ChartSpace();
                             ChChart objChart = objSpace.Charts.Add(0);
                             objChart.Type = ChartChartTypeEnum.chChartTypeArea;
                             objChart.HasTitle = true;
                             objChart.HasLegend = true;
                             objChart.Title.Font.Size = 12;
                             objChart.Title.Font.Bold = true;
                             objChart.Title.Caption = ds_operation.Tables[0].Rows[operationIndex][1].ToString();
                             operationIndex++;
                             string dimCategories = "";


                             for (int l = 1; l < ds.Tables[0].Columns.Count; l++)
                             {
                                 dimCategories += ds.Tables[0].Columns[l].ColumnName + "\t";
                             }
                             dimCategories = dimCategories.Substring(0, dimCategories.Length - 1);
                             for (int m = 0; m < ds.Tables[0].Rows.Count; m++)
                             {
                                 string dimValues = "";
                                 for (int n = 1; n < ds.Tables[0].Columns.Count; n++)
                                 {
                                     var num1 = ds.Tables[0].Rows[m][n];
                                     if (dsAdd != null)
                                     {
                                         var num2 = dsAdd.Tables[0].Rows[m][n];
                                         dimValues += (Convert.ToDouble(num1 is DBNull ? 0 : num1)
                                                     + Convert.ToDouble(num2 is DBNull ? 0 : num2)).ToString() + "\t";
                                     }
                                     else
                                         dimValues += (num1 is DBNull ? "0\t" : num1.ToString() + "\t");
                                 }
                                 dimValues = dimValues.Substring(0, dimValues.Length - 1);
                                 objChart.SeriesCollection.Add(0);
                                 objChart.SeriesCollection[0].Caption = ds.Tables[0].Rows[m][0].ToString();
                                 objChart.SeriesCollection[0].SetData(ChartDimensionsEnum.chDimCategories,
                                        +(int)ChartSpecialDataSourcesEnum.chDataLiteral, dimCategories);
                                 objChart.SeriesCollection[0].SetData(ChartDimensionsEnum.chDimValues,
                                         +(int)ChartSpecialDataSourcesEnum.chDataLiteral, dimValues);
                             }
                             objSpace.ExportPicture(picturePath, "GIF", 260, 200);
                             picturePath = pictureFolderPath + pictureName;
                             InsertPicture(slide, picturePath, x, 80, 260, 200);

                             int rowCount = ds.Tables[0].Rows.Count;
                             int columnCount = ds.Tables[0].Columns.Count;
                             PowerPoint.Shape shape = slide.Shapes.AddTable(rowCount + 1, columnCount, 385, 300, 700, 200);

                             for (int s = 0; s < rowCount + 1; s++)
                             {
                                 for (int t = 0; t < columnCount; t++)
                                 {
                                     PowerPoint.TextFrame textF = shape.Table.Cell(s + 1, t + 1).Shape.TextFrame;
                                     textF.HorizontalAnchor = Microsoft.Office.Core.MsoHorizontalAnchor.msoAnchorCenter;
                                     PowerPoint.TextRange textR = textF.TextRange;
                                     textR.Font.Bold = MsoTriState.msoTrue;
                                     textR.Font.Size = 4;
                                     if (s == 0)
                                     {
                                         textR.Text = ds.Tables[0].Columns[t].ColumnName;
                                     }
                                     else
                                     {
                                         if (t == 0)
                                         {
                                             var num1 = ds.Tables[0].Rows[s - 1][t];
                                             textR.Text = num1.ToString();
                                         }
                                         else
                                         {
                                             var num1 = ds.Tables[0].Rows[s - 1][t];
                                             if (dsAdd != null)
                                             {
                                                 var num2 = dsAdd.Tables[0].Rows[s - 1][t];
                                                 textR.Text = (Convert.ToDouble(num1 is DBNull ? 0 : num1) + Convert.ToDouble(num2 is DBNull ? 0 : num2)).ToString();
                                             }
                                             else
                                             {
                                                 textR.Text = Convert.ToDouble(num1 is DBNull ? 0 : num1).ToString();
                                             }
                                         }
                                     }
                                 }
                             }
                             shape.Export(picturePath, Microsoft.Office.Interop.PowerPoint.PpShapeFormat.ppShapeFormatGIF, 480, 240, Microsoft.Office.Interop.PowerPoint.PpExportMode.ppScaleXY);
                             shape.Delete();
                             InsertPicture(slide, picturePath, x + 20, 340, 240, 120);
                             x = 380;

                             x = 380;
                         }
                     }
                 }

             }
         }
     }
     protected DataSet getSlide_TwentyNine(string abbr, string operationID)
     {
         string query_EBl = "SELECT 'EB/L' AS ' '";
         string query_SalesAF = "SELECT 'Sales A/F' AS ' '";
         for (int i = 0; i < alYears.Count; i++)
         {
             if (alYears[i].ToString().Trim() == meeting.getnextyear().Substring(2, 2).Trim())
             {
                 query_EBl += ",ROUND(SUM(CASE WHEN [Bookings].BookingY='" + meeting.getyear().Substring(2, 2).Trim() + "'"
                            + " AND [Bookings].DeliverY='" + meeting.getnextyear().Substring(2, 2).Trim() + "'"
                            + " AND YEAR([Bookings].TimeFlag)='" + meeting.getyear().Trim() + "'"
                            + " AND MONTH([Bookings].TimeFlag)='" + month.Trim() + "'"
                            + " THEN Amount*[Currency_Exchange].Rate2 ELSE 0 END)/1000,0) AS '" + meeting.getnextyear().Substring(2, 2).Trim() + "'";
                 query_SalesAF += ",ROUND(SUM(CASE WHEN ([Bookings].BookingY='" + meeting.getyear().Substring(2, 2).Trim() + "'"
                                + " OR [Bookings].BookingY='" + meeting.getnextyear().Substring(2, 2).Trim() + "')"
                                + "  AND [Bookings].DeliverY='" + meeting.getnextyear().Substring(2, 2).Trim() + "'"
                                + " AND YEAR([Bookings].TimeFlag)='" + meeting.getyear().Trim() + "'"
                                + " AND MONTH([Bookings].TimeFlag)='" + month.Trim() + "'"
                                + " THEN Amount*[Currency_Exchange].Rate2 ELSE 0 END)/1000,0) AS '" + meeting.getnextyear().Substring(2, 2).Trim() + "'";
             }
             if (alYears[i].ToString().Trim() == meeting.getyear().Substring(2, 2).Trim())
             {
                 query_EBl += ",ROUND(0/1000,0) AS '" + meeting.getyear().Substring(2, 2).Trim() + "'";
                 query_SalesAF += ",ROUND(SUM(CASE WHEN [Bookings].BookingY='" + meeting.getyear().Substring(2, 2).Trim() + "'"
                                + "  AND [Bookings].DeliverY='" + meeting.getyear().Substring(2, 2).Trim() + "'"
                                + " AND YEAR([Bookings].TimeFlag)='" + meeting.getyear().Trim() + "'"
                                + " AND MONTH([Bookings].TimeFlag)='" + month.Trim() + "'"
                                + " THEN Amount*[Currency_Exchange].Rate1 ELSE 0 END)/1000,0) AS '" + meeting.getyear().Substring(2, 2).Trim() + "'";
             }
             if (alYears[i].ToString().Trim() == meeting.getpreyear().Substring(2, 2).Trim())
             {
                 query_EBl += ",ROUND(0/1000,0) AS '" + meeting.getpreyear().Substring(2, 2).Trim() + "'";
                 query_SalesAF += ",ROUND(SUM(CASE WHEN [Bookings].BookingY='" + meeting.getpreyear().Substring(2, 2).Trim() + "'"
                                + "  AND [Bookings].DeliverY='" + meeting.getpreyear().Substring(2, 2).Trim() + "'"
                                + " AND YEAR([Bookings].TimeFlag)='" + meeting.getpreyear().Trim() + "'"
                                + " AND MONTH([Bookings].TimeFlag)='" + month.Trim() + "'"
                                + " THEN Amount*[Currency_Exchange].Rate1 ELSE 0 END)/1000,0) AS '" + meeting.getpreyear().Substring(2, 2).Trim() + "'";
             }
             if (alYears[i].ToString().Trim() == meeting.getyearBeforePre().Substring(2, 2).Trim())
             {
                 query_EBl += ",ROUND(0/1000,0) AS '" + meeting.getyear().Substring(2, 2).Trim() + "'";
                 query_SalesAF += ",ROUND(SUM(CASE WHEN [Bookings].BookingY='" + meeting.getyearBeforePre().Substring(2, 2).Trim() + "'"
                                + "  AND [Bookings].DeliverY='" + meeting.getyearBeforePre().Substring(2, 2).Trim() + "'"
                                + " AND YEAR([Bookings].TimeFlag)='" + meeting.getyearBeforePre().Trim() + "'"
                                + " AND MONTH([Bookings].TimeFlag)='" + month.Trim() + "'"
                                + " THEN Amount*[Currency_Exchange].Rate1 ELSE 0 END)/1000,0) AS '" + meeting.getyearBeforePre().Substring(2, 2).Trim() + "'";
             }
         }
         query_EBl += " FROM [Bookings],[Segment],[SalesOrg],[SalesOrg_User],[Currency_Exchange]"
                    + " WHERE [Bookings].OperationID='" + operationID + "'"
                    + " AND Segment.Deleted=0 "
                    + " AND SalesOrg.Deleted=0 "
                    + " AND SalesOrg_User.Deleted=0 "
                    + " AND Currency_Exchange.Deleted=0 "
                    + " AND [Segment].Abbr='" + abbr + "'"
                    + " AND [Segment].ID=[Bookings].SegmentID"
                    + " AND [Bookings].RSMID=[SalesOrg_User].UserID"
                    + " AND [SalesOrg_User].SalesOrgID=[SalesOrg].ID"
                    + " AND [SalesOrg].CurrencyID=[Currency_Exchange].CurrencyID"
                    + " AND [Bookings].TimeFlag=[Currency_Exchange].TimeFlag";
         query_SalesAF += " FROM [Bookings],[Segment],[SalesOrg],[SalesOrg_User],[Currency_Exchange]"
                    + " WHERE [Bookings].OperationID='" + operationID + "'"
                    + " AND Segment.Deleted=0 "
                    + " AND SalesOrg.Deleted=0 "
                    + " AND SalesOrg_User.Deleted=0 "
                    + " AND Currency_Exchange.Deleted=0 "
                    + " AND [Segment].Abbr='" + abbr + "'"
                    + " AND [Segment].ID=[Bookings].SegmentID"
                    + " AND [Bookings].RSMID=[SalesOrg_User].UserID"
                    + " AND [SalesOrg_User].SalesOrgID=[SalesOrg].ID"
                    + " AND [SalesOrg].CurrencyID=[Currency_Exchange].CurrencyID"
                    + " AND [Bookings].TimeFlag=[Currency_Exchange].TimeFlag";
         query_EBl += " UNION " + query_SalesAF;
         DataSet ds = helper.GetDataSet(query_EBl);
         if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
             return ds;
         else
             return null;
     }
     protected DataSet getSlide_TwentyNineAdd(string abbr, string operationID)
     {
         string query_EBl = "SELECT 'EB/L' AS ' '";
         string query_SalesAF = "SELECT 'Sales A/F' AS ' '";
         for (int i = 0; i < alYears.Count; i++)
         {
             if (alYears[i].ToString().Trim() == meeting.getyear().Substring(2, 2).Trim())
             {
                 query_EBl += ",ROUND(SUM(CASE WHEN [ActualSalesandBL].BacklogY='" + meeting.getyear().Substring(2, 2).Trim() + "'"
                            + " AND YEAR([ActualSalesandBL].TimeFlag)='" + meeting.getyear().Trim() + "'"
                            + " AND MONTH([ActualSalesandBL].TimeFlag)='" + month.Trim() + "'"
                            + " THEN Backlog*[Currency_Exchange].Rate1 ELSE 0 END)/1000,0) AS ' '";
                 query_SalesAF += ",ROUND(SUM(CASE WHEN [ActualSalesandBL].BacklogY='" + meeting.getyear().Substring(2, 2).Trim() + "'"
                            + " AND YEAR([ActualSalesandBL].TimeFlag)='" + meeting.getyear().Trim() + "'"
                            + " AND MONTH([ActualSalesandBL].TimeFlag)='" + month.Trim() + "'"
                            + " THEN Backlog*[Currency_Exchange].Rate1 ELSE 0 END)/1000,0) AS ' '";
             }
             if (alYears[i].ToString().Trim() == meeting.getnextyear().Substring(2, 2).Trim())
             {
                 query_EBl += ",ROUND(SUM(CASE WHEN [ActualSalesandBL].BacklogY='" + meeting.getnextyear().Substring(2, 2).Trim() + "'"
                            + " AND YEAR([ActualSalesandBL].TimeFlag)='" + meeting.getyear().Trim() + "'"
                            + " AND MONTH([ActualSalesandBL].TimeFlag)='" + month.Trim() + "'"
                            + " THEN Backlog*[Currency_Exchange].Rate2 ELSE 0 END)/1000,0) AS ' '";
                 query_SalesAF += ",ROUND(SUM(CASE WHEN [ActualSalesandBL].BacklogY='" + meeting.getnextyear().Substring(2, 2).Trim() + "'"
                            + " AND YEAR([ActualSalesandBL].TimeFlag)='" + meeting.getyear().Trim() + "'"
                            + " AND MONTH([ActualSalesandBL].TimeFlag)='" + month.Trim() + "'"
                            + " THEN Backlog*[Currency_Exchange].Rate2 ELSE 0 END)/1000,0) AS ' '";
             }
             if (alYears[i].ToString().Trim() == meeting.getpreyear().Substring(2, 2).Trim())
             {
                 query_EBl += ",ROUND(SUM(CASE WHEN [ActualSalesandBL].BacklogY='" + meeting.getpreyear().Substring(2, 2).Trim() + "'"
                            + " AND YEAR([ActualSalesandBL].TimeFlag)='" + meeting.getpreyear().Trim() + "'"
                            + " AND MONTH([ActualSalesandBL].TimeFlag)='" + month.Trim() + "'"
                            + " THEN Backlog*[Currency_Exchange].Rate1 ELSE 0 END)/1000,0) AS ' '";
                 query_SalesAF += ",ROUND(SUM(CASE WHEN [ActualSalesandBL].BacklogY='" + meeting.getpreyear().Substring(2, 2).Trim() + "'"
                            + " AND YEAR([ActualSalesandBL].TimeFlag)='" + meeting.getpreyear().Trim() + "'"
                            + " AND MONTH([ActualSalesandBL].TimeFlag)='" + month.Trim() + "'"
                            + " THEN Backlog*[Currency_Exchange].Rate1 ELSE 0 END)/1000,0) AS ' '";
             }
             if (alYears[i].ToString().Trim() == meeting.getyearBeforePre().Substring(2, 2).Trim())
             {
                 query_EBl += ",ROUND(SUM(CASE WHEN [ActualSalesandBL].BacklogY='" + meeting.getyearBeforePre().Substring(2, 2).Trim() + "'"
                            + " AND YEAR([ActualSalesandBL].TimeFlag)='" + meeting.getyearBeforePre().Trim() + "'"
                            + " AND MONTH([ActualSalesandBL].TimeFlag)='" + month.Trim() + "'"
                            + " THEN Backlog*[Currency_Exchange].Rate1 ELSE 0 END)/1000,0) AS ' '";
                 query_SalesAF += ",ROUND(SUM(CASE WHEN [ActualSalesandBL].BacklogY='" + meeting.getyearBeforePre().Substring(2, 2).Trim() + "'"
                            + " AND YEAR([ActualSalesandBL].TimeFlag)='" + meeting.getyearBeforePre().Trim() + "'"
                            + " AND MONTH([ActualSalesandBL].TimeFlag)='" + month.Trim() + "'"
                            + " THEN Backlog*[Currency_Exchange].Rate1 ELSE 0 END)/1000,0) AS ' '";
             }
         }
         query_EBl += " FROM [ActualSalesandBL],[Segment],[SalesOrg],[Currency_Exchange]"
                    + " WHERE [Segment].ID=[ActualSalesandBL].SegmentID"
                    + " AND Segment.Deleted=0 "
                    + " AND SalesOrg.Deleted=0 "
                    + " AND Currency_Exchange.Deleted=0 "
                    + " AND [Segment].Abbr='" + abbr + "'"
                    + " AND [ActualSalesandBL].OperationID='" + operationID + "'"
                    + " AND [ActualSalesandBL].SalesOrgID=[SalesOrg].ID"
                    + " AND [SalesOrg].CurrencyID=[Currency_Exchange].CurrencyID"
                    + " AND [Currency_Exchange].TimeFlag=[ActualSalesandBL].TimeFlag";
         query_SalesAF += " FROM [ActualSalesandBL],[Segment],[SalesOrg],[Currency_Exchange]"
                    + " WHERE [Segment].ID=[ActualSalesandBL].SegmentID"
                    + " AND Segment.Deleted=0 "
                    + " AND SalesOrg.Deleted=0 "
                    + " AND Currency_Exchange.Deleted=0 "
                    + " AND [Segment].Abbr='" + abbr + "'"
                    + " AND [ActualSalesandBL].OperationID='" + operationID + "'"
                    + " AND [ActualSalesandBL].SalesOrgID=[SalesOrg].ID"
                    + " AND [SalesOrg].CurrencyID=[Currency_Exchange].CurrencyID"
                    + " AND [Currency_Exchange].TimeFlag=[ActualSalesandBL].TimeFlag";
         query_EBl += " UNION " + query_SalesAF;
         DataSet ds = helper.GetDataSet(query_EBl);
         if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
             return ds;
         else
             return null;
     }
}
