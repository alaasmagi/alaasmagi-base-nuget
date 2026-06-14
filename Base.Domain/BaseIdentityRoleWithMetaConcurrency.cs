using System.ComponentModel.DataAnnotations;
using Base.Contracts.Domain;

namespace Base.Domain;

/// <summary>
/// Provides an ASP.NET Core Identity role base type with metadata, a repository-managed concurrency token, and a <see cref="Guid"/> identifier.
/// </summary>
public abstract class BaseIdentityRoleWithMetaConcurrency : BaseIdentityRoleWithMetaConcurrency<Guid>
{
}

/// <summary>
/// Provides an ASP.NET Core Identity role base type with metadata, a repository-managed concurrency token, and a strongly typed identifier.
/// </summary>
/// <typeparam name="TKey">The identifier type of the identity role.</typeparam>
public abstract class BaseIdentityRoleWithMetaConcurrency<TKey> : BaseIdentityRoleWithMeta<TKey>, IBaseEntityWithMetaConcurrency<TKey>
    where TKey : IEquatable<TKey>
{
    /// <summary>
    /// Gets or sets the current concurrency token value used to detect stale updates.
    /// </summary>
    [Required]
    [MaxLength(128)]
    public virtual string ConcurrencyToken { get; set; } = default!;
}
