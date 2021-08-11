using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using TMSDeloitte.DAL;
using TMSDeloitte.Helper;
using TMSDeloitte.Models;

namespace TMSDeloitte.BAL
{

    public class UserManagement
    {
      
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
                parmList.Add(parm);

                HANA_DAL HANADAL = new HANA_DAL();
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
                parmList.Add(parm);

                HANA_DAL HANADAL = new HANA_DAL();
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
                parmList.Add(parm);
                //parm = new B1SP_Parameter();
                //parm.ParameterName = "Email";
                //parm.ParameterValue = Convert.ToString(email);
                //parmList.Add(parm);

                HANA_DAL HANADAL = new HANA_DAL();
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
                parmList.Add(parm);

                HANA_DAL HANADAL = new HANA_DAL();
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
                parmList.Add(parm);

                HANA_DAL HANADAL = new HANA_DAL();
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

        public List<UserProfile> GetAllUsers(bool IncludeIsSuper=true)
        {
            List<UserProfile> userProfiles = new List<UserProfile>();
            try
            {
                HANA_DAL HANADAL = new HANA_DAL();
                DataTable dt_UserProfile = HANADAL.GetDataTableByStoredProcedure("GetAllUser","UserManagement");
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
                parmList.Add(parm);
                
                HANA_DAL HANADAL = new HANA_DAL();
                DataTable dt_UserProfile = HANADAL.GetDataTableByStoredProcedure("GetUserByEmpCode", parmList, "UserManagement");
                Encrypt_Decrypt security = new Encrypt_Decrypt();
                userProfile.EMPLOYEECODE = empCode;
                userProfile = TranslateDataTableToUserProfile(dt_UserProfile,empCode);
                try { userProfile.PASSWORD = security.DecryptString(userProfile.PASSWORD); } catch (Exception ex) { }
                

                if(userProfile!=null)
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
                parmList.Add(parm);

                Encrypt_Decrypt EncryptDecrypt = new Encrypt_Decrypt();
                parm = new B1SP_Parameter();
                parm.ParameterName = "Password";
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

      
        private UserProfile TranslateDataTableToUserProfile(DataTable dt,string empCode=null)
        {
            UserProfile userProfile = new UserProfile();
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
                }
                if (string.IsNullOrEmpty(userProfile.EMPLOYEECODE))
                    EmployeeCode = empCode;
                else
                    EmployeeCode = userProfile.EMPLOYEECODE;

                if(!string.IsNullOrEmpty(EmployeeCode))
                {
                    HCM_Employee emp = GetHCMOneEmployeeByCode(EmployeeCode);
                    if (emp != null)
                    {
                        userProfile.DEPARTMENTID = emp.DepartmentID;
                        userProfile.DEPARTMENTNAME = emp.DepartmentName;
                        userProfile.DESIGNATIONID = emp.DesignationID;
                        userProfile.DESIGNATIONNAME = emp.DesignationName;
                        userProfile.OBAPPROVEDHOURS = emp.OBApprovedHours;
                        userProfile.OBOVERTIMEHOURS = emp.OBOverTimeHours;
                        userProfile.OBASONDATE = emp.OBAsOnDate;
                        userProfile.LOCATIONID = emp.LocationID;
                        userProfile.LOCATIONNAME = emp.LocationName;
                        userProfile.BRANCHID = emp.BranchID;
                        userProfile.BRANCHNAME = emp.BranchName;
                        userProfile.EMAIL = emp.OfficeEmail;
                        userProfile.FULLNAME = emp.EmpName;
                        userProfile.DETAILNAME = emp.EmpDetailName;
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
           
            try
            {
                HCMOneManagement mgt = new HCMOneManagement();

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
                        HCM_Employee emp = GetHCMOneEmployeeByCode(EmployeeCode);
                        if (emp != null)
                        {
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
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "HCMOneID";
                parm.ParameterValue = Convert.ToString(user.HCMOneID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "EMPLOYEECODE";
                parm.ParameterValue = Convert.ToString(user.EMPLOYEECODE);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "USERNAME";
                parm.ParameterValue = Convert.ToString(user.USERNAME);
                parmList.Add(parm);

                Encrypt_Decrypt EncryptDecrypt = new Encrypt_Decrypt();
                parm = new B1SP_Parameter();
                parm.ParameterName = "PASSWORD";
                parm.ParameterValue = EncryptDecrypt.EncryptString(user.PASSWORD);
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
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ISACTIVE";
                parm.ParameterValue = Convert.ToString(user.ISACTIVE);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ISDELETED";
                parm.ParameterValue = Convert.ToString(user.ISDELETED);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CREATEDBY";
                parm.ParameterValue = Convert.ToString(user.CREATEDBY);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CREATEDDATE";
                parm.ParameterValue = DateTime.Now.ToString("yyyy-MM-dd HH:MM:s");
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "UPDATEDEDBY";
                parm.ParameterValue = Convert.ToString(user.UPDATEDEDBY);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "UPDATEDDATE";
                parm.ParameterValue = DateTime.Now.ToString("yyyy-MM-dd HH:MM:s");
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
                parmList.Add(parm);

                Encrypt_Decrypt EncryptDecrypt = new Encrypt_Decrypt();
                parm = new B1SP_Parameter();
                parm.ParameterName = "PASSWORD_PREVIOUS";
                try { parm.ParameterValue = EncryptDecrypt.DecryptString(user.PASSWORD); }catch(Exception ex)
                { parm.ParameterValue = Convert.ToString(user.PASSWORD); }
                parmList.Add(parm);

              
                parm = new B1SP_Parameter();
                parm.ParameterName = "ISSUPER_PREVIOUS";
                parm.ParameterValue = Convert.ToString(user.ISSUPER);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ISACTIVE_PREVIOUS";
                parm.ParameterValue = Convert.ToString(user.ISACTIVE);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ISDELETED_PREVIOUS";
                parm.ParameterValue = Convert.ToString(user.ISDELETED);
                parmList.Add(parm);

                
                parm = new B1SP_Parameter();
                parm.ParameterName = "USERNAME_NEW";
                parm.ParameterValue = Convert.ToString(user.USERNAME);
                parmList.Add(parm);
                
                parm = new B1SP_Parameter();
                parm.ParameterName = "PASSWORD_NEW";
                try { parm.ParameterValue = EncryptDecrypt.DecryptString(user.PASSWORD); }
                catch (Exception ex)
                { parm.ParameterValue = Convert.ToString(user.PASSWORD); }
                parmList.Add(parm);
                
                parm = new B1SP_Parameter();
                parm.ParameterName = "ISSUPER_NEW";
                parm.ParameterValue = Convert.ToString(user.ISSUPER);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ISACTIVE_NEW";
                parm.ParameterValue = Convert.ToString(user.ISACTIVE);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ISDELETED_NEW";
                parm.ParameterValue = Convert.ToString(user.ISDELETED);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "USER_ID";
                parm.ParameterValue = Convert.ToString(user.ID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CREATEDBY";
                parm.ParameterValue = Convert.ToString(user.CREATEDBY);
                parmList.Add(parm);
                
                parm = new B1SP_Parameter();
                parm.ParameterName = "CREATEDDATE";
                parm.ParameterValue = DateTime.Now.ToString("yyyy-MM-dd HH:MM:s");
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

      
        #endregion



    }
}