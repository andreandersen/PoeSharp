using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using PoeSharp.Filetypes.Ggpk.Records;

namespace PoeSharp.Filetypes.Ggpk
{
    internal static class StreamExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static RecordHeader ReadRecordHeader(this FileStream stream)
        {
            Span<byte> headerBytes = new byte[8];
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
