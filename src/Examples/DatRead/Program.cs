using System;
using System.Diagnostics;

using PoeSharp.Filetypes.BuildingBlocks;
using PoeSharp.Filetypes.Dat;
using PoeSharp.Filetypes.Dat.Specification;

namespace DatRead
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var datSpecIdx = DetSpecificationIndex.Default;

            var idx = new DatFileIndex((DiskDirectory)@"C:\noindex\old\Data", datSpecIdx);

            var sw = Stopwatch.StartNew();
            
            var dat = idx["BaseItemTypes.dat"];
            var firstRow = dat[0];

            var elapsed = sw.Elapsed;

            Console.WriteLine(elapsed);

        }
    }
}
