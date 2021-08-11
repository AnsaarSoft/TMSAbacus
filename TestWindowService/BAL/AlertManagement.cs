using AlertWindowService.Helper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMSDeloitte.BAL;
using TMSDeloitte.DAL;
using TMSDeloitte.Helper;
using TMSDeloitte.Models;

namespace AlertWindowService.BAL
{
    public class AlertManagement
    {
        public void SendAlertAndNotification(int AlertSetupID)
        {
            Log log = new Log();
            string frequencyType = "";
            try
            {
                AlertSetup alertSetup = new AlertSetup();
                List<UserProfile> userList = new List<UserProfile>();
                List<GroupSetupInfo> groupList = new List<GroupSetupInfo>();
                AlertSetupManagement alertSetupMgt = new AlertSetupManagement();

                alertSetupMgt.GetAlertSetupByDocID(AlertSetupID, out alertSetup, out userList, out groupList);
                if(alertSetup !=null)
                {
                    frequencyType = alertSetup.FrequencyType; 

                    if (alertSetup.IsActive == false)
                    {
                        throw new Exception("Alert setip has been in active, ID: " + alertSetup.ID);
                    }
                       

                    if (!string.IsNullOrEmpty(alertSetup.Query))
                    {
                        HANA_DAL hana_DAL = new HANA_DAL();
                        DataTable dt = hana_DAL.GetDataTableByQuery(alertSetup.Query, "AlertManagement");
                        if(dt.Rows.Count>0)
                        {
                            string htmlTable = Common.ExportDatatableToHtml(dt);
                            string jsonTable = JsonConvert.SerializeObject(dt); 
                            string guid = Guid.NewGuid().ToString();
                            string folderPath = Setting.AlertAndNotificationPath;
                            if(string.IsNullOrEmpty(folderPath))
                                throw new Exception("Getting empty folder path from config file.");
                            string sndNotification = Setting.SndNotification;
                            string sndEmail = Setting.SndEmail;

                            

                            #region For Notification


                            log.DebugDocLog(frequencyType, "guid: " + guid + Environment.NewLine + "jsonTable: " + jsonTable);

                            if (saveTable(folderPath, guid, jsonTable, frequencyType))
                            {
                                NotificationManagement notifiMgt = new NotificationManagement();
                                List<int> sendedNotification = new List<int>();
                                if (userList != null)
                                {
                                    foreach (var user in userList)
                                    {
                                        log.DebugDocLog(frequencyType, "User ID: " + user.ID+ " , IsNotification: "+ user.IsNotification);

                                        if (user.IsNotification)
                                        {
                                            bool isUserExist = sendedNotification.Contains(user.ID);
                                            if (!isUserExist)
                                            {
                                                notifiMgt.AddNotification(-1, user.ID, alertSetup.AlertName, guid, true);
                                                sendedNotification.Add(user.ID);
                                            }
                                        }
                                    }
                                }
                                if (groupList != null)
                                {
                                    foreach (var group in groupList)
                                    {
                                        if(group.IsNotification)
                                        {
                                            log.DebugDocLog(frequencyType, "group ID: " + group.ID + " , IsNotification: " + group.IsNotification);

                                            foreach (var groupUser in group.Table)
                                            {
                                                bool isUserExist = sendedNotification.Contains(Convert.ToInt32(groupUser.UserID));
                                                if (!isUserExist)
                                                {
                                                    notifiMgt.AddNotification(-1, Convert.ToInt32(groupUser.UserID), alertSetup.AlertName, guid, true);
                                                    sendedNotification.Add(Convert.ToInt32(groupUser.UserID));
                                                }
                                            }
                                        }
                                        
                                    }
                                }

                            }
                            #endregion

                            #region For Email

                            log.DebugDocLog(frequencyType, "htmlTable: " + htmlTable);

                            List<int> sendedEmail = new List<int>();
                            Email email = new Email();
                            string msg = "";
                            foreach (var user in userList)
                            {
                                log.DebugDocLog(frequencyType, "User ID: " + user.ID + " , IsEmail: " + user.IsEmail);

                                if (user.IsEmail)
                                {
                                    bool isUserExist = sendedEmail.Contains(user.ID);
                                    if (!isUserExist)
                                    {
                                        if (!string.IsNullOrEmpty(user.EMAIL))
                                        {
                                            bool isSend = email.SendEmail(user.EMAIL, htmlTable, alertSetup.AlertName, "", out msg);
                                            sendedEmail.Add(user.ID);
                                            if (isSend == false)
                                                log.DebugDocLog(frequencyType, "Email not send to the user id: " + user.ID +
                                                    Environment.NewLine + "AlertName: " + alertSetup.AlertName +
                                                    Environment.NewLine + "Email: " + user.EMAIL +
                                                    Environment.NewLine + "Body: " + htmlTable +
                                                    Environment.NewLine + "Exception: " + msg);
                                        }

                                    }
                                }
                               

                            }
                            foreach (var group in groupList)
                            {
                                log.DebugDocLog(frequencyType, "group ID: " + group.ID + " , IsEmail: " + group.IsEmail);
                                if(group.IsEmail)
                                {
                                    foreach (var groupUser in group.Table)
                                    {
                                        bool isUserExist = sendedEmail.Contains(Convert.ToInt32(groupUser.UserID));
                                        if (!isUserExist)
                                        {
                                            UserManagement userMgt = new UserManagement();
                                            UserProfile userprofile = userMgt.GetUserByID(Convert.ToInt32(groupUser.UserID));
                                            email.SendEmail(userprofile.EMAIL, htmlTable, alertSetup.AlertName, "", out msg);
                                            sendedEmail.Add(Convert.ToInt32(groupUser.UserID));
                                        }
                                    }
                                }
                                
                            }
                            #endregion


                            if (alertSetup.FrequencyType== "One time")
                            {
                                log.DebugDocLog(frequencyType, "InActiveAlertSetup of ID: "+ alertSetup.ID);
                                alertSetupMgt.InActiveAlertSetup(alertSetup);

                            }

                        }
                        else
                            throw new Exception("Alert setup has no rows , AlertSetupID:" + AlertSetupID);
                    }
                    else
                        throw new Exception("Alert setup has null or empty query , AlertSetupID:" + AlertSetupID);
                }
                else
                    throw new Exception("Alert setup not found , AlertSetupID:" + AlertSetupID);
            }
            catch (Exception ex)
            {
                log.LogFile(ex.Message);
                log.InputOutputDocLog("AlertManagement", "Exception occured on SendAlertAndNotification"+Environment.NewLine+"Exception: " + ex.Message);
                if (frequencyType != "")
                    log.DebugDocLog(frequencyType, ex.Message);
            }
        }

        public bool saveTable(string folderPath,string fileName,string htmlTable,string docType)
        {
            bool isSaved = false;
            try
            {
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                string path = folderPath + "/" + fileName + ".txt";
                if (!File.Exists(path))
                    File.Create(path).Close();

                using (StreamWriter log = File.AppendText(path))
                {
                    // Write to the file:
                    //log.WriteLine("Data Time:" + DateTime.Now);
                    log.WriteLine(htmlTable);
                    // Close the stream:
                    log.Close();
                }
                isSaved = true;
            }
            catch (Exception ex)
            {
                isSaved = false;
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("AlertManagement", "Exception occured on saveTable , folderPath: "+ folderPath+ " , fileName: "+ fileName+" , htmlTable: "+ htmlTable 
                    + Environment.NewLine + "Exception: " + ex.Message);

                log.DebugDocLog(docType, "Exception occured on saveTable , folderPath: " + folderPath + " , fileName: " + fileName + " , htmlTable: " + htmlTable
                    + Environment.NewLine + "Exception: " + ex.Message);
            }
            return isSaved;
        }
    }
}
