using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TMSDeloitte.Helper;

namespace TMSDeloitte.Models
{

    public class UserDataAccess_Menu
    {
        [IgnorePropertyCompare]
        public int _ID { get; set; } = 0; //db table id
        [IgnorePropertyCompare]
        public int Index { get; set; }
        [IgnorePropertyCompare]
        public int ID { get; set; } //Menu ID
        [IgnorePropertyCompare]
        public int Head_ID { get; set; }
        [IgnorePropertyCompare]
        public int DataAccessID { get; set; } //Parent Table
        [IgnorePropertyCompare]
        public string Name { get; set; }

        public int Authorization { get; set; } // Authorization Static List

        public bool IsDeleted { get; set; } = false;

        [IgnorePropertyCompare]
        public bool IsChecked { get; set; } = false; //using for filter out selected record in grid
    }
}