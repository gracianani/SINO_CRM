using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;

public partial class MarketingMgr_MarketingMgrSalesChannel : System.Web.UI.Page
{
    SQLHelper helper = new SQLHelper();
    LogUtility log = new LogUtility();
    DisplayInfo info = new DisplayInfo();
    WebUtility web = new WebUtility();
    SQLStatement sql = new SQLStatement();

    protected void Page_Load(object sender, EventArgs e)
    {
        Page.MaintainScrollPositionOnPostBack = true;
        if (getRoleID(getRole()) == "2")
        {
            panel_readonly.Visible = false;
        }
        else
        {
            Response.Redirect("~/AccessDenied.aspx");
        }
        if (!IsPostBack)
        {
            log.WriteLog(LogUtility.LogErrorLevel.LOG_ACCESS, "MarketingMgrSalesChannel Access.");
            panel_addchannel.Visible = false;
            null_input();
            getsearchIN();
            list.bindFind(list.getSalesChannel(), ddlist_find);
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

    protected void null_input()
    {
        label_add.Text = "";
        label_del.Text = "";

        tbox_name.Text = "";
    }

    protected void getsearchIN()
    {
        ddlist_in.Items.Add(new ListItem("Sales Channel", "0"));
    }

    protected void btn_find_Click(object sender, EventArgs e)
    {
        gv_channel.Columns.Clear();
        string str = ddlist_find.Text.Trim();
        int sel = int.Parse(ddlist_in.SelectedItem.Value.Trim());
        DataSet ds;
        ds = getchannelInfo(str, sel);
        bindDataSource(ds);
    }

    protected DataSet getchannelInfo(string str, int sel)
    {
        string sql_channel = "SELECT ID, Name AS 'Sales Channel'"
                           + " FROM [SalesChannel] WHERE Deleted = 0 ";
        if (str != "" && sel != -1)
        {
            if (sel == 0)
                sql_channel += " AND Name like '%" + str + "%'";
        }
        sql_channel += " ORDER BY Name ASC";
        DataSet ds_channel = helper.GetDataSet(sql_channel);
        return ds_channel;
    }

    protected void bindDataSource(DataSet ds_channel)
    {
        bool notNullFlag = true;
        if (ds_channel.Tables[0].Rows.Count == 0)
        {
            notNullFlag = false;
            sql.getNullDataSet(ds_channel);
        }
        gv_channel.Width = Unit.Pixel(400);
        gv_channel.AutoGenerateColumns = false;
        //By Mbq 20110504 ITEM 1 DEL Start 
        //gv_channel.AllowPaging = true;
        //By Mbq 20110504 ITEM 1 DEL End 

        //By Mbq 20110504 ITEM 1 ADD Start 
        gv_channel.AllowPaging = false;
        //By Mbq 20110504 ITEM 1 ADD End 
        gv_channel.Visible = true;

        for (int i = 0; i < ds_channel.Tables[0].Columns.Count; i++)
        {
            BoundField bf = new BoundField();

            bf.DataField = ds_channel.Tables[0].Columns[i].ColumnName.ToString();
            bf.HeaderText = ds_channel.Tables[0].Columns[i].Caption.ToString();
            bf.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
            bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
            bf.ReadOnly = true;
            //By Wsy 20110512 ITEM 18 ADD Start 
            bf.ItemStyle.Width = 500;
            //By Wsy 20110512 ITEM 18 ADD End 
            bf.ControlStyle.Width = bf.ItemStyle.Width;
            gv_channel.Columns.Add(bf);
        }

        CommandField cf_Delete = new CommandField();
        cf_Delete.ButtonType = ButtonType.Image;
        cf_Delete.ShowDeleteButton = true;
        cf_Delete.ShowCancelButton = true;
        cf_Delete.CausesValidation = false;
        cf_Delete.DeleteImageUrl = "~/images/del.jpg";
        cf_Delete.DeleteText = "Delete";
        gv_channel.Columns.Add(cf_Delete);

        gv_channel.AllowSorting = true;
        gv_channel.DataSource = ds_channel.Tables[0];
        gv_channel.DataBind();
        gv_channel.Columns[0].Visible = false;
        gv_channel.Columns[gv_channel.Columns.Count - 1].Visible = notNullFlag;
        if (getRoleID(getRole()) != "0")
        {
            gv_channel.Columns[gv_channel.Columns.Count - 1].Visible = false;
        }
        lbtn_channel.Visible = true;
    }

    protected void bindDataSource()
    {
        gv_channel.Columns.Clear();
        string str = ddlist_find.Text.Trim();
        int sel = int.Parse(ddlist_in.SelectedItem.Value.Trim());
        DataSet ds;
        ds = getchannelInfo(str, sel);
        bindDataSource(ds);
    }

    protected void lbtn_findhelp_Click(object sender, EventArgs e)
    {
        string str_args = "'MarketingMgrHelp.aspx'" + ",'Help', 'height=500,width=800,top=100,left=200,toolbar=no,status=no, menubar=no,location=no,resizable=no,scrollbars=yes'";
        Response.Write("<script   language='javascript'>window.open(" + str_args + ");</script>");
    }

    protected void lbtn_channel_Click(object sender, EventArgs e)
    {
        lbtn_channel.Text = "Input sales channel";
        lbtn_channel.Enabled = false;
        panel_addchannel.Visible = true;
        null_input();
    }

    protected bool existchannel(string str_name)
    {
        string sql_channel = "SELECT ID"
                           + " FROM [SalesChannel]"
                           + " WHERE Deleted = 0 AND Name = '" + str_name + "'";
        DataSet ds_channel = helper.GetDataSet(sql_channel);

        if (ds_channel.Tables[0].Rows.Count > 0)
            return true;
        else
            return false;
    }

    /// <summary>
    /// add one sales channel.
    /// </summary>
    /// <param name="str_ownbusiness">RC own business</param>
    /// <param name="str_internal">Internal business(HS,PS)</param>
    /// <param name="str_direct">Direct(Headquarter)</param>
    /// <param name="str_agent">Agent/Distributor</param>
    /// <param name="str_commission">RC Commission</param>
    private void addchannel(string str_name)
    {
        label_add.ForeColor = System.Drawing.Color.Red;
        if (existchannel(str_name))
        {
            label_add.Text = info.addExist(str_name);
            return;
        }

        string sql = "INSERT INTO [SalesChannel](Name,Deleted)"
                   + " VALUES ('" + str_name + "','0')";
        int count = helper.ExecuteNonQuery(CommandType.Text, sql, null);
        if (count == 1)
        {
            label_add.ForeColor = System.Drawing.Color.Green;
            label_add.Text = info.addLabelInfo(str_name, true);
        }
        else
        {
            label_add.Text = info.addLabelInfo(str_name, false);
        }
    }

    protected void btn_cancelchannel_Click(object sender, EventArgs e)
    {
        lbtn_channel.Text = "Add sales channel";
        lbtn_channel.Enabled = true;
        panel_addchannel.Visible = false;
    }

    protected void btn_addchannel_Click(object sender, EventArgs e)
    {
        lbtn_channel.Text = "Add sales channel";
        lbtn_channel.Enabled = true;
        panel_addchannel.Visible = false;

        string str_name = tbox_name.Text.Trim();
        addchannel(str_name);
        bindDataSource();
    }

    protected void gv_channel_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        gv_channel.PageIndex = e.NewPageIndex;
        bindDataSource();
    }

    /// <summary>
    /// delete one sales channel.
    /// </summary>
    /// <param name="str_id">ID</param>
    /// <param name="str_ownbusiness">RC own business</param>
    private void delchannel(string str_id, string str_name)
    {
        string sql = "UPDATE [SalesChannel] SET Deleted = 1 WHERE ID = " + str_id;
        int count = helper.ExecuteNonQuery(CommandType.Text, sql, null);
        if (count == 1)
        {
            label_del.ForeColor = System.Drawing.Color.Green;
            label_del.Text = info.delLabelInfo(str_name, true);
        }
        else
        {
            label_del.ForeColor = System.Drawing.Color.Red;
            label_del.Text = info.delLabelInfo(str_name, false);
        }
    }

    protected void gv_channel_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        null_input();

        string str_id = gv_channel.Rows[e.RowIndex].Cells[0].Text.Trim();
        string str_name = gv_channel.Rows[e.RowIndex].Cells[1].Text.Trim();
        delchannel(str_id, str_name);

        bindDataSource();
    }

    protected void gv_channel_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (e.Row.RowState == DataControlRowState.Normal || e.Row.RowState == DataControlRowState.Alternate)
            {
                ((ImageButton)e.Row.Cells[gv_channel.Columns.Count - 1].Controls[0]).Attributes.Add("onclick", info.setRowDataBound(e.Row.Cells[1].Text));
            }
        }
    }

    //Find
    FindList list = new FindList();

    protected void ddlist_in_SelectedIndexChanged(object sender, EventArgs e)
    {
        int sel = int.Parse(ddlist_in.SelectedItem.Value.Trim());
        ddlist_find.Items.Clear();
        switch (sel)
        {
            case 0:
                {
                    list.bindFind(list.getSalesChannel(), ddlist_find);
                    break;
                }
        }
    }
}