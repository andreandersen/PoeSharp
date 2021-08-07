using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

using PoeSharp.Filetypes.Bundle;
using PoeSharp.Filetypes.Ggpk;

var sw = Stopwatch.StartNew();

var dir = new GgpkFileSystem(@"D:\Games\Path of Exile\Content.ggpk")
    .Directories["Bundles2"]
    .OpenBundleIndex()
    .Root.Directories["Data"];

Parallel.ForEach(dir.Files.Values, (file) =>
{
    var content = file.AsSpan();
    using (var hand = File.OpenHandle(@"C:\noindex\data\" + file.Name,
        FileMode.Create, FileAccess.ReadWrite))
    {
        RandomAccess.Write(hand, content, 0);
    }
    Console.WriteLine(file.Name);
});

Console.WriteLine(sw.Elapsed);
Console.Write("Done.");

if (Debugger.IsAttached)
{
    Console.ReadKey(true);
    Console.WriteLine();
}