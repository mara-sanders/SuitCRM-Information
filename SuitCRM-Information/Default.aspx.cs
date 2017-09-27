using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Configuration;
using MySql.Data.MySqlClient;
using System.Diagnostics;

namespace SuitCRM_Information
{
    public partial class _Default : Page
    {
        MySql.Data.MySqlClient.MySqlConnection conn;
        MySql.Data.MySqlClient.MySqlCommand cmd;
        MySql.Data.MySqlClient.MySqlDataReader reader;
        protected string name { get; set; }
        String queryStr;
        //protected void Page_Load(object sender, EventArgs e)
        //{
        //}
        //    protected void getData(object sender, EventArgs e)

        //    {
        //        if (!this.IsPostBack)
        //        {
        //            string constr = ConfigurationManager.ConnectionStrings["SuiteCRM_Dashboard"].ConnectionString;
        //            using (MySqlConnection con = new MySqlConnection(constr))
        //            {
        //                using (MySqlCommand cmd = new MySqlCommand("SELECT * FROM bitnami_suitecrm.accounts"))
        //                {
        //                    using (MySqlDataAdapter sda = new MySqlDataAdapter())
        //                    {
        //                        cmd.Connection = con;
        //                        sda.SelectCommand = cmd;
        //                        using (DataTable dt = new DataTable())
        //                        {
        //                            //sda.Fill(dt);
        //                            System.Diagnostics.Debug.WriteLine("DT + "+dt.ToString());
        //                            //GridView1.DataSource = dt;
        //                            //GridView1.DataBind();
        //                        }
        //                    }
        //                }
        //        }
        //    }
        //}

        protected void Page_Load(object sender, EventArgs e)
        {

        }
        public string data;

        protected void getData(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("hello");
            Console.WriteLine("hello2");

            String connString = System.Configuration.ConfigurationManager.ConnectionStrings["SuiteCRM_Dashboard"].ToString();

            conn = new MySql.Data.MySqlClient.MySqlConnection(connString);
            conn.Open();
            queryStr = "";
            queryStr = "SELECT * FROM bitnami_suitecrm.users";
            cmd = new MySql.Data.MySqlClient.MySqlCommand(queryStr, conn);


            name = "";
            //SqlDataReader reader = cmd.ExecuteReader();
            reader = cmd.ExecuteReader();

            try
            {
                reader.Read();
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0} exception caught.", ex);   
            };
            //reader.Read();
            name = reader.GetString(reader.GetOrdinal("user_name"));
            Console.WriteLine(name);
            //dataTextBox.Text = name;

            //while (reader.HasRows && reader.Read())

            //{

            //    name = reader.GetString(reader.GetOrdinal("name"));
            //    Console.WriteLine(name);
            //    dataTextBox.Text = name;

            //}
            //if (!reader.HasRows)
            //{
            //    dataTextBox.Text = "NO ROWS";
            //    Console.WriteLine("NO ROWS FOUND");
            //}


            //SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection)

            reader.Close();

            conn.Close();
        }
    }
}