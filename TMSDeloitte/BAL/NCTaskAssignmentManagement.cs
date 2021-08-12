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
    public class NCTaskAssignmentManagement
    {
        public DataTable GetNCTaskByEmpID(int id)
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
                dt = HANADAL.GetDataTableByStoredProcedure("GetNCTaskByEmpID", parmList, "TimeSheetFormManagement");


            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("NCTaskAssignmentManagement", "Exception occured on GetNCTaskByEmpID, " + ex.Message);
            }

            return dt;
        }

        public List<string> GetDocNum()
        {
            List<string> docNumList = new List<string>();
            BAL.Common setupManagement = new BAL.Common();
            try
            {
              

                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                Common cmn = new Common();
                DataTable dt = HANADAL.GetDataTableByStoredProcedure("GetNCTaskAssignmentDocNum",  "NCTaskAssignmentManagement");
                if (dt.Rows.Count > 0)
                {
                    docNumList = setupManagement.TranslateDataTableToDocNumList(dt);
                }

               
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("NCTaskAssignmentManagement", "Exception occured on GetDocNum, " + ex.Message);
            }

            return docNumList;
        }
        public List<NCTaskAssignment> GetAllNCTaskAssignmentByEmpID(int id)
        {
            List<NCTaskAssignment> NCTaskAssignmentDetails = new List<NCTaskAssignment>();
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
                DataSet ds = HANADAL.GetDataSetByStoredProcedure("GetNCTaskAssignmentByEmpID", parmList, "NCTaskAssignmentManagement");
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        NCTaskAssignmentDetails = TranslateDataTableToNCTaskAssignmentList(ds.Tables[0]);
                        //NCTaskAssignmentDetails.Detail = TranslateDataTableToNCTaskAssignmentDetail(ds.Tables[1]);
                    }
                }



            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("NCTaskAssignmentManagement", "Exception occured on GetNCTaskAssignmentDetailsByID ID: " + id + " , " + ex.Message);
            }

            return NCTaskAssignmentDetails;
        }

        public List<NCTaskAssignment> GetAllNCTaskAssignment()
        {
            List<NCTaskAssignment> NCTaskAssignmentDetails = new List<NCTaskAssignment>();
            try
            {
                List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();

                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                Common cmn = new Common();
                DataSet ds = HANADAL.GetDataSetByStoredProcedure("GetAllNCTaskAssignment", parmList, "NCTaskAssignmentManagement");
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        NCTaskAssignmentDetails = TranslateDataTableToNCTaskAssignmentList(ds.Tables[0]);
                        //NCTaskAssignmentDetails.Detail = TranslateDataTableToNCTaskAssignmentDetail(ds.Tables[1]);
                    }
                }



            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("NCTaskAssignmentManagement", "Exception occured on GetNCTaskAssignmentDetailsByID " + " , " + ex.Message);
            }

            return NCTaskAssignmentDetails;
        }

        public NCTaskAssignment GetNCTaskAssignmentDetailsByID(int id)
        {
            NCTaskAssignment NCTaskAssignmentDetails = new NCTaskAssignment();
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
                DataSet ds = HANADAL.GetDataSetByStoredProcedure("GetNCTaskAssignmentByID", parmList, "NCTaskAssignmentManagement");
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        NCTaskAssignmentDetails = TranslateDataTableToNCTaskAssignment(ds.Tables[0]);
                        NCTaskAssignmentDetails.Detail = TranslateDataTableToNCTaskAssignmentDetail(ds.Tables[1]);
                    }
                }



            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("NCTaskAssignmentManagement", "Exception occured on GetNCTaskAssignmentDetailsByID ID: " + id + " , " + ex.Message);
            }

            return NCTaskAssignmentDetails;
        }


        public DataTable GetNCTaskAssignmentDeatilByEmpID(int id)
        {
            DataTable dt = new DataTable();
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
                 dt = HANADAL.GetDataTableByStoredProcedure("GetNCTaskAssignmentDeatilByEmpID", parmList, "GetNCTaskAssignmentDeatilByEmpID");
                
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("NCTaskAssignmentManagement", "Exception occured on GetNCTaskAssignmentDeatilByEmpID ID: " + id + " , " + ex.Message);
            }

            return dt;
        }

        public bool AddUpdateNCTaskAssignmentDetailsSetup(NCTaskAssignment NCTaskAssignmentDetails, out string msg)
        {
            bool isSuccess = false;
            bool isAddOccured = false;
            bool isUpdateOccured = false;
            bool isDeleteOccured = false;
            NCTaskAssignment previousObj = new NCTaskAssignment();
           Common cmn = new Common();
            string docNum = "";
            int docId = 0;
            msg = "Successfully Added/Updated";
            try
            {
                if(NCTaskAssignmentDetails.Detail!=null)
                {
                    if (NCTaskAssignmentDetails.Detail.Count>0)
                    {
                        var filtererdList = NCTaskAssignmentDetails.Detail.Where(x => x.IsDeleted == false).ToList();
                        var duplicates = filtererdList.GroupBy(x => x.TaskID).Where(g => g.Count() > 1).Select(y => y.Key).ToList();
                        if (duplicates.Count > 0)
                        {
                            msg = "Duplicate record exist!";
                            return false;
                        }
                    }
                }
               

                var value = NCTaskAssignmentDetails.DocNum;
                if (!string.IsNullOrEmpty(value))
                {
                    docNum = value;
                    docId = Convert.ToInt32(NCTaskAssignmentDetails.ID);
                    
                    previousObj = GetNCTaskAssignmentDetailsByID(docId);

                    AddHeader_Log(NCTaskAssignmentDetails, previousObj,docId);
                    
                }
                else
                {
                   
                    int no = 1;
                    List<string> docNumList = cmn.GetDocNum("GetNCTaskAssignmentsDocNum", "NCTaskAssignmentManagement");
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



                NCTaskAssignmentDetails.DocNum = docNum;
                if(docId==0)
                    NCTaskAssignmentDetails.Detail = NCTaskAssignmentDetails.Detail.Where(x => x.IsDeleted == false && x.ID == 0).ToList();
                else
                    NCTaskAssignmentDetails.Detail = NCTaskAssignmentDetails.Detail.Where(x => x.IsDeleted == false && (x.ID == 0|| x.ID>0)).ToList();
               
                ////For Form Log
                // AddNCTaskAssignmentDetailsSetup_Log(NCTaskAssignmentDetailsList, out isUpdateOccured);

                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                DataTable dtHeader = HANADAL.AddUpdateDataByStoredProcedure("AddUpdateNCTaskAssignment", TranslateNCTaskAssignmentDetailsSetupToParameterList(NCTaskAssignmentDetails), "NCTaskAssignmentManagement");
                if (dtHeader.Rows.Count == 0)
                    throw new Exception("Exception occured when AddUpdateNCTaskAssignmentDetailsSetup , ID:" + NCTaskAssignmentDetails.ID + " , Emp ID:" + NCTaskAssignmentDetails.EmpID );
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
                //    //AddNCTaskAssignmentDetails_Log(NCTaskAssignmentDetails, out isUpdateOccured);
                //    DeleteNCTaskAssignmentDetailsDetailByHeaderID(docId); 
                //}

                NCTaskAssignmentDetails.Detail.Select(c => { c.HeaderID = docId; return c; }).ToList();

                foreach (var list in NCTaskAssignmentDetails.Detail)
                {
                    try
                    {
                        if(list.ID>0)
                        {
                            var previousObjectDetail = previousObj.Detail.Where(x => x.ID == list.ID).FirstOrDefault();

                            AddDetail_Log(list,previousObjectDetail, docId);
                        }

                        HANADAL.AddUpdateDataByStoredProcedure("AddUpdateNCTaskAssignmentDetail", TranslateNCTaskAssignmentDetailsDetailToParameterList(list), "NCTaskAssignmentManagement");
                       
                    }
                    catch (Exception ex)
                    {
                        isSuccess = false;
                        Log log = new Log();
                        log.LogFile(ex.Message);
                        log.InputOutputDocLog("NCTaskAssignmentManagement", "Exception occured in foreach loop AddUpdateNCTaskAssignmentDetail, " + ex.Message);
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
                            List<NCTaskAssignmentDetails> missingList = previousObj.Detail.Where(n => !NCTaskAssignmentDetails.Detail.Any(o => o.ID == n.ID && o.IsDeleted == n.IsDeleted)).ToList();
                            missingList.Select(c => { c.IsDeleted = true; return c; }).ToList();
                            foreach (var item in missingList)
                            {
                                NCTaskAssignmentDetails previousObjectDetail = new NCTaskAssignmentDetails();
                                previousObjectDetail = previousObj.Detail.Where(x => x.ID == item.ID).FirstOrDefault();
                              

                                AddDetail_Log(item, previousObjectDetail, docId,true);

                                
                                isDeleteOccured = true;
                                //AddUserAlertSetup_Log(user, previous_item, DocId);
                                HANADAL.AddUpdateDataByStoredProcedure("AddUpdateNCTaskAssignmentDetail", TranslateNCTaskAssignmentDetailsDetailToParameterList(item), "NCTaskAssignmentManagement");

                            }
                        }
                    }
                   

                   
                }

                //For Master Log
                if (NCTaskAssignmentDetails.ID > 0)
                    isAddOccured = true;
                if (Convert.ToBoolean(NCTaskAssignmentDetails.IsDeleted))
                    isDeleteOccured = true;

                int CreatedBy = Convert.ToInt32(NCTaskAssignmentDetails.CreatedBy);
                Task.Run(() => cmn.AddMasterLog(isAddOccured, isUpdateOccured, isDeleteOccured, nameof(Enums.FormsName.NCTaskAssignmentForm), CreatedBy, "NCTaskAssignmentManagement"));
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
                log.InputOutputDocLog("NCTaskAssignmentManagement", "Exception occured on AddUpdateAssignmentCostSetup, " + ex.Message);
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
                DataTable dt_UserProfile = HANADAL.GetDataTableByStoredProcedure("ValidateTimeSheetPeriodDateRange", parmList, "NCTaskAssignmentManagement");
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
                log.InputOutputDocLog("NCTaskAssignmentManagement", "Exception occured on ValidateDateRange, " + ex.Message);
            }
            return isSuccess;
        }

       
        public DataTable GetAllDocumentsDataTable()
        {
            DataTable dt = new DataTable();
            Common cmn = new Common();
            try

            {
                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                dt = HANADAL.GetDataTableByStoredProcedure("GetNCTaskAssignmentAllDocuments", "NCTaskAssignmentManagement");
                dt = cmn.RemoveDuplicateRows(dt, "DocNum");
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("NCTaskAssignmentManagement", "Exception occured on GetTimeSheetAllDocumentsList, " + ex.Message);
            }

            return dt;
        }

        public List<NCTaskAssignment> GetAllDocumentsList()
        {
            List<NCTaskAssignment> list = new List<NCTaskAssignment>();
            DataTable dt = new DataTable();
            Common cmn = new Common();
            try

            {
                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                dt = HANADAL.GetDataTableByStoredProcedure("GetNCTaskAssignmentAllDocuments", "NCTaskAssignmentManagement");
                dt = cmn.RemoveDuplicateRows(dt, "DocNum");
                list = TranslateDataTableToNCTaskAssignmentList(dt);
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("NCTaskAssignmentManagement", "Exception occured on GetTimeSheetAllDocumentsList, " + ex.Message);
            }

            return list;
        }

        public NCTaskAssignment GetNCTaskAssignmentByDocNum(string docNo)
        {
            NCTaskAssignment obj = new NCTaskAssignment();
            try
            {

                List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();

                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "DocNum";
                parm.ParameterValue = docNo;
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

              

                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                Common cmn = new Common();
                DataSet ds = HANADAL.GetDataSetByStoredProcedure("GetNCTaskAssignmentByDocNum", parmList, "NCTaskAssignmentManagement");
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        obj = TranslateDataTableToNCTaskAssignment(ds.Tables[0]);
                        obj.Detail = TranslateDataTableToNCTaskAssignmentDetail(ds.Tables[1]);
                    }
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("NCTaskAssignmentManagement", "Exception occured on GetResourceBillingRatesByDocNum DocNum: " + docNo + " , " + ex.Message);
            }

            return obj;
        }

        public DataSet GetNCTaskAssignmentLog(int docid)
        {
            DataSet ds = new DataSet();
            try
            {

                List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();

                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "DocID";
                parm.ParameterValue =Convert.ToString(docid);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                //parm = new B1SP_Parameter();
                //parm.ParameterName = "EmpID";
                //parm.ParameterValue = Convert.ToString(empID);
                //parmList.Add(parm);

                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                Common cmn = new Common();
                 ds = HANADAL.GetDataSetByStoredProcedure("GetNCTaskAssignmentLog", parmList, "NCTaskAssignmentManagement");
                //if (ds.Tables.Count > 0)
                //{
                //    if (ds.Tables[0].Rows.Count > 0)
                //    {
                //        obj = TranslateDataTableToNCTaskAssignment(ds.Tables[0]);
                //        obj.Detail = TranslateDataTableToNCTaskAssignmentDetail(ds.Tables[1]);
                //    }
                //}
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("NCTaskAssignmentManagement", "Exception occured on GetNCTaskAssignmentLog DocID: " + docid + " , " + ex.Message);
            }

            return ds;
        }


        public string GetNCTaskNameFromID(int id)
        {
            string name = "";
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
                DataTable dt = HANADAL.GetDataTableByStoredProcedure("GetNCTaskDetailByID", parmList, "NCTaskAssignmentManagement");
                foreach (DataRow dtRow in dt.Rows)
                {
                    name = Convert.ToString(dtRow["Name"]);
                    break;
                }


            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("NCTaskAssignmentManagement", "Exception occured on GetNCTaskNameFromID ID: " + id + " , " + ex.Message);
            }

            return name;
        }

        public void AddHeader_Log(NCTaskAssignment newObject, NCTaskAssignment previousObject, int docID)
        {
            try
            {
                if (newObject.ID > 0)
                {
                    newObject.DocDate = newObject.DocDate.Replace("/","-");

                    List<B1SP_Parameter> paramList = TranslateNCTaskAssignmentDetailsSetupLogToParameterList(newObject, docID);
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


                               
                                case "DocDate":
                                    isChangeOccured = true;
                                    paramList.Where(x => x.ParameterName == "DocDate_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                    paramList.Where(x => x.ParameterName == "DocDate_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();

                                    break;

                              
                            }

                        }

                        if (isChangeOccured)
                        {
                            HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                            Task.Run(() => HANADAL.AddUpdateDataByStoredProcedure("AddNCTaskAssignment_Log", paramList, "NCTaskAssignmentManagement"));
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("NCTaskAssignmentManagement", "Exception occured on AddHeader_Log, " + ex.Message);
            }
        }

        public void AddDetail_Log(NCTaskAssignmentDetails newObject, NCTaskAssignmentDetails previousObject, int docID,bool isDeleted=false)
        {
            try
            {
                if (newObject.ID > 0)
                {
                    List<B1SP_Parameter> paramList = TranslateNCTaskAssignmentDetailsDetailLogToParameterList(newObject, docID);
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


                                case "Date":
                                    isChangeOccured = true;
                                    
                                    paramList.Where(x => x.ParameterName == "Date_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                    paramList.Where(x => x.ParameterName == "Date_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();

                                    break;

                                case "TaskID":
                                    isChangeOccured = true;
                                    paramList.Where(x => x.ParameterName == "AssignmentID_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                    paramList.Where(x => x.ParameterName == "AssignmentID_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();

                                    break;

                                    
                                case "IsDeleted":
                                    isChangeOccured = true;
                                    paramList.Where(x => x.ParameterName == "IsDeleted_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                    paramList.Where(x => x.ParameterName == "IsDeleted_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();
                                    break;
                            }

                        }
                        if(isDeleted)
                        {
                            isChangeOccured = true;
                            paramList.Where(x => x.ParameterName == "IsDeleted_Previous").Select(c => { c.ParameterValue = Convert.ToString(false); return c; }).ToList();
                            paramList.Where(x => x.ParameterName == "IsDeleted_New").Select(c => { c.ParameterValue = Convert.ToString(true); return c; }).ToList();
                        }
                        if (isChangeOccured)
                        {
                            HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                            Task.Run(() => HANADAL.AddUpdateDataByStoredProcedure("AddUpdateNCTaskAssignmentDetail_Log", paramList, "NCTaskAssignmentManagement"));
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("NCTaskAssignmentManagement", "Exception occured on AddDetail_Log, " + ex.Message);
            }
        }

        

        private List<NCTaskAssignment> TranslateDataTableToNCTaskAssignmentList(DataTable dt)
        {
            List<NCTaskAssignment> list = new List<NCTaskAssignment>();
            List<HCM_Employee> hcmEmpList = new List<HCM_Employee>();
            try
            {
                if (dt.Rows.Count>0)
                {
                    HCMOneManagement hcmMgt = new HCMOneManagement();
                    hcmEmpList = hcmMgt.GetAllHCMUser();
                }
               
                foreach (DataRow dtRow in dt.Rows)
                {
                    NCTaskAssignment NCTaskAssignmentDetails = new NCTaskAssignment();
                    NCTaskAssignmentDetails.ID = Convert.ToInt32(dtRow["ID"]);
                    NCTaskAssignmentDetails.DocNum = Convert.ToString(dtRow["DocNum"]);
                    NCTaskAssignmentDetails.EmpID = Convert.ToInt32(dtRow["EmpID"]);
                    NCTaskAssignmentDetails.EmpCode = Convert.ToString(dtRow["EmpCode"]);

                   HCM_Employee hcmEmpData= hcmEmpList.Where(x=>x.EmpID== NCTaskAssignmentDetails.EmpCode).FirstOrDefault();
                    if (hcmEmpData != null)
                        NCTaskAssignmentDetails.EmpName = hcmEmpData.FirstName + " " + hcmEmpData.MiddleName + " " + hcmEmpData.LastName;

                    NCTaskAssignmentDetails.DocDate = Convert.ToDateTime(dtRow["DocDate"]).ToString("yyyy-MM-dd");
                   
                    NCTaskAssignmentDetails.CreatedBy = Convert.ToInt32(dtRow["CreatedBy"]);
                    NCTaskAssignmentDetails.CreatedDate = Convert.ToDateTime(dtRow["CREATEDDATE"]);
                    if (dtRow["UpdatedBy"] != DBNull.Value)
                        NCTaskAssignmentDetails.UpdatedBy = Convert.ToInt32(dtRow["UpdatedBy"]);
                    if (dtRow["UpdatedDate"] != DBNull.Value)
                        NCTaskAssignmentDetails.UpdatedDate = Convert.ToDateTime(dtRow["UpdatedDate"]);

                  
                    NCTaskAssignmentDetails.IsDeleted = Convert.ToBoolean(dtRow["IsDeleted"]);

                  
                    list.Add(NCTaskAssignmentDetails);
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("NCTaskAssignmentManagement", "Exception occured on TranslateDataTableToNCTaskAssignmentList, " + ex.Message);
            }

            return list;
        }
        private NCTaskAssignment TranslateDataTableToNCTaskAssignment(DataTable dt)
        {
            NCTaskAssignment NCTaskAssignmentDetails = new NCTaskAssignment();

            try
            {
                foreach (DataRow dtRow in dt.Rows)
                {

                    NCTaskAssignmentDetails.ID = Convert.ToInt32(dtRow["ID"]);
                 
                    NCTaskAssignmentDetails.DocNum = Convert.ToString(dtRow["DocNum"]);
                   
                    NCTaskAssignmentDetails.EmpID = Convert.ToInt32(dtRow["EmpID"]);
                    NCTaskAssignmentDetails.EmpCode = Convert.ToString(dtRow["EmpCode"]);
                   
                    NCTaskAssignmentDetails.DocDate = Convert.ToDateTime(dtRow["DocDate"]).ToString("yyyy-MM-dd");
                  
                    NCTaskAssignmentDetails.CreatedBy = Convert.ToInt32(dtRow["CreatedBy"]);
                    NCTaskAssignmentDetails.CreatedDate = Convert.ToDateTime(dtRow["CREATEDDATE"]);
                    if (dtRow["UpdatedBy"] != DBNull.Value)
                        NCTaskAssignmentDetails.UpdatedBy = Convert.ToInt32(dtRow["UpdatedBy"]);
                    if (dtRow["UpdatedDate"] != DBNull.Value)
                        NCTaskAssignmentDetails.UpdatedDate = Convert.ToDateTime(dtRow["UpdatedDate"]);
                    
                    NCTaskAssignmentDetails.IsDeleted = Convert.ToBoolean(dtRow["IsDeleted"]);
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("NCTaskAssignmentManagement", "Exception occured on TranslateDataTableToNCTaskAssignment, " + ex.Message);
            }

            return NCTaskAssignmentDetails;
        }
        private List<NCTaskAssignmentDetails> TranslateDataTableToNCTaskAssignmentDetail(DataTable dt)
        {
            List<NCTaskAssignmentDetails> NCTaskAssignmentDetailsList = new List<NCTaskAssignmentDetails>();

            try
            {
                int sno = 1;
                foreach (DataRow dtRow in dt.Rows)
                {
                    NCTaskAssignmentDetails NCTaskAssignmentDetails = new NCTaskAssignmentDetails();
                    NCTaskAssignmentDetails.SNo = sno;
                    NCTaskAssignmentDetails.KEY = Guid.NewGuid().ToString();
                    NCTaskAssignmentDetails.ID = Convert.ToInt32(dtRow["ID"]);
                    NCTaskAssignmentDetails.HeaderID = Convert.ToInt32(dtRow["HeaderID"]);
                    NCTaskAssignmentDetails.TaskID = Convert.ToInt32(dtRow["TaskID"]);
                    NCTaskAssignmentDetails._Name = GetNCTaskNameFromID(NCTaskAssignmentDetails.TaskID);
                    NCTaskAssignmentDetails.IsActive = Convert.ToBoolean(dtRow["IsActive"]);
                    NCTaskAssignmentDetails.IsDeleted = Convert.ToBoolean(dtRow["IsDeleted"]);
                  
                    sno = sno + 1;
                    NCTaskAssignmentDetailsList.Add(NCTaskAssignmentDetails);
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("NCTaskAssignmentManagement", "Exception occured on TranslateDataTableToNCTaskAssignmentDetail, " + ex.Message);
            }

            return NCTaskAssignmentDetailsList;
        }
        private List<B1SP_Parameter> TranslateNCTaskAssignmentDetailsSetupToParameterList(NCTaskAssignment NCTaskAssignmentDetails)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {

                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = Convert.ToString(NCTaskAssignmentDetails.ID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

               
                parm = new B1SP_Parameter();
                parm.ParameterName = "DocNum";
                parm.ParameterValue = Convert.ToString(NCTaskAssignmentDetails.DocNum);
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "EmpID";
                parm.ParameterValue = Convert.ToString(NCTaskAssignmentDetails.EmpID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "EmpCode";
                parm.ParameterValue = Convert.ToString(NCTaskAssignmentDetails.EmpCode);
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

              
                parm = new B1SP_Parameter();
                parm.ParameterName = "DocDate";
                parm.ParameterValue = Convert.ToString(NCTaskAssignmentDetails.DocDate);
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);
                
                parm = new B1SP_Parameter();
                parm.ParameterName = "IsDeleted";
                parm.ParameterValue = Convert.ToString(NCTaskAssignmentDetails.IsDeleted);
                parm.ParameterType = DBTypes.Bool.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CreatedBy";
                parm.ParameterValue = Convert.ToString(NCTaskAssignmentDetails.CreatedBy);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CreatedDate";
                parm.ParameterValue = DateTime.Now.ToString("yyyy-MM-dd");
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "UpdatedBy";
                parm.ParameterValue = Convert.ToString(NCTaskAssignmentDetails.UpdatedBy);
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
                log.InputOutputDocLog("NCTaskAssignmentManagement", "Exception occured on TranslateNCTaskAssignmentDetailsSetupToParameterList, " + ex.Message);
            }


            return parmList;
        }
        private List<B1SP_Parameter> TranslateNCTaskAssignmentDetailsDetailToParameterList(NCTaskAssignmentDetails NCTaskAssignmentDetails)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {

                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = Convert.ToString(NCTaskAssignmentDetails.ID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "HeaderID";
                parm.ParameterValue = Convert.ToString(NCTaskAssignmentDetails.HeaderID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

              

                parm = new B1SP_Parameter();
                parm.ParameterName = "TaskID";
                parm.ParameterValue = Convert.ToString(NCTaskAssignmentDetails.TaskID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                
                
                parm = new B1SP_Parameter();
                parm.ParameterName = "IsActive";
                parm.ParameterValue = Convert.ToString(NCTaskAssignmentDetails.IsActive);
                parm.ParameterType = DBTypes.Bool.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "IsDeleted";
                parm.ParameterValue = Convert.ToString(NCTaskAssignmentDetails.IsDeleted);
                parm.ParameterType = DBTypes.Bool.ToString();
                parmList.Add(parm);
                
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("NCTaskAssignmentManagement", "Exception occured on TranslateNCTaskAssignmentDetailsDetailToParameterList, " + ex.Message);
            }


            return parmList;
        }
        
        private List<B1SP_Parameter> TranslateNCTaskAssignmentDetailsSetupLogToParameterList(NCTaskAssignment NCTaskAssignmentDetails,int DocID)
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
                parm.ParameterName = "DocNum";
                parm.ParameterValue = Convert.ToString(NCTaskAssignmentDetails.DocNum);
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "DocID";
                parm.ParameterValue = Convert.ToString(DocID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

               

                parm = new B1SP_Parameter();
                parm.ParameterName = "EmpID";
                parm.ParameterValue = Convert.ToString(NCTaskAssignmentDetails.EmpID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

              
                parm = new B1SP_Parameter();
                parm.ParameterName = "DocDate_Previous";
                parm.ParameterValue = Convert.ToString(NCTaskAssignmentDetails.DocDate);
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);
                
               
                parm = new B1SP_Parameter();
                parm.ParameterName = "IsDeleted_Previous";
                parm.ParameterValue = Convert.ToString(NCTaskAssignmentDetails.IsDeleted);
                parm.ParameterType = DBTypes.Bool.ToString();
                parmList.Add(parm);

              

                parm = new B1SP_Parameter();
                parm.ParameterName = "DocDate_New";
                parm.ParameterValue = Convert.ToString(NCTaskAssignmentDetails.DocDate);
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                
                parm = new B1SP_Parameter();
                parm.ParameterName = "IsDeleted_New";
                parm.ParameterValue = Convert.ToString(NCTaskAssignmentDetails.IsDeleted);
                parm.ParameterType = DBTypes.Bool.ToString();
                parmList.Add(parm);


                parm = new B1SP_Parameter();
                parm.ParameterName = "CreatedBy";
                parm.ParameterValue = Convert.ToString(NCTaskAssignmentDetails.CreatedBy);
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
                log.InputOutputDocLog("NCTaskAssignmentManagement", "Exception occured on TranslateNCTaskAssignmentDetailsSetupLogToParameterList, " + ex.Message);
            }


            return parmList;
        }

        private List<B1SP_Parameter> TranslateNCTaskAssignmentDetailsDetailLogToParameterList(NCTaskAssignmentDetails NCTaskAssignmentDetails,int DocID)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {

                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = Convert.ToString(NCTaskAssignmentDetails.ID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "HeaderID";
                parm.ParameterValue = Convert.ToString(DocID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

               

                parm = new B1SP_Parameter();
                parm.ParameterName = "TaskID_Previous";
                parm.ParameterValue = Convert.ToString(NCTaskAssignmentDetails.TaskID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                
                parm = new B1SP_Parameter();
                parm.ParameterName = "IsActive_Previous";
                parm.ParameterValue = Convert.ToString(NCTaskAssignmentDetails.IsActive);
                parm.ParameterType = DBTypes.Bool.ToString();
                parmList.Add(parm);


                parm = new B1SP_Parameter();
                parm.ParameterName = "IsDeleted_Previous";
                parm.ParameterValue = Convert.ToString(NCTaskAssignmentDetails.IsDeleted);
                parm.ParameterType = DBTypes.Bool.ToString();
                parmList.Add(parm);

              
                parm = new B1SP_Parameter();
                parm.ParameterName = "TaskID_New";
                parm.ParameterValue = Convert.ToString(NCTaskAssignmentDetails.TaskID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                

                parm = new B1SP_Parameter();
                parm.ParameterName = "IsActive_New";
                parm.ParameterValue = Convert.ToString(NCTaskAssignmentDetails.IsActive);
                parm.ParameterType = DBTypes.Bool.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "IsDeleted_New";
                parm.ParameterValue = Convert.ToString(NCTaskAssignmentDetails.IsDeleted);
                parm.ParameterType = DBTypes.Bool.ToString();
                parmList.Add(parm);

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("NCTaskAssignmentManagement", "Exception occured on TranslateNCTaskAssignmentDetailsDetailLogToParameterList, " + ex.Message);
            }


            return parmList;
        }
    }
}