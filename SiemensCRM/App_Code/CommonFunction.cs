using System.Data;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

/// <summary>
/// basic common functions
/// </summary>
public class CommonFunction
{
    private SQLHelper helper = new SQLHelper();

    /// <summary>
    /// export to excel file
    /// </summary>
    /// <param name="ctl">control need to export</param>
    /// <param name="FileName">exported file name</param>
    public void ToExcel(Control ctl, string FileName)
    {
        HttpContext.Current.Response.Charset = "UTF-8";
        HttpContext.Current.Response.ContentEncoding = Encoding.UTF8;
        HttpContext.Current.Response.ContentType = "application/ms-excel";
        HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + "" + FileName);
        ctl.Page.EnableViewState = false;
        var tw = new StringWriter();
        var hw = new HtmlTextWriter(tw);
        ctl.RenderControl(hw);
        HttpContext.Current.Response.Write(tw.ToString());
        HttpContext.Current.Response.End();
    }

    /// <summary>
    /// bind DropDownList control
    /// </summary>
    /// <param name="ddl">DropDownList control need to be bound</param>
    /// <param name="ds">data source</param>
    /// <param name="flag">bind style. true for binding text and value; false for binding text only.</param>
    public void bindDropDownList(DropDownList ddl, DataSet ds, bool flag)
    {
        if (ds.Tables[0].Rows.Count > 0)
        {
            DataTable dt = ds.Tables[0];
            int count = dt.Rows.Count;
            int index = 0;
            while (index < count)
            {
                if (flag)
                {
                    var li = new ListItem(dt.Rows[index][0].ToString().Trim(), dt.Rows[index][1].ToString().Trim());
                    ddl.Items.Add(li);
                }
                else
                    ddl.Items.Add(dt.Rows[index][0].ToString().Trim());
                index++;
            }
            ddl.Enabled = true;
        }
        else
        {
            if (flag)
            {
                var li = new ListItem("Not Exist", "-1");
                ddl.Items.Add(li);
            }
            else
                ddl.Items.Add("Not Exist");
            ddl.Enabled = false;
        }
    }

    public void unlockdata(string rsmID)
    {
    }

    public void unlockdataSales(string salesID)
    {
    }


    public bool IsReadOnly(string rsmID)
    {
        return false;
    }

    public bool GetLockUser(string rsmID)
    {
        return false;
    }
}