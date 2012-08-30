using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class RSM_RSMError : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        label_error.Text = Request.QueryString["errorInfo"];
    }
}
