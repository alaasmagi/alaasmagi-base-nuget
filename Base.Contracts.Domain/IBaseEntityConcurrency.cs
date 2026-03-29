namespace Base.Contracts.Domain;

/// <summary>
/// Defines a concurrency token for optimistic concurrency checks.
/// </summary>
public interface IBaseEntityConcurrency
{
    /// <summary>
    /// Gets or sets the current concurrency token.
    /// </summary>
    public string ConcurrencyToken { get; set; }
}