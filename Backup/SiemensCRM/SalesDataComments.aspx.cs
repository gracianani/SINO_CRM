/*
 *   File Name   :AdminBooingDataComments.aspx.cs
 *   
 *   Description :
 *   
 *   Author      : WangJun
 * 
 *   Modified    :2010-3-31
 * 
 *   Problem     : 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Drawing;

public partial class SalesDataComments : System.Web.UI.Page
{
    #region Global variable
    private static SQLHelper helper = new SQLHelper();
    private static LogUtility log = new LogUtility();
    private static GetMeetingDate meeting = new GetMeetingDate();
    private static SQLStatement sql = new SQLStatement();
    private static string marketingMgrID;
    private static string salesOrgID;
    private static string segmentID;
    private static string operationID;
    private static string backLogY;
    protected static string year;
    protected static string month;
    #endregion

    #region Event
    protected void Page_Load(object sender, EventArgs e)
    {
        meeting.setDate();
        year = meeting.getyear();
        month = meeting.getmonth();
        marketingMgrID = Request.QueryString["marketingMgrID"].ToString().Trim();
        salesOrgID = Request.QueryString["salesOrgID"].ToString().Trim();
        segmentID = Request.QueryString["segmentID"].ToString().Trim();
        operationID = Request.QueryString["operationID"].ToString().Trim();
        backLogY = Request.QueryString["backLogY"].ToString().Trim();
        log.WriteLog(LogUtility.LogErrorLevel.LOG_ACCESS, "SalesDataComments Access.");
        if (!IsPostBack)
        {
            bindProductInfo();
        }
    }

    public void ddl_SelectedProductChanged(object sender, EventArgs e)
    {
        this.content.Value = getCommentsByProductID(this.ddlist_product.SelectedValue);
    }

    protected void btn_ok_Click(object sender, EventArgs e)
    {
        string str_ProductID = ddlist_product.Text.Trim();
        string str_input = content.Value.ToString().Trim();
        StringBuilder strSQL = new StringBuilder();
        strSQL.AppendLine(" UPDATE ");
        strSQL.AppendLine("   ActualSalesandBL ");
        strSQL.AppendLine(" SET ");
        strSQL.AppendLine("   Comments=@Comments ");
        strSQL.AppendLine(" WHERE ");
        strSQL.AppendLine("   MarketingMgrID=" + marketingMgrID);
        strSQL.AppendLine("   AND SalesOrgID=" + salesOrgID);
        strSQL.AppendLine("   AND SegmentID=" + segmentID);
        strSQL.AppendLine("   AND OperationID=" + operationID);
        strSQL.AppendLine("   AND BacklogY=" + backLogY);
        strSQL.AppendLine("   AND ProductID=" + this.ddlist_product.SelectedValue);
        strSQL.AppendLine("   AND YEAR(TimeFlag)=" + year);
        strSQL.AppendLine("   AND MONTH(TimeFlag)=" + month);
        SqlParameter[] parameters = new SqlParameter[1];
        parameters[0] = new SqlParameter("@Comments", this.content.Value.Trim());
        int count = helper.ExecuteNonQuery(CommandType.Text, strSQL.ToString(), parameters);
        if (count == 1)
        {
            this.label_note.ForeColor = Color.Green;
            this.label_note.Text = "Modified successfully.";
        }
        else
        {
            this.label_note.ForeColor = Color.Red;
            this.label_note.Text = "Please input again.";
        }
    }
    #endregion

    #region Method
    /// <summary>
    /// Bind product infomation
    /// </summary>
    public void bindProductInfo()
    {
        DataSet ds = getProductInfo();
        if (ds == null)
        {
            this.label_note.ForeColor = System.Drawing.Color.Red;
            this.label_note.Text = "Please input amount first!";
            this.ddlist_product.Items.Add(new ListItem("Not Exist", "-1"));
            this.ddlist_product.Enabled = false;
            this.label_input.Visible = false;
            this.content.Visible = false;
            this.btn_ok.Visible = false;
        }
        else
        {
            this.content.Value = getCommentsByProductID(ds.Tables[0].Rows[0][0].ToString());
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                ddlist_product.Items.Add(new ListItem(ds.Tables[0].Rows[i][1].ToString(), ds.Tables[0].Rows[i][0].ToString()));
            }
            ddlist_product.Enabled = true;
        }
    }

    /// <summary>
    /// Get Product information
    /// </summary>
    /// <returns>Product information</returns>
    public DataSet getProductInfo()
    {
        StringBuilder strSQL = new StringBuilder();
        strSQL.AppendLine(" SELECT ");
        strSQL.AppendLine("   ActualSalesandBL.ProductID, ");
        strSQL.AppendLine("   Product.Abbr ");
        strSQL.AppendLine(" FROM ");
        strSQL.AppendLine("   ActualSalesandBL INNER JOIN Product ON ActualSalesandBL.ProductID=Product.ID ");
        strSQL.AppendLine(" WHERE ");
        strSQL.AppendLine("   ActualSalesandBL.MarketingMgrID=" + marketingMgrID);
        strSQL.AppendLine("   AND ActualSalesandBL.SalesOrgID=" + salesOrgID);
        strSQL.AppendLine("   AND ActualSalesandBL.SegmentID=" + segmentID);
        strSQL.AppendLine("   AND ActualSalesandBL.OperationID=" + operationID);
        strSQL.AppendLine("   AND ActualSalesandBL.BacklogY=" + backLogY);
        strSQL.AppendLine("   AND YEAR(ActualSalesandBL.TimeFlag)=" + year);
        strSQL.AppendLine("   AND MONTH(ActualSalesandBL.TimeFlag)=" + month);
        strSQL.AppendLine("   AND Product.Deleted=0 ");
        strSQL.AppendLine(" ORDER BY ");
        strSQL.AppendLine("   Product.Abbr ");
        DataSet ds = helper.GetDataSet(strSQL.ToString());
        if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
        {
            return null;
        }
        else
        {
            return ds;
        }
    }

    /// <summary>
    /// Get comments by product ID
    /// </summary>
    /// <param name="productID">Product ID</param>
    /// <returns>Comments</returns>
    public string getCommentsByProductID(string productID)
    {
        StringBuilder strSQL = new StringBuilder();
        strSQL.AppendLine(" SELECT ");
        strSQL.AppendLine("   ActualSalesandBL.Comments ");
        strSQL.AppendLine(" FROM ");
        strSQL.AppendLine("   ActualSalesandBL");
        strSQL.AppendLine(" WHERE ");
        strSQL.AppendLine("   ActualSalesandBL.MarketingMgrID=" + marketingMgrID);
        strSQL.AppendLine("   AND ActualSalesandBL.SalesOrgID=" + salesOrgID);
        strSQL.AppendLine("   AND ActualSalesandBL.SegmentID=" + segmentID);
        strSQL.AppendLine("   AND ActualSalesandBL.OperationID=" + operationID);
        strSQL.AppendLine("   AND ActualSalesandBL.BacklogY=" + backLogY);
        strSQL.AppendLine("   AND ActualSalesandBL.ProductID=" + productID);
        strSQL.AppendLine("   AND YEAR(ActualSalesandBL.TimeFlag)=" + year);
        strSQL.AppendLine("   AND MONTH(ActualSalesandBL.TimeFlag)=" + month);
        DataSet ds = helper.GetDataSet(strSQL.ToString());
        if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
        {
            return string.Empty;
        }
        else
        {
            return ds.Tables[0].Rows[0][0].ToString();
        }
    }
    #endregion
}
