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

    public class WIPRecordingForm
    {
        
        [IgnorePropertyCompare]
        public string KEY { get; set; }
        [IgnorePropertyCompare]
        public int ID { get; set; } = 0;
        [IgnorePropertyCompare]
        public string DocNum { get; set; }
        public int EmpID { get; set; } = 0;
        public string EmpCode { get; set; }
        public string AsOnDate { get; set; }
        public string DebitAccount { get; set; }
        public string CreditAccount { get; set; }
        public string ReversalDate { get; set; }
        public string Month { get; set; }
        public string Year { get; set; }

        [IgnorePropertyCompare]
        public string DocDate { get; set; }
        public bool? ReverseJE { get; set; }
        public bool? Posted { get; set; }
        
        [IgnorePropertyCompare]
        public int? CreatedBy { get; set; }
        [IgnorePropertyCompare]
        public DateTime CreateDate { get; set; }

        [IgnorePropertyCompare]
        public int? UpdatedBy { get; set; }
        [IgnorePropertyCompare]
        public DateTime UpdateDate { get; set; }
        
        public bool? IsDeleted { get; set; }

        public int BranchID { get; set; }
        public double WipTotal { get; set; }
        
        [IgnorePropertyCompare]
        public bool isNewDocument { get; set; }

        public List<WIPRecordingFormChild> Table ;
        
        public bool IsNotification { get; set; } = false; //For Alert Setup
        public bool IsEmail { get; set; } = false; //For Alert Setup

        [IgnorePropertyCompare]
        public int SNO { get; set; } = 0; //For Alert Setup 
        [IgnorePropertyCompare]
        public string CurrencyID { get; set; } 
        public string Status { get; set; }
    }
}