using Base.Contracts.DataAccess;
using Base.Contracts.Domain;

namespace Base.Contracts.Application;

/// <summary>
/// Defines a base soft-delete aware application service contract for entities that use <see cref="Guid"/> as both the entity key and user key.
/// </summary>
/// <typeparam name="TEntity">The entity type handled by the service.</typeparam>
public interface IBaseServiceSoftDelete<TEntity> : IBaseServiceSoftDelete<TEntity, Guid, Guid>
    where TEntity : class, IBaseEntity, IBaseEntitySoftDelete
{
}

/// <summary>
/// Defines a base soft-delete aware application service contract that extends regular service operations with soft-delete specific members.
/// </summary>
/// <typeparam name="TEntity">The entity type handled by the service.</typeparam>
/// <typeparam name="TKey">The identifier type of the entity.</typeparam>
/// <typeparam name="TUserKey">The identifier type of the current user or owner.</typeparam>
public interface IBaseServiceSoftDelete<TEntity, TKey, TUserKey> : IBaseService<TEntity, TKey, TUserKey>,
    IBaseRepositorySoftDelete<TEntity, TKey, TUserKey>
    where TEntity : class, IBaseEntity<TKey>, IBaseEntitySoftDelete
    where TKey : IEquatable<TKey>
    where TUserKey : IEquatable<TUserKey>
{
}
