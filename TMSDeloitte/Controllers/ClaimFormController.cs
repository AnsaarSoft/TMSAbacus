using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TMSDeloitte.BAL;
using TMSDeloitte.Helper;
using TMSDeloitte.Models;

namespace TMSDeloitte.Controllers
{
    public class ClaimFormController : BaseController
    {
        // GET: ClaimForm
        public ActionResult Index(string docNum = "", string empID = "")
        {
            ViewBag.DocNum = docNum;
            ViewBag.EmpID = empID;

            if (docNum == "" && empID == "")
                ViewBag.isView = false;
            else
                ViewBag.isView = true;

            return View();
        }

        public ActionResult GetDocList(string empid)
        {
            List<string> list = new List<string>();
            List<TypeOfClaimSetupInfo> TypeOfClaimList = new List<TypeOfClaimSetupInfo>();
            BAL.TimeSheetFormManagement timeSheetMgt = new TimeSheetFormManagement();
            DataTable dtAssignmentList = new DataTable();
            try
            {
                BAL.ClaimFormManagement timeMgt = new BAL.ClaimFormManagement();
                BAL.Common cmn = new Common();
                BAL.TypeOfClaimManagement TypeOfClaimManagement = new BAL.TypeOfClaimManagement();

                int employeeID = 0;
                string empCode = ""; ;
                int branchID = 0; ;
                string employeeFullName = "";
                bool isSuper = false;
                string department = "";
                string designation = "";

                employeeID = CurrentUser.SessionUser.ID;
                empCode = CurrentUser.SessionUser.EMPLOYEECODE;
                branchID = Convert.ToInt32(CurrentUser.SessionUser.BRANCHID);
                employeeFullName = CurrentUser.SessionUser.FULLNAME;
                isSuper = CurrentUser.SessionUser.ISSUPER;
                department = CurrentUser.SessionUser.BRANCHNAME;
                designation = CurrentUser.SessionUser.DESIGNATIONNAME;

                list = timeMgt.GetDocNumByEmpID(employeeID);
                TypeOfClaimList = TypeOfClaimManagement.GetTypeOfClaimSetup(0);

                dtAssignmentList = timeSheetMgt.GetAssignmentByEmpID(employeeID);
                string jsonString_dtAssignmentList = JsonConvert.SerializeObject(dtAssignmentList, Formatting.Indented, new JsonSerializerSettings() { DateFormatString = "dd/MM/yyyy HH:MM:s" });
                var jsonResult_dtAssignmentList = Json(jsonString_dtAssignmentList, JsonRequestBehavior.AllowGet);
                jsonResult_dtAssignmentList.MaxJsonLength = int.MaxValue;
                return Json(new
                {
                    docList = list,
                    statusList = cmn.GetTimeSheetFormStatusList(),
                    EmpID = employeeID,//CurrentUser.SessionUser.ID,
                    EmpCode = empCode,//CurrentUser.SessionUser.EMPLOYEECODE,
                    EmpFullName = employeeFullName,//CurrentUser.SessionUser.FULLNAME
                    IsSuper = isSuper,
                    Department = department,
                    Designation = designation,
                    ImageFolderName = Guid.NewGuid(),
                    TypeOfClaimList= TypeOfClaimList,
                    AssignmentList = jsonResult_dtAssignmentList,
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ClaimFormController", "Exception occured on GetAllDocumentNum Controller, " + ex.Message);

                return Json(new
                {
                    docList = list,
                    EmpID = 0,
                    EmpCode = "",
                    EmpFullName = "",
                    IsSuper = false,
                    Department = "",
                    Designation = "",
                    ImageFolderName = "",
                    TypeOfClaimList= TypeOfClaimList
                }, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public ActionResult UploadImages(string ImageFolderName)
        {
            string ImageURL = "";
            try
            {
                string domainName = Request.Url.GetLeftPart(UriPartial.Authority);

                if (Request.Files.Count > 0)
                {
                    try
                    {
                        string guid = Guid.NewGuid().ToString();
                        string ImageName = guid + ".png";

                        //string dir = ConfigurationManager.AppSettings["HostDomain"].ToString()+"/Images/";
                        string dir = Server.MapPath("~/Images/ClaimForm/" + CurrentUser.SessionUser.EMPLOYEECODE + "_" + CurrentUser.SessionUser.ID+"/"+ ImageFolderName);



                        if (!Directory.Exists(dir))
                        {
                            Directory.CreateDirectory(dir);
                        }
                        HttpFileCollectionBase files = Request.Files;
                        for (int i = 0; i < files.Count; i++)
                        {
                            HttpPostedFileBase file = files[i];

                            ImageURL = Path.Combine(dir, file.FileName.Replace(" ", "_"));

                            file.SaveAs(ImageURL);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                }
                else
                {
                    return Json(new { Success = false, Message = "No image selected.", url = "" }, JsonRequestBehavior.AllowGet);

                }

                string[] arrImageURL = ImageURL.Split('\\');
                ImageURL = domainName + "\\Images\\ClaimForm" + "\\" + CurrentUser.SessionUser.EMPLOYEECODE + "_" + CurrentUser.SessionUser.ID + "\\" + ImageFolderName + "\\" + arrImageURL[arrImageURL.Count() - 1];


                return Json(new { Success = true, Message = "", url = ImageURL }, JsonRequestBehavior.AllowGet);
            }

            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ClaimFormController", "Exception occured on Upload Image, " + ex.Message);


                return Json(new { Success = false, Message = "Exception occured on Upload Image", url = "" }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public ActionResult RemoveImage(string ImageFolderName, string fileName)
        {
            string ImageURL = "";
            try
            {
                string[] arrFilePath = fileName.Split('\\');
                fileName = arrFilePath[arrFilePath.Count() - 1];

                string dir = Server.MapPath("~/Images/ClaimForm/" + CurrentUser.SessionUser.EMPLOYEECODE + "_" + CurrentUser.SessionUser.ID + "/" +ImageFolderName);

                ImageURL = Path.Combine(dir, fileName);

                if (System.IO.File.Exists(ImageURL))
                {
                    System.IO.File.Delete(ImageURL);
                }
                else
                {
                    return Json(new { Success = false, Message = "Image Not Found , Please refreash current page" }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { Success = true, Message = "Successfully Removed" }, JsonRequestBehavior.AllowGet);
            }

            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ClaimFormController", "Exception occured on Remove Image, " + ex.Message);


                return Json(new { Success = false, Message = "Exception occured on Remove Image" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetTimeSheetByEmpIDandYear(int empID, string year)
        {
            GenerateTimeSheet obj = new GenerateTimeSheet();
            try
            {
                BAL.TimeSheetPeriodManagement setupManagement = new BAL.TimeSheetPeriodManagement();
                obj = setupManagement.GetTimeSheetByEmpIDandYear(empID, year);
                string jsonString = JsonConvert.SerializeObject(obj, Formatting.Indented, new JsonSerializerSettings() { DateFormatString = "yyyy/MM/dd" });

                var jsonResult = Json(jsonString, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;

                return Json(new { response = jsonResult }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ResourceBillingRateController", "Exception occured on GetResourceBillingRateByDocNum Controller, " + ex.Message);
            }
            return Json(new { response = obj }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddUpdateClaimForm(ClaimForm obj)
        {
            try
            {
                string msg = "Successfully Added/Updated";
                BAL.ClaimFormManagement setupManagement = new BAL.ClaimFormManagement();
                obj.CreatedBy = CurrentUser.SessionUser.ID;
                obj.UpdatedBy = CurrentUser.SessionUser.ID;
                return Json(new { Success = setupManagement.AddUpdateClaimFormDetailsSetup(obj, out msg), Message = msg }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ClaimFormController", "Exception occured on AddUpdateTimeSheetPeriod Controller, " + ex.Message);
                return Json(new { Success = false, Message = "Exception Occured When Add/Update Time Sheet Period!" }, JsonRequestBehavior.AllowGet);

            }

        }

        public ActionResult GetAllDocuments()
        {
            DataTable table = new DataTable();
            try
            {
                BAL.ClaimFormManagement setupManagement = new BAL.ClaimFormManagement();
                table = setupManagement.GetAllDocumentsList();

                string jsonString = JsonConvert.SerializeObject(table, Formatting.Indented, new JsonSerializerSettings() { DateFormatString = "ddd, dd MMM yyy" });

                var jsonResult = Json(jsonString, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;

                return Json(new { response = jsonResult }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ClaimFormController", "Exception occured on GetAllDocuments Controller, " + ex.Message);
                return Json(new { response = table }, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult GetAllDocumentsByEmpID()
        {
            List<ClaimForm> list = new List<ClaimForm>();
            try
            {
                BAL.ClaimFormManagement setupManagement = new BAL.ClaimFormManagement();
                list = setupManagement.GetAllClaimFormByEmpID(CurrentUser.SessionUser.ID);

                string jsonString = JsonConvert.SerializeObject(list, Formatting.Indented, new JsonSerializerSettings() { DateFormatString = "dd/MM/yyyy" });

                var jsonResult = Json(jsonString, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;

                return Json(new { response = jsonResult }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ClaimFormController", "Exception occured on GetAllDocuments Controller, " + ex.Message);
                return Json(new { response = list }, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult GetClaimFormByDocNum(string docNum, string EmpID, bool isView)
        {
            ClaimForm obj = new ClaimForm();
            Encrypt_Decrypt security = new Encrypt_Decrypt();
            int empid = 0;
            try
            {
                if (isView)
                {
                    docNum = security.DecryptURLString(docNum);
                    empid = Convert.ToInt32(security.DecryptURLString(EmpID));
                }
                else
                    empid = Convert.ToInt32(EmpID);

                BAL.ClaimFormManagement setupManagement = new BAL.ClaimFormManagement();
                obj = setupManagement.GetClaimFormByDocNum(docNum, empid);
                string domainName = Request.Url.GetLeftPart(UriPartial.Authority);
                string ImageURL = domainName + "\\Images\\ClaimForm" + "\\" + CurrentUser.SessionUser.EMPLOYEECODE + "_" + CurrentUser.SessionUser.ID + "\\" + obj.ImageFolder+"\\";
                obj._AttachementsURL = ImageURL;


                string jsonString = JsonConvert.SerializeObject(obj, Formatting.Indented, new JsonSerializerSettings() { DateFormatString = "yyyy/MM/dd" });

                var jsonResult = Json(jsonString, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;

                return Json(new { response = jsonResult }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ClaimFormController", "Exception occured on GetTimeSheetByDocNum Controller, " + ex.Message);
            }
            return Json(new { response = obj }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetClaimForm(string docNum, string EmpID)
        {
            ClaimForm obj = new ClaimForm();
            Encrypt_Decrypt security = new Encrypt_Decrypt();
            try
            {
                docNum = security.DecryptString(docNum);
                //EmpID = security.DecryptString(EmpID);

                BAL.ClaimFormManagement setupManagement = new BAL.ClaimFormManagement();
                obj = setupManagement.GetClaimFormByDocNum(docNum, Convert.ToInt32(EmpID));
                string jsonString = JsonConvert.SerializeObject(obj, Formatting.Indented, new JsonSerializerSettings() { DateFormatString = "yyyy/MM/dd" });

                var jsonResult = Json(jsonString, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;

                return Json(new { response = jsonResult }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ClaimFormController", "Exception occured on GetTimeSheetByDocNum Controller, " + ex.Message);
            }
            return Json(new { response = obj }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult GeClaimFormLog(int docid)
        {
            DataTable dtHeader = new DataTable();
            DataTable dtDetail = new DataTable();
            DataTable dt_Group = new DataTable();

            try
            {
                BAL.ClaimFormManagement mgt = new BAL.ClaimFormManagement();
                DataSet ds = mgt.GetClaimFormLog(docid, CurrentUser.SessionUser.ID);

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
                log.InputOutputDocLog("ClaimFormController", "Exception occured on GeClaimFormLog Controller, " + ex.Message);
            }
            return Json(new { Header = dtHeader, Detail = dtDetail }, JsonRequestBehavior.AllowGet);
        }

    }
}