﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace LinqSharp
{
    public static partial class XIEnumerable
    {
        public static IEnumerable<TSource> SelectMore<TSource>(this IEnumerable<TSource> @this, Func<TSource, IEnumerable<TSource>> selector)
        {
            return SelectMore(@this, selector, null);
        }

        public static IEnumerable<TSource> SelectMore<TSource>(this IEnumerable<TSource> @this, Func<TSource, IEnumerable<TSource>> childrenSelector, Func<TSource, bool> predicate)
        {
            IEnumerable<TSource> RecursiveChildren(TSource node)
            {
                if (predicate?.Invoke(node) ?? true)
                    yield return node;

                var selectNode = childrenSelector(node);
                if (selectNode?.Any() ?? false)
                {
                    var children = selectNode.SelectMany(x => RecursiveChildren(x));
                    foreach (var child in children)
                    {
                        if (predicate?.Invoke(node) ?? true)
                            yield return child;
                    }
                }
            }

            var ret = @this
                .Select(x => RecursiveChildren(x))
                .Aggregate((a, b) => a.Concat(b));
            return ret;
        }

    }
}