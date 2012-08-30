/*
 * File Name    : AdminCustomer.aspx.cs
 * 
 * Description  : add, update and delete a customer
 * 
 * Author       : Wangjun
 * 
 * Modify Date  : 2010.12.13
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

public partial class Admin_AdminCustomer : System.Web.UI.Page
{
    SQLHelper helper = new SQLHelper();
    LogUtility log = new LogUtility();
    DisplayInfo info = new DisplayInfo();
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
            panel_addcustomer.Visible = false;
            //clearText();
            //getsearchIN();
            //list.bindFind(list.getCustomerName(), ddlist_find);
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

    

    protected bool Exist_customer(string str_nameID, string str_typeID, string str_countryID)
    {
        string query_customer = "SELECT ID FROM [Customer] WHERE  NameID = '" + str_nameID
                            + "' AND TypeID = '" + str_typeID + "' AND CountryID = '" + str_countryID + "' AND Deleted = 0 ";
        DataSet ds_customer = helper.GetDataSet(query_customer);

        if (ds_customer.Tables[0].Rows.Count > 0)
            return true;
        else
            return false;
    }

    protected void bindDataSource(DataSet ds_customer)
    {
        bool notNullFlag = true;
        if (ds_customer.Tables[0].Rows.Count == 0)
        {
            notNullFlag = false;
            sql.getNullDataSet(ds_customer);
        }

        gv_Customer.Width = Unit.Pixel(800);
        gv_Customer.AutoGenerateColumns = false;
        gv_Customer.AllowPaging = true;
        gv_Customer.Visible = true;

        for (int i = 0; i < ds_customer.Tables[0].Columns.Count; i++)
        {
            BoundField bf = new BoundField();

            bf.DataField = ds_customer.Tables[0].Columns[i].ColumnName.ToString();
            bf.HeaderText = ds_customer.Tables[0].Columns[i].Caption.ToString();
            bf.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
            bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
            if (i <= 4)
            {
                bf.ReadOnly = true;
            }
            else if (i == 5)
                bf.ItemStyle.Width = 60;
            else if (i == 6)
                bf.ItemStyle.Width = 80;
            else if (i == 7)
                bf.ItemStyle.Width = 80;
            bf.ControlStyle.Width = bf.ItemStyle.Width;
            gv_Customer.Columns.Add(bf);
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
        gv_Customer.Columns.Add(cf_Update);

        CommandField cf_Delete = new CommandField();
        cf_Delete.ButtonType = ButtonType.Image;
        cf_Delete.ShowDeleteButton = true;
        cf_Delete.ShowCancelButton = true;
        cf_Delete.CausesValidation = false;
        cf_Delete.DeleteImageUrl = "~/images/del.jpg";
        cf_Delete.DeleteText = "Delete";
        gv_Customer.Columns.Add(cf_Delete);

        gv_Customer.AllowSorting = true;
        gv_Customer.DataSource = ds_customer.Tables[0];
        gv_Customer.DataBind();

        gv_Customer.Columns[gv_Customer.Columns.Count - 2].Visible = notNullFlag;
        gv_Customer.Columns[gv_Customer.Columns.Count - 1].Visible = notNullFlag;
        gv_Customer.Columns[0].Visible = false;
        if (getRoleID(getRole()) != "0")
        {
            gv_Customer.Columns[gv_Customer.Columns.Count - 2].Visible = false;
            gv_Customer.Columns[gv_Customer.Columns.Count - 1].Visible = false;
        }
    }

    protected void bindDataSource()
    {
        gv_Customer.Columns.Clear();
        //ds = sql.getCe(ds);
        //string str = ddlist_find.Text.Trim();
        //int sel = int.Parse(ddlist_in.SelectedItem.Value.Trim());
        DataSet ds;
        //ds = sql.getCustomerInfo(str, sel);
        //bindDataSource(ds);
        //fangxuwei add 
        string sql_customer = "SELECT [Customer].ID, [CustomerName].Name AS 'Customer Name', [CustomerType].Name AS 'Customer Type', [SalesChannel].Name AS 'Sales Channel', [SubRegion].Name AS SubRegion, [Customer].City, [Customer].Address, [Customer].Department FROM [Customer] "
                                   + " INNER JOIN [SubRegion] ON [SubRegion].ID = [Customer].CountryID"
                                   + " INNER JOIN [CustomerType] ON [Customer].TypeID = [CustomerType].ID"
                                   + " INNER JOIN [CustomerName] ON [Customer].NameID = [CustomerName].ID"
                                   + " INNER JOIN [SalesChannel] ON [Customer].SalesChannelID = [SalesChannel].ID"
                                   + " WHERE [CustomerName].Name = null AND [Customer].Deleted = 0 AND [SubRegion].Deleted = 0 AND [CustomerType].Deleted = 0 AND [CustomerName].Deleted = 0"
                                   + " AND [SalesChannel].Deleted = 0";
        //fangxuwei add 
        DataSet ds_customer = helper.GetDataSet(sql_customer);
        //fangxuwei add 
        bindDataSource(ds_customer);
    }

    //protected void gv_Customer_PageIndexChanging(object sender, GridViewPageEventArgs e)
    //{
    //    gv_Customer.Columns.Clear();
    //    gv_Customer.PageIndex = e.NewPageIndex;
    //    bindDataSource();
    //}

    private void bindDrop(DataSet ds, DropDownList ddlist)
    {
        if (ds.Tables[0].Rows.Count > 0)
        {
            DataTable dt = ds.Tables[0];
            int count = dt.Rows.Count;
            int index = 0;
            while (index < count)
            {
                ListItem ll = new ListItem(dt.Rows[index][1].ToString().Trim(), dt.Rows[index][0].ToString().Trim());
                ddlist.Items.Add(ll);
                index++;
            }
            ddlist.Enabled = true;
        }
    }

    //
    protected void gv_Customer_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        string del_Info;
        string del_ID;

        //clearText();
        del_ID = gv_Customer.Rows[e.RowIndex].Cells[0].Text.ToString().Trim();
        del_Info = gv_Customer.Rows[e.RowIndex].Cells[1].Text.ToString().Trim();

        string del_customer = "UPDATE [Customer] SET Deleted = 1 "
                               + " WHERE ID = " + del_ID;
        int delcount = helper.ExecuteNonQuery(CommandType.Text, del_customer, null);

        if (delcount > 0)
        {
            label_del.ForeColor = System.Drawing.Color.Green;
            label_del.Text = info.delLabelInfo(del_Info, true);
        }
        else
        {
            label_del.ForeColor = System.Drawing.Color.Red;
            label_del.Text = info.delLabelInfo(del_Info, false);
        }

        bindDataSource();
    }

    protected void gv_Customer_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (e.Row.RowState == DataControlRowState.Normal || e.Row.RowState == DataControlRowState.Alternate)
            {
                ((ImageButton)e.Row.Cells[gv_Customer.Columns.Count - 1].Controls[0]).Attributes.Add("onclick", info.setRowDataBound(e.Row.Cells[1].Text));
            }
        }
    }

   

    protected void gv_Customer_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        gv_Customer.EditIndex = -1;
        bindDataSource();
    }

    

    protected void gv_Customer_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        string str_ID = gv_Customer.Rows[e.RowIndex].Cells[0].Text.Trim();
        string str_name = gv_Customer.Rows[e.RowIndex].Cells[1].Text.Trim();
        string str_city = ((TextBox)(gv_Customer.Rows[e.RowIndex].Cells[5].Controls[0])).Text.Trim();
        string str_address = ((TextBox)(gv_Customer.Rows[e.RowIndex].Cells[6].Controls[0])).Text.Trim();
        string str_department = ((TextBox)(gv_Customer.Rows[e.RowIndex].Cells[7].Controls[0])).Text.Trim();

        updateCustomer(str_ID, str_name, str_city, str_address, str_department);

        gv_Customer.EditIndex = -1;
        bindDataSource();
    }

    /// <summary>
    /// update the basic information of one customer, but the relation to customer can not be updated
    /// </summary>
    /// <param name="del_ID">Customer ID</param>
    /// <param name="del_Info"></param>
    /// <param name="str_type"></param>
    private void updateCustomer(string str_ID, string str_name, string str_city, string str_address, string str_department)
    {
        string sql_update = "UPDATE [Customer] SET City = '" + str_city + "'"
                            + ", Address = '" + str_address + "'"
                            + ", Department = '" + str_department + "'"
                            + " WHERE ID = " + str_ID + " AND Deleted = 0";
        int count = helper.ExecuteNonQuery(CommandType.Text, sql_update, null);

        if (count == 1)
        {
            label_del.ForeColor = System.Drawing.Color.Green;
            label_del.Text = info.edtLabelInfo(str_name, true);
        }
        else
        {
            label_del.ForeColor = System.Drawing.Color.Red;
            label_del.Text = info.edtLabelInfo(str_name, false);
        }
    }

    

    protected DataSet getchannelInfo()
    {
        string sql_channel = "SELECT ID, Name FROM [SalesChannel] WHERE Deleted = 0 ORDER BY Name ASC";
        DataSet ds_channel = helper.GetDataSet(sql_channel);
        return ds_channel;
    }

    //protected void lbtn_customer_Click(object sender, EventArgs e)
    //{
    //    lbtn_addcustomer.Text = "Select customer name, type and country";
    //    lbtn_addcustomer.Enabled = false;
    //    panel_addcustomer.Visible = true;
    //    clearText();

    //    ddlist_name.Items.Clear();
    //    ddlist_type.Items.Clear();
    //    ddlist_country.Items.Clear();
    //    ddlist_saleschannel.Items.Clear();

    //    bindDrop(sql.getCustomerName(), ddlist_name);
    //    bindDrop(sql.getCustomerType(), ddlist_type);
    //    bindDrop(sql.getSubRegionInfo(), ddlist_country);
    //    bindDrop(getchannelInfo(), ddlist_saleschannel);
    //}

    //protected void btn_add_Click(object sender, EventArgs e)
    //{
    //    lbtn_addcustomer.Text = "Add Customer";
    //    lbtn_addcustomer.Enabled = true;
    //    panel_addcustomer.Visible = false;

    //    label_add.ForeColor = System.Drawing.Color.Red;
    //    string str_nameID = ddlist_name.SelectedItem.Value.Trim();
    //    string str_typeID = ddlist_type.SelectedItem.Value.Trim();
    //    string str_saleschannelID = ddlist_saleschannel.SelectedItem.Value.Trim();
    //    string str_countryID = ddlist_country.SelectedItem.Value.Trim();
    //    string str_address = tbox_address.Text.Trim();
    //    string str_city = tbox_city.Text.Trim();
    //    string str_department = tbox_department.Text.Trim();

    //    if (!Exist_customer(str_nameID, str_typeID, str_countryID))
    //    {
    //        string add_country = "INSERT INTO [Customer](NameID, TypeID, SalesChannelID, CountryID, City, Address, Department, Deleted)"
    //                    + " VALUES('" + str_nameID + "','" + str_typeID + "','" + str_saleschannelID + "','" + str_countryID + "','" + str_city + "','" + str_address + "','" + str_department + "','0')";
    //        int add_count = helper.ExecuteNonQuery(CommandType.Text, add_country, null);

    //        if (add_count == 1)
    //        {
    //            label_add.ForeColor = System.Drawing.Color.Green;
    //            label_add.Text = info.addLabelInfo(ddlist_name.SelectedItem.Text.Trim(), true);
    //        }
    //        else
    //            label_add.Text = info.addLabelInfo(ddlist_name.SelectedItem.Text.Trim(), false);
    //    }
    //    else
    //        label_add.Text = info.addExist(ddlist_name.SelectedItem.Text.Trim());

    //    bindDataSource();
    //}

    //protected void btn_cancel_Click(object sender, EventArgs e)
    //{
    //    lbtn_addcustomer.Text = "Add Customer";
    //    lbtn_addcustomer.Enabled = true;
    //    panel_addcustomer.Visible = false;
    //}

    //Find
    FindList list = new FindList();

    //protected void ddlist_in_SelectedIndexChanged(object sender, EventArgs e)
    //{
    //    int sel = int.Parse(ddlist_in.SelectedItem.Value.Trim());
    //    ddlist_find.Items.Clear();
    //    switch (sel)
    //    {
    //        case 0:
    //            {
    //                list.bindFind(list.getCustomerName(), ddlist_find);
    //                break;
    //            }
    //        case 1:
    //            {
    //                list.bindFind(list.getCustomerType(), ddlist_find);
    //                break;
    //            }
    //        case 2:
    //            {
    //                list.bindFind(list.getCountryISO_Code(), ddlist_find);
    //                break;
    //            }
    //        case 3:
    //            {
    //                list.bindFind(list.getSalesChannel(), ddlist_find);
    //                break;
    //            }
    //    }
    //}
}
