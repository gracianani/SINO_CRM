/*
* File Name   : AssistantStandardReportResult.aspx.cs
* 
* Author      : yyan 
* 
* Modified Date : 2011-05-11
* 
* Problem     : none
* 
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using Resources;
public partial class AssistantStandardReportResult : System.Web.UI.Page
{
    SQLHelper helper = new SQLHelper();
    LogUtility log = new LogUtility();
    SQLStatement state = new SQLStatement();
    WebUtility webU = new WebUtility();
    DisplayInfo info = new DisplayInfo();
    CommonFunction cf = new CommonFunction();
   
    protected void Page_Load(object sender, EventArgs e)
    {
        string roleID = getRoleID(getRole());
        if (!string.Equals(roleID, "0") && !string.Equals(roleID, "5") && !string.Equals(roleID, "6"))
        {
            Response.Redirect("~/AccessDenied.aspx");
            return;
        }

        Page.MaintainScrollPositionOnPostBack = true;

        if (!IsPostBack)
        {
            HidReportId.Value = Request.QueryString["reportid"].ToString();
            log.WriteLog(LogUtility.LogErrorLevel.LOG_ACCESS, "AssistantAccountProfile Access.");
            this.gv_administrator.Attributes["SortDirection"] = "Asc";
            bindDataSource(getAdministratorInfo(""));
            //by yyan itemw49 20110624 add start
            lbl_welcom.Text = "Welcome " + Session["Alias"].ToString();
            //by yyan itemw49 20110624 add end

        }
    }
    
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
        //if (e.Row.RowType == DataControlRowType.Header)
        //{
        //    (e.Row.Cells[1].Controls[0])";
        //}
        //if (e.Row.RowType == DataControlRowType.DataRow)
        //{
        //    CheckBox check = (CheckBox)e.Row.FindControl("chkCheck");
        //    if (check != null)
        //    {
        //        string shouValue;
        //        getSessionValue(out shouValue);
        //        string curValue = "'" + ((DataRowView)e.Row.DataItem)["ShowValue"].ToString().Trim() + "'";
        //        check.Checked = isSame(shouValue.Trim(), curValue.Trim());
        //    }
        //}
        #endregion
    }

    
    protected DataSet getAdministratorInfo(string strOrdey)
    {
        string sql_searchUser;

        StandRepComToSQL sql = new StandRepComToSQL();
        string sqlReport = "SELECT ViewName FROM ReportList where ID = " + HidReportId.Value;
        DataSet ds_qreport = helper.GetDataSet(sqlReport);
        string viewName = ds_qreport.Tables[0].Rows[0][0].ToString();
        if (viewName.Equals("BookingsSales"))
        {
            sql_searchUser = sql.RepCompileSql(HidReportId.Value, getLoginRole(), "", "", strOrdey, sql.getSelectMeetingDate(Session["AssistantID"].ToString()), Session["AssistantID"].ToString());
        }
        else if (viewName.Equals("SalesBacklog"))
        {
            sql_searchUser = sql.RepCompileSqlBacklog(HidReportId.Value, getLoginRole(), strOrdey, strOrdey, Session["AssistantID"].ToString());
        }
        //by yyan ItemW149 20110916 add start
        //else if (viewName.Equals("ProjectInformation"))
        //{
        //    sql_searchUser = sql.RepCompileSqlA(HidReportId.Value, getLoginRole(), "", "", strOrdey, viewName);
        //}
        //by yyan ItemW149 20110916 add end
        else {
            sql_searchUser = sql.RepCompileSql(HidReportId.Value, getLoginRole(), "", "", strOrdey,Session["AssistantID"].ToString());
        }
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
        gv_administrator.Visible = true;
        gv_administrator.DataSource = ds.Tables[0];
        gv_administrator.DataBind();
    }

    
    protected void gv_administrator_Sorting(object sender, GridViewSortEventArgs e)
    {
        string sortExpression = e.SortExpression;
        string strOrder = null;
        string sortDirection = "Asc";
        if (sortDirection == gv_administrator.Attributes["SortDirection"].ToString())
        {
            sortDirection = "Desc";
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
        }
        else
        {
            sortDirection = "Asc";

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
        exportToExcel2();
    }

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

    
    private string getLoginRole()
    {
        string strReturn = "";
        if (getRoleID(getRole()) != "0")
        {
            strReturn = Session["AssistantID"].ToString().Trim();
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
