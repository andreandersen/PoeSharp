namespace PoeSharp.Files.Ggpk.Records
{
    public interface IRecord
    {
        long Offset { get; }
        int Length { get; }
        string Name { get; }
    }
}
