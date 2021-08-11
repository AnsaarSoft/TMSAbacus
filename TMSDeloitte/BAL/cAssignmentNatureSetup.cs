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

    public class cAssignmentNatureSetup
    {
        #region "AssignmentNaturetSetup"
        public List<AssignmentNatureSetup> GetAssignmentNatureSetup(int id = 0)
        {
            List<AssignmentNatureSetup> AssignmentNatureSetupList = new List<AssignmentNatureSetup>();
            try
            {
                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                Common cmn = new Common();
                List<HCM_Designation> designationList = cmn.GetHCMDesignationList();
                DataTable dt_AssignmentCostSetup = HANADAL.GetDataTableByStoredProcedure("GetAssignmentNatureSetup", cmn.TranslateIDToParameterList(id), "AssignmentNatureSetupManagement");
                if (dt_AssignmentCostSetup.Rows.Count > 0)
                {
                    AssignmentNatureSetupList = TranslateDataTableToAssignmentNatureSetupList(dt_AssignmentCostSetup);
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("AssignmentNatureSetupManagement", "Exception occured on GetAllAssignmentNatureSetup, " + ex.Message);
            }

            return AssignmentNatureSetupList;
        }
        public bool AddUpdateAssignmentNatureSetup(List<AssignmentNatureSetup> AssignmentNatureSetupList,out string msg)
        {
            bool isSuccess = false;
            bool isAddOccured = false;
            bool isUpdateOccured = false;
            bool isDeleteOccured = false;
            msg = "Successfully Added/Updated";
            try
            {
                var filtererdList = AssignmentNatureSetupList.Where(x => x.IsDeleted == false).ToList();
                var duplicates = filtererdList.GroupBy(x => x.AssignmentNature).Where(g => g.Count() > 1).Select(y => y.Key).ToList();
                if (duplicates.Count > 0)
                {
                    msg = "Duplicate record exist!";
                    return false;
                }


                AssignmentNatureSetupList = AssignmentNatureSetupList.Where(x => x.ID > 0 || x.IsDeleted == false).ToList();

                //For Form Log
                AddAssignmentNatureSetup_Log(AssignmentNatureSetupList, out isUpdateOccured);
                //

                foreach (var list in AssignmentNatureSetupList)
                {
                    try
                    {
                        HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                        DataTable dt_AssignmentCostSetup = HANADAL.AddUpdateDataByStoredProcedure("AddUpdateAssignmentNatureSetup", TranslateAssignmentNatureSetupToParameterList(list), "AssignmentNatureSetupManagement");
                        if (dt_AssignmentCostSetup.Rows.Count == 0)
                            throw new Exception("Exception occured when Add/Update assignment cost setup, ID:" + list.ID + " , Type Of Cost:" + list.Type);
                    }
                    catch (Exception ex)
                    {
                        isSuccess = false;
                        Log log = new Log();
                        log.LogFile(ex.Message);
                        log.InputOutputDocLog("AssignmentNatureSetupManagement", "Exception occured in foreach loop AddUpdateAssignmentNatureSetup, " + ex.Message);
                        continue;
                    }
                }

                //For Master Log
                if (AssignmentNatureSetupList.Where(x => x.ID == 0).ToList().Count > 0)
                    isAddOccured = true;
                if (AssignmentNatureSetupList.Where(x => x.IsDeleted == true).ToList().Count > 0)
                    isDeleteOccured = true;

                int createdBy = 0;
                var val = AssignmentNatureSetupList.Where(x => x.CreatedBy != null).FirstOrDefault();
                if (val != null)
                    createdBy = Convert.ToInt32(val.CreatedBy);

                Common cmn = new Common();
                Task.Run(() => cmn.AddMasterLog(isAddOccured, isUpdateOccured, isDeleteOccured, nameof(Enums.FormsName.AssignmentCostSetup), createdBy, "AssignmentNatureSetupManagement"));
                //End MAster Log


                isSuccess = true;
            }
            catch (Exception ex)
            {
                isSuccess = false;
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("AssignmentNatureSetupManagement", "Exception occured on AddUpdateAssignmentCostSetup, " + ex.Message);
            }

            return isSuccess;
        }

        public void AddAssignmentNatureSetup_Log(List<AssignmentNatureSetup> AssignmentNatureSetupList, out bool isUpdateOccured)
        {
            isUpdateOccured = false;
            try
            {
                AssignmentNatureSetupList = AssignmentNatureSetupList.Where(x => x.ID > 0).ToList();

                foreach (var newObject in AssignmentNatureSetupList)
                {

                    try
                    {
                        List<AssignmentNatureSetup> previousObject = GetAssignmentNatureSetup(Convert.ToInt32(newObject.ID));
                        List<B1SP_Parameter> paramList = TranslateAssignmentNatureSetupLogToParameterList(newObject);

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
                                    case "Type":
                                        paramList.Where(x => x.ParameterName == "Type_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                        paramList.Where(x => x.ParameterName == "Type_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();

                                        break;

                                    case "AssignmentNature":
                                        paramList.Where(x => x.ParameterName == "AssignmentNature_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                        paramList.Where(x => x.ParameterName == "AssignmentNature_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();

                                        break;

                                    case "IsActive":
                                        paramList.Where(x => x.ParameterName == "IsActive_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                        paramList.Where(x => x.ParameterName == "IsActive_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();

                                        break;
                                    case "IsDeleted":
                                        paramList.Where(x => x.ParameterName == "IsDeleted_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                        paramList.Where(x => x.ParameterName == "IsDeleted_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();

                                        break;
                                }

                            }

                            if (isChangeOccured)
                            {
                                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                                Task.Run(() => HANADAL.AddUpdateDataByStoredProcedure("AddAssignmentNatureSetup_Log", paramList, "SetupManagement"));
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        Log log = new Log();
                        log.LogFile(ex.Message);
                        log.InputOutputDocLog("SetupManagement", "Exception occured in foreach loop AddAssignmentCostSetup_Log, " + ex.Message);
                        continue;
                    }


                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("SetupManagement", "Exception occured on AddAssignmentCostSetup_Log, " + ex.Message);
            }
        }



        public DataTable GetAssignmentNatureSetupLog()
        {
            DataTable dt_TypeOfClaimSetupLog = new DataTable();
            try
            {
                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                dt_TypeOfClaimSetupLog = HANADAL.GetDataTableByStoredProcedure("GetAssignmentNatureSetupLog", "AssignmentNatureSetupManagement");

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("AssignmentNatureSetupManagement", "Exception occured on GetAssignmentNatureSetupLog, " + ex.Message);
            }

            return dt_TypeOfClaimSetupLog;
        }

        #endregion

        #region "Translation AssignmentNatureSetup"

        private List<AssignmentNatureSetup> TranslateDataTableToAssignmentNatureSetupList(DataTable dt)
        {
            List<AssignmentNatureSetup> AssignmentNatureSetupList = new List<AssignmentNatureSetup>();

            try
            {
                int sno = 1;
                foreach (DataRow dtRow in dt.Rows)
                {
                    AssignmentNatureSetup assignmentCostSetup = new AssignmentNatureSetup();
                    assignmentCostSetup.SNO = sno;
                    assignmentCostSetup.KEY = Guid.NewGuid().ToString();
                    assignmentCostSetup.ID = Convert.ToInt32(dtRow["ID"]);
                    assignmentCostSetup.Type = Convert.ToString(dtRow["Type"]);
                    assignmentCostSetup.AssignmentNature = Convert.ToString(dtRow["AssignmentNature"]);
                    assignmentCostSetup.CreatedBy = Convert.ToInt32(dtRow["CreatedBy"]);
                    assignmentCostSetup.CreatedDate = Convert.ToDateTime(dtRow["CreatedDate"]);

                    if (dtRow["UpdatedBy"] != DBNull.Value)
                        assignmentCostSetup.UpdatedBy = Convert.ToInt32(dtRow["UpdatedBy"]);
                    if (dtRow["UpdateDate"] != DBNull.Value)
                        assignmentCostSetup.UpdatedDate = Convert.ToDateTime(dtRow["UpdateDate"]);

                    assignmentCostSetup.IsActive = Convert.ToBoolean(dtRow["IsActive"]);
                    assignmentCostSetup.IsDeleted = Convert.ToBoolean(dtRow["IsDeleted"]);
                    sno = sno + 1;
                    AssignmentNatureSetupList.Add(assignmentCostSetup);
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("AssignmentNatureSetupManagement", "Exception occured on TranslateDataTableToAssignmentNatureSetupList, " + ex.Message);
            }

            return AssignmentNatureSetupList;
        }

        private List<B1SP_Parameter> TranslateAssignmentNatureSetupToParameterList(AssignmentNatureSetup objAssignmentNatureSetup)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {

                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = Convert.ToString(objAssignmentNatureSetup.ID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "Type";
                parm.ParameterValue = Convert.ToString(objAssignmentNatureSetup.Type);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "AssignmentNature";
                parm.ParameterValue = Convert.ToString(objAssignmentNatureSetup.AssignmentNature);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "IsActive";
                parm.ParameterValue = Convert.ToString(objAssignmentNatureSetup.IsActive);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "isDeleted";
                parm.ParameterValue = Convert.ToString(objAssignmentNatureSetup.IsDeleted);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CreatedBy";
                parm.ParameterValue = Convert.ToString(objAssignmentNatureSetup.CreatedBy);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CreatedDate";
                parm.ParameterValue = DateTime.Now.ToString("yyyy-MM-dd HH:MM:s");
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "UpdatedBy";
                parm.ParameterValue = Convert.ToString(objAssignmentNatureSetup.UpdatedBy);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "UpdatedDate";
                parm.ParameterValue = DateTime.Now.ToString("yyyy-MM-dd HH:MM:s");
                parmList.Add(parm);
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("AssignmentNatureSetupManagement", "Exception occured on TranslateAssignmentNatureSetupToParameterList, " + ex.Message);
            }


            return parmList;
        }

        private List<B1SP_Parameter> TranslateAssignmentNatureSetupLogToParameterList(AssignmentNatureSetup objAssignmentNatureSetup)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {

                B1SP_Parameter parm = new B1SP_Parameter();

                parm = new B1SP_Parameter();
                parm.ParameterName = "Type_Previous";
                parm.ParameterValue = Convert.ToString(objAssignmentNatureSetup.Type);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "Type_New";
                parm.ParameterValue = Convert.ToString(objAssignmentNatureSetup.Type);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "AssignmentNature_Previous";
                parm.ParameterValue = Convert.ToString(objAssignmentNatureSetup.AssignmentNature);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "AssignmentNature_New";
                parm.ParameterValue = Convert.ToString(objAssignmentNatureSetup.AssignmentNature);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "IsActive_Previous";
                parm.ParameterValue = Convert.ToString(objAssignmentNatureSetup.IsDeleted);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "IsActive_New";
                parm.ParameterValue = Convert.ToString(objAssignmentNatureSetup.IsDeleted);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "IsDeleted_Previous";
                parm.ParameterValue = Convert.ToString(objAssignmentNatureSetup.IsActive);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "IsDeleted_New";
                parm.ParameterValue = Convert.ToString(objAssignmentNatureSetup.IsActive);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CreatedBy";
                parm.ParameterValue = Convert.ToString(objAssignmentNatureSetup.CreatedBy);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CreatedDate";
                parm.ParameterValue = DateTime.Now.ToString("yyyy-MM-dd HH:MM:s");
                parmList.Add(parm);

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("AssignmentNatureSetupManagement", "Exception occured on TranslateAssignmentNatureSetupLogToParameterList, " + ex.Message);
            }


            return parmList;
        }

        #endregion
    }
}