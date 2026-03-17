namespace Base.Contracts.Application;

/// <summary>
/// Defines a unit-of-work abstraction for the application layer.
/// </summary>
public interface IBaseServiceUow
{
    /// <summary>
    /// Persists all pending changes for the current application service unit of work.
    /// </summary>
    /// <returns>
    /// A task that resolves to the number of state entries written to the underlying store.
    /// </returns>
    public Task<int> SaveChangesAsync();
}
