using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using TMSDeloitte.DAL;
using TMSDeloitte.Helper;
using TMSDeloitte.Models;

namespace TMSDeloitte.BAL
{
    public class AssignmentFormManagement
    {
        public string GetDocID(string DOCNUM)
        {
            int no = 1;
            string docNum = "";
            if (DOCNUM == "")
            {
                List<string> docNumList = GetAssignmentFormDocNum();// GetTaskMasterDocNum();
                if (docNumList.Count > 0)
                {
                    var item = docNumList[docNumList.Count - 1];
                    no = Convert.ToInt32(item.Split('-')[1]);
                    no = no + 1;
                    docNum = Convert.ToString(no).PadLeft(6, '0');
                    docNum = "ASS-" + docNum;
                }
                else
                {
                    docNum = Convert.ToString(no).PadLeft(6, '0');
                    docNum = "ASS-" + docNum;
                }
            }
            else
            {
                docNum = DOCNUM;
            }
            return docNum;
        }

        public List<string> GetAssignmentFormDocNum()
        {
            List<string> docNumList = new List<string>();
            try
            {
                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();

                DataTable dt = HANADAL.GetDataTableByStoredProcedure("GetAssignmentForm_LastDocNum", "AssignmentFormManagement");
                if (dt.Rows.Count > 0)
                {
                    docNumList = TranslateDataTableToAssignmentFormkDocNumList(dt);
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("AssignmentFormManagement", "Exception occured on GetAssignmentFormDocNum, " + ex.Message);
            }

            return docNumList;
        }


        public List<AssignmentForm> GetAssignmentFormInit(int ID)
        {
            List<AssignmentForm> assignmentForm = new List<AssignmentForm>();
            List<AssignmentFormGeneral> assignmentFormGeneral = new List<AssignmentFormGeneral>();
            List<AssignmentFormChild> assignmentFormChild = new List<AssignmentFormChild>();
            List<AssignmentFormCost> assignmentFormCost = new List<AssignmentFormCost>();
            List<AssignmentFormSummary> assignmentFormSummary = new List<AssignmentFormSummary>();

            try
            {
                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                DataTable dt_AssignmentForm = HANADAL.GetDataTableByStoredProcedure("GetAssignmentForm", TranslateIDToParameterList(ID), "AssignmentFormManagement");

                if (dt_AssignmentForm.Rows.Count > 0)
                {
                    assignmentForm = TranslateDataTableToAssignmentFormManagementList(dt_AssignmentForm);
                    //foreach (AssignmentForm item in assignmentForm)
                    //{
                    //    int AssignmentFormID = Convert.ToInt32(item.ID);
                    //    DataTable dt_AssignmentFormGeneral = HANADAL.GetDataTableByStoredProcedure("GetAssignmentFormGeneral", TranslateAssignmentFormIDToParameterList(AssignmentFormID), "AssignmentFormManagement");
                    //    if (dt_AssignmentFormGeneral.Rows.Count > 0)
                    //    {
                    //        assignmentFormGeneral = TranslateDataTableToAssignmentFormGeneralManagementList(dt_AssignmentFormGeneral);
                    //    }

                    //    DataTable dt_AssignmentFormResource = HANADAL.GetDataTableByStoredProcedure("GetAssignmentFormResource", TranslateAssignmentFormIDToParameterList(AssignmentFormID), "AssignmentFormManagement");

                    //    if (dt_AssignmentFormResource.Rows.Count > 0)
                    //    {
                    //        assignmentFormChild = TranslateDataTableToAssignmentFormChildManagementList(dt_AssignmentFormResource);
                    //    }

                    //    DataTable dt_AssignmentFormCost = HANADAL.GetDataTableByStoredProcedure("GetAssignmentFormCost", TranslateAssignmentFormIDToParameterList(AssignmentFormID), "AssignmentFormManagement");

                    //    if (dt_AssignmentFormCost.Rows.Count > 0)
                    //    {
                    //        assignmentFormCost = TranslateDataTableToAssignmentFormCostManagementList(dt_AssignmentFormCost);
                    //    }
                    //    else
                    //    {
                    //        assignmentFormCost = null;
                    //    }

                    //    DataTable dt_AssignmentFormSummary = HANADAL.GetDataTableByStoredProcedure("GetAssignmentFormSummary", TranslateAssignmentFormIDToParameterList(AssignmentFormID), "AssignmentFormManagement");

                    //    if (dt_AssignmentFormSummary.Rows.Count > 0)
                    //    {
                    //        assignmentFormSummary = TranslateDataTableToAssignmentFormSummaryManagementList(dt_AssignmentFormSummary);
                    //    }

                    //    item.General = assignmentFormGeneral;
                    //    item.Table = assignmentFormChild;
                    //    if (assignmentFormCost != null)
                    //        item.Table2 = assignmentFormCost;
                    //    item.Table3 = assignmentFormSummary;
                    //}
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("AssignmentFormManagement", "Exception occured on GetMaster_Task ID: " + ID + " , " + ex.Message);
            }

            return assignmentForm;
        }


        public List<AssignmentForm> GetAssignmentForm(int ID)
        {
            List<AssignmentForm> assignmentForm = new List<AssignmentForm>();
            List<AssignmentFormGeneral> assignmentFormGeneral = new List<AssignmentFormGeneral>();
            List<AssignmentFormChild> assignmentFormChild = new List<AssignmentFormChild>();
            List<AssignmentFormCost> assignmentFormCost = new List<AssignmentFormCost>();
            List<AssignmentFormSummary> assignmentFormSummary = new List<AssignmentFormSummary>();

            try
            {
                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                DataTable dt_AssignmentForm = HANADAL.GetDataTableByStoredProcedure("GetAssignmentForm", TranslateIDToParameterList(ID), "AssignmentFormManagement");

                if (dt_AssignmentForm.Rows.Count > 0)
                {
                    assignmentForm = TranslateDataTableToAssignmentFormManagementList(dt_AssignmentForm);
                    foreach (AssignmentForm item in assignmentForm)
                    {
                        int AssignmentFormID = Convert.ToInt32(item.ID);
                        DataTable dt_AssignmentFormGeneral = HANADAL.GetDataTableByStoredProcedure("GetAssignmentFormGeneral", TranslateAssignmentFormIDToParameterList(AssignmentFormID), "AssignmentFormManagement");
                        if (dt_AssignmentFormGeneral.Rows.Count > 0)
                        {
                            assignmentFormGeneral = TranslateDataTableToAssignmentFormGeneralManagementList(dt_AssignmentFormGeneral);
                        }

                        DataTable dt_AssignmentFormResource = HANADAL.GetDataTableByStoredProcedure("GetAssignmentFormResource", TranslateAssignmentFormIDToParameterList(AssignmentFormID), "AssignmentFormManagement");

                        if (dt_AssignmentFormResource.Rows.Count > 0)
                        {
                            assignmentFormChild = TranslateDataTableToAssignmentFormChildManagementList(dt_AssignmentFormResource);
                        }

                        DataTable dt_AssignmentFormCost = HANADAL.GetDataTableByStoredProcedure("GetAssignmentFormCost", TranslateAssignmentFormIDToParameterList(AssignmentFormID), "AssignmentFormManagement");

                        if (dt_AssignmentFormCost.Rows.Count > 0)
                        {
                            assignmentFormCost = TranslateDataTableToAssignmentFormCostManagementList(dt_AssignmentFormCost);
                        }
                        else
                        {
                            assignmentFormCost = null;
                        }

                        DataTable dt_AssignmentFormSummary = HANADAL.GetDataTableByStoredProcedure("GetAssignmentFormSummary", TranslateAssignmentFormIDToParameterList(AssignmentFormID), "AssignmentFormManagement");

                        if (dt_AssignmentFormSummary.Rows.Count > 0)
                        {
                            assignmentFormSummary = TranslateDataTableToAssignmentFormSummaryManagementList(dt_AssignmentFormSummary);
                        }

                        item.General = assignmentFormGeneral;
                        item.Table = assignmentFormChild;
                        if(assignmentFormCost != null)
                            item.Table2 = assignmentFormCost;
                        item.Table3 = assignmentFormSummary;
                    }
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("AssignmentFormManagement", "Exception occured on GetMaster_Task ID: " + ID + " , " + ex.Message);
            }

            return assignmentForm;
        }

        public List<TaskMasterSetupInfo> GetTaskMasterSetup(int id = 0)
        {
            List<TaskMasterSetupInfo> TaskMasterSetupInfo = new List<TaskMasterSetupInfo>();
            try
            {
                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC(); // Filter with Department/function ID
                DataTable dt_TaskMaster = HANADAL.GetDataTableByStoredProcedure("GetMaster_TaskByFunctionID", TranslateIDToParameterList(id), "TaskMasterManagement");
                if (dt_TaskMaster.Rows.Count > 0)
                {
                    TaskMasterSetupInfo = TranslateDataTableToTaskMasterManagementList(dt_TaskMaster);
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("TaskMasterManagement", "Exception occured on GetMaster_Task ID: " + id + " , " + ex.Message);
            }

            return TaskMasterSetupInfo;
        }

        public List<DropDown> GETBillingRateForUser(string FunctionID, int DesignationID, double FromDate, double ToDate)
        {
            List<DropDown> billingRate = new List<DropDown>();
            try
            {
                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();

                DataTable dt = HANADAL.GetDataTableByStoredProcedure("GetResourceBillingRatesforUserByIDS", TranslateIDSToParameterList(FunctionID, DesignationID, FromDate, ToDate), "AssignmentFormManagement");
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow item in dt.Rows)
                    {
                        DropDown billing = new DropDown();

                        //sapFunction.ID = Convert.ToString(item["PartnerCode"]);
                        billing.StdBillingRateHr = Convert.ToDouble(item["RatesPerHour"]);
                        billingRate.Add(billing);
                    }
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("AssignmentFormManagement", "Exception occured on GETBillingRateForUser, " + ex.Message);
            }

            return billingRate;
        }

        public List<AssignmentFormChild> GetTripRatePolicySetup(string ClientID, int BranchID)
        {
            List<AssignmentFormChild> TravelLocation = new List<AssignmentFormChild>();
            try
            {
                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                DataTable dt_TravelLocation = HANADAL.GetDataTableByStoredProcedure("GetTripRatesforUserByIDS", TranslateIDSToParameterList(ClientID, BranchID), "TravelLocationManagement");
                if (dt_TravelLocation.Rows.Count > 0)
                {
                    TravelLocation = TranslateDataTableToTripRatePolicyManagementList(dt_TravelLocation);
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("AssignmentFormManagement", "Exception occured on GetTravel_Location_Setup BranchID: " + BranchID + " , " + ex.Message);
            }

            return TravelLocation;
        }

        public bool GetAssignmentCreationCheck(out string msg, int UserID)
        {
            bool isSuccess = false;
            msg = "";
            try
            {
                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                DataTable dt_AssignmentCheck = HANADAL.GetDataTableByStoredProcedure("GetAssignmentCreationCheck", TranslateIDToParameterList(UserID), "TravelLocationManagement");
                if (dt_AssignmentCheck.Rows.Count > 0)
                {
                    isSuccess = true;
                    msg = "Authorize";
                }
                else
                {
                    isSuccess = false;
                    msg = " Please set the approval setup for this user , contact TMS Administrator";
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("AssignmentFormManagement", "Exception occured on GetAssignmentCreationCheck UserID: " + UserID + " , " + ex.Message);
            }

            return isSuccess;
        }

        public void AddAssignmentForm_Log(string DOCNUM, AssignmentForm AssignmentForm, int UserID, int ID, AssignmentForm previousObject, int docID)
        {
            try
            {
                AssignmentForm.DOCNUM = Convert.ToString(DOCNUM);

                List<B1SP_Parameter> paramList = TranslatAssignmentFormLogToParameterList(AssignmentForm, docID);
                bool isChangeOccured = false;
                if (previousObject != null)
                {
                    foreach (PropertyCompareResult resultItem in PropertyCompare.Compare(previousObject, AssignmentForm))
                    {
                        string Name = resultItem.Name;
                        object PreviousValue = resultItem.OldValue;
                        object NewValue = resultItem.NewValue;
                        if (Name == "IsDeleted")
                        {
                            if (NewValue == null)
                                NewValue = false;
                        }
                        switch (Name)
                        {

                            case "AssignmentTitle":
                                isChangeOccured = true;
                                paramList.Where(x => x.ParameterName == "AssignmentTitle_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                paramList.Where(x => x.ParameterName == "AssignmentTitle_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();
                                break;

                            case "BranchID":
                                isChangeOccured = true;
                                paramList.Where(x => x.ParameterName == "BranchID_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                paramList.Where(x => x.ParameterName == "BranchID_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();
                                break;

                            case "ClientID":
                                isChangeOccured = true;
                                paramList.Where(x => x.ParameterName == "ClientID_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                paramList.Where(x => x.ParameterName == "ClientID_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();
                                break;

                            case "FunctionID":
                                isChangeOccured = true;
                                paramList.Where(x => x.ParameterName == "FunctionID_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                paramList.Where(x => x.ParameterName == "FunctionID_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();
                                break;
                            case "SubFunctionID":
                                isChangeOccured = true;
                                paramList.Where(x => x.ParameterName == "SubFunctionID_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                paramList.Where(x => x.ParameterName == "SubFunctionID_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();
                                break;

                            case "PartnerID":
                                isChangeOccured = true;
                                paramList.Where(x => x.ParameterName == "PartnerID_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                paramList.Where(x => x.ParameterName == "PartnerID_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();
                                break;

                            case "DirectorID":
                                isChangeOccured = true;
                                paramList.Where(x => x.ParameterName == "DirectorID_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                paramList.Where(x => x.ParameterName == "DirectorID_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();
                                break;

                            case "Status":
                                isChangeOccured = true;
                                paramList.Where(x => x.ParameterName == "Status_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                paramList.Where(x => x.ParameterName == "Status_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();
                                break;

                            case "flgPost":
                                isChangeOccured = true;
                                paramList.Where(x => x.ParameterName == "flgPost_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                paramList.Where(x => x.ParameterName == "flgPost_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();
                                break;

                            case "IsDeleted":
                                isChangeOccured = true;
                                paramList.Where(x => x.ParameterName == "IsDeleted_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                paramList.Where(x => x.ParameterName == "IsDeleted_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();
                                break;
                        }

                    }

                    if (isChangeOccured)
                    {
                        HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                        //Task.Run(() => 
                        HANADAL.AddUpdateDataByStoredProcedure("AddUpdateAssignmentForm_Log", paramList, "AssignmentFormManagement");
                    }

                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("AssignmentFormManagement", "Exception occured on AddAssignmentForm_Log, " + ex.Message);
            }
        }

        private List<B1SP_Parameter> TranslatAssignmentFormLogToParameterList(AssignmentForm obj, int docID)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {
                if (obj == null)
                    return null;

                B1SP_Parameter parm = new B1SP_Parameter();

                parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = Convert.ToString(0);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "DocNum";
                parm.ParameterValue = Convert.ToString(obj.DOCNUM);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "NonChargeable";

                if (obj.NonChargeable == null)
                    parm.ParameterValue = Convert.ToString(false);
                else
                    parm.ParameterValue = Convert.ToString(obj.NonChargeable);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "DocID";
                parm.ParameterValue = Convert.ToString(docID);
                parmList.Add(parm);


                parm = new B1SP_Parameter();
                parm.ParameterName = "AssignmentTitle_Previous";
                parm.ParameterValue = Convert.ToString(obj.AssignmentTitle);
                parmList.Add(parm);


                parm = new B1SP_Parameter();
                parm.ParameterName = "BranchID_Previous";
                parm.ParameterValue = Convert.ToString(obj.BranchID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ClientID_Previous";
                parm.ParameterValue = Convert.ToString(obj.ClientID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "FunctionID_Previous";
                parm.ParameterValue = Convert.ToString(obj.FunctionID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "SubFunctionID_Previous";
                parm.ParameterValue = Convert.ToString(obj.SubFunctionID);
                parmList.Add(parm);


                parm = new B1SP_Parameter();
                parm.ParameterName = "PartnerID_Previous";
                parm.ParameterValue = Convert.ToString(obj.PartnerID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "DirectorID_Previous";
                parm.ParameterValue = Convert.ToString(obj.DirectorID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "Status_Previous";
                parm.ParameterValue = Convert.ToString(obj.Status);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "flgPost_Previous";
                parm.ParameterValue = Convert.ToString(obj.flgPost);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "IsDeleted_Previous";
                if (obj.IsDeleted != null)
                    parm.ParameterValue = Convert.ToString(obj.IsDeleted);
                else
                    parm.ParameterValue = Convert.ToString(false);
                parmList.Add(parm);


                parm = new B1SP_Parameter();
                parm.ParameterName = "AssignmentTitle_New";
                parm.ParameterValue = Convert.ToString(obj.AssignmentTitle);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "BranchID_New";
                parm.ParameterValue = Convert.ToString(obj.BranchID);
                parmList.Add(parm);


                parm = new B1SP_Parameter();
                parm.ParameterName = "ClientID_New";
                parm.ParameterValue = Convert.ToString(obj.ClientID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "FunctionID_New";
                parm.ParameterValue = Convert.ToString(obj.FunctionID);
                parmList.Add(parm);


                parm = new B1SP_Parameter();
                parm.ParameterName = "SubFunctionID_New";
                parm.ParameterValue = Convert.ToString(obj.SubFunctionID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "PartnerID_New";
                parm.ParameterValue = Convert.ToString(obj.PartnerID);
                parmList.Add(parm);


                parm = new B1SP_Parameter();
                parm.ParameterName = "DirectorID_New";
                parm.ParameterValue = Convert.ToString(obj.DirectorID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "Status_New";
                parm.ParameterValue = Convert.ToString(obj.Status);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "flgPost_New";
                parm.ParameterValue = Convert.ToString(obj.flgPost);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "IsDeleted_New";
                if (obj.IsDeleted != null)
                    parm.ParameterValue = Convert.ToString(obj.IsDeleted);
                else
                    parm.ParameterValue = Convert.ToString(false);
                parmList.Add(parm);


                parm = new B1SP_Parameter();
                parm.ParameterName = "CreatedBy";
                parm.ParameterValue = Convert.ToString(obj.CreatedBy);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CreateDate";
                parm.ParameterValue = DateTime.Now.ToString("yyyy-MM-dd HH:MM:s");
                parmList.Add(parm);
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("AssignmentFormManagement", "Exception occured on TranslatAssignmentFormLogToParameterList, " + ex.Message);
            }


            return parmList;
        }

        private List<B1SP_Parameter> TranslateAssignmentFormGeneralLogToParameterList(AssignmentFormGeneral obj, int docID)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {
                if (obj == null)
                    return null;

                B1SP_Parameter parm = new B1SP_Parameter();

                parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = Convert.ToString(0);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "AssingmentFormID";
                parm.ParameterValue = Convert.ToString(docID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "TypeOfAssignment_Previous";
                parm.ParameterValue = Convert.ToString(obj.TypeOfAssignment);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "AssignmentNatureID_Previous";
                parm.ParameterValue = Convert.ToString(obj.AssignmentNatureID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "TypeOfBilling_Previous";
                parm.ParameterValue = Convert.ToString(obj.TypeOfBilling);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CurrencyID_Previous";
                parm.ParameterValue = Convert.ToString(obj.CurrencyID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "AssignmentValue_Previous";
                parm.ParameterValue = Convert.ToString(obj.AssignmentValue);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "StartDate_Previous";
                parm.ParameterValue = Convert.ToDateTime(obj.StartDate).ToString("yyyy-MM-dd HH:MM:s");
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "EndDate_Previous";
                parm.ParameterValue = Convert.ToDateTime(obj.EndDate).ToString("yyyy-MM-dd HH:MM:s");
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "DurationInDays_Previous";
                parm.ParameterValue = Convert.ToString(obj.DurationInDays);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ClosureDate_Previous";
                parm.ParameterValue = Convert.ToDateTime(obj.ClosureDate).ToString("yyyy-MM-dd HH:MM:s");
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "Status_Previous";
                parm.ParameterValue = Convert.ToString(obj.Status);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "TypeOfAssignment_New";
                parm.ParameterValue = Convert.ToString(obj.TypeOfAssignment);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "AssignmentNatureID_New";
                parm.ParameterValue = Convert.ToString(obj.AssignmentNatureID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "TypeOfBilling_New";
                parm.ParameterValue = Convert.ToString(obj.TypeOfBilling);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CurrencyID_New";
                parm.ParameterValue = Convert.ToString(obj.CurrencyID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "AssignmentValue_New";
                parm.ParameterValue = Convert.ToString(obj.AssignmentValue);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "StartDate_New";
                parm.ParameterValue = Convert.ToDateTime(obj.StartDate).ToString("yyyy-MM-dd HH:MM:s"); ;
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "EndDate_New";
                parm.ParameterValue = Convert.ToDateTime(obj.EndDate).ToString("yyyy-MM-dd HH:MM:s"); ;
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "DurationInDays_New";
                parm.ParameterValue = Convert.ToString(obj.DurationInDays);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ClosureDate_New";
                parm.ParameterValue = Convert.ToDateTime(obj.ClosureDate).ToString("yyyy-MM-dd HH:MM:s"); ;
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "Status_New";
                parm.ParameterValue = Convert.ToString(obj.Status);
                parmList.Add(parm);

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("AssignmentFormManagement", "Exception occured on TranslateAssignmentFormGeneralLogToParameterList, " + ex.Message);
            }


            return parmList;
        }

        private List<B1SP_Parameter> TranslateAssignmentFormResourceLogToParameterList(AssignmentFormChild obj, int docID)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {
                if (obj == null)
                    return null;

                B1SP_Parameter parm = new B1SP_Parameter();

                parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = Convert.ToString(0);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "AssingmentFormID";
                parm.ParameterValue = Convert.ToString(docID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "EmpID_Previous";
                parm.ParameterValue = Convert.ToString(obj.ID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "TaskID_Previous";
                parm.ParameterValue = Convert.ToString(obj.TaskID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "TotalHours_Previous";
                parm.ParameterValue = Convert.ToString(obj.TotalHours);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "StdBillingRateHr_Previous";
                parm.ParameterValue = Convert.ToString(obj.StdBillingRateHr);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ResourceCost_Previous";
                parm.ParameterValue = Convert.ToString(obj.ResourceCost);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "LocationID_Previous";
                parm.ParameterValue = Convert.ToString(obj.TravelRateID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "TravelCost_Previous";
                parm.ParameterValue = Convert.ToString(obj.TravelCost);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "TotalCost_Previous";
                parm.ParameterValue = Convert.ToString(obj.TotalCost);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "RevenueRateHr_Previous";
                parm.ParameterValue = Convert.ToString(obj.RevenueRateHr);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "Revenue_Previous";
                parm.ParameterValue = Convert.ToString(obj.Revenue);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "IsChargeable_Previous";
                parm.ParameterValue = Convert.ToString(obj.IsChargeable);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "InActive_Previous";
                parm.ParameterValue = Convert.ToString(obj.InActive);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "EmpID_New";
                parm.ParameterValue = Convert.ToString(obj.ID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "TaskID_New";
                parm.ParameterValue = Convert.ToString(obj.TaskID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "TotalHours_New";
                parm.ParameterValue = Convert.ToString(obj.TotalHours);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "StdBillingRateHr_New";
                parm.ParameterValue = Convert.ToString(obj.StdBillingRateHr);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ResourceCost_New";
                parm.ParameterValue = Convert.ToString(obj.ResourceCost);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "LocationID_New";
                parm.ParameterValue = Convert.ToString(obj.TravelRateID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "TravelCost_New";
                parm.ParameterValue = Convert.ToString(obj.TravelCost);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "TotalCost_New";
                parm.ParameterValue = Convert.ToString(obj.TotalCost);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "RevenueRateHr_New";
                parm.ParameterValue = Convert.ToString(obj.RevenueRateHr);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "Revenue_New";
                parm.ParameterValue = Convert.ToString(obj.Revenue);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "IsChargeable_New";
                parm.ParameterValue = Convert.ToString(obj.IsChargeable);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "InActive_New";
                parm.ParameterValue = Convert.ToString(obj.InActive);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CreatedBy";
                parm.ParameterValue = Convert.ToString(obj.CreatedBy);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CreateDate";
                parm.ParameterValue = Convert.ToDateTime(obj.CreateDate).ToString("yyyy-MM-dd HH:MM:s");
                parmList.Add(parm);


            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("AssignmentFormManagement", "Exception occured on TranslateAssignmentFormResourceLogToParameterList, " + ex.Message);
            }


            return parmList;
        }

        private List<B1SP_Parameter> TranslateAssignmentFormCostLogToParameterList(AssignmentFormCost obj, int docID)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {
                if (obj == null)
                    return null;

                B1SP_Parameter parm = new B1SP_Parameter();

                parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = Convert.ToString(0);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "AssingmentFormID";
                parm.ParameterValue = Convert.ToString(docID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "AssignmentCostSetupID_Previous";
                parm.ParameterValue = Convert.ToString(obj.ID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "Amount_Previous";
                parm.ParameterValue = Convert.ToString(obj.Amount);
                parmList.Add(parm);


                parm = new B1SP_Parameter();
                parm.ParameterName = "AssignmentCostSetupID_New";
                parm.ParameterValue = Convert.ToString(obj.ID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "Amount_New";
                parm.ParameterValue = Convert.ToString(obj.Amount);
                parmList.Add(parm);


                parm = new B1SP_Parameter();
                parm.ParameterName = "CreatedBy";
                parm.ParameterValue = Convert.ToString(obj.CreatedBy);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CreateDate";
                parm.ParameterValue = DateTime.Now.ToString("yyyy-MM-dd HH:MM:s");
                parmList.Add(parm);


            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("AssignmentFormManagement", "Exception occured on TranslateAssignmentFormCostLogToParameterList, " + ex.Message);
            }


            return parmList;
        }

        private List<B1SP_Parameter> TranslateAssignmentFormSummaryLogToParameterList(AssignmentFormSummary obj, int docID)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {
                if (obj == null)
                    return null;

                B1SP_Parameter parm = new B1SP_Parameter();

                parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = Convert.ToString(0);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "AssingmentFormID";
                parm.ParameterValue = Convert.ToString(docID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "TaskID_Previous";
                parm.ParameterValue = Convert.ToString(obj.TaskID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "TotalBudgetedHour_Previous";
                parm.ParameterValue = Convert.ToString(obj.TotalBudgetedHour);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "EstimatedResourceCost_Previous";
                parm.ParameterValue = Convert.ToString(obj.EstimatedResourceCost);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "RevenueDistribution_Previous";
                parm.ParameterValue = Convert.ToString(obj.RevenueDistribution);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "EstimatedRevenue_Previous";
                parm.ParameterValue = Convert.ToString(obj.EstimatedRevenue);
                parmList.Add(parm);


                parm = new B1SP_Parameter();
                parm.ParameterName = "TaskID_New";
                parm.ParameterValue = Convert.ToString(obj.TaskID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "TotalBudgetedHour_New";
                parm.ParameterValue = Convert.ToString(obj.TotalBudgetedHour);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "EstimatedResourceCost_New";
                parm.ParameterValue = Convert.ToString(obj.EstimatedResourceCost);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "RevenueDistribution_New";
                parm.ParameterValue = Convert.ToString(obj.RevenueDistribution);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "EstimatedRevenue_New";
                parm.ParameterValue = Convert.ToString(obj.EstimatedRevenue);
                parmList.Add(parm);
                parm = new B1SP_Parameter();
                parm.ParameterName = "CreatedBy";
                parm.ParameterValue = Convert.ToString(obj.CreatedBy);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CreateDate";
                parm.ParameterValue = DateTime.Now.ToString("yyyy-MM-dd HH:MM:s");
                parmList.Add(parm);


            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("AssignmentFormManagement", "Exception occured on TranslateAssignmentFormSummaryLogToParameterList, " + ex.Message);
            }


            return parmList;
        }

        public void AddAssignmentFormGeneral_Log(List<AssignmentFormGeneral> newObjectList, List<AssignmentFormGeneral> previousObjectList, int docID)
        {
            try
            {
                AssignmentFormGeneral newObject = new AssignmentFormGeneral();
                AssignmentFormGeneral previousObject = new AssignmentFormGeneral();

                foreach (var obj in newObjectList)
                {
                    previousObject = previousObjectList.Where(x => x.ID == obj.ID).FirstOrDefault();
                    if (previousObject != null)
                    {
                        newObject = obj;


                        List<B1SP_Parameter> paramList = TranslateAssignmentFormGeneralLogToParameterList(newObject, docID);
                        bool isChangeOccured = false;
                        if (previousObject != null)
                        {
                            foreach (PropertyCompareResult resultItem in PropertyCompare.Compare(previousObject, newObject))
                            {
                                isChangeOccured = true;
                                string Name = resultItem.Name;
                                object PreviousValue = resultItem.OldValue;
                                object NewValue = resultItem.NewValue;

                                switch (Name)
                                {
                                    case "TypeOfAssignment":
                                        isChangeOccured = true;
                                        paramList.Where(x => x.ParameterName == "TypeOfAssignment_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                        paramList.Where(x => x.ParameterName == "TypeOfAssignment_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();
                                        break;

                                    case "AssignmentNatureID":
                                        isChangeOccured = true;
                                        paramList.Where(x => x.ParameterName == "AssignmentNatureID_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                        paramList.Where(x => x.ParameterName == "AssignmentNatureID_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();
                                        break;

                                    case "TypeOfBilling":
                                        isChangeOccured = true;
                                        paramList.Where(x => x.ParameterName == "TypeOfBilling_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                        paramList.Where(x => x.ParameterName == "TypeOfBilling_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();
                                        break;

                                    case "CurrencyID":
                                        isChangeOccured = true;
                                        paramList.Where(x => x.ParameterName == "CurrencyID_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                        paramList.Where(x => x.ParameterName == "CurrencyID_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();
                                        break;

                                    case "AssignmentValue":
                                        isChangeOccured = true;
                                        paramList.Where(x => x.ParameterName == "AssignmentValue_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                        paramList.Where(x => x.ParameterName == "AssignmentValue_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();
                                        break;

                                    case "StartDate":
                                        isChangeOccured = true;
                                        paramList.Where(x => x.ParameterName == "StartDate_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                        paramList.Where(x => x.ParameterName == "StartDate_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();
                                        break;

                                    case "EndDate":
                                        isChangeOccured = true;
                                        paramList.Where(x => x.ParameterName == "EndDate_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                        paramList.Where(x => x.ParameterName == "EndDate_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();
                                        break;

                                    case "DurationInDays":
                                        isChangeOccured = true;
                                        paramList.Where(x => x.ParameterName == "DurationInDays_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                        paramList.Where(x => x.ParameterName == "DurationInDays_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();
                                        break;

                                    case "ClosureDate":
                                        isChangeOccured = true;
                                        paramList.Where(x => x.ParameterName == "ClosureDate_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                        paramList.Where(x => x.ParameterName == "ClosureDate_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();
                                        break;

                                    case "Status":
                                        isChangeOccured = true;
                                        paramList.Where(x => x.ParameterName == "Status_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                        paramList.Where(x => x.ParameterName == "Status_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();
                                        break;
                                }
                            }
                            if (isChangeOccured)
                            {
                                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                                //Task.Run(() => 
                                HANADAL.AddUpdateDataByStoredProcedure("AddUpdateAssignmentFormGeneral_Log", paramList, "AssignmentFormManagement");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("AssignmentFormManagement", "Exception occured on AddAssignmentFormGeneral_Log, " + ex.Message);
            }
        }

        public void AddAssignmentFormResource_Log(List<AssignmentFormChild> newObjectList, List<AssignmentFormChild> previousObjectList, int docID)
        {
            try
            {
                AssignmentFormChild newObject = new AssignmentFormChild();
                AssignmentFormChild previousObject = new AssignmentFormChild();

                foreach (var obj in newObjectList)
                {
                    previousObject = previousObjectList.Where(x => x.ID == obj.ID).FirstOrDefault();
                    if (previousObject != null)
                    {
                        newObject = obj;


                        List<B1SP_Parameter> paramList = TranslateAssignmentFormResourceLogToParameterList(newObject, docID);
                        bool isChangeOccured = false;
                        if (previousObject != null)
                        {
                            foreach (PropertyCompareResult resultItem in PropertyCompare.Compare(previousObject, newObject))
                            {
                                isChangeOccured = true;
                                string Name = resultItem.Name;
                                object PreviousValue = resultItem.OldValue;
                                object NewValue = resultItem.NewValue;

                                switch (Name)
                                {
                                    case "EmpID":
                                        isChangeOccured = true;
                                        paramList.Where(x => x.ParameterName == "EmpID_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                        paramList.Where(x => x.ParameterName == "EmpID_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();
                                        break;

                                    case "TaskID":
                                        isChangeOccured = true;
                                        paramList.Where(x => x.ParameterName == "TaskID_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                        paramList.Where(x => x.ParameterName == "TaskID_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();
                                        break;

                                    case "TotalHours":
                                        isChangeOccured = true;
                                        paramList.Where(x => x.ParameterName == "TotalHours_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                        paramList.Where(x => x.ParameterName == "TotalHours_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();
                                        break;

                                    case "StdBillingRateHr":
                                        isChangeOccured = true;
                                        paramList.Where(x => x.ParameterName == "StdBillingRateHr_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                        paramList.Where(x => x.ParameterName == "StdBillingRateHr_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();
                                        break;

                                    case "ResourceCost":
                                        isChangeOccured = true;
                                        paramList.Where(x => x.ParameterName == "ResourceCost_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                        paramList.Where(x => x.ParameterName == "ResourceCost_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();
                                        break;

                                    case "LocationID":
                                        isChangeOccured = true;
                                        paramList.Where(x => x.ParameterName == "LocationID_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                        paramList.Where(x => x.ParameterName == "LocationID_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();
                                        break;

                                    case "TravelCost":
                                        isChangeOccured = true;
                                        paramList.Where(x => x.ParameterName == "TravelCost_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                        paramList.Where(x => x.ParameterName == "TravelCost_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();
                                        break;

                                    case "TotalCost":
                                        isChangeOccured = true;
                                        paramList.Where(x => x.ParameterName == "TotalCost_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                        paramList.Where(x => x.ParameterName == "TotalCost_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();
                                        break;

                                    case "RevenueRateHr":
                                        isChangeOccured = true;
                                        paramList.Where(x => x.ParameterName == "RevenueRateHr_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                        paramList.Where(x => x.ParameterName == "RevenueRateHr_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();
                                        break;

                                    case "Revenue":
                                        isChangeOccured = true;
                                        paramList.Where(x => x.ParameterName == "Revenue_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                        paramList.Where(x => x.ParameterName == "Revenue_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();
                                        break;

                                    case "IsChargeable":
                                        isChangeOccured = true;
                                        paramList.Where(x => x.ParameterName == "IsChargeable_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                        paramList.Where(x => x.ParameterName == "IsChargeable_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();
                                        break;

                                    case "InActive":
                                        isChangeOccured = true;
                                        paramList.Where(x => x.ParameterName == "InActive_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                        paramList.Where(x => x.ParameterName == "InActive_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();
                                        break;
                                }
                            }
                            if (isChangeOccured)
                            {
                                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                                //Task.Run(() => 
                                HANADAL.AddUpdateDataByStoredProcedure("AddUpdateAssignmentFormResource_Log", paramList, "GroupSetupManagement");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("AssignmentFormManagement", "Exception occured on AddAssignmentFormResource_Log, " + ex.Message);
            }
        }

        public void AddAssignmentFormCost_Log(List<AssignmentFormCost> newObjectList, List<AssignmentFormCost> previousObjectList, int docID)
        {
            try
            {
                AssignmentFormCost newObject = new AssignmentFormCost();
                AssignmentFormCost previousObject = new AssignmentFormCost();

                foreach (var obj in newObjectList)
                {
                    previousObject = previousObjectList.Where(x => x.SNO == obj.SNO).FirstOrDefault();
                    if (previousObject != null)
                    {
                        newObject = obj;


                        List<B1SP_Parameter> paramList = TranslateAssignmentFormCostLogToParameterList(newObject, docID);
                        bool isChangeOccured = false;
                        if (previousObject != null)
                        {
                            foreach (PropertyCompareResult resultItem in PropertyCompare.Compare(previousObject, newObject))
                            {
                                isChangeOccured = true;
                                string Name = resultItem.Name;
                                object PreviousValue = resultItem.OldValue;
                                object NewValue = resultItem.NewValue;

                                switch (Name)
                                {
                                    case "AssignmentCostSetupID":
                                        isChangeOccured = true;
                                        paramList.Where(x => x.ParameterName == "AssignmentCostSetupID_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                        paramList.Where(x => x.ParameterName == "AssignmentCostSetupID_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();
                                        break;

                                    case "Amount":
                                        isChangeOccured = true;
                                        paramList.Where(x => x.ParameterName == "Amount_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                        paramList.Where(x => x.ParameterName == "Amount_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();
                                        break;
                                }
                            }
                            if (isChangeOccured)
                            {
                                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                                // Task.Run(() => 
                                HANADAL.AddUpdateDataByStoredProcedure("AddUpdateAssignmentFormCost_Log", paramList, "AssignmentFormManagement");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("AssignmentFormManagement", "Exception occured on AddAssignmentFormCost_Log, " + ex.Message);
            }
        }

        public void AddAssignmentFormSummary_Log(List<AssignmentFormSummary> newObjectList, List<AssignmentFormSummary> previousObjectList, int docID)
        {
            try
            {
                AssignmentFormSummary newObject = new AssignmentFormSummary();
                AssignmentFormSummary previousObject = new AssignmentFormSummary();

                foreach (var obj in newObjectList)
                {
                    previousObject = previousObjectList.Where(x => x.SNO == obj.SNO).FirstOrDefault();
                    if (previousObject != null)
                    {
                        newObject = obj;


                        List<B1SP_Parameter> paramList = TranslateAssignmentFormSummaryLogToParameterList(newObject, docID);
                        bool isChangeOccured = false;
                        if (previousObject != null)
                        {
                            foreach (PropertyCompareResult resultItem in PropertyCompare.Compare(previousObject, newObject))
                            {
                                isChangeOccured = true;
                                string Name = resultItem.Name;
                                object PreviousValue = resultItem.OldValue;
                                object NewValue = resultItem.NewValue;

                                switch (Name)
                                {
                                    case "TaskID":
                                        isChangeOccured = true;
                                        paramList.Where(x => x.ParameterName == "TaskID_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                        paramList.Where(x => x.ParameterName == "TaskID_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();
                                        break;

                                    case "TotalBudgetedHour":
                                        isChangeOccured = true;
                                        paramList.Where(x => x.ParameterName == "TotalBudgetedHour_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                        paramList.Where(x => x.ParameterName == "TotalBudgetedHour_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();
                                        break;

                                    case "EstimatedResourceCost":
                                        isChangeOccured = true;
                                        paramList.Where(x => x.ParameterName == "EstimatedResourceCost_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                        paramList.Where(x => x.ParameterName == "EstimatedResourceCost_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();
                                        break;

                                    case "RevenueDistribution":
                                        isChangeOccured = true;
                                        paramList.Where(x => x.ParameterName == "RevenueDistribution_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                        paramList.Where(x => x.ParameterName == "RevenueDistribution_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();
                                        break;

                                    case "EstimatedRevenue":
                                        isChangeOccured = true;
                                        paramList.Where(x => x.ParameterName == "EstimatedRevenue_Previous").Select(c => { c.ParameterValue = Convert.ToString(PreviousValue); return c; }).ToList();
                                        paramList.Where(x => x.ParameterName == "EstimatedRevenue_New").Select(c => { c.ParameterValue = Convert.ToString(NewValue); return c; }).ToList();
                                        break;

                                }
                            }
                            if (isChangeOccured)
                            {
                                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                                //Task.Run(() =>
                                HANADAL.AddUpdateDataByStoredProcedure("AddUpdateAssignmentFormSummary_Log", paramList, "AssignmentFormManagement");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("AssignmentFormManagement", "Exception occured on AddAssignmentFormSummary_Log, " + ex.Message);
            }
        }

        public bool AddUpdateNCAssignmentForm(out string msg, string DOCNUM, bool NonChargeable, AssignmentForm AssignmentForm, int ID, int UserID)
        {
            bool isSuccess = false;
            bool isAddOccured = false;
            bool isUpdateOccured = false;
            bool isDeleteOccured = false;
            Common cmn = new Common();

            msg = "";
            try
            {

                string docNum = GetDocID(DOCNUM);
                List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
                B1SP_Parameter parm = new B1SP_Parameter();
                string approvalCode = "";
                bool isValidateAssignmentForm = true;
                try
                {
                    HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();

                    if (ID > 0)
                    {
                        parmList = new List<B1SP_Parameter>();
                        parm = new B1SP_Parameter();
                        parm.ParameterName = "ID";
                        parm.ParameterValue = Convert.ToString(ID);
                        parmList.Add(parm);
                        DataSet ds = HANADAL.GetDataSetByStoredProcedure("GetAssignmentForm", parmList, "AssignmentFormManagement");
                        if (ds.Tables.Count > 0)
                        {
                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                foreach (DataRow dtRow in ds.Tables[0].Rows)
                                {
                                    approvalCode = Convert.ToString(dtRow["DocNum"]);
                                    if (approvalCode.ToLower() == DOCNUM.ToLower())
                                        isValidateAssignmentForm = false;
                                    break;
                                }
                            }
                        }
                    }
                    if (isValidateAssignmentForm)
                    {
                        parmList = new List<B1SP_Parameter>();
                        parm = new B1SP_Parameter();
                        parm.ParameterName = "DocNum";
                        parm.ParameterValue = Convert.ToString(DOCNUM);
                        parmList.Add(parm);

                        DataTable dt = HANADAL.GetDataTableByStoredProcedure("ValidateAssignmentForm", parmList, "AssignmentFormManagement");
                        if (dt.Rows.Count > 0)
                        {
                            msg = "Assignment Form Already Exist!";
                            return false;
                        }
                    }

                    List<AssignmentForm> previousData = new List<Models.AssignmentForm>();
                    //For Log
                    if (ID > 0)
                    {
                        previousData = GetAssignmentForm(ID);
                        if (previousData.Count > 0)
                        {
                            AddAssignmentForm_Log(docNum, AssignmentForm, UserID, ID, previousData[0], ID);
                        }             
                    }


                    DataTable dt_AssignmentForm = HANADAL.AddUpdateDataByStoredProcedure("AddUpdateAssignmentForm", TranslateAssignmentFormToParameterList(docNum, NonChargeable, AssignmentForm, ID, UserID), "AssignmentFormManagement");
                    if (dt_AssignmentForm.Rows.Count == 0)
                        throw new Exception("Exception occured when AddUpdateAssignmentForm,  Assignment Code:" + docNum + " , Assignment Title" + AssignmentForm.AssignmentTitle);
                    else
                    {

                    }

                    msg = "Successfully Added/Updated";
                    isSuccess = true;
                    int CreatedBy = Convert.ToInt32(AssignmentForm.CreatedBy);
                    Task.Run(() => cmn.AddMasterLog(isAddOccured, isUpdateOccured, isDeleteOccured, nameof(Enums.FormsName.AssignmentForm), CreatedBy, "AssignmentFormManagement"));
                    //End MAster Log


                    isSuccess = true;

                    //if (AssignmentForm.Status == 2)
                    //{
                    //    //For Notification and Email when time sheet submit
                    //    System.Web.Routing.RequestContext requestContext = HttpContext.Current.Request.RequestContext;
                    //    string lnkHref = new System.Web.Mvc.UrlHelper(requestContext).Action("GetApprovalDecision", "Home", new { empID = "EncryptedID" }, HttpContext.Current.Request.Url.Scheme);
                    //    cmn = new Common();
                    //    Task.Run(() => cmn.SndNotificationAndEmail(Convert.ToInt32(AssignmentForm.CreatedBy), AssignmentFormGeneral.AssignmentValue, 0, lnkHref, "AssignmentFormManagement"));
                    //}
                }
                catch (Exception ex)
                {
                    msg = "Exception occured in foreach loop Add/Update Assignment Form !";
                    isSuccess = false;
                    Log log = new Log();
                    log.LogFile(ex.Message);
                    log.InputOutputDocLog("AssignmentFormManagement", "Exception occured in foreach loop AddUpdateAssignmentForm, " + ex.Message);
                }


            }
            catch (Exception ex)
            {
                msg = "Exception occured in foreach loop Add/Update Assignment Form!";
                isSuccess = false;
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("AssignmentFormManagement", "Exception occured on AddUpdateAssignmentForm, " + ex.Message);
            }

            return isSuccess;
        }

        public bool AddUpdateAssignmentForm(out string msg, string DOCNUM, bool NonChargeable, AssignmentForm AssignmentForm, AssignmentFormGeneral AssignmentFormGeneral, List<AssignmentFormChild> AssignmentFormChild, List<AssignmentFormCost> AssignmentFormCost, List<AssignmentFormSummary> AssignmentFormSummary, int ID, int UserID)
        {
            bool isSuccess = false;
            bool isAddOccured = false;
            bool isUpdateOccured = false;
            bool isDeleteOccured = false;
            Common cmn = new Common();

            msg = "";
            try
            {
                
                string docNum = GetDocID(DOCNUM);
                List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
                B1SP_Parameter parm = new B1SP_Parameter();
                string approvalCode = "";
                bool isValidateAssignmentForm = true;
                try
                {
                    HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();

                    if (ID > 0)
                    {
                        parmList = new List<B1SP_Parameter>();
                        parm = new B1SP_Parameter();
                        parm.ParameterName = "ID";
                        parm.ParameterValue = Convert.ToString(ID);
                        parmList.Add(parm);
                        DataSet ds = HANADAL.GetDataSetByStoredProcedure("GetAssignmentForm", parmList, "AssignmentFormManagement");
                        if (ds.Tables.Count > 0)
                        {
                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                foreach (DataRow dtRow in ds.Tables[0].Rows)
                                {
                                    approvalCode = Convert.ToString(dtRow["DocNum"]);
                                    if (approvalCode.ToLower() == DOCNUM.ToLower())
                                        isValidateAssignmentForm = false;
                                    break;
                                }
                            }
                        }
                    }
                    if (isValidateAssignmentForm)
                    {
                        parmList = new List<B1SP_Parameter>();
                        parm = new B1SP_Parameter();
                        parm.ParameterName = "DocNum";
                        parm.ParameterValue = Convert.ToString(DOCNUM);
                        parmList.Add(parm);

                        DataTable dt = HANADAL.GetDataTableByStoredProcedure("ValidateAssignmentForm", parmList, "AssignmentFormManagement");
                        if (dt.Rows.Count > 0)
                        {
                            msg = "Assignment Form Already Exist!";
                            return false;
                        }
                    }

                    List<AssignmentForm> previousData = new List<Models.AssignmentForm>();
                    //For Log
                    if (ID > 0)
                    {
                        try
                        {
                            previousData = GetAssignmentForm(ID);
                            if (previousData.Count > 0)
                            {
                                AddAssignmentForm_Log(docNum, AssignmentForm, UserID, ID, previousData[0], ID);
                                if (previousData[0].General.Count > 0)
                                {
                                    List<AssignmentFormGeneral> general = new List<AssignmentFormGeneral>();
                                    // AssignmentForm.General =<AssignmentFormGeneral> AssignmentFormGeneral;
                                    general.Add(AssignmentFormGeneral);
                                    foreach (var list in general)
                                    {
                                        AssignmentForm.General = general;
                                        AddAssignmentFormGeneral_Log(AssignmentForm.General, previousData[0].General, ID);
                                    }

                                }
                                if (previousData[0].Table.Count > 0)
                                {
                                    AssignmentForm.Table = AssignmentFormChild;
                                    AddAssignmentFormResource_Log(AssignmentForm.Table, previousData[0].Table, ID);
                                }

                            }
                            if(previousData[0].Table2 != null)
                            {
                                if (previousData[0].Table2.Count > 0)
                                {
                                    AssignmentForm.Table2 = AssignmentFormCost;
                                    AddAssignmentFormCost_Log(AssignmentForm.Table2, previousData[0].Table2, ID);
                                }
                            }
                            
                            if (previousData[0].Table3.Count > 0)
                            {
                                AssignmentForm.Table3 = AssignmentFormSummary;
                                AddAssignmentFormSummary_Log(AssignmentForm.Table3, previousData[0].Table3, ID);
                            }
                        }
                        catch (Exception ex)
                        {
                            isSuccess = false;
                            Log log = new Log();
                            log.LogFile(ex.Message);
                            log.InputOutputDocLog("AssignmentFormManagement", "Exception occured in logs AddUpdateAssignmentFormGeneral, " + ex.Message);
                        }

                    }


                    DataTable dt_AssignmentForm = HANADAL.AddUpdateDataByStoredProcedure("AddUpdateAssignmentForm", TranslateAssignmentFormToParameterList(docNum, NonChargeable, AssignmentForm, ID, UserID), "AssignmentFormManagement");
                    if (dt_AssignmentForm.Rows.Count == 0)
                        throw new Exception("Exception occured when AddUpdateAssignmentForm,  Assignment Code:" + docNum + " , Assignment Title" + AssignmentForm.AssignmentTitle);
                    else
                    {
                        ID = Convert.ToInt32(dt_AssignmentForm.Rows[0]["ID"]);
                        if (ID > 0)
                        {
                            AssignmentForm.AssignmentFormID = ID;
                            
                            try
                            {
                                AssignmentFormGeneral.AssignmentFormID = ID;
                                HANADAL = new HANA_DAL_ODBC();
                                DataTable dt_AssignmentFormGeneral;
                                if (AssignmentFormGeneral.ClosureDate == null)
                                {
                                    dt_AssignmentFormGeneral = HANADAL.AddUpdateDataByStoredProcedure("AddUpdateAssignmentFormGeneralWithOut", TranslateAssignmentFormGeneralToParameterList(AssignmentFormGeneral, UserID), "AssignmentFormManagement");
                                }
                                else
                                {
                                    dt_AssignmentFormGeneral = HANADAL.AddUpdateDataByStoredProcedure("AddUpdateAssignmentFormGeneral", TranslateAssignmentFormGeneralToParameterList(AssignmentFormGeneral, UserID), "AssignmentFormManagement");
                                }                               
                                if (dt_AssignmentFormGeneral.Rows.Count == 0)
                                    throw new Exception("Exception occured when AddUpdateAssignmentFormGeneral,  Assignment ID:" + ID);
                            }
                            catch (Exception ex)
                            {
                                isSuccess = false;
                                Log log = new Log();
                                log.LogFile(ex.Message);
                                log.InputOutputDocLog("AssignmentFormManagement", "Exception occured in foreach loop AddUpdateAssignmentFormGeneral, " + ex.Message);
                            }
                            
                            
                            foreach (var list in AssignmentFormChild)
                            {

                                try
                                {
                                    list.AssignmentFormID = ID;
                                    HANADAL = new HANA_DAL_ODBC();
                                    DataTable dt_AssignmentFormResource = HANADAL.AddUpdateDataByStoredProcedure("AddUpdateAssignmentFormResource", TranslateAssignmentFormChildToParameterList(list), "AssignmentFormManagement");
                                    if (dt_AssignmentFormResource.Rows.Count == 0)
                                        throw new Exception("Exception occured when Add/Update AssignmentForm Resource, ID:" + list.RowID + " , USER CODE" + list.USER_CODE);
                                }
                                catch (Exception ex)
                                {
                                    isSuccess = false;
                                    Log log = new Log();
                                    log.LogFile(ex.Message);
                                    log.InputOutputDocLog("AssignmentFormManagement", "Exception occured in foreach loop AddUpdateAssignmentFormResource, " + ex.Message);
                                    continue;
                                }
                            }
                            if(AssignmentFormCost != null)
                            {
                                foreach (var list in AssignmentFormCost)
                                {

                                    try
                                    {
                                        list.AssignmentFormID = ID;
                                        HANADAL = new HANA_DAL_ODBC();
                                        DataTable dt_approvalSetup = HANADAL.AddUpdateDataByStoredProcedure("AddUpdateAssignmentFormCost", TranslateAssignmentFormCostToParameterList(list), "AssignmentFormManagement");
                                        if (dt_approvalSetup.Rows.Count == 0)
                                            throw new Exception("Exception occured when Add/Update AssignmentForm Cost, ID:" + list.ID + "  TYPE CODE" + list.TYPEOFCOST);
                                    }
                                    catch (Exception ex)
                                    {
                                        isSuccess = false;
                                        Log log = new Log();
                                        log.LogFile(ex.Message);
                                        log.InputOutputDocLog("AssignmentFormManagement", "Exception occured in foreach loop AddUpdateAssignmentFormCost, " + ex.Message);
                                        continue;
                                    }
                                }
                            }
                            
                            foreach (var list in AssignmentFormSummary)
                            {

                                try
                                {
                                    list.AssignmentFormID = ID;
                                    HANADAL = new HANA_DAL_ODBC();
                                    DataTable dt_approvalSetup = HANADAL.AddUpdateDataByStoredProcedure("AddUpdateAssignmentFormSummary", TranslateAssignmentFormSummaryToParameterList(list), "AssignmentFormManagement");
                                    if (dt_approvalSetup.Rows.Count == 0)
                                        throw new Exception("Exception occured when Add/Update AssignmentForm Summary, ID:" + list.TaskID );
                                }
                                catch (Exception ex)
                                {
                                    isSuccess = false;
                                    Log log = new Log();
                                    log.LogFile(ex.Message);
                                    log.InputOutputDocLog("AssignmentFormManagement", "Exception occured in foreach loop AddUpdateAssignmentFormSummary, " + ex.Message);
                                    continue;
                                }
                            }
                        }
                        if (previousData.Count > 0)
                        {
                            //For Deleted Items
                            List<AssignmentFormChild> missingList = previousData[0].Table.Where(n => !AssignmentFormChild.Any(o => o.RowID == n.RowID && o.ISDELETED == n.ISDELETED)).ToList();

                            foreach (AssignmentFormChild previousObject in missingList)
                            {
                                AssignmentFormChild newObj = new AssignmentFormChild();
                                newObj = previousObject;
                                newObj.ISDELETED = true;
                                newObj.UpdatedBy = UserID;
                                newObj.AssignmentFormID = ID;
                                isDeleteOccured = true;

                                HANADAL.AddUpdateDataByStoredProcedure("AddUpdateAssignmentFormResource", TranslateAssignmentFormChildToParameterList(newObj), "AssignmentFormManagement");
                            }
                        }
                    }

                    msg = "Successfully Added/Updated";
                    isSuccess = true;
                    int CreatedBy = Convert.ToInt32(AssignmentForm.CreatedBy);
                    Task.Run(() => cmn.AddMasterLog(isAddOccured, isUpdateOccured, isDeleteOccured, nameof(Enums.FormsName.AssignmentForm), CreatedBy, "AssignmentFormManagement"));
                    //End MAster Log
                    

                    if (AssignmentForm.Status == 2)
                    {
                        msg = "Successfully Submitted";
                        if (ID > 0)
                        {
                            try
                            {
                                DataTable dt_approvalDecision = HANADAL.AddUpdateDataByStoredProcedure("DeleteFromApprovalDecisionByDocID", TranslateIDToParameterList(ID), "AssignmentFormManagement");
                            }
                            catch (Exception ex)
                            {
                                msg = "Exception occured when Delete Approval Decision, Document_ID:" + ID +" !";
                                isSuccess = false;
                                Log log = new Log();
                                log.LogFile(ex.Message);
                                log.InputOutputDocLog("AssignmentFormManagement", "Exception occured in foreach loop AddUpdateAssignmentForm, " + ex.Message);
                            }
                        }

                        Encrypt_Decrypt security = new Encrypt_Decrypt();

                        //For Notification and Email when time sheet submit
                        System.Web.Routing.RequestContext requestContext = HttpContext.Current.Request.RequestContext;
                        string lnkHref = new System.Web.Mvc.UrlHelper(requestContext).Action("GetApprovalDecision", "Home", new { empID = "EncryptedID", docID = security.EncryptURLString(ID.ToString()), docType = security.EncryptURLString("Assignment") }, HttpContext.Current.Request.Url.Scheme);
                        cmn = new Common();
                        Task.Run(() => cmn.SndNotificationAndEmail(Convert.ToInt32(AssignmentForm.CreatedBy), AssignmentFormGeneral.AssignmentValue, 0, lnkHref, "AssignmentFormManagement"));
                    }
                }
                catch (Exception ex)
                {
                    msg = ex.Message;
                    isSuccess = false;
                    Log log = new Log();
                    log.LogFile(ex.Message);
                    log.InputOutputDocLog("AssignmentFormManagement", "Exception occured in foreach loop AddUpdateAssignmentForm, " + ex.Message);
                }
                

            }
            catch (Exception ex)
            {
                msg = "Exception occured in foreach loop Add/Update Assignment Form!";
                isSuccess = false;
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("AssignmentFormManagement", "Exception occured on AddUpdateAssignmentForm, " + ex.Message);
            }

            return isSuccess;
        }

        public bool POSTToSBO(out string msg, string DOCNUM, bool NonChargeable, AssignmentForm AssignmentForm, AssignmentFormGeneral AssignmentFormGeneral, List<AssignmentFormSummary> AssignmentFormSummary, int ID, int UserID)
        {
            bool isSuccess = true;
            msg = "Assignment document has posted successfully";
            try
            {
                List<AssignmentFormGeneral> AssignmentFormGenerals = new List<AssignmentFormGeneral>();
                AssignmentFormGenerals.Add(AssignmentFormGeneral);

                foreach (var list in AssignmentFormGenerals)
                {
                    AssignmentForm.General = AssignmentFormGenerals;
                }
                foreach (var list in AssignmentFormSummary)
                {
                    AssignmentForm.Table3 = AssignmentFormSummary;
                }
                var Json = JsonConvert.SerializeObject(AssignmentForm);

                string resp = PostInfo(Json);

                if (resp.ToString().Contains("successfully"))
                {
                    HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                    DataTable dt_AssignmentForm = HANADAL.AddUpdateDataByStoredProcedure("AddUpdateAssignmentForm", TranslateAssignmentFormToParameterList(DOCNUM, NonChargeable, AssignmentForm, ID, UserID), "AssignmentFormManagement");
                    if (dt_AssignmentForm.Rows.Count == 0)
                        throw new Exception("Exception occured when AddUpdateAssignmentForm,  Assignment Code:" + DOCNUM + " , Assignment Title" + AssignmentForm.AssignmentTitle);

                    isSuccess = true;
                    msg = resp.ToString();
                }
            }
            catch (Exception ex)
            {
                isSuccess = false;
                msg = ex.ToString();
            }
            return isSuccess;
        }

        public string PostInfo(string Json)
        {
            try
            {
                string url = ConfigurationManager.AppSettings["WEBAPIUtility"];
                url = url + "AssignmentCalled";

                var request = (HttpWebRequest)WebRequest.Create(url);

                request.Method = "POST";
                request.ContentType = "application/json";
                request.ContentLength = Json.Length;

                using (Stream webStream = request.GetRequestStream())
                using (StreamWriter requestWriter = new StreamWriter(webStream, System.Text.Encoding.ASCII))
                {
                    requestWriter.Write(Json);
                }
                var response = (HttpWebResponse)request.GetResponse();
                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                responseString = responseString.Replace("\"", "");
                return responseString;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public DataSet GetAssignmentFormLog(string id)
        {
            DataSet ds = new DataSet();
            try

            {
                List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();

                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = id;
                parmList.Add(parm);

                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                ds = HANADAL.GetDataSetByStoredProcedure("GetAssignmentFormLogByID", parmList, "AssignmentFormManagement");

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("AssignmentFormManagement", "Exception occured on GetAssignmentFormLog, " + ex.Message);
            }

            return ds;
        }

        public bool ValidateAssignmentTitle(string assignmentTitle, out AssignmentForm assignmentForm)
        {
            assignmentForm = new AssignmentForm();
            bool isSuccess = false;
            try
            {
                List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();

                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "AssignmentTitle";
                parm.ParameterValue = Convert.ToString(assignmentTitle);
                parmList.Add(parm);

                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                DataTable dt_AssignmentTitle = HANADAL.GetDataTableByStoredProcedure("GetAssignmentTitle", parmList, "AssignmentFormManagement");
                if (dt_AssignmentTitle.Rows.Count > 0)
                {
                    isSuccess = true;
                    assignmentForm = TranslateDataTableToAssignmentFormList(dt_AssignmentTitle);
                }

            }
            catch (Exception ex)
            {
                isSuccess = false;
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("AssignmentFormManagement", "Exception occured on ValidateAssignmentTitle, " + ex.Message);
            }
            return isSuccess;
        }

        #region "Translation assignmentForm"

        private List<B1SP_Parameter> TranslateIDSToParameterList(string ClientID, int BranchID)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {
                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "ClientID";
                parm.ParameterValue = Convert.ToString(ClientID);
                parm.ParameterType = DBTypes.String.ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "BranchID";
                parm.ParameterValue = Convert.ToString(BranchID);
                parm.ParameterType = DBTypes.Int32.ToString();
                parmList.Add(parm);

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("AssignmentFormManagement", "Exception occured on TranslateIDSToParameterList, " + ex.Message);
            }

            return parmList;
        }

        private List<B1SP_Parameter> TranslateIDSToParameterList(string FunctionID, int DesignationID, double FromDate, double ToDate)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {
                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "FunctionID";
                parm.ParameterValue = Convert.ToString(FunctionID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "DesignationID";
                parm.ParameterValue = Convert.ToString(DesignationID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "FromDate";
                //DateTime StartDate = Convert.ToDateTime(FromDate);

                //FromDate = string.Format("{0:dd/MM/yyyy}", StartDate);
                //FromDate = FromDate.Replace("/", "");
                parm.ParameterValue = Convert.ToString(FromDate);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ToDate";
                parm.ParameterValue = Convert.ToString(ToDate);
                parmList.Add(parm);

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("AssignmentFormManagement", "Exception occured on TranslateIDSToParameterList, " + ex.Message);
            }

            return parmList;
        }
             
        private List<B1SP_Parameter> TranslateIDToParameterList(int Id)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {
                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = Convert.ToString(Id);
                parmList.Add(parm);
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("AssignmentFormManagement", "Exception occured on TranslateIDToParameterList, " + ex.Message);
            }
            return parmList;
        }

        private List<string> TranslateDataTableToAssignmentFormkDocNumList(DataTable dt)
        {
            List<string> docNumList = new List<string>();
            foreach (DataRow dtRow in dt.Rows)
            {
                docNumList.Add(Convert.ToString(dtRow["DOCNUM"]));
            }

            return docNumList.Distinct().ToList();
        }


        //POSTDATA
        private List<B1SP_Parameter> TranslateAssignmentFormToParameterList(string DOCNUM, bool NonChargeable, AssignmentForm AssignmentForm, int ID, int UserID)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {
                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = Convert.ToString(ID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "DOCNUM";
                parm.ParameterValue = Convert.ToString(DOCNUM);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "NonChargeable";
                parm.ParameterValue = Convert.ToString(NonChargeable);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "DocDate";
                parm.ParameterValue = Convert.ToString(Convert.ToDateTime(AssignmentForm.DocDate).ToString("yyyy-MM-dd HH:MM:s")); //DateTime.Now.ToString("yyyy-MM-dd HH:MM:s");
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "AssignmentTitle";
                parm.ParameterValue = Convert.ToString(AssignmentForm.AssignmentTitle);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "BranchID";
                parm.ParameterValue = Convert.ToString(AssignmentForm.BranchID); //branch id nahe arahe
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ClientID";
                parm.ParameterValue = Convert.ToString(AssignmentForm.ClientID);//branch id nahe arahe
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "FunctionID";
                parm.ParameterValue = Convert.ToString(AssignmentForm.FunctionID);//branch id nahe arahe
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "SubFunctionID";
                parm.ParameterValue = Convert.ToString(AssignmentForm.SubFunctionID);//branch id nahe arahe
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "PartnerID";
                parm.ParameterValue = Convert.ToString(AssignmentForm.PartnerID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "DirectorID";
                parm.ParameterValue = (Convert.ToInt64(AssignmentForm.DirectorID)).ToString();
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "Status";
                parm.ParameterValue = Convert.ToString(AssignmentForm.Status);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "flgPost";
                parm.ParameterValue = Convert.ToString(AssignmentForm.flgPost);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "IsDeleted";
                parm.ParameterValue = Convert.ToString(false);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CreatedBy";
                parm.ParameterValue = Convert.ToString(UserID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CreateDate";
                parm.ParameterValue = DateTime.Now.ToString("yyyy-MM-dd HH:MM:s");
                parmList.Add(parm);

               
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("AssignmentFormManagement", "Exception occured on TranslateAssignmentFormToParameterList, " + ex.Message);
            }

            return parmList;
        }

        private List<B1SP_Parameter> TranslateAssignmentFormGeneralToParameterList(AssignmentFormGeneral AssignmentForm, int UserID)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {

                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = Convert.ToString(AssignmentForm.ID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "AssignmentFormID";
                parm.ParameterValue = Convert.ToString(AssignmentForm.AssignmentFormID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "TypeOfAssignment";
                parm.ParameterValue = Convert.ToString(AssignmentForm.TypeOfAssignment);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "AssignmentNatureID";
                parm.ParameterValue = Convert.ToString(AssignmentForm.AssignmentNatureID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "TypeOfBilling";
                parm.ParameterValue = Convert.ToString(AssignmentForm.TypeOfBilling);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CurrencyID";
                parm.ParameterValue = Convert.ToString(AssignmentForm.CurrencyID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "AssignmentValue";
                parm.ParameterValue = Convert.ToString(AssignmentForm.AssignmentValue);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "StartDate";
                parm.ParameterValue = Convert.ToString(Convert.ToDateTime(AssignmentForm.StartDate).ToString("yyyy-MM-dd HH:MM:s"));
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "EndDate";
                parm.ParameterValue = Convert.ToString(Convert.ToDateTime(AssignmentForm.EndDate).ToString("yyyy-MM-dd HH:MM:s"));
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "DurationInDays";
                parm.ParameterValue = Convert.ToString(AssignmentForm.DurationInDays);
                parmList.Add(parm);

                if(AssignmentForm.ClosureDate != null)
                {
                    parm = new B1SP_Parameter();
                    parm.ParameterName = "ClosureDate";
                    parm.ParameterValue = Convert.ToString(Convert.ToDateTime(AssignmentForm.ClosureDate).ToString("yyyy-MM-dd HH:MM:s"));
                    parmList.Add(parm);
                }

                parm = new B1SP_Parameter();
                parm.ParameterName = "Status";
                parm.ParameterValue = Convert.ToString(AssignmentForm.Status);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CreatedBy";
                parm.ParameterValue = Convert.ToString(UserID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CreateDate";
                parm.ParameterValue = DateTime.Now.ToString("yyyy-MM-dd HH:MM:s");
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "UpdatedBy";
                parm.ParameterValue = Convert.ToString(UserID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "UpdateDate";
                parm.ParameterValue = DateTime.Now.ToString("yyyy-MM-dd HH:MM:s");
                parmList.Add(parm);
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("AssignmentFormManagement", "Exception occured on TranslateAssignmentFormGeneralToParameterList, " + ex.Message);
            }

            return parmList;
        }

        private List<B1SP_Parameter> TranslateAssignmentFormChildToParameterList(AssignmentFormChild assignmentFormChild)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {

                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                if (assignmentFormChild.RowID == null)
                    assignmentFormChild.RowID = 0;
                parm.ParameterValue = Convert.ToString(assignmentFormChild.RowID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "AssignmentFormID";
                parm.ParameterValue = Convert.ToString(assignmentFormChild.AssignmentFormID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "EmpID";
                parm.ParameterValue = Convert.ToString(assignmentFormChild.ID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "EmpCode";
                parm.ParameterValue = Convert.ToString(assignmentFormChild.USER_CODE);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "DesignationID";
                parm.ParameterValue = Convert.ToString(assignmentFormChild.DesignationID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "DepartmentID";
                parm.ParameterValue = Convert.ToString(assignmentFormChild.DepartmentID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "TaskID";
                parm.ParameterValue = Convert.ToString(assignmentFormChild.TaskID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "TotalHours";
                parm.ParameterValue = Convert.ToString(assignmentFormChild.TotalHours);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "StdBillingRateHr";
                parm.ParameterValue = Convert.ToString(assignmentFormChild.StdBillingRateHr);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ResourceCost";
                parm.ParameterValue = Convert.ToString(assignmentFormChild.ResourceCost);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "LocationID";
                if (assignmentFormChild.TravelRateID == null)
                    assignmentFormChild.TravelRateID = 0;
                parm.ParameterValue = Convert.ToString(assignmentFormChild.TravelRateID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "TravelCost";
                parm.ParameterValue = Convert.ToString(assignmentFormChild.TravelCost);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "TotalCost";
                parm.ParameterValue = Convert.ToString(assignmentFormChild.TotalCost);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "RevenueRateHr";
                parm.ParameterValue = Convert.ToString(assignmentFormChild.RevenueRateHr);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "Revenue";
                parm.ParameterValue = Convert.ToString(assignmentFormChild.Revenue);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "IsChargeable";
                parm.ParameterValue = Convert.ToString(assignmentFormChild.IsChargeable);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "InActive";
                parm.ParameterValue = Convert.ToString(assignmentFormChild.InActive);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ISDELETED";
                if(assignmentFormChild.ISDELETED == null)
                {
                    assignmentFormChild.ISDELETED = false;
                }
                parm.ParameterValue = Convert.ToString(assignmentFormChild.ISDELETED);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CreatedBy";
                parm.ParameterValue = Convert.ToString(assignmentFormChild.CreatedBy);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CreateDate";
                parm.ParameterValue = DateTime.Now.ToString("yyyy-MM-dd HH:MM:s");
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "UpdatedBy";
                parm.ParameterValue = Convert.ToString(assignmentFormChild.UpdatedBy);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "UpdateDate";
                parm.ParameterValue = DateTime.Now.ToString("yyyy-MM-dd HH:MM:s");
                parmList.Add(parm);

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("AssignmentFormManagement", "Exception occured on TranslateAssignmentFormChildToParameterList, " + ex.Message);
            }


            return parmList;
        }

        private List<B1SP_Parameter> TranslateAssignmentFormCostToParameterList(AssignmentFormCost assignmentFormChild)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {

                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                if (assignmentFormChild.RowID == null)
                    assignmentFormChild.RowID = 0;
                parm.ParameterValue = Convert.ToString(assignmentFormChild.RowID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "AssignmentFormID";
                parm.ParameterValue = Convert.ToString(assignmentFormChild.AssignmentFormID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "AssignmentCostSetupID";
                parm.ParameterValue = Convert.ToString(assignmentFormChild.ID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "Amount";
                parm.ParameterValue = Convert.ToString(assignmentFormChild.Amount);
                parmList.Add(parm);

                //parm = new B1SP_Parameter();
                //parm.ParameterName = "InActive";
                //parm.ParameterValue = Convert.ToString(assignmentFormChild.InActive);
                //parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CreatedBy";
                parm.ParameterValue = Convert.ToString(assignmentFormChild.CreatedBy);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CreateDate";
                parm.ParameterValue = DateTime.Now.ToString("yyyy-MM-dd HH:MM:s");
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "UpdatedBy";
                parm.ParameterValue = Convert.ToString(assignmentFormChild.UpdatedBy);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "UpdateDate";
                parm.ParameterValue = DateTime.Now.ToString("yyyy-MM-dd HH:MM:s");
                parmList.Add(parm);

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("AssignmentFormManagement", "Exception occured on TranslateAssignmentFormCostManagementList, " + ex.Message);
            }


            return parmList;
        }

        private List<B1SP_Parameter> TranslateAssignmentFormSummaryToParameterList(AssignmentFormSummary assignmentFormChild)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {

                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                if (assignmentFormChild.RowID == null)
                    assignmentFormChild.RowID = 0;
                parm.ParameterValue = Convert.ToString(assignmentFormChild.RowID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "AssignmentFormID";
                parm.ParameterValue = Convert.ToString(assignmentFormChild.AssignmentFormID);
                parmList.Add(parm);
                
                parm = new B1SP_Parameter();
                parm.ParameterName = "TaskID";
                parm.ParameterValue = Convert.ToString(assignmentFormChild.TaskID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "TotalBudgetedHour";
                parm.ParameterValue = Convert.ToString(assignmentFormChild.TotalBudgetedHour);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "EstimatedResourceCost";
                parm.ParameterValue = Convert.ToString(assignmentFormChild.EstimatedResourceCost);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "RevenueDistribution";
                parm.ParameterValue = Convert.ToString(assignmentFormChild.RevenueDistribution);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "EstimatedRevenue";
                parm.ParameterValue = Convert.ToString(assignmentFormChild.EstimatedRevenue);
                parmList.Add(parm);
                
                parm = new B1SP_Parameter();
                parm.ParameterName = "CreatedBy";
                parm.ParameterValue = Convert.ToString(assignmentFormChild.CreatedBy);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CreateDate";
                parm.ParameterValue = DateTime.Now.ToString("yyyy-MM-dd HH:MM:s");
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "UpdatedBy";
                parm.ParameterValue = Convert.ToString(assignmentFormChild.UpdatedBy);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "UpdateDate";
                parm.ParameterValue = DateTime.Now.ToString("yyyy-MM-dd HH:MM:s");
                parmList.Add(parm);

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("AssignmentFormManagement", "Exception occured on TranslateAssignmentFormSummaryToParameterList, " + ex.Message);
            }


            return parmList;
        }

        //GETDATA
        private List<AssignmentFormChild> TranslateDataTableToTripRatePolicyManagementList(DataTable dt)
        {
            List<AssignmentFormChild> TravelLocationL = new List<AssignmentFormChild>();

            try
            {
                int sno = 1;
                foreach (DataRow dtRow in dt.Rows)
                {
                    AssignmentFormChild TravelLocation = new AssignmentFormChild();
                    TravelLocation.SNO = sno;
                    TravelLocation.KEY = Guid.NewGuid().ToString();
                    TravelLocation.TravelRateID = Convert.ToInt32(dtRow["ID"]);
                    TravelLocation.BranchID = Convert.ToInt32(dtRow["BRANCHID"]);
                    TravelLocation.ClientID = Convert.ToString(dtRow["CLIENTNAME"]);
                    TravelLocation.LOCATION = Convert.ToString(dtRow["LOCATION"]);
                    TravelLocation.KM = Convert.ToDouble(dtRow["KM"]);
                    TravelLocation.FROMKM = Convert.ToDouble(dtRow["FROMKM"]);
                    TravelLocation.TOKM = Convert.ToDouble(dtRow["TOKM"]);
                    TravelLocation.RATETRIP = Convert.ToDouble(dtRow["RATETRIP"]);

                    sno = sno + 1;
                    TravelLocationL.Add(TravelLocation);
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("AssignmentFormManagement", "Exception occured on TranslateDataTableToTravelLocationManagementList, " + ex.Message);
            }

            return TravelLocationL;
        }

        private List<TaskMasterSetupInfo> TranslateDataTableToTaskMasterManagementList(DataTable dt)
        {
            List<TaskMasterSetupInfo> TaskMasterSetupInfo = new List<TaskMasterSetupInfo>();

            try
            {
                int sno = 1;
                foreach (DataRow dtRow in dt.Rows)
                {
                    TaskMasterSetupInfo taskMasterSetup = new TaskMasterSetupInfo();
                    taskMasterSetup.SNO = sno;
                    taskMasterSetup.KEY = Guid.NewGuid().ToString();
                    taskMasterSetup.TaskID = Convert.ToInt32(dtRow["ID"]);
                    taskMasterSetup.TASK = Convert.ToString(dtRow["TASK"]);
                    taskMasterSetup.FUNCTIONID = Convert.ToInt32(dtRow["FUNCTIONID"]);
                    taskMasterSetup.FUNCTIONNAME = Convert.ToString(dtRow["FUNCTIONNAME"]);
                    taskMasterSetup.DOCNUM = Convert.ToString(dtRow["DOCNUM"]);
                    taskMasterSetup.CREATEDBY = Convert.ToInt32(dtRow["CREATEDBY"]);
                    taskMasterSetup.CREATEDDATE = Convert.ToDateTime(dtRow["CREATEDDATE"]);

                    if (dtRow["UPDATEDEDBY"] != DBNull.Value)
                        taskMasterSetup.UPDATEDEDBY = Convert.ToInt32(dtRow["UPDATEDEDBY"]);
                    if (dtRow["UPDATEDDATE"] != DBNull.Value)
                        taskMasterSetup.UPDATEDDATE = Convert.ToDateTime(dtRow["UPDATEDDATE"]);

                    taskMasterSetup.ISACTIVE = Convert.ToBoolean(dtRow["ISACTIVE"]);
                    taskMasterSetup.ISDELETED = Convert.ToBoolean(dtRow["ISDELETED"]);
                    sno = sno + 1;
                    TaskMasterSetupInfo.Add(taskMasterSetup);
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("AssignmentFormManagement", "Exception occured on TranslateDataTableToTaskMasterManagementList, " + ex.Message);
            }

            return TaskMasterSetupInfo;
        }

        private List<AssignmentForm> TranslateDataTableToAssignmentFormManagementList(DataTable dt)
        {
            List<AssignmentForm> assignmentFormL = new List<AssignmentForm>();

            try
            {
                int sno = 1;
                foreach (DataRow dtRow in dt.Rows)
                {
                    AssignmentForm assignmentForm = new AssignmentForm();
                    //assignmentForm.SNO = sno;
                    assignmentForm.KEY = Guid.NewGuid().ToString();
                    assignmentForm.ID = Convert.ToInt32(dtRow["ID"]);
                    assignmentForm.DOCNUM = Convert.ToString(dtRow["DocNum"]);
                    assignmentForm.NonChargeable = Convert.ToBoolean(dtRow["NonChargeable"]);
                    assignmentForm.DocDate = Convert.ToString(dtRow["DocDate"]);
                    assignmentForm.AssignmentTitle = Convert.ToString(dtRow["AssignmentTitle"]);
                    assignmentForm.BranchID = Convert.ToInt32(dtRow["BranchID"]);
                    assignmentForm.ClientID = Convert.ToString(dtRow["ClientID"]);
                    assignmentForm.FunctionID = Convert.ToString(dtRow["FunctionID"]);
                    assignmentForm.SubFunctionID = Convert.ToString(dtRow["SubFunctionID"]);
                    assignmentForm.PartnerID = Convert.ToString(dtRow["PartnerID"]);
                    assignmentForm.DirectorID = Convert.ToInt32(dtRow["DirectorID"]);
                    string stts = Convert.ToString(dtRow["Status"]);
                    if (stts == "" || stts == null)
                        assignmentForm.Status = 0;
                    else
                        assignmentForm.Status = Convert.ToInt32(dtRow["Status"]);

                    assignmentForm.flgPost = Convert.ToBoolean(dtRow["flgPost"]);
                    if (Convert.ToString(dtRow["DocDate"]) != "")
                        assignmentForm.DocDate = Convert.ToString(Convert.ToDateTime(dtRow["DocDate"]).ToString("MM/dd/yyyy"));

                    assignmentForm.IsDeleted = Convert.ToBoolean(dtRow["IsDeleted"]);
                    assignmentForm.CreatedBy = Convert.ToInt32(dtRow["CreatedBy"]);
                    assignmentForm.CreateDate = Convert.ToDateTime(dtRow["CreateDate"]);
                    
                    if (dtRow["UpdatedBy"] != DBNull.Value)
                        assignmentForm.UpdatedBy = Convert.ToInt32(dtRow["UpdatedBy"]);
                    if (dtRow["UpdateDate"] != DBNull.Value)
                        assignmentForm.UpdateDate = Convert.ToDateTime(dtRow["UpdateDate"]);

                    sno = sno + 1;
                    assignmentFormL.Add(assignmentForm);
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("AssignmentFormManagement", "Exception occured on TranslateDataTableToAssignmentFormManagementList, " + ex.Message);
            }

            return assignmentFormL;
        }

        private List<B1SP_Parameter> TranslateAssignmentFormIDToParameterList(int AssignmentFormID)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {

                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "AssignmentFormID";
                parm.ParameterValue = Convert.ToString(AssignmentFormID);
                parmList.Add(parm);


            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("AssignmentFormManagement", "Exception occured on TranslateAssignmentFormToParameterList, " + ex.Message);
            }


            return parmList;
        }

        private List<AssignmentFormGeneral> TranslateDataTableToAssignmentFormGeneralManagementList(DataTable dt)
        {
            List<AssignmentFormGeneral> assignmentFormL = new List<AssignmentFormGeneral>();

            try
            {
                int sno = 1;
                foreach (DataRow dtRow in dt.Rows)
                {
                    AssignmentFormGeneral assignmentForm = new AssignmentFormGeneral();
                    assignmentForm.SNO = sno;
                    assignmentForm.KEY = Guid.NewGuid().ToString();
                    assignmentForm.ID = Convert.ToInt32(dtRow["ID"]);

                    assignmentForm.AssignmentFormID = Convert.ToInt32(dtRow["AssignmentFormID"]);
                    assignmentForm.TypeOfAssignment = Convert.ToString(dtRow["TypeOfAssignment"]);
                    assignmentForm.AssignmentNatureID = Convert.ToInt32(dtRow["AssignmentNatureID"]);
                    assignmentForm.TypeOfBilling = Convert.ToString(dtRow["TypeOfBilling"]);
                    assignmentForm.CurrencyID = Convert.ToString(dtRow["CurrencyID"]);
                    assignmentForm.AssignmentValue = Convert.ToInt32(dtRow["AssignmentValue"]);
                    if (Convert.ToString(dtRow["StartDate"]) != "")
                        assignmentForm.StartDate = Convert.ToString(Convert.ToDateTime(dtRow["StartDate"]).ToString("MM/dd/yyyy"));
                    if (Convert.ToString(dtRow["EndDate"]) != "")
                        assignmentForm.EndDate = Convert.ToString(Convert.ToDateTime(dtRow["EndDate"]).ToString("MM/dd/yyyy"));
                    assignmentForm.DurationInDays = Convert.ToInt32(dtRow["DurationInDays"]);
                    if (Convert.ToString(dtRow["ClosureDate"]) != "")
                        assignmentForm.ClosureDate = Convert.ToString(Convert.ToDateTime(dtRow["ClosureDate"]).ToString("MM/dd/yyyy"));
                    assignmentForm.Status = Convert.ToString(dtRow["Status"]);
                    assignmentForm.CreatedBy = Convert.ToInt32(dtRow["CreatedBy"]);
                    assignmentForm.CreateDate = Convert.ToDateTime(dtRow["CreateDate"]);

                    if (dtRow["UpdatedBy"] != DBNull.Value)
                        assignmentForm.UpdatedBy = Convert.ToInt32(dtRow["UpdatedBy"]);
                    if (dtRow["UpdateDate"] != DBNull.Value)
                        assignmentForm.UpdateDate = Convert.ToDateTime(dtRow["UpdateDate"]);

                    sno = sno + 1;
                    assignmentFormL.Add(assignmentForm);
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("AssignmentFormManagement", "Exception occured on TranslateDataTableToAssignmentFormGeneralManagementList, " + ex.Message);
            }

            return assignmentFormL;
        }

        private List<AssignmentFormChild> TranslateDataTableToAssignmentFormChildManagementList(DataTable dt)
        {
            BAL.UserManagement usr = new BAL.UserManagement();
            List<AssignmentFormChild> assignmentFormL = new List<AssignmentFormChild>();
            HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();

            try
            {
                int sno = 1;
                foreach (DataRow dtRow in dt.Rows)
                {
                    AssignmentFormChild assignmentForm = new AssignmentFormChild();
                    assignmentForm.SNO = sno;
                    assignmentForm.KEY = Guid.NewGuid().ToString();
                    assignmentForm.RowID = Convert.ToInt32(dtRow["ID"]);

                    assignmentForm.AssignmentFormID = Convert.ToInt32(dtRow["AssignmentFormID"]);
                    assignmentForm.ID = Convert.ToInt32(dtRow["EmpID"]);
                    //assignmentForm.EmpCode = Convert.ToString(dtRow["EmpCode"]);
                    assignmentForm.USER_CODE = Convert.ToString(dtRow["EmpCode"]);
                    //assignmentForm.USER_NAME = Convert.ToString(dtRow["USER_NAME"]);
                    assignmentForm.UserID = Convert.ToInt32(dtRow["EmpID"]);
                    assignmentForm.DesignationID = Convert.ToInt32(dtRow["DesignationID"]);
                    assignmentForm.DepartmentID = Convert.ToInt32(dtRow["DepartmentID"]);

                    if (!string.IsNullOrEmpty(assignmentForm.USER_CODE))
                    {
                        HCM_Employee emp = usr.GetHCMOneEmployeeByCode(assignmentForm.USER_CODE);
                        if (emp != null)
                        {
                            assignmentForm.DEPARTMENTNAME = emp.DepartmentName;
                            assignmentForm.DESIGNATIONNAME = emp.DesignationName;
                            assignmentForm.FULLNAME = emp.EmpName;
                        }
                    }
                    assignmentForm.TaskID = Convert.ToInt32(dtRow["TaskID"]);
                    //assignmentForm.TASK = Convert.ToString(dtRow["TaskID"]);
                    assignmentForm.TotalHours = Convert.ToDouble(dtRow["TotalHours"]);
                    assignmentForm.StdBillingRateHr = Convert.ToDouble(dtRow["StdBillingRateHr"]);
                    assignmentForm.ResourceCost = Convert.ToDouble(dtRow["ResourceCost"]);
                    assignmentForm.TravelRateID = Convert.ToInt32(dtRow["LocationID"]);
                    assignmentForm.TravelCost = Convert.ToDouble(dtRow["TravelCost"]);
                    assignmentForm.TotalCost = Convert.ToDouble(dtRow["TotalCost"]);
                    assignmentForm.RevenueRateHr = Convert.ToDouble(dtRow["RevenueRateHr"]);
                    assignmentForm.Revenue = Convert.ToDouble(dtRow["Revenue"]);
                    assignmentForm.IsChargeable = Convert.ToBoolean(dtRow["IsChargeable"]);
                    assignmentForm.InActive = Convert.ToBoolean(dtRow["InActive"]);
                    assignmentForm.ISDELETED = Convert.ToBoolean(dtRow["ISDELETED"]);
                    assignmentForm.CreatedBy = Convert.ToInt32(dtRow["CREATEDBY"]);
                    assignmentForm.CreateDate = Convert.ToDateTime(dtRow["CREATEDATE"]);

                    if (dtRow["UPDATEDBY"] != DBNull.Value)
                        assignmentForm.UpdatedBy = Convert.ToInt32(dtRow["UPDATEDBY"]);
                    if (dtRow["UPDATEDATE"] != DBNull.Value)
                        assignmentForm.UPDATEDATE = Convert.ToDateTime(dtRow["UPDATEDATE"]);

                    sno = sno + 1;
                    assignmentFormL.Add(assignmentForm);
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("AssignmentFormManagement", "Exception occured on TranslateDataTableToAssignmentFormChildManagementList, " + ex.Message);
            }

            return assignmentFormL;
        }

        private List<AssignmentFormCost> TranslateDataTableToAssignmentFormCostManagementList(DataTable dt)
        {
            List<AssignmentFormCost> assignmentFormL = new List<AssignmentFormCost>();
            HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();

            try
            {
                int sno = 1;
                foreach (DataRow dtRow in dt.Rows)
                {
                    AssignmentFormCost assignmentForm = new AssignmentFormCost();
                    assignmentForm.SNO = sno;
                    assignmentForm.KEY = Guid.NewGuid().ToString();
                    assignmentForm.RowID = Convert.ToInt32(dtRow["ID"]);

                    assignmentForm.AssignmentFormID = Convert.ToInt32(dtRow["AssignmentFormID"]);
                    assignmentForm.ID = Convert.ToInt32(dtRow["AssignmentCostSetupID"]);
                    assignmentForm.Amount = Convert.ToDouble(dtRow["Amount"]);
                    assignmentForm.CreatedBy = Convert.ToInt32(dtRow["CreatedBy"]);
                    assignmentForm.CreateDate = Convert.ToDateTime(dtRow["CreateDate"]);

                    if (dtRow["UpdatedBy"] != DBNull.Value)
                        assignmentForm.UpdatedBy = Convert.ToInt32(dtRow["UpdatedBy"]);
                    if (dtRow["UpdateDate"] != DBNull.Value)
                        assignmentForm.UpdateDate = Convert.ToDateTime(dtRow["UpdateDate"]);

                    sno = sno + 1;
                    assignmentFormL.Add(assignmentForm);
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("AssignmentFormManagement", "Exception occured on TranslateDataTableToAssignmentFormCostManagementList, " + ex.Message);
            }

            return assignmentFormL;
        }

        private List<AssignmentFormSummary> TranslateDataTableToAssignmentFormSummaryManagementList(DataTable dt)
        {
            List<AssignmentFormSummary> assignmentFormL = new List<AssignmentFormSummary>();
            HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();

            try
            {
                int sno = 1;
                foreach (DataRow dtRow in dt.Rows)
                {
                    AssignmentFormSummary assignmentForm = new AssignmentFormSummary();
                    assignmentForm.SNO = sno;
                    assignmentForm.KEY = Guid.NewGuid().ToString();
                    assignmentForm.RowID = Convert.ToInt32(dtRow["ID"]);

                    assignmentForm.AssignmentFormID = Convert.ToInt32(dtRow["AssignmentFormID"]);
                    assignmentForm.TaskID = Convert.ToInt32(dtRow["TaskID"]);
                    //assignmentForm.TASK = Convert.ToString(dtRow["TaskID"]);
                    assignmentForm.TotalBudgetedHour = Convert.ToDouble(dtRow["TotalBudgetedHour"]);
                    assignmentForm.EstimatedResourceCost = Convert.ToDouble(dtRow["EstimatedResourceCost"]);
                    assignmentForm.RevenueDistribution = Convert.ToDouble(dtRow["RevenueDistribution"]);
                    assignmentForm.EstimatedRevenue = Convert.ToDouble(dtRow["EstimatedRevenue"]);
                    assignmentForm.CreatedBy = Convert.ToInt32(dtRow["CreatedBy"]);
                    assignmentForm.CreateDate = Convert.ToDateTime(dtRow["CreateDate"]);

                    if (dtRow["UpdatedBy"] != DBNull.Value)
                        assignmentForm.UpdatedBy = Convert.ToInt32(dtRow["UpdatedBy"]);
                    if (dtRow["UpdateDate"] != DBNull.Value)
                        assignmentForm.UpdateDate = Convert.ToDateTime(dtRow["UpdateDate"]);

                    sno = sno + 1;
                    assignmentFormL.Add(assignmentForm);
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("AssignmentFormManagement", "Exception occured on TranslateDataTableToAssignmentFormSummaryManagementList, " + ex.Message);
            }

            return assignmentFormL;
        }

        private AssignmentForm TranslateDataTableToAssignmentFormList(DataTable dt)
        {
            AssignmentForm assignmentForm = new AssignmentForm();

            try
            {
                int sno = 1;
                foreach (DataRow dtRow in dt.Rows)
                {
                    //assignmentForm.SNO = sno;
                    assignmentForm.KEY = Guid.NewGuid().ToString();
                    assignmentForm.ID = Convert.ToInt32(dtRow["ID"]);
                    assignmentForm.DOCNUM = Convert.ToString(dtRow["DocNum"]);
                    assignmentForm.NonChargeable = Convert.ToBoolean(dtRow["NonChargeable"]);
                    assignmentForm.DocDate = Convert.ToString(dtRow["DocDate"]);
                    assignmentForm.AssignmentTitle = Convert.ToString(dtRow["AssignmentTitle"]);
                    assignmentForm.BranchID = Convert.ToInt32(dtRow["BranchID"]);
                    assignmentForm.ClientID = Convert.ToString(dtRow["ClientID"]);
                    assignmentForm.FunctionID = Convert.ToString(dtRow["FunctionID"]);
                    assignmentForm.SubFunctionID = Convert.ToString(dtRow["SubFunctionID"]);
                    assignmentForm.PartnerID = Convert.ToString(dtRow["PartnerID"]);
                    assignmentForm.DirectorID = Convert.ToInt32(dtRow["DirectorID"]);
                    string stts = Convert.ToString(dtRow["Status"]);
                    if (stts == "" || stts == null)
                        assignmentForm.Status = 0;
                    else
                        assignmentForm.Status = Convert.ToInt32(dtRow["Status"]);

                    assignmentForm.flgPost = Convert.ToBoolean(dtRow["flgPost"]);
                    assignmentForm.IsDeleted = Convert.ToBoolean(dtRow["IsDeleted"]);
                    assignmentForm.CreatedBy = Convert.ToInt32(dtRow["CreatedBy"]);
                    assignmentForm.CreateDate = Convert.ToDateTime(dtRow["CreateDate"]);

                    if (dtRow["UpdatedBy"] != DBNull.Value)
                        assignmentForm.UpdatedBy = Convert.ToInt32(dtRow["UpdatedBy"]);
                    if (dtRow["UpdateDate"] != DBNull.Value)
                        assignmentForm.UpdateDate = Convert.ToDateTime(dtRow["UpdateDate"]);

                    sno = sno + 1;
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("AssignmentFormManagement", "Exception occured on TranslateDataTableToAssignmentFormManagementList, " + ex.Message);
            }

            return assignmentForm;
        }

        #endregion

    }
}