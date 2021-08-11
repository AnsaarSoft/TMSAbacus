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

    public class TravelRatesManagement
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
                log.InputOutputDocLog("TravelRatesManagement", "Exception occured on TranslateTravelRatesToParameterList, " + ex.Message);
            }


            return parmList;
        }

        #region "TravelRates"

        public List<TravelRates> GetTravel_RatesSetupByBranchID(int ID)
        {
            List<TravelRates> TravelRates = new List<TravelRates>();
            try
            {
                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                DataTable dt_TravelRates = HANADAL.GetDataTableByStoredProcedure("GetTravel_RatesByBranchID", TranslateIDToParameterList(ID), "TravelRatesManagement");
                if (dt_TravelRates.Rows.Count > 0)
                {
                    TravelRates = TranslateDataTableToTravelRatesManagementList(dt_TravelRates);
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("TravelRatesManagement", "Exception occured on GetMaster_Task ID: " + ID + " , " + ex.Message);
            }

            return TravelRates;
        }

        public List<TravelRates> GetTravel_RatesSetup(int ID)
        {
            List<TravelRates> TravelRates = new List<TravelRates>();
            try
            {
                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                DataTable dt_TravelRates = HANADAL.GetDataTableByStoredProcedure("GetTravel_Rates", TranslateIDToParameterList(ID), "TravelRatesManagement");
                if (dt_TravelRates.Rows.Count > 0)
                {
                    TravelRates = TranslateDataTableToTravelRatesManagementList(dt_TravelRates);
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("TravelRatesManagement", "Exception occured on GetMaster_Task ID: " + ID + " , " + ex.Message);
            }

            return TravelRates;
        }
        public bool AddUpdateTravelRates(List<TravelRates> TravelRates,out string msg)
        {
            bool isSuccess = false;
            bool isAddOccured = false;
            bool isUpdateOccured = false;
            bool isDeleteOccured = false;
            msg = "Successfully Added/Updated";
            int updatedby = 0;
            string docNum = "";
            try
            {
                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                List<TravelRates> previousList = new List<Models.TravelRates>();

                

                foreach (var item in TravelRates)
                {
                    item.FROMKM_TOKM = item.FROMKM + "_" + item.TOKM;
                }

                var filtererdList = TravelRates.Where(x => x.ISDELETED == false).ToList();
                var duplicates = filtererdList.GroupBy(x => x.FROMKM_TOKM).Where(g => g.Count() > 1).Select(y => y.Key).ToList();
                if (duplicates.Count > 0)
                {
                    msg = "Duplicate record exist!";
                    return false;
                }

                //TravelRates = TravelRates.Where(x => x.ID > 0 || x.ISDELETED == false).ToList();
                if(TravelRates.Count>0)
                {
                    var itemDoc = TravelRates.Where(x => x.DOCNUM != "").FirstOrDefault();

                    if (itemDoc != null)
                    {
                        docNum = itemDoc.DOCNUM;
                    }
                }
                

                if(string.IsNullOrEmpty(docNum))
                {
                    int no = 1;
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
                

                if(TravelRates.Count>0)
                {
                    
                    updatedby = Convert.ToInt32(TravelRates[0].UPDATEDBY);
                    previousList = GetTravel_RatesSetupByBranchID(Convert.ToInt32(TravelRates[0].BRANCHID));
                }
                
                foreach (var list in TravelRates)
                {
                    
                    try
                    {
                        list.DOCNUM = docNum;
                        DataTable dt_TravelRates = HANADAL.AddUpdateDataByStoredProcedure("AddUpdateTravel_Rates", TranslateTravelRatesToParameterList(list), "TravelRatesManagement");
                        if (dt_TravelRates.Rows.Count == 0)
                            throw new Exception("Exception occured when Add/Update Task Master setup, ID:" + list.ID + " , From KM:" + list.FROMKM + " , To KM" + list.TOKM);
                    }
                    catch (Exception ex)
                    {
                        isSuccess = false;
                        Log log = new Log();
                        log.LogFile(ex.Message);
                        log.InputOutputDocLog("TravelRatesManagement", "Exception occured in foreach loop AddUpdateMaster_Task, " + ex.Message);
                        continue;
                    }
                }
                if(previousList.Count>0)
                {
                    //List<TravelRates> missingUserList = TravelRates.Where(n => !previousList.Any(o => o.ID == n.ID)).ToList();

                    var missingUserList = previousList.Where(x => !TravelRates.Select(i => i.ID).Contains(x.ID));

                    foreach (var item in missingUserList)
                    {
                        item.ISDELETED = true;
                        item.UPDATEDBY = updatedby;
                        HANADAL.AddUpdateDataByStoredProcedure("AddUpdateTravel_Rates", TranslateTravelRatesToParameterList(item), "TravelRatesManagement");
                    }
                }


                //For Form Log
                AddTravelRates_Log(previousList, out isUpdateOccured);
                //


                //For Master Log
                if (TravelRates.Where(x => x.ID == 0).ToList().Count > 0)
                    isAddOccured = true;
                if (TravelRates.Where(x => x.ISDELETED == true).ToList().Count > 0)
                    isDeleteOccured = true;

                int createdBy = 0;
                var val = TravelRates.Where(x => x.CREATEDBY != null).FirstOrDefault();
                if (val != null)
                    createdBy = Convert.ToInt32(val.CREATEDBY);

                Common cmn = new Common();
                Task.Run(() => cmn.AddMasterLog(isAddOccured, isUpdateOccured, isDeleteOccured, nameof(Enums.FormsName.TravelRatesSetup), createdBy, "TravelRatesManagement"));
                //End MAster Log


                isSuccess = true;
            }
            catch (Exception ex)
            {
                isSuccess = false;
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("TravelRatesManagement", "Exception occured on AddUpdateMaster_Task, " + ex.Message);
            }

            return isSuccess;
        }

        public void AddTravelRates_Log(List<TravelRates> TravelRates, out bool isUpdateOccured)
        {
            isUpdateOccured = false;
            try
            {
                TravelRates = TravelRates.Where(x => x.ID > 0).ToList();

                foreach (var newObject in TravelRates)
                {

                    try
                    {
                        List<TravelRates> previousObject = GetTravel_RatesSetup(Convert.ToInt32(newObject.ID));
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
                                Task.Run(() => HANADAL.AddUpdateDataByStoredProcedure("AddTravel_Rates_LOG", paramList, "TravelRatesManagement"));
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        Log log = new Log();
                        log.LogFile(ex.Message);
                        log.InputOutputDocLog("TravelRatesManagement", "Exception occured in foreach loop AddTravelRates_Log, " + ex.Message);
                        continue;
                    }


                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("TravelRatesManagement", "Exception occured on AddTravelRates_Log, " + ex.Message);
            }
        }

        public DataTable GetTravelRatesLog()
        {
            DataTable dt_TravelRatesSetupLog = new DataTable();
            try
            {
                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                dt_TravelRatesSetupLog = HANADAL.GetDataTableByStoredProcedure("GetTravel_Rates_LOG", "TravelRatesManagement");

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("TravelRatesManagement", "Exception occured on GetTravelRatesLog, " + ex.Message);
            }

            return dt_TravelRatesSetupLog;
        }

        public List<TravelRates> GetTaskMasterByFunctionID(int id)
        {
            List<TravelRates> TaskMasterList = new List<TravelRates>();
            try
            {
                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                Common cmn = new Common();
                //List<HCM_Designation> designationList = cmn.GetHCMDesignationList();
                DataTable dt_TravelRatesSetup = HANADAL.GetDataTableByStoredProcedure("GetMaster_TaskByFunctionID", cmn.TranslateIDToParameterList(id), "TravelRatesManagement");
                bool isNewDoc = false;
                if (dt_TravelRatesSetup.Rows.Count > 0)
                {
                    TaskMasterList = TranslateDataTableToTravelRatesManagementList(dt_TravelRatesSetup);
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
                log.InputOutputDocLog("TravelRatesManagement", "Exception occured on GetTaskMasterByFunctionID ID: " + id + " , " + ex.Message);
            }

            return TaskMasterList;
        }


        public List<string> GetTaskMasterDocNum()
        {
            List<string> docNumList = new List<string>();
            try
            {
                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();

                DataTable dt = HANADAL.GetDataTableByStoredProcedure("GetTravel_Rates_LastDocNum", "TravelRatesManagement");
                if (dt.Rows.Count > 0)
                {
                    docNumList = TranslateDataTableToMaster_TaskDocNumList(dt);
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("TravelRatesManagement", "Exception occured on GetTaskMasterDocNum, " + ex.Message);
            }

            return docNumList;
        }

        public List<TravelRates> GetTask_MasterByDocNum(string docNo)
        {
            List<TravelRates> Task_MasterList = new List<TravelRates>();
            try
            {

                List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();

                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "DOCNUM";
                parm.ParameterValue = docNo;
                parmList.Add(parm);

                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                Common cmn = new Common();
                DataTable dt_Task_Master = HANADAL.GetDataTableByStoredProcedure("GetMaster_TaskByDocNum", parmList, "TravelRatesManagement");
                if (dt_Task_Master.Rows.Count > 0)
                {

                    Task_MasterList = TranslateDataTableToTravelRatesManagementList(dt_Task_Master);
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
                log.InputOutputDocLog("TravelRatesManagement", "Exception occured on GetResourceBillingRatesByDocNum DocNum: " + docNo + " , " + ex.Message);
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

        #region "Translation TravelRates"
            

        private List<TravelRates> TranslateDataTableToTravelRatesManagementList(DataTable dt)
        {
            List<TravelRates> TravelRates = new List<TravelRates>();

            try
            {
                int sno = 1;
                foreach (DataRow dtRow in dt.Rows)
                {
                    TravelRates travelrates = new TravelRates();
                    travelrates.SNO = sno;
                    travelrates.KEY = Guid.NewGuid().ToString();
                    travelrates.ID = Convert.ToInt32(dtRow["ID"]);
                    travelrates.BRANCHID = Convert.ToInt32(dtRow["BRANCHID"]);
                    travelrates.FROMKM= Convert.ToDouble(dtRow["FROMKM"]);
                    travelrates.TOKM = Convert.ToDouble(dtRow["TOKM"]);
                    travelrates.RATETRIP = Convert.ToDouble(dtRow["RATETRIP"]);
                    travelrates.DOCNUM= Convert.ToString(dtRow["DOCNUM"]);
                    travelrates.CREATEDBY = Convert.ToInt32(dtRow["CREATEDBY"]);
                    travelrates.CREATEDATE = Convert.ToDateTime(dtRow["CREATEDATE"]);

                    if (dtRow["UPDATEDBY"] != DBNull.Value)
                        travelrates.UPDATEDBY = Convert.ToInt32(dtRow["UPDATEDBY"]);
                    if (dtRow["UPDATEDATE"] != DBNull.Value)
                        travelrates.UPDATEDATE = Convert.ToDateTime(dtRow["UPDATEDATE"]);

                    travelrates.ISACTIVE = Convert.ToBoolean(dtRow["ISACTIVE"]);
                    travelrates.ISDELETED = Convert.ToBoolean(dtRow["ISDELETED"]);
                    sno = sno + 1;
                    TravelRates.Add(travelrates);
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("TravelRatesManagement", "Exception occured on TranslateDataTableToTravelRatesManagementList, " + ex.Message);
            }

            return TravelRates;
        }

        private List<B1SP_Parameter> TranslateTravelRatesToParameterList(TravelRates travelrates)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {

                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = Convert.ToString(travelrates.ID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ISACTIVE";
                parm.ParameterValue = Convert.ToString(travelrates.ISACTIVE);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ISDELETED";
                parm.ParameterValue = Convert.ToString(travelrates.ISDELETED);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CREATEDBY";
                parm.ParameterValue = Convert.ToString(travelrates.CREATEDBY);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CREATEDATE";
                parm.ParameterValue = DateTime.Now.ToString("yyyy-MM-dd HH:MM:s");
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "UPDATEDBY";
                parm.ParameterValue = Convert.ToString(travelrates.UPDATEDBY);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "UPDATEDATE";
                parm.ParameterValue = DateTime.Now.ToString("yyyy-MM-dd HH:MM:s");
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "BRANCHID";
                parm.ParameterValue = Convert.ToString(travelrates.BRANCHID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "DOCNUM";
                parm.ParameterValue = Convert.ToString(travelrates.DOCNUM);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "FROMKM";
                parm.ParameterValue = Convert.ToString(travelrates.FROMKM);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "TOKM";
                parm.ParameterValue = Convert.ToString(travelrates.TOKM);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "RATETRIP";
                parm.ParameterValue = Convert.ToString(travelrates.RATETRIP);
                parmList.Add(parm);


            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("TravelRatesManagement", "Exception occured on TranslateTravelRatesToParameterList, " + ex.Message);
            }


            return parmList;
        }

        private List<B1SP_Parameter> TranslateMaster_TaskLogToParameterList(TravelRates travelrates)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {

                B1SP_Parameter parm = new B1SP_Parameter();

                parm = new B1SP_Parameter();
                parm.ParameterName = "FROMKM_PREVIOUS";
                parm.ParameterValue = Convert.ToString(travelrates.FROMKM);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "FROMKM_NEW";
                parm.ParameterValue = Convert.ToString(travelrates.FROMKM);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "TOKM_NEW";
                parm.ParameterValue = Convert.ToString(travelrates.TOKM);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "TOKM_PREVIOUS";
                parm.ParameterValue = Convert.ToString(travelrates.TOKM);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "RATETRIP_PREVIOUS";
                parm.ParameterValue = Convert.ToString(travelrates.RATETRIP);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "RATETRIP_NEW";
                parm.ParameterValue = Convert.ToString(travelrates.RATETRIP);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ISACTIVE_PREVIOUS";
                parm.ParameterValue = Convert.ToString(travelrates.ISACTIVE);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ISACTIVE_NEW";
                parm.ParameterValue = Convert.ToString(travelrates.ISACTIVE);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ISDELETED_PREVIOUS";
                parm.ParameterValue = Convert.ToString(travelrates.ISDELETED);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ISDELETED_NEW";
                parm.ParameterValue = Convert.ToString(travelrates.ISDELETED);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CREATEDBY";
                parm.ParameterValue = Convert.ToString(travelrates.CREATEDBY);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CREATEDATE";
                parm.ParameterValue = DateTime.Now.ToString("yyyy-MM-dd HH:MM:s");
                parmList.Add(parm);

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("TravelRatesManagement", "Exception occured on TranslateMaster_TaskLogToParameterList, " + ex.Message);
            }


            return parmList;
        }

        #endregion

        public DataTable GetTravel_RatesAllDocumentsList()
        {
            DataTable dt = new DataTable();
            Common cmn = new Common();
            try

            {
                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                dt = HANADAL.GetDataTableByStoredProcedure("GetTravel_Rates_AllDocuments", "TravelRatesManagement");
                dt = cmn.RemoveDuplicateRows(dt, "DOCNUM");
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("TravelRatesManagement", "Exception occured on GetTravel_RatesAllDocumentsList, " + ex.Message);
            }

            return dt;
        }
    }
}