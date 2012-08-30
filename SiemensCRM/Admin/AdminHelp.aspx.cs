/*
 * File Name   : AdminHelp.aspx.cs
 * 
 * Description : help to how to use finding fuction
 * 
 * Author      : Wang Jun
 * 
 * Modify Date : 2010-04-09
 * 
 * Problem     : none
 *  
 * Version     : Release (1.0)
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Admin_AdminHelp : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void btn_close_Click(object sender, EventArgs e)
    {
        Response.Write("<script language=javascript> window.opener.window.document.forms(0).submit();window.close();</script>");
    }
}
