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

    public class AssignmentFormGeneral
    {

        [IgnorePropertyCompare]
        public int ID { get; set; } = 0;
        [IgnorePropertyCompare]
        public int SNO { get; set; } = 0; //For Alert Setup 

        [IgnorePropertyCompare]
        public int? CreatedBy { get; set; }
        [IgnorePropertyCompare]
        public DateTime CreateDate { get; set; }



        [IgnorePropertyCompare]
        public string DOCNUM { get; set; }
        
        //public string TASK { get; set; }
        public bool? NonChargeable { get; set; }
        public bool? IsDeleted { get; set; }
        [IgnorePropertyCompare]
        public string DocDate { get; set; }
        
        [IgnorePropertyCompare]
        public int? UpdatedBy { get; set; }
        [IgnorePropertyCompare]
        public DateTime UpdateDate { get; set; }

        [IgnorePropertyCompare]
        public string KEY { get; set; } 

        [IgnorePropertyCompare]
        public bool isNewDocument { get; set; }

        [IgnorePropertyCompare]
        public int AuthorizationTableID { get; set; } //For User Authorization

        [IgnorePropertyCompare]
        public int AlertSetupTableID { get; set; } //For Alert Setup

        [IgnorePropertyCompare]
        public int AlertSetupHeaderTableID { get; set; } //For Alert Setup

        public bool IsNotification { get; set; } = false; //For Alert Setup
        public bool IsEmail { get; set; } = false; //For Alert Setup

        public int AssignmentFormID { get; set; } = 0;

        public string TypeOfAssignment { get; set; }
        public int AssignmentNatureID { get; set; } = 0;
        public string TypeOfBilling { get; set; } 
        public string CurrencyID { get; set; } 
        public double AssignmentValue { get; set; } = 0;
        public string StartDate { get; set; }
        
        public string EndDate { get; set; }
        
        public string ClosureDate { get; set; }

        public string Status { get; set; }
        public int DurationInDays { get; set; } = 0;
    }
}