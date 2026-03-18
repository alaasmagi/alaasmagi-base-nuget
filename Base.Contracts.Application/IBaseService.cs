namespace Base.Contracts.Application;

/// <summary>
/// Defines a base application service contract for entities that use <see cref="Guid"/> as both the entity key and user key.
/// </summary>
/// <typeparam name="TEntity">The application-layer entity type handled by the service.</typeparam>
public interface IBaseService<TEntity> : IBaseService<TEntity, Guid, Guid>
    where TEntity : class
{
}

/// <summary>
/// Defines a base application service contract for CRUD-style service operations.
/// </summary>
/// <typeparam name="TEntity">The application-layer entity type handled by the service.</typeparam>
/// <typeparam name="TKey">The identifier type of the entity.</typeparam>
/// <typeparam name="TUserKey">The identifier type of the current user or owner.</typeparam>
public interface IBaseService<TEntity, TKey, TUserKey>
    where TEntity : class
    where TKey : IEquatable<TKey>
    where TUserKey : IEquatable<TUserKey>
{
    /// <summary>
    /// Retrieves all entities visible to the specified user.
    /// </summary>
    Task<IEnumerable<TEntity>?> GetAllAsync(TUserKey? userId = default!);

    /// <summary>
    /// Retrieves a page of entities visible to the specified user.
    /// </summary>
    Task<IEnumerable<TEntity>?> GetAllByPageAsync(int pageNr, int pageSize, TUserKey? userId = default!);

    /// <summary>
    /// Counts the entities visible to the specified user.
    /// </summary>
    Task<int> GetCountAsync(TUserKey? userId = default!);

    /// <summary>
    /// Determines whether an entity with the specified identifier exists.
    /// </summary>
    Task<bool> ExistsAsync(TKey id, TUserKey? userId = default!);

    /// <summary>
    /// Retrieves an entity by its identifier.
    /// </summary>
    Task<TEntity?> GetByIdAsync(TKey id, TUserKey? userId = default!);

    /// <summary>
    /// Creates a new entity and persists the change.
    /// </summary>
    Task<TEntity?> CreateAsync(TEntity entity, TUserKey? userId = default!);

    /// <summary>
    /// Updates an existing entity and persists the change.
    /// </summary>
    Task<TEntity?> UpdateAsync(TKey id, TEntity entity, TUserKey? userId = default!);

    /// <summary>
    /// Removes an entity by its identifier and persists the change.
    /// </summary>
    Task<bool> RemoveAsync(TKey id, TUserKey? userId = default!);
}
