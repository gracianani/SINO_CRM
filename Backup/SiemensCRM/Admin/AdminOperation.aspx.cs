/*
 * File Name    : AdminOperation.aspx.cs
 * 
 * Description  : Manage operation information and relation of operation and segment
 * 
 * Author       : Wangjun
 * 
 * Modify Date  : 2010-12-29
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

public partial class AdminOperation : System.Web.UI.Page
{
    SQLHelper helper = new SQLHelper();
    LogUtility log = new LogUtility();
    DisplayInfo info = new DisplayInfo();
    WebUtility webU = new WebUtility();
    SQLStatement sql = new SQLStatement();

    protected void Page_Load(object sender, EventArgs e)
    {
        Page.MaintainScrollPositionOnPostBack = true;
        if (getRoleID(getRole()) == "0")
        {
            panel_readonly.Visible = true;
        }
        else if (getRoleID(getRole()) == "5")
        {
            panel_readonly.Visible = false;
        }
        else
        {
            Response.Redirect("~/AccessDenied.aspx");
        }

        if (!IsPostBack)
        {
            panel_add.Visible = false;
            pnl_addsegment.Visible = false;
            pnl_updatecurrency.Visible = false;
            clearOperation();
            getsearchIN();
            list.bindFind(list.getOperationName(), ddlist_find);
            bindDataSource();
        }
    }

    private string getRole()
    {
        return Session["Role"].ToString().Trim();
    }

    private void getsearchIN()
    {
        ddlist_in.Width = 100;
        ddlist_in.Items.Add(new ListItem("Operation", "0"));
        ddlist_in.Items.Add(new ListItem("Abbr", "1"));
        ddlist_in.Items.Add(new ListItem("Allocation", "2"));
        ddlist_in.Items.Add(new ListItem("Segment", "3"));
        ddlist_in.Items.Add(new ListItem("Currency", "4"));
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

    protected void clearOperation()
    {
        label_add.Text = "";
        label_edt_del.Text = "";

        tbox_OperationName.Text = "";
        tbox_operationAbbr.Text = "";
        tbox_operationAbbrL.Text = "";
    }

    protected void getSegmentInfo()
    {
        DataSet ds_segment = sql.getSegmentInfo();
        if (ds_segment.Tables[0].Rows.Count > 0)
        {
            DataTable dt_segment = ds_segment.Tables[0];
            int countsegment = dt_segment.Rows.Count;
            int indexsegment = 0;
            while (indexsegment < countsegment)
            {
                ListItem ll = new ListItem(dt_segment.Rows[indexsegment][1].ToString().Trim(), dt_segment.Rows[indexsegment][0].ToString().Trim());
                ddlist_Segment.Items.Add(ll);
                indexsegment++;
            }
            ddlist_Segment.Enabled = true;
        }
        else
        {
            ddlist_Segment.Items.Add(new ListItem("", "-1"));
            ddlist_Segment.Enabled = false;
        }
    }

    protected string getOperationID(string operationName)
    {
        string query_operation = "SELECT ID FROM [Operation] WHERE Name = '" + operationName + "' AND Deleted = 0";
        DataSet ds_operationID = helper.GetDataSet(query_operation);

        if (ds_operationID.Tables.Count > 0)
        {
            return ds_operationID.Tables[0].Rows[0][0].ToString().Trim();
        }
        else
        {
            Response.Redirect(info.errorInfo(query_operation, false));
            return null;
        }
    }

    protected void bindDataSource(DataSet ds_operation)
    {
        bool notNullFlag = true;
        if (ds_operation.Tables[0].Rows.Count == 0)
        {
            notNullFlag = false;
            sql.getNullDataSet(ds_operation);
        }
        //By Wsy 20110512 ITEM 18 DEL Start 
        //gv_Operation.Width = Unit.Pixel(800);
        //By Wsy 20110512 ITEM 18 DEL End  

        //By Wsy 20110512 ITEM 18 ADD Start 
        gv_Operation.Width = Unit.Pixel(750);
        //By Wsy 20110512 ITEM 18 ADD End 
        gv_Operation.AutoGenerateColumns = false;
        //By Wsy 20110512 ITEM 18 DEL Start 
        //gv_Operation.AllowPaging = true;
        //By Wsy 20110512 ITEM 18 DEL End 

        //By Wsy 20110512 ITEM 18 ADD Start 
        gv_Operation.AllowPaging = false;
        //By Wsy 20110512 ITEM 18 ADD End 
        gv_Operation.Visible = true;

        for (int i = 0; i < ds_operation.Tables[0].Columns.Count; i++)
        {
            BoundField bf = new BoundField();

            bf.DataField = ds_operation.Tables[0].Columns[i].ColumnName.ToString();
            bf.HeaderText = ds_operation.Tables[0].Columns[i].Caption.ToString();
            bf.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
            bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;

            if (i == 0 || i == 1 || i == 5 || i == 6)
            {
                bf.ReadOnly = true;
            }
            else if (i == 2)
            {
                bf.ItemStyle.Width = 350;
                bf.ControlStyle.Width = bf.ItemStyle.Width;
            }
            else
            {
                bf.ItemStyle.Width = 60;
                bf.ControlStyle.Width = bf.ItemStyle.Width;
            }

            gv_Operation.Columns.Add(bf);
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
        gv_Operation.Columns.Add(cf_Update);

        CommandField cf_Delete = new CommandField();
        cf_Delete.ButtonType = ButtonType.Image;
        cf_Delete.ShowDeleteButton = true;
        cf_Delete.ShowCancelButton = true;
        cf_Delete.CausesValidation = false;
        cf_Delete.DeleteImageUrl = "~/images/del.jpg";
        cf_Delete.DeleteText = "Delete";
        gv_Operation.Columns.Add(cf_Delete);

        gv_Operation.AllowSorting = true;
        gv_Operation.DataSource = ds_operation.Tables[0];
        gv_Operation.DataBind();

        gv_Operation.Columns[gv_Operation.Columns.Count - 1].Visible = notNullFlag;
        gv_Operation.Columns[gv_Operation.Columns.Count - 2].Visible = notNullFlag;
        gv_Operation.Columns[0].Visible = false;
        gv_Operation.Columns[1].Visible = false;
        if (getRoleID(getRole()) != "0")
        {
            gv_Operation.Columns[gv_Operation.Columns.Count - 1].Visible = false;
            gv_Operation.Columns[gv_Operation.Columns.Count - 2].Visible = false;
        }

        lbtn_AddOperation.Visible = true;
    }

    protected void bindDataSource()
    {
        gv_Operation.Columns.Clear();
        string str = ddlist_find.Text.Trim();
        int sel = int.Parse(ddlist_in.SelectedItem.Value.Trim());
        DataSet ds;

        ds = sql.getOperationInfo(str, sel);
        bindDataSource(ds);
    }

    protected void gv_Operation_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        gv_Operation.Columns.Clear();
        gv_Operation.PageIndex = e.NewPageIndex;
        bindDataSource();
    }

    protected void gv_Operation_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        //by yyan item W32 20110609 add start 
        lbtn_AddOperation.Enabled = true;
        lbtn_addsegment.Enabled = true;
        lbtn_updatecurrency.Enabled = true;
        //by yyan item W32 20110609 add end 
        gv_Operation.Columns.Clear();
        gv_Operation.EditIndex = -1;
        bindDataSource();
    }

    protected void gv_Operation_RowEditing(object sender, GridViewEditEventArgs e)
    {
        //by yyan item W32 20110609 add start 
        lbtn_AddOperation.Enabled=false;
        lbtn_AddOperation.Text = "Add operation";
        panel_add.Visible = false;
        lbtn_addsegment.Enabled=false;
        lbtn_addsegment.Text = "Add segment to operation";
        pnl_addsegment.Visible = false;
        lbtn_updatecurrency.Enabled = false;
        lbtn_updatecurrency.Text = "Modify currency";
        pnl_updatecurrency.Visible = false;
        //by yyan item W32 20110609 add end 
        gv_Operation.Columns.Clear();
        gv_Operation.EditIndex = e.NewEditIndex;
        bindDataSource();
        clearOperation();
    }

    protected void gv_Operation_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        label_edt_del.ForeColor = System.Drawing.Color.Red;

        string str_abbrL = ((TextBox)(gv_Operation.Rows[e.RowIndex].Cells[3].Controls[0])).Text.Trim();
        string str_abbr = ((TextBox)(gv_Operation.Rows[e.RowIndex].Cells[4].Controls[0])).Text.Trim();
        string str_name = ((TextBox)(gv_Operation.Rows[e.RowIndex].Cells[2].Controls[0])).Text.Trim();
        string str_ID = gv_Operation.Rows[e.RowIndex].Cells[0].Text.ToString().Trim();

        if (str_abbrL.Length <= 10)
        {
            bool success = sql.updateOperation(str_abbrL, str_abbr, str_name, str_ID);

            if (success)
            {
                label_edt_del.ForeColor = System.Drawing.Color.Green;
                label_edt_del.Text = info.edtLabelInfo(str_name, true);
            }
            else
                label_edt_del.Text = info.edtLabelInfo(str_name, false);
        }
        else
            label_edt_del.Text = "Abbr.Length:" + str_abbrL.Length + ")," + info.addillegal();

        gv_Operation.Columns.Clear();
        gv_Operation.EditIndex = -1;
        bindDataSource();
        //by yyan item W32 20110609 add start 
        lbtn_AddOperation.Enabled = true;
        lbtn_addsegment.Enabled = true;
        lbtn_updatecurrency.Enabled = true;
        //by yyan item W32 20110609 add end 
    }

    /// <summary>
    ///  Judging whether or not operation exist
    /// </summary>
    /// <param name="str_name">Operation Name</param>
    /// <param name="str_abbr">Operation Abbr</param>
    /// <returns>Only Operation name or abbr exist, return false</returns>
    public bool existOperation(string str_name, string str_abbrl, string str_abbr)
    {
        string query_Operation = "SELECT"
            + " [Operation].Name"
            + " FROM [Operation]"
            + " WHERE [Operation].Deleted = 0"
            + " AND [Operation].Name = '" + str_name + "' AND [Operation].AbbrL = '" + str_abbrl + "' AND Abbr = '" + str_abbr + "'";
        DataSet ds_Operation = helper.GetDataSet(query_Operation);
        if (ds_Operation.Tables[0].Rows.Count > 0)
            return true;
        else
            return false;
    }

    protected void btn_AddOperation_click(object sender, EventArgs e)
    {
        //By Wsy 20110520 ITEM 18 ADD Start 
        label_add.Text = "";
        label_edt_del.Text = "";
        //By Wsy 20110520 ITEM 18 ADD End
 
        //By Wsy 20110513 ITEM 6 DEL Start 
        //lbtn_AddOperation.Text = "Add operation and segment";
        //By Wsy 20110513 ITEM 6 DEL End 

        //By Wsy 20110513 ITEM 6 ADD Start 
        lbtn_AddOperation.Text = "Add operation";
        //By Wsy 20110513 ITEM 6 ADD End 
        lbtn_AddOperation.Enabled = true;
        panel_add.Visible = false;
        label_add.ForeColor = System.Drawing.Color.Red;

        string str_operation = tbox_OperationName.Text.Trim();
        string str_abbrl = tbox_operationAbbrL.Text.Trim();
        string str_abbr = tbox_operationAbbr.Text.Trim();
        string str_currency = ddlist_currency.SelectedItem.Value.Trim();

        if (str_operation != "")
        {
            if (!existOperation(str_operation, str_abbrl, str_abbr))
            {
                string insert_Operation = "INSERT INTO [Operation](Name, AbbrL, Abbr, CurrencyID, Deleted)"
                                        + " VALUES('" + str_operation + "','" + str_abbrl + "','" + str_abbr + "','" + str_currency + "', 0)";
                int countoperation = helper.ExecuteNonQuery(CommandType.Text, insert_Operation, null);

                if (countoperation > 0)
                {
                    label_add.ForeColor = System.Drawing.Color.Green;
                    label_add.Text = info.addLabelInfo(str_operation, true);
                }
                else
                    label_add.Text = info.addLabelInfo(str_operation, false);
            }
            else
                label_add.Text = info.addExist(str_operation);
        }
        else
            label_add.Text = info.addillegal();

        gv_Operation.Columns.Clear();
        bindDataSource();
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
                ddlist_currency2.Items.Add(ll);
                index++;
            }
            ddlist_currency.Enabled = true;
            ddlist_currency2.Enabled = true;
        }
        else
        {
            ddlist_currency.Enabled = false;
            ddlist_currency2.Enabled = false;
        }
    }

    protected void lbtn_AddOperation_Click(object sender, EventArgs e)
    {
        lbtn_AddOperation.Text = "Input Operation Information";
        lbtn_AddOperation.Enabled = false;
        panel_add.Visible = true;
        clearOperation();
        ddlist_currency.Items.Clear();
        bindCurrency();
    }

    protected void gv_Operation_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        clearOperation();

        string strOperationID = gv_Operation.Rows[e.RowIndex].Cells[0].Text.ToString().Trim();
        string strSegmentID = gv_Operation.Rows[e.RowIndex].Cells[1].Text.ToString().Trim();
        string strOperation = gv_Operation.Rows[e.RowIndex].Cells[2].Text.ToString().Trim();
        string strSegment = gv_Operation.Rows[e.RowIndex].Cells[4].Text.ToString().Trim();

        bool success = sql.delOperation(strOperationID, strSegmentID);

        if (success)
        {
            label_edt_del.ForeColor = System.Drawing.Color.Green;
            label_edt_del.Text = info.delLabelInfo(strOperation, strSegment, true);
        }
        else
        {
            label_edt_del.ForeColor = System.Drawing.Color.Red;
            label_edt_del.Text = info.delLabelInfo(strOperation, strSegment, false);
        }

        gv_Operation.Columns.Clear();
        bindDataSource();
    }

    protected void gv_Operation_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (e.Row.RowState == DataControlRowState.Normal || e.Row.RowState == DataControlRowState.Alternate)
            {
                ((ImageButton)e.Row.Cells[gv_Operation.Columns.Count - 1].Controls[0]).Attributes.Add("onclick", info.setRowDataBound(e.Row.Cells[2].Text.Trim(), e.Row.Cells[5].Text.Trim()));
            }
        }
    }

    protected void btn_CancelOperation_click(object sender, EventArgs e)
    {
        //By Wsy 20110513 ITEM 6 DEL Start 
        //lbtn_AddOperation.Text = "Add operation and segment";
        //By Wsy 20110513 ITEM 6 DEL End 

        //By Wsy 20110513 ITEM 6 ADD Start 
        lbtn_AddOperation.Text = "Add operation";
        //By Wsy 20110513 ITEM 6 ADD End 
        lbtn_AddOperation.Enabled = true;
        panel_add.Visible = false;

        clearOperation();
        gv_Operation.Columns.Clear();
        bindDataSource();
    }

    protected void btn_find_Click(object sender, EventArgs e)
    {
        //by yyan item W32 20110609 add start 
        lbtn_AddOperation.Enabled = true;
        lbtn_AddOperation.Text = "Add operation";
        panel_add.Visible = false;
        lbtn_addsegment.Enabled = true;
        lbtn_addsegment.Text = "Add segment to operation";
        pnl_addsegment.Visible = false;
        lbtn_updatecurrency.Enabled = true;
        lbtn_updatecurrency.Text = "Modify currency";
        pnl_updatecurrency.Visible = false;
        gv_Operation.EditIndex = -1;
        //by yyan item W32 20110609 add end 
        
        //By Wsy 20110520 ITEM 18 ADD Start 
        label_add.Text = "";
        label_edt_del.Text = "";
        //By Wsy 20110520 ITEM 18 ADD End 
        gv_Operation.Columns.Clear();
        string str = ddlist_find.Text.Trim();
        int sel = int.Parse(ddlist_in.SelectedItem.Value.Trim());
        DataSet ds;

        ds = sql.getOperationInfo(str, sel);
        bindDataSource(ds);
    }

    protected void lbtn_findhelp_Click(object sender, EventArgs e)
    {
        string str_args = "'AdminHelp.aspx'" + ",'Help', 'height=500,width=800,top=100,left=200,toolbar=no,status=no, menubar=no,location=no,resizable=no,scrollbars=yes'";
        Response.Write("<script   language='javascript'>window.open(" + str_args + ");</script>");
    }

    /// <summary>
    ///  Judging whether or not segment exist in operation
    /// </summary>
    /// <param name="str_name">Operation Name</param>
    /// <param name="str_segment">Operation Segment</param>
    /// <returns>if the relation of segment and operation has existed, return false</returns>
    public bool existSegment(string str_operationID, string str_segmentID)
    {
        string query_Segment = "SELECT *"
            + " FROM [Operation_Segment]"
            + " WHERE [Operation_Segment].Deleted = 0"
            + " AND OperationID = '" + str_operationID + "'"
            + " AND SegmentID = '" + str_segmentID + "'";
        DataSet ds_Segment = helper.GetDataSet(query_Segment);
        if (ds_Segment.Tables[0].Rows.Count > 0)
            return true;
        else
            return false;
    }

    protected void btn_addsegment_Click(object sender, EventArgs e)
    {
        //By Wsy 20110520 ITEM 18 ADD Start 
        label_add.Text = "";
        label_edt_del.Text = "";
        //By Wsy 20110520 ITEM 18 ADD End 
        lbtn_addsegment.Text = "Add segment to operation";
        lbtn_addsegment.Enabled = true;
        pnl_addsegment.Visible = false;
        label_add.ForeColor = System.Drawing.Color.Red;

        string str_operationID = ddlist_operation.SelectedItem.Value.Trim();
        string str_segment = ddlist_Segment.SelectedItem.Text.Trim();
        string str_segmentID = ddlist_Segment.SelectedItem.Value.Trim();

        if (!existSegment(str_operationID, str_segmentID))
        {
            string insert_segment = "INSERT INTO [Operation_Segment] VALUES(" + str_operationID + "," + str_segmentID + ",0)";
            int count = helper.ExecuteNonQuery(CommandType.Text, insert_segment, null);
            if (count == 1)
            {
                label_add.ForeColor = System.Drawing.Color.Green;
                label_add.Text = info.addLabelInfo(str_segment, true);
            }
            else
                label_add.Text = info.addLabelInfo(str_segment, false);
        }
        else
            label_add.Text = info.addExist(str_segment);

        gv_Operation.Columns.Clear();
        bindDataSource();
    }

    protected void btn_cancelsegment_Click(object sender, EventArgs e)
    {
        lbtn_addsegment.Text = "Add segment to operation";
        lbtn_addsegment.Enabled = true;
        pnl_addsegment.Visible = false;
    }

    private void bindOperation()
    {
        string sql = "SELECT ID, AbbrL FROM [Operation] WHERE Deleted = 0 ORDER BY Name ASC";
        DataSet ds = helper.GetDataSet(sql);

        if (ds.Tables[0].Rows.Count > 0)
        {
            DataTable dt = ds.Tables[0];
            int count = dt.Rows.Count;
            int index = 0;
            while (index < count)
            {
                ListItem ll = new ListItem(dt.Rows[index][1].ToString().Trim(), dt.Rows[index][0].ToString().Trim());
                ddlist_operation.Items.Add(ll);
                ddlist_operation2.Items.Add(ll);
                index++;
            }
            ddlist_operation.Enabled = true;
            ddlist_operation2.Enabled = true;
        }
        else
        {
            ddlist_operation.Enabled = false;
            ddlist_operation2.Enabled = false;
        }
    }

    protected void lbtn_addsegment_Click(object sender, EventArgs e)
    {
        lbtn_addsegment.Text = "Select operation and segment";
        lbtn_addsegment.Enabled = false;
        pnl_addsegment.Visible = true;

        clearOperation();
        ddlist_operation.Items.Clear();
        ddlist_Segment.Items.Clear();
        getSegmentInfo();
        bindOperation();
    }

    //Find
    FindList list = new FindList();

    protected void ddlist_in_SelectedIndexChanged(object sender, EventArgs e)
    {
        //By Wsy 20110520 ITEM 18 ADD Start 
        clearOperation();
        //By Wsy 20110520 ITEM 18 ADD End 
        int sel = int.Parse(ddlist_in.SelectedItem.Value.Trim());
        ddlist_find.Items.Clear();

        switch (sel)
        {
            case 0:
                {
                    list.bindFind(list.getOperationName(), ddlist_find);
                    break;
                }
            case 1:
                {
                    list.bindFind(list.getOperationAbbrL(), ddlist_find);
                    break;
                }
            case 2:
                {
                    list.bindFind(list.getOperationAbbr(), ddlist_find);
                    break;
                }
            case 3:
                {
                    list.bindFind(list.getSegmentAbbr(), ddlist_find);
                    break;
                }
            case 4:
                {
                    list.bindFind(list.getCurrencyName(), ddlist_find);
                    break;
                }
        }
    }

    protected void lbtn_updatecurrency_Click(object sender, EventArgs e)
    {
        lbtn_updatecurrency.Text = "Select operation and currency";
        lbtn_updatecurrency.Enabled = false;
        pnl_updatecurrency.Visible = true;

        clearOperation();
        ddlist_operation2.Items.Clear();
        ddlist_currency2.Items.Clear();
        ddlist_operation.Items.Clear();
        ddlist_currency.Items.Clear();
        bindCurrency();
        bindOperation();
    }

    protected void btn_addcurrency_Click(object sender, EventArgs e)
    {
        //By Wsy 20110520 ITEM 18 ADD Start 
        label_add.Text = "";
        label_edt_del.Text = "";
        //By Wsy 20110520 ITEM 18 ADD End 
        lbtn_updatecurrency.Text = "Modify currency";
        lbtn_updatecurrency.Enabled = true;
        pnl_updatecurrency.Visible = false;
        label_add.ForeColor = System.Drawing.Color.Red;

        string str_operationID = ddlist_operation2.SelectedItem.Value.Trim();
        string str_currency = ddlist_currency2.SelectedItem.Text.Trim();
        string str_currencyID = ddlist_currency2.SelectedItem.Value.Trim();

        string update_currency = " UPDATE [Operation] SET CurrencyID = '" + str_currencyID + "'"
                              + " WHERE Deleted = 0 AND ID = '" + str_operationID + "'";
        int count = helper.ExecuteNonQuery(CommandType.Text, update_currency, null);
        if (count == 1)
        {
            label_add.ForeColor = System.Drawing.Color.Green;
            label_add.Text = info.addLabelInfo(str_currency, true);
        }
        else
            label_add.Text = info.addLabelInfo(str_currency, false);

        gv_Operation.Columns.Clear();
        bindDataSource();
    }

    protected void btn_cancelcurrency_Click(object sender, EventArgs e)
    {
        lbtn_updatecurrency.Text = "Modify currency";
        lbtn_updatecurrency.Enabled = true;
        pnl_updatecurrency.Visible = false;
    }
}
