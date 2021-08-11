using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Mvc;
using TMSDeloitte.BAL;
using TMSDeloitte.Helper;
using TMSDeloitte.Models;

namespace TMSDeloitte.Controllers
{
    public class TimeSheetViewController : BaseController
    {
        // GET: TimeSheetView
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetUsers()
        {
            List<UserProfile> HCMOneUsers = new List<UserProfile>();
            BAL.Common cmn = new Common();
            try
            {
                BAL.UserManagement mgt = new BAL.UserManagement();
                HCMOneUsers = mgt.GetAllTimeSheetFilteredUsersByDepartAndBranch(CurrentUser.SessionUser.ID, CurrentUser.TimeSheetViewList, CurrentUser.BranchList, CurrentUser.isTimeSheetViewDataAccess, CurrentUser.isBranchDataAccess);

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("TimeSheetViewController", "Exception occured on GetHCMUsers Controller, " + ex.Message);
            }
            return Json(new {
                response = HCMOneUsers,
                statusList = cmn.GetTimeSheetFormStatusList(),
                IsSuper = CurrentUser.SessionUser.ISSUPER,
                EmpID=CurrentUser.SessionUser.ID
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetUserInfoByEmpCode(string empCode)
        {
            UserProfile user = new UserProfile();
            bool isSuccess = false;
            try
            {
                BAL.UserManagement userMgt = new BAL.UserManagement();

                if(CurrentUser.SessionUser.ISSUPER)
                    isSuccess = userMgt.GetUserByEmpCode(empCode, out user);

                if (!isSuccess)
                    if (CurrentUser.SessionUser.EMPLOYEECODE.ToLower().Replace(" ","")==empCode.ToLower().Replace(" ", ""))
                    isSuccess = userMgt.GetUserByEmpCode(empCode, out user);


            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("TimeSheetViewController", "Exception occured on GetUserInfoByEmpCode Controller, " + ex.Message);
            }
            return Json(new { Success = isSuccess, UserInfo = user }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetTimeSheetView(int isShowAll, int empId, string fromDate, string toDate)
        {
            DataTable dt= new DataTable();

            try
            {
                BAL.TimeSheetViewManagement mgt = new BAL.TimeSheetViewManagement();
                dt= mgt.GetTimeSheetView(CurrentUser.SessionUser.ISSUPER,isShowAll,  empId, fromDate, toDate);

                string jsonString = JsonConvert.SerializeObject(dt, Formatting.Indented, new JsonSerializerSettings() { DateFormatString = "dd/MM/yyyy" });
                var jsonResult = Json(jsonString, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;

                return Json(new { datatable = jsonResult }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("AlertSetupController", "Exception occured on GetLogByDocID Controller, " + ex.Message);
            }
            return Json(new { datatable = dt }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UpdateTimeSheetForm(TimeSheetForm obj)
        {
            try
            {
                string msg = "Successfully Added/Updated";
                BAL.TimeSheetFormManagement setupManagement = new BAL.TimeSheetFormManagement();
                obj.UpdatedBy = CurrentUser.SessionUser.ID;
                return Json(new { Success = setupManagement.UpdateTimeSheetFormStatus(obj, out msg), Message = msg }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("TimeSheetFormController", "Exception occured on AddUpdateTimeSheetPeriod Controller, " + ex.Message);
                return Json(new { Success = false, Message = "Exception Occured When Add/Update Time Sheet Period!" }, JsonRequestBehavior.AllowGet);

            }

        }
    }
}