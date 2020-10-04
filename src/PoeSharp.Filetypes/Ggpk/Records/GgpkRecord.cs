using System;
using System.IO;

using Microsoft.Toolkit.HighPerformance.Extensions;

namespace PoeSharp.Filetypes.Ggpk.Records
{
    internal sealed class GgpkRecord : IRecord
    {
        internal GgpkRecord(Stream fileStream, in int length)
        {
            Offset = fileStream.Position - 8;
            Length = length;

            var numberOfRecords = fileStream.Read<int>();
            var recordOffsets = new long[numberOfRecords];

            for (int i = 0; i < numberOfRecords; i++)
            {
                recordOffsets[i] = fileStream.Read<long>();
            }

            RecordOffsets = recordOffsets;
        }

        public long Offset { get; }
        public int Length { get; }
        public ReadOnlyMemory<char> Name => Memory<char>.Empty;
        public Memory<long> RecordOffsets { get; }
    }
}
