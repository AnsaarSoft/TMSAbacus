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
    public class GroupSetupManagement
    {

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
                log.InputOutputDocLog("GroupSetupManagement", "Exception occured on TranslateGroupSetupToParameterList, " + ex.Message);
            }


            return parmList;
        }

        private List<B1SP_Parameter> TranslateGroupSetupIDToParameterList(int GROUPSETUP_ID)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {

                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "GROUPSETUP_ID";
                parm.ParameterValue = Convert.ToString(GROUPSETUP_ID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);


            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("GroupSetupManagement", "Exception occured on TranslateGroupSetupToParameterList, " + ex.Message);
            }


            return parmList;
        }
        
        public List<GroupSetupInfo> GetGroupSetup(int ID)
        {
            List<GroupSetupInfo> groupSetup = new List<GroupSetupInfo>();
            List<GroupSetupChildInfo> groupSetupChild = new List<GroupSetupChildInfo>();
            try
            {
                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                DataTable dt_GroupSetup = HANADAL.GetDataTableByStoredProcedure("GetGroup_Setup", TranslateIDToParameterList(ID), "GroupSetupManagement");

                if (dt_GroupSetup.Rows.Count > 0)
                {
                    groupSetup = TranslateDataTableToGroupSetupManagementList(dt_GroupSetup);
                    foreach (GroupSetupInfo item in groupSetup)
                    {
                        int Group_Setup_ID = Convert.ToInt32(item.ID);
                        DataTable dt_GroupSetupChild = HANADAL.GetDataTableByStoredProcedure("GetGroup_Setup_Child", TranslateGroupSetupIDToParameterList(Group_Setup_ID), "GroupSetupManagement");

                        if (dt_GroupSetupChild.Rows.Count>0)
                        {
                            groupSetupChild = TranslateDataTableToGroupSetupChildManagementList(dt_GroupSetupChild);
                        }
                        item.Table = groupSetupChild;
                    }
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("GroupSetupManagement", "Exception occured on GetMaster_Task ID: " + ID + " , " + ex.Message);
            }

            return groupSetup;
        }

        public List<GroupSetupInfo> GetGroupSetupHeader(int ID)
        {
            List<GroupSetupInfo> groupSetup = new List<GroupSetupInfo>();
            List<GroupSetupChildInfo> groupSetupChild = new List<GroupSetupChildInfo>();
            try
            {
                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                DataTable dt_GroupSetup = HANADAL.GetDataTableByStoredProcedure("GetGroup_Setup", TranslateIDToParameterList(ID), "GroupSetupManagement");

                if (dt_GroupSetup.Rows.Count > 0)
                {
                    groupSetup = TranslateDataTableToGroupSetupManagementList(dt_GroupSetup);
                   
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("GroupSetupManagement", "Exception occured on GetMaster_Task ID: " + ID + " , " + ex.Message);
            }

            return groupSetup;
        }
        public bool AddUpdateGroupSetup(out string msg,List<GroupSetupChildInfo> GroupSetupInfo , string DOCNUM, string GROUPCODE, string GROUPNAME, int UserID, int ID , bool ISACTIVE = true )
        {
            bool isSuccess = false;
            bool isAddOccured = false;
            bool isUpdateOccured = false;
            bool isDeleteOccured = false;
            msg = "";
            try
            {
                var filtererdList = GroupSetupInfo.Where(x => x.ISDELETED == false).ToList();
                var duplicates = filtererdList.GroupBy(x => x.UserID).Where(g => g.Count() > 1).Select(y => y.Key).ToList();
                if (duplicates.Count > 0)
                {
                    msg = "Duplicate record exist!";
                    return false;
                }


                //GroupSetupInfo = GroupSetupInfo.Where(x => x.ID > 0 || x.ISDELETED == false).ToList();

                ///////////For Form Log

                //AddTravelRates_Log(GroupSetupInfo, out isUpdateOccured);


                int no = 1;
                string docNum = "";
                if (DOCNUM == "")
                {
                    List<string> docNumList = GetTaskMasterDocNum();
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
                else
                {
                    docNum = DOCNUM;
                }
                List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
                B1SP_Parameter parm = new B1SP_Parameter();
                string groupCode = "";
                bool isValidateGroupCode = true;
                try
                {
                    HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                   
                    if (ID > 0)
                    {
                        DataSet ds = new DataSet();
                        parmList = new List<B1SP_Parameter>();
                        parm = new B1SP_Parameter();
                        parm.ParameterName = "ID";
                        parm.ParameterValue = Convert.ToString(ID);
                        parm.ParameterType = DBTypes.Int32.ToString();
                        parmList.Add(parm);
                        ds = HANADAL.GetDataSetByStoredProcedure("GetGroup_Setup", parmList, "GroupSetupManagement");
                        if (ds.Tables.Count > 0)
                        {
                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                foreach (DataRow dtRow in ds.Tables[0].Rows)
                                {
                                   
                                    groupCode = Convert.ToString(dtRow["GROUPCODE"]);
                                    if (groupCode.ToLower() == GROUPCODE.ToLower())
                                        isValidateGroupCode = false;
                                    break;
                                }
                            }
                        }
                    }
                    if(isValidateGroupCode)
                    {
                        parmList = new List<B1SP_Parameter>();
                        parm = new B1SP_Parameter();
                        parm.ParameterName = "GROUPCODE";
                        parm.ParameterValue = Convert.ToString(GROUPCODE);
                        parm.ParameterType = DBTypes.String.ToString();
                        parmList.Add(parm);

                        DataTable dt = HANADAL.GetDataTableByStoredProcedure("ValidateGroupCode", parmList, "GroupSetupManagement");
                        if (dt.Rows.Count > 0)
                        {
                            msg = "Group Code Already Exist!";
                            return false;
                        }
                    }
                    List<GroupSetupInfo> previousData =new List<Models.GroupSetupInfo>();
                    //For Log
                    if (ID>0)
                    {
                        previousData = GetGroupSetup(ID);
                        if (previousData.Count>0)
                        {
                            AddAlertSetup_Log(docNum, GROUPCODE, GROUPNAME, UserID, ID, ISACTIVE, previousData[0], ID);
                            if(previousData[0].Table.Count>0)
                            {
                                AddGropSetupChild_Log(GroupSetupInfo, previousData[0].Table, ID);
                            }
                        }
                        
                    }

                    DataTable dt_GroupSetup = HANADAL.AddUpdateDataByStoredProcedure("AddUpdateGroup_Setup", TranslateGroupSetupToParameterList(docNum, GROUPCODE, GROUPNAME, ISACTIVE, false,ID, UserID), "GroupSetupManagement");
                    if (dt_GroupSetup.Rows.Count == 0)
                        throw new Exception("Exception occured when AddUpdateGroupSetup,  GROUP CODE:" + GROUPCODE + " , GROUP NAME" + GROUPNAME);
                    else
                    {
                        ID = Convert.ToInt32(dt_GroupSetup.Rows[0]["ID"]);
                        if (ID>0)
                        {
                            foreach (var list in GroupSetupInfo)
                            {

                                try
                                {
                                    list.GROUPSETUP_ID = ID;
                                    HANADAL = new HANA_DAL_ODBC();
                                    DataTable dt_groupSetup = HANADAL.AddUpdateDataByStoredProcedure("AddUpdateGroup_Setup_Child", TranslateGroupSetupChildToParameterList(list), "GroupSetupManagement");
                                    if (dt_groupSetup.Rows.Count == 0)
                                        throw new Exception("Exception occured when Add/Update Group Setup, ID:" + list.ID + " , DESIGNATION NAME" + list.USER_NAME + " , USER CODE" + list.USER_CODE+ " , DEPARTMENT NAME:" + list.DEPARTMENTNAME + " , DESIGNATION NAME" + list.DESIGNATIONNAME );
                                }
                                catch (Exception ex)
                                {
                                    isSuccess = false;
                                    Log log = new Log();
                                    log.LogFile(ex.Message);
                                    log.InputOutputDocLog("GroupSetupManagement", "Exception occured in foreach loop AddUpdategroupSetup, " + ex.Message);
                                    continue;
                                }
                            }
                        }

                        if(previousData.Count>0)
                        {
                            //For Deleted Items
                            List<GroupSetupChildInfo> missingList = previousData[0].Table.Where(n => !GroupSetupInfo.Any(o => o.ID == n.ID && o.ISDELETED == n.ISDELETED)).ToList();

                            foreach (GroupSetupChildInfo previousObject in missingList)
                            {
                                GroupSetupChildInfo newObj = new GroupSetupChildInfo();
                                newObj = previousObject;
                                newObj.ISDELETED = true;
                                newObj.UPDATEDBY = UserID;
                                newObj.GROUPSETUP_ID = ID;
                                isDeleteOccured = true;

                                AddGropSetupChild_Log(newObj, previousObject, ID, true);
                                HANADAL.AddUpdateDataByStoredProcedure("AddUpdateGroup_Setup_Child", TranslateGroupSetupChildToParameterList(newObj), "GroupSetupManagement");
                            }
                        }
                       
                    }

                    msg = "Successfully Added/Updated";
                    isSuccess = true;
                }
                catch (Exception ex)
                {
                    msg = "Exception occured in foreach loop Add/Update Group Setup!";
                    isSuccess = false;
                    Log log = new Log();
                    log.LogFile(ex.Message);
                    log.InputOutputDocLog("GroupSetupManagement", "Exception occured in foreach loop AddUpdateGroupSetup, " + ex.Message);
                }

                //For Master Log
                //if (GroupSetupInfo.Where(x => x.ID == 0).ToList().Count > 0)
                  //  isAddOccured = true;
                //if (GroupSetupInfo.Where(x => x.ISDELETED == true).ToList().Count > 0)
                  //  isDeleteOccured = true;

                //int createdBy = 0;
                //var val = GroupSetupInfo.Where(x => x.CREATEDBY != null).FirstOrDefault();
                //if (val != null)
//                    createdBy = Convert.ToInt32(val.CREATEDBY);

                Common cmn = new Common();
                //Task.Run(() => cmn.AddMasterLog(isAddOccured, isUpdateOccured, isDeleteOccured, nameof(Enums.FormsName.GroupSetup), createdBy, "GroupSetupManagement"));
                Task.Run(() => cmn.AddMasterLog(isAddOccured, isUpdateOccured, isDeleteOccured, nameof(Enums.FormsName.GroupSetup), UserID, "GroupSetupManagement"));
                //End MAster Log

                
            }
            catch (Exception ex)
            {
                msg = "Exception occured in Add/Update Group Setup!";
                isSuccess = false;
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("GroupSetupManagement", "Exception occured on AddUpdateGroupSetup, " + ex.Message);
            }

            return isSuccess;
        }


        //public bool AddUpdateGroupSetup(List<GroupSetupInfo> GroupSetupInfo)
        //{
        //    bool isSuccess = false;
        //    bool isAddOccured = false;
        //    bool isUpdateOccured = false;
        //    bool isDeleteOccured = false;
        //    try
        //    {
        //        GroupSetupInfo = GroupSetupInfo.Where(x => x.ID > 0 || x.ISDELETED == false).ToList();

        //        ///////////For Form Log

        //        //AddTravelRates_Log(GroupSetupInfo, out isUpdateOccured);


        //        int no = 1;
        //        string docNum = "";
        //        List<string> docNumList = GetTaskMasterDocNum();
        //        if (docNumList.Count > 0)
        //        {
        //            var item = docNumList[docNumList.Count - 1];
        //            no = Convert.ToInt32(item.Split('-')[1]);
        //            no = no + 1;
        //            docNum = Convert.ToString(no).PadLeft(5, '0');
        //            docNum = "Doc-" + docNum;
        //        }
        //        else
        //        {
        //            docNum = Convert.ToString(no).PadLeft(5, '0');
        //            docNum = "Doc-" + docNum;
        //        }


        //        bool isnew = true;
        //        foreach (var list in GroupSetupInfo)
        //        {

        //            try
        //            {

        //                if (isnew == true && list.ID == 0)
        //                {
        //                    foreach (var item in GroupSetupInfo)
        //                    {
        //                        item.DOCNUM = docNum;
        //                    }
        //                    isnew = false;
        //                }


        //                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
        //                DataTable dt_GroupSetup = HANADAL.AddUpdateDataByStoredProcedure("AddUpdateGroupSetup", TranslateGroupSetupToParameterList(list), "GroupSetupManagement");
        //                if (dt_GroupSetup.Rows.Count == 0)
        //                    throw new Exception("Exception occured when AddUpdateGroupSetup, ID:" + list.ID + " , GROUP CODE:" + list.GROUPCODE + " , GROUP NAME" + list.GROUPNAME);
        //            }
        //            catch (Exception ex)
        //            {
        //                isSuccess = false;
        //                Log log = new Log();
        //                log.LogFile(ex.Message);
        //                log.InputOutputDocLog("GroupSetupManagement", "Exception occured in foreach loop AddUpdateGroupSetup, " + ex.Message);
        //                continue;
        //            }
        //        }

        //        //For Master Log
        //        if (GroupSetupInfo.Where(x => x.ID == 0).ToList().Count > 0)
        //            isAddOccured = true;
        //        if (GroupSetupInfo.Where(x => x.ISDELETED == true).ToList().Count > 0)
        //            isDeleteOccured = true;

        //        int createdBy = 0;
        //        var val = GroupSetupInfo.Where(x => x.CREATEDBY != null).FirstOrDefault();
        //        if (val != null)
        //            createdBy = Convert.ToInt32(val.CREATEDBY);

        //        Common cmn = new Common();
        //        Task.Run(() => cmn.AddMasterLog(isAddOccured, isUpdateOccured, isDeleteOccured, nameof(Enums.FormsName.GroupSetup), createdBy, "GroupSetupManagement"));
        //        //End MAster Log


        //        isSuccess = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        isSuccess = false;
        //        Log log = new Log();
        //        log.LogFile(ex.Message);
        //        log.InputOutputDocLog("GroupSetupManagement", "Exception occured on AddUpdateGroupSetup, " + ex.Message);
        //    }

        //    return isSuccess;
        //}

        public void AddTravelRates_Log(List<GroupSetupInfo> groupSetup, out bool isUpdateOccured)
        {
            isUpdateOccured = false;
            try
            {
                groupSetup = groupSetup.Where(x => x.ID > 0).ToList();

                foreach (var newObject in groupSetup)
                {

                    try
                    {
                        List<GroupSetupInfo> previousObject = GetGroupSetup(Convert.ToInt32(newObject.ID));
                        List<B1SP_Parameter> paramList = TranslateMaster_TaskLogToParameterList(newObject);
                        //List<B1SP_Parameter> paramList = new List<B1SP_Parameter>();
                        //B1SP_Parameter parm = new B1SP_Parameter();
                        bool isChangeOccured = false;
                        if (previousObject.Count > 0)
                        {
                            foreach (PropertyCompareResult resultItem in PropertyCompare.Compare(previousObject[0], newObject))
                            {
                                isChangeOccured = true;
                                isUpdateOccured = true;
                                string Name = resultItem.Name;
                                object PreviousValue = resultItem.OldValue;
                                object NewValue = resultItem.NewValue;

                                switch (Name)
                                {
                                    case "FROMKM":
                                        paramList.Where(x => x.ParameterName == "FROMKM_PREVIOUS").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                        paramList.Where(x => x.ParameterName == "FROMKM_NEW").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();

                                        break;
                                    case "TOKM":
                                        paramList.Where(x => x.ParameterName == "TOKM_PREVIOUS").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                        paramList.Where(x => x.ParameterName == "TOKM_NEW").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();

                                        break;
                                    case "RATETRIP":
                                        paramList.Where(x => x.ParameterName == "RATETRIP_PREVIOUS").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                        paramList.Where(x => x.ParameterName == "RATETRIP_NEW").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();

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
                                Task.Run(() => HANADAL.AddUpdateDataByStoredProcedure("AddTravel_Rates_LOG", paramList, "GroupSetupManagement"));
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        Log log = new Log();
                        log.LogFile(ex.Message);
                        log.InputOutputDocLog("GroupSetupManagement", "Exception occured in foreach loop AddTravelRates_Log, " + ex.Message);
                        continue;
                    }


                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("GroupSetupManagement", "Exception occured on AddTravelRates_Log, " + ex.Message);
            }
        }

        public DataTable GetGroupSetupLogByDocID(int docID, out DataTable dt_Header, out DataTable dt_Detail)
        {
            dt_Header = new DataTable();
            dt_Detail = new DataTable();
            DataTable dt_GroupSetupSetupLog = new DataTable();
            try
            {
                List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();

                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "Year";
                parm.ParameterValue =Convert.ToString(docID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                DataSet ds = HANADAL.GetDataSetByStoredProcedure("GetGroupSetupLogByDocID", parmList, "GroupSetupManagement");
                if(ds.Tables.Count>0)
                {
                    dt_Header = ds.Tables[0];
                    dt_Detail = ds.Tables[1];
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("GroupSetupManagement", "Exception occured on GetTravelRatesLog, " + ex.Message);
            }

            return dt_GroupSetupSetupLog;
        }

        public List<GroupSetupInfo> GetTaskMasterByFunctionID(int id)
        {
            List<GroupSetupInfo> TaskMasterList = new List<GroupSetupInfo>();
            try
            {
                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                Common cmn = new Common();
                //List<HCM_Designation> designationList = cmn.GetHCMDesignationList();
                DataTable dt_GroupSetupSetup = HANADAL.GetDataTableByStoredProcedure("GetMaster_TaskByFunctionID", cmn.TranslateIDToParameterList(id), "GroupSetupManagement");
                bool isNewDoc = false;
                if (dt_GroupSetupSetup.Rows.Count > 0)
                {
                    TaskMasterList = TranslateDataTableToGroupSetupManagementList(dt_GroupSetupSetup);
                }
                else
                    isNewDoc = true;


                foreach (var item in TaskMasterList)
                {
                    item.isNewDocument = isNewDoc;
                    item.KEY = Guid.NewGuid().ToString();

                }


                int sNo = TaskMasterList.Count() + 1;


                string docNum = "";
                var value = TaskMasterList.Where(x => x.DOCNUM != null).FirstOrDefault();
                if (value != null)
                {
                    docNum = value.DOCNUM;
                }
                else
                {


                }

                TaskMasterList.Select(c => { c.DOCNUM = docNum; return c; }).ToList();
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("GroupSetupManagement", "Exception occured on GetTaskMasterByFunctionID ID: " + id + " , " + ex.Message);
            }

            return TaskMasterList;
        }


        public List<string> GetTaskMasterDocNum()
        {
            List<string> docNumList = new List<string>();
            try
            {
                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();

                DataTable dt = HANADAL.GetDataTableByStoredProcedure("GetGroup_Setup_LastDocNum", "GroupSetupManagement");
                if (dt.Rows.Count > 0)
                {
                    docNumList = TranslateDataTableToMaster_TaskDocNumList(dt);
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("GroupSetupManagement", "Exception occured on GetTaskMasterDocNum, " + ex.Message);
            }

            return docNumList;
        }

        public List<GroupSetupInfo> GetTask_MasterByDocNum(string docNo)
        {
            List<GroupSetupInfo> Task_MasterList = new List<GroupSetupInfo>();
            try
            {

                List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();

                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "DOCNUM";
                parm.ParameterValue = docNo;
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                Common cmn = new Common();
                DataTable dt_Task_Master = HANADAL.GetDataTableByStoredProcedure("GetMaster_TaskByDocNum", parmList, "GroupSetupManagement");
                if (dt_Task_Master.Rows.Count > 0)
                {

                    Task_MasterList = TranslateDataTableToGroupSetupManagementList(dt_Task_Master);
                }

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("GroupSetupManagement", "Exception occured on GetResourceBillingRatesByDocNum DocNum: " + docNo + " , " + ex.Message);
            }

            return Task_MasterList;
        }

        private List<string> TranslateDataTableToMaster_TaskDocNumList(DataTable dt)
        {
            List<string> docNumList = new List<string>();
            foreach (DataRow dtRow in dt.Rows)
            {
                docNumList.Add(Convert.ToString(dtRow["DOCNUM"]));
            }

            return docNumList.Distinct().ToList();
        }


        public void AddAlertSetup_Log( string DOCNUM, string GROUPCODE, string GROUPNAME, int UserID, int ID, bool ISACTIVE, GroupSetupInfo previousObject, int docID)
        {
            GroupSetupInfo newObject = new GroupSetupInfo();
            try
            {
                newObject.DOCNUM = DOCNUM;
                newObject.GROUPNAME = GROUPNAME; 
                newObject.GROUPCODE = GROUPCODE;
                newObject.ISACTIVE = ISACTIVE;
                newObject.CREATEDBY = UserID;

                //if (newObject.ID > 0)
                {
                    List<B1SP_Parameter> paramList = TranslateGroupSetupLogToParameterList(newObject, docID);
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


                                case "GROUPNAME":
                                    isChangeOccured = true;
                                    paramList.Where(x => x.ParameterName == "GROUPNAME_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                    paramList.Where(x => x.ParameterName == "GROUPNAME_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();

                                    break;

                                case "ISACTIVE":
                                    isChangeOccured = true;
                                    paramList.Where(x => x.ParameterName == "ISACTIVE_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                    paramList.Where(x => x.ParameterName == "ISACTIVE_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();

                                    break;
                                    

                                case "ISDELETED":
                                    isChangeOccured = true;
                                    paramList.Where(x => x.ParameterName == "ISDELETED_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                    paramList.Where(x => x.ParameterName == "ISDELETED_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();
                                    break;
                            }

                        }

                        if (isChangeOccured)
                        {
                            HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                            Task.Run(() => HANADAL.AddUpdateDataByStoredProcedure("AddUpdateGroup_SetupLog", paramList, "GroupSetupManagement"));
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

        public void AddGropSetupChild_Log(List<GroupSetupChildInfo> newObjectList,List<GroupSetupChildInfo> previousObjectList, int docID)
        {
            try
            {
                GroupSetupChildInfo newObject = new GroupSetupChildInfo();
                GroupSetupChildInfo previousObject = new GroupSetupChildInfo();

                foreach (var obj in newObjectList)
                {
                    previousObject = previousObjectList.Where(x => x.UserID == obj.UserID).FirstOrDefault();
                    if (previousObject != null)
                    {
                        newObject = obj;


                        List<B1SP_Parameter> paramList = TranslateGroupSetupChildLogToParameterList(newObject, docID);
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


                                    case "UserID":
                                        isChangeOccured = true;
                                        paramList.Where(x => x.ParameterName == "UserID_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                        paramList.Where(x => x.ParameterName == "UserID_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();

                                        break;

                                    case "ISDELETED":
                                        isChangeOccured = true;
                                        paramList.Where(x => x.ParameterName == "ISDELETED_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                        paramList.Where(x => x.ParameterName == "ISDELETED_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();

                                        break;

                                }

                            }

                            if (isChangeOccured)
                            {
                                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                                Task.Run(() => HANADAL.AddUpdateDataByStoredProcedure("AddUpdateGroup_SetupChildLog", paramList, "GroupSetupManagement"));
                            }

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

        public void AddGropSetupChild_Log(GroupSetupChildInfo newObject, GroupSetupChildInfo previousObject, int docID,bool isDeleted=false)
        {
            try
            {
                newObject.ISDELETED = isDeleted;
                List<B1SP_Parameter> paramList = TranslateGroupSetupChildLogToParameterList(newObject, docID);
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


                            case "UserID":
                                isChangeOccured = true;
                                paramList.Where(x => x.ParameterName == "UserID_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                paramList.Where(x => x.ParameterName == "UserID_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();

                                break;

                            case "ISDELETED":
                                isChangeOccured = true;
                                paramList.Where(x => x.ParameterName == "ISDELETED_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                paramList.Where(x => x.ParameterName == "ISDELETED_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();

                                break;

                        }

                    }
                    if(isDeleted)
                    {
                        isChangeOccured = true;
                        paramList.Where(x => x.ParameterName == "ISDELETED_Previous").Select(c => { c.ParameterValue = Convert.ToString(false); return c; }).ToList();
                        paramList.Where(x => x.ParameterName == "ISDELETED_New").Select(c => { c.ParameterValue = Convert.ToString(true); return c; }).ToList();
                    }
                    if (isChangeOccured)
                    {
                        HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                        Task.Run(() => HANADAL.AddUpdateDataByStoredProcedure("AddUpdateGroup_SetupChildLog", paramList, "GroupSetupManagement"));
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
        #region "Translation groupSetup"


        private List<GroupSetupInfo> TranslateDataTableToGroupSetupManagementList(DataTable dt)
        {
            List<GroupSetupInfo> groupSetupL = new List<GroupSetupInfo>();

            try
            {
                int sno = 1;
                foreach (DataRow dtRow in dt.Rows)
                {
                    GroupSetupInfo groupSetup = new GroupSetupInfo();
                    //groupSetup.SNO = sno;
                    groupSetup.KEY = Guid.NewGuid().ToString();
                    groupSetup.ID = Convert.ToInt32(dtRow["ID"]);
                    groupSetup.GROUPCODE= Convert.ToString(dtRow["GROUPCODE"]);
                    groupSetup.GROUPNAME= Convert.ToString(dtRow["GROUPNAME"]);
                    groupSetup.DOCNUM = Convert.ToString(dtRow["DOCNUM"]);
                    groupSetup.CREATEDBY = Convert.ToInt32(dtRow["CREATEDBY"]);
                    groupSetup.CREATEDATE = Convert.ToDateTime(dtRow["CREATEDATE"]);

                    if (dtRow["UPDATEDBY"] != DBNull.Value)
                        groupSetup.UPDATEDBY = Convert.ToInt32(dtRow["UPDATEDBY"]);
                    if (dtRow["UPDATEDATE"] != DBNull.Value)
                        groupSetup.UPDATEDATE = Convert.ToDateTime(dtRow["UPDATEDATE"]);

                    groupSetup.ISACTIVE = Convert.ToBoolean(dtRow["ISACTIVE"]);
                    groupSetup.ISDELETED = Convert.ToBoolean(dtRow["ISDELETED"]);
                    sno = sno + 1;
                    groupSetupL.Add(groupSetup);
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("GroupSetupManagement", "Exception occured on TranslateDataTableToGroupSetupManagementList, " + ex.Message);
            }

            return groupSetupL;
        }

        private List<GroupSetupChildInfo> TranslateDataTableToGroupSetupChildManagementList(DataTable dt)
        {
            List<GroupSetupChildInfo> groupSetupL = new List<GroupSetupChildInfo>();

            try
            {
                int sno = 1;
                foreach (DataRow dtRow in dt.Rows)
                {
                    GroupSetupChildInfo groupSetup = new GroupSetupChildInfo();
                    groupSetup.SNO = sno;
                    groupSetup.KEY = Guid.NewGuid().ToString();
                    groupSetup.ID = Convert.ToInt32(dtRow["ID"]);

                    groupSetup.USER_CODE= Convert.ToString(dtRow["USER_CODE"]);
                    groupSetup.USER_NAME= Convert.ToString(dtRow["USER_NAME"]);
                    groupSetup.UserID = Convert.ToInt32(dtRow["UserID"]);
                    groupSetup.BRANCHID = Convert.ToInt32(dtRow["BRANCHID"]);
                    groupSetup.BRANCHNAME = Convert.ToString(dtRow["BRANCHNAME"]);
                    groupSetup.DESIGNATIONID= Convert.ToInt32(dtRow["DESIGNATIONID"]);
                    groupSetup.DESIGNATIONNAME= Convert.ToString(dtRow["DESIGNATIONNAME"]);
                    groupSetup.DEPARTMENTID= Convert.ToInt32(dtRow["DEPARTMENTID"]);
                    groupSetup.DEPARTMENTNAME = Convert.ToString(dtRow["DEPARTMENTNAME"]);
                    //groupSetup.DOCNUM = Convert.ToString(dtRow["DOCNUM"]);
                    groupSetup.CREATEDBY = Convert.ToInt32(dtRow["CREATEDBY"]);
                    groupSetup.CREATEDATE = Convert.ToDateTime(dtRow["CREATEDATE"]);

                    if (dtRow["UPDATEDBY"] != DBNull.Value)
                        groupSetup.UPDATEDBY = Convert.ToInt32(dtRow["UPDATEDBY"]);
                    if (dtRow["UPDATEDATE"] != DBNull.Value)
                        groupSetup.UPDATEDATE = Convert.ToDateTime(dtRow["UPDATEDATE"]);
                    
                    groupSetup.ISDELETED = Convert.ToBoolean(dtRow["ISDELETED"]);
                    sno = sno + 1;
                    groupSetupL.Add(groupSetup);
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("GroupSetupManagement", "Exception occured on TranslateDataTableToGroupSetupManagementList, " + ex.Message);
            }

            return groupSetupL;
        }

        private List<B1SP_Parameter> TranslateGroupSetupToParameterList(string DOCNUM, string GROUPCODE, string GROUPNAME, bool ISACTIVE,bool ISDELETED, int ID,int UserID)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {

                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = Convert.ToString(ID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ISACTIVE";
                parm.ParameterValue = Convert.ToString(ISACTIVE);
                parm.ParameterType = DBTypes.Bool.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ISDELETED";
                parm.ParameterValue = Convert.ToString(ISDELETED);
                parm.ParameterType = DBTypes.Bool.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CREATEDBY";
                parm.ParameterValue = Convert.ToString(UserID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CREATEDATE";
                parm.ParameterValue = DateTime.Now.ToString("yyyy-MM-dd HH:MM:s");
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "UPDATEDBY";
                parm.ParameterValue = Convert.ToString(UserID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "UPDATEDATE";
                parm.ParameterValue = DateTime.Now.ToString("yyyy-MM-dd HH:MM:s");
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "GROUPCODE";
                parm.ParameterValue = Convert.ToString(GROUPCODE);
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "GROUPNAME";
                parm.ParameterValue = Convert.ToString(GROUPNAME);
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "DOCNUM";
                parm.ParameterValue = Convert.ToString(DOCNUM);
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("GroupSetupManagement", "Exception occured on TranslateGroupSetupToParameterList, " + ex.Message);
            }


            return parmList;
        }

        private List<B1SP_Parameter> TranslateMaster_TaskLogToParameterList(GroupSetupInfo groupSetup)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {

                B1SP_Parameter parm = new B1SP_Parameter();

                //parm = new B1SP_Parameter();
                //parm.ParameterName = "FROMKM_PREVIOUS";
                //parm.ParameterValue = Convert.ToString(groupSetup.FROMKM);
                //parmList.Add(parm);

                //parm = new B1SP_Parameter();
                //parm.ParameterName = "FROMKM_NEW";
                //parm.ParameterValue = Convert.ToString(groupSetup.FROMKM);
                //parmList.Add(parm);

                //parm = new B1SP_Parameter();
                //parm.ParameterName = "TOKM_NEW";
                //parm.ParameterValue = Convert.ToString(groupSetup.TOKM);
                //parmList.Add(parm);

                //parm = new B1SP_Parameter();
                //parm.ParameterName = "TOKM_PREVIOUS";
                //parm.ParameterValue = Convert.ToString(groupSetup.TOKM);
                //parmList.Add(parm);

                //parm = new B1SP_Parameter();
                //parm.ParameterName = "RATETRIP_PREVIOUS";
                //parm.ParameterValue = Convert.ToString(groupSetup.RATETRIP);
                //parmList.Add(parm);

                //parm = new B1SP_Parameter();
                //parm.ParameterName = "RATETRIP_NEW";
                //parm.ParameterValue = Convert.ToString(groupSetup.RATETRIP);
                //parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ISACTIVE_PREVIOUS";
                parm.ParameterValue = Convert.ToString(groupSetup.ISACTIVE);
                parm.ParameterType = DBTypes.Bool.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ISACTIVE_NEW";
                parm.ParameterValue = Convert.ToString(groupSetup.ISACTIVE);
                parm.ParameterType = DBTypes.Bool.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ISDELETED_PREVIOUS";
                parm.ParameterValue = Convert.ToString(groupSetup.ISDELETED);
                parm.ParameterType = DBTypes.Bool.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ISDELETED_NEW";
                parm.ParameterValue = Convert.ToString(groupSetup.ISDELETED);
                parm.ParameterType = DBTypes.Bool.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CREATEDBY";
                parm.ParameterValue = Convert.ToString(groupSetup.CREATEDBY);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CREATEDATE";
                parm.ParameterValue = DateTime.Now.ToString("yyyy-MM-dd HH:MM:s");
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("GroupSetupManagement", "Exception occured on TranslateMaster_TaskLogToParameterList, " + ex.Message);
            }


            return parmList;
        }

        private List<B1SP_Parameter> TranslateGroupSetupChildToParameterList(GroupSetupChildInfo groupSetupChild)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {

                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = Convert.ToString(groupSetupChild.ID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);


                parm = new B1SP_Parameter();
                parm.ParameterName = "GROUPSETUP_ID";
                parm.ParameterValue = Convert.ToString(groupSetupChild.GROUPSETUP_ID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);


                parm = new B1SP_Parameter();
                parm.ParameterName = "UserID";
                parm.ParameterValue = Convert.ToString(groupSetupChild.UserID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ISACTIVE";
                parm.ParameterValue = Convert.ToString(groupSetupChild.ISACTIVE);
                parm.ParameterType = DBTypes.Bool.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ISDELETED";
                parm.ParameterValue = Convert.ToString(groupSetupChild.ISDELETED);
                parm.ParameterType = DBTypes.Bool.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CREATEDBY";
                parm.ParameterValue = Convert.ToString(groupSetupChild.CREATEDBY);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CREATEDATE";
                parm.ParameterValue = DateTime.Now.ToString("yyyy-MM-dd HH:MM:s");
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "UPDATEDBY";
                parm.ParameterValue = Convert.ToString(groupSetupChild.UPDATEDBY);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "UPDATEDATE";
                parm.ParameterValue = DateTime.Now.ToString("yyyy-MM-dd HH:MM:s");
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "BRANCHID";
                parm.ParameterValue = Convert.ToString(groupSetupChild.BRANCHID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "BRANCHNAME";
                parm.ParameterValue = Convert.ToString(groupSetupChild.BRANCHNAME);
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);


                parm = new B1SP_Parameter();
                parm.ParameterName = "USER_CODE";
                parm.ParameterValue = Convert.ToString(groupSetupChild.USER_CODE);
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "USER_NAME";
                parm.ParameterValue = Convert.ToString(groupSetupChild.USER_NAME);
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);


                parm = new B1SP_Parameter();
                parm.ParameterName = "DESIGNATIONID";
                parm.ParameterValue = Convert.ToString(groupSetupChild.DESIGNATIONID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "DESIGNATIONNAME";
                parm.ParameterValue = Convert.ToString(groupSetupChild.DESIGNATIONNAME);
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);


                parm = new B1SP_Parameter();
                parm.ParameterName = "DEPARTMENTID";
                parm.ParameterValue = Convert.ToString(groupSetupChild.DEPARTMENTID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);


                parm = new B1SP_Parameter();
                parm.ParameterName = "DEPARTMENTNAME";
                parm.ParameterValue = Convert.ToString(groupSetupChild.DEPARTMENTNAME);
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);
        
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("GroupSetupManagement", "Exception occured on TranslateGroupSetupChildToParameterList, " + ex.Message);
            }


            return parmList;
        }

        private List<B1SP_Parameter> TranslateGroupSetupLogToParameterList(GroupSetupInfo obj, int docID)
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
                parm.ParameterName = "DOCNUM";
                parm.ParameterValue = Convert.ToString(obj.DOCNUM);
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "DOCID";
                parm.ParameterValue = Convert.ToString(docID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "GROUPCODE";
                parm.ParameterValue = Convert.ToString(obj.GROUPCODE);
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);


                parm = new B1SP_Parameter();
                parm.ParameterName = "GROUPNAME_Previous";
                parm.ParameterValue = Convert.ToString(obj.GROUPNAME);
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                
                parm = new B1SP_Parameter();
                parm.ParameterName = "ISACTIVE_Previous";
                parm.ParameterValue = Convert.ToString(obj.ISACTIVE);
                parm.ParameterType = DBTypes.Bool.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ISDELETED_Previous";
                parm.ParameterValue = Convert.ToString(obj.ISDELETED);
                parm.ParameterType = DBTypes.Bool.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "GROUPNAME_New";
                parm.ParameterValue = Convert.ToString(obj.GROUPNAME);
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);


                parm = new B1SP_Parameter();
                parm.ParameterName = "ISACTIVE_New";
                parm.ParameterValue = Convert.ToString(obj.ISACTIVE);
                parm.ParameterType = DBTypes.Bool.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ISDELETED_New";
                parm.ParameterValue = Convert.ToString(obj.ISDELETED);
                parm.ParameterType = DBTypes.Bool.ToString();
                parmList.Add(parm);


                parm = new B1SP_Parameter();
                parm.ParameterName = "CREATEDBY";
                parm.ParameterValue = Convert.ToString(obj.CREATEDBY);
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
                log.InputOutputDocLog("GroupSetupManagement", "Exception occured on TranslateAlertSetupLogToParameterList, " + ex.Message);
            }


            return parmList;
        }

        private List<B1SP_Parameter> TranslateGroupSetupChildLogToParameterList(GroupSetupChildInfo obj, int docID)
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
                parm.ParameterName = "GROUPSETUP_ID";
                parm.ParameterValue = Convert.ToString(docID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "DOCID";
                parm.ParameterValue = Convert.ToString(docID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "UserID_Previous";
                parm.ParameterValue = Convert.ToString(obj.UserID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ISDELETED_Previous";
                parm.ParameterValue = Convert.ToString(obj.ISDELETED);
                parm.ParameterType = DBTypes.Bool.ToString();
                parmList.Add(parm);


                parm = new B1SP_Parameter();
                parm.ParameterName = "UserID_New";
                parm.ParameterValue = Convert.ToString(obj.UserID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ISDELETED_New";
                parm.ParameterValue = Convert.ToString(obj.ISDELETED);
                parm.ParameterType = DBTypes.Bool.ToString();
                parmList.Add(parm);
                
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("GroupSetupManagement", "Exception occured on TranslateAlertSetupLogToParameterList, " + ex.Message);
            }


            return parmList;
        }
        #endregion

    }
}