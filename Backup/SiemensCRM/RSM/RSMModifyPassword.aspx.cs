using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using System.Data;
using System.Data.SqlClient;

public partial class RSM_RSMModifyPassword : System.Web.UI.Page
{
    SQLHelper helper = new SQLHelper();

    protected void Page_Load(object sender, EventArgs e)
    {

    }

    private string getUserAlias()
    {
        return Session["Alias"].ToString().Trim();
    }

    private bool checkPassword()
    {
        string str_password = tbox_password.Text.Trim();
        string str_refirm = tbox_refirmpassword.Text.Trim();
        //By Wsy 20110519 ITEM W6 DEL Start 
        /*if (str_password.Equals(str_refirm))
            return true;
        else
            return false;*/
        //By Wsy 20110519 ITEM W6 DEL End 

        //By Wsy 20110519 ITEM W6 ADD Start 
        if (!string.IsNullOrEmpty(str_password))
        {
            if (str_password.Equals(str_refirm))
                return true;
            else
                return false;
        }
        else {
            return false;
        }
        //By Wsy 20110519 ITEM W6 ADD End 
       
    }

    protected void btn_ok_Click(object sender, EventArgs e)
    {
        string str_useralias = getUserAlias();
        string str_newpassword = tbox_password.Text.Trim();
        lbl_error.ForeColor = System.Drawing.Color.Red;

        if (checkPassword())
        {
            string str_passwordmd5 = FormsAuthentication.HashPasswordForStoringInConfigFile(str_newpassword, "MD5");
            string sql = "UPDATE [User] SET Password = '" + str_passwordmd5 + "' WHERE Alias = '" + str_useralias + "' AND Deleted = 0";
            int count = helper.ExecuteNonQuery(CommandType.Text, sql, null);

            if (count > 0)
            {
                lbl_error.ForeColor = System.Drawing.Color.Green;
                Session["Password"] = str_passwordmd5;
                lbl_error.Text = "Succeed in modifying password.";
            }
            else
            {
                lbl_error.Text = "System error.";
            }
        }
        else
        {
            //By Wsy 20110519 ITEM W6 DEL Start 
            //lbl_error.Text = "The two passwords is inconsistent.";
            //By Wsy 20110519 ITEM W6 DEL End 

            //By Wsy 20110519 ITEM W6 ADD Start 
            if (string.IsNullOrEmpty(str_newpassword))
            {
                lbl_error.Text = "Password can not be Null.";
            }
            else { 
                lbl_error.Text = "The two passwords is inconsistent."; 
            }
            //By Wsy 20110519 ITEM W6 ADD End 
        }
    }
}
