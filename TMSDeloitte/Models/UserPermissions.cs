using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TMSDeloitte.Models
{
    public class UserPermissions
    {

        public int ID { get; set; }
        public int HeadID { get; set; }
        public string PageName { get; set; }
        public string PageURL { get; set; }
        public Nullable<int> Order { get; set; }
        public string Controller { get; set; }
        public int Role { get; set; }
    }
}