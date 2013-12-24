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
                isFirst = false;
            }
            return default(TSource);
        }
        public static TSource ThirdOrDefault<TSource>(this IEnumerable<TSource> source)
        {
            if (source.Count() <= 2) return default(TSource);
            int count = 0;
            foreach (var item in source)
            {
                if (count == 2)
                {
                    return item;
                    break;
                }
                count = count + 1;
            }
            return default(TSource);
        }
    }
}