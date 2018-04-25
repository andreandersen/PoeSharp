using System.Collections.Generic;
using System.IO;
using PoeSharp.Util;

namespace PoeSharp.Files.Ggpk.Records
{
    internal class GgpkRecord : IRecord
    {
        private readonly List<long> _recordOffsets;

        internal GgpkRecord(Stream stream, int length)
        {
            _recordOffsets = new List<long>();

            Offset = stream.Position;
            Length = length;

            var records = stream.ReadInt32();
            for (var i = 0; i < records; i++)
            {
                var recordOffset = stream.ReadInt64() + 8;
                _recordOffsets.Add(recordOffset);
            }

            Name = "GGPK Root";
        }

        internal IReadOnlyList<long> RecordOffsets => _recordOffsets;

        public long Offset { get; }
        public int Length { get; }
        public string Name { get; }
    }
}