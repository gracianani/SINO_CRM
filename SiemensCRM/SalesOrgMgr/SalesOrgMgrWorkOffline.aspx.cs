/*
 *  File Name    : SalesOrgMgrWorkOffline.aspx
 * 
 *  Description  : 
 * 
 *  Author       : Wang Jun
 * 
 *  Modified Date: 2010-03-29
 * 
 *  Problem      : 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Net;
using System.Threading;
using Microsoft.Win32;
using System.Diagnostics;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System.Configuration;
using System.Web.Security;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

public partial class SalesOrgMgr_SalesOrgMgrWorkOffline : System.Web.UI.Page
{
    SQLHelper helper = new SQLHelper();
    LogUtility log = new LogUtility();
    WebUtility webU = new WebUtility();
    GetMeetingDate meeting = new GetMeetingDate();

    protected static string year;
    protected static string month;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (getRoleID(getRole()) == "3")
        { }
        else
            Response.Redirect("~/AccessDenied.aspx");

        if (!IsPostBack)
        {
            meeting.setDate();
            year = meeting.getyear();
            month = meeting.getmonth();
        }
    }

    protected string getRole()
    {
        return Session["Role"].ToString().Trim();
    }

    protected string getRoleID(string str_name)
    {
        string query_role = "SELECT ID FROM [Role] WHERE Name = '" + str_name + "'";
        DataSet ds_role = helper.GetDataSet(query_role);
        return ds_role.Tables[0].Rows[0][0].ToString().Trim();
    }

    private string getApplicationPath()
    {
        return Server.MapPath("~").ToString().Trim();
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
            log.WriteLog(LogUtility.LogErrorLevel.LOG_CRITICAL, "That downloading path is '" + str_filepath + "' did not exist.");

            return false;
        }
    }

    //Virtual Machine
    protected void ibtn_virtualmachine_Click(object sender, ImageClickEventArgs e)
    {
        string str_codePath = getApplicationPath();
        string str_fileName = "Virtual_Machine.rar";
        string str_virtualmachine = str_codePath + "\\DownLoad\\VirtualMachine";

        if (zipVirtualMachine(str_virtualmachine, str_fileName))
        {
            lbl_virtualmachine.ForeColor = System.Drawing.Color.Green;
            lbl_virtualmachine.Text = "Succeed in zipping the virtual machine.you may click the right link.";
        }
        else
        {
            lbl_virtualmachine.ForeColor = System.Drawing.Color.Red;
            lbl_virtualmachine.Text = "Downloading the virtual machine, the wrong happened.";
            return;
        }
    }

    protected void ibtn_downloadmachine_Click(object sender, ImageClickEventArgs e)
    {
        string str_codePath = getApplicationPath();
        string str_fileName = "Virtual_Machine.rar";
        string str_virtualmachine = str_codePath + "\\DownLoad\\VirtualMachine\\" + str_fileName;

        if (downloadSourceCode(str_virtualmachine, str_fileName))
        {
        }
        else
        {
            lbl_virtualmachine.ForeColor = System.Drawing.Color.Red;
            lbl_virtualmachine.Text = "The file has been removed out or the path has been changed. file doesn't exist.";
        }
    }

    private bool zipVirtualMachine(string str_machinepath, string str_machinename)
    {
        string rar;
        string args;
        ProcessStartInfo procStart;
        Process process;

        string machinedestinationpath = str_machinepath + "\\" + str_machinename;

        try
        {
            if (System.IO.File.Exists(machinedestinationpath))
            {
                System.IO.File.Delete(machinedestinationpath);
            }
        }
        catch (Exception ex)
        {
            log.WriteLog(LogUtility.LogErrorLevel.LOG_CRITICAL, "Machine path, '" + machinedestinationpath + "', is wrong,Exception:" + ex.Message);
            return false;
        }

        try
        {
            rar = ConfigurationSettings.AppSettings["winrarpath"].ToString();
            args = "a -inul -y -o+ -ep1 " + machinedestinationpath + " " + str_machinepath;
            procStart = new ProcessStartInfo();
            procStart.FileName = rar;
            procStart.Arguments = args;
            procStart.WorkingDirectory = Server.MapPath("");
            process = new Process();
            process.StartInfo = procStart;
            process.Start();

            return true;
        }
        catch (Exception ex)
        {
            log.WriteLog(LogUtility.LogErrorLevel.LOG_CRITICAL, "Zipping the virtual machine, the wrong happened. Exception: " + ex.Message);
            Response.Close();

            return false;
        }
    }

    //Source Code
    protected void ibtn_sourcecode_Click(object sender, ImageClickEventArgs e)
    {
        string str_codePath = getApplicationPath();
        string str_fileName = "SiemensHPCRM_Application.rar";

        if (zipSourceCode(str_codePath, str_fileName))
        {
            lbl_sourcecode.ForeColor = System.Drawing.Color.Green;
            lbl_sourcecode.Text = "Succeed in zipping the application.you may click the right link.";
        }
        else
        {
            lbl_sourcecode.ForeColor = System.Drawing.Color.Red;
            lbl_sourcecode.Text = "Downloading the application, the wrong happened.";
            return;
        }
    }

    protected void ibtn_downloadcode_Click(object sender, ImageClickEventArgs e)
    {
        string str_codePath = getApplicationPath();
        string str_fileName = "SiemensHPCRM_Application.rar";
        string codedestinationpath = str_codePath + "\\DownLoad\\Code\\" + str_fileName;
        if (downloadSourceCode(codedestinationpath, str_fileName))
        {
        }
        else
        {
            lbl_sourcecode.ForeColor = System.Drawing.Color.Red;
            lbl_sourcecode.Text = "The file has been removed out or the path has been changed. file doesn't exist.";
        }
    }

    private bool zipSourceCode(string str_applicationpath, string str_filename)
    {
        string rar;
        string args;
        ProcessStartInfo procStart;
        Process process;

        string codedestinationpath = str_applicationpath + "\\DownLoad\\Code\\" + str_filename;

        try
        {
            if (System.IO.File.Exists(codedestinationpath))
            {
                System.IO.File.Delete(codedestinationpath);
            }
        }
        catch (Exception ex)
        {
            log.WriteLog(LogUtility.LogErrorLevel.LOG_CRITICAL, "Application path, '" + codedestinationpath + "', is wrong,Exception:" + ex.Message);
            return false;
        }

        try
        {
            rar = ConfigurationSettings.AppSettings["winrarpath"].ToString();
            args = "a -inul -y -o+ -ep1 " + str_applicationpath + "\\Download\\Code\\" + str_filename + " " + str_applicationpath;
            procStart = new ProcessStartInfo();
            procStart.FileName = rar;
            procStart.Arguments = args;
            procStart.WorkingDirectory = Server.MapPath("");
            process = new Process();
            process.StartInfo = procStart;
            process.Start();

            return true;
        }
        catch (Exception ex)
        {
            log.WriteLog(LogUtility.LogErrorLevel.LOG_CRITICAL, "Zipping the application, the wrong happened. Exception: " + ex.Message);
            Response.Close();

            return false;
        }
    }

    private bool downloadSourceCode(string str_filepath, string str_filename)
    {
        return downloadFile(str_filepath, str_filename);
    }

    //DataBase
    protected void ibtn_database_Click(object sender, ImageClickEventArgs e)
    {
        string code_path = getApplicationPath();
        string str_databasepath = code_path + "\\DownLoad\\DataBase";
        string str_databasename = "DataBaseDatasource.sql";
        string str_database = "DataBase.rar";

        downloadDatabase(str_databasepath, str_databasename);
        zipDataBase(str_databasepath, str_database);
        lbl_database.ForeColor = System.Drawing.Color.Green;
        lbl_database.Text = "Succeed in zipping the database.you may click the right link.";
    }

    protected void ibtn_downloaddatabase_Click(object sender, ImageClickEventArgs e)
    {
        string str_codePath = getApplicationPath();
        string str_fileName = "DataBase.rar";
        string codedestinationpath = str_codePath + "\\DownLoad\\DataBase\\" + str_fileName;
        if (downloadSourceCode(codedestinationpath, str_fileName))
        {
        }
        else
        {
            lbl_database.ForeColor = System.Drawing.Color.Red;
            lbl_database.Text = "The file has been removed out or the path has been changed. file doesn't exist.";
        }
    }

    private bool zipDataBase(string str_databasepath, string str_database)
    {
        string rar;
        string args;
        ProcessStartInfo procStart;
        Process process;

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
            log.WriteLog(LogUtility.LogErrorLevel.LOG_CRITICAL, "Database path, '" + databasedestinationpath + "', is wrong,Exception:" + ex.Message);
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
            process = new Process();
            process.StartInfo = procStart;
            process.Start();

            return true;
        }
        catch (Exception ex)
        {
            log.WriteLog(LogUtility.LogErrorLevel.LOG_CRITICAL, "Zipping the database, the wrong happened. Exception: " + ex.Message);
            Response.Close();

            return false;
        }
    }

    private void downloadDatabase(string str_databasepath, string str_databasename)
    {
        string sql_path = str_databasepath + "\\" + str_databasename;
        try
        {
            if (System.IO.File.Exists(sql_path))
            {
                System.IO.File.Delete(sql_path);
            }
        }
        catch (Exception ex)
        {
            log.WriteLog(LogUtility.LogErrorLevel.LOG_CRITICAL, "Database data source file path, '" + sql_path + "' , is wrong ,Exception:" + ex.Message);
        }

        preparedata(str_databasepath, str_databasename);
    }

    private void insertTable(string str_sql, string str_filepath, string str_filename)
    {
        string sql_path = str_filepath + "\\" + str_filename;

        try
        {
            if (!System.IO.File.Exists(sql_path))
            {
                System.IO.FileStream f = System.IO.File.Create(sql_path);
                f.Close();
            }
            System.IO.StreamWriter f2 = new System.IO.StreamWriter(sql_path, true, System.Text.Encoding.GetEncoding("gb2312"));
            f2.WriteLine(str_sql);
            f2.Close();
            f2.Dispose();
        }
        catch (Exception ex)
        {
            log.WriteLog(LogUtility.LogErrorLevel.LOG_CRITICAL, "Exception:" + ex.Message);
        }
    }

    private void preparedata(string str_filepath, string str_filename)
    {
        //User
        string sql_user = "SELECT UserID, FirstName,LastName,Alias,Abbr,RoleID, CONVERT(varchar(15),StartDate,23) AS StartDate,CONVERT(varchar(15),EndDate,23) AS EndDate,Email,Gender,Deleted FROM [User]";
        SqlDataReader rd_user = helper.ExecuteReader(CommandType.Text, sql_user, null);
        insertTable("/* * * * * *  Table [User]  * * * * * */", str_filepath, str_filename);
        insertTable("SET IDENTITY_INSERT [User] ON", str_filepath, str_filename);
        while (rd_user.Read())
        {
            string UserID = rd_user["UserID"].ToString().Trim();
            string FirstName = rd_user["FirstName"].ToString().Trim();
            string LastName = rd_user["LastName"].ToString().Trim();
            string Alias = rd_user["Alias"].ToString().Trim();
            string Abbr = rd_user["Abbr"].ToString().Trim();
            string RoleID = rd_user["RoleID"].ToString().Trim();
            string StartDate = rd_user["StartDate"].ToString().Trim();
            string EndDate = rd_user["EndDate"].ToString().Trim();
            string Email = rd_user["Email"].ToString().Trim();
            string Gender = rd_user["Gender"].ToString().Trim();
            string Deleted = rd_user["Deleted"].ToString().Trim();

            if (EndDate == "" || EndDate == null)
            {
                EndDate = "2099-12-31";
            }

            if (StartDate == "" || StartDate == null)
            {
                StartDate = System.DateTime.Now.Year.ToString() + "-" + System.DateTime.Now.Month.ToString() + "-" + System.DateTime.Now.Day.ToString();
            }

            FirstName = FirstName.Replace("'", "''");
            LastName = LastName.Replace("'", "''");

            string sql_insertUser = "INSERT INTO [User]([UserID],[FirstName],[LastName],[Alias],[Abbr],[RoleID],[StartDate],[EndDate],[Email],[Gender],[Deleted])"
                                  + "VALUES('" + UserID + "','" + FirstName + "','" + LastName + "','" + Alias + "','" + Abbr + "','" + RoleID + "','" + StartDate + "','" + EndDate + "','" + Email + "','" + Gender + "','" + Deleted + "')";
            insertTable(sql_insertUser, str_filepath, str_filename);
        }
        insertTable("SET IDENTITY_INSERT [User] OFF", str_filepath, str_filename);
        insertTable("/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */", str_filepath, str_filename);

        //segment
        string sql_segment = "SELECT ID,Description,Abbr,Deleted FROM [Segment]";
        SqlDataReader rd_segment = helper.ExecuteReader(CommandType.Text, sql_segment, null);
        insertTable("/* * * * * *  Table [Segment]  * * * * * */", str_filepath, str_filename);
        insertTable("SET IDENTITY_INSERT [Segment] ON", str_filepath, str_filename);
        while (rd_segment.Read())
        {
            string ID = rd_segment["ID"].ToString().Trim();
            string Description = rd_segment["Description"].ToString().Trim();
            string Abbr = rd_segment["Abbr"].ToString().Trim();
            string Deleted = rd_segment["Deleted"].ToString().Trim();

            Description = Description.Replace("'", "''");

            string sql_insertSegment = "INSERT INTO [Segment]([ID],[Description],[Abbr],[Deleted])"
                                  + "VALUES('" + ID + "','" + Description + "','" + Abbr + "','" + Deleted + "')";
            insertTable(sql_insertSegment, str_filepath, str_filename);
        }
        insertTable("SET IDENTITY_INSERT [Segment] OFF", str_filepath, str_filename);
        insertTable("/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */", str_filepath, str_filename);

        //Product
        string sql_product = "SELECT [ID],[Description],[Abbr],[Deleted] FROM [Product]";
        SqlDataReader rd_product = helper.ExecuteReader(CommandType.Text, sql_product, null);
        insertTable("/* * * * * *  Table [Product]  * * * * * */", str_filepath, str_filename);
        insertTable("SET IDENTITY_INSERT [Product] ON", str_filepath, str_filename);
        while (rd_product.Read())
        {
            string ID = rd_product["ID"].ToString().Trim();
            string Description = rd_product["Description"].ToString().Trim();
            string Abbr = rd_product["Abbr"].ToString().Trim();
            string Deleted = rd_product["Deleted"].ToString().Trim();

            Description = Description.Replace("'", "''");

            string sql_insertProduct = "INSERT INTO [Product]([ID],[Description],[Abbr],[Deleted])"
                                  + "VALUES('" + ID + "','" + Description + "','" + Abbr + "','" + Deleted + "')";
            insertTable(sql_insertProduct, str_filepath, str_filename);
        }
        insertTable("SET IDENTITY_INSERT [Product] OFF", str_filepath, str_filename);
        insertTable("/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */", str_filepath, str_filename);

        //segment product
        string sql_segment_product = "SELECT SegmentID,ProductID,Deleted FROM [Segment_Product]";
        SqlDataReader rd_segment_product = helper.ExecuteReader(CommandType.Text, sql_segment_product, null);
        insertTable("/* * * * * *  Table [Segment_Product]  * * * * * */", str_filepath, str_filename);
        while (rd_segment_product.Read())
        {
            string SegmentID = rd_segment_product["SegmentID"].ToString().Trim();
            string ProductID = rd_segment_product["ProductID"].ToString().Trim();
            string Deleted = rd_segment_product["Deleted"].ToString().Trim();

            string sql_insertSegment_Product = "INSERT INTO [Segment_Product]([SegmentID],[ProductID],[Deleted])"
                                  + "VALUES('" + SegmentID + "','" + ProductID + "','" + Deleted + "')";
            insertTable(sql_insertSegment_Product, str_filepath, str_filename);
        }
        insertTable("/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */", str_filepath, str_filename);

        //customer name
        string sql_customername = "SELECT ID,Name,Deleted FROM [CustomerName]";
        SqlDataReader rd_customername = helper.ExecuteReader(CommandType.Text, sql_customername, null);
        insertTable("/* * * * * *  Table [CustomerName]  * * * * * */", str_filepath, str_filename);
        insertTable("SET IDENTITY_INSERT [CustomerName] ON", str_filepath, str_filename);
        while (rd_customername.Read())
        {
            string ID = rd_customername["ID"].ToString().Trim();
            string Name = rd_customername["Name"].ToString().Trim();
            string Deleted = rd_customername["Deleted"].ToString().Trim();

            Name = Name.Replace("'", "''");

            string sql_insertCustomer = "INSERT INTO [CustomerName]([ID],[Name],Deleted)"
                                  + "VALUES('" + ID + "','" + Name + "','" + Deleted + "')";
            insertTable(sql_insertCustomer, str_filepath, str_filename);
        }
        insertTable("SET IDENTITY_INSERT [CustomerName] OFF", str_filepath, str_filename);
        insertTable("/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */", str_filepath, str_filename);

        //customer type
        string sql_customertype = "SELECT ID,Name,Deleted FROM [CustomerType]";
        SqlDataReader rd_customertype = helper.ExecuteReader(CommandType.Text, sql_customertype, null);
        insertTable("/* * * * * *  Table [CustomerType]  * * * * * */", str_filepath, str_filename);
        insertTable("SET IDENTITY_INSERT [CustomerType] ON", str_filepath, str_filename);
        while (rd_customertype.Read())
        {
            string ID = rd_customertype["ID"].ToString().Trim();
            string Name = rd_customertype["Name"].ToString().Trim();
            string Deleted = rd_customertype["Deleted"].ToString().Trim();

            Name = Name.Replace("'", "''");

            string sql_insertCustomer = "INSERT INTO [CustomerType]([ID],[Name],Deleted)"
                                  + "VALUES('" + ID + "','" + Name + "','" + Deleted + "')";
            insertTable(sql_insertCustomer, str_filepath, str_filename);
        }
        insertTable("SET IDENTITY_INSERT [CustomerType] OFF", str_filepath, str_filename);
        insertTable("/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */", str_filepath, str_filename);

        //customer
        string sql_customer = "SELECT ID,NameID,TypeID,CountryID,City,Address,Department,Deleted FROM [Customer]";
        SqlDataReader rd_customer = helper.ExecuteReader(CommandType.Text, sql_customer, null);
        insertTable("/* * * * * *  Table [Customer]  * * * * * */", str_filepath, str_filename);
        insertTable("SET IDENTITY_INSERT [Customer] ON", str_filepath, str_filename);
        while (rd_customer.Read())
        {
            string ID = rd_customer["ID"].ToString().Trim();
            string NameID = rd_customer["NameID"].ToString().Trim();
            string TypeID = rd_customer["TypeID"].ToString().Trim();
            string CountryID = rd_customer["CountryID"].ToString().Trim();
            string City = rd_customer["City"].ToString().Trim();
            string Address = rd_customer["Address"].ToString().Trim();
            string Department = rd_customer["Department"].ToString().Trim();
            string Deleted = rd_customer["Deleted"].ToString().Trim();

            City = City.Replace("'", "''");
            Address = Address.Replace("'", "''");
            Department = Department.Replace("'", "''");

            string sql_insertCustomer = "INSERT INTO [Customer]([ID],[NameID],[TypeID],[CountryID],[City],[Address],[Department],Deleted)"
                                  + "VALUES('" + ID + "','" + NameID + "','" + TypeID + "','" + CountryID + "','" + City + "','" + Address + "','" + Department + "','" + Deleted + "')";
            insertTable(sql_insertCustomer, str_filepath, str_filename);
        }
        insertTable("SET IDENTITY_INSERT [Customer] OFF", str_filepath, str_filename);
        insertTable("/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */", str_filepath, str_filename);

        //operation
        string sql_operation = "SELECT [ID],[Name],AbbrL,Abbr,CurrencyID,Deleted FROM [Operation]";
        SqlDataReader rd_operation = helper.ExecuteReader(CommandType.Text, sql_operation, null);
        insertTable("/* * * * * *  Table [Operation]  * * * * * */", str_filepath, str_filename);
        insertTable("SET IDENTITY_INSERT [Operation] ON", str_filepath, str_filename);
        while (rd_operation.Read())
        {
            string ID = rd_operation["ID"].ToString().Trim();
            string Name = rd_operation["Name"].ToString().Trim();
            string AbbrL = rd_operation["AbbrL"].ToString().Trim();
            string Abbr = rd_operation["Abbr"].ToString().Trim();
            string CurrencyID = rd_operation["CurrencyID"].ToString().Trim();
            string Deleted = rd_operation["Deleted"].ToString().Trim();

            Name = Name.Replace("'", "''");

            string sql_insertOperation = "INSERT INTO [Operation]( [ID],[Name],AbbrL,Abbr,CurrencyID,Deleted)"
                                  + "VALUES('" + ID + "','" + Name + "','" + AbbrL + "','" + Abbr + "','" + CurrencyID + "','" + Deleted + "')";
            insertTable(sql_insertOperation, str_filepath, str_filename);
        }
        insertTable("SET IDENTITY_INSERT [Operation] OFF", str_filepath, str_filename);
        insertTable("/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */", str_filepath, str_filename);

        //operation segment
        string sql_operation_segment = "SELECT OperationID,SegmentID,Deleted FROM [Operation_Segment]";
        SqlDataReader rd_operation_segment = helper.ExecuteReader(CommandType.Text, sql_operation_segment, null);
        insertTable("/* * * * * *  Table [Operation_Segment]  * * * * * */", str_filepath, str_filename);
        while (rd_operation_segment.Read())
        {
            string OperationID = rd_operation_segment["OperationID"].ToString().Trim();
            string SegmentID = rd_operation_segment["SegmentID"].ToString().Trim();
            string Deleted = rd_operation_segment["Deleted"].ToString().Trim();

            string sql_insertOperation_segment = "INSERT INTO [Operation_Segment](OperationID,SegmentID,Deleted)"
                                  + "VALUES('" + OperationID + "','" + SegmentID + "','" + Deleted + "')";
            insertTable(sql_insertOperation_segment, str_filepath, str_filename);
        }
        insertTable("/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */", str_filepath, str_filename);

        //user operation
        string sql_user_operation = "SELECT OperationID,UserID,Deleted FROM [User_Operation]";
        SqlDataReader rd_user_operation = helper.ExecuteReader(CommandType.Text, sql_user_operation, null);
        insertTable("/* * * * * *  Table [User_Operation]  * * * * * */", str_filepath, str_filename);
        while (rd_user_operation.Read())
        {
            string OperationID = rd_user_operation["OperationID"].ToString().Trim();
            string UserID = rd_user_operation["UserID"].ToString().Trim();
            string Deleted = rd_user_operation["Deleted"].ToString().Trim();

            string sql_insertOperation_marketingmgr = "INSERT INTO [User_Operation](OperationID,UserID,Deleted)"
                                  + "VALUES('" + OperationID + "','" + UserID + "','" + Deleted + "')";
            insertTable(sql_insertOperation_marketingmgr, str_filepath, str_filename);
        }
        insertTable("/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */", str_filepath, str_filename);

        //role
        string sql_role = "SELECT ID,Name FROM [Role]";
        SqlDataReader rd_role = helper.ExecuteReader(CommandType.Text, sql_role, null);
        insertTable("/* * * * * *  Table [Role]  * * * * * */", str_filepath, str_filename);
        while (rd_role.Read())
        {
            string ID = rd_role["ID"].ToString().Trim();
            string Name = rd_role["Name"].ToString().Trim();

            string sql_insertRole = "INSERT INTO [Role]([ID],[Name])"
                                  + "VALUES('" + ID + "','" + Name + "')";
            insertTable(sql_insertRole, str_filepath, str_filename);
        }
        insertTable("/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */", str_filepath, str_filename);

        //salesorg
        string sql_salesorg = "SELECT ID,Name,Abbr,CurrencyID,Deleted FROM [SalesOrg]";
        SqlDataReader rd_salesorg = helper.ExecuteReader(CommandType.Text, sql_salesorg, null);
        insertTable("/* * * * * *  Table [SalesOrg]  * * * * * */", str_filepath, str_filename);
        insertTable("SET IDENTITY_INSERT [SalesOrg] ON", str_filepath, str_filename);
        while (rd_salesorg.Read())
        {
            string ID = rd_salesorg["ID"].ToString().Trim();
            string Name = rd_salesorg["Name"].ToString().Trim();
            string Abbr = rd_salesorg["Abbr"].ToString().Trim();
            string CurrencyID = rd_salesorg["CurrencyID"].ToString().Trim();
            string Deleted = rd_salesorg["Deleted"].ToString().Trim();

            Name = Name.Replace("'", "''");

            string sql_insertSalesOrgMgr = "INSERT INTO [SalesOrg]([ID],[Name],[Abbr],CurrencyID,Deleted)"
                                  + "VALUES('" + ID + "','" + Name + "','" + Abbr + "','" + CurrencyID + "','" + Deleted + "')";
            insertTable(sql_insertSalesOrgMgr, str_filepath, str_filename);
        }
        insertTable("SET IDENTITY_INSERT [SalesOrg] OFF", str_filepath, str_filename);
        insertTable("/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */", str_filepath, str_filename);

        //salesorg segment
        string sql_salesorg_segment = "SELECT SalesOrgID,SegmentID,Deleted FROM [SalesOrg_Segment]";
        SqlDataReader rd_salesorg_segment = helper.ExecuteReader(CommandType.Text, sql_salesorg_segment, null);
        insertTable("/* * * * * *  Table [SalesOrg_Segment]  * * * * * */", str_filepath, str_filename);
        while (rd_salesorg_segment.Read())
        {
            string SalesOrgID = rd_salesorg_segment["SalesOrgID"].ToString().Trim();
            string SegmentID = rd_salesorg_segment["SegmentID"].ToString().Trim();
            string Deleted = rd_salesorg_segment["Deleted"].ToString().Trim();

            string sql_insertSalesOrg_Segment = "INSERT INTO [SalesOrg_Segment](SalesOrgID,SegmentID,Deleted)"
                                  + "VALUES('" + SalesOrgID + "','" + SegmentID + "','" + Deleted + "')";
            insertTable(sql_insertSalesOrg_Segment, str_filepath, str_filename);
        }
        insertTable("/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */", str_filepath, str_filename);

        //salesorg user
        string sql_salesorg_rsm = "SELECT SalesOrgID,UserID,Deleted FROM [SalesOrg_User]";
        SqlDataReader rd_salesorg_rsm = helper.ExecuteReader(CommandType.Text, sql_salesorg_rsm, null);
        insertTable("/* * * * * *  Table [SalesOrg_User]  * * * * * */", str_filepath, str_filename);
        while (rd_salesorg_rsm.Read())
        {
            string SalesOrgID = rd_salesorg_rsm["SalesOrgID"].ToString().Trim();
            string UserID = rd_salesorg_rsm["UserID"].ToString().Trim();
            string Deleted = rd_salesorg_rsm["Deleted"].ToString().Trim();

            string sql_insertSalesOrg_RSM = "INSERT INTO [SalesOrg_User](SalesOrgID,UserID,Deleted)"
                                  + "VALUES('" + SalesOrgID + "','" + UserID + "','" + Deleted + "')";
            insertTable(sql_insertSalesOrg_RSM, str_filepath, str_filename);
        }
        insertTable("/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */", str_filepath, str_filename);

        //user segment
        string sql_rsm_segment = "SELECT UserID,SegmentID,Deleted FROM [User_Segment]";
        SqlDataReader rd_rsm_segment = helper.ExecuteReader(CommandType.Text, sql_rsm_segment, null);
        insertTable("/* * * * * *  Table [User_Segment]  * * * * * */", str_filepath, str_filename);
        while (rd_rsm_segment.Read())
        {
            string UserID = rd_rsm_segment["UserID"].ToString().Trim();
            string SegmentID = rd_rsm_segment["SegmentID"].ToString().Trim();
            string Deleted = rd_rsm_segment["Deleted"].ToString().Trim();

            string sql_insertRSM_Segment = "INSERT INTO [User_Segment](UserID,SegmentID,Deleted)"
                                  + "VALUES('" + UserID + "','" + SegmentID + "','" + Deleted + "')";
            insertTable(sql_insertRSM_Segment, str_filepath, str_filename);
        }
        insertTable("/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */", str_filepath, str_filename);

        //user country
        string sql_rsm_country = "SELECT UserID,CountryID,Deleted FROM [User_Country]";
        SqlDataReader rd_rsm_country = helper.ExecuteReader(CommandType.Text, sql_rsm_country, null);
        insertTable("/* * * * * *  Table [User_Country]  * * * * * */", str_filepath, str_filename);
        while (rd_rsm_country.Read())
        {
            string UserID = rd_rsm_country["UserID"].ToString().Trim();
            string CountryID = rd_rsm_country["CountryID"].ToString().Trim();
            string Deleted = rd_rsm_country["Deleted"].ToString().Trim();

            string sql_insertRSM_Country = "INSERT INTO [User_Country](UserID,CountryID,Deleted)"
                                  + "VALUES('" + UserID + "','" + CountryID + "','" + Deleted + "')";
            insertTable(sql_insertRSM_Country, str_filepath, str_filename);
        }
        insertTable("/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */", str_filepath, str_filename);

        //user status
        string sql_user_status = "SELECT UserID,SegmentID,Status FROM [User_Status]";
        SqlDataReader rd_user_status = helper.ExecuteReader(CommandType.Text, sql_user_status, null);
        insertTable("/* * * * * *  Table [User_Status]  * * * * * */", str_filepath, str_filename);
        while (rd_user_status.Read())
        {
            string UserID = rd_user_status["UserID"].ToString().Trim();
            string SegmentID = rd_user_status["SegmentID"].ToString().Trim();
            string Status = rd_user_status["Status"].ToString().Trim();

            string sql_insertUser_Status = "INSERT INTO [User_Status](UserID,SegmentID,Status)"
                                  + "VALUES('" + UserID + "','" + SegmentID + "','" + Status + "')";
            insertTable(sql_insertUser_Status, str_filepath, str_filename);
        }
        insertTable("/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */", str_filepath, str_filename);

        //sales channel
        string sql_saleschannel = "SELECT ID,Name,Deleted FROM [SalesChannel]";
        SqlDataReader rd_saleschannel = helper.ExecuteReader(CommandType.Text, sql_saleschannel, null);
        insertTable("/* * * * * *  Table [SalesChannel]  * * * * * */", str_filepath, str_filename);
        insertTable("SET IDENTITY_INSERT [SalesChannel] ON", str_filepath, str_filename);
        while (rd_saleschannel.Read())
        {
            string ID = rd_saleschannel["ID"].ToString().Trim();
            string Name = rd_saleschannel["Name"].ToString().Trim();
            string Deleted = rd_saleschannel["Deleted"].ToString().Trim();

            Name = Name.Replace("'", "''");

            string sql_insertSalesChannel = "INSERT INTO [SalesChannel]([ID],[Name],Deleted)"
                                  + "VALUES('" + ID + "','" + Name + "','" + Deleted + "')";
            insertTable(sql_insertSalesChannel, str_filepath, str_filename);
        }
        insertTable("SET IDENTITY_INSERT [Region] OFF", str_filepath, str_filename);
        insertTable("/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */", str_filepath, str_filename);

        //project
        string sql_project = "SELECT [ID],[Name],CustomerNameID,PoSID,Value,Probability,PoDID,CurrencyID,Comments,Deleted FROM [Project]";
        SqlDataReader rd_project = helper.ExecuteReader(CommandType.Text, sql_project, null);
        insertTable("/* * * * * *  Table [Project]  * * * * * */", str_filepath, str_filename);
        insertTable("SET IDENTITY_INSERT [Project] ON", str_filepath, str_filename);
        while (rd_project.Read())
        {
            string ID = rd_project["ID"].ToString().Trim();
            string Name = rd_project["Name"].ToString().Trim();
            string CustomerNameID = rd_project["CustomerNameID"].ToString().Trim();
            string PoSID = rd_project["PoSID"].ToString().Trim();
            string Value = rd_project["Value"].ToString().Trim();
            string Probability = rd_project["Probability"].ToString().Trim();
            string PoDID = rd_project["PoDID"].ToString().Trim();
            string CurrencyID = rd_project["CurrencyID"].ToString().Trim();
            string Comments = rd_project["Comments"].ToString().Trim();
            string Deleted = rd_project["Deleted"].ToString().Trim();

            Name = Name.Replace("'", "''");

            string sql_insertSalesChannel = "INSERT INTO [Project]([ID],[Name],CustomerNameID,PoSID,Value,Probability,PoDID,CurrencyID,Comments,Deleted)"
                                  + "VALUES('" + ID + "','" + Name + "','" + CustomerNameID + "','" + PoSID + "','" + Value + "','" + Probability + "','" + PoDID + "','" + CurrencyID + "','" + Comments + "','" + Deleted + "')";
            insertTable(sql_insertSalesChannel, str_filepath, str_filename);
        }
        insertTable("SET IDENTITY_INSERT [Project] OFF", str_filepath, str_filename);
        insertTable("/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */", str_filepath, str_filename);

        //region
        string sql_region = "SELECT ID,Name,Deleted FROM [Region]";
        SqlDataReader rd_region = helper.ExecuteReader(CommandType.Text, sql_region, null);
        insertTable("/* * * * * *  Table [Region]  * * * * * */", str_filepath, str_filename);
        insertTable("SET IDENTITY_INSERT [Region] ON", str_filepath, str_filename);
        while (rd_region.Read())
        {
            string ID = rd_region["ID"].ToString().Trim();
            string Name = rd_region["Name"].ToString().Trim();
            string Deleted = rd_region["Deleted"].ToString().Trim();

            Name = Name.Replace("'", "''");

            string sql_insertRegion = "INSERT INTO [Region]([ID],[Name],Deleted)"
                                  + "VALUES('" + ID + "','" + Name + "','" + Deleted + "')";
            insertTable(sql_insertRegion, str_filepath, str_filename);
        }
        insertTable("SET IDENTITY_INSERT [Region] OFF", str_filepath, str_filename);
        insertTable("/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */", str_filepath, str_filename);

        //cluster
        string sql_cluster = "SELECT ID,Name,Deleted FROM [Cluster]";
        SqlDataReader rd_cluster = helper.ExecuteReader(CommandType.Text, sql_cluster, null);
        insertTable("/* * * * * *  Table [Cluster]  * * * * * */", str_filepath, str_filename);
        insertTable("SET IDENTITY_INSERT [Cluster] ON", str_filepath, str_filename);
        while (rd_cluster.Read())
        {
            string ID = rd_cluster["ID"].ToString().Trim();
            string Name = rd_cluster["Name"].ToString().Trim();
            string Deleted = rd_cluster["Deleted"].ToString().Trim();

            Name = Name.Replace("'", "''");

            string sql_insertRegion = "INSERT INTO [Cluster]([ID],[Name],Deleted)"
                                  + "VALUES('" + ID + "','" + Name + "','" + Deleted + "')";
            insertTable(sql_insertRegion, str_filepath, str_filename);
        }
        insertTable("SET IDENTITY_INSERT [Cluster] OFF", str_filepath, str_filename);
        insertTable("/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */", str_filepath, str_filename);

        //subregion
        string sql_subregion = "SELECT ID,Name,Deleted FROM [SubRegion]";
        SqlDataReader rd_subregion = helper.ExecuteReader(CommandType.Text, sql_subregion, null);
        insertTable("/* * * * * *  Table [SubRegion]  * * * * * */", str_filepath, str_filename);
        insertTable("SET IDENTITY_INSERT [SubRegion] ON", str_filepath, str_filename);
        while (rd_subregion.Read())
        {
            string ID = rd_subregion["ID"].ToString().Trim();
            string Name = rd_subregion["Name"].ToString().Trim();
            string Deleted = rd_subregion["Deleted"].ToString().Trim();

            Name = Name.Replace("'", "''");

            string sql_insertSubRegion = "INSERT INTO [SubRegion]([ID],[Name],Deleted)"
                                  + "VALUES('" + ID + "','" + Name + "','" + Deleted + "')";
            insertTable(sql_insertSubRegion, str_filepath, str_filename);
        }
        insertTable("SET IDENTITY_INSERT [SubRegion] OFF", str_filepath, str_filename);
        insertTable("/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */", str_filepath, str_filename);

        //country
        string sql_country = "SELECT ID,Name,ISO_Code,Deleted FROM [Country]";
        SqlDataReader rd_country = helper.ExecuteReader(CommandType.Text, sql_country, null);
        insertTable("/* * * * * *  Table [Country]  * * * * * */", str_filepath, str_filename);
        insertTable("SET IDENTITY_INSERT [Country] ON", str_filepath, str_filename);
        while (rd_country.Read())
        {
            string ID = rd_country["ID"].ToString().Trim();
            string Name = rd_country["Name"].ToString().Trim();
            string ISO_Code = rd_country["ISO_Code"].ToString().Trim();
            string Deleted = rd_country["Deleted"].ToString().Trim();

            Name = Name.Replace("'", "''");

            string sql_insertCountry = "INSERT INTO [Country]([ID],[Name],ISO_Code,Deleted)"
                                  + "VALUES('" + ID + "','" + Name + "','" + ISO_Code + "','" + Deleted + "')";
            insertTable(sql_insertCountry, str_filepath, str_filename);
        }
        insertTable("SET IDENTITY_INSERT [Country] OFF", str_filepath, str_filename);
        insertTable("/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */", str_filepath, str_filename);

        //region cluster
        string sql_region_cluster = "SELECT RegionID,ClusterID,Deleted FROM [Region_Cluster]";
        SqlDataReader rd_region_cluster = helper.ExecuteReader(CommandType.Text, sql_region_cluster, null);
        insertTable("/* * * * * *  Table [Region_Cluster]  * * * * * */", str_filepath, str_filename);
        while (rd_region_cluster.Read())
        {
            string RegionID = rd_region_cluster["RegionID"].ToString().Trim();
            string ClusterID = rd_region_cluster["ClusterID"].ToString().Trim();
            string Deleted = rd_region_cluster["Deleted"].ToString().Trim();

            string sql_insertRegion_Cluster = "INSERT INTO [Region_Cluster](RegionID,ClusterID,Deleted)"
                                  + "VALUES('" + RegionID + "','" + ClusterID + "','" + Deleted + "')";
            insertTable(sql_insertRegion_Cluster, str_filepath, str_filename);
        }
        insertTable("/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */", str_filepath, str_filename);

        //cluster country
        string sql_cluster_country = "SELECT ClusterID,CountryID,Deleted FROM [Cluster_Country]";
        SqlDataReader rd_cluster_country = helper.ExecuteReader(CommandType.Text, sql_cluster_country, null);
        insertTable("/* * * * * *  Table [Cluster_Country]  * * * * * */", str_filepath, str_filename);
        while (rd_cluster_country.Read())
        {
            string ClusterID = rd_cluster_country["ClusterID"].ToString().Trim();
            string CountryID = rd_cluster_country["CountryID"].ToString().Trim();
            string Deleted = rd_cluster_country["Deleted"].ToString().Trim();

            string sql_insertCluster_Country = "INSERT INTO [Cluster_Country](ClusterID,CountryID,Deleted)"
                                  + "VALUES('" + ClusterID + "','" + CountryID + "','" + Deleted + "')";
            insertTable(sql_insertCluster_Country, str_filepath, str_filename);
        }
        insertTable("/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */", str_filepath, str_filename);

        //cluster country
        string sql_country_subregion = "SELECT SubRegionID,CountryID,Deleted FROM [Country_SubRegion]";
        SqlDataReader rd_country_subregion = helper.ExecuteReader(CommandType.Text, sql_country_subregion, null);
        insertTable("/* * * * * *  Table [Country_SubRegion]  * * * * * */", str_filepath, str_filename);
        while (rd_country_subregion.Read())
        {
            string SubRegionID = rd_country_subregion["SubRegionID"].ToString().Trim();
            string CountryID = rd_country_subregion["CountryID"].ToString().Trim();
            string Deleted = rd_country_subregion["Deleted"].ToString().Trim();

            string sql_insertCountry_SubRegion = "INSERT INTO [Country_SubRegion](SubRegionID,CountryID,Deleted)"
                                  + "VALUES('" + SubRegionID + "','" + CountryID + "','" + Deleted + "')";
            insertTable(sql_insertCountry_SubRegion, str_filepath, str_filename);
        }
        insertTable("/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */", str_filepath, str_filename);

        //setmeetingdate
        string sql_setmeetingdate = "SELECT CONVERT(varchar(15),MeetingDate,23) AS MeetingDate FROM [SetMeetingDate]";
        SqlDataReader rd_setmeetingdate = helper.ExecuteReader(CommandType.Text, sql_setmeetingdate, null);
        insertTable("/* * * * * *  Table [SetMeetingDate]  * * * * * */", str_filepath, str_filename);
        while (rd_setmeetingdate.Read())
        {
            string MeetingDate = rd_setmeetingdate["MeetingDate"].ToString().Trim();

            string sql_insertMeetingDate = "INSERT INTO [SetMeetingDate](MeetingDate)"
                                  + "VALUES('" + MeetingDate + "')";
            insertTable(sql_insertMeetingDate, str_filepath, str_filename);
        }
        insertTable("/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */", str_filepath, str_filename);

        //meetingdate
        string sql_meetingdate = "SELECT CONVERT(varchar(15),MeetingDate,23) AS MeetingDate FROM [MeetingDate]";
        SqlDataReader rd_meetingdate = helper.ExecuteReader(CommandType.Text, sql_meetingdate, null);
        insertTable("/* * * * * *  Table [MeetingDate]  * * * * * */", str_filepath, str_filename);
        while (rd_meetingdate.Read())
        {
            string MeetingDate = rd_meetingdate["MeetingDate"].ToString().Trim();

            string sql_insertMeetingDate = "INSERT INTO [MeetingDate](MeetingDate)"
                                  + "VALUES('" + MeetingDate + "')";
            insertTable(sql_insertMeetingDate, str_filepath, str_filename);
        }
        insertTable("/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */", str_filepath, str_filename);

        //log
        string sql_log = "SELECT Message,LogFileError FROM [Log]";
        SqlDataReader rd_log = helper.ExecuteReader(CommandType.Text, sql_log, null);
        insertTable("/* * * * * *  Table [Log]  * * * * * */", str_filepath, str_filename);
        while (rd_log.Read())
        {
            string Message = rd_log["Message"].ToString().Trim();
            string LogFileError = rd_log["LogFileError"].ToString().Trim();

            Message = Message.Replace("'", "''");
            LogFileError = LogFileError.Replace("'", "''");

            string sql_insertLog = "INSERT INTO [Log](Message,LogFileError)"
                                  + "VALUES('" + Message + "','" + LogFileError + "')";
            insertTable(sql_insertLog, str_filepath, str_filename);
        }
        insertTable("/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */", str_filepath, str_filename);

        //lock
        string sql_lock = "SELECT UserID,CONVERT(varchar(15),UnlockTime,23) AS UnlockTime FROM [Lock]";
        SqlDataReader rd_lock = helper.ExecuteReader(CommandType.Text, sql_lock, null);
        insertTable("/* * * * * *  Table [Lock]  * * * * * */", str_filepath, str_filename);
        while (rd_lock.Read())
        {
            string UserID = rd_lock["UserID"].ToString().Trim();
            string UnlockTime = rd_lock["UnlockTime"].ToString().Trim();

            string sql_insertLock = "INSERT INTO [Lock](UserID,UnlockTime)"
                                  + "VALUES('" + UserID + "','" + UnlockTime + "')";
            insertTable(sql_insertLock, str_filepath, str_filename);
        }
        insertTable("/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */", str_filepath, str_filename);

        //currency exchange
        string sql_curreny = "SELECT CurrencyID,Rate1,Rate2,CONVERT(varchar(15),TimeFlag,23) AS TimeFlag,Deleted FROM [Currency_Exchange]";
        SqlDataReader rd_curreny = helper.ExecuteReader(CommandType.Text, sql_curreny, null);
        insertTable("/* * * * * *  Table [Currency_Exchange]  * * * * * */", str_filepath, str_filename);
        while (rd_curreny.Read())
        {
            string CurrencyID = rd_curreny["CurrencyID"].ToString().Trim();
            string Rate1 = rd_curreny["Rate1"].ToString().Trim();
            string Rate2 = rd_curreny["Rate2"].ToString().Trim();
            string TimeFlag = rd_curreny["TimeFlag"].ToString().Trim();
            string Deleted = rd_curreny["Deleted"].ToString().Trim();

            string sql_insertCurrency = "INSERT INTO [Currency_Exchange]([CurrencyID],Rate1,Rate2,TimeFlag,Deleted)"
                                  + "VALUES('" + CurrencyID + "','" + Rate1 + "','" + Rate2 + "','" + TimeFlag + "','" + Deleted + "')";
            insertTable(sql_insertCurrency, str_filepath, str_filename);
        }
        insertTable("/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */", str_filepath, str_filename);

        //currency
        string sql_currenyinfo = "SELECT ID,Name,Description,Deleted FROM [Currency]";
        SqlDataReader rd_currenyinfo = helper.ExecuteReader(CommandType.Text, sql_currenyinfo, null);
        insertTable("/* * * * * *  Table [Currency]  * * * * * */", str_filepath, str_filename);
        while (rd_currenyinfo.Read())
        {
            string ID = rd_currenyinfo["ID"].ToString().Trim();
            string Name = rd_currenyinfo["Name"].ToString().Trim();
            string Description = rd_currenyinfo["Description"].ToString().Trim();
            string Deleted = rd_currenyinfo["Deleted"].ToString().Trim();

            string sql_insertCurrency = "INSERT INTO [Currency]([ID],Name,Description,Deleted)"
                                  + "VALUES('" + ID + "','" + Name + "','" + Description + "','" + Deleted + "')";
            insertTable(sql_insertCurrency, str_filepath, str_filename);
        }
        insertTable("/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */", str_filepath, str_filename);

        //market
        string sql_market = "SELECT SegmentID,[CountryID],ThisYear,NextYear,AfterYear,CONVERT(varchar(15),TimeFlag,23) AS TimeFlag FROM [Market]";
        SqlDataReader rd_market = helper.ExecuteReader(CommandType.Text, sql_market, null);
        insertTable("/* * * * * *  Table [Market]  * * * * * */", str_filepath, str_filename);
        while (rd_market.Read())
        {
            string SegmentID = rd_market["SegmentID"].ToString().Trim();
            string CountryID = rd_market["CountryID"].ToString().Trim();
            string ThisYear = rd_market["ThisYear"].ToString().Trim();
            string NextYear = rd_market["NextYear"].ToString().Trim();
            string AfterYear = rd_market["AfterYear"].ToString().Trim();
            string TimeFlag = rd_market["TimeFlag"].ToString().Trim();

            string sql_insertMarket = "INSERT INTO [Market]([SegmentID],CountryID,ThisYear,NextYear,AfterYear,TimeFlag)"
                                  + "VALUES('" + SegmentID + "','" + CountryID + "','" + ThisYear + "','" + NextYear + "','" + AfterYear + "','" + TimeFlag + "')";
            insertTable(sql_insertMarket, str_filepath, str_filename);
        }
        insertTable("/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */", str_filepath, str_filename);

        //bookings
        string sql_bookings = "SELECT RSMID,SalesOrgID,CountryID,CustomerID,BookingY,DeliverY,SegmentID,ProductID,OperationID,ProjectID,Amount,Comments,CONVERT(varchar(15),TimeFlag,23) AS TimeFlag FROM [Bookings]";
        SqlDataReader rd_bookings = helper.ExecuteReader(CommandType.Text, sql_bookings, null);
        insertTable("/* * * * * *  Table [Bookings]  * * * * * */", str_filepath, str_filename);
        while (rd_bookings.Read())
        {
            string RSMID = rd_bookings["RSMID"].ToString().Trim();
            string SalesOrgID = rd_bookings["SalesOrgID"].ToString().Trim();
            string CountryID = rd_bookings["CountryID"].ToString().Trim();
            string CustomerID = rd_bookings["CustomerID"].ToString().Trim();
            string BookingY = rd_bookings["BookingY"].ToString().Trim();
            string DeliverY = rd_bookings["DeliverY"].ToString().Trim();
            string SegmentID = rd_bookings["SegmentID"].ToString().Trim();
            string ProductID = rd_bookings["ProductID"].ToString().Trim();
            string OperationID = rd_bookings["OperationID"].ToString().Trim();
            string ProjectID = rd_bookings["ProjectID"].ToString().Trim();
            string Amount = rd_bookings["Amount"].ToString().Trim();
            string Comments = rd_bookings["Comments"].ToString().Trim();
            string TimeFlag = rd_bookings["TimeFlag"].ToString().Trim();

            if (CustomerID == "" || CustomerID == null)
                CustomerID = "NULL";
            Comments = Comments.Replace("'", "''");

            string sql_insertBookings = "INSERT INTO [Bookings](RSMID,SalesOrgID,CountryID,CustomerID,BookingY,DeliverY,SegmentID,ProductID,OperationID,ProjectID,Amount,Comments,TimeFlag)"
                                  + "VALUES('" + RSMID + "','" + SalesOrgID + "','" + CountryID + "'," + CustomerID + ",'" + BookingY + "','" + DeliverY + "','" + SegmentID + "','" + ProductID + "','" + OperationID + "','" + ProjectID + "','" + Amount + "','" + Comments + "','" + TimeFlag + "')";
            insertTable(sql_insertBookings, str_filepath, str_filename);

        }
        insertTable("/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */", str_filepath, str_filename);

        //actual sales
        string sql_actualsales = "SELECT MarketingMgrID,OperationID,SegmentID,SalesOrgID,ProductID,Backlog,BacklogY,CONVERT(varchar(15),TimeFlag,23) AS TimeFlag FROM [ActualSalesandBL]";
        SqlDataReader rd_actualsales = helper.ExecuteReader(CommandType.Text, sql_actualsales, null);
        insertTable("/* * * * * *  Table [ActualSalesandBL]  * * * * * */", str_filepath, str_filename);
        while (rd_actualsales.Read())
        {
            string MarketingMgrID = rd_actualsales["MarketingMgrID"].ToString().Trim();
            string OperationID = rd_actualsales["OperationID"].ToString().Trim();
            string SegmentID = rd_actualsales["SegmentID"].ToString().Trim();
            string SalesOrgID = rd_actualsales["SalesOrgID"].ToString().Trim();
            string ProductID = rd_actualsales["ProductID"].ToString().Trim();
            string Backlog = rd_actualsales["Backlog"].ToString().Trim();
            string BacklogY = rd_actualsales["BacklogY"].ToString().Trim();
            string TimeFlag = rd_actualsales["TimeFlag"].ToString().Trim();

            string sql_insertBookings = "INSERT INTO [ActualSalesandBL](MarketingMgrID,OperationID,SegmentID,SalesOrgID,ProductID,Backlog,BacklogY,TimeFlag)"
                                  + "VALUES('" + MarketingMgrID + "','" + OperationID + "'," + SegmentID + ",'" + SalesOrgID + "','" + ProductID + "','" + Backlog + "','" + BacklogY + "','" + TimeFlag + "')";
            insertTable(sql_insertBookings, str_filepath, str_filename);
        }
        insertTable("/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */", str_filepath, str_filename);

        //report role
        string sql_reportrole = "SELECT RoleID, ViewName FROM [ReportRole]";
        SqlDataReader rd_reportrole = helper.ExecuteReader(CommandType.Text, sql_reportrole, null);
        insertTable("/* * * * * *  Table [ReportRole]  * * * * * */", str_filepath, str_filename);
        while (rd_reportrole.Read())
        {
            string RoleID = rd_reportrole["RoleID"].ToString().Trim();
            string ViewName = rd_reportrole["ViewName"].ToString().Trim();

            string sql_insertReport_Role = "INSERT INTO [ReportRole](RoleID,ViewName)"
                                  + "VALUES('" + RoleID + "','" + ViewName + "')";
            insertTable(sql_insertReport_Role, str_filepath, str_filename);
        }
        insertTable("/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */", str_filepath, str_filename);

        //report list
        string sql_reportlist = "SELECT ID,Name,Depiction,UserID,ViewName,CreateDate FROM [ReportList]";
        SqlDataReader rd_reportlist = helper.ExecuteReader(CommandType.Text, sql_reportlist, null);
        insertTable("/* * * * * *  Table [ReportList]  * * * * * */", str_filepath, str_filename);
        while (rd_reportlist.Read())
        {
            string ID = rd_reportlist["ID"].ToString().Trim();
            string Name = rd_reportlist["Name"].ToString().Trim();
            string Depiction = rd_reportlist["Depiction"].ToString().Trim();
            string UserID = rd_reportlist["UserID"].ToString().Trim();
            string ViewName = rd_reportlist["ViewName"].ToString().Trim();
            string CreateDate = rd_reportlist["CreateDate"].ToString().Trim();

            string sql_insertReport_List = "INSERT INTO [ReportList](ID,Name,Depiction,UserID,ViewName,CreateDate)"
                                  + "VALUES('" + ID + "','" + Name + "','" + Depiction + "','" + UserID + "','" + ViewName + "','" + CreateDate + "')";
            insertTable(sql_insertReport_List, str_filepath, str_filename);
        }
        insertTable("/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */", str_filepath, str_filename);

        //datadict
        string sql_datadict = "SELECT BigType,SmallType,Name,Sort,Special,Modifiability FROM [DataDict]";
        SqlDataReader rd_datadict = helper.ExecuteReader(CommandType.Text, sql_datadict, null);
        insertTable("/* * * * * *  Table [DataDict]  * * * * * */", str_filepath, str_filename);
        while (rd_datadict.Read())
        {
            string BigType = rd_datadict["BigType"].ToString().Trim();
            string SmallType = rd_datadict["SmallType"].ToString().Trim();
            string Name = rd_datadict["Name"].ToString().Trim();
            string Sort = rd_datadict["Sort"].ToString().Trim();
            string Special = rd_datadict["Special"].ToString().Trim();
            string Modifiability = rd_datadict["Modifiability"].ToString().Trim();

            string sql_insertDataDict = "INSERT INTO [DataDict](BigType,SmallType,Name,Sort,Special,CreateDate)"
                                  + "VALUES('" + BigType + "','" + SmallType + "','" + Name + "','" + Sort + "','" + Special + "','" + Modifiability + "')";
            insertTable(sql_insertDataDict, str_filepath, str_filename);
        }
        insertTable("/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */", str_filepath, str_filename);

        //report value
        string sql_reportvalue = "SELECT FieldID,ID,FieldName,FieldType,FieldLength,Operator,Condition1,Condition2,Sort FROM [ReportValue]";
        SqlDataReader rd_reportvalue = helper.ExecuteReader(CommandType.Text, sql_reportvalue, null);
        insertTable("/* * * * * *  Table [ReportValue]  * * * * * */", str_filepath, str_filename);
        while (rd_reportvalue.Read())
        {
            string FieldID = rd_reportvalue["FieldID"].ToString().Trim();
            string ID = rd_reportvalue["ID"].ToString().Trim();
            string FieldName = rd_reportvalue["FieldName"].ToString().Trim();
            string FieldType = rd_reportvalue["FieldType"].ToString().Trim();
            string FieldLength = rd_reportvalue["FieldLength"].ToString().Trim();
            string Operator = rd_reportvalue["Operator"].ToString().Trim();
            string Condition1 = rd_reportvalue["Condition1"].ToString().Trim();
            string Condition2 = rd_reportvalue["Condition2"].ToString().Trim();
            string Sort = rd_reportvalue["Sort"].ToString().Trim();

            string sql_insertReportValue = "INSERT INTO [ReportValue](FieldID,ID,FieldName,FieldType,FieldLength,Operator,Condition1,Condition2,Sort)"
                                  + "VALUES('" + FieldID + "','" + ID + "','" + FieldName + "','" + FieldType + "','" + FieldLength + "','" + Operator + "','" + Condition1 + "','" + Condition2 + "','" + Sort + "')";
            insertTable(sql_insertReportValue, str_filepath, str_filename);
        }
        insertTable("/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */", str_filepath, str_filename);
    }
}
