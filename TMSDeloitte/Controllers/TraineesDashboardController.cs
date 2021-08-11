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
    public class TraineesDashboardController : BaseController
    {
        // GET: TraineesDashboard
        public ActionResult Index()
        {
            ViewBag.DashboardIDs = "";
            string ids = string.Join(",", CurrentUser.pagelist.Where(x=>x.HeadID==6).ToList().Select(x => x.ID));
            ViewBag.DashboardIDs = ids;
            return View();
        }

        public ActionResult GetDashboardTotalHours()
        {
            DataTable dt_Task = new DataTable();

            try
            {
                BAL.DashboardManagement mgt = new BAL.DashboardManagement();
                dt_Task = mgt.GetDashboardTotalHours(9, "GetDashboardTotalHours");
                string jsonString_Task = JsonConvert.SerializeObject(dt_Task, Formatting.Indented, new JsonSerializerSettings() { DateFormatString = "dd/MM/yyyy HH:MM:s" });
                var jsonResult_Task = Json(jsonString_Task, JsonRequestBehavior.AllowGet);
                jsonResult_Task.MaxJsonLength = int.MaxValue;



                return Json(new { dt_List = jsonResult_Task }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("GetDashboardTotalHours", "Exception occured on GetDashboardTotalHours Controller, " + ex.Message);
            }
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTimeSheetStatus()
        {
            DataTable dt_Task = new DataTable();

            try
            {
                BAL.DashboardManagement mgt = new BAL.DashboardManagement();
                dt_Task = mgt.GetTimeSheetStatus(1, "GetTimeSheetStatus");
                string jsonString_Task = JsonConvert.SerializeObject(dt_Task, Formatting.Indented, new JsonSerializerSettings() { DateFormatString = "dd/MM/yyyy HH:MM:s" });
                var jsonResult_Task = Json(jsonString_Task, JsonRequestBehavior.AllowGet);
                jsonResult_Task.MaxJsonLength = int.MaxValue;



                return Json(new { dt_List = jsonResult_Task }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("GetTimeSheetStatus", "Exception occured on GetTimeSheetStatus Controller, " + ex.Message);
            }
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult GetLeavStatus()
        {
            DataTable dt_Task = new DataTable();
            List<HCM_EmployeeLeaves> empLeaveList = new List<HCM_EmployeeLeaves>();
            try
            {
                //BAL.HCMOneManagement mgt = new BAL.HCMOneManagement();
                //empLeaveList = mgt.GetHCMUserLeaves(1);

                HCM_EmployeeLeaves leave = new HCM_EmployeeLeaves();
                leave.ID = 1;
                leave.LeaveType = "Leave Type1";
                leave.TotalAllowed = 30;
                leave.UseD = 1;
                leave.CarryForward = 0;
                leave.Balance = 29;
                empLeaveList.Add(leave);

                leave = new HCM_EmployeeLeaves();
                leave.ID = 1;
                leave.LeaveType = "Leave Type2";
                leave.TotalAllowed = 20;
                leave.UseD = 1;
                leave.CarryForward = 0;
                leave.Balance = 19;
                empLeaveList.Add(leave);

                leave = new HCM_EmployeeLeaves();
                leave.ID = 1;
                leave.LeaveType = "Leave Type3";
                leave.TotalAllowed = 10;
                leave.UseD = 1;
                leave.CarryForward = 0;
                leave.Balance = 9;
                empLeaveList.Add(leave);

                return Json(new { List = empLeaveList }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("GetLeavStatus", "Exception occured on GetLeavStatus Controller, " + ex.Message);
            }
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDashboardTotalHours_ProfessionalStaff()
        {
            DataTable dt_Task = new DataTable();

            try
            {
                BAL.DashboardManagement mgt = new BAL.DashboardManagement();
                dt_Task = mgt.GetDashboardTotalHours_ProfessionalStaff(1, "GetDashboardTotalHours_ProfessionalStaff");
                string jsonString_Task = JsonConvert.SerializeObject(dt_Task, Formatting.Indented, new JsonSerializerSettings() { DateFormatString = "dd/MM/yyyy HH:MM:s" });
                var jsonResult_Task = Json(jsonString_Task, JsonRequestBehavior.AllowGet);
                jsonResult_Task.MaxJsonLength = int.MaxValue;



                return Json(new { dt_List = jsonResult_Task }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("GetDashboardTotalHours_ProfessionalStaff", "Exception occured on GetDashboardTotalHours Controller, " + ex.Message);
            }
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDashboardStaffPendingTimeStatus()
        {
           

            try
            {
                DashboardManagement dMgt = new DashboardManagement();
                return Json(new { List = dMgt.GetDashboardStaffPendingTimeStatus("GetDashboardStaffPendingTimeStatus") }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("GetDashboardTotalHours_ProfessionalStaff", "Exception occured on GetDashboardTotalHours Controller, " + ex.Message);
            }
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }
    }
}