/*
 * File Name   : MarketingMgrGrossSales.aspx.cs
 * 
 * Description : Get sales data by operation
 * 
 * Author      : Wang Jun
 * 
 * Modify Date : 2010-04-12
 * 
 * Problem     : none
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

public partial class MarketingMgr_Default : System.Web.UI.Page
{
    SQLHelper helper = new SQLHelper();
    WebUtility web = new WebUtility();
    LogUtility log = new LogUtility();
    SQLStatement sql = new SQLStatement();
    DisplayInfo info = new DisplayInfo();
    //by ryzhang item49 20110519 del start 
    //GetMeetingDate meeting = new GetMeetingDate();
    //by ryzhang item49 20110519 del end 
    //by ryzhang item49 20110519 add start 
    GetSelectMeetingDate meeting = new GetSelectMeetingDate();
    //by ryzhang item49 20110519 add end 
    CommonFunction cf = new CommonFunction();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (getRoleID(getRole()) != "2")
            Response.Redirect("~/AccessDenied.aspx");

        if (!IsPostBack)
        {
            //by ryzhang item49 20110519 del start 
            //meeting.setDate();
            //by ryzhang item49 20110519 del end 
            //by ryzhang item49 20110519 add start 
            meeting.setSelectDate(Session["GeneralMarketingMgrID"].ToString());
            //by ryzhang item49 20110519 add end 
            label_description.Text = getSegmentDec(getSegmentID()) + " SALES BY OPERATION";
            //by yyan item8 20110623 del start 
            //bindDataSource();
            //by yyan item8 20110623 del end 
            //by yyan item8 20110623 add start 
            bindDataSource(0);
            //by yyan item8 20110623 add end 
        }
    }

    /* Get user'role */
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

    protected string getSegmentID()
    {
        return Request.QueryString["SegmentID"].ToString().Trim();
    }

    //Get segment description by segmentID
    protected string getSegmentDec(string segmentID)
    {
        DataSet ds_segment = sql.getSegmentInfo();
        string str_segmentDec = "";
        for (int i = 0; i < ds_segment.Tables[0].Rows.Count; i++)
        {
            string str_segmentID = ds_segment.Tables[0].Rows[i][0].ToString().Trim();
            if (str_segmentID == segmentID)
            {
                str_segmentDec = ds_segment.Tables[0].Rows[i][2].ToString().Trim();
                break;
            }
        }
        return str_segmentDec;
    }

    protected string getProductIDBySegmentIDByProductAbbr(string str_segmentID, string str_productAbbr)
    {
        DataSet dspro = sql.getProductInfoBySegmentID(str_segmentID);
        string proID = "";
        for (int i = 0; i < dspro.Tables[0].Rows.Count; i++)
        {
            proID = dspro.Tables[0].Rows[i][0].ToString().Trim();
            string abbr = dspro.Tables[0].Rows[i][1].ToString().Trim();
            if (abbr == str_productAbbr)
                break;
        }
        return proID;
    }

    

    //by yyan item8 20110623 del start 
    //protected GridView bindDataByOperation(DataSet ds, GridView gv, string str_operationID, string header, DataSet ds_budget)
    //by yyan item8 20110623 del end 
    //by yyan item8 20110623 add start 
    protected GridView bindDataByOperation(DataSet ds, GridView gv, string str_operationID, string header, DataSet ds_budget,int flagMoney,int flag)
    //by yyan item8 20110623 add end
    {
        if (ds != null && ds.Tables[0].Rows.Count > 0)
        {
            gv.Visible = true;
            gv.AutoGenerateColumns = false;
            gv.Width = Unit.Pixel(600);
            gv.Columns.Clear();

            //Calculate the total column of next year.
            ds.Tables[0].Columns.Add("Total");
            if (ds.Tables[0].Rows[0][0] != DBNull.Value)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    float tmp = 0;
                    for (int count2 = 1; count2 < ds.Tables[0].Columns.Count - 1; count2++)
                    {
                        tmp += float.Parse(dr[count2].ToString());
                    }
                    dr["Total"] = tmp.ToString();
                }
            }

            for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
            {
                BoundField bf = new BoundField();

                bf.DataField = ds.Tables[0].Columns[i].ColumnName.ToString();
                bf.HeaderText = ds.Tables[0].Columns[i].Caption.ToString();
                bf.ItemStyle.HorizontalAlign = HorizontalAlign.Right;
                bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Right;

                if (i == 0)
                {
                    bf.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
                    bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
                    bf.ItemStyle.Width = 100;
                }

                gv.Columns.Add(bf);
            }
            gv.Caption = header;
            gv.CaptionAlign = TableCaptionAlign.Left;
            gv.DataSource = ds;

            if (flag != 0)
            {
                DataRow[] rows = new DataRow[2];
                rows[0] = ds.Tables[0].NewRow();
                rows[1] = ds.Tables[0].NewRow();

                float[] Sum1 = new float[20];
                float[] Sum2 = new float[20];

                // add by md
                List<string> productabbs = new List<string>();
                for (int m = 1; m < ds.Tables[0].Columns.Count - 1; m++)
                {
                    productabbs.Add(ds.Tables[0].Columns[m].ColumnName.Trim());
                }
                Dictionary<string, int> productDics=null;
                Dictionary<int, float> backLogDics=null;
                Dictionary<int, float> backLogDicsNext=null;
                if (productabbs.Count > 0)
                {
                    productDics = sql.getProductIDsBySegmentIDByProductAbbrs(getSegmentID(), productabbs);
                    backLogDics = sql.getBackLogByOperationProductids(str_operationID, getSegmentID(), productDics.Values.ToArray(), meeting.getyear(), meeting.getmonth(), meeting.getyear().Substring(2, 2).Trim(), flagMoney);
                    backLogDicsNext = sql.getBackLogByOperationProductids(str_operationID, getSegmentID(), productDics.Values.ToArray(), meeting.getyear(), meeting.getmonth(), meeting.getnextyear().Substring(2, 2).Trim(), flagMoney);

                }
                // add end

                for (int j = 1; j < ds.Tables[0].Columns.Count - 1; j++)
                {
                    string str_productAbbr = ds.Tables[0].Columns[j].ColumnName.Trim();
                    //string str_productID = getProductIDBySegmentIDByProductAbbr(getSegmentID(), str_productAbbr);
                    //by yyan item8 20110623 del start 
                    //string str_bl = sql.getBackLogByOperation(str_operationID, getSegmentID(), str_productID, meeting.getyear(), meeting.getmonth(), meeting.getyear().Substring(2, 2).Trim());
                    //by yyan item8 20110623 del end 
                    //by yyan item8 20110623 add start 
                    //string str_bl = sql.getBackLogByOperation(str_operationID, getSegmentID(), str_productID, meeting.getyear(), meeting.getmonth(), meeting.getyear().Substring(2, 2).Trim(), flagMoney);
                    //by yyan item8 20110623 add end
                    //if (str_bl == "")
                    //    str_bl = "0";
                    float aa = 0;
                    backLogDics.TryGetValue(productDics[str_productAbbr], out aa);
                    //Sum1[j] = float.Parse(str_bl);
                    Sum1[j] = aa;
                    //by yyan item8 20110623 del start 
                    //string str_blnext = sql.getBackLogByOperation(str_operationID, getSegmentID(), str_productID, meeting.getyear(), meeting.getmonth(), meeting.getnextyear().Substring(2, 2).Trim());
                    //by yyan item8 20110623 del end 
                    //by yyan item8 20110623 add start 
                    //string str_blnext = sql.getBackLogByOperation(str_operationID, getSegmentID(), str_productID, meeting.getyear(), meeting.getmonth(), meeting.getnextyear().Substring(2, 2).Trim(), flagMoney);
                    //by yyan item8 20110623 add end
                    //if (str_blnext == "")
                    //    str_blnext = "0";
                    //Sum2[j] = float.Parse(str_blnext);

                    float bb = 0;
                    backLogDicsNext.TryGetValue(productDics[str_productAbbr], out bb);
                    Sum2[j] = bb;
                }

                for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
                {
                    if (i == 0)
                    {
                        rows[0][0] = meeting.getyear().Substring(2, 2) + "B/L";
                        rows[1][0] = meeting.getnextyear().Substring(2, 2) + "B/L";
                    }
                    else
                    {
                        rows[0][i] = Sum1[i];
                        rows[1][i] = Sum2[i];
                    }
                }

                float tempY = 0;
                float tempNY = 0;
                for (int i = 0; i < ds.Tables[0].Columns.Count - 1; i++)
                {
                    tempY += Sum1[i];
                    tempNY += Sum2[i];
                }
                rows[0][ds.Tables[0].Columns.Count - 1] = tempY;
                rows[1][ds.Tables[0].Columns.Count - 1] = tempNY;
                ds.Tables[0].Rows.InsertAt(rows[0], 0);
                ds.Tables[0].Rows.InsertAt(rows[1], 1);
                //by yyan item8 20110623 del start 
                //getSalesForecast(ds, str_operationID, meeting.getyear().Substring(2, 2));
                //getSalesForecast(ds, str_operationID, meeting.getnextyear().Substring(2, 2));
                //by yyan item8 20110623 del end 
                //by yyan item8 20110623 add start 
                getSalesForecast(ds, str_operationID, meeting.getyear().Substring(2, 2),flagMoney);
                getSalesForecast(ds, str_operationID, meeting.getnextyear().Substring(2, 2),flagMoney);
                //by yyan item8 20110623 add end
            }
            else
            {
                DataRow drSum = ds.Tables[0].NewRow();
                float[] Sum = new float[20];
                for (int j = 1; j < ds.Tables[0].Columns.Count - 1; j++)
                {
                    if (ds_budget.Tables[0].Rows.Count != 0)
                        Sum[j] = float.Parse(ds_budget.Tables[0].Rows[0][j].ToString().Trim());
                    else
                        Sum[j] = 0;
                }

                float tmp1 = 0;
                for (int i = 0; i < ds.Tables[0].Columns.Count - 1; i++)
                {
                    if (i == 0)
                    {
                        drSum[0] = "Bookings " + meeting.getnextyear().Substring(2, 2) + " B";
                    }
                    else
                    {
                        drSum[i] = Sum[i];
                        tmp1 += Sum[i];
                    }
                }
                drSum[ds.Tables[0].Columns.Count - 1] = tmp1.ToString().Trim();
                ds.Tables[0].Rows.InsertAt(drSum, ds.Tables[0].Rows.Count);
            }
            //by yyan itemw93 add start 
            if (flag != 0)
            {
                bool bz = false;
                int k = 0;
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    if (ds.Tables[0].Rows[i][0].ToString().Contains("F/C"))
                    {
                        bz = true;
                        k = i;
                        break;
                    }
                }
                if (bz)
                {
                    if (flag == 1)
                    {
                        for (int i = ds.Tables[0].Rows.Count - 1; i >= k; i--)
                        {
                            ds.Tables[0].Rows[i].Delete();
                        }
                    }
                    else if (flag == 2)
                    {
                        for (int i = k - 1; i >= 0; i--)
                        {
                            ds.Tables[0].Rows[i].Delete();
                        }
                    }
                }
            }
            //by yyan itemw93 add end 
            gv.DataBind();
        }
        else
            gv.Visible = false;
        return gv;
    }
    //by yyan item8 20110623 del start 
    //protected void getSalesForecast(DataSet ds, string str_operationID, string backlogY)
    //by yyan item8 20110623 del end 
    //by yyan item8 20110623 add start 
    protected void getSalesForecast(DataSet ds, string str_operationID, string backlogY,int flagMoney)
    //by yyan item8 20110623 add end
    {
        DataSet[] ds_budget = new DataSet[2];
        DataSet dsPro = sql.getProductInfoBySegmentID(getSegmentID());
        //by yyan item8 20110623 del start 
        //ds_budget[0] = sql.getSalesBudgetByOperation(dsPro, str_operationID, getSegmentID(), meeting.getpreyear(), meeting.getyear().Substring(2, 2));
        //ds_budget[1] = sql.getSalesBudgetByOperation(dsPro, str_operationID, getSegmentID(), meeting.getyear(), meeting.getnextyear().Substring(2, 2));
        //by yyan item8 20110623 del end 
        //by yyan item8 20110623 add start 
        ds_budget[0] = sql.getSalesBudgetByOperation(dsPro, str_operationID, getSegmentID(), meeting.getpreyear(), meeting.getyear().Substring(2, 2), flagMoney);
        ds_budget[1] = sql.getSalesBudgetByOperation(dsPro, str_operationID, getSegmentID(), meeting.getyear(), meeting.getnextyear().Substring(2, 2), flagMoney);
        //by yyan item8 20110623 add end
        
        DataRow[] rows = new DataRow[2];
        rows[0] = ds.Tables[0].NewRow();
        rows[1] = ds.Tables[0].NewRow();

        float[] SumFC = new float[20];
        float[] SumBL = new float[20];
        for (int j = 1; j < ds.Tables[0].Columns.Count - 1; j++)
        {
            string str_productAbbr = ds.Tables[0].Columns[j].ColumnName.Trim();
            string str_productID = getProductIDBySegmentIDByProductAbbr(getSegmentID(), str_productAbbr);
            string backlog;
            //by yyan item8 20110623 del start 
            //backlog = sql.getBackLogByOperation(str_operationID, getSegmentID(), str_productID, meeting.getyear(), meeting.getmonth(), backlogY);
            //by yyan item8 20110623 del end 
            //by yyan item8 20110623 add start 
            backlog = sql.getBackLogByOperation(str_operationID, getSegmentID(), str_productID, meeting.getyear(), meeting.getmonth(), backlogY, flagMoney);
            //by yyan item8 20110623 add end
            if (backlog == "")
                backlog = "0";
            float backlogValue = float.Parse(backlog);

            SumFC[j] = backlogValue;
            for (int i = 2; i < ds.Tables[0].Rows.Count; i++)
            {
                string rowName = ds.Tables[0].Rows[i][0].ToString();
                int index = rowName.IndexOf('-');
                int length = rowName.Length;
                //by yyan item8 20110620 del start
                //string tmp = rowName.Substring(index + 1, length - index - 1).Trim();
                //by yyan item8 20110620 del end
                //by yyan item8 20110620 add start
                string tmp = rowName.Substring(index + 2, length - index - 2).Trim();
                //by yyan item8 20110620 add end
                if (backlogY == tmp)
                {
                    SumFC[j] += float.Parse(ds.Tables[0].Rows[i][j].ToString());
                }
                else
                    SumFC[j] += 0;
            }
            string backlogBDGT;
            //by yyan item8 20110623 del start 
            //backlogBDGT = sql.getBackLogByOperation(str_operationID, getSegmentID(), str_productID, meeting.getyear(), "03", backlogY);
            //by yyan item8 20110623 del end 
            //by yyan item8 20110623 add start 
            if (backlogY.Equals(meeting.getyear().Substring(2, 2).Trim()))
            {
                backlogBDGT = sql.getBackLogByOperation(str_operationID, getSegmentID(), str_productID, meeting.getpreyear(), "03", backlogY, flagMoney);
            }
            else {
                backlogBDGT = sql.getBackLogByOperation(str_operationID, getSegmentID(), str_productID, meeting.getyear(), "03", backlogY, flagMoney);
       
 
            }//by yyan item8 20110623 add end

            if (backlogBDGT == "")
                backlogBDGT = "0";
            float backlogValueBDGT = float.Parse(backlogBDGT);
            if (backlogY.Trim() == meeting.getyear().Substring(2, 2).Trim() && ds_budget[0].Tables[0].Rows.Count != 0)
                SumBL[j] = float.Parse(ds_budget[0].Tables[0].Rows[0][j].ToString().Trim()) + backlogValueBDGT;
            else if (backlogY.Trim() == meeting.getnextyear().Substring(2, 2).Trim() && ds_budget[1].Tables[0].Rows.Count != 0)
                SumBL[j] = float.Parse(ds_budget[1].Tables[0].Rows[0][j].ToString().Trim()) + backlogValueBDGT;
            else
                SumBL[j] = 0;
        }

        for (int i = 0; i < ds.Tables[0].Columns.Count - 1; i++)
        {
            if (i == 0)
            {
                rows[0][0] = "20" + backlogY + "F/C";
                rows[1][0] = backlogY + "BDGT";
            }
            else
            {
                rows[0][i] = SumFC[i];
                rows[1][i] = SumBL[i];
            }
        }

        float tempFC = 0;
        float tempBDGT = 0;
        for (int i = 0; i < ds.Tables[0].Columns.Count - 1; i++)
        {
            tempFC += SumFC[i];
            tempBDGT += SumBL[i];
        }
        rows[0][ds.Tables[0].Columns.Count - 1] = tempFC;
        rows[1][ds.Tables[0].Columns.Count - 1] = tempBDGT;
        ds.Tables[0].Rows.InsertAt(rows[0], ds.Tables[0].Rows.Count);
        ds.Tables[0].Rows.InsertAt(rows[1], ds.Tables[0].Rows.Count);
    }

    //by yyan item8 20110623 del start 
    //protected void bindDataSource()
    //by yyan item8 20110623 del end 
    //by yyan item8 20110623 add start 
    protected void bindDataSource(int flagMoney)
    //by yyan item8 20110623 add end
    {
        //by yyan 20100608 Item22 add start
        int index = 0;
        //by yyan 20100608 Item22 add end
        //By Fxw 20110517 ITEM25 ADD Start
        string query_date = "SELECT CONVERT(varchar(15),SelectMeetingDate,23) FROM [SetSelectMeetingDate] where userid=" + Session["GeneralMarketingMgrID"].ToString();
        DataSet ds_date = helper.GetDataSet(query_date);
        if (ds_date.Tables[0].Rows.Count > 0 && !ds_date.Tables[0].Rows[0][0].ToString().Equals("") && ds_date.Tables[0].Rows[0][0].ToString() != null)
        {
            label_show.Text = "This report is related to the meeting date " + ds_date.Tables[0].Rows[0][0].ToString();
        }
        else
        {
            label_show.Text = "There is no meeting date selected!";
        }
        //By Fxw 20110517 ITEM25 ADD End
        DataSet dsPro = sql.getProductInfoBySegmentID(getSegmentID());
        DataSet dsOp = sql.getOperationBySegment(getSegmentID());
        if (dsPro.Tables[0].Rows.Count > 0 && dsOp.Tables[0].Rows.Count > 0)
        {
            //by ryzhang item49 20110520 ad start
            DataSet dsTotal = new DataSet();
            GridView gvTotal = new GridView();
            DataSet dsBookingsTotal = new DataSet();
            GridView gvBookingsTotal = new GridView();
            gvTotal = new GridView();
            web.setProperties(gvTotal);
            gvBookingsTotal = new GridView();
            web.setProperties(gvBookingsTotal);
            dsTotal = sql.getSalesDataTotalByOperation(dsPro, getSegmentID());
            //by yyan itemw146 20110914 add start
            if (meeting.getmonth() == "10" || meeting.getmonth() == "01" || meeting.getmonth() == "1")
            {
                if (dsTotal.Tables[0].Rows.Count == 0)
                {
                    DataRow[] rows = new DataRow[1];
                    rows[0] = dsTotal.Tables[0].NewRow();
                    if (meeting.getmonth() == "10")
                    {
                        rows[0][0] = meeting.getnextyear().Substring(2) + "  --" + meeting.getnextyear().Substring(2);
                    }
                    else
                    {
                        rows[0][0] = meeting.getyear().Substring(2) + "  --" + meeting.getyear().Substring(2);
                    }
                    for (int i = 0; i < dsTotal.Tables[0].Columns.Count - 1; i++)
                    {
                        rows[0][i + 1] = 0;
                    }
                    dsTotal.Tables[0].Rows.InsertAt(rows[0], 0);
                }
            }
            //by yyan itemw146 20110914 add end
            DataSet ds_budgetTotal = sql.getBookingsBudgetTotal(dsPro, getSegmentID());
            //by yyan itemw93 add start 
            DataSet dsTotal1 = dsTotal.Copy();
            GridView gvTotal1 = new GridView();
            web.setProperties(gvTotal1);
            //by yyan itemw93 add end   
            if (dsTotal.Tables[0].Rows.Count > 0)
            {
                //by yyan 20100608 Item22 del start
                //gvTotal = bindDataTotalByOperation(dsTotal, gvTotal, "<Br />Total " + getSegmentDec(getSegmentID()) + " (kEUR)", ds_budgetTotal);
                //by yyan 20100608 Item22 del end
                //by yyan 20100608 Item22 add start
                gvTotal = bindDataTotalByOperation(dsTotal, gvTotal, "<Br />", ds_budgetTotal,1);
                //by yyan itemw93 add start 
                gvTotal1 = bindDataTotalByOperation(dsTotal1, gvTotal1, "", ds_budgetTotal, 2);
                //by yyan itemw93 add end
                gvTotal.HeaderRow.Cells[0].Text = "Total " + getSegmentDec(getSegmentID()) + " (kEUR)";
                //by yyan 20100608 Item22 add end

            }
            dsBookingsTotal = sql.getBookingsDataTotaltoSales(dsPro, getSegmentID());
            if (dsBookingsTotal.Tables[0].Rows.Count > 0)
            {
                gvBookingsTotal = bindDataTotalByOperation(dsBookingsTotal, gvBookingsTotal, "", ds_budgetTotal,0);
            }
            /*foreach (GridViewRow row in gvTotal.Rows)
            {
                foreach (TableCell cell in row.Cells)
                {
                    int num;
                    if (int.TryParse(cell.Text, out num))
                    {
                        cell.Text = "0";
                    }
                }
            }
            foreach (GridViewRow row in gvBookingsTotal.Rows)
            {
                foreach (TableCell cell in row.Cells)
                {
                    int num;
                    if (int.TryParse(cell.Text, out num))
                    {
                        cell.Text = "0";
                    }
                }
            }*/
            //by ryzhang item49 20110520 add end
            TableRow tr = new TableRow();
            table_GrSales.Rows.Add(tr);
            table_GrSales.Visible = true;

            TableCell tc = new TableCell();
            tc.HorizontalAlign = HorizontalAlign.Left;
            tc.VerticalAlign = VerticalAlign.Top;

            List<string> operationIdsList = new List<string>();
            Dictionary<string, string> currList = null;
            for (int count = 0; count < dsOp.Tables[0].Rows.Count; count++)
            {
                operationIdsList.Add(dsOp.Tables[0].Rows[count][1].ToString().Trim());
            }
            if (operationIdsList != null && operationIdsList.Count > 0)
                currList = sql.getOperationCurrencys(operationIdsList);


            for (int count = 0; count < dsOp.Tables[0].Rows.Count; count++)
            {
                DataSet[] ds = new DataSet[dsOp.Tables[0].Rows.Count];
                //by yyan item8 20110623 add start 
                //DataSet[] dsEur = new DataSet[dsOp.Tables[0].Rows.Count];
                //DataSet[] dsBookingsEur = new DataSet[dsOp.Tables[0].Rows.Count];
                //GridView[] gvEur = new GridView[dsOp.Tables[0].Rows.Count];
               // GridView[] gvBookingsEur = new GridView[dsOp.Tables[0].Rows.Count];
               // gvBookingsEur[count] = new GridView();
               // web.setProperties(gvBookingsEur[count]);
               // gvEur[count] = new GridView();
               // web.setProperties(gvEur[count]);
                //by yyan item8 20110623 add end 
                GridView[] gv = new GridView[dsOp.Tables[0].Rows.Count];
                //by yyan itemw93 add start 
                DataSet[] ds1 = new DataSet[dsOp.Tables[0].Rows.Count];
                GridView[] gv1 = new GridView[dsOp.Tables[0].Rows.Count];
                gv1[count] = new GridView();
                web.setProperties(gv1[count]);
                //by yyan itemw93 add end 
                DataSet[] dsBookings = new DataSet[dsOp.Tables[0].Rows.Count];
                GridView[] gvBookings = new GridView[dsOp.Tables[0].Rows.Count];

                string str_OperationID = dsOp.Tables[0].Rows[count][1].ToString().Trim();
                string str_OperationName = dsOp.Tables[0].Rows[count][0].ToString().Trim();
                //string str_OperationCurrency = sql.getOperationCurrency(str_OperationID);
                string str_OperationCurrency = currList[str_OperationID];
                //New the instance to this controls
                gv[count] = new GridView();
                web.setProperties(gv[count]);
                gvBookings[count] = new GridView();
                web.setProperties(gvBookings[count]);
                //by yyan item8 20110623 del start 
                //ds[count] = sql.getSalesDataByOperation(dsPro, str_OperationID, getSegmentID());
                //DataSet ds_budget = sql.getBookingsBudgetTotalByOperation(dsPro, str_OperationID, getSegmentID());
                //by yyan item8 20110623 del end 
                //by yyan item8 20110623 add start 
                ds[count] = sql.getSalesDataByOperation(dsPro, str_OperationID, getSegmentID(), flagMoney);
                //by yyan itemw146 20110914 add start
                if (meeting.getmonth() == "10" || meeting.getmonth() == "01" || meeting.getmonth() == "1")
                {
                    if (ds[count].Tables[0].Rows.Count == 0)
                    {
                        DataRow[] rows = new DataRow[1];
                        rows[0] = ds[count].Tables[0].NewRow();
                        if (meeting.getmonth() == "10")
                        {
                            rows[0][0] = meeting.getnextyear().Substring(2) + "  --" + meeting.getnextyear().Substring(2);
                        }
                        else
                        {
                            rows[0][0] = meeting.getyear().Substring(2) + "  --" + meeting.getyear().Substring(2);
                        }
                        for (int i = 0; i < ds[count].Tables[0].Columns.Count - 1; i++)
                        {
                            rows[0][i + 1] = 0;
                        }
                        ds[count].Tables[0].Rows.InsertAt(rows[0], 0);
                    }
                }
                //by yyan itemw146 20110914 add end
                ds1[count] = ds[count].Copy(); 
                //dsEur[count] = sql.getSalesDataByOperation(dsPro, str_OperationID, getSegmentID(), 1);
                DataSet ds_budget = sql.getBookingsBudgetTotalByOperation(dsPro, str_OperationID, getSegmentID(),flagMoney);
                //DataSet ds_budgetEur = sql.getBookingsBudgetTotalByOperation(dsPro, str_OperationID, getSegmentID(), 1);

                //by yyan item8 20110623 add end
                if (ds[count].Tables[0].Rows.Count > 0)
                {
                    //by yyan 20100608 Item22 del start
                    //gv[count] = bindDataByOperation(ds[count], gv[count], str_OperationID, "<br />" + str_OperationName + " (k" + str_OperationCurrency + ")", ds_budget);
                    //by yyan 20100608 Item22 del end
                    //by yyan 20100608 Item22 add start
                    gv[count] = bindDataByOperation(ds[count], gv[count], str_OperationID, "<br />", ds_budget,flagMoney,1);
                    gv1[count] = bindDataByOperation(ds1[count], gv1[count], str_OperationID, "", ds_budget, flagMoney, 2);
                    //by yyan item8 20110623 add start 
                    //gvEur[count] = bindDataByOperation(dsEur[count], gvEur[count], str_OperationID, "<br />", ds_budgetEur, 1);
                    //by yyan item8 20110623 add end 
                    gv[count].HeaderRow.Cells[0].Text = str_OperationName + " (k" + str_OperationCurrency + ")";
                    addStyle(gv[count], index);
                    //by yyan 20100608 Item22 add end
                    //by ryzhang item49 20110526 modify start
                    //by ryzhang item49 20110520 add start
                    /*for (int i = 0; i < gvTotal.Rows.Count; i++)
                    {
                        for (int j = 0; j < gvTotal.Rows[i].Cells.Count; j++)
                        {
                            if (j == 0)
                            {
                                //by yyan item8 20110623 del start 
                                //gvTotal.Rows[i].Cells[j].Text = gv[count].Rows[i].Cells[j].Text;
                                //by yyan item8 20110623 del end 
                                //by yyan item8 20110623 add start 
                                gvTotal.Rows[i].Cells[j].Text = gvEur[count].Rows[i].Cells[j].Text;
                                //by yyan item8 20110623 add end 
                            }
                            else
                            {
                                try
                                {
                                    //by yyan item8 20110623 del start 
                                    //string sum = gv[count].Rows[i].Cells[j].Text.ToString() == "" ? "0" : gv[count].Rows[i].Cells[j].Text.ToString();
                                    //by yyan item8 20110623 del end 
                                    //by yyan item8 20110623 add start 
                                    string sum = gvEur[count].Rows[i].Cells[j].Text.ToString() == "" ? "0" : gvEur[count].Rows[i].Cells[j].Text.ToString();
                                    //by yyan item8 20110623 add end 
                                    string sumNum = gvTotal.Rows[i].Cells[j].Text.ToString() == "" ? "0" : gvTotal.Rows[i].Cells[j].Text.ToString();
                                    int sum_d;
                                    if (int.TryParse(sum, out sum_d))
                                    {
                                        gvTotal.Rows[i].Cells[j].Text = (int.Parse(sumNum) + int.Parse(sum)) + "";
                                    }
                                    else
                                    {
                                        continue;
                                    }
                                }
                                catch (Exception)
                                {
                                }
                            }

                        }
                    }*/
                    //by ryzhang item49 20110520 add end
                    //by ryzhang item49 20110526 modify end
                    tc.Controls.Add(gv[count]);
                    addStyle(gv1[count], index);
                    tc.Controls.Add(gv1[count]);
                }
                //by yyan item8 20110623 del start 
                //dsBookings[count] = sql.getBookingsDataTotalByOperationtoSales(dsPro, str_OperationID, getSegmentID());
                //by yyan item8 20110623 del end 
                //by yyan item8 20110623 add start 
                dsBookings[count] = sql.getBookingsDataTotalByOperationtoSales(dsPro, str_OperationID, getSegmentID(), flagMoney);
                //dsBookingsEur[count] = sql.getBookingsDataTotalByOperationtoSales(dsPro, str_OperationID, getSegmentID(), 1);
                //by yyan item8 20110623 add end

                if (dsBookings[count].Tables[0].Rows.Count > 0)
                {
                    //by yyan item8 20110623 del start 
                    //gvBookings[count] = bindDataByOperation(dsBookings[count], gvBookings[count], str_OperationID, "", ds_budget);
                    //by yyan item8 20110623 del end 
                    //by yyan item8 20110623 add start 
                    gvBookings[count] = bindDataByOperation(dsBookings[count], gvBookings[count], str_OperationID, "", ds_budget,flagMoney,0);
                    //gvBookingsEur[count] = bindDataByOperation(dsBookingsEur[count], gvBookingsEur[count], str_OperationID, "", ds_budgetEur, 1);
                    //by yyan item8 20110623 add end

                    //by yyan 20100608 Item22 add start
                    addStyle(gvBookings[count], index);
                    //by yyan 20100608 Item22 add end
                    //by ryzhang item49 20110526 modify start
                    //by ryzhang item49 20110520 add start
                    /*for (int i = 0; i < gvBookingsTotal.Rows.Count; i++)
                    {
                        for (int j = 0; j < gvBookingsTotal.Rows[i].Cells.Count; j++)
                        {
                            if (j == 0)
                            {
                                //by yyan item8 20110623 del start 
                                //gvBookingsTotal.Rows[i].Cells[j].Text = gvBookings[count].Rows[i].Cells[j].Text;
                                //by yyan item8 20110623 del end 
                                //by yyan item8 20110623 add start 
                                gvBookingsTotal.Rows[i].Cells[j].Text = gvBookingsEur[count].Rows[i].Cells[j].Text;
                                //by yyan item8 20110623 add end 
                            }
                            else
                            {
                                try
                                {
                                    //by yyan item8 20110623 del start 
                                    //string sum = gvBookingsEur[count].Rows[i].Cells[j].Text.ToString() == "" ? "0" : gvBookings[count].Rows[i].Cells[j].Text.ToString();
                                    //by yyan item8 20110623 del end 
                                    //by yyan item8 20110623 add start 
                                    string sum = gvBookingsEur[count].Rows[i].Cells[j].Text.ToString() == "" ? "0" : gvBookingsEur[count].Rows[i].Cells[j].Text.ToString();
                                    //by yyan item8 20110623 add end 
                                    string sumNum = gvBookingsTotal.Rows[i].Cells[j].Text.ToString() == "" ? "0" : gvBookingsTotal.Rows[i].Cells[j].Text.ToString();
                                    int sum_d;
                                    if (int.TryParse(sum, out sum_d))
                                    {
                                        gvBookingsTotal.Rows[i].Cells[j].Text = (int.Parse(sumNum) + int.Parse(sum)) + "";
                                    }
                                    else
                                    {
                                        continue;
                                    }
                                }
                                catch (Exception)
                                {
                                }
                            }
                        }
                    }*/
                    //by ryzhang item49 20110520 add end
                    //by ryzhang item49 20110526 modify end
                    tc.Controls.Add(gvBookings[count]);
                }
                if (ds[count].Tables[0].Rows.Count > 0)
                {
                    //by yyan 20100608 Item22 add start
                    index++;
                    //by yyan 20100608 Item22 add end
                }
            }
            //by ryzhang item49 20110520 del start
            //DataSet dsTotal = new DataSet();
            //GridView gvTotal = new GridView();
            //DataSet dsBookingsTotal = new DataSet();
            //GridView gvBookingsTotal = new GridView();
            //gvTotal = new GridView();
            //web.setProperties(gvTotal);
            //gvBookingsTotal = new GridView();
            //web.setProperties(gvBookingsTotal);
            //dsTotal = sql.getSalesDataTotalByOperation(dsPro, getSegmentID());
            //DataSet ds_budgetTotal = sql.getBookingsBudgetTotal(dsPro, getSegmentID());
            //if (dsTotal.Tables[0].Rows.Count > 0)
            //{
            //    gvTotal = bindDataTotalByOperation(dsTotal, gvTotal, "<Br />Total " + getSegmentDec(getSegmentID()) + " (kEUR)", ds_budgetTotal);
            //    tc.Controls.Add(gvTotal);
            //}
            //dsBookingsTotal = sql.getBookingsDataTotaltoSales(dsPro, getSegmentID());
            //if (dsBookingsTotal.Tables[0].Rows.Count > 0)
            //{
            //    gvBookingsTotal = bindDataTotalByOperation(dsBookingsTotal, gvBookingsTotal, "", ds_budgetTotal);
            //    tc.Controls.Add(gvBookingsTotal);
            //}
            //by ryzhang item49 20110520 del end
            //by ryzhang item49 20110520 add start
            if (dsTotal.Tables[0].Rows.Count > 0)
            {
                //by yyan 20100608 Item22 add start
                addStyle(gvTotal, index);
                //by yyan 20100608 Item22 add end
                tc.Controls.Add(gvTotal);
                //by yyan itemw93 add start 
                addStyle(gvTotal1, index);
                tc.Controls.Add(gvTotal1);
                //by yyan itemw93 add end
            }
            if (dsBookingsTotal.Tables[0].Rows.Count > 0)
            {
                //by yyan 20100608 Item22 add start
                addStyle(gvBookingsTotal, index);
                //by yyan 20100608 Item22 add end
                tc.Controls.Add(gvBookingsTotal);
            }
            //by ryzhang item49 20110520 add end
            tr.Controls.Add(tc);
        }
    }

    protected GridView bindDataTotalByOperation(DataSet ds, GridView gv, string header, DataSet ds_budget,int flag)
    {
        if (ds != null && ds.Tables[0].Rows.Count > 0)
        {
            gv.Visible = true;
            gv.AutoGenerateColumns = false;
            gv.Width = Unit.Pixel(600);
            gv.Columns.Clear();

            //Calculate the total column of next year.
            ds.Tables[0].Columns.Add("Total");
            if (ds.Tables[0].Rows[0][0] != DBNull.Value)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    float tmp = 0;
                    for (int count2 = 1; count2 < ds.Tables[0].Columns.Count - 1; count2++)
                    {
                        tmp += float.Parse(dr[count2].ToString());
                    }
                    dr["Total"] = tmp.ToString();
                }
            }

            for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
            {
                BoundField bf = new BoundField();

                bf.DataField = ds.Tables[0].Columns[i].ColumnName.ToString();
                bf.HeaderText = ds.Tables[0].Columns[i].Caption.ToString();
                bf.ItemStyle.HorizontalAlign = HorizontalAlign.Right;
                bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Right;

                if (i == 0)
                {
                    bf.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
                    bf.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
                    bf.ItemStyle.Width = 100;
                }

                gv.Columns.Add(bf);
            }
            gv.Caption = header;
            gv.CaptionAlign = TableCaptionAlign.Left;
            gv.DataSource = ds;

            if (flag != 0)
            {
                DataRow[] rows = new DataRow[2];
                rows[0] = ds.Tables[0].NewRow();
                rows[1] = ds.Tables[0].NewRow();

                float[] Sum1 = new float[20];
                float[] Sum2 = new float[20];
                for (int j = 1; j < ds.Tables[0].Columns.Count - 1; j++)
                {
                    string str_productAbbr = ds.Tables[0].Columns[j].ColumnName.Trim();
                    string str_productID = getProductIDBySegmentIDByProductAbbr(getSegmentID(), str_productAbbr);

                    string str_bl = sql.getBackLog(getSegmentID(), str_productID, meeting.getyear(), meeting.getmonth(), meeting.getyear().Substring(2, 2).Trim());
                    if (str_bl == "")
                        str_bl = "0";
                    Sum1[j] = float.Parse(str_bl);
                    string str_blnext = sql.getBackLog(getSegmentID(), str_productID, meeting.getyear(), meeting.getmonth(), meeting.getnextyear().Substring(2, 2).Trim());
                    if (str_blnext == "")
                        str_blnext = "0";
                    Sum2[j] = float.Parse(str_blnext);

                }

                for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
                {
                    if (i == 0)
                    {
                        rows[0][0] = meeting.getyear().Substring(2, 2) + "B/L";
                        rows[1][0] = meeting.getnextyear().Substring(2, 2) + "B/L";
                    }
                    else
                    {
                        rows[0][i] = Sum1[i];
                        rows[1][i] = Sum2[i];
                    }
                }

                float tempY = 0;
                float tempNY = 0;
                for (int i = 0; i < ds.Tables[0].Columns.Count - 1; i++)
                {
                    tempY += Sum1[i];
                    tempNY += Sum2[i];
                }
                rows[0][ds.Tables[0].Columns.Count - 1] = tempY;
                rows[1][ds.Tables[0].Columns.Count - 1] = tempNY;
                ds.Tables[0].Rows.InsertAt(rows[0], 0);
                ds.Tables[0].Rows.InsertAt(rows[1], 1);

                getSalesTotalForecast(ds, meeting.getyear().Substring(2, 2));
                getSalesTotalForecast(ds, meeting.getnextyear().Substring(2, 2));
            }
            else
            {
                DataRow drSum = ds.Tables[0].NewRow();
                DataRow[] rows = ds.Tables[0].Select();

                float[] Sum = new float[20];
                for (int j = 1; j < ds.Tables[0].Columns.Count - 1; j++)
                {
                    if (ds_budget.Tables[0].Rows.Count != 0)
                        Sum[j] = float.Parse(ds_budget.Tables[0].Rows[0][j].ToString().Trim());
                    else
                        Sum[j] = 0;
                }

                float tmp = 0;
                for (int i = 0; i < ds.Tables[0].Columns.Count - 1; i++)
                {
                    if (i == 0)
                    {
                        drSum[0] = "Bookings " + meeting.getnextyear().Substring(2, 2) + " B";
                    }
                    else
                    {
                        drSum[i] = Sum[i];
                        tmp += Sum[i];
                    }
                }
                drSum[ds.Tables[0].Columns.Count - 1] = tmp.ToString().Trim();
                ds.Tables[0].Rows.InsertAt(drSum, ds.Tables[0].Rows.Count);
            }
            //by yyan itemw93 add start 
            if (flag != 0)
            {
                bool bz = false;
                int k = 0;
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    if (ds.Tables[0].Rows[i][0].ToString().Contains("F/C"))
                    {
                        bz = true;
                        k = i;
                        break;
                    }
                }
                if (bz)
                {
                    if (flag == 1)
                    {
                        for (int i = ds.Tables[0].Rows.Count - 1; i >= k; i--)
                        {
                            ds.Tables[0].Rows[i].Delete();
                        }
                    }
                    else if (flag == 2)
                    {
                        for (int i = k - 1; i >= 0; i--)
                        {
                            ds.Tables[0].Rows[i].Delete();
                        }
                    }
                }
            }
            //by yyan itemw93 add end 
            gv.DataBind();
        }
        else
            gv.Visible = false;
        return gv;
    }

    protected void getSalesTotalForecast(DataSet ds, string backlogY)
    {
        DataSet[] ds_budget = new DataSet[2];
        DataSet dsPro = sql.getProductInfoBySegmentID(getSegmentID());
        ds_budget[0] = sql.getSalesBudget(dsPro, getSegmentID(), meeting.getpreyear(), meeting.getyear().Substring(2, 2));
        ds_budget[1] = sql.getSalesBudget(dsPro, getSegmentID(), meeting.getyear(), meeting.getnextyear().Substring(2, 2));

        DataRow[] rows = new DataRow[2];
        rows[0] = ds.Tables[0].NewRow();
        rows[1] = ds.Tables[0].NewRow();

        float[] SumFC = new float[20];
        float[] SumBL = new float[20];
        for (int j = 1; j < ds.Tables[0].Columns.Count - 1; j++)
        {
            string str_productAbbr = ds.Tables[0].Columns[j].ColumnName.Trim();
            string str_productID = getProductIDBySegmentIDByProductAbbr(getSegmentID(), str_productAbbr);
            string backlog;
            backlog = sql.getBackLog(getSegmentID(), str_productID, meeting.getyear(), meeting.getmonth(), backlogY);
            if (backlog == "")
                backlog = "0";
            float backlogValue = float.Parse(backlog);

            SumFC[j] = backlogValue;
            for (int i = 2; i < ds.Tables[0].Rows.Count; i++)
            {
                string rowName = ds.Tables[0].Rows[i][0].ToString();
                int index = rowName.IndexOf('-');
                int length = rowName.Length;
                //by yyan item8 20110620 del start
                //string tmp = rowName.Substring(index + 1, length - index - 1).Trim();
                //by yyan item8 20110620 del end
                //by yyan item8 20110620 add start
                string tmp = rowName.Substring(index + 2, length - index - 2).Trim();
                //by yyan item8 20110620 add end
                if (backlogY == tmp)
                {
                    SumFC[j] += float.Parse(ds.Tables[0].Rows[i][j].ToString());
                }
                else
                    SumFC[j] += 0;
            }
            string backlogBDGT;
            if (backlogY.Equals(meeting.getyear().Substring(2, 2).Trim()))
            {
                backlogBDGT = sql.getBackLog(getSegmentID(), str_productID, meeting.getpreyear(), "03", backlogY);
            }
            else
            {
                backlogBDGT = sql.getBackLog(getSegmentID(), str_productID, meeting.getyear(), "03", backlogY);
            } if (backlogBDGT == "")
                backlogBDGT = "0";
            float backlogValueBDGT = float.Parse(backlogBDGT);
            if (backlogY.Trim() == meeting.getyear().Substring(2, 2).Trim() && ds_budget[0].Tables[0].Rows.Count != 0)
                SumBL[j] = float.Parse(ds_budget[0].Tables[0].Rows[0][j].ToString().Trim()) + backlogValueBDGT;
            else if (backlogY.Trim() == meeting.getnextyear().Substring(2, 2).Trim() && ds_budget[1].Tables[0].Rows.Count != 0)
                SumBL[j] = float.Parse(ds_budget[1].Tables[0].Rows[0][j].ToString().Trim()) + backlogValueBDGT;
            else
                SumBL[j] = 0;
        }
        for (int i = 0; i < ds.Tables[0].Columns.Count - 1; i++)
        {
            if (i == 0)
            {
                rows[0][0] = "20" + backlogY + "F/C";
                rows[1][0] = backlogY + "BDGT";
            }
            else
            {
                rows[0][i] = SumFC[i];
                rows[1][i] = SumBL[i];
            }
        }

        float tempFC = 0;
        float tempBDGT = 0;
        for (int i = 0; i < ds.Tables[0].Columns.Count - 1; i++)
        {
            tempFC += SumFC[i];
            tempBDGT += SumBL[i];
        }
        rows[0][ds.Tables[0].Columns.Count - 1] = tempFC;
        rows[1][ds.Tables[0].Columns.Count - 1] = tempBDGT;
        ds.Tables[0].Rows.InsertAt(rows[0], ds.Tables[0].Rows.Count);
        ds.Tables[0].Rows.InsertAt(rows[1], ds.Tables[0].Rows.Count);
    }

    /* One page to another page */

    protected void lbtn_grSales_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/MarketingMgr/MarketingMgrGrossSales.aspx?SegmentID=" + getSegmentID());
    }

    protected void lbtn_grBKG_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/MarketingMgr/MarketingMgrGrossBookings.aspx?SegmentID=" + getSegmentID());
    }

    protected void lbtn_opSales_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/MarketingMgr/MarketingMgrOperationSales.aspx?SegmentID=" + getSegmentID());
    }

    protected void lbtn_opBKG_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/MarketingMgr/MarketingMgrOperationalBookings.aspx?SegmentID=" + getSegmentID());
    }

    protected void lbtn_SO_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/MarketingMgr/MarketingMgrBookingBySalesOrg.aspx?SegmentID=" + getSegmentID());
    }

    #region
    public override void VerifyRenderingInServerForm(Control control)
    {
        // Confirms that an HtmlForm control is rendered for
    }

    protected void btn_export_Click(object sender, EventArgs e)
    {
        TableRow tr5 = new TableRow();
        table_GrSales.Rows.Add(tr5);
        TableCell tc5 = new TableCell();
        tc5.Text = label_show.Text;
        tr5.Controls.Add(tc5);
        
        //TableRow tr1 = new TableRow();
        //table_GrSales.Rows.Add(tr1);
        TableRow tr = new TableRow();
        table_GrSales.Rows.Add(tr);
        table_GrSales.Visible = true;
        TableCell tc = new TableCell();
        tc.BorderColor = System.Drawing.Color.Black;
        tc.Font.Bold = true;
        tc.BorderWidth = 1;
        tc.BackColor = System.Drawing.Color.LawnGreen;
        if (lblEUR.Text == "")
        {
            tc.Text = "GR-Sales(" + getSegmentDec(getSegmentID()) + ")" + " Values in Local";
        }
        else
        {
            tc.Text = "GR-Sales(" + getSegmentDec(getSegmentID()) + ")" + " Values in EURO";
        }
        tr.Controls.Add(tc);
        //by yyan item8 20110623 add start 
        if (lblEUR.Text == "")
        { bindDataSource(0); }
        else
        { bindDataSource(1); }
        //by yyan item8 20110623 add end 
        //by yyan item8 20110623 del start 
        //bindDataSource();
        //by yyan item8 20110623 del end 
        cf.ToExcel(table_GrSales, "GR-Sales(" + getSegmentDec(getSegmentID()) + ").xls");
    }
    #endregion

    //by yyan 20100608 Item22 add start
    private void addStyle(GridView gv, int index)
    {
        gv.Style.Clear();
        gv.Style.Add("border", "#000 solid 1px");
        gv.Style.Add("border-collapse", "collapse");
        gv.Style.Add("font-size", "12px");
        gv.HeaderRow.Style.Add("background", "#000");
        for (int i = 0; i < gv.Rows.Count; i++)
        {
            //by yyan item8 20110704 add start
            gvUpdateCell(gv, i);
            //by yyan item8 20110704 add end
            foreach (TableCell cell in gv.Rows[i].Cells)
            {
                cell.Style.Clear();
                if (index % 2 == 0)
                {
                    cell.Style.Add("background", "#FF9");
                }
                else
                {
                    cell.Style.Add("background", "#ccffff");
                }
            }
        }
    }
    //by yyan 20100608 Item22 add end
    //by yyan item8 20110623 add start 
    protected void btnEUR_Click(object sender, EventArgs e)
    {
        bindDataSource(1);
        lblEUR.Text = "The amount is calculated based on EUR.";
    }
    //by yyan item8 20110623 add end 

    //by yyan item8 20110704 add start
    protected void gvUpdateCell(GridView gvWindow, int i)
    {
        if (meeting.getmonth() == "3" || meeting.getmonth() == "7")
        {
            if (gvWindow.Rows[i].Cells[0].Text.Trim().Equals(meeting.getnextyear().Trim() + "F/C"))
            {
                for (int j = 1; j < gvWindow.Rows[i].Cells.Count; j++)
                {
                    gvWindow.Rows[i].Cells[j].Text = "n.a.";
                }
            }
        }
        //else if (meeting.getmonth() == "1")
        //{
        //    if (gvWindow.Rows[i].Cells[0].Text.Trim().Substring(0, 2).Equals(meeting.getyear().Substring(2, 2)) ||
        //        gvWindow.Rows[i].Cells[0].Text.Trim().Substring(0, 4).Equals(meeting.getyear().Trim()))
        //    {
        //        for (int j = 1; j < gvWindow.Rows[i].Cells.Count; j++)
        //        {
        //            gvWindow.Rows[i].Cells[j].Text = "n.a.";
        //        }
        //    }
        //}

    }
    //by yyan item8 20110704 add end 
    //by yyan itemw90 20110805 add start 
    protected void btnLocal_Click(object sender, EventArgs e)
    {
        bindDataSource(0);
        lblEUR.Text = "";
    }
    //by yyan itemw90 20110805 add end 
}
