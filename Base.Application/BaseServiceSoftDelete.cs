using Base.Contracts.Application;
using Base.Contracts.DataAccess;
using Base.Contracts.Domain;
using Base.Contracts.DTO;

namespace Base.Application;

/// <summary>
/// Provides a reusable base implementation for application services that support soft-delete operations.
/// </summary>
/// <typeparam name="TUpperEntity">The entity type exposed by the application layer.</typeparam>
/// <typeparam name="TLowerEntity">The entity type used by the data access layer.</typeparam>
/// <typeparam name="TRepository">The repository type used to access persisted entities.</typeparam>
/// <typeparam name="TMapper">The mapper type used to translate between upper and lower entity types.</typeparam>
/// <typeparam name="TKey">The identifier type of the entity.</typeparam>
/// <typeparam name="TUserKey">The identifier type of the current user or owner.</typeparam>
public class BaseServiceSoftDelete<TUpperEntity, TLowerEntity, TRepository, TMapper, TKey, TUserKey> :
    BaseService<TUpperEntity, TLowerEntity, TRepository, TMapper, TKey, TUserKey>,
    IBaseServiceSoftDelete<TUpperEntity, TKey, TUserKey>
    where TUpperEntity : class, IBaseEntity<TKey>, IBaseEntitySoftDelete
    where TLowerEntity : class, IBaseEntity<TKey>, IBaseEntitySoftDelete
    where TRepository : class, IBaseRepositorySoftDelete<TLowerEntity, TKey, TUserKey>
    where TMapper : class, IMapper<TUpperEntity, TLowerEntity, TKey>
    where TKey : IEquatable<TKey>
    where TUserKey : IEquatable<TUserKey>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BaseServiceSoftDelete{TUpperEntity, TLowerEntity, TRepository, TMapper, TKey, TUserKey}"/> class.
    /// </summary>
    /// <param name="serviceUow">The unit of work used to persist changes.</param>
    /// <param name="serviceRepository">The repository used to access lower-layer entities.</param>
    /// <param name="serviceMapper">The mapper used to translate between entity representations.</param>
    public BaseServiceSoftDelete(IBaseUow serviceUow, TRepository serviceRepository, TMapper serviceMapper)
        : base(serviceUow, serviceRepository, serviceMapper)
    {
    }

    /// <summary>
    /// Retrieves all entities while optionally including soft-deleted records and maps them to the application-layer type.
    /// </summary>
    /// <param name="includeSoftDeleted">Controls whether soft-deleted entities are included in the result.</param>
    /// <param name="userId">The optional user identifier used to scope the query.</param>
    /// <returns>
    /// A task that resolves to the mapped entities, or <see langword="null"/> when no entities are available.
    /// </returns>
    public async Task<IEnumerable<TUpperEntity>?> GetAllAsync(bool includeSoftDeleted = false, TUserKey? userId = default)
    {
        var entities = await ServiceRepository.GetAllAsync(includeSoftDeleted, userId);
        return entities?.Select(entity => ServiceMapper.Map(entity)).OfType<TUpperEntity>();
    }

    /// <summary>
    /// Retrieves a page of entities while optionally including soft-deleted records and maps them to the application-layer type.
    /// </summary>
    /// <param name="pageNr">The one-based page number to retrieve.</param>
    /// <param name="pageSize">The number of items to include in the page.</param>
    /// <param name="includeSoftDeleted">Controls whether soft-deleted entities are included in the result.</param>
    /// <param name="userId">The optional user identifier used to scope the query.</param>
    /// <returns>
    /// A task that resolves to the mapped entities for the requested page, or <see langword="null"/> when no entities are available.
    /// </returns>
    public async Task<IEnumerable<TUpperEntity>?> GetAllByPageAsync(int pageNr, int pageSize, bool includeSoftDeleted = false, TUserKey? userId = default)
    {
        var entities = await ServiceRepository.GetAllByPageAsync(pageNr, pageSize, includeSoftDeleted, userId);
        return entities?.Select(entity => ServiceMapper.Map(entity)).OfType<TUpperEntity>();
    }

    /// <summary>
    /// Counts entities while optionally including soft-deleted records.
    /// </summary>
    /// <param name="includeSoftDeleted">Controls whether soft-deleted entities are included in the count.</param>
    /// <param name="userId">The optional user identifier used to scope the query.</param>
    /// <returns>
    /// A task that resolves to the number of matching entities.
    /// </returns>
    public async Task<int> GetCountAsync(bool includeSoftDeleted = false, TUserKey? userId = default)
    {
        return await ServiceRepository.GetCountAsync(includeSoftDeleted, userId);
    }

    /// <summary>
    /// Retrieves an entity by its identifier while optionally including soft-deleted records and maps it to the application-layer type.
    /// </summary>
    /// <param name="id">The identifier of the entity to retrieve.</param>
    /// <param name="includeSoftDeleted">Controls whether soft-deleted entities are included in the search.</param>
    /// <param name="userId">The optional user identifier used to scope the query.</param>
    /// <returns>
    /// A task that resolves to the mapped entity, or <see langword="null"/> when it is not found or cannot be mapped.
    /// </returns>
    public async Task<TUpperEntity?> GetByIdAsync(TKey id, bool includeSoftDeleted = false, TUserKey? userId = default)
    {
        var entity = await ServiceRepository.GetByIdAsync(id, includeSoftDeleted, userId);
        return ServiceMapper.Map(entity);
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
    public async Task<bool> ExistsAsync(TKey id, bool includeSoftDeleted = false, TUserKey? userId = default)
    {
        return await ServiceRepository.ExistsAsync(id, includeSoftDeleted, userId);
    }

    /// <summary>
    /// Marks an entity as soft deleted and persists the change.
    /// </summary>
    /// <param name="id">The identifier of the entity to soft delete.</param>
    /// <param name="userId">The optional user identifier used to scope or stamp the operation.</param>
    /// <returns>
    /// A task that resolves to <see langword="true"/> when the entity was marked as deleted; otherwise, <see langword="false"/>.
    /// </returns>
    public async Task<bool> SoftDeleteAsync(TKey id, TUserKey? userId = default)
    {
        var deleted = await ServiceRepository.SoftDeleteAsync(id, userId);

        if (!deleted)
        {
            return false;
        }

        await ServiceUow.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Restores a soft-deleted entity and persists the change.
    /// </summary>
    /// <param name="id">The identifier of the entity to restore.</param>
    /// <param name="userId">The optional user identifier used to scope or stamp the operation.</param>
    /// <returns>
    /// A task that resolves to the restored entity, or <see langword="null"/> when the entity cannot be restored.
    /// </returns>
    public async Task<TUpperEntity?> RestoreAsync(TKey id, TUserKey? userId = default)
    {
        var restoredEntity = await ServiceRepository.RestoreAsync(id, userId);

        if (restoredEntity == null)
        {
            return null;
        }

        await ServiceUow.SaveChangesAsync();
        return ServiceMapper.Map(restoredEntity);
    }
}
