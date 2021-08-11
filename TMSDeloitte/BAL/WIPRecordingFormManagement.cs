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
using System.Web.Http;
using TMSDeloitte.DAL;
using TMSDeloitte.Helper;
using TMSDeloitte.Models;

namespace TMSDeloitte.BAL
{
    public class WIPRecordingFormManagement
    {
        public string GetDocID(string DOCNUM)
        {
            int no = 1;
            string docNum = "";
            if (DOCNUM == "")
            {
                List<string> docNumList = GetWIPRecordingFormDocNum();// GetTaskMasterDocNum();
                if (docNumList.Count > 0)
                {
                    var item = docNumList[docNumList.Count - 1];
                    //no = Convert.ToInt32(item.Split('-')[1]);
                    no = Convert.ToInt32(item);
                    no = no + 1;
                    docNum = Convert.ToString(no).PadLeft(7, '0');
                }
                else
                {
                    //docNum = Convert.ToString(no).PadLeft(7, '0');
                    docNum = "1"+Convert.ToString(no).PadLeft(6, '0');
                }
                //docNum = "1000008";
            }
            else
            {
                docNum = DOCNUM;
            }
            return docNum;
        }

        public List<string> GetWIPRecordingFormDocNum()
        {
            List<string> docNumList = new List<string>();
            try
            {
                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();

                DataTable dt = HANADAL.GetDataTableByStoredProcedure("GetWIPRecordingFormDocNum", "WIPRecordingFormManagement");
                //DataTable dt = HANADAL.GetDataTableByStoredProcedure("GetWIPRecordingFormSAPDocNum", "WIPRecordingFormManagement");
                if (dt.Rows.Count > 0)
                {
                    docNumList = TranslateDataTableToMaster_WIPDocNumList(dt);
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("WIPRecordingFormManagement", "Exception occured on GetWIPRecordingFormDocNum, " + ex.Message);
            }

            return docNumList;
        }

        private List<string> TranslateDataTableToMaster_WIPDocNumList(DataTable dt)
        {
            List<string> docNumList = new List<string>();
            foreach (DataRow dtRow in dt.Rows)
            {
                //docNumList.Add(Convert.ToString(dtRow["BaseRef"]));
                docNumList.Add(Convert.ToString(dtRow["DocNum"]));
            }

            return docNumList.Distinct().ToList();
        }

        public List<WIPRecordingForm> GetWIPRecordingForm(int ID)
        {
            List<WIPRecordingForm> wipRecordingForm = new List<WIPRecordingForm>();
            List<WIPRecordingFormChild> wipRecordingFormChild = new List<WIPRecordingFormChild>();

            try
            {

                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                //DataTable dt_WIPRecordingForm = HANADAL.GetDataTableByStoredProcedure("GetWIPDocumentFromSAP", TranslateIDToParameterList(ID), "WIPRecordingFormManagement");
                DataTable dt_WIPRecordingForm = HANADAL.GetDataTableByStoredProcedure("GetWIPRecordingForm", TranslateIDToParameterList(ID), "WIPRecordingFormManagement");

                if (dt_WIPRecordingForm.Rows.Count > 0) 
                {
                    wipRecordingForm = TranslateDataTableToGetWIPRecordingFormList(dt_WIPRecordingForm);
                   // wipRecordingForm = TranslateDataTableToGetWIPRecordingFormManagementList(dt_WIPRecordingForm);

                    foreach (WIPRecordingForm item in wipRecordingForm)
                    {
                        int WIPFormID = Convert.ToInt32(item.DocNum);
                        //DataTable dt_WIPRecordingFormChild = HANADAL.GetDataTableByStoredProcedure("GetWIPDocumentChildFromSAP", TranslateIDToParameterList(ID), "AssignmentFormManagement");
                        DataTable dt_WIPRecordingFormChild = HANADAL.GetDataTableByStoredProcedure("GetWIPDocumentChildFromSAP", TranslateWIPFormIDToParameterList(WIPFormID), "WIPRecordingFormManagement");

                        if (dt_WIPRecordingFormChild.Rows.Count > 0)
                        {
                            wipRecordingFormChild = TranslateDataTableToGetWIPRecordingChildFormManagementList(dt_WIPRecordingFormChild);                
                        }
                        item.Table = wipRecordingFormChild;
                    }
                }
            }
            
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("WIPRecordingFormManagement", "Exception occured on GetWIPRecordingForm ID: " + ID + " , " + ex.Message);
            }

            return wipRecordingForm;
        }



        public List<WIPRecordingFormChild> GetAssignments(string AsOnDate, int BranchID)
        {
            List<WIPRecordingFormChild> wipRecordingFormChild = new List<WIPRecordingFormChild>();

            try
            {
                HANA_DAL_ODBC HANADAL = new HANA_DAL_ODBC();
                
                    
                DataTable dt_WIPRecordingFormChild = HANADAL.GetDataTableByStoredProcedure("GetAssignmentForWIP", TranslateAssignBranchDateToParameterList(AsOnDate, BranchID), "AssignmentFormManagement");

                if (dt_WIPRecordingFormChild.Rows.Count > 0)
                {
                    wipRecordingFormChild = TranslateDataTableToGetWIPRecordingChildFormManagementList(dt_WIPRecordingFormChild);
                }

            }

            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("WIPRecordingFormManagement", "Exception occured on GetAssignments before this Date: " + AsOnDate + " , " + ex.Message);
            }

            return wipRecordingFormChild;
        }
      
        public string PostInfo(string Json)
        {
            try
            {
                string url = ConfigurationManager.AppSettings["WEBAPIUtility"];
                url = url + "ServiceCalled";
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

        //POST to SAP
        public bool POSTToSBO(out string msg, string DOCNUM, WIPRecordingForm WIPRecordingForm, List<WIPRecordingFormChild> WIPRecordingFormChild, int ID, int UserID)
        {
            bool isSuccess = false;
            msg = "";
            try
            {
                AddUpdateWipRecordingForm(out msg, WIPRecordingForm,  ID,  UserID);

                foreach (var list in WIPRecordingFormChild)
                {
                    WIPRecordingForm.Table= WIPRecordingFormChild;
                }
                var Json = JsonConvert.SerializeObject(WIPRecordingForm);

                string resp = PostInfo(Json);

                if (resp.ToString().Contains("Successfully"))
                {
                    isSuccess = true;
                    msg = resp.ToString();
                }
                else
                {
                    isSuccess = false;
                    msg = resp.ToString();
                }
            }
            catch(Exception ex)
            {
                isSuccess = false;
                msg = ex.ToString();
            }
            return isSuccess;
        }

        private bool AddUpdateWipRecordingForm(out string msg, WIPRecordingForm WIPRecordingForm, int ID, int UserID)
        {
            bool isSuccess = false;
            bool isAddOccured = false;
            bool isUpdateOccured = false;
            bool isDeleteOccured = false;
            Common cmn = new Common();

            msg = "";
            try
            {

                string docNum = GetDocID(WIPRecordingForm.DocNum);
                List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
                B1SP_Parameter parm = new B1SP_Parameter();
                string wipCode = "";
                bool isValidateWipRecordingForm = true;
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
                        DataSet ds = HANADAL.GetDataSetByStoredProcedure("GetWIPRecordingForm", parmList, "WIPRecordingFormManagement");
                        if (ds.Tables.Count > 0)
                        {
                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                foreach (DataRow dtRow in ds.Tables[0].Rows)
                                {
                                    wipCode = Convert.ToString(dtRow["DocNum"]);
                                    if (wipCode.ToLower() == WIPRecordingForm.DocNum.ToLower())
                                        isValidateWipRecordingForm = false;
                                    break;
                                }
                            }
                        }
                    }
                    if (isValidateWipRecordingForm)
                    {
                        parmList = new List<B1SP_Parameter>();
                        parm = new B1SP_Parameter();
                        parm.ParameterName = "DocNum";
                        parm.ParameterValue = Convert.ToString(WIPRecordingForm.DocNum);
                        parmList.Add(parm);

                        DataTable dt = HANADAL.GetDataTableByStoredProcedure("ValidateWIPRecordingForm", parmList, "WIPRecordingFormManagement");
                        if (dt.Rows.Count > 0)
                        {
                            msg = "WIPRecording Form Already Exist!";
                            return false;
                        }
                    }

                    List<WIPRecordingForm> previousData = new List<Models.WIPRecordingForm>();
                    //For Log
                    if (ID > 0)
                    {
                        previousData = GetWIPRecordingForm(ID);
                        if (previousData.Count > 0)
                        {
                            //AddWIPRecordingForm_Log(docNum, WIPRecordingForm, UserID, ID, previousData[0], ID);
                            
                            //if (previousData[0].Table.Count > 0)
                            //{
                            //    AssignmentForm.Table = AssignmentFormChild;
                            //    AddAssignmentFormResource_Log(AssignmentForm.Table, previousData[0].Table, ID);
                            //}

                        }
                        
                    }


                    DataTable dt_AssignmentForm = HANADAL.AddUpdateDataByStoredProcedure("AddUpdateWIPRecordingForm", TranslateWIPRecordingFormToParameterList(docNum, WIPRecordingForm, ID, UserID), "WIPRecordingFormManagement");
                    if (dt_AssignmentForm.Rows.Count == 0)
                        throw new Exception("Exception occured when AddWipRecordingForm,  WIPRecording Code:" + docNum +"" );
                    else
                    {
                        
                    }

                    msg = "Successfully Added/Updated";
                    isSuccess = true;
                    int CreatedBy = Convert.ToInt32(WIPRecordingForm.CreatedBy);
                    Task.Run(() => cmn.AddMasterLog(isAddOccured, isUpdateOccured, isDeleteOccured, nameof(Enums.FormsName.WIPRecordingForm), CreatedBy, "WIPRecordingFormManagement"));
                    //End MAster Log
                    
                }
                catch (Exception ex)
                {
                    msg = "Exception occured in foreach loop Add/Update WIPRecording Form !";
                    isSuccess = false;
                    Log log = new Log();
                    log.LogFile(ex.Message);
                    log.InputOutputDocLog("WIPRecordingFormManagement", "Exception occured in foreach loop AddUpdateWipRecordingForm, " + ex.Message);
                }


            }
            catch (Exception ex)
            {
                msg = "Exception occured in foreach loop Add/Update WipRecording Form!";
                isSuccess = false;
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("WIPRecordingFormManagement", "Exception occured on AddUpdateWipRecordingForm, " + ex.Message);
            }

            return isSuccess;
        }

        public void AddWIPRecordingForm_Log(string DOCNUM, WIPRecordingForm WIPRecordingForm, int UserID, int ID, WIPRecordingForm previousObject, int docID)
        {
            try
            {
                WIPRecordingForm.DocNum = Convert.ToString(DOCNUM);

                List<B1SP_Parameter> paramList = TranslatAddWIPRecordingForm_LogLogToParameterList(WIPRecordingForm, docID);
                bool isChangeOccured = false;
                if (previousObject != null)
                {
                    foreach (PropertyCompareResult resultItem in PropertyCompare.Compare(previousObject, WIPRecordingForm))
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

        private List<B1SP_Parameter> TranslatAddWIPRecordingForm_LogLogToParameterList(WIPRecordingForm obj, int docID)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            //try
            //{
            //    if (obj == null)
            //        return null;

            //    B1SP_Parameter parm = new B1SP_Parameter();

            //    parm = new B1SP_Parameter();
            //    parm.ParameterName = "ID";
            //    parm.ParameterValue = Convert.ToString(0);
            //    parmList.Add(parm);

            //    parm = new B1SP_Parameter();
            //    parm.ParameterName = "DocNum";
            //    parm.ParameterValue = Convert.ToString(obj.DOCNUM);
            //    parmList.Add(parm);

            //    parm = new B1SP_Parameter();
            //    parm.ParameterName = "NonChargeable";

            //    if (obj.NonChargeable == null)
            //        parm.ParameterValue = Convert.ToString(false);
            //    else
            //        parm.ParameterValue = Convert.ToString(obj.NonChargeable);
            //    parmList.Add(parm);

            //    parm = new B1SP_Parameter();
            //    parm.ParameterName = "DocID";
            //    parm.ParameterValue = Convert.ToString(docID);
            //    parmList.Add(parm);


            //    parm = new B1SP_Parameter();
            //    parm.ParameterName = "AssignmentTitle_Previous";
            //    parm.ParameterValue = Convert.ToString(obj.AssignmentTitle);
            //    parmList.Add(parm);


            //    parm = new B1SP_Parameter();
            //    parm.ParameterName = "BranchID_Previous";
            //    parm.ParameterValue = Convert.ToString(obj.BranchID);
            //    parmList.Add(parm);

            //    parm = new B1SP_Parameter();
            //    parm.ParameterName = "ClientID_Previous";
            //    parm.ParameterValue = Convert.ToString(obj.ClientID);
            //    parmList.Add(parm);

            //    parm = new B1SP_Parameter();
            //    parm.ParameterName = "FunctionID_Previous";
            //    parm.ParameterValue = Convert.ToString(obj.FunctionID);
            //    parmList.Add(parm);

            //    parm = new B1SP_Parameter();
            //    parm.ParameterName = "SubFunctionID_Previous";
            //    parm.ParameterValue = Convert.ToString(obj.SubFunctionID);
            //    parmList.Add(parm);


            //    parm = new B1SP_Parameter();
            //    parm.ParameterName = "PartnerID_Previous";
            //    parm.ParameterValue = Convert.ToString(obj.PartnerID);
            //    parmList.Add(parm);

            //    parm = new B1SP_Parameter();
            //    parm.ParameterName = "DirectorID_Previous";
            //    parm.ParameterValue = Convert.ToString(obj.DirectorID);
            //    parmList.Add(parm);

            //    parm = new B1SP_Parameter();
            //    parm.ParameterName = "Status_Previous";
            //    parm.ParameterValue = Convert.ToString(obj.Status);
            //    parmList.Add(parm);

            //    parm = new B1SP_Parameter();
            //    parm.ParameterName = "flgPost_Previous";
            //    parm.ParameterValue = Convert.ToString(obj.flgPost);
            //    parmList.Add(parm);

            //    parm = new B1SP_Parameter();
            //    parm.ParameterName = "IsDeleted_Previous";
            //    if (obj.IsDeleted != null)
            //        parm.ParameterValue = Convert.ToString(obj.IsDeleted);
            //    else
            //        parm.ParameterValue = Convert.ToString(false);
            //    parmList.Add(parm);


            //    parm = new B1SP_Parameter();
            //    parm.ParameterName = "AssignmentTitle_New";
            //    parm.ParameterValue = Convert.ToString(obj.AssignmentTitle);
            //    parmList.Add(parm);

            //    parm = new B1SP_Parameter();
            //    parm.ParameterName = "BranchID_New";
            //    parm.ParameterValue = Convert.ToString(obj.BranchID);
            //    parmList.Add(parm);


            //    parm = new B1SP_Parameter();
            //    parm.ParameterName = "ClientID_New";
            //    parm.ParameterValue = Convert.ToString(obj.ClientID);
            //    parmList.Add(parm);

            //    parm = new B1SP_Parameter();
            //    parm.ParameterName = "FunctionID_New";
            //    parm.ParameterValue = Convert.ToString(obj.FunctionID);
            //    parmList.Add(parm);


            //    parm = new B1SP_Parameter();
            //    parm.ParameterName = "SubFunctionID_New";
            //    parm.ParameterValue = Convert.ToString(obj.SubFunctionID);
            //    parmList.Add(parm);

            //    parm = new B1SP_Parameter();
            //    parm.ParameterName = "PartnerID_New";
            //    parm.ParameterValue = Convert.ToString(obj.PartnerID);
            //    parmList.Add(parm);


            //    parm = new B1SP_Parameter();
            //    parm.ParameterName = "DirectorID_New";
            //    parm.ParameterValue = Convert.ToString(obj.DirectorID);
            //    parmList.Add(parm);

            //    parm = new B1SP_Parameter();
            //    parm.ParameterName = "Status_New";
            //    parm.ParameterValue = Convert.ToString(obj.Status);
            //    parmList.Add(parm);

            //    parm = new B1SP_Parameter();
            //    parm.ParameterName = "flgPost_New";
            //    parm.ParameterValue = Convert.ToString(obj.flgPost);
            //    parmList.Add(parm);

            //    parm = new B1SP_Parameter();
            //    parm.ParameterName = "IsDeleted_New";
            //    if (obj.IsDeleted != null)
            //        parm.ParameterValue = Convert.ToString(obj.IsDeleted);
            //    else
            //        parm.ParameterValue = Convert.ToString(false);
            //    parmList.Add(parm);


            //    parm = new B1SP_Parameter();
            //    parm.ParameterName = "CreatedBy";
            //    parm.ParameterValue = Convert.ToString(obj.CreatedBy);
            //    parmList.Add(parm);

            //    parm = new B1SP_Parameter();
            //    parm.ParameterName = "CreateDate";
            //    parm.ParameterValue = DateTime.Now.ToString("yyyy-MM-dd HH:MM:s");
            //    parmList.Add(parm);
            //}
            //catch (Exception ex)
            //{
            //    Log log = new Log();
            //    log.LogFile(ex.Message);
            //    log.InputOutputDocLog("AssignmentFormManagement", "Exception occured on TranslatAssignmentFormLogToParameterList, " + ex.Message);
            //}


            return parmList;
        }

        private List<B1SP_Parameter> TranslateWIPRecordingFormToParameterList(string DOCNUM, WIPRecordingForm WIPRecordingForm, int ID, int UserID)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {
                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "ID";
                parm.ParameterValue = Convert.ToString(ID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "AsOnDate";
                parm.ParameterValue = Convert.ToString(Convert.ToDateTime(WIPRecordingForm.AsOnDate).ToString("yyyy-MM-dd HH:MM:s")); //DateTime.Now.ToString("yyyy-MM-dd HH:MM:s");
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "DebitAccount";
                parm.ParameterValue = Convert.ToString(WIPRecordingForm.DebitAccount);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CreditAccount";
                parm.ParameterValue = Convert.ToString(WIPRecordingForm.CreditAccount);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ReverseDate";
                parm.ParameterValue = Convert.ToString(Convert.ToDateTime(WIPRecordingForm.ReversalDate).ToString("yyyy-MM-dd HH:MM:s")); 
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "DocNum";
                parm.ParameterValue = Convert.ToString(DOCNUM);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "DocDate";
                parm.ParameterValue = Convert.ToString(Convert.ToDateTime(WIPRecordingForm.DocDate).ToString("yyyy-MM-dd HH:MM:s")); //DateTime.Now.ToString("yyyy-MM-dd HH:MM:s");
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "Year";
                parm.ParameterValue = Convert.ToString(WIPRecordingForm.Year);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "Month";
                parm.ParameterValue = Convert.ToString(WIPRecordingForm.Month);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "BranchID";
                parm.ParameterValue = Convert.ToString(WIPRecordingForm.BranchID); 
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CurrencyID";
                parm.ParameterValue = Convert.ToString(WIPRecordingForm.CurrencyID);
                parmList.Add(parm);
                
                parm = new B1SP_Parameter();
                parm.ParameterName = "TotalWIP";
                parm.ParameterValue = Convert.ToString(WIPRecordingForm.WipTotal);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "ReverseJE";
                parm.ParameterValue = Convert.ToString(WIPRecordingForm.ReverseJE);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "IsDeleted";
                if (WIPRecordingForm.IsDeleted == null)
                    WIPRecordingForm.IsDeleted = false;
                parm.ParameterValue = Convert.ToString(WIPRecordingForm.IsDeleted);
                parmList.Add(parm);
                
                parm = new B1SP_Parameter();
                parm.ParameterName = "CreatedBy";
                parm.ParameterValue = Convert.ToString(UserID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "CreatedDate";
                parm.ParameterValue = DateTime.Now.ToString("yyyy-MM-dd HH:MM:s");
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "UpdatedBy";
                parm.ParameterValue = Convert.ToString(UserID);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "UpdatedDate";
                parm.ParameterValue = DateTime.Now.ToString("yyyy-MM-dd HH:MM:s");
                parmList.Add(parm);

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("WIPRecordingFormManagement", "Exception occured on TranslateWIPRecordingFormToParameterList, " + ex.Message);
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
                log.InputOutputDocLog("WIPRecordingFormManagement", "Exception occured on TranslateIDToParameterList, " + ex.Message);
            }
            return parmList;
        }

        private List<B1SP_Parameter> TranslateWIPFormIDToParameterList(int Id)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {
                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "WIPFormID";
                parm.ParameterValue = Convert.ToString(Id);
                parmList.Add(parm);
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("WIPRecordingFormManagement", "Exception occured on TranslateIDToParameterList, " + ex.Message);
            }
            return parmList;
        }

        private List<B1SP_Parameter> TranslateAssignBranchDateToParameterList(string AsOnDate, int BranchID)
        {
            List<B1SP_Parameter> parmList = new List<B1SP_Parameter>();
            try
            {
                B1SP_Parameter parm = new B1SP_Parameter();
                parm.ParameterName = "AsOnDate";
                parm.ParameterValue = Convert.ToString(AsOnDate);
                parmList.Add(parm);

                parm = new B1SP_Parameter();
                parm.ParameterName = "BranchID";
                parm.ParameterValue = Convert.ToString(BranchID);
                parmList.Add(parm);

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("WIPRecordingFormManagement", "Exception occured on TranslateAssignIDDateToParameterList, " + ex.Message);
            }
            return parmList;
        }

        private List<WIPRecordingForm> TranslateDataTableToGetWIPRecordingFormManagementList(DataTable dt)
        {
            List<WIPRecordingForm> wipRecordingFormL = new List<WIPRecordingForm>();

            try
            {
                int sno = 1;
                foreach (DataRow dtRow in dt.Rows)
                {
                    WIPRecordingForm wipRecordingForm = new WIPRecordingForm();
                    wipRecordingForm.SNO = sno;
                    wipRecordingForm.KEY = Guid.NewGuid().ToString();
                    //wipRecordingForm.ID = Convert.ToInt32(dtRow["ID"]);
                    wipRecordingForm.DocNum = Convert.ToString(dtRow["DocNum"]);
                    wipRecordingForm.DocDate = Convert.ToString(dtRow["DocDate"]);

                    wipRecordingForm.DebitAccount = Convert.ToString(dtRow["DebitAccount"]);
                    wipRecordingForm.CreditAccount = Convert.ToString(dtRow["CreditAccount"]);
                    //wipRecordingForm.ReversalDate = Convert.ToString(dtRow["ReversalDate"]);
                    //wipRecordingForm.Year = Convert.ToString(dtRow["Year"]);
                    //wipRecordingForm.Month = Convert.ToString(dtRow["Month"]);
                    wipRecordingForm.BranchID = Convert.ToInt32(dtRow["BranchID"]);
                    wipRecordingForm.WipTotal = Convert.ToInt32(dtRow["LocTotal"]);
                    string boolReverse = Convert.ToString(dtRow["ReverseJE"]);
                    if (boolReverse == "N")
                        wipRecordingForm.ReverseJE = Convert.ToBoolean(false);

                    else
                        wipRecordingForm.ReverseJE = Convert.ToBoolean(true);


                    //wipRecordingForm.Posted = Convert.ToBoolean(dtRow["Posted"]);
                    //wipRecordingForm.CreatedBy = Convert.ToInt32(dtRow["CreatedBy"]);
                    //wipRecordingForm.CreateDate = Convert.ToDateTime(dtRow["CreateDate"]);

                    //if (dtRow["UpdatedBy"] != DBNull.Value)
                    //    wipRecordingForm.UpdatedBy = Convert.ToInt32(dtRow["UpdatedBy"]);
                    //if (dtRow["UpdateDate"] != DBNull.Value)
                    //    wipRecordingForm.UpdateDate = Convert.ToDateTime(dtRow["UpdateDate"]);

                    sno = sno + 1;
                    wipRecordingFormL.Add(wipRecordingForm);
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("WIPRecordingFormManagement", "Exception occured on TranslateDataTableToGetWIPRecordingFormManagementList, " + ex.Message);
            }

            return wipRecordingFormL;
        }

        private List<WIPRecordingForm> TranslateDataTableToGetWIPRecordingFormList(DataTable dt)
        {
            List<WIPRecordingForm> wipRecordingFormL = new List<WIPRecordingForm>();

            try
            {
                int sno = 1;
                foreach (DataRow dtRow in dt.Rows)
                {
                    WIPRecordingForm wipRecordingForm = new WIPRecordingForm();
                    wipRecordingForm.SNO = sno;
                    wipRecordingForm.KEY = Guid.NewGuid().ToString();
                    wipRecordingForm.ID = Convert.ToInt32(dtRow["ID"]);
                    wipRecordingForm.AsOnDate = Convert.ToString(dtRow["AsOnDate"]);
                    wipRecordingForm.DebitAccount = Convert.ToString(dtRow["DebitAccount"]);
                    wipRecordingForm.CreditAccount = Convert.ToString(dtRow["CreditAccount"]);
                    wipRecordingForm.ReversalDate = Convert.ToString(dtRow["ReverseDate"]);
                    wipRecordingForm.DocNum = Convert.ToString(dtRow["DocNum"]);
                    wipRecordingForm.DocDate = Convert.ToString(dtRow["DocDate"]);
                    wipRecordingForm.Month = Convert.ToString(dtRow["Month"]);
                    wipRecordingForm.Year = Convert.ToString(dtRow["Year"]);
                    wipRecordingForm.BranchID = Convert.ToInt32(dtRow["BranchID"]);
                    wipRecordingForm.CurrencyID = Convert.ToString(dtRow["CurrencyID"]);
                    wipRecordingForm.WipTotal = Convert.ToInt32(dtRow["TotalWIP"]);
                    string boolReverse = Convert.ToString(dtRow["ReverseJE"]);
                    if (boolReverse == "0")
                        wipRecordingForm.ReverseJE = Convert.ToBoolean(false);

                    else
                        wipRecordingForm.ReverseJE = Convert.ToBoolean(true);


                    //wipRecordingForm.Posted = Convert.ToBoolean(dtRow["Posted"]);
                    //wipRecordingForm.CreatedBy = Convert.ToInt32(dtRow["CreatedBy"]);
                    //wipRecordingForm.CreateDate = Convert.ToDateTime(dtRow["CreateDate"]);

                    //if (dtRow["UpdatedBy"] != DBNull.Value)
                    //    wipRecordingForm.UpdatedBy = Convert.ToInt32(dtRow["UpdatedBy"]);
                    //if (dtRow["UpdateDate"] != DBNull.Value)
                    //    wipRecordingForm.UpdateDate = Convert.ToDateTime(dtRow["UpdateDate"]);

                    sno = sno + 1;
                    wipRecordingFormL.Add(wipRecordingForm);
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("WIPRecordingFormManagement", "Exception occured on TranslateDataTableToGetWIPRecordingFormManagementList, " + ex.Message);
            }

            return wipRecordingFormL;
        }

        private List<WIPRecordingFormChild> TranslateDataTableToGetWIPRecordingChildFormManagementList(DataTable dt)
        {
            List<WIPRecordingFormChild> wipRecordingFormL = new List<WIPRecordingFormChild>();

            try
            {
                int sno = 1;
                foreach (DataRow dtRow in dt.Rows)
                {
                    WIPRecordingForm wipRecordingFormDoc = new WIPRecordingForm();

                    WIPRecordingFormChild wipRecordingForm = new WIPRecordingFormChild();
                    wipRecordingForm.SNO = sno;
                    wipRecordingForm.KEY = Guid.NewGuid().ToString();
                    // wipRecordingForm = Convert.ToString(dtRow["ClientID"]);

                    //if(Convert.ToDouble(dtRow["Debit"]) > 0)
                    //{
                    //    wipRecordingFormDoc.DebitAccount = Convert.ToString(dtRow["ShortName"]);
                    //}
                    //else
                    //{
                    //    wipRecordingFormDoc.CreditAccount = Convert.ToString(dtRow["ShortName"]);
                    //}
                    wipRecordingForm.ClientID = Convert.ToString(dtRow["ClientID"]);
                    wipRecordingForm.ClientName = Convert.ToString(dtRow["ClientName"]);
                    wipRecordingForm.AssignmentCode = Convert.ToString(dtRow["AssignmentCode"]);
                    wipRecordingForm.AssignmentTitle = Convert.ToString(dtRow["AssignmentTitle"]);
                    wipRecordingForm.PartnerID = Convert.ToString(dtRow["PartnerID"]);
                    wipRecordingForm.PartnerName = Convert.ToString(dtRow["PartnerName"]);
                    wipRecordingForm.FunctionID = Convert.ToString(dtRow["FunctionID"]);
                    wipRecordingForm.FunctionName = Convert.ToString(dtRow["FunctionName"]);
                    wipRecordingForm.SubFunctionID = Convert.ToString(dtRow["SubFunctionID"]);
                    wipRecordingForm.SubFunctionName = Convert.ToString(dtRow["SubFunctionName"]);
                    wipRecordingForm.BilledToDate = Convert.ToDouble(dtRow["BilledToDate"]);
                    
                    wipRecordingForm.WIPAmountToDate = Convert.ToDouble(string.Format("{0:0.00}", Convert.ToDouble(dtRow["WIPAmountToDate"])));

                    double wipNotCharged = wipRecordingForm.WIPAmountToDate - wipRecordingForm.BilledToDate;
                    wipNotCharged =   Convert.ToDouble( string.Format("{0:0.00}", wipNotCharged) ) ;
                    wipRecordingForm.WIPNotCharged = wipNotCharged;
                    if(wipNotCharged < 0)
                    {
                        wipRecordingForm.WIPToBeCharged = 0.00;
                    }
                    else
                    {
                        wipRecordingForm.WIPToBeCharged = wipNotCharged;
                    }
                    //wipRecordingForm.CreatedBy = Convert.ToInt32(dtRow["CreatedBy"]);
                    //wipRecordingForm.CreateDate = Convert.ToDateTime(dtRow["CreateDate"]);

                    //if (dtRow["UpdatedBy"] != DBNull.Value)
                    //    wipRecordingForm.UpdatedBy = Convert.ToInt32(dtRow["UpdatedBy"]);
                    //if (dtRow["UpdateDate"] != DBNull.Value)
                    //    wipRecordingForm.UpdateDate = Convert.ToDateTime(dtRow["UpdateDate"]);

                    sno = sno + 1;
                    wipRecordingFormL.Add(wipRecordingForm);
                }
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("WIPRecordingFormManagement", "Exception occured on TranslateDataTableToGetWIPRecordingFormManagementList, " + ex.Message);
            }

            return wipRecordingFormL;
        }

    }
}