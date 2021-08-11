using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TMSDeloitte.Helper;

namespace TMSDeloitte.Models
{
    public class ReportInfo
    {
        [IgnorePropertyCompare]
        public int ID { get; set; } = 0;

        [IgnorePropertyCompare]
        public string DocNum { get; set; }

        public string ReportCode { get; set; }
        public string ReportName { get; set; }
        public string RptFile { get; set; }

        public HttpPostedFileBase File { get; set; }

        public int? DOCID { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDeleted { get; set; }
        [IgnorePropertyCompare]
        public int? CreatedBy { get; set; }
        [IgnorePropertyCompare]
        public DateTime CreateDate { get; set; }
        [IgnorePropertyCompare]
        public int? UpdatedBy { get; set; }
        [IgnorePropertyCompare]
        public DateTime UpdateDate { get; set; }

        [IgnorePropertyCompare]
        public string KEY { get; set; }

        [IgnorePropertyCompare]
        public int SNO { get; set; } = 0; //For Alert Setup 
    }
}