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
using System.Data.SqlClient;
using System.Drawing;
using System.Collections.Generic;

public partial class Admin_AdminReportCondition : System.Web.UI.Page
{
    SQLHelper helper = new SQLHelper();

    SQLStatement sql = new SQLStatement();

    DisplayInfo info = new DisplayInfo();

    WebUtility utility = new WebUtility();

    DataTable OperaterDataTable = null;

    DropDownList OperaterDropDownList = null;

    HiddenField FieldTypeHiddenField = null;

    /// <summary>
    /// page load
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        string roleID = getRoleID(getRole());
        if (!string.Equals(roleID, "0") && !string.Equals(roleID,"5") && !string.Equals(roleID,"6"))
        {
            Response.Redirect("~/AccessDenied.aspx");
            return;
        }
        else if (!IsPostBack)
        {
            
            GetOperaterData();
            
            SetGridView(Request.QueryString["reportid"]);
        }
    }

    /// <summary>
    /// data bind
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            OperaterDropDownList = (DropDownList)e.Row.Cells[1].FindControl("selOperator");
            OperaterDropDownList.Attributes.Add("onchange", "OperatorChange(this);");
            // add by zy 20110107 start
            string fieldName = ((System.Data.DataRowView)(e.Row.DataItem)).Row.ItemArray[2].ToString();
            TextBox TBCondiction1 = (TextBox)e.Row.Cells[2].FindControl("txtCondition1");
            HtmlImage IBSelect = (HtmlImage)e.Row.Cells[3].FindControl("imgSelect");
            IBSelect.Attributes.Add("onclick", string.Format("topopmask('../SelectReportCondiction.aspx',800,450,'Select Report Condition','{0}','{1}');", TBCondiction1.ClientID, fieldName));
            // add by zy 20110107 end
            FieldTypeHiddenField = (HiddenField)e.Row.Cells[3].FindControl("hidFieldType");
            BoundOperater(OperaterDropDownList, FieldTypeHiddenField.Value);
        }
    }

   
    protected void btnConfrim_Click(object sender, EventArgs e)
    {
        if (InputCheck())
        {
            DropDownList selOperator = null;
            TextBox txtCondition1 = null;
            TextBox txtCondition2 = null;
            DropDownList selSortierung = null;
            HiddenField hidFieldID = null;

            string sql = "UPDATE ReportValue SET Operator=@Operator,Condition1=@Condition1,Condition2=@Condition2,Sort=@Sort WHERE FieldID=@FieldID";
            string sql1 = "UPDATE StandardReportValue SET Operator=@Operator,Condition1=@Condition1,Condition2=@Condition2,Sort=@Sort WHERE FieldID=@FieldID";
            for (var i = 0; i < this.GridView1.Rows.Count; i++)
            {
                selOperator = (DropDownList)this.GridView1.Rows[i].Cells[1].FindControl("selOperator");
                txtCondition1 = (TextBox)this.GridView1.Rows[i].Cells[2].FindControl("txtCondition1");
                txtCondition2 = (TextBox)this.GridView1.Rows[i].Cells[2].FindControl("txtCondition2");
                selSortierung = (DropDownList)this.GridView1.Rows[i].Cells[3].FindControl("selSortierung");
                hidFieldID = (HiddenField)this.GridView1.Rows[i].Cells[3].FindControl("hidFieldID");
                SqlParameter[] sqlParameter = new SqlParameter[5];
                sqlParameter[0] = new SqlParameter("@Operator", selOperator.SelectedValue);
                sqlParameter[1] = new SqlParameter("@Condition1", txtCondition1.Text.Trim());
                sqlParameter[2] = new SqlParameter("@Condition2", txtCondition2.Text.Trim());
                sqlParameter[3] = new SqlParameter("@Sort", selSortierung.SelectedValue);
                sqlParameter[4] = new SqlParameter("@FieldID", hidFieldID.Value);
                helper.ExecuteNonQuery(CommandType.Text, sql, sqlParameter);
                helper.ExecuteNonQuery(CommandType.Text, sql1, sqlParameter);
            }
            Response.Redirect("AdminReportView.aspx");
        }
    }

   
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect("AdminReportView.aspx");
    }

    
    private void BoundOperater(DropDownList dropDownList, string fieldType)
    {
        
        dropDownList.DataSource = OperaterDataTable;
        dropDownList.DataTextField = "Name";
        dropDownList.DataValueField = "SmallType";
        dropDownList.DataBind();
        
        ListItem item = new ListItem("Please select", "");
        dropDownList.Items.Insert(0, item);
        
        if (!string.Equals(fieldType, "DateTime"))
        {
            dropDownList.Items.Remove(dropDownList.Items.FindByValue("8"));
        }
        
        else
        {
            dropDownList.Items.Remove(dropDownList.Items.FindByValue("6"));
            dropDownList.Items.Remove(dropDownList.Items.FindByValue("7"));
        }
        
        if (string.Equals(fieldType, "String"))
        {
            dropDownList.Items.Remove(dropDownList.Items.FindByValue("1"));
            dropDownList.Items.Remove(dropDownList.Items.FindByValue("2"));
            dropDownList.Items.Remove(dropDownList.Items.FindByValue("3"));
            dropDownList.Items.Remove(dropDownList.Items.FindByValue("4"));
            dropDownList.Items.Remove(dropDownList.Items.FindByValue("5"));
        }
        // add by zy 20110110 start
        else
        {
            dropDownList.Items.Remove(dropDownList.Items.FindByValue("9"));
        }
        // add by zy 20110110 end
    }

    
    private void GetOperaterData()
    {
        string sql = "SELECT * FROM DataDict WHERE BigType=1 ORDER BY Sort";
        DataSet dataSet = helper.GetDataSet(sql);
        OperaterDataTable = dataSet.Tables[0];
    }

    
    private void SetGridView(string reportID)
    {
        
        // update by zy 20110125 start
        //string sql = "SELECT * FROM ReportValue WHERE ID=@ID";
        string sql = "SELECT * FROM ReportValue WHERE ID=@ID ORDER BY FieldOrder";
        // update by zy 20110125 end
        SqlParameter[] sqlParameter = new SqlParameter[1];
        sqlParameter[0] = new SqlParameter("@ID", reportID);
        DataSet dataSet = helper.GetDataSet(sql, CommandType.Text,sqlParameter);
        if (dataSet.Tables.Count > 0)
        {
            DataTable dataTable = dataSet.Tables[0];
            this.GridView1.DataSource = dataTable;
            this.GridView1.DataBind();
            
            DropDownList selOperator = null;
            TextBox txtCondition1 = null;
            TextBox txtCondition2 = null;
            DropDownList selSortierung = null;
            for (int i = 0; i < this.GridView1.Rows.Count; i++)
            {
                selOperator = (DropDownList)this.GridView1.Rows[i].Cells[1].FindControl("selOperator");
                txtCondition1 = (TextBox)this.GridView1.Rows[i].Cells[2].FindControl("txtCondition1");
                txtCondition2 = (TextBox)this.GridView1.Rows[i].Cells[2].FindControl("txtCondition2");
                selSortierung = (DropDownList)this.GridView1.Rows[i].Cells[3].FindControl("selSortierung");
                selOperator.SelectedValue = dataTable.Rows[i]["Operator"].ToString();
                txtCondition1.Text = dataTable.Rows[i]["Condition1"].ToString();
                txtCondition2.Text = dataTable.Rows[i]["Condition2"].ToString();
                selSortierung.SelectedValue = dataTable.Rows[i]["Sort"].ToString();
            }
        }
        else
        {
            this.btnConfrim.Visible = false;
        }
    }

    
    private bool InputCheck()
    {
        DropDownList selOperator = null;
        TextBox txtCondition1 = null;
        TextBox txtCondition2 = null;
        DropDownList selSortierung = null;
        HiddenField hidFieldType = null;
        List<string> intTypeList = GetIntList();
        List<string> floatTypeList = GetFloatList();
        bool errorFlag = true;
        this.divError.Controls.Clear();
        for (int i = 0; i < this.GridView1.Rows.Count; i++)
        {
            selOperator = (DropDownList)this.GridView1.Rows[i].Cells[1].FindControl("selOperator");
            txtCondition1 = (TextBox)this.GridView1.Rows[i].Cells[2].FindControl("txtCondition1");
            txtCondition2 = (TextBox)this.GridView1.Rows[i].Cells[2].FindControl("txtCondition2");
            selSortierung = (DropDownList)this.GridView1.Rows[i].Cells[3].FindControl("selSortierung");
            hidFieldType = (HiddenField)this.GridView1.Rows[i].Cells[3].FindControl("hidFieldType");
            
            if (!string.IsNullOrEmpty(selOperator.SelectedValue))
            {
                #region 条件1相关校验
               
                if (string.IsNullOrEmpty(txtCondition1.Text.Trim()))
                {
                    SetErrorInfo(new Label(), "The Condition of Name [" + this.GridView1.Rows[i].Cells[0].Text + "] can not be null!");
                    errorFlag = false;
                }
                else
                {
                    // update by zy 20110110 start
                    
                    if (txtCondition1.Text.Trim().Length > 200)
                    {
                        SetErrorInfo(new Label(), "The Condition of Name [" + this.GridView1.Rows[i].Cells[0].Text + "] must be less then 200 char!");
                        errorFlag = false;
                    }
                    // update by zy 20110110 end
                   
                    if (intTypeList.Contains(hidFieldType.Value))
                    {
                        if (!utility.IsInteger(txtCondition1.Text.Trim()))
                        {
                            SetErrorInfo(new Label(), "The Condition of Name [" + this.GridView1.Rows[i].Cells[0].Text + "] must be Integer!");
                            errorFlag = false;
                        }
                    }
                    
                    if (floatTypeList.Contains(hidFieldType.Value))
                    {
                        if (!utility.IsFloat(txtCondition1.Text.Trim()))
                        {
                            SetErrorInfo(new Label(), "The Condition of Name [" + this.GridView1.Rows[i].Cells[0].Text + "] must be Integer or float!");
                            errorFlag = false;
                        }
                    }
                   
                    if (string.Equals(hidFieldType.Value, "DateTime"))
                    {
                        if (!utility.IsDate(txtCondition1.Text.Trim()))
                        {
                            SetErrorInfo(new Label(), "The Condition of Name [" + this.GridView1.Rows[i].Cells[0].Text + "] must be a date like 'yyyy-mm-dd')!");
                            errorFlag = false;
                        }
                    }
                }
                #endregion

                #region 
                
                if (string.Equals(selOperator.SelectedValue, "8"))
                {
                   
                    if (string.IsNullOrEmpty(txtCondition2.Text.Trim()))
                    {
                        SetErrorInfo(new Label(), "The Condition of Name [" + this.GridView1.Rows[i].Cells[0].Text + "] can not be null!");
                        errorFlag = false;
                    }
                    else
                    {
                       
                        if (!utility.CheckDate(txtCondition2.Text.Trim()))
                        {
                            SetErrorInfo(new Label(), "The Condition of Name [" + this.GridView1.Rows[i].Cells[0].Text + "] must be a date like 'yyyy-mm-dd')!");
                            errorFlag = false;
                        }
                    }
                }
                #endregion
            }

            
            if (!string.IsNullOrEmpty(txtCondition1.Text.Trim()))
            {
                
                if (string.IsNullOrEmpty(selOperator.SelectedValue))
                {
                    SetErrorInfo(new Label(), "The Operator of Name [" + this.GridView1.Rows[i].Cells[0].Text + "] can not be null!");
                    errorFlag = false;
                }
            }
        }
        return errorFlag;
    }

    
    private List<string> GetIntList()
    {
        List<string> list = new List<string>();
        list.Add("Boolean");
        list.Add("Int16");
        list.Add("Int32");
        list.Add("Int64");
        list.Add("Byte");
        return list;
    }

   
    private List<string> GetFloatList()
    {
        List<string> list = new List<string>();
        list.Add("Decimal");
        list.Add("Double");
        list.Add("Single");
        return list;
    }

    private void SetErrorInfo(Label label, string errorMsg)
    {
        label.ForeColor = Color.Red;
        label.Text = info.addillegal(errorMsg) + "<br/>";
        this.divError.Controls.Add(label);
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
}