using System;
using System.Buffers;
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
            const string path = @"c:\noindex\new\Bundles2\";
            using var indexFs = File.OpenRead($"{path}_.index.bin");

            var destDir = new DirectoryInfo(Path.Combine(path, "idxprep"));
            if (!destDir.Exists) destDir.Create();

            var dst = File.OpenWrite(Path.Combine(destDir.FullName, "_.index.dec"));
            EncodedBundle.DecompressToStream(indexFs, dst);


        }
    }

}
