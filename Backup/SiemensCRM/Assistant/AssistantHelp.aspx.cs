using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Assistant_AssistantHelp : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void btn_close_Click(object sender, EventArgs e)
    {
        Response.Write("<script language=javascript> window.opener.window.document.forms(0).submit();window.close();</script>");
    }
}
