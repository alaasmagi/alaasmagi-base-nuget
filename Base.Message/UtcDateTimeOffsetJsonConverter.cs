using System.Text.Json;
using System.Text.Json.Serialization;

namespace Base.Message;

/// <summary>Writes instants in UTC using the ISO 8601 Z suffix.</summary>
public sealed class UtcDateTimeOffsetJsonConverter : JsonConverter<DateTimeOffset>
{
    public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options) => reader.GetDateTimeOffset().ToUniversalTime();

    public override void Write(Utf8JsonWriter writer, DateTimeOffset value,
        JsonSerializerOptions options) => writer.WriteStringValue(value.UtcDateTime);
}
