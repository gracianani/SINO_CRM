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
using System.Text;
using System.Data.SqlClient;


/*
 * File Name      : AdminRSMInfoClickSelect.aspx.cs
 * 
 * Description    : User select operation and subregion
 * 
 * Author         : ryzhang
 * 
 * Modify Date    : 2010-12-09
 * 
 * Problem        : problem
 * 
 * Version        : Release (1.0)
 */
public partial class Admin_AdminRSMInfoClickSelect : System.Web.UI.Page
{
    SQLHelper helper = new SQLHelper();
    LogUtility log = new LogUtility();
    DisplayInfo info = new DisplayInfo();
    SQLStatement sql = new SQLStatement();
    protected void Page_Load(object sender, EventArgs e)
    {
        this.btn_selectAll.Attributes.Add("onclick","var items = document.getElementsByTagName('input');"+
            "for(i=0;i<items.length;i++){"+
            "if(items[i].type=='checkbox'){items[i].checked = 'checked';}} return false;");
        this.btn_selectNone.Attributes.Add("onclick", "var items = document.getElementsByTagName('input');" +
            "for(i=0;i<items.length;i++){" +
            "if(items[i].type=='checkbox'){items[i].checked = '';}} return false;");
        this.btn_cancel.Attributes.Add("onclick", "closeWindow();return false;");
        if (getRoleID(getRole()) == "0")
        {
            string query_string = null;
            string query_string_selected = null;
            if (Request.QueryString["select"] == "operation")
            {   
                //Get operation items
                query_string = "SELECT ID, AbbrL FROM [Operation] WHERE Deleted = 0"
                                    + " GROUP BY AbbrL,ID"
                                    + " ORDER BY AbbrL ASC";
                query_string_selected = "SELECT OperationID FROM [User_Operation] WHERE Deleted = 0 AND UserID = '" + Request.QueryString["userid"] + "'";
                Page.Title = "Add Operation";
                this.lbl_selectInfo.Text = "Please Select Operation";
            }
            else if (Request.QueryString["select"] == "subregion")
            {
                //Get subregion items
                query_string = "SELECT [SubRegion].ID, [Country].ISO_Code + '(' + [SubRegion].Name + ')' FROM [Country_SubRegion] "
                            + " INNER JOIN [Country] ON [Country].ID = [Country_SubRegion].CountryID"
                            + " INNER JOIN [SubRegion] ON [SubRegion].ID = [Country_SubRegion].SubRegionID"
                            + " WHERE [SubRegion].Deleted = 0"
                            + " AND Country_SubRegion.Deleted=0 "
                            + " AND Country.Deleted=0 "
                            + " GROUP BY [SubRegion].Name,[SubRegion].ID,[Country].ISO_Code"
                            + " ORDER BY [Country].ISO_Code ASC";
                query_string_selected = "SELECT CountryID FROM [User_Country] WHERE Deleted = 0 AND UserID = '" + Request.QueryString["userid"] + "'";
                Page.Title = "Add Subregion";
                this.lbl_selectInfo.Text = "Please Select Subregion";
            }
            if (query_string != null)
            {
                DataSet ds = helper.GetDataSet(query_string);
                DataSet ds_select = helper.GetDataSet(query_string_selected);
                DataRowCollection rows = ds.Tables[0].Rows;
                DataRowCollection rowsSelected = ds_select.Tables[0].Rows;
                StringBuilder sb = new StringBuilder("[");
                for (int i = 0; i < rows.Count; i++)
                {
                    ListItem li = new ListItem(rows[i][1].ToString(), rows[i][0].ToString());
                    bool pd = false;
                    foreach (DataRow var in rowsSelected)
                    {
                        if (var[0].ToString() == rows[i][0].ToString())
                        {
                            pd = true;
                        }
                    }
                    if (pd)
                    {
                        li.Selected = true;
                    }
                    chk_list.Items.Add(li);
                    sb.Append("{ id:'" + rows[i][0] + "'},");
                }
                this.txt_keyAndValue.Value = sb.ToString().Substring(0, sb.ToString().Length - 1) + "]";
                this.btn_submit.Attributes.Add("onclick", "modifyOperation();return false;");
            }
           
        }
        else
        {
            Response.Redirect("~/AccessDenied.aspx");
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

    private bool IsExists(String id, DataRowCollection drc)
    {
        bool pd = false;
        foreach (DataRow var in drc)
        {
            if (var[0].ToString() == id)
            {
                pd = true;
                break;
            }
        }
        return pd;
    }
}
