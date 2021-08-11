using AlertWindowService.BAL;
using AlertWindowService.Helper;
using AlertWindowService.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using TMSDeloitte.BAL;
using TMSDeloitte.Helper;
using TMSDeloitte.Models;

namespace AlertWindowService
{
    public partial class Scheduler : ServiceBase
    {
        public Dictionary<int, Timer> timers = new Dictionary<int, Timer>();
        List<string> FrequencyTypeList = new List<string>();
       
        List<AlertSetup> dtMinutes = new List<AlertSetup>();
        List<AlertSetup> dtHours = new List<AlertSetup>();
        List<AlertSetup> dtDays = new List<AlertSetup>();
        List<AlertSetup> dtWeeks = new List<AlertSetup>();
        List<AlertSetup> dtMonths = new List<AlertSetup>();
        List<AlertSetup> dtOne_time = new List<AlertSetup>();

        #region Scheduler Main

        private Timer timer1 = null;

        public Scheduler()
        {
            Library.WriteErrorLog("Alert window service InitializeComponent()");
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            timer1 = new Timer();
            this.timer1.Interval = TimeSpanUtil.ConvertMinutesToMilliseconds(15); //every 30 secs
            this.timer1.Elapsed += new System.Timers.ElapsedEventHandler(this.timer1_Tick);
            timer1.Enabled = true;

            Setting.AlertAndNotificationPath = ConfigurationManager.AppSettings["AlertAndNotificationPath"];
            Setting.SndNotification = ConfigurationManager.AppSettings["SndNotification"];
            Setting.SndEmail = ConfigurationManager.AppSettings["SndEmail"];

            FrequencyTypeList.Add("Minutes");
            FrequencyTypeList.Add("Hours");
            FrequencyTypeList.Add("Days");
            FrequencyTypeList.Add("Weeks");
            FrequencyTypeList.Add("Months");
            FrequencyTypeList.Add("One time");


            TMSAlertService();
            Library.WriteErrorLog("Alert window service started");
        }

        private void timer1_Tick(object sender, ElapsedEventArgs e)
        {
            

            TMSAlertService();
            Library.WriteErrorLog("Timer ticked and alert service called ....");
        }

        protected override void OnStop()
        {
            timer1.Enabled = false;
            Library.WriteErrorLog("Alert window service stopped");
        }

        #endregion

        #region TMSAlertService
        

        public void TMSAlertService()
        {
            //////For Testing Locally
            //Setting.AlertAndNotificationPath = ConfigurationManager.AppSettings["AlertAndNotificationPath"];
            //Setting.SndNotification = ConfigurationManager.AppSettings["SndNotification"];
            //Setting.SndEmail = ConfigurationManager.AppSettings["SndEmail"];

            //FrequencyTypeList.Add("Minutes");
            //FrequencyTypeList.Add("Hours");
            //FrequencyTypeList.Add("Days");
            //FrequencyTypeList.Add("Weeks");
            //FrequencyTypeList.Add("Months");
            //FrequencyTypeList.Add("One time");
            

            GetAlertSeupList();

            ////For Testing Locally
            //while (true)
            //{
            //    System.Threading.Thread.Sleep(Convert.ToInt32(TimeSpanUtil.ConvertMinutesToMilliseconds(120)));
            //    Task.Run(() => GetAlertSeupList());
            //}
        }

        public void GetAlertSeupList()
        {
            Log log = new Log();
            AlertSetupManagement mgt = new AlertSetupManagement();

            foreach (var FrequencyType in FrequencyTypeList)
            {
                Timer tim = new Timer();
                log.DebugDocLog(FrequencyType, "Getting Alert Setup BY Sp");

                switch (FrequencyType)
                {
                    case "Minutes":
                        List<AlertSetup> minuteList = mgt.GetAllAlertSetupByFrequencyType(FrequencyType);
                        if (dtMinutes.Count == 0)
                            dtMinutes = minuteList;
                        else
                        {
                            if(minuteList.Count==0 && dtMinutes.Count>0)
                            {
                                foreach (var minuteItems in dtMinutes)
                                {
                                    DisableAlertTimerEvent(minuteItems.ID, "Minutes");
                                }
                            }
                            else
                            {
                                foreach (var minuteItems in minuteList)
                                {
                                    var ifExist = dtMinutes.Where(x => x.ID == minuteItems.ID).FirstOrDefault();
                                    if (ifExist == null)
                                    {
                                        dtMinutes.Add(minuteItems);

                                        AlertSetup ifFreqTypeChanged = new AlertSetup();
                                        ifFreqTypeChanged = dtHours.Where(x => x.ID == minuteItems.ID).FirstOrDefault();
                                        if(ifFreqTypeChanged==null)
                                            ifFreqTypeChanged = dtDays.Where(x => x.ID == minuteItems.ID).FirstOrDefault();
                                        if (ifFreqTypeChanged == null)
                                            ifFreqTypeChanged = dtWeeks.Where(x => x.ID == minuteItems.ID).FirstOrDefault();
                                        if (ifFreqTypeChanged == null)
                                            ifFreqTypeChanged = dtMonths.Where(x => x.ID == minuteItems.ID).FirstOrDefault();
                                        if (ifFreqTypeChanged != null)
                                        {
                                            DisableAlertTimerEvent(minuteItems.ID, "Minutes");
                                        }
                                    }
                                    else
                                    {
                                        UpdateList(minuteItems, ifExist);
                                        minuteItems.Frequency = ifExist.Frequency;
                                        minuteItems.IsActive = ifExist.IsActive;
                                        //dtMinutes.Add(ifExist);
                                    }

                                }
                            }
                        }

                        break;

                    case "Hours":
                        List<AlertSetup> hourList = mgt.GetAllAlertSetupByFrequencyType(FrequencyType);
                        if (dtHours.Count == 0)
                            dtHours = hourList;
                        else
                        {
                            if (hourList.Count == 0 && dtHours.Count > 0)
                            {
                                foreach (var hoursItems in dtHours)
                                {
                                    DisableAlertTimerEvent(hoursItems.ID, "Hours");
                                }
                            }
                            else
                            {
                                foreach (var hoursItems in hourList)
                                {
                                    var ifExist = dtHours.Where(x => x.ID == hoursItems.ID).FirstOrDefault();
                                    if (ifExist == null)
                                    {
                                        dtHours.Add(hoursItems);

                                        AlertSetup ifFreqTypeChanged = new AlertSetup();
                                        ifFreqTypeChanged = dtMinutes.Where(x => x.ID == hoursItems.ID).FirstOrDefault();
                                        if (ifFreqTypeChanged == null)
                                            ifFreqTypeChanged = dtDays.Where(x => x.ID == hoursItems.ID).FirstOrDefault();
                                        if (ifFreqTypeChanged == null)
                                            ifFreqTypeChanged = dtWeeks.Where(x => x.ID == hoursItems.ID).FirstOrDefault();
                                        if (ifFreqTypeChanged == null)
                                            ifFreqTypeChanged = dtMonths.Where(x => x.ID == hoursItems.ID).FirstOrDefault();
                                        if (ifFreqTypeChanged != null)
                                        {
                                            DisableAlertTimerEvent(hoursItems.ID, "Hours");
                                        }
                                    }
                                    else
                                    {
                                        UpdateList(hoursItems, ifExist);
                                        hoursItems.Frequency = ifExist.Frequency;
                                        hoursItems.IsActive = ifExist.IsActive;
                                        //dtHours.Add(ifExist);
                                    }

                                }
                            }
                        }

                        break;

                    case "Days":

                        List<AlertSetup> daysList = mgt.GetAllAlertSetupByFrequencyType(FrequencyType);
                        if (dtDays.Count == 0)
                            dtDays = daysList;
                        else
                        {
                            if (daysList.Count == 0 && dtDays.Count > 0)
                            {
                                foreach (var daysItems in dtDays)
                                {
                                    DisableAlertTimerEvent(daysItems.ID, "Days");
                                }
                            }
                            else
                            {
                                foreach (var daysItems in daysList)
                                {
                                    var ifExist = dtDays.Where(x => x.ID == daysItems.ID).FirstOrDefault();
                                    if (ifExist == null)
                                    {
                                        dtDays.Add(daysItems);

                                        AlertSetup ifFreqTypeChanged = new AlertSetup();
                                        ifFreqTypeChanged = dtMinutes.Where(x => x.ID == daysItems.ID).FirstOrDefault();
                                        if (ifFreqTypeChanged == null)
                                            ifFreqTypeChanged = dtHours.Where(x => x.ID == daysItems.ID).FirstOrDefault();
                                        if (ifFreqTypeChanged == null)
                                            ifFreqTypeChanged = dtWeeks.Where(x => x.ID == daysItems.ID).FirstOrDefault();
                                        if (ifFreqTypeChanged == null)
                                            ifFreqTypeChanged = dtMonths.Where(x => x.ID == daysItems.ID).FirstOrDefault();
                                        if (ifFreqTypeChanged != null)
                                        {
                                            DisableAlertTimerEvent(daysItems.ID, "Days");
                                        }
                                    }
                                    else
                                    {
                                        UpdateList(daysItems, ifExist);
                                        daysItems.Frequency = ifExist.Frequency;
                                        daysItems.IsActive = ifExist.IsActive;
                                        //dtDays.Add(ifExist);
                                    }

                                }
                            }
                        }


                        break;

                    case "Weeks":
                        List<AlertSetup> weeksList = mgt.GetAllAlertSetupByFrequencyType(FrequencyType);
                        if (dtWeeks.Count == 0)
                            dtWeeks = weeksList;
                        else
                        {
                            if (weeksList.Count == 0 && dtWeeks.Count > 0)
                            {
                                foreach (var weekItems in dtWeeks)
                                {
                                    DisableAlertTimerEvent(weekItems.ID, "Weeks");
                                }
                            }
                            else
                            {
                                foreach (var weekItems in weeksList)
                                {
                                    var ifExist = dtWeeks.Where(x => x.ID == weekItems.ID).FirstOrDefault();
                                    if (ifExist == null)
                                    {
                                        dtWeeks.Add(weekItems);

                                        AlertSetup ifFreqTypeChanged = new AlertSetup();
                                        ifFreqTypeChanged = dtMinutes.Where(x => x.ID == weekItems.ID).FirstOrDefault();
                                        if (ifFreqTypeChanged == null)
                                            ifFreqTypeChanged = dtHours.Where(x => x.ID == weekItems.ID).FirstOrDefault();
                                        if (ifFreqTypeChanged == null)
                                            ifFreqTypeChanged = dtDays.Where(x => x.ID == weekItems.ID).FirstOrDefault();
                                        if (ifFreqTypeChanged == null)
                                            ifFreqTypeChanged = dtMonths.Where(x => x.ID == weekItems.ID).FirstOrDefault();
                                        if (ifFreqTypeChanged != null)
                                        {
                                            DisableAlertTimerEvent(weekItems.ID, "Weeks");
                                        }
                                    }
                                    else
                                    {
                                        UpdateList(weekItems, ifExist);
                                        weekItems.Frequency = ifExist.Frequency;
                                        weekItems.IsActive = ifExist.IsActive;
                                        //dtWeeks.Add(ifExist);
                                    }

                                }
                            }
                        }
                        break;

                    case "Months":
                        List<AlertSetup> monthsList = mgt.GetAllAlertSetupByFrequencyType(FrequencyType);
                        if (dtMonths.Count == 0)
                            dtMonths = monthsList;
                        else
                        {
                            if (monthsList.Count == 0 && dtMonths.Count > 0)
                            {
                                foreach (var monthItems in dtMonths)
                                {
                                    DisableAlertTimerEvent(monthItems.ID, "Months");
                                }
                            }
                            else
                            {
                                foreach (var monthItems in monthsList)
                                {
                                    var ifExist = dtMonths.Where(x => x.ID == monthItems.ID).FirstOrDefault();
                                    if (ifExist == null)
                                    {
                                        dtMonths.Add(monthItems);

                                        AlertSetup ifFreqTypeChanged = new AlertSetup();
                                        ifFreqTypeChanged = dtMinutes.Where(x => x.ID == monthItems.ID).FirstOrDefault();
                                        if (ifFreqTypeChanged == null)
                                            ifFreqTypeChanged = dtHours.Where(x => x.ID == monthItems.ID).FirstOrDefault();
                                        if (ifFreqTypeChanged == null)
                                            ifFreqTypeChanged = dtDays.Where(x => x.ID == monthItems.ID).FirstOrDefault();
                                        if (ifFreqTypeChanged == null)
                                            ifFreqTypeChanged = dtWeeks.Where(x => x.ID == monthItems.ID).FirstOrDefault();
                                        if (ifFreqTypeChanged != null)
                                        {
                                            DisableAlertTimerEvent(monthItems.ID, "Months");
                                        }
                                    }
                                    else
                                    {
                                        UpdateList(monthItems, ifExist);
                                        monthItems.Frequency = ifExist.Frequency;
                                        monthItems.IsActive = ifExist.IsActive;
                                        //dtMonths.Add(ifExist);
                                    }

                                }
                            }
                        }
                        break;

                    case "One time":
                        dtOne_time = mgt.GetAllAlertSetupByFrequencyType(FrequencyType);
                        foreach (var item in dtOne_time)
                        {
                            log.DebugDocLog("One time", "tickHandeler_Months Started Of Alert Setup ID:  " + item.ID);
                            AlertManagement alertMgt = new AlertManagement();
                            Task.Run(() => alertMgt.SendAlertAndNotification(item.ID));
                        }
                        dtOne_time = new List<AlertSetup>();
                        break;

                    default:
                        break;
                }
            }
            CreateAlertSetupTimers();
        }

        public void CreateAlertSetupTimers()
        {
            Task.Run(()=> CreateMinuteTimer());
            Task.Run(() => CreateHoursTimer());
            Task.Run(() => CreateDaysTimer());
            Task.Run(() => CreateWeeksTimer());
            Task.Run(() => CreateMonthsTimer());
        }
        
        public void UpdateList(AlertSetup newObject,AlertSetup previousObject)
        {
            try
            {
                if (newObject != null && previousObject != null)
                {
                    bool isChangeOccured = false; 
                    foreach (PropertyCompareResult resultItem in PropertyCompare.Compare(previousObject, newObject))
                    {
                        string Name = resultItem.Name;
                        object PreviousValue = resultItem.OldValue;
                        object NewValue = resultItem.NewValue;
                      
                        switch (Name)
                        {
                            case "Frequency":
                                isChangeOccured = true;
                               
                                break;

                            case "IsActive":
                                isChangeOccured = true;
                                newObject.IsActive = previousObject.IsActive;
                                break;

                        }

                    }
                    if(isChangeOccured)
                    {
                        if (timers.Count > 0)
                        {
                            DisableAlertTimerEvent(newObject.ID, "Minutes");

                        }

                    }

                }
            }
            catch (Exception ex)
            {
                if(!string.IsNullOrEmpty(previousObject.FrequencyType))
                {
                    Log log = new Log();
                    log.DebugDocLog(previousObject.FrequencyType, "Exception occured at UpdateList method, " + ex.Message);
                }
               
               
            }
        }

        public void DisableAlertTimerEvent(int alertID,string FrequencyType)
        {
            Log log = new Log();
            try
            {
                Timer tim = new Timer();
                if (timers.Count > 0)
                {
                    tim = timers[alertID];
                    tim.Enabled = false;
                    tim.Dispose();
                    timers.Remove(alertID);
                    InActiveAlert(alertID, FrequencyType);
                    log.DebugDocLog(FrequencyType, "Removed Alert From List and Timer Tic Event Handler , Alert ID: " + alertID);
                }

            }
            catch (Exception ex)
            {
                log.DebugDocLog(FrequencyType, "Exception occured when Removed Alert From List and Timer Tic Event Handler, Alert ID: " + FrequencyType +
                Environment.NewLine + ex.Message);
            }
        }

        public void InActiveAlert(int id,string frequencyType)
        {
            Log log = new Log(); 
            try
            {
                log.DebugDocLog(frequencyType, "Removing Alert From List and Timer Tic Event Handler , Alert ID: " + id);
                switch (frequencyType)
                {
                    case "Minutes":
                        dtMinutes.Where(x=>x.ID==id).Select(c => { c.IsActive = false; return c; }).ToList();
                        dtMinutes = dtMinutes.Where(x => x.IsActive == true).ToList();
                        break;
                    case "Hours":
                        dtHours.Where(x => x.ID == id).Select(c => { c.IsActive = false; return c; }).ToList();
                        dtHours = dtHours.Where(x => x.IsActive == true).ToList();
                        break;
                    case "Days":
                        dtDays.Where(x => x.ID == id).Select(c => { c.IsActive = false; return c; }).ToList();
                        dtDays = dtDays.Where(x => x.IsActive == true).ToList();
                        break;
                    case "Weeks":
                        dtWeeks.Where(x => x.ID == id).Select(c => { c.IsActive = false; return c; }).ToList();
                        dtWeeks = dtWeeks.Where(x => x.IsActive == true).ToList();
                        break;
                    case "Months":
                        dtMinutes.Where(x => x.ID == id).Select(c => { c.IsActive = false; return c; }).ToList();
                        dtMinutes = dtMinutes.Where(x => x.IsActive == true).ToList();
                        break;
                    case "One time":
                        dtOne_time.Where(x => x.ID == id).Select(c => { c.IsActive = false; return c; }).ToList();
                        dtOne_time = dtOne_time.Where(x => x.IsActive == true).ToList();
                        break;
                    default:
                        break;
                }

                
            }
            catch(Exception ex)
            {
                log.DebugDocLog(frequencyType, "Exception At InActiveAlert: " + ex.Message);
            }
        }


        #region For Minute

        public void CreateMinuteTimer()
        {
            Log log = new Log();
            try
            {
                var groupedList_Frequency = dtMinutes.Where(x => x.IsActive == true).ToList()
                                  .GroupBy(u => u.Frequency)
                                  .Select(grp => grp.ToList())
                                  .ToList();

                log.DebugDocLog("Minutes", "CreateMinuteTimer Count: " + groupedList_Frequency.Count);

                foreach (var itemList_Frequency in groupedList_Frequency)
                {
                    foreach (var item in itemList_Frequency)
                    {
                        try
                        {
                            if (!timers.ContainsKey(item.ID))
                            {
                                Timer timer = new Timer();
                                timer.Interval = (int)TimeSpan.FromMinutes(item.Frequency).TotalMilliseconds;
                                timer.Elapsed += new System.Timers.ElapsedEventHandler(tickHandeler_Minute);
                                timer.Enabled = true;
                                timers.Add(item.ID, timer);
                                log.DebugDocLog("Minutes", "Timer Tic Event Handler Created Of Alert ID: " + item.ID + " , Interval :" + timer.Interval);
                            }
                        }
                        catch (Exception ex)
                        {
                            log.DebugDocLog("Minutes", "Exception Occure At Creating Timer Tic Event Handler Of Alert ID: " + item.ID +
                                Environment.NewLine + ex.Message);
                            continue;
                        }

                        break;
                    }

                }
            }
            catch (Exception ex)
            {
                log.DebugDocLog("Minutes", "Exception occured at CreateMinuteTimer : " + ex.Message);
            }


        }
        public void tickHandeler_Minute(object sender, EventArgs e)
        {
            Log log = new Log();
            try
            {

                Timer timer = (Timer)sender;

                var itemList = dtMinutes.Where(x => x.Frequency == (int)TimeSpan.FromMilliseconds(timer.Interval).TotalMinutes && x.IsActive == true).ToList();
                foreach (var item in itemList)
                {
                    log.DebugDocLog("Minutes", "tickHandeler_Minute Started Of Alert Setup ID:  " + item.ID);
                    AlertManagement alertMgt = new AlertManagement();
                    Task.Run(() => alertMgt.SendAlertAndNotification(item.ID));
                }


            }
            catch (Exception ex)
            {
                log.DebugDocLog("Minutes", "Exception Occure At tickHandeler_Minute , Exception:  " + ex.Message);
            }

        }

        #endregion

        #region For Hours

        public void CreateHoursTimer()
        {
            Log log = new Log();
            try
            {
                var groupedList_Frequency = dtHours.Where(x => x.IsActive == true).ToList()
                                  .GroupBy(u => u.Frequency)
                                  .Select(grp => grp.ToList())
                                  .ToList();

                log.DebugDocLog("Hours", "CreateHoursTimer Count: " + groupedList_Frequency.Count);

                foreach (var itemList_Frequency in groupedList_Frequency)
                {
                    foreach (var item in itemList_Frequency)
                    {
                        try
                        {
                            if (!timers.ContainsKey(item.ID))
                            {
                                log.DebugDocLog("Hours", "CreateHoursTimer of alert setup ID: " + item.ID);
                                Timer timer = new Timer();
                                timer.Interval = (int)TimeSpan.FromHours(item.Frequency).TotalMilliseconds;
                                timer.Elapsed += new System.Timers.ElapsedEventHandler(tickHandeler_Hours);
                                timer.Enabled = true;
                                timers.Add(item.ID, timer);
                                log.DebugDocLog("Hours", "Timer Tic Event Handler Created Of Alert ID: " + item.ID+" , Interval :"+ timer.Interval);
                            }
                        }
                        catch (Exception ex)
                        {
                            log.DebugDocLog("Hours", "Exception Occure At Creating Timer Tic Event Handler Of Alert ID: " + item.ID +
                                Environment.NewLine + ex.Message);
                            continue;
                        }

                        break;
                    }

                }
            }
            catch (Exception ex)
            {
                log.DebugDocLog("Hours", "Exception occured at CreateHoursTimer : " + ex.Message);
            }


        }
        public void tickHandeler_Hours(object sender, EventArgs e)
        {
            Log log = new Log();
            try
            {
                log.DebugDocLog("Hours", "tickHandeler_Hours Started, Count: "+ dtHours.Count);

                Timer timer = (Timer)sender;

                var itemList = dtHours.Where(x => x.Frequency == (int)TimeSpan.FromMilliseconds(timer.Interval).TotalHours && x.IsActive == true).ToList();
                foreach (var item in itemList)
                {
                    log.DebugDocLog("Hours", "tickHandeler_Hours Started Of Alert Setup ID:  " + item.ID);
                    AlertManagement alertMgt = new AlertManagement();
                    Task.Run(() => alertMgt.SendAlertAndNotification(item.ID));
                }


            }
            catch (Exception ex)
            {
                log.DebugDocLog("Hours", "Exception Occure At tickHandeler_Hours , Exception:  " + ex.Message);
            }

        }

        #endregion

        #region For Days

        public void CreateDaysTimer()
        {
            Log log = new Log();
            try
            {
                var groupedList_Frequency = dtDays.Where(x => x.IsActive == true).ToList()
                                  .GroupBy(u => u.Frequency)
                                  .Select(grp => grp.ToList())
                                  .ToList();

                log.DebugDocLog("Days", "CreateDaysTimer Count: " + groupedList_Frequency.Count);

                foreach (var itemList_Frequency in groupedList_Frequency)
                {
                    foreach (var item in itemList_Frequency)
                    {
                        try
                        {
                            if (!timers.ContainsKey(item.ID))
                            {
                                Timer timer = new Timer();
                                timer.Interval = (int)TimeSpan.FromDays(item.Frequency).TotalMilliseconds;
                                timer.Elapsed += new System.Timers.ElapsedEventHandler(tickHandeler_Days);
                                timer.Enabled = true;
                                timers.Add(item.ID, timer);
                                log.DebugDocLog("Days", "Timer Tic Event Handler Created Of Alert ID: " + item.ID + " , Interval :" + timer.Interval);
                            }
                        }
                        catch (Exception ex)
                        {
                            log.DebugDocLog("Days", "Exception Occure At Creating Timer Tic Event Handler Of Alert ID: " + item.ID +
                                Environment.NewLine + ex.Message);
                            continue;
                        }

                        break;
                    }

                }
            }
            catch (Exception ex)
            {
                log.DebugDocLog("Days", "Exception occured at CreateDaysTimer : " + ex.Message);
            }


        }
        public void tickHandeler_Days(object sender, EventArgs e)
        {
            Log log = new Log();
            try
            {
                log.DebugDocLog("Hours", "tickHandeler_Hours Started, Count: " + dtDays.Count);

                Timer timer = (Timer)sender;

                var itemList = dtDays.Where(x => x.Frequency ==(int)TimeSpan.FromMilliseconds(timer.Interval).TotalDays && x.IsActive == true).ToList();
                foreach (var item in itemList)
                {
                    log.DebugDocLog("Days", "tickHandeler_Days Started Of Alert Setup ID:  " + item.ID);
                    AlertManagement alertMgt = new AlertManagement();
                    Task.Run(() => alertMgt.SendAlertAndNotification(item.ID));
                }


            }
            catch (Exception ex)
            {
                log.DebugDocLog("Days", "Exception Occure At tickHandeler_Days , Exception:  " + ex.Message);
            }

        }

        #endregion

        #region For Weeks

        public void CreateWeeksTimer()
        {
            Log log = new Log();
            try
            {
                var groupedList_Frequency = dtWeeks.Where(x => x.IsActive == true).ToList()
                                  .GroupBy(u => u.Frequency)
                                  .Select(grp => grp.ToList())
                                  .ToList();

                log.DebugDocLog("Weeks", "CreateWeeksTimer Count: " + groupedList_Frequency.Count);

                foreach (var itemList_Frequency in groupedList_Frequency)
                {
                    foreach (var item in itemList_Frequency)
                    {
                        try
                        {
                            if (!timers.ContainsKey(item.ID))
                            {
                                Timer timer = new Timer();
                                timer.Interval = (double)TimeSpan.FromDays((item.Frequency*7)).TotalMilliseconds; //(int)TimeSpan.Fromw(item.Frequency).TotalMilliseconds;
                                timer.Elapsed += new System.Timers.ElapsedEventHandler(tickHandeler_Weeks);
                                timer.Enabled = true;
                                timers.Add(item.ID, timer);
                                log.DebugDocLog("Weeks", "Timer Tic Event Handler Created Of Alert ID: " + item.ID + " , Interval :" + timer.Interval);
                            }
                        }
                        catch (Exception ex)
                        {
                            log.DebugDocLog("Weeks", "Exception Occure At Creating Timer Tic Event Handler Of Alert ID: " + item.ID +
                                Environment.NewLine + ex.Message);
                            continue;
                        }

                        break;
                    }

                }
            }
            catch (Exception ex)
            {
                log.DebugDocLog("Weeks", "Exception occured at CreateWeeksTimer : " + ex.Message);
            }


        }
        public void tickHandeler_Weeks(object sender, EventArgs e)
        {
            Log log = new Log();
            try
            {
                log.DebugDocLog("Hours", "tickHandeler_Hours Started, Count: " + dtWeeks.Count);

                Timer timer = (Timer)sender;

                var itemList = dtWeeks.Where(x => x.Frequency ==(int)(TimeSpan.FromMilliseconds(timer.Interval).TotalDays/7) && x.IsActive == true).ToList();
                foreach (var item in itemList)
                {
                    log.DebugDocLog("Weeks", "tickHandeler_Weeks Started Of Alert Setup ID:  " + item.ID);
                    AlertManagement alertMgt = new AlertManagement();
                    Task.Run(() => alertMgt.SendAlertAndNotification(item.ID));
                }


            }
            catch (Exception ex)
            {
                log.DebugDocLog("Weeks", "Exception Occure At tickHandeler_Weeks , Exception:  " + ex.Message);
            }

        }

        #endregion

        #region For Months

        public void CreateMonthsTimer()
        {
            Log log = new Log();
            try
            {
                var groupedList_Frequency = dtMinutes.Where(x => x.IsActive == true).ToList()
                                  .GroupBy(u => u.Frequency)
                                  .Select(grp => grp.ToList())
                                  .ToList();

                log.DebugDocLog("Months", "CreateMonthsTimer Count: " + groupedList_Frequency.Count);

                foreach (var itemList_Frequency in groupedList_Frequency)
                {
                    foreach (var item in itemList_Frequency)
                    {
                        try
                        {
                            if (!timers.ContainsKey(item.ID))
                            {
                                Timer timer = new Timer();
                                timer.Interval = (double)TimeSpan.FromDays((item.Frequency * 30)).TotalMilliseconds; //(int)TimeSpan.Fromw(item.Frequency).TotalMilliseconds;
                                timer.Elapsed += new System.Timers.ElapsedEventHandler(tickHandeler_Months);
                                timer.Enabled = true;
                                timers.Add(item.ID, timer);
                                log.DebugDocLog("Months", "Timer Tic Event Handler Created Of Alert ID: " + item.ID + " , Interval :" + timer.Interval);
                            }
                        }
                        catch (Exception ex)
                        {
                            log.DebugDocLog("Months", "Exception Occure At Creating Timer Tic Event Handler Of Alert ID: " + item.ID +
                                Environment.NewLine + ex.Message);
                            continue;
                        }

                        break;
                    }

                }
            }
            catch (Exception ex)
            {
                log.DebugDocLog("Months", "Exception occured at CreateMonthsTimer : " + ex.Message);
            }


        }
        public void tickHandeler_Months(object sender, EventArgs e)
        {
            Log log = new Log();
            try
            {
                log.DebugDocLog("Hours", "tickHandeler_Hours Started, Count: " + dtMonths.Count);

                Timer timer = (Timer)sender;

                var itemList = dtMonths.Where(x => x.Frequency ==(int)(TimeSpan.FromMilliseconds(timer.Interval).TotalDays / 30) && x.IsActive == true).ToList();
                foreach (var item in itemList)
                {
                    log.DebugDocLog("Months", "tickHandeler_Months Started Of Alert Setup ID:  " + item.ID);
                    AlertManagement alertMgt = new AlertManagement();
                    Task.Run(() => alertMgt.SendAlertAndNotification(item.ID));
                }


            }
            catch (Exception ex)
            {
                log.DebugDocLog("Months", "Exception Occure At tickHandeler_Months , Exception:  " + ex.Message);
            }

        }

        #endregion

        #endregion
    }
}
