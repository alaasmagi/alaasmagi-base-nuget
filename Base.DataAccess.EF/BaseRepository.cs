using Base.Contracts.DataAccess;
using Base.Contracts.Domain;
using Base.Contracts.DTO;
using Base.DTO;
using Base.Exception;
using Microsoft.EntityFrameworkCore;

namespace Base.DataAccess.EF;

public class BaseRepository<TDomainEntity, TDataAccessEntity, TMapper> : BaseRepository<TDomainEntity, TDataAccessEntity, TMapper, Guid, Guid>
    where TDomainEntity : class, IBaseEntity<Guid>
    where TDataAccessEntity : class, IBaseEntity<Guid>
    where TMapper : class, IMapper<TDomainEntity, TDataAccessEntity, Guid>
{
    public BaseRepository(DbContext repositoryDbContext, TMapper repositoryMapper)
        : base(repositoryDbContext, repositoryMapper)
    {
    }
}

/// <summary>
/// Provides a reusable Entity Framework repository implementation that maps between domain and database entities.
/// </summary>
/// <typeparam name="TDomainEntity">The domain entity type exposed by the repository.</typeparam>
/// <typeparam name="TDataAccessEntity">The database entity type persisted by Entity Framework.</typeparam>
/// <typeparam name="TMapper">The mapper used to translate between domain and database entities.</typeparam>
/// <typeparam name="TResourceKey">The identifier type of the entity.</typeparam>
/// <typeparam name="TActor">The identifier type of the actor used to scope or stamp repository operations.</typeparam>
public class BaseRepository<TDomainEntity, TDataAccessEntity, TMapper, TResourceKey, TActor> : IBaseRepository<TDomainEntity, TResourceKey, TActor>
    where TDomainEntity : class, IBaseEntity<TResourceKey>
    where TDataAccessEntity : class, IBaseEntity<TResourceKey>
    where TMapper : class, IMapper<TDomainEntity, TDataAccessEntity, TResourceKey>
    where TResourceKey : IEquatable<TResourceKey>
    where TActor : IEquatable<TActor>
{
    /// <summary>
    /// Gets the default error code used when a requested entity or result cannot be found.
    /// </summary>
    protected virtual string NotFoundErrorCode => ErrorDefaults.Codes.NotFound;

    /// <summary>
    /// Gets the default error message used when a requested entity or result cannot be found.
    /// </summary>
    protected virtual string NotFoundErrorMessage => ErrorDefaults.Messages.NotFound;
    
    /// <summary>
    /// Gets the default error code used when entity mapping fails.
    /// </summary>
    protected virtual string MappingFailureErrorCode => ErrorDefaults.Codes.MapFailed;

    /// <summary>
    /// Gets the default error message used when entity mapping fails.
    /// </summary>
    protected virtual string MappingFailureErrorMessage => ErrorDefaults.Messages.MapToDomainModelFailed;
    
    /// <summary>
    /// Gets the default error code used when an operation is forbidden.
    /// </summary>
    protected virtual string ForbiddenErrorCode => ErrorDefaults.Codes.Forbidden;

    /// <summary>
    /// Gets the default error message used when an operation is forbidden.
    /// </summary>
    protected virtual string ForbiddenErrorMessage => ErrorDefaults.Messages.Forbidden;
    
    /// <summary>
    /// Gets the default error code used when an invalid paging parameter is provided.
    /// </summary>
    protected virtual string InvalidPagingErrorCode => ErrorDefaults.Codes.InvalidPaging;

    /// <summary>
    /// Gets the default error message used when  an invalid paging parameter is provided.
    /// </summary>
    protected virtual string InvalidPagingErrorMessage => ErrorDefaults.Messages.InvalidPaging;

    
    /// <summary>
    /// Stores the database context used by the repository.
    /// </summary>
    protected readonly DbContext RepositoryDbContext;

    /// <summary>
    /// Stores the mapper used to convert between domain and database entities.
    /// </summary>
    protected readonly TMapper RepositoryMapper;

    /// <summary>
    /// Stores the entity set used for CRUD operations.
    /// </summary>
    protected readonly DbSet<TDataAccessEntity> RepositoryDbSet;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseRepository{TDomainEntity, TDataAccessEntity, TMapper, TResourceKey, TActor}"/> class.
    /// </summary>
    /// <param name="repositoryDbContext">The database context used by the repository.</param>
    /// <param name="repositoryMapper">The mapper used to translate between entity representations.</param>
    public BaseRepository(DbContext repositoryDbContext, TMapper repositoryMapper)
    {
        RepositoryDbContext = repositoryDbContext;
        RepositoryMapper = repositoryMapper;
        RepositoryDbSet = RepositoryDbContext.Set<TDataAccessEntity>();
    }

    /// <summary>
    /// Builds the base query for the repository while optionally filtering by user ownership and tracking mode.
    /// </summary>
    /// <param name="actor">The optional actor used to scope the query.</param>
    /// <param name="asTracking">Controls whether Entity Framework change tracking is enabled for the query.</param>
    /// <returns>
    /// An <see cref="IQueryable{T}"/> representing the base query for the repository.
    /// </returns>
    protected virtual IQueryable<TDataAccessEntity> GetQuery(TActor? actor = default!, bool asTracking = false)
    {
        var query = RepositoryDbSet.AsQueryable();

        if (!asTracking)
        {
            query = query.AsNoTracking();
        }

        if (ShouldUseUserId(actor))
        {
            query = query.Where(e => ((IBaseEntityUserId<TActor>)e).UserId.Equals(actor));
        }

        return query;
    }

    /// <summary>
    /// Creates a standard error payload for repository-level operation failures.
    /// </summary>
    protected virtual IError CreateError(string code, string message) => new Error(code, message);

    /// <summary>
    /// Validates paging arguments and throws when the values are out of range.
    /// </summary>
    /// <param name="pageNr">The one-based page number to validate.</param>
    /// <param name="pageSize">The number of items per page to validate.</param>
    protected virtual void ValidatePaging(int pageNr, int pageSize)
    {
        if (pageNr <= 0)
        {
            throw new BaseException(InvalidPagingErrorCode, InvalidPagingErrorMessage);
        }

        if (pageSize <= 0)
        {
            throw new BaseException(InvalidPagingErrorCode, InvalidPagingErrorMessage);
        }
    }

    /// <summary>
    /// Determines whether the entity type supports user scoping and a non-default user identifier was provided.
    /// </summary>
    /// <param name="actor">The optional actor value used to determine whether user-based scoping should be applied.</param>
    /// <returns>
    /// <see langword="true"/> when user scoping should be applied; otherwise, <see langword="false"/>.
    /// </returns>
    private bool ShouldUseUserId(TActor? actor = default!)
    {
        return typeof(IBaseEntityUserId<TActor>).IsAssignableFrom(typeof(TDataAccessEntity)) &&
               actor != null &&
               !EqualityComparer<TActor>.Default.Equals(actor, default);
    }

    /// <summary>
    /// Determines whether the entity type supports metadata fields.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> when the entity type implements <see cref="IBaseEntityMeta"/>; otherwise, <see langword="false"/>.
    /// </returns>
    private static bool HasMeta()
    {
        return typeof(IBaseEntityMeta).IsAssignableFrom(typeof(TDataAccessEntity));
    }

    /// <summary>
    /// Converts an actor value to its string representation for metadata fields.
    /// </summary>
    /// <param name="actor">The optional actor value to convert.</param>
    /// <returns>
    /// The string representation of the actor value, or <see langword="null"/> when the value is not set.
    /// </returns>
    private static string? GetActorIdentifier(TActor? actor = default!)
    {
        if (actor == null || EqualityComparer<TActor>.Default.Equals(actor, default!))
        {
            return string.Empty;
        }

        return actor.ToString();
    }

    /// <summary>
    /// Applies creation metadata to an entity when the entity type supports metadata fields.
    /// </summary>
    protected virtual void ApplyCreateMetadata(TDataAccessEntity entity, TActor? actor = default!)
    {
        if (!HasMeta())
        {
            return;
        }

        var now = DateTime.UtcNow;
        var actorIdentifier = GetActorIdentifier(actor);
        var metaEntity = (IBaseEntityMeta)entity;

        metaEntity.CreatedAt = now;
        metaEntity.UpdatedAt = now;

        if (actorIdentifier != null)
        {
            metaEntity.CreatedBy = actorIdentifier;
            metaEntity.UpdatedBy = actorIdentifier;
        }
    }

    /// <summary>
    /// Applies update metadata to an entity while preserving the original creation metadata.
    /// </summary>
    protected virtual void ApplyUpdateMetadata(TDataAccessEntity entity, TDataAccessEntity existingEntity, TActor? actor = default!)
    {
        if (!HasMeta())
        {
            return;
        }

        var now = DateTime.UtcNow;
        var actorIdentifier = GetActorIdentifier(actor);
        var metaEntity = (IBaseEntityMeta)entity;
        var existingMetaEntity = (IBaseEntityMeta)existingEntity;

        metaEntity.CreatedAt = existingMetaEntity.CreatedAt;
        metaEntity.CreatedBy = existingMetaEntity.CreatedBy;
        metaEntity.UpdatedAt = now;

        if (actorIdentifier != string.Empty && actorIdentifier != null)
        {
            metaEntity.UpdatedBy = actorIdentifier;
        }
        else
        {
            metaEntity.UpdatedBy = existingMetaEntity.UpdatedBy;
        }
    }

    /// <summary>
    /// Applies modification metadata to an entity when the entity type supports metadata fields.
    /// </summary>
    protected virtual void ApplyModificationMetadata(TDataAccessEntity entity, TActor? actor = default!)
    {
        if (!HasMeta())
        {
            return;
        }

        var actorIdentifier = GetActorIdentifier(actor);
        var metaEntity = (IBaseEntityMeta)entity;
        metaEntity.UpdatedAt = DateTime.UtcNow;

        if (!string.IsNullOrWhiteSpace(actorIdentifier))
        {
            metaEntity.UpdatedBy = actorIdentifier;
        }
    }

    /// <summary>
    /// Retrieves all entities visible to the specified actor.
    /// </summary>
    public virtual async Task<IMethodResponse<IEnumerable<TDomainEntity>>> GetAllAsync(TActor? actor = default)
    {
        var entities = await GetQuery(actor).ToListAsync();
        var mappedEntities = RepositoryMapper.Map(entities);

        if (mappedEntities == null)
        {
            return MethodResponse<IEnumerable<TDomainEntity>>.Failure(CreateError(MappingFailureErrorCode, MappingFailureErrorMessage));
        }

        return MethodResponse<IEnumerable<TDomainEntity>>.Success(mappedEntities);
    }

    /// <summary>
    /// Retrieves a single page of entities visible to the specified actor.
    /// </summary>
    public virtual async Task<IMethodResponse<IEnumerable<TDomainEntity>>> GetAllByPageAsync(int pageNr, int pageSize, TActor? actor = default)
    {
        ValidatePaging(pageNr, pageSize);

        var entities = await GetQuery(actor)
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
    /// Counts all entities visible to the specified actor.
    /// </summary>
    public virtual async Task<IMethodResponse<int>> GetCountAsync(TActor? actor = default)
    {
        var count = await GetQuery(actor).CountAsync();
        return MethodResponse<int>.Success(count);
    }

    /// <summary>
    /// Determines whether an entity with the specified identifier exists.
    /// </summary>
    public virtual async Task<IMethodResponse<bool>> ExistsAsync(TResourceKey id, TActor? actor = default)
    {
        var exists = await GetQuery(actor).AnyAsync(e => e.Id.Equals(id));
        return MethodResponse<bool>.Success(exists);
    }

    /// <summary>
    /// Retrieves an entity by its identifier.
    /// </summary>
    public virtual async Task<IMethodResponse<TDomainEntity>> GetByIdAsync(TResourceKey id, TActor? actor = default)
    {
        var entity = await GetQuery(actor).FirstOrDefaultAsync(e => e.Id.Equals(id));

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
    /// Creates a new entity instance and applies ownership and metadata when supported.
    /// </summary>
    public virtual Task<IMethodResponse<TDomainEntity>> CreateAsync(TDomainEntity entity, TActor? actor = default)
    {
        var dbEntity = RepositoryMapper.Map(entity);

        if (dbEntity == null)
        {
            return Task.FromResult<IMethodResponse<TDomainEntity>>(MethodResponse<TDomainEntity>.Failure(CreateError(MappingFailureErrorCode, MappingFailureErrorMessage)));
        }

        if (ShouldUseUserId(actor))
        {
            ((IBaseEntityUserId<TActor>)dbEntity).UserId = actor!;
        }

        ApplyCreateMetadata(dbEntity, actor);
        var createdEntity = RepositoryDbSet.Add(dbEntity).Entity;
        var mappedEntity = RepositoryMapper.Map(createdEntity);

        if (mappedEntity == null)
        {
            return Task.FromResult<IMethodResponse<TDomainEntity>>(MethodResponse<TDomainEntity>.Failure(CreateError(MappingFailureErrorCode, MappingFailureErrorMessage)));
        }

        return Task.FromResult<IMethodResponse<TDomainEntity>>(MethodResponse<TDomainEntity>.Success(mappedEntity));
    }

    /// <summary>
    /// Updates an existing entity instance and applies metadata when supported.
    /// </summary>
    public virtual async Task<IMethodResponse<TDomainEntity>> UpdateAsync(TResourceKey id, TDomainEntity entity, TActor? actor = default)
    {
        var dbEntity = RepositoryMapper.Map(entity);

        if (dbEntity == null)
        {
            return MethodResponse<TDomainEntity>.Failure(CreateError(MappingFailureErrorCode, MappingFailureErrorMessage));
        }

        var existingDbEntity = await RepositoryDbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id.Equals(id));

        if (existingDbEntity == null)
        {
            return MethodResponse<TDomainEntity>.Failure(CreateError(NotFoundErrorCode, NotFoundErrorMessage));
        }

        dbEntity.Id = id;

        if (ShouldUseUserId(actor))
        {
            if (!((IBaseEntityUserId<TActor>)existingDbEntity).UserId.Equals(actor))
            {
                return MethodResponse<TDomainEntity>.Failure(CreateError(ForbiddenErrorCode, ForbiddenErrorMessage));
            }

            ((IBaseEntityUserId<TActor>)dbEntity).UserId = ((IBaseEntityUserId<TActor>)existingDbEntity).UserId;
        }

        ApplyUpdateMetadata(dbEntity, existingDbEntity, actor);
        var updatedEntity = RepositoryDbSet.Update(dbEntity).Entity;
        var mappedEntity = RepositoryMapper.Map(updatedEntity);

        if (mappedEntity == null)
        {
            return MethodResponse<TDomainEntity>.Failure(CreateError(MappingFailureErrorCode, MappingFailureErrorMessage));
        }

        return MethodResponse<TDomainEntity>.Success(mappedEntity);
    }

    /// <summary>
    /// Removes an entity by its identifier.
    /// </summary>
    public virtual async Task<IMethodResponse<bool>> RemoveAsync(TResourceKey id, TActor? actor = default)
    {
        var query = GetQuery(actor, asTracking: true).Where(e => e.Id.Equals(id));
        var dbEntity = await query.FirstOrDefaultAsync();

        if (dbEntity == null)
        {
            return MethodResponse<bool>.Failure(CreateError(NotFoundErrorCode, NotFoundErrorMessage));
        }

        RepositoryDbSet.Remove(dbEntity);
        return MethodResponse<bool>.Success(true);
    }
}
