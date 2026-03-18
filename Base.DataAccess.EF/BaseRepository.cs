using Base.Contracts.DataAccess;
using Base.Contracts.Domain;
using Base.Contracts.DTO;
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
/// <typeparam name="TUserKey">The identifier type of the current user or owner.</typeparam>
public class BaseRepository<TDomainEntity, TDataAccessEntity, TMapper, TResourceKey, TUserKey> : IBaseRepository<TDomainEntity, TResourceKey, TUserKey>
    where TDomainEntity : class, IBaseEntity<TResourceKey>
    where TDataAccessEntity : class, IBaseEntity<TResourceKey>
    where TMapper : class, IMapper<TDomainEntity, TDataAccessEntity, TResourceKey>
    where TResourceKey : IEquatable<TResourceKey>
    where TUserKey : IEquatable<TUserKey>
{
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
    /// Initializes a new instance of the <see cref="BaseRepository{TDomainEntity, TDataAccessEntity, TMapper, TResourceKey, TUserKey}"/> class.
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
    /// <param name="userId">The optional user identifier used to scope the query.</param>
    /// <param name="asTracking">Controls whether Entity Framework change tracking is enabled for the query.</param>
    /// <returns>
    /// An <see cref="IQueryable{T}"/> representing the base query for the repository.
    /// </returns>
    protected virtual IQueryable<TDataAccessEntity> GetQuery(TUserKey? userId = default!, bool asTracking = false)
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
        return typeof(IBaseEntityUserId<TUserKey>).IsAssignableFrom(typeof(TDataAccessEntity)) &&
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
        return typeof(IBaseEntityMeta).IsAssignableFrom(typeof(TDataAccessEntity));
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
    protected virtual void ApplyCreateMetadata(TDataAccessEntity entity, TUserKey? userId = default!)
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
    protected virtual void ApplyUpdateMetadata(TDataAccessEntity entity, TDataAccessEntity existingEntity, TUserKey? userId = default!)
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
    protected void ApplyModificationMetadata(TDataAccessEntity entity, TUserKey? userId = default!)
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
    public async Task<IEnumerable<TDomainEntity>?> GetAllAsync(TUserKey? userId = default)
    {
        var entities = await GetQuery(userId).ToListAsync();
        return RepositoryMapper.Map(entities);
    }

    /// <summary>
    /// Retrieves a page of entities visible to the specified user.
    /// </summary>
    public async Task<IEnumerable<TDomainEntity>?> GetAllByPageAsync(int pageNr, int pageSize, TUserKey? userId = default)
    {
        ValidatePaging(pageNr, pageSize);

        var entities = await GetQuery(userId)
            .Skip((pageNr - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return RepositoryMapper.Map(entities);
    }

    /// <summary>
    /// Counts the entities visible to the specified user.
    /// </summary>
    public async Task<int> GetCountAsync(TUserKey? userId = default)
    {
        return await GetQuery(userId).CountAsync();
    }

    /// <summary>
    /// Determines whether an entity with the specified identifier exists.
    /// </summary>
    public async Task<bool> ExistsAsync(TResourceKey id, TUserKey? userId = default)
    {
        return await GetQuery(userId).AnyAsync(e => e.Id.Equals(id));
    }

    /// <summary>
    /// Retrieves an entity by its identifier.
    /// </summary>
    public async Task<TDomainEntity?> GetByIdAsync(TResourceKey id, TUserKey? userId = default)
    {
        var entity = await GetQuery(userId).FirstOrDefaultAsync(e => e.Id.Equals(id));
        return RepositoryMapper.Map(entity);
    }

    /// <summary>
    /// Creates a new entity instance and applies ownership and metadata when supported.
    /// </summary>
    public Task<TDomainEntity?> CreateAsync(TDomainEntity entity, TUserKey? userId = default)
    {
        var dbEntity = RepositoryMapper.Map(entity);

        if (dbEntity == null)
        {
            return Task.FromResult<TDomainEntity?>(null);
        }

        if (ShouldUseUserId(userId))
        {
            ((IBaseEntityUserId<TUserKey>)dbEntity).UserId = userId!;
        }

        ApplyCreateMetadata(dbEntity, userId);
        var createdEntity = RepositoryDbSet.Add(dbEntity).Entity;
        return Task.FromResult(RepositoryMapper.Map(createdEntity));
    }

    /// <summary>
    /// Updates an existing entity instance and applies metadata when supported.
    /// </summary>
    public async Task<TDomainEntity?> UpdateAsync(TResourceKey id, TDomainEntity entity, TUserKey? userId = default)
    {
        var dbEntity = RepositoryMapper.Map(entity);

        if (dbEntity == null)
        {
            return null;
        }

        var existingDbEntity = await RepositoryDbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id.Equals(id));

        if (existingDbEntity == null)
        {
            return null;
        }

        dbEntity.Id = id;

        if (ShouldUseUserId(userId))
        {
            if (!((IBaseEntityUserId<TUserKey>)existingDbEntity).UserId.Equals(userId))
            {
                return null;
            }

            ((IBaseEntityUserId<TUserKey>)dbEntity).UserId = ((IBaseEntityUserId<TUserKey>)existingDbEntity).UserId;
        }

        ApplyUpdateMetadata(dbEntity, existingDbEntity, userId);
        var updatedEntity = RepositoryDbSet.Update(dbEntity).Entity;
        return RepositoryMapper.Map(updatedEntity);
    }

    /// <summary>
    /// Removes an entity by its identifier.
    /// </summary>
    public async Task<bool> RemoveAsync(TResourceKey id, TUserKey? userId = default)
    {
        var query = GetQuery(userId, asTracking: true).Where(e => e.Id.Equals(id));
        var dbEntity = await query.FirstOrDefaultAsync();

        if (dbEntity == null)
        {
            return false;
        }

        RepositoryDbSet.Remove(dbEntity);
        return true;
    }
}
