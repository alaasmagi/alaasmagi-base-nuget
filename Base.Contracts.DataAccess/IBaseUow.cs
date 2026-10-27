using Base.Contracts.DTO;

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

    /// <summary>
    /// Persists all pending changes and reports a rejected write (for example a unique, foreign-key or
    /// concurrency violation) as a failed response instead of throwing. After a failure the pending changes are
    /// discarded so the unit of work can be reused.
    /// </summary>
    /// <returns>
    /// A task that resolves to the number of state entries written, or a failure describing why the write was rejected.
    /// </returns>
    public Task<IMethodResponse<int>> TrySaveChangesAsync();
}
