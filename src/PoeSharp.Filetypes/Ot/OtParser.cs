using System.Collections.Generic;
using System.Linq;

using PoeSharp.Filetypes.Ot.SyntaxTree;

using Sprache;

namespace PoeSharp.Filetypes.Ot
{
    public class OtParser
    {
        private static readonly Parser<string> Identifier =
            Parse.AnyChar.Except(Parse.WhiteSpace).AtLeastOnce().Text().Token();

        private static readonly Parser<OtLiteral> StringValue =
            from first in Parse.Char('"')
            from chars in Parse.AnyChar.Except(Parse.Char('"')).Many().Text()
            from last in Parse.Char('"')
            select new OtLiteral(chars, LiteralType.String);

        private static readonly Parser<OtLiteral> NumberValue =
            from x in Parse.Chars("-01234567890.f").AtLeastOnce().Text()
            select new OtLiteral(float.Parse(x.Replace("f", "")), LiteralType.Number);

        private static readonly Parser<OtLiteral> BooleanValue =
            from x in Parse.String("true").Or(Parse.String("false")).Text()
            select new OtLiteral(bool.Parse(x), LiteralType.Boolean);

        private static readonly Parser<OtLiteral> Value =
            StringValue.Or(BooleanValue).XOr(NumberValue);

        private static readonly Parser<OtObject> ObjectParser =
            from identifier in Identifier
            from opening in Parse.Char('{').Token()
            from pairs in PairParser.Many()
            from closing in Parse.Char('}').Token()
            select new OtObject(identifier, pairs.Select(c =>
                new KeyValuePair<string, string>(c.Identifier,
                    c.Value.Value.ToString())).ToList());

        private static readonly Parser<Pair> PairParser =
            from identifier in Identifier
            from equal in Parse.Char('=').Token()
            from value in Value
            select new Pair(identifier, value);

        private static readonly Parser<Pair> OtFilePropertyParser =
            from identifier in Identifier
            from value in Value.Token()
            select new Pair(identifier, value);

        private static readonly Parser<OtFile> OtFileParser =
            from props in OtFilePropertyParser.Many()
            from objects in ObjectParser.Many()
            let version = props.FirstOrDefault(c => c.Identifier == "version")
            let extends = props.FirstOrDefault(c => c.Identifier == "extends")
            select new OtFile(version?.Value.Value.ToString(),
                (string)(extends?.Value.Value), objects.ToList());

        public static Pair ParsePair(string value) =>
            PairParser.Parse(value);

        public static OtObject ParseObject(string value) =>
            ObjectParser.Parse(value);

        public static Pair ParseFileProp(string value) =>
            OtFilePropertyParser.Parse(value);

        public static OtFile ParseOtFile(string value) =>
            OtFileParser.Parse(value);

        public class Pair
        {
            public Pair(string identifier, OtLiteral value)
            {
                Identifier = identifier;
                Value = value;
            }

            public string Identifier { get; }
            public OtLiteral Value { get; }
        }

    }
}
