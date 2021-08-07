using System;
using System.IO;

using Microsoft.Toolkit.HighPerformance;

namespace PoeSharp.Filetypes.Ggpk.Records
{
    public sealed class FreeRecord : IRecord
    {
        internal FreeRecord(Stream stream, in int length)
        {
            Length = length;
            Offset = stream.Position - 8;

            NextFreeRecord = stream.Read<long>();

            stream.Seek(length - 16, SeekOrigin.Current);
        }

        public long NextFreeRecord { get; }
        public long Offset { get; }
        public int Length { get; }
    }
}
