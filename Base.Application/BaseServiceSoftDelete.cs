using Base.Contracts.Application;
using Base.Contracts.DataAccess;
using Base.Contracts.Domain;
using Base.Contracts.DTO;

namespace Base.Application;

public class BaseServiceSoftDelete<TEntity, TDomainEntity, TRepository> 
    : BaseServiceSoftDelete<TEntity, TDomainEntity, TRepository, Guid, Guid>
        where TEntity : class
        where TDomainEntity : class, IBaseEntity<Guid>, IBaseEntitySoftDelete
        where TRepository : class, IBaseRepositorySoftDelete<TDomainEntity, Guid, Guid>
{
    public BaseServiceSoftDelete(IBaseUow serviceUow, TRepository serviceRepository, IMapper<TEntity, TDomainEntity, Guid> serviceMapper) 
        : base(serviceUow, serviceRepository, serviceMapper)
    {
    }
}

/// <summary>
/// Provides a reusable base implementation for application services that support soft-delete operations.
/// </summary>
/// <typeparam name="TEntity">The application-layer entity type exposed by the service.</typeparam>
/// <typeparam name="TDomainEntity">The repository/domain entity type used for persistence.</typeparam>
/// <typeparam name="TRepository">The repository type used to access persisted entities.</typeparam>
/// <typeparam name="TKey">The identifier type of the entity.</typeparam>
/// <typeparam name="TActor">The identifier type of the actor used to scope or stamp service operations.</typeparam>
public class BaseServiceSoftDelete<TEntity, TDomainEntity, TRepository, TKey, TActor> :
    BaseService<TEntity, TDomainEntity, TRepository, TKey, TActor>,
    IBaseServiceSoftDelete<TEntity, TKey, TActor>
    where TEntity : class
    where TDomainEntity : class, IBaseEntity<TKey>, IBaseEntitySoftDelete
    where TRepository : class, IBaseRepositorySoftDelete<TDomainEntity, TKey, TActor>
    where TKey : IEquatable<TKey>
    where TActor : IEquatable<TActor>
{
    /// <summary>
    /// Stores the unit of work used by soft-delete operations to persist changes.
    /// </summary>
    protected readonly IBaseUow SoftDeleteServiceUow;

    /// <summary>
    /// Stores the soft-delete-aware repository used for entity persistence operations.
    /// </summary>
    protected readonly TRepository SoftDeleteServiceRepository;

    /// <summary>
    /// Stores the mapper used to translate between application-layer and repository entities.
    /// </summary>
    protected readonly IMapper<TEntity, TDomainEntity, TKey> SoftDeleteServiceMapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseServiceSoftDelete{TEntity, TDomainEntity, TRepository, TKey, TActor}"/> class.
    /// </summary>
    /// <param name="serviceUow">The unit of work used to persist changes.</param>
    /// <param name="serviceRepository">The soft-delete-aware repository used to access entities.</param>
    /// <param name="serviceMapper">The mapper used to translate between application-layer and repository entities.</param>
    public BaseServiceSoftDelete(
        IBaseUow serviceUow,
        TRepository serviceRepository,
        IMapper<TEntity, TDomainEntity, TKey> serviceMapper)
        : base(serviceUow, serviceRepository, serviceMapper)
    {
        SoftDeleteServiceUow = serviceUow;
        SoftDeleteServiceRepository = serviceRepository;
        SoftDeleteServiceMapper = serviceMapper;
    }

    /// <summary>
    /// Retrieves all entities visible to the specified actor while optionally including soft-deleted records.
    /// </summary>
    public async Task<IEnumerable<TEntity>?> GetAllAsync(bool includeSoftDeleted = false, TActor? actor = default)
    {
        var domainEntities = await SoftDeleteServiceRepository.GetAllAsync(includeSoftDeleted, actor);
        return SoftDeleteServiceMapper.Map(domainEntities);
    }

    /// <summary>
    /// Retrieves a single page of entities visible to the specified actor while optionally including soft-deleted records.
    /// </summary>
    public async Task<IEnumerable<TEntity>?> GetAllByPageAsync(int pageNr, int pageSize, bool includeSoftDeleted = false, TActor? actor = default)
    {
        var domainEntities = await SoftDeleteServiceRepository.GetAllByPageAsync(pageNr, pageSize, includeSoftDeleted, actor);
        return SoftDeleteServiceMapper.Map(domainEntities);
    }

    /// <summary>
    /// Counts all entities visible to the specified actor while optionally including soft-deleted records.
    /// </summary>
    public async Task<int> GetCountAsync(bool includeSoftDeleted = false, TActor? actor = default)
    {
        return await SoftDeleteServiceRepository.GetCountAsync(includeSoftDeleted, actor);
    }

    /// <summary>
    /// Retrieves an entity by its identifier while optionally including soft-deleted records visible to the specified actor.
    /// </summary>
    public async Task<TEntity?> GetByIdAsync(TKey id, bool includeSoftDeleted = false, TActor? actor = default)
    {
        var domainEntity = await SoftDeleteServiceRepository.GetByIdAsync(id, includeSoftDeleted, actor);
        return SoftDeleteServiceMapper.Map(domainEntity);
    }

    /// <summary>
    /// Determines whether an entity with the specified identifier exists while optionally including soft-deleted records visible to the specified actor.
    /// </summary>
    public async Task<bool> ExistsAsync(TKey id, bool includeSoftDeleted = false, TActor? actor = default)
    {
        return await SoftDeleteServiceRepository.ExistsAsync(id, includeSoftDeleted, actor);
    }

    /// <summary>
    /// Marks an entity as soft deleted.
    /// </summary>
    public async Task<bool> SoftDeleteAsync(TKey id, TActor? actor = default)
    {
        var deleted = await SoftDeleteServiceRepository.SoftDeleteAsync(id, actor);

        if (!deleted)
        {
            return false;
        }

        await SoftDeleteServiceUow.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Restores a previously soft-deleted entity.
    /// </summary>
    public async Task<TEntity?> RestoreAsync(TKey id, TActor? actor = default)
    {
        var restoredDomainEntity = await SoftDeleteServiceRepository.RestoreAsync(id, actor);

        if (restoredDomainEntity == null)
        {
            return null;
        }

        await SoftDeleteServiceUow.SaveChangesAsync();
        return SoftDeleteServiceMapper.Map(restoredDomainEntity);
    }
}
