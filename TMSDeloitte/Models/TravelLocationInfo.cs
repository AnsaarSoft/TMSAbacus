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
    public class TravelLocationInfo
    {

        [IgnorePropertyCompare]
        public  int?  SNO { get; set; }
        [IgnorePropertyCompare]
        public  int?  ID { get; set; }
        //public string TASK { get; set; }
        public bool? ISACTIVE { get; set; }
        public bool? ISDELETED { get; set; }
        [IgnorePropertyCompare]
        public  int?  CREATEDBY { get; set; }
        [IgnorePropertyCompare]
        public DateTime CREATEDATE { get; set; }
        [IgnorePropertyCompare]
        public  int?  UPDATEDBY { get; set; }
        [IgnorePropertyCompare]
        public DateTime UPDATEDATE { get; set; }
        [IgnorePropertyCompare]
        public string KEY { get; set; }
        [IgnorePropertyCompare]
        public string DOCNUM { get; set; }

        [IgnorePropertyCompare]
        public int? BRANCHID { get; set; }

        [IgnorePropertyCompare]
        public int? BRANCHNAME { get; set; }

        [IgnorePropertyCompare]
        public int? CLIENTID { get; set; }

        [IgnorePropertyCompare]

        public string CLIENTNAME { get; set; }
        public string LOCATION { get; set; }

        public double KM { get; set; }
        [IgnorePropertyCompare]
        public bool isNewDocument { get; set; }
   
    }
}