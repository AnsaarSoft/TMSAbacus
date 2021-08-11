using Sap.Data.Hana;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Data.Odbc;
using System.Web;
using TMSDeloitte.Helper;
using TMSDeloitte.Models;

namespace TMSDeloitte.DAL
{
    public class Hana_DataContext_ODBC
    {
        

        internal static OdbcCommand OpenConnectionOdbc(string connectionString, string docType = "")
        {
            Log log = new Log();
            try
            {
                OdbcConnection odbCon = new OdbcConnection(connectionString);

                odbCon.Open();
                OdbcCommand loCommand = new OdbcCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    Connection = odbCon
                };
                //HanaConnection.ClearPool(loConnection);
                //HanaConnection.ClearAllPools();
                //if (loCommand.Connection.State == ConnectionState.Open)
                if (odbCon.State == ConnectionState.Open)
                {
                    if (docType != "")
                        log.InputOutputDocLog(docType, "Closing connection");

                    //loCommand.Connection.Close();
                    odbCon.Close();

                    if (docType != "")
                        log.InputOutputDocLog(docType, "Closing connection : Success");
                }

                if (docType != "")
                    log.InputOutputDocLog(docType, "Opening new connection");

                //loCommand.Connection.Open();
                odbCon.Open();

                if (docType != "")
                    log.InputOutputDocLog(docType, "Opening new connection : Success");
                return loCommand;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        internal static OdbcCommand SetStoredProcedure(OdbcCommand loCommand, string schemaName, string storedProcedureName)
        {
            try
            {
                loCommand.CommandText = "call  \"" + schemaName + "\".\"" + storedProcedureName + "\"()";
                return loCommand;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        internal static OdbcCommand SetStoredProcedureWithParameters(OdbcCommand loCommand, string schemaName, string storedProcedureName, List<B1SP_Parameter> parameterList)
        {
            try
            {
                string commText = "";
                
                foreach (var p in parameterList)
                {
                    if (p.ParameterType == DBTypes.String.ToString())
                    {
                        commText += "'" + p.ParameterValue + "',";
                    }
                    else if (p.ParameterType == DBTypes.Bool.ToString())
                    {
                        //if values is 0 then false 
                        //if value is 1 then true
                        commText += "" + p.ParameterValue + ",";
                    }
                    else
                    {
                        commText += "" + p.ParameterValue + ",";
                    }
                    
                }
                if (commText.EndsWith(","))
                {
                    commText = commText.Remove(commText.Length - 1, 1);
                }
                commText += ")";
                loCommand.CommandText = "call  \"" + schemaName + "\".\"" + storedProcedureName + "\"(" + commText;
                return loCommand;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        internal static OdbcCommand SetStoredProcedureOdbc(OdbcCommand loCommand, string schemaName, string storedProcedureName)
        {
            try
            {
                loCommand.CommandText = " \"" + schemaName + "\".\"" + storedProcedureName + "\"";
                return loCommand;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        internal static DataTable GetDateTable(OdbcCommand loCommand, string docType = "")
        {
            Log log = new Log();
            try
            {
                DataTable loDataTable = new DataTable();
                OdbcDataAdapter loAdapter = new OdbcDataAdapter(loCommand);

                if (docType != "")
                    log.InputOutputDocLog(docType, "Filling Data Table");

                loAdapter.Fill(loDataTable);

                if (docType != "")
                    log.InputOutputDocLog(docType, "Filling Data Table: Success");

                return loDataTable;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        internal static DataTable GetDateTableOdbc(OdbcCommand loCommand, string docType = "")
        {
            Log log = new Log();
            try
            {
                DataTable loDataTable = new DataTable();
                OdbcDataAdapter loAdapter = new OdbcDataAdapter(loCommand);

                if (docType != "")
                    log.InputOutputDocLog(docType, "Filling Data Table");

                loAdapter.Fill(loDataTable);

                if (docType != "")
                    log.InputOutputDocLog(docType, "Filling Data Table: Success");

                return loDataTable;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        internal static DataSet GetDateSet(OdbcCommand loCommand, string docType = "")
        {

            Log log = new Log();
            try
            {
                DataSet loDataTable = new DataSet();
                OdbcDataAdapter loAdapter = new OdbcDataAdapter(loCommand);
                if (docType != "")
                    log.InputOutputDocLog(docType, "Filling Dataset");

                loAdapter.Fill(loDataTable);

                if (docType != "")
                    log.InputOutputDocLog(docType, "Filling Dataset : Success");
                return loDataTable;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}