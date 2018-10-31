using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace KeySign
{
    class SQLClass
    {
        public static string connsql = @"server=127.0.0.1;Database=dmkeybase;uid=root;pwd=Shanghai804";
        public static void SqlExcuteCMD(String CmdStr)
        {
            using (MySqlConnection con = new MySqlConnection(connsql))
            using (MySqlCommand cmd = new MySqlCommand(CmdStr, con))
            {
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }


        public static DataTable ExcuteQueryUsingDataReader(string myQuery)
        {
            DataTable dt = new DataTable();
            using (MySqlConnection con = new MySqlConnection(connsql))
            using (MySqlCommand cmd = new MySqlCommand(myQuery, con))
            {
                con.Open();
                using (var dr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    if (dr.HasRows)
                    {
                        dt.Load(dr);
                    }
                }
                return dt;
            }
        }

        public static DataTable ExcuteQueryUsingDataAdapter(string myQuery)
        {
            using (MySqlConnection con = new MySqlConnection(connsql))
            using (MySqlDataAdapter dap = new MySqlDataAdapter(myQuery, con))
            {
                DataTable dt = new DataTable();
                dap.Fill(dt);
                return dt;
            }
        }




    }
}
