using Base.Contracts.Application;
using Base.Contracts.DataAccess;
using Base.Contracts.Domain;
using Base.Contracts.DTO;

namespace Base.Application;

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
    public BaseServiceSoftDelete(IBaseUow serviceUow, TRepository serviceRepository, TMapper serviceMapper)
        : base(serviceUow, serviceRepository, serviceMapper)
    {
    }

    public async Task<IEnumerable<TUpperEntity>?> GetAllAsync(bool includeSoftDeleted = false, TUserKey? userId = default)
    {
        var entities = await ServiceRepository.GetAllAsync(includeSoftDeleted, userId);
        return entities?.Select(entity => ServiceMapper.Map(entity)).OfType<TUpperEntity>();
    }

    public async Task<IEnumerable<TUpperEntity>?> GetAllByPageAsync(int pageNr, int pageSize, bool includeSoftDeleted = false, TUserKey? userId = default)
    {
        var entities = await ServiceRepository.GetAllByPageAsync(pageNr, pageSize, includeSoftDeleted, userId);
        return entities?.Select(entity => ServiceMapper.Map(entity)).OfType<TUpperEntity>();
    }

    public async Task<int> GetCountAsync(bool includeSoftDeleted = false, TUserKey? userId = default)
    {
        return await ServiceRepository.GetCountAsync(includeSoftDeleted, userId);
    }

    public async Task<TUpperEntity?> GetByIdAsync(TKey id, bool includeSoftDeleted = false, TUserKey? userId = default)
    {
        var entity = await ServiceRepository.GetByIdAsync(id, includeSoftDeleted, userId);
        return ServiceMapper.Map(entity);
    }

    public async Task<bool> ExistsAsync(TKey id, bool includeSoftDeleted = false, TUserKey? userId = default)
    {
        return await ServiceRepository.ExistsAsync(id, includeSoftDeleted, userId);
    }

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
