/*
 * File Name   : RSMGrossBookings.aspx.cs
 * 
 * Description : Get gross booking data by operation
 * 
 * Author      : Wang Jun
 * 
 * Modify Date : 2010-04-12
 * 
 * Problem     : none
 * 
 * End Date    :    
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

public partial class RSM_Default : System.Web.UI.Page
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
        if (getRoleID(getRole()) != "4")
            Response.Redirect("!/AccessDenied.aspx");

        if (!IsPostBack)
        {
            //by ryzhang item49 20110519 del start 
            //meeting.setDate();
            //by ryzhang item49 20110519 del end 
            //by ryzhang item49 20110519 add start 
            meeting.setSelectDate(Session["RSMID"].ToString());
            //by ryzhang item49 20110519 add end
            label_description.Text = getSegmentDec(getSegmentID()) + " NEW ORDERS BY OPERATION";
            //by yyan item8 20110622 del start 
            //bindDataSource();
            //by yyan item8 20110622 del end 
            //by yyan item8 20110622 add start 
            bindDataSource(0);
            //by yyan item8 20110622 add end 
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

    protected GridView bindDataByOperation(DataSet ds, GridView gv, string header, DataSet ds_budget)
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
                    bf.ItemStyle.Width = 150;
                }

                gv.Columns.Add(bf);
            }
            gv.Caption = header;
            gv.CaptionAlign = TableCaptionAlign.Left;
            gv.DataSource = ds;

            if (header == "")
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

    //by yyan item8 20110622 del start 
    //protected void bindDataSource()
    //by yyan item8 20110622 del end 
    //by yyan item8 20110622 add start 
    protected void bindDataSource(int flagMoney)
    //by yyan item8 20110622 add end
    {
        //By Fxw 20110517 ITEM25 ADD Start
        string query_date = "SELECT CONVERT(varchar(15),SelectMeetingDate,23) FROM [SetSelectMeetingDate] where userid=" + Session["RSMID"].ToString();
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
        //By FXW 20110517 ITEM22 DEL Start
        int p = 0;
        //By FXW 20110517 ITEM22 DEL End
        if (dsPro.Tables[0].Rows.Count > 0 && dsOp.Tables[0].Rows.Count > 0)
        {
            TableRow tr = new TableRow();
            table_GrBKG.Rows.Add(tr);
            table_GrBKG.Visible = true;

            TableCell tc = new TableCell();
            tc.HorizontalAlign = HorizontalAlign.Left;
            tc.VerticalAlign = VerticalAlign.Top;
            //by ryzhang item49 20110530 add start
            DataSet dsTotalD = new DataSet();
            GridView gvTotalD = new GridView();
            DataSet dsTotalData = new DataSet();
            GridView gvTotalData = new GridView();

            gvTotalD = new GridView();
            web.setProperties(gvTotalD);
            gvTotalData = new GridView();
            web.setProperties(gvTotalData);

            dsTotalD = sql.getBookingsData(dsPro, getSegmentID());
            dsTotalD.Tables[0].Columns.RemoveAt(0);
            dsTotalData = sql.getBookingsDataTotal(dsPro, getSegmentID());
            DataSet ds_budgetTotal = sql.getBookingsBudgetTotal(dsPro, getSegmentID());

            if (dsTotalD.Tables[0].Rows.Count > 0)
            {
                gvTotalD = bindDataByOperation(dsTotalD, gvTotalD, "<br />", null);
            }
            if (dsTotalData.Tables[0].Rows.Count > 0)
            {
                gvTotalData = bindDataByOperation(dsTotalData, gvTotalData, "", ds_budgetTotal);
            }
            /*for (int i = 0; i < dsTotalD.Tables[0].Rows.Count; i++)
            {
                for (int j = 1; j < dsTotalD.Tables[0].Columns.Count; j++)
                {
                    dsTotalD.Tables[0].Rows[i][j] = 0;
                }
            }
            for (int i = 0; i < dsTotalData.Tables[0].Rows.Count; i++)
            {
                for (int j = 1; j < dsTotalData.Tables[0].Columns.Count; j++)
                {
                    dsTotalData.Tables[0].Rows[i][j] = 0;
                }
            }*/
            //by ryzhang item49 20110530 add end
            for (int count = 0; count < dsOp.Tables[0].Rows.Count; count++)
            {
                DataSet[] ds = new DataSet[dsOp.Tables[0].Rows.Count];
                //by yyan item8 20110622 add start 
                DataSet[] dsEur = new DataSet[dsOp.Tables[0].Rows.Count];
                DataSet[] dsTotalEur = new DataSet[dsOp.Tables[0].Rows.Count];
                GridView[] gvEur = new GridView[dsOp.Tables[0].Rows.Count];
                GridView[] gvTotalEur = new GridView[dsOp.Tables[0].Rows.Count];
                gvTotalEur[count] = new GridView();
                web.setProperties(gvTotalEur[count]);
                gvEur[count] = new GridView();
                web.setProperties(gvEur[count]);
                //by yyan item8 20110622 add end 
                DataSet[] dsTotal = new DataSet[dsOp.Tables[0].Rows.Count];
                GridView[] gv = new GridView[dsOp.Tables[0].Rows.Count];
                GridView[] gvTotal = new GridView[dsOp.Tables[0].Rows.Count];

                string str_OperationID = dsOp.Tables[0].Rows[count][1].ToString().Trim();
                string str_OperationName = dsOp.Tables[0].Rows[count][0].ToString().Trim();
                string str_OperationCurrency = sql.getOperationCurrency(str_OperationID);
                //New the instance to this controls
                gv[count] = new GridView();
                web.setProperties(gv[count]);
                gvTotal[count] = new GridView();
                web.setProperties(gvTotal[count]);
                //by yyan item8 20110622 del start 
                //ds[count] = sql.getBookingsDataByOperation(dsPro, str_OperationID, getSegmentID());
                //by yyan item8 20110622 del end 
                //by yyan item8 20110622 add start 
                ds[count] = sql.getBookingsDataByOperation(dsPro, str_OperationID, getSegmentID(),flagMoney);
                ds[count].Tables[0].Columns.RemoveAt(0);
                dsEur[count] = sql.getBookingsDataByOperation(dsPro, str_OperationID, getSegmentID(), 1);
                dsEur[count].Tables[0].Columns.RemoveAt(0);
                //by yyan item8 20110622 add end

                if (ds[count].Tables[0].Rows.Count > 0)
                {
                    //By FXW 20110517 ITEM22 DEL Start
                    //gv[count] = bindDataByOperation(ds[count], gv[count], "<br />" + str_OperationName + " (k" + str_OperationCurrency + ")", null);
                    //By FXW 20110517 ITEM22 DEL End
                    //By FXW 20110517 ITEM22 ADD Start
                    gv[count] = bindDataByOperation(ds[count], gv[count], "<br />", null);
                    //by yyan item8 20110622 add start 
                    gvEur[count] = bindDataByOperation(dsEur[count], gvEur[count], "<br />", null);
                    //by yyan item8 20110622 add end 
                    gv[count].HeaderRow.Cells[0].Text = str_OperationName + " (k" + str_OperationCurrency + ")";
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
                            cell.Style.Add("background", "#ccffff");
                            if (p % 2 == 0)
                            {
                                cell.Style.Add("background", "#FF9");
                            }
                        }
                    }
                    //By FXW 20110517 ITEM22 ADD End
                    tc.Controls.Add(gv[count]);
                    //by ryzhang item49 20100530 add start
                    //by yyan item8 20110622 del start
                    /*if (gv[count].Rows.Count > dsTotalD.Tables[0].Rows.Count)
                    {
                        int sub = gv[count].Rows.Count - dsTotalD.Tables[0].Rows.Count;
                        for (int i = 0; i < sub; i++)
                        {
                            dsTotalD.Tables[0].Rows.Add(new Object[dsTotalD.Tables[0].Columns.Count]);
                        }
                    }
                    for (int i = 0; i < gv[count].Rows.Count; i++)
                    {
                        for (int j = 0; j < gv[count].Rows[i].Cells.Count; j++)
                        {
                            if (j == 0)
                            {
                                dsTotalD.Tables[0].Rows[i][j] = gv[count].Rows[i].Cells[j].Text;
                            }
                            else
                            {
                                int sum = int.Parse(gv[count].Rows[i].Cells[j].Text);
                                int sumTotal = int.Parse(dsTotalD.Tables[0].Rows[i][j].ToString());
                                dsTotalD.Tables[0].Rows[i][j] = (sum + sumTotal);
                            }
                        }
                    }*/
                    //by yyan item8 20110622 del end
                    //by yyan item8 20110622 add start
                    /*if (gvEur[count].Rows.Count > dsTotalD.Tables[0].Rows.Count)
                    {
                        int sub = gvEur[count].Rows.Count - dsTotalD.Tables[0].Rows.Count;
                        for (int i = 0; i < sub; i++)
                        {
                            dsTotalD.Tables[0].Rows.Add(new Object[dsTotalD.Tables[0].Columns.Count]);
                        }
                    }
                    for (int i = 0; i < gvEur[count].Rows.Count; i++)
                    {
                        for (int j = 0; j < gvEur[count].Rows[i].Cells.Count; j++)
                        {
                            if (j == 0)
                            {
                                dsTotalD.Tables[0].Rows[i][j] = gvEur[count].Rows[i].Cells[j].Text;
                            }
                            else
                            {
                                int sum = int.Parse(gvEur[count].Rows[i].Cells[j].Text);
                                int sumTotal = int.Parse(dsTotalD.Tables[0].Rows[i][j].ToString());
                                dsTotalD.Tables[0].Rows[i][j] = (sum + sumTotal);
                            }
                        }
                    }*/
                    //by yyan item8 20110622 add end
                    //by ryzhang item49 20100530 add end
                }
                //by yyan item8 20110622 del start 
                //dsTotal[count] = sql.getBookingsDataTotalByOperation(dsPro, str_OperationID, getSegmentID());
                //DataSet ds_budget = sql.getBookingsBudgetTotalByOperation(dsPro, str_OperationID, getSegmentID());
                //by yyan item8 20110622 del end 
                //by yyan item8 20110622 add start 
                dsTotal[count] = sql.getBookingsDataTotalByOperation(dsPro, str_OperationID, getSegmentID(), flagMoney);
                dsTotalEur[count] = sql.getBookingsDataTotalByOperation(dsPro, str_OperationID, getSegmentID(), 1);
                DataSet ds_budget = sql.getBookingsBudgetTotalByOperation(dsPro, str_OperationID, getSegmentID(), flagMoney);
                DataSet ds_budgetEur = sql.getBookingsBudgetTotalByOperation(dsPro, str_OperationID, getSegmentID(), 1);
                //by yyan item8 20110622 add end
               
                if (dsTotal[count].Tables[0].Rows.Count > 0)
                {
                    gvTotal[count] = bindDataByOperation(dsTotal[count], gvTotal[count], "", ds_budget);
                    //by yyan item8 20110622 add start 
                    gvTotalEur[count] = bindDataByOperation(dsTotalEur[count], gvTotalEur[count], "", ds_budgetEur);
                    //by yyan item8 20110622 add end 
                    //By FXW 20110517 ITEM22 ADD Start
                    gvTotal[count].Style.Clear();
                    gvTotal[count].Style.Add("border", "#000 solid 1px");
                    gvTotal[count].Style.Add("border-collapse", "collapse");
                    gvTotal[count].Style.Add("font-size", "12px");
                    gvTotal[count].HeaderRow.Style.Add("background", "#000");
                    for (int i = 0; i < gvTotal[count].Rows.Count; i++)
                    {
                        //by yyan item8 20110704 add start
                        gvUpdateCell(gvTotal[count], i);
                        //by yyan item8 20110704 add end
                        foreach (TableCell cell in gvTotal[count].Rows[i].Cells)
                        {
                            cell.Style.Clear();
                            cell.Style.Add("background", "#ccffff");
                            if (p % 2 == 0)
                            {
                                cell.Style.Add("background", "#FF9");
                            }
                        }
                    }
                    p++;
                    //By FXW 20110517 ITEM22 ADD End
                    tc.Controls.Add(gvTotal[count]);
                    //by ryzhang item49 20100530 add start
                    //by yyan item8 20110622 del start
                    /*if (gvTotal[count].Rows.Count > dsTotalData.Tables[0].Rows.Count)
                    {
                        int sub = gvTotal[count].Rows.Count - dsTotalData.Tables[0].Rows.Count;
                        for (int i = 0; i < sub; i++)
                        {
                            dsTotalData.Tables[0].Rows.Add(new Object[dsTotalData.Tables[0].Columns.Count]);
                        }
                    }
                    for (int i = 0; i < gvTotal[count].Rows.Count; i++)
                    {
                        for (int j = 0; j < gvTotal[count].Rows[i].Cells.Count; j++)
                        {
                            if (j == 0)
                            {
                                dsTotalData.Tables[0].Rows[i][j] = gvTotal[count].Rows[i].Cells[j].Text;
                            }
                            else
                            {
                                int sum = int.Parse(gvTotal[count].Rows[i].Cells[j].Text);
                                int sumTotal = int.Parse(dsTotalData.Tables[0].Rows[i][j].ToString());
                                dsTotalData.Tables[0].Rows[i][j] = (sum + sumTotal);
                            }
                        }
                    }*/
                    //by yyan item8 20110622 del end
                    //by yyan item8 20110622 add start
                    /*if (gvTotalEur[count].Rows.Count > dsTotalData.Tables[0].Rows.Count)
                    {
                        int sub = gvTotalEur[count].Rows.Count - dsTotalData.Tables[0].Rows.Count;
                        for (int i = 0; i < sub; i++)
                        {
                            dsTotalData.Tables[0].Rows.Add(new Object[dsTotalData.Tables[0].Columns.Count]);
                        }
                    }
                    for (int i = 0; i < gvTotalEur[count].Rows.Count; i++)
                    {
                        for (int j = 0; j < gvTotalEur[count].Rows[i].Cells.Count; j++)
                        {
                            if (j == 0)
                            {
                                dsTotalData.Tables[0].Rows[i][j] = gvTotalEur[count].Rows[i].Cells[j].Text;
                            }
                            else
                            {
                                int sum = int.Parse(gvTotalEur[count].Rows[i].Cells[j].Text);
                                int sumTotal = int.Parse(dsTotalData.Tables[0].Rows[i][j].ToString());
                                dsTotalData.Tables[0].Rows[i][j] = (sum + sumTotal);
                            }
                        }
                    }*/
                    //by yyan item8 20110622 add end
                    //by ryzhang item49 20100530 add end
                }
            }
            //by ryzhang item49 20100530 modify start
            //DataSet dsTotalD = new DataSet();
            //GridView gvTotalD = new GridView();
            //DataSet dsTotalData = new DataSet();
            //GridView gvTotalData = new GridView();

            //gvTotalD = new GridView();
            //web.setProperties(gvTotalD);
            //gvTotalData = new GridView();
            //web.setProperties(gvTotalData);

            //dsTotalD = sql.getBookingsData(dsPro, getSegmentID());
            //DataSet ds_budgetTotal = sql.getBookingsBudgetTotal(dsPro, getSegmentID());
            if (dsTotalD.Tables[0].Rows.Count > 0)
            {
                //By FXW 20110517 ITEM22 DEl Start
                //gvTotalD = bindDataByOperation(dsTotalD, gvTotalD, "<br />" + "Total " + getSegmentDec(getSegmentID()) + " (kEUR)", null);
                //By FXW 20110517 ITEM22 DEl End
                //By FXW 20110517 ITEM22 ADD Start
                //gvTotalD = bindDataByOperation(dsTotalD, gvTotalD, "<br />", null);
                gvTotalD.DataSource = dsTotalD;
                gvTotalD.DataBind();
                gvTotalD.HeaderRow.Cells[0].Text = "Total " + getSegmentDec(getSegmentID()) + " (kEUR)";
                gvTotalD.Style.Clear();
                gvTotalD.Style.Add("border", "#000 solid 1px");
                gvTotalD.Style.Add("border-collapse", "collapse");
                gvTotalD.Style.Add("font-size", "12px");
                gvTotalD.HeaderRow.Style.Add("background", "#000");
                for (int i = 0; i < gvTotalD.Rows.Count; i++)
                {
                    //by yyan item8 20110704 add start
                    gvUpdateCell(gvTotalD, i);
                    //by yyan item8 20110704 add end
                    foreach (TableCell cell in gvTotalD.Rows[i].Cells)
                    {
                        cell.Style.Clear();
                        cell.Style.Add("background", "#ccffff");
                        if (p % 2 == 0)
                        {
                            cell.Style.Add("background", "#FF9");
                        }
                    }
                }
                //By FXW 20110517 ITEM22 ADD End
                tc.Controls.Add(gvTotalD);
            }
            //dsTotalData = sql.getBookingsDataTotal(dsPro, getSegmentID());
            if (dsTotalData.Tables[0].Rows.Count > 0)
            {
                //gvTotalData = bindDataByOperation(dsTotalData, gvTotalData, "", ds_budgetTotal);
                //By FXW 20110517 ITEM22 ADD Start
                gvTotalData.DataSource = dsTotalData;
                gvTotalData.DataBind();
                gvTotalData.Style.Clear();
                gvTotalData.Style.Add("border", "#000 solid 1px");
                gvTotalData.Style.Add("border-collapse", "collapse");
                gvTotalData.Style.Add("font-size", "12px");
                gvTotalData.HeaderRow.Style.Add("background", "#000");
                for (int i = 0; i < gvTotalData.Rows.Count; i++)
                {
                    //by yyan item8 20110704 add start
                    gvUpdateCell(gvTotalData, i);
                    //by yyan item8 20110704 add end
                    foreach (TableCell cell in gvTotalData.Rows[i].Cells)
                    {
                        cell.Style.Clear();
                        cell.Style.Add("background", "#ccffff");
                        if (p % 2 == 0)
                        {
                            cell.Style.Add("background", "#FF9");
                        }
                    }
                }
                //By FXW 20110517 ITEM22 ADD End
                tc.Controls.Add(gvTotalData);
            }
            //by ryzhang item49 20100530 modify end
            tr.Controls.Add(tc);
        }
    }

    /* One page to another page */

    protected void lbtn_grSales_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/RSM/RSMGrossSales.aspx?SegmentID=" + getSegmentID());
    }

    protected void lbtn_grBKG_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/RSM/RSMGrossBookings.aspx?SegmentID=" + getSegmentID());
    }

    protected void lbtn_opSales_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/RSM/RSMOperationSales.aspx?SegmentID=" + getSegmentID());
    }

    protected void lbtn_opBKG_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/RSM/RSMOperationalBookings.aspx?SegmentID=" + getSegmentID());
    }

    protected void lbtn_SO_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/RSM/RSMBookingBySalesOrg.aspx?SegmentID=" + getSegmentID());
    }

    #region
    public override void VerifyRenderingInServerForm(Control control)
    {
        // Confirms that an HtmlForm control is rendered for
    }

    protected void btn_export_Click(object sender, EventArgs e)
    {
        TableRow tr5 = new TableRow();
        table_GrBKG.Rows.Add(tr5);
        TableCell tc5 = new TableCell();
        tc5.Text = label_show.Text;
        tr5.Controls.Add(tc5);
        //TableRow tr1 = new TableRow();
        //table_GrBKG.Rows.Add(tr1);
        TableRow tr = new TableRow();
        table_GrBKG.Rows.Add(tr);
        table_GrBKG.Visible = true;
        TableCell tc = new TableCell();
        tc.BorderColor = System.Drawing.Color.Black;
        tc.Font.Bold = true;
        tc.BorderWidth = 1;
        tc.BackColor = System.Drawing.Color.LawnGreen;
        if (lblEUR.Text == "")
        {
            tc.Text = "GR-BKG(" + getSegmentDec(getSegmentID()) + ")" + " Values in Local";
        }
        else
        {
            tc.Text = "GR-BKG(" + getSegmentDec(getSegmentID()) + ")" + " Values in EURO";
        }
        tr.Controls.Add(tc);
        //by yyan item8 20110622 add start 
        if (lblEUR.Text == "")
        { bindDataSource(0); }
        else
        { bindDataSource(1); }
        //by yyan item8 20110622 add end 
        //by yyan item8 20110622 del start 
        //bindDataSource();
        //by yyan item8 20110622 del end 
        cf.ToExcel(table_GrBKG, "GR-BKG(" + getSegmentDec(getSegmentID()) + ").xls");
    }
    #endregion
    //by yyan item8 20110622 add start 
    protected void btnEUR_Click(object sender, EventArgs e)
    {
        bindDataSource(1);
        lblEUR.Text = "The amount is calculated based on EUR.";
    }
    //by yyan item8 20110622 add end 

    //by yyan item8 20110704 add start
    protected void gvUpdateCell(GridView gvWindow, int i)
    {
        //if (meeting.getmonth() == "1")
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
        bindDataSource(0);
        lblEUR.Text = "";
    }
    //by yyan itemw90 20110805 add end 
}
