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

    public class ApprovalDecision
    {
        [IgnorePropertyCompare]
        public int? SNO { get; set; }

        [IgnorePropertyCompare]
        public int? ID { get; set; }

        [IgnorePropertyCompare]
        public string DocNum { get; set; }
        [IgnorePropertyCompare]
        public string Task { get; set; }
        [IgnorePropertyCompare]
        public string Location { get; set; }
        [IgnorePropertyCompare]
        public int? Year { get; set; }
        [IgnorePropertyCompare]
        public string WorkDate { get; set; }
        [IgnorePropertyCompare]
        public string ClaimDate { get; set; }
        [IgnorePropertyCompare]
        public string Description { get; set; }
        [IgnorePropertyCompare]
        public string WorkHours { get; set; }
        //[IgnorePropertyCompare]
        //public string Status { get; set; }
        [IgnorePropertyCompare]
        public string TravelDate { get; set; }
        [IgnorePropertyCompare]
        public string Assignment { get; set; }
        [IgnorePropertyCompare]
        public string Kilometers { get; set; }
        [IgnorePropertyCompare]
        public string Amount { get; set; }
        [IgnorePropertyCompare]
        public string ParkingCharges { get; set; }
        [IgnorePropertyCompare]
        public string TotalAmount { get; set; }



        [IgnorePropertyCompare]
        public int? UserID { get; set; }

        [IgnorePropertyCompare]
        public bool ISACTIVE { get; set; }
        public string CreateDate { get; set; }
        public string TypeOfAssignment { get; set; }
        public string TypeOfBilling { get; set; }
        public string CurrencyID { get; set; }
        public double AssignmentValue { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }

        public string USER_CODE { get; set; }

        [IgnorePropertyCompare]
        public string ENC_USER_CODE { get; set; }
        public string USER_NAME { get; set; }
        public string EMPLOYEECODE { get; set; }

        [IgnorePropertyCompare]
        public int? DESIGNATIONID { get; set; } = 0;

        [IgnorePropertyCompare]
        public string DESIGNATIONNAME { get; set; }
        public string EMAIL { get; set; }

        [IgnorePropertyCompare]
        public int? DEPARTMENTID { get; set; } = 0;

        [IgnorePropertyCompare]
        public string DEPARTMENTNAME { get; set; }
        public int? DOCUMENT_ID { get; set; }
        public int? ApprovalDecision_ID { get; set; }
        [IgnorePropertyCompare]
        public string COMMENTS { get; set; }

        [IgnorePropertyCompare]
        public string FROMDATE { get; set; }
        [IgnorePropertyCompare]
        public string TODATE { get; set; }

        [IgnorePropertyCompare]
        public string DOCUMENT { get; set; }

        [IgnorePropertyCompare]
        public string DOCUMENT_NO { get; set; }

        [IgnorePropertyCompare]
        public string ENC_DOCUMENT_NO { get; set; }
        public string APPROVALREQUIRED { get; set; }
        public string REJECTIONREQUIRED { get; set; }

        public string CurrentREJECTION { get; set; }
        public string CurrentAPPROVAL { get; set; }

        [IgnorePropertyCompare]
        public string STATUS { get; set; }
        [IgnorePropertyCompare]
        public int? CREATEDBY { get; set; }
        //[IgnorePropertyCompare]
        //public DateTime CREATEDATE { get; set; }
        [IgnorePropertyCompare]
        public int? UPDATEDBY { get; set; }
        [IgnorePropertyCompare]
        public DateTime UPDATEDATE { get; set; }
        [IgnorePropertyCompare]
        public string KEY { get; set; }

        [IgnorePropertyCompare]
        public bool isNewDocument { get; set; }

        //Adde by Muhammad Maqbool
        [IgnorePropertyCompare]
        public string AppSetupID { get; set; } 

    }
}