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

    public class GroupSetupInfo
    {

        [IgnorePropertyCompare]
        public int ID { get; set; } = 0;

        [IgnorePropertyCompare]
        public string DOCNUM { get; set; }

        public string GROUPCODE { get; set; }
        public string GROUPNAME { get; set; }

        //public string TASK { get; set; }
        public bool? ISACTIVE { get; set; }
        public bool? ISDELETED { get; set; } = false;
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
        public List<GroupSetupChildInfo> Table ;

        [IgnorePropertyCompare]
        public int AuthorizationTableID { get; set; } //For User Authorization

        [IgnorePropertyCompare]
        public int AlertSetupTableID { get; set; } //For Alert Setup

        [IgnorePropertyCompare]
        public int AlertSetupHeaderTableID { get; set; } //For Alert Setup

        public bool IsNotification { get; set; } = false; //For Alert Setup
        public bool IsEmail { get; set; } = false; //For Alert Setup

        [IgnorePropertyCompare]
        public int SNO { get; set; } = 0; //For Alert Setup 
    }
}