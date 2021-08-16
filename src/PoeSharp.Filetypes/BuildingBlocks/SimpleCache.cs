namespace PoeSharp.Filetypes.BuildingBlocks
{

    public class SimpleCache<TKey, TValue> where TKey : notnull
    {
        private readonly Dictionary<TKey, TValue> _cacheDict = new();
        private readonly Queue<TKey> _invQueue = new();
        private readonly int _maxCached;
        private readonly object _lock = new();

        public SimpleCache(int maxCache) => _maxCached = maxCache;

        public TValue GetOrAdd(TKey key, Func<TValue> whenMissing)
        {
            lock (_lock)
            {
                if (_cacheDict.TryGetValue(key, out var cachedValue))
                    return cachedValue;

                var val = whenMissing();
                Add(key, val);
                return val;
            }
        }

        private void Add(TKey key, TValue val)
        {
            if (_invQueue.Count >= _maxCached)
                _cacheDict.Remove(_invQueue.Dequeue());

            _invQueue.Enqueue(key);
            _cacheDict.Add(key, val);
        }

        internal void Clear()
        {
            _invQueue.Clear();
            _cacheDict.Clear();
        }
    }
}