using System.Collections;
using System.Collections.Generic;

namespace PoeSharp.Shared
{
    public abstract class ReadOnlyDictionaryBase<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>
    {
        protected Dictionary<TKey, TValue> Underlying;

        protected ReadOnlyDictionaryBase()
        {
            Underlying = new Dictionary<TKey, TValue>();
        }

        public bool ContainsKey(TKey key) => Underlying.ContainsKey(key);
        public bool TryGetValue(TKey key, out TValue value) =>
            Underlying.TryGetValue(key, out value);
        public TValue this[TKey key] => Underlying[key];
        public IEnumerable<TKey> Keys => Underlying.Keys;
        public IEnumerable<TValue> Values => Underlying.Values;
        public int Count => Underlying.Count;
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() =>
            Underlying.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => Underlying.GetEnumerator();
    }
}
