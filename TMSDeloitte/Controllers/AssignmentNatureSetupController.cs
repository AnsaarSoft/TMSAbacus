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
    public class AssignmentNatureSetupController : BaseController
    {
        // GET: AssignmentNatureSetup
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult AssignmentNatureSetup()
        {
            return View();
        }
        public ActionResult GetAllAssignmentNatureSetup()
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
                log.InputOutputDocLog("AssignmentNatureSetupController", "Exception occured on GetAssignmentNatureSetup Controller, " + ex.Message);
            }
            return Json(new { response = AssignmentNatureSetupList }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddUpdateAssignmentNatureSetup(List<AssignmentNatureSetup> AssignmentNatureSetupList)
        {
            string msg = "Successfully Added/Updated";
            try
            {
                BAL.cAssignmentNatureSetup setupManagement = new BAL.cAssignmentNatureSetup();
                AssignmentNatureSetupList.Select(c => { c.CreatedBy = CurrentUser.SessionUser.ID; c.UpdatedBy = CurrentUser.SessionUser.ID; return c; }).ToList();
                return Json(new { Success = setupManagement.AddUpdateAssignmentNatureSetup(AssignmentNatureSetupList, out msg), Message = msg }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = "Exception Occured When Add/Update Assignment Nature Setup" }, JsonRequestBehavior.AllowGet);
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("AssignmentNatureSetupController", "Exception occured on GetAssignmentNatureSetup Controller, " + ex.Message);

            }

        }

        public ActionResult GetAssignmentNatureSetupLog()
        {

            DataTable table = new DataTable();

            try
            {
                BAL.cAssignmentNatureSetup typeOfClaimManagement = new BAL.cAssignmentNatureSetup();
                table = typeOfClaimManagement.GetAssignmentNatureSetupLog();

                string jsonString = JsonConvert.SerializeObject(table, Formatting.Indented, new JsonSerializerSettings() { DateFormatString = "dd/MM/yyyy" });

                var jsonResult = Json(jsonString, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;

                return Json(new { response = jsonResult }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("AssignmentNatureSetupController", "Exception occured on GetTypeOfClaimsSetupLog Controller, " + ex.Message);
                return Json(new { response = table }, JsonRequestBehavior.AllowGet);
            }

        }

    }
}