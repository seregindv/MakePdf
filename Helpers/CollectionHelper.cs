using System.Collections;
using System.Collections.Generic;

namespace MakePdf.Helpers
{
    public static class CollectionHelper
    {
        public static bool IsNullOrEmpty<T>(T collection) where T : ICollection
        {
            return collection == null || collection.Count == 0;
        }

        public static IEnumerable<T> ToEnumerable<T>(this T obj)
        {
            yield return obj;
        }
    }
}
