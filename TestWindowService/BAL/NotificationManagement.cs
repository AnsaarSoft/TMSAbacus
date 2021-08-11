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
    public class NotificationManagement
    {
        public void AddNotification(int FromEmpID, List<int> ToEmpIDs, String Detail, string FileName = "", bool isTable = false)
        {

            foreach (var ToEmpID in ToEmpIDs)
            {
                AddNotification(FromEmpID, ToEmpID, Detail, FileName, isTable);
            }
        }

        public void AddNotification(int FromEmpID, int ToEmpID, String Detail, string FileName = "", bool isTable = false)
        {

            try
            {
                List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = Convert.ToString(0);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "FromEmpID";
                parm.ParameterValue = Convert.ToString(FromEmpID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ToEmpID";
                parm.ParameterValue = Convert.ToString(ToEmpID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "Detail";
                parm.ParameterValue = Convert.ToString(Detail);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "Table";
                parm.ParameterValue = Convert.ToString(isTable);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "FileName";
                parm.ParameterValue = Convert.ToString(FileName);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CreatedDate";
                parm.ParameterValue = DateTime.Now.ToString("yyyy-MM-dd HH:MM:s");
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "Read";
                parm.ParameterValue = Convert.ToString(false);
                parmList.Add(parm);

                HANA_DAL HANADAL = new HANA_DAL();
                Task.Run(() => HANADAL.AddUpdateDataByStoredProcedure("AddNotification", parmList, "NotificationManagement"));


            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("NotificationManagement", "Exception occured on AddNotification, FromEmpID: " + FromEmpID + " , ToEmpID: " + ToEmpID + " , Detail: " + Detail
                    + Environment.NewLine + " Exception:" + ex.Message);
            }

        }
        
        public void UpdateNotificationAsRead(int ID)
        {

            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            B1SP_Parameter parm = new B1SP_Parameter();

            try
            {

                parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = Convert.ToString(ID);
                parmList.Add(parm);

                HANA_DAL HANADAL = new HANA_DAL();
                Task.Run(() => HANADAL.AddUpdateDataByStoredProcedure("UpdateNotificationAsRead", parmList, "NotificationManagement"));

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("NotificationManagement", "Exception occured on UpdateNotificationAsRead, " + ex.Message);
            }
        }

        public void UpdateEmpNotificationAsRead(int empID)
        {

            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            B1SP_Parameter parm = new B1SP_Parameter();

            try
            {

                parm = new B1SP_Parameter();
                parm.ParameterName = "ToEmpID";
                parm.ParameterValue = Convert.ToString(empID);
                parmList.Add(parm);

                HANA_DAL HANADAL = new HANA_DAL();
                Task.Run(() => HANADAL.AddUpdateDataByStoredProcedure("UpdateEmpNotificationAsRead", parmList, "NotificationManagement"));

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("NotificationManagement", "Exception occured on UpdateEmpNotificationAsRead, " + ex.Message);
            }
        }


        public void UpdateNotificationAsRead(List<int> IDs)
        {

            foreach (var ID in IDs)
            {
                UpdateNotificationAsRead(ID);
            }
        }



    }
}