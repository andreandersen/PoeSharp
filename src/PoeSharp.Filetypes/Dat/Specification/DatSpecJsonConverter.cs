using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace PoeSharp.Filetypes.Dat.Specification
{
    public class DatSpecJsonConverter : JsonConverter<DatSpecIndex>
    {
        public override DatSpecIndex Read(
            ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var node = JsonSerializer.Deserialize<JsonObject>(ref reader);
            if (node == null)
                throw new ArgumentException("The JSON provided seems to be null");
            if (node["tables"] is null or not JsonArray)
                throw new ArgumentException("The JSON provided does not have any 'tables' array");

            var tables = (node["tables"]!).AsArray().Select(tn =>
            {
                var off32 = 0;
                var off64 = 0;
                var unkIdx = 0;

                var tableName = (string)tn!["name"]!;

                var columns = (tn["columns"] ?? new JsonArray()).AsArray()
                    .Where(cn => cn is not null)
                    .Select(cn =>
                    {
                        var n = (string?)cn!["name"] ?? $"Unknown{unkIdx++}";
                        var d = (string?)cn["description"] ?? "";
                        var a = (bool?)cn["array"] ?? false;
                        var t = ColumnTypeFromString((string?)cn["type"] ?? "");
                        var u = (bool?)cn["unique"] ?? false;
                        var l = (bool?)cn["localized"] ?? false;
                        var rt = (string?)cn["references"]?["table"];
                        var rc = (string?)cn["references"]?["column"];
                        var r = rt != null ? new ReferenceDefinition(rt, rc) : null;
                        var un = (string?)cn["until"];

                        var sz32 = a ? 8 : t.GetSize32();
                        var sz64 = a ? 16 : t.GetSize64();

                        var cd = new ColumnDefinition(
                            n, d, a, t, u, l, r, un,
                            off32, sz32, off64, sz64);

                        off32 += sz32;
                        off64 += sz64;

                        return cd;

                    }).ToList();
                return new TableDefinition(tableName, columns);
            }).ToList();

            return new DatSpecIndex(
                (int?)node["version"] ?? 0,
                DateTimeOffset.FromUnixTimeSeconds((long?)node["createdAt"] ?? 0),
                tables);
        }

        public static ColumnType ColumnTypeFromString(string val) => val switch
        {
            "array" => ColumnType.Array,
            "bool" => ColumnType.Bool,
            "f32" => ColumnType.F32,
            "foreignrow" => ColumnType.ForeignRow,
            "i16" => ColumnType.I16,
            "i32" => ColumnType.I32,
            "i64" => ColumnType.I64,
            "i8" => ColumnType.I8,
            "row" => ColumnType.Row,
            "string" => ColumnType.String,
            "u32" => ColumnType.U32,
            "u64" => ColumnType.U64,
            _ => ColumnType.Unknown
        };

        public override void Write(
            Utf8JsonWriter writer, DatSpecIndex value, JsonSerializerOptions options) =>
            throw new System.NotImplementedException();
    }
}
