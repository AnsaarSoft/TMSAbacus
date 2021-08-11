using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TMSDeloitte.Models
{

    public class B1SP_Parameter
    {
        public string ParameterName { get; set; }
        public string ParameterValue { get; set; }
        public string ParameterType { get; set; }
    }


    public enum DBTypes
    {
        String,
        Bool,
        Int32
    }
}