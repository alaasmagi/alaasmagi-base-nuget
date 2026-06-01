using System.ComponentModel.DataAnnotations;
using Base.Contracts.Domain;

namespace Base.Domain;

/// <summary>
/// Provides a base entity implementation with metadata, soft-delete support, a repository-managed concurrency token, and a <see cref="Guid"/> identifier.
/// </summary>
public abstract class BaseEntityWithMetaSoftDeleteConcurrency : BaseEntityWithMetaSoftDeleteConcurrency<Guid>
{
}

/// <summary>
/// Provides a base entity implementation with metadata, soft-delete support, a repository-managed concurrency token, and a strongly typed identifier.
/// </summary>
/// <typeparam name="TKey">The identifier type of the entity.</typeparam>
public abstract class BaseEntityWithMetaSoftDeleteConcurrency<TKey> : BaseEntityWithMetaConcurrency<TKey>, IBaseEntityWithMetaSoftDeleteConcurrency<TKey>
    where TKey : IEquatable<TKey>
{
    /// <summary>
    /// Gets or sets a value indicating whether the entity is soft deleted.
    /// </summary>
    [Required]
    public virtual bool IsDeleted { get; set; } = false;
}
