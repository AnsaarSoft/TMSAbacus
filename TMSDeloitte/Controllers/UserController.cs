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
    public class UserController : BaseController
    {

        // GET: USer
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetHCMUsers()
        {
            List<HCM_Employee> list = new List<HCM_Employee>();
            try
            {
                BAL.UserManagement setup = new UserManagement();
                list = setup.GetAllFilteredHCMUsersByDepartAndBranch(CurrentUser.SessionUser.ID, CurrentUser.DepartmentList, CurrentUser.BranchList, CurrentUser.isDepartmentDataAccess, CurrentUser.isBranchDataAccess);
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("UserController", "Exception occured on GetHCMUsers Controller, " + ex.Message);
            }
            return Json(new { response = list }, JsonRequestBehavior.AllowGet);
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
                log.InputOutputDocLog("UserController", "Exception occured on GetUserInfoByEmpCode Controller, " + ex.Message);
            }
            return Json(new { Success = isSuccess, UserInfo = user }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddUpdateUser(UserProfile user)
        {
            string Msg = "Successfully Added/Updated";
            try
            {
                BAL.UserManagement setupManagement = new BAL.UserManagement();
                user.CREATEDBY = CurrentUser.SessionUser.ID;
                user.UPDATEDEDBY = CurrentUser.SessionUser.ID;
                return Json(new { Success = setupManagement.AddUpdateUser(user, out Msg), Message = Msg }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = "Exception Occured When Add/Update Assignment user" }, JsonRequestBehavior.AllowGet);
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("UserController", "Exception occured on AddUpdateUser Controller, " + ex.Message);

            }

        }

        public ActionResult GetUserLogByUserID(string id)
        {
            DataTable table = new DataTable();
            try
            {
                BAL.UserManagement setupManagement = new BAL.UserManagement();
                table = setupManagement.GetUserLogByUserID(id);

                string jsonString = JsonConvert.SerializeObject(table, Formatting.Indented, new JsonSerializerSettings() { DateFormatString = "dd/MM/yyyy HH:MM:s" });

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


        [HttpPost]
        public JsonResult validateUser(string userName)
        {
            bool isSuccess = false;
            string message = "";
            try
            {
                UserProfile userProfile = new UserProfile();
                BAL.UserManagement userMgt = new UserManagement();
                isSuccess = userMgt.ValidateUserName(userName, out userProfile);
                if (isSuccess)
                {
                    if (userProfile != null)
                    {
                        message = userProfile.USERNAME;
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

        [HttpPost]
        public JsonResult ResetPasswordByAdmin(int id, string userPassword)
        {
            bool isSuccess = false;
            string message = "";
            try
            {
                if (id != 0)
                {
                    UserManagement Profile = new UserManagement();
                    isSuccess = Profile.UpdateUserPassword(id, userPassword, CurrentUser.SessionUser.ID);

                    if (isSuccess)
                    {
                        message = "Successfully updated user password";
                    }
                    else
                        message = "Exception occured in updating user passowrd";
                }
                else
                {
                    message = "Invalid user!";
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



    }

}