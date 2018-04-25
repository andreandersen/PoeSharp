namespace PoeSharp.Shared.EphemeralId
{
    public interface IEphemeralIdObject<out T>
    {
        /// <summary>
        ///     This Id should not be used other than for equality checks.
        ///     Do NOT persist this id, as it may change at any given time.
        /// </summary>
        T EphemeralId { get; }
    }
}