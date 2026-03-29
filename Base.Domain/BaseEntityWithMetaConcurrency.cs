using System.ComponentModel.DataAnnotations;
using Base.Contracts.Domain;

namespace Base.Domain;

/// <summary>
/// Provides a base entity implementation with metadata, a repository-managed concurrency token, and a <see cref="Guid"/> identifier.
/// </summary>
public abstract class BaseEntityWithMetaConcurrency : BaseEntityWithMetaConcurrency<Guid>
{
}

/// <summary>
/// Provides a base entity implementation with metadata, a repository-managed concurrency token, and a strongly typed identifier.
/// </summary>
/// <typeparam name="TKey">The identifier type of the entity.</typeparam>
public abstract class BaseEntityWithMetaConcurrency<TKey> : BaseEntityWithMeta<TKey>, IBaseEntityWithMetaConcurrency<TKey>
    where TKey : IEquatable<TKey>
{
    /// <summary>
    /// Gets or sets the current concurrency token value used to detect stale updates.
    /// </summary>
    [Required]
    [MaxLength(128)]
    public virtual string ConcurrencyToken { get; set; } = default!;
}
