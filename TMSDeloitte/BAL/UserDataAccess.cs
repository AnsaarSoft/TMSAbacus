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

    public class UserDataAccess
    {
        public bool AddUpdateUserDataAccess(int DocId, List<UserDataAccess_Menu> authList, int userId, int sessionUserID, out string Msg)
        {
            bool isSuccess = false;
            bool isAddOccured = false;
            bool isUpdateOccured = false;
            bool isDeleteOccured = false;
            Msg = "";
            try
            {
                Common cmn = new Common();
                int AuthID = 0;

                if (!ValidateUserDataAccess(authList,userId, out Msg))
                    return false;

                string docNum = "";
                if (DocId == 0)
                {
                    int no = 1;
                   
                    List<string> docNumList = cmn.GetDocNum("GetDataAccessDocNum", "UserDataAccess");
                    if (docNumList.Count > 0)
                    {
                        var item = docNumList[docNumList.Count - 1];
                        no = Convert.ToInt32(item.Split('-')[1]);
                        no = no + 1;
                        docNum = Convert.ToString(no).PadLeft(5, '0');
                        docNum = "Doc-" + docNum;
                    }
                    else
                    {
                        docNum = Convert.ToString(no).PadLeft(5, '0');
                        docNum = "Doc-" + docNum;
                    }
                }
                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                DataTable dt_DataAccess = HANADAL.AddUpdateDataByStoredProcedure("AddUpdateDataAccess", GetDataAccessParameterList(DocId, docNum,userId, sessionUserID), "UserDataAccess");
                if (dt_DataAccess.Rows.Count == 0)
                    throw new Exception("Exception occured when add/update DataAccess");
                else
                {

                    foreach (DataRow dtRow in dt_DataAccess.Rows)
                    {
                        isAddOccured = true;
                        AuthID = Convert.ToInt32(dtRow["ID"]);
                        docNum = Convert.ToString(dtRow["DocNum"]);
                    }
                }

                if (AuthID == 0)
                    throw new Exception("Exception occured when add/update DataAccess");

                if (DocId == 0)
                {
                    foreach (var auth in authList)
                    {
                        HANADAL.AddUpdateDataByStoredProcedure("AddUpdateDataAccessList", TranslateAuthToParameterList(0, AuthID, auth, auth.IsDeleted), "UserDataAccess");
                    }
                   
                   
                }
                else
                {
                    List<UserDataAccess_Menu> authList_Previous = GetDataAccessListByAuthID(DocId);

                    //For DataAccess List
                    if (authList!=null)
                    {
                     
                        foreach (var auth in authList)
                        {
                            var previous_item = authList_Previous.Where(x => x.ID == auth.ID).FirstOrDefault();
                            int ID = 0;
                            if (previous_item != null)
                            {
                                isUpdateOccured = true;
                                ID = previous_item._ID;
                                AddDataAccess_Log(auth, previous_item, DocId, docNum, sessionUserID);
                            }
                            HANADAL.AddUpdateDataByStoredProcedure("AddUpdateDataAccessList", TranslateAuthToParameterList(ID, AuthID, auth, auth.IsDeleted), "UserDataAccess");

                        }
                    }
                    List<UserDataAccess_Menu> missingAuthList = new List<UserDataAccess_Menu>();
                    if (authList == null)
                        missingAuthList = authList_Previous;
                    else
                        missingAuthList = authList_Previous.Where(n => !authList.Any(o => o.ID == n.ID)).ToList();
                    foreach (var auth in missingAuthList)
                    {
                        UserDataAccess_Menu previous_item = new UserDataAccess_Menu();
                        previous_item.IsDeleted = true;
                        isDeleteOccured = true;
                        AddDataAccess_Log(auth, previous_item, DocId, docNum, sessionUserID);
                        HANADAL.AddUpdateDataByStoredProcedure("AddUpdateDataAccessList", TranslateAuthToParameterList(auth._ID, AuthID, auth, true), "UserDataAccess");
                    }

                   
                }
                Msg = "Successfully Added/Updated";
                
                //For Master Log
                Task.Run(() => cmn.AddMasterLog(isAddOccured, isUpdateOccured, isDeleteOccured, nameof(Enums.FormsName.UserDataAccess), sessionUserID, "UserDataAccess"));
                // End MAster Log


                isSuccess = true;
            }
            catch (Exception ex)
            {
                Msg = "Exception occured when add/update user!";
                isSuccess = false;
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("UserDataAccess", "Exception occured on AddUpdateUser, " + ex.Message);
            }

            return isSuccess;
        }

        private bool ValidateUserDataAccess(List<UserDataAccess_Menu> authList,int userId, out string Msg)
        {
            Msg = "";
            if (userId == 0)
            {
                Msg = "Please select atleast user";
                return false;
            }
            
            //if (authList == null)
            //{
            //    Msg = "Please select atleast 1 DataAccess";
            //    return false;
            //}
           
            //if (authList.Count == 0)
            //{
            //    Msg = "Please select atleast 1 DataAccess";
            //    return false;
            //}

            return true;
        }

        public void GetDataAccessByDocNum(string docNum, out int DocID,out int EmpID, out string  EmpCode)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            DocID = 0;
            EmpID = 0;
            EmpCode ="";
            try
            {
                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "DocNum";
                parm.ParameterValue = Convert.ToString(docNum);
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                DataTable dt = HANADAL.GetDataTableByStoredProcedure("GetDataAccessByDocNum", parmList, "UserDataAccess");
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dtRow in dt.Rows)
                    {
                        DocID = Convert.ToInt32(dtRow["ID"]);
                        int UserID = Convert.ToInt32(dtRow["UserID"]);
                        UserManagement userMgt = new UserManagement();
                        UserProfile user= userMgt.GetUserByID(UserID);
                        EmpID = user.ID;
                        EmpCode = user.EMPLOYEECODE;
                        break;
                    }
                }
                

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("UserDataAccess", "Exception occured on GetDataAccessByDocNum, " + ex.Message);
            }
        }
        public void GetDataAccessByUserID(string UserID, out int DocID,out string DocNum, out List<UserDataAccess_Menu> authList)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            authList = new List<UserDataAccess_Menu>();
            DocID = 0;
            DocNum = "";
            try
            {
                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = Convert.ToString(UserID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                DataSet ds = HANADAL.GetDataSetByStoredProcedure("GetDataAccessByUserID", parmList, "UserDataAccess");
                Common cmn = new Common();
                authList = cmn.GetUserDataAccessMenuList();
                if (ds.Tables[0].Rows.Count > 0 && ds.Tables[1].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        DocID =Convert.ToInt32(row["ID"]);
                        DocNum = Convert.ToString(row["DocNum"]);
                        break;
                    }
                    foreach (var auth in TranslateDataTableToAuthList(ds.Tables[1]))
                    {
                        var item = authList.Where(x => x.ID == auth.ID).FirstOrDefault();
                        if (item != null)
                        {
                            item.IsChecked = true;
                            item.Authorization = auth.Authorization;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("UserDataAccess", "Exception occured on GetDataAccessByDocNum, " + ex.Message);
            }
        }

        public List<UserDataAccess_Menu> GetDataAccessListByAuthID(int id)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            List<UserDataAccess_Menu> authList = new List<UserDataAccess_Menu>();
            try
            {
                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = Convert.ToString(id);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                DataTable dt = HANADAL.GetDataTableByStoredProcedure("GetDataAccessListByDocID", parmList, "UserDataAccess");
                if (dt.Rows.Count > 0)
                {
                    authList = TranslateDataTableToAuthList(dt);
                }

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("UserDataAccess", "Exception occured on GetDataAccessListByDocID, " + ex.Message);
            }
            return authList;
        }
        
     
        public void AddDataAccess_Log(UserDataAccess_Menu newObject, UserDataAccess_Menu previousObject, int DocId, string DocNum, int sessionUserID)
        {
            try
            {
                if (newObject.ID > 0)
                {
                    List<B1SP_Parameter> paramList = TranslateDataAccessLogToParameterList(newObject, DocId, DocNum, sessionUserID);
                    bool isChangeOccured = false;
                    if (previousObject != null)
                    {
                        foreach (PropertyCompareResult resultItem in PropertyCompare.Compare(previousObject, newObject))
                        {
                            isChangeOccured = true;
                            string Name = resultItem.Name;
                            object PreviousValue = resultItem.OldValue;
                            object NewValue = resultItem.NewValue;

                            switch (Name)
                            {


                                case "Authorization":

                                    PreviousValue = Convert.ToString(((Enums.UserAuthorization)PreviousValue));
                                    NewValue = Convert.ToString(((Enums.UserAuthorization)NewValue));

                                    paramList.Where(x => x.ParameterName == "UserAuthorization_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                    paramList.Where(x => x.ParameterName == "UserAuthorization_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();

                                    break;
                                case "IsDeleted":
                                    paramList.Where(x => x.ParameterName == "IsChecked_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                    paramList.Where(x => x.ParameterName == "IsChecked_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();

                                    break;

                            }

                        }

                        if (isChangeOccured)
                        {
                            HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                            Task.Run(() => HANADAL.AddUpdateDataByStoredProcedure("AddDataAccessList_Log", paramList, "UserDataAccess"));
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("UserManagement", "Exception occured on AddDataAccess_Log, " + ex.Message);
            }
        }

        public void AddUserDataAccess_Log(UserProfile newObject, UserProfile previousObject, int DocId, string DocNum, int sessionUserID)
        {
            try
            {
                if (newObject.ID > 0)
                {
                    List<B1SP_Parameter> paramList = TranslateUserDataAccessLogToParameterList(newObject, DocId, DocNum, sessionUserID);
                    bool isChangeOccured = false;
                    if (previousObject != null)
                    {
                        foreach (PropertyCompareResult resultItem in PropertyCompare.Compare(previousObject, newObject))
                        {
                            isChangeOccured = true;
                            string Name = resultItem.Name;
                            object PreviousValue = resultItem.OldValue;
                            object NewValue = resultItem.NewValue;

                            switch (Name)
                            {

                                case "ISDELETED":
                                    paramList.Where(x => x.ParameterName == "IsChecked_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                    paramList.Where(x => x.ParameterName == "IsChecked_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();

                                    break;

                            }

                        }

                        if (isChangeOccured)
                        {
                            HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                            Task.Run(() => HANADAL.AddUpdateDataByStoredProcedure("AddUserDataAccess_Log", paramList, "UserDataAccess"));
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("UserManagement", "Exception occured on AddUserDataAccess_Log, " + ex.Message);
            }
        }

        public void GetDataAccessLogByDocID(string docId, out DataTable dt_Auth)
        {
            dt_Auth = new DataTable();
            try

            {
                List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();

                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = docId;
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                dt_Auth = HANADAL.GetDataTableByStoredProcedure("GetDataAccessLogByDocID", parmList, "UserDataAccess");
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("UserDataAccess", "Exception occured on GetDataAccessLogByDocID, " + ex.Message);
            }

        }


        #region Translation
        
        private List<B1SP_Parameter> GetDataAccessParameterList(int ID, string DocNum,int userID, int sessionUserID)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {

                B1SP_Parameter parm = new B1SP_Parameter();

                parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = Convert.ToString(ID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "DocNum";
                parm.ParameterValue = Convert.ToString(DocNum);
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "UserID";
                parm.ParameterValue = Convert.ToString(userID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CREATEDBY";
                parm.ParameterValue = Convert.ToString(sessionUserID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CREATEDDATE";
                parm.ParameterValue = DateTime.Now.ToString("yyyy-MM-dd HH:MM:s");
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "UPDATEDEDBY";
                parm.ParameterValue = Convert.ToString(sessionUserID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "UPDATEDDATE";
                parm.ParameterValue = DateTime.Now.ToString("yyyy-MM-dd HH:MM:s");
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("UserDataAccess", "Exception occured on GetDataAccessParameterList, " + ex.Message);
            }


            return parmList;
        }


        private List<B1SP_Parameter> TranslateAuthToParameterList(int ID, int DataAccessID, UserDataAccess_Menu auth, bool isDeleted)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {
                if (auth == null)
                    return null;

                B1SP_Parameter parm = new B1SP_Parameter();

                parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = Convert.ToString(ID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "DataAccessID";
                parm.ParameterValue = Convert.ToString(DataAccessID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "MenuID";
                string menuID = Convert.ToString(auth.ID);
                //if(menuID.Length>=4)
                //    menuID= menuID.Substring(4, menuID.Length - 4);
                parm.ParameterValue = menuID;
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "UserDataAccessID";
                parm.ParameterValue = Convert.ToString(auth.Authorization);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);


                parm = new B1SP_Parameter();
                parm.ParameterName = "IsDeleted";
                parm.ParameterValue = Convert.ToString(isDeleted);
                parm.ParameterType = DBTypes.Bool.ToString();
                parmList.Add(parm);


            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("UserDataAccess", "Exception occured on TranslateAuthToParameterList, " + ex.Message);
            }


            return parmList;
        }

      
        private List<UserProfile> TranslateDataTableToUserAuthList(DataTable dt)
        {
            List<UserProfile> list = new List<UserProfile>();

            try
            {

                foreach (DataRow dtRow in dt.Rows)
                {
                    UserProfile userProfile = new UserProfile();
                    userProfile.ID = Convert.ToInt32(dtRow["UserID"]);
                    userProfile.AuthorizationTableID = Convert.ToInt32(dtRow["ID"]);
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

        private List<UserDataAccess_Menu> TranslateDataTableToAuthList(DataTable dt)
        {
            List<UserDataAccess_Menu> list = new List<UserDataAccess_Menu>();

            try
            {

                foreach (DataRow dtRow in dt.Rows)
                {
                    UserDataAccess_Menu menu = new UserDataAccess_Menu();
                    menu._ID = Convert.ToInt32(dtRow["ID"]);
                    menu.ID = Convert.ToInt32(dtRow["MenuID"]);
                    menu.DataAccessID = Convert.ToInt32(dtRow["DataAccessID"]);
                    menu.Authorization = Convert.ToInt32(dtRow["UserAuthorizationID"]);
                    menu.IsDeleted = Convert.ToBoolean(dtRow["IsDeleted"]);
                    list.Add(menu);
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
        
        private List<B1SP_Parameter> TranslateDataAccessLogToParameterList(UserDataAccess_Menu menu, int DocId, string Docnum, int sessionUser)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {
                if (menu == null)
                    return null;

                B1SP_Parameter parm = new B1SP_Parameter();

                parm = new B1SP_Parameter();
                parm.ParameterName = "DataAccessID";
                parm.ParameterValue = Convert.ToString(DocId);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "DocNum";
                parm.ParameterValue = Convert.ToString(Docnum);
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);


                parm = new B1SP_Parameter();
                parm.ParameterName = "Menu";
                Common cmn = new Common();
                parm.ParameterValue = Convert.ToString(cmn.GetUserDataAccessMenuList().Where(x => x.ID == menu.ID).Select(x => x.Name).FirstOrDefault());
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "UserAuthorization_Previous";
                parm.ParameterValue = Convert.ToString(((Enums.UserAuthorization)menu.Authorization));
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "IsChecked_Previous";
                parm.ParameterValue = Convert.ToString(menu.IsDeleted);
                parm.ParameterType = DBTypes.Bool.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "UserAuthorization_New";
                parm.ParameterValue = Convert.ToString(((Enums.UserAuthorization)menu.Authorization));
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "IsChecked_New";
                parm.ParameterValue = Convert.ToString(menu.IsDeleted);
                parm.ParameterType = DBTypes.Bool.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CREATEDBY";
                parm.ParameterValue = Convert.ToString(sessionUser);
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
                log.InputOutputDocLog("UserManagament", "Exception occured on TranslateDataAccessLogToParameterList, " + ex.Message);
            }


            return parmList;
        }

        private List<B1SP_Parameter> TranslateUserDataAccessLogToParameterList(UserProfile user, int DocId, string Docnum, int sessionUser)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {
                if (user == null)
                    return null;

                B1SP_Parameter parm = new B1SP_Parameter();

                parm = new B1SP_Parameter();
                parm.ParameterName = "DataAccessID";
                parm.ParameterValue = Convert.ToString(DocId);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "DocNum";
                parm.ParameterValue = Convert.ToString(Docnum);
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);



                parm = new B1SP_Parameter();
                parm.ParameterName = "UserID_Previous";
                parm.ParameterValue = Convert.ToString(user.ID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "IsChecked_Previous";
                parm.ParameterValue = Convert.ToString(user.ISDELETED);
                parm.ParameterType = DBTypes.Bool.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "UserID_New";
                parm.ParameterValue = Convert.ToString(user.ID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "IsChecked_New";
                parm.ParameterValue = Convert.ToString(user.ISDELETED);
                parm.ParameterType = DBTypes.Bool.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CREATEDBY";
                parm.ParameterValue = Convert.ToString(sessionUser);
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
                log.InputOutputDocLog("UserManagament", "Exception occured on TranslateDataAccessLogToParameterList, " + ex.Message);
            }


            return parmList;
        }

      


        #endregion

    }
}