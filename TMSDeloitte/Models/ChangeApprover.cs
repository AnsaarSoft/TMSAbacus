using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using TMSDeloitte.DAL;
using TMSDeloitte.Helper;

namespace TMSDeloitte.Models
{

    public class ChangeApproverChild
    {
        
        [IgnorePropertyCompare]
        public int SNO { get; set; } = 0; 
        [IgnorePropertyCompare]
        public string KEY { get; set; }
        
        [IgnorePropertyCompare]
        public int ID { get; set; } = 0;

        public int DocumentID { get; set; } = 0;
        public int ApprovalID { get; set; } = 0;
        public int ApprovalChildID { get; set; } = 0;
        public string EmpID { get; set; }
        public string EmpCode { get; set; }
        public string FullName { get; set; }
        
        public int DepartmentID { get; set; }
        [IgnorePropertyCompare]
        public string DepartmentName { get; set; }

        public int DesignationID { get; set; }
        [IgnorePropertyCompare]
        public string DesignationName { get; set; }

        [IgnorePropertyCompare]
        public string Year { get; set; }
        [IgnorePropertyCompare]
        public string Month { get; set; }
        [IgnorePropertyCompare]
        public string YearsWeeks { get; set; }
        public string Weeks { get; set; }
        [IgnorePropertyCompare]
        public string PendingAt { get; set; }

        public string CHANGETONAME { get; set; }
        public int CHANGETOEMPID { get; set; }
        public string CHANGETOEMPCODE { get; set; }
        public bool Status { get; set; }

        [IgnorePropertyCompare]
        public int? CreatedBy { get; set; }
        [IgnorePropertyCompare]
        public DateTime CreateDate { get; set; }
        [IgnorePropertyCompare]
        public int? UpdatedBy { get; set; }
        [IgnorePropertyCompare]
        public DateTime UpdateDate { get; set; }
        public bool? IsDeleted { get; set; }
        
        public int ApprovalStatus { get; set; }


    }
}