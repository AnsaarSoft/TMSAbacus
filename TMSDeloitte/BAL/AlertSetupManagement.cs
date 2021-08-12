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
    public class AlertSetupManagement
    {

        public bool AddUpdateAlertSetup(AlertSetup obj, List<UserProfile> userList, List<GroupSetupInfo> groupList, int sessionUserID, out string Msg)
        {

           

            bool isSuccess = false;
            bool isAddOccured = false;
            bool isUpdateOccured = false;
            bool isDeleteOccured = false;
            Msg = "";
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            B1SP_Parameter parm = new B1SP_Parameter();
            HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
            string alertName = "";

            AlertSetup setup = new AlertSetup();
            List<UserProfile> userList_Previous = new List<UserProfile>();
            List<GroupSetupInfo> groupList_Previous = new List<GroupSetupInfo>();
            try
            {

                var filtererdUserList = userList.Where(x => x.ISDELETED == false).ToList();
                var duplicatesUser = filtererdUserList.GroupBy(x => x.ID).Where(g => g.Count() > 1).Select(y => y.Key).ToList();
                if (duplicatesUser.Count > 0)
                {
                    Msg = "Duplicate user exist!";
                    return false;
                }

                var filtererdUGroupList = groupList.Where(x => x.ISDELETED == false && x.ID != 0).ToList();
                var duplicatesGroup = filtererdUGroupList.GroupBy(x => x.ID).Where(g => g.Count() > 1).Select(y => y.Key).ToList();
                if (duplicatesGroup.Count > 0)
                {
                    Msg = "Duplicate group exist!";
                    return false;
                }


                Common cmn = new Common();
                int AlertSetupID = 0;
                int DocId = obj.ID;

                if (!ValidateAlertSetup(obj,userList, groupList, out Msg))
                    return false;

                string docNum = "";
                if (DocId == 0)
                {
                    int no = 1;
                    
                    List<string> docNumList = cmn.GetDocNum("GetAlertSetupDocNum", "AlertSetupManagement");
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
                    obj.DocNum = docNum;
                }
                else
                    docNum= obj.DocNum ;

                bool isValidate = true;
                if (DocId > 0)
                {
                   
                    GetAlertSetupByDocNum(docNum, out setup, out userList_Previous, out groupList_Previous);

                    AddAlertSetup_Log(obj, setup, DocId);

                    if (setup.AlertName.ToLower() == obj.AlertName.ToLower())
                        isValidate = false;

                    //parmList = new List<B1SP_Parameter>();
                    //parm = new B1SP_Parameter();
                    //parm.ParameterName = "DocNum";
                    //parm.ParameterValue = Convert.ToString(obj.DocNum);
                    //parmList.Add(parm);
                    //DataSet ds = HANADAL.GetDataSetByStoredProcedure("GetAlertSetupByDocNum", parmList, "GroupSetupManagement");
                    //if (ds.Tables.Count > 0)
                    //{
                    //    if (ds.Tables[0].Rows.Count > 0)
                    //    {
                    //        foreach (DataRow dtRow in ds.Tables[0].Rows)
                    //        {
                    //            alertName = Convert.ToString(dtRow["AlertName"]);
                    //            if (alertName.ToLower() == obj.AlertName.ToLower())
                    //                isValidate = false;
                    //            break;
                    //        }
                    //    }
                    //}
                }
                if (isValidate)
                {
                    parmList = new List<B1SP_Parameter>();
                    parm = new B1SP_Parameter();
                    parm.ParameterName = "AlertName";
                    parm.ParameterValue = Convert.ToString(obj.AlertName);
                    parm.ParameterType = DBTypes.String.ToString();
                    parmList.Add(parm);

                    DataTable dt = HANADAL.GetDataTableByStoredProcedure("ValidateAlertSetupName", parmList, "GroupSetupManagement");
                    if (dt.Rows.Count > 0)
                    {
                        Msg = "Alert Name Already Exist!";
                        return false;
                    }
                }

               
                DataTable dt_AlertSetup = HANADAL.AddUpdateDataByStoredProcedure("AddUpdateAlertSetup", GetAlertSetupParameterList(obj), "AlertSetupManagement");
                if (dt_AlertSetup.Rows.Count == 0)
                    throw new Exception("Exception occured when add/update alert setup");
                else
                {

                    foreach (DataRow dtRow in dt_AlertSetup.Rows)
                    {
                        isAddOccured = true;
                        AlertSetupID = Convert.ToInt32(dtRow["ID"]);
                        docNum = Convert.ToString(dtRow["DocNum"]);
                    }
                }

                if (AlertSetupID == 0)
                    throw new Exception("Exception occured when add/update alert setup");

                if (DocId == 0)
                {
                    foreach (var user in userList)
                    {
                        HANADAL.AddUpdateDataByStoredProcedure("AddUpdateUserAlertSetup", TranslateUserAlertSetupToParameterList(0, AlertSetupID, user.ID,user.IsNotification,user.IsEmail, false), "AlertSetupManagement");
                    }
                    foreach (var group in groupList)
                    {
                        HANADAL.AddUpdateDataByStoredProcedure("AddUpdateUserGroupAlertSetup", TranslateAlertSetuoGroupToParameterList(0, AlertSetupID, group.ID, group.IsNotification, group.IsEmail, false), "AlertSetupManagement");
                    }
                }
                else
                {

                    //for User
                    foreach (var user in userList)
                    {
                        var previous_item = userList_Previous.Where(x => x.ID == user.ID).FirstOrDefault();
                        int ID = 0;
                        if (previous_item != null)
                        {
                            isUpdateOccured = true;
                            ID = Convert.ToInt32(previous_item.AlertSetupTableID);
                        }
                        AddUserAlertSetup_Log(user, previous_item, DocId);
                        HANADAL.AddUpdateDataByStoredProcedure("AddUpdateUserAlertSetup", TranslateUserAlertSetupToParameterList(ID, AlertSetupID, user.ID, user.IsNotification, user.IsEmail, false), "AlertSetupManagement");

                    }
                    List<UserProfile> missingUserList = userList_Previous.Where(n => !userList.Any(o => o.ID == n.ID && o.ISDELETED==n.ISDELETED)).ToList();
                    foreach (var user in missingUserList)
                    {
                        UserProfile previous_item = new UserProfile();
                        user.ISDELETED = false;
                        previous_item.ISDELETED = true;
                        isDeleteOccured = true;
                        AddUserAlertSetup_Log(user, previous_item, DocId);
                        HANADAL.AddUpdateDataByStoredProcedure("AddUpdateUserAlertSetup", TranslateUserAlertSetupToParameterList(Convert.ToInt32(user.AlertSetupTableID), AlertSetupID, user.ID, user.IsNotification, user.IsEmail, true), "AlertSetupManagement");
                    }


                    //For Group
                    foreach (var group in groupList)
                    {
                        var item = groupList_Previous.Where(x => x.ID == group.ID).FirstOrDefault();
                        int ID = 0;
                        if (item != null)
                        {
                            isUpdateOccured = true;
                            ID = item.AlertSetupTableID;

                        }
                        AddUserGroupAlert_Log(group, item, DocId);
                        HANADAL.AddUpdateDataByStoredProcedure("AddUpdateUserGroupAlertSetup", TranslateAlertSetuoGroupToParameterList(ID, AlertSetupID, group.ID,group.IsNotification,group.IsEmail, false), "AlertSetupManagement");

                    }
                    List<GroupSetupInfo> missingGroupList = groupList_Previous.Where(n => !groupList.Any(o => o.ID == n.ID && o.ISDELETED == n.ISDELETED)).ToList();
                    foreach (var group in missingGroupList)
                    {
                        GroupSetupInfo previous_item = new GroupSetupInfo();
                        group.ISDELETED = false;
                        previous_item.ISDELETED = true;
                        isDeleteOccured = true;
                        AddUserGroupAlert_Log(group, previous_item, DocId);
                        HANADAL.AddUpdateDataByStoredProcedure("AddUpdateUserGroupAlertSetup", TranslateAlertSetuoGroupToParameterList(group.AlertSetupTableID, AlertSetupID, group.ID, group.IsNotification, group.IsEmail, true), "AlertSetupManagement");

                    }



                }
                Msg = "Successfully Added/Updated";

                //For Master Log
                Task.Run(() => cmn.AddMasterLog(isAddOccured, isUpdateOccured, isDeleteOccured, nameof(Enums.FormsName.AlertSetup), sessionUserID, "AlertSetupManagement"));
                // End MAster Log


                isSuccess = true;
            }
            catch (Exception ex)
            {
                Msg = "Exception occured when add/update alert setup!";
                isSuccess = false;
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("AlertSetupManagement", "Exception occured on alert setup, " + ex.Message);
            }

            return isSuccess;
        }

        private bool ValidateAlertSetup( AlertSetup obj,List<UserProfile> userList, List<GroupSetupInfo> groupList, out string Msg)
        {
            Msg = "";

            if(string.IsNullOrEmpty(obj.AlertName.Replace(" ","")))
            {
                Msg = "Please enter alert name";
                return false;
            }
            if (string.IsNullOrEmpty(obj.Query.Replace(" ", "")))
            {
                Msg = "Please enter query";
                return false;
            }
            if (obj.Frequency==0)
            {
                Msg = "Please enter valid frequency";
                return false;
            }
            if (string.IsNullOrEmpty(obj.FrequencyType.Replace(" ", "")))
            {
                Msg = "Please enter frequency type";
                return false;
            }
            if (userList == null && groupList == null)
            {
                Msg = "Please select atleast 1 user and 1 group";
                return false;
            }
            else
            {
                if (groupList.Count == 0 && userList.Count == 0)
                {
                    Msg = "Please select atleast 1 user and 1 group";
                    return false;
                }
            }
            return true;
        }

        public void GetAlertSetupByDocNum( string DocNum,out AlertSetup obj , out List<UserProfile> userList, out List<GroupSetupInfo> groupList)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            obj = new AlertSetup();
            userList = new List<UserProfile>();
            groupList = new List<GroupSetupInfo>();
            try
            {
                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "DocNum";
                parm.ParameterValue = Convert.ToString(DocNum);
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                DataSet ds = HANADAL.GetDataSetByStoredProcedure("GetAlertSetupByDocNum", parmList, "AlertSetupManagement");
                if (ds.Tables.Count > 0)
                {

                    obj = TranslateDataTableToAlertSetup(ds.Tables[0]);
                    userList = TranslateDataTableToUserList(ds.Tables[1]);
                    groupList = TranslateDataTableToUserGroupList(ds.Tables[2]);
                }
                

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("AlertSetupManagement", "Exception occured on GetAlertSetupByDocNum, " + ex.Message);
            }
        }

        public List<UserProfile> GetAlertSetupByID(int id)
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
                DataTable dt_UserProfile = HANADAL.GetDataTableByStoredProcedure("GetAlertSetupByAuthID", parmList, "AlertSetupManagement");
                if (dt_UserProfile.Rows.Count > 0)
                {
                    userProfiles = TranslateDataTableToUserList(dt_UserProfile);
                }

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("AlertSetupManagement", "Exception occured on GetAlertSetupByAuthID, " + ex.Message);
            }
            return userProfiles;
        }
        public List<GroupSetupInfo> GetUserGroupAlertSetupByAuthID(int id)
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
                DataTable dt = HANADAL.GetDataTableByStoredProcedure("GetUserGroupAlertSetupByAuthID", parmList, "AlertSetupManagement");
                if (dt.Rows.Count > 0)
                {
                    groupList = TranslateDataTableToUserGroupList(dt);
                }

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("AlertSetupManagement", "Exception occured on GetUserGroupAlertSetupByAuthID, " + ex.Message);
            }
            return groupList;
        }

        public DataTable GetAlertSetupAllDocumentsList()
        {
            DataTable dt = new DataTable();
            Common cmn = new Common();
            try

            {
                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                dt = HANADAL.GetDataTableByStoredProcedure("GetAlertSetupAllDocuments", "AlertSetupManagement");
                dt = cmn.RemoveDuplicateRows(dt, "DocNum");
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("AlertSetupManagement", "Exception occured on GetAlertSetupAllDocumentsList, " + ex.Message);
            }

            return dt;
        }


        public void AddAlertSetup_Log(AlertSetup newObject, AlertSetup previousObject,int docID)
        {
            try
            {
                if (newObject.ID > 0)
                {
                    List<B1SP_Parameter> paramList = TranslateAlertSetupLogToParameterList(newObject, docID);
                    bool isChangeOccured = false;
                    if (previousObject != null)
                    {
                        foreach (PropertyCompareResult resultItem in PropertyCompare.Compare(previousObject, newObject))
                        {
                           
                            string Name = resultItem.Name;
                            object PreviousValue = resultItem.OldValue;
                            object NewValue = resultItem.NewValue;

                            switch (Name)
                            {
                                case "AlertName":
                                    isChangeOccured = true;
                                    //PreviousValue = Convert.ToString(((Enums.AlertSetup)PreviousValue));
                                    //NewValue = Convert.ToString(((Enums.AlertSetup)NewValue));

                                    paramList.Where(x => x.ParameterName == "AlertName_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                    paramList.Where(x => x.ParameterName == "AlertName_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();

                                    break;

                                case "Query":
                                    isChangeOccured = true;
                                    paramList.Where(x => x.ParameterName == "Query_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                    paramList.Where(x => x.ParameterName == "Query_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();

                                    break;

                                case "Frequency":
                                    isChangeOccured = true;
                                    paramList.Where(x => x.ParameterName == "Frequency_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                    paramList.Where(x => x.ParameterName == "Frequency_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();
                                    break;

                                case "FrequencyType":
                                    isChangeOccured = true;
                                    paramList.Where(x => x.ParameterName == "FrequencyType_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                    paramList.Where(x => x.ParameterName == "FrequencyType_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();
                                    break;

                                case "IsActive":
                                    isChangeOccured = true;
                                    paramList.Where(x => x.ParameterName == "IsActive_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                    paramList.Where(x => x.ParameterName == "IsActive_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();
                                    break;

                                case "IsDeleted":
                                    isChangeOccured = true;
                                    paramList.Where(x => x.ParameterName == "IsDeleted_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                    paramList.Where(x => x.ParameterName == "IsDeleted_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();
                                    break;
                            }

                        }

                        if (isChangeOccured)
                        {
                            HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                            Task.Run(() => HANADAL.AddUpdateDataByStoredProcedure("AddUpdateAlertSetup_Log", paramList, "AlertSetupManagement"));
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("AlertSetupManagement", "Exception occured on AddUpdateAlertSetup_Log, " + ex.Message);
            }
        }

        public void AddUserAlertSetup_Log(UserProfile newObject, UserProfile previousObject, int docID)
        {
            try
            {
                if (newObject.ID > 0)
                {
                    List<B1SP_Parameter> paramList = TranslateAlertSetupLogToParameterList(newObject, docID);
                    bool isChangeOccured = false;
                    if (previousObject != null)
                    {
                        foreach (PropertyCompareResult resultItem in PropertyCompare.Compare(previousObject, newObject))
                        {
                            
                            string Name = resultItem.Name;
                            object PreviousValue = resultItem.OldValue;
                            object NewValue = resultItem.NewValue;

                            switch (Name)
                            {

                                case "IsNotification":
                                    isChangeOccured = true;
                                    paramList.Where(x => x.ParameterName == "IsNotification_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                    paramList.Where(x => x.ParameterName == "IsNotification_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();
                                    break;
                                case "IsEmail":
                                    isChangeOccured = true;
                                    paramList.Where(x => x.ParameterName == "IsEmail_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                    paramList.Where(x => x.ParameterName == "IsEmail_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();
                                    break;
                                case "ISDELETED":
                                    isChangeOccured = true;
                                    paramList.Where(x => x.ParameterName == "IsChecked_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                    paramList.Where(x => x.ParameterName == "IsChecked_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();
                                    break;
                               
                            }

                        }

                        if (isChangeOccured)
                        {
                            HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                            Task.Run(() => HANADAL.AddUpdateDataByStoredProcedure("AddUpdateUserAlertSetup_Log", paramList, "AlertSetupManagement"));
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("AlertSetupManagement", "Exception occured on AddAlertSetup_Log, " + ex.Message);
            }
        }

        public void AddUserGroupAlert_Log(GroupSetupInfo newObject, GroupSetupInfo previousObject, int docID)
        {
            try
            {
                List<B1SP_Parameter> paramList = TranslateUserGroupAlertSetupLogToParameterList(newObject, docID);
                bool isChangeOccured = false;
                if (previousObject != null)
                {
                    foreach (PropertyCompareResult resultItem in PropertyCompare.Compare(previousObject, newObject))
                    {
                        
                        string Name = resultItem.Name;
                        object PreviousValue = resultItem.OldValue;
                        object NewValue = resultItem.NewValue;

                        switch (Name)
                        {

                            case "IsNotification":
                                isChangeOccured = true;
                                paramList.Where(x => x.ParameterName == "IsNotification_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                paramList.Where(x => x.ParameterName == "IsNotification_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();
                                break;
                            case "IsEmail":
                                isChangeOccured = true;
                                paramList.Where(x => x.ParameterName == "IsEmail_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                paramList.Where(x => x.ParameterName == "IsEmail_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();
                                break;
                            case "ISDELETED":
                                isChangeOccured = true;
                                paramList.Where(x => x.ParameterName == "IsChecked_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                paramList.Where(x => x.ParameterName == "IsChecked_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();
                                break;


                        }

                    }

                    if (isChangeOccured)
                    {
                        HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                        Task.Run(() => HANADAL.AddUpdateDataByStoredProcedure("AddUpdateUserGroupAlertSetup_Log", paramList, "AlertSetupManagement"));
                    }

                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("AlertSetupManagement", "Exception occured on AddAlertSetup_Log, " + ex.Message);
            }
        }

        public void GetAlertSetupLogByDocID(string docId, out DataTable dt_Auth, out DataTable dt_User, out DataTable dt_group)
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
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                DataSet ds = HANADAL.GetDataSetByStoredProcedure("GetAlertSetupLogByDocID", parmList, "AlertSetupManagement");
                dt_Auth = ds.Tables[0];
                dt_User = ds.Tables[1];
                dt_group = ds.Tables[2];
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("AlertSetupManagement", "Exception occured on GetAlertSetupLogByDocID, " + ex.Message);
            }

        }

        public void GetLogByDocNum(string docID, out DataTable dt_Setup, out DataTable dt_User, out DataTable dt_group)
        {
            dt_Setup = new DataTable();
            dt_User = new DataTable();
            dt_group = new DataTable();
            try

            {
                List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();

                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "DocID";
                parm.ParameterValue = docID;
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                DataSet ds = HANADAL.GetDataSetByStoredProcedure("GetAlertSetupLogByDocID", parmList, "AlertSetupManagement");
                dt_Setup = ds.Tables[0];
                dt_User = ds.Tables[1];
                dt_group = ds.Tables[2];
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("AlertSetupManagement", "Exception occured on GetLogByDocNum, " + ex.Message);
            }

        }

        #region Translation

        private List<B1SP_Parameter> GetAlertSetupParameterList(AlertSetup obj)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {

                B1SP_Parameter parm = new B1SP_Parameter();

                parm = new B1SP_Parameter();
                parm.ParameterName = "ID"; 
                parm.ParameterValue = Convert.ToString(obj.ID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "DocNum";
                parm.ParameterValue = Convert.ToString(obj.DocNum);
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "AlertName";
                parm.ParameterValue = Convert.ToString(obj.AlertName);
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "Query";
                parm.ParameterValue = Convert.ToString(obj.Query);
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "Frequency";
                parm.ParameterValue = Convert.ToString(obj.Frequency);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "FrequencyType";
                parm.ParameterValue = Convert.ToString(obj.FrequencyType);
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "IsActive";
                parm.ParameterValue = Convert.ToString(obj.IsActive);
                parm.ParameterType = DBTypes.Bool.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "IsDeleted";
                parm.ParameterValue = Convert.ToString(obj.IsDeleted);
                parm.ParameterType = DBTypes.Bool.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CreatedBy";
                parm.ParameterValue = Convert.ToString(obj.CreatedBy);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CreatedDate";
                parm.ParameterValue = DateTime.Now.ToString("yyyy-MM-dd HH:MM:s");
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "UpdatedBy";
                parm.ParameterValue = Convert.ToString(obj.UpdatedBy);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "UpdatedDate";
                parm.ParameterValue = DateTime.Now.ToString("yyyy-MM-dd HH:MM:s");
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("AlertSetupManagement", "Exception occured on GetAlertSetupParameterList, " + ex.Message);
            }


            return parmList;
        }
      
        private List<B1SP_Parameter> TranslateUserAlertSetupToParameterList(int ID, int HeaderID, int userID,bool IsNotification,bool IsEmail, bool isDeleted)
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
                parm.ParameterName = "HeaderID";
                parm.ParameterValue = Convert.ToString(HeaderID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "UserID";
                parm.ParameterValue = Convert.ToString(userID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "IsNotification";
                parm.ParameterValue = Convert.ToString(IsNotification);
                parm.ParameterType = DBTypes.Bool.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "IsEmail";
                parm.ParameterValue = Convert.ToString(IsEmail);
                parm.ParameterType = DBTypes.Bool.ToString();
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
                log.InputOutputDocLog("AlertSetupManagement", "Exception occured on TranslateAuthToParameterList, " + ex.Message);
            }


            return parmList;
        }

        private List<B1SP_Parameter> TranslateAlertSetuoGroupToParameterList(int ID, int AlertSetupID, int groupID,bool IsNotification,bool IsEmail, bool isDeleted)
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
                parm.ParameterName = "HeaderID";
                parm.ParameterValue = Convert.ToString(AlertSetupID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "UserGroupID";
                parm.ParameterValue = Convert.ToString(groupID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "IsNotification";
                parm.ParameterValue = Convert.ToString(IsNotification);
                parm.ParameterType = DBTypes.Bool.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "IsEmail";
                parm.ParameterValue = Convert.ToString(IsEmail);
                parm.ParameterType = DBTypes.Bool.ToString();
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
                log.InputOutputDocLog("AlertSetupManagement", "Exception occured on TranslateAuthGroupToParameterList, " + ex.Message);
            }


            return parmList;
        }

        private AlertSetup TranslateDataTableToAlertSetup(DataTable dt)
        {
            AlertSetup setup = new AlertSetup();
            try
            {

                foreach (DataRow dtRow in dt.Rows)
                {
                    setup.ID = Convert.ToInt32(dtRow["ID"]);
                    setup.AlertName = Convert.ToString(dtRow["AlertName"]);
                    setup.DocNum = Convert.ToString(dtRow["DocNum"]);
                    setup.Frequency = Convert.ToInt32(dtRow["Frequency"]);
                    setup.FrequencyType = Convert.ToString(dtRow["FrequencyType"]);
                    setup.IsActive = Convert.ToBoolean(dtRow["IsActive"]);
                    setup.Query = Convert.ToString(dtRow["Query"]);
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("AlertSetupManagement", "Exception occured on TranslateDataTableToUserProfile, " + ex.Message);
            }

            return setup;
        }

        private List<UserProfile> TranslateDataTableToUserList(DataTable dt)
        {
            List<UserProfile> list = new List<UserProfile>();
            BAL.UserManagement userMgt = new UserManagement();
            try
            {
                int sno = 0;
                foreach (DataRow dtRow in dt.Rows)
                {
                    int userCode = Convert.ToInt32(dtRow["UserID"]);

                    UserProfile userProfile = new UserProfile();
                    if (userCode != 0)
                    {
                        UserProfile userdata = new UserProfile();
                        userProfile = userMgt.GetUserByID(Convert.ToInt32(userCode));
                    }

                    sno = sno + 1;
                    userProfile.SNO = sno;
                    userProfile.KEY = Guid.NewGuid().ToString();
                    userProfile.USER_CODE = Convert.ToInt32(dtRow["UserID"]);
                    userProfile.AlertSetupHeaderTableID = Convert.ToInt32(dtRow["HeaderID"]);
                    userProfile.AlertSetupTableID = Convert.ToInt32(dtRow["ID"]);
                    userProfile.IsNotification = Convert.ToBoolean(dtRow["IsNotification"]);
                    userProfile.IsEmail = Convert.ToBoolean(dtRow["IsEmail"]);
                    userProfile.ISDELETED = Convert.ToBoolean(dtRow["IsDeleted"]);
                    list.Add(userProfile);
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("AlertSetupManagement", "Exception occured on TranslateDataTableToUserProfile, " + ex.Message);
            }

            return list;
        }

       
        private List<GroupSetupInfo> TranslateDataTableToUserGroupList(DataTable dt)
        {
            List<GroupSetupInfo> list = new List<GroupSetupInfo>();
            BAL.GroupSetupManagement mgt = new GroupSetupManagement();
            try
            {
                int sno = 0;

                foreach (DataRow dtRow in dt.Rows)
                {
                    GroupSetupInfo desig = new GroupSetupInfo();
                    desig = mgt.GetGroupSetup(Convert.ToInt32(dtRow["UserGroupID"]))[0];
                    
                    sno = sno + 1;
                    desig.SNO = sno;
                    desig.KEY = Guid.NewGuid().ToString();
                    //desig.GROUPCODE = Convert.ToString(dtRow["UserGroupID"]);
                    
                    desig.AlertSetupHeaderTableID = Convert.ToInt32(dtRow["HeaderID"]);
                    desig.AlertSetupTableID = Convert.ToInt32(dtRow["ID"]);
                    desig.IsNotification = Convert.ToBoolean(dtRow["IsNotification"]);
                    desig.IsEmail = Convert.ToBoolean(dtRow["IsEmail"]);
                    desig.ISDELETED = Convert.ToBoolean(dtRow["IsDeleted"]);
                    list.Add(desig);
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("AlertSetupManagement", "Exception occured on TranslateDataTableToUserProfile, " + ex.Message);
            }

            return list;
        }
        
        private List<B1SP_Parameter> TranslateAlertSetupLogToParameterList(AlertSetup obj,int docID)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {
                if (obj == null)
                    return null;

                B1SP_Parameter parm = new B1SP_Parameter();

                parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = Convert.ToString(0);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "DocID";
                parm.ParameterValue = Convert.ToString(docID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "DocNum";
                parm.ParameterValue = Convert.ToString(obj.DocNum);
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);


                parm = new B1SP_Parameter();
                parm.ParameterName = "AlertName_Previous";
                parm.ParameterValue = Convert.ToString(obj.AlertName);
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "Query_Previous";
                parm.ParameterValue = Convert.ToString(obj.Query);
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "Frequency_Previous";
                parm.ParameterValue = Convert.ToString(obj.Frequency);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "FrequencyType_Previous";
                parm.ParameterValue = Convert.ToString(obj.FrequencyType);
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "IsActive_Previous";
                parm.ParameterValue = Convert.ToString(obj.IsActive);
                parm.ParameterType = DBTypes.Bool.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "IsDeleted_Previous";
                parm.ParameterValue = Convert.ToString(obj.IsDeleted);
                parm.ParameterType = DBTypes.Bool.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "AlertName_New";
                parm.ParameterValue = Convert.ToString(obj.AlertName);
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "Query_New";
                parm.ParameterValue = Convert.ToString(obj.Query);
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "Frequency_New";
                parm.ParameterValue = Convert.ToString(obj.Frequency);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "FrequencyType_New";
                parm.ParameterValue = Convert.ToString(obj.FrequencyType);
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "IsActive_New";
                parm.ParameterValue = Convert.ToString(obj.IsActive);
                parm.ParameterType = DBTypes.Bool.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "IsDeleted_New";
                parm.ParameterValue = Convert.ToString(obj.IsDeleted);
                parm.ParameterType = DBTypes.Bool.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CreatedBy";
                parm.ParameterValue = Convert.ToString(obj.CreatedBy);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CreatedDate";
                parm.ParameterValue = DateTime.Now.ToString("yyyy-MM-dd HH:MM:s");
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("AlertSetupManagement", "Exception occured on TranslateAlertSetupLogToParameterList, " + ex.Message);
            }


            return parmList;
        }

        private List<B1SP_Parameter> TranslateAlertSetupLogToParameterList(UserProfile user, int docID)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {
                if (user == null)
                    return null;

                B1SP_Parameter parm = new B1SP_Parameter();

                parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = Convert.ToString(0);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "DocID";
                parm.ParameterValue = Convert.ToString(docID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "HeaderID";
                parm.ParameterValue = Convert.ToString(user.AlertSetupHeaderTableID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);


                parm = new B1SP_Parameter();
                parm.ParameterName = "UserID";
                parm.ParameterValue = Convert.ToString(user.ID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "IsNotification_Previous";
                parm.ParameterValue = Convert.ToString(user.IsNotification);
                parm.ParameterType = DBTypes.Bool.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "IsEmail_Previous";
                parm.ParameterValue = Convert.ToString(user.IsEmail);
                parm.ParameterType = DBTypes.Bool.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "IsDeleted_Previous";
                parm.ParameterValue = Convert.ToString(user.ISDELETED);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "IsNotification_New";
                parm.ParameterValue = Convert.ToString(user.IsNotification);
                parm.ParameterType = DBTypes.Bool.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "IsEmail_New";
                parm.ParameterValue = Convert.ToString(user.IsEmail);
                parm.ParameterType = DBTypes.Bool.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "IsDeleted_New";
                parm.ParameterValue = Convert.ToString(user.ISDELETED);
                parm.ParameterType = DBTypes.Bool.ToString();
                parmList.Add(parm);

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("AlertSetupManagement", "Exception occured on TranslateAlertSetupLogToParameterList, " + ex.Message);
            }


            return parmList;
        }

        private List<B1SP_Parameter> TranslateUserGroupAlertSetupLogToParameterList(GroupSetupInfo user, int docID)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {
                if (user == null)
                    return null;

                B1SP_Parameter parm = new B1SP_Parameter();

                parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = Convert.ToString(0);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "DocID";
                parm.ParameterValue = Convert.ToString(docID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "HeaderID";
                parm.ParameterValue = Convert.ToString(user.AlertSetupHeaderTableID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "UserGroupID";
                parm.ParameterValue = Convert.ToString(user.ID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);
                

                parm = new B1SP_Parameter();
                parm.ParameterName = "IsNotification_Previous";
                parm.ParameterValue = Convert.ToString(user.IsNotification);
                parm.ParameterType = DBTypes.Bool.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "IsEmail_Previous";
                parm.ParameterValue = Convert.ToString(user.IsEmail);
                parm.ParameterType = DBTypes.Bool.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "IsDeleted_Previous";
                parm.ParameterValue = Convert.ToString(user.ISDELETED);
                parm.ParameterType = DBTypes.Bool.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "IsNotification_New";
                parm.ParameterValue = Convert.ToString(user.IsNotification);
                parm.ParameterType = DBTypes.Bool.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "IsEmail_New";
                parm.ParameterValue = Convert.ToString(user.IsEmail);
                parm.ParameterType = DBTypes.Bool.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "IsDeleted_New";
                parm.ParameterType = DBTypes.Bool.ToString();
                parmList.Add(parm);
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("AlertSetupManagement", "Exception occured on TranslateAlertSetupLogToParameterList, " + ex.Message);
            }


            return parmList;
        }



        #endregion

    }
}