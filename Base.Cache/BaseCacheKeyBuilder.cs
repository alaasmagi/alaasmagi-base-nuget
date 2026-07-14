using Base.Contracts.Cache;

namespace Base.Cache;

/// <summary>
/// Provides the default cache key builder using colon-delimited prefixes.
/// </summary>
public class BaseCacheKeyBuilder : IBaseCacheKeyBuilder
{
    /// <summary>
    /// Builds a cache key by validating the logical key and prepending an optional prefix.
    /// </summary>
    public virtual string BuildKey(string key, string? keyPrefix = default)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException("Cache key cannot be empty.", nameof(key));
        }

        if (string.IsNullOrWhiteSpace(keyPrefix))
        {
            return key;
        }

        var prefix = keyPrefix.Trim().TrimEnd(':');
        if (string.IsNullOrWhiteSpace(prefix))
        {
            return key;
        }

        return $"{prefix}:{key.TrimStart(':')}";
    }
}
