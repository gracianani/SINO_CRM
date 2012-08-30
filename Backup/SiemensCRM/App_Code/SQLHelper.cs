/*++

Module Name:

    SQLHelper.cs

Abstract:
    
    This is a helper to operate the database.

Author:

    Longran Wei 07-January-2010


Revision History:

--*/


using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
/// <summary>
/// the helper for DB operation
/// </summary>
public class SQLHelper
{
    public static string ConnectionString = ConfigurationSettings.AppSettings["ConnectionString"];
    private readonly LogUtility log;
    private SqlConnection Connection;

    public SQLHelper()
    {
        log = new LogUtility();
    }
    /// <summary>
    /// open database connection.
    /// </summary>
    public void Open()
    {
        Connection = new SqlConnection(ConnectionString);
        try
        {
            if (Connection.State.Equals(ConnectionState.Closed))
            {
                Connection.Open();
                //log.WriteLog(LogUtility.LogErrorLevel.LOG_NOISE, "SQLHelper.Open: Successfully open a database connection('" + ConnectionString + "').");
            }
        }
        catch (Exception ex)
        {
            //log.WriteLog(LogUtility.LogErrorLevel.LOG_CRITICAL, "SQLHelper.Open: Failed to open a database connection('" + ConnectionString + "'). Exception: " + ex.Message);
            log.WriteLog(LogUtility.LogErrorLevel.LOG_CRITICAL,
                         "SQLHelper.Open: Failed to open a database connection. Exception: " + ex.Message);
        }
    }
    /// <summary>
    /// close database connection.
    /// </summary>
    public void Close()
    {
        try
        {
            if (Connection.State.Equals(ConnectionState.Open))
            {
                Connection.Close();
                Connection.Dispose();
                log.WriteLog(LogUtility.LogErrorLevel.LOG_NOISE,
                             "SQLHelper.Close: Successfully close the connection to the database and release the resource.");
            }
        }
        catch (Exception ex)
        {
            log.WriteLog(LogUtility.LogErrorLevel.LOG_CRITICAL,
                         "SQLHelper.Close: Failed to close the connection to the database and release the resource. Exception: " +
                         ex.Message);
        }
    }

    /// <summary>
    /// Execute a SqlCommand (that returns no resultset) against the database specified in the connection string 
    /// using the provided parameters.
    /// </summary>
    /// <remarks>
    /// e.g.:  
    ///  int result = ExecuteNonQuery(CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));
    ///  int result = ExecuteNonQuery(CommandType.Text, sqlString,  SqlParameter [] parameter);
    /// </remarks>
    /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
    /// <param name="commandText">the stored procedure name or T-SQL command</param>
    /// <param name="commandParameters">an array of SqlParamters used to execute the command</param>
    /// <returns>an int representing the number of rows affected by the command</returns>
    public int ExecuteNonQuery(CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
    {
        var cmd = new SqlCommand();

        try
        {
            PrepareCommand(cmd, cmdType, cmdText, commandParameters);
            int val = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            log.WriteLog(LogUtility.LogErrorLevel.LOG_NOISE,
                         "SQLHelper.ExecuteNonQuery: Successfully execute a sqlcommand('" + cmdText + "').");
            return val;
        }
        catch (Exception ex)
        {
            log.WriteLog(LogUtility.LogErrorLevel.LOG_CRITICAL,
                         "SQLHelper.ExecuteNonQuery: Failed to execute a sqlcommand('" + cmdText + "'). Exception: " +
                         ex.Message);
            return -1;
        }
        finally
        {
            Close();
        }
    }

    /// <summary>
    /// Execute a SqlCommand (that returns no resultset) against the database specified in the connection string 
    /// using the provided parameters.
    /// </summary>
    /// <remarks>
    /// e.g.:  
    ///  int result = ExecuteNonQuery(CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));
    ///  int result = ExecuteNonQuery(CommandType.Text, sqlString,  SqlParameter [] parameter);
    /// </remarks>
    /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
    /// <param name="commandText">the stored procedure name or T-SQL command</param>
    /// <param name="commandParameters">an array of SqlParamters used to execute the command</param>
    /// <returns>an int representing the number of rows affected by the command</returns>
    public int ExecuteNonQuery(CommandType cmdType, ArrayList cmdText, params SqlParameter[] commandParameters)
    {
        var cmd = new SqlCommand();
        PrepareCommand(cmd, cmdType, "", commandParameters);
        SqlTransaction sqlTran = Connection.BeginTransaction();
        cmd.Transaction = sqlTran;
        try
        {
            foreach (string sql in cmdText)
            {
                if (!string.IsNullOrEmpty(sql))
                {
                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();
                }
            }
            sqlTran.Commit();
            int val = 1;
            cmd.Parameters.Clear();
            log.WriteLog(LogUtility.LogErrorLevel.LOG_NOISE,
                         "SQLHelper.ExecuteNonQuery: Successfully execute a sqlcommand('" + cmdText + "').");
            return val;
        }
        catch (Exception ex)
        {
            sqlTran.Rollback();
            log.WriteLog(LogUtility.LogErrorLevel.LOG_CRITICAL,
                         "SQLHelper.ExecuteNonQuery: Failed to execute a sqlcommand('" + cmdText + "'). Exception: " +
                         ex.Message);
            return -1;
        }
        finally
        {
            Close();
        }
    }

    /// <summary>
    /// Execute a SqlCommand that returns the first column of the first record against an existing database connection 
    /// using the provided parameters.
    /// </summary>
    /// <remarks>
    /// e.g.:  
    ///  Object obj = ExecuteScalar(CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));
    /// </remarks>
    /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
    /// <param name="commandText">the stored procedure name or T-SQL command</param>
    /// <param name="commandParameters">an array of SqlParamters used to execute the command</param>
    /// <returns>An object that should be converted to the expected type using Convert.To{Type}</returns>
    public object ExecuteScalar(CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
    {
        var cmd = new SqlCommand();

        try
        {
            PrepareCommand(cmd, cmdType, cmdText, commandParameters);
            object val = cmd.ExecuteScalar();
            cmd.Parameters.Clear();
            log.WriteLog(LogUtility.LogErrorLevel.LOG_NOISE,
                         "SQLHelper.ExecuteScalar: Successfully execute a sqlcommand('" + cmdText + "').");
            return val;
        }
        catch (Exception ex)
        {
            log.WriteLog(LogUtility.LogErrorLevel.LOG_CRITICAL,
                         "SQLHelper.ExecuteScalar: Failed to execute a sqlcommand('" + cmdText + "'). Exception: " +
                         ex.Message);
            return null;
        }
        finally
        {
            Close();
        }
    }

    /// <summary>
    /// Execute a SqlCommand that returns a resultset against the database specified in the connection string 
    /// using the provided parameters.
    /// </summary>
    /// <remarks>
    /// e.g.:  
    ///  SqlDataReader r = ExecuteReader(CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));
    /// </remarks>
    /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
    /// <param name="commandText">the stored procedure name or T-SQL command</param>
    /// <param name="commandParameters">an array of SqlParamters used to execute the command</param>
    /// <returns>A SqlDataReader containing the results</returns>
    public SqlDataReader ExecuteReader(CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
    {
        var cmd = new SqlCommand();

        // we use a try/catch here because if the method throws an exception we want to 
        // close the connection throw code, because no datareader will exist, hence the 
        // commandBehaviour.CloseConnection will not work
        try
        {
            PrepareCommand(cmd, cmdType, cmdText, commandParameters);
            SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            log.WriteLog(LogUtility.LogErrorLevel.LOG_NOISE,
                         "SQLHelper.ExecuteReader: Successfully execute a sqlcommand('" + cmdText + "').");
            return rdr;
        }
        catch (Exception ex)
        {
            Close();
            log.WriteLog(LogUtility.LogErrorLevel.LOG_CRITICAL,
                         "SQLHelper.ExecuteReader: Failed to execute a sqlcommand('" + cmdText + "'). Exception: " +
                         ex.Message);
            return null;
        }
    }

    /// <summary>
    /// Prepare a command for execution
    /// </summary>
    /// <param name="cmd">SqlCommand object</param>
    /// <param name="cmdType">Cmd type e.g. stored procedure or text</param>
    /// <param name="cmdText">Command text, e.g. Select * from Products</param>
    /// <param name="cmdParms">SqlParameters to use in the command</param>
    private void PrepareCommand(SqlCommand cmd, CommandType cmdType, string cmdText, SqlParameter[] cmdParms)
    {
        Open();

        cmd.Connection = Connection;

        cmd.CommandText = cmdText;

        cmd.CommandType = cmdType;

        if (cmdParms != null)
        {
            foreach (SqlParameter parm in cmdParms)
                cmd.Parameters.Add(parm);
        }
    }

    /// <summary>
    /// Adds or refreshes rows in the DataSet.
    /// </summary>
    /// <param name="SqlString">a T-SQL command</param>
    /// <returns>A DataSet containing the results.</returns>
    public DataSet GetDataSet(String SqlString)
    {
        var dataset = new DataSet();
        try
        {
            Open();
            var adapter = new SqlDataAdapter(SqlString, Connection);
            adapter.Fill(dataset);
            log.WriteLog(LogUtility.LogErrorLevel.LOG_NOISE,
                         "SQLHelper.GetDataSet: Successfully add or refresh rows in the DataSet('" + SqlString + "').");
        }
        catch (Exception ex)
        {
            log.WriteLog(LogUtility.LogErrorLevel.LOG_CRITICAL,
                         "SQLHelper.GetDataSet: Failed to add or refresh rows in the DataSet('" + SqlString +
                         "'). Exception: " + ex.Message);
        }
        finally
        {
            Close();
        }
        return dataset;
    }

    /// <summary>
    /// Override GetDataSet method. Adds or refreshes rows in the DataSet.
    /// </summary>
    /// <param name="cmdText">the stored procedure name or T-SQL command</param>
    /// <param name="cmdType">the CommandType (stored procedure, text, etc.)</param>
    /// <param name="cmdParms">an array of SqlParamters used to execute the command</param>
    /// <returns>A DataSet containing the results.</returns>
    public DataSet GetDataSet(string cmdText, CommandType cmdType, SqlParameter[] cmdParms)
    {
        var cmd = new SqlCommand();
        PrepareCommand(cmd, cmdType, cmdText, cmdParms);
        var dataset = new DataSet();
        try
        {
            Open();
            var adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dataset);
            log.WriteLog(LogUtility.LogErrorLevel.LOG_NOISE,
                         "SQLHelper.GetDataSet: Successfully add or refresh rows in the DataSet('" + cmdText + "').");
        }
        catch (Exception ex)
        {
            log.WriteLog(LogUtility.LogErrorLevel.LOG_CRITICAL,
                         "SQLHelper.GetDataSet: Failed to add or refresh rows in the DataSet('" + cmdText +
                         "'). Exception: " + ex.Message);
        }
        finally
        {
            Close();
        }
        return dataset;
    }

    /// <summary>
    /// Adds or refreshes rows in the DataSet.
    /// </summary>
    /// <param name="SqlString">a T-SQL command</param>
    /// <returns>A DataSet containing the results.</returns>
    public DataTable GetDataSetStru(String SqlString)
    {
        var dt = new DataTable();
        try
        {
            Open();
            var adapter = new SqlDataAdapter(SqlString, Connection);
            adapter.FillSchema(dt, SchemaType.Source);
            log.WriteLog(LogUtility.LogErrorLevel.LOG_NOISE,
                         "SQLHelper.GetDataSet: Successfully add or refresh rows in the DataSet('" + SqlString + "').");
        }
        catch (Exception ex)
        {
            log.WriteLog(LogUtility.LogErrorLevel.LOG_CRITICAL,
                         "SQLHelper.GetDataSet: Failed to add or refresh rows in the DataSet('" + SqlString +
                         "'). Exception: " + ex.Message);
        }
        finally
        {
            Close();
        }
        return dt;
    }
}