using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using TMSDeloitte.Helper;

namespace TMSDeloitte.DAL
{
    public class SQL_DAL
    {
        string connectionString = "";

        public  SQL_DAL ()
        {
            Encrypt_Decrypt EncryptDecrypt = new Encrypt_Decrypt();
            connectionString = "";
            string SQL_DataSource = ConfigurationManager.AppSettings["SQL_DataSource"];
            string SQL_DB = ConfigurationManager.AppSettings["SQL_DB"];
            string SQL_User = ConfigurationManager.AppSettings["SQL_User"];
            string SQL_Password = ConfigurationManager.AppSettings["SQL_Password"];

            string SQLConString = ConfigurationManager.AppSettings["SQLConString"];
            SQLConString = SQLConString.Replace("[SQL_DataSource]", EncryptDecrypt.DecryptString(SQL_DataSource));
            SQLConString = SQLConString.Replace("[SQL_DB]", EncryptDecrypt.DecryptString(SQL_DB));
            SQLConString = SQLConString.Replace("[SQL_User]", EncryptDecrypt.DecryptString(SQL_User));
            SQLConString = SQLConString.Replace("[SQL_Password]", EncryptDecrypt.DecryptString(SQL_Password));
            connectionString = SQLConString;
        }

        public  DataSet GetDataSet(string SPName, SqlParameter[] parameters, string docType = "")
        {
            DataSet ds = new DataSet();
            SqlConnection con = new SqlConnection(); 
            try
            {
                using (con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(SPName, con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        if (parameters != null)
                        {
                            //cmd.Parameters.Add(parameters);

                            foreach (SqlParameter parm in parameters)
                                cmd.Parameters.Add(parm);
                        }

                        con.Open();
                        SqlDataAdapter adp = new SqlDataAdapter(cmd);
                        adp.Fill(ds);
                       
                    }
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile("SQL DB exception: " + ex.Message.ToString());
                if (docType != "")
                    log.InputOutputDocLog(docType, "Getting Data From SQL : Exception, " + ex.Message.ToString());
            }
            finally
            {
                con.Close();
                con.Dispose();
            }
            return ds;
        }
    }
}