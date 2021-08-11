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
    public class ApprovalSetupController : BaseController
    {
        // GET: ApprovalSetup

        public ActionResult Index()
        {
            return View();
        }
        
        //public ActionResult GetApprovalSetup(int ID = 0)
        public ActionResult GetApprovalSetup(int ID = 0, string APPROVALSTAGE = "", string DOCUMENT = "")
        {
            BAL.ApprovalSetupManagement approvalSetup = new BAL.ApprovalSetupManagement();
            List<UserProfile> HCMOneUsers = new List<UserProfile>();
            try
            {
                //BAL.UserManagement mgt = new BAL.UserManagement();
                HCMOneUsers = approvalSetup.GetAllUsers(false, APPROVALSTAGE, DOCUMENT);
                
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ApprovalSetup", "Exception occured on GetApprovalSetup Controller, " + ex.Message);
            }
            return Json(new { response = HCMOneUsers , ApprovalSetup = approvalSetup.GetApprovalSetup(ID) }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetApprovalSetupHeader(int ID = 0)
        {
            BAL.ApprovalSetupManagement approvalSetup = new BAL.ApprovalSetupManagement();
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
                log.InputOutputDocLog("ApprovalSetup", "Exception occured on GetApprovalSetupHeader Controller, " + ex.Message);
            }
            return Json(new {  ApprovalSetup = approvalSetup.GetApprovalSetup(ID) }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult AddUpdateApprovalSetup(string APPROVALCODE , string APPROVALDESCRIPTION, string APPROVALSTAGE, string DOCUMENT, string DOCNUM , bool ISACTIVE, List<ApprovalSetupChildInfo> ApprovalSetupInfo, int ID)
        {
            bool suc = true;
            string msg = "Successfully Added/Updated";
            try
            {
                BAL.ApprovalSetupManagement ApprovalSetupManagement = new BAL.ApprovalSetupManagement();
                
                ApprovalSetupInfo.Select(c => { c.CREATEDBY = CurrentUser.SessionUser.ID; c.UPDATEDBY = CurrentUser.SessionUser.ID; return c; }).ToList();
                string fullName = CurrentUser.SessionUser.FULLNAME;
                suc= ApprovalSetupManagement.AddUpdateApprovalSetup(out msg, ApprovalSetupInfo,DOCNUM, APPROVALCODE, APPROVALDESCRIPTION, APPROVALSTAGE, DOCUMENT, CurrentUser.SessionUser.ID, fullName, ID, ISACTIVE);
                return Json(new
                {
                    Success = suc,
                    Message = msg
                },
                    JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ApprovalSetup", "Exception occured on AddUpdateGetApprovalSetup Controller, " + ex.Message);
                return Json(new { Success = false, Message = "Exception Occured When Add/Update ApprovalSetup" }, JsonRequestBehavior.AllowGet);
                
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
                log.InputOutputDocLog("ApprovalSetupController", "Exception occured on GetMaster_TaskByFunctionID Controller, " + ex.Message);
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
                log.InputOutputDocLog("ApprovalSetupController", "Exception occured on GetMaster_TaskByDocNum Controller, " + ex.Message);
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
                log.InputOutputDocLog("ApprovalSetup", "Exception occured on GetTravelLocation Controller, " + ex.Message);
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
                log.InputOutputDocLog("ApprovalSetup", "Exception occured on GetAssignmentCostSetup Controller, " + ex.Message);
                return Json(new { response = table }, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult GetSapFunctions()
        {
            List<StageSetupInfo> list = new List<StageSetupInfo>();
            List<string> docList = new List<string>();
            try
            {
                BAL.StageSetupManagement setup = new BAL.StageSetupManagement();
                list = setup.GetFunctionsFromStage();
                docList = setup.GetTaskMasterDocNum();
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ApprovalSetup", "Exception occured on GetSapFunctions Controller, " + ex.Message);
            }
            return Json(new { response = list, DocList = docList }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDocFunctions()
        {
            List<Documents> list = new List<Documents>();
            try
            {
                BAL.StageSetupManagement setup = new BAL.StageSetupManagement();
                list = setup.Documentss;
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ApprovalSetupController", "Exception occured on GetDocFunctions Controller, " + ex.Message);
            }
            return Json(new { response = list}, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetApprovalSetupLog(string docId)
        {
            DataSet table = new DataSet();

            try
            {
                BAL.ApprovalSetupManagement setupManagement = new BAL.ApprovalSetupManagement();
                table = setupManagement.GetApprovalSetupLog(docId);

                string jsonString = JsonConvert.SerializeObject(table, Formatting.Indented, new JsonSerializerSettings() { DateFormatString = "dd/MM/yyyy HH:MM:s" });

                var jsonResult = Json(jsonString, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;

                return Json(new { response = jsonResult }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ApprovalSetupController", "Exception occured on GetApprovalSetupLog Controller, " + ex.Message);
                return Json(new { response = table }, JsonRequestBehavior.AllowGet);
            }

        }

    }
}