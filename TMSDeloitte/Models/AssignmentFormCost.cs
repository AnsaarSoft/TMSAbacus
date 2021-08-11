using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TMSDeloitte.Helper;

namespace TMSDeloitte.Models
{
    public class AssignmentFormCost
    {

        [IgnorePropertyCompare]
        public  int?  SNO { get; set; }

        [IgnorePropertyCompare]
        public  int?  ID { get; set; }
        [IgnorePropertyCompare]
        public int? AssignmentFormID { get; set; }
        public int? RowID { get; set; }
        public int? AssignmentCostSetupID { get; set; }
        
        //public string TASK { get; set; }
        public bool? ISACTIVE { get; set; }
        public bool? ISDELETED { get; set; }
        [IgnorePropertyCompare]
        public string KEY { get; set; }
        [IgnorePropertyCompare]
        public string DOCNUM { get; set; }
        public string TYPEOFCOST { get; set; }
        public double Amount { get; set; }
        [IgnorePropertyCompare]
        public bool isNewDocument { get; set; }
        public int? CreatedBy { get; set; }
        [IgnorePropertyCompare]
        public DateTime CreateDate { get; set; }
        [IgnorePropertyCompare]
        public int? UpdatedBy { get; set; }
        [IgnorePropertyCompare]
        public DateTime UpdateDate { get; set; }

        //public List<Master_Task_Detail> taskList{ get; set; }

    }
}