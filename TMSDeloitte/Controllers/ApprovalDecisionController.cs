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
    public class ApprovalDecisionController : BaseController
    {
        // GET: ApprovalDecision

        public ActionResult Index(int docID=0,string docType="")
        {
            ViewBag.docID = docID;
            ViewBag.docType = docType;

            return View();
        }
        public ActionResult GetApprovalDecision(int ID=0)
        {
            BAL.ApprovalDecisionManagement groupSetup = new BAL.ApprovalDecisionManagement();
            BAL.Common cmn = new Common();
            List<UserProfile> HCMOneUsers = new List<UserProfile>();
            try
            {
                
                BAL.UserManagement mgt = new BAL.UserManagement();
                HCMOneUsers = mgt.GetAllUsers(false);

                ID = CurrentUser.SessionUser.ID;
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ApprovalDecision", "Exception occured on GetApprovalDecision Controller, " + ex.Message);
            }
            return Json(new { response = HCMOneUsers,
                ApprovalDecision = groupSetup.GetApprovalDecision(Convert.ToInt32(ID), HCMOneUsers) }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDocumentView(int DocID = 0, int EmpID=0, string DocType="")
        {
            BAL.ApprovalDecisionManagement approvalDecision = new BAL.ApprovalDecisionManagement();
            //List<UserProfile> HCMOneUsers = new List<UserProfile>();
            try
            {
                //BAL.UserManagement mgt = new BAL.UserManagement();
                //HCMOneUsers = mgt.GetAllUsers(false);

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ApprovalDecision", "Exception occured on GetDocumentView Controller, " + ex.Message);
            }
            return Json(new { DocView = approvalDecision.GetDocView(DocID, EmpID, DocType) }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AddUpdateApprovalDecision(string DocType, int STATUS, List<ApprovalDecision> ApprovalDecisionInfo, string COMMENTS, 
        int ID)
        {
            bool suc = true;
            string msg = "Successfully Updated";
            try
            {
                BAL.ApprovalDecisionManagement ApprovalDecisionManagement = new BAL.ApprovalDecisionManagement();

                //ApprovalDecisionInfo.Select(c => { c.CREATEDBY = ID; return c; }).ToList();

                suc = ApprovalDecisionManagement.UpdateApprovalDecision(out msg, DocType, STATUS, ApprovalDecisionInfo, CurrentUser.SessionUser.ID, CurrentUser.SessionUser.FULLNAME, ID, COMMENTS);
                return Json(new { Success = suc, Message = msg }, JsonRequestBehavior.AllowGet);

                //return Json(TravelLocationManagement.AddUpdateTravelLocation(ApprovalDecision), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ApprovalDecision", "Exception occured on AddUpdateGetApprovalDecision Controller, " + ex.Message);
                return Json(new { Success = false, Message = "Exception Occured When Add/Update ApprovalDecision" }, JsonRequestBehavior.AllowGet);
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
                log.InputOutputDocLog("ApprovalDecisionController", "Exception occured on GetMaster_TaskByFunctionID Controller, " + ex.Message);
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
                log.InputOutputDocLog("ApprovalDecisionController", "Exception occured on GetMaster_TaskByDocNum Controller, " + ex.Message);
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