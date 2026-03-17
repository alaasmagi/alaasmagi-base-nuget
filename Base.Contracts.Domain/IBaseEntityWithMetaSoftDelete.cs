namespace Base.Contracts.Domain;

/// <summary>
/// Defines an entity contract that combines an identifier, metadata, and soft-delete support using <see cref="Guid"/> as the identifier type.
/// </summary>
public interface IBaseEntityWithMetaSoftDelete : IBaseEntityWithMetaSoftDelete<Guid>
{
}

/// <summary>
/// Defines an entity contract that combines an identifier, metadata, and soft-delete support.
/// </summary>
/// <typeparam name="TKey">The identifier type of the entity.</typeparam>
public interface IBaseEntityWithMetaSoftDelete<TKey> : IBaseEntity<TKey>, IBaseEntityMeta, IBaseEntitySoftDelete
    where TKey : IEquatable<TKey>
{
}
