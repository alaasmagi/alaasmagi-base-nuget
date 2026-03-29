using Base.Contracts.Application;
using Base.Contracts.DataAccess;
using Base.Contracts.Domain;
using Base.Contracts.DTO;
using Base.DTO;

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
/// <typeparam name="TActor">The identifier type of the actor used to scope or stamp service operations.</typeparam>
public class BaseService<TEntity, TDomainEntity, TRepository, TKey, TActor> : IBaseService<TEntity, TKey, TActor>
    where TEntity : class
    where TDomainEntity : class, IBaseEntity<TKey>
    where TRepository : class, IBaseRepository<TDomainEntity, TKey, TActor>
    where TKey : IEquatable<TKey>
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
    /// Gets the default error code used when entity updates fail.
    /// </summary>
    protected virtual string UpdatingFailureErrorCode => ErrorDefaults.Codes.UpdateFailed;

    /// <summary>
    /// Gets the default error message used when entity updates fail.
    /// </summary>
    protected virtual string UpdatingFailureErrorMessage => ErrorDefaults.Messages.UpdateFailed;

    /// <summary>
    /// Gets the default error code used when entity removal fails.
    /// </summary>
    protected virtual string RemovingFailureErrorCode => ErrorDefaults.Codes.RemoveFailed;

    /// <summary>
    /// Gets the default error message used when entity removal fails.
    /// </summary>
    protected virtual string RemovingFailureErrorMessage => ErrorDefaults.Messages.RemoveFailed;

    /// <summary>
    /// Gets the default error code used when entity mapping fails.
    /// </summary>
    protected virtual string MappingFailureErrorCode => ErrorDefaults.Codes.MapFailed;

    /// <summary>
    /// Gets the default error message used when entity mapping fails.
    /// </summary>
    protected virtual string MappingFailureErrorMessage => ErrorDefaults.Messages.MapToServiceModelFailed;

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
    /// Initializes a new instance of the <see cref="BaseService{TEntity, TDomainEntity, TRepository, TKey, TActor}"/> class.
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
    /// Retrieves all entities visible to the specified actor.
    /// </summary>
    public virtual async Task<IMethodResponse<IEnumerable<TEntity>>> GetAllAsync(TActor? actor = default)
    {
        var repositoryResponse = await ServiceRepository.GetAllAsync(actor);

        if (!repositoryResponse.Successful)
        {
            return MethodResponse<IEnumerable<TEntity>>.Failure(repositoryResponse.Error ?? CreateError(NotFoundErrorCode, NotFoundErrorMessage));
        }

        var mappedEntities = ServiceMapper.Map(repositoryResponse.Value);

        if (mappedEntities == null)
        {
            return MethodResponse<IEnumerable<TEntity>>.Failure(CreateError(MappingFailureErrorCode, MappingFailureErrorMessage));
        }

        return MethodResponse<IEnumerable<TEntity>>.Success(mappedEntities);
    }

    /// <summary>
    /// Retrieves a single page of entities visible to the specified actor.
    /// </summary>
    public virtual async Task<IMethodResponse<IEnumerable<TEntity>>> GetAllByPageAsync(int pageNr, int pageSize, TActor? actor = default)
    {
        var repositoryResponse = await ServiceRepository.GetAllByPageAsync(pageNr, pageSize, actor);

        if (!repositoryResponse.Successful)
        {
            return MethodResponse<IEnumerable<TEntity>>.Failure(repositoryResponse.Error ?? CreateError(NotFoundErrorCode, NotFoundErrorMessage));
        }

        var mappedEntities = ServiceMapper.Map(repositoryResponse.Value);

        if (mappedEntities == null)
        {
            return MethodResponse<IEnumerable<TEntity>>.Failure(CreateError(MappingFailureErrorCode, MappingFailureErrorMessage));
        }

        return MethodResponse<IEnumerable<TEntity>>.Success(mappedEntities);
    }

    /// <summary>
    /// Counts all entities visible to the specified actor.
    /// </summary>
    public virtual async Task<IMethodResponse<int>> GetCountAsync(TActor? actor = default)
    {
        var repositoryResponse = await ServiceRepository.GetCountAsync(actor);

        if (!repositoryResponse.Successful)
        {
            return MethodResponse<int>.Failure(repositoryResponse.Error ?? CreateError(NotFoundErrorCode, NotFoundErrorMessage));
        }

        return MethodResponse<int>.Success(repositoryResponse.Value);
    }

    /// <summary>
    /// Determines whether an entity with the specified identifier exists.
    /// </summary>
    public virtual async Task<IMethodResponse<bool>> ExistsAsync(TKey id, TActor? actor = default)
    {
        var repositoryResponse = await ServiceRepository.ExistsAsync(id, actor);

        if (!repositoryResponse.Successful)
        {
            return MethodResponse<bool>.Failure(repositoryResponse.Error ?? CreateError(NotFoundErrorCode, NotFoundErrorMessage));
        }

        return MethodResponse<bool>.Success(repositoryResponse.Value);
    }

    /// <summary>
    /// Creates a standard error payload for service-level operation failures.
    /// </summary>
    protected virtual IError CreateError(string code, string message) => new Error(code, message);

    /// <summary>
    /// Retrieves an entity by its identifier.
    /// </summary>
    public virtual async Task<IMethodResponse<TEntity>> GetByIdAsync(TKey id, TActor? actor = default)
    {
        var repositoryResponse = await ServiceRepository.GetByIdAsync(id, actor);

        if (!repositoryResponse.Successful)
        {
            return MethodResponse<TEntity>.Failure(repositoryResponse.Error ?? CreateError(NotFoundErrorCode, NotFoundErrorMessage));
        }

        var mappedEntity = ServiceMapper.Map(repositoryResponse.Value);

        if (mappedEntity == null)
        {
            return MethodResponse<TEntity>.Failure(CreateError(MappingFailureErrorCode, MappingFailureErrorMessage));
        }

        return MethodResponse<TEntity>.Success(mappedEntity);
    }

    /// <summary>
    /// Creates a new entity instance.
    /// </summary>
    public virtual async Task<IMethodResponse<TEntity>> CreateAsync(TEntity entity, TActor? actor = default)
    {
        var domainEntity = ServiceMapper.Map(entity);

        if (domainEntity == null)
        {
            return MethodResponse<TEntity>.Failure(CreateError(MappingFailureErrorCode, MappingFailureErrorMessage));
        }

        var repositoryResponse = await ServiceRepository.CreateAsync(domainEntity, actor);

        if (!repositoryResponse.Successful)
        {
            return MethodResponse<TEntity>.Failure(repositoryResponse.Error ?? CreateError(MappingFailureErrorCode, MappingFailureErrorMessage));
        }

        await ServiceUow.SaveChangesAsync();
        var mappedEntity = ServiceMapper.Map(repositoryResponse.Value);

        if (mappedEntity == null)
        {
            return MethodResponse<TEntity>.Failure(CreateError(MappingFailureErrorCode, MappingFailureErrorMessage));
        }

        return MethodResponse<TEntity>.Success(mappedEntity);
    }

    /// <summary>
    /// Updates an existing entity instance and forwards the supplied concurrency token to the repository.
    /// </summary>
    public virtual async Task<IMethodResponse<TEntity>> UpdateAsync(TKey id, TEntity entity, string? expectedConcurrencyToken = default, TActor? actor = default)
    {
        var domainEntity = ServiceMapper.Map(entity);

        if (domainEntity == null)
        {
            return MethodResponse<TEntity>.Failure(CreateError(MappingFailureErrorCode, MappingFailureErrorMessage));
        }

        var repositoryResponse = await ServiceRepository.UpdateAsync(id, domainEntity, expectedConcurrencyToken, actor);

        if (!repositoryResponse.Successful)
        {
            return MethodResponse<TEntity>.Failure(repositoryResponse.Error ?? CreateError(UpdatingFailureErrorCode, UpdatingFailureErrorMessage));
        }

        await ServiceUow.SaveChangesAsync();
        var mappedEntity = ServiceMapper.Map(repositoryResponse.Value);

        if (mappedEntity == null)
        {
            return MethodResponse<TEntity>.Failure(CreateError(MappingFailureErrorCode, MappingFailureErrorMessage));
        }

        return MethodResponse<TEntity>.Success(mappedEntity);
    }

    /// <summary>
    /// Removes an entity by its identifier and forwards the supplied concurrency token to the repository.
    /// </summary>
    public virtual async Task<IMethodResponse<bool>> RemoveAsync(TKey id, string? expectedConcurrencyToken = default, TActor? actor = default)
    {
        var repositoryResponse = await ServiceRepository.RemoveAsync(id, expectedConcurrencyToken, actor);

        if (!repositoryResponse.Successful)
        {
            return MethodResponse<bool>.Failure(repositoryResponse.Error ?? CreateError(RemovingFailureErrorCode, RemovingFailureErrorMessage));
        }

        await ServiceUow.SaveChangesAsync();
        return MethodResponse<bool>.Success(true);
    }
}
