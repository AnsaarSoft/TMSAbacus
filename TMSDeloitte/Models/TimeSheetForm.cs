using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TMSDeloitte.Helper;

namespace TMSDeloitte.Models
{

    public class TimeSheetForm
    {
        [IgnorePropertyCompare]
        public int ID { get; set; }

        public int Status { get; set; }

        [IgnorePropertyCompare]
        public string StatusName { get; set; }

        
        public string DocNum { get; set; }

        [IgnorePropertyCompare]
        public string _DocNum { get; set; }

        [IgnorePropertyCompare]
        public int EmpID { get; set; }

        [IgnorePropertyCompare]
        public string _EmpID { get; set; }

        [IgnorePropertyCompare]
        public string EmpCode { get; set; }
        public int Year { get; set; }
        public int Period { get; set; }

        [IgnorePropertyCompare]
        public string PeriodText { get; set; }

        [IgnorePropertyCompare]
        public string _Monday { get; set; }

        [IgnorePropertyCompare]
        public string _Friday { get; set; }

        [IgnorePropertyCompare]
        public string FromDate { get; set; }

        [IgnorePropertyCompare]
        public int StdHoursInWeek { get; set; }

        [IgnorePropertyCompare]
        public string ToDate { get; set; }
        public double StandardHours { get; set; }
        public double NonChargeableHours { get; set; }
        public double ChargeableHours { get; set; }
        public double OverTimeHours { get; set; }
        public double LeaveHours { get; set; }
        public double TotalHours { get; set; }

        [IgnorePropertyCompare]
        public bool? IsDeleted { get; set; } = false;

        [IgnorePropertyCompare]
        public int? CreatedBy { get; set; }

        [IgnorePropertyCompare]
        public DateTime CreatedDate { get; set; }

        [IgnorePropertyCompare]
        public int? UpdatedBy { get; set; }

        [IgnorePropertyCompare]
        public DateTime UpdatedDate { get; set; }

        public List<TimeSheetFormDetails> Detail { get; set; }
    }
}