namespace Base.Contracts.Cache;

/// <summary>
/// Defines how logical cache keys are converted into backend keys.
/// </summary>
public interface IBaseCacheKeyBuilder
{
    /// <summary>
    /// Builds the backend cache key from a logical key and optional prefix.
    /// </summary>
    /// <param name="key">The logical cache key supplied by the caller.</param>
    /// <param name="keyPrefix">The optional prefix used to isolate keys for an application or bounded context.</param>
    /// <returns>The key that should be sent to the backing cache store.</returns>
    public string BuildKey(string key, string? keyPrefix = default);
}
