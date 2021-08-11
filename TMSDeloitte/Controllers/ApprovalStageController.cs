using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TMSDeloitte.Helper;
using TMSDeloitte.Models;
using TMSDeloitte.BAL;
using Newtonsoft.Json;

namespace TMSDeloitte.Controllers
{
    public class ApprovalStageController : BaseController
    {
        // GET: StageSetup

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetStageSetup(int ID = 0)
        {
            BAL.StageSetupManagement groupSetup = new BAL.StageSetupManagement();
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
                log.InputOutputDocLog("StageSetup", "Exception occured on GetStageSetup Controller, " + ex.Message);
            }
            return Json(new { response = HCMOneUsers, StageSetup = groupSetup.GetStageSetup(ID) }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetStageSetupHeader(int ID = 0)
        {
            BAL.StageSetupManagement groupSetup = new BAL.StageSetupManagement();
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
                log.InputOutputDocLog("StageSetup", "Exception occured on GetStageSetupHeader Controller, " + ex.Message);
            }
            return Json(new { StageSetup = groupSetup.GetStageSetup(ID) }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult AddUpdateStageSetup(string STAGECODE, string STAGEDESCRIPTION, int APPROVALREQUIRED, int REJECTIONREQUIRED, string DOCNUM, bool INACTIVE,
        List<StageSetupChildInfo> StageSetupInfo, int ID)
        {
            bool suc = true;
            string msg = "Successfully Added/Updated";
            try
            {
                BAL.StageSetupManagement StageSetupManagement = new BAL.StageSetupManagement();

                StageSetupInfo.Select(c => { c.CREATEDBY = CurrentUser.SessionUser.ID; c.UPDATEDBY = CurrentUser.SessionUser.ID; return c; }).ToList();

                suc = StageSetupManagement.AddUpdateStageSetup(out msg, StageSetupInfo, DOCNUM, STAGECODE, STAGEDESCRIPTION, APPROVALREQUIRED, REJECTIONREQUIRED, CurrentUser.SessionUser.ID, ID, INACTIVE);
                
                return Json(new { Success = suc, Message = msg }, JsonRequestBehavior.AllowGet);

                //return Json(TravelLocationManagement.AddUpdateTravelLocation(StageSetupInfo), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = "Exception Occured When Add/Update StageSetup" }, JsonRequestBehavior.AllowGet);
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("StageSetup", "Exception occured on AddUpdateGetStageSetup Controller, " + ex.Message);

            }

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
                log.InputOutputDocLog("StageSetupController", "Exception occured on GetMaster_TaskByFunctionID Controller, " + ex.Message);
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
                log.InputOutputDocLog("StageSetupController", "Exception occured on GetMaster_TaskByDocNum Controller, " + ex.Message);
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

        public ActionResult GetStageSetupLogByDOCNUM(string docId)
        {
            DataSet table = new DataSet();
            try
            {
                BAL.StageSetupManagement setupManagement = new BAL.StageSetupManagement();
                table = setupManagement.GetStageSetupLogByDOCNUM(docId);

                string jsonString = JsonConvert.SerializeObject(table, Formatting.Indented, new JsonSerializerSettings() { DateFormatString = "dd/MM/yyyy HH:MM:ss" });

                var jsonResult = Json(jsonString, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;

                return Json(new { response = jsonResult }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("UserController", "Exception occured on GetAssignmentCostSetup Controller, " + ex.Message);
                return Json(new { response = table }, JsonRequestBehavior.AllowGet);
            }

        }

    }
}