using System;
using System.Linq;

namespace PoeSharp.Util
{
    public sealed class DispoableObjectPool<T> : ObjectPool<T>, IDisposable
        where T : IDisposable
    {
        public DispoableObjectPool(Func<T> creator, int max) : base(creator, max)
        {
        }

        public void Dispose()
        {
            Semaphore.Dispose();
            Creator = null;
            Pool.ToList().ForEach(c => c?.Dispose());
        }
    }
}