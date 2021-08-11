using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TMSDeloitte.Models
{
    public class TimeSheetPeriods
    {   public int ID { get; set; } = 0;
        public int HeaderID { get; set; } = 0;
        public int SNo { get; set; } = 0;
        public string Period { get; set; }
        public string Monday{ get; set; }
        public string Friday{ get; set; }
        public string _Monday { get; set; }
        public string _Friday { get; set; }
        public int StdHoursInWeek { get; set; } = 45;

        public bool IsDeleted { get; set; } = false;

        //For Display Purpose
        public string TimeSheetPeriodDisply { get; set; }
    }

    
}