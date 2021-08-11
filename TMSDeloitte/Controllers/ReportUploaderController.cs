using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TMSDeloitte.Helper;
using TMSDeloitte.Models;

namespace TMSDeloitte.Controllers
{
    public class ReportUploaderController : BaseController
    {
        // GET: ReportUploader
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetReports(int ID = 0)
        {
            BAL.ReportManagement reportSetup = new BAL.ReportManagement();
            List<ReportInfo> report = new List<ReportInfo>();
            try
            {
                report = reportSetup.GetReports(ID);
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ReportUploader", "Exception occured on GetReports Controller, " + ex.Message);
            }
            return Json(new { response = report }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddUpdateReports(ReportInfo ReportInfo, int ID)
        {
            bool suc = true;
            string msg = "Successfully Added/Updated";
            try
            {
                BAL.ReportManagement ReportManagement = new BAL.ReportManagement();
                string ff = Request.FilePath;
                //ReportInfo.RptFile = Server.MapPath(@"~/Rpt/") + ReportInfo.RptFile;
                ReportInfo.CreatedBy = CurrentUser.SessionUser.ID;
                ReportInfo.UpdatedBy = CurrentUser.SessionUser.ID;
                suc = ReportManagement.AddUpdateReports(out msg, ReportInfo, CurrentUser.SessionUser.ID, ID);
                return Json(new
                {
                    Success = suc,
                    Message = msg
                },
                    JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ReportUploader", "Exception occured on AddUpdateReports Controller, " + ex.Message);
                return Json(new { Success = false, Message = "Exception Occured When Add/Update Reports" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult DeleteReports(ReportInfo ReportInfo, int ID)
        {
            bool suc = true;
            string msg = "Successfully Deleted";
            try
            {
                BAL.ReportManagement ReportManagement = new BAL.ReportManagement();
                ReportInfo.CreatedBy = CurrentUser.SessionUser.ID;
                ReportInfo.UpdatedBy = CurrentUser.SessionUser.ID;
                suc = ReportManagement.DeleteReports(out msg, ReportInfo, CurrentUser.SessionUser.ID, ID);
                return Json(new
                {
                    Success = suc,
                    Message = msg
                },
                    JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ReportUploader", "Exception occured on DeleteReports Controller, " + ex.Message);
                return Json(new { Success = false, Message = "Exception Occured When DeleteReports Reports" }, JsonRequestBehavior.AllowGet);
            }
        }
        public static string strfileNames = "";
        public JsonResult Upload()
        {
            for (int i = 0; i < Request.Files.Count; i++)
            {
                HttpPostedFileBase file = Request.Files[i]; //Uploaded file
                                                            //Use the following properties to get file's name, size and MIMEType
                int fileSize = file.ContentLength;
                string fileName = file.FileName;
                string mimeType = file.ContentType;
                System.IO.Stream fileContent = file.InputStream;
                //To save file, use SaveAs method
                file.SaveAs(Server.MapPath(@"~/Rpt/") + fileName);
                strfileNames = fileName;//File will be saved in application root
            }
            //return Json("Uploaded " + Request.Files.Count + " files");
            return Json(strfileNames);
            //return Json(response = fileName);
            //return Json(new { response = fileName }, JsonRequestBehavior.AllowGet);
        }

    }
}