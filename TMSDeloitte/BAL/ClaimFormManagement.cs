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
    public class ClaimFormManagement
    {
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
                DataTable dt = HANADAL.GetDataTableByStoredProcedure("GetClaimFormByEmpID", parmList, "ClaimFormManagement");
                if (dt.Rows.Count > 0)
                {
                    docNumList = setupManagement.TranslateDataTableToDocNumList(dt);
                }

               
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ClaimFormManagement", "Exception occured on GetResourceBillingRatesDocNum, " + ex.Message);
            }

            return docNumList;
        }
        public List<ClaimForm> GetAllClaimFormByEmpID(int id)
        {
            List<ClaimForm> ClaimFormDetails = new List<ClaimForm>();
            try
            {
                List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = Convert.ToString(id);
                parmList.Add(parm);

                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                Common cmn = new Common();
                DataSet ds = HANADAL.GetDataSetByStoredProcedure("GetClaimFormByEmpID", parmList, "ClaimFormManagement");
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        ClaimFormDetails = TranslateDataTableToClaimFormList(ds.Tables[0]);
                        //ClaimFormDetails.Detail = TranslateDataTableToClaimFormDetail(ds.Tables[1]);
                    }
                }



            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ClaimFormManagement", "Exception occured on GetClaimFormDetailsByID ID: " + id + " , " + ex.Message);
            }

            return ClaimFormDetails;
        }

        public List<ClaimForm> GetSubmittedClaimFormDetailsByEmpID(int id)
        {
            List<ClaimForm> ClaimFormDetails = new List<ClaimForm>();
            try
            {
                List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = Convert.ToString(id);
                parmList.Add(parm);

                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                Common cmn = new Common();
                DataSet ds = HANADAL.GetDataSetByStoredProcedure("GetSubmittedClaimFormDetailsByEmpID", parmList, "ClaimFormManagement");
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        ClaimFormDetails = TranslateDataTableToClaimFormList(ds.Tables[0]);
                        //ClaimFormDetails.Detail = TranslateDataTableToClaimFormDetail(ds.Tables[1]);
                    }
                }



            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ClaimFormManagement", "Exception occured on GetClaimFormDetailsByID ID: " + id + " , " + ex.Message);
            }

            return ClaimFormDetails;
        }

        public ClaimForm GetClaimFormDetailsByID(int id)
        {
            ClaimForm ClaimFormDetails = new ClaimForm();
            try
            {
                List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = Convert.ToString(id);
                parmList.Add(parm);

                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                Common cmn = new Common();
                DataSet ds = HANADAL.GetDataSetByStoredProcedure("GetClaimFormByID", parmList, "ClaimFormManagement");
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        ClaimFormDetails = TranslateDataTableToClaimForm(ds.Tables[0]);
                        ClaimFormDetails.Detail = TranslateDataTableToClaimFormDetail(ds.Tables[1]);
                    }
                }



            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ClaimFormManagement", "Exception occured on GetClaimFormDetailsByID ID: " + id + " , " + ex.Message);
            }

            return ClaimFormDetails;
        }

        public bool AddUpdateClaimFormDetailsSetup(ClaimForm ClaimFormDetails, out string msg)
        {
            bool isSuccess = false;
            bool isAddOccured = false;
            bool isUpdateOccured = false;
            bool isDeleteOccured = false;
            ClaimForm previousObj = new ClaimForm();
           Common cmn = new Common();
            string docNum = "";
            int docId = 0;
            msg = "Successfully Added/Updated";
            try
            {
                var value = ClaimFormDetails.DocNum;
                if (!string.IsNullOrEmpty(value))
                {
                    docNum = value;
                    docId = Convert.ToInt32(ClaimFormDetails.ID);
                    
                    previousObj = GetClaimFormDetailsByID(docId);

                    AddHeader_Log(ClaimFormDetails, previousObj,docId);
                    
                }
                else
                {
                   
                    int no = 1;
                    List<string> docNumList = cmn.GetDocNum("GetClaimFormDocNum", "ClaimFormManagement");
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



                ClaimFormDetails.DocNum = docNum;
                if(docId==0)
                    ClaimFormDetails.Detail = ClaimFormDetails.Detail.Where(x => x.IsDeleted == false && x.ID == 0).ToList();
                else
                    ClaimFormDetails.Detail = ClaimFormDetails.Detail.Where(x => x.IsDeleted == false && (x.ID == 0|| x.ID>0)).ToList();
               
                ////For Form Log
                // AddClaimFormDetailsSetup_Log(ClaimFormDetailsList, out isUpdateOccured);

                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                DataTable dtHeader = HANADAL.AddUpdateDataByStoredProcedure("AddUpdateClaimForm", TranslateClaimFormDetailsSetupToParameterList(ClaimFormDetails), "ClaimFormManagement");
                if (dtHeader.Rows.Count == 0)
                    throw new Exception("Exception occured when AddUpdateClaimFormDetailsSetup , ID:" + ClaimFormDetails.ID + " , Emp ID:" + ClaimFormDetails.EmpID );
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
                //    //AddClaimFormDetails_Log(ClaimFormDetails, out isUpdateOccured);
                //    DeleteClaimFormDetailsDetailByHeaderID(docId); 
                //}

                ClaimFormDetails.Detail.Select(c => { c.HeaderID = docId; return c; }).ToList();

                foreach (var list in ClaimFormDetails.Detail)
                {
                    try
                    {
                        if(list.ID>0)
                        {
                            var previousObjectDetail = previousObj.Detail.Where(x => x.ID == list.ID).FirstOrDefault();

                            AddDetail_Log(list, previousObjectDetail, docId);
                        }

                        HANADAL.AddUpdateDataByStoredProcedure("AddUpdateClaimFormDetail", TranslateClaimFormDetailsDetailToParameterList(list), "ClaimFormManagement");
                       
                    }
                    catch (Exception ex)
                    {
                        isSuccess = false;
                        Log log = new Log();
                        log.LogFile(ex.Message);
                        log.InputOutputDocLog("ClaimFormManagement", "Exception occured in foreach loop AddUpdateClaimFormDetail, " + ex.Message);
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
                            List<ClaimFormDetails> missingList = previousObj.Detail.Where(n => !ClaimFormDetails.Detail.Any(o => o.ID == n.ID && o.IsDeleted == n.IsDeleted)).ToList();
                            missingList.Select(c => { c.IsDeleted = true; return c; }).ToList();
                            foreach (var item in missingList)
                            {
                                ClaimFormDetails previousObjectDetail = new ClaimFormDetails();
                                previousObjectDetail = previousObj.Detail.Where(x => x.ID == item.ID).FirstOrDefault();
                              

                                AddDetail_Log(item, previousObjectDetail, docId);

                                
                                isDeleteOccured = true;
                                //AddUserAlertSetup_Log(user, previous_item, DocId);
                                HANADAL.AddUpdateDataByStoredProcedure("AddUpdateClaimFormDetail", TranslateClaimFormDetailsDetailToParameterList(item), "ClaimFormManagement");

                            }
                        }
                    }
                   

                   
                }

                //For Master Log
                if (ClaimFormDetails.ID > 0)
                    isAddOccured = true;
                if (Convert.ToBoolean(ClaimFormDetails.IsDeleted))
                    isDeleteOccured = true;

                int CreatedBy = Convert.ToInt32(ClaimFormDetails.CreatedBy);
                Task.Run(() => cmn.AddMasterLog(isAddOccured, isUpdateOccured, isDeleteOccured, nameof(Enums.FormsName.MonthlyTravelManagement), CreatedBy, "ClaimFormManagement"));
                //End MAster Log


                isSuccess = true;


                if (ClaimFormDetails.Status == 2)
                {
                    Encrypt_Decrypt security = new Encrypt_Decrypt();

                    //For Notification and Email when time sheet submit
                    System.Web.Routing.RequestContext requestContext = HttpContext.Current.Request.RequestContext;
                    string lnkHref = new System.Web.Mvc.UrlHelper(requestContext).Action("GetApprovalDecision", "Home", new { empID = "EncryptedID", docID = security.EncryptURLString(docId.ToString()), docType = security.EncryptURLString("Claim") }, HttpContext.Current.Request.Url.Scheme);

                    cmn = new Common();
                    Task.Run(() => cmn.SndNotificationAndEmail(Convert.ToInt32(ClaimFormDetails.CreatedBy), ClaimFormDetails.TotalAmount, 0, lnkHref, "ClaimFormManagement"));
                }
            }
            catch (Exception ex)
            {
                isSuccess = false;
                msg = "Exception occured on Add/Update";
                isSuccess = false;
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ClaimFormManagement", "Exception occured on AddUpdateAssignmentCostSetup, " + ex.Message);
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
                DataTable dt_UserProfile = HANADAL.GetDataTableByStoredProcedure("ValidateTimeSheetPeriodDateRange", parmList, "ClaimFormManagement");
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
                log.InputOutputDocLog("ClaimFormManagement", "Exception occured on ValidateDateRange, " + ex.Message);
            }
            return isSuccess;
        }

        public ClaimForm DeleteClaimFormDetailsDetailByHeaderID(int HeaderID)
        {
            ClaimForm ClaimFormDetails = new ClaimForm();
            try
            {
                List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "HeaderID";
                parm.ParameterValue = Convert.ToString(HeaderID);
                parmList.Add(parm);

                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                Common cmn = new Common();
                HANADAL.AddUpdateDataByStoredProcedure("DeleteTimeSheetPeriodDetail", parmList, "ClaimFormManagement");
                //DataSet ds = HANADAL.GetDataSetByStoredProcedure("DeleteTimeSheetPeriodDetail", parmList, "ClaimFormManagement");
                //if (ds.Tables.Count > 0)
                //{
                //    if (ds.Tables[0].Rows.Count > 0)
                //    {
                //        ClaimFormDetails = TranslateDataTableToClaimForm(ds.Tables[0]);
                //        ClaimFormDetails.Detail = TranslateDataTableToClaimFormDetail(ds.Tables[1]);
                //    }
                //}



            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ClaimFormManagement", "Exception occured on DeleteClaimFormDetailsDetailByHeaderID, ID: " + HeaderID + " , Exception: " + ex.Message);
            }

            return ClaimFormDetails;
        }

        public DataTable GetAllDocumentsList()
        {
            DataTable dt = new DataTable();
            Common cmn = new Common();
            try

            {
                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                dt = HANADAL.GetDataTableByStoredProcedure("GetClaimFormAllDocuments", "ClaimFormManagement");
                dt = cmn.RemoveDuplicateRows(dt, "DocNum");
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ClaimFormManagement", "Exception occured on GetTimeSheetAllDocumentsList, " + ex.Message);
            }

            return dt;
        }

        public ClaimForm GetClaimFormByDocNum(string docNo,int empID)
        {
            ClaimForm obj = new ClaimForm();
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
                DataSet ds = HANADAL.GetDataSetByStoredProcedure("GetClaimFormByDocNum", parmList, "ClaimFormManagement");
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        obj = TranslateDataTableToClaimForm(ds.Tables[0]);
                        obj.Detail = TranslateDataTableToClaimFormDetail(ds.Tables[1]);
                    }
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ClaimFormManagement", "Exception occured on GetResourceBillingRatesByDocNum DocNum: " + docNo + " , " + ex.Message);
            }

            return obj;
        }

        public DataSet GetClaimFormLog(int docid, int empID)
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
                 ds = HANADAL.GetDataSetByStoredProcedure("GetClaimFormLog", parmList, "ClaimFormManagement");
                //if (ds.Tables.Count > 0)
                //{
                //    if (ds.Tables[0].Rows.Count > 0)
                //    {
                //        obj = TranslateDataTableToClaimForm(ds.Tables[0]);
                //        obj.Detail = TranslateDataTableToClaimFormDetail(ds.Tables[1]);
                //    }
                //}
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ClaimFormManagement", "Exception occured on GetClaimFormLog DocID: " + docid + " , " + ex.Message);
            }

            return ds;
        }
        public ClaimForm GetTimeSheetByYear(string year)
        {
            ClaimForm obj = new ClaimForm();
            try
            {

                List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();

                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "Year";
                parm.ParameterValue = year;
                parmList.Add(parm);

                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                Common cmn = new Common();
                DataSet ds = HANADAL.GetDataSetByStoredProcedure("GetTimeSheetPeriodByPeriod", parmList, "ClaimFormManagement");
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        obj = TranslateDataTableToClaimForm(ds.Tables[0]);
                        obj.Detail = TranslateDataTableToClaimFormDetail(ds.Tables[1]);
                    }
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ClaimFormManagement", "Exception occured on GetTimeSheetByYear year: " + year + " , " + ex.Message);
            }

            return obj;
        }


        public void AddHeader_Log(ClaimForm newObject, ClaimForm previousObject, int docID)
        {
            try
            {
                if (newObject.ID > 0)
                {
                    List<B1SP_Parameter> paramList = TranslateClaimFormDetailsSetupLogToParameterList(newObject, docID);
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
                            Task.Run(() => HANADAL.AddUpdateDataByStoredProcedure("AddClaimForm_Log", paramList, "ClaimFormManagement"));
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ClaimFormManagement", "Exception occured on AddHeader_Log, " + ex.Message);
            }
        }

        public void AddDetail_Log(ClaimFormDetails newObject, ClaimFormDetails previousObject, int docID)
        {
            try
            {
                if (newObject.ID > 0)
                {
                    List<B1SP_Parameter> paramList = TranslateClaimFormDetailsDetailLogToParameterList(newObject, docID);
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


                                case "_Date":
                                    isChangeOccured = true;
                                    //PreviousValue = Convert.ToString(((Enums.AlertSetup)PreviousValue));
                                    //NewValue = Convert.ToString(((Enums.AlertSetup)NewValue));

                                    paramList.Where(x => x.ParameterName == "Date_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                    paramList.Where(x => x.ParameterName == "Date_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();

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
                            Task.Run(() => HANADAL.AddUpdateDataByStoredProcedure("AddUpdateClaimFormDetail_Log", paramList, "ClaimFormManagement"));
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ClaimFormManagement", "Exception occured on AddDetail_Log, " + ex.Message);
            }
        }


        public bool UpdateClaimFormStatus(ClaimForm ClaimFormDetails, out string msg)
        {
            bool isSuccess = false;
            bool isAddOccured = false;
            bool isUpdateOccured = false;
            bool isDeleteOccured = false;
            ClaimForm previousObj = new ClaimForm();
            Common cmn = new Common();
            string docNum = "";
            int docId = 0;
            msg = "Successfully Updated";

            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            B1SP_Parameter parm = new B1SP_Parameter();

            try
            {
                if(ClaimFormDetails==null)
                {
                   
                    msg = "Document number not found!";
                    return false;
                }
                var value = ClaimFormDetails._DocNum;
                if (string.IsNullOrEmpty(value))
                {

                    msg = "Document number not found!";
                    return false;
                }

              
                if (!string.IsNullOrEmpty(value))
                {
                    Encrypt_Decrypt security = new Encrypt_Decrypt();

                    docId = Convert.ToInt32(ClaimFormDetails.ID);

                    parm = new B1SP_Parameter();
                    parm.ParameterName = "ID";
                    parm.ParameterValue =Convert.ToString(docId);// Convert.ToString(ClaimFormDetails.ID);
                    parmList.Add(parm);

                    parm = new B1SP_Parameter();
                    parm.ParameterName = "Status";
                    parm.ParameterValue = Convert.ToString(ClaimFormDetails.Status);
                    parmList.Add(parm);

                    parm = new B1SP_Parameter();
                    parm.ParameterName = "DocNum";
                    parm.ParameterValue = Convert.ToString(security.DecryptString(ClaimFormDetails._DocNum));
                    parmList.Add(parm);

                    parm = new B1SP_Parameter();
                    parm.ParameterName = "EmpID";
                    parm.ParameterValue = Convert.ToString(security.DecryptString(ClaimFormDetails._EmpID));
                    parmList.Add(parm);

                    parm = new B1SP_Parameter();
                    parm.ParameterName = "UpdatedBy";
                    parm.ParameterValue = Convert.ToString(ClaimFormDetails.UpdatedBy);
                    parmList.Add(parm);

                    parm = new B1SP_Parameter();
                    parm.ParameterName = "UpdatedDate";
                    parm.ParameterValue = DateTime.Now.ToString("yyyy-MM-dd");
                    parmList.Add(parm);

                    previousObj = GetClaimFormDetailsByID(docId);

                    AddHeader_Log(ClaimFormDetails, previousObj, docId);

                    HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                    DataTable dtHeader = HANADAL.AddUpdateDataByStoredProcedure("UpdateClaimFormStatus", parmList, "ClaimFormManagement");
                    if (dtHeader.Rows.Count == 0)
                        throw new Exception("Exception occured when cencel document , ID:" + ClaimFormDetails.ID + " , Emp ID:" + ClaimFormDetails.EmpID);
                   

                    //For Master Log
                    isUpdateOccured = true;
                    int CreatedBy = Convert.ToInt32(ClaimFormDetails.CreatedBy);
                    Task.Run(() => cmn.AddMasterLog(isAddOccured, isUpdateOccured, isDeleteOccured, nameof(Enums.FormsName.MonthlyTravelManagement), CreatedBy, "ClaimFormManagement"));
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
                log.InputOutputDocLog("ClaimFormManagement", "Exception occured on UpdateClaimFormStatus, " + ex.Message);
            }

            return isSuccess;
        }

        private List<ClaimForm> TranslateDataTableToClaimFormList(DataTable dt)
        {
            List<ClaimForm> list = new List<ClaimForm>();
            Common cmn = new Common();
            List<TimseSheetStatus> statusList = cmn.GetTimeSheetFormStatusList();
            try
            {
                foreach (DataRow dtRow in dt.Rows)
                {
                    ClaimForm ClaimFormDetails = new ClaimForm();
                    ClaimFormDetails.ID = Convert.ToInt32(dtRow["ID"]);
                    ClaimFormDetails.Status = Convert.ToInt32(dtRow["Status"]);
                    var status = statusList.Where(x => x.ID == ClaimFormDetails.Status).FirstOrDefault();
                    if (status != null)
                        ClaimFormDetails.StatusName = status.Name;

                    ClaimFormDetails.DocNum = Convert.ToString(dtRow["DocNum"]);
                    ClaimFormDetails.EmpID = Convert.ToInt32(dtRow["EmpID"]);
                    ClaimFormDetails.EmpCode = Convert.ToString(dtRow["EmpCode"]);
                    ClaimFormDetails.AdvanceReceived = Convert.ToInt32(dtRow["AdvanceReceived"]);
                    ClaimFormDetails.Receivable = Convert.ToInt32(dtRow["Receivable"]);
                    ClaimFormDetails.DocDate = Convert.ToDateTime(dtRow["DocDate"]).ToString("yyyy-MM-dd");
                    ClaimFormDetails.TotalAmount = Convert.ToDouble(dtRow["TotalAmount"]);
                    ClaimFormDetails.CreatedBy = Convert.ToInt32(dtRow["CreatedBy"]);
                    ClaimFormDetails.CreatedDate = Convert.ToDateTime(dtRow["CREATEDDATE"]);
                    if (dtRow["UpdatedBy"] != DBNull.Value)
                        ClaimFormDetails.UpdatedBy = Convert.ToInt32(dtRow["UpdatedBy"]);
                    if (dtRow["UpdatedDate"] != DBNull.Value)
                        ClaimFormDetails.UpdatedDate = Convert.ToDateTime(dtRow["UpdatedDate"]);

                    ClaimFormDetails.ImageFolder = Convert.ToString(dtRow["ImageFolder"]);
                    ClaimFormDetails.IsDeleted = Convert.ToBoolean(dtRow["IsDeleted"]);

                   // BAL.TimeSheetPeriodManagement mgt = new TimeSheetPeriodManagement();

                   //TimeSheetPeriods detail= mgt.GetTimeSheetPeriodDetailByID(ClaimFormDetails.Period);
                   // if(detail!=null)
                   // {
                   //     ClaimFormDetails.PeriodText = detail.Period;
                   //     ClaimFormDetails.FromDate = detail._Monday;
                   //     ClaimFormDetails.ToDate = detail._Friday;
                   // }

                    list.Add(ClaimFormDetails);
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ClaimFormManagement", "Exception occured on TranslateDataTableToClaimFormList, " + ex.Message);
            }

            return list;
        }
        private ClaimForm TranslateDataTableToClaimForm(DataTable dt)
        {
            ClaimForm ClaimFormDetails = new ClaimForm();
            Common cmn = new Common();
            List<TimseSheetStatus> statusList = cmn.GetTimeSheetFormStatusList();
            try
            {
                foreach (DataRow dtRow in dt.Rows)
                {

                    ClaimFormDetails.ID = Convert.ToInt32(dtRow["ID"]);
                    ClaimFormDetails.Status = Convert.ToInt32(dtRow["Status"]);
                    var status = statusList.Where(x => x.ID == ClaimFormDetails.Status).FirstOrDefault();
                    if (status != null)
                        ClaimFormDetails.StatusName = status.Name;

                    ClaimFormDetails.DocNum = Convert.ToString(dtRow["DocNum"]);
                    ClaimFormDetails.ImageFolder = Convert.ToString(dtRow["ImageFolder"]);
                    ClaimFormDetails.EmpID = Convert.ToInt32(dtRow["EmpID"]);
                    ClaimFormDetails.EmpCode = Convert.ToString(dtRow["EmpCode"]);
                    ClaimFormDetails.AdvanceReceived = Convert.ToDouble(dtRow["AdvanceReceived"]);
                    ClaimFormDetails.Receivable = Convert.ToDouble(dtRow["Receivable"]);
                    ClaimFormDetails.DocDate = Convert.ToDateTime(dtRow["DocDate"]).ToString("yyyy-MM-dd");
                    ClaimFormDetails.TotalAmount = Convert.ToDouble(dtRow["TotalAmount"]);
                    ClaimFormDetails.CreatedBy = Convert.ToInt32(dtRow["CreatedBy"]);
                    ClaimFormDetails.CreatedDate = Convert.ToDateTime(dtRow["CREATEDDATE"]);
                    if (dtRow["UpdatedBy"] != DBNull.Value)
                        ClaimFormDetails.UpdatedBy = Convert.ToInt32(dtRow["UpdatedBy"]);
                    if (dtRow["UpdatedDate"] != DBNull.Value)
                        ClaimFormDetails.UpdatedDate = Convert.ToDateTime(dtRow["UpdatedDate"]);

                    ClaimFormDetails._Attachements = Convert.ToString(dtRow["Attachements"]);
                    ClaimFormDetails.IsDeleted = Convert.ToBoolean(dtRow["IsDeleted"]);
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ClaimFormManagement", "Exception occured on TranslateDataTableToClaimForm, " + ex.Message);
            }

            return ClaimFormDetails;
        }
        private List<ClaimFormDetails> TranslateDataTableToClaimFormDetail(DataTable dt)
        {
            List<ClaimFormDetails> ClaimFormDetailsList = new List<ClaimFormDetails>();

            try
            {
                int sno = 1;
                foreach (DataRow dtRow in dt.Rows)
                {
                    ClaimFormDetails ClaimFormDetails = new ClaimFormDetails();
                    ClaimFormDetails.SNo = sno;
                    ClaimFormDetails.KEY = Guid.NewGuid().ToString();
                    ClaimFormDetails.ID = Convert.ToInt32(dtRow["ID"]);
                    ClaimFormDetails.HeaderID = Convert.ToInt32(dtRow["HeaderID"]);
                    ClaimFormDetails.Date = Convert.ToDateTime(dtRow["Date"]);
                    ClaimFormDetails._Date = Convert.ToDateTime(dtRow["Date"]).ToString("yyyy-MM-dd");
                    ClaimFormDetails.AssignmentID = Convert.ToInt32(dtRow["AssignmentID"]);
                    ClaimFormDetails.ClaimID = Convert.ToInt32(dtRow["ClaimID"]);
                    ClaimFormDetails.Description = Convert.ToString(dtRow["Description"]);
                    ClaimFormDetails.TotalAmount = Convert.ToDouble(dtRow["TotalAmount"]);
                    ClaimFormDetails.IsDeleted = Convert.ToBoolean(dtRow["IsDeleted"]);
                  
                    sno = sno + 1;
                    ClaimFormDetailsList.Add(ClaimFormDetails);
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ClaimFormManagement", "Exception occured on TranslateDataTableToClaimFormDetail, " + ex.Message);
            }

            return ClaimFormDetailsList;
        }
        private List<B1SP_Parameter> TranslateClaimFormDetailsSetupToParameterList(ClaimForm ClaimFormDetails)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {

                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = Convert.ToString(ClaimFormDetails.ID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "Status";
                parm.ParameterValue = Convert.ToString(ClaimFormDetails.Status);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "DocNum";
                parm.ParameterValue = Convert.ToString(ClaimFormDetails.DocNum);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "EmpID";
                parm.ParameterValue = Convert.ToString(ClaimFormDetails.EmpID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "EmpCode";
                parm.ParameterValue = Convert.ToString(ClaimFormDetails.EmpCode);
                parmList.Add(parm);

              
                parm = new B1SP_Parameter();
                parm.ParameterName = "DocDate";
                parm.ParameterValue = Convert.ToString(ClaimFormDetails.DocDate);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ImageFolder";
                parm.ParameterValue = Convert.ToString(ClaimFormDetails.ImageFolder);
                parmList.Add(parm);


                parm = new B1SP_Parameter();
                parm.ParameterName = "TotalAmount";
                parm.ParameterValue = Convert.ToString(ClaimFormDetails.TotalAmount);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "AdvanceReceived";
                parm.ParameterValue = Convert.ToString(ClaimFormDetails.AdvanceReceived);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "Receivable";
                parm.ParameterValue = Convert.ToString(ClaimFormDetails.Receivable);
                parmList.Add(parm);

                string attachements ="";
                if(ClaimFormDetails.Attachements!=null)
                {
                    if (ClaimFormDetails.Attachements.Count >0)
                    {
                        attachements = string.Join(",", ClaimFormDetails.Attachements.Select(x => x.Name));
                    }
                }
               
                parm = new B1SP_Parameter();
                parm.ParameterName = "Attachements";
                parm.ParameterValue = attachements;
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "IsDeleted";
                parm.ParameterValue = Convert.ToString(ClaimFormDetails.IsDeleted);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CreatedBy";
                parm.ParameterValue = Convert.ToString(ClaimFormDetails.CreatedBy);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CreatedDate";
                parm.ParameterValue = DateTime.Now.ToString("yyyy-MM-dd");
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "UpdatedBy";
                parm.ParameterValue = Convert.ToString(ClaimFormDetails.UpdatedBy);
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
                log.InputOutputDocLog("ClaimFormManagement", "Exception occured on TranslateClaimFormDetailsSetupToParameterList, " + ex.Message);
            }


            return parmList;
        }
        private List<B1SP_Parameter> TranslateClaimFormDetailsDetailToParameterList(ClaimFormDetails ClaimFormDetails)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {

                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = Convert.ToString(ClaimFormDetails.ID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "HeaderID";
                parm.ParameterValue = Convert.ToString(ClaimFormDetails.HeaderID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "Date";
                parm.ParameterValue = Convert.ToString(ClaimFormDetails._Date);
                // parm.ParameterValue = Convert.ToString(DateTime.ParseExact(ClaimFormDetails.WorkDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd"));
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "AssignmentID";
                parm.ParameterValue = Convert.ToString(ClaimFormDetails.AssignmentID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ClaimID";
                parm.ParameterValue = Convert.ToString(ClaimFormDetails.ClaimID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "Description";
                parm.ParameterValue = Convert.ToString(ClaimFormDetails.Description);
                parmList.Add(parm);
                
                parm = new B1SP_Parameter();
                parm.ParameterName = "TotalAmount";
                parm.ParameterValue = Convert.ToString(ClaimFormDetails.TotalAmount);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "IsDeleted";
                parm.ParameterValue = Convert.ToString(ClaimFormDetails.IsDeleted);
                parmList.Add(parm);
                
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ClaimFormManagement", "Exception occured on TranslateClaimFormDetailsDetailToParameterList, " + ex.Message);
            }


            return parmList;
        }
        
        private List<B1SP_Parameter> TranslateClaimFormDetailsSetupLogToParameterList(ClaimForm ClaimFormDetails,int DocID)
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
                parm.ParameterValue = Convert.ToString(ClaimFormDetails.DocNum);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "EmpID";
                parm.ParameterValue = Convert.ToString(ClaimFormDetails.EmpID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "EmpCode";
                parm.ParameterValue = Convert.ToString(ClaimFormDetails.EmpCode);
                parmList.Add(parm);
                

                parm = new B1SP_Parameter();
                parm.ParameterName = "Status_Previous";
                parm.ParameterValue = Convert.ToString(ClaimFormDetails.Status);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "DocDate_Previous";
                parm.ParameterValue = Convert.ToString(ClaimFormDetails.DocDate);
                parmList.Add(parm);
                
                parm = new B1SP_Parameter();
                parm.ParameterName = "TotalAmount_Previous";
                parm.ParameterValue = Convert.ToString(ClaimFormDetails.TotalAmount);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "AdvanceReceived_Previous";
                parm.ParameterValue = Convert.ToString(ClaimFormDetails.AdvanceReceived);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "Receivable_Previous";
                parm.ParameterValue = Convert.ToString(ClaimFormDetails.Receivable);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "IsDeleted_Previous";
                parm.ParameterValue = Convert.ToString(ClaimFormDetails.IsDeleted);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "Status_New";
                parm.ParameterValue = Convert.ToString(ClaimFormDetails.Status);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "DocDate_New";
                parm.ParameterValue = Convert.ToString(ClaimFormDetails.DocDate);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "TotalAmount_New";
                parm.ParameterValue = Convert.ToString(ClaimFormDetails.TotalAmount);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "AdvanceReceived_New";
                parm.ParameterValue = Convert.ToString(ClaimFormDetails.AdvanceReceived);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "Receivable_New";
                parm.ParameterValue = Convert.ToString(ClaimFormDetails.Receivable);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "IsDeleted_New";
                parm.ParameterValue = Convert.ToString(ClaimFormDetails.IsDeleted);
                parmList.Add(parm);


                parm = new B1SP_Parameter();
                parm.ParameterName = "CreatedBy";
                parm.ParameterValue = Convert.ToString(ClaimFormDetails.CreatedBy);
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
                log.InputOutputDocLog("ClaimFormManagement", "Exception occured on TranslateClaimFormDetailsSetupLogToParameterList, " + ex.Message);
            }


            return parmList;
        }

        private List<B1SP_Parameter> TranslateClaimFormDetailsDetailLogToParameterList(ClaimFormDetails ClaimFormDetails,int DocID)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {

                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = Convert.ToString(ClaimFormDetails.ID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "HeaderID";
                parm.ParameterValue = Convert.ToString(DocID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "Date_Previous";
                parm.ParameterValue = Convert.ToString(ClaimFormDetails._Date);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "AssignmentID_Previous";
                parm.ParameterValue = Convert.ToString(ClaimFormDetails.AssignmentID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ClaimID_Previous";
                parm.ParameterValue = Convert.ToString(ClaimFormDetails.ClaimID);
                parmList.Add(parm);


                parm = new B1SP_Parameter();
                parm.ParameterName = "Description_Previous";
                parm.ParameterValue = Convert.ToString(ClaimFormDetails.Description);
                parmList.Add(parm);
                
                
                parm = new B1SP_Parameter();
                parm.ParameterName = "TotalAmount_Previous";
                parm.ParameterValue = Convert.ToString(ClaimFormDetails.TotalAmount);
                parmList.Add(parm);


                parm = new B1SP_Parameter();
                parm.ParameterName = "IsDeleted_Previous";
                parm.ParameterValue = Convert.ToString(ClaimFormDetails.IsDeleted);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "Date_New";
                parm.ParameterValue = Convert.ToString(ClaimFormDetails._Date);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "AssignmentID_New";
                parm.ParameterValue = Convert.ToString(ClaimFormDetails.AssignmentID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ClaimID_New";
                parm.ParameterValue = Convert.ToString(ClaimFormDetails.ClaimID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "Description_New";
                parm.ParameterValue = Convert.ToString(ClaimFormDetails.Description);
                parmList.Add(parm);
                

                parm = new B1SP_Parameter();
                parm.ParameterName = "TotalAmount_New";
                parm.ParameterValue = Convert.ToString(ClaimFormDetails.TotalAmount);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "IsDeleted_New";
                parm.ParameterValue = Convert.ToString(ClaimFormDetails.IsDeleted);
                parmList.Add(parm);

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ClaimFormManagement", "Exception occured on TranslateClaimFormDetailsDetailLogToParameterList, " + ex.Message);
            }


            return parmList;
        }
    }
}