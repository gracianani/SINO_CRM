/*++

Module Name:

    Error.aspx.cs

Abstract:
    
    If the page throws an exception, this page will be shown.

Author:

    Wang Jun 04-26-2010

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
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;
using System.Collections.Generic;

public partial class Error : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        label_note.Text = Request.QueryString["error"].ToString();
    }
}
