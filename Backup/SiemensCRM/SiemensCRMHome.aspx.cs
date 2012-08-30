using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;

public partial class SiemensCRMHome : System.Web.UI.Page
{
    SQLHelper helper = new SQLHelper();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ddlist_role.Items.Clear();
            ddl_indentity_bind(getRoleByAlias(getAlias()));
        }
    }

    private string getAlias()
    {
        //By Wsy 20110504 ITEM 28 ADD Start 
          string userName = Session["Alias"].ToString().Trim();
          string[] user = userName.Split('.');
          string loginName = user[0] + " " + user[1];
          lbl_welcom.Text = "Welcome " + loginName;
          //By Wsy 20110504 ITEM 28 ADD END
        return Session["Alias"].ToString().Trim();
    }

    private void ddl_indentity_bind(DataSet ds)
    {
        if (ds != null)
        {
            int count = ds.Tables[0].Rows.Count;
            int index = 0;
            while (index < count)
            {
                ListItem li = new ListItem(ds.Tables[0].Rows[index][0].ToString().Trim(), ds.Tables[0].Rows[index][1].ToString().Trim());
                ddlist_role.Items.Add(li);

                index++;
            }
            btn_login.Enabled = true;
        }
        else
            btn_login.Enabled = false;
    }

    private string getUserID(string str_user, string str_roleID)
    {
        if (str_user != "" || str_user != null)
        {
            string sql = "SELECT UserID FROM [User] WHERE Alias='" + str_user + "' AND RoleID = '" + str_roleID + "' AND Deleted = 0";
            DataSet ds = helper.GetDataSet(sql);

            if (ds.Tables[0].Rows.Count == 1)
                return ds.Tables[0].Rows[0][0].ToString().Trim();
        }
        return "";
    }

    private DataSet getRoleByAlias(string user_alias)
    {
        if (user_alias != "" || user_alias != null)
        {
            string sql = "SELECT [Role].Name, [Role].ID FROM [Role] INNER JOIN [User]"
                       + " ON [Role].ID = [User].RoleID"
                       + " WHERE [User].Alias = '" + user_alias + "' AND [User].Deleted = 0";
            DataSet ds = helper.GetDataSet(sql);

            if (ds.Tables[0].Rows.Count == 0)
                return null;
            else
                return ds;
        }
        else
            return null;
    }

    protected void btn_login_Click(object sender, EventArgs e)
    {
        string roleID = ddlist_role.SelectedItem.Value.Trim();
        string roleName = ddlist_role.SelectedItem.Text.Trim();
        getWelcomStringByRole(roleID, roleName);
    }

    private void getWelcomStringByRole(string roleID, string roleName)
    {
        if (ddlist_role.SelectedItem.Value.Trim().Equals("0"))
        {
            Session["Role"] = "Administrator";
            Session["WelcomStr"] = "Welcome " + getAlias() + " (Administrator)";
            Session["AdministratorID"] = getUserID(getAlias(),"0");
            //Response.Redirect("~/Admin/AdminAccountProfile.aspx");
            Response.Redirect("~/Admin/AdminFirstPage.aspx");
        }
        else if (ddlist_role.SelectedItem.Value.Trim().Equals("1"))
        {
            Session["Role"] = "Executive";
            Session["WelcomStr"] = "Welcome " + getAlias() + " (Executive)";
            Session["ExecutiveID"] = getUserID(getAlias(), "1");
            //Response.Redirect("~/Executive/ExecutiveProfile.aspx");
            Response.Redirect("~/Executive/ExecutiveFirstPage.aspx");
        }
        else if (ddlist_role.SelectedItem.Value.Trim().Equals("2"))
        {
            Session["Role"] = "GeneralMarketingMgr";
            Session["WelcomStr"] = "Welcome " + getAlias() + " (General Marketing Manager)";
            Session["GeneralMarketingMgrID"] = getUserID(getAlias(), "2");
            //Response.Redirect("~/MarketingMgr/MarketingMgrProfile.aspx");
            Response.Redirect("~/MarketingMgr/MarketingMgrFirstPage.aspx");
        }
        else if (ddlist_role.SelectedItem.Value.Trim().Equals("3"))
        {
            Session["GeneralSalesOrgMgrID"] = getUserID(getAlias(), "3");
            Session["Role"] = "GeneralSalesMgr";
            Session["WelcomStr"] = "Welcome " + getAlias() + " (General Sales Manager)";
            //Response.Redirect("~/SalesOrgMgr/SalesOrgMgrAccountProfile.aspx");
            Response.Redirect("~/SalesOrgMgr/SalesOrgMgrFirstPage.aspx");
        }
        else if (ddlist_role.SelectedItem.Value.Trim().Equals("4"))
        {
            Session["RSMID"] = getUserID(getAlias(), "4");
            Session["Role"] = "RSM";
            Session["WelcomStr"] = "Welcome " + getAlias() + " (Regional Sales Manager)";
            //Response.Redirect("~/RSM/RSMProfile.aspx");
            Response.Redirect("~/RSM/RSMFirstPage.aspx");
        }
        else if (ddlist_role.SelectedItem.Value.Trim().Equals("5"))
        {
            Session["AssistantID"] = getUserID(getAlias(), "5");
            Session["Role"] = "Assistant";
            Session["WelcomStr"] = "Welcome " + getAlias() + " (Assistant S&M)";
            //Response.Redirect("~/Assistant/AssistantProfile.aspx");
            Response.Redirect("~/Assistant/AssistantFirstPage.aspx");
        }
        else
        {
            Response.Redirect("~/AccessDenied.aspx");
        }
    }
    protected void btn_off_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/SiemensCRMEnter.aspx");
    }
}
