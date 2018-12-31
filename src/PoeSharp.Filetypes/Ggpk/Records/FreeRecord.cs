using System;
using System.IO;

namespace PoeSharp.Filetypes.Ggpk.Records
{
    public sealed class FreeRecord : IRecord
    {
        internal FreeRecord(Stream stream, in int length)
        {
            Length = length;
            Offset = stream.Position - 8;
            NextFreeRecord = stream.ReadInt64();

            stream.Seek(length - 16, SeekOrigin.Current);
        }

        public long NextFreeRecord { get; }
        public long Offset { get; }
        public int Length { get; }
        public ReadOnlyMemory<byte> Name => Memory<byte>.Empty;
    }
}
