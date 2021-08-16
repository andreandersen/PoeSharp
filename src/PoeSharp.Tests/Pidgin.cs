/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PoeSharp.Filetypes.StatDescriptions;

using Xunit;

namespace PoeSharp.Tests
{
    public class PidginTests
    {
        [Fact]
        public void ParseRange()
        {
            var input = "!0    0     0     #     #     #     #     #     \"";
            var result = PidginStatParser.ParseRange(input);
        }

        [Fact]
        public void ParseInclude()
        {
            var input = "include \"Metadata/StatDescriptions/stat_descriptions.txt\"";
            var result = PidginStatParser.ParseInclude(input);
        }

        //[Fact]
        public void ParseNoDescription()
        {
            var input = "no_description monster_dropped_item_quantity_+%";
            var result = PidginStatParser.ParseNoDescription(input);
        }
    }
}
*/