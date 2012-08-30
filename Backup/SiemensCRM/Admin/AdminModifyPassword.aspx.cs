using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using System.Data;
using System.Data.SqlClient;

public partial class Admin_AdminModifyPassword : System.Web.UI.Page
{
    SQLHelper helper = new SQLHelper();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            bindAlias(getAllAlias());
        }
    }

    private string getUserAlias()
    {
        return Session["Alias"].ToString().Trim();
    }

    private bool checkPassword()
    {
        string str_password = tbox_password.Text.Trim();
        string str_refirm = tbox_refirmpassword.Text.Trim();

        // By SJ 20110519 Item W6 Update Start
        //if (str_password.Equals(str_refirm))
        //    return true;
        //else
        //    return false;
        if (string.IsNullOrEmpty(str_password))
            return false;
        else if (str_password.Equals(str_refirm))
            return true;
        else
            return false;
        // By SJ 20110519 Item W6 Update End
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
            // By SJ 20110519 Item W6 Add Start
            if (string.IsNullOrEmpty(str_newpassword))
                lbl_error.Text = " Password can not be Null.";
            else
            // By SJ 20110519 Item W6 Add End
            lbl_error.Text = "The two passwords is inconsistent.";
        }
        // By SJ 20110519 Item W6 Add Start
        lbl_note.Text = "";
        // By SJ 20110519 Item W6 Add End
    }

    private DataSet getAllAlias()
    {
        string sql = "SELECT Alias FROM [User] WHERE Deleted = 0 GROUP BY Alias ORDER BY Alias";
        DataSet ds = helper.GetDataSet(sql);

        return ds;
    }

    private void bindAlias(DataSet ds)
    {
        if (ds.Tables[0].Rows.Count > 0)
        {
            DataTable dt = ds.Tables[0];
            int count = dt.Rows.Count;
            int index = 0;

            while (index < count)
            {
                ddlist_user.Items.Add(dt.Rows[index][0].ToString().Trim());
                index++;
            }
            btn_recover.Enabled = true;
        }
        else
        {
            btn_recover.Enabled = false;
        }
    }

    protected void btn_recover_Click(object sender, EventArgs e)
    {
        //By ryzhang 20110505 ITEM 29 DEL Start 
        /*string str_user = ddlist_user.Text.Trim();

        string str_passwordmd5 = FormsAuthentication.HashPasswordForStoringInConfigFile("123456", "MD5");
        string sql = "UPDATE [User] SET Password = '" + str_passwordmd5 + "' WHERE Alias = '" + str_user + "' AND Deleted = 0";
        int count = helper.ExecuteNonQuery(CommandType.Text, sql, null);

        if (count > 0)
        {
            lbl_note.ForeColor = System.Drawing.Color.Green;
            lbl_note.Text = "Succeed in recover default.";
        }
        else
        {
            lbl_note.ForeColor = System.Drawing.Color.Red;
            lbl_note.Text = "System error.";
        }
         */
        //By ryzhang 20110505 ITEM 29 DEL End 

        // By SJ 20110519 Item W6 Add Start
        lbl_error.Text = "";
        // By SJ 20110519 Item W6 Add End

        //By ryzhang 20110505 ITEM 29 ADD Start 
        string str_user = ddlist_user.Text.Trim();
        string str_pwd = this.txt_create_new_pwd.Text.Trim();

        // By SJ 20110519 Item W6 Add Start
        if (!string.IsNullOrEmpty(str_pwd))
        {
        // By SJ 20110519 Item W6 Add End
            string str_passwordmd5 = FormsAuthentication.HashPasswordForStoringInConfigFile(str_pwd, "MD5");
            string sql = "UPDATE [User] SET Password = '" + str_passwordmd5 + "' WHERE Alias = '" + str_user + "' AND Deleted = 0";
            int count = helper.ExecuteNonQuery(CommandType.Text, sql, null);

            if (count > 0)
            {
                lbl_note.ForeColor = System.Drawing.Color.Green;
                lbl_note.Text = "New password has been created successfully!.";
            }
            else
            {
                lbl_note.ForeColor = System.Drawing.Color.Red;
                lbl_note.Text = "System error.";
            }
            //By ryzhang 20110505 ITEM 29 ADD END 
        // By SJ 20110519 Item W6 Add Start
        }
        else 
        {
            lbl_note.Text = "New password can not be Null.";
        }
        // By SJ 20110519 Item W6 Add End
    }
}
