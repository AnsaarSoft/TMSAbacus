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
    public class UserAuthorizationController : BaseController
    {

        // GET: UserAuthorization
        public ActionResult Index()
        {
            return View();
        }
        
        public ActionResult GetUserAuthorizationData()
        {
            BAL.Common cmn = new BAL.Common();
            BAL.UserManagement userMgt = new UserManagement();
            BAL.GroupSetupManagement groupSetup = new BAL.GroupSetupManagement();
            try
            {
                return Json(new { docList = cmn.GetDocNum("GetUserAuthorizationDocNum", "UserAuthorization"), MenuList = cmn.GetMenuList(), AuthList = cmn.GetUserAuthorizationEnumsList(), UserList = userMgt.GetAllFilteredUsersByDepartAndBranch(CurrentUser.SessionUser.ID, CurrentUser.DepartmentList, CurrentUser.BranchList, CurrentUser.isDepartmentDataAccess, CurrentUser.isBranchDataAccess), GroupList = groupSetup.GetGroupSetup(0).Where(x=>x.ISACTIVE==true).ToList() }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("UserAuthorization", "Exception occured on UserAuthorizationData Controller, " + ex.Message);
                return Json(new { MenuList = new List<TMS_Menu>(), AuthList = new List<EnumUserAuthorizationViewModel>() }, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public ActionResult AddUpdateUserAuthorization(int DocID, List<TMS_Menu> authList, List<UserProfile> userList, List<GroupSetupInfo> groupList)
        {
            string Msg = "Successfully Added/Updated";
            bool isSuccess = false;
            try
            {
                BAL.UserAuthorization userMgt = new BAL.UserAuthorization();
                isSuccess = userMgt.AddUpdateUserAuthorization(DocID, authList, userList, groupList, CurrentUser.SessionUser.ID, out Msg);
                


                return Json(new { Success=isSuccess, Message = Msg }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = "Exception Occured When Add/Update Assignment Cost Setup" }, JsonRequestBehavior.AllowGet);
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("UserAuthorization", "Exception occured on GetAssignmentCostSetup Controller, " + ex.Message);

            }

        }

        public ActionResult GetAuthorizationByDocNum(string docNum)
        {

            List<TMS_Menu> authList = new List<TMS_Menu>();
            List<UserProfile> userList = new List<UserProfile>();
            List<GroupSetupInfo> groupList = new List<GroupSetupInfo>();
            int DocID = 0;
            try
            {
                BAL.UserAuthorization userMgt = new BAL.UserAuthorization();
                userMgt.GetAuthorizationByDocNum(docNum, out DocID, out authList, out userList, out groupList);

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("UserAuthorization", "Exception occured on GetAuthorizationByDocNum Controller, " + ex.Message);
            }
            return Json(new { DocID = DocID, authList = authList, userList = userList, groupList = groupList }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAuthorizationByUserID(int userid)
        {

            List<TMS_Menu> authList = new List<TMS_Menu>();
            List<UserProfile> userList = new List<UserProfile>();
            List<GroupSetupInfo> groupList = new List<GroupSetupInfo>();
            int DocID = 0;
            try
            {
                BAL.UserAuthorization userMgt = new BAL.UserAuthorization();
                userMgt.GetAuthorizationByUserID(userid, out DocID, out authList, out userList, out groupList);

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("UserAuthorization", "Exception occured on GetAuthorizationByDocNum Controller, " + ex.Message);
            }
            return Json(new { DocID = DocID, authList = authList, userList = userList, groupList = groupList }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAuthorizationByGroupID(int groupid)
        {

            List<TMS_Menu> authList = new List<TMS_Menu>();
            List<UserProfile> userList = new List<UserProfile>();
            List<GroupSetupInfo> groupList = new List<GroupSetupInfo>();
            int DocID = 0;
            try
            {
                BAL.UserAuthorization userMgt = new BAL.UserAuthorization();
                userMgt.GetAuthorizationByGroupID(groupid, out DocID, out authList, out userList, out groupList);

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("UserAuthorization", "Exception occured on GetAuthorizationByDocNum Controller, " + ex.Message);
            }
            return Json(new { DocID = DocID, authList = authList, userList = userList, groupList = groupList }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetAuthorizationLogByDocID(string docId)
        {
            DataTable dt_Auth = new DataTable();
            DataTable dt_User = new DataTable();
            DataTable dt_Group = new DataTable();

            try
            {
                BAL.UserAuthorization userMgt = new BAL.UserAuthorization();
                userMgt.GetAuthorizationLogByDocID(docId, out dt_Auth, out dt_User, out dt_Group);

                string jsonString_Auth = JsonConvert.SerializeObject(dt_Auth, Formatting.Indented, new JsonSerializerSettings() { DateFormatString = "dd/MM/yyyy HH:MM:s" });
                var jsonResult_Auth = Json(jsonString_Auth, JsonRequestBehavior.AllowGet);
                jsonResult_Auth.MaxJsonLength = int.MaxValue;

                string jsonString_User = JsonConvert.SerializeObject(dt_User, Formatting.Indented, new JsonSerializerSettings() { DateFormatString = "dd/MM/yyyy HH:MM:s" });
                var jsonResult_User = Json(jsonString_User, JsonRequestBehavior.AllowGet);
                jsonResult_User.MaxJsonLength = int.MaxValue;

                string jsonString_Group = JsonConvert.SerializeObject(dt_Group, Formatting.Indented, new JsonSerializerSettings() { DateFormatString = "dd/MM/yyyy HH:MM:s" });
                var jsonResult_Group = Json(jsonString_Group, JsonRequestBehavior.AllowGet);
                jsonResult_Group.MaxJsonLength = int.MaxValue;

                return Json(new { dt_Auth = jsonResult_Auth, dt_User = jsonResult_User, dt_Group = jsonResult_Group }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("UserAuthorization", "Exception occured on GetAuthorizationLogByDocID Controller, " + ex.Message);
            }
            return Json(new { dt_Auth = dt_Auth, dt_User = dt_User, dt_Group = dt_Group }, JsonRequestBehavior.AllowGet);
        }
    }
}