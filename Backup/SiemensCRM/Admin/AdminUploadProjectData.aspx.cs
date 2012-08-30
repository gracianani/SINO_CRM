using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;

public partial class Admin_AdminUploadProjectData : System.Web.UI.Page
{
    #region Globle Variable
    SQLHelper helper = new SQLHelper();
    SQLStatement sql = new SQLStatement();
    LogUtility log = new LogUtility();
    ExcelHandler excelHandler = new ExcelHandler();
    GetMeetingDate date = new GetMeetingDate();
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

    protected void btn_upload_Click(object sender, EventArgs e)
    {
        // error message init
        label_visible.Text = "";

        if (!excelHandler.isExcel(this.fleMarketData))
        {
            label_visible.ForeColor = System.Drawing.Color.Red;
            label_visible.Text = "Please select excel file only!";
            return;
        }
        bool checkNull = checkExcelNullExist();
        if (checkNull == false)
        {
            return;
        }
        else
        {
            label_visible.Text = "";
        }

        bool checkE = checkExcel();
        if (checkE == false)
        {
            return;
        }
        else
        {
            label_visible.Text = "";
        }

        bool check = checkData();
        if (check == true)
        {
            DataTable ds = excelHandler.importExcel(fleMarketData, 0);
            int flag = 0;
            if (ds != null)
            {
                if (ds.Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Rows.Count; i++)
                    {
                        if (i >= 1)
                        {
                            string ProjectName = ds.Rows[i][0].ToString().Trim();
                            string Segment = ds.Rows[i][1].ToString().Trim();
                            string CustomerName = ds.Rows[i][2].ToString().Trim();
                            string ProjectCountryPoS = ds.Rows[i][3].ToString().Trim();
                            string ProjectValue = ds.Rows[i][4].ToString().Equals("") ? "0" : ds.Rows[i][4].ToString().Trim();
                            string InBudget = ds.Rows[i][5].ToString().Equals("") ? "0" : ds.Rows[i][5].ToString().Trim().Trim('%');
                            string Currency = ds.Rows[i][6].ToString().Equals("") ? "0" : ds.Rows[i][6].ToString().Trim();
                            string ProjectCountryPoD = ds.Rows[i][8].ToString().Trim();
                            string Comments = ds.Rows[i][9].ToString().Trim();

                            string CustomerNameID = getCustomerNameID(CustomerName);
                            string SubRegionIDPoS = getSubRegionID(ProjectCountryPoS);
                            string SubRegionIDPoD = getSubRegionID(ProjectCountryPoD);
                            string CurrencyID = getCurrencyID(Currency);
                            string SegmentID = getSegmentID(Segment);
                            string sql = "INSERT INTO [Project](Name,CustomerNameID,PoSID,Value,"
                            + "Probability,PoDID,CurrencyID,Comments,Deleted,ProSegmentID,CreateUser)"
                            + " VALUES ('" + ProjectName + "'," + CustomerNameID + ","
                            + SubRegionIDPoS + "," + ProjectValue + "," + InBudget + ","
                            + SubRegionIDPoD + "," + CurrencyID + ",'" + Comments + "',0,"
                            + SegmentID + ",null)";
                            int count = helper.ExecuteNonQuery(CommandType.Text, sql, null);
                            if (count == 0)
                            {
                                flag = flag + 1;
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
    //by yyan itemW69 edit start
    protected bool checkExcelNullExist()
    {
        DataTable ds = excelHandler.importExcel(fleMarketData, 0);
        string flag = "";

        if (ds != null)
        {
            if (ds.Rows.Count > 0)
            {
                for (int i = 0; i < ds.Rows.Count; i++)
                {
                    if (i >= 1)
                    {
                        int ProjectName = ds.Rows[i][0].ToString().Trim().Length;
                        int ProjectValue = ds.Rows[i][4].ToString().Trim().Length;
                        int InBudget = ds.Rows[i][5].ToString().Trim().Length;
                        string Segment = ds.Rows[i][1].ToString().Trim();
                        string CustomerName = ds.Rows[i][2].ToString().Trim();
                        string ProjectCountryPoS = ds.Rows[i][3].ToString().Trim();
                        string Currency = ds.Rows[i][6].ToString().Trim();
                        string ProjectCountryPoD = ds.Rows[i][8].ToString().Trim();

                        if (ProjectName <= 0)
                        {
                            label_visible.Text = label_visible.Text + "Project name value in line " + (i + 1) + " is empty! Please check.</br>";
                        }
                        if (ProjectValue <= 0)
                        {
                            label_visible.Text = label_visible.Text + "Project Value in line " + (i + 1) + " is empty! Please check.</br>";
                        }
                        if (InBudget <= 0)
                        {
                            label_visible.Text = label_visible.Text + "% in budget value in line " + (i + 1) + " is empty! Please check.</br>";
                        }
                        // exist is or not
                        string CustomerNameID = getCustomerNameID(CustomerName);
                        string SubRegionIDPoS = getSubRegionID(ProjectCountryPoS);
                        string SubRegionIDPoD = getSubRegionID(ProjectCountryPoD);
                        string CurrencyID = getCurrencyID(Currency);
                        string SegmentID = getSegmentID(Segment);
                        if (CustomerName.Length <= 0)
                        {
                            label_visible.Text = label_visible.Text + "Customer name value in line " + (i + 1) + " is empty! Please check.</br>";
                        }
                        else if ("".Equals(CustomerNameID))
                        {
                            label_visible.Text = label_visible.Text + "Customer name(" + CustomerName + ") in excel file line " + (i + 1) + " does not exist in System.</br>";
                        }
                        if (ProjectCountryPoS.Length <= 0)
                        {
                            label_visible.Text = label_visible.Text + "Project Country (PoS) value in line " + (i + 1) + " is empty! Please check.</br>";
                        }
                        else if ("".Equals(SubRegionIDPoS))
                        {
                            label_visible.Text = label_visible.Text + "Project Country (PoS)(" + ProjectCountryPoS + ") in excel file line " + (i + 1) + " does not exist in System.</br>";
                        }
                        if (ProjectCountryPoD.Length <= 0)
                        {
                            label_visible.Text = label_visible.Text + "Project Country (PoD) value in line " + (i + 1) + " is empty! Please check.</br>";
                        }
                        else if ("".Equals(SubRegionIDPoD))
                        {
                            label_visible.Text = label_visible.Text + "Project Country (PoD)(" + ProjectCountryPoD + ") in excel file line " + (i + 1) + " does not exist in System.</br>";
                        }
                        if (Currency.Length <= 0)
                        {
                            label_visible.Text = label_visible.Text + "Currency value in line " + (i + 1) + " is empty! Please check.</br>";
                        }
                        else if ("".Equals(CurrencyID))
                        {
                            label_visible.Text = label_visible.Text + "Currency(" + Currency + ") in excel file line " + (i + 1) + " does not exist in System.</br>";
                        }
                        if (Segment.Length <= 0)
                        {
                            label_visible.Text = label_visible.Text + "Segment value in line " + (i + 1) + " is empty! Please check.</br>";
                        }
                        else if ("".Equals(SegmentID))
                        {
                            label_visible.Text = label_visible.Text + "Segment(" + Segment + ") in excel file line " + (i + 1) + " does not exist in System.</br>";
                        }
                    }
                }
            }
        }
        if (!"".Equals(label_visible.Text))
        {
            label_visible.ForeColor = System.Drawing.Color.Red;
            return false;
        }
        return true;
    }
    //by yyan itemW69 edit end
    protected bool checkData()
    {
        DataTable ds = excelHandler.importExcel(fleMarketData, 0);
        string flag = "";

        if (ds != null)
        {
            if (ds.Rows.Count > 0)
            {
                for (int i = 0; i < ds.Rows.Count; i++)
                {
                    if (i >= 1)
                    {
                        string ProjectName = ds.Rows[i][0].ToString().Trim();
                        string Segment = ds.Rows[i][1].ToString().Trim();
                        string CustomerName = ds.Rows[i][2].ToString().Trim();
                        string ProjectCountryPoD = ds.Rows[i][8].ToString().Trim();

                        string sql = "select count(*) from [Project] where "
                              + " Name = '" + ProjectName + "'"
                              + " AND ProSegmentID = " + getSegmentID(Segment)
                              + " AND CustomerNameID = " + getCustomerNameID(CustomerName)
                              + " AND PoDID = " + getSubRegionID(ProjectCountryPoD);
                        DataSet select_marketDs = helper.GetDataSet(sql);
                        if (Convert.ToInt32(select_marketDs.Tables[0].Rows[0][0]) >= 1)
                        {
                            label_visible.ForeColor = System.Drawing.Color.Red;
                            label_visible.Text = label_visible.Text + "Data in excel file ( line " + (i + 1) + ") exists in System.</br>";
                            flag = "true";
                        }
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

    protected string getCustomerNameID(string CustomerName)
    {
        CustomerName = CustomerName.Replace("'", "''");
        string query_product = "SELECT ID FROM [CustomerName] WHERE Name = '" + CustomerName + "' AND Deleted = 0";
        DataSet ds = helper.GetDataSet(query_product);
        if (ds.Tables[0].Rows.Count >= 1)
            return ds.Tables[0].Rows[0][0].ToString().Trim();
        else
        {
            return "";
        }
    }

    protected string getCurrencyID(string Currency)
    {
        Currency = Currency.Replace("'", "''");
        string query_product = "SELECT ID FROM [Currency] WHERE Name = '" + Currency + "' AND Deleted = 0";
        DataSet ds = helper.GetDataSet(query_product);
        if (ds.Tables[0].Rows.Count >= 1)
            return ds.Tables[0].Rows[0][0].ToString().Trim();
        else
        {
            return "";
        }
    }

    protected string getSubRegionID(string SubRegion)
    {
        SubRegion = SubRegion.Replace("'", "''");
        string query_product = "SELECT ID FROM [Country] WHERE Name = '" + SubRegion + "' AND Deleted = 0";
        DataSet ds = helper.GetDataSet(query_product);
        if (ds.Tables[0].Rows.Count >= 1)
            return ds.Tables[0].Rows[0][0].ToString().Trim();
        else
        {
            return "";
        }
    }

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
                        if (excelHandler.isForm(ds, 10))
                        {
                            string ProjectName = ds.Rows[i][0].ToString().Trim();
                            string Segment = ds.Rows[i][1].ToString().Trim();
                            string CustomerName = ds.Rows[i][2].ToString().Trim();
                            string ProjectCountryPoD = ds.Rows[i][8].ToString().Trim();
                            for (int j = 2; j < ds.Rows.Count; j++)
                            {
                                if (j != i && j > i)
                                {
                                    string ProjectNamej = ds.Rows[j][0].ToString().Trim();
                                    string Segmentj = ds.Rows[j][1].ToString().Trim();
                                    string CustomerNamej = ds.Rows[j][2].ToString().Trim();
                                    string ProjectCountryPoDj = ds.Rows[j][8].ToString().Trim();

                                    string stri = "";
                                    string strj = "";
                                    if (ProjectName.Equals(ProjectNamej) && Segment.Equals(Segmentj) &&
                                        CustomerName.Equals(CustomerNamej) && ProjectCountryPoD.Equals(ProjectCountryPoDj))
                                    {
                                        stri = i + 1 + "";
                                        strj = j + 1 + "";
                                        label_visible.Text = label_visible.Text + "Same data in line " + stri + " and " + strj + ".</br>";
                                        flag = "true";
                                    }
                                }
                            }
                        }
                        else
                        {
                            label_visible.Text = "Please check the format of the excel file!";
                            return false;
                        }
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
}