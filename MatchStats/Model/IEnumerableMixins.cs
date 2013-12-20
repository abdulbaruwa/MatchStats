using System.Collections.Generic;
using System.Linq;

namespace MatchStats.Model
{
    public static class IEnumerableMixins
    {
        public static TSource SecondOrDefault<TSource>(this IEnumerable<TSource> source)
        {
            if (source.Count() <= 1) return default(TSource);

            bool isFirst = true;
            foreach (var item in source)
            {
                if (isFirst == false)
                {
                    return item;
                    break;
                }
            }
            return default(TSource);
        }
    }
}