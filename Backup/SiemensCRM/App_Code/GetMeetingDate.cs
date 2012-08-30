/*
 *  File Name     : GetMeetingDate.cs
 * 
 *  Description   : Get the meeting date
 * 
 *  Author        : Wang Jun
 *  
 *  Problem       : 
 * 
 *  Modified Date : 2010-05-18
 */

using System.Data;

/// <summary>
/// get or set MeetingData which is used for other parts except Report.
/// </summary>
public class GetMeetingDate
{
    private const string fiscalStart = "Oct.1"; //start date of fiscal year
    private const string fiscalEnd = "Sept.30"; //end date of fiscal year
    private static string yearBeforePre; //the year before last
    private static string preyear; //last year
    private static string year; //this year
    private static string nextyear; //next year
    private static string yearAfterNext; //the year after next
    private static string month; //meeting month
    private readonly SQLHelper helper = new SQLHelper();
    private LogUtility log = new LogUtility();

    /// <summary>
    /// set meeting data message show in web front page.
    /// </summary>
    /// <returns>meeting data message show in web front page.</returns>
    public string setDisplayDate()
    {
        return "(Meeting Date: " + getMeetingName(int.Parse(getmonth())) + " " + getyear() + ")";
    }

    /// <summary>
    /// get detail meeting data information.
    /// </summary>
    public void setDate()
    {
        year = getMeetingDateYear();
        month = getMeetingDateMonth();

        preyear = (int.Parse(year) - 1).ToString();
        yearBeforePre = (int.Parse(preyear) - 1).ToString();
        nextyear = (int.Parse(year) + 1).ToString();
        yearAfterNext = (int.Parse(nextyear) + 1).ToString();
    }


    /// <summary>
    /// set detail meeting data information by user id
    /// </summary>
    /// <param name="userID">user id</param>
    public void setSelectDate(string userID)
    {
        year = getSelectMeetingDateYear(userID);
        month = getSelectMeetingDateMonth(userID);

        if (year != null)
        {
            preyear = (int.Parse(year) - 1).ToString();
            yearBeforePre = (int.Parse(preyear) - 1).ToString();
            nextyear = (int.Parse(year) + 1).ToString();
            yearAfterNext = (int.Parse(nextyear) + 1).ToString();
        }
    }

    // add by zy 20110120 end
    /// <summary>
    /// get meeting name by month
    /// </summary>
    /// <param name="month">month</param>
    /// <returns>meeting name</returns>
    public string getMeetingName(int month)
    {
        switch (month)
        {
            case 1:
                return "3+9";
            case 3:
                return "5+7";
            case 7:
                return "9+3";
            case 10:
                return "0+12";
            default:
                return "-1";
        }
    }

    /// <summary>
    /// get meeting year by meeting date.
    /// </summary>
    /// <returns>meeting year</returns>
    public string getyear()
    {
        return year;
    }

    /// <summary>
    /// get pre year of meeting date
    /// </summary>
    /// <returns>pre year of meeting date</returns>
    public string getpreyear()
    {
        return preyear;
    }

    /// <summary>
    /// get the year befor pre year.
    /// </summary>
    /// <returns>the year befor pre year.</returns>
    public string getyearBeforePre()
    {
        return yearBeforePre;
    }

    /// <summary>
    /// get next year of meeting date.
    /// </summary>
    /// <returns>next year</returns>
    public string getnextyear()
    {
        return nextyear;
    }

    /// <summary>
    /// get the year after next year.
    /// </summary>
    /// <returns>the year after next year.</returns>
    public string getyearAfterNext()
    {
        return yearAfterNext;
    }

    /// <summary>
    /// get month of meeting date.
    /// </summary>
    /// <returns>month</returns>
    public string getmonth()
    {
        return month;
    }


    /// <summary>
    /// get day of meeting date.
    /// </summary>
    /// <returns>day</returns>
    public string getDay()
    {
        return getMeetingDateDay();
    }

    private string getMeetingDateYear()
    {
        string query_meetingyear = "SELECT YEAR(MeetingDate) FROM [SetMeetingDate]";
        DataSet ds_year = helper.GetDataSet(query_meetingyear);

        if (ds_year.Tables.Count > 0)
        {
            if (ds_year.Tables[0].Rows.Count > 0)
            {
                string str_year = ds_year.Tables[0].Rows[0][0].ToString();
                return str_year;
            }
        }
        return null;
    }

    private string getMeetingDateMonth()
    {
        string query_meetingmonth = "SELECT MONTH(MeetingDate) FROM [SetMeetingDate]";
        DataSet ds_month = helper.GetDataSet(query_meetingmonth);

        if (ds_month.Tables.Count > 0)
        {
            if (ds_month.Tables[0].Rows.Count > 0)
            {
                string str_month = ds_month.Tables[0].Rows[0][0].ToString();
                return str_month;
            }
        }
        return null;
    }

    private string getMeetingDateDay()
    {
        string query_meetingday = "SELECT Day(MeetingDate) FROM [SetMeetingDate]";
        DataSet ds_day = helper.GetDataSet(query_meetingday);

        if (ds_day.Tables.Count > 0)
        {
            if (ds_day.Tables[0].Rows.Count > 0)
            {
                string str_day = ds_day.Tables[0].Rows[0][0].ToString();
                return str_day;
            }
        }
        return null;
    }

    // add by zy 20110120 start
    private string getSelectMeetingDateYear(string userID)
    {
        string query_meetingyear = "SELECT YEAR(SelectMeetingDate) FROM [SetSelectMeetingDate] where userid=" + userID;
        DataSet ds_year = helper.GetDataSet(query_meetingyear);

        if (ds_year.Tables.Count > 0)
        {
            if (ds_year.Tables[0].Rows.Count > 0)
            {
                string str_year = ds_year.Tables[0].Rows[0][0].ToString();
                if (str_year != "")
                {
                    return str_year;
                }
            }
        }
        return null;
    }

    private string getSelectMeetingDateMonth(string userID)
    {
        string query_meetingmonth = "SELECT MONTH(SelectMeetingDate) FROM [SetSelectMeetingDate] where userid=" + userID;
        DataSet ds_month = helper.GetDataSet(query_meetingmonth);

        if (ds_month.Tables.Count > 0)
        {
            if (ds_month.Tables[0].Rows.Count > 0)
            {
                string str_month = ds_month.Tables[0].Rows[0][0].ToString();
                if (str_month != "")
                {
                    return str_month;
                }
            }
        }
        return null;
    }

    private string getSelectMeetingDateDay(string userID)
    {
        string query_meetingday = "SELECT Day(SelectMeetingDate) FROM [SetSelectMeetingDate] where userid=" + userID;
        DataSet ds_day = helper.GetDataSet(query_meetingday);

        if (ds_day.Tables.Count > 0)
        {
            if (ds_day.Tables[0].Rows.Count > 0)
            {
                string str_day = ds_day.Tables[0].Rows[0][0].ToString();
                if (str_day != "")
                {
                    return str_day;
                }
            }
        }
        return null;
    }

    // add by zy 20110120 end
    /// <summary>
    /// Get month abbreviation by digtial form
    /// </summary>
    /// <param name="mon">month</param>
    /// <returns>month abbreviation</returns>
    public string getMonth(string mon)
    {
        int imon = int.Parse(mon);
        switch (imon)
        {
            case 1:
                return "Jan.";
            case 2:
                return "Feb.";
            case 3:
                return "Mar.";
            case 4:
                return "Apr.";
            case 5:
                return "May.";
            case 6:
                return "Jun.";
            case 7:
                return "Jul.";
            case 8:
                return "Aug.";
            case 9:
                return "Sept.";
            case 10:
                return "Oct.";
            case 11:
                return "Nov.";
            case 12:
                return "Dec.";
            default:
                return "Error.";
        }
    }


    /// <summary>
    /// get all meeting monthes.
    /// </summary>
    /// <returns>meeting monthes</returns>
    public DataSet getMeetingMonth()
    {
        string query_meetingmonth =
            "SELECT MONTH(MeetingDate) AS 'Month' FROM [MeetingDate] GROUP BY Month(MeetingDate) ORDER BY Month(MeetingDate) ASC";
        DataSet ds_month = helper.GetDataSet(query_meetingmonth);
        return ds_month;
    }

    /// <summary>
    /// get all meeting date
    /// </summary>
    /// <returns>meeting date</returns>
    public DataSet getMeetingDate()
    {
        string query_meetingdate =
            "SELECT CONVERT(varchar(15),MeetingDate,23) AS 'MeetingDate', Month(MeetingDate), Year(MeetingDate) FROM [MeetingDate] GROUP BY Year(MeetingDate), Month(MeetingDate), MeetingDate ORDER BY Year(MeetingDate), Month(MeetingDate) ASC";
        DataSet ds_date = helper.GetDataSet(query_meetingdate);
        return ds_date;
    }

    /// <summary>
    /// check if exist meeting date
    /// </summary>
    /// <param name="str_date">meeting date</param>
    /// <returns>check result</returns>
    public bool existMeetingDate(string str_date)
    {
        DataSet ds = getMeetingDate();
        int index = 0;
        int count = ds.Tables[0].Rows.Count;
        while (index < count)
        {
            if (ds.Tables[0].Rows[index][0].ToString().Trim() == str_date)
                return true;
            index++;
        }
        return false;
    }

    /// <summary>
    /// get all set meeting date
    /// </summary>
    /// <returns>set meeting date</returns>
    public DataSet getSetMeetingDate()
    {
        string query_meetingdate =
            "SELECT CONVERT(varchar(15),MeetingDate,23) AS 'MeetingDate' FROM [SetMeetingDate] GROUP BY Year(MeetingDate), Month(MeetingDate), MeetingDate ORDER BY Year(MeetingDate), Month(MeetingDate) ASC";
        DataSet ds_date = helper.GetDataSet(query_meetingdate);
        return ds_date;
    }

    /// <summary>
    /// check if exist meeting date
    /// </summary>
    /// <param name="str_date">meeting date</param>
    /// <returns>check result</returns>
    public bool existSetMeetingDate(string str_date)
    {
        DataSet ds = getSetMeetingDate();
        if (ds.Tables[0].Rows[0][0].ToString().Trim() == str_date)
            return true;
        return false;
    }

    /// <summary>
    /// check if is the first month
    /// </summary>
    /// <param name="mon">month</param>
    /// <returns>check result</returns>
    public bool JudgeFirstMonth(string mon)
    {
        DataSet ds = getMeetingMonth();
        if (ds.Tables[0].Rows[0][0].ToString().Trim() == mon)
            return true;
        else
            return false;
    }

    /// <summary>
    /// get pre month by input month.
    /// </summary>
    /// <param name="mon">input month</param>
    /// <returns>pre month</returns>
    public string getPreMonth(string mon)
    {
        DataSet ds = getMeetingMonth();
        int index = 0;
        int count = ds.Tables[0].Rows.Count;
        while (index < count)
        {
            if (ds.Tables[0].Rows[index][0].ToString().Trim() == mon)
            {
                break;
            }
            index++;
        }
        if (index == 0)
            return ds.Tables[0].Rows[count - 1][0].ToString().Trim();
        else
            return ds.Tables[0].Rows[index - 1][0].ToString().Trim();
    }
}