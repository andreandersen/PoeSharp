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
            switch (header.Type)
            {
                case RecordType.File:
                    return new FileRecord(stream, header.Length);
                case RecordType.Free:
                    return new FreeRecord(stream, header.Length);
                case RecordType.Directory:
                    return new DirectoryRecord(stream, header.Length);
                case RecordType.Ggpk:
                    return new GgpkRecord(stream, header.Length);
                default:
                    throw ParseException.GgpkParseFailure;
            }
        }
    }
}
