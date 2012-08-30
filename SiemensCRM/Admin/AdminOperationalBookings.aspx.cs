/*
 * File Name   : AdminOperationalBookings.aspx.cs
 * 
 * Description : Get bookings data by operation by salesorg
 * 
 * Author      : Wang Jun
 * 
 * Modify Date : 2010-12-21
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

public partial class Admin_AdminOperationalBookings : System.Web.UI.Page
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
            //by yyan item8 20110616 add start 
            btnEUR.Visible = false;
            //by yyan item8 20110616 add end 
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
        label_description.Text = getSegmentDec(getSegmentID()) + " - NEW ORDERS BY OPERATION BY SALES ORGANIZATION";
        string str_currency = sql.getOperationCurrency(ddlist_operation.SelectedItem.Value.Trim());
        if (str_currency != "")
        {
            label_currency.Text = "k" + str_currency;
            //by yyan item8 20110616 del start 
            //bindDataSource();
            //by yyan item8 20110616 del end 
            //by yyan item8 20110616 add start 
            bindDataSource(0);
            //by yyan item8 20110616 add end 
        }
        else
            label_currency.Text = info.notCurrency("Operation:" + ddlist_operation.SelectedItem.Text);
        btn_export.Visible = true;
        //by yyan item8 20110616 add start 
        btnEUR.Visible = true;
        lblEUR.Text = "";
        //by yyan item8 20110616 add end
        //by yyan itemw90 20110805 add start 
        btnLocal.Visible = true;
        btnLocal.Text = label_currency.Text;
        //by yyan itemw90 20110805 add end 
    }

    /// <summary>
    ///  Bind bookings data to gridview
    /// </summary>
    /// <param name="ds">DataSet</param>
    /// <param name="gv">GridView</param>
    /// <param name="header">Title</param>
    /// <param name="ds_budget">Budget DataSet</param>
    /// <returns>return gridview</returns>
    protected GridView bindDataBySalesOrgByOperation(DataSet ds, GridView gv, string header, DataSet ds_budget)
    {
        //last
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

            if (header == "")
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

                float tmp = 0;
                for (int i = 0; i < ds.Tables[0].Columns.Count - 1; i++)
                {
                    if (i == 0)
                    {
                        drSum[0] = meeting.getnextyear() + "-BDGT";
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
            gv.DataBind();
        }
        else if (ds_budget != null && ds_budget.Tables[0].Rows.Count > 0)
        {
            gv.Caption = header;
            gv.CaptionAlign = TableCaptionAlign.Left;
            gv.DataSource = ds;

            if (header == "")
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

                float tmp = 0;
                for (int i = 0; i < ds.Tables[0].Columns.Count - 1; i++)
                {
                    if (i == 0)
                    {
                        drSum[0] = meeting.getnextyear() + "-BDGT";
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
            gv.DataBind();
        }
        else
            gv.Visible = false;
        return gv;
    }

    //by yyan item8 20110616 del start 
    //protected void bindDataSource()
    //by yyan item8 20110616 del end
    //by yyan item8 20110616 add start 
    protected void bindDataSource(int flagMoney)
    //by yyan item8 20110616 add end
    {
        //DataSet dsSaleOrg = sql.getSalesOrgInfo();
        DataSet dsSaleOrg = sql.getSalesOrgInfoBySeg1(Convert.ToInt32(getSegmentID()));
        DataSet dsPro = sql.getProductInfoBySegmentID(getSegmentID());
        // by daixuesong  20110517  Item22 add start
        int index = 0;
        // by daixuesong  20110517  Item22 add end
        if (dsPro.Tables[0].Rows.Count > 0 && dsSaleOrg.Tables[0].Rows.Count > 0)
        {
            TableRow tr = new TableRow();
            table_opSales.Rows.Add(tr);
            table_opSales.Visible = true;

            TableCell tc = new TableCell();
            tc.HorizontalAlign = HorizontalAlign.Left;
            tc.VerticalAlign = VerticalAlign.Top;

            //by ryzhang item49 20110530 add start
            GridView gvOp = new GridView();
            GridView gvTotalOp = new GridView();
            web.setProperties(gvOp);
            web.setProperties(gvTotalOp);

            DataSet ds_budgetTotal = sql.getBookingsBudgetTotalByOperation(dsPro, ddlist_operation.SelectedItem.Value, getSegmentID(),flagMoney);
            DataSet dsOp = sql.getBookingsDataByOperation(dsPro, ddlist_operation.SelectedItem.Value, getSegmentID(),flagMoney);
            dsOp.Tables[0].Columns.RemoveAt(0);
            DataSet dsTotalOp = sql.getBookingsDataTotalByOperation(dsPro, ddlist_operation.SelectedItem.Value, getSegmentID(),flagMoney);

            //by yyan item8 20110616 del start 
            //for (int i = 0; i < dsOp.Tables[0].Rows.Count; i++)
            //{
            //    for (int j = 1; j < dsOp.Tables[0].Columns.Count; j++)
            //    {
            //        dsOp.Tables[0].Rows[i][j] = 0;
            //    }
            //}
            //by yyan item8 20110616 del end 
            //for (int i = 0; i < dsTotalOp.Tables[0].Rows.Count; i++)
            //{
            //    for (int j = 1; j < dsTotalOp.Tables[0].Columns.Count; j++)
            //    {
            //        dsTotalOp.Tables[0].Rows[i][j] = 0;
            //    }
            //}
            //by ryzhang item49 20110530 add end

            //if (meeting.getmonth() == "10" || meeting.getmonth() == "01" || meeting.getmonth() == "1")
            //{
            //    if (dsOp.Tables[0].Rows.Count == 0)
            //    {
            //        DataRow[] rows = new DataRow[1];
            //        rows[0] = dsOp.Tables[0].NewRow();
            //        if (meeting.getmonth() == "10")
            //        {
            //            rows[0][0] = meeting.getnextyear().Substring(2) + "  --" + meeting.getnextyear().Substring(2);
            //        }
            //        else
            //        {
            //            rows[0][0] = meeting.getyear().Substring(2) + "  --" + meeting.getyear().Substring(2);
            //        }
            //        for (int i = 0; i < dsOp.Tables[0].Columns.Count - 1; i++)
            //        {
            //            rows[0][i + 1] = 0;
            //        }

            //        dsOp.Tables[0].Rows.InsertAt(rows[0], 0);
            //    }

            //    if (dsOp.Tables[0].Rows.Count == 0)
            //    {

            //    }
            //}

            if (dsOp.Tables[0].Rows.Count > 0)
            {
                gvOp = bindDataBySalesOrgByOperation(dsOp, gvOp, "<br />", null);
            }
            if (dsTotalOp.Tables[0].Rows.Count > 0 || ds_budgetTotal.Tables[0].Rows.Count>0)
            {
                gvTotalOp = bindDataBySalesOrgByOperation(dsTotalOp, gvTotalOp, "", ds_budgetTotal);
            }
           

            for (int count = 0; count < dsSaleOrg.Tables[0].Rows.Count; count++)
            {
                DataSet[] ds = new DataSet[dsSaleOrg.Tables[0].Rows.Count];
                DataSet[] dsTotal = new DataSet[dsSaleOrg.Tables[0].Rows.Count];
                //by yyan 20100616 item8 add start 
                DataSet[] dsTotalSum = new DataSet[dsSaleOrg.Tables[0].Rows.Count];
                //by yyan 20100616 item8 add end 
                GridView[] gv = new GridView[dsSaleOrg.Tables[0].Rows.Count];
                GridView[] gvTotal = new GridView[dsSaleOrg.Tables[0].Rows.Count];
                //by yyan 20100616 item8 add start 
                GridView[] gvTotalSum = new GridView[dsSaleOrg.Tables[0].Rows.Count];
                //by yyan 20100616 item8 add end 
                string str_SalesOrgID = dsSaleOrg.Tables[0].Rows[count][0].ToString().Trim();
                string str_SalesOrgAbbr = dsSaleOrg.Tables[0].Rows[count][1].ToString().Trim();
                //New the instance to this controls
                gv[count] = new GridView();
                web.setProperties(gv[count]);
                gvTotal[count] = new GridView();
                web.setProperties(gvTotal[count]);
                //by yyan item8 20110617 del start 
                //ds[count] = sql.getBookingsDataBySalesOrgByOperation(dsPro, str_SalesOrgID, ddlist_operation.SelectedItem.Value, getSegmentID());
                //DataSet ds_budget = sql.getBookingsBudgetBySalesOrgByOperation(dsPro, str_SalesOrgID, ddlist_operation.SelectedItem.Value, getSegmentID());
                //by yyan item8 20110617 del end 
                //by yyan item8 20110617 add start 
                ds[count] = sql.getBookingsDataBySalesOrgByOperation(dsPro, str_SalesOrgID, ddlist_operation.SelectedItem.Value, getSegmentID(), flagMoney);
                ds[count].Tables[0].Columns.RemoveAt(0);
                DataSet ds_budget = sql.getBookingsBudgetBySalesOrgByOperation(dsPro, str_SalesOrgID, ddlist_operation.SelectedItem.Value, getSegmentID(), 0);
                gvTotalSum[count] = new GridView();
                web.setProperties(gvTotalSum[count]);
                DataSet ds_budgetSum = null;
                if (flagMoney == 1)
                {
                    ds_budgetSum = sql.getBookingsBudgetBySalesOrgByOperation(dsPro, str_SalesOrgID, ddlist_operation.SelectedItem.Value, getSegmentID(), 1);
                }
                //by yyan 20100616 item8 add end 
                // by daixuesong  20110517  Item22 update start
                //by yyan item8 20110617 del start 
                //dsTotal[count] = sql.getBookingsDataTotalBySalesOrgByOperation(dsPro, str_SalesOrgID, ddlist_operation.SelectedItem.Value, getSegmentID());
                //by yyan item8 20110617 del end 
                //by yyan 20100616 item8 add start 
                dsTotal[count] = sql.getBookingsDataTotalBySalesOrgByOperation(dsPro, str_SalesOrgID, ddlist_operation.SelectedItem.Value, getSegmentID(), 0);
                if (flagMoney == 1)
                {
                    dsTotalSum[count] = sql.getBookingsDataTotalBySalesOrgByOperation(dsPro, str_SalesOrgID, ddlist_operation.SelectedItem.Value, getSegmentID(), 1);
                }
                //by yyan 20100616 item8 add end 

                //if (meeting.getmonth() == "10" || meeting.getmonth() == "01" || meeting.getmonth() == "1")
                //{
                //    if (ds[count].Tables[0].Rows.Count == 0)
                //    {
                //        DataRow[] rows = new DataRow[1];
                //        rows[0] = ds[count].Tables[0].NewRow();
                //        if (meeting.getmonth() == "10")
                //        {
                //            rows[0][0] = meeting.getnextyear().Substring(2) + "  --" + meeting.getnextyear().Substring(2);
                //        }
                //        else
                //        {
                //            rows[0][0] = meeting.getyear().Substring(2) + "  --" + meeting.getyear().Substring(2);
                //        }
                //        for (int i = 0; i < ds[count].Tables[0].Columns.Count - 1; i++)
                //        {
                //            rows[0][i + 1] = 0;
                //        }

                //        ds[count].Tables[0].Rows.InsertAt(rows[0], 0);
                //    }

                //    if (dsTotal[count].Tables[0].Rows.Count == 0)
                //    { 
                        
                //    }
                //}


                if (ds[count].Tables[0].Rows.Count > 0 || dsTotal[count].Tables[0].Rows.Count > 0 || ds_budget.Tables[0].Rows.Count>0)
                {
                    if (ds[count].Tables[0].Rows.Count > 0)
                    {
                        gv[count] = bindDataBySalesOrgByOperation(ds[count], gv[count], "<BR />", null);
                        gv[count].HeaderRow.Cells[0].Text = str_SalesOrgAbbr;
                        gv[count].Style.Clear();
                        gv[count].Style.Add("border", "#000 solid 1px");
                        gv[count].Style.Add("border-collapse", "collapse");
                        gv[count].Style.Add("font-size", "12px");

                        gv[count].HeaderRow.Style.Add("background", "#000");
                        for (int i = 0; i < gv[count].Rows.Count; i++)
                        {
                            //by yyan item8 20110704 add start
                            gvUpdateCell(gv[count], i);
                            //by yyan item8 20110704 add end
                            foreach (TableCell cell in gv[count].Rows[i].Cells)
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
                        tc.Controls.Add(gv[count]);
                        //by ryzhang item49 20100530 add start
                        //by yyan item8 20100616 del start
                        /*if (gv[count].Rows.Count > dsOp.Tables[0].Rows.Count)
                        {
                            int sub = gv[count].Rows.Count - dsOp.Tables[0].Rows.Count;
                            for (int i = 0; i < sub; i++)
                            {
                                dsOp.Tables[0].Rows.Add(new Object[dsOp.Tables[0].Columns.Count]);
                            }
                        }
                        if (gv[count].HeaderRow.Cells.Count > dsOp.Tables[0].Columns.Count)
                        {
                            dsOp.Tables[0].Columns.Add("Total");
                        }
                        for (int i = 0; i < gv[count].Rows.Count; i++)
                        {
                            for (int j = 0; j < gv[count].Rows[i].Cells.Count; j++)
                            {
                                if (j == 0)
                                {
                                    dsOp.Tables[0].Rows[i][j] = gv[count].Rows[i].Cells[j].Text;
                                }
                                else
                                {
                                    int sum = int.Parse(gv[count].Rows[i].Cells[j].Text);
                                    int sumTotal = int.Parse(dsOp.Tables[0].Rows[i][j].ToString() == "" ? "0" : dsOp.Tables[0].Rows[i][j].ToString());
                                    dsOp.Tables[0].Rows[i][j] = (sum + sumTotal);
                                }
                            }
                        }*/
                        //by yyan item8 20100616 del end
                        //by ryzhang item49 20100530 add end
                    }
                    //by yyan 20100616 item8 add start 
                    if (flagMoney == 1)
                    {
                        if (dsTotalSum[count].Tables[0].Rows.Count > 0)
                        {

                            gvTotalSum[count] = bindDataBySalesOrgByOperation(dsTotalSum[count], gvTotalSum[count], "", ds_budgetSum);

                            gvTotalSum[count].Style.Clear();
                            gvTotalSum[count].Style.Add("border", "#000 solid 1px");
                            gvTotalSum[count].Style.Add("border-collapse", "collapse");
                            gvTotalSum[count].Style.Add("font-size", "12px");

                            gvTotalSum[count].HeaderRow.Style.Add("background", "#000");
                            for (int i = 0; i < gvTotalSum[count].Rows.Count; i++)
                            {
                                //by yyan item8 20110704 add start
                                gvUpdateCell(gvTotalSum[count], i);
                                //by yyan item8 20110704 add end
                                foreach (TableCell cell in gvTotalSum[count].Rows[i].Cells)
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
                            tc.Controls.Add(gvTotalSum[count]);
                        }
                    }
                    //by yyan 20100616 item8 add end 
                    if (dsTotal[count].Tables[0].Rows.Count > 0 || ds_budget.Tables[0].Rows.Count > 0)
                    {
                        gvTotal[count] = bindDataBySalesOrgByOperation(dsTotal[count], gvTotal[count], "", ds_budget);
                        gvTotal[count].Style.Clear();
                        gvTotal[count].Style.Add("border", "#000 solid 1px");
                        gvTotal[count].Style.Add("border-collapse", "collapse");
                        gvTotal[count].Style.Add("font-size", "12px");

                        gvTotal[count].HeaderRow.Style.Add("background", "#000");
                        for (int i = 0; i < gvTotal[count].Rows.Count; i++)
                        {
                            foreach (TableCell cell in gvTotal[count].Rows[i].Cells)
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

                        //by ryzhang item49 20100530 add start
                        //if (gvTotal[count].Rows.Count > dsTotalOp.Tables[0].Rows.Count)
                        //{
                        //    int sub = gvTotal[count].Rows.Count - dsTotalOp.Tables[0].Rows.Count;
                        //    for (int i = 0; i < sub; i++)
                        //    {
                        //        dsTotalOp.Tables[0].Rows.Add(new Object[dsTotalOp.Tables[0].Columns.Count]);
                        //    }
                        //}
                        //if (gvTotal[count].HeaderRow.Cells.Count > dsTotalOp.Tables[0].Columns.Count)
                        //{
                        //    dsTotalOp.Tables[0].Columns.Add("Total");
                        //}
                        //for (int i = 0; i < gvTotal[count].Rows.Count; i++)
                        //{
                        //    for (int j = 0; j < gvTotal[count].Rows[i].Cells.Count; j++)
                        //    {
                        //        if (j == 0)
                        //        {
                        //            dsTotalOp.Tables[0].Rows[i][j] = gvTotal[count].Rows[i].Cells[j].Text;
                        //        }
                        //        else
                        //        {
                        //            int sum = int.Parse(gvTotal[count].Rows[i].Cells[j].Text);
                        //            int sumTotal = int.Parse(dsTotalOp.Tables[0].Rows[i][j].ToString() == "" ? "0" : dsTotalOp.Tables[0].Rows[i][j].ToString());
                        //            dsTotalOp.Tables[0].Rows[i][j] = (sum + sumTotal);
                        //        }
                        //    }
                        //}
                        //by ryzhang item49 20100530 add end
                        //by yyan 20100616 item8 add start 
                        if (flagMoney == 0)
                        {
                            //by yyan item8 20110704 add start
                            for (int i = 0; i < gvTotal[count].Rows.Count; i++)
                            {
                                gvUpdateCell(gvTotal[count], i);
                            }
                            //by yyan item8 20110704 add end
                            tc.Controls.Add(gvTotal[count]);
                        }
                        //by yyan 20100616 item8 add end 
                    }
                    else
                    {
                        gvTotal[count].Visible = false;
                    }
                    index++;
                }
                // by daixuesong  20110517  Item22 update end

            }
            //by ryzhang item49 20110530 modify start
            //GridView gvOp = new GridView();
            //GridView gvTotalOp = new GridView();
            //web.setProperties(gvOp);
            //web.setProperties(gvTotalOp);

            //DataSet ds_budgetTotal = sql.getBookingsBudgetTotalByOperation(dsPro, ddlist_operation.SelectedItem.Value, getSegmentID());
            //DataSet dsOp = sql.getBookingsDataByOperation(dsPro, ddlist_operation.SelectedItem.Value, getSegmentID());
            //// by daixuesong  20110517  Item22 update start
            //DataSet dsTotalOp = sql.getBookingsDataTotalByOperation(dsPro, ddlist_operation.SelectedItem.Value, getSegmentID());
            if (dsOp.Tables[0].Rows.Count > 0 || dsTotalOp.Tables[0].Rows.Count > 0)
            {
                if (dsOp.Tables[0].Rows.Count > 0)
                {
                    //gvOp = bindDataBySalesOrgByOperation(dsOp, gvOp, "<br />", null);
                    //gvOp.DataSource = dsOp;
                    //gvOp.DataBind();
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
                    tc.Controls.Add(gvOp);
                }


                if (dsTotalOp.Tables[0].Rows.Count > 0)
                {
                    //gvTotalOp = bindDataBySalesOrgByOperation(dsTotalOp, gvTotalOp, "", ds_budgetTotal);
                    //gvTotalOp.DataSource = dsTotalOp;
                    //gvTotalOp.DataBind();
                    gvTotalOp.Style.Clear();
                    gvTotalOp.Style.Add("border", "#000 solid 1px");
                    gvTotalOp.Style.Add("border-collapse", "collapse");
                    gvTotalOp.Style.Add("font-size", "12px");
                    gvTotalOp.HeaderRow.Style.Add("background", "#000");
                    for (int i = 0; i < gvTotalOp.Rows.Count; i++)
                    {
                        //by yyan item8 20110704 add start
                        gvUpdateCell(gvTotalOp, i);
                        //by yyan item8 20110704 add end
                        foreach (TableCell cell in gvTotalOp.Rows[i].Cells)
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
                    tc.Controls.Add(gvTotalOp);
                }
                //by ryzhang item49 20110530 modify start
                tr.Controls.Add(tc);
            }
        }
        // by daixuesong  20110517  Item22 update end
    }

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
            tc.Text = "OP-BKG(" + getSegmentDec(getSegmentID()) + "-" + ddlist_operation.SelectedItem.Text + ")" + " Values in " + label_currency.Text.Substring(1);
        }
        else {
            tc.Text = "OP-BKG(" + getSegmentDec(getSegmentID()) + "-" + ddlist_operation.SelectedItem.Text + ")" + " Values in EURO";
        }
        tr.Controls.Add(tc);

        //by yyan item8 20110616 add start 
        if (lblEUR.Text == "")
        { bindDataSource(0); }
        else
        { bindDataSource(1); }
        //by yyan item8 20110616 add end 
        //by yyan item8 20110616 del start 
        //bindDataSource();
        //by yyan item8 20110616 del end 
        cf.ToExcel(table_opSales, "OP-BKG(" + getSegmentDec(getSegmentID()) + "-" + ddlist_operation.SelectedItem.Text + ").xls");
    }
    #endregion

    //by yyan item8 20110616 add start 
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
        label_description.Text = getSegmentDec(getSegmentID()) + " - NEW ORDERS BY OPERATION BY SALES ORGANIZATION";
        string str_currency = sql.getOperationCurrency(ddlist_operation.SelectedItem.Value.Trim());
        if (str_currency != "")
        {
            bindDataSource(1);
        }
        lblEUR.Text = "The amount is calculated based on EUR.";
    }
    //by yyan item8 20110616 add end 

    //by yyan item8 20110704 add start
    protected void gvUpdateCell(GridView gvWindow, int i)
    {
        //if (meeting.getmonth() == "1" )
        //{
        //    if (gvWindow.Rows[i].Cells[0].Text.Trim().Substring(0, 2).Equals(meeting.getyear().Substring(2, 2)) ||
        //         gvWindow.Rows[i].Cells[0].Text.Trim().Substring(0, 4).Equals(meeting.getyear().Trim()))
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
