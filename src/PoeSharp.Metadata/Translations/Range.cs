namespace PoeSharp.Metadata.Translations
{
    public class Range
    {
        public Range(int? min, int? max)
        {
            Min = min;
            Max = max;
        }

        public int? Max { get; }
        public int? Min { get; }

        public bool SatisfiesRange(int min, int max) =>
            (!Max.HasValue || Max.Value >= max) &&
            (!Min.HasValue || Min.Value <= min);

        public bool SatisfiesValue(int value) =>
            (!Max.HasValue || Max.Value <= value) &&
            (!Min.HasValue || Min.Value >= value);
    }
}
