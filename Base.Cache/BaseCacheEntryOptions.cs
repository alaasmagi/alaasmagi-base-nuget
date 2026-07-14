using Base.Contracts.Cache;

namespace Base.Cache;

/// <summary>
/// Provides cache entry expiration options for base cache implementations.
/// </summary>
public class BaseCacheEntryOptions : IBaseCacheEntryOptions
{
    /// <summary>
    /// Gets a default options instance with no expiration configured.
    /// </summary>
    public static BaseCacheEntryOptions Default => new();

    /// <summary>
    /// Gets or sets the absolute point in time when the cache entry should expire.
    /// </summary>
    public DateTimeOffset? AbsoluteExpiration { get; set; }

    /// <summary>
    /// Gets or sets the relative duration after which the cache entry should expire.
    /// </summary>
    public TimeSpan? AbsoluteExpirationRelativeToNow { get; set; }

    /// <summary>
    /// Gets or sets the duration after the last access when the cache entry should expire.
    /// </summary>
    public TimeSpan? SlidingExpiration { get; set; }
}
