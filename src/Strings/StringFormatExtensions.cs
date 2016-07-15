using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonDotNetHelpers.Strings
{
    public static class StringFormatExtensions
    {
   
        public static string EscapeCurlyBraces(this string message)
        {
            string ret = message.Replace("{", "{{").Replace("}", "}}");
            return ret;
        }
    }
}
