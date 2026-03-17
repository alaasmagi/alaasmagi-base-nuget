using Base.Contracts.DataAccess;
using Base.Contracts.Domain;
using Microsoft.EntityFrameworkCore;

namespace Base.DataAccess.EF;

/// <summary>
/// Provides a reusable Entity Framework repository implementation for entities that support soft delete.
/// </summary>
/// <typeparam name="TResourceEntity">The entity type managed by the repository.</typeparam>
/// <typeparam name="TResourceKey">The identifier type of the entity.</typeparam>
/// <typeparam name="TUserKey">The identifier type of the current user or owner.</typeparam>
public class BaseRepositorySoftDelete<TResourceEntity, TResourceKey, TUserKey> :
    BaseRepository<TResourceEntity, TResourceKey, TUserKey>,
    IBaseRepositorySoftDelete<TResourceEntity, TResourceKey, TUserKey>
    where TResourceEntity : class, IBaseEntity<TResourceKey>, IBaseEntitySoftDelete
    where TResourceKey : IEquatable<TResourceKey>
    where TUserKey : IEquatable<TUserKey>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BaseRepositorySoftDelete{TResourceEntity, TResourceKey, TUserKey}"/> class.
    /// </summary>
    /// <param name="repositoryDbContext">The database context used by the repository.</param>
    public BaseRepositorySoftDelete(DbContext repositoryDbContext)
        : base(repositoryDbContext)
    {
    }

    /// <summary>
    /// Builds the base query while optionally including soft-deleted records.
    /// </summary>
    /// <param name="includeSoftDeleted">Controls whether soft-deleted entities are included in the result.</param>
    /// <param name="userId">The optional user identifier used to scope the query.</param>
    /// <param name="asTracking">Controls whether Entity Framework change tracking is enabled for the query.</param>
    /// <returns>
    /// An <see cref="IQueryable{T}"/> representing the base query for the repository.
    /// </returns>
    protected virtual IQueryable<TResourceEntity> GetQuery(bool includeSoftDeleted = false, TUserKey? userId = default!, bool asTracking = false)
    {
        var query = base.GetQuery(userId, asTracking);

        if (!includeSoftDeleted)
        {
            query = query.Where(entity => !entity.IsDeleted);
        }

        return query;
    }

    /// <summary>
    /// Retrieves all entities while optionally including soft-deleted records.
    /// </summary>
    /// <param name="includeSoftDeleted">Controls whether soft-deleted entities are included in the result.</param>
    /// <param name="userId">The optional user identifier used to scope the query.</param>
    /// <returns>
    /// A task that resolves to the matching entities, or <see langword="null"/> when no result set is available.
    /// </returns>
    public async Task<IEnumerable<TResourceEntity>?> GetAllAsync(bool includeSoftDeleted = false, TUserKey? userId = default!)
    {
        return await GetQuery(includeSoftDeleted, userId).ToListAsync();
    }

    /// <summary>
    /// Retrieves a page of entities while optionally including soft-deleted records.
    /// </summary>
    /// <param name="pageNr">The one-based page number to retrieve.</param>
    /// <param name="pageSize">The number of items to include in the page.</param>
    /// <param name="includeSoftDeleted">Controls whether soft-deleted entities are included in the result.</param>
    /// <param name="userId">The optional user identifier used to scope the query.</param>
    /// <returns>
    /// A task that resolves to the matching entities for the requested page, or <see langword="null"/> when no result set is available.
    /// </returns>
    public async Task<IEnumerable<TResourceEntity>?> GetAllByPageAsync(int pageNr, int pageSize, bool includeSoftDeleted = false, TUserKey? userId = default!)
    {
        ValidatePaging(pageNr, pageSize);

        return await GetQuery(includeSoftDeleted, userId)
            .Skip((pageNr - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    /// <summary>
    /// Counts entities while optionally including soft-deleted records.
    /// </summary>
    /// <param name="includeSoftDeleted">Controls whether soft-deleted entities are included in the count.</param>
    /// <param name="userId">The optional user identifier used to scope the query.</param>
    /// <returns>
    /// A task that resolves to the number of matching entities.
    /// </returns>
    public async Task<int> GetCountAsync(bool includeSoftDeleted = false, TUserKey? userId = default!)
    {
        return await GetQuery(includeSoftDeleted, userId).CountAsync();
    }

    /// <summary>
    /// Retrieves an entity by its identifier while optionally including soft-deleted records.
    /// </summary>
    /// <param name="id">The identifier of the entity to retrieve.</param>
    /// <param name="includeSoftDeleted">Controls whether soft-deleted entities are included in the search.</param>
    /// <param name="userId">The optional user identifier used to scope the query.</param>
    /// <returns>
    /// A task that resolves to the matching entity, or <see langword="null"/> when it is not found.
    /// </returns>
    public async Task<TResourceEntity?> GetByIdAsync(TResourceKey id, bool includeSoftDeleted = false, TUserKey? userId = default!)
    {
        return await GetQuery(includeSoftDeleted, userId)
            .FirstOrDefaultAsync(entity => entity.Id.Equals(id));
    }

    /// <summary>
    /// Determines whether an entity exists while optionally including soft-deleted records.
    /// </summary>
    /// <param name="id">The identifier of the entity to check.</param>
    /// <param name="includeSoftDeleted">Controls whether soft-deleted entities are included in the search.</param>
    /// <param name="userId">The optional user identifier used to scope the query.</param>
    /// <returns>
    /// A task that resolves to <see langword="true"/> when the entity exists; otherwise, <see langword="false"/>.
    /// </returns>
    public async Task<bool> ExistsAsync(TResourceKey id, bool includeSoftDeleted = false, TUserKey? userId = default!)
    {
        return await GetQuery(includeSoftDeleted, userId)
            .AnyAsync(entity => entity.Id.Equals(id));
    }

    /// <summary>
    /// Marks an entity as soft deleted.
    /// </summary>
    /// <param name="id">The identifier of the entity to soft delete.</param>
    /// <param name="userId">The optional user identifier used to scope or stamp the operation.</param>
    /// <returns>
    /// A task that resolves to <see langword="true"/> when the entity was marked as deleted; otherwise, <see langword="false"/>.
    /// </returns>
    public async Task<bool> SoftDeleteAsync(TResourceKey id, TUserKey? userId = default!)
    {
        var entity = await GetQuery(true, userId, asTracking: true)
            .FirstOrDefaultAsync(resourceEntity => resourceEntity.Id.Equals(id));

        if (entity == null || entity.IsDeleted)
        {
            return false;
        }

        entity.IsDeleted = true;
        ApplyModificationMetadata(entity, userId);
        return true;
    }

    /// <summary>
    /// Restores a previously soft-deleted entity.
    /// </summary>
    /// <param name="id">The identifier of the entity to restore.</param>
    /// <param name="userId">The optional user identifier used to scope or stamp the operation.</param>
    /// <returns>
    /// A task that resolves to the restored entity, or <see langword="null"/> when the entity cannot be restored.
    /// </returns>
    public async Task<TResourceEntity?> RestoreAsync(TResourceKey id, TUserKey? userId = default!)
    {
        var entity = await GetQuery(true, userId, asTracking: true)
            .FirstOrDefaultAsync(resourceEntity => resourceEntity.Id.Equals(id));

        if (entity == null || !entity.IsDeleted)
        {
            return null;
        }

        entity.IsDeleted = false;
        ApplyModificationMetadata(entity, userId);
        return entity;
    }
}
