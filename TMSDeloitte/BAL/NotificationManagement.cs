using System;
using System.Collections.Generic;
using System.Configuration;
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
        public void AddNotification(int FromEmpID,List<int> ToEmpIDs,String Detail, string FileName = "", bool isTable = false)
        {
            
            foreach (var ToEmpID in ToEmpIDs)
            {
                AddNotification(FromEmpID, ToEmpID, Detail, FileName,isTable);
            }
        }

        public void AddNotification(int FromEmpID, int ToEmpID, String Detail,string FileName = "",bool isTable=false)
        {

            try
            {
                List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = Convert.ToString(0);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "FromEmpID";
                parm.ParameterValue = Convert.ToString(FromEmpID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ToEmpID";
                parm.ParameterValue = Convert.ToString(ToEmpID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "Detail";
                parm.ParameterValue = Convert.ToString(Detail);
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "Table";
                parm.ParameterValue = Convert.ToString(isTable);
                parm.ParameterType = DBTypes.Bool.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "FileName";
                parm.ParameterValue = Convert.ToString(FileName);
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CreatedDate";
                parm.ParameterValue = DateTime.Now.ToString("yyyy-MM-dd HH:MM:s");
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "Read";
                parm.ParameterValue = Convert.ToString(false);
                parm.ParameterType = DBTypes.Bool.ToString();
                parmList.Add(parm);

                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
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


        public List<Notification> GetNotificationByEmpID(int id,bool isNotificationWindow=false,string fromDate="",string toDate="")
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            List<Notification> NotificationList = new List<Notification>();
           

            B1SP_Parameter parm = new B1SP_Parameter();
            parm.ParameterName = "ID";
            parm.ParameterValue = Convert.ToString(id);
            parm.ParameterType = DBTypes.Int32.ToString();
            parmList.Add(parm);

            string spName = "GetNotificationByEmpID";

            if(isNotificationWindow)
            {
                spName = "GetWindowNotificationByEmpID";
            }
            else
            {
                parm = new B1SP_Parameter();
                parm.ParameterName = "FromDate";
                parm.ParameterValue = Convert.ToString(fromDate);
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ToDate";
                parm.ParameterValue = Convert.ToString(toDate);
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);
            }

            if(spName == "GetNotificationByEmpID" && (fromDate=="" || toDate==""))
            {

                return NotificationList;
            }

            HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
            DataTable dt = HANADAL.GetDataTableByStoredProcedure(spName, parmList, "NotificationManagement");
            if (dt.Rows.Count > 0)
            {
                int sno = 0;
                foreach (DataRow dtRow in dt.Rows)
                {
                    try
                    {
                        sno = sno + 1;

                        UserManagement userMgt = new UserManagement();
                        Notification notification = new Notification();
                        Encrypt_Decrypt security = new Encrypt_Decrypt();
                        notification.SNO = sno;
                        notification.ID = Convert.ToInt32(dtRow["ID"]);

                        if (Convert.ToInt32(dtRow["FromEmpID"])>0)
                        {
                            UserProfile fromUser = new UserProfile();
                            fromUser = userMgt.GetUserByID(Convert.ToInt32(dtRow["FromEmpID"]));
                            notification.FromEmp = fromUser.FULLNAME;
                        }
                        if (Convert.ToInt32(dtRow["FromEmpID"]) == -1)
                            notification.FromEmp = "System Generated Notification";

                        UserProfile toUser = new UserProfile();
                        toUser = userMgt.GetUserByID(Convert.ToInt32(dtRow["ToEmpID"]));
                        notification.ToEmp = toUser.FULLNAME;

                        if(isNotificationWindow)
                            notification.Detail = Convert.ToString(dtRow["Detail"]).Length > 40 ? Convert.ToString(dtRow["Detail"]).Substring(0, 40).ToString() : Convert.ToString(dtRow["Detail"]);
                        else
                            notification.Detail = Convert.ToString(dtRow["Detail"]);


                        notification.Date = Convert.ToDateTime(dtRow["CreatedDate"]).ToString("dd/MM/yyyy HH:mm");
                        notification.Read = Convert.ToBoolean(dtRow["Read"]);
                        notification.Table = Convert.ToBoolean(dtRow["Table"]);
                        //if(notification.Table)
                        //{
                        //    notification.Detail = "System Generated Message Click To View Detail.";
                        //}

                        NotificationList.Add(notification);
                    }
                    catch (Exception ex)
                    {
                        Log log = new Log();
                        log.LogFile(ex.Message);
                        log.InputOutputDocLog("NotificationManagement", "Exception occured on GetNotificationByEmpID, " + ex.Message);
                        continue;
                    }
                }
            }
            return NotificationList;
        }

        public List<Notification> GetNotificationByDocID(int id,int ToEmpID)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            List<Notification> NotificationList = new List<Notification>();


            B1SP_Parameter parm = new B1SP_Parameter();
            parm.ParameterName = "ID";
            parm.ParameterValue = Convert.ToString(id);
            parm.ParameterType = DBTypes.Int32.ToString();
            parmList.Add(parm);

            parm = new B1SP_Parameter();
            parm.ParameterName = "ToEmpID";
            parm.ParameterValue = Convert.ToString(ToEmpID);
                parm.ParameterType = DBTypes.Int32.ToString();
            parmList.Add(parm);


            HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
            DataTable dt = HANADAL.GetDataTableByStoredProcedure("GetNotificationByDocAndEmpID", parmList, "NotificationManagement");
            if (dt.Rows.Count > 0)
            {
                int sno = 0;
                foreach (DataRow dtRow in dt.Rows)
                {
                    try
                    {
                        sno = sno + 1;

                        UserManagement userMgt = new UserManagement();
                        Notification notification = new Notification();
                        Encrypt_Decrypt security = new Encrypt_Decrypt();
                        notification.SNO = sno;
                        notification.ID = Convert.ToInt32(dtRow["ID"]);
                        

                        if (Convert.ToInt32(dtRow["FromEmpID"]) > 0)
                        {
                            UserProfile fromUser = new UserProfile();
                            fromUser = userMgt.GetUserByID(Convert.ToInt32(dtRow["FromEmpID"]));
                            notification.FromEmp = fromUser.FULLNAME;
                        }
                        if (Convert.ToInt32(dtRow["FromEmpID"]) == -1)
                            notification.FromEmp = "System Generated Notification";

                        UserProfile toUser = new UserProfile();
                        toUser = userMgt.GetUserByID(Convert.ToInt32(dtRow["ToEmpID"]));
                        notification.ToEmp = toUser.FULLNAME;
                        notification.Table = Convert.ToBoolean(dtRow["Table"]);
                        notification.Detail = Convert.ToString(dtRow["Detail"]);
                        notification.FileName = Convert.ToString(dtRow["FileName"]);
                        if (notification.Table)
                        {
                            string folderPath = ConfigurationManager.AppSettings["AlertAndNotificationPath"];
                            folderPath = folderPath + "\\" + notification.FileName + ".txt";
                            string text = System.IO.File.ReadAllText(folderPath);
                            notification.FileName = text;
                        }
                       
                        notification.Date = Convert.ToDateTime(dtRow["CreatedDate"]).ToString("dd/MM/yyyy HH:mm");
                        notification.Read = Convert.ToBoolean(dtRow["Read"]);
                        
                        NotificationList.Add(notification);
                    }
                    catch (Exception ex)
                    {
                        Log log = new Log();
                        log.LogFile(ex.Message);
                        log.InputOutputDocLog("NotificationManagement", "Exception occured on GetNotificationByEmpID, " + ex.Message);
                        continue;
                    }
                }
            }
            return NotificationList;
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
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
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
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
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