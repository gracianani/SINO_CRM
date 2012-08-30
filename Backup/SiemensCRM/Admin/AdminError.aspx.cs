/*
* File Name   : AdminError.aspx.cs
* 
* Description : Display error information
* 
* Author      : Wang Jun
* 
* Modify Date : 2010.1.29
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

public partial class Admin_AdminError : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        label_error.Text = Request.QueryString["errorInfo"];
    }
}
