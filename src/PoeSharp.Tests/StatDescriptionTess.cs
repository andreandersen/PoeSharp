using PoeSharp.Files.StatDescriptions;
using PoeSharp.Files.StatDescriptions.SyntaxTree;
using System.Linq;
using Xunit;

namespace PoeSharp.Tests
{
    [Collection("Stat Descriptions Parser")]
    public class StatDescriptionTests
    {
        [Fact]
        public void Include_Parses()
        {
            var actual = "Metadata/blabla";
            Include e = (Include)StatDescriptionParser.ParseInclude($"include \"{actual}\"");
            Assert.Equal(actual, e.File);
        }

        [Fact]
        public void Range_Parses()
        {
            var str = @"#|-1 ""This Attack and Minions have % 1 %%% reduced Attack Speed"" negate 1";
            var res = StatDescriptionParser.ParseVariation(str);
            Assert.Equal("#|-1", res.RangeText);
            Assert.Equal("This Attack and Minions have % 1 %%% reduced Attack Speed", res.TranslationTemplate);
            Assert.Equal("negate 1", res.ExtraInstructions);
        }

        [Fact]
        public void Params_Parses()
        {
            var str = "2 base_chance_to_freeze_% always_freeze";
            var res = StatDescriptionParser.ParseStats(str).ToList();
            Assert.Equal(2, res.Count);
            Assert.Equal("base_chance_to_freeze_%", res[0]);
            Assert.Equal("always_freeze", res[1]);
        }

        [Fact]
        public void Language_Parses()
        {
            var str = @"	2
		1|# ""%1%%% increased Quantity of Items Dropped by enemies Slain by Minions or This Attack""
		#|-1 ""%1%%% reduced Quantity of Items Dropped by enemies Slain by Minions or This Attack"" negate 1 bladi 2";

            var res = StatDescriptionParser.ParseLanguage(str);
            Assert.Equal(2, res.Variations.Count);

            Assert.Equal("%1%%% increased Quantity of Items Dropped by enemies Slain by Minions or This Attack", res.Variations[0].TranslationTemplate);
            Assert.Equal("1|#", res.Variations[0].RangeText);
            Assert.Equal("", res.Variations[0].ExtraInstructions);

            Assert.Equal("%1%%% reduced Quantity of Items Dropped by enemies Slain by Minions or This Attack", res.Variations[1].TranslationTemplate);
            Assert.Equal("#|-1", res.Variations[1].RangeText);
            Assert.Equal("negate 1 bladi 2", res.Variations[1].ExtraInstructions);

            Assert.Equal("English", res.Name);
        }

        [Fact]
        public void Description_Parses()
        {
            var str = @"description dklwqmdlkw
	1 base_poison_damage_+%
	2
		1|# ""This Attack and Minions have %1%%% increased Damage with Poison"" reminderstring ReminderTextPoison
		#|-1 ""This Attack and Minions have %1%%% reduced Damage with Poison"" reminderstring ReminderTextPoison
	lang ""Portuguese""
	2
		1|# ""Este Ataque e Lacaios têm %1%%% de Dano com Veneno aumentado"" reminderstring ReminderTextPoison
		#|-1 ""Este Ataque e Lacaios têm %1%%% de Dano com Veneno reduzido"" reminderstring ReminderTextPoison";

            Description res = (Description)StatDescriptionParser.ParseDescription(str);
            Assert.Equal(2, res.Languages.Count);
            Assert.Equal(1, res.Stats.Count);
            Assert.Equal("dklwqmdlkw", res.Name);

            str = @"description
	1 base_poison_damage_+%
	2
		1|# ""This Attack and Minions have %1%%% increased Damage with Poison"" reminderstring ReminderTextPoison
		#|-1 ""This Attack and Minions have %1%%% reduced Damage with Poison"" reminderstring ReminderTextPoison
	lang ""Portuguese""
	2
		1|# ""Este Ataque e Lacaios têm %1%%% de Dano com Veneno aumentado"" reminderstring ReminderTextPoison
		#|-1 ""Este Ataque e Lacaios têm %1%%% de Dano com Veneno reduzido"" reminderstring ReminderTextPoison";

            res = (Description)StatDescriptionParser.ParseDescription(str);
            Assert.Equal(2, res.Languages.Count);
            Assert.Equal(1, res.Stats.Count);
            Assert.Equal("", res.Name);
        }

        [Fact]
        public void NoDesription_Parses()
        {
            var str = "no_description current_endurance_charges";
            NoDescription res = (NoDescription)StatDescriptionParser.ParseNoDescription(str);
            Assert.Equal("current_endurance_charges", res.Stat);
        }

        [Fact]
        public void StatsDescription_File_Parses()
        {
            var str = CompleteStatsDescriptionTestString.Replace("\r\n", "\n");
            var res = StatDescriptionParser.ParseInstructions(str).ToList();
            Assert.Equal(9, res.Count);
        }

        static string CompleteStatsDescriptionTestString = 
            @"include ""Metadata/StatDescriptions/skill_stat_descriptions.txt""
no_description level
no_description running

no_description item_drop_slots
no_description main_hand_weapon_type

has_identifiers

something_%ok
description buff_duration
	1	buff_effect_duration
	1
		1|# ""Debuff Lasts %1% seconds"" milliseconds_to_seconds_2dp 1
	lang ""Portuguese""
	1
		1|# ""Penalidade Dura %1% segundos"" milliseconds_to_seconds_2dp 1
	lang ""Traditional Chinese""
	1
		1|# ""減益效果持續 %1% 秒"" milliseconds_to_seconds_2dp 1
	lang ""Simplified Chinese""
	1
		1|# ""减益效果持续 %1% 秒"" milliseconds_to_seconds_2dp 1
	lang ""Thai""
	1
		1|# ""Debuff จะอยู่นาน %1% วินาที"" milliseconds_to_seconds_2dp 1
	lang ""Russian""
	1
		1|# ""Длительность отрицательного эффекта: %1% сек."" milliseconds_to_seconds_2dp 1
	lang ""Spanish""
	1
		1|# ""La Penalidad Dura %1% segundos"" milliseconds_to_seconds_2dp 1
	lang ""French""
	1
		1|# ""L'Effet néfaste dure %1% secondes"" milliseconds_to_seconds_2dp 1
	lang ""German""
	1
		1|# ""Schwächung hält %1% Sekunden an"" milliseconds_to_seconds_2dp 1
description secondary_buff_duration
	1	secondary_buff_effect_duration
	1
		1|# ""Secondary Debuff Lasts %1% seconds"" milliseconds_to_seconds_2dp 1
	lang ""Portuguese""
	1
		1|# ""Penalidade Secundária Dura %1% segundos"" milliseconds_to_seconds_2dp 1
	lang ""Traditional Chinese""
	1
		1|# ""附屬益效果持續 %1% 秒"" milliseconds_to_seconds_2dp 1
	lang ""Simplified Chinese""
	1
		1|# ""附带的减益效果持续 %1% 秒"" milliseconds_to_seconds_2dp 1
	lang ""Thai""
	1
		1|# ""Debuff รอง จะอยู่นาน %1% วินาที"" milliseconds_to_seconds_2dp 1
	lang ""Russian""
	1
		1|# ""Длительность вторичного отрицательного эффекта: %1% сек."" milliseconds_to_seconds_2dp 1
	lang ""Spanish""
	1
		1|# ""La Penalidad Secundaria Dura %1% segundos"" milliseconds_to_seconds_2dp 1
	lang ""French""
	1
		1|# ""L'Effet néfaste secondaire dure %1% secondes"" milliseconds_to_seconds_2dp 1
	lang ""German""
	1
		1|# ""Sekundäre Schwächung hält %1% Sekunden an"" milliseconds_to_seconds_2dp 1";
    }
}
