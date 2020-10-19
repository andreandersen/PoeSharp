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
            
            const string datFile = "MonsterVarieties.dat";
            Console.Write($"Reading all rows in dat file {datFile}... ");
            var dat = dats[datFile];
            var c = dat.Count;

            var ret = new MonsterVarieties[c];

            for (var i = 0; i < c; i++)
            {
                var r = dat[i];
                ReadMonsterVarietiesRow(ret, i, r);
            }

            sw.ElapsedAndReset().PrintElapsedMs().AddTo(ref total);

            Console.Write("Done in ");
            total.PrintElapsedMs();

            Console.WriteLine($"Rows in {datFile}: {c}");

            for (var i = 0; i < 10; i++)
            {
                Console.WriteLine(ret[i].Name);
            }
        }

        // ---

        private static void ReadMonsterVarietiesRow(MonsterVarieties[] ret, int i, DatFile.DatRow r) => ret[i] = new MonsterVarieties
        {
            Id = r.GetString("Id"),
            MonsterTypesKey = r.GetPrimitive<ulong>("MonsterTypesKey"),
            Unknown0 = r.GetPrimitive<int>("Unknown0"),
            ObjectSize = r.GetPrimitive<int>("ObjectSize"),
            MinimumAttackDistance = r.GetPrimitive<int>("MinimumAttackDistance"),
            MaximumAttackDistance = r.GetPrimitive<int>("MaximumAttackDistance"),
            ActFiles = r.GetStringList("ACTFiles"),
            AoFiles = r.GetStringList("AOFiles"),
            BaseMonsterTypeIndex = r.GetString("BaseMonsterTypeIndex"),
            ModsKeys = r.GetPrimitiveList<ulong>("ModsKeys"),
            Unknown14 = r.GetPrimitive<int>("Unknown14"),
            Unknown15 = r.GetPrimitive<int>("Unknown15"),
            Unknown16 = r.GetPrimitive<int>("Unknown16"),
            ModelSizeMultiplier = r.GetPrimitive<int>("ModelSizeMultiplier"),
            Unknown18 = r.GetPrimitive<int>("Unknown18"),
            Unknown19 = r.GetPrimitive<int>("Unknown19"),
            Unknown20 = r.GetPrimitive<int>("Unknown20"),
            Unknown21 = r.GetPrimitive<int>("Unknown21"),
            Unknown22 = r.GetPrimitive<int>("Unknown22"),
            TagsKeys = r.GetPrimitiveList<ulong>("TagsKeys"),
            ExperienceMultiplier = r.GetPrimitive<int>("ExperienceMultiplier"),
            Unknown7 = r.GetPrimitive<int>("Unknown7"),
            Unknown28 = r.GetPrimitive<int>("Unknown28"),
            Unknown29 = r.GetPrimitive<int>("Unknown29"),
            Unknown30 = r.GetPrimitive<int>("Unknown30"),
            CriticalStrikeChance = r.GetPrimitive<int>("CriticalStrikeChance"),
            Unknown32 = r.GetPrimitive<int>("Unknown32"),
            GrantedEffectsKeys = r.GetPrimitiveList<ulong>("GrantedEffectsKeys"),
            AisFile = r.GetString("AISFile"),
            ModsKeys2 = r.GetPrimitiveList<ulong>("ModsKeys2"),
            Stance = r.GetString("Stance"),
            Key2 = r.GetPrimitive<ulong>("Key2"),
            Name = r.GetString("Name"),
            DamageMultiplier = r.GetPrimitive<int>("DamageMultiplier"),
            LifeMultiplier = r.GetPrimitive<int>("LifeMultiplier"),
            AttackSpeed = r.GetPrimitive<int>("AttackSpeed"),
            Weapon1ItemVisualIdentityKeys = r.GetPrimitiveList<ulong>("Weapon1_ItemVisualIdentityKeys"),
            Weapon2ItemVisualIdentityKeys = r.GetPrimitiveList<ulong>("Weapon2_ItemVisualIdentityKeys"),
            BackItemVisualIdentityKey = r.GetPrimitive<ulong>("Back_ItemVisualIdentityKey"),
            MainHandItemClassesKey = r.GetPrimitive<ulong>("MainHand_ItemClassesKey"),
            OffHandItemClassesKey = r.GetPrimitive<ulong>("OffHand_ItemClassesKey"),
            Key1 = r.GetPrimitive<ulong>("Key1"),
            HelmetItemVisualIdentityKey = r.GetPrimitive<ulong>("Helmet_ItemVisualIdentityKey"),
            Unknown59 = r.GetPrimitive<int>("Unknown59"),
            KillSpecificMonsterCountAchievementItemsKeys = r.GetPrimitiveList<ulong>("KillSpecificMonsterCount_AchievementItemsKeys"),
            SpecialModsKeys = r.GetPrimitiveList<ulong>("Special_ModsKeys"),
            KillRareAchievementItemsKeys = r.GetPrimitiveList<ulong>("KillRare_AchievementItemsKeys"),
            Flag0 = r.GetPrimitive<bool>("Flag0"),
            Unknown66 = r.GetPrimitive<int>("Unknown66"),
            Unknwon67 = r.GetPrimitive<int>("Unknown67"),
            Unknown68 = r.GetPrimitive<int>("Unknown68"),
            Unknown69 = r.GetPrimitive<int>("Unknown69"),
            Unknown70 = r.GetPrimitive<int>("Unknown70"),
            Unknown71 = r.GetPrimitive<int>("Unknown71"),
            Unknown72 = r.GetPrimitive<int>("Unknown72"),
            Flag1 = r.GetPrimitive<byte>("Flag1"),
            Unknown73 = r.GetPrimitive<int>("Unknown73"),
            KillWhileOnslaughtIsActiveAchivementItemsKey = r.GetPrimitive<ulong>("KillWhileOnslaughtIsActive_AchievementItemsKey"),
            MonsterSegmentsKey = r.GetPrimitive<ulong>("MonsterSegmentsKey"),
            MonsterArmoursKey = r.GetPrimitive<ulong>("MonsterArmoursKey"),
            KillWhileTalismanIsActiveAchivementItemsKey = r.GetPrimitive<ulong>("KillWhileTalismanIsActive_AchievementItemsKey"),
            Part1ModsKeys = r.GetPrimitiveList<ulong>("Part1_ModsKeys"),
            Part2ModsKeys = r.GetPrimitiveList<ulong>("Part2_ModsKeys"),
            EndgameModsKeys = r.GetPrimitiveList<ulong>("Endgame_ModsKeys"),
            Key0 = r.GetPrimitive<ulong>("Key0"),
            Unknown90 = r.GetPrimitive<int>("Unknown90"),
            Unknown91 = r.GetPrimitive<int>("Unknown91"),
            Keys0 = r.GetPrimitiveList<ulong>("Keys0"),
            Keys1 = r.GetPrimitiveList<ulong>("Keys1"),
            Unknown96 = r.GetPrimitive<int>("Unknown96"),
            SinkAnimationAoFile = r.GetString("SinkAnimation_AOFile"),
            Flag2 = r.GetPrimitive<byte>("Flag2"),
            Keys2 = r.GetPrimitiveList<ulong>("Keys2"),
            Flag3 = r.GetPrimitive<byte>("Flag3"),
            Flag4 = r.GetPrimitive<byte>("Flag4"),
            Flag5 = r.GetPrimitive<byte>("Flag5"),
            Unknown100 = r.GetPrimitive<int>("Unknown100"),
            Unknown101 = r.GetPrimitive<int>("Unknown101"),
            Unknown102 = r.GetPrimitive<int>("Unknown102"),
            Unknown103 = r.GetPrimitive<int>("Unknown103"),
            Unknown104 = r.GetPrimitive<int>("Unknown104"),
            Unknown105 = r.GetPrimitive<int>("Unknown105"),
            Key3 = r.GetPrimitive<ulong>("Key3")
        };
    }

    public class MonsterVarieties
    {
        public string Id { get; set; }
        public ulong MonsterTypesKey { get; set; }
        public int Unknown0 { get; set; }
        public int ObjectSize { get; set; }
        public int MinimumAttackDistance { get; set; }
        public int MaximumAttackDistance { get; set; }
        public string[] ActFiles { get; set; }
        public string[] AoFiles { get; set; }
        public string BaseMonsterTypeIndex { get; set; }
        public ulong[] ModsKeys { get; set; }
        public int Unknown14 { get; set; }
        public int Unknown15 { get; set; }
        public int Unknown16 { get; set; }
        public int ModelSizeMultiplier { get; set; }
        public int Unknown18 { get; set; }
        public int Unknown19 { get; set; }
        public int Unknown20 { get; set; }
        public int Unknown21 { get; set; }
        public int Unknown22 { get; set; }
        public ulong[] TagsKeys { get; set; }
        public int ExperienceMultiplier { get; set; }
        public int Unknown7 { get; set; }
        public int Unknown28 { get; set; }
        public int Unknown29 { get; set; }
        public int Unknown30 { get; set; }
        public int CriticalStrikeChance { get; set; }
        public int Unknown32 { get; set; }
        public ulong[] GrantedEffectsKeys { get; set; }
        public string AisFile { get; set; }
        public ulong[] ModsKeys2 { get; set; }
        public string Stance { get; set; }
        public ulong Key2 { get; set; }
        public string Name { get; set; }
        public int DamageMultiplier { get; set; }
        public int LifeMultiplier { get; set; }
        public int AttackSpeed { get; set; }
        public ulong[] Weapon1ItemVisualIdentityKeys { get; set; }
        public ulong[] Weapon2ItemVisualIdentityKeys { get; set; }
        public ulong BackItemVisualIdentityKey { get; set; }
        public ulong MainHandItemClassesKey { get; set; }
        public ulong OffHandItemClassesKey { get; set; }
        public ulong Key1 { get; set; }
        public ulong HelmetItemVisualIdentityKey { get; set; }
        public int Unknown59 { get; set; }
        public ulong[] KillSpecificMonsterCountAchievementItemsKeys { get; set; }
        public ulong[] SpecialModsKeys { get; set; }
        public ulong[] KillRareAchievementItemsKeys { get; set; }
        public bool Flag0 { get; set; }
        public int Unknown66 { get; set; }
        public int Unknwon67 { get; set; }
        public int Unknown68 { get; set; }
        public int Unknown69 { get; set; }
        public int Unknown70 { get; set; }
        public int Unknown71 { get; set; }
        public int Unknown72 { get; set; }
        public byte Flag1 { get; set; }
        public int Unknown73 { get; set; }
        public ulong KillWhileOnslaughtIsActiveAchivementItemsKey { get; set; }
        public ulong MonsterSegmentsKey { get; set; }
        public ulong MonsterArmoursKey { get; set; }
        public ulong KillWhileTalismanIsActiveAchivementItemsKey { get; set; }
        public ulong[] Part1ModsKeys { get; set; }
        public ulong[] Part2ModsKeys { get; set; }
        public ulong[] EndgameModsKeys { get; set; }
        public ulong Key0 { get; set; }
        public int Unknown90 { get; set; }
        public int Unknown91 { get; set; }
        public ulong[] Keys0 { get; set; }
        public ulong[] Keys1 { get; set; }
        public int Unknown96 { get; set; }
        public string SinkAnimationAoFile { get; set; }
        public byte Flag2 { get; set; }
        public ulong[] Keys2 { get; set; }
        public byte Flag3 { get; set; }
        public byte Flag4 { get; set; }
        public byte Flag5 { get; set; }
        public int Unknown100 { get; set; }
        public int Unknown101 { get; set; }
        public int Unknown102 { get; set; }
        public int Unknown103 { get; set; }
        public int Unknown104 { get; set; }
        public int Unknown105 { get; set; }
        public ulong Key3 { get; set; }
    }
}
