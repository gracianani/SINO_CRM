/*
 * File Name    : AdminCustomerRelation.aspx.cs
 * 
 * Description  : add and delete a customer type and a customer name
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

public partial class Admin_AdminCustomerRelation : System.Web.UI.Page
{
    SQLHelper helper = new SQLHelper();
    LogUtility log = new LogUtility();
    DisplayInfo info = new DisplayInfo();
    SQLStatement sql = new SQLStatement();

    protected void Page_Load(object sender, EventArgs e)
    {
        Page.MaintainScrollPositionOnPostBack = true;
        if (getRoleID(getRole()) == "0")
            pnl_readonly.Visible = true;
        else if (getRoleID(getRole()) == "5")
            pnl_readonly.Visible = false;
        else
            Response.Redirect("~/AccessDenied.aspx");

        if (!IsPostBack)
        {
            pnl_addtype.Visible = false;
            pnl_addname.Visible = false;
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

    private void clearText()
    {
        lbl_addtype.Text = "";
        lbl_deltype.Text = "";
        tbox_type.Text = "";

        lbl_addname.Text = "";
        lbl_delname.Text = "";
        tbox_name.Text = "";
    }

    private void bindDataSource(GridView gv, DataSet ds)
    {
        bool lastVisible = true;
        if (ds.Tables[0].Rows.Count == 0)
        {
            sql.getNullDataSet(ds);
            lastVisible = false;
        }
        gv.Width = Unit.Pixel(350);
        gv.AutoGenerateColumns = false;
        // by dxs 20110511 del start
        //gv.AllowPaging = true
        // by dxs 20110511 del end
        // by dxs 20110513 add start
        gv.AllowPaging = false;
        // by dxs 20110513 add end
        gv.Visible = true;

        for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
        {
            BoundField bf = new BoundField();

            bf.DataField = ds.Tables[0].Columns[i].ColumnName.ToString();
            bf.HeaderText = ds.Tables[0].Columns[i].Caption.ToString();
            bf.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
            bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
            gv.Columns.Add(bf);
        }

        CommandField cf_Delete = new CommandField();
        cf_Delete.ButtonType = ButtonType.Image;
        cf_Delete.ShowDeleteButton = true;
        cf_Delete.ShowCancelButton = true;
        cf_Delete.CausesValidation = false;
        cf_Delete.DeleteImageUrl = "~/images/del.jpg";
        cf_Delete.DeleteText = "Delete";
        gv.Columns.Add(cf_Delete);

        gv.AllowSorting = true;
        gv.DataSource = ds.Tables[0];
        gv.DataBind();
        gv.Columns[0].Visible = false;
        gv.Columns[gv.Columns.Count - 1].Visible = lastVisible;
        if (getRoleID(getRole()) != "0")
            gv.Columns[gv.Columns.Count - 1].Visible = false;
    }

    private void bindDataSource()
    {
        DataSet ds_type = sql.getCustomerType();
        DataSet ds_name = sql.getCustomerName();

        gv_type.Columns.Clear();
        gv_name.Columns.Clear();

        bindDataSource(gv_type, ds_type);
        bindDataSource(gv_name, ds_name);
    }

    protected void lbtn_addtype_Click(object sender, EventArgs e)
    {
        lbtn_addtype.Text = "Input Customer Type";
        lbtn_addtype.Enabled = false;
        pnl_addtype.Visible = true;
        clearText();
        this.lbtn_addname.Enabled = false;
        this.pnl_addname.Visible = false;
    }

    private bool existType(string str_type)
    {
        string sql_type = "SELECT ID, Name AS 'Customer Type' FROM [CustomerType] WHERE Name = '" + str_type + "' AND Deleted = 0 ORDER BY Name ASC";
        DataSet ds_type = helper.GetDataSet(sql_type);
        if (ds_type.Tables[0].Rows.Count > 0)
            return true;
        else
            return false;
    }

    private void addType(string str_type)
    {
        //by dxs 20110511 add start
        clearText();
        //by dxs 20110511 add end
        lbl_addtype.ForeColor = System.Drawing.Color.Red;
        if (str_type.Trim().Length > 0 || str_type.Trim().Length <= 50)
        {
            if (!existType(str_type))
            {
                bool success = sql.insertCustomerType(str_type);
                if (success)
                {
                    lbl_addtype.ForeColor = System.Drawing.Color.Green;
                    lbl_addtype.Text = info.addLabelInfo(str_type, true);
                
                }
                else
                    lbl_addtype.Text = info.addLabelInfo(str_type, false);
            }
            else
                lbl_addtype.Text = info.addExist(str_type);
        }
        else
            lbl_addtype.Text = info.addillegal(str_type + ", length is illegal.");
            
    }

    protected void btn_addtype_Click(object sender, EventArgs e)
    {
        lbtn_addtype.Text = "Add Customer Type";
        lbtn_addtype.Enabled = true;
        pnl_addtype.Visible = false;

        string str_type = tbox_type.Text.Trim();
        addType(str_type);

        bindDataSource();
    }

    protected void btn_canceltype_Click(object sender, EventArgs e)
    {
        lbtn_addtype.Text = "Add Customer Type";
        lbtn_addtype.Enabled = true;
        pnl_addtype.Visible = false;
        this.lbtn_addname.Enabled = true;
    }

    protected void lbtn_addname_Click(object sender, EventArgs e)
    {
        lbtn_addname.Text = "Input Customer Name";
        lbtn_addname.Enabled = false;
        pnl_addname.Visible = true;
        clearText();
        this.lbtn_addtype.Enabled = false;
        this.pnl_addtype.Visible = false;
    }

    private bool existName(string str_name)
    {
        string sql_name = "SELECT ID, Name AS 'Customer Type' FROM [CustomerName] WHERE Name = '" + str_name + "' AND Deleted = 0 ORDER BY Name ASC";
        DataSet ds_name = helper.GetDataSet(sql_name);
        if (ds_name.Tables[0].Rows.Count > 0)
            return true;
        else
            return false;
    }

    private void addName(string str_name)
    {
        //by dxs 20110511 add start
        clearText();
        //by dxs 20110511 add end
        lbl_addname.ForeColor = System.Drawing.Color.Red;
        if (str_name.Trim().Length > 0 || str_name.Trim().Length <= 200)
        {
            if (!existName(str_name))
            {
                bool success = sql.insertCustomerName(str_name);
                if (success)
                {
                    lbl_addname.ForeColor = System.Drawing.Color.Green;
                    lbl_addname.Text = info.addLabelInfo(str_name, true);
                 
                }
                else
                    lbl_addname.Text = info.addLabelInfo(str_name, false);
            }
            else
                lbl_addname.Text = info.addExist(str_name);
        }
        else
            lbl_addname.Text = info.addillegal(str_name + ", length is illegal.");
            
    }

    protected void btn_addname_Click(object sender, EventArgs e)
    {
        lbtn_addname.Text = "Add Customer Name";
        lbtn_addname.Enabled = true;
        pnl_addname.Visible = false;

        string str_name = tbox_name.Text.Trim();
        addName(str_name);

        bindDataSource();
    }

    protected void btn_cancelname_Click(object sender, EventArgs e)
    {
        lbtn_addname.Text = "Add Customer Name";
        lbtn_addname.Enabled = true;
        pnl_addname.Visible = false;
        this.lbtn_addtype.Enabled = true;
    }

    protected void gv_type_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        gv_type.PageIndex = e.NewPageIndex;
        bindDataSource();
    }

    private void deleteType(string str_typeID, string str_type)
    {
        string del_type = "UPDATE [CustomerType] SET Deleted = 1 WHERE ID = " + str_typeID;
        int count = helper.ExecuteNonQuery(CommandType.Text, del_type, null);
        //by dxs 20110511 add start
        clearText();
        //by dxs 20110511 add end
        if (count == 1)
        {
            lbl_deltype.ForeColor = System.Drawing.Color.Green;
            lbl_deltype.Text = info.delLabelInfo(str_type, true);
          
        }
        else
        {
            lbl_deltype.ForeColor = System.Drawing.Color.Red;
            lbl_deltype.Text = info.delLabelInfo(str_type, false);
        }
    }

    protected void gv_type_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        string str_typeID = gv_type.Rows[e.RowIndex].Cells[0].Text.Trim();
        string str_type = gv_type.Rows[e.RowIndex].Cells[1].Text.Trim();

        deleteType(str_typeID, str_type);

        bindDataSource();
    }

    protected void gv_type_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (e.Row.RowState == DataControlRowState.Normal || e.Row.RowState == DataControlRowState.Alternate)
            {
                ((ImageButton)e.Row.Cells[gv_type.Columns.Count - 1].Controls[0]).Attributes.Add("onclick", info.setRowDataBound(e.Row.Cells[1].Text));
            }
        }
    }

    protected void gv_name_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        gv_name.PageIndex = e.NewPageIndex;
        bindDataSource();
    }

    private void deleteName(string str_nameID, string str_name)
    {
        string del_name = "UPDATE [CustomerName] SET Deleted = 1 WHERE ID = " + str_nameID;
        int count = helper.ExecuteNonQuery(CommandType.Text, del_name, null);
        //by dxs 20110511 add start
        clearText();
        //by dxs 20110511 add end
        if (count == 1)
        {
            lbl_delname.ForeColor = System.Drawing.Color.Green;
            lbl_delname.Text = info.delLabelInfo(str_name, true);
        
        }
        else
        {
            lbl_delname.ForeColor = System.Drawing.Color.Red;
            lbl_delname.Text = info.delLabelInfo(str_name, false);
        }
    }

    protected void gv_name_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        string str_nameID = gv_name.Rows[e.RowIndex].Cells[0].Text.Trim();
        string str_name = gv_name.Rows[e.RowIndex].Cells[1].Text.Trim();

        deleteName(str_nameID, str_name);

        bindDataSource();
    }

    protected void gv_name_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (e.Row.RowState == DataControlRowState.Normal || e.Row.RowState == DataControlRowState.Alternate)
            {
                ((ImageButton)e.Row.Cells[gv_name.Columns.Count - 1].Controls[0]).Attributes.Add("onclick", info.setRowDataBound(e.Row.Cells[1].Text));
            }
        }
    }
}
