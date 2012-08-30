/*++

Module Name:

    AccessDenied.aspx.cs

Abstract:
    
    When the user does not have permission to access this site, this page will be displayed.

Author:

    Longran Wei 20-January-2010


Revision History:

--*/

using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

public partial class AccessDenied : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        label_display.Text = ConfigurationSettings.AppSettings["emailadd"].ToString(); 
    }
}
