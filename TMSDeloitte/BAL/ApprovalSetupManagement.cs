using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using TMSDeloitte.DAL;
using TMSDeloitte.Helper;
using TMSDeloitte.Models;

namespace TMSDeloitte.BAL
{
    public class ApprovalSetupManagement
    {

        private List<B1SP_Parameter> TranslateIDToParameterList(int Id)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {
                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = Convert.ToString(Id);
                parmList.Add(parm);
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ApprovalSetupManagement", "Exception occured on TranslateApprovalSetupToParameterList, " + ex.Message);
            }
            return parmList;
        }

        private List<B1SP_Parameter> TranslateApprovalSetupIDToParameterList(int APPROVALSETUP_ID)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {

                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "APPROVALSETUP_ID";
                parm.ParameterValue = Convert.ToString(APPROVALSETUP_ID);
                parmList.Add(parm);


            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ApprovalSetupManagement", "Exception occured on TranslateApprovalSetupToParameterList, " + ex.Message);
            }


            return parmList;
        }

        private List<B1SP_Parameter> TranslateApprovalSetupStageCodeToParameterList(string StageCode)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {

                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "StageCode";
                parm.ParameterValue = Convert.ToString(StageCode);
                parmList.Add(parm);


            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ApprovalSetupManagement", "Exception occured on TranslateApprovalSetupToParameterList, " + ex.Message);
            }


            return parmList;
        }

        public List<ApprovalSetupInfo> GetApprovalSetup(int ID)
        {
            List<ApprovalSetupInfo> approvalSetup = new List<ApprovalSetupInfo>();
            List<ApprovalSetupChildInfo> approvalSetupChild = new List<ApprovalSetupChildInfo>();
            try
            {
                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                DataTable dt_ApprovalSetup = HANADAL.GetDataTableByStoredProcedure("GetApproval_Setup", TranslateIDToParameterList(ID), "ApprovalSetupManagement");

                if (dt_ApprovalSetup.Rows.Count > 0)
                {
                    approvalSetup = TranslateDataTableToApprovalSetupManagementList(dt_ApprovalSetup);
                    foreach (ApprovalSetupInfo item in approvalSetup)
                    {
                        int Approval_Setup_ID = Convert.ToInt32(item.ID);
                        DataTable dt_ApprovalSetupChild = HANADAL.GetDataTableByStoredProcedure("GetApproval_Setup_Child", TranslateApprovalSetupIDToParameterList(Approval_Setup_ID), "ApprovalSetupManagement");

                        if (dt_ApprovalSetupChild.Rows.Count > 0)
                        {
                            approvalSetupChild = TranslateDataTableToApprovalSetupChildManagementList(dt_ApprovalSetupChild);
                        }
                        item.Table = approvalSetupChild;
                    }
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ApprovalSetupManagement", "Exception occured on GetMaster_Task ID: " + ID + " , " + ex.Message);
            }

            return approvalSetup;
        }

        public List<UserProfile> GetAllUsers(bool IncludeIsSuper = true, string APPROVALSTAGE = "", string DOCUMENT = "")
        {
            List<UserProfile> userProfiles = new List<UserProfile>();
            try
            {
                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                //DataTable dt_UserProfile = HANADAL.GetDataTableByStoredProcedure("GetAllUserWithoutApprover", TranslateApprovalStagesCodeToParameterList(APPROVALSTAGE), "ApprovalSetupManagement");
                string query = " SELECT DISTINCT U.* FROM \"USERPROFILE\" U WHERE U.\"ISDELETED\"=False AND U.\"ID\" NOT IN (SELECT DISTINCT ssc.\"USER_CODE\" from \"Stage_Setup_Child\" ssc " +
                               " INNER JOIN \"Stage_Setup\" ss on ssc.\"STAGESETUP_ID\" = ss.\"ID\" WHERE ss.\"STAGECODE\" = '" + APPROVALSTAGE + "'  and ssc.\"ISDELETED\" = false) "+
                               " AND U.\"ID\" NOT IN ( SELECT DISTINCT ssc.\"USER_CODE\" from \"Approval_Setup_Child\" ssc " +
                               " INNER JOIN \"Approval_Setup\" ss on ssc.\"APPROVALSETUP_ID\" = ss.\"ID\" "+
                               " WHERE ss.\"DOCUMENT\" = '" + DOCUMENT + "' and ssc.\"ISDELETED\" = false) ";

                DataTable dt_UserProfile = HANADAL.GetDataTableByQuery(query, "ApprovalSetupManagement");

                if (dt_UserProfile.Rows.Count > 0)
                {
                    userProfiles = TranslateDataTableToUserProfileList(dt_UserProfile);
                    if (IncludeIsSuper == false)
                        userProfiles = userProfiles.Where(x => x.ISSUPER == false).ToList();
                }

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ApprovalSetupManagement", "Exception occured on GetAllUserWithoutApprover, " + ex.Message);
            }
            return userProfiles;
        }

        private List<B1SP_Parameter> TranslateApprovalStagesCodeToParameterList(string APPROVALSTAGE)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {

                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "STAGECODE";
                parm.ParameterValue = Convert.ToString(APPROVALSTAGE);
                parmList.Add(parm);


            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ApprovalSetupManagement", "Exception occured on TranslateApprovalSetupToParameterList, " + ex.Message);
            }


            return parmList;
        }

        private List<UserProfile> TranslateDataTableToUserProfileList(DataTable dt)
        {
            List<UserProfile> list = new List<UserProfile>();

            try
            {
                HCMOneManagement mgt = new HCMOneManagement();

                foreach (DataRow dtRow in dt.Rows)
                {
                    UserProfile userProfile = new UserProfile();
                    userProfile.CREATEDBY = Convert.ToInt32(dtRow["CREATEDBY"]);
                    userProfile.CREATEDDATE = Convert.ToDateTime(dtRow["CREATEDDATE"]);
                    userProfile.UPDATEDEDBY = Convert.ToInt32(dtRow["UPDATEDEDBY"]);
                    userProfile.UPDATEDDATE = Convert.ToDateTime(dtRow["UPDATEDDATE"]);
                    userProfile.EMPLOYEECODE = Convert.ToString(dtRow["EMPLOYEECODE"]);
                    userProfile.ID = Convert.ToInt32(dtRow["ID"]);
                    userProfile.ISACTIVE = Convert.ToBoolean(dtRow["ISACTIVE"]);
                    userProfile.ISDELETED = Convert.ToBoolean(dtRow["ISDELETED"]);
                    userProfile.ISSUPER = Convert.ToBoolean(dtRow["ISSUPER"]);
                    userProfile.PASSWORD = Convert.ToString(dtRow["PASSWORD"]);
                    userProfile.USERNAME = Convert.ToString(dtRow["USERNAME"]);
                    string EmployeeCode = userProfile.EMPLOYEECODE;

                    if (!string.IsNullOrEmpty(EmployeeCode))
                    {
                        BAL.UserManagement mgts = new BAL.UserManagement();

                        HCM_Employee emp = mgts.GetHCMOneEmployeeByCode(EmployeeCode);
                        if (emp != null)
                        {
                            userProfile.DEPARTMENTID = emp.DepartmentID;
                            userProfile.DEPARTMENTNAME = emp.DepartmentName;
                            userProfile.DESIGNATIONID = emp.DesignationID;
                            userProfile.DESIGNATIONNAME = emp.DesignationName;
                            userProfile.LOCATIONID = emp.LocationID;
                            userProfile.LOCATIONNAME = emp.LocationName;
                            userProfile.BRANCHID = emp.BranchID;
                            userProfile.BRANCHNAME = emp.BranchName;
                            userProfile.EMAIL = emp.OfficeEmail;
                            userProfile.FULLNAME = emp.EmpName;
                            userProfile.DETAILNAME = emp.EmpDetailName;
                        }
                    }

                    list.Add(userProfile);
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ApprovalSetupManagement", "Exception occured on TranslateDataTableToUserProfile, " + ex.Message);
            }

            return list;
        }

        public bool AddUpdateApprovalSetup(out string msg, List<ApprovalSetupChildInfo> ApprovalSetupInfo, string DOCNUM, string APPROVALCODE, string APPROVALDESCRIPTION,
            string APPROVALSTAGE, string DOCUMENT, int UserID, string fullName, int ID, bool ISACTIVE = true)
        {
            bool isSuccess = false;
            bool isAddOccured = false;
            bool isUpdateOccured = false;
            bool isDeleteOccured = false;
            msg = "";
            try
            {
                NotificationManagement notification = new NotificationManagement();
                Email email = new Email();


                int no = 1;
                string docNum = "";
                if (DOCNUM == "")
                {
                    List<string> docNumList = GetTaskMasterDocNum();
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
                    docNum = DOCNUM;
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
                        parmList.Add(parm);
                        DataSet ds = HANADAL.GetDataSetByStoredProcedure("GetApproval_Setup", parmList, "ApprovalSetupManagement");
                        if (ds.Tables.Count > 0)
                        {
                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                foreach (DataRow dtRow in ds.Tables[0].Rows)
                                {
                                    approvalCode = Convert.ToString(dtRow["APPROVALCODE"]);
                                    if (approvalCode.ToLower() == APPROVALCODE.ToLower())
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
                        parm.ParameterName = "APPROVALCODE";
                        parm.ParameterValue = Convert.ToString(APPROVALCODE);
                        parmList.Add(parm);

                        DataTable dt = HANADAL.GetDataTableByStoredProcedure("ValidateApprovalCode", parmList, "ApprovalSetupManagement");
                        if (dt.Rows.Count > 0)
                        {
                            msg = "Approval Code Already Exist!";
                            return false;
                        }
                    }
                    UserManagement usr = new UserManagement();
                    ApprovalSetupInfo approvalSetup = new ApprovalSetupInfo();
                    approvalSetup.DOCNUM = docNum;
                    approvalSetup.APPROVALCODE = APPROVALCODE;
                    approvalSetup.APPROVALDESCRIPTION = APPROVALDESCRIPTION;
                    approvalSetup.APPROVALSTAGE = APPROVALSTAGE;
                    approvalSetup.DOCUMENT = DOCUMENT;
                    approvalSetup.ISACTIVE = ISACTIVE;
                    approvalSetup.ISDELETED = false;
                    approvalSetup.ID = ID;
                    approvalSetup.CREATEDBY = UserID;
                    approvalSetup.Table = ApprovalSetupInfo;
                    approvalSetup.Table = approvalSetup.Table.Where(x => x.ID > 0 || x.ISDELETED == false).ToList();

                    //AddApprovalSetup_Log(approvalSetup, out isUpdateOccured);
                    List<ApprovalSetupInfo> previousData = new List<Models.ApprovalSetupInfo>();
                    //For Log
                    if (ID > 0)
                    {
                        previousData = GetApprovalSetup(ID);
                        if (previousData.Count > 0)
                        {
                            AddApprovalSetup_Log(previousData[0], out isUpdateOccured);
                            if (previousData[0].Table.Count > 0)
                            {
                                AddApprovalSetupChild_Log(ApprovalSetupInfo, previousData[0].Table, ID);
                            }
                        }

                    }

                    DataTable dt_ApprovalSetup = HANADAL.AddUpdateDataByStoredProcedure("AddUpdateApproval_Setup", TranslateApprovalSetupToParameterList(docNum, APPROVALCODE, APPROVALDESCRIPTION, APPROVALSTAGE, DOCUMENT, ISACTIVE, false, ID, UserID), "ApprovalSetupManagement");
                    if (dt_ApprovalSetup.Rows.Count == 0)
                        throw new Exception("Exception occured when AddUpdateApprovalSetup,  APPROVAL CODE:" + APPROVALCODE + " , APPROVAL DESCRIPTION" + APPROVALDESCRIPTION);
                    else
                    {
                        ID = Convert.ToInt32(dt_ApprovalSetup.Rows[0]["ID"]);
                        if (ID > 0)
                        {

                            approvalSetup.Table.Select(c => { c.APPROVALSETUP_ID = ID; return c; }).ToList();
                            //AddApprovalSetupChild_Log(approvalSetup.Table, out isUpdateOccured);

                            foreach (var list in ApprovalSetupInfo)
                            {

                                try
                                {
                                    list.APPROVALSETUP_ID = ID;
                                    HANADAL = new HANA_DAL_ODBC();
                                    DataTable dt_approvalSetup = HANADAL.AddUpdateDataByStoredProcedure("AddUpdateApproval_Setup_Child", TranslateApprovalSetupChildToParameterList(list), "ApprovalSetupManagement");
                                    if (dt_approvalSetup.Rows.Count == 0)
                                        throw new Exception("Exception occured when Add/Update Approval_Setup_Child, ID:" + list.ID + ", USER CODE" + list.USER_CODE);

                                    else
                                    {
                                        int empCode = Convert.ToInt32(list.USER_CODE);
                                        
  
                                        string Msg = "Approval Setup has been created for your " + DOCUMENT + " document";
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


                                        List<StageSetupChildInfo> stageApprover = new List<StageSetupChildInfo>();
                                        //DataTable dt_Approver = HANADAL.GetDataTableByStoredProcedure("GetApproverByStageCode", TranslateApprovalSetupStageCodeToParameterList(APPROVALSTAGE), "ApprovalSetupManagement");
                                        string query = " SELECT DISTINCT  R.*, U.\"EMPLOYEECODE\" FROM \"Stage_Setup\" H INNER JOIN \"Stage_Setup_Child\" R on H.\"ID\" = R.\"STAGESETUP_ID\" " +
                                    " INNER JOIN \"USERPROFILE\" U on R.\"USER_CODE\" = U.\"ID\" WHERE H.\"STAGECODE\" = '"+ APPROVALSTAGE + "' AND H.\"ISDELETED\" = FALSE AND R.\"ISDELETED\" = FALSE ";
                                       
                                        DataTable dt_Approver = HANADAL.GetDataTableByQuery(query, "ApprovalSetupManagement");

                                        if (dt_Approver.Rows.Count > 0)
                                        {
                                            stageApprover = TranslateDataTableToApproverManagementList(dt_Approver);
                                        }
                                        foreach (var approverList in stageApprover)
                                        {                                        
                                            string Msgs = "Approval Setup has been created. You are aprroving " + DOCUMENT + " document of "+ list.FULLNAME;
                                            notification.AddNotification(UserID, Convert.ToInt32(approverList.UserID), Msgs);
                                            //sendedNotification.Add(approvarId);
                                            string prevMsgs = msg;
                                            string subjects = "Approval Setup created by " + fullName;
                                            string bodys = "<b>" + Msgs + " </b>";

                                            msg = "";
                                            if (!email.SendEmail(approverList.EMAIL, bodys, subjects, null, out msg))
                                                throw new Exception(msg);

                                            //sendedEmail.Add(approvarId);
                                            msg = prevMsgs;
                                        }
                                       

                                    }
                                }
                                catch (Exception ex)
                                {
                                    isSuccess = false;
                                    Log log = new Log();
                                    log.LogFile(ex.Message);
                                    log.InputOutputDocLog("ApprovalSetupManagement", "Exception occured in foreach loop AddUpdateapprovalSetup, " + ex.Message);
                                    continue;
                                }
                            }

                        }
                        if (previousData.Count > 0)
                        {
                            //For Deleted Items
                            List<ApprovalSetupChildInfo> missingList = previousData[0].Table.Where(n => !ApprovalSetupInfo.Any(o => o.ID == n.ID && o.ISDELETED == n.ISDELETED)).ToList();

                            foreach (ApprovalSetupChildInfo previousObject in missingList)
                            {
                                ApprovalSetupChildInfo newObj = new ApprovalSetupChildInfo();
                                newObj = previousObject;
                                newObj.ISDELETED = true;
                                newObj.UPDATEDBY = UserID;
                                newObj.APPROVALSETUP_ID = ID;
                                isDeleteOccured = true;

                                //AddGropSetupChild_Log(newObj, previousObject, ID, true);
                                HANADAL.AddUpdateDataByStoredProcedure("AddUpdateApproval_Setup_Child", TranslateApprovalSetupChildToParameterList(newObj), "ApprovalSetupManagement");
                            }
                        }
                    }
                    msg = "Successfully Added/Updated";
                    isSuccess = true;
                }
                catch (Exception ex)
                {
                    msg = "Exception occured in foreach loop Add/Update Approval Setup!";
                    isSuccess = false;
                    Log log = new Log();
                    log.LogFile(ex.Message);
                    log.InputOutputDocLog("ApprovalSetupManagement", "Exception occured in foreach loop AddUpdateApprovalSetup, " + ex.Message);
                }

                //For Master Log
                //if (ApprovalSetupInfo.Where(x => x.ID == 0).ToList().Count > 0)
                //  isAddOccured = true;
                //if (ApprovalSetupInfo.Where(x => x.ISDELETED == true).ToList().Count > 0)
                //  isDeleteOccured = true;

                //int createdBy = 0;
                //var val = ApprovalSetupInfo.Where(x => x.CREATEDBY != null).FirstOrDefault();
                //if (val != null)
                //                    createdBy = Convert.ToInt32(val.CREATEDBY);

                Common cmn = new Common();
                //Task.Run(() => cmn.AddMasterLog(isAddOccured, isUpdateOccured, isDeleteOccured, nameof(Enums.FormsName.ApprovalSetup), createdBy, "ApprovalSetupManagement"));
                Task.Run(() => cmn.AddMasterLog(isAddOccured, isUpdateOccured, isDeleteOccured, nameof(Enums.FormsName.ApprovalSetup), UserID, "ApprovalSetupManagement"));
                //End MAster Log


            }
            catch (Exception ex)
            {
                msg = "Exception occured in foreach loop Add/Update Approval Setup!";
                isSuccess = false;
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ApprovalSetupManagement", "Exception occured on AddUpdateApprovalSetup, " + ex.Message);
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
                log.InputOutputDocLog("ApprovalSetupManagement", "Exception occured on TranslateDataTableToApproverManagementList, " + ex.Message);
            }

            return stageSetupL;
        }


        private List<B1SP_Parameter> TranslateApprovalSetupChildLogToParameterList(ApprovalSetupChildInfo approvalSetupChild)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {

                B1SP_Parameter parm = new B1SP_Parameter();

                parm = new B1SP_Parameter();
                parm.ParameterName = "APPROVALSETUP_ID";
                parm.ParameterValue = Convert.ToString(approvalSetupChild.APPROVALSETUP_ID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "DOCID";
                parm.ParameterValue = Convert.ToString(approvalSetupChild.UserID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "UserID_Previous";
                parm.ParameterValue = Convert.ToString(approvalSetupChild.UserID);
                parmList.Add(parm);

                //parm = new B1SP_Parameter();
                //parm.ParameterName = "USER_CODE_Previous";
                //parm.ParameterValue = Convert.ToString(approvalSetupChild.USER_CODE);
                //parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "FullName_Previous";
                parm.ParameterValue = Convert.ToString(approvalSetupChild.FULLNAME);
                parmList.Add(parm);

                //parm = new B1SP_Parameter();
                //parm.ParameterName = "DEPARTMENTNAME_Previous";
                //parm.ParameterValue = Convert.ToString(approvalSetupChild.DEPARTMENTNAME);
                //parmList.Add(parm);

                //parm = new B1SP_Parameter();
                //parm.ParameterName = "DESIGNATIONNAME_Previous";
                //parm.ParameterValue = Convert.ToString(approvalSetupChild.DESIGNATIONNAME);
                //parmList.Add(parm);

                //parm = new B1SP_Parameter();
                //parm.ParameterName = "ISACTIVE_Previous";
                //parm.ParameterValue = Convert.ToString(approvalSetupChild.ISACTIVE);
                //parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ISDELETED_Previous";
                parm.ParameterValue = Convert.ToString(approvalSetupChild.ISDELETED);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "UserID_New";
                parm.ParameterValue = Convert.ToString(approvalSetupChild.UserID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "FullName_New";
                parm.ParameterValue = Convert.ToString(approvalSetupChild.FULLNAME);
                parmList.Add(parm);

                //parm = new B1SP_Parameter();
                //parm.ParameterName = "DEPARTMENTNAME_New";
                //parm.ParameterValue = Convert.ToString(approvalSetupChild.DEPARTMENTNAME);
                //parmList.Add(parm);

                //parm = new B1SP_Parameter();
                //parm.ParameterName = "DESIGNATIONNAME_New";
                //parm.ParameterValue = Convert.ToString(approvalSetupChild.DESIGNATIONNAME);
                //parmList.Add(parm);

                //parm = new B1SP_Parameter();
                //parm.ParameterName = "ISACTIVE_New";
                //parm.ParameterValue = Convert.ToString(approvalSetupChild.ISACTIVE);
                //parmList.Add(parm);


                parm = new B1SP_Parameter();
                parm.ParameterName = "ISDELETED_New";
                parm.ParameterValue = Convert.ToString(approvalSetupChild.ISDELETED);
                parmList.Add(parm);


            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ApprovalSetupManagement", "Exception occured on TranslateApprovalSetupChildLogToParameterList, " + ex.Message);
            }


            return parmList;
        }

        public void AddApprovalSetupChild_Log(List<ApprovalSetupChildInfo> newObjectList, List<ApprovalSetupChildInfo> previousObjectList, int docID)
        {
            try
            {
                ApprovalSetupChildInfo newObject = new ApprovalSetupChildInfo();
                ApprovalSetupChildInfo previousObject = new ApprovalSetupChildInfo();

                foreach (var obj in newObjectList)
                {
                    previousObject = previousObjectList.Where(x => x.ID == obj.ID).FirstOrDefault();
                    if (previousObject != null)
                    {
                        newObject = obj;

                        //ApprovalSetupInfo previousObject = GetApprovalSetupByID(Convert.ToInt32(approvalSetupList[0].APPROVALSETUP_ID));
                        List<B1SP_Parameter> paramList = TranslateApprovalSetupChildLogToParameterList(newObject);
                        //List<B1SP_Parameter> paramList = new List<B1SP_Parameter>();
                        //B1SP_Parameter parm = new B1SP_Parameter();
                        bool isChangeOccured = false;

                        //var val = previousObject.Table.Where(x => x.ID == newObject.ID).FirstOrDefault();
                        //if (val != null)
                        //{
                        if (previousObject != null)
                        {
                            foreach (PropertyCompareResult resultItem in PropertyCompare.Compare(previousObject, newObject))
                            {
                                isChangeOccured = true;
                                string Name = resultItem.Name;
                                object PreviousValue = resultItem.OldValue;
                                object NewValue = resultItem.NewValue;

                                switch (Name)
                                {
                                    case "USER_CODE":
                                        paramList.Where(x => x.ParameterName == "UserID_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                        paramList.Where(x => x.ParameterName == "UserID_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();

                                        break;
                                    case "FULLNAME":
                                        paramList.Where(x => x.ParameterName == "USER_NAME_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                        paramList.Where(x => x.ParameterName == "USER_NAME_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();

                                        break;
                                    case "ISDELETED":
                                        paramList.Where(x => x.ParameterName == "ISDELETED_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                        paramList.Where(x => x.ParameterName == "ISDELETED_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();

                                        break;
                                }
                            }

                            if (isChangeOccured)
                            {
                                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                                HANADAL.AddUpdateDataByStoredProcedure("AddUpdateApprovalSetupChildLog", paramList, "ApprovalSetupChildManagement");
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ApprovalSetupChildManagement", "Exception occured on AddApprovalSetupChild_Log, " + ex.Message);
            }
        }


        private List<B1SP_Parameter> TranslateApprovalSetupLogToParameterList(ApprovalSetupInfo approvalSetup)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {

                B1SP_Parameter parm = new B1SP_Parameter();
                parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = Convert.ToString(approvalSetup.ID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "DOCNUM";
                parm.ParameterValue = Convert.ToString(approvalSetup.DOCNUM);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "DOCID";
                parm.ParameterValue = Convert.ToString(approvalSetup.ID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "APPROVALCODE";
                parm.ParameterValue = Convert.ToString(approvalSetup.APPROVALCODE);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "APPROVALDESCRIPTION_Previous";
                parm.ParameterValue = Convert.ToString(approvalSetup.APPROVALDESCRIPTION);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "APPROVALSTAGE_Previous";
                parm.ParameterValue = Convert.ToString(approvalSetup.APPROVALSTAGE);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "DOCUMENT_Previous";
                parm.ParameterValue = Convert.ToString(approvalSetup.DOCUMENT);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ISACTIVE_Previous";
                parm.ParameterValue = Convert.ToString(approvalSetup.ISACTIVE);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ISDELETED_Previous";
                parm.ParameterValue = Convert.ToString(approvalSetup.ISDELETED);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "APPROVALDESCRIPTION_New";
                parm.ParameterValue = Convert.ToString(approvalSetup.APPROVALDESCRIPTION);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "APPROVALSTAGE_New";
                parm.ParameterValue = Convert.ToString(approvalSetup.APPROVALSTAGE);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "DOCUMENT_New";
                parm.ParameterValue = Convert.ToString(approvalSetup.DOCUMENT);
                parmList.Add(parm);


                parm = new B1SP_Parameter();
                parm.ParameterName = "ISACTIVE_New";
                parm.ParameterValue = Convert.ToString(approvalSetup.ISACTIVE);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ISDELETED_New";
                parm.ParameterValue = Convert.ToString(approvalSetup.ISDELETED);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CREATEDBY";
                parm.ParameterValue = Convert.ToString(approvalSetup.CREATEDBY);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CREATEDATE";
                parm.ParameterValue = DateTime.Now.ToString("yyyy-MM-dd");
                parmList.Add(parm);

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ApprovalSetupManagement", "Exception occured on TranslateApprovalSetupDetailLogToParameterList, " + ex.Message);
            }


            return parmList;
        }

        public void AddApprovalSetup_Log(ApprovalSetupInfo approvalSetupList, out bool isUpdateOccured)
        {
            isUpdateOccured = false;
            try
            {
                if (approvalSetupList.ID == 0)
                    return;

                ApprovalSetupInfo previousObject = GetApprovalSetupByID(Convert.ToInt32(approvalSetupList.ID));
                List<B1SP_Parameter> paramList = TranslateApprovalSetupLogToParameterList(approvalSetupList);
                bool isChangeOccured = false;
                if (previousObject != null)
                {

                    foreach (PropertyCompareResult resultItem in PropertyCompare.Compare(previousObject, approvalSetupList))
                    {
                        isChangeOccured = true;
                        isUpdateOccured = true;
                        string Name = resultItem.Name;
                        object PreviousValue = resultItem.OldValue;
                        object NewValue = resultItem.NewValue;
                        switch (Name)
                        {
                            case "APPROVALDESCRIPTION":
                                paramList.Where(x => x.ParameterName == "APPROVALDESCRIPTION_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                paramList.Where(x => x.ParameterName == "APPROVALDESCRIPTION_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();

                                break;
                            case "APPROVALSTAGE":
                                paramList.Where(x => x.ParameterName == "APPROVALSTAGE_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                paramList.Where(x => x.ParameterName == "APPROVALSTAGE_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();

                                break;
                            case "DOCUMENT":
                                paramList.Where(x => x.ParameterName == "DOCUMENT_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                paramList.Where(x => x.ParameterName == "DOCUMENT_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();

                                break;

                            case "ISACTIVE":
                                if (PreviousValue == null)
                                    PreviousValue = true;
                                paramList.Where(x => x.ParameterName == "ISACTIVE_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                paramList.Where(x => x.ParameterName == "ISACTIVE_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();

                                break;
                            case "ISDELETED":
                                if (PreviousValue == null)
                                    PreviousValue = false;
                                paramList.Where(x => x.ParameterName == "ISDELETED_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                paramList.Where(x => x.ParameterName == "ISDELETED_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();

                                break;
                        }

                    }
                    if (isChangeOccured)
                    {
                        HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                        DataTable dt_approvalSetupLog = HANADAL.AddUpdateDataByStoredProcedure("AddUpdateApproval_SetupLog", paramList, "ApprovalSetupManagement");


                    }
                }

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ApprovalSetupManagement", "Exception occured on AddAssignmentCostSetup_Log, " + ex.Message);
            }
        }

        public List<ApprovalSetupInfo> GetTaskMasterByFunctionID(int id)
        {
            List<ApprovalSetupInfo> TaskMasterList = new List<ApprovalSetupInfo>();
            try
            {
                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                Common cmn = new Common();
                //List<HCM_Designation> designationList = cmn.GetHCMDesignationList();
                DataTable dt_ApprovalSetupSetup = HANADAL.GetDataTableByStoredProcedure("GetMaster_TaskByFunctionID", cmn.TranslateIDToParameterList(id), "ApprovalSetupManagement");
                bool isNewDoc = false;
                if (dt_ApprovalSetupSetup.Rows.Count > 0)
                {
                    TaskMasterList = TranslateDataTableToApprovalSetupManagementList(dt_ApprovalSetupSetup);
                }
                else
                    isNewDoc = true;


                foreach (var item in TaskMasterList)
                {
                    item.isNewDocument = isNewDoc;
                    item.KEY = Guid.NewGuid().ToString();

                }


                int sNo = TaskMasterList.Count() + 1;


                string docNum = "";
                var value = TaskMasterList.Where(x => x.DOCNUM != null).FirstOrDefault();
                if (value != null)
                {
                    docNum = value.DOCNUM;
                }
                else
                {


                }

                TaskMasterList.Select(c => { c.DOCNUM = docNum; return c; }).ToList();
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ApprovalSetupManagement", "Exception occured on GetTaskMasterByFunctionID ID: " + id + " , " + ex.Message);
            }

            return TaskMasterList;
        }

        public List<string> GetTaskMasterDocNum()
        {
            List<string> docNumList = new List<string>();
            try
            {
                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();

                DataTable dt = HANADAL.GetDataTableByStoredProcedure("GetApproval_Setup_LastDocNum", "ApprovalSetupManagement");
                if (dt.Rows.Count > 0)
                {
                    docNumList = TranslateDataTableToMaster_TaskDocNumList(dt);
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ApprovalSetupManagement", "Exception occured on GetTaskMasterDocNum, " + ex.Message);
            }

            return docNumList;
        }

        public List<ApprovalSetupInfo> GetTask_MasterByDocNum(string docNo)
        {
            List<ApprovalSetupInfo> Task_MasterList = new List<ApprovalSetupInfo>();
            try
            {

                List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();

                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "DOCNUM";
                parm.ParameterValue = docNo;
                parmList.Add(parm);

                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                Common cmn = new Common();
                DataTable dt_Task_Master = HANADAL.GetDataTableByStoredProcedure("GetMaster_TaskByDocNum", parmList, "ApprovalSetupManagement");
                if (dt_Task_Master.Rows.Count > 0)
                {

                    Task_MasterList = TranslateDataTableToApprovalSetupManagementList(dt_Task_Master);
                }

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ApprovalSetupManagement", "Exception occured on GetApprovalSetupByDocNum DocNum: " + docNo + " , " + ex.Message);
            }

            return Task_MasterList;
        }

        private List<string> TranslateDataTableToMaster_TaskDocNumList(DataTable dt)
        {
            List<string> docNumList = new List<string>();
            foreach (DataRow dtRow in dt.Rows)
            {
                docNumList.Add(Convert.ToString(dtRow["DOCNUM"]));
            }

            return docNumList.Distinct().ToList();
        }


        #region "Translation approvalSetup"


        private List<ApprovalSetupInfo> TranslateDataTableToApprovalSetupManagementList(DataTable dt)
        {
            List<ApprovalSetupInfo> approvalSetupL = new List<ApprovalSetupInfo>();

            try
            {
                int sno = 1;
                foreach (DataRow dtRow in dt.Rows)
                {
                    ApprovalSetupInfo approvalSetup = new ApprovalSetupInfo();
                    //approvalSetup.SNO = sno;
                    approvalSetup.KEY = Guid.NewGuid().ToString();
                    approvalSetup.ID = Convert.ToInt32(dtRow["ID"]);
                    approvalSetup.APPROVALCODE = Convert.ToString(dtRow["APPROVALCODE"]);
                    approvalSetup.APPROVALDESCRIPTION = Convert.ToString(dtRow["APPROVALDESCRIPTION"]);
                    approvalSetup.APPROVALSTAGE = Convert.ToString(dtRow["APPROVALSTAGE"]);
                    approvalSetup.DOCUMENT = Convert.ToString(dtRow["DOCUMENT"]);
                    approvalSetup.DOCNUM = Convert.ToString(dtRow["DOCNUM"]);
                    approvalSetup.CREATEDBY = Convert.ToInt32(dtRow["CREATEDBY"]);
                    approvalSetup.CREATEDATE = Convert.ToDateTime(dtRow["CREATEDATE"]);

                    if (dtRow["UPDATEDBY"] != DBNull.Value)
                        approvalSetup.UPDATEDBY = Convert.ToInt32(dtRow["UPDATEDBY"]);
                    if (dtRow["UPDATEDATE"] != DBNull.Value)
                        approvalSetup.UPDATEDATE = Convert.ToDateTime(dtRow["UPDATEDATE"]);

                    approvalSetup.ISACTIVE = Convert.ToBoolean(dtRow["ISACTIVE"]);
                    approvalSetup.ISDELETED = Convert.ToBoolean(dtRow["ISDELETED"]);
                    sno = sno + 1;
                    approvalSetupL.Add(approvalSetup);
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ApprovalSetupManagement", "Exception occured on TranslateDataTableToApprovalSetupManagementList, " + ex.Message);
            }

            return approvalSetupL;
        }

        private List<ApprovalSetupChildInfo> TranslateDataTableToApprovalSetupChildManagementList(DataTable dt)
        {
            List<ApprovalSetupChildInfo> approvalSetupL = new List<ApprovalSetupChildInfo>();

            try
            {
                int sno = 1;
                foreach (DataRow dtRow in dt.Rows)
                {
                    ApprovalSetupChildInfo approvalSetup = new ApprovalSetupChildInfo();
                    approvalSetup.SNO = sno;
                    approvalSetup.KEY = Guid.NewGuid().ToString();
                    approvalSetup.ID = Convert.ToInt32(dtRow["ID"]);

                    approvalSetup.APPROVALSETUP_ID = Convert.ToInt32(dtRow["APPROVALSETUP_ID"]);
                    approvalSetup.USER_CODE = Convert.ToString(dtRow["USER_CODE"]);
                    approvalSetup.FULLNAME = Convert.ToString(dtRow["USER_NAME"]);
                    approvalSetup.UserID = Convert.ToInt32(dtRow["UserID"]);
                    approvalSetup.DESIGNATIONID = Convert.ToInt32(dtRow["DESIGNATIONID"]);
                    approvalSetup.DESIGNATIONNAME = Convert.ToString(dtRow["DESIGNATIONNAME"]);
                    approvalSetup.DEPARTMENTID = Convert.ToInt32(dtRow["DEPARTMENTID"]);
                    approvalSetup.DEPARTMENTNAME = Convert.ToString(dtRow["DEPARTMENTNAME"]);
                    approvalSetup.CREATEDBY = Convert.ToInt32(dtRow["CREATEDBY"]);
                    approvalSetup.CREATEDATE = Convert.ToDateTime(dtRow["CREATEDATE"]);
                    //string EmployeeCode = approvalSetup.USER_CODE;
                    //if (!string.IsNullOrEmpty(EmployeeCode))
                    //{
                    //    BAL.UserManagement mgts = new BAL.UserManagement();

                    //    HCM_Employee emp = mgts.GetHCMOneEmployeeByCode(EmployeeCode);
                    //    if (emp != null)
                    //    {
                    //        approvalSetup.USER_NAME = emp.EmpName;
                    //    }
                    //}
                    if (dtRow["UPDATEDBY"] != DBNull.Value)
                        approvalSetup.UPDATEDBY = Convert.ToInt32(dtRow["UPDATEDBY"]);
                    if (dtRow["UPDATEDATE"] != DBNull.Value)
                        approvalSetup.UPDATEDATE = Convert.ToDateTime(dtRow["UPDATEDATE"]);
                    if (Convert.ToString(dtRow["CHANGETOEMPID"]) != "")
                    {
                        approvalSetup.CHANGETOEMPID = Convert.ToInt32(dtRow["CHANGETOEMPID"]);
                        approvalSetup.CHANGETOEMPCODE = Convert.ToString(dtRow["CHANGETOEMPCODE"]);
                        string EmployeeCode = approvalSetup.CHANGETOEMPCODE;
                        if (!string.IsNullOrEmpty(EmployeeCode))
                        {
                            BAL.UserManagement mgts = new BAL.UserManagement();

                            HCM_Employee emp = mgts.GetHCMOneEmployeeByCode(EmployeeCode);
                            if (emp != null)
                            {
                                approvalSetup.CHANGETONAME = emp.EmpName;
                            }
                        }
                    }
                    approvalSetup.ISDELETED = Convert.ToBoolean(dtRow["ISDELETED"]);
                    sno = sno + 1;
                    approvalSetupL.Add(approvalSetup);
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ApprovalSetupManagement", "Exception occured on TranslateDataTableToApprovalSetupManagementList, " + ex.Message);
            }

            return approvalSetupL;
        }

        
        private List<B1SP_Parameter> TranslateApprovalSetupToParameterList(string DOCNUM, string APPROVALCODE, string APPROVALDESCRIPTION, string APPROVALSTAGE, string DOCUMENT,
            bool ISACTIVE, bool ISDELETED, int ID, int UserID)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {
                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = Convert.ToString(ID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ISACTIVE";
                parm.ParameterValue = Convert.ToString(ISACTIVE);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ISDELETED";
                parm.ParameterValue = Convert.ToString(ISDELETED);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CREATEDBY";
                parm.ParameterValue = Convert.ToString(UserID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CREATEDATE";
                parm.ParameterValue = DateTime.Now.ToString("yyyy-MM-dd HH:MM:s");
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "UPDATEDBY";
                parm.ParameterValue = Convert.ToString(UserID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "UPDATEDATE";
                parm.ParameterValue = DateTime.Now.ToString("yyyy-MM-dd HH:MM:s");
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "APPROVALCODE";
                parm.ParameterValue = Convert.ToString(APPROVALCODE);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "APPROVALDESCRIPTION";
                parm.ParameterValue = Convert.ToString(APPROVALDESCRIPTION);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "APPROVALSTAGE";
                parm.ParameterValue = Convert.ToString(APPROVALSTAGE);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "DOCUMENT";
                parm.ParameterValue = Convert.ToString(DOCUMENT);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "DOCNUM";
                parm.ParameterValue = Convert.ToString(DOCNUM);
                parmList.Add(parm);

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ApprovalSetupManagement", "Exception occured on TranslateApprovalSetupToParameterList, " + ex.Message);
            }

            return parmList;
        }

        string ApproverRequest = "";
        private List<B1SP_Parameter> TranslateApprovalSetupChildToParameterList(ApprovalSetupChildInfo approvalSetupChild)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {

                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = Convert.ToString(approvalSetupChild.ID);
                parmList.Add(parm);


                parm = new B1SP_Parameter();
                parm.ParameterName = "APPROVALSETUP_ID";
                parm.ParameterValue = Convert.ToString(approvalSetupChild.APPROVALSETUP_ID);
                parmList.Add(parm);


                parm = new B1SP_Parameter();
                parm.ParameterName = "UserID";
                parm.ParameterValue = Convert.ToString(approvalSetupChild.UserID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ISACTIVE";
                parm.ParameterValue = Convert.ToString(approvalSetupChild.ISACTIVE);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ISDELETED";
                parm.ParameterValue = Convert.ToString(approvalSetupChild.ISDELETED);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CREATEDBY";
                parm.ParameterValue = Convert.ToString(approvalSetupChild.CREATEDBY);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CREATEDATE";
                parm.ParameterValue = DateTime.Now.ToString("yyyy-MM-dd HH:MM:s");
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "UPDATEDBY";
                parm.ParameterValue = Convert.ToString(approvalSetupChild.UPDATEDBY);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "UPDATEDATE";
                parm.ParameterValue = DateTime.Now.ToString("yyyy-MM-dd HH:MM:s");
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "USER_CODE";
                parm.ParameterValue = Convert.ToString(approvalSetupChild.USER_CODE);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "USER_NAME";
                parm.ParameterValue = Convert.ToString(approvalSetupChild.FULLNAME);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "DESIGNATIONID";
                parm.ParameterValue = Convert.ToString(approvalSetupChild.DESIGNATIONID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "DESIGNATIONNAME";
                parm.ParameterValue = Convert.ToString(approvalSetupChild.DESIGNATIONNAME);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "DEPARTMENTID";
                parm.ParameterValue = Convert.ToString(approvalSetupChild.DEPARTMENTID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "DEPARTMENTNAME";
                parm.ParameterValue = Convert.ToString(approvalSetupChild.DEPARTMENTNAME);
                parmList.Add(parm);

                ApproverRequest = ApproverRequest + Convert.ToString(approvalSetupChild.FULLNAME);

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ApprovalSetupManagement", "Exception occured on TranslateApprovalSetupChildToParameterList, " + ex.Message);
            }


            return parmList;
        }

        private List<B1SP_Parameter> TranslateMaster_TaskLogToParameterList(ApprovalSetupInfo approvalSetup)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {

                B1SP_Parameter parm = new B1SP_Parameter();

                //parm = new B1SP_Parameter();
                //parm.ParameterName = "FROMKM_PREVIOUS";
                //parm.ParameterValue = Convert.ToString(approvalSetup.FROMKM);
                //parmList.Add(parm);

                //parm = new B1SP_Parameter();
                //parm.ParameterName = "FROMKM_NEW";
                //parm.ParameterValue = Convert.ToString(approvalSetup.FROMKM);
                //parmList.Add(parm);

                //parm = new B1SP_Parameter();
                //parm.ParameterName = "TOKM_NEW";
                //parm.ParameterValue = Convert.ToString(approvalSetup.TOKM);
                //parmList.Add(parm);

                //parm = new B1SP_Parameter();
                //parm.ParameterName = "TOKM_PREVIOUS";
                //parm.ParameterValue = Convert.ToString(approvalSetup.TOKM);
                //parmList.Add(parm);

                //parm = new B1SP_Parameter();
                //parm.ParameterName = "RATETRIP_PREVIOUS";
                //parm.ParameterValue = Convert.ToString(approvalSetup.RATETRIP);
                //parmList.Add(parm);

                //parm = new B1SP_Parameter();
                //parm.ParameterName = "RATETRIP_NEW";
                //parm.ParameterValue = Convert.ToString(approvalSetup.RATETRIP);
                //parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ISACTIVE_PREVIOUS";
                parm.ParameterValue = Convert.ToString(approvalSetup.ISACTIVE);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ISACTIVE_NEW";
                parm.ParameterValue = Convert.ToString(approvalSetup.ISACTIVE);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ISDELETED_PREVIOUS";
                parm.ParameterValue = Convert.ToString(approvalSetup.ISDELETED);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ISDELETED_NEW";
                parm.ParameterValue = Convert.ToString(approvalSetup.ISDELETED);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CREATEDBY";
                parm.ParameterValue = Convert.ToString(approvalSetup.CREATEDBY);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CREATEDATE";
                parm.ParameterValue = DateTime.Now.ToString("yyyy-MM-dd HH:MM:s");
                parmList.Add(parm);

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ApprovalSetupManagement", "Exception occured on TranslateMaster_TaskLogToParameterList, " + ex.Message);
            }


            return parmList;
        }

        #endregion


        #region
        public DataSet GetApprovalSetupLog(string id)
        {
            DataSet ds = new DataSet();
            try

            {
                List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();

                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = id;
                parmList.Add(parm);

                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                ds = HANADAL.GetDataSetByStoredProcedure("GetApprovalSetupLogByID", parmList, "ApprovalSetupManagement");
                ds.Tables["Table"].Columns.Remove(ds.Tables["Table"].Columns["CREATEDBY"]);
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ApprovalSetupManagement", "Exception occured on GetApprovalSetupLog, " + ex.Message);
            }

            return ds;
        }

        public ApprovalSetupInfo GetApprovalSetupByID(int id)
        {
            ApprovalSetupInfo approvalSetup = new ApprovalSetupInfo();
            try
            {
                List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = Convert.ToString(id);
                parmList.Add(parm);

                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                Common cmn = new Common();
                DataSet ds = HANADAL.GetDataSetByStoredProcedure("GetApprovalSetupByID", parmList, "ApprovalSetupManagement");
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        approvalSetup = TranslateDataTableToApprovalSetup(ds.Tables[0]);
                        approvalSetup.Table = TranslateDataTableToApprovalSetupChildManagementList(ds.Tables[1]);
                    }
                }


            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ApprovalSetupManagement", "Exception occured on GetApprovalSetupByID ID: " + id + " , " + ex.Message);
            }

            return approvalSetup;
        }

        private ApprovalSetupInfo TranslateDataTableToApprovalSetup(DataTable dt)
        {
            ApprovalSetupInfo approvalSetup = new ApprovalSetupInfo();

            try
            {
                foreach (DataRow dtRow in dt.Rows)
                {

                    approvalSetup.ID = Convert.ToInt32(dtRow["ID"]);
                    approvalSetup.DOCNUM = Convert.ToString(dtRow["DocNum"]);

                    approvalSetup.APPROVALCODE = Convert.ToString(dtRow["APPROVALCODE"]);
                    approvalSetup.APPROVALDESCRIPTION = Convert.ToString(dtRow["APPROVALDESCRIPTION"]);
                    approvalSetup.APPROVALSTAGE = Convert.ToString(dtRow["APPROVALSTAGE"]);
                    approvalSetup.DOCUMENT = Convert.ToString(dtRow["DOCUMENT"]);

                    //DateTime tdate = Convert.ToDateTime(dtRow["ToDate"]);
                    //approvalSetup.ToDate = Convert.ToDateTime(tdate.Date);

                    approvalSetup.CREATEDBY = Convert.ToInt32(dtRow["CREATEDBY"]);
                    approvalSetup.CREATEDATE = Convert.ToDateTime(dtRow["CREATEDDATE"]);

                    if (dtRow["UPDATEDEDBY"] != DBNull.Value)
                        approvalSetup.UPDATEDBY = Convert.ToInt32(dtRow["UPDATEDEDBY"]);
                    if (dtRow["UPDATEDDATE"] != DBNull.Value)
                        approvalSetup.UPDATEDATE = Convert.ToDateTime(dtRow["UPDATEDDATE"]);

                    approvalSetup.ISDELETED = Convert.ToBoolean(dtRow["ISDELETED"]);
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ApprovalSetupManagement", "Exception occured on TranslateDataTableToApprovalSetup, " + ex.Message);
            }

            return approvalSetup;
        }


        #endregion

    }
}