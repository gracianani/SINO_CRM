using System;
using System.Data;
using System.Text;
using Resources;

/// <summary>
/// sql statement for query individual report result.
/// </summary>
public class ReportCompileToSQL
{
    private readonly SQLHelper helper = new SQLHelper();

   

    /// <summary>
    /// get sql script for query report result related to BookingsSales view.
    /// </summary>
    /// <param name="RepID">report id</param>
    /// <param name="RSMID">user id</param>
    /// <param name="CountryID">country id</param>
    /// <param name="CustomerID">customer id</param>
    /// <param name="orderby">order condition</param>
    /// <param name="SelectMeetingDate">select meeting date</param>
    /// <returns>sql script</returns>
    public string RepCompileSql(string RepID, string RSMID, string CountryID, string CustomerID, string orderby,
                                string SelectMeetingDate)
        
    {
        try
        {
            string selrep = string.Format("select * from  ReportList WHERE ID={0}", RepID);
            // update by zy 20110106 start
            //string selrepv = string.Format("select * from  ReportValue WHERE ID={0}", RepID);
            string selrepv = string.Format("select * from  ReportValue WHERE ID={0} ORDER BY FieldOrder", RepID);
            // update by zy 20110106 end
            DataTable tabrep = helper.GetDataSet(selrep).Tables[0];
            DataTable tabrepv = helper.GetDataSet(selrepv).Tables[0];
            if (tabrep != null && selrepv != null)
            {
                if (tabrep.Rows.Count > 0 && tabrepv.Rows.Count > 0)
                {
                    string viewname = tabrep.Rows[0]["ViewName"].ToString();
                    string fieldname = getFieldNames(tabrepv);
                    //BY yyan 20110513 item 53 add start
                    string newFieldname = getNewFieldNames(tabrepv);
                    //BY yyan 20110513 item 53 add start
                    string wherename = getWhereNames(tabrepv);
                    // add by zy 20110106 start
                    string sqlgroup = "";
                    //BY yyan 20110524 item W10 del start
                    //if (fieldname.Contains("Amount"))
                    //BY yyan 20110524 item W10 del end
                    //BY yyan 20110524 item W10 add start
                    if (fieldname.Contains(Resource.GAPrice) || fieldname.Contains(Resource.AmountEUR))
                        //BY yyan 20110524 item W10 add end
                    {
                        //BY yyan 20110524 item W10 del start
                        //sqlgroup = fieldname.Replace(",Amount", "");
                        //BY yyan 20110524 item W10 del end
                        //BY yyan 20110524 item W10 add start
                        sqlgroup = fieldname.Replace(",AmountEUR", "");
                        sqlgroup = sqlgroup.Replace("AmountEUR,", "");
                        fieldname = fieldname.Replace("AmountEUR", "changeEUR");
                        sqlgroup = sqlgroup.Replace(",GAPrice", "");
                        //BY yyan 20110524 item W10 add end
                        sqlgroup = sqlgroup.Replace("GAPrice,", "");
                        //yyan 20110909 itemw140 add start
                        sqlgroup = sqlgroup.Replace(",KPrice", "");
                        sqlgroup = sqlgroup.Replace("KPrice,", "");
                        fieldname = fieldname.Replace("KPrice", "ROUND(SUM(KPrice)~0)");
                        //yyan 20110909 itemw140 add start

                        //BY yyan 20110513 item 53 del start
                        //fieldname = fieldname.Replace("Amount", "ROUND(SUM(Amount), 0) AS Amount");
                        //BY yyan 20110513 item 53 del start

                        //BY yyan 20110513 item 53 add start
                        fieldname = fieldname.Replace("GAPrice", "ROUND(SUM(GAPrice)~0)");
                        //BY yyan 20110513 item 53 add end

                        //BY yyan 20110524 item W10 add start
                        fieldname = fieldname.Replace("changeEUR", "ROUND(SUM(AmountEUR)~0)");
                        //BY yyan 20110524 item W10 add end
                    }
                    else
                    {
                        sqlgroup = fieldname;
                    }

                    //BY yyan 20110513 item 53 add start
                    if (fieldname.Contains(","))
                    {
                        string[] strFieldNames = fieldname.Split(',');
                        string[] strNewFieldNames = newFieldname.Split(',');
                        string strInfo = "";
                        for (int i = 0; i < strFieldNames.Length; i++)
                        {
                            strInfo = strInfo + strFieldNames[i] + "#AS#" + strNewFieldNames[i] + ",";
                        }
                        strInfo = strInfo.Substring(0, strInfo.Length - 1);
                        strInfo = strInfo.Replace('#', ' ');
                        fieldname = strInfo;
                    }
                    else
                    {
                        fieldname = fieldname + " AS " + newFieldname;
                    }
                    fieldname = fieldname.Replace('~', ',');
                    //BY yyan 20110513 item 53 add end


                    // add by zy 20110106 end
                    string ordername = getSortNames(tabrepv);
                    string sqlselect = string.Format("select {0} from {1}", fieldname, viewname);

                    string sqlrsmid = "";
                    bool rsmFlag = false;
                    if (!string.IsNullOrEmpty(RSMID))
                    {
                        //yyan itemw124 20110829 add start
                        //sqlrsmid = string.Format(" and RSMID = '{0}'", RSMID);
                        string sqlGsm = "select roleid from [user] where userid=" + RSMID;
                        DataTable tabGsm = helper.GetDataSet(sqlGsm).Tables[0];
                        if (tabGsm.Rows[0][0].ToString() == "3")
                        {
                            rsmFlag = true;
                            sqlGsm = "select userid from [user] where roleid=4";
                            DataTable tabGsmCount = helper.GetDataSet(sqlGsm).Tables[0];
                            string strGsm = "";
                            for (int k = 0; k < tabGsmCount.Rows.Count; k++)
                            {
                                strGsm = strGsm + tabGsmCount.Rows[k][0] + ",";
                            }
                            strGsm = strGsm + RSMID;
                            sqlrsmid = string.Format(" and RSMID in ({0})", strGsm);
                        }
                            // by yyan itemw147 20110915 add start
                        else if (tabGsm.Rows[0][0].ToString() == "1" || tabGsm.Rows[0][0].ToString() == "5")
                        {
                            sqlrsmid =
                                " and SegmentAbbr in (select Segment.abbr from Segment,[User],User_Segment where " +
                                " Segment.Deleted=0 and User_Segment.SegmentID =Segment.ID and " +
                                " User_Segment.Deleted=0 and User_Segment.UserID= [User].UserID and [User].Deleted =0 and" +
                                " [User].UserID=" + RSMID + ")";
                        }
                            // by yyan itemw147 20110915 add end
                        else
                        {
                            sqlrsmid = string.Format(" and RSMID = '{0}'", RSMID);
                        }
                        //yyan itemw124 20110829 add end 
                    }

                    string sqlcountry = "";
                    if (!string.IsNullOrEmpty(CountryID))
                    {
                        sqlcountry = string.Format(" and CountryID = '{0}'", CountryID);
                    }

                    string sqlcustomer = "";
                    if (!string.IsNullOrEmpty(CustomerID))
                    {
                        sqlcustomer = string.Format(" and CustomerID = '{0}'", CustomerID);
                    }

                    // add by zy 20110120 start
                    string sqlmeetingdate = "";
                    if (!string.IsNullOrEmpty(SelectMeetingDate))
                    {
                        //item 20110519 item 53 del start
                        //sqlmeetingdate = string.Format(" and TimeFlag = '{0}'", SelectMeetingDate);
                        //item 20110519 item 53 del end
                        //item 20110519 item 53 add start
                        //item 20110914 itemw143 del start
                        //sqlmeetingdate = string.Format(" and CONVERT(varchar(15),TimeFlag,23) = '{0}'", SelectMeetingDate);
                        //item 20110914 itemw143 del end
                        //item 20110914 itemw143 add start
                        sqlmeetingdate =
                            string.Format(
                                " and CONVERT(varchar(15),TimeFlag,23) = '{0}' and  CONVERT(varchar(15),CurrencyTimeFlag,23) = '{1}' ",
                                SelectMeetingDate, SelectMeetingDate);
                        //item 20110914 itemw143 add end
                        //item 20110519 item 53 add end
                    }
                    // add by zy 20110120 end

                    string sqlwhere = "";

                    // update by zy 20110120 start
                    //sqlwhere = string.Format(" where 0=0 {0} {1} {2} {3} ", sqlrsmid, sqlcountry, sqlcustomer, wherename);
                    sqlwhere = string.Format(" where 0=0 {0} {1} {2} {3} {4}", sqlrsmid, sqlcountry, sqlcustomer,
                                             sqlmeetingdate, wherename);
                    // update by zy 20110120 end
                    if (rsmFlag)
                    {
                        sqlwhere +=
                            string.Format(
                                @" and  SegmentAbbr in ( SELECT abbr FROM Segment,User_Segment where 
 User_Segment.Deleted=0  and User_Segment.UserID={0})
 and SubRegion in
 (SELECT SubRegion.Name FROM User_Country 
 INNER JOIN SubRegion ON User_Country.CountryID=
 SubRegion.ID WHERE User_Country.Deleted=0 AND SubRegion.Deleted=0 AND
  User_Country.UserID={1})
  
  and SalesOrgNameAbbr 
  in
 (SELECT SalesOrg.abbr FROM SalesOrg_User 
 INNER JOIN SalesOrg ON SalesOrg_User.SalesOrgID=
 SalesOrg.ID WHERE SalesOrg_User.Deleted=0 AND SalesOrg.Deleted=0 AND
  SalesOrg_User.UserID={2})",
                                RSMID, RSMID, RSMID);
                    }
                    // add by zy 20110106 start
                    if (!string.IsNullOrEmpty(sqlgroup))
                    {
                        sqlgroup = string.Format(" group by {0} ", sqlgroup);
                    }


                    // add by zy 20110106 end

                    string sqlorder = "";
                    if (!string.IsNullOrEmpty(orderby))
                    {
                        //BY yyan 20110513 item 53 del start
                        //sqlorder = string.Format(" order by {0} ", orderby);
                        //BY yyan 20110513 item 53 del end
                        //BY yyan 20110513 item 53 add start
                        string[] orderSplit = orderby.Split(' ');
                        if (orderSplit.Length > 2)
                        {
                            string selOrder =
                                string.Format("select FieldName from ReportValue WHERE ID={0} and NewFieldName={1}",
                                              RepID, "'" + orderSplit[1] + "'");
                            DataTable tabOrder = helper.GetDataSet(selOrder).Tables[0];
                            //BY yyan 20110524 item W10 del start
                            //if (tabOrder.Rows[0][0].ToString().Trim().Equals("Amount"))
                            //BY yyan 20110524 item W10 del end
                            //BY yyan 20110524 item W10 add start
                            if (tabOrder.Rows[0][0].ToString().Trim().Equals(Resource.GAPrice)
                                || tabOrder.Rows[0][0].ToString().Trim().Equals(Resource.AmountEUR)
                                || tabOrder.Rows[0][0].ToString().Trim().Equals(Resource.KPrice))
                                //BY yyan 20110524 item W10 add end
                            {
                                sqlorder = string.Format(" order by {0} {1}", orderSplit[1], orderSplit[4]);
                            }
                            else
                            {
                                sqlorder = string.Format(" order by {0} {1} {2} {3} {4}", orderSplit[0],
                                                         tabOrder.Rows[0][0], orderSplit[2], orderSplit[3],
                                                         orderSplit[4]);
                            }
                        }
                        else
                        {
                            sqlorder = string.Format(" order by {0} ", orderby);
                        }
                        //BY yyan 20110513 item 53 add end
                    }
                    else if (!string.IsNullOrEmpty(ordername))
                    {
                        sqlorder = string.Format(" order by {0} ", ordername);
                    }
                    // update by zy 20110106 start
                    //string sql = sqlselect + sqlwhere + sqlorder;
                    string sql = sqlselect + sqlwhere + sqlgroup + sqlorder;
                    // update by zy 20110106 end
                    return sql;
                }
                else
                {
                    return "";
                }
            }
            else
            {
                return "";
            }
        }
        catch (Exception ex)
        {
            return "";
        }
    }



    
    private string getFieldNames(DataTable tab)
    {
        string fieldname = "";
        if (tab.Columns.IndexOf("FieldName") != -1)
        {
            var fieldnames = new StringBuilder();
            foreach (DataRow row in tab.Rows)
            {
                if (!string.IsNullOrEmpty(row["FieldName"].ToString()))
                {
                    //BY yyan 20110513 item 53 del start 
                    //fieldnames.Append("," + row["FieldName"].ToString());
                    //BY yyan 20110513 item 53 del end 
                    //BY yyan 20110513 item 53 add start 
                    fieldnames.Append(row["FieldName"] + ",");
                    //BY yyan 20110513 item 53 add end 
                }
            }
            if (fieldnames.Length > 0)
            {
                fieldname = fieldnames.ToString();
                //BY yyan 20110513 item 53 del start 
                //fieldname = fieldname.Replace(",", " ");
                //fieldname = fieldname.Trim().Replace(" ", ",");
                //BY yyan 20110513 item 53 del end 
                //BY yyan 20110513 item 53 add start 
                fieldname = fieldname.Substring(0, fieldname.Length - 1);
                //BY yyan 20110513 item 53 add end 
            }
        }
        return fieldname;
    }

    
    private string getWhereNames(DataTable tab)
    {
        string fieldname = "";
        if (tab.Columns.IndexOf("FieldName") != -1 && tab.Columns.IndexOf("Operator") != -1)
        {
            var fieldnames = new StringBuilder();
            foreach (DataRow row in tab.Rows)
            {
                if (!string.IsNullOrEmpty(row["Operator"].ToString()))
                {
                    string sql = string.Format("select Name from DataDict where BigType=1 and SmallType = {0}",
                                               row["Operator"]);
                    var wheretype = (string) helper.ExecuteScalar(CommandType.Text, sql, null);

                    if (!string.IsNullOrEmpty(row["FieldName"].ToString()) &&
                        !string.IsNullOrEmpty(row["Condition1"].ToString()))
                    {
                        switch (wheretype.ToLower())
                        {
                            case "between":
                                if (!string.IsNullOrEmpty(row["Condition2"].ToString()))
                                {
                                    fieldnames.Append(" and " + row["FieldName"] + " " + wheretype + " '" +
                                                      WebUtility.transParam(row["Condition1"].ToString()) + "' and '" +
                                                      WebUtility.transParam(row["Condition2"].ToString()) + "'");
                                }
                                break;
                            case "like":
                                fieldnames.Append(" and " + row["FieldName"] + " " + wheretype + " '%" +
                                                  WebUtility.transParam(row["Condition1"].ToString()) + "%'");
                                break;
                            case "not like":
                                fieldnames.Append(" and " + row["FieldName"] + " " + wheretype + " '%" +
                                                  WebUtility.transParam(row["Condition1"].ToString()) + "%'");
                                break;
                                // add by zy 20110110 start
                            case "in":
                                fieldnames.Append(" and " + row["FieldName"] + " " + wheretype + " (" +
                                                  row["Condition1"] + ")");
                                break;
                                // add by zy 20110110 end
                            default:
                                fieldnames.Append(" and " + row["FieldName"] + " " + wheretype + " '" +
                                                  WebUtility.transParam(row["Condition1"].ToString()) + "'");
                                break;
                        }
                    }
                }
            }
            fieldname = fieldnames.ToString();
        }
        return fieldname;
    }


    private string getSortNames(DataTable tab)
    {
        string fieldname = "";
        if (tab.Columns.IndexOf("FieldName") != -1 && tab.Columns.IndexOf("Sort") != -1)
        {
            var fieldnames = new StringBuilder();
            foreach (DataRow row in tab.Rows)
            {
                string sortid = row["Sort"].ToString();
                if (!string.IsNullOrEmpty(sortid))
                {
                    if (!string.IsNullOrEmpty(row["FieldName"].ToString()))
                    {
                        switch (sortid)
                        {
                            case "1":
                                if (row["FieldType"].ToString().ToLower().Equals("string"))
                                {
                                    fieldnames.Append(",cast(" + row["FieldName"] + " as nvarchar) asc ");
                                }
                                else
                                {
                                    fieldnames.Append("," + row["FieldName"] + " asc ");
                                }
                                break;
                            case "2":
                                if (row["FieldType"].ToString().ToLower().Equals("string"))
                                {
                                    fieldnames.Append(",cast(" + row["FieldName"] + " as nvarchar) desc ");
                                }
                                else
                                {
                                    fieldnames.Append("," + row["FieldName"] + " desc ");
                                }
                                break;
                        }
                    }
                }
            }
            if (fieldnames.Length > 0)
            {
                fieldname = fieldnames.ToString();
                fieldname = fieldname.Remove(0, 1);
            }
        }
        return fieldname;
    }

    // add by zy 20110120 start

    /// <summary>
    /// get select meeting date by user.
    /// </summary>
    /// <param name="userID">user id</param>
    /// <returns>select meeting date</returns>
    public string getSelectMeetingDate(string userID)
    {
        //item 20110519 item 53 del start
        //string sql_query = "SELECT SelectMeetingDate FROM [SetMeetingDate]";
        //item 20110519 item 53 del end
        //item 20110519 item 53 add start
        string sql_query =
            "SELECT CONVERT(varchar(15),SelectMeetingDate,23) SelectMeetingDate FROM [SetSelectMeetingDate] where userid=" +
            userID;
        //item 20110519 item 53 add end
        DataSet ds = helper.GetDataSet(sql_query);

        if (ds.Tables.Count > 0)
        {
            if (ds.Tables[0].Rows.Count > 0)
            {
                string str_date = ds.Tables[0].Rows[0][0].ToString();
                if (str_date != "")
                {
                    return str_date;
                }
            }
        }
        return null;
    }

    
    /// <summary>
    /// get sql query script for for query report result except BookingsSales view and SalesBacklog view.
    /// </summary>
    /// <param name="RepID">report id</param>
    /// <param name="RSMID">user id</param>
    /// <param name="CountryID">country id</param>
    /// <param name="CustomerID">customer id</param>
    /// <param name="orderby">order condition</param>
    /// <returns>sql query script</returns>
    public string RepCompileSql(string RepID, string RSMID, string CountryID, string CustomerID, string orderby)
    {
        try
        {
            string selrep = string.Format("select * from  ReportList WHERE ID={0}", RepID);
            string selrepv = string.Format("select * from  ReportValue WHERE ID={0} ORDER BY FieldOrder", RepID);
            DataTable tabrep = helper.GetDataSet(selrep).Tables[0];
            DataTable tabrepv = helper.GetDataSet(selrepv).Tables[0];
            if (tabrep != null && selrepv != null)
            {
                if (tabrep.Rows.Count > 0 && tabrepv.Rows.Count > 0)
                {
                    string viewname = tabrep.Rows[0]["ViewName"].ToString();
                    //by yyan item 53 20110513 del start
                    //string fieldname = getFieldNames(tabrepv);
                    //by yyan item 53 20110513 del end
                    //by yyan item 53 20110513 add start
                    string fieldname = getFieldNames1(tabrepv);
                    //by yyan item 53 20110513 add end
                    string wherename = getWhereNames(tabrepv);
                    string ordername = getSortNames(tabrepv);
                    string sqlselect = string.Format("select {0} from {1}", fieldname, viewname);

                    string sqlwhere = "";
                    //by yyan item 17 20110511 del start
                    //sqlwhere = string.Format(" where 0=0 ");
                    //by yyan item 17 20110511 del end
                    //by yyan item 17 20110511 add start
                    sqlwhere = string.Format(" where 0=0 {0} ", wherename);
                    //by yyan item 17 20110511 add end
                    string sqlorder = "";
                    if (!string.IsNullOrEmpty(orderby))
                    {
                        //by yyan item 53 20110513 del start
                        //sqlorder = string.Format(" order by {0} ", orderby);
                        //by yyan item 53 20110513 del end
                        //by yyan item 53 20110513 add start
                        string[] orderSplit = orderby.Split(' ');
                        string selOrder =
                            string.Format("select FieldName from ReportValue WHERE ID={0} and NewFieldName={1}", RepID,
                                          "'" + orderSplit[1] + "'");
                        DataTable tabOrder = helper.GetDataSet(selOrder).Tables[0];
                        sqlorder = string.Format(" order by {0} {1} {2} {3} {4}", orderSplit[0], tabOrder.Rows[0][0],
                                                 orderSplit[2], orderSplit[3], orderSplit[4]);
                        //by yyan item 53 20110513 add end
                    }
                    else if (!string.IsNullOrEmpty(ordername))
                    {
                        sqlorder = string.Format(" order by {0} ", ordername);
                    }
                    string sql = sqlselect + sqlwhere + sqlorder;
                    return sql;
                }
                else
                {
                    return "";
                }
            }
            else
            {
                return "";
            }
        }
        catch (Exception ex)
        {
            return "";
        }
    }


    private string getFieldNames1(DataTable tab)
    {
        string fieldname = "";
        if (tab.Columns.IndexOf("FieldName") != -1)
        {
            var fieldnames = new StringBuilder();
            foreach (DataRow row in tab.Rows)
            {
                if (!string.IsNullOrEmpty(row["FieldName"].ToString()))
                {
                    fieldnames.Append(row["FieldName"] + "#AS#" + row["NewFieldName"] + ",");
                }
            }
            if (fieldnames.Length > 0)
            {
                fieldname = fieldnames.ToString();
                fieldname = fieldname.Substring(0, fieldname.Length - 1);
                fieldname = fieldname.Trim().Replace("#", " ");
            }
        }
        return fieldname;
    }


    private string getNewFieldNames(DataTable tab)
    {
        string fieldname = "";
        if (tab.Columns.IndexOf("NewFieldName") != -1)
        {
            var fieldnames = new StringBuilder();
            foreach (DataRow row in tab.Rows)
            {
                if (!string.IsNullOrEmpty(row["NewFieldName"].ToString()))
                {
                    fieldnames.Append(row["NewFieldName"] + ",");
                }
            }
            if (fieldnames.Length > 0)
            {
                fieldname = fieldnames.ToString();
                fieldname = fieldname.Substring(0, fieldname.Length - 1);
            }
        }
        return fieldname;
    }

    //BY yyan 20110509 item 53 add end
    /// <summary>
    /// get sql query script for query report result related to SalesBacklog view.
    /// </summary>
    /// <param name="RepID">report id</param>
    /// <param name="RSMID">user id</param>
    /// <param name="orderby">order condition</param>
    /// <param name="SelectMeetingDate">select meeting date</param>
    /// <returns>sql query script</returns>
    public string RepCompileSqlBacklog(string RepID, string RSMID, string orderby, string SelectMeetingDate)
    {
        try
        {
            string selrep = string.Format("select * from  ReportList WHERE ID={0}", RepID);
            string selrepv = string.Format("select * from  ReportValue WHERE ID={0} ORDER BY FieldOrder", RepID);
            DataTable tabrep = helper.GetDataSet(selrep).Tables[0];
            DataTable tabrepv = helper.GetDataSet(selrepv).Tables[0];
            if (tabrep != null && selrepv != null)
            {
                if (tabrep.Rows.Count > 0 && tabrepv.Rows.Count > 0)
                {
                    string viewname = tabrep.Rows[0]["ViewName"].ToString();
                    string fieldname = getFieldNames(tabrepv);
                    string newFieldname = getNewFieldNames(tabrepv);
                    string wherename = getWhereNames(tabrepv);
                    string sqlgroup = "";
                    if (fieldname.Contains("Amount") || fieldname.Contains("AmountEUR"))
                    {
                        sqlgroup = fieldname.Replace(",AmountEUR", "");
                        sqlgroup = sqlgroup.Replace("AmountEUR,", "");
                        fieldname = fieldname.Replace("AmountEUR", "changeEUR");
                        sqlgroup = sqlgroup.Replace(",Amount", "");
                        sqlgroup = sqlgroup.Replace("Amount,", "");
                        fieldname = fieldname.Replace("Amount", "ROUND(SUM(Amount)~0)");
                        fieldname = fieldname.Replace("changeEUR", "ROUND(SUM(AmountEUR)~0)");
                    }
                    else
                    {
                        sqlgroup = fieldname;
                    }
                    if (fieldname.Contains(","))
                    {
                        string[] strFieldNames = fieldname.Split(',');
                        string[] strNewFieldNames = newFieldname.Split(',');
                        string strInfo = "";
                        for (int i = 0; i < strFieldNames.Length; i++)
                        {
                            strInfo = strInfo + strFieldNames[i] + "#AS#" + strNewFieldNames[i] + ",";
                        }
                        strInfo = strInfo.Substring(0, strInfo.Length - 1);
                        strInfo = strInfo.Replace('#', ' ');
                        fieldname = strInfo;
                    }
                    else
                    {
                        fieldname = fieldname + " AS " + newFieldname;
                    }
                    fieldname = fieldname.Replace('~', ',');
                    string ordername = getSortNames(tabrepv);
                    string sqlselect = string.Format("select {0} from {1}", fieldname, viewname);

                    string sqlrsmid = "";
                    // by yyan itemw147 20110915 add start
                    if (!string.IsNullOrEmpty(RSMID))
                    {
                        string sqlGsm = "select roleid from [user] where userid=" + RSMID;
                        DataTable tabGsm = helper.GetDataSet(sqlGsm).Tables[0];
                        if (tabGsm.Rows[0][0].ToString() == "1" || tabGsm.Rows[0][0].ToString() == "5")
                        {
                            sqlrsmid =
                                " and SegmentAbbr in (select Segment.abbr from Segment,[User],User_Segment where " +
                                " Segment.Deleted=0 and User_Segment.SegmentID =Segment.ID and " +
                                " User_Segment.Deleted=0 and User_Segment.UserID= [User].UserID and [User].Deleted =0 and" +
                                " [User].UserID=" + RSMID + ")";
                        }
                    }
                    // by yyan itemw147 20110915 add end
                    string sqlmeetingdate = "";
                    if (!string.IsNullOrEmpty(SelectMeetingDate))
                    {
                        sqlmeetingdate = string.Format(" and CONVERT(varchar(15),TimeFlag,23) = '{0}'",
                                                       SelectMeetingDate);
                    }
                    string sqlwhere = "";
                    sqlwhere = string.Format(" where 0=0 {0} {1} {2} ", sqlrsmid, sqlmeetingdate, wherename);

                    if (!string.IsNullOrEmpty(sqlgroup))
                    {
                        sqlgroup = string.Format(" group by {0} ", sqlgroup);
                    }

                    string sqlorder = "";
                    if (!string.IsNullOrEmpty(orderby))
                    {
                        string[] orderSplit = orderby.Split(' ');
                        if (orderSplit.Length > 2)
                        {
                            string selOrder =
                                string.Format("select FieldName from ReportValue WHERE ID={0} and NewFieldName={1}",
                                              RepID, "'" + orderSplit[1] + "'");
                            DataTable tabOrder = helper.GetDataSet(selOrder).Tables[0];

                            if (tabOrder.Rows[0][0].ToString().Trim().Equals("Amount") ||
                                tabOrder.Rows[0][0].ToString().Trim().Equals("AmountEUR"))
                            {
                                sqlorder = string.Format(" order by {0} {1}", orderSplit[1], orderSplit[4]);
                            }
                            else
                            {
                                sqlorder = string.Format(" order by {0} {1} {2} {3} {4}", orderSplit[0],
                                                         tabOrder.Rows[0][0], orderSplit[2], orderSplit[3],
                                                         orderSplit[4]);
                            }
                        }
                        else
                        {
                            sqlorder = string.Format(" order by {0} ", orderby);
                        }
                    }
                    else if (!string.IsNullOrEmpty(ordername))
                    {
                        sqlorder = string.Format(" order by {0} ", ordername);
                    }
                    string sql = sqlselect + sqlwhere + sqlgroup + sqlorder;
                    return sql;
                }
                else
                {
                    return "";
                }
            }
            else
            {
                return "";
            }
        }
        catch (Exception ex)
        {
            return "";
        }
    }

    //by yyan ItemW149 20110916 add start
    ///// <summary>
    ///// get sql query script
    ///// </summary>
    ///// <param name="RepID">report id</param>
    ///// <param name="RSMID">user id</param>
    ///// <param name="CountryID">country id</param>
    ///// <param name="CustomerID">customer id</param>
    ///// <param name="orderby">order condition</param>
    ///// <param name="viewName">view name</param>
    ///// <returns>sql query script</returns>
    //public string RepCompileSqlA(string RepID, string RSMID, string CountryID, string CustomerID, string orderby,
    //                             string viewName)
    //{
    //    try
    //    {
    //        string selrep = string.Format("select * from  ReportList WHERE ID={0}", RepID);
    //        string selrepv = string.Format("select * from  ReportValue WHERE ID={0} ORDER BY FieldOrder", RepID);
    //        DataTable tabrep = helper.GetDataSet(selrep).Tables[0];
    //        DataTable tabrepv = helper.GetDataSet(selrepv).Tables[0];
    //        if (tabrep != null && selrepv != null)
    //        {
    //            if (tabrep.Rows.Count > 0 && tabrepv.Rows.Count > 0)
    //            {
    //                string viewname = tabrep.Rows[0]["ViewName"].ToString();

    //                string fieldname = getFieldNames1(tabrepv);

    //                string wherename = getWhereNames(tabrepv);
    //                string ordername = getSortNames(tabrepv);
    //                string sqlselect = string.Format("select {0} from {1}", fieldname, viewname);

    //                string sqlwhere = "";
    //                string sqlrsmid = "";
    //                if (!string.IsNullOrEmpty(RSMID))
    //                {
    //                    string sqlGsm = "select roleid from [user] where userid=" + RSMID;
    //                    DataTable tabGsm = helper.GetDataSet(sqlGsm).Tables[0];
    //                    if (tabGsm.Rows[0][0].ToString() == "1" || tabGsm.Rows[0][0].ToString() == "5")
    //                    {
    //                        sqlrsmid = " and Segment in (select Segment.abbr from Segment,[User],User_Segment where " +
    //                                   " Segment.Deleted=0 and User_Segment.SegmentID =Segment.ID and " +
    //                                   " User_Segment.Deleted=0 and User_Segment.UserID= [User].UserID and [User].Deleted =0 and" +
    //                                   " [User].UserID=" + RSMID + ")";
    //                    }
    //                }

    //                sqlwhere = string.Format(" where 0=0 {0} {1}", wherename, sqlrsmid);
    //                //by yyan item 17 20110511 add end
    //                string sqlorder = "";
    //                if (!string.IsNullOrEmpty(orderby))
    //                {
    //                    //by yyan item 53 20110513 del start
    //                    //sqlorder = string.Format(" order by {0} ", orderby);
    //                    //by yyan item 53 20110513 del end
    //                    //by yyan item 53 20110513 add start
    //                    string[] orderSplit = orderby.Split(' ');
    //                    string selOrder =
    //                        string.Format("select FieldName from ReportValue WHERE ID={0} and NewFieldName={1}", RepID,
    //                                      "'" + orderSplit[1] + "'");
    //                    DataTable tabOrder = helper.GetDataSet(selOrder).Tables[0];
    //                    sqlorder = string.Format(" order by {0} {1} {2} {3} {4}", orderSplit[0], tabOrder.Rows[0][0],
    //                                             orderSplit[2], orderSplit[3], orderSplit[4]);
    //                    //by yyan item 53 20110513 add end
    //                }
    //                else if (!string.IsNullOrEmpty(ordername))
    //                {
    //                    sqlorder = string.Format(" order by {0} ", ordername);
    //                }
    //                string sql = sqlselect + sqlwhere + sqlorder;
    //                return sql;
    //            }
    //            else
    //            {
    //                return "";
    //            }
    //        }
    //        else
    //        {
    //            return "";
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        return "";
    //    }
    //}

    //by yyan ItemW149 20110916 add end
}