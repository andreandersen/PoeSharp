﻿namespace PoeSharp.Filetypes.Ggpk.Records
{
    public sealed class DirectoryRecord : IRecord
    {
        internal DirectoryRecord(Stream stream, int length)
        {
            Offset = stream.Position - 8;

            Span<byte> bytes = new byte[length - 8];
            _ = stream.Read(bytes);

            var nameLength = bytes.Slice(0, 4).To<int>();
            var entriesLength = bytes.Slice(4, 4).To<int>();

            Hash = bytes.Slice(8, 32).ToArray();

            Span<char> name = stackalloc char[nameLength - 1];

            bytes.Slice(40, (nameLength - 1) * 2)
                .FromUnicodeBytesToUtf8()
                .CopyTo(name);

            Name = new string(name);

            var offset = 40 + nameLength * 2;

            var entries = new DirectoryEntry[entriesLength];

            for (var i = 0; i < entriesLength; i++)
            {
                var x = offset + i * 12;
                entries[i] = new DirectoryEntry
                (
                    bytes.Slice(x, sizeof(uint)).To<uint>(),
                    bytes.Slice(x + 4, sizeof(long)).To<long>()
                );
            }

            Entries = entries;
            Length = (int)(stream.Position - Offset);
        }

        public long Offset { get; }
        public int Length { get; }

        public DirectoryEntry[] Entries { get; }
        public byte[] Hash { get; }
        public string Name { get; }
    }
}