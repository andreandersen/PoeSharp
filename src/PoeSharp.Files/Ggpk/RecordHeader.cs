using System.Runtime.InteropServices;
using PoeSharp.Files.Ggpk.Records;

namespace PoeSharp.Files.Ggpk
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct RecordHeader
    {
        internal int Length { get; set; }
        internal RecordType Type { get; set; }
    }
}