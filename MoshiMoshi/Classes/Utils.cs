using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoshiMoshi.Classes
{
    public static class Utils
    {
        private static Random rng = new Random();
        public static void ShuffleList<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static Dictionary<TKey, TValue> Shuffle<TKey, TValue>(this Dictionary<TKey, TValue> dict)
        {
            return dict.OrderBy(x => rng.Next()).ToDictionary(item => item.Key, item => item.Value);
        }
    }
}
