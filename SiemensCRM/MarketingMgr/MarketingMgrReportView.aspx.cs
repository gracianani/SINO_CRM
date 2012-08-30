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

public partial class MarketingMgr_MarketingMgrReportView : System.Web.UI.Page
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
        if (!string.Equals(roleID, "2"))
        {
            Response.Redirect("~/AccessDenied.aspx");
            return;
        }

        if (Session["GeneralMarketingMgrID"] != null)
        {
            UserID = Session["GeneralMarketingMgrID"].ToString();
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
        // add by zy 20110130 start
        // display select meeting data
        date.setSelectDate(Session["GeneralMarketingMgrID"].ToString());

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
        string query_region = string.Format("select * from ReportList where UserID = {0} order by ID desc",UserID);  
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
        if (e.CommandName == "EidtReport")
        {
            string reportid = e.CommandArgument.ToString();
            Session["reppageindex"] = gvList.PageIndex;
            Response.Redirect(string.Format("MarketingMgrReportValue.aspx?reportid={0}", reportid));
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
            Response.Redirect(string.Format("MarketingMgrReportCondition.aspx?reportid={0}", reportid));
        }
        else if (e.CommandName == "Run")
        {
            string reportid = e.CommandArgument.ToString();
            Session["reppageindex"] = gvList.PageIndex;
            // update by zy 20110117 start
            //Response.Redirect(string.Format("MarketingMgrReportResult.aspx?reportid={0}", reportid));
            this.Page.RegisterStartupScript("", "<script>window.open('" + string.Format("MarketingMgrReportResult.aspx?reportid={0}", reportid) + "','newwindow','fullscreen=yes, top=0, left=0, toolbar=no, menubar=no, scrollbars=no, resizable=no,location=no, status=no');</script>");
            // update by zy 20110117 end
        }
        //By yyan 20110509 item 53 add start
        else if (e.CommandName == "Share")
        {
            string reportid = e.CommandArgument.ToString();
            ArrayList sqltextlist = new ArrayList();
            string updateSql = string.Format("UPDATE [ReportList] SET FlagShare = 0 WHERE ID={0}", reportid);
            sqltextlist.Add(updateSql);
            helper.ExecuteNonQuery(CommandType.Text, sqltextlist, null);
        }
        else if (e.CommandName == "Rename")
        {
            string reportid = e.CommandArgument.ToString();
            Session["reppageindex"] = gvList.PageIndex;
            Response.Redirect(string.Format("MarketingMgrStandardReportRename.aspx?reportid={0}", reportid));
        }
        //By yyan 20110509 item 53 add end
    }
    protected void lbtnAdd_Click(object sender, EventArgs e)
    {
        Session["reppageindex"] = gvList.PageIndex;
        Response.Redirect("MarketingMgrReportValue.aspx");
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
