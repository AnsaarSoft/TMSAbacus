using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using TMSDeloitte.DAL;
using TMSDeloitte.Helper;
using TMSDeloitte.Models;

namespace TMSDeloitte.BAL
{
    public class TimeSheetViewManagement
    {
        public DataTable GetTimeSheetView(bool isSuper,int isShowAll,int empId,string fromDate,string toDate)
        {
            DataTable dt = new DataTable();
            Common cmn = new Common();
            try
            {

                string spName = "";
                if (empId == 0)
                    spName = "GetTimeSheetView";
                else
                    spName = "GetTimeSheetViewByEmpID";

                List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "IsSuper";
                parm.ParameterValue = Convert.ToString(isSuper);
                parm.ParameterType = DBTypes.Bool.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ShowAll";
                parm.ParameterValue = Convert.ToString(isShowAll);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "EmpID";
                parm.ParameterValue = Convert.ToString(empId);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

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

                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                dt = HANADAL.GetDataTableByStoredProcedure(spName, parmList, "TimeSheetFormManagement");
                Encrypt_Decrypt security = new Encrypt_Decrypt();

                DataColumn newCol = new DataColumn("EmpID", typeof(string));
                newCol.AllowDBNull = true;
                dt.Columns.Add(newCol);

                newCol = new DataColumn("_DocNum", typeof(string));
                newCol.AllowDBNull = true;
                dt.Columns.Add(newCol);

                newCol = new DataColumn("EmpFullName", typeof(string));
                newCol.AllowDBNull = true;
                dt.Columns.Add(newCol);

                foreach (DataRow dtRow in dt.Rows)
                {
                    dtRow["EmpID"] = security.EncryptURLString(Convert.ToString(dtRow["EMPLOYEEID"]));
                    dtRow["_DocNum"] = security.EncryptURLString(Convert.ToString(dtRow["DocNum"]));
                    UserManagement userMgt = new UserManagement();
                    UserProfile user = userMgt.GetUserByID(Convert.ToInt32(dtRow["EMPLOYEEID"])) ;
                    if(user!=null)
                        dtRow["EmpFullName"] = user.FULLNAME;
                }

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("TimeSheetFormManagement", "Exception occured on GetTimeSheetAllDocumentsList, " + ex.Message);
            }

            return dt;
        }
    }
}