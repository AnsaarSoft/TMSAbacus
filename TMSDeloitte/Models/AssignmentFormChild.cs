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

    public class AssignmentFormChild
    {      
        //[IgnorePropertyCompare]
        //public bool ISACTIVE { get; set; }
        //[IgnorePropertyCompare]
        //public int? BRANCHID { get; set; } = 0;
        //[IgnorePropertyCompare]
        //public string BRANCHNAME { get; set; }
        public bool? ISDELETED { get; set; }
        [IgnorePropertyCompare]
        public int? CreatedBy { get; set; }
        [IgnorePropertyCompare]
        public DateTime CreateDate { get; set; }
        [IgnorePropertyCompare]
        public int? UpdatedBy { get; set; }
        [IgnorePropertyCompare]
        public DateTime UPDATEDATE { get; set; }

        [IgnorePropertyCompare]
        public string KEY { get; set; }

        [IgnorePropertyCompare]
        public int? SNO { get; set; }

        [IgnorePropertyCompare]
        public int? ID { get; set; }
        [IgnorePropertyCompare]
        public int? BranchID { get; set; }
        [IgnorePropertyCompare]
        public string ClientID { get; set; }
        [IgnorePropertyCompare]
        public int? RowID { get; set; }

        [IgnorePropertyCompare]
        public int? AssignmentFormID { get; set; }


        [IgnorePropertyCompare]
        public int? UserID { get; set; }
        public int? EmpID { get; set; }
        public string EmpCode { get; set; }
        
        public string USER_CODE { get; set; }
        public string FULLNAME { get; set; }

        [IgnorePropertyCompare]
        public int? DesignationID { get; set; } = 0;

        [IgnorePropertyCompare]
        public string DESIGNATIONNAME { get; set; }

        [IgnorePropertyCompare]
        public int? DepartmentID { get; set; } = 0;

        [IgnorePropertyCompare]
        public string DEPARTMENTNAME { get; set; }

        [IgnorePropertyCompare]
        public int? TaskID { get; set; }
        public string TASK { get; set; }

        public double TotalHours { get; set; }

        public double StdBillingRateHr { get; set; }
        public double ResourceCost { get; set; }

        [IgnorePropertyCompare]
        public int? TravelRateID { get; set; }
        [IgnorePropertyCompare]
        public int? LocationID { get; set; }

        public string LOCATION { get; set; }

        public double TravelCost { get; set; }
        public double TotalCost { get; set; }

        public double RevenueRateHr { get; set; }
        public double Revenue { get; set; }
        [IgnorePropertyCompare]
        public bool IsChargeable { get; set; }
        [IgnorePropertyCompare]
        public bool InActive { get; set; }


        [IgnorePropertyCompare]
        public bool isNewDocument { get; set; }

        //Rate Trip Policy
       
        public double KM { get; set; }
        public double FROMKM { get; set; }
        public double TOKM { get; set; }
        public double RATETRIP { get; set; }

     
        

    }
}