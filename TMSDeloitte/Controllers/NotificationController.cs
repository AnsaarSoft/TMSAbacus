using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TMSDeloitte.Helper;
using TMSDeloitte.Models;

namespace TMSDeloitte.Controllers
{
    public class NotificationController : Controller
    {
        // GET: Notification
        public ActionResult Index()
        {
            if (System.Web.HttpContext.Current.Session["TMSUserSession"] == null)
                return RedirectToAction("Index", "Home");

            return View();
        }

        public ActionResult GetNotifications(int id)
        {
            List<Notification> list = new List<Notification>();

            try
            {
                BAL.NotificationManagement Mgt = new BAL.NotificationManagement();
                list = Mgt.GetNotificationByEmpID(id, true);

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("NotificationController", "Exception occured on GetAlertSetupByDocNum Controller, " + ex.Message);
            }
            return Json(new { NotificationList = list }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllNotifications(int id,string fromDate,string toDate)
        {
            List<Notification> list = new List<Notification>();

            try
            {
                BAL.NotificationManagement Mgt = new BAL.NotificationManagement();
                list = Mgt.GetNotificationByEmpID(id, false, fromDate, toDate);

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("NotificationController", "Exception occured on GetAlertSetupByDocNum Controller, " + ex.Message);
            }
            return Json(new { NotificationList = list }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetAllNotificationByDocID(int id,int empID)
        {
            List<Notification> list = new List<Notification>();

            try
            {
                BAL.NotificationManagement Mgt = new BAL.NotificationManagement();
                list = Mgt.GetNotificationByDocID(id, empID);
                
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("NotificationController", "Exception occured on GetAllNotificationByDocID Controller, " + ex.Message);
            }
            return Json(new { NotificationList = list }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult UpdateAnotificationAsRead(int docId)
        {
            try
            {
                string msg = "Successfully Updated";
                BAL.NotificationManagement setupManagement = new BAL.NotificationManagement();
                setupManagement.UpdateNotificationAsRead(docId);
                return Json(new { Success = true, Message = msg }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("NotificationController", "Exception occured on UpdateAnotificationAsRead Controller, " + ex.Message);
                return Json(new { Success = false, Message = "Exception Occured!" }, JsonRequestBehavior.AllowGet);

            }

        }


        [HttpPost]
        public ActionResult UpdateAllAnotificationAsRead(int empID)
        {
            try
            {
                string msg = "Successfully Updated";
                BAL.NotificationManagement setupManagement = new BAL.NotificationManagement();
                setupManagement.UpdateEmpNotificationAsRead(empID);
                return Json(new { Success = true, Message = msg }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("NotificationController", "Exception occured on UpdateAnotificationAsRead Controller, " + ex.Message);
                return Json(new { Success = false, Message = "Exception Occured!" }, JsonRequestBehavior.AllowGet);

            }

        }


        public ActionResult Detail(int? NotificationID)
        {
            if (System.Web.HttpContext.Current.Session["TMSUserSession"] == null)
                return RedirectToAction("Index", "Home");

            int ID = 0;

            try
            {
                ID = Convert.ToInt32((NotificationID));
               
            }
            catch(Exception ex) { }
            ViewBag.ID = ID;


            return View();
        }

    }
}