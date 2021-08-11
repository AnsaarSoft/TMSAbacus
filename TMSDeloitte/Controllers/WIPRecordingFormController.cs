using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TMSDeloitte.Helper;
using TMSDeloitte.Models;

namespace TMSDeloitte.Controllers
{
    public class WIPRecordingFormController : BaseController
    {
        // GET: WipRecordingForm
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetAccounts()
        {
            List<SAP_Function> AccountList = new List<SAP_Function>();
            try
            {
                BAL.Common setupManagement = new BAL.Common();
                AccountList = setupManagement.GetAccountsFromSAPB1();

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("Common", "Exception occured on GetAccounts Controller, " + ex.Message);
            }
            return Json(new { response = AccountList }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetWIPRecordingForm(int ID = 0)
        {
            BAL.WIPRecordingFormManagement WIP = new BAL.WIPRecordingFormManagement();
            List<WIPRecordingForm> WIPmodel = new List<WIPRecordingForm>();

            try
            {
                WIPmodel = WIP.GetWIPRecordingForm(ID);
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("WIPRecordingForm", "Exception occured on GetWIPRecordingForm Controller, " + ex.Message);
            }
            //return Json(new { response = WIPmodel, Doc = WIP.GetDocID("") }, JsonRequestBehavior.AllowGet);
            return Json(new { response = WIPmodel}, JsonRequestBehavior.AllowGet);

            //BAL.AssignmentFormManagement assign = new BAL.AssignmentFormManagement();
            //List<AssignmentForm> taskList = new List<AssignmentForm>();
            //try
            //{
            // }
            //catch (Exception ex)
            //{
            //    Log log = new Log();
            //    log.LogFile(ex.Message);
            //    log.InputOutputDocLog("AssignmentFormController", "Exception occured on GetAssignmentForm Controller, " + ex.Message);
            //}
            ////return Json(new { response = HCMOneUsers, StageSetup = groupSetup.GetStageSetup(ID) }, JsonRequestBehavior.AllowGet);
            //return Json(new { response = "", Doc = assign.GetDocID("") }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAssignments(string AsOnDate, int BranchID)
        {
            BAL.WIPRecordingFormManagement WIP = new BAL.WIPRecordingFormManagement();
            List<WIPRecordingFormChild> wipRecordingFormChild = new List<WIPRecordingFormChild>();
            bool Success = true;
            try
            {
                wipRecordingFormChild = WIP.GetAssignments(AsOnDate, BranchID);
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("WIPRecordingForm", "Exception occured on GetWIPRecordingForm Controller, " + ex.Message);
            }
            return Json(new { response = wipRecordingFormChild, Success = Success }, JsonRequestBehavior.AllowGet);

            //BAL.AssignmentFormManagement assign = new BAL.AssignmentFormManagement();
            //List<AssignmentForm> taskList = new List<AssignmentForm>();
            //try
            //{
            // }
            //catch (Exception ex)
            //{
            //    Log log = new Log();
            //    log.LogFile(ex.Message);
            //    log.InputOutputDocLog("AssignmentFormController", "Exception occured on GetAssignmentForm Controller, " + ex.Message);
            //}
            ////return Json(new { response = HCMOneUsers, StageSetup = groupSetup.GetStageSetup(ID) }, JsonRequestBehavior.AllowGet);
            //return Json(new { response = "", Doc = assign.GetDocID("") }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult PostToSBO(string DOCNUM, WIPRecordingForm WipRecordingForm, List<WIPRecordingFormChild> WIPRecordingFormChild, int ID)
        {
            bool suc = true;
            string msg = "Successfully Added/Updated";
            try
            {
                WipRecordingForm.DocNum = DOCNUM;
                BAL.WIPRecordingFormManagement WIPRecordingFormManagement = new BAL.WIPRecordingFormManagement();
                WipRecordingForm.CreatedBy = CurrentUser.SessionUser.ID;
                WipRecordingForm.UpdatedBy = CurrentUser.SessionUser.ID;

                //DateTime ReversalDate = DateTime.ParseExact(WipRecordingForm.ReversalDate, "dd/MM/yyyy", null); 
                //DateTime DocDate = DateTime.ParseExact(WipRecordingForm.DocDate, "dd/MM/yyyy", null);
                //WipRecordingForm.ReversalDate = Convert.ToString(ReversalDate);
                //WipRecordingForm.DocDate = Convert.ToString(DocDate);

                WIPRecordingFormChild.Select(c => { c.CreatedBy = CurrentUser.SessionUser.ID; c.UpdatedBy = CurrentUser.SessionUser.ID; return c; }).ToList();
               
                suc = WIPRecordingFormManagement.POSTToSBO(out msg, DOCNUM, WipRecordingForm, WIPRecordingFormChild, ID, CurrentUser.SessionUser.ID);

                return Json(new { Success = suc, Message = msg }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("AssignmentForm", "Exception occured on AddUpdateAssignmentForm Controller, " + ex.Message);
                return Json(new { Success = false, Message = "Exception Occured When Add/Update AddUpdateAssignmentForm" }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}