using System;
using System.Collections.Concurrent;
using System.Threading;

namespace PoeSharp.Util
{
    public class ObjectPool<T>
    {
        protected Func<T> Creator;
        protected ConcurrentQueue<T> Pool;
        protected SemaphoreSlim Semaphore;

        public ObjectPool(Func<T> creator, int max)
        {
            Pool = new ConcurrentQueue<T>();
            Semaphore = new SemaphoreSlim(max);
            Creator = creator;
        }

        public T Get()
        {
            Semaphore.Wait();
            return Pool.TryDequeue(out var res) ? res : Creator.Invoke();
        }

        public void Release(T obj)
        {
            Pool.Enqueue(obj);
            Semaphore.Release();
        }
    }
}