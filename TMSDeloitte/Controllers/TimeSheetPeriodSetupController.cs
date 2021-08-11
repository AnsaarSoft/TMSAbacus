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
    public class TimeSheetPeriodSetupController : BaseController
    {

        // GET: TimeSheetPeriodSetup
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetDocList()
        {
            BAL.Common setupManagement = new BAL.Common();
            HCMOneManagement mgt = new HCMOneManagement();
            BAL.TimeSheetPeriodManagement timeSheetperiod = new TimeSheetPeriodManagement();
            try
            {
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ResourceBillingRateController", "Exception occured on GetAllDocumentNum Controller, " + ex.Message);
            }
            return Json(new {  docList = setupManagement.GetDocNum("GetTimeSheetPeriodsDocNum", "TimeSheetPeriodSetupController"),TimeSheetYearUtilized = timeSheetperiod.GetUtilitizedYear() }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GenerateTimeSheetPeriod(GenerateTimeSheet generateTimeSheet)
        {
            List<TimeSheetPeriods> list = new List<TimeSheetPeriods>();
            bool isSuccess = false;
            try
            {
                BAL.Common cmn = new BAL.Common();
                list = cmn.GenerateTimeSheetPeriods(generateTimeSheet.fromDate, generateTimeSheet.toDate, generateTimeSheet.year);
                if(list.Count>0)
                     isSuccess = true;

                string jsonString = JsonConvert.SerializeObject(list, Formatting.Indented, new JsonSerializerSettings() { DateFormatString = "dd/MM/yyyy" });

                var jsonResult = Json(jsonString, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return Json(new { list = jsonResult, Success= isSuccess }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                 Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("TimeSheetPeriodSetupController", "Exception occured on GenerateTimeSheetPeriod Controller, " + ex.Message);
                return Json(new { list = list }, JsonRequestBehavior.AllowGet);
            }
           
        }

        [HttpPost]
        public ActionResult AddUpdateTimeSheetPeriod(GenerateTimeSheet list)
        {
            try
            {
                string msg = "Successfully Added/Updated";
                BAL.TimeSheetPeriodManagement setupManagement = new BAL.TimeSheetPeriodManagement();
                list.CREATEDBY = CurrentUser.SessionUser.ID;
                list.UPDATEDEDBY = CurrentUser.SessionUser.ID;
                return Json(new { Success = setupManagement.AddUpdateTimeSheetPeriodsSetup(list, out msg), Message = msg }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("TimeSheetPeriodSetupController", "Exception occured on AddUpdateTimeSheetPeriod Controller, " + ex.Message);
                return Json(new { Success = false, Message = "Exception Occured When Add/Update Time Sheet Period!" }, JsonRequestBehavior.AllowGet);

            }

        }
        public ActionResult GetAllDocuments()
        {
            DataTable table = new DataTable();
            try
            {
                BAL.TimeSheetPeriodManagement setupManagement = new BAL.TimeSheetPeriodManagement();
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
                log.InputOutputDocLog("TimeSheetPeriodSetupController", "Exception occured on GetAllDocuments Controller, " + ex.Message);
                return Json(new { response = table }, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult GetTimeSheetByDocNum(string docNum)
        {
            GenerateTimeSheet obj = new GenerateTimeSheet();
            try
            {
                BAL.TimeSheetPeriodManagement setupManagement = new BAL.TimeSheetPeriodManagement();
                obj = setupManagement.GetTimeSheetByDocNum(docNum);
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

    }
}