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

public partial class CustomerNameSelect : System.Web.UI.Page
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
    protected void gvCusName_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        this.gvCusName.PageIndex = e.NewPageIndex;
        dataBind();
    }

    /// <summary>
    /// Query Button On Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnQuery_Click(object sender, EventArgs e)
    {
        this.hidCusNameQuery.Value = this.txtCusNameQuery.Text.Trim();
        this.gvCusName.PageIndex = 0;
        dataBind();
    }
    #endregion

    #region Method
    /// <summary>
    /// Data Bind
    /// </summary>
    /// <param name="cusName">Customer Name</param>
    private void dataBind()
    {
        DataSet ds = null;
        if (string.IsNullOrEmpty(this.hidCusNameQuery.Value))
        {
            ds = sql.getCustomerName1();
            if (ds != null && ds.Tables.Count > 0)
            {
                this.gvCusName.DataSource = ds.Tables[0];
                this.gvCusName.DataBind();
            }
        }
        else
        {
            ds = sql.getCustomerNameByName1(this.hidCusNameQuery.Value);
            if (ds != null && ds.Tables.Count > 0)
            {
                this.gvCusName.DataSource = ds.Tables[0];
                this.gvCusName.DataBind();
            }
        }
    }

    #endregion
}
