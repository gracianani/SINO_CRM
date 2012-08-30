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
using System.Data.SqlClient;
using System.Text;
using System.Collections.Generic;

public partial class SelectReportCondiction : System.Web.UI.Page
{
    SQLStatement sql = new SQLStatement();
    SQLHelper helper = new SQLHelper();

    protected void Page_Load(object sender, EventArgs e)
    {

        if (!IsPostBack)
        {
            Session.Remove("check_value");
            string valueName = Request.QueryString["valueName"];
            textbindsession(valueName, valueName);
            sessionbindtext();
            bindDataSource();
            //Response.Write(this.Parent.Parent.FindControl("ctl00_ContentPlaceHolder1_clicktextid").ToString());
        }
    }


    protected void bindDataSource()
    {
        //TemplateField tf = new TemplateField();
        //tf.ItemTemplate = new gridviewte


        String fieldName ="OperationName";
        if (!string.IsNullOrEmpty(Request.QueryString["fieldName"]))
        {
            fieldName = Request.QueryString["fieldName"];
        }
        // Request.QueryString["customerID"].ToString().Trim();
        DataSet ds_list = getListInfo(fieldName);

        if (ds_list.Tables[0].Rows.Count == 0)
        {
            sql.getNullDataSet(ds_list);
        }

        gv_ReportCondiction.Width = Unit.Pixel(765);
        gv_ReportCondiction.AutoGenerateColumns = false;
        //gv_ReportCondiction.AllowPaging = true;
        gv_ReportCondiction.Visible = true;

        //BoundField bfcb = new BoundField();
        while (gv_ReportCondiction.Columns.Count > 1)
        {
            gv_ReportCondiction.Columns.RemoveAt(1);
        }
        for (int i = 0; i < ds_list.Tables[0].Columns.Count; i++)
        {
            BoundField bf = new BoundField();

            bf.DataField = ds_list.Tables[0].Columns[i].ColumnName.ToString();
            bf.HeaderText = ds_list.Tables[0].Columns[i].Caption.ToString();
            bf.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
            bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
            bf.ReadOnly = true;
            bf.ControlStyle.Width = bf.ItemStyle.Width;
            gv_ReportCondiction.Columns.Add(bf);
        }
        if (gv_ReportCondiction.Columns.Count > 1)
        {
            gv_ReportCondiction.Columns[1].Visible = false;
        }
        gv_ReportCondiction.DataSource = ds_list.Tables[0];
        gv_ReportCondiction.DataBind();
    }

    private void textbindsession(string id, string name)
    {
        string[] sid = id.Split(',');
        string[] sname = name.Split(',');
        for (int i = 0; i < sid.Length; i++)
        {
            bindSession(true, sid[i], sname[i]);
        }
    }

    //by yyan 20110520 ITEM w4 add start
    private string getUserID(string str_user, string str_roleName)
    {
        if (str_user != "" || str_user != null)
        {
            string sql = " SELECT UserID FROM [User],[Role] WHERE Alias='" + str_user + "'  " +
                " AND [Role].ID =[User].RoleID  AND [Role].Name ='" + str_roleName + "' AND Deleted = 0 ";
            DataSet ds = helper.GetDataSet(sql);

            if (ds.Tables[0].Rows.Count == 1)
                return ds.Tables[0].Rows[0][0].ToString().Trim();
        }
        return "";
    }
    //by yyan 20110520 ITEM w4 add end

    public DataSet getListInfo(string fieldName)
    {
        //by yyan 20110520 ITEM w4 add start
        string strRole = Session["Role"].ToString();
        string strAlias = Session["Alias"].ToString();
        string strUserID = getUserID(strAlias, strRole); ;
        //by yyan 20110520 ITEM w4 add end

        string sql = "";
        if (fieldName.Equals("OperationName"))
        {
            //by yyan 20110520 ITEM w4 del start
            //sql = "SELECT Name AS ShowValue, Name AS OperationName, Abbr As OperationAbbr FROM Operation WHERE  Name <> '' AND Deleted=0 ORDER BY ShowValue";
            //by yyan 20110520 ITEM w4 del end
            //by yyan 20110520 ITEM w4 add start
            if (strRole.Trim().Equals("Administrator"))
            {
                sql = "SELECT DISTINCT Name AS ShowValue, Name AS OperationName, Abbr As OperationAbbr FROM Operation WHERE  Name <> '' AND Deleted=0 ORDER BY ShowValue";
            }
            else
            {
                sql = "SELECT DISTINCT Name AS ShowValue, Name AS OperationName, Abbr As OperationAbbr FROM Operation WHERE  Name <> '' AND Deleted=0 AND " +
                     " Name IN (SELECT Operation.Name FROM Operation,User_Operation,[User] " +
                     " WHERE Operation.ID=User_Operation.OperationID AND User_Operation.Deleted=0 " +
                     " AND User_Operation.UserID =[USER].UserID AND [User].Deleted =0 " +
                     " AND [USER].UserID=" + strUserID + " )" +
                    "ORDER BY ShowValue";
            }
            //by yyan 20110520 ITEM w4 add end
        }
        else if (fieldName.Equals("OperationAbbr"))
        {
            //by yyan 20110520 ITEM w4 del start
            //sql = "SELECT Abbr AS ShowValue,  Abbr As OperationAbbrL FROM Operation WHERE Abbr <> '' AND Deleted=0 GROUP BY Abbr ORDER BY ShowValue";
            //by yyan 20110520 ITEM w4 del end
            //by yyan 20110520 ITEM w4 add start
            if (strRole.Trim().Equals("Administrator")) {
                sql = "SELECT Abbr AS ShowValue,Abbr As OperationAbbr FROM Operation WHERE Abbr <> '' AND Deleted=0 GROUP BY Abbr ORDER BY ShowValue";
            }
            else
            {
                sql = "SELECT Abbr AS ShowValue,Abbr As OperationAbbr FROM Operation WHERE Abbr <> '' AND Deleted=0 AND " +
                     " Abbr IN (select Operation.abbr FROM Operation,User_Operation,[User] " +
                     " WHERE Operation.ID=User_Operation.OperationID AND  User_Operation.Deleted=0 " +
                     " AND User_Operation.UserID =[USER].UserID AND [User].Deleted =0 " +
                     " AND [USER].UserID=" + strUserID + " )" +
                     " GROUP BY Abbr ORDER BY ShowValue";
            }
            //by yyan 20110520 ITEM w4 add end
        }
        else if (fieldName.Equals("OperationAbbrL") || fieldName.Equals("OperationAllocation"))
        {
            //by yyan 20110520 ITEM w4 del start
            //sql = "SELECT AbbrL AS ShowValue, AbbrL As OperationAbbrL FROM Operation WHERE AbbrL <> '' AND Deleted=0 GROUP BY AbbrL ORDER BY ShowValue";
            //by yyan 20110520 ITEM w4 del end
            //by yyan 20110520 ITEM w4 add start
            if (strRole.Trim().Equals("Administrator"))
            {
                sql = "SELECT AbbrL AS ShowValue, AbbrL As OperationAbbrL FROM Operation WHERE AbbrL <> '' AND Deleted=0 GROUP BY AbbrL ORDER BY ShowValue";
            }
            else
            {
                sql = "SELECT AbbrL AS ShowValue, AbbrL As OperationAbbrL FROM Operation WHERE AbbrL <> '' AND Deleted=0  AND " +
                    " AbbrL IN (SELECT Operation.AbbrL FROM Operation,User_Operation,[User] " +
                    " WHERE Operation.ID=User_Operation.OperationID AND User_Operation.Deleted=0 " +
                    " AND User_Operation.UserID =[USER].UserID AND [User].Deleted =0 " +
                    " AND [USER].UserID=" + strUserID + " )" +
                    " GROUP BY AbbrL ORDER BY ShowValue";
            }
            //by yyan 20110520 ITEM w4 add end
        }
        else if (fieldName.Equals("Comments"))
        {
            sql = "SELECT DISTINCT Cast(Comments AS nvarchar) AS ShowValue, Cast(Comments AS nvarchar) AS Comments FROM Bookings WHERE Cast(Comments AS nvarchar) <> '' ORDER BY ShowValue ";
        }
        else if (fieldName.Equals("BookingY"))
        {
            //by yyan 20110520 ITEM w4 del start
            //sql = "SELECT BookingY AS ShowValue, BookingY AS BookingY FROM Bookings WHERE BookingY <> '' GROUP BY BookingY ORDER BY ShowValue ";
            //by yyan 20110520 ITEM w4 del end
            //by yyan 20110520 ITEM w4 add start
            sql = "SELECT DISTINCT BookingY AS ShowValue, BookingY AS BookingY FROM Bookings WHERE BookingY <> '' GROUP BY BookingY ORDER BY ShowValue ";
            //by yyan 20110520 ITEM w4 add end
        }
        else if (fieldName.Equals("DeliverY"))
        {
            //by yyan 20110520 ITEM w4 del start
            //sql = "SELECT DeliverY AS ShowValue, DeliverY AS DeliverY FROM Bookings WHERE DeliverY <> '' GROUP BY DeliverY ORDER BY ShowValue ";
            //by yyan 20110520 ITEM w4 del end
            //by yyan 20110520 ITEM w4 add start
            sql = "SELECT DISTINCT DeliverY AS ShowValue, DeliverY AS DeliverY FROM Bookings WHERE DeliverY <> '' GROUP BY DeliverY ORDER BY ShowValue ";
            //by yyan 20110520 ITEM w4 add end
        }
        else if (fieldName.Equals("CountryName") || fieldName.Equals("Country") || fieldName.Equals("ProjectCountryPOD") || fieldName.Equals("ProjectCountryPOS"))
        {
            //by yyan 20110520 ITEM w4 del start
            //sql = "SELECT Name AS ShowValue, Name AS CountryName, ISO_Code FROM Country WHERE Name <> '' AND Deleted=0 ORDER BY ShowValue ";
            //by yyan 20110520 ITEM w4 del end
            //by yyan 20110520 ITEM w4 add start
            sql = "SELECT DISTINCT Name AS ShowValue, Name AS CountryName, ISO_Code FROM Country WHERE Name <> '' AND Deleted=0 ORDER BY ShowValue ";
            //by yyan 20110520 ITEM w4 add end
        }
        //by yyan 20110622 ITEM w43 del start
        //else if (fieldName.Equals("Currency"))
        //by yyan 20110622 ITEM w43 del end
        //by yyan 20110622 ITEM w43 add start
        else if (fieldName.Equals("Currency") || fieldName.Equals("ProjectCurrency"))
        //by yyan 20110622 ITEM w43 add end
        {
            //by yyan 20110520 ITEM w4 del start
            //sql = "SELECT Name AS ShowValue, Name AS Currency, Description FROM Currency WHERE Name <> '' AND Deleted=0 ORDER BY ShowValue ";
            //by yyan 20110520 ITEM w4 del end
            //by yyan 20110520 ITEM w4 add start
            sql = "SELECT DISTINCT Name AS ShowValue, Name AS Currency, Description FROM Currency WHERE Name <> '' AND Deleted=0 ORDER BY ShowValue ";
            //by yyan 20110520 ITEM w4 add end
        }
        //by yyan 20110622 ITEM w43 del start
        //else if (fieldName.Equals("CustomerName"))
        //by yyan 20110622 ITEM w43 del end
        //by yyan 20110622 ITEM w43 add start
        else if (fieldName.Equals("CustomerName") || fieldName.Equals("ProjectCustomer"))
        //by yyan 20110622 ITEM w43 add end
        {
            //by yyan 20110520 ITEM w4 del start
            //sql = "SELECT Name AS ShowValue, Name AS CustomerName FROM CustomerName WHERE Name <> '' AND Deleted=0 ORDER BY ShowValue ";
            //by yyan 20110520 ITEM w4 del end
            //by yyan 20110520 ITEM w4 add start
            sql = "SELECT DISTINCT Name AS ShowValue, Name AS CustomerName FROM CustomerName WHERE Name <> '' AND Deleted=0 ORDER BY ShowValue ";
            //by yyan 20110520 ITEM w4 add end
        }
        else if (fieldName.Equals("CustomerAddress"))
        {
            //by yyan 20110520 ITEM w4 del start
            //sql = "SELECT Cast(Address AS nvarchar) AS ShowValue, Cast(Address AS nvarchar) AS CustomerAddress FROM Customer WHERE Cast(Address AS nvarchar) <> '' AND Deleted=0 ORDER BY ShowValue ";
            //by yyan 20110520 ITEM w4 del end
            //by yyan 20110520 ITEM w4 add start
            sql = "SELECT DISTINCT Cast(Address AS nvarchar) AS ShowValue, Cast(Address AS nvarchar) AS CustomerAddress FROM Customer WHERE Cast(Address AS nvarchar) <> '' AND Deleted=0 ORDER BY ShowValue ";
            //by yyan 20110520 ITEM w4 add end
        }
        else if (fieldName.Equals("City"))
        {
            //by yyan 20110520 ITEM w4 del start
            //sql = "SELECT City AS ShowValue, City  FROM Customer WHERE City <> '' AND Deleted=0 ORDER BY ShowValue ";
            //by yyan 20110520 ITEM w4 del end
            //by yyan 20110520 ITEM w4 add start
            sql = "SELECT DISTINCT City AS ShowValue, City  FROM Customer WHERE City <> '' AND Deleted=0 ORDER BY ShowValue ";
            //by yyan 20110520 ITEM w4 add end
        }
        //by yyan 20110622 ITEM w43 del start
        //else if (fieldName.Equals("SegmentAbbr"))
        //by yyan 20110622 ITEM w43 del end
        //by yyan 20110622 ITEM w43 add start
        else if (fieldName.Equals("SegmentAbbr") || fieldName.Equals("ProjectSegment"))
        //by yyan 20110622 ITEM w43 add end
        {
            //by yyan 20110520 ITEM w4 del start
            //sql = "SELECT Abbr AS ShowValue, Abbr AS SegmentAbbr  FROM Segment WHERE Abbr <> '' AND Deleted=0 ORDER BY ShowValue ";
            //by yyan 20110520 ITEM w4 del end
            //by yyan 20110520 ITEM w4 add start
            if (strRole.Trim().Equals("Administrator"))
            {
                sql = "SELECT DISTINCT Abbr AS ShowValue, Abbr AS SegmentAbbr  FROM Segment WHERE Abbr <> '' AND Deleted=0 ORDER BY ShowValue ";
            }else{
                sql = "SELECT DISTINCT Abbr AS ShowValue, Abbr AS SegmentAbbr  FROM Segment WHERE Abbr <> '' AND Deleted=0 " +
                      " AND Abbr in (select Segment.Abbr FROM Segment,[user],[User_segment] " +
                      " WHERE Segment.ID=User_Segment.SegmentID AND segment.Deleted=0 " +
                      " AND User_Segment.Deleted=0 AND [USER].UserID =User_Segment.UserID " +
                      " AND [user].Deleted=0 AND [user].UserID= " + strUserID + " )" +
                      " ORDER BY ShowValue ";
            } 
            //by yyan 20110520 ITEM w4 add end
        }
        else if (fieldName.Equals("SegmentDescription"))
        {
            //by yyan 20110520 ITEM w4 del start
            //sql = "SELECT Description AS ShowValue, Description AS SegmentDescription  FROM Segment WHERE Description <> '' AND Deleted=0 ORDER BY ShowValue ";
            //by yyan 20110520 ITEM w4 del end
            //by yyan 20110520 ITEM w4 add start
            if (strRole.Trim().Equals("Administrator"))
            { sql = "SELECT DISTINCT Description AS ShowValue, Description AS SegmentDescription  FROM Segment WHERE Description <> '' AND Deleted=0 ORDER BY ShowValue "; }
            else
            {
                sql = "SELECT DISTINCT Description AS ShowValue, Description AS SegmentDescription  FROM Segment WHERE Description <> '' AND Deleted=0 " +
                      " AND Description in (select Segment.Description FROM Segment,[user],[User_segment] " +
                      " WHERE Segment.ID=User_Segment.SegmentID AND segment.Deleted=0 " +
                      " AND User_Segment.Deleted=0 AND [USER].UserID =User_Segment.UserID " +
                      " AND [user].Deleted=0 AND [user].UserID= " + strUserID + " )" +
                      " ORDER BY ShowValue ";
            }
            //by yyan 20110520 ITEM w4 add end
        }
        //by yyan 20110520 ITEM w4 del start
        //else if (fieldName.Equals("Alias"))
        //by yyan 20110520 ITEM w4 del end
        //by yyan 20110520 ITEM w4 add start
        else if (fieldName.Equals("UserLogin"))
        //by yyan 20110520 ITEM w4 add end
        {
            //by yyan 20110915 itemw145 edit start
            sql = "SELECT DISTINCT Alias AS ShowValue,  Alias ,Abbr AS UserAbbr FROM [User] WHERE Alias <> '' AND Deleted=0 ORDER BY ShowValue ";
            //by yyan 20110915 itemw145 edit end
        }
        else if (fieldName.Equals("UserAbbr") || fieldName.Equals("Staff"))
        {
            sql = "SELECT Abbr AS ShowValue ,Abbr AS UserAbbr FROM [User] WHERE Abbr <> '' AND Deleted=0 GROUP BY Abbr ORDER BY ShowValue ";
        }
        else if (fieldName.Equals("Email"))
        {
            sql = "SELECT Email AS ShowValue,  Email AS Email  FROM [User] WHERE Email <> '' AND Deleted=0 Group by Email ORDER BY ShowValue ";
        }
        else if (fieldName.Equals("ProductDescription"))
        {
            //by yyan 20110520 ITEM w4 del start
            //sql = "SELECT Description AS ShowValue,  Description AS ProductDescription  FROM Product WHERE Description <> '' AND Deleted=0 ORDER BY ShowValue ";
            //by yyan 20110520 ITEM w4 del end
            //by yyan 20110520 ITEM w4 add start
            if (strRole.Trim().Equals("Administrator"))
            {
                sql = "SELECT DISTINCT Description AS ShowValue,  Description AS ProductDescription  FROM Product WHERE Description <> '' AND Deleted=0 " +
                      " ORDER BY ShowValue ";
            }
            else
            {
                sql = "SELECT DISTINCT Description AS ShowValue,  Description AS ProductDescription  FROM Product WHERE Description <> '' AND Deleted=0 " +
                      " AND Description IN (SELECT Product.Description FROM Product,Segment,Segment_Product " +
                      " WHERE Product.Deleted =0 AND Product.ID= Segment_Product.ProductID and " +
                      " Segment_Product.Deleted =0 AND Segment_Product.SegmentID =Segment.ID " +
                      " AND Segment.Deleted =0 AND Segment.ID in ( " +
                      " select Segment.ID FROM Segment,[user],[User_segment] " +
                      " WHERE Segment.ID=User_Segment.SegmentID AND segment.Deleted=0 " +
                      " AND User_Segment.Deleted=0 AND [USER].UserID =User_Segment.UserID " +
                      " AND [user].Deleted=0 AND [user].UserID=  " + strUserID + " ))" +
                      " ORDER BY ShowValue ";
            }
            //by yyan 20110520 ITEM w4 add end
        }
        else if (fieldName.Equals("ProductAbbr"))
        {
            //by yyan 20110520 ITEM w4 del start
            //sql = "SELECT Abbr AS ShowValue,  Abbr AS ProductAbbr  FROM Product WHERE Abbr <> '' AND Deleted=0 ORDER BY ShowValue ";
            //by yyan 20110520 ITEM w4 del end
            //by yyan 20110520 ITEM w4 add start
            if (strRole.Trim().Equals("Administrator"))
            {
                sql = "SELECT DISTINCT Abbr AS ShowValue,  Abbr AS ProductAbbr  FROM Product WHERE Abbr <> '' AND Deleted=0 ORDER BY ShowValue ";
            }
            else
            {
                sql = " SELECT DISTINCT Abbr AS ShowValue,  Abbr AS ProductAbbr  FROM Product WHERE Abbr <> '' AND Deleted=0 " +
                      " AND Abbr IN (SELECT Product.Abbr FROM Product,Segment,Segment_Product " +
                      " WHERE Product.Deleted =0 AND Product.ID= Segment_Product.ProductID and " +
                      " Segment_Product.Deleted =0 AND Segment_Product.SegmentID =Segment.ID " +
                      " AND Segment.Deleted =0 AND Segment.ID in ( " +
                      " select Segment.ID FROM Segment,[user],[User_segment] " +
                      " WHERE Segment.ID=User_Segment.SegmentID AND segment.Deleted=0 " +
                      " AND User_Segment.Deleted=0 AND [USER].UserID =User_Segment.UserID " +
                      " AND [user].Deleted=0 AND [user].UserID=  " + strUserID + " ))" +
                      " ORDER BY ShowValue ";
            }
            //by yyan 20110520 ITEM w4 add end
        }
        else if (fieldName.Equals("ProjectName"))
        {
            //by yyan 20110520 ITEM w4 del start
            //sql = "SELECT Name AS ShowValue,  Name AS ProjectName  FROM Project WHERE Name <> '' AND Deleted=0 ORDER BY ShowValue ";
            //by yyan 20110520 ITEM w4 del end
            //by yyan 20110520 ITEM w4 add start
            sql = "SELECT DISTINCT Name AS ShowValue,  Name AS ProjectName  FROM Project WHERE Name <> '' AND Deleted=0 ORDER BY ShowValue ";
            //by yyan 20110520 ITEM w4 add end
        }
        else if (fieldName.Equals("CustomerCountryName"))
        {
            //by yyan 20110520 ITEM w4 del start
            //sql = "SELECT Name AS ShowValue, Name AS CountryName, ISO_Code FROM Country WHERE Name <> '' AND Deleted=0 ORDER BY ShowValue ";
            //by yyan 20110520 ITEM w4 del end
            //by yyan 20110520 ITEM w4 add start
            sql = "SELECT DISTINCT Name AS ShowValue, Name AS CountryName, ISO_Code FROM Country WHERE Name <> '' AND Deleted=0 ORDER BY ShowValue ";
            //by yyan 20110520 ITEM w4 add end
        }
        else if (fieldName.Equals("CustomerTypeName"))
        {
            //by yyan 20110520 ITEM w4 del start
            //sql = "SELECT Name AS ShowValue, Name AS CustomerTypeName  FROM CustomerType WHERE Name <> '' AND Deleted=0 ORDER BY ShowValue ";
            //by yyan 20110520 ITEM w4 del end
            //by yyan 20110520 ITEM w4 add start
            sql = "SELECT DISTINCT Name AS ShowValue, Name AS CustomerTypeName  FROM CustomerType WHERE Name <> '' AND Deleted=0 ORDER BY ShowValue ";
            //by yyan 20110520 ITEM w4 add end
        }
        else if (fieldName.Equals("Department"))
        {
            //by yyan 20110520 ITEM w4 del start
            //sql = "SELECT Department AS ShowValue, Department  FROM Customer WHERE Department <> '' AND Deleted=0 ORDER BY ShowValue";
            //by yyan 20110520 ITEM w4 del end
            //by yyan 20110520 ITEM w4 add start
            sql = "SELECT DISTINCT Department AS ShowValue, Department  FROM Customer WHERE Department <> '' AND Deleted=0 ORDER BY ShowValue";
            //by yyan 20110520 ITEM w4 add end
        }
        else if (fieldName.Equals("RoleName"))
        {
            //by yyan 20110520 ITEM w4 del start
            //sql = "SELECT Name AS ShowValue, Name AS RoleName  FROM Role WHERE Name <> '' ORDER BY ShowValue";
            //by yyan 20110520 ITEM w4 del end
            //by yyan 20110520 ITEM w4 add start
            sql = "SELECT DISTINCT Name AS ShowValue, Name AS RoleName  FROM Role WHERE Name <> '' ORDER BY ShowValue";
            //by yyan 20110520 ITEM w4 add end
        }
        else if (fieldName.Equals("ClusterName") || fieldName.Equals("Cluster"))
        {
            //by yyan 20110520 ITEM w4 del start
            //sql = "SELECT Name AS ShowValue, Name AS ClusterName  FROM Cluster WHERE Name <> '' and Deleted=0 ORDER BY ShowValue";
            //by yyan 20110520 ITEM w4 del end
            //by yyan 20110520 ITEM w4 add start
            sql = "SELECT DISTINCT Name AS ShowValue, Name AS ClusterName  FROM Cluster WHERE Name <> '' and Deleted=0 ORDER BY ShowValue";
            //by yyan 20110520 ITEM w4 add end
        }
        else if (fieldName.Equals("RegionName") || fieldName.Equals("Region"))
        {
            //by yyan 20110520 ITEM w4 del start
            //sql = "SELECT Name AS ShowValue, Name AS RegionName  FROM Region WHERE Name <> '' and Deleted=0 ORDER BY ShowValue";
            //by yyan 20110520 ITEM w4 del end
            //by yyan 20110520 ITEM w4 add start
            sql = "SELECT DISTINCT Name AS ShowValue, Name AS RegionName  FROM Region WHERE Name <> '' and Deleted=0 ORDER BY ShowValue";
            //by yyan 20110520 ITEM w4 add end
        }
        else if (fieldName.Equals("SalesOrgName") || fieldName.Equals("SalesOrganization"))
        {
            //by yyan 20110520 ITEM w4 del start
            //sql = "SELECT Name AS ShowValue, Name AS SalesOrgName, Abbr As SalesOrgNameAbbr  FROM SalesOrg WHERE Name <> '' and Deleted=0 ORDER BY ShowValue";
            //by yyan 20110520 ITEM w4 del end
            //by yyan 20110520 ITEM w4 add start
            if (strRole.Trim().Equals("Administrator"))
            {
                sql = "SELECT DISTINCT Name AS ShowValue, Name AS SalesOrgName, Abbr As SalesOrgNameAbbr  FROM SalesOrg WHERE Name <> '' " +
                      " and Deleted=0 ORDER BY ShowValue";
            }
            else
            {
                sql = "SELECT DISTINCT Name AS ShowValue, Name AS SalesOrgName, Abbr As SalesOrgNameAbbr  FROM SalesOrg WHERE Name <> '' and Deleted=0 " +
                      " AND Name IN (SELECT SalesOrg.Name FROM SalesOrg,[User],SalesOrg_User " +
                      " WHERE SalesOrg.Deleted=0 AND SalesOrg.ID = SalesOrg_User.SalesOrgID " +
                      " AND SalesOrg_User.Deleted =0 AND SalesOrg_User.UserID=[USER].UserID " +
                      " AND [USER].Deleted=0 AND [USER].UserID=" + strUserID + " )" +
                      " ORDER BY ShowValue";
            }
            //by yyan 20110520 ITEM w4 add end
        }
        else if (fieldName.Equals("SalesOrgNameAbbr") || fieldName.Equals("SaleOrgAbbr") || fieldName.Equals("SalesOrgAbbr"))
        {
            //by yyan 20110520 ITEM w4 del start
            //sql = "SELECT Abbr AS ShowValue, Name AS SalesOrgName, Abbr As SalesOrgNameAbbr  FROM SalesOrg WHERE Name <> '' and Deleted=0 ORDER BY ShowValue";
            //by yyan 20110520 ITEM w4 del end
            //by yyan 20110520 ITEM w4 add start
            if (strRole.Trim().Equals("Administrator"))
            { 
                sql = "SELECT DISTINCT Abbr AS ShowValue, Name AS SalesOrgName, Abbr As SalesOrgNameAbbr  FROM SalesOrg WHERE Name <> '' and Deleted=0 ORDER BY ShowValue"; 
            }
            else
            {
                sql = "SELECT DISTINCT Abbr AS ShowValue, Name AS SalesOrgName, Abbr As SalesOrgNameAbbr FROM SalesOrg WHERE Name <> '' and Deleted=0 " +
                      " AND Abbr IN (SELECT SalesOrg.Abbr FROM SalesOrg,[User],SalesOrg_User " +
                      " WHERE SalesOrg.Deleted=0 AND SalesOrg.ID = SalesOrg_User.SalesOrgID " +
                      " AND SalesOrg_User.Deleted =0 AND SalesOrg_User.UserID=[USER].UserID " +
                      " AND [USER].Deleted=0 AND [USER].UserID=" + strUserID + " )" +
                      " ORDER BY ShowValue";
            }
            //by yyan 20110520 ITEM w4 add end
        }
        else if (fieldName.Equals("Gender"))
        {
            sql = "SELECT Case Gender when '0' then 'F' when '1' then 'M' end AS ShowValue, Case Gender when '0' then 'F' when '1' then 'M' end  AS Gender FROM [User] WHERE Deleted=0 GROUP BY Gender ORDER BY ShowValue";
        }
        // add by zy 20110130 start
        else if (fieldName.Equals("DeliveryInFY"))
        {
            //by yyan 20110520 ITEM w4 del start
            //sql = "SELECT [Delivery in FY] AS ShowValue, [Delivery in FY] FROM bookings WHERE [Delivery in FY] IS NOT NULL GROUP BY [Delivery in FY] ORDER BY ShowValue ";
            //by yyan 20110520 ITEM w4 del end
            //by yyan 20110520 ITEM w4 add start
            sql = "SELECT DISTINCT [Delivery in FY] AS ShowValue, [Delivery in FY] FROM bookings WHERE [Delivery in FY] IS NOT NULL GROUP BY [Delivery in FY] ORDER BY ShowValue ";
            //by yyan 20110520 ITEM w4 add end
        }
        else if (fieldName.Equals("NoInFY"))
        {
            //by yyan 20110520 ITEM w4 del start
            //sql = "SELECT [NO in FY] AS ShowValue, [NO in FY] FROM bookings WHERE [NO in FY] IS NOT NULL GROUP BY [NO in FY] ORDER BY ShowValue";
            //by yyan 20110520 ITEM w4 del end
            //by yyan 20110520 ITEM w4 add start
            sql = "SELECT DISTINCT [NO in FY] AS ShowValue, [NO in FY] FROM bookings WHERE [NO in FY] IS NOT NULL GROUP BY [NO in FY] ORDER BY ShowValue";
            //by yyan 20110520 ITEM w4 add end
        }
        //by yyan 20110520 ITEM w4  add start
        else if (fieldName.Equals("FirstName"))
        {
            //by yyan 20110520 ITEM w4 del start
            //sql = "SELECT [FirstName] AS ShowValue,[FirstName] AS FirstName FROM [User] WHERE FirstName <> '' ORDER BY FirstName";
            //by yyan 20110520 ITEM w4 del end
            //by yyan 20110520 ITEM w4 add start
            sql = "SELECT DISTINCT [FirstName] AS ShowValue,[FirstName] AS FirstName FROM [User] WHERE FirstName <> '' ORDER BY FirstName";
            //by yyan 20110520 ITEM w4 add end
        }
        else if (fieldName.Equals("LastName"))
        {
            //by yyan 20110520 ITEM w4 del start
            //sql = "SELECT [LastName] AS ShowValue,[LastName] AS LastName FROM [User] WHERE LastName <> '' ORDER BY LastName";
            //by yyan 20110520 ITEM w4 del end
            //by yyan 20110520 ITEM w4 add start
            sql = "SELECT DISTINCT [LastName] AS ShowValue,[LastName] AS LastName FROM [User] WHERE LastName <> '' ORDER BY LastName";
            //by yyan 20110520 ITEM w4 add end
        }
        //by yyan 20110520 ITEM w43 del start
        //else if (fieldName.Equals("SubRegion"))
        //by yyan 20110520 ITEM w43 del end
        //by yyan 20110520 ITEM w43 add start
        //else if (fieldName.Equals("SubRegion") || fieldName.Equals("ProjectCountryPOD") || fieldName.Equals("ProjectCountryPOS"))
        else if (fieldName.Equals("SubRegion") )
        //by yyan 20110520 ITEM w43 add end
        {
            //by yyan 20110520 ITEM w4 del start
            //sql = "SELECT [Name] AS ShowValue,[Name] AS SubRegion FROM [SubRegion] WHERE Name <> '' ORDER BY Name";
            //by yyan 20110520 ITEM w4 del end
            //by yyan 20110520 ITEM w4 add start
            sql = "SELECT DISTINCT [Name] AS ShowValue,[Name] AS SubRegion FROM [SubRegion] WHERE Name <> '' ORDER BY Name";
            //by yyan 20110520 ITEM w4 add end
        }
        //else if (fieldName.Equals("ProjectCountryPOS"))
        //{
        //    //by yyan 20110520 ITEM w4 del start
        //    //sql = "SELECT [Name] AS ShowValue,[Name] AS ProjectCountryPOS FROM [SubRegion] WHERE Name <> '' ORDER BY Name";
        //    //by yyan 20110520 ITEM w4 del end
        //    //by yyan 20110520 ITEM w4 add start
        //    sql = "SELECT DISTINCT [Name] AS ShowValue,[Name] AS ProjectCountryPOS FROM [SubRegion] WHERE Name <> '' ORDER BY Name";
        //    //by yyan 20110520 ITEM w4 add end
        //}
        //else if (fieldName.Equals("ProjectCountryPOD"))
        //{
        //    //by yyan 20110520 ITEM w4 del start
        //    //sql = "SELECT [Name] AS ShowValue,[Name] AS ProjectCountryPOD FROM [SubRegion] WHERE Name <> '' ORDER BY Name";
        //    //by yyan 20110520 ITEM w4 del end
        //    //by yyan 20110520 ITEM w4 add start
        //    sql = "SELECT DISTINCT [Name] AS ShowValue,[Name] AS ProjectCountryPOD FROM [SubRegion] WHERE Name <> '' ORDER BY Name";
        //    //by yyan 20110520 ITEM w4 add end
        //}
        else if (fieldName.Equals("Segment"))
        {
            //by yyan 20110520 ITEM w4 del start
            //sql = "SELECT [Abbr] AS ShowValue,[Abbr] AS Segment FROM [Segment] WHERE Abbr <> '' ORDER BY Abbr ";
            //by yyan 20110520 ITEM w4 del end
            //by yyan 20110520 ITEM w4 add start
            if (strRole.Trim().Equals("Administrator"))
            { 
                sql = "SELECT DISTINCT [Abbr] AS ShowValue,[Abbr] AS Segment FROM [Segment] WHERE Abbr <> '' ORDER BY Abbr ";
            }
            else {
                sql = "SELECT DISTINCT [Abbr] AS ShowValue,[Abbr] AS Segment FROM [Segment] WHERE Abbr <> '' " +
                      " AND Abbr IN (SELECT Segment.Abbr FROM Segment,[user],[User_segment] " +
                      " WHERE Segment.ID=User_Segment.SegmentID AND segment.Deleted=0 " +
                      " AND User_Segment.Deleted=0 AND [USER].UserID =User_Segment.UserID " +
                      " AND [user].Deleted=0 AND [user].UserID= " + strUserID + " )" +
                      " ORDER BY Abbr ";
            }
            //by yyan 20110520 ITEM w4 add end
        }
        else if (fieldName.Equals("Address"))
        {
            //by yyan 20110520 ITEM w4 del start
            //sql = "SELECT cast([Address] as varchar) AS ShowValue,cast([Address] as varchar) AS Address FROM [Customer]  WHERE cast([Address] as varchar) <> '' ORDER BY Address  ";
            //by yyan 20110520 ITEM w4 del end
            //by yyan 20110520 ITEM w4 add start
            sql = "SELECT DISTINCT cast([Address] as varchar) AS ShowValue,cast([Address] as varchar) AS Address FROM [Customer]  WHERE cast([Address] as varchar) <> '' ORDER BY Address  ";
            //by yyan 20110520 ITEM w4 add end
        }
        else if (fieldName.Equals("SalesChannel"))
        {
            //by yyan 20110520 ITEM w4 del start
            //sql = "SELECT [Name] AS ShowValue,[Name] AS SalesChannel FROM [SalesChannel] WHERE Name  <> '' ORDER BY Name  ";
            //by yyan 20110520 ITEM w4 del end
            //by yyan 20110520 ITEM w4 add start
            sql = "SELECT DISTINCT [Name] AS ShowValue,[Name] AS SalesChannel FROM [SalesChannel] WHERE Name  <> '' ORDER BY Name  ";
            //by yyan 20110520 ITEM w4 add end
        }

        else if (fieldName.Equals("CustomerType"))
        {
            //by yyan 20110520 ITEM w4 del start
            //sql = "SELECT [Name] AS ShowValue,[Name] AS CustomerType FROM [CustomerType] WHERE Name  <> '' ORDER BY Name  ";
            //by yyan 20110520 ITEM w4 del end
            //by yyan 20110520 ITEM w4 add start
            sql = "SELECT DISTINCT [Name] AS ShowValue,[Name] AS CustomerType FROM [CustomerType] WHERE Name  <> '' ORDER BY Name  ";
            //by yyan 20110520 ITEM w4 add end
        }
        //by yyan 20110520 ITEM w4 add end
        // add by zy 20110130 end
        //by yyan 20110524 ITEM w10 add start
        else if (fieldName.Equals("ISO_Code"))
        {
            sql = "SELECT DISTINCT [ISO_Code] AS ShowValue,[ISO_Code] AS ISO_Code FROM [Country] WHERE ISO_Code  <> '' ORDER BY ISO_Code  ";
        }
        //by yyan 20110524 ITEM w10 add end

        //by yyan 20110621 ITEM w39 add start
        else if (fieldName.Equals("Operation"))
        {
           
            if (strRole.Trim().Equals("Administrator"))
            {
                sql = "SELECT AbbrL AS ShowValue, AbbrL As OperationAbbrL FROM Operation WHERE AbbrL <> '' AND Deleted=0 GROUP BY AbbrL ORDER BY ShowValue";
            }
            else
            {
                sql = "SELECT AbbrL AS ShowValue, AbbrL As OperationAbbrL FROM Operation WHERE AbbrL <> '' AND Deleted=0  AND " +
                    " AbbrL IN (SELECT Operation.AbbrL FROM Operation,User_Operation,[User] " +
                    " WHERE Operation.ID=User_Operation.OperationID AND User_Operation.Deleted=0 " +
                    " AND User_Operation.UserID =[USER].UserID AND [User].Deleted =0 " +
                    " AND [USER].UserID=" + strUserID + " )" +
                    " GROUP BY AbbrL ORDER BY ShowValue";
            }
        }
        else if (fieldName.Equals("Role"))
        {
            sql = "SELECT Name AS ShowValue ,Name AS Role FROM [Role] ";
        }
        //by yyan 20110524 ITEM w39 add end
        //by yyan 20110622 ITEM w43 add start
        else if (fieldName.Equals("ProjectComments"))
        {
            sql = "SELECT DISTINCT Cast(Comments AS nvarchar) AS ShowValue, Cast(Comments AS nvarchar) AS Comments FROM Project WHERE Cast(Comments AS nvarchar) <> '' ORDER BY ShowValue ";
        }
        //by yyan 20110622 ITEM w43 add end
        else if (fieldName.Equals("BacklogComments"))
        {
            sql = "SELECT DISTINCT Cast(Comments AS nvarchar) AS ShowValue, Cast(Comments AS nvarchar) AS Comments FROM ActualSalesandBL WHERE Cast(Comments AS nvarchar) <> '' ORDER BY ShowValue ";
        }
        else if (fieldName.Equals("BacklogYear"))
        {
            sql = "SELECT DISTINCT BacklogY AS ShowValue, BacklogY AS BacklogYear FROM ActualSalesandBL WHERE BacklogY <> '' ORDER BY ShowValue ";
        }
        DataSet ds_list = helper.GetDataSet(sql);

        return ds_list;
    }

    protected void chkCheck_CheckedChanged(object sender, EventArgs e)
    {
        foreach (GridViewRow row in gv_ReportCondiction.Rows)
        {

            CheckBox check = (CheckBox)row.FindControl("chkCheck");
            HiddenField hf = (HiddenField)row.FindControl("hdShowValue");
            
            bindSession(check.Checked, hf.Value.Trim(), row.Cells[2].Text.Trim());
        }
        sessionbindtext();
    }

    private void bindSession(bool bchecked, string id, string name)
    {
        if (id.Length > 0)
        {
            if (!id.Substring(0, 1).Equals("'"))
            {
                id = "'" + id + "'";
            }
        }
        Dictionary<string, string> parameters;
        if (Session["check_value"] != null)
        {
            
            parameters = (Dictionary<string, string>)Session["check_value"];
            if (parameters == null)
            {
                parameters = new Dictionary<string, string>();
            }
        }
        else
        {
            parameters = new Dictionary<string, string>();
        }
        if (bchecked)
        {
            bool falg = parameters.ContainsKey(id);
            if (!falg)
            {
                parameters.Add(id, name);
            }
        }
        else
        {
            bool falg = parameters.ContainsKey(id);
            if (falg)
            {
                parameters.Remove(id);
            }
        }
        Session.Add("check_value", parameters);
         
    }

    private void sessionbindtext()
    {
        string showValue = "";
        getSessionValue(out showValue);
        sendto.Value = showValue;
    }

    private void getSessionValue(out string name)
    {
        Dictionary<string, string> parameters = (Dictionary<string, string>)Session["check_value"];
        name = "";
        StringBuilder tmpname = new StringBuilder();
        if (parameters != null)
        {
            foreach (KeyValuePair<string, string>paritme in parameters)
            {
               // tmpid.Append("," + paritme.Key);
                if (tmpname.Length > 0)
                {
                    tmpname.Append("," + paritme.Key);
                }
                else
                {
                    tmpname.Append(paritme.Key);                
                }
            }
            if (tmpname.Length > 0)
            {
                name = tmpname.ToString();
                //name = name.Replace(",", " ");
                //name = name.Trim().Replace(" ", ",");
            }
        }
    }


    protected void gv_ReportCondiction_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        #region set page no
        this.gv_ReportCondiction.PageIndex = e.NewPageIndex;
        bindDataSource();
        #endregion
    }
    protected void gv_ReportCondiction_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        #region set multi button
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            CheckBox check = (CheckBox)e.Row.FindControl("chkCheck");
            if (check != null)
            {
                string shouValue;
                getSessionValue(out shouValue);
                string curValue = "'"+ ((DataRowView)e.Row.DataItem)["ShowValue"].ToString().Trim()+"'";
                check.Checked = isSame(shouValue.Trim(), curValue.Trim());
            }
        }
        #endregion
    }
    private bool isSame(string id, string uid)
    {
        string[] lid = id.Split(',');
        foreach (string tid in lid)
        {
            if (tid == uid)
            {
                return true;
            }
        }
        return false;
    }
}
