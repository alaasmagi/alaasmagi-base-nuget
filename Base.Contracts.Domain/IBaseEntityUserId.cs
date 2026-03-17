namespace Base.Contracts.Domain;

/// <summary>
/// Defines a user ownership contract that uses <see cref="Guid"/> as the user identifier type.
/// </summary>
public interface IBaseEntityUserId : IBaseEntityUserId<Guid>
{
}

/// <summary>
/// Defines a user ownership contract with a strongly typed user identifier.
/// </summary>
/// <typeparam name="TKey">The identifier type of the user.</typeparam>
public interface IBaseEntityUserId<TKey>
    where TKey : IEquatable<TKey>
{
    /// <summary>
    /// Gets or sets the identifier of the user associated with the entity.
    /// </summary>
    public TKey UserId { get; set; }
}
