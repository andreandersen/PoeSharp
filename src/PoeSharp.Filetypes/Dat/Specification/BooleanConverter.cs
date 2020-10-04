using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PoeSharp.Filetypes.Dat.Specification
{
    public class BooleanConverter : JsonConverter<bool>
    {
        public override bool Read(
            ref Utf8JsonReader reader, Type typeToConvert,
            JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                var str = reader.GetString();
                if (bool.TryParse(str, out var b))
                    return b;
            }
            else if (reader.TokenType == JsonTokenType.False)
            {
                return false;
            }
            else if (reader.TokenType == JsonTokenType.True)
            {
                return true;
            }

            throw new JsonException();
        }

        public override void Write(
            Utf8JsonWriter writer, bool value, JsonSerializerOptions options) =>
            writer.WriteBooleanValue(value);
    }
}
