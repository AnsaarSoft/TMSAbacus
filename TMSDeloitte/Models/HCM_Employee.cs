using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TMSDeloitte.Models
{

    public class HCM_Employee
    {
        public int ID { get; set; }
        public string Fax { get; set; }
        public string EmpID { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string EmpName { get; set; }
        public string EmpDetailName { get; set; }
        public int DesignationID { get; set; }
        public string DesignationName { get; set; }

        public int OBApprovedHours { get; set; }
        public int OBOverTimeHours { get; set; }
        public string OBAsOnDate { get; set; }

        public int DepartmentID { get; set; }
        public string DepartmentName { get; set; }
        public int LocationID { get; set; }
        public string LocationName { get; set; }
        public int BranchID { get; set; }
        public string BranchName { get; set; }
        public string OfficeEmail { get; set; }
        public string Dimension2 { get; set; }
        public string UserCode { get; set; }
        public string PassCode { get; set; }

    }
}