using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using TMSDeloitte.DAL;
using TMSDeloitte.Helper;

namespace TMSDeloitte.Models
{

    public class GroupSetupChildInfo
    {
        [IgnorePropertyCompare]
        public int? SNO { get; set; }

        [IgnorePropertyCompare]
        public int? ID { get; set; }

        [IgnorePropertyCompare]
        public int? GROUPSETUP_ID { get; set; }


        [IgnorePropertyCompare]
        public int? UserID { get; set; }

        [IgnorePropertyCompare]
        public bool ISACTIVE { get; set; }

        public string USER_CODE { get; set; }

        [IgnorePropertyCompare]
        public string USER_NAME { get; set; }

        [IgnorePropertyCompare]
        public int? DESIGNATIONID { get; set; } = 0;

        [IgnorePropertyCompare]
        public string DESIGNATIONNAME { get; set; }

        [IgnorePropertyCompare]
        public int? DEPARTMENTID { get; set; } = 0;

        [IgnorePropertyCompare]
        public string DEPARTMENTNAME { get; set; }

        [IgnorePropertyCompare]
        public int? BRANCHID { get; set; } = 0;

        [IgnorePropertyCompare]
        public string BRANCHNAME { get; set; }
        public bool? ISDELETED { get; set; }
        [IgnorePropertyCompare]
        public int? CREATEDBY { get; set; }
        [IgnorePropertyCompare]
        public DateTime CREATEDATE { get; set; }
        [IgnorePropertyCompare]
        public int? UPDATEDBY { get; set; }
        [IgnorePropertyCompare]
        public DateTime UPDATEDATE { get; set; }
        [IgnorePropertyCompare]
        public string KEY { get; set; }

        [IgnorePropertyCompare]
        public bool isNewDocument { get; set; }

    }
}