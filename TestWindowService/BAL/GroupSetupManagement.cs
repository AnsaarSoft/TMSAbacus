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
    public class GroupSetupManagement
    {

        private List<B1SP_Parameter> TranslateIDToParameterList(int Id)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {


                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = Convert.ToString(Id);
                parmList.Add(parm);


            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("GroupSetupManagement", "Exception occured on TranslateGroupSetupToParameterList, " + ex.Message);
            }


            return parmList;
        }

        private List<B1SP_Parameter> TranslateGroupSetupIDToParameterList(int GROUPSETUP_ID)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {

                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "GROUPSETUP_ID";
                parm.ParameterValue = Convert.ToString(GROUPSETUP_ID);
                parmList.Add(parm);


            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("GroupSetupManagement", "Exception occured on TranslateGroupSetupToParameterList, " + ex.Message);
            }


            return parmList;
        }
        
        public List<GroupSetupInfo> GetGroupSetup(int ID)
        {
            List<GroupSetupInfo> groupSetup = new List<GroupSetupInfo>();
            List<GroupSetupChildInfo> groupSetupChild = new List<GroupSetupChildInfo>();
            try
            {
                HANA_DAL HANADAL = new HANA_DAL();
                DataTable dt_GroupSetup = HANADAL.GetDataTableByStoredProcedure("GetGroup_Setup", TranslateIDToParameterList(ID), "GroupSetupManagement");

                if (dt_GroupSetup.Rows.Count > 0)
                {
                    groupSetup = TranslateDataTableToGroupSetupManagementList(dt_GroupSetup);
                    foreach (GroupSetupInfo item in groupSetup)
                    {
                        int Group_Setup_ID = Convert.ToInt32(item.ID);
                        DataTable dt_GroupSetupChild = HANADAL.GetDataTableByStoredProcedure("GetGroup_Setup_Child", TranslateGroupSetupIDToParameterList(Group_Setup_ID), "GroupSetupManagement");

                        if (dt_GroupSetupChild.Rows.Count>0)
                        {
                            groupSetupChild = TranslateDataTableToGroupSetupChildManagementList(dt_GroupSetupChild);
                        }
                        item.Table = groupSetupChild;
                    }
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("GroupSetupManagement", "Exception occured on GetMaster_Task ID: " + ID + " , " + ex.Message);
            }

            return groupSetup;
        }

      

        #region "Translation groupSetup"


        private List<GroupSetupInfo> TranslateDataTableToGroupSetupManagementList(DataTable dt)
        {
            List<GroupSetupInfo> groupSetupL = new List<GroupSetupInfo>();

            try
            {
                int sno = 1;
                foreach (DataRow dtRow in dt.Rows)
                {
                    GroupSetupInfo groupSetup = new GroupSetupInfo();
                    //groupSetup.SNO = sno;
                    groupSetup.KEY = Guid.NewGuid().ToString();
                    groupSetup.ID = Convert.ToInt32(dtRow["ID"]);
                    groupSetup.GROUPCODE= Convert.ToString(dtRow["GROUPCODE"]);
                    groupSetup.GROUPNAME= Convert.ToString(dtRow["GROUPNAME"]);
                    groupSetup.DOCNUM = Convert.ToString(dtRow["DOCNUM"]);
                    groupSetup.CREATEDBY = Convert.ToInt32(dtRow["CREATEDBY"]);
                    groupSetup.CREATEDATE = Convert.ToDateTime(dtRow["CREATEDATE"]);

                    if (dtRow["UPDATEDBY"] != DBNull.Value)
                        groupSetup.UPDATEDBY = Convert.ToInt32(dtRow["UPDATEDBY"]);
                    if (dtRow["UPDATEDATE"] != DBNull.Value)
                        groupSetup.UPDATEDATE = Convert.ToDateTime(dtRow["UPDATEDATE"]);

                    groupSetup.ISACTIVE = Convert.ToBoolean(dtRow["ISACTIVE"]);
                    groupSetup.ISDELETED = Convert.ToBoolean(dtRow["ISDELETED"]);
                    sno = sno + 1;
                    groupSetupL.Add(groupSetup);
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("GroupSetupManagement", "Exception occured on TranslateDataTableToGroupSetupManagementList, " + ex.Message);
            }

            return groupSetupL;
        }

        private List<GroupSetupChildInfo> TranslateDataTableToGroupSetupChildManagementList(DataTable dt)
        {
            List<GroupSetupChildInfo> groupSetupL = new List<GroupSetupChildInfo>();

            try
            {
                int sno = 1;
                foreach (DataRow dtRow in dt.Rows)
                {
                    GroupSetupChildInfo groupSetup = new GroupSetupChildInfo();
                    groupSetup.SNO = sno;
                    groupSetup.KEY = Guid.NewGuid().ToString();
                    groupSetup.ID = Convert.ToInt32(dtRow["ID"]);

                    groupSetup.USER_CODE= Convert.ToString(dtRow["USER_CODE"]);
                    groupSetup.USER_NAME= Convert.ToString(dtRow["USER_NAME"]);
                    groupSetup.UserID = Convert.ToInt32(dtRow["UserID"]);
                    groupSetup.BRANCHID = Convert.ToInt32(dtRow["BRANCHID"]);
                    groupSetup.BRANCHNAME = Convert.ToString(dtRow["BRANCHNAME"]);
                    groupSetup.DESIGNATIONID= Convert.ToInt32(dtRow["DESIGNATIONID"]);
                    groupSetup.DESIGNATIONNAME= Convert.ToString(dtRow["DESIGNATIONNAME"]);
                    groupSetup.DEPARTMENTID= Convert.ToInt32(dtRow["DEPARTMENTID"]);
                    groupSetup.DEPARTMENTNAME = Convert.ToString(dtRow["DEPARTMENTNAME"]);
                    //groupSetup.DOCNUM = Convert.ToString(dtRow["DOCNUM"]);
                    groupSetup.CREATEDBY = Convert.ToInt32(dtRow["CREATEDBY"]);
                    groupSetup.CREATEDATE = Convert.ToDateTime(dtRow["CREATEDATE"]);

                    if (dtRow["UPDATEDBY"] != DBNull.Value)
                        groupSetup.UPDATEDBY = Convert.ToInt32(dtRow["UPDATEDBY"]);
                    if (dtRow["UPDATEDATE"] != DBNull.Value)
                        groupSetup.UPDATEDATE = Convert.ToDateTime(dtRow["UPDATEDATE"]);
                    
                    groupSetup.ISDELETED = Convert.ToBoolean(dtRow["ISDELETED"]);
                    sno = sno + 1;
                    groupSetupL.Add(groupSetup);
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("GroupSetupManagement", "Exception occured on TranslateDataTableToGroupSetupManagementList, " + ex.Message);
            }

            return groupSetupL;
        }

        #endregion

    }
}