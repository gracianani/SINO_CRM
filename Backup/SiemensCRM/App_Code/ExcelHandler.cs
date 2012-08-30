using System;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI.WebControls;
using org.in2bits.MyXls;
/// <summary>
/// the helper for operating excel file.
/// </summary>
public class ExcelHandler
{
    /// <summary>
    /// import from excel file to DataTable.
    /// </summary>
    /// <param name="fileUpload">FileUpload control upload excel file.</param>
    /// <param name="sheetIndex">sheet page index</param>
    /// <returns>DataTable</returns>
    public DataTable importExcel(FileUpload fileUpload, int sheetIndex)
    {
        DataTable sheetInfo = null;
        if (isExcel(fileUpload))
        {
            // upload file
            string serverFolderPath = HttpContext.Current.Request.PhysicalApplicationPath + "upload\\";
            string serverFileName = fileUpload.FileName.Insert(fileUpload.FileName.LastIndexOf('.'),
                                                               DateTime.Now.Ticks.ToString());
            string filePath = serverFolderPath + serverFileName;
            fileUpload.SaveAs(filePath);
            // get sheet pages
            string[] sheetNameArr = getExcelSheetNames(filePath);

            if (sheetNameArr != null && sheetIndex <= sheetNameArr.Length - 1)
            {
                sheetInfo = GetAllDataInfo(filePath, sheetNameArr[sheetIndex]);
                File.Delete(filePath);
            }
        }
        return sheetInfo;
    }

    /// <summary>
    /// read the content of excel file into DataTable
    /// </summary>
    /// <param name="filePath">excel file path</param> 
    /// <param name="sheetName">sheet name</param> 
    /// <returns>DataTable</returns>
    public static DataTable GetAllDataInfo(string filePath, string sheetName)
    {
        var dataTable = new DataTable();
        string mystring = " Provider = Microsoft.Jet.OLEDB.4.0 ; Data Source =" + filePath +
                          ";Extended Properties='Excel 8.0;FMT=Delimited;HDR=NO;IMEX=1';";
        var cnnxls = new OleDbConnection(mystring);
        var myDa = new OleDbDataAdapter("select * from [" + sheetName + "]", cnnxls);
        var myDs = new DataSet();
        myDa.Fill(myDs);
        dataTable = myDs.Tables[0];
        return dataTable;
    }

    /// <summary>
    /// export excel file
    /// </summary>
    /// <param name="excelInfo">DataTable need to be export.</param>
    public void exportExcel(DataTable excelInfo)
    {
        var xls = new XlsDocument();
        // set excel file
        xls.FileName = HttpContext.Current.Server.UrlEncode(excelInfo.TableName) + ".xls"; //;"Detail.xls";
        // create excel sheet page
        Worksheet sheet = xls.Workbook.Worksheets.Add("Sheet1");
        // set column width
        var ci = new ColumnInfo(xls, sheet);
        ci.ColumnIndexStart = 1;
        ci.ColumnIndexEnd = (ushort) (excelInfo.Columns.Count - 1);
        ci.Width = 13*256;
        sheet.AddColumnInfo(ci);
        // output content
        Cells cells = sheet.Cells;
        // set title style
        XF xf = xls.NewXF();
        xf.HorizontalAlignment = HorizontalAlignments.Centered;
        xf.VerticalAlignment = VerticalAlignments.Centered;
        // output title
        cells.Merge(1, 1, 1, excelInfo.Columns.Count);
        cells.Add(1, 1, excelInfo.TableName, xf);
        // append cell style
        xf.UseBorder = true;
        xf.TopLineStyle = 1;
        xf.BottomLineStyle = 1;
        xf.LeftLineStyle = 1;
        xf.RightLineStyle = 1;
        // output data
        for (int i = 0; i < excelInfo.Columns.Count; i++)
        {
            // output output table header
            cells.Add(2, i + 1, excelInfo.Columns[i].ColumnName, xf);
            // output data
            for (int j = 0; j < excelInfo.Rows.Count; j++)
            {
                // check data type
                if (IsFloat(excelInfo.Rows[j][i].ToString()))
                {
                    cells.Add(j + 3, i + 1, Convert.ToDecimal(excelInfo.Rows[j][i]), xf);
                }
                else
                {
                    cells.Add(j + 3, i + 1, excelInfo.Rows[j][i].ToString(), xf);
                }
            }
        }
        xls.Send();
    }

    /// <summary>
    /// check if a float number
    /// </summary>
    /// <param name="str">input string</param>
    /// <returns>result</returns>
    public static bool IsFloat(string str)
    {
        var reg1
            = new Regex(@"^(-?\d+)(\.\d+)?$");
        return reg1.IsMatch(str);
    }

    /// <summary>
    /// get excel sheet names.
    /// </summary>
    /// <param name="filePath">excel file path.</param>
    /// <returns>sheet names array.</returns>
    private string[] getExcelSheetNames(string filePath)
    {
        OleDbConnection objConn = null;
        DataTable dt = null;
        try
        {
            string connString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + filePath +
                                ";Extended Properties=Excel 8.0;";
            objConn = new OleDbConnection(connString);
            objConn.Open();
            dt = objConn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
            string[] excelSheets = null;
            if (dt != null)
            {
                excelSheets = new string[dt.Rows.Count];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    excelSheets[i] = dt.Rows[i]["TABLE_NAME"].ToString();
                }
            }
            return excelSheets;
        }
        catch (Exception e)
        {
            throw e;
        }
        finally
        {
            if (objConn != null)
            {
                objConn.Close();
            }
        }
    }

    /// <summary>
    /// check if upload file is a excel file.
    /// </summary>
    /// <param name="fileUpload">FileUpload control upload file.</param>
    /// <returns>check result</returns>
    public bool isExcel(FileUpload fileUpload)
    {
        if (string.IsNullOrEmpty(fileUpload.FileName))
        {
            return false;
        }
        else if (fileUpload.FileName.LastIndexOf('.') == -1)
        {
            return false;
        }
        else if (string.Equals(fileUpload.FileName.Substring(fileUpload.FileName.LastIndexOf('.')), ".xls"))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    
    /// <summary>
    /// check dataTable content format
    /// </summary>
    /// <param name="ds">DataTable need to be checked.</param>
    /// <param name="iNum">num of datatable columns</param>
    /// <returns>check result</returns>
    public bool isForm(DataTable ds, int iNum)
    {
        if (ds.Columns.Count != iNum)
        {
            return false;
        }
        for (int i = 0; i < ds.Columns.Count - 1; i++)
        {
            if (ds.Rows[0][i].ToString().Equals(""))
            {
                return false;
            }
        }
        return true;
    }

}