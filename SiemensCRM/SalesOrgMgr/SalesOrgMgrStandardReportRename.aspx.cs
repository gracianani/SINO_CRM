/*
* File Name   : SalesOrgMgrStandardReportRename.aspx.cs
* 
* Author      : yyan 
* 
* Modified Date : 2011-05-11
* 
* Problem     : none
* 
*/
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

public partial class SalesOrgMgr_SalesOrgMgrStandardReportRename : System.Web.UI.Page
{
    SQLHelper helper = new SQLHelper();
    SQLStatement sql = new SQLStatement();
    DisplayInfo info = new DisplayInfo();
    WebUtility utility = new WebUtility();

    
    protected void Page_Load(object sender, EventArgs e)
    {
        string roleID = getRoleID(getRole());
        if (!string.Equals(roleID, "3"))
        {
            Response.Redirect("~/AccessDenied.aspx");
            return;
        }
        else if (!IsPostBack)
        {
            
            SetGridView(Request.QueryString["reportid"]);
        }
    }

    
    protected void btnConfrim_Click(object sender, EventArgs e)
    {
        if (InputCheck())
        {
            TextBox txtname = null;
            HiddenField hidFieldID = null;
            HiddenField hidReportID = null;
            string sql = "UPDATE ReportValue SET NewFieldName = @Name WHERE FieldID = @FieldID AND ID = @ID";
            string sql1 = "UPDATE StandardReportValue SET NewFieldName = @Name WHERE FieldID = @FieldID AND ID = @ID";
            for (var i = 0; i < this.GridView1.Rows.Count; i++)
            {
                txtname = (TextBox)this.GridView1.Rows[i].Cells[1].FindControl("txtname");
                hidFieldID = (HiddenField)this.GridView1.Rows[i].Cells[1].FindControl("hidFieldID");
                hidReportID = (HiddenField)this.GridView1.Rows[i].Cells[1].FindControl("hidReportID");
                SqlParameter[] sqlParameter = new SqlParameter[3];
                sqlParameter[0] = new SqlParameter("@Name", txtname.Text);
                sqlParameter[1] = new SqlParameter("@FieldID", hidFieldID.Value);
                sqlParameter[2] = new SqlParameter("@ID", hidReportID.Value);
                helper.ExecuteNonQuery(CommandType.Text, sql, sqlParameter);
                helper.ExecuteNonQuery(CommandType.Text, sql1, sqlParameter);
            }
            Response.Redirect("SalesOrgMgrReportView.aspx");
        }
    }

    
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect("SalesOrgMgrReportView.aspx");
    }

   
    private void SetGridView(string reportID)
    {
        
        string sql = " SELECT FieldID,ID,FieldName,NewFieldName " +
                     " FROM ReportValue  WHERE ID=@ID ORDER BY FieldOrder ";
        SqlParameter[] sqlParameter = new SqlParameter[1];
        sqlParameter[0] = new SqlParameter("@ID", reportID);
        DataSet dataSet = helper.GetDataSet(sql, CommandType.Text,sqlParameter);
        if (dataSet.Tables.Count > 0)
        {
            DataTable dataTable = dataSet.Tables[0];
            this.GridView1.DataSource = dataTable;
            this.GridView1.DataBind();
            
            TextBox txtName = null;
            for (int i = 0; i < this.GridView1.Rows.Count; i++)
            {
                txtName = (TextBox)this.GridView1.Rows[i].Cells[1].FindControl("txtName");
                txtName.Text = dataTable.Rows[i]["NewFieldName"].ToString();
            }
        }
        else
        {
            this.btnConfrim.Visible = false;
        }
    }

    
    private bool InputCheck()
    {
        TextBox txtName = null;
        bool errorFlag = true;
        this.divError.Controls.Clear();
        for (int i = 0; i < this.GridView1.Rows.Count; i++)
        {
            txtName = (TextBox)this.GridView1.Rows[i].Cells[1].FindControl("txtName");
            #region 
            
            if (string.IsNullOrEmpty(txtName.Text.Trim()))
            {
                SetErrorInfo(new Label(), "The NEW NAME [" + this.GridView1.Rows[i].Cells[0].Text + "] can not be null!");
                errorFlag = false;
            }
            else
            {
               
                if (txtName.Text.Trim().Length > 50)
                {
                    SetErrorInfo(new Label(), "The NEW NAME [" + this.GridView1.Rows[i].Cells[0].Text + "] must be less then 50 char!");
                    errorFlag = false;
                }
                else {
                    if  (txtName.Text.Trim().Contains(" ")) {
                        SetErrorInfo(new Label(), "The NEW NAME [" + this.GridView1.Rows[i].Cells[0].Text + "] Can't contain null character!");
                        errorFlag = false;
                    }
                    //By wsy Item53 20110601 ADD Start      
                    else
                    {
                        if (utility.checkString(txtName.Text.Trim()))
                        {
                            SetErrorInfo(new Label(), "The NEW NAME [" + this.GridView1.Rows[i].Cells[0].Text + "] Can't contain special character!");
                            errorFlag = false;
                        }
                    }
                    //By wsy Item53 20110601 ADD End  
                }
            }
            #endregion
        }
        if (!errorFlag) {
            return errorFlag;
        }
        bool errorFlag1 = true;
        for (int i = 0; i < this.GridView1.Rows.Count-1; i++)
        {
            TextBox txtName1 = null;
            txtName1 = (TextBox)this.GridView1.Rows[i].Cells[1].FindControl("txtName");
            for (int j = 1; j < this.GridView1.Rows.Count; j++)
            {
                if (j != i)
                {
                    TextBox txtName2 = null;
                    txtName2 = (TextBox)this.GridView1.Rows[j].Cells[1].FindControl("txtName");
                    if (txtName1.Text.Trim().Equals(txtName2.Text.Trim()))
                    {
                        errorFlag1 = false;
                    }
                }
            }
        }
        if (!errorFlag1) {
            errorFlag = false;
            SetErrorInfo(new Label(), "New name may not be repeated!");
        }
        return errorFlag;
    }

    private void SetErrorInfo(Label label, string errorMsg)
    {
        label.ForeColor = Color.Red;
        label.Text = info.addillegal(errorMsg) + "<br/>";
        this.divError.Controls.Add(label);
    }

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