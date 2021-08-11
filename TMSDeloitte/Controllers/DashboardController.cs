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
    public class DashboardController : Controller
    {

        // GET: Dashboard
        public ActionResult Index()
        {

            if (System.Web.HttpContext.Current.Session["TMSUserSession"] == null)
                return RedirectToAction("Index", "Home");

            UserSession sessUser = (UserSession)System.Web.HttpContext.Current.Session["TMSUserSession"];
            ViewBag.DashboardIDs = "";
            string ids = string.Join(",", sessUser.pagelist.Where(x => x.HeadID == 6).ToList().Select(x => x.ID));
            ViewBag.DashboardIDs = ids;
            return View();
        }

        public ActionResult GetDashboardTotalHours()
        {
            DataTable dt_Task = new DataTable();

            try
            {
                if (System.Web.HttpContext.Current.Session["TMSUserSession"] != null)
                {
                    UserSession User = (UserSession)System.Web.HttpContext.Current.Session["TMSUserSession"];

                    BAL.DashboardManagement mgt = new BAL.DashboardManagement();
                    dt_Task = mgt.GetDashboardTotalHours(User.SessionUser.ID, "GetDashboardTotalHours");
                    string jsonString_Task = JsonConvert.SerializeObject(dt_Task, Formatting.Indented, new JsonSerializerSettings() { DateFormatString = "dd/MM/yyyy HH:MM:s" });
                    var jsonResult_Task = Json(jsonString_Task, JsonRequestBehavior.AllowGet);
                    jsonResult_Task.MaxJsonLength = int.MaxValue;

                    return Json(new { dt_List = jsonResult_Task }, JsonRequestBehavior.AllowGet);
                }

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
                if (System.Web.HttpContext.Current.Session["TMSUserSession"] != null)
                {
                    UserSession User = (UserSession)System.Web.HttpContext.Current.Session["TMSUserSession"];

                    BAL.DashboardManagement mgt = new BAL.DashboardManagement();
                    dt_Task = mgt.GetTimeSheetStatus(User.SessionUser.ID, "GetTimeSheetStatus");
                    string jsonString_Task = JsonConvert.SerializeObject(dt_Task, Formatting.Indented, new JsonSerializerSettings() { DateFormatString = "dd/MM/yyyy HH:MM:s" });
                    var jsonResult_Task = Json(jsonString_Task, JsonRequestBehavior.AllowGet);
                    jsonResult_Task.MaxJsonLength = int.MaxValue;



                    return Json(new { dt_List = jsonResult_Task }, JsonRequestBehavior.AllowGet);
                }

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
                if (System.Web.HttpContext.Current.Session["TMSUserSession"] != null)
                {
                    UserSession User = (UserSession)System.Web.HttpContext.Current.Session["TMSUserSession"];

                    BAL.HCMOneManagement mgt = new BAL.HCMOneManagement();
                    empLeaveList = mgt.GetHCMUserLeaves(User.SessionUser.ID, User.SessionUser.HCMOneID);


                    return Json(new { List = empLeaveList }, JsonRequestBehavior.AllowGet);
                }
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
                if (System.Web.HttpContext.Current.Session["TMSUserSession"] != null)
                {
                    UserSession User = (UserSession)System.Web.HttpContext.Current.Session["TMSUserSession"];

                    BAL.DashboardManagement mgt = new BAL.DashboardManagement();
                    dt_Task = mgt.GetDashboardTotalHours_ProfessionalStaff(User.SessionUser.ID, "GetDashboardTotalHours_ProfessionalStaff");
                    string jsonString_Task = JsonConvert.SerializeObject(dt_Task, Formatting.Indented, new JsonSerializerSettings() { DateFormatString = "dd/MM/yyyy HH:MM:s" });
                    var jsonResult_Task = Json(jsonString_Task, JsonRequestBehavior.AllowGet);
                    jsonResult_Task.MaxJsonLength = int.MaxValue;

                    return Json(new { dt_List = jsonResult_Task }, JsonRequestBehavior.AllowGet);
                }


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

        public ActionResult GetDashboardStaffPendingTimeStatusKPI()
        {
            try
            {
                DashboardManagement dMgt = new DashboardManagement();
                List<StaffPendingTimeStatus> list = dMgt.GetDashboardStaffPendingTimeStatus("GetDashboardStaffPendingTimeStatus");
                int TotalSubmittedSum = list.Sum(item => item.TotalSubmitted);
                int TotalApprovedSum = list.Sum(item => item.TotalApproved);

                int Total = TotalSubmittedSum - TotalApprovedSum;
                if (Total < 0)
                    Total = 0;

                return Json(new { List = list, Total = Total, TotalSubmittedSum = TotalSubmittedSum, TotalApprovedSum = TotalApprovedSum }, JsonRequestBehavior.AllowGet);
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