using Base.Contracts.Application;
using Base.Contracts.DataAccess;
using Base.Contracts.Domain;
using Base.Contracts.DTO;

namespace Base.Application;

public class BaseService<TEntity, TDomainEntity, TRepository> : BaseService<TEntity, TDomainEntity, TRepository, Guid, Guid>
    where TEntity : class
    where TDomainEntity : class, IBaseEntity<Guid>
    where TRepository : class, IBaseRepository<TDomainEntity, Guid, Guid>
{
    public BaseService(IBaseUow serviceUow, TRepository serviceRepository, IMapper<TEntity, TDomainEntity, Guid> serviceMapper) : 
        base(serviceUow, serviceRepository, serviceMapper)
    {
    }
}

/// <summary>
/// Provides a reusable base implementation for application services that map between application-layer entities and repository entities.
/// </summary>
/// <typeparam name="TEntity">The application-layer entity type exposed by the service.</typeparam>
/// <typeparam name="TDomainEntity">The repository/domain entity type used for persistence.</typeparam>
/// <typeparam name="TRepository">The repository type used to access persisted entities.</typeparam>
/// <typeparam name="TKey">The identifier type of the entity.</typeparam>
/// <typeparam name="TUserKey">The identifier type of the current user or owner.</typeparam>
public class BaseService<TEntity, TDomainEntity, TRepository, TKey, TUserKey> : IBaseService<TEntity, TKey, TUserKey>
    where TEntity : class
    where TDomainEntity : class, IBaseEntity<TKey>
    where TRepository : class, IBaseRepository<TDomainEntity, TKey, TUserKey>
    where TKey : IEquatable<TKey>
    where TUserKey : IEquatable<TUserKey>
{
    /// <summary>
    /// Stores the unit of work used to persist service-level changes.
    /// </summary>
    protected readonly IBaseUow ServiceUow;

    /// <summary>
    /// Stores the repository used for entity persistence operations.
    /// </summary>
    protected readonly TRepository ServiceRepository;

    /// <summary>
    /// Stores the mapper used to translate between application-layer and repository entities.
    /// </summary>
    protected readonly IMapper<TEntity, TDomainEntity, TKey> ServiceMapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseService{TEntity, TDomainEntity, TRepository, TKey, TUserKey}"/> class.
    /// </summary>
    /// <param name="serviceUow">The unit of work used to persist changes.</param>
    /// <param name="serviceRepository">The repository used to access entities.</param>
    /// <param name="serviceMapper">The mapper used to translate between application-layer and repository entities.</param>
    public BaseService(
        IBaseUow serviceUow,
        TRepository serviceRepository,
        IMapper<TEntity, TDomainEntity, TKey> serviceMapper)
    {
        ServiceUow = serviceUow;
        ServiceRepository = serviceRepository;
        ServiceMapper = serviceMapper;
    }

    /// <summary>
    /// Retrieves all entities visible to the specified user.
    /// </summary>
    public async Task<IEnumerable<TEntity>?> GetAllAsync(TUserKey? userId = default)
    {
        var domainEntities = await ServiceRepository.GetAllAsync(userId);
        return ServiceMapper.Map(domainEntities);
    }

    /// <summary>
    /// Retrieves a page of entities visible to the specified user.
    /// </summary>
    public async Task<IEnumerable<TEntity>?> GetAllByPageAsync(int pageNr, int pageSize, TUserKey? userId = default)
    {
        var domainEntities = await ServiceRepository.GetAllByPageAsync(pageNr, pageSize, userId);
        return ServiceMapper.Map(domainEntities);
    }

    /// <summary>
    /// Counts the entities visible to the specified user.
    /// </summary>
    public async Task<int> GetCountAsync(TUserKey? userId = default)
    {
        return await ServiceRepository.GetCountAsync(userId);
    }

    /// <summary>
    /// Determines whether an entity with the specified identifier exists.
    /// </summary>
    public async Task<bool> ExistsAsync(TKey id, TUserKey? userId = default)
    {
        return await ServiceRepository.ExistsAsync(id, userId);
    }

    /// <summary>
    /// Retrieves an entity by its identifier.
    /// </summary>
    public async Task<TEntity?> GetByIdAsync(TKey id, TUserKey? userId = default)
    {
        var domainEntity = await ServiceRepository.GetByIdAsync(id, userId);
        return ServiceMapper.Map(domainEntity);
    }

    /// <summary>
    /// Creates a new entity and persists the change.
    /// </summary>
    public async Task<TEntity?> CreateAsync(TEntity entity, TUserKey? userId = default)
    {
        var domainEntity = ServiceMapper.Map(entity);

        if (domainEntity == null)
        {
            return null;
        }

        var createdDomainEntity = await ServiceRepository.CreateAsync(domainEntity, userId);

        if (createdDomainEntity == null)
        {
            return null;
        }

        await ServiceUow.SaveChangesAsync();
        return ServiceMapper.Map(createdDomainEntity);
    }

    /// <summary>
    /// Updates an existing entity and persists the change.
    /// </summary>
    public async Task<TEntity?> UpdateAsync(TKey id, TEntity entity, TUserKey? userId = default)
    {
        var domainEntity = ServiceMapper.Map(entity);

        if (domainEntity == null)
        {
            return null;
        }

        var updatedDomainEntity = await ServiceRepository.UpdateAsync(id, domainEntity, userId);

        if (updatedDomainEntity == null)
        {
            return null;
        }

        await ServiceUow.SaveChangesAsync();
        return ServiceMapper.Map(updatedDomainEntity);
    }

    /// <summary>
    /// Removes an entity by its identifier and persists the change.
    /// </summary>
    public async Task<bool> RemoveAsync(TKey id, TUserKey? userId = default)
    {
        var removed = await ServiceRepository.RemoveAsync(id, userId);

        if (!removed)
        {
            return false;
        }

        await ServiceUow.SaveChangesAsync();
        return true;
    }
}
