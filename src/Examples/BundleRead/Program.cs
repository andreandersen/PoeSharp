using System;
using System.Diagnostics;
using System.IO;

using PoeSharp.Filetypes.Bundle;

var sw = Stopwatch.StartNew();
const string path = @"c:\noindex\new\Bundles2\";
using var indexFs = File.OpenRead($"{path}_.index.bin");

//var destDir = new DirectoryInfo(Path.Combine(path, "idxprep"));
//if (!destDir.Exists) destDir.Create();

//var destFile = Path.Combine(destDir.FullName, "_.index.dec");

//if (File.Exists(destFile))
//    File.Delete(destFile);

//var dst = File.OpenWrite(destFile);

var dst = indexFs.DecompressBundleToStream();

//EncodedBundle.DecompressToStream(indexFs, dst);

BundleIndex.C(dst);

Console.WriteLine(sw.Elapsed);
Console.WriteLine("Done");