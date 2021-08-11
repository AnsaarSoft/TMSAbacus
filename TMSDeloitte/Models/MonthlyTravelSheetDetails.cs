using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TMSDeloitte.Helper;

namespace TMSDeloitte.Models
{
    public class MonthlyTravelSheetDetails
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
        public string _TravelDate { get; set; }

        
        public DateTime TravelDate { get; set; }
        public int AssignmentID { get; set; }

        [IgnorePropertyCompare]
        public string AssignmentTitle { get; set; } = "";

        [IgnorePropertyCompare]
        public string Description { get; set; } = "";

        [IgnorePropertyCompare]
        public string ClientID { get; set; } = "";

        [IgnorePropertyCompare]
        public string ClientName { get; set; } = "";

        [IgnorePropertyCompare]
        public int LocationID { get; set; } = 0;
        public double Kilometers { get; set; }
        public double Amount { get; set; }
        public double ParkingCharges { get; set; }
      
        public double TotalAmount { get; set; } 
        public bool IsDeleted { get; set; } = false;

    }

    
}