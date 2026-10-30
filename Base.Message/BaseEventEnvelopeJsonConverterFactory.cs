using System.Text.Json;
using System.Text.Json.Serialization;

namespace Base.Message;

/// <summary>Preserves the wire distinction between events and commands with a null expiry.</summary>
public sealed class BaseEventEnvelopeJsonConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert) =>
        typeToConvert.IsGenericType && typeToConvert.GetGenericTypeDefinition() == typeof(BaseEventEnvelope<>);

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options) =>
        (JsonConverter)Activator.CreateInstance(typeof(EnvelopeConverter<>).MakeGenericType(
            typeToConvert.GetGenericArguments()))!;

    private sealed class EnvelopeConverter<TContent> : JsonConverter<BaseEventEnvelope<TContent>>
    {
        public override BaseEventEnvelope<TContent> Read(ref Utf8JsonReader reader, Type typeToConvert,
            JsonSerializerOptions options)
        {
            using var document = JsonDocument.ParseValue(ref reader);
            var root = document.RootElement;
            string Required(string name) => root.TryGetProperty(name, out var value) &&
                value.ValueKind == JsonValueKind.String ? value.GetString()! :
                throw new JsonException($"Missing or invalid envelope field '{name}'.");
            if (!root.TryGetProperty("content", out var content))
                throw new JsonException("Missing envelope content.");
            var hasExpiry = root.TryGetProperty("expiresAt", out var expiry);
            return new BaseEventEnvelope<TContent>
            {
                Id = Required("id"), Source = Required("source"), Tenant = Required("tenant"),
                Action = Required("action"), Timestamp = Required("timestamp"),
                ContentVersion = Required("contentVersion"),
                Content = content.Deserialize<TContent>(options)!,
                ExpiresAt = hasExpiry && expiry.ValueKind != JsonValueKind.Null
                    ? expiry.GetDateTimeOffset().ToUniversalTime() : null,
                IsCommand = hasExpiry
            };
        }

        public override void Write(Utf8JsonWriter writer, BaseEventEnvelope<TContent> value,
            JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString("id", value.Id);
            writer.WriteString("source", value.Source);
            writer.WriteString("tenant", value.Tenant);
            writer.WriteString("action", value.Action);
            writer.WriteString("timestamp", value.Timestamp);
            if (value.IsCommand || value.Content is BaseNotificationContent || value.ExpiresAt.HasValue ||
                value.Action is "user-verify" or "user-2fa-otp" or "user-password-reset" or "invoice" or "lunch-recommendation")
            {
                if (value.ExpiresAt is { } expiry)
                    writer.WriteString("expiresAt", expiry.UtcDateTime);
                else
                    writer.WriteNull("expiresAt");
            }
            writer.WriteString("contentVersion", value.ContentVersion);
            writer.WritePropertyName("content");
            JsonSerializer.Serialize(writer, value.Content, options);
            writer.WriteEndObject();
        }
    }
}
