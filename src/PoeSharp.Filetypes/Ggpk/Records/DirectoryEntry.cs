using System;
using System.Runtime.InteropServices;

namespace PoeSharp.Filetypes.Ggpk.Records
{
    [StructLayout(LayoutKind.Auto, Pack = 0)]
    internal readonly struct DirectoryEntry
    {
        internal DirectoryEntry(in uint hash, long offset)
        {
            Hash = hash;
            Offset = offset;
        }

        public long Offset { get; }
        public uint Hash { get; }
    }
}
