using System;

namespace PoeSharp.Filetypes
{
    public class ParseException : Exception
    {
        public ParseException()
        {
        }

        public ParseException(string message) : base(message)
        {
        }

        public static ParseException GgpkParseFailure =>
            new("Parsing GGPK failed");
    }
}
