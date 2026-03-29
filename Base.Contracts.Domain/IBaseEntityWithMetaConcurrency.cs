namespace Base.Contracts.Domain;

/// <summary>
/// Defines an entity contract that combines an identifier, metadata, and a concurrency token using <see cref="Guid"/> as the identifier type.
/// </summary>
public interface IBaseEntityWithMetaConcurrency : IBaseEntityWithMetaConcurrency<Guid>
{
}

/// <summary>
/// Defines an entity contract that combines an identifier, metadata, and a concurrency token.
/// </summary>
/// <typeparam name="TKey">The identifier type of the entity.</typeparam>
public interface IBaseEntityWithMetaConcurrency<TKey> : IBaseEntityWithMeta<TKey>, IBaseEntityConcurrency
    where TKey : IEquatable<TKey>
{
}