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
    public class SetupManagement
    {

        #region "AssignmentCostSetup"
        public List<AssignmentCostSetup> GetAssignmentCostSetup(int id=0)
        {
            List<AssignmentCostSetup> assignmentCostSetupList = new List<AssignmentCostSetup>();
            try
            {
                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                Common cmn = new Common();
                DataTable dt_AssignmentCostSetup = HANADAL.GetDataTableByStoredProcedure("GetAssignmentCostSetup", cmn.TranslateIDToParameterList(id), "SetupManagement");
                if (dt_AssignmentCostSetup.Rows.Count > 0)
                {
                    assignmentCostSetupList = TranslateDataTableToAssignmentCostSetupList(dt_AssignmentCostSetup);
                }
            }
            catch(Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("SetupManagement", "Exception occured on GetAssignmentCostSetup ID: " + id + " , " + ex.Message);
            }

            return assignmentCostSetupList;
        }
        public bool AddUpdateAssignmentCostSetup(List<AssignmentCostSetup> assignmentCostSetupList,out string msg)
        {
            bool isSuccess = false;
            bool isAddOccured = false;
            bool isUpdateOccured = false;
            bool isDeleteOccured = false;
            msg = "Successfully Added/Updated";
            try
            {
                var filtererdList = assignmentCostSetupList.Where(x => x.ISDELETED == false).ToList();
                var duplicates = filtererdList.GroupBy(x => x.TYPEOFCOST).Where(g => g.Count() > 1).Select(y => y.Key).ToList();
                if (duplicates.Count>0)
                {
                    msg = "Duplicate record exist!";
                    return false;
                }

                assignmentCostSetupList = assignmentCostSetupList.Where(x => x.ID > 0 || x.ISDELETED == false).ToList();
               
                //For Form Log
                AddAssignmentCostSetup_Log(assignmentCostSetupList,out isUpdateOccured);
                //

                foreach (var list in assignmentCostSetupList)
                {
                    try
                    {
                        HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                        DataTable dt_AssignmentCostSetup = HANADAL.AddUpdateDataByStoredProcedure("AddUpdateAssignmentCostSetup", TranslateAssignmentCostSetupToParameterList(list), "SetupManagement");
                        if (dt_AssignmentCostSetup.Rows.Count == 0)
                            throw new Exception("Exception occured when Add/Update assignment cost setup, ID:" + list.ID + " , Type Of Cost:" + list.TYPEOFCOST);
                    }
                    catch (Exception ex)
                    {
                        isSuccess = false;
                        Log log = new Log();
                        log.LogFile(ex.Message);
                        log.InputOutputDocLog("SetupManagement", "Exception occured in foreach loop AddUpdateAssignmentCostSetup, " + ex.Message);
                        continue;
                    }
                }

                //For Master Log
                if (assignmentCostSetupList.Where(x => x.ID == 0).ToList().Count > 0)
                    isAddOccured = true;
                if (assignmentCostSetupList.Where(x => x.ISDELETED == true).ToList().Count > 0)
                    isDeleteOccured = true;

                int createdBy = 0;
                var val = assignmentCostSetupList.Where(x=>x.CREATEDBY !=null).FirstOrDefault();
                if (val != null)
                    createdBy = Convert.ToInt32(val.CREATEDBY);

                Common cmn = new Common();
                Task.Run(() => cmn.AddMasterLog(isAddOccured,isUpdateOccured,isDeleteOccured,nameof(Enums.FormsName.AssignmentCostSetup),createdBy, "SetupManagement"));
                //End MAster Log


                isSuccess = true;
            }
            catch (Exception ex)
            {
                isSuccess = false;
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("SetupManagement", "Exception occured on AddUpdateAssignmentCostSetup, " + ex.Message);
            }
            
            return isSuccess;
        }

        public void AddAssignmentCostSetup_Log(List<AssignmentCostSetup> assignmentCostSetupList,out bool isUpdateOccured)
        {
            isUpdateOccured = false;
            try
            {
                assignmentCostSetupList = assignmentCostSetupList.Where(x => x.ID > 0).ToList();
                
                foreach (var newObject in assignmentCostSetupList)
                {

                    try
                    {
                        List<AssignmentCostSetup> previousObject = GetAssignmentCostSetup(Convert.ToInt32(newObject.ID));
                        List<B1SP_Parameter> paramList = TranslateAssignmentCostSetupLogToParameterList(newObject);
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
                                    case "TYPEOFCOST":
                                        paramList.Where(x => x.ParameterName == "TYPEOFCOST_PREVIOUS").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                        paramList.Where(x => x.ParameterName == "TYPEOFCOST_NEW").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();

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
                                Task.Run(() => HANADAL.AddUpdateDataByStoredProcedure("AddAssignmentCostSetup_Log", paramList, "SetupManagement"));
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
            catch(Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("SetupManagement", "Exception occured on AddAssignmentCostSetup_Log, " + ex.Message);
            }
        }

        public DataTable GetAssignmentCostSetupLog()
        {
            DataTable dt_AssignmentCostSetupLog = new DataTable();
            try
            {
                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                dt_AssignmentCostSetupLog = HANADAL.GetDataTableByStoredProcedure("GetAssignmentCostSetupLog", "SetupManagement");
               
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("SetupManagement", "Exception occured on GetAllAssignmentCostSetup, " + ex.Message);
            }

            return dt_AssignmentCostSetupLog;
        }

        #region "Translation AssignmentCostSetup"


        private List<AssignmentCostSetup> TranslateDataTableToAssignmentCostSetupList(DataTable dt)
        {
            List<AssignmentCostSetup> assignmentCostSetupList = new List<AssignmentCostSetup>();

            try
            {
                int sno = 1;
                foreach (DataRow dtRow in dt.Rows)
                {
                    AssignmentCostSetup assignmentCostSetup = new AssignmentCostSetup();
                    assignmentCostSetup.SNO = sno;
                    assignmentCostSetup.KEY = Guid.NewGuid().ToString();
                    assignmentCostSetup.ID = Convert.ToInt32(dtRow["ID"]);
                    assignmentCostSetup.TYPEOFCOST = Convert.ToString(dtRow["TYPEOFCOST"]);
                    assignmentCostSetup.CREATEDBY = Convert.ToInt32(dtRow["CREATEDBY"]);
                    assignmentCostSetup.CREATEDDATE = Convert.ToDateTime(dtRow["CREATEDDATE"]);

                    if (dtRow["UPDATEDEDBY"] != DBNull.Value)
                        assignmentCostSetup.UPDATEDEDBY = Convert.ToInt32(dtRow["UPDATEDEDBY"]);
                    if (dtRow["UPDATEDDATE"] != DBNull.Value)
                        assignmentCostSetup.UPDATEDDATE = Convert.ToDateTime(dtRow["UPDATEDDATE"]);

                    assignmentCostSetup.ISACTIVE = Convert.ToBoolean(dtRow["ISACTIVE"]);
                    assignmentCostSetup.ISDELETED = Convert.ToBoolean(dtRow["ISDELETED"]);
                    sno = sno + 1;
                    assignmentCostSetupList.Add(assignmentCostSetup);
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("SetupManagement", "Exception occured on TranslateDataTableToAssignmentCostSetupList, " + ex.Message);
            }

            return assignmentCostSetupList;
        }

        private List<B1SP_Parameter> TranslateAssignmentCostSetupToParameterList(AssignmentCostSetup assignmentCostSetup)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {

                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = Convert.ToString(assignmentCostSetup.ID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "TYPEOFCOST";
                parm.ParameterValue = Convert.ToString(assignmentCostSetup.TYPEOFCOST);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ISACTIVE";
                parm.ParameterValue = Convert.ToString(assignmentCostSetup.ISACTIVE);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ISDELETED";
                parm.ParameterValue = Convert.ToString(assignmentCostSetup.ISDELETED);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CREATEDBY";
                parm.ParameterValue = Convert.ToString(assignmentCostSetup.CREATEDBY);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CREATEDDATE";
                parm.ParameterValue = DateTime.Now.ToString("yyyy-MM-dd HH:MM:s");
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "UPDATEDDATE";
                parm.ParameterValue = DateTime.Now.ToString("yyyy-MM-dd HH:MM:s");
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "UPDATEDEDBY";
                parm.ParameterValue = Convert.ToString(assignmentCostSetup.UPDATEDEDBY);
                parmList.Add(parm);
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("SetupManagement", "Exception occured on TranslateAssignmentCostSetupToParameterList, " + ex.Message);
            }


            return parmList;
        }

        private List<B1SP_Parameter> TranslateAssignmentCostSetupLogToParameterList(AssignmentCostSetup assignmentCostSetup)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {

                B1SP_Parameter parm = new B1SP_Parameter();

                parm = new B1SP_Parameter();
                parm.ParameterName = "TYPEOFCOST_PREVIOUS";
                parm.ParameterValue = Convert.ToString(assignmentCostSetup.TYPEOFCOST);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "TYPEOFCOST_NEW";
                parm.ParameterValue = Convert.ToString(assignmentCostSetup.TYPEOFCOST);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ISACTIVE_PREVIOUS";
                parm.ParameterValue = Convert.ToString(assignmentCostSetup.ISACTIVE);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ISACTIVE_NEW";
                parm.ParameterValue = Convert.ToString(assignmentCostSetup.ISACTIVE);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ISDELETED_PREVIOUS";
                parm.ParameterValue = Convert.ToString(assignmentCostSetup.ISDELETED);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ISDELETED_NEW";
                parm.ParameterValue = Convert.ToString(assignmentCostSetup.ISDELETED);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CREATEDBY";
                parm.ParameterValue = Convert.ToString(assignmentCostSetup.CREATEDBY);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CREATEDDATE";
                parm.ParameterValue = DateTime.Now.ToString("yyyy-MM-dd HH:MM:s");
                parmList.Add(parm);

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("SetupManagement", "Exception occured on TranslateAssignmentCostSetupLogToParameterList, " + ex.Message);
            }


            return parmList;
        }

        #endregion


        #endregion
        
      

     

    }


}