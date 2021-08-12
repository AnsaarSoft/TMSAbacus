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

    public class TypeOfClaimManagement
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
                log.InputOutputDocLog("TypeOfClaimManagement", "Exception occured on TranslateTypeOfClaimToParameterList, " + ex.Message);
            }


            return parmList;
        }

        #region "TypeOfClaimSetupInfo"

        public List<TypeOfClaimSetupInfo> GetTypeOfClaimSetup(int id = 0)
        {
            List<TypeOfClaimSetupInfo> typeofclaimsetupinfo = new List<TypeOfClaimSetupInfo>();
            try
            {
                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                DataTable dt_TypeOfClaim = HANADAL.GetDataTableByStoredProcedure("GetTypeOfClaimSetup", TranslateIDToParameterList(id), "TypeOfClaimManagement");
                if (dt_TypeOfClaim.Rows.Count > 0)
                {
                    typeofclaimsetupinfo = TranslateDataTableToTypeOfClaimManagementList(dt_TypeOfClaim);
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("TypeOfClaimManagement", "Exception occured on GetTypeOfClaimSetup ID: " + id + " , " + ex.Message);
            }

            return typeofclaimsetupinfo;
        }
        public bool AddUpdateTypeOfClaimSetup(List<TypeOfClaimSetupInfo> typeofclaimsetupinfo,out string msg)
        {
            bool isSuccess = false;
            bool isAddOccured = false;
            bool isUpdateOccured = false;
            bool isDeleteOccured = false;
            msg = "Successfully Added/Updated";
            try
            {
                var filtererdList = typeofclaimsetupinfo.Where(x => x.ISDELETED == false).ToList();
                var duplicates = filtererdList.GroupBy(x => x.TYPEOFCLAIM).Where(g => g.Count() > 1).Select(y => y.Key).ToList();
                if (duplicates.Count > 0)
                {
                    msg = "Duplicate record exist!";
                    return false;
                }

                typeofclaimsetupinfo = typeofclaimsetupinfo.Where(x => x.ID > 0 || x.ISDELETED == false).ToList();

                //For Form Log
                AddTypeOfClaimSetup_Log(typeofclaimsetupinfo, out isUpdateOccured);
                //

                foreach (var list in typeofclaimsetupinfo)
                {
                    try
                    {
                        HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                        DataTable dt_TypeOfClaim = HANADAL.AddUpdateDataByStoredProcedure("AddUpdateTypeOfClaimSetup", TranslateTypeOfClaimToParameterList(list), "TypeOfClaimManagement");
                        if (dt_TypeOfClaim.Rows.Count == 0)
                            throw new Exception("Exception occured when Add/Update Type of claim setup, ID:" + list.ID + " , TYPE OF CLAIM:" + list.TYPEOFCLAIM);
                    }
                    catch (Exception ex)
                    {
                        isSuccess = false;
                        Log log = new Log();
                        log.LogFile(ex.Message);
                        log.InputOutputDocLog("TypeOfClaimManagement", "Exception occured in foreach loop AddUpdateTypeOfClaimSetup, " + ex.Message);
                        continue;
                    }
                }

                //For Master Log
                if (typeofclaimsetupinfo.Where(x => x.ID == 0).ToList().Count > 0)
                    isAddOccured = true;
                if (typeofclaimsetupinfo.Where(x => x.ISDELETED == true).ToList().Count > 0)
                    isDeleteOccured = true;

                int createdBy = 0;
                var val = typeofclaimsetupinfo.Where(x => x.CREATEDBY != null).FirstOrDefault();
                if (val != null)
                    createdBy = Convert.ToInt32(val.CREATEDBY);

                Common cmn = new Common();
                Task.Run(() => cmn.AddMasterLog(isAddOccured, isUpdateOccured, isDeleteOccured, nameof(Enums.FormsName.TypeOfClaimSetup), createdBy, "TypeOfClaimManagement"));
                //End MAster Log


                isSuccess = true;
            }
            catch (Exception ex)
            {
                isSuccess = false;
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("TypeOfClaimManagement", "Exception occured on AddUpdateTypeOfClaimSetup, " + ex.Message);
            }

            return isSuccess;
        }

        public void AddTypeOfClaimSetup_Log(List<TypeOfClaimSetupInfo> typeofclaimsetupinfo, out bool isUpdateOccured)
        {
            isUpdateOccured = false;
            try
            {
                typeofclaimsetupinfo = typeofclaimsetupinfo.Where(x => x.ID > 0).ToList();

                foreach (var newObject in typeofclaimsetupinfo)
                {

                    try
                    {
                        List<TypeOfClaimSetupInfo> previousObject = GetTypeOfClaimSetup(Convert.ToInt32(newObject.ID));
                        List<B1SP_Parameter> paramList = TranslateTypeOfClaimLogToParameterList(newObject);
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
                                    case "TYPEOFCLAIM":
                                        paramList.Where(x => x.ParameterName == "TYPEOFCLAIM_PREVIOUS").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                        paramList.Where(x => x.ParameterName == "TYPEOFCLAIM_NEW").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();

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
                                Task.Run(() => HANADAL.AddUpdateDataByStoredProcedure("AddTypeOfClaimSetup_LOG", paramList, "TypeOfClaimManagement"));
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        Log log = new Log();
                        log.LogFile(ex.Message);
                        log.InputOutputDocLog("TypeOfClaimManagement", "Exception occured in foreach loop AddTypeOfClaimSetup_Log, " + ex.Message);
                        continue;
                    }


                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("TypeOfClaimManagement", "Exception occured on AddTypeOfClaimSetup_Log, " + ex.Message);
            }
        }

        public DataTable GetTypeOfClaimSetupLog()
        {
            DataTable dt_TypeOfClaimSetupLog = new DataTable();
            try
            {
                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                dt_TypeOfClaimSetupLog = HANADAL.GetDataTableByStoredProcedure("GetTypeOfClaimSetupLog", "TypeOfClaimManagement");

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("TypeOfClaimManagement", "Exception occured on GetTypeOfClaimSetupLog, " + ex.Message);
            }

            return dt_TypeOfClaimSetupLog;
        }
        #endregion

        #region "Translation TypeOfClaimSetupInfo"


        private List<TypeOfClaimSetupInfo> TranslateDataTableToTypeOfClaimManagementList(DataTable dt)
        {
            List<TypeOfClaimSetupInfo> typeofclaimsetupinfo = new List<TypeOfClaimSetupInfo>();

            try
            {
                int sno = 1;
                foreach (DataRow dtRow in dt.Rows)
                {
                    TypeOfClaimSetupInfo typeOfClaimSetupInfo = new TypeOfClaimSetupInfo();
                    typeOfClaimSetupInfo.SNO = sno;
                    typeOfClaimSetupInfo.KEY = Guid.NewGuid().ToString();
                    typeOfClaimSetupInfo.ID = Convert.ToInt32(dtRow["ID"]);
                    typeOfClaimSetupInfo.TYPEOFCLAIM = Convert.ToString(dtRow["TYPEOFCLAIM"]);
                    typeOfClaimSetupInfo.CREATEDBY = Convert.ToInt32(dtRow["CREATEDBY"]);
                    typeOfClaimSetupInfo.CREATEDDATE = Convert.ToDateTime(dtRow["CREATEDDATE"]);

                    if (dtRow["UPDATEDEDBY"] != DBNull.Value)
                        typeOfClaimSetupInfo.UPDATEDEDBY = Convert.ToInt32(dtRow["UPDATEDEDBY"]);
                    if (dtRow["UPDATEDDATE"] != DBNull.Value)
                        typeOfClaimSetupInfo.UPDATEDDATE = Convert.ToDateTime(dtRow["UPDATEDDATE"]);

                    typeOfClaimSetupInfo.ISACTIVE = Convert.ToBoolean(dtRow["ISACTIVE"]);
                    typeOfClaimSetupInfo.ISDELETED = Convert.ToBoolean(dtRow["ISDELETED"]);
                    sno = sno + 1;
                    typeofclaimsetupinfo.Add(typeOfClaimSetupInfo);
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("TypeOfClaimManagement", "Exception occured on TranslateDataTableTotypeOfClaimSetupInfoList, " + ex.Message);
            }

            return typeofclaimsetupinfo;
        }

        private List<B1SP_Parameter> TranslateTypeOfClaimToParameterList(TypeOfClaimSetupInfo typeOfClaimSetupInfo)
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
                parm.ParameterName = "TYPEOFCLAIM";
                parm.ParameterValue = Convert.ToString(typeOfClaimSetupInfo.TYPEOFCLAIM);
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
                log.InputOutputDocLog("TypeOfClaimManagement", "Exception occured on TranslateTypeOfClaimToParameterList, " + ex.Message);
            }


            return parmList;
        }

        private List<B1SP_Parameter> TranslateTypeOfClaimLogToParameterList(TypeOfClaimSetupInfo typeOfClaimSetupInfo)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {

                B1SP_Parameter parm = new B1SP_Parameter();

                parm = new B1SP_Parameter();
                parm.ParameterName = "TYPEOFCLAIM_PREVIOUS";
                parm.ParameterValue = Convert.ToString(typeOfClaimSetupInfo.TYPEOFCLAIM);
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "TYPEOFCLAIM_NEW";
                parm.ParameterValue = Convert.ToString(typeOfClaimSetupInfo.TYPEOFCLAIM);
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
                log.InputOutputDocLog("TypeOfClaimManagement", "Exception occured on TranslateTypeOfClaimLogToParameterList, " + ex.Message);
            }


            return parmList;
        }

        #endregion

    }
}