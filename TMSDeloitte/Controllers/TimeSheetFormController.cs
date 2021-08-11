using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TMSDeloitte.BAL;
using TMSDeloitte.Helper;
using TMSDeloitte.Models;

namespace TMSDeloitte.Controllers
{
    public class TimeSheetFormController : BaseController
    {
        // GET: TimeSheetForm
        public ActionResult Index(string docNum, string empID)
        {
            bool isView = false;
            try
            {
                //if (!string.IsNullOrEmpty(docNum))
                //    docNum = docNum.Replace("_", "+");
                //if (!string.IsNullOrEmpty(empID))
                //    empID = empID.Replace("_", "+");

                Encrypt_Decrypt security = new Encrypt_Decrypt();
                if (!string.IsNullOrEmpty(docNum))
                {
                    docNum = security.DecryptURLString(docNum);
                    ViewBag.DocNum = docNum;
                    isView = true;
                }
                else
                    ViewBag.DocNum = "";

                if (!string.IsNullOrEmpty(empID))
                {
                    empID = security.DecryptURLString(empID);
                    ViewBag.EmpID = empID;
                    isView = true;
                }
                else
                    ViewBag.EmpID = "";

                ViewBag.isView = isView;
            }
            catch(Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("TimeSheetFormController", "Exception occured on Index Controller, DocNum: "+docNum+" , EmpID: "+empID+" , Exception" + ex.Message);
            }
           
            return View();
        }
       
        public ActionResult GetUsers()
        {
            List<UserProfile> HCMOneUsers = new List<UserProfile>();
            try
            {
              
                BAL.UserManagement mgt = new BAL.UserManagement();
                HCMOneUsers = mgt.GetAllUsers(false);

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("TimeSheetFormController", "Exception occured on GetHCMUsers Controller, " + ex.Message);
            }
            return Json(new { response = HCMOneUsers }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetUserInfoByEmpCode(string empCode)
        {
            UserProfile user = new UserProfile();
            bool isSuccess = false;
            try
            {
                BAL.UserManagement userMgt = new BAL.UserManagement();

                isSuccess = userMgt.GetUserByEmpCode(empCode, out user);
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("TimeSheetFormController", "Exception occured on GetUserInfoByEmpCode Controller, " + ex.Message);
            }
            return Json(new { Success = isSuccess, UserInfo = user }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTimeSheetByEmpIDandYear(int empID,string year,bool isView)
        {
            GenerateTimeSheet obj = new GenerateTimeSheet();
            try
            {
                BAL.TimeSheetPeriodManagement setupManagement = new BAL.TimeSheetPeriodManagement();
                obj = setupManagement.GetTimeSheetByEmpIDandYear(empID, year, isView);
                string jsonString = JsonConvert.SerializeObject(obj, Formatting.Indented, new JsonSerializerSettings() { DateFormatString = "yyyy/MM/dd" });

                var jsonResult = Json(jsonString, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;

                return Json(new { response = jsonResult }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ResourceBillingRateController", "Exception occured on GetResourceBillingRateByDocNum Controller, " + ex.Message);
            }
            return Json(new { response = obj }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLocationBrachID(int branchID)
        {
            List<TravelLocationInfo> objList = new List<TravelLocationInfo>();
            try
            {
                BAL.TravelLocationManagement setupManagement = new BAL.TravelLocationManagement();
                objList = setupManagement.GetTravel_LocationSetup_ByBranchID(branchID);

                //string jsonString = JsonConvert.SerializeObject(obj, Formatting.Indented, new JsonSerializerSettings() { DateFormatString = "yyyy/MM/dd" });
                //var jsonResult = Json(jsonString, JsonRequestBehavior.AllowGet);
                //jsonResult.MaxJsonLength = int.MaxValue;

                return Json(new { response = objList }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ResourceBillingRateController", "Exception occured on GetResourceBillingRateByDocNum Controller, " + ex.Message);
            }
            return Json(new { response = objList }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDocList(string empid)
        {
            BAL.TimeSheetFormManagement timeMgt = new BAL.TimeSheetFormManagement();
            BAL.TimeSheetFormManagement timeSheetMgt = new TimeSheetFormManagement();
            BAL.Common cmn = new Common();
            List<string> list = new List<string>();
            DataTable dtAssignmentList = new DataTable();
            try
            {
                int employeeID = 0;
                string empCode = ""; ;
                int branchID = 0; ;
                string employeeFullName = "";

                if (string.IsNullOrEmpty(empid))
                {
                    employeeID = CurrentUser.SessionUser.ID;
                    empCode = CurrentUser.SessionUser.EMPLOYEECODE;
                    branchID = Convert.ToInt32(CurrentUser.SessionUser.BRANCHID);
                    employeeFullName = CurrentUser.SessionUser.FULLNAME;
                }
                else
                {
                    employeeID = Convert.ToInt32(empid);
                    bool isSuccess = false;
                    UserProfile user = new UserProfile();
                    BAL.UserManagement mgt = new BAL.UserManagement();
                    isSuccess = mgt.ValidateUserByID(Convert.ToString(employeeID), out user);
                    if(isSuccess)
                    {
                        empCode = user.EMPLOYEECODE;
                        branchID = Convert.ToInt32(user.BRANCHID);
                        employeeFullName = user.FULLNAME;
                    }
                   
                }

                

                list = timeMgt.GetDocNumByEmpID(employeeID);

                dtAssignmentList = timeSheetMgt.GetAssignmentByEmpID(employeeID);
                string jsonString_dtAssignmentList = JsonConvert.SerializeObject(dtAssignmentList, Formatting.Indented, new JsonSerializerSettings() { DateFormatString = "dd/MM/yyyy HH:MM:s" });
                var jsonResult_dtAssignmentList = Json(jsonString_dtAssignmentList, JsonRequestBehavior.AllowGet);
                jsonResult_dtAssignmentList.MaxJsonLength = int.MaxValue;

                return Json(new
                {
                    docList = list,
                    statusList = cmn.GetTimeSheetFormStatusList(),
                    EmpID = employeeID,//CurrentUser.SessionUser.ID,
                    EmpCode = empCode,//CurrentUser.SessionUser.EMPLOYEECODE,
                    BranchID = branchID,//CurrentUser.SessionUser.BRANCHID,
                    EmpFullName = employeeFullName,//CurrentUser.SessionUser.FULLNAME
                   AssignmentList= jsonResult_dtAssignmentList,
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ResourceBillingRateController", "Exception occured on GetAllDocumentNum Controller, " + ex.Message);

                return Json(new
                {
                    docList = list,
                    EmpID = 0,
                    EmpCode = "",
                    BranchID = 0,
                    EmpFullName = "",
                    dtAssignmentList = dtAssignmentList
                }, JsonRequestBehavior.AllowGet);
            }
           
        }


        [HttpPost]
        public ActionResult AddUpdateTimeSheetPeriod(TimeSheetForm obj)
        {
            try
            {
                string msg = "Successfully Added/Updated";
                BAL.TimeSheetFormManagement setupManagement = new BAL.TimeSheetFormManagement();
                obj.CreatedBy = CurrentUser.SessionUser.ID;
                obj.UpdatedBy = CurrentUser.SessionUser.ID;
                return Json(new { Success = setupManagement.AddUpdateTimeSheetFormDetailsSetup(obj, out msg), Message = msg }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("TimeSheetFormController", "Exception occured on AddUpdateTimeSheetPeriod Controller, " + ex.Message);
                return Json(new { Success = false, Message = "Exception Occured When Add/Update Time Sheet Period!" }, JsonRequestBehavior.AllowGet);

            }

        }

        public ActionResult GetAllDocuments()
        {
            DataTable table = new DataTable();
            try
            {
                BAL.TimeSheetFormManagement setupManagement = new BAL.TimeSheetFormManagement();
                table = setupManagement.GetTimeSheetAllDocumentsList();

                string jsonString = JsonConvert.SerializeObject(table, Formatting.Indented, new JsonSerializerSettings() { DateFormatString = "ddd, dd MMM yyy" });

                var jsonResult = Json(jsonString, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;

                return Json(new { response = jsonResult }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("TimeSheetFormController", "Exception occured on GetAllDocuments Controller, " + ex.Message);
                return Json(new { response = table }, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult GetAllDocumentsByEmpID()
        {
            List<TimeSheetForm> list = new List<TimeSheetForm>();
            try
            {
                BAL.TimeSheetFormManagement setupManagement = new BAL.TimeSheetFormManagement();
                list = setupManagement.GetAllTimeSheetFormByEmpID(CurrentUser.SessionUser.ID);

                string jsonString = JsonConvert.SerializeObject(list, Formatting.Indented, new JsonSerializerSettings() { DateFormatString = "dd/MM/yyyy" });

                var jsonResult = Json(jsonString, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;

                return Json(new { response = jsonResult }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("TimeSheetFormController", "Exception occured on GetAllDocuments Controller, " + ex.Message);
                return Json(new { response = list }, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult GetTimeSheetByDocNum(string docNum)
        {
            TimeSheetForm obj = new TimeSheetForm();
            try
            {
                BAL.TimeSheetFormManagement setupManagement = new BAL.TimeSheetFormManagement();
                obj = setupManagement.GetTimeSheetByDocNum(docNum,CurrentUser.SessionUser.ID);
                string jsonString = JsonConvert.SerializeObject(obj, Formatting.Indented, new JsonSerializerSettings() { DateFormatString = "yyyy/MM/dd" });

                var jsonResult = Json(jsonString, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;

                return Json(new { response = jsonResult }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("TimeSheetFormController", "Exception occured on GetTimeSheetByDocNum Controller, " + ex.Message);
            }
            return Json(new { response = obj }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTimeSheet(string docNum,string EmpID)
        {
            TimeSheetForm obj = new TimeSheetForm();
            Encrypt_Decrypt security = new Encrypt_Decrypt();
            try
            {
                //docNum = docNum.Replace("_", "+");
                //docNum = security.DecryptString(docNum);
                //EmpID = security.DecryptString(EmpID);

                BAL.TimeSheetFormManagement setupManagement = new BAL.TimeSheetFormManagement();
                obj = setupManagement.GetTimeSheetByDocNum(docNum,Convert.ToInt32(EmpID));
                string jsonString = JsonConvert.SerializeObject(obj, Formatting.Indented, new JsonSerializerSettings() { DateFormatString = "yyyy/MM/dd" });

                var jsonResult = Json(jsonString, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;

                return Json(new { response = jsonResult }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("TimeSheetFormController", "Exception occured on GetTimeSheetByDocNum Controller, " + ex.Message);
            }
            return Json(new { response = obj }, JsonRequestBehavior.AllowGet);
            
        }
        
        public ActionResult GetAssignmentTaskByEmpID(int id)
        {
            DataTable dt_Task = new DataTable();
           
            try
            {
                BAL.TimeSheetFormManagement mgt = new BAL.TimeSheetFormManagement();

                dt_Task = mgt.GetAssignmentTaskByEmpID(id);
                string jsonString_Task = JsonConvert.SerializeObject(dt_Task, Formatting.Indented, new JsonSerializerSettings() { DateFormatString = "dd/MM/yyyy HH:MM:s" });
                var jsonResult_Task = Json(jsonString_Task, JsonRequestBehavior.AllowGet);
                jsonResult_Task.MaxJsonLength = int.MaxValue;

               

                return Json(new { dt_TaskList = jsonResult_Task }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("TimeSheetFormController", "Exception occured on GetTaskAndLocationByAssignmentID Controller, " + ex.Message);
            }
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAssignmentLocationByEmpID(int id)
        {
            DataTable dt_Location = new DataTable();

            try
            {
                BAL.TimeSheetFormManagement mgt = new BAL.TimeSheetFormManagement();

               
                dt_Location = mgt.GetAssignmentLocationByEmpID(id);
                string jsonString_Location = JsonConvert.SerializeObject(dt_Location, Formatting.Indented, new JsonSerializerSettings() { DateFormatString = "dd/MM/yyyy HH:MM:s" });
                var jsonResult_Location = Json(jsonString_Location, JsonRequestBehavior.AllowGet);
                jsonResult_Location.MaxJsonLength = int.MaxValue;


                return Json(new { dt_LocationList = jsonResult_Location }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("TimeSheetFormController", "Exception occured on GetTaskAndLocationByAssignmentID Controller, " + ex.Message);
            }
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetHolidays(int year, string fromDate, string toDate)
        {
            try
            {
                BAL.HoliDayManagement mgt = new BAL.HoliDayManagement();
                return Json(new { holidayDateList = mgt.GetHolidays(year,fromDate,toDate) }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("TimeSheetFormController", "Exception occured on GetTaskAndLocationByAssignmentID Controller, " + ex.Message);
            }
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }
    }
}