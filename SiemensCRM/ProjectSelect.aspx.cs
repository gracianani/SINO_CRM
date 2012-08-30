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
using System.Text;
public partial class ProjectSelect : System.Web.UI.Page
{
    #region Global Variable
    SQLStatement sql = new SQLStatement();
    #endregion

    #region Event
    /// <summary>
    /// Onload
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        dataBind();
    }

    /// <summary>
    /// Page Index Changeing
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void gvProName_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        this.gvProName.PageIndex = e.NewPageIndex;
        dataBind();
    }

    /// <summary>
    /// Query Button On Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnQuery_Click(object sender, EventArgs e)
    {
        this.hidProNameQuery.Value = this.txtProNameQuery.Text.Trim();
        this.gvProName.PageIndex = 0;
        dataBind();
    }
    #endregion

    #region Method
    /// <summary>
    /// Data Bind
    /// </summary>
    /// <param name="cusName">Project Name</param>
    private void dataBind()
    {
        DataSet ds = null;
        if (string.IsNullOrEmpty(this.hidProNameQuery.Value))
        {
            ds = getProject();
            if (ds != null && ds.Tables.Count > 0)
            {
                this.gvProName.DataSource = ds.Tables[0];
                this.gvProName.DataBind();
            }
        }
        else
        {
            ds = getProjectNameByName(this.hidProNameQuery.Value);
            if (ds != null && ds.Tables.Count > 0)
            {
                this.gvProName.DataSource = ds.Tables[0];
                this.gvProName.DataBind();
            }
        }
    }

    #endregion


    private DataSet getProject()
    {
        SQLHelper helper = new SQLHelper();
        StringBuilder sql = new StringBuilder();
        sql.AppendLine(" SELECT ");
        sql.AppendLine("   P.ID, ");
        sql.AppendLine("   P.Name+'('+C.ISO_Code+')' 'Project Name' ");
        sql.AppendLine(" FROM ");
        sql.AppendLine("   Project P LEFT JOIN Country C ON P.PoDID=C.ID ");
        sql.AppendLine(" WHERE ");
        sql.AppendLine("   P.Deleted=0 ");
        sql.AppendLine(" ORDER BY ");
        sql.AppendLine("   P.Name ");
        DataSet ds_project = helper.GetDataSet(sql.ToString());
        if (ds_project.Tables.Count > 0)
        {
            return ds_project;
        }
        else
        {
            return null;
        }
    }

    private DataSet getProjectNameByName(string proName)
    {
        SQLHelper helper = new SQLHelper();
        StringBuilder sql = new StringBuilder();
        sql.AppendLine(" SELECT ");
        sql.AppendLine("   P.ID, ");
        sql.AppendLine("   P.Name+'('+C.ISO_Code+')' 'Project Name' ");
        sql.AppendLine(" FROM ");
        sql.AppendLine("   Project P LEFT JOIN Country C ON P.PoDID=C.ID ");
        sql.AppendLine(" WHERE ");
        sql.AppendLine("    P.Name  LIKE @NAME ESCAPE '/' AND P.Deleted=0 ");
        SqlParameter[] sqlParams = { new SqlParameter("@NAME", "%" + proName.Replace("/", "//").Replace("%", "/%").Replace("_", "/_") + "%") };
        return helper.GetDataSet(sql.ToString() , CommandType.Text, sqlParams);
    }
}
