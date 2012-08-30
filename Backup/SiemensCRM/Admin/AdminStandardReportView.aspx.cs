/*
* File Name   : AdminStandardReportView.aspx.cs
* 
* Author      : yyan 
* 
* Modified Date : 2011-05-11
* 
* Problem     : none
* 
*/
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
using System.Drawing;
using System.Collections.Generic;
using System.Data.SqlClient;

public partial class AdminStandardReportView : System.Web.UI.Page
{
    SQLHelper helper = new SQLHelper();
    DisplayInfo info = new DisplayInfo();
    SQLStatement sql = new SQLStatement();
    GetMeetingDate date = new GetMeetingDate();
    string UserID = "";
    protected void Page_Load(object sender, EventArgs e)
    {
        string roleID = getRoleID(getRole());
        if (!string.Equals(roleID, "0") && !string.Equals(roleID, "5") && !string.Equals(roleID, "6"))
        {
            Response.Redirect("~/AccessDenied.aspx");
            return;
        }
        if (Session["AdministratorID"] != null)
        {
            UserID = Session["AdministratorID"].ToString();
        }
        else
        {
            UserID = "";
        }
        if (!IsPostBack)
        {
          databindGV();
        }
    }

    private void databindGV()
    {
        gvList.DataSource = getReportList();
        gvList.DataBind();
        if (Session["reppageindex"] != null)
        {
            int pageindex = 0;
            if (int.TryParse(Session["reppageindex"].ToString(), out pageindex))
            {
                if (pageindex < gvList.PageCount)
                {
                    gvList.PageIndex = pageindex;
                }
                else if (pageindex != 0)
                {
                    gvList.PageIndex = pageindex - 1;
                }
            }
            Session.Remove("reppageindex");
            gvList.DataBind();
        }
        date.setSelectDate(Session["AdministratorID"].ToString());

        label_currentmeetingdate.Text = "";
        label_currentmeetingdate.ForeColor = System.Drawing.Color.Green;
        if (date.getyear() == null)
        {
            label_currentmeetingdate.Text = "There is no selected meeting date.";
        }
        else
        {
            label_currentmeetingdate.Text = "The current selected meeting date is " + date.getyear() + "-" + date.getmonth() + "-" + date.getDay();
        }
    }

    protected DataSet getReportList()
    {
        string query_region = string.Format("select * from ReportList where FlagShare =0 order by ID desc",UserID);  
        DataSet ds_region = helper.GetDataSet(query_region);
        return ds_region;
    }

     protected void gvList_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        string reportid = e.CommandArgument.ToString();
        string sql = string.Format("select * from StandardReportValue where id={0} and userid ={1}", reportid, Session["AdministratorID"].ToString());
        DataTable dt = helper.GetDataSet(sql).Tables[0];
        if (dt.Rows.Count <=0)
        {
            string strSelect = "select FieldID,ID,FieldName,FieldType,FieldLength,Operator,Condition1,Condition2,Sort,FieldOrder,NewFieldName from ReportValue where ID = " + reportid;
            DataTable dtReport = helper.GetDataSet(strSelect).Tables[0];
            for (int i = 0; i < dtReport.Rows.Count; i++)
            {
                string sqlInsert = "INSERT INTO StandardReportValue (FieldID,ID,FieldName,FieldType,FieldLength," +
                      "Operator,Condition1,Condition2,Sort,FieldOrder,NewFieldName,UserID) VALUES(@FieldID,@ID,@FieldName," +
                      " @FieldType,@FieldLength,@Operator,@Condition1,@Condition2,@Sort,@FieldOrder,@NEWName,@UserID) ";
                SqlParameter[] sqlParameter = new SqlParameter[12];
                sqlParameter[0] = new SqlParameter("@FieldID", dtReport.Rows[i]["FieldID"].ToString());
                sqlParameter[1] = new SqlParameter("@ID", dtReport.Rows[i]["ID"].ToString());
                sqlParameter[2] = new SqlParameter("@FieldName", dtReport.Rows[i]["FieldName"].ToString());
                sqlParameter[3] = new SqlParameter("@FieldType", dtReport.Rows[i]["FieldType"].ToString());
                sqlParameter[4] = new SqlParameter("@FieldLength", dtReport.Rows[i]["FieldLength"].ToString());
                sqlParameter[5] = new SqlParameter("@Operator", dtReport.Rows[i]["Operator"].ToString());
                sqlParameter[6] = new SqlParameter("@Condition1", dtReport.Rows[i]["Condition1"].ToString());
                sqlParameter[7] = new SqlParameter("@Condition2", dtReport.Rows[i]["Condition2"].ToString());
                sqlParameter[8] = new SqlParameter("@Sort", dtReport.Rows[i]["Sort"].ToString());
                sqlParameter[9] = new SqlParameter("@FieldOrder", dtReport.Rows[i]["FieldOrder"].ToString());
                sqlParameter[10] = new SqlParameter("@NEWName", dtReport.Rows[i]["NewFieldName"].ToString());
                sqlParameter[11] = new SqlParameter("@UserID", Session["AdministratorID"].ToString());
                helper.ExecuteNonQuery(CommandType.Text, sqlInsert, sqlParameter);
            }

        }
         if (e.CommandName == "Factor")
        {
            Session["reppageindex"] = gvList.PageIndex;
            Response.Redirect(string.Format("AdminStandardReportCondition.aspx?reportid={0}", reportid));
        }
        else if (e.CommandName == "Run")
        {
            Session["reppageindex"] = gvList.PageIndex;
            this.Page.RegisterStartupScript("", "<script>window.open('" + string.Format("AdminStandardReportResult.aspx?reportid={0}", reportid) + "','newwindow','fullscreen=yes, top=0, left=0, toolbar=no, menubar=no, scrollbars=no, resizable=no,location=no, status=no');</script>");
        }
        //by yyan 20110817 itemW111 add start
        else 
        {
            Session["reppageindex"] = gvList.PageIndex;
            string updatreRep = string.Format("update [ReportList] set flagshare=null WHERE ID={0}", reportid);
            helper.ExecuteNonQuery(CommandType.Text, updatreRep, null);
            databindGV();
           
        }
        //by yyan 20110817 itemW111 add end
    }

    protected void gvList_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            ImageButton but = (ImageButton)e.Row.FindControl("mbtnDel");
            DataRowView row = (DataRowView)e.Row.DataItem;
            if(but!=null)
            {
                but.Attributes.Add("onclick", info.setRowDataBound(row["Name"].ToString()));
            }
        }

        //by yyan 201108019 itemW119 add start
        #region 
        if (e.Row.RowType == DataControlRowType.Pager)
        {
            DropDownList ddl = (DropDownList)e.Row.FindControl("ddlpage");
            if (ddl != null)
            {
                ddl.Items.Clear();
                for (int i = 1; i <= gvList.PageCount; i++)
                {
                    ddl.Items.Add(new ListItem(i.ToString(), (i - 1).ToString()));
                }
                ddl.SelectedValue = gvList.PageIndex.ToString();
            }
        }
        #endregion
        //by yyan 201108019 itemW119 add end
    }
    
    protected void gvList_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        this.gvList.PageIndex = e.NewPageIndex;
        //databindGV();
    }

    private string getRole()
    {
        return Session["Role"].ToString().Trim();
    }

    private string getRoleID(string str_name)
    {
        DataSet ds_role = sql.getRole();

        for (int i = 0; i < ds_role.Tables[0].Rows.Count; i++)
        {
            if (ds_role.Tables[0].Rows[i][0].ToString().Trim() == str_name)
            {
                return ds_role.Tables[0].Rows[i][1].ToString().Trim();
            }
        }
        return "";
    }

    private void SetErrorInfo(Label label, string errorMsg)
    {
        label.ForeColor = Color.Red;
        label.Text = info.addillegal(errorMsg) + "<br/>";
        this.divError.Controls.Add(label);
    }
    //by yyan 201108019 itemW119 add start
    protected void SelectedIndexChanged(object sender, EventArgs e)
    {
        DropDownList ddl = (DropDownList)sender;
        int page = int.Parse(ddl.SelectedValue);
        gvList.PageIndex = page;
        databindGV();
    }
    //by yyan 201108019 itemW119 add end
}
