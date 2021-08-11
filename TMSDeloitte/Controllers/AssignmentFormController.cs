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
    public class AssignmentFormController : BaseController
    {
        // GET: ApprovalSetup

        public ActionResult Index(string docNum = "", string empID = "")
        {
            ViewBag.DocNum = docNum;
            ViewBag.EmpID = empID;

            if (docNum == "" && empID == "")
            {
                ViewBag.isView = false;
                //GetAssignmentFormInit();
            }
            else
            {
                Encrypt_Decrypt security = new Encrypt_Decrypt();
                ViewBag.DocNum = security.DecryptURLString(docNum);
                ViewBag.EmpID = security.DecryptURLString(empID);
                ViewBag.isView = true;
            }

            
            return View();
        }

        [HttpGet]
        public ActionResult GetAssignmentCreationCheck()
        {
            bool suc = true;
            string msg = "Authorize";
            
            List<ApprovalSetupChildInfo> approvalSetupCheck = new List<ApprovalSetupChildInfo>();
            try
            {
                BAL.AssignmentFormManagement Assign = new BAL.AssignmentFormManagement();
                suc = Assign.GetAssignmentCreationCheck(out msg, CurrentUser.SessionUser.ID);

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("AssignmentFormController", "Exception occured on GetTripRatePolicy Controller, " + ex.Message);
            }
            return Json(new { Success = suc, Message = msg }, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetAssignmentFormInit(int ID = 0)
        {
            BAL.AssignmentFormManagement assign = new BAL.AssignmentFormManagement();
            List<UserProfile> HCMOneUsers = new List<UserProfile>();
            try
            {
                BAL.UserManagement mgt = new BAL.UserManagement();
                //HCMOneUsers = mgt.GetAllUsers(false);
                HCMOneUsers = mgt.GetAllFilteredUsersByDepartAndBranch(CurrentUser.SessionUser.ID, CurrentUser.DepartmentList, CurrentUser.BranchList, CurrentUser.isDepartmentDataAccess, CurrentUser.isBranchDataAccess);

                return Json(new { response = assign.GetAssignmentFormInit(ID), Doc = assign.GetDocID("") }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("AssingmentForm", "Exception occured on GetAssignmentForm Controller, " + ex.Message);

                return Json(new { response = assign.GetAssignmentForm(ID), Doc = assign.GetDocID("") }, JsonRequestBehavior.AllowGet);

            }
            //return Json(new { response = assign.GetAssignmentForm(ID), Doc = assign.GetDocID("") }, JsonRequestBehavior.AllowGet);
            //return Json(new { response = assign.GetAssignmentForm(ID) }, JsonRequestBehavior.AllowGet);

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


        public JsonResult GetAssignmentForm(int ID)
        {
            BAL.AssignmentFormManagement assign = new BAL.AssignmentFormManagement();
            List<UserProfile> HCMOneUsers = new List<UserProfile>();
            try
            {
                BAL.UserManagement mgt = new BAL.UserManagement();
                //HCMOneUsers = mgt.GetAllUsers(false);
                HCMOneUsers = mgt.GetAllFilteredUsersByDepartAndBranch(CurrentUser.SessionUser.ID, CurrentUser.DepartmentList, CurrentUser.BranchList, CurrentUser.isDepartmentDataAccess, CurrentUser.isBranchDataAccess);

                //return Json(new { response = assign.GetAssignmentForm(ID), Doc = assign.GetDocID("") }, JsonRequestBehavior.AllowGet);
                
                return Json(new { response = assign.GetAssignmentForm(ID) }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("AssingmentForm", "Exception occured on GetAssignmentForm Controller, " + ex.Message);

                return Json(new { response = assign.GetAssignmentForm(ID), Doc = assign.GetDocID("") }, JsonRequestBehavior.AllowGet);

            }
            //return Json(new { response = assign.GetAssignmentForm(ID), Doc = assign.GetDocID("") }, JsonRequestBehavior.AllowGet);
            //return Json(new { response = assign.GetAssignmentForm(ID) }, JsonRequestBehavior.AllowGet);

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




        public ActionResult GetAssignmentFormResourceOnLoad(int ID)
        {
            //BAL.Common cmn = new BAL.Common();
            //BAL.StageSetupManagement groupSetup = new BAL.StageSetupManagement();
            List<UserProfile> HCMOneUsers = new List<UserProfile>();
            List<TaskMasterSetupInfo> taskList = new List<TaskMasterSetupInfo>();
            try
            {
                BAL.UserManagement mgt = new BAL.UserManagement();
                //HCMOneUsers = mgt.GetAllUsers(true);
                HCMOneUsers = mgt.GetAllFilteredUsersByDepartAndBranch(CurrentUser.SessionUser.ID, CurrentUser.DepartmentList, CurrentUser.BranchList, CurrentUser.isDepartmentDataAccess, CurrentUser.isBranchDataAccess);

                BAL.AssignmentFormManagement setup = new BAL.AssignmentFormManagement();
                taskList = setup.GetTaskMasterSetup(ID);
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("AssignmentFormController", "Exception occured on GetAssignmentForm Controller, " + ex.Message);
            }
            //return Json(new { response = HCMOneUsers, StageSetup = groupSetup.GetStageSetup(ID) }, JsonRequestBehavior.AllowGet);
            return Json(new { response = HCMOneUsers, TaskList = taskList }, JsonRequestBehavior.AllowGet);
        }




        public ActionResult GetAssignmentFormResource(int ID = 0)
        {
            //BAL.Common cmn = new BAL.Common();
            //BAL.StageSetupManagement groupSetup = new BAL.StageSetupManagement();
            List<UserProfile> HCMOneUsers = new List<UserProfile>();
            List<TaskMasterSetupInfo> taskList = new List<TaskMasterSetupInfo>();
            try
            {
                BAL.UserManagement mgt = new BAL.UserManagement();
                //HCMOneUsers = mgt.GetAllUsers(true);
                HCMOneUsers = mgt.GetAllFilteredUsersByDepartAndBranch(CurrentUser.SessionUser.ID, CurrentUser.DepartmentList, CurrentUser.BranchList, CurrentUser.isDepartmentDataAccess, CurrentUser.isBranchDataAccess);

                BAL.AssignmentFormManagement setup = new BAL.AssignmentFormManagement();
                taskList = setup.GetTaskMasterSetup(ID);
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("AssignmentFormController", "Exception occured on GetAssignmentForm Controller, " + ex.Message);
            }
            //return Json(new { response = HCMOneUsers, StageSetup = groupSetup.GetStageSetup(ID) }, JsonRequestBehavior.AllowGet);
            return Json(new { response = HCMOneUsers, TaskList = taskList}, JsonRequestBehavior.AllowGet);
        }
       
        [HttpGet]
        public ActionResult GetSapFunctions()
        {
            List<SAP_Function> list = new List<SAP_Function>();
            try
            {

                BAL.Common setupManagement = new BAL.Common();
                list = setupManagement.GetFunctionsSAPB1(0);
                
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("AssignmentFormController", "Exception occured on GetSapFunctions Controller, " + ex.Message);
            }
            return Json(new { response = list }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetSapSubFunctions()
        {
            List<SAP_Function> list = new List<SAP_Function>();
            try
            {

                BAL.Common setupManagement = new BAL.Common();
                list = setupManagement.GetSubFunctionsFromSAPB1();

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("AssignmentFormController", "Exception occured on GetSapSubFunctions Controller, " + ex.Message);
            }
            return Json(new { response = list }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetSapPartners()
        {
            List<SAP_Function> list = new List<SAP_Function>();
            try
            {

                BAL.Common setupManagement = new BAL.Common();
                list = setupManagement.GETPartnerFromSAPB1();

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("AssignmentFormController", "Exception occured on GetSapPartner Controller, " + ex.Message);
            }
            return Json(new { response = list }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetSapBranches()
        {
            List<SAP_Function> list = new List<SAP_Function>();
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
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("AssignmentFormController", "Exception occured on GetSapBranches Controller, " + ex.Message);
            }
            return Json(new { response = list}, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetDirectors()
        {
            HCMOneManagement mgt = new HCMOneManagement();
            
            return Json(new { response = mgt.GetAllHCMDirector() }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetAssignmentType()
        {
            List<DropDown> list = new List<DropDown>();
            List<string> docList = new List<string>();
            try
            {

                BAL.Common setupManagement = new BAL.Common();
                list = setupManagement.GetAssignmentType();

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("AssignmentFormController", "Exception occured on GetAssignmentType Controller, " + ex.Message);
            }
            return Json(new { response = list }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetBillingType()
        {
            List<DropDown> list = new List<DropDown>();
            List<string> docList = new List<string>();
            try
            {
                BAL.Common setupManagement = new BAL.Common();
                list = setupManagement.GetBillingType();
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("AssignmentFormController", "Exception occured on GetBillingType Controller, " + ex.Message);
            }
            return Json(new { response = list }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetStatusFunction()
        {
            List<DropDown> list = new List<DropDown>();
            List<string> docList = new List<string>();
            try
            {
                BAL.Common setupManagement = new BAL.Common();
                list = setupManagement.GetStatusFunction();
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("AssignmentFormController", "Exception occured on GetStatusFunction Controller, " + ex.Message);
            }
            return Json(new { response = list }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetDocStatusFunction()
        {
            List<TimseSheetStatus> list = new List<TimseSheetStatus>();
            List<string> docList = new List<string>();
            try
            {
                BAL.Common setupManagement = new BAL.Common();
                list = setupManagement.GetTimeSheetFormStatusList();
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("AssignmentFormController", "Exception occured on GetDocStatusFunction Controller, " + ex.Message);
            }
            return Json(new { response = list }, JsonRequestBehavior.AllowGet);
        }
        

        [HttpGet]
        public ActionResult GetAssignmentNature()
        {
            List<AssignmentNatureSetup> AssignmentNatureSetupList = new List<AssignmentNatureSetup>();
            try
            {
                BAL.cAssignmentNatureSetup setupManagement = new BAL.cAssignmentNatureSetup();
                AssignmentNatureSetupList = setupManagement.GetAssignmentNatureSetup(0);

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("cAssignmentNatureSetup", "Exception occured on GetAssignmentNatureSetup Controller, " + ex.Message);
            }
            return Json(new { response = AssignmentNatureSetupList }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetClientFunction()
        {
            List<SAP_Function> ClientList = new List<SAP_Function>();
            try
            {
                BAL.Common setupManagement = new BAL.Common();
                ClientList = setupManagement.GetClientFromSAPB1();

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("Common", "Exception occured on GetClientFunction Controller, " + ex.Message);
            }
            return Json(new { response = ClientList }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetCurrencyFunction()
        {
            List<SAP_Function> CurrencyList = new List<SAP_Function>();
            try
            {
                BAL.Common setupManagement = new BAL.Common();
                CurrencyList = setupManagement.GetCurrencyFromSAPB1();

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("Common", "Exception occured on GetCurrencyFunction Controller, " + ex.Message);
            }
            return Json(new { response = CurrencyList }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetAssignmentCostSetup()
        {
            List<AssignmentCostSetup> assignmentCostSetupList = new List<AssignmentCostSetup>();
            try
            {
                BAL.SetupManagement setupManagement = new BAL.SetupManagement();
                assignmentCostSetupList = setupManagement.GetAssignmentCostSetup(0);

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("SetupManagement", "Exception occured on GetAssignmentCostSetup Controller, " + ex.Message);
            }
            return Json(new { response = assignmentCostSetupList }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetBillingRates(string FunctionID, int DesignationID, double FromDate, double ToDate)
        {
            List<DropDown> list = new List<DropDown>();
            try
            {
                BAL.AssignmentFormManagement Assign = new BAL.AssignmentFormManagement();
                list = Assign.GETBillingRateForUser(FunctionID, DesignationID, FromDate, ToDate);
            }
            catch (Exception ex)
            {
                 Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("AssignmentFormManagement", "Exception occured on GetBillingRates Controller, " + ex.Message);
            }
            return Json(new { response = list }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetTripRatePolicy(string ClientID, int BranchID)
        {
            List<AssignmentFormChild> travelLocationList = new List<AssignmentFormChild>();
            try
            {
                BAL.AssignmentFormManagement Assign = new BAL.AssignmentFormManagement();
                travelLocationList = Assign.GetTripRatePolicySetup(ClientID, BranchID);

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("AssignmentFormController", "Exception occured on GetTripRatePolicy Controller, " + ex.Message);
            }
            //return Json(new { response = HCMOneUsers, StageSetup = groupSetup.GetStageSetup(ID) }, JsonRequestBehavior.AllowGet);
            return Json(new { TravelLocationList = travelLocationList }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetAssignmentFormSummary()
        {
            List<TaskMasterSetupInfo> AssignmentFormSummaryList = new List<TaskMasterSetupInfo>();
            try
            {
                BAL.AssignmentFormManagement AssignmentFormManagement = new BAL.AssignmentFormManagement();
                AssignmentFormSummaryList = AssignmentFormManagement.GetTaskMasterSetup(0);

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("AssignmentFormManagement", "Exception occured on GetTaskMasterSetup Controller, " + ex.Message);
            }
            return Json(new { response = AssignmentFormSummaryList }, JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult GetAssignmentFormHeader(int ID = 0)
        {
            BAL.AssignmentFormManagement assign = new BAL.AssignmentFormManagement();
            List<UserProfile> HCMOneUsers = new List<UserProfile>();
            List<AssignmentForm> assignmentFormList = new List<AssignmentForm>();
            List<AssignmentForm> assignmentFormListFiltered = new List<AssignmentForm>();
            try
            {
                BAL.UserManagement mgt = new BAL.UserManagement();
                //HCMOneUsers = mgt.GetAllUsers(false);
                // HCMOneUsers= mgt.GetAllFilteredUsersByDepartAndBranch(CurrentUser.SessionUser.ID, CurrentUser.TimeSheetViewList, CurrentUser.BranchList, CurrentUser.isDepartmentDataAccess, CurrentUser.isBranchDataAccess);

                //assignmentFormList = assign.GetAssignmentForm(ID);

                assignmentFormList = assign.GetAssignmentFormInit(ID);

                var filterByBranch = assignmentFormList.Where(item => CurrentUser.BranchList.Contains(Convert.ToInt32(item.BranchID)));
                assignmentFormListFiltered = filterByBranch.ToList();
               
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("AssingmentFormController", "Exception occured on GetAssignmentFormHeader , " + ex.Message);
            }
            return Json(new { AssignmentForm = assignmentFormListFiltered }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddUpdateAssignmentForm(string DOCNUM, bool NonChargeable, AssignmentForm AssignmentForm, AssignmentFormGeneral AssignmentFormGeneral,
            List<AssignmentFormChild> AssignmentFormChild, List<AssignmentFormCost> AssignmentFormCost, List<AssignmentFormSummary> AssignmentFormSummary, int ID)
        {
            bool suc = true;
            string msg = "Successfully Added/Updated";
            try
            {
                BAL.AssignmentFormManagement AssignmentFormManagement = new BAL.AssignmentFormManagement();
                AssignmentForm.CreatedBy = CurrentUser.SessionUser.ID;
                AssignmentForm.UpdatedBy = CurrentUser.SessionUser.ID;
                if(AssignmentForm.flgPost == false)
                {
                    //DateTime StartDate = Convert.ToDateTime(AssignmentFormGeneral.StartDate);
                    //DateTime EndDate = Convert.ToDateTime(AssignmentFormGeneral.EndDate);
                    //AssignmentFormGeneral.StartDate = string.Format("{0:dd/MM/yyyy}", StartDate);
                    //AssignmentFormGeneral.EndDate = string.Format("{0:dd/MM/yyyy}", EndDate);

                    AssignmentFormChild.Select(c => { c.CreatedBy = CurrentUser.SessionUser.ID; c.UpdatedBy = CurrentUser.SessionUser.ID; return c; }).ToList();
                    if(AssignmentFormCost != null)
                    {
                        if(AssignmentFormCost.Count != 0)
                            AssignmentFormCost.Select(c => { c.CreatedBy = CurrentUser.SessionUser.ID; c.UpdatedBy = CurrentUser.SessionUser.ID; return c; }).ToList();
                    }

                    if (AssignmentFormSummary != null)
                    {
                        AssignmentFormSummary.Select(c => { c.CreatedBy = CurrentUser.SessionUser.ID; c.UpdatedBy = CurrentUser.SessionUser.ID; return c; }).ToList();
                    }

                    suc = AssignmentFormManagement.AddUpdateAssignmentForm(out msg, DOCNUM, NonChargeable, AssignmentForm, AssignmentFormGeneral, AssignmentFormChild, (AssignmentFormCost == null ? new List<AssignmentFormCost>() : AssignmentFormCost), (AssignmentFormSummary == null ? new List<AssignmentFormSummary>() : AssignmentFormSummary), ID, CurrentUser.SessionUser.ID);

                }
                else
                {
                    AssignmentForm.DOCNUM = DOCNUM;
                    //DateTime StartDate = Convert.ToDateTime(AssignmentFormGeneral.StartDate);
                    //DateTime EndDate = Convert.ToDateTime(AssignmentFormGeneral.EndDate);
                    //AssignmentFormGeneral.StartDate = string.Format("{0:dd.MM.yyyy}", StartDate);
                    //AssignmentFormGeneral.EndDate = string.Format("{0:dd.MM.yyyy}", EndDate);

                    suc = AssignmentFormManagement.POSTToSBO(out msg, DOCNUM, NonChargeable, AssignmentForm, AssignmentFormGeneral, AssignmentFormSummary, ID, CurrentUser.SessionUser.ID);

                }

                return Json(new {  Success = suc, Message = msg  }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("AssingmentFormController", "Exception occured on AddUpdateAssignmentForm , " + ex.Message);
                return Json(new { Success = false, Message = "Exception Occured When Add/Update AddUpdateAssignmentForm" }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public ActionResult AddUpdateNCAssignmentForm(string DocNum, bool NonChargeable, AssignmentForm AssignmentForm, int ID)
        {
            bool suc = true;
            string msg = "Successfully Added/Updated";
            try
            {
                BAL.AssignmentFormManagement AssignmentFormManagement = new BAL.AssignmentFormManagement();
                AssignmentForm.CreatedBy = CurrentUser.SessionUser.ID;
                AssignmentForm.UpdatedBy = CurrentUser.SessionUser.ID;
                if (AssignmentForm.flgPost == false)
                {
                    suc = AssignmentFormManagement.AddUpdateNCAssignmentForm(out msg, DocNum, NonChargeable, AssignmentForm, ID, CurrentUser.SessionUser.ID);
                }
                else
                {
                    AssignmentForm.DOCNUM = DocNum;
                    
                   // suc = AssignmentFormManagement.POSTToSBO(out msg, DocNum, NonChargeable, AssignmentForm, AssignmentFormGeneral, AssignmentFormSummary, ID, CurrentUser.SessionUser.ID);

                }

                return Json(new { Success = suc, Message = msg }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("AssingmentFormController", "Exception occured on AddUpdateNCAssignmentForm , " + ex.Message);
                return Json(new { Success = false, Message = "Exception Occured When Add/Update AddUpdateNCAssignmentForm" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult validateAssignmentTitle(string assignmentTitle)
        {
            bool isSuccess = false;
            string message = "";
            try
            {
                AssignmentForm assignmentForm = new AssignmentForm();
                BAL.AssignmentFormManagement assignmentMgt = new AssignmentFormManagement();
                isSuccess = assignmentMgt.ValidateAssignmentTitle(assignmentTitle, out assignmentForm);
                if (isSuccess)
                {
                    if (assignmentForm != null)
                    {
                        message = assignmentForm.AssignmentTitle;
                    }
                }


                return Json(new { Success = isSuccess, Msg = message }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                message = "Exception occured in updating user passowrd";
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("UserController", "Exception occured on GetAssignmentCostSetup Controller, " + ex.Message);
                return Json(new { Success = isSuccess, Msg = message }, JsonRequestBehavior.AllowGet);
            }



        }
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////





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
                log.InputOutputDocLog("AssingmentFormController", "Exception occured on GetMaster_TaskByFunctionID , " + ex.Message);
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
                log.InputOutputDocLog("AssingmentFormController", "Exception occured on GetMaster_TaskByDocNum , " + ex.Message);
            }
            return Json(new { response = list }, JsonRequestBehavior.AllowGet);
        }

        
      
        public ActionResult GetAssignmentFormLog(string docId)
        {
            DataSet table = new DataSet();

            try
            {
                BAL.AssignmentFormManagement assignmentFormManagement = new BAL.AssignmentFormManagement();
                table = assignmentFormManagement.GetAssignmentFormLog(docId);

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
        
        
    }
}