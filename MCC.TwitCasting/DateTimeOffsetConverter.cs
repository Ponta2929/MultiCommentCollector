using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MCC.TwitCasting
{
    public class DateTimeOffsetConverter : JsonConverter<DateTimeOffset>
    {
        public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => DateTimeOffset.FromUnixTimeSeconds(reader.GetInt64() / 1000);

        public override void Write(Utf8JsonWriter writer, DateTimeOffset dateTimeValue, JsonSerializerOptions options)
            => writer.WriteNumberValue(dateTimeValue.ToUnixTimeSeconds() * 1000);
    }
}
