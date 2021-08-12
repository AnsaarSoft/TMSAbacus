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
    public class ApprovalDecisionManagement
    {

        public List<ApprovalDecision> GetDocView(int DocID, int EmpID, string DocType)
        {
            List<ApprovalDecision> approvalDecision = new List<ApprovalDecision>();
            try
            {
                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                if (DocType == "Timesheet")
                {
                    DataTable dt_ApprovalDecision = HANADAL.GetDataTableByStoredProcedure("GetTimeSheetViewByIDS", TranslateIDSToParameterList(DocID, EmpID), "ApprovalDecisionManagement");

                    if (dt_ApprovalDecision.Rows.Count > 0)
                    {
                        approvalDecision = TranslateDataTableToDocViewManagementList(dt_ApprovalDecision);
                    }
                }
                else if (DocType == "Travel Claim")
                {
                    DataTable dt_ApprovalDecision = HANADAL.GetDataTableByStoredProcedure("GetMonthlyTravelSheetViewByIDS", TranslateIDSToParameterList(DocID, EmpID), "ApprovalDecisionManagement");

                    if (dt_ApprovalDecision.Rows.Count > 0)
                    {
                        approvalDecision = TranslateDataTableToTravelClaimDocViewManagementList(dt_ApprovalDecision);
                    }
                }
                else if (DocType == "Claim")
                {
                    DataTable dt_ApprovalDecision = HANADAL.GetDataTableByStoredProcedure("GetClaimViewByIDS", TranslateIDSToParameterList(DocID, EmpID), "ApprovalDecisionManagement");

                    if (dt_ApprovalDecision.Rows.Count > 0)
                    {
                        approvalDecision = TranslateDataTableToClaimDocViewManagementList(dt_ApprovalDecision);
                    }
                }
                else if (DocType == "Assignment")
                {
                    DataTable dt_ApprovalDecision = HANADAL.GetDataTableByStoredProcedure("GetAssignmentViewByIDS", TranslateIDSToParameterList(DocID, EmpID), "ApprovalDecisionManagement");

                    if (dt_ApprovalDecision.Rows.Count > 0)
                    {
                        approvalDecision = TranslateDataTableToAssignmentDocViewManagementList(dt_ApprovalDecision);
                    }
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ApprovalDecisionManagement", "Exception occured on GetDocView ID: " + DocID + " , " + ex.Message);
            }

            return approvalDecision;
        }

        private List<B1SP_Parameter> TranslateIDSToParameterList(int DocID, int EmpID)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {
                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "DOCUMENT_ID";
                parm.ParameterValue = Convert.ToString(DocID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "EmpID";
                parm.ParameterValue = Convert.ToString(EmpID);
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ApprovalDecisionManagement", "Exception occured on TranslateIDSToParameterList, " + ex.Message);
            }

            return parmList;
        }

        private List<ApprovalDecision> TranslateDataTableToDocViewManagementList(DataTable dt)
        {
            List<ApprovalDecision> approvalDecisionL = new List<ApprovalDecision>();

            try
            {
                int sno = 1;
                foreach (DataRow dtRow in dt.Rows)
                {
                    ApprovalDecision approvalDecision = new ApprovalDecision();
                    approvalDecision.SNO = sno;
                    approvalDecision.KEY = Guid.NewGuid().ToString();
                    approvalDecision.ID = Convert.ToInt32(dtRow["ID"]);
                    approvalDecision.DocNum = Convert.ToString(dtRow["DocNum"]);
                    approvalDecision.EMPLOYEECODE = Convert.ToString(dtRow["EMPLOYEECODE"]);
                    approvalDecision.USER_NAME = Convert.ToString(dtRow["USERNAME"]);
                    approvalDecision.Task = Convert.ToString(dtRow["Task"]);
                    approvalDecision.Location = Convert.ToString(dtRow["Location"]);
                    approvalDecision.Year = Convert.ToInt32(dtRow["Year"]);
                    if (Convert.ToString(dtRow["WorkDate"]) != "")
                        approvalDecision.WorkDate = Convert.ToString(Convert.ToDateTime(dtRow["WorkDate"]).ToString("dd/MM/yyyy"));
                    approvalDecision.Description = Convert.ToString(dtRow["Description"]);
                    approvalDecision.WorkHours = Convert.ToString(dtRow["WorkHours"]);
                    approvalDecision.STATUS = Convert.ToString(dtRow["Status"]);
                    sno = sno + 1;
                    approvalDecisionL.Add(approvalDecision);
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("GetDocView", "Exception occured on TranslateDataTableToDocViewManagementList, " + ex.Message);
            }

            return approvalDecisionL;
        }

        private List<ApprovalDecision> TranslateDataTableToTravelClaimDocViewManagementList(DataTable dt)
        {
            List<ApprovalDecision> approvalDecisionL = new List<ApprovalDecision>();

            try
            {
                int sno = 1;
                foreach (DataRow dtRow in dt.Rows)
                {
                    ApprovalDecision approvalDecision = new ApprovalDecision();
                    approvalDecision.SNO = sno;
                    approvalDecision.KEY = Guid.NewGuid().ToString();
                    approvalDecision.ID = Convert.ToInt32(dtRow["ID"]);
                    approvalDecision.DocNum = Convert.ToString(dtRow["DocNum"]);
                    approvalDecision.EMPLOYEECODE = Convert.ToString(dtRow["EMPLOYEECODE"]);
                    approvalDecision.USER_NAME = Convert.ToString(dtRow["USERNAME"]);
                    approvalDecision.Year = Convert.ToInt32(dtRow["Year"]);
                    if (Convert.ToString(dtRow["TravelDate"]) != "")
                        approvalDecision.TravelDate = Convert.ToString(Convert.ToDateTime(dtRow["TravelDate"]).ToString("dd/MM/yyyy"));
                    approvalDecision.Assignment = Convert.ToString(dtRow["AssignmentTitle"]);
                    approvalDecision.Description = Convert.ToString(dtRow["Description"]);
                    approvalDecision.Kilometers = Convert.ToString(dtRow["Kilometers"]);
                    approvalDecision.Amount = Convert.ToString(dtRow["Amount"]);
                    approvalDecision.ParkingCharges = Convert.ToString(dtRow["ParkingCharges"]);
                    approvalDecision.TotalAmount = Convert.ToString(dtRow["TotalAmount"]);
                    sno = sno + 1;
                    approvalDecisionL.Add(approvalDecision);
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("GetDocView", "Exception occured on TranslateDataTableToTravelClaimDocViewManagementList, " + ex.Message);
            }

            return approvalDecisionL;
        }

        private List<ApprovalDecision> TranslateDataTableToClaimDocViewManagementList(DataTable dt)
        {
            List<ApprovalDecision> approvalDecisionL = new List<ApprovalDecision>();

            try
            {
                int sno = 1;
                foreach (DataRow dtRow in dt.Rows)
                {
                    ApprovalDecision approvalDecision = new ApprovalDecision();
                    approvalDecision.SNO = sno;
                    approvalDecision.KEY = Guid.NewGuid().ToString();
                    approvalDecision.ID = Convert.ToInt32(dtRow["ID"]);
                    approvalDecision.DocNum = Convert.ToString(dtRow["DocNum"]);
                    approvalDecision.EMPLOYEECODE = Convert.ToString(dtRow["EMPLOYEECODE"]);
                    approvalDecision.USER_NAME = Convert.ToString(dtRow["USERNAME"]);
                    if (Convert.ToString(dtRow["Date"]) != "")
                        approvalDecision.ClaimDate = Convert.ToString(Convert.ToDateTime(dtRow["Date"]).ToString("dd/MM/yyyy"));
                    approvalDecision.Assignment = Convert.ToString(dtRow["AssignmentTitle"]);
                    approvalDecision.Description = Convert.ToString(dtRow["Description"]);
                    approvalDecision.TotalAmount = Convert.ToString(dtRow["TotalAmount"]);
                    sno = sno + 1;
                    approvalDecisionL.Add(approvalDecision);
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("GetDocView", "Exception occured on TranslateDataTableToClaimDocViewManagementList, " + ex.Message);
            }

            return approvalDecisionL;
        }

        private List<ApprovalDecision> TranslateDataTableToAssignmentDocViewManagementList(DataTable dt)
        {
            List<ApprovalDecision> approvalDecisionL = new List<ApprovalDecision>();

            try
            {
                int sno = 1;
                foreach (DataRow dtRow in dt.Rows)
                {
                    ApprovalDecision approvalDecision = new ApprovalDecision();
                    approvalDecision.SNO = sno;
                    approvalDecision.KEY = Guid.NewGuid().ToString();
                    approvalDecision.ID = Convert.ToInt32(dtRow["ID"]);
                    approvalDecision.DocNum = Convert.ToString(dtRow["DocNum"]);
                    approvalDecision.EMPLOYEECODE = Convert.ToString(dtRow["EMPLOYEECODE"]);
                    approvalDecision.USER_NAME = Convert.ToString(dtRow["USERNAME"]);
                    if (Convert.ToString(dtRow["CreateDate"]) != "")
                        approvalDecision.CreateDate = Convert.ToString(Convert.ToDateTime(dtRow["CreateDate"]).ToString("dd/MMM/yyyy"));
                    approvalDecision.TypeOfAssignment = Convert.ToString(dtRow["TypeOfAssignment"]);
                    approvalDecision.TypeOfBilling = Convert.ToString(dtRow["TypeOfBilling"]);
                    approvalDecision.CurrencyID = Convert.ToString(dtRow["CurrencyID"]);
                    approvalDecision.AssignmentValue = Convert.ToDouble(dtRow["AssignmentValue"]);
                    approvalDecision.StartDate = Convert.ToString(Convert.ToDateTime(dtRow["StartDate"]).ToString("dd/MMM/yyyy"));
                    approvalDecision.EndDate = Convert.ToString(Convert.ToDateTime(dtRow["EndDate"]).ToString("dd/MMM/yyyy"));
                    approvalDecision.STATUS = Convert.ToString(dtRow["Status"]);
                    sno = sno + 1;
                    approvalDecisionL.Add(approvalDecision);
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("GetDocView", "Exception occured on TranslateDataTableToAssignmentDocViewManagementList, " + ex.Message);
            }

            return approvalDecisionL;
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
                log.InputOutputDocLog("ApprovalDecisionManagement", "Exception occured on TranslateApprovalDecisionToParameterList, " + ex.Message);
            }
            return parmList;
        }

        public List<ApprovalDecision> Table;

        public List<ApprovalDecision> GetApprovalDecision(int ID, List<UserProfile> usr)
        {
            List<ApprovalDecision> ApprovalDecision = new List<ApprovalDecision>();
            try
            {
                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                DataTable dt_ApprovalDecision = HANADAL.GetDataTableByStoredProcedure("GetSubmittedTimeSheetForApprovalsByEmpID", TranslateIDToParameterList(ID), "ApprovalDecisionManagement");

                if (dt_ApprovalDecision.Rows.Count > 0)
                {
                    ApprovalDecision = TranslateDataTableToApprovalDecisionManagementList(dt_ApprovalDecision, usr);
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ApprovalDecisionManagement", "Exception occured on GetSubmittedTimeSheetForApprovalsByEmpID: " + ID + " , " + ex.Message);
            }

            return ApprovalDecision;
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
                        UserManagement umgt = new UserManagement();

                        HCM_Employee emp = umgt.GetHCMOneEmployeeByCode(EmployeeCode);
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
                log.InputOutputDocLog("UserManagament", "Exception occured on TranslateDataTableToUserProfile, " + ex.Message);
            }

            return list;
        }


        private List<ApprovalDecision> TranslateDataTableToApprovalDecisionManagementList(DataTable dt, List<UserProfile> usr)
        {
            List<ApprovalDecision> approvalDecisionL = new List<ApprovalDecision>();
            UserManagement usrr = new UserManagement();
            Encrypt_Decrypt security = new Encrypt_Decrypt();
            try
            {
                int sno = 1;

                foreach (DataRow dtRow in dt.Rows)
                {
                    ApprovalDecision approvalDecision = new ApprovalDecision();
                    approvalDecision.SNO = sno;
                    approvalDecision.KEY = Guid.NewGuid().ToString();

                    approvalDecision.USER_CODE = Convert.ToString(dtRow["USER_CODE"]);
                    approvalDecision.ENC_USER_CODE = security.EncryptURLString(approvalDecision.USER_CODE);
                    approvalDecision.USER_NAME = Convert.ToString(dtRow["USER_NAME"]);
                    approvalDecision.EMPLOYEECODE = Convert.ToString(dtRow["EMPLOYEECODE"]);
                    approvalDecision.DOCUMENT_ID = Convert.ToInt32(dtRow["DOCUMENT_ID"]);
                    approvalDecision.DOCUMENT_NO = Convert.ToString(dtRow["DOCUMENT_NO"]);
                    approvalDecision.ENC_DOCUMENT_NO = security.EncryptURLString(approvalDecision.DOCUMENT_NO);
                    approvalDecision.FROMDATE = Convert.ToString(Convert.ToDateTime(dtRow["FromDate"]).ToLongDateString());
                    approvalDecision.TODATE = Convert.ToString(Convert.ToDateTime(dtRow["ToDate"]).ToLongDateString());
                    approvalDecision.DOCUMENT = Convert.ToString(dtRow["DocumentType"]);
                    approvalDecision.APPROVALREQUIRED = Convert.ToString(dtRow["APPROVALREQUIRED"]);
                    approvalDecision.REJECTIONREQUIRED = Convert.ToString(dtRow["REJECTIONREQUIRED"]);
                    approvalDecision.ID = Convert.ToInt32(dtRow["ID"]);
                    approvalDecision.CurrentAPPROVAL = Convert.ToString(dtRow["APPROVAL"]);
                    approvalDecision.CurrentREJECTION = Convert.ToString(dtRow["REJECTION"]);
                    string EmployeeCode = approvalDecision.EMPLOYEECODE;

                    if (!string.IsNullOrEmpty(EmployeeCode))
                    {
                        HCM_Employee emp = usrr.GetHCMOneEmployeeByCode(EmployeeCode);
                        if (emp != null)
                        {
                            approvalDecision.DEPARTMENTID = emp.DepartmentID;
                            approvalDecision.DEPARTMENTNAME = emp.DepartmentName;
                            approvalDecision.DESIGNATIONID = emp.DesignationID;

                            approvalDecision.DESIGNATIONNAME = emp.DesignationName;
                            approvalDecision.EMAIL = emp.OfficeEmail;
                            approvalDecision.USER_NAME = emp.EmpName;
                        }
                    }

                    //approvalDecision.UserID = Convert.ToInt32(dtRow["UserID"]);
                    //approvalDecision.DESIGNATIONID = Convert.ToInt32(dtRow["DESIGNATIONID"]);
                    //int usrcode = Convert.ToInt32(approvalDecision.USER_CODE);

                    //UserProfile usrpro = new UserProfile();
                    //usrpro = (from a in usr where a.ID == usrcode || a.USERNAME == approvalDecision.USER_NAME select a).FirstOrDefault();
                    //if(usrpro != null)
                    //{
                    //    approvalDecision.DESIGNATIONNAME = usrpro.DESIGNATIONNAME;
                    //    approvalDecision.DEPARTMENTNAME = usrpro.DEPARTMENTNAME;
                    //}

                    //approvalDecision.CREATEDBY = Convert.ToInt32(dtRow["CREATEDBY"]);
                    //approvalDecision.CREATEDATE = Convert.ToDateTime(dtRow["CREATEDATE"]);
                    //if (dtRow["UPDATEDBY"] != DBNull.Value)
                    //    approvalDecision.UPDATEDBY = Convert.ToInt32(dtRow["UPDATEDBY"]);
                    //if (dtRow["UPDATEDATE"] != DBNull.Value)
                    //    approvalDecision.UPDATEDATE = Convert.ToDateTime(dtRow["UPDATEDATE"]);

                    //approvalDecision.ISACTIVE = Convert.ToBoolean(dtRow["ISACTIVE"]);
                    //approvalDecision.ISREJECTED = Convert.ToBoolean(dtRow["ISREJECTED"]);
                    sno = sno + 1;
                    approvalDecisionL.Add(approvalDecision);
                }

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ApprovalDecisionManagement", "Exception occured on TranslateDataTableToApprovalDecisionManagementList, " + ex.Message);
            }

            return approvalDecisionL;
        }

        private List<B1SP_Parameter> TranslateIDSToParameterListforChange(int EmpID, int DocumentID)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {
                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "EmpID";
                parm.ParameterValue = Convert.ToString(EmpID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "DocumentID";
                parm.ParameterValue = Convert.ToString(DocumentID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ApprovalDecisionManagement", "Exception occured on TranslateIDSToParameterListforChange, " + ex.Message);
            }
            return parmList;
        }

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
                log.InputOutputDocLog("ApprovalDecisionManagement", "Exception occured on TranslateApprovalSetupChildToParameterList, " + ex.Message);
            }


            return parmList;
        }
        string ApproverRequest = "";

        private List<B1SP_Parameter> TranslateApprovalSetupChildChangeRemoveToParameterList(int UserID, string OriginatorUser)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {
                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "UserID";
                parm.ParameterValue = Convert.ToString(OriginatorUser);
                parm.ParameterType = DBTypes.Int32.ToString();
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
                log.InputOutputDocLog("ApprovalDecisionManagement", "Exception occured on TranslateApprovalSetupChildChangeRemoveToParameterList, " + ex.Message);
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
                log.InputOutputDocLog("ApprovalDecisionManagement", "Exception occured on TranslateTimeSheetToParameterList, " + ex.Message);
            }


            return parmList;
        }

        public bool UpdateApprovalDecision(out string msg, string DocType, int STATUS, List<ApprovalDecision> ApprovalDecision, int UserID, string UserFullName, int DOCUMENT_ID, string Comments)
        {
            bool isSuccess = false;
            bool isAddOccured = false;
            bool isUpdateOccured = false;
            bool isDeleteOccured = false;
            msg = "";
            NotificationManagement notification = new NotificationManagement();
            Email emails = new Email();
            UserManagement usr = new UserManagement();

            try
            {
                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                //ApprovalDecision ApprovalDecision1 = ApprovalDecision.Where(z => z.DOCUMENT_ID == DOCUMENT_ID).Select(z => z).FirstOrDefault();
                ApprovalDecision ApprovalDecision1 = ApprovalDecision.Where(z => z.DOCUMENT_ID == DOCUMENT_ID && z.DOCUMENT == DocType).Select(z => z).FirstOrDefault();
                if (ApprovalDecision1==null)
                {
                    msg = "Your document status has been changed.";
                    return false;
                }
                int rejection = Convert.ToInt32(ApprovalDecision1.CurrentREJECTION);
                int approval = Convert.ToInt32(ApprovalDecision1.CurrentAPPROVAL);
                ApprovalDecision1.STATUS = Convert.ToString(STATUS);
                string docType = ApprovalDecision1.DOCUMENT;

                if (STATUS == 5)
                {
                    rejection = rejection + 1;
                    ApprovalDecision1.CurrentREJECTION = Convert.ToString(rejection);
                    if (Convert.ToInt32(ApprovalDecision1.REJECTIONREQUIRED) == rejection)
                    {
                        msg = "Your " + docType + " document has successfully rejected.";
                        DataTable dt_Decision = HANADAL.AddUpdateDataByStoredProcedure("UpdateDocumentViaApprovalDecision", TranslateApprovalDecisionToParameterList(DOCUMENT_ID, STATUS, UserID, docType), "ApprovalDecisionManagement");

                        if (dt_Decision.Rows.Count == 0)
                            throw new Exception("Exception occured when UpdateTimeSheetViaApprovalDecision,  DOCUMENT CODE:" + DOCUMENT_ID + " , STATUS" + STATUS);

                    }
                    else
                    {
                        msg = "Your " + docType + " document has successfully rejected. Need more to reject.";
                    }
                }
                if (STATUS == 4)
                {
                    approval = approval + 1;
                    ApprovalDecision1.CurrentAPPROVAL = Convert.ToString(approval);
                    if (Convert.ToInt32(ApprovalDecision1.APPROVALREQUIRED) == approval)
                    {
                        msg = "Your " + docType + " document has successfully approved.";
                        DataTable dt_Decision = HANADAL.AddUpdateDataByStoredProcedure("UpdateDocumentViaApprovalDecision", TranslateApprovalDecisionToParameterList(DOCUMENT_ID, STATUS, UserID, docType), "ApprovalDecisionManagement");

                        if (dt_Decision.Rows.Count == 0)
                            throw new Exception("Exception occured when UpdateTimeSheetViaApprovalDecision,  DOCUMENT CODE:" + DOCUMENT_ID + " , STATUS" + STATUS);

                        if (ApprovalDecision1.DOCUMENT == "Timesheet")
                        {
                            DataTable dt_approvalSetup = HANADAL.AddUpdateDataByStoredProcedure("UpdateApprovalSetupforRemoveChange", TranslateApprovalSetupChildChangeRemoveToParameterList(UserID, ApprovalDecision1.USER_CODE), "ApprovalDecisionManagement");
                                if (dt_approvalSetup.Rows.Count == 0)
                                    throw new Exception("Exception occured when UpdateDocumentViaApprovalDecision, ID:" + UserID + ", USER CODE" + ApprovalDecision1.USER_CODE);

                        }
                        if (ApprovalDecision1.DOCUMENT == "Change Approver")
                        {
                            List<ChangeApproverChild> changeApproverChild = new List<ChangeApproverChild>();
                            int ChangeEmpId = 0;
                            int docID = 0;
                            if(ApprovalDecision1.USER_CODE != null && ApprovalDecision1.USER_CODE != "")
                            {
                                ChangeEmpId = Convert.ToInt32(ApprovalDecision1.USER_CODE);

                            }
                            if(ApprovalDecision1.DOCUMENT_ID != null && ApprovalDecision1.DOCUMENT_ID != 0)
                            {
                                docID = Convert.ToInt32(ApprovalDecision1.DOCUMENT_ID);
                            }
                            //    ChangeApproverManagement CAM = new ChangeApproverManagement();
                            //changeApproverChild = CAM.GetChangeApproverForms(ChangeEmpId, 0, 0);

                            try
                            {
                                DataTable dt_ChangeApprover = new DataTable();
                                if (ChangeEmpId > 0)
                                {
                                    dt_ChangeApprover = HANADAL.GetDataTableByStoredProcedure("GetChange_Approver", TranslateIDToParameterList(docID), "ApprovalDecisionManagement");
                                }
                               
                                if (dt_ChangeApprover.Rows.Count > 0)
                                {
                                    int empID = Convert.ToInt32( dt_ChangeApprover.Rows[0]["EmpID"]);
                                    //dt_ChangeApprover = HANADAL.GetDataTableByStoredProcedure("GetPendingApproveTimeSheetByEmpIDForChange", TranslateIDSToParameterListforChange(empID, docID), "ApprovalDecisionManagement");
                                    changeApproverChild = TranslateDataTableToGetChangeApproverTimesheetList(dt_ChangeApprover);
                                    
                                }
                                foreach (var list in changeApproverChild)
                                {
                                    DataTable dt_approvalSetup = HANADAL.AddUpdateDataByStoredProcedure("UpdateApproval_Setup_Child", TranslateApprovalSetupChildToParameterList(list), "ChangeApproverManagement");
                                    if (dt_approvalSetup.Rows.Count == 0)
                                        throw new Exception("Exception occured when Update Approval_Setup_Child, ID:" + list.ID + ", USER CODE" + list.EmpID);

                                    else
                                    {
                                        DataTable dt_timeSheet = HANADAL.AddUpdateDataByStoredProcedure("UpdateTimeSheetViaChangeApprover", TranslateTimeSheetToParameterList(list), "ChangeApproverManagement");
                                        if (dt_timeSheet.Rows.Count == 0)
                                            throw new Exception("Exception occured when Update Timesheet, ID:" + list.DocumentID + ", USER CODE" + list.EmpID);

                                        int empCode = Convert.ToInt32(list.EmpID);

                                        string Msg = "Approval Setup has been created for your timesheet document";
                                        notification.AddNotification(UserID, empCode, Msg);
                                        string usrEmail = usr.GetUserEmailByID(empCode);
                                        string prevMsg = msg;
                                        //string subject = "Approval Setup created by " + fullName;
                                        string subject = "Approval Setup created " + 0;
                                        string body = "<b>" + Msg + " </b>";

                                        msg = "";
                                        if (!emails.SendEmail(usrEmail, body, subject, null, out msg))
                                            throw new Exception(msg);

                                        msg = prevMsg;

                                        if (!string.IsNullOrEmpty(list.CHANGETOEMPCODE))
                                        {
                                            BAL.UserManagement mgts = new BAL.UserManagement();

                                            HCM_Employee emp = mgts.GetHCMOneEmployeeByCode(list.CHANGETOEMPCODE);
                                            if (emp != null)
                                            {
                                                string Msgs = "Approval Setup has been created. You are approving Timesheet document of " + list.FullName;
                                                notification.AddNotification(UserID, Convert.ToInt32(list.CHANGETOEMPID), Msgs);
                                                string prevMsgs = msg;
                                                //string subjects = "Approval Setup created by " + fullName;
                                                string subjects = "Approval Setup created " + 0;
                                                string bodys = "<b>" + Msgs + " </b>";

                                                msg = "";
                                                if (!emails.SendEmail(emp.OfficeEmail, bodys, subjects, null, out msg))
                                                    throw new Exception(msg);

                                                //sendedEmail.Add(approvarId);
                                                msg = prevMsgs;
                                            }
                                        }
                                    }
                                    msg = "Successfully Added/Updated";
                                    isSuccess = true;
                                }

                                    ChangeApproverManagement CAM = new ChangeApproverManagement();
                                changeApproverChild = CAM.GetChangeApproverForms(ChangeEmpId, 0, 0);

                            }

                            catch (Exception ex)
                            {
                                Log log = new Log();
                                log.LogFile(ex.Message);
                                log.InputOutputDocLog("ChangeApproverManagement", "Exception occured on GetChangeApproverForms on EmpID: " + ChangeEmpId + " , " + ex.Message);
                            }
                        }
                    }
                    else
                    {
                        msg = "Your " + docType + " document has successfully approved. Need more to approve.";
                    }
                }
                // Condition add krni hai stages code mai jakay dekhay ga kitne approvals required hai agr count pora tou status update krdy
                DataTable dt_approvalDecision = HANADAL.AddUpdateDataByStoredProcedure("AddUpdateApproval_Decision", TranslateApprovalDecisionToParameterList(ApprovalDecision1, UserID), "ApprovalDecisionManagement");
                if (dt_approvalDecision.Rows.Count == 0)
                    throw new Exception("Exception occured when UpdateApprovalDecision,  DOCUMENT CODE:" + DOCUMENT_ID + " , STATUS" + STATUS);

                else
                {
                    ApprovalDecision1.ApprovalDecision_ID = Convert.ToInt32(dt_approvalDecision.Rows[0]["ID"]);
                    //ApprovalDecision1.STATUS = Convert.ToString(STATUS);
                    int empCode = Convert.ToInt32(dt_approvalDecision.Rows[0]["EmpID"]);
                    //string empName = Convert.ToString(dt_approvalDecision.Rows[0]["USERNAME"]);
                    ApprovalDecision1.COMMENTS = Comments;
                    DataTable dt_timesheet = HANADAL.AddUpdateDataByStoredProcedure("AddApproval_Decision_Comments", TranslateApprovalDecisionCommentsToParameterList(ApprovalDecision1, UserID), "ApprovalDecisionManagement");
                    if (dt_timesheet.Rows.Count == 0)
                        throw new Exception("Exception occured when AddApproval_Decision_Comments,  DOCUMENT CODE:" + ApprovalDecision1.ApprovalDecision_ID + " , STATUS" + STATUS);
                    //int rejection = Convert.ToInt32(dt_approvalDecision.Rows[0]["REJECTION"]);
                    //int approval = Convert.ToInt32(dt_approvalDecision.Rows[0]["APPROVAL"]);
                    //if (STATUS == 5)
                    //{
                    //    rejection = rejection + 1;
                    //    dt_approvalDecision.Rows[0]["REJECTION"] = rejection;
                    //    if (ApprovalDecision1.REJECTIONREQUIRED == rejection)
                    //    {
                    //        msg = "Successfully Rejected";
                    //    }
                    //    else
                    //    {
                    //        msg = "Successfully Submitted. Need more to reject.";
                    //    }
                    //}
                    //if (STATUS == 4)
                    //{
                    //    approval = approval + 1;
                    //    dt_approvalDecision.Rows[0]["APPROVAL"] = approval + 1;
                    //    if (ApprovalDecision1.APPROVAL == approval)
                    //    {
                    //        msg = "Successfully Approved";
                    //    }
                    //    else
                    //    {
                    //        msg = "Successfully Submitted. Need more to approve.";
                    //    }
                    //}


                    //if (STATUS == 4)
                    //{
                    //    msg = "Successfully Approved";
                    //}
                    //else if (STATUS == 5)
                    //{
                    //    msg = "Successfully Rejected";
                    //}
                    //UserManagement usrr = new UserManagement();
                    //if (!string.IsNullOrEmpty())
                    //{
                    //    HCM_Employee emp = usrr.GetUserByID.GetHCMOneEmployeeByCode(UserID);
                    //    if (emp != null)
                    //    {
                    //        approvalDecision.DEPARTMENTID = emp.DepartmentID;
                    //        approvalDecision.DEPARTMENTNAME = emp.DepartmentName;
                    //        approvalDecision.DESIGNATIONID = emp.DesignationID;

                    //        approvalDecision.DESIGNATIONNAME = emp.DesignationName;
                    //        approvalDecision.EMAIL = emp.OfficeEmail;
                    //        approvalDecision.USER_NAME = emp.EmpName;
                    //    }
                    //}
                    string prevMsg = msg;
                    NotificationManagement notifications = new NotificationManagement();
                    msg = msg + "\n" + Comments;
                    notifications.AddNotification(UserID, empCode, msg);
                    //sendedNotification.Add(approvarId);

                    string subject = "Approval Decision by " + UserFullName;
                    string body = "<b>" + msg + " </b>";

                    //string 
                    msg = "";
                    Email email = new Email();
                    if (!email.SendEmail(ApprovalDecision1.EMAIL, body, subject, null, out msg))
                        throw new Exception(msg);

                    //sendedEmail.Add(approvarId);
                    msg = prevMsg;

                    isSuccess = true;


                }

            }
            catch (Exception ex)
            {
                msg = "Exception occured in Update Aprroval Decision!";
                isSuccess = false;
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ApprovalDecisionManagement", "Exception occured in foreach loop UpdateApprovalDecision, " + ex.Message);
            }
            return isSuccess;
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
                    //changeApprover.KEY = Guid.NewGuid().ToString();
                    changeApprover.ID = Convert.ToInt32(dtRow["ID"]);
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
                            changeApprover.DepartmentName = emp.DepartmentName;
                            changeApprover.DesignationID = emp.DesignationID;
                            changeApprover.DesignationName = emp.DesignationName;
                        }
                    }
                    changeApprover.CHANGETOEMPID = Convert.ToInt32(dtRow["CHANGETOEMPID"]);
                    changeApprover.CHANGETOEMPCODE = Convert.ToString(dtRow["CHANGETOEMPCODE"]);
                    changeApprover.ApprovalStatus = Convert.ToInt32(dtRow["ApprovalStatus"]);
                    changeApprover.UpdatedBy = changeApprover.CHANGETOEMPID;
                    changeApprover.UpdateDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:MM:s"));
                    //changeApprover.DepartmentID = Convert.ToInt32(dtRow["DEPARTMENTID"]);
                    //changeApprover.DepartmentName = Convert.ToString(dtRow["DEPARTMENTNAME"]);
                    //changeApprover.DesignationID = Convert.ToInt32(dtRow["DESIGNATIONID"]);
                    //changeApprover.DesignationName = Convert.ToString(dtRow["DESIGNATIONNAME"]);
                    //changeApprover.Year = Convert.ToString(dtRow["Year"]);
                    //changeApprover.Month = Convert.ToString(dtRow["Month"]);
                    //changeApprover.YearsWeeks = Convert.ToString(dtRow["YearsWeeks"]);
                    //changeApprover.Weeks = Convert.ToString(dtRow["Weeks"]);
                    //changeApprover.PendingAt = Convert.ToString(dtRow["PendingAt"]);
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

        private List<B1SP_Parameter> TranslateTimeSheetFormDetailsSetupToParameterList(TimeSheetForm TimeSheetFormDetails)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {

                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = Convert.ToString(TimeSheetFormDetails.ID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "Status";
                parm.ParameterValue = Convert.ToString(TimeSheetFormDetails.Status);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);


                parm = new B1SP_Parameter();
                parm.ParameterName = "UpdatedBy";
                parm.ParameterValue = Convert.ToString(TimeSheetFormDetails.UpdatedBy);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "UpdatedDate";
                parm.ParameterValue = DateTime.Now.ToString("yyyy-MM-dd");
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);


            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ApprovalDecisionManagement", "Exception occured on TranslateTimeSheetFormDetailsSetupToParameterList, " + ex.Message);
            }


            return parmList;
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


        #region "Translation approvalDecision"



        private List<B1SP_Parameter> TranslateApprovalDecisionToParameterList(int ID, int STATUS, int UserID, string docType)
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
                parm.ParameterName = "STATUS";
                parm.ParameterValue = Convert.ToString(STATUS);
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "UPDATEDBY";
                parm.ParameterValue = Convert.ToString(UserID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "UPDATEDATE";
                parm.ParameterValue = DateTime.Now.ToString("yyyy-MM-dd HH:MM:s");
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "DOCUMENT";
                parm.ParameterValue = Convert.ToString(docType);
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ApprovalDecisionManagement", "Exception occured on TranslateApprovalDecisionToParameterList, " + ex.Message);
            }

            return parmList;
        }

        private List<B1SP_Parameter> TranslateApprovalDecisionToParameterList(int ID, List<ApprovalDecision> ApprovalDecision, string DOCUMENT, int APPROVAL, int REJECTION, int STATUS, int UserID)
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
                parm.ParameterName = "DOCUMENT_ID";
                parm.ParameterValue = Convert.ToString(ID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "DOCUMENT";
                parm.ParameterValue = Convert.ToString(DOCUMENT);
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "APPROVAL";
                parm.ParameterValue = Convert.ToString(APPROVAL);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "REJECTION";
                parm.ParameterValue = Convert.ToString(REJECTION);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "STATUS";
                parm.ParameterValue = Convert.ToString(STATUS);
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "UPDATEDBY";
                parm.ParameterValue = Convert.ToString(UserID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "UPDATEDATE";
                parm.ParameterValue = DateTime.Now.ToString("yyyy-MM-dd HH:MM:s");
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ApprovalDecisionManagement", "Exception occured on TranslateApprovalDecisionToParameterList, " + ex.Message);
            }

            return parmList;
        }

        private List<B1SP_Parameter> TranslateApprovalDecisionToParameterList(ApprovalDecision approvalDecision, int UserID)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {

                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = Convert.ToString(approvalDecision.ID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CREATEDBY";
                parm.ParameterValue = Convert.ToString(UserID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CREATEDATE";
                parm.ParameterValue = DateTime.Now.ToString("yyyy-MM-dd HH:MM:s");
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "UPDATEDBY";
                parm.ParameterValue = Convert.ToString(UserID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "UPDATEDATE";
                parm.ParameterValue = DateTime.Now.ToString("yyyy-MM-dd HH:MM:s");
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "DOCUMENT_ID";
                parm.ParameterValue = Convert.ToString(approvalDecision.DOCUMENT_ID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "EmpId";
                parm.ParameterValue = Convert.ToString(approvalDecision.USER_CODE);
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "DOCUMENT";
                parm.ParameterValue = Convert.ToString(approvalDecision.DOCUMENT);
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "APPROVAL";
                parm.ParameterValue = Convert.ToString(approvalDecision.CurrentAPPROVAL);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "REJECTION";
                parm.ParameterValue = Convert.ToString(approvalDecision.CurrentREJECTION);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                //parm = new B1SP_Parameter();
                //parm.ParameterName = "APPROVALREQUIRED";
                //parm.ParameterValue = Convert.ToString(approvalDecision.APPROVALREQUIRED);
                //parmList.Add(parm);

                //parm = new B1SP_Parameter();
                //parm.ParameterName = "REJECTIONREQUIRED";
                //parm.ParameterValue = Convert.ToString(approvalDecision.REJECTIONREQUIRED);
                //parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "STATUS";
                parm.ParameterValue = Convert.ToString(approvalDecision.STATUS);
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);



            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ApprovalDecisionManagement", "Exception occured on TranslateApprovalDecisionChildToParameterList, " + ex.Message);
            }


            return parmList;
        }

        private List<B1SP_Parameter> TranslateApprovalDecisionCommentsToParameterList(ApprovalDecision approvalDecision, int UserID)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {

                B1SP_Parameter parm = new B1SP_Parameter();
                parm = new B1SP_Parameter();
                parm.ParameterName = "CREATEDBY";
                parm.ParameterValue = Convert.ToString(UserID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CREATEDATE";
                parm.ParameterValue = DateTime.Now.ToString("yyyy-MM-dd HH:MM:s");
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ApprovalDecision_ID";
                parm.ParameterValue = Convert.ToString(approvalDecision.ApprovalDecision_ID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "DOCUMENT_ID";
                parm.ParameterValue = Convert.ToString(approvalDecision.DOCUMENT_ID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                //parm = new B1SP_Parameter();
                //parm.ParameterName = "APPROVAL";
                //parm.ParameterValue = Convert.ToString(approvalDecision.CurrentAPPROVAL);
                //parmList.Add(parm);

                //parm = new B1SP_Parameter();
                //parm.ParameterName = "REJECTION";
                //parm.ParameterValue = Convert.ToString(approvalDecision.CurrentREJECTION);
                //parmList.Add(parm);

                //parm = new B1SP_Parameter();
                //parm.ParameterName = "APPROVALREQUIRED";
                //parm.ParameterValue = Convert.ToString(approvalDecision.APPROVALREQUIRED);
                //parmList.Add(parm);

                //parm = new B1SP_Parameter();
                //parm.ParameterName = "REJECTIONREQUIRED";
                //parm.ParameterValue = Convert.ToString(approvalDecision.REJECTIONREQUIRED);
                //parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "STATUS";
                parm.ParameterValue = Convert.ToString(approvalDecision.STATUS);
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "COMMENTS";
                parm.ParameterValue = Convert.ToString(approvalDecision.COMMENTS);
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);



            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ApprovalDecisionManagement", "Exception occured on TranslateApprovalDecisionCommentsToParameterList, " + ex.Message);
            }


            return parmList;
        }

        #endregion

    }
}