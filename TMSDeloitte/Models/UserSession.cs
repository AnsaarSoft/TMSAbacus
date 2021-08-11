using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TMSDeloitte.Models
{
    public class UserSession
    {
        public UserProfile SessionUser { get; set; }
        public List<UserPermissions> pagelist { get; set; }

        public List<int> TimeSheetViewList { get; set; }
        public List<int> DepartmentList { get; set; }
        public List<int> BranchList { get; set; }

        public bool isTimeSheetViewDataAccess { get; set; } = true;
        public bool isBranchDataAccess { get; set; } = true;
        public bool isDepartmentDataAccess { get; set; } = true;
    }
}