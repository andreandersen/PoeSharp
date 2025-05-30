﻿using System.Collections.Generic;

namespace PoeSharp.Filetypes.StatDescriptions.SyntaxTree
{
    public class Language
    {
        public Language(string name, IReadOnlyList<DescriptionVariation> variations)
        {
            Name = name;
            Variations = variations;
        }

        public string Name { get; }
        public IReadOnlyList<DescriptionVariation> Variations { get; }
    }
}
