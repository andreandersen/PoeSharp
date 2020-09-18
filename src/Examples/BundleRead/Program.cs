using System;
using System.Buffers;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

using PoeSharp.Filetypes.Bundle;

namespace BundleRead
{
    internal class Program
    {
        #pragma warning disable IDE0060 // Remove unused parameter
        private static void Main(string[] args)
        #pragma warning restore IDE0060 // Remove unused parameter
        {
            var sw = Stopwatch.StartNew();
            const string path = @"c:\noindex\312\Bundles2\";
            using var indexFs = File.OpenRead($"{path}_.index.bin");

            var destDir = new DirectoryInfo(Path.Combine(path, "idxprep"));
            if (!destDir.Exists) destDir.Create();

            var destFile = Path.Combine(destDir.FullName, "_.index.dec");

            if (File.Exists(destFile))
                File.Delete(destFile);

            var dst = File.OpenWrite(destFile);

            //var dst = indexFs.ReadAndDecompressBundle();

            EncodedBundle.DecompressToStream(indexFs, dst);

            //BundleIndex.C(dst);

            Console.WriteLine(sw.Elapsed);
        }
    }

}
