using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TMSDeloitte.Helper;

namespace TMSDeloitte.Models
{

    public class GenerateTimeSheet
    {
        public int ID { get; set; }
        public string DocNum { get; set; }
        public DateTime fromDate { get; set; }
        public DateTime toDate { get; set; }
        public string year { get; set; }
        public bool? ISDELETED { get; set; } = false;
        
        public int? CREATEDBY { get; set; }
        
        public DateTime CREATEDDATE { get; set; }
        
        public int? UPDATEDEDBY { get; set; }
        
        public DateTime UPDATEDDATE { get; set; }

        public List<TimeSheetPeriods> PeriodList { get; set; }
    }
}