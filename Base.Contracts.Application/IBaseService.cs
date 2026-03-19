namespace Base.Contracts.Application;

/// <summary>
/// Defines a base application service contract for entities that use <see cref="Guid"/> as both the entity key and actor key.
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
/// <typeparam name="TActor">The identifier type of the actor used to scope or stamp service operations.</typeparam>
public interface IBaseService<TEntity, TKey, TActor>
    where TEntity : class
    where TKey : IEquatable<TKey>
    where TActor : IEquatable<TActor>
{
    /// <summary>
    /// Retrieves all entities visible to the specified actor.
    /// </summary>
    /// <param name="actor">The optional actor used to scope the query.</param>
    /// <returns>
    /// A task that resolves to the matching entities, or <see langword="null"/> when no result set is available.
    /// </returns>
    Task<IEnumerable<TEntity>?> GetAllAsync(TActor? actor = default!);

    /// <summary>
    /// Retrieves a single page of entities visible to the specified actor.
    /// </summary>
    /// <param name="pageNr">The one-based page number to retrieve.</param>
    /// <param name="pageSize">The number of items to include in the page.</param>
    /// <param name="actor">The optional actor used to scope the query.</param>
    /// <returns>
    /// A task that resolves to the matching entities for the requested page, or <see langword="null"/> when no result set is available.
    /// </returns>
    Task<IEnumerable<TEntity>?> GetAllByPageAsync(int pageNr, int pageSize, TActor? actor = default!);

    /// <summary>
    /// Counts all entities visible to the specified actor.
    /// </summary>
    /// <param name="actor">The optional actor used to scope the query.</param>
    /// <returns>
    /// A task that resolves to the number of matching entities.
    /// </returns>
    Task<int> GetCountAsync(TActor? actor = default!);

    /// <summary>
    /// Determines whether an entity with the specified identifier exists.
    /// </summary>
    /// <param name="id">The identifier of the entity to check.</param>
    /// <param name="actor">The optional actor used to scope the query.</param>
    /// <returns>
    /// A task that resolves to <see langword="true"/> when the entity exists; otherwise, <see langword="false"/>.
    /// </returns>
    Task<bool> ExistsAsync(TKey id, TActor? actor = default!);

    /// <summary>
    /// Retrieves an entity by its identifier.
    /// </summary>
    /// <param name="id">The identifier of the entity to retrieve.</param>
    /// <param name="actor">The optional actor used to scope the query.</param>
    /// <returns>
    /// A task that resolves to the matching entity, or <see langword="null"/> when it is not found.
    /// </returns>
    Task<TEntity?> GetByIdAsync(TKey id, TActor? actor = default!);

    /// <summary>
    /// Creates a new entity instance.
    /// </summary>
    /// <param name="entity">The entity to create.</param>
    /// <param name="actor">The optional actor used to scope or stamp the operation.</param>
    /// <returns>
    /// A task that resolves to the created entity, or <see langword="null"/> when creation cannot be completed.
    /// </returns>
    Task<TEntity?> CreateAsync(TEntity entity, TActor? actor = default!);

    /// <summary>
    /// Updates an existing entity instance.
    /// </summary>
    /// <param name="id">The identifier of the entity to update.</param>
    /// <param name="entity">The new entity state.</param>
    /// <param name="actor">The optional actor used to scope or stamp the operation.</param>
    /// <returns>
    /// A task that resolves to the updated entity, or <see langword="null"/> when the entity cannot be updated.
    /// </returns>
    Task<TEntity?> UpdateAsync(TKey id, TEntity entity, TActor? actor = default!);

    /// <summary>
    /// Removes an entity by its identifier.
    /// </summary>
    /// <param name="id">The identifier of the entity to remove.</param>
    /// <param name="actor">The optional actor used to scope the operation.</param>
    /// <returns>
    /// A task that resolves to <see langword="true"/> when the entity was removed; otherwise, <see langword="false"/>.
    /// </returns>
    Task<bool> RemoveAsync(TKey id, TActor? actor = default!);
}
