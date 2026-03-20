using Base.Contracts.Application;
using Base.Contracts.DataAccess;

namespace Base.Application;

/// <summary>
/// Provides a base application-layer unit-of-work implementation that delegates persistence to a data access unit of work.
/// </summary>
/// <typeparam name="TUow">The concrete data access unit-of-work type.</typeparam>
public class BaseServiceUow<TUow> : IBaseServiceUow
    where TUow : IBaseUow
{
    /// <summary>
    /// Stores the underlying data access unit of work.
    /// </summary>
    protected readonly TUow Uow;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseServiceUow{TUow}"/> class.
    /// </summary>
    /// <param name="uow">The underlying data access unit of work.</param>
    public BaseServiceUow(TUow uow)
    {
        Uow = uow;
    }

    /// <summary>
    /// Persists all pending changes using the underlying data access unit of work.
    /// </summary>
    /// <returns>
    /// A task that resolves to the number of state entries written to the underlying store.
    /// </returns>
    public virtual async Task<int> SaveChangesAsync()
    {
        return await Uow.SaveChangesAsync();
    }
}
