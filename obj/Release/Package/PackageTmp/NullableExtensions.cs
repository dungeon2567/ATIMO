using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ATIMO
{
    public static class NullableExtensions
    {
        public static string ToStringOrDefault(this DateTime? source, string format, string defaultValue)
        {
            if (source != null)
            {
                return source.Value.ToString(format);
            }
            else
                return defaultValue;
        }
    }
}