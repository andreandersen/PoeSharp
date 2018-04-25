namespace PoeSharp.Metadata.Modifiers
{
    public class TagWeight
    {
        public TagWeight(string tagId, int weight)
        {
            TagId = tagId;
            Weight = weight;
        }

        public string TagId { get; }
        public int Weight { get; }

        public override string ToString() => $"{TagId}, Weight: {Weight}";
    }
}