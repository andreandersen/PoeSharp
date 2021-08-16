namespace PoeSharp.Filetypes.Ggpk.Records
{
    public interface IRecord
    {
        int Length { get; }

        /// <summary>
        /// UTF-8 representation in bytes
        /// </summary>
        string Name => string.Empty;

        long Offset { get; }
    }
}