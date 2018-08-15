using System.Collections.Generic;
using System.Runtime.CompilerServices;
using PoeSharp.Files.Ggpk.Records;

namespace PoeSharp.Files.Ggpk
{
    public static class GgpkRecordExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static IEnumerable<IRecord> ToRecords(
            this IEnumerable<DirectoryRecord.DirectoryEntry> entries,
            Dictionary<long, IRecord> ggpkRecords)
        {
            foreach (DirectoryRecord.DirectoryEntry entry in entries)
            {
                yield return ggpkRecords[entry.Offset];
            }
        }
    }
}
