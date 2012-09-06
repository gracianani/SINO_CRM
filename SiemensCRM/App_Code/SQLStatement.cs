/*
 *  File Name    : SQLStatement.cs
 * 
 *  Description  : Get value by sql statement in common use
 * 
 *  Author       : Wang Jun
 * 
 *  Modify Date  : 2010.5.12
 *  
 *  Problem      : 
 */

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
// By DingJunjie 20110516 Item 2 Add Start

// By DingJunjie 20110516 Item 2 Add End

/// <summary>
/// Get value by sql statement in common use.
/// </summary>
public class SQLStatement
{
    private readonly SQLHelper helper = new SQLHelper();
    private readonly DisplayInfo info = new DisplayInfo();
    private readonly LogUtility log = new LogUtility();
    //private readonly GetMeetingDate meeting = new GetMeetingDate();
    private readonly GetSelectMeetingDate meeting = new GetSelectMeetingDate();
    //ryzhang item49 20110519 add start
    private readonly GetSelectMeetingDate selectMeeting = new GetSelectMeetingDate();
    private readonly WebUtility web = new WebUtility();
    //ryzhang item49 20110519 add start


    /// <summary>
    /// if a DataSet has no rows, add a null row to it.
    /// </summary>
    /// /// <returns>return null dataset</returns>
    public DataSet getNullDataSet(DataSet ds)
    {
        DataRow row = ds.Tables[0].NewRow();
        foreach (DataColumn col in ds.Tables[0].Columns)
        {
            col.AllowDBNull = true;
            row[col] = DBNull.Value;
        }
        ds.Tables[0].Rows.Add(row);
        return ds;
    }

    /* * * * * Statement: Relation * * * * */

    /// <summary>
    /// Get all role that consist of his ID and Name.
    /// </summary>
    /// <returns>return all role info.</returns>
    public DataSet getRole()
    {
        string sql_role = "SELECT Name, ID FROM [Role] ORDER BY Name ASC";

        try
        {
            DataSet ds_role = helper.GetDataSet(sql_role);
            return ds_role;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// get all users by role id.
    /// </summary>
    /// <param name="roleID">user RoleID</param>
    /// <returns>all users</returns>
    public DataSet getUser(string roleID)
    {
        string sql_searchUser;
        sql_searchUser = "SELECT Alias, UserID FROM [User] WHERE Deleted = 0 AND RoleID = " + roleID;
        sql_searchUser += " ORDER BY Alias ASC ";

        try
        {
            DataSet ds_query_Admin = helper.GetDataSet(sql_searchUser);
            log.WriteLog(LogUtility.LogErrorLevel.LOG_NOISE,
                         "SQLStatment.cs:(" + sql_searchUser + "), execute successfully.");
            return ds_query_Admin;
        }
        catch (Exception ex)
        {
            log.WriteLog(LogUtility.LogErrorLevel.LOG_CRITICAL,
                         "SQLStatment.cs:(" + sql_searchUser + "), Exception:" + ex.Message);
            return null;
        }
    }

    /* * * * * Statement: User * * * * */

    /// <summary>
    ///  Get administrator's information by defferent options
    /// </summary>
    /// <param name="str_Name">option string</param>
    /// <param name="sel">option type</param>
    /// <returns>administators info</returns>
    public DataSet getAdministratorInfo(string str_Name, int sel)
    {
        string sql_searchUser;
        sql_searchUser = "SELECT [User].UserID, [User].FirstName,[User].LastName, [User].Alias AS Login,[User].Abbr,"
                         +
                         " [Role].Name AS Role,(case when [User].Gender = 1 Then 'Female' else 'Male' End) AS Gender, "
                         + " CONVERT(varchar(15),[User].StartDate,23) AS StartDate,"
                         + " CONVERT(varchar(15),[User].EndDate,23) AS EndDate,"
                         + " [User].Email "
                         + " FROM [User] INNER JOIN [Role]"
                         + " ON [User].RoleID = [Role].ID"
                         + " WHERE [User].Deleted = 0";
        if (sel == 0)
        {
            sql_searchUser += " AND [User].FirstName like '%" + str_Name + "%'"
                              + " ORDER BY [User].FirstName,[Role].ID  ASC";
        }
        else if (sel == 1)
        {
            sql_searchUser += " AND [User].LastName like '%" + str_Name + "%'"
                              + " ORDER BY [User].LastName,[Role].ID  ASC";
        }
        else if (sel == 2)
        {
            sql_searchUser += " AND [User].Alias like '%" + str_Name + "%'"
                              + " ORDER BY [User].Alias,[Role].ID  ASC";
        }
        else if (sel == 3)
        {
            sql_searchUser += " AND [User].Abbr like '%" + str_Name + "%'"
                              + " ORDER BY [User].Abbr,[Role].ID  ASC";
        }
        else if (sel == 4)
        {
            sql_searchUser += " AND [Role].Name like '%" + str_Name + "%'"
                              + " ORDER BY [Role].ID, [User].Alias  ASC";
        }
        else
            sql_searchUser += " ORDER By [Role].ID,[User].Alias ASC ";

        try
        {
            DataSet ds_query_Admin = helper.GetDataSet(sql_searchUser);
            log.WriteLog(LogUtility.LogErrorLevel.LOG_NOISE,
                         "SQLStatment.cs:(" + sql_searchUser + "), execute successfully.");
            return ds_query_Admin;
        }
        catch (Exception ex)
        {
            log.WriteLog(LogUtility.LogErrorLevel.LOG_CRITICAL,
                         "SQLStatment.cs:(" + sql_searchUser + "), Exception:" + ex.Message);
            return null;
        }
    }

    /// <summary>
    /// get operation abbrs by user id.
    /// </summary>
    /// <param name="userid">User ID</param>
    /// <returns>operation info</returns>
    public DataSet getOperationByUser(string userid)
    {
        string sql_operation = "SELECT [Operation].AbbrL, [Operation].ID FROM [Operation] INNER JOIN [User_Operation]"
                               + " ON [Operation].ID = [User_Operation].OperationID WHERE [User_Operation].UserID = '" +
                               userid +
                               "' AND [User_Operation].Deleted = 0 AND [Operation].Deleted = 0 GROUP BY [Operation].AbbrL, [Operation].ID ORDER BY [Operation].AbbrL, [Operation].ID";
        try
        {
            DataSet ds_operation = helper.GetDataSet(sql_operation);
            /*by ryzhang 20110509 item47 add start*/
            String queryString = @"SELECT COUNT(ID) FROM Operation WHERE Deleted = 0";
            DataSet ds_operationCount = helper.GetDataSet(queryString);
            var count = (int) ds_operationCount.Tables[0].Rows[0][0];
            if (count == ds_operation.Tables[0].Rows.Count)
            {
                ds_operation.Tables[0].Clear();
                ds_operation.Tables[0].Rows.Add(new Object[2] {"All", 0});
            }
            /*by ryzhang 20110509 item47 add end*/
            log.WriteLog(LogUtility.LogErrorLevel.LOG_NOISE,
                         "SQLStatment.cs:(" + sql_operation + "), execute successfully.");
            return ds_operation;
        }
        catch (Exception ex)
        {
            log.WriteLog(LogUtility.LogErrorLevel.LOG_CRITICAL,
                         "SQLStatment.cs:(" + sql_operation + "), Exception:" + ex.Message);
            return null;
        }
    }

    /// <summary>
    /// get segments by user id
    /// </summary>
    /// <param name="userid">User ID</param>
    /// <returns>segments</returns>
    public DataSet getSegmentByUser(string userid)
    {
        string sql_segment = "SELECT [Segment].Abbr, [Segment].ID FROM [Segment] INNER JOIN [User_Segment]"
                             + " ON [Segment].ID = [User_Segment].SegmentID WHERE [User_Segment].UserID = '" + userid +
                             "' AND [User_Segment].Deleted = 0 AND [Segment].Deleted = 0 GROUP BY [Segment].Abbr, [Segment].ID ORDER BY [Segment].Abbr, [Segment].ID";

        try
        {
            DataSet ds_segment = helper.GetDataSet(sql_segment);
            log.WriteLog(LogUtility.LogErrorLevel.LOG_NOISE,
                         "SQLStatment.cs:(" + sql_segment + "), execute successfully.");
            return ds_segment;
        }
        catch (Exception ex)
        {
            log.WriteLog(LogUtility.LogErrorLevel.LOG_CRITICAL,
                         "SQLStatment.cs:(" + sql_segment + "), Exception:" + ex.Message);
            return null;
        }
    }

    /// <summary>
    /// get country by user id
    /// </summary>
    /// <param name="userid">User ID</param>
    /// <returns>countries</returns>
    public DataSet getCountryByUser(string userid)
    {
        string sql_country = "SELECT [SubRegion].Name, [SubRegion].ID FROM [SubRegion] INNER JOIN [User_Country]"
                             + " ON [SubRegion].ID = [User_Country].CountryID WHERE [User_Country].UserID = '" + userid +
                             "' AND [User_Country].Deleted = 0 AND [SubRegion].Deleted = 0 GROUP BY [SubRegion].Name, [SubRegion].ID ORDER BY [SubRegion].Name, [SubRegion].ID";

        try
        {
            DataSet ds_country = helper.GetDataSet(sql_country);
            /*by ryzhang 20110509 item47 add start*/
            String queryString = "SELECT Count(distinct [SubRegion].ID) "
                                 + " FROM [Country_SubRegion]"
                                 + " INNER JOIN [Country] ON [Country].ID = [Country_SubRegion].CountryID"
                                 + " INNER JOIN [SubRegion] ON [SubRegion].ID = [Country_SubRegion].SubRegionID"
                                 +
                                 " WHERE [SubRegion].Deleted = 0 AND Country.Deleted=0 AND Country_SubRegion.Deleted=0 ";
            DataSet ds_subregionCount = helper.GetDataSet(queryString);
            var count = (int) ds_subregionCount.Tables[0].Rows[0][0];
            if (count == ds_country.Tables[0].Rows.Count)
            {
                ds_country.Tables[0].Clear();
                ds_country.Tables[0].Rows.Add(new Object[2] {"All", 0});
            }
            /*by ryzhang 20110509 item47 add end*/
            log.WriteLog(LogUtility.LogErrorLevel.LOG_NOISE,
                         "SQLStatment.cs:(" + sql_country + "), execute successfully.");
            return ds_country;
        }
        catch (Exception ex)
        {
            log.WriteLog(LogUtility.LogErrorLevel.LOG_CRITICAL,
                         "SQLStatment.cs:(" + sql_country + "), Exception:" + ex.Message);
            return null;
        }
    }

    /// <summary>
    ///  Judge whether or not having existed the user named str_user
    ///  when roleId = str_roleID and gender = str_gender
    /// </summary>
    /// <param name="str_user">User Alias</param>
    /// <param name="str_roleID">User RoleID</param>
    /// <param name="str_gender">User Gender</param>
    /// <returns>if exist, return true, else false</returns>
    public bool existUser(string str_user, string str_roleID, string str_gender)
    {
        string query_user = "SELECT UserID FROM [User] WHERE Alias = '" + str_user + "' AND RoleID = " + str_roleID +
                            " AND Gender = " + str_gender + " AND Deleted = 0";
        DataSet ds_user = helper.GetDataSet(query_user);

        if (ds_user.Tables[0].Rows.Count == 1)
            return true;
        else
            return false;
    }

    /// <summary>
    ///  the function provide a interface of adding user.
    /// </summary>
    /// <param name="str_alias"> User Alias</param>
    /// <param name="str_abbr">User Abbr</param>
    /// <param name="str_role">User roleID</param>
    /// <param name="str_startDate">User date of starting work</param>
    /// <param name="str_gender">User gender</param>
    /// <returns>operation result</returns>
    public string addUser(string str_alias, string str_abbr, string str_role, string str_startDate, string str_gender)
    {
        if (str_alias != "" && web.CheckDate(str_startDate) && str_abbr != "")
        {
            int gender = (str_gender == "Female" ? 0 : 1);
            if (existUser(str_alias, str_role, gender.ToString()))
            {
                string insert_user = "INSERT INTO [User](Alias,Abbr,RoleID,StartDate,Gender,Deleted) VALUES('" +
                                     str_alias + "','" + str_abbr + "'," + str_role + ",'" + str_startDate + "'," +
                                     gender + ",0)";
                int count = helper.ExecuteNonQuery(CommandType.Text, insert_user, null);

                if (count == 1)
                    return info.addLabelInfo(str_alias, true);
                else
                    return info.addLabelInfo(str_alias, false);
                ;
            }
            else
                return info.addExist(str_alias);
        }
        else
        {
            return info.addillegal("Alias = " + str_alias + ",StartDate = " + str_startDate + ",Abbr = " + str_abbr);
        }
    }

    /* * * * * Statement: Segment/Product * * * * */

    /// <summary>
    /// Get segment information that consist of his ID,Abbr and Description.
    /// </summary>
    /// <returns>segments</returns>
    public DataSet getSegmentInfo()
    {
        string sql_segment =
            "SELECT ID,Abbr AS 'Segment',Description FROM [Segment] WHERE Deleted = 0 ORDER BY Abbr ASC";

        try
        {
            DataSet ds_segment = helper.GetDataSet(sql_segment);
            log.WriteLog(LogUtility.LogErrorLevel.LOG_NOISE,
                         "SQLStatment.cs:(" + sql_segment + "), execute successfully.");
            return ds_segment;
        }
        catch (Exception ex)
        {
            log.WriteLog(LogUtility.LogErrorLevel.LOG_CRITICAL,
                         "SQLStatment.cs:(" + sql_segment + "), Exception:" + ex.Message);
            return null;
        }
    }

    /// <summary>
    /// judge whether or not exist the segment whose abbrevation is 'abbr'
    /// </summary>
    /// <param name="abbr">string Segment Abbr</param>
    /// <returns>if exist, return true, else false.</returns>
    public bool existSegment(string abbr)
    {
        string sql_segment = "SELECT ID FROM [Segment] WHERE Deleted = 0 "
                             + " AND Abbr = '" + abbr + "'";
        DataSet ds_segment = helper.GetDataSet(sql_segment);

        if (ds_segment.Tables[0].Rows.Count > 0)
            return true;
        else
            return false;
    }

    /// <summary>
    /// update segment infomation which consist abbr and description
    /// </summary>
    /// <param name="abbr">string segment abbr</param>
    /// <param name="description">string segment description</param>
    /// <param name="ID">string segment id</param>
    /// <returns>if success, return true, else false</returns>
    public bool updateSegment(string abbr, string description, string ID)
    {
        string update_segment = "UPDATE [Segment] SET Abbr = '" + abbr + "'"
                                + " , Description = '" + description + "'"
                                + " WHERE ID = " + ID;
        int count = helper.ExecuteNonQuery(CommandType.Text, update_segment, null);
        if (count == 1)
            return true;
        else
            return false;
    }

    /// <summary>
    /// delete one segment
    /// </summary>
    /// <param name="ID">string segment id</param>
    /// <returns>if success, return true, else false</returns>
    public bool delSegment(string ID)
    {
        string del_segment = "UPDATE [Segment] SET Deleted = 1 "
                             + " WHERE ID = " + ID;
        int count = helper.ExecuteNonQuery(CommandType.Text, del_segment, null);
        if (count == 1)
            return true;
        else
            return false;
    }

    /// <summary>
    /// add one segment
    /// </summary>
    /// <param name="abbr">string segment abbr</param>
    /// <param name="description">string segment description</param>
    /// <returns>if success, return true, else false</returns>
    public bool addSegment(string abbr, string description)
    {
        string insert_segment = "INSERT INTO [Segment](Description,Abbr,Deleted)"
                                + " VALUES('" + description + "','" + abbr + "',0)";
        int count = helper.ExecuteNonQuery(CommandType.Text, insert_segment, null);
        if (count == 1)
            return true;
        else
            return false;
    }

    /// <summary>
    /// Get product information that consist of his ID,Abbr,Description and Note by segment.
    /// </summary>
    /// <param name="str_segmentID">Segment ID</param>
    /// <returns>return dataset</returns>
    public DataSet getProductInfoBySegmentID(string str_segmentID)
    {
        string sql_product = "SELECT [Product].ID,[Product].Abbr AS 'Product',[Product].Description FROM [Segment_Product] "
                             + " INNER JOIN [Product] ON [Product].ID = [Segment_Product].ProductID"
                             + " WHERE [Segment_Product].SegmentID = " + str_segmentID
                             + " AND [Segment_Product].Deleted = 0 AND [Product].Deleted = 0"
                             + " ORDER BY [Product].ID ASC";

        try
        {
            DataSet ds_product = helper.GetDataSet(sql_product);
            log.WriteLog(LogUtility.LogErrorLevel.LOG_NOISE,
                         "SQLStatment.cs:(" + sql_product + "), execute successfully.");
            return ds_product;
        }
        catch (Exception ex)
        {
            log.WriteLog(LogUtility.LogErrorLevel.LOG_CRITICAL,
                         "SQLStatment.cs:(" + sql_product + "), Exception:" + ex.Message);
            return null;
        }
    }

    public Dictionary<string, int> getProductIDsBySegmentIDByProductAbbrs(string str_segmentID, List<string> abbrs)
    {
        if (abbrs == null || abbrs.Count < 1)
            return null;
        string cond = string.Empty;
        foreach (string item in abbrs)
        {
            if (string.IsNullOrEmpty(cond))
                cond = "'" + item + "'";
            else
                cond += ",'" + item + "'";
        }
        string sql_product = "SELECT [Product].ID,[Product].Abbr AS 'Product',[Product].Description FROM [Segment_Product] "
                             + " INNER JOIN [Product] ON [Product].ID = [Segment_Product].ProductID"
                             + " WHERE [Segment_Product].SegmentID = " + str_segmentID
                             + " AND [Segment_Product].Deleted = 0 AND [Product].Deleted = 0"
                             + " and [Product].Abbr in (" + cond + ")"
                             + " ORDER BY [Product].ID ASC";
        DataSet ds = helper.GetDataSet(sql_product);
        var result = new Dictionary<string, int>();
        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
        {
            result.Add(ds.Tables[0].Rows[i][1].ToString().Trim(), Convert.ToInt32(ds.Tables[0].Rows[i][0].ToString()));
        }
        return result;
    }

    /// <summary>
    /// judge whether or not exist the product whose abbrevation is 'abbr' and which beyond to the segment.
    /// </summary>
    /// <param name="str_segmentID">string segment id</param>
    /// <param name="str_abbr">string product abbr</param>
    /// <returns>if exist, return true, else false.</returns>
    public bool existProduct(string str_segmentID, string str_abbr)
    {
        string sql_product = "SELECT [Product].ID FROM [Segment_Product] "
                             + " INNER JOIN [Product] ON [Product].ID = [Segment_Product].ProductID"
                             + " WHERE [Segment_Product].SegmentID = " + str_segmentID
                             + " AND [Segment_Product].Deleted = 0 AND [Product].Deleted = 0"
                             + " AND [Product].Abbr = '" + str_abbr + "'";
        DataSet ds_product = helper.GetDataSet(sql_product);

        if (ds_product.Tables[0].Rows.Count == 1)
            return true;
        else
            return false;
    }

    /// <summary>
    /// update product
    /// </summary>
    /// <param name="description">string product description</param>
    /// <param name="ID">string product id</param>
    /// <returns>if success, return true, else false</returns>
    public bool updateProduct(string description, string ID)
    {
        string update_product = "UPDATE [Product] SET "
                                + " Description = '" + description + "'"
                                + " WHERE ID = " + ID;
        int count = helper.ExecuteNonQuery(CommandType.Text, update_product, null);
        if (count == 1)
            return true;
        else
            return false;
    }

    /// <summary>
    /// delete product
    /// </summary>
    /// <param name="ID">string product id</param>
    /// <returns>if success, return true, else false</returns>
    public bool delProduct(string ID)
    {
        string del_product = "UPDATE [Product] SET Deleted = 1 "
                             + " WHERE ID = " + ID;
        int count = helper.ExecuteNonQuery(CommandType.Text, del_product, null);
        string delrel = "UPDATE [Segment_Product] SET Deleted = 1 "
                        + " WHERE ProductID = " + ID;
        helper.ExecuteNonQuery(CommandType.Text, delrel);
        if (count == 1)
            return true;
        else
            return false;
    }

    /// <summary>
    /// add product
    /// </summary>
    /// <param name="abbr">string product abbr</param>
    /// <returns>if success, return true, else false</returns>
    public bool addProduct(string abbr)
    {
        string insert_product = "INSERT INTO [Product](Abbr,Deleted)"
                                + " VALUES('" + abbr + "',0)";
        int count = helper.ExecuteNonQuery(CommandType.Text, insert_product, null);
        if (count == 1)
            return true;
        else
            return false;
    }

    /* * * * * Statement: Operation * * * * */

    /// <summary>
    /// get operation information
    /// </summary>
    /// <param name="str">option string</param>
    /// <param name="sel">option type</param>
    /// <returns>return dataset</returns>
    public DataSet getOperationInfo(string str, int sel)
    {
        string sql_operation = "SELECT [Operation].ID AS OpID, [Segment].ID AS SegID,"
                               +
                               " [Operation].Name AS Operation,[Operation].AbbrL AS 'Abbr',[Operation].Abbr AS 'Allocation',[Segment].Abbr AS Segment, [Currency].Name AS 'Currency'"
                               + " FROM [Operation_Segment] INNER JOIN [Operation]"
                               + " ON [Operation].ID = [Operation_Segment].OperationID"
                               + " INNER JOIN [Segment] ON [Operation_Segment].SegmentID = [Segment].ID"
                               + " INNER JOIN [Currency] ON [Operation].CurrencyID = [Currency].ID"
                               + " WHERE [Segment].Deleted = 0 AND [Operation].Deleted = 0 "
                               + " AND [Operation_Segment].Deleted = 0 AND [Currency].Deleted = 0";

        if (sel == 0)
        {
            sql_operation += " AND [Operation].Name like '%" + str + "%'";
            sql_operation += " ORDER BY [Operation].Name, [Segment].Abbr ASC";
        }
        else if (sel == 1)
        {
            sql_operation += " AND [Operation].AbbrL like '%" + str + "%'";
            sql_operation += " ORDER BY [Operation].AbbrL, [Segment].Abbr ASC";
        }
        else if (sel == 2)
        {
            sql_operation += " AND [Operation].Abbr like '%" + str + "%'";
            sql_operation += " ORDER BY [Operation].Abbr, [Segment].Abbr ASC";
        }
        else if (sel == 3)
        {
            sql_operation += " AND [Segment].Abbr like '%" + str + "%'";
            sql_operation += " ORDER BY [Segment].Abbr ASC";
        }
        else if (sel == 4)
        {
            sql_operation += " AND [Currency].Name like '%" + str + "%'";
            sql_operation += " ORDER BY [Currency].Name, [Segment].Abbr ASC";
        }
        else
        {
            sql_operation += " ORDER BY [Operation].Name, [Segment].Abbr ASC";
        }


        try
        {
            DataSet ds_operation = helper.GetDataSet(sql_operation);
            log.WriteLog(LogUtility.LogErrorLevel.LOG_NOISE,
                         "SQLStatment.cs:(" + sql_operation + "), execute successfully.");
            return ds_operation;
        }
        catch (Exception ex)
        {
            log.WriteLog(LogUtility.LogErrorLevel.LOG_CRITICAL,
                         "SQLStatment.cs:(" + sql_operation + "), Exception:" + ex.Message);
            return null;
        }
    }

    /// <summary>
    ///  Judging whether or not operation exist
    /// </summary>
    /// <param name="str_name">Operation Name</param>
    /// <param name="str_abbr">Operation Abbr</param>
    /// <returns>Only Operation name or abbr exist, return false</returns>
    public bool existOperation(string str_name)
    {
        string query_Operation = "SELECT"
                                 + " [Operation].Name"
                                 + " FROM [Operation]"
                                 + " WHERE [Operation].Deleted = 0"
                                 + " AND [Operation].Name = '" + str_name + "'";
        DataSet ds_Operation = helper.GetDataSet(query_Operation);
        if (ds_Operation.Tables[0].Rows.Count > 0)
            return true;
        else
            return false;
    }

    /// <summary>
    /// update operation
    /// </summary>
    /// <param name="abbrL">string operation abbr</param>
    /// <param name="abbr">string operation allocation</param>
    /// <param name="ID">string operation ID</param>
    /// <returns>if success, return true, else false.</returns>
    public bool updateOperation(string abbrL, string abbr, string str_name, string ID)
    {
        string update_operation = "UPDATE [Operation] SET Name = '" + str_name + "', AbbrL = '" + abbrL + "', Abbr = '" +
                                  abbr + "'"
                                  + " WHERE ID = " + ID + " AND Deleted = 0";
        int count = helper.ExecuteNonQuery(CommandType.Text, update_operation, null);
        if (count == 1)
            return true;
        else
            return false;
    }

    /// <summary>
    /// delete operation
    /// </summary>
    /// <param name="operationID">string operation ID</param>
    /// <param name="segmentID">string segment ID</param>
    /// <returns>if success, return true, else false.</returns>
    public bool delOperation(string operationID, string segmentID)
    {
        string del_operation = "UPDATE [Operation_Segment] SET Deleted = 1 "
                               + " WHERE OperationID = " + operationID + " AND SegmentID = " + segmentID;
        int count = helper.ExecuteNonQuery(CommandType.Text, del_operation, null);
        if (count == 1)
            return true;
        else
            return false;
    }

    /// <summary>
    ///  Judging whether or not segment exist in operation
    /// </summary>
    /// <param name="str_name">Operation Name</param>
    /// <param name="str_segment">Operation Segment</param>
    /// <returns>if the relation of segment and operation has existed, return false</returns>
    public bool existSegment(string str_name, string str_segment)
    {
        string query_Segment = "SELECT"
                               + " [Operation].Name, [Segment].Abbr"
                               + " FROM [Operation_Segment] INNER JOIN [Operation]"
                               + " ON [Operation].ID = [Operation_Segment].OperationID"
                               + " INNER JOIN [Segment]"
                               + " ON [Operation_Segment].SegmentID = [Segment].ID"
                               +
                               " WHERE [Segment].Deleted = 0 AND [Operation].Deleted = 0 AND [Operation_Segment].Deleted = 0"
                               + " AND [Operation].Name = '" + str_name + "'"
                               + " AND [Segment].Abbr = '" + str_segment + "'";
        DataSet ds_Segment = helper.GetDataSet(query_Segment);
        if (ds_Segment.Tables[0].Rows.Count > 0)
            return true;
        else
            return false;
    }

    /* * * * * Statement: Country * * * * */


    public DataSet getRegionInfo(string str, int sel)
    {
        string sql_country = "SELECT [Region].ID AS RegID,[Cluster].ID AS CluID,[Country].ID AS CouID,[SubRegion].ID AS SubRegID,"
                             +
                             " [Region].Name AS Region,[Cluster].Name AS Cluster,[Country].Name AS Country,[SubRegion].Name AS SubRegion,[Country].ISO_Code AS 'ISO Code' "
                             +
                             " FROM [Region],[Cluster],[SubRegion],[Country],[Region_Cluster],[Cluster_Country],[Country_SubRegion]"
                             + " WHERE [Region].ID = [Region_Cluster].RegionID"
                             + " AND  [Cluster].ID = [Region_Cluster].ClusterID"
                             + " AND [Cluster].ID = [Cluster_Country].ClusterID"
                             + " AND [Country].ID = [Cluster_Country].CountryID "
                             + " AND [Country].ID = [Country_SubRegion].CountryID"
                             + " AND [SubRegion].ID = [Country_SubRegion].SubRegionID "
                             + " AND [Region].Deleted = 0 AND [Region_Cluster].Deleted = 0 AND [Cluster].Deleted = 0"
                             +
                             " AND [Cluster_Country].Deleted = 0 AND [Country].Deleted = 0 AND [Country_SubRegion].Deleted = 0"
                             + " AND [SubRegion].Deleted = 0";
        if (sel == 0)
        {
            sql_country += " AND [Region].Name like '%" + str + "%' "
                           + " GROUP BY [Region].ID,[Cluster].ID,[Country].ID,[SubRegion].ID,"
                           + " [Region].Name,[Cluster].Name,[Country].Name,[SubRegion].Name,[Country].ISO_Code";
            //+" ORDER BY [Region].Name ASC";
        }
        else if (sel == 1)
        {
            sql_country += " AND [Cluster].Name like '%" + str + "%' "
                           + " GROUP BY [Region].ID,[Cluster].ID,[Country].ID,[SubRegion].ID,"
                           + " [Region].Name,[Cluster].Name,[Country].Name,[SubRegion].Name,[Country].ISO_Code";
            //+" ORDER BY [Cluster].Name ASC";
        }
        else if (sel == 2)
        {
            sql_country += " AND [Country].Name like '%" + str + "%'"
                           + " GROUP BY [Region].ID,[Cluster].ID,[Country].ID,[SubRegion].ID,"
                           + " [Region].Name,[Cluster].Name,[Country].Name,[SubRegion].Name,[Country].ISO_Code";
            // + " ORDER BY [Country].Name ASC";
        }
        else if (sel == 3)
        {
            sql_country += " AND [SubRegion].Name like '%" + str + "%' "
                           + " GROUP BY [Region].ID,[Cluster].ID,[Country].ID,[SubRegion].ID,"
                           + " [Region].Name,[Cluster].Name,[Country].Name,[SubRegion].Name,[Country].ISO_Code";
            //+" ORDER BY [SubRegion].Name ASC";
        }
        else if (sel == 4)
        {
            sql_country += " AND [Country].ISO_Code like '%" + str + "%'"
                           + " GROUP BY [Region].ID,[Cluster].ID,[Country].ID,[SubRegion].ID,"
                           + " [Region].Name,[Cluster].Name,[Country].Name,[SubRegion].Name,[Country].ISO_Code";
            //+ "  ORDER BY [Country].ISO_Code ASC";
        }
        else
        {
            sql_country += " GROUP BY [Region].ID,[Cluster].ID,[Country].ID,[SubRegion].ID,"
                           + " [Region].Name,[Cluster].Name,[Country].Name,[SubRegion].Name,[Country].ISO_Code";
        }
        sql_country += " ORDER BY [Region].Name, [Cluster].Name, [Country].Name, [SubRegion].Name ASC";

        try
        {
            DataSet ds_country = helper.GetDataSet(sql_country);
            log.WriteLog(LogUtility.LogErrorLevel.LOG_NOISE,
                         "SQLStatment.cs:(" + sql_country + "), execute successfully.");
            return ds_country;
        }
        catch (Exception ex)
        {
            log.WriteLog(LogUtility.LogErrorLevel.LOG_CRITICAL,
                         "SQLStatment.cs:(" + sql_country + "), Exception:" + ex.Message);
            return null;
        }
    }

    //By Lhy 20110506 ITEM 6 ADD End
    /// <summary>
    /// get all region name and id info.
    /// </summary>
    /// <returns>return dataset</returns>
    public DataSet getRegion()
    {
        string sql_region = "SELECT ID, Name FROM [Region] WHERE Deleted = 0 GROUP BY Name ORDER BY Name";
        try
        {
            DataSet ds_region = helper.GetDataSet(sql_region);
            log.WriteLog(LogUtility.LogErrorLevel.LOG_NOISE,
                         "SQLStatment.cs:(" + sql_region + "), execute successfully.");
            return ds_region;
        }
        catch (Exception ex)
        {
            log.WriteLog(LogUtility.LogErrorLevel.LOG_CRITICAL,
                         "SQLStatment.cs:(" + sql_region + "), Exception:" + ex.Message);
            return null;
        }
    }

    /// <summary>
    /// get cluster names
    /// </summary>
    /// <returns>return dataset</returns>
    public DataSet getCluster(string regionID)
    {
        string sql_cluster = " SELECT [Cluster].ID, [Cluster].Name FROM [Cluster] INNER JOIN [Region_Cluster]"
                             + " ON [Cluster].ID = [Region_Cluster].ClusterID"
                             + " WHERE [Region_Cluster].RegionID = " + regionID +
                             " AND [Region_Cluster].Deleted = 0 AND [Cluster].Deleted = 0"
                             + " GROUP BY [Cluster].ID, [Cluster].Name ORDER BY [Cluster].Name ASC";
        try
        {
            DataSet ds_cluster = helper.GetDataSet(sql_cluster);
            log.WriteLog(LogUtility.LogErrorLevel.LOG_NOISE,
                         "SQLStatment.cs:(" + sql_cluster + "), execute successfully.");
            return ds_cluster;
        }
        catch (Exception ex)
        {
            log.WriteLog(LogUtility.LogErrorLevel.LOG_CRITICAL,
                         "SQLStatment.cs:(" + sql_cluster + "), Exception:" + ex.Message);
            return null;
        }
    }

    /// <summary>
    /// get country names by cluster id.
    /// </summary>
    /// <returns>return dataset</returns>
    public DataSet getCountry(string clusterID)
    {
        string sql_country = " SELECT [Country].ID, [Country].Name FROM [Country] INNER JOIN [Cluster_Country]"
                             + " ON [Country].ID = [Cluster_Country].CountryID"
                             + " WHERE [Cluster_Country].ClusterID = " + clusterID +
                             " AND [Cluster_Country].Deleted = 0 AND [Country].Deleted = 0"
                             + " GROUP BY [Country].ID, [Country].Name ORDER BY [Country].Name ASC";
        try
        {
            DataSet ds_country = helper.GetDataSet(sql_country);
            log.WriteLog(LogUtility.LogErrorLevel.LOG_NOISE,
                         "SQLStatment.cs:(" + sql_country + "), execute successfully.");
            return ds_country;
        }
        catch (Exception ex)
        {
            log.WriteLog(LogUtility.LogErrorLevel.LOG_CRITICAL,
                         "SQLStatment.cs:(" + sql_country + "), Exception:" + ex.Message);
            return null;
        }
    }


    /// <summary>
    /// get subregion names by country id.
    /// </summary>
    /// <returns>return dataset</returns>
    public DataSet getSubRegion(string countryID)
    {
        string sql_subregion = " SELECT [SubRegion].ID, [SubRegion].Name FROM [SubRegion] INNER JOIN [Country_SubRegion]"
                               + " ON [SubRegion].ID = [Country_SubRegion].ClusterID"
                               + " WHERE [Country_SubRegion].CountryID = " + countryID +
                               " AND [Country_SubRegion].Deleted = 0 AND [SubRegion].Deleted = 0"
                               + " GROUP BY [SubRegion].ID, [SubRegion].Name ORDER BY [SubRegion].Name ASC";
        try
        {
            DataSet ds_subregion = helper.GetDataSet(sql_subregion);
            log.WriteLog(LogUtility.LogErrorLevel.LOG_NOISE,
                         "SQLStatment.cs:(" + sql_subregion + "), execute successfully.");
            return ds_subregion;
        }
        catch (Exception ex)
        {
            log.WriteLog(LogUtility.LogErrorLevel.LOG_CRITICAL,
                         "SQLStatment.cs:(" + sql_subregion + "), Exception:" + ex.Message);
            return null;
        }
    }

    /// <summary>
    /// get subregion names
    /// </summary>
    /// <returns>return dataset</returns>
    public DataSet getSubRegionInfo()
    {
        string sql_subregion = " SELECT [SubRegion].ID, [SubRegion].Name FROM [SubRegion]"
                               + " WHERE [SubRegion].Deleted = 0"
                               + " GROUP BY [SubRegion].ID, [SubRegion].Name ORDER BY [SubRegion].Name ASC";
        try
        {
            DataSet ds_subregion = helper.GetDataSet(sql_subregion);
            log.WriteLog(LogUtility.LogErrorLevel.LOG_NOISE,
                         "SQLStatment.cs:(" + sql_subregion + "), execute successfully.");
            return ds_subregion;
        }
        catch (Exception ex)
        {
            log.WriteLog(LogUtility.LogErrorLevel.LOG_CRITICAL,
                         "SQLStatment.cs:(" + sql_subregion + "), Exception:" + ex.Message);
            return null;
        }
    }


    /// <summary>
    /// get all country information
    /// </summary>
    /// <returns>return dataset</returns>
    public DataSet getCountryInfo()
    {
        // update by zy 20110117 start
        //string sql_country = "SELECT ID, ISO_Code, Name FROM [Country] WHERE Deleted = 0 "
        //                    + " ORDER BY ID ASC";
        string sql_country = "SELECT ID, ISO_Code, Name FROM [Country] WHERE Deleted = 0 "
                             + " ORDER BY ISO_Code";

        // update by zy 20110117 end
        DataSet ds_country = helper.GetDataSet(sql_country);
        return ds_country;
    }

    /* * * * * Statement: Customer * * * * */

    /// <summary>
    /// get customer information
    /// </summary>
    /// <param name="str">option string</param>
    /// <param name="sel">option type</param>
    /// <returns>customer info</returns>
    public DataSet getCustomerInfo(string str, int sel)
    {
        string sql_customer = "SELECT [Customer].ID, [CustomerName].Name AS 'Customer Name', [CustomerType].Name AS 'Customer Type', [SalesChannel].Name AS 'Sales Channel', [SubRegion].Name AS SubRegion, [Customer].City, [Customer].Address, [Customer].Department FROM [Customer] "
                              + " INNER JOIN [SubRegion] ON [SubRegion].ID = [Customer].CountryID"
                              + " INNER JOIN [CustomerType] ON [Customer].TypeID = [CustomerType].ID"
                              + " INNER JOIN [CustomerName] ON [Customer].NameID = [CustomerName].ID"
                              + " INNER JOIN [SalesChannel] ON [Customer].SalesChannelID = [SalesChannel].ID"
                              +
                              " WHERE [Customer].Deleted = 0 AND [SubRegion].Deleted = 0 AND [CustomerType].Deleted = 0 AND [CustomerName].Deleted = 0"
                              + " AND [SalesChannel].Deleted = 0";
        if (sel == 0)
        {
            sql_customer += " AND [CustomerName].Name like '%" + str + "%'";
            sql_customer += " ORDER BY [CustomerName].Name, [CustomerType].Name,  [SubRegion].Name ASC";
        }
        else if (sel == 1)
        {
            sql_customer += " AND [CustomerType].Name like '%" + str + "%'";
            sql_customer += " ORDER BY [CustomerType].Name, [CustomerName].Name,  [SubRegion].Name ASC";
        }
        else if (sel == 2)
        {
            sql_customer += " AND [SubRegion].Name like '%" + str + "%'";
            sql_customer += " ORDER BY [SubRegion].Name, [CustomerName].Name,  [CustomerType].Name ASC";
        }
        else if (sel == 3)
        {
            sql_customer += " AND [SalesChannel].Name like '%" + str + "%'";
            sql_customer += " ORDER BY [SalesChannel].Name, [CustomerName].Name,  [CustomerType].Name ASC";
        }
        else
            sql_customer += " ORDER BY [CustomerName].Name, [CustomerType].Name,  [SubRegion].Name ASC";

        DataSet ds_customer = helper.GetDataSet(sql_customer);

        return ds_customer;
    }

    /// <summary>
    /// get customer names
    /// </summary>
    /// <returns>DataSet</returns>
    public DataSet getCustomerName()
    {
        string sql_name = "SELECT ID, Name AS 'Customer Name' FROM [CustomerName] WHERE Deleted = 0 ORDER BY Name ASC";
        DataSet ds_name = helper.GetDataSet(sql_name);
        return ds_name;
    }

    public DataSet getCustomerName1()
    {
        var sql = new StringBuilder();
        sql.AppendLine(" SELECT ");
        sql.AppendLine("   Customer.ID, ");
        sql.AppendLine("   CustomerName.Name as 'Customer Name', ");
        sql.AppendLine(" CustomerType.Name as  'Customer Type', ");
        sql.AppendLine(" SubRegion.Name as 'SubRegion Name' ");
        sql.AppendLine(" FROM ");
        sql.AppendLine("   Customer ");
        sql.AppendLine("   INNER JOIN CustomerName ON Customer.NameID=CustomerName.ID ");
        sql.AppendLine("   INNER JOIN SubRegion ON SubRegion.ID=Customer.CountryID ");
        sql.AppendLine("   INNER JOIN CustomerType ON Customer.TypeID=CustomerType.ID ");
        sql.AppendLine(" WHERE ");
        sql.AppendLine("   Customer.Deleted=0 ");
        sql.AppendLine("   AND CustomerName.Deleted=0 ");
        sql.AppendLine("   AND SubRegion.Deleted=0 ");
        sql.AppendLine("   AND CustomerType.Deleted=0 ");
        sql.AppendLine(" ORDER BY ");
        sql.AppendLine("   CustomerName.Name ");
        string query_customer = sql.ToString();

        DataSet ds_customer = helper.GetDataSet(query_customer);

        if (ds_customer != null && ds_customer.Tables.Count > 0 && ds_customer.Tables[0].Rows.Count > 0)
        {
            return ds_customer;
        }
        else
        {
            return null;
        }
    }
    /// <summary>
    /// add a customer name
    /// </summary>
    /// <param name="str_name">customer name</param>
    /// <returns>opration result</returns>
    public bool insertCustomerName(string str_name)
    {
        string ins_name = "INSERT INTO [CustomerName](Name, Deleted) VALUES('" + str_name + "',0)";
        int count = helper.ExecuteNonQuery(CommandType.Text, ins_name, null);
        if (count == 1)
            return true;
        else
            return false;
    }

    /// <summary>
    /// get all customer type
    /// </summary>
    /// <returns>DataSet</returns>
    public DataSet getCustomerType()
    {
        string sql_type = "SELECT ID, Name AS 'Customer Type' FROM [CustomerType] WHERE Deleted = 0 ORDER BY Name ASC";
        DataSet ds_type = helper.GetDataSet(sql_type);
        return ds_type;
    }
    /// <summary>
    /// add customer type.
    /// </summary>
    /// <param name="str_type">customer type to be added.</param>
    /// <returns>if succeed true, failed false.</returns>
    public bool insertCustomerType(string str_type)
    {
        string ins_type = "INSERT INTO [CustomerType](Name, Deleted) VALUES('" + str_type + "',0)";
        int count = helper.ExecuteNonQuery(CommandType.Text, ins_type, null);
        if (count == 1)
            return true;
        else
            return false;
    }

    /// <summary>
    /// Get all project information
    /// </summary>
    /// <returns>DataSet</returns>
    public DataSet getProjectInfo()
    {
        string query_project = "SELECT ID, Name FROM [Project] WHERE Deleted = 0"
                               + " ORDER BY Name ASC";
        DataSet ds = helper.GetDataSet(query_project);

        return ds;
    }

    public DataSet getProjectByID(string id)
    {
        string query_project = "SELECT Probability FROM [Project] WHERE ID = " + id + " AND Deleted=0 ";
        DataSet ds = helper.GetDataSet(query_project);

        return ds;
    }

    public DataSet getCurrency()
    {
        string query_currency = "SELECT Name,ID FROM [Currency] WHERE Deleted=0 GROUP BY Name,ID"
                                + " ORDER BY Name,ID ASC";
        DataSet ds = helper.GetDataSet(query_currency);

        return ds;
    }

    /// <summary>
    ///  Get sales organization information that consist of ID and abbr, 
    ///  these sales orgnizations were made up with rsms in table [Bookings]
    /// </summary>
    /// <returns>return dataset</returns>
    public DataSet getSalesOrgInfo()
    {
        string query_salesOrg = "SELECT [SalesOrg].ID, [SalesOrg].Abbr FROM [SalesOrg]"
                                + " INNER JOIN [Bookings] ON [SalesOrg].ID = [Bookings].SalesOrgID"
                                + " WHERE [SalesOrg].Deleted = 0"
                                + " GROUP BY [SalesOrg].ID, [SalesOrg].Abbr"
                                + " ORDER BY [SalesOrg].Abbr ASC";
        DataSet ds = helper.GetDataSet(query_salesOrg);

        return ds;
    }

    public DataSet getSalesOrgInfoBySeg1(int segmentId)
    {
        string query_salesOrg = "SELECT B.ID, B.Abbr "
                                + " from SalesOrg_Segment A,SalesOrg B "
                                + " where A.SalesOrgID=B.ID and A.Deleted=0 and B.Deleted=0 and SegmentID=" + segmentId;

        DataSet ds = helper.GetDataSet(query_salesOrg);

        return ds;
    }

    public DataSet getSalesOrgByRSM(string rsmid)
    {
        string query_salesOrg = "SELECT [SalesOrg].Abbr,[Currency].Name FROM [SalesOrg]"
                                + " INNER JOIN [SalesOrg_User] ON [SalesOrg].ID = [SalesOrg_User].SalesOrgID"
                                + " INNER JOIN [Currency] ON [Currency].ID = [SalesOrg].CurrencyID"
                                + " WHERE UserID = " + rsmid
                                + " AND SalesOrg.Deleted=0 "
                                + " AND SalesOrg_User.Deleted=0 "
                                + " AND Currency.Deleted=0 ";
        DataSet ds = helper.GetDataSet(query_salesOrg);

        return ds;
    }

    /// <summary>
    ///  Get sales organization information that consist of ID and Name by Segment 
    /// </summary>
    /// <returns>return dataset</returns>
    public DataSet getSalesOrgInfoBySegment(string str_segmentID)
    {
        string query_salesorg = "SELECT [SalesOrg].Name, [SalesOrg].ID, [SalesOrg].Abbr"
                                + " FROM [SalesOrg_Segment] INNER JOIN [SalesOrg] "
                                + " ON [SalesOrg_Segment].SalesOrgID  = [SalesOrg].ID "
                                + " WHERE SegmentID = " + str_segmentID
                                + " AND SalesOrg_Segment.Deleted=0 "
                                + " AND SalesOrg.Deleted=0 "
                                + " ORDER BY [SalesOrg].Name ASC";
        DataSet ds_salesorg = helper.GetDataSet(query_salesorg);

        if (ds_salesorg.Tables[0].Rows.Count > 0)
            return ds_salesorg;
        else
            return null;
    }

    /// <summary>
    /// Get operation information that consist of name and ID by segmentID
    /// </summary>
    /// <param name="str_segmentID">SegmentID</param>
    /// <returns>return dataset</returns>
    public DataSet getOperationBySegment(string str_segmentID)
    {
        string sql_operation = "SELECT [Operation].Name, [Operation].ID,[Operation].Abbr,[Operation].AbbrL FROM [Operation_Segment]"
                               + " INNER JOIN [Operation] ON [Operation].ID = [Operation_Segment].OperationID"
                               + " WHERE SegmentID = " + str_segmentID
                               + " AND Operation_Segment.Deleted=0 "
                               + " AND Operation.Deleted=0 "
                               //+ " GROUP BY [Operation].Name,[Operation].ID,[Operation].Abbr,[Operation].AbbrL"
                               + " ORDER BY [Operation].Name,[Operation].Abbr,[Operation].AbbrL ASC";

        try
        {
            DataSet ds_operation = helper.GetDataSet(sql_operation);
            if (ds_operation.Tables[0].Rows.Count > 0)
            {
                log.WriteLog(LogUtility.LogErrorLevel.LOG_NOISE, "SQLStatment.cs:(" + sql_operation + ")");
            }
            else
            {
                log.WriteLog(LogUtility.LogErrorLevel.LOG_SUSPICIOUS,
                             "SQLStatment.cs:(" + sql_operation +
                             "),The table product is null or the table segment_product is lack for relations.");
            }
            return ds_operation;
        }
        catch (Exception ex)
        {
            log.WriteLog(LogUtility.LogErrorLevel.LOG_CRITICAL,
                         "SQLStatment.cs:(" + sql_operation + "),Exception:" + ex.Message);
            return null;
        }
    }

    /// <summary>
    ///  Get the currency of operation by operationID
    /// </summary>
    /// <param name="str_operationID">OperationID</param>
    /// <returns>return currency</returns>
    public string getOperationCurrency(string str_operationID)
    {
        string sql_operation_currency = "SELECT [Currency].Name, [Operation].Name FROM [Operation] INNER JOIN [Currency]"
                                        + " ON [Operation].CurrencyID = [Currency].ID"
                                        + " WHERE [Operation].ID = " + str_operationID + " AND [Operation].Deleted = 0"
                                        + " AND [Currency].Deleted = 0";
        DataSet ds_operation_currency = helper.GetDataSet(sql_operation_currency);

        string currency = ds_operation_currency.Tables[0].Rows[0][0].ToString().Trim();
        string operationName = ds_operation_currency.Tables[0].Rows[0][1].ToString().Trim();
        if (currency == "" || currency == null)
        {
            currency = "";
            log.WriteLog(LogUtility.LogErrorLevel.LOG_ERROR,
                         "Operation named " + operationName + " whose currency dode not exist.");
        }
        return currency;
    }

    public Dictionary<string, string> getOperationCurrencys(IList<string> operationIds)
    {
        if (operationIds == null || operationIds.Count < 1)
            return null;
        string sql_operation_currency = "SELECT [Operation].ID,[Currency].Name, [Operation].Name FROM [Operation] INNER JOIN [Currency]"
                                        + " ON [Operation].CurrencyID = [Currency].ID"
                                        + " WHERE [Operation].ID in (" + string.Join(",", operationIds.ToArray()) +
                                        ") AND [Operation].Deleted = 0"
                                        + " AND [Currency].Deleted = 0";
        DataSet ds_operation_currency = helper.GetDataSet(sql_operation_currency);

        var result = new Dictionary<string, string>();
        string operation = string.Empty;
        for (int i = 0; i < ds_operation_currency.Tables[0].Rows.Count; i++)
        {
            operation = ds_operation_currency.Tables[0].Rows[i][0].ToString().Trim();
            if (!result.ContainsKey(operation))
                result.Add(operation, ds_operation_currency.Tables[0].Rows[i][1].ToString().Trim());
        }
        return result;
    }
    /// <summary>
    /// get currency of salesOrg by salesOrg id.
    /// </summary>
    /// <param name="str_salesorgID">salsesOrg id</param>
    /// <returns>currency</returns>
    public string getSalesOrgCurrency(string str_salesorgID)
    {
        string sql_salesorg_currency = " SELECT [Currency].Name, [SalesOrg].Abbr FROM [SalesOrg] INNER JOIN [Currency]"
                                       + " ON [SalesOrg].CurrencyID = [Currency].ID"
                                       + " WHERE [SalesOrg].ID = '" + str_salesorgID + "' AND [SalesOrg].Deleted = 0"
                                       + " AND [Currency].Deleted = 0";
        DataSet ds_salesorg_currency = helper.GetDataSet(sql_salesorg_currency);

        string currency = ds_salesorg_currency.Tables[0].Rows[0][0].ToString().Trim();
        string salesorgAbbr = ds_salesorg_currency.Tables[0].Rows[0][1].ToString().Trim();
        if (currency == "" || currency == null)
        {
            currency = "";
            log.WriteLog(LogUtility.LogErrorLevel.LOG_ERROR,
                         "Sales Organization named " + salesorgAbbr + " whose currency dode not exist.");
        }
        return currency;
    }

    /// <summary>
    ///  Get currency rate
    /// </summary>
    /// <param name="str_currency">Currency Name</param>
    /// <param name="sel">true for current Rate, false for next Rate</param>
    /// <param name="str_year">Meeting Year</param>
    /// <param name="str_month">Meeting Month</param>
    /// <returns>return rate</returns>
    public double getRate(string str_currency, bool sel, string str_year, string str_month)
    {
        string sql_currency_rate;
        if (str_currency != "")
        {
            sql_currency_rate = "SELECT Rate1,Rate2 "
                                + " FROM [Currency_Exchange]"
                                + " INNER JOIN [Currency] ON [Currency].ID = [Currency_Exchange].CurrencyID"
                                + " WHERE Name = '" + str_currency + "'"
                                + " AND [Currency_Exchange].Deleted = 0 AND [Currency].Deleted = 0"
                                + " AND YEAR(TimeFlag) = '" + str_year + "'"
                                + " AND MONTH(TimeFlag) = '" + str_month + "'";

            DataSet ds_currency_rate = helper.GetDataSet(sql_currency_rate);
            if (ds_currency_rate.Tables[0].Rows.Count == 1)
            {
                if (sel)
                    return Convert.ToDouble(ds_currency_rate.Tables[0].Rows[0][0]);
                else
                    return Convert.ToDouble(ds_currency_rate.Tables[0].Rows[0][1]);
            }
            else
            {
                log.WriteLog(LogUtility.LogErrorLevel.LOG_CRITICAL,
                             "In " + str_year + "-" + str_month + ",The rate of '" + str_currency + "' was not input");
                return 0;
            }
        }
        else
        {
            log.WriteLog(LogUtility.LogErrorLevel.LOG_CRITICAL,
                         "Some operations or some sales organizations did not have currency.");
            return 0;
        }
    }

    /// <summary>
    ///  Get the percentage of operation currency and sales organization currency,if sel is true, current
    /// </summary>
    /// <param name="str_operationID">operation id</param>
    /// <param name="str_salesorgID">sales org id</param>
    /// <param name="sel">true for current Rate, false for next Rate</param>
    /// <param name="str_year">meeting year</param>
    /// <param name="str_month">meeting month</param>
    /// <returns>the percentage of operation currency and sales organization currency.</returns>
    public double getPercentage(string str_operationID, string str_salesorgID, bool sel, string str_year,
                                string str_month)
    {
        double db_operationRate = getRate(getOperationCurrency(str_operationID), sel, str_year, str_month);
        double db_salesorgRate = getRate(getSalesOrgCurrency(str_salesorgID), sel, str_year, str_month);
        double Rate;
        if (db_operationRate < 0.000000001 && db_operationRate > -0.00000001)
            Rate = 0;
        else
            Rate = db_salesorgRate/db_operationRate;
        return Rate;
    }

    /// <summary>
    /// Get bookings data by sales organization by operation during meetingdate.
    /// GROUP BY Booking Year and Deliver Year.
    /// </summary>
    /// <param name="ds_pro">DataSet products</param>
    /// <param name="str_salesOrgID">string salesorgID</param>
    /// <param name="str_operationID">string operationID</param>
    /// <param name="str_segmentID">string segmentID</param>
    /// <param name="flagMoney">1:need to convert to EUR, 0:not need to convert.</param>
    /// <returns>dataset</returns>
    public DataSet getBookingsDataBySalesOrgByOperation(DataSet ds_pro, string str_salesOrgID, string str_operationID,
                                                        string str_segmentID, int flagMoney)
        //by yyan item8 20110616 add end
    {
        //by ryzhang 20110530 item49 modify start
        string str_year = selectMeeting.getyear();
        string str_month = selectMeeting.getmonth();
        //by ryzhang 20110530 item49 modify end
        double rate1 = getPercentage(str_operationID, str_salesOrgID, true, str_year, str_month);
        double rate2 = getPercentage(str_operationID, str_salesOrgID, false, str_year, str_month);
        if (ds_pro.Tables[0].Rows.Count > 0)
        {
            string sql =
                "SELECT (CASE WHEN DeliverY = 'YTD' THEN '00' ELSE DeliverY END) AS 'NewDeliverY',(BookingY + (CASE WHEN DeliverY <> 'YTD' THEN ' -- ' + DeliverY ELSE DeliverY END)) AS ' '";
            string temp = "";
            //by yyan item8 20110616 add start 
            if (flagMoney == 0)
            {
                //by yyan item8 20110616 add end 
                for (int i = 0; i < ds_pro.Tables[0].Rows.Count; i++)
                {
                    temp += ",ROUND(SUM(CASE WHEN ProductID = " + ds_pro.Tables[0].Rows[i][0].ToString().Trim()
                            //by ryzhang item49 del start
                            //+ " AND YEAR(TimeFlag) = '" + meeting.getyear() + "'"
                            //by ryzhang item49 del end
                            //by ryzhang item49 add start
                            + " AND YEAR(TimeFlag) = '" + selectMeeting.getyear() + "'"
                            + " AND MONTH(TimeFlag) = '" + selectMeeting.getmonth() + "'"
                            //by ryzhang item49 del start
                            + " THEN (CASE WHEN DeliverY = '" + str_year.Substring(2, 2) + "' OR DeliverY = 'YTD'"
                            + " THEN Amount*" + rate1.ToString().Replace(',', '.') + " ELSE Amount*" +
                            rate2.ToString().Replace(',', '.') + " END) ELSE 0 END),0) AS  '" +
                            ds_pro.Tables[0].Rows[i][1].ToString().Trim() + "'";
                }
                temp += " FROM [Bookings]"
                        + " WHERE SalesOrgID = " + str_salesOrgID
                        + " AND SegmentID = " + str_segmentID
                        + " AND OperationID = " + str_operationID
                        + " AND YEAR(TimeFlag) = '" + selectMeeting.getyear() + "'"
                        + " AND MONTH(TimeFlag) = '" + selectMeeting.getmonth() + "'"
                        + " GROUP BY BookingY,DeliverY"
                        + " ORDER BY BookingY,NewDeliverY ASC";
                //by yyan item8 20110616 add start 
            }
            else
            {
                double operationRate1 = getRate(getOperationCurrency(str_operationID), true, str_year, str_month);
                double operationRate2 = getRate(getOperationCurrency(str_operationID), false, str_year, str_month);
                if (operationRate1 < 0.000000001 && operationRate1 > -0.00000001)
                {
                    operationRate1 = 100000000;
                }
                if (operationRate2 < 0.000000001 && operationRate2 > -0.00000001)
                {
                    operationRate2 = 100000000;
                }
                for (int i = 0; i < ds_pro.Tables[0].Rows.Count; i++)
                {
                    temp += ",ROUND(SUM(CASE WHEN ProductID = " + ds_pro.Tables[0].Rows[i][0].ToString().Trim()
                            + " AND YEAR(TimeFlag) = '" + selectMeeting.getyear() + "'"
                            + " AND MONTH(TimeFlag) = '" + selectMeeting.getmonth() + "'";
                    //by yyan 20110819 itemw118 edit start
                    if (str_month == "3" || str_month == "03")
                    {
                        temp += " THEN (CASE WHEN BookingY = '" + str_year.Substring(2, 2) + "' or DeliverY = '" +
                                str_year.Substring(2, 2) + "' OR DeliverY = 'YTD' "
                                + " THEN Amount*" + rate1.ToString().Replace(',', '.') + "*" +
                                operationRate1.ToString().Replace(',', '.') + " ELSE Amount*" +
                                rate2.ToString().Replace(',', '.') + "*" + operationRate2.ToString().Replace(',', '.') +
                                " END) ELSE 0 END),0) AS  '" + ds_pro.Tables[0].Rows[i][1].ToString().Trim() + "'";
                    }
                    else
                    {
                        temp += " THEN (CASE WHEN DeliverY = '" + str_year.Substring(2, 2) + "' OR DeliverY = 'YTD'"
                                + " THEN Amount*" + rate1.ToString().Replace(',', '.') + "*" +
                                operationRate1.ToString().Replace(',', '.') + " ELSE Amount*" +
                                rate2.ToString().Replace(',', '.') + "*" + operationRate2.ToString().Replace(',', '.') +
                                " END) ELSE 0 END),0) AS  '" + ds_pro.Tables[0].Rows[i][1].ToString().Trim() + "'";
                    }
                    //by yyan 20110819 itemw118 edit end           
                }
                temp += " FROM [Bookings]"
                        + " WHERE SalesOrgID = " + str_salesOrgID
                        + " AND SegmentID = " + str_segmentID
                        + " AND OperationID = " + str_operationID
                        + " AND YEAR(TimeFlag) = '" + selectMeeting.getyear() + "'"
                        + " AND MONTH(TimeFlag) = '" + selectMeeting.getmonth() + "'"
                        + " GROUP BY BookingY,DeliverY"
                        + " ORDER BY BookingY,NewDeliverY ASC";
            }
            //by yyan item8 20110616 add end 
            sql += temp;
            DataSet ds = helper.GetDataSet(sql);
            return ds;
        }
        return null;
    }

    /// <summary>
    /// Get bookings data total by sales org by operation.
    /// GROUP BY Booking Year.
    /// </summary>
    /// <param name="ds_pro">DataSet products</param>
    /// <param name="str_salesOrgID">salesOrg id</param>
    /// <param name="str_operationID">operation id</param>
    /// <param name="str_segmentID">segment id</param>
    /// <param name="flagMoney">1:need to convert to EUR, 0:not need to convert.</param>
    /// <returns>dataset</returns>
    public DataSet getBookingsDataTotalBySalesOrgByOperation(DataSet ds_pro, string str_salesOrgID,
                                                             string str_operationID, string str_segmentID, int flagMoney)
        //by yyan item8 20110616 add end
    {
        //by ryzhang 20110530 item49 modify start
        string str_year = selectMeeting.getyear();
        string str_month = selectMeeting.getmonth();
        //by ryzhang 20110530 item49 modify end
        double rate1 = getPercentage(str_operationID, str_salesOrgID, true, str_year, str_month);
        double rate2 = getPercentage(str_operationID, str_salesOrgID, false, str_year, str_month);
        if (ds_pro.Tables[0].Rows.Count > 0)
        {
            string sql = "SELECT ('20' + BookingY) AS ' '";
            string temp = "";
            //by yyan item8 20110616 add start 
            if (flagMoney == 0)
            {
                //by yyan item8 20110616 add end
                for (int i = 0; i < ds_pro.Tables[0].Rows.Count; i++)
                {
                    temp += ",ROUND(SUM(CASE WHEN ProductID = " + ds_pro.Tables[0].Rows[i][0].ToString().Trim()
                            + " THEN (CASE WHEN DeliverY = '" + str_year.Substring(2, 2) + "' OR DeliverY = 'YTD'"
                            + " THEN Amount*" + rate1.ToString().Replace(',', '.') + " ELSE Amount*" +
                            rate2.ToString().Replace(',', '.') + " END) ELSE 0 END),0) AS  '" +
                            ds_pro.Tables[0].Rows[i][1].ToString().Trim() + "'";
                }
                temp += " FROM [Bookings]"
                        + " WHERE SalesOrgID = " + str_salesOrgID
                        + " AND SegmentID = " + str_segmentID
                        + " AND OperationID = " + str_operationID
                        //by ryzhang item49 del start
                        //+ " AND YEAR(TimeFlag) = '" + meeting.getyear() + "' AND MONTH(TimeFlag) = '" + meeting.getmonth() + "'"
                        //by ryzhang item49 del end
                        //by ryzhang item49 add start
                        + " AND YEAR(TimeFlag) = '" + selectMeeting.getyear() + "' AND MONTH(TimeFlag) = '" +
                        selectMeeting.getmonth() + "'"
                        //by ryzhang item49 del start
                        + " GROUP BY BookingY"
                        + " ORDER BY BookingY ASC";
                //by yyan item8 20110616 add start 
            }
            else
            {
                double operationRate1 = getRate(getOperationCurrency(str_operationID), true, str_year, str_month);
                double operationRate2 = getRate(getOperationCurrency(str_operationID), false, str_year, str_month);
                if (operationRate1 < 0.000000001 && operationRate1 > -0.00000001)
                {
                    operationRate1 = 100000000;
                }
                if (operationRate2 < 0.000000001 && operationRate2 > -0.00000001)
                {
                    operationRate2 = 100000000;
                }
                for (int i = 0; i < ds_pro.Tables[0].Rows.Count; i++)
                {
                    temp += ",ROUND(SUM(CASE WHEN ProductID = " + ds_pro.Tables[0].Rows[i][0].ToString().Trim()
                            + " THEN (CASE WHEN DeliverY = '" + str_year.Substring(2, 2) + "' OR DeliverY = 'YTD'"
                            + " THEN Amount*" + rate1.ToString().Replace(',', '.') + "*" +
                            operationRate1.ToString().Replace(',', '.') + " ELSE Amount*" +
                            rate2.ToString().Replace(',', '.') + "*" + operationRate2.ToString().Replace(',', '.') +
                            " END) ELSE 0 END),0) AS  '" + ds_pro.Tables[0].Rows[i][1].ToString().Trim() + "'";
                }
                temp += " FROM [Bookings]"
                        + " WHERE SalesOrgID = " + str_salesOrgID
                        + " AND SegmentID = " + str_segmentID
                        + " AND OperationID = " + str_operationID
                        + " AND YEAR(TimeFlag) = '" + selectMeeting.getyear() + "' AND MONTH(TimeFlag) = '" +
                        selectMeeting.getmonth() + "'"
                        + " GROUP BY BookingY"
                        + " ORDER BY BookingY ASC";
            }
            //by yyan item8 20110616 add end
            sql += temp;
            DataSet ds = helper.GetDataSet(sql);
            return ds;
        }
        return null;
    }

    
    /// <summary>
    /// Get bookings budget by sales org by operation,the budget was calculated in every year.
    /// </summary>
    /// <param name="ds_pro">products</param>
    /// <param name="str_salesOrgID">salesOrg id</param>
    /// <param name="str_operationID">operation id</param>
    /// <param name="str_segmentID">segment id</param>
    /// <param name="flagMoney">1:need to convert to EUR, 0:not need to convert.</param>
    /// <returns>dataset</returns>
    public DataSet getBookingsBudgetBySalesOrgByOperation(DataSet ds_pro, string str_salesOrgID, string str_operationID,
                                                          string str_segmentID, int flagMoney)
        //by yyan item8 20110616 add end
    {
        //by ryzhang 20110530 item49 modify start
        string str_year = selectMeeting.getyear();
        string str_nextyear = selectMeeting.getnextyear();
        //by ryzhang 20110530 item49 modify end
        double rate1 = getPercentage(str_operationID, str_salesOrgID, true, str_year, "03");
        double rate2 = getPercentage(str_operationID, str_salesOrgID, false, str_year, "03");
        if (ds_pro.Tables[0].Rows.Count > 0)
        {
            string sql = "SELECT BookingY";
            string temp = "";
            //by yyan item8 20110616 add start 
            if (flagMoney == 0)
            {
                //by yyan item8 20110616 add end 
                for (int i = 0; i < ds_pro.Tables[0].Rows.Count; i++)
                {
                    temp += ", ROUND(SUM(CASE WHEN ProductID = " + ds_pro.Tables[0].Rows[i][0].ToString().Trim()
                            + " THEN (CASE WHEN DeliverY = '" + str_year.Substring(2, 2) + "' OR DeliverY = 'YTD'"
                            + " THEN Amount*" + rate1.ToString().Replace(',', '.') + " ELSE Amount*" +
                            rate2.ToString().Replace(',', '.') + " END) ELSE 0 END),0) AS  '" +
                            ds_pro.Tables[0].Rows[i][1].ToString().Trim() + "'";
                }
                temp += " FROM [Bookings]"
                        + " WHERE SalesOrgID = " + str_salesOrgID
                        + " AND BookingY = '" + str_nextyear.Substring(2, 2) + "'" //str_nextyear
                        + " AND SegmentID = " + str_segmentID
                        + " AND OperationID = " + str_operationID
                        + " AND (YEAR(TimeFlag) = '" + selectMeeting.getyear() + "' AND MONTH(TimeFlag) = '03')"
                        + " GROUP BY BookingY"
                        + " ORDER BY BookingY ASC";
                //by yyan item8 20110616 add start 
            }
            else
            {
                double operationRate1 = getRate(getOperationCurrency(str_operationID), true, str_year, "03");
                double operationRate2 = getRate(getOperationCurrency(str_operationID), false, str_year, "03");
                if (operationRate1 < 0.000000001 && operationRate1 > -0.00000001)
                {
                    operationRate1 = 100000000;
                }
                if (operationRate2 < 0.000000001 && operationRate2 > -0.00000001)
                {
                    operationRate2 = 100000000;
                }
                for (int i = 0; i < ds_pro.Tables[0].Rows.Count; i++)
                {
                    temp += ", ROUND(SUM(CASE WHEN ProductID = " + ds_pro.Tables[0].Rows[i][0].ToString().Trim()
                            + " THEN (CASE WHEN DeliverY = '" + str_year.Substring(2, 2) + "' OR DeliverY = 'YTD'"
                            + " THEN Amount*" + rate1.ToString().Replace(',', '.') + "*" +
                            operationRate1.ToString().Replace(',', '.') + " ELSE Amount*" +
                            rate2.ToString().Replace(',', '.') + "*" + operationRate2.ToString().Replace(',', '.') +
                            " END) ELSE 0 END),0) AS  '" + ds_pro.Tables[0].Rows[i][1].ToString().Trim() + "'";
                }
                temp += " FROM [Bookings]"
                        + " WHERE SalesOrgID = " + str_salesOrgID
                        + " AND BookingY = '" + str_nextyear.Substring(2, 2) + "'" //str_nextyear
                        + " AND SegmentID = " + str_segmentID
                        + " AND OperationID = " + str_operationID
                        + " AND (YEAR(TimeFlag) = '" + selectMeeting.getyear() + "' AND MONTH(TimeFlag) = '03')"
                        + " GROUP BY BookingY"
                        + " ORDER BY BookingY ASC";
            }
            sql += temp;
            //ryzhang item49 20110510 add end
            DataSet ds = helper.GetDataSet(sql);
            return ds;
        }
        return null;
    }

    
    /// <summary>
    /// get booking data by operation;
    /// GROUP BY Booking Year and Deliver Year.
    /// </summary>
    /// <param name="ds_pro">products</param>
    /// <param name="str_operationID">operation id</param>
    /// <param name="str_segmentID">segment id</param>
    /// <param name="flagmoney">1:need to convert to EUR, 0:not need to convert.</param>
    /// <returns>dataset</returns>
    public DataSet getBookingsDataByOperation(DataSet ds_pro, string str_operationID, string str_segmentID,
                                              int flagmoney)
        //by yyan item8 20110617 add end

    {
        //by ryzhang 20110530 item49 modify start
        string str_year = selectMeeting.getyear();
        string str_month = selectMeeting.getmonth();
        //by ryzhang 20110530 item49 modify end
        double operationRate1 = getRate(getOperationCurrency(str_operationID), true, str_year, str_month);
        double operationRate2 = getRate(getOperationCurrency(str_operationID), false, str_year, str_month);
        if (operationRate1 < 0.000000001 && operationRate1 > -0.00000001)
        {
            operationRate1 = 100000000;
        }
        if (operationRate2 < 0.000000001 && operationRate2 > -0.00000001)
        {
            operationRate2 = 100000000;
        }
        if (ds_pro.Tables[0].Rows.Count > 0)
        {
            string sql =
                "SELECT (CASE WHEN DeliverY = 'YTD' THEN '00'  ELSE DeliverY END) AS 'NewDeliverY',(BookingY + (CASE WHEN DeliverY <> 'YTD' THEN '--' + DeliverY ELSE DeliverY END)) AS ' '";
            string temp = "";
            //by yyan item8 20110617 add start 
            if (flagmoney == 0)
            {
                //by yyan item8 20110617 add end 
                for (int i = 0; i < ds_pro.Tables[0].Rows.Count; i++)
                {
                    temp += ",ROUND(SUM(CASE WHEN ProductID = " + ds_pro.Tables[0].Rows[i][0].ToString().Trim()
                            + " THEN (CASE WHEN DeliverY = '" + str_year.Substring(2, 2) + "' OR DeliverY = 'YTD'"
                            + " THEN Amount/" + operationRate1.ToString().Replace(',', '.') + " ELSE Amount/" +
                            operationRate2.ToString().Replace(',', '.') + " END) ELSE 0 END),0) AS  '" +
                            ds_pro.Tables[0].Rows[i][1].ToString().Trim() + "'";
                }
                string fromsql = "SELECT [SalesOrg].ID,[Bookings].BookingY,"
                                 + " [Bookings].DeliverY,[Bookings].ProductID,[Bookings].OperationID"
                                 + " ,SUM(CASE WHEN [Bookings].DeliverY = '" + str_year.Substring(2, 2) +
                                 "' OR [Bookings].DeliverY = 'YTD'"
                                 +
                                 " THEN [Bookings].Amount*[Currency_Exchange].Rate1 ELSE [Bookings].Amount*[Currency_Exchange].Rate2 END ) AS Amount"
                                 + " ,[Bookings].SegmentID,[Bookings].TimeFlag"
                                 + " FROM [Bookings],[SalesOrg],[Currency_Exchange]"
                                 + " WHERE [SalesOrg].ID = [Bookings].SalesOrgID"
                                 + " AND [SalesOrg].CurrencyID = [Currency_Exchange].CurrencyID"
                                 + " AND YEAR([Currency_Exchange].TimeFlag) = '" + str_year + "'"
                                 + " AND MONTH([Currency_Exchange].TimeFlag) = '" + str_month + "'"
                                 + " AND SalesOrg.Deleted=0 "
                                 + " AND Currency_Exchange.Deleted=0 "
                                 + " GROUP BY [SalesOrg].ID,[Bookings].BookingY,[Bookings].DeliverY,"
                                 + "  [Bookings].ProductID,[Bookings].OperationID"
                                 + " ,[Bookings].SegmentID,[Bookings].TimeFlag";

                temp += " FROM (" + fromsql + ") AS NewBookings"
                        + " WHERE SegmentID = " + str_segmentID
                        + " AND OperationID = " + str_operationID
                        + " AND YEAR(TimeFlag) = '" + selectMeeting.getyear() + "' AND MONTH(TimeFlag) = '" +
                        selectMeeting.getmonth() + "'"
                        + " GROUP BY BookingY,DeliverY"
                        + " ORDER BY BookingY,NewDeliverY ASC";
                //by yyan item8 20110617 add start 
            }
            else
            {
                for (int i = 0; i < ds_pro.Tables[0].Rows.Count; i++)
                {
                    //temp += ",SUM(CASE WHEN ProductID = " + ds_pro.Tables[0].Rows[i][0].ToString().Trim()
                    //     + " THEN (CASE WHEN DeliverY = '" + str_year.Substring(2, 2) + "' OR DeliverY = 'YTD'"
                    //     + " THEN ROUND(Amount,0) ELSE ROUND(Amount,0) END) ELSE 0 END) AS  '" + ds_pro.Tables[0].Rows[i][1].ToString().Trim() + "'";

                    temp += ",ROUND(SUM(CASE WHEN ProductID = " + ds_pro.Tables[0].Rows[i][0].ToString().Trim()
                            + " THEN (CASE WHEN DeliverY = '" + str_year.Substring(2, 2) + "' OR DeliverY = 'YTD'"
                            + " THEN Amount ELSE Amount END) ELSE 0 END),0) AS  '" +
                            ds_pro.Tables[0].Rows[i][1].ToString().Trim() + "'";
                }
                string fromsql = "SELECT [SalesOrg].ID,[Bookings].BookingY,"
                                 + " [Bookings].DeliverY,[Bookings].ProductID,[Bookings].OperationID";
                //by yyan 20110819 itemw118 edit start
                if (str_month == "3" || str_month == "03")
                {
                    fromsql += " ,SUM(CASE WHEN [Bookings].bookingY = '" + str_year.Substring(2, 2) +
                               "'  or DeliverY = '" + str_year.Substring(2, 2) + "' OR DeliverY = 'YTD' "
                               +
                               " THEN [Bookings].Amount*[Currency_Exchange].Rate1 ELSE [Bookings].Amount*[Currency_Exchange].Rate2 END ) AS Amount";
                }
                else
                {
                    fromsql += " ,SUM(CASE WHEN [Bookings].DeliverY = '" + str_year.Substring(2, 2) +
                               "' OR [Bookings].DeliverY = 'YTD'"
                               +
                               " THEN [Bookings].Amount*[Currency_Exchange].Rate1 ELSE [Bookings].Amount*[Currency_Exchange].Rate2 END ) AS Amount";
                }
                //by yyan 20110819 itemw118 edit end
                fromsql += " ,[Bookings].SegmentID,[Bookings].TimeFlag"
                           + " FROM [Bookings],[SalesOrg],[Currency_Exchange]"
                           + " WHERE [SalesOrg].ID = [Bookings].SalesOrgID"
                           + " AND [SalesOrg].CurrencyID = [Currency_Exchange].CurrencyID"
                           + " AND YEAR([Currency_Exchange].TimeFlag) = '" + str_year + "'"
                           + " AND MONTH([Currency_Exchange].TimeFlag) = '" + str_month + "'"
                           + " AND SalesOrg.Deleted=0 "
                           + " AND Currency_Exchange.Deleted=0 "
                           + " GROUP BY [SalesOrg].ID,[Bookings].BookingY,[Bookings].DeliverY,"
                           + "  [Bookings].ProductID,[Bookings].OperationID"
                           + " ,[Bookings].SegmentID,[Bookings].TimeFlag";

                temp += " FROM (" + fromsql + ") AS NewBookings"
                        + " WHERE SegmentID = " + str_segmentID
                        + " AND OperationID = " + str_operationID
                        + " AND YEAR(TimeFlag) = '" + selectMeeting.getyear() + "' AND MONTH(TimeFlag) = '" +
                        selectMeeting.getmonth() + "'"
                        + " GROUP BY BookingY,DeliverY"
                        + " ORDER BY BookingY,NewDeliverY ASC";
            }
            //by yyan item8 20110617 add end 
            sql += temp;
            //ryzhang item49 20110510 add end
            DataSet ds = helper.GetDataSet(sql);
            return ds;
        }
        return null;
    }

   
    /// <summary>
    /// Get Bookings total data by operation;
    /// GROUP BY Booking Year.
    /// </summary>
    /// <param name="ds_pro">products</param>
    /// <param name="str_operationID">operation id</param>
    /// <param name="str_segmentID">segment id</param>
    /// <param name="flagMoney">1:need to convert to EUR, 0:not need to convert.</param>
    /// <returns>dataset/returns>
    public DataSet getBookingsDataTotalByOperation(DataSet ds_pro, string str_operationID, string str_segmentID,
                                                   int flagMoney)
        //by yyan item8 20110617 add end
    {
        //by ryzhang 20110530 item49 modify start
        string str_year = selectMeeting.getyear();
        string str_month = selectMeeting.getmonth();
        //by ryzhang 20110530 item49 modify end
        double operationRate1 = getRate(getOperationCurrency(str_operationID), true, str_year, str_month);
        double operationRate2 = getRate(getOperationCurrency(str_operationID), false, str_year, str_month);
        if (operationRate1 < 0.000000001 && operationRate1 > -0.00000001)
        {
            operationRate1 = 100000000;
        }
        if (operationRate2 < 0.000000001 && operationRate2 > -0.00000001)
        {
            operationRate2 = 100000000;
        }
        if (ds_pro.Tables[0].Rows.Count > 0)
        {
            //ryzhang item49 20110510 add start
            string sql = "SELECT ('20' + BookingY) AS ' '";
            string temp = "";
            //by yyan item8 20110617 add start 
            if (flagMoney == 0)
            {
                //by yyan item8 20110617 add end 
                for (int i = 0; i < ds_pro.Tables[0].Rows.Count; i++)
                {
                    //temp += ",SUM(CASE WHEN ProductID = " + ds_pro.Tables[0].Rows[i][0].ToString().Trim()
                    //     + " THEN (CASE WHEN DeliverY = '" + str_year.Substring(2, 2) + "' OR DeliverY = 'YTD'"
                    //     + " THEN ROUND(Amount/" + operationRate1.ToString().Replace(',', '.') + ",0) ELSE ROUND(Amount/" + operationRate2.ToString().Replace(',', '.') + ",0) END) ELSE 0 END) AS  '" + ds_pro.Tables[0].Rows[i][1].ToString().Trim() + "'";

                    temp += ",ROUND(SUM(CASE WHEN ProductID = " + ds_pro.Tables[0].Rows[i][0].ToString().Trim()
                            + " THEN (CASE WHEN DeliverY = '" + str_year.Substring(2, 2) + "' OR DeliverY = 'YTD'"
                            + " THEN Amount/" + operationRate1.ToString().Replace(',', '.') + " ELSE Amount/" +
                            operationRate2.ToString().Replace(',', '.') + " END) ELSE 0 END),0) AS  '" +
                            ds_pro.Tables[0].Rows[i][1].ToString().Trim() + "'";
                }
                string fromsql = "SELECT [SalesOrg].ID, [Bookings].BookingY,"
                                 + " [Bookings].DeliverY,[Bookings].ProductID,[Bookings].OperationID"
                                 + " ,SUM(CASE WHEN [Bookings].DeliverY = '" + str_year.Substring(2, 2) +
                                 "' OR [Bookings].DeliverY = 'YTD'"
                                 +
                                 " THEN [Bookings].Amount*[Currency_Exchange].Rate1 ELSE [Bookings].Amount*[Currency_Exchange].Rate2 END ) AS Amount"
                                 + " ,[Bookings].SegmentID,[Bookings].TimeFlag"
                                 + " FROM [Bookings],[SalesOrg],[Currency_Exchange]"
                                 + " WHERE [SalesOrg].ID = [Bookings].SalesOrgID"
                                 + " AND [SalesOrg].CurrencyID = [Currency_Exchange].CurrencyID"
                                 + " AND YEAR([Currency_Exchange].TimeFlag) = '" + str_year + "'"
                                 + " AND MONTH([Currency_Exchange].TimeFlag) = '" + str_month + "'"
                                 + " AND SalesOrg.Deleted=0 "
                                 + " AND Currency_Exchange.Deleted=0 "
                                 + " GROUP BY [SalesOrg].ID,[Bookings].CountryID"
                                 + " ,[Bookings].BookingY,[Bookings].DeliverY,"
                                 + "  [Bookings].ProductID,[Bookings].OperationID"
                                 + " ,[Bookings].SegmentID,[Bookings].TimeFlag";
                temp += " FROM (" + fromsql + ") AS NewBookings"
                        + " WHERE SegmentID = " + str_segmentID
                        + " AND OperationID = " + str_operationID
                        + " AND YEAR(TimeFlag) = '" + selectMeeting.getyear() + "'"
                        + " AND MONTH(TimeFlag) = '" + selectMeeting.getmonth() + "'"
                        + " GROUP BY BookingY"
                        + " ORDER BY BookingY ASC";
                //by yyan item8 20110617 add start 
            }
            else
            {
                for (int i = 0; i < ds_pro.Tables[0].Rows.Count; i++)
                {
                    //temp += ",SUM(CASE WHEN ProductID = " + ds_pro.Tables[0].Rows[i][0].ToString().Trim()
                    //     + " THEN (CASE WHEN DeliverY = '" + str_year.Substring(2, 2) + "' OR DeliverY = 'YTD'"
                    //     + " THEN ROUND(Amount,0) ELSE ROUND(Amount,0) END) ELSE 0 END) AS  '" + ds_pro.Tables[0].Rows[i][1].ToString().Trim() + "'";
                    temp += ",ROUND(SUM(CASE WHEN ProductID = " + ds_pro.Tables[0].Rows[i][0].ToString().Trim()
                            + " THEN (CASE WHEN DeliverY = '" + str_year.Substring(2, 2) + "' OR DeliverY = 'YTD'"
                            + " THEN Amount ELSE Amount END) ELSE 0 END),0) AS  '" +
                            ds_pro.Tables[0].Rows[i][1].ToString().Trim() + "'";
                }
                string fromsql = "SELECT [SalesOrg].ID, [Bookings].BookingY,"
                                 + " [Bookings].DeliverY,[Bookings].ProductID,[Bookings].OperationID"
                                 + " ,SUM(CASE WHEN [Bookings].DeliverY = '" + str_year.Substring(2, 2) +
                                 "' OR [Bookings].DeliverY = 'YTD'"
                                 +
                                 " THEN [Bookings].Amount*[Currency_Exchange].Rate1 ELSE [Bookings].Amount*[Currency_Exchange].Rate2 END ) AS Amount"
                                 + " ,[Bookings].SegmentID,[Bookings].TimeFlag"
                                 + " FROM [Bookings],[SalesOrg],[Currency_Exchange]"
                                 + " WHERE [SalesOrg].ID = [Bookings].SalesOrgID"
                                 + " AND [SalesOrg].CurrencyID = [Currency_Exchange].CurrencyID"
                                 + " AND YEAR([Currency_Exchange].TimeFlag) = '" + str_year + "'"
                                 + " AND MONTH([Currency_Exchange].TimeFlag) = '" + str_month + "'"
                                 + " AND SalesOrg.Deleted=0 "
                                 + " AND Currency_Exchange.Deleted=0 "
                                 + " GROUP BY [SalesOrg].ID,[Bookings].CountryID"
                                 + " ,[Bookings].BookingY,[Bookings].DeliverY,"
                                 + "  [Bookings].ProductID,[Bookings].OperationID"
                                 + " ,[Bookings].SegmentID,[Bookings].TimeFlag";
                temp += " FROM (" + fromsql + ") AS NewBookings"
                        + " WHERE SegmentID = " + str_segmentID
                        + " AND OperationID = " + str_operationID
                        + " AND YEAR(TimeFlag) = '" + selectMeeting.getyear() + "'"
                        + " AND MONTH(TimeFlag) = '" + selectMeeting.getmonth() + "'"
                        + " GROUP BY BookingY"
                        + " ORDER BY BookingY ASC";
            }
            //by yyan item8 20110617 add end 
            sql += temp;
            //ryzhang item49 20110510 add end
            DataSet ds = helper.GetDataSet(sql);
            return ds;
        }
        return null;
    }

    
    /// <summary>
    /// get booking sales by operation
    /// </summary>
    /// <param name="ds_pro">products</param>
    /// <param name="str_operationID">operation id</param>
    /// <param name="str_segmentID">segment id</param>
    /// <param name="flagMoney">1:need to convert to EUR, 0:not need to convert.</param>
    /// <returns>dataset</returns>
    public DataSet getBookingsDataTotalByOperationtoSales(DataSet ds_pro, string str_operationID, string str_segmentID,
                                                          int flagMoney)
        //by yyan item8 20110617 add end
    {
        //by ryzhang 20110530 item49 modify start
        string str_year = selectMeeting.getyear();
        string str_month = selectMeeting.getmonth();
        //by ryzhang 20110530 item49 modify end
        double operationRate1 = getRate(getOperationCurrency(str_operationID), true, str_year, str_month);
        double operationRate2 = getRate(getOperationCurrency(str_operationID), false, str_year, str_month);
        if (operationRate1 < 0.000000001 && operationRate1 > -0.00000001)
        {
            operationRate1 = 100000000;
        }
        if (operationRate2 < 0.000000001 && operationRate2 > -0.00000001)
        {
            operationRate2 = 100000000;
        }
        if (ds_pro.Tables[0].Rows.Count > 0)
        {
            //by ryzhang item49 201105019 del start
            //by yyan item8 20110617 add start 
            string sql = "SELECT ('Bookings ' + BookingY + 'F') AS ' '";
            string temp = "";
            if (flagMoney == 0)
            {
                for (int i = 0; i < ds_pro.Tables[0].Rows.Count; i++)
                {
                    temp += ",SUM(CASE WHEN ProductID = " + ds_pro.Tables[0].Rows[i][0].ToString().Trim()
                            + " THEN (CASE WHEN DeliverY = '" + str_year.Substring(2, 2) + "' OR DeliverY = 'YTD'"
                            + " THEN ROUND(Amount/" + operationRate1.ToString().Replace(',', '.') +
                            ",0) ELSE ROUND(Amount/" + operationRate2.ToString().Replace(',', '.') +
                            ",0) END) ELSE 0 END) AS  '" + ds_pro.Tables[0].Rows[i][1].ToString().Trim() + "'";
                }
                string fromsql = "SELECT [SalesOrg].ID,[Bookings].BookingY,"
                                 + " [Bookings].DeliverY,[Bookings].ProductID,[Bookings].OperationID"
                                 + " ,SUM(CASE WHEN [Bookings].DeliverY = '" + str_year.Substring(2, 2) +
                                 "' OR [Bookings].DeliverY = 'YTD'"
                                 +
                                 " THEN [Bookings].Amount*[Currency_Exchange].Rate1 ELSE [Bookings].Amount*[Currency_Exchange].Rate2 END ) AS Amount"
                                 + " ,[Bookings].SegmentID,[Bookings].TimeFlag"
                                 + " FROM [Bookings],[SalesOrg],[Currency_Exchange]"
                                 + " WHERE [SalesOrg].ID = [Bookings].SalesOrgID"
                                 + " AND [SalesOrg].CurrencyID = [Currency_Exchange].CurrencyID"
                                 + " AND YEAR([Currency_Exchange].TimeFlag) = '" + str_year + "'"
                                 + " AND MONTH([Currency_Exchange].TimeFlag) = '" + str_month + "'"
                                 + " AND SalesOrg.Deleted=0 "
                                 + " AND Currency_Exchange.Deleted=0 "
                                 + " GROUP BY [SalesOrg].ID,[Bookings].BookingY,[Bookings].DeliverY,"
                                 + "  [Bookings].ProductID,[Bookings].OperationID"
                                 + " ,[Bookings].SegmentID,[Bookings].TimeFlag";
                temp += " FROM (" + fromsql + ") AS NewBookings"
                        + " WHERE SegmentID = " + str_segmentID
                        + " AND OperationID = " + str_operationID
                        + " AND YEAR(TimeFlag) = '" + str_year + "'"
                        + " AND MONTH(TimeFlag) = '" + str_month + "'"
                        + " AND BookingY = '" + selectMeeting.getnextyear().Substring(2, 2) + "'"
                        + " GROUP BY BookingY"
                        + " ORDER BY BookingY ASC";
            }
            else
            {
                for (int i = 0; i < ds_pro.Tables[0].Rows.Count; i++)
                {
                    temp += ",SUM(CASE WHEN ProductID = " + ds_pro.Tables[0].Rows[i][0].ToString().Trim()
                            + " THEN (CASE WHEN DeliverY = '" + str_year.Substring(2, 2) + "' OR DeliverY = 'YTD'"
                            + " THEN ROUND(Amount,0) ELSE ROUND(Amount,0) END) ELSE 0 END) AS  '" +
                            ds_pro.Tables[0].Rows[i][1].ToString().Trim() + "'";
                }
                string fromsql = "SELECT [SalesOrg].ID,[Bookings].BookingY,"
                                 + " [Bookings].DeliverY,[Bookings].ProductID,[Bookings].OperationID"
                                 + " ,SUM(CASE WHEN [Bookings].DeliverY = '" + str_year.Substring(2, 2) +
                                 "' OR [Bookings].DeliverY = 'YTD'"
                                 +
                                 " THEN [Bookings].Amount*[Currency_Exchange].Rate1 ELSE [Bookings].Amount*[Currency_Exchange].Rate2 END ) AS Amount"
                                 + " ,[Bookings].SegmentID,[Bookings].TimeFlag"
                                 + " FROM [Bookings],[SalesOrg],[Currency_Exchange]"
                                 + " WHERE [SalesOrg].ID = [Bookings].SalesOrgID"
                                 + " AND [SalesOrg].CurrencyID = [Currency_Exchange].CurrencyID"
                                 + " AND YEAR([Currency_Exchange].TimeFlag) = '" + str_year + "'"
                                 + " AND MONTH([Currency_Exchange].TimeFlag) = '" + str_month + "'"
                                 + " AND SalesOrg.Deleted=0 "
                                 + " AND Currency_Exchange.Deleted=0 "
                                 + " GROUP BY [SalesOrg].ID,[Bookings].BookingY,[Bookings].DeliverY,"
                                 + "  [Bookings].ProductID,[Bookings].OperationID"
                                 + " ,[Bookings].SegmentID,[Bookings].TimeFlag";
                temp += " FROM (" + fromsql + ") AS NewBookings"
                        + " WHERE SegmentID = " + str_segmentID
                        + " AND OperationID = " + str_operationID
                        + " AND YEAR(TimeFlag) = '" + str_year + "'"
                        + " AND MONTH(TimeFlag) = '" + str_month + "'"
                        + " AND BookingY = '" + selectMeeting.getnextyear().Substring(2, 2) + "'"
                        + " GROUP BY BookingY"
                        + " ORDER BY BookingY ASC";
            }
            sql += temp;

            DataSet ds = helper.GetDataSet(sql);
            return ds;
        }
        return null;
    }

    /// <summary>
    ///  Get bookings budget by sales org by operation,the budget was calculated in every year
    /// </summary>
    /// <param name="ds_pro">products</param>
    /// <param name="str_operationID">operation id</param>
    /// <param name="str_segmentID">segment id</param>
    /// <param name="flagMoney">1:need to convert to EUR, 0:not need to convert.</param>
    /// <returns>dataset</returns>
    public DataSet getBookingsBudgetTotalByOperation(DataSet ds_pro, string str_operationID, string str_segmentID,
                                                     int flagMoney)
        //by yyan item8 20110617 add end

    {
        //by ryzhang 20110530 item49 modify start
        string str_year = selectMeeting.getyear();
        string str_nextyear = selectMeeting.getnextyear();
        //yyan itemw140 20110909 edit start
        string str_month = "03"; //selectMeeting.getmonth();
        //yyan itemw140 20110909 edit end
        //by ryzhang 20110530 item49 modify end
        double operationRate1 = getRate(getOperationCurrency(str_operationID), true, str_year, "03");
        double operationRate2 = getRate(getOperationCurrency(str_operationID), false, str_year, "03");
        if (operationRate1 < 0.000000001 && operationRate1 > -0.00000001)
        {
            operationRate1 = 100000000;
        }
        if (operationRate2 < 0.000000001 && operationRate2 > -0.00000001)
        {
            operationRate2 = 100000000;
        }
        if (ds_pro.Tables[0].Rows.Count > 0)
        {
            //by ryzhang item49 201105019 del start
            //by yyan item8 20110616 add start
            string sql = "SELECT BookingY";
            string temp = "";
            //by yyan item8 20110617 add start 
            if (flagMoney == 0)
            {
                //by yyan item8 20110617 add end 
                for (int i = 0; i < ds_pro.Tables[0].Rows.Count; i++)
                {
                    temp += ", ROUND(SUM(CASE WHEN ProductID = " + ds_pro.Tables[0].Rows[i][0].ToString().Trim()
                            + " THEN (CASE WHEN DeliverY = '" + str_year.Substring(2, 2) + "' OR DeliverY = 'YTD'"
                            + " THEN Amount/" + operationRate1.ToString().Replace(',', '.') + " ELSE Amount/" +
                            operationRate2.ToString().Replace(',', '.') + " END) ELSE 0 END),0) AS  '" +
                            ds_pro.Tables[0].Rows[i][1].ToString().Trim() + "'";
                }
                string fromsql = "SELECT [SalesOrg].ID,[Bookings].BookingY,"
                                 + " [Bookings].DeliverY,[Bookings].ProductID,[Bookings].OperationID"
                                 + " ,SUM(CASE WHEN [Bookings].DeliverY = '" + str_year.Substring(2, 2) +
                                 "' OR [Bookings].DeliverY = 'YTD'"
                                 +
                                 " THEN [Bookings].Amount*[Currency_Exchange].Rate1 ELSE [Bookings].Amount*[Currency_Exchange].Rate2 END ) AS Amount"
                                 + " ,[Bookings].SegmentID,[Bookings].TimeFlag"
                                 + " FROM [Bookings],[SalesOrg],[Currency_Exchange]"
                                 + " WHERE [SalesOrg].ID = [Bookings].SalesOrgID"
                                 + " AND [SalesOrg].CurrencyID = [Currency_Exchange].CurrencyID"
                                 + " AND YEAR([Currency_Exchange].TimeFlag) = '" + str_year + "'"
                                 + " AND MONTH([Currency_Exchange].TimeFlag) = '" + str_month + "'"
                                 + " AND SalesOrg.Deleted=0 "
                                 + " AND Currency_Exchange.Deleted=0 "
                                 + " GROUP BY [SalesOrg].ID,[Bookings].BookingY,[Bookings].DeliverY,"
                                 + " [Bookings].ProductID,[Bookings].OperationID"
                                 + " ,[Bookings].SegmentID,[Bookings].TimeFlag";
                temp += " FROM (" + fromsql + ") AS NewBookings"
                        + " WHERE BookingY = '" + str_nextyear.Substring(2, 2) + "'" //str_nextyear
                        + " AND SegmentID = " + str_segmentID
                        + " AND OperationID = " + str_operationID
                        + " AND (YEAR(TimeFlag) = '" + str_year + "' AND MONTH(TimeFlag) = '03')"
                        + " GROUP BY BookingY"
                        + " ORDER BY BookingY ASC";
                //by yyan item8 20110617 add start 
            }
            else
            {
                for (int i = 0; i < ds_pro.Tables[0].Rows.Count; i++)
                {
                    temp += ", ROUND(SUM(CASE WHEN ProductID = " + ds_pro.Tables[0].Rows[i][0].ToString().Trim()
                            + " THEN (CASE WHEN DeliverY = '" + str_year.Substring(2, 2) + "' OR DeliverY = 'YTD'"
                            + " THEN Amount ELSE Amount END) ELSE 0 END),0) AS  '" +
                            ds_pro.Tables[0].Rows[i][1].ToString().Trim() + "'";
                }
                string fromsql = "SELECT [SalesOrg].ID,[Bookings].BookingY,"
                                 + " [Bookings].DeliverY,[Bookings].ProductID,[Bookings].OperationID"
                                 + " ,SUM(CASE WHEN [Bookings].DeliverY = '" + str_year.Substring(2, 2) +
                                 "' OR [Bookings].DeliverY = 'YTD'"
                                 +
                                 " THEN [Bookings].Amount*[Currency_Exchange].Rate1 ELSE [Bookings].Amount*[Currency_Exchange].Rate2 END ) AS Amount"
                                 + " ,[Bookings].SegmentID,[Bookings].TimeFlag"
                                 + " FROM [Bookings],[SalesOrg],[Currency_Exchange]"
                                 + " WHERE [SalesOrg].ID = [Bookings].SalesOrgID"
                                 + " AND [SalesOrg].CurrencyID = [Currency_Exchange].CurrencyID"
                                 + " AND YEAR([Currency_Exchange].TimeFlag) = '" + str_year + "'"
                                 + " AND MONTH([Currency_Exchange].TimeFlag) = '" + str_month + "'"
                                 + " AND SalesOrg.Deleted=0 "
                                 + " AND Currency_Exchange.Deleted=0 "
                                 + " GROUP BY [SalesOrg].ID,[Bookings].BookingY,[Bookings].DeliverY,"
                                 + " [Bookings].ProductID,[Bookings].OperationID"
                                 + " ,[Bookings].SegmentID,[Bookings].TimeFlag";
                temp += " FROM (" + fromsql + ") AS NewBookings"
                        + " WHERE BookingY = '" + str_nextyear.Substring(2, 2) + "'" //str_nextyear
                        + " AND SegmentID = " + str_segmentID
                        + " AND OperationID = " + str_operationID
                        + " AND (YEAR(TimeFlag) = '" + str_year + "' AND MONTH(TimeFlag) = '03')"
                        + " GROUP BY BookingY"
                        + " ORDER BY BookingY ASC";
            }
            //by yyan item8 20110617 add end 
            sql += temp;

            DataSet ds = helper.GetDataSet(sql);
            return ds;
        }
        return null;
    }

    /// <summary>
    /// Get bookings data by segment id;
    /// GROUP BY Booking Year and Deliver Year.
    /// </summary>
    /// <param name="ds_pro">products</param>
    /// <param name="str_segmentID">segment id</param>
    /// <returns>dataset</returns>
    public DataSet getBookingsData(DataSet ds_pro, string str_segmentID)
    {
        //by ryzhang 20110530 item49 modify start
        string str_year = selectMeeting.getyear();
        string str_month = selectMeeting.getmonth();
        //by ryzhang 20110530 item49 modify end
        if (ds_pro.Tables[0].Rows.Count > 0)
        {
            string sql;
            sql =
                "SELECT (CASE WHEN [Bookings].DeliverY ='YTD' THEN '00' ELSE [Bookings].DeliverY END) AS 'NewDeliverY',([Bookings].BookingY + (CASE WHEN [Bookings].DeliverY <> 'YTD' THEN '--' + [Bookings].DeliverY ELSE [Bookings].DeliverY END)) AS ' '";
            string temp = "";
            for (int i = 0; i < ds_pro.Tables[0].Rows.Count; i++)
            {
                temp += ",SUM(CASE WHEN [Bookings].ProductID = " + ds_pro.Tables[0].Rows[i][0].ToString().Trim();

                //by yyan 20110819 itemw118 edit start
                if (str_month == "3" || str_month == "03")
                {
                    temp += " THEN (CASE WHEN  [Bookings].bookingY = '" + str_year.Substring(2, 2) +
                            "'  or DeliverY = '" + str_year.Substring(2, 2) + "' OR DeliverY = 'YTD' THEN"
                            +
                            " ROUND([Bookings].Amount*[Currency_Exchange].Rate1,0) ELSE ROUND([Bookings].Amount*[Currency_Exchange].Rate2,0) END) ELSE 0 END)";
                }
                else
                {
                    temp += " THEN (CASE WHEN  [Bookings].DeliverY = '" + str_year.Substring(2, 2) +
                            "' OR [Bookings].DeliverY = 'YTD' THEN"
                            +
                            " ROUND([Bookings].Amount*[Currency_Exchange].Rate1,0) ELSE ROUND([Bookings].Amount*[Currency_Exchange].Rate2,0) END) ELSE 0 END)";
                }
                //by yyan 20110819 itemw118 edit end
                temp += " AS  '" + ds_pro.Tables[0].Rows[i][1].ToString().Trim() + "'";
            }
            temp += " FROM [Bookings],[SalesOrg],[Currency_Exchange]"
                    + " WHERE [Bookings].SegmentID = " + str_segmentID
                    + " AND [Bookings].SalesOrgID = [SalesOrg].ID"
                    + " AND [Currency_Exchange].CurrencyID = [SalesOrg].CurrencyID";
            temp += " AND YEAR([Bookings].TimeFlag) = '" + selectMeeting.getyear() + "'"
                    + " AND MONTH([Bookings].TimeFlag) = '" + selectMeeting.getmonth() + "'"
                    + " AND YEAR([Currency_Exchange].TimeFlag) = '" + str_year + "'"
                    + " AND MONTH([Currency_Exchange].TimeFlag) = '" + str_month + "'"
                    + " AND SalesOrg.Deleted=0 "
                    + " AND Currency_Exchange.Deleted=0 "
                    + " GROUP BY [Bookings].BookingY,[Bookings].DeliverY"
                    + " ORDER BY [Bookings].BookingY,NewDeliverY ASC";
            sql += temp;
            //by ryzhang item49 20110520 add end
            DataSet ds = helper.GetDataSet(sql);
            return ds;
        }
        return null;
    }

    /// <summary>
    /// Get bookings data total by segment id
    /// GROUP BY Booking Year.
    /// </summary>
    /// <param name="ds_pro">products</param>
    /// <param name="str_segmentID">segment id</param>
    /// <returns>dataset</returns>
    public DataSet getBookingsDataTotal(DataSet ds_pro, string str_segmentID)
    {
        //by ryzhang 20110530 item49 modify start
        string str_year = selectMeeting.getyear();
        string str_month = selectMeeting.getmonth();
        //by ryzhang 20110530 item49 modify end
        if (ds_pro.Tables[0].Rows.Count > 0)
        {
            string sql;
            sql = "SELECT ('20' + [Bookings].BookingY) AS ' '";
            string temp = "";
            for (int i = 0; i < ds_pro.Tables[0].Rows.Count; i++)
            {
                temp += ",SUM(CASE WHEN [Bookings].ProductID = " + ds_pro.Tables[0].Rows[i][0].ToString().Trim()
                        + " THEN (CASE WHEN [Bookings].DeliverY = '" + selectMeeting.getyear().Substring(2, 2) +
                        "' OR [Bookings].DeliverY = 'YTD' THEN"
                        +
                        " ROUND([Bookings].Amount*[Currency_Exchange].Rate1,0) ELSE ROUND([Bookings].Amount*[Currency_Exchange].Rate2,0) END) ELSE 0 END)"
                        + " AS  '" + ds_pro.Tables[0].Rows[i][1].ToString().Trim() + "'";
            }
            temp += " FROM [Bookings],[SalesOrg],[Currency_Exchange]"
                    + " WHERE [Bookings].SegmentID = " + str_segmentID
                    + " AND [Bookings].SalesOrgID = [SalesOrg].ID"
                    + " AND [Currency_Exchange].CurrencyID = [SalesOrg].CurrencyID";
            temp += " AND YEAR([Bookings].TimeFlag) = '" + selectMeeting.getyear() + "'"
                    + " AND MONTH([Bookings].TimeFlag) = '" + selectMeeting.getmonth() + "'"
                    + " AND YEAR([Currency_Exchange].TimeFlag) = '" + str_year + "'"
                    + " AND MONTH([Currency_Exchange].TimeFlag) = '" + str_month + "'"
                    + " AND SalesOrg.Deleted=0 "
                    + " AND Currency_Exchange.Deleted=0 "
                    + " GROUP BY [Bookings].BookingY"
                    + " ORDER BY [Bookings].BookingY ASC";
            sql += temp;
            //by ryzhang item49 20110520 add end
            DataSet ds = helper.GetDataSet(sql);
            return ds;
        }
        return null;
    }

    /// <summary>
    /// Get bookings data , use for GR-Sales
    /// </summary>
    /// <param name="ds_pro">products</param>
    /// <param name="str_segmentID">segment id</param>
    /// <returns>dataset</returns>
    public DataSet getBookingsDataTotaltoSales(DataSet ds_pro, string str_segmentID)
    {
        //by ryzhang 20110530 item49 modify start
        string str_year = selectMeeting.getyear();
        string str_month = selectMeeting.getmonth();
        //by ryzhang 20110530 item49 modify end
        if (ds_pro.Tables[0].Rows.Count > 0)
        {
            string sql = "SELECT ('Bookings ' + [Bookings].BookingY + 'F') AS ' '";
            string temp = "";
            for (int i = 0; i < ds_pro.Tables[0].Rows.Count; i++)
            {
                temp += ",SUM(CASE WHEN [Bookings].ProductID = " + ds_pro.Tables[0].Rows[i][0].ToString().Trim()
                        + " THEN (CASE WHEN [Bookings].DeliverY = '" + selectMeeting.getyear().Substring(2, 2) +
                        "' OR [Bookings].DeliverY = 'YTD' THEN"
                        +
                        " ROUND([Bookings].Amount*[Currency_Exchange].Rate1,0) ELSE ROUND([Bookings].Amount*[Currency_Exchange].Rate2,0) END) ELSE 0 END)"
                        + " AS  '" + ds_pro.Tables[0].Rows[i][1].ToString().Trim() + "'";
            }
            temp += " FROM [Bookings] INNER JOIN [SalesOrg] ON [Bookings].SalesOrgID = [SalesOrg].ID AND [SalesOrg].Deleted = 0"
                    +
                    " INNER JOIN [Currency_Exchange] ON [Currency_Exchange].CurrencyID = [SalesOrg].CurrencyID AND [Currency_Exchange].Deleted = 0"
                    + " WHERE [Bookings].SegmentID = " + str_segmentID;
            temp += " AND YEAR([Bookings].TimeFlag) = '" + selectMeeting.getyear() +
                    "' AND MONTH([Bookings].TimeFlag) = '" + selectMeeting.getmonth() + "'"
                    + " AND YEAR([Currency_Exchange].TimeFlag) = '" + str_year + "'"
                    + " AND MONTH([Currency_Exchange].TimeFlag) = '" + str_month + "'"
                    + " AND [Bookings].BookingY = '" + selectMeeting.getnextyear().Substring(2, 2) + "'"
                    + " GROUP BY [Bookings].BookingY"
                    + " ORDER BY [Bookings].BookingY ASC";
            sql += temp;
            //by ryzhang item49 20110520 add end
            DataSet ds = helper.GetDataSet(sql);

            return ds;
        }
        return null;
    }

    /// <summary>
    /// Get bookings budget by segment
    /// </summary>
    /// <param name="ds_pro">products</param>
    /// <param name="str_segmentID">segment id</param>
    /// <returns>dataset</returns>
    public DataSet getBookingsBudgetTotal(DataSet ds_pro, string str_segmentID)
    {
        string str_year = meeting.getyear();
        string str_nextyear = meeting.getnextyear();
        //yyan itemw140 20110909 edit start
        string str_month = "03"; //meeting.getmonth();
        //yyan itemw140 20110909 edit end
        if (ds_pro.Tables[0].Rows.Count > 0)
        {
            string sql = "SELECT BookingY";
            string temp = "";
            for (int i = 0; i < ds_pro.Tables[0].Rows.Count; i++)
            {
                temp += ",SUM(CASE WHEN [Bookings].ProductID = " + ds_pro.Tables[0].Rows[i][0].ToString().Trim()
                        + " THEN (CASE WHEN [Bookings].DeliverY = '" + str_year.Substring(2, 2) +
                        "' OR [Bookings].DeliverY = 'YTD' THEN"
                        +
                        " ROUND([Bookings].Amount*[Currency_Exchange].Rate1,0) ELSE ROUND([Bookings].Amount*[Currency_Exchange].Rate2,0) END) ELSE 0 END)"
                        + " AS  '" + ds_pro.Tables[0].Rows[i][1].ToString().Trim() + "'";
            }
            temp += " FROM [Bookings],[SalesOrg],[Currency_Exchange]"
                    + " WHERE [Bookings].BookingY = '" + str_nextyear.Substring(2, 2) + "'"
                    + " AND [Bookings].SegmentID = " + str_segmentID
                    + " AND [Bookings].SalesOrgID = [SalesOrg].ID"
                    + " AND [Currency_Exchange].CurrencyID = [SalesOrg].CurrencyID"
                    + " AND [Bookings].SegmentID = " + str_segmentID
                    + " AND (YEAR([Bookings].TimeFlag) = '" + str_year + "' AND MONTH([Bookings].TimeFlag) = '03')"
                    + " AND YEAR([Currency_Exchange].TimeFlag) = '" + str_year + "'"
                    + " AND MONTH([Currency_Exchange].TimeFlag) = '" + str_month + "'"
                    + " AND SalesOrg.Deleted=0 "
                    + " AND Currency_Exchange.Deleted=0 "
                    + " GROUP BY [Bookings].BookingY"
                    + " ORDER BY [Bookings].BookingY ASC";
            sql += temp;
            DataSet ds = helper.GetDataSet(sql);

            return ds;
        }
        return null;
    }

    /// <summary>
    ///  Get one backlog in table actualsalesBL By sales org by operation
    /// </summary>
    /// <param name="str_operationID">operation id</param>
    /// <param name="str_segmentID">segment id</param>
    /// <param name="str_productID">product id</param>
    /// <param name="str_year">year</param>
    /// <param name="str_month">month</param>
    /// <param name="str_salesOrgID">salesOrg id</param>
    /// <param name="flagMoney">1:need to convert to EUR, 0:not need to convert.</param>
    /// <param name="str_blY">backlog year</param>
    /// <returns>string</returns>
    public string getBackLogBySalesOrgByOperation(string str_operationID, string str_segmentID, string str_productID,
                                                  string str_year, string str_month, string str_salesOrgID,
                                                  string str_blY, int flagMoney)
        //by yyan item8 20110614 add end 
    {
        string query_bl = "";
        if (flagMoney == 0)
        {
            double operationRate1 = getRate(getOperationCurrency(str_operationID), true, str_year, str_month);
            double operationRate2 = getRate(getOperationCurrency(str_operationID), false, str_year, str_month);
            if (operationRate1 < 0.000000001 && operationRate1 > -0.00000001)
            {
                operationRate1 = 100000000;
            }
            if (operationRate2 < 0.000000001 && operationRate2 > -0.00000001)
            {
                operationRate2 = 100000000;
            }
            query_bl = "SELECT ROUND(SUM(CASE WHEN [ActualSalesandBL].BacklogY = '" + str_year.Substring(2, 2) + "' "
                       + " THEN [ActualSalesandBL].Backlog*[Currency_Exchange].Rate1/" +
                       operationRate1.ToString(CultureInfo.InvariantCulture) +
                       " ELSE [ActualSalesandBL].Backlog*[Currency_Exchange].Rate2/" +
                       operationRate2.ToString(CultureInfo.InvariantCulture) + " END ),0) AS 'Backlog'"
                       + " FROM [ActualSalesandBL],[User_Operation],[Operation],[Currency_Exchange] "
                       + " WHERE [ActualSalesandBL].SegmentID = " + str_segmentID
                       + " AND [User_Operation].UserID = [ActualSalesandBL].MarketingMgrID "
                       + " AND [User_Operation].OperationID = [Operation].ID "
                       + " AND [Operation].CurrencyID = [Currency_Exchange].CurrencyID "
                       + " AND [ActualSalesandBL].ProductID = " + str_productID
                       + " AND YEAR([ActualSalesandBL].TimeFlag) = '" + str_year + "'"
                       + " AND MONTH([ActualSalesandBL].TimeFlag) = '" + str_month + "'"
                       + " AND YEAR([Currency_Exchange].TimeFlag) = '" + str_year + "'"
                       + " AND MONTH([Currency_Exchange].TimeFlag) = '" + str_month + "'"
                       + " AND [ActualSalesandBL].BacklogY = '" + str_blY + "' "
                       + " AND [ActualSalesandBL].OperationID = '" + str_operationID + "' "
                       + " AND [Operation].ID = '" + str_operationID + "'"
                       + " AND [User_Operation].Deleted=0"
                       + " AND Operation.Deleted=0 "
                       //by yyan itemw151 20110922 add start
                       + " AND [ActualSalesandBL].SalesOrgID = " + str_salesOrgID
                       //by yyan itemw151 20110922 add end 
                       + " AND Currency_Exchange.Deleted=0 ";
        }
        else
        {
            query_bl = "SELECT ROUND(SUM(CASE WHEN [ActualSalesandBL].BacklogY = '" + str_year.Substring(2, 2) + "' "
                       +
                       " THEN [ActualSalesandBL].Backlog*[Currency_Exchange].Rate1 ELSE [ActualSalesandBL].Backlog*[Currency_Exchange].Rate2 END ),0) AS 'Backlog'"
                       + " FROM [ActualSalesandBL],[User_Operation],[Operation],[Currency_Exchange] "
                       + " WHERE [ActualSalesandBL].SegmentID = " + str_segmentID
                       + " AND [User_Operation].UserID = [ActualSalesandBL].MarketingMgrID "
                       + " AND [User_Operation].OperationID = [Operation].ID "
                       + " AND [Operation].CurrencyID = [Currency_Exchange].CurrencyID "
                       + " AND [ActualSalesandBL].ProductID = " + str_productID
                       + " AND YEAR([ActualSalesandBL].TimeFlag) = '" + str_year + "'"
                       + " AND MONTH([ActualSalesandBL].TimeFlag) = '" + str_month + "'"
                       + " AND YEAR([Currency_Exchange].TimeFlag) = '" + str_year + "'"
                       + " AND MONTH([Currency_Exchange].TimeFlag) = '" + str_month + "'"
                       + " AND [ActualSalesandBL].BacklogY = '" + str_blY + "' "
                       + " AND [ActualSalesandBL].OperationID = '" + str_operationID + "' "
                       + " AND [Operation].ID = '" + str_operationID + "'"
                       + " AND [User_Operation].Deleted=0"
                       + " AND Operation.Deleted=0 "
                       //by yyan itemw151 20110922 add start
                       + " AND [ActualSalesandBL].SalesOrgID = " + str_salesOrgID
                       //by yyan itemw151 20110922 add end 
                       + " AND Currency_Exchange.Deleted=0 ";
        }
        //by yyan item8 20110614 add end 
        DataSet ds = helper.GetDataSet(query_bl);

        if (ds.Tables[0].Rows.Count == 1)
            return ds.Tables[0].Rows[0][0].ToString().Trim();
        else
            return "";
    }

    /// <summary>
    ///  Get backlog total in table actualsalesBL by operation
    /// </summary>
    /// <param name="str_operationID">operation id</param>
    /// <param name="str_segmentID">segment id</param>
    /// <param name="str_productID">product id</param>
    /// <param name="str_year">year</param>
    /// <param name="str_month">month</param>
    /// <param name="str_blY">backlog year</param>
    /// <param name="flagMoney">1:need to convert to EUR, 0:not need to convert.</param>
    /// <returns>dataset</returns>
    public string getBackLogByOperation(string str_operationID, string str_segmentID, string str_productID,
                                        string str_year, string str_month, string str_blY, int flagMoney)
        //by yyan item8 20110617 add end
    {
        string query_bl = "";
        if (flagMoney == 0)
        {
            double operationRate1 = getRate(getOperationCurrency(str_operationID), true, str_year, str_month);
            double operationRate2 = getRate(getOperationCurrency(str_operationID), false, str_year, str_month);
            if (operationRate1 < 0.000000001 && operationRate1 > -0.00000001)
            {
                operationRate1 = 100000000;
            }
            if (operationRate2 < 0.000000001 && operationRate2 > -0.00000001)
            {
                operationRate2 = 100000000;
            }
            query_bl = "SELECT ROUND(SUM(CASE WHEN [ActualSalesandBL].BacklogY = '" + str_year.Substring(2, 2) + "' "
                       + " THEN [ActualSalesandBL].Backlog*[Currency_Exchange].Rate1/" +
                       operationRate1.ToString(CultureInfo.InvariantCulture) +
                       " ELSE [ActualSalesandBL].Backlog*[Currency_Exchange].Rate2/" +
                       operationRate2.ToString(CultureInfo.InvariantCulture) + " END ),0) AS 'Backlog'"
                       + " FROM [ActualSalesandBL],[User_Operation],[Operation],[Currency_Exchange] "
                       + " WHERE [ActualSalesandBL].SegmentID = " + str_segmentID
                       + " AND [User_Operation].UserID = [ActualSalesandBL].MarketingMgrID "
                       + " AND [User_Operation].OperationID = [Operation].ID "
                       + " AND [Operation].CurrencyID = [Currency_Exchange].CurrencyID "
                       + " AND [ActualSalesandBL].ProductID = " + str_productID
                       + " AND YEAR([ActualSalesandBL].TimeFlag) = '" + str_year + "'"
                       + " AND MONTH([ActualSalesandBL].TimeFlag) = '" + str_month + "'"
                       + " AND YEAR([Currency_Exchange].TimeFlag) = '" + str_year + "'"
                       + " AND MONTH([Currency_Exchange].TimeFlag) = '" + str_month + "'"
                       + " AND [ActualSalesandBL].BacklogY = '" + str_blY + "'"
                       + " AND [ActualSalesandBL].OperationID = '" + str_operationID + "'"
                       + " AND [Operation].ID = '" + str_operationID + "'"
                       + " AND [User_Operation].Deleted=0"
                       + " AND Operation.Deleted=0 "
                       + " AND Currency_Exchange.Deleted=0 ";
        }
        else
        {
            query_bl = "SELECT ROUND(SUM(CASE WHEN [ActualSalesandBL].BacklogY = '" + str_year.Substring(2, 2) + "' "
                       +
                       " THEN [ActualSalesandBL].Backlog*[Currency_Exchange].Rate1 ELSE [ActualSalesandBL].Backlog*[Currency_Exchange].Rate2 END ),0) AS 'Backlog'"
                       + " FROM [ActualSalesandBL],[User_Operation],[Operation],[Currency_Exchange] "
                       + " WHERE [ActualSalesandBL].SegmentID = " + str_segmentID
                       + " AND [User_Operation].UserID = [ActualSalesandBL].MarketingMgrID "
                       + " AND [User_Operation].OperationID = [Operation].ID "
                       + " AND [Operation].CurrencyID = [Currency_Exchange].CurrencyID "
                       + " AND [ActualSalesandBL].ProductID = " + str_productID
                       + " AND YEAR([ActualSalesandBL].TimeFlag) = '" + str_year + "'"
                       + " AND MONTH([ActualSalesandBL].TimeFlag) = '" + str_month + "'"
                       + " AND YEAR([Currency_Exchange].TimeFlag) = '" + str_year + "'"
                       + " AND MONTH([Currency_Exchange].TimeFlag) = '" + str_month + "'"
                       + " AND [ActualSalesandBL].BacklogY = '" + str_blY + "'"
                       + " AND [ActualSalesandBL].OperationID = '" + str_operationID + "'"
                       + " AND [Operation].ID = '" + str_operationID + "'"
                       + " AND [User_Operation].Deleted=0"
                       + " AND Operation.Deleted=0 "
                       + " AND Currency_Exchange.Deleted=0 ";
        }
        //by yyan item8 20110614 add end 
        DataSet ds = helper.GetDataSet(query_bl);

        if (ds.Tables[0].Rows.Count == 1)
            return ds.Tables[0].Rows[0][0].ToString().Trim();
        else
            return "";
    }


    public Dictionary<int, float> getBackLogByOperationProductids(string str_operationID, string str_segmentID,
                                                                  int[] products, string str_year, string str_month,
                                                                  string str_blY, int flagMoney)
    {
        if (products == null || products.Count() < 1)
            return null;
        string query_bl = "";
        if (flagMoney == 0)
        {
            double operationRate1 = getRate(getOperationCurrency(str_operationID), true, str_year, str_month);
            double operationRate2 = getRate(getOperationCurrency(str_operationID), false, str_year, str_month);
            if (operationRate1 < 0.000000001 && operationRate1 > -0.00000001)
            {
                operationRate1 = 100000000;
            }
            if (operationRate2 < 0.000000001 && operationRate2 > -0.00000001)
            {
                operationRate2 = 100000000;
            }
            query_bl =
                "SELECT [ActualSalesandBL].ProductID,ISNULL(ROUND(SUM(CASE WHEN [ActualSalesandBL].BacklogY = '" +
                str_year.Substring(2, 2) + "' "
                + " THEN [ActualSalesandBL].Backlog*[Currency_Exchange].Rate1/" +
                operationRate1.ToString(CultureInfo.InvariantCulture) +
                " ELSE [ActualSalesandBL].Backlog*[Currency_Exchange].Rate2/" +
                operationRate2.ToString(CultureInfo.InvariantCulture) + " END ),0),0) AS 'Backlog'"
                + " FROM [ActualSalesandBL],[User_Operation],[Operation],[Currency_Exchange] "
                + " WHERE [ActualSalesandBL].SegmentID = " + str_segmentID
                + " AND [User_Operation].UserID = [ActualSalesandBL].MarketingMgrID "
                + " AND [User_Operation].OperationID = [Operation].ID "
                + " AND [Operation].CurrencyID = [Currency_Exchange].CurrencyID "
                //+ " AND [ActualSalesandBL].ProductID = " + str_productID
                + " AND YEAR([ActualSalesandBL].TimeFlag) = '" + str_year + "'"
                + " AND MONTH([ActualSalesandBL].TimeFlag) = '" + str_month + "'"
                + " AND YEAR([Currency_Exchange].TimeFlag) = '" + str_year + "'"
                + " AND MONTH([Currency_Exchange].TimeFlag) = '" + str_month + "'"
                + " AND [ActualSalesandBL].BacklogY = '" + str_blY + "'"
                + " AND [ActualSalesandBL].OperationID = '" + str_operationID + "'"
                + " AND [Operation].ID = '" + str_operationID + "'"
                + " AND [User_Operation].Deleted=0"
                + " AND Operation.Deleted=0 "
                + " AND Currency_Exchange.Deleted=0 "
                + " AND [ActualSalesandBL].ProductID in(" +
                string.Join(",", products.Select(o => o.ToString(CultureInfo.InvariantCulture)).ToArray()) + ")"
                + " Group by [ActualSalesandBL].ProductID";
        }
        else
        {
            query_bl =
                "SELECT [ActualSalesandBL].ProductID,ISNULL(ROUND(SUM(CASE WHEN [ActualSalesandBL].BacklogY = '" +
                str_year.Substring(2, 2) + "' "
                +
                " THEN [ActualSalesandBL].Backlog*[Currency_Exchange].Rate1 ELSE [ActualSalesandBL].Backlog*[Currency_Exchange].Rate2 END ),0),0) AS 'Backlog'"
                + " FROM [ActualSalesandBL],[User_Operation],[Operation],[Currency_Exchange] "
                + " WHERE [ActualSalesandBL].SegmentID = " + str_segmentID
                + " AND [User_Operation].UserID = [ActualSalesandBL].MarketingMgrID "
                + " AND [User_Operation].OperationID = [Operation].ID "
                + " AND [Operation].CurrencyID = [Currency_Exchange].CurrencyID "
                //+ " AND [ActualSalesandBL].ProductID = " + str_productID
                + " AND YEAR([ActualSalesandBL].TimeFlag) = '" + str_year + "'"
                + " AND MONTH([ActualSalesandBL].TimeFlag) = '" + str_month + "'"
                + " AND YEAR([Currency_Exchange].TimeFlag) = '" + str_year + "'"
                + " AND MONTH([Currency_Exchange].TimeFlag) = '" + str_month + "'"
                + " AND [ActualSalesandBL].BacklogY = '" + str_blY + "'"
                + " AND [ActualSalesandBL].OperationID = '" + str_operationID + "'"
                + " AND [Operation].ID = '" + str_operationID + "'"
                + " AND [User_Operation].Deleted=0"
                + " AND Operation.Deleted=0 "
                + " AND Currency_Exchange.Deleted=0 "
                + " AND [ActualSalesandBL].ProductID in(" +
                string.Join(",", products.Select(o => o.ToString(CultureInfo.InvariantCulture)).ToArray()) + ")"
                + " Group by [ActualSalesandBL].ProductID";
        }
        //by yyan item8 20110614 add end 
        DataSet ds = helper.GetDataSet(query_bl);

        //if (ds.Tables[0].Rows.Count == 1)
        //    return ds.Tables[0].Rows[0][0].ToString().Trim();
        //else
        //    return "";

        var result = new Dictionary<int, float>();
        //foreach (var item in products)
        //{
        //    result.Add(item.Key, 0);
        //}
        for (int p = 0; p < ds.Tables[0].Rows.Count; p++)
        {
            result.Add(Convert.ToInt32(ds.Tables[0].Rows[p][0].ToString()),
                       float.Parse(ds.Tables[0].Rows[p][1].ToString()));
        }
        return result;
    }

    /// <summary>
    ///  Get backlog total in table actualsalesBL
    /// </summary>
    /// <param name="str_segmentID">segment id</param>
    /// <param name="str_productID">product id</param>
    /// <param name="str_year">year</param>
    /// <param name="str_month">month</param>
    /// <param name="str_blY">backlog year</param>
    /// <returns>backlog</returns>
    public string getBackLog(string str_segmentID, string str_productID, string str_year, string str_month,
                             string str_blY)
    {
        string query_bl = "SELECT SUM(CASE WHEN [ActualSalesandBL].BacklogY = '" + str_year.Substring(2, 2) + "' "
                          +
                          " THEN ROUND([ActualSalesandBL].Backlog*[Currency_Exchange].Rate1,0) ELSE ROUND([ActualSalesandBL].Backlog*[Currency_Exchange].Rate2,0) END ) AS 'Backlog'"
                          + " FROM [ActualSalesandBL],[User_Operation],[Operation],[Currency_Exchange] "
                          + " WHERE [ActualSalesandBL].SegmentID = " + str_segmentID
                          + " AND [User_Operation].UserID = [ActualSalesandBL].MarketingMgrID "
                          + " AND [User_Operation].OperationID = [Operation].ID "
                          + " AND [Operation].CurrencyID = [Currency_Exchange].CurrencyID "
                          + " AND [ActualSalesandBL].ProductID = " + str_productID
                          + " AND YEAR([ActualSalesandBL].TimeFlag) = '" + str_year + "'"
                          + " AND MONTH([ActualSalesandBL].TimeFlag) = '" + str_month + "'"
                          + " AND YEAR([Currency_Exchange].TimeFlag) = '" + str_year + "'"
                          + " AND MONTH([Currency_Exchange].TimeFlag) = '" + str_month + "'"
                          + " AND [ActualSalesandBL].BacklogY = '" + str_blY + "' "
                          + " AND [User_Operation].Deleted=0"
                          + " AND Operation.Deleted=0 "
                          + " AND Currency_Exchange.Deleted=0 "
                          + " and [User_Operation].OperationID=[ActualSalesandBL].OperationID";
        DataSet ds = helper.GetDataSet(query_bl);

        if (ds.Tables[0].Rows.Count > 0)
            return ds.Tables[0].Rows[0][0].ToString().Trim();
        else
            return "";
    }

    /// <summary>
    /// get sales data by salesOrg by operation
    /// </summary>
    /// <param name="ds_pro">products</param>
    /// <param name="str_salesOrgID">salesOrg id</param>
    /// <param name="str_operationID">operation id</param>
    /// <param name="str_segmentID">segment id</param>
    /// <param name="flagMoney">1:need to convert to EUR, 0:not need to convert.</param>
    /// <returns>dataset</returns>
    public DataSet getSalesDataBySalesOrgByOperation(DataSet ds_pro, string str_salesOrgID, string str_operationID,
                                                     string str_segmentID, int flagMoney)
        //by yyan item8 20110614 add end
    {
        //by ryzhang 20110530 item49 modify start
        string str_year = selectMeeting.getyear();
        string str_nextyear = selectMeeting.getnextyear();
        string str_month = selectMeeting.getmonth();
        //by ryzhang 20110530 item49 modify end
        double rate1 = getPercentage(str_operationID, str_salesOrgID, true, str_year, str_month);
        double rate2 = getPercentage(str_operationID, str_salesOrgID, false, str_year, str_month);
        if (ds_pro.Tables[0].Rows.Count > 0)
        {
            string sql = "SELECT (BookingY + '--' + DeliverY) AS ' '";
            string temp = "";
            //by yyan item8 20110614 add start
            if (flagMoney == 1)
            {
                for (int i = 0; i < ds_pro.Tables[0].Rows.Count; i++)
                {
                    temp += ",SUM(CASE WHEN ProductID = " + ds_pro.Tables[0].Rows[i][0].ToString().Trim()
                            + " THEN (CASE WHEN DeliverY = '" + str_year.Substring(2, 2) + "'"
                            + " THEN Amount ELSE Amount END) ELSE 0 END) AS  '" +
                            ds_pro.Tables[0].Rows[i][1].ToString().Trim() + "'";
                }
                string fromsql = "SELECT [SalesOrg].ID,[Bookings].BookingY,"
                                 + " [Bookings].DeliverY,[Bookings].ProductID,[Bookings].OperationID"
                                 + " ,SUM(CASE WHEN [Bookings].DeliverY = '" + str_year.Substring(2, 2) + "'"
                                 +
                                 " THEN ROUND([Bookings].Amount*[Currency_Exchange].Rate1,0) ELSE ROUND([Bookings].Amount*[Currency_Exchange].Rate2,0) END ) AS Amount"
                                 + " ,[Bookings].SegmentID,[Bookings].TimeFlag"
                                 + " FROM [Bookings],[SalesOrg],[Currency_Exchange]"
                                 + " WHERE [SalesOrg].ID = [Bookings].SalesOrgID"
                                 + " AND [Bookings].SalesOrgID = " + str_salesOrgID
                                 + " AND [SalesOrg].CurrencyID = [Currency_Exchange].CurrencyID"
                                 + " AND YEAR([Currency_Exchange].TimeFlag) = '" + str_year + "'"
                                 + " AND MONTH([Currency_Exchange].TimeFlag) = '" + str_month + "'"
                                 + " AND SalesOrg.Deleted=0 "
                                 + " AND Currency_Exchange.Deleted=0 "
                                 + " GROUP BY [SalesOrg].ID, [Bookings].BookingY,[Bookings].DeliverY,"
                                 + " [Bookings].ProductID,[Bookings].OperationID"
                                 + " ,[Bookings].SegmentID,[Bookings].TimeFlag";
                temp += " FROM (" + fromsql + ") AS NewBookings"
                        + " WHERE SegmentID = " + str_segmentID
                        + " AND OperationID = " + str_operationID
                        + " AND YEAR(TimeFlag) = '" + selectMeeting.getyear() + "'"
                        + " AND MONTH(TimeFlag) = '" + selectMeeting.getmonth() + "'"
                        + " AND (DeliverY = '" + str_year.Substring(2, 2) + "'"
                        + " OR DeliverY = '" + str_nextyear.Substring(2, 2) + "')"
                        + " GROUP BY BookingY,DeliverY"
                        + " ORDER BY BookingY,DeliverY ASC";
            }
            else
            {
                //by yyan item8 20110614 add end
                for (int i = 0; i < ds_pro.Tables[0].Rows.Count; i++)
                {
                    temp += ",SUM(CASE WHEN ProductID = " + ds_pro.Tables[0].Rows[i][0].ToString().Trim()
                            + " THEN (CASE WHEN DeliverY = '" + str_year.Substring(2, 2) + "'"
                            + " THEN ROUND(Amount*" + rate1.ToString().Replace(',', '.') + ",0) ELSE ROUND(Amount*" +
                            rate2.ToString().Replace(',', '.') + ",0) END) ELSE 0 END) AS  '" +
                            ds_pro.Tables[0].Rows[i][1].ToString().Trim() + "'";
                }
                temp += " FROM [Bookings]"
                        + " WHERE SegmentID = " + str_segmentID
                        + " AND OperationID = " + str_operationID
                        + " AND YEAR(TimeFlag) = '" + selectMeeting.getyear() + "'"
                        + " AND MONTH(TimeFlag) = '" + selectMeeting.getmonth() + "'"
                        + " AND (DeliverY = '" + str_year.Substring(2, 2) + "'"
                        + " OR DeliverY = '" + str_nextyear.Substring(2, 2) + "')"
                        + " AND SalesOrgID = " + str_salesOrgID
                        + " GROUP BY BookingY,DeliverY"
                        + " ORDER BY BookingY,DeliverY ASC";
                //by yyan item8 20110614 add start
            }
            //by yyan item8 20110614 add end
            sql += temp;
            //ryzhang item49 20110510 add end
            DataSet ds = helper.GetDataSet(sql);

            return ds;
        }
        return null;
    }

    /// <summary>
    /// get sales data by operation
    /// </summary>
    /// <param name="ds_pro">products</param>
    /// <param name="str_operationID">operation id</param>
    /// <param name="str_segmentID">segment id</param>
    /// <param name="flagMoney">1:need to convert to EUR, 0:not need to convert.</param>
    /// <returns>dataset</returns>
    public DataSet getSalesDataByOperation(DataSet ds_pro, string str_operationID, string str_segmentID, int flagMoney)
        //by yyan item8 20110617 add end
    {
        //by ryzhang 20110530 item49 modify start
        string str_year = selectMeeting.getyear();
        string str_yearafternext = selectMeeting.getyearAfterNext();
        string str_month = selectMeeting.getmonth();
        //by ryzhang 20110530 item49 modify end
        double operationRate1 = getRate(getOperationCurrency(str_operationID), true, str_year, str_month);
        double operationRate2 = getRate(getOperationCurrency(str_operationID), false, str_year, str_month);
        if (operationRate1 < 0.000000001 && operationRate1 > -0.00000001)
        {
            operationRate1 = 100000000;
        }
        if (operationRate2 < 0.000000001 && operationRate2 > -0.00000001)
        {
            operationRate2 = 100000000;
        }
        if (ds_pro.Tables[0].Rows.Count > 0)
        {
            //by yyan item8 20110614 add start
            string sql = "SELECT (BookingY + '--' + DeliverY) AS ' '";
            string temp = "";
            if (flagMoney == 0)
            {
                for (int i = 0; i < ds_pro.Tables[0].Rows.Count; i++)
                {
                    temp += ",ROUND(SUM(CASE WHEN ProductID = " + ds_pro.Tables[0].Rows[i][0].ToString().Trim()
                            + " THEN (CASE WHEN DeliverY = '" + str_year.Substring(2, 2) + "'"
                            + " THEN Amount/" + operationRate1.ToString().Replace(',', '.') + " ELSE Amount/" +
                            operationRate2.ToString().Replace(',', '.') + " END) ELSE 0 END),0) AS  '" +
                            ds_pro.Tables[0].Rows[i][1].ToString().Trim() + "'";
                }
                string fromsql = "SELECT [SalesOrg].ID,[Bookings].BookingY,"
                                 + " [Bookings].DeliverY,[Bookings].ProductID,[Bookings].OperationID"
                                 + " ,SUM(CASE WHEN [Bookings].DeliverY = '" + str_year.Substring(2, 2) + "'"
                                 +
                                 " THEN [Bookings].Amount*[Currency_Exchange].Rate1 ELSE [Bookings].Amount*[Currency_Exchange].Rate2 END ) AS Amount"
                                 + " ,[Bookings].SegmentID,[Bookings].TimeFlag"
                                 + " FROM [Bookings],[SalesOrg],[Currency_Exchange]"
                                 + " WHERE [SalesOrg].ID = [Bookings].SalesOrgID"
                                 + " AND [SalesOrg].CurrencyID = [Currency_Exchange].CurrencyID"
                                 + " AND YEAR([Currency_Exchange].TimeFlag) = '" + str_year + "'"
                                 + " AND MONTH([Currency_Exchange].TimeFlag) = '" + str_month + "'"
                                 + " AND SalesOrg.Deleted=0 "
                                 + " AND Currency_Exchange.Deleted=0 "
                                 + " GROUP BY [SalesOrg].ID, [Bookings].BookingY,[Bookings].DeliverY,"
                                 + " [Bookings].ProductID,[Bookings].OperationID"
                                 + " ,[Bookings].SegmentID,[Bookings].TimeFlag";
                temp += " FROM (" + fromsql + ") AS NewBookings"
                        + " WHERE SegmentID = " + str_segmentID
                        + " AND OperationID = " + str_operationID
                        + " AND YEAR(TimeFlag) = '" + selectMeeting.getyear() + "'"
                        + " AND MONTH(TimeFlag) = '" + selectMeeting.getmonth() + "'"
                        + " AND DeliverY <> 'YTD'"
                        + " AND DeliverY <> '" + selectMeeting.getyearAfterNext().Substring(2, 2) + "'"
                        + " GROUP BY BookingY,DeliverY"
                        + " ORDER BY BookingY,DeliverY ASC";
            }
            else
            {
                for (int i = 0; i < ds_pro.Tables[0].Rows.Count; i++)
                {
                    temp += ",ROUND(SUM(CASE WHEN ProductID = " + ds_pro.Tables[0].Rows[i][0].ToString().Trim()
                            + " THEN (CASE WHEN DeliverY = '" + str_year.Substring(2, 2) + "'"
                            + " THEN Amount ELSE Amount END) ELSE 0 END),0) AS  '" +
                            ds_pro.Tables[0].Rows[i][1].ToString().Trim() + "'";
                }
                string fromsql = "SELECT [SalesOrg].ID,[Bookings].BookingY,"
                                 + " [Bookings].DeliverY,[Bookings].ProductID,[Bookings].OperationID"
                                 + " ,SUM(CASE WHEN [Bookings].DeliverY = '" + str_year.Substring(2, 2) + "'"
                                 +
                                 " THEN [Bookings].Amount*[Currency_Exchange].Rate1 ELSE [Bookings].Amount*[Currency_Exchange].Rate2 END ) AS Amount"
                                 + " ,[Bookings].SegmentID,[Bookings].TimeFlag"
                                 + " FROM [Bookings],[SalesOrg],[Currency_Exchange]"
                                 + " WHERE [SalesOrg].ID = [Bookings].SalesOrgID"
                                 + " AND [SalesOrg].CurrencyID = [Currency_Exchange].CurrencyID"
                                 + " AND YEAR([Currency_Exchange].TimeFlag) = '" + str_year + "'"
                                 + " AND MONTH([Currency_Exchange].TimeFlag) = '" + str_month + "'"
                                 + " AND SalesOrg.Deleted=0 "
                                 + " AND Currency_Exchange.Deleted=0 "
                                 + " GROUP BY [SalesOrg].ID, [Bookings].BookingY,[Bookings].DeliverY,"
                                 + " [Bookings].ProductID,[Bookings].OperationID"
                                 + " ,[Bookings].SegmentID,[Bookings].TimeFlag";
                temp += " FROM (" + fromsql + ") AS NewBookings"
                        + " WHERE SegmentID = " + str_segmentID
                        + " AND OperationID = " + str_operationID
                        + " AND YEAR(TimeFlag) = '" + selectMeeting.getyear() + "'"
                        + " AND MONTH(TimeFlag) = '" + selectMeeting.getmonth() + "'"
                        + " AND DeliverY <> 'YTD'"
                        + " AND DeliverY <> '" + selectMeeting.getyearAfterNext().Substring(2, 2) + "'"
                        + " GROUP BY BookingY,DeliverY"
                        + " ORDER BY BookingY,DeliverY ASC";
            }
            sql += temp;
            //by yyan item8 20110614 add end
            DataSet ds = helper.GetDataSet(sql);
            return ds;
        }
        return null;
    }

    /// <summary>
    /// get sales data total by operation
    /// </summary>
    /// <param name="ds_pro">products</param>
    /// <param name="str_segmentID">segment id</param>
    /// <returns>dataset</returns>
    public DataSet getSalesDataTotalByOperation(DataSet ds_pro, string str_segmentID)
    {
        string str_year = meeting.getyear();
        string str_nextyear = meeting.getnextyear();
        string str_month = meeting.getmonth();
        if (ds_pro.Tables[0].Rows.Count > 0)
        {
            string sql = "SELECT (BookingY + '--' + DeliverY) AS ' '";
            string temp = "";
            for (int i = 0; i < ds_pro.Tables[0].Rows.Count; i++)
            {
                temp += ",SUM(CASE WHEN [Bookings].ProductID = " + ds_pro.Tables[0].Rows[i][0].ToString().Trim()
                        + " THEN (CASE WHEN [Bookings].DeliverY = '" + str_year.Substring(2, 2) + "' THEN"
                        +
                        " ROUND([Bookings].Amount*[Currency_Exchange].Rate1,0) ELSE ROUND([Bookings].Amount*[Currency_Exchange].Rate2,0) END) ELSE 0 END)"
                        + " AS  '" + ds_pro.Tables[0].Rows[i][1].ToString().Trim() + "'";
            }
            temp += " FROM [Bookings],[SalesOrg],[Currency_Exchange]"
                    + " WHERE [Bookings].SegmentID = " + str_segmentID
                    + " AND [SalesOrg].ID = [Bookings].SalesOrgID"
                    + " AND [Currency_Exchange].CurrencyID = [SalesOrg].CurrencyID";

            temp += " AND YEAR([Bookings].TimeFlag) = '" + str_year + "'"
                    + " AND MONTH([Bookings].TimeFlag) = '" + str_month + "'"
                    + " AND (DeliverY = '" + str_year.Substring(2, 2) + "'"
                    + " OR DeliverY = '" + str_nextyear.Substring(2, 2) + "')"
                    + " AND YEAR([Currency_Exchange].TimeFlag) = '" + str_year + "'"
                    + " AND MONTH([Currency_Exchange].TimeFlag) = '" + str_month + "'"
                    + " AND SalesOrg.Deleted=0 "
                    + " AND Currency_Exchange.Deleted=0 "
                    + " GROUP BY [Bookings].BookingY,[Bookings].DeliverY"
                    + " ORDER BY [Bookings].BookingY,[Bookings].DeliverY ASC";
            sql += temp;
            DataSet ds = helper.GetDataSet(sql);

            return ds;
        }
        return null;
    }

    /// <summary>
    /// get sales budget by salesOrg by operation.
    /// </summary>
    /// <param name="ds_pro">products</param>
    /// <param name="str_salesOrgID">salesOrg id</param>
    /// <param name="str_operationID">operation id</param>
    /// <param name="str_segmentID">segment id</param>
    /// <param name="str_year">year</param>
    /// <param name="str_deliverY">deliver year</param>
    /// <param name="flagMoney">1:need to convert to EUR, 0:not need to convert.</param>
    /// <returns>dataset</returns>
    public DataSet getSalesBudgetBySalesOrgByOperation(DataSet ds_pro, string str_salesOrgID, string str_operationID,
                                                       string str_segmentID, string str_year, string str_deliverY,
                                                       int flagMoney)
        //by yyan item8 20110614 add end
    {
        double rate1 = getPercentage(str_operationID, str_salesOrgID, true, str_year, "03");
        double rate2 = getPercentage(str_operationID, str_salesOrgID, false, str_year, "03");
        if (ds_pro.Tables[0].Rows.Count > 0)
        {
            //ryzhang item49 20110510 add start
            string sql = "SELECT DeliverY";
            string temp = "";
            //by yyan item8 20110614 add start
            if (flagMoney == 1)
            {
                //by yyan item8 20110614 add end
                double operationRate1 = getRate(getOperationCurrency(str_operationID), true, str_year, "03");
                double operationRate2 = getRate(getOperationCurrency(str_operationID), false, str_year, "03");
                if (operationRate1 < 0.000000001 && operationRate1 > -0.00000001)
                {
                    operationRate1 = 100000000;
                }
                if (operationRate2 < 0.000000001 && operationRate2 > -0.00000001)
                {
                    operationRate2 = 100000000;
                }
                for (int i = 0; i < ds_pro.Tables[0].Rows.Count; i++)
                {
                    temp += ", ROUND(SUM(CASE WHEN ProductID = " + ds_pro.Tables[0].Rows[i][0].ToString().Trim()
                            + " THEN (CASE WHEN DeliverY = '" + str_year.Substring(2, 2) + "' OR DeliverY = 'YTD'"
                            + " THEN Amount*" + rate1.ToString().Replace(',', '.') + "*" +
                            operationRate1.ToString(CultureInfo.InvariantCulture) + " ELSE Amount*" +
                            rate2.ToString().Replace(',', '.') + "*" +
                            operationRate2.ToString(CultureInfo.InvariantCulture) + " END) ELSE 0 END),0) AS  '" +
                            ds_pro.Tables[0].Rows[i][1].ToString().Trim() + "'";
                }
                temp += " FROM [Bookings]"
                        + " WHERE SalesOrgID = " + str_salesOrgID
                        + " AND DeliverY = '" + str_deliverY + "'"
                        + " AND SegmentID = " + str_segmentID
                        + " AND OperationID = " + str_operationID
                        + " AND (YEAR(TimeFlag) = '" + str_year + "' AND MONTH(TimeFlag) = '03')"
                        + " GROUP BY DeliverY"
                        + " ORDER BY DeliverY ASC";
                //by yyan item8 20110614 add start
            }
            else
            {
                for (int i = 0; i < ds_pro.Tables[0].Rows.Count; i++)
                {
                    temp += ", ROUND(SUM(CASE WHEN ProductID = " + ds_pro.Tables[0].Rows[i][0].ToString().Trim()
                            + " THEN (CASE WHEN DeliverY = '" + str_year.Substring(2, 2) + "' OR DeliverY = 'YTD'"
                            + " THEN Amount*" + rate1.ToString().Replace(',', '.') + " ELSE Amount*" +
                            rate2.ToString().Replace(',', '.') + " END) ELSE 0 END),0) AS  '" +
                            ds_pro.Tables[0].Rows[i][1].ToString().Trim() + "'";
                }
                temp += " FROM [Bookings]"
                        + " WHERE SalesOrgID = " + str_salesOrgID
                        + " AND DeliverY = '" + str_deliverY + "'"
                        + " AND SegmentID = " + str_segmentID
                        + " AND OperationID = " + str_operationID
                        + " AND (YEAR(TimeFlag) = '" + str_year + "' AND MONTH(TimeFlag) = '03')"
                        + " GROUP BY DeliverY"
                        + " ORDER BY DeliverY ASC";
            }
            //by yyan item8 20110614 add end
            sql += temp;
            //ryzhang item49 20110510 add end
            DataSet ds = helper.GetDataSet(sql);

            return ds;
        }
        return null;
    }

    /// <summary>
    /// get sales budget by operation
    /// </summary>
    /// <param name="ds_pro">products</param>
    /// <param name="str_operationID">operation id</param>
    /// <param name="str_segmentID">segment id</param>
    /// <param name="str_year">year</param>
    /// <param name="str_deliverY">deliver year</param>
    /// <param name="flagMoney">1:need to convert to EUR, 0:not need to convert.</param>
    /// <returns>dataset</returns>
    public DataSet getSalesBudgetByOperation(DataSet ds_pro, string str_operationID, string str_segmentID,
                                             string str_year, string str_deliverY, int flagMoney)
        //by yyan item8 20110617 add end
    {
        double operationRate1 = getRate(getOperationCurrency(str_operationID), true, str_year, "03");
        double operationRate2 = getRate(getOperationCurrency(str_operationID), false, str_year, "03");
        if (operationRate1 < 0.000000001 && operationRate1 > -0.00000001)
        {
            operationRate1 = 100000000;
        }
        if (operationRate2 < 0.000000001 && operationRate2 > -0.00000001)
        {
            operationRate2 = 100000000;
        }
        if (ds_pro.Tables[0].Rows.Count > 0)
        {
            //ryzhang item49 20110510 add start
            string sql = "SELECT DeliverY";
            string temp = "";
            //by yyan item8 20110617 add start 
            if (flagMoney == 0)
            {
                //by yyan item8 20110617 add end
                for (int i = 0; i < ds_pro.Tables[0].Rows.Count; i++)
                {
                    temp += ", ROUND(SUM(CASE WHEN ProductID = " + ds_pro.Tables[0].Rows[i][0].ToString().Trim()
                            + " THEN (CASE WHEN DeliverY = '" + str_year.Substring(2, 2) + "' OR DeliverY = 'YTD'"
                            + " THEN Amount/" + operationRate1.ToString().Replace(',', '.') + " ELSE Amount/" +
                            operationRate2.ToString().Replace(',', '.') + " END) ELSE 0 END),0) AS  '" +
                            ds_pro.Tables[0].Rows[i][1].ToString().Trim() + "'";
                }
                string fromsql = "SELECT [SalesOrg].ID,[Bookings].BookingY,"
                                 + " [Bookings].DeliverY,[Bookings].ProductID,[Bookings].OperationID"
                                 + " ,SUM(CASE WHEN [Bookings].DeliverY = '" + str_year.Substring(2, 2) +
                                 "' OR DeliverY = 'YTD'"
                                 +
                                 " THEN [Bookings].Amount*[Currency_Exchange].Rate1 ELSE [Bookings].Amount*[Currency_Exchange].Rate2 END ) AS Amount"
                                 + " ,[Bookings].SegmentID,[Bookings].TimeFlag"
                                 + " FROM [SalesOrg],[Bookings],[Currency_Exchange]"
                                 + " WHERE [SalesOrg].ID = [Bookings].SalesOrgID"
                                 + " AND [SalesOrg].CurrencyID = [Currency_Exchange].CurrencyID"
                                 + " AND YEAR([Currency_Exchange].TimeFlag) = '" + str_year + "'"
                                 + " AND (MONTH([Currency_Exchange].TimeFlag) = '03')"
                                 + " AND (YEAR([Bookings].TimeFlag) = '" + str_year +
                                 "' AND MONTH([Bookings].TimeFlag) = '03')"
                                 + " AND SalesOrg.Deleted=0 "
                                 + " AND Currency_Exchange.Deleted=0 "
                                 + " GROUP BY [SalesOrg].ID,[Bookings].BookingY,[Bookings].DeliverY,"
                                 + " [Bookings].ProductID,[Bookings].OperationID"
                                 + " ,[Bookings].SegmentID,[Bookings].TimeFlag";
                temp += " FROM (" + fromsql + ") AS NewBookings"
                        + " WHERE DeliverY = '" + str_deliverY + "'"
                        + " AND SegmentID = " + str_segmentID
                        + " AND OperationID = " + str_operationID
                        + " AND (YEAR(TimeFlag) = '" + str_year + "' AND MONTH(TimeFlag) = '03')"
                        + " GROUP BY DeliverY"
                        + " ORDER BY DeliverY ASC";
                //by yyan item8 20110617 add start 
            }
            else
            {
                for (int i = 0; i < ds_pro.Tables[0].Rows.Count; i++)
                {
                    temp += ", ROUND(SUM(CASE WHEN ProductID = " + ds_pro.Tables[0].Rows[i][0].ToString().Trim()
                            + " THEN (CASE WHEN DeliverY = '" + str_year.Substring(2, 2) + "' OR DeliverY = 'YTD'"
                            + " THEN Amount ELSE Amount END) ELSE 0 END),0) AS  '" +
                            ds_pro.Tables[0].Rows[i][1].ToString().Trim() + "'";
                }
                string fromsql = "SELECT [SalesOrg].ID,[Bookings].BookingY,"
                                 + " [Bookings].DeliverY,[Bookings].ProductID,[Bookings].OperationID"
                                 + " ,SUM(CASE WHEN [Bookings].DeliverY = '" + str_year.Substring(2, 2) +
                                 "' OR DeliverY = 'YTD'"
                                 +
                                 " THEN [Bookings].Amount*[Currency_Exchange].Rate1 ELSE [Bookings].Amount*[Currency_Exchange].Rate2 END ) AS Amount"
                                 + " ,[Bookings].SegmentID,[Bookings].TimeFlag"
                                 + " FROM [SalesOrg],[Bookings],[Currency_Exchange]"
                                 + " WHERE [SalesOrg].ID = [Bookings].SalesOrgID"
                                 + " AND [SalesOrg].CurrencyID = [Currency_Exchange].CurrencyID"
                                 + " AND YEAR([Currency_Exchange].TimeFlag) = '" + str_year + "'"
                                 + " AND (MONTH([Currency_Exchange].TimeFlag) = '03')"
                                 + " AND (YEAR([Bookings].TimeFlag) = '" + str_year +
                                 "' AND MONTH([Bookings].TimeFlag) = '03')"
                                 + " AND SalesOrg.Deleted=0 "
                                 + " AND Currency_Exchange.Deleted=0 "
                                 + " GROUP BY [SalesOrg].ID,[Bookings].BookingY,[Bookings].DeliverY,"
                                 + " [Bookings].ProductID,[Bookings].OperationID"
                                 + " ,[Bookings].SegmentID,[Bookings].TimeFlag";
                temp += " FROM (" + fromsql + ") AS NewBookings"
                        + " WHERE DeliverY = '" + str_deliverY + "'"
                        + " AND SegmentID = " + str_segmentID
                        + " AND OperationID = " + str_operationID
                        + " AND (YEAR(TimeFlag) = '" + str_year + "' AND MONTH(TimeFlag) = '03')"
                        + " GROUP BY DeliverY"
                        + " ORDER BY DeliverY ASC";
            }
            //by yyan item8 20110617 add end

            sql += temp;
            //ryzhang item49 20110510 add end
            DataSet ds = helper.GetDataSet(sql);

            return ds;
        }
        return null;
    }

    /// <summary>
    /// Get sales budget data
    /// </summary>
    /// <param name="ds_pro">products</param>
    /// <param name="str_segmentID">segment id</param>
    /// <param name="str_year">year</param>
    /// <param name="str_deliverY">deliver year</param>
    /// <returns>dataset</returns>
    public DataSet getSalesBudget(DataSet ds_pro, string str_segmentID, string str_year, string str_deliverY)
    {
        if (ds_pro.Tables[0].Rows.Count > 0)
        {
            string sql = "SELECT DeliverY";
            string temp = "";
            for (int i = 0; i < ds_pro.Tables[0].Rows.Count; i++)
            {
                temp += ",SUM(CASE WHEN [Bookings].ProductID = " + ds_pro.Tables[0].Rows[i][0].ToString().Trim()
                        + " THEN (CASE WHEN [Bookings].DeliverY = '" + str_year.Substring(2, 2) + "' THEN"
                        +
                        " ROUND([Bookings].Amount*[Currency_Exchange].Rate1,0) ELSE ROUND([Bookings].Amount*[Currency_Exchange].Rate2,0) END) ELSE 0 END)"
                        + " AS  '" + ds_pro.Tables[0].Rows[i][1].ToString().Trim() + "'";
            }
            temp += " FROM [Bookings],[SalesOrg],[Currency_Exchange]"
                    + " WHERE [Bookings].SegmentID = " + str_segmentID
                    + " AND [Bookings].SalesOrgID = [SalesOrg].ID"
                    + " AND [Currency_Exchange].CurrencyID = [SalesOrg].CurrencyID"
                    + " AND [Bookings].DeliverY = '" + str_deliverY + "'"
                    + " AND [Bookings].SegmentID = " + str_segmentID
                    + " AND (YEAR([Bookings].TimeFlag) = '" + str_year + "' AND MONTH([Bookings].TimeFlag) = '03')"
                    + " AND YEAR([Currency_Exchange].TimeFlag) = '" + str_year + "'"
                    + " AND MONTH([Currency_Exchange].TimeFlag) = '03'"
                    + " AND SalesOrg.Deleted=0 "
                    + " AND Currency_Exchange.Deleted=0 "
                    + " GROUP BY [Bookings].DeliverY"
                    + " ORDER BY [Bookings].DeliverY ASC";
            sql += temp;
            DataSet ds = helper.GetDataSet(sql);

            return ds;
        }
        return null;
    }

    
    public DataSet getBookingDataByDateByProduct(DataSet dsPro, string segID, string bookingY, string deliverY,
                                                 string salesOrgID)
    {
        if (dsPro != null)
        {
            //ryzhang item49 20110510 add start
            string sqlstr;
            string temp = "";
            sqlstr = "SELECT [SubRegion].Name AS SubRegion,[Country].ISO_Code AS Code";
            for (int count = 0; count < dsPro.Tables[0].Rows.Count; count++)
            {
                temp += " ,SUM(CASE WHEN ProductID = " + dsPro.Tables[0].Rows[count][0]
                        + " AND YEAR(TimeFlag) = '" + selectMeeting.getyear() + "' AND MONTH(TimeFlag) = '" +
                        selectMeeting.getmonth()
                        + "' THEN (CASE WHEN [Bookings].BookingY='" + bookingY + "' AND DeliverY='" + deliverY
                        + "' THEN Amount ELSE 0 END ) ELSE 0 END) AS '"
                        + dsPro.Tables[0].Rows[count][1] + "'";
            }
            temp += " FROM [Bookings]"
                    + " INNER JOIN [SubRegion] ON [Bookings].CountryID = [SubRegion].ID"
                    + " INNER JOIN [Country_SubRegion] ON [Country_SubRegion].SubRegionID = [SubRegion].ID"
                    + " INNER JOIN [Country] ON [Country_SubRegion].CountryID = [Country].ID"
                    + " WHERE SegmentID = " + segID
                    + " AND SalesOrgID = " + salesOrgID
                    + " AND SubRegion.Deleted=0 "
                    + " AND Country_SubRegion.Deleted=0 "
                    + " AND Country.Deleted=0 "
                    + " GROUP BY SubRegion.Name,Country.ISO_Code"
                    + " ORDER BY SubRegion.Name ASC";

            sqlstr += temp;
            //ryzhang item49 20110510 add end
            DataSet ds = helper.GetDataSet(sqlstr);

            if (ds.Tables[0].Rows.Count > 0)
                return ds;
        }
        return null;
    }

    /// <summary>
    ///  Get all countries data that the countries belong to one subregion and one segment
    /// </summary>
    /// <param name="str_ClusterID">ClusterID</param>
    /// <param name="str_segmentID">SegmentID</param>
    /// <returns>dataset</returns>
    public DataSet getBookingDataByCountry(string str_ClusterID, string str_segmentID)
    {
        //by ryzhang 20110530 item49 modify start
        string yearBeforePre = selectMeeting.getyearBeforePre();
        string preyear = selectMeeting.getpreyear();
        string year = selectMeeting.getyear();
        string nextyear = selectMeeting.getnextyear();
        string month = selectMeeting.getmonth();
        //by ryzhang 20110530 item49 modify end
        string sql = "";
        if (str_segmentID != "-1")
        {
            // by daixuesong Item W19 update start
            // sql = "SELECT [SubRegion].Name,"
            sql = "SELECT [Country].Name,"
                  // by daixuesong Item W19 update end
                  + " SUM(CASE WHEN [Bookings].BookingY = '" + yearBeforePre.Substring(2, 2)
                  + "' and YEAR([Bookings].TimeFlag) = '" + yearBeforePre
                  + "' and MONTH([Bookings].TimeFlag) = '10"
                  + "' and YEAR([Currency_Exchange].TimeFlag) = '" + yearBeforePre
                  + "' and MONTH([Currency_Exchange].TimeFlag) = '10'"
                  + " THEN (CASE WHEN [Bookings].DeliverY = '" + yearBeforePre.Substring(2, 2) +
                  "' OR [Bookings].DeliverY = 'YTD'"
                  + " THEN ROUND([Bookings].Amount * [Currency_Exchange].Rate1,0)"
                  + " ELSE ROUND([Bookings].Amount * [Currency_Exchange].Rate2,0) END) "
                  + " ELSE 0 END) AS 'FY " + yearBeforePre + "',"
                  + " SUM(CASE WHEN [Bookings].BookingY = '" + preyear.Substring(2, 2)
                  + "' and YEAR([Bookings].TimeFlag) = '" + preyear
                  + "' and MONTH([Bookings].TimeFlag) = '10"
                  + "' and YEAR([Currency_Exchange].TimeFlag) = '" + preyear
                  + "' and MONTH([Currency_Exchange].TimeFlag) = '10'"
                  + " THEN (CASE WHEN [Bookings].DeliverY = '" + preyear.Substring(2, 2) +
                  "' OR [Bookings].DeliverY = 'YTD'"
                  + " THEN ROUND([Bookings].Amount * [Currency_Exchange].Rate1,0)"
                  + " ELSE ROUND([Bookings].Amount * [Currency_Exchange].Rate2,0) END) "
                  + " ELSE 0 END) AS 'FY " + preyear + "',"
                  + " SUM(CASE WHEN [Bookings].BookingY = '" + year.Substring(2, 2)
                  + "' and YEAR([Bookings].TimeFlag) = '" + year
                  + "' and MONTH([Bookings].TimeFlag) = '" + month
                  + "' and YEAR([Currency_Exchange].TimeFlag) = '" + year
                  + "' and MONTH([Currency_Exchange].TimeFlag) = '" + month + "'"
                  + " THEN (CASE WHEN [Bookings].DeliverY = '" + year.Substring(2, 2) +
                  "' OR [Bookings].DeliverY = 'YTD'"
                  + " THEN ROUND([Bookings].Amount * [Currency_Exchange].Rate1,0)"
                  + " ELSE ROUND([Bookings].Amount * [Currency_Exchange].Rate2,0) END) "
                  + " ELSE 0 END) AS 'FY " + year + "',"
                  + " SUM(CASE WHEN [Bookings].BookingY = '" + nextyear.Substring(2, 2)
                  + "' and YEAR([Bookings].TimeFlag) = '" + year
                  + "' and MONTH([Bookings].TimeFlag) = '" + month
                  + "' and YEAR([Currency_Exchange].TimeFlag) = '" + year
                  + "' and MONTH([Currency_Exchange].TimeFlag) = '" + month + "'"
                  + " THEN (CASE WHEN [Bookings].DeliverY = '" + year.Substring(2, 2) +
                  "' OR [Bookings].DeliverY = 'YTD'"
                  + " THEN ROUND([Bookings].Amount * [Currency_Exchange].Rate1,0)"
                  + " ELSE ROUND([Bookings].Amount * [Currency_Exchange].Rate2,0) END) "
                  + " ELSE 0 END) AS 'FY " + nextyear + "(" + year + "." + month + ")',"
                  + " SUM(CASE WHEN [Bookings].BookingY = '" + nextyear.Substring(2, 2)
                  + "' and YEAR([Bookings].TimeFlag) = '" + year
                  + "' and MONTH([Bookings].TimeFlag) = '03"
                  + "' and YEAR([Currency_Exchange].TimeFlag) = '" + year
                  + "' and MONTH([Currency_Exchange].TimeFlag) = '03'"
                  + " THEN (CASE WHEN [Bookings].DeliverY = '" + year.Substring(2, 2) +
                  "' OR [Bookings].DeliverY = 'YTD'"
                  + " THEN ROUND([Bookings].Amount * [Currency_Exchange].Rate1,0)"
                  + " ELSE ROUND([Bookings].Amount * [Currency_Exchange].Rate2,0) END) "
                  + " ELSE 0 END) AS 'Budget (" + nextyear + ")' "
                  + " FROM [Bookings],[Country],[SubRegion],[Country_SubRegion],[SalesOrg],[Currency_Exchange]"
                  + " WHERE [SubRegion].ID = [Bookings].CountryID"
                  + " AND [Bookings].CountryID = [Country_SubRegion].SubRegionID"
                  + " AND [Country].ID = [Country_SubRegion].CountryID"
                  + " AND [Bookings].SalesOrgID = [SalesOrg].ID"
                  + " AND [Currency_Exchange].CurrencyID = [SalesOrg].CurrencyID"
                  + " AND [Bookings].CountryID IN (SELECT [Country_SubRegion].SubRegionID FROM [Country_SubRegion]"
                  + " INNER JOIN [Cluster_Country] ON [Cluster_Country].CountryID = [Country_SubRegion].CountryID"
                  + " WHERE [Cluster_Country].ClusterID = " + str_ClusterID +
                  " AND [Cluster_Country].Deleted = 0 AND [Country_SubRegion].Deleted = 0)"
                  + " AND [Country].Deleted = 0 AND [SalesOrg].Deleted = 0 AND [Currency_Exchange].Deleted = 0 "
                  + " AND SubRegion.Deleted=0 "
                  + " AND Country_SubRegion.Deleted=0 "
                  + " AND [Bookings].SegmentID = " + str_segmentID
                  + " GROUP BY [Country].Name"
                  + " ORDER BY [Country].Name";
        }
        else
        {
            // by daixuesong Item W19 update start
            //sql = "SELECT [SubRegion].Name,"
            sql = "SELECT [Country].Name,"
                  // by daixuesong Item W19 update end
                  + " SUM(CASE WHEN [Bookings].BookingY = '" + yearBeforePre.Substring(2, 2)
                  + "' and YEAR([Bookings].TimeFlag) = '" + yearBeforePre
                  + "' and MONTH([Bookings].TimeFlag) = '10"
                  + "' and YEAR([Currency_Exchange].TimeFlag) = '" + yearBeforePre
                  + "' and MONTH([Currency_Exchange].TimeFlag) = '10'"
                  + " THEN (CASE WHEN [Bookings].DeliverY = '" + yearBeforePre.Substring(2, 2) +
                  "' OR [Bookings].DeliverY = 'YTD'"
                  + " THEN ROUND([Bookings].Amount * [Currency_Exchange].Rate1,0)"
                  + " ELSE ROUND([Bookings].Amount * [Currency_Exchange].Rate2,0) END) "
                  + " ELSE 0 END) AS 'FY " + yearBeforePre + "',"
                  + " SUM(CASE WHEN [Bookings].BookingY = '" + preyear.Substring(2, 2)
                  + "' and YEAR([Bookings].TimeFlag) = '" + preyear
                  + "' and MONTH([Bookings].TimeFlag) = '10"
                  + "' and YEAR([Currency_Exchange].TimeFlag) = '" + preyear
                  + "' and MONTH([Currency_Exchange].TimeFlag) = '10'"
                  + " THEN (CASE WHEN [Bookings].DeliverY = '" + preyear.Substring(2, 2) +
                  "' OR [Bookings].DeliverY = 'YTD'"
                  + " THEN ROUND([Bookings].Amount * [Currency_Exchange].Rate1,0)"
                  + " ELSE ROUND([Bookings].Amount * [Currency_Exchange].Rate2,0) END) "
                  + " ELSE 0 END) AS 'FY " + preyear + "',"
                  + " SUM(CASE WHEN [Bookings].BookingY = '" + year.Substring(2, 2)
                  + "' and YEAR([Bookings].TimeFlag) = '" + year
                  + "' and MONTH([Bookings].TimeFlag) = '" + month
                  + "' and YEAR([Currency_Exchange].TimeFlag) = '" + year
                  + "' and MONTH([Currency_Exchange].TimeFlag) = '" + month + "'"
                  + " THEN (CASE WHEN [Bookings].DeliverY = '" + year.Substring(2, 2) +
                  "' OR [Bookings].DeliverY = 'YTD'"
                  + " THEN ROUND([Bookings].Amount * [Currency_Exchange].Rate1,0)"
                  + " ELSE ROUND([Bookings].Amount * [Currency_Exchange].Rate2,0) END) "
                  + " ELSE 0 END) AS 'FY " + year + "',"
                  + " SUM(CASE WHEN [Bookings].BookingY = '" + nextyear.Substring(2, 2)
                  + "' and YEAR([Bookings].TimeFlag) = '" + year
                  + "' and MONTH([Bookings].TimeFlag) = '" + month
                  + "' and YEAR([Currency_Exchange].TimeFlag) = '" + year
                  + "' and MONTH([Currency_Exchange].TimeFlag) = '" + month + "'"
                  + " THEN (CASE WHEN [Bookings].DeliverY = '" + year.Substring(2, 2) +
                  "' OR [Bookings].DeliverY = 'YTD'"
                  + " THEN ROUND([Bookings].Amount * [Currency_Exchange].Rate1,0)"
                  + " ELSE ROUND([Bookings].Amount * [Currency_Exchange].Rate2,0) END) "
                  + " ELSE 0 END) AS 'FY " + nextyear + "(" + year + "." + month + ")',"
                  + " SUM(CASE WHEN [Bookings].BookingY = '" + nextyear.Substring(2, 2)
                  + "' and YEAR([Bookings].TimeFlag) = '" + year
                  + "' and MONTH([Bookings].TimeFlag) = '03"
                  + "' and YEAR([Currency_Exchange].TimeFlag) = '" + year
                  + "' and MONTH([Currency_Exchange].TimeFlag) = '03'"
                  + " THEN (CASE WHEN [Bookings].DeliverY = '" + year.Substring(2, 2) +
                  "' OR [Bookings].DeliverY = 'YTD'"
                  + " THEN ROUND([Bookings].Amount * [Currency_Exchange].Rate1,0)"
                  + " ELSE ROUND([Bookings].Amount * [Currency_Exchange].Rate2,0) END) "
                  + " ELSE 0 END) AS 'Budget (" + nextyear + ")' "
                  + " FROM [Bookings],[Country],[SubRegion],[Country_SubRegion],[SalesOrg],[Currency_Exchange]"
                  + " WHERE [SubRegion].ID = [Bookings].CountryID"
                  + " AND [Bookings].CountryID = [Country_SubRegion].SubRegionID"
                  + " AND [Country].ID = [Country_SubRegion].CountryID"
                  + " AND [Bookings].SalesOrgID = [SalesOrg].ID"
                  + " AND [Currency_Exchange].CurrencyID = [SalesOrg].CurrencyID"
                  + " AND [Bookings].CountryID IN (SELECT [Country_SubRegion].SubRegionID FROM [Country_SubRegion]"
                  + " INNER JOIN [Cluster_Country] ON [Cluster_Country].CountryID = [Country_SubRegion].CountryID"
                  + " WHERE [Cluster_Country].ClusterID = " + str_ClusterID +
                  " AND [Cluster_Country].Deleted = 0 AND [Country_SubRegion].Deleted = 0)"
                  + " AND [Country].Deleted = 0 AND [SalesOrg].Deleted = 0 AND [Currency_Exchange].Deleted = 0"
                  + " AND SubRegion.Deleted=0 "
                  + " AND Country_SubRegion.Deleted=0 "
                  + " GROUP BY [Country].Name"
                  + " ORDER BY [Country].Name";
        }
        DataSet ds = helper.GetDataSet(sql);

        if (ds.Tables[0].Rows.Count > 0)
            return ds;
        else
            return null;
    }

    /// <summary>
    ///  Get all sub regions data that the subregions belong to one region and one segment
    /// </summary>
    /// <param name="str_regionID">region id</param>
    /// <param name="str_segmentID">segment id</param>
    /// <returns>dataset</returns>
    public DataSet getBookingDataTotalByCountry(string str_regionID, string str_segmentID)
    {
        //by ryzhang 20110530 item49 modify start
        string yearBeforePre = selectMeeting.getyearBeforePre();
        string preyear = selectMeeting.getpreyear();
        string year = selectMeeting.getyear();
        string nextyear = selectMeeting.getnextyear();
        string month = selectMeeting.getmonth();
        //by ryzhang 20110530 item49 modify end
        string sql = "";
        if (str_segmentID != "-1")
        {
            sql = "SELECT [Cluster].Name,"
                  + " SUM(CASE WHEN [Bookings].BookingY = '" + yearBeforePre.Substring(2, 2)
                  + "' and YEAR([Bookings].TimeFlag) = '" + yearBeforePre
                  + "' and MONTH([Bookings].TimeFlag) = '10"
                  + "' and YEAR([Currency_Exchange].TimeFlag) = '" + yearBeforePre
                  + "' and MONTH([Currency_Exchange].TimeFlag) = '10'"
                  + " THEN (CASE WHEN [Bookings].DeliverY = '" + yearBeforePre.Substring(2, 2) +
                  "' OR [Bookings].DeliverY = 'YTD'"
                  + " THEN ROUND([Bookings].Amount * [Currency_Exchange].Rate1,0)"
                  + " ELSE ROUND([Bookings].Amount * [Currency_Exchange].Rate2,0) END) "
                  + " ELSE 0 END) AS 'FY " + yearBeforePre + "',"
                  + " SUM(CASE WHEN [Bookings].BookingY = '" + preyear.Substring(2, 2)
                  + "' and YEAR([Bookings].TimeFlag) = '" + preyear
                  + "' and MONTH([Bookings].TimeFlag) = '10"
                  + "' and YEAR([Currency_Exchange].TimeFlag) = '" + preyear
                  + "' and MONTH([Currency_Exchange].TimeFlag) = '10'"
                  + " THEN (CASE WHEN [Bookings].DeliverY = '" + preyear.Substring(2, 2) +
                  "' OR [Bookings].DeliverY = 'YTD'"
                  + " THEN ROUND([Bookings].Amount * [Currency_Exchange].Rate1,0)"
                  + " ELSE ROUND([Bookings].Amount * [Currency_Exchange].Rate2,0) END) "
                  + " ELSE 0 END) AS 'FY " + preyear + "',"
                  + " SUM(CASE WHEN [Bookings].BookingY = '" + year.Substring(2, 2)
                  + "' and YEAR([Bookings].TimeFlag) = '" + year
                  + "' and MONTH([Bookings].TimeFlag) = '" + month
                  + "' and YEAR([Currency_Exchange].TimeFlag) = '" + year
                  + "' and MONTH([Currency_Exchange].TimeFlag) = '" + month + "'"
                  + " THEN (CASE WHEN [Bookings].DeliverY = '" + year.Substring(2, 2) +
                  "' OR [Bookings].DeliverY = 'YTD'"
                  + " THEN ROUND([Bookings].Amount * [Currency_Exchange].Rate1,0)"
                  + " ELSE ROUND([Bookings].Amount * [Currency_Exchange].Rate2,0) END) "
                  + " ELSE 0 END) AS 'FY " + year + "',"
                  + " SUM(CASE WHEN [Bookings].BookingY = '" + nextyear.Substring(2, 2)
                  + "' and YEAR([Bookings].TimeFlag) = '" + year
                  + "' and MONTH([Bookings].TimeFlag) = '" + month
                  + "' and YEAR([Currency_Exchange].TimeFlag) = '" + year
                  + "' and MONTH([Currency_Exchange].TimeFlag) = '" + month + "'"
                  + " THEN (CASE WHEN [Bookings].DeliverY = '" + year.Substring(2, 2) +
                  "' OR [Bookings].DeliverY = 'YTD'"
                  + " THEN ROUND([Bookings].Amount * [Currency_Exchange].Rate1,0)"
                  + " ELSE ROUND([Bookings].Amount * [Currency_Exchange].Rate2,0) END) "
                  + " ELSE 0 END) AS 'FY " + nextyear + "(" + year + "." + month + ")',"
                  + " SUM(CASE WHEN [Bookings].BookingY = '" + nextyear.Substring(2, 2)
                  + "' and YEAR([Bookings].TimeFlag) = '" + year
                  + "' and MONTH([Bookings].TimeFlag) = '03"
                  + "' and YEAR([Currency_Exchange].TimeFlag) = '" + year
                  + "' and MONTH([Currency_Exchange].TimeFlag) = '03'"
                  + " THEN (CASE WHEN [Bookings].DeliverY = '" + year.Substring(2, 2) +
                  "' OR [Bookings].DeliverY = 'YTD'"
                  + " THEN ROUND([Bookings].Amount * [Currency_Exchange].Rate1,0)"
                  + " ELSE ROUND([Bookings].Amount * [Currency_Exchange].Rate2,0) END) "
                  + " ELSE 0 END) AS 'Budget (" + nextyear + ")' "
                  +
                  " FROM [Bookings],[SubRegion],[Country_SubRegion],[Country],[Cluster_Country],[Cluster],[SalesOrg],[Currency_Exchange]"
                  + " WHERE [SubRegion].ID = [Bookings].CountryID"
                  + " AND [Bookings].CountryID = [Country_SubRegion].SubRegionID"
                  + " AND [Country].ID = [Country_SubRegion].CountryID"
                  + " AND [Country].ID = [Cluster_Country].CountryID "
                  + " AND [Cluster].ID = [Cluster_Country].ClusterID"
                  + " AND [Bookings].SalesOrgID = [SalesOrg].ID"
                  + " AND [Currency_Exchange].CurrencyID = [SalesOrg].CurrencyID"
                  + " AND [Cluster].ID IN (SELECT ClusterID FROM [Region_Cluster] WHERE RegionID = " + str_regionID +
                  " AND [Region_Cluster].Deleted = 0)"
                  + " AND [Country].Deleted = 0 AND [SalesOrg].Deleted = 0 AND [Currency_Exchange].Deleted = 0"
                  + " AND [Cluster_Country].Deleted = 0 AND [Cluster].Deleted = 0"
                  + " AND SubRegion.Deleted=0 "
                  + " AND Country_SubRegion.Deleted=0 "
                  + " AND [Bookings].SegmentID = " + str_segmentID
                  + " GROUP BY [Cluster].Name";
        }
        else
        {
            sql = "SELECT [Cluster].Name,"
                  + " SUM(CASE WHEN [Bookings].BookingY = '" + yearBeforePre.Substring(2, 2)
                  + "' and YEAR([Bookings].TimeFlag) = '" + yearBeforePre
                  + "' and MONTH([Bookings].TimeFlag) = '10"
                  + "' and YEAR([Currency_Exchange].TimeFlag) = '" + yearBeforePre
                  + "' and MONTH([Currency_Exchange].TimeFlag) = '10'"
                  + " THEN (CASE WHEN [Bookings].DeliverY = '" + yearBeforePre.Substring(2, 2) +
                  "' OR [Bookings].DeliverY = 'YTD'"
                  + " THEN ROUND([Bookings].Amount * [Currency_Exchange].Rate1,0)"
                  + " ELSE ROUND([Bookings].Amount * [Currency_Exchange].Rate2,0) END) "
                  + " ELSE 0 END) AS 'FY " + yearBeforePre + "',"
                  + " SUM(CASE WHEN [Bookings].BookingY = '" + preyear.Substring(2, 2)
                  + "' and YEAR([Bookings].TimeFlag) = '" + preyear
                  + "' and MONTH([Bookings].TimeFlag) = '10"
                  + "' and YEAR([Currency_Exchange].TimeFlag) = '" + preyear
                  + "' and MONTH([Currency_Exchange].TimeFlag) = '10'"
                  + " THEN (CASE WHEN [Bookings].DeliverY = '" + preyear.Substring(2, 2) +
                  "' OR [Bookings].DeliverY = 'YTD'"
                  + " THEN ROUND([Bookings].Amount * [Currency_Exchange].Rate1,0)"
                  + " ELSE ROUND([Bookings].Amount * [Currency_Exchange].Rate2,0) END) "
                  + " ELSE 0 END) AS 'FY " + preyear + "',"
                  + " SUM(CASE WHEN [Bookings].BookingY = '" + year.Substring(2, 2)
                  + "' and YEAR([Bookings].TimeFlag) = '" + year
                  + "' and MONTH([Bookings].TimeFlag) = '" + month
                  + "' and YEAR([Currency_Exchange].TimeFlag) = '" + year
                  + "' and MONTH([Currency_Exchange].TimeFlag) = '" + month + "'"
                  + " THEN (CASE WHEN [Bookings].DeliverY = '" + year.Substring(2, 2) +
                  "' OR [Bookings].DeliverY = 'YTD'"
                  + " THEN ROUND([Bookings].Amount * [Currency_Exchange].Rate1,0)"
                  + " ELSE ROUND([Bookings].Amount * [Currency_Exchange].Rate2,0) END) "
                  + " ELSE 0 END) AS 'FY " + year + "',"
                  + " SUM(CASE WHEN [Bookings].BookingY = '" + nextyear.Substring(2, 2)
                  + "' and YEAR([Bookings].TimeFlag) = '" + year
                  + "' and MONTH([Bookings].TimeFlag) = '" + month
                  + "' and YEAR([Currency_Exchange].TimeFlag) = '" + year
                  + "' and MONTH([Currency_Exchange].TimeFlag) = '" + month + "'"
                  + " THEN (CASE WHEN [Bookings].DeliverY = '" + year.Substring(2, 2) +
                  "' OR [Bookings].DeliverY = 'YTD'"
                  + " THEN ROUND([Bookings].Amount * [Currency_Exchange].Rate1,0)"
                  + " ELSE ROUND([Bookings].Amount * [Currency_Exchange].Rate2,0) END) "
                  + " ELSE 0 END) AS 'FY " + nextyear + "(" + year + "." + month + ")',"
                  + " SUM(CASE WHEN [Bookings].BookingY = '" + nextyear.Substring(2, 2)
                  + "' and YEAR([Bookings].TimeFlag) = '" + year
                  + "' and MONTH([Bookings].TimeFlag) = '03"
                  + "' and YEAR([Currency_Exchange].TimeFlag) = '" + year
                  + "' and MONTH([Currency_Exchange].TimeFlag) = '03'"
                  + " THEN (CASE WHEN [Bookings].DeliverY = '" + year.Substring(2, 2) +
                  "' OR [Bookings].DeliverY = 'YTD'"
                  + " THEN ROUND([Bookings].Amount * [Currency_Exchange].Rate1,0)"
                  + " ELSE ROUND([Bookings].Amount * [Currency_Exchange].Rate2,0) END) "
                  + " ELSE 0 END) AS 'Budget (" + nextyear + ")' "
                  +
                  " FROM [Bookings],[SubRegion],[Country_SubRegion],[Country],[Cluster_Country],[Cluster],[SalesOrg],[Currency_Exchange]"
                  + " WHERE [SubRegion].ID = [Bookings].CountryID"
                  + " AND [Bookings].CountryID = [Country_SubRegion].SubRegionID"
                  + " AND [Country].ID = [Country_SubRegion].CountryID"
                  + " AND [Country].ID = [Cluster_Country].CountryID "
                  + " AND [Cluster].ID = [Cluster_Country].ClusterID"
                  + " AND [Bookings].SalesOrgID = [SalesOrg].ID"
                  + " AND [Currency_Exchange].CurrencyID = [SalesOrg].CurrencyID"
                  + " AND [Cluster].ID IN (SELECT ClusterID FROM [Region_Cluster] WHERE RegionID = " + str_regionID +
                  " AND [Region_Cluster].Deleted = 0)"
                  + " AND [Country].Deleted = 0 AND [SalesOrg].Deleted = 0 AND [Currency_Exchange].Deleted = 0"
                  + " AND [Cluster_Country].Deleted = 0 AND [Cluster].Deleted = 0"
                  + " AND SubRegion.Deleted=0 "
                  + " AND Country_SubRegion.Deleted=0 "
                  + " GROUP BY [Cluster].Name";
        }

        DataSet ds = helper.GetDataSet(sql);

        if (ds.Tables[0].Rows.Count > 0)
            return ds;
        else
            return null;
    }

    
    public DataSet getTotalRegion(string str_segmentID)
    {
        //by ryzhang 20110530 item49 modify start
        string yearBeforePre = selectMeeting.getyearBeforePre();
        string preyear = selectMeeting.getpreyear();
        string year = selectMeeting.getyear();
        string nextyear = selectMeeting.getnextyear();
        string month = selectMeeting.getmonth();
        //by ryzhang 20110530 item49 modify end
        if (str_segmentID != "-1")
        {
            string sql = "SELECT "
                         + " SUM(CASE WHEN [Bookings].BookingY = '" + yearBeforePre.Substring(2, 2)
                         + "' and YEAR([Bookings].TimeFlag) = '" + yearBeforePre
                         + "' and MONTH([Bookings].TimeFlag) = '10"
                         + "' and YEAR([Currency_Exchange].TimeFlag) = '" + yearBeforePre
                         + "' and MONTH([Currency_Exchange].TimeFlag) = '10'"
                         + " THEN (CASE WHEN [Bookings].DeliverY = '" + yearBeforePre.Substring(2, 2) +
                         "' OR [Bookings].DeliverY = 'YTD'"
                         + " THEN ROUND([Bookings].Amount * [Currency_Exchange].Rate1,0)"
                         + " ELSE ROUND([Bookings].Amount * [Currency_Exchange].Rate2,0) END) "
                         + " ELSE 0 END) AS 'FY " + yearBeforePre + "',"
                         + " SUM(CASE WHEN [Bookings].BookingY = '" + preyear.Substring(2, 2)
                         + "' and YEAR([Bookings].TimeFlag) = '" + preyear
                         + "' and MONTH([Bookings].TimeFlag) = '10"
                         + "' and YEAR([Currency_Exchange].TimeFlag) = '" + preyear
                         + "' and MONTH([Currency_Exchange].TimeFlag) = '10'"
                         + " THEN (CASE WHEN [Bookings].DeliverY = '" + preyear.Substring(2, 2) +
                         "' OR [Bookings].DeliverY = 'YTD'"
                         + " THEN ROUND([Bookings].Amount * [Currency_Exchange].Rate1,0)"
                         + " ELSE ROUND([Bookings].Amount * [Currency_Exchange].Rate2,0) END) "
                         + " ELSE 0 END) AS 'FY " + preyear + "',"
                         + " SUM(CASE WHEN [Bookings].BookingY = '" + year.Substring(2, 2)
                         + "' and YEAR([Bookings].TimeFlag) = '" + year
                         + "' and MONTH([Bookings].TimeFlag) = '" + month
                         + "' and YEAR([Currency_Exchange].TimeFlag) = '" + year
                         + "' and MONTH([Currency_Exchange].TimeFlag) = '" + month + "'"
                         + " THEN (CASE WHEN [Bookings].DeliverY = '" + year.Substring(2, 2) +
                         "' OR [Bookings].DeliverY = 'YTD'"
                         + " THEN ROUND([Bookings].Amount * [Currency_Exchange].Rate1,0)"
                         + " ELSE ROUND([Bookings].Amount * [Currency_Exchange].Rate2,0) END) "
                         + " ELSE 0 END) AS 'FY " + year + "',"
                         + " SUM(CASE WHEN [Bookings].BookingY = '" + nextyear.Substring(2, 2)
                         + "' and YEAR([Bookings].TimeFlag) = '" + year
                         + "' and MONTH([Bookings].TimeFlag) = '" + month
                         + "' and YEAR([Currency_Exchange].TimeFlag) = '" + year
                         + "' and MONTH([Currency_Exchange].TimeFlag) = '" + month + "'"
                         + " THEN (CASE WHEN [Bookings].DeliverY = '" + year.Substring(2, 2) +
                         "' OR [Bookings].DeliverY = 'YTD'"
                         + " THEN ROUND([Bookings].Amount * [Currency_Exchange].Rate1,0)"
                         + " ELSE ROUND([Bookings].Amount * [Currency_Exchange].Rate2,0) END) "
                         + " ELSE 0 END) AS 'FY " + nextyear + "(" + year + "." + month + ")',"
                         + " SUM(CASE WHEN [Bookings].BookingY = '" + nextyear.Substring(2, 2)
                         + "' and YEAR([Bookings].TimeFlag) = '" + year
                         + "' and MONTH([Bookings].TimeFlag) = '03"
                         + "' and YEAR([Currency_Exchange].TimeFlag) = '" + year
                         + "' and MONTH([Currency_Exchange].TimeFlag) = '03'"
                         + " THEN (CASE WHEN [Bookings].DeliverY = '" + year.Substring(2, 2) +
                         "' OR [Bookings].DeliverY = 'YTD'"
                         + " THEN ROUND([Bookings].Amount * [Currency_Exchange].Rate1,0)"
                         + " ELSE ROUND([Bookings].Amount * [Currency_Exchange].Rate2,0) END) "
                         + " ELSE 0 END) AS 'Budget (" + nextyear + ")' "
                         +
                         " FROM [Bookings],[SubRegion],[Country_SubRegion],[Country],[Cluster_Country],[Cluster],[SalesOrg],[Currency_Exchange]"
                         + " WHERE [SubRegion].ID = [Bookings].CountryID"
                         + " AND [Bookings].CountryID = [Country_SubRegion].SubRegionID"
                         + " AND [Country].ID = [Country_SubRegion].CountryID"
                         + " AND [Country].ID = [Cluster_Country].CountryID "
                         + " AND [Cluster].ID = [Cluster_Country].ClusterID"
                         + " AND [Bookings].SalesOrgID = [SalesOrg].ID"
                         + " AND [Currency_Exchange].CurrencyID = [SalesOrg].CurrencyID"
                         + " AND [Country].Deleted = 0 AND [SalesOrg].Deleted = 0 AND [Currency_Exchange].Deleted = 0"
                         + " AND [Cluster_Country].Deleted = 0 AND [Cluster].Deleted = 0"
                         + " AND SubRegion.Deleted=0 "
                         + " AND Country_SubRegion.Deleted=0 "
                         + " AND [Bookings].SegmentID = " + str_segmentID;
            DataSet ds = helper.GetDataSet(sql);

            if (ds.Tables[0].Rows.Count > 0)
                return ds;
            else
                return null;
        }
        else
        {
            string sql = "SELECT "
                         + " SUM(CASE WHEN [Bookings].BookingY = '" + yearBeforePre.Substring(2, 2)
                         + "' and YEAR([Bookings].TimeFlag) = '" + yearBeforePre
                         + "' and MONTH([Bookings].TimeFlag) = '10"
                         + "' and YEAR([Currency_Exchange].TimeFlag) = '" + yearBeforePre
                         + "' and MONTH([Currency_Exchange].TimeFlag) = '10'"
                         + " THEN (CASE WHEN [Bookings].DeliverY = '" + yearBeforePre.Substring(2, 2) +
                         "' OR [Bookings].DeliverY = 'YTD'"
                         + " THEN ROUND([Bookings].Amount * [Currency_Exchange].Rate1,0)"
                         + " ELSE ROUND([Bookings].Amount * [Currency_Exchange].Rate2,0) END) "
                         + " ELSE 0 END) AS 'FY " + yearBeforePre + "',"
                         + " SUM(CASE WHEN [Bookings].BookingY = '" + preyear.Substring(2, 2)
                         + "' and YEAR([Bookings].TimeFlag) = '" + preyear
                         + "' and MONTH([Bookings].TimeFlag) = '10"
                         + "' and YEAR([Currency_Exchange].TimeFlag) = '" + preyear
                         + "' and MONTH([Currency_Exchange].TimeFlag) = '10'"
                         + " THEN (CASE WHEN [Bookings].DeliverY = '" + preyear.Substring(2, 2) +
                         "' OR [Bookings].DeliverY = 'YTD'"
                         + " THEN ROUND([Bookings].Amount * [Currency_Exchange].Rate1,0)"
                         + " ELSE ROUND([Bookings].Amount * [Currency_Exchange].Rate2,0) END) "
                         + " ELSE 0 END) AS 'FY " + preyear + "',"
                         + " SUM(CASE WHEN [Bookings].BookingY = '" + year.Substring(2, 2)
                         + "' and YEAR([Bookings].TimeFlag) = '" + year
                         + "' and MONTH([Bookings].TimeFlag) = '" + month
                         + "' and YEAR([Currency_Exchange].TimeFlag) = '" + year
                         + "' and MONTH([Currency_Exchange].TimeFlag) = '" + month + "'"
                         + " THEN (CASE WHEN [Bookings].DeliverY = '" + year.Substring(2, 2) +
                         "' OR [Bookings].DeliverY = 'YTD'"
                         + " THEN ROUND([Bookings].Amount * [Currency_Exchange].Rate1,0)"
                         + " ELSE ROUND([Bookings].Amount * [Currency_Exchange].Rate2,0) END) "
                         + " ELSE 0 END) AS 'FY " + year + "',"
                         + " SUM(CASE WHEN [Bookings].BookingY = '" + nextyear.Substring(2, 2)
                         + "' and YEAR([Bookings].TimeFlag) = '" + year
                         + "' and MONTH([Bookings].TimeFlag) = '" + month
                         + "' and YEAR([Currency_Exchange].TimeFlag) = '" + year
                         + "' and MONTH([Currency_Exchange].TimeFlag) = '" + month + "'"
                         + " THEN (CASE WHEN [Bookings].DeliverY = '" + year.Substring(2, 2) +
                         "' OR [Bookings].DeliverY = 'YTD'"
                         + " THEN ROUND([Bookings].Amount * [Currency_Exchange].Rate1,0)"
                         + " ELSE ROUND([Bookings].Amount * [Currency_Exchange].Rate2,0) END) "
                         + " ELSE 0 END) AS 'FY " + nextyear + "(" + year + "." + month + ")',"
                         + " SUM(CASE WHEN [Bookings].BookingY = '" + nextyear.Substring(2, 2)
                         + "' and YEAR([Bookings].TimeFlag) = '" + year
                         + "' and MONTH([Bookings].TimeFlag) = '03"
                         + "' and YEAR([Currency_Exchange].TimeFlag) = '" + year
                         + "' and MONTH([Currency_Exchange].TimeFlag) = '03'"
                         + " THEN (CASE WHEN [Bookings].DeliverY = '" + year.Substring(2, 2) +
                         "' OR [Bookings].DeliverY = 'YTD'"
                         + " THEN ROUND([Bookings].Amount * [Currency_Exchange].Rate1,0)"
                         + " ELSE ROUND([Bookings].Amount * [Currency_Exchange].Rate2,0) END) "
                         + " ELSE 0 END) AS 'Budget (" + nextyear + ")' "
                         +
                         " FROM [Bookings],[SubRegion],[Country_SubRegion],[Country],[Cluster_Country],[Cluster],[SalesOrg],[Currency_Exchange]"
                         + " WHERE [SubRegion].ID = [Bookings].CountryID"
                         + " AND [Bookings].CountryID = [Country_SubRegion].SubRegionID"
                         + " AND [Country].ID = [Country_SubRegion].CountryID"
                         + " AND [Country].ID = [Cluster_Country].CountryID "
                         + " AND [Cluster].ID = [Cluster_Country].ClusterID"
                         + " AND [Bookings].SalesOrgID = [SalesOrg].ID"
                         + " AND [Currency_Exchange].CurrencyID = [SalesOrg].CurrencyID"
                         + " AND [Country].Deleted = 0 AND [SalesOrg].Deleted = 0 AND [Currency_Exchange].Deleted = 0"
                         + " AND [Cluster_Country].Deleted = 0 AND [Cluster].Deleted = 0"
                         + " AND SubRegion.Deleted=0 "
                         + " AND Country_SubRegion.Deleted=0 ";
            DataSet ds = helper.GetDataSet(sql);

            if (ds.Tables[0].Rows.Count > 0)
                return ds;
            else
                return null;
        }
    }
    /// <summary>
    /// get all region info
    /// </summary>
    /// <returns>dataset</returns>
    public DataSet getRegionInfo()
    {
        string sql = "";
        DataSet ds = null;

        //sql = "SELECT ID, Name FROM [Region] WHERE Deleted = 0 GROUP BY Name, ID ORDER BY Name, ID ASC";
        sql = "SELECT ID, Name FROM [Region] WHERE Deleted = 0  ORDER BY Name, ID ASC";
        ds = helper.GetDataSet(sql);
        return ds;
    }

    /// <summary>
    /// get country iso by id
    /// </summary>
    /// <returns>ISO_Code</returns>
    public string getCountryByID(string str_ID)
    {
        string sql_country = "SELECT ISO_Code FROM [Country] WHERE ID = '" + str_ID + "' AND Deleted = 0";
        try
        {
            DataSet ds_country = helper.GetDataSet(sql_country);
            log.WriteLog(LogUtility.LogErrorLevel.LOG_NOISE,
                         "SQLStatment.cs:(" + sql_country + "), execute successfully.");
            return ds_country.Tables[0].Rows[0][0].ToString().Trim();
        }
        catch (Exception ex)
        {
            log.WriteLog(LogUtility.LogErrorLevel.LOG_CRITICAL,
                         "SQLStatment.cs:(" + sql_country + "), Exception:" + ex.Message);
            return "";
        }
    }

    /// <summary>
    /// get subregion by id
    /// </summary>
    /// <returns>subregion name</returns>
    public string getSubRegionByID(string str_ID)
    {
        string sql_country = "SELECT Name FROM [SubRegion] WHERE ID = '" + str_ID + "' AND Deleted = 0";
        try
        {
            DataSet ds_country = helper.GetDataSet(sql_country);
            log.WriteLog(LogUtility.LogErrorLevel.LOG_NOISE,
                         "SQLStatment.cs:(" + sql_country + "), execute successfully.");
            return ds_country.Tables[0].Rows[0][0].ToString().Trim();
        }
        catch (Exception ex)
        {
            log.WriteLog(LogUtility.LogErrorLevel.LOG_CRITICAL,
                         "SQLStatment.cs:(" + sql_country + "), Exception:" + ex.Message);
            return "";
        }
    }

    /// <summary>
    /// get all salesChannel info
    /// </summary>
    /// <returns>dataset</returns>
    public DataSet getSalesChannelInfo()
    {
        string sql_channel = "SELECT ID, Name FROM [SalesChannel] WHERE Deleted = 0 ORDER BY Name ASC";
        DataSet ds_channel = helper.GetDataSet(sql_channel);
        return ds_channel;
    }

    // add by zy 20110121 end

    //Edit by Forquan at 2011-12-29 begin

    /// <summary>
    /// get all Currency id and name.
    /// </summary>
    /// <returns>dataset</returns>
    public DataSet getCurrencyName()
    {
        string query_currency = "SELECT ID,Name FROM [Currency] WHERE Deleted=0  ORDER BY ID ASC";
        DataSet ds = helper.GetDataSet(query_currency);

        return ds;
    }

    #region DingJunjie Add

    // By DingJunjie 20110516 Add Start
    /// <summary>
    /// Get All User Operation relation.
    /// </summary>
    /// <returns>All User Operation relation</returns>
    public DataSet getAllUserOperation()
    {
        var sql = new StringBuilder();
        sql.AppendLine(" SELECT ");
        sql.AppendLine("   User_Operation.UserID, ");
        sql.AppendLine("   Operation.ID, ");
        sql.AppendLine("   Operation.AbbrL ");
        sql.AppendLine(" FROM ");
        sql.AppendLine("   Operation ");
        sql.AppendLine("   INNER JOIN User_Operation ON Operation.ID=User_Operation.OperationID ");
        sql.AppendLine(" WHERE ");
        sql.AppendLine("   User_Operation.Deleted=0 ");
        sql.AppendLine("   AND Operation.Deleted=0 ");
        sql.AppendLine(" ORDER BY ");
        sql.AppendLine("   User_Operation.UserID, ");
        sql.AppendLine("   Operation.AbbrL, ");
        sql.AppendLine("   Operation.ID ");
        try
        {
            DataSet ds_operation = helper.GetDataSet(sql.ToString());
            log.WriteLog(LogUtility.LogErrorLevel.LOG_NOISE, "SQLStatment.cs:(" + sql + "), execute successfully.");
            return ds_operation;
        }
        catch (Exception ex)
        {
            log.WriteLog(LogUtility.LogErrorLevel.LOG_CRITICAL, "SQLStatment.cs:(" + sql + "), Exception:" + ex.Message);
            return null;
        }
    }

    /// <summary>
    /// Get All User Segment relations
    /// </summary>
    /// <returns>All User Segment relations</returns>
    public DataSet getAllUserSegment()
    {
        var sql = new StringBuilder();
        sql.AppendLine(" SELECT ");
        sql.AppendLine("   User_Segment.UserID, ");
        sql.AppendLine("   Segment.ID, ");
        sql.AppendLine("   Segment.Abbr ");
        sql.AppendLine(" FROM ");
        sql.AppendLine("   Segment ");
        sql.AppendLine("   INNER JOIN User_Segment ON Segment.ID=User_Segment.SegmentID ");
        sql.AppendLine(" WHERE ");
        sql.AppendLine("   User_Segment.Deleted=0 ");
        sql.AppendLine("   AND Segment.Deleted=0 ");
        sql.AppendLine(" ORDER BY ");
        sql.AppendLine("   User_Segment.UserID, ");
        sql.AppendLine("   Segment.Abbr, ");
        sql.AppendLine("   Segment.ID ");
        try
        {
            DataSet ds_segment = helper.GetDataSet(sql.ToString());
            log.WriteLog(LogUtility.LogErrorLevel.LOG_NOISE, "SQLStatment.cs:(" + sql + "), execute successfully.");
            return ds_segment;
        }
        catch (Exception ex)
        {
            log.WriteLog(LogUtility.LogErrorLevel.LOG_CRITICAL, "SQLStatment.cs:(" + sql + "), Exception:" + ex.Message);
            return null;
        }
    }

    /// <summary>
    /// Get All User SubRegion relations
    /// </summary>
    /// <returns>All User SubRegion relations</returns>
    public DataSet getAllUserSubRegion()
    {
        var sql = new StringBuilder();
        sql.AppendLine(" SELECT ");
        sql.AppendLine("   User_Country.UserID, ");
        sql.AppendLine("   SubRegion.ID, ");
        sql.AppendLine("   SubRegion.Name ");
        sql.AppendLine(" FROM ");
        sql.AppendLine("   SubRegion ");
        sql.AppendLine("   INNER JOIN User_Country ON SubRegion.ID=User_Country.CountryID ");
        sql.AppendLine(" WHERE ");
        sql.AppendLine("   User_Country.Deleted=0 ");
        sql.AppendLine("   AND SubRegion.Deleted=0 ");
        sql.AppendLine(" ORDER BY ");
        sql.AppendLine("   User_Country.UserID, ");
        sql.AppendLine("   SubRegion.Name, ");
        sql.AppendLine("   SubRegion.ID ");
        try
        {
            DataSet ds_country = helper.GetDataSet(sql.ToString());
            log.WriteLog(LogUtility.LogErrorLevel.LOG_NOISE, "SQLStatment.cs:(" + sql + "), execute successfully.");
            return ds_country;
        }
        catch (Exception ex)
        {
            log.WriteLog(LogUtility.LogErrorLevel.LOG_CRITICAL, "SQLStatment.cs:(" + sql + "), Exception:" + ex.Message);
            return null;
        }
    }

    /// <summary>
    /// Get All Operation Count
    /// </summary>
    /// <returns>Operation count</returns>
    public string getAllOperationCount()
    {
        string sql = "SELECT COUNT(ID) FROM Operation WHERE Deleted=0";
        try
        {
            object count = helper.ExecuteScalar(CommandType.Text, sql, null);
            log.WriteLog(LogUtility.LogErrorLevel.LOG_NOISE, "SQLStatment.cs:(" + sql + "), execute successfully.");
            return count.ToString();
        }
        catch (Exception ex)
        {
            log.WriteLog(LogUtility.LogErrorLevel.LOG_CRITICAL, "SQLStatment.cs:(" + sql + "), Exception:" + ex.Message);
            return null;
        }
    }

    /// <summary>
    /// Get All subregion Count
    /// </summary>
    /// <returns>subregion count</returns>
    public string getAllSubRegionCount()
    {
        var sql = new StringBuilder();
        sql.AppendLine(" SELECT ");
        sql.AppendLine("   COUNT(DISTINCT SubRegion.ID) ");
        sql.AppendLine(" FROM ");
        sql.AppendLine(" Country_SubRegion ");
        sql.AppendLine("   INNER JOIN Country ON Country.ID=Country_SubRegion.CountryID ");
        sql.AppendLine("   INNER JOIN SubRegion ON SubRegion.ID=Country_SubRegion.SubRegionID ");
        sql.AppendLine(" WHERE ");
        sql.AppendLine("   Country_SubRegion.Deleted=0 ");
        sql.AppendLine("   AND Country.Deleted=0 ");
        sql.AppendLine("   AnD SubRegion.Deleted=0 ");
        try
        {
            object count = helper.ExecuteScalar(CommandType.Text, sql.ToString(), null);
            log.WriteLog(LogUtility.LogErrorLevel.LOG_NOISE, "SQLStatment.cs:(" + sql + "), execute successfully.");
            return count.ToString();
        }
        catch (Exception ex)
        {
            log.WriteLog(LogUtility.LogErrorLevel.LOG_CRITICAL, "SQLStatment.cs:(" + sql + "), Exception:" + ex.Message);
            return null;
        }
    }

    // By DingJunjie 20110516 Add End

    // By DingJunjie 20110520 Item 12 Add Start
    /// <summary>
    /// Get All Cluster
    /// </summary>
    /// <returns>return dataset</returns>
    public DataSet getClusterInfo()
    {
        string sql_cluster =
            "SELECT [Cluster].ID, [Cluster].Name FROM [Cluster] WHERE [Cluster].Deleted=0 ORDER BY [Cluster].Name";
        try
        {
            DataSet ds_cluster = helper.GetDataSet(sql_cluster);
            log.WriteLog(LogUtility.LogErrorLevel.LOG_NOISE,
                         "SQLStatment.cs:(" + sql_cluster + "), execute successfully.");
            return ds_cluster;
        }
        catch (Exception ex)
        {
            log.WriteLog(LogUtility.LogErrorLevel.LOG_CRITICAL,
                         "SQLStatment.cs:(" + sql_cluster + "), Exception:" + ex.Message);
            return null;
        }
    }

    // By DingJunjie 20110520 Item 12 Add End

    // By DingJunjie 20110608 ItemW18 Add Start
    /// <summary>
    /// Get Customer Name By Name
    /// </summary>
    /// <param name="cusName">Customer Name</param>
    /// <returns>Customer Names</returns>
    public DataSet getCustomerNameByName(string cusName)
    {
        string sql =
            "SELECT ID, Name AS 'Customer Name' FROM [CustomerName] WHERE Name LIKE @NAME ESCAPE '/' AND Deleted=0 ORDER BY Name ASC";
        SqlParameter[] sqlParams = {
                                       new SqlParameter("@NAME",
                                                        "%" +
                                                        cusName.Replace("/", "//").Replace("%", "/%").Replace("_", "/_") +
                                                        "%")
                                   };
        return helper.GetDataSet(sql, CommandType.Text, sqlParams);
    }

    public DataSet getCustomerNameByName1(string cusName)
    {
        var sql = new StringBuilder();
        sql.AppendLine(" SELECT ");
        sql.AppendLine("   Customer.ID, ");
        sql.AppendLine("   CustomerName.Name as 'Customer Name', ");
        sql.AppendLine(" CustomerType.Name as  'Customer Type', ");
        sql.AppendLine(" SubRegion.Name as 'SubRegion Name' ");
        sql.AppendLine(" FROM ");
        sql.AppendLine("   Customer ");
        sql.AppendLine("   INNER JOIN CustomerName ON Customer.NameID=CustomerName.ID ");
        sql.AppendLine("   INNER JOIN SubRegion ON SubRegion.ID=Customer.CountryID ");
        sql.AppendLine("   INNER JOIN CustomerType ON Customer.TypeID=CustomerType.ID ");
        sql.AppendLine(" WHERE ");
        sql.AppendLine("   Customer.Deleted=0 ");
        sql.AppendLine("   AND CustomerName.Deleted=0 ");
        sql.AppendLine("   AND SubRegion.Deleted=0 ");
        sql.AppendLine("   AND CustomerType.Deleted=0 ");
        sql.AppendLine(" and CustomerName.Name LIKE @NAME ESCAPE '/' ");
        sql.AppendLine(" ORDER BY ");
        sql.AppendLine("   CustomerName.Name ");
        string query_customer = sql.ToString();
        SqlParameter[] sqlParams = {
                                       new SqlParameter("@NAME",
                                                        "%" +
                                                        cusName.Replace("/", "//").Replace("%", "/%").Replace("_", "/_") +
                                                        "%")
                                   };
        return helper.GetDataSet(query_customer, CommandType.Text, sqlParams);
    }

    // By DingJunjie 20110608 ItemW18 Add End

    /// <summary>
    /// Update product
    /// </summary>
    /// <param name="abbr">string product abbr</param>
    /// <returns>if success, return false, else true</returns>
    public bool updProductByAbbr(string abbr)
    {
        string updSQL = "UPDATE [Product] SET Deleted=0 WHERE Abbr=@Abbr";
        var parameters = new SqlParameter[1];
        parameters[0] = new SqlParameter("@Abbr", abbr);
        int count = helper.ExecuteNonQuery(CommandType.Text, updSQL, parameters);
        if (count > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Get booking sales data by bookingY and deliverY
    /// </summary>
    /// <param name="sql">SQL for Getting booking sales data</param>
    /// <param name="dsPro">DataSet of product</param>
    /// <param name="salesOrgID">Sales Organization ID</param>
    /// <param name="bookingY">Booking year</param>
    /// <param name="deliverY">Deliver year</param>
    /// <param name="year">Year</param>
    /// <param name="month">Month</param>
    /// <param name="convertFlag">true for converting to Euro, false for not converting</param>
    /// <returns>DataSet</returns>
    public DataSet getBookingSalesDataByBD(string sql, DataSet dsPro, string salesOrgID,
                                           string bookingY, string deliverY, string year, string month, bool convertFlag)
    {
        DataSet ds = helper.GetDataSet(sql);
        if (ds != null && ds.Tables.Count > 0)
        {
            DataTable dt = ds.Tables[0].Clone();
            dt.Columns.RemoveAt(4);
            dt.Columns.RemoveAt(4);
            dt.Columns.RemoveAt(4);
            dt.Columns.RemoveAt(4);
            dt.Columns.RemoveAt(dt.Columns.Count - 1);
            dt.Columns.RemoveAt(dt.Columns.Count - 1);
            dt.Columns.RemoveAt(dt.Columns.Count - 1);
            dt.Columns.RemoveAt(dt.Columns.Count - 1);
            dt.Columns.RemoveAt(dt.Columns.Count - 1);
            dt.Columns.RemoveAt(dt.Columns.Count - 1);
            dt.Columns.Add("Total");
            for (int i = 0; i < dsPro.Tables[0].Rows.Count; i++)
            {
                dt.Columns.Add(dsPro.Tables[0].Rows[i][1].ToString());
                dt.Columns.Add("OperationAbbr" + i);
                dt.Columns.Add("Comments" + i);
            }
            DataRow[] rows = null;
            DataRow row = null;
            string where = null;
            int index = 0;
            int sum = 0;
            var productIDList = new List<object>();
            var operationIDList = new List<object>();
            var operationNameList = new List<object>();
            string currency = getSalesOrgCurrency(salesOrgID);
            double db_rate1 = getRate(currency, true, year, month);
            double db_rate2 = getRate(currency, false, year, month);
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                where = "SubRegionID=" + ds.Tables[0].Rows[i][0] + " AND CustomerID=" + ds.Tables[0].Rows[i][1]
                        + " AND ProjectID=" + ds.Tables[0].Rows[i][2] + " AND SalesChannelID=" + ds.Tables[0].Rows[i][3] +
                        " and RecordID=" + ds.Tables[0].Rows[i][8];
                rows = dt.Select(where);
                if (rows.Length == 0)
                {
                    row = dt.NewRow();
                    row[0] = ds.Tables[0].Rows[i][0];
                    row[1] = ds.Tables[0].Rows[i][1];
                    row[2] = ds.Tables[0].Rows[i][2];
                    row[3] = ds.Tables[0].Rows[i][3];
                    row[5] = ds.Tables[0].Rows[i][9];
                    row[6] = ds.Tables[0].Rows[i][10];
                    row[7] = ds.Tables[0].Rows[i][11];
                    row[8] = ds.Tables[0].Rows[i][12];
                    row[9] = ds.Tables[0].Rows[i][13];
                    row[4] = ds.Tables[0].Rows[i][8];
                    productIDList.Clear();
                    operationIDList.Clear();
                    operationNameList.Clear();
                    rows = ds.Tables[0].Select(where);
                    for (int j = 0; j < rows.Length; j++)
                    {
                        if (!productIDList.Contains(rows[j][4]))
                        {
                            productIDList.Add(rows[j][4]);
                            operationIDList.Add(rows[j][5].ToString());
                            operationNameList.Add(rows[j][15].ToString().Trim());
                        }
                    }
                    rows =
                        ds.Tables[0].Select(where + " AND bookingY='" + bookingY + "' AND DeliverY='" + deliverY + "'");
                    if (rows.Length == 0)
                    {
                        index = 0;
                        for (int j = 0; j < dsPro.Tables[0].Rows.Count; j++)
                        {
                            if (productIDList.Contains(dsPro.Tables[0].Rows[j][0]))
                            {
                                row[10 + j*3 + 1] = "0";
                                row[10 + j*3 + 2] = operationNameList[index];
                                row[10 + j*3 + 3] = string.Empty;
                                index++;
                            }
                            else
                            {
                                row[10 + j*3 + 1] = "0";
                                row[10 + j*3 + 2] = string.Empty;
                                row[10 + j*3 + 3] = string.Empty;
                            }
                        }
                    }
                    else
                    {
                        index = 0;
                        for (int j = 0; j < dsPro.Tables[0].Rows.Count; j++)
                        {
                            //Edit by sino bug35、36、38 begin

                            if (index >= rows.Length)
                            {
                                break;
                            }
                            //Edit by sino bug35、36、38 end

                            if (productIDList.Contains(dsPro.Tables[0].Rows[j][0]))
                            {
                                if (convertFlag)
                                {
                                    //if (((string.Equals(month, "3") || string.Equals(month, "03")) && string.Equals(bookingY, year.Substring(2)))
                                    //    || string.Equals(deliverY, year.Substring(2)) || string.Equals(deliverY, "YTD"))
                                    if (((string.Equals(month, "3") || string.Equals(month, "03")) &&
                                         string.Equals(bookingY, year.Substring(2)))
                                        || string.Equals(deliverY, "YTD"))
                                    {
                                        row[10 + j*3 + 1] = Math.Round(Convert.ToDouble(rows[index][16])*db_rate1, 0);
                                    }
                                    else
                                    {
                                        row[10 + j*3 + 1] = Math.Round(Convert.ToDouble(rows[index][16])*db_rate2, 0);
                                    }
                                }
                                else
                                {
                                    row[10 + j*3 + 1] = Math.Round(Convert.ToDouble(rows[index][16]), 0);
                                }
                                row[10 + j*3 + 2] = rows[index][15];
                                row[10 + j*3 + 3] = rows[index][19];
                                index++;
                            }
                            else
                            {
                                row[10 + j*3 + 1] = "0";
                                row[10 + j*3 + 2] = string.Empty;
                                row[10 + j*3 + 3] = string.Empty;
                            }
                        }
                    }
                    dt.Rows.Add(row);
                    sum = 0;
                    for (int j = 11; j < dt.Columns.Count; j += 3)
                    {
                        //    sum += Convert.ToInt32(row[j]);
                        //Edit by sino bug35、36、38 begin
                        if (row[j].ToString() != null && row[j].ToString() != "")
                        {
                            sum += Convert.ToInt32(row[j]);
                        }
                        //Edit by sino bug35、36、38 end
                    }
                    row[10] = sum;
                }
            }
            ds.Tables.Clear();
            ds.Tables.Add(dt);
        }
        else
        {
            return null;
        }
        return ds;
    }

    /// <summary>
    /// Get this year booking sales data.
    /// </summary>
    /// <param name="sql">SQL for getting booking data </param>
    /// <param name="dsPro">DataSet of product</param>
    /// <param name="salesOrgID">Sales Organization ID</param>
    /// <param name="year">Year</param>
    /// <param name="month">Month</param>
    /// <param name="convertFlag">true for converting to Euro, false for not converting</param>
    /// <returns>DataSet</returns>
    public DataSet getBookingSalesDataThisYear(string sql, DataSet dsPro,
                                               string salesOrgID, string year, string month, bool convertFlag)
    {
        DataSet ds = helper.GetDataSet(sql);
        if (ds != null && ds.Tables.Count > 0)
        {
            ds.Tables[0].Columns.RemoveAt(8);
            DataTable dt = ds.Tables[0].Clone();
            //dt.Columns.RemoveAt(8); //special for this total.
            dt.Columns.RemoveAt(4);
            dt.Columns.RemoveAt(4);
            dt.Columns.RemoveAt(4);
            dt.Columns.RemoveAt(4);
            dt.Columns.RemoveAt(dt.Columns.Count - 1);
            dt.Columns.RemoveAt(dt.Columns.Count - 1);
            dt.Columns.RemoveAt(dt.Columns.Count - 1);
            dt.Columns.RemoveAt(dt.Columns.Count - 1);
            dt.Columns.RemoveAt(dt.Columns.Count - 1);
            dt.Columns.RemoveAt(dt.Columns.Count - 1);
            for (int i = 0; i < dsPro.Tables[0].Rows.Count; i++)
            {
                dt.Columns.Add(dsPro.Tables[0].Rows[i][1].ToString());
                dt.Columns.Add("OperationAbbr" + i);
            }
            DataRow[] rows = null;
            var sumRows = new List<DataRow>();
            DataRow row = null;
            DataRow sumRow = null;
            var sumDt = new DataTable();
            bool notExistFlag = true;
            string where = null;
            int index = 0;
            var productIDList = new List<object>();
            var operationNameList = new List<object>();
            string currency = getSalesOrgCurrency(salesOrgID);
            double db_rate1 = getRate(currency, true, year, month);
            double db_rate2 = getRate(currency, false, year, month);
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                where = "SubRegionID=" + ds.Tables[0].Rows[i][0] + " AND CustomerID=" + ds.Tables[0].Rows[i][1]
                        + " AND ProjectID=" + ds.Tables[0].Rows[i][2] + " AND SalesChannelID=" + ds.Tables[0].Rows[i][3];
                rows = dt.Select(where);
                if (rows.Length == 0)
                {
                    row = dt.NewRow();
                    row[0] = ds.Tables[0].Rows[i][0];
                    row[1] = ds.Tables[0].Rows[i][1];
                    row[2] = ds.Tables[0].Rows[i][2];
                    row[3] = ds.Tables[0].Rows[i][3];
                    row[4] = ds.Tables[0].Rows[i][8];
                    row[5] = ds.Tables[0].Rows[i][9];
                    row[6] = ds.Tables[0].Rows[i][10];
                    row[7] = ds.Tables[0].Rows[i][11];
                    row[8] = ds.Tables[0].Rows[i][12];
                    productIDList.Clear();
                    operationNameList.Clear();
                    rows = ds.Tables[0].Select(where);
                    for (int j = 0; j < rows.Length; j++)
                    {
                        if (!productIDList.Contains(rows[j][4]))
                        {
                            productIDList.Add(rows[j][4]);
                            operationNameList.Add(rows[j][14].ToString().Trim());
                        }
                    }
                    rows = ds.Tables[0].Select(where + " AND bookingY='" + year.Substring(2) + "'");
                    //rows = ds.Tables[0].Select(where + " AND bookingY='" + year.Substring(2) + "' and DeliverY='YTD'");
                    if (rows.Length == 0)
                    {
                        index = 0;
                        for (int j = 0; j < dsPro.Tables[0].Rows.Count; j++)
                        {
                            if (productIDList.Contains(dsPro.Tables[0].Rows[j][0]))
                            {
                                row[8 + j*2 + 1] = "0";
                                row[8 + j*2 + 2] = operationNameList[index];
                                index++;
                            }
                            else
                            {
                                row[8 + j*2 + 1] = "0";
                                row[8 + j*2 + 2] = string.Empty;
                            }
                        }
                    }
                    else
                    {
                        sumRows.Clear();
                        for (int j = 0; j < rows.Length; j++)
                        {
                            sumRow = ds.Tables[0].NewRow();
                            for (int n = 0; n <= 18; n++)
                            {
                                sumRow[n] = n == 15 ? 0 : rows[j][n];
                            }
                            notExistFlag = true;
                            for (int k = 0; k < sumRows.Count; k++)
                            {
                                if (string.Equals(rows[j][0].ToString(), rows[k][0].ToString())
                                    && string.Equals(rows[j][1].ToString(), rows[k][1].ToString())
                                    && string.Equals(rows[j][2].ToString(), rows[k][2].ToString())
                                    && string.Equals(rows[j][3].ToString(), rows[k][3].ToString())
                                    && string.Equals(rows[j][4].ToString(), rows[k][4].ToString()))
                                {
                                    notExistFlag = false;
                                    break;
                                }
                            }
                            if (notExistFlag)
                            {
                                sumRows.Add(sumRow);
                            }
                        }
                        for (int j = 0; j < sumRows.Count; j++)
                        {
                            for (int k = 0; k < rows.Length; k++)
                            {
                                if (string.Equals(rows[j][0].ToString(), rows[k][0].ToString())
                                    && string.Equals(rows[j][1].ToString(), rows[k][1].ToString())
                                    && string.Equals(rows[j][2].ToString(), rows[k][2].ToString())
                                    && string.Equals(rows[j][3].ToString(), rows[k][3].ToString())
                                    && string.Equals(rows[j][4].ToString(), rows[k][4].ToString()))
                                {
                                    if (convertFlag)
                                        sumRows[j][15] = Convert.ToDouble(sumRows[j][15]) +
                                                         Convert.ToDouble(rows[k][15])*
                                                         (rows[k][7].ToString().Trim() == "YTD" ? db_rate1 : db_rate2);
                                    else
                                        sumRows[j][15] = Convert.ToDouble(sumRows[j][15]) +
                                                         Convert.ToDouble(rows[k][15]);
                                }
                            }
                        }
                        rows = sumRows.ToArray();
                        index = 0;
                        for (int j = 0; j < dsPro.Tables[0].Rows.Count; j++)
                        {
                            if (productIDList.Contains(dsPro.Tables[0].Rows[j][0]))
                            {
                                //if (convertFlag)
                                //{
                                //    row[8 + j * 2 + 1] = Math.Round(Convert.ToDouble(rows[index][15]) * db_rate2, 0);
                                //}
                                //else
                                //{
                                row[8 + j*2 + 1] = Math.Round(Convert.ToDouble(rows[index][15]), 0);
                                //}
                                row[8 + j*2 + 2] = rows[index][14];
                                index++;
                            }
                            else
                            {
                                row[8 + j*2 + 1] = "0";
                                row[8 + j*2 + 2] = string.Empty;
                            }
                        }
                    }
                    dt.Rows.Add(row);
                }
            }
            ds.Tables.Clear();
            ds.Tables.Add(dt);
        }
        else
        {
            return null;
        }
        return ds;
    }

    /// <summary>
    /// Get next year booking sales data
    /// </summary>
    /// <param name="sql">SQL for getting booking sales data.</param>
    /// <param name="dsPro">DataSet of product</param>
    /// <param name="salesOrgID">Sales Organization ID</param>
    /// <param name="year">Year</param>
    /// <param name="month">Month</param>
    /// <param name="nextyear">Next year</param>
    /// <param name="convertFlag">true for converting to Euro, false for not converting</param>
    /// <returns>DataSet</returns>
    public DataSet getBookingSalesDataNextYear(string sql, DataSet dsPro,
                                               string salesOrgID, string year, string month, string nextyear,
                                               bool convertFlag)
    {
        DataSet ds = helper.GetDataSet(sql);
        if (ds != null && ds.Tables.Count > 0)
        {
            ds.Tables[0].Columns.RemoveAt(8);
            DataTable dt = ds.Tables[0].Clone();
            dt.Columns.RemoveAt(4);
            dt.Columns.RemoveAt(4);
            dt.Columns.RemoveAt(4);
            dt.Columns.RemoveAt(4);
            dt.Columns.RemoveAt(dt.Columns.Count - 1);
            dt.Columns.RemoveAt(dt.Columns.Count - 1);
            dt.Columns.RemoveAt(dt.Columns.Count - 1);
            dt.Columns.RemoveAt(dt.Columns.Count - 1);
            dt.Columns.RemoveAt(dt.Columns.Count - 1);
            dt.Columns.RemoveAt(dt.Columns.Count - 1);
            for (int i = 0; i < dsPro.Tables[0].Rows.Count; i++)
            {
                dt.Columns.Add(dsPro.Tables[0].Rows[i][1].ToString());
                dt.Columns.Add("OperationAbbr" + i);
            }
            DataRow[] rows = null;
            var sumRows = new List<DataRow>();
            DataRow row = null;
            DataRow sumRow = null;
            var sumDt = new DataTable();
            bool notExistFlag = true;
            string where = null;
            int index = 0;
            var productIDList = new List<object>();
            var operationNameList = new List<object>();
            string currency = getSalesOrgCurrency(salesOrgID);
            double db_rate1 = getRate(currency, true, year, month);
            double db_rate2 = getRate(currency, false, year, month);
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                where = "SubRegionID=" + ds.Tables[0].Rows[i][0] + " AND CustomerID=" + ds.Tables[0].Rows[i][1]
                        + " AND ProjectID=" + ds.Tables[0].Rows[i][2] + " AND SalesChannelID=" + ds.Tables[0].Rows[i][3];
                rows = dt.Select(where);
                if (rows.Length == 0)
                {
                    row = dt.NewRow();
                    row[0] = ds.Tables[0].Rows[i][0];
                    row[1] = ds.Tables[0].Rows[i][1];
                    row[2] = ds.Tables[0].Rows[i][2];
                    row[3] = ds.Tables[0].Rows[i][3];
                    row[4] = ds.Tables[0].Rows[i][8];
                    row[5] = ds.Tables[0].Rows[i][9];
                    row[6] = ds.Tables[0].Rows[i][10];
                    row[7] = ds.Tables[0].Rows[i][11];
                    row[8] = ds.Tables[0].Rows[i][12];
                    productIDList.Clear();
                    operationNameList.Clear();
                    rows = ds.Tables[0].Select(where);
                    for (int j = 0; j < rows.Length; j++)
                    {
                        if (!productIDList.Contains(rows[j][4]))
                        {
                            productIDList.Add(rows[j][4]);
                            operationNameList.Add(rows[j][14].ToString().Trim());
                        }
                    }
                    rows = ds.Tables[0].Select(where + " AND bookingY='" + nextyear.Substring(2) + "'");
                    if (rows.Length == 0)
                    {
                        index = 0;
                        for (int j = 0; j < dsPro.Tables[0].Rows.Count; j++)
                        {
                            if (productIDList.Contains(dsPro.Tables[0].Rows[j][0]))
                            {
                                row[8 + j*2 + 1] = "0";
                                row[8 + j*2 + 2] = operationNameList[index];
                                index++;
                            }
                            else
                            {
                                row[8 + j*2 + 1] = "0";
                                row[8 + j*2 + 2] = string.Empty;
                            }
                        }
                    }
                    else
                    {
                        sumRows.Clear();
                        for (int j = 0; j < rows.Length; j++)
                        {
                            sumRow = ds.Tables[0].NewRow();
                            for (int n = 0; n <= 18; n++)
                            {
                                sumRow[n] = n == 15 ? 0 : rows[j][n];
                            }
                            notExistFlag = true;
                            for (int k = 0; k < sumRows.Count; k++)
                            {
                                if (string.Equals(rows[j][0].ToString(), rows[k][0].ToString())
                                    && string.Equals(rows[j][1].ToString(), rows[k][1].ToString())
                                    && string.Equals(rows[j][2].ToString(), rows[k][2].ToString())
                                    && string.Equals(rows[j][3].ToString(), rows[k][3].ToString())
                                    && string.Equals(rows[j][4].ToString(), rows[k][4].ToString()))
                                {
                                    notExistFlag = false;
                                    break;
                                }
                            }
                            if (notExistFlag)
                            {
                                sumRows.Add(sumRow);
                            }
                        }
                        for (int j = 0; j < sumRows.Count; j++)
                        {
                            for (int k = 0; k < rows.Length; k++)
                            {
                                if (string.Equals(rows[j][0].ToString(), rows[k][0].ToString())
                                    && string.Equals(rows[j][1].ToString(), rows[k][1].ToString())
                                    && string.Equals(rows[j][2].ToString(), rows[k][2].ToString())
                                    && string.Equals(rows[j][3].ToString(), rows[k][3].ToString())
                                    && string.Equals(rows[j][4].ToString(), rows[k][4].ToString()))
                                {
                                    sumRows[j][15] = Convert.ToDouble(sumRows[j][15]) + Convert.ToDouble(rows[k][15]);
                                }
                            }
                        }
                        rows = sumRows.ToArray();
                        index = 0;
                        for (int j = 0; j < dsPro.Tables[0].Rows.Count; j++)
                        {
                            if (productIDList.Contains(dsPro.Tables[0].Rows[j][0]))
                            {
                                if (convertFlag)
                                {
                                    row[8 + j*2 + 1] = Math.Round(Convert.ToDouble(rows[index][15])*db_rate2, 0);
                                }
                                else
                                {
                                    row[8 + j*2 + 1] = Math.Round(Convert.ToDouble(rows[index][15]), 0);
                                }
                                row[8 + j*2 + 2] = rows[index][14];
                                index++;
                            }
                            else
                            {
                                row[8 + j*2 + 1] = "0";
                                row[8 + j*2 + 2] = string.Empty;
                            }
                        }
                    }
                    dt.Rows.Add(row);
                }
            }
            ds.Tables.Clear();
            ds.Tables.Add(dt);
        }
        else
        {
            return null;
        }
        return ds;
    }

    /// <summary>
    /// Get booking sales data VS Year
    /// </summary>
    /// <param name="sql">SQL for getting booking sales data</param>
    /// <param name="dsPro">DataSet of products</param>
    /// <param name="salesOrgID">Sales Organization ID</param>
    /// <param name="year">Year</param>
    /// <param name="month">Month</param>
    /// <param name="convertFlag">true for converting to Euro, false for not converting</param>
    /// <returns>DataSet</returns>
    public DataSet getBookingSalesDataThisVSThisYear(string sql, DataSet dsPro,
                                                     string salesOrgID, string year, string month, bool convertFlag)
    {
        DataSet ds = helper.GetDataSet(sql);
        if (ds != null && ds.Tables.Count > 0)
        {
            ds.Tables[0].Columns.RemoveAt(8);
            DataTable dt = ds.Tables[0].Clone();
            dt.Columns.RemoveAt(4);
            dt.Columns.RemoveAt(4);
            dt.Columns.RemoveAt(4);
            dt.Columns.RemoveAt(4);
            dt.Columns.RemoveAt(dt.Columns.Count - 1);
            dt.Columns.RemoveAt(dt.Columns.Count - 1);
            dt.Columns.RemoveAt(dt.Columns.Count - 1);
            dt.Columns.RemoveAt(dt.Columns.Count - 1);
            dt.Columns.RemoveAt(dt.Columns.Count - 1);
            dt.Columns.RemoveAt(dt.Columns.Count - 1);
            for (int i = 0; i < dsPro.Tables[0].Rows.Count; i++)
            {
                dt.Columns.Add(dsPro.Tables[0].Rows[i][1].ToString());
            }
            DataRow[] rows = null;
            var sumRows = new List<DataRow>();
            DataRow row = null;
            DataRow sumRow = null;
            var sumDt = new DataTable();
            bool notExistFlag = true;
            string where = null;
            int index = 0;
            var productIDList = new List<object>();
            string currency = getSalesOrgCurrency(salesOrgID);
            double db_rate1 = getRate(currency, true, year, month);
            double db_rate2 = getRate(currency, false, year, month);
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                where = "SubRegionID=" + ds.Tables[0].Rows[i][0] + " AND CustomerID=" + ds.Tables[0].Rows[i][1]
                        + " AND ProjectID=" + ds.Tables[0].Rows[i][2] + " AND SalesChannelID=" + ds.Tables[0].Rows[i][3];
                rows = dt.Select(where);
                if (rows.Length == 0)
                {
                    row = dt.NewRow();
                    row[0] = ds.Tables[0].Rows[i][0];
                    row[1] = ds.Tables[0].Rows[i][1];
                    row[2] = ds.Tables[0].Rows[i][2];
                    row[3] = ds.Tables[0].Rows[i][3];
                    row[4] = ds.Tables[0].Rows[i][8];
                    row[5] = ds.Tables[0].Rows[i][9];
                    row[6] = ds.Tables[0].Rows[i][10];
                    row[7] = ds.Tables[0].Rows[i][11];
                    row[8] = ds.Tables[0].Rows[i][12];
                    productIDList.Clear();
                    rows = ds.Tables[0].Select(where);
                    for (int j = 0; j < rows.Length; j++)
                    {
                        if (!productIDList.Contains(rows[j][4]))
                        {
                            productIDList.Add(rows[j][4]);
                        }
                    }
                    rows = ds.Tables[0].Select(where + " AND bookingY='" + year.Substring(2) + "'");
                    if (rows.Length == 0)
                    {
                        for (int j = 0; j < dsPro.Tables[0].Rows.Count; j++)
                        {
                            row[8 + j + 1] = "0";
                        }
                    }
                    else
                    {
                        sumRows.Clear();
                        for (int j = 0; j < rows.Length; j++)
                        {
                            sumRow = ds.Tables[0].NewRow();
                            for (int n = 0; n <= 18; n++)
                            {
                                sumRow[n] = n == 15 ? 0 : rows[j][n];
                            }
                            notExistFlag = true;
                            for (int k = 0; k < sumRows.Count; k++)
                            {
                                if (string.Equals(rows[j][0].ToString(), rows[k][0].ToString())
                                    && string.Equals(rows[j][1].ToString(), rows[k][1].ToString())
                                    && string.Equals(rows[j][2].ToString(), rows[k][2].ToString())
                                    && string.Equals(rows[j][3].ToString(), rows[k][3].ToString())
                                    && string.Equals(rows[j][4].ToString(), rows[k][4].ToString()))
                                {
                                    notExistFlag = false;
                                    break;
                                }
                            }
                            if (notExistFlag)
                            {
                                sumRows.Add(sumRow);
                            }
                        }
                        for (int j = 0; j < sumRows.Count; j++)
                        {
                            for (int k = 0; k < rows.Length; k++)
                            {
                                if (string.Equals(rows[j][0].ToString(), rows[k][0].ToString())
                                    && string.Equals(rows[j][1].ToString(), rows[k][1].ToString())
                                    && string.Equals(rows[j][2].ToString(), rows[k][2].ToString())
                                    && string.Equals(rows[j][3].ToString(), rows[k][3].ToString())
                                    && string.Equals(rows[j][4].ToString(), rows[k][4].ToString()))
                                {
                                    sumRows[j][15] = Convert.ToDouble(sumRows[j][15]) + Convert.ToDouble(rows[k][15]);
                                }
                            }
                        }
                        rows = sumRows.ToArray();
                        index = 0;
                        for (int j = 0; j < dsPro.Tables[0].Rows.Count; j++)
                        {
                            if (productIDList.Contains(dsPro.Tables[0].Rows[j][0]))
                            {
                                if (convertFlag)
                                {
                                    if (string.Equals(rows[index][7].ToString().Trim(), "YTD")
                                        || string.Equals(rows[index][7].ToString().Trim(), year.Substring(2)))
                                    {
                                        row[8 + j + 1] = Math.Round(Convert.ToDouble(rows[index][15])*db_rate1, 0);
                                    }
                                    else
                                    {
                                        row[8 + j + 1] = Math.Round(Convert.ToDouble(rows[index][15])*db_rate2, 0);
                                    }
                                }
                                else
                                {
                                    row[8 + j + 1] = Math.Round(Convert.ToDouble(rows[index][15]), 0);
                                }
                                index++;
                            }
                            else
                            {
                                row[8 + j + 1] = "0";
                            }
                        }
                    }
                    dt.Rows.Add(row);
                }
            }
            ds.Tables.Clear();
            ds.Tables.Add(dt);
        }
        else
        {
            return null;
        }
        return ds;
    }

    

    /// <summary>
    /// Get Booking Sales Data ThisYear VS PreYear
    /// </summary>
    /// <param name="yearSQL">sql for Getting booking sales data by current meeting date</param>
    /// <param name="yearVSSQL">sql for Get booking sales data by pre meeting date</param>
    /// <param name="preYearVSSQL">sql for Get booking sales data by pre meeting date pre year.</param>
    /// <param name="dsPro">DataSet of product</param>
    /// <param name="salesOrgID">Sales Organization ID</param>
    /// <param name="year">Year</param>
    /// <param name="month">Month</param>
    /// <param name="preYear">Preyear</param>
    /// <param name="preMonth">Premonth</param>
    /// <param name="convertFlag">true for converting to Euro, false for not converting</param>
    /// <returns>DataSet</returns>
    public DataSet getBookingSalesDataThisYearVSPreYear(string yearSQL, string yearVSSQL, string preYearVSSQL,
                                                        DataSet dsPro, string salesOrgID, string year, string month,
                                                        string preYear, string preMonth, bool convertFlag)
    {
        DataSet yearDs = getBookingSalesDataThisVSThisYear(yearSQL, dsPro, salesOrgID, year, month, convertFlag);
        if (yearDs == null || yearDs.Tables.Count == 0 || yearDs.Tables[0].Rows.Count == 0)
        {
            return null;
        }
        else
        {
            DataSet yearVS = null;
            DataRow[] rows = null;
            if (meeting.JudgeFirstMonth(month))
            {
                yearVS = getBookingSalesDataThisVSThisYear(preYearVSSQL, dsPro, salesOrgID, preYear, preMonth,
                                                           convertFlag);
            }
            else
            {
                yearVS = getBookingSalesDataThisVSThisYear(yearVSSQL, dsPro, salesOrgID, year, preMonth, convertFlag);
            }
            for (int i = 0; i < yearDs.Tables[0].Rows.Count; i++)
            {
                rows =
                    yearVS.Tables[0].Select("SubRegionID=" + yearDs.Tables[0].Rows[i][0] + " AND CustomerID=" +
                                            yearDs.Tables[0].Rows[i][1]
                                            + " AND ProjectID=" + yearDs.Tables[0].Rows[i][2] + " AND SalesChannelID=" +
                                            yearDs.Tables[0].Rows[i][3]);
                if (rows.Length != 0)
                {
                    for (int j = 9; j < yearDs.Tables[0].Columns.Count; j++)
                    {
                        yearDs.Tables[0].Rows[i][j] = rows[0][j];
                    }
                }
                else
                {
                    for (int j = 9; j < yearDs.Tables[0].Columns.Count; j++)
                    {
                        yearDs.Tables[0].Rows[i][j] = 0;
                    }
                }
            }
            return yearDs;
        }
    }

    #endregion

    //Edit by Forquan at 2011-12-29 End
}