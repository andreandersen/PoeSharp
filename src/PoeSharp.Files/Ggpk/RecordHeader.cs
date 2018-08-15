using System.Runtime.InteropServices;
using PoeSharp.Files.Ggpk.Records;

namespace PoeSharp.Files.Ggpk
{
    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct RecordHeader
    {
        public readonly int Length;
        public readonly RecordType Type;
    }
}
