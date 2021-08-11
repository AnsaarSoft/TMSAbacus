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
    public class UserAuthorization
    {

        public bool AddUpdateUserAuthorization(int DocId, List<TMS_Menu> authList, List<UserProfile> userList, List<GroupSetupInfo> groupList, int sessionUserID, out string Msg)
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

                if (!ValidateUserAuthorization(authList, userList, groupList, out Msg))
                {
                    return false;
                }
                   

                string docNum = "";
                if (DocId == 0)
                {
                    int no = 1;
                    
                    List<string> docNumList = cmn.GetDocNum("GetAuthorizationDocNum", "UserAuthorization");
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
                DataTable dt_Authorization = HANADAL.AddUpdateDataByStoredProcedure("AddUpdateAuthorization", GetAuthorizationParameterList(DocId, docNum, sessionUserID), "UserAuthorization");
                if (dt_Authorization.Rows.Count == 0)
                    throw new Exception("Exception occured when add/update authorization");
                else
                {

                    foreach (DataRow dtRow in dt_Authorization.Rows)
                    {
                        isAddOccured = true;
                        AuthID = Convert.ToInt32(dtRow["ID"]);
                        docNum = Convert.ToString(dtRow["DocNum"]);
                    }
                }

                if (AuthID == 0)
                    throw new Exception("Exception occured when add/update authorization");

                if (DocId == 0)
                {
                    if(authList!=null)
                    {
                        foreach (var auth in authList)
                        {
                            HANADAL.AddUpdateDataByStoredProcedure("AddUpdateAuthorizationList", TranslateAuthToParameterList(0, AuthID, auth, auth.IsDeleted), "UserAuthorization");
                        }
                    }
                    if(userList!=null)
                    {
                        foreach (var user in userList)
                        {
                            HANADAL.AddUpdateDataByStoredProcedure("AddUpdateUserAuthorization", TranslateUserAuthToParameterList(0, AuthID, user.ID, false), "UserAuthorization");
                        }
                    }
                    
                    if(groupList!=null)
                    {
                        foreach (var group in groupList)
                        {
                            HANADAL.AddUpdateDataByStoredProcedure("AddUpdateUserGroupAuthorization", TranslateAuthGroupToParameterList(0, AuthID, group, false), "UserAuthorization");
                        }
                    }
                    
                }
                else
                {
                    //For Authorization List
                    List<TMS_Menu> authList_Previous = GetAuthorizationListByAuthID(DocId);
                    if(authList!=null)
                    {
                        foreach (var auth in authList)
                        {
                            var previous_item = authList_Previous.Where(x => x.ID == auth.ID).FirstOrDefault();
                            int ID = 0;
                            if (previous_item != null)
                            {
                                ID = previous_item._ID;
                                isUpdateOccured = true;
                                AddAuthorization_Log(auth, previous_item, DocId, docNum, sessionUserID);
                            }
                            HANADAL.AddUpdateDataByStoredProcedure("AddUpdateAuthorizationList", TranslateAuthToParameterList(ID, AuthID, auth, auth.IsDeleted), "UserAuthorization");

                        }
                    }


                    List<TMS_Menu> missingAuthList = new List<TMS_Menu>();
                    if (authList != null)
                        missingAuthList = authList_Previous.Where(n => !authList.Any(o => o.ID == n.ID)).ToList();
                    else
                        missingAuthList = authList_Previous;
                    foreach (var auth in missingAuthList)
                    {
                        TMS_Menu previous_item = new TMS_Menu();
                        previous_item.IsDeleted = true;
                        isDeleteOccured = true;
                        AddAuthorization_Log(auth, previous_item, DocId, docNum, sessionUserID);
                        HANADAL.AddUpdateDataByStoredProcedure("AddUpdateAuthorizationList", TranslateAuthToParameterList(auth._ID, AuthID, auth, true), "UserAuthorization");
                    }

                    //for User
                    if(userList!=null)
                    {
                        List<UserProfile> userList_Previous = GetUserAuthorizationByAuthID(DocId);
                        foreach (var user in userList)
                        {
                            var previous_item = userList_Previous.Where(x => x.ID == user.ID).FirstOrDefault();
                            int ID = 0;
                            if (previous_item != null)
                            {
                                isUpdateOccured = true;
                                ID = Convert.ToInt32(previous_item.AuthorizationTableID);
                            }
                            HANADAL.AddUpdateDataByStoredProcedure("AddUpdateUserAuthorization", TranslateUserAuthToParameterList(ID, AuthID, user.ID, false), "UserAuthorization");

                        }
                        List<UserProfile> missingUserList = userList_Previous.Where(n => !userList.Any(o => o.ID == n.ID)).ToList();
                        foreach (var user in missingUserList)
                        {
                            UserProfile previous_item = new UserProfile();
                            user.ISDELETED = false;
                            previous_item.ISDELETED = true;
                            isDeleteOccured = true;
                            AddUserAuthorization_Log(user, previous_item, DocId, docNum, sessionUserID);
                            HANADAL.AddUpdateDataByStoredProcedure("AddUpdateUserAuthorization", TranslateUserAuthToParameterList(Convert.ToInt32(user.AuthorizationTableID), AuthID, user.ID, true), "UserAuthorization");
                        }
                    }
                    


                    //For Group
                    List<GroupSetupInfo> groupList_Previous = GetUserGroupAuthorizationByAuthID(DocId);
                    if(groupList!=null)
                    {
                        foreach (var group in groupList)
                        {
                            var item = groupList_Previous.Where(x => x.ID == group.ID).FirstOrDefault();
                            int ID = 0;
                            if (item != null)
                            {
                                isUpdateOccured = true;
                                ID = item.AuthorizationTableID;

                            }
                            HANADAL.AddUpdateDataByStoredProcedure("AddUpdateUserGroupAuthorization", TranslateAuthGroupToParameterList(ID, AuthID, group, false), "UserAuthorization");

                        }
                        List<GroupSetupInfo> missingGroupList = groupList_Previous.Where(n => !groupList.Any(o => o.ID == n.ID)).ToList();
                        foreach (var group in missingGroupList)
                        {
                            GroupSetupInfo previous_item = new GroupSetupInfo();
                            group.ISDELETED = false;
                            previous_item.ISDELETED = true;
                            isDeleteOccured = true;
                            AddUserGroupAuthorization_Log(group, previous_item, DocId, docNum, sessionUserID);
                            HANADAL.AddUpdateDataByStoredProcedure("AddUpdateUserGroupAuthorization", TranslateAuthGroupToParameterList(group.AuthorizationTableID, AuthID, group, true), "UserAuthorization");

                        }
                    }
                    



                }
                Msg = "Successfully Added/Updated";





                //For Master Log
                Task.Run(() => cmn.AddMasterLog(isAddOccured, isUpdateOccured, isDeleteOccured, nameof(Enums.FormsName.UserAuthorization), sessionUserID, "USerAuthorization"));
                // End MAster Log


                isSuccess = true;
            }
            catch (Exception ex)
            {
                if(Msg=="")
                    Msg = "Exception occured when add/update user!";
                isSuccess = false;
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("UserAuthorization", "Exception occured on AddUpdateUser, " + ex.Message);
            }

            return isSuccess;
        }

        private bool ValidateUserAuthorization(List<TMS_Menu> authList, List<UserProfile> userList, List<GroupSetupInfo> groupList, out string Msg)
        {
            bool isSuccess = false; 
            Msg = "";
            if (userList == null && groupList == null)
            {
                Msg = "Please select atleast 1 user and 1 group";
                return false;
            }
            //if (groupList == null)
            //{
            //    Msg = "Please select atleast 1 group";
            //    return false;
            //}
            //if (authList == null)
            //{
            //    Msg = "Please select atleast 1 authorization";
            //    return false;
            //}
            if(authList!=null)
            {
                if (authList.Count == 0)
                {
                    Msg = "Please select atleast 1 authorization";
                    return false;
                }
            }
            

            if (userList!=null)
            {
                if (userList.Count == 0)
                {
                    Msg = "Please select atleast 1 user";
                    return false;
                }
                else
                    isSuccess = true;
            }
            if(!isSuccess)
            {
                if (groupList != null)
                {
                    if (groupList.Count == 0)
                    {
                        Msg = "Please select atleast 1 group";
                        return false;
                    }
                }
            }
           
            
            //if (authList.Count == 0)
            //{
            //    Msg = "Please select atleast 1 authorization";
            //    return false;
            //}

            return true;
        }

        public void GetAuthorizationByDocNum(string docNum, out int DocID, out List<TMS_Menu> authList, out List<UserProfile> userList, out List<GroupSetupInfo> groupList)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            authList = new List<TMS_Menu>();
            userList = new List<UserProfile>();
            groupList = new List<GroupSetupInfo>();
            DocID = 0;
            try
            {
                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "DocNum";
                parm.ParameterValue = Convert.ToString(docNum);
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                DataTable dt = HANADAL.GetDataTableByStoredProcedure("GetAuthorizationByDocNum", parmList, "UserAuthorization");
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dtRow in dt.Rows)
                    {
                        DocID = Convert.ToInt32(dtRow["ID"]);
                    }
                }
                if (DocID > 0)
                {
                    Common cmn = new Common();
                    authList = cmn.GetMenuList();
                    foreach (var auth in GetAuthorizationListByAuthID(DocID))
                    {
                        var item = authList.Where(x => x.ID == auth.ID).FirstOrDefault();
                        if (item != null)
                        {
                            item.IsChecked = true;
                            item.Authorization = auth.Authorization;
                        }
                    }
                    //authList = GetAuthorizationListByAuthID(DocID);


                    userList = GetUserAuthorizationByAuthID(DocID);
                    groupList = GetUserGroupAuthorizationByAuthID(DocID);
                }

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("UserAuthorization", "Exception occured on GetAuthorizationByDocNum, " + ex.Message);
            }
        }

        public void GetAuthorizationByUserID(int UserID, out int DocID, out List<TMS_Menu> authList, out List<UserProfile> userList, out List<GroupSetupInfo> groupList)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            authList = new List<TMS_Menu>();
            userList = new List<UserProfile>();
            groupList = new List<GroupSetupInfo>();
            DocID = 0;
            try
            {
                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = Convert.ToString(UserID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                DataTable dt = HANADAL.GetDataTableByStoredProcedure("GetUserAuthorizationByID", parmList, "UserAuthorization");
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dtRow in dt.Rows)
                    {
                        DocID = Convert.ToInt32(dtRow["AuthorizationID"]);
                    }
                }
                if (DocID > 0)
                {
                    Common cmn = new Common();
                    authList = cmn.GetMenuList();
                    foreach (var auth in GetAuthorizationListByAuthID(DocID))
                    {
                        var item = authList.Where(x => x.ID == auth.ID).FirstOrDefault();
                        if (item != null)
                        {
                            item.IsChecked = true;
                            item.Authorization = auth.Authorization;
                        }
                    }
                    //authList = GetAuthorizationListByAuthID(DocID);
                    //userList = GetUserAuthorizationByAuthID(DocID);

                    groupList = GetUserGroupAuthorizationByAuthID(DocID);
                }

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("UserAuthorization", "Exception occured on GetAuthorizationByUserID, " + ex.Message);
            }
        }

        public void GetAuthorizationByGroupID(int groupID, out int DocID, out List<TMS_Menu> authList, out List<UserProfile> userList, out List<GroupSetupInfo> groupList)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            authList = new List<TMS_Menu>();
            userList = new List<UserProfile>();
            groupList = new List<GroupSetupInfo>();
            DocID = 0;
            try
            {
                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "GROUPID";
                parm.ParameterValue = Convert.ToString(groupID);
                parmList.Add(parm);

                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                DataTable dt = HANADAL.GetDataTableByStoredProcedure("GetGroupAuthorizationByID", parmList, "UserAuthorization");
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dtRow in dt.Rows)
                    {
                        DocID = Convert.ToInt32(dtRow["AuthorizationID"]);
                    }
                }
                if (DocID > 0)
                {
                    Common cmn = new Common();
                    authList = cmn.GetMenuList();
                    foreach (var auth in GetAuthorizationListByAuthID(DocID))
                    {
                        var item = authList.Where(x => x.ID == auth.ID).FirstOrDefault();
                        if (item != null)
                        {
                            item.IsChecked = true;
                            item.Authorization = auth.Authorization;
                        }
                    }
                    //authList = GetAuthorizationListByAuthID(DocID);
                    //userList = GetUserAuthorizationByAuthID(DocID);

                    groupList = GetUserGroupAuthorizationByAuthID(DocID);
                }

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("UserAuthorization", "Exception occured on GetAuthorizationByUserID, " + ex.Message);
            }
        }

        public List<TMS_Menu> GetAuthorizationListByAuthID(int id)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            List<TMS_Menu> authList = new List<TMS_Menu>();
            try
            {
                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = Convert.ToString(id);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                DataTable dt = HANADAL.GetDataTableByStoredProcedure("GetAuthorizationListByAuthID", parmList, "UserAuthorization");
                if (dt.Rows.Count > 0)
                {
                    authList = TranslateDataTableToAuthList(dt);
                }

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("UserAuthorization", "Exception occured on GetAuthorizationListByAuthID, " + ex.Message);
            }
            return authList;
        }
        public List<UserProfile> GetUserAuthorizationByAuthID(int id)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            List<UserProfile> userProfiles = new List<UserProfile>();
            try
            {
                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = Convert.ToString(id);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                DataTable dt_UserProfile = HANADAL.GetDataTableByStoredProcedure("GetUserAuthorizationByAuthID", parmList, "UserAuthorization");
                if (dt_UserProfile.Rows.Count > 0)
                {
                    userProfiles = TranslateDataTableToUserAuthList(dt_UserProfile);
                }

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("UserAuthorization", "Exception occured on GetUserAuthorizationByAuthID, " + ex.Message);
            }
            return userProfiles;
        }
        public List<GroupSetupInfo> GetUserGroupAuthorizationByAuthID(int id)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            List<GroupSetupInfo> groupList = new List<GroupSetupInfo>();
            try
            {
                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = Convert.ToString(id);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                DataTable dt = HANADAL.GetDataTableByStoredProcedure("GetUserGroupAuthorizationByAuthID", parmList, "UserAuthorization");
                if (dt.Rows.Count > 0)
                {
                    groupList = TranslateDataTableToUserGroupAuthList(dt);
                }

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("UserAuthorization", "Exception occured on GetUserGroupAuthorizationByAuthID, " + ex.Message);
            }
            return groupList;
        }


        public void AddAuthorization_Log(TMS_Menu newObject, TMS_Menu previousObject, int DocId, string DocNum, int sessionUserID)
        {
            try
            {
                if (newObject.ID > 0)
                {
                    List<B1SP_Parameter> paramList = TranslateAuthorizationLogToParameterList(newObject, DocId, DocNum, sessionUserID);
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
                            Task.Run(() => HANADAL.AddUpdateDataByStoredProcedure("AddAuthorizationList_Log", paramList, "UserAuthorization"));
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("UserManagement", "Exception occured on AddAuthorization_Log, " + ex.Message);
            }
        }

        public void AddUserAuthorization_Log(UserProfile newObject, UserProfile previousObject, int DocId, string DocNum, int sessionUserID)
        {
            try
            {
                if (newObject.ID > 0)
                {
                    List<B1SP_Parameter> paramList = TranslateUserAuthorizationLogToParameterList(newObject, DocId, DocNum, sessionUserID);
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
                            Task.Run(() => HANADAL.AddUpdateDataByStoredProcedure("AddUserAuthorization_Log", paramList, "UserAuthorization"));
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("UserManagement", "Exception occured on AddUserAuthorization_Log, " + ex.Message);
            }
        }

        public void AddUserGroupAuthorization_Log(GroupSetupInfo newObject, GroupSetupInfo previousObject, int DocId, string DocNum, int sessionUserID)
        {
            try
            {
                List<B1SP_Parameter> paramList = TranslateUserGroupAuthorizationLogToParameterList(newObject, DocId, DocNum, sessionUserID);
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
                        Task.Run(() => HANADAL.AddUpdateDataByStoredProcedure("AddUserGroupAuthorization_Log", paramList, "UserAuthorization"));
                    }

                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("UserManagement", "Exception occured on AddUserAuthorization_Log, " + ex.Message);
            }
        }

        public void GetAuthorizationLogByDocID(string docId, out DataTable dt_Auth, out DataTable dt_User, out DataTable dt_group)
        {
            dt_Auth = new DataTable();
            dt_User = new DataTable();
            dt_group = new DataTable();
            try

            {
                List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();

                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = docId;
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                DataSet ds = HANADAL.GetDataSetByStoredProcedure("GetAuthorizationLogByDocID", parmList, "UserAuthorization");
                dt_Auth = ds.Tables[0];
                dt_User = ds.Tables[1];
                dt_group = ds.Tables[2];
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("UserAuthorization", "Exception occured on GetAuthorizationLogByDocID, " + ex.Message);
            }

        }


        #region Translation
        
        private List<B1SP_Parameter> GetAuthorizationParameterList(int ID, string DocNum, int sessionUserID)
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
                log.InputOutputDocLog("UserAuthorization", "Exception occured on GetAuthorizationParameterList, " + ex.Message);
            }


            return parmList;
        }


        private List<B1SP_Parameter> TranslateAuthToParameterList(int ID, int AuthorizationID, TMS_Menu auth, bool isDeleted)
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
                parm.ParameterName = "AuthorizationID";
                parm.ParameterValue = Convert.ToString(AuthorizationID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "MenuID";
                parm.ParameterValue = Convert.ToString(auth.ID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "UserAuthorizationID";
                parm.ParameterValue = Convert.ToString(auth.Authorization);
                parmList.Add(parm);


                parm = new B1SP_Parameter();
                parm.ParameterName = "IsDeleted";
                parm.ParameterValue = Convert.ToString(isDeleted);
                parmList.Add(parm);


            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("UserAuthorization", "Exception occured on TranslateAuthToParameterList, " + ex.Message);
            }


            return parmList;
        }

        private List<B1SP_Parameter> TranslateUserAuthToParameterList(int ID, int AuthorizationID, int userID, bool isDeleted)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {

                B1SP_Parameter parm = new B1SP_Parameter();

                parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = Convert.ToString(ID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "AuthorizationID";
                parm.ParameterValue = Convert.ToString(AuthorizationID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "UserID";
                parm.ParameterValue = Convert.ToString(userID);
                parmList.Add(parm);



                parm = new B1SP_Parameter();
                parm.ParameterName = "IsDeleted";
                parm.ParameterValue = Convert.ToString(isDeleted);
                parmList.Add(parm);


            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("UserAuthorization", "Exception occured on TranslateAuthToParameterList, " + ex.Message);
            }


            return parmList;
        }

        private List<B1SP_Parameter> TranslateAuthGroupToParameterList(int ID, int AuthorizationID, GroupSetupInfo group, bool isDeleted)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {
                if (group == null)
                    return null;

                B1SP_Parameter parm = new B1SP_Parameter();

                parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = Convert.ToString(ID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "AuthorizationID";
                parm.ParameterValue = Convert.ToString(AuthorizationID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "UserGroupID";
                parm.ParameterValue = Convert.ToString(group.ID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "IsDeleted";
                parm.ParameterValue = Convert.ToString(isDeleted);
                parmList.Add(parm);


            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("UserAuthorization", "Exception occured on TranslateAuthGroupToParameterList, " + ex.Message);
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

        private List<TMS_Menu> TranslateDataTableToAuthList(DataTable dt)
        {
            List<TMS_Menu> list = new List<TMS_Menu>();

            try
            {

                foreach (DataRow dtRow in dt.Rows)
                {
                    TMS_Menu menu = new TMS_Menu();
                    menu._ID = Convert.ToInt32(dtRow["ID"]);
                    menu.ID = Convert.ToInt32(dtRow["MenuID"]);
                    menu.AuthorizationID = Convert.ToInt32(dtRow["AuthorizationID"]);
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

        private List<GroupSetupInfo> TranslateDataTableToUserGroupAuthList(DataTable dt)
        {
            List<GroupSetupInfo> list = new List<GroupSetupInfo>();

            try
            {

                foreach (DataRow dtRow in dt.Rows)
                {
                    GroupSetupInfo desig = new GroupSetupInfo();
                    desig.ID = Convert.ToInt32(dtRow["UserGroupID"]);
                    desig.AuthorizationTableID = Convert.ToInt32(dtRow["ID"]);
                    list.Add(desig);
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


        private List<B1SP_Parameter> TranslateAuthorizationLogToParameterList(TMS_Menu menu, int DocId, string Docnum, int sessionUser)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {
                if (menu == null)
                    return null;

                B1SP_Parameter parm = new B1SP_Parameter();

                parm = new B1SP_Parameter();
                parm.ParameterName = "AuthorizationID";
                parm.ParameterValue = Convert.ToString(DocId);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "DocNum";
                parm.ParameterValue = Convert.ToString(Docnum);
                parmList.Add(parm);


                parm = new B1SP_Parameter();
                parm.ParameterName = "Menu";
                Common cmn = new Common();
                parm.ParameterValue = Convert.ToString(cmn.GetMenuList().Where(x => x.ID == menu.ID).Select(x => x.Name).FirstOrDefault());
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "UserAuthorization_Previous";
                parm.ParameterValue = Convert.ToString(((Enums.UserAuthorization)menu.Authorization));
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "IsChecked_Previous";
                parm.ParameterValue = Convert.ToString(menu.IsDeleted);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "UserAuthorization_New";
                parm.ParameterValue = Convert.ToString(((Enums.UserAuthorization)menu.Authorization));
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "IsChecked_New";
                parm.ParameterValue = Convert.ToString(menu.IsDeleted);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CREATEDBY";
                parm.ParameterValue = Convert.ToString(sessionUser);
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
                log.InputOutputDocLog("UserManagament", "Exception occured on TranslateAuthorizationLogToParameterList, " + ex.Message);
            }


            return parmList;
        }

        private List<B1SP_Parameter> TranslateUserAuthorizationLogToParameterList(UserProfile user, int DocId, string Docnum, int sessionUser)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {
                if (user == null)
                    return null;

                B1SP_Parameter parm = new B1SP_Parameter();

                parm = new B1SP_Parameter();
                parm.ParameterName = "AuthorizationID";
                parm.ParameterValue = Convert.ToString(DocId);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "DocNum";
                parm.ParameterValue = Convert.ToString(Docnum);
                parmList.Add(parm);



                parm = new B1SP_Parameter();
                parm.ParameterName = "UserID_Previous";
                parm.ParameterValue = Convert.ToString(user.ID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "IsChecked_Previous";
                parm.ParameterValue = Convert.ToString(user.ISDELETED);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "UserID_New";
                parm.ParameterValue = Convert.ToString(user.ID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "IsChecked_New";
                parm.ParameterValue = Convert.ToString(user.ISDELETED);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CREATEDBY";
                parm.ParameterValue = Convert.ToString(sessionUser);
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
                log.InputOutputDocLog("UserManagament", "Exception occured on TranslateAuthorizationLogToParameterList, " + ex.Message);
            }


            return parmList;
        }

        private List<B1SP_Parameter> TranslateUserGroupAuthorizationLogToParameterList(GroupSetupInfo user, int DocId, string Docnum, int sessionUser)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {
                if (user == null)
                    return null;

                B1SP_Parameter parm = new B1SP_Parameter();

                parm = new B1SP_Parameter();
                parm.ParameterName = "AuthorizationID";
                parm.ParameterValue = Convert.ToString(DocId);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "DocNum";
                parm.ParameterValue = Convert.ToString(Docnum);
                parmList.Add(parm);


                parm = new B1SP_Parameter();
                parm.ParameterName = "UserGroup_Previous";
                parm.ParameterValue = Convert.ToString(user.ID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "IsChecked_Previous";
                parm.ParameterValue = Convert.ToString(user.ISDELETED);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "UserGroup_New";
                parm.ParameterValue = Convert.ToString(user.ID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "IsChecked_New";
                parm.ParameterValue = Convert.ToString(user.ISDELETED);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CREATEDBY";
                parm.ParameterValue = Convert.ToString(sessionUser);
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
                log.InputOutputDocLog("UserManagament", "Exception occured on TranslateAuthorizationLogToParameterList, " + ex.Message);
            }


            return parmList;
        }



        #endregion

    }
}