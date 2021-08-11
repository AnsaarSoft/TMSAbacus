using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TMSDeloitte.Models
{
    public class Master_Task_Detail
    {
        public  int?  SNO { get; set; }
        public  int?  ID { get; set; }
        public int? TASKID { get; set; }
        public string TASK { get; set; }
        public bool? ISACTIVE { get; set; }
        public bool? ISDELETED { get; set; }
        public  int?  CREATEDBY { get; set; }
        public DateTime CREATEDDATE { get; set; }
        public  int?  UPDATEDEDBY { get; set; }
        public DateTime UPDATEDDATE { get; set; }
        public string KEY { get; set; }
     
    }
}