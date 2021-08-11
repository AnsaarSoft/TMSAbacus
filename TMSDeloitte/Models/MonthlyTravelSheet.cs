using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TMSDeloitte.Helper;

namespace TMSDeloitte.Models
{

    public class MonthlyTravelSheet
    {
        [IgnorePropertyCompare]
        public int ID { get; set; }
        public int Status { get; set; }

        [IgnorePropertyCompare]
        public string StatusName { get; set; }
        public string DocNum { get; set; }

        [IgnorePropertyCompare]
        public string _DocNum { get; set; }

        [IgnorePropertyCompare]
        public int EmpID { get; set; }

        [IgnorePropertyCompare]
        public string _EmpID { get; set; }

        [IgnorePropertyCompare]
        public string EmpCode { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }

        [IgnorePropertyCompare]
        public string MonthName { get; set; }


        public string DocDate { get; set; }

        public double TotalAmount { get; set; }
        

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
        public string _Attachements { get; set; }

        [IgnorePropertyCompare]
        public string _AttachementsURL { get; set; }

        [IgnorePropertyCompare]
        public List<Attachement> Attachements { get; set; }

        public List<MonthlyTravelSheetDetails> Detail { get; set; }
    }
}