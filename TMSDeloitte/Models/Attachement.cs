using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TMSDeloitte.Helper;

namespace TMSDeloitte.Models
{
    public class Attachement
    {
        [IgnorePropertyCompare]
        public int SNo { get; set; }

        [IgnorePropertyCompare]
        public string Name { get; set; }

        [IgnorePropertyCompare]
        public string ImagePath { get; set; }
    }
}