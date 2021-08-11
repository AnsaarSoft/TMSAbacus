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
    public class TypeOfClaimSetupController : BaseController
    {

        // GET: TypeOfClaimSetup
        #region "TypeOfClaimSetup"
        public ActionResult GetTypeOfClaim()
        {
            return View();
        }


        
        public ActionResult GetTypeOfClaimSetup()
        {
            List<TypeOfClaimSetupInfo> TypeOfClaimList = new List<TypeOfClaimSetupInfo>();
            try
            {
                BAL.TypeOfClaimManagement TypeOfClaimManagement = new BAL.TypeOfClaimManagement();
                TypeOfClaimList = TypeOfClaimManagement.GetTypeOfClaimSetup(0);

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("SetupManagement", "Exception occured on GetTypeOfClaimSetup Controller, " + ex.Message);
            }
            return Json(new { response = TypeOfClaimList }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddUpdateGetTypeOfClaimSetup(List<TypeOfClaimSetupInfo> TypeOfClaimList)
        {
            string msg = "Successfully Added/Updated";
            try
            {
                BAL.TypeOfClaimManagement TypeOfClaimManagement = new BAL.TypeOfClaimManagement();
                TypeOfClaimList.Select(c => { c.CREATEDBY = CurrentUser.SessionUser.ID; c.UPDATEDEDBY = CurrentUser.SessionUser.ID; return c; }).ToList();
                //TypeOfClaimList.Select(c => { c.CREATEDBY = 1; c.UPDATEDEDBY = 1; return c; }).ToList();
                return Json(new { Success = TypeOfClaimManagement.AddUpdateTypeOfClaimSetup(TypeOfClaimList,out msg), Message = msg }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = "Exception Occured When Add/Update GetTypeOfClaimSetup" }, JsonRequestBehavior.AllowGet);
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("SetupManagement", "Exception occured on AddUpdateGetTypeOfClaimSetup Controller, " + ex.Message);

            }

        }

        #endregion


        public ActionResult GetTypeOfClaimsSetupLog(string id)
        {

            DataTable table = new DataTable();

            try
            {
                BAL.TypeOfClaimManagement typeOfClaimManagement = new BAL.TypeOfClaimManagement();
                table = typeOfClaimManagement.GetTypeOfClaimSetupLog();

                string jsonString = JsonConvert.SerializeObject(table, Formatting.Indented, new JsonSerializerSettings() { DateFormatString = "dd/MM/yyyy" });

                var jsonResult = Json(jsonString, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;

                return Json(new { response = jsonResult }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("SetupManagement", "Exception occured on GetTypeOfClaimsSetupLog Controller, " + ex.Message);
                return Json(new { response = table }, JsonRequestBehavior.AllowGet);
            }

        }



    }
}