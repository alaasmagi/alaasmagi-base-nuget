namespace Base.Contracts.Application;

/// <summary>
/// Defines a base soft-delete aware application service contract for entities that use <see cref="Guid"/> as both the entity key and user key.
/// </summary>
/// <typeparam name="TEntity">The application-layer entity type handled by the service.</typeparam>
public interface IBaseServiceSoftDelete<TEntity> : IBaseServiceSoftDelete<TEntity, Guid, Guid>
    where TEntity : class
{
}

/// <summary>
/// Defines a base soft-delete aware application service contract that extends regular service operations with soft-delete specific members.
/// </summary>
/// <typeparam name="TEntity">The application-layer entity type handled by the service.</typeparam>
/// <typeparam name="TKey">The identifier type of the entity.</typeparam>
/// <typeparam name="TUserKey">The identifier type of the current user or owner.</typeparam>
public interface IBaseServiceSoftDelete<TEntity, TKey, TUserKey> : IBaseService<TEntity, TKey, TUserKey>
    where TEntity : class
    where TKey : IEquatable<TKey>
    where TUserKey : IEquatable<TUserKey>
{
    /// <summary>
    /// Retrieves all entities while optionally including soft-deleted records.
    /// </summary>
    Task<IEnumerable<TEntity>?> GetAllAsync(bool includeSoftDeleted = false, TUserKey? userId = default!);

    /// <summary>
    /// Retrieves a page of entities while optionally including soft-deleted records.
    /// </summary>
    Task<IEnumerable<TEntity>?> GetAllByPageAsync(int pageNr, int pageSize, bool includeSoftDeleted = false, TUserKey? userId = default!);

    /// <summary>
    /// Counts entities while optionally including soft-deleted records.
    /// </summary>
    Task<int> GetCountAsync(bool includeSoftDeleted = false, TUserKey? userId = default!);

    /// <summary>
    /// Retrieves an entity by its identifier while optionally including soft-deleted records.
    /// </summary>
    Task<TEntity?> GetByIdAsync(TKey id, bool includeSoftDeleted = false, TUserKey? userId = default!);

    /// <summary>
    /// Determines whether an entity exists while optionally including soft-deleted records.
    /// </summary>
    Task<bool> ExistsAsync(TKey id, bool includeSoftDeleted = false, TUserKey? userId = default!);

    /// <summary>
    /// Marks an entity as soft deleted and persists the change.
    /// </summary>
    Task<bool> SoftDeleteAsync(TKey id, TUserKey? userId = default!);

    /// <summary>
    /// Restores a soft-deleted entity and persists the change.
    /// </summary>
    Task<TEntity?> RestoreAsync(TKey id, TUserKey? userId = default!);
}
