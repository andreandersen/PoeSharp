using System;
using System.IO;
using System.Runtime.InteropServices;

namespace PoeSharp.Filetypes.Ggpk.Records
{
    internal sealed class GgpkRecord : IRecord
    {
        internal GgpkRecord(Stream fileStream, in int length)
        {
            Offset = fileStream.Position - 8;
            Length = length;

            var numberOfRecords = fileStream.ReadInt32();
            var recordOffsets = new long[numberOfRecords];

            for (int i = 0; i < numberOfRecords; i++)
            {
                recordOffsets[i] = fileStream.ReadInt64();
            }

            RecordOffsets = recordOffsets;
        }

        public long Offset { get; }
        public int Length { get; }
        public ReadOnlyMemory<byte> Name => Memory<byte>.Empty;
        public Memory<long> RecordOffsets { get; }
    }
}
