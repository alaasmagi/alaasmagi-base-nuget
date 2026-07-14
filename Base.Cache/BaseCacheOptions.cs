using Base.Contracts.Cache;

namespace Base.Cache;

/// <summary>
/// Provides shared configuration for base cache implementations.
/// </summary>
public class BaseCacheOptions
{
    /// <summary>
    /// Gets or sets the optional prefix applied to all cache keys.
    /// </summary>
    public string? KeyPrefix { get; set; }

    /// <summary>
    /// Gets or sets the default entry options used when an operation does not provide options explicitly.
    /// </summary>
    public IBaseCacheEntryOptions? DefaultEntryOptions { get; set; }
}
