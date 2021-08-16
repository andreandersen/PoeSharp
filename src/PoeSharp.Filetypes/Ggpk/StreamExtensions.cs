namespace PoeSharp.Filetypes.Ggpk
{
    internal static class StreamExtensions
    {
        private static int _sizeOfRecordHeader = Unsafe.SizeOf<RecordHeader>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static RecordHeader ReadRecordHeader(this FileStream stream)
        {
            Span<byte> headerBytes = stackalloc byte[_sizeOfRecordHeader];
            stream.Read(headerBytes);
            return MemoryMarshal.Read<RecordHeader>(headerBytes);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static IRecord ReadRecord(this FileStream stream)
        {
            var header = stream.ReadRecordHeader();
            return header.Type switch
            {
                RecordType.File => new FileRecord(stream, header.Length),
                RecordType.Free => new FreeRecord(stream, header.Length),
                RecordType.Directory => new DirectoryRecord(stream, header.Length),
                RecordType.Ggpk => new GgpkRecord(stream, header.Length),
                _ => throw ParseException.GgpkParseFailure,
            };
        }
    }
}