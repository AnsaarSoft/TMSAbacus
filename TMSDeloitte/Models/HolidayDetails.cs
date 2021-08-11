using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TMSDeloitte.Helper;

namespace TMSDeloitte.Models
{
    public class HolidayDetails
    {
        [IgnorePropertyCompare]
        public int? SNO { get; set; }

        [IgnorePropertyCompare]
        public string KEY { get; set; }
        [IgnorePropertyCompare]
        public int ID { get; set; }

        [IgnorePropertyCompare]
        public int HeaderID { get; set; }

        public DateTime Holidate { get; set; }
        [IgnorePropertyCompare]
        public string _Holidate { get; set; }
        public string Description { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? IsActive { get; set; }

    }
}