using Base.Contracts.DTO;

namespace Base.Contracts.DataAccess;

/// <summary>
/// Defines a soft-delete aware repository contract for entities that use <see cref="Guid"/> as both the resource key and actor key.
/// </summary>
/// <typeparam name="TEntity">The entity type managed by the repository.</typeparam>
public interface IBaseRepositorySoftDelete<TEntity> : IBaseRepositorySoftDelete<TEntity, Guid, Guid>
    where TEntity : class
{
}

/// <summary>
/// Defines a repository contract that extends CRUD operations with soft-delete support.
/// </summary>
/// <typeparam name="TEntity">The entity type managed by the repository.</typeparam>
/// <typeparam name="TResourceKey">The identifier type of the entity.</typeparam>
/// <typeparam name="TActor">The identifier type of the actor used to scope or stamp repository operations.</typeparam>
public interface IBaseRepositorySoftDelete<TEntity, TResourceKey, TActor> : IBaseRepository<TEntity, TResourceKey, TActor>
    where TEntity : class
    where TResourceKey : IEquatable<TResourceKey>
    where TActor : IEquatable<TActor>
{
    /// <summary>
    /// Retrieves all entities while optionally including soft-deleted records.
    /// </summary>
    /// <param name="includeSoftDeleted">Controls whether soft-deleted entities are included in the result.</param>
    /// <param name="actor">The optional actor used to scope the query.</param>
    /// <returns>
    /// A task that resolves to the operation result containing either the matching entities or an error.
    /// </returns>
    Task<IMethodResponse<IEnumerable<TEntity>>> GetAllAsync(bool includeSoftDeleted = false, TActor? actor = default!);

    /// <summary>
    /// Retrieves a single page of entities while optionally including soft-deleted records.
    /// </summary>
    /// <param name="pageNr">The one-based page number to retrieve.</param>
    /// <param name="pageSize">The number of items to include in the page.</param>
    /// <param name="includeSoftDeleted">Controls whether soft-deleted entities are included in the result.</param>
    /// <param name="actor">The optional actor used to scope the query.</param>
    /// <returns>
    /// A task that resolves to the operation result containing either the matching page of entities or an error.
    /// </returns>
    Task<IMethodResponse<IEnumerable<TEntity>>> GetAllByPageAsync(int pageNr, int pageSize, bool includeSoftDeleted = false, TActor? actor = default!);

    /// <summary>
    /// Counts entities while optionally including soft-deleted records.
    /// </summary>
    /// <param name="includeSoftDeleted">Controls whether soft-deleted entities are included in the count.</param>
    /// <param name="actor">The optional actor used to scope the query.</param>
    /// <returns>
    /// A task that resolves to the operation result containing either the number of matching entities or an error.
    /// </returns>
    Task<IMethodResponse<int>> GetCountAsync(bool includeSoftDeleted = false, TActor? actor = default!);

    /// <summary>
    /// Retrieves an entity by its identifier while optionally including soft-deleted records.
    /// </summary>
    /// <param name="id">The identifier of the entity to retrieve.</param>
    /// <param name="includeSoftDeleted">Controls whether soft-deleted entities are included in the search.</param>
    /// <param name="actor">The optional actor used to scope the query.</param>
    /// <returns>
    /// A task that resolves to the operation result containing either the matching entity or an error.
    /// </returns>
    Task<IMethodResponse<TEntity>> GetByIdAsync(TResourceKey id, bool includeSoftDeleted = false, TActor? actor = default!);

    /// <summary>
    /// Determines whether an entity exists while optionally including soft-deleted records.
    /// </summary>
    /// <param name="id">The identifier of the entity to check.</param>
    /// <param name="includeSoftDeleted">Controls whether soft-deleted entities are included in the search.</param>
    /// <param name="actor">The optional actor used to scope the query.</param>
    /// <returns>
    /// A task that resolves to the operation result containing either the existence flag or an error.
    /// </returns>
    Task<IMethodResponse<bool>> ExistsAsync(TResourceKey id, bool includeSoftDeleted = false, TActor? actor = default!);

    /// <summary>
    /// Marks an entity as soft deleted.
    /// </summary>
    /// <param name="id">The identifier of the entity to soft delete.</param>
    /// <param name="actor">The optional actor used to scope or stamp the operation.</param>
    /// <returns>
    /// A task that resolves to the operation result containing either a successful soft-delete flag or an error.
    /// </returns>
    Task<IMethodResponse<bool>> SoftDeleteAsync(TResourceKey id, TActor? actor = default!);

    /// <summary>
    /// Restores a previously soft-deleted entity.
    /// </summary>
    /// <param name="id">The identifier of the entity to restore.</param>
    /// <param name="actor">The optional actor used to scope or stamp the operation.</param>
    /// <returns>
    /// A task that resolves to the operation result containing either the restored entity or an error.
    /// </returns>
    Task<IMethodResponse<TEntity>> RestoreAsync(TResourceKey id, TActor? actor = default!);
}
