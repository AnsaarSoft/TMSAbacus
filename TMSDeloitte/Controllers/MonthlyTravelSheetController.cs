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
    public class MonthlyTravelSheetController : BaseController
    {
        // GET: MonthlyTravelSheet
        public ActionResult Index(string docNum = "",string empID = "")
        {
            ViewBag.DocNum = docNum;
            ViewBag.EmpID = empID;

            if (docNum=="" && empID=="")
                ViewBag.isView = false;
            else
                ViewBag.isView = true;

            return View();
        }

        public ActionResult GetDocList(string empid)
        {
            BAL.MonthlyTravelSheetManagement timeMgt = new BAL.MonthlyTravelSheetManagement();
            BAL.Common cmn = new Common();
            List<string> list = new List<string>();
            List<TravelLocationInfo> locationList = new List<TravelLocationInfo>();
            try
            {
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
                designation= CurrentUser.SessionUser.DESIGNATIONNAME;

                list = timeMgt.GetDocNumByEmpID(employeeID);

                TravelLocationManagement travelMgt = new TravelLocationManagement();
                locationList = travelMgt.GetTravel_LocationSetup(0);

                return Json(new
                {
                    docList = list,
                    LocationList= locationList,
                    statusList = cmn.GetTimeSheetFormStatusList(),
                    EmpID = employeeID,//CurrentUser.SessionUser.ID,
                    EmpCode = empCode,//CurrentUser.SessionUser.EMPLOYEECODE,
                    EmpFullName = employeeFullName,//CurrentUser.SessionUser.FULLNAME
                    IsSuper = isSuper,
                    Department = department,
                    Designation=designation
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("MonthlyTravelSheetController", "Exception occured on GetAllDocumentNum Controller, " + ex.Message);

                return Json(new
                {
                    docList = list,
                    EmpID = 0,
                    EmpCode = "",
                    EmpFullName = "",
                    IsSuper = false,
                    Department = "",
                    Designation = ""
                }, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public ActionResult UploadImages(string year,string month)
        {
            string ImageURL = "";
            try
            {
                string domainName = Request.Url.GetLeftPart(UriPartial.Authority);

                if (Request.Files.Count > 0)
                {
                    try
                    {
                        //string guid = Guid.NewGuid().ToString();
                        //string ImageName = guid + ".png";

                        //string dir = ConfigurationManager.AppSettings["HostDomain"].ToString()+"/Images/";

                        string dir = Server.MapPath("~/Images/MonthlyTravelSheet/"+CurrentUser.SessionUser.EMPLOYEECODE+"_"+CurrentUser.SessionUser.ID+"/"+year+"_"+month);

                       

                        if (!Directory.Exists(dir))
                        {
                            Directory.CreateDirectory(dir);
                        }
                        HttpFileCollectionBase files = Request.Files;
                        for (int i = 0; i < files.Count; i++)
                        {
                            HttpPostedFileBase file = files[i];

                            ImageURL = Path.Combine(dir, file.FileName.Replace(" ","_"));

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
                ImageURL = domainName + "\\Images\\MonthlyTravelSheet" + "\\"+CurrentUser.SessionUser.EMPLOYEECODE + "_" + CurrentUser.SessionUser.ID + "\\" + year + "_" + month + "\\"  + arrImageURL[arrImageURL.Count() - 1];

                return Json(new { Success = true,Message = "", url = ImageURL }, JsonRequestBehavior.AllowGet);
                }

            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("MonthlyTravelSheetController", "Exception occured on Upload Image, " + ex.Message);


                return Json(new { Success = false, Message = "Exception occured on Upload Image" , url =""}, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public ActionResult RemoveImage(string year, string month,string fileName)
        {
            string ImageURL = "";
            try
            {
                string[] arrFilePath = fileName.Split('\\');
                fileName = arrFilePath[arrFilePath.Count()-1];

                string dir = Server.MapPath("~/Images/MonthlyTravelSheet/" + CurrentUser.SessionUser.EMPLOYEECODE + "_" + CurrentUser.SessionUser.ID + "/" + year + "_" + month);

                ImageURL = Path.Combine(dir, fileName);

                if (System.IO.File.Exists(ImageURL))
                {
                    System.IO.File.Delete(ImageURL);
                }
                else
                {
                    return Json(new { Success = false, Message = "File Not Found , Please refreash current page" }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { Success = true, Message = "Successfully Removed"}, JsonRequestBehavior.AllowGet);
            }

            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("MonthlyTravelSheetController", "Exception occured on Remove Image, " + ex.Message);


                return Json(new { Success = false, Message = "Exception occured on Remove Image" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetEmpMonthlyTravelSheet(int empID, string year,int month,string fromDate,string toDate)
        {
            MonthlyTravelSheet obj = new MonthlyTravelSheet();
            try
            {
                BAL.MonthlyTravelSheetManagement setupManagement = new BAL.MonthlyTravelSheetManagement();
                obj = setupManagement.GetEmpMonthlyTravelSheet(year,month,empID,fromDate,toDate);

                string domainName = Request.Url.GetLeftPart(UriPartial.Authority);
                string ImageURL = domainName + "\\Images\\MonthlyTravelSheet" + "\\" + obj.EmpCode + "_" + obj.EmpID + "\\" + obj.Year + "_" + obj.Month + "\\";
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
                log.InputOutputDocLog("ResourceBillingRateController", "Exception occured on GetResourceBillingRateByDocNum Controller, " + ex.Message);
            }
            return Json(new { response = obj }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddUpdateTimeSheetPeriod(MonthlyTravelSheet obj)
        {
            try
            {
                string msg = "Successfully Added/Updated";
                BAL.MonthlyTravelSheetManagement setupManagement = new BAL.MonthlyTravelSheetManagement();
                obj.CreatedBy = CurrentUser.SessionUser.ID;
                obj.UpdatedBy = CurrentUser.SessionUser.ID;
                return Json(new { Success = setupManagement.AddUpdateMonthlyTravelSheetDetailsSetup(obj, out msg), Message = msg }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("MonthlyTravelSheetController", "Exception occured on AddUpdateTimeSheetPeriod Controller, " + ex.Message);
                return Json(new { Success = false, Message = "Exception Occured When Add/Update Time Sheet Period!" }, JsonRequestBehavior.AllowGet);

            }

        }

        public ActionResult GetAllDocuments()
        {
            DataTable table = new DataTable();
            try
            {
                BAL.MonthlyTravelSheetManagement setupManagement = new BAL.MonthlyTravelSheetManagement();
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
                log.InputOutputDocLog("MonthlyTravelSheetController", "Exception occured on GetAllDocuments Controller, " + ex.Message);
                return Json(new { response = table }, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult GetAllDocumentsByEmpID()
        {
            List<MonthlyTravelSheet> list = new List<MonthlyTravelSheet>();
            try
            {
                BAL.MonthlyTravelSheetManagement setupManagement = new BAL.MonthlyTravelSheetManagement();
                list = setupManagement.GetAllMonthlyTravelSheetByEmpID(CurrentUser.SessionUser.ID);

                string jsonString = JsonConvert.SerializeObject(list, Formatting.Indented, new JsonSerializerSettings() { DateFormatString = "dd/MM/yyyy" });

                var jsonResult = Json(jsonString, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;

                return Json(new { response = jsonResult }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("MonthlyTravelSheetController", "Exception occured on GetAllDocuments Controller, " + ex.Message);
                return Json(new { response = list }, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult GetMonthlyTravelSheetByDocNum(string docNum,string EmpID, bool isView)
        {
            MonthlyTravelSheet obj = new MonthlyTravelSheet();
            Encrypt_Decrypt security = new Encrypt_Decrypt();
            int empid =0;
            try
            {
                if (isView)
                {
                    docNum = security.DecryptURLString(docNum);
                    empid = Convert.ToInt32(security.DecryptURLString(EmpID));
                }
                else
                    empid = Convert.ToInt32(EmpID);

                BAL.MonthlyTravelSheetManagement setupManagement = new BAL.MonthlyTravelSheetManagement();
                obj = setupManagement.GetMonthlyTravelSheetByDocNum(docNum, empid);

                string domainName = Request.Url.GetLeftPart(UriPartial.Authority);
               string ImageURL = domainName + "\\Images\\MonthlyTravelSheet" + "\\" + obj.EmpCode + "_" + obj.EmpID + "\\" + obj.Year + "_" + obj.Month + "\\";
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
                log.InputOutputDocLog("MonthlyTravelSheetController", "Exception occured on GetTimeSheetByDocNum Controller, " + ex.Message);
            }
            return Json(new { response = obj }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMonthlyTravelSheet(string docNum, string EmpID)
        {
            MonthlyTravelSheet obj = new MonthlyTravelSheet();
            Encrypt_Decrypt security = new Encrypt_Decrypt();
            try
            {
                docNum = security.DecryptString(docNum);
                //EmpID = security.DecryptString(EmpID);

                BAL.MonthlyTravelSheetManagement setupManagement = new BAL.MonthlyTravelSheetManagement();
                obj = setupManagement.GetMonthlyTravelSheetByDocNum(docNum, Convert.ToInt32(EmpID));
                string jsonString = JsonConvert.SerializeObject(obj, Formatting.Indented, new JsonSerializerSettings() { DateFormatString = "yyyy/MM/dd" });

                var jsonResult = Json(jsonString, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;

                return Json(new { response = jsonResult }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("MonthlyTravelSheetController", "Exception occured on GetTimeSheetByDocNum Controller, " + ex.Message);
            }
            return Json(new { response = obj }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult GeMonthlyTravelSheetLog(int docid)
        {
            DataTable dtHeader = new DataTable();
            DataTable dtDetail = new DataTable();
            DataTable dt_Group = new DataTable();

            try
            {
                BAL.MonthlyTravelSheetManagement mgt = new BAL.MonthlyTravelSheetManagement();
                DataSet ds= mgt.GetMonthlyTravelSheetLog(docid, CurrentUser.SessionUser.ID);

                if(ds.Tables.Count>0)
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
                log.InputOutputDocLog("MonthlyTravelSheetController", "Exception occured on GeMonthlyTravelSheetLog Controller, " + ex.Message);
            }
            return Json(new { Header = dtHeader, Detail = dtDetail }, JsonRequestBehavior.AllowGet);
        }

    }
}