using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Data.SqlClient;
// By DingJunjie 20110525 ItemW12 Add Start
using System.Text;
// By DingJunjie 20110525 ItemW12 Add End

public partial class CustomerDetail : System.Web.UI.Page
{
    SQLStatement sql = new SQLStatement();
    SQLHelper helper = new SQLHelper();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            bindDataSource();
        }
    }

    protected void gv_Customer_RowDataBound(object sender, GridViewRowEventArgs e)
    {

    }

    protected void bindDataSource()
    {
        String customerID = Request.QueryString["customerID"].ToString().Trim();
        String salesChannelID = Request.QueryString["salesChannelID"].ToString().Trim();

        DataSet ds_customer = getCustomerInfo1(customerID, salesChannelID);

        if (ds_customer.Tables[0].Rows.Count == 0)
        {
            sql.getNullDataSet(ds_customer);
        }

        gv_Customer.Width = Unit.Pixel(800);
        gv_Customer.AutoGenerateColumns = false;
        gv_Customer.AllowPaging = false;
        gv_Customer.Visible = true;

        for (int i = 0; i < ds_customer.Tables[0].Columns.Count; i++)
        {
            BoundField bf = new BoundField();

            bf.DataField = ds_customer.Tables[0].Columns[i].ColumnName.ToString();
            bf.HeaderText = ds_customer.Tables[0].Columns[i].Caption.ToString();
            bf.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
            bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
            if (i <= 3)
            {
                bf.ReadOnly = true;
            }
            bf.ControlStyle.Width = bf.ItemStyle.Width;
            gv_Customer.Columns.Add(bf);
        }

        gv_Customer.DataSource = ds_customer.Tables[0];
        gv_Customer.DataBind();
        gv_Customer.Columns[2].Visible = false;
    }

    public DataSet getCustomerInfo(string customerID, string salesChannelID)
    {
        // By DingJunjie 20110525 ItemW12 Delete Start
        //string sql_customer = "SELECT [CustomerName].Name AS 'Customer Name', [CustomerType].Name AS 'Customer Type', [SalesChannel].Name AS 'Sales Channel',[Country].ISO_Code AS Country, [Customer].City, [Customer].Address, [Customer].Department FROM [Customer] "
        //                           + " LEFT OUTER JOIN [Country] ON [Country].ID = [Customer].CountryID"
        //                           + " INNER JOIN [CustomerType] ON [Customer].TypeID = [CustomerType].ID"
        //                           + " INNER JOIN [CustomerName] ON [Customer].NameID = [CustomerName].ID"
        //                           + " LEFT OUTER JOIN [SalesChannel] ON [Customer].SalesChannelID = [SalesChannel].ID"
        //                           + " WHERE ";//[Customer].Deleted = 0 AND [Country].Deleted = 0 AND [CustomerType].Deleted = 0 AND [CustomerName].Deleted = 0";
        //sql_customer += " [CustomerName].ID = " + customerID;
        //if (!string.IsNullOrEmpty(salesChannelID))
        //{
        //    sql_customer += " AND [SalesChannel].ID = " + salesChannelID;
        //}
        // By DingJunjie 20110525 ItemW12 Delete End
        // By DingJunjie 20110525 ItemW12 Add Start
        StringBuilder sql = new StringBuilder();
        sql.AppendLine(" SELECT ");
        sql.AppendLine("   CustomerName.Name AS 'Customer Name', ");
        sql.AppendLine("   CustomerType.Name AS 'Customer Type', ");
        sql.AppendLine("   SalesChannel.Name AS 'Sales Channel', ");
        sql.AppendLine("   SubRegion.Name AS SubRegion, ");
        sql.AppendLine("   Customer.City, ");
        sql.AppendLine("   Customer.Address, ");
        sql.AppendLine("   Customer.Department ");
        sql.AppendLine(" FROM ");
        sql.AppendLine("   Customer ");
        sql.AppendLine("   INNER JOIN CustomerType ON Customer.TypeID=CustomerType.ID ");
        sql.AppendLine("   INNER JOIN CustomerName ON Customer.NameID=CustomerName.ID ");
        sql.AppendLine("   LEFT JOIN SubRegion ON Customer.CountryID=SubRegion.ID ");
        sql.AppendLine("   LEFT JOIN SalesChannel ON Customer.SalesChannelID=SalesChannel.ID ");
        sql.AppendLine(" WHERE ");
        sql.AppendLine("   CustomerName.ID=" + customerID);
        //if (!string.IsNullOrEmpty(salesChannelID))
        //{
        //    sql.AppendLine("   AND SalesChannel.ID=" + salesChannelID);
        //}
        string sql_customer = sql.ToString();
        // By DingJunjie 20110525 ItemW12 Add End
        DataSet ds_customer = helper.GetDataSet(sql_customer);

        return ds_customer;
    }

    public DataSet getCustomerInfo1(string customerID, string salesChannelID)
    {
        
        StringBuilder sql = new StringBuilder();
        sql.AppendLine(" SELECT ");
        sql.AppendLine("   CustomerName.Name AS 'Customer Name', ");
        sql.AppendLine("   CustomerType.Name AS 'Customer Type', ");
        sql.AppendLine("   SalesChannel.Name AS 'Sales Channel', ");
        sql.AppendLine("   SubRegion.Name AS SubRegion, ");
        sql.AppendLine("   Customer.City, ");
        sql.AppendLine("   Customer.Address, ");
        sql.AppendLine("   Customer.Department ");
        sql.AppendLine(" FROM ");
        sql.AppendLine("   Customer ");
        sql.AppendLine("   INNER JOIN CustomerType ON Customer.TypeID=CustomerType.ID ");
        sql.AppendLine("   INNER JOIN CustomerName ON Customer.NameID=CustomerName.ID ");
        sql.AppendLine("   LEFT JOIN SubRegion ON Customer.CountryID=SubRegion.ID ");
        sql.AppendLine("   LEFT JOIN SalesChannel ON Customer.SalesChannelID=SalesChannel.ID ");
        sql.AppendLine(" WHERE ");
        sql.AppendLine("   Customer.ID=" + customerID);
        
        string sql_customer = sql.ToString();
        
        DataSet ds_customer = helper.GetDataSet(sql_customer);

        return ds_customer;
    }


}
