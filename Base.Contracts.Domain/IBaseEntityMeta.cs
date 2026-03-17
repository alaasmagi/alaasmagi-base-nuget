namespace Base.Contracts.Domain;

/// <summary>
/// Defines metadata fields that capture creation and update information for an entity.
/// </summary>
public interface IBaseEntityMeta
{
    /// <summary>
    /// Gets or sets the identifier of the actor who created the entity.
    /// </summary>
    public string CreatedBy { get; set; }

    /// <summary>
    /// Gets or sets the UTC timestamp when the entity was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the actor who last updated the entity.
    /// </summary>
    public string UpdatedBy { get; set; }

    /// <summary>
    /// Gets or sets the UTC timestamp when the entity was last updated.
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}
