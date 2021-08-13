using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;

namespace TMSDeloitte.Helper
{
    public class Log
    {

        public void ConnectionLogFile(string exception)
        {
            try
            {

                string folderPath = "~/Logs/ConnectionLog";
                if (!Directory.Exists(System.Web.HttpContext.Current.Server.MapPath(folderPath)))
                    Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath(folderPath));

                string path = folderPath + "/" + DateTime.Now.ToString("dd_MM_yyyy") + ".txt";
                if (!File.Exists(System.Web.HttpContext.Current.Server.MapPath(path)))
                    File.Create(System.Web.HttpContext.Current.Server.MapPath(path)).Close();

                using (StreamWriter log = File.AppendText(System.Web.HttpContext.Current.Server.MapPath(path)))
                {
                    // Write to the file:
                    log.WriteLine("Data Time:" + DateTime.Now);
                    log.WriteLine(exception);
                    // Close the stream:
                    log.Close();
                }

            }
            catch (Exception ex)
            { }


        }
        public void LogFile(string exception)
        {
            try
            {
                if (System.Web.HttpContext.Current != null)
                {
                    string folderPath = "~/Logs/ApplicationLog";
                    if (!Directory.Exists(System.Web.HttpContext.Current.Server.MapPath(folderPath)))
                        Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath(folderPath));

                    string path = folderPath + "/" + DateTime.Now.ToString("dd_MM_yyyy") + ".txt";
                    if (!File.Exists(System.Web.HttpContext.Current.Server.MapPath(path)))
                        File.Create(System.Web.HttpContext.Current.Server.MapPath(path)).Close();

                    using (StreamWriter log = File.AppendText(System.Web.HttpContext.Current.Server.MapPath(path)))
                    {
                        // Write to the file:
                        log.WriteLine("Data Time:" + DateTime.Now);
                        log.WriteLine(exception);
                        // Close the stream:
                        log.Close();
                    }
                }
            }
            catch (Exception ex)
            { }


        }

        public void InputOutputDocLog(string docType, string msg)
        {
            try
            {

                string folderPath = "~/Logs/AllDocumentLog/"+docType;

                if (System.Web.HttpContext.Current != null)
                {

                    if (!Directory.Exists(System.Web.HttpContext.Current.Server.MapPath(folderPath)))
                        Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath(folderPath));

                    string path = folderPath + "/" + DateTime.Now.ToString("dd_MM_yyyy") + ".txt";
                    if (!File.Exists(System.Web.HttpContext.Current.Server.MapPath(path)))
                        File.Create(System.Web.HttpContext.Current.Server.MapPath(path)).Close();

                    using (StreamWriter log = File.AppendText(System.Web.HttpContext.Current.Server.MapPath(path)))
                    {
                        // Write to the file:
                        log.WriteLine("Data Time:" + DateTime.Now);
                        log.WriteLine("Document Type: " + docType);
                        log.WriteLine("Message: " + msg);
                        log.WriteLine();
                        log.WriteLine("====================================================================================");
                        // Close the stream:
                        log.Close();

                    }
                }

            }
            catch (Exception)
            {

            }

        }

        public void InputOutputEmailDocLog(string docType, string msg)
        {
            try
            {

                string folderPath = "~/Logs/EmailDocumentLog/" + docType;
                if (!Directory.Exists(System.Web.HttpContext.Current.Server.MapPath(folderPath)))
                    Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath(folderPath));

                string path = folderPath + "/" + DateTime.Now.ToString("dd_MM_yyyy") + ".txt";
                if (!File.Exists(System.Web.HttpContext.Current.Server.MapPath(path)))
                    File.Create(System.Web.HttpContext.Current.Server.MapPath(path)).Close();

                using (StreamWriter log = File.AppendText(System.Web.HttpContext.Current.Server.MapPath(path)))
                {
                    // Write to the file:
                    log.WriteLine("Data Time:" + DateTime.Now);
                    log.WriteLine("Document Type: " + docType);
                    log.WriteLine("Message: " + msg);
                    log.WriteLine();
                    log.WriteLine("====================================================================================");
                    // Close the stream:
                    log.Close();

                }

            }
            catch (Exception)
            {

            }

        }

        public void HCMOneLog(string msg)
        {
            try
            {
                string HCMOneLog = ConfigurationManager.AppSettings["HCMOneLog"];
                if (HCMOneLog != "1")
                    return;

                string folderPath = "~/Logs/HCMOneLog";
                if (!Directory.Exists(System.Web.HttpContext.Current.Server.MapPath(folderPath)))
                    Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath(folderPath));

                string path = folderPath + "/" + DateTime.Now.ToString("dd_MM_yyyy") + ".txt";
                if (!File.Exists(System.Web.HttpContext.Current.Server.MapPath(path)))
                    File.Create(System.Web.HttpContext.Current.Server.MapPath(path)).Close();

                using (StreamWriter log = File.AppendText(System.Web.HttpContext.Current.Server.MapPath(path)))
                {
                    // Write to the file:
                    log.WriteLine("Data Time:" + DateTime.Now);
                    log.WriteLine(msg);
                    // Close the stream:
                    log.Close();
                }

            }
            catch (Exception ex)
            { }


        }
    }
}