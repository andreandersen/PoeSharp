namespace PoeSharp.Shared.EphemeralId
{
    public class StandardIntStringEphemeralGenerator : EphemeralIdGenerator<string, int>
    {
        public StandardIntStringEphemeralGenerator() : base((d, b) => d.Count + 1)
        {
        }
    }
}