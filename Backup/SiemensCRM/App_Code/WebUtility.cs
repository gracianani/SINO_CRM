/*++

Module Name:

    WebUtility.cs

Abstract:
    
    Collection of utility methods for web tier.

Author:

    Longran Wei 07-January-2010


Revision History:

--*/

using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;

/// <summary>
/// basic functions of data type checking and control style setting.
/// </summary>
public class WebUtility : Page
{
    private readonly LogUtility log = new LogUtility();

    /// <summary>
    /// set surface style for GridView control.
    /// </summary>
    /// <param name="gridview">GridView control need to be set style.</param>
    public void setProperties(GridView gridview)
    {
        gridview.GridLines = GridLines.None;
        gridview.CellPadding = 4;
        gridview.ForeColor = Color.FromName("#333333");
        gridview.RowStyle.BackColor = Color.FromName("#EFF3FB");
        gridview.FooterStyle.BackColor = Color.FromName("#507CD1");
        gridview.FooterStyle.Font.Bold = true;
        gridview.FooterStyle.ForeColor = Color.White;
        gridview.PagerStyle.BackColor = Color.FromName("#2461BF");
        gridview.PagerStyle.ForeColor = Color.White;
        gridview.PagerStyle.HorizontalAlign = HorizontalAlign.Center;
        gridview.SelectedRowStyle.BackColor = Color.FromName("#D1DDF1");
        gridview.SelectedRowStyle.Font.Bold = true;
        gridview.SelectedRowStyle.ForeColor = Color.FromName("#333333");
        gridview.HeaderStyle.BackColor = Color.FromName("#507CD1");
        gridview.HeaderStyle.Font.Bold = true;
        gridview.HeaderStyle.ForeColor = Color.White;
        gridview.EditRowStyle.BackColor = Color.FromName("#2461BF");
        gridview.AlternatingRowStyle.BackColor = Color.White;

        gridview.PageSize = 10;
    }

    /// <summary>
    /// Check if is the DataTime type.
    /// </summary>
    /// <param name="strDate">input string.</param>
    /// <returns>check result</returns>
    public bool CheckDate(string strDate)
    {
        DateTime dtDate;
        bool bValid = true;

        if (strDate == "")
            return bValid;
        try
        {
            dtDate = DateTime.Parse(strDate);
        }
        catch (FormatException)
        {
            bValid = false;
            log.WriteLog(LogUtility.LogErrorLevel.LOG_ERROR, "Date format is illegal and date' value is " + strDate);
        }
        return bValid;
    }

    /// <summary>
    /// check if the email format.
    /// </summary>
    /// <param name="email">input string</param>
    /// <returns>check result</returns>
    public bool checkEmail(string email)
    {
        var rx = new Regex(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");
        if (rx.IsMatch(email) || email == "")
        {
            return true;
        }
        else
        {
            log.WriteLog(LogUtility.LogErrorLevel.LOG_ERROR, "Email format is illegal and date' value is " + email);
            return false;
        }
    }


    /// <summary>
    /// check if the float format.
    /// </summary>
    /// <param name="str">input string</param>
    /// <returns>check result</returns>
    public bool checkFloat(string str)
    {
        var rx = new Regex(@"^[0-9]+(.[0-9]{1,10})?$");
        if (rx.IsMatch(str))
        {
            return true;
        }
        else
        {
            log.WriteLog(LogUtility.LogErrorLevel.LOG_ERROR, "Float format is illegal and float' value is " + str);
            return false;
        }
    }


    /// <summary>
    /// check if the IP format.
    /// </summary>
    /// <param name="str">input string</param>
    /// <returns>check result</returns>
    public bool checkIP(string str)
    {
        var rx = new Regex(@"^([1-9]|[1-9]\d|1\d{2}|2[0-1]\d|22[0-3])(\.(\d|[1-9]\d|1\d{2}|2[0-4]\d|25[0-5])){3}$");
        if (rx.IsMatch(str))
        {
            return true;
        }
        else
        {
            log.WriteLog(LogUtility.LogErrorLevel.LOG_ERROR, "IP format is illegal and IP' value is " + str);
            return false;
        }
    }

    /// <summary>
    /// check if is integer type.
    /// </summary>
    /// <param name="str">input string</param>
    /// <returns>check result</returns>
    public bool IsInteger(string str)
    {
        var reg1
            = new Regex(@"^-?\d+$");
        return reg1.IsMatch(str);
    }

    /// <summary>
    /// check if is float type.
    /// </summary>
    /// <param name="str">input string</param>
    /// <returns>check result</returns>
    public bool IsFloat(string str)
    {
        var reg1
            = new Regex(@"^(-?\d+)(\.\d+)?$");
        return reg1.IsMatch(str);
    }


    /// <summary>
    /// check is Date type. 
    /// </summary>
    /// <param name="StrSource">input string.</param>
    /// <returns>check result</returns>          
    public bool IsDate(string StrSource)
    {
        return Regex.IsMatch(StrSource,
                             @"^((((1[6-9]|[2-9]\d)\d{2})-(0?[13578]|1[02])-(0?[1-9]" +
                             @"|[12]\d|3[01]))|(((1[6-9]|[2-9]\d)\d{2})-(0?[13456789]|"
                             + @"1[012])-(0?[1-9]|[12]\d|30))|(((1[6-9]|[2-9]\d)\d{2})-0?"
                             + @"2-(0?[1-9]|1\d|2[0-9]))|(((1[6-9]|[2-9]\d)(0[48]|[2468]"
                             + @"[048]|[13579][26])|((16|[2468][048]|[3579][26])00))-0?2-29-))$");
    }

    /// <summary>
    /// transform input string to a safe string for DB.
    /// </summary>
    /// <param name="pStr">input string</param>
    /// <returns>output string after transforming.</returns>
    public static string transParam(string pStr)
    {
        if (pStr == null)
        {
            return null;
        }
        else if (pStr.Trim() == "")
        {
            return null;
        }
        else
        {
            return pStr.Trim().Replace("/", "//").Replace("_", "/_").Replace("%", "/%").Replace("'", "''");
        }
    }


   
    /// <summary>
    /// check if input string has special characters.
    /// </summary>
    /// <param name="str">input string to be checked</param>
    /// <returns>check result</returns>
    public bool checkString(string str)
    {
        var reg1
            = new Regex("[~!@#$%^&*()=+[\\]{}''\";:/?.,><`|！·￥…—（）\\-、；：。，》《]");
        bool a = reg1.IsMatch(str);
        return reg1.IsMatch(str);
    }

    
}