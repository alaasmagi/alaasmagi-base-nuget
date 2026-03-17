using System.ComponentModel.DataAnnotations;
using Base.Contracts.Domain;

namespace Base.Domain;

/// <summary>
/// Provides a base entity implementation that uses <see cref="Guid"/> as the identifier type.
/// </summary>
public abstract class BaseEntity : BaseEntity<Guid>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BaseEntity"/> class with a new identifier value.
    /// </summary>
    protected BaseEntity()
    {
        Id = Guid.NewGuid();
    }
}

/// <summary>
/// Provides a base entity implementation with a strongly typed identifier.
/// </summary>
/// <typeparam name="TKey">The identifier type of the entity.</typeparam>
public abstract class BaseEntity<TKey> : IBaseEntity<TKey>
    where TKey : IEquatable<TKey>
{
    /// <summary>
    /// Gets or sets the unique identifier of the entity.
    /// </summary>
    [Required]
    public virtual TKey Id { get; set; } = default!;
}
