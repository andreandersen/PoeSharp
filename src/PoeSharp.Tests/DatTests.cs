using System.ComponentModel;
using System.Text.Json;

using PoeSharp.Filetypes.BuildingBlocks;
using PoeSharp.Filetypes.Dat;
using PoeSharp.Filetypes.Dat.Specification;

using Xunit;
using Xunit.Abstractions;

namespace PoeSharp.Tests
{
    [Category("Dat Json Tests")]
    public class DatJsonTests : IClassFixture<DatFixture>
    {
        private readonly ITestOutputHelper _output;
        private readonly DatSpecIndex _specIndex;
        private readonly DatFileIndex _datFileIndex;

        public DatJsonTests(ITestOutputHelper output, DatFixture fixture)
        {
            _output = output;
            _specIndex = fixture.SpecIndex;
            _datFileIndex = fixture.DatFileIndex;
        }

        [Theory]
        [InlineData("ShrineBuffs.dat")]
        [InlineData("ShrineBuffs.dat64")]
        public void Can_Read_PrimitiveArray(string file)
        {
            var dat = _datFileIndex[file];
            var row = dat[33];
            int[] val = row["BuffStatValues"];
            var expected = new[] { 25, 2, 1 };
            Assert.Equal(expected, val);
        }

        [Fact]
        public void Can_Deserialize_Spec()
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };

            options.Converters.Add(new DatSpecJsonConverter());

            var json = File.ReadAllText(@"spec.json");
            var stuff = JsonSerializer.Deserialize<DatSpecIndex>(json, options);
        }

        [Theory]
        [InlineData("Shrines.dat", "Id", 0, "Acceleration")]
        [InlineData("Shrines.dat64", "Id", 0, "Acceleration")]
        [InlineData("Shrines.datl", "Id", 0, "Acceleration")]
        [InlineData("Shrines.datl64", "Id", 0, "Acceleration")]
        [InlineData("Shrines.dat", "Name", 1, "Lightning Shrine")]
        [InlineData("Shrines.dat64", "Name", 1, "Lightning Shrine")]
        [InlineData("Shrines.datl", "Name", 1, "Lightning Shrine")]
        [InlineData("Shrines.datl64", "Name", 1, "Lightning Shrine")]
        [InlineData("Environments.dat", "Corrupted_ENVFile", 0, "")]
        [InlineData("Environments.dat64", "Corrupted_ENVFile", 0, "")]
        [InlineData("Environments.datl", "Corrupted_ENVFile", 0, "")]
        [InlineData("Environments.datl64", "Corrupted_ENVFile", 0, "")]
        public void Dat_Read_Strings(string filename, string column, int row, string expected)
        {
            var datFile = _datFileIndex[filename];
            var actual = datFile[row][column];
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("Shrines.dat", "TimeoutInSeconds", 17, 45)]
        [InlineData("Shrines.dat64", "TimeoutInSeconds", 17, 45)]
        public void Dat_Read_Numbers(string filename, string column, int row, int expected)
        {
            var datFile = _datFileIndex[filename];
            var actual = datFile[row][column];
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("BuffVisuals.dat")]
        [InlineData("BuffVisuals.dat64")]
        [InlineData("BuffVisuals.datl")]
        [InlineData("BuffVisuals.datl64")]
        public void Dat_Read_StringArray(string filename)
        {
            var datFile = _datFileIndex[filename];
            string[] actual = datFile[55]["EPKFiles1"];

            foreach (var item in actual)
            {
                _output.WriteLine(item);
            }

            Assert.Equal(2, actual.Length);
            Assert.Equal("Metadata/Effects/StatusAilments/monsters/champions/gold/gold.epk", actual[0]);
            Assert.Equal("Metadata/Effects/StatusAilments/monsters/champions/gold/gold_large.epk", actual[1]);
        }

        [Theory]
        [InlineData("BuffVisuals.dat")]
        [InlineData("BuffVisuals.dat64")]
        [InlineData("BuffVisuals.datl")]
        [InlineData("BuffVisuals.datl64")]
        public void Dat_Read_Reference(string filename)
        {
            var refVal = _datFileIndex[filename][760]["MiscAnimated1"].GetReference();
            Assert.NotNull(refVal);
            Assert.Equal(1805, refVal.RowIndex);

        }

        [Theory]
        [InlineData("Shrines.dat")]
        [InlineData("Shrines.dat64")]
        public void Dat_Read_References(string filename)
        {
            var refVal = _datFileIndex[filename][4]["AchievementItemsKeys"].GetReferenceArray();
            Assert.Equal(2, refVal.Length);
            Assert.Equal(516, refVal[0].RowIndex);
            Assert.Equal(8031, refVal[1].RowIndex);

            DatRow[] vals = _datFileIndex[filename][4]["AchievementItemsKeys"];
        }
    }

    public class DatFixture
    {
        public DatSpecIndex SpecIndex = DatSpecIndex.Default;
        public DatFileIndex DatFileIndex;

        public DatFixture()
        {
            DatFileIndex = new DatFileIndex(
                new DiskDirectory(@"Dats"), SpecIndex);
        }
    }


}