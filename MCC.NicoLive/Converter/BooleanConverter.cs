using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MCC.NicoLive.Converter
{
    public class BooleanConverter : JsonConverter<bool>
    {
        public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => reader.GetInt32() == 0 ? false : true;

        public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
            => writer.WriteNumberValue(value ? 1 : 0);
    }
}
