using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TMSDeloitte.DAL;
using TMSDeloitte.Helper;
using TMSDeloitte.Models;

namespace TMSDeloitte.BAL
{
    public class TimeSheetFormManagement
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
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);


                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                Common cmn = new Common();
                DataTable dt = HANADAL.GetDataTableByStoredProcedure("GetTimeSheetFormsDocNumByEmpID", parmList, "TimeSheetFormManagement");
                if (dt.Rows.Count > 0)
                {
                    docNumList = setupManagement.TranslateDataTableToDocNumList(dt);
                }

               
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("TimeSheetFormManagement", "Exception occured on GetResourceBillingRatesDocNum, " + ex.Message);
            }

            return docNumList;
        }
        public List<TimeSheetForm> GetAllTimeSheetFormByEmpID(int id)
        {
            List<TimeSheetForm> TimeSheetFormDetails = new List<TimeSheetForm>();
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
                DataSet ds = HANADAL.GetDataSetByStoredProcedure("GetTimeSheetFormByEmpID", parmList, "TimeSheetFormManagement");
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        TimeSheetFormDetails = TranslateDataTableToTimeSheetFormList(ds.Tables[0]);
                        //TimeSheetFormDetails.Detail = TranslateDataTableToTimeSheetFormDetail(ds.Tables[1]);
                    }
                }



            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("TimeSheetFormManagement", "Exception occured on GetTimeSheetFormDetailsByID ID: " + id + " , " + ex.Message);
            }

            return TimeSheetFormDetails;
        }

        public List<TimeSheetForm> GetSubmittedTimeSheetFormDetailsByEmpID(int id)
        {
            List<TimeSheetForm> TimeSheetFormDetails = new List<TimeSheetForm>();
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
                DataSet ds = HANADAL.GetDataSetByStoredProcedure("GetSubmittedTimeSheetFormDetailsByEmpID", parmList, "TimeSheetFormManagement");
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        TimeSheetFormDetails = TranslateDataTableToTimeSheetFormList(ds.Tables[0]);
                        //TimeSheetFormDetails.Detail = TranslateDataTableToTimeSheetFormDetail(ds.Tables[1]);
                    }
                }



            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("TimeSheetFormManagement", "Exception occured on GetTimeSheetFormDetailsByID ID: " + id + " , " + ex.Message);
            }

            return TimeSheetFormDetails;
        }

        public TimeSheetForm GetTimeSheetFormDetailsByID(int id)
        {
            TimeSheetForm TimeSheetFormDetails = new TimeSheetForm();
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
                DataSet ds = HANADAL.GetDataSetByStoredProcedure("GetTimeSheetFormByID", parmList, "TimeSheetFormManagement");
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        TimeSheetFormDetails = TranslateDataTableToTimeSheetForm(ds.Tables[0]);
                        TimeSheetFormDetails.Detail = TranslateDataTableToTimeSheetFormDetail(ds.Tables[1]);
                    }
                }



            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("TimeSheetFormManagement", "Exception occured on GetTimeSheetFormDetailsByID ID: " + id + " , " + ex.Message);
            }

            return TimeSheetFormDetails;
        }

        public bool AddUpdateTimeSheetFormDetailsSetup(TimeSheetForm TimeSheetFormDetails, out string msg)
        {
            bool isSuccess = false;
            bool isAddOccured = false;
            bool isUpdateOccured = false;
            bool isDeleteOccured = false;
            TimeSheetForm previousObj = new TimeSheetForm();
           Common cmn = new Common();
            string docNum = "";
            int docId = 0;
            msg = "Successfully Added/Updated";
            try
            {
                var value = TimeSheetFormDetails.DocNum;
                if (!string.IsNullOrEmpty(value))
                {
                    docNum = value;
                    docId = Convert.ToInt32(TimeSheetFormDetails.ID);
                    
                    previousObj = GetTimeSheetFormDetailsByID(docId);

                    AddHeader_Log(TimeSheetFormDetails, previousObj,docId);

                   
                }
                else
                {
                   

                    int no = 1;
                    List<string> docNumList = cmn.GetDocNum("GetTimeSheetFormsDocNum", "TimeSheetFormManagement");
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



                TimeSheetFormDetails.DocNum = docNum;
                if(docId==0)
                    TimeSheetFormDetails.Detail = TimeSheetFormDetails.Detail.Where(x => x.IsDeleted == false && x.ID == 0).ToList();
                else
                    TimeSheetFormDetails.Detail = TimeSheetFormDetails.Detail.Where(x => x.IsDeleted == false && (x.ID == 0|| x.ID>0)).ToList();
               
                ////For Form Log
                // AddTimeSheetFormDetailsSetup_Log(TimeSheetFormDetailsList, out isUpdateOccured);
                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                DataTable dtHeader = HANADAL.AddUpdateDataByStoredProcedure("AddUpdateTimeSheetForm", TranslateTimeSheetFormDetailsSetupToParameterList(TimeSheetFormDetails), "TimeSheetFormManagement");
                if (dtHeader.Rows.Count == 0)
                    throw new Exception("Exception occured when AddUpdateTimeSheetFormDetailsSetup , ID:" + TimeSheetFormDetails.ID + " , Emp ID:" + TimeSheetFormDetails.EmpID +" , Year: " + TimeSheetFormDetails.Year + " , PeriodID: " + TimeSheetFormDetails.Period);
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
                //    //AddTimeSheetFormDetails_Log(TimeSheetFormDetails, out isUpdateOccured);
                //    DeleteTimeSheetFormDetailsDetailByHeaderID(docId); 
                //}

                TimeSheetFormDetails.Detail.Select(c => { c.HeaderID = docId; return c; }).ToList();

                foreach (var list in TimeSheetFormDetails.Detail)
                {
                    try
                    {
                        if(list.ID>0)
                        {
                            var previousObjectDetail = previousObj.Detail.Where(x => x.ID == list.ID).FirstOrDefault();

                            AddDetail_Log(list, previousObjectDetail, docId);
                        }

                        HANADAL.AddUpdateDataByStoredProcedure("AddUpdateTimeSheetFormDetail", TranslateTimeSheetFormDetailsDetailToParameterList(list), "TimeSheetFormManagement");
                       
                    }
                    catch (Exception ex)
                    {
                        isSuccess = false;
                        Log log = new Log();
                        log.LogFile(ex.Message);
                        log.InputOutputDocLog("TimeSheetFormManagement", "Exception occured in foreach loop AddUpdateTimeSheetFormDetail, " + ex.Message);
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
                            List<TimeSheetFormDetails> missingList = previousObj.Detail.Where(n => !TimeSheetFormDetails.Detail.Any(o => o.ID == n.ID && o.IsDeleted == n.IsDeleted)).ToList();
                            foreach (var item in missingList)
                            {
                                item.IsDeleted = true;

                                var previousObjectDetail = previousObj.Detail.Where(x => x.ID == item.ID).FirstOrDefault();

                                AddDetail_Log(item, previousObjectDetail, docId);


                               
                                isDeleteOccured = true;
                                //AddUserAlertSetup_Log(user, previous_item, DocId);
                                HANADAL.AddUpdateDataByStoredProcedure("AddUpdateTimeSheetFormDetail", TranslateTimeSheetFormDetailsDetailToParameterList(item), "TimeSheetFormManagement");

                            }
                        }
                    }
                   

                   
                }

                //For Master Log
                if (TimeSheetFormDetails.ID > 0)
                    isAddOccured = true;
                if (Convert.ToBoolean(TimeSheetFormDetails.IsDeleted))
                    isDeleteOccured = true;


                int CreatedBy = Convert.ToInt32(TimeSheetFormDetails.CreatedBy);
                Task.Run(() => cmn.AddMasterLog(isAddOccured, isUpdateOccured, isDeleteOccured, nameof(Enums.FormsName.TimeSheetForm), CreatedBy, "TimeSheetFormManagement"));
                //End MAster Log


                isSuccess = true;

              


                if (TimeSheetFormDetails.Status==2)
                {
                    Encrypt_Decrypt security = new Encrypt_Decrypt();
                    //For Notification and Email when time sheet submit
                    System.Web.Routing.RequestContext requestContext = HttpContext.Current.Request.RequestContext;
                    //string lnkHref = new System.Web.Mvc.UrlHelper(requestContext).Action("GetApprovalDecision", "Home", new { empID = "EncryptedID", docID = security.EncryptURLString(docId.ToString()), docType= security.EncryptURLString("Timesheet") }, HttpContext.Current.Request.Url.Scheme);
                    string lnkHref = new System.Web.Mvc.UrlHelper(requestContext).Action("GetApprovalDecision", "Home", new { empID = "EncryptedID", docID = security.EncryptString(docId.ToString()), docType = security.EncryptString("Timesheet") }, HttpContext.Current.Request.Url.Scheme);
                    cmn = new Common();
                    Task.Run(() => cmn.SndNotificationAndEmail(Convert.ToInt32(TimeSheetFormDetails.CreatedBy), TimeSheetFormDetails.Period, TimeSheetFormDetails.Year, lnkHref, "TimeSheetFormManagement"));
                }

            }
            catch (Exception ex)
            {
                isSuccess = false;
                msg = "Exception occured on Add/Update";
                isSuccess = false;
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("TimeSheetFormManagement", "Exception occured on AddUpdateAssignmentCostSetup, " + ex.Message);
            }

            return isSuccess;
        }

        public int GetEmpUsedLeave(int empID,int assignmentID,int taskID)
        {
            List<string> docNumList = new List<string>();
            BAL.Common setupManagement = new BAL.Common();
            try
            {
                List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "EmpID";
                parm.ParameterValue = Convert.ToString(empID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "AssignmentID";
                parm.ParameterValue = Convert.ToString(assignmentID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "TaskID";
                parm.ParameterValue = Convert.ToString(taskID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);


                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                Common cmn = new Common();
                DataTable dt = HANADAL.GetDataTableByStoredProcedure("GetEmpUsedLeave", parmList, "TimeSheetFormManagement");
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dtRow in dt.Rows)
                    {
                        return Convert.ToInt32(dtRow["Count"]);
                    }
                }


            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("TimeSheetFormManagement", "Exception occured on GetEmpUsedLeave, " + ex.Message);
            }

            return 0;
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
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);
                parm = new B1SP_Parameter();
                parm.ParameterName = "fromDate";
                parm.ParameterValue = Convert.ToString(fromDate.ToString("yyyy-MM-dd"));
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);
                parm = new B1SP_Parameter();
                parm.ParameterName = "toDate";
                parm.ParameterValue = Convert.ToString(toDate.ToString("yyyy-MM-dd"));
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                DataTable dt_UserProfile = HANADAL.GetDataTableByStoredProcedure("ValidateTimeSheetPeriodDateRange", parmList, "TimeSheetFormManagement");
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
                log.InputOutputDocLog("TimeSheetFormManagement", "Exception occured on ValidateDateRange, " + ex.Message);
            }
            return isSuccess;
        }

        public TimeSheetForm DeleteTimeSheetFormDetailsDetailByHeaderID(int HeaderID)
        {
            TimeSheetForm TimeSheetFormDetails = new TimeSheetForm();
            try
            {
                List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "HeaderID";
                parm.ParameterValue = Convert.ToString(HeaderID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                Common cmn = new Common();
                HANADAL.AddUpdateDataByStoredProcedure("DeleteTimeSheetPeriodDetail", parmList, "TimeSheetFormManagement");
                //DataSet ds = HANADAL.GetDataSetByStoredProcedure("DeleteTimeSheetPeriodDetail", parmList, "TimeSheetFormManagement");
                //if (ds.Tables.Count > 0)
                //{
                //    if (ds.Tables[0].Rows.Count > 0)
                //    {
                //        TimeSheetFormDetails = TranslateDataTableToTimeSheetForm(ds.Tables[0]);
                //        TimeSheetFormDetails.Detail = TranslateDataTableToTimeSheetFormDetail(ds.Tables[1]);
                //    }
                //}



            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("TimeSheetFormManagement", "Exception occured on DeleteTimeSheetFormDetailsDetailByHeaderID, ID: " + HeaderID + " , Exception: " + ex.Message);
            }

            return TimeSheetFormDetails;
        }

        public DataTable GetTimeSheetAllDocumentsList()
        {
            DataTable dt = new DataTable();
            Common cmn = new Common();
            try

            {
                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                dt = HANADAL.GetDataTableByStoredProcedure("GetTimeSheetFormAllDocuments", "TimeSheetFormManagement");
                dt = cmn.RemoveDuplicateRows(dt, "DocNum");
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("TimeSheetFormManagement", "Exception occured on GetTimeSheetAllDocumentsList, " + ex.Message);
            }

            return dt;
        }

        public TimeSheetForm GetTimeSheetByDocNum(string docNo,int empID)
        {
            TimeSheetForm obj = new TimeSheetForm();
            try
            {

                List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();

                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "DocNum";
                parm.ParameterValue = docNo;
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "EmpID";
                parm.ParameterValue =Convert.ToString(empID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                Common cmn = new Common();
                DataSet ds = HANADAL.GetDataSetByStoredProcedure("GetTimeSheetFormByDocNum", parmList, "TimeSheetFormManagement");
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        obj = TranslateDataTableToTimeSheetForm(ds.Tables[0]);
                        obj.Detail = TranslateDataTableToTimeSheetFormDetail(ds.Tables[1]);
                    }
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("TimeSheetFormManagement", "Exception occured on GetResourceBillingRatesByDocNum DocNum: " + docNo + " , " + ex.Message);
            }

            return obj;
        }

        public TimeSheetForm GetTimeSheetByYear(string year)
        {
            TimeSheetForm obj = new TimeSheetForm();
            try
            {

                List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();

                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "Year";
                parm.ParameterValue = year;
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                Common cmn = new Common();
                DataSet ds = HANADAL.GetDataSetByStoredProcedure("GetTimeSheetPeriodByPeriod", parmList, "TimeSheetFormManagement");
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        obj = TranslateDataTableToTimeSheetForm(ds.Tables[0]);
                        obj.Detail = TranslateDataTableToTimeSheetFormDetail(ds.Tables[1]);
                    }
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("TimeSheetFormManagement", "Exception occured on GetTimeSheetByYear year: " + year + " , " + ex.Message);
            }

            return obj;
        }


        public DataTable GetAssignmentByEmpID(int id)
        {
            DataTable dt = new DataTable();
            BAL.Common setupManagement = new BAL.Common();
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
                dt = HANADAL.GetDataTableByStoredProcedure("GetAssignmentByEmpID", parmList, "TimeSheetFormManagement");

                DataRow row = dt.NewRow();
                row["ID"] = Convert.ToInt32(Enums.General.Nonchargeable_Task);
                row["Name"] = "Non Chargeable task";
                dt.Rows.Add(row);

                row = dt.NewRow();
                row["ID"] = Convert.ToInt32(Enums.General.Absence_Management_Internal);
                row["Name"] = "Absence Management Internal";
                dt.Rows.Add(row);
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("TimeSheetFormManagement", "Exception occured on GetResourceBillingRatesDocNum, " + ex.Message);
            }

            return dt;
        }

        public DataTable GetAssignmentTaskByEmpID(int id)
        {
            DataTable dt = new DataTable();
            BAL.Common setupManagement = new BAL.Common();
            try
            {
                int Nonchargeable_TaskID= Convert.ToInt32(Enums.General.Nonchargeable_Task);
                int Absence_Management_InternalID= Convert.ToInt32(Enums.General.Absence_Management_Internal);

                List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = Convert.ToString(id);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);


                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                Common cmn = new Common();
                dt = HANADAL.GetDataTableByStoredProcedure("GetAssignmentTaskByEmpID", parmList, "TimeSheetFormManagement");
                dt.Columns.Add("isLeave", typeof(System.Boolean));
                NCTaskAssignmentManagement mgt = new NCTaskAssignmentManagement();
                DataTable dtTask = mgt.GetNCTaskAssignmentDeatilByEmpID(id);
                foreach (DataRow dtRow in dt.Rows)
                {
                    dtRow["isLeave"] = false;
                }
                foreach (DataRow dtRow in dtTask.Rows)
                {
                    DataRow row = dt.NewRow();
                    row["ID"] = Convert.ToInt32(Convert.ToString(Nonchargeable_TaskID) + Convert.ToString(dtRow["ID"])); 
                    row["Name"] = Convert.ToString(dtRow["NCTASKS"]);
                    row["AssignmentID"] = Nonchargeable_TaskID;
                    row["NonChargeable"] = true;

                    row["TotalAllowed"] = "0";
                    row["CarryForward"] = "0";
                    row["UseD"] = "0";
                    row["Balance"] = "0";
                    row["isLeave"] = false;
                    dt.Rows.Add(row);
                }
                UserManagement user = new UserManagement();
                UserProfile userProfile = user.GetUserByID(id);
                if(userProfile!=null)
                {
                    HCMOneManagement hcm1Mgt = new HCMOneManagement();
                    List<HCM_EmployeeLeaves> leaveList = hcm1Mgt.GetHCMUserLeaves(userProfile.ID, userProfile.HCMOneID);
                    foreach (var item in leaveList)
                    {
                            DataRow row = dt.NewRow();
                            row["ID"] = Convert.ToInt32(Convert.ToString(Absence_Management_InternalID)+ Convert.ToString(item.ID));
                            row["Name"] = Convert.ToString(item.LeaveType);
                            row["AssignmentID"] = Absence_Management_InternalID;
                            row["NonChargeable"] = false;
                            row["TotalAllowed"] = item.TotalAllowed;
                            row["CarryForward"] = item.CarryForward;
                            row["UseD"] = item.UseD;
                            row["Balance"] = item.Balance;
                            row["isLeave"] = true;
                            dt.Rows.Add(row);
                       
                    }
                }



            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("TimeSheetFormManagement", "Exception occured on GetAssignmentTaskByEmpID, " + ex.Message);
            }

            return dt;
        }
        public DataTable GetAssignmentLocationByEmpID(int id)
        {
            DataTable dt = new DataTable();
            BAL.Common setupManagement = new BAL.Common();
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
                dt = HANADAL.GetDataTableByStoredProcedure("GetAssignmentLocationByEmpID", parmList, "TimeSheetFormManagement");



            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("TimeSheetFormManagement", "Exception occured on GetAssignmentLocationByEmpID, " + ex.Message);
            }

            return dt;
        }

        public void AddHeader_Log(TimeSheetForm newObject, TimeSheetForm previousObject, int docID)
        {
            try
            {
                if (newObject.ID > 0)
                {
                    List<B1SP_Parameter> paramList = TranslateTimeSheetFormDetailsSetupLogToParameterList(newObject, docID);
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
                                    //PreviousValue = Convert.ToString(((Enums.AlertSetup)PreviousValue));
                                    //NewValue = Convert.ToString(((Enums.AlertSetup)NewValue));

                                    paramList.Where(x => x.ParameterName == "Status_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                    paramList.Where(x => x.ParameterName == "Status_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();

                                    break;

                                case "Year":
                                    isChangeOccured = true;
                                    paramList.Where(x => x.ParameterName == "Year_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                    paramList.Where(x => x.ParameterName == "Year_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();

                                    break;

                                case "Period":
                                    isChangeOccured = true;
                                    paramList.Where(x => x.ParameterName == "Period_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                    paramList.Where(x => x.ParameterName == "Period_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();
                                    break;

                                case "FrequencyType":
                                    isChangeOccured = true;
                                    paramList.Where(x => x.ParameterName == "FrequencyType_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                    paramList.Where(x => x.ParameterName == "FrequencyType_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();
                                    break;

                                case "StandardHours":
                                    isChangeOccured = true;
                                    paramList.Where(x => x.ParameterName == "StandardHours_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                    paramList.Where(x => x.ParameterName == "StandardHours_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();
                                    break;

                                case "NonChargeableHours":
                                    isChangeOccured = true;
                                    paramList.Where(x => x.ParameterName == "NonChargeableHours_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                    paramList.Where(x => x.ParameterName == "NonChargeableHours_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();
                                    break;

                                case "ChargeableHours":
                                    isChangeOccured = true;
                                    paramList.Where(x => x.ParameterName == "ChargeableHours_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                    paramList.Where(x => x.ParameterName == "ChargeableHours_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();
                                    break;

                                case "OverTimeHours":
                                    isChangeOccured = true;
                                    paramList.Where(x => x.ParameterName == "OverTimeHours_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                    paramList.Where(x => x.ParameterName == "OverTimeHours_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();
                                    break;

                                case "TotalHours":
                                    isChangeOccured = true;
                                    paramList.Where(x => x.ParameterName == "TotalHours_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                    paramList.Where(x => x.ParameterName == "TotalHours_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();
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
                            Task.Run(() => HANADAL.AddUpdateDataByStoredProcedure("AddUpdateTimeSheetForm_Log", paramList, "TimeSheetFormManagement"));
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("TimeSheetFormManagement", "Exception occured on AddHeader_Log, " + ex.Message);
            }
        }

        public void AddDetail_Log(TimeSheetFormDetails newObject, TimeSheetFormDetails previousObject, int docID)
        {
            try
            {
                if (newObject.ID > 0)
                {
                    List<B1SP_Parameter> paramList = TranslateTimeSheetFormDetailsDetailLogToParameterList(newObject, docID);
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


                                case "_WorkDate":
                                    isChangeOccured = true;
                                    //PreviousValue = Convert.ToString(((Enums.AlertSetup)PreviousValue));
                                    //NewValue = Convert.ToString(((Enums.AlertSetup)NewValue));

                                    paramList.Where(x => x.ParameterName == "WorkDate_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                    paramList.Where(x => x.ParameterName == "WorkDate_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();

                                    break;

                                case "AssignmentID":
                                    isChangeOccured = true;
                                    paramList.Where(x => x.ParameterName == "AssignmentID_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                    paramList.Where(x => x.ParameterName == "AssignmentID_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();

                                    break;

                                case "TaskID":
                                    isChangeOccured = true;
                                    paramList.Where(x => x.ParameterName == "TaskID_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                    paramList.Where(x => x.ParameterName == "TaskID_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();
                                    break;

                                case "LocationID":
                                    isChangeOccured = true;
                                    paramList.Where(x => x.ParameterName == "LocationID_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                    paramList.Where(x => x.ParameterName == "LocationID_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();
                                    break;

                                case "WorkHours":
                                    isChangeOccured = true;
                                    paramList.Where(x => x.ParameterName == "WorkHours_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                    paramList.Where(x => x.ParameterName == "WorkHours_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();
                                    break;

                                case "Description":
                                    if (NewValue != null)
                                    {
                                        isChangeOccured = true;
                                        paramList.Where(x => x.ParameterName == "Description_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                        paramList.Where(x => x.ParameterName == "Description_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();

                                    }
                                    break;

                                case "OnSite":
                                    isChangeOccured = true;
                                    paramList.Where(x => x.ParameterName == "OnSite_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                    paramList.Where(x => x.ParameterName == "OnSite_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();
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
                            Task.Run(() => HANADAL.AddUpdateDataByStoredProcedure("AddUpdateTimeSheetFormDetail_Log", paramList, "TimeSheetFormManagement"));
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("TimeSheetFormManagement", "Exception occured on AddDetail_Log, " + ex.Message);
            }
        }


        public bool UpdateTimeSheetFormStatus(TimeSheetForm TimeSheetFormDetails, out string msg)
        {
            bool isSuccess = false;
            bool isAddOccured = false;
            bool isUpdateOccured = false;
            bool isDeleteOccured = false;
            TimeSheetForm previousObj = new TimeSheetForm();
            Common cmn = new Common();
            string docNum = "";
            int docId = 0;
            msg = "Successfully Updated";

            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            B1SP_Parameter parm = new B1SP_Parameter();

            try
            {
                if(TimeSheetFormDetails==null)
                {
                   
                    msg = "Document number not found!";
                    return false;
                }
                var value = TimeSheetFormDetails._DocNum;
                if (string.IsNullOrEmpty(value))
                {

                    msg = "Document number not found!";
                    return false;
                }

              
                if (!string.IsNullOrEmpty(value))
                {
                    Encrypt_Decrypt security = new Encrypt_Decrypt();

                    docId = Convert.ToInt32(TimeSheetFormDetails.ID);

                    parm = new B1SP_Parameter();
                    parm.ParameterName = "ID";
                    parm.ParameterValue =Convert.ToString(docId);// Convert.ToString(TimeSheetFormDetails.ID);
                    parm.ParameterType = DBTypes.Int32.ToString();
                    parmList.Add(parm);

                    parm = new B1SP_Parameter();
                    parm.ParameterName = "Status";
                    parm.ParameterValue = Convert.ToString(TimeSheetFormDetails.Status);
                    parm.ParameterType = DBTypes.Int32.ToString();
                    parmList.Add(parm);

                    parm = new B1SP_Parameter();
                    parm.ParameterName = "DocNum";
                    //parm.ParameterValue = Convert.ToString(security.DecryptString(TimeSheetFormDetails._DocNum));
                    parm.ParameterValue = Convert.ToString(TimeSheetFormDetails._DocNum);
                    parm.ParameterType = DBTypes.String.ToString();
                    parmList.Add(parm);

                    parm = new B1SP_Parameter();
                    parm.ParameterName = "EmpID";
                    //parm.ParameterValue = Convert.ToString(security.DecryptString(TimeSheetFormDetails._EmpID));
                    parm.ParameterValue = Convert.ToString(TimeSheetFormDetails._EmpID);
                    parm.ParameterType = DBTypes.String.ToString();
                    parmList.Add(parm);

                    parm = new B1SP_Parameter();
                    parm.ParameterName = "UpdatedBy";
                    parm.ParameterValue = Convert.ToString(TimeSheetFormDetails.UpdatedBy);
                    parm.ParameterType = DBTypes.Int32.ToString();
                    parmList.Add(parm);

                    parm = new B1SP_Parameter();
                    parm.ParameterName = "UpdatedDate";
                    parm.ParameterValue = DateTime.Now.ToString("yyyy-MM-dd");
                    parm.ParameterType = DBTypes.String.ToString();
                    parmList.Add(parm);

                    previousObj = GetTimeSheetFormDetailsByID(docId);

                    AddHeader_Log(TimeSheetFormDetails, previousObj, docId);

                    HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                    DataTable dtHeader = HANADAL.AddUpdateDataByStoredProcedure("UpdateTimeSheetFormStatus", parmList, "TimeSheetFormManagement");
                    if (dtHeader.Rows.Count == 0)
                        throw new Exception("Exception occured when cencel document , ID:" + TimeSheetFormDetails.ID + " , Emp ID:" + TimeSheetFormDetails.EmpID);
                   

                    //For Master Log
                    isUpdateOccured = true;
                    int CreatedBy = Convert.ToInt32(TimeSheetFormDetails.CreatedBy);
                    Task.Run(() => cmn.AddMasterLog(isAddOccured, isUpdateOccured, isDeleteOccured, nameof(Enums.FormsName.TimeSheetForm), CreatedBy, "TimeSheetFormManagement"));
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
                log.InputOutputDocLog("TimeSheetFormManagement", "Exception occured on UpdateTimeSheetFormStatus, " + ex.Message);
            }

            return isSuccess;
        }

        private List<TimeSheetForm> TranslateDataTableToTimeSheetFormList(DataTable dt)
        {
            Common cmn = new Common();
            List<TimeSheetForm> list = new List<TimeSheetForm>();
            List<TimseSheetStatus> statusList = cmn.GetTimeSheetFormStatusList();
            try
            {
                foreach (DataRow dtRow in dt.Rows)
                {
                    TimeSheetForm TimeSheetFormDetails = new TimeSheetForm();
                    TimeSheetFormDetails.ID = Convert.ToInt32(dtRow["ID"]);
                    TimeSheetFormDetails.Status = Convert.ToInt32(dtRow["Status"]);
                    var status = statusList.Where(x => x.ID == TimeSheetFormDetails.Status).FirstOrDefault();
                    if (status != null)
                        TimeSheetFormDetails.StatusName = status.Name;

                    TimeSheetFormDetails.DocNum = Convert.ToString(dtRow["DocNum"]);
                    TimeSheetFormDetails.EmpID = Convert.ToInt32(dtRow["EmpID"]);
                    TimeSheetFormDetails.EmpCode = Convert.ToString(dtRow["EmpCode"]);
                    TimeSheetFormDetails.Year = Convert.ToInt32(dtRow["Year"]);
                    TimeSheetFormDetails.Period = Convert.ToInt32(dtRow["Period"]);
                    TimeSheetFormDetails.StandardHours = Convert.ToDouble(dtRow["StandardHours"]);
                    TimeSheetFormDetails.NonChargeableHours = Convert.ToDouble(dtRow["NonChargeableHours"]);
                    TimeSheetFormDetails.ChargeableHours = Convert.ToDouble(dtRow["ChargeableHours"]);
                    TimeSheetFormDetails.OverTimeHours = Convert.ToDouble(dtRow["OverTimeHours"]);
                    TimeSheetFormDetails.LeaveHours = Convert.ToDouble(dtRow["LeaveHours"]);
                    TimeSheetFormDetails.TotalHours = Convert.ToDouble(dtRow["TotalHours"]);
                    TimeSheetFormDetails.CreatedBy = Convert.ToInt32(dtRow["CreatedBy"]);
                    TimeSheetFormDetails.CreatedDate = Convert.ToDateTime(dtRow["CREATEDDATE"]);
                    if (dtRow["UpdatedBy"] != DBNull.Value)
                        TimeSheetFormDetails.UpdatedBy = Convert.ToInt32(dtRow["UpdatedBy"]);
                    if (dtRow["UpdatedDate"] != DBNull.Value)
                        TimeSheetFormDetails.UpdatedDate = Convert.ToDateTime(dtRow["UpdatedDate"]);

                    TimeSheetFormDetails.IsDeleted = Convert.ToBoolean(dtRow["IsDeleted"]);

                    BAL.TimeSheetPeriodManagement mgt = new TimeSheetPeriodManagement();

                   TimeSheetPeriods detail= mgt.GetTimeSheetPeriodDetailByID(TimeSheetFormDetails.Period);
                    if(detail!=null)
                    {
                        TimeSheetFormDetails.PeriodText = detail.Period;
                        TimeSheetFormDetails.FromDate = detail._Monday;
                        TimeSheetFormDetails.ToDate = detail._Friday;
                    }

                    list.Add(TimeSheetFormDetails);
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("TimeSheetFormManagement", "Exception occured on TranslateDataTableToTimeSheetFormList, " + ex.Message);
            }

            return list;
        }
        private TimeSheetForm TranslateDataTableToTimeSheetForm(DataTable dt)
        {
            TimeSheetForm TimeSheetFormDetails = new TimeSheetForm();
            Common cmn = new Common();
            List<TimseSheetStatus> statusList = cmn.GetTimeSheetFormStatusList();
            try
            {
                foreach (DataRow dtRow in dt.Rows)
                {

                    TimeSheetFormDetails.ID = Convert.ToInt32(dtRow["ID"]);
                    TimeSheetFormDetails.Status = Convert.ToInt32(dtRow["Status"]);
                    var status = statusList.Where(x => x.ID == TimeSheetFormDetails.Status).FirstOrDefault();
                    if (status != null)
                        TimeSheetFormDetails.StatusName = status.Name;

                    TimeSheetFormDetails.DocNum = Convert.ToString(dtRow["DocNum"]);
                    TimeSheetFormDetails.EmpID = Convert.ToInt32(dtRow["EmpID"]);
                    TimeSheetFormDetails.EmpCode = Convert.ToString(dtRow["EmpCode"]);
                    TimeSheetFormDetails.Year = Convert.ToInt32(dtRow["Year"]);
                    TimeSheetFormDetails.Period = Convert.ToInt32(dtRow["Period"]);
                    if(TimeSheetFormDetails.Period!=0)
                    {
                        TimeSheetPeriodManagement mgt = new TimeSheetPeriodManagement();
                        TimeSheetPeriods period = mgt.GetTimeSheetPeriodDetailByID(TimeSheetFormDetails.Period);
                        TimeSheetFormDetails.PeriodText = period.Period;
                        TimeSheetFormDetails._Monday = period._Monday;
                        TimeSheetFormDetails.StdHoursInWeek = period.StdHoursInWeek;
                    }
                   
                    TimeSheetFormDetails.StandardHours = Convert.ToDouble(dtRow["StandardHours"]);
                    TimeSheetFormDetails.NonChargeableHours = Convert.ToDouble(dtRow["NonChargeableHours"]);
                    TimeSheetFormDetails.ChargeableHours = Convert.ToDouble(dtRow["ChargeableHours"]);
                    TimeSheetFormDetails.OverTimeHours = Convert.ToDouble(dtRow["OverTimeHours"]);
                    TimeSheetFormDetails.LeaveHours = Convert.ToDouble(dtRow["LeaveHours"]);
                    TimeSheetFormDetails.TotalHours = Convert.ToDouble(dtRow["TotalHours"]);
                    TimeSheetFormDetails.CreatedBy = Convert.ToInt32(dtRow["CreatedBy"]);
                    TimeSheetFormDetails.CreatedDate = Convert.ToDateTime(dtRow["CREATEDDATE"]);
                    if (dtRow["UpdatedBy"] != DBNull.Value)
                        TimeSheetFormDetails.UpdatedBy = Convert.ToInt32(dtRow["UpdatedBy"]);
                    if (dtRow["UpdatedDate"] != DBNull.Value)
                        TimeSheetFormDetails.UpdatedDate = Convert.ToDateTime(dtRow["UpdatedDate"]);

                    TimeSheetFormDetails.IsDeleted = Convert.ToBoolean(dtRow["IsDeleted"]);
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("TimeSheetFormManagement", "Exception occured on TranslateDataTableToTimeSheetForm, " + ex.Message);
            }

            return TimeSheetFormDetails;
        }
        private List<TimeSheetFormDetails> TranslateDataTableToTimeSheetFormDetail(DataTable dt)
        {
            List<TimeSheetFormDetails> TimeSheetFormDetailsList = new List<TimeSheetFormDetails>();

            try
            {
                int sno = 1;
                foreach (DataRow dtRow in dt.Rows)
                {
                    TimeSheetFormDetails TimeSheetFormDetails = new TimeSheetFormDetails();
                    TimeSheetFormDetails.SNo = sno;
                    TimeSheetFormDetails.KEY = Guid.NewGuid().ToString();
                    TimeSheetFormDetails.ID = Convert.ToInt32(dtRow["ID"]);
                    TimeSheetFormDetails.HeaderID = Convert.ToInt32(dtRow["HeaderID"]);
                    TimeSheetFormDetails.WorkDate = Convert.ToDateTime(dtRow["WorkDate"]).ToString("dd/MM/yyyy");
                    TimeSheetFormDetails._WorkDate = Convert.ToDateTime(dtRow["WorkDate"]).ToString("yyyy-MM-dd");
                    TimeSheetFormDetails.AssignmentID = Convert.ToInt32(dtRow["AssignmentID"]);
                    TimeSheetFormDetails.TaskID = Convert.ToInt32(dtRow["TaskID"]);
                    TimeSheetFormDetails.LocationID = Convert.ToInt32(dtRow["LocationID"]);
                    TimeSheetFormDetails.WorkHours = Convert.ToDouble(dtRow["WorkHours"]);
                    TimeSheetFormDetails.Description = Convert.ToString(dtRow["Description"]);
                    TimeSheetFormDetails.IsDeleted = Convert.ToBoolean(dtRow["IsDeleted"]);
                    TimeSheetFormDetails.OnSite = Convert.ToBoolean(dtRow["OnSite"]);
                    sno = sno + 1;
                    TimeSheetFormDetailsList.Add(TimeSheetFormDetails);
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("TimeSheetFormManagement", "Exception occured on TranslateDataTableToTimeSheetFormDetail, " + ex.Message);
            }

            return TimeSheetFormDetailsList;
        }
        private List<B1SP_Parameter> TranslateTimeSheetFormDetailsSetupToParameterList(TimeSheetForm TimeSheetFormDetails)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {

                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = Convert.ToString(TimeSheetFormDetails.ID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "Status";
                parm.ParameterValue = Convert.ToString(TimeSheetFormDetails.Status);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "DocNum";
                parm.ParameterValue = Convert.ToString(TimeSheetFormDetails.DocNum);
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "EmpID";
                parm.ParameterValue = Convert.ToString(TimeSheetFormDetails.EmpID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "EmpCode";
                parm.ParameterValue = Convert.ToString(TimeSheetFormDetails.EmpCode);
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "Year";
                parm.ParameterValue = Convert.ToString(TimeSheetFormDetails.Year);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "Period";
                parm.ParameterValue = Convert.ToString(TimeSheetFormDetails.Period);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "StandardHours";
                parm.ParameterValue = Convert.ToString(TimeSheetFormDetails.StandardHours);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "NonChargeableHours";
                parm.ParameterValue = Convert.ToString(TimeSheetFormDetails.NonChargeableHours);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ChargeableHours";
                parm.ParameterValue = Convert.ToString(TimeSheetFormDetails.ChargeableHours);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "OverTimeHours";
                parm.ParameterValue = Convert.ToString(TimeSheetFormDetails.OverTimeHours);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "LeaveHours";
                parm.ParameterValue = Convert.ToString(TimeSheetFormDetails.LeaveHours);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "TotalHours";
                parm.ParameterValue = Convert.ToString(TimeSheetFormDetails.TotalHours);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "IsDeleted";
                parm.ParameterValue = Convert.ToString(TimeSheetFormDetails.IsDeleted);
                parm.ParameterType = DBTypes.Bool.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CreatedBy";
                parm.ParameterValue = Convert.ToString(TimeSheetFormDetails.CreatedBy);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CreatedDate";
                parm.ParameterValue = DateTime.Now.ToString("yyyy-MM-dd");
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "UpdatedBy";
                parm.ParameterValue = Convert.ToString(TimeSheetFormDetails.UpdatedBy);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "UpdatedDate";
                parm.ParameterValue = DateTime.Now.ToString("yyyy-MM-dd");
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);


            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("TimeSheetFormManagement", "Exception occured on TranslateTimeSheetFormDetailsSetupToParameterList, " + ex.Message);
            }


            return parmList;
        }
        private List<B1SP_Parameter> TranslateTimeSheetFormDetailsDetailToParameterList(TimeSheetFormDetails TimeSheetFormDetails)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {

                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = Convert.ToString(TimeSheetFormDetails.ID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "HeaderID";
                parm.ParameterValue = Convert.ToString(TimeSheetFormDetails.HeaderID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "WorkDate";
                parm.ParameterValue = Convert.ToString(TimeSheetFormDetails._WorkDate);
                // parm.ParameterValue = Convert.ToString(DateTime.ParseExact(TimeSheetFormDetails.WorkDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd"));
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "AssignmentID";
                parm.ParameterValue = Convert.ToString(TimeSheetFormDetails.AssignmentID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "TaskID";
                parm.ParameterValue = Convert.ToString(TimeSheetFormDetails.TaskID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "LocationID";
                parm.ParameterValue = Convert.ToString(TimeSheetFormDetails.LocationID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "WorkHours";
                parm.ParameterValue = Convert.ToString(TimeSheetFormDetails.WorkHours);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "Description";
                parm.ParameterValue = Convert.ToString(TimeSheetFormDetails.Description);
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "OnSite";
                parm.ParameterValue = Convert.ToString(TimeSheetFormDetails.OnSite);
                parm.ParameterType = DBTypes.Bool.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "IsDeleted";
                parm.ParameterValue = Convert.ToString(TimeSheetFormDetails.IsDeleted);
                parm.ParameterType = DBTypes.Bool.ToString();
                parmList.Add(parm);
                
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("TimeSheetFormManagement", "Exception occured on TranslateTimeSheetFormDetailsDetailToParameterList, " + ex.Message);
            }


            return parmList;
        }
        
        private List<B1SP_Parameter> TranslateTimeSheetFormDetailsSetupLogToParameterList(TimeSheetForm TimeSheetFormDetails,int DocID)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {

                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = Convert.ToString(0);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "DocID";
                parm.ParameterValue = Convert.ToString(DocID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "DocNum";
                parm.ParameterValue = Convert.ToString(TimeSheetFormDetails.DocNum);
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "EmpID";
                parm.ParameterValue = Convert.ToString(TimeSheetFormDetails.EmpID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);


                parm = new B1SP_Parameter();
                parm.ParameterName = "Status_Previous";
                parm.ParameterValue = Convert.ToString(TimeSheetFormDetails.Status);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "Year_Previous";
                parm.ParameterValue = Convert.ToString(TimeSheetFormDetails.Year);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "Period_Previous";
                parm.ParameterValue = Convert.ToString(TimeSheetFormDetails.Period);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "StandardHours_Previous";
                parm.ParameterValue = Convert.ToString(TimeSheetFormDetails.StandardHours);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "NonChargeableHours_Previous";
                parm.ParameterValue = Convert.ToString(TimeSheetFormDetails.NonChargeableHours);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ChargeableHours_Previous";
                parm.ParameterValue = Convert.ToString(TimeSheetFormDetails.ChargeableHours);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "OverTimeHours_Previous";
                parm.ParameterValue = Convert.ToString(TimeSheetFormDetails.OverTimeHours);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "TotalHours_Previous";
                parm.ParameterValue = Convert.ToString(TimeSheetFormDetails.TotalHours);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);
                
                parm = new B1SP_Parameter();
                parm.ParameterName = "IsDeleted_Previous";
                parm.ParameterValue = Convert.ToString(TimeSheetFormDetails.IsDeleted);
                parm.ParameterType = DBTypes.Bool.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "Status_New";
                parm.ParameterValue = Convert.ToString(TimeSheetFormDetails.Status);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "Year_New";
                parm.ParameterValue = Convert.ToString(TimeSheetFormDetails.Year);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "Period_New";
                parm.ParameterValue = Convert.ToString(TimeSheetFormDetails.Period);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "StandardHours_New";
                parm.ParameterValue = Convert.ToString(TimeSheetFormDetails.StandardHours);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "NonChargeableHours_New";
                parm.ParameterValue = Convert.ToString(TimeSheetFormDetails.NonChargeableHours);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ChargeableHours_New";
                parm.ParameterValue = Convert.ToString(TimeSheetFormDetails.ChargeableHours);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "OverTimeHours_New";
                parm.ParameterValue = Convert.ToString(TimeSheetFormDetails.OverTimeHours);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "TotalHours_New";
                parm.ParameterValue = Convert.ToString(TimeSheetFormDetails.TotalHours);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "IsDeleted_New";
                parm.ParameterValue = Convert.ToString(TimeSheetFormDetails.IsDeleted);
                parm.ParameterType = DBTypes.Bool.ToString();
                parmList.Add(parm);


                parm = new B1SP_Parameter();
                parm.ParameterName = "CreatedBy";
                parm.ParameterValue = Convert.ToString(TimeSheetFormDetails.CreatedBy);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CreatedDate";
                parm.ParameterValue = DateTime.Now.ToString("yyyy-MM-dd");
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

             

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("TimeSheetFormManagement", "Exception occured on TranslateTimeSheetFormDetailsSetupLogToParameterList, " + ex.Message);
            }


            return parmList;
        }

        private List<B1SP_Parameter> TranslateTimeSheetFormDetailsDetailLogToParameterList(TimeSheetFormDetails TimeSheetFormDetails,int DocID)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {

                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = Convert.ToString(TimeSheetFormDetails.ID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "HeaderID";
                parm.ParameterValue = Convert.ToString(DocID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "WorkDate_Previous";
                parm.ParameterValue = Convert.ToString(TimeSheetFormDetails._WorkDate);
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "AssignmentID_Previous";
                parm.ParameterValue = Convert.ToString(TimeSheetFormDetails.AssignmentID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "TaskID_Previous";
                parm.ParameterValue = Convert.ToString(TimeSheetFormDetails.TaskID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "LocationID_Previous";
                parm.ParameterValue = Convert.ToString(TimeSheetFormDetails.LocationID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "WorkHours_Previous";
                parm.ParameterValue = Convert.ToString(TimeSheetFormDetails.WorkHours);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "Description_Previous";
                parm.ParameterValue = Convert.ToString(TimeSheetFormDetails.Description);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "OnSite_Previous";
                parm.ParameterValue = Convert.ToString(TimeSheetFormDetails.OnSite);
                parm.ParameterType = DBTypes.Bool.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "IsDeleted_Previous";
                parm.ParameterValue = Convert.ToString(TimeSheetFormDetails.IsDeleted);
                parm.ParameterType = DBTypes.Bool.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "WorkDate_New";
                parm.ParameterValue = Convert.ToString(TimeSheetFormDetails._WorkDate);
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "AssignmentID_New";
                parm.ParameterValue = Convert.ToString(TimeSheetFormDetails.AssignmentID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "TaskID_New";
                parm.ParameterValue = Convert.ToString(TimeSheetFormDetails.TaskID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "LocationID_New";
                parm.ParameterValue = Convert.ToString(TimeSheetFormDetails.LocationID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "WorkHours_New";
                parm.ParameterValue = Convert.ToString(TimeSheetFormDetails.WorkHours);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "Description_New";
                parm.ParameterValue = Convert.ToString(TimeSheetFormDetails.Description);
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "OnSite_New";
                parm.ParameterValue = Convert.ToString(TimeSheetFormDetails.OnSite);
                parm.ParameterType = DBTypes.Bool.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "IsDeleted_New";
                parm.ParameterValue = Convert.ToString(TimeSheetFormDetails.IsDeleted);
                parm.ParameterType = DBTypes.Bool.ToString();
                parmList.Add(parm);

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("TimeSheetFormManagement", "Exception occured on TranslateTimeSheetFormDetailsDetailLogToParameterList, " + ex.Message);
            }


            return parmList;
        }
    }
}