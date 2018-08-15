using System.IO;
using PoeSharp.Util;

namespace PoeSharp.Files.Ggpk.Records
{
    internal readonly struct GgpkRecord : IRecord
    {
        internal GgpkRecord(Stream stream, int length)
        {
            Offset = stream.Position;
            Length = length;

            var records = stream.ReadInt32();
            RecordOffsets = new long[records];

            for (var i = 0; i < records; i++)
            {
                var recordOffset = stream.ReadInt64() + 8;
                RecordOffsets[i] = recordOffset;
            }

            Name = "GGPK Root";
        }

        public int Length { get; }
        public string Name { get; }
        public long Offset { get; }
        public long[] RecordOffsets { get; }
    }
}
