using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TMSDeloitte.Models
{

    public class Director
    {
        public int DirectorID { get; set; }
        public string PmId { get; set; }
        public string PmName { get; set; }
        public string DirectorCode { get; set; }
        public string DirectorName { get; set; }
        public string DepartmentName { get; set; }
    }
}