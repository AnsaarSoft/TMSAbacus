using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TMSDeloitte.Helper;

namespace TMSDeloitte.Models
{
    public class TimeSheetFormDetails
    {
        [IgnorePropertyCompare]
        public int ID { get; set; } = 0;

        [IgnorePropertyCompare]
        public int HeaderID { get; set; } = 0;

        [IgnorePropertyCompare]
        public int SNo { get; set; } = 0;

        [IgnorePropertyCompare]
        public string KEY { get; set; }
        public string _WorkDate { get; set; }

        [IgnorePropertyCompare]
        public string WorkDate { get; set; }
        public int AssignmentID { get; set; }
        public int TaskID { get; set; }
        public int LocationID { get; set; }
        public double WorkHours { get; set; }
        public string Description { get; set; } = ""; 
        public bool OnSite { get; set; } = false;
        public bool IsDeleted { get; set; } = false;

    }

    
}