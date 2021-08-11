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
    public class NCTaskAssignmentController : BaseController
    {
        // GET: NCTaskAssignment
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetUsers()
        {
            List<UserProfile> HCMOneUsers = new List<UserProfile>();
            List<UserProfile> FilteredHCMOneUsers = new List<UserProfile>();
            BAL.Common cmn = new Common();
            try
            {
                BAL.UserManagement mgt = new BAL.UserManagement();
                HCMOneUsers = mgt.GetAllFilteredUsersByDepartAndBranch(CurrentUser.SessionUser.ID, CurrentUser.DepartmentList, CurrentUser.BranchList, CurrentUser.isDepartmentDataAccess, CurrentUser.isBranchDataAccess);

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("NCTaskAssignmentController", "Exception occured on GetHCMUsers Controller, " + ex.Message);
            }
            return Json(new
            {
                response = HCMOneUsers
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDocList()
        {
            List<string> list = new List<string>();
            BAL.NCTaskAssignmentManagement ncTaskMgt = new NCTaskAssignmentManagement();
            try
            {
                BAL.Common cmn = new Common();
                BAL.TypeOfClaimManagement TypeOfClaimManagement = new BAL.TypeOfClaimManagement();
                
                list = ncTaskMgt.GetDocNum();

               
                return Json(new
                {
                    docList = list
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("NCTaskAssignmentController", "Exception occured on GetAllDocumentNum Controller, " + ex.Message);

                return Json(new
                {
                    docList = list
                }, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult GetNCTaskByEmpID(int id)
        {
            DataTable dt_Task = new DataTable();

            try
            {
                BAL.NCTaskAssignmentManagement mgt = new BAL.NCTaskAssignmentManagement();

                dt_Task = mgt.GetNCTaskByEmpID(id);
                string jsonString_Task = JsonConvert.SerializeObject(dt_Task, Formatting.Indented, new JsonSerializerSettings() { DateFormatString = "dd/MM/yyyy HH:MM:s" });
                var jsonResult_Task = Json(jsonString_Task, JsonRequestBehavior.AllowGet);
                jsonResult_Task.MaxJsonLength = int.MaxValue;



                return Json(new { dt_TaskList = jsonResult_Task }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("TimeSheetFormController", "Exception occured on GetTaskAndLocationByAssignmentID Controller, " + ex.Message);
            }
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult AddUpdateNCTaskAssignment(NCTaskAssignment obj)
        {
            try
            {
                string msg = "Successfully Added/Updated";
                BAL.NCTaskAssignmentManagement setupManagement = new BAL.NCTaskAssignmentManagement();
                obj.CreatedBy = CurrentUser.SessionUser.ID;
                obj.UpdatedBy = CurrentUser.SessionUser.ID;
                return Json(new { Success = setupManagement.AddUpdateNCTaskAssignmentDetailsSetup(obj, out msg), Message = msg }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("NCTaskAssignmentController", "Exception occured on AddUpdateTimeSheetPeriod Controller, " + ex.Message);
                return Json(new { Success = false, Message = "Exception Occured When Add/Update Time Sheet Period!" }, JsonRequestBehavior.AllowGet);

            }

        }


        public ActionResult GetAllDocuments()
        {
            List<NCTaskAssignment> list = new List<NCTaskAssignment>();
            List<NCTaskAssignment> filteredList = new List<NCTaskAssignment>();
            try
            {
                BAL.NCTaskAssignmentManagement setupManagement = new BAL.NCTaskAssignmentManagement();
                list = setupManagement.GetAllNCTaskAssignment();


                BAL.UserManagement mgt = new BAL.UserManagement();
                List<UserProfile> userlist = new List<UserProfile>();
                userlist = mgt.GetAllFilteredUsersByDepartAndBranch(CurrentUser.SessionUser.ID, CurrentUser.DepartmentList, CurrentUser.BranchList, CurrentUser.isDepartmentDataAccess, CurrentUser.isBranchDataAccess);
                

                var filter = list.Where(item => userlist.Select(x => x.ID).Contains(Convert.ToInt32(item.EmpID)));
                filteredList = filter.ToList();

                List<string> docNumList = new List<string>();
                docNumList = filteredList.Select(x => x.DocNum).ToList();

                //string jsonString = JsonConvert.SerializeObject(list, Formatting.Indented, new JsonSerializerSettings() { DateFormatString = "ddd, dd MMM yyy" });

                //var jsonResult = Json(jsonString, JsonRequestBehavior.AllowGet);
                //jsonResult.MaxJsonLength = int.MaxValue;

                return Json(new { docList = docNumList }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("NCTaskAssignmentController", "Exception occured on GetAllDocuments Controller, " + ex.Message);
                return Json(new { response = list }, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult GetAllDocumentsByEmpID()
        {
            List<NCTaskAssignment> filteredlist = new List<NCTaskAssignment>();
            List<NCTaskAssignment> list = new List<NCTaskAssignment>();
            List<UserProfile> userlist = new List<UserProfile>();
            try
            {
                BAL.NCTaskAssignmentManagement setupManagement = new BAL.NCTaskAssignmentManagement();
                list = setupManagement.GetAllNCTaskAssignment() ;

                BAL.UserManagement mgt = new BAL.UserManagement();
                userlist = mgt.GetAllFilteredUsersByDepartAndBranch(CurrentUser.SessionUser.ID, CurrentUser.DepartmentList, CurrentUser.BranchList, CurrentUser.isDepartmentDataAccess, CurrentUser.isBranchDataAccess);

                var filter = list.Where(item => userlist.Select(x=>x.ID).Contains(Convert.ToInt32(item.EmpID)));
                filteredlist = filter.ToList();

                List<string> docNumList = new List<string>();
                docNumList = filteredlist.Select(x => x.DocNum).ToList();

                //foreach (var item in filter)
                //{
                //    filteredlist.Add(item);
                //}

                string jsonString = JsonConvert.SerializeObject(filteredlist, Formatting.Indented, new JsonSerializerSettings() { DateFormatString = "dd/MM/yyyy" });
                var jsonResult = Json(jsonString, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;

                return Json(new { response = jsonResult, docList = docNumList }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("NCTaskAssignmentController", "Exception occured on GetAllDocuments Controller, " + ex.Message);
                return Json(new { response = list }, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult GetNCTaskAssignmentByDocNum(string docNum)
        {
            NCTaskAssignment obj = new NCTaskAssignment();
            try
            {
                BAL.NCTaskAssignmentManagement setupManagement = new BAL.NCTaskAssignmentManagement();
                obj = setupManagement.GetNCTaskAssignmentByDocNum(docNum);
                string jsonString = JsonConvert.SerializeObject(obj, Formatting.Indented, new JsonSerializerSettings() { DateFormatString = "yyyy/MM/dd" });

                var jsonResult = Json(jsonString, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;

                return Json(new { response = jsonResult }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("NCTaskAssignmentController", "Exception occured on GetTimeSheetByDocNum Controller, " + ex.Message);
            }
            return Json(new { response = obj }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetNCTaskAssignment(string docNum, string EmpID)
        {
            NCTaskAssignment obj = new NCTaskAssignment();
            Encrypt_Decrypt security = new Encrypt_Decrypt();
            try
            {
                docNum = security.DecryptString(docNum);
                //EmpID = security.DecryptString(EmpID);

                BAL.NCTaskAssignmentManagement setupManagement = new BAL.NCTaskAssignmentManagement();
                obj = setupManagement.GetNCTaskAssignmentByDocNum(docNum);
                string jsonString = JsonConvert.SerializeObject(obj, Formatting.Indented, new JsonSerializerSettings() { DateFormatString = "yyyy/MM/dd" });

                var jsonResult = Json(jsonString, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;

                return Json(new { response = jsonResult }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("NCTaskAssignmentController", "Exception occured on GetTimeSheetByDocNum Controller, " + ex.Message);
            }
            return Json(new { response = obj }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult GeNCTaskAssignmentLog(int docid)
        {
            DataTable dtHeader = new DataTable();
            DataTable dtDetail = new DataTable();

            try
            {
                BAL.NCTaskAssignmentManagement mgt = new BAL.NCTaskAssignmentManagement();
                DataSet ds = mgt.GetNCTaskAssignmentLog(docid);

                if (ds.Tables.Count > 0)
                {
                    string jsonString_Header = JsonConvert.SerializeObject(ds.Tables[0], Formatting.Indented, new JsonSerializerSettings() { DateFormatString = "dd/MM/yyyy HH:MM:s" });
                    var jsonResult_Header = Json(jsonString_Header, JsonRequestBehavior.AllowGet);
                    jsonResult_Header.MaxJsonLength = int.MaxValue;

                    string jsonString_Detail = JsonConvert.SerializeObject(ds.Tables[1], Formatting.Indented, new JsonSerializerSettings() { DateFormatString = "dd/MM/yyyy HH:MM:s" });
                    var jsonResult_Detail = Json(jsonString_Detail, JsonRequestBehavior.AllowGet);
                    jsonResult_Detail.MaxJsonLength = int.MaxValue;

                    return Json(new { Header = jsonResult_Header, Detail = jsonResult_Detail }, JsonRequestBehavior.AllowGet);
                }


            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("NCTaskAssignmentController", "Exception occured on GeNCTaskAssignmentLog Controller, " + ex.Message);
            }
            return Json(new { Header = dtHeader, Detail = dtDetail }, JsonRequestBehavior.AllowGet);
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
                log.InputOutputDocLog("TimeSheetViewController", "Exception occured on GetUserInfoByEmpCode Controller, " + ex.Message);
            }
            return Json(new { Success = isSuccess, UserInfo = user }, JsonRequestBehavior.AllowGet);
        }
    }
}