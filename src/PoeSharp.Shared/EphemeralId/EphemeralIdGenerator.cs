using System;
using System.Collections.Concurrent;

namespace PoeSharp.Shared.EphemeralId
{
    public class EphemeralIdGenerator<TIn, TOut> : IEphemeralIdGenerator<TIn, TOut>
    {
        private readonly Func<ConcurrentDictionary<TIn, TOut>, TIn, TOut> _incrementor;

        private readonly ConcurrentDictionary<TIn, TOut> _observedIds;

        // ReSharper disable once MemberCanBeProtected.Global
        public EphemeralIdGenerator(
            Func<ConcurrentDictionary<TIn, TOut>, TIn, TOut> incrementor)
        {
            _observedIds = new ConcurrentDictionary<TIn, TOut>();
            _incrementor = incrementor;
        }

        public TOut Generate(TIn id) =>
            _observedIds.GetOrAdd(id, _incrementor(_observedIds, id));
    }
}