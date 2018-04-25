using System.IO;
using PoeSharp.Util;

namespace PoeSharp.Files.Ggpk.Records
{
    internal class FreeRecord : IRecord
    {
        internal FreeRecord(Stream stream, int length)
        {
            Length = length;
            Offset = stream.Position;
            NextFreeRecord = stream.ReadInt64();
            stream.Seek(length - 16, SeekOrigin.Current);
        }

        public long NextFreeRecord { get; }
        public long Offset { get; }
        public int Length { get; }
        public string Name => string.Empty;
    }
}