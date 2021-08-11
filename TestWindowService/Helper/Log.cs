using System;
using System.Collections.Generic;
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
            StreamWriter sw = null;
            try
            {

                string folderPath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\ConnectionLog";
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                string filePath = folderPath + "/" + DateTime.Now.ToString("dd_MM_yyyy") + ".txt";
                if (!File.Exists(filePath))
                    File.Create(filePath).Close();


                sw = new StreamWriter(filePath, true);
                sw.WriteLine(DateTime.Now.ToString() + ": " + exception);
                sw.Flush();
                sw.Close();
            }
            catch
            {
            }

        }
        public void LogFile(string exception)
        {
            StreamWriter sw = null;
            try
            {

                string folderPath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\UtilityLog";
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                string filePath = folderPath + "/" + DateTime.Now.ToString("dd_MM_yyyy") + ".txt";
                if (!File.Exists(filePath))
                    File.Create(filePath).Close();


                sw = new StreamWriter(filePath, true);
                sw.WriteLine(DateTime.Now.ToString() + ": " + exception);
                sw.Flush();
                sw.Close();
            }
            catch
            {
            }

        }

        public void InputOutputDocLog(string docType, string msg)
        {
            StreamWriter sw = null;
            try
            {
                string folderPath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\AllDocumentLog";
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                string filePath = folderPath + "/" + DateTime.Now.ToString("dd_MM_yyyy") + ".txt";
                if (!File.Exists(filePath))
                    File.Create(filePath).Close();

                sw = new StreamWriter(filePath, true);

                // Write to the file:
                sw.WriteLine("Data Time:" + DateTime.Now);
                sw.WriteLine("Document Type: " + docType);
                sw.WriteLine("Message: " + msg);
                sw.WriteLine();
                sw.WriteLine("===================================================================================="); sw.Flush();

                sw.Close();
            }
            catch
            {
            }

        }

        public void InputOutputEmailDocLog(string docType, string msg)
        {
            try
            {

                StreamWriter sw = null;
                try
                {
                    string folderPath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\EmailDocumentLog";
                    if (!Directory.Exists(folderPath))
                        Directory.CreateDirectory(folderPath);

                    string filePath = folderPath + "/" + DateTime.Now.ToString("dd_MM_yyyy") + ".txt";
                    if (!File.Exists(filePath))
                        File.Create(filePath).Close();

                    sw = new StreamWriter(filePath, true);

                    // Write to the file:
                    sw.WriteLine("Data Time:" + DateTime.Now);
                    sw.WriteLine("Document Type: " + docType);
                    sw.WriteLine("Message: " + msg);
                    sw.WriteLine();
                    sw.WriteLine("===================================================================================="); sw.Flush();

                    sw.Close();
                }
                catch
                {
                }

            }
            catch (Exception)
            {

            }

        }

        public void DebugDocLog(string docType, string msg)
        {
            try
            {
                if (docType == null)
                    return;

                StreamWriter sw = null;
                try
                {

                    string folderPath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\DebugDocumentLog\\" + docType ;
                    if (!Directory.Exists(folderPath))
                        Directory.CreateDirectory(folderPath);

                    string filePath = folderPath + "/" + DateTime.Now.ToString("dd_MM_yyyy") + ".txt";
                    if (!File.Exists(filePath))
                        File.Create(filePath).Close();

                    sw = new StreamWriter(filePath, true);

                    // Write to the file:
                    sw.WriteLine("Data Time:" + DateTime.Now);
                    sw.WriteLine("Document Type: " + docType);
                    sw.WriteLine("Message: " + msg);
                    sw.WriteLine();
                    sw.WriteLine("===================================================================================="); sw.Flush();

                    sw.Close();
                }
                catch
                {
                }

            }
            catch (Exception)
            {

            }

        }

    }
}