using System;
using System.Collections.Concurrent;

namespace PoeSharp.Util
{
    public static class MemoizeExtension
    {
        public static Func<TArg, TResult> Memoize<TArg, TResult>(
            this Func<TArg, TResult> func)
        {
            var cache = new ConcurrentDictionary<TArg, TResult>();
            return argument => cache.GetOrAdd(argument, func);
        }
    }
}