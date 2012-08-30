using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Text;

public partial class Assistant_AssistantCurrency : System.Web.UI.Page
{
    SQLHelper helper = new SQLHelper();
    LogUtility log = new LogUtility();
    DisplayInfo info = new DisplayInfo();
    WebUtility web = new WebUtility();
    SQLStatement sql = new SQLStatement();
    GetMeetingDate date = new GetMeetingDate();

    protected void Page_Load(object sender, EventArgs e)
    {
        Page.MaintainScrollPositionOnPostBack = true;
        if (getRoleID(getRole()) == "0")
            panel_readonly.Visible = true;
        else if (getRoleID(getRole()) == "5")
            panel_readonly.Visible = false;
        else
            Response.Redirect("~/AccessDenied.aspx");

        if (!IsPostBack)
        {
            panel_addcurrency.Visible = false;
            lbtn_AddCurrency.Visible = false;
            date.setDate();
            getAllMeetingDate(sender, e);
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

    protected void null_input()
    {
        tbox_rate.Text = "";
        tbox_rate2.Text = "";
        label_add.Text = "";
        label_edt_del.Text = "";
    }

    private void bindCurrency()
    {
        string sql = "SELECT ID, Name AS 'Planning Currency' FROM [Currency] WHERE Deleted = 0 ORDER BY Name ASC";
        DataSet ds = helper.GetDataSet(sql);

        if (ds.Tables[0].Rows.Count > 0)
        {
            DataTable dt = ds.Tables[0];
            int count = dt.Rows.Count;
            int index = 0;
            while (index < count)
            {
                ListItem ll = new ListItem(dt.Rows[index][1].ToString().Trim(), dt.Rows[index][0].ToString().Trim());
                ddlist_currency.Items.Add(ll);
                index++;
            }
            btn_Addcurrency.Enabled = true;
            ddlist_currency.Enabled = true;
        }
        else
        {
            ddlist_currency.Enabled = false;
            btn_Addcurrency.Enabled = false;
        }
    }

    protected void getAllMeetingDate(object sender, EventArgs e)
    {
        DataSet ds = date.getMeetingDate();
        ddlist_meetingdate.Items.Clear();
        bindDropDownList(ddlist_meetingdate, ds);
        //by yyan 20110811 itemw94 add start 
        date.setDate();
        string str_displayYear = date.getyear();
        string str_displayMonth = date.getmonth();
        if (str_displayMonth.Equals("10"))
            str_displayYear = (int.Parse(str_displayYear) + 1).ToString().Trim();
        string sessionTime = date.getMeetingName(int.Parse(str_displayMonth)) + " " + str_displayYear;
        for (int i = 0; i < ddlist_meetingdate.Items.Count; i++)
        {
            if (ddlist_meetingdate.Items[i].Text == sessionTime)
            {
                ddlist_meetingdate.SelectedValue = ddlist_meetingdate.Items[i].Value;
                btn_search_Click(sender, e);
                break;
            }
        }
        //by yyan 20110811 itemw94 add end
    }

    public void bindDropDownList(DropDownList ddl, DataSet ds)
    {
        if (ds.Tables[0].Rows.Count > 0)
        {
            DataTable dt = ds.Tables[0];
            int count = dt.Rows.Count;
            int index = 0;
            while (index < count)
            {
                string str_displayYear = dt.Rows[index][2].ToString().Trim();
                string str_displayMonth = dt.Rows[index][1].ToString().Trim();
                if (str_displayMonth.Equals("10"))
                    str_displayYear = (int.Parse(str_displayYear) + 1).ToString().Trim();
                string str_display = date.getMeetingName(int.Parse(str_displayMonth)) + " " + str_displayYear;
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

    protected bool existCurrency(string str_currency, string str_date)
    {
        string query_date = "SELECT CurrencyID FROM [Currency_Exchange]"
                            + " WHERE CurrencyID = '" + str_currency + "' AND Deleted = 0"
                            + " AND TimeFlag ='" + str_date + "'";
        DataSet ds_date = helper.GetDataSet(query_date);

        if (ds_date.Tables[0].Rows.Count > 0)
            return true;
        else
            return false;
    }

    protected DataSet getCurrencyInfo(string str_date)
    {
        string query_currency = "SELECT [Currency].ID, [Currency].Name AS Currency,Replace([Currency_Exchange].Rate1,'.',',') AS 'Current Rate*',Replace([Currency_Exchange].Rate2,'.',',') AS 'Next Rate*'"
                                + " FROM [Currency_Exchange] INNER JOIN [Currency] ON [Currency_Exchange].CurrencyID = [Currency].ID WHERE [Currency_Exchange].Deleted = 0 AND [Currency].Deleted = 0 AND [Currency_Exchange].TimeFlag = '" + str_date + "'"
                                + " ORDER BY [Currency].Name ASC";
        DataSet ds_currency = helper.GetDataSet(query_currency);
        return ds_currency;
    }

    protected void bindDataSource()
    {
        tbox_date.Text = ddlist_meetingdate.SelectedItem.Text.Trim();
        DataSet ds_currency = getCurrencyInfo(ddlist_meetingdate.SelectedItem.Value.Trim());

        if (ds_currency != null)
        {
            gv_currency.Width = Unit.Pixel(600);
            gv_currency.AutoGenerateColumns = false;
            // update by SJ 20110511 Start
            //gv_currency.AllowPaging = true;
            gv_currency.AllowPaging = false;
            // update by SJ 20110511 End
            gv_currency.Visible = true;

            for (int i = 0; i < ds_currency.Tables[0].Columns.Count; i++)
            {
                BoundField bf = new BoundField();
                bf.DataField = ds_currency.Tables[0].Columns[i].ColumnName.ToString();
                bf.HeaderText = ds_currency.Tables[0].Columns[i].Caption.ToString();
                if (ds_currency.Tables[0].Columns[i].ColumnName == "ID" || ds_currency.Tables[0].Columns[i].ColumnName == "Currency")
                    bf.ReadOnly = true;
                else
                {
                    bf.ItemStyle.Width = 120;
                    bf.ReadOnly = false;
                }
                bf.ControlStyle.Width = bf.ItemStyle.Width;
                bf.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
                bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
                gv_currency.Columns.Add(bf);
            }

            CommandField cf_Update = new CommandField();
            cf_Update.ButtonType = ButtonType.Image;
            cf_Update.ShowEditButton = true;
            cf_Update.ShowCancelButton = true;
            cf_Update.EditImageUrl = "~/images/edit.jpg";
            cf_Update.EditText = "Edit";
            cf_Update.CausesValidation = false;
            cf_Update.CancelImageUrl = "~/images/cancel.jpg";
            cf_Update.CancelText = "Cancel";
            cf_Update.UpdateImageUrl = "~/images/ok.jpg";
            cf_Update.UpdateText = "Update";
            gv_currency.Columns.Add(cf_Update);

            CommandField cf_Delete = new CommandField();
            cf_Delete.ButtonType = ButtonType.Image;
            cf_Delete.ShowDeleteButton = true;
            cf_Delete.ShowCancelButton = true;
            cf_Delete.CausesValidation = false;
            cf_Delete.DeleteImageUrl = "~/images/del.jpg";
            cf_Delete.DeleteText = "Delete";
            gv_currency.Columns.Add(cf_Delete);

            gv_currency.AllowSorting = true;
            gv_currency.DataSource = ds_currency.Tables[0];
            gv_currency.DataBind();
            gv_currency.Columns[0].Visible = false;
            if (getRoleID(getRole()) != "0")
            {
                gv_currency.Columns[gv_currency.Columns.Count - 1].Visible = false;
                gv_currency.Columns[gv_currency.Columns.Count - 2].Visible = false;
            }
        }
    }

    protected void gv_currency_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        gv_currency.Columns.Clear();
        gv_currency.PageIndex = e.NewPageIndex;
        bindDataSource();
    }

    protected void lbtn_AddCurrency_Click(object sender, EventArgs e)
    {
        lbtn_AddCurrency.Text = "Input currency";
        lbtn_AddCurrency.Enabled = false;
        panel_addcurrency.Visible = true;

        null_input();
        ddlist_currency.Items.Clear();
        bindCurrency();
    }

    protected void btn_Addcurrency_Click(object sender, EventArgs e)
    {
        lbtn_AddCurrency.Text = "Add currency";
        lbtn_AddCurrency.Enabled = true;
        panel_addcurrency.Visible = false;

        string str_currencyID = ddlist_currency.SelectedItem.Value.Trim();
        string str_currency = ddlist_currency.SelectedItem.Text.Trim();
        string str_currentrate = tbox_rate.Text.Trim();
        string str_nextrate = tbox_rate2.Text.Trim();
        string str_date = ddlist_meetingdate.SelectedItem.Value.Trim();
        label_add.ForeColor = System.Drawing.Color.Red;

        if (web.checkFloat(str_currentrate) && web.checkFloat(str_nextrate))
        {
            if (!existCurrency(str_currencyID, str_date))
            {
                string insert_currency = "INSERT INTO [Currency_Exchange](CurrencyID, Rate1, Rate2, TimeFlag, Deleted) VALUES"
                                        + "('" + str_currencyID + "','" + str_currentrate + "','" + str_nextrate + "','" + str_date + "' ,0)";
                int count = helper.ExecuteNonQuery(CommandType.Text, insert_currency, null);

                if (count > 0)
                {
                    label_add.ForeColor = System.Drawing.Color.Green;
                    label_add.Text = info.addLabelInfo(str_currency, true);
                }
                else
                    label_add.Text = info.addLabelInfo(str_currency, false);
            }
            else
                label_add.Text = info.addExist(str_currency);
        }
        else
            label_add.Text = info.addillegal("The currency rate should be float.");

        gv_currency.Columns.Clear();
        bindDataSource();
    }

    protected void gv_currency_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        null_input();
        string strID = gv_currency.Rows[e.RowIndex].Cells[0].Text.Trim();
        string str_currency = gv_currency.Rows[e.RowIndex].Cells[1].Text.Trim();
        string str_date = ddlist_meetingdate.SelectedItem.Value.Trim();

        string del_currency = "UPDATE [Currency_Exchange] SET Deleted = 1 WHERE CurrencyID = '" + strID + "'"
                            + " AND TimeFlag = '" + str_date + "'";
        int count = helper.ExecuteNonQuery(CommandType.Text, del_currency, null);

        if (count == 1)
        {
            label_edt_del.ForeColor = System.Drawing.Color.Green;
            label_edt_del.Text = info.delLabelInfo(str_currency, true);
        }
        else
        {
            label_edt_del.ForeColor = System.Drawing.Color.Red;
            label_edt_del.Text = info.delLabelInfo(str_currency, false);
        }

        gv_currency.Columns.Clear();
        bindDataSource();
    }

    protected void gv_currency_RowEditing(object sender, GridViewEditEventArgs e)
    {
        null_input();
        gv_currency.Columns.Clear();
        gv_currency.EditIndex = e.NewEditIndex;
        bindDataSource();
    }

    protected void gv_currency_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        string str_currentrate = ((TextBox)(gv_currency.Rows[e.RowIndex].Cells[2].Controls[0])).Text.ToString().Trim();
        string str_nextrate = ((TextBox)(gv_currency.Rows[e.RowIndex].Cells[3].Controls[0])).Text.ToString().Trim();
        string str_currencyID = gv_currency.Rows[e.RowIndex].Cells[0].Text.Trim();
        string str_currency = gv_currency.Rows[e.RowIndex].Cells[1].Text.Trim();
        string str_date = ddlist_meetingdate.SelectedItem.Value.Trim();
        label_edt_del.ForeColor = System.Drawing.Color.Red;

        if (web.checkFloat(str_currentrate) && web.checkFloat(str_nextrate))
        {
            string update_currency = "UPDATE [Currency_Exchange] SET "
                                    + " Rate1 = " + str_currentrate
                                    + " ,Rate2 = " + str_nextrate
                                    + " WHERE CurrencyID = '" + str_currencyID + "'"
                                    + " AND TimeFlag = '" + str_date + "'"
                                    + " AND Deleted = 0";
            int count = helper.ExecuteNonQuery(CommandType.Text, update_currency, null);

            if (count == 1)
            {
                label_edt_del.ForeColor = System.Drawing.Color.Green;
                label_edt_del.Text = info.edtLabelInfo(str_currency, true);
            }
            else
                label_edt_del.Text = info.edtLabelInfo(str_currency, false);
        }
        else
            label_edt_del.Text = info.addillegal("The currency rate should be float.");

        gv_currency.EditIndex = -1;
        gv_currency.Columns.Clear();
        bindDataSource();
    }

    protected void gv_currency_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        gv_currency.EditIndex = -1;
        gv_currency.Columns.Clear();
        bindDataSource();
    }

    protected void gv_currency_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (e.Row.RowState == DataControlRowState.Normal || e.Row.RowState == DataControlRowState.Alternate)
            {
                ((ImageButton)e.Row.Cells[gv_currency.Columns.Count - 1].Controls[0]).Attributes.Add("onclick", info.setRowDataBound(e.Row.Cells[4].Text, e.Row.Cells[1].Text));
            }
        }
    }

    protected void btn_search_Click(object sender, EventArgs e)
    {
        gv_currency.Columns.Clear();
        bindDataSource();
        lbtn_AddCurrency.Visible = true;
    }

    protected void btn_CancelCcurrency_Click(object sender, EventArgs e)
    {
        lbtn_AddCurrency.Text = "Add currency";
        lbtn_AddCurrency.Enabled = true;
        panel_addcurrency.Visible = false;
    }
}
