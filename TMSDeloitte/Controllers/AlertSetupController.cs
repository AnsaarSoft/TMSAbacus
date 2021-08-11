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
    public class AlertSetupController : BaseController
    {
        // GET: AlertSetup
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetAlertSetup()
        {
            BAL.GroupSetupManagement groupSetup = new BAL.GroupSetupManagement();
            BAL.Common setupManagement = new BAL.Common();
            List<UserProfile> list = new List<UserProfile>();
            try
            {
                BAL.UserManagement mgt = new BAL.UserManagement();
                //HCMOneUsers = mgt.GetAllUsers(false);
                list = mgt.GetAllFilteredUsersByDepartAndBranch(CurrentUser.SessionUser.ID, CurrentUser.DepartmentList, CurrentUser.BranchList, CurrentUser.isDepartmentDataAccess, CurrentUser.isBranchDataAccess);
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("AlertSetupController", "Exception occured on GetAlertSetup Controller, " + ex.Message);
            }
            return Json(new { response = list, GroupSetup = groupSetup.GetGroupSetupHeader(0)}, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetDocList()
        {
            BAL.Common setupManagement = new BAL.Common();
            HCMOneManagement mgt = new HCMOneManagement();
            try
            {
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("AlertSetupController", "Exception occured on GetAllDocumentNum Controller, " + ex.Message);
            }
            return Json(new { docList = setupManagement.GetDocNum("GetAlertSetupDocNum", "AlertSetupController") }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult AddUpdateAlertSetup(AlertSetup alertSetupObj, List<UserProfile> userList, List<GroupSetupInfo> groupList)
        {
            string Msg = "Successfully Added/Updated";
            try
            {
                BAL.AlertSetupManagement Mgt = new BAL.AlertSetupManagement();
                if (userList == null)
                    userList = new List<UserProfile>();
                if (groupList == null)
                    groupList = new List<GroupSetupInfo>();

                alertSetupObj.CreatedBy = CurrentUser.SessionUser.ID;
                alertSetupObj.UpdatedBy = CurrentUser.SessionUser.ID;

                return Json(new { Success = Mgt.AddUpdateAlertSetup(alertSetupObj, userList, groupList, CurrentUser.SessionUser.ID, out Msg), Message = Msg }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = "Exception Occured When Add/Update Assignment Cost Setup" }, JsonRequestBehavior.AllowGet);
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("AlertSetupController", "Exception occured on AddUpdateAlertSetup Controller, " + ex.Message);

            }

        }

        public ActionResult GetAlertSetupByDocNum(string docNum)
        {


            AlertSetup setup = new AlertSetup();
            List<UserProfile> userList = new List<UserProfile>();
            List<GroupSetupInfo> groupList = new List<GroupSetupInfo>();

            try
            {
                BAL.AlertSetupManagement Mgt = new BAL.AlertSetupManagement();
                Mgt.GetAlertSetupByDocNum(docNum, out setup, out userList, out groupList);

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("AlertSetupController", "Exception occured on GetAlertSetupByDocNum Controller, " + ex.Message);
            }
            return Json(new {  AlertSetup = setup, userList = userList, groupList = groupList }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllDocuments()
        {
            DataTable table = new DataTable();
            try
            {
                BAL.AlertSetupManagement setupManagement = new BAL.AlertSetupManagement();
                table = setupManagement.GetAlertSetupAllDocumentsList();

                string jsonString = JsonConvert.SerializeObject(table, Formatting.Indented, new JsonSerializerSettings() { DateFormatString = "ddd, dd MMM yyy" });

                var jsonResult = Json(jsonString, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;

                return Json(new { response = jsonResult }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("AlertSetupController", "Exception occured on GetAllDocuments Controller, " + ex.Message);
                return Json(new { response = table }, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult GetLogByDocID(string docID)
        {
            DataTable dt_Setup = new DataTable();
            DataTable dt_User = new DataTable();
            DataTable dt_Group = new DataTable();

            try
            {
                BAL.AlertSetupManagement userMgt = new BAL.AlertSetupManagement();
                userMgt.GetLogByDocNum(docID, out dt_Setup, out dt_User, out dt_Group);

                string jsonString_Auth = JsonConvert.SerializeObject(dt_Setup, Formatting.Indented, new JsonSerializerSettings() { DateFormatString = "dd/MM/yyyy HH:MM:s" });
                var jsonResult_Auth = Json(jsonString_Auth, JsonRequestBehavior.AllowGet);
                jsonResult_Auth.MaxJsonLength = int.MaxValue;

                string jsonString_User = JsonConvert.SerializeObject(dt_User, Formatting.Indented, new JsonSerializerSettings() { DateFormatString = "dd/MM/yyyy HH:MM:s" });
                var jsonResult_User = Json(jsonString_User, JsonRequestBehavior.AllowGet);
                jsonResult_User.MaxJsonLength = int.MaxValue;

                string jsonString_Group = JsonConvert.SerializeObject(dt_Group, Formatting.Indented, new JsonSerializerSettings() { DateFormatString = "dd/MM/yyyy HH:MM:s" });
                var jsonResult_Group = Json(jsonString_Group, JsonRequestBehavior.AllowGet);
                jsonResult_Group.MaxJsonLength = int.MaxValue;

                return Json(new { dt_Setup = jsonResult_Auth, dt_User = jsonResult_User, dt_Group = jsonResult_Group }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("AlertSetupController", "Exception occured on GetLogByDocID Controller, " + ex.Message);
            }
            return Json(new { dt_Setup = dt_Setup, dt_User = dt_User, dt_Group = dt_Group }, JsonRequestBehavior.AllowGet);
        }
    }
}