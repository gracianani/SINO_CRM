/*
 * File Name    : DisPlayInfo.cs
 * 
 * Description  : Use for displaying some information that point out whether or not you operate successfully
 * 
 * Author       : Wangjun
 * 
 * Modify Date : 2010.1.29
 * 
 * Problem     : none
 * 
 * End Date    :
 */

/// <summary>
/// Use for displaying some information that point out whether or not you operate successfully.
/// </summary>
public class DisplayInfo
{
    /// <summary>
    /// set javascript for reminding user to confirm deletion.
    /// </summary>
    /// <param name="str">object to be deleted.</param>
    /// <returns>output js script</returns>
    public string setRowDataBound(string str)
    {
        return "if( !confirm('Are you sure to delete :\"" + str + "\"?'))return false;";
    }

    /// <summary>
    /// set javascript for reminding user to confirm removing.
    /// </summary>
    /// <param name="strfrom">from object name</param>
    /// <param name="strto">to object name</param>
    /// <returns>remove js script</returns>
    public string setRowDataBound(string strfrom, string strto)
    {
        return "if( !confirm('Are you sure to remove \"" + strto + "\" from \"" + strfrom + "\"?'))return false;";
    }

    /// <summary>
    /// set deletion result message show in the web front page.
    /// </summary>
    /// <param name="str">original information</param>
    /// <param name="flag">operation succed or failed</param>
    /// <returns>deletion result message</returns>
    public string delLabelInfo(string str, bool flag)
    {
        if (flag)
            return "Successfully deleted \"" + str + "\" !";
        else
            return "Failed to delete \"" + str + "\" !";
    }

    /// <summary>
    /// set remove result message show in the web front page.
    /// </summary>
    /// <param name="strfrom">frome object name</param>
    /// <param name="strto">to object name</param>
    /// <param name="flag">operation succed or failed</param>
    /// <returns>remove result message</returns>
    public string delLabelInfo(string strfrom, string strto, bool flag)
    {
        if (flag)
            return "Successfully removed \"" + strto + "\" from \"" + strfrom + "\"!";
        else
            return "Failed to remove \"" + strto + "\" from \"" + strfrom + "\"!";
    }

    /// <summary>
    /// set modification result message show in the web front page.
    /// </summary>
    /// <param name="str">original information</param>
    /// <param name="flag">operation succed or failed</param>
    /// <returns>modification result message</returns>
    public string edtLabelInfo(string str, bool flag)
    {
        if (flag)
            return "Successfully modified \"" + str + "\" !";
        else
            return "Failed to modify \"" + str + "\" !";
    }

    /// <summary>
    /// set add object result message show in the web front page.
    /// </summary>
    /// <param name="str">original information</param>
    /// <param name="flag">operation succed or failed</param>
    /// <returns>result message</returns>
    public string addLabelInfo(string str, bool flag)
    {
        if (flag)
            return "Successfully added \"" + str + "\" !";
        else
            return "Failed to add \"" + str + "\" ! Please input again.";
    }

    /// <summary>
    /// set illegal message show in the web front page 
    /// </summary>
    /// <param name="str">original information</param>
    /// <returns>result message</returns>
    public string addillegal(string str)
    {
        return "Input format is illegal. Input information: (" + str + ").";
    }

    /// <summary>
    /// set illegal message
    /// </summary>
    /// <returns>result message</returns>
    public string addillegal()
    {
        return "Input format is illegal.";
    }

    /// <summary>
    /// set error message for the input string is more than limited length
    /// </summary>
    /// <param name="str">input string</param>
    /// <param name="length">limited length</param>
    /// <returns>error message</returns>
    public string thanlength(string str, string length)
    {
        return "The length of '" + str + "', is not more than " + length + ".";
    }

    /// <summary>
    /// set object existed message
    /// </summary>
    /// <param name="str">original information</param>
    /// <returns>result message</returns>
    public string addExist(string str)
    {
        return "\"" + str + "\"" + " exists in the system !";
    }

    /// <summary>
    /// set inconsistent message
    /// </summary>
    /// <param name="str">original information</param>
    /// <returns>result message</returns>
    public string errorInfo(string str)
    {
        return str + " might be inconsistent.";
    }

    /// <summary>
    /// set error message
    /// </summary>
    /// <param name="str">original information</param>
    /// <returns>result message</returns>
    public string errorSelectInfo(string str)
    {
        return " Please select " + str + " from dropdown list only or input " + str + " only.";
    }

    /// <summary>
    /// set null error message
    /// </summary>
    /// <param name="str">original information</param>
    /// <returns>result message</returns>
    public string errorNullInfo(string str)
    {
        return "Please select or input " + str + "!";
    }

    /// <summary>
    /// return error page
    /// </summary>
    /// <param name="str">error information</param>
    /// <param name="bl">error type</param>
    /// <returns></returns>
    public string errorInfo(string str, bool bl)
    {
        if (bl)
            return "~/Admin/AdminError.aspx?errorInfo=" + str + " might be inconsistent.";
        else
            return "~/Admin/AdminError.aspx?errorInfo=The sql ," + str + ", is error.";
    }

    /// <summary>
    ///  set message currency do not exist.
    /// </summary>
    /// <param name="str">currency info</param>
    /// <returns>result message</returns>
    public string notCurrency(string str)
    {
        return str + "doesn't have currency info. Please enter its currency info.";
    }

    /// <summary>
    /// set deletion message show in the web front page
    /// </summary>
    /// <param name="str_out">first original information</param>
    /// <param name="str_in">second original information</param>
    /// <returns>result message</returns>
    public string delconsistContent(string str_out, string str_in)
    {
        return str_out + " can be not deleted as it contains info related to " + str_in;
    }
}