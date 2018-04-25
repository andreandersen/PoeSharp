using System;

namespace PoeSharp.Util
{
    public static class ObjectPoolExtensions
    {
        public static void Use<T>(this ObjectPool<T> pool, Action<T> action)
        {
            var t = pool.Get();
            action(t);
            pool.Release(t);
        }

        public static TOut Use<T, TOut>(this ObjectPool<T> pool, Func<T, TOut> func)
        {
            var t = pool.Get();
            var res = func(t);
            pool.Release(t);
            return res;
        }
    }
}