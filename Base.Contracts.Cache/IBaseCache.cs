namespace Base.Contracts.Cache;

/// <summary>
/// Defines a strongly typed key-value cache contract for string-keyed cache stores.
/// </summary>
public interface IBaseCache
{
    /// <summary>
    /// Retrieves a cached value by key and preserves the difference between a missing entry and a cached
    /// <see langword="null"/> value.
    /// </summary>
    /// <param name="key">The cache key to retrieve.</param>
    /// <param name="cancellationToken">A token used to cancel the asynchronous operation.</param>
    /// <typeparam name="TValue">The value type stored for the key.</typeparam>
    /// <returns>A task that resolves to the cache result for the requested key.</returns>
    public Task<IBaseCacheResult<TValue>> GetAsync<TValue>(
        string key,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Stores a value by key with optional expiration metadata.
    /// </summary>
    /// <param name="key">The cache key to write.</param>
    /// <param name="value">The value to cache. Implementations should preserve cached <see langword="null"/> values.</param>
    /// <param name="options">The optional expiration options for this entry.</param>
    /// <param name="cancellationToken">A token used to cancel the asynchronous operation.</param>
    /// <typeparam name="TValue">The value type stored for the key.</typeparam>
    /// <returns>A task that represents the asynchronous cache write operation.</returns>
    public Task SetAsync<TValue>(
        string key,
        TValue? value,
        IBaseCacheEntryOptions? options = default,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Determines whether the cache contains the specified key.
    /// </summary>
    /// <param name="key">The cache key to check.</param>
    /// <param name="cancellationToken">A token used to cancel the asynchronous operation.</param>
    /// <returns>A task that resolves to <see langword="true"/> when the key exists; otherwise, <see langword="false"/>.</returns>
    public Task<bool> ExistsAsync(
        string key,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a cached value by key.
    /// </summary>
    /// <param name="key">The cache key to remove.</param>
    /// <param name="cancellationToken">A token used to cancel the asynchronous operation.</param>
    /// <returns>A task that resolves to <see langword="true"/> when an entry was removed; otherwise, <see langword="false"/>.</returns>
    public Task<bool> RemoveAsync(
        string key,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves an existing cached value or creates, stores, and returns a new value when the key is missing.
    /// </summary>
    /// <param name="key">The cache key to retrieve or populate.</param>
    /// <param name="factory">The factory used to create a value when the key is missing.</param>
    /// <param name="options">The optional expiration options used when the factory value is cached.</param>
    /// <param name="cancellationToken">A token used to cancel the asynchronous operation.</param>
    /// <typeparam name="TValue">The value type stored for the key.</typeparam>
    /// <returns>A task that resolves to the existing or newly cached value.</returns>
    public Task<IBaseCacheResult<TValue>> GetOrSetAsync<TValue>(
        string key,
        Func<CancellationToken, Task<TValue?>> factory,
        IBaseCacheEntryOptions? options = default,
        CancellationToken cancellationToken = default);
}
