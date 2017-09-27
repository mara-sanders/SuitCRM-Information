using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using MySql.Data;
using System.Web.Script.Services;
using System.Web.Script.Serialization;
using ClosedXML.Excel;
using System.IO;
using System.ComponentModel;
using Newtonsoft.Json;
using System.Net;


/// <summary>
/// Summary description for WebService
/// </summary>
[WebService(Namespace = "SuiteCRM_Dashboard")]

[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line.
[System.Web.Script.Services.ScriptService]
public class WebService : System.Web.Services.WebService
{
    public static string jsonString;
    //throws error - public string ConnectionStringHome = System.Configuration.ConfigurationManager.ConnectionStrings["SuiteCRM_Dashboard"].ToString();
    //System.NullReferenceException: Object reference not set to an instance of an object.
    public string ConnectionStringHome = "Data Source=.;Initial Catalog=SuiteCRM_Dashboard; Integrated Security=False;User ID=Mara.Sanders;Password=P@ssword1";
    //"Data Source=.;Initial Catalog=MARS2;integrated security=False;User ID=MarsUser;Password=P@ssWord86452664327";//"Provider=sqloledb;Data Source=.;Initial Catalog=MARS2;User Id=qwerty;Password=123123;";
    //host=10.1.10.67;Port=3306;oldguids=true";Initial Catalog=bitnami_suitecrm;


    public WebService()
    {
        //Uncomment the following line if using designed components
        //InitializeComponent();
    }

    public DataTable GetData(string TableOrSPName, string SP_Param, bool bIsStoredProcedure)
    {
        DataTable dataTable = new DataTable();
        string connect = ConnectionStringHome;
        //   using (SqlConnection conn = new SqlConnection(connect)) - changed to MySql
          using (MySqlConnection conn = new MySqlConnection(connect))
   //     using (MySqlConnection conn = new MySql.Data.MySqlClient.MySqlConnection(connect)) - doesn't make a visible difference
        {
            MySqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "select * from " + TableOrSPName;

            cmd.CommandType = CommandType.Text;
            try
            {
                conn.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0} exception caught.", ex);
            };
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }

            //OleDbDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            MySqlDataAdapter da = new MySqlDataAdapter(cmd);
            //dataTable.Load(dr);
            da.Fill(dataTable);
            conn.Close();
            da.Dispose();
        }

        return dataTable;
    }


    public String ConvertDataTableTojSonString(DataTable dataTable)
    {
        System.Web.Script.Serialization.JavaScriptSerializer serializer =
               new System.Web.Script.Serialization.JavaScriptSerializer();

        List<Dictionary<String, Object>> tableRows = new List<Dictionary<String, Object>>();

        Dictionary<String, Object> row;

        foreach (DataRow dr in dataTable.Rows)
        {
            row = new Dictionary<String, Object>();
            foreach (DataColumn col in dataTable.Columns)
            {
                row.Add(col.ColumnName, dr[col]);
            }
            tableRows.Add(row);
        }
        serializer.MaxJsonLength = Int32.MaxValue;
        return serializer.Serialize(tableRows);
    }

    [WebMethod]
    [ScriptMethod(UseHttpGet = true, ResponseFormat = ResponseFormat.Json)]
    public void GetTableData(String tableName, String conString)
    {
        GetJSONDataFromTableOrSP(tableName, null, false);
    }

    [WebMethod]
    [ScriptMethod(UseHttpGet = true, ResponseFormat = ResponseFormat.Json)]
    public void GetDataFiltered(String source, String filters)
    {
        //GetJSONDataFromTableOrSP(tableName, null, false);
    }

    [WebMethod]
    [ScriptMethod(UseHttpGet = true, ResponseFormat = ResponseFormat.Json)]
    public void GetSPData(String SPName, String SP_parameter)
    {
        GetJSONDataFromTableOrSP(SPName, SP_parameter, true);
    }

    public void GetJSONDataFromTableOrSP(string theTableOrSPName, string SP_parameter, bool bIsStoredProcedure)
    {
        Context.Response.Clear();
        Context.Response.ContentType = "application/json";
        String data = ConvertDataTableTojSonString(GetData(theTableOrSPName, SP_parameter, bIsStoredProcedure));
        Context.Response.Write(data);
    }

    public Stream GetStream(XLWorkbook excelWorkbook)
    {
        Stream fs = new MemoryStream();
        excelWorkbook.SaveAs(fs);
        fs.Position = 0;
        return fs;
    }

    [WebMethod]
    [ScriptMethod(UseHttpGet = true)]
    public string setJSON(string data)
    {
        jsonString = data;

        using (StreamWriter _testData = new StreamWriter(Server.MapPath("~/data.txt"), true))
        {
            _testData.WriteLine(jsonString); // Write the file.
        }

        return jsonString;
    }


    [System.Web.Services.WebMethod]
    public void GetXLSDataFromJSON(string jsonString)
    {
        //Context.Response.Clear();
        //Context.Response.ContentType = "application/json";



        System.Web.Script.Serialization.JavaScriptSerializer serializer =
               new System.Web.Script.Serialization.JavaScriptSerializer();
        //DataTable data = GetData(theTableOrSPName, SP_parameter, bIsStoredProcedure);

        //DataTable dt = data;

        DataTable dt = (DataTable)JsonConvert.DeserializeObject(jsonString, (typeof(DataTable)));

        XLWorkbook workbook = new XLWorkbook();
        workbook.Worksheets.Add(dt, "WorksheetName");
        workbook.Save();
    }


    public DataTable GetDataTableFromTableOrSP(string theTableOrSPName, string SP_parameter, bool bIsStoredProcedure)
    {
        Context.Response.Clear();
        Context.Response.ContentType = "application/json";
        return GetData(theTableOrSPName, SP_parameter, bIsStoredProcedure);
    }

}
