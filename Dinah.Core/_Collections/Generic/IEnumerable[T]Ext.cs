using System;
using System.Collections.Generic;
using System.Linq;

namespace Dinah.Core.Collections.Generic
{
    public static class IEnumerable_T_Ext
    {
        public static bool In<T>(this T source, params T[] parameters) => _in(source, parameters);
        public static bool In<T>(this T source, IEnumerable<T> parameters) => _in(source, parameters);
        private static bool _in<T>(T source, IEnumerable<T> parameters) => parameters.Contains(source);

        /// <summary>Determines whether a string collection contains a specified string. Case-INsensative.</summary>
        public static bool ContainsInsensative(this IEnumerable<string> collection, string str) => collection.Any(item => item.EqualsInsensitive(str));

        /// <summary>Are there any common values between a and b?</summary>
        public static bool SharesAnyValueWith<T>(this IEnumerable<T> a, IEnumerable<T> b)
            => a == null || b == null
            ? false
            : a.Intersect(b).Any();
    }
}
