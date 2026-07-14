using Base.Contracts.Cache;

namespace Base.Cache;

/// <summary>
/// Provides a reusable byte-backed implementation for typed key-value cache stores.
/// </summary>
public abstract class BaseCache : IBaseCache
{
    /// <summary>
    /// Stores the serializer used to convert typed values to backend byte payloads.
    /// </summary>
    protected readonly IBaseCacheSerializer CacheSerializer;

    /// <summary>
    /// Stores the key builder used to normalize cache keys before backend operations.
    /// </summary>
    protected readonly IBaseCacheKeyBuilder CacheKeyBuilder;

    /// <summary>
    /// Stores default cache behavior shared by all operations.
    /// </summary>
    protected readonly BaseCacheOptions CacheOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseCache"/> class.
    /// </summary>
    /// <param name="cacheSerializer">The serializer used to convert typed values to bytes.</param>
    /// <param name="cacheKeyBuilder">The key builder used to normalize cache keys.</param>
    /// <param name="cacheOptions">The optional default cache options.</param>
    protected BaseCache(
        IBaseCacheSerializer cacheSerializer,
        IBaseCacheKeyBuilder cacheKeyBuilder,
        BaseCacheOptions? cacheOptions = default)
    {
        ArgumentNullException.ThrowIfNull(cacheSerializer);
        ArgumentNullException.ThrowIfNull(cacheKeyBuilder);

        CacheSerializer = cacheSerializer;
        CacheKeyBuilder = cacheKeyBuilder;
        CacheOptions = cacheOptions ?? new BaseCacheOptions();
    }

    /// <summary>
    /// Retrieves and deserializes a cached value by key.
    /// </summary>
    public virtual async Task<IBaseCacheResult<TValue>> GetAsync<TValue>(
        string key,
        CancellationToken cancellationToken = default)
    {
        var cacheKey = BuildCacheKey(key);
        var bytes = await GetBytesAsync(cacheKey, cancellationToken);

        if (bytes is null)
        {
            return BaseCacheResult<TValue>.Miss();
        }

        var value = CacheSerializer.Deserialize<TValue>(bytes);
        return BaseCacheResult<TValue>.Hit(value);
    }

    /// <summary>
    /// Serializes and stores a typed value by key.
    /// </summary>
    public virtual async Task SetAsync<TValue>(
        string key,
        TValue? value,
        IBaseCacheEntryOptions? options = default,
        CancellationToken cancellationToken = default)
    {
        var cacheKey = BuildCacheKey(key);
        var entryOptions = ResolveEntryOptions(options);
        ValidateEntryOptions(entryOptions);

        var bytes = CacheSerializer.Serialize(value);
        await SetBytesAsync(cacheKey, bytes, entryOptions, cancellationToken);
    }

    /// <summary>
    /// Determines whether the normalized cache key exists in the backing store.
    /// </summary>
    public virtual Task<bool> ExistsAsync(
        string key,
        CancellationToken cancellationToken = default)
    {
        return ExistsByKeyAsync(BuildCacheKey(key), cancellationToken);
    }

    /// <summary>
    /// Removes a normalized cache key from the backing store.
    /// </summary>
    public virtual Task<bool> RemoveAsync(
        string key,
        CancellationToken cancellationToken = default)
    {
        return RemoveByKeyAsync(BuildCacheKey(key), cancellationToken);
    }

    /// <summary>
    /// Reads a value from cache or populates the key from the supplied factory when the key is missing.
    /// </summary>
    public virtual async Task<IBaseCacheResult<TValue>> GetOrSetAsync<TValue>(
        string key,
        Func<CancellationToken, Task<TValue?>> factory,
        IBaseCacheEntryOptions? options = default,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(factory);

        var existing = await GetAsync<TValue>(key, cancellationToken);
        if (existing.Found)
        {
            return existing;
        }

        var value = await factory(cancellationToken);
        await SetAsync(key, value, options, cancellationToken);
        return BaseCacheResult<TValue>.Hit(value);
    }

    /// <summary>
    /// Builds the backend key used for cache operations.
    /// </summary>
    /// <param name="key">The caller-supplied logical cache key.</param>
    /// <returns>The normalized backend cache key.</returns>
    protected virtual string BuildCacheKey(string key)
    {
        return CacheKeyBuilder.BuildKey(key, CacheOptions.KeyPrefix);
    }

    /// <summary>
    /// Resolves per-entry options using explicit options first, then defaults configured for the cache.
    /// </summary>
    /// <param name="options">The options supplied to a specific cache write.</param>
    /// <returns>The options that should be applied to the cache write.</returns>
    protected virtual IBaseCacheEntryOptions ResolveEntryOptions(IBaseCacheEntryOptions? options = default)
    {
        return options ?? CacheOptions.DefaultEntryOptions ?? BaseCacheEntryOptions.Default;
    }

    /// <summary>
    /// Validates expiration options before they are passed to a backend implementation.
    /// </summary>
    /// <param name="options">The options to validate.</param>
    protected virtual void ValidateEntryOptions(IBaseCacheEntryOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        if (options.AbsoluteExpirationRelativeToNow is { } absoluteRelativeToNow &&
            absoluteRelativeToNow <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(
                nameof(options),
                "AbsoluteExpirationRelativeToNow must be greater than zero when provided.");
        }

        if (options.SlidingExpiration is { } slidingExpiration && slidingExpiration <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(
                nameof(options),
                "SlidingExpiration must be greater than zero when provided.");
        }
    }

    /// <summary>
    /// Retrieves raw bytes for an already-normalized backend key.
    /// </summary>
    /// <param name="key">The normalized backend cache key.</param>
    /// <param name="cancellationToken">A token used to cancel the asynchronous operation.</param>
    /// <returns>The stored byte payload, or <see langword="null"/> when the key is missing.</returns>
    protected abstract Task<byte[]?> GetBytesAsync(
        string key,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Stores raw bytes for an already-normalized backend key.
    /// </summary>
    /// <param name="key">The normalized backend cache key.</param>
    /// <param name="value">The serialized byte payload to store.</param>
    /// <param name="options">The expiration options to apply to this entry.</param>
    /// <param name="cancellationToken">A token used to cancel the asynchronous operation.</param>
    protected abstract Task SetBytesAsync(
        string key,
        byte[] value,
        IBaseCacheEntryOptions options,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Determines whether an already-normalized backend key exists.
    /// </summary>
    /// <param name="key">The normalized backend cache key.</param>
    /// <param name="cancellationToken">A token used to cancel the asynchronous operation.</param>
    /// <returns>A task that resolves to whether the key exists.</returns>
    protected abstract Task<bool> ExistsByKeyAsync(
        string key,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes an already-normalized backend key.
    /// </summary>
    /// <param name="key">The normalized backend cache key.</param>
    /// <param name="cancellationToken">A token used to cancel the asynchronous operation.</param>
    /// <returns>A task that resolves to whether an entry was removed.</returns>
    protected abstract Task<bool> RemoveByKeyAsync(
        string key,
        CancellationToken cancellationToken = default);
}
