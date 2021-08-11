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
    public class ResourceBillingRateController : BaseController
    {

        // GET: ResourceBillingRate
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetSapFunctions()
        {
            List<SAP_Function> list = new List<SAP_Function>();
            BAL.Common setupManagement = new BAL.Common();
            HCMOneManagement mgt = new HCMOneManagement();
            try
            {

                //list = setupManagement.GetSAPFunctionList();
                list = setupManagement.GetFunctionsFromSAPB1();
                BAL.ResourceBillingRatesSetupManagement setup = new BAL.ResourceBillingRatesSetupManagement();
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ResourceBillingRateController", "Exception occured on GetAssignmentCostSetup Controller, " + ex.Message);
            }
            return Json(new { response = list ,docList= setupManagement.GetDocNum("GetResourceBillingRatesDocNum", "ResourceBillingRatesSetupManagement") ,DesignationList= mgt.GetAllHCMDesignation()}, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ResourceBillingRate()
        {
            return View();
        }
        public ActionResult GetResourceBillingRateByID(int id)
        {
            ResourceBillingRates list = new ResourceBillingRates();
            try
            {
                BAL.ResourceBillingRatesSetupManagement setupManagement = new BAL.ResourceBillingRatesSetupManagement();
                list = setupManagement.GetResourceBillingRatesByID(id);
                string jsonString = JsonConvert.SerializeObject(list, Formatting.Indented, new JsonSerializerSettings() { DateFormatString = "yyyy/MM/dd" });

                var jsonResult = Json(jsonString, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;

                return Json(new { response = jsonResult }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ResourceBillingRateController", "Exception occured on GetResourceBillingRateByID Controller, " + ex.Message);
            }
            return Json(new { response = list }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetResourceBillingRateByDocNum(string docNum)
        {
            ResourceBillingRates list = new ResourceBillingRates();
            try
            {
                BAL.ResourceBillingRatesSetupManagement setupManagement = new BAL.ResourceBillingRatesSetupManagement();
                list = setupManagement.GetResourceBillingRatesByDocNum(docNum);
                string jsonString = JsonConvert.SerializeObject(list, Formatting.Indented, new JsonSerializerSettings() { DateFormatString = "yyyy/MM/dd" });

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
            return Json(new { response = list }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult ValidateDateRange(string functionID, DateTime fromDate, DateTime toDate)
        {
            try
            {
                BAL.ResourceBillingRatesSetupManagement setupManagement = new BAL.ResourceBillingRatesSetupManagement();
              
                return Json(new { Success = setupManagement.ValidateDateRange(functionID,fromDate,toDate) }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = "Exception Occured When Add/Update Assignment Cost Setup" }, JsonRequestBehavior.AllowGet);
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ResourceBillingRateController", "Exception occured on GetAssignmentCostSetup Controller, " + ex.Message);

            }

        }

        [HttpPost]
        public ActionResult AddUpdateResourceBillingRate(ResourceBillingRates list)
        {
            try
            {
                string msg= "Successfully Added/Updated";
                BAL.ResourceBillingRatesSetupManagement setupManagement = new BAL.ResourceBillingRatesSetupManagement();
                list.CREATEDBY = CurrentUser.SessionUser.ID;
                list.UPDATEDEDBY = CurrentUser.SessionUser.ID;
                return Json(new { Success = setupManagement.AddUpdateResourceBillingRatesSetup(list,out msg), Message = msg }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = "Exception Occured When Add/Update Assignment Cost Setup" }, JsonRequestBehavior.AllowGet);
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ResourceBillingRateController", "Exception occured on GetAssignmentCostSetup Controller, " + ex.Message);

            }

        }

        public ActionResult GetResourceBillingRateSetupLog(string docId)
        {
            DataSet table = new DataSet();
           
            try
            {
                BAL.ResourceBillingRatesSetupManagement setupManagement = new BAL.ResourceBillingRatesSetupManagement();
                table = setupManagement.GetResourceBillingRatesSetupLog(docId);

                string jsonString = JsonConvert.SerializeObject(table, Formatting.Indented, new JsonSerializerSettings() { DateFormatString = "dd/MM/yyyy HH:MM:s" });

                var jsonResult = Json(jsonString, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;

                return Json(new { response = jsonResult }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ResourceBillingRateController", "Exception occured on GetAssignmentCostSetup Controller, " + ex.Message);
                return Json(new { response = table }, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult GetResourceBillingRateAllDocuments()
        {
            DataTable table = new DataTable();
            try
            {
                BAL.ResourceBillingRatesSetupManagement setupManagement = new BAL.ResourceBillingRatesSetupManagement();
                table = setupManagement.GetResourceBillingRatesAllDocumentsList();

                string jsonString = JsonConvert.SerializeObject(table, Formatting.Indented, new JsonSerializerSettings() { DateFormatString = "dd/MM/yyyy HH:MM:s" });

                var jsonResult = Json(jsonString, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;

                return Json(new { response = jsonResult }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ResourceBillingRateController", "Exception occured on GetAssignmentCostSetup Controller, " + ex.Message);
                return Json(new { response = table }, JsonRequestBehavior.AllowGet);
            }

        }
    }
}