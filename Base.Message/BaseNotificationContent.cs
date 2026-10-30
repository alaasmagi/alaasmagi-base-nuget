using System.Globalization;
using System.Text.Json.Serialization;

namespace Base.Message;

/// <summary>Common recipient fields for a command targeting exactly one person.</summary>
public class BaseNotificationContent
{
    [JsonPropertyName("userId")]
    public required string UserId { get; init; }

    [JsonPropertyName("email")]
    public required string Email { get; init; }

    [JsonPropertyName("fullName")]
    public required string FullName { get; init; }

    /// <summary>A BCP 47 locale including a region, for example et-EE.</summary>
    [JsonPropertyName("locale")]
    public string? Locale { get; init; }

    /// <summary>An IANA time-zone identifier, for example Europe/Tallinn.</summary>
    [JsonPropertyName("timeZone")]
    public string? TimeZone { get; init; }

    /// <summary>Validates required recipient values before publishing. No recipient data is logged.</summary>
    public void Validate()
    {
        if (!Guid.TryParse(UserId, out _))
            throw new ArgumentException("Command userId must be a UUID.", nameof(UserId));
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(FullName))
            throw new ArgumentException("Command email and fullName must be non-empty.");
        if (string.IsNullOrWhiteSpace(Locale) || !Locale.Split('-').Skip(1).Any(
                part => (part.Length == 2 && part.All(char.IsAsciiLetter)) ||
                        (part.Length == 3 && part.All(char.IsAsciiDigit))))
            throw new ArgumentException("Command locale must include a BCP 47 region.", nameof(Locale));
        try
        {
            var culture = CultureInfo.GetCultureInfo(Locale);
            if (culture.IsNeutralCulture)
                throw new ArgumentException("Command locale must include a BCP 47 region.", nameof(Locale));
        }
        catch (CultureNotFoundException)
        {
            throw new ArgumentException("Command locale must be a BCP 47 locale.", nameof(Locale));
        }
        if (string.IsNullOrWhiteSpace(TimeZone) || !TimeZoneInfo.TryConvertIanaIdToWindowsId(TimeZone, out _))
            throw new ArgumentException("Command timeZone must be an IANA identifier.", nameof(TimeZone));
    }
}
