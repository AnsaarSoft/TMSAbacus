using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace TMSDeloitte.Helper
{

    public static class PropertyCompare
    {
        public static List<PropertyCompareResult> Compare<T>(T oldObject, T newObject)
        {
            PropertyInfo[] properties = typeof(T).GetProperties();
            List<PropertyCompareResult> result = new List<PropertyCompareResult>();

            foreach (PropertyInfo pi in properties)
            {
                if (pi.CustomAttributes.Any(ca => ca.AttributeType == typeof(IgnorePropertyCompareAttribute)))
                {
                    continue;
                }

                object oldValue = pi.GetValue(oldObject), newValue = pi.GetValue(newObject);

                if (!object.Equals(oldValue, newValue))
                {
                    result.Add(new PropertyCompareResult(pi.Name, oldValue, newValue));
                }
            }

            return result;
        }
    }
}