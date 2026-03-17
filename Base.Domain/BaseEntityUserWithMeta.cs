using System.ComponentModel.DataAnnotations;
using Base.Contracts.Domain;

namespace Base.Domain;

/// <summary>
/// Provides a base entity implementation that combines metadata with a <see cref="Guid"/> user identifier.
/// </summary>
public abstract class BaseEntityUserWithMeta : BaseEntityUserWithMeta<Guid, Guid>
{
}

/// <summary>
/// Provides a base entity implementation that combines metadata with a strongly typed user identifier.
/// </summary>
/// <typeparam name="TKey">The identifier type of the entity.</typeparam>
/// <typeparam name="TUserKey">The identifier type of the user.</typeparam>
public abstract class BaseEntityUserWithMeta<TKey, TUserKey> : BaseEntityWithMeta<TKey>, IBaseEntityUserId<TUserKey>
    where TKey : IEquatable<TKey>
    where TUserKey : IEquatable<TUserKey>
{
    /// <summary>
    /// Gets or sets the identifier of the user associated with the entity.
    /// </summary>
    [Required]
    public virtual TUserKey UserId { get; set; } = default!;
}
