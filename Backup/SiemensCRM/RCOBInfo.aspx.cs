using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using AjaxPro;
using System.Text;

public partial class RCOBInfo : System.Web.UI.Page
{
    SQLHelper helper = new SQLHelper();

    #region Event
    protected void Page_Load(object sender, EventArgs e)
    {
        Utility.RegisterTypeForAjax(typeof(RCOBInfo));
    }
    #endregion

    #region Method
    [AjaxMethod]
    public string[] bindDataSource(string[] paramArr)
    {
        StringBuilder sql = new StringBuilder();
        sql.AppendLine(" SELECT ");
        sql.AppendLine("   Value, ");
        sql.AppendLine("   Percentage, ");
        sql.AppendLine("   Amount ");
        sql.AppendLine(" FROM ");
        sql.AppendLine("   Bookings ");
        sql.AppendLine(" WHERE ");
        sql.AppendLine("   RSMID=" + paramArr[0]);
        sql.AppendLine("   AND SalesOrgID=" + paramArr[1]);
        sql.AppendLine("   AND CountryID=" + paramArr[2]);
        sql.AppendLine("   AND CustomerID=" + paramArr[3]);
        sql.AppendLine("   AND BookingY='" + paramArr[4] + "' ");
        sql.AppendLine("   AND DeliverY='" + paramArr[5] + "' ");
        sql.AppendLine("   AND SegmentID=" + paramArr[6]);
        sql.AppendLine("   AND OperationID=" + paramArr[7]);
        sql.AppendLine("   AND ProjectID=" + paramArr[8]);
        sql.AppendLine("   AND SalesChannelID=" + paramArr[9]);
        sql.AppendLine("   AND YEAR(TimeFlag)=" + paramArr[10]);
        sql.AppendLine("   AND MONTH(TimeFlag)=" + paramArr[11]);
        sql.AppendLine("   AND ProductID=" + paramArr[12]);
        DataSet ds = helper.GetDataSet(sql.ToString());
        string[] valueArr = { "0", "0", "0" };
        if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        {
            //yyan itemw136 20110908 edit start 
            valueArr[0] = ds.Tables[0].Rows[0]["Value"].ToString();
            valueArr[1] = ds.Tables[0].Rows[0]["Percentage"].ToString();
            valueArr[2] = ds.Tables[0].Rows[0]["Amount"].ToString();
            //yyan itemw136 20110908 edit end 
        }
        return valueArr;
    }
    #endregion
}
