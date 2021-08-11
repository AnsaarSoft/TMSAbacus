using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TMSDeloitte.Helper;

namespace TMSDeloitte.Models
{
    public class NonChargableSetupInfo
    {

        [IgnorePropertyCompare]
        public int? SNO { get; set; }

        [IgnorePropertyCompare]
        public int? ID { get; set; }
        public string NCTASKS { get; set; }
        public bool? ISACTIVE { get; set; }

        public bool? ISDELETED { get; set; }

        [IgnorePropertyCompare]
        public int? CREATEDBY { get; set; }

        [IgnorePropertyCompare]
        public DateTime CREATEDDATE { get; set; }

        [IgnorePropertyCompare]
        public int? UPDATEDEDBY { get; set; }

        [IgnorePropertyCompare]
        public DateTime UPDATEDDATE { get; set; }

        [IgnorePropertyCompare]
        public string KEY { get; set; }
    }
}