using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;

public partial class AssistantSegmentProduct : System.Web.UI.Page
{
    SQLHelper helper = new SQLHelper();
    LogUtility log = new LogUtility();
    SQLStatement sql = new SQLStatement();
    DisplayInfo info = new DisplayInfo();

    private static string segmentID;

    protected void Page_Load(object sender, EventArgs e)
    {
        Page.MaintainScrollPositionOnPostBack = true;
        if (getRoleID(getRole()) == "0")
        {
            panel_readonly1.Visible = true;
            panel_readonly2.Visible = true;
        }
        else if (getRoleID(getRole()) == "5")
        {
            panel_readonly1.Visible = false;
            panel_readonly2.Visible = false;
        }
        else
        {
            Response.Redirect("~/AccessDenied.aspx");
        }
        if (!IsPostBack)
        {
            clearCentent();
            panel_addsegment.Visible = false;
            panel_addproduct.Visible = false;
            hiddenPro(false);
            bindDataSource(sql.getSegmentInfo(), gv_segment, true);
        }
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

    /// <summary>
    ///  Binding segments or products data to GridView control.
    /// </summary>
    /// <param name="ds">Data collection of binding data</param>
    /// <param name="gv">ID of GridView control</param>
    /// <param name="flag">if flag is true, data collection is segments, else products</param>
    protected void bindDataSource(DataSet ds, GridView gv, bool flag)
    {
        bool notNullFlag = true;                           //The flag whether or not dataset is null

        if (ds.Tables[0].Rows.Count == 0)
        {
            notNullFlag = false;
            sql.getNullDataSet(ds);
        }
        //By Wsy 20110512 ITEM 18 DEL Start 
        //gv.Width = Unit.Pixel(800);
        //By Wsy 20110512 ITEM 18 DEL End 

        //By Wsy 20110512 ITEM 18 ADD Start 
        gv.Width = Unit.Pixel(750);
        //By Wsy 20110512 ITEM 18 ADD End 
        gv.AutoGenerateColumns = false;
        gv.AllowPaging = false;
        gv.Visible = true;

        for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
        {
            BoundField bf = new BoundField();

            bf.DataField = ds.Tables[0].Columns[i].ColumnName.ToString();
            bf.HeaderText = ds.Tables[0].Columns[i].Caption.ToString();
            bf.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
            bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;

            if (flag)
            {
                if (i == 1)
                    bf.ItemStyle.Width = 100;
                else
                    bf.ItemStyle.Width = 400;
            }
            else
            {
                if (i == 1)
                {
                    bf.ItemStyle.Width = 100;
                    bf.ReadOnly = true;
                }
                else
                    bf.ItemStyle.Width = 480;
            }
            bf.ControlStyle.Width = bf.ItemStyle.Width;

            gv.Columns.Add(bf);
        }

        if (flag)
        {
            CommandField cf_Select = new CommandField();
            cf_Select.ButtonType = ButtonType.Image;
            cf_Select.ShowSelectButton = true;
            cf_Select.ShowCancelButton = true;
            cf_Select.SelectImageUrl = "~/images/search.jpg";
            cf_Select.SelectText = "Search products that segment consist of";
            cf_Select.CausesValidation = false;
            gv.Columns.Add(cf_Select);
        }

        CommandField cf_Update = new CommandField();
        cf_Update.ButtonType = ButtonType.Image;
        cf_Update.ShowEditButton = true;
        cf_Update.ShowCancelButton = true;
        cf_Update.EditImageUrl = "~/images/edit.jpg";
        cf_Update.EditText = "Edit segment";
        cf_Update.CausesValidation = false;
        cf_Update.CancelImageUrl = "~/images/cancel.jpg";
        cf_Update.CancelText = "Cancel";
        cf_Update.UpdateImageUrl = "~/images/ok.jpg";
        cf_Update.UpdateText = "Update";
        gv.Columns.Add(cf_Update);

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

        gv.Columns[gv.Columns.Count - 1].Visible = notNullFlag;
        gv.Columns[gv.Columns.Count - 2].Visible = notNullFlag;
        gv.Columns[gv.Columns.Count - 3].Visible = notNullFlag;
        gv.Columns[0].Visible = false;
        if (getRoleID(getRole()) != "0")
        {
            gv.Columns[gv.Columns.Count - 1].Visible = false;
            gv.Columns[gv.Columns.Count - 2].Visible = false;
        }
    }

    protected void bindDataSource(string str_segmentID)
    {
        gv_segment.Columns.Clear();
        gv_product.Columns.Clear();

        bindDataSource(sql.getSegmentInfo(), gv_segment, true);
        bindDataSource(sql.getProductInfoBySegmentID(str_segmentID), gv_product, false);
    }

    /* * * *  The followings are some operations about segment    * * * */

    protected void clearCentent()
    {
        labelseg_edit_del.Text = "";
        labelpro_edit_del.Text = "";

        label_addsegment.Text = "";
        label_addproduct.Text = "";

        tbox_SegmentDes.Text = "";
        tbox_productDes.Text = "";
    }

    protected void hiddenPro(bool isVisible)
    {
        gv_product.Visible = isVisible;
        label_addproduct.Visible = isVisible;
        labelpro_edit_del.Visible = isVisible;
        lbtn_AddProduct.Visible = isVisible;
        panel_addproduct.Visible = isVisible;
    }

    protected void gv_segment_SelectedIndexChanging(object sender, GridViewSelectEventArgs e)
    {
        lbtn_AddSegment.Enabled = true;
        panel_addsegment.Visible = false;
        lbtn_AddSegment.Text = "Add Segment";
        clearCentent();
        hiddenPro(true);

        lbtn_AddProduct.Enabled = true;
        lbtn_AddProduct.Text = "Add Product";

        gv_segment.SelectedIndex = e.NewSelectedIndex;
        int index = 0;
        index = gv_segment.SelectedIndex;
        if (index < 0)
            Response.Redirect("~/Admin/AdminError.aspx");
        segmentID = gv_segment.Rows[index].Cells[0].Text.Trim();

        gv_segment.SelectedIndex = -1;
        panel_addproduct.Visible = false;
        bindDataSource(segmentID);

    }

    protected void gv_segment_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        gv_segment.EditIndex = -1;
        lbtn_AddSegment.Enabled = true;
        panel_addsegment.Visible = false;
        lbtn_AddSegment.Text = "Add Segment";

        gv_segment.Columns.Clear();
        bindDataSource(sql.getSegmentInfo(), gv_segment, true);
    }

    protected void gv_segment_RowEditing(object sender, GridViewEditEventArgs e)
    {
        gv_segment.SelectedIndex = -1;
        gv_product.Visible = false;
        lbtn_AddSegment.Enabled = false;
        panel_addsegment.Visible = false;
        lbtn_AddSegment.Text = "Add Segment";
        clearCentent();
        hiddenPro(false);
        gv_segment.EditIndex = e.NewEditIndex;

        gv_segment.Columns.Clear();
        bindDataSource(sql.getSegmentInfo(), gv_segment, true);
    }

    protected void gv_segment_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        string str_abbr;
        string str_description;
        string str_ID;

        labelseg_edit_del.Visible = true;
        str_abbr = ((TextBox)(gv_segment.Rows[e.RowIndex].Cells[1].Controls[0])).Text.Trim();
        str_description = ((TextBox)(gv_segment.Rows[e.RowIndex].Cells[2].Controls[0])).Text.Trim();
        str_ID = sql.getSegmentInfo().Tables[0].Rows[e.RowIndex][0].ToString().Trim();

        updateSegment(str_abbr, str_description, str_ID);

        gv_segment.EditIndex = -1;
        lbtn_AddSegment.Enabled = true;
        panel_addsegment.Visible = false;
        lbtn_AddSegment.Text = "Add Segment";

        gv_segment.Columns.Clear();
        bindDataSource(sql.getSegmentInfo(), gv_segment, true);
    }

    /// <summary>
    /// Modify segment information
    /// </summary>
    /// <param name="str_abbr">Segment Abbr</param>
    /// <param name="str_description">Segment Description</param>
    /// <param name="str_ID">Segment ID</param>
    private void updateSegment(string str_abbr, string str_description, string str_ID)
    {
        bool success;

        labelseg_edit_del.ForeColor = System.Drawing.Color.Red;
        if (str_abbr.Trim() != "")
        {
            success = sql.updateSegment(str_abbr, str_description, str_ID);

            if (success)
            {
                labelseg_edit_del.ForeColor = System.Drawing.Color.Green;
                labelseg_edit_del.Text = info.edtLabelInfo(str_abbr, true);
            }
            else
                labelseg_edit_del.Text = info.edtLabelInfo(str_abbr, false);
        }
        else
            labelseg_edit_del.Text = info.addillegal("Abbr cannot be null.");
    }

    protected void gv_segment_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        string str_ID;
        string str_abbr;

        labelseg_edit_del.Visible = true;
        gv_product.Visible = false;
        gv_segment.SelectedIndex = -1;
        str_ID = gv_segment.Rows[e.RowIndex].Cells[0].Text.ToString().Trim();
        str_abbr = gv_segment.Rows[e.RowIndex].Cells[1].Text.Trim();

        delSegment(str_ID, str_abbr);

        lbtn_AddSegment.Enabled = true;
        panel_addsegment.Visible = false;
        lbtn_AddSegment.Text = "Add Segment";
        hiddenPro(false);

        gv_segment.Columns.Clear();
        bindDataSource(sql.getSegmentInfo(), gv_segment, true);
    }

    /// <summary>
    /// Delete segment,If consists of product,the segment can not be deleted
    /// </summary>
    /// <param name="str_ID"></param>
    /// <param name="str_description"></param>
    private void delSegment(string str_ID, string str_abbr)
    {
        bool success;

        labelseg_edit_del.ForeColor = System.Drawing.Color.Red;
        if (sql.getProductInfoBySegmentID(str_ID).Tables[0].Rows.Count == 0)
        {
            success = sql.delSegment(str_ID);
            if (success)
            {
                labelseg_edit_del.ForeColor = System.Drawing.Color.Green;
                labelseg_edit_del.Text = info.delLabelInfo(str_abbr, true);
            }
            else
                labelseg_edit_del.Text = info.delLabelInfo(str_abbr, false);
        }
        else
            labelseg_edit_del.Text = info.delconsistContent(str_abbr, "products");

    }

    protected void gv_segment_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (e.Row.RowState == DataControlRowState.Normal || e.Row.RowState == DataControlRowState.Alternate)
            {
                ((ImageButton)e.Row.Cells[gv_segment.Columns.Count - 1].Controls[0]).Attributes.Add("onclick", info.setRowDataBound(e.Row.Cells[1].Text));
            }
        }
    }

    protected void btn_AddSegment_Click(object sender, EventArgs e)
    {
        string str_abbr;
        string str_description;


        labelseg_edit_del.Visible = true;
        lbtn_AddSegment.Enabled = true;
        lbtn_AddSegment.Text = "Add Segment";
        panel_addsegment.Visible = false;

        str_abbr = tbox_segmentAbbr.Text.Trim();
        str_description = tbox_SegmentDes.Text.Trim();

        addSegment(str_abbr,str_description);

        gv_segment.Columns.Clear();
        bindDataSource(sql.getSegmentInfo(), gv_segment, true);
    }

    /// <summary>
    /// Add segment,if abbr or description has existed, the segment can not be added
    /// </summary>
    /// <param name="str_abbr"></param>
    /// <param name="str_description"></param>
    private void addSegment(string str_abbr, string str_description)
    {
        bool success;

        label_addsegment.ForeColor = System.Drawing.Color.Red;
        if (str_abbr != "")
        {
            if (!sql.existSegment(str_abbr))
            {
                success = sql.addSegment(str_abbr, str_description);

                if (success)
                {
                    label_addsegment.ForeColor = System.Drawing.Color.Green;
                    label_addsegment.Text = info.addLabelInfo(str_abbr, true);
                }
                else
                    label_addsegment.Text = info.addLabelInfo(str_abbr, false);
            }
            else
                label_addsegment.Text = info.addExist(str_abbr);
        }
        else
            label_addsegment.Text = info.addillegal("Abbr cannot be null!");
    }

    protected void btn_CancelSegment_Click(object sender, EventArgs e)
    {
        lbtn_AddSegment.Enabled = true;
        lbtn_AddSegment.Text = "Add Segment";
        panel_addsegment.Visible = false;
        clearCentent();
    }

    protected void lbtn_AddSegment_Click(object sender, EventArgs e)
    {
        lbtn_AddSegment.Enabled = false;
        lbtn_AddSegment.Text = "Input Segment Description and Abbr(Notice:Abbr's length is not more than 10)";
        panel_addsegment.Visible = true;
    }

    /* * * *  The followings are some operations about products    * * * */
    protected void gv_product_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        gv_product.EditIndex = -1;
        lbtn_AddProduct.Enabled = true;
        lbtn_AddProduct.Text = "Add Product";
        panel_addproduct.Visible = false;

        bindDataSource(segmentID);
    }

    protected void gv_product_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        string str_ID;
        string str_abbr;

        labelpro_edit_del.Visible = true;
        str_ID = gv_product.Rows[e.RowIndex].Cells[0].Text.ToString().Trim();
        str_abbr = gv_product.Rows[e.RowIndex].Cells[1].Text.Trim();

        delProduct(str_ID, str_abbr);

        gv_product.Columns.Clear();
        bindDataSource(segmentID);
    }

    /// <summary>
    /// Delete Product
    /// </summary>
    /// <param name="str_ID"></param>
    /// <param name="str_abbr"></param>
    private void delProduct(string str_ID, string str_abbr)
    {
        bool success;

        success = sql.delProduct(str_ID);

        if (success)
        {
            labelpro_edit_del.ForeColor = System.Drawing.Color.Green;
            labelpro_edit_del.Text = info.delLabelInfo(str_abbr, true);
        }
        else
        {
            labelpro_edit_del.ForeColor = System.Drawing.Color.Red;
            labelpro_edit_del.Text = info.delLabelInfo(str_abbr, false);
        }
    }

    protected void gv_product_RowEditing(object sender, GridViewEditEventArgs e)
    {
        gv_product.EditIndex = e.NewEditIndex;
        lbtn_AddProduct.Enabled = false;
        lbtn_AddProduct.Text = "Add Product";
        panel_addproduct.Visible = false;

        bindDataSource(segmentID);
    }

    protected string getProductID(string str_abbr)
    {
        string query_segment = "SELECT MAX(ID) FROM [Product] "
                               + " WHERE Abbr = '" + str_abbr + "'";
        DataSet ds_segment = helper.GetDataSet(query_segment);

        if (ds_segment.Tables[0].Rows.Count == 1)
            return ds_segment.Tables[0].Rows[0][0].ToString().Trim();
        else
            return "";
    }

    protected void gv_product_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        string str_abbr;
        string str_description;
        string str_ID;

        labelpro_edit_del.Visible = true;
        str_abbr = gv_product.Rows[e.RowIndex].Cells[1].Text.Trim();
        str_description = ((TextBox)(gv_product.Rows[e.RowIndex].Cells[2].Controls[0])).Text.Trim();
        str_ID = sql.getProductInfoBySegmentID(segmentID).Tables[0].Rows[e.RowIndex][0].ToString().Trim();

        updateProduct(str_abbr, str_description, str_ID);

        gv_product.Columns.Clear();
        bindDataSource(segmentID);
        lbtn_AddProduct.Enabled = true;
    }

    /// <summary>
    /// Modify Product
    /// </summary>
    /// <param name="str_abbr"></param>
    /// <param name="str_description"></param>
    /// <param name="str_note"></param>
    /// <param name="str_ID"></param>
    private void updateProduct(string str_abbr, string str_description, string str_ID)
    {
        bool success;

        success = sql.updateProduct(str_description, str_ID);
        if (success)
        {
            gv_product.EditIndex = -1;
            labelpro_edit_del.ForeColor = System.Drawing.Color.Green;
            labelpro_edit_del.Text = info.edtLabelInfo(str_abbr, true);
        }
        else
        {
            labelpro_edit_del.ForeColor = System.Drawing.Color.Red;
            labelpro_edit_del.Text = info.edtLabelInfo(str_abbr, false);
        }
    }

    protected void gv_product_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (e.Row.RowState == DataControlRowState.Normal || e.Row.RowState == DataControlRowState.Alternate)
            {
                ((ImageButton)e.Row.Cells[gv_product.Columns.Count - 1].Controls[0]).Attributes.Add("onclick", info.setRowDataBound(e.Row.Cells[1].Text));
            }
        }
    }

    protected void lbtn_AddProduct_Click(object sender, EventArgs e)
    {
        lbtn_AddProduct.Enabled = false;
        lbtn_AddProduct.Text = "Input Product Abbr(Notice:Abbr's length is not more than 20)";
        panel_addproduct.Visible = true;
    }

    protected void btn_addproduct_Click(object sender, EventArgs e)
    {
        labelpro_edit_del.Visible = true;
        lbtn_AddProduct.Enabled = true;
        lbtn_AddProduct.Text = "Add Product";
        panel_addproduct.Visible = false;

        string str_abbr = tbox_productDes.Text.Trim();
        addProduct(str_abbr);

        gv_product.Columns.Clear();
        bindDataSource(segmentID);
    }

    /// <summary>
    /// Add Product
    /// </summary>
    /// <param name="str_abbr"></param>
    private void addProduct(string str_abbr)
    {
        bool success;

        label_addproduct.ForeColor = System.Drawing.Color.Red;
        if (str_abbr != "")
        {
            if (!sql.existProduct(segmentID, str_abbr))
            {
                //success = sql.addProduct(str_abbr);

                //if (success)
                //{
                //    string str_proID = getProductID(str_abbr);
                //    string insert_related = "INSERT INTO [Segment_Product](SegmentID,ProductID,Deleted)  VALUES(" + segmentID + "," + str_proID + ",0)";
                //    int count = helper.ExecuteNonQuery(CommandType.Text, insert_related, null);
                //    if (count == 1)
                //    {
                //        label_addproduct.ForeColor = System.Drawing.Color.Green;
                //        label_addproduct.Text = info.addLabelInfo(str_abbr, true);
                //    }
                //}
                //else
                //    label_addproduct.Text = info.addLabelInfo(str_abbr, false);

                success = sql.updProductByAbbr(str_abbr);
                if (!success)
                    success = sql.addProduct(str_abbr);
                //if (success)
                //{
                //success = sql.addProduct(str_abbr);

                if (success)
                {
                    string str_proID = getProductID(str_abbr);
                    string insert_related = "INSERT INTO [Segment_Product](SegmentID,ProductID,Deleted)  VALUES(" + segmentID + "," + str_proID + ",0)";
                    int count = helper.ExecuteNonQuery(CommandType.Text, insert_related, null);
                    if (count == 1)
                    {
                        label_addproduct.ForeColor = System.Drawing.Color.Green;
                        label_addproduct.Text = info.addLabelInfo(str_abbr, true);
                    }
                }
                else
                    label_addproduct.Text = info.addLabelInfo(str_abbr, false);
                //}
                //else
                //{
                //    label_addproduct.ForeColor = System.Drawing.Color.Green;
                //    label_addproduct.Text = info.addLabelInfo(str_abbr, true);
                //}
            }
            else
                label_addproduct.Text = info.addExist(str_abbr);
        }
        else
            label_addproduct.Text = info.addillegal("Abbr:" + str_abbr);
    }

    protected void btn_cancelproduct_Click(object sender, EventArgs e)
    {
        lbtn_AddProduct.Enabled = true;
        lbtn_AddProduct.Text = "Add Product";
        panel_addproduct.Visible = false;
        clearCentent();
    }
}
