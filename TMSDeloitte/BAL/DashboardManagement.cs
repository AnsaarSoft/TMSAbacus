using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using TMSDeloitte.DAL;
using TMSDeloitte.Helper;
using TMSDeloitte.Models;

namespace TMSDeloitte.BAL
{
    public class DashboardManagement
    {
        public DataTable GetDashboardTotalHours(int empID,string docType)
        {
            DataTable dt = new DataTable();
            Common cmn = new Common();
            try
            {
                string StartDate = "";
                string EndDate = "";
                cmn.GetFiscYear(out StartDate, out EndDate);


                List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
                B1SP_Parameter
                parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = Convert.ToString(empID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "Year";
                parm.ParameterValue = Convert.ToString(DateTime.Now.Year-1);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "FromDate";
                parm.ParameterValue = StartDate;
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ToDate";
                parm.ParameterValue = EndDate;
                parmList.Add(parm);

                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                DataSet ds = HANADAL.GetDataSetByStoredProcedure("GetDashboardTotalHours", parmList, docType);
                if(ds.Tables.Count>0)
                {
                    //dt = ds.Tables[0];
                   
                    dt.Columns.Add("Period", typeof(string));
                    dt.Columns.Add("Year", typeof(string));
                    dt.Columns.Add("NonChargeableHours", typeof(double));
                    dt.Columns.Add("ChargeableHours", typeof(double));

                    foreach (DataRow dtRow1 in ds.Tables[1].Rows)
                    {
                        bool isExist = false;
                        foreach (DataRow dtRow0 in ds.Tables[0].Rows)
                        {
                            if (Convert.ToString(dtRow0["Period"]) == Convert.ToString(dtRow1["Period"]))
                            {
                                string val = Convert.ToString(dtRow0["Period"]);
                                DataRow row = dt.NewRow();
                                row["Period"] = val.Split('/')[0];
                                row["Year"] = val.Split('/')[1];
                                row["NonChargeableHours"] = Math.Round(Convert.ToDouble(dtRow0["NonChargeableHours"]),2); ;
                                row["ChargeableHours"] =Math.Round(Convert.ToDouble(dtRow0["ChargeableHours"]),2);
                                dt.Rows.Add(row);
                                isExist = true;
                                break;
                            } 
                        }
                        if(!isExist)
                        {
                            string val = Convert.ToString(dtRow1["Period"]);
                            DataRow row = dt.NewRow();
                            row["Period"] = val.Split('/')[0];
                            row["Year"] = val.Split('/')[1];
                            row["NonChargeableHours"] = 0;
                            row["ChargeableHours"] = 0;
                            dt.Rows.Add(row);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog(docType, "Exception occured on GetDashboardTotalHours, " + ex.Message);
            }

            return dt;
        }

        public DataTable GetTimeSheetStatus(int empID, string docType)
        {
            DataTable dt = new DataTable();
            Common cmn = new Common();
            try
            {
                string StartDate = "";
                string EndDate = "";
                cmn.GetFiscYear(out StartDate, out EndDate);

                List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = Convert.ToString(empID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "Year";
                parm.ParameterValue = Convert.ToString(DateTime.Now.Year );
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "FromDate";
                parm.ParameterValue = StartDate;
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ToDate";
                parm.ParameterValue = EndDate;
                parmList.Add(parm);


                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                dt = HANADAL.GetDataTableByStoredProcedure("GetTimeSheetStatus", parmList, docType);

                List<TimseSheetStatus> statusList = cmn.GetTimeSheetFormStatusList();
                List<int> statusIds = new List<int>();
                foreach (DataRow dtRow in dt.Rows)
                {
                    var status = statusList.Where(x => x.ID == Convert.ToInt32(dtRow["statusID"])).FirstOrDefault();
                    if (status != null)
                    {
                        dtRow["status"] = status.Name;
                        statusIds.Add(status.ID);
                    }

                }
                foreach (var item in statusList)
                {
                    if(!statusIds.Contains(item.ID))
                    {
                        DataRow row = dt.NewRow();
                        row["status"] = item.Name;
                        row["statusID"] = item.ID;
                        row["count"] = 0;
                        dt.Rows.Add(row);
                    }
                  
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog(docType, "Exception occured on GetTimeSheetStatus, " + ex.Message);
            }

            return dt;
        }

        public DataTable GetDashboardTotalHours(int empID, string docType,string StartDate,string EndDate)
        {
            DataTable dt = new DataTable();
            Common cmn = new Common();
            try
            {
                
                List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
                B1SP_Parameter
                parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = Convert.ToString(empID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "Year";
                parm.ParameterValue = Convert.ToString(DateTime.Now.Year - 1);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "FromDate";
                parm.ParameterValue = StartDate;
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ToDate";
                parm.ParameterValue = EndDate;
                parmList.Add(parm);

                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                dt = HANADAL.GetDataTableByStoredProcedure("GetDashboardTotalHours_ProfessionalStaff", parmList, docType);
                
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog(docType, "Exception occured on GetDashboardTotalHours_ProfessionalStaff, " + ex.Message);
            }

            return dt;
        }

        public DataTable GetDashboardTotalHours_ProfessionalStaff(int empID, string docType)
        {
            DataTable dt = new DataTable();
            try
            {
                for (int i = 0; i < 4; i++)
                {
                    string Type = "";
                    string startDate = "";
                    string endDate = "";
                    if(i==0)
                    {
                        DateTime now = DateTime.Now;
                        startDate = new DateTime(now.Year, now.Month, 1).ToString("yyyy-MM-dd");
                        endDate = new DateTime(now.Year, now.Month, 1).AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd");
                        Type = "Current Month";
                    }
                    if (i == 1)
                    {
                        DateTime now = DateTime.Now.AddMonths(-1); 
                        startDate = new DateTime(now.Year, now.Month, 1).ToString("yyyy-MM-dd");
                        endDate = new DateTime(now.Year, now.Month, 1).AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd");
                        Type = "Previous Month";
                    }
                    if (i == 2)
                    {
                        DateTime date = DateTime.Now.AddMonths(-3);
                        DateTime now = DateTime.Now;
                        startDate = new DateTime(date.Year, date.Month, 1).ToString("yyyy-MM-dd");
                        endDate = new DateTime(now.Year, now.Month, 1).AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd");
                        Type = "Quarter";
                    }
                    if (i == 3)
                    {
                        Common cmn = new Common();
                        cmn.GetFiscYear(out startDate, out startDate);
                        Type = "TYD";
                    }


                    DataTable dTable = GetDashboardTotalHours(empID, docType, startDate, endDate);
                    if (dTable.Rows.Count==0)
                        dTable.Rows.Add(new Object[] { 0, 0 });

                    System.Data.DataColumn newColumn = new System.Data.DataColumn("Type", typeof(System.String));
                    newColumn.DefaultValue = Type;
                    dTable.Columns.Add(newColumn);
                    if (i == 0)
                        dt = dTable;
                    else
                        dt.Merge(dTable);
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog(docType, "Exception occured on GetTimeSheetStatus, " + ex.Message);
            }

            return dt;
        }


        public List<StaffPendingTimeStatus> GetDashboardStaffPendingTimeStatus(string docType)
        {
            List<StaffPendingTimeStatus> list = new List<StaffPendingTimeStatus>();
            DataTable dt = new DataTable();
            dt.Columns.Add("Function", typeof(string));
            dt.Columns.Add("TotalSubmitted", typeof(int));
            dt.Columns.Add("TotalApproved", typeof(int));
          
            try
            {
                UserManagement mgt = new UserManagement();
                List<UserProfile> userList = mgt.GetAllUsers();
                foreach (var user in userList)
                {
                    List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
                    B1SP_Parameter
                    parm = new B1SP_Parameter();
                    parm.ParameterName = "EmpID";
                    parm.ParameterValue = Convert.ToString(user.ID);

                    HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                    DataSet ds = HANADAL.GetDataSetByStoredProcedure("GetDashboardStaffPendingTimeStatus", parmList, docType);
                    if(ds.Tables.Count>0)
                    {
                        BAL.Common setupManagement = new BAL.Common();
                        List<SAP_Function> sapFunctionList = setupManagement.GetFunctionsFromSAPB1();

                        if (!string.IsNullOrEmpty(user.DIMENSION2))
                        {
                            DataRow dtRow = dt.NewRow();
                            dtRow["Function"] = user.DIMENSION2 !=""? sapFunctionList.Where(x=>x.FunctionCode== user.DIMENSION2).Select(x=>x.FunctionName).FirstOrDefault() : ""; //-> Get Name From SAP SP "GETFunctionFromSAPB1"
                            dtRow["TotalSubmitted"] = Convert.ToInt32(ds.Tables[0].Rows[0]["Count"]);
                            dtRow["TotalApproved"] = Convert.ToInt32(ds.Tables[1].Rows[0]["Count"]);
                            dt.Rows.Add(dtRow);
                        }
                       
                    }
                }

                
                foreach (DataRow dtRow in dt.Rows)
                {
                    var ifExist = list.Where(x => x.Function == Convert.ToString(dtRow["Function"])).FirstOrDefault();
                    if(ifExist==null)
                    {
                        StaffPendingTimeStatus obj = new StaffPendingTimeStatus();
                        obj.Function = Convert.ToString(dtRow["Function"]);
                        obj.TotalSubmitted = Convert.ToInt32(dtRow["TotalSubmitted"]);
                        obj.TotalApproved = Convert.ToInt32(dtRow["TotalApproved"]);
                        list.Add(obj);
                    }
                    else
                    {
                        int TotalSubmitted = ifExist.TotalSubmitted + Convert.ToInt32(dtRow["TotalSubmitted"]);
                        int TotalApproved = ifExist.TotalSubmitted + Convert.ToInt32(dtRow["TotalApproved"]);

                        list.Where(x=>x.Function== Convert.ToString(dtRow["Function"])).ToList().Select(c => { c.TotalSubmitted = TotalSubmitted; return c; }).ToList();
                        list.Where(x => x.Function == Convert.ToString(dtRow["Function"])).ToList().Select(c => { c.TotalApproved = TotalApproved; return c; }).ToList();
                    }
                    
                }

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog(docType, "Exception occured on GetStaffPendingTimeStatus, " + ex.Message);
            }
            return list.OrderBy(x=>x.Function).ToList();
        }

    }
}