using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CDC_Web_Portal_Add_On.Models
{
    public class Dependencies
    {
        public int Id { get; set; }
        public int Page { get; set; }
        public bool IsSync { get; set; }
        public int Dependency { get; set; }
    }
}