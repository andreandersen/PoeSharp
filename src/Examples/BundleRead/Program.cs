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
            const string path = @"c:\noindex\ggpk3112\Bundles2\";

            using var indexFs = File.OpenRead($"{path}_.index.bin");

            var destDir = new DirectoryInfo(Path.Combine(path, "idxprep"));
            if (!destDir.Exists) destDir.Create();

            var chunks = EncodedIndexBundle.PrepareIndexForDecompression(indexFs);
            var i = 0;

            foreach (var chunk in chunks)
            {
                var chunkFile = Path.Combine(destDir.FullName, $"_.index.{i}.bin");
                File.WriteAllBytes(chunkFile, chunk.ToArray());
                i++;
            }

            Console.WriteLine(DateTime.Now);
        }
    }

}
