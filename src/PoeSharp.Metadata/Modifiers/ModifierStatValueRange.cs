using PoeSharp.Metadata.Stats;

namespace PoeSharp.Metadata.Modifiers
{
    public class ModifierStatValueRange
    {
        public ModifierStatValueRange(Stat stat, int min, int max)
        {
            Stat = stat;
            Min = min;
            Max = max;
        }

        public Stat Stat { get; }
        public int Min { get; }
        public int Max { get; }
    }
}