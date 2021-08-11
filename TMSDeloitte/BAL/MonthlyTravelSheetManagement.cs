using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using TMSDeloitte.DAL;
using TMSDeloitte.Helper;
using TMSDeloitte.Models;

namespace TMSDeloitte.BAL
{
    public class MonthlyTravelSheetManagement
    {

        public MonthlyTravelSheet GetEmpMonthlyTravelSheetByYearAndMonth(string year, int month, int empID)
        {
            MonthlyTravelSheet obj = new MonthlyTravelSheet();
            try
            {

                List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();

                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "Year";
                parm.ParameterValue = year;
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "Month";
                parm.ParameterValue = Convert.ToString(month);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "EmpID";
                parm.ParameterValue = Convert.ToString(empID);
                parmList.Add(parm);

                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                Common cmn = new Common();
                DataSet ds = HANADAL.GetDataSetByStoredProcedure("GetEmpMonthlyTravelSheetByYearAndMonth", parmList, "MonthlyTravelSheetManagement");
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        obj = TranslateDataTableToMonthlyTravelSheet(ds.Tables[0]);
                        obj.Detail = TranslateDataTableToMonthlyTravelSheetDetail(ds.Tables[1]);
                    }
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("MonthlyTravelSheetManagement", "Exception occured on GetEmpMonthlyTravelSheetByYearAndMont , Empid: "+empID+" , Year: "+year+" , Month: "+month);
            }

            return obj;
        }
       
        public MonthlyTravelSheet GetEmpMonthlyTravelSheet(string year,int month, int empID,string fromDate,string toDate)
        {
            MonthlyTravelSheet header = new MonthlyTravelSheet();
            List<MonthlyTravelSheetDetails> obj = new List<MonthlyTravelSheetDetails>();
            try
            {

                header = GetEmpMonthlyTravelSheetByYearAndMonth(year,month,empID);
                if(header!=null)
                {
                    if(header.Detail!=null)
                    {
                        if (header.Detail.Count > 0)
                            return header;
                    }
                }
               


                List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();

                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "Year";
                parm.ParameterValue = year;
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "EmpID";
                parm.ParameterValue = Convert.ToString(empID);
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
                Common cmn = new Common();
                DataTable dt = HANADAL.GetDataTableByStoredProcedure("GetEmpMonthlyTravelSheet", parmList, "MonthlyTravelSheetManagement");
                if (dt.Rows.Count > 0)
                {
                    obj = TranslateDataTableToMonthlyTravelSheetDetail(dt);
                    header.Detail = obj;
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("MonthlyTravelSheetManagement", "Exception occured on GetEmpMonthlyTravelSheet EmpID: " + empID + " , Year" + year +" , "+ex.Message);
            }

            return header;
        }

        public List<string> GetDocNumByEmpID(int id)
        {
            List<string> docNumList = new List<string>();
            BAL.Common setupManagement = new BAL.Common();
            try
            {
                List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = Convert.ToString(id);
                parmList.Add(parm);


                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                Common cmn = new Common();
                DataTable dt = HANADAL.GetDataTableByStoredProcedure("GetMonthlyTravelSheetByEmpID", parmList, "MonthlyTravelSheetManagement");
                if (dt.Rows.Count > 0)
                {
                    docNumList = setupManagement.TranslateDataTableToDocNumList(dt);
                }

               
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("MonthlyTravelSheetManagement", "Exception occured on GetResourceBillingRatesDocNum, " + ex.Message);
            }

            return docNumList;
        }
        public List<MonthlyTravelSheet> GetAllMonthlyTravelSheetByEmpID(int id)
        {
            List<MonthlyTravelSheet> MonthlyTravelSheetDetails = new List<MonthlyTravelSheet>();
            try
            {
                List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = Convert.ToString(id);
                parmList.Add(parm);

                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                Common cmn = new Common();
                DataSet ds = HANADAL.GetDataSetByStoredProcedure("GetMonthlyTravelSheetByEmpID", parmList, "MonthlyTravelSheetManagement");
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        MonthlyTravelSheetDetails = TranslateDataTableToMonthlyTravelSheetList(ds.Tables[0]);
                        //MonthlyTravelSheetDetails.Detail = TranslateDataTableToMonthlyTravelSheetDetail(ds.Tables[1]);
                    }
                }



            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("MonthlyTravelSheetManagement", "Exception occured on GetMonthlyTravelSheetDetailsByID ID: " + id + " , " + ex.Message);
            }

            return MonthlyTravelSheetDetails;
        }

        public List<MonthlyTravelSheet> GetSubmittedMonthlyTravelSheetDetailsByEmpID(int id)
        {
            List<MonthlyTravelSheet> MonthlyTravelSheetDetails = new List<MonthlyTravelSheet>();
            try
            {
                List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = Convert.ToString(id);
                parmList.Add(parm);

                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                Common cmn = new Common();
                DataSet ds = HANADAL.GetDataSetByStoredProcedure("GetSubmittedMonthlyTravelSheetDetailsByEmpID", parmList, "MonthlyTravelSheetManagement");
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        MonthlyTravelSheetDetails = TranslateDataTableToMonthlyTravelSheetList(ds.Tables[0]);
                        //MonthlyTravelSheetDetails.Detail = TranslateDataTableToMonthlyTravelSheetDetail(ds.Tables[1]);
                    }
                }



            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("MonthlyTravelSheetManagement", "Exception occured on GetMonthlyTravelSheetDetailsByID ID: " + id + " , " + ex.Message);
            }

            return MonthlyTravelSheetDetails;
        }

        public MonthlyTravelSheet GetMonthlyTravelSheetDetailsByID(int id)
        {
            MonthlyTravelSheet MonthlyTravelSheetDetails = new MonthlyTravelSheet();
            try
            {
                List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = Convert.ToString(id);
                parmList.Add(parm);

                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                Common cmn = new Common();
                DataSet ds = HANADAL.GetDataSetByStoredProcedure("GetMonthlyTravelSheetByID", parmList, "MonthlyTravelSheetManagement");
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        MonthlyTravelSheetDetails = TranslateDataTableToMonthlyTravelSheet(ds.Tables[0]);
                        MonthlyTravelSheetDetails.Detail = TranslateDataTableToMonthlyTravelSheetDetail(ds.Tables[1]);
                    }
                }



            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("MonthlyTravelSheetManagement", "Exception occured on GetMonthlyTravelSheetDetailsByID ID: " + id + " , " + ex.Message);
            }

            return MonthlyTravelSheetDetails;
        }

        public bool AddUpdateMonthlyTravelSheetDetailsSetup(MonthlyTravelSheet MonthlyTravelSheetDetails, out string msg)
        {
            bool isSuccess = false;
            bool isAddOccured = false;
            bool isUpdateOccured = false;
            bool isDeleteOccured = false;
            MonthlyTravelSheet previousObj = new MonthlyTravelSheet();
           Common cmn = new Common();
            string docNum = "";
            int docId = 0;
            msg = "Successfully Added/Updated";
            try
            {
                var value = MonthlyTravelSheetDetails.DocNum;
                if (!string.IsNullOrEmpty(value))
                {
                    docNum = value;
                    docId = Convert.ToInt32(MonthlyTravelSheetDetails.ID);
                    
                    previousObj = GetMonthlyTravelSheetDetailsByID(docId);

                    AddHeader_Log(MonthlyTravelSheetDetails, previousObj,docId);
                    
                }
                else
                {
                   
                    int no = 1;
                    List<string> docNumList = cmn.GetDocNum("GetMonthlyTravelSheetDocNum", "MonthlyTravelSheetManagement");
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



                MonthlyTravelSheetDetails.DocNum = docNum;
                if(docId==0)
                    MonthlyTravelSheetDetails.Detail = MonthlyTravelSheetDetails.Detail.Where(x => x.IsDeleted == false && x.ID == 0).ToList();
                else
                    MonthlyTravelSheetDetails.Detail = MonthlyTravelSheetDetails.Detail.Where(x => x.IsDeleted == false && (x.ID == 0|| x.ID>0)).ToList();
               
                ////For Form Log
                // AddMonthlyTravelSheetDetailsSetup_Log(MonthlyTravelSheetDetailsList, out isUpdateOccured);

                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                DataTable dtHeader = HANADAL.AddUpdateDataByStoredProcedure("AddUpdateMonthlyTravelSheet", TranslateMonthlyTravelSheetDetailsSetupToParameterList(MonthlyTravelSheetDetails), "MonthlyTravelSheetManagement");
                if (dtHeader.Rows.Count == 0)
                    throw new Exception("Exception occured when AddUpdateMonthlyTravelSheetDetailsSetup , ID:" + MonthlyTravelSheetDetails.ID + " , Emp ID:" + MonthlyTravelSheetDetails.EmpID +" , Year: " + MonthlyTravelSheetDetails.Year + " , Month: " + MonthlyTravelSheetDetails.Month);
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
                //    //AddMonthlyTravelSheetDetails_Log(MonthlyTravelSheetDetails, out isUpdateOccured);
                //    DeleteMonthlyTravelSheetDetailsDetailByHeaderID(docId); 
                //}

                MonthlyTravelSheetDetails.Detail.Select(c => { c.HeaderID = docId; return c; }).ToList();

                foreach (var list in MonthlyTravelSheetDetails.Detail)
                {
                    try
                    {
                        if(list.ID>0)
                        {
                            var previousObjectDetail = previousObj.Detail.Where(x => x.ID == list.ID).FirstOrDefault();

                            AddDetail_Log(list, previousObjectDetail, docId);
                        }

                        HANADAL.AddUpdateDataByStoredProcedure("AddUpdateMonthlyTravelSheetDetail", TranslateMonthlyTravelSheetDetailsDetailToParameterList(list), "MonthlyTravelSheetManagement");
                       
                    }
                    catch (Exception ex)
                    {
                        isSuccess = false;
                        Log log = new Log();
                        log.LogFile(ex.Message);
                        log.InputOutputDocLog("MonthlyTravelSheetManagement", "Exception occured in foreach loop AddUpdateMonthlyTravelSheetDetail, " + ex.Message);
                        continue;
                    }
                }
                //For deleted Item
                if(docId>0)
                {
                    if(previousObj!=null)
                    {
                        if (previousObj.Detail!=null)
                        {
                            List<MonthlyTravelSheetDetails> missingList = previousObj.Detail.Where(n => !MonthlyTravelSheetDetails.Detail.Any(o => o.ID == n.ID && o.IsDeleted == n.IsDeleted)).ToList();
                            foreach (var item in missingList)
                            {
                                item.IsDeleted = true;

                                var previousObjectDetail = previousObj.Detail.Where(x => x.ID == item.ID).FirstOrDefault();

                                AddDetail_Log(item, previousObjectDetail, docId);


                               
                                isDeleteOccured = true;
                                //AddUserAlertSetup_Log(user, previous_item, DocId);
                                HANADAL.AddUpdateDataByStoredProcedure("AddUpdateMonthlyTravelSheetDetail", TranslateMonthlyTravelSheetDetailsDetailToParameterList(item), "MonthlyTravelSheetManagement");

                            }
                        }
                    }
                   

                   
                }

                //For Master Log
                if (MonthlyTravelSheetDetails.ID > 0)
                    isAddOccured = true;
                if (Convert.ToBoolean(MonthlyTravelSheetDetails.IsDeleted))
                    isDeleteOccured = true;

                int CreatedBy = Convert.ToInt32(MonthlyTravelSheetDetails.CreatedBy);
                Task.Run(() => cmn.AddMasterLog(isAddOccured, isUpdateOccured, isDeleteOccured, nameof(Enums.FormsName.MonthlyTravelManagement), CreatedBy, "MonthlyTravelSheetManagement"));
                //End MAster Log


                isSuccess = true;


                if (MonthlyTravelSheetDetails.Status == 2)
                {
                    Encrypt_Decrypt security = new Encrypt_Decrypt();
                    //For Notification and Email when time sheet submit
                    System.Web.Routing.RequestContext requestContext = HttpContext.Current.Request.RequestContext;
                    string lnkHref = new System.Web.Mvc.UrlHelper(requestContext).Action("GetApprovalDecision", "Home", new { empID = "EncryptedID", docID = security.EncryptURLString(docId.ToString()), docType = security.EncryptURLString("Travel Claim") }, HttpContext.Current.Request.Url.Scheme);

                    cmn = new Common();
                    Task.Run(() => cmn.SndNotificationAndEmail(Convert.ToInt32(MonthlyTravelSheetDetails.CreatedBy), MonthlyTravelSheetDetails.Month, MonthlyTravelSheetDetails.Year, lnkHref, "MonthlyTravelSheetManagement"));
                }
            }
            catch (Exception ex)
            {
                isSuccess = false;
                msg = "Exception occured on Add/Update";
                isSuccess = false;
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("MonthlyTravelSheetManagement", "Exception occured on AddUpdateAssignmentCostSetup, " + ex.Message);
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
                DataTable dt_UserProfile = HANADAL.GetDataTableByStoredProcedure("ValidateTimeSheetPeriodDateRange", parmList, "MonthlyTravelSheetManagement");
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
                log.InputOutputDocLog("MonthlyTravelSheetManagement", "Exception occured on ValidateDateRange, " + ex.Message);
            }
            return isSuccess;
        }

        public MonthlyTravelSheet DeleteMonthlyTravelSheetDetailsDetailByHeaderID(int HeaderID)
        {
            MonthlyTravelSheet MonthlyTravelSheetDetails = new MonthlyTravelSheet();
            try
            {
                List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "HeaderID";
                parm.ParameterValue = Convert.ToString(HeaderID);
                parmList.Add(parm);

                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                Common cmn = new Common();
                HANADAL.AddUpdateDataByStoredProcedure("DeleteTimeSheetPeriodDetail", parmList, "MonthlyTravelSheetManagement");
                //DataSet ds = HANADAL.GetDataSetByStoredProcedure("DeleteTimeSheetPeriodDetail", parmList, "MonthlyTravelSheetManagement");
                //if (ds.Tables.Count > 0)
                //{
                //    if (ds.Tables[0].Rows.Count > 0)
                //    {
                //        MonthlyTravelSheetDetails = TranslateDataTableToMonthlyTravelSheet(ds.Tables[0]);
                //        MonthlyTravelSheetDetails.Detail = TranslateDataTableToMonthlyTravelSheetDetail(ds.Tables[1]);
                //    }
                //}



            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("MonthlyTravelSheetManagement", "Exception occured on DeleteMonthlyTravelSheetDetailsDetailByHeaderID, ID: " + HeaderID + " , Exception: " + ex.Message);
            }

            return MonthlyTravelSheetDetails;
        }

        public DataTable GetAllDocumentsList()
        {
            DataTable dt = new DataTable();
            Common cmn = new Common();
            try

            {
                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                dt = HANADAL.GetDataTableByStoredProcedure("GetMonthlyTravelSheetAllDocuments", "MonthlyTravelSheetManagement");
                dt = cmn.RemoveDuplicateRows(dt, "DocNum");
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("MonthlyTravelSheetManagement", "Exception occured on GetTimeSheetAllDocumentsList, " + ex.Message);
            }

            return dt;
        }

        public MonthlyTravelSheet GetMonthlyTravelSheetByDocNum(string docNo,int empID)
        {
            MonthlyTravelSheet obj = new MonthlyTravelSheet();
            try
            {

                List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();

                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "DocNum";
                parm.ParameterValue = docNo;
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "EmpID";
                parm.ParameterValue =Convert.ToString(empID);
                parmList.Add(parm);

                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                Common cmn = new Common();
                DataSet ds = HANADAL.GetDataSetByStoredProcedure("GetMonthlyTravelSheetByDocNum", parmList, "MonthlyTravelSheetManagement");
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        obj = TranslateDataTableToMonthlyTravelSheet(ds.Tables[0]);
                        obj.Detail = TranslateDataTableToMonthlyTravelSheetDetail(ds.Tables[1]);
                    }
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("MonthlyTravelSheetManagement", "Exception occured on GetResourceBillingRatesByDocNum DocNum: " + docNo + " , " + ex.Message);
            }

            return obj;
        }

        public DataSet GetMonthlyTravelSheetLog(int docid, int empID)
        {
            DataSet ds = new DataSet();
            try
            {

                List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();

                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "DocID";
                parm.ParameterValue =Convert.ToString(docid);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "EmpID";
                parm.ParameterValue = Convert.ToString(empID);
                parmList.Add(parm);

                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                Common cmn = new Common();
                 ds = HANADAL.GetDataSetByStoredProcedure("GetMonthlyTravelLog", parmList, "MonthlyTravelSheetManagement");
                //if (ds.Tables.Count > 0)
                //{
                //    if (ds.Tables[0].Rows.Count > 0)
                //    {
                //        obj = TranslateDataTableToMonthlyTravelSheet(ds.Tables[0]);
                //        obj.Detail = TranslateDataTableToMonthlyTravelSheetDetail(ds.Tables[1]);
                //    }
                //}
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("MonthlyTravelSheetManagement", "Exception occured on GetMonthlyTravelSheetLog DocID: " + docid + " , " + ex.Message);
            }

            return ds;
        }
        public MonthlyTravelSheet GetTimeSheetByYear(string year)
        {
            MonthlyTravelSheet obj = new MonthlyTravelSheet();
            try
            {

                List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();

                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "Year";
                parm.ParameterValue = year;
                parmList.Add(parm);

                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                Common cmn = new Common();
                DataSet ds = HANADAL.GetDataSetByStoredProcedure("GetTimeSheetPeriodByPeriod", parmList, "MonthlyTravelSheetManagement");
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        obj = TranslateDataTableToMonthlyTravelSheet(ds.Tables[0]);
                        obj.Detail = TranslateDataTableToMonthlyTravelSheetDetail(ds.Tables[1]);
                    }
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("MonthlyTravelSheetManagement", "Exception occured on GetTimeSheetByYear year: " + year + " , " + ex.Message);
            }

            return obj;
        }

        public string GetAssignmentFormTitleByID(int id)
        {
            string title = "";
            BAL.Common setupManagement = new BAL.Common();
            try
            {
                List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = Convert.ToString(id);
                parmList.Add(parm);


                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                Common cmn = new Common();
                DataTable dt = HANADAL.GetDataTableByStoredProcedure("GetAssignmentFormTitleByID", parmList, "MonthlyTravelSheetManagement");
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dtRow in dt.Rows)
                    {
                        title = Convert.ToString(dtRow["AssignmentTitle"]);
                    }
                }


            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("MonthlyTravelSheetManagement", "Exception occured on GetAssignmentFormTitleByID, " + ex.Message);
            }

            return title;
        }

        public void AddHeader_Log(MonthlyTravelSheet newObject, MonthlyTravelSheet previousObject, int docID)
        {
            try
            {
                if (newObject.ID > 0)
                {
                    List<B1SP_Parameter> paramList = TranslateMonthlyTravelSheetDetailsSetupLogToParameterList(newObject, docID);
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


                                case "Status":
                                    isChangeOccured = true;

                                    paramList.Where(x => x.ParameterName == "Status_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                    paramList.Where(x => x.ParameterName == "Status_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();

                                    break;

                                case "DocDate":
                                    isChangeOccured = true;
                                    paramList.Where(x => x.ParameterName == "DocDate_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                    paramList.Where(x => x.ParameterName == "DocDate_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();

                                    break;

                                case "TotalAmount":
                                    isChangeOccured = true;
                                    paramList.Where(x => x.ParameterName == "TotalAmount_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                    paramList.Where(x => x.ParameterName == "TotalAmount_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();
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
                            Task.Run(() => HANADAL.AddUpdateDataByStoredProcedure("AddMonthlyTravelSheet_Log", paramList, "MonthlyTravelSheetManagement"));
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("MonthlyTravelSheetManagement", "Exception occured on AddHeader_Log, " + ex.Message);
            }
        }

        public void AddDetail_Log(MonthlyTravelSheetDetails newObject, MonthlyTravelSheetDetails previousObject, int docID)
        {
            try
            {
                if (newObject.ID > 0)
                {
                    List<B1SP_Parameter> paramList = TranslateMonthlyTravelSheetDetailsDetailLogToParameterList(newObject, docID);
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


                                case "_TravelDate":
                                    isChangeOccured = true;
                                    //PreviousValue = Convert.ToString(((Enums.AlertSetup)PreviousValue));
                                    //NewValue = Convert.ToString(((Enums.AlertSetup)NewValue));

                                    paramList.Where(x => x.ParameterName == "TravelDate_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                    paramList.Where(x => x.ParameterName == "TravelDate_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();

                                    break;

                                case "AssignmentID":
                                    isChangeOccured = true;
                                    paramList.Where(x => x.ParameterName == "AssignmentID_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                    paramList.Where(x => x.ParameterName == "AssignmentID_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();

                                    break;

                                case "Description":
                                    if (NewValue != null)
                                    {
                                        isChangeOccured = true;
                                        paramList.Where(x => x.ParameterName == "Description_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                        paramList.Where(x => x.ParameterName == "Description_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();
                                    }
                                    break;

                                case "Kilometers":
                                    isChangeOccured = true;
                                    paramList.Where(x => x.ParameterName == "Kilometers_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                    paramList.Where(x => x.ParameterName == "Kilometers_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();
                                    break;

                                case "Amount":
                                    isChangeOccured = true;
                                    paramList.Where(x => x.ParameterName == "Amount_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                    paramList.Where(x => x.ParameterName == "Amount_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();
                                    break;

                                case "ParkingCharges":
                                    
                                    isChangeOccured = true;
                                    paramList.Where(x => x.ParameterName == "ParkingCharges_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                    paramList.Where(x => x.ParameterName == "ParkingCharges_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();

                                    break;

                                case "TotalAmount":
                                    isChangeOccured = true;
                                    paramList.Where(x => x.ParameterName == "TotalAmount_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                    paramList.Where(x => x.ParameterName == "TotalAmount_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();
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
                            Task.Run(() => HANADAL.AddUpdateDataByStoredProcedure("AddUpdateMonthlyTravelSheetDetail_Log", paramList, "MonthlyTravelSheetManagement"));
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("MonthlyTravelSheetManagement", "Exception occured on AddDetail_Log, " + ex.Message);
            }
        }


        public bool UpdateMonthlyTravelSheetStatus(MonthlyTravelSheet MonthlyTravelSheetDetails, out string msg)
        {
            bool isSuccess = false;
            bool isAddOccured = false;
            bool isUpdateOccured = false;
            bool isDeleteOccured = false;
            MonthlyTravelSheet previousObj = new MonthlyTravelSheet();
            Common cmn = new Common();
            string docNum = "";
            int docId = 0;
            msg = "Successfully Updated";

            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            B1SP_Parameter parm = new B1SP_Parameter();

            try
            {
                if(MonthlyTravelSheetDetails==null)
                {
                   
                    msg = "Document number not found!";
                    return false;
                }
                var value = MonthlyTravelSheetDetails._DocNum;
                if (string.IsNullOrEmpty(value))
                {

                    msg = "Document number not found!";
                    return false;
                }

              
                if (!string.IsNullOrEmpty(value))
                {
                    Encrypt_Decrypt security = new Encrypt_Decrypt();

                    docId = Convert.ToInt32(MonthlyTravelSheetDetails.ID);

                    parm = new B1SP_Parameter();
                    parm.ParameterName = "ID";
                    parm.ParameterValue =Convert.ToString(docId);// Convert.ToString(MonthlyTravelSheetDetails.ID);
                    parmList.Add(parm);

                    parm = new B1SP_Parameter();
                    parm.ParameterName = "Status";
                    parm.ParameterValue = Convert.ToString(MonthlyTravelSheetDetails.Status);
                    parmList.Add(parm);

                    parm = new B1SP_Parameter();
                    parm.ParameterName = "DocNum";
                    parm.ParameterValue = Convert.ToString(security.DecryptString(MonthlyTravelSheetDetails._DocNum));
                    parmList.Add(parm);

                    parm = new B1SP_Parameter();
                    parm.ParameterName = "EmpID";
                    parm.ParameterValue = Convert.ToString(security.DecryptString(MonthlyTravelSheetDetails._EmpID));
                    parmList.Add(parm);

                    parm = new B1SP_Parameter();
                    parm.ParameterName = "UpdatedBy";
                    parm.ParameterValue = Convert.ToString(MonthlyTravelSheetDetails.UpdatedBy);
                    parmList.Add(parm);

                    parm = new B1SP_Parameter();
                    parm.ParameterName = "UpdatedDate";
                    parm.ParameterValue = DateTime.Now.ToString("yyyy-MM-dd");
                    parmList.Add(parm);

                    previousObj = GetMonthlyTravelSheetDetailsByID(docId);

                    AddHeader_Log(MonthlyTravelSheetDetails, previousObj, docId);

                    HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                    DataTable dtHeader = HANADAL.AddUpdateDataByStoredProcedure("UpdateMonthlyTravelSheetStatus", parmList, "MonthlyTravelSheetManagement");
                    if (dtHeader.Rows.Count == 0)
                        throw new Exception("Exception occured when cencel document , ID:" + MonthlyTravelSheetDetails.ID + " , Emp ID:" + MonthlyTravelSheetDetails.EmpID);
                   

                    //For Master Log
                    isUpdateOccured = true;
                    int CreatedBy = Convert.ToInt32(MonthlyTravelSheetDetails.CreatedBy);
                    Task.Run(() => cmn.AddMasterLog(isAddOccured, isUpdateOccured, isDeleteOccured, nameof(Enums.FormsName.MonthlyTravelManagement), CreatedBy, "MonthlyTravelSheetManagement"));
                    //End MAster Log
                }
                else
                {
                    isSuccess = false;
                    msg = "Document number not found!";
                }
                    




                isSuccess = true;
            }
            catch (Exception ex)
            {
                isSuccess = false;
                msg = "Exception occured on cencel document!";
                isSuccess = false;
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("MonthlyTravelSheetManagement", "Exception occured on UpdateMonthlyTravelSheetStatus, " + ex.Message);
            }

            return isSuccess;
        }

        private List<MonthlyTravelSheet> TranslateDataTableToMonthlyTravelSheetList(DataTable dt)
        {
            List<MonthlyTravelSheet> list = new List<MonthlyTravelSheet>();
            Common cmn = new Common();
            List<TimseSheetStatus> statusList = cmn.GetTimeSheetFormStatusList();
            try
            {
                foreach (DataRow dtRow in dt.Rows)
                {
                    MonthlyTravelSheet MonthlyTravelSheetDetails = new MonthlyTravelSheet();
                    MonthlyTravelSheetDetails.ID = Convert.ToInt32(dtRow["ID"]);
                    MonthlyTravelSheetDetails.Status = Convert.ToInt32(dtRow["Status"]);
                    var status = statusList.Where(x => x.ID == MonthlyTravelSheetDetails.Status).FirstOrDefault();
                    if (status != null)
                        MonthlyTravelSheetDetails.StatusName = status.Name;

                    MonthlyTravelSheetDetails.DocNum = Convert.ToString(dtRow["DocNum"]);
                    MonthlyTravelSheetDetails.EmpID = Convert.ToInt32(dtRow["EmpID"]);
                    MonthlyTravelSheetDetails.EmpCode = Convert.ToString(dtRow["EmpCode"]);
                    MonthlyTravelSheetDetails.Year = Convert.ToInt32(dtRow["Year"]);
                    MonthlyTravelSheetDetails.Month = Convert.ToInt32(dtRow["Month"]);
                    MonthlyTravelSheetDetails.MonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(MonthlyTravelSheetDetails.Month);

                    MonthlyTravelSheetDetails.DocDate = Convert.ToDateTime(dtRow["DocDate"]).ToString("yyyy-MM-dd");
                    MonthlyTravelSheetDetails.TotalAmount = Convert.ToDouble(dtRow["TotalAmount"]);
                    MonthlyTravelSheetDetails.CreatedBy = Convert.ToInt32(dtRow["CreatedBy"]);
                    MonthlyTravelSheetDetails.CreatedDate = Convert.ToDateTime(dtRow["CREATEDDATE"]);
                    if (dtRow["UpdatedBy"] != DBNull.Value)
                        MonthlyTravelSheetDetails.UpdatedBy = Convert.ToInt32(dtRow["UpdatedBy"]);
                    if (dtRow["UpdatedDate"] != DBNull.Value)
                        MonthlyTravelSheetDetails.UpdatedDate = Convert.ToDateTime(dtRow["UpdatedDate"]);

                    MonthlyTravelSheetDetails.IsDeleted = Convert.ToBoolean(dtRow["IsDeleted"]);

                   // BAL.TimeSheetPeriodManagement mgt = new TimeSheetPeriodManagement();

                   //TimeSheetPeriods detail= mgt.GetTimeSheetPeriodDetailByID(MonthlyTravelSheetDetails.Period);
                   // if(detail!=null)
                   // {
                   //     MonthlyTravelSheetDetails.PeriodText = detail.Period;
                   //     MonthlyTravelSheetDetails.FromDate = detail._Monday;
                   //     MonthlyTravelSheetDetails.ToDate = detail._Friday;
                   // }

                    list.Add(MonthlyTravelSheetDetails);
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("MonthlyTravelSheetManagement", "Exception occured on TranslateDataTableToMonthlyTravelSheetList, " + ex.Message);
            }

            return list;
        }
        private MonthlyTravelSheet TranslateDataTableToMonthlyTravelSheet(DataTable dt)
        {
            MonthlyTravelSheet MonthlyTravelSheetDetails = new MonthlyTravelSheet();
            Common cmn = new Common();
            List<TimseSheetStatus> statusList = cmn.GetTimeSheetFormStatusList();
            try
            {
                foreach (DataRow dtRow in dt.Rows)
                {

                    MonthlyTravelSheetDetails.ID = Convert.ToInt32(dtRow["ID"]);
                    MonthlyTravelSheetDetails.Status = Convert.ToInt32(dtRow["Status"]);
                    var status = statusList.Where(x => x.ID == MonthlyTravelSheetDetails.Status).FirstOrDefault();
                    if (status != null)
                        MonthlyTravelSheetDetails.StatusName = status.Name;

                    MonthlyTravelSheetDetails.DocNum = Convert.ToString(dtRow["DocNum"]);
                    MonthlyTravelSheetDetails.EmpID = Convert.ToInt32(dtRow["EmpID"]);
                    MonthlyTravelSheetDetails.EmpCode = Convert.ToString(dtRow["EmpCode"]);
                    MonthlyTravelSheetDetails.Year = Convert.ToInt32(dtRow["Year"]);
                    MonthlyTravelSheetDetails.Month = Convert.ToInt32(dtRow["Month"]);
                    MonthlyTravelSheetDetails.DocDate = Convert.ToDateTime(dtRow["DocDate"]).ToString("yyyy-MM-dd");
                    MonthlyTravelSheetDetails.TotalAmount = Convert.ToDouble(dtRow["TotalAmount"]);
                    MonthlyTravelSheetDetails.CreatedBy = Convert.ToInt32(dtRow["CreatedBy"]);
                    MonthlyTravelSheetDetails.CreatedDate = Convert.ToDateTime(dtRow["CREATEDDATE"]);
                    if (dtRow["UpdatedBy"] != DBNull.Value)
                        MonthlyTravelSheetDetails.UpdatedBy = Convert.ToInt32(dtRow["UpdatedBy"]);
                    if (dtRow["UpdatedDate"] != DBNull.Value)
                        MonthlyTravelSheetDetails.UpdatedDate = Convert.ToDateTime(dtRow["UpdatedDate"]);

                    MonthlyTravelSheetDetails._Attachements = Convert.ToString(dtRow["Attachements"]);
                    MonthlyTravelSheetDetails.IsDeleted = Convert.ToBoolean(dtRow["IsDeleted"]);
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("MonthlyTravelSheetManagement", "Exception occured on TranslateDataTableToMonthlyTravelSheet, " + ex.Message);
            }

            return MonthlyTravelSheetDetails;
        }
        private List<MonthlyTravelSheetDetails> TranslateDataTableToMonthlyTravelSheetDetail(DataTable dt)
        {
            List<MonthlyTravelSheetDetails> MonthlyTravelSheetDetailsList = new List<MonthlyTravelSheetDetails>();
            List<SAP_Function> Clients = new List<SAP_Function>();
            int sno = 1;
            try
            {
                if(dt.Rows.Count>0)
                {
                    BAL.Common setupManagement = new BAL.Common();
                    Clients = setupManagement.GetClientFromSAPB1();
                }
               
                
                foreach (DataRow dtRow in dt.Rows)
                {
                    var isExist = MonthlyTravelSheetDetailsList.Where(x => x._TravelDate == Convert.ToDateTime(dtRow["TravelDate"]).ToString("yyyy-MM-dd")
                                                                        && x.AssignmentID == Convert.ToInt32(dtRow["AssignmentID"])
                                                                        && x.LocationID == Convert.ToInt32(dtRow["LocationID"])).ToList();
                    if(isExist.Count==0)
                    {
                        MonthlyTravelSheetDetails MonthlyTravelSheetDetails = new MonthlyTravelSheetDetails();
                        MonthlyTravelSheetDetails.SNo = sno;
                        MonthlyTravelSheetDetails.KEY = Guid.NewGuid().ToString();
                        MonthlyTravelSheetDetails.ID = Convert.ToInt32(dtRow["ID"]);
                        MonthlyTravelSheetDetails.HeaderID = Convert.ToInt32(dtRow["HeaderID"]);
                        MonthlyTravelSheetDetails.TravelDate = Convert.ToDateTime(dtRow["TravelDate"]);
                        MonthlyTravelSheetDetails._TravelDate = Convert.ToDateTime(dtRow["TravelDate"]).ToString("yyyy-MM-dd");
                        MonthlyTravelSheetDetails.AssignmentID = Convert.ToInt32(dtRow["AssignmentID"]);
                        MonthlyTravelSheetDetails.Description = Convert.ToString(dtRow["Description"]);
                        MonthlyTravelSheetDetails.Kilometers = Convert.ToDouble(dtRow["Kilometers"]);
                        MonthlyTravelSheetDetails.Amount = Convert.ToDouble(dtRow["Amount"]);
                        MonthlyTravelSheetDetails.ParkingCharges = Convert.ToDouble(dtRow["ParkingCharges"]);
                        MonthlyTravelSheetDetails.TotalAmount = Convert.ToDouble(dtRow["TotalAmount"]);
                        MonthlyTravelSheetDetails.IsDeleted = Convert.ToBoolean(dtRow["IsDeleted"]);
                        try
                        {
                            MonthlyTravelSheetDetails.AssignmentTitle = Convert.ToString(dtRow["AssignmentTitle"]);
                            if (MonthlyTravelSheetDetails.AssignmentTitle == "")
                            {
                                if (MonthlyTravelSheetDetails.AssignmentID > 0)
                                {
                                    MonthlyTravelSheetDetails.AssignmentTitle = GetAssignmentFormTitleByID(MonthlyTravelSheetDetails.AssignmentID);
                                }
                            }
                            MonthlyTravelSheetDetails.ClientID = Convert.ToString(dtRow["ClientID"]);
                            var clientData = Clients.Where(x => x.CLIENTID == MonthlyTravelSheetDetails.ClientID).FirstOrDefault();
                            if (clientData != null)
                                MonthlyTravelSheetDetails.ClientName = clientData.CLIENTNAME;
                            MonthlyTravelSheetDetails.LocationID = Convert.ToInt32(dtRow["LocationID"]);
                        }
                        catch (Exception) { }

                        try
                        {
                            if (MonthlyTravelSheetDetails.LocationID != 0)
                            {
                                TravelLocationManagement travelLocMgt = new TravelLocationManagement();
                                List<TravelLocationInfo> locList = travelLocMgt.GetTravel_LocationSetup(MonthlyTravelSheetDetails.LocationID);
                                if (locList.Count > 0)
                                {
                                    MonthlyTravelSheetDetails.Kilometers = locList[0].KM;

                                    TravelRatesManagement travelRateMgt = new TravelRatesManagement();
                                    List<TravelRates> rateList = travelRateMgt.GetTravel_RatesSetupByBranchID(Convert.ToInt32(locList[0].BRANCHID));
                                    if (rateList.Count > 0)
                                    {

                                        foreach (var item in rateList)
                                        {
                                            if (IsWithin(locList[0].KM, item.FROMKM, item.TOKM))
                                            {
                                                MonthlyTravelSheetDetails.Amount = item.RATETRIP;//MonthlyTravelSheetDetails.Kilometers * item.RATETRIP;
                                                MonthlyTravelSheetDetails.TotalAmount = MonthlyTravelSheetDetails.ParkingCharges + MonthlyTravelSheetDetails.Amount;
                                                break;
                                            }
                                        }

                                        //var rate = rateList.Where(x => x.FROMKM >= locList[0].KM || x.TOKM <= locList[0].KM).FirstOrDefault();
                                        //if(rate!=null)
                                        //{
                                        //    MonthlyTravelSheetDetails.Amount = MonthlyTravelSheetDetails.Kilometers * rate.RATETRIP;
                                        //}
                                    }
                                }
                            }
                        }
                        catch (Exception) { }

                        sno = sno + 1;
                        MonthlyTravelSheetDetailsList.Add(MonthlyTravelSheetDetails);
                    }
                }
                sno = 1;
                MonthlyTravelSheetDetailsList =MonthlyTravelSheetDetailsList.OrderBy(x => x.TravelDate).ToList();
                foreach (var item in MonthlyTravelSheetDetailsList)
                {
                    item.SNo = sno;
                    sno = sno + 1;
                }


            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("MonthlyTravelSheetManagement", "Exception occured on TranslateDataTableToMonthlyTravelSheetDetail, " + ex.Message);
            }

            return MonthlyTravelSheetDetailsList;
        }

        public  bool IsWithin(double value, double minimum, double maximum)
        {
            return value >= minimum && value <= maximum;
        }
        private List<B1SP_Parameter> TranslateMonthlyTravelSheetDetailsSetupToParameterList(MonthlyTravelSheet MonthlyTravelSheetDetails)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {

                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = Convert.ToString(MonthlyTravelSheetDetails.ID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "Status";
                parm.ParameterValue = Convert.ToString(MonthlyTravelSheetDetails.Status);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "DocNum";
                parm.ParameterValue = Convert.ToString(MonthlyTravelSheetDetails.DocNum);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "EmpID";
                parm.ParameterValue = Convert.ToString(MonthlyTravelSheetDetails.EmpID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "EmpCode";
                parm.ParameterValue = Convert.ToString(MonthlyTravelSheetDetails.EmpCode);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "Year";
                parm.ParameterValue = Convert.ToString(MonthlyTravelSheetDetails.Year);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "Month";
                parm.ParameterValue = Convert.ToString(MonthlyTravelSheetDetails.Month);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "DocDate";
                parm.ParameterValue = Convert.ToString(MonthlyTravelSheetDetails.DocDate);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "TotalAmount";
                parm.ParameterValue = Convert.ToString(MonthlyTravelSheetDetails.TotalAmount);
                parmList.Add(parm);

                string attachements ="";
                if(MonthlyTravelSheetDetails.Attachements!=null)
                {
                    if (MonthlyTravelSheetDetails.Attachements.Count >0)
                    {
                        attachements = string.Join(",", MonthlyTravelSheetDetails.Attachements.Select(x => x.Name));
                    }
                }
               
                parm = new B1SP_Parameter();
                parm.ParameterName = "Attachements";
                parm.ParameterValue = attachements;
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "IsDeleted";
                parm.ParameterValue = Convert.ToString(MonthlyTravelSheetDetails.IsDeleted);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CreatedBy";
                parm.ParameterValue = Convert.ToString(MonthlyTravelSheetDetails.CreatedBy);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CreatedDate";
                parm.ParameterValue = DateTime.Now.ToString("yyyy-MM-dd");
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "UpdatedBy";
                parm.ParameterValue = Convert.ToString(MonthlyTravelSheetDetails.UpdatedBy);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "UpdatedDate";
                parm.ParameterValue = DateTime.Now.ToString("yyyy-MM-dd");
                parmList.Add(parm);


            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("MonthlyTravelSheetManagement", "Exception occured on TranslateMonthlyTravelSheetDetailsSetupToParameterList, " + ex.Message);
            }


            return parmList;
        }
        private List<B1SP_Parameter> TranslateMonthlyTravelSheetDetailsDetailToParameterList(MonthlyTravelSheetDetails MonthlyTravelSheetDetails)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {

                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = Convert.ToString(MonthlyTravelSheetDetails.ID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "HeaderID";
                parm.ParameterValue = Convert.ToString(MonthlyTravelSheetDetails.HeaderID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "TravelDate";
                parm.ParameterValue = Convert.ToString(MonthlyTravelSheetDetails._TravelDate);
                // parm.ParameterValue = Convert.ToString(DateTime.ParseExact(MonthlyTravelSheetDetails.WorkDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd"));
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "AssignmentID";
                parm.ParameterValue = Convert.ToString(MonthlyTravelSheetDetails.AssignmentID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "Description";
                parm.ParameterValue = Convert.ToString(MonthlyTravelSheetDetails.Description);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "LocationID";
                parm.ParameterValue = Convert.ToString(MonthlyTravelSheetDetails.LocationID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ClientID";
                parm.ParameterValue = Convert.ToString(MonthlyTravelSheetDetails.ClientID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "Kilometers";
                parm.ParameterValue = Convert.ToString(MonthlyTravelSheetDetails.Kilometers);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "Amount";
                parm.ParameterValue = Convert.ToString(MonthlyTravelSheetDetails.Amount);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ParkingCharges";
                parm.ParameterValue = Convert.ToString(MonthlyTravelSheetDetails.ParkingCharges);
                parmList.Add(parm);

               

                parm = new B1SP_Parameter();
                parm.ParameterName = "TotalAmount";
                parm.ParameterValue = Convert.ToString(MonthlyTravelSheetDetails.TotalAmount);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "IsDeleted";
                parm.ParameterValue = Convert.ToString(MonthlyTravelSheetDetails.IsDeleted);
                parmList.Add(parm);
                
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("MonthlyTravelSheetManagement", "Exception occured on TranslateMonthlyTravelSheetDetailsDetailToParameterList, " + ex.Message);
            }


            return parmList;
        }
        
        private List<B1SP_Parameter> TranslateMonthlyTravelSheetDetailsSetupLogToParameterList(MonthlyTravelSheet MonthlyTravelSheetDetails,int DocID)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {

                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = Convert.ToString(0);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "DocID";
                parm.ParameterValue = Convert.ToString(DocID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "DocNum";
                parm.ParameterValue = Convert.ToString(MonthlyTravelSheetDetails.DocNum);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "EmpID";
                parm.ParameterValue = Convert.ToString(MonthlyTravelSheetDetails.EmpID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "EmpCode";
                parm.ParameterValue = Convert.ToString(MonthlyTravelSheetDetails.EmpCode);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "Year";
                parm.ParameterValue = Convert.ToString(MonthlyTravelSheetDetails.Year);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "Month";
                parm.ParameterValue = Convert.ToString(MonthlyTravelSheetDetails.Month);
                parmList.Add(parm);


                parm = new B1SP_Parameter();
                parm.ParameterName = "Status_Previous";
                parm.ParameterValue = Convert.ToString(MonthlyTravelSheetDetails.Status);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "DocDate_Previous";
                parm.ParameterValue = Convert.ToString(MonthlyTravelSheetDetails.DocDate);
                parmList.Add(parm);
                
                parm = new B1SP_Parameter();
                parm.ParameterName = "TotalAmount_Previous";
                parm.ParameterValue = Convert.ToString(MonthlyTravelSheetDetails.TotalAmount);
                parmList.Add(parm);
 
                parm = new B1SP_Parameter();
                parm.ParameterName = "IsDeleted_Previous";
                parm.ParameterValue = Convert.ToString(MonthlyTravelSheetDetails.IsDeleted);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "Status_New";
                parm.ParameterValue = Convert.ToString(MonthlyTravelSheetDetails.Status);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "DocDate_New";
                parm.ParameterValue = Convert.ToString(MonthlyTravelSheetDetails.DocDate);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "TotalAmount_New";
                parm.ParameterValue = Convert.ToString(MonthlyTravelSheetDetails.TotalAmount);
                parmList.Add(parm);
                
                parm = new B1SP_Parameter();
                parm.ParameterName = "IsDeleted_New";
                parm.ParameterValue = Convert.ToString(MonthlyTravelSheetDetails.IsDeleted);
                parmList.Add(parm);


                parm = new B1SP_Parameter();
                parm.ParameterName = "CreatedBy";
                parm.ParameterValue = Convert.ToString(MonthlyTravelSheetDetails.CreatedBy);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CreatedDate";
                parm.ParameterValue = DateTime.Now.ToString("yyyy-MM-dd");
                parmList.Add(parm);

             

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("MonthlyTravelSheetManagement", "Exception occured on TranslateMonthlyTravelSheetDetailsSetupLogToParameterList, " + ex.Message);
            }


            return parmList;
        }

        private List<B1SP_Parameter> TranslateMonthlyTravelSheetDetailsDetailLogToParameterList(MonthlyTravelSheetDetails MonthlyTravelSheetDetails,int DocID)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {

                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = Convert.ToString(MonthlyTravelSheetDetails.ID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "HeaderID";
                parm.ParameterValue = Convert.ToString(DocID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "TravelDate_Previous";
                parm.ParameterValue = Convert.ToString(MonthlyTravelSheetDetails._TravelDate);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "AssignmentID_Previous";
                parm.ParameterValue = Convert.ToString(MonthlyTravelSheetDetails.AssignmentID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "Description_Previous";
                parm.ParameterValue = Convert.ToString(MonthlyTravelSheetDetails.Description);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "Kilometers_Previous";
                parm.ParameterValue = Convert.ToString(MonthlyTravelSheetDetails.Kilometers);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "Amount_Previous";
                parm.ParameterValue = Convert.ToString(MonthlyTravelSheetDetails.Amount);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ParkingCharges_Previous";
                parm.ParameterValue = Convert.ToString(MonthlyTravelSheetDetails.ParkingCharges);
                parmList.Add(parm);
                
                parm = new B1SP_Parameter();
                parm.ParameterName = "TotalAmount_Previous";
                parm.ParameterValue = Convert.ToString(MonthlyTravelSheetDetails.TotalAmount);
                parmList.Add(parm);


                parm = new B1SP_Parameter();
                parm.ParameterName = "IsDeleted_Previous";
                parm.ParameterValue = Convert.ToString(MonthlyTravelSheetDetails.IsDeleted);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "TravelDate_New";
                parm.ParameterValue = Convert.ToString(MonthlyTravelSheetDetails._TravelDate);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "AssignmentID_New";
                parm.ParameterValue = Convert.ToString(MonthlyTravelSheetDetails.AssignmentID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "Description_New";
                parm.ParameterValue = Convert.ToString(MonthlyTravelSheetDetails.Description);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "Kilometers_New";
                parm.ParameterValue = Convert.ToString(MonthlyTravelSheetDetails.Kilometers);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "Amount_New";
                parm.ParameterValue = Convert.ToString(MonthlyTravelSheetDetails.Amount);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ParkingCharges_New";
                parm.ParameterValue = Convert.ToString(MonthlyTravelSheetDetails.ParkingCharges);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "TotalAmount_New";
                parm.ParameterValue = Convert.ToString(MonthlyTravelSheetDetails.TotalAmount);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "IsDeleted_New";
                parm.ParameterValue = Convert.ToString(MonthlyTravelSheetDetails.IsDeleted);
                parmList.Add(parm);

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("MonthlyTravelSheetManagement", "Exception occured on TranslateMonthlyTravelSheetDetailsDetailLogToParameterList, " + ex.Message);
            }


            return parmList;
        }
    }
}