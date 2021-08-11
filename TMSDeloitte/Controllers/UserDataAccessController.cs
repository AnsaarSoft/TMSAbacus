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
    public class UserDataAccessController : BaseController
    {

        // GET: UserDataAccess
        public ActionResult Index()
        {
            return View();
        }
        
        public ActionResult GetUserDataAccessData()
        {
            BAL.Common cmn = new BAL.Common();
            BAL.UserManagement userMgt = new UserManagement();
            try
            {
                return Json(new { docList = cmn.GetDocNum("GetDataAccessDocNum", "UserDataAccess"), MenuList = cmn.GetUserDataAccessMenuList(), AuthList = cmn.GetUserAuthorizationEnumsList(), UserList = userMgt.GetAllFilteredUsersByDepartAndBranch(CurrentUser.SessionUser.ID, CurrentUser.DepartmentList, CurrentUser.BranchList, CurrentUser.isDepartmentDataAccess, CurrentUser.isBranchDataAccess) }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("UserDataAccess", "Exception occured on GetUserDataAccessData Controller, " + ex.Message);
                return Json(new { MenuList = new List<TMS_Menu>(), AuthList = new List<EnumUserAuthorizationViewModel>() }, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult GetUserInfoByEmpCode(string empCode)
        {
            UserProfile user = new UserProfile();
            bool isSuccess = false;
            try
            {
                BAL.UserManagement userMgt = new BAL.UserManagement();

                isSuccess = userMgt.GetUserByEmpCode(empCode, out user);
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("UserDataAccess", "Exception occured on GetUserInfoByEmpCode Controller, " + ex.Message);
            }
            return Json(new { Success = isSuccess, UserInfo = user }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddUpdateUserDataAccess(int DocID, List<UserDataAccess_Menu> authList, int UserID)
        {
            string Msg = "Successfully Added/Updated";
            try
            {
                BAL.UserDataAccess userMgt = new BAL.UserDataAccess();

                return Json(new { Success = userMgt.AddUpdateUserDataAccess(DocID, authList, UserID, CurrentUser.SessionUser.ID, out Msg), Message = Msg }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = "Exception Occured When Add/Update Assignment Cost Setup" }, JsonRequestBehavior.AllowGet);
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("UserDataAccess", "Exception occured on GetAssignmentCostSetup Controller, " + ex.Message);

            }

        }

        public ActionResult GetUserDataAccessByDocNum(string docNum)
        {
            int DocID = 0;
            string EmpCode = "";
            int EmpID = 0;
            try
            {
                BAL.UserDataAccess userMgt = new BAL.UserDataAccess();
                userMgt.GetDataAccessByDocNum(docNum, out DocID,out EmpID, out EmpCode);

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("UserDataAccess", "Exception occured on GetDataAccessByDocNum Controller, " + ex.Message);
            }
            return Json(new { DocID = DocID, EmpID= EmpID, EmpCode = EmpCode }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetUserDataAccessByUserID(string UserID)
        {

            List<UserDataAccess_Menu> authList = new List<UserDataAccess_Menu>();
            int DocID = 0;
            string DocNum = "";
            try
            {
                BAL.UserDataAccess userMgt = new BAL.UserDataAccess();
                userMgt.GetDataAccessByUserID(UserID, out DocID,out DocNum, out authList);

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("UserDataAccess", "Exception occured on GetDataAccessByDocNum Controller, " + ex.Message);
            }
            return Json(new { DocID = DocID, DocNum= DocNum, authList = authList }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDataAccessLogByDocID(string docId)
        {
            DataTable dt_Auth = new DataTable();

            try
            {
                BAL.UserDataAccess userMgt = new BAL.UserDataAccess();
                userMgt.GetDataAccessLogByDocID(docId, out dt_Auth);

                string jsonString_Auth = JsonConvert.SerializeObject(dt_Auth, Formatting.Indented, new JsonSerializerSettings() { DateFormatString = "dd/MM/yyyy HH:MM:s" });
                var jsonResult_Auth = Json(jsonString_Auth, JsonRequestBehavior.AllowGet);
                jsonResult_Auth.MaxJsonLength = int.MaxValue;

             

               
                return Json(new { response = jsonResult_Auth}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("UserDataAccess", "Exception occured on GetDataAccessLogByDocID Controller, " + ex.Message);
            }
            return Json(new { response = dt_Auth}, JsonRequestBehavior.AllowGet);
        }
    }
}