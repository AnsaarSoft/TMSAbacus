using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TMSDeloitte.Helper;

namespace TMSDeloitte.Models
{

    public class UserProfile
    {
        [IgnorePropertyCompare]
        public int ID { get; set; }

        [IgnorePropertyCompare]
        public int HCMOneID { get; set; }

        [IgnorePropertyCompare]
        public string FULLNAME { get; set; }

        [IgnorePropertyCompare]
        public string DETAILNAME { get; set; }

        public string EMPLOYEECODE { get; set; }
        public string USERNAME { get; set; }
        public string PASSWORD { get; set; }

        [IgnorePropertyCompare]
        public int? DESIGNATIONID { get; set; } = 0;

        [IgnorePropertyCompare]
        public string DESIGNATIONNAME { get; set; }

        [IgnorePropertyCompare]
        public int OBAPPROVEDHOURS { get; set; }

        [IgnorePropertyCompare]
        public int OBOVERTIMEHOURS { get; set; }

        [IgnorePropertyCompare]
        public DateTime OBASONDATE { get; set; }

        [IgnorePropertyCompare]
        public int? DEPARTMENTID { get; set; } = 0;

        [IgnorePropertyCompare]
        public string DEPARTMENTNAME { get; set; }

        [IgnorePropertyCompare]
        public int? LOCATIONID { get; set; } = 0;

        [IgnorePropertyCompare]
        public string LOCATIONNAME { get; set; }

        [IgnorePropertyCompare]
        public int? BRANCHID { get; set; } = 0;
        [IgnorePropertyCompare]
        public string BRANCHNAME { get; set; } 

        [IgnorePropertyCompare]
        public string EMAIL { get; set; }

        public bool ISSUPER { get; set; } = false;
        public bool ISACTIVE { get; set; } = true;
        public bool? ISDELETED { get; set; } = false;

        [IgnorePropertyCompare]
        public int? CREATEDBY { get; set; }

        [IgnorePropertyCompare]
        public DateTime CREATEDDATE { get; set; }

        [IgnorePropertyCompare]
        public int? UPDATEDEDBY { get; set; }

        [IgnorePropertyCompare]
        public DateTime UPDATEDDATE { get; set; }

        #region "Below Properties Is Using For Other Table"

        [IgnorePropertyCompare]
        public bool? ISLOGIN { get; set; } //For User Session
        
        [IgnorePropertyCompare]
        public int AuthorizationTableID { get; set; } = 0; //For User Authorization
        
        [IgnorePropertyCompare]
        public int AlertSetupTableID { get; set; } = 0; //For Alert Setup

        [IgnorePropertyCompare]
        public int AlertSetupHeaderTableID { get; set; } //For Alert Setup
        
        public bool IsNotification { get; set; } = false; //For Alert Setup
        
        public bool IsEmail { get; set; } = false; //For Alert Setup

        [IgnorePropertyCompare]
        public string KEY { get; set; }  //For Alert Setup

        [IgnorePropertyCompare]
        public int SNO { get; set; } = 0; //For Alert Setup 
        [IgnorePropertyCompare]
        public int USER_CODE { get; set; } = 0; //For Alert Setup 

        #endregion
    }


}