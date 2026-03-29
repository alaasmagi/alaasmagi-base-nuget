namespace Base.Contracts.Domain;

/// <summary>
/// Defines an entity contract that combines an identifier, metadata, soft-delete state, and a repository-managed concurrency token using <see cref="Guid"/> as the identifier type.
/// </summary>
public interface IBaseEntityWithMetaSoftDeleteConcurrency : IBaseEntityWithMetaSoftDeleteConcurrency<Guid>
{
}

/// <summary>
/// Defines an entity contract that combines an identifier, metadata, soft-delete state, and a repository-managed concurrency token.
/// </summary>
/// <typeparam name="TKey">The identifier type of the entity.</typeparam>
public interface IBaseEntityWithMetaSoftDeleteConcurrency<TKey> : IBaseEntityWithMetaSoftDelete<TKey>, IBaseEntityConcurrency
    where TKey : IEquatable<TKey>
{
}
