using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TMSDeloitte.Helper;

namespace TMSDeloitte.Models
{
    public class ResourceBillingRates
    {
        [IgnorePropertyCompare]
        public int? ID { get; set; }
        [IgnorePropertyCompare]
        public string DocNum { get; set; }
        
        [IgnorePropertyCompare]
        public string FunctionID { get; set; }

        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public bool? ISDELETED { get; set; } = false;

        [IgnorePropertyCompare]
        public int? CREATEDBY { get; set; }

        [IgnorePropertyCompare]
        public DateTime CREATEDDATE { get; set; }

        [IgnorePropertyCompare]
        public int? UPDATEDEDBY { get; set; }

        [IgnorePropertyCompare]
        public DateTime UPDATEDDATE { get; set; }

        

        [IgnorePropertyCompare]
        public bool isNewDocument { get; set; }

        public List<ResourceBillingRatesDetail> Detail { get; set; }
    }
}