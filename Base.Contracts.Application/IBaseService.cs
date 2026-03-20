using Base.Contracts.DTO;

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
    /// A task that resolves to the operation result containing either the matching entities or an error.
    /// </returns>
    Task<IMethodResponse<IEnumerable<TEntity>>> GetAllAsync(TActor? actor = default!);

    /// <summary>
    /// Retrieves a single page of entities visible to the specified actor.
    /// </summary>
    /// <param name="pageNr">The one-based page number to retrieve.</param>
    /// <param name="pageSize">The number of items to include in the page.</param>
    /// <param name="actor">The optional actor used to scope the query.</param>
    /// <returns>
    /// A task that resolves to the operation result containing either the matching page of entities or an error.
    /// </returns>
    Task<IMethodResponse<IEnumerable<TEntity>>> GetAllByPageAsync(int pageNr, int pageSize, TActor? actor = default!);

    /// <summary>
    /// Counts all entities visible to the specified actor.
    /// </summary>
    /// <param name="actor">The optional actor used to scope the query.</param>
    /// <returns>
    /// A task that resolves to the operation result containing either the number of matching entities or an error.
    /// </returns>
    Task<IMethodResponse<int>> GetCountAsync(TActor? actor = default!);

    /// <summary>
    /// Determines whether an entity with the specified identifier exists.
    /// </summary>
    /// <param name="id">The identifier of the entity to check.</param>
    /// <param name="actor">The optional actor used to scope the query.</param>
    /// <returns>
    /// A task that resolves to the operation result containing either the existence flag or an error.
    /// </returns>
    Task<IMethodResponse<bool>> ExistsAsync(TKey id, TActor? actor = default!);

    /// <summary>
    /// Retrieves an entity by its identifier.
    /// </summary>
    /// <param name="id">The identifier of the entity to retrieve.</param>
    /// <param name="actor">The optional actor used to scope the query.</param>
    /// <returns>
    /// A task that resolves to the operation result containing either the matching entity or an error.
    /// </returns>
    Task<IMethodResponse<TEntity>> GetByIdAsync(TKey id, TActor? actor = default!);

    /// <summary>
    /// Creates a new entity instance.
    /// </summary>
    /// <param name="entity">The entity to create.</param>
    /// <param name="actor">The optional actor used to scope or stamp the operation.</param>
    /// <returns>
    /// A task that resolves to the operation result containing either the created entity or an error.
    /// </returns>
    Task<IMethodResponse<TEntity>> CreateAsync(TEntity entity, TActor? actor = default!);

    /// <summary>
    /// Updates an existing entity instance.
    /// </summary>
    /// <param name="id">The identifier of the entity to update.</param>
    /// <param name="entity">The new entity state.</param>
    /// <param name="actor">The optional actor used to scope or stamp the operation.</param>
    /// <returns>
    /// A task that resolves to the operation result containing either the updated entity or an error.
    /// </returns>
    Task<IMethodResponse<TEntity>> UpdateAsync(TKey id, TEntity entity, TActor? actor = default!);

    /// <summary>
    /// Removes an entity by its identifier.
    /// </summary>
    /// <param name="id">The identifier of the entity to remove.</param>
    /// <param name="actor">The optional actor used to scope the operation.</param>
    /// <returns>
    /// A task that resolves to the operation result containing either a successful removal flag or an error.
    /// </returns>
    Task<IMethodResponse<bool>> RemoveAsync(TKey id, TActor? actor = default!);
}
