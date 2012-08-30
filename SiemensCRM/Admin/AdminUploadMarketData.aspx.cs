using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Text;

public partial class Admin_AdminUploadMarketData : System.Web.UI.Page
{
    #region Globle Variable
    SQLHelper helper = new SQLHelper();
    SQLStatement sql = new SQLStatement();
    LogUtility log = new LogUtility();
    //by mbq 20110513 item12 add start   
    ExcelHandler excelHandler = new ExcelHandler();
    //by mbq 20110513 item12 add end   
    // By DingJunjie 20110520 Item 12 Add Start
    GetMeetingDate date = new GetMeetingDate();
    // By DingJunjie 20110520 Item 12 Add End
    #endregion

    #region Event
    /// <summary>
    /// Page Load
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (string.Equals(getRoleID(getRole()), "0"))
        {

        }
        else
        {
            Response.Redirect("~/AccessDenied.aspx");
        }
    }
    #endregion

    #region Method
    /// <summary>
    /// Get User Role
    /// </summary>
    /// <returns>User Role</returns>
    private string getRole()
    {
        return Session["Role"].ToString().Trim();
    }

    /// <summary>
    /// Get Role ID
    /// </summary>
    /// <param name="str_name">Role Name</param>
    /// <returns>Role ID</returns>
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
    #endregion

     // by mbq 20110513 item12 add start   
    protected void btn_upload_Click(object sender, EventArgs e)
    {
        try
        {
            if (!excelHandler.isExcel(this.fleMarketData))
            {
                label_visible.ForeColor = System.Drawing.Color.Red;
                label_visible.Text = "Please select excel file only!";
                return;
            }

            // by mbq 20110523 item12 add start  
            bool checkE = checkExcel();
            if (checkE == false)
            {
                return;
            }
            else
            {
                label_visible.Text = "";
            }
            // by mbq 20110523 item12 add end  

            bool check = checkData();
            if (check == true)
            {
                DataTable ds = excelHandler.importExcel(fleMarketData, 0);
                int flag = 0;
                if (ds != null)
                {
                    if (ds.Rows.Count > 0)
                    {
                        StringBuilder strSQL = null;
                        date.setDate();
                        int count = 0;
                        for (int i = 0; i < ds.Rows.Count; i++)
                        {

                            if (i >= 1)
                            {
                               

                                // by mbq 20110523 item12 add start   
                                string Segment = ds.Rows[i][0].ToString().Trim();
                                string Region = ds.Rows[i][1].ToString().Trim();
                                string Cluster = ds.Rows[i][2].ToString().Trim();
                                string Country = ds.Rows[i][3].ToString().Trim();
                                string Totalmarket1 = ds.Rows[i][4].ToString().Equals("") ? "0" : ds.Rows[i][4].ToString().Trim();
                                string Totalmarket2 = ds.Rows[i][5].ToString().Equals("") ? "0" : ds.Rows[i][5].ToString().Trim();
                                string Totalmarket3 = ds.Rows[i][6].ToString().Equals("") ? "0" : ds.Rows[i][6].ToString().Trim();
                                // by mbq 20110523 item12 add end

                                // By DingJunjie 20110520 Item 12 Delete Start
                                //string sql = "INSERT INTO [Market](CountryID,SegmentID,ThisYear,NextYear,AfterYear)"
                                // + " VALUES ('" + getCountryID(Country) + "','" + getSegmentID(Segment) + "'," + Totalmarket1 + "," + Totalmarket2 + "," + Totalmarket3 + ")";
                                // By DingJunjie 20110520 Item 12 Delete End


                                // By DingJunjie 20110523 Item 12 del Start
                                // By DingJunjie 20110520 Item 12 Add Start
                                //string sql = "INSERT INTO [Market](CountryID,SegmentID,ThisYear,NextYear,AfterYear,TimeFlag)"
                                // + " VALUES ('" + getCountryID(Country) + "','" + getSegmentID(Segment) + "'," + Totalmarket1 + "," + Totalmarket2 + "," + Totalmarket3 + ",'" + date.getSetMeetingDate().Tables[0].Rows[0][0].ToString().Trim() + "')";
                                // By DingJunjie 20110520 Item 12 Add End
                                // By DingJunjie 20110523 Item 12 del end

                                strSQL = new StringBuilder();
                                strSQL.AppendLine(" UPDATE ");
                                strSQL.AppendLine("   [Market] ");
                                strSQL.AppendLine(" SET ");
                                strSQL.AppendLine("   ThisYear=" + Totalmarket1 + ",");
                                strSQL.AppendLine("   NextYear=" + Totalmarket2 + ",");
                                strSQL.AppendLine("   AfterYear=" + Totalmarket3);
                                strSQL.AppendLine(" WHERE ");
                                strSQL.AppendLine("   SegmentID=" + getSegmentID(Segment));
                                strSQL.AppendLine("   AND RegionID=" + getRegionID(Region));
                                strSQL.AppendLine("   AND ClusterID=" + getClusterID(Cluster));
                                strSQL.AppendLine("   AND CountryID=" + getCountryID(Country));
                                strSQL.AppendLine("   AND Year(TimeFlag)=" + date.getyear());
                                strSQL.AppendLine("   AND Month(TimeFlag)=" + date.getmonth());
                                count = helper.ExecuteNonQuery(CommandType.Text, strSQL.ToString(), null);
                                if (count == 0)
                                {
                                    // By mbq 20110523 Item 12 Add Start
                                    string sql = "INSERT INTO [Market](CountryID,SegmentID,ThisYear,NextYear,AfterYear,TimeFlag,RegionID,ClusterID)"
                                     + " VALUES ('" + getCountryID(Country) + "','" + getSegmentID(Segment) + "'," + Totalmarket1 + "," + Totalmarket2 + "," + Totalmarket3 + ",'" + date.getSetMeetingDate().Tables[0].Rows[0][0].ToString().Trim() + "'" + ",'" + getRegionID(Region) + "','" + getClusterID(Cluster) + "')";
                                    // By mbq 20110523 Item 12 Add End
                                    count = helper.ExecuteNonQuery(CommandType.Text, sql, null);
                                    if (count == 0)
                                    {
                                        flag = flag + 1;
                                    }
                                }
                            }
                        }
                    }
                }
                if (flag < 1)
                {
                    label_visible.ForeColor = System.Drawing.Color.Green;
                    label_visible.Text = "The file has been uploaded successfully!";
                }
                else
                {
                    label_visible.Text = "";
                }
            }
        }
        catch (Exception em)
        {
            label_visible.ForeColor = System.Drawing.Color.Red;
            label_visible.Text = "The file has been uploaded failed!";
            log.WriteLog(LogUtility.LogErrorLevel.LOG_ERROR, "Upload Market excel file failed. Exception: " + em.Message + "\n" + em.StackTrace);
        }
    }
     // by mbq 20110513 item12 add end   

     // by mbq 20110513 item12 add start   
    protected string getSegmentID(string str)
    {
        str = str.Replace("'", "''");
        string sql = "SELECT [Segment].ID FROM [Segment]  WHERE [Segment].Abbr = '" + str + "' AND [Segment].Deleted = 0 ";
        DataSet ds = helper.GetDataSet(sql);
        if (ds.Tables[0].Rows.Count >= 1)
            return ds.Tables[0].Rows[0][0].ToString().Trim();
        else
        {
            return "";
        }
    }
    // by mbq 20110513 item12 add end   

    // by mbq 20110513 item12 add start   
    protected string getCountryID(string str)
    {
        str = str.Replace("'", "''");
        string sql = "SELECT [Country].ID FROM [Country]  WHERE [Country].Name = '" + str + "' AND [Country].Deleted = 0 ";
        DataSet ds = helper.GetDataSet(sql);
            if (ds.Tables[0].Rows.Count >= 1)
                return ds.Tables[0].Rows[0][0].ToString().Trim();
            else
            {
                return "";
            }

    }
    // by mbq 20110513 item12 add end   

    // by mbq 20110517 item12 add start   
    protected bool checkData()
    {
        DataTable ds = excelHandler.importExcel(fleMarketData,0);

        // by zy 20110708 W61 start
        if (ds != null){
            if (ds.Rows.Count > 0) {
                for (int i = 1; i < ds.Rows.Count; i++)
                {
                    // by mbq 20110523 item12 add start   
                    string Segment = ds.Rows[i][0].ToString().Trim();
                    string Region = ds.Rows[i][1].ToString().Trim();
                    string Cluster = ds.Rows[i][2].ToString().Trim();
                    string Country = ds.Rows[i][3].ToString().Trim();
                    string Totalmarket1 = ds.Rows[i][4].ToString().Equals("") ? "0" : ds.Rows[i][4].ToString().Trim();
                    string Totalmarket2 = ds.Rows[i][5].ToString().Equals("") ? "0" : ds.Rows[i][5].ToString().Trim();
                    string Totalmarket3 = ds.Rows[i][6].ToString().Equals("") ? "0" : ds.Rows[i][6].ToString().Trim();
                    // by mbq 20110523 item12 add end   

                    string RegionID = getRegionID(Region);
                    string CountryID = getCountryID(Country);
                    string ClusterID = getClusterID(Cluster);
                    string SegmentID = getSegmentID(Segment);

                    // Segment check
                    if (Segment == "")
                    {
                        label_visible.Text = label_visible.Text + "Segment value in excel file ( line " + (i + 1) + ") is empty! Please check.</br>";
                    }
                    else if ("".Equals(SegmentID))
                    {
                        label_visible.Text = label_visible.Text + "Segment(" + Segment + ") in excel file ( line " + (i + 1) + ") is not exist in System.</br>";
                    }
                    // Region check
                    if (Region == "")
                    {
                        label_visible.Text = label_visible.Text + "Region value in excel file ( line " + (i + 1) + ") is empty! Please check.</br>";
                    }
                    else if ("".Equals(RegionID))
                    {
                        label_visible.Text = label_visible.Text + "Region(" + Region + ") in excel file ( line " + (i + 1) + ") is not exist in System.</br>";
                    }
                    // Cluster check
                    if (Cluster == "")
                    {
                        label_visible.Text = label_visible.Text + "Cluster value in excel file ( line " + (i + 1) + ") is empty! Please check.</br>";
                    }
                    else if ("".Equals(ClusterID))
                    {
                        label_visible.Text = label_visible.Text + "Cluster(" + Cluster + ") in excel file ( line " + (i + 1) + ") is not exist in System.</br>";
                    }
                    // Country check
                    if (Country == "")
                    {
                        label_visible.Text = label_visible.Text + "Country value in excel file ( line " + (i + 1) + ") is empty! Please check.</br>";
                    }
                    else if ("".Equals(CountryID))
                    {
                        label_visible.Text = label_visible.Text + "Country(" + Country + ") in excel file ( line " + (i + 1) + ") is not exist in System.</br>";
                    }
                }
            }
        }
        // check ok?
        if ("".Equals(label_visible.Text))
        {
            return true;
        }

        // check NG
        label_visible.ForeColor = System.Drawing.Color.Red;
        return false;
        // by zy 20110708 W61 end 
    }
    // by mbq 20110517 item12 add end   




    // by mbq 20110523 item12 add start   
    protected string getRegionID(string Region)
    {
        Region = Region.Replace("'", "''");
        string query_product = "SELECT ID FROM [Region] WHERE Name = '" + Region + "'AND Deleted = 0";
        DataSet ds = helper.GetDataSet(query_product);
        if (ds.Tables[0].Rows.Count >= 1)
            return ds.Tables[0].Rows[0][0].ToString().Trim();
        else
        {
            return "";
        }
    }
    // by mbq 20110523 item12 add end   

    // by mbq 20110523 item12 add start   
    protected string getClusterID(string Cluster)
    {
        Cluster = Cluster.Replace("'", "''");
        string query_product = "SELECT ID FROM [Cluster] WHERE Name = '" + Cluster + "'AND Deleted = 0";
        DataSet ds = helper.GetDataSet(query_product);
        if (ds.Tables[0].Rows.Count >= 1)
            return ds.Tables[0].Rows[0][0].ToString().Trim();
        else
        {
            return "";
        }
    }
    // by mbq 20110523 item12 add end 


    // by mbq 20110523 item12 add start   
    protected bool checkExcel()
    {
        string flag = "";
        label_visible.Text = "";
        DataTable ds = excelHandler.importExcel(fleMarketData, 0);
        if (ds != null)
        {
            if (ds.Rows.Count > 0)
            {
                for (int i = 0; i < ds.Rows.Count; i++)
                {
                    if (i >= 1)
                    {
                        //by yyan item w33 20110620 add start
                        if (excelHandler.isForm(ds,7))
                        {
                         //by yyan item w33 20110620 add end
                            string Segment = ds.Rows[i][0].ToString().Trim();
                            string Region = ds.Rows[i][1].ToString().Trim();
                            string Cluster = ds.Rows[i][2].ToString().Trim();
                            string Country = ds.Rows[i][3].ToString().Trim();
                            string Totalmarket1 = ds.Rows[i][4].ToString().Equals("") ? "0" : ds.Rows[i][4].ToString().Trim();
                            string Totalmarket2 = ds.Rows[i][5].ToString().Equals("") ? "0" : ds.Rows[i][5].ToString().Trim();
                            string Totalmarket3 = ds.Rows[i][6].ToString().Equals("") ? "0" : ds.Rows[i][6].ToString().Trim();
                            for (int j = 0; j < ds.Rows.Count; j++)
                            {
                                if (j != i && j > i) {
                                    string Segmentj = ds.Rows[j][0].ToString().Trim();
                                    string Regionj  = ds.Rows[j][1].ToString().Trim();
                                    string Clusterj = ds.Rows[j][2].ToString().Trim();
                                    string Countryj = ds.Rows[j][3].ToString().Trim();
                                    string Totalmarket1j = ds.Rows[j][4].ToString().Equals("") ? "0" : ds.Rows[j][4].ToString().Trim();
                                    string Totalmarket2j = ds.Rows[j][5].ToString().Equals("") ? "0" : ds.Rows[j][5].ToString().Trim();
                                    string Totalmarket3j = ds.Rows[j][6].ToString().Equals("") ? "0" : ds.Rows[j][6].ToString().Trim();
                                     
                                    string stri = "";
                                    string strj = "";
                                    if (Segment.Equals(Segmentj) && Region.Equals(Regionj) && Cluster.Equals(Clusterj) && Country.Equals(Countryj))
                                    {
                                        stri = i + 1 + "";
                                        strj = j + 1  + "";
                                        label_visible.Text = label_visible.Text + "Same data in line " + stri + " and " + strj + ".</br>";
                                        flag = "true";
                                    }
                                }
                            }
                        //by yyan item w33 20110620 add start
                        }
                        else
                        {
                            label_visible.Text = "Please check the format of the excel file!";
                            return false;
                        }
                        //by yyan item w33 20110620 add end
                    }
                }
            }
        }
        if ("true".Equals(flag))
        {
            return false;
        }
        return true;
    }
    // by mbq 20110523 item12 add end   
}