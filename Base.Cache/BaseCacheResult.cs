using Base.Contracts.Cache;

namespace Base.Cache;

/// <summary>
/// Provides a cache lookup result that preserves hit and miss state separately from the cached value.
/// </summary>
/// <typeparam name="TValue">The cached value type.</typeparam>
public class BaseCacheResult<TValue> : IBaseCacheResult<TValue>
{
    /// <summary>
    /// Gets a value indicating whether the cache contained the requested key.
    /// </summary>
    public bool Found { get; }

    /// <summary>
    /// Gets the cached value when <see cref="Found"/> is <see langword="true"/>.
    /// </summary>
    public TValue? Value { get; }

    private BaseCacheResult(bool found, TValue? value = default)
    {
        Found = found;
        Value = value;
    }

    /// <summary>
    /// Creates a cache hit result with the supplied value.
    /// </summary>
    /// <param name="value">The cached value. This may be <see langword="null"/>.</param>
    /// <returns>A cache hit result.</returns>
    public static BaseCacheResult<TValue> Hit(TValue? value) => new(true, value);

    /// <summary>
    /// Creates a cache miss result.
    /// </summary>
    /// <returns>A cache miss result.</returns>
    public static BaseCacheResult<TValue> Miss() => new(false);
}
