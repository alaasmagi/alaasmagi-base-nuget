using Base.Contracts.DataAccess;
using Base.Contracts.Domain;
using Base.Contracts.DTO;
using Microsoft.EntityFrameworkCore;

namespace Base.DataAccess.EF;

public class BaseRepositorySoftDelete<TDomainEntity, TDataAccessEntity, TMapper> :
    BaseRepositorySoftDelete<TDomainEntity, TDataAccessEntity, TMapper, Guid, Guid>
    where TDomainEntity : class, IBaseEntity<Guid>, IBaseEntitySoftDelete
    where TDataAccessEntity : class, IBaseEntity<Guid>, IBaseEntitySoftDelete
    where TMapper : class, IMapper<TDomainEntity, TDataAccessEntity, Guid>
{
    public BaseRepositorySoftDelete(DbContext repositoryDbContext, TMapper repositoryMapper) : base(repositoryDbContext, repositoryMapper)
    {
    }
}

/// <summary>
/// Provides a reusable Entity Framework repository implementation for entities that support soft delete.
/// </summary>
/// <typeparam name="TDomainEntity">The domain entity type exposed by the repository.</typeparam>
/// <typeparam name="TDataAccessEntity">The database entity type persisted by Entity Framework.</typeparam>
/// <typeparam name="TMapper">The mapper used to translate between domain and database entities.</typeparam>
/// <typeparam name="TResourceKey">The identifier type of the entity.</typeparam>
/// <typeparam name="TUserKey">The identifier type of the current user or owner.</typeparam>
public class BaseRepositorySoftDelete<TDomainEntity, TDataAccessEntity, TMapper, TResourceKey, TUserKey> :
    BaseRepository<TDomainEntity, TDataAccessEntity, TMapper, TResourceKey, TUserKey>,
    IBaseRepositorySoftDelete<TDomainEntity, TResourceKey, TUserKey>
    where TDomainEntity : class, IBaseEntity<TResourceKey>, IBaseEntitySoftDelete
    where TDataAccessEntity : class, IBaseEntity<TResourceKey>, IBaseEntitySoftDelete
    where TMapper : class, IMapper<TDomainEntity, TDataAccessEntity, TResourceKey>
    where TResourceKey : IEquatable<TResourceKey>
    where TUserKey : IEquatable<TUserKey>
{
    /// <summary>
    /// Stores the mapper used to convert between domain and database entities.
    /// </summary>
    protected new readonly TMapper RepositoryMapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseRepositorySoftDelete{TDomainEntity, TDataAccessEntity, TMapper, TResourceKey, TUserKey}"/> class.
    /// </summary>
    /// <param name="repositoryDbContext">The database context used by the repository.</param>
    /// <param name="repositoryMapper">The mapper used to translate between entity representations.</param>
    public BaseRepositorySoftDelete(DbContext repositoryDbContext, TMapper repositoryMapper)
        : base(repositoryDbContext, repositoryMapper)
    {
        RepositoryMapper = repositoryMapper;
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
    protected virtual IQueryable<TDataAccessEntity> GetQuery(bool includeSoftDeleted = false, TUserKey? userId = default!, bool asTracking = false)
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
    public async Task<IEnumerable<TDomainEntity>?> GetAllAsync(bool includeSoftDeleted = false, TUserKey? userId = default!)
    {
        var entities = await GetQuery(includeSoftDeleted, userId).ToListAsync();
        return RepositoryMapper.Map(entities);
    }

    /// <summary>
    /// Retrieves a page of entities while optionally including soft-deleted records.
    /// </summary>
    public async Task<IEnumerable<TDomainEntity>?> GetAllByPageAsync(int pageNr, int pageSize, bool includeSoftDeleted = false, TUserKey? userId = default!)
    {
        ValidatePaging(pageNr, pageSize);

        var entities = await GetQuery(includeSoftDeleted, userId)
            .Skip((pageNr - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return RepositoryMapper.Map(entities);
    }

    /// <summary>
    /// Counts entities while optionally including soft-deleted records.
    /// </summary>
    public async Task<int> GetCountAsync(bool includeSoftDeleted = false, TUserKey? userId = default!)
    {
        return await GetQuery(includeSoftDeleted, userId).CountAsync();
    }

    /// <summary>
    /// Retrieves an entity by its identifier while optionally including soft-deleted records.
    /// </summary>
    public async Task<TDomainEntity?> GetByIdAsync(TResourceKey id, bool includeSoftDeleted = false, TUserKey? userId = default!)
    {
        var entity = await GetQuery(includeSoftDeleted, userId)
            .FirstOrDefaultAsync(resourceEntity => resourceEntity.Id.Equals(id));

        return RepositoryMapper.Map(entity);
    }

    /// <summary>
    /// Determines whether an entity exists while optionally including soft-deleted records.
    /// </summary>
    public async Task<bool> ExistsAsync(TResourceKey id, bool includeSoftDeleted = false, TUserKey? userId = default!)
    {
        return await GetQuery(includeSoftDeleted, userId)
            .AnyAsync(resourceEntity => resourceEntity.Id.Equals(id));
    }

    /// <summary>
    /// Marks an entity as soft deleted.
    /// </summary>
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
    public async Task<TDomainEntity?> RestoreAsync(TResourceKey id, TUserKey? userId = default!)
    {
        var entity = await GetQuery(true, userId, asTracking: true)
            .FirstOrDefaultAsync(resourceEntity => resourceEntity.Id.Equals(id));

        if (entity == null || !entity.IsDeleted)
        {
            return null;
        }

        entity.IsDeleted = false;
        ApplyModificationMetadata(entity, userId);
        return RepositoryMapper.Map(entity);
    }
}
