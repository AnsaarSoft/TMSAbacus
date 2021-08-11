using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TMSDeloitte.Models
{
    public class StaffPendingTimeStatus
    {
        public string Function { get; set; }
        public int TotalSubmitted { get; set; }
        public int TotalApproved { get; set; }
    }
}