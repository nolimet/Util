using System;
using System.Collections.Generic;
using System.Linq;

namespace NoUtil.Extentsions
{
    public static class LinqExtensions
    {
        public static bool TryFind<T>(this IEnumerable<T> arr, Func<T, bool> func, out T result)
        {
            if (arr.Any(func))
            {
                result = arr.First(func);
                return true;
            }

            result = default;
            return false;
        }
    }
}