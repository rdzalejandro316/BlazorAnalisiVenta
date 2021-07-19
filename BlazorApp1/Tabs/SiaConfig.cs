using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorApp1.Tabs
{
    public class SiaConfig
    {

        [Inject]
        public static IConfiguration Configuration { get; set; }
        public static string con { get; set; } = "Data Source=64.250.116.210,8334;Initial Catalog=Milanelo_Emp010;User ID=sa;Password=Q1w2e3r4*/*;";
        public SiaConfig()
        {
        
        }

        public static DataTable SqlDT(string _sql)
        {
            //con = Configuration.GetConnectionString("DefaultConnectionEmpresa");

            DataTable dt = new DataTable();
            try
            {

                using (SqlConnection sqlCon = new SqlConnection(con))
                {
                    using (SqlDataAdapter SqlDa = new SqlDataAdapter(_sql, sqlCon))
                    {
                        SqlDa.Fill(dt);
                    }

                }
            }
            catch (SqlException w)
            {
                Console.WriteLine("erro en sql:" + w);
                dt = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine("error :" + ex);
                dt = null;
            }
            return dt;
        }




    }
}
