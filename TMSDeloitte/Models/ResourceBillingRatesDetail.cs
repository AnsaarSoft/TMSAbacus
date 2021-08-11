using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TMSDeloitte.Helper;

namespace TMSDeloitte.Models
{
    public class ResourceBillingRatesDetail
    {
        [IgnorePropertyCompare]
        public int? SNO { get; set; }

        [IgnorePropertyCompare]
        public string KEY { get; set; }
        [IgnorePropertyCompare]
        public int ID { get; set; }

        [IgnorePropertyCompare]
        public int HeaderID { get; set; }

        public int DesignationID { get; set; }
        public double RatesPerHour { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? IsActive { get; set; }

    }
}