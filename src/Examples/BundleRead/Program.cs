using System;
using System.IO;
using System.Threading.Tasks;

using PoeSharp.Filetypes.Bundle;

namespace BundleRead
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            const string path = @"c:\noindex\new\Bundles2\";
            using var indexFs = File.OpenRead($"{path}_.index.bin");

            var destDir = new DirectoryInfo(Path.Combine(path, "idxprep"));
            if (!destDir.Exists) destDir.Create();

            var dst = File.OpenWrite(Path.Combine(destDir.FullName, "_.index.dec"));
            EncodedIndexBundle.PrepareIndexForDecompression2(indexFs, dst);
        }
    }

}
