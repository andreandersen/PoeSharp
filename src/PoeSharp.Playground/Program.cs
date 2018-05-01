using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using PoeSharp.Files.Dat;
using PoeSharp.Files.Dat.Specification;
using PoeSharp.Files.Ggpk;
using PoeSharp.Files.Psg;
using PoeSharp.Metadata.Stats.Loaders;
using PoeSharp.Metadata.Translations.Loaders;
using PoeSharp.Metadata.Modifiers.Loaders;
using PoeSharp.Shared.DataSources.Disk;

namespace PoeSharp.Playground
{
    internal class Program
    {
        private static void Main(string[] args)
        {

            var sw = Stopwatch.StartNew();

            var ggpkPath =
                @"C:\Program Files (x86)\Grinding Gear Games\Path of Exile\Content.ggpk";


            Console.WriteLine("Reading PassiveSkillGraph.psg...");

            var psgFile = new DiskFile(@"C:\ggpk3\Metadata\PassiveSkillGraph.psg");

            var psg = new PsgFile(psgFile);

            sw.Stop();

            Console.WriteLine($"Parsed Passive Skill Graph in {sw.ElapsedMilliseconds}ms.\r\n");
            Console.WriteLine($"{psg.Groups.Count} groups, with {psg.Groups.Sum(c => c.Count)} nodes in total");

            //var file = @"C:\ggpk2\Metadata\StatDescriptions\stat_descriptions.txt";

            //var ggpk = new GgpkFileSystem(ggpkPath);

            //var file = ggpk.Directories
            //    .First(c => c.Name == "Metadata").Directories
            //    .First(c => c.Name == "StatDescriptions").Files
            //    .First(c => c.Name == "stat_descriptions.txt")
            //    .GetStream();

            //var datIndex = new DatFileIndex(ggpk,
            //    DetSpecificationIndex.Default);

            //var stats = new StatsDatLoader().Load(datIndex);
            //var mods = new ModifiersDatLoader(stats).Load(datIndex);

            //var transLoader = new TranslationsLoader(stats);
            //var transIndex = transLoader.Load(file);

            //var passiveSkills = datIndex.Where(c => c.Key.ToLower().Contains("tree")).ToList();

        }
    }
}
