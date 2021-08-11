using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using TMSDeloitte.DAL;
using TMSDeloitte.Helper;
using TMSDeloitte.Models;

namespace TMSDeloitte.BAL
{

    public class Common
    {
       
        public void GetFiscYear(out string StartDate, out string EndDate)
        {
            StartDate = "";
            EndDate = "";

             StartDate = ConfigurationManager.AppSettings["StartDate"];
             EndDate = ConfigurationManager.AppSettings["EndDate"];

            if (string.IsNullOrEmpty(StartDate) || string.IsNullOrEmpty(EndDate))
            {
                string currentYear = ((DateTime.Now.Year) - 1).ToString();

                StartDate = currentYear + "-07-01";
                if (DateTime.Now > Convert.ToDateTime(StartDate).AddYears(1))
                    StartDate = Convert.ToDateTime(StartDate).AddYears(1).ToString("yyyy-MM-dd");

                EndDate = Convert.ToDateTime(StartDate).AddYears(1).AddMonths(-1).AddDays(29).ToString("yyyy-MM-dd");
            }
        }

        public void AddMasterLog(string formName, string action, int createdBy, string docType)
        {
            try
            {
                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                HANADAL.AddUpdateDataByStoredProcedure("AddLog", GetLogParameterList(formName, action, createdBy, docType), docType);
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog(docType, "Exception occured on AddMasterLog, " + ex.Message);
            }

        }

        public void AddMasterLog(bool isAdd,bool isUpdate,bool isDelete,string formName,int createdBy,string docType)
        {
            try
            {
                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();

                if(isAdd)
                    HANADAL.AddUpdateDataByStoredProcedure("AddLog", GetLogParameterList(formName, nameof(Enums.FormsOperations.Add), createdBy, docType), docType);
                if (isUpdate)
                    HANADAL.AddUpdateDataByStoredProcedure("AddLog", GetLogParameterList(formName, nameof(Enums.FormsOperations.Update), createdBy, docType), docType);
                if (isDelete)
                    HANADAL.AddUpdateDataByStoredProcedure("AddLog", GetLogParameterList(formName, nameof(Enums.FormsOperations.Delete), createdBy, docType), docType);
            }
            catch(Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog(docType, "Exception occured on AddMasterLog, " + ex.Message);
            }
           
        }

        public List<B1SP_Parameter> GetLogParameterList(string formName, string action, int createdBy,string docType)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {

                B1SP_Parameter parm = new B1SP_Parameter();

                parm = new B1SP_Parameter();
                parm.ParameterName = "FORMNAME";
                parm.ParameterValue = formName;
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "FORMNAMEACTION";
                parm.ParameterValue = action;
                parmList.Add(parm);

              
                parm = new B1SP_Parameter();
                parm.ParameterName = "CREATEDBY";
                parm.ParameterValue = Convert.ToString(createdBy);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CREATEDDATE";
                parm.ParameterValue = DateTime.Now.ToString("yyyy-MM-dd HH:MM:s");
                parmList.Add(parm);

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog(docType, "Exception occured on GetLogParameterList, " + ex.Message);
            }


            return parmList;
        }


        public List<SAP_Function> GetSAPFunctionList()
        {
            List<SAP_Function> sapFunctionList = new List<SAP_Function>();
            SAP_Function func = new SAP_Function();

            func = new SAP_Function();
            func.FunctionID = "1";
            func.FunctionName = "Audit";
            sapFunctionList.Add(func);

            func = new SAP_Function();
            func.FunctionID = "2";
            func.FunctionName = "Internal Audit";
            sapFunctionList.Add(func);

            func = new SAP_Function();
            func.FunctionID =" 3";
            func.FunctionName = "External Audit";
            sapFunctionList.Add(func);

            return sapFunctionList;
        }

        public List<HCM_Designation> GetHCMDesignationList()
        {
            List<HCM_Designation> hcmDesignationList = new List<HCM_Designation>();
            HCM_Designation desig = new HCM_Designation();

            desig = new HCM_Designation();
            desig.DesignationID = 1;
            desig.FunctionID = 1;
            desig.DesignationName = "Partner";
            hcmDesignationList.Add(desig);

            desig = new HCM_Designation();
            desig.DesignationID = 2;
            desig.FunctionID = 1;
            desig.DesignationName = "Director";
            hcmDesignationList.Add(desig);

            desig = new HCM_Designation();
            desig.DesignationID = 3;
            desig.FunctionID = 1;
            desig.DesignationName = "Manager";
            hcmDesignationList.Add(desig);

            desig = new HCM_Designation();
            desig.DesignationID = 4;
            desig.FunctionID = 2;
            desig.DesignationName = "Assistant Manager";
            hcmDesignationList.Add(desig);


            desig = new HCM_Designation();
            desig.DesignationID = 5;
            desig.FunctionID = 2;
            desig.DesignationName = "Senior";
            hcmDesignationList.Add(desig);

            desig = new HCM_Designation();
            desig.DesignationID = 6;
            desig.FunctionID = 2;
            desig.DesignationName = "Semi Senior";
            hcmDesignationList.Add(desig);

            desig = new HCM_Designation();
            desig.DesignationID = 7;
            desig.FunctionID = 3;
            desig.DesignationName = "Junior";
            hcmDesignationList.Add(desig);

            desig = new HCM_Designation();
            desig.DesignationID = 8;
            desig.FunctionID = 3;
            desig.DesignationName = "Trainee";
            hcmDesignationList.Add(desig);

            desig = new HCM_Designation();
            desig.DesignationID = 9;
            desig.FunctionID = 3;
            desig.DesignationName = "Test";
            hcmDesignationList.Add(desig);

            return hcmDesignationList;
        }

        public List<B1SP_Parameter> TranslateIDToParameterList(int id)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {

                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = Convert.ToString(id);
                parmList.Add(parm);
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("SetupManagement", "Exception occured on TranslateIDToParameterList, " + ex.Message);
            }


            return parmList;
        }

        public List<UserProfile> GetHCMUsers()
        {
            List<HCM_Designation> designationList= GetHCMDesignationList(); 

            List <UserProfile> userList = new List<UserProfile>();
            UserProfile user = new UserProfile();
            user.ID = 1;
            user.EMAIL = "admin@admin.com";
            user.EMPLOYEECODE = "EMP-001";
            user.DESIGNATIONID = 1;
            user.DEPARTMENTID = 1;
            user.DESIGNATIONNAME= designationList.Where(x => x.DesignationID == user.DESIGNATIONID).Select(l => l.DesignationName).FirstOrDefault();
            user.DEPARTMENTNAME = "Department1";
            user.USERNAME = "admin";
            userList.Add(user);

            user = new UserProfile();
            user.ID = 2;
            user.EMAIL = "jawwad.ati@gmail.com";
            user.EMPLOYEECODE = "EMP-002";
            user.DESIGNATIONID = 1;
            user.DEPARTMENTID = 1;
            user.DESIGNATIONNAME = designationList.Where(x => x.DesignationID == user.DESIGNATIONID).Select(l => l.DesignationName).FirstOrDefault();
            user.DEPARTMENTNAME = "Department1";
            user.USERNAME = "jawwad";
            userList.Add(user);


            user = new UserProfile();
            user.EMAIL = "shahzad@mail.com";
            user.EMPLOYEECODE = "EMP-003";
            user.DESIGNATIONID = 2;
            user.DEPARTMENTID = 2;
            user.DESIGNATIONNAME = designationList.Where(x => x.DesignationID == user.DESIGNATIONID).Select(l => l.DesignationName).FirstOrDefault();
            user.DEPARTMENTNAME = "Department2";
            user.USERNAME = "shahzad";
            userList.Add(user);

            user = new UserProfile();
            user.EMAIL = "bilal@mail.com";
            user.EMPLOYEECODE = "EMP-004";
            user.DESIGNATIONID = 2;
            user.DEPARTMENTID = 2;
            user.DESIGNATIONNAME = designationList.Where(x => x.DesignationID == user.DESIGNATIONID).Select(l => l.DesignationName).FirstOrDefault();
            user.DEPARTMENTNAME = "Department2";
            user.USERNAME = "bilal";
            userList.Add(user);

            user = new UserProfile();
            user.EMAIL = "User5@mail.com";
            user.EMPLOYEECODE = "EMP-005";
            user.DESIGNATIONID = 3;
            user.DEPARTMENTID = 3;
            user.DESIGNATIONNAME = designationList.Where(x => x.DesignationID == user.DESIGNATIONID).Select(l => l.DesignationName).FirstOrDefault();
            user.DEPARTMENTNAME = "Department3";
            user.USERNAME = "User5";
            userList.Add(user);

            user = new UserProfile();
            user.EMAIL = "User6@mail.com";
            user.EMPLOYEECODE = "EMP-006";
            user.DESIGNATIONID = 3;
            user.DEPARTMENTID = 3;
            user.DESIGNATIONNAME = designationList.Where(x => x.DesignationID == user.DESIGNATIONID).Select(l => l.DesignationName).FirstOrDefault();
            user.DEPARTMENTNAME = "Department3";
            user.USERNAME = "User6";
            userList.Add(user);

            return userList;
        }

        public List<SAP_Function> GetFunctionsFromSAPB1()
        {
            List<SAP_Function> sapFunctionL = new List<SAP_Function>();
            

            try
            {
                
                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();

                DataTable dt = HANADAL.GetDataTableByStoredProcedure("GETFunctionFromSAPB1", "TaskMasterManagement");
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow item in dt.Rows)
                    {
                        SAP_Function sapFunction = new SAP_Function();
                        
                        sapFunction.FunctionCode = Convert.ToString(item["FunctionCode"]);
                        sapFunction.FunctionName = Convert.ToString(item["FunctionName"]);

                        sapFunctionL.Add(sapFunction);
                    }
                    
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("TaskMasterManagement", "Exception occured on GetTaskMasterDocNum, " + ex.Message);
            }

            return sapFunctionL;
        }

        public List<SAP_Function> GetBranchesFromSAPB1(List<int> BranchList) 
        {
            //Below Committed code is changed at 19/01/2021 after session with moazzam 
            //He said all branches get from HCM DB
            //If we get branches from SAP it will not make any join between User Data Acces Authorization if user has no any authorization given

            List<SAP_Function> sapFunctionL = new List<SAP_Function>();

            try
            {

                HCMOneManagement mgt = new HCMOneManagement();
                List<Branch> List = mgt.GetAllFilteredHCMBranch(BranchList);
                foreach (Branch item in List)
                {
                    SAP_Function sapFunction = new SAP_Function();

                    sapFunction.BranchID = Convert.ToInt32(item.Id);
                    sapFunction.BranchName = Convert.ToString(item.Name);

                    sapFunctionL.Add(sapFunction);
                }


                //HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();

                //DataTable dt = HANADAL.GetDataTableByStoredProcedure("GETBranchesFromSAPB1", "TaskMasterManagement");
                //if (dt.Rows.Count > 0)
                //{
                //    foreach (DataRow item in dt.Rows)
                //    {
                //        SAP_Function sapFunction = new SAP_Function();

                //        sapFunction.BranchID = Convert.ToInt32(item["BranchID"]);
                //        sapFunction.BranchName = Convert.ToString(item["BranchName"]);

                //        sapFunctionL.Add(sapFunction);
                //    }

                //}
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("TaskMasterManagement", "Exception occured on GetTaskMasterDocNum, " + ex.Message);
            }

            return sapFunctionL;
        }


        public List<SAP_Function> GetFunctionsSAPB1(int j)
        {
            List<SAP_Function> sapFunctionL = new List<SAP_Function>();


            try
            {

                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();

                DataTable dt = HANADAL.GetDataTableByStoredProcedure("GETFunctionFromSAPB1", "TaskMasterManagement");
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow item in dt.Rows)
                    {
                        SAP_Function sapFunction = new SAP_Function();

                        sapFunction.FunctionID = Convert.ToString(item["FunctionCode"]);
                        sapFunction.FunctionName = Convert.ToString(item["FunctionName"]);

                        sapFunctionL.Add(sapFunction);
                    }

                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("TaskMasterManagement", "Exception occured on GetTaskMasterDocNum, " + ex.Message);
            }

            return sapFunctionL;
        }

        public List<SAP_Function> GetSubFunctionsFromSAPB1()
        {
            List<SAP_Function> sapFunctionL = new List<SAP_Function>();


            try
            {

                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();

                DataTable dt = HANADAL.GetDataTableByStoredProcedure("GETSubFunctionFromSAPB1", "AssifnmentFormController");
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow item in dt.Rows)
                    {
                        SAP_Function sapFunction = new SAP_Function();

                        sapFunction.SubFunctionID = Convert.ToString(item["SubFunctionCode"]);
                        sapFunction.SubFunctionName = Convert.ToString(item["SubFunctionName"]);

                        sapFunctionL.Add(sapFunction);
                    }

                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("AssignmentFormManagement", "Exception occured on GetSubFunctionsFromSAPB1, " + ex.Message);
            }

            return sapFunctionL;
        }

        public List<SAP_Function> GETPartnerFromSAPB1()
        {
            List<SAP_Function> sapFunctionL = new List<SAP_Function>();


            try
            {

                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();

                DataTable dt = HANADAL.GetDataTableByStoredProcedure("GETPartnerFromSAPB1", "AssifnmentFormController");
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow item in dt.Rows)
                    {
                        SAP_Function sapFunction = new SAP_Function();

                        sapFunction.PartnerID = Convert.ToString(item["PartnerCode"]);
                        sapFunction.PartnerName = Convert.ToString(item["PartnerName"]);

                        sapFunctionL.Add(sapFunction);
                    }

                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("AssignmentFormManagement", "Exception occured on GETPartnerFromSAPB1, " + ex.Message);
            }

            return sapFunctionL;
        }

        

        public List<SAP_Function> GetClientFromSAPB1()
        {
            List<SAP_Function> sapFunctionL = new List<SAP_Function>();


            try
            {

                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();

                DataTable dt = HANADAL.GetDataTableByStoredProcedure("GETClientFromSAPB1", "TravelLocationManagement");
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow item in dt.Rows)
                    {
                        SAP_Function sapFunction = new SAP_Function();

                        sapFunction.CLIENTID = Convert.ToString(item["CLIENTID"]);
                        sapFunction.CLIENTNAME = Convert.ToString(item["CLIENTNAME"]);

                        sapFunctionL.Add(sapFunction);
                    }

                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("TravelLocationManagement", "Exception occured on GetClientFromSAPB1, " + ex.Message);
            }

            return sapFunctionL;
        }

        public List<SAP_Function> GetAccountsFromSAPB1()
        {
            List<SAP_Function> sapFunctionL = new List<SAP_Function>();


            try
            {

                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();

                DataTable dt = HANADAL.GetDataTableByStoredProcedure("GETAccountsFromSAPB1", "WIPRecordingFormManagement");
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow item in dt.Rows)
                    {
                        SAP_Function sapFunction = new SAP_Function();

                        sapFunction.AcctCode = Convert.ToString(item["AcctCode"]);
                        sapFunction.AcctName = Convert.ToString(item["AcctName"]);

                        sapFunctionL.Add(sapFunction);
                    }

                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("WIPRecordingFormManagement", "Exception occured on GetAccountsFromSAPB1, " + ex.Message);
            }

            return sapFunctionL;
        }

        public List<SAP_Function> GetCurrencyFromSAPB1()
        {
            List<SAP_Function> sapFunctionL = new List<SAP_Function>();
            try
            {
                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();

                DataTable dt = HANADAL.GetDataTableByStoredProcedure("GETCurrencyFromSAPB1", "AssignmentFormManagement");
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow item in dt.Rows)
                    {
                        SAP_Function sapFunction = new SAP_Function();
                        sapFunction.CurrencyID = Convert.ToString(item["CurrencyID"]);
                        sapFunction.CurrencyName = Convert.ToString(item["CurrencyName"]);
                        sapFunctionL.Add(sapFunction);
                    }
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("TravelLocationManagement", "Exception occured on GetClientFromSAPB1, " + ex.Message);
            }
            return sapFunctionL;
        }

        public List<EnumUserAuthorizationViewModel> GetUserAuthorizationEnumsList()
        {
            
            List<EnumUserAuthorizationViewModel> userAuthEnumsList = ((Enums.UserAuthorization[])Enum.GetValues(typeof(Enums.UserAuthorization))).Select(c => new EnumUserAuthorizationViewModel() { Id = (int)c, Name = c.ToString() }).ToList();


            return userAuthEnumsList;
        }

        public  List<TMS_Menu> GetMenuList()
        {
            List<TMS_Menu> list = new List<TMS_Menu>();
            TMS_Menu menu = new TMS_Menu();
            int iCount = 1;

            #region Administration

            menu = new TMS_Menu();
            menu.ID = 1;
            menu.Head_ID = 0;
            menu.Name = "Administration";
            menu.PageURL = "";
            menu.Index = iCount;
            iCount = iCount + 1;
            list.Add(menu);

            menu = new TMS_Menu();
            menu.ID = 101;
            menu.Head_ID = 1;
            menu.Name = "User Management";
            menu.PageURL = "/User/Index";
            menu.Index = iCount;
            iCount = iCount + 1;
            list.Add(menu);
            
            menu = new TMS_Menu();
            menu.ID = 102;
            menu.Head_ID = 1;
            menu.Name = "User Authentication";
            menu.PageURL = "/UserAuthorization/Index";
            menu.Index = iCount;
            iCount = iCount + 1;
            list.Add(menu);
            
            menu = new TMS_Menu();
            menu.ID = 103;
            menu.Head_ID = 1;
            menu.Name = "User Data Access";
            menu.PageURL = "/UserDataAccess/Index";
            menu.Index = iCount;
            iCount = iCount + 1;
            list.Add(menu);

            #region Setup

            menu = new TMS_Menu();
            menu.ID = 104;
            menu.Head_ID = 1;
            menu.Name = "Setups";
            menu.PageURL = "";
            menu.Index = iCount;
            iCount = iCount + 1;
            list.Add(menu);
            
            //Child Setup Management
            menu = new TMS_Menu();
            menu.ID = 10041;
            menu.Head_ID = 104;
            menu.Name = "Assignment Cost Setup";
            menu.PageURL = "/AssignmentCostSetup/Index";
            menu.Index = iCount;
            iCount = iCount + 1;
            list.Add(menu);

            menu = new TMS_Menu();
            menu.ID = 10042;
            menu.Head_ID = 104;
            menu.Name = "Task Master Setup";
            menu.PageURL = "/TaskMasterSetup/TaskMasterSetup";
            menu.Index = iCount;
            iCount = iCount + 1;
            list.Add(menu);

            menu = new TMS_Menu();
            menu.ID = 10043;
            menu.Head_ID = 104; 
            menu.Name = "Assignment Nature Setup";
            menu.PageURL = "/AssignmentNatureSetup/Index";
            menu.Index = iCount;
            iCount = iCount + 1;
            list.Add(menu);

            menu = new TMS_Menu();
            menu.ID = 10044;
            menu.Head_ID = 104;
            menu.Name = "Non Chargable Setup";
            menu.PageURL = "/NonChargeableSetup/GetNonChargeable";
            menu.Index = iCount;
            iCount = iCount + 1;
            list.Add(menu);

            menu = new TMS_Menu();
            menu.ID = 10045;
            menu.Head_ID = 104;
            menu.Name = "Type of Claim Setup";
            menu.PageURL = "/TypeOfClaimSetup/GetTypeOfClaim";
            menu.Index = iCount;
            iCount = iCount + 1;
            list.Add(menu);

            menu = new TMS_Menu();
            menu.ID = 10046;
            menu.Head_ID = 104;
            menu.Name = "Resource Billing Rates Setup";
            menu.PageURL = "/ResourceBillingRate/Index";
            menu.Index = iCount;
            iCount = iCount + 1;
            list.Add(menu);

            menu = new TMS_Menu();
            menu.ID = 10047;
            menu.Head_ID = 104;
            menu.Name = "Travel Rates Setup";
            menu.PageURL = "/TravelRates/TravelRates";
            menu.Index = iCount;
            iCount = iCount + 1;
            list.Add(menu);

            menu = new TMS_Menu();
            menu.ID = 10048;
            menu.Head_ID = 104;
            menu.Name = "Travel Location Setup";
            menu.PageURL = "/TravelLocation/Index";
            menu.Index = iCount;
            iCount = iCount + 1;
            list.Add(menu);

            menu = new TMS_Menu();
            menu.ID = 10049;
            menu.Head_ID = 104;
            menu.Name = "Time Sheet Period Setup";
            menu.PageURL = "/TimeSheetPeriodSetup/Index";
            menu.Index = iCount;
            iCount = iCount + 1;
            list.Add(menu);

            menu = new TMS_Menu();
            menu.ID = 100410;
            menu.Head_ID = 104;
            menu.Name = "Alert Setup";
            menu.PageURL = "/AlertSetup/Index";
            menu.Index = iCount;
            iCount = iCount + 1;
            list.Add(menu);

            menu = new TMS_Menu();
            menu.ID = 100411;
            menu.Head_ID = 104;
            menu.Name = "Group Setup";
            menu.PageURL = "/GroupSetup/Index";
            menu.Index = iCount;
            iCount = iCount + 1;
            list.Add(menu);

            menu = new TMS_Menu();
            menu.ID = 100412;
            menu.Head_ID = 104;
            menu.Name = "Holiday Setup";
            menu.PageURL = "/Holiday/Index";
            menu.Index = iCount;
            iCount = iCount + 1;
            list.Add(menu);

            #endregion

            #region Approval Manegement
            menu = new TMS_Menu();
            menu.ID = 105;
            menu.Head_ID = 1;
            menu.Name = "Approval Management";
            menu.PageURL = "#";
            menu.Index = iCount;
            iCount = iCount + 1;
            list.Add(menu);

            //Child Approval Manegement
            menu = new TMS_Menu();
            menu.ID = 10051;
            menu.Head_ID = 105;
            menu.Name = "Approval Stages";
            menu.PageURL = "/ApprovalStage/Index";
            menu.Index = iCount;
            iCount = iCount + 1;
            list.Add(menu);

            menu = new TMS_Menu();
            menu.ID = 10052;
            menu.Head_ID = 105;
            menu.Name = "Approval Setup";
            menu.PageURL = "/ApprovalSetup/Index";
            menu.Index = iCount;
            iCount = iCount + 1;
            list.Add(menu);

            menu = new TMS_Menu();
            menu.ID = 10053;
            menu.Head_ID = 105;
            menu.Name = "Change Approver";
            menu.PageURL = "/ChangeApprover/Index";
            menu.Index = iCount;
            iCount = iCount + 1;
            list.Add(menu);

            menu = new TMS_Menu();
            menu.ID = 10054;
            menu.Head_ID = 105;
            menu.Name = "Approval Decision";
            menu.PageURL = "/ApprovalDecision/Index";
            menu.Index = iCount;
            iCount = iCount + 1;
            list.Add(menu);

            #endregion

            #region Project Management

            menu = new TMS_Menu();
            menu.ID = 106;
            menu.Head_ID = 1;
            menu.Name = "Project Management";
            menu.PageURL = "#";
            menu.Index = iCount;
            iCount = iCount + 1;
            list.Add(menu);

            menu = new TMS_Menu();
            menu.ID = 10061;
            menu.Head_ID = 106;
            menu.Name = "Assignment Form";
            menu.PageURL = "/AssignmentForm/Index";
            menu.Index = iCount;
            iCount = iCount + 1;
            list.Add(menu);

            menu = new TMS_Menu();
            menu.ID = 10062;
            menu.Head_ID = 106;
            menu.Name = "NC Task Assignment Form";
            menu.PageURL = "/NCTaskAssignment/Index";
            menu.Index = iCount;
            iCount = iCount + 1;
            list.Add(menu);

            menu = new TMS_Menu();
            menu.ID = 10063;
            menu.Head_ID = 106;
            menu.Name = "WIP Recording Form";
            menu.PageURL = "/WIPRecordingForm/Index";
            menu.Index = iCount;
            iCount = iCount + 1;
            list.Add(menu);

            #endregion

            #endregion


            #region Time sheet Management

            menu = new TMS_Menu();
            menu.ID = 2;
            menu.Head_ID = 0;
            menu.Name = "Time Sheet Management";
            menu.PageURL = "#";
            menu.Index = iCount;
            iCount = iCount + 1;
            list.Add(menu);


            menu = new TMS_Menu();
            menu.ID = 301;
            menu.Head_ID = 2;
            menu.Name = "Time Sheet Form";
            menu.PageURL = "/TimeSheetForm/Index";
            menu.Index = iCount;
            iCount = iCount + 1;
            list.Add(menu);

            menu = new TMS_Menu();
            menu.ID = 302;
            menu.Head_ID = 2;
            menu.Name = "Time Sheet View";
            menu.PageURL = "/TimeSheetView/Index";
            menu.Index = iCount;
            iCount = iCount + 1;
            list.Add(menu);

            #endregion


            #region Travel Management

            menu = new TMS_Menu();
            menu.ID = 3;
            menu.Head_ID = 0;
            menu.Name = "Travel Management";
            menu.PageURL = "#";
            menu.Index = iCount;
            iCount = iCount + 1;
            list.Add(menu);


            menu = new TMS_Menu();
            menu.ID = 401;
            menu.Head_ID = 3;
            menu.Name = "Monthly Travel Management";
            menu.PageURL = "/MonthlyTravelSheet/Index";
            menu.Index = iCount;
            iCount = iCount + 1;
            list.Add(menu);

            menu = new TMS_Menu();
            menu.ID = 402;
            menu.Head_ID = 3;
            menu.Name = "Claim Form";
            menu.PageURL = "/ClaimForm/Index";
            menu.Index = iCount;
            iCount = iCount + 1;
            list.Add(menu);

            #endregion


            #region Reports

            menu = new TMS_Menu();
            menu.ID = 4;
            menu.Head_ID = 0;
            menu.Name = "Reports";
            menu.PageURL = "#";
            menu.Index = iCount;
            iCount = iCount + 1;
            list.Add(menu);


            menu = new TMS_Menu();
            menu.ID = 501;
            menu.Head_ID = 4;
            menu.Name = "Report Uploader";
            menu.PageURL = "/ReportUploader/Index";
            menu.Index = iCount;
            iCount = iCount + 1;
            list.Add(menu);

            int serial = 502;
            HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
            DataTable dt = HANADAL.GetDataTableByStoredProcedure("GetReports", "AssignmentFormManagement");
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow item in dt.Rows)
                {
                    menu = new TMS_Menu();
                    menu.ID = serial;
                    menu.Head_ID = 4;
                    menu.Name = Convert.ToString(item["ReportName"]);
                    //menu.Name = "Report Viewer";
                    menu.PageURL = "/ReportViewer.aspx?ReportName=" + Convert.ToString(item["RptFile"]);
                    menu.Index = iCount;
                    iCount = iCount + 1;
                    list.Add(menu);
                    serial++;
                }

            }
            //menu = new TMS_Menu();
            //menu.ID = 503;
            //menu.Head_ID = 4;
            //menu.Name = "User Report";
            //menu.PageURL = "#";
            //menu.Index = iCount;
            //iCount = iCount + 1;
            //list.Add(menu);

            //menu = new TMS_Menu();
            //menu.ID = 504;
            //menu.Head_ID = 4;
            //menu.Name = "Higher Management Report";
            //menu.PageURL = "#";
            //menu.Index = iCount;
            //iCount = iCount + 1;
            //list.Add(menu);


            #endregion


            #region Dashboard

            menu = new TMS_Menu();
            menu.ID = 6;
            menu.Head_ID = 0;
            menu.Name = "Dashboard";
            menu.PageURL = "#";
            menu.Index = iCount;
            iCount = iCount + 1;
            list.Add(menu);


            menu = new TMS_Menu();
            menu.ID = 601;
            menu.Head_ID = 6;
            menu.Name = "My Total Hours";
            menu.PageURL = "";
            menu.Index = iCount;
            iCount = iCount + 1;
            list.Add(menu);

            menu = new TMS_Menu();
            menu.ID = 602;
            menu.Head_ID = 6;
            menu.Name = "My Leave Status";
            menu.PageURL = "";
            menu.Index = iCount;
            iCount = iCount + 1;
            list.Add(menu);

            menu = new TMS_Menu();
            menu.ID = 603;
            menu.Head_ID = 6;
            menu.Name = "My Timesheet Status";
            menu.PageURL = "";
            menu.Index = iCount;
            iCount = iCount + 1;
            list.Add(menu);

            menu = new TMS_Menu();
            menu.ID = 604;
            menu.Head_ID = 6;
            menu.Name = "Staff Wise Total Hours";
            menu.PageURL = "";
            menu.Index = iCount;
            iCount = iCount + 1;
            list.Add(menu);

            menu = new TMS_Menu();
            menu.ID = 605;
            menu.Head_ID = 6;
            menu.Name = "Staff Wise Timesheet Status";
            menu.PageURL = "";
            menu.Index = iCount;
            iCount = iCount + 1;
            list.Add(menu);

            menu = new TMS_Menu();
            menu.ID = 606;
            menu.Head_ID = 6;
            menu.Name = "Function Wise Timesheet Status";
            menu.PageURL = "";
            menu.Index = iCount;
            iCount = iCount + 1;
            list.Add(menu);

            //menu = new TMS_Menu();
            //menu.ID = 607;
            //menu.Head_ID = 6;
            //menu.Name = "Staff Wise Total Hours";
            //menu.PageURL = "";
            //menu.Index = iCount;
            //iCount = iCount + 1;
            //list.Add(menu);

            menu = new TMS_Menu();
            menu.ID = 607;
            menu.Head_ID = 6;
            menu.Name = "Time To Lapse";
            menu.PageURL = "";
            menu.Index = iCount;
            iCount = iCount + 1;
            list.Add(menu);

            #endregion


            return list;

        }

        public List<string> GetDocNum(string spName,string docType="")
        {
            List<string> docNumList = new List<string>();
            try
            {
                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();

                DataTable dt = HANADAL.GetDataTableByStoredProcedure(spName,docType);
                if (dt.Rows.Count > 0)
                {
                    docNumList = TranslateDataTableToDocNumList(dt);
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog(docType, "Exception occured on GetResourceBillingRatesDocNum, " + ex.Message);
            }

            return docNumList;
        }

        public List<string> TranslateDataTableToDocNumList(DataTable dt)
        {
            List<string> docNumList = new List<string>();
            foreach (DataRow dtRow in dt.Rows)
            {
                docNumList.Add(Convert.ToString(dtRow["DocNum"]));
            }

            return docNumList.Distinct().ToList();
        }

        public List<UserDataAccess_Menu> GetUserDataAccessMenuList()
        {
            List<UserDataAccess_Menu> list = new List<UserDataAccess_Menu>();
            UserDataAccess_Menu menu = new UserDataAccess_Menu();
            HCMOneManagement mgt = new HCMOneManagement();

            List<Department> allDepartments = mgt.GetAllHCMDepartment();
            List<Branch> allLocation = mgt.GetAllHCMBranch();
            int iCount = 1;

            menu = new UserDataAccess_Menu();
            menu.ID = 1;
            menu.Head_ID = 0;
            menu.Name = "Time Sheet View";
            menu.Index = iCount;
            iCount = iCount + 1;
            list.Add(menu);

            menu = new UserDataAccess_Menu();
            menu.ID = 2;
            menu.Head_ID = 0;
            menu.Name = "Department";
            menu.Index = iCount;
            iCount = iCount + 1;
            list.Add(menu);

            menu = new UserDataAccess_Menu();
            menu.ID = 3;
            menu.Head_ID = 0;
            menu.Name = "Branch";
            menu.Index = iCount;
            iCount = iCount + 1;
            list.Add(menu);

            foreach (Department item in allDepartments)
            {
                menu = new UserDataAccess_Menu();
                menu.ID =Convert.ToInt32("1000"+item.Id);
                menu.Head_ID = 1;
                menu.Name = item.Name;
                menu.Index = iCount;
                iCount = iCount + 1;
                list.Add(menu);
            }
            foreach (Department item in allDepartments)
            {
                menu = new UserDataAccess_Menu();
                menu.ID = Convert.ToInt32("2000" + item.Id);
                menu.Head_ID = 2;
                menu.Name = item.Name;
                menu.Index = iCount;
                iCount = iCount + 1;
                list.Add(menu);
            }

            foreach (Branch item in allLocation)
            {
                menu = new UserDataAccess_Menu();
                menu.ID = Convert.ToInt32("3000" + item.Id);
                menu.Head_ID = 3;
                menu.Name = item.Name;
                menu.Index = iCount;
                iCount = iCount + 1;
                list.Add(menu);
            }

            /*
            menu = new UserDataAccess_Menu();
            menu.ID = 1;
            menu.Head_ID = 0;
            menu.Name = "Time Sheet";
            menu.Index = iCount;
            iCount = iCount + 1;
            list.Add(menu);

            menu = new UserDataAccess_Menu();
            menu.ID = 101;
            menu.Head_ID = 1;
            menu.Name = "Financial Advisory";
            menu.Index = iCount;
            iCount = iCount + 1;
            list.Add(menu);

            menu = new UserDataAccess_Menu();
            menu.ID = 102;
            menu.Head_ID = 1;
            menu.Name = "Audit";
            menu.Index = iCount;
            iCount = iCount + 1;
            list.Add(menu);

            menu = new UserDataAccess_Menu();
            menu.ID = 103;
            menu.Head_ID = 1;
            menu.Name = "Tax & Legal";
            menu.Index = iCount;
            iCount = iCount + 1;
            list.Add(menu);

            menu = new UserDataAccess_Menu();
            menu.ID = 2;
            menu.Head_ID = 0;
            menu.Name = "Department";
            menu.Index = iCount;
            iCount = iCount + 1;
            list.Add(menu);

            menu = new UserDataAccess_Menu();
            menu.ID = 201;
            menu.Head_ID = 2;
            menu.Name = "Financial Advisory";
            menu.Index = iCount;
            iCount = iCount + 1;
            list.Add(menu);

            menu = new UserDataAccess_Menu();
            menu.ID = 202;
            menu.Head_ID = 2;
            menu.Name = "Audit";
            menu.Index = iCount;
            iCount = iCount + 1;
            list.Add(menu);

            menu = new UserDataAccess_Menu();
            menu.ID = 203;
            menu.Head_ID = 2;
            menu.Name = "Tax & Legal";
            menu.Index = iCount;
            iCount = iCount + 1;
            list.Add(menu);



            menu = new UserDataAccess_Menu();
            menu.ID = 3;
            menu.Head_ID = 0;
            menu.Name = "Branch";
            menu.Index = iCount;
            iCount = iCount + 1;
            list.Add(menu);

            menu = new UserDataAccess_Menu();
            menu.ID = 301;
            menu.Head_ID = 3;
            menu.Name = "Karachi";
            menu.Index = iCount;
            iCount = iCount + 1;
            list.Add(menu);

            menu = new UserDataAccess_Menu();
            menu.ID = 302;
            menu.Head_ID = 3;
            menu.Name = "Lahore";
            menu.Index = iCount;
            iCount = iCount + 1;
            list.Add(menu);

            menu = new UserDataAccess_Menu();
            menu.ID = 303;
            menu.Head_ID = 3;
            menu.Name = "Islamabad";
            menu.Index = iCount;
            iCount = iCount + 1;
            list.Add(menu);
            */
            return list;

        }
        
        public DataTable RemoveDuplicateRows(DataTable dTable, string colName)
        {
            Hashtable hTable = new Hashtable();
            ArrayList duplicateList = new ArrayList();

            //Add list of all the unique item value to hashtable, which stores combination of key, value pair.
            //And add duplicate item value in arraylist.
            foreach (DataRow drow in dTable.Rows)
            {
                if (hTable.Contains(drow[colName]))
                    duplicateList.Add(drow);
                else
                    hTable.Add(drow[colName], string.Empty);
            }

            //Removing a list of duplicate items from datatable.
            foreach (DataRow dRow in duplicateList)
                dTable.Rows.Remove(dRow);

            //Datatable which contains unique records will be return as output.
            return dTable;
        }

        public List<TimeSheetPeriods> GenerateTimeSheetPeriods(DateTime StartDate, DateTime EndDate,string year)
        {
            List<TimeSheetPeriods> dates = new List<TimeSheetPeriods>();
            try
            {
                //find first monday
                DateTime firstMonday = Enumerable.Range(0, 5)
                    .SkipWhile(x => StartDate.AddDays(x).DayOfWeek != DayOfWeek.Monday)
                    .Select(x => StartDate.AddDays(x))
                    .First();
                //get count of days
                TimeSpan ts = (TimeSpan)(EndDate - firstMonday);
                int sNo = 1;
                //add dates to list
                for (int i = 0; i < ts.Days; i += 7)
                {

                    //if(firstMonday.AddDays(i+6)<SeriesEndDate) //uncomment this line if you would like to get last sunday before SeriesEndDate
                    dates.Add(new TimeSheetPeriods()
                    {
                        SNo = sNo,
                        Period = sNo + "/" + year,
                        Monday = firstMonday.AddDays(i).ToString("ddd, dd MMM yyy"),
                        Friday = firstMonday.AddDays(i + 4).ToString("ddd, dd MMM yyy"),
                        _Monday = firstMonday.AddDays(i).ToString("yyyy-MM-dd"),
                        _Friday = firstMonday.AddDays(i + 4).ToString("yyyy-MM-dd")
                    });
                    sNo++;
                }
            }
            catch(Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("TimeSheetPeriodsManagement", "Exception occured on Generate TimeSheet Periods Method in Common"+Environment.NewLine+"Start Date: "+StartDate+" , End Date: "+EndDate+Environment.NewLine+"Exception: " + ex.Message);
            }
           

            return dates;
        }

        public List<TimseSheetStatus> GetTimeSheetFormStatusList()
        {
            List<TimseSheetStatus> list = new List<TimseSheetStatus>();
            TimseSheetStatus obj = new TimseSheetStatus();
            // (Approved, Pending, Rejected, Saved, canceled and Submitted)

            obj = new TimseSheetStatus(); 
            obj.ID = 1;
            obj.Name = "Saved"; //Save and Pending are same
            list.Add(obj); 

            obj = new TimseSheetStatus();
            obj.ID = 2;
            obj.Name = "Submitted";
            list.Add(obj);

            obj = new TimseSheetStatus();
            obj.ID = 3;
            obj.Name = "Canceled";
            list.Add(obj);

            obj = new TimseSheetStatus();
            obj.ID = 4;
            obj.Name = "Approved";
            list.Add(obj);

            obj = new TimseSheetStatus();
            obj.ID = 5;
            obj.Name = "Rejected";
            list.Add(obj);
            
            return list;
        }

        public List<DropDown> GetAssignmentType()
        {
            List<DropDown> list = new List<DropDown>();
            DropDown obj = new DropDown();
            // (Internal, External)

            obj = new DropDown();
            obj.ID = 1;
            obj.AssignmentType = "Internal";
            list.Add(obj);

            obj = new DropDown();
            obj.ID = 2;
            obj.AssignmentType = "External";
            list.Add(obj);

            return list;
        }
        public List<DropDown> GetBillingType()
        {
            List<DropDown> list = new List<DropDown>();
            DropDown obj = new DropDown();
            // (Internal, External)

            obj = new DropDown();
            obj.ID = 1;
            obj.BillingType = "Fixed";
            list.Add(obj);

            obj = new DropDown();
            obj.ID = 2;
            obj.BillingType = "Hourly";
            list.Add(obj);

            return list;
        }
        public List<DropDown> GetStatusFunction()
        {
            List<DropDown> list = new List<DropDown>();
            DropDown obj = new DropDown();
            // (Open, Close, Hold)

            obj = new DropDown();
            obj.ID = 1;
            obj.StatusType = "Open";
            list.Add(obj);

            obj = new DropDown();
            obj.ID = 2;
            obj.StatusType = "Close";
            list.Add(obj);

            obj = new DropDown();
            obj.ID = 3;
            obj.StatusType = "Hold";
            list.Add(obj);

            return list;
        }


        public void SndNotificationAndEmail(int fromEmpID, double period, int year, string lnkHref,string docType)
        {
            //TimeSheetFormManagement
            //MonthlyTravelSheetManagement
            //ClaimFormManagement 

            try
            {

                string spName = "";
                if (docType == "TimeSheetFormManagement")
                    spName = "GetTimeSheetApprovarIDs";

                if (docType == "MonthlyTravelSheetManagement")
                    spName = "GetTravelClaimApprovarIDs";

                if (docType == "ClaimFormManagement")
                    spName = "GetClaimApprovarIDs";

                if (docType == "AssignmentFormManagement")
                    spName = "GetAssigmentApprovarIDs";

                if (docType == "ChangeApproverManagement")
                    spName = "GetChangeApprovarIDs";

                UserManagement userMgt = new UserManagement();
                UserProfile user = userMgt.GetUserByID(fromEmpID);
                if (user == null)
                    throw new Exception("User detail not found when sending notification and email , User ID: " + fromEmpID);

                List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = Convert.ToString(fromEmpID);
                parmList.Add(parm);


                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                Common cmn = new Common();
                DataTable dt = HANADAL.GetDataTableByStoredProcedure(spName, parmList, docType);
                if (dt.Rows.Count > 0)
                {
                    List<int> sendedNotification = new List<int>();
                    foreach (DataRow dtRow in dt.Rows)
                    {
                        try
                        {
                            //For Notification
                            int approvarId = Convert.ToInt32(dtRow["ID"]);
                            string detail = "";
                            if (!sendedNotification.Contains(approvarId))
                            {
                                if (docType == "TimeSheetFormManagement")
                                    detail = user.FULLNAME + " has submitted time sheet of period: " + period + " and year: " + year;

                                if (docType == "MonthlyTravelSheetManagement")
                                {
                                    string monthName = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(8);
                                    detail = user.FULLNAME + " has submitted monthly travel of month: " + monthName + " and year: " + year;
                                }

                                if (docType == "ClaimFormManagement")
                                    detail = user.FULLNAME + " has submitted claim of total amount : " + period;

                                if (docType == "AssignmentFormManagement")
                                    detail = user.FULLNAME + " has submitted assignment of total amount : " + period;

                                if (docType == "ChangeApproverManagement")
                                    detail = user.FULLNAME + " has submitted change Approver of timesheet document no. : " + period;

                                NotificationManagement notiMgt = new NotificationManagement();
                                notiMgt.AddNotification(fromEmpID, approvarId, detail);
                                sendedNotification.Add(approvarId);
                            }
                            List<int> sendedEmail = new List<int>();
                            if (!sendedEmail.Contains(approvarId))
                            {
                                //For Email
                                UserProfile approvar = userMgt.GetUserByID(approvarId);
                                if (string.IsNullOrEmpty(approvar.EMAIL))
                                    throw new Exception("Approvar Email Not Found , Approvar id: " + approvarId);

                                Encrypt_Decrypt security = new Encrypt_Decrypt();
                                string encryptedID = security.EncryptString(Convert.ToString(approvarId)).Replace("+", "_");

                               string emailLinkHref = lnkHref.Replace("EncryptedID", encryptedID);

                                string subject = "";
                                if (docType == "TimeSheetFormManagement")
                                    subject = "Time Sheet Aproval Request - " + user.FULLNAME;

                                if (docType == "MonthlyTravelSheetManagement")
                                    subject = "Monthly Travel Aproval Request - " + user.FULLNAME;

                                if (docType == "ClaimFormManagement")
                                    subject = "Claim Aproval Request - " + user.FULLNAME;

                                if (docType == "AssignmentFormManagement")
                                    subject = "Assignment Aproval Request - " + user.FULLNAME;

                                if (docType == "ChangeApproverManagement")
                                    subject = "Change Aproval Request - " + user.FULLNAME;

                                string body = "<b>" + detail + ". </b><br/><a href='" + emailLinkHref + "'>Click To View</a>\n";
                                body = body+ "<br/><a href='" + emailLinkHref.Replace("GetApprovalDecision", "DocumentDecisionByEmail") + "&status="+ security.EncryptString("4") + "'>Click To Approve</a>\n";
                                body = body + "<br/><a href='" + emailLinkHref.Replace("GetApprovalDecision", "DocumentDecisionByEmail") + "&status=" + security.EncryptString("5") + "'>Click To Reject</a>\n";

                                string msg = "";
                                Email email = new Email();
                                if (!email.SendEmail(approvar.EMAIL, body, subject, null, out msg))
                                    throw new Exception(msg);

                                sendedEmail.Add(approvarId);
                            }
                        }
                        catch (Exception ex)
                        {
                            Log log = new Log();
                            log.LogFile(ex.Message);
                            log.InputOutputDocLog(docType, "Exception occured on Inner Loop, " + ex.Message);
                            continue;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog(docType, "Exception occured on SndNotificationAndEmail, " + ex.Message);
            }
        }

    }

}