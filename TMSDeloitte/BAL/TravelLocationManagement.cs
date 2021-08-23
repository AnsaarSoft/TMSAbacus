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
    public class TravelLocationManagement
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
                log.InputOutputDocLog("TravelLocationManagement", "Exception occured on TranslateTravelLocationToParameterList, " + ex.Message);
            }


            return parmList;
        }

        #region "TravelLocation"

        public List<TravelLocationInfo> GetTravel_LocationSetup_ByBranchID(int ID)
        {
            List<TravelLocationInfo> TravelLocation = new List<TravelLocationInfo>();
            try
            {
                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                DataTable dt_TravelLocation = HANADAL.GetDataTableByStoredProcedure("GetTravel_Location_ByBracnhID", TranslateIDToParameterList(ID), "TravelLocationManagement");
                if (dt_TravelLocation.Rows.Count > 0)
                {
                    TravelLocation = TranslateDataTableToTravelLocationManagementList(dt_TravelLocation);
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("TravelLocationManagement", "Exception occured on GetTravel_Location_ByBracnhID ID: " + ID + " , " + ex.Message);
            }

            return TravelLocation;
        }
        public List<TravelLocationInfo> GetTravel_LocationSetup(int ID)
        {
            List<TravelLocationInfo> TravelLocation = new List<TravelLocationInfo>();
            try
            {
                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                DataTable dt_TravelLocation = HANADAL.GetDataTableByStoredProcedure("GetTravel_Location_Setup", TranslateIDToParameterList(ID), "TravelLocationManagement");
                if (dt_TravelLocation.Rows.Count > 0)
                {
                    TravelLocation = TranslateDataTableToTravelLocationManagementList(dt_TravelLocation);
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("TravelLocationManagement", "Exception occured on GetMaster_Task ID: " + ID + " , " + ex.Message);
            }

            return TravelLocation;
        }

        public bool CheckDuplicateRecord(List<TravelLocationInfo> TravelLocation)
        {
            bool IsDuplicate = false;
            var query = TravelLocation.GroupBy(x => x.LOCATION)
             .Where(g => g.Count() > 1)
             .Select(y => y.Key)
             .ToList();
            if (query.Count>0)
            {
                IsDuplicate = true;
            }
            return IsDuplicate;
        }

        public string getDocNum() {
            int no = 1;
            string docNum = "";
            List<string> docNumList = GetTravelLocationDocNum();
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
            return docNum;
        }

        public bool AddUpdateTravelLocation(List<TravelLocationInfo> TravelLocation,out string msg)
        {
            bool isSuccess = false;
            bool isAddOccured = false;
            bool isUpdateOccured = false;
            bool isDeleteOccured = false;
            msg = "Successfully Added/Updated";
            string docNum = "";
            int updatedby = 0;
            try
            {
                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                List<TravelLocationInfo> previousList = new List<Models.TravelLocationInfo>();

                var filtererdList = TravelLocation.Where(x => x.ISDELETED == false).ToList();
                var duplicates = filtererdList.GroupBy(x => x.LOCATION).Where(g => g.Count() > 1).Select(y => y.Key).ToList();
                if (duplicates.Count > 0)
                {
                    msg = "Duplicate record exist!";
                    return false;
                }




                TravelLocation = TravelLocation.Where(x => x.ID > 0 || x.ISDELETED == false).ToList();

               

                if (TravelLocation.Count > 0)
                {
                    var item = TravelLocation.Where(x => x.DOCNUM != "").FirstOrDefault();

                    if (item != null)
                    {
                        docNum = item.DOCNUM;
                    }
                    else
                    {
                        docNum = getDocNum();
                    }

                    updatedby = Convert.ToInt32(TravelLocation[0].UPDATEDBY);
                    previousList = GetTask_MasterByDocNum(TravelLocation[0].DOCNUM);
                }


                bool isnew = true;
                foreach (var list in TravelLocation)
                {
                    
                    try
                    {

                        //if (isnew == true && list.ID == 0 && string.IsNullOrEmpty(list.DOCNUM))
                        //{

                        //    foreach (var item in TravelLocation)
                        //    {
                        //        item.DOCNUM = getDocNum();
                        //    }
                        //    isnew = false;
                        //}

                        list.DOCNUM = docNum;

                        DataTable dt_TravelLocation = HANADAL.AddUpdateDataByStoredProcedure("AddUpdateTravel_Location", TranslateTravelLocationToParameterList(list), "TravelLocationManagement");
                        if (dt_TravelLocation.Rows.Count == 0)
                            throw new Exception("Exception occured when Add/Update Travel Location, ID:" + list.ID + " , KM:" + list.KM + " , To Location" + list.LOCATION);
                    }
                    catch (Exception ex)
                    {
                        isSuccess = false;
                        Log log = new Log();
                        log.LogFile(ex.Message);
                        log.InputOutputDocLog("TravelLocationManagement", "Exception occured in foreach loop AddUpdateTravelLocation, " + ex.Message);
                        continue;
                    }
                }


                if (previousList.Count > 0)
                {
                    var missingList = previousList.Where(x => !TravelLocation.Select(i => i.ID).Contains(x.ID));

                    foreach (var item in missingList)
                    {
                        item.ISDELETED = true;
                        item.UPDATEDBY = updatedby;
                        HANADAL.AddUpdateDataByStoredProcedure("AddUpdateTravel_Location", TranslateTravelLocationToParameterList(item), "TravelLocationManagement");
                    }
                }

                //For Form Log
                AddTravelLocation_Log(previousList, out isUpdateOccured);
                //

                //For Master Log
                if (TravelLocation.Where(x => x.ID == 0).ToList().Count > 0)
                    isAddOccured = true;
                if (TravelLocation.Where(x => x.ISDELETED == true).ToList().Count > 0)
                    isDeleteOccured = true;

                int createdBy = 0;
                var val = TravelLocation.Where(x => x.CREATEDBY != null).FirstOrDefault();
                if (val != null)
                    createdBy = Convert.ToInt32(val.CREATEDBY);

                Common cmn = new Common();
                Task.Run(() => cmn.AddMasterLog(isAddOccured, isUpdateOccured, isDeleteOccured, nameof(Enums.FormsName.TravelLocationSetup), createdBy, "TravelLocationManagement"));
                //End MAster Log


                isSuccess = true;
            }
            catch (Exception ex)
            {
                isSuccess = false;
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("TravelLocationManagement", "Exception occured on AddUpdateTravelLocation, " + ex.Message);
            }

            return isSuccess;
        }

        public void AddTravelLocation_Log(List<TravelLocationInfo> TravelLocation, out bool isUpdateOccured)
        {
            isUpdateOccured = false;
            try
            {
                TravelLocation = TravelLocation.Where(x => x.ID > 0).ToList();

                foreach (var newObject in TravelLocation)
                {

                    try
                    {
                        List<TravelLocationInfo> previousObject = GetTravel_LocationSetup(Convert.ToInt32(newObject.ID));
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
                                    case "KM":
                                        paramList.Where(x => x.ParameterName == "KM_PREVIOUS").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                        paramList.Where(x => x.ParameterName == "KM_NEW").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();

                                        break;

                                    case "LOCATION":
                                        paramList.Where(x => x.ParameterName == "LOCATION_PREVIOUS").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                        paramList.Where(x => x.ParameterName == "LOCATION_NEW").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();

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
                                Task.Run(() => HANADAL.AddUpdateDataByStoredProcedure("AddTravel_Location_LOG", paramList, "TravelLocationManagement"));
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        Log log = new Log();
                        log.LogFile(ex.Message);
                        log.InputOutputDocLog("TravelLocationManagement", "Exception occured in foreach loop AddTravelLocation_Log, " + ex.Message);
                        continue;
                    }


                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("TravelLocationManagement", "Exception occured on AddTravelLocation_Log, " + ex.Message);
            }
        }

        public DataTable GetTravelLocationLog()
        {
            DataTable dt_TravelLocationSetupLog = new DataTable();
            try
            {
                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                dt_TravelLocationSetupLog = HANADAL.GetDataTableByStoredProcedure("GetTravel_Location_LOG", "TravelLocationManagement");

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("TravelLocationManagement", "Exception occured on GetTravelLocationLog, " + ex.Message);
            }

            return dt_TravelLocationSetupLog;
        }

        public List<TravelLocationInfo> GetTaskMasterByFunctionID(int id)
        {
            List<TravelLocationInfo> TaskMasterList = new List<TravelLocationInfo>();
            try
            {
                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                Common cmn = new Common();
                //List<HCM_Designation> designationList = cmn.GetHCMDesignationList();
                DataTable dt_TravelLocationSetup = HANADAL.GetDataTableByStoredProcedure("GetMaster_TaskByFunctionID", cmn.TranslateIDToParameterList(id), "TravelLocationManagement");
                bool isNewDoc = false;
                if (dt_TravelLocationSetup.Rows.Count > 0)
                {
                    TaskMasterList = TranslateDataTableToTravelLocationManagementList(dt_TravelLocationSetup);
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
                log.InputOutputDocLog("TravelLocationManagement", "Exception occured on GetTaskMasterByFunctionID ID: " + id + " , " + ex.Message);
            }

            return TaskMasterList;
        }


        public List<string> GetTravelLocationDocNum()
        {
            List<string> docNumList = new List<string>();
            try
            {
                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();

                DataTable dt = HANADAL.GetDataTableByStoredProcedure("GetTravel_Location_LastDocNum", "TravelLocationManagement");
                if (dt.Rows.Count > 0)
                {
                    docNumList = TranslateDataTableToTravel_LocationDocNumList(dt);
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("TravelLocationManagement", "Exception occured on GetTravelLocationDocNum, " + ex.Message);
            }

            return docNumList;
        }

        public List<TravelLocationInfo> GetTask_MasterByDocNum(string docNo)
        {
            List<TravelLocationInfo> Task_MasterList = new List<TravelLocationInfo>();
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
                DataTable dt_Task_Master = HANADAL.GetDataTableByStoredProcedure("GetTravel_Location_ByDocNum", parmList, "TravelLocationManagement");
                if (dt_Task_Master.Rows.Count > 0)
                {

                    Task_MasterList = TranslateDataTableToTravelLocationManagementList(dt_Task_Master);
                }
              
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("TravelLocationManagement", "Exception occured on GetResourceBillingRatesByDocNum DocNum: " + docNo + " , " + ex.Message);
            }

            return Task_MasterList;
        }

        private List<string> TranslateDataTableToTravel_LocationDocNumList(DataTable dt)
        {
            List<string> docNumList = new List<string>();
            foreach (DataRow dtRow in dt.Rows)
            {
                docNumList.Add(Convert.ToString(dtRow["DocNum"]));
            }

            return docNumList.Distinct().ToList();
        }

        #endregion

        #region "Translation TravelLocation"
            

        private List<TravelLocationInfo> TranslateDataTableToTravelLocationManagementList(DataTable dt)
        {
            List<TravelLocationInfo> TravelLocationL = new List<TravelLocationInfo>();

            try
            {
                int sno = 1;
                foreach (DataRow dtRow in dt.Rows)
                {
                    TravelLocationInfo TravelLocation = new TravelLocationInfo();
                    TravelLocation.SNO = sno;
                    TravelLocation.KEY = Guid.NewGuid().ToString();
                    TravelLocation.ID = Convert.ToInt32(dtRow["ID"]);
                    TravelLocation.BRANCHID = Convert.ToInt32(dtRow["BRANCHID"]);
                    TravelLocation.CLIENTNAME = Convert.ToString(dtRow["CLIENTNAME"]);
                    TravelLocation.KM = Convert.ToDouble(dtRow["KM"]);
                    TravelLocation.LOCATION = Convert.ToString(dtRow["LOCATION"]);
                    TravelLocation.DOCNUM= Convert.ToString(dtRow["DOCNUM"]);
                    TravelLocation.CREATEDBY = Convert.ToInt32(dtRow["CREATEDBY"]);
                    TravelLocation.CREATEDATE = Convert.ToDateTime(dtRow["CREATEDATE"]);

                    if (dtRow["UPDATEDBY"] != DBNull.Value)
                        TravelLocation.UPDATEDBY = Convert.ToInt32(dtRow["UPDATEDBY"]);
                    if (dtRow["UPDATEDATE"] != DBNull.Value)
                        TravelLocation.UPDATEDATE = Convert.ToDateTime(dtRow["UPDATEDATE"]);

                    TravelLocation.ISACTIVE = Convert.ToBoolean(dtRow["ISACTIVE"]);
                    TravelLocation.ISDELETED = Convert.ToBoolean(dtRow["ISDELETED"]);
                    sno = sno + 1;
                    TravelLocationL.Add(TravelLocation);
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("TravelLocationManagement", "Exception occured on TranslateDataTableToTravelLocationManagementList, " + ex.Message);
            }

            return TravelLocationL;
        }

        private List<B1SP_Parameter> TranslateTravelLocationToParameterList(TravelLocationInfo TravelLocation)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {

                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = Convert.ToString(TravelLocation.ID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ISACTIVE";
                parm.ParameterValue = Convert.ToString(TravelLocation.ISACTIVE);
                parm.ParameterType = DBTypes.Bool.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ISDELETED";
                parm.ParameterValue = Convert.ToString(TravelLocation.ISDELETED);
                parm.ParameterType = DBTypes.Bool.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CREATEDBY";
                parm.ParameterValue = Convert.ToString(TravelLocation.CREATEDBY);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CREATEDATE";
                parm.ParameterValue = DateTime.Now.ToString("yyyy-MM-dd HH:MM:s");
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "UPDATEDBY";
                parm.ParameterValue = Convert.ToString(TravelLocation.UPDATEDBY);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "UPDATEDATE";
                parm.ParameterValue = DateTime.Now.ToString("yyyy-MM-dd HH:MM:s");
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "BRANCHID";
                parm.ParameterValue = Convert.ToString(TravelLocation.BRANCHID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CLIENTNAME";
                parm.ParameterValue = Convert.ToString(TravelLocation.CLIENTNAME);
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "DOCNUM";
                parm.ParameterValue = Convert.ToString(TravelLocation.DOCNUM);
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "KM";
                parm.ParameterValue = Convert.ToString(TravelLocation.KM);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "LOCATION";
                parm.ParameterValue = Convert.ToString(TravelLocation.LOCATION);
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);
                

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("TravelLocationManagement", "Exception occured on TranslateTravelLocationToParameterList, " + ex.Message);
            }


            return parmList;
        }

        private List<B1SP_Parameter> TranslateMaster_TaskLogToParameterList(TravelLocationInfo TravelLocation)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {

                B1SP_Parameter parm = new B1SP_Parameter();

                parm = new B1SP_Parameter();
                parm.ParameterName = "KM_PREVIOUS";
                parm.ParameterValue = Convert.ToString(TravelLocation.KM);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "KM_NEW";
                parm.ParameterValue = Convert.ToString(TravelLocation.KM);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "LOCATION_PREVIOUS";
                parm.ParameterValue = Convert.ToString(TravelLocation.LOCATION);
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "LOCATION";
                parm.ParameterValue = Convert.ToString(TravelLocation.LOCATION);
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ISACTIVE_PREVIOUS";
                parm.ParameterValue = Convert.ToString(TravelLocation.ISACTIVE);
                parm.ParameterType = DBTypes.Bool.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ISACTIVE_NEW";
                parm.ParameterValue = Convert.ToString(TravelLocation.ISACTIVE);
                parm.ParameterType = DBTypes.Bool.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ISDELETED_PREVIOUS";
                parm.ParameterValue = Convert.ToString(TravelLocation.ISDELETED);
                parm.ParameterType = DBTypes.Bool.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ISDELETED_NEW";
                parm.ParameterValue = Convert.ToString(TravelLocation.ISDELETED);
                parm.ParameterType = DBTypes.Bool.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CREATEDBY";
                parm.ParameterValue = Convert.ToString(TravelLocation.CREATEDBY);
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
                log.InputOutputDocLog("TravelLocationManagement", "Exception occured on TranslateMaster_TaskLogToParameterList, " + ex.Message);
            }


            return parmList;
        }

        #endregion
        public DataTable GetTravel_LocationAllDocumentsList()
        {
            DataTable dt = new DataTable();
            Common cmn = new Common();
            try

            {
                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                dt = HANADAL.GetDataTableByStoredProcedure("GetTravel_Location_AllDocuments", "TravelLocationManagement");
                dt = cmn.RemoveDuplicateRows(dt, "DOCNUM");
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("TravelLocationManagement", "Exception occured on GetTravel_LocationAllDocumentsList, " + ex.Message);
            }

            return dt;
        }
    }
}