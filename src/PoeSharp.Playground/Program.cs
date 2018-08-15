using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using PoeSharp.Files.Dat;
using PoeSharp.Files.Dat.Specification;
using PoeSharp.Files.Ggpk;
using PoeSharp.Files.Psg;
using PoeSharp.Metadata.Stats.Loaders;
using PoeSharp.Metadata.Translations.Loaders;
using PoeSharp.Metadata.Modifiers.Loaders;
using PoeSharp.Metadata.SkillTree.Loader;
using PoeSharp.Shared.DataSources.Disk;

namespace PoeSharp.Playground
{
    internal class Program
    {
        const string GgpkPath =
            @"C:\Program Files (x86)\Grinding Gear Games\Path of Exile\Content.ggpk";

        private static void Main(string[] args)
        {
            GgpkExtractor.ExtractGggpk(GgpkPath, "C:\\noindex\\ggpk45");
        }

        private static void Translations()
        {
            var ggpk = new GgpkFileSystem(GgpkPath);

            var file = ggpk.Directories
                .First(c => c.Name == "Metadata").Directories
                .First(c => c.Name == "StatDescriptions").Files
                .First(c => c.Name == "stat_descriptions.txt")
                .GetStream();

            var datIndex = new DatFileIndex(ggpk,
                DetSpecificationIndex.Default);

            var stats = new StatsDatLoader().Load(datIndex);
            var mods = new ModifiersDatLoader(stats).Load(datIndex);

            var transLoader = new TranslationsLoader(stats);
            var transIndex = transLoader.Load(file);

            var passiveSkills = datIndex.Where(c => c.Key.ToLower().Contains("tree")).ToList();
        }

        private static void PsgExtract()
        {
            var sw = Stopwatch.StartNew();

            Console.WriteLine("Reading PassiveSkillGraph.psg...");

            var psgFile = new DiskFile(@"C:\ggpk3\Metadata\PassiveSkillGraph.psg");
            var diskDirectory = new DiskDirectory(@"C:\ggpk3\Data\");
            var datIndex = new DatFileIndex(diskDirectory, DetSpecificationIndex.Default);

            var stats = new StatsDatLoader().Load(datIndex);
            var mods = new ModifiersDatLoader(stats).Load(datIndex);
            var passiveTree = new SkillTreeLoader(stats, datIndex);

            var psg = new PsgFile(psgFile);
            var result = passiveTree.Load(psg);

            sw.Stop();

            Console.WriteLine($"Parsed Passive Skill Graph in {sw.ElapsedMilliseconds}ms.\r\n");
            Console.WriteLine($"{psg.Groups.Count} groups, with {psg.Groups.Sum(c => c.Count)} nodes in total");
        }
    }
}
