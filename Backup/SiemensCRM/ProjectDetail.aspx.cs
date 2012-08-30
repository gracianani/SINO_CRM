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
// By DingJunjie 20110524 Item 7 Add Start
using System.Text;
// By DingJunjie 20110524 Item 7 Delete End

public partial class ProjectDetail : System.Web.UI.Page
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

    protected void gv_Project_RowDataBound(object sender, GridViewRowEventArgs e)
    {

    }

    protected void bindDataSource()
    {
        String projectID = Request.QueryString["projectID"].ToString().Trim();
        String customerID = Request.QueryString["customerID"].ToString().Trim();
        DataSet ds_project = getProjectInfo(projectID,customerID);

        if (ds_project.Tables[0].Rows.Count == 0)
        {
            sql.getNullDataSet(ds_project);
        }

        gv_Project.Width = Unit.Pixel(800);
        gv_Project.AutoGenerateColumns = false;
        gv_Project.AllowPaging = false;
        gv_Project.Visible = true;

        for (int i = 0; i < ds_project.Tables[0].Columns.Count; i++)
        {
            BoundField bf = new BoundField();

            bf.DataField = ds_project.Tables[0].Columns[i].ColumnName.ToString();
            bf.HeaderText = ds_project.Tables[0].Columns[i].Caption.ToString();
            bf.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
            bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
            bf.ReadOnly = true;         
            bf.ControlStyle.Width = bf.ItemStyle.Width;
            gv_Project.Columns.Add(bf);
        }

        gv_Project.DataSource = ds_project.Tables[0];
        gv_Project.DataBind();
    }

    public DataSet getProjectInfo(string projectID, string customerID)
    {
        // By DingJunjie 20110524 Item 7 Delete Start
        //string sql_project = "SELECT [Project].Name AS 'Project Name', [CustomerName].Name AS 'Customer Name',"
        //                + " [Country].ISO_Code AS 'Project Country(POS)',"
        //                + " [Project].Value AS 'Project Value', [Project].Probability AS '% in budget', [Project].Comments"
        //                + " FROM [Project], [Country], [CustomerName]"
        //                + " WHERE [Country].ID = [Project].POSID"
        //                + " AND [CustomerName].ID = [Project].CustomerNameID"
        //                + " AND [Project].Deleted = 0 AND [Country].Deleted = 0";
        //sql_project += " AND [Project].ID = " + projectID ;
        //if (!string.IsNullOrEmpty(customerID))
        //{
        //    sql_project += " AND [Project].CustomerNameID = " + customerID;
        //}
        // By DingJunjie 20110524 Item 7 Delete End
        // By DingJunjie 20110524 Item 7 Add Start
        StringBuilder sql = new StringBuilder();
        sql.AppendLine(" SELECT ");
        sql.AppendLine("   [Project].Name AS 'Project Name', ");
        sql.AppendLine("   [Segment].Abbr AS 'Segment', ");
        sql.AppendLine("   [CustomerName].Name AS 'Customer Name', ");
        sql.AppendLine("   [Country].ISO_Code AS 'Project Country(POS)', ");
        sql.AppendLine("   [Project].Value AS 'Project Value', ");
        sql.AppendLine("   [Project].Probability AS '% in budget', ");
        //by yyan itemw72 20110713 add start 
        sql.AppendLine("   [Currency] .Name AS 'Currency',");
        sql.AppendLine("   [Project].Comments ,");
        sql.AppendLine("    dbo.Project.Value * dbo.Project.Probability / 100 AS 'absolute in budget',");
        //by yyan itemw72 20110713 add end 
        //by yyan itemw51 20110624 add start 
        sql.AppendLine("   [Country1].ISO_Code AS 'Project Country(POD)' ");
        //by yyan itemw51 20110624 add end 
        
        sql.AppendLine(" FROM ");
        sql.AppendLine("   [Project] ");
        sql.AppendLine("   LEFT JOIN [Segment] ON [Project].ProSegmentID=[Segment].ID AND [Segment].Deleted=0 ");
        sql.AppendLine("   LEFT JOIN [Country] ON [Project].POSID=[Country].ID AND [Country].Deleted=0 ");
        //sql.AppendLine("   LEFT JOIN [CustomerName] ON [Project].CustomerNameID=[CustomerName].ID AND [CustomerName].Deleted=0 ");
        sql.AppendLine("   LEFT JOIN [Customer] ON [Project].CustomerNameID=[Customer].ID AND [Customer].Deleted=0 ");
        sql.AppendLine("   LEFT JOIN [CustomerName] ON [Customer].NameID=[CustomerName].ID AND [CustomerName].Deleted=0 ");
        //by yyan itemw51 20110624 add start 
        sql.AppendLine("   LEFT JOIN [Country] [Country1] ON [Project].PODID=[Country1].ID AND [Country1].Deleted=0 ");
        //by yyan itemw51 20110624 add end 
        //by yyan itemw72 20110713 add start 
        sql.AppendLine("   LEFT JOIN [Currency] ON dbo.Project.CurrencyID = [Currency].ID ");
        //by yyan itemw72 20110713 add end 
        sql.AppendLine(" WHERE ");
        sql.AppendLine("   [Project].Deleted=0 ");
        sql.AppendLine("   AND [Project].ID=" + projectID);
        string sql_project = sql.ToString();
        // By DingJunjie 20110524 Item 7 Add End
        DataSet ds_project = helper.GetDataSet(sql_project);

        return ds_project;
    }

}
