using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using PoeSharp.Files.Ggpk.Records;
using PoeSharp.Shared.DataSources;

namespace PoeSharp.Files.Ggpk
{
    public static class GgpkRecordExtensions
    {
        internal static IEnumerable<IRecord> ToRecords(
            this IEnumerable<DirectoryRecord.DirectoryEntry> entries,
            Dictionary<long, IRecord> ggpkRecords)
        {
            return entries.Select(entry => ggpkRecords[entry.Offset]);
        }

        public static string GetPathString(this IDirectory directory)
        {
            var sb = new StringBuilder();
            var current = directory;
            while (current?.Parent != null)
            {
                sb.Insert(0, $"{current.Name}{Path.DirectorySeparatorChar}");
                current = current.Parent;
            }

            return sb.ToString();
        }
    }
}