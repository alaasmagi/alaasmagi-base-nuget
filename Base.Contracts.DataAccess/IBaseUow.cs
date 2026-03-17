namespace Base.Contracts.DataAccess;

/// <summary>
/// Defines a unit-of-work abstraction for the data access layer.
/// </summary>
public interface IBaseUow
{
    /// <summary>
    /// Persists all pending changes for the current data access unit of work.
    /// </summary>
    /// <returns>
    /// A task that resolves to the number of state entries written to the underlying store.
    /// </returns>
    public Task<int> SaveChangesAsync();
}
