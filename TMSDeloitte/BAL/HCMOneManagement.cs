using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using TMSDeloitte.DAL;
using TMSDeloitte.Helper;
using TMSDeloitte.Models;

namespace TMSDeloitte.BAL
{
    public class HCMOneManagement
    {

        public List<Designation> GetAllHCMDesignation()
        {
            Log log = new Log();
            List<Designation> list = new List<Designation>();
            try
            {
                log.HCMOneLog("Getting Data From Sp: spGetAllDesignation");
                SQL_DAL sqlDAL = new SQL_DAL();
                SqlParameter[] parameters = new SqlParameter[] { };
                DataSet ds = sqlDAL.GetDataSet("spGetAllDesignation", parameters);

                if (ds.Tables.Count > 0)
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        log.HCMOneLog("Getting Data From Sp: spGetAllDesignation Count:" + ds.Tables[0].Rows.Count);

                        foreach (DataRow row in ds.Tables[0].Rows)
                        {
                            Designation desig = new Designation();
                            desig.CreateDate = Convert.ToDateTime(row["CreateDate"] == DBNull.Value ? DateTime.Now : row["CreateDate"]);
                            desig.Description = Convert.ToString(row["Description"]);
                            desig.flgActive = Convert.ToBoolean(row["flgActive"] == DBNull.Value ? false : row["flgActive"]);
                            desig.GradeID = Convert.ToInt32(row["GradeID"] == DBNull.Value ? 0 : row["GradeID"]);
                            desig.Id = Convert.ToInt32(row["Id"] == DBNull.Value ? 0 : row["Id"]);
                            desig.Name = Convert.ToString(row["Name"]);
                            desig.UpdateDate = Convert.ToDateTime(row["UpdateDate"] == DBNull.Value ? DateTime.Now : row["UpdateDate"]);
                            desig.UpdatedBy = Convert.ToString(row["UpdatedBy"]);
                            desig.UserId = Convert.ToString(row["UserId"]);
                            list.Add(desig);
                        }
                    }

                log.HCMOneLog("Output: " + Environment.NewLine + JsonConvert.SerializeObject(list));
            }
            catch (Exception ex)
            {

                log.HCMOneLog("Exception occured in GetAllDesignation : " + ex.Message);
            }
            return list;
        }
        public List<Designation> GetAllHCMDesignation(List<int> DesignationList)
        {
            Log log = new Log();
            List<Designation> list = new List<Designation>();
            try
            {
                log.HCMOneLog("Getting Data From Sp: spGetAllDesignation");
                SQL_DAL sqlDAL = new SQL_DAL();
                SqlParameter[] parameters = new SqlParameter[] { };
                DataSet ds = sqlDAL.GetDataSet("spGetAllDesignation", parameters);

                if (ds.Tables.Count > 0)
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        log.HCMOneLog("Getting Data From Sp: spGetAllDesignation Count:" + ds.Tables[0].Rows.Count);

                        foreach (DataRow row in ds.Tables[0].Rows)
                        {
                            Designation desig = new Designation();
                            desig.CreateDate = Convert.ToDateTime(row["CreateDate"] == DBNull.Value ? DateTime.Now : row["CreateDate"]);
                            desig.Description = Convert.ToString(row["Description"]);
                            desig.flgActive = Convert.ToBoolean(row["flgActive"] == DBNull.Value ? false : row["flgActive"]);
                            desig.GradeID = Convert.ToInt32(row["GradeID"] == DBNull.Value ? 0 : row["GradeID"]);
                            desig.Id = Convert.ToInt32(row["Id"] == DBNull.Value ? 0 : row["Id"]);
                            desig.Name = Convert.ToString(row["Name"]);
                            desig.UpdateDate = Convert.ToDateTime(row["UpdateDate"] == DBNull.Value ? DateTime.Now : row["UpdateDate"]);
                            desig.UpdatedBy = Convert.ToString(row["UpdatedBy"]);
                            desig.UserId = Convert.ToString(row["UserId"]);
                            if (DesignationList.Contains(desig.Id))
                            {
                                list.Add(desig);
                            }
                        }
                    }

                log.HCMOneLog("Output: " + Environment.NewLine + JsonConvert.SerializeObject(list));
            }
            catch (Exception ex)
            {

                log.HCMOneLog("Exception occured in GetAllDesignation : " + ex.Message);
            }
            return list;
        }

        public List<Director> GetAllHCMDirector()
        {
            Log log = new Log();
            List<Director> list = new List<Director>();
            try
            {
                log.HCMOneLog("Getting Data From Sp: spGetAllDirector");
                SQL_DAL sqlDAL = new SQL_DAL();
                SqlParameter[] parameters = new SqlParameter[] { };
                DataSet ds = sqlDAL.GetDataSet("spGetAllDirector", parameters);

                if (ds.Tables.Count > 0)
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        log.HCMOneLog("Getting Data From Sp: spGetAllDirector count: " + ds.Tables[0].Rows.Count);

                        foreach (DataRow row in ds.Tables[0].Rows)
                        {
                            Director direc = new Director();
                            direc.DirectorID = Convert.ToInt32(row["DirectorID"] == DBNull.Value ? 0 : row["DirectorID"]);
                            direc.DirectorCode = Convert.ToString(row["DirectorCode"]);
                            direc.DirectorName = Convert.ToString(row["DirectorName"]);
                            direc.DepartmentName = Convert.ToString(row["DepartmentName"]);
                            list.Add(direc);
                        }
                    }

                log.HCMOneLog("Output: " + Environment.NewLine + JsonConvert.SerializeObject(list));
            }
            catch (Exception ex)
            {

                log.HCMOneLog("Exception occured in spGetAllDirector : " + ex.Message);
            }
            return list;
        }

        public List<Department> GetAllHCMDepartment()
        {
            Log log = new Log();
            List<Department> list = new List<Department>();
            try
            {
                log.HCMOneLog("Getting Data From Sp: spGetAllDepartment");
                SQL_DAL sqlDAL = new SQL_DAL();
                SqlParameter[] parameters = new SqlParameter[] { };
                DataSet ds = sqlDAL.GetDataSet("spGetAllDepartment", parameters);

                if (ds.Tables.Count > 0)
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        log.HCMOneLog("Getting Data From Sp: spGetAllDepartment count: " + ds.Tables[0].Rows.Count);
                        foreach (DataRow row in ds.Tables[0].Rows)
                        {
                            Department desig = new Department();
                            desig.CreateDate = Convert.ToDateTime(row["CreateDate"] == DBNull.Value ? DateTime.Now : row["CreateDate"]);
                            desig.flgActive = Convert.ToBoolean(row["flgActive"] == DBNull.Value ? false : row["flgActive"]);
                            desig.Id = Convert.ToInt32(row["ID"] == DBNull.Value ? 0 : row["ID"]);
                            desig.DeptLevel = Convert.ToInt32(row["DeptLevel"] == DBNull.Value ? 0 : row["DeptLevel"]);
                            desig.Name = Convert.ToString(row["DeptName"]);
                            desig.Code = Convert.ToString(row["Code"]);
                            desig.UpdateDate = Convert.ToDateTime(row["UpdateDate"] == DBNull.Value ? DateTime.Now : row["UpdateDate"]);
                            desig.UpdatedBy = Convert.ToString(row["UpdatedBy"]);
                            desig.UserId = Convert.ToString(row["UserId"]);
                            list.Add(desig);
                        }
                    }

                log.HCMOneLog("Output: " + Environment.NewLine + JsonConvert.SerializeObject(list));
            }
            catch (Exception ex)
            {

                log.HCMOneLog("Exception occured in GetAllDesignation : " + ex.Message);
            }
            return list;
        }

        public List<Department> GetAllFilteredHCMDepartment(List<int> DepartmentList)
        {
            Log log = new Log();
            List<Department> list = new List<Department>();
            try
            {
                log.HCMOneLog("Getting Data From Sp: spGetAllDepartment");
                SQL_DAL sqlDAL = new SQL_DAL();
                SqlParameter[] parameters = new SqlParameter[] { };
                DataSet ds = sqlDAL.GetDataSet("spGetAllDepartment", parameters);

                if (ds.Tables.Count > 0)
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        log.HCMOneLog("Getting Data From Sp: spGetAllDepartment count: " + ds.Tables[0].Rows.Count);
                        foreach (DataRow row in ds.Tables[0].Rows)
                        {

                            Department desig = new Department();
                            desig.CreateDate = Convert.ToDateTime(row["CreateDate"] == DBNull.Value ? DateTime.Now : row["CreateDate"]);
                            desig.flgActive = Convert.ToBoolean(row["flgActive"] == DBNull.Value ? false : row["flgActive"]);
                            desig.Id = Convert.ToInt32(row["ID"] == DBNull.Value ? 0 : row["ID"]);
                            desig.DeptLevel = Convert.ToInt32(row["DeptLevel"] == DBNull.Value ? 0 : row["DeptLevel"]);
                            desig.Name = Convert.ToString(row["DeptName"]);
                            desig.Code = Convert.ToString(row["Code"]);
                            desig.UpdateDate = Convert.ToDateTime(row["UpdateDate"] == DBNull.Value ? DateTime.Now : row["UpdateDate"]);
                            desig.UpdatedBy = Convert.ToString(row["UpdatedBy"]);
                            desig.UserId = Convert.ToString(row["UserId"]);
                            if (DepartmentList.Contains(desig.Id))
                            {
                                list.Add(desig);
                            }
                        }
                    }

                log.HCMOneLog("Output: " + Environment.NewLine + JsonConvert.SerializeObject(list));
            }
            catch (Exception ex)
            {

                log.HCMOneLog("Exception occured in GetAllDesignation : " + ex.Message);
            }
            return list;
        }
        public List<Location> GetAllHCMLocation()
        {
            Log log = new Log();
            List<Location> list = new List<Location>();
            try
            {
                log.HCMOneLog("Getting Data From Sp: spGetAllLocation");
                SQL_DAL sqlDAL = new SQL_DAL();
                SqlParameter[] parameters = new SqlParameter[] { };
                DataSet ds = sqlDAL.GetDataSet("spGetAllLocation", parameters);

                if (ds.Tables.Count > 0)
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        log.HCMOneLog("Getting Data From Sp: spGetAllLocation count" + ds.Tables[0].Rows.Count);
                        foreach (DataRow row in ds.Tables[0].Rows)
                        {
                            Location desig = new Location();
                            desig.CreateDate = Convert.ToDateTime(row["CreateDate"] == DBNull.Value ? DateTime.Now : row["CreateDate"]);
                            desig.Description = Convert.ToString(row["Description"]);
                            desig.flgActive = Convert.ToBoolean(row["flgActive"] == DBNull.Value ? false : row["flgActive"]);
                            desig.Id = Convert.ToInt32(row["Id"] == DBNull.Value ? 0 : row["Id"]);
                            desig.Name = Convert.ToString(row["Name"]);
                            desig.UpdateDate = Convert.ToDateTime(row["UpdateDate"] == DBNull.Value ? DateTime.Now : row["UpdateDate"]);
                            desig.UpdatedBy = Convert.ToString(row["UpdatedBy"]);
                            desig.UserId = Convert.ToString(row["UserId"]);
                            list.Add(desig);
                        }
                    }

                log.HCMOneLog("Output: " + Environment.NewLine + JsonConvert.SerializeObject(list));
            }
            catch (Exception ex)
            {

                log.HCMOneLog("Exception occured in GetAllDesignation : " + ex.Message);
            }
            return list;
        }

        public List<Branch> GetAllHCMBranch()
        {
            Log log = new Log();
            List<Branch> list = new List<Branch>();
            try
            {
                log.HCMOneLog("Getting Data From Sp: spGetAllDesignation");
                SQL_DAL sqlDAL = new SQL_DAL();
                SqlParameter[] parameters = new SqlParameter[] { };
                DataSet ds = sqlDAL.GetDataSet("spGetAllBranch", parameters);

                if (ds.Tables.Count > 0)
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        log.HCMOneLog("Getting Data From Sp: spGetAllDesignation count" + ds.Tables[0].Rows.Count);
                        foreach (DataRow row in ds.Tables[0].Rows)
                        {

                            Branch obj = new Branch();
                            obj.Id = Convert.ToInt32(row["Id"] == DBNull.Value ? 0 : row["Id"]);
                            obj.Name = Convert.ToString(row["Name"]);
                            list.Add(obj);
                        }
                    }

                log.HCMOneLog("Output: " + Environment.NewLine + JsonConvert.SerializeObject(list));
            }
            catch (Exception ex)
            {

                log.HCMOneLog("Exception occured in GetAllHCMBranch : " + ex.Message);
            }
            return list;
        }

        public List<Branch> GetAllFilteredHCMBranch(List<int> BranchList)
        {
            Log log = new Log();
            List<Branch> list = new List<Branch>();
            try
            {
                log.HCMOneLog("Getting Data From Sp: spGetAllBranch");

                SQL_DAL sqlDAL = new SQL_DAL();
                SqlParameter[] parameters = new SqlParameter[] { };
                DataSet ds = sqlDAL.GetDataSet("spGetAllBranch", parameters);

                if (ds.Tables.Count > 0)
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        log.HCMOneLog("Getting Data From Sp: spGetAllBranch count" + ds.Tables[0].Rows.Count);
                        foreach (DataRow row in ds.Tables[0].Rows)
                        {

                            Branch obj = new Branch();
                            obj.Id = Convert.ToInt32(row["Id"] == DBNull.Value ? 0 : row["Id"]);
                            obj.Name = Convert.ToString(row["Name"]);

                            if (BranchList.Contains(obj.Id))
                            {
                                list.Add(obj);
                            }

                        }
                    }

                log.HCMOneLog("Output: " + Environment.NewLine + JsonConvert.SerializeObject(list));
            }
            catch (Exception ex)
            {

                log.HCMOneLog("Exception occured in GetAllHCMBranch : " + ex.Message);
            }
            return list;
        }

        public List<EmployeeLeaves> GetAllHCMEmployeeLeaves()
        {
            Log log = new Log();
            List<EmployeeLeaves> list = new List<EmployeeLeaves>();
            try
            {
                log.HCMOneLog("Getting Data From Sp: spGetAllLocation");

                SQL_DAL sqlDAL = new SQL_DAL();
                SqlParameter[] parameters = new SqlParameter[] { };
                DataSet ds = sqlDAL.GetDataSet("spGetAllLocation", parameters);

                if (ds.Tables.Count > 0)
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        log.HCMOneLog("Getting Data From Sp: spGetAllLocation count" + ds.Tables[0].Rows.Count);
                        foreach (DataRow row in ds.Tables[0].Rows)
                        {
                            EmployeeLeaves obj = new EmployeeLeaves();
                            obj.ID = Convert.ToInt32(row["Id"] == DBNull.Value ? 0 : row["Id"]);
                            obj.Name = Convert.ToString(row["Name"] == DBNull.Value ? "" : row["Name"]);
                            list.Add(obj);
                        }
                    }

                log.HCMOneLog("Output: " + Environment.NewLine + JsonConvert.SerializeObject(list));
            }
            catch (Exception ex)
            {
                log.HCMOneLog("Exception occured in GetAllHCMEmployeeLeaves : " + ex.Message);
            }
            return list;
        }

        public List<HCM_Employee> GetAllHCMUser()
        {
            Log log = new Log();
            List<HCM_Employee> list = new List<HCM_Employee>();
            try
            {
                log.HCMOneLog("Getting Data From Sp: spGetAllEmp");

                SQL_DAL sqlDAL = new SQL_DAL();
                SqlParameter[] parameters = new SqlParameter[] { };
                DataSet ds = sqlDAL.GetDataSet("spGetAllEmp", parameters);

                if (ds.Tables.Count > 0)
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        log.HCMOneLog("Getting Data From Sp: spGetAllEmp count" + ds.Tables[0].Rows.Count);
                        foreach (DataRow row in ds.Tables[0].Rows)
                        {
                            HCM_Employee emp = new HCM_Employee();
                            emp.BranchID = Convert.ToInt32(row["BranchID"] == DBNull.Value ? 0 : row["BranchID"]);
                            emp.BranchName = Convert.ToString(row["BranchName"] == DBNull.Value ? "" : row["BranchName"]);
                            emp.DepartmentID = Convert.ToInt32(row["DepartmentID"] == DBNull.Value ? 0 : row["DepartmentID"]);
                            emp.DepartmentName = Convert.ToString(row["DepartmentName"] == DBNull.Value ? "" : row["DepartmentName"]);
                            emp.DesignationID = Convert.ToInt32(row["DesignationID"] == DBNull.Value ? 0 : row["DesignationID"]);
                            emp.DesignationName = Convert.ToString(row["DesignationName"] == DBNull.Value ? "" : row["DesignationName"]);
                            emp.EmpID = Convert.ToString(row["EmpID"] == DBNull.Value ? "" : row["EmpID"]);
                            emp.Fax = Convert.ToString(row["Fax"] == DBNull.Value ? "" : row["Fax"]);
                            emp.EmpName = Convert.ToString(row["EmpName"] == DBNull.Value ? "" : row["EmpName"]);
                            emp.FirstName = Convert.ToString(row["FirstName"] == DBNull.Value ? "" : row["FirstName"]);
                            emp.ID = Convert.ToInt32(row["ID"] == DBNull.Value ? "" : row["ID"]);
                            emp.LastName = Convert.ToString(row["LastName"] == DBNull.Value ? "" : row["LastName"]);
                            emp.LocationID = Convert.ToInt32(row["LocationID"] == DBNull.Value ? 0 : row["LocationID"]);
                            emp.LocationName = Convert.ToString(row["LocationName"] == DBNull.Value ? "" : row["LocationName"]);
                            emp.MiddleName = Convert.ToString(row["MiddleName"] == DBNull.Value ? "" : row["MiddleName"]);
                            emp.OfficeEmail = Convert.ToString(row["OfficeEmail"] == DBNull.Value ? "" : row["OfficeEmail"]);
                            emp.Dimension2 = Convert.ToString(row["Dimension2"] == DBNull.Value ? "" : row["Dimension2"]);
                            //emp.UserCode = Convert.ToString(row["UserCode"] == DBNull.Value ? "" : row["UserCode"]);
                            //emp.PassCode = Convert.ToString(row["PassCode"] == DBNull.Value ? "" : row["PassCode"]);
                            emp.EmpName = emp.FirstName + " " + emp.MiddleName + " " + emp.LastName;
                            emp.EmpDetailName = emp.EmpID + " , " + emp.FirstName + " " + emp.MiddleName + " " + emp.LastName + " , " + emp.BranchName + " , " + emp.DepartmentName + " , " + emp.DesignationName;
                            list.Add(emp);
                        }
                    }


            }
            catch (Exception ex)
            {

                log.HCMOneLog("Exception occured in GetAllHCMUser : " + ex.Message);
            }
            return list;
        }

        public HCM_Employee GetHCMUserByID(int id)
        {
            Log log = new Log();
            HCM_Employee emp = new HCM_Employee();
            try
            {
                log.HCMOneLog("Getting Data From Sp: spGetEmpByID");

                SQL_DAL sqlDAL = new SQL_DAL();
                SqlParameter[] parameters = new SqlParameter[] {
                new SqlParameter("@id", SqlDbType.Int)
                };

                DataSet ds = sqlDAL.GetDataSet("spGetEmpByID", parameters);

                if (ds.Tables.Count > 0)
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        log.HCMOneLog("Getting Data From Sp: spGetEmpByID count" + ds.Tables[0].Rows.Count);

                        foreach (DataRow row in ds.Tables[0].Rows)
                        {

                            emp.BranchID = Convert.ToInt32(row["BranchID"] == DBNull.Value ? 0 : row["BranchID"]);
                            emp.BranchName = Convert.ToString(row["BranchName"] == DBNull.Value ? "" : row["BranchName"]);
                            emp.DepartmentID = Convert.ToInt32(row["DepartmentID"] == DBNull.Value ? 0 : row["DepartmentID"]);
                            emp.DepartmentName = Convert.ToString(row["DepartmentName"] == DBNull.Value ? "" : row["DepartmentName"]);
                            emp.DesignationID = Convert.ToInt32(row["DesignationID"] == DBNull.Value ? 0 : row["DesignationID"]);
                            emp.DesignationName = Convert.ToString(row["DesignationName"] == DBNull.Value ? "" : row["DesignationName"]);
                            emp.EmpID = Convert.ToString(row["EmpID"] == DBNull.Value ? "" : row["EmpID"]);
                            emp.Fax = Convert.ToString(row["Fax"] == DBNull.Value ? "" : row["Fax"]);
                            emp.EmpName = Convert.ToString(row["EmpName"] == DBNull.Value ? "" : row["EmpName"]);
                            emp.FirstName = Convert.ToString(row["FirstName"] == DBNull.Value ? "" : row["FirstName"]);
                            emp.ID = Convert.ToInt32(row["ID"] == DBNull.Value ? "" : row["ID"]);
                            emp.LastName = Convert.ToString(row["LastName"] == DBNull.Value ? "" : row["LastName"]);
                            emp.LocationID = Convert.ToInt32(row["LocationID"] == DBNull.Value ? 0 : row["LocationID"]);
                            emp.LocationName = Convert.ToString(row["LocationName"] == DBNull.Value ? "" : row["LocationName"]);
                            emp.MiddleName = Convert.ToString(row["MiddleName"] == DBNull.Value ? "" : row["MiddleName"]);
                            emp.OfficeEmail = Convert.ToString(row["OfficeEmail"] == DBNull.Value ? "" : row["OfficeEmail"]);
                            emp.Dimension2 = Convert.ToString(row["Dimension2"] == DBNull.Value ? "" : row["Dimension2"]);
                            emp.UserCode = Convert.ToString(row["UserCode"] == DBNull.Value ? "" : row["UserCode"]);
                            emp.PassCode = Convert.ToString(row["PassCode"] == DBNull.Value ? "" : row["PassCode"]);
                            emp.EmpName = emp.FirstName + " " + emp.MiddleName + " " + emp.LastName;
                            emp.EmpDetailName = emp.EmpID + " , " + emp.FirstName + " " + emp.MiddleName + " " + emp.LastName + " , " + emp.BranchName + " , " + emp.DepartmentName + " , " + emp.DesignationName;

                        }
                    }


            }
            catch (Exception ex)
            {

                log.HCMOneLog("Exception occured in GetHCMUserByID : " + ex.Message);
            }
            return emp;
        }

        public HCM_Employee GetHCMUserByCode(string code)
        {
            Log log = new Log();
            HCM_Employee emp = new HCM_Employee();
            try
            {
                log.HCMOneLog("Getting Data From Sp: spGetEmpByCode");

                SQL_DAL sqlDAL = new SQL_DAL();
                SqlParameter[] parameters = new SqlParameter[] {
                new SqlParameter("@code", code)
                };

                DataSet ds = sqlDAL.GetDataSet("spGetEmpByCode", parameters);

                if (ds.Tables.Count > 0)
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        log.HCMOneLog("Getting Data From Sp: spGetEmpByCode count" + ds.Tables[0].Rows.Count);
                        foreach (DataRow row in ds.Tables[0].Rows)
                        {

                            emp.BranchID = Convert.ToInt32(row["BranchID"] == DBNull.Value ? 0 : row["BranchID"]);
                            emp.BranchName = Convert.ToString(row["BranchName"] == DBNull.Value ? "" : row["BranchName"]);
                            emp.DepartmentID = Convert.ToInt32(row["DepartmentID"] == DBNull.Value ? 0 : row["DepartmentID"]);
                            emp.DepartmentName = Convert.ToString(row["DepartmentName"] == DBNull.Value ? "" : row["DepartmentName"]);
                            emp.DesignationID = Convert.ToInt32(row["DesignationID"] == DBNull.Value ? 0 : row["DesignationID"]);
                            emp.DesignationName = Convert.ToString(row["DesignationName"] == DBNull.Value ? "" : row["DesignationName"]);
                            emp.EmpID = Convert.ToString(row["EmpID"] == DBNull.Value ? "" : row["EmpID"]);
                            emp.Fax = Convert.ToString(row["Fax"] == DBNull.Value ? "" : row["Fax"]);
                            emp.EmpName = Convert.ToString(row["EmpName"] == DBNull.Value ? "" : row["EmpName"]);
                            emp.FirstName = Convert.ToString(row["FirstName"] == DBNull.Value ? "" : row["FirstName"]);
                            emp.ID = Convert.ToInt32(row["ID"] == DBNull.Value ? "" : row["ID"]);
                            emp.LastName = Convert.ToString(row["LastName"] == DBNull.Value ? "" : row["LastName"]);
                            emp.LocationID = Convert.ToInt32(row["LocationID"] == DBNull.Value ? 0 : row["LocationID"]);
                            emp.LocationName = Convert.ToString(row["LocationName"] == DBNull.Value ? "" : row["LocationName"]);
                            emp.MiddleName = Convert.ToString(row["MiddleName"] == DBNull.Value ? "" : row["MiddleName"]);
                            emp.OfficeEmail = Convert.ToString(row["OfficeEmail"] == DBNull.Value ? "" : row["OfficeEmail"]);
                            emp.Dimension2 = Convert.ToString(row["Dimension2"] == DBNull.Value ? "" : row["Dimension2"]);
                            emp.UserCode = Convert.ToString(row["UserCode"] == DBNull.Value ? "" : row["UserCode"]);
                            emp.PassCode = Convert.ToString(row["PassCode"] == DBNull.Value ? "" : row["PassCode"]);
                            emp.EmpName = emp.FirstName + " " + emp.MiddleName + " " + emp.LastName;
                            emp.EmpDetailName = emp.EmpID + " , " + emp.FirstName + " " + emp.MiddleName + " " + emp.LastName + " , " + emp.BranchName + " , " + emp.DepartmentName + " , " + emp.DesignationName;
                        }
                    }


            }
            catch (Exception ex)
            {

                log.HCMOneLog("Exception occured in GetHCMUserByID : " + ex.Message);
            }
            return emp;
        }

        public List<HCM_EmployeeLeaves> GetHCMUserLeaves(int TMSID, int HCMOneID)
        {
            Log log = new Log();
            List<HCM_EmployeeLeaves> empLeaveList = new List<HCM_EmployeeLeaves>();
            try
            {

                string StartDate = "";
                string EndDate = "";
                Common cmn = new Common();
                cmn.GetFiscYear(out StartDate, out EndDate);

                log.HCMOneLog("Getting Data From Sp: spGetCalendarCode");

                SQL_DAL sqlDAL = new SQL_DAL();
                SqlParameter[] parameters = new SqlParameter[] {
                new SqlParameter("@StartDate", StartDate),
                new SqlParameter("@EndDate", EndDate)
                };

                DataSet dsCode = sqlDAL.GetDataSet("spGetCalendarCode", parameters);

                if (dsCode.Tables.Count > 0)
                    if (dsCode.Tables[0].Rows.Count > 0)
                    {
                        log.HCMOneLog("Getting Data From Sp: spGetCalendarCode count" + dsCode.Tables[0].Rows.Count);
                        foreach (DataRow row in dsCode.Tables[0].Rows)
                        {
                            string code = Convert.ToString(row["Code"] == DBNull.Value ? "" : row["Code"]);
                            log.HCMOneLog("Code: " + code);
                            sqlDAL = new SQL_DAL();
                            parameters = new SqlParameter[] {
                             new SqlParameter("@HCMOneID", HCMOneID),
                             new SqlParameter("@CalenderCode", code)
                              };

                            DataSet ds = sqlDAL.GetDataSet("spGetEmpLeavesStatus", parameters);
                            TimeSheetFormManagement mgt = new TimeSheetFormManagement();
                            if (ds.Tables.Count > 0)
                                if (ds.Tables[0].Rows.Count > 0)
                                {
                                    foreach (DataRow dtrow in ds.Tables[0].Rows)
                                    {
                                        HCM_EmployeeLeaves empLeave = new HCM_EmployeeLeaves();
                                        empLeave.ID = Convert.ToInt32(dtrow["ID"] == DBNull.Value ? "" : dtrow["ID"]);
                                        empLeave.TotalAllowed = Convert.ToInt32(dtrow["TotalAllowed"] == DBNull.Value ? 0 : dtrow["TotalAllowed"]);
                                        empLeave.CarryForward = Convert.ToInt32(dtrow["CarryForward"] == DBNull.Value ? "" : dtrow["CarryForward"]);
                                        empLeave.UseD = Convert.ToInt32(dtrow["UseD"] == DBNull.Value ? 0 : dtrow["UseD"]);
                                        if (dtrow["CarryFrwdToDate"] != DBNull.Value)
                                            empLeave.TimeToLapse = (Convert.ToDateTime(dtrow["CarryFrwdToDate"]) - DateTime.Now).TotalDays;

                                        int leaveInTMSCount = mgt.GetEmpUsedLeave(TMSID, Convert.ToInt32(Enums.General.Absence_Management_Internal), empLeave.ID);
                                        empLeave.UseD = empLeave.UseD + leaveInTMSCount;

                                        empLeave.LeaveType = Convert.ToString(dtrow["LeaveType"] == DBNull.Value ? "" : dtrow["LeaveType"]);
                                        empLeave.Balance = empLeave.TotalAllowed - empLeave.UseD;
                                        if (empLeave.Balance < 0)
                                            empLeave.Balance = 0;

                                        empLeaveList.Add(empLeave);
                                    }
                                }
                            break;
                        }
                    }

                log.HCMOneLog("Output: " + Environment.NewLine + JsonConvert.SerializeObject(empLeaveList));
            }
            catch (Exception ex)
            {

                log.HCMOneLog("Exception occured in GetHCMUserLeaves : " + ex.Message);
            }
            return empLeaveList;
        }


        public List<HCM_EmployeeLeaves> GetTestLeaves()
        {
            List<HCM_EmployeeLeaves> empLeaveList = new List<HCM_EmployeeLeaves>();

            HCM_EmployeeLeaves leave = new HCM_EmployeeLeaves();
            leave.ID = 1;
            leave.LeaveType = "1 Leave Type";
            leave.TotalAllowed = 30;
            leave.UseD = 1;
            leave.CarryForward = 0;
            leave.Balance = 29;
            empLeaveList.Add(leave);

            leave = new HCM_EmployeeLeaves();
            leave.ID = 2;
            leave.LeaveType = "2 Leave Type";
            leave.TotalAllowed = 20;
            leave.UseD = 1;
            leave.CarryForward = 0;
            leave.Balance = 19;
            empLeaveList.Add(leave);

            leave = new HCM_EmployeeLeaves();
            leave.ID = 3;
            leave.LeaveType = "3 Leave Type";
            leave.TotalAllowed = 10;
            leave.UseD = 1;
            leave.CarryForward = 0;
            leave.Balance = 9;
            empLeaveList.Add(leave);

            leave = new HCM_EmployeeLeaves();
            leave.ID = 4;
            leave.LeaveType = "4 Leave Type";
            leave.TotalAllowed = 10;
            leave.UseD = 1;
            leave.CarryForward = 0;
            leave.Balance = 9;
            empLeaveList.Add(leave);

            leave = new HCM_EmployeeLeaves();
            leave.ID = 5;
            leave.LeaveType = "5 Leave Type";
            leave.TotalAllowed = 10;
            leave.UseD = 1;
            leave.CarryForward = 0;
            leave.Balance = 9;
            empLeaveList.Add(leave);

            leave = new HCM_EmployeeLeaves();
            leave.ID = 6;
            leave.LeaveType = "6 Leave Type";
            leave.TotalAllowed = 10;
            leave.UseD = 1;
            leave.CarryForward = 0;
            leave.Balance = 9;
            empLeaveList.Add(leave);

            leave = new HCM_EmployeeLeaves();
            leave.ID = 7;
            leave.LeaveType = "7 Leave Type";
            leave.TotalAllowed = 10;
            leave.UseD = 1;
            leave.CarryForward = 0;
            leave.Balance = 9;
            empLeaveList.Add(leave);

            leave = new HCM_EmployeeLeaves();
            leave.ID = 8;
            leave.LeaveType = "8 Leave Type";
            leave.TotalAllowed = 10;
            leave.UseD = 1;
            leave.CarryForward = 0;
            leave.Balance = 9;
            empLeaveList.Add(leave);

            leave = new HCM_EmployeeLeaves();
            leave.ID = 9;
            leave.LeaveType = "9 Leave Type";
            leave.TotalAllowed = 10;
            leave.UseD = 1;
            leave.CarryForward = 0;
            leave.Balance = 9;
            empLeaveList.Add(leave);

            leave = new HCM_EmployeeLeaves();
            leave.ID = 10;
            leave.LeaveType = "10 Leave Type";
            leave.TotalAllowed = 10;
            leave.UseD = 1;
            leave.CarryForward = 0;
            leave.Balance = 9;
            empLeaveList.Add(leave);

            leave = new HCM_EmployeeLeaves();
            leave.ID = 11;
            leave.LeaveType = "11 Leave Type";
            leave.TotalAllowed = 10;
            leave.UseD = 1;
            leave.CarryForward = 0;
            leave.Balance = 9;
            empLeaveList.Add(leave);

            leave = new HCM_EmployeeLeaves();
            leave.ID = 12;
            leave.LeaveType = "12 Leave Type";
            leave.TotalAllowed = 10;
            leave.UseD = 1;
            leave.CarryForward = 0;
            leave.Balance = 9;
            empLeaveList.Add(leave);

            return empLeaveList;
        }
    }
}