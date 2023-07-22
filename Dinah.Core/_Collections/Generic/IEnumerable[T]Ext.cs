using System;
using System.Collections.Generic;
using System.Linq;

namespace Dinah.Core.Collections.Generic
{
    public enum WinnerEnum
    {
        FirstInWins,
        LastInWins
    }

    public static class IEnumerable_T_Ext
    {
        /// <summary>Determines whether a string collection contains a specified string. Case-INsensative.</summary>
        public static bool ContainsInsensative(this IEnumerable<string> collection, string str) => collection.Any(item => item.EqualsInsensitive(str));

        /// <summary>Are there any common values between a and b?</summary>
        public static bool SharesAnyValueWith<T>(this IEnumerable<T> a, IEnumerable<T> b)
            => a is not null
            && b is not null
            && a.Intersect(b).Any();

        /// <summary>
        /// Same as ToDictionary but avoids key conflicts:
        /// Creates a System.Collections.Generic.Dictionary`2 from an System.Collections.Generic.IEnumerable`1 according to a specified key selector function.
        /// </summary>
        /// <typeparam name="TKey">The type of the key returned by keySelector.</typeparam>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">An System.Collections.Generic.IEnumerable`1 to create a System.Collections.Generic.Dictionary`2 from.</param>
        /// <param name="keySelector">A function to extract a key from each element.</param>
        /// <param name="winner">Whether first in wins or last in wins. Default: first in wins</param>
        /// <returns>
        /// A System.Collections.Generic.Dictionary`2 that contains keys and values. The values within each group are in the same order as in source. Key conflicts are safely avoided.
        /// </returns>
        public static Dictionary<TKey, TSource> ToDictionarySafe<TKey, TSource>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, WinnerEnum winner = WinnerEnum.FirstInWins) where TKey : notnull
        {
            var dic = new Dictionary<TKey, TSource>();

            if (winner == WinnerEnum.LastInWins)
                foreach (var value in source)
                    dic[keySelector(value)] = value;
            else
            {
                foreach (var value in source)
                {
                    TKey key = keySelector(value);
                    if (!dic.ContainsKey(key))
                        dic[key] = value;
                }
            }

            return dic;
        }
    }
}
