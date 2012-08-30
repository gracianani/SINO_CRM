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

public partial class Executive_ExecutiveSelectMeetingDate : System.Web.UI.Page
{
    SQLHelper helper = new SQLHelper();
    LogUtility log = new LogUtility();
    SQLStatement sql = new SQLStatement();

    GetMeetingDate date = new GetMeetingDate();
    CommonFunction cf = new CommonFunction();
    DisplayInfo info = new DisplayInfo();

    protected void Page_Load(object sender, EventArgs e)
    {

        if (getRoleID(getRole()) != "1")
        {
            Response.Redirect("~/AccessDenied.aspx");
            return;
        }

        if (!IsPostBack)
        {
            bindDropDownList();
            refreshSession();
        }
    }

    /* Get user'role */
    private string getRole()
    {
        return Session["Role"].ToString().Trim();
    }

    private void refreshSession()
    {
        //Session["SelectMeetingDate"] = date.setDisplaySelectMeetingDate();
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

    public void bindDropDownList(DropDownList ddl, DataSet ds)
    {
        if (ds.Tables[0].Rows.Count > 0)
        {
            ddl.Items.Add(new ListItem("", "0"));
            DataTable dt = ds.Tables[0];
            int count = dt.Rows.Count;
            int index = 0;
            while (index < count)
            {
                string str_displayYear = dt.Rows[index][2].ToString().Trim();
                string str_displayMonth = dt.Rows[index][1].ToString().Trim();
                //if (str_displayMonth.Equals("10"))
                //    str_displayYear = (int.Parse(str_displayYear) + 1).ToString().Trim();
                //string str_display = date.getMeetingName(int.Parse(str_displayMonth)) + " " + str_displayYear;
                string str_display = getDisplayDate(str_displayYear, str_displayMonth);
                ddl.Items.Add(new ListItem(str_display, dt.Rows[index][0].ToString().Trim()));
                index++;
            }
            ddl.Enabled = true;
        }
        else
        {
            ddl.Items.Add(new ListItem("Not Exist", "-1"));
            ddl.Enabled = false;
        }
    }

    private string getDisplayDate(string str_displayYear, string str_displayMonth)
    {
        if (str_displayMonth.Equals("10"))
            str_displayYear = (int.Parse(str_displayYear) + 1).ToString().Trim();
        return date.getMeetingName(int.Parse(str_displayMonth)) + " " + str_displayYear;
    }

    private void bindDropDownList()
    {
        //by yyan 20110818 itemW112 edit start
        date.setSelectDate(Session["ExecutiveID"].ToString());
        //by yyan 20110818 itemW112 edit end
        DataSet ds = date.getMeetingDate();
        ddlist_meetingdate.Items.Clear();
        bindDropDownList(ddlist_meetingdate, ds);

        label_currentmeetingdate.Text = "";
        label_currentmeetingdate.ForeColor = System.Drawing.Color.Green;
        if (date.getyear() == null)
        {
            label_currentmeetingdate.Text = "There is no selected meeting date.";
        }
        else
        {
            label_currentmeetingdate.Text = "The current selected meeting date is " + date.getyear() + "-" + date.getmonth() + "-" + date.getDay();
            string str_displayDate = getDisplayDate(date.getyear(), date.getmonth());
            for (int i = 0; i < ddlist_meetingdate.Items.Count; i++)
            {
                if (ddlist_meetingdate.Items[i].Text == str_displayDate)
                {
                    ddlist_meetingdate.SelectedIndex = i;
                    break;
                }
            }            
        }
    }

    protected void btn_set_Click(object sender, EventArgs e)
    {
        if (meetingdate_check())
        {
            //by yyan 20110818 itemW112 edit start
            string query_date = "SELECT SelectMeetingDate FROM [SetSelectMeetingDate] where userid='" + Session["ExecutiveID"].ToString() + "'";
            //by yyan 20110818 itemW112 edit end
            DataSet ds_date = helper.GetDataSet(query_date);
            int count = -1;
            string str_date = ddlist_meetingdate.Text.Trim();
            if (ds_date.Tables[0].Rows.Count > 0)
            {
                //by yyan 20110818 itemW112 edit start
                string update_date = "UPDATE [SetSelectMeetingDate] SET SelectMeetingDate = '" + str_date + "' where userid='" + Session["ExecutiveID"].ToString() + "'";
                //by yyan 20110818 itemW112 edit end
                count = helper.ExecuteNonQuery(CommandType.Text, update_date, null);
            }
            else
            {
                //by yyan 20110818 itemW112 edit start
                string insert_date = "INSERT INTO [SetSelectMeetingDate](userid,SelectMeetingDate) VALUES('" + Session["ExecutiveID"].ToString() + "','" + str_date + "')";
                //by yyan 20110818 itemW112 edit end
                count = helper.ExecuteNonQuery(CommandType.Text, insert_date, null);
            }

            if (count > 0)
            {
                label_info.ForeColor = System.Drawing.Color.Green;
                label_info.Text = "Meeting date has been selected as " + str_date + " successfully.";
            }
            else
            {
                label_info.ForeColor = System.Drawing.Color.Red;
                label_info.Text = "Failed to select date.";
            }
            bindDropDownList();
            refreshSession();
        }
    }

    protected bool meetingdate_check()
    {
        if (ddlist_meetingdate.SelectedItem.Text.ToString().Trim() == "")
        {
            label_info.ForeColor = System.Drawing.Color.Red;
            label_info.Text = "Please select meeting date!";
            return false;
        }
        return true;
    }

    protected void btn_unset_Click(object sender, EventArgs e)
    {
        int count = -1;
        //by yyan 20110818 itemW112 edit start
        string update_date = "UPDATE [SetSelectMeetingDate] SET SelectMeetingDate = null where userid='" + Session["ExecutiveID"].ToString() + "'";
        //by yyan 20110818 itemW112 edit end
        count = helper.ExecuteNonQuery(CommandType.Text, update_date, null);
        if (count > 0)
        {
            label_info.ForeColor = System.Drawing.Color.Green;
            label_info.Text = "Meeting date has not been selected. All data will be included.";
        }
        else
        {
            label_info.ForeColor = System.Drawing.Color.Red;
            label_info.Text = "Failed to unselect meeitng date";
        }

        bindDropDownList();
        refreshSession();
    }

}
