using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Collections;
using System.Threading; 
public partial class Admin_ReportValue : System.Web.UI.Page
{
    SQLHelper helper = new SQLHelper();
    LogUtility log = new LogUtility();
    SQLStatement state = new SQLStatement();
    WebUtility webU = new WebUtility();
    DisplayInfo info = new DisplayInfo();
    private static string isSubmit = "";
    /// <summary>
    /// Initialization
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        Page.MaintainScrollPositionOnPostBack = true;
        if (getRoleID(getRole()) == "0" || getRoleID(getRole()) == "5" || getRoleID(getRole()) == "6")
        {
            hidRoleId.Value = getRoleID(Session["Role"].ToString());
        }
        else
        {
            Response.Redirect("~/AccessDenied.aspx");
            return;
        }
        if (Session["AdministratorID"] != null)
        {
            hidUserId.Value = Session["AdministratorID"].ToString();
        }
        else
        {
            hidUserId.Value = "";
        }
        if (!IsPostBack)
        {
            getViewName(hidRoleId.Value);
            if (Request.QueryString["reportid"] != null)
            {
                hidReportId.Value = Request.QueryString["reportid"].ToString();
                getReportInfo(hidReportId.Value);
            }
            else
            {
                hidReportId.Value = "";
            }
            UpdateSign();
            ddlViewName_SelectedIndexChanged(sender, e);
        }
        
    }

    /// <summary>
    /// Get user'role
    /// </summary>
    /// <returns></returns>
    private string getRole()
    {
        return Session["Role"].ToString().Trim();
    }

    /// <summary>
    /// Get user'role
    /// </summary>
    /// <param name="str_name">RoleName</param>
    /// <returns></returns>
    private string getRoleID(string str_name)
    {
        DataSet ds_role = state.getRole();

        for (int i = 0; i < ds_role.Tables[0].Rows.Count; i++)
        {
            if (str_name.Equals(ds_role.Tables[0].Rows[i][0].ToString().Trim()))
            {
                return ds_role.Tables[0].Rows[i][1].ToString().Trim();
            }
        }
        return "";
    }

    /// <summary>
    /// Get viewname
    /// </summary>
    /// <param name="strRoleId">RoleId</param>
    private void getViewName(string strRoleId)
    {
        ddlViewName.Width = 200;
        DataSet dsViewName;
        dsViewName = getViewName(int.Parse(strRoleId));

        if (dsViewName.Tables[0].Rows.Count > 0)
        {
            DataTable dt = dsViewName.Tables[0];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                ddlViewName.Items.Add(new ListItem(dt.Rows[i]["ViewName"].ToString(), dt.Rows[i]["ViewName"].ToString()));
            }
        }
    }

    /// <summary>
    /// Get  viewname dataset
    /// </summary>
    /// <param name="intRoleId">RoleId</param>
    /// <returns></returns>
    protected DataSet getViewName(int intRoleId)
    {
        string sql;
        sql = "SELECT RoleID,ViewName FROM ReportRole WHERE RoleID=" + intRoleId;
        DataSet ds_query_ViewName = helper.GetDataSet(sql);
        return ds_query_ViewName;
    }

    /// <summary>
    /// Get  reportinfo
    /// </summary>
    /// <param name="strReportId">ReportId</param>
    private void getReportInfo(string strReportId)
    {
        DataSet dsReport;
        dsReport = getReportInfo(int.Parse(strReportId));
        if (dsReport.Tables[0].Rows.Count > 0)
        {
            DataTable dt = dsReport.Tables[0];
            if (dt.Rows.Count > 0)
            {
                txtName.Text = dt.Rows[0]["Name"].ToString();
                txtDepiction.Text = dt.Rows[0]["Depiction"].ToString();
                ddlViewName.SelectedValue = dt.Rows[0]["ViewName"].ToString();
                hidViewName.Value = dt.Rows[0]["ViewName"].ToString();
                hidCreateUser.Value = dt.Rows[0]["UserID"].ToString();
                DataSet dsReportValue;
                dsReportValue = getReportValue(int.Parse(strReportId));
                if (dsReportValue.Tables[0].Rows.Count > 0)
                {
                    DataTable dtReportValue = dsReportValue.Tables[0];
                    string strName = "";
                    for (int i = 0; i < dtReportValue.Rows.Count; i++)
                    {
                        strName = strName + dtReportValue.Rows[i]["FieldName"].ToString() + "|";
                    }
                    if (strName.Length > 0)
                    {
                        hidRight.Value = strName.Substring(0, strName.Length - 1);
                        hidValue.Value = hidRight.Value;
                    }
                }
            }
        }
    }
    /// <summary>
    /// Get reportinfo dataset
    /// </summary>
    /// <param name="intReportId">ReportId</param>
    /// <returns></returns>
    protected DataSet getReportInfo(int intReportId)
    {
        string sql;
        sql = "SELECT ID,Name,UserID,Depiction,ViewName,CreateDate FROM ReportList WHERE ID=" + intReportId;
        DataSet ds_query_Report = helper.GetDataSet(sql);
        return ds_query_Report;
    }

    /// <summary>
    /// Get reportvalue dataset
    /// </summary>
    /// <param name="intReportId">ReportId</param>
    /// <returns></returns>
    protected DataSet getReportValue(int intReportId)
    {
        StringBuilder sql = new StringBuilder();
        sql.Append("\n SELECT ID,FieldID,FieldName,FieldType,FieldLength,Operator,Condition1,");
        // update by zy 20101230 start
        //sql.Append("\n Condition2,Sort FROM ReportValue WHERE ID=" + intReportId + " order by Fieldid");
        sql.Append("\n Condition2,Sort FROM ReportValue WHERE ID=" + intReportId + " order by FieldOrder, Fieldid");
        // update by zy 20101230 end
        DataSet ds_query_ReportValue = helper.GetDataSet(sql.ToString());
        return ds_query_ReportValue;
    }

    /// <summary>
    /// Insert report
    /// </summary>
    /// <returns></returns>
    private bool insertReport()
    {
        string date = DateTime.Now.ToString("yyyy-MM-dd");
        SqlParameter[] sqlParameterReprot = new SqlParameter[5];
        sqlParameterReprot[0] = new SqlParameter("@Name", txtName.Text.Trim());
        sqlParameterReprot[1] = new SqlParameter("@Depiction", txtDepiction.Text.Trim());
        sqlParameterReprot[2] = new SqlParameter("@UserID", hidUserId.Value);
        sqlParameterReprot[3] = new SqlParameter("@ViewName", ddlViewName.SelectedValue);
        sqlParameterReprot[4] = new SqlParameter("@CreateDate", date);
        StringBuilder sqlReportlist = new StringBuilder();
        sqlReportlist.Append("\n INSERT INTO ReportList(Name,Depiction,UserID,ViewName,CreateDate)");
        sqlReportlist.Append("\n VALUES(@Name,@Depiction,@UserID,@ViewName,@CreateDate)");
        //insert reportlist
        int iError = helper.ExecuteNonQuery(CommandType.Text, sqlReportlist.ToString(), sqlParameterReprot);

        if (iError == 1)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("\n SELECT ID FROM ReportList where Name='" + txtName.Text.Trim() + "'");
            sql.Append("\n AND UserId=" + hidUserId.Value + " AND ViewName='" + ddlViewName.SelectedValue + "'");
            DataSet ds = helper.GetDataSet(sql.ToString());
            string strId = ds.Tables[0].Rows[0]["ID"].ToString();
            insertReportValueInfo(strId);
        }
        else
        {
            label_add.Text = info.addillegal("insert report is failed!");
            return true;
        }
        return false;
    }

    /// <summary>
    /// Update report
    /// </summary>
    /// <returns></returns>
    private bool updateReport()
    {
        string date = DateTime.Now.ToString("yyyy-MM-dd");
        SqlParameter[] sqlParameterReprot = new SqlParameter[5];
        sqlParameterReprot[0] = new SqlParameter("@Name", txtName.Text.Trim());
        sqlParameterReprot[1] = new SqlParameter("@Depiction", txtDepiction.Text.Trim());
        sqlParameterReprot[2] = new SqlParameter("@UserID", hidCreateUser.Value); //hidUserId.Value
        sqlParameterReprot[3] = new SqlParameter("@ViewName", ddlViewName.SelectedValue);
        sqlParameterReprot[4] = new SqlParameter("@CreateDate", date);
        StringBuilder sqlReportlist = new StringBuilder();
        sqlReportlist.Append("\n UPDATE ReportList SET Name=@Name,Depiction=@Depiction,UserID=@UserID,");
        sqlReportlist.Append("\n ViewName=@ViewName,CreateDate=@CreateDate WHERE Id=" + hidReportId.Value + "");
        int iError = helper.ExecuteNonQuery(CommandType.Text, sqlReportlist.ToString(), sqlParameterReprot);
        if (hidViewName.Value.Equals(ddlViewName.SelectedValue))
        {
            string sql = "";
            string[] str = hidRight.Value.Split('|');
            string strFieldName = "";
            for (int i = 0; i < str.Length; i++)
            {
                sql = "SELECT * FROM ReportValue WHERE FieldName='" + str[i] + "' AND ID=" + hidReportId.Value;
                DataSet ds = helper.GetDataSet(sql);
                if (ds.Tables[0].Rows.Count <= 0)
                {
                    string sqlField = "SELECT " + str[i] + " FROM " + ddlViewName.SelectedValue;
                    DataTable dt = helper.GetDataSetStru(sqlField);
                    if (dt.Columns.Count > 0)
                    {
                        // update by zy 20101230 start
                        //SqlParameter[] sqlParameter = new SqlParameter[4];
                        SqlParameter[] sqlParameter = new SqlParameter[5];
                        // update by zy 20101230 end
                        sqlParameter[0] = new SqlParameter("@Id", hidReportId.Value);
                        sqlParameter[1] = new SqlParameter("@FieldName", str[i]);
                        sqlParameter[2] = new SqlParameter("@FieldType", dt.Columns[0].DataType.Name);
                        sqlParameter[3] = new SqlParameter("@FieldLength", dt.Columns[0].MaxLength);
                        // add by zy 20101230 start
                        sqlParameter[4] = new SqlParameter("@FieldOrder", i);
                        // add by zy 20101230 end
                        StringBuilder sqlReportValue = new StringBuilder();
                        // update by zy 20110125 start
                        //sqlReportValue.Append("\n INSERT INTO ReportValue(ID,FieldName,FieldType,FieldLength,Sort) ");
                        //BY yyan 20110513 item 53 del start
                        //sqlReportValue.Append("\n INSERT INTO ReportValue(ID,FieldName,FieldType,FieldLength,Sort,FieldOrder) ");
                        //BY yyan 20110513 item 53 del end
                        //BY yyan 20110513 item 53 add start
                        sqlReportValue.Append("\n INSERT INTO ReportValue(ID,FieldName,FieldType,FieldLength,Sort,FieldOrder,NewFieldName) ");
                        //BY yyan 20110513 item 53 add end
                        // update by zy 20110125 end
                        // update by zy 20101230 start
                        //sqlReportValue.Append("\n VALUES( @Id,@FieldName,@FieldType,@FieldLength,0)");
                        //BY yyan 20110513 item 53 del start
                        //sqlReportValue.Append("\n VALUES( @Id,@FieldName,@FieldType,@FieldLength,0,@FieldOrder)");
                        //BY yyan 20110513 item 53 del end
                        //BY yyan 20110513 item 53 add start
                        sqlReportValue.Append("\n VALUES( @Id,@FieldName,@FieldType,@FieldLength,0,@FieldOrder,@FieldName)");
                        //BY yyan 20110513 item 53 add end
                        // update by zy 20101230 end
                        iError = helper.ExecuteNonQuery(CommandType.Text, sqlReportValue.ToString(), sqlParameter);
                    }
                }
                // add by zy 20101230 start
                else
                {
                    SqlParameter[] sqlParameter = new SqlParameter[3];
                    sqlParameter[0] = new SqlParameter("@Id", hidReportId.Value);
                    sqlParameter[1] = new SqlParameter("@FieldName", str[i]);
                    sqlParameter[2] = new SqlParameter("@FieldOrder", i);
                    StringBuilder sqlReportValue = new StringBuilder();
                    sqlReportValue.Append("\n Update ReportValue set FieldOrder = @FieldOrder ");
                    sqlReportValue.Append("\n WHERE FieldName = @FieldName AND ID = @Id");
                    iError = helper.ExecuteNonQuery(CommandType.Text, sqlReportValue.ToString(), sqlParameter);

                }
                // add by zy 20101230 end
                strFieldName = strFieldName + "'" + str[i] + "'" + ",";

            }
            strFieldName = strFieldName.Substring(0, strFieldName.Length - 1);
            sql = "DELETE FROM ReportValue WHERE ID=" + hidReportId.Value + " AND FieldName NOT IN (" + strFieldName + ")";
            helper.ExecuteNonQuery(CommandType.Text, sql, null);
        }
        else
        {
            string sqlDeleteReportValue = "DELETE FROM ReportValue WHERE ID=" + hidReportId.Value;
            helper.ExecuteNonQuery(CommandType.Text, sqlDeleteReportValue, null);
            insertReportValueInfo(hidReportId.Value);
        }
        return false;
    }

    /// <summary>
    /// Insert table
    /// </summary>
    /// <param name="strId">ReportId</param>
    private void insertReportValueInfo(string strId)
    {
        string[] str = hidRight.Value.Split('|');
        for (int i = 0; i < str.Length; i++)
        {
            string sqlField = "SELECT " + str[i] + " FROM " + ddlViewName.SelectedValue;
            DataTable dt = helper.GetDataSetStru(sqlField);
            if (dt.Columns.Count > 0)
            {
                SqlParameter[] sqlParameter = new SqlParameter[5];
                sqlParameter[0] = new SqlParameter("@Id", strId);
                sqlParameter[1] = new SqlParameter("@FieldName", str[i]);
                sqlParameter[2] = new SqlParameter("@FieldType", dt.Columns[0].DataType.Name);
                sqlParameter[3] = new SqlParameter("@FieldLength", dt.Columns[0].MaxLength);
                // add by zy 20110125 start
                sqlParameter[4] = new SqlParameter("@FieldOrder",i);
                // add by zy 20110125 end
                StringBuilder sqlReportValue = new StringBuilder();
                // update by zy 20110125 start
                //sqlReportValue.Append("\n INSERT INTO ReportValue(ID,FieldName,FieldType,FieldLength,Sort) ");
                //sqlReportValue.Append("\n VALUES( @Id,@FieldName,@FieldType,@FieldLength,0)");
                //BY yyan 20110513 item 43 del start
                //sqlReportValue.Append("\n INSERT INTO ReportValue(ID,FieldName,FieldType,FieldLength,Sort,FieldOrder) ");
                //sqlReportValue.Append("\n VALUES( @Id,@FieldName,@FieldType,@FieldLength,0,@FieldOrder)");
                //BY yyan 20110513 item 43 del end
                //BY yyan 20110513 item 43 add start
                sqlReportValue.Append("\n INSERT INTO ReportValue(ID,FieldName,FieldType,FieldLength,Sort,FieldOrder,NewFieldName) ");
                sqlReportValue.Append("\n VALUES( @Id,@FieldName,@FieldType,@FieldLength,0,@FieldOrder,@FieldName)");
                //BY yyan 20110513 item 43 add end
                // update by zy 20110125 end
                helper.ExecuteNonQuery(CommandType.Text, sqlReportValue.ToString(), sqlParameter);
            }
        }
    }

    /// <summary>
    /// Records exist to determine
    /// </summary>
    /// <param name="bz">bz</param>
    /// <returns></returns>
    private bool isExitsUser(string bz)
    {
        StringBuilder sql = new StringBuilder();
        if ("add".Equals(bz))
        {
            sql.Append("\n SELECT Name FROM Reportlist");
            sql.Append("\n WHERE UserID=" + hidUserId.Value + "");
            sql.Append("\n AND Name='" + txtName.Text.Trim() + "'");
            DataSet ds = helper.GetDataSet(sql.ToString());
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    label_add.Text = info.addillegal("Name is exits!");
                    return true;
                }
            }
            else
            {
                label_add.Text = info.addillegal(txtName.Text.Trim());
                return true;
            }
        }
        else
        {
            sql.Append("\n SELECT Name FROM Reportlist");
            sql.Append("\n WHERE Id<>" + hidReportId.Value + "");
            sql.Append("\n AND UserID=" + hidUserId.Value + "");
            sql.Append("\n AND Name='" + txtName.Text.Trim() + "'");
            DataSet ds = helper.GetDataSet(sql.ToString());
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    label_add.Text = info.addillegal("Name is exits!");
                    return true;
                }
            }
            else
            {
                label_add.Text = info.addillegal(txtName.Text.Trim());
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// View selection change
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void ddlViewName_SelectedIndexChanged(object sender, EventArgs e)
    {
        label_add.Text = "";
        DataTable dt = getViewNameByName();
        string strName = "";
        for (int i = 0; i < dt.Columns.Count; i++)
        {
            if (!"".Equals(hidReportId.Value))
            {
                if (hidViewName.Value.Equals(ddlViewName.SelectedValue))
                {
                    if (hidValue.Value.Contains(dt.Columns[i].ColumnName))
                    {
                        //by yyan 20110524 w10 add start
                        string[] strSplit = hidValue.Value.Split('|');
                        bool flag=false;
                        for (int j = 0; j < strSplit.Length; j++) {
                            if (strSplit[j].Equals(dt.Columns[i].ColumnName))
                            {
                                flag = true;
                            }
                        }
                        if (flag == false)
                        {
                            strName = strName + dt.Columns[i].ColumnName + "|";
                        }
                        //by yyan 20110524 w10 add end
                    }
                    else
                    {
                        strName = strName + dt.Columns[i].ColumnName + "|";
                    }
                }
                else
                {
                    strName = strName + dt.Columns[i].ColumnName + "|";
                }
            }
            else
            {
                strName = strName + dt.Columns[i].ColumnName + "|";
            }
        }

        hidLeft.Value = strName;
        if (hidLeft.Value.Length > 0)
        {
            hidLeft.Value = hidLeft.Value.Substring(0, hidLeft.Value.Length - 1);
        }
        if (!"".Equals(hidReportId.Value))
        {
            if (hidViewName.Value.Equals(ddlViewName.SelectedValue))
            {
                hidRight.Value = hidValue.Value;
            }
            else
            { hidRight.Value = ""; }
        }
        else
        {
            hidRight.Value = "";
        }
    }

    /// <summary>
    /// Get table structure
    /// </summary>
    /// <returns></returns>
    protected DataTable getViewNameByName()
    {
        string sql;
        sql = "SELECT * FROM " + ddlViewName.SelectedItem.Text;
        DataTable dt = helper.GetDataSetStru(sql);
        return dt;
    }


    /// <summary>
    /// Submit
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSubmit_Click(object sender, ImageClickEventArgs e)
    {
        lock(isSubmit)
        {
            if (isSubmit!="")
                return;
            isSubmit = "busy";
            label_add.Text = "";
            if ("".Equals(txtName.Text.Trim()))
            {
                label_add.Text = info.addillegal("Name can not be null!");
                UpdateSign();
                return;
            }
            else
            {
                if (txtName.Text.Length > 100)
                {
                    label_add.Text = info.addillegal("Enter Name can not be more than 100!");
                    UpdateSign();
                    return;
                }
            }
            if (txtDepiction.Text.Length > 1000)
            {
                label_add.Text = info.addillegal("Enter Depiction can not be more than 1000!");
                UpdateSign();
                return;
            }
            if ("".Equals(hidRight.Value))
            {
                label_add.Text = info.addillegal("ausgewählte Felder can not be null!");
                UpdateSign();
                return;
            }
            if ("".Equals(hidReportId.Value))
            {
                if (isExitsUser("add"))
                {
                    UpdateSign();
                    return;
                }
                insertReport();
            }
            else
            {
                if (isExitsUser("edit"))
                {
                    UpdateSign();
                    return;
                }
                string sql = "DELETE FROM StandardReportValue WHERE ID=" + hidReportId.Value;
                helper.ExecuteNonQuery(CommandType.Text, sql, null);
                updateReport();
            }


            Response.Redirect("AdminReportView.aspx");
        }
    }

    /// <summary>
    /// Return
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnCancel_Click(object sender, ImageClickEventArgs e)
    {
        Response.Redirect("AdminReportView.aspx");
    }
    //yyan itemW137 20110908 add start
    private void UpdateSign()
    {
        isSubmit= "";

    }
    //yyan itemW137 20110908 add end
}
