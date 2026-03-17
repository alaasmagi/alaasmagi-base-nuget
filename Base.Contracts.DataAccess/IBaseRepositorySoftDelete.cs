namespace Base.Contracts.DataAccess;

/// <summary>
/// Defines a soft-delete aware repository contract for entities that use <see cref="Guid"/> as both the entity key and user key.
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
/// <typeparam name="TUserKey">The identifier type of the current user or owner.</typeparam>
public interface IBaseRepositorySoftDelete<TEntity, TResourceKey, TUserKey> : IBaseRepository<TEntity, TResourceKey, TUserKey>
    where TEntity : class
    where TResourceKey : IEquatable<TResourceKey>
    where TUserKey : IEquatable<TUserKey>
{
    /// <summary>
    /// Retrieves all entities while optionally including soft-deleted records.
    /// </summary>
    /// <param name="includeSoftDeleted">Controls whether soft-deleted entities are included in the result.</param>
    /// <param name="userId">The optional user identifier used to scope the query.</param>
    /// <returns>
    /// A task that resolves to the matching entities, or <see langword="null"/> when no result set is available.
    /// </returns>
    Task<IEnumerable<TEntity>?> GetAllAsync(bool includeSoftDeleted = false, TUserKey? userId = default!);

    /// <summary>
    /// Retrieves a single page of entities while optionally including soft-deleted records.
    /// </summary>
    /// <param name="pageNr">The one-based page number to retrieve.</param>
    /// <param name="pageSize">The number of items to include in the page.</param>
    /// <param name="includeSoftDeleted">Controls whether soft-deleted entities are included in the result.</param>
    /// <param name="userId">The optional user identifier used to scope the query.</param>
    /// <returns>
    /// A task that resolves to the matching entities for the requested page, or <see langword="null"/> when no result set is available.
    /// </returns>
    Task<IEnumerable<TEntity>?> GetAllByPageAsync(int pageNr, int pageSize, bool includeSoftDeleted = false, TUserKey? userId = default!);

    /// <summary>
    /// Counts entities while optionally including soft-deleted records.
    /// </summary>
    /// <param name="includeSoftDeleted">Controls whether soft-deleted entities are included in the count.</param>
    /// <param name="userId">The optional user identifier used to scope the query.</param>
    /// <returns>
    /// A task that resolves to the number of matching entities.
    /// </returns>
    Task<int> GetCountAsync(bool includeSoftDeleted = false, TUserKey? userId = default!);

    /// <summary>
    /// Retrieves an entity by its identifier while optionally including soft-deleted records.
    /// </summary>
    /// <param name="id">The identifier of the entity to retrieve.</param>
    /// <param name="includeSoftDeleted">Controls whether soft-deleted entities are included in the search.</param>
    /// <param name="userId">The optional user identifier used to scope the query.</param>
    /// <returns>
    /// A task that resolves to the matching entity, or <see langword="null"/> when it is not found.
    /// </returns>
    Task<TEntity?> GetByIdAsync(TResourceKey id, bool includeSoftDeleted = false, TUserKey? userId = default!);

    /// <summary>
    /// Determines whether an entity exists while optionally including soft-deleted records.
    /// </summary>
    /// <param name="id">The identifier of the entity to check.</param>
    /// <param name="includeSoftDeleted">Controls whether soft-deleted entities are included in the search.</param>
    /// <param name="userId">The optional user identifier used to scope the query.</param>
    /// <returns>
    /// A task that resolves to <see langword="true"/> when the entity exists; otherwise, <see langword="false"/>.
    /// </returns>
    Task<bool> ExistsAsync(TResourceKey id, bool includeSoftDeleted = false, TUserKey? userId = default!);

    /// <summary>
    /// Marks an entity as soft deleted.
    /// </summary>
    /// <param name="id">The identifier of the entity to soft delete.</param>
    /// <param name="userId">The optional user identifier used to scope or stamp the operation.</param>
    /// <returns>
    /// A task that resolves to <see langword="true"/> when the entity was marked as deleted; otherwise, <see langword="false"/>.
    /// </returns>
    Task<bool> SoftDeleteAsync(TResourceKey id, TUserKey? userId = default!);

    /// <summary>
    /// Restores a previously soft-deleted entity.
    /// </summary>
    /// <param name="id">The identifier of the entity to restore.</param>
    /// <param name="userId">The optional user identifier used to scope or stamp the operation.</param>
    /// <returns>
    /// A task that resolves to the restored entity, or <see langword="null"/> when the entity cannot be restored.
    /// </returns>
    Task<TEntity?> RestoreAsync(TResourceKey id, TUserKey? userId = default!);
}
