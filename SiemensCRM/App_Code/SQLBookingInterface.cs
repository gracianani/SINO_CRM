/*
 * File Name    : SQLBookingInterface.cs
 * 
 * Description  : SQL statements about "booking sales data" page
 * 
 * Author       : Wangjun
 * 
 * Modify Date : 2010-01-24
 * 
 * Problem     : 
 * 
 * Version     : Release(2.0)
 */

using System.Data;
using System.Globalization;
using System.Text;

/// <summary>
///  SQL statements about "booking sales data" page.
/// </summary>
public class SQLBookingInterface
{
    private readonly GetMeetingDate date;
    private readonly SQLStatement sql;

    public SQLBookingInterface()
    {
        //
        // TODO: Add constructor logic here
        //
        date = new GetMeetingDate();
        sql = new SQLStatement();
    }

    protected string getCurrencyBySalesOrgID(string str_salesorgID)
    {
        return sql.getSalesOrgCurrency(str_salesorgID).Trim();
    }
    
    /// <summary>
    /// get booking data total sql script
    /// </summary>
    /// <param name="salesorgID">salesorg id</param>
    /// <param name="dsProduct">products</param>
    /// <param name="segmentID">segment id</param>
    /// <param name="rsmAbbr">user Abbr</param>
    /// <param name="rsmID">user id</param>
    /// <param name="bookingYear">booking year</param>
    /// <param name="deliverYear">deliver year</param>
    /// <param name="year">year</param>
    /// <param name="month">month</param>
    /// <param name="countryID">country id</param>
    /// <param name="convert_flag">true for converting to Euro, false for not converting</param>
    /// <param name="strSQL">country ids sql</param>
    /// <returns>sql script</returns>
    public string getBookingDataTotal(string salesorgID, DataSet dsProduct, string segmentID, string rsmAbbr,
                                      string rsmID, string bookingYear, string deliverYear, string year, string month,
                                      string countryID, bool convert_flag, string strSQL)
    {
        string str_currency = getCurrencyBySalesOrgID(salesorgID);
        double db_rate1 = sql.getRate(str_currency, true, year, month);
        double db_rate2 = sql.getRate(str_currency, false, year, month);
        string str_rate1 = db_rate1.ToString().Replace(',', '.');
        string str_rate2 = db_rate2.ToString().Replace(',', '.');

        if (dsProduct != null)
        {
            string sqlstr = "SELECT ('" + rsmAbbr + "/'+[Operation].AbbrL) AS Operation";
            string temp = "";
            for (int count = 0; count < dsProduct.Tables[0].Rows.Count; count++)
            {
                temp += " ,SUM(CASE WHEN ProductID = " + dsProduct.Tables[0].Rows[count][0]
                        + " AND YEAR(TimeFlag) = '" + year + "'"
                        + " AND MONTH(TimeFlag) = '" + month;

                if (convert_flag)
                {
                    //if (((string.Equals(month,"3") || string.Equals(month, "03")) && string.Equals(bookingYear, year.Substring(2)))
                    //    || deliverYear == year.Substring(2, 2).Trim() || deliverYear == "YTD")
                    if (((string.Equals(month, "3") || string.Equals(month, "03")) &&
                         string.Equals(bookingYear, year.Substring(2)))
                        || deliverYear == "YTD")
                        temp += "' AND BookingY='" + bookingYear + "' AND DeliverY='" + deliverYear
                                + "' THEN ROUND(Amount*" + str_rate1 + ",0) ELSE 0 END) AS '"
                                + dsProduct.Tables[0].Rows[count][1] + "'";
                    else
                        temp += "' AND BookingY='" + bookingYear + "' AND DeliverY='" + deliverYear
                                + "' THEN ROUND(Amount*" + str_rate2 + ",0) ELSE 0 END) AS '"
                                + dsProduct.Tables[0].Rows[count][1] + "'";
                }
                else
                {
                    temp += "' AND BookingY='" + bookingYear + "' AND DeliverY='" + deliverYear
                            + "' THEN ROUND(Amount,0) ELSE 0 END) AS '"
                            + dsProduct.Tables[0].Rows[count][1] + "'";
                }
            }
            temp += " FROM [Bookings] "
                    + " INNER JOIN [Operation] ON [Bookings].OperationID = [Operation].ID"
                    + " WHERE RSMID = " + rsmID + " AND SegmentID = " + segmentID + " AND SalesOrgID = " + salesorgID
                    + " AND Operation.Deleted=0 ";
            if (!countryID.Trim().Equals("-1") && !countryID.Trim().Equals(""))
            {
                temp = temp + " AND [Bookings].CountryID = " + countryID;
            }
            else
            {
                temp += " AND [Bookings].CountryID IN (" + strSQL + ")";
            }

            //by yyan 20110526 w14 add start
            DataSet dtTimeFlag = date.getSetMeetingDate();
            temp += " AND [Bookings].TimeFlag = '" + dtTimeFlag.Tables[0].Rows[0][0] + "'";
            //by yyan 20110526 w14 add end 

            temp += " GROUP BY [Operation].AbbrL"
                    + " ORDER BY [Operation].AbbrL ASC";

            sqlstr += temp;
            return sqlstr;
        }
        return "";
    }
    
    

    /// <summary>
    /// get booking data total by this year
    /// </summary>
    /// <param name="dsProduct">products</param>
    /// <param name="salesOrgID">salesorg id</param>
    /// <param name="segmentID">segment id</param>
    /// <param name="str_rsmAbbr">user Abbr</param>
    /// <param name="rsmID">user id</param>
    /// <param name="year">year</param>
    /// <param name="month">month</param>
    /// <param name="countryID">country id</param>
    /// <param name="convert_flag">true for converting to Euro, false for not converting</param>
    /// <param name="strSQL">country ids sql</param>
    /// <returns>sql script</returns>
    public string getBookingDataTotalThisYear(DataSet dsProduct, string salesOrgID, string segmentID,
                                              string str_rsmAbbr, string rsmID, string year, string month,
                                              string countryID, bool convert_flag, string strSQL)
    {
        string str_currency = getCurrencyBySalesOrgID(salesOrgID);
        double db_rate1 = sql.getRate(str_currency, false, year, month);
        string str_rate1 = db_rate1.ToString().Replace(',', '.');

        if (dsProduct != null)
        {
            DataTable dt = dsProduct.Tables[0];
            string sqlstr = "SELECT ('" + str_rsmAbbr + "/'+[Operation].AbbrL) AS Operation";
            string temp = "";
            for (int count = 0; count < dt.Rows.Count; count++)
            {
                temp += " ,SUM(CASE WHEN ProductID = " + dt.Rows[count][0]
                        + " AND YEAR(TimeFlag)='" + year + "'"
                        + " AND MONTH(TimeFlag)='" + month;
                if (convert_flag)
                {
                    temp += "' AND BookingY='" + year.Substring(2, 2) + "' THEN ROUND(Amount*" + str_rate1 +
                            ",0) ELSE 0 END) AS '"
                            + dt.Rows[count][1] + "'";
                }
                else
                {
                    temp += "' AND BookingY='" + year.Substring(2, 2) + "' THEN ROUND(Amount,0) ELSE 0 END) AS '"
                            + dt.Rows[count][1] + "'";
                }
            }
            temp += " FROM [Bookings] INNER JOIN [Operation] ON [Bookings].OperationID = [Operation].ID"
                    + " WHERE RSMID= " + rsmID + " AND SegmentID = " + segmentID + " AND SalesOrgID = " + salesOrgID
                    + " AND Operation.Deleted=0 ";

            if (!countryID.Trim().Equals("-1") && !countryID.Trim().Equals(""))
            {
                temp = temp + " AND [Bookings].CountryID = " + countryID;
            }
            else
            {
                temp += " AND [Bookings].CountryID IN (" + strSQL + ")";
            }

            //by yyan 20110526 w14 add start
            DataSet dtTimeFlag = date.getSetMeetingDate();
            temp += " AND [Bookings].TimeFlag = '" + dtTimeFlag.Tables[0].Rows[0][0] + "'";
            //by yyan 20110526 w14 add end 

            temp = temp + " GROUP BY Operation.AbbrL"
                   + " ORDER BY Operation.AbbrL ASC";

            sqlstr += temp;
            return sqlstr;
        }
        else
            return "";
    }

  
    /// <summary>
    /// get booking data total by next year
    /// </summary>
    /// <param name="salesOrgID">salesorg id</param>
    /// <param name="dsProduct">products</param>
    /// <param name="segmentID">segment id</param>
    /// <param name="rsmAbbr">user Abbr</param>
    /// <param name="rsmID">user id</param>
    /// <param name="year">year</param>
    /// <param name="month">month</param>
    /// <param name="nextyear">next year</param>
    /// <param name="countryID">country id</param>
    /// <param name="convert_flag">true for converting to Euro, false for not converting</param>
    /// <param name="strSQL">country ids sql</param>
    /// <returns>sql script</returns>
    public string getBookingDataTotalByNextYear(string salesOrgID, DataSet dsProduct, string segmentID, string rsmAbbr,
                                                string rsmID, string year, string month, string nextyear,
                                                string countryID, bool convert_flag, string strSQL)
    {
        string str_currency = getCurrencyBySalesOrgID(salesOrgID);
        double db_rate2 = sql.getRate(str_currency, false, year, month);
        string str_rate2 = db_rate2.ToString().Replace(',', '.');

        if (dsProduct != null)
        {
            DataTable dt = dsProduct.Tables[0];
            string sqlstr = "SELECT ('" + rsmAbbr + "/'+[Operation].AbbrL) AS Operation";
            string temp = "";
            for (int count = 0; count < dt.Rows.Count; count++)
            {
                temp += " ,SUM(CASE WHEN ProductID = " + dt.Rows[count][0]
                        + " AND YEAR(TimeFlag)='" + year + "'"
                        + " AND MONTH(TimeFlag)='" + month;
                if (convert_flag)
                {
                    temp += "' AND BookingY='" + nextyear.Substring(2, 2) + "' THEN ROUND(Amount*" + str_rate2 +
                            ",0) ELSE 0 END) AS '"
                            + dt.Rows[count][1] + "'";
                }
                else
                {
                    temp += "' AND BookingY='" + nextyear.Substring(2, 2) + "' THEN ROUND(Amount,0) ELSE 0 END) AS '"
                            + dt.Rows[count][1] + "'";
                }
            }
            temp += " FROM [Bookings] INNER JOIN [Operation] ON [Bookings].OperationID = [Operation].ID"
                    + " WHERE RSMID= " + rsmID + " AND SegmentID = " + segmentID + " AND SalesOrgID = " + salesOrgID
                    + " AND Operation.Deleted=0 ";

            if (!countryID.Trim().Equals("-1") && !countryID.Trim().Equals(""))
            {
                temp = temp + " AND [Bookings].CountryID = " + countryID;
            }
            else
            {
                temp += " AND [Bookings].CountryID IN (" + strSQL + ")";
            }

            //by yyan 20110526 w14 add start
            DataSet dtTimeFlag = date.getSetMeetingDate();
            temp += " AND [Bookings].TimeFlag = '" + dtTimeFlag.Tables[0].Rows[0][0] + "'";
            //by yyan 20110526 w14 add end 

            temp = temp + " GROUP BY Operation.AbbrL"
                   + " ORDER BY Operation.AbbrL ASC";

            sqlstr += temp;
            return sqlstr;
        }
        else
            return "";
    }


    /// <summary>
    /// get booking data total by pre year
    /// </summary>
    /// <param name="salesOrgID">salesorg id</param>
    /// <param name="dsProduct">products</param>
    /// <param name="segmentID">segment id</param>
    /// <param name="rsmAbbr">user Abbr</param>
    /// <param name="rsmID">user id</param>
    /// <param name="year">year</param>
    /// <param name="month">month</param>
    /// <param name="preyear">pre year</param>
    /// <param name="premonth">pre month</param>
    /// <param name="countryID">country id</param>
    /// <param name="convert_flag">true for converting to Euro, false for not converting</param>
    /// <param name="strSQL">country ids sql</param>
    /// <returns>sql script</returns>
    public string getBookingDataTotaByThislVSPre(string salesOrgID, DataSet dsProduct, string segmentID, string rsmAbbr,
                                                 string rsmID, string year, string month, string preyear,
                                                 string premonth, string countryID, bool convert_flag, string strSQL)
    {
        string str_currency = getCurrencyBySalesOrgID(salesOrgID);
        double db_rate1 = sql.getRate(str_currency, true, year, premonth);
        double db_rate2 = sql.getRate(str_currency, false, year, premonth);
        double db_prerate2 = sql.getRate(str_currency, false, preyear, premonth);
        string str_rate1 = db_rate1.ToString().Replace(',', '.');
        string str_rate2 = db_rate2.ToString().Replace(',', '.');
        string str_prerate2 = db_rate2.ToString().Replace(',', '.');

        if (dsProduct != null)
        {
            DataTable dt = dsProduct.Tables[0];
            string sqlstr = "SELECT ('" + rsmAbbr + "/'+[Operation].AbbrL) AS Operation";
            string temp = "";
            for (int count = 0; count < dt.Rows.Count; count++)
            {
                temp += " ,SUM(CASE WHEN ProductID = " + dt.Rows[count][0];

                if (convert_flag)
                {
                    if (date.JudgeFirstMonth(month))
                        temp += " AND YEAR(TimeFlag)='" + preyear + "' AND MONTH(TimeFlag)='" + premonth + "'"
                                + " AND BookingY = '" + year.Substring(2, 2)
                                + "' THEN ROUND(Amount*" + str_prerate2 + ",0) ELSE 0 END) AS '"
                                + dt.Rows[count][1] + "'";
                    else
                        temp += " AND YEAR(TimeFlag)='" + year + "' AND MONTH(TimeFlag)='" + premonth + "'"
                                + " AND BookingY = '" + year.Substring(2, 2)
                                + "' THEN (CASE WHEN DeliverY = 'YTD' OR DeliverY = '" + year.Substring(2, 2).Trim() +
                                "' THEN ROUND(Amount*" + str_rate1 + ",0) ELSE ROUND(Amount*" + str_rate2 +
                                ",0) END) ELSE 0 END) AS '"
                                + dt.Rows[count][1] + "'";
                }
                else
                {
                    if (date.JudgeFirstMonth(month))
                        temp += " AND YEAR(TimeFlag)='" + preyear + "' AND MONTH(TimeFlag)='" + premonth + "'";
                    else
                        temp += " AND YEAR(TimeFlag)='" + year + "' AND MONTH(TimeFlag)='" + premonth + "'";
                    temp += " AND BookingY = '" + year.Substring(2, 2)
                            + "' THEN ROUND(Amount,0) ELSE 0 END) AS '"
                            + dt.Rows[count][1] + "'";
                }
            }
            temp += " FROM Bookings INNER JOIN Operation ON Bookings.OperationID = Operation.ID"
                    + " WHERE RSMID= " + rsmID + " AND SegmentID = " + segmentID + " AND SalesOrgID = " + salesOrgID
                    + " AND Operation.Deleted=0 ";

            if (!countryID.Trim().Equals("-1") && !countryID.Trim().Equals(""))
            {
                temp = temp + " AND [Bookings].CountryID = " + countryID;
            }
            else
            {
                temp += " AND [Bookings].CountryID IN (" + strSQL + ")";
            }

            //by yyan 20110526 w14 add start
            DataSet dtTimeFlag = date.getSetMeetingDate();
            temp += " AND [Bookings].TimeFlag = '" + dtTimeFlag.Tables[0].Rows[0][0] + "'";
            //by yyan 20110526 w14 add end 

            temp = temp + " GROUP BY Operation.AbbrL"
                   + " ORDER BY Operation.AbbrL ASC";
            sqlstr += temp;

            return sqlstr;
        }
        else
            return "";
    }


    /// <summary>
    /// Get Totle for PerProduct By Country SQL
    /// </summary>
    /// <param name="salesorgID">Sales Organization ID</param>
    /// <param name="dsProduct">DataSet For Product </param>
    /// <param name="segmentID">Segment ID</param>
    /// <param name="rsmAbbr">RSM Abbr</param>
    /// <param name="rsmID">RSM ID</param>
    /// <param name="bookingYear">Booking Year</param>
    /// <param name="deliverYear">Delivery Year</param>
    /// <param name="year">Year</param>
    /// <param name="month">Month</param>
    /// <param name="countryID">Country ID</param>
    /// <param name="convert_flag">true for converting to Euro, false for not converting</param>
    /// <param name="strSQL">country ids sql</param>
    /// <returns>SQL</returns>
    public string getBookingDataTotalByCountry(string salesorgID, DataSet dsProduct, string segmentID,
                                               string rsmAbbr, string rsmID, string bookingYear, string deliverYear,
                                               string year, string month,
                                               string countryID, bool convert_flag, string strSQL)
    {
        string str_currency = getCurrencyBySalesOrgID(salesorgID);
        double db_rate1 = sql.getRate(str_currency, true, year, month);
        double db_rate2 = sql.getRate(str_currency, false, year, month);
        string str_rate1 = db_rate1.ToString().Replace(',', '.');
        string str_rate2 = db_rate2.ToString().Replace(',', '.');

        var sqlSql = new StringBuilder();
        if (dsProduct != null)
        {
            sqlSql.AppendLine(" SELECT ");
            sqlSql.AppendLine("   ('" + rsmAbbr + "/'+Country.ISO_Code) AS Country ");
            for (int count = 0; count < dsProduct.Tables[0].Rows.Count; count++)
            {
                sqlSql.AppendLine("   ,SUM(CASE WHEN ProductID=" + dsProduct.Tables[0].Rows[count][0]);
                sqlSql.AppendLine("                  AND YEAR(TimeFlag)=" + year);
                sqlSql.AppendLine("                  AND MONTH(TimeFlag)=" + month);
                sqlSql.AppendLine("                  AND BookingY='" + bookingYear + "' ");
                sqlSql.AppendLine("                  AND DeliverY='" + deliverYear + "' ");
                if (convert_flag)
                {
                    if (string.Equals(deliverYear, year.Substring(2, 2).Trim()) || string.Equals(deliverYear, "YTD"))
                    {
                        sqlSql.AppendLine("             THEN ROUND(Amount*" + str_rate1 + ",0) ");
                    }
                    else
                    {
                        sqlSql.AppendLine("             THEN ROUND(Amount*" + str_rate2 + ",0) ");
                    }
                }
                else
                {
                    sqlSql.AppendLine("             THEN ROUND(Amount,0) ");
                }
                sqlSql.AppendLine("             ELSE 0 ");
                sqlSql.AppendLine("        END) AS '" + dsProduct.Tables[0].Rows[count][1] + "' ");
            }
            sqlSql.AppendLine(" FROM ");
            sqlSql.AppendLine("   [Bookings] ");
            sqlSql.AppendLine("   INNER JOIN SubRegion ON Bookings.CountryID=SubRegion.ID ");
            sqlSql.AppendLine("   INNER JOIN Country_SubRegion ON SubRegion.ID=Country_SubRegion.SubRegionID ");
            sqlSql.AppendLine("   INNER JOIN Country ON Country_SubRegion.CountryID=Country.ID ");
            sqlSql.AppendLine(" WHERE ");
            sqlSql.AppendLine("   SubRegion.Deleted=0 ");
            sqlSql.AppendLine("   AND Country_SubRegion.Deleted=0 ");
            sqlSql.AppendLine("   AND Country.Deleted=0 ");
            sqlSql.AppendLine("   AND RSMID=" + rsmID);
            sqlSql.AppendLine("   AND SegmentID=" + segmentID);
            sqlSql.AppendLine("   AND SalesOrgID=" + salesorgID);
            if (!string.Equals(countryID.Trim(), "-1") && !string.IsNullOrEmpty(countryID.Trim()))
            {
                sqlSql.AppendLine("   AND Bookings.CountryID=" + countryID);
            }
            else
            {
                sqlSql.AppendLine("   AND Bookings.CountryID IN (" + strSQL + ")");
            }
            DataSet dtTimeFlag = date.getSetMeetingDate();
            sqlSql.AppendLine("   AND [Bookings].TimeFlag='" + dtTimeFlag.Tables[0].Rows[0][0] + "' ");
            sqlSql.AppendLine(" GROUP BY ");
            sqlSql.AppendLine("   Country.ISO_Code  ");
            sqlSql.AppendLine(" ORDER BY ");
            sqlSql.AppendLine("   Country.ISO_Code ");
        }
        return sqlSql.ToString();
    }

    /// <summary>
    /// Get booking sales data total by product group by country.
    /// </summary>
    /// <param name="segmentID">Segment ID</param>
    /// <param name="productID">Product ID</param>
    /// <param name="rsmAbbr">RSM Abbr</param> 
    /// <param name="rsmID">RSM ID</param>
    /// <param name="salesorgID">Sales Organization ID</param>
    /// <param name="year">Year</param>
    /// <param name="month">Month</param>
    /// <param name="preyear">Preyear</param>
    /// <param name="countryID">Country ID</param>
    /// <param name="convertFlag">true for converting to Euro, false for not converting</param>
    /// <param name="countrySQL">country ids sql</param>
    /// <returns>SQL</returns>
    public string getBDTotalByProductGBCountry(string segmentID, string productID, string str_rsmAbbr, string RSMID,
                                               string salesOrgID, string year, string month, string preyear,
                                               string subRegionID, bool convertFlag, string countrySQL)
    {
        string currency = sql.getSalesOrgCurrency(salesOrgID);
        double db_rate1 = sql.getRate(currency, true, year, month);
        double db_rate2 = sql.getRate(currency, false, year, month);
        var strSQL = new StringBuilder();
        strSQL.AppendLine(" SELECT ");
        strSQL.AppendLine("   A.ISO_Code, ");
        strSQL.AppendLine("   ROUND(ISNULL(A.Amount,0),0) AS '" + year.Substring(2) + "', ");
        strSQL.AppendLine("   ROUND(ISNULL(B.Amount,0),0) AS BUD ");
        strSQL.AppendLine(" FROM ");
        strSQL.AppendLine("   (SELECT ");
        strSQL.AppendLine("     [Bookings].CountryID, ");
        strSQL.AppendLine("     ('" + str_rsmAbbr + "/'+Country.ISO_Code) AS 'ISO_Code', ");
        if (convertFlag)
        {
            strSQL.AppendLine("     SUM(CASE WHEN [Bookings].DeliverY='YTD' OR [Bookings].DeliverY='" +
                              year.Substring(2) + "' ");
            strSQL.AppendLine("              THEN Amount*" + db_rate1);
            strSQL.AppendLine("              ELSE Amount*" + db_rate2);
            strSQL.AppendLine("     END) AS Amount ");
        }
        else
        {
            strSQL.AppendLine("     SUM(Amount) AS Amount ");
        }
        strSQL.AppendLine("   FROM ");
        strSQL.AppendLine("     [Bookings] ");
        strSQL.AppendLine("     INNER JOIN [SubRegion] ON [Bookings].CountryID=[SubRegion].ID ");
        strSQL.AppendLine("     INNER JOIN [Country_SubRegion] ON [SubRegion].ID=[Country_SubRegion].SubRegionID ");
        strSQL.AppendLine("     INNER JOIN [Country] ON [Country_SubRegion].CountryID=[Country].ID ");
        strSQL.AppendLine("   WHERE ");
        strSQL.AppendLine("     [Bookings].RSMID=" + RSMID);
        strSQL.AppendLine("     AND [Bookings].SalesOrgID=" + salesOrgID);
        strSQL.AppendLine("     AND [Bookings].SegmentID=" + segmentID);
        if (string.IsNullOrEmpty(subRegionID) || string.Equals(subRegionID, "-1"))
        {
            strSQL.AppendLine("     AND [Bookings].CountryID IN (" + countrySQL + ")");
        }
        else
        {
            strSQL.AppendLine("     AND [Bookings].CountryID=" + subRegionID);
        }
        strSQL.AppendLine("     AND [Bookings].ProductID=" + productID);
        strSQL.AppendLine("     AND YEAR([Bookings].TimeFlag)=" + year);
        strSQL.AppendLine("     AND MONTH([Bookings].TimeFlag)=" + month);
        strSQL.AppendLine("     AND [Bookings].BookingY=" + year.Substring(2));
        strSQL.AppendLine("   GROUP BY ");
        strSQL.AppendLine("     [Bookings].CountryID, ");
        strSQL.AppendLine("     [Country].ISO_Code) AS A ");
        strSQL.AppendLine("   LEFT JOIN ");
        strSQL.AppendLine("   (SELECT ");
        strSQL.AppendLine("     [Bookings].CountryID, ");
        strSQL.AppendLine("     ('" + str_rsmAbbr + "/'+Country.ISO_Code) AS 'ISO_Code', ");
        if (convertFlag)
        {
            strSQL.AppendLine("     SUM(CASE WHEN [Bookings].DeliverY='YTD' OR [Bookings].DeliverY='" +
                              year.Substring(2) + "' ");
            strSQL.AppendLine("              THEN Amount*" + db_rate1);
            strSQL.AppendLine("              ELSE Amount*" + db_rate2);
            strSQL.AppendLine("     END) AS Amount ");
        }
        else
        {
            strSQL.AppendLine("     SUM(Amount) AS Amount ");
        }
        strSQL.AppendLine("   FROM ");
        strSQL.AppendLine("     [Bookings] ");
        strSQL.AppendLine("     INNER JOIN [SubRegion] ON [Bookings].CountryID=[SubRegion].ID ");
        strSQL.AppendLine("     INNER JOIN [Country_SubRegion] ON [SubRegion].ID=[Country_SubRegion].SubRegionID ");
        strSQL.AppendLine("     INNER JOIN [Country] ON [Country_SubRegion].CountryID=[Country].ID ");
        strSQL.AppendLine("   WHERE ");
        strSQL.AppendLine("     [Bookings].RSMID=" + RSMID);
        strSQL.AppendLine("     AND [Bookings].SalesOrgID=" + salesOrgID);
        strSQL.AppendLine("     AND [Bookings].SegmentID=" + segmentID);
        if (string.IsNullOrEmpty(subRegionID) || string.Equals(subRegionID, "-1"))
        {
            strSQL.AppendLine("     AND [Bookings].CountryID IN (" + countrySQL + ")");
        }
        else
        {
            strSQL.AppendLine("     AND [Bookings].CountryID=" + subRegionID);
        }
        strSQL.AppendLine("     AND [Bookings].ProductID=" + productID);
        strSQL.AppendLine("     AND YEAR([Bookings].TimeFlag)=" + preyear);
        strSQL.AppendLine("     AND MONTH([Bookings].TimeFlag)=03");
        strSQL.AppendLine("     AND [Bookings].BookingY=" + year.Substring(2));
        strSQL.AppendLine("   GROUP BY ");
        strSQL.AppendLine("     [Bookings].CountryID, ");
        strSQL.AppendLine("     [Country].ISO_Code) AS B ON A.CountryID=B.CountryID ");
        strSQL.AppendLine(" ORDER BY ");
        strSQL.AppendLine("   A.ISO_Code ");
        return strSQL.ToString();
    }

    /// <summary>
    /// Get Totle Booking Sales Data Group By Country
    /// </summary>
    /// <param name="salesOrgID">Sales Organization ID</param>
    /// <param name="segmentID">Segment ID</param>
    /// <param name="str_rsmAbbr">RSM Abbr</param>
    /// <param name="RSMID">RSM ID</param>
    /// <param name="year">Year</param>
    /// <param name="month">Month</param>
    /// <param name="preyear">Peryear</param>
    /// <param name="subRegionID">SubRegion ID</param>
    /// <param name="convertFlag">true for converting to Euro, false for not converting</param>
    /// <param name="countrySQL">country id or country ids sql</param>
    /// <returns>SQL</returns>
    public string getBDTotalThisYearGBCountry(DataSet dsProduct, string salesOrgID, string segmentID, string str_rsmAbbr,
                                              string RSMID, string year, string month, string subRegionID,
                                              bool convertFlag, string countrySQL)
    {
        string str_currency = getCurrencyBySalesOrgID(salesOrgID);
        double db_rate1 = sql.getRate(str_currency, false, year, month);
        string str_rate1 = db_rate1.ToString().Replace(',', '.');
        var selSql = new StringBuilder();
        if (dsProduct != null)
        {
            DataTable dt = dsProduct.Tables[0];
            selSql.AppendLine(" SELECT ");
            selSql.AppendLine("   ('" + str_rsmAbbr + "/'+Country.ISO_Code) AS Country ");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                selSql.AppendLine("   ,SUM(CASE WHEN ProductID=" + dt.Rows[i][0]);
                selSql.AppendLine("                  AND YEAR(TimeFlag)=" + year);
                selSql.AppendLine("                  AND MONTH(TimeFlag)=" + month);
                selSql.AppendLine("                  AND BookingY='" + year.Substring(2, 2) + "' ");
                if (convertFlag)
                {
                    selSql.AppendLine("             THEN ROUND(Amount*" + db_rate1 + ",0) ");
                }
                else
                {
                    selSql.AppendLine("             THEN ROUND(Amount,0) ");
                }
                selSql.AppendLine("             ELSE 0 ");
                selSql.AppendLine("        END) AS '" + dt.Rows[i][1] + "' ");
            }
            selSql.AppendLine(" FROM ");
            selSql.AppendLine("   Bookings ");
            selSql.AppendLine("   INNER JOIN SubRegion ON Bookings.CountryID=SubRegion.ID ");
            selSql.AppendLine("   INNER JOIN Country_SubRegion ON SubRegion.ID=Country_SubRegion.SubRegionID ");
            selSql.AppendLine("   INNER JOIN Country ON Country_SubRegion.CountryID=Country.ID ");
            selSql.AppendLine(" WHERE ");
            selSql.AppendLine("    RSMID=" + RSMID);
            selSql.AppendLine("    AND SegmentID=" + segmentID);
            selSql.AppendLine("    AND SalesOrgID=" + salesOrgID);
            if (string.Equals(subRegionID, "-1") || string.IsNullOrEmpty(subRegionID))
            {
                selSql.AppendLine("   AND [Bookings].CountryID IN (" + countrySQL + ")");
            }
            else
            {
                selSql.AppendLine("   AND [Bookings].CountryID=" + countrySQL);
            }
            selSql.AppendLine("   AND SubRegion.Deleted=0 ");
            selSql.AppendLine("   AND Country_SubRegion.Deleted=0 ");
            selSql.AppendLine("   AND Country.Deleted=0 ");
            DataSet dtTimeFlag = date.getSetMeetingDate();
            selSql.AppendLine("   AND Bookings.TimeFlag='" + dtTimeFlag.Tables[0].Rows[0][0] + "' ");
            selSql.AppendLine(" GROUP BY ");
            selSql.AppendLine("   Country.ISO_Code ");
            selSql.AppendLine(" ORDER BY ");
            selSql.AppendLine("   Country.ISO_Code ");
        }
        return selSql.ToString();
    }

    /// <summary>
    /// Get Next Year Totle Bookings Sales Data Group By Country
    /// </summary>
    /// <param name="salesOrgID">Sales Organization ID</param>
    /// <param name="dsProduct">Product</param>
    /// <param name="segmentID">Segment ID</param>
    /// <param name="rsmAbbr">RSM Abbr</param>
    /// <param name="rsmID">RSM ID</param>
    /// <param name="year">Year</param>
    /// <param name="month">Month</param>
    /// <param name="nextyear">Next Year</param>
    /// <param name="countryID">Country ID</param>
    /// <param name="convert_flag">true for converting to Euro, false for not converting</param>
    /// <param name="strSQL">counry ids sql</param>
    /// <returns>SQL</returns>
    public string getBDTotalNextYearGBCountry(string salesOrgID, DataSet dsProduct, string segmentID, string rsmAbbr,
                                              string rsmID, string year, string month, string nextyear, string countryID,
                                              bool convert_flag, string strSQL)
    {
        string str_currency = getCurrencyBySalesOrgID(salesOrgID);
        double db_rate2 = sql.getRate(str_currency, false, year, month);
        string str_rate2 = db_rate2.ToString().Replace(',', '.');
        var selSql = new StringBuilder();
        if (dsProduct != null)
        {
            DataTable dt = dsProduct.Tables[0];
            selSql.AppendLine(" SELECT ");
            selSql.AppendLine("   ('" + rsmAbbr + "/'+Country.ISO_Code) AS Country ");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                selSql.AppendLine("   ,SUM(CASE WHEN ProductID=" + dt.Rows[i][0]);
                selSql.AppendLine("                  AND YEAR(TimeFlag)=" + year);
                selSql.AppendLine("                  AND MONTH(TimeFlag)=" + month);
                selSql.AppendLine("                  AND BookingY='" + nextyear.Substring(2, 2) + "' ");
                if (convert_flag)
                {
                    selSql.AppendLine("             THEN ROUND(Amount*" + str_rate2 + ",0) ");
                }
                else
                {
                    selSql.AppendLine("             THEN ROUND(Amount,0) ");
                }
                selSql.AppendLine("             ELSE 0 ");
                selSql.AppendLine("        END) AS '" + dt.Rows[i][1] + "' ");
            }
            selSql.AppendLine(" FROM ");
            selSql.AppendLine("   Bookings ");
            selSql.AppendLine("   INNER JOIN SubRegion ON Bookings.CountryID=SubRegion.ID ");
            selSql.AppendLine("   INNER JOIN Country_SubRegion ON SubRegion.ID=Country_SubRegion.SubRegionID ");
            selSql.AppendLine("   INNER JOIN Country ON Country_SubRegion.CountryID=Country.ID ");
            selSql.AppendLine(" WHERE ");
            selSql.AppendLine("    RSMID=" + rsmID);
            selSql.AppendLine("    AND SegmentID=" + segmentID);
            selSql.AppendLine("    AND SalesOrgID=" + salesOrgID);
            if (string.Equals(countryID, "-1") || string.IsNullOrEmpty(countryID))
            {
                selSql.AppendLine("   AND [Bookings].CountryID IN (" + strSQL + ")");
            }
            else
            {
                selSql.AppendLine("   AND [Bookings].CountryID=" + countryID);
            }
            selSql.AppendLine("   AND SubRegion.Deleted=0 ");
            selSql.AppendLine("   AND Country_SubRegion.Deleted=0 ");
            selSql.AppendLine("   AND Country.Deleted=0 ");
            DataSet dtTimeFlag = date.getSetMeetingDate();
            selSql.AppendLine("   AND Bookings.TimeFlag='" + dtTimeFlag.Tables[0].Rows[0][0] + "' ");
            selSql.AppendLine(" GROUP BY ");
            selSql.AppendLine("   Country.ISO_Code ");
            selSql.AppendLine(" ORDER BY ");
            selSql.AppendLine("   Country.ISO_Code ");
        }
        return selSql.ToString();
    }

    /// <summary>
    /// Get Year VS PreYear Totle Bookings Sales Data Group By Country
    /// </summary>
    /// <param name="salesOrgID">Sales Organization ID</param>
    /// <param name="dsProduct">Product</param>
    /// <param name="segmentID">Segment ID</param>
    /// <param name="rsmAbbr">RSM Abbr</param>
    /// <param name="rsmID">RSM ID</param>
    /// <param name="year">Year</param>
    /// <param name="month">Month</param>
    /// <param name="preyear">Peryear</param>
    /// <param name="premonth">Premonth</param>
    /// <param name="countryID">Country ID</param>
    /// <param name="convert_flag">true for converting to Euro, false for not converting</param>
    /// <param name="strSQL">country ids sql</param>
    /// <returns>SQL</returns>
    public string getBDTotaByThislVSPreGBCountry(string salesOrgID, DataSet dsProduct, string segmentID, string rsmAbbr,
                                                 string rsmID, string year, string month, string preyear,
                                                 string premonth, string countryID, bool convert_flag, string strSQL)
    {
        string str_currency = getCurrencyBySalesOrgID(salesOrgID);
        double db_rate1 = sql.getRate(str_currency, true, year, premonth);
        double db_rate2 = sql.getRate(str_currency, false, year, premonth);
        double db_prerate2 = sql.getRate(str_currency, false, preyear, premonth);
        string str_rate1 = db_rate1.ToString().Replace(',', '.');
        string str_rate2 = db_rate2.ToString().Replace(',', '.');
        string str_prerate2 = db_rate2.ToString().Replace(',', '.');
        var selSql = new StringBuilder();
        if (dsProduct != null)
        {
            DataTable dt = dsProduct.Tables[0];

            selSql.AppendLine(" SELECT ");
            selSql.AppendLine("   ('" + rsmAbbr + "/'+Country.ISO_Code) AS Country ");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                selSql.AppendLine("   ,SUM(CASE WHEN ProductID=" + dt.Rows[i][0]);
                if (convert_flag)
                {
                    if (date.JudgeFirstMonth(month))
                    {
                        selSql.AppendLine("                  AND YEAR(TimeFlag)='" + preyear + "' ");
                        selSql.AppendLine("                  AND MONTH(TimeFlag)='" + premonth + "' ");
                        selSql.AppendLine("                  AND BookingY='" + year.Substring(2, 2) + "' ");
                        selSql.AppendLine("             THEN ROUND(Amount*" + str_prerate2 + ",0) ");
                        selSql.AppendLine("             ELSE 0 ");
                        selSql.AppendLine("        END) AS '" + dt.Rows[i][1] + "' ");
                    }
                    else
                    {
                        selSql.AppendLine("                  AND YEAR(TimeFlag)='" + year + "' ");
                        selSql.AppendLine("                  AND MONTH(TimeFlag)='" + premonth + "' ");
                        selSql.AppendLine("                  AND BookingY='" + year.Substring(2, 2) + "' ");
                        selSql.AppendLine("             THEN (CASE WHEN DeliverY='YTD' ");
                        selSql.AppendLine("                             OR DeliverY='" + year.Substring(2, 2) + "' ");
                        selSql.AppendLine("                        THEN ROUND(Amount*" + str_rate1 + ",0) ");
                        selSql.AppendLine("                        ELSE ROUND(Amount*" + str_rate2 + ",0) ");
                        selSql.AppendLine("                   END) ");
                        selSql.AppendLine("             ELSE 0 ");
                        selSql.AppendLine("        END) AS '" + dt.Rows[i][1] + "' ");
                    }
                }
                else
                {
                    if (date.JudgeFirstMonth(month))
                    {
                        selSql.AppendLine("                  AND YEAR(TimeFlag)='" + preyear + "' ");
                    }
                    else
                    {
                        selSql.AppendLine("                  AND YEAR(TimeFlag)='" + year + "' ");
                    }
                    selSql.AppendLine("                  AND MONTH(TimeFlag)='" + premonth + "' ");
                    selSql.AppendLine("                  AND BookingY='" + year.Substring(2, 2) + "' ");
                    selSql.AppendLine("             THEN ROUND(Amount,0) ");
                    selSql.AppendLine("             ELSE 0 ");
                    selSql.AppendLine("        END) AS '" + dt.Rows[i][1] + "' ");
                }
            }
            selSql.AppendLine(" FROM ");
            selSql.AppendLine("   Bookings ");
            selSql.AppendLine("   INNER JOIN SubRegion ON Bookings.CountryID=SubRegion.ID ");
            selSql.AppendLine("   INNER JOIN Country_SubRegion ON SubRegion.ID=Country_SubRegion.SubRegionID ");
            selSql.AppendLine("   INNER JOIN Country ON Country_SubRegion.CountryID=Country.ID ");
            selSql.AppendLine(" WHERE ");
            selSql.AppendLine("   RSMID=" + rsmID);
            selSql.AppendLine("   AND SegmentID=" + segmentID);
            selSql.AppendLine("   AND SalesOrgID=" + salesOrgID);
            if (string.Equals(countryID, "-1") || string.IsNullOrEmpty(countryID))
            {
                selSql.AppendLine("   AND [Bookings].CountryID IN (" + strSQL + ")");
            }
            else
            {
                selSql.AppendLine("   AND [Bookings].CountryID=" + countryID);
            }
            selSql.AppendLine("   AND SubRegion.Deleted=0 ");
            selSql.AppendLine("   AND Country_SubRegion.Deleted=0 ");
            selSql.AppendLine("   AND Country.Deleted=0 ");
            DataSet dtTimeFlag = date.getSetMeetingDate();
            selSql.AppendLine("   AND [Bookings].TimeFlag='" + dtTimeFlag.Tables[0].Rows[0][0] + "' ");
            selSql.AppendLine(" GROUP BY ");
            selSql.AppendLine("   Country.ISO_Code ");
            selSql.AppendLine(" ORDER BY ");
            selSql.AppendLine("   Country.ISO_Code ");
        }
        return selSql.ToString();
    }
    /// <summary>
    /// get booking sales data by booking year and deliver year.
    /// </summary>
    /// <param name="dsPro">products</param>
    /// <param name="segID">segment id</param>
    /// <param name="bookingY">booking year</param>
    /// <param name="deliverY">deliver year</param>
    /// <param name="salesOrgID">salesOrg id</param>
    /// <param name="year">year</param>
    /// <param name="month">month</param>
    /// <param name="convertFlag">true for converting to Euro, false for not converting</param>
    /// <returns>sql script</returns>
    public string getBDDataByYearSOSql(DataSet dsPro, string segID, string bookingY, string deliverY,
                                       string salesOrgID, string year, string month, string convertFlag)
    {
        string str_currency = getCurrencyBySalesOrgID(salesOrgID);
        double db_rate1 = sql.getRate(str_currency, true, year, month);
        double db_rate2 = sql.getRate(str_currency, false, year, month);
        string str_rate1 = db_rate1.ToString().Replace(',', '.');
        string str_rate2 = db_rate2.ToString().Replace(',', '.');

        string sqlstr;
        string temp = "";
        sqlstr = "SELECT [SubRegion].Name AS SubRegion,[Country].ISO_Code AS Country";
        for (int count = 0; count < dsPro.Tables[0].Rows.Count; count++)
        {
            if (string.Equals(convertFlag, "true"))
            {
                temp += " ,SUM(CASE WHEN ProductID = " + dsPro.Tables[0].Rows[count][0] + " AND YEAR(TimeFlag) = '" +
                        year + "' AND MONTH(TimeFlag) = '" + month
                        //+ "' AND [Bookings].BookingY='" + bookingY + "' AND DeliverY='" + deliverY + "' THEN (CASE WHEN DeliverY='YTD' OR DeliverY='" + year.Substring(2, 2)
                        //+ "' AND [Bookings].BookingY='" + bookingY + "' AND DeliverY='" + deliverY + "' THEN (CASE WHEN DeliverY='YTD' " // OR DeliverY='" //+ year.Substring(2, 2)
                        + "' AND [Bookings].BookingY='" + bookingY + "' AND DeliverY='" + deliverY +
                        "' THEN (CASE WHEN DeliverY='YTD'"
                        + " THEN ROUND(Amount*" + str_rate1 + ",0) ELSE ROUND(Amount*" + str_rate2 +
                        ",0) END ) ELSE 0 END) AS '"
                        + dsPro.Tables[0].Rows[count][1] + "'";
            }
            else
            {
                temp += " ,SUM(CASE WHEN ProductID = " + dsPro.Tables[0].Rows[count][0] + " AND YEAR(TimeFlag) = '" +
                        year + "' AND MONTH(TimeFlag) = '" + month + "' THEN (CASE WHEN [Bookings].BookingY='" +
                        bookingY + "' AND DeliverY='" + deliverY +
                        "' THEN ROUND(Amount,0) ELSE 0 END ) ELSE 0 END) AS '"
                        + dsPro.Tables[0].Rows[count][1] + "'";
            }
        }
        temp += " FROM [Bookings]"
                + " INNER JOIN [SubRegion] ON [Bookings].CountryID = [SubRegion].ID"
                + " INNER JOIN [Country_SubRegion] ON [Bookings].CountryID = [Country_SubRegion].SubRegionID"
                + " INNER JOIN [Country] ON [Country].ID = [Country_SubRegion].CountryID"
                + " WHERE SegmentID = " + segID
                + " AND SalesOrgID = " + salesOrgID
                + " AND SubRegion.Deleted=0 AND Country_SubRegion.Deleted=0 AND Country.Deleted=0 "
                + " GROUP BY SubRegion.Name,Country.ISO_Code"
                + " ORDER BY SubRegion.Name ASC";

        sqlstr += temp;
        return sqlstr;
    }

    public string getBDDataTotleByYearSOSql(string str_salesOrgabbr, DataSet dsPro, string segID, string bookingY,
                                            string deliverY,
                                            string salesOrgID, string year, string month, string convertFlag)
    {
        string str_currency = getCurrencyBySalesOrgID(salesOrgID);
        double db_rate1 = sql.getRate(str_currency, true, year, month);
        double db_rate2 = sql.getRate(str_currency, false, year, month);
        string str_rate1 = db_rate1.ToString().Replace(',', '.');
        string str_rate2 = db_rate2.ToString().Replace(',', '.');

        string sqlstr = "SELECT ('" + str_salesOrgabbr + "/'+[Operation].AbbrL) AS Operation";
        string temp = "";
        for (int count = 0; count < dsPro.Tables[0].Rows.Count; count++)
        {
            if (string.Equals(convertFlag, "true"))
            {
                temp += " ,SUM(CASE WHEN ProductID = " + dsPro.Tables[0].Rows[count][0] + " AND YEAR(TimeFlag) = '" +
                        year + "' AND MONTH(TimeFlag) = '" + month
                        //+ "' AND [Bookings].BookingY='" + bookingY + "' AND DeliverY='" + deliverY + "' THEN (CASE WHEN DeliverY='YTD' OR DeliverY='" + year.Substring(2, 2)
                        //+ "' AND [Bookings].BookingY='" + bookingY + "' AND DeliverY='" + deliverY + "' THEN (CASE WHEN DeliverY='YTD' or DeliverY='" + year.Substring(2, 2) //OR DeliverY='" + year.Substring(2, 2)
                        + "' AND [Bookings].BookingY='" + bookingY + "' AND DeliverY='" + deliverY +
                        "' THEN (CASE WHEN DeliverY='YTD' "
                        + " THEN ROUND(Amount*" + str_rate1 + ",0) ELSE ROUND(Amount*" + str_rate2 +
                        ",0) END ) ELSE 0 END) AS '"
                        + dsPro.Tables[0].Rows[count][1] + "'";
            }
            else
            {
                temp += " ,SUM(CASE WHEN ProductID = " + dsPro.Tables[0].Rows[count][0] + " AND YEAR(TimeFlag) = '" +
                        year + "' AND MONTH(TimeFlag) = '" + month + "'  AND BookingY='" + bookingY + "' AND DeliverY='" +
                        deliverY + "' THEN ROUND(Amount,0) ELSE 0 END) AS '"
                        + dsPro.Tables[0].Rows[count][1] + "'";
            }
        }
        temp += " FROM [Bookings] "
                + " INNER JOIN [Operation] ON [Bookings].OperationID = [Operation].ID"
                + " WHERE SegmentID = " + segID
                + " AND SalesOrgID = " + salesOrgID
                + " AND Operation.Deleted=0 "
                + " GROUP BY Operation.AbbrL"
                + " ORDER BY [Operation].AbbrL ASC";

        sqlstr += temp;
        return sqlstr;
    }

    public string getBDProdDataSOSql(string salesOrgID, string year, string month, string productID, string preyear,
                                     string segmentID, string convertFlag)
    {
        string str_currency = getCurrencyBySalesOrgID(salesOrgID);
        double db_rate1 = sql.getRate(str_currency, true, year, month);
        double db_rate2 = sql.getRate(str_currency, false, year, month);
        string str_rate1 = db_rate1.ToString().Replace(',', '.');
        string str_rate2 = db_rate2.ToString().Replace(',', '.');

        string sqlstr = "SELECT [SubRegion].Name AS SubRegion";
        if (string.Equals(convertFlag, "true"))
        {
            //sqlstr += ",SUM(CASE WHEN ProductID = " + productID + " AND [BookingY]='" + year.Substring(2, 2)
            // + "' AND YEAR(TimeFlag)=" + year + " AND MONTH(TimeFlag)=" + month
            // + " THEN (CASE WHEN DeliverY='YTD' OR DeliverY='" + year.Substring(2, 2)
            // + "' THEN ROUND(Amount*" + str_rate1 + ",0) ELSE ROUND(Amount*" + str_rate2 + ",0) END) ELSE 0 END) AS '" + year.Substring(2, 2) + "'"
            // + ",SUM(CASE WHEN ProductID = " + productID + " AND [BookingY]='" + year.Substring(2, 2)
            // + "' AND YEAR(TimeFlag)=" + preyear + " AND MONTH(TimeFlag)=03 THEN (CASE WHEN DeliverY='YTD' OR DeliverY='" + year.Substring(2, 2)
            // + "' THEN ROUND(Amount*" + str_rate1 + ",0) ELSE ROUND(Amount*" + str_rate2 + ",0) END) ELSE 0 END) AS BUD FROM [Bookings] ";

            sqlstr += ",SUM(CASE WHEN ProductID = " + productID + " AND [BookingY]='" + year.Substring(2, 2)
                      + "' AND YEAR(TimeFlag)=" + year + " AND MONTH(TimeFlag)=" + month
                      + " THEN (CASE WHEN DeliverY='YTD'"
                      + " THEN ROUND(Amount*" + str_rate1 + ",0) ELSE ROUND(Amount*" + str_rate2 +
                      ",0) END) ELSE 0 END) AS '" + year.Substring(2, 2) + "'"
                      + ",SUM(CASE WHEN ProductID = " + productID + " AND [BookingY]='" + year.Substring(2, 2)
                      + "' AND YEAR(TimeFlag)=" + preyear + " AND MONTH(TimeFlag)=03 THEN (CASE WHEN DeliverY='YTD' "
                      + " THEN ROUND(Amount*" + str_rate1 + ",0) ELSE ROUND(Amount*" + str_rate2 +
                      ",0) END) ELSE 0 END) AS BUD FROM [Bookings] ";
        }
        else
        {
            sqlstr += ",SUM(CASE WHEN ProductID = " + productID + " AND [BookingY]='" + year.Substring(2, 2) +
                      "' AND YEAR(TimeFlag)=" + year + " AND MONTH(TimeFlag)=" + month +
                      " THEN ROUND(Amount,0) ELSE 0 END) AS '" + year.Substring(2, 2) + "'"
                      + ",SUM(CASE WHEN ProductID = " + productID + " AND [BookingY]='" + year.Substring(2, 2) +
                      "' AND YEAR(TimeFlag)=" + preyear +
                      " AND MONTH(TimeFlag)=03 THEN ROUND(Amount,0) ELSE 0 END) AS BUD FROM [Bookings] ";
        }
        sqlstr += " INNER JOIN [SubRegion] ON [Bookings].CountryID = [SubRegion].ID"
                  + " INNER JOIN [Country_SubRegion] ON [Bookings].CountryID = [Country_SubRegion].SubRegionID"
                  + " INNER JOIN [Country] ON [Country].ID = [Country_SubRegion].CountryID"
                  + " WHERE SegmentID = " + segmentID
                  + " AND SalesOrgID = " + salesOrgID
                  + " AND SubRegion.Deleted=0 AND Country_SubRegion.Deleted=0 AND Country.Deleted=0 "
                  + " GROUP BY [SubRegion].Name"
                  + " ORDER BY [SubRegion].Name ASC";
        return sqlstr;
    }

    public string getBDProdDataTotleSOSql(string str_salesOrgabbr, string salesOrgID, string year, string month,
                                          string productID, string preyear, string segmentID, string convertFlag)
    {
        string str_currency = getCurrencyBySalesOrgID(salesOrgID);
        double db_rate1 = sql.getRate(str_currency, true, year, month);
        double db_rate2 = sql.getRate(str_currency, false, year, month);
        string str_rate1 = db_rate1.ToString().Replace(',', '.');
        string str_rate2 = db_rate2.ToString().Replace(',', '.');

        string sqlstr = "SELECT ('" + str_salesOrgabbr + "/'+[Operation].AbbrL) AS Operation";
        if (string.Equals(convertFlag, "true"))
        {
            //sqlstr += ",SUM(CASE WHEN ProductID = " + productID + " AND [BookingY]='" + year.Substring(2, 2)
            // + "' AND YEAR(TimeFlag)=" + year + " AND MONTH(TimeFlag)=" + month
            // + " THEN (CASE WHEN DeliverY='YTD' OR DeliverY='" + year.Substring(2, 2)
            // + "' THEN ROUND(Amount*" + str_rate1 + ",0) ELSE ROUND(Amount*" + str_rate2 + ",0) END) ELSE 0 END) AS '" + year.Substring(2, 2) + "'"
            // + ",SUM(CASE WHEN ProductID = " + productID + " AND [BookingY]='" + year.Substring(2, 2)
            // + "' AND YEAR(TimeFlag)=" + preyear + " AND MONTH(TimeFlag)=03 THEN (CASE WHEN DeliverY='YTD' OR DeliverY='" + year.Substring(2, 2)
            // + "' THEN ROUND(Amount*" + str_rate1 + ",0) ELSE ROUND(Amount*" + str_rate2 + ",0) END) ELSE 0 END) AS BUD FROM [Bookings] ";

            sqlstr += ",SUM(CASE WHEN ProductID = " + productID + " AND [BookingY]='" + year.Substring(2, 2)
                      + "' AND YEAR(TimeFlag)=" + year + " AND MONTH(TimeFlag)=" + month
                      + " THEN (CASE WHEN DeliverY='YTD' "
                      + " THEN ROUND(Amount*" + str_rate1 + ",0) ELSE ROUND(Amount*" + str_rate2 +
                      ",0) END) ELSE 0 END) AS '" + year.Substring(2, 2) + "'"
                      + ",SUM(CASE WHEN ProductID = " + productID + " AND [BookingY]='" + year.Substring(2, 2)
                      + "' AND YEAR(TimeFlag)=" + preyear + " AND MONTH(TimeFlag)=03 THEN (CASE WHEN DeliverY='YTD' "
                      + " THEN ROUND(Amount*" + str_rate1 + ",0) ELSE ROUND(Amount*" + str_rate2 +
                      ",0) END) ELSE 0 END) AS BUD FROM [Bookings] ";
        }
        else
        {
            sqlstr += ",SUM(CASE WHEN ProductID = " + productID + " AND [BookingY]='" + year.Substring(2, 2) +
                      "' AND YEAR(TimeFlag)=" + year + " AND MONTH(TimeFlag)=" + month +
                      " THEN ROUND(Amount,0) ELSE 0 END) AS '" + year.Substring(2, 2) + "'"
                      + ",SUM(CASE WHEN ProductID = " + productID + " AND [BookingY]='" + year.Substring(2, 2) +
                      "' AND YEAR(TimeFlag)=" + preyear +
                      " AND MONTH(TimeFlag)=03 THEN ROUND(Amount,0) ELSE 0 END) AS BUD FROM [Bookings] ";
        }
        sqlstr += " INNER JOIN Operation ON Bookings.OperationID=Operation.ID"
                  + " WHERE SegmentID = " + segmentID
                  + " AND SalesOrgID = " + salesOrgID
                  + " AND Operation.Deleted=0 "
                  + " GROUP BY Operation.AbbrL"
                  + " ORDER BY Operation.AbbrL ASC";
        return sqlstr;
    }
    /// <summary>
    /// get booking sales data by year.
    /// </summary>
    /// <param name="salesOrgID">salesOrg id</param>
    /// <param name="year">year</param>
    /// <param name="month">month</param>
    /// <param name="preyear">pre year</param>
    /// <param name="segmentID">segment id</param>
    /// <param name="convertFlag">true for converting to Euro, false for not converting.</param>
    /// <returns>sql script</returns>
    public string getBDDataByYearSOSql(string salesOrgID, string year, string month, string preyear, string segmentID,
                                       string convertFlag)
    {
        string str_currency = getCurrencyBySalesOrgID(salesOrgID);
        double db_rate1 = sql.getRate(str_currency, true, year, month);
        double db_rate2 = sql.getRate(str_currency, false, year, month);
        string str_rate1 = db_rate1.ToString().Replace(',', '.');
        string str_rate2 = db_rate2.ToString().Replace(',', '.');

        string sqlstr = "SELECT SubRegion.Name AS SubRegion";
        if (string.Equals(convertFlag, "true"))
        {
            //sqlstr += " ,SUM(CASE WHEN YEAR(TimeFlag)=" + year + " AND BookingY='" + year.Substring(2, 2)
            //    + "' AND MONTH(TimeFlag)=" + month + " THEN (CASE WHEN DeliverY='YTD' OR DeliverY='" + year.Substring(2, 2)
            // + "' THEN ROUND(Amount*" + str_rate1 + ",0) ELSE ROUND(Amount*" + str_rate2 + ",0) END) ELSE 0 END) AS '" + year.Substring(2, 2) + "'"
            //    + " ,SUM(CASE WHEN YEAR(TimeFlag)=" + preyear + " AND BookingY='" + year.Substring(2, 2)
            //    + "' AND MONTH(TimeFlag) = '03' THEN (CASE WHEN DeliverY='YTD' OR DeliverY='" + year.Substring(2, 2)
            // + "' THEN ROUND(Amount*" + str_rate1 + ",0) ELSE ROUND(Amount*" + str_rate2 + ",0) END) ELSE 0 END) AS BUD ";

            sqlstr += " ,SUM(CASE WHEN YEAR(TimeFlag)=" + year + " AND BookingY='" + year.Substring(2, 2)
                      + "' AND MONTH(TimeFlag)=" + month + " THEN (CASE WHEN DeliverY='YTD' "
                      + " THEN ROUND(Amount*" + str_rate1 + ",0) ELSE ROUND(Amount*" + str_rate2 +
                      ",0) END) ELSE 0 END) AS '" + year.Substring(2, 2) + "'"
                      + " ,SUM(CASE WHEN YEAR(TimeFlag)=" + preyear + " AND BookingY='" + year.Substring(2, 2)
                      + "' AND MONTH(TimeFlag) = '03' THEN (CASE WHEN DeliverY='YTD' "
                      + " THEN ROUND(Amount*" + str_rate1 + ",0) ELSE ROUND(Amount*" + str_rate2 +
                      ",0) END) ELSE 0 END) AS BUD ";
        }
        else
        {
            sqlstr += " ,SUM(CASE WHEN YEAR(TimeFlag)=" + year + " AND BookingY='" + year.Substring(2, 2) +
                      "' AND MONTH(TimeFlag)=" + month + " THEN ROUND(Amount,0) ELSE 0 END) AS '" + year.Substring(2, 2) +
                      "'"
                      + " ,SUM(CASE WHEN YEAR(TimeFlag)=" + preyear + " AND BookingY='" + year.Substring(2, 2) +
                      "' AND MONTH(TimeFlag) = '03' THEN ROUND(Amount,0) ELSE 0 END) AS BUD ";
        }
        sqlstr += " FROM [Bookings] "
                  + " INNER JOIN [SubRegion] ON [Bookings].CountryID = [SubRegion].ID"
                  + " INNER JOIN [Country_SubRegion] ON [Bookings].CountryID = [Country_SubRegion].SubRegionID"
                  + " INNER JOIN [Country] ON [Country].ID = [Country_SubRegion].CountryID"
                  + " WHERE SegmentID='" + segmentID + "'"
                  + " AND SalesOrgID = " + salesOrgID
                  + " AND SubRegion.Deleted=0 AND Country_SubRegion.Deleted=0 AND Country.Deleted=0 "
                  + " GROUP BY SubRegion.Name"
                  + " ORDER BY SubRegion.Name ASC";
        return sqlstr;
    }
    /// <summary>
    /// get booking sales data totle by year.
    /// </summary>
    /// <param name="str_salesOrgabbr">salesOrg abbr</param>
    /// <param name="salesOrgID">salesOrg id</param>
    /// <param name="year">year</param>
    /// <param name="month">month</param>
    /// <param name="preyear">pre year</param>
    /// <param name="segmentID">segment id</param>
    /// <param name="convertFlag">true for converting to Euro, false for not converting.</param>
    /// <returns></returns>
    public string getBDDataTotleByYearSOSql(string str_salesOrgabbr, string salesOrgID, string year, string month,
                                            string preyear, string segmentID, string convertFlag)
    {
        string str_currency = getCurrencyBySalesOrgID(salesOrgID);
        double db_rate1 = sql.getRate(str_currency, true, year, month);
        double db_rate2 = sql.getRate(str_currency, false, year, month);
        string str_rate1 = db_rate1.ToString().Replace(',', '.');
        string str_rate2 = db_rate2.ToString().Replace(',', '.');

        string sqlstr = "SELECT ('" + str_salesOrgabbr + "/'+[Operation].AbbrL) AS Operation";
        if (string.Equals(convertFlag, "true"))
        {
            //sqlstr += " ,SUM(CASE WHEN YEAR(TimeFlag)=" + year + " AND BookingY='" + year.Substring(2, 2)
            //    + "' AND MONTH(TimeFlag)=" + month + " THEN (CASE WHEN DeliverY='YTD' OR DeliverY='" + year.Substring(2, 2)
            // + "' THEN ROUND(Amount*" + str_rate1 + ",0) ELSE ROUND(Amount*" + str_rate2 + ",0) END) ELSE 0 END) AS '" + year.Substring(2, 2) + "'"
            //+ " ,SUM(CASE WHEN YEAR(TimeFlag)=" + preyear + " AND BookingY='"
            //+ year.Substring(2, 2) + "' AND MONTH(TimeFlag)=03 THEN (CASE WHEN DeliverY='YTD' OR DeliverY='" + year.Substring(2, 2)
            // + "' THEN ROUND(Amount*" + str_rate1 + ",0) ELSE ROUND(Amount*" + str_rate2 + ",0) END) ELSE 0 END) AS BUD ";

            sqlstr += " ,SUM(CASE WHEN YEAR(TimeFlag)=" + year + " AND BookingY='" + year.Substring(2, 2)
                      + "' AND MONTH(TimeFlag)=" + month + " THEN (CASE WHEN DeliverY='YTD' "
                      + " THEN ROUND(Amount*" + str_rate1 + ",0) ELSE ROUND(Amount*" + str_rate2 +
                      ",0) END) ELSE 0 END) AS '" + year.Substring(2, 2) + "'"
                      + " ,SUM(CASE WHEN YEAR(TimeFlag)=" + preyear + " AND BookingY='"
                      + year.Substring(2, 2) + "' AND MONTH(TimeFlag)=03 THEN (CASE WHEN DeliverY='YTD' "
                      + " THEN ROUND(Amount*" + str_rate1 + ",0) ELSE ROUND(Amount*" + str_rate2 +
                      ",0) END) ELSE 0 END) AS BUD ";
        }
        else
        {
            sqlstr += " ,SUM(CASE WHEN YEAR(TimeFlag)=" + year + " AND BookingY='" + year.Substring(2, 2) +
                      "' AND MONTH(TimeFlag)=" + month + " THEN ROUND(Amount,0) ELSE 0 END) AS '" + year.Substring(2, 2) +
                      "'"
                      + " ,SUM(CASE WHEN YEAR(TimeFlag)=" + preyear + " AND BookingY='" + year.Substring(2, 2) +
                      "' AND MONTH(TimeFlag)=03 THEN ROUND(Amount,0) ELSE 0 END) AS BUD ";
        }
        sqlstr += " FROM [Bookings] "
                  + " INNER JOIN [Operation] ON [Bookings].OperationID=[Operation].ID"
                  + " WHERE SegmentID='" + segmentID + "'"
                  + " AND SalesOrgID = " + salesOrgID
                  + " AND Operation.Deleted=0 "
                  + " GROUP BY [Operation].AbbrL"
                  + " ORDER BY [Operation].AbbrL ASC";
        return sqlstr;
    }
    /// <summary>
    /// get booking sales data by next year.
    /// </summary>
    /// <param name="dsPro">product</param>
    /// <param name="segmentID">segment id</param>
    /// <param name="salesOrgID">salesOrg id</param>
    /// <param name="year">year</param>
    /// <param name="month">month</param>
    /// <param name="nextyear">next year</param>
    /// <param name="convertFlag">true for converting to Euro, false for not converting.</param>
    /// <returns>sql script</returns>
    public string getBDDataByNextYearSOSql(DataSet dsPro, string segmentID, string salesOrgID,
                                           string year, string month, string nextyear, string convertFlag)
    {
        string str_currency = getCurrencyBySalesOrgID(salesOrgID);
        double db_rate1 = sql.getRate(str_currency, true, year, month);
        double db_rate2 = sql.getRate(str_currency, false, year, month);
        string str_rate1 = db_rate1.ToString().Replace(',', '.');
        string str_rate2 = db_rate2.ToString().Replace(',', '.');

        DataTable dt = dsPro.Tables[0];
        string sqlstr;
        string temp = "";
        sqlstr = "SELECT SubRegion.Name AS SubRegion";
        for (int count = 0; count < dt.Rows.Count; count++)
        {
            if (string.Equals(convertFlag, "true"))
            {
                temp += " ,SUM(CASE WHEN ProductID = " + dt.Rows[count][0] + " AND BookingY='" +
                        nextyear.Substring(2, 2) + "' AND YEAR(TimeFlag)=" + year + " AND MONTH(TimeFlag)=" + month +
                        " THEN ROUND(Amount*" + str_rate2 + ",0) ELSE 0 END) AS '"
                        + dt.Rows[count][1] + "'";
            }
            else
            {
                temp += " ,SUM(CASE WHEN ProductID = " + dt.Rows[count][0] + " AND BookingY='" +
                        nextyear.Substring(2, 2) + "' AND YEAR(TimeFlag)=" + year + " AND MONTH(TimeFlag)=" + month +
                        " THEN ROUND(Amount,0) ELSE 0 END) AS '"
                        + dt.Rows[count][1] + "'";
            }
        }
        temp += " FROM Bookings INNER JOIN SubRegion ON Bookings.CountryID=SubRegion.ID"
                + " INNER JOIN [Country_SubRegion] ON [Bookings].CountryID = [Country_SubRegion].SubRegionID"
                + " INNER JOIN [Country] ON [Country].ID = [Country_SubRegion].CountryID"
                + " WHERE SegmentID='" + segmentID + "'"
                + " AND SalesOrgID = " + salesOrgID
                + " AND SubRegion.Deleted=0 AND Country_SubRegion.Deleted=0 AND Country.Deleted=0 "
                + " GROUP BY SubRegion.Name"
                + " ORDER BY SubRegion.Name ASC";

        sqlstr += temp;
        return sqlstr;
    }
    /// <summary>
    /// get booking sales data totle by next year
    /// </summary>
    /// <param name="str_salesOrgabbr">salesOrg abbr</param>
    /// <param name="dsPro">products</param>
    /// <param name="segmentID">segment id</param>
    /// <param name="salesOrgID">salesOrg id</param>
    /// <param name="year">year</param>
    /// <param name="month">month</param>
    /// <param name="nextyear">next year</param>
    /// <param name="convertFlag">true for converting to Euro, false for not converting.</param>
    /// <returns>sql script</returns>
    public string getBDDataTotleByNextYearSOSql(string str_salesOrgabbr, DataSet dsPro, string segmentID,
                                                string salesOrgID, string year, string month, string nextyear,
                                                string convertFlag)
    {
        string str_currency = getCurrencyBySalesOrgID(salesOrgID);
        double db_rate1 = sql.getRate(str_currency, true, year, month);
        double db_rate2 = sql.getRate(str_currency, false, year, month);
        string str_rate1 = db_rate1.ToString().Replace(',', '.');
        string str_rate2 = db_rate2.ToString().Replace(',', '.');

        DataTable dt = dsPro.Tables[0];
        string sqlstr = "SELECT ('" + str_salesOrgabbr + "/'+[Operation].AbbrL) AS Operation";
        string temp = "";
        for (int count = 0; count < dt.Rows.Count; count++)
        {
            if (string.Equals(convertFlag, "true"))
            {
                temp += " ,SUM(CASE WHEN ProductID = " + dt.Rows[count][0] + " AND BookingY='" +
                        nextyear.Substring(2, 2) + "' AND YEAR(TimeFlag)=" + year + " AND MONTH(TimeFlag)=" + month +
                        " THEN ROUND(Amount*" + str_rate2 + ",0) ELSE 0 END) AS '"
                        + dt.Rows[count][1] + "'";
            }
            else
            {
                temp += " ,SUM(CASE WHEN ProductID = " + dt.Rows[count][0] + " AND BookingY='" +
                        nextyear.Substring(2, 2) + "' AND YEAR(TimeFlag)=" + year + " AND MONTH(TimeFlag)=" + month +
                        " THEN ROUND(Amount,0) ELSE 0 END) AS '"
                        + dt.Rows[count][1] + "'";
            }
        }
        temp += " FROM Bookings INNER JOIN Operation ON Bookings.OperationID = Operation.ID"
                + " WHERE SegmentID='" + segmentID + "'"
                + " AND SalesOrgID = " + salesOrgID
                + " AND Operation.Deleted=0 "
                + " GROUP BY Operation.AbbrL"
                + " ORDER BY Operation.AbbrL ASC";

        sqlstr += temp;
        return sqlstr;
    }
    /// <summary>
    /// get booking sales data by year VS next year.
    /// </summary>
    /// <param name="dsPro">products</param>
    /// <param name="segmentID">segment id</param>
    /// <param name="str_salesOrgID">salesOrg id</param>
    /// <param name="year">year</param>
    /// <param name="month">month</param>
    /// <param name="preyear">pre year</param>
    /// <param name="premonth">premonth</param>
    /// <param name="convertFlag">true for converting to Euro, false for not converting.</param>
    /// <returns>sql script</returns>
    public string getBDDataByYearVSNextYearSOSql(DataSet dsPro, string segmentID, string str_salesOrgID,
                                                 string year, string month, string preyear, string premonth,
                                                 string convertFlag)
    {
        string str_currency = getCurrencyBySalesOrgID(str_salesOrgID);
        double db_rate1 = sql.getRate(str_currency, true, year, premonth);
        double db_rate2 = sql.getRate(str_currency, false, year, premonth);
        double db_prerate2 = sql.getRate(str_currency, false, preyear, premonth);
        string str_rate1 = db_rate1.ToString().Replace(',', '.');
        string str_rate2 = db_rate2.ToString().Replace(',', '.');
        string str_prerate2 = db_rate2.ToString().Replace(',', '.');

        DataTable dt = dsPro.Tables[0];
        string sqlstr = "SELECT SubRegion.Name AS SubRegion,Country.ISO_Code AS Country";
        string temp = "";
        for (int count = 0; count < dt.Rows.Count; count++)
        {
            temp += " ,SUM(CASE WHEN ProductID = " + dt.Rows[count][0];
            if (string.Equals(convertFlag, "true"))
            {
                if (date.JudgeFirstMonth(month))
                    temp += " AND YEAR(TimeFlag)='" + preyear + "' AND MONTH(TimeFlag)='" + premonth + "'"
                            + " AND BookingY = '" + year.Substring(2, 2)
                            + "' THEN ROUND(Amount*" + str_prerate2 + ",0) ELSE 0 END) AS '"
                            + dt.Rows[count][1] + "'";
                else
                    temp += " AND YEAR(TimeFlag)='" + year + "' AND MONTH(TimeFlag)='" + premonth + "'"
                            + " AND BookingY = '" + year.Substring(2, 2)
                            + "' THEN (CASE WHEN DeliverY = 'YTD' OR DeliverY = '" + year.Substring(2, 2).Trim()
                            + "' THEN ROUND(Amount*" + str_rate1 + ",0) ELSE ROUND(Amount*" + str_rate2 +
                            ",0) END) ELSE 0 END) AS '"
                            + dt.Rows[count][1] + "'";
            }
            else
            {
                if (date.JudgeFirstMonth(month))
                    temp += " AND YEAR(TimeFlag)='" + preyear + "' AND MONTH(TimeFlag)='" + premonth + "'"
                            + " AND BookingY = '" + year.Substring(2, 2)
                            + "' THEN ROUND(Amount,0) ELSE 0 END) AS '"
                            + dt.Rows[count][1] + "'";
                else
                    temp += " AND YEAR(TimeFlag)='" + year + "' AND MONTH(TimeFlag)='" + premonth + "'"
                            + " AND BookingY = '" + year.Substring(2, 2)
                            + "' THEN ROUND(Amount,0) ELSE 0 END) AS '"
                            + dt.Rows[count][1] + "'";
            }
        }

        temp += " FROM Bookings INNER JOIN SubRegion ON Bookings.CountryID=SubRegion.ID"
                + " INNER JOIN [Country_SubRegion] ON [Bookings].CountryID = [Country_SubRegion].SubRegionID"
                + " INNER JOIN [Country] ON [Country].ID = [Country_SubRegion].CountryID"
                + " WHERE SegmentID='" + segmentID + "'"
                + " AND SalesOrgID = " + str_salesOrgID
                + " AND SubRegion.Deleted=0 AND Country_SubRegion.Deleted=0 AND Country.Deleted=0 "
                + " GROUP BY SubRegion.Name,Country.ISO_Code"
                + " ORDER BY SubRegion.Name,Country.ISO_Code ASC";

        sqlstr += temp;
        return sqlstr;
    }
    /// <summary>
    /// get booking sales data total by year VS next year.
    /// </summary>
    /// <param name="str_salesOrgabbr">salesOrg abbr</param>
    /// <param name="dsPro">products</param>
    /// <param name="segmentID">segment id</param>
    /// <param name="str_salesOrgID">salesOrg id</param>
    /// <param name="year">year</param>
    /// <param name="month">month</param>
    /// <param name="preyear">preyear</param>
    /// <param name="premonth">premonth</param>
    /// <param name="convertFlag">true for converting to Euro, false for not converting.</param>
    /// <returns>sql script</returns>
    public string getBDDataTotleByYearVSNextYearSOSql(string str_salesOrgabbr, DataSet dsPro, string segmentID,
                                                      string str_salesOrgID,
                                                      string year, string month, string preyear, string premonth,
                                                      string convertFlag)
    {
        string str_currency = getCurrencyBySalesOrgID(str_salesOrgID);
        double db_rate1 = sql.getRate(str_currency, true, year, premonth);
        double db_rate2 = sql.getRate(str_currency, false, year, premonth);
        double db_prerate2 = sql.getRate(str_currency, false, preyear, premonth);
        string str_rate1 = db_rate1.ToString().Replace(',', '.');
        string str_rate2 = db_rate2.ToString().Replace(',', '.');
        string str_prerate2 = db_rate2.ToString().Replace(',', '.');

        DataTable dt = dsPro.Tables[0];
        string sqlstr = "SELECT ('" + str_salesOrgabbr + "/'+[Operation].AbbrL) AS Operation";
        string temp = "";
        for (int count = 0; count < dt.Rows.Count; count++)
        {
            temp += " ,SUM(CASE WHEN ProductID = " + dt.Rows[count][0];
            if (string.Equals(convertFlag, "true"))
            {
                if (date.JudgeFirstMonth(month))
                    temp += " AND YEAR(TimeFlag)='" + preyear + "' AND MONTH(TimeFlag)='" + premonth + "'"
                            + " AND BookingY = '" + year.Substring(2, 2)
                            + "' THEN ROUND(Amount*" + str_prerate2 + ",0) ELSE 0 END) AS '"
                            + dt.Rows[count][1] + "'";
                else
                    temp += " AND YEAR(TimeFlag)='" + year + "' AND MONTH(TimeFlag)='" + premonth + "'"
                            + " AND BookingY = '" + year.Substring(2, 2)
                            + "' THEN (CASE WHEN DeliverY = 'YTD' OR DeliverY = '" + year.Substring(2, 2).Trim()
                            + "' THEN ROUND(Amount*" + str_rate1 + ",0) ELSE ROUND(Amount*" + str_rate2 +
                            ",0) END) ELSE 0 END) AS '"
                            + dt.Rows[count][1] + "'";
            }
            else
            {
                if (date.JudgeFirstMonth(month))
                    temp += " AND YEAR(TimeFlag)='" + preyear + "' AND MONTH(TimeFlag)='" + premonth + "'"
                            + " AND BookingY = '" + year.Substring(2, 2)
                            + "' THEN ROUND(Amount,0) ELSE 0 END) AS '"
                            + dt.Rows[count][1] + "'";
                else
                    temp += " AND YEAR(TimeFlag)='" + year + "' AND MONTH(TimeFlag)='" + premonth + "'"
                            + " AND BookingY = '" + year.Substring(2, 2)
                            + "' THEN ROUND(Amount,0) ELSE 0 END) AS '"
                            + dt.Rows[count][1] + "'";
            }
        }
        temp += " FROM Bookings INNER JOIN Operation ON Bookings.OperationID = Operation.ID"
                + " WHERE SegmentID='" + segmentID + "'"
                + " AND SalesOrgID = " + str_salesOrgID
                + " AND Operation.Deleted=0 "
                + " GROUP BY Operation.AbbrL";

        sqlstr += temp;
        return sqlstr;
    }

    /// <summary>
    /// Get booking sales data by bookingY and deliverY
    /// </summary>
    /// <param name="RSMID">RSM ID</param>
    /// <param name="salesOrgID">Sales organization ID</param>
    /// <param name="segmentID">Segment ID</param>
    /// <param name="subregionID">Subregion ID</param>
    /// <param name="year">Year</param>
    /// <param name="month">Month</param>
    /// <param name="strSQL">Country SQL</param>
    /// <returns>SQL</returns>
    public string getBookingSalesDataByBD(string RSMID, string salesOrgID,
                                          string segmentID, string subregionID, string year, string month, string strSQL)
    {
        var sql = new StringBuilder();
        sql.AppendLine(" SELECT ");
        sql.AppendLine("   [Bookings].CountryID AS SubRegionID, ");
        sql.AppendLine("   [Bookings].CustomerID, ");
        sql.AppendLine("   [Bookings].ProjectID, ");
        sql.AppendLine("   [Bookings].SalesChannelID, ");
        sql.AppendLine("   [Bookings].ProductID, ");
        sql.AppendLine("   [Bookings].OperationID, ");
        sql.AppendLine("   [Bookings].BookingY, ");
        sql.AppendLine("   [Bookings].DeliverY, ");
        sql.AppendLine(" [Bookings].RecordID,");
        sql.AppendLine("   [SubRegion].Name AS SubRegion, ");
        sql.AppendLine("   [CustomerName].Name AS Customer, ");
        sql.AppendLine("   [Project].Name AS Project, ");
        sql.AppendLine("   [SalesChannel].Name AS SalesChannel, ");
        sql.AppendLine("   [Country].ISO_Code AS Country, ");
        sql.AppendLine("   [Product].Abbr AS ProductAbbr, ");
        sql.AppendLine("   [Operation].Abbr AS Operation, ");
        sql.AppendLine("   [Bookings].Amount, ");
        sql.AppendLine("   [Bookings].Value, ");
        sql.AppendLine("   [Bookings].Percentage, ");
        sql.AppendLine("   [Bookings].Comments ");
        sql.AppendLine(" FROM ");
        sql.AppendLine("   [Bookings] ");
        sql.AppendLine("   INNER JOIN [SubRegion] ON [Bookings].CountryID=[SubRegion].ID ");
        //sql.AppendLine("   INNER JOIN [CustomerName] ON [CustomerName].ID=[Bookings].CustomerID ");
        sql.AppendLine("   INNER JOIN [Customer] ON [Customer].ID=[Bookings].CustomerID ");
        sql.AppendLine("   INNER JOIN [CustomerName] ON [Customer].NameID=[CustomerName].ID ");
        sql.AppendLine("   INNER JOIN [Operation] ON [Bookings].OperationID=[Operation].ID ");
        sql.AppendLine("   INNER JOIN [Product] ON [Bookings].ProductID=[Product].ID ");
        sql.AppendLine(
            "   LEFT JOIN [Country_SubRegion] ON [Bookings].CountryID=[Country_SubRegion].SubRegionID AND [Country_SubRegion].Deleted=0 ");
        sql.AppendLine("   LEFT JOIN [Country] ON [Country].ID=[Country_SubRegion].CountryID ");
        sql.AppendLine("   LEFT JOIN [Project] ON [Project].ID=[Bookings].ProjectID and [Project].Deleted=0  ");
        sql.AppendLine("   LEFT JOIN [SalesChannel] ON [SalesChannel].ID=[Bookings].SalesChannelID ");
        sql.AppendLine(" WHERE ");
        sql.AppendLine("   RSMID=" + RSMID);
        sql.AppendLine("   AND [Bookings].SalesOrgID=" + salesOrgID);
        sql.AppendLine("   AND [Bookings].SegmentID=" + segmentID);
        if (!string.IsNullOrEmpty(subregionID) && !string.Equals(subregionID, "-1"))
        {
            sql.AppendLine("   AND [Bookings].CountryID=" + subregionID);
        }
        else
        {
            sql.AppendLine("   AND [Bookings].CountryID IN (" + strSQL + ")");
        }
        sql.AppendLine("   AND YEAR([Bookings].TimeFlag)=" + year);
        sql.AppendLine("   AND MONTH([Bookings].TimeFlag)=" + month);
        sql.AppendLine(" ORDER BY ");
        sql.AppendLine("    [SubRegion].Name, ");
        sql.AppendLine("    [CustomerName].Name, ");
        sql.AppendLine("    [Project].Name, ");
        sql.AppendLine("    [SalesChannel].Name, ");
        sql.AppendLine("    [Bookings].bookingY, ");
        sql.AppendLine("    [Bookings].DeliverY, ");
        sql.AppendLine("    [Product].Abbr ");
        return sql.ToString();
    }

    /// <summary>
    /// Get booking sales data by product
    /// </summary>
    /// <param name="RSMID">RSM ID</param>
    /// <param name="salesOrgID">Sales organization ID</param>
    /// <param name="segmentID">Segment ID</param>
    /// <param name="subregionID">Subregion ID</param>
    /// <param name="productID">Product ID</param>
    /// <param name="year">Year</param>
    /// <param name="month">Month</param>
    /// <param name="preyear">Peryear</param>
    /// <param name="convertFlag">true for converting to Euro, false for not converting</param>
    /// <param name="strSQL">Country SQL</param>
    /// <returns>SQL</returns>
    public string getBookingSalesDataByProduct(string RSMID, string salesOrgID, string segmentID, string subRegionID,
                                               string productID, string year, string month, string preyear,
                                               bool convertFlag, string countrySQL)
    {
        string currency = sql.getSalesOrgCurrency(salesOrgID);
        //double db_rate1 = sql.getRate(currency, true, year, month);
        //double db_rate2 = sql.getRate(currency, false, year, month);
        string db_rate1 = sql.getRate(currency, true, year, month).ToString(CultureInfo.InvariantCulture);
        string db_rate2 = sql.getRate(currency, false, year, month).ToString(CultureInfo.InvariantCulture);
        var strSQL = new StringBuilder();
        strSQL.AppendLine(" SELECT ");
        strSQL.AppendLine("   A.SubRegionID, ");
        strSQL.AppendLine("   A.CustomerID, ");
        strSQL.AppendLine("   A.ProjectID, ");
        strSQL.AppendLine("   A.SalesChannelID, ");
        strSQL.AppendLine("   A.SubRegion, ");
        strSQL.AppendLine("   A.Customer, ");
        strSQL.AppendLine("   A.Project, ");
        strSQL.AppendLine("   A.SalesChannel, ");
        strSQL.AppendLine("   ROUND(ISNULL(A.Amount,0),0) AS '" + year.Substring(2, 2) + "', ");
        strSQL.AppendLine("   ROUND(ISNULL(B.Amount,0),0) AS BUD ");
        strSQL.AppendLine(" FROM ");
        strSQL.AppendLine("   (SELECT ");
        strSQL.AppendLine("     [Bookings].CountryID AS SubRegionID, ");
        strSQL.AppendLine("     [Bookings].CustomerID, ");
        strSQL.AppendLine("     [Bookings].ProjectID, ");
        strSQL.AppendLine("     [Bookings].SalesChannelID, ");
        strSQL.AppendLine("     [SubRegion].Name AS SubRegion, ");
        strSQL.AppendLine("     [CustomerName].Name AS Customer, ");
        strSQL.AppendLine("     [Project].Name AS Project, ");
        strSQL.AppendLine("     [SalesChannel].Name AS SalesChannel, ");
        if (convertFlag)
        {
            strSQL.AppendLine("     SUM(CASE WHEN [Bookings].DeliverY='YTD' OR [Bookings].DeliverY='" +
                              year.Substring(2) + "' ");
            strSQL.AppendLine("              THEN Amount*" + db_rate1);
            strSQL.AppendLine("              ELSE Amount*" + db_rate2);
            strSQL.AppendLine("     END) AS Amount ");
        }
        else
        {
            strSQL.AppendLine("     SUM(Amount) AS Amount ");
        }
        strSQL.AppendLine("   FROM ");
        strSQL.AppendLine("     [Bookings] ");
        strSQL.AppendLine("     INNER JOIN [SubRegion] ON [Bookings].CountryID=[SubRegion].ID ");
        //strSQL.AppendLine("     INNER JOIN [CustomerName] ON [CustomerName].ID=[Bookings].CustomerID ");
        strSQL.AppendLine(" inner join Customer on Bookings.CustomerID=Customer.ID ");
        strSQL.AppendLine(" inner join CustomerName on Customer.NameID=CustomerName.ID ");
        strSQL.AppendLine("     INNER JOIN [Operation] ON [Bookings].OperationID=[Operation].ID ");
        strSQL.AppendLine("     LEFT JOIN [Project] ON [Project].ID=[Bookings].ProjectID ");
        strSQL.AppendLine("     LEFT JOIN [SalesChannel] ON [SalesChannel].ID=[Bookings].SalesChannelID ");
        strSQL.AppendLine("   WHERE ");
        strSQL.AppendLine("     RSMID=" + RSMID);
        strSQL.AppendLine("     AND SalesOrgID=" + salesOrgID);
        strSQL.AppendLine("     AND SegmentID=" + segmentID);
        if (string.IsNullOrEmpty(subRegionID) || string.Equals(subRegionID, "-1"))
        {
            strSQL.AppendLine("     AND [Bookings].CountryID IN (" + countrySQL + ")");
        }
        else
        {
            strSQL.AppendLine("     AND [Bookings].CountryID=" + subRegionID);
        }
        strSQL.AppendLine("     AND [Bookings].ProductID=" + productID);
        strSQL.AppendLine("     AND YEAR([Bookings].TimeFlag)=" + year);
        strSQL.AppendLine("     AND MONTH([Bookings].TimeFlag)=" + month);
        strSQL.AppendLine("     AND [Bookings].BookingY=" + year.Substring(2));
        strSQL.AppendLine("   GROUP BY ");
        strSQL.AppendLine("     [Bookings].CountryID, ");
        strSQL.AppendLine("     [Bookings].CustomerID, ");
        strSQL.AppendLine("     [Bookings].ProjectID, ");
        strSQL.AppendLine("     [Bookings].SalesChannelID, ");
        strSQL.AppendLine("     [SubRegion].Name, ");
        strSQL.AppendLine("     [CustomerName].Name, ");
        strSQL.AppendLine("     [Project].Name, ");
        strSQL.AppendLine("     [SalesChannel].Name) AS A ");
        strSQL.AppendLine("   LEFT JOIN ");
        strSQL.AppendLine("   (SELECT ");
        strSQL.AppendLine("     [Bookings].CountryID AS SubRegionID, ");
        strSQL.AppendLine("     [Bookings].CustomerID, ");
        strSQL.AppendLine("     [Bookings].ProjectID, ");
        strSQL.AppendLine("     [Bookings].SalesChannelID, ");
        strSQL.AppendLine("     [SubRegion].Name AS SubRegion, ");
        strSQL.AppendLine("     [CustomerName].Name AS Customer, ");
        strSQL.AppendLine("     [Project].Name AS Project, ");
        strSQL.AppendLine("     [SalesChannel].Name AS SalesChannel, ");
        if (convertFlag)
        {
            strSQL.AppendLine("     SUM(CASE WHEN [Bookings].DeliverY='YTD' OR [Bookings].DeliverY='" +
                              year.Substring(2) + "' ");
            strSQL.AppendLine("              THEN Amount*" + db_rate1);
            strSQL.AppendLine("              ELSE Amount*" + db_rate2);
            strSQL.AppendLine("     END) AS Amount ");
        }
        else
        {
            strSQL.AppendLine("     SUM(Amount) AS Amount ");
        }
        strSQL.AppendLine("   FROM ");
        strSQL.AppendLine("     [Bookings] ");
        strSQL.AppendLine("     INNER JOIN [SubRegion] ON [Bookings].CountryID=[SubRegion].ID ");
        //strSQL.AppendLine("     INNER JOIN [CustomerName] ON [CustomerName].ID=[Bookings].CustomerID ");
        strSQL.AppendLine(" inner join Customer on Bookings.CustomerID=Customer.ID ");
        strSQL.AppendLine(" inner join CustomerName on Customer.NameID=CustomerName.ID ");
        strSQL.AppendLine("     INNER JOIN [Operation] ON [Bookings].OperationID=[Operation].ID ");
        strSQL.AppendLine("     LEFT JOIN [Project] ON [Project].ID=[Bookings].ProjectID ");
        strSQL.AppendLine("     LEFT JOIN [SalesChannel] ON [SalesChannel].ID=[Bookings].SalesChannelID ");
        strSQL.AppendLine("   WHERE ");
        strSQL.AppendLine("     RSMID=" + RSMID);
        strSQL.AppendLine("     AND SalesOrgID=" + salesOrgID);
        strSQL.AppendLine("     AND SegmentID=" + segmentID);
        if (string.IsNullOrEmpty(subRegionID) || string.Equals(subRegionID, "-1"))
        {
            strSQL.AppendLine("     AND [Bookings].CountryID IN (" + countrySQL + ")");
        }
        else
        {
            strSQL.AppendLine("     AND [Bookings].CountryID=" + subRegionID);
        }
        strSQL.AppendLine("     AND [Bookings].ProductID=" + productID);
        strSQL.AppendLine("     AND YEAR([Bookings].TimeFlag)=" + preyear);
        strSQL.AppendLine("     AND MONTH([Bookings].TimeFlag)=03 ");
        strSQL.AppendLine("     AND [Bookings].BookingY=" + year.Substring(2));
        strSQL.AppendLine("   GROUP BY ");
        strSQL.AppendLine("     [Bookings].CountryID, ");
        strSQL.AppendLine("     [Bookings].CustomerID, ");
        strSQL.AppendLine("     [Bookings].ProjectID, ");
        strSQL.AppendLine("     [Bookings].SalesChannelID, ");
        strSQL.AppendLine("     [SubRegion].Name, ");
        strSQL.AppendLine("     [CustomerName].Name, ");
        strSQL.AppendLine("     [Project].Name, ");
        strSQL.AppendLine("     [SalesChannel].Name) AS B ON A.SubRegionID=B.SubRegionID ");
        strSQL.AppendLine("                                  AND A.CustomerID=B.CustomerID ");
        strSQL.AppendLine("                                  AND A.ProjectID=B.ProjectID ");
        strSQL.AppendLine("                                  AND A.SalesChannelID=B.SalesChannelID ");
        strSQL.AppendLine(" ORDER BY ");
        strSQL.AppendLine("   A.SubRegion, ");
        strSQL.AppendLine("   A.Customer, ");
        strSQL.AppendLine("   A.Project, ");
        strSQL.AppendLine("   A.SalesChannel ");
        return strSQL.ToString();
    }

    /// <summary>
    /// Get booking sales data totle by product
    /// </summary>
    /// <param name="RSMID">RSM ID</param>
    /// <param name="RSMAbbr">RSM Abbr</param>
    /// <param name="salesOrgID">Sales organization ID</param>
    /// <param name="segmentID">Segment ID</param>
    /// <param name="subregionID">Subregion ID</param>
    /// <param name="productID">Product ID</param>
    /// <param name="year">Year</param>
    /// <param name="month">Month</param>
    /// <param name="preyear">Peryear</param>
    /// <param name="convertFlag">true for converting to Euro, false for not converting</param>
    /// <param name="strSQL">Country SQL</param>
    /// <returns>SQL</returns>
    public string getBookingSalesDataTotleByProduct(string RSMID, string RSMAbbr, string salesOrgID, string segmentID,
                                                    string subRegionID, string productID, string year, string month,
                                                    string preyear, bool convertFlag, string countrySQL)
    {
        string currency = sql.getSalesOrgCurrency(salesOrgID);
        //double db_rate1 = sql.getRate(currency, true, year, month);
        //double db_rate2 = sql.getRate(currency, false, year, month);
        string db_rate1 = sql.getRate(currency, true, year, month).ToString(CultureInfo.InvariantCulture);
        string db_rate2 = sql.getRate(currency, false, year, month).ToString(CultureInfo.InvariantCulture);
        var strSQL = new StringBuilder();
        strSQL.AppendLine(" SELECT ");
        strSQL.AppendLine("   A.Operation, ");
        strSQL.AppendLine("   ROUND(ISNULL(B.Amount,0),0) AS '" + year.Substring(2) + "', ");
        strSQL.AppendLine("   ROUND(ISNULL(C.Amount,0),0) AS BUD ");
        strSQL.AppendLine(" FROM ");
        strSQL.AppendLine("   (SELECT DISTINCT");
        strSQL.AppendLine("     [Bookings].OperationID, ");
        strSQL.AppendLine("     ('" + RSMAbbr + "/'+[Operation].AbbrL) AS Operation ");
        strSQL.AppendLine("   FROM ");
        strSQL.AppendLine("     [Bookings] ");
        strSQL.AppendLine("     INNER JOIN [Operation] ON [Bookings].OperationID=[Operation].ID ");
        strSQL.AppendLine("   WHERE ");
        strSQL.AppendLine("     RSMID=" + RSMID);
        strSQL.AppendLine("     AND SalesOrgID=" + salesOrgID);
        strSQL.AppendLine("     AND SegmentID=" + segmentID);
        if (string.IsNullOrEmpty(subRegionID) || string.Equals(subRegionID, "-1"))
        {
            strSQL.AppendLine("     AND [Bookings].CountryID IN (" + countrySQL + ")");
        }
        else
        {
            strSQL.AppendLine("     AND [Bookings].CountryID=" + subRegionID);
        }
        strSQL.AppendLine("     AND YEAR([Bookings].TimeFlag)=" + year);
        strSQL.AppendLine("     AND MONTH([Bookings].TimeFlag)=" + month);
        strSQL.AppendLine("     AND [Bookings].BookingY=" + year.Substring(2) + ") AS A ");
        strSQL.AppendLine("   LEFT JOIN ");
        strSQL.AppendLine("   (SELECT ");
        strSQL.AppendLine("     [Bookings].OperationID, ");
        strSQL.AppendLine("     ('" + RSMAbbr + "/'+[Operation].AbbrL) AS Operation, ");
        if (convertFlag)
        {
            strSQL.AppendLine("     SUM(CASE WHEN [Bookings].DeliverY='YTD' OR [Bookings].DeliverY='" +
                              year.Substring(2) + "' ");
            strSQL.AppendLine("              THEN Amount*" + db_rate1);
            strSQL.AppendLine("              ELSE Amount*" + db_rate2);
            strSQL.AppendLine("     END) AS Amount ");
        }
        else
        {
            strSQL.AppendLine("     SUM(Amount) AS Amount ");
        }
        strSQL.AppendLine("   FROM ");
        strSQL.AppendLine("     [Bookings] ");
        strSQL.AppendLine("     INNER JOIN [Operation] ON [Bookings].OperationID=[Operation].ID ");
        strSQL.AppendLine("   WHERE ");
        strSQL.AppendLine("     RSMID=" + RSMID);
        strSQL.AppendLine("     AND SalesOrgID=" + salesOrgID);
        strSQL.AppendLine("     AND SegmentID=" + segmentID);
        if (string.IsNullOrEmpty(subRegionID) || string.Equals(subRegionID, "-1"))
        {
            strSQL.AppendLine("     AND [Bookings].CountryID IN (" + countrySQL + ")");
        }
        else
        {
            strSQL.AppendLine("     AND [Bookings].CountryID=" + subRegionID);
        }
        strSQL.AppendLine("     AND [Bookings].ProductID=" + productID);
        strSQL.AppendLine("     AND YEAR([Bookings].TimeFlag)=" + year);
        strSQL.AppendLine("     AND MONTH([Bookings].TimeFlag)=" + month);
        strSQL.AppendLine("     AND [Bookings].BookingY=" + year.Substring(2));
        strSQL.AppendLine("   GROUP BY ");
        strSQL.AppendLine("     [Bookings].OperationID, ");
        strSQL.AppendLine("     Operation.AbbrL) AS B ON A.OperationID=B.OperationID ");
        strSQL.AppendLine("   LEFT JOIN ");
        strSQL.AppendLine("   (SELECT ");
        strSQL.AppendLine("     [Bookings].OperationID, ");
        strSQL.AppendLine("     ('" + RSMAbbr + "/'+[Operation].AbbrL) AS Operation, ");
        if (convertFlag)
        {
            strSQL.AppendLine("     SUM(CASE WHEN [Bookings].DeliverY='YTD' OR [Bookings].DeliverY='" +
                              year.Substring(2) + "' ");
            strSQL.AppendLine("              THEN Amount*" + db_rate1);
            strSQL.AppendLine("              ELSE Amount*" + db_rate2);
            strSQL.AppendLine("     END) AS Amount ");
        }
        else
        {
            strSQL.AppendLine("     SUM(Amount) AS Amount ");
        }
        strSQL.AppendLine("   FROM ");
        strSQL.AppendLine("     [Bookings] ");
        strSQL.AppendLine("     INNER JOIN [Operation] ON [Bookings].OperationID=[Operation].ID ");
        strSQL.AppendLine("   WHERE ");
        strSQL.AppendLine("     RSMID=" + RSMID);
        strSQL.AppendLine("     AND SalesOrgID=" + salesOrgID);
        strSQL.AppendLine("     AND SegmentID=" + segmentID);
        if (string.IsNullOrEmpty(subRegionID) || string.Equals(subRegionID, "-1"))
        {
            strSQL.AppendLine("     AND [Bookings].CountryID IN (" + countrySQL + ")");
        }
        else
        {
            strSQL.AppendLine("     AND [Bookings].CountryID=" + subRegionID);
        }
        strSQL.AppendLine("     AND [Bookings].ProductID=" + productID);
        strSQL.AppendLine("     AND YEAR([Bookings].TimeFlag)=" + preyear);
        strSQL.AppendLine("     AND MONTH([Bookings].TimeFlag)=03");
        strSQL.AppendLine("     AND [Bookings].BookingY=" + year.Substring(2));
        strSQL.AppendLine("   GROUP BY ");
        strSQL.AppendLine("     [Bookings].OperationID, ");
        strSQL.AppendLine("     Operation.AbbrL) AS C ON B.OperationID=C.OperationID ");
        strSQL.AppendLine(" ORDER BY ");
        strSQL.AppendLine("   A.Operation ");
        return strSQL.ToString();
    }
}