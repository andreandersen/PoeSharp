namespace PoeSharp.Filetypes.Ggpk.Records
{
    internal readonly struct RecordHeader
    {
        public readonly int Length;
        public readonly RecordType Type;

        public RecordHeader(RecordType type, int length)
        {
            Type = type;
            Length = length;
        }
    }
}