using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using System.Data;
using System.Data.SqlClient;
using System.IO;

public partial class SiemensCRMEnter : System.Web.UI.Page
{
    SQLHelper helper = new SQLHelper();

    protected void Page_Load(object sender, EventArgs e)
    {

    }

    private bool checkUser(string str_user, string str_password)
    {
        string sql = "SELECT UserID FROM [User] WHERE Alias = '" + str_user + "' AND Password = '" + str_password + "' AND Deleted = 0";
        DataSet ds = helper.GetDataSet(sql);

        if (ds.Tables[0].Rows.Count > 0)
            return true;
        else
            return false;
    }

    protected void btn_login_Click(object sender, EventArgs e)
    {
        string str_user = tbox_user.Text.Trim();
        string str_password = tbox_password.Text.Trim();
        bool b_check = ck_remember.Checked;

        string str_passwordmd5 = FormsAuthentication.HashPasswordForStoringInConfigFile(str_password, "MD5");
        if (checkUser(str_user, str_passwordmd5))
        {
            autoUnlock();
            destroyOldExcel();
            Session["Password"] = str_passwordmd5;
            Session["Alias"] = str_user;
            //by yyan item59 20110705 add start
            Session["MenuValue"] = "";
            //by yyan item59 20110705 add end
            Response.Redirect("~/SiemensCRMHome.aspx");
        }
        else
            lbl_error.Text = "User account or password is error, please check them again.";
    }

    /// <summary>
    /// Auto Unlock
    /// </summary>
    private void autoUnlock()
    {
        helper.ExecuteNonQuery(CommandType.Text, "DELETE FROM Lock WHERE Convert(VARCHAR(10),UnlockTime,23)<'" + DateTime.Now.ToString("yyyy-MM-dd") + "'", null);
        helper.ExecuteNonQuery(CommandType.Text, "DELETE FROM LockAllUser WHERE Convert(VARCHAR(10),UnlockTime,23)<'" + DateTime.Now.ToString("yyyy-MM-dd") + "'", null);
        helper.ExecuteNonQuery(CommandType.Text, "DELETE FROM LockSegment WHERE Convert(VARCHAR(10),UnlockTime,23)<'" + DateTime.Now.ToString("yyyy-MM-dd") + "'", null);
    }

    /// <summary>
    /// Destory Old Excel
    /// </summary>
    private void destroyOldExcel()
    {
        try
        {
            string path = Server.MapPath("~") + @"\ExcelReport\";
            string folderName = DateTime.Now.ToString("yyyyMMdd");
            DirectoryInfo root = new DirectoryInfo(path);
            DirectoryInfo[] nodes = root.GetDirectories();
            FileInfo[] files = null;
            for (int i = 0; i < nodes.Length; i++)
            {
                if (!string.Equals(nodes[i].Name, folderName))
                {
                    files = nodes[i].GetFiles();
                    for (int j = 0; j < files.Length; j++)
                    {
                        if (files[j].Attributes.ToString().IndexOf("ReadOnly") != -1)
                        {
                            files[j].Attributes = FileAttributes.Normal;
                        }
                        files[j].Delete();
                    }
                    nodes[i].Delete();
                }
            }
        }
        catch { }
    }
}
