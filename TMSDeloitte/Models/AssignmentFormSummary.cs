using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TMSDeloitte.Helper;

namespace TMSDeloitte.Models
{
    public class AssignmentFormSummary
    {

        [IgnorePropertyCompare]
        public  int?  SNO { get; set; }
        [IgnorePropertyCompare]
        public int? AssignmentFormID { get; set; }
        public int? RowID { get; set; }

        [IgnorePropertyCompare]
        public  int? TaskID { get; set; }
        //public string TASK { get; set; }
        public bool? ISACTIVE { get; set; }
        public bool? ISDELETED { get; set; }
        [IgnorePropertyCompare]
        public  int? CreatedBy { get; set; }
        [IgnorePropertyCompare]
        public DateTime CreateDate { get; set; }
        [IgnorePropertyCompare]
        public  int? UpdatedBy { get; set; }
        [IgnorePropertyCompare]
        public DateTime UpdateDate { get; set; }
        [IgnorePropertyCompare]
        public string KEY { get; set; }
        [IgnorePropertyCompare]
        public string DOCNUM { get; set; }
        [IgnorePropertyCompare]
        public int? FUNCTIONID { get; set; }
        [IgnorePropertyCompare]
        public string FUNCTIONNAME { get; set; }
        public string TASK { get; set; }
        public double TotalBudgetedHour { get; set; }
        public double EstimatedResourceCost { get; set; }
        public double RevenueDistribution { get; set; }
        public double EstimatedRevenue { get; set; }
        [IgnorePropertyCompare]
        public bool isNewDocument { get; set; }
        
        //public List<Master_Task_Detail> taskList{ get; set; }

    }
}