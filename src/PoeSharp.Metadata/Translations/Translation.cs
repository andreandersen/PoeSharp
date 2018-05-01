using System.Collections.Generic;

namespace PoeSharp.Metadata.Translations
{
    public class Translation
    {
        private readonly List<Variation> _variations;
        public IReadOnlyList<Variation> Variations => _variations;

        public Translation(List<Variation> variations)
        {
            _variations = variations;
        }
    }
}
