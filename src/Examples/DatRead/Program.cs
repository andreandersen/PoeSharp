using System;
using System.Diagnostics;

using PoeSharp.Filetypes.BuildingBlocks;
using PoeSharp.Filetypes.Dat;
using PoeSharp.Filetypes.Dat.Specification;

namespace DatRead
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            DiskDirectory pathToDatFiles = @"C:\noindex\3124\Bundles2\Bundles2\Data";

            TimeSpan total = default;
            var sw = Stopwatch.StartNew();

            Console.Write("Loading specification... ");
            var datSpecIdx = DatSpecIndex.Default;
            sw.ElapsedAndReset().PrintElapsedMs().AddTo(ref total);

            Console.Write("Enumerating dat files... ");
            var dats = new DatFileIndex(pathToDatFiles, datSpecIdx, true);

            sw.ElapsedAndReset().PrintElapsedMs().AddTo(ref total);

            const string datFile = "AchievementItems.dat";
            Console.Write($"Reading all rows in dat file {datFile}... ");
            var dat = dats[datFile];
            var c = dat.Count;

            var ret = new AchievementItem[c];
            for (var i = 0; i < c; i++)
            {
                var r = dat[i];

                var achOk = r.TryGetReferencedDatValue("AchievementsKey", out var ach);
                var ai = new AchievementItem
                {
                    Achievement = new Achievement
                    {
                        Id = ach.GetString("Id"),
                        Description = ach.GetString("Description"),
                        Objective = ach.GetString("Objective")
                    },
                    Id = r.GetString("Id"),
                    CompletionsRequired = r.GetPrimitive<int>("CompletionsRequired"),
                    Name = r.GetString("Name")
                };

                ret[i] = ai;
            }

            sw.ElapsedAndReset().PrintElapsedMs().AddTo(ref total);

            Console.Write("Done in ");
            total.PrintElapsedMs();

            Console.WriteLine($"Rows in {datFile}: {c}");

            for (var i = 0; i < 10; i++)
            {
                Console.WriteLine($"{ret[i].Id,-20} {ret[i].Name}");
                Console.WriteLine($" Achievement: {ret[i].Achievement.Description}");
            }
        }

        public class AchievementItem
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public Achievement Achievement { get; set; }
            public int CompletionsRequired { get; set; }
        }

        public class Achievement
        {
            public string Id { get; set; }
            public string Description { get; set; }
            public string Objective { get; set; }
        }

    }


}
