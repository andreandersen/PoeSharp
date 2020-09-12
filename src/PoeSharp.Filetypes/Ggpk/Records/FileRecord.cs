using System;
using System.IO;
using System.Runtime.InteropServices;

namespace PoeSharp.Filetypes.Ggpk.Records
{
    internal sealed class FileRecord : IRecord
    {
        public FileRecord(in Stream stream, in int length)
        {
            Offset = stream.Position - 8;
            Length = length;

            var nameLength = stream.ReadInt32();
            Span<byte> bytes = new byte[32 + (nameLength * 2)];
            stream.Read(bytes);

            Hash = bytes
                .Slice(0, 32)
                .ToArray();

            Name = bytes
                .Slice(32, bytes.Length - 34)
                .FromUnicodeBytesToUtf8()
                .ToArray();

            DataOffset = Offset + 12 + bytes.Length;
            DataLength = Length - 12 - bytes.Length;

            stream.Position += DataLength;
        }

        public long Offset { get; }
        public long DataOffset { get; }
        public int DataLength { get; }
        public int Length { get; }

        public ReadOnlyMemory<byte> Hash { get; }
        public ReadOnlyMemory<char> Name { get; }
    }
}
