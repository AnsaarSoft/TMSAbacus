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

    public class TaskMasterManagement
    {
        private List<B1SP_Parameter> TranslateIDToParameterList(int Id)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {

                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = Convert.ToString(Id);
                parmList.Add(parm);


            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("TaskMasterManagement", "Exception occured on TranslateTaskMasterToParameterList, " + ex.Message);
            }


            return parmList;
        }

        #region "TaskMasterSetupInfo"

        public List<TaskMasterSetupInfo> GetTaskMasterSetup(int id = 0)
        {
            List<TaskMasterSetupInfo> TaskMasterSetupInfo = new List<TaskMasterSetupInfo>();
            try
            {
                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                DataTable dt_TaskMaster = HANADAL.GetDataTableByStoredProcedure("GetMaster_Task", TranslateIDToParameterList(id), "TaskMasterManagement");
                if (dt_TaskMaster.Rows.Count > 0)
                {
                    TaskMasterSetupInfo = TranslateDataTableToTaskMasterManagementList(dt_TaskMaster);
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("TaskMasterManagement", "Exception occured on GetMaster_Task ID: " + id + " , " + ex.Message);
            }

            return TaskMasterSetupInfo;
        }
        public bool AddUpdateTaskMasterSetup(List<TaskMasterSetupInfo> TaskMasterSetupInfo,out string msg)
        {
            bool isSuccess = false;
            bool isAddOccured = false;
            bool isUpdateOccured = false;
            bool isDeleteOccured = false;
            msg = "Successfully Added/Updated";
            try
            {
                var filtererdList = TaskMasterSetupInfo.Where(x => x.ISDELETED == false).ToList();
                var duplicates = filtererdList.GroupBy(x => x.TASK).Where(g => g.Count() > 1).Select(y => y.Key).ToList();
                if (duplicates.Count > 0)
                {
                    msg = "Duplicate record exist!";
                    return false;
                }


                TaskMasterSetupInfo = TaskMasterSetupInfo.Where(x => x.ID > 0 || x.ISDELETED == false).ToList();

                //For Form Log
                AddTaskMasterSetup_Log(TaskMasterSetupInfo, out isUpdateOccured);
                //
                
                int no = 1;
                string docNum = "";
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

                bool isnew = true;

                foreach (var list in TaskMasterSetupInfo)
                {
                    try
                    {
                        if (isnew == true && list.ID == 0)
                        {
                            foreach (var item in TaskMasterSetupInfo)
                            {
                                item.DOCNUM = docNum;
                            }
                            isnew = false;
                        }
                        HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                        DataTable dt_TaskMaster = HANADAL.AddUpdateDataByStoredProcedure("AddUpdateMaster_Task", TranslateTaskMasterToParameterList(list), "TaskMasterManagement");
                        if (dt_TaskMaster.Rows.Count == 0)
                            throw new Exception("Exception occured when Add/Update Task Master setup, ID:" + list.ID + " , TASK :" + list.TASK);
                    }
                    catch (Exception ex)
                    {
                        isSuccess = false;
                        Log log = new Log();
                        log.LogFile(ex.Message);
                        log.InputOutputDocLog("TaskMasterManagement", "Exception occured in foreach loop AddUpdateMaster_Task, " + ex.Message);
                        continue;
                    }
                }

                //For Master Log
                if (TaskMasterSetupInfo.Where(x => x.ID == 0).ToList().Count > 0)
                    isAddOccured = true;
                if (TaskMasterSetupInfo.Where(x => x.ISDELETED == true).ToList().Count > 0)
                    isDeleteOccured = true;

                int createdBy = 0;
                var val = TaskMasterSetupInfo.Where(x => x.CREATEDBY != null).FirstOrDefault();
                if (val != null)
                    createdBy = Convert.ToInt32(val.CREATEDBY);

                Common cmn = new Common();
                Task.Run(() => cmn.AddMasterLog(isAddOccured, isUpdateOccured, isDeleteOccured, nameof(Enums.FormsName.TaskMasterSetup), createdBy, "TaskMasterManagement"));
                //End MAster Log


                isSuccess = true;
            }
            catch (Exception ex)
            {
                isSuccess = false;
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("TaskMasterManagement", "Exception occured on AddUpdateMaster_Task, " + ex.Message);
            }

            return isSuccess;
        }

        public void AddTaskMasterSetup_Log(List<TaskMasterSetupInfo> TaskMasterSetupInfo, out bool isUpdateOccured)
        {
            isUpdateOccured = false;
            try
            {
                TaskMasterSetupInfo = TaskMasterSetupInfo.Where(x => x.ID > 0).ToList();

                foreach (var newObject in TaskMasterSetupInfo)
                {

                    try
                    {
                        List<TaskMasterSetupInfo> previousObject = GetTaskMasterSetup(Convert.ToInt32(newObject.ID));
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
                                    case "TASK":
                                        paramList.Where(x => x.ParameterName == "TASK_PREVIOUS").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                        paramList.Where(x => x.ParameterName == "TASK_NEW").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();

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
                                Task.Run(() => HANADAL.AddUpdateDataByStoredProcedure("AddMaster_TaskSetup_LOG", paramList, "TaskMasterManagement"));
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        Log log = new Log();
                        log.LogFile(ex.Message);
                        log.InputOutputDocLog("TaskMasterManagement", "Exception occured in foreach loop AddTaskMasterSetup_Log, " + ex.Message);
                        continue;
                    }


                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("TaskMasterManagement", "Exception occured on AddTaskMasterSetup_Log, " + ex.Message);
            }
        }

        public DataTable GetTaskMasterSetupLog()
        {
            DataTable dt_TaskMasterSetupLog = new DataTable();
            try
            {
                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                dt_TaskMasterSetupLog = HANADAL.GetDataTableByStoredProcedure("GetMaster_TaskSetup_LOG", "TaskMasterManagement");

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("TaskMasterManagement", "Exception occured on GetTaskMasterSetupLog, " + ex.Message);
            }

            return dt_TaskMasterSetupLog;
        }

        public List<B1SP_Parameter> TranslateFunctionCodeToParameterList(string FunctionCode)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {

                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "FunctionCode";
                parm.ParameterValue = Convert.ToString(FunctionCode);
                parmList.Add(parm);
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("SetupManagement", "Exception occured on TranslateCodeToParameterList, " + ex.Message);
            }


            return parmList;
        }
        public List<TaskMasterSetupInfo> GetTaskMasterByFunctionID(string functionID)
        {
            List<TaskMasterSetupInfo> TaskMasterList = new List<TaskMasterSetupInfo>();
            try
            {
                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                Common cmn = new Common();
                //List<HCM_Designation> designationList = cmn.GetHCMDesignationList();
                DataTable dt_taskMasterSetup = HANADAL.GetDataTableByStoredProcedure("GetMaster_TaskByFunctionID", TranslateFunctionCodeToParameterList(functionID), "TaskMasterManagement");
                bool isNewDoc = false;
                if (dt_taskMasterSetup.Rows.Count > 0)
                {
                    TaskMasterList = TranslateDataTableToTaskMasterManagementList(dt_taskMasterSetup);
                }
                else
                    isNewDoc = true;


                foreach (var item in TaskMasterList)
                {
                    item.isNewDocument = isNewDoc;
                    item.KEY = Guid.NewGuid().ToString();

                }
                //List<HCM_Designation> missingDesignationList = designationList.Where(n => !resourceBillingRatesList.Any(o => o.DesignationID == n.DesignationID)).Where(x => x.FunctionID == id).ToList();

                //int sNo = resourceBillingRatesList.Count() + 1;

                //foreach (var item in missingDesignationList)
                //{
                //    ResourceBillingRates resourceBillingRates = new ResourceBillingRates();
                //    resourceBillingRates.DesignationID = item.DesignationID;
                //    resourceBillingRates.DesignationName = item.DesignationName;
                //    resourceBillingRates.FunctionID = item.FunctionID;
                //    resourceBillingRates.ID = 0;
                //    resourceBillingRates.KEY = Guid.NewGuid().ToString();
                //    resourceBillingRates.SNO = sNo;
                //    resourceBillingRates.ISACTIVE = true;
                //    resourceBillingRates.ISDELETED = false;
                //    resourceBillingRates.RatesPerHour = 0;
                //    resourceBillingRates.isNewDocument = isNewDoc;
                //    resourceBillingRatesList.Add(resourceBillingRates);
                //    sNo++;
                //}

                int sNo = TaskMasterList.Count() + 1;


                string docNum = "";
                var value = TaskMasterList.Where(x => x.DOCNUM != null).FirstOrDefault();
                if (value != null)
                {
                    docNum = value.DOCNUM;
                }
                else
                {

                    //int no = 1;
                    //List<string> docNumList = GetTaskMasterDocNum();
                    //if (docNumList.Count > 0)
                    //{
                    //    var item = docNumList[docNumList.Count - 1];
                    //    //no = Convert.ToInt32(item.Split('-')[1]);
                    //    no = no + 1;
                    //    docNum = Convert.ToString(no).PadLeft(5, '0');
                    //    docNum = "Doc-" + docNum;
                    //}
                    //else
                    //{
                    //    docNum = Convert.ToString(no).PadLeft(5, '0');
                    //    docNum = "Doc-" + docNum;
                    //}
                }

                TaskMasterList.Select(c => { c.DOCNUM = docNum; return c; }).ToList();
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("TaskMasterManagement", "Exception occured on GetTaskMasterByFunctionID: " + functionID + " , " + ex.Message);
            }

            return TaskMasterList;
        }


        public List<string> GetTaskMasterDocNum()
        {
            List<string> docNumList = new List<string>();
            try
            {
                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();

                DataTable dt = HANADAL.GetDataTableByStoredProcedure("GetMaster_TaskDocNum", "TaskMasterManagement");
                if (dt.Rows.Count > 0)
                {
                    docNumList = TranslateDataTableToMaster_TaskDocNumList(dt);
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("TaskMasterManagement", "Exception occured on GetTaskMasterDocNum, " + ex.Message);
            }

            return docNumList;
        }

        public List<TaskMasterSetupInfo> GetTask_MasterByDocNum(string docNo)
        {
            List<TaskMasterSetupInfo> Task_MasterList = new List<TaskMasterSetupInfo>();
            try
            {

                List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();

                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "DOCNUM";
                parm.ParameterValue = docNo;
                parmList.Add(parm);

                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                Common cmn = new Common();
                DataTable dt_Task_Master = HANADAL.GetDataTableByStoredProcedure("GetMaster_TaskByDocNum", parmList, "TaskMasterManagement");
                if (dt_Task_Master.Rows.Count > 0)
                {

                    Task_MasterList = TranslateDataTableToTaskMasterManagementList(dt_Task_Master);
                }
                /*
                List<HCM_Designation> missingDesignationList = designationList.Where(n => !Task_MasterList.Any(o => o.DesignationID == n.DesignationID)).Where(x => x.FunctionID == id).ToList();

                int sNo = Task_MasterList.Count() + 1;
                foreach (var item in missingDesignationList)
                {
                    ResourceBillingRates resourceBillingRates = new ResourceBillingRates();
                    resourceBillingRates.DesignationID = item.DesignationID;
                    resourceBillingRates.DesignationName = item.DesignationName;
                    resourceBillingRates.FunctionID = item.FunctionID;
                    resourceBillingRates.ID = 0;
                    resourceBillingRates.KEY = Guid.NewGuid().ToString();
                    resourceBillingRates.SNO = sNo;
                    resourceBillingRates.ISACTIVE = true;
                    resourceBillingRates.ISDELETED = false;
                    resourceBillingRates.RatesPerHour = 0;
                    Task_MasterList.Add(resourceBillingRates);
                    sNo++;
                }

                string docNum = "";
                var value = Task_MasterList.Where(x => x.DocNum != null).FirstOrDefault();
                if (value != null)
                {
                    docNum = value.DocNum;
                }
                else
                {

                    int no = 1;
                    List<string> docNumList = GetResourceBillingRatesDocNum();
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
                Task_MasterList.Select(c => { c.DocNum = docNum; return c; }).ToList();
                */
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("TaskMasterManagement", "Exception occured on GetResourceBillingRatesByDocNum DocNum: " + docNo + " , " + ex.Message);
            }

            return Task_MasterList;
        }

        private List<string> TranslateDataTableToMaster_TaskDocNumList(DataTable dt)
        {
            List<string> docNumList = new List<string>();
            foreach (DataRow dtRow in dt.Rows)
            {
                docNumList.Add(Convert.ToString(dtRow["DocNum"]));
            }

            return docNumList.Distinct().ToList();
        }

        #endregion

        #region "Translation TaskMasterSetupInfo"


        private List<TaskMasterSetupInfo> TranslateDataTableToTaskMasterManagementList(DataTable dt)
        {
            List<TaskMasterSetupInfo> TaskMasterSetupInfo = new List<TaskMasterSetupInfo>();

            try
            {
                int sno = 1;
                foreach (DataRow dtRow in dt.Rows)
                {
                    TaskMasterSetupInfo taskMasterSetup = new TaskMasterSetupInfo();
                    taskMasterSetup.SNO = sno;
                    taskMasterSetup.KEY = Guid.NewGuid().ToString();
                    taskMasterSetup.ID = Convert.ToInt32(dtRow["ID"]);
                    taskMasterSetup.TASK= Convert.ToString(dtRow["TASK"]);
                    taskMasterSetup.FUNCTIONID = Convert.ToInt32(dtRow["FUNCTIONID"]);
                    taskMasterSetup.FUNCTIONNAME = Convert.ToString(dtRow["FUNCTIONNAME"]);
                    taskMasterSetup.DOCNUM= Convert.ToString(dtRow["DOCNUM"]);
                    taskMasterSetup.CREATEDBY = Convert.ToInt32(dtRow["CREATEDBY"]);
                    taskMasterSetup.CREATEDDATE = Convert.ToDateTime(dtRow["CREATEDDATE"]);

                    if (dtRow["UPDATEDEDBY"] != DBNull.Value)
                        taskMasterSetup.UPDATEDEDBY = Convert.ToInt32(dtRow["UPDATEDEDBY"]);
                    if (dtRow["UPDATEDDATE"] != DBNull.Value)
                        taskMasterSetup.UPDATEDDATE = Convert.ToDateTime(dtRow["UPDATEDDATE"]);

                    taskMasterSetup.ISACTIVE = Convert.ToBoolean(dtRow["ISACTIVE"]);
                    taskMasterSetup.ISDELETED = Convert.ToBoolean(dtRow["ISDELETED"]);
                    sno = sno + 1;
                    TaskMasterSetupInfo.Add(taskMasterSetup);
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("TaskMasterManagement", "Exception occured on TranslateDataTableToTaskMasterManagementList, " + ex.Message);
            }

            return TaskMasterSetupInfo;
        }

        private List<B1SP_Parameter> TranslateTaskMasterToParameterList(TaskMasterSetupInfo taskMasterSetup)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {

                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = Convert.ToString(taskMasterSetup.ID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ISACTIVE";
                parm.ParameterValue = Convert.ToString(taskMasterSetup.ISACTIVE);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ISDELETED";
                parm.ParameterValue = Convert.ToString(taskMasterSetup.ISDELETED);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CREATEDBY";
                parm.ParameterValue = Convert.ToString(taskMasterSetup.CREATEDBY);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CREATEDDATE";
                parm.ParameterValue = DateTime.Now.ToString("yyyy-MM-dd HH:MM:s");
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "UPDATEDEDBY";
                parm.ParameterValue = Convert.ToString(taskMasterSetup.UPDATEDEDBY);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "UPDATEDDATE";
                parm.ParameterValue = DateTime.Now.ToString("yyyy-MM-dd HH:MM:s");
                parmList.Add(parm);


                parm = new B1SP_Parameter();
                parm.ParameterName = "FUNCTIONID";
                //parm.ParameterValue = Convert.ToString(taskMasterSetup.FUNCTIONID);
                parm.ParameterValue = Convert.ToString(0);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "FUNCTIONNAME";
                parm.ParameterValue = Convert.ToString(taskMasterSetup.FUNCTIONNAME);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "DOCNUM";
                parm.ParameterValue = Convert.ToString(taskMasterSetup.DOCNUM);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "TASK";
                parm.ParameterValue = Convert.ToString(taskMasterSetup.TASK);
                parmList.Add(parm);

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("TaskMasterManagement", "Exception occured on TranslateTaskMasterToParameterList, " + ex.Message);
            }


            return parmList;
        }

        private List<B1SP_Parameter> TranslateMaster_TaskLogToParameterList(TaskMasterSetupInfo taskMasterSetup)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {

                B1SP_Parameter parm = new B1SP_Parameter();

                parm = new B1SP_Parameter();
                parm.ParameterName = "TASK_PREVIOUS";
                parm.ParameterValue = Convert.ToString(taskMasterSetup.TASK);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "TASK_NEW";
                parm.ParameterValue = Convert.ToString(taskMasterSetup.TASK);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ISACTIVE_PREVIOUS";
                parm.ParameterValue = Convert.ToString(taskMasterSetup.ISACTIVE);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ISACTIVE_NEW";
                parm.ParameterValue = Convert.ToString(taskMasterSetup.ISACTIVE);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ISDELETED_PREVIOUS";
                parm.ParameterValue = Convert.ToString(taskMasterSetup.ISDELETED);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ISDELETED_NEW";
                parm.ParameterValue = Convert.ToString(taskMasterSetup.ISDELETED);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CREATEDBY";
                parm.ParameterValue = Convert.ToString(taskMasterSetup.CREATEDBY);
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
                log.InputOutputDocLog("TaskMasterManagement", "Exception occured on TranslateMaster_TaskLogToParameterList, " + ex.Message);
            }


            return parmList;
        }

        #endregion


        public DataTable GetTask_MasterAllDocumentsList()
        {
            DataTable dt = new DataTable();
            Common cmn = new Common();
            try

            {
                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                dt = HANADAL.GetDataTableByStoredProcedure("GetMaster_Task_AllDocuments", "TaskMasterManagement");
                dt = cmn.RemoveDuplicateRows(dt, "DOCNUM");
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("TaskMasterManagement", "Exception occured on GetTask_MasterAllDocumentsList, " + ex.Message);
            }

            return dt;
        }
    }
}