using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TMSDeloitte.Helper;

namespace TMSDeloitte.Models
{
    public class NCTaskAssignmentDetails
    {
        [IgnorePropertyCompare]
        public int ID { get; set; } = 0;

        [IgnorePropertyCompare]
        public int HeaderID { get; set; } = 0;

        [IgnorePropertyCompare]
        public int SNo { get; set; } = 0;

        [IgnorePropertyCompare]
        public string KEY { get; set; }

        [IgnorePropertyCompare]
        public string _Name { get; set; }

        public int TaskID { get; set; }
        
        public bool IsActive { get; set; } = false;
        public bool IsDeleted { get; set; } = false;

    }

    
}