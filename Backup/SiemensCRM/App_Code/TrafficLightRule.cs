using System;
using System.Data;

/// <summary>
/// logic related to traffic light.
/// </summary>
public class TrafficLightRule
{
    private static readonly SQLHelper helper = new SQLHelper();

    /// <summary>
    /// check if system is locked
    /// </summary>
    /// <returns>check result</returns>
    public static bool IsLockAll()
    {
        string sql = "select * from [LockAllUser] where UnlockTime>=GETDATE()";

        DataSet ds = helper.GetDataSet(sql);

        if (ds.Tables[0].Rows.Count >= 1)
            return true;
        else
            return false;
    }

    private static bool IsLockSegment(int segmentId)
    {
        string sql = string.Format("select * from [LockSegment] where UnlockTime>=GETDATE() and SegmentID={0}",
                                   segmentId);

        DataSet ds = helper.GetDataSet(sql);

        if (ds.Tables[0].Rows.Count >= 1)
            return true;
        else
            return false;
    }

    private static bool IsLockUser(int userId)
    {
        string sql = string.Format("SELECT * from [Lock] where UnlockTime>=GETDATE() and UserID={0}", userId);
        DataSet ds = helper.GetDataSet(sql);

        if (ds.Tables[0].Rows.Count >= 1)
            return true;
        else
            return false;
    }

    /// <summary>
    /// check if user is locked
    /// </summary>
    /// <param name="userId">user id</param>
    /// <param name="segmentId">segment id</param>
    /// <returns>check result</returns>
    public static bool IsLock(int userId, int segmentId)
    {
        if (IsLockAll())
            return true;
        if (IsLockSegment(segmentId))
            return true;
        if (IsLockUser(userId))
            return true;

        return false;
    }


    /// <summary>
    /// get user status
    /// </summary>
    /// <param name="str_userID">user id</param>
    /// <param name="str_segmentID">segment id</param>
    /// <returns>user status</returns>
    public static string GetUserStatus(string str_userID, string str_segmentID)
    {
        string sql = "SELECT status FROM [User_Status] WHERE UserID = '" + str_userID + "' AND SegmentID = '" +
                     str_segmentID + "'";
        DataSet ds = helper.GetDataSet(sql);

        if (ds.Tables[0].Rows.Count == 0)
            return "";
        else
            return ds.Tables[0].Rows[0][0].ToString().Trim();
    }

    /// <summary>
    /// insert default status
    /// </summary>
    /// <param name="str_userID">user id</param>
    /// <param name="str_segmentID">segment id</param>
    /// <returns>default status</returns>
    public static string InsertDefaultUserStatus(string str_userID, string str_segmentID)
    {
        string roleId = string.Empty;
        string str_status = string.Empty;
        string roleSql = "SELECT RoleID FROM [User] where [UserID]=" + str_userID;
        DataSet ds = helper.GetDataSet(roleSql);
        if (ds == null || ds.Tables[0].Rows.Count < 1)
            return string.Empty;
        else
        {
            roleId = ds.Tables[0].Rows[0][0].ToString().Trim();
        }
        if (roleId == "4")
            str_status = "R";
        else if (roleId == "3")
            str_status = "Y";
        else
            return string.Empty;
        string sql = "INSERT INTO [User_Status](UserID, SegmentID, Status) VALUES('" + str_userID + "','" +
                     str_segmentID + "','" + str_status + "')";
        helper.ExecuteNonQuery(CommandType.Text, sql.Trim(), null);
        return str_status;
    }

    /// <summary>
    /// update user status
    /// </summary>
    /// <param name="str_userID">user id</param>
    /// <param name="str_segmentID">segment id</param>
    /// <param name="str_status">user status</param>
    /// <returns></returns>
    public static bool UpdateUserStatus(string str_userID, string str_segmentID, string str_status)
    {
        string sql = "UPDATE [User_Status] SET Status = '" + str_status + "' WHERE UserID = '" + str_userID +
                     "' AND SegmentID = '" + str_segmentID + "'";
        int count = helper.ExecuteNonQuery(CommandType.Text, sql.Trim(), null);
        if (count == 1)
            return true;
        else
            return false;
    }

    /// <summary>
    /// set user defult status
    /// </summary>
    /// <param name="marketingMgrID">user id</param>
    /// <param name="segmentID">segment id</param>
    /// <param name="operationID">operation id</param>
    /// <param name="salesOrgID">salesorg id</param>
    /// <returns>default status</returns>
    public static string SetDefaultBLStatus(String marketingMgrID, String segmentID, String operationID,
                                            String salesOrgID)
    {
        string roleId = string.Empty;
        string status = "R";
        string roleSql = "SELECT RoleID FROM [User] where [RoleID]=2 and [UserID]=" + marketingMgrID;
        DataSet ds = helper.GetDataSet(roleSql);
        if (ds == null || ds.Tables[0].Rows.Count < 1)
            return string.Empty;

        string sql =
            String.Format(
                @"INSERT INTO [ActualSalesandBL_Status] (MarketingMgrID,SegmentID,OperationID,SalesOrgID,[Status]) VALUES ({0},{1},{2},{3},'{4}')"
                , marketingMgrID, segmentID, operationID, salesOrgID, status);
        helper.ExecuteNonQuery(CommandType.Text, sql.Trim(), null);
        return status;
    }
}