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
    public class StageSetupManagement
    {       
        public List<StageSetupInfo> GetStageSetup(int ID)
        {
            List<StageSetupInfo> stageSetup = new List<StageSetupInfo>();
            List<StageSetupChildInfo> stageSetupChild = new List<StageSetupChildInfo>();
            try
            {
                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                DataTable dt_StageSetup = HANADAL.GetDataTableByStoredProcedure("GetStage_Setup", TranslateIDToParameterList(ID), "StageSetupManagement");

                if (dt_StageSetup.Rows.Count > 0)
                {
                    stageSetup = TranslateDataTableToStageSetupManagementList(dt_StageSetup);
                    foreach (StageSetupInfo item in stageSetup)
                    {
                        int Stage_Setup_ID = Convert.ToInt32(item.ID);
                        DataTable dt_StageSetupChild = HANADAL.GetDataTableByStoredProcedure("GetStage_Setup_Child", TranslateStageSetupIDToParameterList(Stage_Setup_ID), "StageSetupManagement");

                        if (dt_StageSetupChild.Rows.Count > 0)
                        {
                            stageSetupChild = TranslateDataTableToStageSetupChildManagementList(dt_StageSetupChild);
                        }
                        item.Table = stageSetupChild;
                    }
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("StageSetupManagement", "Exception occured on GetMaster_Task ID: " + ID + " , " + ex.Message);
            }

            return stageSetup;
        }

        public StageSetupInfo GetStageSetupByID(int id)
        {
            StageSetupInfo stageSetup = new StageSetupInfo();
            try
            {
                List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = Convert.ToString(id);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                Common cmn = new Common();
                DataSet ds = HANADAL.GetDataSetByStoredProcedure("GetStageSetupByID", parmList, "StageSetupManagement");
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        stageSetup = TranslateDataTableToApprovalStageManagementList(ds.Tables[0]);
                        stageSetup.Table = TranslateDataTableToStageSetupChildManagementList(ds.Tables[1]);
                    }
                }


            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("StageSetupManagement", "Exception occured on GetStageSetupByID ID: " + id + " , " + ex.Message);
            }

            return stageSetup;
        }

        public bool AddUpdateStageSetup(out string msg, List<StageSetupChildInfo> StageSetupInfo, string DOCNUM, string STAGECODE, string STAGEDESCRIPTION,
            int APPROVALREQUIRED, int REJECTIONREQUIRED, int UserID, int ID, bool INACTIVE = false)
        {
            bool isSuccess = false;
            bool isAddOccured = false;
            bool isUpdateOccured = false;
            bool isDeleteOccured = false;
            msg = "";
            try
            {
                
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
                string stageCode = "";
                bool isValidateStageCode = true;
                try
                {
                    HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();

                    if (ID > 0)
                    {
                        parmList = new List<B1SP_Parameter>();
                        parm = new B1SP_Parameter();
                        parm.ParameterName = "ID";
                        parm.ParameterValue = Convert.ToString(ID);
                        parm.ParameterType = DBTypes.Int32.ToString();
                        parmList.Add(parm);
                        DataSet ds = HANADAL.GetDataSetByStoredProcedure("GetStage_Setup", parmList, "StageSetupManagement");
                        if (ds.Tables.Count > 0)
                        {
                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                foreach (DataRow dtRow in ds.Tables[0].Rows)
                                {
                                    stageCode = Convert.ToString(dtRow["STAGECODE"]);
                                    if (stageCode.ToLower() == STAGECODE.ToLower())
                                        isValidateStageCode = false;
                                    break;
                                }
                            }
                        }
                    }
                    if (isValidateStageCode)
                    {
                        parmList = new List<B1SP_Parameter>();
                        parm = new B1SP_Parameter();
                        parm.ParameterName = "STAGECODE";
                        parm.ParameterValue = Convert.ToString(STAGECODE);
                        parm.ParameterType = DBTypes.String.ToString();
                        parmList.Add(parm);

                        DataTable dt = HANADAL.GetDataTableByStoredProcedure("ValidateStageCode", parmList, "StageSetupManagement");
                        if (dt.Rows.Count > 0)
                        {
                            msg = "Stage Code Already Exist!";
                            return false;
                        }
                    }

                    
                    List<StageSetupInfo> SSI = new List<StageSetupInfo>();
                    StageSetupInfo SS = new StageSetupInfo();
                    SS.ID = ID;
                    SS.DOCNUM = DOCNUM;
                    SS.STAGECODE = STAGECODE;
                    SS.STAGEDESCRIPTION = STAGEDESCRIPTION;
                    SS.APPROVALREQUIRED = APPROVALREQUIRED;
                    SS.REJECTIONREQUIRED = REJECTIONREQUIRED;
                    SS.ISDELETED = INACTIVE;
                    SS.CREATEDBY = UserID; 
                    SSI.Add(SS);
                    
                    List<StageSetupInfo> previousData = new List<Models.StageSetupInfo>();
                    //For Log
                    if (ID > 0)
                    {
                        previousData = GetStageSetup(ID);
                        if (previousData.Count > 0)
                        {
                            AddStageSetup_Log(SS, out isUpdateOccured);
                            if (previousData[0].Table.Count > 0)
                            {
                                AddStageSetupChild_Log(StageSetupInfo, previousData[0].Table, ID);
                            }
                        }

                    }

                    DataTable dt_StageSetup = HANADAL.AddUpdateDataByStoredProcedure("AddUpdateStage_Setup", TranslateStageSetupToParameterList(docNum, STAGECODE, STAGEDESCRIPTION, APPROVALREQUIRED, REJECTIONREQUIRED, INACTIVE, ID, UserID), "StageSetupManagement");
                    if (dt_StageSetup.Rows.Count == 0)
                        throw new Exception("Exception occured when AddUpdateStageSetup,  STAGE CODE:" + STAGECODE + " , STAGE DESCRIPTION" + STAGEDESCRIPTION);
                    else
                    {
                        ID = Convert.ToInt32(dt_StageSetup.Rows[0]["ID"]);
                        if (ID > 0)
                        {
                            foreach (var list in StageSetupInfo)
                            {

                                try
                                {
                                    list.STAGESETUP_ID = ID;
                                    HANADAL = new HANA_DAL_ODBC();
                                    DataTable dt_stageSetup = HANADAL.AddUpdateDataByStoredProcedure("AddUpdateStage_Setup_Child", TranslateStageSetupChildToParameterList(list), "StageSetupManagement");
                                    if (dt_stageSetup.Rows.Count == 0)
                                        throw new Exception("Exception occured when Add/Update Travel Location, ID:" + list.ID + " ,  NAME" + list.FULLNAME + " , USER CODE" + list.USER_CODE + " , DEPARTMENT NAME:" + list.DEPARTMENTNAME + " , DESIGNATION NAME" + list.DESIGNATIONNAME);
                                }
                                catch (Exception ex)
                                {
                                    isSuccess = false;
                                    Log log = new Log();
                                    log.LogFile(ex.Message);
                                    log.InputOutputDocLog("StageSetupManagement", "Exception occured in foreach loop AddUpdatestageSetup, " + ex.Message);
                                    continue;
                                }
                            }

                        }
                        if (previousData.Count > 0)
                        {
                            //For Deleted Items
                            List<StageSetupChildInfo> missingList = previousData[0].Table.Where(n => !StageSetupInfo.Any(o => o.ID == n.ID && o.ISDELETED == n.ISDELETED)).ToList();

                            foreach (StageSetupChildInfo previousObject in missingList)
                            {
                                StageSetupChildInfo newObj = new StageSetupChildInfo();
                                newObj = previousObject;
                                newObj.ISDELETED = true;
                                newObj.UPDATEDBY = UserID;
                                newObj.STAGESETUP_ID = ID;
                                isDeleteOccured = true;

                                HANADAL.AddUpdateDataByStoredProcedure("AddUpdateStage_Setup_Child", TranslateStageSetupChildToParameterList(newObj), "StageSetupManagement");
                            }
                        }
                    }

                    msg = "Successfully Added/Updated";
                    isSuccess = true;
                }
                catch (Exception ex)
                {
                    msg = "Exception occured in foreach loop Add/Update Stage Setup!";
                    isSuccess = false;
                    Log log = new Log();
                    log.LogFile(ex.Message);
                    log.InputOutputDocLog("StageSetupManagement", "Exception occured in foreach loop AddUpdateStageSetup, " + ex.Message);
                }
                
                Common cmn = new Common();
                Task.Run(() => cmn.AddMasterLog(isAddOccured, isUpdateOccured, isDeleteOccured, nameof(Enums.FormsName.ApprovalStages), UserID, "StageSetupManagement"));
                
            }
            catch (Exception ex)
            {
                msg = "Exception occured in foreach loop Add/Update Stage Setup!";
                isSuccess = false;
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("StageSetupManagement", "Exception occured on AddUpdateStageSetup, " + ex.Message);
            }

            return isSuccess;
        }


        public void AddStageSetup_Log(StageSetupInfo stageSetup, out bool isUpdateOccured)
        {
            isUpdateOccured = false;
            try
            {
                if (stageSetup.ID == 0)
                    return;

                StageSetupInfo previousObject = GetStageSetupByID(Convert.ToInt32(stageSetup.ID));
                List<B1SP_Parameter> paramList = TranslateStageSetupLogToParameterList(stageSetup);
                bool isChangeOccured = false;
                if (previousObject != null)
                {

                    foreach (PropertyCompareResult resultItem in PropertyCompare.Compare(previousObject, stageSetup))
                    {
                        isChangeOccured = true;
                        isUpdateOccured = true;
                        string Name = resultItem.Name;
                        object PreviousValue = resultItem.OldValue;
                        object NewValue = resultItem.NewValue;
                        switch (Name)
                        {
                            case "STAGEDESCRIPTION":
                                paramList.Where(x => x.ParameterName == "STAGEDESCRIPTION_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                paramList.Where(x => x.ParameterName == "STAGEDESCRIPTION_NEW").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();

                                break;
                            case "APPROVALREQUIRED":
                                paramList.Where(x => x.ParameterName == "APPROVALREQUIRED_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                paramList.Where(x => x.ParameterName == "APPROVALREQUIRED_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();

                                break;
                            case "REJECTIONREQUIRED":
                                paramList.Where(x => x.ParameterName == "REJECTIONREQUIRED_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                paramList.Where(x => x.ParameterName == "REJECTIONREQUIRED_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();

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
                        HANADAL.AddUpdateDataByStoredProcedure("AddUpdateStage_SetupLog", paramList, "StageSetupManagement");
                    }
                }    
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("StageSetupManagement", "Exception occured on AddStageSetup_Log, " + ex.Message);
            }
        }

        public void AddStageSetupChild_Log(List<StageSetupChildInfo> newObjectList, List<StageSetupChildInfo> previousObjectList, int docID)
        {
            try
            {
                StageSetupChildInfo newObject = new StageSetupChildInfo();
                StageSetupChildInfo previousObject = new StageSetupChildInfo();

                foreach (var obj in newObjectList)
                {
                    previousObject = previousObjectList.Where(x => x.ID == obj.ID).FirstOrDefault();
                    if (previousObject != null)
                    {
                        newObject = obj;
                        newObject.STAGESETUP_ID = docID;
                        List<B1SP_Parameter> paramList = TranslateStageSetupChildLogToParameterList(newObject);
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
                                    case "USER_CODE":
                                        paramList.Where(x => x.ParameterName == "UserID_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                        paramList.Where(x => x.ParameterName == "UserID_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();

                                        break;
                                    case "FULLNAME":
                                        paramList.Where(x => x.ParameterName == "FullName_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                        paramList.Where(x => x.ParameterName == "FullName_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();

                                        break;
                                    case "ISDELETED":
                                        paramList.Where(x => x.ParameterName == "ISDELETED_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                        paramList.Where(x => x.ParameterName == "ISDELETED_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();

                                        break;
                                }
                            }

                            if (isChangeOccured)
                            {
                                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                                HANADAL.AddUpdateDataByStoredProcedure("AddUpdateStage_SetupChildLog", paramList, "StageSetupManagement");
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("StageSetupManagement", "Exception occured on AddStageSetupChild_Log, " + ex.Message);
            }
        }

        public List<string> GetTaskMasterDocNum()
        {
            List<string> docNumList = new List<string>();
            try
            {
                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();

                DataTable dt = HANADAL.GetDataTableByStoredProcedure("GetStage_Setup_LastDocNum", "StageSetupManagement");
                if (dt.Rows.Count > 0)
                {
                    docNumList = TranslateDataTableToMaster_TaskDocNumList(dt);
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("StageSetupManagement", "Exception occured on GetTaskMasterDocNum, " + ex.Message);
            }

            return docNumList;
        }

        public List<StageSetupInfo> GetFunctionsFromStage()
        {
            List<StageSetupInfo> SSIL = new List<StageSetupInfo>();
            try
            {
                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();

                DataTable dt = HANADAL.GetDataTableByStoredProcedure("GETFunctionFromStageSetup", "StageSetupManagement");
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow item in dt.Rows)
                    {
                        StageSetupInfo SSI = new StageSetupInfo();

                        SSI.STAGECODE = Convert.ToString(item["StageCode"]);
                        SSI.STAGEDESCRIPTION = Convert.ToString(item["StageDescription"]);

                        SSIL.Add(SSI);
                    }
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("StageSetupManagement", "Exception occured on GetTaskMasterDocNum, " + ex.Message);
            }
            return SSIL;
        }

        
        public List<Documents> Documentss = new List<Documents>() {
            new Documents {
                Document = "Timesheet",
            },
            new Documents {
                Document = "Travel Claim",
            },
            new Documents {
                Document = "Assignment",
            },
            new Documents {
                Document = "Claim",
            },
            new Documents {
                Document = "Change Approver",
            }
        };
            

        public List<StageSetupInfo> GetTask_MasterByDocNum(string docNo)
        {
            List<StageSetupInfo> Task_MasterList = new List<StageSetupInfo>();
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
                DataTable dt_Task_Master = HANADAL.GetDataTableByStoredProcedure("GetMaster_TaskByDocNum", parmList, "StageSetupManagement");
                if (dt_Task_Master.Rows.Count > 0)
                {

                    Task_MasterList = TranslateDataTableToStageSetupManagementList(dt_Task_Master);
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("StageSetupManagement", "Exception occured on GetResourceBillingRatesByDocNum DocNum: " + docNo + " , " + ex.Message);
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


        #region "Translation stageSetup"

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
                log.InputOutputDocLog("StageSetupManagement", "Exception occured on TranslateStageSetupToParameterList, " + ex.Message);
            }
            return parmList;
        }

        private List<B1SP_Parameter> TranslateStageSetupIDToParameterList(int STAGESETUP_ID)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {

                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "STAGESETUP_ID";
                parm.ParameterValue = Convert.ToString(STAGESETUP_ID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);


            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("StageSetupManagement", "Exception occured on TranslateStageSetupToParameterList, " + ex.Message);
            }


            return parmList;
        }
        
        private List<StageSetupInfo> TranslateDataTableToStageSetupManagementList(DataTable dt)
        {
            List<StageSetupInfo> stageSetupL = new List<StageSetupInfo>();

            try
            {
                int sno = 1;
                foreach (DataRow dtRow in dt.Rows)
                {
                    StageSetupInfo stageSetup = new StageSetupInfo();
                    //stageSetup.SNO = sno;
                    stageSetup.KEY = Guid.NewGuid().ToString();
                    stageSetup.ID = Convert.ToInt32(dtRow["ID"]);
                    stageSetup.STAGECODE = Convert.ToString(dtRow["STAGECODE"]);
                    stageSetup.STAGEDESCRIPTION = Convert.ToString(dtRow["STAGEDESCRIPTION"]);
                    stageSetup.APPROVALREQUIRED = Convert.ToInt32(dtRow["APPROVALREQUIRED"]);
                    stageSetup.REJECTIONREQUIRED = Convert.ToInt32(dtRow["REJECTIONREQUIRED"]);
                    stageSetup.DOCNUM = Convert.ToString(dtRow["DOCNUM"]);
                    stageSetup.CREATEDBY = Convert.ToInt32(dtRow["CREATEDBY"]);
                    stageSetup.CREATEDATE = Convert.ToDateTime(dtRow["CREATEDATE"]);

                    if (dtRow["UPDATEDBY"] != DBNull.Value)
                        stageSetup.UPDATEDBY = Convert.ToInt32(dtRow["UPDATEDBY"]);
                    if (dtRow["UPDATEDATE"] != DBNull.Value)
                        stageSetup.UPDATEDATE = Convert.ToDateTime(dtRow["UPDATEDATE"]);

                    //stageSetup.ISACTIVE = Convert.ToBoolean(dtRow["ISACTIVE"]);
                    stageSetup.ISDELETED = Convert.ToBoolean(dtRow["ISDELETED"]);
                    sno = sno + 1;
                    stageSetupL.Add(stageSetup);
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("StageSetupManagement", "Exception occured on TranslateDataTableToStageSetupManagementList, " + ex.Message);
            }

            return stageSetupL;
        }

        private StageSetupInfo TranslateDataTableToApprovalStageManagementList(DataTable dt)
        {
            StageSetupInfo stageSetup = new StageSetupInfo();
            try
            {
                int sno = 1;
                foreach (DataRow dtRow in dt.Rows)
                {
                    //stageSetup.SNO = sno;
                    stageSetup.KEY = Guid.NewGuid().ToString();
                    stageSetup.ID = Convert.ToInt32(dtRow["ID"]);
                    stageSetup.STAGECODE = Convert.ToString(dtRow["STAGECODE"]);
                    stageSetup.STAGEDESCRIPTION = Convert.ToString(dtRow["STAGEDESCRIPTION"]);
                    stageSetup.APPROVALREQUIRED = Convert.ToInt32(dtRow["APPROVALREQUIRED"]);
                    stageSetup.REJECTIONREQUIRED = Convert.ToInt32(dtRow["REJECTIONREQUIRED"]);
                    stageSetup.DOCNUM = Convert.ToString(dtRow["DOCNUM"]);
                    stageSetup.CREATEDBY = Convert.ToInt32(dtRow["CREATEDBY"]);
                    stageSetup.CREATEDATE = Convert.ToDateTime(dtRow["CREATEDATE"]);

                    if (dtRow["UPDATEDBY"] != DBNull.Value)
                        stageSetup.UPDATEDBY = Convert.ToInt32(dtRow["UPDATEDBY"]);
                    if (dtRow["UPDATEDATE"] != DBNull.Value)
                        stageSetup.UPDATEDATE = Convert.ToDateTime(dtRow["UPDATEDATE"]);

                    //stageSetup.ISACTIVE = Convert.ToBoolean(dtRow["ISACTIVE"]);
                    stageSetup.ISDELETED = Convert.ToBoolean(dtRow["ISDELETED"]);
                    sno = sno + 1;
                    
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("StageSetupManagement", "Exception occured on TranslateDataTableToStageSetupManagementList, " + ex.Message);
            }
            return stageSetup;
        }

        private List<StageSetupChildInfo> TranslateDataTableToStageSetupChildManagementList(DataTable dt)
        {
            List<StageSetupChildInfo> stageSetupL = new List<StageSetupChildInfo>();

            try
            {
                int sno = 1;
                foreach (DataRow dtRow in dt.Rows)
                {
                    StageSetupChildInfo stageSetup = new StageSetupChildInfo();
                    stageSetup.SNO = sno;
                    stageSetup.KEY = Guid.NewGuid().ToString();
                    stageSetup.ID = Convert.ToInt32(dtRow["ID"]);

                    stageSetup.USER_CODE = Convert.ToString(dtRow["USER_CODE"]);
                    stageSetup.FULLNAME = Convert.ToString(dtRow["USER_NAME"]);
                    stageSetup.UserID = Convert.ToInt32(dtRow["UserID"]);
                    stageSetup.DESIGNATIONID = Convert.ToInt32(dtRow["DESIGNATIONID"]);
                    stageSetup.DESIGNATIONNAME = Convert.ToString(dtRow["DESIGNATIONNAME"]);
                    stageSetup.DEPARTMENTID = Convert.ToInt32(dtRow["DEPARTMENTID"]);
                    stageSetup.DEPARTMENTNAME = Convert.ToString(dtRow["DEPARTMENTNAME"]);
                    stageSetup.CREATEDBY = Convert.ToInt32(dtRow["CREATEDBY"]);
                    stageSetup.CREATEDATE = Convert.ToDateTime(dtRow["CREATEDATE"]);

                    if (dtRow["UPDATEDBY"] != DBNull.Value)
                        stageSetup.UPDATEDBY = Convert.ToInt32(dtRow["UPDATEDBY"]);
                    if (dtRow["UPDATEDATE"] != DBNull.Value)
                        stageSetup.UPDATEDATE = Convert.ToDateTime(dtRow["UPDATEDATE"]);

                    stageSetup.ISDELETED = Convert.ToBoolean(dtRow["ISDELETED"]);
                    sno = sno + 1;
                    stageSetupL.Add(stageSetup);
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("StageSetupManagement", "Exception occured on TranslateDataTableToStageSetupManagementList, " + ex.Message);
            }

            return stageSetupL;
        }

        private List<B1SP_Parameter> TranslateStageSetupToParameterList(string DOCNUM, string STAGECODE, string STAGEDESCRIPTION,
            int APPROVALREQUIRED, int REJECTIONREQUIRED, bool ISDELETED, int ID, int UserID)
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
                parm.ParameterName = "STAGECODE";
                parm.ParameterValue = Convert.ToString(STAGECODE);
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "STAGEDESCRIPTION";
                parm.ParameterValue = Convert.ToString(STAGEDESCRIPTION);
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "APPROVALREQUIRED";
                parm.ParameterValue = Convert.ToString(APPROVALREQUIRED);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "REJECTIONREQUIRED";
                parm.ParameterValue = Convert.ToString(REJECTIONREQUIRED);
                parm.ParameterType = DBTypes.Int32.ToString();
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
                log.InputOutputDocLog("StageSetupManagement", "Exception occured on TranslateStageSetupToParameterList, " + ex.Message);
            }

            return parmList;
        }

        
        
        private List<B1SP_Parameter> TranslateStageSetupChildToParameterList(StageSetupChildInfo stageSetupChild)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {

                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = Convert.ToString(stageSetupChild.ID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);


                parm = new B1SP_Parameter();
                parm.ParameterName = "STAGESETUP_ID";
                parm.ParameterValue = Convert.ToString(stageSetupChild.STAGESETUP_ID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);


                parm = new B1SP_Parameter();
                parm.ParameterName = "UserID";
                parm.ParameterValue = Convert.ToString(stageSetupChild.UserID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ISACTIVE";
                parm.ParameterValue = Convert.ToString(stageSetupChild.ISACTIVE);
                parm.ParameterType = DBTypes.Bool.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ISDELETED";
                parm.ParameterValue = Convert.ToString(stageSetupChild.ISDELETED);
                parm.ParameterType = DBTypes.Bool.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CREATEDBY";
                parm.ParameterValue = Convert.ToString(stageSetupChild.CREATEDBY);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CREATEDATE";
                parm.ParameterValue = DateTime.Now.ToString("yyyy-MM-dd HH:MM:s");
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "UPDATEDBY";
                parm.ParameterValue = Convert.ToString(stageSetupChild.UPDATEDBY);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "UPDATEDATE";
                parm.ParameterValue = DateTime.Now.ToString("yyyy-MM-dd HH:MM:s");
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "USER_CODE";
                parm.ParameterValue = Convert.ToString(stageSetupChild.USER_CODE);
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "USER_NAME";
                parm.ParameterValue = Convert.ToString(stageSetupChild.FULLNAME);
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "DESIGNATIONID";
                parm.ParameterValue = Convert.ToString(stageSetupChild.DESIGNATIONID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "DESIGNATIONNAME";
                parm.ParameterValue = Convert.ToString(stageSetupChild.DESIGNATIONNAME);
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "DEPARTMENTID";
                parm.ParameterValue = Convert.ToString(stageSetupChild.DEPARTMENTID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "DEPARTMENTNAME";
                parm.ParameterValue = Convert.ToString(stageSetupChild.DEPARTMENTNAME);
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("StageSetupManagement", "Exception occured on TranslateStageSetupChildToParameterList, " + ex.Message);
            }


            return parmList;
        }

        #endregion


        #region LogSetup
        public DataSet GetStageSetupLogByDOCNUM(string ID)
        {
            DataSet ds = new DataSet();
            try

            {
                List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();

                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = Convert.ToString(ID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                ds = HANADAL.GetDataSetByStoredProcedure("GETSTAGE_SETUPLOGBYDOCNUM", parmList, "StageSetupManagement");
                //ds.Tables["Table"].Columns.Remove(ds.Tables["Table"].Columns["ID"]);
                ds.Tables["Table"].Columns.Remove(ds.Tables["Table"].Columns["DOCID"]);
                ds.Tables["Table"].Columns.Remove(ds.Tables["Table"].Columns["CREATEDBY"]);
                ds.Tables["Table1"].Columns.Remove(ds.Tables["Table1"].Columns["DOCID"]);
                ds.Tables["Table1"].Columns.Remove(ds.Tables["Table1"].Columns["STAGESETUP_ID"]);

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("StageSetupManagement", "Exception occured on GetAllAssignmentCostSetup, " + ex.Message);
            }

            return ds;
        }
        
        private List<B1SP_Parameter> TranslateStageSetupLogToParameterList(StageSetupInfo stageSetup)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {

                B1SP_Parameter parm = new B1SP_Parameter();
                parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = Convert.ToString(stageSetup.ID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "DOCNUM";
                parm.ParameterValue = Convert.ToString(stageSetup.DOCNUM);
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "DOCID";
                parm.ParameterValue = Convert.ToString(stageSetup.ID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "STAGECODE";
                parm.ParameterValue = Convert.ToString(stageSetup.STAGECODE);
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "STAGEDESCRIPTION_Previous";
                parm.ParameterValue = Convert.ToString(stageSetup.STAGEDESCRIPTION);
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "APPROVALREQUIRED_Previous";
                parm.ParameterValue = Convert.ToString(stageSetup.APPROVALREQUIRED);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "REJECTIONREQUIRED_Previous";
                parm.ParameterValue = Convert.ToString(stageSetup.REJECTIONREQUIRED);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ISDELETED_Previous";
                parm.ParameterValue = Convert.ToString(stageSetup.ISDELETED);
                parm.ParameterType = DBTypes.Bool.ToString();
                parmList.Add(parm);


                parm = new B1SP_Parameter();
                parm.ParameterName = "STAGEDESCRIPTION_New";
                parm.ParameterValue = Convert.ToString(stageSetup.STAGEDESCRIPTION);
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "APPROVALREQUIRED_New";
                parm.ParameterValue = Convert.ToString(stageSetup.APPROVALREQUIRED);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "REJECTIONREQUIRED_New";
                parm.ParameterValue = Convert.ToString(stageSetup.REJECTIONREQUIRED);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ISDELETED_New";
                parm.ParameterValue = Convert.ToString(stageSetup.ISDELETED);
                parm.ParameterType = DBTypes.Bool.ToString();
                parmList.Add(parm);


                parm = new B1SP_Parameter();
                parm.ParameterName = "CREATEDBY";
                parm.ParameterValue = Convert.ToString(stageSetup.CREATEDBY);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CREATEDATE";
                parm.ParameterValue = DateTime.Now.ToString("yyyy-MM-dd HH:MM:ss");
                parm.ParameterType = DBTypes.String.ToString();
                //parm.ParameterValue = string.Format("{0:dd/MM/yyyy}", DateTime.Now);// DateTime.Now.ToString("dd-MM-yyyy HH:MM:ss");
                parmList.Add(parm);

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("StageSetupManagement", "Exception occured on TranslateStageSetupLogToParameterList, " + ex.Message);
            }


            return parmList;
        }

        private List<B1SP_Parameter> TranslateStageSetupChildLogToParameterList(StageSetupChildInfo stageSetupChild)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {

                B1SP_Parameter parm = new B1SP_Parameter();

                parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = Convert.ToString(0);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "STAGESETUP_ID";
                parm.ParameterValue = Convert.ToString(stageSetupChild.STAGESETUP_ID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "DOCID";
                parm.ParameterValue = Convert.ToString(stageSetupChild.STAGESETUP_ID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "UserID_Previous";
                parm.ParameterValue = Convert.ToString(stageSetupChild.UserID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "FullName_Previous";
                parm.ParameterValue = Convert.ToString(stageSetupChild.FULLNAME);
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ISDELETED_Previous";
                parm.ParameterValue = Convert.ToString(stageSetupChild.ISDELETED);
                parm.ParameterType = DBTypes.Bool.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "UserID_New";
                parm.ParameterValue = Convert.ToString(stageSetupChild.UserID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "FullName_New";
                parm.ParameterValue = Convert.ToString(stageSetupChild.FULLNAME);
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ISDELETED_New";
                parm.ParameterValue = Convert.ToString(stageSetupChild.ISDELETED);
                parm.ParameterType = DBTypes.Bool.ToString();
                parmList.Add(parm);

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("StageSetupManagement", "Exception occured on TranslateStageSetupChildLogToParameterList, " + ex.Message);
            }


            return parmList;
        }


        #endregion
    }
}