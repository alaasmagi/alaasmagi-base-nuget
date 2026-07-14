namespace Base.Contracts.Cache;

/// <summary>
/// Defines optional expiration metadata for a cache entry.
/// </summary>
public interface IBaseCacheEntryOptions
{
    /// <summary>
    /// Gets the absolute point in time when the cache entry should expire.
    /// </summary>
    public DateTimeOffset? AbsoluteExpiration { get; }

    /// <summary>
    /// Gets the relative duration after which the cache entry should expire.
    /// </summary>
    public TimeSpan? AbsoluteExpirationRelativeToNow { get; }

    /// <summary>
    /// Gets the duration after the last access when the cache entry should expire.
    /// </summary>
    public TimeSpan? SlidingExpiration { get; }
}
