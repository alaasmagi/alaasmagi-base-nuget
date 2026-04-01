using Base.Contracts.DataAccess;
using Base.Contracts.Domain;
using Base.Contracts.DTO;
using Base.DTO;
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
    /// Gets the default error code used when a soft-delete operation fails.
    /// </summary>
    protected virtual string SoftDeleteFailureErrorCode => ErrorDefaults.Codes.SoftDeleteFailed;

    /// <summary>
    /// Gets the default error message used when a soft-delete operation fails.
    /// </summary>
    protected virtual string SoftDeleteFailureErrorMessage => ErrorDefaults.Messages.SoftDeleteFailed;

    /// <summary>
    /// Gets the default error code used when a restore operation fails.
    /// </summary>
    protected virtual string RestoreFailureErrorCode => ErrorDefaults.Codes.RestoreFailed;

    /// <summary>
    /// Gets the default error message used when a restore operation fails.
    /// </summary>
    protected virtual string RestoreFailureErrorMessage => ErrorDefaults.Messages.RestoreFailed;
    
    
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
    /// Builds the default query for inherited CRUD methods, excluding soft-deleted records.
    /// </summary>
    protected override IQueryable<TDataAccessEntity> GetQuery(TActor? actor = default!, bool asTracking = false)
    {
        return GetQuery(false, actor, asTracking);
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
    public virtual async Task<IMethodResponse<IEnumerable<TDomainEntity>>> GetAllAsync(bool includeSoftDeleted = false, TActor? actor = default!)
    {
        var entities = await GetQuery(includeSoftDeleted, actor).ToListAsync();
        var mappedEntities = RepositoryMapper.Map(entities);

        if (mappedEntities == null)
        {
            return MethodResponse<IEnumerable<TDomainEntity>>.Failure(CreateError(MappingFailureErrorCode, MappingFailureErrorMessage));
        }

        return MethodResponse<IEnumerable<TDomainEntity>>.Success(mappedEntities);
    }

    /// <summary>
    /// Retrieves a single page of entities visible to the specified actor while optionally including soft-deleted records.
    /// </summary>
    public virtual async Task<IMethodResponse<IEnumerable<TDomainEntity>>> GetAllByPageAsync(int pageNr, int pageSize, bool includeSoftDeleted = false, TActor? actor = default!)
    {
        var pagingError = ValidatePaging(pageNr, pageSize);

        if (pagingError != null)
        {
            return MethodResponse<IEnumerable<TDomainEntity>>.Failure(pagingError);
        }

        var entities = await GetQuery(includeSoftDeleted, actor)
            .Skip((pageNr - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var mappedEntities = RepositoryMapper.Map(entities);

        if (mappedEntities == null)
        {
            return MethodResponse<IEnumerable<TDomainEntity>>.Failure(CreateError(MappingFailureErrorCode, MappingFailureErrorMessage));
        }

        return MethodResponse<IEnumerable<TDomainEntity>>.Success(mappedEntities);
    }

    /// <summary>
    /// Counts all entities visible to the specified actor while optionally including soft-deleted records.
    /// </summary>
    public virtual async Task<IMethodResponse<int>> GetCountAsync(bool includeSoftDeleted = false, TActor? actor = default!)
    {
        var count = await GetQuery(includeSoftDeleted, actor).CountAsync();
        return MethodResponse<int>.Success(count);
    }

    /// <summary>
    /// Retrieves an entity by its identifier while optionally including soft-deleted records visible to the specified actor.
    /// </summary>
    public virtual async Task<IMethodResponse<TDomainEntity>> GetByIdAsync(TResourceKey id, bool includeSoftDeleted = false, TActor? actor = default!)
    {
        var entity = await GetQuery(includeSoftDeleted, actor)
            .FirstOrDefaultAsync(resourceEntity => resourceEntity.Id.Equals(id));

        if (entity == null)
        {
            return MethodResponse<TDomainEntity>.Failure(CreateError(NotFoundErrorCode, NotFoundErrorMessage));
        }

        var mappedEntity = RepositoryMapper.Map(entity);

        if (mappedEntity == null)
        {
            return MethodResponse<TDomainEntity>.Failure(CreateError(MappingFailureErrorCode, MappingFailureErrorMessage));
        }

        return MethodResponse<TDomainEntity>.Success(mappedEntity);
    }

    /// <summary>
    /// Determines whether an entity with the specified identifier exists while optionally including soft-deleted records visible to the specified actor.
    /// </summary>
    public virtual async Task<IMethodResponse<bool>> ExistsAsync(TResourceKey id, bool includeSoftDeleted = false, TActor? actor = default!)
    {
        var exists = await GetQuery(includeSoftDeleted, actor)
            .AnyAsync(resourceEntity => resourceEntity.Id.Equals(id));

        return MethodResponse<bool>.Success(exists);
    }

    /// <summary>
    /// Marks an entity as soft deleted after validating the supplied concurrency token when supported.
    /// </summary>
    public virtual async Task<IMethodResponse<bool>> SoftDeleteAsync(TResourceKey id, string? expectedConcurrencyToken = default, TActor? actor = default!)
    {
        var entity = await GetQuery(true, actor, asTracking: true)
            .FirstOrDefaultAsync(resourceEntity => resourceEntity.Id.Equals(id));

        if (entity == null || entity.IsDeleted)
        {
            return MethodResponse<bool>.Failure(CreateError(SoftDeleteFailureErrorCode, SoftDeleteFailureErrorMessage));
        }

        var concurrencyError = ValidateConcurrencyToken(entity, expectedConcurrencyToken);

        if (concurrencyError != null)
        {
            return MethodResponse<bool>.Failure(concurrencyError);
        }

        entity.IsDeleted = true;
        ApplyModificationMetadata(entity, actor);
        ApplyNewConcurrencyToken(entity);
        return MethodResponse<bool>.Success(true);
    }

    /// <summary>
    /// Restores a previously soft-deleted entity after validating the supplied concurrency token when supported.
    /// </summary>
    public virtual async Task<IMethodResponse<TDomainEntity>> RestoreAsync(TResourceKey id, string? expectedConcurrencyToken = default, TActor? actor = default!)
    {
        var entity = await GetQuery(true, actor, asTracking: true)
            .FirstOrDefaultAsync(resourceEntity => resourceEntity.Id.Equals(id));

        if (entity == null || !entity.IsDeleted)
        {
            return MethodResponse<TDomainEntity>.Failure(CreateError(RestoreFailureErrorCode, RestoreFailureErrorMessage));
        }

        var concurrencyError = ValidateConcurrencyToken(entity, expectedConcurrencyToken);

        if (concurrencyError != null)
        {
            return MethodResponse<TDomainEntity>.Failure(concurrencyError);
        }

        entity.IsDeleted = false;
        ApplyModificationMetadata(entity, actor);
        ApplyNewConcurrencyToken(entity);
        var mappedEntity = RepositoryMapper.Map(entity);

        if (mappedEntity == null)
        {
            return MethodResponse<TDomainEntity>.Failure(CreateError(MappingFailureErrorCode, MappingFailureErrorMessage));
        }

        return MethodResponse<TDomainEntity>.Success(mappedEntity);
    }
}
