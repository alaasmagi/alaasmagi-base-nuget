namespace Base.Contracts.Domain;

/// <summary>
/// Defines a soft-delete flag for an entity.
/// </summary>
public interface IBaseEntitySoftDelete
{
    /// <summary>
    /// Gets or sets a value indicating whether the entity is soft deleted.
    /// </summary>
    public bool IsDeleted { get; set; }
}
