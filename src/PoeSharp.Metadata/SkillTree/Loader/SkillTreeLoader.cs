using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoeSharp.Files.Dat;
using PoeSharp.Files.Psg;
using PoeSharp.Metadata.Stats;
using PoeSharp.Shared;

namespace PoeSharp.Metadata.SkillTree.Loader
{
    public class SkillTreeLoader : IDataLoader<PsgFile, SkillTree>
    {
        private readonly IReadOnlyDictionary<string, Stat> _stats;
        private readonly DatFileIndex _datIndex;

        public SkillTreeLoader(IReadOnlyDictionary<string, Stat> stats,
            DatFileIndex datIndex)
        {
            _stats = stats;
            _datIndex = datIndex;
        }


        public SkillTree Load(PsgFile source)
        {
            var passiveSkillStatCategories =
                _datIndex["PassiveSkillStatCategories.dat"].Select(c => new
                {
                    Id = (string)c["Id"].Value,
                    Name = (string)c["Name"].Value
                });

            var passiveSkills =
                _datIndex["PassiveSkills.dat"].Select(c => new
                {
                    Id = c.Value<string>("Id"),
                    Icon = c.Value<string>("Icon_DDSFile"),
                    StatKeys = c.Value("StatsKeys"),
                    Stat1Value = c.Value<int>("Stat1Value"),
                    Stat2Value = c.Value<int>("Stat2Value"),
                    Stat3Value = c.Value<int>("Stat3Value"),
                    Stat4Value = c.Value<int>("Stat4Value"),
                    Stat5Value = c.Value<int>("Stat5Value"),
                    PassiveSkillGraphId = c.Value<int>("PassiveSkillGraphId"),
                    Name = c.Value<string>("Name"),
                    CharacterKeys = c.Value("CharactersKeys"),
                    IsKeystone = c.Value<bool>("IsKeystone"),
                    IsNotable = c.Value<bool>("IsNotable"),
                    FlavourText = c.Value<string>("FlavourText"),
                    IsJustIcon = c.Value<bool>("IsJustIcon"),
                    AchievementItems = c.Value("AchievementItemsKey"),
                    IsJewelSocket = c.Value<bool>("IsJewelSocket"),
                    Ascendancy = c.Value("AscendancyKey"),
                    IsAscendancyStartingNode = c.Value<bool>("IsAscendancyStartingNode"),
                    ReminderClientStrings = c.Value("Reminder_ClientStringsKeys"),
                    SkillPointsGranted = c.Value<int>("SkillPointsGranted"),
                    IsMultipleChoice = c.Value<bool>("IsMultipleChoice"),
                    IsMultipleChoiceOption = c.Value<bool>("IsMultipleChoiceOption")
                }).ToList();

            var multipleChoice = passiveSkills
                .Where(c => c.IsMultipleChoice || c.IsMultipleChoiceOption)
                .ToList();

            var memoryFragments =
                _datIndex["Commands.dat"].Select(c => new
                {
                    Id = c.Value("Id"),
                    Command = c.Value("Command"),
                    Command2 = c.Value("Command2"),
                    Flag0 = c.Value("Flag0"),
                    Description = c.Value("Description")
                });


            return null;
        }

        
    }
}
