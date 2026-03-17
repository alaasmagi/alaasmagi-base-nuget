using System.ComponentModel.DataAnnotations;
using Base.Contracts.Domain;

namespace Base.Domain;

/// <summary>
/// Provides a base entity implementation with metadata and a <see cref="Guid"/> identifier.
/// </summary>
public abstract class BaseEntityWithMeta : BaseEntityWithMeta<Guid>
{
}

/// <summary>
/// Provides a base entity implementation with metadata and a strongly typed identifier.
/// </summary>
/// <typeparam name="TKey">The identifier type of the entity.</typeparam>
public abstract class BaseEntityWithMeta<TKey> : BaseEntity<TKey>, IBaseEntityWithMeta<TKey>
    where TKey : IEquatable<TKey>
{
    /// <summary>
    /// Gets or sets the identifier of the actor who created the entity.
    /// </summary>
    [Required]
    [MaxLength(128)]
    public virtual string CreatedBy { get; set; } = default!;

    /// <summary>
    /// Gets or sets the UTC timestamp when the entity was created.
    /// </summary>
    [Required]
    public virtual DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the identifier of the actor who last updated the entity.
    /// </summary>
    [Required]
    [MaxLength(128)]
    public virtual string UpdatedBy { get; set; } = default!;

    /// <summary>
    /// Gets or sets the UTC timestamp when the entity was last updated.
    /// </summary>
    [Required]
    public virtual DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
