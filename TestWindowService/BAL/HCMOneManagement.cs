using System;
using System.Collections.Generic;
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
            List<Designation> list = new List<Designation>();
            try
            {
                SQL_DAL sqlDAL = new SQL_DAL();
                SqlParameter[] parameters = new SqlParameter[] { };
                DataSet ds = sqlDAL.GetDataSet("spGetAllDesignation", parameters);

                if (ds.Tables.Count > 0)
                    if (ds.Tables[0].Rows.Count > 0)
                    {
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


            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile("Exception occured in GetAllDesignation : " + ex.Message);
            }
            return list;
        }

        public List<Department> GetAllHCMDepartment()
        {
            List<Department> list = new List<Department>();
            try
            {
                SQL_DAL sqlDAL = new SQL_DAL();
                SqlParameter[] parameters = new SqlParameter[] { };
                DataSet ds = sqlDAL.GetDataSet("spGetAllDepartment", parameters);

                if (ds.Tables.Count > 0)
                    if (ds.Tables[0].Rows.Count > 0)
                    {
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


            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile("Exception occured in GetAllDesignation : " + ex.Message);
            }
            return list;
        }

        public List<Location> GetAllHCMLocation()
        {
            List<Location> list = new List<Location>();
            try
            {
                SQL_DAL sqlDAL = new SQL_DAL();
                SqlParameter[] parameters = new SqlParameter[] { };
                DataSet ds = sqlDAL.GetDataSet("spGetAllLocation", parameters);

                if (ds.Tables.Count > 0)
                    if (ds.Tables[0].Rows.Count > 0)
                    {
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


            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile("Exception occured in GetAllDesignation : " + ex.Message);
            }
            return list;
        }

        public List<HCM_Employee> GetAllHCMUser()
        {
            List<HCM_Employee> list = new List<HCM_Employee>();
            try
            {
                SQL_DAL sqlDAL = new SQL_DAL();
                SqlParameter[] parameters = new SqlParameter[] { };
                DataSet ds = sqlDAL.GetDataSet("spGetAllEmp", parameters);

                if (ds.Tables.Count > 0)
                    if (ds.Tables[0].Rows.Count > 0)
                    {
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
                            emp.EmpName = Convert.ToString(row["EmpName"] == DBNull.Value ? "" : row["EmpName"]);
                            emp.FirstName = Convert.ToString(row["FirstName"] == DBNull.Value ? "" : row["FirstName"]);
                            emp.ID = Convert.ToInt32(row["ID"] == DBNull.Value ? "" : row["ID"]);
                            emp.LastName = Convert.ToString(row["LastName"] == DBNull.Value ? "" : row["LastName"]);
                            emp.LocationID = Convert.ToInt32(row["LocationID"] == DBNull.Value ? 0 : row["LocationID"]);
                            emp.LocationName = Convert.ToString(row["LocationName"] == DBNull.Value ? "" : row["LocationName"]);
                            emp.MiddleName = Convert.ToString(row["MiddleName"] == DBNull.Value ? "" : row["MiddleName"]);
                            emp.OfficeEmail = Convert.ToString(row["OfficeEmail"] == DBNull.Value ? "" : row["OfficeEmail"]);
                            emp.EmpName = emp.FirstName + " " + emp.MiddleName + " " + emp.LastName;
                            emp.EmpDetailName = emp.EmpID + " , " + emp.FirstName + " " + emp.MiddleName + " " + emp.LastName + " , " + emp.BranchName + " , " + emp.DepartmentName + " , " + emp.DesignationName;
                            list.Add(emp);
                        }
                    }


            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile("Exception occured in GetAllHCMUser : " + ex.Message);
            }
            return list;
        }

        public HCM_Employee GetHCMUserByID(int id)
        {
            HCM_Employee emp = new HCM_Employee();
            try
            {
                SQL_DAL sqlDAL = new SQL_DAL();
                SqlParameter[] parameters = new SqlParameter[] {
                new SqlParameter("@id", SqlDbType.Int)
                };

                DataSet ds = sqlDAL.GetDataSet("spGetEmpByID", parameters);

                if (ds.Tables.Count > 0)
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow row in ds.Tables[0].Rows)
                        {

                            emp.BranchID = Convert.ToInt32(row["BranchID"] == DBNull.Value ? 0 : row["BranchID"]);
                            emp.BranchName = Convert.ToString(row["BranchName"] == DBNull.Value ? "" : row["BranchName"]);
                            emp.DepartmentID = Convert.ToInt32(row["DepartmentID"] == DBNull.Value ? 0 : row["DepartmentID"]);
                            emp.DepartmentName = Convert.ToString(row["DepartmentName"] == DBNull.Value ? "" : row["DepartmentName"]);
                            emp.DesignationID = Convert.ToInt32(row["DesignationID"] == DBNull.Value ? 0 : row["DesignationID"]);
                            emp.DesignationName = Convert.ToString(row["DesignationName"] == DBNull.Value ? "" : row["DesignationName"]);
                            emp.OBApprovedHours = Convert.ToInt32(row["OBApprovedHours"] == DBNull.Value ? "" : row["OBApprovedHours"]);
                            emp.OBOverTimeHours = Convert.ToInt32(row["OBOverTimeHours"] == DBNull.Value ? "" : row["OBOverTimeHours"]);
                            emp.OBAsOnDate = Convert.ToDateTime(row["OBAsOnDate"] == DBNull.Value ? "" : row["OBAsOnDate"]);
                            emp.EmpID = Convert.ToString(row["EmpID"] == DBNull.Value ? "" : row["EmpID"]);
                            emp.EmpName = Convert.ToString(row["EmpName"] == DBNull.Value ? "" : row["EmpName"]);
                            emp.FirstName = Convert.ToString(row["FirstName"] == DBNull.Value ? "" : row["FirstName"]);
                            emp.ID = Convert.ToInt32(row["ID"] == DBNull.Value ? "" : row["ID"]);
                            emp.LastName = Convert.ToString(row["LastName"] == DBNull.Value ? "" : row["LastName"]);
                            emp.LocationID = Convert.ToInt32(row["LocationID"] == DBNull.Value ? 0 : row["LocationID"]);
                            emp.LocationName = Convert.ToString(row["LocationName"] == DBNull.Value ? "" : row["LocationName"]);
                            emp.MiddleName = Convert.ToString(row["MiddleName"] == DBNull.Value ? "" : row["MiddleName"]);
                            emp.OfficeEmail = Convert.ToString(row["OfficeEmail"] == DBNull.Value ? "" : row["OfficeEmail"]);
                            emp.EmpName = emp.FirstName + " " + emp.MiddleName + " " + emp.LastName;
                            emp.EmpDetailName = emp.EmpID + " , " + emp.FirstName + " " + emp.MiddleName + " " + emp.LastName + " , " + emp.BranchName + " , " + emp.DepartmentName + " , " + emp.DesignationName+ ", "+ emp.OBApprovedHours +", "+emp.OBOverTimeHours+", "+emp.OBAsOnDate;

                        }
                    }


            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile("Exception occured in GetHCMUserByID : " + ex.Message);
            }
            return emp;
        }

        public HCM_Employee GetHCMUserByCode(string code)
        {
            HCM_Employee emp = new HCM_Employee();
            try
            {
                SQL_DAL sqlDAL = new SQL_DAL();
                SqlParameter[] parameters = new SqlParameter[] {
                new SqlParameter("@code", code)
                };

                DataSet ds = sqlDAL.GetDataSet("spGetEmpByCode", parameters);

                if (ds.Tables.Count > 0)
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow row in ds.Tables[0].Rows)
                        {

                            emp.BranchID = Convert.ToInt32(row["BranchID"] == DBNull.Value ? 0 : row["BranchID"]);
                            emp.BranchName = Convert.ToString(row["BranchName"] == DBNull.Value ? "" : row["BranchName"]);
                            emp.DepartmentID = Convert.ToInt32(row["DepartmentID"] == DBNull.Value ? 0 : row["DepartmentID"]);
                            emp.DepartmentName = Convert.ToString(row["DepartmentName"] == DBNull.Value ? "" : row["DepartmentName"]);
                            emp.DesignationID = Convert.ToInt32(row["DesignationID"] == DBNull.Value ? 0 : row["DesignationID"]);
                            emp.DesignationName = Convert.ToString(row["DesignationName"] == DBNull.Value ? "" : row["DesignationName"]);
                            emp.OBApprovedHours = Convert.ToInt32(row["OBApprovedHours"] == DBNull.Value ? "" : row["OBApprovedHours"]);
                            emp.OBOverTimeHours = Convert.ToInt32(row["OBOverTimeHours"] == DBNull.Value ? "" : row["OBOverTimeHours"]);
                            emp.OBAsOnDate = Convert.ToDateTime(row["OBAsOnDate"] == DBNull.Value ? "" : row["OBAsOnDate"]);
                            emp.EmpID = Convert.ToString(row["EmpID"] == DBNull.Value ? "" : row["EmpID"]);
                            emp.EmpName = Convert.ToString(row["EmpName"] == DBNull.Value ? "" : row["EmpName"]);
                            emp.FirstName = Convert.ToString(row["FirstName"] == DBNull.Value ? "" : row["FirstName"]);
                            emp.ID = Convert.ToInt32(row["ID"] == DBNull.Value ? "" : row["ID"]);
                            emp.LastName = Convert.ToString(row["LastName"] == DBNull.Value ? "" : row["LastName"]);
                            emp.LocationID = Convert.ToInt32(row["LocationID"] == DBNull.Value ? 0 : row["LocationID"]);
                            emp.LocationName = Convert.ToString(row["LocationName"] == DBNull.Value ? "" : row["LocationName"]);
                            emp.MiddleName = Convert.ToString(row["MiddleName"] == DBNull.Value ? "" : row["MiddleName"]);
                            emp.OfficeEmail = Convert.ToString(row["OfficeEmail"] == DBNull.Value ? "" : row["OfficeEmail"]);
                            emp.EmpName = emp.FirstName + " " + emp.MiddleName + " " + emp.LastName;
                            emp.EmpDetailName = emp.EmpID + " , " + emp.FirstName + " " + emp.MiddleName + " " + emp.LastName + " , " + emp.BranchName + " , " + emp.DepartmentName + " , " + emp.DesignationName + ", " + emp.OBApprovedHours + ", " + emp.OBOverTimeHours + ", " + emp.OBAsOnDate;
                        }
                    }


            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile("Exception occured in GetHCMUserByID : " + ex.Message);
            }
            return emp;
        }
    }
}