namespace PoeSharp.Shared.DataDeserializers
{
    public interface IDataDeserializer<in TIn, out TOut>
    {
        TOut Deserialize(TIn serialized);
    }
}