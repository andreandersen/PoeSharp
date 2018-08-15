using System;
using System.Collections.Generic;
using System.IO;
using PoeSharp.Util;

namespace PoeSharp.Files.Ggpk.Records
{
    internal sealed class DirectoryRecord : IRecord
    {
        internal DirectoryRecord(Stream stream, int length)
        {
            Offset = stream.Position;

            Span<byte> buffer = stackalloc byte[length - 8];
            var read = stream.Read(buffer);

            if (read < buffer.Length)
                throw new IOException("Read too little");

            var nameLength = buffer.Slice(0, 4).To<int>();
            var entriesLength = buffer.Slice(4, 4).To<int>();

            Hash = buffer.Slice(8, 32).ToArray();
            Name = buffer.Slice(40, (nameLength - 1) * 2).ToUnicodeText();

            var offset = 40 + nameLength * 2;

            Entries = new DirectoryEntry[entriesLength];

            for (var i = 0; i < entriesLength; i++)
            {
                var x = offset + i * 12;
                Entries[i] = new DirectoryEntry
                (
                    buffer.Slice(x, sizeof(uint)).To<uint>(),
                    buffer.Slice(x + 4, sizeof(long)).To<long>() + 8
                );
            }

            Length = (int)(stream.Position - Offset);
        }

        public DirectoryEntry[] Entries { get; }
        public byte[] Hash { get; }
        public int Length { get; }
        public string Name { get; }
        public long Offset { get; }

        public readonly struct DirectoryEntry : IEquatable<DirectoryEntry>
        {
            public DirectoryEntry(uint hash, long offset)
            {
                Hash = hash;
                Offset = offset;
            }

            public uint Hash { get; }
            public long Offset { get; }

            public override bool Equals(object obj) => obj is DirectoryEntry && Equals((DirectoryEntry)obj);
            public bool Equals(DirectoryEntry other) => Hash == other.Hash && Offset == other.Offset;

            public override int GetHashCode()
            {
                var hashCode = 762427779;
                hashCode = hashCode * -1521134295 + Hash.GetHashCode();
                hashCode = hashCode * -1521134295 + Offset.GetHashCode();
                return hashCode;
            }

            public override string ToString() => $"{Offset,-10} | {Hash,-16}";

            public static bool operator ==(DirectoryEntry entry1, DirectoryEntry entry2) => entry1.Equals(entry2);
            public static bool operator !=(DirectoryEntry entry1, DirectoryEntry entry2) => !(entry1 == entry2);
        }
    }
}
