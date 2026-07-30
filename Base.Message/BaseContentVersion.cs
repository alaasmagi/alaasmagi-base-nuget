using System.Globalization;

namespace Base.Message;

/// <summary>
/// Helpers for the envelope <c>contentVersion</c> field, formatted as <c>"major.minor"</c>. Compatibility
/// is decided on the major component only: a consumer accepts any matching-major version and ignores fields
/// it does not know. Comparing the full string dead-letters every additive (minor) change.
/// </summary>
public static class BaseContentVersion
{
    /// <summary>
    /// Extracts the major component of a <c>"major.minor"</c> version string.
    /// </summary>
    /// <param name="contentVersion">The content version string, for example <c>"1.0"</c>.</param>
    /// <returns>The major version number.</returns>
    /// <exception cref="ArgumentException">The version string is null, empty, or not a valid number.</exception>
    public static int Major(string contentVersion)
    {
        if (string.IsNullOrWhiteSpace(contentVersion))
        {
            throw new ArgumentException("Content version must not be null or empty.", nameof(contentVersion));
        }

        var dotIndex = contentVersion.IndexOf('.');
        var majorPart = dotIndex < 0 ? contentVersion : contentVersion[..dotIndex];

        if (!int.TryParse(majorPart, NumberStyles.Integer, CultureInfo.InvariantCulture, out var major))
        {
            throw new ArgumentException(
                $"Content version '{contentVersion}' does not start with a valid major version.",
                nameof(contentVersion));
        }

        return major;
    }

    /// <summary>
    /// Determines whether an <paramref name="actual"/> content version is compatible with a
    /// <paramref name="supported"/> one, comparing the major component only.
    /// </summary>
    /// <param name="supported">The content version the consumer is written against, for example <c>"1.0"</c>.</param>
    /// <param name="actual">The content version carried by the incoming message.</param>
    /// <returns><c>true</c> when both share the same major version; otherwise <c>false</c>.</returns>
    public static bool IsCompatible(string supported, string actual) =>
        Major(supported) == Major(actual);
}
