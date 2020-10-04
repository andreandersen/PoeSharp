namespace PoeSharp.Filetypes.BuildingBlocks
{
    internal class JsonSnakeCaseNamingPolicy : JsonSeparatedCaseNamingPolicy
    {
        public override string ConvertName(string name) => ToSeparatedCase(name, '_');
    }
}