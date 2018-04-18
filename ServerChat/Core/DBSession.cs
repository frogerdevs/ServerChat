using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerChat.Core
{
    public class DBSession
    {
        string dbServerConnectionString ;

        public DBSession()
        {
            dbServerConnectionString = ConfigurationManager.AppSettings["DbServerConnectionString"];
        }
        public DataTable GetWebsockets(int projectid, string id)
        {
            SqlConnection conn = null;
            DataTable dt = new DataTable();
            SqlDataAdapter adp = new SqlDataAdapter();
            try
            {
                try
                {

                    conn = new SqlConnection(dbServerConnectionString);
                    conn.Open();


                    adp.SelectCommand = new SqlCommand("sp_GetWebSockets");
                    adp.SelectCommand.CommandType = CommandType.StoredProcedure;
                    adp.SelectCommand.Connection = conn;
                    adp.SelectCommand.Parameters.Add("@ProjectID", SqlDbType.Int).Value = projectid;
                    adp.SelectCommand.Parameters.Add("@id", SqlDbType.NVarChar, 50).Value = id;
                    adp.Fill(dt);
                    return dt;
                }
                catch (Exception ex)
                {
                    throw new Exception("GetWebsockets !\nError: " + ex.Message);

                }
                finally
                {
                    // Cleaning UP!
                    if (adp.SelectCommand != null)
                        adp.SelectCommand.Dispose();
                    if (conn != null)
                    {
                        conn.Close();
                        conn.Dispose();
                    }

                }

            }
            catch (Exception ex)
            {

                System.Console.WriteLine(string.Format("GetWebsockets Error : {0}", ex));
            }
            finally
            {
                string output = ConvertDatatableToString(dt);
                System.Console.WriteLine(string.Format("Get Websockets  : {0}", output));

            }
            return dt;

        }

        public DataTable GetWebsocketsDataTable()
        {
            DataTable dt = new DataTable();
            try
            {
                dt.Columns.Add("WebsocketsId", typeof(int));
                dt.Columns.Add("Id", typeof(string));
                dt.Columns.Add("SocketIp", typeof(string));
                dt.Columns.Add("SocketPort", typeof(string));
                dt.Columns.Add("MaxConnections", typeof(int));

                dt.Rows.Add(8, "CHAT", "192.168.3.55", "61194", 1);
                //dt.Rows.Add(8, "CHAT", "192.168.3.73", "61194", 3);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            return dt;
        }
        string ConvertDatatableToString(DataTable Ds)
        {
            string OUT = "";

            for (int r = 0; r < Ds.Rows.Count; r++)
            {
                for (int c = 0; c < Ds.Columns.Count; c++)
                {
                    string s = string.Format(@"{0} = {1} {2}", Ds.Columns[c].ToString(), Ds.Rows[r][c].ToString(), Environment.NewLine);
                    OUT += s;
                }
            }

            return OUT;
        }

    }
}
