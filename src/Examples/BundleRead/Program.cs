using System.Diagnostics;

using PoeSharp.Filetypes.Bundle;
using PoeSharp.Filetypes.Ggpk;

var sw = Stopwatch.StartNew();

Console.WriteLine("Opening bundle from GGPK...");
var bun = new GgpkFileSystem(@"D:\Games\Path of Exile\Content.ggpk")
    .Root.OpenBundleIndex();

var dir = bun.Root.Directories["Data"];

//var dir = new DiskDirectory(@"C:\noindex\export\Bundles2")
//    .OpenBundleIndex().Root.Directories["Data"];

Console.WriteLine($"Opened Bundle index. {sw.Elapsed}");
sw.Restart();

Console.Write("Extracting... ");

Parallel.ForEach(dir.Files.Values, (file) =>
{
    var content = file.AsSpan();
    using (var hand = File.OpenHandle(@"C:\noindex\data\" + file.Name,
        FileMode.Create, FileAccess.ReadWrite))
    {
        RandomAccess.Write(hand, content, 0);
    }
});

Console.WriteLine(sw.Elapsed);
Console.Write("Done.");

if (Debugger.IsAttached)
{
    Console.ReadKey(true);
    Console.WriteLine();
}