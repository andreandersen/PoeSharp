using System;
using System.Diagnostics;
using PoeSharp.Files.Ggpk;
using PoeSharp.Shared.DataSources.Disk;

namespace PoeSharp.Playground
{
    public class GgpkExtractor
    {
        public static void ExtractGggpk(string ggpkPath, string destinationPath)
        {
            Console.CursorVisible = false;
            Console.WriteLine("Reading GGPK File...");
            var sw = Stopwatch.StartNew();

            using (var ggpk = new GgpkFileSystem(ggpkPath))
            {
                Console.WriteLine($"GGPK File parsed... {sw.Elapsed.TotalMilliseconds:N} ms");
                Console.WriteLine("Extracting GGPK Contents...");

                var destinationFolder = new DiskDirectory(destinationPath, null, true);
                ggpk.CopyTo(destinationFolder);

                Console.WriteLine($"Elapsed total time: {sw.Elapsed.TotalMilliseconds:N} ms");
                Console.WriteLine($"{Environment.NewLine}{Environment.NewLine}Done...");
                Console.CursorVisible = true;
            }
        }
    }
}
