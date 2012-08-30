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
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
public partial class Admin_AdminReportView : System.Web.UI.Page
{
    SQLHelper helper = new SQLHelper();
    DisplayInfo info = new DisplayInfo();
    SQLStatement sql = new SQLStatement();
    // add by zy 20110130 start
    GetMeetingDate date = new GetMeetingDate();
    // add by zy 20110130 end
    string UserID = "";
    protected void Page_Load(object sender, EventArgs e)
    {

        string roleID = getRoleID(getRole());
        label_add.Text = "";
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
        //by yyan itemw81 20110721 add start
        for (int i = 0; i < this.gvList.Rows.Count; i++)
        {
            HiddenField hidFlagShare = (HiddenField)this.gvList.Rows[i].Cells[3].FindControl("hidFlagShare");
            if (hidFlagShare.Value.ToString() == "0")
            {
                ImageButton imageButton = (ImageButton)this.gvList.Rows[i].Cells[3].FindControl("mbtnShare");
                imageButton.Visible = false;
            }
            HiddenField hidUserId = (HiddenField)this.gvList.Rows[i].Cells[3].FindControl("hidUserId");
            if (UserID.Equals(hidUserId.Value))
            {
               //by yyan 20110817 itemW111 del start
               // ImageButton imageButton = (ImageButton)this.gvList.Rows[i].Cells[3].FindControl("mbtnCopy");
               // imageButton.Visible = false;
               //by yyan 20110817 itemW111 del end
            }
            else {
                ImageButton imageButton = (ImageButton)this.gvList.Rows[i].Cells[3].FindControl("mbtnShare");
                imageButton.Visible = false;
            }
        }
        //by yyan itemw81 20110721 add end
        // add by zy 20110130 start
        // display select meeting data
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
        // add by zy 20110130 end
    }

    protected DataSet getReportList()
    {
        //by yy itemw79 20110720 edit start
        //string query_region = string.Format("select * from ReportList where UserID = {0} order by ID desc",UserID);
        string query_region = "select * from ReportList order by ID desc";
        //by yy itemw79 20110720 edit end
        DataSet ds_region = helper.GetDataSet(query_region);
        return ds_region;
    }

    private void DelReport(string RepID)
    {
        ArrayList sqltextlist = new ArrayList();
        string delrep = string.Format("DELETE FROM [ReportList] WHERE ID={0}", RepID);
        string delrepv = string.Format("DELETE FROM [ReportValue] WHERE ID={0}", RepID);
        //by yyan 20110510 item 53 add start
        string delstandardrepv = string.Format("DELETE FROM [StandardReportValue] WHERE ID={0}", RepID);
        sqltextlist.Add(delstandardrepv);
        //by yyan 20110510 item 53 add end
        sqltextlist.Add(delrep);
        sqltextlist.Add(delrepv);
        helper.ExecuteNonQuery(CommandType.Text, sqltextlist, null);
    }

    protected void gvList_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        label_add.Text = "";
        if (e.CommandName == "EidtReport")
        {
            string reportid = e.CommandArgument.ToString();
            Session["reppageindex"] = gvList.PageIndex;

            Response.Redirect(string.Format("AdminReportValue.aspx?reportid={0}", reportid));
        }
        else if (e.CommandName == "DelReport")
        {
            string reportid = e.CommandArgument.ToString();
            Session["reppageindex"] = gvList.PageIndex;
            DelReport(reportid);
            databindGV();
        }
        else if (e.CommandName == "Factor")
        {
            string reportid = e.CommandArgument.ToString();
            Session["reppageindex"] = gvList.PageIndex;
            Response.Redirect(string.Format("AdminReportCondition.aspx?reportid={0}", reportid));
        }
        else if (e.CommandName == "Run")
        {
            string reportid = e.CommandArgument.ToString();
            Session["reppageindex"] = gvList.PageIndex;
            // update by zy 20110117 start
            //Response.Redirect(string.Format("AdminReportResult.aspx?reportid={0}", reportid));
            this.Page.RegisterStartupScript("", "<script>window.open('" + string.Format("AdminReportResult.aspx?reportid={0}", reportid) + "','newwindow','fullscreen=yes, top=0, left=0, toolbar=no, menubar=no, scrollbars=no, resizable=no,location=no, status=no');</script>");
            // update by zy 20110117 end
        }
        //By yyan 20110509 item 53 add start
        else if (e.CommandName == "Share")
        {
            string reportid = e.CommandArgument.ToString();
            ArrayList sqltextlist = new ArrayList();
            string updateSql = string.Format("UPDATE [ReportList] SET FlagShare = 0 WHERE ID={0}", reportid);
            //by yyan 20110517 item 53 add start
            string delSql = string.Format("DELETE FROM [StandardReportValue] WHERE ID={0}", reportid);
            sqltextlist.Add(delSql);
            //by yyan 20110517 item 53 add end
            sqltextlist.Add(updateSql);
            helper.ExecuteNonQuery(CommandType.Text, sqltextlist, null);
            databindGV();
        }
        else if (e.CommandName == "Rename")
        {
            string reportid = e.CommandArgument.ToString();
            Session["reppageindex"] = gvList.PageIndex;
            Response.Redirect(string.Format("AdminStandardReportRename.aspx?reportid={0}", reportid));
        }
        else if (e.CommandName == "Copy")
        {
            string reportid = e.CommandArgument.ToString();
            Session["reppageindex"] = gvList.PageIndex;
            string queryReportList = "select * from ReportList where id=" + reportid;
            DataSet ds_reportList = helper.GetDataSet(queryReportList);
            if (ds_reportList.Tables[0].Rows.Count > 0)
            {
                
                DataSet ds_userName = helper.GetDataSet("select Abbr from [user] where userid='" + UserID+"'");
                if (ds_userName.Tables[0].Rows.Count >0)
                {
                    string queryName=ds_reportList.Tables[0].Rows[0]["name"].ToString().Trim() +"_Shared By " +ds_userName.Tables[0].Rows[0]["Abbr"].ToString().Trim(); 
                    DataSet ds_countByUser=helper.GetDataSet("select * from ReportList where userid='" + UserID + "' and name = '" + queryName+"'");
                    if (ds_countByUser.Tables[0].Rows.Count > 0)
                    {
                        ArrayList sqltextlist = new ArrayList();
                        string delSql = string.Format("DELETE FROM [ReportList] WHERE ID={0}", ds_countByUser.Tables[0].Rows[0]["ID"].ToString().Trim());
                        sqltextlist.Add(delSql);
                        delSql = string.Format("DELETE FROM [reportvalue] WHERE ID={0}", ds_countByUser.Tables[0].Rows[0]["ID"].ToString().Trim());
                        sqltextlist.Add(delSql);
                        delSql = string.Format("DELETE FROM [StandardReportValue] WHERE ID={0}", ds_countByUser.Tables[0].Rows[0]["ID"].ToString().Trim());
                        sqltextlist.Add(delSql);
                        helper.ExecuteNonQuery(CommandType.Text, sqltextlist, null);
                    }
                    string date = DateTime.Now.ToString("yyyy-MM-dd");
                    SqlParameter[] sqlParameterReprot = new SqlParameter[5];
                    sqlParameterReprot[0] = new SqlParameter("@Name", queryName);
                    sqlParameterReprot[1] = new SqlParameter("@Depiction", ds_reportList.Tables[0].Rows[0]["Depiction"].ToString().Trim());
                    sqlParameterReprot[2] = new SqlParameter("@UserID", UserID);
                    sqlParameterReprot[3] = new SqlParameter("@ViewName", ds_reportList.Tables[0].Rows[0]["ViewName"].ToString().Trim());
                    sqlParameterReprot[4] = new SqlParameter("@CreateDate", date);
                    StringBuilder sqlReportlist = new StringBuilder();
                    sqlReportlist.Append("\n INSERT INTO ReportList(Name,Depiction,UserID,ViewName,CreateDate)");
                    sqlReportlist.Append("\n VALUES(@Name,@Depiction,@UserID,@ViewName,@CreateDate)");
                    //insert reportlist
                    int iError = helper.ExecuteNonQuery(CommandType.Text, sqlReportlist.ToString(), sqlParameterReprot);
                    if (iError == 1)
                    {
                        StringBuilder sql = new StringBuilder();
                        sql.Append("\n SELECT ID FROM ReportList where Name='" + queryName + "'");
                        sql.Append("\n AND UserId=" + UserID + " AND ViewName='" + ds_reportList.Tables[0].Rows[0]["ViewName"].ToString().Trim() + "'");
                        DataSet ds = helper.GetDataSet(sql.ToString());
                        string strId = ds.Tables[0].Rows[0]["ID"].ToString();
                        insertReportValueInfo(strId, reportid);
                        label_add.Text = "Successfully copied the report! <br> Please use this copied report(" + queryName + ") to share!";
                    }
                    else {
                        label_add.Text = "Failed to copy!";
                    }
                }
            }
            databindGV();
        }
        //By yyan 20110509 item 53 add end
    }

    private void insertReportValueInfo(string strId,string reportid) {
    
        string sql = "INSERT INTO ReportValue (ID,FieldName,FieldType,FieldLength," +
                     "Operator,Condition1,Condition2,Sort,FieldOrder,NewFieldName) VALUES(@ID,@FieldName," +
                     " @FieldType,@FieldLength,@Operator,@Condition1,@Condition2,@Sort,@FieldOrder,@NEWName) ";
        DataSet ds = helper.GetDataSet("select * from ReportValue where id=" + reportid);
        for (var i = 0; i < ds.Tables[0].Rows.Count; i++)
        {
            SqlParameter[] sqlParameter = new SqlParameter[10];
            sqlParameter[0] = new SqlParameter("@ID", strId);
            sqlParameter[1] = new SqlParameter("@FieldName", ds.Tables[0].Rows[i]["FieldName"].ToString().Trim());
            sqlParameter[2] = new SqlParameter("@FieldType", ds.Tables[0].Rows[i]["FieldType"].ToString().Trim());
            sqlParameter[3] = new SqlParameter("@FieldLength", ds.Tables[0].Rows[i]["FieldLength"].ToString().Trim());
            sqlParameter[4] = new SqlParameter("@Operator", ds.Tables[0].Rows[i]["Operator"].ToString().Trim());
            sqlParameter[5] = new SqlParameter("@Condition1", ds.Tables[0].Rows[i]["Condition1"].ToString().Trim());
            sqlParameter[6] = new SqlParameter("@Condition2", ds.Tables[0].Rows[i]["Condition2"].ToString().Trim());
            sqlParameter[7] = new SqlParameter("@Sort", ds.Tables[0].Rows[i]["Sort"].ToString().Trim());
            sqlParameter[8] = new SqlParameter("@FieldOrder", ds.Tables[0].Rows[i]["FieldOrder"].ToString().Trim());
            sqlParameter[9] = new SqlParameter("@NEWName", ds.Tables[0].Rows[i]["NewFieldName"].ToString().Trim());
            helper.ExecuteNonQuery(CommandType.Text, sql, sqlParameter);
        }

    }
    protected void lbtnAdd_Click(object sender, EventArgs e)
    {
        Session["reppageindex"] = gvList.PageIndex;
        Response.Redirect("AdminReportValue.aspx");
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
        //by yyan 201108019 itemW119 del start
        //databindGV();
        //by yyan 201108019 itemW119 del end
    }

    /* Get user'role */
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
