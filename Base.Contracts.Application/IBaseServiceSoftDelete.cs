namespace Base.Contracts.Application;

/// <summary>
/// Defines a base soft-delete aware application service contract for entities that use <see cref="Guid"/> as both the entity key and actor key.
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
/// <typeparam name="TActor">The identifier type of the actor used to scope or stamp service operations.</typeparam>
public interface IBaseServiceSoftDelete<TEntity, TKey, TActor> : IBaseService<TEntity, TKey, TActor>
    where TEntity : class
    where TKey : IEquatable<TKey>
    where TActor : IEquatable<TActor>
{
    /// <summary>
    /// Retrieves all entities visible to the specified actor while optionally including soft-deleted records.
    /// </summary>
    /// <param name="includeSoftDeleted">Controls whether soft-deleted entities are included in the result.</param>
    /// <param name="actor">The optional actor used to scope the query.</param>
    /// <returns>
    /// A task that resolves to the matching entities, or <see langword="null"/> when no result set is available.
    /// </returns>
    Task<IEnumerable<TEntity>?> GetAllAsync(bool includeSoftDeleted = false, TActor? actor = default!);

    /// <summary>
    /// Retrieves a single page of entities visible to the specified actor while optionally including soft-deleted records.
    /// </summary>
    /// <param name="pageNr">The one-based page number to retrieve.</param>
    /// <param name="pageSize">The number of items to include in the page.</param>
    /// <param name="includeSoftDeleted">Controls whether soft-deleted entities are included in the result.</param>
    /// <param name="actor">The optional actor used to scope the query.</param>
    /// <returns>
    /// A task that resolves to the matching entities for the requested page, or <see langword="null"/> when no result set is available.
    /// </returns>
    Task<IEnumerable<TEntity>?> GetAllByPageAsync(int pageNr, int pageSize, bool includeSoftDeleted = false, TActor? actor = default!);

    /// <summary>
    /// Counts all entities visible to the specified actor while optionally including soft-deleted records.
    /// </summary>
    /// <param name="includeSoftDeleted">Controls whether soft-deleted entities are included in the count.</param>
    /// <param name="actor">The optional actor used to scope the query.</param>
    /// <returns>
    /// A task that resolves to the number of matching entities.
    /// </returns>
    Task<int> GetCountAsync(bool includeSoftDeleted = false, TActor? actor = default!);

    /// <summary>
    /// Retrieves an entity by its identifier while optionally including soft-deleted records visible to the specified actor.
    /// </summary>
    /// <param name="id">The identifier of the entity to retrieve.</param>
    /// <param name="includeSoftDeleted">Controls whether soft-deleted entities are included in the search.</param>
    /// <param name="actor">The optional actor used to scope the query.</param>
    /// <returns>
    /// A task that resolves to the matching entity, or <see langword="null"/> when it is not found.
    /// </returns>
    Task<TEntity?> GetByIdAsync(TKey id, bool includeSoftDeleted = false, TActor? actor = default!);

    /// <summary>
    /// Determines whether an entity with the specified identifier exists while optionally including soft-deleted records visible to the specified actor.
    /// </summary>
    /// <param name="id">The identifier of the entity to check.</param>
    /// <param name="includeSoftDeleted">Controls whether soft-deleted entities are included in the search.</param>
    /// <param name="actor">The optional actor used to scope the query.</param>
    /// <returns>
    /// A task that resolves to <see langword="true"/> when the entity exists; otherwise, <see langword="false"/>.
    /// </returns>
    Task<bool> ExistsAsync(TKey id, bool includeSoftDeleted = false, TActor? actor = default!);

    /// <summary>
    /// Marks an entity as soft deleted.
    /// </summary>
    /// <param name="id">The identifier of the entity to soft delete.</param>
    /// <param name="actor">The optional actor used to scope or stamp the operation.</param>
    /// <returns>
    /// A task that resolves to <see langword="true"/> when the entity was marked as deleted; otherwise, <see langword="false"/>.
    /// </returns>
    Task<bool> SoftDeleteAsync(TKey id, TActor? actor = default!);

    /// <summary>
    /// Restores a previously soft-deleted entity.
    /// </summary>
    /// <param name="id">The identifier of the entity to restore.</param>
    /// <param name="actor">The optional actor used to scope or stamp the operation.</param>
    /// <returns>
    /// A task that resolves to the restored entity, or <see langword="null"/> when the entity cannot be restored.
    /// </returns>
    Task<TEntity?> RestoreAsync(TKey id, TActor? actor = default!);
}
