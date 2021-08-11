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

    public class WIPRecordingFormChild
    {
        
        [IgnorePropertyCompare]
        public int SNO { get; set; } = 0; //For Alert Setup 
        [IgnorePropertyCompare]
        public string KEY { get; set; }
        [IgnorePropertyCompare]
        public int? WIPRecordingFormID { get; set; }

        [IgnorePropertyCompare]
        public int ID { get; set; } = 0;

        public string ClientID { get; set; }
        public string ClientName { get; set; }
        [IgnorePropertyCompare]
        public string AssignmentCode { get; set; }
        [IgnorePropertyCompare]
        public string AssignmentTitle { get; set; }
        public string PartnerID { get; set; }
        [IgnorePropertyCompare]
        public string PartnerName { get; set; }
        public string FunctionID { get; set; }
        [IgnorePropertyCompare]
        public string FunctionName { get; set; }
        public string SubFunctionID { get; set; }
        [IgnorePropertyCompare]
        public string SubFunctionName { get; set; }
        public double BilledToDate { get; set; } = 0;
        public double WIPAmountToDate { get; set; } = 0;
        public double WIPNotCharged { get; set; } = 0;
        public double WIPToBeCharged { get; set; } = 0;

        [IgnorePropertyCompare]
        public int? CreatedBy { get; set; }
        [IgnorePropertyCompare]
        public DateTime CreateDate { get; set; }
        [IgnorePropertyCompare]
        public int? UpdatedBy { get; set; }
        [IgnorePropertyCompare]
        public DateTime UpdateDate { get; set; }
        public bool? IsDeleted { get; set; }

    }
}