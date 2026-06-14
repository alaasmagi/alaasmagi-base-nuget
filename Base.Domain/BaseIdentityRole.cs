using Base.Contracts.Domain;
using Microsoft.AspNetCore.Identity;

namespace Base.Domain;

/// <summary>
/// Provides an ASP.NET Core Identity role base type that uses <see cref="Guid"/> as the identifier type.
/// </summary>
public abstract class BaseIdentityRole : BaseIdentityRole<Guid>
{
}

/// <summary>
/// Provides an ASP.NET Core Identity role base type with a strongly typed identifier.
/// </summary>
/// <typeparam name="TKey">The identifier type of the identity role.</typeparam>
public abstract class BaseIdentityRole<TKey> : IdentityRole<TKey>, IBaseEntity<TKey>
    where TKey : IEquatable<TKey>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BaseIdentityRole{TKey}"/> class.
    /// A new identifier is assigned automatically when <typeparamref name="TKey"/> is <see cref="Guid"/>.
    /// </summary>
    protected BaseIdentityRole()
    {
        if (typeof(TKey) == typeof(Guid) && EqualityComparer<TKey>.Default.Equals(Id, default!))
        {
            Id = (TKey)(object)Guid.NewGuid();
        }
    }
}
