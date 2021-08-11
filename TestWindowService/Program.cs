using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using TMSDeloitte.Helper;

namespace AlertWindowService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            try
            {
                Library.WriteErrorLog("Alert window service starting from Main()");

                try
                {
                    Encrypt_Decrypt EncryptDecrypt = new Encrypt_Decrypt();
                    string SERVERNODE = ConfigurationManager.AppSettings["SERVERNODE"];
                    string UID = ConfigurationManager.AppSettings["UID"];
                    string PWD = ConfigurationManager.AppSettings["PWD"];
                    string CS = ConfigurationManager.AppSettings["CS"];
                   
                    string HANAConString = ConfigurationManager.AppSettings["HANAConString"];
                    HANAConString = HANAConString.Replace("[SERVERNODE]", EncryptDecrypt.DecryptString(SERVERNODE));
                    HANAConString = HANAConString.Replace("[UID]", EncryptDecrypt.DecryptString(UID));
                    HANAConString = HANAConString.Replace("[PWD]", EncryptDecrypt.DecryptString(PWD));
                    HANAConString = HANAConString.Replace("[CS]", EncryptDecrypt.DecryptString(CS));
                    Log log = new Log();
                    log.ConnectionLogFile(HANAConString);
                }
                catch(Exception)
                {

                }

                //For Window Service
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                new Scheduler()
                };
                ServiceBase.Run(ServicesToRun);

            }
            catch (Exception)
            {

            }


            ////For Test Service On Run Time
            //Scheduler myServ = new Scheduler();
            //myServ.TMSAlertService();

        }
    }
}
