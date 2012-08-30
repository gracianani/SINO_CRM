/*
 * File Name    : AdminCurrencyDic.aspx.cs
 * 
 * Description  : currency data dictionary
 * 
 * Author       : Wangjun
 * 
 * Modify Date  : 2010.12.29
 * 
 * Problem      : 
 * 
 * Version      : Release (2.0)
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;

public partial class Admin_AdminCurrencyDic : System.Web.UI.Page
{
    SQLHelper helper = new SQLHelper();
    LogUtility log = new LogUtility();
    DisplayInfo info = new DisplayInfo();
    WebUtility web = new WebUtility();
    SQLStatement sql = new SQLStatement();

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
            pnl_addcurrency.Visible = false;
            bindDataSource();
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

    private DataSet getCurrencyInfo()
    {
        string sql = "SELECT ID, Name AS 'Currency', Description FROM [Currency] WHERE Deleted = 0 ORDER BY Name ASC";
        DataSet ds = helper.GetDataSet(sql);
        return ds;
    }

    private void bindDataSource()
    {
        bool notNullFlag = true;
        DataSet ds_currency = getCurrencyInfo();

        if (ds_currency.Tables[0].Rows.Count == 0)
        {
            notNullFlag = false;
            sql.getNullDataSet(ds_currency);
        }

        gv_currency.Width = Unit.Pixel(400);
        gv_currency.AutoGenerateColumns = false;
        // update by SJ 20110510 Start
        //gv_currency.AllowPaging = true;
        gv_currency.AllowPaging = false;
        // update by SJ 20110510 End
        gv_currency.Visible = true;

        for (int i = 0; i < ds_currency.Tables[0].Columns.Count; i++)
        {
            BoundField bf = new BoundField();
            bf.DataField = ds_currency.Tables[0].Columns[i].ColumnName.ToString();
            bf.HeaderText = ds_currency.Tables[0].Columns[i].Caption.ToString();
            bf.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
            bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;

            if (i == 0)
                bf.ReadOnly = true;
            else if (i == 1)
            {
                bf.ReadOnly = true;
                bf.ItemStyle.Width = 80;
            }
            else if (i == 2)
            {
                bf.ReadOnly = false;
                bf.ItemStyle.Width = 200;
            }

            bf.ControlStyle.Width = bf.ItemStyle.Width;
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
        gv_currency.Columns[gv_currency.Columns.Count - 2].Visible = notNullFlag;
        gv_currency.Columns[gv_currency.Columns.Count - 1].Visible = notNullFlag;
        if (getRoleID(getRole()) != "0")
        {
            gv_currency.Columns[gv_currency.Columns.Count - 2].Visible = false;
            gv_currency.Columns[gv_currency.Columns.Count - 1].Visible = false;
        }
    }

    private void clearText()
    {
        lbl_add.Text = "";
        lbl_del.Text = "";
        tbox_currency.Text = "";
        tbox_currencydes.Text = "";
    }

    protected void lbtn_addcurrency_Click(object sender, EventArgs e)
    {
        lbtn_addcurrency.Text = "Input Currency";
        lbtn_addcurrency.Enabled = false;
        pnl_addcurrency.Visible = true;
        clearText();
    }

    private bool existCurrency(string str_currency)
    {
        string sql = "SELECT ID FROM [Currency] WHERE Deleted = 0 AND Name = '" + str_currency + "'";
        DataSet ds = helper.GetDataSet(sql);
        if (ds.Tables[0].Rows.Count > 0)
            return true;
        else
            return false;
    }

    private void addCurrency(string str_currency, string str_currencydes)
    {
        lbl_add.ForeColor = System.Drawing.Color.Red;

        if (!existCurrency(str_currency))
        {
            string sql = "INSERT INTO [Currency](Name, Description, Deleted) VALUES('" + str_currency + "','" + str_currencydes + "',0)";
            int count = helper.ExecuteNonQuery(CommandType.Text, sql, null);
            if (count == 1)
            {
                lbl_add.ForeColor = System.Drawing.Color.Green;
                lbl_add.Text = info.addLabelInfo(str_currency, true);
            }
            else
                lbl_add.Text = info.addLabelInfo(str_currency, false);
        }
        else
            lbl_add.Text = info.addExist(str_currency);
    }

    protected void btn_add_Click(object sender, EventArgs e)
    {
        lbtn_addcurrency.Text = "Add Currency";
        lbtn_addcurrency.Enabled = true;
        pnl_addcurrency.Visible = false;

        string str_currency = tbox_currency.Text.Trim();
        string str_currencydes = tbox_currencydes.Text.Trim();
        addCurrency(str_currency, str_currencydes);

        gv_currency.Columns.Clear();
        bindDataSource();
    }

    protected void btn_cancel_Click(object sender, EventArgs e)
    {
        lbtn_addcurrency.Text = "Add Currency";
        lbtn_addcurrency.Enabled = true;
        pnl_addcurrency.Visible = false;
        clearText();
    }

    protected void gv_currency_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        gv_currency.Columns.Clear();
        gv_currency.PageIndex = e.NewPageIndex;
        bindDataSource();
    }

    protected void gv_currency_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        clearText();
        string str_ID = gv_currency.Rows[e.RowIndex].Cells[0].Text.Trim();
        string str_name = gv_currency.Rows[e.RowIndex].Cells[1].Text.Trim();

        string del_currency = "UPDATE [Currency] SET Deleted = 1 WHERE ID = '" + str_ID + "'";
        int count = helper.ExecuteNonQuery(CommandType.Text, del_currency, null);

        if (count > 0)
        {
            lbl_del.ForeColor = System.Drawing.Color.Green;
            lbl_del.Text = info.delLabelInfo(str_name, true);
        }
        else
        {
            lbl_del.ForeColor = System.Drawing.Color.Red;
            lbl_del.Text = info.delLabelInfo(str_name, false);
        }

        gv_currency.Columns.Clear();
        bindDataSource();
    }

    protected void gv_currency_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (e.Row.RowState == DataControlRowState.Normal || e.Row.RowState == DataControlRowState.Alternate)
            {
                ((ImageButton)e.Row.Cells[gv_currency.Columns.Count - 1].Controls[0]).Attributes.Add("onclick", info.setRowDataBound(e.Row.Cells[1].Text));
            }
        }
    }

    protected void gv_currency_RowEditing(object sender, GridViewEditEventArgs e)
    {
        clearText();
        gv_currency.Columns.Clear();
        gv_currency.EditIndex = e.NewEditIndex;
        bindDataSource();

        this.lbtn_addcurrency.Enabled = false;
        this.lbtn_addcurrency.Text = "Add Currency";
        this.pnl_addcurrency.Visible = false;
    }

    private void updateCurrency(string str_ID, string str_name, string str_des)
    {
        lbl_del.ForeColor = System.Drawing.Color.Red;

            string sql = "UPDATE [Currency] SET"
                       + " Description = '" + str_des + "'"
                       + " WHERE ID = '" + str_ID + "' AND Deleted = 0";
            int count = helper.ExecuteNonQuery(CommandType.Text, sql, null);
            if (count == 1)
            {
                lbl_del.ForeColor = System.Drawing.Color.Green;
                lbl_del.Text = info.addLabelInfo(str_name, true);
            }
            else
                lbl_del.Text = info.addLabelInfo(str_name, false);
    }

    protected void gv_currency_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        string str_ID = gv_currency.Rows[e.RowIndex].Cells[0].Text.Trim();
        string str_name = gv_currency.Rows[e.RowIndex].Cells[1].Text.Trim();
        string str_des = ((TextBox)(gv_currency.Rows[e.RowIndex].Cells[2].Controls[0])).Text.ToString().Trim();

        updateCurrency(str_ID, str_name, str_des);

        gv_currency.EditIndex = -1;
        gv_currency.Columns.Clear();
        bindDataSource();

        this.lbtn_addcurrency.Enabled = true;
        this.lbtn_addcurrency.Text = "Add Currency";
    }

    protected void gv_currency_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        gv_currency.EditIndex = -1;
        gv_currency.Columns.Clear();
        bindDataSource();
        this.lbtn_addcurrency.Enabled = true;
        this.lbtn_addcurrency.Text = "Add Currency";
    }
}
