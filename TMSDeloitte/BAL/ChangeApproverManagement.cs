using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using TMSDeloitte.DAL;
using TMSDeloitte.Helper;
using TMSDeloitte.Models;

namespace TMSDeloitte.BAL
{
    public class ChangeApproverManagement
    {
        public string GetDocID(string DOCNUM)
        {
            int no = 1;
            string docNum = "";
            if (DOCNUM == "")
            {
                List<string> docNumList = GetWIPRecordingFormDocNum();// GetTaskMasterDocNum();
                if (docNumList.Count > 0)
                {
                    var item = docNumList[docNumList.Count - 1];
                    //no = Convert.ToInt32(item.Split('-')[1]);
                    no = Convert.ToInt32(item);
                    no = no + 1;
                    docNum = Convert.ToString(no).PadLeft(7, '0');
                }
                else
                {
                    docNum = Convert.ToString(no).PadLeft(7, '0');
                }
            }
            else
            {
                docNum = DOCNUM;
            }
            return docNum;
        }

        public List<string> GetWIPRecordingFormDocNum()
        {
            List<string> docNumList = new List<string>();
            try
            {
                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();

                DataTable dt = HANADAL.GetDataTableByStoredProcedure("GetWIPRecordingFormSAPDocNum", "WIPRecordingFormManagement");
                if (dt.Rows.Count > 0)
                {
                    docNumList = TranslateDataTableToMaster_ChangeApproverDocNumList(dt);
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("WIPRecordingFormManagement", "Exception occured on GetWIPRecordingFormDocNum, " + ex.Message);
            }

            return docNumList;
        }

        private List<string> TranslateDataTableToMaster_ChangeApproverDocNumList(DataTable dt)
        {
            List<string> docNumList = new List<string>();
            foreach (DataRow dtRow in dt.Rows)
            {
                docNumList.Add(Convert.ToString(dtRow["BaseRef"]));
            }

            return docNumList.Distinct().ToList();
        }

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
                log.InputOutputDocLog("ChangeApproverManagement", "Exception occured on TranslateIDToParameterList, " + ex.Message);
            }
            return parmList;
        }

        private List<B1SP_Parameter> TranslateWIPFormIDToParameterList(int Id)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {
                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "WIPFormID";
                parm.ParameterValue = Convert.ToString(Id);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ChangeApproverManagement", "Exception occured on TranslateIDToParameterList, " + ex.Message);
            }
            return parmList;
        }
        
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public List<ChangeApproverChild> GetChangeApproverForms(int EmpID, int DesignationID, int DepartmentID)
        {
            List<ChangeApproverChild> changeApproverChild = new List<ChangeApproverChild>();

            try
            {
                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                DataTable dt_ChangeApprover = new DataTable();
                if (DesignationID == 0 && DepartmentID == 0 && EmpID > 0)
                {
                    dt_ChangeApprover = HANADAL.GetDataTableByStoredProcedure("GetPendingApproveTimeSheetByEmpID", TranslateEmpIDToParameterList(EmpID), "ChangeApproverManagement");
                }
                else if (DesignationID == 0 && EmpID == 0 && DepartmentID > 0)
                {
                    dt_ChangeApprover = HANADAL.GetDataTableByStoredProcedure("GetPendingApproveTimeSheetByDepartmentID", TranslateDepartIDToParameterList(DepartmentID), "ChangeApproverManagement");
                }
                else if (EmpID == 0 && DepartmentID == 0 && DesignationID > 0)
                {
                    dt_ChangeApprover = HANADAL.GetDataTableByStoredProcedure("GetPendingApproveTimeSheetByDesignationID", TranslateDesignIDToParameterList(DesignationID), "ChangeApproverManagement");
                }
                else
                {
                    dt_ChangeApprover = HANADAL.GetDataTableByStoredProcedure("GetPendingApproveTimeSheetByIDS", TranslateIDSToParameterList(EmpID, DesignationID, DepartmentID), "ChangeApproverManagement");
                }
                if (dt_ChangeApprover.Rows.Count > 0)
                {
                    changeApproverChild = TranslateDataTableToGetChangeApproverTimesheetList(dt_ChangeApprover);
                }
            }

            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ChangeApproverManagement", "Exception occured on GetChangeApproverForms on EmpID: " + EmpID + " , " + ex.Message);
            }

            return changeApproverChild;
        }

        private List<B1SP_Parameter> TranslateIDSToParameterList(int EmpID, int DesignationID, int DepartmentID)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {
                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "EmpID";
                parm.ParameterValue = Convert.ToString(EmpID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm.ParameterName = "DepartID";
                parm.ParameterValue = Convert.ToString(DepartmentID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm.ParameterName = "DesignID";
                parm.ParameterValue = Convert.ToString(DesignationID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ChangeApproverManagement", "Exception occured on TranslateIDSToParameterList, " + ex.Message);
            }
            return parmList;
        }

        private List<B1SP_Parameter> TranslateEmpIDToParameterList(int EmpID)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {
                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "EmpID";
                parm.ParameterValue = Convert.ToString(EmpID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ChangeApproverManagement", "Exception occured on TranslateEmpIDToParameterList, " + ex.Message);
            }
            return parmList;
        }

        private List<B1SP_Parameter> TranslateDepartIDToParameterList(int DepartID)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {
                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "DepartID";
                parm.ParameterValue = Convert.ToString(DepartID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ChangeApproverManagement", "Exception occured on TranslateDepartIDToParameterList, " + ex.Message);
            }
            return parmList;
        }

        private List<B1SP_Parameter> TranslateDesignIDToParameterList(int DesignID)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {
                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "DesignID";
                parm.ParameterValue = Convert.ToString(DesignID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ChangeApproverManagement", "Exception occured on TranslateDesignIDToParameterList, " + ex.Message);
            }
            return parmList;
        }

        private List<ChangeApproverChild> TranslateDataTableToGetChangeApproverTimesheetList(DataTable dt)
        {
            BAL.UserManagement usr = new BAL.UserManagement();
            List<ChangeApproverChild> changeApproverChild = new List<ChangeApproverChild>();

            try
            {
                int sno = 1;
                foreach (DataRow dtRow in dt.Rows)
                {
                    ChangeApproverChild changeApprover = new ChangeApproverChild();
                    changeApprover.SNO = sno;
                    changeApprover.KEY = Guid.NewGuid().ToString();
                    changeApprover.DocumentID = Convert.ToInt32(dtRow["DocumentID"]);
                    changeApprover.ApprovalID = Convert.ToInt32(dtRow["ApprovalID"]);
                    changeApprover.ApprovalChildID = Convert.ToInt32(dtRow["ApprovalChildID"]);
                    changeApprover.EmpID = Convert.ToString(dtRow["EmpID"]);
                    changeApprover.EmpCode = Convert.ToString(dtRow["EmpCode"]);
                    if (!string.IsNullOrEmpty(changeApprover.EmpCode))
                    {
                        HCM_Employee emp = usr.GetHCMOneEmployeeByCode(changeApprover.EmpCode);
                        if (emp != null)
                        {
                            changeApprover.FullName = emp.EmpName;
                            changeApprover.DepartmentID = emp.DepartmentID;
                            changeApprover.DepartmentName = Convert.ToString(emp.DepartmentName);
                            changeApprover.DesignationID = emp.DesignationID;
                            changeApprover.DesignationName = Convert.ToString(emp.DesignationName);
                        }
                    }
                    //changeApprover.DepartmentID = Convert.ToInt32(dtRow["DEPARTMENTID"]);
                    //changeApprover.DepartmentName = Convert.ToString(dtRow["DEPARTMENTNAME"]);
                    //changeApprover.DesignationID = Convert.ToInt32(dtRow["DESIGNATIONID"]);
                    //changeApprover.DesignationName = Convert.ToString(dtRow["DESIGNATIONNAME"]);
                    changeApprover.Year = Convert.ToString(dtRow["Year"]);
                    changeApprover.Month = Convert.ToString(dtRow["Month"]);
                    changeApprover.YearsWeeks = Convert.ToString(dtRow["YearsWeeks"]);
                    changeApprover.Weeks = Convert.ToString(dtRow["Weeks"]);
                    changeApprover.PendingAt = Convert.ToString(dtRow["PendingAt"]);
                    changeApprover.Status = false;
                    sno = sno + 1;
                    changeApproverChild.Add(changeApprover);
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ChangeApproverManagement", "Exception occured on TranslateDataTableToGetChangeApproverTimesheetList, " + ex.Message);
            }

            return changeApproverChild;
        }

        private List<ChangeApproverChild> TranslateDataTableToGetApprovalChangeApproverList(DataTable dt)
        {
            BAL.UserManagement usr = new BAL.UserManagement();
            List<ChangeApproverChild> changeApproverChild = new List<ChangeApproverChild>();

            try
            {
                int sno = 1;
                foreach (DataRow dtRow in dt.Rows)
                {
                    ChangeApproverChild changeApprover = new ChangeApproverChild();
                    changeApprover.SNO = sno;
                    changeApprover.KEY = Guid.NewGuid().ToString();
                    changeApprover.DocumentID = Convert.ToInt32(dtRow["DocumentID"]);
                    changeApprover.ApprovalID = Convert.ToInt32(dtRow["ApprovalID"]);
                    changeApprover.ApprovalChildID = Convert.ToInt32(dtRow["ApprovalChildID"]);
                    changeApprover.EmpID = Convert.ToString(dtRow["EmpID"]);
                    changeApprover.EmpCode = Convert.ToString(dtRow["EmpCode"]);
                    if (!string.IsNullOrEmpty(changeApprover.EmpCode))
                    {
                        HCM_Employee emp = usr.GetHCMOneEmployeeByCode(changeApprover.EmpCode);
                        if (emp != null)
                        {
                            changeApprover.FullName = emp.EmpName;
                        }
                    }
                    changeApprover.DepartmentID = Convert.ToInt32(dtRow["DEPARTMENTID"]);
                    changeApprover.DepartmentName = Convert.ToString(dtRow["DEPARTMENTNAME"]);
                    changeApprover.DesignationID = Convert.ToInt32(dtRow["DESIGNATIONID"]);
                    changeApprover.DesignationName = Convert.ToString(dtRow["DESIGNATIONNAME"]);
                    changeApprover.Year = Convert.ToString(dtRow["Year"]);
                    changeApprover.Month = Convert.ToString(dtRow["Month"]);
                    changeApprover.YearsWeeks = Convert.ToString(dtRow["YearsWeeks"]);
                    changeApprover.Weeks = Convert.ToString(dtRow["Weeks"]);
                    changeApprover.PendingAt = Convert.ToString(dtRow["PendingAt"]);
                    changeApprover.Status = false;
                    sno = sno + 1;
                    changeApproverChild.Add(changeApprover);
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ChangeApproverManagement", "Exception occured on TranslateDataTableToGetChangeApproverTimesheetList, " + ex.Message);
            }

            return changeApproverChild;
        }


        string ApproverRequest = "";
        private List<B1SP_Parameter> TranslateApprovalSetupChildToParameterList(ChangeApproverChild changeApproverChild)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {

                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = Convert.ToString(changeApproverChild.ApprovalChildID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);


                parm = new B1SP_Parameter();
                parm.ParameterName = "APPROVALSETUP_ID";
                parm.ParameterValue = Convert.ToString(changeApproverChild.ApprovalID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);


                parm = new B1SP_Parameter();
                parm.ParameterName = "UserID";
                parm.ParameterValue = Convert.ToString(changeApproverChild.EmpID);
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);
                
                //parm = new B1SP_Parameter();
                //parm.ParameterName = "CREATEDBY";
                //parm.ParameterValue = Convert.ToString(changeApproverChild.CREATEDBY);
                //parmList.Add(parm);

                //parm = new B1SP_Parameter();
                //parm.ParameterName = "CREATEDATE";
                //parm.ParameterValue = DateTime.Now.ToString("yyyy-MM-dd HH:MM:s");
                //parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "UpdatedBy";
                parm.ParameterValue = Convert.ToString(changeApproverChild.UpdatedBy);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "UpdateDate";
                parm.ParameterValue = DateTime.Now.ToString("yyyy-MM-dd HH:MM:s");
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "USER_CODE";
                parm.ParameterValue = Convert.ToString(changeApproverChild.EmpID);
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "USER_NAME";
                parm.ParameterValue = Convert.ToString(changeApproverChild.FullName);
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "DesignationID";
                parm.ParameterValue = Convert.ToString(changeApproverChild.DesignationID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "DesignationName";
                parm.ParameterValue = Convert.ToString(changeApproverChild.DesignationName);
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "DepartmentID";
                parm.ParameterValue = Convert.ToString(changeApproverChild.DepartmentID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "DepartmentName";
                parm.ParameterValue = Convert.ToString(changeApproverChild.DepartmentName);
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CHANGETOEMPID";
                parm.ParameterValue = Convert.ToString(changeApproverChild.CHANGETOEMPID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CHANGETOEMPCODE";
                parm.ParameterValue = Convert.ToString(changeApproverChild.CHANGETOEMPCODE);
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                ApproverRequest = ApproverRequest + Convert.ToString(changeApproverChild.FullName);

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message); 
                log.InputOutputDocLog("ChangeApproverManagement", "Exception occured on TranslateApprovalSetupChildToParameterList, " + ex.Message);
            }


            return parmList;
        }

        private List<B1SP_Parameter> TranslateChangeApproverToParameterList(ChangeApproverChild changeApproverChild)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {

                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = Convert.ToString(0);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "DocumentID";
                parm.ParameterValue = Convert.ToString(changeApproverChild.DocumentID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ApprovalID";
                parm.ParameterValue = Convert.ToString(changeApproverChild.ApprovalID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ApprovalChildID";
                parm.ParameterValue = Convert.ToString(changeApproverChild.ApprovalChildID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "EmpID";
                parm.ParameterValue = Convert.ToString(changeApproverChild.EmpID);
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "EmpCode";
                parm.ParameterValue = Convert.ToString(changeApproverChild.EmpCode);
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CHANGETOEMPID";
                parm.ParameterValue = Convert.ToString(changeApproverChild.CHANGETOEMPID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CHANGETOEMPCODE";
                parm.ParameterValue = Convert.ToString(changeApproverChild.CHANGETOEMPCODE);
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ApprovalStatus";
                parm.ParameterValue = Convert.ToString(changeApproverChild.ApprovalStatus);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CreatedBy";
                parm.ParameterValue = Convert.ToString(changeApproverChild.CreatedBy);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CreateDate";
                parm.ParameterValue = DateTime.Now.ToString("yyyy-MM-dd HH:MM:s");
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "UpdatedBy";
                parm.ParameterValue = Convert.ToString(changeApproverChild.UpdatedBy);
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
                log.InputOutputDocLog("ChangeApproverManagement", "Exception occured on TranslateChangeApproverToParameterList, " + ex.Message);
            }


            return parmList;
        }

        private List<B1SP_Parameter> TranslateTimeSheetToParameterList(ChangeApproverChild changeApproverChild)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {

                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = Convert.ToString(changeApproverChild.DocumentID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);
                
                parm = new B1SP_Parameter();
                parm.ParameterName = "UpdatedBy";
                parm.ParameterValue = Convert.ToString(changeApproverChild.UpdatedBy);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "UpdateDate";
                parm.ParameterValue = DateTime.Now.ToString("yyyy-MM-dd HH:MM:s");
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);
             
                parm = new B1SP_Parameter();
                parm.ParameterName = "CHANGETOEMPID";
                parm.ParameterValue = Convert.ToString(changeApproverChild.CHANGETOEMPID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ChangeApproverManagement", "Exception occured on TranslateTimeSheetToParameterList, " + ex.Message);
            }


            return parmList;
        }

        public bool AddUpdateApprovalSetup(out string msg, List<ChangeApproverChild> ApprovalSetupInfo, int UserID, string fullName)
        {
            bool isSuccess = false;
            bool isAddOccured = false;
            bool isUpdateOccured = false;
            bool isDeleteOccured = false;
            Common cmn = new Common();

            msg = "";
            try
            {
                NotificationManagement notification = new NotificationManagement();
                Email email = new Email();
              
                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();                   
                UserManagement usr = new UserManagement();
                    
                foreach (var list in ApprovalSetupInfo)
                {
                    try
                    {
                        //list.APPROVALSETUP_ID = ID;
                        HANADAL = new HANA_DAL_ODBC();
                        
                        DataTable dt_ApprovalCheck = new DataTable();
                        dt_ApprovalCheck = HANADAL.GetDataTableByStoredProcedure("GetApproval_ChangeApprover", TranslateIDToParameterList(UserID), "ChangeApproverManagement");

                        if(dt_ApprovalCheck.Rows.Count > 0)
                        {
                            list.ApprovalStatus = 2;
                            DataTable dt_changeApprover = HANADAL.AddUpdateDataByStoredProcedure("AddChange_Approver", TranslateChangeApproverToParameterList(list), "ChangeApproverManagement");
                            if (dt_changeApprover.Rows.Count == 0)
                                throw new Exception("Exception occured when Add Change_Approver, ID:" + list.ID + ", USER CODE" + list.EmpID);
                            else
                            {
                                if (list.ApprovalStatus == 2)
                                {
                                    Encrypt_Decrypt security = new Encrypt_Decrypt();

                                    //For Notification and Email when time sheet submit
                                    System.Web.Routing.RequestContext requestContext = HttpContext.Current.Request.RequestContext;
                                    string lnkHref = new System.Web.Mvc.UrlHelper(requestContext).Action("GetApprovalDecision", "Home", new { empID = "EncryptedID", docID = security.EncryptURLString(dt_changeApprover.Rows[0]["ID"].ToString()), docType = security.EncryptURLString("ChangeApprover") }, HttpContext.Current.Request.Url.Scheme);
                                    Task.Run(() => cmn.SndNotificationAndEmail(Convert.ToInt32(list.CreatedBy), list.DocumentID, 0, lnkHref, "ChangeApproverManagement"));
                                }
                                msg = "Approver change request has submitted";
                                isSuccess = true;
                            }
                        }

                        else
                        {
                            list.ApprovalStatus = 4;
                            DataTable dt_changeApprover = HANADAL.AddUpdateDataByStoredProcedure("AddChange_Approver", TranslateChangeApproverToParameterList(list), "ChangeApproverManagement");
                            if (dt_changeApprover.Rows.Count == 0)
                                throw new Exception("Exception occured when Add Change_Approver, ID:" + list.ID + ", USER CODE" + list.EmpID);
                            else
                            {
                                DataTable dt_approvalSetup = HANADAL.AddUpdateDataByStoredProcedure("UpdateApproval_Setup_Child", TranslateApprovalSetupChildToParameterList(list), "ChangeApproverManagement");
                                if (dt_approvalSetup.Rows.Count == 0)
                                    throw new Exception("Exception occured when Update Approval_Setup_Child, ID:" + list.ID + ", USER CODE" + list.EmpID);

                                else
                                {
                                    DataTable dt_timeSheet = HANADAL.AddUpdateDataByStoredProcedure("UpdateTimeSheetViaChangeApprover", TranslateTimeSheetToParameterList(list), "ChangeApproverManagement");
                                    if (dt_timeSheet.Rows.Count == 0)
                                        throw new Exception("Exception occured when Update Timesheet, ID:" + list.DocumentID + ", USER CODE" + list.EmpID);

                                    //int empCode = Convert.ToInt32(list.USER_CODE);
                                    int empCode = Convert.ToInt32(list.EmpID);

                                    //string Msg = "Approval Setup has been created for your " + DOCUMENT + " document";
                                    string Msg = "Approval Setup has been created for your timesheet document";
                                    notification.AddNotification(UserID, empCode, Msg);
                                    string usrEmail = usr.GetUserEmailByID(empCode);
                                    //sendedNotification.Add(approvarId);
                                    string prevMsg = msg;
                                    string subject = "Approval Setup created by " + fullName;
                                    string body = "<b>" + Msg + " </b>";

                                    msg = "";
                                    if (!email.SendEmail(usrEmail, body, subject, null, out msg))
                                        throw new Exception(msg);

                                    //sendedEmail.Add(approvarId);
                                    msg = prevMsg;

                                    if (!string.IsNullOrEmpty(list.CHANGETOEMPCODE))
                                    {
                                        BAL.UserManagement mgts = new BAL.UserManagement();

                                        HCM_Employee emp = mgts.GetHCMOneEmployeeByCode(list.CHANGETOEMPCODE);
                                        if (emp != null)
                                        {
                                            string Msgs = "Approval Setup has been created. You are approving Timesheet document of " + list.FullName;
                                            //string Msgs = "Approval Setup has been created. You are aprroving " + DOCUMENT + " document of " + list.FULLNAME;
                                            notification.AddNotification(UserID, Convert.ToInt32(list.CHANGETOEMPID), Msgs);
                                            //sendedNotification.Add(approvarId);
                                            string prevMsgs = msg;
                                            string subjects = "Approval Setup created by " + fullName;
                                            string bodys = "<b>" + Msgs + " </b>";

                                            msg = "";
                                            if (!email.SendEmail(emp.OfficeEmail, bodys, subjects, null, out msg))
                                                throw new Exception(msg);

                                            //sendedEmail.Add(approvarId);
                                            msg = prevMsgs;
                                        }
                                    }
                                }                       
                            }
                            msg = "Successfully Added/Updated";
                            isSuccess = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        isSuccess = false;
                        Log log = new Log();
                        log.LogFile(ex.Message);
                        log.InputOutputDocLog("ChangeApproverManagement", "Exception occured in foreach loop AddUpdateApprovalSetup, " + ex.Message);
                        continue;
                    }
                }

                
                cmn = new Common();
                Task.Run(() => cmn.AddMasterLog(isAddOccured, isUpdateOccured, isDeleteOccured, nameof(Enums.FormsName.ChangeApprover), UserID, "ChangeApproverManagement"));
            }
            catch (Exception ex)
            {
                msg = "Exception occured in foreach loop Add/Update Approval Setup!";
                isSuccess = false;
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ChangeApproverManagement", "Exception occured on AddUpdateApprovalSetup, " + ex.Message);
            }

            return isSuccess;
        }

        private List<StageSetupChildInfo> TranslateDataTableToApproverManagementList(DataTable dt)
        {
            List<StageSetupChildInfo> stageSetupL = new List<StageSetupChildInfo>();

            try
            {
                int sno = 1;
                foreach (DataRow dtRow in dt.Rows)
                {
                    StageSetupChildInfo stageSetup = new StageSetupChildInfo();
                    stageSetup.SNO = sno;
                    stageSetup.KEY = Guid.NewGuid().ToString();
                    stageSetup.ID = Convert.ToInt32(dtRow["ID"]);

                    stageSetup.USER_CODE = Convert.ToString(dtRow["USER_CODE"]);
                    stageSetup.FULLNAME = Convert.ToString(dtRow["FULLNAME"]);
                    stageSetup.UserID = Convert.ToInt32(dtRow["UserID"]);
                    stageSetup.DESIGNATIONID = Convert.ToInt32(dtRow["DESIGNATIONID"]);
                    stageSetup.DESIGNATIONNAME = Convert.ToString(dtRow["DESIGNATIONNAME"]);
                    stageSetup.DEPARTMENTID = Convert.ToInt32(dtRow["DEPARTMENTID"]);
                    stageSetup.DEPARTMENTNAME = Convert.ToString(dtRow["DEPARTMENTNAME"]);
                    string EmployeeCode = Convert.ToString(dtRow["EMPLOYEECODE"]);

                    if (!string.IsNullOrEmpty(EmployeeCode))
                    {
                        BAL.UserManagement mgts = new BAL.UserManagement();

                        HCM_Employee emp = mgts.GetHCMOneEmployeeByCode(EmployeeCode);
                        if (emp != null)
                        {
                            stageSetup.UserID = Convert.ToInt32(stageSetup.USER_CODE);
                            stageSetup.EMAIL = emp.OfficeEmail;
                            stageSetup.FULLNAME = emp.EmpName;
                        }
                    }


                    stageSetup.CREATEDBY = Convert.ToInt32(dtRow["CREATEDBY"]);
                    stageSetup.CREATEDATE = Convert.ToDateTime(dtRow["CREATEDATE"]);

                    if (dtRow["UPDATEDBY"] != DBNull.Value)
                        stageSetup.UPDATEDBY = Convert.ToInt32(dtRow["UPDATEDBY"]);
                    if (dtRow["UPDATEDATE"] != DBNull.Value)
                        stageSetup.UPDATEDATE = Convert.ToDateTime(dtRow["UPDATEDATE"]);

                    stageSetup.ISDELETED = Convert.ToBoolean(dtRow["ISDELETED"]);
                    sno = sno + 1;
                    stageSetupL.Add(stageSetup);
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ChangeApproverManagement", "Exception occured on TranslateDataTableToApproverManagementList, " + ex.Message);
            }

            return stageSetupL;
        }

    }
}