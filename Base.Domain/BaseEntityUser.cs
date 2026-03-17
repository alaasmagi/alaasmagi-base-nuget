using System.ComponentModel.DataAnnotations;
using Base.Contracts.Domain;

namespace Base.Domain;

/// <summary>
/// Provides a base entity implementation that stores a <see cref="Guid"/> user identifier.
/// </summary>
public abstract class BaseEntityUser : BaseEntityUser<Guid>
{
}

/// <summary>
/// Provides a base entity implementation that stores a strongly typed user identifier.
/// </summary>
/// <typeparam name="TKey">The identifier type of the user.</typeparam>
public abstract class BaseEntityUser<TKey> : IBaseEntityUserId<TKey>
    where TKey : IEquatable<TKey>
{
    /// <summary>
    /// Gets or sets the identifier of the user associated with the entity.
    /// </summary>
    [Required]
    public TKey UserId { get; set; } = default!;
}
