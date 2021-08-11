using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TMSDeloitte.Models
{

    public class HCM_EmployeeLeaves
    {
        public int ID { get; set; }
        public string LeaveType { get; set; }
        public int TotalAllowed { get; set; }
        public int CarryForward { get; set; }
        public int UseD { get; set; }
        public int Balance { get; set; }

        //For Dashboard
        public double TimeToLapse { get; set; }
    }
}