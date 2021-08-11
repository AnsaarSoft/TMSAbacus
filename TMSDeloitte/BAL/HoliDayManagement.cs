using System;
using System.Collections;
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
    public class HoliDayManagement
    {
        public Holiday GetHolidayByID(int id)
        {
            Holiday Holiday = new Holiday();
            try
            {
                List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = Convert.ToString(id);
                parmList.Add(parm);

                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                Common cmn = new Common();
                DataSet ds = HANADAL.GetDataSetByStoredProcedure("GetHolidayByID", parmList, "HolidayManagement");
                if(ds.Tables.Count>0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        Holiday = TranslateDataTableToHoliday(ds.Tables[0]);
                        Holiday.Detail = TranslateDataTableToHolidayDetails(ds.Tables[1]);
                    }
                }
               
                
                
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("HolidayManagement", "Exception occured on GetHolidayByID ID: " + id + " , " + ex.Message);
            }

            return Holiday;
        }

        public Holiday GetHolidayByDocNum(string docNo)
        {
           Holiday Holiday = new Holiday();
            try
            {

                List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();

                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "DocNum";
                parm.ParameterValue = docNo;
                parmList.Add(parm);

                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                Common cmn = new Common();
                DataSet ds = HANADAL.GetDataSetByStoredProcedure("GetHolidayByDocNum", parmList, "HolidayManagement");
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        Holiday = TranslateDataTableToHoliday(ds.Tables[0]);
                        Holiday.Detail = TranslateDataTableToHolidayDetails(ds.Tables[1]);
                    }
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("HolidayManagement", "Exception occured on GetHolidayByDocNum DocNum: " + docNo + " , " + ex.Message);
            }

            return Holiday;
        }

    
        public bool ValidateDateRange(int year,DateTime fromDate,DateTime toDate)
        {
            bool isSuccess = false;

            try
            {
                List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();

                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "Year";
                parm.ParameterValue = Convert.ToString(year);
                parmList.Add(parm);
                parm = new B1SP_Parameter();
                parm.ParameterName = "FromDate";
                parm.ParameterValue = Convert.ToString(fromDate.ToString("yyyy-MM-dd"));
                parmList.Add(parm);
                parm = new B1SP_Parameter();
                parm.ParameterName = "ToDate";
                parm.ParameterValue = Convert.ToString(toDate.ToString("yyyy-MM-dd"));
                parmList.Add(parm);

                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                DataTable dt_UserProfile = HANADAL.GetDataTableByStoredProcedure("ValidateHolidayDateRange", parmList, "HolidayManagement");
                if (dt_UserProfile.Rows.Count > 0)
                {
                    isSuccess = true;
                }

            }
            catch (Exception ex)
            {
                isSuccess = false;
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("HolidayManagement", "Exception occured on ValidateDateRange, " + ex.Message);
            }
            return isSuccess;
        }

        public bool AddUpdateHolidaySetup(Holiday Holiday,out string msg)
        {
            bool isSuccess = false;
            bool isAddOccured = false;
            bool isUpdateOccured = false;
            bool isDeleteOccured = false;

            Common cmn = new Common();
            string docNum = "";
            int docId = 0;
            msg = "Successfully Added/Updated";
            try
            {
                if(Holiday.Detail!=null)
                {
                    if(Holiday.Detail.Count>0)
                    {
                        var filtererdList = Holiday.Detail.Where(x => x.IsDeleted == false).ToList();
                        var duplicates = filtererdList.GroupBy(x => x._Holidate).Where(g => g.Count() > 1).Select(y => y.Key).ToList();
                        if (duplicates.Count > 0)
                        {
                            msg = "Duplicate record exist!";
                            return false;
                        }
                    }
                }

               
                

                var value = Holiday.DocNum;
                if (!string.IsNullOrEmpty(value))
                {
                    docNum = value;
                    docId =Convert.ToInt32(Holiday.ID);
                    Holiday  oldObj= GetHolidayByID(docId);
                    if( oldObj.FromDate.ToString("yyyy-MM-dd")!= Holiday.FromDate.ToString("yyyy-MM-dd") ||
                        oldObj.ToDate.ToString("yyyy-MM-dd") != Holiday.ToDate.ToString("yyyy-MM-dd")
                        )
                    {
                        if(ValidateDateRange(Holiday.Year, Holiday.FromDate, Holiday.ToDate))
                        {
                            msg = "Record exist in this date range!";
                            return false;
                        }
                    }
                }
                else
                {
                    if (ValidateDateRange(Holiday.Year, Holiday.FromDate, Holiday.ToDate))
                    {
                        msg = "Record exist in this date range!";
                        return false;
                    }

                    int no = 1;
                    List<string> docNumList = cmn.GetDocNum("GetHolidayDocNum", "HolidayManagement");
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



                Holiday.DocNum = docNum;
                Holiday.Detail= Holiday.Detail.Where(x => x.ID > 0 || x.IsDeleted == false).ToList();

                ////For Form Log
                // AddHolidaySetup_Log(HolidayList, out isUpdateOccured);
                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                if (docId==0)
                {
                    DataTable dtHeader = HANADAL.AddUpdateDataByStoredProcedure("AddUpdateHoliday", TranslateHolidaySetupToParameterList(Holiday), "HolidayManagement");
                    if (dtHeader.Rows.Count == 0)
                        throw new Exception("Exception occured when AddUpdateHolidaySetup , ID:" + Holiday.ID + " , Year:" + Holiday.Year);
                    else
                    {
                        foreach (DataRow dtRow in dtHeader.Rows)
                        {
                            isAddOccured = true;
                            docId = Convert.ToInt32(dtRow["ID"]);
                            docNum = Convert.ToString(dtRow["DocNum"]);
                        }
                       
                    }
                }
                else
                {
                    AddHoliday_Log(Holiday,out isUpdateOccured);
                    
                }

                Holiday.Detail.Select(c => { c.HeaderID = docId; return c; }).ToList();
                AddHolidayDetails_Log(Holiday.Detail, out isUpdateOccured);

                foreach (var list in Holiday.Detail)
                {
                    try
                    {
                        HANADAL.AddUpdateDataByStoredProcedure("AddUpdateHoliDayDetail", TranslateHolidayDetailsToParameterList(list), "HolidayManagement");
                        //DataTable dtRow = HANADAL.AddUpdateDataByStoredProcedure("AddUpdateHolidayDetails", TranslateHolidayDetailsToParameterList(list), "HolidayManagement");
                        //if (dtRow.Rows.Count == 0)
                        //    throw new Exception("Exception occured when AddUpdateHolidayRow , ID:" + list.ID + " , HeaderID:" + list.HeaderID );
                    }
                    catch (Exception ex)
                    {
                        isSuccess = false;
                        Log log = new Log();
                        log.LogFile(ex.Message);
                        log.InputOutputDocLog("HolidayManagement", "Exception occured in foreach loop AddUpdateAssignmentCostSetup, " + ex.Message);
                        continue;
                    }
                }
                
                //For Master Log
                if (Holiday.ID > 0)
                    isAddOccured = true;
                if (Convert.ToBoolean(Holiday.ISDELETED))
                    isDeleteOccured = true;

                int createdBy =Convert.ToInt32(Holiday.CREATEDBY);
                Task.Run(() => cmn.AddMasterLog(isAddOccured, isUpdateOccured, isDeleteOccured, nameof(Enums.FormsName.HoliDaySetup), createdBy, "HolidayManagement"));
                //End MAster Log


                isSuccess = true;
            }
            catch (Exception ex)
            {
                isSuccess = false;
                msg = "Exception occured on Add/Update";
                isSuccess = false;
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("HolidayManagement", "Exception occured on AddUpdateAssignmentCostSetup, " + ex.Message);
            }

            return isSuccess;
        }

        public void AddHoliday_Log(Holiday HolidayList, out bool isUpdateOccured)
        {
            isUpdateOccured = false;
            try
            {
                if (HolidayList.ID == 0)
                    return;

                Holiday previousObject = GetHolidayByID(Convert.ToInt32(HolidayList.ID));
                List<B1SP_Parameter> paramList = TranslateHolidayLogToParameterList(HolidayList);
                bool isChangeOccured = false;
                if (previousObject !=null)
                {
                    foreach (PropertyCompareResult resultItem in PropertyCompare.Compare(previousObject, HolidayList))
                    {
                        isChangeOccured = false;
                        isUpdateOccured = false;
                        string Name = resultItem.Name;
                        object PreviousValue = resultItem.OldValue;
                        object NewValue = resultItem.NewValue;

                        switch (Name)
                        {
                            case "Year":
                                paramList.Where(x => x.ParameterName == "Year_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                paramList.Where(x => x.ParameterName == "Year_new").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();
                                isChangeOccured = true;
                                break;
                            case "FromDate":
                                paramList.Where(x => x.ParameterName == "FromDate_Previous").Select(c => { c.ParameterValue = Convert.ToDateTime(PreviousValue).ToString("yyyy-MM-dd"); return c; }).ToList();
                                paramList.Where(x => x.ParameterName == "FromDate_new").Select(c => { c.ParameterValue = Convert.ToDateTime(NewValue).ToString("yyyy-MM-dd"); return c; }).ToList();
                                isChangeOccured = true;
                                break;
                            case "ToDate":
                                paramList.Where(x => x.ParameterName == "ToDate_Previous").Select(c => { c.ParameterValue = Convert.ToDateTime(PreviousValue).ToString("yyyy-MM-dd"); return c; }).ToList();
                                paramList.Where(x => x.ParameterName == "ToDate_new").Select(c => { c.ParameterValue = Convert.ToDateTime(NewValue).ToString("yyyy-MM-dd"); return c; }).ToList();
                                isChangeOccured = true;
                                break;
                            case "ISDELETED":
                                paramList.Where(x => x.ParameterName == "IsDeleted_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                paramList.Where(x => x.ParameterName == "IsDeleted_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();
                                isChangeOccured = true;
                                break;
                        }

                    }

                    if (isChangeOccured)
                    {
                        isUpdateOccured = true;
                        HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                        Task.Run(() => HANADAL.AddUpdateDataByStoredProcedure("AddUpdateHolidayLog", paramList, "HolidayManagement"));
                    }

                }
            
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("HolidayManagement", "Exception occured on AddAssignmentCostSetup_Log, " + ex.Message);
            }
        }

        public void AddHolidayDetails_Log(List<HolidayDetails> HolidayList, out bool isUpdateOccured)
        {
            isUpdateOccured = false;
            try
            {
                HolidayList = HolidayList.Where(x => x.ID > 0).ToList();

                foreach (var newObject in HolidayList)
                {

                    try
                    {
                        Holiday previousObject = GetHolidayByID(Convert.ToInt32(HolidayList[0].HeaderID));
                        List<B1SP_Parameter> paramList = TranslateHolidayDetailsLogToParameterList(newObject);
                        //List<B1SP_Parameter> paramList = new List<B1SP_Parameter>();
                        //B1SP_Parameter parm = new B1SP_Parameter();
                        bool isChangeOccured = false;

                        var val = previousObject.Detail.Where(x => x.ID == newObject.ID).FirstOrDefault();
                        if (val != null)
                        {
                            foreach (PropertyCompareResult resultItem in PropertyCompare.Compare(val, newObject))
                            {
                                isChangeOccured = false;
                                isUpdateOccured = false;
                                string Name = resultItem.Name;
                                object PreviousValue = resultItem.OldValue;
                                object NewValue = resultItem.NewValue;

                                switch (Name)
                                {
                                    case "Description":
                                        paramList.Where(x => x.ParameterName == "Description_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                        paramList.Where(x => x.ParameterName == "Description_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();
                                        isChangeOccured = true;
                                        break;
                                   
                                    case "IsDeleted":
                                        paramList.Where(x => x.ParameterName == "ISDELETED_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                        paramList.Where(x => x.ParameterName == "ISDELETED_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();
                                        isChangeOccured = true;
                                        break;
                                }

                            }
                        }


                        if (isChangeOccured)
                        {
                            isUpdateOccured = true;
                            HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                            Task.Run(() => HANADAL.AddUpdateDataByStoredProcedure("AddUpdateHoliDayDetailLog", paramList, "HolidayManagement"));
                        }


                    }
                    catch (Exception ex)
                    {
                        Log log = new Log();
                        log.LogFile(ex.Message);
                        log.InputOutputDocLog("HolidayManagement", "Exception occured in foreach loop AddHolidaySetup_Log, " + ex.Message);
                        continue;
                    }


                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("HolidayManagement", "Exception occured on AddAssignmentCostSetup_Log, " + ex.Message);
            }
        }


        public List<string> GetHolidays(int year, string fromDate, string toDate)
        {
            List<string> dateList = new List<string>();
            try
            {
                List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();

                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "Year";
                parm.ParameterValue = Convert.ToString(year);
                parmList.Add(parm);
                parm = new B1SP_Parameter();
                parm.ParameterName = "FromDate";
                parm.ParameterValue = Convert.ToString(fromDate);
                parmList.Add(parm);
                parm = new B1SP_Parameter();
                parm.ParameterName = "ToDate";
                parm.ParameterValue = Convert.ToString(toDate);
                parmList.Add(parm);

                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                DataTable dt = HANADAL.GetDataTableByStoredProcedure("ValidateHolidayDateRange", parmList, "HolidayManagement");
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dtRow in dt.Rows)
                    {
                        Holiday holiday = GetHolidayByID(Convert.ToInt32(dtRow["ID"]));
                        if(holiday!=null)
                        {
                            if(holiday.Detail!=null)
                            {
                                foreach (var item in holiday.Detail)
                                {
                                    dateList.Add(item.Holidate.ToString("yyyy-MM-dd"));
                                }
                            }
                           
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("HolidayManagement", "Exception occured on GetHolidays, " + ex.Message);
            }
            return dateList;
        }

        public DataSet GetHolidayLogByID(string id)
        {
            DataSet ds = new DataSet();
            try

            {
                List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();

                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = id;
                parmList.Add(parm);

                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                ds = HANADAL.GetDataSetByStoredProcedure("GetHolidayLogByID", parmList, "HolidayManagement");

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("HolidayManagement", "Exception occured on GetAllAssignmentCostSetup, " + ex.Message);
            }

            return ds;
        }
        public DataTable GetHolidayAllDocumentsList()
        {
            DataTable dt = new DataTable();
            Common cmn = new Common();
            try

            {
                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                dt = HANADAL.GetDataTableByStoredProcedure("GetHolidayAllDocuments", "HolidayManagement");
                dt = cmn.RemoveDuplicateRows(dt, "DocNum");
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("HolidayManagement", "Exception occured on GetAllAssignmentCostSetup, " + ex.Message);
            }

            return dt;
        }
        
        #region Translation HolidaySetup"

        private Holiday TranslateDataTableToHoliday(DataTable dt)
        {
            Holiday Holiday = new Holiday();

            try
            {
                foreach (DataRow dtRow in dt.Rows)
                {
                   
                    Holiday.ID = Convert.ToInt32(dtRow["ID"]);
                    Holiday.DocNum = Convert.ToString(dtRow["DocNum"]);
                    DateTime fdate = Convert.ToDateTime(dtRow["FromDate"]);
                    Holiday.FromDate = Convert.ToDateTime(fdate.Date);
                    Holiday.Year = Convert.ToInt32(dtRow["Year"]);

                    DateTime tdate = Convert.ToDateTime(dtRow["ToDate"]);
                    Holiday.ToDate = Convert.ToDateTime(tdate.Date);

                    Holiday.CREATEDBY = Convert.ToInt32(dtRow["CREATEDBY"]);
                    Holiday.CREATEDDATE = Convert.ToDateTime(dtRow["CREATEDDATE"]);

                    if (dtRow["UPDATEDEDBY"] != DBNull.Value)
                        Holiday.UPDATEDEDBY = Convert.ToInt32(dtRow["UPDATEDEDBY"]);
                    if (dtRow["UPDATEDDATE"] != DBNull.Value)
                        Holiday.UPDATEDDATE = Convert.ToDateTime(dtRow["UPDATEDDATE"]);
                    
                    Holiday.ISDELETED = Convert.ToBoolean(dtRow["ISDELETED"]);
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("HolidayManagement", "Exception occured on TranslateDataTableToHoliday, " + ex.Message);
            }

            return Holiday;
        }

        private List<HolidayDetails> TranslateDataTableToHolidayDetails(DataTable dt)
        {
            List<HolidayDetails> HolidayList = new List<HolidayDetails>();

            try
            {
                int sno = 1;
                foreach (DataRow dtRow in dt.Rows)
                {
                    HolidayDetails Holiday = new HolidayDetails();
                    Holiday.SNO = sno;
                    Holiday.KEY = Guid.NewGuid().ToString();
                    Holiday.ID = Convert.ToInt32(dtRow["ID"]);
                    Holiday.Holidate = Convert.ToDateTime(dtRow["Holidate"]);
                    Holiday._Holidate = Convert.ToDateTime(dtRow["Holidate"]).ToString("yyyy-MM-dd"); 
                    Holiday.HeaderID = Convert.ToInt32(dtRow["HeaderID"]);
                    Holiday.Description = Convert.ToString(dtRow["Description"]);
                    Holiday.IsDeleted = Convert.ToBoolean(dtRow["IsDeleted"]);
                    sno = sno + 1;
                    HolidayList.Add(Holiday);
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("HolidayManagement", "Exception occured on TranslateDataTableToHolidayDetails, " + ex.Message);
            }

            return HolidayList;
        }
        private List<B1SP_Parameter> TranslateHolidaySetupToParameterList(Holiday Holiday)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {

                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = Convert.ToString(Holiday.ID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "DocNum";
                parm.ParameterValue = Convert.ToString(Holiday.DocNum);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "FuncationID";
                parm.ParameterValue = Convert.ToString(Holiday.Year);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "FromDate";
                parm.ParameterValue = Convert.ToString(Holiday.FromDate.ToString("yyyy-MM-dd"));
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ToDate";
                parm.ParameterValue = Convert.ToString(Holiday.ToDate.ToString("yyyy-MM-dd"));
                parmList.Add(parm);
                

                parm = new B1SP_Parameter();
                parm.ParameterName = "ISDELETED";
                parm.ParameterValue = Convert.ToString(Holiday.ISDELETED);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CREATEDBY";
                parm.ParameterValue = Convert.ToString(Holiday.CREATEDBY);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CREATEDDATE";
                parm.ParameterValue = DateTime.Now.ToString("yyyy-MM-dd");
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "UPDATEDEDBY";
                parm.ParameterValue = Convert.ToString(Holiday.UPDATEDEDBY);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "UPDATEDDATE";
                parm.ParameterValue = DateTime.Now.ToString("yyyy-MM-dd");
                parmList.Add(parm);


            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("HolidayManagement", "Exception occured on TranslateHolidaySetupToParameterList, " + ex.Message);
            }


            return parmList;
        }
        private List<B1SP_Parameter> TranslateHolidayDetailsToParameterList(HolidayDetails Holiday)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {

                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = Convert.ToString(Holiday.ID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "HeaderID";
                parm.ParameterValue = Convert.ToString(Holiday.HeaderID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "Holidate";
                parm.ParameterValue = Convert.ToString(Holiday.Holidate.ToString("yyyy-MM-dd"));
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "Description";
                parm.ParameterValue = Convert.ToString(Holiday.Description);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "IsDeleted";
                parm.ParameterValue = Convert.ToString(Holiday.IsDeleted);
                parmList.Add(parm);



            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("HolidayManagement", "Exception occured on TranslateHolidayDetailsToParameterList, " + ex.Message);
            }


            return parmList;
        }

        private List<B1SP_Parameter> TranslateHolidayLogToParameterList(Holiday Holiday)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {

                B1SP_Parameter parm = new B1SP_Parameter();
                parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = Convert.ToString(0);
                parmList.Add(parm);
                
                parm = new B1SP_Parameter();
                parm.ParameterName = "DocID";
                parm.ParameterValue = Convert.ToString(Holiday.ID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "DocNum";
                parm.ParameterValue = Convert.ToString(Holiday.DocNum);
                parmList.Add(parm);

                

                parm = new B1SP_Parameter();
                parm.ParameterName = "FromDate_Previous";
                parm.ParameterValue = Convert.ToString(Holiday.FromDate.ToString("yyyy-MM-dd"));
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "Year_Previous";
                parm.ParameterValue = Convert.ToString(Holiday.Year);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ToDate_Previous";
                parm.ParameterValue = Convert.ToString(Holiday.ToDate.ToString("yyyy-MM-dd"));
                parmList.Add(parm);
                

                parm = new B1SP_Parameter();
                parm.ParameterName = "IsDeleted_Previous";
                parm.ParameterValue = Convert.ToString(Holiday.ISDELETED);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "FromDate_New";
                parm.ParameterValue = Convert.ToString(Holiday.FromDate.ToString("yyyy-MM-dd"));
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "Year_New";
                parm.ParameterValue = Convert.ToString(Holiday.Year);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ToDate_New";
                parm.ParameterValue = Convert.ToString(Holiday.ToDate.ToString("yyyy-MM-dd"));
                parmList.Add(parm);


                parm = new B1SP_Parameter();
                parm.ParameterName = "IsDeleted_New";
                parm.ParameterValue = Convert.ToString(Holiday.ISDELETED);
                parmList.Add(parm);

                
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("HolidayManagement", "Exception occured on TranslateHolidayDetailsLogToParameterList, " + ex.Message);
            }


            return parmList;
        }
        private List<B1SP_Parameter> TranslateHolidayDetailsLogToParameterList(HolidayDetails Holiday)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {

                B1SP_Parameter parm = new B1SP_Parameter();

                parm = new B1SP_Parameter();
                parm.ParameterName = "DocID";
                parm.ParameterValue = Convert.ToString(Holiday.HeaderID);
                parmList.Add(parm);

              
                parm = new B1SP_Parameter();
                parm.ParameterName = "HoliDate_Previous";
                parm.ParameterValue = Convert.ToString(Holiday.Holidate.ToString("yyyy-MM-dd"));
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "Description_Previous";
                parm.ParameterValue = Convert.ToString(Holiday.Description);
                parmList.Add(parm);


                parm = new B1SP_Parameter();
                parm.ParameterName = "IsDeleted_Previous";
                parm.ParameterValue = Convert.ToString(Holiday.IsDeleted);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "HoliDate_New";
                parm.ParameterValue = Convert.ToString(Holiday.Holidate.ToString("yyyy-MM-dd"));
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "Description_New";
                parm.ParameterValue = Convert.ToString(Holiday.Description);
                parmList.Add(parm);


                parm = new B1SP_Parameter();
                parm.ParameterName = "IsDeleted_New";
                parm.ParameterValue = Convert.ToString(Holiday.IsDeleted);
                parmList.Add(parm);

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("HolidayManagement", "Exception occured on TranslateHolidaySetupLogToParameterList, " + ex.Message);
            }


            return parmList;
        }
        #endregion
    }
}