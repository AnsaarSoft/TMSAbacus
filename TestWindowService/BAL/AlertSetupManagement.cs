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
    public class AlertSetupManagement
    {
        
        public List<AlertSetup> GetAllAlertSetupByFrequencyType(string FrequencyType)
        {
            List<AlertSetup> list = new List<AlertSetup>();
            DataTable dt = new DataTable();
            try
            {

                List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();

                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "FrequencyType";
                parm.ParameterValue = Convert.ToString(FrequencyType);
                parmList.Add(parm);
                HANA_DAL HANADAL = new HANA_DAL();

                dt = HANADAL.GetDataTableByStoredProcedure("GetAllAlertSetupByFrequencyType", parmList, "GroupSetupManagement");
                if(dt.Rows.Count>0)
                    list.Add(TranslateDataTableToAlertSetup(dt));
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("GroupSetupManagement", "Exception occured on GetAllAlertSetupByFrequencyType, " + ex.Message);
            }

            return list;
        }

       
        
        public void GetAlertSetupByDocID(int DocID, out AlertSetup obj, out List<UserProfile> userList, out List<GroupSetupInfo> groupList)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            obj = new AlertSetup();
            userList = new List<UserProfile>();
            groupList = new List<GroupSetupInfo>();
            try
            {
                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = Convert.ToString(DocID);
                parmList.Add(parm);

                HANA_DAL HANADAL = new HANA_DAL();
                DataSet ds = HANADAL.GetDataSetByStoredProcedure("GetAlertSetupByDocID", parmList, "AlertSetupManagement");
                if (ds.Tables.Count > 0)
                {

                    obj = TranslateDataTableToAlertSetup(ds.Tables[0]);
                    userList = TranslateDataTableToUserList(ds.Tables[1]);
                    groupList = TranslateDataTableToUserGroupList(ds.Tables[2]);
                }


            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("AlertSetupManagement", "Exception occured on GetAlertSetupByDocNum, " + ex.Message);
            }
        }

        public void InActiveAlertSetup(AlertSetup alertsetup)
        {

            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            B1SP_Parameter parm = new B1SP_Parameter();

            try
            {

                parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = Convert.ToString(alertsetup.ID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "IsActive";
                parm.ParameterValue = Convert.ToString(false);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "UpdatedBy";
                parm.ParameterValue = Convert.ToString(-1);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "UpdatedDate";
                parm.ParameterValue = DateTime.Now.ToString("yyyy-MM-dd HH:MM:s");
                parmList.Add(parm);

                HANA_DAL HANADAL = new HANA_DAL();
                Task.Run(() => HANADAL.AddUpdateDataByStoredProcedure("InActiveAlertSetup", parmList, "AlertSetupManagement"));

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("AlertSetupManagement", "Exception occured on InActiveAlertSetup, " + ex.Message);
            }
        }


        #region Translation


        private AlertSetup TranslateDataTableToAlertSetup(DataTable dt)
        {
            AlertSetup setup = new AlertSetup();
            try
            {

                foreach (DataRow dtRow in dt.Rows)
                {
                    setup.ID = Convert.ToInt32(dtRow["ID"]);
                    setup.AlertName = Convert.ToString(dtRow["AlertName"]);
                    setup.DocNum = Convert.ToString(dtRow["DocNum"]);
                    setup.Frequency = Convert.ToInt32(dtRow["Frequency"]);
                    setup.FrequencyType = Convert.ToString(dtRow["FrequencyType"]);
                    setup.IsActive = Convert.ToBoolean(dtRow["IsActive"]);
                    setup.Query = Convert.ToString(dtRow["Query"]);
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("AlertSetupManagement", "Exception occured on TranslateDataTableToUserProfile, " + ex.Message);
            }

            return setup;
        }

        private List<UserProfile> TranslateDataTableToUserList(DataTable dt)
        {
            List<UserProfile> list = new List<UserProfile>();
            BAL.UserManagement userMgt = new UserManagement();
            try
            {
                int sno = 0;
                foreach (DataRow dtRow in dt.Rows)
                {
                    int userCode = Convert.ToInt32(dtRow["UserID"]);

                    UserProfile userProfile = new UserProfile();
                    if (userCode != 0)
                    {
                        UserProfile userdata = new UserProfile();
                        userProfile = userMgt.GetUserByID(Convert.ToInt32(userCode));
                    }

                    sno = sno + 1;
                    userProfile.SNO = sno;
                    userProfile.KEY = Guid.NewGuid().ToString();
                    userProfile.USER_CODE = Convert.ToInt32(dtRow["UserID"]);
                    userProfile.AlertSetupHeaderTableID = Convert.ToInt32(dtRow["HeaderID"]);
                    userProfile.AlertSetupTableID = Convert.ToInt32(dtRow["ID"]);
                    userProfile.IsNotification = Convert.ToBoolean(dtRow["IsNotification"]);
                    userProfile.IsEmail = Convert.ToBoolean(dtRow["IsEmail"]);
                    userProfile.ISDELETED = Convert.ToBoolean(dtRow["IsDeleted"]);
                    list.Add(userProfile);
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("AlertSetupManagement", "Exception occured on TranslateDataTableToUserProfile, " + ex.Message);
            }

            return list;
        }

       
        private List<GroupSetupInfo> TranslateDataTableToUserGroupList(DataTable dt)
        {
            List<GroupSetupInfo> list = new List<GroupSetupInfo>();
            BAL.GroupSetupManagement mgt = new GroupSetupManagement();
            try
            {
                int sno = 0;

                foreach (DataRow dtRow in dt.Rows)
                {
                    GroupSetupInfo desig = new GroupSetupInfo();
                    desig = mgt.GetGroupSetup(Convert.ToInt32(dtRow["UserGroupID"]))[0];
                    
                    sno = sno + 1;
                    desig.SNO = sno;
                    desig.KEY = Guid.NewGuid().ToString();
                    //desig.GROUPCODE = Convert.ToString(dtRow["UserGroupID"]);
                    
                    desig.AlertSetupHeaderTableID = Convert.ToInt32(dtRow["HeaderID"]);
                    desig.AlertSetupTableID = Convert.ToInt32(dtRow["ID"]);
                    desig.IsNotification = Convert.ToBoolean(dtRow["IsNotification"]);
                    desig.IsEmail = Convert.ToBoolean(dtRow["IsEmail"]);
                    desig.ISDELETED = Convert.ToBoolean(dtRow["IsDeleted"]);
                    list.Add(desig);
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("AlertSetupManagement", "Exception occured on TranslateDataTableToUserProfile, " + ex.Message);
            }

            return list;
        }
      
        #endregion

    }
}