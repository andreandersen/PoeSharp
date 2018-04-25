using System;
using System.Collections.Generic;
using System.Linq;
using PoeSharp.Files.StatDescriptions.SyntaxTree;
using Sprache;


namespace PoeSharp.Files.StatDescriptions
{
    public static class StatDescriptionParser
    {
        private static readonly Parser<char> Quote = Parse.Char('"');

        private static readonly Parser<IInstruction> Include =
            from instruction in Parse.String("include").Token()
            from openingQuote in Quote.Token()
            from identifier in Parse.AnyChar.Except(Parse.Char('"')).AtLeastOnce().Text()
            from closingQuote in Quote.Token()
            select new Include(identifier);

        private static readonly Parser<IInstruction> NoDescription =
            from instruction in Parse.String("no_description").Token()
            from identifier in Parse.AnyChar.Except(Parse.LineEnd).AtLeastOnce().Text().Token()
            select new NoDescription(identifier);

        private static readonly Parser<IInstruction> SimpleInstructions =
            from instruction in Parse.LetterOrDigit.Or(Parse.Chars("-_%")).Many().Text().Token()
            select new SimpleInstruction(instruction);

        private static readonly Parser<string> Range =
            from range in Parse.Chars("-0123456789#| ").AtLeastOnce().Text().Token()
            select range.Trim();

        private static readonly Parser<string> TranslatedTemplate =
            from openQuote in Quote
            from text in Parse.AnyChar.Except(Parse.Char('"')).AtLeastOnce().Text()
            from closingQuote in Quote
            select text;

        private static readonly Parser<DescriptionVariation> Variation =
            from range in Range
            from template in TranslatedTemplate
            from extra in Parse.AnyChar.Except(Parse.LineEnd).Many().Text()
            select new DescriptionVariation(range, template, extra.Trim());

        private static readonly Parser<string> Lang =
            from line in Parse.String("lang").Token()
            from openingQuote in Quote
            from languate in Parse.AnyChar.Except(Quote).AtLeastOnce().Text()
            from closingQuote in Quote
            select languate;

        private static readonly Parser<IEnumerable<string>> Stats =
            from number in Parse.Number.Token()
            from stat in Parse.AnyChar.Except(Parse.LineEnd).AtLeastOnce().Text()
            select stat.Split(' ');

        private static readonly Parser<Language> Language =
            from name in Lang.Optional()
            from num in Parse.Number.Text().Token()
            from vars in Variation.DelimitedBy(Parse.LineEnd)
            select new Language(name.IsDefined ? name.Get() : "English",
                vars.ToList());

        private static Parser<IInstruction> Description =
            from instruction in Parse.String("description")
            from name in Parse.AnyChar.Except(Parse.LineEnd).AtLeastOnce().Text().Or(Parse.LineEnd)
            from stats in Stats
            from langs in Language.AtLeastOnce().Token()
            select new Description(name.Trim(), stats.Select(c => c.Trim()).ToList(), langs.ToList());

        private static Parser<IEnumerable<IInstruction>> Instructions =
            from x in
                Description
                .Or(NoDescription)
                .Or(Include)
                .Or(SimpleInstructions)
                .Many()
            select x;

        public static IInstruction ParseInclude(string str) => Include.Parse(str);
        public static IInstruction ParseNoDescription(string str) => NoDescription.Parse(str);
        public static DescriptionVariation ParseVariation(string str) => Variation.Parse(str);
        public static IEnumerable<string> ParseStats(string str) => Stats.Parse(str);
        public static Language ParseLanguage(string str) => Language.Parse(str);
        public static IInstruction ParseDescription(string str) => Description.Parse(str);
        public static IEnumerable<IInstruction> ParseInstructions(string str) => Instructions.Parse(str);
    }
}
