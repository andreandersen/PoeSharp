﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using PoeSharp.Filetypes.BuildingBlocks;
using PoeSharp.Filetypes.Dat;
using PoeSharp.Filetypes.Dat.Specification;

namespace DatRead
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //Thread.Sleep(1500);

            var datSpecIdx = DetSpecificationIndex.Default;
            DiskDirectory directory = @"C:\noindex\3124\Bundles2\Bundles2\Data";

            var dats = new DatFileIndex(directory, datSpecIdx, true);

            var sw = Stopwatch.StartNew();
            var dat = dats["BaseItemTypes.dat"];
            var row = dat[0];

            var elapsed = sw.Elapsed;

            Console.WriteLine(elapsed);
            //Task.Delay(2500).Wait();
            //Console.Read();
        }

        private static DatRow GetRows(DetSpecificationIndex datSpecIdx, DiskDirectory directory)
        {
            var dats = new DatFileIndex(directory, datSpecIdx, false);
            var dat = dats["BaseItemTypes.dat"];
            var row = dat[0];

            return row;
        }
    }
}