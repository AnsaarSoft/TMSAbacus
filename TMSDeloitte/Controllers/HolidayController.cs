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
    public class HolidayController : BaseController
    {
        // GET: Holiday
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetDocNumList()
        {
            List<SAP_Function> list = new List<SAP_Function>();

            HCMOneManagement mgt = new HCMOneManagement();
            try
            {
                BAL.Common setupManagement = new BAL.Common();
                return Json(new { docList = setupManagement.GetDocNum("GetHolidayDocNum", "HoliDayManagement") }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("HolidayController", "Exception occured on GetAssignmentCostSetup Controller, " + ex.Message);
            }
            return Json(new { docList = new List<string>() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetHolidayByID(int id)
        {
            Holiday list = new Holiday();
            try
            {
                BAL.HoliDayManagement setupManagement = new BAL.HoliDayManagement();
                list = setupManagement.GetHolidayByID(id);
                string jsonString = JsonConvert.SerializeObject(list, Formatting.Indented, new JsonSerializerSettings() { DateFormatString = "yyyy/MM/dd" });

                var jsonResult = Json(jsonString, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;

                return Json(new { response = jsonResult }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("HolidayController", "Exception occured on GetHolidayByID Controller, " + ex.Message);
            }
            return Json(new { response = list }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetHolidayByDocNum(string docNum)
        {
            Holiday list = new Holiday();
            try
            {
                BAL.HoliDayManagement setupManagement = new BAL.HoliDayManagement();
                list = setupManagement.GetHolidayByDocNum(docNum);
                string jsonString = JsonConvert.SerializeObject(list, Formatting.Indented, new JsonSerializerSettings() { DateFormatString = "yyyy/MM/dd" });

                var jsonResult = Json(jsonString, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;

                return Json(new { response = jsonResult }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("HolidayController", "Exception occured on GetHolidayByDocNum Controller, " + ex.Message);
            }
            return Json(new { response = list }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult ValidateDateRange(int year, DateTime fromDate, DateTime toDate)
        {
            try
            {
                BAL.HoliDayManagement setupManagement = new BAL.HoliDayManagement();

                return Json(new { Success = setupManagement.ValidateDateRange(year, fromDate, toDate) }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = "Exception Occured When Add/Update Assignment Cost Setup" }, JsonRequestBehavior.AllowGet);
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("HolidayController", "Exception occured on GetAssignmentCostSetup Controller, " + ex.Message);

            }

        }

        [HttpPost]
        public ActionResult AddUpdateHoliday(Holiday list)
        {
            try
            {
                string msg = "Successfully Added/Updated";
                BAL.HoliDayManagement setupManagement = new BAL.HoliDayManagement();
                list.CREATEDBY = CurrentUser.SessionUser.ID;
                list.UPDATEDEDBY = CurrentUser.SessionUser.ID;
                return Json(new { Success = setupManagement.AddUpdateHolidaySetup(list, out msg), Message = msg }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = "Exception Occured When Add/Update Assignment Cost Setup" }, JsonRequestBehavior.AllowGet);
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("HolidayController", "Exception occured on GetAssignmentCostSetup Controller, " + ex.Message);

            }

        }

        public ActionResult GetHolidaySetupLog(string docId)
        {
            DataSet table = new DataSet();

            try
            {
                BAL.HoliDayManagement setupManagement = new BAL.HoliDayManagement();
                table = setupManagement.GetHolidayLogByID(docId);

                string jsonString = JsonConvert.SerializeObject(table, Formatting.Indented, new JsonSerializerSettings() { DateFormatString = "dd/MM/yyyy" });

                var jsonResult = Json(jsonString, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;

                return Json(new { response = jsonResult }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("HolidayController", "Exception occured on GetAssignmentCostSetup Controller, " + ex.Message);
                return Json(new { response = table }, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult GetHolidayAllDocuments()
        {
            DataTable table = new DataTable();
            try
            {
                BAL.HoliDayManagement setupManagement = new BAL.HoliDayManagement();
                table = setupManagement.GetHolidayAllDocumentsList();

                string jsonString = JsonConvert.SerializeObject(table, Formatting.Indented, new JsonSerializerSettings() { DateFormatString = "dd/MM/yyyy HH:MM:s" });

                var jsonResult = Json(jsonString, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;

                return Json(new { response = jsonResult }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("HolidayController", "Exception occured on GetAssignmentCostSetup Controller, " + ex.Message);
                return Json(new { response = table }, JsonRequestBehavior.AllowGet);
            }

        }
    }
}