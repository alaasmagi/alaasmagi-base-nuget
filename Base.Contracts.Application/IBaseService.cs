using Base.Contracts.DataAccess;
using Base.Contracts.Domain;

namespace Base.Contracts.Application;

/// <summary>
/// Defines a base application service contract for entities that use <see cref="Guid"/> as both the entity key and user key.
/// </summary>
/// <typeparam name="TEntity">The entity type handled by the service.</typeparam>
public interface IBaseService<TEntity> : IBaseService<TEntity, Guid, Guid>
    where TEntity : class, IBaseEntity
{
}

/// <summary>
/// Defines a base application service contract that extends repository operations for an entity type.
/// </summary>
/// <typeparam name="TEntity">The entity type handled by the service.</typeparam>
/// <typeparam name="TKey">The identifier type of the entity.</typeparam>
/// <typeparam name="TUserKey">The identifier type of the current user or owner.</typeparam>
public interface IBaseService<TEntity, TKey, TUserKey> : IBaseRepository<TEntity, TKey, TUserKey>
    where TEntity : class, IBaseEntity<TKey>
    where TKey : IEquatable<TKey>
    where TUserKey : IEquatable<TUserKey>
{
}
