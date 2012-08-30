/*++

Module Name:

    LogUtility.cs

Abstract:
    
    This is a helper to write the log. It sets the contents and error levels of the log file.

Author:

    Longran Wei 07-January-2010


Revision History:

--*/


using System;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;

/// <summary>
/// This is a helper to write the log. It sets the contents and error levels of the log file.
/// </summary>
public class LogUtility : Page
{
    #region LogErrorLevel enum

    public enum LogErrorLevel
    {
        LOG_NOISE = 1,
        LOG_SUSPICIOUS = 2,
        LOG_ERROR = 3,
        LOG_CRITICAL = 4,
        LOG_ACCESS = 5
    }

    #endregion

    private static string logfileName = "SiemensHPCRMTool.log"; //log file name

    private static readonly string logPath = HttpContext.Current.Request.PhysicalApplicationPath + "Log";
                                   // log file path 

    private static readonly FileInfo Finfo = new FileInfo(logPath + "\\" + logfileName);

    protected LogErrorLevel reportLowestErrorLevel = LogErrorLevel.LOG_NOISE;

    /// <summary>
    /// write log to file.
    /// </summary>
    /// <param name="level">log level</param>
    /// <param name="message">log information</param>
    public void WriteLog(LogErrorLevel level, string message)
    {
        lock (Finfo)
        {
            StreamWriter writer = null;
            try
            {
                writer = Finfo.AppendText();
                StringBuilder strInput = SetLogMessage(level, message);
                writer.Write(strInput);
            }
            catch (Exception x)
            {
                StringBuilder strInput = SetLogMessage(level, message);
                var conn = new SqlConnection(SQLHelper.ConnectionString);
                conn.Open();
                string sqlstr = "Insert into Log(Message,LogFileError) Values(@Message,@LogFileError)";
                var cmd = new SqlCommand(sqlstr, conn);
                cmd.Parameters.AddWithValue("@Message", strInput.ToString());
                cmd.Parameters.AddWithValue("@LogFileError", x.ToString());
                cmd.ExecuteNonQuery();
                conn.Close();
            }
            finally
            {
                if (writer != null)
                {
                    writer.Flush();
                    writer.Close();
                }
            }
        }
    }

    /// <summary>
    /// format log information
    /// </summary>
    /// <param name="level">log level</param>
    /// <param name="message">original information</param>
    /// <returns>log information</returns>
    public StringBuilder SetLogMessage(LogErrorLevel level, string message)
    {
        var strInput = new StringBuilder();
        if (level >= reportLowestErrorLevel)
        {
            try
            {
                string remoteHost = HttpContext.Current.Request.UserHostAddress;
                string remoteIdent = User.Identity.Name;
                string requestMethod = HttpContext.Current.Request.HttpMethod;
                string requestUrl = HttpContext.Current.Request.Url.PathAndQuery;
                string serverProtocol = HttpContext.Current.Request.ServerVariables["SERVER_PROTOCOL"];
                int statusCode = HttpContext.Current.Response.StatusCode;
                string dateString = DateTime.Now.ToString();
                string user = "";

                if (HttpContext.Current.Session["AdministratorID"] != null)
                    user = "Administrator: " + HttpContext.Current.Session["AdministratorID"];
                else if (HttpContext.Current.Session["ExecutiveID"] != null)
                    user = "Executive: " + HttpContext.Current.Session["ExecutiveID"];
                else if (HttpContext.Current.Session["GeneralMarketingMgrID"] != null)
                    user = "GeneralMarketingMgr: " + HttpContext.Current.Session["GeneralMarketingMgrID"];
                else if (HttpContext.Current.Session["GeneralSalesOrgMgrID"] != null)
                    user = "GeneralSalesOrgMgr: " + HttpContext.Current.Session["GeneralSalesOrgMgrID"];
                else if (HttpContext.Current.Session["MarketingID"] != null)
                    user = "Marketing: " + HttpContext.Current.Session["MarketingID"];
                else if (HttpContext.Current.Session["SalesID"] != null)
                    user = "Sales: " + HttpContext.Current.Session["SalesID"];
                else if (HttpContext.Current.Session["RSMID"] != null)
                    user = "RSM: " + HttpContext.Current.Session["RSMID"];

                if (string.IsNullOrEmpty(remoteIdent))
                    remoteIdent = "-";
                if (string.IsNullOrEmpty(user))
                    user = "-";
                strInput.Append(remoteHost + " ");
                strInput.Append(remoteIdent + " ");
                strInput.Append("[" + dateString + "] ");
                strInput.Append("\"" + requestMethod + " " + requestUrl + " " + serverProtocol + "\" ");
                strInput.Append(statusCode + " ");
                strInput.Append("[" + user + "] ");
                strInput.Append("[" + level.ToString() + "] ");
                strInput.Append(message + "\r\n");
            }
            catch
            {
                strInput.Append("unknown ");
                strInput.Append(message + "\r\n");
            }
        }
        return strInput;
    }
}