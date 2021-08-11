using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TMSDeloitte.Models
{
    public class Notification
    {
        public int SNO { get; set; }
        public int ID { get; set; }
        public string FromEmp { get; set; }
        public string ToEmp { get; set; }
        public string Detail { get; set; }
        public bool Table { get; set; }
        public string FileName { get; set; } = "";
        public string Date { get; set; }
        public bool Read { get; set; }
    }
}