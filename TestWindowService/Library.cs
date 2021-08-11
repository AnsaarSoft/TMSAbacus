using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using TMSDeloitte.Helper;
using AlertWindowService.Helper;

namespace AlertWindowService
{
    public static class Library
    {
       
        public static void WriteErrorLog(string Message)
        {
            StreamWriter sw = null;
            try
            {
                string folderPath = AppDomain.CurrentDomain.BaseDirectory + "\\WindowSeriviceLog";
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                string filePath = folderPath + "/" + DateTime.Now.ToString("dd_MM_yyyy") + ".txt";
                if (!File.Exists(filePath))
                    File.Create(filePath).Close();

                sw = new StreamWriter(filePath, true);
                sw.WriteLine(DateTime.Now.ToString() + ": " + Message);
                sw.Flush();
                sw.Close();

                Log log = new Log();
                log.LogFile(Message);


                //string testFolderPath = @"D:\Jawwad\Projects\TMS Alert Window Service\TestWindowService\TestWindowService\TestWindowService\AlertAndNotification"; ;

                //folderPath = testFolderPath + "\\WindowSeriviceLog";
                //if (!Directory.Exists(folderPath))
                //    Directory.CreateDirectory(folderPath);

                //filePath = folderPath + "/" + DateTime.Now.ToString("dd_MM_yyyy") + ".txt";
                //if (!File.Exists(filePath))
                //    File.Create(filePath).Close();

                //sw = new StreamWriter( filePath, true);
                //sw.WriteLine(DateTime.Now.ToString() + ": " + Message);
                //sw.Flush();
                //sw.Close();

            }
            catch(Exception ex)
            {
            }
        }
    }
}
