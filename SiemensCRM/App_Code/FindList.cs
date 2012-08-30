using System.Data;
using System.Web.UI.WebControls;

/// <summary>
/// the helper for query information from database
/// </summary>
public class FindList
{
    private readonly SQLHelper helper = new SQLHelper();

    /// <summary>
    /// bind DropDownList control
    /// </summary>
    /// <param name="ds">data source</param>
    /// <param name="ddlist">DropDownList control to be bound.</param>
    public void bindFind(DataSet ds, DropDownList ddlist)
    {
        ddlist.Items.Add("");
        if (ds.Tables[0].Rows.Count > 0)
        {
            DataTable dt = ds.Tables[0];
            int count = dt.Rows.Count;
            int index = 0;
            while (index < count)
            {
                ddlist.Items.Add(dt.Rows[index][0].ToString().Trim());
                index++;
            }
            ddlist.Enabled = true;
        }
        else
        {
            ddlist.Enabled = false;
        }
    }

    
    /// <summary>
    /// get user role info
    /// </summary>
    /// <returns>user role info</returns>
    public DataSet getRole()
    {
        string sql_role = "SELECT Name FROM [Role] ORDER BY Name ASC";
        DataSet ds_role = helper.GetDataSet(sql_role);
        return ds_role;
    }

    /// <summary>
    /// get user first names
    /// </summary>
    /// <returns>user first names</returns>
    public DataSet getUserFirstName()
    {
        string sql_searchUser;
        sql_searchUser = "SELECT FirstName FROM [User] "
                         + " WHERE Deleted = 0 GROUP BY FirstName ORDER BY FirstName ASC";
        DataSet ds_query_Admin = helper.GetDataSet(sql_searchUser);
        return ds_query_Admin;
    }

    /// <summary>
    /// get user last names
    /// </summary>
    /// <returns>user last names</returns>
    public DataSet getUserLastName()
    {
        string sql_searchUser;
        sql_searchUser = "SELECT LastName FROM [User] "
                         + " WHERE Deleted = 0 GROUP BY LastName ORDER BY LastName ASC";
        DataSet ds_query_Admin = helper.GetDataSet(sql_searchUser);
        return ds_query_Admin;
    }

    /// <summary>
    /// get user Alias
    /// </summary>
    /// <returns>user Alias info</returns>
    public DataSet getUserAlias()
    {
        string sql_searchUser;
        sql_searchUser = "SELECT Alias FROM [User] "
                         + " WHERE Deleted = 0 GROUP BY Alias ORDER BY Alias ASC";
        DataSet ds_query_Admin = helper.GetDataSet(sql_searchUser);
        return ds_query_Admin;
    }

    /// <summary>
    /// get user Abbr info
    /// </summary>
    /// <returns>user Abbr info</returns>
    public DataSet getUserAbbr()
    {
        string sql_searchUser;
        sql_searchUser = "SELECT Abbr FROM [User] "
                         + " WHERE Deleted = 0 GROUP BY Abbr ORDER BY Abbr ASC";
        DataSet ds_query_Admin = helper.GetDataSet(sql_searchUser);
        return ds_query_Admin;
    }

    //Operation
    /// <summary>
    /// get operation names
    /// </summary>
    /// <returns>operation names</returns>
    public DataSet getOperationName()
    {
        string sql;
        sql = "SELECT Name FROM [Operation] "
              + " WHERE Deleted = 0 GROUP BY Name ORDER BY Name ASC";
        DataSet ds = helper.GetDataSet(sql);
        return ds;
    }

    /// <summary>
    /// get operation AbbrL info
    /// </summary>
    /// <returns>operation AbbrL</returns>
    public DataSet getOperationAbbrL()
    {
        string sql;
        sql = "SELECT AbbrL FROM [Operation] "
              + " WHERE Deleted = 0 GROUP BY AbbrL ORDER BY AbbrL ASC";
        DataSet ds = helper.GetDataSet(sql);
        return ds;
    }

    /// <summary>
    /// get operation Abbr info
    /// </summary>
    /// <returns>operation Abbr info</returns>
    public DataSet getOperationAbbr()
    {
        string sql;
        sql = "SELECT Abbr FROM [Operation] "
              + " WHERE Deleted = 0 GROUP BY Abbr ORDER BY Abbr ASC";
        DataSet ds = helper.GetDataSet(sql);
        return ds;
    }

    //Segment
    /// <summary>
    /// get segment Abbr info
    /// </summary>
    /// <returns>segment Abbr info</returns>
    public DataSet getSegmentAbbr()
    {
        string sql;
        sql = "SELECT Abbr FROM [Segment] "
              + " WHERE Deleted = 0 GROUP BY Abbr ORDER BY Abbr ASC";
        DataSet ds = helper.GetDataSet(sql);
        return ds;
    }

    //Currency
    /// <summary>
    /// get currency names
    /// </summary>
    /// <returns>currency names</returns>
    public DataSet getCurrencyName()
    {
        string sql;
        sql = "SELECT Name FROM [Currency] "
              + " WHERE Deleted = 0 GROUP BY Name ORDER BY Name ASC";
        DataSet ds = helper.GetDataSet(sql);
        return ds;
    }

    //Country
    /// <summary>
    /// get country names
    /// </summary>
    /// <returns>country names</returns>
    public DataSet getCountryName()
    {
        string sql;
        sql = "SELECT Name FROM [Country] "
              + " WHERE Deleted = 0 GROUP BY Name ORDER BY Name ASC";
        DataSet ds = helper.GetDataSet(sql);
        return ds;
    }

    /// <summary>
    /// get country ISO code
    /// </summary>
    /// <returns>country ISO code</returns>
    public DataSet getCountryISO_Code()
    {
        string sql;
        sql = "SELECT ISO_Code FROM [Country] "
              + " WHERE Deleted = 0 GROUP BY ISO_Code ORDER BY ISO_Code ASC";
        DataSet ds = helper.GetDataSet(sql);
        return ds;
    }

    /// <summary>
    /// get region names
    /// </summary>
    /// <returns>region names</returns>
    public DataSet getRegion()
    {
        string sql;
        sql = "SELECT Name FROM [Region] "
              + " WHERE Deleted = 0 GROUP BY Name ORDER BY Name ASC";
        DataSet ds = helper.GetDataSet(sql);
        return ds;
    }

    /// <summary>
    /// get cluster names
    /// </summary>
    /// <returns>cluster names</returns>
    public DataSet getCluster()
    {
        string sql;
        sql = "SELECT Name FROM [Cluster] "
              + " WHERE Deleted = 0 GROUP BY Name ORDER BY Name ASC";
        DataSet ds = helper.GetDataSet(sql);
        return ds;
    }

    /// <summary>
    /// get subregion names
    /// </summary>
    /// <returns>subregion names</returns>
    public DataSet getSubRegion()
    {
        string sql;
        sql = "SELECT Name FROM [SubRegion] "
              + " WHERE Deleted = 0 GROUP BY Name ORDER BY Name ASC";
        DataSet ds = helper.GetDataSet(sql);
        return ds;
    }

    //Customer
    /// <summary>
    /// get customer names
    /// </summary>
    /// <returns>customer names</returns>
    public DataSet getCustomerName()
    {
        string sql;
        sql = "SELECT Name FROM [CustomerName] "
              + " WHERE Deleted = 0 GROUP BY Name ORDER BY Name ASC";
        DataSet ds = helper.GetDataSet(sql);
        return ds;
    }

    /// <summary>
    /// get customer types
    /// </summary>
    /// <returns>customer types</returns>
    public DataSet getCustomerType()
    {
        string sql;
        sql = "SELECT Name FROM [CustomerType] "
              + " WHERE Deleted = 0 GROUP BY Name ORDER BY Name ASC";
        DataSet ds = helper.GetDataSet(sql);
        return ds;
    }

    //Project
    /// <summary>
    /// get project names
    /// </summary>
    /// <returns>project names</returns>
    public DataSet getProject()
    {
        string sql;
        sql = "SELECT Name FROM [Project] "
              + " WHERE Deleted = 0 GROUP BY Name ORDER BY Name ASC";
        DataSet ds = helper.GetDataSet(sql);
        return ds;
    }

    /// <summary>
    /// get project values
    /// </summary>
    /// <returns>project values</returns>
    public DataSet getProjectValue()
    {
        string sql;
        sql = "SELECT Value FROM [Project] "
              + " WHERE Deleted = 0 GROUP BY Value ORDER BY Value ASC";
        DataSet ds = helper.GetDataSet(sql);
        return ds;
    }

    //Sales Channel
    /// <summary>
    /// get salesChannel names
    /// </summary>
    /// <returns>saleschannel names</returns>
    public DataSet getSalesChannel()
    {
        string sql;
        sql = "SELECT Name FROM [SalesChannel] "
              + " WHERE Deleted = 0 GROUP BY Name ORDER BY Name ASC";
        DataSet ds = helper.GetDataSet(sql);
        return ds;
    }

    //Sales Org
    /// <summary>
    /// get salesorg names
    /// </summary>
    /// <returns>salesorg names</returns>
    public DataSet getSalesOrgName()
    {
        string sql;
        sql = "SELECT Name FROM [SalesOrg] "
              + " WHERE Deleted = 0 GROUP BY Name ORDER BY Name ASC";
        DataSet ds = helper.GetDataSet(sql);
        return ds;
    }

    /// <summary>
    /// get salesorg Abbrs
    /// </summary>
    /// <returns>salesorg Abbrs</returns>
    public DataSet getSalesOrgAbbr()
    {
        string sql;
        sql = "SELECT Abbr FROM [SalesOrg] "
              + " WHERE Deleted = 0 GROUP BY Abbr ORDER BY Abbr ASC";
        DataSet ds = helper.GetDataSet(sql);
        return ds;
    }
}