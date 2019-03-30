using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Helpers
{
    public static class ListExtensions
    {
        public static IEnumerable<T> Exclude<T>(this IEnumerable<T> list, T value)
            => list.Where(v => !v.Equals(value));
    }
}
