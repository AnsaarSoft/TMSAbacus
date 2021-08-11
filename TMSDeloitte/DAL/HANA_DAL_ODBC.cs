using Sap.Data.Hana;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Odbc;
using System.Linq;
using System.Web;
using TMSDeloitte.DAL;
using TMSDeloitte.Helper;
using TMSDeloitte.Models;

namespace TMSDeloitte.DAL
{
    public class HANA_DAL_ODBC
    {
        string connectionString = "";
        string schemaName = "";
        string SchemaNameSAPB1 = "";
        public HANA_DAL_ODBC()
        {
            try
            {

                Encrypt_Decrypt EncryptDecrypt = new Encrypt_Decrypt();
                connectionString = "";
                string SERVERNODE = ConfigurationManager.AppSettings["SERVERNODE"];
                string UID = ConfigurationManager.AppSettings["UID"];
                string PWD = ConfigurationManager.AppSettings["PWD"];
                string CS = ConfigurationManager.AppSettings["CS"];
                schemaName = ConfigurationManager.AppSettings["SchemaName"];
                SchemaNameSAPB1 = ConfigurationManager.AppSettings["SchemaNameSAPB1"];

                string HANAConString = ConfigurationManager.AppSettings["HANAConString"];
                HANAConString = HANAConString.Replace("[SERVERNODE]", EncryptDecrypt.DecryptString(SERVERNODE));
                HANAConString = HANAConString.Replace("[UID]", EncryptDecrypt.DecryptString(UID));
                HANAConString = HANAConString.Replace("[PWD]", EncryptDecrypt.DecryptString(PWD));
                HANAConString = HANAConString.Replace("[CS]", EncryptDecrypt.DecryptString(CS));

                connectionString = HANAConString;
            }
           catch(Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
            }

        }

        public DataTable GetDataTableByQuery(string Query, string docType = "")
        {
            Log log = new Log();
            DataTable dt = new DataTable();
            OdbcConnection con = new OdbcConnection(connectionString);
            OdbcDataAdapter adpt = new OdbcDataAdapter(Query, con);
            if (docType != "")
                log.InputOutputDocLog(docType, "Getting Record From Query ,query: " + Query);
            try
            {
                if (con.State == ConnectionState.Open)
                {
                    if (docType != "")
                        log.InputOutputDocLog(docType, "Closing connection");

                    con.Close();

                    if (docType != "")
                        log.InputOutputDocLog(docType, "Closing connection : Success");
                }
                if (docType != "")
                    log.InputOutputDocLog(docType, "Opening new connection");

                con.Open();
                adpt.SelectCommand.CommandTimeout = 10;

                if (docType != "")
                    log.InputOutputDocLog(docType, "Filling data table");

                adpt.Fill(dt);

                if (docType != "")
                    log.InputOutputDocLog(docType, "Getting Record From Query : Success");
            }
            catch (Exception ex)
            {
                log.LogFile(ex.Message);
                if (docType != "")
                    log.InputOutputDocLog(docType, "Getting Record From Query ,Exception: " + ex.Message);
            }
            finally
            {
                con.Close();
                con.Dispose();
                adpt.Dispose();
            }
            return dt;
        }

        public DataTable GetDataTableByStoredProcedure(string spName, string docType = "")
        {
            Log log = new Log();
            DataTable dt = new DataTable();
            OdbcCommand loCommand = new OdbcCommand();
            if (docType != "")
                log.InputOutputDocLog(docType, "Getting Data From Hana");
            try
            {

                loCommand = Hana_DataContext_ODBC.OpenConnectionOdbc(connectionString, docType);
                loCommand = Hana_DataContext_ODBC.SetStoredProcedure(loCommand, schemaName, spName);
                loCommand.CommandTimeout = 8;
               
                dt = Hana_DataContext_ODBC.GetDateTable(loCommand, docType);

                if (docType != "")
                    log.InputOutputDocLog(docType, "Getting Data From Hana : Success");

                if (dt.Rows.Count == 0)
                    throw new Exception("No record found.");

            }
            catch (Exception ex)
            {
                log.LogFile("DB exception: " + ex.Message.ToString());
                if (docType != "")
                    log.InputOutputDocLog(docType, "Getting Data From Hana : Exception, " + ex.Message.ToString());
            }
            finally
            {
                loCommand.Connection.Close();
                loCommand.Dispose();
            }
            return dt;
        }

        public DataTable GetDataTableByStoredProcedureSAPB1(string spName, string docType = "")
        {
            Log log = new Log();
            DataTable dt = new DataTable();
            OdbcCommand loCommand = new OdbcCommand();
            if (docType != "")
                log.InputOutputDocLog(docType, "Getting Data From Hana");
            try
            {

                loCommand = Hana_DataContext_ODBC.OpenConnectionOdbc(connectionString, docType);
                loCommand = Hana_DataContext_ODBC.SetStoredProcedure(loCommand, SchemaNameSAPB1, spName);
                loCommand.CommandTimeout = 8;

                dt = Hana_DataContext_ODBC.GetDateTable(loCommand, docType);

                if (docType != "")
                    log.InputOutputDocLog(docType, "Getting Data From Hana : Success");

                if (dt.Rows.Count == 0)
                    throw new Exception("No record found.");

            }
            catch (Exception ex)
            {
                log.LogFile("DB exception: " + ex.Message.ToString());
                if (docType != "")
                    log.InputOutputDocLog(docType, "Getting Data From Hana : Exception, " + ex.Message.ToString());
            }
            finally
            {
                loCommand.Connection.Close();
                loCommand.Dispose();
            }
            return dt;
        }

        public DataTable GetDataTableByStoredProcedure( string spName, List<B1SP_Parameter> parameterList, string docType = "")
        {
            Log log = new Log();
            DataTable dt = new DataTable();
            OdbcCommand loCommand = new OdbcCommand();
            if (docType != "")
                log.InputOutputDocLog(docType, "Getting Data From Hana");
            try
            {

                loCommand = Hana_DataContext_ODBC.OpenConnectionOdbc(connectionString, docType);
                loCommand = Hana_DataContext_ODBC.SetStoredProcedureWithParameters(loCommand, schemaName, spName,parameterList);
                loCommand.CommandTimeout = 8;
                //foreach (var item in parameterList)
                //{
                //    loCommand.Parameters.AddWithValue(item.ParameterName, item.ParameterValue);
                //}
                //loCommand.CommandText = "call  \"" + schemaName + "\".\"" + spName + "\"()";
                dt = Hana_DataContext_ODBC.GetDateTable(loCommand, docType);

                if (docType != "")
                    log.InputOutputDocLog(docType, "Getting Data From Hana : Success");

                if (dt.Rows.Count == 0)
                    throw new Exception("No record found.");

            }
            catch (Exception ex)
            {
                log.LogFile("DB exception: " + ex.Message.ToString());
                if (docType != "")
                    log.InputOutputDocLog(docType, "Getting Data From Hana : Exception, " + ex.Message.ToString());
            }
            finally
            {
                loCommand.Connection.Close();
                loCommand.Dispose();
            }
            return dt;
        }

        public DataSet GetDataSetByStoredProcedure(string spName, List<B1SP_Parameter> parameterList, string docType = "")
        {
            Log log = new Log();
            DataSet dt = new DataSet();
            OdbcCommand loCommand = new OdbcCommand();
            if (docType != "")
                log.InputOutputDocLog(docType, "Getting Data From Hana");
            try
            {

                loCommand = Hana_DataContext_ODBC.OpenConnectionOdbc(connectionString, docType);
                loCommand = Hana_DataContext_ODBC.SetStoredProcedure(loCommand, schemaName, spName);
                loCommand.CommandTimeout = 8;
                foreach (var item in parameterList)
                {
                    loCommand.Parameters.AddWithValue(item.ParameterName, item.ParameterValue);
                }
                dt = Hana_DataContext_ODBC.GetDateSet(loCommand, docType);

                if (docType != "")
                    log.InputOutputDocLog(docType, "Getting Data From Hana : Success");

                

            }
            catch (Exception ex)
            {
                log.LogFile("DB exception: " + ex.Message.ToString());
                if (docType != "")
                    log.InputOutputDocLog(docType, "Getting Data From Hana : Exception, " + ex.Message.ToString());
            }
            finally
            {
                loCommand.Connection.Close();
                loCommand.Dispose();
            }
            return dt;
        }


        public DataTable AddUpdateDataByStoredProcedure(string spName, List<B1SP_Parameter> parameterList, string docType = "")
        {
            Log log = new Log();
            DataTable dt = new DataTable();
            OdbcCommand loCommand = new OdbcCommand();
            if (docType != "")
                log.InputOutputDocLog(docType, "AddUpdateDataByStoredProcedure");
            try
            {
                loCommand = Hana_DataContext_ODBC.OpenConnectionOdbc(connectionString, docType);
                loCommand = Hana_DataContext_ODBC.SetStoredProcedure(loCommand, schemaName, spName);
                loCommand.CommandTimeout = 8;
                foreach (var item in parameterList)
                {
                    loCommand.Parameters.AddWithValue(item.ParameterName, item.ParameterValue);
                }
                dt = Hana_DataContext_ODBC.GetDateTable(loCommand, docType);

                if (docType != "")
                    log.InputOutputDocLog(docType, "AddUpdateDataByStoredProcedure : Success");


            }
            catch (Exception ex)
            {
                log.LogFile("DB exception: " + ex.Message.ToString());
                if (docType != "")
                    log.InputOutputDocLog(docType, "AddUpdateDataByStoredProcedure : Exception, " + ex.Message.ToString());
            }
            finally
            {
                loCommand.Connection.Close();
                loCommand.Dispose();
            }
            return dt;
        }

    }
}