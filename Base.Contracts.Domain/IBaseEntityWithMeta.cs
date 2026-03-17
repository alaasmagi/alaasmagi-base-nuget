namespace Base.Contracts.Domain;

/// <summary>
/// Defines an entity contract that combines an identifier and metadata using <see cref="Guid"/> as the identifier type.
/// </summary>
public interface IBaseEntityWithMeta : IBaseEntityWithMeta<Guid>
{
}

/// <summary>
/// Defines an entity contract that combines an identifier and metadata.
/// </summary>
/// <typeparam name="TKey">The identifier type of the entity.</typeparam>
public interface IBaseEntityWithMeta<TKey> : IBaseEntity<TKey>, IBaseEntityMeta
    where TKey : IEquatable<TKey>
{
}
