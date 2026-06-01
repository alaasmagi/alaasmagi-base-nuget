using System.ComponentModel.DataAnnotations;
using Base.Contracts.Domain;

namespace Base.Domain;

/// <summary>
/// Provides an ASP.NET Core Identity user base type with metadata, soft-delete support, and a <see cref="Guid"/> identifier.
/// </summary>
public abstract class BaseIdentityUserWithMetaSoftDelete : BaseIdentityUserWithMetaSoftDelete<Guid>
{
}

/// <summary>
/// Provides an ASP.NET Core Identity user base type with metadata, soft-delete support, and a strongly typed identifier.
/// </summary>
/// <typeparam name="TKey">The identifier type of the identity user.</typeparam>
public abstract class BaseIdentityUserWithMetaSoftDelete<TKey> : BaseIdentityUserWithMeta<TKey>, IBaseEntityWithMetaSoftDelete<TKey>
    where TKey : IEquatable<TKey>
{
    /// <summary>
    /// Gets or sets a value indicating whether the entity is soft deleted.
    /// </summary>
    [Required]
    public virtual bool IsDeleted { get; set; } = false;
}
