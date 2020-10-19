using System.ComponentModel;

using FluentAssertions;

using PoeSharp.Filetypes.BuildingBlocks;
using PoeSharp.Filetypes.Dat;
using PoeSharp.Filetypes.Dat.Specification;

using Xunit;

/**
 * These test are not exhaustive in any manner (yet)
 * I use them mainly right now for tight loop development.
 * 
 * Pull Requests welcome for more exhaustive tests :)
 * 
 **/

namespace PoeSharp.Tests
{
    [Category("Dat Tests")]
    [Trait("Category", "Dat Tests")]
    public class DatTests : IClassFixture<DatTestsFixture>
    {
        public DatTestsFixture Fixture;

        public DatTests(DatTestsFixture fixture)
        {
            Fixture = fixture;
        }

        [Fact]
        public void RealmsDat_Reads_String_Correctly()
        {
            var realmsDat = Fixture.DatIndex["Realms.dat"];
            var firstRow = realmsDat[0];
            var secondRow = realmsDat[1];

            var id = firstRow.GetString("Id");
            Assert.Equal("America", id);

            var name = firstRow.GetString("Name");
            Assert.Equal("Texas (US)", name);

            var id2 = secondRow.GetString("Id");
            Assert.Equal("Europe", id2);

            var name2 = secondRow.GetString("Name");
            Assert.Equal("Amsterdam (EU)", name2);
        }

        [Fact]
        public void RealmsDat_Reads_Strings_Correctly()
        {
            var realmsDat = Fixture.DatIndex["Realms.dat"];
            var firstRow = realmsDat[0];

            var serverRow1 = firstRow.GetStringList("Server");

            serverRow1.Should().HaveCount(1);
            Assert.Equal("dal01.login.pathofexile.com", serverRow1[0]);
        }

        [Fact]
        public void RealmsDat_Reads_Data0_Ints_Correctly()
        {
            var realmsDat = Fixture.DatIndex["Realms.dat"];
            var firstRow = realmsDat[0];

            var ints = firstRow.GetPrimitiveList<int>("Data0");
            ints.Should().HaveCount(2);
            ints[0].Should().Be(12);
            ints[1].Should().Be(11);
        }


        [Fact]
        public void GrantedEffects_Reads_GenericReference()
        {
            var dat = Fixture.DatIndex["GrantedEffects.dat"];
            var row = dat[7672];
            var gefSuccess = row.TryGetReferencedDatValue("GrantedEffectsKey", out var outRow);

            gefSuccess.Should().BeTrue();
            outRow.GetString("Id").Should().Be("SupportAddedFireDamage");
        }

        [Fact]
        public void GrantedEffects_Reads_ActiveSkillReference()
        {
            var dat = Fixture.DatIndex["GrantedEffects.dat"];
            var row = dat[424];

            var askSucces = row.TryGetReferencedDatValue("ActiveSkillsKey", out var outRow);
            askSucces.Should().BeTrue();

            outRow.GetString("Id").Should().Be("melee");
        }

        [Fact]
        public void DatRowListTest()
        {
            var dat = Fixture.DatIndex["ActiveSkills.dat"];
            var row = dat[0];
            var y = row.TryGetReferencedDatValueList("Input_StatKeys", out var rows);
            y.Should().BeTrue();
            rows.Length.Should().Be(4);
            rows[0].GetString("Id").Should().Be("sweep_damage_+%");
            rows[1].GetString("Id").Should().Be("sweep_knockback_chance_%");
            rows[2].GetString("Id").Should().Be("sweep_radius_+%");
            rows[3].GetString("Id").Should().Be("sweep_add_endurance_charge_on_hit_%");
        }
    }

    public class DatTestsFixture
    {
        public DatSpecIndex SpecIndex { get; }
        public DatFileIndex DatIndex { get; }

        public DatTestsFixture()
        {
            SpecIndex = DatSpecIndex.Default;
            DatIndex = new DatFileIndex((DiskDirectory)"Dats", SpecIndex);
        }
    }
}
