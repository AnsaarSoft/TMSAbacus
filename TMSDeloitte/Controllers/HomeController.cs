using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using TMSDeloitte.BAL;
using TMSDeloitte.Helper;
using TMSDeloitte.Models;

namespace TMSDeloitte.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {



            Log log = new Log();
            Login login = new Login();
            login.remeber = false;
            try
            {
                try
                {
                    Encrypt_Decrypt EncryptDecrypt = new Encrypt_Decrypt();

                    string SERVERNODE = ConfigurationManager.AppSettings["SERVERNODE"];
                    string UID = ConfigurationManager.AppSettings["UID"];
                    string PWD = ConfigurationManager.AppSettings["PWD"];
                    string CS = ConfigurationManager.AppSettings["CS"];
                    string HANAConString = ConfigurationManager.AppSettings["HANAConString"];
                    HANAConString = HANAConString.Replace("[SERVERNODE]", EncryptDecrypt.DecryptString(SERVERNODE));
                    HANAConString = HANAConString.Replace("[UID]", EncryptDecrypt.DecryptString(UID));
                    HANAConString = HANAConString.Replace("[PWD]", EncryptDecrypt.DecryptString(PWD));
                    HANAConString = HANAConString.Replace("[CS]", EncryptDecrypt.DecryptString(CS));

                    log.ConnectionLogFile(HANAConString);


                    string SQL_DataSource = ConfigurationManager.AppSettings["SQL_DataSource"];
                    string SQL_DB = ConfigurationManager.AppSettings["SQL_DB"];
                    string SQL_User = ConfigurationManager.AppSettings["SQL_User"];
                    string SQL_Password = ConfigurationManager.AppSettings["SQL_Password"];

                    string SQLConString = ConfigurationManager.AppSettings["SQLConString"];
                    SQLConString = SQLConString.Replace("[SQL_DataSource]", EncryptDecrypt.DecryptString(SQL_DataSource));
                    SQLConString = SQLConString.Replace("[SQL_DB]", EncryptDecrypt.DecryptString(SQL_DB));
                    SQLConString = SQLConString.Replace("[SQL_User]", EncryptDecrypt.DecryptString(SQL_User));
                    SQLConString = SQLConString.Replace("[SQL_Password]", EncryptDecrypt.DecryptString(SQL_Password));

                    log.ConnectionLogFile(SQLConString);

                }
                catch (Exception ex)
                {
                    log.ConnectionLogFile(ex.Message);
                }



                Encrypt_Decrypt security = new Encrypt_Decrypt();
                var TMSCookiesRemember = Request.Cookies["TMSCookiesRemember"];

                if (TMSCookiesRemember != null)
                {
                    if (Convert.ToBoolean(TMSCookiesRemember.Value))
                    {
                        var TMSUserNameCookies = Request.Cookies["TMSUserNameCookies"];
                        var TMSUserPasswordCookies = Request.Cookies["TMSUserPasswordCookies"];

                        if (TMSUserNameCookies != null)
                            login.Username = security.DecryptString(TMSUserNameCookies.Value);
                        if (TMSUserPasswordCookies != null)
                            login.Password = security.DecryptString(TMSUserPasswordCookies.Value);

                        login.remeber = true;
                    }

                }
                if (System.Web.HttpContext.Current.Session["TMSUserSession"] != null)
                {
                    var TMSUserSession = (UserSession)System.Web.HttpContext.Current.Session["TMSUserSession"];
                    ViewBag.isLogin = TMSUserSession.SessionUser.ISLOGIN;

                    if (Convert.ToBoolean(TMSUserSession.SessionUser.ISLOGIN))
                        return RedirectToAction("Index", "Dashboard");
                    else
                    {
                        var TMSUserNameCookies = Request.Cookies["TMSUserNameCookies"];
                        var TMSUserPasswordCookies = Request.Cookies["TMSUserPasswordCookies"];

                        if (TMSUserNameCookies != null)
                            login.Username = "";
                        if (TMSUserPasswordCookies != null)
                            login.Password = "";
                    }
                }

            }
            catch (Exception ex)
            {

                log.LogFile(ex.Message);
                log.InputOutputDocLog("HomeController", "Exception occured on Login Page Coockies , " + ex.Message);
            }

            return View(login);
        }


        public ActionResult LoginUser(Login obj)
        {

            if (ModelState.IsValid)
            {

                UserSession sess = new UserSession();
                UserProfile UserProfile = new UserProfile();

                Session.Add("TMSUserSession", null);
                UserProfile.ISLOGIN = false;

                UserManagement Profile = new UserManagement();
                List<UserPermissions> userPermission = new List<UserPermissions>();
                List<int> TimeSheetViewList = new List<int>();
                List<int> DepartmentList = new List<int>();
                List<int> BranchList = new List<int>();

                bool isTimeSheetViewDataAccess = true;
                bool isBranchDataAccess = true;
                bool isDepartmentDataAccess = true;

                bool islogin = Profile.ValidateLogin(obj, out UserProfile, out userPermission, out TimeSheetViewList, out DepartmentList, out BranchList, out isTimeSheetViewDataAccess, out isDepartmentDataAccess, out isBranchDataAccess);

                UserProfile.ISLOGIN = islogin;
                sess.SessionUser = UserProfile;
                sess.pagelist = userPermission;

                //if (!TimeSheetViewList.Contains(Convert.ToInt32(UserProfile.DEPARTMENTID)))
                //    TimeSheetViewList.Add(Convert.ToInt32(UserProfile.DEPARTMENTID));

                //if (!DepartmentList.Contains(Convert.ToInt32(UserProfile.DEPARTMENTID)))
                //    DepartmentList.Add(Convert.ToInt32(UserProfile.DEPARTMENTID));

                //if (!BranchList.Contains(Convert.ToInt32(UserProfile.BRANCHID)))
                //    BranchList.Add(Convert.ToInt32(UserProfile.BRANCHID));

                sess.TimeSheetViewList = TimeSheetViewList;
                sess.DepartmentList = DepartmentList;
                sess.BranchList = BranchList;

                sess.isTimeSheetViewDataAccess = isTimeSheetViewDataAccess;
                sess.isDepartmentDataAccess = isDepartmentDataAccess;
                sess.isBranchDataAccess = isBranchDataAccess;
                string jsonString = JsonConvert.SerializeObject(sess);
                Log log = new Log();
                log.ConnectionLogFile(jsonString);
                Session.Add("TMSUserSession", sess);
                Session.Timeout = 180;

                if (!Convert.ToBoolean(sess.SessionUser.ISLOGIN))
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    if (obj.remeber)
                        AddCoockies(obj.Username, obj.Password, obj.remeber);
                    else
                        AddCoockies("", "", false);

                    return RedirectToAction("Index", "Dashboard");
                }

            }
            else
                return RedirectToAction("Index");

        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            Session.Add("TMSUserSession", null);
            return RedirectToAction("Index");
        }


        public void AddCoockies(string userName, string userPassword, bool remember)
        {
            try
            {
                Encrypt_Decrypt security = new Encrypt_Decrypt();

                HttpCookie TMSUserNameCookies = new HttpCookie("TMSUserNameCookies");
                TMSUserNameCookies.Value = security.EncryptString(userName);
                TMSUserNameCookies.Expires = DateTime.Now.AddDays(14);
                Response.SetCookie(TMSUserNameCookies);

                HttpCookie TMSUserPasswordCookies = new HttpCookie("TMSUserPasswordCookies");
                TMSUserPasswordCookies.Value = security.EncryptString(userPassword);
                TMSUserPasswordCookies.Expires = DateTime.Now.AddDays(14);
                Response.SetCookie(TMSUserPasswordCookies);

                HttpCookie TMSCookiesRemember = new HttpCookie("TMSCookiesRemember");
                TMSCookiesRemember.Value = Convert.ToString(remember);
                TMSCookiesRemember.Expires = DateTime.Now.AddDays(14);
                Response.SetCookie(TMSCookiesRemember);

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("HomeController", "Exception occured on AddCoockies , " + ex.Message);
            }

        }

        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ForgotPassword(string userName)
        {
            bool isSuccess = false;
            string msg = "";
            if (userName.Replace(" ", "") != "")
            {
                BAL.UserManagement userMgt = new UserManagement();
                isSuccess = userMgt.SendResetPasswordEmail(userName, out msg);
            }
            else
            {
                msg = "Please enter User Name to reset password!";
            }
            return Json(new { Success = isSuccess, Message = msg }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ResetPassword(string userId, string email, string dateTime)
        {
            try
            {
                string id = "";
                string userEmail = "";
                DateTime expireDateTime = new DateTime();
                ViewBag.isSuccess = false;
                Encrypt_Decrypt security = new Encrypt_Decrypt();
                try
                {
                    id = security.DecryptString(userId);
                    userEmail = security.DecryptString(email);
                    expireDateTime = Convert.ToDateTime(security.DecryptString(dateTime));

                    if (id == "" || userEmail == "" || dateTime == "" || userId == "" || email == "")
                    {
                        ViewBag.isSuccess = false;
                        ViewBag.userID = "";
                        ViewBag.userEmail = "";
                        ViewBag.Msg = "Invalid Url please contact admin to reset password!";
                    }
                    else
                    {
                        if (DateTime.Now <= expireDateTime)
                        {
                            UserProfile user = new UserProfile();
                            BAL.UserManagement userMgt = new UserManagement();

                            ViewBag.isSuccess = userMgt.ValidateUserByID(security.DecryptString(userId), out user);
                            if (ViewBag.isSuccess)
                            {
                                ViewBag.userID = userId;
                                ViewBag.userEmail = email;
                                ViewBag.Msg = "";
                            }
                            else
                            {
                                ViewBag.userID = "";
                                ViewBag.userEmail = "";
                                ViewBag.Msg = "Invalid Url please contact admin to reset password!";
                            }
                        }
                        else
                        {
                            ViewBag.isSuccess = false;
                            ViewBag.userID = "";
                            ViewBag.userEmail = "";
                            ViewBag.Msg = "Reset password link is expired!";
                        }

                    }


                }
                catch (Exception ex)
                {
                    ViewBag.isSuccess = false;
                    ViewBag.userID = "";
                    ViewBag.userEmail = "";
                    ViewBag.Msg = "Invalid Url please contact admin to reset password!";
                }

            }
            catch (Exception ex)
            {
                ViewBag.isSuccess = false;
                ViewBag.userID = "";
                ViewBag.userEmail = "";
                ViewBag.Msg = "Some exception occured please contact admin to reset password!";
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("HomeController", "Exception occured on ResetPassword Controller, userId: " + userId + " , email: " + email + Environment.NewLine + "Exception: " + ex.Message);
                return RedirectToAction("Index");
            }
            return View();
        }


        [HttpPost]
        public JsonResult ResetUserPassword(string userId, string userEmail, string newPassword, string confirmPass)
        {
            bool isSuccess = false;
            string message = "";
            Encrypt_Decrypt encDecObj = new Encrypt_Decrypt();
            try
            {
                UserManagement Profile = new UserManagement();
                UserProfile UserProfile = new UserProfile();

                if (confirmPass != newPassword)
                    return Json(new { Success = false, Msg = "New Password and Confirm Password Must Be Same!" }, JsonRequestBehavior.AllowGet);

                string Id = Convert.ToString(encDecObj.DecryptString(userId));
                string email = Convert.ToString(encDecObj.DecryptString(userEmail));

                if (Profile.ValidateUserByID(Id, out UserProfile))
                {
                    if (Profile.UpdateUserPassword(Convert.ToInt32(Id), newPassword, Convert.ToInt32(Id)))
                    {
                        isSuccess = true;
                        message = "Successfully updated user password";
                    }
                    else
                        message = "Exception occured in updating user passowrd please contact admin!";

                    return Json(new { Success = isSuccess, Msg = message }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Success = false, Msg = "Invalid Url please contact admin to reset password!" }, JsonRequestBehavior.AllowGet);
                }




            }
            catch (Exception ex)
            {
                message = "Exception occured in updating user passowrd please contact admin!";
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("HomeController", "Exception occured on ResetUserPassword Controller, " + ex.Message);
                return Json(new { Success = isSuccess, Msg = message }, JsonRequestBehavior.AllowGet);
            }



        }


        public ActionResult UpdatePassword()
        {
            if (System.Web.HttpContext.Current.Session["TMSUserSession"] != null)
            {
                var TMSUserSession = (UserSession)System.Web.HttpContext.Current.Session["TMSUserSession"];
                ViewBag.Username = TMSUserSession.SessionUser.USERNAME;
                ViewBag.UserID = TMSUserSession.SessionUser.ID;
            }
            else
                return RedirectToAction("Index");

            return View();
        }

        [HttpPost]
        public JsonResult UpdateUserPassword(string userPassword, string newPassword, string Username, int UserID)
        {
            bool isSuccess = false;
            string message = "";
            try
            {
                if (userPassword == newPassword)
                    return Json(new { Success = false, Msg = "Password And New Password Must Be Change" }, JsonRequestBehavior.AllowGet);

                Login obj = new Login();
                obj.Password = userPassword;
                obj.Username = Username;

                UserProfile UserProfile = new UserProfile();

                UserManagement Profile = new UserManagement();
                List<UserPermissions> userPermission = new List<UserPermissions>();
                List<int> TimeSheetViewList = new List<int>();
                List<int> DepartmentList = new List<int>();
                List<int> BranchList = new List<int>();

                bool isTimeSheetViewDataAccess = true;
                bool isBranchDataAccess = true;
                bool isDepartmentDataAccess = true;

                isSuccess = Profile.ValidateLogin(obj, out UserProfile, out userPermission, out TimeSheetViewList, out DepartmentList, out BranchList, out isTimeSheetViewDataAccess, out isDepartmentDataAccess, out isBranchDataAccess);

                if (isSuccess)
                {
                    Encrypt_Decrypt encDecObj = new Encrypt_Decrypt();

                    if (Profile.UpdateUserPassword(Convert.ToInt32(UserID), newPassword, Convert.ToInt32(UserID)))
                    {
                        isSuccess = true;
                        message = "Successfully updated user password login again";
                    }
                    else
                        message = "Exception occured in updating user passowrd";

                }
                else
                    message = "Invalid User Password";

                return Json(new { Success = isSuccess, Msg = message }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                message = "Exception occured in updating user passowrd";
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("HomeController", "Exception occured on GetAssignmentCostSetup Controller, " + ex.Message);
                return Json(new { Success = isSuccess, Msg = message }, JsonRequestBehavior.AllowGet);
            }



        }



        public ActionResult UploadLogo()
        {
            try
            {
                if (System.Web.HttpContext.Current.Session["TMSUserSession"] == null)
                    return RedirectToAction("Index", "Home");

                var TMSUserSession = (UserSession)System.Web.HttpContext.Current.Session["TMSUserSession"];
                if (!TMSUserSession.SessionUser.ISSUPER)
                    return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("HomeController", "Exception occured on UploadLogo, " + ex.Message);
            }


            return View();
        }

        [HttpPost]
        public ActionResult UploadInnerLogo()
        {
            Log log = new Log();
            string ImageURL = "";
            try
            {
                string domainName = Request.Url.GetLeftPart(UriPartial.Authority);

                if (Request.Files.Count > 0)
                {
                    try
                    {

                        string dir = Server.MapPath("~/assets/images/Logo");


                        if (!Directory.Exists(dir))
                        {
                            Directory.CreateDirectory(dir);
                        }
                        HttpFileCollectionBase files = Request.Files;
                        for (int i = 0; i < files.Count; i++)
                        {
                            HttpPostedFileBase file = files[i];

                            ImageURL = Path.Combine(dir, "innerLogo.png");

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

                log.InputOutputDocLog("UploadLogo", "Inner Logo Updated at  " + ImageURL);

                return Json(new { Success = true, Message = "", url = ImageURL }, JsonRequestBehavior.AllowGet);
            }

            catch (Exception ex)
            {

                log.LogFile(ex.Message);
                log.InputOutputDocLog("UploadLogo", "Exception occured on Upload Image, " + ex.Message);


                return Json(new { Success = false, Message = "Exception occured on Upload Image", url = "" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult OuterInnerLogo()
        {
            string ImageURL = "";
            Log log = new Log();
            try
            {
                string domainName = Request.Url.GetLeftPart(UriPartial.Authority);

                if (Request.Files.Count > 0)
                {
                    try
                    {

                        string dir = Server.MapPath("~/assets/images/Logo");


                        if (!Directory.Exists(dir))
                        {
                            Directory.CreateDirectory(dir);
                        }
                        HttpFileCollectionBase files = Request.Files;
                        for (int i = 0; i < files.Count; i++)
                        {
                            HttpPostedFileBase file = files[i];

                            ImageURL = Path.Combine(dir, "loginLogo.png");

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

                log.InputOutputDocLog("UploadLogo", "Outer Logo Updated at  " + ImageURL);

                return Json(new { Success = true, Message = "", url = ImageURL }, JsonRequestBehavior.AllowGet);
            }

            catch (Exception ex)
            {

                log.LogFile(ex.Message);
                log.InputOutputDocLog("UploadLogo", "Exception occured on Upload Image, " + ex.Message);


                return Json(new { Success = false, Message = "Exception occured on Upload Image", url = "" }, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult GetApprovalDecision(string empID, string docID, string docType)
        {
            BAL.Common cmn = new Common();
            try
            {
                if (!string.IsNullOrEmpty(empID) && !string.IsNullOrEmpty(docID) && !string.IsNullOrEmpty(docType))
                {
                    empID = empID.Replace("_", "+");
                    docID = docID.Replace("_", "+");
                    docType = docType.Replace("_", "+");

                    Encrypt_Decrypt security = new Encrypt_Decrypt();
                    if (!string.IsNullOrEmpty(empID))
                    {
                        //Getting User By Encrypted ID From Email.
                        empID = security.DecryptString(empID);
                        docID = security.DecryptString(docID);
                        docType = security.DecryptString(docType);

                        UserManagement userMgt = new UserManagement();
                        UserProfile user = userMgt.GetUserByID(Convert.ToInt32(empID));

                        //Creating User Session.
                        if (user != null)
                        {
                            UserSession sess = new UserSession();
                            UserProfile UserProfile = new UserProfile();

                            Session.Add("TMSUserSession", null);
                            UserProfile.ISLOGIN = false;

                            UserManagement Profile = new UserManagement();
                            List<UserPermissions> userPermission = new List<UserPermissions>();

                            List<int> TimeSheetViewList = new List<int>();
                            List<int> DepartmentList = new List<int>();
                            List<int> BranchList = new List<int>();

                            bool isTimeSheetViewDataAccess = true;
                            bool isBranchDataAccess = true;
                            bool isDepartmentDataAccess = true;

                            Login obj = new Login();
                            obj.Username = user.USERNAME;
                            obj.Password = security.DecryptString(user.PASSWORD);
                            bool islogin = Profile.ValidateLogin(obj, out UserProfile, out userPermission, out TimeSheetViewList, out DepartmentList, out BranchList, out isTimeSheetViewDataAccess, out isDepartmentDataAccess, out isBranchDataAccess);
                            if (islogin)
                            {

                                //if (!TimeSheetViewList.Contains(Convert.ToInt32(UserProfile.DEPARTMENTID)))
                                //    TimeSheetViewList.Add(Convert.ToInt32(UserProfile.DEPARTMENTID));

                                //if (!DepartmentList.Contains(Convert.ToInt32(UserProfile.DEPARTMENTID)))
                                //    DepartmentList.Add(Convert.ToInt32(UserProfile.DEPARTMENTID));

                                //if (!BranchList.Contains(Convert.ToInt32(UserProfile.BRANCHID)))
                                //    BranchList.Add(Convert.ToInt32(UserProfile.BRANCHID));

                                UserProfile.ISLOGIN = islogin;
                                sess.SessionUser = UserProfile;
                                sess.pagelist = userPermission;

                                sess.TimeSheetViewList = TimeSheetViewList;
                                sess.DepartmentList = DepartmentList;
                                sess.BranchList = BranchList;

                                sess.isTimeSheetViewDataAccess = isTimeSheetViewDataAccess;
                                sess.isDepartmentDataAccess = isDepartmentDataAccess;


                                sess.isBranchDataAccess = isBranchDataAccess;
                                Session.Add("TMSUserSession", sess);
                                Session.Timeout = 120;
                            }
                            return RedirectToAction("Index", "ApprovalDecision", new { docID = Convert.ToInt32(docID), docType });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ApprovalDecision", "Exception occured on GetApprovalDecision Controller, " + ex.Message);
            }
            return RedirectToAction("Index", "Home");
        }

        public ActionResult DocumentDecisionByEmail(string empID, string docID, string docType, string status)
        {
            BAL.Common cmn = new Common();
            string comment = "Document approved by email.";
            string msg = "";
            bool suc = true;
            try
            {
                if (!string.IsNullOrEmpty(empID) && !string.IsNullOrEmpty(docID) && !string.IsNullOrEmpty(docType))
                {
                    empID = empID.Replace("_", "+");
                    docID = docID.Replace("_", "+");
                    docType = docType.Replace("_", "+");
                    status = status.Replace("_", "+");

                    Encrypt_Decrypt security = new Encrypt_Decrypt();
                    if (!string.IsNullOrEmpty(empID))
                    {
                        //Getting User By Encrypted ID From Email.
                        empID = security.DecryptString(empID);
                        docID = security.DecryptString(docID);
                        docType = security.DecryptString(docType);
                        status = security.DecryptString(status);
                        if (status == "5")
                            comment = "Document rejected by email.";

                        UserManagement userMgt = new UserManagement();
                        UserProfile user = userMgt.GetUserByID(Convert.ToInt32(empID));

                        if (user != null)
                        {
                            ApprovalDecisionManagement appMgt = new ApprovalDecisionManagement();
                            List<UserProfile> usr = new List<UserProfile>();
                            List<ApprovalDecision> ApprovalDecisionInfo = appMgt.GetApprovalDecision(Convert.ToInt32(empID), usr);

                            BAL.ApprovalDecisionManagement ApprovalDecisionManagement = new BAL.ApprovalDecisionManagement();
                            suc = ApprovalDecisionManagement.UpdateApprovalDecision(out msg, docType, Convert.ToInt32(status), ApprovalDecisionInfo, user.ID, user.FULLNAME, Convert.ToInt32(docID), comment);

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ApprovalDecision", "Exception occured on DocumentDecisionByEmail Controller, " + ex.Message);
            }

            return RedirectToAction("DocumentDecision", new
            {
                msg = msg,
            });
        }

        public ActionResult DocumentDecision(string msg = "")
        {
            ViewBag.Msg = msg;

            return View();
        }
    }
}