using Base.Contracts.Application;
using Base.Contracts.DataAccess;
using Base.Contracts.Domain;
using Base.Contracts.DTO;

namespace Base.Application;

public class BaseService<TUpperEntity, TLowerEntity, TRepository, TMapper, TKey, TUserKey> : IBaseService<TUpperEntity, TKey, TUserKey> 
    where TUpperEntity : class, IBaseEntity<TKey>
    where TLowerEntity : class, IBaseEntity<TKey>
    where TRepository : class, IBaseRepository<TLowerEntity, TKey, TUserKey>
    where TMapper : class, IMapper<TUpperEntity, TLowerEntity, TKey>
    where TKey : IEquatable<TKey> 
    where TUserKey : IEquatable<TUserKey>
{
    protected readonly IBaseUow ServiceUow;
    protected readonly TRepository ServiceRepository;
    protected readonly TMapper ServiceMapper;
    
    public BaseService(IBaseUow serviceUow, TRepository serviceRepository, TMapper serviceMapper)
    {
        ServiceUow = serviceUow;
        ServiceRepository = serviceRepository;
        ServiceMapper = serviceMapper;
    }

    public async Task<IEnumerable<TUpperEntity>?> GetAllAsync(TUserKey? userId = default)
    {
        var entities = await ServiceRepository.GetAllAsync(userId);
        return entities?.Select(entity => ServiceMapper.Map(entity)).OfType<TUpperEntity>();
    }

    public async Task<IEnumerable<TUpperEntity>?> GetAllByPageAsync(int pageNr, int pageSize, TUserKey? userId = default)
    {
        var entities = await ServiceRepository.GetAllByPageAsync(pageNr, pageSize, userId);
        return entities?.Select(entity => ServiceMapper.Map(entity)).OfType<TUpperEntity>();
    }

    public async Task<int> GetCountAsync(TUserKey? userId = default)
    {
        return await ServiceRepository.GetCountAsync(userId);
    }

    public async Task<bool> ExistsAsync(TKey id, TUserKey? userId = default)
    {
        return await ServiceRepository.ExistsAsync(id, userId);
    }

    public async Task<TUpperEntity?> GetByIdAsync(TKey id, TUserKey? userId = default)
    {
        var entity = await ServiceRepository.GetByIdAsync(id, userId);
        return ServiceMapper.Map(entity);
    }

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
