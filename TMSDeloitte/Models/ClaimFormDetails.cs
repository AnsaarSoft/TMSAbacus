using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TMSDeloitte.Helper;

namespace TMSDeloitte.Models
{
    public class ClaimFormDetails
    {
        [IgnorePropertyCompare]
        public int ID { get; set; } = 0;

        [IgnorePropertyCompare]
        public int HeaderID { get; set; } = 0;

        [IgnorePropertyCompare]
        public int SNo { get; set; } = 0;

        [IgnorePropertyCompare]
        public string KEY { get; set; }

        [IgnorePropertyCompare]
        public string _Date { get; set; }

        
        public DateTime Date { get; set; }
        public int AssignmentID { get; set; }

        public int ClaimID { get; set; }
        public string Description { get; set; } = "";

     
      
        public double TotalAmount { get; set; } 
        public bool IsDeleted { get; set; } 

    }

    
}