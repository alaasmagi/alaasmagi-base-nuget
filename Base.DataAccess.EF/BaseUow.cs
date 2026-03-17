using Base.Contracts.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Base.DataAccess.EF;

/// <summary>
/// Provides a base Entity Framework unit-of-work implementation backed by a <see cref="DbContext"/>.
/// </summary>
/// <typeparam name="TDbContext">The concrete database context type.</typeparam>
public class BaseUow<TDbContext> : IBaseUow
    where TDbContext : DbContext
{
    /// <summary>
    /// Stores the database context used by the unit of work.
    /// </summary>
    protected readonly TDbContext UowDbContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseUow{TDbContext}"/> class.
    /// </summary>
    /// <param name="uowDbContext">The database context used to persist changes.</param>
    public BaseUow(TDbContext uowDbContext)
    {
        UowDbContext = uowDbContext;
    }

    /// <summary>
    /// Persists all pending changes in the underlying database context.
    /// </summary>
    /// <returns>
    /// A task that resolves to the number of state entries written to the database.
    /// </returns>
    public async Task<int> SaveChangesAsync()
    {
        return await UowDbContext.SaveChangesAsync();
    }
}
