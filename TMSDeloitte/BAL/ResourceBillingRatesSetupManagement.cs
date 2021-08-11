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
    public class ResourceBillingRatesSetupManagement
    {

        public ResourceBillingRates GetResourceBillingRatesByID(int id)
        {
            ResourceBillingRates resourceBillingRates = new ResourceBillingRates();
            try
            {
                List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = Convert.ToString(id);
                parmList.Add(parm);

                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                Common cmn = new Common();
                DataSet ds = HANADAL.GetDataSetByStoredProcedure("GetResourceBillingRatesByID", parmList, "ResourceBillingRatesManagement");
                if(ds.Tables.Count>0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        resourceBillingRates = TranslateDataTableToResourceBillingRates(ds.Tables[0]);
                        resourceBillingRates.Detail = TranslateDataTableToResourceBillingRatesDetail(ds.Tables[1]);
                    }
                }
               
                
                
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ResourceBillingRatesManagement", "Exception occured on GetResourceBillingRatesByID ID: " + id + " , " + ex.Message);
            }

            return resourceBillingRates;
        }

        public ResourceBillingRates GetResourceBillingRatesByDocNum(string docNo)
        {
           ResourceBillingRates resourceBillingRates = new ResourceBillingRates();
            try
            {

                List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();

                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "DocNum";
                parm.ParameterValue = docNo;
                parmList.Add(parm);

                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                Common cmn = new Common();
                DataSet ds = HANADAL.GetDataSetByStoredProcedure("GetResourceBillingRatesByDocNum", parmList, "ResourceBillingRatesManagement");
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        resourceBillingRates = TranslateDataTableToResourceBillingRates(ds.Tables[0]);
                        resourceBillingRates.Detail = TranslateDataTableToResourceBillingRatesDetail(ds.Tables[1]);
                    }
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ResourceBillingRatesManagement", "Exception occured on GetResourceBillingRatesByDocNum DocNum: " + docNo + " , " + ex.Message);
            }

            return resourceBillingRates;
        }

        public List<ResourceBillingRates> GetResourceBillingRates(int id = 0)
        {
            List<ResourceBillingRates> resourceBillingRatesList = new List<ResourceBillingRates>();
            try
            {
                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                Common cmn = new Common();
                List<HCM_Designation> designationList = cmn.GetHCMDesignationList();
                DataTable dt_ResourceBillingRates = HANADAL.GetDataTableByStoredProcedure("GetResourceBillingRates", cmn.TranslateIDToParameterList(id), "ResourceBillingRatesManagement");
                if (dt_ResourceBillingRates.Rows.Count > 0)
                {
                   // resourceBillingRatesList = TranslateDataTableToResourceBillingRatesSetupList(dt_ResourceBillingRates, designationList);
                }



            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ResourceBillingRatesManagement", "Exception occured on GetResourceBillingRates ID: " + id + " , " + ex.Message);
            }

            return resourceBillingRatesList;
        }

        public bool ValidateDateRange(string functionID,DateTime fromDate,DateTime toDate)
        {
            bool isSuccess = false;

            try
            {
                List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();

                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "FunctionID";
                parm.ParameterValue = Convert.ToString(functionID);
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
                DataTable dt_UserProfile = HANADAL.GetDataTableByStoredProcedure("ValidateResourceBillingRatesDateRange", parmList, "ResourceBillingRatesManagement");
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
                log.InputOutputDocLog("ResourceBillingRatesManagement", "Exception occured on ValidateDateRange, " + ex.Message);
            }
            return isSuccess;
        }

        public bool AddUpdateResourceBillingRatesSetup(ResourceBillingRates resourceBillingRates,out string msg)
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
                if(resourceBillingRates.Detail!=null)
                {
                    if(resourceBillingRates.Detail.Count>0)
                    {
                        var filtererdList = resourceBillingRates.Detail.Where(x => x.IsDeleted == false).ToList();
                        var duplicates = filtererdList.GroupBy(x => x.DesignationID).Where(g => g.Count() > 1).Select(y => y.Key).ToList();
                        if (duplicates.Count > 0)
                        {
                            msg = "Duplicate record exist!";
                            return false;
                        }
                    }
                }

               
                

                var value = resourceBillingRates.DocNum;
                if (!string.IsNullOrEmpty(value))
                {
                    docNum = value;
                    docId =Convert.ToInt32(resourceBillingRates.ID);
                    ResourceBillingRates  oldObj= GetResourceBillingRatesByID(docId);
                    if( oldObj.FromDate.ToString("yyyy-MM-dd")!= resourceBillingRates.FromDate.ToString("yyyy-MM-dd") ||
                        oldObj.ToDate.ToString("yyyy-MM-dd") != resourceBillingRates.ToDate.ToString("yyyy-MM-dd")
                        )
                    {
                        if(ValidateDateRange(resourceBillingRates.FunctionID, resourceBillingRates.FromDate, resourceBillingRates.ToDate))
                        {
                            msg = "Record exist in this date range!";
                            return false;
                        }
                    }
                }
                else
                {
                    if (ValidateDateRange(resourceBillingRates.FunctionID, resourceBillingRates.FromDate, resourceBillingRates.ToDate))
                    {
                        msg = "Record exist in this date range!";
                        return false;
                    }

                    int no = 1;
                    List<string> docNumList = cmn.GetDocNum("GetResourceBillingRatesDocNum", "ResourceBillingRatesManagement");
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



                resourceBillingRates.DocNum = docNum;
                resourceBillingRates.Detail= resourceBillingRates.Detail.Where(x => x.ID > 0 || x.IsDeleted == false).ToList();

                ////For Form Log
                // AddResourceBillingRatesSetup_Log(resourceBillingRatesList, out isUpdateOccured);
                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                if (docId==0)
                {
                    DataTable dtHeader = HANADAL.AddUpdateDataByStoredProcedure("AddUpdateResourceBillingRates", TranslateResourceBillingRatesSetupToParameterList(resourceBillingRates), "ResourceBillingRatesManagement");
                    if (dtHeader.Rows.Count == 0)
                        throw new Exception("Exception occured when AddUpdateResourceBillingRatesSetup , ID:" + resourceBillingRates.ID + " , FuncationID:" + resourceBillingRates.FunctionID);
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
                    AddResourceBillingRates_Log(resourceBillingRates,out isUpdateOccured);
                    
                }

                resourceBillingRates.Detail.Select(c => { c.HeaderID = docId; return c; }).ToList();
                AddResourceBillingRatesDetail_Log(resourceBillingRates.Detail, out isUpdateOccured);

                foreach (var list in resourceBillingRates.Detail)
                {
                    try
                    {
                        HANADAL.AddUpdateDataByStoredProcedure("AddUpdateResourceBillingRatesDetail", TranslateResourceBillingRatesDetailToParameterList(list), "ResourceBillingRatesManagement");
                        //DataTable dtRow = HANADAL.AddUpdateDataByStoredProcedure("AddUpdateResourceBillingRatesDetail", TranslateResourceBillingRatesDetailToParameterList(list), "ResourceBillingRatesManagement");
                        //if (dtRow.Rows.Count == 0)
                        //    throw new Exception("Exception occured when AddUpdateResourceBillingRatesRow , ID:" + list.ID + " , HeaderID:" + list.HeaderID );
                    }
                    catch (Exception ex)
                    {
                        isSuccess = false;
                        Log log = new Log();
                        log.LogFile(ex.Message);
                        log.InputOutputDocLog("ResourceBillingRatesManagement", "Exception occured in foreach loop AddUpdateAssignmentCostSetup, " + ex.Message);
                        continue;
                    }
                }
                
                //For Master Log
                if (resourceBillingRates.ID > 0)
                    isAddOccured = true;
                if (Convert.ToBoolean(resourceBillingRates.ISDELETED))
                    isDeleteOccured = true;

                int createdBy =Convert.ToInt32(resourceBillingRates.CREATEDBY);
                Task.Run(() => cmn.AddMasterLog(isAddOccured, isUpdateOccured, isDeleteOccured, nameof(Enums.FormsName.ResourceBillingRateSetup), createdBy, "ResourceBillingRatesManagement"));
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
                log.InputOutputDocLog("ResourceBillingRatesManagement", "Exception occured on AddUpdateAssignmentCostSetup, " + ex.Message);
            }

            return isSuccess;
        }

        public void AddResourceBillingRates_Log(ResourceBillingRates resourceBillingRatesList, out bool isUpdateOccured)
        {
            isUpdateOccured = false;
            try
            {
                if (resourceBillingRatesList.ID == 0)
                    return;

                ResourceBillingRates previousObject = GetResourceBillingRatesByID(Convert.ToInt32(resourceBillingRatesList.ID));
                List<B1SP_Parameter> paramList = TranslateResourceBillingRatesLogToParameterList(resourceBillingRatesList);
                bool isChangeOccured = false;
                if (previousObject !=null)
                {
                    foreach (PropertyCompareResult resultItem in PropertyCompare.Compare(previousObject, resourceBillingRatesList))
                    {
                        isChangeOccured = true;
                        isUpdateOccured = true;
                        string Name = resultItem.Name;
                        object PreviousValue = resultItem.OldValue;
                        object NewValue = resultItem.NewValue;

                        switch (Name)
                        {
                            case "FromDate":
                                paramList.Where(x => x.ParameterName == "FromDate_Previous").Select(c => { c.ParameterValue = Convert.ToDateTime(PreviousValue).ToString("yyyy-MM-dd"); return c; }).ToList();
                                paramList.Where(x => x.ParameterName == "FromDate_new").Select(c => { c.ParameterValue = Convert.ToDateTime(NewValue).ToString("yyyy-MM-dd"); return c; }).ToList();

                                break;
                            case "ToDate":
                                paramList.Where(x => x.ParameterName == "ToDate_Previous").Select(c => { c.ParameterValue = Convert.ToDateTime(PreviousValue).ToString("yyyy-MM-dd"); return c; }).ToList();
                                paramList.Where(x => x.ParameterName == "ToDate_new").Select(c => { c.ParameterValue = Convert.ToDateTime(NewValue).ToString("yyyy-MM-dd"); return c; }).ToList();

                                break;
                            case "ISDELETED":
                                paramList.Where(x => x.ParameterName == "IsDeleted_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                paramList.Where(x => x.ParameterName == "IsDeleted_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();

                                break;
                        }

                    }

                    if (isChangeOccured)
                    {
                        HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                        Task.Run(() => HANADAL.AddUpdateDataByStoredProcedure("AddUpdateResourceBillingRatesLog", paramList, "ResourceBillingRatesManagement"));
                    }

                }
            
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ResourceBillingRatesManagement", "Exception occured on AddAssignmentCostSetup_Log, " + ex.Message);
            }
        }

        public void AddResourceBillingRatesDetail_Log(List<ResourceBillingRatesDetail> resourceBillingRatesList, out bool isUpdateOccured)
        {
            isUpdateOccured = false;
            try
            {
                resourceBillingRatesList = resourceBillingRatesList.Where(x => x.ID > 0).ToList();

                foreach (var newObject in resourceBillingRatesList)
                {

                    try
                    {
                        ResourceBillingRates previousObject = GetResourceBillingRatesByID(Convert.ToInt32(resourceBillingRatesList[0].HeaderID));
                        List<B1SP_Parameter> paramList = TranslateResourceBillingRatesDetailLogToParameterList(newObject);
                        //List<B1SP_Parameter> paramList = new List<B1SP_Parameter>();
                        //B1SP_Parameter parm = new B1SP_Parameter();
                        bool isChangeOccured = false;

                        var val = previousObject.Detail.Where(x => x.ID == newObject.ID).FirstOrDefault();
                        if (val != null)
                        {
                            foreach (PropertyCompareResult resultItem in PropertyCompare.Compare(val, newObject))
                            {
                                isChangeOccured = true;
                                isUpdateOccured = true;
                                string Name = resultItem.Name;
                                object PreviousValue = resultItem.OldValue;
                                object NewValue = resultItem.NewValue;

                                switch (Name)
                                {
                                    case "RatesPerHour":
                                        paramList.Where(x => x.ParameterName == "RatesPerHour_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                        paramList.Where(x => x.ParameterName == "RatesPerHour_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();

                                        break;
                                    case "IsActive":
                                        paramList.Where(x => x.ParameterName == "ISACTIVE_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                        paramList.Where(x => x.ParameterName == "ISACTIVE_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();

                                        break;
                                    case "IsDeleted":
                                        paramList.Where(x => x.ParameterName == "ISDELETED_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                        paramList.Where(x => x.ParameterName == "ISDELETED_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();

                                        break;
                                }

                            }
                        }


                        if (isChangeOccured)
                        {
                            HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                            Task.Run(() => HANADAL.AddUpdateDataByStoredProcedure("AddUpdateResourceBillingRatesDetailLog", paramList, "ResourceBillingRatesManagement"));
                        }


                    }
                    catch (Exception ex)
                    {
                        Log log = new Log();
                        log.LogFile(ex.Message);
                        log.InputOutputDocLog("ResourceBillingRatesManagement", "Exception occured in foreach loop AddResourceBillingRatesSetup_Log, " + ex.Message);
                        continue;
                    }


                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ResourceBillingRatesManagement", "Exception occured on AddAssignmentCostSetup_Log, " + ex.Message);
            }
        }

        public DataSet GetResourceBillingRatesSetupLog(string id)
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
                    ds = HANADAL.GetDataSetByStoredProcedure("GetResourceBillingRatesLogByID", parmList, "ResourceBillingRatesManagement");

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ResourceBillingRatesManagement", "Exception occured on GetAllAssignmentCostSetup, " + ex.Message);
            }

            return ds;
        }

        public DataTable GetResourceBillingRatesAllDocumentsList()
        {
            DataTable dt = new DataTable();
            Common cmn = new Common();
            try

            {
                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                dt = HANADAL.GetDataTableByStoredProcedure("GetResourceBillingRatesAllDocuments", "ResourceBillingRatesManagement");
                dt = cmn.RemoveDuplicateRows(dt, "DocNum");
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ResourceBillingRatesManagement", "Exception occured on GetAllAssignmentCostSetup, " + ex.Message);
            }

            return dt;
        }
        
        #region Translation ResourceBillingRatesSetup"

        private ResourceBillingRates TranslateDataTableToResourceBillingRates(DataTable dt)
        {
            ResourceBillingRates resourceBillingRates = new ResourceBillingRates();

            try
            {
                foreach (DataRow dtRow in dt.Rows)
                {
                   
                    resourceBillingRates.ID = Convert.ToInt32(dtRow["ID"]);
                    resourceBillingRates.DocNum = Convert.ToString(dtRow["DocNum"]);
                    DateTime fdate = Convert.ToDateTime(dtRow["FromDate"]);
                    resourceBillingRates.FromDate = Convert.ToDateTime(fdate.Date);
                    resourceBillingRates.FunctionID = Convert.ToString(dtRow["FunctionID"]);

                    DateTime tdate = Convert.ToDateTime(dtRow["ToDate"]);
                    resourceBillingRates.ToDate = Convert.ToDateTime(tdate.Date);

                    resourceBillingRates.CREATEDBY = Convert.ToInt32(dtRow["CREATEDBY"]);
                    resourceBillingRates.CREATEDDATE = Convert.ToDateTime(dtRow["CREATEDDATE"]);

                    if (dtRow["UPDATEDEDBY"] != DBNull.Value)
                        resourceBillingRates.UPDATEDEDBY = Convert.ToInt32(dtRow["UPDATEDEDBY"]);
                    if (dtRow["UPDATEDDATE"] != DBNull.Value)
                        resourceBillingRates.UPDATEDDATE = Convert.ToDateTime(dtRow["UPDATEDDATE"]);
                    
                    resourceBillingRates.ISDELETED = Convert.ToBoolean(dtRow["ISDELETED"]);
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ResourceBillingRatesManagement", "Exception occured on TranslateDataTableToResourceBillingRates, " + ex.Message);
            }

            return resourceBillingRates;
        }

        private List<ResourceBillingRatesDetail> TranslateDataTableToResourceBillingRatesDetail(DataTable dt)
        {
            List<ResourceBillingRatesDetail> resourceBillingRatesList = new List<ResourceBillingRatesDetail>();

            try
            {
                int sno = 1;
                foreach (DataRow dtRow in dt.Rows)
                {
                    ResourceBillingRatesDetail resourceBillingRates = new ResourceBillingRatesDetail();
                    resourceBillingRates.SNO = sno;
                    resourceBillingRates.KEY = Guid.NewGuid().ToString();
                    resourceBillingRates.ID = Convert.ToInt32(dtRow["ID"]);
                    resourceBillingRates.DesignationID = Convert.ToInt32(dtRow["DesignationID"]);
                    resourceBillingRates.HeaderID = Convert.ToInt32(dtRow["HeaderID"]);
                    resourceBillingRates.RatesPerHour = Convert.ToDouble(dtRow["RatesPerHour"]);
                    resourceBillingRates.IsDeleted = Convert.ToBoolean(dtRow["IsDeleted"]);
                    resourceBillingRates.IsActive = Convert.ToBoolean(dtRow["IsActive"]);
                    sno = sno + 1;
                    resourceBillingRatesList.Add(resourceBillingRates);
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ResourceBillingRatesManagement", "Exception occured on TranslateDataTableToResourceBillingRatesDetail, " + ex.Message);
            }

            return resourceBillingRatesList;
        }
        private List<B1SP_Parameter> TranslateResourceBillingRatesSetupToParameterList(ResourceBillingRates resourceBillingRates)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {

                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = Convert.ToString(resourceBillingRates.ID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "DocNum";
                parm.ParameterValue = Convert.ToString(resourceBillingRates.DocNum);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "FuncationID";
                parm.ParameterValue = Convert.ToString(resourceBillingRates.FunctionID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "FromDate";
                parm.ParameterValue = Convert.ToString(resourceBillingRates.FromDate.ToString("yyyy-MM-dd"));
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ToDate";
                parm.ParameterValue = Convert.ToString(resourceBillingRates.ToDate.ToString("yyyy-MM-dd"));
                parmList.Add(parm);
                

                parm = new B1SP_Parameter();
                parm.ParameterName = "ISDELETED";
                parm.ParameterValue = Convert.ToString(resourceBillingRates.ISDELETED);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CREATEDBY";
                parm.ParameterValue = Convert.ToString(resourceBillingRates.CREATEDBY);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CREATEDDATE";
                parm.ParameterValue = DateTime.Now.ToString("yyyy-MM-dd");
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "UPDATEDEDBY";
                parm.ParameterValue = Convert.ToString(resourceBillingRates.UPDATEDEDBY);
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
                log.InputOutputDocLog("ResourceBillingRatesManagement", "Exception occured on TranslateResourceBillingRatesSetupToParameterList, " + ex.Message);
            }


            return parmList;
        }
        private List<B1SP_Parameter> TranslateResourceBillingRatesDetailToParameterList(ResourceBillingRatesDetail resourceBillingRates)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {

                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = Convert.ToString(resourceBillingRates.ID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "HeaderID";
                parm.ParameterValue = Convert.ToString(resourceBillingRates.HeaderID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "DesignationID";
                parm.ParameterValue = Convert.ToString(resourceBillingRates.DesignationID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "RatesPerHour";
                parm.ParameterValue = Convert.ToString(resourceBillingRates.RatesPerHour);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "IsDeleted";
                parm.ParameterValue = Convert.ToString(resourceBillingRates.IsDeleted);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "IsActive";
                parm.ParameterValue = Convert.ToString(resourceBillingRates.IsActive);
                parmList.Add(parm);



            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ResourceBillingRatesManagement", "Exception occured on TranslateResourceBillingRatesDetailToParameterList, " + ex.Message);
            }


            return parmList;
        }

        private List<B1SP_Parameter> TranslateResourceBillingRatesLogToParameterList(ResourceBillingRates resourceBillingRates)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {

                B1SP_Parameter parm = new B1SP_Parameter();
                parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = Convert.ToString(resourceBillingRates.ID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "DocNum";
                parm.ParameterValue = Convert.ToString(resourceBillingRates.DocNum);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "FunctionID";
                parm.ParameterValue = Convert.ToString(resourceBillingRates.FunctionID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "FromDate_Previous";
                parm.ParameterValue = Convert.ToString(resourceBillingRates.FromDate.ToString("yyyy-MM-dd"));
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ToDate_Previous";
                parm.ParameterValue = Convert.ToString(resourceBillingRates.ToDate.ToString("yyyy-MM-dd"));
                parmList.Add(parm);
                

                parm = new B1SP_Parameter();
                parm.ParameterName = "IsDeleted_Previous";
                parm.ParameterValue = Convert.ToString(resourceBillingRates.ISDELETED);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "FromDate_New";
                parm.ParameterValue = Convert.ToString(resourceBillingRates.FromDate.ToString("yyyy-MM-dd"));
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ToDate_New";
                parm.ParameterValue = Convert.ToString(resourceBillingRates.ToDate.ToString("yyyy-MM-dd"));
                parmList.Add(parm);


                parm = new B1SP_Parameter();
                parm.ParameterName = "IsDeleted_New";
                parm.ParameterValue = Convert.ToString(resourceBillingRates.ISDELETED);
                parmList.Add(parm);

                
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ResourceBillingRatesManagement", "Exception occured on TranslateResourceBillingRatesDetailLogToParameterList, " + ex.Message);
            }


            return parmList;
        }
        private List<B1SP_Parameter> TranslateResourceBillingRatesDetailLogToParameterList(ResourceBillingRatesDetail resourceBillingRates)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {

                B1SP_Parameter parm = new B1SP_Parameter();

                parm = new B1SP_Parameter();
                parm.ParameterName = "HeaderID";
                parm.ParameterValue = Convert.ToString(resourceBillingRates.HeaderID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "DesignationID";
                parm.ParameterValue = Convert.ToString(resourceBillingRates.DesignationID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "RatesPerHour_Previous";
                parm.ParameterValue = Convert.ToString(resourceBillingRates.RatesPerHour);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ISACTIVE_Previous";
                parm.ParameterValue = Convert.ToString(resourceBillingRates.IsActive);
                parmList.Add(parm);


                parm = new B1SP_Parameter();
                parm.ParameterName = "ISDELETED_Previous";
                parm.ParameterValue = Convert.ToString(resourceBillingRates.IsDeleted);
                parmList.Add(parm);


                parm = new B1SP_Parameter();
                parm.ParameterName = "RatesPerHour_New";
                parm.ParameterValue = Convert.ToString(resourceBillingRates.RatesPerHour);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ISACTIVE_New";
                parm.ParameterValue = Convert.ToString(resourceBillingRates.IsActive);
                parmList.Add(parm);


                parm = new B1SP_Parameter();
                parm.ParameterName = "ISDELETED_New";
                parm.ParameterValue = Convert.ToString(resourceBillingRates.IsDeleted);
                parmList.Add(parm);


            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("ResourceBillingRatesManagement", "Exception occured on TranslateResourceBillingRatesSetupLogToParameterList, " + ex.Message);
            }


            return parmList;
        }
        #endregion
    }
}