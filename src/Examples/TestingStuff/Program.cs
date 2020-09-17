using System;
using System.Buffers;
using System.IO;
using System.Linq;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Diagnostics;
using BenchmarkDotNet.Diagnostics.Windows;
using BenchmarkDotNet.Diagnostics.Windows.Configs;
using BenchmarkDotNet.Running;

using Microsoft.CodeAnalysis.Diagnostics;

using PoeSharp.Filetypes;
using PoeSharp.Filetypes.Bundle;

namespace TestingStuff
{
    class Program
    {
#pragma warning disable IDE0060 // Remove unused parameter
        static void Main(string[] args)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            BenchmarkRunner.Run<Bench>();
        }
    }

    [SimpleJob(launchCount: 1, warmupCount: 1, targetCount: 1), MemoryDiagnoser]
    [RPlotExporter]
    [BenchmarkDotNet.Attributes.DisassemblyDiagnoser]
    [BenchmarkDotNet.Diagnostics.Windows.Configs.NativeMemoryProfiler]
    [BenchmarkDotNet.Diagnostics.Windows.Configs.TailCallDiagnoser]
    [BenchmarkDotNet.Diagnostics.Windows.Configs.ConcurrencyVisualizerProfiler]
    public class Bench
    {
        private const int MaxChunkSize = 256 * 1024;
        ReadOnlyMemory<byte> _bytesToTest;
        MemoryStream src;

        [GlobalSetup]
        public void BenchSetup()
        {
            _bytesToTest = System.IO.File.ReadAllBytes(
                @"C:\noindex\new\Bundles2\_.index.bin");
            src = new(_bytesToTest.ToArray());
        }

        //[IterationCleanup]
        //public void IterCleanUp() => src.Position = 0;


        [Benchmark]
        public EncodedBundleHeader TestOne()
        {
            src.Position = 0;

            var hdr = src.Read<EncodedBundleHeader>();
            var count = hdr.EntryCount;
            var lastEntry = count - 1;
            var sizes = src.Read<uint>((int)count).ToArray();

            for (var i = 0; i < sizes.Length; i++)
            {
                var sz = (int)sizes[i];
                var decSize = (int)(i == lastEntry ? hdr.UncompressedSize - (MaxChunkSize * lastEntry) : MaxChunkSize);

                using var bufRent = MemoryPool<byte>.Shared.Rent(sz);
                var buf = bufRent.Memory.Span.Slice(0, sz);
                src.Read(buf);

                using var dstRent = MemoryPool<byte>.Shared.Rent(decSize + 64);
                var dstArr = dstRent.Memory.Span.Slice(0, decSize + 64);

                var dst = dstArr.Slice(0, decSize);
            }

            return hdr;
        }

        [Benchmark]
        public EncodedBundleHeader TestTwo()
        {
            src.Position = 0;

            var hdr = src.Read<EncodedBundleHeader>();
            var count = hdr.EntryCount;
            var lastEntry = count - 1;
            var sizes = src.Read<uint>((int)count).ToArray();

            var max = (int)sizes.Max();

            Span<byte> buf = stackalloc byte[max];
            Span<byte> dstArr = stackalloc byte[(256 * 1024)+64];
            for (var i = 0; i < sizes.Length; i++)
            {
                var sz = (int)sizes[i];
                var decSize = (int)(i == lastEntry ? hdr.UncompressedSize - (MaxChunkSize * lastEntry) : MaxChunkSize);

                src.Read(buf);
                var dst = dstArr.Slice(0, decSize);
            }

            return hdr;
        }

        public static void Throw() => throw new Exception("Boo");
    }
}
