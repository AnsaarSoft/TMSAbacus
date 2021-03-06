using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TMSDeloitte.Helper;

namespace TMSDeloitte.Models
{

    public class NCTaskAssignment
    {
        [IgnorePropertyCompare]
        public int ID { get; set; }

        [IgnorePropertyCompare]
        public string DocNum { get; set; }

        [IgnorePropertyCompare]
        public int EmpID { get; set; }

        [IgnorePropertyCompare]
        public string EmpCode { get; set; }

        [IgnorePropertyCompare]
        public string EmpName { get; set; }

        public string DocDate { get; set; }

       

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

        [IgnorePropertyCompare]
        public List<NCTaskAssignmentDetails> Detail { get; set; }
    }
}