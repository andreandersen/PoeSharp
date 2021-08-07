using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

using PoeSharp.Filetypes.Bundle;

namespace TestingStuff
{
    internal static class Program
    {

        private static void Main(string[] args)
        {
            BenchmarkRunner.Run<Bench>();
            //var b = new Bench();
            
            //b.One();
            //b.One();
            //b.One();

            var a = 0;
        }
    }

    [MemoryDiagnoser]
    public class Bench
    {
        private readonly byte[] _cmr;
        public Bench()
        {
            const string path = @"c:\noindex\3124\Bundles2\";
            using var indexFs = File.OpenRead($"{path}_.index.bin");
            var cmrSpan = new Span<byte>(new byte[(int)indexFs.Length]);
            indexFs.Read(cmrSpan);
            _cmr = cmrSpan.ToArray();
        }

        [Benchmark]
        public void One()
        {
            const string path = @"c:\noindex\3124\Bundles2\";
            using var indexFs = File.OpenRead($"{path}_.index.bin");
            var cmrSpan = new Span<byte>(new byte[(int)indexFs.Length]);
            indexFs.Read(cmrSpan);

            //BundleIndex.D(cmrSpan);
        }
    }
}
