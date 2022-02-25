using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace IFFCO.TECHPROD.Web.CommonFunctions
{
    public static class CustomClass
    {
        public static string Date(this DateTime date)
        {
            return date.ToString("dd-MMM-yyyy");
        }
        public static string ToProperCase(this string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return "";
            }
              return string.Join(" ", (text.Split(' ').Select(e => e.Substring(0, 1).ToUpper() + e.Substring(1).ToLower()).ToArray()));
            
           
        }
    }
}
