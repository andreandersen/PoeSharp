using System;
using System.IO;
using PoeSharp.Util;

namespace PoeSharp.Files.Ggpk.Records
{
    internal class FileRecord : IRecord
    {
        internal FileRecord(Stream stream, int length)
        {
            Offset = stream.Position;
            Length = length;
            var nameLength = stream.ReadInt32() * 2;

            var buffer = new byte[32 + nameLength];
            stream.Read(buffer, 0, buffer.Length);

            var span = new Span<byte>(buffer);

            Hash = span.Slice(0, 32).ToArray();
            Name = span.Slice(32, buffer.Length - 34).ToUnicodeText();

            DataOffset = stream.Position;

            DataLength = length - 44 - nameLength;
            stream.Position = Offset + length - 8;
        }

        public long DataOffset { get; }
        public int DataLength { get; }
        public byte[] Hash { get; }
        public long Offset { get; }
        public int Length { get; }
        public string Name { get; }
    }
}