using System.Collections.Generic;

namespace PoeSharp.Files.StatDescriptions.SyntaxTree
{
    public class Description : IInstruction
    {
        public Description(string name, IReadOnlyList<string> stats,
            IReadOnlyList<Language> languages)
        {
            Name = name;
            Stats = stats;
            Languages = languages;
        }

        public string Name { get; }
        public IReadOnlyList<string> Stats { get; }
        public IReadOnlyList<Language> Languages { get; }
    }
}
