/*
using System.Linq;

using Pidgin;

using static Pidgin.Parser;
using static Pidgin.Parser<char>;

namespace PoeSharp.Filetypes.StatDescriptions
{
    public class PidginStatParser
    {
        static readonly Parser<char, char> Quote = Char('"');

        static readonly Parser<char, string> IncludeInstruction =
            String("include")
            .Then(Whitespaces).Optional()
            .Then(Quote)
            .Then(AnyCharExcept('"').ManyString())
            .Until(Quote)
            .Select(c => string.Concat(c));

        static readonly Parser<char, string> NoDescription =
            String("no_description")
            .Then(Whitespaces).Optional()
            .Then(Any.AtLeastOnceUntil(EndOfLine))
            .Select(c => string.Concat(c));

        public static string ParseNoDescription(string input)
        {
            var p = NoDescription.Parse(input);
            return p.Value;
        }

        public static string ParseInclude(string input)
        {
            var p = IncludeInstruction.Parse(input);
            return p.Value;
        }

        static readonly Parser<char, char> RangeCharacter =
            Token(c => "01234567890#|!-".Contains(c));

        static readonly Parser<char, string[]> Ranges =
            RangeCharacter
            .Until(Whitespace)
            .Select(c => string.Concat(c))
            .Until(Quote)
            .Select(e => e.Where(e => !string.IsNullOrEmpty(e)).ToArray());

        public static string[] ParseRange(string input)
        {
            var p = Ranges.Parse(input);
            return p.Value;
        }
    }
}
*/