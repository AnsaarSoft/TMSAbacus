using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TMSDeloitte.Models
{
    public class Function
    {
        public int ID { get; set; }
        public string Value { get; set; }
        public Function(int Id, string value)
        {
            this.ID = Id;
            this.Value = value;

        }
    }
}