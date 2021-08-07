using System.Collections.Generic;

namespace PoeSharp.Filetypes.StatDescriptions.SyntaxTree
{
    public class DescriptionVariation
    {
        private static readonly Dictionary<string, string> AnnoyingIssuesReplacements =
            new()
            {
                {
                    "divide_by_one_hundred 2reminderstring ReminderTextLifeLeech",
                    "divide_by_one_hundred 2"
                }
            };

        public DescriptionVariation(string range, string text, string extra)
        {
            RangeText = range;
            TranslationTemplate = text;
            ExtraInstructions = extra.Trim();

            if (AnnoyingIssuesReplacements.TryGetValue(ExtraInstructions,
                out var replacement))
                ExtraInstructions = replacement;
        }

        public string RangeText { get; }
        public string TranslationTemplate { get; }
        public string ExtraInstructions { get; }
    }
}
