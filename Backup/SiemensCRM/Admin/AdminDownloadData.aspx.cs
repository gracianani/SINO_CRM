using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Diagnostics;
using System.IO;
using org.in2bits.MyXls;

public partial class Admin_AdminDownloadData : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    protected void Button1_Click(object sender, EventArgs e)
    {
        Export("ActualSalesandBL");
        Export("Bookings");
        Export("Cluster");
        Export("Cluster_Country");
        Export("Country");
        Export("Country_SubRegion");
        Export("Currency");
        Export("Currency_Exchange");
        Export("Customer");
        Export("CustomerName");
        Export("CustomerType");
        Export("DataDict");
        Export("Lock");
        Export("LockAllUser");
        Export("LockSegment");
        Export("Market");
        Export("MeetingDate");
        Export("Operation");
        Export("Operation_Segment");
        Export("Product");
        Export("Project");
        Export("Region");
        Export("Region_Cluster");
        Export("ReportList");
        Export("ReportRole");
        Export("ReportValue");
        Export("Role");
        Export("SalesChannel");
        Export("SalesOrg");
        Export("SalesOrg_Segment");
        Export("SalesOrg_User");
        Export("Segment");
        Export("Segment_Product");
        Export("SetMeetingDate");
        Export("SubRegion");
        Export("User");
        Export("User_Country");
        Export("User_Operation");
        Export("User_Segment");
        Export("User_Status");
        Export("TrafficLights");

        string code_path = getApplicationPath();
        string str_databasepath = code_path + "\\DownLoad\\excel\\";
        string str_database = "Excel.zip";
        zipDataBase(str_databasepath, str_database);

        string codedestinationpath = code_path + "\\DownLoad\\excel\\" + str_database;
        downloadSourceCode(codedestinationpath, str_database);
    }
    private bool zipDataBase(string str_databasepath, string str_database)
    {
        string rar;
        string args;
        ProcessStartInfo procStart;
        //By mbq 20110531 Item W20 del Start 
        //Process process;
        //By mbq 20110531 Item W20 del end 
        //By mbq 20110531 Item W20 add Start 
        Process process = new Process();
        //By mbq 20110531 Item W20 add end 

        string databasedestinationpath = str_databasepath + "\\" + str_database;

        try
        {
            if (System.IO.File.Exists(databasedestinationpath))
            {
                System.IO.File.Delete(databasedestinationpath);
            }
        }
        catch (Exception ex)
        {
            return false;
        }

        try
        {
            rar = ConfigurationSettings.AppSettings["winrarpath"].ToString();
            args = "a -inul -y -o+ -ep1 " + databasedestinationpath + " " + str_databasepath;
            procStart = new ProcessStartInfo();
            procStart.FileName = rar;
            procStart.Arguments = args;
            procStart.WorkingDirectory = Server.MapPath("");
            // By mbq 20110531 Item W20 del Start 
            // process = new Process();
            // By mbq 20110531 Item W20 del end 
            process.StartInfo = procStart;
            process.Start();
            process.WaitForExit();
            // By mbq 20110531 Item W20 Add Start 
            process.Close();
            // By mbq 20110531 Item W20 Add end 
            
            return true;
        }
        catch (Exception ex)
        {
            // By mbq 20110531 Item W20 Add Start 
            process.Close();
            // By mbq 20110531 Item W20 Add end 
            Response.Close();
            return false;
        }
    }
    private string getApplicationPath()
    {
        return Server.MapPath("~").ToString().Trim();
    }

    private bool downloadSourceCode(string str_filepath, string str_filename)
    {
        return downloadFile(str_filepath, str_filename);
    }
    private bool downloadFile(string str_filepath, string str_filename)
    {
        FileInfo file = new FileInfo(str_filepath);
        if (file.Exists)
        {
            GC.Collect();
            Response.Clear();
            Response.Charset = "GB2312";
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.AddHeader("Content-Disposition", "attachment; filename=" + Server.UrlEncode(str_filename));
            Response.AddHeader("Content-Length", file.Length.ToString());
            Response.WriteFile(file.FullName);
            Response.End();

            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Excel export
    /// </summary>
    /// <param name="excelInfo"></param>
    public void exportExcel(DataTable excelInfo)
    {
       // old code
        
        //XlsDocument xls = new XlsDocument();
        
        //xls.FileName = HttpContext.Current.Server.UrlEncode(excelInfo.TableName) + ".xlsx";
       
        //Worksheet sheet = xls.Workbook.Worksheets.Add("Sheet1");
       
        //ColumnInfo ci = new ColumnInfo(xls, sheet);
        //ci.ColumnIndexStart = 1;
        //ci.ColumnIndexEnd = (ushort)(excelInfo.Columns.Count - 1);
        //ci.Width = 13 * 256;
        //sheet.AddColumnInfo(ci);
       
        //Cells cells = sheet.Cells;
        
        //XF xf = xls.NewXF();
        //xf.HorizontalAlignment = HorizontalAlignments.Centered;
        //xf.VerticalAlignment = VerticalAlignments.Centered;
       
        //cells.Merge(1, 1, 1, excelInfo.Columns.Count);
        //cells.Add(1, 1, excelInfo.TableName, xf);
       
        //xf.UseBorder = true;
        //xf.TopLineStyle = 1;
        //xf.BottomLineStyle = 1;
        //xf.LeftLineStyle = 1;
        //xf.RightLineStyle = 1;
        
        //for (int i = 0; i < excelInfo.Columns.Count; i++)
        //{
        
        //    cells.Add(2, i + 1, excelInfo.Columns[i].ColumnName, xf);
       
        //    for (int j = 0; j < excelInfo.Rows.Count; j++)
        //    {
      
        //        if (IsFloat(excelInfo.Rows[j][i].ToString()))
        //        {
     
        //            cells.Add(j + 3, i + 1, Convert.ToDecimal(excelInfo.Rows[j][i]), xf);
        //        }
        //        else
        //        {
       
        //            cells.Add(j + 3, i + 1, excelInfo.Rows[j][i].ToString(), xf);
        //        }
        //    }
        //}

        //string serverFolderPath = HttpContext.Current.Request.PhysicalApplicationPath + "DownLoad\\excel\\";
        //xls.Save(serverFolderPath, true);

        // old code end
        string fileName = HttpContext.Current.Request.PhysicalApplicationPath + "DownLoad\\excel\\" + HttpContext.Current.Server.UrlEncode(excelInfo.TableName) + ".csv";
        try
        {
            StreamWriter sw = new StreamWriter(fileName, false, System.Text.Encoding.GetEncoding("Unicode"));
            string columnTitle = "";
            try
            {
                columnTitle += excelInfo.TableName;
                sw.WriteLine(columnTitle);

                string columnValue = "";
                for (int m = 0; m < excelInfo.Columns.Count; m++)
                {

                    if (m > 0)
                    {
                        columnValue += "\t";
                    }
                    columnValue += excelInfo.Columns[m].ColumnName.Replace(",", "，");
                }
                sw.WriteLine(columnValue);
                for (int j = 0; j < excelInfo.Rows.Count; j++)
                {
                    columnValue = "";
                    for (int k = 0; k < excelInfo.Columns.Count; k++)
                    {

                        if (k > 0)
                        {
                            columnValue += "\t";
                        }
                        if (IsFloat(excelInfo.Rows[j][k].ToString()))
                            columnValue += Convert.ToDecimal(excelInfo.Rows[j][k]);
                        else
                            columnValue += excelInfo.Rows[j][k].ToString().Replace(",", "，");

                        //if (excelInfo.Rows[j][k] == null)
                        //{
                        //    columnValue += "";
                        //}
                        //else
                        //{
                        //    string m = excelInfo.Rows[j].Cells[k].Value.ToString().Trim();
                        //    columnValue += m.Replace(",", "，");
                        //}
                    }
                    columnValue.Remove(columnValue.Length - 1);
                    sw.WriteLine(columnValue);
                }
                sw.Close();

            }
            finally
            {
                sw.Close();

            }
        }
        catch
        { return; }

    }

    public void Export(string tableName)
    {
        SQLHelper helper = new SQLHelper();
        DataSet ds1 = null;
        if (tableName == "User_Status")
        {
            string strSql = "select B.Alias as [User],c.Abbr as Segment,A.Status,Name as Rool from User_Status A,[User] B,Segment C,[Role]  "
                      + " where A.UserID=B.UserID and A.SegmentID=C.ID and B.Deleted=0 and C.Deleted=0 and B.RoleID=[Role].ID";
            ds1 = helper.GetDataSet(strSql);
        }
        else if (tableName == "TrafficLights")
        {
            string trafficLightSql = "  select Alias,Abbr, 'Status'= "
                              + " case "
                              + " when Status IS NULL and RoleID=2 then 'R' "
                              + " when Status IS null and RoleID=3 then 'Y' "
                              + " when Status IS null and RoleID=4 then 'R' "
                              + " when Status is not null then Status "
                              + " end ,Name as Role"
                              + " from "
                              + " (select A.UserID,A.SegmentID,B.Alias,C.Abbr,B.RoleID from User_Segment A inner join [User] B on A.UserID=B.UserID "
                              + " inner join Segment C on A.SegmentID=C.ID "
                              + " where B.RoleID in (2,3,4) and B.Deleted=0 and A.Deleted=0 and C.Deleted=0) O "
                              + " left join "
                              + " (select UserID,SegmentID,Status from User_Status "
                              + " union "
                              + " select MarketingMgrID as UserID,SegmentID,Status from ActualSalesandBL_Status) P "
                              + " on O.UserID=P.UserID and O.SegmentID=P.SegmentID "
                              + " inner join Role on RoleID=Role.ID";
            ds1 = helper.GetDataSet(trafficLightSql);
        }
        else
            ds1 = helper.GetDataSet("select * from [" + tableName + "]");
        if (ds1 == null)
        {
            return;
        }
        ds1.Tables[0].TableName = tableName;
        exportExcel(ds1.Tables[0]);
    }

    /// <summary>
    /// float number
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static bool IsFloat(string str)
    {
        System.Text.RegularExpressions.Regex reg1
          = new System.Text.RegularExpressions.Regex(@"^(-?\d+)(\.\d+)?$");
        return reg1.IsMatch(str);
    }
}