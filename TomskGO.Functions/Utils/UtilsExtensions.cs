using System;
using System.Collections.Generic;
using System.Linq;

namespace TomskGO.Functions.Utils
{
    public static class UtilsExtensions
    {
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>
            (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }

        public static string GetNameFromHost(this string host)
        {
            if (host.Count(f => f == '.') == 1)
                return host.Split('.')[0];
            else
            {
                var _list = host.Split('.').ToList();
                return _list.ElementAt(_list.Count - 2);
            }
        }
    }
}