namespace Base.Contracts.Cache;

/// <summary>
/// Represents the result of a cache lookup.
/// </summary>
/// <typeparam name="TValue">The cached value type.</typeparam>
public interface IBaseCacheResult<TValue>
{
    /// <summary>
    /// Gets a value indicating whether the cache contained the requested key.
    /// </summary>
    public bool Found { get; }

    /// <summary>
    /// Gets the cached value when <see cref="Found"/> is <see langword="true"/>.
    /// </summary>
    public TValue? Value { get; }
}
