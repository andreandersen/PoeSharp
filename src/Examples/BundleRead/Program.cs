using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

using PoeSharp.Filetypes.Bundle;
using PoeSharp.Filetypes.Bundle.Internal;

namespace BundleRead
{
    class Program
    {
        static async Task Main(string[] args)
        {
            const string path = @"c:\noindex\ggpk3112\Bundles2\";

            using var indexFs = 
                File.OpenRead($"{path}_.index.bin");


            Console.WriteLine(Unsafe.SizeOf<IndexBin>());
            Console.WriteLine(Unsafe.SizeOf<IndexBinHead>());
            Console.WriteLine(Unsafe.SizeOf<IndexBinHead2>());

            var bundles = EncodedIndexBundle.EnumerateChunks(indexFs).ToArray();

            //int i = 1;
            //foreach (var item in bundles)
            //{
            //    File.WriteAllBytes($"{path}idx\\{i}.bin", item.ToArray());
            //    var e = BitConverter.ToString(item.Slice(0, 8).ToArray());
            //    Console.WriteLine(e);
            //}

            //await foreach (var item in bundles)
            //{
            //    var e = BitConverter.ToString(item.Slice(0, 8).ToArray());
            //    Console.WriteLine(e);
            //}
        }
    }

}
