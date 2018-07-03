using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ATIMO
{
    public static class LinqExtensions
    {
        public static IEnumerable<TResult> FullOuterJoin<TOuter, TInner, TKey, TResult>(

this IEnumerable<TOuter> outer,

IEnumerable<TInner> inner,

Func<TOuter, TKey> outerKeySelector,

Func<TInner, TKey> innerKeySelector,

Func<TOuter, TInner, TResult> resultSelector)

        {

            if (outer == null)
            {

                throw new ArgumentNullException("outer");

            }

            if (inner == null)

            {

                throw new ArgumentNullException("inner");

            }

            if (outerKeySelector == null)

            {

                throw new ArgumentNullException("outerKeySelector");

            }

            if (innerKeySelector == null)

            {

                throw new ArgumentNullException("innerKeySelector");

            }

            if (resultSelector == null)

            {

                throw new ArgumentNullException("resultSelector");

            }

            var innerLookup = inner.ToLookup(innerKeySelector);

            var outerLookup = outer.ToLookup(outerKeySelector);

            var allKeys = (from i in innerLookup select i.Key).Union(

            from o in outerLookup select o.Key);

            var result = from key in allKeys

                         from innerElement in innerLookup[key].DefaultIfEmpty()

                         from outerElement in outerLookup[key].DefaultIfEmpty()

                         select resultSelector(outerElement, innerElement);

            return result;

        }
    }
}