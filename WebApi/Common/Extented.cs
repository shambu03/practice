using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi
{
    public static class Extended
    {
        public static bool In<T>(this T obj, params T[] values)
        {
            return values.Any(x => x.Equals(obj));
        }
    }
}
