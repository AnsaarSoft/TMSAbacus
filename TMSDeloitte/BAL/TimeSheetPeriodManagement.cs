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
    public class TimeSheetPeriodManagement
    {

        public GenerateTimeSheet GetTimeSheetPeriodsByID(int id)
        {
            GenerateTimeSheet TimeSheetPeriods = new GenerateTimeSheet();
            try
            {
                List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = Convert.ToString(id);
                parmList.Add(parm);

                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                Common cmn = new Common();
                DataSet ds = HANADAL.GetDataSetByStoredProcedure("GetTimeSheetPeriodsByID", parmList, "TimeSheetPeriodsManagement");
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        TimeSheetPeriods = TranslateDataTableToTimeSheetPeriods(ds.Tables[0]);
                        TimeSheetPeriods.PeriodList = TranslateDataTableToTimeSheetPeriodsDetail(ds.Tables[1]);
                    }
                }



            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("TimeSheetPeriodsManagement", "Exception occured on GetTimeSheetPeriodsByID ID: " + id + " , " + ex.Message);
            }

            return TimeSheetPeriods;
        }

        public TimeSheetPeriods GetTimeSheetPeriodDetailByID(int id)
        {
            TimeSheetPeriods TimeSheetPeriods = new TimeSheetPeriods();
            try
            {
                List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = Convert.ToString(id);
                parmList.Add(parm);

                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                Common cmn = new Common();
                DataTable ds = HANADAL.GetDataTableByStoredProcedure("GetTimeSheetPeriodsDetailByID", parmList, "TimeSheetPeriodsManagement");
                if (ds.Rows.Count > 0)
                {
                    TimeSheetPeriods = TranslateDataTableToTimeSheetDetail(ds);
                }



            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("TimeSheetPeriodsManagement", "Exception occured on GetTimeSheetPeriodsByID ID: " + id + " , " + ex.Message);
            }

            return TimeSheetPeriods;
        }
        public bool AddUpdateTimeSheetPeriodsSetup(GenerateTimeSheet TimeSheetPeriods, out string msg)
        {
            bool isSuccess = false;
            bool isAddOccured = false;
            bool isUpdateOccured = false;
            bool isDeleteOccured = false;

            Common cmn = new Common();
            string docNum = "";
            int docId = 0;
            msg = "Successfully Added/Updated";
            bool isUpdate = false;
            try
            {
                var value = TimeSheetPeriods.DocNum;
                if (!string.IsNullOrEmpty(value))
                {
                    isUpdate = true;
                    docNum = value;
                    docId = Convert.ToInt32(TimeSheetPeriods.ID);
                    GenerateTimeSheet oldObj = GetTimeSheetPeriodsByID(docId);
                    if (oldObj.fromDate.ToString("yyyy-MM-dd") != TimeSheetPeriods.fromDate.ToString("yyyy-MM-dd") ||
                        oldObj.toDate.ToString("yyyy-MM-dd") != TimeSheetPeriods.toDate.ToString("yyyy-MM-dd")
                        )
                    {
                        if (ValidateDateRange(TimeSheetPeriods.year, TimeSheetPeriods.fromDate, TimeSheetPeriods.toDate))
                        {
                            msg = "Record exist in this date range!";
                            return false;
                        }
                        //if (ValidatPeriodCanUsed(TimeSheetPeriods.year, TimeSheetPeriods.fromDate, TimeSheetPeriods.toDate))
                        //{
                        //    msg = "Not allowed to update time sheet period has been used!";
                        //    return false;
                        //}
                    }
                }
                else
                {
                    if (ValidateDateRange(TimeSheetPeriods.year, TimeSheetPeriods.fromDate, TimeSheetPeriods.toDate))
                    {
                        msg = "Record exist in this date range!";
                        return false;
                    }

                    int no = 1;
                    List<string> docNumList = cmn.GetDocNum("GetTimeSheetPeriodsDocNum", "TimeSheetPeriodsManagement");
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



                TimeSheetPeriods.DocNum = docNum;
                TimeSheetPeriods.PeriodList = TimeSheetPeriods.PeriodList.Where(x => x.IsDeleted == false).ToList();

                ////For Form Log
                // AddTimeSheetPeriodsSetup_Log(TimeSheetPeriodsList, out isUpdateOccured);
                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                DataTable dtHeader = HANADAL.AddUpdateDataByStoredProcedure("AddUpdateTimeSheetPeriod", TranslateTimeSheetPeriodsSetupToParameterList(TimeSheetPeriods), "TimeSheetPeriodsManagement");
                if (dtHeader.Rows.Count == 0)
                    throw new Exception("Exception occured when AddUpdateTimeSheetPeriodsSetup , ID:" + TimeSheetPeriods.ID + " , Year:" + TimeSheetPeriods.year);
                else
                {
                    foreach (DataRow dtRow in dtHeader.Rows)
                    {
                        isAddOccured = true;
                        docId = Convert.ToInt32(dtRow["ID"]);
                        docNum = Convert.ToString(dtRow["DocNum"]);
                    }

                }
                //if (docId > 0)
                //{
                //    DeleteTimeSheetPeriodsDetailByHeaderID(docId); 
                //}

                TimeSheetPeriods.PeriodList.Select(c => { c.HeaderID = docId; return c; }).ToList();
                //AddTimeSheetPeriodsDetail_Log(TimeSheetPeriods.Detail, out isUpdateOccured);

                string spName = "AddTimeSheetPeriodDetail";
                if (isUpdate)
                    spName = "UpdateTimeSheetPeriodDetail";

                foreach (var list in TimeSheetPeriods.PeriodList)
                {
                    try
                    {
                        HANADAL.AddUpdateDataByStoredProcedure(spName, TranslateTimeSheetPeriodsDetailToParameterList(list, isUpdate), "TimeSheetPeriodsManagement");
                       
                    }
                    catch (Exception ex)
                    {
                        isSuccess = false;
                        Log log = new Log();
                        log.LogFile(ex.Message);
                        log.InputOutputDocLog("TimeSheetPeriodsManagement", "Exception occured in foreach loop AddUpdateAssignmentCostSetup, " + ex.Message);
                        continue;
                    }
                }

                //For Master Log
                if (TimeSheetPeriods.ID > 0)
                    isAddOccured = true;
                if (Convert.ToBoolean(TimeSheetPeriods.ISDELETED))
                    isDeleteOccured = true;

                int createdBy = Convert.ToInt32(TimeSheetPeriods.CREATEDBY);
                Task.Run(() => cmn.AddMasterLog(isAddOccured, isUpdateOccured, isDeleteOccured, nameof(Enums.FormsName.TimeSheetPeriodSetup), createdBy, "TimeSheetPeriodsManagement"));
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
                log.InputOutputDocLog("TimeSheetPeriodsManagement", "Exception occured on AddUpdateAssignmentCostSetup, " + ex.Message);
            }

            return isSuccess;
        }
        public bool ValidateDateRange(string year, DateTime fromDate, DateTime toDate)
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
                parm.ParameterName = "fromDate";
                parm.ParameterValue = Convert.ToString(fromDate.ToString("yyyy-MM-dd"));
                parmList.Add(parm);
                parm = new B1SP_Parameter();
                parm.ParameterName = "toDate";
                parm.ParameterValue = Convert.ToString(toDate.ToString("yyyy-MM-dd"));
                parmList.Add(parm);

                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                DataTable dt_UserProfile = HANADAL.GetDataTableByStoredProcedure("ValidateTimeSheetPeriodDateRange", parmList, "TimeSheetPeriodsManagement");
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
                log.InputOutputDocLog("TimeSheetPeriodsManagement", "Exception occured on ValidateDateRange, " + ex.Message);
            }
            return isSuccess;
        }

        public bool ValidatPeriodCanUsed(string year, DateTime fromDate, DateTime toDate)
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
                DataTable dt_UserProfile = HANADAL.GetDataTableByStoredProcedure("ValidateTimeSheetPeriodCanUsed", parmList, "TimeSheetPeriodsManagement");
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
                log.InputOutputDocLog("TimeSheetPeriodsManagement", "Exception occured on ValidatPeriodCanUsed, " + ex.Message);
            }
            return isSuccess;
        }

        public GenerateTimeSheet DeleteTimeSheetPeriodsDetailByHeaderID(int HeaderID)
        {
            GenerateTimeSheet TimeSheetPeriods = new GenerateTimeSheet();
            try
            {
                List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "HeaderID";
                parm.ParameterValue = Convert.ToString(HeaderID);
                parmList.Add(parm);

                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                Common cmn = new Common();
                HANADAL.AddUpdateDataByStoredProcedure("DeleteTimeSheetPeriodDetail", parmList, "TimeSheetPeriodsManagement");
                //DataSet ds = HANADAL.GetDataSetByStoredProcedure("DeleteTimeSheetPeriodDetail", parmList, "TimeSheetPeriodsManagement");
                //if (ds.Tables.Count > 0)
                //{
                //    if (ds.Tables[0].Rows.Count > 0)
                //    {
                //        TimeSheetPeriods = TranslateDataTableToTimeSheetPeriods(ds.Tables[0]);
                //        TimeSheetPeriods.PeriodList = TranslateDataTableToTimeSheetPeriodsDetail(ds.Tables[1]);
                //    }
                //}



            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("TimeSheetPeriodsManagement", "Exception occured on DeleteTimeSheetPeriodsDetailByHeaderID, ID: " + HeaderID + " , Exception: " + ex.Message);
            }

            return TimeSheetPeriods;
        }

        public DataTable GetTimeSheetAllDocumentsList()
        {
            DataTable dt = new DataTable();
            Common cmn = new Common();
            try

            {
                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                dt = HANADAL.GetDataTableByStoredProcedure("GetTimeSheetAllDocuments", "TimeSheetPeriodsManagement");
                dt = cmn.RemoveDuplicateRows(dt, "DocNum");
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("TimeSheetPeriodsManagement", "Exception occured on GetTimeSheetAllDocumentsList, " + ex.Message);
            }

            return dt;
        }

        public GenerateTimeSheet GetTimeSheetByDocNum(string docNo)
        {
            GenerateTimeSheet obj = new GenerateTimeSheet();
            try
            {

                List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();

                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "DocNum";
                parm.ParameterValue = docNo;
                parmList.Add(parm);

                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                Common cmn = new Common();
                DataSet ds = HANADAL.GetDataSetByStoredProcedure("GetTimeSheetByDocNum", parmList, "TimeSheetPeriodsManagement");
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        obj = TranslateDataTableToTimeSheetPeriods(ds.Tables[0]);
                        obj.PeriodList = TranslateDataTableToTimeSheetPeriodsDetail(ds.Tables[1]);
                    }
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("TimeSheetPeriodsManagement", "Exception occured on GetResourceBillingRatesByDocNum DocNum: " + docNo + " , " + ex.Message);
            }

            return obj;
        }

        public GenerateTimeSheet GetTimeSheetByEmpIDandYear(int empID,string year,bool isView=false)
        {
            GenerateTimeSheet obj = new GenerateTimeSheet();
            try
            {

                List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();

                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "Year";
                parm.ParameterValue = year;
                parmList.Add(parm);

                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                Common cmn = new Common();
                DataSet ds = HANADAL.GetDataSetByStoredProcedure("GetTimeSheetPeriodByPeriod", parmList, "TimeSheetPeriodsManagement");
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        obj = TranslateDataTableToTimeSheetPeriods(ds.Tables[0]);
                        obj.PeriodList = TranslateDataTableToTimeSheetPeriodsDetail(ds.Tables[1]);
                        if(isView==false)
                        {
                            TimeSheetFormManagement objFrm = new TimeSheetFormManagement();
                            List<TimeSheetForm> list = objFrm.GetSubmittedTimeSheetFormDetailsByEmpID(empID);
                            foreach (var item in list)
                            {
                                obj.PeriodList.RemoveAll(x => x.ID == item.Period);
                            }

                        }
                       

                    }
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("TimeSheetPeriodsManagement", "Exception occured on GetTimeSheetByYear year: " + year + " , " + ex.Message);
            }

            return obj;
        }

        public List<string> GetUtilitizedYear()
        {
            List<string> docNumList = new List<string>();
            try
            {
                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();

                DataTable dt = HANADAL.GetDataTableByStoredProcedure("GetTimeSheetYearUtilized", "TimeSheetPeriodsManagement");
                if (dt.Rows.Count > 0)
                {
                    docNumList = TranslateDataTableToYearList(dt);
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("TimeSheetPeriodsManagement", "Exception occured on GetResourceBillingRatesDocNum, " + ex.Message);
            }

            return docNumList;
        }


        public List<string> TranslateDataTableToYearList(DataTable dt)
        {
            List<string> docNumList = new List<string>();
            foreach (DataRow dtRow in dt.Rows)
            {
                docNumList.Add(Convert.ToString(dtRow["Year"]));
            }

            return docNumList.Distinct().ToList();
        }

        private GenerateTimeSheet TranslateDataTableToTimeSheetPeriods(DataTable dt)
        {
            GenerateTimeSheet TimeSheetPeriods = new GenerateTimeSheet();

            try
            {
                foreach (DataRow dtRow in dt.Rows)
                {

                    TimeSheetPeriods.ID = Convert.ToInt32(dtRow["ID"]);
                    TimeSheetPeriods.DocNum = Convert.ToString(dtRow["DocNum"]);
                    DateTime fdate = Convert.ToDateTime(dtRow["FromDate"]);
                    TimeSheetPeriods.fromDate = Convert.ToDateTime(fdate.Date);
                    TimeSheetPeriods.year = Convert.ToString(dtRow["year"]);

                    DateTime tdate = Convert.ToDateTime(dtRow["ToDate"]);
                    TimeSheetPeriods.toDate = Convert.ToDateTime(tdate.Date);

                    TimeSheetPeriods.CREATEDBY = Convert.ToInt32(dtRow["CREATEDBY"]);
                    TimeSheetPeriods.CREATEDDATE = Convert.ToDateTime(dtRow["CREATEDDATE"]);

                    if (dtRow["UPDATEDEDBY"] != DBNull.Value)
                        TimeSheetPeriods.UPDATEDEDBY = Convert.ToInt32(dtRow["UPDATEDEDBY"]);
                    if (dtRow["UPDATEDDATE"] != DBNull.Value)
                        TimeSheetPeriods.UPDATEDDATE = Convert.ToDateTime(dtRow["UPDATEDDATE"]);

                    TimeSheetPeriods.ISDELETED = Convert.ToBoolean(dtRow["ISDELETED"]);
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("TimeSheetPeriodsManagement", "Exception occured on TranslateDataTableToTimeSheetPeriods, " + ex.Message);
            }

            return TimeSheetPeriods;
        }

        private TimeSheetPeriods TranslateDataTableToTimeSheetDetail(DataTable dt)
        {
            TimeSheetPeriods TimeSheetPeriods = new TimeSheetPeriods();

            try
            {
                int sno = 1;
                foreach (DataRow dtRow in dt.Rows)
                {
                    TimeSheetPeriods.SNo = sno;
                    // TimeSheetPeriods.KEY = Guid.NewGuid().ToString();
                    TimeSheetPeriods.ID = Convert.ToInt32(dtRow["ID"]);
                    TimeSheetPeriods.IsDeleted = Convert.ToBoolean(dtRow["IsDeleted"]);
                    TimeSheetPeriods.HeaderID = Convert.ToInt32(dtRow["HeaderID"]);
                    TimeSheetPeriods.Period = Convert.ToString(dtRow["Period"]);
                    TimeSheetPeriods.Monday = Convert.ToDateTime(dtRow["FromDate"]).ToString("ddd, dd MMM yyy");
                    TimeSheetPeriods.Friday = Convert.ToDateTime(dtRow["ToDate"]).ToString("ddd, dd MMM yyy");
                    TimeSheetPeriods._Monday = Convert.ToDateTime(dtRow["FromDate"]).ToString("yyyy-MM-dd");
                    TimeSheetPeriods._Friday = Convert.ToDateTime(dtRow["ToDate"]).ToString("yyyy-MM-dd");
                    TimeSheetPeriods.StdHoursInWeek = Convert.ToInt32(dtRow["StdHoursInWeek"]);
                    sno = sno + 1;
                    //TimeSheetPeriodsList.Add(TimeSheetPeriods);
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("TimeSheetPeriodsManagement", "Exception occured on TranslateDataTableToTimeSheetPeriodsDetail, " + ex.Message);
            }

            return TimeSheetPeriods;
        }
        private List<TimeSheetPeriods> TranslateDataTableToTimeSheetPeriodsDetail(DataTable dt)
        {
            List<TimeSheetPeriods> TimeSheetPeriodsList = new List<TimeSheetPeriods>();

            try
            {
                int sno = 1;
                foreach (DataRow dtRow in dt.Rows)
                {
                    TimeSheetPeriods TimeSheetPeriods = new TimeSheetPeriods();
                    TimeSheetPeriods.SNo = sno;
                   // TimeSheetPeriods.KEY = Guid.NewGuid().ToString();
                    TimeSheetPeriods.ID = Convert.ToInt32(dtRow["ID"]);
                    TimeSheetPeriods.IsDeleted = Convert.ToBoolean(dtRow["IsDeleted"]);
                    TimeSheetPeriods.HeaderID = Convert.ToInt32(dtRow["HeaderID"]);
                    TimeSheetPeriods.Period = Convert.ToString(dtRow["Period"]);
                    TimeSheetPeriods.Monday = Convert.ToDateTime(dtRow["FromDate"]).ToString("ddd, dd MMM yyy");
                    TimeSheetPeriods.Friday = Convert.ToDateTime(dtRow["ToDate"]).ToString("ddd, dd MMM yyy");
                    TimeSheetPeriods._Monday = Convert.ToDateTime(dtRow["FromDate"]).ToString("yyyy-MM-dd");
                    TimeSheetPeriods._Friday = Convert.ToDateTime(dtRow["ToDate"]).ToString("yyyy-MM-dd");
                    TimeSheetPeriods.StdHoursInWeek = Convert.ToInt32(dtRow["StdHoursInWeek"]);
                    TimeSheetPeriods.TimeSheetPeriodDisply = Convert.ToString(dtRow["Period"]) + " - (" + Convert.ToDateTime(dtRow["FromDate"]).AddDays(-1).ToString("dd/MM/yyyy") + " - " + Convert.ToDateTime(dtRow["ToDate"]).AddDays(1).ToString("dd/MM/yyyy") + ")";

                    sno = sno + 1;
                    TimeSheetPeriodsList.Add(TimeSheetPeriods);
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("TimeSheetPeriodsManagement", "Exception occured on TranslateDataTableToTimeSheetPeriodsDetail, " + ex.Message);
            }

            return TimeSheetPeriodsList;
        }
        private List<B1SP_Parameter> TranslateTimeSheetPeriodsSetupToParameterList(GenerateTimeSheet TimeSheetPeriods)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {

                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = Convert.ToString(TimeSheetPeriods.ID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "DocNum";
                parm.ParameterValue = Convert.ToString(TimeSheetPeriods.DocNum);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "Year";
                parm.ParameterValue = Convert.ToString(TimeSheetPeriods.year);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "FromDate";
                parm.ParameterValue = Convert.ToString(TimeSheetPeriods.fromDate.ToString("yyyy-MM-dd"));
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ToDate";
                parm.ParameterValue = Convert.ToString(TimeSheetPeriods.toDate.ToString("yyyy-MM-dd"));
                parmList.Add(parm);


                parm = new B1SP_Parameter();
                parm.ParameterName = "ISDELETED";
                parm.ParameterValue = Convert.ToString(TimeSheetPeriods.ISDELETED);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CREATEDBY";
                parm.ParameterValue = Convert.ToString(TimeSheetPeriods.CREATEDBY);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CREATEDDATE";
                parm.ParameterValue = DateTime.Now.ToString("yyyy-MM-dd");
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "UPDATEDEDBY";
                parm.ParameterValue = Convert.ToString(TimeSheetPeriods.UPDATEDEDBY);
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
                log.InputOutputDocLog("TimeSheetPeriodsManagement", "Exception occured on TranslateTimeSheetPeriodsSetupToParameterList, " + ex.Message);
            }


            return parmList;
        }
        private List<B1SP_Parameter> TranslateTimeSheetPeriodsDetailToParameterList(TimeSheetPeriods TimeSheetPeriods,bool isUpdate=false)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {

                B1SP_Parameter parm = new B1SP_Parameter();
                if(isUpdate)
                {
                    parm.ParameterName = "ID";
                    parm.ParameterValue = Convert.ToString(TimeSheetPeriods.ID);
                    parmList.Add(parm);

                    parm = new B1SP_Parameter();
                    parm.ParameterName = "StdHoursInWeek";
                    parm.ParameterValue = Convert.ToString(TimeSheetPeriods.StdHoursInWeek);
                    parmList.Add(parm);
                }
                else
                {
                    parm = new B1SP_Parameter();
                    parm.ParameterName = "HeaderID";
                    parm.ParameterValue = Convert.ToString(TimeSheetPeriods.HeaderID);
                    parmList.Add(parm);

                    parm = new B1SP_Parameter();
                    parm.ParameterName = "Period";
                    parm.ParameterValue = Convert.ToString(TimeSheetPeriods.Period);
                    parmList.Add(parm);

                    parm = new B1SP_Parameter();
                    parm.ParameterName = "FromDate";
                    parm.ParameterValue = Convert.ToString(TimeSheetPeriods._Monday);
                    parmList.Add(parm);

                    parm = new B1SP_Parameter();
                    parm.ParameterName = "ToDate";
                    parm.ParameterValue = Convert.ToString(TimeSheetPeriods._Friday);
                    parmList.Add(parm);

                    parm = new B1SP_Parameter();
                    parm.ParameterName = "StdHoursInWeek";
                    parm.ParameterValue = Convert.ToString(TimeSheetPeriods.StdHoursInWeek);
                    parmList.Add(parm);
                }

                

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("TimeSheetPeriodsManagement", "Exception occured on TranslateTimeSheetPeriodsDetailToParameterList, " + ex.Message);
            }


            return parmList;
        }

      
    }
}