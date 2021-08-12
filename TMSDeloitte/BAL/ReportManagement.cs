using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using TMSDeloitte.DAL;
using TMSDeloitte.Helper;
using TMSDeloitte.Models;

namespace TMSDeloitte.BAL
{
    public class ReportManagement
    {
        private List<B1SP_Parameter> TranslateIDToParameterList(int Id)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {
                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = Convert.ToString(Id);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ReportManagement", "Exception occured on TranslateIDToParameterList, " + ex.Message);
            }
            return parmList;
        }

        public List<ReportInfo> GetReports(int ID)
        {
            List<ReportInfo> reportSetup = new List<ReportInfo>();
            try
            {
                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                DataTable dt_Report = HANADAL.GetDataTableByStoredProcedure("GetReports", TranslateIDToParameterList(ID), "ReportManagement");

                if (dt_Report.Rows.Count > 0)
                {
                    reportSetup = TranslateDataTableToReportManagementList(dt_Report);
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ReportManagement", "Exception occured on GetReports ID: " + ID + " , " + ex.Message);
            }
            return reportSetup;
        }

        private List<ReportInfo> TranslateDataTableToReportManagementList(DataTable dt)
        {
            List<ReportInfo> reportSetup = new List<ReportInfo>();

            try
            {
                int sno = 1;
                foreach (DataRow dtRow in dt.Rows)
                {
                    ReportInfo report = new ReportInfo();
                    report.SNO = sno;
                    report.KEY = Guid.NewGuid().ToString();
                    report.ID = Convert.ToInt32(dtRow["ID"]);
                    report.ReportCode = Convert.ToString(dtRow["ReportCode"]);
                    report.ReportName = Convert.ToString(dtRow["ReportName"]);
                    report.RptFile = Convert.ToString(dtRow["RptFile"]);
                    report.DocNum = Convert.ToString(dtRow["DocNum"]);
                    report.CreatedBy = Convert.ToInt32(dtRow["CreatedBy"]);
                    report.CreateDate = Convert.ToDateTime(dtRow["CreateDate"]);

                    if (dtRow["UpdatedBy"] != DBNull.Value)
                        report.UpdatedBy = Convert.ToInt32(dtRow["UpdatedBy"]);
                    if (dtRow["UpdateDate"] != DBNull.Value)
                        report.UpdateDate = Convert.ToDateTime(dtRow["UpdateDate"]);

                    report.IsActive = Convert.ToBoolean(dtRow["IsActive"]);
                    report.IsDeleted = Convert.ToBoolean(dtRow["IsDeleted"]);
                    sno = sno + 1;
                    reportSetup.Add(report);
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ReportManagement", "Exception occured on TranslateDataTableToReportManagementList, " + ex.Message);
            }
            return reportSetup;
        }

        public bool AddUpdateReports(out string msg, ReportInfo ReportInfo, int UserID, int ID)
        {
            bool isSuccess = false;
            bool isAddOccured = false;
            bool isUpdateOccured = false;
            bool isDeleteOccured = false;
            msg = "";
            try
            {
                string DocNum = ReportInfo.DocNum;

                int no = 1;
                string docNum = "";
                if (DocNum == "" || DocNum == null)
                {
                    List<string> docNumList = GetReportDocNum();
                    if (docNumList.Count > 0)
                    {
                        var item = docNumList[docNumList.Count - 1];
                        no = Convert.ToInt32(item.Split('-')[1]);
                        no = no + 1;
                        docNum = Convert.ToString(no).PadLeft(5, '0');
                        docNum = "Doc-" + docNum;
                    }
                    else
                    {
                        docNum = Convert.ToString(no).PadLeft(5, '0');
                        docNum = "Doc-" + docNum;
                    }
                }
                else
                {
                    docNum = DocNum;
                }
                List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
                B1SP_Parameter parm = new B1SP_Parameter();
                string approvalCode = "";
                bool isValidateApprovalCode = true;
                try
                {
                    HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();

                    if (ID > 0)
                    {
                        parmList = new List<B1SP_Parameter>();
                        parm = new B1SP_Parameter();
                        parm.ParameterName = "ID";
                        parm.ParameterValue = Convert.ToString(ID);
                        parm.ParameterType = DBTypes.Int32.ToString();
                        parmList.Add(parm);
                        DataSet ds = HANADAL.GetDataSetByStoredProcedure("GetReports", parmList, "ReportManagement");
                        if (ds.Tables.Count > 0)
                        {
                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                foreach (DataRow dtRow in ds.Tables[0].Rows)
                                {
                                    approvalCode = Convert.ToString(dtRow["ReportCode"]);
                                    if (approvalCode.ToLower() == ReportInfo.ReportCode.ToLower())
                                        isValidateApprovalCode = false;
                                    break;
                                }
                            }
                        }
                    }
                    if (isValidateApprovalCode)
                    {
                        parmList = new List<B1SP_Parameter>();
                        parm = new B1SP_Parameter();
                        parm.ParameterName = "ReportCode";
                        parm.ParameterValue = Convert.ToString(ReportInfo.ReportCode);
                        parm.ParameterType = DBTypes.String.ToString();
                        parmList.Add(parm);

                        DataTable dt = HANADAL.GetDataTableByStoredProcedure("ValidateReportCode", parmList, "ReportManagement");
                        if (dt.Rows.Count > 0)
                        {
                            msg = "Report Code Already Exist!";
                            return false;
                        }
                    }
                    ReportInfo.DocNum = docNum;

                    List<ReportInfo> previousData = new List<Models.ReportInfo>();
                    //For Log
                    if (ID > 0)
                    {
                        previousData = GetReports(ID);
                        if (previousData.Count > 0)
                        {
                            //AddApprovalSetup_Log(previousData[0], out isUpdateOccured);
                        }
                    }
                    //ReportInfo.RptFile = Convert.ToString(GetBytesFromFile(ReportInfo.RptFile));
                    DataTable dt_ApprovalSetup = HANADAL.AddUpdateDataByStoredProcedure("AddUpdateReports", TranslateReportsToParameterList(ReportInfo, ID, UserID), "ReportManagement");
                    if (dt_ApprovalSetup.Rows.Count == 0)
                        throw new Exception("Exception occured when AddUpdateReports,  Report Code:" + ReportInfo.ReportCode + " , Report Name" + ReportInfo.ReportName);
                    else
                    {
                        msg = "Successfully Added/Updated";
                        isSuccess = true;
                    }
                }
                catch (Exception ex)
                {
                    msg = "Exception occured in loop Add/Update Reports!";
                    isSuccess = false;
                    Log log = new Log();
                    log.LogFile(ex.Message);
                    log.InputOutputDocLog("ReportManagement", "Exception occured in foreach loop AddUpdateReports, " + ex.Message);
                }

                Common cmn = new Common();
                Task.Run(() => cmn.AddMasterLog(isAddOccured, isUpdateOccured, isDeleteOccured, nameof(Enums.FormsName.ReportUploader), UserID, "ReportManagement"));
                //End MAster Log
            }
            catch (Exception ex)
            {
                msg = "Exception occured in foreach loop Add/Update Reports!";
                isSuccess = false;
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ReportManagement", "Exception occured on AddUpdateReports, " + ex.Message);
            }

            return isSuccess;
        }

        public bool DeleteReports(out string msg, ReportInfo ReportInfo, int UserID, int ID)
        {
            bool isSuccess = false;
            bool isAddOccured = false;
            bool isUpdateOccured = false;
            bool isDeleteOccured = false;
            msg = "";
            try
            {
                
                List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
                B1SP_Parameter parm = new B1SP_Parameter();
                try
                {
                    HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();

                    List<ReportInfo> previousData = new List<Models.ReportInfo>();
                    //For Log
                    if (ID > 0)
                    {
                        previousData = GetReports(ID);
                        if (previousData.Count > 0)
                        {
                            //AddApprovalSetup_Log(previousData[0], out isUpdateOccured);
                        }
                    }
                    DataTable dt_ApprovalSetup = HANADAL.AddUpdateDataByStoredProcedure("DeleteReports", TranslateReportsDeleteToParameterList(ReportInfo, ID, UserID), "ReportManagement");
                    if (dt_ApprovalSetup.Rows.Count == 0)
                        throw new Exception("Exception occured when DeleteReports,  Report Code:" + ReportInfo.ReportCode + " , Report Name" + ReportInfo.ReportName);
                    else
                    {
                        msg = "Successfully Deleted";
                        isSuccess = true;
                    }
                }
                catch (Exception ex)
                {
                    msg = "Exception occured in loop Add/Update Reports!";
                    isSuccess = false;
                    Log log = new Log();
                    log.LogFile(ex.Message);
                    log.InputOutputDocLog("ReportManagement", "Exception occured in DeleteReports, " + ex.Message);
                }

                Common cmn = new Common();
                Task.Run(() => cmn.AddMasterLog(isAddOccured, isUpdateOccured, isDeleteOccured, nameof(Enums.FormsName.ReportUploader), UserID, "ReportManagement"));
                //End MAster Log
            }
            catch (Exception ex)
            {
                msg = "Exception occured in Delete Reports!";
                isSuccess = false;
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ReportManagement", "Exception occured in DeleteReports, " + ex.Message);
            }

            return isSuccess;
        }

        private static byte[] GetBytesFromFile(string fullFilePath)
        {

            // this method is limited to 2^32 byte files (4.2 GB)
            fullFilePath = ("D:\\Asad\\Projects\\TMSDeloitte\\TMSDeloitte\\TMSDeloitte\\Rpt\\") + fullFilePath;
            FileStream fs = File.OpenRead(fullFilePath);
            byte[] bytes = null;
            try
            {
                bytes = new byte[fs.Length];
                fs.Read(bytes, 0, Convert.ToInt32(fs.Length));
                fs.Close();

            }
            catch (Exception Ex)
            {
                Log log = new Log();
                log.LogFile(Ex.Message);
                log.InputOutputDocLog("ReportManagement", "Exception occured on GetBytesFromFile, " + Ex.Message);
            }
            finally
            {
                fs.Close();
            }
            return bytes;
        }

        private List<B1SP_Parameter> TranslateReportsToParameterList(ReportInfo ReportInfo, int ID, int UserID)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {
                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = Convert.ToString(ID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "IsActive";
                parm.ParameterValue = Convert.ToString(ReportInfo.IsActive);
                parm.ParameterType = DBTypes.Bool.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "IsDeleted";
                parm.ParameterValue = Convert.ToString(ReportInfo.IsDeleted);
                parm.ParameterType = DBTypes.Bool.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CreatedBy";
                parm.ParameterValue = Convert.ToString(UserID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CreateDate";
                parm.ParameterValue = DateTime.Now.ToString("yyyy-MM-dd HH:MM:s");
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "UpdatedBy";
                parm.ParameterValue = Convert.ToString(UserID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "UpdateDate";
                parm.ParameterValue = DateTime.Now.ToString("yyyy-MM-dd HH:MM:s");
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ReportCode";
                parm.ParameterValue = Convert.ToString(ReportInfo.ReportCode);
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ReportName";
                parm.ParameterValue = Convert.ToString(ReportInfo.ReportName);
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "RptFile";
                parm.ParameterValue = Convert.ToString(ReportInfo.RptFile);
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "DocNum";
                parm.ParameterValue = Convert.ToString(ReportInfo.DocNum);
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ReportManagement", "Exception occured on TranslateReportsToParameterList, " + ex.Message);
            }

            return parmList;
        }

        private List<B1SP_Parameter> TranslateReportsDeleteToParameterList(ReportInfo ReportInfo, int ID, int UserID)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {
                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = Convert.ToString(ID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "IsActive";
                parm.ParameterValue = Convert.ToString(ReportInfo.IsActive);
                parm.ParameterType = DBTypes.Bool.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "IsDeleted";
                parm.ParameterValue = Convert.ToString(ReportInfo.IsDeleted);
                parm.ParameterType = DBTypes.Bool.ToString();
                parmList.Add(parm);

                 parm = new B1SP_Parameter();
                parm.ParameterName = "UpdatedBy";
                parm.ParameterValue = Convert.ToString(UserID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "UpdateDate";
                parm.ParameterValue = DateTime.Now.ToString("yyyy-MM-dd HH:MM:s");
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ReportManagement", "Exception occured on TranslateReportsDeleteToParameterList, " + ex.Message);
            }

            return parmList;
        }

        public List<string> GetReportDocNum()
        {
            List<string> docNumList = new List<string>();
            try
            {
                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();

                DataTable dt = HANADAL.GetDataTableByStoredProcedure("GetReports_LastDocNum", "ReportManagement");
                if (dt.Rows.Count > 0)
                {
                    docNumList = TranslateDataTableToReportDocNumList(dt);
                    if (docNumList[0].ToString() == "")
                        docNumList.Clear();
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ReportManagement", "Exception occured on GetReportDocNum, " + ex.Message);
            }

            return docNumList;
        }

        private List<string> TranslateDataTableToReportDocNumList(DataTable dt)
        {
            List<string> docNumList = new List<string>();
            foreach (DataRow dtRow in dt.Rows)
            {
                docNumList.Add(Convert.ToString(dtRow["DocNum"]));
            }

            return docNumList.Distinct().ToList();
        }

    }
}