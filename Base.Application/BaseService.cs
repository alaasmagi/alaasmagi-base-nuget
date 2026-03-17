using Base.Contracts.Application;
using Base.Contracts.DataAccess;
using Base.Contracts.Domain;
using Base.Contracts.DTO;

namespace Base.Application;

/// <summary>
/// Provides a reusable base implementation for application services that wrap repository operations and mapping logic.
/// </summary>
/// <typeparam name="TUpperEntity">The entity type exposed by the application layer.</typeparam>
/// <typeparam name="TLowerEntity">The entity type used by the data access layer.</typeparam>
/// <typeparam name="TRepository">The repository type used to access persisted entities.</typeparam>
/// <typeparam name="TMapper">The mapper type used to translate between upper and lower entity types.</typeparam>
/// <typeparam name="TKey">The identifier type of the entity.</typeparam>
/// <typeparam name="TUserKey">The identifier type of the current user or owner.</typeparam>
public class BaseService<TUpperEntity, TLowerEntity, TRepository, TMapper, TKey, TUserKey> : IBaseService<TUpperEntity, TKey, TUserKey>
    where TUpperEntity : class, IBaseEntity<TKey>
    where TLowerEntity : class, IBaseEntity<TKey>
    where TRepository : class, IBaseRepository<TLowerEntity, TKey, TUserKey>
    where TMapper : class, IMapper<TUpperEntity, TLowerEntity, TKey>
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
    /// Stores the mapper used to convert between upper and lower entity models.
    /// </summary>
    protected readonly TMapper ServiceMapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseService{TUpperEntity, TLowerEntity, TRepository, TMapper, TKey, TUserKey}"/> class.
    /// </summary>
    /// <param name="serviceUow">The unit of work used to persist changes.</param>
    /// <param name="serviceRepository">The repository used to access lower-layer entities.</param>
    /// <param name="serviceMapper">The mapper used to translate between entity representations.</param>
    public BaseService(IBaseUow serviceUow, TRepository serviceRepository, TMapper serviceMapper)
    {
        ServiceUow = serviceUow;
        ServiceRepository = serviceRepository;
        ServiceMapper = serviceMapper;
    }

    /// <summary>
    /// Retrieves all entities visible to the specified user and maps them to the application-layer type.
    /// </summary>
    /// <param name="userId">The optional user identifier used to scope the query.</param>
    /// <returns>
    /// A task that resolves to the mapped entities, or <see langword="null"/> when no entities are available.
    /// </returns>
    public async Task<IEnumerable<TUpperEntity>?> GetAllAsync(TUserKey? userId = default)
    {
        var entities = await ServiceRepository.GetAllAsync(userId);
        return entities?.Select(entity => ServiceMapper.Map(entity)).OfType<TUpperEntity>();
    }

    /// <summary>
    /// Retrieves a page of entities visible to the specified user and maps them to the application-layer type.
    /// </summary>
    /// <param name="pageNr">The one-based page number to retrieve.</param>
    /// <param name="pageSize">The number of items to include in the page.</param>
    /// <param name="userId">The optional user identifier used to scope the query.</param>
    /// <returns>
    /// A task that resolves to the mapped entities for the requested page, or <see langword="null"/> when no entities are available.
    /// </returns>
    public async Task<IEnumerable<TUpperEntity>?> GetAllByPageAsync(int pageNr, int pageSize, TUserKey? userId = default)
    {
        var entities = await ServiceRepository.GetAllByPageAsync(pageNr, pageSize, userId);
        return entities?.Select(entity => ServiceMapper.Map(entity)).OfType<TUpperEntity>();
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
        return await ServiceRepository.GetCountAsync(userId);
    }

    /// <summary>
    /// Determines whether an entity with the specified identifier exists.
    /// </summary>
    /// <param name="id">The identifier of the entity to check.</param>
    /// <param name="userId">The optional user identifier used to scope the query.</param>
    /// <returns>
    /// A task that resolves to <see langword="true"/> when the entity exists; otherwise, <see langword="false"/>.
    /// </returns>
    public async Task<bool> ExistsAsync(TKey id, TUserKey? userId = default)
    {
        return await ServiceRepository.ExistsAsync(id, userId);
    }

    /// <summary>
    /// Retrieves an entity by its identifier and maps it to the application-layer type.
    /// </summary>
    /// <param name="id">The identifier of the entity to retrieve.</param>
    /// <param name="userId">The optional user identifier used to scope the query.</param>
    /// <returns>
    /// A task that resolves to the mapped entity, or <see langword="null"/> when it is not found or cannot be mapped.
    /// </returns>
    public async Task<TUpperEntity?> GetByIdAsync(TKey id, TUserKey? userId = default)
    {
        var entity = await ServiceRepository.GetByIdAsync(id, userId);
        return ServiceMapper.Map(entity);
    }

    /// <summary>
    /// Creates a new entity, persists the change, and maps the created result back to the application-layer type.
    /// </summary>
    /// <param name="entity">The application-layer entity to create.</param>
    /// <param name="userId">The optional user identifier used to scope or stamp the operation.</param>
    /// <returns>
    /// A task that resolves to the created entity, or <see langword="null"/> when mapping or creation fails.
    /// </returns>
    public async Task<TUpperEntity?> CreateAsync(TUpperEntity entity, TUserKey? userId = default)
    {
        var lowerEntity = ServiceMapper.Map(entity);

        if (lowerEntity == null)
        {
            return null;
        }

        var createdEntity = await ServiceRepository.CreateAsync(lowerEntity, userId);
        await ServiceUow.SaveChangesAsync();
        return ServiceMapper.Map(createdEntity);
    }

    /// <summary>
    /// Updates an existing entity, persists the change, and maps the updated result back to the application-layer type.
    /// </summary>
    /// <param name="id">The identifier of the entity to update.</param>
    /// <param name="entity">The application-layer entity state to persist.</param>
    /// <param name="userId">The optional user identifier used to scope or stamp the operation.</param>
    /// <returns>
    /// A task that resolves to the updated entity, or <see langword="null"/> when mapping fails or the entity cannot be updated.
    /// </returns>
    public async Task<TUpperEntity?> UpdateAsync(TKey id, TUpperEntity entity, TUserKey? userId = default)
    {
        var lowerEntity = ServiceMapper.Map(entity);

        if (lowerEntity == null)
        {
            return null;
        }

        var updatedEntity = await ServiceRepository.UpdateAsync(id, lowerEntity, userId);

        if (updatedEntity == null)
        {
            return null;
        }

        await ServiceUow.SaveChangesAsync();
        return ServiceMapper.Map(updatedEntity);
    }

    /// <summary>
    /// Removes an entity by its identifier and persists the change.
    /// </summary>
    /// <param name="id">The identifier of the entity to remove.</param>
    /// <param name="userId">The optional user identifier used to scope the operation.</param>
    /// <returns>
    /// A task that resolves to <see langword="true"/> when the entity was removed; otherwise, <see langword="false"/>.
    /// </returns>
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
