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
using System.Text;
using System.Data.SqlClient;


/*
 * File Name      : AdminLockClickSelect.aspx.cs
 * 
 * Description    : User select segment
 * 
 * Author         : mbq
 * 
 * Modify Date    : 2010-12-09
 * 
 * Problem        : problem
 * 
 * Version        : Release (1.0)
 */
public partial class Admin_AdminLockClickSelect : System.Web.UI.Page
{
    SQLHelper helper = new SQLHelper();
    LogUtility log = new LogUtility();
    DisplayInfo info = new DisplayInfo();
    SQLStatement sql = new SQLStatement();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            this.btn_selectAll.Attributes.Add("onclick", "var items = document.getElementsByTagName('input');" +
                "for(i=0;i<items.length;i++){" +
                "if(items[i].type=='checkbox'){items[i].checked = 'checked';}} return false;");
            this.btn_selectNone.Attributes.Add("onclick", "var items = document.getElementsByTagName('input');" +
                "for(i=0;i<items.length;i++){" +
                "if(items[i].type=='checkbox'){items[i].checked = '';}} return false;");
            this.btn_cancel.Attributes.Add("onclick", "closeWindow();return false;");
            if (getRoleID(getRole()) == "0")
            {
                string query_string = null;
                if (Request.QueryString["select"] == "segment")
                {
                    query_string = " SELECT [Segment].ID,[Segment].Abbr AS 'Segment'"
                        + " FROM [Segment] "
                        + " WHERE [Segment].Deleted = 0 AND [Segment].ID not in ( SELECT [LockSegment].SegmentID FROM [LockSegment] where UnlockTime>=GETDATE() )  "
                        + " ORDER BY [Segment].Abbr ASC";
                    Page.Title = "Lock Segment";
                    this.lbl_selectInfo.Text = "Please Select Segment";
                    this.trDate.Attributes.Add("style", "display:''");
                }
                else if (Request.QueryString["select"] == "unsegment")
                {
                    query_string = " SELECT [Segment].ID,[Segment].Abbr AS 'Segment'"
                         + " FROM [Segment] , [LockSegment]"
                         + " WHERE [Segment].Deleted = 0 AND [Segment].ID = LockSegment.SegmentID and [LockSegment].UnlockTime>=GETDATE()"
                         + " ORDER BY [Segment].Abbr ASC";
                    Page.Title = "Unlock Segment";
                    this.lbl_selectInfo.Text = "Please Select Segment";
                    this.trDate.Attributes.Add("style", "display:none");
                }
                if (query_string != null)
                {
                    DataSet ds = helper.GetDataSet(query_string);

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        lbl_nothing.Text = "";
                    }
                    else
                    {
                        if (Request.QueryString["select"] == "segment")
                        {
                            lbl_nothing.Text = "All segments have been locked!";
                        }
                        else if (Request.QueryString["select"] == "unsegment")
                        {
                            lbl_nothing.Text = "There are no locked segments!";
                        }

                    }

                    DataRowCollection rows = ds.Tables[0].Rows;
                    StringBuilder sb = new StringBuilder("[");
                    for (int i = 0; i < rows.Count; i++)
                    {
                        ListItem li = new ListItem(rows[i][1].ToString(), rows[i][0].ToString());
                        chk_list.Items.Add(li);
                        sb.Append("{ id:'" + rows[i][0] + "'},");
                    }
                    if (rows.Count != 0)
                    {
                        this.txt_keyAndValue.Value = sb.ToString().Substring(0, sb.ToString().Length - 1) + "]";
                    }
                    else
                    {
                        this.txt_keyAndValue.Value = sb.ToString() + "]";
                    }
                    this.btn_submit.Attributes.Add("onclick", "modifySegment();return false;");
                }

            }
            else
            {
                Response.Redirect("~/AccessDenied.aspx");
            }
        }
    }

    protected string getRole()
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

    private bool IsExists(String id, DataRowCollection drc)
    {
        bool pd = false;
        foreach (DataRow var in drc)
        {
            if (var[0].ToString() == id)
            {
                pd = true;
                break;
            }
        }
        return pd;
    }

    protected void cal_date_LockSegment_SelectionChanged(object sender, EventArgs e)
    {
        tbox_date_LockSegment.Text = cal_date_LockSegment.SelectedDate.Year.ToString() + "-" + cal_date_LockSegment.SelectedDate.Month.ToString() + "-" + cal_date_LockSegment.SelectedDate.Day.ToString();
    }
}
