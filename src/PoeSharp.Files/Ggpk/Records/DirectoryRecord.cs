using System;
using System.Collections.Generic;
using System.IO;
using PoeSharp.Util;

namespace PoeSharp.Files.Ggpk.Records
{
    internal class DirectoryRecord : IRecord
    {
        internal DirectoryRecord(Stream stream, int length)
        {
            Entries = new List<DirectoryEntry>();
            Offset = stream.Position;

            var buffer = new byte[length - 8];
            stream.Read(buffer, 0, buffer.Length);
            var span = new Span<byte>(buffer);
            var nameLength = span.Slice(0, 4).To<int>();
            var entriesLength = span.Slice(4, 4).To<int>();

            Hash = span.Slice(8, 32).ToArray();
            Name = span.Slice(40, (nameLength - 1) * 2).ToUnicodeText();

            var offset = 40 + nameLength * 2;

            Span<DirectoryEntry> res = new DirectoryEntry[entriesLength];

            for (var i = 0; i < entriesLength; i++)
            {
                var x = offset + i * 12;
                res[i] = new DirectoryEntry
                {
                    Hash = span.Slice(x, sizeof(uint)).To<uint>(),
                    Offset = span.Slice(x + 4, sizeof(long)).To<long>() + 8
                };
            }

            Entries = new List<DirectoryEntry>(res.ToArray());
            Length = (int)(stream.Position - Offset);
        }

        public List<DirectoryEntry> Entries { get; }

        public struct DirectoryEntry
        {
            public uint Hash;
            public long Offset;

            public override string ToString() => $"{Offset,-10} | {Hash,-16}";
        }

        public byte[] Hash { get; }
        public long Offset { get; }
        public int Length { get; }
        public string Name { get; }
    }
}