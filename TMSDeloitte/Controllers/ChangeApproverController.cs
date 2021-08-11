using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TMSDeloitte.BAL;
using TMSDeloitte.Helper;
using TMSDeloitte.Models;

namespace TMSDeloitte.Controllers
{
    public class ChangeApproverController : BaseController
    {
        // GET: ChangeApprover
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult GetHCMFunctions()
        {
            List<UserProfile> HCMOneUsersRow = new List<UserProfile>();
            List<UserProfile> HCMOneUsers = new List<UserProfile>();
            List<Department> listDepartment = new List<Department>();
            List<Designation> listDesignation = new List<Designation>();

            HCMOneManagement mgt = new HCMOneManagement();
            BAL.UserManagement usrmgt = new BAL.UserManagement();
            try
            {
                List<int> departList = new List<int>();
                //List<int> designList = new List<int>();
                if (CurrentUser.DepartmentList.Count == 0)
                {
                    departList.Add(Convert.ToInt32(CurrentUser.SessionUser.DEPARTMENTID));
                    //designList.Add(Convert.ToInt32(CurrentUser.SessionUser.DESIGNATIONID));
                }
                else
                    departList = CurrentUser.DepartmentList;

               
                listDepartment = mgt.GetAllFilteredHCMDepartment(departList);
                listDesignation = mgt.GetAllHCMDesignation();
                HCMOneUsersRow = usrmgt.GetAllUsers(false);
                HCMOneUsers = usrmgt.GetAllFilteredUsersByDepartAndBranch(CurrentUser.SessionUser.ID, CurrentUser.DepartmentList, CurrentUser.BranchList, CurrentUser.isDepartmentDataAccess, CurrentUser.isBranchDataAccess);
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ChangeApproverController", "Exception occured on GetHCMFunctions, " + ex.Message);
            }
            return Json(new {AllEmployee= HCMOneUsers, AllEmployees = HCMOneUsersRow, DepartmentList = listDepartment, DesignationList = listDesignation }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetChangeApproverForms(int EmpID, int DesignationID, int DepartmentID)
        {
            List<UserProfile> HCMOneUsers = new List<UserProfile>();
            BAL.ChangeApproverManagement CAM = new BAL.ChangeApproverManagement();
            List<ChangeApproverChild> changeApproverChild = new List<ChangeApproverChild>();

            BAL.UserManagement usrmgt = new BAL.UserManagement();
            bool Success = true;
            try
            {
                changeApproverChild = CAM.GetChangeApproverForms(EmpID, DesignationID, DepartmentID);
                HCMOneUsers = usrmgt.GetAllUsers(false);
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ChangeApproverController", "Exception occured on GetChangeApproverForms, " + ex.Message);
            }
            return Json(new { response = changeApproverChild,  Success = Success }, JsonRequestBehavior.AllowGet);

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
        public ActionResult UpdateApprovalSetup(List<ChangeApproverChild> ChangeApproverChild)
        {
            bool suc = true;
            string msg = "Successfully Added/Updated";
            try
            {
                BAL.ChangeApproverManagement ChangeApproverManagement = new BAL.ChangeApproverManagement();

                ChangeApproverChild.Select(c => { c.CreatedBy = CurrentUser.SessionUser.ID; c.UpdatedBy = CurrentUser.SessionUser.ID; return c; }).ToList();
                string fullName = CurrentUser.SessionUser.FULLNAME;
                suc = ChangeApproverManagement.AddUpdateApprovalSetup(out msg, ChangeApproverChild, CurrentUser.SessionUser.ID, fullName);
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

    }
}