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
    public class TravelLocationController : BaseController
    {

        // GET: TravelLocation
        public ActionResult Index()
        {
            return View();
        }
        
        public ActionResult GetTravelLocation(int ID = 0)
        {
            List<TravelLocationInfo> TravelLocationList = new List<TravelLocationInfo>();
            try
            {
                BAL.TravelLocationManagement TravelLocationManagement = new BAL.TravelLocationManagement();
                TravelLocationList = TravelLocationManagement.GetTravel_LocationSetup(ID);

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("Travel Rates", "Exception occured on GetTravelLocation Controller, " + ex.Message);
            }
            return Json(new { response = TravelLocationList }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddUpdateGetTravelLocation(List<TravelLocationInfo> TravelLocationList)
        {
            bool suc = false;
            string msg = "";
            msg = "Successfully Added/Updated";
            try
            {
                BAL.TravelLocationManagement TravelLocationManagement = new BAL.TravelLocationManagement();
                TravelLocationList.Select(c => { c.CREATEDBY = CurrentUser.SessionUser.ID; c.UPDATEDBY = CurrentUser.SessionUser.ID; return c; }).ToList();
                //TravelLocationList.Select(c => { c.CREATEDBY = 1; c.UPDATEDBY = 1; return c; }).ToList();
                //suc = TravelLocationManagement.AddUpdateTravelLocation(TravelLocationList);
                bool IsDuplicationOccured = TravelLocationManagement.CheckDuplicateRecord(TravelLocationList);

                if (IsDuplicationOccured)
                {
                    suc = false;
                    msg = "Location Can not be duplicate";
                }
                else
                {
                    suc = TravelLocationManagement.AddUpdateTravelLocation(TravelLocationList,out msg);
                   
                }
                return Json(new
                {
                    //success = suc,
                    Success = suc,
                    Message = msg
                },
                    JsonRequestBehavior.AllowGet);

                //return Json(TravelLocationManagement.AddUpdateTravelLocation(TravelLocationList), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = "Exception Occured When Add/Update Travel Rates" }, JsonRequestBehavior.AllowGet);
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("Travel Rates", "Exception occured on AddUpdateGetTravelLocation Controller, " + ex.Message);

            }

        }

        public ActionResult GetSapFunctions()
        {
            List<SAP_Function> list = new List<SAP_Function>();
            List<SAP_Function> Clients = new List<SAP_Function>();
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
                Clients = setupManagement.GetClientFromSAPB1();
                BAL.TravelLocationManagement setup = new BAL.TravelLocationManagement();

                docList = setup.GetTravelLocationDocNum();
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("Travel RatesController", "Exception occured on GetSapFunctions Controller, " + ex.Message);
            }
            return Json(new { response = list, DocList = docList , Clients  = Clients }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMaster_TaskByFunctionID(int id)
        {
            List<TravelLocationInfo> list = new List<TravelLocationInfo>();
            try
            {
                BAL.TravelLocationManagement taskMaster = new BAL.TravelLocationManagement();
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
            List<TravelLocationInfo> list = new List<TravelLocationInfo>();
            try
            {
                BAL.TravelLocationManagement setupManagement = new BAL.TravelLocationManagement();
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


        

        public ActionResult GetTravelLocationLog(string docNum)
        {

            DataTable table = new DataTable();

            try
            {
                BAL.TravelLocationManagement setupManagement = new BAL.TravelLocationManagement();
                table = setupManagement.GetTravelLocationLog();

                string jsonString = JsonConvert.SerializeObject(table, Formatting.Indented, new JsonSerializerSettings() { DateFormatString = "dd/MM/yyyy" });

                var jsonResult = Json(jsonString, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;

                return Json(new { response = jsonResult }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("SetupManagement", "Exception occured on GetTravelLocation Controller, " + ex.Message);
                return Json(new { response = table }, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult GetTravel_LocationAllDocuments()
        {
            DataTable table = new DataTable();
            try
            {
                BAL.TravelLocationManagement setupManagement = new BAL.TravelLocationManagement();
                table = setupManagement.GetTravel_LocationAllDocumentsList();

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