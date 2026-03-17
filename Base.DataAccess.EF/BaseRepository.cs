using Base.Contracts.DataAccess;
using Base.Contracts.Domain;
using Microsoft.EntityFrameworkCore;

namespace Base.DataAccess.EF;

/// <summary>
/// Provides a reusable Entity Framework repository implementation for entities with an identifier.
/// </summary>
/// <typeparam name="TResourceEntity">The entity type managed by the repository.</typeparam>
/// <typeparam name="TResourceKey">The identifier type of the entity.</typeparam>
/// <typeparam name="TUserKey">The identifier type of the current user or owner.</typeparam>
public class BaseRepository<TResourceEntity, TResourceKey, TUserKey> : IBaseRepository<TResourceEntity, TResourceKey, TUserKey>
    where TResourceEntity : class, IBaseEntity<TResourceKey>
    where TResourceKey : IEquatable<TResourceKey>
    where TUserKey : IEquatable<TUserKey>
{
    /// <summary>
    /// Stores the database context used by the repository.
    /// </summary>
    protected readonly DbContext RepositoryDbContext;

    /// <summary>
    /// Stores the entity set used for CRUD operations.
    /// </summary>
    protected readonly DbSet<TResourceEntity> RepositoryDbSet;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseRepository{TResourceEntity, TResourceKey, TUserKey}"/> class.
    /// </summary>
    /// <param name="repositoryDbContext">The database context used by the repository.</param>
    public BaseRepository(DbContext repositoryDbContext)
    {
        RepositoryDbContext = repositoryDbContext;
        RepositoryDbSet = RepositoryDbContext.Set<TResourceEntity>();
    }

    /// <summary>
    /// Builds the base query for the repository while optionally filtering by user ownership and tracking mode.
    /// </summary>
    /// <param name="userId">The optional user identifier used to scope the query.</param>
    /// <param name="asTracking">Controls whether Entity Framework change tracking is enabled for the query.</param>
    /// <returns>
    /// An <see cref="IQueryable{T}"/> representing the base query for the repository.
    /// </returns>
    protected virtual IQueryable<TResourceEntity> GetQuery(TUserKey? userId = default!, bool asTracking = false)
    {
        var query = RepositoryDbSet.AsQueryable();

        if (!asTracking)
        {
            query = query.AsNoTracking();
        }

        if (ShouldUseUserId(userId))
        {
            query = query.Where(e => ((IBaseEntityUserId<TUserKey>)e).UserId.Equals(userId));
        }

        return query;
    }

    /// <summary>
    /// Validates paging arguments and throws when the values are out of range.
    /// </summary>
    /// <param name="pageNr">The one-based page number to validate.</param>
    /// <param name="pageSize">The number of items per page to validate.</param>
    protected virtual void ValidatePaging(int pageNr, int pageSize)
    {
        if (pageNr <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(pageNr), pageNr, "Page number must be greater than 0.");
        }

        if (pageSize <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(pageSize), pageSize, "Page size must be greater than 0.");
        }
    }

    /// <summary>
    /// Determines whether the entity type supports user scoping and a non-default user identifier was provided.
    /// </summary>
    /// <param name="userId">The optional user identifier used to scope the query or command.</param>
    /// <returns>
    /// <see langword="true"/> when user scoping should be applied; otherwise, <see langword="false"/>.
    /// </returns>
    private bool ShouldUseUserId(TUserKey? userId = default!)
    {
        return typeof(IBaseEntityUserId<TUserKey>).IsAssignableFrom(typeof(TResourceEntity)) &&
               userId != null &&
               !EqualityComparer<TUserKey>.Default.Equals(userId, default);
    }

    /// <summary>
    /// Determines whether the entity type supports metadata fields.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> when the entity type implements <see cref="IBaseEntityMeta"/>; otherwise, <see langword="false"/>.
    /// </returns>
    private static bool HasMeta()
    {
        return typeof(IBaseEntityMeta).IsAssignableFrom(typeof(TResourceEntity));
    }

    /// <summary>
    /// Converts a user identifier to its string representation for metadata fields.
    /// </summary>
    /// <param name="userId">The optional user identifier to convert.</param>
    /// <returns>
    /// The string representation of the user identifier, or <see langword="null"/> when the identifier is not set.
    /// </returns>
    private static string? GetUserIdentifier(TUserKey? userId = default!)
    {
        if (userId == null || EqualityComparer<TUserKey>.Default.Equals(userId, default!))
        {
            return null;
        }

        return userId.ToString();
    }

    /// <summary>
    /// Applies creation metadata to an entity when the entity type supports metadata fields.
    /// </summary>
    /// <param name="entity">The entity being created.</param>
    /// <param name="userId">The optional user identifier used to populate metadata fields.</param>
    protected virtual void ApplyCreateMetadata(TResourceEntity entity, TUserKey? userId = default!)
    {
        if (!HasMeta())
        {
            return;
        }

        var now = DateTime.UtcNow;
        var userIdentifier = GetUserIdentifier(userId);
        var metaEntity = (IBaseEntityMeta)entity;

        metaEntity.CreatedAt = now;
        metaEntity.UpdatedAt = now;

        if (!string.IsNullOrWhiteSpace(userIdentifier))
        {
            metaEntity.CreatedBy = userIdentifier;
            metaEntity.UpdatedBy = userIdentifier;
        }
    }

    /// <summary>
    /// Applies update metadata to an entity while preserving the original creation metadata.
    /// </summary>
    /// <param name="entity">The updated entity instance.</param>
    /// <param name="existingEntity">The existing persisted entity instance.</param>
    /// <param name="userId">The optional user identifier used to populate metadata fields.</param>
    protected virtual void ApplyUpdateMetadata(TResourceEntity entity, TResourceEntity existingEntity, TUserKey? userId = default!)
    {
        if (!HasMeta())
        {
            return;
        }

        var now = DateTime.UtcNow;
        var userIdentifier = GetUserIdentifier(userId);
        var metaEntity = (IBaseEntityMeta)entity;
        var existingMetaEntity = (IBaseEntityMeta)existingEntity;

        metaEntity.CreatedAt = existingMetaEntity.CreatedAt;
        metaEntity.CreatedBy = existingMetaEntity.CreatedBy;
        metaEntity.UpdatedAt = now;

        if (!string.IsNullOrWhiteSpace(userIdentifier))
        {
            metaEntity.UpdatedBy = userIdentifier;
        }
        else
        {
            metaEntity.UpdatedBy = existingMetaEntity.UpdatedBy;
        }
    }

    /// <summary>
    /// Applies modification metadata to an entity when the entity type supports metadata fields.
    /// </summary>
    /// <param name="entity">The entity being modified.</param>
    /// <param name="userId">The optional user identifier used to populate metadata fields.</param>
    protected virtual void ApplyModificationMetadata(TResourceEntity entity, TUserKey? userId = default!)
    {
        if (!HasMeta())
        {
            return;
        }

        var userIdentifier = GetUserIdentifier(userId);
        var metaEntity = (IBaseEntityMeta)entity;
        metaEntity.UpdatedAt = DateTime.UtcNow;

        if (!string.IsNullOrWhiteSpace(userIdentifier))
        {
            metaEntity.UpdatedBy = userIdentifier;
        }
    }

    /// <summary>
    /// Retrieves all entities visible to the specified user.
    /// </summary>
    /// <param name="userId">The optional user identifier used to scope the query.</param>
    /// <returns>
    /// A task that resolves to the matching entities, or <see langword="null"/> when no result set is available.
    /// </returns>
    public async Task<IEnumerable<TResourceEntity>?> GetAllAsync(TUserKey? userId = default)
    {
        return await GetQuery(userId)
            .ToListAsync();
    }

    /// <summary>
    /// Retrieves a page of entities visible to the specified user.
    /// </summary>
    /// <param name="pageNr">The one-based page number to retrieve.</param>
    /// <param name="pageSize">The number of items to include in the page.</param>
    /// <param name="userId">The optional user identifier used to scope the query.</param>
    /// <returns>
    /// A task that resolves to the matching entities for the requested page, or <see langword="null"/> when no result set is available.
    /// </returns>
    public async Task<IEnumerable<TResourceEntity>?> GetAllByPageAsync(int pageNr, int pageSize, TUserKey? userId = default)
    {
        ValidatePaging(pageNr, pageSize);

        return await GetQuery(userId)
            .Skip((pageNr - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    /// <summary>
    /// Counts the entities visible to the specified user.
    /// </summary>
    /// <param name="userId">The optional user identifier used to scope the query.</param>
    /// <returns>
    /// A task that resolves to the number of matching entities.
    /// </returns>
    public async Task<int> GetCountAsync(TUserKey? userId = default)
    {
        var query = GetQuery(userId);
        return await query.CountAsync();
    }

    /// <summary>
    /// Determines whether an entity with the specified identifier exists.
    /// </summary>
    /// <param name="id">The identifier of the entity to check.</param>
    /// <param name="userId">The optional user identifier used to scope the query.</param>
    /// <returns>
    /// A task that resolves to <see langword="true"/> when the entity exists; otherwise, <see langword="false"/>.
    /// </returns>
    public async Task<bool> ExistsAsync(TResourceKey id, TUserKey? userId = default)
    {
        var query = GetQuery(userId);
        return await query.AnyAsync(e => e.Id.Equals(id));
    }

    /// <summary>
    /// Retrieves an entity by its identifier.
    /// </summary>
    /// <param name="id">The identifier of the entity to retrieve.</param>
    /// <param name="userId">The optional user identifier used to scope the query.</param>
    /// <returns>
    /// A task that resolves to the matching entity, or <see langword="null"/> when it is not found.
    /// </returns>
    public async Task<TResourceEntity?> GetByIdAsync(TResourceKey id, TUserKey? userId = default)
    {
        var query = GetQuery(userId);
        var res = await query.FirstOrDefaultAsync(e => e.Id.Equals(id));
        return res;
    }

    /// <summary>
    /// Creates a new entity instance and applies ownership and metadata when supported.
    /// </summary>
    /// <param name="entity">The entity to create.</param>
    /// <param name="userId">The optional user identifier used to scope or stamp the operation.</param>
    /// <returns>
    /// A task that resolves to the created entity, or <see langword="null"/> when creation cannot be completed.
    /// </returns>
    public Task<TResourceEntity?> CreateAsync(TResourceEntity entity, TUserKey? userId = default)
    {
        if (ShouldUseUserId(userId))
        {
            ((IBaseEntityUserId<TUserKey>)entity).UserId = userId!;
        }

        ApplyCreateMetadata(entity, userId);
        return Task.FromResult<TResourceEntity?>(RepositoryDbSet.Add(entity).Entity);
    }

    /// <summary>
    /// Updates an existing entity instance and applies metadata when supported.
    /// </summary>
    /// <param name="id">The identifier of the entity to update.</param>
    /// <param name="entity">The new entity state.</param>
    /// <param name="userId">The optional user identifier used to scope or stamp the operation.</param>
    /// <returns>
    /// A task that resolves to the updated entity, or <see langword="null"/> when the entity cannot be updated.
    /// </returns>
    public async Task<TResourceEntity?> UpdateAsync(TResourceKey id, TResourceEntity entity, TUserKey? userId = default)
    {
        var dbEntity = await RepositoryDbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id.Equals(id));

        if (dbEntity == null)
        {
            return null;
        }

        if (ShouldUseUserId(userId))
        {
            if (!((IBaseEntityUserId<TUserKey>)dbEntity).UserId.Equals(userId))
            {
                return null;
            }

            ((IBaseEntityUserId<TUserKey>)entity).UserId = ((IBaseEntityUserId<TUserKey>)dbEntity).UserId;
        }

        ApplyUpdateMetadata(entity, dbEntity, userId);
        return RepositoryDbSet.Update(entity).Entity;
    }

    /// <summary>
    /// Removes an entity by its identifier.
    /// </summary>
    /// <param name="id">The identifier of the entity to remove.</param>
    /// <param name="userId">The optional user identifier used to scope the operation.</param>
    /// <returns>
    /// A task that resolves to <see langword="true"/> when the entity was removed; otherwise, <see langword="false"/>.
    /// </returns>
    public async Task<bool> RemoveAsync(TResourceKey id, TUserKey? userId = default)
    {
        var query = GetQuery(userId, asTracking: true);
        query = query.Where(e => e.Id.Equals(id));
        var dbEntity = await query.FirstOrDefaultAsync();

        if (dbEntity == null)
        {
            return false;
        }

        RepositoryDbSet.Remove(dbEntity);
        return true;
    }
}
