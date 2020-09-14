using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

using PoeSharp.Filetypes.Bundle;

namespace BundleRead
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using var indexFs = 
                File.OpenRead(@"c:\noindex\ggpk3112\Bundles2\_.index.bin");

            var bundles = EncodedIndexBundle.EnumerateChunks(indexFs);

            foreach (var item in bundles)
            {
                var e = BitConverter.ToString(item.Slice(0, 8).ToArray());
                Console.WriteLine(e);
            }

            //await foreach (var item in bundles)
            //{
            //    var e = BitConverter.ToString(item.Slice(0, 8).ToArray());
            //    Console.WriteLine(e);
            //}
        }
    }

}
