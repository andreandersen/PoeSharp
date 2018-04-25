namespace PoeSharp.Shared
{
    public interface IDataLoader<TIn, TOut>
    {
        TOut Load(TIn source);
    }
}
