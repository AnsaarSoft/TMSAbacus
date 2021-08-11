using Sap.Data.Hana;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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
                HanaConnection loConnection = new HanaConnection(connectionString);
                HanaCommand loCommand = new HanaCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    Connection = loConnection
                };
                //HanaConnection.ClearPool(loConnection);
                //HanaConnection.ClearAllPools();
                if (loCommand.Connection.State == ConnectionState.Open)
                {
                    if (docType != "")
                        log.InputOutputDocLog(docType, "Closing connection");

                    loCommand.Connection.Close();

                    if (docType != "")
                        log.InputOutputDocLog(docType, "Closing connection : Success");
                }

                if (docType != "")
                    log.InputOutputDocLog(docType, "Opening new connection");

                loCommand.Connection.Open();

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