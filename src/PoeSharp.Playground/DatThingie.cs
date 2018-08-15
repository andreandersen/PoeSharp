using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Threading;
using PoeSharp.Files.Dat;
using PoeSharp.Files.Dat.Specification;
using PoeSharp.Files.Ggpk;
using PoeSharp.Metadata.Modifiers.Loaders;
using PoeSharp.Metadata.Stats.Loaders;
using PoeSharp.Shared.DataSources.Disk;

namespace PoeSharp.Playground
{
    public static class DatThingie
    {
        public static void GetThoseDats()
        {
            var path = @"C:\Program Files (x86)\Grinding Gear Games\Path of Exile\Content.ggpk";

            //var file = @"C:\ggpk2\Metadata\StatDescriptions\stat_descriptions.txt";

            var ggpk = new GgpkFileSystem(path);

            var datIndex = new DatFileIndex(ggpk,
                DetSpecificationIndex.Default);

            var stats = new StatsDatLoader().Load(datIndex);
            var mods = new ModifiersDatLoader(stats).Load(datIndex);

            var we = mods.SelectMany(c => c.Value.SpawnWeights.Select(e => e.TagId)).Distinct().ToList();

            Console.WriteLine("First 10 mods:");
            int i = 0;
            foreach (var row in datIndex["Mods.dat"])
            {
                Console.WriteLine(row["Name"]);
                if (++i == 10)
                    break;
            }

        }
    }
}
