using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TMSDeloitte.Helper;

namespace TMSDeloitte.Models
{

    public class AlertSetup
    {
        [IgnorePropertyCompare]
        public int ID { get; set; }

        [IgnorePropertyCompare]
        public string DocNum { get;  set; }

        [IgnorePropertyCompare]
        public string AlertName { get; set; }
        public string Query { get; set; }
        public int Frequency { get; set; }
        public string FrequencyType { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; } = false;

        [IgnorePropertyCompare]
        public int CreatedBy { get; set; }

        [IgnorePropertyCompare]
        public DateTime CreatedDate { get; set; }

        [IgnorePropertyCompare]
        public int UpdatedBy { get; set; }

        [IgnorePropertyCompare]
        public DateTime UpdatedDate { get; set; }

    }
}