using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TMSDeloitte.Helper;

namespace TMSDeloitte.Models
{
    public class TaskMasterSetupInfo
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
        public DateTime CREATEDDATE { get; set; }
        [IgnorePropertyCompare]
        public  int?  UPDATEDEDBY { get; set; }
        [IgnorePropertyCompare]
        public DateTime UPDATEDDATE { get; set; }
        [IgnorePropertyCompare]
        public string KEY { get; set; }
        [IgnorePropertyCompare]
        public string DOCNUM { get; set; }
        [IgnorePropertyCompare]
        public int? FUNCTIONID { get; set; }
        [IgnorePropertyCompare]
        public string FUNCTIONNAME { get; set; }
        public string TASK { get; set; }
        [IgnorePropertyCompare]
        public bool isNewDocument { get; set; }
        [IgnorePropertyCompare]
        public int? TaskID { get; set; }

        //public List<Master_Task_Detail> taskList{ get; set; }

    }
}