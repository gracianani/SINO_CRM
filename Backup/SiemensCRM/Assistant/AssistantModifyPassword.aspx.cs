using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using System.Data;
using System.Data.SqlClient;

public partial class Assistant_AssistantModifyPassword : System.Web.UI.Page
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
        // by FXW 20110520 ITEM W6 add Start
        if (string.IsNullOrEmpty(str_password))
            return false;
        // by FXW 20110520 ITEM W6 add end
        if (str_password.Equals(str_refirm))
            return true;
        else
            return false;
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
            // by FXW 20110520 ITEM W6 add Start
            string str_password = tbox_password.Text.Trim();
            if (string.IsNullOrEmpty(str_password))
                lbl_error.Text = " Password can not be Null.";
            else
            // by FXW 20110520 ITEM W6 add end
            lbl_error.Text = "The two passwords is inconsistent.";
        }
    }
}
