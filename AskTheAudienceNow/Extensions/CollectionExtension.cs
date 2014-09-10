using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AskTheAudienceNow
{
    public static class CollectionExtension
    {
        public static IEnumerable<T> ToEnumerable<T>(this Random random, Func<Random, T> generator)
        {
            for (; ; )
            {
                yield return generator(random);
            }
        }
    }
}