using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TMSDeloitte.Helper;
using TMSDeloitte.Models;
using TMSDeloitte.BAL;

namespace TMSDeloitte.Controllers
{
    public class GroupSetupController : BaseController
    {
        // GET: GroupSetup

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetGroupSetup(int ID = 0)
        {
            BAL.GroupSetupManagement groupSetup = new BAL.GroupSetupManagement();
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
                log.InputOutputDocLog("GroupSetupController", "Exception occured on GetGroupSetup Controller, " + ex.Message);
            }
            return Json(new { response = list, GroupSetup =  groupSetup.GetGroupSetup(ID) }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetGroupSetupHeader(int ID = 0)
        {
            BAL.GroupSetupManagement groupSetup = new BAL.GroupSetupManagement();
            List<UserProfile> HCMOneUsers = new List<UserProfile>();
            try
            {
                BAL.UserManagement mgt = new BAL.UserManagement();
                HCMOneUsers = mgt.GetAllUsers(false);

                string jsonString = JsonConvert.SerializeObject(groupSetup.GetGroupSetup(ID), Formatting.Indented, new JsonSerializerSettings() { DateFormatString = "dd/MM/yyyy HH:MM:s" });

                var jsonResult = Json(jsonString, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;

                return Json(new { GroupSetup = jsonResult }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("GroupSetupController", "Exception occured on GetGroupSetupHeader Controller, " + ex.Message);
            }
            return Json(new { GroupSetup = "" }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult AddUpdateGroupSetup(string GROUPCODE , string GROUPNAME, string DOCNUM , bool ISACTIVE, List<GroupSetupChildInfo> GroupSetupInfo, int ID)
        {
            bool suc = true;
            string msg = "Successfully Added/Updated";
            try
            {
                BAL.GroupSetupManagement GroupSetupManagement = new BAL.GroupSetupManagement();
                
                GroupSetupInfo.Select(c => { c.CREATEDBY = CurrentUser.SessionUser.ID; c.UPDATEDBY = CurrentUser.SessionUser.ID; return c; }).ToList();

                suc= GroupSetupManagement.AddUpdateGroupSetup(out msg, GroupSetupInfo,DOCNUM, GROUPCODE, GROUPNAME, CurrentUser.SessionUser.ID,  ID, ISACTIVE);
                //GroupSetupInfo.Select(c => { c.CREATEDBY = 1; c.UPDATEDBY = 1; return c; }).ToList();
                //suc = TravelLocationManagement.AddUpdateTravelLocation(GroupSetupInfo);
                //bool IsDuplicationOccured = TravelLocationManagement.CheckDuplicateRecord(GroupSetupInfo);

                //if (IsDuplicationOccured)
                //{
                //    suc = false;
                //    msg = "Location Can not be duplicate";
                //}
                //else
                //{
                //    suc = TravelLocationManagement.AddUpdateTravelLocation(GroupSetupInfo);
                //    msg = "Successfully Added/Updated";
                //}
                //suc = TravelLocationManagement.AddUpdateGroupSetup(GroupSetupInfo);
                //suc = 
                return Json(new
                {
                    Success = suc,
                    Message = msg
                },
                    JsonRequestBehavior.AllowGet);

                //return Json(TravelLocationManagement.AddUpdateTravelLocation(GroupSetupInfo), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = "Exception Occured When Add/Update GroupSetup" }, JsonRequestBehavior.AllowGet);
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("GroupSetupController", "Exception occured on AddUpdateGetGroupSetup Controller, " + ex.Message);

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
                log.InputOutputDocLog("GroupSetupController", "Exception occured on GetMaster_TaskByFunctionID Controller, " + ex.Message);
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
                log.InputOutputDocLog("GroupSetupController", "Exception occured on GetMaster_TaskByDocNum Controller, " + ex.Message);
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
                log.InputOutputDocLog("GroupSetupController", "Exception occured on GetTravelLocation Controller, " + ex.Message);
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
                log.InputOutputDocLog("GroupSetupController", "Exception occured on GetAssignmentCostSetup Controller, " + ex.Message);
                return Json(new { response = table }, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult GetLogByDocID(int docID)
        {
            DataTable dt_Header = new DataTable();
            DataTable dt_Detail = new DataTable();

            try
            {
                BAL.GroupSetupManagement userMgt = new BAL.GroupSetupManagement();
                userMgt.GetGroupSetupLogByDocID(docID, out dt_Header, out dt_Detail);

                string jsonString_Auth = JsonConvert.SerializeObject(dt_Header, Formatting.Indented, new JsonSerializerSettings() { DateFormatString = "dd/MM/yyyy HH:MM:s" });
                var jsonResult_Auth = Json(jsonString_Auth, JsonRequestBehavior.AllowGet);
                jsonResult_Auth.MaxJsonLength = int.MaxValue;

                string jsonString_User = JsonConvert.SerializeObject(dt_Detail, Formatting.Indented, new JsonSerializerSettings() { DateFormatString = "dd/MM/yyyy HH:MM:s" });
                var jsonResult_User = Json(jsonString_User, JsonRequestBehavior.AllowGet);
                jsonResult_User.MaxJsonLength = int.MaxValue;

               

                return Json(new { Header = jsonResult_Auth, Detail = jsonResult_User }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("GroupSetupController", "Exception occured on GetLogByDocID Controller, " + ex.Message);
            }
            return Json(new { Header = dt_Header, Detail = dt_Detail }, JsonRequestBehavior.AllowGet);
        }

    }
}