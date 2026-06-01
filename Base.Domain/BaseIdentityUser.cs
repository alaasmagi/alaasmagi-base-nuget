using Base.Contracts.Domain;
using Microsoft.AspNetCore.Identity;

namespace Base.Domain;

/// <summary>
/// Provides an ASP.NET Core Identity user base type that uses <see cref="Guid"/> as the identifier type.
/// </summary>
public abstract class BaseIdentityUser : BaseIdentityUser<Guid>
{
}

/// <summary>
/// Provides an ASP.NET Core Identity user base type with a strongly typed identifier.
/// </summary>
/// <typeparam name="TKey">The identifier type of the identity user.</typeparam>
public abstract class BaseIdentityUser<TKey> : IdentityUser<TKey>, IBaseEntity<TKey>
    where TKey : IEquatable<TKey>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BaseIdentityUser{TKey}"/> class.
    /// A new identifier is assigned automatically when <typeparamref name="TKey"/> is <see cref="Guid"/>.
    /// </summary>
    protected BaseIdentityUser()
    {
        if (typeof(TKey) == typeof(Guid) && EqualityComparer<TKey>.Default.Equals(Id, default!))
        {
            Id = (TKey)(object)Guid.NewGuid();
        }
    }
}
