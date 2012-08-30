/*
 * File Name   : AdminOperationalSales.aspx.cs
 * 
 * Description : Get sales data by salesorg by operation
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

public partial class Admin_AdminOperationSales : System.Web.UI.Page
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
        if (getRoleID(getRole()) == "0" || getRoleID(getRole()) == "5")
        {

        }
        else
            Response.Redirect("~/AccessDenied.aspx");

        if (!IsPostBack)
        {
            //by ryzhang item49 20110519 del start 
            //meeting.setDate();
            //by ryzhang item49 20110519 del end 
            //by ryzhang item49 20110519 add start 
            meeting.setSelectDate(Session["AdministratorID"].ToString());
            //by ryzhang item49 20110519 add end 
            ddlist_operation.Items.Clear();
            bindOperation(getSegmentID());
            btn_export.Visible = false;
            //by yyan item8 20110614 add start 
            btnEUR.Visible = false;
            //by yyan item8 20110614 add end 
            //by yyan itemw90 20110805 add start 
            btnLocal.Visible = false;
            //by yyan itemw90 20110805 add end

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


    /* Conditions And Description Start */
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

    protected void bindOperation(string segmentID)
    {
        DataSet ds = sql.getOperationBySegment(segmentID);
        if (ds != null)
        {
            DataTable dt = ds.Tables[0];
            int count = dt.Rows.Count;
            int index = 0;
            while (index < count)
            {
                ListItem li = new ListItem(dt.Rows[index][0].ToString().Trim(), dt.Rows[index][1].ToString().Trim());
                ddlist_operation.Items.Add(li);
                index++;
            }
            ddlist_operation.SelectedIndex = 0;
            ddlist_operation.Enabled = true;
            btn_search.Enabled = true;
        }
        else
        {
            ddlist_operation.Enabled = false;
            ddlist_operation.Items.Add("");
            btn_search.Enabled = false;
        }
    }

    protected void btn_search_Click(object sender, EventArgs e)
    {
        //By Fxw 20110517 ITEM25 ADD Start
        string query_date = "SELECT CONVERT(varchar(15),SelectMeetingDate,23) FROM [SetSelectMeetingDate] where userid=" + Session["AdministratorID"].ToString();
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
        label_description.Visible = true;
        label_description.Text = getSegmentDec(getSegmentID()) + " - SALES BY OPERATION BY SALES ORGANIZATION";
        string str_currency = sql.getOperationCurrency(ddlist_operation.SelectedItem.Value.Trim());
        if (str_currency != "")
        {
            
            label_currency.Text = "k" + str_currency;
            //by yyan item8 20110614 del start 
            //bindDataSource();
            //by yyan item8 20110614 del end 
            //by yyan item8 20110614 add start 
            bindDataSource(0);
            //by yyan item8 20110614 add end 
             
        }
        else
            label_currency.Text = info.notCurrency("Operation:" + ddlist_operation.SelectedItem.Text);
        btn_export.Visible = true;
        //by yyan item8 20110614 add start 
        btnEUR.Visible = true;
        lblEUR.Text = "";
        //by yyan item8 20110614 add end
        //by yyan itemw90 20110805 add start 
        btnLocal.Visible = true;
        btnLocal.Text = label_currency.Text;
        //by yyan itemw90 20110805 add end 
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

    //by yyan item8 20110614 del start
    //protected GridView bindDataBySalesOrgByOperation(DataSet ds, GridView gv, string header, string salesOrgID)
    //by yyan item8 20110614 del end
    //by yyan item8 20110614 add start
    protected GridView bindDataBySalesOrgByOperation(DataSet ds, GridView gv, string header, string salesOrgID, int flagMoney,bool flag)
    //by yyan item8 20110614 add end
    {
        if (ds != null)
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

            DataRow[] rows = new DataRow[2];
            rows[0] = ds.Tables[0].NewRow();
            rows[1] = ds.Tables[0].NewRow();

            float[] Sum1 = new float[20];
            float[] Sum2 = new float[20];
            for (int j = 1; j < ds.Tables[0].Columns.Count - 1; j++)
            {
                string str_productAbbr = ds.Tables[0].Columns[j].ColumnName.Trim();
                string str_productID = getProductIDBySegmentIDByProductAbbr(getSegmentID(), str_productAbbr);
                if (salesOrgID != "")
                {
                    //by yyan item8 20110614 del start
                    //string str_bl = sql.getBackLogBySalesOrgByOperation(ddlist_operation.SelectedItem.Value, getSegmentID(), str_productID, meeting.getyear(), meeting.getmonth(), salesOrgID, meeting.getyear().Substring(2, 2).Trim());
                    //by yyan item8 20110614 del end
                    //by yyan item8 20110614 add start
                    string str_bl = sql.getBackLogBySalesOrgByOperation(ddlist_operation.SelectedItem.Value, getSegmentID(), str_productID, meeting.getyear(), meeting.getmonth(), salesOrgID, meeting.getyear().Substring(2, 2).Trim(), flagMoney);
                    //by yyan item8 20110614 add end
                    if (str_bl == "")
                        str_bl = "0";
                    Sum1[j] = float.Parse(str_bl);
                    //by yyan item8 20110614 del start
                    //string str_blnext = sql.getBackLogBySalesOrgByOperation(ddlist_operation.SelectedItem.Value, getSegmentID(), str_productID, meeting.getyear(), meeting.getmonth(), salesOrgID, meeting.getnextyear().Substring(2, 2).Trim());
                    //by yyan item8 20110614 del end
                    //by yyan item8 20110614 add start
                    string str_blnext = sql.getBackLogBySalesOrgByOperation(ddlist_operation.SelectedItem.Value, getSegmentID(), str_productID, meeting.getyear(), meeting.getmonth(), salesOrgID, meeting.getnextyear().Substring(2, 2).Trim(), flagMoney);
                    //by yyan item8 20110614 add end
                    if (str_blnext == "")
                        str_blnext = "0";
                    Sum2[j] = float.Parse(str_blnext);
                }
                else
                {
                    //by yyan item8 20110614 del start
                    //string str_bl = sql.getBackLogByOperation(ddlist_operation.SelectedItem.Value, getSegmentID(), str_productID, meeting.getyear(), meeting.getmonth(), meeting.getyear().Substring(2, 2).Trim());
                    //by yyan item8 20110614 del end
                    //by yyan item8 20110614 add start
                    string str_bl = sql.getBackLogByOperation(ddlist_operation.SelectedItem.Value, getSegmentID(), str_productID, meeting.getyear(), meeting.getmonth(), meeting.getyear().Substring(2, 2).Trim(), flagMoney);
                    //by yyan item8 20110614 add end
                    if (str_bl == "")
                        str_bl = "0";
                    Sum1[j] = float.Parse(str_bl);
                    //by yyan item8 20110614 del start
                    //string str_blnext = sql.getBackLogByOperation(ddlist_operation.SelectedItem.Value, getSegmentID(), str_productID, meeting.getyear(), meeting.getmonth(), meeting.getnextyear().Substring(2, 2).Trim());
                    //by yyan item8 20110614 del end
                    //by yyan item8 20110614 add start
                    string str_blnext = sql.getBackLogByOperation(ddlist_operation.SelectedItem.Value, getSegmentID(), str_productID, meeting.getyear(), meeting.getmonth(), meeting.getnextyear().Substring(2, 2).Trim(),flagMoney);
                    //by yyan item8 20110614 add end
                    if (str_blnext == "")
                        str_blnext = "0";
                    Sum2[j] = float.Parse(str_blnext);
                }
            }

            for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
            {
                if (i == 0)
                {
                    if (meeting.getmonth() == "10")
                        rows[0][0] = meeting.getyear().Substring(2, 2) + "Actual Sales";
                    else
                        rows[0][0] = meeting.getyear().Substring(2, 2) + "Actual Sales+B/L";
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

            if (salesOrgID != "")
            {
                //by yyan item8 20110614 del start
                //getSalesForecast(ds, salesOrgID, meeting.getyear().Substring(2, 2));
                //getSalesForecast(ds, salesOrgID, meeting.getnextyear().Substring(2, 2));
                //by yyan item8 20110614 del end
                //by yyan item8 20110614 add start
                getSalesForecast(ds, salesOrgID, meeting.getyear().Substring(2, 2), flagMoney);
                getSalesForecast(ds, salesOrgID, meeting.getnextyear().Substring(2, 2), flagMoney);
                //by yyan item8 20110614 add end
            }
            else
            {
                //by yyan item8 20110614 del start
                //getSalesForecast(ds, "", meeting.getyear().Substring(2, 2));
                //getSalesForecast(ds, "", meeting.getnextyear().Substring(2, 2));
                //by yyan item8 20110614 del end
                //by yyan item8 20110614 add start
                getSalesForecast(ds, "", meeting.getyear().Substring(2, 2), flagMoney);
                getSalesForecast(ds, "", meeting.getnextyear().Substring(2, 2), flagMoney);
                //by yyan item8 20110614 add end
            }

            //by yyan itemw93 add start 
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
                if (flag)
                {
                    for (int i = ds.Tables[0].Rows.Count-1; i >= k; i--)
                    {
                        ds.Tables[0].Rows[i].Delete();
                    }
                }
                else {
                    for (int i = k-1; i >= 0; i--)
                    {
                        ds.Tables[0].Rows[i].Delete();
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

    //by yyan item8 20110614 del start
    //protected void getSalesForecast(DataSet ds, string salesOrgID, string backlogY)
    //by yyan item8 20110614 del end
    //by yyan item8 20110614 add start
    protected void getSalesForecast(DataSet ds, string salesOrgID, string backlogY, int flagMoney)
    //by yyan item8 20110614 add end
    {
        DataSet[] ds_budget = new DataSet[2];
        DataSet dsPro = sql.getProductInfoBySegmentID(getSegmentID());
        if (salesOrgID != "")
        {
            //by yyan item8 20110614 del start
            //ds_budget[0] = sql.getSalesBudgetBySalesOrgByOperation(dsPro, salesOrgID, ddlist_operation.SelectedItem.Value, getSegmentID(), meeting.getpreyear(), meeting.getyear().Substring(2, 2));
            //ds_budget[1] = sql.getSalesBudgetBySalesOrgByOperation(dsPro, salesOrgID, ddlist_operation.SelectedItem.Value, getSegmentID(), meeting.getyear(), meeting.getnextyear().Substring(2, 2));
            //by yyan item8 20110614 del end
            //by yyan item8 20110614 add start
            ds_budget[0] = sql.getSalesBudgetBySalesOrgByOperation(dsPro, salesOrgID, ddlist_operation.SelectedItem.Value, getSegmentID(), meeting.getpreyear(), meeting.getyear().Substring(2, 2),flagMoney);
            ds_budget[1] = sql.getSalesBudgetBySalesOrgByOperation(dsPro, salesOrgID, ddlist_operation.SelectedItem.Value, getSegmentID(), meeting.getyear(), meeting.getnextyear().Substring(2, 2),flagMoney);
            //by yyan item8 20110614 add end
        }
        else
        {
            //by yyan item8 20110614 del start
            //ds_budget[0] = sql.getSalesBudgetByOperation(dsPro, ddlist_operation.SelectedItem.Value, getSegmentID(), meeting.getpreyear(), meeting.getyear().Substring(2, 2));
            //ds_budget[1] = sql.getSalesBudgetByOperation(dsPro, ddlist_operation.SelectedItem.Value, getSegmentID(), meeting.getyear(), meeting.getnextyear().Substring(2, 2));
            //by yyan item8 20110614 del end
            //by yyan item8 20110614 add start
            ds_budget[0] = sql.getSalesBudgetByOperation(dsPro, ddlist_operation.SelectedItem.Value, getSegmentID(), meeting.getpreyear(), meeting.getyear().Substring(2, 2), flagMoney);
            ds_budget[1] = sql.getSalesBudgetByOperation(dsPro, ddlist_operation.SelectedItem.Value, getSegmentID(), meeting.getyear(), meeting.getnextyear().Substring(2, 2), flagMoney);
            //by yyan item8 20110614 add end
        }

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
            if (salesOrgID != "")
                //by yyan item8 20110614 del start
                //backlog = sql.getBackLogBySalesOrgByOperation(ddlist_operation.SelectedItem.Value, getSegmentID(), str_productID, meeting.getyear(), meeting.getmonth(), salesOrgID, backlogY);
                //by yyan item8 20110614 del end
                //by yyan item8 20110614 add start
                backlog = sql.getBackLogBySalesOrgByOperation(ddlist_operation.SelectedItem.Value, getSegmentID(), str_productID, meeting.getyear(), meeting.getmonth(), salesOrgID, backlogY, flagMoney);
                //by yyan item8 20110614 add end
            else
                //by yyan item8 20110614 del start
                //backlog = sql.getBackLogByOperation(ddlist_operation.SelectedItem.Value, getSegmentID(), str_productID, meeting.getyear(), meeting.getmonth(), backlogY);
                //by yyan item8 20110614 del end
                //by yyan item8 20110614 add start
                backlog = sql.getBackLogByOperation(ddlist_operation.SelectedItem.Value, getSegmentID(), str_productID, meeting.getyear(), meeting.getmonth(), backlogY, flagMoney);
                //by yyan item8 20110614 add end
            if (backlog == "")
                backlog = "0";
            float backlogValue = float.Parse(backlog);

            SumFC[j] = backlogValue;
            for (int i = 2; i < ds.Tables[0].Rows.Count; i++)
            {
                string rowName = ds.Tables[0].Rows[i][0].ToString();
                int index = rowName.IndexOf('-');
                int length = rowName.Length;
                //by yyan item8 20110614 del start
                //string tmp = rowName.Substring(index + 1, length - index - 1).Trim();
                //by yyan item8 20110614 del end
                //by yyan item8 20110614 add start
                string tmp = rowName.Substring(index + 2, length - index - 2).Trim();
                //by yyan item8 20110614 add end
                if (backlogY == tmp)
                {
                    SumFC[j] += float.Parse(ds.Tables[0].Rows[i][j].ToString());
                }
                else
                    SumFC[j] += 0;
            }
            string backlogBDGT;
            if (salesOrgID != "")
            { //by yyan item8 20110614 del start
                //backlogBDGT = sql.getBackLogBySalesOrgByOperation(ddlist_operation.SelectedItem.Value, getSegmentID(), str_productID, meeting.getyear(), "03", salesOrgID, backlogY);
                //by yyan item8 20110614 del end
                //by yyan item8 20110614 add start
                if (backlogY.Equals(meeting.getyear().Substring(2, 2).Trim()))
                {
                    backlogBDGT = sql.getBackLogBySalesOrgByOperation(ddlist_operation.SelectedItem.Value, getSegmentID(), str_productID, meeting.getpreyear(), "03", salesOrgID, backlogY, flagMoney);
                }
                else
                {
                    backlogBDGT = sql.getBackLogBySalesOrgByOperation(ddlist_operation.SelectedItem.Value, getSegmentID(), str_productID, meeting.getyear(), "03", salesOrgID, backlogY, flagMoney);
                }
            }
            //by yyan item8 20110614 add end
            else
            {   //by yyan item8 20110614 del start
                //backlogBDGT = sql.getBackLogByOperation(ddlist_operation.SelectedItem.Value, getSegmentID(), str_productID, meeting.getyear(), "03", backlogY);
                //by yyan item8 20110614 del end
                //by yyan item8 20110614 add start
                if (backlogY.Equals(meeting.getyear().Substring(2, 2).Trim()))
                {
                    backlogBDGT = sql.getBackLogByOperation(ddlist_operation.SelectedItem.Value, getSegmentID(), str_productID, meeting.getpreyear(), "03", backlogY, flagMoney);
                }
                else
                {
                    backlogBDGT = sql.getBackLogByOperation(ddlist_operation.SelectedItem.Value, getSegmentID(), str_productID, meeting.getyear(), "03", backlogY, flagMoney);
                }
                //by yyan item8 20110614 add end
            }
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
    //by yyan item8 20110614 del start 
    //protected void bindDataSource()
    //by yyan item8 20110614 del end
    //by yyan item8 20110614 add start 
    protected void bindDataSource(int flagMoney)
    //by yyan item8 20110614 add end
    {
        //By SJ 20110517 Item22 Add Start
        int index = 0;
        //By SJ 20110517 Item22 Add End
        //DataSet dsSaleOrg = sql.getSalesOrgInfo();
        DataSet dsSaleOrg = sql.getSalesOrgInfoBySeg1(Convert.ToInt32(getSegmentID()));
        DataSet dsPro = sql.getProductInfoBySegmentID(getSegmentID());

        if (dsPro.Tables[0].Rows.Count > 0 && dsSaleOrg.Tables[0].Rows.Count > 0)
        {
            TableRow tr = new TableRow();
            table_opSales.Rows.Add(tr);
            table_opSales.Visible = true;

            TableCell tc = new TableCell();
            tc.HorizontalAlign = HorizontalAlign.Left;
            tc.VerticalAlign = VerticalAlign.Top;
            //by ryzhang item49 20100530 add start
            DataSet dsOp = sql.getSalesDataByOperation(dsPro, ddlist_operation.SelectedItem.Value, getSegmentID(),flagMoney);
            //by yyan itemw146 20110914 add start
            if (meeting.getmonth() == "10" || meeting.getmonth() == "01" || meeting.getmonth()=="1")
            {
                if (dsOp.Tables[0].Rows.Count ==0){
                    DataRow[] rows = new DataRow[1];
                    rows[0] = dsOp.Tables[0].NewRow();
                    if (meeting.getmonth() == "10")
                    {
                        rows[0][0] = meeting.getnextyear().Substring(2) + "  --" + meeting.getnextyear().Substring(2);
                    }
                    else {
                        rows[0][0] = meeting.getyear().Substring(2) + "  --" + meeting.getyear().Substring(2);
                    }
                    for (int i = 0; i < dsOp.Tables[0].Columns.Count - 1; i++)
                    {
                        rows[0][i + 1] = 0;
                    }

                    dsOp.Tables[0].Rows.InsertAt(rows[0], 0);
                }  
            }
            //by yyan itemw146 20110914 add end
            
            //by yyan itemw93 add start 
            DataSet dsOp1 = dsOp.Copy();
            GridView gvOp1 = new GridView();
            //by yyan itemw93 add end           
            GridView gvOp = new GridView();
            if (dsOp.Tables[0].Rows.Count > 0)
            {
                //by yyan item8 20110614 del start
                //gvOp = bindDataBySalesOrgByOperation(dsOp, gvOp, "<br />", "");
                //by yyan item8 20110614 del end
                //by yyan item8 20110614 add start
                gvOp = bindDataBySalesOrgByOperation(dsOp, gvOp, "<br />", "", flagMoney, true);
                gvOp1 = bindDataBySalesOrgByOperation(dsOp1, gvOp1, "", "", flagMoney, false);
                //by yyan item8 20110614 add end
            }
            
            web.setProperties(gvOp);
            web.setProperties(gvOp1);
            //by yyan item8 20110614 del add
            /*for (int i = 0; i < dsOp.Tables[0].Rows.Count; i++)
            {
                for (int j = 1; j < dsOp.Tables[0].Columns.Count; j++)
                {
                    dsOp.Tables[0].Rows[i][j] = 0;
                }
            }*/
            //by yyan item8 20110614 del end
            //by ryzhang item49 20100530 end start

            for (int count = 0; count < dsSaleOrg.Tables[0].Rows.Count; count++)
            {
                DataSet[] ds = new DataSet[dsSaleOrg.Tables[0].Rows.Count];
                GridView[] gv = new GridView[dsSaleOrg.Tables[0].Rows.Count];
                string str_SalesOrgID = dsSaleOrg.Tables[0].Rows[count][0].ToString().Trim();
                string str_SalesOrgAbbr = dsSaleOrg.Tables[0].Rows[count][1].ToString().Trim();
                //New the instance to this controls
                //by yyan itemw93 add start 
                DataSet[] dsStyle = new DataSet[dsSaleOrg.Tables[0].Rows.Count];
                GridView[] gvStyle = new GridView[dsSaleOrg.Tables[0].Rows.Count];
                gvStyle[count] = new GridView();
                web.setProperties(gvStyle[count]);
                //by yyan itemw93 add end 
                gv[count] = new GridView();
                web.setProperties(gv[count]);
                //by yyan item8 20110614 del start
                //ds[count] = sql.getSalesDataBySalesOrgByOperation(dsPro, str_SalesOrgID, ddlist_operation.SelectedItem.Value, getSegmentID());
                //by yyan item8 20110614 del end
                //by yyan item8 20110614 add start
                ds[count] = sql.getSalesDataBySalesOrgByOperation(dsPro, str_SalesOrgID, ddlist_operation.SelectedItem.Value, getSegmentID(),flagMoney);
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
                //by yyan itemw93 add start 
                dsStyle[count] = ds[count].Copy();
                //by yyan itemw93 add end 
                //by yyan item8 20110614 add end
               

                if (ds[count].Tables[0].Rows.Count > 0)
                {
                    //By SJ 20110516 Item22 Update Start
                    //gv[count] = bindDataBySalesOrgByOperation(ds[count], gv[count], str_SalesOrgAbbr, str_SalesOrgID);
                    //by yyan item8 20110614 del start
                    //gv[count] = bindDataBySalesOrgByOperation(ds[count], gv[count], "<br/>", str_SalesOrgID);
                    //by yyan item8 20110614 del end
                    //by yyan item8 20110614 add start
                    gv[count] = bindDataBySalesOrgByOperation(ds[count], gv[count], "<br/>", str_SalesOrgID, flagMoney,true);
                    //by yyan item8 20110614 add end
                    //By SJ 20110516 Item22 Update End
                    //By SJ 20110516 Item22 Add Start
                    gv[count].HeaderRow.Cells[0].Text = str_SalesOrgAbbr;
                    //sql.getSalesOrgCurrency(str_SalesOrgID);
                    gv[count].Style.Clear();
                    gv[count].Style.Add("border", "#000 solid 1px");
                    gv[count].Style.Add("border-collapse", "collapse");
                    gv[count].Style.Add("font-size", "12px");

                    gv[count].HeaderRow.Style.Add("background", "#000");
                    for (int i = 0; i < gv[count].Rows.Count; i++)
                    {
                        foreach (TableCell cell in gv[count].Rows[i].Cells)
                        {
                            //by yyan item8 20110704 add start
                            gvUpdateCell(gv[count], i);
                            //by yyan item8 20110704 add end
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

                    //by yyan itemw93 add start 
                    gvStyle[count] = bindDataBySalesOrgByOperation(dsStyle[count], gvStyle[count], "", str_SalesOrgID, flagMoney, false);
                    gvStyle[count].HeaderRow.Cells[0].Text = "";
                    gvStyle[count].Style.Clear();
                    gvStyle[count].Style.Add("border", "#000 solid 1px");
                    gvStyle[count].Style.Add("border-collapse", "collapse");
                    gvStyle[count].Style.Add("font-size", "12px");
                    gvStyle[count].HeaderRow.Style.Add("background", "#000");
                    for (int i = 0; i < gvStyle[count].Rows.Count; i++)
                    {
                        foreach (TableCell cell in gvStyle[count].Rows[i].Cells)
                        {
                            gvUpdateCell(gvStyle[count], i);
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
                    //by yyan itemw93 add end 


                    index++;
                    //By SJ 20110516 Item22 Add End
                    tc.Controls.Add(gv[count]);
                    tc.Controls.Add(gvStyle[count]);
                    //by ryzhang item49 20100530 add start
                    //by yyan item8 20100614 del start
                    /*if (gv[count].Rows.Count > dsOp.Tables[0].Rows.Count)
                    {
                        int sub = gv[count].Rows.Count - dsOp.Tables[0].Rows.Count;
                        for (int i = 0; i < sub; i++)
                        {
                            dsOp.Tables[0].Rows.Add(new Object[dsOp.Tables[0].Columns.Count]);
                        }
                    }
                    for (int i = 0; i < gv[count].Rows.Count; i++)
                    {
                        for (int j = 0; j < gv[count].Rows[i].Cells.Count; j++)
                        {
                            if (j==0)
                            {
                                dsOp.Tables[0].Rows[i][j] = gv[count].Rows[i].Cells[j].Text;
                            }
                            else
                            {
                                int sum = int.Parse(gv[count].Rows[i].Cells[j].Text);
                                int sumTotal = int.Parse(dsOp.Tables[0].Rows[i][j].ToString());
                                dsOp.Tables[0].Rows[i][j] = (sum + sumTotal);
                            }
                        }
                    }*/
                    //by yyan item8 20100614 del end
                    //by ryzhang item49 20100530 add end
                }
            }
            ////by ryzhang item49 20100530 modify start
            //DataSet dsOp = sql.getSalesDataByOperation(dsPro, ddlist_operation.SelectedItem.Value, getSegmentID());
            //GridView gvOp = new GridView();
            //New the instance to this controlsgv[count] = new GridView();
            //web.setProperties(gvOp);

            if (dsOp.Tables[0].Rows.Count > 0)
            {
                //By SJ 20110516 Item22 Update Start
                //gvOp = bindDataBySalesOrgByOperation(dsOp, gvOp, "<br />Total", "");
                //gvOp = bindDataBySalesOrgByOperation(dsOp, gvOp, "<br />", "");
                //by yyan item8 20110614 del start
                //gvOp.DataSource = dsOp;
                //gvOp.DataBind();
                //by yyan item8 20110614 del end
                //By SJ 20110516 Item22 Update End
                //By SJ 20110516 Item22 Add Start
                gvOp.HeaderRow.Cells[0].Text = "Total";
                gvOp.Style.Clear();
                gvOp.Style.Add("border", "#000 solid 1px");
                gvOp.Style.Add("border-collapse", "collapse");
                gvOp.Style.Add("font-size", "12px");

                gvOp.HeaderRow.Style.Add("background", "#000");
                for (int i = 0; i < gvOp.Rows.Count; i++)
                {
                    //by yyan item8 20110704 add start
                    gvUpdateCell(gvOp, i);
                    //by yyan item8 20110704 add end
                    foreach (TableCell cell in gvOp.Rows[i].Cells)
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
                //By SJ 20110516 Item22 Add End
                //by yyan itemw93 add start 
                gvOp1.HeaderRow.Cells[0].Text = "";
                gvOp1.Style.Clear();
                gvOp1.Style.Add("border", "#000 solid 1px");
                gvOp1.Style.Add("border-collapse", "collapse");
                gvOp1.Style.Add("font-size", "12px");
                gvOp1.HeaderRow.Style.Add("background", "#000");
                for (int i = 0; i < gvOp1.Rows.Count; i++)
                {
                    gvUpdateCell(gvOp1, i);
                    foreach (TableCell cell in gvOp1.Rows[i].Cells)
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
                
                //by yyan itemw93 add end 
                tc.Controls.Add(gvOp);
                tc.Controls.Add(gvOp1);
            }
            tr.Controls.Add(tc);
        }
        //by ryzhang item49 20100530 modify end
    }

    /* Get Data By Sales Organization By Operation End */

    /* One page to another page */

    protected void lbtn_grSales_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/Admin/AdminGrossSales.aspx?SegmentID=" + getSegmentID());
    }

    protected void lbtn_grBKG_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/Admin/AdminGrossBookings.aspx?SegmentID=" + getSegmentID());
    }

    protected void lbtn_opSales_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/Admin/AdminOperationSales.aspx?SegmentID=" + getSegmentID());
    }

    protected void lbtn_opBKG_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/Admin/AdminOperationalBookings.aspx?SegmentID=" + getSegmentID());
    }

    protected void lbtn_SO_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/Admin/AdminBookingBySalesOrg.aspx?SegmentID=" + getSegmentID());
    }

    #region
    public override void VerifyRenderingInServerForm(Control control)
    {
        // Confirms that an HtmlForm control is rendered for
    }

    protected void btn_export_Click(object sender, EventArgs e)
    {
        TableRow tr5 = new TableRow();
        table_opSales.Rows.Add(tr5);
        TableCell tc5 = new TableCell();
        tc5.Text = label_show.Text;
        tr5.Controls.Add(tc5);
        
        //TableRow tr1 = new TableRow();
        //table_opSales.Rows.Add(tr1);
        TableRow tr = new TableRow();
        table_opSales.Rows.Add(tr);
        table_opSales.Visible = true;
        TableCell tc = new TableCell();
        tc.BorderColor = System.Drawing.Color.Black;
        tc.Font.Bold = true;
        tc.BorderWidth = 1;
        tc.BackColor = System.Drawing.Color.LawnGreen;
        if (lblEUR.Text == "")
        {
            tc.Text = "OP-Sales(" + getSegmentDec(getSegmentID()) + "-" + ddlist_operation.SelectedItem.Text + ")" + " Values in " + label_currency.Text.Substring(1);

        }
        else
        {
            tc.Text = "OP-Sales(" + getSegmentDec(getSegmentID()) + "-" + ddlist_operation.SelectedItem.Text + ")" + " Values in EURO";

        }
              tr.Controls.Add(tc);
         
        //by yyan item8 20110614 add start 
        if (lblEUR.Text == "") 
        {bindDataSource(0);}
        else 
        { bindDataSource(1); }
        //by yyan item8 20110614 add end 
        //by yyan item8 20110614 del start 
        //bindDataSource();
        //by yyan item8 20110614 del end 
        
        cf.ToExcel(table_opSales, "OP-Sales(" + getSegmentDec(getSegmentID()) + "-" + ddlist_operation.SelectedItem.Text + ").xls");
    }
    #endregion

    //by yyan item8 20110614 add start 
    protected void btnEUR_Click(object sender, EventArgs e)
    {
        string query_date = "SELECT CONVERT(varchar(15),SelectMeetingDate,23) FROM [SetSelectMeetingDate] where userid=" + Session["AdministratorID"].ToString();
        DataSet ds_date = helper.GetDataSet(query_date);
        if (ds_date.Tables[0].Rows.Count > 0 && !ds_date.Tables[0].Rows[0][0].ToString().Equals("") && ds_date.Tables[0].Rows[0][0].ToString() != null)
        {
            label_show.Text = "This report is related to the meeting date " + ds_date.Tables[0].Rows[0][0].ToString();
        }
        else
        {
            label_show.Text = "There is no meeting date selected!";
        }
        label_description.Visible = true;
        label_description.Text = getSegmentDec(getSegmentID()) + " - SALES BY OPERATION BY SALES ORGANIZATION";
        string str_currency = sql.getOperationCurrency(ddlist_operation.SelectedItem.Value.Trim());
        if (str_currency != "")
        {
            bindDataSource(1);
        }
        lblEUR.Text = "The amount is calculated based on EUR.";
    }
    //by yyan item8 20110614 add end 


    //by yyan item8 20110704 add start
    protected void gvUpdateCell(GridView gvWindow,int i)
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
        //    if (gvWindow.Rows[i].Cells[0].Text.Trim().Substring(0,2).Equals(meeting.getyear().Substring(2,2)) ||
        //        gvWindow.Rows[i].Cells[0].Text.Trim().Substring(0,4).Equals(meeting.getyear().Trim()))
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
        btn_search_Click(sender, e);
    }
    //by yyan itemw90 20110805 add end 
}
