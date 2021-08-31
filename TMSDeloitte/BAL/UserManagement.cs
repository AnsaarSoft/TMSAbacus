using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TMSDeloitte.DAL;
using TMSDeloitte.Helper;
using TMSDeloitte.Models;

namespace TMSDeloitte.BAL
{

    public class UserManagement
    {
        public bool ValidateLogin(Login login, out UserProfile userProfile, out List<UserPermissions> userPermission, out List<int> TimeSheetViewList, out List<int> DepartmentList, out List<int> BranchList, out bool isTimeSheetViewDataAccess, out bool isDepartmentDataAccess, out bool isBranchDataAccess)
        {
            userPermission = new List<UserPermissions>();
            userProfile = new UserProfile();
            TimeSheetViewList = new List<int>();
            DepartmentList = new List<int>();
            BranchList = new List<int>();
            bool isSuccess = false;
            isTimeSheetViewDataAccess = true;
            isBranchDataAccess = true;
            isDepartmentDataAccess = true;

            try
            {
                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                DataTable dt_UserProfile = HANADAL.GetDataTableByStoredProcedure("GetUserProfileByCredential", TranslateLoginToParameterList(login), "UserManagement");
                if (dt_UserProfile.Rows.Count > 0)
                {
                    isSuccess = true;
                    userProfile = TranslateDataTableToUserProfile(dt_UserProfile);
                    if (userProfile.ISSUPER)
                    {
                        userPermission = GetSuperUserPermission();
                        GetSuperUserDataAccess(ref TimeSheetViewList, ref DepartmentList, ref BranchList);
                    }

                    else
                    {
                        userPermission = GetUserPermission(userProfile.ID, Convert.ToInt32(userProfile.DESIGNATIONID));
                        GetNormalUserDataAccess(userProfile.ID, ref TimeSheetViewList, ref DepartmentList, ref BranchList, ref isTimeSheetViewDataAccess, ref isDepartmentDataAccess, ref isBranchDataAccess);
                    }

                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("UserManagament", "Exception occured on ValidateLogin, " + ex.Message);
            }
            return isSuccess;
        }

        public List<UserPermissions> GetUserPermission(int ID, int designationID)
        {
            List<UserPermissions> userPermission = new List<UserPermissions>();
            try
            {
                List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();

                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = Convert.ToString(ID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);
                parm = new B1SP_Parameter();
                parm.ParameterName = "GROUPID";
                parm.ParameterValue = Convert.ToString(designationID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);
                DataTable dt_AuthByUserID = new DataTable();
                DataTable dt_AuthByGroupID = new DataTable();
                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                DataSet ds = HANADAL.GetDataSetByStoredProcedure("GetUserAuthorizationByID", parmList, "UserManagement");
                dt_AuthByUserID = ds.Tables[0];
                dt_AuthByGroupID = ds.Tables[1];
                if (dt_AuthByGroupID.Rows.Count > 0)
                {
                    userPermission = TranslateDataTableToUserPermissionList(dt_AuthByGroupID);
                }
                else
                {
                    if (dt_AuthByUserID.Rows.Count > 0)
                    {
                        userPermission = TranslateDataTableToUserPermissionList(dt_AuthByUserID);
                    }
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("UserManagament", "Exception occured on GetUserPermission, " + ex.Message);
            }
            return userPermission;
        }

        public List<UserPermissions> GetSuperUserPermission()
        {
            List<UserPermissions> list = new List<UserPermissions>();
            Common cmn = new Common();
            List<TMS_Menu> menuList = cmn.GetMenuList();
            try
            {
                foreach (var menu in menuList)
                {
                    UserPermissions perm = new UserPermissions();
                    perm.ID = menu.ID;
                    perm.HeadID = menu.Head_ID;
                    perm.Order = menu.Index;
                    perm.PageName = menu.Name;
                    perm.PageURL = menu.PageURL;
                    perm.Role = 1;
                    list.Add(perm);
                }
                list = list.OrderBy(x => x.Order).ToList();
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("UserManagament", "Exception occured on GetAdminUserPermission, " + ex.Message);
            }

            return list;
        }


        public void GetSuperUserDataAccess(ref List<int> TimeSheetViewList, ref List<int> DepartmentList, ref List<int> BranchList)
        {
            try
            {
                HCMOneManagement mgt = new HCMOneManagement();

                List<Department> allDepartments = mgt.GetAllHCMDepartment();
                List<Branch> allLocation = mgt.GetAllHCMBranch();

                foreach (Department item in allDepartments)
                {
                    TimeSheetViewList.Add(item.Id);
                }
                foreach (Department item in allDepartments)
                {
                    DepartmentList.Add(item.Id);

                }

                foreach (Branch item in allLocation)
                {
                    BranchList.Add(item.Id);
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("UserManagament", "Exception occured on GetSuperUserDataAccess, " + ex.Message);
            }

        }

        public void GetNormalUserDataAccess(int UserID, ref List<int> TimeSheetViewList, ref List<int> DepartmentList, ref List<int> BranchList, ref bool isTimeSheetViewDataAccess, ref bool isDepartmentDataAccess, ref bool isBranchDataAccess)
        {
            try
            {
                int DocID = 0;
                string DocNum = "";
                List<UserDataAccess_Menu> authList = new List<UserDataAccess_Menu>();
                UserDataAccess userDataAccess = new UserDataAccess();
                userDataAccess.GetDataAccessByUserID(Convert.ToString(UserID), out DocID, out DocNum, out authList);
                authList = authList.Where(x => x.IsChecked == true && x.Authorization == 0).ToList();
                if (authList.Count > 0)
                {
                    HCMOneManagement mgt = new HCMOneManagement();
                    var allTimeSheetViewList = authList.Where(x => (x.ID < 20000 && x.ID > 10000) || x.Head_ID == 1).ToList();
                    if (allTimeSheetViewList.Count > 0)
                    {
                        foreach (var item in allTimeSheetViewList)
                        {
                            if (item.ID == 1)
                            {
                                TimeSheetViewList = new List<int>();
                                List<Department> allDepartments = mgt.GetAllHCMDepartment();
                                foreach (Department itemDepart in allDepartments)
                                {
                                    TimeSheetViewList.Add(itemDepart.Id);
                                }
                            }
                            else
                            {
                                TimeSheetViewList.Add(Convert.ToInt32(Convert.ToString(item.ID).Replace("1000", "0")));
                            }
                        }
                    }
                    else
                        isTimeSheetViewDataAccess = false;


                    var allDepartmentList = authList.Where(x => (x.ID > 20000 && x.ID < 30000) || x.Head_ID == 2).ToList();
                    if (allDepartmentList.Count > 0)
                    {
                        foreach (var item in allDepartmentList)
                        {
                            if (item.ID == 2)
                            {
                                DepartmentList = new List<int>();
                                List<Department> allDepartments = mgt.GetAllHCMDepartment();
                                foreach (Department itemDepart in allDepartments)
                                {
                                    DepartmentList.Add(itemDepart.Id);
                                }
                            }
                            else
                            {
                                DepartmentList.Add(Convert.ToInt32(Convert.ToString(item.ID).Replace("2000", "0")));
                            }
                        }
                    }
                    else
                        isDepartmentDataAccess = false;

                    var allBranchList = authList.Where(x => (x.ID > 30000 && x.ID < 40000) || x.Head_ID == 3).ToList();
                    if (allBranchList.Count > 0)
                    {
                        foreach (var item in allBranchList)
                        {
                            if (item.ID == 3)
                            {
                                BranchList = new List<int>();
                                List<Branch> allLocation = mgt.GetAllFilteredHCMBranch(BranchList);
                                foreach (Branch itemLoc in allLocation)
                                {
                                    BranchList.Add(itemLoc.Id);
                                }
                            }
                            else
                            {
                                BranchList.Add(Convert.ToInt32(Convert.ToString(item.ID).Replace("3000", "0")));
                            }
                        }
                    }
                    else
                        isBranchDataAccess = false;


                }
                else
                {
                    isTimeSheetViewDataAccess = false;
                    isDepartmentDataAccess = false;
                    isBranchDataAccess = false;
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("UserManagament", "Exception occured on GetNormalUserDataAccess, " + ex.Message);
            }

        }

        public bool UpdateUserPassword(int id, string password, int updateBy)
        {
            bool isSuccess = false;
            try
            {
                bool isUpdateOccured = false;
                AddUser_Log(GetUserByID(id), out isUpdateOccured);

                List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
                Encrypt_Decrypt security = new Encrypt_Decrypt();
                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = Convert.ToString(id);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);
                parm = new B1SP_Parameter();
                parm.ParameterName = "PASSWORD";
                parm.ParameterValue = password == "" ? "" : security.EncryptString(Convert.ToString(password));
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);
                parm = new B1SP_Parameter();
                parm.ParameterName = "UPDATEDEDBY";
                parm.ParameterValue = Convert.ToString(updateBy);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);
                parm = new B1SP_Parameter();
                parm.ParameterName = "UPDATEDDATE";
                parm.ParameterValue = DateTime.Now.ToString("yyyy-MM-dd HH:MM:s");
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                DataTable dt = HANADAL.AddUpdateDataByStoredProcedure("UpdateUserPassword", parmList, "UserManagement");

                if (dt.Rows.Count == 0)
                    isSuccess = false;
                else
                {
                    isSuccess = true;

                    Common cmn = new Common();
                    Task.Run(() => cmn.AddMasterLog(nameof(Enums.FormsName.UserManagement), nameof(Enums.FormsOperations.Update), updateBy, "UserManagement"));

                }


            }
            catch (Exception ex)
            {
                isSuccess = false;
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("UserManagement", "Exception occured when UpdateUserPassword , ID:" + id + " , password:" + password + " , updatedBy:" + updateBy + ", " + ex.Message);

            }

            return isSuccess;
        }
        public bool ValidateUserByEmail(string email, out UserProfile userProfile)
        {
            userProfile = new UserProfile();
            bool isSuccess = false;
            try
            {
                List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();

                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "Email";
                parm.ParameterValue = Convert.ToString(email);
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                DataTable dt_UserProfile = HANADAL.GetDataTableByStoredProcedure("GetUserProfileByUserName", parmList, "UserManagement");
                if (dt_UserProfile.Rows.Count > 0)
                {
                    isSuccess = true;
                    userProfile = TranslateDataTableToUserProfile(dt_UserProfile);
                }

            }
            catch (Exception ex)
            {
                isSuccess = false;
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("UserManagament", "Exception occured on ValidateUserByEmail, " + ex.Message);
            }
            return isSuccess;
        }
        public bool ValidateUserByUserName(string UserName, out UserProfile userProfile)
        {
            userProfile = new UserProfile();
            bool isSuccess = false;
            try
            {
                List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();

                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "UserName";
                parm.ParameterValue = Convert.ToString(UserName);
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                DataTable dt_UserProfile = HANADAL.GetDataTableByStoredProcedure("GetUserProfileByUserName", parmList, "UserManagement");
                if (dt_UserProfile.Rows.Count > 0)
                {
                    isSuccess = true;
                    userProfile = TranslateDataTableToUserProfile(dt_UserProfile);
                }

            }
            catch (Exception ex)
            {
                isSuccess = false;
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("UserManagament", "Exception occured on ValidateUserByEmail, " + ex.Message);
            }
            return isSuccess;
        }

        public bool ValidateUserByID(string id, out UserProfile userProfile)
        {
            userProfile = new UserProfile();
            bool isSuccess = false;
            try
            {
                List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();

                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = Convert.ToString(id);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);
                //parm = new B1SP_Parameter();
                //parm.ParameterName = "Email";
                //parm.ParameterValue = Convert.ToString(email);
                //parmList.Add(parm);

                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                DataTable dt_UserProfile = HANADAL.GetDataTableByStoredProcedure("GetUserProfileByID", parmList, "UserManagement");
                if (dt_UserProfile.Rows.Count > 0)
                {
                    isSuccess = true;
                    userProfile = TranslateDataTableToUserProfile(dt_UserProfile);
                }

            }
            catch (Exception ex)
            {
                isSuccess = false;
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("UserManagament", "Exception occured on ValidateUserByEmail, " + ex.Message);
            }
            return isSuccess;
        }

        public UserProfile GetUserByID(int id)
        {
            UserProfile userProfile = new UserProfile();
            try
            {
                List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();

                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = Convert.ToString(id);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                DataTable dt_UserProfile = HANADAL.GetDataTableByStoredProcedure("GetUserProfileByID", parmList, "UserManagement");
                if (dt_UserProfile.Rows.Count > 0)
                {
                    userProfile = TranslateDataTableToUserProfile(dt_UserProfile);
                }

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("UserManagament", "Exception occured on ValidateUserByEmail, " + ex.Message);
            }
            return userProfile;
        }

        public bool SendResetPasswordEmail(string userName, out string msg)
        {
            msg = "";
            bool isSuccess = false;
            UserProfile userProfile = new UserProfile();
            try
            {
                if (ValidateUserByUserName(userName, out userProfile))
                {

                    if (Convert.ToBoolean(userProfile.ISSUPER) || !Convert.ToBoolean(userProfile.ISACTIVE))
                    {
                        isSuccess = false;
                        msg = "Please contact admin to reset password.";
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(userProfile.EMAIL))
                        {
                            Encrypt_Decrypt security = new Encrypt_Decrypt();

                            string encryptedID = security.EncryptString(Convert.ToString(userProfile.ID));
                            string encryptedEmail = security.EncryptString(Convert.ToString(userProfile.EMAIL));
                            string expireDateTime = security.EncryptString(DateTime.Now.AddMinutes(15).ToString("yyyy-MM-dd HH:MM:ss"));
                            // var lnkHref = "<a href='" + Url.Action("ResetPassword", "Account", new { email = UserName, code = token }, "http") + "'>Reset Password</a>";

                            var requestContext = HttpContext.Current.Request.RequestContext;
                            string lnkHref = new UrlHelper(requestContext).Action("ResetPassword", "Home", new { userId = encryptedID, email = encryptedEmail, dateTime = expireDateTime }, HttpContext.Current.Request.Url.Scheme);
                            string subject = "Reset Password Request";
                            //string body = "<b>Please find the Password Reset Link. </b><br/>" + lnkHref;
                            string detail = "Click on the link given below to reset your password.";
                            string body = "<b>" + detail + ". </b><br/><a href='" + lnkHref + "'>Reset Password</a>";

                            Email email = new Email();
                            isSuccess = email.SendEmail(userProfile.EMAIL, body, subject, null, out msg);
                        }
                        else
                        {
                            isSuccess = false;
                            msg = "User has invalid email address.";
                        }

                    }



                }
                else
                {
                    isSuccess = false;
                    msg = "User Name Not Exist.";
                }


            }
            catch (Exception ex)
            {
                isSuccess = false;
                msg = "Exception occured on reset password please contact admin.";
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("UserManagament", "Exception occured on SendResetPasswordEmail, userName: " + userName + Environment.NewLine + "Exception: " + ex.Message);
            }

            return isSuccess;
        }

        public bool ValidateUserName(string userName, out UserProfile userProfile)
        {
            userProfile = new UserProfile();
            bool isSuccess = false;
            try
            {
                List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();

                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "UserName";
                parm.ParameterValue = Convert.ToString(userName);
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                DataTable dt_UserProfile = HANADAL.GetDataTableByStoredProcedure("GetUserName", parmList, "UserManagement");
                if (dt_UserProfile.Rows.Count > 0)
                {
                    isSuccess = true;
                    userProfile = TranslateDataTableToUserProfile(dt_UserProfile);
                }

            }
            catch (Exception ex)
            {
                isSuccess = false;
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("UserManagament", "Exception occured on ValidateUserName, " + ex.Message);
            }
            return isSuccess;
        }

        public List<UserProfile> GetAllUsers(bool IncludeIsSuper = true)
        {
            List<UserProfile> userProfiles = new List<UserProfile>();
            try
            {
                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                DataTable dt_UserProfile = HANADAL.GetDataTableByStoredProcedure("GetAllUser", "UserManagement");
                if (dt_UserProfile.Rows.Count > 0)
                {
                    userProfiles = TranslateDataTableToUserProfileList(dt_UserProfile);
                    if (IncludeIsSuper == false)
                        userProfiles = userProfiles.Where(x => x.ISSUPER == false).ToList();
                }

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("UserManagament", "Exception occured on GetAllUsers, " + ex.Message);
            }
            return userProfiles;
        }


        public List<UserProfile> GetAllTimeSheetFilteredUsersByDepartAndBranch(int empID, List<int> TimesheetListList, List<int> BranchList, bool isTimseSheetDataAccess, bool isBranchDataAccess)
        {
            List<UserProfile> userProfiles = new List<UserProfile>();
            List<UserProfile> FilteredUsers = new List<UserProfile>();
            try
            {
                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                DataTable dt_UserProfile = HANADAL.GetDataTableByStoredProcedure("GetAllUser", "UserManagement");
                if (dt_UserProfile.Rows.Count > 0)
                {
                    userProfiles = TranslateDataTableToUserProfileList(dt_UserProfile);
                    if (userProfiles.Count > 0)
                    {
                        if (isTimseSheetDataAccess && isBranchDataAccess)
                        {
                            var filterByTimeSheet = userProfiles.Where(item => TimesheetListList.Contains(Convert.ToInt32(item.DEPARTMENTID)));
                            foreach (var user in filterByTimeSheet)
                            {
                                FilteredUsers.Add(user);
                            }

                            var filterByBranch = FilteredUsers.Where(item => BranchList.Contains(Convert.ToInt32(item.BRANCHID)));
                            foreach (var user in filterByBranch)
                            {
                                var isExist = FilteredUsers.Where(x => x.ID == user.ID).FirstOrDefault();
                                if (isExist == null)
                                    FilteredUsers.Add(user);
                            }

                        }
                        else
                        {
                            if (isTimseSheetDataAccess)
                            {
                                var filterByTimeSheet = userProfiles.Where(item => TimesheetListList.Contains(Convert.ToInt32(item.DEPARTMENTID)));
                                foreach (var user in filterByTimeSheet)
                                {
                                    FilteredUsers.Add(user);
                                }
                            }
                            if (isBranchDataAccess)
                            {
                                var filterByBranch = userProfiles.Where(item => BranchList.Contains(Convert.ToInt32(item.BRANCHID)));
                                foreach (var user in filterByBranch)
                                {
                                    var isExist = FilteredUsers.Where(x => x.ID == user.ID).FirstOrDefault();
                                    if (isExist == null)
                                        FilteredUsers.Add(user);
                                }
                            }

                            var selfUser = userProfiles.Where(x => x.ID == empID).FirstOrDefault();
                            if (FilteredUsers.Count == 0)
                                FilteredUsers.Add(selfUser);
                            else
                            {
                                var isSelfUserExist = FilteredUsers.Where(x => x.ID == empID).FirstOrDefault();
                                if (isSelfUserExist == null)
                                    FilteredUsers.Add(selfUser);
                            }

                        }
                    }

                }

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("UserManagament", "Exception occured on GetAllUsers, " + ex.Message);
            }
            return FilteredUsers;
        }
        public List<UserProfile> GetAllFilteredUsersByDepartAndBranch(int empID, List<int> DepartmentList, List<int> BranchList, bool isDepartmentDataAccess, bool isBranchDataAccess)
        {
            List<UserProfile> userProfiles = new List<UserProfile>();
            List<UserProfile> FilteredUsers = new List<UserProfile>();
            try
            {
                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                DataTable dt_UserProfile = HANADAL.GetDataTableByStoredProcedure("GetAllUser", "UserManagement");

                if (dt_UserProfile.Rows.Count > 0)
                {
                    userProfiles = TranslateDataTableToUserProfileList(dt_UserProfile);
                    if (userProfiles.Count > 0)
                    {
                        if (isDepartmentDataAccess && isBranchDataAccess)
                        {
                            var filterByDepart = userProfiles.Where(item => DepartmentList.Contains(Convert.ToInt32(item.DEPARTMENTID)));
                            FilteredUsers = filterByDepart.ToList();
                            //foreach (var user in filterByDepart)
                            //{
                            //    FilteredUsers.Add(user);
                            //}
                            var filterByBranch = FilteredUsers.Where(item => BranchList.Contains(Convert.ToInt32(item.BRANCHID)));
                            FilteredUsers = filterByBranch.ToList();
                            //foreach (var user in filterByBranch)
                            //{
                            //    var isExist = FilteredUsers.Where(x => x.ID == user.ID).FirstOrDefault();
                            //    if (isExist == null)
                            //        FilteredUsers.Add(user);
                            //}
                        }
                        else
                        {
                            if (isDepartmentDataAccess)
                            {
                                var filterByDepart = userProfiles.Where(item => DepartmentList.Contains(Convert.ToInt32(item.DEPARTMENTID)));
                                FilteredUsers = filterByDepart.ToList();
                                //foreach (var user in filterByDepart)
                                //{
                                //    FilteredUsers.Add(user);
                                //}
                            }
                            if (isBranchDataAccess)
                            {
                                var filterByBranch = userProfiles.Where(item => BranchList.Contains(Convert.ToInt32(item.BRANCHID)));
                                FilteredUsers = filterByBranch.ToList();
                                //foreach (var user in filterByBranch)
                                //{
                                //    var isExist = FilteredUsers.Where(x => x.ID == user.ID).FirstOrDefault();
                                //    if (isExist == null)
                                //        FilteredUsers.Add(user);
                                //}
                            }
                            var selfUser = userProfiles.Where(x => x.ID == empID).FirstOrDefault();
                            if (FilteredUsers.Count == 0)
                                FilteredUsers.Add(selfUser);
                            else
                            {
                                var isSelfUserExist = FilteredUsers.Where(x => x.ID == empID).FirstOrDefault();
                                if (isSelfUserExist == null)
                                    FilteredUsers.Add(selfUser);
                            }

                        }
                    }

                }

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("UserManagament", "Exception occured on GetAllUsers, " + ex.Message);
            }
            return FilteredUsers;
        }
        public List<UserProfile> GetAllFilteredUsersByDepart(int empID, List<int> DepartmentList, bool isDepartmentDataAccess)
        {
            List<UserProfile> userProfiles = new List<UserProfile>();
            List<UserProfile> FilteredUsers = new List<UserProfile>();
            try
            {
                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                DataTable dt_UserProfile = HANADAL.GetDataTableByStoredProcedure("GetAllUser", "UserManagement");
                if (dt_UserProfile.Rows.Count > 0)
                {
                    userProfiles = TranslateDataTableToUserProfileList(dt_UserProfile);
                    if (userProfiles.Count > 0)
                    {
                        if (isDepartmentDataAccess)
                        {
                            var filterByTimeSheetView = userProfiles.Where(item => DepartmentList.Contains(Convert.ToInt32(item.DEPARTMENTID)));
                            foreach (var user in filterByTimeSheetView)
                            {
                                FilteredUsers.Add(user);
                            }
                        }
                        var selfUser = userProfiles.Where(x => x.ID == empID).FirstOrDefault();
                        if (FilteredUsers.Count == 0)
                            FilteredUsers.Add(selfUser);
                        else
                        {
                            var isSelfUserExist = FilteredUsers.Where(x => x.ID == empID).FirstOrDefault();
                            if (isSelfUserExist == null)
                                FilteredUsers.Add(selfUser);
                        }

                    }

                }

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("UserManagament", "Exception occured on GetAllUsers, " + ex.Message);
            }
            return FilteredUsers;
        }
        public List<UserProfile> GetAllFilteredUsersByBranch(int empID, List<int> BranchList, bool isBranchDataAccess)
        {
            List<UserProfile> userProfiles = new List<UserProfile>();
            List<UserProfile> FilteredUsers = new List<UserProfile>();
            try
            {
                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                DataTable dt_UserProfile = HANADAL.GetDataTableByStoredProcedure("GetAllUser", "UserManagement");
                if (dt_UserProfile.Rows.Count > 0)
                {
                    userProfiles = TranslateDataTableToUserProfileList(dt_UserProfile);
                    if (userProfiles.Count > 0)
                    {
                        if (isBranchDataAccess)
                        {
                            var filterByBranch = userProfiles.Where(item => BranchList.Contains(Convert.ToInt32(item.BRANCHID)));
                            foreach (var user in filterByBranch)
                            {
                                FilteredUsers.Add(user);
                            }
                        }
                        var selfUser = userProfiles.Where(x => x.ID == empID).FirstOrDefault();
                        if (FilteredUsers.Count == 0)
                            FilteredUsers.Add(selfUser);
                        else
                        {
                            var isSelfUserExist = FilteredUsers.Where(x => x.ID == empID).FirstOrDefault();
                            if (isSelfUserExist == null)
                                FilteredUsers.Add(selfUser);
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("UserManagament", "Exception occured on GetAllUsers, " + ex.Message);
            }
            return FilteredUsers;
        }

        public List<HCM_Employee> GetAllFilteredHCMUsersByDepartAndBranch(int empID, List<int> DepartmentList, List<int> BranchList, bool isDepartmentDataAccess, bool isBranchDataAccess)
        {
            List<HCM_Employee> userProfiles = new List<HCM_Employee>();
            List<HCM_Employee> FilteredUsers = new List<HCM_Employee>();
            try
            {
                BAL.HCMOneManagement setupManagement = new BAL.HCMOneManagement();
                userProfiles = setupManagement.GetAllHCMUser();
                if (userProfiles.Count > 0)
                {
                    if (isDepartmentDataAccess && isBranchDataAccess)
                    {
                        var filterByDepart = userProfiles.Where(item => DepartmentList.Contains(Convert.ToInt32(item.DepartmentID)));
                        FilteredUsers = filterByDepart.ToList();
                        //foreach (var user in filterByDepart)
                        //{
                        //    FilteredUsers.Add(user);
                        //}
                        var filterByBranch = FilteredUsers.Where(item => BranchList.Contains(Convert.ToInt32(item.BranchID)));
                        FilteredUsers = filterByBranch.ToList();
                        //foreach (var user in filterByBranch)
                        //{
                        //    var isExist = FilteredUsers.Where(x => x.ID == user.ID).FirstOrDefault();
                        //    if (isExist == null)
                        //        FilteredUsers.Add(user);
                        //}
                    }
                    else
                    {
                        if (isDepartmentDataAccess)
                        {
                            var filterByDepart = userProfiles.Where(item => DepartmentList.Contains(Convert.ToInt32(item.DepartmentID)));
                            FilteredUsers = filterByDepart.ToList();
                            //foreach (var user in filterByDepart)
                            //{
                            //    FilteredUsers.Add(user);
                            //}
                        }
                        if (isBranchDataAccess)
                        {
                            var filterByBranch = userProfiles.Where(item => BranchList.Contains(Convert.ToInt32(item.BranchID)));
                            FilteredUsers = filterByBranch.ToList();
                            //foreach (var user in filterByBranch)
                            //{
                            //    var isExist = FilteredUsers.Where(x => x.ID == user.ID).FirstOrDefault();
                            //    if (isExist == null)
                            //        FilteredUsers.Add(user);
                            //}
                        }
                        var selfUser = userProfiles.Where(x => x.ID == empID).FirstOrDefault();
                        if (FilteredUsers.Count == 0)
                            FilteredUsers.Add(selfUser);
                        else
                        {
                            var isSelfUserExist = FilteredUsers.Where(x => x.ID == empID).FirstOrDefault();
                            if (isSelfUserExist == null)
                                FilteredUsers.Add(selfUser);
                        }

                    }
                }

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("UserManagament", "Exception occured on GetAllUsers, " + ex.Message);
            }
            return FilteredUsers;
        }

        public bool GetUserByEmpCode(string empCode, out UserProfile userProfile)
        {
            userProfile = new UserProfile();
            bool isSuccess = false;
            try
            {
                List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();

                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "EmpCode";
                parm.ParameterValue = Convert.ToString(empCode);
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                Common cmn = new Common();
                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                DataTable dt_UserProfile = HANADAL.GetDataTableByStoredProcedure("GetUserByEmpCode", parmList, "UserManagement");
                Encrypt_Decrypt security = new Encrypt_Decrypt();
                userProfile.EMPLOYEECODE = empCode;
                userProfile = TranslateDataTableToUserProfile(dt_UserProfile, empCode);
                try { userProfile.PASSWORD = security.DecryptString(userProfile.PASSWORD); } catch (Exception ex) { }


                if (userProfile != null)
                {
                    isSuccess = true;
                }

            }
            catch (Exception ex)
            {
                isSuccess = false;
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("UserManagament", "Exception occured on ValidateUserByEmail, " + ex.Message);
            }
            return isSuccess;
        }

        public bool AddUpdateUser(UserProfile userProfile, out string Msg)
        {
            bool isSuccess = false;
            bool isAddOccured = false;
            bool isUpdateOccured = false;
            bool isDeleteOccured = false;
            Msg = "Successfully Added/Updated";
            try
            {
                if (userProfile.ID == 0)
                {
                    UserProfile existingUser = new UserProfile();
                    if (ValidateUserName(userProfile.USERNAME, out existingUser))
                    {
                        Msg = "Username already registered!";
                        return false;
                    }
                }

                ////For Form Log
                AddUser_Log(userProfile, out isUpdateOccured);

                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                DataTable dt_AssignmentCostSetup = HANADAL.AddUpdateDataByStoredProcedure("AddUpdateUser", TranslateUserProfileToParameterList(userProfile), "UserManagement");
                if (dt_AssignmentCostSetup.Rows.Count > 0)
                {
                    isSuccess = true;
                }

                ////For Master Log
                if (userProfile.ID == 0)
                    isAddOccured = true;
                if (userProfile.ISDELETED == true)
                    isDeleteOccured = true;

                int createdBy = createdBy = Convert.ToInt32(userProfile.CREATEDBY);


                Common cmn = new Common();
                Task.Run(() => cmn.AddMasterLog(isAddOccured, isUpdateOccured, isDeleteOccured, nameof(Enums.FormsName.UserManagement), createdBy, "ResourceBillingRatesSetupManagement"));
                //// End MAster Log


                isSuccess = true;
            }
            catch (Exception ex)
            {
                Msg = "Exception occured when add/update user!";
                isSuccess = false;
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("UserManagement", "Exception occured on AddUpdateUser, " + ex.Message);
            }

            return isSuccess;
        }

        public void AddUser_Log(UserProfile newObject, out bool isUpdateOccured)
        {
            isUpdateOccured = false;
            try
            {
                if (newObject.ID > 0)
                {
                    UserProfile previousObject = GetUserByID(Convert.ToInt32(newObject.ID));
                    List<B1SP_Parameter> paramList = TranslateUserProfileLogToParameterList(newObject);
                    //List<B1SP_Parameter> paramList = new List<B1SP_Parameter>();
                    //B1SP_Parameter parm = new B1SP_Parameter();
                    bool isChangeOccured = false;
                    if (previousObject != null)
                    {
                        foreach (PropertyCompareResult resultItem in PropertyCompare.Compare(previousObject, newObject))
                        {
                            isChangeOccured = true;
                            isUpdateOccured = true;
                            string Name = resultItem.Name;
                            object PreviousValue = resultItem.OldValue;
                            object NewValue = resultItem.NewValue;

                            switch (Name)
                            {

                                case "USERNAME":
                                    paramList.Where(x => x.ParameterName == "USERNAME_PREVIOUS").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                    paramList.Where(x => x.ParameterName == "USERNAME_NEW").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();

                                    break;
                                case "PASSWORD":
                                    paramList.Where(x => x.ParameterName == "PASSWORD_PREVIOUS").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                    paramList.Where(x => x.ParameterName == "PASSWORD_NEW").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();

                                    break;

                                case "ISSUPER":
                                    paramList.Where(x => x.ParameterName == "ISSUPER_PREVIOUS").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                    paramList.Where(x => x.ParameterName == "ISSUPER_NEW").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();

                                    break;
                                case "ISACTIVE":
                                    paramList.Where(x => x.ParameterName == "ISACTIVE_PREVIOUS").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                    paramList.Where(x => x.ParameterName == "ISACTIVE_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();

                                    break;
                                case "ISDELETED":
                                    paramList.Where(x => x.ParameterName == "ISDELETED_PREVIOUS").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                    paramList.Where(x => x.ParameterName == "ISDELETED_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();

                                    break;
                            }

                        }

                        if (isChangeOccured)
                        {
                            HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                            Task.Run(() => HANADAL.AddUpdateDataByStoredProcedure("AddUser_Log", paramList, "UserManagement"));
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("UserManagement", "Exception occured on AddUser_Log, " + ex.Message);
            }
        }

        public DataTable GetUserLogByUserID(string ID)
        {
            DataTable dt = new DataTable();
            Encrypt_Decrypt security = new Encrypt_Decrypt();
            try

            {
                List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();

                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = Convert.ToString(ID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                dt = HANADAL.GetDataTableByStoredProcedure("GetUSerProfileLogByUserID", parmList, "SetupManagement");
                foreach (DataRow dr in dt.Rows)
                {
                    try { dr["PASSWORD_PREVIOUS"] = security.DecryptString(Convert.ToString(dr["PASSWORD_PREVIOUS"] == DBNull.Value ? "" : Convert.ToString(dr["PASSWORD_PREVIOUS"]))); } catch (Exception) { }
                    try { dr["PASSWORD_NEW"] = security.DecryptString(Convert.ToString(dr["PASSWORD_NEW"] == DBNull.Value ? "" : Convert.ToString(dr["PASSWORD_NEW"]))); } catch (Exception) { }
                }
                if (dt.Rows.Count > 0)
                {
                    dt.Columns.Remove("ISDELETED_PREVIOUS");
                    dt.Columns.Remove("ISDELETED_NEW");
                }

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("SetupManagement", "Exception occured on GetAllAssignmentCostSetup, " + ex.Message);
            }

            return dt;
        }

        public string GetUserEmailByID(int id)
        {
            string email = "";
            try
            {
                string empCode = "";
                List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();

                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = Convert.ToString(id);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                DataTable dt_UserProfile = HANADAL.GetDataTableByStoredProcedure("GetUserProfileByID", parmList, "UserManagement");
                if (dt_UserProfile.Rows.Count > 0)
                {
                    empCode = dt_UserProfile.Rows[0]["EMPLOYEECODE"].ToString();
                    if (!string.IsNullOrEmpty(empCode))
                    {
                        BAL.UserManagement mgts = new BAL.UserManagement();

                        HCM_Employee emp = mgts.GetHCMOneEmployeeByCode(empCode);
                        if (emp != null)
                        {
                            email = emp.OfficeEmail;
                        }
                    }
                }
                return email;
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("UserManagament", "Exception occured on ValidateUserByEmail, " + ex.Message);
            }
            return email;

        }

        #region "Translation User Management"

        private List<B1SP_Parameter> TranslateLoginToParameterList(Login login)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {
                if (login == null)
                    return null;

                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "UserName";
                parm.ParameterValue = login.Username;
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                Encrypt_Decrypt EncryptDecrypt = new Encrypt_Decrypt();
                parm = new B1SP_Parameter();
                parm.ParameterName = "Password";
                parm.ParameterType = DBTypes.String.ToString();
                parm.ParameterValue = EncryptDecrypt.EncryptString(login.Password);
                // parm.ParameterValue = (login.Password);
                parmList.Add(parm);
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("UserManagament", "Exception occured on TranslateLoginToParameterList, " + ex.Message);
            }


            return parmList;
        }


        private UserProfile TranslateDataTableToUserProfile(DataTable dt, string empCode = null)
        {
            UserProfile userProfile = new UserProfile();
            Encrypt_Decrypt security = new Encrypt_Decrypt();
            try
            {
                HCMOneManagement mgt = new HCMOneManagement();

                string EmployeeCode = "";

                foreach (DataRow dtRow in dt.Rows)
                {
                    userProfile.CREATEDBY = Convert.ToInt32(dtRow["CREATEDBY"]);
                    userProfile.CREATEDDATE = Convert.ToDateTime(dtRow["CREATEDDATE"]);
                    userProfile.UPDATEDEDBY = Convert.ToInt32(dtRow["UPDATEDEDBY"]);
                    userProfile.UPDATEDDATE = Convert.ToDateTime(dtRow["UPDATEDDATE"]);
                    userProfile.EMPLOYEECODE = Convert.ToString(dtRow["EMPLOYEECODE"]);
                    userProfile.ID = Convert.ToInt32(dtRow["ID"]);
                    userProfile.ISACTIVE = Convert.ToBoolean(dtRow["ISACTIVE"]);
                    userProfile.ISDELETED = Convert.ToBoolean(dtRow["ISDELETED"]);
                    userProfile.ISSUPER = Convert.ToBoolean(dtRow["ISSUPER"]);
                    userProfile.PASSWORD = Convert.ToString(dtRow["PASSWORD"]);
                    userProfile.USERNAME = Convert.ToString(dtRow["USERNAME"]);
                    userProfile.OBAPPROVEDHOURS = dtRow["OBAPPROVEDHOURS"] is DBNull ? 0 : Convert.ToInt32(dtRow["OBAPPROVEDHOURS"]);
                    userProfile.OBASONDATE = dtRow["OBASONDATE"] is DBNull ? DateTime.MinValue : Convert.ToDateTime(dtRow["OBASONDATE"]);
                    userProfile.OBOVERTIMEHOURS = dtRow["OBOVERTIMEHOURS"] is DBNull ? 0 : Convert.ToInt32(dtRow["OBOVERTIMEHOURS"]);
                }
                if (string.IsNullOrEmpty(userProfile.EMPLOYEECODE))
                    EmployeeCode = empCode;
                else
                    EmployeeCode = userProfile.EMPLOYEECODE;

                if (!string.IsNullOrEmpty(EmployeeCode))
                {
                    HCM_Employee emp = GetHCMOneEmployeeByCode(EmployeeCode);
                    if (emp != null)
                    {
                        userProfile.HCMOneID = emp.ID;
                        userProfile.DEPARTMENTID = emp.DepartmentID;
                        userProfile.DEPARTMENTNAME = emp.DepartmentName;
                        userProfile.DESIGNATIONID = emp.DesignationID;
                        userProfile.DESIGNATIONNAME = emp.DesignationName;
                        userProfile.LOCATIONID = emp.LocationID;
                        userProfile.LOCATIONNAME = emp.LocationName;
                        userProfile.BRANCHID = emp.BranchID;
                        userProfile.BRANCHNAME = emp.BranchName;
                        userProfile.EMAIL = emp.OfficeEmail;
                        userProfile.FULLNAME = emp.EmpName;
                        userProfile.DETAILNAME = emp.EmpDetailName;
                        userProfile.FAX = emp.Fax;
                        if (emp.UserCode != "" && emp.PassCode != "")
                        {
                            userProfile.HCMONELINK = Convert.ToString(ConfigurationManager.AppSettings["HCMOneLink"]);
                            //userProfile.HCMONELINK = userProfile.HCMONELINK.Replace("[UserName]", security.EncryptURLString(emp.UserCode)).Replace("[pwd]", security.EncryptURLString(emp.PassCode));
                            userProfile.HCMONELINK = userProfile.HCMONELINK.Replace("[UserName]", emp.UserCode).Replace("[pwd]", emp.PassCode);
                        }
                        else
                        {
                            userProfile.HCMONELINK = "#";
                        }

                    }
                }


            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("UserManagament", "Exception occured on TranslateDataTableToUserProfile, " + ex.Message);
            }

            return userProfile;
        }

        public HCM_Employee GetHCMOneEmployeeByCode(string empCode)
        {
            HCMOneManagement mgt = new HCMOneManagement();

            return mgt.GetHCMUserByCode(empCode);
        }

        private List<UserProfile> TranslateDataTableToUserProfileList(DataTable dt)
        {
            List<UserProfile> list = new List<UserProfile>();
            HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();

            try
            {
                List<HCM_Employee> empList = new List<HCM_Employee>();
                if (dt.Rows.Count > 0)
                {
                    HCMOneManagement mgt = new HCMOneManagement();
                    empList = mgt.GetAllHCMUser();
                }

                foreach (DataRow dtRow in dt.Rows)
                {
                    UserProfile userProfile = new UserProfile();
                    userProfile.CREATEDBY = Convert.ToInt32(dtRow["CREATEDBY"]);
                    userProfile.CREATEDDATE = Convert.ToDateTime(dtRow["CREATEDDATE"]);
                    userProfile.UPDATEDEDBY = Convert.ToInt32(dtRow["UPDATEDEDBY"]);
                    userProfile.UPDATEDDATE = Convert.ToDateTime(dtRow["UPDATEDDATE"]);
                    userProfile.EMPLOYEECODE = Convert.ToString(dtRow["EMPLOYEECODE"]);
                    userProfile.ID = Convert.ToInt32(dtRow["ID"]);
                    userProfile.ISACTIVE = Convert.ToBoolean(dtRow["ISACTIVE"]);
                    userProfile.ISDELETED = Convert.ToBoolean(dtRow["ISDELETED"]);
                    userProfile.ISSUPER = Convert.ToBoolean(dtRow["ISSUPER"]);
                    userProfile.PASSWORD = Convert.ToString(dtRow["PASSWORD"]);
                    userProfile.USERNAME = Convert.ToString(dtRow["USERNAME"]);
                    string EmployeeCode = userProfile.EMPLOYEECODE;

                    if (!string.IsNullOrEmpty(EmployeeCode))
                    {
                        //HCM_Employee emp = GetHCMOneEmployeeByCode(EmployeeCode);
                        HCM_Employee emp = new HCM_Employee();
                        emp = empList.Where(x => x.EmpID == EmployeeCode).FirstOrDefault();
                        if (emp != null)
                        {
                            userProfile.DEPARTMENTID = emp.DepartmentID;
                            userProfile.DEPARTMENTNAME = emp.DepartmentName;
                            userProfile.DESIGNATIONID = emp.DesignationID;



                            //int iDesignationId = Convert.ToInt32(userProfile.DESIGNATIONID);
                            //DataTable dtBillingRate = HANADAL.GetDataTableByStoredProcedure("GetResourceBillingRatesByDesignationID", TranslateIDToParameterList(iDesignationId), "UserManagament");

                            //if (dtBillingRate.Rows.Count > 0)
                            //{
                            //    string strBillingRate = Convert.ToString(dtBillingRate.Rows[0]["RatesPerHour"]);
                            //    if (strBillingRate != "" && strBillingRate != null)
                            //    {
                            //        userProfile.BillingRatesPerHour = Convert.ToDouble(strBillingRate);
                            //    }
                            //}
                            //else
                            //{
                            //    userProfile.BillingRatesPerHour = 0.00;
                            //}

                            userProfile.FAX = emp.Fax;
                            userProfile.DESIGNATIONNAME = emp.DesignationName;
                            userProfile.LOCATIONID = emp.LocationID;
                            userProfile.LOCATIONNAME = emp.LocationName;
                            userProfile.BRANCHID = emp.BranchID;
                            userProfile.BRANCHNAME = emp.BranchName;
                            userProfile.EMAIL = emp.OfficeEmail;
                            userProfile.FULLNAME = emp.EmpName;
                            userProfile.DETAILNAME = emp.EmpDetailName;
                            userProfile.DIMENSION2 = emp.Dimension2;
                        }
                    }

                    list.Add(userProfile);
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("UserManagament", "Exception occured on TranslateDataTableToUserProfile, " + ex.Message);
            }

            return list;
        }

        private List<B1SP_Parameter> TranslateIDToParameterList(int Id)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {
                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = Convert.ToString(Id);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("AssignmentFormManagement", "Exception occured on TranslateIDToParameterList, " + ex.Message);
            }
            return parmList;
        }


        private List<B1SP_Parameter> TranslateUserProfileToParameterList(UserProfile user)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {
                if (user == null)
                    return null;

                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = Convert.ToString(user.ID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "HCMOneID";
                parm.ParameterValue = Convert.ToString(user.HCMOneID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "EMPLOYEECODE";
                parm.ParameterValue = Convert.ToString(user.EMPLOYEECODE);
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "USERNAME";
                parm.ParameterValue = Convert.ToString(user.USERNAME);
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                Encrypt_Decrypt EncryptDecrypt = new Encrypt_Decrypt();
                parm = new B1SP_Parameter();
                parm.ParameterName = "PASSWORD";
                parm.ParameterValue = EncryptDecrypt.EncryptString(user.PASSWORD);
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                //parm = new B1SP_Parameter();
                //parm.ParameterName = "DESIGNATIONID";
                //parm.ParameterValue = Convert.ToString(user.DESIGNATIONID);
                //parmList.Add(parm);

                //parm = new B1SP_Parameter();
                //parm.ParameterName = "DEPARTMENTID";
                //parm.ParameterValue = Convert.ToString(user.DEPARTMENTID);
                //parmList.Add(parm);

                //parm = new B1SP_Parameter();
                //parm.ParameterName = "EMAIL";
                //parm.ParameterValue = Convert.ToString(user.EMAIL);
                //parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ISSUPER";
                parm.ParameterValue = Convert.ToString(user.ISSUPER);
                parm.ParameterType = DBTypes.Bool.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ISACTIVE";
                parm.ParameterValue = Convert.ToString(user.ISACTIVE);
                parm.ParameterType = DBTypes.Bool.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ISDELETED";
                parm.ParameterValue = Convert.ToString(user.ISDELETED);
                parm.ParameterType = DBTypes.Bool.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CREATEDBY";
                parm.ParameterValue = Convert.ToString(user.CREATEDBY);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CREATEDDATE";
                parm.ParameterValue = DateTime.Now.ToString("yyyy-MM-dd HH:MM:s");
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "UPDATEDEDBY";
                parm.ParameterValue = Convert.ToString(user.UPDATEDEDBY);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "UPDATEDDATE";
                parm.ParameterValue = DateTime.Now.ToString("yyyy-MM-dd HH:MM:s");
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);


                parm = new B1SP_Parameter();
                parm.ParameterName = "EMPLOYEENAME";
                parm.ParameterValue = Convert.ToString(user.FULLNAME);
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "DEPARTMENT";
                parm.ParameterValue = Convert.ToString(user.DEPARTMENTNAME);
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "DESIGNATION";
                parm.ParameterValue = Convert.ToString(user.DESIGNATIONNAME);
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("UserManagament", "Exception occured on TranslateLoginToParameterList, " + ex.Message);
            }


            return parmList;
        }

        private List<B1SP_Parameter> TranslateUserProfileLogToParameterList(UserProfile user)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {
                if (user == null)
                    return null;

                B1SP_Parameter parm = new B1SP_Parameter();

                parm = new B1SP_Parameter();
                parm.ParameterName = "USERNAME_PREVIOUS";
                parm.ParameterValue = Convert.ToString(user.USERNAME);
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                Encrypt_Decrypt EncryptDecrypt = new Encrypt_Decrypt();
                parm = new B1SP_Parameter();
                parm.ParameterName = "PASSWORD_PREVIOUS";                
                try { parm.ParameterValue = EncryptDecrypt.DecryptString(user.PASSWORD); }
                catch (Exception ex)
                { parm.ParameterValue = Convert.ToString(user.PASSWORD); }
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);


                parm = new B1SP_Parameter();
                parm.ParameterName = "ISSUPER_PREVIOUS";
                parm.ParameterValue = Convert.ToString(user.ISSUPER);
                parm.ParameterType = DBTypes.Bool.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ISACTIVE_PREVIOUS";
                parm.ParameterValue = Convert.ToString(user.ISACTIVE);
                parm.ParameterType = DBTypes.Bool.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ISDELETED_PREVIOUS";
                parm.ParameterValue = Convert.ToString(user.ISDELETED);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);


                parm = new B1SP_Parameter();
                parm.ParameterName = "USERNAME_NEW";
                parm.ParameterValue = Convert.ToString(user.USERNAME);
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "PASSWORD_NEW";
                try { parm.ParameterValue = EncryptDecrypt.DecryptString(user.PASSWORD); }
                catch (Exception ex)
                { parm.ParameterValue = Convert.ToString(user.PASSWORD); }
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ISSUPER_NEW";
                parm.ParameterValue = Convert.ToString(user.ISSUPER);
                parm.ParameterType = DBTypes.Bool.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ISACTIVE_NEW";
                parm.ParameterValue = Convert.ToString(user.ISACTIVE);
                parm.ParameterType = DBTypes.Bool.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ISDELETED_NEW";
                parm.ParameterValue = Convert.ToString(user.ISDELETED);
                parm.ParameterType = DBTypes.Bool.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "USER_ID";
                parm.ParameterValue = Convert.ToString(user.ID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CREATEDBY";
                parm.ParameterValue = Convert.ToString(user.CREATEDBY);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CREATEDDATE";
                parm.ParameterValue = DateTime.Now.ToString("yyyy-MM-dd HH:MM:s");
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("UserManagament", "Exception occured on TranslateUserProfileLogToParameterList, " + ex.Message);
            }


            return parmList;
        }

        private List<UserPermissions> TranslateDataTableToUserPermissionList(DataTable dt)
        {
            List<UserPermissions> list = new List<UserPermissions>();
            Common cmn = new Common();
            List<TMS_Menu> menuList = cmn.GetMenuList();
            try
            {
                foreach (DataRow dtRow in dt.Rows)
                {
                    UserPermissions perm = new UserPermissions();
                    TMS_Menu menu = menuList.Where(x => x.ID == Convert.ToInt32(dtRow["MenuID"])).FirstOrDefault();
                    if (menu != null)
                    {
                        if (menu.Head_ID == 0)
                        {
                            perm.ID = menu.ID;
                            perm.HeadID = menu.Head_ID;
                            perm.Order = menu.Index;
                            perm.PageName = menu.Name;
                            perm.PageURL = menu.PageURL;
                            perm.Role = Convert.ToInt32(dtRow["UserAuthorizationID"]);
                            list.Add(perm);
                            //For Child1
                            List<TMS_Menu> menu_Child1 = menuList.Where(x => x.Head_ID == perm.ID).ToList();
                            foreach (var child1 in menu_Child1)
                            {
                                UserPermissions perm_Child1 = new UserPermissions();
                                perm_Child1.ID = child1.ID;
                                perm_Child1.HeadID = child1.Head_ID;
                                perm_Child1.Order = child1.Index;
                                perm_Child1.PageName = child1.Name;
                                perm_Child1.PageURL = child1.PageURL;
                                perm_Child1.Role = Convert.ToInt32(dtRow["UserAuthorizationID"]);
                                list.Add(perm_Child1);

                                //For Child2
                                List<TMS_Menu> menu_Child2 = menuList.Where(x => x.Head_ID == perm_Child1.ID).ToList();
                                foreach (var child2 in menu_Child2)
                                {
                                    UserPermissions perm_Child2 = new UserPermissions();
                                    perm_Child2.ID = child2.ID;
                                    perm_Child2.HeadID = child2.Head_ID;
                                    perm_Child2.Order = child2.Index;
                                    perm_Child2.PageName = child2.Name;
                                    perm_Child2.PageURL = child2.PageURL;
                                    perm_Child2.Role = Convert.ToInt32(dtRow["UserAuthorizationID"]);
                                    list.Add(perm_Child2);
                                }
                            }

                        }//Convert.ToInt32(dtRow["MenuID"]))
                        else
                        {
                            //For Parent1
                            TMS_Menu menu_Parent = menuList.Where(x => x.ID == menu.Head_ID).FirstOrDefault();
                            if (menu_Parent != null)
                            {
                                if (list.Where(x => x.ID == menu_Parent.ID).FirstOrDefault() == null)
                                {
                                    perm = new UserPermissions();
                                    perm.ID = menu_Parent.ID;
                                    perm.HeadID = menu_Parent.Head_ID;
                                    perm.Order = menu_Parent.Index;
                                    perm.PageName = menu_Parent.Name;
                                    perm.PageURL = menu_Parent.PageURL;
                                    list.Add(perm);

                                    //For Parent2
                                    TMS_Menu menu_Parent2 = menuList.Where(x => x.ID == perm.HeadID).FirstOrDefault();
                                    if (menu_Parent2 != null)
                                    {
                                        if (list.Where(x => x.ID == menu_Parent2.ID).FirstOrDefault() == null)
                                        {
                                            perm = new UserPermissions();
                                            perm.ID = menu_Parent2.ID;
                                            perm.HeadID = menu_Parent2.Head_ID;
                                            perm.Order = menu_Parent2.Index;
                                            perm.PageName = menu_Parent2.Name;
                                            perm.PageURL = menu_Parent2.PageURL;
                                            list.Add(perm);
                                        }
                                    }


                                }
                            }


                            //For Child
                            perm = new UserPermissions();
                            perm.ID = menu.ID;
                            perm.HeadID = menu.Head_ID;
                            perm.Order = menu.Index;
                            perm.PageName = menu.Name;
                            perm.PageURL = menu.PageURL;
                            perm.Role = Convert.ToInt32(dtRow["UserAuthorizationID"]);
                            list.Add(perm);

                            List<TMS_Menu> menu_Child2 = menuList.Where(x => x.Head_ID == perm.ID).ToList();
                            foreach (var child2 in menu_Child2)
                            {
                                UserPermissions perm_Child2 = new UserPermissions();
                                perm_Child2.ID = child2.ID;
                                perm_Child2.HeadID = child2.Head_ID;
                                perm_Child2.Order = child2.Index;
                                perm_Child2.PageName = child2.Name;
                                perm_Child2.PageURL = child2.PageURL;
                                perm_Child2.Role = Convert.ToInt32(dtRow["UserAuthorizationID"]);
                                list.Add(perm_Child2);
                            }
                        }

                    }
                }
                list = list.Where(x => x.Role != 1).OrderBy(x => x.Order).ToList();
                //list = list.GroupBy(x => x.ID).Select(x => x.First()).ToList();
                List<UserPermissions> filteredlist = new List<UserPermissions>();
                foreach (var item in list)
                {
                    var existData = filteredlist.Where(x => x.ID == item.ID).FirstOrDefault();
                    if (existData == null)
                    {
                        UserPermissions perm = new UserPermissions();
                        perm.ID = item.ID;
                        perm.HeadID = item.HeadID;
                        perm.Order = item.Order;
                        perm.PageName = item.PageName;
                        perm.PageURL = item.PageURL;
                        perm.Role = item.Role;
                        filteredlist.Add(perm);
                    }
                    else
                    {
                        filteredlist.Where(x => x.ID == item.ID).ToList().ForEach(x => x.Role = item.Role);
                    }

                }
                list = filteredlist;
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("UserManagament", "Exception occured on TranslateDataTableToUserPermissionList, " + ex.Message);
            }

            return list;
        }

        #endregion



    }
}