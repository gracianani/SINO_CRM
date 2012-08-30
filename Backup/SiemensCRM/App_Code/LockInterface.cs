/*
 * File Name    : LockInterface.cs
 * 
 * Description  : Lock
 * 
 * Author       : mbq
 * 
 * Modify Date : 2010-01-24
 * 
 * Problem     : 
 * 
 * Version     : Release(2.0)
 */

using System.Data;

/// <summary>
/// related to Lock function.
/// </summary>
public class LockInterface
{
    private readonly SQLHelper helper;

    public LockInterface()
    {
        helper = new SQLHelper();
    }

    /// <summary>
    /// check if user is locked.
    /// </summary>
    /// <param name="userID">user id</param>
    /// <returns>check result</returns>
    public bool getLockUserData(string userID)
    {
        string sql = "select UserID from [Lock] where UnlockTime>=GETDATE() and [Lock].UserID = '" + userID + "'"
                     + " union select UserID from [LockAllUser] where [LockAllUser].UserID =  '" + userID + "'";

        DataSet ds = helper.GetDataSet(sql);

        if (ds.Tables[0].Rows.Count >= 1)
            return true;
        else
            return false;
    }

    /// <summary>
    /// check if segment is locked.
    /// </summary>
    /// <param name="segmentID">segment id</param>
    /// <returns>check result</returns>
    public bool getLockSegmentData(string segmentID)
    {
        string sql = "select SegmentID from [LockSegment] where [LockSegment].SegmentID = '" + segmentID + "'";
        DataSet ds = helper.GetDataSet(sql);
        if (ds.Tables[0].Rows.Count >= 1)
            return true;
        else
            return false;
    }
}