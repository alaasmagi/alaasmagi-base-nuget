using System.ComponentModel.DataAnnotations;
using Base.Contracts.Domain;

namespace Base.Domain;

/// <summary>
/// Provides a base entity implementation that stores a <see cref="Guid"/> entity identifier and a <see cref="Guid"/> user identifier.
/// </summary>
public abstract class BaseEntityUser : BaseEntityUser<Guid, Guid>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BaseEntityUser"/> class with a new identifier value.
    /// </summary>
    protected BaseEntityUser()
    {
        Id = Guid.NewGuid();
    }
}

/// <summary>
/// Provides a base entity implementation that stores a strongly typed entity identifier and user identifier.
/// </summary>
/// <typeparam name="TKey">The identifier type of the entity.</typeparam>
/// <typeparam name="TUserKey">The identifier type of the user.</typeparam>
public abstract class BaseEntityUser<TKey, TUserKey> : BaseEntity<TKey>, IBaseEntityUserId<TUserKey>
    where TKey : IEquatable<TKey>
    where TUserKey : IEquatable<TUserKey>
{
    /// <summary>
    /// Gets or sets the identifier of the user associated with the entity.
    /// </summary>
    [Required]
    public virtual TUserKey UserId { get; set; } = default!;
}
