using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Text;

public partial class Admin_AdminUploadBookingSalesData : System.Web.UI.Page
{
    #region Globle Variable
    SQLHelper helper = new SQLHelper();
    SQLStatement sql = new SQLStatement();
    LogUtility log = new LogUtility();
    ExcelHandler excelHandler = new ExcelHandler();
    #endregion

    #region Event

    protected void Page_Load(object sender, EventArgs e)
    {
        if (string.Equals(getRoleID(getRole()), "0"))
        {

        }
        else
        {
            Response.Redirect("~/AccessDenied.aspx");
        }
    }
    #endregion

    #region Method

    private string getRole()
    {
        return Session["Role"].ToString().Trim();
    }

    private string getRoleID(string str_name)
    {
        DataSet ds_role = sql.getRole();
        for (int i = 0; i < ds_role.Tables[0].Rows.Count; i++)
        {
            if (ds_role.Tables[0].Rows[i][0].ToString().Trim() == str_name)
            {
                return ds_role.Tables[0].Rows[i][1].ToString().Trim();
            }
        }
        return "";
    }
    #endregion

    protected void btn_upload_Click(object sender, EventArgs e)
    {
        if (!excelHandler.isExcel(this.fleMarketData))
        {
            label_visible.ForeColor = System.Drawing.Color.Red;
            label_visible.Text = "Please select excel file only!";
            return;
        }
        label_visible.Text = "";
        DataTable ds = excelHandler.importExcel(fleMarketData, 0);

        // input data's check
        bool bCheck = checkData(ds);
        if (bCheck == false)
        {
            return;
        }
  
        int flag = 0;
        if (ds != null)
        {
            if (ds.Rows.Count > 0)
            {
                for (int i = 0; i < ds.Rows.Count; i++)
                {
                    if (i >= 1)
                    {
                        //by yyan item w33 20110620 add end
                        //RSM	
                        string str_RSM = ds.Rows[i][0].ToString().Trim();
                        // Sales Organization
                        string str_SalesOrg = ds.Rows[i][1].ToString().Trim();
                        // Date (Status)	
                        string str_DateStatus = ds.Rows[i][2].ToString().Trim();

                        //0+12 2011 
                        string year = str_DateStatus.Substring(str_DateStatus.Length - 4, 4);
                        string month = str_DateStatus.Substring(0, str_DateStatus.Length - 5);
                        if ("3+9".Equals(month))
                        {
                            month = "01";
                        }
                        else if ("5+7".Equals(month))
                        {
                            month = "03";
                        }
                        else if ("9+3".Equals(month))
                        {
                            month = "07";
                        }
                        else if ("0+12".Equals(month))
                        {
                            month = "10";
                            year = Convert.ToInt32(year) - 1 + "";
                        }
                        else
                        {
                            flag = flag + 1;
                            label_visible.Text = label_visible.Text + "Meeting Date(" + str_DateStatus + ") in excel file ( line " + (i + 1) + ") is invalid.</br>";
                            continue; 
                        }
                        str_DateStatus = year + "-" + month + "-01";
                        //delivery in FY	
                        string str_delivery = ds.Rows[i][3].ToString().Trim();
                        //NO in FY	
                        string str_bookings = ds.Rows[i][4].ToString().Trim();
                        //Product	
                        string str_Product = ds.Rows[i][5].ToString().Trim();
                        //Country
                        string str_Country = ds.Rows[i][6].ToString().Trim();
                        //Subregion	
                        string str_Subregion = ds.Rows[i][7].ToString().Trim();
                        // Customer
                        string strCustomerName = ds.Rows[i][8].ToString().Trim();
                        string strCustomerNameID = null;
                        if (string.IsNullOrEmpty(strCustomerName))
                        {
                            string strSQL = "INSERT INTO [CustomerName](Name,Deleted) VALUES('C_Dummy_'+(SELECT CAST(MAX(ID)+1 AS NVARCHAR) FROM [CustomerName]),'0')";
                            helper.ExecuteNonQuery(CommandType.Text, strSQL, null);
                            strSQL = "SELECT MAX(ID) FROM [CustomerName]";
                            object dummyID = helper.ExecuteScalar(CommandType.Text, strSQL, null);

                            strCustomerNameID = Convert.ToString(dummyID);

                            //create new customer
                            strSQL = "INSERT INTO [Customer](NameID,TypeID,SalesChannelID,CountryID,City,Address,Department,Deleted) VALUES('" +
                            strCustomerNameID +
                             "', '1', '-1', '"+getSubregionID(str_Subregion)+"', '', '', '', '0')";
                            helper.ExecuteNonQuery(CommandType.Text, strSQL, null);
                            //get new customer id
                            strSQL = "SELECT MAX(ID) FROM [Customer] Where NameID=" + strCustomerNameID;
                            object customerID = helper.ExecuteScalar(CommandType.Text, strSQL, null);
                            strCustomerNameID = Convert.ToString(customerID);

                            

                        }
                        else
                        {
                            strCustomerNameID = getCustomerNameID(strCustomerName);
                        }
                        //Volume k local currency
                        string str_amount_local = ds.Rows[i][9].ToString().Trim();
                        //Operation (Short)"
                        string str_OperationShort = ds.Rows[i][11].ToString().Trim();
                        //Operation (Name)
                        string str_OperationName = ds.Rows[i][12].ToString().Trim();
                        //Operation(Long)"	
                        string str_OperationLong = ds.Rows[i][13].ToString().Trim();
                        //Segment	
                        string str_Segment = ds.Rows[i][14].ToString().Trim();
                        //Comment
                        string str_Comment = ds.Rows[i][15].ToString().Trim();
                        string tempSegmentID = getSegmentID(str_Segment);

                        int count = UpdData(getRSMID(str_RSM), getSalesOrgID(str_SalesOrg), getSubregionID(str_Subregion),
                            strCustomerNameID, getBookingY(str_bookings), getDeliverY(str_delivery), tempSegmentID,
                            getProductID(str_Product, tempSegmentID), getOperationID(str_OperationName, str_OperationLong),
                            str_amount_local.Replace(",", ""), str_delivery, str_bookings, str_DateStatus);
                        if (count < 1)
                        {
                            count = InsData(getRSMID(str_RSM), getSalesOrgID(str_SalesOrg), getSubregionID(str_Subregion),
                                strCustomerNameID, getBookingY(str_bookings), getDeliverY(str_delivery), tempSegmentID,
                                getProductID(str_Product, tempSegmentID), getOperationID(str_OperationName, str_OperationLong),
                                str_amount_local.Replace(",", ""), str_Comment, str_delivery, str_bookings, str_DateStatus);
                        }
                        if (count < 1)
                        {
                            flag = flag + 1;
                        }
                    }
                }
            }
            if ((flag < 1) || (label_visible.Text == ""))
            {
                label_visible.ForeColor = System.Drawing.Color.Green;
                label_visible.Text = "The file has been uploaded successfully!";
            }
        }
    }

    protected bool checkData(DataTable ds)
    {   
        if (ds == null)
            return true;

        if (ds.Rows.Count > 0)
        {
            for (int i = 1; i < ds.Rows.Count; i++)
            {
                if (excelHandler.isForm(ds, 16))
                {
                    //RSM	
                    string str_RSM = ds.Rows[i][0].ToString().Trim();
                    // Sales Organization
                    string str_SalesOrg = ds.Rows[i][1].ToString().Trim();
                    // Date (Status)	
                    string str_DateStatus = ds.Rows[i][2].ToString().Trim();
                   
                    //delivery in FY	
                    string str_delivery = ds.Rows[i][3].ToString().Trim();
                    //NO in FY	
                    string str_bookings = ds.Rows[i][4].ToString().Trim();
                    //Subregion	
                    string str_Subregion = ds.Rows[i][7].ToString().Trim();
                    // Customer(Name)
                    string strCustomerName = ds.Rows[i][8].ToString().Trim();
                    //Operation (Name)
                    string str_OperationName = ds.Rows[i][12].ToString().Trim();
                    //Segment	
                    string str_Segment = ds.Rows[i][14].ToString().Trim();

                    string SegmentID = getSegmentID(str_Segment);
                    string RSMID = getRSMID(str_RSM);
                    string SalesOrgID = getSalesOrgID(str_SalesOrg);
                    string SubregionID = getSubregionID(str_Subregion);
                    string customerNameID = getCustomerNameID(strCustomerName);

                    // RSM check
                    if ("".Equals(str_RSM))
                    {
                        label_visible.Text = label_visible.Text + "RSM value in excel file ( line " + (i + 1) + ") is empty! Please check.</br>";
                    }
                    else if ("".Equals(RSMID))
                    {
                        label_visible.Text = label_visible.Text + "RSM(" + str_RSM + ") in excel file ( line " + (i + 1) + ") does not exist in System.</br>";
                    }

                    // Sales Organization check
                    if ("".Equals(str_SalesOrg))
                    {
                        label_visible.Text = label_visible.Text + "Sales Organization value in excel file ( line " + (i + 1) + ") is empty! Please check.</br>";
                    }
                    else if ("".Equals(SalesOrgID))
                    {
                        label_visible.Text = label_visible.Text + "Sales Organization(" + str_SalesOrg + ") in excel file ( line " + (i + 1) + ") does not exist in System.</br>";
                    }
                    // Meeting date check
                    if (str_DateStatus.Length < 8)
                    {
                        label_visible.Text = label_visible.Text + "Meeting Date(" + str_DateStatus + ") in excel file ( line " + (i + 1) + ") is empty or invalid! Please check.</br>";
                    }

                    // SubRegion check
                    if ("".Equals(str_Subregion))
                    {
                        label_visible.Text = label_visible.Text + "SubRegion value in excel file ( line " + (i + 1) + ") is empty! Please check.</br>";
                    }
                    else if ("".Equals(SubregionID))
                    {
                        label_visible.Text = label_visible.Text + "SubRegion(" + str_Subregion + ") in excel file ( line " + (i + 1) + ") does not exist in System.</br>";
                    }

                    // Customer check
                    if (!string.IsNullOrEmpty(strCustomerName) && string.IsNullOrEmpty(customerNameID))
                    {
                        label_visible.Text = label_visible.Text + "Customer(" + strCustomerName + ") in excel file ( line " + (i + 1) + ") does not exist in System.</br>";
                    }

                    // delivery in FY check
                    if (str_delivery.Length < 6)
                    {
                        label_visible.Text = label_visible.Text + "delivery in FY(" + str_delivery + ") in excel file ( line " + (i + 1) + ") is empty or invalid! Please check.</br>";
                    }

                    // NO in FY check
                    if (str_bookings.Length < 3)
                    {
                        label_visible.Text = label_visible.Text + "NO in FY(" + str_bookings + ") in excel file ( line " + (i + 1) + ") is empty or invalid! Please check.</br>";                  
                    }

                    // Sales Organization check
                    if ("".Equals(str_Segment))
                    {
                        label_visible.Text = label_visible.Text + "Segment value in excel file ( line " + (i + 1) + ") is empty! Please check.</br>";
                    }
                    else if ("".Equals(SegmentID))
                    {
                        label_visible.Text = label_visible.Text + "Segment(" + str_Segment + ") in excel file ( line " + (i + 1) + ") does not exist in System.</br>";
                    }
                }
                else
                {
                    label_visible.Text = "Please check the format of the excel file!";
                    return false;
                }
            }
        }

        // check ok?
        if ("".Equals(label_visible.Text))
        {
            return true;
        }

        // check NG
        label_visible.ForeColor = System.Drawing.Color.Red;
        return false;
    }
    protected int getRowId(string str_RSMID, string str_bookingY, string str_deliverY, string str_countryID, string str_customerID, string str_projectID, string SalesChannelID, string str_salesorgID, string str_segmentID, string str_timeFlag)
    {
        string sql = "SELECT  case when MAX(RecordID) is null then 0 else MAX(RecordID) end FROM [Bookings]"
                    + " WHERE RSMID = " + str_RSMID
                    + " AND CountryID = " + str_countryID
                    + " AND BookingY = '" + str_bookingY + "'"
                    + " AND DeliverY = '" + str_deliverY + "'"
                    + " AND SegmentID = '" + str_segmentID + "'"
                    + " AND TimeFlag = '" + str_timeFlag + "'";

        DataSet ds = helper.GetDataSet(sql);
        return Convert.ToInt32(ds.Tables[0].Rows[0][0]);
    }
    private int InsData(string RSMID, string SalesOrgID, string CountryID,
        string CustomerNameID, string BookingY, string DeliverY, string SegmentID,
        string ProductID, string OperationID, string Amount, string Comments,
        string DeliveryinFY, string NOinFY, string TimeFlag)
    {

        DataSet ds_product = getProductBySegment(SegmentID);
        var dict = new Dictionary<int, string[]>();

        for (int key = 0; key < ds_product.Tables[0].Rows.Count; key++)
        {
            var product = ds_product.Tables[0].Rows[key];
            dict.Add(key, new String[] { product.ItemArray[0].ToString(), OperationID });
        }
        int recordId = getRowId(RSMID, BookingY, DeliverY, CountryID, CustomerNameID, "-1", "-1", SalesOrgID, SegmentID, TimeFlag) + 1;
        int i;
        for (i = 0; i < ds_product.Tables[0].Rows.Count; i++)
        {
            string insAmount = "0";
            if (ProductID.Equals(ds_product.Tables[0].Rows[i][0].ToString()))
            {
                insAmount = Amount;
            }
            string insert_booking = "INSERT INTO [Bookings](RecordID,RSMID, SalesOrgID, CountryID, CustomerID, BookingY, DeliverY, SegmentID, ProductID, OperationID, ProjectID, SalesChannelID, Amount, Comments,[Delivery in FY], [NO in FY],  TimeFlag)"
                        + " VALUES("
                        + recordId.ToString() + ","
                        + RSMID + ","
                        + SalesOrgID + ","
                        + CountryID + ","
                        + CustomerNameID + ","
                        + "'" + BookingY + "'" + ","
                        + "'" + DeliverY + "'" + ","
                        + SegmentID + ","
                        + ProductID + ","
                        + OperationID + ","
                        + "-1,"
                        + "-1,"
                        + insAmount + ","
                        + "NULL,'"
                        + DeliveryinFY + "','"
                        + NOinFY + "','"
                        + TimeFlag + "')";
            int count = helper.ExecuteNonQuery(CommandType.Text, insert_booking, null);

            if (count == 1)
            {
                //success
            }
            else
            {
                //add error
                break;
            }
            
        }
        return i;
        /*
        StringBuilder sql = new StringBuilder();
        sql.AppendLine(" INSERT INTO ");
        sql.AppendLine("   Bookings( ");
        sql.AppendLine("     RSMID, ");
        sql.AppendLine("     SalesOrgID, ");
        sql.AppendLine("     CountryID, ");
        sql.AppendLine("     CustomerID, ");
        sql.AppendLine("     BookingY, ");
        sql.AppendLine("     DeliverY, ");
        sql.AppendLine("     SegmentID, ");
        sql.AppendLine("     ProductID, ");
        sql.AppendLine("     OperationID, ");
        sql.AppendLine("     ProjectID, ");
        sql.AppendLine("     SalesChannelID, ");
        sql.AppendLine("     Amount, ");
        sql.AppendLine("     Comments, ");
        sql.AppendLine("     [Delivery in FY], ");
        sql.AppendLine("     [NO in FY], ");
        sql.AppendLine("     TimeFlag, ");
        sql.AppendLine("    RecordId) ");
        sql.AppendLine(" VALUES( ");
        sql.AppendLine("   @RSMID,");
        sql.AppendLine("   @SalesOrgID,");
        sql.AppendLine("   @CountryID,");
        sql.AppendLine("   @CustomerID,");
        sql.AppendLine("   @BookingY,");
        sql.AppendLine("   @DeliverY,");
        sql.AppendLine("   @SegmentID,");
        sql.AppendLine("   @ProductID,");
        sql.AppendLine("   @OperationID,");
        sql.AppendLine("   -1,");
        sql.AppendLine("   -1,");
        sql.AppendLine("   @Amount,");
        sql.AppendLine("   @Comments,");
        sql.AppendLine("   @DeliveryinFY,");
        sql.AppendLine("   @NOinFY,");
        sql.AppendLine("   @TimeFlag,");
        sql.AppendLine("   @RecordId)");
        SqlParameter[] paramArray = new SqlParameter[14];
        paramArray[0] = new SqlParameter("@RSMID", RSMID);
        paramArray[1] = new SqlParameter("@SalesOrgID", SalesOrgID);
        paramArray[2] = new SqlParameter("@CountryID", CountryID);
        paramArray[3] = new SqlParameter("@CustomerID", CustomerNameID);
        paramArray[4] = new SqlParameter("@BookingY", BookingY);
        paramArray[5] = new SqlParameter("@DeliverY", DeliverY);
        paramArray[6] = new SqlParameter("@SegmentID", SegmentID);
        paramArray[7] = new SqlParameter("@ProductID", ProductID);
        paramArray[8] = new SqlParameter("@OperationID", OperationID);
        paramArray[9] = new SqlParameter("@Amount", Amount);
        paramArray[10] = new SqlParameter("@Comments", Comments);
        paramArray[11] = new SqlParameter("@DeliveryinFY", DeliveryinFY);
        paramArray[12] = new SqlParameter("@NOinFY", NOinFY);
        paramArray[13] = new SqlParameter("@TimeFlag", TimeFlag);
        paramArray[14] = new SqlParameter("@RecordId", recordId);
        return helper.ExecuteNonQuery(CommandType.Text, sql.ToString(), paramArray);
        */
    }

    private int UpdData(string RSMID, string SalesOrgID, string CountryID,
        string CustomerNameID, string BookingY, string DeliverY, string SegmentID,
        string ProductID, string OperationID, string Amount, string DeliveryinFY,
        string NOinFY, string TimeFlag)
    {
        StringBuilder sql = new StringBuilder();
        sql.AppendLine(" UPDATE ");
        sql.AppendLine("   Bookings ");
        sql.AppendLine(" SET ");
        sql.AppendLine("   Amount=Amount+ " + Amount);
        sql.AppendLine(" WHERE ");
        sql.AppendLine("   RSMID=@RSMID");
        sql.AppendLine("   AND SalesOrgID=@SalesOrgID");
        sql.AppendLine("   AND CountryID=@CountryID");
        sql.AppendLine("   AND CustomerID=@CustomerID");
        sql.AppendLine("   AND BookingY=@BookingY");
        sql.AppendLine("   AND DeliverY=@DeliverY");
        sql.AppendLine("   AND SegmentID=@SegmentID");
        sql.AppendLine("   AND ProductID=@ProductID");
        sql.AppendLine("   AND OperationID=@OperationID");
        sql.AppendLine("   AND ProjectID=-1");
        sql.AppendLine("   AND SalesChannelID=-1");
        sql.AppendLine("   AND [Delivery in FY]=@DeliveryinFY");
        sql.AppendLine("   AND [NO in FY]=@NOinFY");
        sql.AppendLine("   AND TimeFlag=@TimeFlag");
        SqlParameter[] paramArray = new SqlParameter[13];
        paramArray[0] = new SqlParameter("@Amount", Amount);
        paramArray[1] = new SqlParameter("@RSMID", RSMID);
        paramArray[2] = new SqlParameter("@SalesOrgID", SalesOrgID);
        paramArray[3] = new SqlParameter("@CountryID", CountryID);
        paramArray[4] = new SqlParameter("@CustomerID", CustomerNameID);
        paramArray[5] = new SqlParameter("@BookingY", BookingY);
        paramArray[6] = new SqlParameter("@DeliverY", DeliverY);
        paramArray[7] = new SqlParameter("@SegmentID", SegmentID);
        paramArray[8] = new SqlParameter("@ProductID", ProductID);
        paramArray[9] = new SqlParameter("@OperationID", OperationID);
        paramArray[10] = new SqlParameter("@DeliveryinFY", DeliveryinFY);
        paramArray[11] = new SqlParameter("@NOinFY", NOinFY);
        paramArray[12] = new SqlParameter("@TimeFlag", TimeFlag);
        return helper.ExecuteNonQuery(CommandType.Text, sql.ToString(), paramArray);
    }

    protected string getSubregionID(string str)
    {
        str = str.Replace("'", "''");
        string sql = "SELECT [SubRegion].ID FROM [SubRegion]  WHERE [SubRegion].Name = '" + str + "' AND [SubRegion].Deleted = 0 ";
        DataSet ds = helper.GetDataSet(sql);
        if (ds.Tables[0].Rows.Count >= 1)
            return ds.Tables[0].Rows[0][0].ToString().Trim();
        else
        {
            return "";
        }
    }
    

    protected string getSalesOrgID(string str)
    {
        str = str.Replace("'", "''");
        string sql = "SELECT [SalesOrg].ID FROM [SalesOrg]  WHERE [SalesOrg].Abbr = '" + str + "' AND [SalesOrg].Deleted = 0 ";
        DataSet ds = helper.GetDataSet(sql);
        if (ds.Tables[0].Rows.Count >= 1)
            return ds.Tables[0].Rows[0][0].ToString().Trim();
        else
        {
            return "";
        }
    }
    
 
    protected string getRSMID(string str)
    {
        str = str.Replace("'", "''");
        string sql = "SELECT [User].UserID FROM [User]  WHERE [User].Abbr = '" + str + "' AND [User].Deleted = 0 ";
        DataSet ds = helper.GetDataSet(sql);
        if (ds.Tables[0].Rows.Count >= 1)
            return ds.Tables[0].Rows[0][0].ToString().Trim();
        else
        {
            return "";
        }
    }

    protected string getSegmentID(string str)
    {
        str = str.Replace("'", "''");
        string sql = "SELECT [Segment].ID FROM [Segment]  WHERE [Segment].Abbr = '" + str + "' AND [Segment].Deleted = 0 ";
        DataSet ds = helper.GetDataSet(sql);
        if (ds.Tables[0].Rows.Count >= 1)
            return ds.Tables[0].Rows[0][0].ToString().Trim();
        else
        {
            return "";
        }
    }

    protected string getCustomerNameID(string str)
    {
        str = str.Replace("'", "''");
        string sql = "SELECT [CustomerName].ID FROM [CustomerName]  WHERE [CustomerName].Name = '" + str + "' AND [CustomerName].Deleted = 0 ";
        DataSet ds = helper.GetDataSet(sql);
        if (ds.Tables[0].Rows.Count >= 1)
            return ds.Tables[0].Rows[0][0].ToString().Trim();
        else
        {
            return "";
        }
    }

    protected DataSet getProductBySegment(string segmentID)
    {
        string query_product = "SELECT ID,Abbr FROM [Product] INNER JOIN [Segment_Product] ON [Segment_Product].ProductID = [Product].ID "
                       + " WHERE SegmentID = " + segmentID + " AND [Product].Deleted = 0 AND [Segment_Product].Deleted = 0";
        DataSet ds_product = helper.GetDataSet(query_product);
        return ds_product;
    }

    protected string getDeliverY(string deliverY)
    {
        string str_DeliverY = "";
        if (!"".Equals(deliverY))
        {
            //by mbq 20110524 item12 del End 
                //by mbq 20110524 item12 add start 
                //if (str_DeliverY.Length > 6)
                //{
                //by mbq 20110524 item12 add End 
            //by mbq 20110524 item12 del End 


           //by mbq 20110524 item12 add start 
            if (deliverY.Length > 6)
            {
            //by mbq 20110524 item12 add End 

            //by mbq 20110513 item12 add start 
                str_DeliverY = deliverY.Substring(deliverY.Length - 2, 2);
                
            }
            else {
            //by mbq 20110524 item12 add End 
                str_DeliverY = deliverY.Substring(deliverY.Length - 3, 3);
            //by mbq 20110513 item12 add start 
            }
            //by mbq 20110524 item12 add End 
            
        }
        //by mbq 20110524 item12 del start 
        // return str_DeliverY;
        //by mbq 20110524 item12 del End 
        //by mbq 20110524 item12 add start 
         return str_DeliverY.Trim();
        //by mbq 20110524 item12 add End 
    }

    protected string getBookingY(string bookingY)
    {
        //by mbq 20110526 item12 del Start 
        //string str_BookingY
        //by mbq 20110526 item12 del End 

        //by mbq 20110526 item12 add Start 
        string str_BookingY = "" ;
        //by mbq 20110526 item12 add End 


        //by mbq 20110526 item12 del Start 
        /**
        if (bookingY.Length > 4)
        {
            str_BookingY = bookingY.Substring(2, 3);
        }
        else {
            str_BookingY = bookingY.Substring(0, 2);
        }
        if (!"YTD".Equals(str_BookingY))
        {
            str_BookingY = bookingY.Substring(0, 2);
        }
         **/
        //by mbq 20110526 item12 del End 
        //by mbq 20110526 item12 del Start 
        //return str_BookingY;
        //by mbq 20110526 item12 del End 


        //by mbq 20110526 item12 add Start 
        str_BookingY = bookingY.Substring(0, 2);
        return str_BookingY.Trim();
        //by mbq 20110526 item12 add End 
        
    }

    protected string getOperationID(string AbbrL, string Name)
    {
        AbbrL = AbbrL.Replace("'", "''");
        Name = Name.Replace("'", "''");
        string query_product = "SELECT ID FROM [Operation] WHERE AbbrL = '" + AbbrL + "'AND Name ='" + Name + "' AND Deleted = 0";
        DataSet ds = helper.GetDataSet(query_product);
        if (ds.Tables[0].Rows.Count >= 1)
            return ds.Tables[0].Rows[0][0].ToString().Trim();
        else
        {
            return "-1";
        }
    }

    protected string getCurrencyID(string Currency)
    {
        Currency = Currency.Replace("'", "''");
        string query_product = "SELECT ID FROM [Currency] WHERE Name = '" + Currency + "'AND Deleted = 0";
        DataSet ds = helper.GetDataSet(query_product);
        if (ds.Tables[0].Rows.Count >= 1)
            return ds.Tables[0].Rows[0][0].ToString().Trim();
        else
        {
            return "-1";
        }
    }
    //by mbq 20110526 item12 del Start 
    //protected string getProductID(string Product)
    //by mbq 20110526 item12 del End 
    //by mbq 20110526 item12 add Add 
    protected string getProductID(string Product,string segmentId)
    //by mbq 20110526 item12 add End 
    {
        Product = Product.Replace("'", "''");
        //by mbq 20110526 item12 del Start 
        //string query_product = "SELECT ID FROM [Product] WHERE Abbr = '" + Product + "'AND Deleted = 0";
        //by mbq 20110526 item12 del End 
        //by mbq 20110526 item12 add Add 
        string query_product = "SELECT ID FROM [Product] ,[Segment_Product]"
        + " WHERE Abbr = '" + Product + "'"
        + " AND [Segment_Product].SegmentID = '" + segmentId + "'"
        +" AND [Segment_Product].ProductID = [Product].ID "
        +" AND [Product].Deleted = 0 "
        +" AND[Segment_Product].Deleted = 0 ";
        //by mbq 20110526 item12 add End 
        DataSet ds = helper.GetDataSet(query_product);
        if (ds.Tables[0].Rows.Count >= 1)
            return ds.Tables[0].Rows[0][0].ToString().Trim();
        else
        {
            return "-1";
        }
    }

    protected object getGroupID(int i)
    {
        DataTable ds = excelHandler.importExcel(fleMarketData, 0);
        ds.Columns.Add("GroupID");

        //RSM	
        string str_RSM = ds.Rows[i][0].ToString().Trim();
        // Sales Organization
        string str_SalesOrg = ds.Rows[i][1].ToString().Trim();
        // Date (Status)	
        string str_DateStatus = ds.Rows[i][2].ToString().Trim();
        //0+12 2011 
        string year = str_DateStatus.Substring(str_DateStatus.Length - 4, 4);
        string month = str_DateStatus.Substring(0, str_DateStatus.Length - 5);

        if ("3+9".Equals(month))
        {
            month = "01";
        }
        else if ("5+7".Equals(month))
        {
            month = "03";
        }
        else if ("9+3".Equals(month))
        {
            month = "07";
        }
        else if ("0+12".Equals(month))
        {
            month = "10";
            year = Convert.ToInt32(year) - 1 + "";
        }
        str_DateStatus = year + "-" + month + "-01";
        //delivery in FY	
        string str_delivery = ds.Rows[i][3].ToString().Trim();
        //NO in FY	
        string str_bookings = ds.Rows[i][4].ToString().Trim();
        //Product	
        string str_Product = ds.Rows[i][5].ToString().Trim();
        //Country
        string str_Country = ds.Rows[i][6].ToString().Trim();
        //Subregion	
        string str_Subregion = ds.Rows[i][7].ToString().Trim();
        //Volume k local currency
        string str_amount_local = ds.Rows[i][8].ToString().Trim();
        //Operation (Short)"
        string str_OperationShort = ds.Rows[i][10].ToString().Trim();
        //Operation (Name)
        string str_OperationName = ds.Rows[i][11].ToString().Trim();
        //Operation(Long)"	
        string str_OperationLong = ds.Rows[i][12].ToString().Trim();
        //Segment	
        string str_Segment = ds.Rows[i][13].ToString().Trim();
        //Comment
        string str_Comment = ds.Rows[i][14].ToString().Trim();

        string tempSegmentID = getSegmentID(str_Segment);

        string select_booking = "select ISNULL(MAX(GroupID),0)+1 GroupID from [Bookings] where "
          + " RSMID = '" + getRSMID(str_RSM) + "'"
          + " AND SalesOrgID = '" + getSalesOrgID(str_SalesOrg) + "'"
          + " AND CountryID  = '" + getSubregionID(str_Subregion) + "'"
          + " AND CustomerID = '-1'"
          + " AND BookingY   = '" + getBookingY(str_bookings) + "'"
          + " AND DeliverY   = '" + getDeliverY(str_delivery) + "'"
            //by mbq 20110526 item12 del Start 
            //+ " AND SegmentID  = '" + getSegmentID(str_Segment) + "'"
            //by mbq 20110526 item12 del End 
            //by mbq 20110526 item12 add Start 
          + " AND SegmentID  = '" + tempSegmentID + "'"
            //by mbq 20110526 item12 add End 
            //by mbq 20110526 item12 del Start 
            //+ " AND ProductID  = '" + getProductID(str_Product) + "'"
            //by mbq 20110526 item12 del End 
            //by mbq 20110526 item12 add Start 
          + " AND ProductID  = '" + getProductID(str_Product, tempSegmentID) + "'"
            //by mbq 20110526 item12 add End 
          + " AND OperationID = '" + getOperationID(str_OperationName, str_OperationLong) + "'"
          + " AND ProjectID  = '-1'"
          + " AND SalesChannelID = '-1'"
          + " AND amount = '" + str_amount_local.Replace(",", "") + "'"
          + " AND [Delivery in FY] = '" + str_delivery + "'"
          + " AND [NO in FY] = '" + str_bookings + "'"
          + " AND TimeFlag = '" + str_DateStatus + "'";
        DataSet select_bookingDs = helper.GetDataSet(select_booking);
        return select_bookingDs.Tables[0].Rows[0][0];
    }
/*
    protected bool checkExcel()
    {
        string flag = "";
        label_visible.Text = "";
        DataTable ds = excelHandler.importExcel(fleMarketData, 0);
        if (ds != null)
        {
            if (ds.Rows.Count > 0)
            {
                for (int i = 0; i < ds.Rows.Count; i++)
                {
                    if (i >= 1)
                    {
                        //by yyan item w33 20110620 add start
                        if (excelHandler.isForm(ds,15))
                        {
                        //by yyan item w33 20110620 add end
                        //RSM	
                        string str_RSM = ds.Rows[i][0].ToString().Trim();
                        // Sales Organization
                        string str_SalesOrg = ds.Rows[i][1].ToString().Trim();
                        // Date (Status)	
                        string str_DateStatus = ds.Rows[i][2].ToString().Trim();
                        //delivery in FY	
                        string str_delivery = ds.Rows[i][3].ToString().Trim();
                        //NO in FY	
                        string str_bookings = ds.Rows[i][4].ToString().Trim();
                        //Product	
                        string str_Product = ds.Rows[i][5].ToString().Trim();
                        //Country
                        string str_Country = ds.Rows[i][6].ToString().Trim();
                        //Subregion	
                        string str_Subregion = ds.Rows[i][7].ToString().Trim();
                        //Volume k local currency
                        string str_amount_local = ds.Rows[i][8].ToString().Trim();
                        //Operation (Short)"
                        string str_OperationShort = ds.Rows[i][10].ToString().Trim();
                        //Operation (Name)
                        string str_OperationName = ds.Rows[i][11].ToString().Trim();
                        //Operation(Long)"	
                        string str_OperationLong = ds.Rows[i][12].ToString().Trim();
                        //Segment	
                        string str_Segment = ds.Rows[i][13].ToString().Trim();
                        //Comment
                        string str_Comment = ds.Rows[i][14].ToString().Trim();

                        for (int j = 0; j < ds.Rows.Count; j++)
                        {
                            if (j != i && j > i)
                            {
                                //RSM	
                                string str_RSMj = ds.Rows[j][0].ToString().Trim();
                                // Sales Organization
                                string str_SalesOrgj = ds.Rows[j][1].ToString().Trim();
                                // Date (Status)	
                                string str_DateStatusj = ds.Rows[j][2].ToString().Trim();
                                //delivery in FY	
                                string str_deliveryj = ds.Rows[j][3].ToString().Trim();
                                //NO in FY	
                                string str_bookingsj = ds.Rows[j][4].ToString().Trim();
                                //Product	
                                string str_Productj = ds.Rows[j][5].ToString().Trim();
                                //Country
                                string str_Countryj = ds.Rows[j][6].ToString().Trim();
                                //Subregion	
                                string str_Subregionj = ds.Rows[j][7].ToString().Trim();
                                //Volume k local currency
                                string str_amount_localj = ds.Rows[j][8].ToString().Trim();
                                //Operation (Short)"
                                string str_OperationShortj = ds.Rows[j][10].ToString().Trim();
                                //Operation (Name)
                                string str_OperationNamej = ds.Rows[j][11].ToString().Trim();
                                //Operation(Long)"	
                                string str_OperationLongj = ds.Rows[j][12].ToString().Trim();
                                //Segment	
                                string str_Segmentj = ds.Rows[j][13].ToString().Trim();
                                //Comment
                                string str_Commentj = ds.Rows[j][14].ToString().Trim();

                                string stri = "";
                                string strj = "";
                                if (str_RSM.Equals(str_RSMj) && str_SalesOrg.Equals(str_SalesOrgj) && str_DateStatus.Equals(str_DateStatusj) && str_delivery.Equals(str_deliveryj)
                                    && str_bookings.Equals(str_bookingsj) && str_Product.Equals(str_Productj) && str_Country.Equals(str_Countryj)
                                    && str_Subregion.Equals(str_Subregionj) && str_amount_local.Equals(str_amount_localj) && str_OperationName.Equals(str_OperationNamej)
                                    && str_OperationLong.Equals(str_OperationLongj) && str_Segment.Equals(str_Segmentj))
                                {
                                    stri = i + 1 + "";
                                    strj = j + 1 + "";
                                    label_visible.Text = label_visible.Text + "Same data in line " + stri + " and " + strj + ".</br>";
                                    flag = "true";
                                }
                            }
                        }
                        //by yyan item w33 20110620 add start
                        }
                        else
                        {
                            label_visible.Text = "Please check the format of the excel file!";
                            return false;
                        }
                        //by yyan item w33 20110620 add end
                    }
                }
            }
        }
        if ("true".Equals(flag))
        {
            return false;
        }
        return true;
    }
    */    
}
