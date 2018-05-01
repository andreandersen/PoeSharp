using System;
using System.Diagnostics;
using PoeSharp.Files.Ggpk;
using PoeSharp.Shared.DataSources.Disk;

namespace PoeSharp.Playground
{
    public class GgpkExtractor
    {
        private const string Filename =
            @"C:\Program Files (x86)\Grinding Gear Games\Path of Exile\Content.ggpk";

        public static void ExtractGggpk(params string[] args)
        {
            Console.CursorVisible = false;
            Console.WriteLine("Reading GGPK File...");
            var sw = Stopwatch.StartNew();

            using (var ggpk = new GgpkFileSystem(Filename))
            {
                Console.WriteLine($"GGPK File parsed... {sw.Elapsed.TotalMilliseconds:N} ms");

                var basePath = @"c:\ggpk3";

                if (args.Length > 0)
                    basePath = args[0];

                Console.WriteLine("Extracting GGPK Contents...");

                var destinationFolder = new DiskDirectory(basePath, null, true);
                ggpk.CopyTo(destinationFolder);

                Console.WriteLine($"Elapsed total time: {sw.Elapsed.TotalMilliseconds:N} ms");
                Console.WriteLine($"{Environment.NewLine}{Environment.NewLine}Done...");
                Console.CursorVisible = true;
            }
        }
    }
}
