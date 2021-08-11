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
    public class AssignmentCostSetupController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetAllAssignmentCostSetup()
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

        [HttpPost]

        public ActionResult AddUpdateAssignmentCostSetup(List<AssignmentCostSetup> assignmentCostSetupList)
        {
            try
            {
                string msg = "Successfully Added/Updated";
                BAL.SetupManagement setupManagement = new BAL.SetupManagement();
                assignmentCostSetupList.Select(c => { c.CREATEDBY = CurrentUser.SessionUser.ID; c.UPDATEDEDBY = CurrentUser.SessionUser.ID; return c; }).ToList();
                return Json(new { Success = setupManagement.AddUpdateAssignmentCostSetup(assignmentCostSetupList,out msg), Message = msg }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = "Exception Occured When Add/Update Assignment Cost Setup" }, JsonRequestBehavior.AllowGet);
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("SetupManagement", "Exception occured on GetAssignmentCostSetup Controller, " + ex.Message);

            }

        }

        public ActionResult GetAssignmentCostSetupLog(string id)
        {
            DataTable table = new DataTable();
            try
            {
                BAL.SetupManagement setupManagement = new BAL.SetupManagement();
                table = setupManagement.GetAssignmentCostSetupLog();

                string jsonString = JsonConvert.SerializeObject(table, Formatting.Indented, new JsonSerializerSettings() { DateFormatString = "dd/MM/yyyy" });

                var jsonResult = Json(jsonString, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;

                return Json(new { response = jsonResult }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("SetupManagement", "Exception occured on GetAssignmentCostSetup Controller, " + ex.Message);
                return Json(new { response = table }, JsonRequestBehavior.AllowGet);
            }

        }


    }
}