using Base.Contracts.Domain;

namespace Base.Contracts.DTO;

/// <summary>
/// Defines a mapper between two entity representations that use <see cref="Guid"/> as the key type.
/// </summary>
/// <typeparam name="TUpperEntity">The source or target entity type used in the upper layer.</typeparam>
/// <typeparam name="TLowerEntity">The source or target entity type used in the lower layer.</typeparam>
public interface IMapper<TUpperEntity, TLowerEntity> : IMapper<TUpperEntity, TLowerEntity, Guid>
    where TUpperEntity : class
    where TLowerEntity : class
{
}

/// <summary>
/// Defines a mapper between two entity representations.
/// </summary>
/// <typeparam name="TUpperEntity">The source or target entity type used in the upper layer.</typeparam>
/// <typeparam name="TLowerEntity">The source or target entity type used in the lower layer.</typeparam>
/// <typeparam name="TKey">The identifier type associated with the mapped entities.</typeparam>
public interface IMapper<TUpperEntity, TLowerEntity, TKey>
    where TKey : IEquatable<TKey>
    where TUpperEntity : class
    where TLowerEntity : class
{
    /// <summary>
    /// Maps a lower-layer entity to an upper-layer entity.
    /// </summary>
    /// <param name="entity">The lower-layer entity to map.</param>
    /// <returns>
    /// The mapped upper-layer entity, or <see langword="null"/> when the source entity is <see langword="null"/> or cannot be mapped.
    /// </returns>
    public TUpperEntity? Map(TLowerEntity? entity);

    /// <summary>
    /// Maps an upper-layer entity to a lower-layer entity.
    /// </summary>
    /// <param name="entity">The upper-layer entity to map.</param>
    /// <returns>
    /// The mapped lower-layer entity, or <see langword="null"/> when the source entity is <see langword="null"/> or cannot be mapped.
    /// </returns>
    public TLowerEntity? Map(TUpperEntity? entity);
}
