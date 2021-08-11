using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TMSDeloitte.Helper;

namespace TMSDeloitte.Models
{


    public class HCM_Designation
    {
        [IgnorePropertyCompare]
        public int DesignationID { get; set; }
        [IgnorePropertyCompare]
        public string DesignationName { get; set; }
        [IgnorePropertyCompare]
        public int FunctionID { get; set; }
        [IgnorePropertyCompare]
        public int AuthorizationTableID { get; set; }

        public bool ISDELETED { get; set; }
    }
}