using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TMSDeloitte.Helper;

namespace TMSDeloitte.Models
{

    public class AssignmentNatureSetup
    {
        [IgnorePropertyCompare]
        public int? SNO { get; set; }

        [IgnorePropertyCompare]
        public int? ID { get; set; }
        public string Type { get; set; }

        public string AssignmentNature { get; set; }
        public bool? ISSUPER { get; set; }
        public bool? IsActive { get; set; }

        public bool? IsDeleted { get; set; }

        [IgnorePropertyCompare]
        public int? CreatedBy { get; set; }

        [IgnorePropertyCompare]
        public DateTime CreatedDate { get; set; }

        [IgnorePropertyCompare]
        public int? UpdatedBy { get; set; }

        [IgnorePropertyCompare]
        public DateTime UpdatedDate { get; set; }

        [IgnorePropertyCompare]
        public string KEY { get; set; }
    }
}