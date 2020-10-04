using System.Collections;
using System.Collections.Generic;

namespace PoeSharp.Filetypes.BuildingBlocks
{
    public abstract class ReadOnlyListBase<T> : IReadOnlyList<T>
    {
        protected List<T> Underlying;
        public T this[int index] => Underlying[index];
        public int Count => Underlying.Count;
        public IEnumerator<T> GetEnumerator() => Underlying.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => Underlying.GetEnumerator();
    }
}
