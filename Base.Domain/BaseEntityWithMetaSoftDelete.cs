using System.ComponentModel.DataAnnotations;
using Base.Contracts.Domain;

namespace Base.Domain;

/// <summary>
/// Provides a base entity implementation with metadata, soft-delete support, and a <see cref="Guid"/> identifier.
/// </summary>
public abstract class BaseEntityWithMetaSoftDelete : BaseEntityWithMetaSoftDelete<Guid>
{
}

/// <summary>
/// Provides a base entity implementation with metadata, soft-delete support, and a strongly typed identifier.
/// </summary>
/// <typeparam name="TKey">The identifier type of the entity.</typeparam>
public abstract class BaseEntityWithMetaSoftDelete<TKey> : BaseEntityWithMeta<TKey>, IBaseEntityWithMetaSoftDelete<TKey>
    where TKey : IEquatable<TKey>
{
    public bool IsDeleted { get; set; } = false;
}
