using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
// del by zy 20110104 start
//using Microsoft.Office.Interop.Excel;
// del by zy 20110104 end
using System.IO;
using System.Reflection;
using Resources;
public partial class RSMReportResult : System.Web.UI.Page
{
    SQLHelper helper = new SQLHelper();
    LogUtility log = new LogUtility();
    SQLStatement state = new SQLStatement();
    WebUtility webU = new WebUtility();
    DisplayInfo info = new DisplayInfo();
    // add by zy 20110104 start
    CommonFunction cf = new CommonFunction();
    // add by zy 20110104 end

    
    protected void Page_Load(object sender, EventArgs e)
    {
        string roleID = getRoleID(getRole());
        if (!string.Equals(roleID, "4"))
        {
            Response.Redirect("~/AccessDenied.aspx");
            return;
        }

        Page.MaintainScrollPositionOnPostBack = true;

        if (!IsPostBack)
        {
            HidReportId.Value = Request.QueryString["reportid"].ToString();
            log.WriteLog(LogUtility.LogErrorLevel.LOG_ACCESS, "AdminAccountProfile Access.");
            this.gv_administrator.Attributes["SortDirection"] = "Asc";
            bindDataSource(getAdministratorInfo(""));
            //by yyan itemw49 20110624 add start
            lbl_welcom.Text = "Welcome " + Session["Alias"].ToString();
            //by yyan itemw49 20110624 add end

        }
    }



   
    protected DataSet getAdministratorInfo(string strOrdey)
    {
        string sql_searchUser;

        ReportCompileToSQL sql = new ReportCompileToSQL();
        // update by zy 20110120 start
        //sql_searchUser = sql.RepCompileSql(HidReportId.Value, getLoginRole(), "", "", strOrdey);
        //BY yyan 20110510 item 17 del start
        //sql_searchUser = sql.RepCompileSql(HidReportId.Value, getLoginRole(), "", "", strOrdey, sql.getSelectMeetingDate());
        //BY yyan 20110510 item 17 del end
        // update by zy 20110120 end        
        //BY yyan 20110510 item 17 add start
        string sqlReport = "SELECT ViewName FROM ReportList where ID = " + HidReportId.Value;
        DataSet ds_qreport = helper.GetDataSet(sqlReport);
        string viewName = ds_qreport.Tables[0].Rows[0][0].ToString();
        if (viewName.Equals("BookingsSales"))
        {
            sql_searchUser = sql.RepCompileSql(HidReportId.Value, getLoginRole(), "", "", strOrdey, sql.getSelectMeetingDate(Session["RSMID"].ToString()));
        }
        else if (viewName.Equals("SalesBacklog"))
        {
            sql_searchUser = sql.RepCompileSqlBacklog(HidReportId.Value, getLoginRole(), strOrdey, sql.getSelectMeetingDate(Session["RSMID"].ToString()));
        }
        else
        {
            sql_searchUser = sql.RepCompileSql(HidReportId.Value, getLoginRole(), "", "", strOrdey);
        }

        //BY yyan 20110510 item 17 add end
        DataSet ds_query_Admin = helper.GetDataSet(sql_searchUser);
        return ds_query_Admin;
    }

    
    protected void bindDataSource(DataSet ds)
    {
        if (ds.Tables[0].Rows.Count == 0)
        {
            state.getNullDataSet(ds);
        }
        gv_administrator.Width = Unit.Pixel(1000);
        // del by zy 20110104 start
        // gv_administrator.AllowPaging = true;
        // del by zy 20110104 end
        gv_administrator.Visible = true;
        gv_administrator.DataSource = ds.Tables[0];
        gv_administrator.DataBind();
    }

    // add by zy 20110119 start
    /// <summary>
    /// RowDataBound
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void gv_administrator_hid_RowDataBound(object sender, GridViewRowEventArgs e)
    {

        #region 
        if (e.Row.RowType == DataControlRowType.Header)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                string tmp = ((System.Web.UI.WebControls.DataControlFieldCell)(e.Row.Cells[i])).ContainingField.HeaderText.ToString();
                if ((tmp == "Amount") || (tmp == "DeliverY") || (tmp == "BookingY"))
                {
                    e.Row.Cells[i].Attributes.Add("align", "right");
                }
                else
                {
                    e.Row.Cells[i].Attributes.Add("align", "center");
                }
            }
        }
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            DataRowView view = (DataRowView)e.Row.DataItem;
            for (int i = 0; i < view.DataView.Table.Columns.Count; i++)
            {
                string tmp = view.DataView.Table.Columns[i].ColumnName;
                if ((tmp == "Amount") || (tmp == "DeliverY") || (tmp == "BookingY"))
                {
                    e.Row.Cells[i].Attributes.Add("align", "right");
                }
                else
                {
                    e.Row.Cells[i].Attributes.Add("align", "center");
                }
            }
        }
        #endregion
    }

    /// <summary>
    /// RowDataBound
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void gv_administrator_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        #region 
        if (e.Row.RowType == DataControlRowType.Header)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                string tmp = ((System.Web.UI.WebControls.DataControlFieldCell)(e.Row.Cells[i])).ContainingField.HeaderText.ToString();
                if ((tmp == "Amount") || (tmp == "DeliverY") || (tmp == "BookingY"))
                {
                    e.Row.Cells[i].Attributes.Add("align", "right");
                }
                else
                {
                    e.Row.Cells[i].Attributes.Add("align", "center");
                }
            }
        }
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            DataRowView view = (DataRowView)e.Row.DataItem;
            for (int i = 0; i < view.DataView.Table.Columns.Count; i++)
            {
                string tmp = view.DataView.Table.Columns[i].ColumnName;
                if ((tmp == "Amount") || (tmp == "DeliverY") || (tmp == "BookingY"))
                {
                    e.Row.Cells[i].Attributes.Add("align", "right");
                }
                else
                {
                    e.Row.Cells[i].Attributes.Add("align", "center");
                }
            }
        }
        #endregion
    }
    // add by zy 20110119 end 

    
    protected void gv_administrator_Sorting(object sender, GridViewSortEventArgs e)
    {
        string sortExpression = e.SortExpression;
        string strOrder = null;
        string sortDirection = "Asc";
        if (sortDirection == gv_administrator.Attributes["SortDirection"].ToString())
        {
            sortDirection = "Desc";
            // update by zy 20110106 start
            //strOrder = "cast( " + sortExpression + " as varchar) desc";
            //by yyan 20110524 w10 del start 
            //if (sortExpression.Equals("Amount"))
            //by yyan 20110524 w10 del end 
            //by yyan 20110524 w10 add start 
            //by yyan 20110901 itemw132 edit start
            if (sortExpression.Equals(Resource.Amount) || sortExpression.Equals(Resource.AmountEUR)
                || sortExpression.Equals(Resource.GAPrice) || sortExpression.Equals(Resource.KPrice))
            //by yyan 20110901 itemw132 edit end
            //by yyan 20110524 w10 add end
            {
                strOrder = sortExpression + " desc";
            }
            else
            {
                strOrder = "cast( " + sortExpression + " as varchar) desc";
            }
            // update by zy 20110106 end
        }
        else
        {
            sortDirection = "Asc";
            // update by zy 20110106 start
            //strOrder = "cast(  " + sortExpression + " as varchar)  asc";
            //by yyan 20110524 w10 del start 
            //if (sortExpression.Equals("Amount"))
            //by yyan 20110524 w10 del end 
            //by yyan 20110524 w10 add start 
            //by yyan 20110901 itemw132 edit start
            if (sortExpression.Equals(Resource.Amount) || sortExpression.Equals(Resource.AmountEUR)
                || sortExpression.Equals(Resource.GAPrice) || sortExpression.Equals(Resource.KPrice))
            //by yyan 20110901 itemw132 edit end
            //by yyan 20110524 w10 add end
            {
                strOrder = sortExpression + " asc";
            }
            else
            {
                strOrder = "cast( " + sortExpression + " as varchar) asc";
            }
            // update by zy 20110106 end
        }
        HidOrderBy.Value = strOrder;
        this.gv_administrator.Attributes["SortDirection"] = sortDirection;
        this.gv_administrator.Attributes.Add("SortExpression", sortExpression);
        bindDataSource(getAdministratorInfo(strOrder));
    }
    
    protected void gv_administrator_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        gv_administrator.Columns.Clear();
        gv_administrator.PageIndex = e.NewPageIndex;
        bindDataSource(getAdministratorInfo(""));
    }
    
    protected void btn_output_Click(object sender, EventArgs e)
    {
        // update by zy 20110104 start
        //exportToExcel(Response, getAdministratorInfo(""), "ReportResult.xls");
        exportToExcel2();
        // update by zy 20110104 end 
    }

    // add by zy 20110104 start
    protected void exportToExcel2()
    {
        div_export.Visible = true;
        bindDataSourceTmp(getAdministratorInfo(HidOrderBy.Value));

       
        string tempFileName = "ExcelReport.xls";
        cf.ToExcel(div_export, tempFileName);

        div_export.Visible = false;
    }

    
    protected void bindDataSourceTmp(DataSet ds)
    {
        if (ds.Tables[0].Rows.Count == 0)
        {
            state.getNullDataSet(ds);
        }
        gv_administrator_hid.Width = Unit.Pixel(1000);
        gv_administrator.AllowPaging = false;
        gv_administrator_hid.DataSource = ds.Tables[0];
        gv_administrator_hid.DataBind();
    }

    public override void VerifyRenderingInServerForm(Control control)
    {
        // Confirms that an HtmlForm control is rendered for
    }
    // add by zy 20110104 end

    
   


    /// <summary>
    /// get RSMID
    /// </summary>
    /// <returns></returns>
    private string getLoginRole()
    {
        string strReturn = "";
        if (getRoleID(getRole()) != "0")
        {
            strReturn = Session["RSMID"].ToString().Trim();
        }
        return strReturn;
    }
    protected string getRole()
    {
        return Session["Role"].ToString().Trim();
    }

    protected string getRoleID(string str_name)
    {
        string query_role = "SELECT ID FROM [Role] WHERE Name = '" + str_name + "'";
        DataSet ds_role = helper.GetDataSet(query_role);
        return ds_role.Tables[0].Rows[0][0].ToString().Trim();
    }

   
}
