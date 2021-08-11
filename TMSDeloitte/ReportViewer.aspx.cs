using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using CrystalDecisions.ReportSource;
using CrystalDecisions.CrystalReports.Engine;

using System.Web.Configuration;
////using CrystalDecisions.CrystalReports.TemplateEngine;
using CrystalDecisions.ReportAppServer.ClientDoc;
using CrystalDecisions.ReportAppServer.Controllers;
using CrystalDecisions.ReportAppServer.DataDefModel;

////using CrystalDecisions.ReportAppServer.CommonObjectModel;
////using CrystalDecisions.ReportAppServer.ObjectFactory;
////using CrystalDecisions.ReportAppServer.ReportDefModel;
using CrystalDecisions.Shared;
using System.Net.Mail;
using System.Net;
using TMSDeloitte.Helper;

namespace TMSDeloitte
{
    public partial class ReportViewer : System.Web.UI.Page
    {
        
        protected void Page_Load(object sender, EventArgs e)
        {
            //string doc = Request.QueryString["DocId"];
            string report = Request.QueryString["ReportName"];

            ConfigureCrystalReports2(report);
            
        }
        
        #region CRViewerNormal
        private void ConfigureCrystalReports2(string ReportName)
        {
            CrystalDecisions.CrystalReports.Engine.ReportDocument rd = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            
            string path = "";
            path = Server.MapPath(@"/Rpt/") + ReportName;
            rd.Load(path);

            CrystalDecisions.Shared.ConnectionInfo connectionInfo = new CrystalDecisions.Shared.ConnectionInfo();

            Encrypt_Decrypt EncryptDecrypt = new Encrypt_Decrypt();
            string SERVERNODE = ConfigurationManager.AppSettings["SERVERNODE"];
            string UID = ConfigurationManager.AppSettings["UID"];
            string PWD = ConfigurationManager.AppSettings["PWD"];
            string CS = ConfigurationManager.AppSettings["CS"];

            SERVERNODE =  EncryptDecrypt.DecryptString(SERVERNODE);
            UID =  EncryptDecrypt.DecryptString(UID);
            PWD = EncryptDecrypt.DecryptString(PWD);
            CS =  EncryptDecrypt.DecryptString(CS);

            rd.SetDatabaseLogon(SERVERNODE, UID, PWD, CS);

            connectionInfo.ServerName = SERVERNODE;
            connectionInfo.DatabaseName = CS;
            connectionInfo.UserID = UID;
            connectionInfo.Password = PWD;

            CRV.ReportSource = rd;
            SetDBLogonForReport(connectionInfo);
        }
        private void SetDBLogonForReport(CrystalDecisions.Shared.ConnectionInfo connectionInfo)
        {
            TableLogOnInfos tableLogOnInfos = CRV.LogOnInfo;
            foreach (TableLogOnInfo tableLogOnInfo in tableLogOnInfos)
            {
                tableLogOnInfo.ConnectionInfo = connectionInfo;
            }
        }

        #endregion
    }
}