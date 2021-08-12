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

    public class NonChargableSetupManagement
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
                log.InputOutputDocLog("NonChargableSetupManagement", "Exception occured on TranslateNonChargableToParameterList, " + ex.Message);
            }


            return parmList;
        }

        #region "NonChargableSetupInfo"

        public List<NonChargableSetupInfo> GetNonChargeableSetup(int id = 0)
        {
            List<NonChargableSetupInfo> nonChargableSetupInfo = new List<NonChargableSetupInfo>();
            try
            {
                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                DataTable dt_NonChargable = HANADAL.GetDataTableByStoredProcedure("GetNonChargeableSetup", TranslateIDToParameterList(id), "NonChargableSetupManagement");
                if (dt_NonChargable.Rows.Count > 0)
                {
                    nonChargableSetupInfo = TranslateDataTableToNonChargableSetupManagementList(dt_NonChargable);
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("NonChargableSetupManagement", "Exception occured on GetNonChargeableSetup ID: " + id + " , " + ex.Message);
            }

            return nonChargableSetupInfo;
        }
        public bool AddUpdateNonChargableSetup(List<NonChargableSetupInfo> nonChargableSetupInfo,out string msg)
        {
            bool isSuccess = false;
            bool isAddOccured = false;
            bool isUpdateOccured = false;
            bool isDeleteOccured = false;
             msg = "Successfully Added/Updated";
            try
            {
                var filtererdList = nonChargableSetupInfo.Where(x => x.ISDELETED == false).ToList();
                var duplicates = filtererdList.GroupBy(x => x.NCTASKS).Where(g => g.Count() > 1).Select(y => y.Key).ToList();
                if (duplicates.Count > 0)
                {
                    msg = "Duplicate record exist!";
                    return false;
                }

                nonChargableSetupInfo = nonChargableSetupInfo.Where(x => x.ID > 0 || x.ISDELETED == false).ToList();

                //For Form Log
                AddNonChargableSetup_Log(nonChargableSetupInfo, out isUpdateOccured);
                //

                foreach (var list in nonChargableSetupInfo)
                {
                    try
                    {
                        HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                        DataTable dt_NonChargable = HANADAL.AddUpdateDataByStoredProcedure("AddUpdateNonChargeableSetup", TranslateNonChargableToParameterList(list), "NonChargableSetupManagement");
                        if (dt_NonChargable.Rows.Count == 0)
                            throw new Exception("Exception occured when Add/Update Non Chargable setup, ID:" + list.ID + " , NCTASKS:" + list.NCTASKS);
                    }
                    catch (Exception ex)
                    {
                        isSuccess = false;
                        Log log = new Log();
                        log.LogFile(ex.Message);
                        log.InputOutputDocLog("NonChargableSetupManagement", "Exception occured in foreach loop AddUpdateNonChargableSetup, " + ex.Message);
                        continue;
                    }
                }

                //For Master Log
                if (nonChargableSetupInfo.Where(x => x.ID == 0).ToList().Count > 0)
                    isAddOccured = true;
                if (nonChargableSetupInfo.Where(x => x.ISDELETED == true).ToList().Count > 0)
                    isDeleteOccured = true;

                int createdBy = 0;
                var val = nonChargableSetupInfo.Where(x => x.CREATEDBY != null).FirstOrDefault();
                if (val != null)
                    createdBy = Convert.ToInt32(val.CREATEDBY);

                Common cmn = new Common();
                Task.Run(() => cmn.AddMasterLog(isAddOccured, isUpdateOccured, isDeleteOccured, nameof(Enums.FormsName.NonChargeableSetup), createdBy, "NonChargableSetupManagement"));
                //End MAster Log


                isSuccess = true;
            }
            catch (Exception ex)
            {
                isSuccess = false;
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("NonChargableSetupManagement", "Exception occured on AddUpdateNonChargableSetup, " + ex.Message);
            }

            return isSuccess;
        }

        public void AddNonChargableSetup_Log(List<NonChargableSetupInfo> nonChargableSetupInfo, out bool isUpdateOccured)
        {
            isUpdateOccured = false;
            try
            {
                nonChargableSetupInfo = nonChargableSetupInfo.Where(x => x.ID > 0).ToList();

                foreach (var newObject in nonChargableSetupInfo)
                {

                    try
                    {
                        List<NonChargableSetupInfo> previousObject = GetNonChargeableSetup(Convert.ToInt32(newObject.ID));
                        List<B1SP_Parameter> paramList = TranslateNonChargableLogToParameterList(newObject);
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
                                    case "NCTASKS":
                                        paramList.Where(x => x.ParameterName == "NCTASKS_PREVIOUS").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                        paramList.Where(x => x.ParameterName == "NCTASKS_NEW").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();

                                        break;
                                    case "ISACTIVE":
                                        paramList.Where(x => x.ParameterName == "ISACTIVE_PREVIOUS").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                        paramList.Where(x => x.ParameterName == "ISACTIVE_NEW").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();

                                        break;
                                    case "ISDELETED":
                                        paramList.Where(x => x.ParameterName == "ISDELETED_PREVIOUS").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                        paramList.Where(x => x.ParameterName == "ISDELETED_NEW").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();

                                        break;
                                }

                            }

                            if (isChangeOccured)
                            {
                                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                                Task.Run(() => HANADAL.AddUpdateDataByStoredProcedure("AddNonChargeableSetup_LOG", paramList, "NonChargableSetupManagement"));
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        Log log = new Log();
                        log.LogFile(ex.Message);
                        log.InputOutputDocLog("NonChargableSetupManagement", "Exception occured in foreach loop AddNonChargableSetup_Log, " + ex.Message);
                        continue;
                    }


                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("NonChargableSetupManagement", "Exception occured on AddNonChargableSetup_Log, " + ex.Message);
            }
        }

        public DataTable GetNonChargeableSetupLog()
        {
            DataTable dt_NonChargableSetupLog = new DataTable();
            try
            {
                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                dt_NonChargableSetupLog = HANADAL.GetDataTableByStoredProcedure("GetNonChargeableSetupLog", "NonChargableSetupManagement");

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("NonChargableSetupManagement", "Exception occured on GetNonChargeableSetupLog, " + ex.Message);
            }

            return dt_NonChargableSetupLog;
        }
        #endregion

        #region "Translation NonChargableSetupInfo"


        private List<NonChargableSetupInfo> TranslateDataTableToNonChargableSetupManagementList(DataTable dt)
        {
            List<NonChargableSetupInfo> nonChargableSetupInfo = new List<NonChargableSetupInfo>();

            try
            {
                int sno = 1;
                foreach (DataRow dtRow in dt.Rows)
                {
                    NonChargableSetupInfo typeOfClaimSetupInfo = new NonChargableSetupInfo();
                    typeOfClaimSetupInfo.SNO = sno;
                    typeOfClaimSetupInfo.KEY = Guid.NewGuid().ToString();
                    typeOfClaimSetupInfo.ID = Convert.ToInt32(dtRow["ID"]);
                    typeOfClaimSetupInfo.NCTASKS = Convert.ToString(dtRow["NCTASKS"]);
                    typeOfClaimSetupInfo.CREATEDBY = Convert.ToInt32(dtRow["CREATEDBY"]);
                    typeOfClaimSetupInfo.CREATEDDATE = Convert.ToDateTime(dtRow["CREATEDDATE"]);

                    if (dtRow["UPDATEDEDBY"] != DBNull.Value)
                        typeOfClaimSetupInfo.UPDATEDEDBY = Convert.ToInt32(dtRow["UPDATEDEDBY"]);
                    if (dtRow["UPDATEDDATE"] != DBNull.Value)
                        typeOfClaimSetupInfo.UPDATEDDATE = Convert.ToDateTime(dtRow["UPDATEDDATE"]);

                    typeOfClaimSetupInfo.ISACTIVE = Convert.ToBoolean(dtRow["ISACTIVE"]);
                    typeOfClaimSetupInfo.ISDELETED = Convert.ToBoolean(dtRow["ISDELETED"]);
                    sno = sno + 1;
                    nonChargableSetupInfo.Add(typeOfClaimSetupInfo);
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("NonChargableSetupManagement", "Exception occured on TranslateDataTableTotypeOfClaimSetupInfoList, " + ex.Message);
            }

            return nonChargableSetupInfo;
        }

        private List<B1SP_Parameter> TranslateNonChargableToParameterList(NonChargableSetupInfo typeOfClaimSetupInfo)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {

                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = Convert.ToString(typeOfClaimSetupInfo.ID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "NCTASKS";
                parm.ParameterValue = Convert.ToString(typeOfClaimSetupInfo.NCTASKS);
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ISACTIVE";
                parm.ParameterValue = Convert.ToString(typeOfClaimSetupInfo.ISACTIVE);
                parm.ParameterType = DBTypes.Bool.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ISDELETED";
                parm.ParameterValue = Convert.ToString(typeOfClaimSetupInfo.ISDELETED);
                parm.ParameterType = DBTypes.Bool.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CREATEDBY";
                parm.ParameterValue = Convert.ToString(typeOfClaimSetupInfo.CREATEDBY);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CREATEDDATE";
                parm.ParameterValue = DateTime.Now.ToString("yyyy-MM-dd HH:MM:s");
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "UPDATEDDATE";
                parm.ParameterValue = DateTime.Now.ToString("yyyy-MM-dd HH:MM:s");
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "UPDATEDEDBY";
                parm.ParameterValue = Convert.ToString(typeOfClaimSetupInfo.UPDATEDEDBY);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("NonChargableSetupManagement", "Exception occured on TranslateNonChargableToParameterList, " + ex.Message);
            }


            return parmList;
        }

        private List<B1SP_Parameter> TranslateNonChargableLogToParameterList(NonChargableSetupInfo typeOfClaimSetupInfo)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {

                B1SP_Parameter parm = new B1SP_Parameter();

                parm = new B1SP_Parameter();
                parm.ParameterName = "NCTASKS_PREVIOUS";
                parm.ParameterValue = Convert.ToString(typeOfClaimSetupInfo.NCTASKS);
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "NCTASKS_NEW";
                parm.ParameterValue = Convert.ToString(typeOfClaimSetupInfo.NCTASKS);
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ISACTIVE_PREVIOUS";
                parm.ParameterValue = Convert.ToString(typeOfClaimSetupInfo.ISACTIVE);
                parm.ParameterType = DBTypes.Bool.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ISACTIVE_NEW";
                parm.ParameterValue = Convert.ToString(typeOfClaimSetupInfo.ISACTIVE);
                parm.ParameterType = DBTypes.Bool.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ISDELETED_PREVIOUS";
                parm.ParameterValue = Convert.ToString(typeOfClaimSetupInfo.ISDELETED);
                parm.ParameterType = DBTypes.Bool.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ISDELETED_NEW";
                parm.ParameterValue = Convert.ToString(typeOfClaimSetupInfo.ISDELETED);
                parm.ParameterType = DBTypes.Bool.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CREATEDBY";
                parm.ParameterValue = Convert.ToString(typeOfClaimSetupInfo.CREATEDBY);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CREATEDDATE";
                parm.ParameterValue = DateTime.Now.ToString("yyyy-MM-dd HH:MM:s");
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("NonChargableSetupManagement", "Exception occured on TranslateNonChargableLogToParameterList, " + ex.Message);
            }


            return parmList;
        }

        #endregion

    }
}