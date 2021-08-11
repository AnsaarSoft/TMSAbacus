using Sap.Data.Hana;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Data.Odbc;
using System.Web;
using TMSDeloitte.Helper;

namespace TMSDeloitte.DAL
{
    public class Hana_DataContext
    {
        internal static HanaCommand OpenConnection(string connectionString, string docType = "")
        {
            Log log = new Log();
            try
            {
                //HanaConnection loConnection = new HanaConnection(connectionString);
                OdbcConnection odbCon = new OdbcConnection(connectionString);
                
                odbCon.Open();
                HanaCommand loCommand = new HanaCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    //Connection = loConnection
                };
                //HanaConnection.ClearPool(loConnection);
                //HanaConnection.ClearAllPools();
                //if (loCommand.Connection.State == ConnectionState.Open)
                if(odbCon.State == ConnectionState.Open)
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

        internal static HanaCommand SetStoredProcedure(HanaCommand loCommand, string schemaName, string storedProcedureName)
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


        internal static DataTable GetDateTable(HanaCommand loCommand, string docType = "")
        {
            Log log = new Log();
            try
            {
                DataTable loDataTable = new DataTable();
                HanaDataAdapter loAdapter = new HanaDataAdapter(loCommand);

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

        internal static DataSet GetDateSet(HanaCommand loCommand, string docType = "")
        {

            Log log = new Log();
            try
            {
                DataSet loDataTable = new DataSet();
                HanaDataAdapter loAdapter = new HanaDataAdapter(loCommand);
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