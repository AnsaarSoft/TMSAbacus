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

    public class NonChargeableSetupController : BaseController
    {
        #region "NonChargableSetup"
        public ActionResult GetNonChargeable()
        {
            return View();
        }



        public ActionResult GetNonChargeableSetup()
        {
            List<NonChargableSetupInfo> NonChargableList = new List<NonChargableSetupInfo>();
            try
            {
                BAL.NonChargableSetupManagement NonChargableManagement = new BAL.NonChargableSetupManagement();
                NonChargableList = NonChargableManagement.GetNonChargeableSetup(0);

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("SetupManagement", "Exception occured on GetNonChargeableSetup Controller, " + ex.Message);
            }
            return Json(new { response = NonChargableList }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddUpdateGetNonChargeableSetup(List<NonChargableSetupInfo> NonChargableList)
        {
            string msg = "Successfully Added/Updated";
            try
            {
                BAL.NonChargableSetupManagement NonChargableManagement = new BAL.NonChargableSetupManagement();
                NonChargableList.Select(c => { c.CREATEDBY = CurrentUser.SessionUser.ID; c.UPDATEDEDBY = CurrentUser.SessionUser.ID; return c; }).ToList();
                return Json(new { Success = NonChargableManagement.AddUpdateNonChargableSetup(NonChargableList,out msg), Message =msg  }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = "Exception Occured When Add/Update GetNonChargeableSetup" }, JsonRequestBehavior.AllowGet);
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("SetupManagement", "Exception occured on AddUpdateGetNonChargeableSetup Controller, " + ex.Message);

            }

        }

        #endregion


        public ActionResult GetNonChargeablesSetupLog(string id)
        {

            DataTable table = new DataTable();

            try
            {
                BAL.NonChargableSetupManagement typeOfClaimManagement = new BAL.NonChargableSetupManagement();
                table = typeOfClaimManagement.GetNonChargeableSetupLog();

                string jsonString = JsonConvert.SerializeObject(table, Formatting.Indented, new JsonSerializerSettings() { DateFormatString = "dd/MM/yyyy" });

                var jsonResult = Json(jsonString, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;

                return Json(new { response = jsonResult }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("SetupManagement", "Exception occured on GetNonChargeablesSetupLog Controller, " + ex.Message);
                return Json(new { response = table }, JsonRequestBehavior.AllowGet);
            }

        }


    }
}