using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TMSDeloitte.Helper;
using TMSDeloitte.Models;
using TMSDeloitte.BAL;

namespace TMSDeloitte.Controllers
{
    public class TravelRatesController : BaseController
    {

        
        #region "Travel Rates"
        public ActionResult TravelRates()
        {
            return View();
        }


        public ActionResult GetTravelRates(int ID = 0)
        {
            List<TravelRates> TravelRatesList = new List<TravelRates>();
            try
            {
                BAL.TravelRatesManagement TravelRatesManagement = new BAL.TravelRatesManagement();
                TravelRatesList = TravelRatesManagement.GetTravel_RatesSetup(ID);

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("Travel Rates", "Exception occured on GetTravelRates Controller, " + ex.Message);
            }
            return Json(new { response = TravelRatesList }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddUpdateGetTravelRates(List<TravelRates> TravelRatesList)
        {
            bool suc = false;
            string msg = "Successfully Added/Updated";
            try
            {
                BAL.TravelRatesManagement TravelRatesManagement = new BAL.TravelRatesManagement();
                TravelRatesList.Select(c => { c.CREATEDBY = CurrentUser.SessionUser.ID; c.UPDATEDBY = CurrentUser.SessionUser.ID; return c; }).ToList();
                //suc = TravelRatesManagement.AddUpdateTravelRates(TravelRatesList);

                return Json(new
                {
                    //success = suc,
                    Success = TravelRatesManagement.AddUpdateTravelRates(TravelRatesList,out msg),
                    Message = msg
                },
                    JsonRequestBehavior.AllowGet);

                //return Json(TravelRatesManagement.AddUpdateTravelRates(TravelRatesList), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = "Exception Occured When Add/Update Travel Rates" }, JsonRequestBehavior.AllowGet);
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("Travel Rates", "Exception occured on AddUpdateGetTravelRates Controller, " + ex.Message);

            }

        }

        public ActionResult GetSapFunctions()
        {
            List<SAP_Function> list = new List<SAP_Function>();
            List<string> docList = new List<string>();
            try
            {
                BAL.Common setupManagement = new BAL.Common();
                List<int> brnachList = new List<int>();
                if (CurrentUser.BranchList.Count == 0)
                {
                    brnachList.Add(Convert.ToInt32(CurrentUser.SessionUser.BRANCHID));
                }
                else
                    brnachList = CurrentUser.BranchList;
                list = setupManagement.GetBranchesFromSAPB1(brnachList);
                BAL.TravelRatesManagement setup = new BAL.TravelRatesManagement();
                docList = setup.GetTaskMasterDocNum();
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("Travel RatesController", "Exception occured on GetSapFunctions Controller, " + ex.Message);
            }
            return Json(new { response = list, DocList = docList }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMaster_TaskByFunctionID(int id)
        {
            List<TravelRates> list = new List<TravelRates>();
            try
            {
                BAL.TravelRatesManagement taskMaster = new BAL.TravelRatesManagement();
                list = taskMaster.GetTaskMasterByFunctionID(id);

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("Travel RatesController", "Exception occured on GetMaster_TaskByFunctionID Controller, " + ex.Message);
            }
            return Json(new { response = list }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMaster_TaskByDocNum(string docNum)
        {
            List<TravelRates> list = new List<TravelRates>();
            try
            {
                BAL.TravelRatesManagement setupManagement = new BAL.TravelRatesManagement();
                list = setupManagement.GetTask_MasterByDocNum(docNum);

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("Travel RatesController", "Exception occured on GetMaster_TaskByDocNum Controller, " + ex.Message);
            }
            return Json(new { response = list }, JsonRequestBehavior.AllowGet);
        }



        #endregion


        public ActionResult GetTravelRatesLog(string docNum)
        {

            DataTable table = new DataTable();

            try
            {
                BAL.TravelRatesManagement setupManagement = new BAL.TravelRatesManagement();
                table = setupManagement.GetTravelRatesLog();

                string jsonString = JsonConvert.SerializeObject(table, Formatting.Indented, new JsonSerializerSettings() { DateFormatString = "dd/MM/yyyy" });

                var jsonResult = Json(jsonString, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;

                return Json(new { response = jsonResult }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("SetupManagement", "Exception occured on GetTravelRates Controller, " + ex.Message);
                return Json(new { response = table }, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult GetTravel_RatesAllDocuments()
        {
            DataTable table = new DataTable();
            try
            {
                BAL.TravelRatesManagement setupManagement = new BAL.TravelRatesManagement();
                table = setupManagement.GetTravel_RatesAllDocumentsList();

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