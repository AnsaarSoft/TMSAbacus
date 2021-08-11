using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TMSDeloitte.Helper;

namespace TMSDeloitte.Models
{
    public class DropDown
    {
        [IgnorePropertyCompare]
        public int ID { get; set; }
        [IgnorePropertyCompare]
        public string AssignmentType { get; set; }
        [IgnorePropertyCompare]
        public string BillingType { get; set; }
        [IgnorePropertyCompare]
        public string StatusType { get; set; }

        public double StdBillingRateHr { get; set; }

    }
}