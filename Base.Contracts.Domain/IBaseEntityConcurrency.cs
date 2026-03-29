namespace Base.Contracts.Domain;

/// <summary>
/// Defines a repository-managed concurrency token for optimistic concurrency checks.
/// </summary>
public interface IBaseEntityConcurrency
{
    /// <summary>
    /// Gets or sets the current concurrency token value used to detect stale updates.
    /// </summary>
    public string ConcurrencyToken { get; set; }
}
