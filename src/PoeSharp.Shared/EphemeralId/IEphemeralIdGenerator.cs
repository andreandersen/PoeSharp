namespace PoeSharp.Shared.EphemeralId
{
    public interface IEphemeralIdGenerator<in TIn, out TOut>
    {
        TOut Generate(TIn id);
    }
}