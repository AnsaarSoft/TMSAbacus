using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TMSDeloitte.Helper;
using TMSDeloitte.Models;

namespace TMSDeloitte.Controllers
{

    public class TaskMasterSetupController : BaseController
    {
        // GET: TaskMasterSetup
        #region "TaskMasterSetup"

        public ActionResult TaskMasterSetup()
        {
            return View();
        }

        public ActionResult GetTaskMasterSetup()
        {
            List<TaskMasterSetupInfo> TaskMasterSetupList = new List<TaskMasterSetupInfo>();
            try
            {
                BAL.TaskMasterManagement TaskMasterManagement = new BAL.TaskMasterManagement();
                TaskMasterSetupList = TaskMasterManagement.GetTaskMasterSetup(0);

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("TaskMasterSetup", "Exception occured on GetTaskMasterSetup Controller, " + ex.Message);
            }
            return Json(new { response = TaskMasterSetupList }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddUpdateGetTaskMasterSetup(List<TaskMasterSetupInfo> TaskMasterSetupList)
        {
            string msg = "Successfully Added/Updated";
            try
            {
                BAL.TaskMasterManagement TaskMasterManagement = new BAL.TaskMasterManagement();
                TaskMasterSetupList.Select(c => { c.CREATEDBY = CurrentUser.SessionUser.ID; c.UPDATEDEDBY = CurrentUser.SessionUser.ID; return c; }).ToList();
                //TaskMasterSetupList.Select(c => { c.CREATEDBY = 1; c.UPDATEDEDBY = 1; return c; }).ToList();
                return Json(new { Success = TaskMasterManagement.AddUpdateTaskMasterSetup(TaskMasterSetupList,out msg), Message = msg }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = "Exception Occured When Add/Update Task Master Setup" }, JsonRequestBehavior.AllowGet);
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("TaskMasterSetup", "Exception occured on AddUpdateGetTaskMasterSetup Controller, " + ex.Message);

            }

        }

        public ActionResult GetSapFunctions()
        {
            List<SAP_Function> list = new List<SAP_Function>();
            List<string> docList = new List<string>();
            try
            {

                BAL.Common setupManagement = new BAL.Common();
                //list = setupManagement.GetSAPFunctionList();
                list = setupManagement.GetFunctionsFromSAPB1();
                BAL.TaskMasterManagement setup = new BAL.TaskMasterManagement();
                docList = setup.GetTaskMasterDocNum();
                
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("TaskMasterSetupController", "Exception occured on GetSapFunctions Controller, " + ex.Message);
            }
            return Json(new { response = list, DocList = docList }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMaster_TaskByFunctionID(string functionID)
        {
            List<TaskMasterSetupInfo> list = new List<TaskMasterSetupInfo>();
            try
            {
                BAL.TaskMasterManagement taskMaster = new BAL.TaskMasterManagement();
                list = taskMaster.GetTaskMasterByFunctionID(functionID);

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("TaskMasterSetupController", "Exception occured on GetMaster_TaskByFunctionID Controller, " + ex.Message);
            }
            return Json(new { response = list }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMaster_TaskByDocNum(string docNum)
        {
            List<TaskMasterSetupInfo> list = new List<TaskMasterSetupInfo>();
            try
            {
                BAL.TaskMasterManagement setupManagement = new BAL.TaskMasterManagement();
                list = setupManagement.GetTask_MasterByDocNum(docNum);

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("TaskMasterSetupController", "Exception occured on GetMaster_TaskByDocNum Controller, " + ex.Message);
            }
            return Json(new { response = list }, JsonRequestBehavior.AllowGet);
        }



        #endregion


        public ActionResult GetTaskMastersSetupLog(string docNum)
        {

            DataTable table = new DataTable();

            try
            {
                BAL.TaskMasterManagement setupManagement = new BAL.TaskMasterManagement();
                table = setupManagement.GetTaskMasterSetupLog();

                string jsonString = JsonConvert.SerializeObject(table, Formatting.Indented, new JsonSerializerSettings() { DateFormatString = "dd/MM/yyyy" });

                var jsonResult = Json(jsonString, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;

                return Json(new { response = jsonResult }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("SetupManagement", "Exception occured on GetTaskMasterSetup Controller, " + ex.Message);
                return Json(new { response = table }, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult GetTask_MasterAllDocuments()
        {
            DataTable table = new DataTable();
            try
            {
                BAL.TaskMasterManagement setupManagement = new BAL.TaskMasterManagement();
                table = setupManagement.GetTask_MasterAllDocumentsList();

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