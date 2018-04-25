using System;

namespace PoeSharp.Shared.EphemeralId
{
    public abstract class WithEphemeralStringId<T> : IEquatable<T>,
        IEphemeralIdObject<int> where T : IEphemeralIdObject<int>
    {
        // ReSharper disable once StaticMemberInGenericType (I know what I am doing, R#, relax)
        private static readonly StandardIntStringEphemeralGenerator EphemeralIdGenerator =
            new StandardIntStringEphemeralGenerator();

        protected WithEphemeralStringId(string id)
        {
            EphemeralId = EphemeralIdGenerator.Generate(id);
        }

        private int EphemeralId { get; }

        int IEphemeralIdObject<int>.EphemeralId => EphemeralId;

        public bool Equals(T other) => EphemeralId == other?.EphemeralId;
    }
}