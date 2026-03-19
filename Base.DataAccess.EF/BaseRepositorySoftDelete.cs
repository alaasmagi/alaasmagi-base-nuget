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
/// <typeparam name="TActor">The identifier type of the actor used to scope or stamp repository operations.</typeparam>
public class BaseRepositorySoftDelete<TDomainEntity, TDataAccessEntity, TMapper, TResourceKey, TActor> :
    BaseRepository<TDomainEntity, TDataAccessEntity, TMapper, TResourceKey, TActor>,
    IBaseRepositorySoftDelete<TDomainEntity, TResourceKey, TActor>
    where TDomainEntity : class, IBaseEntity<TResourceKey>, IBaseEntitySoftDelete
    where TDataAccessEntity : class, IBaseEntity<TResourceKey>, IBaseEntitySoftDelete
    where TMapper : class, IMapper<TDomainEntity, TDataAccessEntity, TResourceKey>
    where TResourceKey : IEquatable<TResourceKey>
    where TActor : IEquatable<TActor>
{
    /// <summary>
    /// Stores the mapper used to convert between domain and database entities.
    /// </summary>
    protected new readonly TMapper RepositoryMapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseRepositorySoftDelete{TDomainEntity, TDataAccessEntity, TMapper, TResourceKey, TActor}"/> class.
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
    /// <param name="actor">The optional actor used to scope the query.</param>
    /// <param name="asTracking">Controls whether Entity Framework change tracking is enabled for the query.</param>
    /// <returns>
    /// An <see cref="IQueryable{T}"/> representing the base query for the repository.
    /// </returns>
    protected virtual IQueryable<TDataAccessEntity> GetQuery(bool includeSoftDeleted = false, TActor? actor = default!, bool asTracking = false)
    {
        var query = base.GetQuery(actor, asTracking);

        if (!includeSoftDeleted)
        {
            query = query.Where(entity => !entity.IsDeleted);
        }

        return query;
    }

    /// <summary>
    /// Retrieves all entities visible to the specified actor while optionally including soft-deleted records.
    /// </summary>
    public async Task<IEnumerable<TDomainEntity>?> GetAllAsync(bool includeSoftDeleted = false, TActor? actor = default!)
    {
        var entities = await GetQuery(includeSoftDeleted, actor).ToListAsync();
        return RepositoryMapper.Map(entities);
    }

    /// <summary>
    /// Retrieves a single page of entities visible to the specified actor while optionally including soft-deleted records.
    /// </summary>
    public async Task<IEnumerable<TDomainEntity>?> GetAllByPageAsync(int pageNr, int pageSize, bool includeSoftDeleted = false, TActor? actor = default!)
    {
        ValidatePaging(pageNr, pageSize);

        var entities = await GetQuery(includeSoftDeleted, actor)
            .Skip((pageNr - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return RepositoryMapper.Map(entities);
    }

    /// <summary>
    /// Counts all entities visible to the specified actor while optionally including soft-deleted records.
    /// </summary>
    public async Task<int> GetCountAsync(bool includeSoftDeleted = false, TActor? actor = default!)
    {
        return await GetQuery(includeSoftDeleted, actor).CountAsync();
    }

    /// <summary>
    /// Retrieves an entity by its identifier while optionally including soft-deleted records visible to the specified actor.
    /// </summary>
    public async Task<TDomainEntity?> GetByIdAsync(TResourceKey id, bool includeSoftDeleted = false, TActor? actor = default!)
    {
        var entity = await GetQuery(includeSoftDeleted, actor)
            .FirstOrDefaultAsync(resourceEntity => resourceEntity.Id.Equals(id));

        return RepositoryMapper.Map(entity);
    }

    /// <summary>
    /// Determines whether an entity with the specified identifier exists while optionally including soft-deleted records visible to the specified actor.
    /// </summary>
    public async Task<bool> ExistsAsync(TResourceKey id, bool includeSoftDeleted = false, TActor? actor = default!)
    {
        return await GetQuery(includeSoftDeleted, actor)
            .AnyAsync(resourceEntity => resourceEntity.Id.Equals(id));
    }

    /// <summary>
    /// Marks an entity as soft deleted.
    /// </summary>
    public async Task<bool> SoftDeleteAsync(TResourceKey id, TActor? actor = default!)
    {
        var entity = await GetQuery(true, actor, asTracking: true)
            .FirstOrDefaultAsync(resourceEntity => resourceEntity.Id.Equals(id));

        if (entity == null || entity.IsDeleted)
        {
            return false;
        }

        entity.IsDeleted = true;
        ApplyModificationMetadata(entity, actor);
        return true;
    }

    /// <summary>
    /// Restores a previously soft-deleted entity.
    /// </summary>
    public async Task<TDomainEntity?> RestoreAsync(TResourceKey id, TActor? actor = default!)
    {
        var entity = await GetQuery(true, actor, asTracking: true)
            .FirstOrDefaultAsync(resourceEntity => resourceEntity.Id.Equals(id));

        if (entity == null || !entity.IsDeleted)
        {
            return null;
        }

        entity.IsDeleted = false;
        ApplyModificationMetadata(entity, actor);
        return RepositoryMapper.Map(entity);
    }
}
