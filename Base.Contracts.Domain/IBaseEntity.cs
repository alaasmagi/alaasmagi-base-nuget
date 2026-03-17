namespace Base.Contracts.Domain;

/// <summary>
/// Defines a base entity contract that uses <see cref="Guid"/> as the identifier type.
/// </summary>
public interface IBaseEntity : IBaseEntity<Guid>
{
}

/// <summary>
/// Defines a base entity contract with a strongly typed identifier.
/// </summary>
/// <typeparam name="TKey">The identifier type of the entity.</typeparam>
public interface IBaseEntity<TKey>
    where TKey : IEquatable<TKey>
{
    /// <summary>
    /// Gets or sets the unique identifier of the entity.
    /// </summary>
    public TKey Id { get; set; }
}
