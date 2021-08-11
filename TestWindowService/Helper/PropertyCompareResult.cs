using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace TMSDeloitte.Helper
{

    class IgnorePropertyCompareAttribute : Attribute { }
    public class PropertyCompareResult
    {
        public string Name { get; private set; }
        public object OldValue { get; private set; }
        public object NewValue { get; private set; }

        public PropertyCompareResult(string name, object oldValue, object newValue)
        {
            Name = name;
            OldValue = oldValue;
            NewValue = newValue;
        }

        
    }
   
}