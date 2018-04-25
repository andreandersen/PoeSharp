using PoeSharp.Files.Ot;
using System;
using System.IO;
using Xunit;

namespace PoeSharp.Tests
{
    [Collection("Ot Parser")]
    public class OtFileParseTests
    {
        [Fact]
        public void String_Value_Parses()
        {
            var d = OtParser.ParsePair(@"    something_cool% = ""yes""");
            Assert.Equal("something_cool%", d.Identifier);
            Assert.Equal("yes", d.Value.Value);
        }

        [Fact]
        public void Float_Value_Parses()
        {
            var d = OtParser.ParsePair(@"    something_cool = 83.34f");
            Assert.Equal("something_cool", d.Identifier);
            Assert.Equal(83.34f, d.Value.Value);
        }

        [Fact]
        public void Boolean_Value_Parses()
        {
            var d = OtParser.ParsePair(@"    something_cool = false");
            Assert.Equal("something_cool", d.Identifier);
            Assert.Equal(false, d.Value.Value);

            d = OtParser.ParsePair(@"    something_cool = true");
            Assert.Equal("something_cool", d.Identifier);
            Assert.Equal(true, d.Value.Value);
        }

        [Fact]
        public void Object_Parses()
        {
            string otString = @"Stats
{
	level = 1
	is_player = 1
	energy_shield_recharge_rate_per_minute_% = 1200
    bkdlöasdöl = ""hello""
}";
            var d = OtParser.ParseObject(otString);
            Assert.Equal("Stats", d.Name);
            Assert.Equal(4, d.Values.Count);
            // TOOD: Probably improve validation of test success
        }

        [Fact]
        public void File_Prop_Parses()
        {
            var d = OtParser.ParseFileProp(@"extends ""nothing""");
            Assert.Equal("extends", d.Identifier);
            Assert.Equal("nothing", d.Value.Value);
        }

        [Fact]
        public void File_Parses()
        {
            var d = OtParser.ParseOtFile(OtTestFile);
            Assert.Equal("nothing", d.Extends);
            Assert.Equal("2", d.Version);
        }

        private const string OtTestFile =
            @"version 2
extends ""nothing""

Stats
{
	item_drop_slots = 1
	energy_shield_recharge_rate_per_minute_% = 1200
	mana_regeneration_rate_per_minute_% = 100
	base_maximum_mana = 200
	maximum_physical_damage_reduction_% = 75
	max_viper_strike_orbs = 4
	base_maximum_fire_damage_resistance_% = 75
	base_maximum_cold_damage_resistance_% = 75
	base_maximum_lightning_damage_resistance_% = 75
	base_maximum_chaos_damage_resistance_% = 75
	max_fuse_arrow_orbs = 5
	max_fire_beam_stacks = 8
	max_charged_attack_stacks = 10
	base_critical_strike_multiplier = 130
	base_critical_degen_multiplier = 130
	max_endurance_charges = 3
	max_frenzy_charges = 3
	max_power_charges = 3
	base_attack_speed_+%_per_frenzy_charge = 15
	base_cast_speed_+%_per_frenzy_charge = 15
	movement_velocity_+%_per_frenzy_charge = 5
	object_inherent_damage_+%_final_per_frenzy_charge = 4
	physical_damage_reduction_%_per_endurance_charge = 15
	resist_all_elements_%_per_endurance_charge = 15
	critical_strike_chance_+%_per_power_charge = 200
	maximum_block_% = 75
	base_number_of_totems_allowed = 1
	base_number_of_traps_allowed = 3
	base_number_of_remote_mines_allowed = 5
	max_corrupted_blood_stacks = 20
	max_corrupted_blood_rain_stacks = 20
	maximum_dodge_chance_% = 75
	maximum_spell_dodge_chance_% = 75
	movement_velocity_cap = 128
	maximum_life_leech_rate_%_per_minute = 1200
	maximum_mana_leech_rate_%_per_minute = 1200
	damage_+%_final_vs_non_taunt_target_from_ot = -10
	monster_ignite_damage_+%_final = -50
	monster_bleeding_damage_+%_final = -86
	monster_poison_damage_+%_final = -50
	bleeding_moving_damage_%_of_base_override = 500
}

    ObjectMagicProperties
{
	stat_description_list = ""Metadata/StatDescriptions/monster_stat_descriptions.txt""
}

Positioned
{
	blocking = true
	team = 0
}

Pathfinding { }

Life { }

Animated { }

Inventories { }

Actor 
{
	basic_action = ""Move""
	basic_action = ""Flee""
	basic_action = ""StrafeLeft""
	basic_action = ""StrafeRight""
	basic_action = ""Advance""
	basic_action = ""DoNothing""
	basic_action = ""MonsterPickup""
	slow_animations_go_to_idle = true
}

Monster
{

}";
    }
}
